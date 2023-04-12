using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Core;
using outRp.Globals;
using outRp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using outRp.OtherSystem;
using outRp.Tutorial;
using AltV.Net.Resources.Chat.Api;

namespace outRp.Vehicle
{
    public class VehicleMain : IScript
    {
        public static void VehicleSystems()
        {
            //Alt.OnClient<PlayerModel, VehModel, float>("UpdateVehKM", UpdateVehKM);
            //Alt.OnClient<PlayerModel, VehModel, float>("Vehicle:FuelOver", VehFuelOver);
        }
        public class VehicleSellModel
        {
            public int senderSqlId { get; set; }
            public int targetSqlId { get; set; }
            public int vehicleSqlId { get; set; }
            public int price { get; set; }
        }

        public static void VehicleMakeSettings(VehModel v)
        {
            /*v.NeonColor = v.settings.NeonColor;
            v.SetWheelsAsync((byte)v.settings.Wheel, (byte)v.settings.Wheel);
            v.SetWheelColorAsync((byte)v.settings.WheelColor);

            v.SetModKitAsync((byte)1);
            foreach(ModifySettings set in v.settings.modifySettings)
            {
                v.SetModAsync((byte)set.category, (byte)set.value);
            }

            return; */
            v.NeonColor = v.settings.NeonColor;
            v.AppearanceData = v.settings.ModifiyData;
            return;
        }

        public static List<VehicleSellModel> sellingVehicles = new List<VehicleSellModel>();

        [AsyncClientEvent("Vehicle:FuelOver")]
        public static void VehFuelOver(PlayerModel p, VehModel v, float distance)
        {
            try
            {
                if (v.Driver == p)
                {
                    v.km += distance / 1000;
                    v.currentFuel = 0;
                    v.EngineOn = false;
                    if (v.jobId < 0)
                    {
                        v.Update();
                        MainChat.SendErrorChat(p, "[!] 车没油了.");
                    }
                }
            }
            catch
            {
            }

        }


        [AsyncClientEvent("UpdateVehKM")]
        public static void UpdateVehKM(PlayerModel p, VehModel v, float distance)
        {
            try
            {
                if (v.Driver == p || v.Driver == null)
                {
                    v.km += (double)(distance / 1000);
                    v.currentFuel -= (int)((distance / 2000) * v.fuelConsumption);
                    if (v.jobId > 0)
                    {
                        return;
                    }


                    int addDirt = (int)((distance / 500) / 5);
                    if (addDirt > 2)
                    {
                        if (v.DirtLevel + addDirt > 14)
                        {
                            v.DirtLevel = 14;
                        }
                        else
                        {
                            v.DirtLevel += (byte)1;
                        }
                    }

                    v.Update();
                    return;
                }

            }
            catch { return; }
        }
        public static async Task<bool> GetKeysQuery(PlayerModel player, VehModel veh)
        {
            if (player.sqlID == veh.owner) { return true; }
            if (player.adminWork == true) { return true; }
            if (veh.settings.PDLock && await Globals.System.PD.CheckPlayerInPd(player)) { return true; }
            if (veh.settings.PDLock) { return false; }
            if (veh.factionId == player.factionId)
            {
                FactionModel fact = await Database.DatabaseMain.GetFactionInfo(player.factionId);
                var rank = fact.rank.Find(x => x.Rank == player.factionRank);
                if (rank != null)
                {
                    if (rank.permission.canUseCar)
                    {
                        return true;
                    }
                }
            }
            if (veh.rentOwner == player.sqlID) { return true; }
            if (veh.businessId == player.businessStaff && veh.businessId > 0) { return true; }
            if (veh.HasData(EntityData.VehicleEntityData.VehicleTempOwner)) { veh.GetData<int>(EntityData.VehicleEntityData.VehicleTempOwner, out int TempOwner); if (TempOwner == player.sqlID) { return true; } }

            List<Database.DatabaseMain.KeyModel> vehicleKeys = await Database.DatabaseMain.getVehicleKeys(veh.sqlID);

            foreach (Database.DatabaseMain.KeyModel key in vehicleKeys)
            {
                if (key.keyOwner == player.sqlID) { return true; }
            }

            return false;
        }
        public static async void VehicleEngine(PlayerModel player)
        {
            VehModel veh = (VehModel)player.Vehicle;

            if (veh == null) { MainChat.SendErrorChat(player, CONSTANT.COM_VQueryVehNotFound); return; }

            if (veh.Driver != player) { MainChat.SendErrorChat(player, CONSTANT.COM_VQueryVehNotDriver); return; }
            if (veh.HasData("Engine:Busy"))
                return;
            if (veh.EngineOn == true) { veh.EngineOn = false; MainChat.AME(player, CONSTANT.EMOTE_VQueryEngineOff); return; }
            if (veh.EngineOn == false)
            {
                if (veh.towwed == true) { MainChat.SendErrorChat(player, "> 此车被扣押中, 因税问题."); return; }
                if (veh.currentFuel <= 0) { MainChat.SendErrorChat(player, "> 此车没油了."); return; }
                if (await GetKeysQuery(player, veh) == false) { MainChat.SendErrorChat(player, CONSTANT.COM_VQueryVehNotHaveKeys); return; }
                else
                {

                    /*if (veh.km > 1000 && veh.km < 10000)
                    {
                        MainChat.EmoteMe(player, "Araç motorunu çalıştırmayı dener.");
                        veh.SetData("Engine:Busy", true);
                        await Task.Delay((int)veh.km / 2);
                        if (!veh.Exists)
                            return;

                        veh.DeleteData("Engine:Busy");
                        veh.EngineOn = true;
                        MainChat.EmoteDo(player, "Araç motoru çalıştı.");
                        GlobalEvents.NativeNotifyVehicle(veh, "~p~Motor çalıştı");
                        veh.EngineOn = true;
                        veh.SetEngineOnAsync(true);
                        return;
                    }
                    else if (veh.km >= 10000)
                    {
                        MainChat.EmoteMe(player, "Araç motorunu çalıştırmayı dener.");
                        veh.SetData("Engine:Busy", true);
                        Random rnd = new Random();
                        if (rnd.Next(0, 10) > 4)
                        {
                            await Task.Delay((int)veh.km / 2);
                            if (!veh.Exists)
                                return;

                            veh.DeleteData("Engine:Busy");
                            veh.EngineOn = true;
                            MainChat.EmoteDo(player, "Araç motoru çalıştı.");
                            GlobalEvents.NativeNotifyVehicle(veh, "~p~Motor çalıştı");
                            veh.EngineOn = true;
                            veh.SetEngineOnAsync(true);
                            return;
                        }
                        else // Motor çalışmadı.
                        {
                            veh.DeleteData("Engine:Busy");
                            MainChat.EmoteDo(player, "Araç motoru çalışmadı.");
                            GlobalEvents.NativeNotifyVehicle(veh, "~r~Motor çalışmadı.");
                            return;
                        }
                    }
                    else
                    {*/
                    veh.EngineOn = true;
                    MainChat.AME(player, CONSTANT.EMOTE_VQueryEngineOn);
                    GlobalEvents.NativeNotifyVehicle(veh, "~p~发动车辆");
                    veh.EngineOn = true;
                    
                    if (player.isFinishTut == 5)
                    {
                        player.isFinishTut = 6;
                        // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                        MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}我有{fc5e03}车{FFFFFF}了!");
                        MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}您真的很棒, 您成功发动车辆了!");
                        MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}按T输入 {fc5e03}/cstats{FFFFFF} 可以查看车辆信息!");
                        MainChat.SendInfoChat(player, "{fc5e03}新手教程: 查看一下车辆信息吧!");                    
                    }                    
                    return;
                    //}


                }
            }
        }
        public static async void VehicleLock(PlayerModel player, params string[] args)
        {
            VehModel veh = null;
            if (args.Length > 0)
            {
                if (!Int32.TryParse(args[0], out int id)) { return; }
                veh = getVehicleFromSqlId(id);
            }
            else
            {
                veh = getNearVehFromPlayer(player);
            }

            if (veh == null) { MainChat.SendErrorChat(player, "无效车辆或附近没有车辆!"); return; }

            bool keys = await GetKeysQuery(player, veh);

            if (await GetKeysQuery(player, veh) == false) { MainChat.SendErrorChat(player, "您没有此车的钥匙!"); return; }

            if (veh.LockState == VehicleLockState.Unlocked)
            {
                veh.LockState = VehicleLockState.Locked;
                MainChat.AME(player, " 掏出钥匙并锁上了车辆.");
                GlobalEvents.NativeNotifyVehicle(veh, "~r~已锁车");
            }
            else
            {
                veh.LockState = VehicleLockState.Unlocked;
                MainChat.AME(player, " 掏出钥匙并解锁了车辆.");
                GlobalEvents.NativeNotifyVehicle(veh, "~g~已解锁");
            }
        }
        public static void RemoveTempOwner(PlayerModel p, params string[] args)
        {
            foreach (VehModel v in Alt.GetAllVehicles())
            {
                if (v.HasData(EntityData.VehicleEntityData.VehicleTempOwner))
                {
                    if (v.lscGetdata<int>(EntityData.VehicleEntityData.VehicleTempOwner) == p.sqlID)
                    {
                        v.DeleteData(EntityData.VehicleEntityData.VehicleTempOwner);
                    }
                }
            }
            return;
        }

        public static bool CleanVehicle(PlayerModel p)
        {
            if (p.Vehicle != null)
                return false;

            VehModel v = getNearVehFromPlayer(p);
            if (v == null)
                return false;

            if (v.DirtLevel <= 0)
                return false;

            GlobalEvents.ClearVehicleTag(v);
            CleanVehicleAnims(p, v);

            return true;
        }

        private static async void CleanVehicleAnims(PlayerModel p, VehModel v)
        {
            GlobalEvents.SetVehicleTag(v, "~p~清洗中...");
            GlobalEvents.FreezeEntity(p, true);
            OtherSystem.Animations.PlayerAnimation(p, "clean1");
            GlobalEvents.ProgresBar(p, "清洗车辆中...", 19);
            await Task.Delay(20000);
            if (!p.Exists)
                return;

            OtherSystem.Animations.PlayerStopAnimation(p);
            GlobalEvents.FreezeEntity(p, false);
            GlobalEvents.ClearVehicleTag(v);
            await v.SetDirtLevelAsync(0);
            MainChat.SendInfoChat(p, "[!] 已完成车辆清洗.");
            GlobalEvents.NativeNotifyAll(p, "~p~车辆已清洗.");
            return;
        }

        [AsyncClientEvent("Vehicle:CruiseControl")]
        public void VehicleCruiseControl(PlayerModel p, float speed)
        {
            if (p.Vehicle == null)
                return;

            VehModel v = (VehModel)p.Vehicle;
            if (v.HasSyncedMetaData("vehicle:CruiseControl"))
            {
                v.DeleteSyncedMetaData("vehicle:CruiseControl");
            }
            else
            {
                v.SetSyncedMetaData("vehicle:CruiseControl", speed);
            }

        }

        /*
        public static VehModel getNearVehFromPlayer(PlayerModel player)
        {
            VehModel veh = null;
            float lastDist = 999;
            if(player.Vehicle != null) { VehModel nVeh = (VehModel)player.Vehicle; return nVeh; }
            foreach(VehModel v in Alt.GetAllVehicles())
            {
                float currDist = v.Position.Distance(player.Position);
                if (currDist < 4)
                {
                    if(currDist < lastDist)
                    {
                        lastDist = currDist;
                        veh = v;
                    }                    
                }
                
            }
            return veh;
        }*/

        public static VehModel getNearVehFromPlayer(PlayerModel p)
        {
            return (VehModel)Alt.GetAllVehicles().Where(x => x.Position.Distance(p.Position) < 4 && x.Dimension == p.Dimension).OrderBy(x => x.Position.Distance(p.Position)).FirstOrDefault();
        }
        public static VehModel? getVehicleFromSqlId(int SqlID)
        {
            foreach (VehModel v in Alt.GetAllVehicles())
            {
                if (v.sqlID == SqlID)
                {
                    return v;
                }
            }
            return null;
        }

        public static VehModel getVehicleFromPlate(string plate)
        {
            VehModel veh = null;
            foreach (VehModel v in Alt.GetAllVehicles())
            {
                if (v.NumberplateText == plate)
                {
                    veh = v;
                    break;
                }
            }

            return veh;
        }
        [Command(CONSTANT.COM_VehEngine)]
        [AsyncClientEvent("Player:VehicleEngineKey")]
        public static void COM_VehEngine(PlayerModel player)
        {
            if (player.Ping > 250)
                return;
            VehicleMain.VehicleEngine(player);
        }

        [Command(CONSTANT.COM_VehLock)]
        [AsyncClientEvent("Player:VehicleLockKey")]
        public static async void COM_VehLock(PlayerModel player)
        {
            if (player.Ping > 250)
                return;
            VehicleMain.VehicleLock(player);
        }

        [Command(CONSTANT.COM_AddVehFactionOrBusiness)]
        public static async void COM_AddVehFOrV(PlayerModel player, params string[] values)
        {
            if (values.Length <= 0) { MainChat.SendInfoChat(player, CONSTANT.DESC_AddVehFactionOrBusiness); return; }
            IVehicle veh = player.Vehicle;
            VehModel v = (VehModel)veh;
            if (v == null) { MainChat.SendErrorChat(player, CONSTANT.COM_VQueryVehNotDriver); return; }
            switch (values[0])
            {
                case "biz":
                    if (v.owner != player.sqlID) { MainChat.SendErrorChat(player, CONSTANT.COM_VQueryVehNotHaveKeys); return; }
                    BusinessModel biz = await Database.DatabaseMain.GetBusinessInfo(Int32.Parse(values[1].ToString()));
                    if (biz == null)
                        return;

                    if (biz.ownerId != player.sqlID) { MainChat.SendErrorChat(player, CONSTANT.ERR_BusinessOwnerNot); return; }
                    v.businessId = biz.ID;
                    if (biz.type == 1)
                    {
                        v.jobId = 1;
                    }
                    string case1Msg = "编号为 " + v.sqlID.ToString() + " 的车辆已经注册至产业 " + biz.name;
                    v.Update();
                    MainChat.SendInfoChat(player, case1Msg);
                    break;
                case "faction":
                    if (v.owner != player.sqlID) { MainChat.SendErrorChat(player, CONSTANT.COM_VQueryVehNotHaveKeys); return; }
                    if (player.factionId <= 0) { MainChat.SendErrorChat(player, CONSTANT.ERR_FNotFound); return; }
                    v.factionId = player.factionId;
                    string case2Msg = "编号为 " + v.sqlID.ToString() + " 的车辆已经注册至组织 " + player.factionId.ToString();
                    v.Update();
                    MainChat.SendInfoChat(player, case2Msg);
                    break;
                default:
                    MainChat.SendInfoChat(player, CONSTANT.DESC_AddVehFactionOrBusiness);
                    break;
            }
        }

        [Command(CONSTANT.COM_RemoveVehicleAll)]
        public static void COM_RemoveVehicleAll(PlayerModel player)
        {
            IVehicle veh = player.Vehicle;
            VehModel v = (VehModel)veh;
            if (v == null) { MainChat.SendErrorChat(player, CONSTANT.COM_VQueryVehNotFound); return; }
            if (v.owner != player.sqlID) { MainChat.SendErrorChat(player, CONSTANT.COM_VQueryVehNotOwnYou); return; }
            v.businessId = -1;
            v.factionId = -1;
            v.jobId = 0;
            v.Update();
            MainChat.SendInfoChat(player, string.Format(CONSTANT.INFO_RemoveVehicleAllSucces, v.sqlID));
        }

        [Command(CONSTANT.COM_VehRent)]
        public static async void COM_VehRent(PlayerModel player, params string[] args)
        {
            /*try
            {
                if (args.Length <= 0) { MainChat.SendInfoChat(player, "[用法] /akirala [süre(Dakika)]"); return; }
                int renttime; bool isOK = Int32.TryParse(args[0], out renttime);
                if (!isOK) { MainChat.SendInfoChat(player, "[用法] /akirala [süre(Dakika)]"); return; }
                if (renttime <= 0) { MainChat.SendInfoChat(player, CONSTANT.DESC_VehRent); return; }
                if (!player.IsInVehicle) { MainChat.SendErrorChat(player, CONSTANT.COM_VQueryVehNotFound); return; }
                IVehicle v = player.Vehicle;
                VehModel veh = (VehModel)v;
                if(veh.jobId != 1) { MainChat.SendErrorChat(player, "[错误] Bu araç kiralık değil."); return; }
                FactionModel fact = Database.DatabaseMain.GetFactionInfo(veh.factionId);

                if (veh.rentOwner != 0) { MainChat.SendErrorChat(player, CONSTANT.COM_RentAlreadyExist); return; }
                if (player.cash < (renttime * 5)) { MainChat.SendErrorChat(player, CONSTANT.ERR_MoneyNotEnought); return; }
                BusinessModel carBusiness = Database.DatabaseMain.GetBusinessInfo(veh.businessId);
                if (carBusiness.type == 1 || veh.jobId == ServerGlobalValues.JOB_RentalCar)
                {
                    veh.rentOwner = player.sqlID;
                    if(veh.jobId != 4)
                    {
                        player.cash -= (renttime * 5);
                    }

                    if (veh.businessId != 0)
                    {
                        (BusinessModel, Marker, PlayerLabel) biz = Props.Business.getBusinessById(veh.businessId);

                        biz.Item1.vault += (renttime * 5);
                        biz.Item1.Update(biz.Item2, biz.Item3);
                    }
                    MainChat.SendInfoChat(player, "Araç kiraladınız. Belge NO: " + veh.sqlID.ToString());
                    int time = renttime * 60000;

                    await Task.Delay(time);

                    if (player.Exists)
                    {
                        MainChat.SendInfoChat(player, CONSTANT.COM_RentOver);
                    }

                    await veh.SetEngineOnAsync(false);
                    await Task.Delay(5000);

                    if (veh.Driver != null) { veh.Driver.Position = veh.Position; }
                    veh.rentOwner = 0;
                    veh.LockState = VehicleLockState.Unlocked;

                    veh.Position = veh.settings.savePosition;
                    veh.Rotation = veh.settings.saveRotation;
                    veh.DamageData = "QA==";                    
                }
                else { MainChat.SendErrorChat(player, CONSTANT.COM_RentVehNotRentable); return; }
            }
            catch { return; }*/
            try
            {
                if (args.Length <= 0) { MainChat.SendInfoChat(player, "[用法] /rentcar [时间(分钟)]"); return; }
                if (!Int32.TryParse(args[0], out int RentTime)) { MainChat.SendInfoChat(player, "[用法] /rentcar [时间(分钟)]"); return; }
                if (RentTime < 5) { MainChat.SendErrorChat(player, "[错误] 无效租用时间, 最少为 5 分钟"); return; }
                if (player.Vehicle == null) { MainChat.SendErrorChat(player, "[错误] 您必须在车内使用."); return; }
                VehModel v = (VehModel)player.Vehicle;
                if (v.jobId != 1)
                {
                    if (v.businessId == 0) { MainChat.SendErrorChat(player, "[错误] 此车不可租用."); return; }
                    var biz = await Props.Business.getBusinessById(v.businessId);
                    if (biz.Item1.type != ServerGlobalValues.rentalCar) { MainChat.SendErrorChat(player, "[错误] 此车不可租用."); return; }
                }

                if (v.HasData(EntityData.VehicleEntityData.VehicleTempOwner) && v.HasData(EntityData.VehicleEntityData.VehicleRentTime))
                {
                    if (v.lscGetdata<int>(EntityData.VehicleEntityData.VehicleTempOwner) != player.sqlID)
                    {
                        MainChat.SendErrorChat(player, "[错误] 此车已被租用."); return;
                    }
                }

                if (v.businessId != 0)
                {
                    int price = 0;
                    var biz = await Props.Business.getBusinessById(v.businessId);
                    if (biz.Item1 != null)
                    {
                        price = RentTime * v.settings.RentPrice;
                        if (player.cash < price) { MainChat.SendErrorChat(player, "[错误] 您没有足够的钱租用此车."); return; }

                        biz.Item1.vault += price;
                        await biz.Item1.Update(biz.Item2, biz.Item3);
                        player.cash -= price;
                        await player.updateSql();

                        v.SetData(EntityData.VehicleEntityData.VehicleTempOwner, player.sqlID);
                        v.SetData(EntityData.VehicleEntityData.VehicleRentTime, RentTime);

                    }
                    else
                    {
                        MainChat.SendErrorChat(player, "[错误] 无效车辆产业信息.");
                        return;
                    }
                }
                else
                {
                    if (player.cash < (RentTime * ServerGlobalValues.CarRentPrice)) { MainChat.SendErrorChat(player, "[错误] 您没有足够的钱租用此车."); return; }
                    player.cash -= (RentTime * ServerGlobalValues.CarRentPrice);
                    await player.updateSql();
                    //Inputs.SendButtonInput(player, "Bu aracı $" + (RentTime * ServerGlobalValues.CarRentPrice) + " karşılığında kiralayacaksınız.", "Vehicle:RentCB", v.sqlID.ToString() + "," + RentTime);
                    v.SetData(EntityData.VehicleEntityData.VehicleTempOwner, player.sqlID);
                    v.SetData(EntityData.VehicleEntityData.VehicleRentTime, RentTime);
                    return;
                }


            }
            catch { /*Console.WriteLine(ex); */return; }
        }

        [AsyncClientEvent("Vehicle:RentCB")]
        public void Event_VehicleRent(PlayerModel p, bool selection, string otherValue)
        {
            string[] val = otherValue.Split(',');
            if (!Int32.TryParse(val[0], out int vehicleID) || !Int32.TryParse(val[1], out int RentTime))
                return;

            if (!selection) { MainChat.SendInfoChat(p, "[?] 您取消了租车."); return; }
            else
            {
                VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(vehicleID);
                if (v == null)
                    return;

                if (v.HasData(EntityData.VehicleEntityData.VehicleTempOwner) || v.HasData(EntityData.VehicleEntityData.VehicleRentTime))
                {
                    if (v.lscGetdata<int>(EntityData.VehicleEntityData.VehicleTempOwner) != p.sqlID)
                    {
                        MainChat.SendErrorChat(p, "[错误] 此车在您租用之前已被租用."); return;
                    }
                }

                if (p.cash < (RentTime * ServerGlobalValues.CarRentPrice)) { MainChat.SendErrorChat(p, "[错误] 您没有足够的钱!"); return; }
                p.cash -= RentTime * ServerGlobalValues.CarRentPrice;
                p.updateSql();
                v.SetData(EntityData.VehicleEntityData.VehicleTempOwner, p.sqlID);
                if (v.HasData(EntityData.VehicleEntityData.VehicleRentTime))
                    v.SetData(EntityData.VehicleEntityData.VehicleRentTime, v.lscGetdata<int>(EntityData.VehicleEntityData.VehicleRentTime) + RentTime);
                else v.SetData(EntityData.VehicleEntityData.VehicleRentTime, RentTime);
                MainChat.SendInfoChat(p, "[?] 您已成功租用此车.");
            }
        }

        public static void Event_Vehicle_RentTimeControl()
        {
            try
            {
                var vehicles = Alt.GetAllVehicles(); // Lock fonksiyonu eklendi.

                foreach (VehModel v in vehicles)
                {
                    if (v.HasData(EntityData.VehicleEntityData.VehicleTempOwner))
                        if (v.HasData(EntityData.VehicleEntityData.VehicleRentTime))
                        {
                            int time = v.lscGetdata<int>(EntityData.VehicleEntityData.VehicleRentTime);
                            time -= 1;
                            v.SetData(EntityData.VehicleEntityData.VehicleRentTime, time);
                            if (time <= 0)
                            {
                                if (v.Driver != null)
                                {
                                    v.Driver.Position = v.Position;
                                }

                                v.SetPositionAsync(v.settings.savePosition);
                                v.SetRotationAsync(v.settings.saveRotation);
                                v.SetDimensionAsync(v.settings.SaveDimension);
                                v.EngineOn = false;
                                v.SetLockStateAsync(VehicleLockState.Unlocked);

                                v.DeleteData(EntityData.VehicleEntityData.VehicleTempOwner);
                                v.DeleteData(EntityData.VehicleEntityData.VehicleRentTime);

                                PlayerModel p = GlobalEvents.GetPlayerFromSqlID(v.lscGetdata<int>(EntityData.VehicleEntityData.VehicleTempOwner));
                                if (p != null) { MainChat.SendInfoChat(p, "[?] 您的租车期限已到期, 系统已收回."); }

                            }
                        }
                }

            }
            catch { /*Console.WriteLine(ex);*/ return; }
        }

        [Command(CONSTANT.COM_VehSave)]
        public static void VehSave(PlayerModel player)
        {
            if (player.adminLevel <= 6) { MainChat.SendErrorChat(player, CONSTANT.COM_VQueryVehNotFound); return; }
            if (player.Vehicle == null) { MainChat.SendErrorChat(player, CONSTANT.COM_VQueryVehNotFound); return; }
            IVehicle v = player.Vehicle;
            VehModel veh = (VehModel)v;
            if (veh.owner != player.sqlID) { MainChat.SendErrorChat(player, "[错误] 此车不属于您."); return; }
            veh.settings.savePosition = veh.Position;
            veh.settings.saveRotation = veh.Rotation;
            veh.settings.SaveDimension = veh.Dimension;
            veh.Update();

            MainChat.SendInfoChat(player, "[?] 已保存车辆坐标和虚拟世界, frespawn 或 服务器重启后会刷新在保存位置.");
            return;
        }

        [Command(CONSTANT.COM_SellVehicleToPlayer)]
        public static async void COM_SellVehicleToPlayeR(PlayerModel player, params string[] args)
        {
            try
            {
                if (args.Length <= 0) { MainChat.SendInfoChat(player, CONSTANT.DESC_SellVehicleToPlayer); return; }
                VehModel veh = (VehModel)player.Vehicle;
                if (veh == null) { MainChat.SendErrorChat(player, CONSTANT.COM_VQueryVehNotFound); return; }

                if (veh.Model == (uint)VehicleModel.Bmx || veh.Model == (uint)VehicleModel.Cruiser || veh.Model == (uint)VehicleModel.Fixter ||
                     veh.Model == (uint)VehicleModel.Scorcher || veh.Model == (uint)VehicleModel.TriBike || veh.Model == (uint)VehicleModel.TriBike2 || veh.Model == (uint)VehicleModel.TriBike3)
                { MainChat.SendErrorChat(player, "[错误] 无效车辆类型出售(自行车及部分两轮车)."); return; }


                if (veh.fine > 0) { MainChat.SendErrorChat(player, "[错误] 此车的税款/罚款还未付清, 无法出售."); return; }
                string selectType = args[0].ToString();
                switch (selectType)
                {
                    case "man":
                        if (veh.owner != player.sqlID) { MainChat.SendErrorChat(player, CONSTANT.COM_VQueryVehNotOwnYou); return; }
                        var checkSelling = sellingVehicles.Find(x => x.senderSqlId == player.sqlID);
                        if (checkSelling != null) { MainChat.SendErrorChat(player, "[错误] 您已发送出售请求了, 请等待回应."); return; }
                        PlayerModel sellTarget = GlobalEvents.GetPlayerFromSqlID(Int32.Parse(args[1]));
                        if (sellTarget == null) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotFound); return; }
                        if (player.Position.Distance(sellTarget.Position) > 4f) { MainChat.SendErrorChat(player, CONSTANT.ERR_NotNearTarget); return; }
                        Int32.TryParse(args[2], out int sellPrice);
                        if (sellPrice <= (veh.price / 4)) { MainChat.SendErrorChat(player, CONSTANT.ERR_ValueNotNegative); return; }
                        sellingVehicles.Add(new VehicleSellModel { senderSqlId = player.sqlID, targetSqlId = sellTarget.sqlID, vehicleSqlId = veh.sqlID, price = sellPrice });
                        MainChat.SendInfoChat(player, string.Format(CONSTANT.INFO_SellVehicleSendPlayer, veh.Model.ToString(), sellTarget.characterName.Replace("_", " "), sellPrice));
                        MainChat.SendInfoChat(sellTarget, string.Format(CONSTANT.INFO_SellVehicleSendTarget, player.characterName.Replace("_", " "), veh.Model.ToString(), sellPrice));
                        await Task.Delay(40000);
                        var sellingVehicle = sellingVehicles.Find(x => x.vehicleSqlId == veh.sqlID);
                        if (sellingVehicle != null) { sellingVehicles.Remove(sellingVehicle); }
                        break;

                    case "system":
                        if (player.Position.Distance(ServerGlobalValues.vehicleSellPoint) > 5f) { MainChat.SendErrorChat(player, CONSTANT.ERR_NotInSellPoint); return; }
                        if (veh.owner != player.sqlID) { MainChat.SendErrorChat(player, CONSTANT.COM_VQueryVehNotOwnYou); return; }
                        /*player.cash += (veh.price / 2);
                        player.updateSql();
                        Database.DatabaseMain.DeleteVehicle(veh);
                        veh.Remove();*/
                        // int price = ((veh.price / 10) * 5) - ((veh.price / 27000) * (int)veh.km);
                        // if (price < ((veh.price / 10) * 1))
                        //    price = ((veh.price / 10) * 1);
                        // OtherSystem.NativeUi.Inputs.SendButtonInput(player, "Aracınızı $" + price + " karşılığında satmak istiyor musunuz?", "VehicleSell:System");
                        int price = (veh.price / 10) * 9;
                        OtherSystem.NativeUi.Inputs.SendButtonInput(player, "是否以 $" + price + " 的价格将您的车辆出售给系统?", "VehicleSell:System");
                        break;

                    default:
                        MainChat.SendInfoChat(player, CONSTANT.DESC_SellVehicleToPlayer);
                        break;
                }
            }
            catch (Exception e) { Core.Core.OutputLog("[/sellcarto] 系统发生错误." + e.Message, ConsoleColor.Red); return; }
        }

        [AsyncClientEvent("VehicleSell:System")]
        public static async void VehicleSell_System(PlayerModel p, bool selection, string otherval)
        {
            if (p.Ping > 250)
                return;

            if (p.Vehicle == null)
                return;

            if (!selection)
                return;

            VehModel veh = (VehModel)p.Vehicle;

            if (veh.owner != p.sqlID) { MainChat.SendErrorChat(p, CONSTANT.COM_VQueryVehNotOwnYou); return; }
            // int price = ((veh.price / 10) * 5) - ((veh.price / 27000) * (int)veh.km);
            int price = (veh.price / 10) * 9;
            if (price < ((veh.price / 10) * 1))
                price = ((veh.price / 10) * 1); // Yılbaşı
            //int price = (veh.price * 9) / 10;
            p.cash += price;
            await p.updateSql();
            await Database.DatabaseMain.DeleteVehicle(veh);
            veh.Remove();
            MainChat.SendInfoChat(p, "[?] 成功以 $" + price + " 的价格出售车辆至系统.");
        }

        [Command(CONSTANT.COM_SellVehicleAccept)]
        public static async void COM_SellVehicleAccept(PlayerModel player)
        {
            var BuyVehicle = sellingVehicles.Find(x => x.targetSqlId == player.sqlID);
            if (BuyVehicle == null) { MainChat.SendErrorChat(player, CONSTANT.ERR_SellVehicleNotFound); return; }
            if (player.cash < BuyVehicle.price) { MainChat.SendErrorChat(player, CONSTANT.ERR_MoneyNotEnought); return; }
            PlayerModel sellerPlayer = GlobalEvents.GetPlayerFromSqlID(BuyVehicle.senderSqlId);
            if (sellerPlayer == null) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotFound); return; }
            VehModel veh = getVehicleFromSqlId(BuyVehicle.vehicleSqlId);

            if (veh == null)
                return;

            veh.owner = player.sqlID;
            veh.businessId = 0;
            veh.factionId = 0;
            veh.Update();
            player.cash -= BuyVehicle.price;
            sellerPlayer.cash += BuyVehicle.price;
            await player.updateSql();
            await sellerPlayer.updateSql();
            List<Database.DatabaseMain.KeyModel> carKeys = new List<Database.DatabaseMain.KeyModel>();
            await Database.DatabaseMain.updateVehicleKeys(veh.sqlID, carKeys);
            sellingVehicles.Remove(BuyVehicle);

            MainChat.SendInfoChat(player, string.Format(CONSTANT.INFO_SellVehicleSuccesPlayer, sellerPlayer.characterName.Replace("_", " "), BuyVehicle.price));
            MainChat.SendInfoChat(sellerPlayer, string.Format(CONSTANT.INFO_SellVehicleSuccesTarget, player.characterName.Replace("_", " "), BuyVehicle.price));
        }

        [Command("decsellcar")]
        public static void COM_SellVehicleDecline(PlayerModel p, params string[] args)
        {
            var check = sellingVehicles.Find(x => x.targetSqlId == p.sqlID);
            if (check == null) { MainChat.SendInfoChat(p, "[!] 无效车辆出售请求!"); return; }
            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(check.senderSqlId);
            if (t != null) { MainChat.SendInfoChat(t, "[!] " + p.characterName.Replace("_", " ") + " 拒绝了您的车辆出售请求."); }
            MainChat.SendInfoChat(p, "[!] 您已拒绝对方的车辆出售请求.");

            sellingVehicles.Remove(check);
            return;
        }

        [Command(CONSTANT.COM_VehicleInfo)]
        public static async void COM_ShowVehicleInfo(PlayerModel p, params string[] args)
        {
            if (p.Vehicle == null) { MainChat.SendErrorChat(p, CONSTANT.COM_VQueryVehNotFound); return; }
            VehModel v = (VehModel)p.Vehicle;
            string oName = "";
            if (v.owner > 0)
            {
                PlayerModelInfo owner = await Database.DatabaseMain.getCharacterInfo(v.owner);
                if (owner != null)
                {
                    oName = "| {49A0CD}所有者: {C8C8C8}" + owner.characterName.Replace("_", " ");
                }
            }

            p.SendChatMessage("{7EB278}________________[ 车辆信息 " + DateTime.Now.ToString("yyyy/MM/dd HH:mm") + "]________________");
            p.SendChatMessage("<i class='fad fa-tire'></i> {49A0CD}[" + v.sqlID + "] {C8C8C8}'" + (VehicleModel)v.Model + "' " + oName + " |{49A0CD} 车牌号码: {C8C8C8}" + v.NumberplateText);
            p.SendChatMessage("<i class='fad fa-tire'></i> {49A0CD}模型: {C8C8C8}" + (VehicleModel)v.Model + " |{49A0CD} 里程: {C8C8C8}" + String.Format("{0:0.00}", v.km));
            p.SendChatMessage("<i class='fad fa-tire'></i> {49A0CD}初始税: {C8C8C8}" + v.defaultTax + "$ |{49A0CD} 税务登记: {C8C8C8}$" + v.fine + "/$" + (v.price / 10) + " |{49A0CD} 价格: {C8C8C8}" + v.price.ToString());
            if (v.factionId > 0)
            {
                FactionModel vF = await Database.DatabaseMain.GetFactionInfo(v.factionId);
                p.SendChatMessage("<i class='fad fa-tire'></i> {49A0CD}注册组织: " + vF.name);
            }
            if (v.businessId > 0)
            {
                BusinessModel vB = await Database.DatabaseMain.GetBusinessInfo(v.businessId);
                p.SendChatMessage("<i class='fad fa-tire'></i> {49A0CD}注册产业: {C8C8C8}" + vB.name);
            }
            p.SendChatMessage("<hr>");
            string fines = String.Join("<br>", v.settings.fines);
            p.SendChatMessage(fines);
            p.SendChatMessage("{7EB278}________________[ 车辆信息 " + DateTime.Now.ToString("yyyy/MM/dd HH:mm") + "]________________");
            if (p.isFinishTut == 6)
            {
                p.isFinishTut = 7;
                CharacterSettings cSet = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
                ServerItems lcItem = Items.LSCitems.Find(x => x.ID == 17);
                lcItem.name = "驾驶证 " + p.sqlID.ToString();
                lcItem.data = p.sqlID.ToString();
                bool succes = await Inventory.AddInventoryItem(p, lcItem, 1);
                if (!succes) { MainChat.SendErrorChat(p, "[错误] 您的库存满了."); return; }
                DriverLicense newLicense = new DriverLicense();
                newLicense.licenseDate = DateTime.Now;
                newLicense.licenseDate = newLicense.licenseDate.AddDays(20);
                newLicense.finePoint = 0;
                cSet.driverLicense = newLicense;
                p.settings = JsonConvert.SerializeObject(cSet);
                await p.updateSql();       
                //p.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}我有{fc5e03}车{FFFFFF}了!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}嘿, 想必这肯定难不倒您!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}小地图旁边有车辆速度记, 可以在驾驶时查看!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}您现在已经了解了车辆系统的大概, 但是您需要知道开车得有驾照!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}为了方便扮演者, 我们现在为您注册了驾照, 要不然一会儿您可能被警察查!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}我们给您的驾照是有期限的, 需要您在驾驶证到期之前去市政府更新驾驶证!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}出示驾驶证可以输入 {fc5e03}/slic{FFFFFF}, 出示身份证可以输入 {fc5e03}/showid{FFFFFF}!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: 现在出发去下一个教程点吧， 记得遵守洛圣都市的交通规则!");        
                GlobalEvents.CheckpointCreate(p, TutorialMain.TutPos[2], 1, 2, new Rgba(255, 255, 255, 100), "Tutorial:Run", "7");
            }   
            return;
        }

        [AsyncClientEvent("Wheel:myCars")]
        public async static Task WheelShowCarStats(PlayerModel p)
        {
            string carsText = "<center>您的车辆</center><br>";
            foreach (VehModel v in Alt.GetAllVehicles())
            {
                if (v.owner == p.sqlID)
                {
                    carsText += ((VehicleModel)v.Model).ToString() + "[" + v.sqlID + "] - 车牌号码: " + v.NumberplateText + " - 待付税款: " + v.fine + "<br>";
                }
            }

            MainChat.SendInfoChat(p, carsText, true);
        }
        
        [Command("mycars")]
        public void COM_ShowAllVehicles(PlayerModel p)
        {
            string carsText = "<center>您的车辆</center><br>";
            foreach (VehModel v in Alt.GetAllVehicles())
            {
                if (v.owner == p.sqlID)
                {
                    carsText += ((VehicleModel)v.Model).ToString() + "[" + v.sqlID + "] - 车牌号码: " + v.NumberplateText + " - 待付税款: " + v.fine + "<br>";
                }
            }

            MainChat.SendInfoChat(p, carsText, true);
        }

        [Command("cgps")]
        public static async Task COM_VehicleGPS(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /agps [车辆ID]"); return; }
            int key; bool isOk = Int32.TryParse(args[0], out key);
            if (!isOk) { MainChat.SendInfoChat(p, "[用法] /agps [车辆ID]"); return; }
            VehModel veh = getVehicleFromSqlId(key);
            if (veh == null)
                return;

            bool hasKey = await GetKeysQuery(p, veh);

            if (!hasKey) { MainChat.SendErrorChat(p, "[错误] 您没有指定车辆的钥匙"); return; }
            
            if (p.isFinishTut == 2)
            {
                p.isFinishTut = 3;
                p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}我有{fc5e03}车{FFFFFF}了!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}成功定位您的车辆，请前往车辆的位置。");
                GlobalEvents.CheckpointCreate(p, veh.Position, 60, 5, new Rgba(255, 0, 0, 255), "Tutorial:Run", "3");
            }
            else
            {
                GlobalEvents.CheckpointCreate(p, veh.Position, 60, 5, new Rgba(255, 0, 0, 255), "", "");
                MainChat.SendInfoChat(p, "> 您的车辆位置已标记至地图.");
            }
        }

        [Command("neonon")]
        public static void COM_VehicleNeon(PlayerModel p, params string[] args)
        {
            if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] 您不在车内."); return; }
            VehModel v = (VehModel)p.Vehicle;
            if (v == null)
                return;

            if (!v.settings.hasNeon) { MainChat.SendErrorChat(p, "[错误] 此车没有霓虹灯."); return; }

            v.NeonColor = v.settings.NeonColor;
            v.SetNeonActive(true, true, true, true);
            //v.AppearanceData = v.settings.ModifiyData;
            return;
        }

        [Command("neonoff")]
        public static void COM_VehicleNeonClose(PlayerModel p, params string[] args)
        {
            if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] 您不在车内."); return; }
            VehModel v = (VehModel)p.Vehicle;
            if (v == null)
                return;

            if (!v.settings.hasNeon) { MainChat.SendErrorChat(p, "[错误] 此车没有霓虹灯."); return; }

            v.SetNeonActive(false, false, false, false);
            //v.AppearanceData = v.settings.ModifiyData;
            return;
        }

        [Command("cw")]
        public static void COM_VehicleWindow(PlayerModel p, params string[] args)
        {

            if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] 您不在车内."); return; }
            VehModel v = (VehModel)p.Vehicle;
            if (v == null)
                return;


            //v.SetWindowOpened(p.Seat, !v.window);
            v.window = !v.window;
            string text = (v.window) ? "打开了" : "关闭了";

            //if (!v.window)
            //{
            //    v.NetworkOwner.EmitAsync("Vehicle:FixWindows", v.Id, (int)p.Seat);
            //}

            MainChat.EmoteMe(p, " " + text + "车窗");

            return;
        }


        // ! New Events
        [AsyncClientEvent("Vehicle:SignalState")]
        public static void Vehicle_SignalState(VehModel v, bool isLeftSignal)
        {
            if (isLeftSignal)
            {
                if (v.HasStreamSyncedMetaData("Vehicle:Signal:Left")) { v.DeleteStreamSyncedMetaData("Vehicle:Signal:Left"); return; }
                else { v.SetStreamSyncedMetaData("Vehicle:Signal:Left", true); return; }
            }
            else
            {
                if (v.HasStreamSyncedMetaData("Vehicle:Signal:Right")) { v.DeleteStreamSyncedMetaData("Vehicle:Signal:Right"); return; }
                else { v.SetStreamSyncedMetaData("Vehicle:Signal:Right", true); return; }
            }
        }

        // KARAVAN 
        public static async Task<bool> tryLoginCaravan(PlayerModel p)
        {

            if (p.HasData("InCaravan"))
                return false;
            VehModel v = getNearVehFromPlayer(p);
            if (v == null)
                return false;
            if (v.Model != (uint)VehicleModel.Camper && v.Model != (uint)VehicleModel.Journey)
                return false;

            //Position interior = Core.Events.GetTruePosition(v.Position, v.Rotation, 2, -1);
            if (p.Position.Distance(v.Position) > 3)
                return false;
            if (await GetKeysQuery(p, v) == false)
            {
                if (v.LockState == VehicleLockState.Locked)
                {
                    MainChat.SendErrorChat(p, "[错误] 房车是锁的."); return false;
                }
                else
                {
                    p.SetData("InCaravan", v.sqlID);
                    p.Position = new Position(1972.9846f, 3816.3428f, 33.424927f);
                    p.Dimension = v.sqlID;
                    return true;
                }

            }
            p.SetData("InCaravan", v.sqlID);
            p.Position = new Position(1972.9846f, 3816.3428f, 33.424927f);
            p.Dimension = v.sqlID;
            return true;
        }

        public static bool leaveCaravan(PlayerModel p)
        {
            if (!p.HasData("InCaravan"))
                return false;

            VehModel v = getVehicleFromSqlId(p.lscGetdata<int>("InCaravan"));
            if (v == null)
                return false;

            Position interior = v.Position;
            interior.X += 1;
            p.Position = interior;
            p.Dimension = v.Dimension;
            p.DeleteData("InCaravan");
            return true;
        }

        [Command("truepos")]
        public static void test_test(PlayerModel p, int number, int x, int y)
        {
            IVehicle v = Alt.GetAllVehicles().Where(x => x.Id == number).FirstOrDefault();
            if (v == null)
                return;

            p.SendChatMessage("传送?");
            p.Position = Core.Events.GetTruePosition(v.Position, v.Rotation, x, y);


        }

        [Command("radtest")]
        public static void test_tes(PlayerModel p, params string[] args)
        {
            if (p.Vehicle == null)
                return;
            IVehicle veh = p.Vehicle;
            p.SendChatMessage("Pitch:" + veh.Rotation.Pitch + " | roll: " + veh.Rotation.Roll + " | Yaw: " + veh.Rotation.Yaw);
            p.SendChatMessage(JsonConvert.SerializeObject(veh.Rotation));
            return;
        }

        [Command("trunklock")]
        public static async void COM_TrunkLock(PlayerModel p, params string[] args)
        {
            VehModel v = getNearVehFromPlayer(p);
            if (v == null) { MainChat.SendErrorChat(p, "[错误] 附近没有车辆!"); return; }
            if (!await GetKeysQuery(p, v)) { MainChat.SendErrorChat(p, "[错误] 您没有此车的钥匙!"); return; }

            v.settings.TrunkLock = !v.settings.TrunkLock;

            MainChat.AME(p, ((v.settings.TrunkLock) ? "掏出钥匙并锁上了" : "掏出钥匙并解锁了") + "后备箱");
            MainChat.SendInfoChat(p, "您成功" + ((v.settings.TrunkLock) ? "锁上了后备箱" : "解锁了后备箱") + ", 您现在可以输入/trunk打开后备箱了");
            return;
        }


        [Command("rentcaroff")]
        public void COM_VehRentRemove(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 1)
                return;

            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v == null)
                return;
            //v.HasData(EntityData.VehicleEntityData.VehicleTempOwner) && v.HasData(EntityData.VehicleEntityData.VehicleRentTime)
            if (v.HasData(EntityData.VehicleEntityData.VehicleTempOwner))
                v.DeleteData(EntityData.VehicleEntityData.VehicleTempOwner);

            if (v.HasData(EntityData.VehicleEntityData.VehicleRentTime))
                v.DeleteData(EntityData.VehicleEntityData.VehicleRentTime);

            MainChat.SendInfoChat(p, "[?] 已重置租车");
            return;
        }

        [Command("setrentprice")]
        public async void COM_SetRentPrice(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /setrentprice [金额]"); return; }
            if (p.Vehicle == null) { MainChat.SendInfoChat(p, "[错误] 您不在车内."); return; }
            VehModel v = (VehModel)p.Vehicle;

            if (v.owner != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 此车不属于您."); return; }
            if (!Int32.TryParse(args[0], out int newPrice)) { MainChat.SendInfoChat(p, "[用法] /setrentprice [金额]"); return; }
            if (newPrice <= 0 || newPrice > 50) { MainChat.SendErrorChat(p, "[错误] 租金为0-50"); return; }

            v.settings.RentPrice = newPrice;
            v.Update();
            MainChat.SendInfoChat(p, "[?] 成功设置车辆租金为 $" + newPrice);
            return;
        }

    }
}
