using System;
using System.Threading.Tasks;
using AltV.Net; using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Resources.Chat.Api;
using outRp.Models;
using outRp.Chat;
using outRp.Globals;
using outRp.OtherSystem.Textlabels;
using Newtonsoft.Json;
using outRp.Core;

namespace outRp.OtherSystem.LSCsystems
{
    public class TruckerJob : IScript
    {
       public class TruckConst
        {
            public static Position truckDuty = new Position(-297.94287f, -2598.9495f, 6.1955566f);
            public static Position uploadTruck = new Position(-236.76923f, -2561.934f, 6.9370117f);
            public static Position uploadTruckStep2 = new Position(2674.0088f, 3521.71f, 53.30774f);

            public static Position firstCheckpoint = new Position(2686.9978f, 3455.4197f, 56.357544f);
            public static Position secondCheckpoint = new Position(-263.0769f, -2584.0747f, 5.993f);

            public static int TruckPayment = 1500;
        }

        public static void LoadTruckJob()
        {
            TextLabelStreamer.Create("~r~[~w~卡车运输~r~]~n~开始工作:~g~/starttruck", TruckConst.truckDuty, center: true, font: 0, streamRange: 5);
            GlobalEvents.blipModel factBlip = new GlobalEvents.blipModel();
            factBlip.blipname = "createFactionBlip";
            factBlip.category = 2;
            factBlip.label = "卡车司机";
            factBlip.position = TruckConst.truckDuty;
            factBlip.sprite = 63;
            GlobalEvents.serverBlips.Add(factBlip);
            Alt.Log("加载 卡车运输工作.");
        }

        [Command("starttruck")]
        public void startTruckJob(PlayerModel p)
        {
            if(p.Position.Distance(TruckConst.truckDuty) > 5) { MainChat.SendErrorChat(p, "[错误] 您必须在卡车司机的起始区域才能使用此指令."); return; }
            if (JsonConvert.DeserializeObject<CharacterSettings>(p.settings).jobBan) { MainChat.SendErrorChat(p, "[!] 服务器禁止您从事某项工作."); return; }
            GlobalEvents.SubTitle(p, "~y~业务主管 说: ~w~选一辆在停车场的卡车吧.", 10);
            GlobalEvents.VehicleWaitInterval(p, "Truck:EnterVehicle", "");
        }

        [AsyncClientEvent("Truck:EnterVehicle")]
        public void TruckJobEnterVeh(PlayerModel p, VehModel v)
        {
            if(v.jobId != ServerGlobalValues.JOB_Trucker) { MainChat.SendErrorChat(p, "[错误] 此车辆不属于卡车司机工作, 运输中止."); return; }
            p.SetData("Truck:Vehicle", v.sqlID);
            v.lscSetData(EntityData.VehicleEntityData.VehicleTempOwner, p.sqlID);
            GlobalEvents.CheckpointCreate(p, TruckConst.uploadTruck, 63, 5, new Rgba(255, 255, 255, 100), "Truck:Step1", "");
            GlobalEvents.SubTitle(p, "~y~业务主管 说: ~w~开始工作吧, 先去装载货物.", 10);
            GlobalEvents.CreateBlip(p, "truckJob", 2, TruckConst.uploadTruck, route: true, sprite: 477, color: 5, label: "运输点", Short: false);
            
            // GlobalEvents.PlayerExitVehicleWatcher(p, "Truck:ExitVehicle");
        }
        //..
        [AsyncClientEvent("Truck:ExitVehicle")]
        public async void TruckJobExitVeh(PlayerModel p)
        {
            if (p.HasData("Truck:Vehicle"))
            {
                int vehId = p.lscGetdata<int>("Truck:Vehicle");
                VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(vehId);
                p.DeleteData("Truck:Vehicle");
                v.Position = v.settings.savePosition;
                v.Rotation = v.settings.saveRotation;
                v.Repair();
                v.EngineOn = false;
                v.LockState = AltV.Net.Enums.VehicleLockState.Unlocked;
                if (v.HasData(EntityData.VehicleEntityData.VehicleTempOwner)) { v.DeleteData(EntityData.VehicleEntityData.VehicleTempOwner); }
                await Task.Delay(3000);                
                

                if(p.Exists)
                    p.Position = TruckConst.truckDuty;
                p.EmitLocked("Checkpoint:Destroy");
            }
        }

        [AsyncClientEvent("Truck:Step1")]
        public async void TruckJobFirstCheckpoint(PlayerModel p)
        {
            if(p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] 您不在任何车辆中, 运输取消."); return; }
            VehModel v = (VehModel)p.Vehicle;
            if(v.jobId != ServerGlobalValues.JOB_Trucker) { MainChat.SendErrorChat(p, "[错误] 此车辆不属于卡车司机工作, 运输取消."); return; }
            if(v.Driver != p) { MainChat.SendErrorChat(p, "[错误] 您必须在工作车辆的主驾驶, 运输取消."); return; }

            GlobalEvents.DestroyBlip(p, "truckJob");
            await Task.Delay(200);

            if (!p.Exists)
                return;

            GlobalEvents.CreateBlip(p, "truckJob", 2, TruckConst.firstCheckpoint, route: true, sprite: 477, color: 5, label: "运输点", Short: false);
            v.LockState = AltV.Net.Enums.VehicleLockState.Unlocked;
            GlobalEvents.SubTitle(p, "~y~业务主管 说: ~w~继续去下一点, 搬运重物时要小心.", 10);
            GlobalEvents.CheckpointCreate(p, TruckConst.firstCheckpoint, 63, 5, new Rgba(255, 255, 255, 100), "Truck:Step2", "");
        }

        [AsyncClientEvent("Truck:Step2")]
        public async void TruckJobFirstRoute(PlayerModel p)
        {
            if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] 您不在任何车辆中, 运输取消."); return; }
            VehModel v = (VehModel)p.Vehicle;
            if (v.jobId != ServerGlobalValues.JOB_Trucker) { MainChat.SendErrorChat(p, "[错误] 此车辆不属于卡车司机工作, 运输取消."); return; }
            if (v.Driver != p) { MainChat.SendErrorChat(p, "[错误] 您必须在工作车辆的主驾驶, 运输取消."); return; }

            GlobalEvents.DestroyBlip(p, "truckJob");
            await Task.Delay(200);

            if (!p.Exists)
                return;

            GlobalEvents.CreateBlip(p, "truckJob", 2, TruckConst.uploadTruckStep2, route: true, sprite: 477, color: 5, label: "运输点", Short: false);
            v.LockState = AltV.Net.Enums.VehicleLockState.Unlocked;
            GlobalEvents.SubTitle(p, "~y~业务主管 说: ~w~继续去下一点, 搬运重物时要小心.", 10);
            GlobalEvents.CheckpointCreate(p, TruckConst.uploadTruckStep2, 63, 5, new Rgba(255, 255, 255, 100), "Truck:Step3", "");
        }

        [AsyncClientEvent("Truck:Step3")]
        public async void TruckJobSecondCheckpoint(PlayerModel p)
        {
            if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] 您不在任何车辆中, 运输取消."); return; }
            VehModel v = (VehModel)p.Vehicle;
            if (v.jobId != ServerGlobalValues.JOB_Trucker) { MainChat.SendErrorChat(p, "[错误] 此车辆不属于卡车司机工作, 运输取消."); return; }
            if (v.Driver != p) { MainChat.SendErrorChat(p, "[错误] 您必须在工作车辆的主驾驶, 运输取消."); return; }

            GlobalEvents.DestroyBlip(p, "truckJob");
            await Task.Delay(200);
            if (!p.Exists)
                return;

            GlobalEvents.CreateBlip(p, "truckJob", 2, TruckConst.secondCheckpoint, route: true, sprite: 477, color: 5, label: "Görev Noktası", Short: false);
            v.LockState = AltV.Net.Enums.VehicleLockState.Unlocked;
            GlobalEvents.SubTitle(p, "~y~业务主管 说: ~w~继续去下一点, 搬运重物时要小心.", 10);
            GlobalEvents.CheckpointCreate(p, TruckConst.secondCheckpoint, 63, 5, new Rgba(255, 255, 255, 100), "Truck:Step4", "");
        }



        [AsyncClientEvent("Truck:Step4")]
        public async void TruckJobSecondRoute(PlayerModel p)
        {
            if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] 您不在任何车辆中, 运输取消."); return; }
            VehModel v = (VehModel)p.Vehicle;
            if (v.jobId != ServerGlobalValues.JOB_Trucker) { MainChat.SendErrorChat(p, "[错误] 此车辆不属于卡车司机工作, 运输取消."); return; }
            if (v.Driver != p) { MainChat.SendErrorChat(p, "[错误] 您必须在工作车辆的主驾驶, 运输取消."); return; }

            GlobalEvents.DestroyBlip(p, "truckJob");

            if (p.Vehicle != null) { p.EmitLocked("setPlayerOutVehicle", p.Vehicle);
                GlobalEvents.ForceLeaveVehicle(p);
                await Task.Delay(3000);
                v.Position = v.settings.savePosition;
                v.Rotation = v.settings.saveRotation;
                v.Repair();
                v.EngineOn = false;
                v.LockState = AltV.Net.Enums.VehicleLockState.Unlocked;
                if (v.HasData(EntityData.VehicleEntityData.VehicleTempOwner)) { v.DeleteData(EntityData.VehicleEntityData.VehicleTempOwner); }
            }

            if (!p.Exists)
                return;
            GlobalEvents.SubTitle(p, "~y~业务主管 说: ~w~这是您成功完成工作的报酬, 拿着吧: ~g~$" + TruckConst.TruckPayment, 10);
            if (p.HasData("AC:LastTrucker"))
            {
                var lastDate = p.lscGetdata<DateTime>("AC:LastTrucker");
                if (lastDate >= DateTime.Now.AddMinutes(2))
                {
                    antiCheat.ACBAN(p, 3, "Trucker Dump");
                    return;
                }
            }
            p.SetData("AC:LastTrucker", DateTime.Now.AddMinutes(3));
            p.cash += TruckConst.TruckPayment;
            Core.Logger.WriteLogData(Logger.logTypes.jobCash, p.characterName + " | 卡车司机");
            p.updateSql();
            return;

        }
        

    }
}
