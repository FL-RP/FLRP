using System;
using AltV.Net; 
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltV.Net.Data;
using outRp.Chat;
using outRp.Globals;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;
using outRp.Models;
using outRp.Core;
using outRp.OtherSystem.NativeUi;
using outRp.Kook;

namespace outRp.OtherSystem.LSCsystems
{
    public class TaxiJob : IScript
    {
        [Command("taximeter")]
        public async Task COM_StartTaxiMeter(PlayerModel p, params string[] args)
        {
            if(args.Length <= 0) { return; }
            if(p.Vehicle == null) { return; }
            VehModel v = (VehModel)p.Vehicle;
            if(!await Vehicle.VehicleMain.GetKeysQuery(p, v)) { MainChat.SendErrorChat(p, CONSTANT.COM_VQueryVehNotHaveKeys); return; }
            if(v.jobId != ServerGlobalValues.JOB_Taxi) { MainChat.SendErrorChat(p, "[错误] 此车辆不隶属于任何出租车公司."); return; }             

            int price; bool priceOk = Int32.TryParse(args[0], out price);
            if (!priceOk) { MainChat.SendInfoChat(p, "[用法] /taximeter 0-100"); return; }

            if (v.HasSyncedMetaData("VehTaxiMeter"))
            {
                v.DeleteSyncedMetaData("VehTaxiMeter");
                MainChat.SendInfoChat(p, "已关闭计费器.");
                return;
            }
            else
            {
                v.SetSyncedMetaData("VehTaxiMeter", price);
                MainChat.SendInfoChat(p, "已打开计费器.");
                return;
            }

            
        }

        [Command("taxiad")]
        public async void COM_TaxiAdversiment(PlayerModel p)
        {
            if (!p.HasData("Taxi:Duty")) { MainChat.SendErrorChat(p, "[错误] 您没有上班!"); return; }
            if (!p.HasData("Taxi:Adversiment"))
                p.SetData("Taxi:Adversiment", DateTime.Now.AddMinutes(-5));

            if (p.lscGetdata<DateTime>("Taxi:Adversiment") > DateTime.Now) { MainChat.SendErrorChat(p, "[错误] 您现在不能使用此指令, 剩余使用时间: " + (DateTime.Now - p.lscGetdata<DateTime>("Taxi:Adversiment")).Minutes.ToString() + "分钟"); return; }

            string message = "{FFFF00}[市中心出租车公司广告]{E3E3DE} 我们的出租车司机 " + p.fakeName.Replace("_", " ") + " 开始为市民服务, 联系电话:" + p.phoneNumber;

            string kook_msg = "🚗[市中心出租车公司广告]\n我们的出租车司机 " + p.fakeName.Replace("_", " ") + " 开始为市民服务\n📞联系电话:" + p.phoneNumber;

            if (p.factionId > 0)
            {
                var faction = await Database.DatabaseMain.GetFactionInfo(p.factionId);
                if(faction.type == Globals.ServerGlobalValues.fType_Taxi)
                    message = "{BDDC27}[" + faction.name + "]{E3E3DE} " + p.fakeName.Replace("_", " ") + " 开始上班了, 联系电话:" + p.phoneNumber;

                    kook_msg = "🚗[" + faction.name + "]\n" + p.fakeName.Replace("_", " ") + " 开始上班了\n📞联系电话:" + p.phoneNumber;
            }

            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (t.isNews)
                {
                    t.SendChatMessage(message);
                }
            }
            await KookSpace.AdMessage(kook_msg);

            p.SetData("Taxi:Adversiment", DateTime.Now.AddMinutes(5));
            return;
        }

        [Command("taximenu")]
        public static void COM_SpawnTaxi(PlayerModel p)
        {
            if(p.Position.Distance(new Position(895, -179, 74)) > 10) { MainChat.SendErrorChat(p, "[错误] 您必须在出租车区才能使用此指令."); return; }
            LSCUI.UI ui = new LSCUI.UI();
            ui.Banner = new string[] { "shopui_title_conveniencestore", "shopui_title_conveniencestore" };
            ui.Title = "出租车司机";
            ui.SubTitle = "出租车菜单";
            ui.StartPoint = new int[] { 600, 400 };

            if (p.HasData("Taxi:SpawnedCar"))
            {
                LSCUI.Component_Item deleteTaxi = new LSCUI.Component_Item();
                deleteTaxi.Header = "删除现有的出租车.";
                deleteTaxi.Description = "删除您刷出的出租车.";
                deleteTaxi.Trigger = "Taxi:Event:RemoveTaxi";

                ui.Items.Add(deleteTaxi);
            }
            else
            {
                LSCUI.Component_Item createTaxi_1 = new LSCUI.Component_Item();
                createTaxi_1.Header = "现代出租车 ~g~$150";
                createTaxi_1.Description = "在您退出游戏之前, 您的出租车不会被删除.";
                createTaxi_1.Trigger = "Taxi:Event:Spawn";
                createTaxi_1.TriggerData = "taxi";
                ui.Items.Add(createTaxi_1);

                LSCUI.Component_Item createTaxi_2 = new LSCUI.Component_Item();
                createTaxi_2.Header = "经典出租车 ~g~$100";
                createTaxi_2.Description = "在您退出游戏之前, 您的出租车不会被删除.";
                createTaxi_2.Trigger = "Taxi:Event:Spawn";
                createTaxi_2.TriggerData = "dynasty";
                ui.Items.Add(createTaxi_2);
            }

            ui.Send(p);
        }

        [AsyncClientEvent("Taxi:Event:RemoveTaxi")]
        public void EVENT_RemoveTaxi(PlayerModel p)
        {
            LSCUI.Close(p);
            if (!p.HasData("Taxi:SpawnedCar"))
                return;

            VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(p.lscGetdata<int>("Taxi:SpawnedCar"));
            if (v == null)
                return;

            v.Remove();
            p.DeleteData("Taxi:SpawnedCar");
            MainChat.SendInfoChat(p, "[!] 已删除指定出租车.");
            return;
        }

        [AsyncClientEvent("Taxi:Event:Spawn")]
        public void EVENT_SpawnTaxi(PlayerModel p, string selection)
        {
            LSCUI.Close(p);
            if (p.HasData("Taxi:SpawnedCar")) { MainChat.SendErrorChat(p, "[错误] 您已有刷出的出租车了."); return; }

            switch (selection)
            {
                case "taxi":
                    if(p.cash < 150) { MainChat.SendErrorChat(p, "[错误] 您没有足够的钱!"); return; }
                    IVehicle v = Alt.CreateVehicle(AltV.Net.Enums.VehicleModel.Taxi, new Position(915.82416f, -163.76703f, 74.62268f), new Rotation(0, 0, 0));
                    VehModel veh = (VehModel)v;
                    veh.sqlID = 200000 + veh.Id;
                    veh.owner = p.sqlID;
                    veh.maxFuel = 100;
                    veh.currentFuel = 10;
                    veh.fuelConsumption = 1;
                    veh.jobId = 4;
                    veh.NumberplateText = "出租车_" + p.sqlID;
                    p.cash -= 150;
                    p.updateSql();
                    p.SetData("Taxi:SpawnedCar", veh.sqlID);
                    break;

                case "dynasty":
                    if(p.cash < 100) { MainChat.SendErrorChat(p, "[错误] 您没有足够的钱!"); return; }
                    IVehicle v2 = Alt.CreateVehicle(AltV.Net.Enums.VehicleModel.Dynasty, new Position(915.82416f, -163.76703f, 74.62268f), new Rotation(0, 0, 0));
                    VehModel veh2 = (VehModel)v2;
                    veh2.sqlID = 200000 + veh2.Id;
                    veh2.owner = p.sqlID;
                    veh2.maxFuel = 100;
                    veh2.currentFuel = 10;
                    veh2.fuelConsumption = 1;
                    veh2.jobId = 4;
                    veh2.SetAppearanceDataAsync("2802_XyoAAA1vACAAAIYWAAAAEQAAAAgAAA8QBgzKlYAAAAAAAAAAAAAAAA");
                    veh2.NumberplateText = "出租车_" + p.sqlID;
                    p.cash -= 100;
                    p.updateSql();
                    p.SetData("Taxi:SpawnedCar", veh2.sqlID);
                    break;

                default:
                    MainChat.SendErrorChat(p, "[错误] 发生了错误.");
                    return;
            }

            
            MainChat.SendInfoChat(p, "[!] " + selection + " 您的出租车已成功生成.");
            return;

        }


    }
}
