using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using AltV.Net.Resources.Chat.Api;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Core;
using outRp.Models;
using outRp.OtherSystem;
using outRp.OtherSystem.LSCsystems;
using outRp.OtherSystem.Textlabels;
using outRp.Props;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using outRp.Kook;

namespace outRp.Globals.Commands
{
    public class AdminCommands : IScript
    {
        [Command("cktakipac")]
        public void COM_OpenCKwatch(PlayerModel p)
        {
            if (p.adminLevel <= 5)
                return;

            p.admiCkWatching = !p.admiCkWatching;
        }

        public static List<System.News.AdversimentModel> trustedAdversiments = new List<System.News.AdversimentModel>();
        // Listeners 
        [AsyncClientEvent("freecam:Update")]
        public void AdminCameraUpdate(PlayerModel p, string pos)
        {
            Position position = JsonConvert.DeserializeObject<Position>(pos);
            p.Position = position;
        }

        // Listeners Enn \\ 

        [Command(CONSTANT.COM_CreateBusiness)]
        public async Task COM_CreateBussines(PlayerModel player, params string[] args)
        {
            if (player.adminLevel < 5) { MainChat.SendErrorChat(player, CONSTANT.ERR_AdminLevel); return; }
            if (args.Length <= 2) { MainChat.SendErrorChat(player, "[用法] /createbusiness [类型: 1-2-3..] [价格] [名称(可使用空格)] "); return; }
            //int Cesit, int Fiyat, params string[] values
            if (!Int32.TryParse(args[0], out int Cesit) || !Int32.TryParse(args[1], out int Fiyat)) { MainChat.SendErrorChat(player, "[用法] /createbusiness [类型: 1-2-3..] [价格] [名称(可使用空格)] "); return; }


            string value = string.Join(" ", args[2..]);
            await Business.BusinessCreate(player, Cesit, Fiyat, value);
            return;
        }

        [Command(CONSTANT.COM_DeleteBusiness)]
        public async Task COM_DeleteBusiness(PlayerModel player, int ID)
        {
            if (player.adminLevel < 6) { MainChat.SendErrorChat(player, CONSTANT.ERR_AdminLevel); return; }
            await Business.BusinessDelete(player, ID);
            return;
        }

        [Command(CONSTANT.COM_UpdateBusiness)]
        public async Task COM_UpdatteBusiness(PlayerModel player, params string[] args)
        {
            if (player.adminLevel < 5) { MainChat.SendErrorChat(player, CONSTANT.ERR_AdminLevel); return; }
            if (args.Length <= 1) { MainChat.SendErrorChat(player, CONSTANT.COM_BusinessUpdateDesc); return; }
            await Business.BusinessUpdateInfo(player, args);
            return;
        }

        [Command(CONSTANT.COM_Goto)]
        public void COM_Goto(PlayerModel player, int? ID)
        {
            if (player.adminLevel < 3) { MainChat.SendErrorChat(player, CONSTANT.ERR_AdminLevel); return; }

            if (ID == null) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotFound); return; }
            PlayerModel target = GlobalEvents.GetPlayerFromSqlID((int)ID);
            if (target == null) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotFound); return; }

            Position pos = target.Position;
            pos.X = pos.X + 1f;
            player.Position = pos;
            player.Dimension = target.Dimension;

            string playerMsg = string.Format(CONSTANT.INFO_COM_Goto_Player, target.characterName.Replace("_", " "));
            string targetMsg = string.Format(CONSTANT.INFO_COM_Goto_Target, player.characterName.Replace("_", " "));

            MainChat.SendInfoChat(player, playerMsg);
            MainChat.SendInfoChat(target, targetMsg);
            return;
        }

        [Command(CONSTANT.COM_Get)]
        public void COM_Get(PlayerModel player, int? ID)
        {
            if (player.adminLevel < 3) { MainChat.SendErrorChat(player, CONSTANT.ERR_AdminLevel); return; }

            if (ID == null) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotFound); return; }
            PlayerModel target = GlobalEvents.GetPlayerFromSqlID((int)ID);
            if (target == null) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotFound); return; }

            Position pos = player.Position;
            pos.X = pos.X + 1f;
            target.Position = pos;
            target.Dimension = player.Dimension;

            string playerMsg = string.Format(CONSTANT.INFO_COM_Get_Player, target.characterName.Replace("_", " "));
            string targetMsg = string.Format(CONSTANT.INFO_COM_Get_Target, player.characterName.Replace("_", " "));

            MainChat.SendInfoChat(player, playerMsg);
            MainChat.SendInfoChat(target, targetMsg);
            return;
        }

        [Command(CONSTANT.COM_EditCharacter)]
        public async Task COM_EditCharacter(PlayerModel player, params string[] type)
        {

            if (type.Length <= 0) { MainChat.SendInfoChat(player, CONSTANT.DESC_EditCharacter); return; }
            if (!Int32.TryParse(type[0], out int targetId)) { MainChat.SendInfoChat(player, CONSTANT.DESC_EditCharacter); return; }
            if (player.adminLevel < 5) { MainChat.SendErrorChat(player, CONSTANT.ERR_AdminLevel); return; }

            if (targetId == 0) { MainChat.SendErrorChat(player, CONSTANT.DESC_EditCharacter); return; }
            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(targetId);
            if (t == null) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotFound); return; }
            //CharacterModel t = GlobalEvents.PlayerGetData(target);
            CharacterSettings settings = JsonConvert.DeserializeObject<CharacterSettings>(t.settings);

            switch (type[1])
            {
                case "name":
                    t.characterName = type[2] + "_" + type[3];
                    break;

                case "age":
                    if (!Int32.TryParse(type[2], out int newAge))
                        return;
                    t.characterAge = newAge;
                    break;

                case "level":
                    if (!Int32.TryParse(type[2], out int newLevel))
                        return;
                    t.characterLevel = newLevel;
                    break;

                case "factionid":
                    if (!Int32.TryParse(type[2], out int newFactionID))
                        return;
                    t.factionId = newFactionID;
                    break;

                case "cash":
                    if (player.adminLevel < 7) { MainChat.SendErrorChat(player, CONSTANT.ERR_PermissionError); return; }
                    if (!Int32.TryParse(type[2], out int newCash))
                        return;
                    t.cash = newCash;
                    break;


                case "jailtime":
                    if (!Int32.TryParse(type[2], out int newJail))
                        return;
                    t.jailTime = newJail;
                    break;

                case "bizstaff":
                    if (!Int32.TryParse(type[2], out int newBusiness))
                        return;
                    t.businessStaff = newBusiness;
                    break;

                case "nation":
                    settings.nation = type[2];
                    break;

                case "payday":
                    if (!Int32.TryParse(type[2], out int newPayday))
                        return;
                    t.gameTime = newPayday;
                    break;

                case "str":
                    if (!Int32.TryParse(type[2], out int newstr))
                        return;
                    t.Strength = newstr;
                    break; //

                case "born":
                    var locationSet = JsonConvert.DeserializeObject<CharacterSettings>(t.settings);
                    if (locationSet == null) { MainChat.SendErrorChat(player, "[错误] 获取角色信息时出错."); return; }
                    locationSet.location = string.Join(" ", type[2..]);
                    t.settings = JsonConvert.SerializeObject(locationSet);
                    await t.updateSql();
                    break;

                default:
                    MainChat.SendInfoChat(player, CONSTANT.DESC_EditCharacter);
                    return;
            }

            t.settings = JsonConvert.SerializeObject(settings);
            await t.updateSql();

            string playerMsg = string.Format(CONSTANT.INFO_COM_EditCharacter_Player, t.characterName.Replace("_", " "), type[1], type[2]);
            string targetMsg = string.Format(CONSTANT.INFO_COM_EditCharacter_Target, player.characterName.Replace("_", " "), type[1], type[2]);

            MainChat.SendErrorChat(player, playerMsg);
            MainChat.SendErrorChat(t, targetMsg);
            return;
        }

        [Command(CONSTANT.COM_CreateVehicle)]
        public async Task COM_CreateVehicle(PlayerModel player, string type)
        {
            if (player.adminLevel < 7) { MainChat.SendErrorChat(player, CONSTANT.ERR_AdminLevel); return; }
            if (type.Length < 0) { MainChat.SendErrorChat(player, CONSTANT.DESC_CreateVehicle); return; }

            IVehicle v = Alt.CreateVehicle(type, player.Position, player.Rotation);
            VehModel veh = (VehModel)v;

            veh.sqlID = await Database.DatabaseMain.CreateVehicle((VehModel)v);
            return;
        }

        [Command(CONSTANT.COM_EditVehicle)]
        public async Task COM_EditVehicle(PlayerModel player, params string[] values)
        {
            if (player.adminLevel < 5) { MainChat.SendErrorChat(player, CONSTANT.ERR_AdminLevel); return; }
            if (values.Length <= 0) { MainChat.SendErrorChat(player, CONSTANT.DESC_EditVehicle); return; }
            VehModel veh = Vehicle.VehicleMain.getNearVehFromPlayer(player);
            if (veh == null) { MainChat.SendErrorChat(player, CONSTANT.COM_VQueryVehNotNear); return; }

            switch (values[0].ToString())
            {
                case "model":
                    if (values[1] == null) { MainChat.SendErrorChat(player, CONSTANT.DESC_EditVehicle); return; }
                    VehicleModel model;
                    Enum.TryParse(values[1].ToString(), out model);
                    veh.Model = (uint)model;
                    break;

                case "owner":
                    if (values[1] == null) { MainChat.SendErrorChat(player, CONSTANT.DESC_EditVehicle); return; }
                    if (!Int32.TryParse(values[1], out int newOwner))
                        return;
                    veh.owner = newOwner;
                    break;

                case "jobid":
                    if (values[1] == null) { MainChat.SendErrorChat(player, CONSTANT.DESC_EditVehicle); return; }
                    if (!Int32.TryParse(values[1], out int newJob))
                        return;
                    veh.jobId = newJob;
                    await Database.DatabaseMain.UpdateVehicleInfo(veh);
                    break;

                case "maincolor":
                    if (values[1] == null) { MainChat.SendErrorChat(player, CONSTANT.DESC_EditVehicle); return; }
                    string[] fColors = values[1].ToString().Split(",");
                    if (!byte.TryParse(fColors[0], out byte b1) || !byte.TryParse(fColors[1], out byte b2) || !byte.TryParse(fColors[2], out byte b3) || !byte.TryParse(fColors[3], out byte b4))
                        return;

                    veh.PrimaryColorRgb = new Rgba(b1, b2, b3, b4);
                    break;

                case "seccolor":
                    if (values[1] == null) { MainChat.SendErrorChat(player, CONSTANT.DESC_EditVehicle); return; }
                    string[] sColors = values[1].ToString().Split(",");
                    if (!byte.TryParse(sColors[0], out byte s1) || !byte.TryParse(sColors[1], out byte s2) || !byte.TryParse(sColors[2], out byte s3) || !byte.TryParse(sColors[3], out byte s4))
                        return;

                    veh.SecondaryColorRgb = new Rgba(s1, s2, s3, s4);
                    break;

                case "factionid":
                    if (values[1] == null) { MainChat.SendErrorChat(player, CONSTANT.DESC_EditVehicle); return; }
                    if (!Int32.TryParse(values[1], out int newFactionid))
                        return;
                    veh.factionId = newFactionid;
                    break;

                case "fuel":
                    if (values[1] == null) { MainChat.SendErrorChat(player, CONSTANT.DESC_EditVehicle); return; }
                    if (!Int32.TryParse(values[1], out int newFuel))
                        return;
                    veh.currentFuel = newFuel;
                    break;

                case "plate":
                    if (values[1] == null) { MainChat.SendErrorChat(player, CONSTANT.DESC_EditVehicle); return; }
                    veh.NumberplateText = values[1].ToString();
                    break;

                case "lock":
                    if (veh.LockState == VehicleLockState.Unlocked) { veh.LockState = VehicleLockState.Locked; break; }
                    else { veh.LockState = VehicleLockState.Unlocked; break; }

                case "engine":
                    if (veh.EngineOn == true) { veh.EngineOn = false; break; }
                    else { veh.EngineOn = true; break; }

                case "price":
                    if (values[1] == null) { MainChat.SendErrorChat(player, CONSTANT.DESC_EditVehicle); return; }
                    if (!Int32.TryParse(values[1], out int newPrice))
                        return;
                    veh.price = newPrice;
                    break;

                case "tax":
                    if (values[1] == null) { MainChat.SendErrorChat(player, CONSTANT.DESC_EditVehicle); return; }
                    if (!Int32.TryParse(values[1], out int newTax))
                        return;
                    veh.defaultTax = newTax;

                    break;

                case "impound":
                    if (values[1] == null) { MainChat.SendErrorChat(player, CONSTANT.DESC_EditVehicle); return; }
                    if (!Int32.TryParse(values[1], out int newFine))
                        return;
                    veh.fine = newFine;

                    if (veh.fine > (veh.price / 10))
                    {
                        veh.towwed = true;
                        veh.EngineOn = false;
                    }
                    else
                    {
                        veh.towwed = false;
                    }
                    break;

                case "engineboost":
                    if (values[1] == null) { MainChat.SendErrorChat(player, CONSTANT.DESC_EditVehicle); return; }
                    if (!float.TryParse(values[1], out float newMotor))
                        return;
                    veh.engineBoost = newMotor;
                    break;

                case "trunksize":
                    if (values[1] == null) { MainChat.SendErrorChat(player, CONSTANT.DESC_EditVehicle); return; }
                    if (!Int32.TryParse(values[1], out int newInv))
                        return;
                    veh.inventoryCapacity = newInv;
                    break;

                case "maxfuel":
                    if (values[1] == null) { MainChat.SendErrorChat(player, CONSTANT.DESC_EditVehicle); return; }
                    if (!Int32.TryParse(values[1], out int newMaxFuel))
                        return;
                    veh.maxFuel = newMaxFuel;
                    break;

                case "fuelused":
                    if (values[1] == null) { MainChat.SendErrorChat(player, CONSTANT.DESC_EditVehicle); return; }
                    if (!Int32.TryParse(values[1], out int newPetrol))
                        return;
                    veh.fuelConsumption = newPetrol;
                    break;

                case "km":
                    if (values[1] == null) { MainChat.SendErrorChat(player, "[错误] 无效"); return; }
                    if (!Int32.TryParse(values[1], out int newKM))
                        return;

                    veh.km = (double)newKM;
                    break;

                default:
                    MainChat.SendInfoChat(player, CONSTANT.DESC_EditVehicle);
                    return;
            }

            veh.Update();
            MainChat.SendInfoChat(player, "成功更新.");
            return;
        }

        [Command(CONSTANT.COM_ADice6)]
        public void COM_ADice6(PlayerModel p, int value)
        {
            if (p.adminLevel < CONSTANT.LVL_ADice6) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; }
            if (value == 0) { MainChat.SendErrorChat(p, CONSTANT.DESC_ADice6); return; }
            MainChat.EmoteMe(p, " 掷了一个骰子.");
            MainChat.EmoteDoAlternative(p, "点数是 " + value.ToString());
            return;
        }

        [Command(CONSTANT.COM_CreateHouse)]
        public async Task CreateHouse(PlayerModel player, params string[] args)
        {
            if (player.adminLevel < 5) { MainChat.SendErrorChat(player, CONSTANT.ERR_AdminLevel); return; }
            if (args.Length <= 0) { MainChat.SendErrorChat(player, CONSTANT.DESC_CreateHouse); return; }
            if (!Int32.TryParse(args[0], out int price))
                return;

            if (price <= 0) { MainChat.SendErrorChat(player, CONSTANT.ERR_ValueNotNegative); return; }
            HouseModel h = new HouseModel()
            {
                price = price,
                pos = player.Position,
            };
            int currID = await h.Create();
            MainChat.SendInfoChat(player, "[!] 成功创建房屋, ID: " + currID);
            return;
        }

        [Command(CONSTANT.COM_AEditHouse)]
        public async Task EditHouse(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; }
            if (args.Length <= 1) { MainChat.SendInfoChat(p, CONSTANT.DESC_AEditHouse); return; }

            if (!Int32.TryParse(args[0], out int hID))
                return;

            (HouseModel, PlayerLabel, Marker) t = await Houses.getHouseById(hID);

            switch (args[1])
            {
                case "name":
                    if (args.Length <= 2)
                        return;
                    t.Item1.name = string.Join(" ", args[2..]);
                    t.Item1.Update(t.Item3, t.Item2);
                    break;

                case "price":
                    if (args.Length <= 2)
                        return;
                    if (!Int32.TryParse(args[2], out int newPrice))
                        return;

                    t.Item1.price = newPrice;
                    t.Item1.Update(t.Item3, t.Item2);
                    break;

                case "owner":
                    if (args.Length <= 2)
                        return;

                    if (!Int32.TryParse(args[2], out int newOwner))
                        return;
                    t.Item1.ownerId = newOwner;
                    t.Item1.Update(t.Item3, t.Item2);
                    break;

                case "exitpos":
                    t.Item1.intPos = p.Position;
                    t.Item1.Update(t.Item3, t.Item2);
                    break;

                case "lock":
                    t.Item1.isLocked = !t.Item1.isLocked;
                    t.Item1.Update(t.Item3, t.Item2);
                    break;

                case "dimension":
                    if (args.Length <= 2)
                        return;
                    if (!Int32.TryParse(args[2], out int newDimension))
                        return;
                    t.Item1.dimension = newDimension;
                    t.Item1.Update(t.Item3, t.Item2);
                    break;

                case "enterpos":
                    t.Item1.pos = p.Position;
                    t.Item1.Update(t.Item3, t.Item2);
                    break;
                case "markertype":
                    if (!Int32.TryParse(args[2], out int newMarkerType))
                        return;

                    if (newMarkerType > 43)
                        return;

                    t.Item1.settings.MarkerType = newMarkerType;
                    t.Item1.Update(t.Item3, t.Item2);
                    return;

                case "markerscale":
                    if (args.Length < 4)
                        return;

                    if (!float.TryParse(args[2], out float ns1) || !float.TryParse(args[3], out float ns2) || !float.TryParse(args[4], out float ns3))
                        return;

                    t.Item1.settings.markerScale = new Vector3(ns1, ns2, ns3);
                    t.Item1.Update(t.Item3, t.Item2);
                    return;

                case "markercolor":
                    if (args.Length < 5)
                        return;

                    if (!Int32.TryParse(args[2], out int nc1) || !Int32.TryParse(args[3], out int nc2) || !Int32.TryParse(args[4], out int nc3) || !Int32.TryParse(args[5], out int nc4))
                        return;

                    t.Item1.settings.MarkerColor = new Rgba((byte)nc1, (byte)nc2, (byte)nc3, (byte)nc4);
                    t.Item1.Update(t.Item3, t.Item2);
                    return;

                case "interiorset":
                    if (args[2] == null)
                        return;

                    t.Item1.settings.houseInterior = args[2];
                    t.Item1.Update(t.Item3, t.Item2);
                    MainChat.SendInfoChat(p, "[!] 房屋内饰调整完成.");
                    return;

                case "interiorplus":
                    if (args[2] == null)
                        return;

                    HouseModel.Settings.interiorSettings newInteriorPlus = new HouseModel.Settings.interiorSettings() { entitySet = args[2] };
                    t.Item1.settings.houseInteriorSettings.Add(newInteriorPlus);
                    t.Item1.Update(t.Item3, t.Item2);

                    MainChat.SendInfoChat(p, "[!] 房屋内饰添加完成.");
                    return;

                case "interiorplusclear":
                    t.Item1.settings.houseInteriorSettings.Clear();
                    t.Item1.Update(t.Item3, t.Item2);

                    MainChat.SendInfoChat(p, "[!] 房屋内饰已添加清理完成.");
                    return;

                case "setint":
                    if (args.Length < 4)
                        return;

                    if (!float.TryParse(args[2], out float np1) || !float.TryParse(args[3], out float np2) || !float.TryParse(args[4], out float np3))
                        return;

                    t.Item1.intPos = new Position(np1, np2, np3);
                    t.Item1.Update(t.Item3, t.Item2);
                    MainChat.SendInfoChat(p, "[!] 房屋内饰位置调整为: " + np1.ToString() + "-" + np2.ToString() + "-" + np3.ToString() + " olarak ayarlandı!");
                    return;

                case "totaltax":
                    if (args.Length < 2)
                        return;

                    if (!Int32.TryParse(args[2], out int newTotalTax))
                        return;

                    t.Item1.settings.TotalTax = newTotalTax;
                    t.Item1.Update(t.Item3, t.Item2);

                    MainChat.SendInfoChat(p, "[?] 成功设置房屋总计税.");
                    return;

                case "tax":
                    if (args.Length < 2)
                        return;

                    if (!Int32.TryParse(args[2], out int newTax))
                        return;

                    t.Item1.settings.Tax = newTax;
                    t.Item1.Update(t.Item3, t.Item2);
                    MainChat.SendInfoChat(p, "[!] 成功设置房屋税.");
                    return;
            }
            MainChat.SendInfoChat(p, "[?] 房屋已更新");
            return;
        }

        [Command(CONSTANT.COM_CharacterCreateSend)]
        public void COM_CharCreationMenu(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 

            if (args.Length <= 0) { MainChat.SendErrorChat(p, CONSTANT.DESC_CharacterCreateSend); return; }
            if (!Int32.TryParse(args[0], out int sqlId))
                return;

            //int sqlId = Int32.Parse(args[0].ToString());
            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(sqlId);
            if (t == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }
            t.EmitLocked("character:Redit", t.charComps);
            MainChat.SendInfoChat(p, "已指定玩家创建角色.");
            return;
        }

        [Command(CONSTANT.COM_TpToOtherPlayer)]
        public void COM_TpToPlayer(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 3) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 

            if (args.Length <= 1) { MainChat.SendErrorChat(p, CONSTANT.DESC_TpToOtherPlayer); return; }

            if (!Int32.TryParse(args[0], out int t1SqlId) || !Int32.TryParse(args[1], out int t2SqlId))
                return;

            PlayerModel t1 = GlobalEvents.GetPlayerFromSqlID(t1SqlId);
            PlayerModel t2 = GlobalEvents.GetPlayerFromSqlID(t2SqlId);
            if (t1 == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }
            if (t2 == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }
            t2.Position = t1.Position;
            MainChat.SendInfoChat(t1, p.characterName.Replace("_", " ") + " 将 " + t2.characterName.Replace("_", " ") + " 拉到了您的身边.", true);
            MainChat.SendInfoChat(t2, p.characterName.Replace("_", " ") + " 将您传送至 " + t1.characterName.Replace("_", " ") + " 的旁边.", true);
            MainChat.SendInfoChat(p, t2.characterName.Replace("_", " ") + " 被传送至 " + t1.characterName.Replace("_", " ") + " 的位置.");
        }

        [Command(CONSTANT.COM_Announcement)]
        public void COM_Announcemet(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 

            if (args.Length <= 0) { MainChat.SendErrorChat(p, CONSTANT.DESC_Announcement); return; }
            string msg = string.Join(" ", args);
            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                MainChat.SendInfoChat(t, "{FF0000}<center>[公告]</center>{FFFFFF}<br>" + msg, true);
            }
        }

        [Command(CONSTANT.COM_KickPlayer)]
        public async Task COM_Kick(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 1) { MainChat.SendErrorChat(p, CONSTANT.DESC_KickPlayer); return; }

            AccountModel adminAcc = await Database.DatabaseMain.getAccInfo(p.accountId);
            if (adminAcc == null)
            {
                MainChat.SendErrorChat(p, "[!] 发生了错误.");
                return;
            }
            if (!Int32.TryParse(args[0], out int ID))
                return;
            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(ID);
            string reason = string.Join(" ", args[1..]);
            target.Kick(reason);

            await Database.DatabaseMain.AddAccountLog(2, target.sqlID, target.accountId, reason, adminAcc.kookId);

            foreach (PlayerModel mT in Alt.GetAllPlayers())
            {
                MainChat.SendInfoChat(mT, "{C63D3D}" + target.characterName.Replace("_", " ") + " {FFFFFF}被 {C63D3D}" + p.characterName.Replace("_", " ") + " {FFFFFF}踢出服务器. {C63D3D}原因: {FFFFFF}" + reason);
            }
            return;
        }

        [Command(CONSTANT.COM_SKickPlayer)]
        public void COM_SKick(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 6) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 1) { MainChat.SendErrorChat(p, CONSTANT.DESC_SKickPlayer); return; }

            if (!Int32.TryParse(args[0], out int ID))
                return;
            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(ID);
            string reason = string.Join(" ", args[1..]);
            target.Kick(reason);
        }

        [Command(CONSTANT.COM_GotoBusiness)]
        public async Task COM_BGoto(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 4) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 0) { MainChat.SendErrorChat(p, CONSTANT.DESC_GotoBusiness); return; }

            if (!Int32.TryParse(args[0], out int ID))
                return;

            (BusinessModel, Marker, PlayerLabel) b = await Props.Business.getBusinessById(ID);
            p.Position = b.Item1.position;
        }

        [Command(CONSTANT.COM_GotoHouse)]
        public async Task COM_HGoto(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 4) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 0) { MainChat.SendErrorChat(p, CONSTANT.DESC_GotoHouse); return; }

            if (!Int32.TryParse(args[0], out int ID))
                return;
            (HouseModel, PlayerLabel, Marker) b = await Houses.getHouseById(ID);
            p.Position = b.Item1.pos;
        }

        [Command(CONSTANT.COM_GotoCar)]
        public void COM_CGoto(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 4) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 0) { MainChat.SendErrorChat(p, CONSTANT.DESC_GotoCar); return; }

            if (!Int32.TryParse(args[0], out int ID))
                return;

            VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(ID);
            Position pos = v.Position;
            pos.X += 1;
            p.Position = pos;
            p.Dimension = v.Dimension;
            return;
        }

        [Command(CONSTANT.COM_GetCar)]
        public void COM_CGet(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 4) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle .
            if (args.Length <= 0) { MainChat.SendErrorChat(p, CONSTANT.DESC_GetCar); return; }

            if (!Int32.TryParse(args[0], out int ID))
                return;
            VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(ID);
            if (v == null)
            {
                MainChat.SendErrorChat(p, "[错误] 没有这台车.");
                return;
            }
            v.ResetNetworkOwner();
            Position pos = p.Position;
            pos.X += 1;
            v.Position = pos;
            v.Dimension = p.Dimension;
            MainChat.SendInfoChat(p, "[!] 成功将ID为 " + v.sqlID + " 的车辆传送过来.");
            return;
        }

        [Command(CONSTANT.COM_AWork)]
        public async Task COM_Awork(PlayerModel p)
        {
            if (p.adminLevel < 1) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; }

            p.adminWork = !p.adminWork;
            if (p.adminLevel > 4)
            {
                if (p.adminWork)
                {
                    AccountModel adminAcc = await Database.DatabaseMain.getAccInfo(p.accountId);
                    p.fakeName = adminAcc.forumName;
                }
                else
                {
                    p.fakeName = p.characterName;
                }
            }

            string awork = (p.adminWork) ? "{17FF00}开启" : "{A40505}关闭";
            p.SendChatMessage("> 管理员执勤状态: " + awork);
        }

        [Command(CONSTANT.COM_ShowAdversiments)]
        public void COM_ShowAdversiments(PlayerModel p)
        {
            if (p.adminLevel < 2) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle .
            if (System.News.serverAdversiment.Count <= 0) { MainChat.SendInfoChat(p, "[!] 无广告."); return; }
            foreach (var x in System.News.serverAdversiment)
            {
                p.SendChatMessage("> 玩家ID:" + x.senderID.ToString());
                p.SendChatMessage("> 广告内容: " + x.addvesimentText);
            }
        }

        //.
        [Command(CONSTANT.COM_AnswerAdversiment)]
        public async Task COM_EventAdversiment(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 2) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 1) { MainChat.SendErrorChat(p, CONSTANT.DESC_AnswerAdversiment); return; }
            AccountModel acc = await Database.DatabaseMain.getAccInfo(p.accountId);

            if (!Int32.TryParse(args[0], out int addID))
                return;
            var x = System.News.serverAdversiment.Find(x => x.senderID == addID);
            if (x == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_AnswerAdversiment); return; }

            switch (args[1])
            {
                case "app":
                    trustedAdversiments.Add(x);
                    System.News.serverAdversiment.Remove(x);
                    PlayerModel t = GlobalEvents.GetPlayerFromSqlID(x.senderID);
                    if (t != null)
                        MainChat.SendInfoChat(t, "[!] 您的广告被 " + acc.forumName + " 批准通过并自动列入了广告投放列表, 每 3分钟 投放一次, 输入 /nextad 可查看下一个投放的广告.");

                    MainChat.SendInfoChat(p, "[!] 成功批准广告投放, 批准通过并自动列入了广告投放列表, 每 3分钟 投放一次.");
                    acc.AdversimentCount += 1;
                    acc.Update();
                    return;

                case "dec":
                    PlayerModel xT = GlobalEvents.GetPlayerFromSqlID(x.senderID);
                    if (xT != null) { MainChat.SendErrorChat(xT, "{C62204}> 您的广告被 " + acc.forumName + " 拒绝投放, 可能内容不符合规定, 请查看 IC广告发布规则."); }
                    System.News.serverAdversiment.Remove(x);
                    acc.AdversimentCount += 1;
                    acc.Update();
                    return;

                default:
                    trustedAdversiments.Add(x);
                    System.News.serverAdversiment.Remove(x);
                    PlayerModel ta = GlobalEvents.GetPlayerFromSqlID(x.senderID);
                    if (ta != null)
                        MainChat.SendInfoChat(ta, "[!] 您的广告被 " + acc.forumName + " 批准通过并自动列入了广告投放列表, 每 3分钟 投放一次, 输入 /nextad 可查看下一个投放的广告");

                    MainChat.SendInfoChat(p, "[!] 成功批准广告投放, 批准通过并自动列入了广告投放列表, 每 3分钟 投放一次.");
                    acc.AdversimentCount += 1;
                    acc.Update();
                    return;
            }
        }

        public static async void Event_ShowAutomaticAdversiment()
        {
            if (trustedAdversiments.Count <= 0)
                return;
            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (t.Exists)
                {
                    if (t.phoneNumber != 0)
                    {
                        t.EmitLocked("Phone:AddAdversiment", trustedAdversiments[0].pureText, trustedAdversiments[0].pureNumber);
                    }
                    if (t.isNews)
                    {
                        t.SendChatMessage(trustedAdversiments[0].addvesimentText);
                    }
                }
            }
            string kook_msg = "转发IC广告:\n📕广告内容:" + trustedAdversiments[0].pureText + "\n📞联系电话:" + trustedAdversiments[0].pureNumber;
            await KookSpace.AdMessage(kook_msg);
            trustedAdversiments.RemoveAt(0);
            return;
        }

        [Command(CONSTANT.COM_Reports)]
        public void COM_ShowAllReports(PlayerModel p)
        {
            if (p.adminLevel < 4) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            p.SendChatMessage("{F13A00}<center>______ [ 举报列表 ] ______</center>");
            if (ReportHelp.serverReports.Count <= 0) { p.SendChatMessage(CONSTANT.ERR_Reports); return; }
            foreach (var x in ReportHelp.serverReports)
            {
                string taken = "无";
                if (x.TakenSql > 0) { taken = GlobalEvents.GetPlayerFromSqlID(x.TakenSql).characterName.Replace("_", " "); }
                p.SendChatMessage("编号: " + x.ID.ToString());
                p.SendChatMessage("内容: " + x.ReportMessage);
                p.SendChatMessage("举报人: " + taken);
                p.SendChatMessage("<hr>");
            }
        }

        [Command(CONSTANT.COM_AnswerReport)]
        public async Task COM_Admin(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 4) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 0) { MainChat.SendInfoChat(p, CONSTANT.DESC_AswerReport); return; }

            if (!Int32.TryParse(args[0], out int reportId))
                return;

            ReportHelp.ReportModel closeCase = ReportHelp.serverReports.Find(x => x.ID == reportId);
            if (closeCase == null) { MainChat.SendErrorChat(p, "[错误] 无效举报!"); return; }
            ReportHelp.serverReports.Remove(closeCase);
            MainChat.SendInfoChat(p, "> 已接手举报.");
            PlayerModel case3T = GlobalEvents.GetPlayerFromSqlID(closeCase.ID);
            if (case3T != null) { MainChat.SendInfoChat(case3T, "> 您的举报已被管理员接手, 请等待处理, 期间管理员可能会通过 OOC私信 联系您, 请留意."); }
            CharacterSettings set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            AccountModel acc = await Database.DatabaseMain.getAccInfo(p.accountId);
            acc.ReportCount += 1;
            await acc.Update();
            return;
        }

        [Command("dere")]
        public void COM_DeleteReport(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 4) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /dere [id] [原因]"); return; }

            if (!Int32.TryParse(args[0], out int reportId))
                return;

            ReportHelp.ReportModel closeCase = ReportHelp.serverReports.Find(x => x.ID == reportId);
            if (closeCase == null) { MainChat.SendErrorChat(p, "[错误] 无效举报!"); return; }

            ReportHelp.serverReports.Remove(closeCase);
            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(closeCase.ID);
            if (t == null)
            {
                MainChat.SendInfoChat(p, "[?] 指定玩家不在线.");
                return;
            }
            else
            {
                MainChat.SendInfoChat(t, "[!] 您的举报被驳回, 原因: <br>" + string.Join(" ", args[1..]));
                MainChat.SendInfoChat(p, "[!] 已驳回指定玩家的举报, 原因: <br>" + string.Join(" ", args[1..]));
                return;
            }
        }

        [Command(CONSTANT.COM_HelpReqs)]
        public void COM_ShowAllHelpReq(PlayerModel p)
        {
            if (p.adminLevel < 2) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            p.SendChatMessage("{F13A00}<center>[ 求助 ]</center>");
            if (ReportHelp.serverHelpReqs.Count <= 0) { p.SendChatMessage("无效求助."); return; }
            foreach (var x in ReportHelp.serverHelpReqs)
            {
                p.SendChatMessage("编号: " + x.ID.ToString());
                p.SendChatMessage("内容: " + x.HelpMessage);
                p.SendChatMessage("<hr>");
            }
        }

        [Command(CONSTANT.COM_AnswerHelpReq)]
        public async Task COM_AnswerHelpReq(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 2) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 1) { MainChat.SendInfoChat(p, CONSTANT.DESC_AnswerHelpReq); return; }
            if (!Int32.TryParse(args[0], out int helpSelect))
                return;

            var help = ReportHelp.serverHelpReqs.Find(x => x.ID == helpSelect);
            if (help == null) { MainChat.SendErrorChat(p, "> 无效求助."); return; }
            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(help.ID);
            string answer = string.Join(" ", args[1..]);
            if (t == null) { MainChat.SendInfoChat(p, "> 指定玩家不在线."); ReportHelp.serverHelpReqs.Remove(help); return; }
            t.SendChatMessage("<div style='border-radius: 32px; background-color: #7d7d7d5b; padding: 10px 15px 10px 15px;'>" +
                                "<center>{49a0cd}您的求助已被 (" + p.characterName.Replace('_', ' ') + "[" + p.sqlID + "]) 回复</center><br>{49a0cd}内容: {FFFFFF}" + help.HelpMessage + "<br>{49a0cd}回复: {FFFFFF}" + answer +
                                "</div>");
            MainChat.SendInfoChat(p, "已回复玩家求助.");
            AccountModel acc = await Database.DatabaseMain.getAccInfo(p.accountId);
            acc.QuestionCount += 1;
            await acc.Update();
            return;
        }

        [Command(CONSTANT.COM_AddMarket)]
        public void addMarket(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; }
            OtherSystem.LSCsystems.Market.MarketModel M = new OtherSystem.LSCsystems.Market.MarketModel();
            if (args.Length <= 1) { MainChat.SendErrorChat(p, CONSTANT.DESC_AddMarket); return; }
            if (!Int32.TryParse(args[0], out int type))
                return;

            M.type = type;
            M.business = (args[1] == null) ? Int32.Parse(args[1]) : 0;
            M.position = p.Position;
            M.marketLabel = TextLabelStreamer.Create("~g~[~w~商店~g~]~n~~g~/~w~mb", M.position, font: 0, streamRange: 10);
            OtherSystem.LSCsystems.Market.serverMarketList.Add(M);
            p.SendChatMessage("成功创建商店.");
        }

        [Command(CONSTANT.COM_AddMarketItem)]
        public void addMarketItem(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; }
            if (args.Length <= 1) { MainChat.SendInfoChat(p, CONSTANT.DESC_AddMarketItem); return; }
            var Mar = OtherSystem.LSCsystems.Market.serverMarketList.Find(x => p.Position.Distance(x.position) < 5);
            if (Mar == null) { MainChat.SendErrorChat(p, "[错误] 附近没有商店."); return; }
            if (!Int32.TryParse(args[0], out int itemID))
                return;
            var item = Items.LSCitems.Find(x => x.ID == itemID);
            if (item == null) { MainChat.SendErrorChat(p, "[错误] 无效物品."); return; }
            OtherSystem.LSCsystems.Market.MarketModel.MarketItems newItem = new OtherSystem.LSCsystems.Market.MarketModel.MarketItems();
            newItem.itemId = item.ID;
            newItem.price = Int32.Parse(args[1]);
            Mar.items.Add(newItem);
            MainChat.SendInfoChat(p, "> 成功添加商品.");
        }

        [Command(CONSTANT.COM_AddMarketStock)]
        public void addMarketItemStock(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; }
            if (args.Length <= 1) { MainChat.SendInfoChat(p, CONSTANT.DESC_AddMarketStock); return; }
            var Mar = OtherSystem.LSCsystems.Market.serverMarketList.Find(x => p.Position.Distance(x.position) < 5);
            if (Mar == null) { MainChat.SendErrorChat(p, "[错误] 附近没有商店."); return; }
            var newItem = Mar.items.Find(x => x.itemId == Int32.Parse(args[0]));
            if (newItem == null) { MainChat.SendErrorChat(p, "此商店没有商品"); return; }
            newItem.stock = Int32.Parse(args[1]);

            MainChat.SendInfoChat(p, "> 成功设置此商店库存.");
        }

        [Command(CONSTANT.COM_DeleteMarket)]
        public void removeMarket(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 4) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; }
            if (args.Length <= 0) { MainChat.SendErrorChat(p, CONSTANT.DESC_DeleteMarket); return; }
            int marketId = Int32.Parse(args[0]);
            var Mar = OtherSystem.LSCsystems.Market.serverMarketList.Find(x => x.ID == marketId);
            if (Mar == null) { MainChat.SendErrorChat(p, "[错误] 附近没有商店."); return; }
            Mar.marketLabel.Delete();
            OtherSystem.LSCsystems.Market.serverMarketList.Remove(Mar);
        }

        /*       [Command(CONSTANT.COM_GiveWeapon)]  #SilahVer
            public void addPlayerWeapon(PlayerModel p, params string[] args)
            {
                if (p.adminLevel < CONSTANT.LVL_GiveWeapon) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; }
                if (args.Length <= 0) { MainChat.SendErrorChat(p, CONSTANT.DESC_GiveWeapon); return; }
                WeaponSystem.WeaponModel newWep = new WeaponSystem.WeaponModel();
                uint wep; bool wepAdded = UInt32.TryParse(args[0], out wep);
                int bullet; bool bulletAdded = Int32.TryParse(args[1], out bullet);
                if(!wepAdded && !bulletAdded) { MainChat.SendErrorChat(p, CONSTANT.DESC_GiveWeapon); return; }
                newWep.weapon = wep;
                newWep.bullet = bullet;
                newWep.serialNo = "RU";
                newWep.weaponDurability = 100;
                ServerItems addWeapon = Items.LSCitems.Find(x => x.ID == 29);
                addWeapon.data2 = JsonConvert.SerializeObject(newWep);
                bool succes = Inventory.AddInventoryItem(p, addWeapon, 1);
                if (succes) { MainChat.SendErrorChat(p, "Silah başarıyla eklendi."); return; }
                // TODO düzenlenecek
            }*/

        [Command(CONSTANT.COM_CheckCharacter)]
        public async Task COM_ShowCharacterStats(PlayerModel p, params string[] args)
        {
            try
            {

                if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
                if (args.Length <= 0) { MainChat.SendInfoChat(p, CONSTANT.DESC_CheckCharacter); return; }
                if (args[0] == "0")
                    return;

                if (!Int32.TryParse(args[0], out int tSQL)) { MainChat.SendInfoChat(p, CONSTANT.DESC_CheckCharacter); return; }

                PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSQL);
                if (t == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }

                AccountModel acc = await Database.DatabaseMain.getAccInfo(t.accountId);
                string sex = "";
                FactionModel fact = new FactionModel();
                if (t.factionId > 0)
                {
                    fact = await Database.DatabaseMain.GetFactionInfo(t.factionId);
                }
                string factName = "";
                string factRank = "";
                if (fact != null)
                {
                    factName = fact.name;
                    factRank = Factions.Faction.GetFactionRankString(t.factionRank, fact);
                }
                else { factName = "无"; factRank = "无"; }
                if (t.sex == 0) { sex = "女性"; } else { sex = "男性"; }
                string bizinfo = "";
                if (t.businessStaff != 0)
                {
                    BusinessModel biz = await Database.DatabaseMain.GetBusinessInfo(t.businessStaff);
                    string bizOwner = "雇员";
                    if (biz.ownerId == t.sqlID) { bizOwner = "业主"; }
                    bizinfo = "产业: " + biz.name + " | 身份: " + bizOwner;
                }
                string healtStatus = "健康";
                if (t.injured.Injured) { healtStatus = "负伤"; }
                if (t.injured.isDead) { healtStatus = "重伤/死亡"; }

                CharacterSettings settings = JsonConvert.DeserializeObject<CharacterSettings>(t.settings);
                string fromCountry = GlobalEvents.getCountryName(settings.nation);
                //string secondLang = GlobalEvents.GetSecondLang(settings.secondLang);

                p.SendChatMessage("{7EB278}[" + t.characterName.Replace("_", " ") + " (" + acc.forumName + ") |" + DateTime.Now.ToString("yyyy/MM/dd HH:mm") + "]");
                p.SendChatMessage("{C8C8C8}>> 数据库ID: " + t.sqlID + " | 性别: " + sex + " | 年龄: " + t.characterAge + " | 国籍: " + fromCountry);
                p.SendChatMessage("{C8C8C8}>> 经验值: " + t.characterExp + "/" + t.characterLevel * 4 + " | 角色等级: " + t.characterLevel + " | 发薪日剩余时间: " + (60 - t.gameTime));
                p.SendChatMessage("{C8C8C8}>> 状态: " + healtStatus + " | 血量: " + t.hp + "/" + t.maxHp + " | 护甲: " + t.Armor + " | 现金: " + t.cash.ToString());
                p.SendChatMessage("{C8C8C8}>> 组织: " + factName + " | 阶级: " + factRank);
                p.SendChatMessage("{C8C8C8}>> 电话号码: " + t.phoneNumber + " | 虚拟世界: " + t.Dimension);
                p.SendChatMessage("{C8C8C8}>> 身体: " + t.Strength);
                if (t.HasData("Player:Accent"))
                    p.SendChatMessage("{C8C8C8}>> 口音: " + t.lscGetdata<string>("Player:Accent"));
                if (bizinfo != "") { p.SendChatMessage("{C8C8C8}>> " + bizinfo); }
                //p.SendChatMessage("{C8C8C8}>> Araçlar: 3(/aracliste) | İşletmeler: 1(/isletmeler) | Evler: 2(/evler)");
                p.SendChatMessage("{7EB278}[" + t.characterName.Replace("_", " ") + " (" + acc.forumName + ") |" + DateTime.Now.ToString("yyyy/MM/dddd HH:mm") + "]");

            }
            catch
            {
                return;
            }
        }

        [Command(CONSTANT.COM_CreateDoor)]
        public void COM_CreateDoor(PlayerModel p, int radius = 2)
        {
            if (p.adminLevel < 6) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            p.EmitLocked("Doors:Create", float.Parse(radius.ToString()));
        }

        [Command(CONSTANT.COM_ShowNearDoors)]
        public void COM_GetNearDoors(PlayerModel p)
        {
            if (p.adminLevel < 4) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            //OtherSystem.LSCsystems.DoorSystem.serverDoors;
            string text = "<center>附近的门<center>";
            p.SendChatMessage(text);
            foreach (var x in OtherSystem.LSCsystems.DoorSystem.serverDoors)
            {
                if (p.Position.Distance(x.pos) < 10)
                {
                    text = "<br> ID: " + x.textlblID + " | 类型: " + x.type + " | 管理人: " + x.owner;
                    p.SendChatMessage(text);
                }
            }

        }

        [Command(CONSTANT.COM_EditDoor)]
        public void COM_EditDoor(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; }
            if (args.Length <= 1) { MainChat.SendInfoChat(p, CONSTANT.DESC_EditDoor); return; }
            if (!ulong.TryParse(args[0], out ulong doorID))
                return;

            var door = OtherSystem.LSCsystems.DoorSystem.serverDoors.Find(x => x.textlblID == doorID);
            if (door == null) { MainChat.SendErrorChat(p, "[错误] 无效门."); return; }
            int value; bool isOk = Int32.TryParse(args[2], out value);

            switch (args[1])
            {
                case "delete":
                    TextLabelStreamer.DestroyDynamicTextLabel(door.textlblID);
                    DoorSystem.serverDoors.Remove(door);
                    MainChat.SendInfoChat(p, "> 成功删除门.");
                    return;
                case "type":
                    if (!isOk) { MainChat.SendInfoChat(p, "无效参数"); return; }
                    door.type = value;
                    MainChat.SendInfoChat(p, "> 成功设置门的类型为: " + value.ToString());
                    return;
                case "sahip":
                    if (!isOk) { MainChat.SendInfoChat(p, "无效参数"); return; }
                    door.owner = value;
                    MainChat.SendInfoChat(p, "> 成功设置门的主人为: " + value.ToString());
                    return;
            }
        }

        /*
        [Command(CONSTANT.COM_AddBlip)]   
        public void COM_CreateBlip(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < CONSTANT.LVL_AddBlip) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 1) { MainChat.SendInfoChat(p, CONSTANT.DESC_AddBlip); return; }
            int sprite; bool spriteOk = Int32.TryParse(args[0], out sprite);
            int number; bool numberOk = Int32.TryParse(args[1], out number);
            string labeltext = string.Join(" ", args[2..]);

            if(spriteOk && numberOk)
            {
                OtherSystem.LSCsystems.BlipSystem.COM_CreateBlip(p, sprite, number, labeltext);
            }
            else
            {
                MainChat.SendInfoChat(p, CONSTANT.DESC_AddBlip); return;
            }
            
        }

        [Command(CONSTANT.COM_ShowNearBlips)]
        public void COM_ShowNearBlips(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < CONSTANT.LVL_ShowNearBlips) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            BlipSystem.COM_ShowNearBlips(p);
        }

        [Command(CONSTANT.COM_DeleteBlip)] 
        public void COM_RemoveBlip(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < CONSTANT.LVL_DeleteBlip) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            BlipSystem.COM_RemoveBlip(p, args);
        } */

        [Command(CONSTANT.COM_AdminRepairVehicle)]
        public async void COM_ForceRepairCar(PlayerModel p)
        {
            if (p.adminLevel < 4) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v == null) { return; }
            //v.Dimension = p.sqlID;
            v.EngineHealth = 1000; v.PetrolTankHealth = 1000; v.BodyHealth = 1000; v.BodyAdditionalHealth = 1000;
            if (v.NetworkOwner != null)
            {
                v.NetworkOwner.EmitLocked("Vehicle:Repair", v.Id);
            }
            await Task.Delay(1000);

            if (!v.Exists)
                return;
            if (!p.Exists)
                return;

            //v.Dimension = p.Dimension;
            v.Repair();
            MainChat.SendInfoChat(p, "已修复车辆.");
            return;
        }

        [Command(CONSTANT.COM_FlipVehicle)]
        public async void COM_ForceFlipCar(PlayerModel p)
        {
            if (p.adminLevel < 3) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v == null) { return; }
            await v.SetRotationAsync(new Rotation(0, 0, 0));
            await Task.Delay(500);

            if (!v.Exists)
                return;
            if (!p.Exists)
                return;

            //v.Dimension = p.Dimension;
            MainChat.SendInfoChat(p, "已翻身车辆.");
            return;
        }

        [Command(CONSTANT.COM_SetHp)]
        public void COM_SetHealth(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 0) { MainChat.SendInfoChat(p, CONSTANT.DESC_SetHp); return; }
            int id; int hp; bool idOk = Int32.TryParse(args[0], out id); bool hpOk = Int32.TryParse(args[1], out hp);
            if (!idOk || !hpOk) { MainChat.SendInfoChat(p, CONSTANT.DESC_SetHp); return; }
            if (hp > 10000) { MainChat.SendErrorChat(p, "[错误] 最大血量可设置为 10000."); return; }
            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(id);
            if (t == null)
                return;

            if (t == p) { MainChat.SendInfoChat(p, "成功设置自己的血量为: [" + hp.ToString()); }
            else
            {
                MainChat.SendInfoChat(t, p.fakeName.Replace("_", " ") + " 将您的角色血量设置为: [" + (hp).ToString(), true);
                MainChat.SendInfoChat(p, t.characterName.Replace("_", " ") + " 的血量被您设置为: [" + (hp).ToString());
            }
            //t.maxHp = 1000;

            t.hp = hp;
            return;
        }

        [Command(CONSTANT.COM_SetArmor)]
        public void COM_SetArmor(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 0) { MainChat.SendInfoChat(p, CONSTANT.DESC_SetArmor); return; }
            int id; int hp; bool idOk = Int32.TryParse(args[0], out id); bool hpOk = Int32.TryParse(args[1], out hp);
            if (!idOk || !hpOk) { MainChat.SendInfoChat(p, CONSTANT.DESC_SetArmor); return; }
            if (hp > 1000) { MainChat.SendErrorChat(p, "[错误] 最大血量可设置为 1000."); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(id);
            if (t == null)
                return;
            hp -= 50;
            t.MaxArmor = 1000;
            t.Armor = (ushort)hp;


            if (t == p) { MainChat.SendInfoChat(p, "成功设置自己的护甲为: [" + hp.ToString());
            }
            else
            {
                MainChat.SendInfoChat(t, p.fakeName.Replace("_", " ") + " 将您的角色护甲设置为: [" + hp.ToString(), true);
                MainChat.SendInfoChat(p, t.characterName.Replace("_", " ") + " 的护甲被您设置为: [" + hp.ToString());
                return;
            }
        }

        [Command(CONSTANT.COM_CloseAdminChat)]
        public void COM_CloseAdminChat(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (p.HasData(EntityData.PlayerEntityData.AdminChatClose))
            {
                p.DeleteData(EntityData.PlayerEntityData.AdminChatClose);
                MainChat.SendInfoChat(p, "管理员频道已开启.");
                return;
            }
            else
            {
                p.SetData(EntityData.PlayerEntityData.AdminChatClose, 1);
                MainChat.SendInfoChat(p, "管理员频道已关闭.");
                return;
            }

        }

        [Command(CONSTANT.COM_AdminChat)]
        public async Task COM_AdminChat(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 0) { MainChat.SendInfoChat(p, CONSTANT.DESC_AdminChat); return; }
            AccountModel acc = await Database.DatabaseMain.getAccInfo(p.accountId);
            MainChat.SendAdminChat(acc.forumName + " | " + p.characterName.Replace("_", " ") + ": " + string.Join(" ", args));
            return;
        }

        [Command(CONSTANT.COM_ClosaHelperChat)]
        public void COM_CloseTesterChat(PlayerModel p)
        {
            if (p.adminLevel < 3) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (p.HasData(EntityData.PlayerEntityData.TesterChatClose))
            {
                p.DeleteData(EntityData.PlayerEntityData.TesterChatClose);
                MainChat.SendInfoChat(p, "志愿者频道已开启.");
                return;
            }
            else
            {
                p.SetData(EntityData.PlayerEntityData.TesterChatClose, 1);
                MainChat.SendInfoChat(p, "志愿者频道已关闭.");
                return;
            }

        }

        [Command("hcc")]
        public void COM_CloseHelperChat(PlayerModel p)
        {
            if (p.adminLevel < 2) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (p.HasData("HelperChatClosed"))
            {
                p.DeleteData("HelperChatClosed");
                MainChat.SendInfoChat(p, "志愿者频道已开启.");
                return;
            }
            else
            {
                p.SetData("HelperChatClosed", 1);
                MainChat.SendInfoChat(p, "志愿者频道已关闭.");
                return;
            }

        }

        [Command("hc")]
        public async Task COM_TesterChat(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 3) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 0) { MainChat.SendInfoChat(p, CONSTANT.DESC_HelperChat); return; }
            AccountModel acc = await Database.DatabaseMain.getAccInfo(p.accountId);
            MainChat.SendTesterChat("[工作频道] " + acc.forumName + " | " + p.characterName.Replace("_", " ") + ": " + string.Join(" ", args));
            return;
        }

        [Command("tc")]
        public async Task COM_SupportChat(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 2) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 0) { MainChat.SendInfoChat(p, CONSTANT.DESC_HelperChat); return; }
            AccountModel acc = await Database.DatabaseMain.getAccInfo(p.accountId);
            MainChat.SendSupportchat("[技术频道] " + acc.forumName + " | " + p.characterName.Replace("_", " ") + ": " + string.Join(" ", args));
            return;
        }

        [Command(CONSTANT.COM_AdminSlap)]
        public void COM_AdminSlap(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 0) { MainChat.SendInfoChat(p, CONSTANT.DESC_AdminSlap); return; }

            int tId; bool tidOk = Int32.TryParse(args[0], out tId);
            if (!tidOk) { MainChat.SendInfoChat(p, CONSTANT.DESC_AdminSlap); return; }
            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tId);
            if (t == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }
            Position slapPos = t.Position;
            slapPos.Z += 5;
            t.Position = slapPos;
            MainChat.SendInfoChat(p, t.characterName.Replace("_", " ") + " 被您拍了一下.");
            return;
        }

        [Command(CONSTANT.COM_Freeze)]
        public void COM_AdminFreeze(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 1) { MainChat.SendInfoChat(p, CONSTANT.DESC_Freeze); return; }

            int tid; bool tOk = Int32.TryParse(args[0], out tid);
            if (!tOk) { MainChat.SendInfoChat(p, CONSTANT.DESC_Freeze); return; }
            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tid);
            if (t == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }

            if (t.Vehicle != null) { GlobalEvents.ForceLeaveVehicle(p); }
            switch (args[1])
            {
                case "on":
                    GlobalEvents.FreezeEntity(t, true);
                    MainChat.SendInfoChat(t, p.characterName.Replace("_", " ") + " 把您角色冻结了(请不要着急认为是惩罚, 可能是为了防止BUG等等/或者更好的处理工作).");
                    MainChat.SendInfoChat(p, t.characterName.Replace("_", " ") + " 被您冻结了.");
                    return;

                case "off":
                    GlobalEvents.FreezeEntity(t, false);
                    MainChat.SendInfoChat(t, p.characterName.Replace("_", " ") + " 把您角色解开了冻结.");
                    MainChat.SendInfoChat(p, t.characterName.Replace("_", " ") + " 被您解开了冻结.");
                    return;
            }
        }

        [Command(CONSTANT.COM_AdminRemoveRoadBlock)]
        public void COM_ARemoveBarricade(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 4) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 0) { MainChat.SendInfoChat(p, CONSTANT.DESC_AdminRemoveRoadBlock); return; }

            int id; bool isOk = Int32.TryParse(args[0], out id);
            if (!isOk) { MainChat.SendInfoChat(p, CONSTANT.DESC_AdminRemoveRoadBlock); return; }
            System.RoadBlockModel rb = System.PD.pdRoadBlocks.Find(x => x.prop.Id == (ulong)id);
            if (rb == null) { MainChat.SendErrorChat(p, "[错误] 无效路障."); return; }
            rb.textlbl.Delete();
            rb.prop.Delete();
            System.PD.pdRoadBlocks.Remove(rb);
            MainChat.SendInfoChat(p, "成功清理指定路障.");
            return;
        }

        [Command(CONSTANT.COM_AdminRevive)]
        public async void COM_Revive(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 0) { MainChat.SendInfoChat(p, CONSTANT.DESC_AdminRevive); return; }

            int id; bool isOk = Int32.TryParse(args[0], out id);
            if (!isOk) { MainChat.SendInfoChat(p, CONSTANT.DESC_AdminRevive); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(id);
            if (t == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }
            t.Spawn(t.Position, 0);
            if (t == p)
            {
                MainChat.SendInfoChat(p, "成功治愈自己.");
            }
            else
            {
                MainChat.SendErrorChat(p, t.characterName.Replace("_", " ") + " 被您OOC治愈了.");
                MainChat.SendInfoChat(t, p.characterName.Replace("_", " ") + " 把您 OOC治愈 了, 原因: 1.遇到BUG死亡 2. 为了配合扮演.");
            }
            t.injured.Injured = false;
            t.injured.isDead = false;
            t.injured.arms = false;
            t.injured.legs = false;
            t.injured.torso = false;
            t.injured.head = false;
            await Task.Delay(300);

            if (!t.Exists)
                return;

            t.EmitLocked("Injured:ClearBloods");
            t.hp = 999;
            await t.SetMaxHealthAsync(999);
            await t.SetHealthAsync(999);
            t.EmitLocked("Damage:ClearStungun");

            GlobalEvents.UpdateInjured(t);
            return;
        }

        [Command(CONSTANT.COM_SetDimension)]
        public void COM_SetDimension(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 3) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 1) { MainChat.SendInfoChat(p, CONSTANT.DESC_SetDimension); return; }

            int id; int dim; bool isId = Int32.TryParse(args[0], out id); bool isDim = Int32.TryParse(args[1], out dim);
            if (!isId || !isDim) { MainChat.SendInfoChat(p, CONSTANT.DESC_SetDimension); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(id);
            if (t == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }

            t.Dimension = dim;

            MainChat.SendInfoChat(p, t.characterName.Replace("_", " ") + " 的角色被您设置到虚拟世界: " + dim.ToString());
            MainChat.SendInfoChat(t, p.characterName.Replace("_", " ") + " 将您的角色设置到虚拟世界: " + dim.ToString());
            return;
        }

        [Command(CONSTANT.COM_AdminRemoveRadio)]
        public void COM_ForceRemoveRadio(PlayerModel p)
        {
            if (p.adminLevel < 2) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            Boombox.BoomBoxModel x = Boombox.serverBoomBoxs.Find(x => p.Position.Distance(x.boxProp.Position) < 20);
            if (x == null) { MainChat.SendErrorChat(p, "[错误] 无效音响."); return; }
            x.boxProp.Delete();
            x.boxLabel.Delete();
            foreach (PlayerModel kalT in Alt.GetAllPlayers())
            {
                kalT.EmitLocked("Boom:RemoveSound", x.ID);
            }
            Boombox.serverBoomBoxs.Remove(x);
            MainChat.SendInfoChat(p, "成功清理指定音响.");
            return;
        }

        [Command(CONSTANT.COM_CheckPlayerFaction)]
        public async Task COM_ShowFactionInfo(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 0) { MainChat.SendInfoChat(p, CONSTANT.DESC_CheckPlayerFaction); return; }
            int tid; bool tidOk = Int32.TryParse(args[0], out tid);
            if (!tidOk) { MainChat.SendInfoChat(p, CONSTANT.DESC_CheckPlayerFaction); return; }
            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tid);
            if (t == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }
            if (t.factionId <= 0) { MainChat.SendErrorChat(p, "[错误] 指定玩家没有组织."); return; }
            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(t.factionId);
            if (fact == null) { MainChat.SendErrorChat(p, "[错误] 无效组织."); return; }

            PlayerModelInfo owner = await Database.DatabaseMain.getCharacterInfo(fact.owner);
            string ownerName = "无";
            if (owner != null) { ownerName = owner.characterName.Replace("_", " "); }
            int totalPlayer = 0;
            foreach (PlayerModel ft in Alt.GetAllPlayers())
            {
                if (ft.factionId == t.factionId) totalPlayer++;
            }
            List<FactionUserModel> factUsers = await Database.DatabaseMain.GetFactionMembers(t.factionId);
            p.SendChatMessage("<br><center>" + fact.name + "</center><br>{A0A506}ID: {FFFFFF}" + fact.ID.ToString() + "<br>{A0A506}类型: {FFFFFF}" + fact.type + "<br>{A0A506}领导人: {FFFFFF}" + ownerName + "<br>{A0A506}金库现金: {FFFFFF}" + fact.cash + "<br>{A0A506}组织等级: {FFFFFF}" + fact.factionLevel + "<br>{A0A506}批准: {FFFFFF}" + fact.isApproved.ToString() + "<br>{A0A506}组织人数: {FFFFFF}" + totalPlayer + "/" + factUsers.Count);
            return;
        }

        [Command(CONSTANT.COM_EditPlayerFaction)]
        public async Task COM_EditFaction(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 6) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            // TODO args.lentgh
            if (args.Length <= 2) { MainChat.SendInfoChat(p, CONSTANT.DESC_EditPlayerFaction); return; }
            int fid; bool fidOk = Int32.TryParse(args[0], out fid);
            if (!fidOk) { MainChat.SendInfoChat(p, CONSTANT.DESC_EditPlayerFaction); return; }

            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(fid);
            if (fact == null) { MainChat.SendErrorChat(p, "[错误] 无效组织."); return; }

            switch (args[1])
            {
                case "type":
                    int newType; bool newTypeOk = Int32.TryParse(args[2], out newType);
                    if (!newTypeOk) { MainChat.SendInfoChat(p, "[用法] /editfaction [id] type [类型ID]"); return; }
                    fact.type = newType;
                    fact.Update();
                    break;

                case "cash":
                    int newCash; bool newCashOk = Int32.TryParse(args[2], out newCash);
                    if (!newCashOk) { MainChat.SendInfoChat(p, "[用法] /editfaction [id] cash [数值]"); return; }
                    fact.cash = newCash;
                    fact.Update();
                    break;

                case "level":
                    int newLevel; bool newLevelOk = Int32.TryParse(args[2], out newLevel);
                    if (!newLevelOk) { MainChat.SendInfoChat(p, "[用法] /editfaction [id] level [数值]"); return; }
                    fact.factionLevel = newLevel;
                    fact.Update();
                    break;

                case "leader":
                    int newOwner; bool newOwnerOk = Int32.TryParse(args[2], out newOwner);
                    if (!newOwnerOk) { MainChat.SendInfoChat(p, "[用法] /editfaction [id] leader [领导人ID]"); return; }
                    fact.owner = newOwner;
                    fact.Update();
                    break;

                case "name":
                    fact.name = string.Join(" ", args[2..]);
                    fact.Update();
                    break;

                case "side":
                    if (!Int32.TryParse(args[2], out int newSide)) { MainChat.SendInfoChat(p, "[用法] /editfaction [id] side [id]"); return; }
                    fact.side = newSide;
                    fact.Update();
                    break;

                case "radio":
                    fact.settings.hasRadio = !fact.settings.hasRadio;
                    MainChat.SendInfoChat(p, "[?] 成功设置组织 " + fact.name + " 的对讲机状态为: " + (fact.settings.hasRadio ? "活跃" : "禁止"));
                    fact.Update();
                    break;

                default:
                    MainChat.SendInfoChat(p, "[用法] /editfaction [组织ID] [选项] [数值]<br>可使用选项:<br>type - leader - cash - level - name - side - radio"); return;
            }
            MainChat.SendInfoChat(p, "指定组织已更新.");
            return;
        }

        [Command(CONSTANT.COM_AdminGiveLicense)]
        public void COM_GiveLicense(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 2) { MainChat.SendInfoChat(p, CONSTANT.DESC_AdminGiveLicense); return; }

            int tSql; bool isOk = Int32.TryParse(args[0], out tSql);
            if (!isOk) { MainChat.SendInfoChat(p, CONSTANT.DESC_AdminGiveLicense); return; }
            int licenseType; bool licenseOk = Int32.TryParse(args[1], out licenseType);
            if (!licenseOk || licenseType >= 7) { MainChat.SendInfoChat(p, CONSTANT.DESC_AdminGiveLicense); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null) { MainChat.SendErrorChat(p, CONSTANT.CharacterNotFoundError); return; }

            string licenseInfo = string.Join(" ", args[3..]);

            OtherSystem.LSCsystems.OtherLicense.GiveLicense(t, p, licenseType, args[2], licenseInfo);
        }

        [Command(CONSTANT.COM_AdminCheckLicense)]
        public void COM_GetPlayerLicenses(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 3) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 0) { MainChat.SendInfoChat(p, CONSTANT.DESC_AdminCheckLicense); return; }

            int tSql; bool isOk = Int32.TryParse(args[0], out tSql);
            if (!isOk) { MainChat.SendInfoChat(p, CONSTANT.DESC_AdminCheckLicense); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }

            CharacterSettings st = JsonConvert.DeserializeObject<CharacterSettings>(t.settings);
            string LicenseText = "<center>" + t.characterName.Replace("_", " ") + " 的执照列表:</center>";
            bool hasLicense = false;
            foreach (Models.OtherLicense l in st.licenses)
            {
                hasLicense = true;
                LicenseText += "<br>" + l.licenseID + " | " + l.licenseString;
            }
            if (!hasLicense) { LicenseText += "<br> 无"; }
            MainChat.SendInfoChat(p, LicenseText, true);
            return;
        }

        [Command(CONSTANT.COM_AdminDeleteLicense)]
        public void COM_DeletePlayerLicense(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 1) { MainChat.SendInfoChat(p, CONSTANT.DESC_AdminDeleteLicense); return; }
            int tSql; bool isOk = Int32.TryParse(args[0], out tSql);

            if (!isOk) { MainChat.SendInfoChat(p, CONSTANT.DESC_AdminDeleteLicense); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }

            CharacterSettings st = JsonConvert.DeserializeObject<CharacterSettings>(t.settings);
            Models.OtherLicense lic = st.licenses.Find(x => x.licenseID == args[1]);
            if (lic == null) { MainChat.SendErrorChat(p, "[错误] 无效执照."); return; }

            st.licenses.Remove(lic);
            t.settings = JsonConvert.SerializeObject(st);
            MainChat.SendInfoChat(p, t.characterName.Replace("_", " ") + " 被您删除了执照: " + args[1]);
            MainChat.SendInfoChat(t, p.characterName.Replace("_", " ") + " 删除了您的执照: " + args[1]);
            return;
        }

        [Command(CONSTANT.COM_Ajail)]
        public async Task COM_AdminJail(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 2) { MainChat.SendInfoChat(p, CONSTANT.DESC_Ajail); return; }
            int tSql; bool sqlOk = Int32.TryParse(args[0], out tSql);
            int time; bool timeOk = Int32.TryParse(args[1], out time);

            AccountModel adminAcc = await Database.DatabaseMain.getAccInfo(p.accountId);
            string reason = string.Join(" ", args[2..]);

            if (!sqlOk || !timeOk) { MainChat.SendInfoChat(p, CONSTANT.DESC_Ajail); return; }



            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null)
            {
                PlayerModelInfo TI = await Database.DatabaseMain.getCharacterInfo(tSql);
                if (TI == null) { MainChat.SendInfoChat(p, "[错误] 无效角色信息."); return; }

                if (p.adminLevel < TI.adminLevel) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }

                TI.adminJail = time;
                foreach (PlayerModel ct in Alt.GetAllPlayers())
                {
                    ct.SendChatMessage("{FF0000} [OOC监禁] {FFFFFF}" + TI.characterName.Replace("_", " ") + " 被 {FFFFFF}" + adminAcc.forumName + " {FF0000}OOC监禁{FFFFFF}, 时间: " + time.ToString() + " 分钟<br>{FF0000}原因: {FFFFFF}" + reason);
                }
                TI.updateSql();
                await Database.DatabaseMain.AddAccountLog(1, TI.sqlID, TI.accountId, reason, adminAcc.kookId);
                return;
            }
            if (p.adminLevel < t.adminLevel) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            t.adminJail = time;
            t.Position = new Position(-1421.015f, -3012.587f, -80.000f);
            t.Dimension = t.sqlID;
            foreach (PlayerModel cta in Alt.GetAllPlayers())
            {
                cta.SendChatMessage("{FF0000} [OOC监禁] {FFFFFF}" + t.characterName.Replace("_", " ") + " 被 {FFFFFF}" + adminAcc.forumName + " {FF0000}OOC监禁 {FFFFFF}, 时间: " + time.ToString() + " 分钟<br>{FF0000}原因: {FFFFFF}" + reason);
            }
            await t.updateSql();
            await Database.DatabaseMain.AddAccountLog(1, t.sqlID, t.accountId, reason, adminAcc.kookId);
            //GlobalEvents.GameControls(t, false);
            Core.Logger.WriteLogData(Core.Logger.logTypes.AdminLog, "[OOC监禁] " + t.characterName.Replace("_", " ") + " | " + adminAcc.forumName + " | 时间: " + time.ToString() + " | 原因: {FFFFFF}" + reason);
            return;
        }

        [Command(CONSTANT.COM_Ban)]
        public async Task COM_NormalBan(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 6) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 1) { MainChat.SendInfoChat(p, CONSTANT.DESC_Ban); return; }
            int tSql; bool sqlOk = Int32.TryParse(args[0], out tSql);

            if (!sqlOk) { MainChat.SendInfoChat(p, CONSTANT.DESC_Ban); return; } // düzenle.
            string reason = string.Join(" ", args[1..]);
            AccountModel banAccount = new AccountModel();

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            string name = "";
            if (t == null)
            {
                PlayerModelInfo toff = await Database.DatabaseMain.getCharacterInfo(tSql);
                if (toff == null) { MainChat.SendInfoChat(p, "无效角色信息."); return; }
                if (p.adminLevel < toff.adminLevel) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }

                banAccount = await Database.DatabaseMain.getAccInfo(toff.accountId);
                name = toff.characterName.Replace("_", " ");
            }
            else
            {
                if (p.adminLevel < t.adminLevel) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
                banAccount = await Database.DatabaseMain.getAccInfo(t.accountId);
                t.Kick("您已被服务器封禁, 原因: " + reason);
                name = t.characterName.Replace("_", " ");
            }
            banAccount.banned = true;
            await banAccount.Update();


            AccountModel adminAcc = await Database.DatabaseMain.getAccInfo(p.accountId);
            foreach (PlayerModel ct in Alt.GetAllPlayers())
            {
                ct.SendChatMessage("{FF0000}[!] {FFFFFF}" + name + "(" + banAccount.forumName + ")" + " 被 {FFFFFF}" + adminAcc.forumName + " {FF0000}封禁账号.<br>原因: {FFFFFF}" + reason);
            }
            Core.Logger.WriteLogData(Core.Logger.logTypes.AdminLog, "[BAN] " + name + "(" + banAccount.forumName + ") | 执行人: " + adminAcc.forumName + " 原因: " + reason);
            return;
        }

        [Command(CONSTANT.COM_SBan)]
        public async Task COM_SBan(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 7) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 1) { MainChat.SendInfoChat(p, CONSTANT.DESC_SBan); return; }
            int tSql; bool sqlOk = Int32.TryParse(args[0], out tSql);

            if (!sqlOk) { MainChat.SendInfoChat(p, CONSTANT.DESC_SBan); return; } // düzenle.

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }
            if (p.adminLevel < t.adminLevel) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }

            string reason = string.Join(" ", args[1..]);

            AccountModel adminAccount = await Database.DatabaseMain.getAccInfo(p.accountId);
            await Database.DatabaseMain.AddSocialBan(t.SocialClubId, adminAccount.forumName, reason);

            AccountModel banAccount = await Database.DatabaseMain.getAccInfo(t.accountId);
            foreach (PlayerModel ct in Alt.GetAllPlayers())
            {
                ct.SendChatMessage("{FF0000}[!] {FFFFFF}" + t.characterName.Replace("_", " ") + "(" + banAccount.forumName + ")" + " 被 {FFFFFF}" + adminAccount.forumName + " {FF0000}封禁R星账号和角色.<br>原因: {FFFFFF}" + reason);
            }

            t.Kick("您的R星账号和角色已经被服务器封禁, 原因: " + reason);
            Core.Logger.WriteLogData(Core.Logger.logTypes.AdminLog, "[封禁] " + t.characterName.Replace("_", " ") + "(" + banAccount.forumName + ") | 执行人: " + adminAccount.forumName + " | 原因: " + reason);
            Core.Logger.WriteLogData(Core.Logger.logTypes.AdminLog, "[R星封禁] " + t.SocialClubId.ToString() + " | 执行人: " + adminAccount.forumName + " | 原因: " + reason);
            return;
        }


        public static List<Position> paperboyPos = new List<Position>()
            {
                new Position(498.7837f, -1700.0837f, 28.4560f),
                new Position(490.3367f, -1714.9023f, 28.5356f),
                new Position(480.3415f, -1740.1134f, 27.8342f),
                new Position(475.6076f, -1757.3097f, 27.9488f),
                new Position(473.4494f, -1774.9735f, 27.7392f)
            };

        [Command("testpaperboy")]
        public void COM_TestPaperboy(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 8) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle
            p.EmitLocked("testEvent:setPaperBoyPosition", paperboyPos[0]);
        }

        [Command("apet")]
        public void COM_CreatePed(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 8) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /apet [PED模型]"); return; }
            OtherSystem.PedModel ped = OtherSystem.PedStreamer.Create(args[0], p.Position);
            //ped.netOwner = p;
            ped.hasNetOwner = true;
            ped.nametag = "~b~[~w~K9~b~]~n~~w~健康: ~g~%100 ~b~";
            p.SendChatMessage(ped.Id.ToString());

            return;
        }

        [Command("apettakip")]
        public void COM_FollowPed(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 8) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            int petID; bool isOK = Int32.TryParse(args[0], out petID);

            if (!isOK)
                return;

            OtherSystem.PedModel curr = PedStreamer.Get((ulong)petID);
            curr.followTarget = p.Id;
            return;
        }

        [Command("apetkaldir")]
        public void COM_DeletePed(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 8) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            int petID; bool isOK = Int32.TryParse(args[0], out petID);

            if (!isOK)
                return;

            OtherSystem.PedModel curr = PedStreamer.Get((ulong)petID);

            curr.Destroy();
            return;
        }

        [Command("acam")]
        public void COM_AdminSpec(PlayerModel p)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (p.HasSyncedMetaData("FREECAM"))
            {
                bool state = false;
                p.GetSyncedMetaData("FREECAM", out state);
                state = !state;
                if (state)
                {
                    p.showSqlId = false;
                    p.fakeName = "";
                    p.Streamed = false;
                }
                else
                {
                    p.Streamed = true;
                    p.showSqlId = true;
                    p.fakeName = p.characterName;

                }
                p.SetSyncedMetaData("FREECAM", state);
                return;
            }

            p.SetSyncedMetaData("FREECAM", true);
            p.showSqlId = false;
            p.fakeName = "";
            p.Streamed = false;
            return;
        }

        [Command("spec")]
        public void COM_AdminSpecPlayer(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /spec [id]"); return; }
            if (!Int32.TryParse(args[0], out int tSQL)) { MainChat.SendInfoChat(p, "[用法] /spec [id]"); return; }

            var t = GlobalEvents.GetPlayerFromSqlID(tSQL);
            if (t == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家."); return; }

            if (t.HasSyncedMetaData("FREECAM"))
            {
                bool state = false;
                t.GetSyncedMetaData("FREECAM", out state);
                state = !state;
                if (state)
                {
                    t.showSqlId = false;
                    t.fakeName = "";
                    t.Streamed = false;
                    MainChat.SendInfoChat(p, "[?] 开始监视.");
                }
                else
                {
                    t.Streamed = true;
                    t.showSqlId = true;
                    t.fakeName = t.characterName;
                    MainChat.SendInfoChat(p, "[?] 关闭监视.");

                }
                t.SetSyncedMetaData("FREECAM", state);
                return;
            }

            t.SetSyncedMetaData("FREECAM", true);
            t.showSqlId = false;
            t.fakeName = "";
            t.Streamed = false;


            return;

        }

        [Command("tpw")]
        public void COM_GoToWayPoint(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            p.EmitLocked("teleportToWaypoint");
        }

        [Command("amic")]
        public async Task COM_GiveMicrophone(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /amic [id] [on/off]"); return; }
            int tSql; bool isOK = Int32.TryParse(args[0], out tSql);

            if (!isOK) { MainChat.SendInfoChat(p, "[用法] /amic [id] [on/off]"); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }

            CharacterSettings set = JsonConvert.DeserializeObject<CharacterSettings>(t.settings);

            switch (args[1])
            {
                case "on":
                    set.hasMicrophone = true;
                    MainChat.SendInfoChat(p, t.characterName.Replace("_", " ") + " 被您开启了麦克风权限.");
                    MainChat.SendInfoChat(t, p.characterName.Replace("_", " ") + " 开启了您的麦克风权限 (仅用于 IC 麦克风讲话的情况, 请合理使用).");
                    break;

                case "off":
                    set.hasMicrophone = false;
                    MainChat.SendInfoChat(p, t.characterName.Replace("_", " ") + " 被您关闭了麦克风.");
                    MainChat.SendInfoChat(t, p.characterName.Replace("_", " ") + " 关闭了您的麦克风权限.");
                    break;

                default:
                    MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return;
            }

            t.settings = JsonConvert.SerializeObject(set);
            await t.updateSql();
            return;

        }

        [Command("gotopos")]
        public void COM_GotoPos(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 2) { MainChat.SendInfoChat(p, "[用法] /gotopos x y z"); return; }
            float x; float y; float z; bool xOk = float.TryParse(args[0], out x); bool yOk = float.TryParse(args[1], out y); bool zOk = float.TryParse(args[2], out z);

            if (!xOk || !yOk || !zOk) { MainChat.SendInfoChat(p, "[用法] /gotopos x y z"); return; }

            Position goPos = new Position(x, y, z);
            p.Position = goPos;
        }

        [Command("aobj")]
        public void COM_CreateObject(PlayerModel p, string name)
        {
            if (p.adminLevel < 8) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            GlobalEvents.ShowObjectPlacement(p, name, "admin:showObject");
        }

        [AsyncClientEvent("admin:showObject")]
        public void PlaceAdminObject(PlayerModel p, string rot, string pos, string model)
        {
            if (p.Ping > 250)
                return;

            Globals.System.RoadBlockModel block = new Globals.System.RoadBlockModel();
            Vector3 position = JsonConvert.DeserializeObject<Vector3>(pos);
            position.Z -= 0.2f;
            Vector3 rotation = JsonConvert.DeserializeObject<Vector3>(rot);

            block.owner = p.sqlID;
            block.prop = PropStreamer.Create(model, position, rotation, dimension: p.Dimension, placeObjectOnGroundProperly: true, frozen: true);
            block.textlbl = TextLabelStreamer.Create("~b~[~w~路障 " + block.prop.Id.ToString() + "~b~]~n~~w~创建人: ~y~" + p.characterName.Replace("_", " "), position, streamRange: 2);
            Globals.System.PD.pdRoadBlocks.Add(block);
        }

        [Command("gamecontrols")]
        public void COM_GameControls(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 6) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /gamecontrols [id] [on/off]"); return; }
            int tSql; bool isOK = Int32.TryParse(args[0], out tSql);
            if (!isOK) { MainChat.SendInfoChat(p, "[用法] /gamecontrols [id] [on/off]"); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }

            switch (args[1])
            {
                case "on":
                    MainChat.SendInfoChat(p, t.characterName.Replace("_", " ") + " 被您开启了游戏控制.");
                    MainChat.SendInfoChat(t, p.characterName.Replace("_", " ") + " 开启了您的游戏控制.");
                    GlobalEvents.GameControls(t, true);
                    break;

                case "off":
                    MainChat.SendInfoChat(p, t.characterName.Replace("_", " ") + " 被您关闭了游戏控制..");
                    MainChat.SendInfoChat(t, p.characterName.Replace("_", " ") + " 关闭了您的游戏控制.");
                    GlobalEvents.GameControls(t, false);
                    break;

                default: MainChat.SendInfoChat(p, "[用法] /gamecontrols [id] [on/off]"); return;
            }

        }

        [Command("refreshcarjob")]
        public void COM_RefreshCarJob(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 3) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] 您必须在车内."); return; }

            VehModel v = (VehModel)p.Vehicle;
            if (v.HasData(EntityData.VehicleEntityData.VehicleTempOwner))
                v.DeleteData(EntityData.VehicleEntityData.VehicleTempOwner);

            MainChat.SendInfoChat(p, "> 成功清理车辆临时管理.");
            return;
        }

        [Command("vehsave")]
        public void COM_VehicleSavePos(PlayerModel p)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] 您必须在车内."); return; }

            VehModel v = (VehModel)p.Vehicle;

            v.settings.savePosition = v.Position;
            v.settings.saveRotation = v.Rotation;
            v.settings.SaveDimension = v.Dimension;
            v.Update();
            MainChat.SendInfoChat(p, "> 成功更新车辆的坐标和虚拟世界.");
            return;
        }

        [Command("rtc")]
        public void COM_RespawnThisCar(PlayerModel p)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v == null) { MainChat.SendErrorChat(p, "[错误] 附近没有车辆."); return; }

            v.Position = v.settings.savePosition;
            v.Rotation = v.settings.saveRotation;

            MainChat.SendInfoChat(p, "> 成功刷新附近车辆回数据位置.");
            return;
        }

        [Command("removecars")]
        public async Task COM_RemoveCar(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 6) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v == null) { MainChat.SendErrorChat(p, "[错误] 附近没有车辆."); return; }

            await Database.DatabaseMain.DeleteVehicle(v);
            v.Remove();
        }

        [Command("createmodcar")]
        public async Task COM_CreateModdedCar(PlayerModel p, string _model)
        {
            if (p.adminLevel < 6) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; }
            uint model;
            if (!uint.TryParse(_model, out model)) { MainChat.SendErrorChat(p, "无效数值."); return; }
            if (model <= 0) { MainChat.SendErrorChat(p, CONSTANT.DESC_CreateVehicle); return; }

            IVehicle v = Alt.CreateVehicle((uint)model, p.Position, p.Rotation);
            VehModel veh = (VehModel)v;

            veh.sqlID = await Database.DatabaseMain.CreateVehicle((VehModel)v);
            return;
        }

        [Command("setweather")]
        public void COM_ChangeWeather(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /setweather [0-13]"); return; }

            int w; bool isOK = Int32.TryParse(args[0], out w);
            if (!isOK) { MainChat.SendInfoChat(p, "[用法] /setweather [0-13]"); return; }
            if (w > 14 || w < 0) { MainChat.SendInfoChat(p, "[用法] /setweather [0-13]"); return; }
            ServerEvent.CurrentWeather = w;
            MainChat.SendInfoChat(p, "已更新世界天气.");
            foreach (IPlayer pl in Alt.GetAllPlayers()) { pl.SetWeather((uint)w); }
            return;
        }

        [Command("sgoto")]
        public void COM_SlientGoto(PlayerModel player, int? ID)
        {
            if (player.adminLevel < 5) { MainChat.SendErrorChat(player, CONSTANT.ERR_AdminLevel); return; }
            if (ID == null) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotFound); return; }
            PlayerModel target = GlobalEvents.GetPlayerFromSqlID((int)ID);
            if (target == null) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotFound); return; }

            Position pos = target.Position;
            pos.X = pos.X + 1f;
            player.Position = pos;
            player.Dimension = target.Dimension;

            return;
        }

        [Command("respawnjobcars")]
        public void COM_RespawnJobCars(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 3) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /respawnjobcars [工作ID]"); return; }
            int jobId; bool isOk = Int32.TryParse(args[0], out jobId);
            if (!isOk) { MainChat.SendInfoChat(p, "[用法] /respawnjobcars [工作ID]"); return; }
            if (jobId == 0)
                return;

            foreach (VehModel v in Alt.GetAllVehicles())
            {
                if (v.jobId == jobId)
                {
                    if (v.HasData("Rented"))
                        continue;

                    if (v.Driver == null)
                    {
                        v.SetPositionAsync(v.settings.savePosition);
                        v.SetRotationAsync(v.settings.saveRotation);
                        //v.Position = (v.settings.savePosition);
                        //v.Rotation = (v.settings.saveRotation);
                        v.Repair();
                        v.EngineOn = false;
                        v.LockState = VehicleLockState.Unlocked;
                    }

                }
            }

            if (!p.Exists)
                return;

            MainChat.SendInfoChat(p, "刷新指定工作车辆.");
        }

        [Command("testhair")]
        public void COM_HairTest(PlayerModel p, string colletion, string overlay)
        {
            p.EmitLocked("Update:Hair", colletion, overlay);
        }

        [Command("givedrug")]
        public async Task COM_AuVer(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 6) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; }
            if (args.Length <= 2) { MainChat.SendInfoChat(p, "[用法] /givedrug [id] [物品ID]"); return; }
            int jobId; bool isOk = Int32.TryParse(args[0], out jobId);
            if (!isOk) { MainChat.SendInfoChat(p, "[用法] /givedrug [id] [物品ID] [数量]"); return; }

            if (!Int32.TryParse(args[1], out int amount) || !Int32.TryParse(args[2], out int gamount))
                return;


            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(jobId);
            if (target == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }

            ServerItems i = Items.LSCitems.Find(x => x.ID == 36);
            await Inventory.AddInventoryItem(target, i, amount);
            i = Items.LSCitems.Find(x => x.ID == 37);
            await Inventory.AddInventoryItem(target, i, gamount);

            MainChat.SendInfoChat(p, "已为 " + target.characterName.Replace("_", " ") + " 的库存添加 " + amount + " 个毒品和毒品种子.");
            MainChat.SendInfoChat(target, p.characterName.Replace("_", " ") + " 为您的库存添加了 " + amount + " 个毒品和毒品种子.");
            return;
        }

        [Command("payday")]
        public void COM_SetPayday(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 7) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /payday [数值]"); return; }
            int payday; bool isOK = Int32.TryParse(args[0], out payday);
            if (!isOK) { MainChat.SendInfoChat(p, "[用法] /payday [数值]"); return; }

            ServerGlobalValues.PayDayPrice = payday;

            MainChat.SendInfoChat(p, "[发薪日] 成功设置额外金额为: $" + payday);
            return;
        }

        [Command("paydayexp")]
        public void COM_SetExp(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 7) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /paydayexp [数值]"); return; }
            int payday; bool isOK = Int32.TryParse(args[0], out payday);
            if (!isOK) { MainChat.SendInfoChat(p, "[用法] /paydayexp [数值]"); return; }

            ServerGlobalValues.PayDayExp = payday;

            MainChat.SendInfoChat(p, "[发薪日] 成功设置额外经验值为: " + payday);
            return;
        }

        [Command("cleartatto")]
        public async Task COM_DeleteTatto(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 4) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /cleartatto [玩家ID]"); return; }
            int payday; bool isOK = Int32.TryParse(args[0], out payday);
            if (!isOK) { MainChat.SendInfoChat(p, "[用法] /cleartatto [玩家ID]"); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(payday);
            if (t == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }

            CharacterSettings tSet = JsonConvert.DeserializeObject<CharacterSettings>(t.settings);
            if (tSet == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }

            tSet.tattos = new List<Tattos>();
            t.EmitLocked("Tatto:Load", JsonConvert.SerializeObject(tSet.tattos));
            t.settings = JsonConvert.SerializeObject(tSet);
            await t.updateSql();

            MainChat.SendInfoChat(p, t.characterName.Replace("_", " ") + " 被您清理了角色纹身.");
            MainChat.SendInfoChat(t, p.characterName.Replace("_", " ") + " 清理了您的角色纹身.");
            return;
        }

        [Command("cleardrops")]
        public async void COM_DeleteGroundItems(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 6) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; }
            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                GlobalEvents.SubTitle(t, "~b~服务器: ~w~所有被丢弃在地上的物品将在 20秒 后删除", 5);
            }

            await Task.Delay(20000);
            foreach (Inventory.GroundObj x in Inventory.groundObjects)
            {
                x.Prop.Destroy();
                x.textLabel.Delete();
            }
            Inventory.groundObjects = new List<Inventory.GroundObj>();

            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                GlobalEvents.SubTitle(t, "~b~服务器: ~w~所有被丢弃在地上的~r~物品已被删除", 5);
            }
        }

        [Command("giveitem")]
        public async Task COM_GiveItem(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 6) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; }
            if (args.Length <= 4) { MainChat.SendInfoChat(p, "[用法] /giveitem [id] [物品ID] [数值] [data1] [data2]"); return; }

            int sqlId; bool sqlIdOK = Int32.TryParse(args[0], out sqlId);
            int itemId; bool itemIdOk = Int32.TryParse(args[1], out itemId);
            int amount; bool amountOK = Int32.TryParse(args[2], out amount);

            if (!sqlIdOK || !itemIdOk || !amountOK) { MainChat.SendInfoChat(p, "[用法] /giveitem [id] [物品ID] [数值] [data1] [data2]"); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(sqlId);
            if (t == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }

            ServerItems nItem = Items.LSCitems.Find(x => x.ID == itemId);
            if (nItem == null) { MainChat.SendErrorChat(p, "[错误] 无效物品!"); return; }

            ServerItems addItem = nItem;
            addItem.data = args[3];
            addItem.data2 = string.Join(" ", args[4..]);


            await Inventory.AddInventoryItem(t, addItem, amount);

            MainChat.SendInfoChat(p, "已给予 " + t.characterName.Replace("_", " ") + " 物品 " + addItem.name);
            MainChat.SendInfoChat(t, p.characterName.Replace("_", " ") + " 给了您 " + addItem.name + " 按 I键 查看库存.");
            return;
        }

        [Command("tv")]
        public void COM_SpecPlayer(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /tv ID  || 再次使用 /tv 可关闭"); return; }

            int tSql; bool isOK = Int32.TryParse(args[0], out tSql);
            if (!isOK) { MainChat.SendInfoChat(p, "[用法] /tv ID  || 再次使用 /tv 可关闭"); return; }
            //if (tSql == 7314) { MainChat.SendErrorChat(p, "[错误] 无效玩家!"); return; }
            //if (tSql == 8) { MainChat.SendErrorChat(p, "[错误] 无效玩家!"); return; }
            //if (tSql == 10) { MainChat.SendErrorChat(p, "[错误] 无效玩家!"); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }
            if (t == p) { MainChat.SendErrorChat(p, "[错误] 无法监视自己."); return; }
            //if(t.adminLevel > 4 && p.adminLevel <= 8) { MainChat.SendInfoChat(t, p.characterName + " isimli yönetici şuan sizi specliyor."); }
            //foreach(var tpl in Alt.GetAllPlayers())
            //{
            //    if (tpl.Position.Distance(t.Position) < 20)
            //        MainChat.SendInfoChat(tpl, p.characterName + " isimli yönetici " + t.characterName + " isimli kişiyi specliyor.");
            //}            

            if (p.HasSyncedMetaData("isInSpec"))
            {
                p.GetSyncedMetaData<Position>("isInSpec", out Position _oldPos);
                p.EmitAsync("Spec:OFF", _oldPos);
                //p.Position = _oldPos;
                p.Spawn(_oldPos, 0);
                p.hp = 1000;
                p.Dimension = 0;
                p.showSqlId = true;
                p.fakeName = p.characterName;
                p.Dimension = 0;
                p.DeleteSyncedMetaData("isInSpec");
                p.Visible = true;
                p.Streamed = true;
                return;
            }

            p.Streamed = false;
            p.SetSyncedMetaData("isInSpec", p.Position);
            Position _specPos = t.Position;
            _specPos.Z -= 3;
            p.showSqlId = false;
            p.fakeName = "";
            p.Position = _specPos;
            p.Dimension = t.Dimension;
            p.Visible = false;
            p.EmitAsync("Spec:ON", t.Id);
            MainChat.SendInfoChat(p, "[=] " + t.characterName.Replace("_", " ") + " 被您监视中!");
        }

        [Command("makeadmin")]
        public static async void TaskAdminGiveLevel(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 8) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle
            if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /makeadmin [id] [等级]"); return; }

            int tSql; bool isOk = Int32.TryParse(args[0], out tSql);
            int nLevel; bool nisOk = Int32.TryParse(args[1], out nLevel);
            if (!isOk || !nisOk) { MainChat.SendInfoChat(p, "[用法] /makeadmin [id] [等级]"); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }


            AccountModel tAcc = await Database.DatabaseMain.getAccInfo(t.accountId);
            if (tAcc == null)
                return;

            tAcc.adminLevel = nLevel;
            t.adminLevel = nLevel;
            await tAcc.Update();
            await t.updateSql();

            MainChat.SendInfoChat(p, t.characterName.Replace("_", " ") + " 被您设置为 " + nLevel.ToString() + " 级管理员.");
            MainChat.SendInfoChat(t, p.characterName.Replace("_", " ") + " 将您设置为 " + nLevel.ToString() + " 级管理员.");
        }

        [Command("atune")]
        public void AdminEditCar(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 7) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle           

            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v == null) { MainChat.SendInfoChat(p, "附近没有车辆!"); return; }
            v.SetModKitAsync((byte)1);

            switch (args[0])
            {
                case "mod":
                    int tuningId; bool tuningIDOK = Int32.TryParse(args[1], out tuningId);
                    int tuningVal; bool tuningValOk = Int32.TryParse(args[2], out tuningVal);

                    if (!tuningIDOK || !tuningValOk)
                        return;

                    v.SetMod((byte)tuningId, (byte)tuningVal);
                    break;

                case "neon":
                    if (args.Length <= 3)
                        return;

                    if (!Int32.TryParse(args[1], out int nC1) || !Int32.TryParse(args[2], out int nC2) || !Int32.TryParse(args[3], out int nC3) || !Int32.TryParse(args[4], out int nC4))
                        return;

                    v.SetNeonColorAsync(new Rgba((byte)nC1, (byte)nC2, (byte)nC3, (byte)nC4));
                    v.SetNeonActive(true, true, true, true);
                    break;

                case "color1":
                    if (!Int32.TryParse(args[1], out int newCl1))
                        return;

                    v.SetPrimaryColorAsync((byte)newCl1);
                    break;

                case "color2":
                    if (!Int32.TryParse(args[1], out int newCl2))
                        return;

                    v.SetSecondaryColorAsync((byte)newCl2);
                    break;

                case "tire":
                    if (!Int32.TryParse(args[1], out int newLastik))
                        return;

                    v.SetWheelColorAsync((byte)newLastik);
                    break;

                case "wheel":
                    //v.WheelType = Int32.Parse(arg[1]);
                    if (!Int32.TryParse(args[1], out int newWheel))
                        return;

                    v.SetWheelsAsync((byte)newWheel, (byte)newWheel);
                    v.SetRearWheelAsync((byte)newWheel);
                    break;

                case "wheel2":
                    if (!Int32.TryParse(args[1], out int newWheel2))
                        return;

                    v.SetRearWheelAsync((byte)newWheel2);
                    break;

                case "sedef":
                    if (!Int32.TryParse(args[1], out int newSedef))
                        return;

                    v.SetPearlColorAsync((byte)newSedef);
                    break;

                case "tint":
                    if (!Int32.TryParse(args[1], out int newTint))
                        return;

                    v.SetWindowTintAsync((byte)newTint);

                    break;

                case "lmultipler":
                    if (!float.TryParse(args[1], out float newLightMul))
                        return;

                    v.SetLightsMultiplierAsync(newLightMul);
                    break;

                case "xeoncolor":
                    if (!Int32.TryParse(args[1], out int newXcolor))
                        return;

                    v.SetHeadlightColorAsync((byte)newXcolor);
                    break;

                case "test":
                    if (args[2] == null)
                        return;

                    if (!Int32.TryParse(args[2], out int newModkit))
                        return;

                    bool extraState = false;
                    if (args[1] == "true") extraState = true;

                    v.ToggleExtra((byte)newModkit, extraState);
                    break;


            }

            MainChat.SendInfoChat(p, "[成功更新车辆改装]");
        }

        [Command("updateservercars")]
        public static async void COM_EditVehicleTax(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 8) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle   
            MainChat.SendAdminChat("[车辆] 根据上次更新开始调整车辆税...");

            await AltAsync.Do(async () =>
            {
                await Task.Run(() =>
                {
                    int count = 0;
                    foreach (VehModel v in Alt.GetAllVehicles())
                    {
                        string ModelName = ((VehicleModel)v.Model).ToString();
                        var CarModel = VehicleVendor.getModelWithName(ModelName.ToLower());
                        if (CarModel != null)
                        {
                            v.defaultTax = CarModel.defaultTax;
                            v.fuelConsumption = CarModel.fuelConsumption;
                            v.maxFuel = CarModel.petrolTank;
                            v.inventoryCapacity = CarModel.inventoryCapacity;
                            if (v.price <= 10000)
                                v.price = CarModel.price;
                            v.Update();
                            count++;
                        }

                    }
                    MainChat.SendAdminChat("[车辆] 已更新 " + count + " 个车辆的信息 (税收、油耗、油箱、行李容量).");
                });
            });
        }

        [Command("subannounce")]
        public void COM_SubAnn(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 6) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle   
            int newTax; bool isOk = Int32.TryParse(args[0], out newTax);

            string text = string.Join(" ", args[1..]);

            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                GlobalEvents.SubTitle(t, text, newTax);
            }

            MainChat.SendInfoChat(p, "已发送公告.");
        }


        [Command("banjob")]
        public static async void COM_JobBan(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 6) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle
            if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /banjob [id] yes/no"); return; }

            if (!Int32.TryParse(args[0], out int tSql))
                return;


            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null) { MainChat.SendInfoChat(p, "无效玩家."); return; }

            CharacterSettings settings = JsonConvert.DeserializeObject<CharacterSettings>(t.settings);
            switch (args[1])
            {
                case "yes":
                    settings.jobBan = true;
                    break;

                case "no":
                    settings.jobBan = false;
                    break;

                default:
                    settings.jobBan = false;
                    break;
            }

            t.settings = JsonConvert.SerializeObject(settings);
            await t.updateSql();

            MainChat.SendInfoChat(p, "成功更新指定工作封禁状态.");
        }

        [Command("createautorepair")]
        public void COM_CreateRepairStation(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 6) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; }
            OtherSystem.LSCsystems.AutoRepairSystem.RepairModel nS = new AutoRepairSystem.RepairModel()
            {
                repairPos = p.Position,
                Dimension = p.Dimension,
                Price = 100,
                factionID = 0,
                textLabelID = (int)TextLabelStreamer.Create("~b~[~w~维修车辆~b~]~n~~w~指令: ~g~/fixcar~n~~w~价格: ~g~$" + 100, p.Position, dimension: p.Dimension, streamRange: 3, font: 0).Id,
            };

            AutoRepairSystem.repairSystem.Add(nS);
            MainChat.SendInfoChat(p, "[?] 成功添加车辆维修点.");
        }

        [Command("editautorepair")]
        public void COM_EditRepairStation(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 6) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /editautorepair [选项] [数值]<br>可用选项: faction - price - delete"); return; }
            AutoRepairSystem.RepairModel cS = AutoRepairSystem.repairSystem.Find(x => x.repairPos.Distance(p.Position) < 5 && x.Dimension == p.Dimension);
            if (cS == null) { MainChat.SendErrorChat(p, "[错误] 无效车辆维修点!"); return; }

            switch (args[0])
            {
                case "faction":
                    if (args[1] == null) { MainChat.SendInfoChat(p, "[用法] /editautorepair faction [id]"); return; }
                    int nFaction; bool isFactionOk = Int32.TryParse(args[1], out nFaction);
                    if (!isFactionOk) { MainChat.SendInfoChat(p, "[用法] /editautorepair faction [id]"); return; }
                    cS.factionID = nFaction;
                    MainChat.SendInfoChat(p, "[?] 成功设置指定车辆维修点的组织为 " + nFaction);
                    return;

                case "price":
                    if (args[1] == null) { MainChat.SendInfoChat(p, "[用法] /editautorepair price [数值]"); return; }
                    int nPrice; bool isPriceOk = Int32.TryParse(args[1], out nPrice);
                    if (!isPriceOk) { MainChat.SendInfoChat(p, "[用法] /editautorepair price [数值]"); return; }
                    cS.Price = nPrice;
                    TextLabelStreamer.GetDynamicTextLabel((ulong)cS.textLabelID).Text = "~b~[~w~维修车辆~b~]~n~~w~指令: ~g~/fixcar~n~~w~费用: ~g~$" + nPrice;

                    MainChat.SendInfoChat(p, "[?] 成功设置指定车辆维修点的价格为 $" + nPrice);
                    return;

                case "delete":
                    TextLabelStreamer.GetDynamicTextLabel((ulong)cS.textLabelID).Delete();
                    AutoRepairSystem.repairSystem.Remove(cS);
                    MainChat.SendInfoChat(p, "[?] 成功删除指定车辆维修点.");
                    return;
            }
        }

        [Command("saveallcarmod")]
        public static async void COM_VehModifySave(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 8) { return; }
            foreach (VehModel v in Alt.GetAllVehicles())
            {
                v.settings.ModifiyData = await v.GetAppearanceDataAsync();
            }
            MainChat.SendInfoChat(p, "成功保存所有车辆的改装数据.");
        }

        [Command("copycarapp")]
        public void COM_CopyCarApperance(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 6) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /copycar [被复制的车辆ID] [目标车辆ID]"); return; }
            if (!Int32.TryParse(args[0], out int copyCar) || !Int32.TryParse(args[1], out int targetCar)) { MainChat.SendInfoChat(p, "[用法] /copycar [被复制的车辆ID] [目标车辆ID]"); return; }

            VehModel copy = Vehicle.VehicleMain.getVehicleFromSqlId(copyCar);
            VehModel target = Vehicle.VehicleMain.getVehicleFromSqlId(targetCar);
            if (copy == null || target == null) { MainChat.SendErrorChat(p, "[错误] 其中一辆车无效!"); return; }

            target.settings.ModifiyData = copy.settings.ModifiyData;
            target.AppearanceData = copy.settings.ModifiyData;
            target.Update();
            MainChat.SendInfoChat(p, "[?] 成功复制车辆外观数据!");
        }

        [Command("setplatetype")]
        public void COM_EditPlateType(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v == null)
                return;

            if (args.Length < 0)
                return;

            if (!Int32.TryParse(args[0], out int plateType))
                return;

            if (plateType > 5)
                return;


            v.SetNumberplateIndexAsync((uint)plateType);

            v.settings.ModifiyData = v.AppearanceData;
            v.Update();

            MainChat.SendInfoChat(p, "[!] 成功设置车牌号");
        }

        [Command("k")]

        public void Test(PlayerModel p, int type, int KID, int TID)
        {
            if (p.adminLevel <= 4)
                return;
            GlobalEvents.SetClothes(p, type, KID, TID);

        }
        [Command("k2")]
        public void Test2(PlayerModel p, int type, int KID, int TID)
        {
            if (p.adminLevel <= 4)
                return;
            p.EmitAsync("SetClothesProps", type, KID, TID);
        }

        [Command("editor")]
        public void COM_OpenEditor(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 5)
                return;

            p.EmitAsync("Commando:Toggle");
        }

        [Command("allcars")]
        public void COM_CheckCharCount(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 5)
                return;

            if (args.Length <= 0)
                return;

            //var cars = Alt.GetAllVehicles().Where(x => x.Model == Alt.Hash(args[0])).ToList();
            var cars = Alt.GetAllVehicles().Where(x => x.Model == Alt.Hash(args[0])).Count();

            MainChat.SendInfoChat(p, "[?] 模型为 " + args[0] + " 的车辆总计有: " + cars);

        }

        [Command("spawntestcar")]
        public void COM_CrateTestCar(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /spawntestcar [model]"); return; }
            if (args[0] == "hydra") { MainChat.SendErrorChat(p, "[?] 无法创建这个, 你要干什么."); return; }
            IVehicle v = Alt.CreateVehicle(args[0], p.Position, p.Rotation);
            VehModel veh = (VehModel)v;
            veh.sqlID = 90000 + veh.Id;
            veh.currentFuel = 5;
            MainChat.SendInfoChat(p, "[!] 成功创建模型 " + args[0] + " 的车辆, 临时编号 " + v.Id + " 临时数据库ID / " + veh.sqlID);
        }

        [Command("testcarlist")]
        public void COM_TestCarList(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 4)
                return;

            string text = "<center>临时车辆列表</center><br>";
            foreach (VehModel v in Alt.GetAllVehicles())
            {
                if (v.sqlID >= 90000)
                    text += ((VehicleModel)v.Model).ToString() + " | ID: " + v.Id + " | 数据库ID: " + v.sqlID + "<br>";
            }

            MainChat.SendInfoChat(p, text, true);
        }

        [Command("setskin")]
        public void COM_SetSkin(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /setskin [id] [model(string)]"); return; }
            if (!Int32.TryParse(args[0], out int tSql)) { MainChat.SendInfoChat(p, "[用法] /setskin [id] [model(string)]"); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家!"); return; }

            t.SetModelAsync(Alt.Hash(args[1]));
            MainChat.SendInfoChat(p, "[!] 成功设置玩家模型.");
            t.SetMaxHealthAsync(1000);
        }

        [Command("kickall")]
        public void COM_KickAll(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 6) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /kickall [原因]"); return; }
            string reason = string.Join(" ", args);
            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (t == p) { continue; }
                t.KickAsync(reason);
            }
            MainChat.SendInfoChat(p, "[- -] 成功将所有玩家踢出服务器.");
        }

        [Command("addsick")]
        public static async void COM_AddDisease(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 7) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (args.Length <= 2) { MainChat.SendInfoChat(p, "[用法] /addsick [id] [level] [病]"); return; }

            if (!Int32.TryParse(args[0], out int tSql) || !Int32.TryParse(args[1], out int level)) { MainChat.SendInfoChat(p, "[用法]..."); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家."); return; }
            if (level < 1 || level > 10) { MainChat.SendInfoChat(p, "[用法] /addsick [id] [level] [病]"); return; }

            DiseaseModel dis = t.injured.diseases.Find(x => x.DiseaseName == string.Join(" ", args[2..]));
            if (dis == null)
            {
                dis = new DiseaseModel() { DiseaseName = string.Join(" ", args[2..]), DiseaseValue = level, };
                t.injured.diseases.Add(dis);
            }
            else
            {
                dis.DiseaseValue = level;
            }
            await t.updateSql();

            MainChat.SendErrorChat(p, "[!] 设置 " + t.characterName.Replace("_", " ") + " 的病为 " + string.Join(" ", args[2..]));
            MainChat.SendInfoChat(t, "[生病] 你突然觉得身体不舒服, 建议去医院检查一下.<br>(( 生病不医治会导致角色死亡 ))");
        }

        [Command("checksick")]
        public void COM_ShowDisease(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 3) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /checksick [id]"); return; }

            if (!Int32.TryParse(args[0], out int tSql)) { MainChat.SendInfoChat(p, "[用法] /checksick [id]"); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家"); return; }

            string diases = "";
            foreach (DiseaseModel x in t.injured.diseases)
            {
                diases += x.DiseaseName + " " + x.DiseaseValue + " <br>";
            }

            p.SendChatMessage(t.characterName.Replace("_", " ") + " 的生病情况:");
            p.SendChatMessage(diases);
        }

        [Command("clearsick")]
        public void COM_ResetDisease(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /clearsick [id]"); return; }

            if (!Int32.TryParse(args[0], out int tSql)) { MainChat.SendInfoChat(p, "[用法] /clearsick [id]"); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家"); return; }

            t.injured.diseases = new List<DiseaseModel>();

            p.SendChatMessage(t.characterName.Replace("_", " ") + " 的生病情况被您重置了.");
            MainChat.SendInfoChat(t, "[?] " + p.characterName.Replace('_', ' ') + " 将您的角色生病情况重置了.");
        }

        [Command("givegun")]
        public void COM_AddWeapon(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (args.Length <= 2) { MainChat.SendInfoChat(p, "[用法] /givegun [id] [弹药] [武器]"); return; }

            if (!Int32.TryParse(args[0], out int tSql) || !Int32.TryParse(args[1], out int bullet)) { MainChat.SendInfoChat(p, "[用法] /..."); return; }
            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null) { MainChat.SendErrorChat(p, "[!] 无效玩家!"); return; }
            t.SetSyncedMetaData("PWepBullet_" + Alt.Hash(args[2]), true);
            t.GiveWeaponAsync(Alt.Hash(args[2]), bullet, true);
        }

        [Command("disarm")]
        public void COM_RemoveAllweapon(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /disarm [id]"); return; }
            if (!Int32.TryParse(args[0], out int tSql)) { MainChat.SendInfoChat(p, "[用法] /disarm [id]"); return; }
            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null)
                return;

            t.RemoveAllWeaponsAsync();
            p.SendChatMessage("[!] 成功没收指定玩家武器."); // 
        }

        [Command("clearapb")]
        public static async void COM_ClearSicil(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /clearapb [id]"); return; }
            if (!Int32.TryParse(args[0], out int tSql)) { MainChat.SendInfoChat(p, "[用法] /clearapb [id]"); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家!"); return; }

            await Database.DatabaseMain.ClearPlayerMDCDatas(t.sqlID);
            MainChat.SendErrorChat(p, "[!]" + t.characterName.Replace("_", " ") + " 在MDC的数据被您清空.");
        }

        [Command("hash")]
        public void COM_GetHash(PlayerModel p, string code)
        {
            if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            MainChat.SendInfoChat(p, "[!] " + code + " 的哈希值 -> " + Alt.Hash(code));
        }

        [Command("clearbikes")]
        public static async void COM_RemoveAllCycles(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 8) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            int total = 0;
            foreach (VehModel v in Alt.GetAllVehicles())
            {
                if (v.Model == (uint)VehicleModel.Bmx || v.Model == (uint)VehicleModel.Cruiser || v.Model == (uint)VehicleModel.Fixter || v.Model == (uint)VehicleModel.Scorcher || v.Model == (uint)VehicleModel.TriBike || v.Model == (uint)VehicleModel.TriBike2 || v.Model == (uint)VehicleModel.TriBike3)
                {
                    await Database.DatabaseMain.DeleteVehicle(v);
                    v.Remove();
                    total++;
                }
            }

            MainChat.SendInfoChat(p, "[!] 总计清理 " + total + " 辆自行车/两轮车.");
        }

        /*[Command("ck")]
        public void COM_SetPlayerCK(PlayerModel p, params string[] args)
        {

        }*/

        [Command("cartag")]
        public void COM_Vehicletag(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /cartag 1-0 [文本]"); return; }

            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v == null) { MainChat.SendErrorChat(p, "[错误] 附近没有车辆/无效车辆!"); return; }

            switch (args[0])
            {
                case "0":
                    GlobalEvents.SetVehicleTag(v, string.Join(" ", args[1..]));
                    MainChat.SendInfoChat(p, "[!] 成功添加车辆标签");
                    return;

                case "1":
                    GlobalEvents.SetVehicleTag(v, string.Join(" ", args[1..]), false);
                    MainChat.SendInfoChat(p, "[!] 成功添加车辆标签");
                    return;

                default:
                    return;
            }
        }
        [Command("clearcartag")]
        public void COM_ClearVehicleTag(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v == null) { MainChat.SendErrorChat(p, "[错误] 附近没有车辆/无效车辆!"); return; }

            GlobalEvents.ClearVehicleTag(v);
            GlobalEvents.ClearVehicleTag(v, false);
        }

        [Command("savesettings")]
        public static async void COM_ServerSave(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            await Database.DatabaseMain.SaveServerSettings();
            MainChat.SendErrorChat(p, "成功保存服务器数据(配置信息)");
        }

        [Command("cleartrunk")]
        public void COM_ClearVehicleTrunk(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 5)
            {
                MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError);
                return;
            }

            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v == null)
            {
                MainChat.SendErrorChat(p, "[错误] 附近没有车辆/无效车辆!");
                return;
            }

            v.vehInv = "[]";
            v.Update();
        }

        [Command("removehouse")]
        public async Task COM_DeleteHouse(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 6)
            {
                MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError);
                return;
            }
            if (!Int32.TryParse(args[0], out int houseID))
            {
                MainChat.SendInfoChat(p, "[用法] /removehouse [房屋ID]");
                return;
            }

            (HouseModel, PlayerLabel, Marker) house = await Props.Houses.getHouseById(houseID);
            if (house.Item1 == null)
            {
                MainChat.SendErrorChat(p, "[错误] 无效房屋.");
                return;
            }

            house.Item2.Delete();
            house.Item3.Destroy();
            house.Item1.Delete();
            MainChat.SendInfoChat(p, "[!] 成功删除 房屋" + houseID);
        }

        [Command("checkcar")]
        public void COM_ShowCarsWithOwnerID(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (!Int32.TryParse(args[0], out int ownerId)) { MainChat.SendInfoChat(p, "[用法] /checkcar [车主ID]"); return; }
            string text = "<center>车辆列表 [" + ownerId + "]</center>";
            foreach (VehModel v in Alt.GetAllVehicles())
            {
                if (v.owner == ownerId)
                {
                    var ModelName = (VehicleModel)v.Model;
                    text += "<br>[" + v.sqlID + "(" + v.Id + ")] 模型: " + ModelName.ToString() + " - 车牌: " + v.NumberplateText;
                }
            }
            MainChat.SendInfoChat(p, text);
        }

        [Command("serverstat")]
        public void COM_SetServerStatus(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 5) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }

            MainChat.SendInfoChat(p, "[?] 服务器状态信息已更新.");
        }

        [Command("giveallcash")]
        public static async void COM_GiveMoneyAll(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 8) { MainChat.SendErrorChat(p, "[错误]无权操作!"); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /giveallcash [数值]"); return; }

            if (!Int32.TryParse(args[0], out int cash)) { MainChat.SendInfoChat(p, "[用法] /giveallcash [数值]"); return; }

            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                t.cash += cash;
                await t.updateSql();
                MainChat.SendInfoChat(t, "[$] " + p.characterName.Replace("_", " ") + " 给所有玩家给予了 $" + cash + " 现金.");
            }
        }

        [Command("giverangecash")]
        public static async void COM_NearPlayers(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 8) { MainChat.SendErrorChat(p, "[错误]无权操作!"); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /giverangecash [数值]"); return; }

            if (!Int32.TryParse(args[0], out int cash)) { MainChat.SendInfoChat(p, "[用法] /giverangecash [数值]"); return; }

            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (t.Position.Distance(p.Position) < 15)
                {
                    t.cash += cash;
                    await t.updateSql();
                    MainChat.SendInfoChat(t, "[$] " + p.characterName.Replace("_", " ") + " 给附近的玩家给予了 $" + cash + " 现金.");
                }
            }
        }

        [Command("showaccent")]
        public void COM_ShowPlayersAccents(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 3) { MainChat.SendErrorChat(p, "[错误]无权操作!"); return; }

            string accents = "<center>玩家口音</center>";
            foreach (PlayerModel x in Alt.GetAllPlayers())
            {
                if (x.HasData("Player:Accent"))
                {
                    x.GetData("Player:Accent", out string xAcc);
                    accents += "<br>" + x.characterName + "-" + xAcc + " ";
                }

            }
            MainChat.SendInfoChat(p, accents);
        }

        [Command("resetallcartax")]
        public void COM_RemoveAllFines(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 8)
                return;
            foreach (VehModel v in Alt.GetAllVehicles())
            {
                v.fine = 0;
            }
            MainChat.SendInfoChat(p, "[!] 所有车辆的税已经设置为0.");
        }

        [Command("reducecartax")]
        public void COM_RemoveAllFines2(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 7)
                return;

            if (!Int32.TryParse(args[0], out int desc))
                return;


            foreach (VehModel v in Alt.GetAllVehicles())
            {
                v.fine -= desc;
                if (v.fine <= 0)
                    v.fine = 5;
            }
            MainChat.SendInfoChat(p, "[!] 所有车辆的税已减少 $" + desc);
        }

        [Command("fillallcar")]
        public void COM_FillAllVehicles(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 7)
                return;

            foreach (VehModel v in Alt.GetAllVehicles())
            {
                if (v.maxFuel >= 1)
                    v.currentFuel = v.maxFuel;
            }

            MainChat.SendInfoChat(p, "[!] 成功加满所有车的油箱.");
        }

        [Command("nextad")]
        public void COM_ShowTrustedAdversiment(PlayerModel p, params string[] args)
        {
            ///if(p.adminLevel <= 0) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            string ads = "<center>即将投放的广告</center><br>";

            foreach (var x in trustedAdversiments)
            {
                ads += x.addvesimentText + " (" + x.senderID + ")<br>";
            }

            MainChat.SendInfoChat(p, ads, true);
        }

        [Command("setallcarlevel")]
        public void COM_FixVehicleSecurityLevels(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 8) { MainChat.SendErrorChat(p, "[错误]无权操作!"); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /setallcarlevel [级别]"); return; }
            if (!Int32.TryParse(args[0], out int level)) { MainChat.SendInfoChat(p, "[用法] /setallcarlevel [级别]"); return; }

            foreach (VehModel v in Alt.GetAllVehicles())
            {
                v.settings.SecurityLevel = level;
                v.Update();
            }

            MainChat.SendInfoChat(p, "[=] 成功设置所有车辆的防盗等级为: " + level);
        }

        [Command("arankname")]
        public static async void COM_ARankName(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 8) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /arankname [id] [名称]"); return; }
            if (!Int32.TryParse(args[0], out int tSql)) { MainChat.SendInfoChat(p, "[用法] /arankname [id] [名称]"); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家!"); return; }

            var settings = JsonConvert.DeserializeObject<CharacterSettings>(t.settings);
            if (settings == null)
                return;

            settings.AdminRankName = string.Join(" ", args[1..]);
            t.settings = JsonConvert.SerializeObject(settings);
            await t.updateSql();

            MainChat.SendInfoChat(p, "[!] " + t.characterName + " 的管理员阶级名称被修改为 " + string.Join(" ", args[1..]));
            MainChat.SendInfoChat(t, "[!] 您的管理员阶级名称已经更新, 请输入 /admins 查看");
        }

        [Command("hwidban")]
        public static async void COM_HwidBan(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 8) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /hwidban [id] [原因]"); return; }
            if (!Int32.TryParse(args[0], out int tSql)) { MainChat.SendInfoChat(p, "[用法] /hwidban [id] [原因]"); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null)
            {
                PlayerModelInfo target = await Database.DatabaseMain.getCharacterInfo(tSql);
                if (target == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家!"); return; }
                AccountModel offlineAccount = await Database.DatabaseMain.getAccInfo(target.accountId);
                if (offlineAccount == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家!"); return; }
                await Database.DatabaseMain.AddHwidBan(offlineAccount.HwId, offlineAccount.HwIdEx, p.characterName, string.Join(" ", args[1..]));
                offlineAccount.banned = true;
                await offlineAccount.Update();

                foreach (PlayerModel ct in Alt.GetAllPlayers())
                {
                    ct.SendChatMessage("{FF0000}[!] {FFFFFF}" + offlineAccount.forumName + ")" + " {FF0000}被封禁硬件码.<br>原因: {FFFFFF}" + string.Join(" ", args[1..]));
                }
            }
            else
            {
                AccountModel accountModel = await Database.DatabaseMain.getAccInfo(t.accountId);
                if (accountModel == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家!"); return; }
                accountModel.banned = true;
                await Database.DatabaseMain.AddHwidBan(t, p.characterName, string.Join(" ", args[1..]));

                foreach (PlayerModel yt in Alt.GetAllPlayers())
                {
                    yt.SendChatMessage("{FF0000}[!] {FFFFFF}" + accountModel.forumName + ")" + " {FF0000}被封禁硬件码.<br>原因: {FFFFFF}" + string.Join(" ", args[1..]));
                }

                t.Kick("您已被封禁硬件码, 原因: " + string.Join(" ", args[1..]));
            }
            MainChat.SendInfoChat(p, "[?] 哎 :(");
        }

        [Command("showadmininfo")]
        public static async void COM_ShowAdminInfo(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 7) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            if (args.Length <= 0) { MainChat.SendErrorChat(p, "[用法] /showadmininfo [id]"); return; }
            if (!Int32.TryParse(args[0], out int tSql)) { MainChat.SendErrorChat(p, "[用法] /showadmininfo [id]"); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家!"); return; }

            AccountModel acc = await Database.DatabaseMain.getAccInfo(t.accountId);
            MainChat.SendInfoChat(p, "[" + acc.forumName + "]<br>累计回应举报: " + acc.ReportCount + "<br>累计回应求助: " + acc.QuestionCount + "<br>累计处理广告: " + acc.AdversimentCount);
        }

        [Command("makelawyer")]
        public static async void COM_MakeLawyer(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 2) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /makelawyer [id]"); return; }

            if (!Int32.TryParse(args[0], out int tSql)) { MainChat.SendInfoChat(p, "[用法] /makelawyer [id]"); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家!"); return; }

            var set = JsonConvert.DeserializeObject<CharacterSettings>(t.settings);
            set.isLawyer = !set.isLawyer;
            t.settings = JsonConvert.SerializeObject(set);
            await t.updateSql();
            MainChat.SendInfoChat(p, "[!] " + t.characterName.Replace("_", " ") + " 被您设置了律师身份为: " + (set.isLawyer ? "启用" : "取消"));
            MainChat.SendInfoChat(t, "[!] " + p.characterName.Replace("_", " ") + " 将您的律师身份设置为: " + (set.isLawyer ? "启用" : "取消"));
        }

        [Command("setlawyerdept")]
        public static async void COM_SetLawyerDepartment(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 2) { MainChat.SendErrorChat(p, "[错误] 无权限操作."); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /setlawyerdept [ID] [事务所/办公室名称, 输入 'none' 取消]"); return; }
            if (!Int32.TryParse(args[0], out int tSql)) { MainChat.SendInfoChat(p, "[用法] /setlawyerdept [ID] [事务所/办公室名称, 输入 'none' 取消]"); return; }
            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家!"); return; }
            var set = JsonConvert.DeserializeObject<CharacterSettings>(t.settings);
            if (set == null)
                return;

            switch (args[1])
            {
                case "none":
                    set.LawyerDep = "";
                    break;
                default:
                    set.LawyerDep = string.Join(" ", args[1..]);
                    break;
            }

            t.settings = JsonConvert.SerializeObject(set);
            await t.updateSql();
            MainChat.SendInfoChat(p, "[!] 成功更新指定玩家律师组织.");
        }

        [Command("cban")]
        public static async void COM_BanCharacter(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /cban [ID] [原因]"); return; }
            if (!Int32.TryParse(args[0], out int tSql)) { MainChat.SendInfoChat(p, "[用法] /cban [ID] [原因]"); return; }

            AccountModel acc = await Database.DatabaseMain.getAccInfo(p.accountId);
            if (acc == null)
                return;



            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (target == null)
            {
                PlayerModelInfo offlineTarget = await Database.DatabaseMain.getCharacterInfo(tSql);
                if (offlineTarget == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家!"); return; }

                offlineTarget.isCk = true;
                offlineTarget.updateSql();
                await Database.DatabaseMain.AddAccountLog(3, offlineTarget.sqlID, offlineTarget.accountId, "角色禁令", acc.kookId);

                foreach (PlayerModel offlineMsgTarget in Alt.GetAllPlayers())
                {
                    offlineMsgTarget.SendChatMessage("{EC1111}[封禁]{FFFFFF} " + offlineTarget.characterName.Replace("_", " ") + " 被角色封禁, 账号: {F04712}" + acc.forumName);
                }
            }
            else
            {
                target.isCk = true;
                await target.updateSql();

                target.Kick("您的角色已被封禁.");

                await Database.DatabaseMain.AddAccountLog(3, target.sqlID, target.accountId, "角色禁令", acc.kookId);

                foreach (PlayerModel MsgTarget in Alt.GetAllPlayers())
                {
                    MsgTarget.SendChatMessage("{EC1111}[封禁]{FFFFFF} " + target.characterName.Replace("_", " ") + " 被角色封禁, 账号: {F04712}" + acc.forumName);
                }
            }
        }

        [Command("settaxstat")]
        public void COM_TaxState(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, "[错误] 无权限操作!"); return; }
            if (ServerGlobalValues.serverCanTax)
            {
                ServerGlobalValues.serverCanTax = false;
                MainChat.SendAdminChat("[?] 成功关闭服务器税收.");
            }
            else
            {
                ServerGlobalValues.serverCanTax = true;
                MainChat.SendAdminChat("[?] 成功开启服务器税收.");
            }
        }

        [Command("serverpass")]
        public void COM_ServerPassword(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, "[错误] 无权限操作!"); return; }

            if (args.Length <= 0)
                Alt.Core.SetPassword(string.Empty);
            else
                Alt.Core.SetPassword(string.Join(" ", args));

            MainChat.SendInfoChat(p, "[成功设置服务器密码]");
        }

        [Command("getss")]
        public void COM_GetPlayerSS(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, "[错误] 无权限操作!"); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /getss [ID]"); return; }
            if (!Int32.TryParse(args[0], out int tSQL)) { MainChat.SendInfoChat(p, "[用法] /getss [ID]"); return; }

            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(tSQL);
            if (target == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家!"); return; }

            target.EmitAsync("Chat:TakeScreenShot", p.characterName);
        }
        [AsyncClientEvent("Chat:ScreenCapture")]
        public static async void GetPlayerSS_Response(PlayerModel p, string link, string admin)
        {
            //Alt.Log("Görüntü bildirimi düştü.");

            string name = p.characterName;
        }

        [Command("ktest")]
        public void COM_KTest(PlayerModel p, params string[] args)
        {
            //if (args.Length <= 3)
            //    return;

            //if (!byte.TryParse(args[0], out byte comp) || !ushort.TryParse(args[1], out ushort varitaion) || !byte.TryParse(args[2], out byte texture) || !byte.TryParse(args[3], out byte x))
            //    return;

            /*for(int a = 0; a < 400; a++)
            {
                await Task.Delay(100);
                p.SetClothes(11, (ushort)a, 0, 2);
            }
            p.SetClothes(6, 57, 9, 2);
            p.SendChatMessage("Kıyafetler ayarlandı.");*/
        }

        [Command("oadmin")]
        public void ObjectAdmin(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, "[错误] 无权限操作."); return; }
            if (p.Vehicle != null)
            {
                VehModel v = (VehModel)p.Vehicle;
                if (v.HasStreamSyncedMetaData("AttachedObjects"))
                {
                    AttachmentSystem.deleteAllAttachs(v);
                    MainChat.SendErrorChat(p, "[OK]");
                }
                else
                {
                    if (!double.TryParse(args[2], out double xPos) || !double.TryParse(args[3], out double yPos) || !double.TryParse(args[4], out double zPos) ||
                        !double.TryParse(args[5], out double xRot) || !double.TryParse(args[6], out double yRot) || !double.TryParse(args[7], out double zRot))
                    {
                        MainChat.SendErrorChat(p, "[错误] 无效参数!");
                        return;
                    }
                    AttachmentSystem.ObjectModel vO = new AttachmentSystem.ObjectModel()
                    {
                        Model = args[0],
                        boneIndex = args[1],
                        xPos = xPos,
                        yPos = yPos,
                        zPos = zPos,
                        xRot = xRot,
                        yRot = yRot,
                        zRot = zRot,
                    };

                    AttachmentSystem.AddAttach(v, vO);
                    MainChat.SendErrorChat(p, "[OK]");
                }
            }
            else
            {
                if (p.HasStreamSyncedMetaData("AttachedObjects"))
                {
                    AttachmentSystem.deleteAllAttachs(p);
                    MainChat.SendErrorChat(p, "[OK]");
                }
                else
                {
                    if (!double.TryParse(args[2], out double xPos) || !double.TryParse(args[3], out double yPos) || !double.TryParse(args[4], out double zPos) ||
                        !double.TryParse(args[5], out double xRot) || !double.TryParse(args[6], out double yRot) || !double.TryParse(args[7], out double zRot))
                    {
                        MainChat.SendErrorChat(p, "[错误] 无效参数!");
                        return;
                    }
                    AttachmentSystem.ObjectModel vO = new AttachmentSystem.ObjectModel()
                    {
                        Model = args[0],
                        boneIndex = args[1],
                        xPos = xPos,
                        yPos = yPos,
                        zPos = zPos,
                        xRot = xRot,
                        yRot = yRot,
                        zRot = zRot,
                    };

                    AttachmentSystem.AddAttach(p, vO);
                    MainChat.SendErrorChat(p, "[OK]");
                }
            }
        }

        [Command("oadmin2")]
        public void ObjectAdmin2(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, "[错误] 无权限操作."); return; }
            if (p.Vehicle != null)
            {
                VehModel v = (VehModel)p.Vehicle;

                if (!double.TryParse(args[2], out double xPos) || !double.TryParse(args[3], out double yPos) || !double.TryParse(args[4], out double zPos) ||
                    !double.TryParse(args[5], out double xRot) || !double.TryParse(args[6], out double yRot) || !double.TryParse(args[7], out double zRot))
                {
                    MainChat.SendErrorChat(p, "[错误] 无效参数!");
                    return;
                }
                AttachmentSystem.ObjectModel vO = new AttachmentSystem.ObjectModel()
                {
                    Model = args[0],
                    boneIndex = args[1],
                    xPos = xPos,
                    yPos = yPos,
                    zPos = zPos,
                    xRot = xRot,
                    yRot = yRot,
                    zRot = zRot,
                };

                AttachmentSystem.AddAttach(v, vO);
                MainChat.SendErrorChat(p, "[OK]");

            }
            else
            {

                if (!double.TryParse(args[2], out double xPos) || !double.TryParse(args[3], out double yPos) || !double.TryParse(args[4], out double zPos) ||
                    !double.TryParse(args[5], out double xRot) || !double.TryParse(args[6], out double yRot) || !double.TryParse(args[7], out double zRot))
                {
                    MainChat.SendErrorChat(p, "[错误] 无效参数!");
                    return;
                }
                AttachmentSystem.ObjectModel vO = new AttachmentSystem.ObjectModel()
                {
                    Model = args[0],
                    boneIndex = args[1],
                    xPos = xPos,
                    yPos = yPos,
                    zPos = zPos,
                    xRot = xRot,
                    yRot = yRot,
                    zRot = zRot,
                };

                AttachmentSystem.AddAttach(p, vO);
                MainChat.SendErrorChat(p, "[OK]");

            }
        }

        [Command("addinfo")]
        public static async void COM_CreateInfo(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, "[错误] 无权限操作!"); return; }
            if (args.Length <= 2) { MainChat.SendInfoChat(p, "[用法] /addinfo [id] [标题(如有空格使用'_')] [消息]"); return; }
            if (!Int32.TryParse(args[0], out int tSQL)) { MainChat.SendInfoChat(p, "[用法] /addinfo [id] [标题(如有空格使用'_')] [消息]"); return; }
            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(tSQL);
            if (target != null)
            {
                AccountModel acc = await Database.DatabaseMain.getAccInfo(target.accountId);
                if (acc == null)
                    return;

                acc.OtherData.Info.Add(new OtherData_Inner.Informations()
                {
                    Title = args[1].Replace('_', ' '),
                    Body = string.Join(" ", args[2..]),
                    Date = DateTime.Now
                });

                await acc.Update();
                MainChat.SendInfoChat(target, "{2d90a9}[服务器] {FFFFFF}您有 {DCFF00}" + acc.OtherData.Info.Count + "{FFFFFF} 个待查看的通知. 请输入 {3FFF00}/{FFFFFF}bildirimler");
            }
            else
            {
                PlayerModelInfo targeti = await Database.DatabaseMain.getCharacterInfo(tSQL);
                if (targeti == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家!"); return; }

                AccountModel acc = await Database.DatabaseMain.getAccInfo(targeti.accountId);
                if (acc == null)
                    return;

                acc.OtherData.Info.Add(new OtherData_Inner.Informations()
                {
                    Title = args[1].Replace('_', ' '),
                    Body = string.Join(" ", args[2..]),
                    Date = DateTime.Now
                });

                await acc.Update();
            }

            MainChat.SendInfoChat(p, "[?] 成功发送通知至玩家ID " + args[0]);
        }

        [Command("refund")]
        public static async void COM_AddRefund(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, "[错误] 无权限操作!"); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /addrefund [id] [数值] [标题(如有空格使用'_')] [内容]"); return; }
            if (!Int32.TryParse(args[0], out int tSQL) || !Int32.TryParse(args[1], out int Cash)) { MainChat.SendInfoChat(p, "[用法] /addrefund [id] [数值] [标题(如有空格使用'_')] [内容]"); return; }
            if (await GlobalEvents.AddAccountRefund(tSQL, args[2].Replace('_', ' '), string.Join(" ", args[3..]), DateTime.Now, Cash))
            {
                MainChat.SendInfoChat(p, "[=] 成功给账号ID " + tSQL + " 发送 - " + args[2].Replace('_', ' ') + " - 补偿单 - $" + Cash);
            }
            else
            {
                MainChat.SendErrorChat(p, "[错误] 定义退款时出错.");
            }
        }

        [Command("masklist")]
        public void COM_ListUnknows(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 1)
                return;

            string list = "<center>在线陌生人</center>";
            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (t.sqlID == 0)
                {
                    list += "<br>ID: " + t.Id + " IP地址: " + t.Ip;
                }
            }

            MainChat.SendInfoChat(p, list);
        }

        [Command("maskhwid")]
        public static async void TaskBAN_Unknows(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 5)
                return;

            if (args.Length <= 0)
                return;

            if (!Int32.TryParse(args[0], out int tid))
                return;

            bool banned = false;
            foreach (PlayerModel tr in Alt.GetAllPlayers())
            {
                if (tr.Id == tid)
                {
                    if (tr.sqlID == 0)
                    {
                        banned = true;
                        await Database.DatabaseMain.AddHwidBan(tr.HardwareIdHash, tr.HardwareIdExHash, p.characterName, "ID 0");
                        tr.Kick("作弊");
                    }
                }
            }

            MainChat.SendAdminChat((banned) ? "ID 0涉嫌作弊被封禁." : "无法执行封禁");
        }

        [Command("tvmask")]
        public void COM_SpecUnknows(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0)
            {
                return;
            }

            if (!Int32.TryParse(args[0], out int tsql))
                return;

            PlayerModel t = GlobalEvents.GetPlayerFromId(tsql);
            if (t == null)
                return;

            if (p.HasSyncedMetaData("isInSpec"))
            {
                p.GetSyncedMetaData<Position>("isInSpec", out Position _oldPos);
                p.EmitAsync("Spec:OFF", _oldPos);
                //p.Position = _oldPos;
                p.Spawn(_oldPos, 0);

                p.Dimension = 0;
                p.showSqlId = true;
                p.fakeName = p.characterName;
                p.Dimension = 0;
                p.DeleteSyncedMetaData("isInSpec");
                p.Visible = true;
                return;
            }

            p.SetSyncedMetaData("isInSpec", p.Position);
            Position _specPos = t.Position;
            _specPos.Z -= 3;
            p.showSqlId = false;
            p.fakeName = "";
            p.Position = _specPos;
            p.Dimension = t.Dimension;
            p.Visible = false;
            p.EmitAsync("Spec:ON", t.Id);
            MainChat.SendInfoChat(p, "[=] " + t.characterName.Replace("_", " ") + " 被您监视中!");

        }

        [Command("ptohouse")]
        public static async void COM_SendPlayerToHouse(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 4)
            {
                MainChat.SendErrorChat(p, "[错误] 无权限操作!");
                return;
            }
            if (!Int32.TryParse(args[0], out int tSql) || !Int32.TryParse(args[1], out int houseId))
            {
                MainChat.SendInfoChat(p, "[用法] /ptohouse [玩家ID] [id]");
                return;
            }

            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (target == null)
            {
                MainChat.SendErrorChat(p, "[错误] 无效玩家.");
                return;
            }

            var house = await Props.Houses.getHouseById(houseId);
            if (house.Item1 == null)
            {
                MainChat.SendErrorChat(p, "[错误] 指定房屋无坐标.");
                return;
            }

            target.Position = house.Item1.pos;
            target.Dimension = 0;

            MainChat.SendInfoChat(p, "[!] 您将 " + target.characterName.Replace('_', ' ') + " 传送至房屋 " + house.Item1.ID);
            MainChat.SendInfoChat(target, "[!] " + p.characterName.Replace('_', ' ') + " 将您传送至房屋 " + house.Item1.ID);
        }

        [Command("igonder")]
        public static async void COM_SendPlayerToBusiness(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 4)
            {
                MainChat.SendErrorChat(p, "[错误] 无权限操作!");
                return;
            }
            if (!Int32.TryParse(args[0], out int tSql) || !Int32.TryParse(args[1], out int houseId))
            {
                MainChat.SendInfoChat(p, "[用法] /igonder [玩家ID] [id]");
                return;
            }

            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (target == null)
            {
                MainChat.SendErrorChat(p, "[错误] 无效玩家.");
                return;
            }

            var house = await Props.Business.getBusinessById(houseId);
            if (house.Item1 == null)
            {
                MainChat.SendErrorChat(p, "[错误] Girilen ID'de bir işyeri bulunamadı.");
                return;
            }

            target.Position = house.Item1.position;
            target.Dimension = 0;

            MainChat.SendInfoChat(p, "[!] " + target.characterName.Replace('_', ' ') + " isimli oyuncu " + house.Item1.ID + " Idli işyerine yollandı.");
            MainChat.SendInfoChat(target, "[!] " + p.characterName.Replace('_', ' ') + " isimli yönetici sizi " + house.Item1.ID + " Idli işyerine yolladı.");
        }

        [Command("psendtocar")]
        public void COM_SendPlayerToCar(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 2)
            {
                MainChat.SendErrorChat(p, "[错误] 无权限操作!");
                return;
            }
            if (!Int32.TryParse(args[0], out int tSql) || !Int32.TryParse(args[1], out int houseId))
            {
                MainChat.SendInfoChat(p, "[用法] /psendtocar [玩家ID] [id]");
                return;
            }

            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (target == null)
            {
                MainChat.SendErrorChat(p, "[错误] 无效玩家.");
                return;
            }

            VehModel veh = Vehicle.VehicleMain.getVehicleFromSqlId(houseId);
            if (veh == null)
            {
                MainChat.SendErrorChat(p, "[错误] 无效车辆.");
                return;
            }

            target.Position = veh.Position;
            target.Dimension = veh.Dimension;

            MainChat.SendInfoChat(p, "[!] " + target.characterName.Replace('_', ' ') + " 被传送至车辆 " + veh.sqlID + " 的坐标.");
            MainChat.SendInfoChat(target, "[!] " + p.characterName.Replace('_', ' ') + " 把您传送至车辆 " + veh.sqlID + " 的坐标.");
        }

        [Command("psend")]
        public void COM_SendPlayerToDestination(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 4)
            {
                MainChat.SendErrorChat(p, "[错误] 无权限操作!");
                return;
            }

            if (!Int32.TryParse(args[0], out int tsql)) { MainChat.SendInfoChat(p, "[用法] /psend [id] [位置]"); return; }

            Position sendPos = new Position(0, 0, 0);
            switch (args[1])
            {
                case "服装店":
                    sendPos = new Position(-1206f, -804.03955f, 16.238037f);
                    break;

                case "卡车":
                    sendPos = new Position(-309.9956f, -2588.0044f, 5.993408f);
                    break;

                case "钓鱼":
                    sendPos = new Position(-1843.7142f, -1223.0637f, 13.00293f);
                    break;

                case "pd":
                    sendPos = new Position(414.03955f, -977.94727f, 29.448364f);
                    break;

                case "mpark":
                    sendPos = new Position(1079.6967f, -529.5692f, 62.524536f);
                    break;

                case "pmhc":
                    sendPos = new Position(294.72528f, -583.4374f, 43.164062f);
                    break;

                case "wnews":
                    sendPos = new Position(-621.45496f, -947.8549f, 21.697388f);
                    break;

                case "paletobay":
                    sendPos = new Position(-98.28132f, 6296.3604f, 31.453491f);
                    break;
            }

            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(tsql);
            if (target == null || !target.Exists)
            {
                MainChat.SendErrorChat(p, "[错误] 无效玩家.");
                return;
            }

            target.Position = sendPos;
            MainChat.SendInfoChat(p, "[!] " + target.characterName.Replace('_', ' ') + " 被传送至 " + args[1]);
            MainChat.SendInfoChat(target, "[!] " + p.characterName.Replace('_', ' ') + " 把您传送至 " + args[1]);
        }

        [Command("showguns")]
        public void COM_ShowTargetWeapons(PlayerModel p, params string[] vs)
        {
            if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, "[错误] 无权限操作!"); return; }
            if (!Int32.TryParse(vs[0], out int tSql)) { MainChat.SendInfoChat(p, "[用法] /showguns [id]"); return; }
            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (target == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家!"); return; }
            MainChat.SendInfoChat(p, "<center>" + target.characterName.Replace('_', ' ') + " 的武器列表:</center><br>" +
                "近战武器: " + ((WeaponModel)target.melee.WeaponHash).ToString() + "<br>" +
                "主武器: " + ((WeaponModel)target.primary.WeaponHash).ToString() + "<br>" +
                "副武器: " + ((WeaponModel)target.secondary.WeaponHash).ToString());
        }


        [Command("testsiren")]
        public void COM_TestSiren(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 7) { MainChat.SendErrorChat(p, "[错误] 无权限操作!"); return; }
            var veh = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (veh == null)
                return;

            Alt.Emit("sirenMastery:setSirens", veh);
        }

        [Command("driftmode")]
        public void COM_lelorozel(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 7) { MainChat.SendErrorChat(p, "[错误] 无权限操作!"); return; }
            var veh = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (veh == null)
                return;

            if (veh.HasStreamSyncedMetaData("DriftMode"))
            {
                veh.DeleteStreamSyncedMetaData("DriftMode");
                MainChat.SendErrorChat(p, "成功取消这辆车的飙车模式.");
            }
            else
            {
                veh.SetStreamSyncedMetaData("DriftMode", true);
                MainChat.SendErrorChat(p, "成功开启这辆车的飙车模式.");
            }
        }

        [Command("driftmode2")]
        public void COM_lelorozeliki(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 7) { MainChat.SendErrorChat(p, "[错误] 无权限操作!"); return; }
            var veh = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (veh == null)
                return;

            if (veh.HasStreamSyncedMetaData("DriftMode2"))
            {
                veh.DeleteStreamSyncedMetaData("DriftMode2");
                MainChat.SendErrorChat(p, "成功取消这辆车的飙车模式.");
            }
            else
            {
                veh.SetStreamSyncedMetaData("DriftMode2", true);
                MainChat.SendErrorChat(p, "成功开启这辆车的飙车模式.");
            }
        }

        [Command("resetrobnpc")]
        public void COM_JackNpcReset(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, "[错误] 无权限操作!"); return; }
            JackingNPC.NpcTimer();
            MainChat.SendInfoChat(p, "成功重置抢劫NPC");
        }

        [Command("btest")]
        public void COM_leloTEstAbe(PlayerModel p, string str)
        {
            if (str == "a")
            {
                Alt.Emit("Basket:AddPlayer", 1, p.Id, 1);
            }
            else
            {
                Alt.Emit("Basket:AddPlayer", 1, p.Id, 2);
            }
            MainChat.SendInfoChat(p, "篮球测试");
        }

        [Command("showhouse")]
        public static async void COM_CheckPlayerHouses(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, "[错误] 无权限操作."); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /showhouse [id]"); return; }
            if (!Int32.TryParse(args[0], out int targetId)) { MainChat.SendInfoChat(p, "[用法] /showhouse [id]"); return; }

            var houses = await Database.DatabaseMain.getPlayerHouses(targetId);
            MainChat.SendInfoChat(p, "[ID " + targetId + " 的房屋]");
            string text = "";
            if (houses.Count > 0)
            {
                foreach (var h in houses)
                {
                    text += "ID: " + h.ID + " - 价格: " + h.price;
                }

                MainChat.SendInfoChat(p, text);
            }
        }

        [Command("showbiz")]
        public static async void COM_CheckPlayerBizs(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, "[错误] 无权限操作."); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /aigor [id]"); return; }
            if (!Int32.TryParse(args[0], out int targetId)) { MainChat.SendInfoChat(p, "[用法] /showbiz [id]"); return; }

            var houses = await Database.DatabaseMain.GetMemberBusinessList(targetId);
            MainChat.SendInfoChat(p, "[ID " + targetId + " 的产业]");
            string text = "";
            if (houses.Count > 0)
            {
                foreach (var h in houses)
                {
                    text += "ID: " + h.ID + " - 价格: " + h.price;
                }

                MainChat.SendInfoChat(p, text);
            }
        }

        [Command("resetprotax")]
        public static async void COM_ResetAllTax(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 7) { MainChat.SendErrorChat(p, "[错误] 无权限操作!"); return; }
            List<BusinessModel> serverBusiness = await Database.DatabaseMain.GetAllServerBusiness();
            foreach (var biz in serverBusiness)
            {

                var b = await Props.Business.getBusinessById(biz.ID);
                b.Item1.settings.TotalTax = 0;
                await b.Item1.Update(b.Item2, b.Item3);
            }

            List<HouseModel> houses = await Database.DatabaseMain.GetAllServerHouses();
            foreach (var ho in houses)
            {

                var house = await Props.Houses.getHouseById(ho.ID);
                house.Item1.settings.TotalTax = 0;
                house.Item1.Update(house.Item3, house.Item2);
            }

            MainChat.SendInfoChat(p, "[已操作完成]");
        }

        [Command("addcardrug")]
        public void COM_AddVehicleDrug(PlayerModel p, string[] args)
        {
            if (p.adminLevel <= 5) { MainChat.SendErrorChat(p, "[错误] 无权操作."); return; }
            if (args.Length < 3) { MainChat.SendInfoChat(p, "[用法] /addcardrug [类型] [品质 (0-100)] [名称] [数量]"); return; }
            if (!Int32.TryParse(args[0], out int type) || !Int32.TryParse(args[1], out int quality) || !Int32.TryParse(args[2], out int amount)) { MainChat.SendInfoChat(p, "[用法] /addcardrug [类型] [品质 (0-100)] [名称] [数量]"); return; }

            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v == null) { MainChat.SendErrorChat(p, "[错误] 附近没有车辆."); return; }

            var inv = JsonConvert.DeserializeObject<List<ServerItems>>(v.vehInv);
            ServerItems item = new ServerItems();

            switch (type)
            {
                case 35:
                    item = Items._LSCitems.Find(x => x.ID == 35);
                    break;

                case 41:
                    item = Items._LSCitems.Find(x => x.ID == 41);
                    break;

                case 42:
                    item = Items._LSCitems.Find(x => x.ID == 42);
                    break;

                case 43:
                    item = Items._LSCitems.Find(x => x.ID == 43);
                    break;

                case 44:
                    item = Items._LSCitems.Find(x => x.ID == 44);
                    break;

                case 45:
                    item = Items._LSCitems.Find(x => x.ID == 45);
                    break;
            }

            var _i = new ServerItems();
            _i.ID = item.ID;
            _i.name = args[3];
            _i.type = item.type;
            _i.selectID = item.selectID;
            _i.picture = item.picture;
            _i.weight = item.weight;
            _i.data = "0";
            _i.data2 = JsonConvert.SerializeObject(new DrugModel
            {
                infection = 1.0,
                quality = quality,
                type = 1
            });
            _i.stackable = true;
            _i.equipable = item.equipable;
            _i.equipSlot = item.equipSlot;
            _i.amount = amount;
            _i.objectModel = item.objectModel;
            _i.isNameChange = false;
            inv.Add(_i);

            v.vehInv = JsonConvert.SerializeObject(inv);
            v.Update();

            MainChat.SendInfoChat(p, "[?] 成功添加 " + amount + " 数量的 " + _i.name + " (" + item.name + ") 至此车辆.");
        }

        [Command("checkhouse")]
        public static async void COM_CheckHouse(PlayerModel p, string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /checkhouse [id]"); return; }
            if (!Int32.TryParse(args[0], out int ID)) { MainChat.SendInfoChat(p, "[用法] /checkhouse [id]"); return; }
            var house = await Props.Houses.getHouseById(ID);
            if (house.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效房屋."); return; }

            MainChat.SendInfoChat(p, "[查看房屋 (ID: " + ID + ") - " + house.Item1.name + "]");
            var owner = await Database.DatabaseMain.getCharacterInfo(house.Item1.ownerId);
            string ownerName = "无";
            if (owner != null) { ownerName = owner.characterName.Replace('_', ' '); }
            MainChat.SendInfoChat(p, "主人: " + ownerName + " 价格:" + house.Item1.price + " 总计税: " + house.Item1.settings.TotalTax + " 税收: " + house.Item1.settings.Tax);
            MainChat.SendInfoChat(p, "租户: " + house.Item1.rentOwner + " 租价: " + house.Item1.rentPrice + " 可出租: " + ((house.Item1.isRentable) ? "是" : "否"));
            MainChat.SendInfoChat(p, "上锁: " + ((house.Item1.isLocked) ? "是" : "否"));
        }

        [Command("checkbiz")]
        public static async void COM_CheckBiz(PlayerModel p, string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /checkbiz [id]"); return; }
            if (!Int32.TryParse(args[0], out int ID)) { MainChat.SendInfoChat(p, "[用法] /checkbiz [id]"); return; }
            var house = await Props.Business.getBusinessById(ID);
            if (house.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效产业."); return; }

            MainChat.SendInfoChat(p, "[查看产业 (ID: " + ID + ") - " + house.Item1.name + "]");
            var owner = await Database.DatabaseMain.getCharacterInfo(house.Item1.ownerId);
            string ownerName = "无";
            if (owner != null) { ownerName = owner.characterName.Replace('_', ' '); }
            MainChat.SendInfoChat(p, "主人: " + ownerName + " 价格:" + house.Item1.price + " 总计税: " + house.Item1.settings.TotalTax + " 税收: " + house.Item1.settings.Tax);
            MainChat.SendInfoChat(p, "类型: " + house.Item1.type + " 收入:" + house.Item1.vault);
            MainChat.SendInfoChat(p, "上锁: " + ((house.Item1.isLocked) ? "是" : "否"));
        }

        [Command("closedprops")]
        public static async void COM_LockedProps(PlayerModel p, string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /closedprops [veh/house/biz]"); return; }
            if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }

            string text = "<hr>";
            switch (args[0])
            {
                case "veh":
                    foreach (VehModel v in Alt.GetAllVehicles())
                    {
                        if (v.fine > (v.price / 10))
                        {
                            text += "<br> ID: " + v.sqlID + " | 税: " + v.fine + "/" + (v.price / 10);
                        }
                    }
                    break;

                case "biz":
                    List<BusinessModel> serverBusiness = await Database.DatabaseMain.GetAllServerBusiness();
                    foreach (var biz in serverBusiness)
                    {
                        if (biz.settings.TotalTax > (biz.price / 10))
                        {
                            text += "<br> ID: " + biz.ID + " | 税: " + biz.settings.TotalTax + "/" + (biz.price / 10);
                        }
                    }
                    break;

                case "house":
                    List<HouseModel> houses = await Database.DatabaseMain.GetAllServerHouses();
                    foreach (var ho in houses)
                    {
                        if (ho.settings.TotalTax > (ho.price / 10))
                        {
                            text += "<br> ID: " + ho.ID + " | 税: " + ho.settings.TotalTax + "/" + (ho.price / 10);
                        }
                    }
                    break;
                default: MainChat.SendInfoChat(p, "[用法] /closedprops [veh/house/biz]"); return;

            }

            MainChat.SendInfoChat(p, "[" + args[0] + " 强行关闭列表]");
            MainChat.SendInfoChat(p, text);
        }

        [Command("showjails")]
        public void COM_ShowJails(PlayerModel p, string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, "[错误] 无权限操作!"); return; }
            string text = "<center>监狱玩家</center>";
            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (t.jailTime > 0)
                {
                    text += "<br> " + t.characterName.Replace('_', ' ') + " # 时间: " + t.jailTime + "分钟";
                }
            }

            MainChat.SendInfoChat(p, text);
        }

        [Command("showajails")]
        public void COM_ShowadminJails(PlayerModel p, string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, "[错误] 无权限操作!"); return; }
            string text = "<center>OOC关押列表</center>";
            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (t.adminJail > 0)
                {
                    text += "<br> " + t.characterName.Replace('_', ' ') + " # 时间: " + t.adminJail + "分钟";
                }
            }

            MainChat.SendInfoChat(p, text);
        }

        [Command("closeserver")]
        public static async void COM_CloseServer(PlayerModel pl, string[] args)
        {
            if (pl.adminLevel <= 6) { MainChat.SendErrorChat(pl, CONSTANT.ERR_PermissionError); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(pl, "[用法] /closeserver [原因]"); return; }

            foreach (PlayerModel p in Alt.GetAllPlayers())
            {
                await p.updateSql();
            }
            foreach (VehModel v in Alt.GetAllVehicles())
            {
                v.Update();
            }
            await Database.DatabaseMain.SaveServerSettings();
            Alt.Log("数据已保存.");
            //Alt.Log("Karakter bilgileri kaydedildi.");
            //Alt.Log("OutRp Kapatıldı");
            //Alt.Log("Çiftlik bilgileri kaydedildi.");
            //Alt.Log("Market sistemi kaydedildi.");
            await Database.DatabaseMain.SaveServerSettings();



            string reason = string.Join(" ", args);
            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (t == pl) { continue; }
                await t.KickAsync(reason);
            }
            MainChat.SendInfoChat(pl, "[- -] 已将所有在线玩家踢出服务器.");


            Alt.Log("服务器关闭 /closeserver .");
        }

        [Command("resetjail")]
        public static async void COM_ResetJail(PlayerModel p, string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, "[错误] 无权限操作!"); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /resetjail [玩家ID]"); return; }
            if (!Int32.TryParse(args[0], out int tSql)) { MainChat.SendErrorChat(p, "[错误] 玩家ID必须是数字!"); return; }
            var target = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (target == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家."); return; }

            target.jailTime = 0;
            await target.updateSql();
            MainChat.SendInfoChat(p, "[- -] " + target.characterName + " 的IC监狱时间重置了.");
            MainChat.SendInfoChat(target, "[- -] 您角色的 IC 监狱时间重置了.");
        }

        [Command("resetajail")]
        public static async void COM_ResetAJail(PlayerModel p, string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, "[错误] 无权限操作!"); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /resetjail [玩家ID]"); return; }
            if (!Int32.TryParse(args[0], out int tSql)) { MainChat.SendErrorChat(p, "[错误] 玩家ID必须是数字!"); return; }
            var target = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (target == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家."); return; }

            target.adminJail = 0;
            await target.updateSql();
            MainChat.SendInfoChat(p, "[- -] " + target.characterName + " 的OOC监狱时间重置了.");
            MainChat.SendInfoChat(target, "[- -] 您角色的 OOC 监狱时间重置了.");
        }

        [Command("sellclosed")]
        public static async void COM_SellTaxed(PlayerModel p, string[] args)
        {
            try
            {
                if (p.adminLevel < 8) { MainChat.SendErrorChat(p, "[错误] 无权限操作!"); return; }

                MainChat.SendInfoChat(p, "[进程开始]");
                int totalBiz = 0;
                int totalHouse = 0;
                int totalVehicle = 0;

                var bizs = await Database.DatabaseMain.GetAllServerBusiness();
                foreach (var b in bizs)
                {
                    if (b.ownerId <= 1) { continue; }

                    var biz = await Props.Business.getBusinessById(b.ID);

                    if (biz.Item1 == null) { continue; }

                    if (biz.Item1.settings.TotalTax > (biz.Item1.price))
                    {
                        ++totalBiz;
                        biz.Item1.ownerId = 0;
                        biz.Item1.name = "出售";
                        biz.Item1.settings.Env = "[]";
                        biz.Item1.isLocked = true;
                        biz.Item1.entrancePrice = 0;
                        biz.Item1.settings.Admins = new();
                        await biz.Item1.Update(biz.Item2, biz.Item3);
                        //await Database.DatabaseMain.updateBusinessKeys(biz.Item1.ID, new List<Database.DatabaseMain.KeyModel>());
                        //await Database.DatabaseMain.ResetBusinessStafs(biz.Item1.ID);
                    }
                }

                var houses = await Database.DatabaseMain.GetAllServerHouses();
                foreach (var h in houses)
                {
                    if (h.ownerId <= 1) { continue; }

                    var house = await Props.Houses.getHouseById(h.ID);

                    if (house.Item1 == null) { continue; }

                    if (house.Item1.settings.TotalTax > (house.Item1.price))
                    {
                        ++totalHouse;
                        house.Item1.ownerId = 0;
                        house.Item1.name = "出售";
                        house.Item1.houseEnv = "[]";
                        house.Item1.isLocked = true;
                        house.Item1.Update(house.Item3, house.Item2);
                        await Database.DatabaseMain.updateHouseKeys(house.Item1.ID, new List<Database.DatabaseMain.KeyModel>());
                    }
                }

                foreach (VehModel veh in Alt.GetAllVehicles())
                {
                    if (veh.owner <= 1) { continue; }

                    if (veh.fine > (veh.price + (veh.price / 2)))
                    {
                        ++totalVehicle;
                        await Database.DatabaseMain.DeleteVehicle(veh);
                        veh.Remove();
                    }
                }

                MainChat.SendAdminChat("[房屋清理] 总计: " + totalBiz + " 个产业, " + totalHouse + " 个房屋, " + totalVehicle + " 个车辆被 系统 清理为出售状态(总计税大于房屋价格).");
            }
            catch (Exception ex)
            {
                MainChat.SendErrorChat(p, "[错误] " + ex.Message);
            }
        }

        [Command("flatperm")]
        public static async void COM_FlatPerm(PlayerModel p, string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, "[错误] 无权限操作!"); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /flatperm [玩家ID]"); return; }
            if (!Int32.TryParse(args[0], out int tSql)) { MainChat.SendErrorChat(p, "[错误] 玩家ID必须是数字!"); return; }
            var target = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (target == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家."); return; }

            var settings = JsonConvert.DeserializeObject<CharacterSettings>(target.settings);
            settings.hasFlatbed = !settings.hasFlatbed;
            target.settings = JsonConvert.SerializeObject(settings);
            await target.updateSql();
            MainChat.SendInfoChat(p, "[- -] " + target.characterName + " 的 FlatBed 权限被更新为: " + (settings.hasFlatbed ? "积极" : "关闭"));
            MainChat.SendInfoChat(target, "[- -] 您的 FlatBed 权限被更新为: " + (settings.hasFlatbed ? "积极" : "关闭"));
        }

        [Command("setsell")]
        public static async void COM_MakeGallery(PlayerModel p, string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, "[错误] 无权限操作!"); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /setsell [玩家ID]"); return; }
            if (!Int32.TryParse(args[0], out int tSql)) { MainChat.SendErrorChat(p, "[错误] 玩家ID必须是数字!"); return; }
            var target = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (target == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家."); return; }

            var settings = JsonConvert.DeserializeObject<CharacterSettings>(target.settings);
            settings.hasGallery = !settings.hasGallery;
            target.settings = JsonConvert.SerializeObject(settings);
            await target.updateSql();
            MainChat.SendInfoChat(p, "[- -] " + target.characterName + " 的 挂牌出售 权限被更新为: " + (settings.hasGallery ? "积极" : "关闭"));
            MainChat.SendInfoChat(target, "[- -] 您的 挂牌出售 权限被更新为: " + (settings.hasGallery ? "积极" : "关闭"));
        }

        [Command("setgraffiti")]
        public static async void COM_SetGraffiti(PlayerModel p, string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, "[错误] 无权限操作!"); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /setgraffiti [玩家ID]"); return; }
            if (!Int32.TryParse(args[0], out int tSql)) { MainChat.SendErrorChat(p, "[错误] 玩家ID必须是数字!"); return; }
            var target = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (target == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家."); return; }

            target.isGraffiti = !target.isGraffiti;
            await target.updateSql();
            MainChat.SendInfoChat(p, "[- -] " + target.characterName + " 的 涂鸦 权限被更新为: " + (target.isGraffiti ? "积极" : "关闭"));
            MainChat.SendInfoChat(target, "[- -] 您的 涂鸦 权限被更新为: " + (target.isGraffiti ? "积极" : "关闭"));
        }
        
        [Command("degraff")]
        public void COM_DeleteGraffiti(PlayerModel p, string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, "[错误] 无权限操作!"); return; }
            p.EmitLocked("graffitix:ungraffitix");
        } 
    }
}

