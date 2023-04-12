using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Globals;
using outRp.Models;
using outRp.OtherSystem.Textlabels;
using AltV.Net.Resources.Chat.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace outRp.Props
{
    public class Houses : IScript
    {
        public static double systemSell = 2;
        [Command("sethousesellmp")]
        public static void COM_HouseSellMultipler(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 6) { MainChat.SendErrorChat(p, "[错误] 无权操作."); return; }
            if (args.Length <= 0) { MainChat.SendErrorChat(p, "[用法] /sethousesellmp [数值]<br>例如: /sethousesellmp 1.2 = %80"); return; }
            if (!double.TryParse(args[0], out double mult)) { MainChat.SendErrorChat(p, "[用法] /sethousesellmp [数值]<br>例如: /sethousesellmp 1.2 = %80"); return; }

            systemSell = mult;

            MainChat.SendInfoChat(p, "[=] 已更新房屋出售率.");
            return;
        }
        public static async Task LoadServerHouses()
        {
            List<HouseModel> houses = await Database.DatabaseMain.GetAllServerHouses(); // LoadHouses
            foreach (HouseModel h in houses)
            {
                /* Position lPos = h.pos;
                 lPos.Z -= 0.2f;                 
                 MarkerTypes mType = MarkerTypes.MarkerTypeDallorSign;
                 if(h.ownerId > 0) { mType = MarkerTypes.MarkerTypeUpsideDownCone; }
                 Marker hM = MarkerStreamer.Create(mType, h.pos, new System.Numerics.Vector3(0.3f, 0.3f, 0.3f), streamRange: 10, color: new Rgba(31, 255, 0, 100), faceCamera: true);
                 hM.SetData(EntityData.GeneralSetting.DataType, EntityData.GeneralSetting.TypeHouse);
                 hM.SetData(EntityData.houseData.HouseID, h.ID);
                 hM.SetData(EntityData.houseData.HouseInfo, JsonConvert.SerializeObject(h));

                 PlayerLabel lbl = TextLabelStreamer.Create(setupHouseName(h), lPos, streamRange: 10);
                 lbl.SetData(EntityData.EntranceTypes.EntranceType, EntityData.EntranceTypes.House);
                 hM.SetData(EntityData.houseData.HouseLabel, lbl.Id);

                 PlayerLabel labelExit = TextLabelStreamer.Create("Çıkmak için [~g~E~w~] basınız.", h.intPos, h.dimension, true, new Rgba(255, 255, 255, 255), streamRange: 2);
                 labelExit.SetData(EntityData.EntranceTypes.EntranceType, EntityData.EntranceTypes.ExitWorld);
                 labelExit.SetData(EntityData.EntranceTypes.ExitWorldPosition, h.pos);
                 hM.SetData(EntityData.houseData.HouseExitWorldLbl, labelExit.Id);*/

                Marker hM = MarkerStreamer.Create((MarkerTypes)h.settings.MarkerType, h.pos, h.settings.markerScale, streamRange: 10);
                hM.DisplayName = setupHouseName(h);
                hM.isBusinessMarker = true;
                hM.Color = h.settings.MarkerColor;
                //hM.SetData(EntityData.GeneralSetting.DataType, EntityData.GeneralSetting.TypeHouse);
                hM.SetData(EntityData.houseData.HouseID, h.ID);

                PlayerLabel labelExit = TextLabelStreamer.Create("按 [~g~E键~w~] 离开", h.intPos, h.dimension, true, new Rgba(255, 255, 255, 255), streamRange: 2);
                labelExit.SetData(EntityData.EntranceTypes.EntranceType, EntityData.EntranceTypes.ExitWorld);
                labelExit.SetData(EntityData.EntranceTypes.ExitWorldPosition, h.pos);
                hM.SetData(EntityData.houseData.HouseExitWorldLbl, labelExit.Id);

                if (h.settings.tv.hasTv)
                {
                    LProp tv = PropStreamer.Create("vw_prop_vw_cinema_tv_01", h.settings.tv.Position, h.settings.tv.Rotation, h.dimension, false, false, true);
                    h.settings.tv.TvPropID = tv.Id;
                    h.Update(hM, labelExit);
                }
            }
            Alt.Log("加载 房屋数量:" + houses.Count.ToString());
        }

        
        public static async Task<(HouseModel, PlayerLabel, Marker)> getHouseById(int ID)
        {
            HouseModel h = null;
            PlayerLabel l = null;
            Marker M = null;

            foreach (Marker x in MarkerStreamer.GetAllDynamicMarkers())
            {
                x.TryGetData(EntityData.houseData.HouseID, out int HID);
                if (ID == HID) { M = x; break; }
            }

            if (M == null) { return (null, null, null); }

            h = await Database.DatabaseMain.GetHouseByID(ID);

            if (M.TryGetData(EntityData.houseData.HouseExitWorldLbl, out ulong txtlblID)) { l = TextLabelStreamer.GetDynamicTextLabel(txtlblID); }


            return (h, l, M);
        }
        
        public static string setupHouseName(HouseModel h)
        {
            string result = "";
            result += "~g~[~w~" + h.name + ", 地址编号:" + h.ID + "~g~]";

            if (h.rentOwner > 0) { result += "~n~~y~已被租住"; }
            else { if (h.isRentable) { result += "~n~出租中: ~g~$" + h.rentPrice; } }

            string locked = (h.isLocked) ? "~r~上锁" : "~g~未上锁";
            if (h.settings.TotalTax >= (h.price / 100) * 10)
                locked = "~r~密封!";
            result += "~n~" + locked;

            return result;
        }
        public static async Task<(HouseModel, PlayerLabel, Marker)> getNearHouse(PlayerModel p)
        {
            HouseModel h = null;
            PlayerLabel l = null;
            Marker M = null;
            foreach (Marker x in MarkerStreamer.GetAllDynamicMarkers())
            {
                if (p.Position.Distance(x.Position) < 2)
                {
                    if (x.TryGetData(EntityData.houseData.HouseID, out int HID))
                    {
                        M = x;
                        h = await Database.DatabaseMain.GetHouseByID(HID);
                        if (x.TryGetData(EntityData.houseData.HouseExitWorldLbl, out ulong txtlblID))
                        {
                            l = TextLabelStreamer.GetDynamicTextLabel(txtlblID);
                        }
                    }

                }
            }
            return (h, l, M);
        }
        public static async Task<bool> HouseKeysQuery(PlayerModel p, HouseModel h)
        {
            if (h.ownerId == p.sqlID) { return true; }
            if (h.rentOwner == p.sqlID) { return true; }

            List<Database.DatabaseMain.KeyModel> keys = await Database.DatabaseMain.getHouseKeys(h.ID);
            foreach (Database.DatabaseMain.KeyModel x in keys)
            {

                if (x.keyOwner == p.sqlID) { return true; }
            }
            return false;
        }
        public static async Task<(HouseModel, PlayerLabel, Marker)> getHouseFromPos(Position pos)
        {
            HouseModel h = null;
            PlayerLabel l = null;
            Marker M = null;
            foreach (Marker x in MarkerStreamer.GetAllDynamicMarkers())
            {
                if (pos.Distance(x.Position) < 1)
                {
                    M = x;
                    if (x.TryGetData(EntityData.houseData.HouseID, out int HID))
                    {
                        h = await Database.DatabaseMain.GetHouseByID(HID);
                        if (x.TryGetData(EntityData.houseData.HouseExitWorldLbl, out ulong txtlblID))
                        {
                            l = TextLabelStreamer.GetDynamicTextLabel(txtlblID);
                        }
                    }
                }                                  //
            }
            return (h, l, M);
        }

        public static (HouseModel, PlayerLabel, Marker) getHouseFromPos(Position pos, int dimension, int range = 20)
        {
            HouseModel h = null;
            PlayerLabel l = null;
            Marker M = null;
            foreach (Marker x in MarkerStreamer.GetAllDynamicMarkers())
            {
                if (pos.Distance(x.Position) < range && x.Dimension == dimension)
                {
                    M = x;
                    if (x.TryGetData(EntityData.houseData.HouseID, out int HID))
                    {
                        if (x.TryGetData(EntityData.houseData.HouseExitWorldLbl, out ulong txtlblID))
                        {
                            l = TextLabelStreamer.GetDynamicTextLabel(txtlblID);
                        }
                    }
                }                                  //
            }
            return (h, l, M);
        }

        public static async Task UpdateHouseTax()
        {
            if (!ServerGlobalValues.serverCanTax)
                return;
            List<int> IDS = await Database.DatabaseMain.getAllHouseIDS();
            foreach (int ID in IDS)
            {
                var House = await getHouseById(ID);
                if (House.Item1 == null)
                    continue;

                if (House.Item1.price >= 30000)
                {
                    House.Item1.settings.TotalTax += House.Item1.settings.Tax;
                    if (House.Item1.settings.TotalTax >= (House.Item1.price / 100) * 60)
                    {
                        House.Item1.ownerId = 0;
                        House.Item1.rentOwner = 0;
                        House.Item1.name = "银行回收房屋";
                        House.Item1.isLocked = true;
                        House.Item1.isRentable = false;
                    }

                    House.Item1.Update(House.Item3, House.Item2);
                }
            }

            MainChat.SendAdminChat("[Ekonomi] Ev vergileri güncellendi.");
            return;
        }

        // [Command(CONSTANT.COM_BuyHouse)]
        //public static async Task BuyHouse(PlayerModel p)
        // {
        // (HouseModel, PlayerLabel, Marker) t = await getNearHouse(p);
        // if (t.Item1 == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_NotNearHouse); return; }
        //if (t.Item1.ownerId >= 1) { MainChat.SendErrorChat(p, "[错误] Bu evin zaten bir sahibi var."); return; }
        //if (t.Item1.price > p.cash) { MainChat.SendErrorChat(p, CONSTANT.ERR_MoneyNotEnought); return; }
        //p.cash -= t.Item1.price;
        //await p.updateSql();
        //t.Item1.ownerId = p.sqlID;
        //t.Item2.Text = setupHouseName(t.Item1);
        //t.Item1.Update(t.Item3, t.Item2);
        //MainChat.SendInfoChat(p, t.Item1.name + " isimli evi " + t.Item1.price + "$ karşılığında satın aldınız.");
        //return;
        //      }

        [Command(CONSTANT.COM_EditHouse)]
        public static async Task EditHome(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, CONSTANT.DESC_EditHouse); return; }
            (HouseModel, PlayerLabel, Marker) t = await getNearHouse(p);
            if (t.Item1 == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_NotNearHouse); return; }
            if (t.Item1.ownerId != p.sqlID) { MainChat.SendErrorChat(p, CONSTANT.ERR_HouseOwnerNotYou); return; }

            switch (args[0])
            {
                case "rentable":
                    // ! VERGİ SİSTEMİ !
                    if (t.Item1.settings.TotalTax > (t.Item1.price / 10)) { MainChat.SendErrorChat(p, "[错误] 无法执行此操作, 因为房屋是被银行征收的."); return; }

                    t.Item1.isRentable = !t.Item1.isRentable;
                    t.Item1.Update(t.Item3, t.Item2); ;
                    //t.Item2.Text = setupHouseName(t.Item1);
                    break;

                case "price":
                    // ! VERGİ SİSTEMİ !
                    if (t.Item1.settings.TotalTax > (t.Item1.price / 10)) { MainChat.SendErrorChat(p, "[错误] 无法执行此操作, 因为房屋是被银行征收的."); return; }

                    if (t.Item1.rentOwner > 0) { MainChat.SendErrorChat(p, CONSTANT.ERR_RentOwnerHaveCant); return; }
                    if (args[1].Length <= 0) { MainChat.SendErrorChat(p, CONSTANT.ERR_FineValue); return; }
                    t.Item1.rentPrice = Int32.Parse(args[1]);
                    if (t.Item1.rentPrice < 0) { t.Item1.rentPrice = 0; }
                    t.Item1.Update(t.Item3, t.Item2);
                    //t.Item2.Text = setupHouseName(t.Item1);
                    break;

                case "kick":
                    PlayerModelInfo x = await Database.DatabaseMain.getCharacterInfo(t.Item1.rentOwner);
                    t.Item1.rentOwner = 0;
                    t.Item1.Update(t.Item3, t.Item2);
                    //t.Item2.Text = setupHouseName(t.Item1);
                    MainChat.SendInfoChat(p, string.Format(CONSTANT.INFO_RentOwnerRemove, x.characterName.Replace("_", " ")));
                    break;
            }
            return;
        }

        [Command(CONSTANT.COM_LockHouse)]
        public static async Task LockHouse(PlayerModel p)
        {
            (HouseModel, PlayerLabel, Marker) t = await getNearHouse(p);
            if (t.Item1 == null)
            {

                PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Find(x => p.Position.Distance(x.Position) < 3f && x.Dimension == p.Dimension);
                if (entranceLabel == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_NotNearHouse); return; }
                entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position bussinesPos);

                t = await getHouseFromPos(bussinesPos);

                if (t.Item1 == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_NotNearHouse); return; }
            }

            if (!await HouseKeysQuery(p, t.Item1)) { MainChat.SendErrorChat(p, CONSTANT.ERR_NotOwnHouseKey); return; }

            // ! VERGİ SİSTEMİ !
            if (t.Item1.settings.TotalTax > (t.Item1.price / 10)) { MainChat.SendErrorChat(p, "[错误] 无法执行此操作, 因为房屋是被银行征收的."); return; }

            t.Item1.isLocked = !t.Item1.isLocked;
            t.Item1.Update(t.Item3, t.Item2);
            //t.Item2.Text = setupHouseName(t.Item1);
            string lockStatus = (t.Item1.isLocked) ? "锁上了房屋." : "解锁了房屋.";
            MainChat.EmoteMe(p, ServerEmotes.EMOTE_LockHouse + lockStatus);
            return;
        }

        [Command("sellhouse")]
        public static async Task SellHouse(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /sellhouse [man/system] [玩家ID] [价格]"); return; }
            (HouseModel, PlayerLabel, Marker) h = await getNearHouse(p);
            if (h.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效房屋."); return; }
            if (h.Item1.ownerId != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 此房屋不属于您!"); return; }
            switch (args[0])
            {
                case "system":
                    OtherSystem.NativeUi.Inputs.SendButtonInput(p, "是否以 $" + ((h.Item1.price - h.Item1.settings.TotalTax < 0) ? 0 : (int)((h.Item1.price) / systemSell - h.Item1.settings.TotalTax)) + " 的价格出售房屋至系统?", "House:SellToSystem", "123," + h.Item1.ID);
                    break;

                case "man":
                    // ! VERGİ SİSTEMİ !
                    if (h.Item1.settings.TotalTax > 100) { MainChat.SendErrorChat(p, "[错误] 您还未结清房屋的税款, 无法出售."); return; }

                    if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /sellhouse [man/system] [玩家ID] [价格]"); return; }
                    if (!Int32.TryParse(args[1], out int TID) || !Int32.TryParse(args[2], out int price)) { MainChat.SendInfoChat(p, "[用法] /sellhouse [man/system] [玩家ID] [价格]"); return; }

                    // ! VERGİ SİSTEMİ !
                    if (h.Item1.settings.TotalTax > (h.Item1.price / 10)) { MainChat.SendErrorChat(p, "[错误] 无法执行此操作, 因为房屋是被银行征收的."); return; }

                    PlayerModel target = GlobalEvents.GetPlayerFromSqlID(TID);
                    if (target == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家."); return; }
                    if (target.Position.Distance(p.Position) > 5) { MainChat.SendErrorChat(p, "[错误] 您离指定玩家太远."); return; }
                    if (price < 5000) { MainChat.SendErrorChat(p, "[错误] 价格无法低于5000."); return; }

                    OtherSystem.NativeUi.Inputs.SendButtonInput(target, "是否以 " + price.ToString() + " 的价格购买房屋?", "House:SellAccept", p.sqlID.ToString() + "," + h.Item1.ID.ToString() + "," + price.ToString());

                    break;

                default:
                    MainChat.SendInfoChat(p, "[用法] /sellhouse [man/system] [玩家ID] [价格]"); return;
            }
        }

        [AsyncClientEvent("House:SellToSystem")]
        public async Task EVENT_House_Sell_To_System(PlayerModel p, bool selection, string otherVal)
        {
            if (p.Ping > 250)
                return;
            if (selection)
            {
                string[] val = otherVal.Split(',');
                if (!Int32.TryParse(val[1], out int hID)) { MainChat.SendErrorChat(p, "[错误] 无效房屋ID."); return; }
                var h = await getHouseById(hID);
                if (h.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效房屋."); return; }
                if (h.Item1.ownerId != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 此房屋不属于您."); return; }
                h.Item1.rentOwner = 0;

                p.cash += (int)((h.Item1.price - h.Item1.settings.TotalTax < 0) ? 0 : (int)((h.Item1.price) / systemSell - h.Item1.settings.TotalTax));
                h.Item1.ownerId = 0;
                //h.Item2.Text = setupHouseName(h.Item1);
                h.Item1.name = "出售中";
                h.Item1.houseEnv = "[]";
                h.Item1.rentOwner = 0;
                h.Item1.isLocked = true;
                h.Item1.isRentable = false;
                h.Item1.Update(h.Item3, h.Item2);
                await Database.DatabaseMain.updateHouseKeys(h.Item1.ID, new List<Database.DatabaseMain.KeyModel>());
                await p.updateSql();
                MainChat.SendAdminChat("{#B2CB87}[信息] " + p.characterName + " 出售了房屋 " + h.Item1.ID + " 至系统.");
                return;
            }
            else
            {
                MainChat.SendInfoChat(p, "[信息] 您取消了出售.");
                return;
            }
        }

        [AsyncClientEvent("House:SellAccept")]
        public async Task House_SellAnswer(PlayerModel p, bool selection, string otherValue)
        {
            string[] _oV = otherValue.Split(",");
            if (!Int32.TryParse(_oV[0], out int tSql) || !Int32.TryParse(_oV[1], out int HID) || !Int32.TryParse(_oV[2], out int price))
                return;

            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (target == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家."); return; }

            if (!selection) { MainChat.SendErrorChat(target, "[错误] 对方拒绝了房屋出售请求."); return; }

            if (p.cash < price) { MainChat.SendInfoChat(p, "[错误] 您没有足够的钱!"); MainChat.SendInfoChat(target, "[?] 对方没有足够的钱"); return; }

            (HouseModel, PlayerLabel, Marker) h = await getHouseById(HID);
            if (h.Item1 == null)
                return;

            if (h.Item1.ownerId != target.sqlID) { MainChat.SendErrorChat(p, "[错误] 此房屋不属于对方."); return; }

            p.cash -= price;
            target.cash += price;
            await p.updateSql();
            await target.updateSql();

            h.Item1.ownerId = p.sqlID;
            h.Item1.rentOwner = 0;
            h.Item1.Update(h.Item3, h.Item2);
            List<Database.DatabaseMain.KeyModel> hKeys = new List<Database.DatabaseMain.KeyModel>();
            await Database.DatabaseMain.updateHouseKeys(h.Item1.ID, hKeys);
            MainChat.SendInfoChat(p, "[!] 恭喜您, 您购买了房屋.");
            MainChat.SendInfoChat(target, "[!] 对方购买了您的房屋.");
            MainChat.SendAdminChat("{#B2CB87}[信息] " + target.characterName + " 的房屋 " + h.Item1.ID + " 被 " + p.characterName + " 以 " + price + " 的价格购买了.");
            return;
        }

        [Command("renthouse")]
        public static async Task COM_RentHouse(PlayerModel p)
        {
            (HouseModel, PlayerLabel, Marker) h = await getNearHouse(p);
            if (h.Item1 == null)
                return;

            if (h.Item1.rentOwner != 0) { MainChat.SendErrorChat(p, "[错误] 此房屋已被租用."); return; }
            if (h.Item1.isRentable == false) { MainChat.SendErrorChat(p, "[错误] 此房屋不可租用."); return; }
            if (p.cash < h.Item1.rentPrice) { MainChat.SendErrorChat(p, CONSTANT.ERR_MoneyNotEnought); return; }

            // ! VERGİ SİSTEMİ !
            if (h.Item1.settings.TotalTax > (h.Item1.price / 10)) { MainChat.SendErrorChat(p, "[错误] 无法执行此操作, 因为房屋是被银行征收的."); return; }

            p.cash -= h.Item1.rentPrice;
            await p.updateSql();
            h.Item1.rentOwner = p.sqlID;
            h.Item1.Update(h.Item3, h.Item2);
            //h.Item2.Text = setupHouseName(h.Item1);
            h.Item2.Scale = 0.7f;
            MainChat.SendInfoChat(p, "您已租用此房屋.<br>如果没有足够的钱支付房租, 房租将作为罚金自动扣款.");
        }

        [Command("quitrenthouse")]
        public static async Task COM_LeftRent(PlayerModel p)
        {
            (HouseModel, PlayerLabel, Marker) h = await getNearHouse(p);
            if (h.Item1 == null) { return; }
            if (h.Item1.rentOwner != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 您没有租用此房屋."); return; }
            h.Item1.rentOwner = 0;
            h.Item1.isLocked = true;
            //h.Item2.Text = setupHouseName(h.Item1);
            h.Item1.Update(h.Item3, h.Item2);
            MainChat.SendInfoChat(p, "> 成功退租.");
            return;
        }

        [Command("hinv")]
        public static async Task COM_HouseEnvater(PlayerModel p)
        {

            PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Where(x => p.Position.Distance(x.Position) < 20f && x.Dimension == p.Dimension).OrderBy(x => p.Position.Distance(x.Position)).FirstOrDefault();
            if (entranceLabel == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_NotNearHouse); return; }
            entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position bussinesPos);

            (HouseModel, PlayerLabel, Marker) t = await getHouseFromPos(bussinesPos);

            if (t.Item1 == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_NotNearHouse); return; }

            if (!await HouseKeysQuery(p, t.Item1)) { MainChat.SendErrorChat(p, "[错误] 您没有此房屋的钥匙!"); return; }

            p.EmitLocked("otherEnv:Show", 3, t.Item1.houseEnv);
        }

        [Command("myhouse")]
        public static async Task COM_MyHouses(PlayerModel p)
        {
            List<HouseModel> houses = await Database.DatabaseMain.getPlayerHouses(p);

            p.SendChatMessage("<center>我的房屋</center>");
            foreach (HouseModel h in houses)
            {
                p.SendChatMessage("ID: " + h.ID.ToString() + " 名称: " + h.name + " 价格: " + h.price + " 总计税: " + h.settings.TotalTax + " 地址: /hgps " + h.ID.ToString());
            }
            p.SendChatMessage("<hr>");
            return;
        }

        [Command("hgps")]
        public static async Task COM_HouseGPS(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /hgps [房屋ID]"); return; }

            if (!Int32.TryParse(args[0], out int HID)) { MainChat.SendInfoChat(p, "[用法] /hgps [房屋ID]"); return; }

            HouseModel h = await Database.DatabaseMain.GetHouseByID(HID);
            if (h == null) { MainChat.SendErrorChat(p, "[错误] 无效房屋!"); return; }

            if (!await HouseKeysQuery(p, h)) { MainChat.SendErrorChat(p, "[错误] 此房不属于您."); return; }

            GlobalEvents.CheckpointCreate(p, h.pos, 1, 2, new Rgba(255, 0, 0, 255), "no", "no");
            MainChat.SendInfoChat(p, "[?] 已将指定房屋标记至地图.");
            return;
        }

        [Command("showhouserenters")]
        public async Task COM_ShowHouseRenters(PlayerModel p)
        {
            List<HouseModel> houses = await Database.DatabaseMain.getPlayerHouses(p.sqlID);
            string text = "<center>查看租客</center>";
            foreach (HouseModel h in houses)
            {
                string prepare = "<br>[" + h.ID + "] " + h.name + " 是否可租用: " + ((h.isRentable) ? "可出租" : "不可出租");
                if (h.isRentable)
                {
                    if (h.rentOwner == 0) { prepare += " 租户: 无"; }
                    else
                    {
                        PlayerModelInfo renter = await Database.DatabaseMain.getCharacterInfo(h.rentOwner);
                        if (renter == null)
                            continue;

                        prepare += " 租户: " + renter.characterName.Replace("_", " ");
                    }
                }
            }

            MainChat.SendInfoChat(p, text, true);
            return;
        }

        [Command("placetv")]
        public async Task COM_PlaceTVOnHome(PlayerModel p)
        {
            PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Find(x => p.Position.Distance(x.Position) < 25f && x.Dimension == p.Dimension);
            if (entranceLabel == null) { MainChat.SendErrorChat(p, "[错误] 无效房屋."); return; }
            entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position bussinesPos);

            var house = await getHouseFromPos(bussinesPos);
            if (house.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效房屋!"); return; }

            if (!await HouseKeysQuery(p, house.Item1)) { MainChat.SendErrorChat(p, "[错误] 此房屋不属于您!"); return; }
            if (house.Item1.settings.tv.hasTv) { MainChat.SendErrorChat(p, "[错误] 此房屋已有电视了."); return; }

            GlobalEvents.ShowObjectPlacement(p, "vw_prop_vw_cinema_tv_01", "House:CreateTv");
            return;
        }

        [AsyncClientEvent("House:CreateTv")]
        public async Task EVENT_CreateTV(PlayerModel p, string rot, string pos, string model)
        {
            Vector3 position = JsonConvert.DeserializeObject<Vector3>(pos);
            Vector3 rotation = JsonConvert.DeserializeObject<Vector3>(rot);

            PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Find(x => p.Position.Distance(x.Position) < 25f && x.Dimension == p.Dimension);
            if (entranceLabel == null) { MainChat.SendErrorChat(p, "[错误] 无效房屋."); return; }
            entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position bussinesPos);

            var house = await getHouseFromPos(bussinesPos);
            if (house.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效房屋."); return; }

            if (!await HouseKeysQuery(p, house.Item1)) { MainChat.SendErrorChat(p, "[错误] 此房屋不属于您!"); return; }
            if (house.Item1.settings.tv.hasTv) { MainChat.SendErrorChat(p, "[错误] 此房屋已有电视了."); return; }

            house.Item1.settings.tv.hasTv = true;
            house.Item1.settings.tv.Position = position;
            house.Item1.settings.tv.Rotation = rotation;
            LProp tv = PropStreamer.Create(model, position, rotation, house.Item1.dimension, false, false, true);
            house.Item1.settings.tv.TvPropID = tv.Id;
            house.Item1.Update(house.Item3, house.Item2);
            MainChat.SendInfoChat(p, "[?] 已放置电视, 使用 /htv 操作电视.");
            return;
        }

        [Command("htv")]
        [Obsolete]
        public async Task COM_HouseTV(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /htv ...."); return; }

            PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Where(x => p.Position.Distance(x.Position) < 50f && x.Dimension == p.Dimension).OrderBy(x => p.Position.Distance(x.Position)).FirstOrDefault();
            if (entranceLabel == null) { MainChat.SendErrorChat(p, "[错误] 无效房屋."); return; }
            entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position bussinesPos);

            var house = await getHouseFromPos(bussinesPos);
            if (house.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效房屋!"); return; }
            if (!await HouseKeysQuery(p, house.Item1)) { MainChat.SendErrorChat(p, "[错误] 此房屋不属于您!"); return; }

            switch (args[0])
            {
                case "link":
                    if (args[1] == null) { MainChat.SendErrorChat(p, "无效音乐链接."); return; }
                    string link = "https://xbeibeix.com/api/bilibili/biliplayer/?url=" + args[1];

                    house.Item1.settings.tv.URL = link;
                    house.Item1.settings.tv.StartTime = DateTime.Now;
                    house.Item1.Update(house.Item3, house.Item2);
                    foreach (PlayerModel target in Alt.GetAllPlayers())
                    {
                        if (target.Position.Distance(p.Position) <= 40 && target.Dimension == p.Dimension)
                        {
                            await target.EmitAsync("TV:Start", link, 0, house.Item1.settings.tv.Volume);
                        }
                    }
                    MainChat.EmoteDo(p, "此人操作了电视.");
                    return;

                case "vol":
                    if (!Int32.TryParse(args[1], out int newSound)) { MainChat.SendErrorChat(p, "[错误] 无效音量."); return; }
                    house.Item1.settings.tv.Volume = newSound;
                    house.Item1.Update(house.Item3, house.Item2);
                    foreach (PlayerModel target in Alt.GetAllPlayers())
                    {
                        if (target.Position.Distance(p.Position) <= 40 && target.Dimension == p.Dimension)
                        {
                            await target.EmitAsync("TV:SetSound", newSound);
                        }
                    }
                    return;

                case "stop":
                    house.Item1.settings.tv.URL = "无";

                    house.Item1.Update(house.Item3, house.Item2);
                    foreach (PlayerModel target in Alt.GetAllPlayers())
                    {
                        if (target.Position.Distance(p.Position) <= 40 && target.Dimension == p.Dimension)
                        {
                            await target.EmitAsync("TV:Stop");
                        }
                    }
                    break;

                case "remove":
                    if (!house.Item1.settings.tv.hasTv) { MainChat.SendErrorChat(p, "[错误] 此房屋没有电视."); return; }
                    LProp TvProp = PropStreamer.GetProp(house.Item1.settings.tv.TvPropID);
                    if (TvProp == null) { MainChat.SendErrorChat(p, "[错误] 无电视物件!"); return; }
                    TvProp.Destroy();
                    house.Item1.settings.tv = new HouseModel.TVModel();
                    house.Item1.Update(house.Item3, house.Item2);
                    MainChat.SendInfoChat(p, "[?] 已移除电视.");
                    return;

                default:

                    return;
            }
        }

        [Obsolete]
        public static void EVENT_StartTV(PlayerModel p, HouseModel h)
        {
            int second = 0;
            second = (int)(DateTime.Now - h.settings.tv.StartTime).TotalSeconds;
            p.EmitAsync("TV:Start", h.settings.tv.URL, second, h.settings.tv.Volume);
            return;
        }

        [Obsolete]
        public static void EVENT_StopTV(PlayerModel p)
        {
            p.EmitAsync("TV:Stop");
        }

        [Command("payhousetax")]
        public async Task COM_PayHouseTax(PlayerModel p)
        {
            var house = await getNearHouse(p);
            if (house.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效房屋!"); return; }
            if (house.Item1.ownerId != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 此房屋不属于您."); return; }

            OtherSystem.NativeUi.Inputs.SendButtonInput(p, "是否支付房屋税款 $" + house.Item1.settings.TotalTax, "House:PayTax", "none");
            return;
        }

        [AsyncClientEvent("House:PayTax")]
        public async Task EVENT_PayHouseTax(PlayerModel p, bool selection)
        {
            if (!selection)
                return;
            var house = await getNearHouse(p);
            if (house.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效房屋!"); return; }
            if (house.Item1.ownerId != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 此房屋不属于您."); return; }

            if (p.cash < house.Item1.settings.TotalTax) { MainChat.SendErrorChat(p, "[错误] 您没有足够的钱!"); return; }
            p.cash -= house.Item1.settings.TotalTax;
            await p.updateSql();

            house.Item1.settings.TotalTax = 0;
            house.Item1.Update(house.Item3, house.Item2);

            MainChat.SendInfoChat(p, "[?] 成功支付房屋税.");
            return;
        }

        [Command("hlight")]
        public async Task COM_HouseLight(PlayerModel p)
        {
            (HouseModel, PlayerLabel, Marker) t = await getNearHouse(p);
            if (t.Item1 == null)
            {

                PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Where(x => p.Position.Distance(x.Position) < 20f && x.Dimension == p.Dimension).OrderBy(x => p.Position.Distance(x.Position)).FirstOrDefault();
                if (entranceLabel == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_NotNearHouse); return; }
                entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position bussinesPos);

                t = await getHouseFromPos(bussinesPos);

                if (t.Item1 == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_NotNearHouse); return; }
            }

            if (!await HouseKeysQuery(p, t.Item1)) { MainChat.SendErrorChat(p, CONSTANT.ERR_NotOwnHouseKey); return; }

            t.Item1.settings.Light = !t.Item1.settings.Light;

            MainChat.EmoteMe(p, " 按下了开关" + ((t.Item1.settings.Light) ? "并打开了房屋的灯" : "并关闭了房屋的灯"));
            t.Item1.Update(t.Item3, t.Item2);

            GlobalEvents.SetLightState(p, t.Item1.settings.Light);

            foreach (PlayerModel ta in Alt.GetAllPlayers())
            {
                if (ta.Position.Distance(p.Position) < 50 && ta.Dimension == p.Dimension)
                {
                    GlobalEvents.SetLightState(ta, t.Item1.settings.Light);
                }
            }
        }
    }
}
