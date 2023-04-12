using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Core;
using outRp.Globals;
using outRp.Models;
using outRp.OtherSystem.NativeUi;
using outRp.OtherSystem.Textlabels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;

namespace outRp.Props
{

    public class BusinessSellModel
    {
        public int senderId { get; set; }
        public int targetId { get; set; }
        public int businessId { get; set; }
        public int price { get; set; }
    }
    public class BusinessInvıteModel
    {
        public int senderId { get; set; }
        public int targetId { get; set; }
        public int businessId { get; set; }
    }
    public class CarMods
    {
        public int Id { get; set; }
        public int Value { get; set; }
    }
    public class Tuning
    {
        public VehModel veh { get; set; }
        public List<CarMods> mods { get; set; }
        public int StockCost { get; set; }
    }


    public class Business : IScript
    {
        public static double systemSell = 2;

        [Command("setbizsellmp")]
        public static void COM_HouseSellMultipler(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 6) { MainChat.SendErrorChat(p, "[错误] 无权操作."); return; }
            if (args.Length <= 0) { MainChat.SendErrorChat(p, "[用法] /sethousesellmp [数值]<br>例如: /sethousesellmp 1.2 = %80"); return; }
            if (!double.TryParse(args[0], out double mult)) { MainChat.SendErrorChat(p, "[用法] /sethousesellmp [数值]<br>例如: /sethousesellmp 1.2 = %80"); return; }

            systemSell = mult;

            MainChat.SendInfoChat(p, "[=] 已更新.");
            return;
        }

        public static int TotalBusiness = 0;
        #region SETUP BUSINESS ON SERVER
        public static List<BusinessSellModel> serverSellBusiness = new List<BusinessSellModel>();
        public static List<BusinessInvıteModel> businessInvte = new List<BusinessInvıteModel>();
        public static List<Tuning> vehicleTuningList = new List<Tuning>();

        public static async Task LoadAllBusiness() // Main Func
        {
            LoadTypeSystems();
            List<BusinessModel> serverBusiness = await Database.DatabaseMain.GetAllServerBusiness();

            foreach (BusinessModel biz in serverBusiness)
            {
                /*PlayerLabel bizLabel = TextLabelStreamer.Create(setupBusinessName(biz), biz.position, 0, true, streamRange: 5);
                bizLabel.SetData(EntityData.EntranceTypes.EntranceType, EntityData.EntranceTypes.Business);
                bizLabel.SetData(EntityData.GeneralSetting.DataType, EntityData.GeneralSetting.TypeBusiness);
                bizLabel.SetData(EntityData.BusinessEntityData.BusinessId, biz.ID);
                bizLabel.SetData(EntityData.BusinessEntityData.BusinessInfo, JsonConvert.SerializeObject(biz));
                bizLabel.SetData(EntityData.BusinessEntityData.BusinessLabel, bizLabel.Id);
                PlayerLabel labelExit = TextLabelStreamer.Create("Çıkmak için [~g~E~w~] basınız.", biz.interiorPosition, biz.dimension, true, new Rgba(255, 255, 255, 255), streamRange: 2);
                labelExit.SetData(EntityData.EntranceTypes.EntranceType, EntityData.EntranceTypes.ExitWorld);
                labelExit.SetData(EntityData.EntranceTypes.ExitWorldPosition, biz.position);
                bizLabel.SetData(EntityData.BusinessEntityData.interiorLabelId, labelExit.Id); */

                Marker bizMark = MarkerStreamer.Create((MarkerTypes)biz.settings.MarkerType, biz.position, biz.settings.markerScale, dimension: 0, color: biz.settings.MarkerColor, streamRange: 5);
                bizMark.DisplayName = await setupBusinessName(biz);
                bizMark.isBusinessMarker = true;
                bizMark.SetData(EntityData.EntranceTypes.EntranceType, EntityData.EntranceTypes.Business);
                bizMark.SetData("Biz:ID", biz.ID);
                PlayerLabel labelExit = TextLabelStreamer.Create("按 [~g~E键~w~] 离开", biz.interiorPosition, biz.dimension, true, new Rgba(255, 255, 255, 255), streamRange: 2);
                labelExit.SetData(EntityData.EntranceTypes.EntranceType, EntityData.EntranceTypes.ExitWorld);
                labelExit.SetData(EntityData.EntranceTypes.ExitWorldPosition, biz.position);
                bizMark.SetData(EntityData.BusinessEntityData.interiorLabelId, labelExit.Id);

                if (biz.settings.tv.hasTv)
                {
                    biz.settings.tv.TvPropID = PropStreamer.Create("vw_prop_vw_cinema_tv_01", biz.settings.tv.Position, biz.settings.tv.Rotation, biz.dimension, frozen: true).Id;
                    await biz.Update(bizMark, labelExit);
                }
            }
            TotalBusiness = serverBusiness.Count;
            Alt.Log($"加载 产业数量: {serverBusiness.Count}");
        }
        #endregion

        public static async Task<string> setupBusinessName(BusinessModel biz)
        {
            string entrance = "";
            if (biz.entrancePrice <= 0) { entrance = "~g~营业"; } else { entrance = "~g~营业~n~~b~入场费: ~w~" + biz.entrancePrice + "~g~$"; }
            string lockedString = (biz.isLocked) ? "~r~打烊" : entrance;
            if (biz.settings.TotalTax >= (biz.price / 100) * 10)
                lockedString = "~r~密封!";
            string str = "~b~[~w~" + biz.name + "~w~, 地址编号:" + biz.ID + "~b~]~n~~b~" + lockedString;

            return str;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>BusinesModel: İnfo - PlayerLabel : Biz Label - Playerlabel : İnterior Label</returns>
        public static async Task<(BusinessModel, Marker, PlayerLabel)> getBusinessById(int Id)
        {
            BusinessModel h = await Database.DatabaseMain.GetBusinessInfo(Id);
            Marker l = null;
            foreach (Marker x in MarkerStreamer.GetAllDynamicMarkers())
            {
                x.TryGetData("Biz:ID", out int ID);
                if (ID == Id)
                {
                    l = x;
                    break;
                }
            }

            PlayerLabel m = null;
            if (l.TryGetData(EntityData.BusinessEntityData.interiorLabelId, out ulong txtlblID))
            {
                m = TextLabelStreamer.GetDynamicTextLabel(txtlblID);
            }


            return (h, l, m);
        }

        public static async Task<(BusinessModel, Marker, PlayerLabel)> getNearestBusiness(PlayerModel p)
        {
            Marker l = null;
            foreach (Marker x in MarkerStreamer.GetAllDynamicMarkers())
            {
                if (p.Position.Distance(x.Position) < 2) { l = x; break; }
            }
            if (l == null)
                return (null, null, null);


            BusinessModel h = null;
            if (l.TryGetData("Biz:ID", out int BizID))
            {
                h = await Database.DatabaseMain.GetBusinessInfo(BizID);
            }

            PlayerLabel m = null;
            if (l.TryGetData(EntityData.BusinessEntityData.interiorLabelId, out ulong interiorlbl))
            {
                m = TextLabelStreamer.GetDynamicTextLabel(interiorlbl);
            }

            return (h, l, m);
        }

        public static async Task<(BusinessModel, Marker, PlayerLabel)> getBusinessFromPos(Position pos)
        {
            Marker l = null;

            foreach (Marker x in MarkerStreamer.GetAllDynamicMarkers())
            {
                if (pos.Distance(x.Position) < 2)
                {
                    l = x;
                    break;
                }
            }

            BusinessModel h = null;
            if (l.TryGetData("Biz:ID", out int BizID))
            {
                h = await Database.DatabaseMain.GetBusinessInfo(BizID);
            }

            PlayerLabel m = null;
            if (l.TryGetData(EntityData.BusinessEntityData.interiorLabelId, out ulong interiorlbl))
            {
                m = TextLabelStreamer.GetDynamicTextLabel(interiorlbl);
            }

            return (h, l, m);
        }

        public static (BusinessModel, Marker, PlayerLabel) getBusinessFromPos(Position pos, int dimension, int range = 20)
        {
            Marker l = null;

            foreach (Marker x in MarkerStreamer.GetAllDynamicMarkers())
            {
                if (pos.Distance(x.Position) < range && x.Dimension == dimension)
                {
                    l = x;
                    break;
                }
            }

            BusinessModel h = null;

            PlayerLabel m = null;
            if (l.TryGetData(EntityData.BusinessEntityData.interiorLabelId, out ulong interiorlbl))
            {
                m = TextLabelStreamer.GetDynamicTextLabel(interiorlbl);
            }

            return (h, l, m);
        }

        public static async Task BusinessCreate(IPlayer player, int type, int price, string name)
        {
            if (type == 0) { MainChat.SendErrorChat(player, CONSTANT.COM_BusinessCreateDesc); return; }
            BusinessModel biz = new BusinessModel();
            biz.name = name;
            biz.type = type;
            biz.price = price;
            biz.position = player.Position;
            int bizId = await biz.Create();
            biz.ID = bizId;

            MainChat.SendErrorChat(player, biz.ID + " 产业已创建.");
            return;
        }
        public static async Task BusinessUpdateInfo(PlayerModel player, params string[] args)
        {
            if (args.Length <= 1) { MainChat.SendErrorChat(player, CONSTANT.COM_BusinessUpdateDesc); return; }
            int ID = Int32.Parse(args[0]);
            string type = args[1];
            //string value = args[2];
            (BusinessModel, Marker, PlayerLabel) biz = await getBusinessById(ID);

            if (type == string.Empty) { MainChat.SendErrorChat(player, CONSTANT.COM_BusinessUpdateDesc); return; }
            if (biz.Item1 == null) { MainChat.SendErrorChat(player, "无效产业ID!"); return; }

            switch (type)
            {
                case "name":
                    biz.Item1.name = string.Join(" ", args[2..]);
                    await biz.Item1.Update(biz.Item2, biz.Item3);
                    break;

                case "price":
                    Int32.TryParse(args[2], out int newprice);
                    biz.Item1.price = newprice;
                    await biz.Item1.Update(biz.Item2, biz.Item3);
                    break;

                case "lock":
                    biz.Item1.isLocked = !biz.Item1.isLocked;
                    await biz.Item1.Update(biz.Item2, biz.Item3);
                    break;

                case "owner":
                    Int32.TryParse(args[2], out int newOwner);
                    biz.Item1.ownerId = newOwner;
                    await biz.Item1.Update(biz.Item2, biz.Item3);
                    break;

                case "type":
                    Int32.TryParse(args[2], out int newType);
                    biz.Item1.type = newType;
                    await biz.Item1.Update(biz.Item2, biz.Item3);
                    break;

                case "vault":
                    Int32.TryParse(args[2], out int newVault);
                    biz.Item1.vault = newVault;
                    await biz.Item1.Update(biz.Item2, biz.Item3);
                    break;

                case "vw":
                    Int32.TryParse(args[2], out int newDimension);
                    biz.Item1.dimension = newDimension;
                    await biz.Item1.Update(biz.Item2, biz.Item3);
                    break;

                case "interior":
                    biz.Item1.interiorPosition = player.Position;
                    await biz.Item1.Update(biz.Item2, biz.Item3);
                    break;

                case "pos":
                    biz.Item1.position = player.Position;
                    await biz.Item1.Update(biz.Item2, biz.Item3);
                    break;

                case "markertype":
                    if (!Int32.TryParse(args[2], out int newMarkerType))
                        return;

                    if (newMarkerType > 43)
                        return;

                    biz.Item1.settings.MarkerType = newMarkerType;
                    await biz.Item1.Update(biz.Item2, biz.Item3);
                    return;

                case "markerscale":
                    if (args.Length < 4)
                        return;

                    if (!float.TryParse(args[2], out float ns1) || !float.TryParse(args[3], out float ns2) || !float.TryParse(args[4], out float ns3))
                        return;

                    biz.Item1.settings.markerScale = new Vector3(ns1, ns2, ns3);
                    await biz.Item1.Update(biz.Item2, biz.Item3);
                    return;

                case "markercolor":
                    if (args.Length < 5)
                        return;

                    if (!Int32.TryParse(args[2], out int nc1) || !Int32.TryParse(args[3], out int nc2) || !Int32.TryParse(args[4], out int nc3) || !Int32.TryParse(args[5], out int nc4))
                        return;

                    biz.Item1.settings.MarkerColor = new Rgba((byte)nc1, (byte)nc2, (byte)nc3, (byte)nc4);
                    await biz.Item1.Update(biz.Item2, biz.Item3);
                    return;

                case "interiorset":
                    if (args[2] == null)
                        return;

                    biz.Item1.settings.businessInterior = args[2];
                    await biz.Item1.Update(biz.Item2, biz.Item3);
                    MainChat.SendInfoChat(player, "[!] 已调整产业内饰.");
                    return;

                case "interiorplus":
                    if (args[2] == null)
                        return;

                    BusinessModel.BizSettings.interiorSettings newInteriorPlus = new BusinessModel.BizSettings.interiorSettings() { entitySet = args[2] };
                    biz.Item1.settings.businessInteriorSettings.Add(newInteriorPlus);
                    await biz.Item1.Update(biz.Item2, biz.Item3);

                    MainChat.SendInfoChat(player, "[!] 已调整产业内饰.");
                    return;

                case "interiorplustemizle":
                    biz.Item1.settings.businessInteriorSettings.Clear();
                    await biz.Item1.Update(biz.Item2, biz.Item3);

                    MainChat.SendInfoChat(player, "[!] 已调整产业内饰.");
                    return;

                case "intpos":
                    if (args.Length < 4)
                        return;

                    if (!float.TryParse(args[2], out float np1) || !float.TryParse(args[3], out float np2) || !float.TryParse(args[4], out float np3))
                        return;

                    biz.Item1.interiorPosition = new Position(np1, np2, np3);
                    await biz.Item1.Update(biz.Item2, biz.Item3);
                    MainChat.SendInfoChat(player, "[!] 产业内饰坐标更新为: " + np1.ToString() + "-" + np2.ToString() + "-" + np3.ToString());
                    return;

                case "ptakim1":
                    biz.Item1.settings.paintBall.team1_pos = player.Position;
                    await biz.Item1.Update(biz.Item2, biz.Item3);
                    MainChat.SendInfoChat(player, "Okey");
                    return;

                case "ptakim2":
                    biz.Item1.settings.paintBall.team2_pos = player.Position;
                    await biz.Item1.Update(biz.Item2, biz.Item3);
                    MainChat.SendInfoChat(player, "Okey");
                    return;

                case "pbekleme":
                    biz.Item1.settings.paintBall.waitingRoom = player.Position;
                    await biz.Item1.Update(biz.Item2, biz.Item3);
                    MainChat.SendInfoChat(player, "Okey");
                    return;

                case "psilah":
                    if (args.Length < 2)
                        return;

                    if (!uint.TryParse(args[2], out uint wep)) { return; }
                    biz.Item1.settings.paintBall.currentWeapon = wep;
                    await biz.Item1.Update(biz.Item2, biz.Item3);
                    MainChat.SendInfoChat(player, "Okey");
                    return;

                case "time":
                    if (args.Length < 2)
                        return;

                    if (!Int32.TryParse(args[2], out int intTime))
                        return;

                    biz.Item1.settings.interiorTime = intTime;
                    await biz.Item1.Update(biz.Item2, biz.Item3);

                    MainChat.SendInfoChat(player, "[=] 已更新内饰时间.");
                    return;

                case "totaltax":
                    if (args.Length < 2)
                        return;

                    if (!Int32.TryParse(args[2], out int newTotalTax))
                        return;

                    biz.Item1.settings.TotalTax = newTotalTax;
                    await biz.Item1.Update(biz.Item2, biz.Item3);
                    MainChat.SendInfoChat(player, "[=] 已更新产业总计税收.");
                    return;

                case "tax":
                    if (args.Length < 2)
                        return;

                    if (!Int32.TryParse(args[2], out int newTax))
                        return;

                    biz.Item1.settings.Tax = newTax;
                    await biz.Item1.Update(biz.Item2, biz.Item3);

                    MainChat.SendInfoChat(player, "[=] 已更新产业税.");
                    return;

                case "floattext":
                    biz.Item1.settings.floatText = !biz.Item1.settings.floatText;
                    await biz.Item1.Update(biz.Item2, biz.Item3);

                    MainChat.SendInfoChat(player, "[=] 已更新产业浮动文本为: " + biz.Item1.settings.floatText.ToString());
                    return;


                default:
                    MainChat.SendErrorChat(player, CONSTANT.COM_BusinessUpdateDesc);
                    return;
            }

            //MainChat.SendErrorChat(player, "başarıyla güncellendi");
            return;
        }
        public static async Task BusinessDelete(IPlayer player, int ID)
        {
            (BusinessModel, Marker, PlayerLabel) biz = await getBusinessById(ID);

            if (biz.Item1 == null) { MainChat.SendErrorChat(player, "无效产业!"); return; }

            await biz.Item1.Delete(biz.Item2, biz.Item3);

            MainChat.SendErrorChat(player, "已删除产业");
            return;
        }
        /// <summary>
        /// Kişi üzerine kayıtlı olan iş yerini istediğiniz çeşit üzerinden sorgular ve true/false değeri yollar.
        /// </summary>
        public static async Task<bool> GetPlayerBusinessType(PlayerModel p, int bizType)
        {
            if (p.businessStaff > 0)
            {
                BusinessModel b = await Database.DatabaseMain.GetBusinessInfo(p.businessStaff);
                if (b.type == bizType) { return true; }
            }
            List<BusinessModel> Bizs = await Database.DatabaseMain.GetMemberBusinessList(p);
            foreach (BusinessModel x in Bizs)
            {
                if (x.type == bizType) { return true; }
            }
            return false;
        }

        public static async Task<bool> CheckBusinessKey(PlayerModel p, BusinessModel b)
        {
            bool result = false;
            if (b.ownerId == p.sqlID) { return true; }
            if (b.ID == p.businessStaff) { return true; }
            if(b.settings.Admins.Contains(p.sqlID)) { return true; }
            List<Database.DatabaseMain.KeyModel> keys = await Database.DatabaseMain.getBusinessKeys(b.ID);

            foreach (Database.DatabaseMain.KeyModel x in keys)
            {
                if (x.keyOwner == p.sqlID) { return true; }
            }

            return result;
        }

        public static async Task BusinessTax()
        {
            if (!ServerGlobalValues.serverCanTax)
                return;
            List<int> IDS = await Database.DatabaseMain.getAllBusinesIDS();
            foreach (int ID in IDS)
            {
                var biz = await getBusinessById(ID);
                if (biz.Item1 == null)
                    continue;

                if (biz.Item1.price >= 20000)
                {
                    biz.Item1.settings.TotalTax += (biz.Item1.settings.Tax);
                    if (biz.Item1.settings.TotalTax >= (biz.Item1.price / 100) * 60)
                    {
                        biz.Item1.ownerId = 0;
                        biz.Item1.isLocked = true;
                        biz.Item1.vault = 0;
                        biz.Item1.name = "由银行出售!";
                    }
                    await biz.Item1.Update(biz.Item2, biz.Item3);
                }
            }
            MainChat.SendAdminChat("[经济] 产业税已更新.");
            return;
        }


        [Command(CONSTANT.COM_BuyBusiness)]
        public static async Task COM_BuyBusiness(PlayerModel player)
        {
            if (player.adminLevel <= 6) { MainChat.SendErrorChat(player, "[错误] 无权操作."); return; }
            (BusinessModel, Marker, PlayerLabel) biz = await getNearestBusiness(player);
            if (biz.Item1 == null) { MainChat.SendErrorChat(player, "[错误] 附近没有产业."); return; }

            if (biz.Item1.ownerId > 0) { MainChat.SendErrorChat(player, "[错误] 此产业已被购买了."); return; }

            //CharacterModel playerChar = GlobalEvents.PlayerGetData(player);

            if (player.cash < biz.Item1.price) { MainChat.SendErrorChat(player, "[错误] 您没有足够的钱."); return; }

            player.cash -= biz.Item1.price;
            biz.Item1.ownerId = player.sqlID;
            await player.updateSql();
            await biz.Item1.Update(biz.Item2, biz.Item3);

            MainChat.SendInfoChat(player, biz.Item1.name + " 被您成功购买.");
            return;
        }

        [Command(CONSTANT.COM_SellBusiness)]
        public static async Task COM_SellBusiness(PlayerModel player, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendErrorChat(player, CONSTANT.COM_SellBusinessDesc); return; }
            //string select, int? id, int? price
            string select = args[0];


            (BusinessModel, Marker, PlayerLabel) biz = await getNearestBusiness(player);

            if (biz.Item1 == null) { MainChat.SendErrorChat(player, "[错误] 附近没有产业."); return; }




            switch (select)
            {
                case "man":
                    // ! VERGİ SİSTEMİ !
                    if (biz.Item1.settings.TotalTax > 100) { MainChat.SendErrorChat(player, "[错误] 暂时无法出售您的产业, 因为还未支付税款."); return; }


                    if (biz.Item1.ownerId != player.sqlID) { MainChat.SendErrorChat(player, "[错误] 此产业不属于您!"); return; }
                    int id; bool isIDOk = Int32.TryParse(args[1], out id);
                    int price; bool isPriceOk = Int32.TryParse(args[2], out price);

                    if (!isIDOk || !isPriceOk) { MainChat.SendErrorChat(player, "[错误] 无效金额."); return; }

                    if (price <= 0) { MainChat.SendErrorChat(player, "[错误] 无效金额."); return; }
                    if (biz.Item1.ownerId != player.sqlID) { MainChat.SendErrorChat(player, "[错误] 此产业不属于您."); return; }
                    if (id == 0) { MainChat.SendErrorChat(player, CONSTANT.COM_SellBusinessDesc); return; }
                    PlayerModel target = GlobalEvents.GetPlayerFromSqlID((int)id);
                    //CharacterModel targetChar = GlobalEvents.PlayerGetData(target);

                    if (target == null) { MainChat.SendErrorChat(player, $"[错误] {id} 无效玩家. "); return; }
                    if (target.Position.Distance(player.Position) > 10) { MainChat.SendErrorChat(player, "[错误] 您离指定玩家太远."); return; }
                    var senderCheck = serverSellBusiness.Find(x => x.senderId == player.sqlID);
                    if (senderCheck != null) { MainChat.SendErrorChat(player, "[错误] 您已经向指定玩家发送了您的产业出售请求, 请等待请求得到答复."); return; }
                    serverSellBusiness.Add(new BusinessSellModel { senderId = player.sqlID, targetId = target.sqlID, businessId = biz.Item1.ID, price = (int)price });
                    string color = "{49A0CD}";
                    MainChat.SendInfoChat(player, $"{color}[信息] 您向 {target.characterName} 发送了产业 {biz.Item1.name} 的出售请求, 您的报价: {price} . ");
                    MainChat.SendInfoChat(target, $"{color}[信息] {player.characterName} 向您发送了产业出售请求;<br> 产业名称: {biz.Item1.name} , 请求报价: {price}$ <br> 接受报价输入 /sellbiz acp");
                    break;

                case "system":
                    OtherSystem.NativeUi.Inputs.SendButtonInput(player, "是否以 $" + ((biz.Item1.price - biz.Item1.settings.TotalTax < 0) ? 0 : (int)((biz.Item1.price) / systemSell - biz.Item1.settings.TotalTax)) + " 的价格出售您的产业至系统?", "Business:SellToSystem", "123," + biz.Item1.ID);
                    /*if (biz.Item1.ownerId != player.sqlID) { MainChat.SendErrorChat(player, "[错误] 您没有此产业的钥匙."); return; }
                    player.cash += biz.Item1.price / 2;
                    biz.Item1.ownerId = 0;
                    biz.Item1.name = "SATILIK";
                    biz.Item1.settings.Env = "[]";
                    biz.Item1.isLocked = true;
                    biz.Item1.entrancePrice = 0;
                    biz.Item1.Update(biz.Item2, biz.Item3);
                    Database.DatabaseMain.updateBusinessKeys(biz.Item1.ID, new List<Database.DatabaseMain.KeyModel>());
                    Database.DatabaseMain.ResetBusinessStafs(biz.Item1.ID);
                    player.updateSql();

                    MainChat.SendInfoChat(player, "[Bilgi] İş yerini başarıyla sisteme sattınız.");*/
                    break;

                case "acp":
                    var businessSellItem = serverSellBusiness.Find(x => x.targetId == player.sqlID);

                    if (businessSellItem == null) { MainChat.SendErrorChat(player, "[错误] 无效出售请求."); return; }
                    if (player.cash < businessSellItem.price) { MainChat.SendErrorChat(player, "[错误] 您没有足够的钱."); return; }

                    PlayerModel kabulTarget = GlobalEvents.GetPlayerFromSqlID(businessSellItem.senderId);
                    if (kabulTarget == null) { MainChat.SendErrorChat(player, "[错误] 出售玩家离线了."); return; }
                    if (biz.Item1.ownerId != kabulTarget.sqlID) { MainChat.SendErrorChat(player, "[错误] 此产业不属于对方!"); return; }
                    //CharacterModel kabulTargetChar = GlobalEvents.PlayerGetData(kabulTarget);
                    //if (kabultar == null) { MainChat.SendErrorChat(player, "[错误] Bir hata meydana geldi."); return; }

                    player.cash -= businessSellItem.price;
                    kabulTarget.cash += businessSellItem.price;
                    biz.Item1.ownerId = player.sqlID;
                    await player.updateSql();
                    await kabulTarget.updateSql();
                    await biz.Item1.Update(biz.Item2, biz.Item3);

                    List<Database.DatabaseMain.KeyModel> hKeys = new List<Database.DatabaseMain.KeyModel>();
                    await Database.DatabaseMain.updateBusinessKeys(biz.Item1.ID, hKeys);

                    MainChat.SendInfoChat(player, "{6DA560}" + biz.Item1.name + " 接受了您的报价: " + businessSellItem.price);
                    MainChat.SendInfoChat(kabulTarget, "{6DA560} 恭喜您, 购买了 " + biz.Item1.name + " , 价格: " + businessSellItem.price);
                    MainChat.SendAdminChat("{#B2CB87}[信息] " + kabulTarget.characterName + " 购买了产业编号 " + biz.Item1.ID + " 属于 " + player.characterName + " 的产业, 价格: " + businessSellItem.price);
                    serverSellBusiness.Remove(businessSellItem);

                    break;

                default:
                    MainChat.SendErrorChat(player, CONSTANT.COM_SellBusinessDesc);
                    break;
            }
            return;
        }

        [AsyncClientEvent("Business:SellToSystem")]
        public async Task EVENT_Business_Sell_To_System(PlayerModel p, bool selection, string otherVal)
        {
            if (p.Ping > 250)
                return;
            if (selection)
            {
                string[] val = otherVal.Split(',');
                if (!Int32.TryParse(val[1], out int bizID)) { MainChat.SendErrorChat(p, "[错误] 无效产业ID."); return; }
                var biz = await getBusinessById(bizID);
                if (biz.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效产业ID!"); return; }
                if (biz.Item1.ownerId != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 此产业不属于您."); return; }
                p.cash += ((biz.Item1.price - biz.Item1.settings.TotalTax < 0) ? 0 : (int)((biz.Item1.price) / systemSell - biz.Item1.settings.TotalTax));
                biz.Item1.ownerId = 0;
                biz.Item1.name = "出售";
                biz.Item1.settings.Env = "[]";
                biz.Item1.isLocked = true;
                biz.Item1.entrancePrice = 0;
                biz.Item1.settings.Admins = new();
                await biz.Item1.Update(biz.Item2, biz.Item3);
                await Database.DatabaseMain.updateBusinessKeys(biz.Item1.ID, new List<Database.DatabaseMain.KeyModel>());
                await Database.DatabaseMain.ResetBusinessStafs(biz.Item1.ID);
                await p.updateSql();

                MainChat.SendAdminChat("{#B2CB87}[信息] " + p.characterName + " 出售产业 " + biz.Item1.ID + " 至系统.");
                MainChat.SendInfoChat(p, "[信息] 您出售了您的产业.");
                return;
            }
            else
            {
                MainChat.SendInfoChat(p, "[信息] 您取消了出售.");
                return;
            }
        }

        [Command(CONSTANT.COM_BusinessLock)]
        public static async Task COM_BusinessLock(PlayerModel player)
        {
            if (player.Ping > 250)
                return;
            (BusinessModel, Marker, PlayerLabel) biz = await getNearestBusiness(player);
            //CharacterModel playerChar = GlobalEvents.PlayerGetData(player);

            if (biz.Item1 == null)
            {
                PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Find(x => player.Position.Distance(x.Position) < 3f && x.Dimension == player.Dimension);
                if (entranceLabel == null) { MainChat.SendErrorChat(player, "[错误] 无效产业."); return; }
                entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position bussinesPos);

                biz = await getBusinessFromPos(bussinesPos);

                if (biz.Item1 == null) { MainChat.SendErrorChat(player, "[错误] 无效产业."); return; }
            }

            // ! VERGİ SİSTEMİ !
            if (biz.Item1.settings.TotalTax > (biz.Item1.price / 10)) { MainChat.SendErrorChat(player, "[错误] 此产业是密封的(被银行征收)."); return; }

            if (await CheckBusinessKey(player, biz.Item1) == false) { MainChat.SendErrorChat(player, CONSTANT.ERR_BusinessOwnerNot); return; }

            biz.Item1.isLocked = !biz.Item1.isLocked;
            string infoMessage = (biz.Item1.isLocked) ? "已上锁产业." : "已解锁产业.";
            await biz.Item1.Update(biz.Item2, biz.Item3);

            MainChat.SendInfoChat(player, infoMessage);
            return;
        }

        [Command("setbgps")]
        public static async Task COM_BusinessGps(PlayerModel player)
        {
            if (player.Ping > 250)
                return;
            (BusinessModel, Marker, PlayerLabel) biz = await getNearestBusiness(player);
            //CharacterModel playerChar = GlobalEvents.PlayerGetData(player);

            if (biz.Item1 == null)
            {
                PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Find(x => player.Position.Distance(x.Position) < 3f && x.Dimension == player.Dimension);
                if (entranceLabel == null) { MainChat.SendErrorChat(player, "[错误] 无效产业."); return; }
                entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position bussinesPos);

                biz = await getBusinessFromPos(bussinesPos);

                if (biz.Item1 == null) { MainChat.SendErrorChat(player, "[错误] 无效产业."); return; }
            }

            if (biz.Item1.ownerId != player.sqlID && !biz.Item1.settings.Admins.Contains(player.sqlID)) { MainChat.SendErrorChat(player, CONSTANT.ERR_BusinessOwnerNot); return; }

            // ! VERGİ SİSTEMİ !
            if (biz.Item1.settings.TotalTax > (biz.Item1.price / 10)) { MainChat.SendErrorChat(player, "[错误] 此产业是密封的(被银行征收)."); return; }

            biz.Item1.settings.gps = !biz.Item1.settings.gps;
            string infoMessage = "是否可使用bgps导航已: " + ((biz.Item1.settings.gps) ? "开启" : "关闭");
            await biz.Item1.Update(biz.Item2, biz.Item3);

            MainChat.SendInfoChat(player, infoMessage);
            return;
        }

        [Command(CONSTANT.COM_EditEntranceBusiness)]
        public static async Task COM_EditEntrancePrice(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, CONSTANT.DESC_EditEntranceBusiness); return; }
            if (!Int32.TryParse(args[0], out int entrancePrice)) { MainChat.SendInfoChat(p, CONSTANT.DESC_EditEntranceBusiness); return; }
            if (entrancePrice < 0) { MainChat.SendErrorChat(p, CONSTANT.ERR_ValueNotNegative); return; }
            if (entrancePrice > 200) { MainChat.SendErrorChat(p, "[错误] 入场费最大为200."); return; }
            var biz = await getNearestBusiness(p);
            if (biz.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 附近没有产业!"); return; }

            if (!await CheckBusinessKey(p, biz.Item1)) { MainChat.SendErrorChat(p, "[错误] 您没有此产业的钥匙!"); return; }

            // ! VERGİ SİSTEMİ !
            if (biz.Item1.settings.TotalTax > (biz.Item1.price / 10)) { MainChat.SendErrorChat(p, "[错误] 此产业是密封的(被银行征收)."); return; }

            biz.Item1.entrancePrice = entrancePrice;
            await biz.Item1.Update(biz.Item2, biz.Item3);
            MainChat.SendInfoChat(p, "> 已更新入场费为 $" + entrancePrice);
            return;
        }

        [Command(CONSTANT.COM_InviteBusiness)]
        public static async Task COM_InviteBusiness(PlayerModel player, params string[] values)
        {//
            if (values.Length <= 1) { MainChat.SendInfoChat(player, CONSTANT.DESC_InviteBusiness); return; }
            int bizId; bool isBizIdOK = Int32.TryParse(values[0], out bizId);
            int targetId; bool isTargetidOk = Int32.TryParse(values[1], out targetId);

            if (!isBizIdOK || !isTargetidOk) { MainChat.SendInfoChat(player, CONSTANT.DESC_InviteBusiness); return; }

            BusinessModel biz = await Database.DatabaseMain.GetBusinessInfo(bizId);
            if (biz == null) { MainChat.SendErrorChat(player, CONSTANT.ERR_BusinessNotFound); return; }
            if (biz.ownerId != player.sqlID && !biz.settings.Admins.Contains(player.sqlID)) { MainChat.SendErrorChat(player, CONSTANT.ERR_BusinessOwnerNot); return; }
            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(targetId);
            if (t == null) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotFound); return; }
            if (t.businessStaff > 0) { MainChat.SendErrorChat(player, string.Format(CONSTANT.ERR_PlayerInBusiness, t.characterName.Replace("_", " "))); return; }
            BusinessModel memberBuesinessList = await Database.DatabaseMain.GetBusinessInfo(player.businessStaff);
            if (t == player) { MainChat.SendErrorChat(player, string.Format(CONSTANT.ERR_PlayerInBusiness, t.characterName.Replace("_", " "))); return; }
            if (memberBuesinessList == null) { MainChat.SendErrorChat(player, string.Format(CONSTANT.ERR_PlayerInBusiness, t.characterName.Replace("_", " "))); return; }

            MainChat.SendInfoChat(t, string.Format(CONSTANT.INFO_InviteTargetInfo, player.characterName.Replace("_", " ")));
            MainChat.SendInfoChat(player, string.Format(CONSTANT.INFO_InvitePlayerInfo, t.characterName.Replace("_", " ")));
            businessInvte.Add(new BusinessInvıteModel { businessId = bizId, senderId = player.sqlID, targetId = t.sqlID });

            await Task.Delay(30000);
            var deleteInvite = businessInvte.Find(x => x.senderId == player.sqlID && x.targetId == t.sqlID);
            if (deleteInvite != null) { businessInvte.Remove(deleteInvite); return; }
            return;
        }

        [Command(CONSTANT.COM_InviteBusinessAccept)]
        public static void COM_InviteBusinessAccept(PlayerModel player)
        {
            var deleteInvıte = businessInvte.Find(x => x.targetId == player.sqlID);
            if (deleteInvıte == null) { MainChat.SendInfoChat(player, CONSTANT.ERR_BusinessInviteNotFound); return; }
            player.businessStaff = deleteInvıte.businessId;
            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(deleteInvıte.senderId);
            if (t != null) { MainChat.SendInfoChat(t, string.Format(CONSTANT.INFO_InviteBusinessAccepted, player.characterName.Replace("_", " "))); }
            MainChat.SendInfoChat(player, "> 您已接受工作邀请.");
            businessInvte.Remove(deleteInvıte);
            return;
        }

        [Command(CONSTANT.COM_KickBusiness)]
        public static async Task COM_KickBusiness(PlayerModel player, params string[] value)
        {
            if (value.Length <= 0) { MainChat.SendInfoChat(player, CONSTANT.DESC_KickBusiness); return; }

            int tID; bool isTIDok = Int32.TryParse(value[0], out tID);
            if (!isTIDok) { MainChat.SendInfoChat(player, CONSTANT.DESC_KickBusiness); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tID);
            if (t == null)
            {
                PlayerModelInfo tinfo = await Database.DatabaseMain.getCharacterInfo(tID);
                if (tinfo == null)
                {
                    MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotFound);
                    return;
                }

                BusinessModel bizInfo2 = await Database.DatabaseMain.GetBusinessInfo(tinfo.businessStaff);
                if (bizInfo2 == null)
                    return;

                if (bizInfo2.ownerId != player.sqlID && !bizInfo2.settings.Admins.Contains(player.sqlID)) { MainChat.SendErrorChat(player, t.sqlID + " " + CONSTANT.ERR_NotInYourBusiness); return; }
                tinfo.businessStaff = -1;
                tinfo.updateSql();

                return;

            }
            BusinessModel bizInfo = await Database.DatabaseMain.GetBusinessInfo(t.businessStaff);
            if (bizInfo == null)
                return;

            if (bizInfo.ownerId != player.sqlID) { MainChat.SendErrorChat(player, t.sqlID + " " + CONSTANT.ERR_NotInYourBusiness); return; }
            t.businessStaff = -1;
            await t.updateSql();

            MainChat.SendInfoChat(player, string.Format(CONSTANT.INFO_KickBusinessSuccesPlayer, t.characterName.Replace("_", " ")));
            MainChat.SendInfoChat(t, string.Format(CONSTANT.INFO_KickBusinessSuccesTarget, player.characterName.Replace("_", " ")));
            return;
        }

        [Command(CONSTANT.COM_GetMoneyInBusiness)]
        public static async Task COM_GetMoneyInBusiness(PlayerModel player, params string[] value)
        {
            if (value.Length <= 0) { MainChat.SendInfoChat(player, CONSTANT.DESC_GetMoneyInBusiness); return; }
            (BusinessModel, Marker, PlayerLabel) biz = await getNearestBusiness(player);
            if (biz.Item1 == null) { MainChat.SendErrorChat(player, "[错误] 无效产业."); return; }

            if (biz.Item1.ownerId != player.sqlID && !biz.Item1.settings.Admins.Contains(player.sqlID)) { MainChat.SendErrorChat(player, CONSTANT.ERR_BusinessOwnerNot); return; }
            int miktar; bool isMiktarOk = Int32.TryParse(value[0], out miktar);

            if (!isMiktarOk) { MainChat.SendInfoChat(player, CONSTANT.DESC_GetMoneyInBusiness); return; }

            if (miktar <= 0) { MainChat.SendErrorChat(player, CONSTANT.ERR_ValueNotNegative); return; }
            if (miktar > biz.Item1.vault) { MainChat.SendErrorChat(player, CONSTANT.ERR_MoneyNotEnoughtInBussines); return; }
            player.cash += miktar;
            biz.Item1.vault -= miktar;
            await biz.Item1.Update(biz.Item2, biz.Item3);
            await player.updateSql();

            MainChat.SendInfoChat(player, string.Format(CONSTANT.INFO_GetMoneyInBusinessSucces, biz.Item1.name, miktar));
            return;
        }

        [Command("bvault")]
        public static async Task COM_CheckBusinessVault(PlayerModel p)
        {
            (BusinessModel, Marker, PlayerLabel) biz = await getNearestBusiness(p);
            if (biz.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效产业."); return; }

            bool isOk = await CheckBusinessKey(p, biz.Item1);
            if (!isOk) { MainChat.SendErrorChat(p, "[错误] 您没有此产业的钥匙."); return; }

            MainChat.SendInfoChat(p, "目前产业收入: $" + biz.Item1.vault.ToString());
            return;
        }

        [Command("baddtv")]
        public async Task COM_AddTv(PlayerModel p)
        {
            PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Find(x => p.Position.Distance(x.Position) < 25f && x.Dimension == p.Dimension);
            if (entranceLabel == null) { MainChat.SendErrorChat(p, "[错误] 无效产业."); return; }
            entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position bussinesPos);

            var biz = await getBusinessFromPos(bussinesPos);

            if (biz.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效产业."); return; }

            if (!await CheckBusinessKey(p, biz.Item1)) { MainChat.SendErrorChat(p, "[错误] 您没有此产业的钥匙!"); return; }
            if (biz.Item1.settings.tv.hasTv) { MainChat.SendErrorChat(p, "[错误] 此产业已经有电视了, 请先删除原有电视."); return; }
            GlobalEvents.ShowObjectPlacement(p, "vw_prop_vw_cinema_tv_01", "Business:CreateTv");
            return;
        }

        [AsyncClientEvent("Business:CreateTv")]
        public async Task EVENT_CreateTV(PlayerModel p, string rot, string pos, string model)
        {
            Vector3 position = JsonConvert.DeserializeObject<Vector3>(pos);
            Vector3 rotation = JsonConvert.DeserializeObject<Vector3>(rot);

            PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Find(x => p.Position.Distance(x.Position) < 25f && x.Dimension == p.Dimension);
            if (entranceLabel == null) { MainChat.SendErrorChat(p, "[错误] 无效产业."); return; }
            entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position bussinesPos);

            var biz = await getBusinessFromPos(bussinesPos);

            if (biz.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效产业."); return; }

            if (!await CheckBusinessKey(p, biz.Item1)) { MainChat.SendErrorChat(p, "[错误] 您没有此产业的钥匙!"); return; }
            if (biz.Item1.settings.tv.hasTv) { MainChat.SendErrorChat(p, "[错误] 此产业已经有电视了, 请先删除原有电视."); return; }

            biz.Item1.settings.tv.hasTv = true;
            biz.Item1.settings.tv.Position = position;
            biz.Item1.settings.tv.Rotation = rotation;
            LProp tv = PropStreamer.Create(model, position, rotation, biz.Item1.dimension, false, false, true);
            biz.Item1.settings.tv.TvPropID = tv.Id;
            await biz.Item1.Update(biz.Item2, biz.Item3);
            MainChat.SendInfoChat(p, "[?] 已添加电视至产业, 输入 /btv 操作.");
            return;
        }

        [Command("btv")]
        public async Task COM_BizTV(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /btv ..."); return; }

            PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Where(x => p.Position.Distance(x.Position) < 20f && x.Dimension == p.Dimension).OrderBy(x => p.Position.Distance(x.Position)).FirstOrDefault();
            if (entranceLabel == null) { MainChat.SendErrorChat(p, "[错误] 无效产业."); return; }
            entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position bussinesPos);

            var biz = await getBusinessFromPos(bussinesPos);

            if (biz.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效产业."); return; }

            if (!await CheckBusinessKey(p, biz.Item1)) { MainChat.SendErrorChat(p, "[错误] 您没有此产业的钥匙!"); return; }

            switch (args[0])
            {
                case "link":
                    if (args[1] == null) { MainChat.SendErrorChat(p, "无效音乐链接."); return; }
                    string link = "https://xbeibeix.com/api/bilibili/biliplayer/?url=" + args[1];

                    biz.Item1.settings.tv.URL = link;
                    biz.Item1.settings.tv.StartTime = DateTime.Now;
                    await biz.Item1.Update(biz.Item2, biz.Item3);
                    foreach (PlayerModel target in Alt.GetAllPlayers())
                    {
                        if (target.Position.Distance(p.Position) <= 40 && target.Dimension == p.Dimension)
                        {
                            await target.EmitAsync("TV:Start", link, 0, biz.Item1.settings.tv.Volume);
                        }
                    }
                    MainChat.EmoteDo(p, "播放了产业电视.");
                    return;

                case "vol":
                    if (!Int32.TryParse(args[1], out int newSound)) { MainChat.SendErrorChat(p, "[错误] 无效音量."); return; }
                    biz.Item1.settings.tv.Volume = newSound;
                    await biz.Item1.Update(biz.Item2, biz.Item3);
                    foreach (PlayerModel target in Alt.GetAllPlayers())
                    {
                        if (target.Position.Distance(p.Position) <= 40 && target.Dimension == p.Dimension)
                        {
                            await target.EmitAsync("TV:SetSound", newSound);
                        }
                    }
                    return;

                case "stop":
                    biz.Item1.settings.tv.URL = "none";

                    await biz.Item1.Update(biz.Item2, biz.Item3);
                    foreach (PlayerModel target in Alt.GetAllPlayers())
                    {
                        if (target.Position.Distance(p.Position) <= 40 && target.Dimension == p.Dimension)
                        {
                            await target.EmitAsync("TV:Stop");
                        }
                    }
                    break;

                case "remove":
                    if (!biz.Item1.settings.tv.hasTv) { MainChat.SendErrorChat(p, "[错误] 此产业没有电视."); return; }
                    LProp TvProp = PropStreamer.GetProp(biz.Item1.settings.tv.TvPropID);
                    if (TvProp == null) { MainChat.SendErrorChat(p, "[错误] 此产业没有电视!"); return; }
                    TvProp.Destroy();
                    biz.Item1.settings.tv = new BusinessModel.TVModel();
                    await biz.Item1.Update(biz.Item2, biz.Item3);
                    MainChat.SendInfoChat(p, "[?] 已删除产业电视.");
                    return;

                default:

                    return;
            }
        }

        public static void EVENT_BusinessPlayTV(PlayerModel p, BusinessModel biz)
        {
            int second = 0;
            second = (int)(DateTime.Now - biz.settings.tv.StartTime).TotalSeconds;
            p.EmitAsync("TV:Start", biz.settings.tv.URL, second, biz.settings.tv.Volume);
        }

        public static void EVENT_BusinessStopTV(PlayerModel p)
        {
            p.EmitAsync("TV:Stop");
        }

        #region Business Type Systems
        public static void LoadTypeSystems()
        {
            //LOJISTIK - STOK
            TextLabelStreamer.Create("~b~[~w~进货港口~b~]~w~~n~输入 /buystock~n~ 购买货物库存, 价格: ~g~$" + ServerGlobalValues.StockPrice.ToString(), ServerGlobalValues.buyStockPosition, streamRange: 10);
        }
        #endregion

        [Command(CONSTANT.COM_BuyStock)]
        public static async Task BuyStock(PlayerModel player)
        {
            if (player.Position.Distance(ServerGlobalValues.buyStockPosition) > 4f) { MainChat.SendErrorChat(player, "[错误] 您不在进货港口."); return; }
            if (player.isCuffed == true) { MainChat.SendErrorChat(player, CONSTANT.ERR_PermissionError); return; }
            if (player.cash <= ServerGlobalValues.StockPrice) { MainChat.SendErrorChat(player, CONSTANT.ERR_MoneyNotEnought); return; }
            if (await GetPlayerBusinessType(player, ServerGlobalValues.stockBusiness) == false) { MainChat.SendErrorChat(player, CONSTANT.ERR_PermissionError); return; }
            if (player.lscGetdata<bool>("HasStock") == true) { MainChat.SendErrorChat(player, "[错误] 您手上已经有货物库存了."); return; }

            player.cash -= ServerGlobalValues.StockPrice;
            await player.updateSql();
            player.lscSetData("HasStock", true);
            OtherSystem.Animations.PlayerAnimation(player, "carrybox");
            MainChat.SendInfoChat(player, "{49A0CD}[服务器]{FFFFFF} 成功购买货物库存, 输入 /loadstock 放入最近的车辆 或 货箱.");
            return;
        }
        [Command(CONSTANT.COM_LoadStock)]
        public static void PushStock(PlayerModel player, params string[] args)
        {
            if (player.lscGetdata<bool>("HasStock") == false) { MainChat.SendErrorChat(player, "[错误] 您身上没有货物库存!"); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(player, "[用法] /loadstock veh/box"); return; }
            switch (args[0].ToString())
            {
                case "veh":
                    VehModel nearVeh = Vehicle.VehicleMain.getNearVehFromPlayer(player);
                    if (nearVeh == null) { MainChat.SendErrorChat(player, "[错误] 附近没有车辆."); return; }
                    if (nearVeh.LockState != AltV.Net.Enums.VehicleLockState.Unlocked) { MainChat.SendErrorChat(player, "[错误] 车辆是锁的"); return; }
                    if (!nearVeh.HasData("VehStock")) { nearVeh.lscSetData("VehStock", 10); }
                    else
                    {
                        int stock = nearVeh.lscGetdata<int>("VehStock");
                        if (stock >= nearVeh.inventoryCapacity)
                        {
                            MainChat.SendErrorChat(player, "[错误] 此车辆后备箱已经满了.");
                            return;
                        }
                        stock += 200; nearVeh.lscSetData("VehStock", stock);
                    }

                    player.DeleteData("HasStock");
                    OtherSystem.Animations.PlayerStopAnimation(player);
                    MainChat.SendInfoChat(player, "[信息] 成功装载货物库存至车辆!");
                    if (nearVeh.lscGetdata<int>("VehStock") > nearVeh.inventoryCapacity)
                    {
                        MainChat.SendInfoChat(player, "此车辆后备箱已经满了.");
                    }
                    break;
                case "box":
                    Crate c = CrateEvents.serverCrates.Find(x => x.pos.Distance(player.Position) < 4f);
                    if (c == null) { MainChat.SendErrorChat(player, "[错误] 附近没有货箱."); return; }
                    if (c.type != 1) { MainChat.SendErrorChat(player, "[错误] 无效货箱!"); return; }
                    c.stock += 200;
                    c.Update();
                    OtherSystem.Animations.PlayerStopAnimation(player);
                    player.DeleteData("HasStock");

                    MainChat.SendInfoChat(player, "[信息] 成功装载货物库存至货箱!");
                    break;
            }
            return;
        }
        [Command(CONSTANT.COM_GetStock)]
        public static void getStockInCar(PlayerModel p)
        {
            if (p.lscGetdata<bool>("HasStock")) { MainChat.SendInfoChat(p, "[信息] 您手上已经有货物库存了."); return; }
            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v == null) { MainChat.SendErrorChat(p, CONSTANT.COM_VQueryVehNotFound); return; }
            if (v.LockState != AltV.Net.Enums.VehicleLockState.Unlocked) { MainChat.SendErrorChat(p, "[错误] 车是锁的"); return; }
            if (!v.HasData("VehStock")) { MainChat.SendErrorChat(p, "[错误] 此车没有货物库存."); return; }
            int stock = v.lscGetdata<int>("VehStock");
            stock -= 200;
            p.lscSetData("HasStock", true);
            OtherSystem.Animations.PlayerAnimation(p, "carrybox");
            if (stock <= 0) { v.DeleteData("VehStock"); }
            else { v.lscSetData("VehStock", stock); }
            MainChat.SendInfoChat(p, "{49A0CD}[服务器]{FFFFFF} 您已从车辆取出货物库存..");
            return;
        }
        [Command(CONSTANT.COM_ShowStock)]
        public static void gosterStock(PlayerModel player)
        {
            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(player);

            if (v == null)
                return;

            if (v.HasData("VehStock")) { MainChat.SendInfoChat(player, "车辆库存: " + v.lscGetdata<int>("VehStock")); return; }
            else { MainChat.SendErrorChat(player, "车辆缺货"); return; }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="p"></param>
        /// <param name="type">Type : 1: Sıgır Eti -> Et yemeği | 2: Balık Eti -> Ekmek arası balık </param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static async Task makeDinner(PlayerModel p, int type, InventoryModel i)
        {

            PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Find(x => p.Position.Distance(x.Position) < 3f && x.Dimension == p.Dimension);
            if (entranceLabel == null) { MainChat.SendErrorChat(p, "[错误] 无效产业."); return; }
            entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position bussinesPos);

            (BusinessModel, Marker, PlayerLabel) biz = await getBusinessFromPos(bussinesPos);
            if (biz.Item1.ownerId != p.sqlID)
            {
                if (p.businessStaff != biz.Item1.ID)
                {
                    MainChat.SendErrorChat(p, "[错误] 您没有在产业工作!");
                }
            }

            if (biz.Item1.type != ServerGlobalValues.dinnerBusiness) { MainChat.SendInfoChat(p, "[错误] 此产业不是餐厅!"); return; }

            switch (type)
            {
                case 1:
                    ServerItems type1 = new ServerItems { ID = 50, type = 37, name = "肉菜", picture = "50", data = "1", data2 = DateTime.Now.ToString(), weight = 1.0, objectModel = "xs_prop_burger_meat_wl" };
                    if (!await OtherSystem.Inventory.AddInventoryItem(p, type1, 1)) { MainChat.SendErrorChat(p, "[错误] 您的库存满了!"); return; }
                    await OtherSystem.Inventory.RemoveInventoryItem(p, i.ID, 1);
                    await OtherSystem.Inventory.UpdatePlayerInventory(p);
                    return;

                case 2:
                    if (i.itemAmount < 1) { MainChat.SendErrorChat(p, "[错误] 您至少要有 1 条鱼才可以做一道菜!"); return; }
                    ServerItems type2 = new ServerItems { ID = 51, type = 37, name = "鱼菜", picture = "51", data = "2", data2 = DateTime.Now.ToString(), weight = 1.0, objectModel = "xs_prop_burger_meat_wl" };
                    if (!await OtherSystem.Inventory.AddInventoryItem(p, type2, 1)) { MainChat.SendErrorChat(p, "[错误] 您的库存满了!"); return; }
                    await OtherSystem.Inventory.RemoveInventoryItem(p, i.ID, 1);
                    await OtherSystem.Inventory.UpdatePlayerInventory(p);
                    return;

                default:
                    return;
            }
        }

        [Command("showbizprice")]
        public async Task COM_ShowBusinessPrice(PlayerModel p)
        {
            var biz = await getNearestBusiness(p);
            if (biz.Item1 == null)
            {
                MainChat.SendErrorChat(p, "[错误] 无效产业!");
                return;
            }

            string text = "<center>" + biz.Item1.name + "</center><br>价格: $" + biz.Item1.price + "<br>库存: " +
                          biz.Item1.stock + "<br>入场费: $" + biz.Item1.entrancePrice;
            MainChat.SendInfoChat(p, text, true);
            return;
        }

        [Command("bgps")]
        public static async Task COM_HouseGPS(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /bgps [产业ID]"); return; }

            if (!Int32.TryParse(args[0], out int HID)) { MainChat.SendInfoChat(p, "[用法] /bgps [产业ID]"); return; }

            BusinessModel h = await Database.DatabaseMain.GetBusinessInfo(HID);
            if (h == null) { MainChat.SendErrorChat(p, "[错误] 无效产业!"); return; }


            if (!h.settings.gps && !await CheckBusinessKey(p, h)) { MainChat.SendErrorChat(p, "[错误] 此产业不属于您或者产业未开启可导航."); return; }

            GlobalEvents.CheckpointCreate(p, h.position, 1, 2, new Rgba(255, 0, 0, 255), "no", "no");
            MainChat.SendInfoChat(p, "[?] 已在小地图标注产业位置.");
            return;
        }

        [Command("binv")]
        public async Task COM_ShowBusinessInventory(PlayerModel p)
        {
            PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Where(x => p.Position.Distance(x.Position) < 20f && x.Dimension == p.Dimension).OrderBy(x => p.Position.Distance(x.Position)).FirstOrDefault();
            if (entranceLabel == null) { MainChat.SendErrorChat(p, "[错误] 无效产业入口."); return; }
            entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position bussinesPos);

            var t = await getBusinessFromPos(bussinesPos);

            if (t.Item1 == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_NotNearHouse); return; }

            if (!await CheckBusinessKey(p, t.Item1)) { MainChat.SendErrorChat(p, "[错误] 您没有此产业的钥匙!"); return; }

            p.EmitLocked("otherEnv:Show", 2, t.Item1.settings.Env);
            return;
        }

        #region VehicleTunning
        [Command(CONSTANT.COM_VehicleTunning)]
        public static async Task COM_VehicleTunning(PlayerModel p)
        {
            if (await GetPlayerBusinessType(p, ServerGlobalValues.mechanicBusiness) == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            Crate x = null;
            p.SetData("InModify", true);
            foreach (var c in CrateEvents.serverCrates)
            {
                if (c.pos.Distance(p.Position) < 10f) { x = c; break; }
            }
            if (x == null) { MainChat.SendErrorChat(p, "[错误] 您不在改装店!"); return; }
            if (x.type != 1) { MainChat.SendErrorChat(p, "[错误] 此产业不属于改装店!"); return; }
            if (!x.useable) { MainChat.SendErrorChat(p, "[错误] 此改装店正在被使用."); return; }
            if (p.businessStaff == x.owner)
            {
                p.EmitLocked("Create:TuningUi", x.settings.modifyLevel);
                VehModel ownerVeh = Vehicle.VehicleMain.getNearVehFromPlayer(p);
                if (ownerVeh == null) { MainChat.SendErrorChat(p, "[错误] 附近没有车辆!"); return; }
                p.SetData("Modify:Vehicle", ownerVeh.sqlID);
                return;
            }
            List<BusinessModel> bizs = await Database.DatabaseMain.GetMemberBusinessList(p);

            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);

            if (v != null)

                foreach (var b in bizs)
                {
                    if (b.ID == x.owner) { 
                        v.settings.ModifiyData = v.AppearanceData;
                        p.EmitLocked("Create:TuningUi", x.settings.modifyLevel);
                        p.SetData("Modify:Vehicle", v.sqlID);
                        Core.Logger.WriteLogData(Logger.logTypes.Modifiye, p.characterName + " 改装了车辆: " + v.sqlID);
                         return; 
                    }
                }

            return;
        }
        [Command("setwtype")]
        public static async Task COM_SetWheelType(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /setwtype [类型]<br>类型: 0 - 运动 | 1 - 肌肉 | 2 - 低盘 | 3 - SUV | 4 - 越野 | 5 - Tuner | 6 - 自行车轮胎 | 7 - 高端"); return; }
            if (!Int32.TryParse(args[0], out int WheelType)) { MainChat.SendInfoChat(p, "[用法] /setwtype [类型]<br>类型: 0 - 运动 | 1 - 肌肉 | 2 - 低盘 | 3 - SUV | 4 - 越野 | 5 - Tuner | 6 - 自行车轮胎 | 7 - 高端"); return; }
            if (await GetPlayerBusinessType(p, ServerGlobalValues.mechanicBusiness) == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }

            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v == null) { MainChat.SendErrorChat(p, "[错误] 附近没有车辆."); return; }

            Crate x = null;
            foreach (var c in CrateEvents.serverCrates)
            {
                if (c.pos.Distance(p.Position) < 15f) { x = c; break; }
            }
            if (x == null) { MainChat.SendErrorChat(p, "[错误] 您不在改装区域!"); return; }
            if (x.type != 1) { MainChat.SendErrorChat(p, "[错误] 您不在改装区域!"); return; }
            if (x.owner != p.businessStaff) { MainChat.SendErrorChat(p, "[错误] 您不在改装区域!"); return; }
            if (x.settings.modifyLevel < 3) { MainChat.SendErrorChat(p, "[错误] 此改装区域的改装等级小于3!"); return; }

            v.AppearanceData = v.settings.ModifiyData;
            v.settings.WheelType = WheelType;
            byte currWheel = v.WheelVariation;
            await v.SetWheelsAsync((byte)WheelType, currWheel);
            v.settings.ModifiyData = v.AppearanceData;
            v.Update();
            Vehicle.VehicleMain.VehicleMakeSettings(v);

            Core.Logger.WriteLogData(Logger.logTypes.Modifiye, p.characterName + " 改变了车辆 " + v.sqlID + " 的轮胎类型为: " + WheelType);
            MainChat.SendInfoChat(p, "> 已更新车辆轮胎.");
            return;
        }

        public static void ServerVehicleTuning(VehModel v, string Data)
        {
            v.SetModKitAsync((byte)1);
            List<CarMods> mods = JsonConvert.DeserializeObject<List<CarMods>>(Data);
            foreach (var x in mods)
            {
                int value = x.Value + 1;
                v.SetMod((byte)x.Id, (byte)value);
            }
            return;
        }

/*        [Command("aracneon")]
        public static async Task COM_NeonColor(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /aracneon RGB kodu<br>Örn: /aracneon 255,255,255,255"); return; }
            if (await GetPlayerBusinessType(p, ServerGlobalValues.mechanicBusiness) == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }

            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v == null) { MainChat.SendErrorChat(p, "[错误] Yakınınızda bir araç bulunmuyor."); return; }

            Crate x = null;
            foreach (var c in CrateEvents.serverCrates)
            {
                if (c.pos.Distance(p.Position) < 15f) { x = c; break; }
            }
            if (x == null) { MainChat.SendErrorChat(p, "[错误] 您不在改装区域!"); return; }
            if (x.type != 1) { MainChat.SendErrorChat(p, "[错误] 您不在改装区域!"); return; }
            if (x.owner != p.businessStaff) { MainChat.SendErrorChat(p, "[错误] 您不在改装区域!"); return; }
            if (x.stock < 80) { MainChat.SendErrorChat(p, "[错误] Stok kutunuzda yeterli stok yok."); return; }
            if (x.settings.modifyLevel < 4) { MainChat.SendErrorChat(p, "[错误] Bu komutu kullanmak için yetkiniz yok!"); return; }

            string[] colors = args[0].Split(",");
            if (colors.Length <= 2) { MainChat.SendInfoChat(p, "[用法] /aracneon RGB kodu<br>Örn: /aracneon 255,255,255,255"); return; }

            x.stock -= 80;
            x.Update();

            if (!byte.TryParse(colors[0], out byte c1) || !byte.TryParse(colors[1], out byte c2) || !byte.TryParse(colors[2], out byte c3) || !byte.TryParse(colors[3], out byte c4)) { MainChat.SendInfoChat(p, "[用法] /aracboya [birincilrenk/ikincilrenk] [RGBA(renk kodu)]<br>Örn: /aracboya birincilrenk 255,255,255,255"); return; }

            v.settings.NeonColor = new Rgba(c1, c2, c3, c4);
            v.settings.hasNeon = true;
            v.Update();
            Vehicle.VehicleMain.VehicleMakeSettings(v);

            Core.Logger.WriteLogData(Logger.logTypes.Modifiye, p.characterName + " aracin rengini değiştirdi. Araç: " + v.sqlID + " | Renk: " + args[0]);
            MainChat.SendInfoChat(p, "> Aracı 80 stok karşılığında yeniden boyadınız.");
            return;
        }*/

        [Command("driftmod")]
        public async Task COM_AddDriftMode(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /driftmod [玩家ID]"); return; }
            if (await GetPlayerBusinessType(p, ServerGlobalValues.mechanicBusiness) == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }

            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v == null) { MainChat.SendErrorChat(p, "[错误] 附近没有车辆."); return; }

            Crate x = null;
            foreach (var c in CrateEvents.serverCrates)
            {
                if (c.pos.Distance(p.Position) < 15f) { x = c; break; }
            }
            if (x == null) { MainChat.SendErrorChat(p, "[错误] 您不在改装区域!"); return; }
            if (x == null) { MainChat.SendErrorChat(p, "[错误] 您不在改装区域!"); return; }
            if (x.type != 1) { MainChat.SendErrorChat(p, "[错误] 您不在改装区域!"); return; }
            if (x.owner != p.businessStaff) { MainChat.SendErrorChat(p, "[错误] 您不在改装区域!"); return; }
            if (x.stock < 500) { MainChat.SendErrorChat(p, "[错误] 此改装区域没有足够的货物库存."); return; }
            if (x.settings.modifyLevel < 4) { MainChat.SendErrorChat(p, "[错误] 此改装区域的改装等级不足4级!"); return; }

            if (!Int32.TryParse(args[0], out int tSql)) { MainChat.SendInfoChat(p, "[用法] /driftmod [玩家ID]"); return; }
            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (target == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家!"); return; }
            if (target.Position.Distance(p.Position) > 5) { MainChat.SendErrorChat(p, "[错误] 离指定玩家太远."); return; }

            Inputs.SendButtonInput(target, (500 * x.settings.stockCost).ToString() + "$ 漂移模式 " + ((v.settings.driftMode) ? "将被移除." : "将被添加."), "Vehicle:DriftMode", p.sqlID + "," + x.sqlID + "," + v.sqlID);
            MainChat.SendInfoChat(p, "[?] 您正在给 " + target.characterName.Replace('_', ' ') + " 的车辆调整漂移模式为 " + ((v.settings.driftMode) ? " 关闭" : " takmak için") + " 开启.");
            return;
        }

        [AsyncClientEvent("Vehicle:DriftMode")]
        public static async Task EVENT_VehicleDriftMode(PlayerModel p, bool selection, string _val)
        {
            string[] val = _val.Split(',');
            if (!Int32.TryParse(val[0], out int tSQl) || !Int32.TryParse(val[1], out int crateSql) || !Int32.TryParse(val[2], out int vehicleSql)) { MainChat.SendErrorChat(p, "[错误] 无效数据."); return; }
            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(tSQl);
            if (!selection)
            {
                MainChat.SendInfoChat(p, "[?] 您拒绝了改装请求(漂移模式).");
                if (target != null) { MainChat.SendInfoChat(target, "[?] " + p.characterName.Replace('_', ' ') + " 拒绝了您的改装请求(漂移模式)."); return; }
                return;
            }
            else
            {
                var crate = CrateEvents.serverCrates.Where(x => x.sqlID == crateSql).FirstOrDefault();
                if (crate == null) { MainChat.SendErrorChat(p, "[错误] 无效确认窗口."); return; }

                int cost = crate.settings.stockCost * 500;
                if (p.cash < cost)
                {
                    MainChat.SendErrorChat(p, "[错误] 您没有足够的现金支付此请求!");
                    if (target != null) { MainChat.SendErrorChat(p, "[错误] 货箱出现错误, 操作无法完成, 库存状态无变化."); return; }
                }

                var biz = await getBusinessById(crate.owner);
                if (biz.Item1 != null)
                {
                    p.cash -= cost;
                    biz.Item1.vault += cost;
                    await biz.Item1.Update(biz.Item2, biz.Item3);
                    await p.updateSql();

                    VehModel vehicle = Vehicle.VehicleMain.getVehicleFromSqlId(vehicleSql);
                    if (vehicle != null)
                    {
                        vehicle.settings.driftMode = !vehicle.settings.driftMode;
                        vehicle.Update();
                        vehicle.SetStreamSyncedMetaData("DriftMode", vehicle.settings.driftMode);
                        crate.stock -= 500;
                        crate.Update();

                        MainChat.SendInfoChat(p, "[?] 您已将车辆的漂移模式调整为: " + ((vehicle.settings.driftMode) ? "开启." : "关闭.") + " 花费: $" + cost);
                        MainChat.SendInfoChat(target, "[?] " + p.characterName.Replace('_', ' ') + " 已接受改装请求, 产业已入账 $" + cost);
                        Core.Logger.WriteLogData(Logger.logTypes.Modifiye, target.characterName + " 为 " + p.characterName + " 的车辆改装了漂移模式");
                        return;
                    }
                }
            }
        }
        #endregion

        #region Security
        [Command("security")]
        public async Task COM_Security(PlayerModel p, params string[] args)
        {
            if (!await GetPlayerBusinessType(p, ServerGlobalValues.SecurityBusiness))
            {
                MainChat.SendErrorChat(p, "[错误] 无权操作!"); return;
            }

            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /security [选项] [数值]<br>可用选项: veh, house, biz"); return; }
            if (!Int32.TryParse(args[1], out int securityLevel)) { MainChat.SendInfoChat(p, "[用法] /security [选项] [数值]<br>可用选项: veh, house, biz"); return; }
            if (securityLevel > 6) { MainChat.SendErrorChat(p, "[错误] 最多可安装 6级 防盗!"); return; }

            switch (args[0])
            {
                case "isyeri":
                    (BusinessModel, Marker, PlayerLabel) biz = await getNearestBusiness(p);
                    if (biz.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 附近没有防盗器材店."); return; }

                    if (biz.Item1.settings.SecurityLevel >= securityLevel) { MainChat.SendErrorChat(p, "[错误] 产业的防盗等级与目前正在购买的相同(或大)!"); return; }

                    int cost = (securityLevel - biz.Item1.settings.SecurityLevel) * (biz.Item1.price / 100);
                    if (p.cash < cost) { MainChat.SendInfoChat(p, "[错误] 您没有足够的资金进行此交易!所需金额: $" + cost.ToString()); return; }

                    Inputs.SendButtonInput(p, "防盗等级 > " + securityLevel + " | 价格: $" + cost.ToString(), "Security:Level:Increase", "0," + cost.ToString() + "," + securityLevel.ToString() + "," + biz.Item1.ID.ToString());

                    return;

                case "ev":
                    (HouseModel, PlayerLabel, Marker) house = await Houses.getNearHouse(p);
                    if (house.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 附近没有房屋."); return; }

                    if (house.Item1.settings.SecurityLevel >= securityLevel) { MainChat.SendErrorChat(p, "[错误] 房屋的防盗等级与目前正在购买的相同(或大)!"); return; }
                    int cost2 = (securityLevel - house.Item1.settings.SecurityLevel) * (house.Item1.price / 100);
                    if (p.cash < cost2) { MainChat.SendInfoChat(p, "[错误] 您没有足够的资金进行此交易!所需金额: $" + cost2.ToString()); return; }
                    Inputs.SendButtonInput(p, "防盗等级 > " + securityLevel + " | 价格: $" + cost2.ToString(), "Security:Level:Increase", "1," + cost2.ToString() + "," + securityLevel.ToString() + "," + house.Item1.ID.ToString());

                    return;

                case "arac":
                    VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
                    if (v == null) { MainChat.SendErrorChat(p, "[错误] 附近没有车辆!"); return; }

                    if (v.settings.SecurityLevel >= securityLevel) { MainChat.SendErrorChat(p, "[错误] 车辆的防盗等级与目前正在购买的相同(或大)."); return; }
                    int cost3 = (securityLevel - v.settings.SecurityLevel) * (v.price / 100);
                    if (p.cash < cost3) { MainChat.SendInfoChat(p, "[错误] 您没有足够的资金进行此交易!所需金额: $" + cost3.ToString()); return; }
                    Inputs.SendButtonInput(p, "防盗等级 > " + securityLevel + " | 价格: $" + cost3.ToString(), "Security:Level:Increase", "2," + cost3.ToString() + "," + securityLevel.ToString() + "," + v.sqlID.ToString());

                    return;

                default:
                    MainChat.SendInfoChat(p, "[用法] /security [选项] [数值]<br>可用选项: veh, house, biz");
                    return;

            }
        }


        [AsyncClientEvent("Security:Level:Increase")]
        public async Task EVENT_BusinessSecurity(PlayerModel p, bool selection, string value)
        {
            // "0" + cost.ToString() + "," + securityLevel.ToString() + "," + biz.Item1.ID.ToString()
            if (!selection)
            {
                MainChat.SendErrorChat(p, "[!] 您取消了交易!");
                return;
            }
            string[] val = value.Split(",");

            if (!Int32.TryParse(val[0], out int type) || !Int32.TryParse(val[1], out int cost) || !Int32.TryParse(val[2], out int securityLevel) || !Int32.TryParse(val[3], out int ID))
                return;

            switch (type)
            {
                case 0:// işyeri
                    var Biz = await Props.Business.getBusinessById(ID);
                    if (Biz.Item1 == null)
                        return;

                    if (p.cash < cost)
                        return;

                    if (Biz.Item1.settings.SecurityLevel >= securityLevel)
                        return;

                    p.cash -= cost;
                    await p.updateSql();
                    Biz.Item1.settings.SecurityLevel = securityLevel;
                    await Biz.Item1.Update(Biz.Item2, Biz.Item3);

                    MainChat.EmoteDoAlternative(p, p.characterName.Replace("_", " ") + " 为 [" + Biz.Item1.ID + "]" + Biz.Item1.name + " 安装了防盗器材.");

                    return;

                case 1: // ev
                    var house = await Props.Houses.getHouseById(ID);
                    if (house.Item1 == null)
                        return;

                    if (p.cash < cost)
                        return;

                    if (house.Item1.settings.SecurityLevel >= securityLevel)
                        return;


                    p.cash -= cost;
                    await p.updateSql();
                    house.Item1.settings.SecurityLevel = securityLevel;
                    house.Item1.Update(house.Item3, house.Item2);

                    MainChat.EmoteDoAlternative(p, p.characterName.Replace("_", " ") + " 为 [" + house.Item1.ID + "]" + house.Item1.name + " 安装了防盗器材.");
                    return;

                case 2: // Araba
                    VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(ID);
                    if (v == null)
                        return;

                    if (p.cash < cost)
                        return;

                    if (v.settings.SecurityLevel >= securityLevel)
                        return;

                    p.cash -= cost;
                    await p.updateSql();
                    v.settings.SecurityLevel = securityLevel;
                    v.Update();

                    MainChat.EmoteDoAlternative(p, p.characterName.Replace("_", " ") + " 为 [" + v.sqlID + "] " + v.NumberplateText + " 安装了防盗器材.");

                    return;
            }

            return;
        }
        #endregion


        #region Business Menu
        public class BusinesMemberList
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }
        [Command("bmenu")]
        public async Task COM_BusinessMenu(PlayerModel p)
        {
            var biz = await getNearestBusiness(p);
            if (biz.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效产业!"); return; }
            if (!await CheckBusinessKey(p, biz.Item1)) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }

            LSCUI.UI ui = new LSCUI.UI();
            ui.StartPoint = new int[] { 600, 400 };
            ui.Title = biz.Item1.name;
            ui.SubTitle = "产业菜单";

            LSCUI.Component_CheckboxItem bizLock = new LSCUI.Component_CheckboxItem();
            bizLock.Header = "产业解锁";
            bizLock.Description = "产业解锁 ~g~开启~w~/~r~关闭";
            bizLock.Trigger = "BizMenu:LockState";
            bizLock.TriggerData = biz.Item1.ID.ToString();
            bizLock.Check = biz.Item1.isLocked;
            ui.CheckboxItems.Add(bizLock);

            LSCUI.Component_CheckboxItem bizText = new LSCUI.Component_CheckboxItem();
            bizText.Header = "产业聊天距离";
            bizText.Description = "产业聊天距离 ~g~开启~w~/~r~关闭";
            bizText.Trigger = "BizMenu:TextState";
            bizText.TriggerData = biz.Item1.ID.ToString();
            bizText.Check = biz.Item1.settings.floatText;
            ui.CheckboxItems.Add(bizText);

            /*LSCUI.Component_Item bizEnterance = new LSCUI.Component_Item();
            bizEnterance.Header = "Giriş Ücreti ~g~$" + biz.Item1.entrancePrice;
            bizEnterance.Description = "İşyeri giriş ücretini ayarlamanızı sağlar.";
            bizEnterance.Trigger = "BizMenu:EntrancePrice";
            bizEnterance.TriggerData = biz.Item1.ID.ToString();
            ui.Items.Add(bizEnterance);*/

            LSCUI.Component_Item bizMoney = new LSCUI.Component_Item();
            bizMoney.Header = "产业金库: $" + biz.Item1.vault;
            bizMoney.Description = "查看产业金库.";
            ui.Items.Add(bizMoney);

            LSCUI.Component_Item bizName = new LSCUI.Component_Item();
            bizName.Header = "~o~更改产业名称";
            bizName.Description = "更改产业名称.";
            bizName.Trigger = "BizMenu:ChangeName";
            bizName.TriggerData = biz.Item1.ID + ",123";
            ui.Items.Add(bizName);

            LSCUI.SubMenu staff = new LSCUI.SubMenu();
            staff.Header = "员工列表";
            staff.SubTitle = "为产业工作的员工.";
            staff.StartPoint = new int[] { 600, 400 };

            LSCUI.Component_Item tax = new LSCUI.Component_Item();
            tax.Header = "总计税: ~g~" + biz.Item1.settings.TotalTax + "$";
            tax.Description = "您可以按回车键缴纳税费.";
            tax.Trigger = "BizMenu:PayTax";
            tax.TriggerData = biz.Item1.ID + ",123";



            List<BusinesMemberList> members = await Database.DatabaseMain.GetBusinessStafList(biz.Item1.ID);
            if (members.Count > 0)
            {
                foreach (var member in members)
                {
                    LSCUI.Component_Item memb = new LSCUI.Component_Item();
                    memb.Header = "[" + member.ID + "]" + member.Name.Replace("_", " ");
                    memb.Description = "产业员工";
                    staff.Items.Add(memb);
                }
            }
            else
            {
                LSCUI.Component_Item memb = new LSCUI.Component_Item();
                memb.Header = "无员工!";
                memb.Description = "无员工!";
                staff.Items.Add(memb);
            }

            ui.SubMenu.Add(staff);
            ui.Send(p);
            return;
        }

        [AsyncClientEvent("BizMenu:EntrancePrice")]
        public void EVENT_BizMenu_Entrance_1(PlayerModel p, string ID)
        {
            LSCUI.Close(p);
            Inputs.SendTypeInput(p, "设置入场费.", "BizMenu:EntrancePriceSecond", ID);
            return;
        }

        [AsyncClientEvent("BizMenu:EntrancePriceSecond")]
        public async Task EVENT_BizMenu_Entrance_2(PlayerModel p, string price, string ID)
        {
            if (!Int32.TryParse(price, out int newPrice) || !Int32.TryParse(ID, out int BizID))
                return;

            if (newPrice < 0 || newPrice > 1000) { MainChat.SendErrorChat(p, "[!] 入场费为 0 - 1000."); return; }
            var biz = await getBusinessById(BizID);
            biz.Item1.entrancePrice = newPrice;
            await biz.Item1.Update(biz.Item2, biz.Item3);

            MainChat.SendInfoChat(p, "[?] 已更新入场费为 $" + newPrice);
            return;
        }

        

        [AsyncClientEvent("BizMenu:PayTax")]
        public async Task EVENT_BizMenu_PayTax(PlayerModel p, string _ID)
        {
            LSCUI.Close(p);
            string[] ID = _ID.Split(",");
            if (!Int32.TryParse(ID[0], out int bizID))
                return;

            var biz = await getBusinessById(bizID);
            if (biz.Item1 == null) return;
            if (biz.Item1.ownerId != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 此产业不属于您!"); return; }
            if (p.cash < biz.Item1.settings.TotalTax) { MainChat.SendErrorChat(p, "[错误] 您没有足够的钱!"); return; }
            p.cash -= biz.Item1.settings.TotalTax;
            await p.updateSql();
            biz.Item1.settings.TotalTax = 0;
            await biz.Item1.Update(biz.Item2, biz.Item3);
            MainChat.SendInfoChat(p, "[?] 成功缴纳税费.");
            return;
        }

        [AsyncClientEvent("BizMenu:ChangeName")]
        public static void COM_ChangeName(PlayerModel p, string val)
        {
            LSCUI.Close(p);
            Inputs.SendTypeInput(p, "产业名称", "BizMenu:ChangeName2", val);
            return;
        }

        [AsyncClientEvent("BizMenu:ChangeName2")]
        public async Task COM_ChangeNameSecond(PlayerModel p, string newName, string val)
        {
            string[] _val = val.Split(",");
            if (_val[0] == null)
                return;
            if (!Int32.TryParse(_val[0], out int bizID))
                return;

            var biz = await getBusinessById(bizID);
            if (biz.Item1 == null)
                return;

            if (biz.Item1.ownerId != p.sqlID) { MainChat.SendErrorChat(p, "[?] 此产业不属于您!"); return; }
            biz.Item1.name = newName;
            await biz.Item1.Update(biz.Item2, biz.Item3);
            MainChat.SendInfoChat(p, "[!] 已更新产业名称.");
            return;
        }

        [AsyncClientEvent("BizMenu:LockState")]
        public async Task EVENT_BizMenu_LockState(PlayerModel p, bool state, string _bizID)
        {
            LSCUI.Close(p);
            if (!Int32.TryParse(_bizID, out int bizID))
                return;
            var biz = await getBusinessById(bizID);
            if (biz.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效产业!"); return; }

            biz.Item1.isLocked = state;
            await biz.Item1.Update(biz.Item2, biz.Item3);

            MainChat.SendInfoChat(p, "[?] 已更新产业状态为: " + (state ? "营业" : "打烊"));
            return;
        }

        [AsyncClientEvent("BizMenu:TextState")]
        public async Task EVENT_BizMenu_TextState(PlayerModel p, bool state, string _bizID) 
        {
            LSCUI.Close(p);
            if(!Int32.TryParse(_bizID, out int bizID))
                return;

            var biz = await getBusinessById(bizID);
            if (biz.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效产业!"); return; }

            biz.Item1.settings.floatText = state;
            await biz.Item1.Update(biz.Item2, biz.Item3);

            MainChat.SendInfoChat(p, "[?] 已更新产业内聊天距离为 " + (state ? "开启" : "关闭"));

            return;
        }
        #endregion


        [Command("bpaytax")]
        public async Task COM_PayHouseTax(PlayerModel p)
        {
            var house = await getNearestBusiness(p);
            if (house.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 附近没有产业!"); return; }
            if (house.Item1.ownerId != p.sqlID && !house.Item1.settings.Admins.Contains(p.sqlID)) { MainChat.SendErrorChat(p, "[错误] 此产业不属于您."); return; }

            OtherSystem.NativeUi.Inputs.SendButtonInput(p, "总计税 " + house.Item1.settings.TotalTax + "$", "Business:PayTax", "none");
            return;
        }

        [AsyncClientEvent("Business:PayTax")]
        public async Task EVENT_PayHouseTax(PlayerModel p, bool selection)
        {
            if (!selection)
                return;

            var house = await getNearestBusiness(p);
            if (house.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 附近没有产业!"); return; }
            if (house.Item1.ownerId != p.sqlID && !house.Item1.settings.Admins.Contains(p.sqlID)) { MainChat.SendErrorChat(p, "[错误] 此产业不属于您."); return; }

            if (p.cash < house.Item1.settings.TotalTax) { MainChat.SendErrorChat(p, "[错误] 您没有足够的钱!"); return; }
            p.cash -= house.Item1.settings.TotalTax;
            await p.updateSql();

            house.Item1.settings.TotalTax = 0;
            await house.Item1.Update(house.Item2, house.Item3);

            MainChat.SendInfoChat(p, "[?] 已缴纳产业税.");
            return;
        }

        [Command("blight")]
        public async Task COM_BusinessLight(PlayerModel player)
        {
            (BusinessModel, Marker, PlayerLabel) biz = await getNearestBusiness(player);
            //CharacterModel playerChar = GlobalEvents.PlayerGetData(player);

            if (biz.Item1 == null)
            {
                PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Where(x => player.Position.Distance(x.Position) < 20f && x.Dimension == player.Dimension).OrderBy(x => player.Position.Distance(x.Position)).FirstOrDefault();
                if (entranceLabel == null) { MainChat.SendErrorChat(player, "[错误] 无效产业."); return; }
                entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position bussinesPos);

                biz = await getBusinessFromPos(bussinesPos);

                if (biz.Item1 == null) { MainChat.SendErrorChat(player, "[错误] 无效产业."); return; }
            }

            if (!await CheckBusinessKey(player, biz.Item1)) { MainChat.SendErrorChat(player, "[错误] 您没有此产业的钥匙."); return; }

            biz.Item1.settings.Light = !biz.Item1.settings.Light;

            MainChat.EmoteMe(player, " 伸出手按下了开关" + ((biz.Item1.settings.Light) ? "开启了灯" : "关闭了灯"));
            await biz.Item1.Update(biz.Item2, biz.Item3);

            GlobalEvents.SetLightState(player, biz.Item1.settings.Light);

            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (t.Position.Distance(player.Position) < 50 && t.Dimension == player.Dimension)
                {
                    GlobalEvents.SetLightState(t, biz.Item1.settings.Light);
                }
            }
        }


        [Command("badmin")]
        public async Task COM_SetAdmin(PlayerModel p, params string[] args)
        {
            if(args.Length < 1) { MainChat.SendInfoChat(p, "[用法] /badmin [选项] [产业ID] [玩家ID]<br>选项: add, remove, reset"); return; }
            if(args[0] != "reset" && args.Length < 3) { MainChat.SendInfoChat(p, "[用法] /badmin [选项] [产业ID] [玩家ID]<br>选项: add, remove, reset"); return; }

            switch(args[0]) 
            {
                case "add":
                    if (!Int32.TryParse(args[1], out int bizID)) { MainChat.SendErrorChat(p, "[错误] 无效产业."); return; }
                    if (!Int32.TryParse(args[2], out int targetID)) { MainChat.SendErrorChat(p, "[错误] 无效玩家."); return; }

                    var biz = await getBusinessById(bizID);
                    if (biz.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效产业."); return; }
                    if (biz.Item1.ownerId != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 此产业不属于您."); return; }

                    var target = GlobalEvents.GetPlayerFromSqlID(targetID);
                    if (target == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家."); return; }

                    if (biz.Item1.settings.Admins.Contains(target.sqlID)) { MainChat.SendErrorChat(p, "[错误] 此玩家已经是您产业的经理了."); return; }

                    biz.Item1.settings.Admins.Add(target.sqlID);
                    await biz.Item1.Update(biz.Item2, biz.Item3);

                    MainChat.SendInfoChat(p, "[?] " + target.characterName.Replace('_', ' ') + " isimli oyuncu işyerinin yöneticisi olarak eklendi.");
                    MainChat.SendInfoChat(target, "[?] " + p.characterName.Replace('_', ' ') + " isimli oyuncu sizi işyerinin yöneticisi olarak ekledi.");
                    return;

                case "remove":
                    if (!Int32.TryParse(args[1], out int bizID2)) { MainChat.SendErrorChat(p, "[错误] 无效产业."); return; }
                    if (!Int32.TryParse(args[2], out int targetID2)) { MainChat.SendErrorChat(p, "[错误] 无效玩家."); return; }

                    var biz2 = await getBusinessById(bizID2);
                    if (biz2.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效产业."); return; }
                    if (biz2.Item1.ownerId != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 此产业不属于您."); return; }

                    var target2 = GlobalEvents.GetPlayerFromSqlID(targetID2);
                    if (target2 == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家."); return; }

                    if (!biz2.Item1.settings.Admins.Contains(target2.sqlID)) { MainChat.SendErrorChat(p, "[错误] 此玩家不是您产业的经理."); return; }

                    biz2.Item1.settings.Admins.Remove(target2.sqlID);
                    await biz2.Item1.Update(biz2.Item2, biz2.Item3);

                    MainChat.SendInfoChat(p, "[?] " + target2.characterName.Replace('_', ' ') + " 被免去了产业经理的职位.");
                    MainChat.SendInfoChat(target2, "[?] " + p.characterName.Replace('_', ' ') + " 免去了您的产业经理职位.");
                    return;

                case "reset":
                    if (!Int32.TryParse(args[1], out int bizID3)) { MainChat.SendErrorChat(p, "[错误] 无效产业."); return; }

                    var biz3 = await getBusinessById(bizID3);
                    if (biz3.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效产业."); return; }
                    if (biz3.Item1.ownerId != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 此产业不属于您."); return; }

                    biz3.Item1.settings.Admins.Clear();
                    await biz3.Item1.Update(biz3.Item2, biz3.Item3);

                    MainChat.SendInfoChat(p, "[?] 已重置产业经理.");
                    return;
            }
        }
    }
}