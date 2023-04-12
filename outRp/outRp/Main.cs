using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.EntitySync;
using AltV.Net.EntitySync.ServerEvent;
using AltV.Net.EntitySync.SpatialPartitions;
using Newtonsoft.Json;
using outRp.Models;
using outRp.OtherSystem.LSCsystems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using outRp.Kook;
using outRp.ReTimerEvent;
using outRp.Utils;

namespace outRp
{
    public class Main : AsyncResource
    {
        public Main() : base(false) { }
        public override IEntityFactory<IPlayer> GetPlayerFactory()
        {
            return new MyPlayerFactory();
        }

        public override IEntityFactory<IVehicle> GetVehicleFactory()
        {
            return new MyVehicleFactory();
        }


        public static Thread mainThread = new Thread(MainThread);
        public static Thread tempThread = new Thread(TempThread);
        public static Thread thread_1 = new Thread(Thread1);
        public static Thread thread_2 = new Thread(Thread2);
        public static Thread thread_3 = new Thread(Thread3);


        public override void OnStart()
        {
            Alt.Log("你好, 世界.");
            //Thread gameThread = new Thread(Thread1);
            //gameThread.Start();
            //Alt.Log("Oyun Thread'ı çalıştırıldı. Thread Id: " + gameThread.ManagedThreadId);
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);

            foreach (var veh in Alt.GetAllVehicles())
            {
                veh.Remove();
            }

            mainThread.Start();
            mainThread.Priority = ThreadPriority.Normal;

            tempThread.Start();

            thread_1.Start();
            thread_2.Start();
            thread_3.Start();
            ServerUtil.InitReTimerEvent();

            foreach (var p in Alt.GetAllPlayers())
            {
                Main.PlayerConnect(p, "Relog");
            }
        }

        public static void MainThread()
        {
            AltEntitySync.Init(7, (threadId) => 500, (threadId) => false,
                (threadCount, repository) => new ServerEventNetworkLayer(threadCount, repository),
                (entity, threadCount) => entity.Type,
                (entityId, entityType, threadCount) => entityType,
                (threadId) =>
                {
                    return threadId switch
                    {
                        // Marker
                        0 => new LimitedGrid3(50_000, 50_000, 75, 10_000, 10_000, 64),
                        // Text
                        1 => new LimitedGrid3(50_000, 50_000, 75, 10_000, 10_000, 32),
                        // Props
                        2 => new LimitedGrid3(50_000, 50_000, 100, 10_000, 10_000, 1500),
                        // Help Text
                        3 => new LimitedGrid3(50_000, 50_000, 100, 10_000, 10_000, 1),
                        // Blips
                        4 => new LimitedGrid3(50_000, 50_000, 70, 10_000, 10_000, 350),
                        // Dynamic Blip
                        5 => new LimitedGrid3(50_000, 50_000, 175, 10_000, 10_000, 200),
                        // Ped
                        6 => new LimitedGrid3(50_000, 50_000, 175, 10_000, 10_000, 128),
                        _ => new LimitedGrid3(50_000, 50_000, 175, 10_000, 10_000, 115),
                    };
                },
            new IdProvider());
            Alt.Log("主线程执行 (" + mainThread.ManagedThreadId.ToString() + ")");
            Alt.Log("开始加载.");
        }
        /*
         * 
         *  { pos: {}, dimension: 0 }
         */
        public static async void TempThread()
        {
            await Database.DatabaseMain.setOfflineAll();
            if (!Alt.Core.IsDebug)
            {
                Database.DatabaseMain.getAllServerVehicles(); // Arabaları yükle.               
            }
            VehicleVendor.LoadVehicleVendor();
            OtherSystem.ClothingShop.LoadClothingShop();
            CrateEvents.LoadServerCrates();
            await Props.Houses.LoadServerHouses();
            OtherSystem.LSCsystems.Market.LoadMarketSystem();
            VehicleTaxPayment.LoadTaxSystem();
            Factions.Faction.LoadFactionSystem();
            Globals.System.News.LoadNewsSystem();
            await Props.Business.LoadAllBusiness();
            DriverLicenses.LoadLicenseSystem();
            await Database.DatabaseMain.LoadServerSettings();
            TruckerJob.LoadTruckJob();
            OtherSystem.LSCsystems.PDelevator.LoadElevatorSystem();
            Globals.System.FD.LoadFDSystem();
            OtherSystem.LSCsystems.PawnShop.LoadPawnShopSystem();
            OtherSystem.LSCsystems.Barber.LoadBarberSystem();
            OtherSystem.LSCsystems.ATM.LoadBankSystem();
            await Models.GarageModel.LoadServerGarages();
            OtherSystem.LSCsystems.Fishing.LoadFishingSystem();
            await teleporters.LoadAllTeleporters();
            OtherSystem.LSCsystems.General.LoadGenerals();
            Globals.System.PD.LoadPDSystems();
            DriverSchool.ServerEvent.Init();

            // Alt.Export("sendNormalChat", new Action<IPlayer, string>(ServerEvent.OnPlayerChatMessage));
            foreach (VehModel v in Alt.GetAllVehicles())
            {
                v.settings.TrunkLock = true;
            }
            Alt.Log("临时线程已完成, 线程正在关闭.");
            tempThread.Interrupt();
            Alt.Log("状态: " + tempThread.ThreadState.ToString() + " - " + tempThread.IsAlive.ToString());
        }

        public static void Thread1()
        {
            //Company.BusinesMain.LoadCompanySystems();
            Alt.Log("线程 1 执行 (" + thread_1.ManagedThreadId.ToString() + ")");
            ServerEvent.ServerTimer();
        }
        public static async void Thread2()
        {
            Alt.Log("线程 2 执行 (" + thread_2.ManagedThreadId.ToString() + ")");
            // NativeEventHandler eventHandler = new NativeEventHandler();
            PetSystem.PetTimer();
            await KookSpace.Init();
        }
        
        public static void Thread3()
        {
            Alt.Log("线程 3 执行 (" + thread_3.ManagedThreadId.ToString() + ")");
        }

        public class LoginCameras
        {
            public Position camera { get; set; }
            public Position lookat { get; set; }
        }

        public static List<LoginCameras> cameras = new List<LoginCameras>() {
            new LoginCameras(){ camera = new Position(-94, -763, 46), lookat = new Position(-132, -656, 86)},
            new LoginCameras(){ camera = new Position(710, 1104, 330), lookat = new Position(728, 1192, 337)},
            new LoginCameras(){ camera = new Position(-1574, 2104, 69), lookat = new Position(-1607, 2094, 72)},
            new LoginCameras(){ camera = new Position(-1124, -286, 38), lookat = new Position(-1113, -270, 41)},
            new LoginCameras(){ camera = new Position(-813, -1409, 5), lookat = new Position(-898, -1373, 11)},
            new LoginCameras(){ camera = new Position(2115, 1862, 131), lookat = new Position(2167, 1929, 133)}
        };

        public static async Task PlayerConnect(IPlayer player, string reason)
        {
            Random rnd = new Random();
            int currCam = rnd.Next(0, 5);
            Position curPos = cameras[currCam].camera;
            curPos.Z -= 5;
            player.Position = curPos;
            Globals.GlobalEvents.FreezeEntity((PlayerModel)player, true);
            Globals.GlobalEvents.CreateCamera((PlayerModel)player, cameras[currCam].camera, new Rotation(0, 0, 0), fov: 50);
            Globals.GlobalEvents.LookCamera((PlayerModel)player, cameras[currCam].lookat);
            bool isBanned = await Database.DatabaseMain.CheckSocialBan(player.SocialClubId);
            if (isBanned) { player.Kick("很抱歉, 您的R星账号已被服务器封禁."); return; }
            bool isHwidBanned = await Database.DatabaseMain.CheckHwidBan(player);
            if (isHwidBanned) { player.Kick("很抱歉, 您已被服务器封禁."); return; }
            await AltAsync.SetDateTimeAsync(player, 20, 3, 2020, 22, 0, 0);
            DoorSystem.LoadDoorsToPlayer((PlayerModel)player);
            string json = JsonConvert.SerializeObject(Database.DatabaseMain.getUpdateInfo());
            await Task.Delay(2000);
            player.EmitLocked("login:Start", json);
            player.Dimension = 0;
        }

        public override async void OnStop()
        {
            foreach (PlayerModel p in Alt.GetAllPlayers())
            {
                await p.updateSql();
            }
            foreach (VehModel v in Alt.GetAllVehicles())
            {
                v.Update();
            }
            await Database.DatabaseMain.SaveServerSettings();
            Alt.Log("车辆数据已保存.");
            Alt.Log("角色数据已保存.");
            Alt.Log("OutRp 关闭");
            Alt.Log("农场数据已保存.");
            Alt.Log("商店数据已保存.");
            await Database.DatabaseMain.SaveServerSettings();
            await KookSpace.StopAsync();
        }
    }
}
