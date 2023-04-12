using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Enums;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Globals;
using outRp.Models;
using outRp.OtherSystem.NativeUi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;
using outRp.Tutorial;

namespace outRp.OtherSystem.LSCsystems
{
    public class VehicleMenu : IScript
    {
        // TODO Tuş ataması yap
        [Command("vmenu")]
        [AsyncClientEvent("showVmenu")]
        public async Task COM_VehicleMenu(PlayerModel p)
        {
            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v == null)
                return;

            if (!await Vehicle.VehicleMain.GetKeysQuery(p, v))
                return;


            LSCUI.UI ui = new LSCUI.UI();
            ui.StartPoint = new int[] { 600, 400 };
            ui.Title = ((VehicleModel)v.Model).ToString();
            ui.SubTitle = v.NumberplateText;



            // Araç Kilit
            LSCUI.Component_CheckboxItem lockState = new LSCUI.Component_CheckboxItem();
            lockState.Header = "车锁开关";
            bool locked = true;
            if (v.LockState == VehicleLockState.Unlocked)
                locked = false;
            lockState.Check = locked;
            lockState.Description = "打开/关闭车辆的锁定状态.";
            lockState.Trigger = "VehMenu:LockState";
            lockState.TriggerData = v.sqlID.ToString();
            ui.CheckboxItems.Add(lockState);

            // Araç Kilit
            LSCUI.Component_CheckboxItem windows = new LSCUI.Component_CheckboxItem();
            windows.Header = "车窗开关";
            windows.Check = v.window;
            windows.Description = "打开/关闭车辆窗户.";
            windows.Trigger = "VehMenu:Window";
            windows.TriggerData = v.sqlID.ToString();
            ui.CheckboxItems.Add(windows);

            // Bagaj Kilit
            LSCUI.Component_CheckboxItem trunkLockState = new LSCUI.Component_CheckboxItem();
            trunkLockState.Header = "~o~后备箱解/锁";
            trunkLockState.Check = v.settings.TrunkLock;
            trunkLockState.Description = "解/锁后备箱.";
            trunkLockState.Trigger = "VehMenu:LockTrunkState";
            trunkLockState.TriggerData = v.sqlID.ToString();
            ui.CheckboxItems.Add(trunkLockState);



            // Kapılar Sub menü
            LSCUI.SubMenu doors = new LSCUI.SubMenu();
            doors.Header = "车门";
            doors.SubTitle = "打开/关闭车门.";
            doors.StartPoint = new int[] { 600, 400 };
            for (int a = 0; a < 4; a++)
            {
                LSCUI.Component_Item door = new LSCUI.Component_Item();
                door.Header = "车门 " + a.ToString() + " ~g~开启";
                door.Trigger = "VehMenu:DoorOpen";
                door.TriggerData = v.sqlID.ToString() + "," + a.ToString();
                doors.Items.Add(door);

                LSCUI.Component_Item door2 = new LSCUI.Component_Item();
                door2.Header = "车门 " + a.ToString() + " ~r~关闭";
                door2.Trigger = "VehMenu:DoorClose";
                door2.TriggerData = v.sqlID.ToString() + "," + a.ToString();
                doors.Items.Add(door2);
            }
            ui.SubMenu.Add(doors);

            LSCUI.Component_Item SecurityLevel = new LSCUI.Component_Item();
            SecurityLevel.Header = "防盗等级: ~g~" + v.settings.SecurityLevel;
            SecurityLevel.Description = "显示车辆防盗等级.";
            ui.Items.Add(SecurityLevel);

            LSCUI.SubMenu parkSystem = new LSCUI.SubMenu();
            parkSystem.Header = "~b~泊车菜单";
            parkSystem.SubTitle = "泊车操作";
            parkSystem.StartPoint = new int[] { 600, 400 };
            if (v.settings.parkPosition == new AltV.Net.Data.Position(0, 0, 0))
            {
                LSCUI.Component_Item buyPark = new LSCUI.Component_Item();
                buyPark.Header = "~g~购买泊车位";
                buyPark.Description = "为您的车辆购买新的泊车位. ~g~$500";
                buyPark.Trigger = "Vehicle:BuyPark";
                buyPark.TriggerData = v.sqlID.ToString() + ",123";
                parkSystem.Items.Add(buyPark);
            }
            else
            {
                LSCUI.Component_Item park = new LSCUI.Component_Item();
                park.Header = "~o~泊车";
                park.Description = "停放车辆(车辆会消失进其他虚拟世界).";
                park.Trigger = "Vehicle:Park";
                park.TriggerData = v.sqlID.ToString() + ",123";
                parkSystem.Items.Add(park);

                LSCUI.Component_Item show = new LSCUI.Component_Item();
                show.Header = "~b~标记泊车位";
                show.Description = "在地图上标记您车辆的泊车位.";
                show.Trigger = "Vehicle:ShowPark";
                show.TriggerData = v.sqlID.ToString() + ",123";
                parkSystem.Items.Add(show);

                if (v.Model == (uint)VehicleModel.Camper)
                {
                    LSCUI.Component_Item resetPark = new LSCUI.Component_Item();
                    resetPark.Header = "~r~重置泊车位";
                    resetPark.Description = "重置车辆泊车位. ~g~免费";
                    resetPark.Trigger = "Vehicle:ResetPark";
                    resetPark.TriggerData = v.sqlID.ToString() + ",123";
                    parkSystem.Items.Add(resetPark);
                }
                else
                {
                    LSCUI.Component_Item resetPark = new LSCUI.Component_Item();
                    resetPark.Header = "~r~重置泊车";
                    resetPark.Description = "重置车辆泊车位. ~g~$1000";
                    resetPark.Trigger = "Vehicle:ResetPark";
                    resetPark.TriggerData = v.sqlID.ToString() + ",123";
                    parkSystem.Items.Add(resetPark);
                }
            }
            ui.SubMenu.Add(parkSystem);

            //outRp.Chat.MainChat.SendInfoChat(p, "[已打开车辆菜单]");
            ui.Send(p);
            return;
        }

        [AsyncClientEvent("VehMenu:Window")]
        public void Event_VehicleWindow(PlayerModel p, bool state, string value)
        {
            if (!Int32.TryParse(value, out int vehSQL))
                return;

            VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(vehSQL);
            if (v == null)
                return;

            if (state)
            {
                v.window = true;
                MainChat.SendInfoChat(p, "[!] 已开启车窗.");
            }
            else
            {
                v.window = false;
                MainChat.SendInfoChat(p, "[!] 已关闭车窗.");
            }

            string text = (v.window) ? "摇下了" : "摇上了";

            MainChat.EmoteMe(p, text + "车窗");
            return;
        }

        [AsyncClientEvent("Vehicle:BuyPark")]
        public void EVENT_BuyPark(PlayerModel p, string vals)
        {
            string[] val = vals.Split(",");
            if (!Int32.TryParse(val[0], out int vehSQL)) { MainChat.SendErrorChat(p, "[!] 发生了错误."); return; }
            VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(vehSQL);
            if (v == null) { MainChat.SendErrorChat(p, "[!] 无效车辆!"); return; }
            if (v.owner != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 此车不属于您."); return; }
            if (v.settings.parkPosition != new AltV.Net.Data.Position(0, 0, 0)) { MainChat.SendErrorChat(p, "[错误] 此车已有泊车位了."); return; }
            if (v.Model != (uint)VehicleModel.Camper)
            {
                if (p.cash < 500) { MainChat.SendErrorChat(p, "[错误] 您没有足够的钱!"); return; }
                p.cash -= 500;
            }

            p.updateSql();
            v.settings.parkPosition = v.Position;
            v.Update();
            MainChat.SendInfoChat(p, "[?] 成功以 $500 的价格购买新的泊车位.", true);
            LSCUI.Close(p);
            
            if (p.isFinishTut == 9)
            {
                p.isFinishTut = 10;
                p.cash += 500;
                p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}我有{fc5e03}车{FFFFFF}了!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}嘿, 想必这肯定难不倒您!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}您成功购买了停车位， 虽然花钱了， 但是我们已经为您报销了!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}现在请重新输入 {fc5e03}/vmenu{FFFFFF} 选择泊车!");
            }               
            return;
        }

        [AsyncClientEvent("Vehicle:ResetPark")]
        public void EVENT_ResetPark(PlayerModel p, string vals)
        {
            string[] val = vals.Split(",");
            if (!Int32.TryParse(val[0], out int vehSQL)) { MainChat.SendErrorChat(p, "[!] 发生了错误."); return; }
            VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(vehSQL);
            if (v == null) { MainChat.SendErrorChat(p, "[!] 无效车辆!"); return; }
            if (v.owner != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 此车不属于您."); return; }
            if (v.settings.parkPosition == new AltV.Net.Data.Position(0, 0, 0)) { MainChat.SendErrorChat(p, "[错误] 此车已有泊车位了."); return; }
            if (v.Model != (int)VehicleModel.Camper)
            {
                if (p.cash < 1000) { MainChat.SendErrorChat(p, "[错误] 您没有足够的钱!"); return; }
            }
            if (v.Dimension == v.sqlID) { MainChat.SendErrorChat(p, "[错误] 车辆在停泊中, 请先取出车辆."); return; }

            if (v.Model != (int)VehicleModel.Camper)
            {
                p.cash -= 1000;
            }
            p.updateSql();
            v.settings.parkPosition = new AltV.Net.Data.Position(0, 0, 0);
            v.Update();

            MainChat.SendInfoChat(p, "[?] 成功以 $1000 的价格重置了车辆泊车位.", true);
            LSCUI.Close(p);
            return;
        }

        [AsyncClientEvent("Vehicle:ShowPark")]
        public void EVENT_ShowPark(PlayerModel p, string vals)
        {
            string[] val = vals.Split(",");
            if (!Int32.TryParse(val[0], out int vehSQL)) { MainChat.SendErrorChat(p, "[!] 发生了错误."); return; }
            VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(vehSQL);
            if (v == null) { MainChat.SendErrorChat(p, "[!] 无效车辆!"); return; }
            if (v.owner != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 此车不属于您."); return; }
            GlobalEvents.CheckpointCreate(p, v.settings.parkPosition, 60, 1, new Rgba(250, 0, 0, 250), "none", "none");
            MainChat.SendInfoChat(p, "[?] 已标记泊车位至您的地图.");
            return;
        }

        [AsyncClientEvent("Vehicle:Park")]
        public void EVENT_MakePark(PlayerModel p, string vals)
        {
            string[] val = vals.Split(",");
            if (!Int32.TryParse(val[0], out int vehSQL)) { MainChat.SendErrorChat(p, "[!] 发生了错误."); return; }
            VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(vehSQL);
            if (v == null) { MainChat.SendErrorChat(p, "[!] 无效车辆!"); return; }
            if (v.settings.parkPosition == new AltV.Net.Data.Position(0, 0, 0)) { MainChat.SendErrorChat(p, "[错误] 此车已有泊车位了!"); return; }
            if (v.Position.Distance(v.settings.parkPosition) > 3) { MainChat.SendErrorChat(p, "[错误] 您离泊车位太远."); return; }
            //if(v.Driver != null) { MainChat.SendErrorChat(p, "[HATA] Aracı park edebilmek için aracın sürücü koltuğunda kimse olmamalı."); return; }
            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (t.Vehicle == v) { MainChat.SendErrorChat(p, "[错误] 指定车辆正在被使用中."); return; }
            }

            var inv = JsonConvert.DeserializeObject<List<ServerItems>>(v.vehInv);
            // sItem.ID == 28 || sItem.ID == 29 || sItem.ID == 30
            if (inv.Where(x => x.ID == 29 || x.ID == 30).FirstOrDefault() != null)
            {
                MainChat.SendErrorChat(p, "[错误] 您无法将车辆后备箱装有武器的车辆停放.");
                return;
            }

            v.Dimension = v.sqlID;
            MainChat.SendInfoChat(p, "[?] 已泊车.");
            LSCUI.Close(p);
            
            if (p.isFinishTut == 10)
            {
                p.isFinishTut = 11;
                p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}我有{fc5e03}车{FFFFFF}了!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}嘿, 想必这肯定难不倒您!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}您成功泊车了，您的车辆会消失进其他世界，关于取车，您可以输入在您的泊车位附近输入{fc5e03}/parkmenu{FFFFFF}取出车辆!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}现在请徒步至地图标记点进入下一个教程!");
                GlobalEvents.CheckpointCreate(p, TutorialMain.TutPos[4], 1, 2, new Rgba(255, 0, 0, 0), "Tutorial:Run", "11");
            }              
            return;

        }

        [Command("parkmenu")]
        public async Task COM_ParkMenu(PlayerModel p)
        {
            //LSCUI.UI ui = new LSCUI.UI();
            //ui.StartPoint = new int[] { 600, 400 };
            //ui.Title = ((VehicleModel)v.Model).ToString();
            //ui.SubTitle = v.NumberplateText;

            LSCUI.UI ui = new LSCUI.UI();
            ui.StartPoint = new int[] { 600, 400 };
            ui.Title = "_";
            ui.SubTitle = "显示您停放的车辆.";
            ui.Banner = new string[] { "shopui_title_carmod2", "shopui_title_carmod2" };

            foreach (VehModel v in Alt.GetAllVehicles())
            {
                if (v.Dimension == v.sqlID)
                {
                    if (await Vehicle.VehicleMain.GetKeysQuery(p, v))
                    {
                        LSCUI.Component_Item car = new LSCUI.Component_Item();
                        car.Header = ((VehicleModel)v.Model).ToString();
                        car.Description = "车牌号码: " + v.NumberplateText + " | 税: " + v.fine;
                        car.Trigger = "Vehicle:OutPark";
                        car.TriggerData = v.sqlID + ",123";
                        ui.Items.Add(car);
                    }
                }
            }

            if (ui.Items.Count <= 0) { MainChat.SendInfoChat(p, "[?] 无已停放车辆."); return; }
            ui.Send(p);
        }

        [AsyncClientEvent("Vehicle:OutPark")]
        [Obsolete]
        public void EVENT_OutPark(PlayerModel p, string vals)
        {
            string[] val = vals.Split(",");
            if (!Int32.TryParse(val[0], out int vehSQL)) { MainChat.SendErrorChat(p, "[!] 发生了错误."); return; }
            VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(vehSQL);
            if (v == null) { MainChat.SendErrorChat(p, "[!] 无效车辆!"); return; }
            if (v.Dimension != v.sqlID) { MainChat.SendErrorChat(p, "[错误] 指定车辆未被停放(已被取出)!"); return; }
            if (v.towwed) { MainChat.SendErrorChat(p, "[错误] 税锁中的车辆无法取出!"); return; }
            if (v.settings.PDLock)
                return;

            v.SetPositionAsync(v.settings.parkPosition);
            v.SetDimensionAsync(p.Dimension);


            MainChat.SendInfoChat(p, "[?] 已取出车辆.");
            LSCUI.Close(p);
            return;
        }

        [AsyncClientEvent("VehMenu:LockState")]
        public void EVENT_LockState(PlayerModel p, bool state, string value)
        {
            if (!Int32.TryParse(value, out int vehSQL))
                return;

            VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(vehSQL);
            if (v == null)
                return;

            if (state)
            {
                v.LockState = VehicleLockState.Locked;
                MainChat.SendInfoChat(p, "[!] 已上锁车辆.");
            }
            else
            {
                v.LockState = VehicleLockState.Unlocked;
                MainChat.SendInfoChat(p, "[!] 已解锁车辆.");
            }
            return;
        }

        [AsyncClientEvent("VehMenu:LockTrunkState")]
        public void EVENT_LockTrunkState(PlayerModel p, bool state, string value)
        {
            if (!Int32.TryParse(value, out int vehSQL))
                return;

            VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(vehSQL);
            if (v == null)
                return;

            if (state)
            {
                v.settings.TrunkLock = state;
                MainChat.SendInfoChat(p, "[!] 已上锁后备箱.");
            }
            else
            {
                v.settings.TrunkLock = state;
                MainChat.SendInfoChat(p, "[!] 已解锁后备箱.");
            }
            return;
        }

        [AsyncClientEvent("VehMenu:DoorOpen")]
        public void EVENT_VehicleDoorOpen(PlayerModel p, string value)
        {
            string[] _val = value.Split(",");
            if (!Int32.TryParse(_val[0], out int SQL) || !Int32.TryParse(_val[1], out int door))
                return;

            VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(SQL);
            if (v == null)
                return;

            if (v.NetworkOwner == null)
                return;

            v.NetworkOwner.EmitLocked("Vehicle:Door", v, true, door);
            return;
        }

        [AsyncClientEvent("VehMenu:DoorClose")]
        public void EVENT_VehicleDoorClose(PlayerModel p, string value)
        {
            string[] _val = value.Split(",");
            if (!Int32.TryParse(_val[0], out int SQL) || !Int32.TryParse(_val[1], out int door))
                return;

            VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(SQL);
            if (v == null)
                return;

            if (v.NetworkOwner == null)
                return;

            v.NetworkOwner.EmitLocked("Vehicle:Door", v, false, door);
            return;
        }
    }

}
