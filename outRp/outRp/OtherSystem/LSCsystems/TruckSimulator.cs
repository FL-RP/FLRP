using System;
using System.Collections.Generic;
using AltV.Net; 
using AltV.Net.Async;
using AltV.Net.Data;
using outRp.Models;
using outRp.OtherSystem.NativeUi;
using outRp.Globals;
using AltV.Net.Elements.Entities;
using AltV.Net.Resources.Chat.Api;
using outRp.Chat;
using outRp.Core;

namespace outRp.OtherSystem.LSCsystems
{
    public class TruckSimulator
    {
        
        public class Positions
        {
            public static Position jobStart = new Position(774, -2474, 20);
            public static Position truckSpawnPos = new Position(810.54065f, -2483.4329f, 22.691528f);

            public static List<Position> serbestPiyasa_RomorkSpawn = new List<Position>()
            {
                new Position(843.9165f, -1987.167f, 29.296753f),
                new Position(914.82196f, -1560.5670f, 30.745728f),
                new Position(921.6528f, -1242.8308f, 25.505493f),
            };

            public static List<Position> serbestPiyasa_RomorkSell = new List<Position>()
            {
                new Position(2900.7525f, 4382.2812f, 50.35901f),
                new Position(879.33624f, 2347.4373f, 51.673218f),
                new Position(-77.8022f, 1879.6879f, 197.17139f),
                new Position(310.08792f, 2876.6375f, 43.5011f),
            };

            public static Position petrolTank_RomorkPos = new Position(1588.5758f, -1715.5121f, 88.13623f);
            public static Position petrolTank_SellPos = new Position(2550.6726f, 417.91647f, 108.45715f);

            public static Position car_RomokrPos = new Position(-273.389f, 6058.233f, 314.487183f);
            public static Position car_SellPos = new Position(1025.6044f, -3184.6682f, 5.89834f);



        }

        public class Datas
        {
            public static string inJob = "Trucker:isJob";
            public static string jobStarted = "Trucker:JobStarted";
            public static string jobFinisPos = "Trucker:JobFinisPos";
        }

        [Command("tircilik")]
        public static void COM_Trucker(PlayerModel p)
        {
            LSCUI.UI ui = new LSCUI.UI();
            ui.StartPoint = new int[] { 700, 500 };
            ui.Title = "Tircilar Birligi";
            ui.SubTitle = "Tırcılık meslek menüsü";
            ui.Banner = new string[] { "shopui_title_gasstation", "shopui_title_gasstation" };

            if (p.HasData(Datas.inJob))
            {
                LSCUI.Component_Item leaveJob = new LSCUI.Component_Item();
                leaveJob.Header = "~r~Meslek bitir";
                leaveJob.Description = "Aktif mesleği sonlandırmanızı sağlar.";
                leaveJob.Trigger = "Trucker:Event:JobStatus";
                ui.Items.Add(leaveJob);

                #region rent Truck
                LSCUI.SubMenu rentCars = new LSCUI.SubMenu();
                rentCars.StartPoint = new int[] { 700, 500 };
                rentCars.Header = "Tır Kirala";
                rentCars.SubTitle = "Eğer meslek için bir aracınız yoksa araç kiralayabilirsiniz.";

                LSCUI.Component_Item rentCar_Hauler = new LSCUI.Component_Item();
                rentCar_Hauler.Header = "Hauler 1saat/~g~$1750";
                rentCar_Hauler.Description = "Hauler model tır kirala";
                rentCar_Hauler.Trigger = "Trucker:Event:RentTruck";
                rentCar_Hauler.TriggerData = "hauler";
                rentCars.Items.Add(rentCar_Hauler);

                LSCUI.Component_Item rentCar_Packer = new LSCUI.Component_Item();
                rentCar_Packer.Header = "Packer 1saat/~g~$2400";
                rentCar_Packer.Description = "Packer model tır kirala";
                rentCar_Packer.Trigger = "Trucker:Event:RentTruck";
                rentCar_Packer.TriggerData = "packer";
                rentCars.Items.Add(rentCar_Packer);

                LSCUI.Component_Item rentCar_Phantom = new LSCUI.Component_Item();
                rentCar_Phantom.Header = "Phantom 1saat/~g~$2700";
                rentCar_Phantom.Description = "Phantom model tır kirala";
                rentCar_Phantom.Trigger = "Trucker:Event:RentTruck";
                rentCar_Phantom.TriggerData = "phantom";
                rentCars.Items.Add(rentCar_Phantom);

                LSCUI.Component_Item rentCar_Phantom3 = new LSCUI.Component_Item();
                rentCar_Phantom3.Header = "Phantom3 1saat/~g~$4000";
                rentCar_Phantom3.Description = "Phantom3 model tır kirala";
                rentCar_Phantom3.Trigger = "Trucker:Event:RentTruck";
                rentCar_Phantom3.TriggerData = "phantom3";
                rentCars.Items.Add(rentCar_Phantom3);

                ui.SubMenu.Add(rentCars);
                #endregion

                #region Select Job
                LSCUI.SubMenu selectJob = new LSCUI.SubMenu();
                selectJob.StartPoint = new int[] { 700, 500 };
                selectJob.Header = "~o~Serbest Piyasa";
                selectJob.SubTitle = "Kiralanmış araçlarla taşıyabileceğiniz yükler.";

                LSCUI.Component_Item job_Petrol = new LSCUI.Component_Item();
                job_Petrol.Header = "~g~Petrol taşıma işi";
                job_Petrol.Description = "Petrol şirketinden petrol alın ve benziliğe sevk edin.";
                job_Petrol.Trigger = "Trucker:Event:StartJob";
                job_Petrol.TriggerData = "petrol";
                selectJob.Items.Add(job_Petrol);

                LSCUI.Component_Item job_Car = new LSCUI.Component_Item();
                job_Car.Header = "~o~Araç taşıma işi";
                job_Car.Description = "Fabrikada üretilen araçları alın ve yurt dışına gönderin.";
                job_Car.Trigger = "Trucker:Event:StartJob";
                job_Car.TriggerData = "car";
                selectJob.Items.Add(job_Car);

                LSCUI.Component_Item job_Random = new LSCUI.Component_Item();
                job_Random.Header = "Diğer";
                job_Random.Description = "Rasgele bir iş verir.";
                job_Random.Trigger = "Trucker:Event:StartJob";
                job_Random.TriggerData = "random";
                selectJob.Items.Add(job_Random);

                ui.SubMenu.Add(selectJob);
                #endregion

            }
            else
            {
                LSCUI.Component_Item joinJob = new LSCUI.Component_Item();
                joinJob.Header = "~g~Meslek başla";
                joinJob.Description = "Mesleğe başlamanızı sağlar.";
                joinJob.Trigger = "Trucker:Event:JobStatus";
                ui.Items.Add(joinJob);
            }

            ui.Send(p);
        }

        [AsyncClientEvent("Trucker:Event:JobStatus")]
        public void EVENT_JobStatus(PlayerModel p)
        {
            if (p.HasData(Datas.inJob))
            {
                p.DeleteData(Datas.inJob);
                GlobalEvents.ClearPlayerTag(p);
                GlobalEvents.UINotifiy(p, 10, "Tircilik", "~r~Meslegi bitirdiniz.");
            }
            else
            {
                p.SetData(Datas.inJob, true);
                GlobalEvents.SetPlayerTag(p, "~y~[Tırcı]");
                GlobalEvents.UINotifiy(p, 10, "Tircilik", "~g~Mesleğe başladınız.");
            }
            LSCUI.Close(p);
            return;
        }

        [AsyncClientEvent("Trucker:Event:RentTruck")]
        public void EVENT_RentTruck(PlayerModel p, string selection)
        {
            LSCUI.Close(p);
            switch (selection)
            {
                case "hauler": // 1750
                    if (p.cash < 1750) { MainChat.SendErrorChat(p, "[HATA] Yeterli paranız bulunmuyor."); return; }
                    p.cash -= 1750;
                    p.updateSql();
                    IVehicle v = Alt.CreateVehicle(AltV.Net.Enums.VehicleModel.Hauler, Positions.truckSpawnPos, new Rotation(0,0,0));
                    VehModel veh = (VehModel)v;
                    veh.sqlID = 200000 + veh.Id;
                    veh.SetData("VehicleTempOwner", p.sqlID);
                    veh.maxFuel = 200;
                    veh.currentFuel = 10;
                    veh.fuelConsumption = 2;
                    veh.NumberplateText = "TR_" + p.sqlID;                    
                    break;

                case "packer": // 2400
                    if (p.cash < 2400) { MainChat.SendErrorChat(p, "[HATA] Yeterli paranız bulunmuyor."); return; }
                    p.cash -= 2400;
                    p.updateSql();
                    IVehicle v2 = Alt.CreateVehicle(AltV.Net.Enums.VehicleModel.Packer, Positions.truckSpawnPos, new Rotation(0, 0, 0));
                    VehModel veh2 = (VehModel)v2;
                    veh2.sqlID = 200000 + veh2.Id;
                    veh2.SetData("VehicleTempOwner", p.sqlID);
                    veh2.maxFuel = 200;
                    veh2.currentFuel = 10;
                    veh2.fuelConsumption = 2;
                    veh2.NumberplateText = "TR_" + p.sqlID;
                    break;

                case "phantom": // 2700
                    if (p.cash < 2700) { MainChat.SendErrorChat(p, "[HATA] Yeterli paranız bulunmuyor."); return; }
                    p.cash -= 2700;
                    p.updateSql();
                    IVehicle v3 = Alt.CreateVehicle(AltV.Net.Enums.VehicleModel.Phantom, Positions.truckSpawnPos, new Rotation(0, 0, 0));
                    VehModel veh3 = (VehModel)v3;
                    veh3.sqlID = 200000 + veh3.Id;
                    veh3.SetData("VehicleTempOwner", p.sqlID);
                    veh3.maxFuel = 200;
                    veh3.currentFuel = 10;
                    veh3.fuelConsumption = 2;
                    veh3.NumberplateText = "TR_" + p.sqlID;
                    break;

                case "phantom3": // 4000
                    if (p.cash < 4000) { MainChat.SendErrorChat(p, "[HATA] Yeterli paranız bulunmuyor."); return; }
                    p.cash -= 4000;
                    p.updateSql();
                    IVehicle v4 = Alt.CreateVehicle(AltV.Net.Enums.VehicleModel.Phantom3, Positions.truckSpawnPos, new Rotation(0, 0, 0));
                    VehModel veh4 = (VehModel)v4;
                    veh4.sqlID = 200000 + veh4.Id;
                    veh4.SetData("VehicleTempOwner", p.sqlID);
                    veh4.maxFuel = 200;
                    veh4.currentFuel = 10;
                    veh4.fuelConsumption = 2;
                    veh4.NumberplateText = "TR_" + p.sqlID;
                    break;
            }
            GlobalEvents.UINotifiy(p, 1, "Kiralama", selection + " model tiri kiraladiniz");
            return;
        }

        [AsyncClientEvent("Trucker:Event:StartJob")]
        public void EVENT_StartJob(PlayerModel p, string selection)
        {
            if (p.HasData(Datas.jobStarted))
            {
                MainChat.SendErrorChat(p, "[HATA] Zaten başlamış bir göreviniz var lütfen önce onu tamamlayın.");
                return;
            }

            switch (selection)
            {
                case "petrol":
                    IVehicle v = Alt.CreateVehicle(AltV.Net.Enums.VehicleModel.Tanker, Positions.petrolTank_RomorkPos, new Rotation(0, 0, 0));
                    VehModel veh = (VehModel)v;
                    veh.sqlID = 200000 + veh.Id;
                    veh.SetData("VehicleTempOwner", p.sqlID);
                    veh.maxFuel = 200;
                    veh.currentFuel = 10;
                    veh.fuelConsumption = 2;
                    veh.NumberplateText = "TR_" + p.sqlID;

                    p.SetData(Datas.jobStarted, veh.sqlID);
                    p.SetData(Datas.jobFinisPos, Positions.petrolTank_SellPos);
                    GlobalEvents.CreateBlip(p, "Romork", 2, veh.Position, true, 479, 46, "Romork", false, 46, false);
                    GlobalEvents.CreateBlip(p, "RomorkSat", 2, Positions.petrolTank_SellPos, true, 473, 2, "Romork Satis", false, 2);

                    GlobalEvents.UINotifiy(p, 6, "Gorev Basladi", "Gorev Basladi");
                    MainChat.SendInfoChat(p, "[Tırcılık] Göreve başladınız. Römorku alarak satış bölgesine götürün ve /romorksat komutunu kullanın. Römorkunuzu kaybederseniz /agps " + veh.sqlID);
                    return;

                case "car":
                    Random carRnd = new Random();
                    AltV.Net.Enums.VehicleModel carModel = AltV.Net.Enums.VehicleModel.Tr3;
                    switch (carRnd.Next(1, 3))
                    {
                        case 1:
                            carModel = AltV.Net.Enums.VehicleModel.Tr3;
                            break;
                        case 2:
                            carModel = AltV.Net.Enums.VehicleModel.Tr4;
                            break;
                        case 3:
                            carModel = AltV.Net.Enums.VehicleModel.ArmyTrailer2;
                            break;
                    }

                    IVehicle vCar = Alt.CreateVehicle(carModel, Positions.car_RomokrPos, new Rotation(0, 0, 0));
                    VehModel vehCar = (VehModel)vCar;
                    vehCar.sqlID = 200000 + vehCar.Id;
                    vehCar.SetData("VehicleTempOwner", p.sqlID);
                    vehCar.maxFuel = 200;
                    vehCar.currentFuel = 10;
                    vehCar.fuelConsumption = 2;
                    vehCar.NumberplateText = "TR_" + p.sqlID;

                    p.SetData(Datas.jobStarted, vehCar.sqlID);
                    p.SetData(Datas.jobFinisPos, Positions.car_SellPos);
                    GlobalEvents.CreateBlip(p, "Romork", 2, vehCar.Position, true, 479, 46, "Romork", false, 46, false);
                    GlobalEvents.CreateBlip(p, "RomorkSat", 2, Positions.car_SellPos, true, 473, 2, "Romork Satis", false, 2);

                    GlobalEvents.UINotifiy(p, 6, "Gorev Basladi", "Gorev Basladi");
                    MainChat.SendInfoChat(p, "[Tırcılık] Göreve başladınız. Römorku alarak satış bölgesine götürün ve /romorksat komutunu kullanın. Römorkunuzu kaybederseniz /agps " + vehCar.sqlID);
                    return;
                
                default:
                    Random rnd = new Random();
                    AltV.Net.Enums.VehicleModel rModel = AltV.Net.Enums.VehicleModel.Trailers;
                    switch (rnd.Next(0, 6))
                    {
                        case 0:
                            rModel = AltV.Net.Enums.VehicleModel.DockTrailer;
                            break;
                        case 1:
                            rModel = AltV.Net.Enums.VehicleModel.TvTrailer;
                            break;
                        case 2:
                            rModel = AltV.Net.Enums.VehicleModel.TrailerLogs;
                            break;
                        case 3:
                            rModel = AltV.Net.Enums.VehicleModel.Trailers;
                            break;
                        case 4:
                            rModel = AltV.Net.Enums.VehicleModel.Trailers2;
                            break;
                        case 5:
                            rModel = AltV.Net.Enums.VehicleModel.Trailers3;
                            break;
                        case 6:
                            rModel = AltV.Net.Enums.VehicleModel.Trailers4;
                            break;
                        default:
                            rModel = AltV.Net.Enums.VehicleModel.Trailers;
                            break;
                    }

                    Position startPos = Positions.serbestPiyasa_RomorkSpawn[(rnd.Next(0, Positions.serbestPiyasa_RomorkSpawn.Count))];
                    Position finishPos = Positions.serbestPiyasa_RomorkSell[(rnd.Next(0, Positions.serbestPiyasa_RomorkSell.Count))];

                    IVehicle rCar = Alt.CreateVehicle(rModel, startPos, new Rotation(0, 0, 0));
                    VehModel rvehCar = (VehModel)rCar;
                    rvehCar.sqlID = 200000 + rvehCar.Id;
                    rvehCar.SetData("VehicleTempOwner", p.sqlID);
                    rvehCar.maxFuel = 200;
                    rvehCar.currentFuel = 10;
                    rvehCar.fuelConsumption = 2;
                    rvehCar.NumberplateText = "TR_" + p.sqlID;

                    p.SetData(Datas.jobFinisPos, finishPos);
                    p.SetData(Datas.jobStarted, rvehCar.sqlID);
                    GlobalEvents.CreateBlip(p, "Romork", 2, rvehCar.Position, true, 479, 46, "Romork", false, 46, false);
                    GlobalEvents.CreateBlip(p, "RomorkSat", 2, finishPos, true, 473, 2, "Romork Satis", false, 2);

                    GlobalEvents.UINotifiy(p, 6, "Gorev Basladi", "Gorev Basladi");
                    MainChat.SendInfoChat(p, "[Tırcılık] Göreve başladınız. Römorku alarak satış bölgesine götürün ve /romorksat komutunu kullanın. Römorkunuzu kaybederseniz /agps " + rvehCar.sqlID);
                    return;
            }
        }

        [Command("romorksat")]
        public void EVENT_SellRomork(PlayerModel p)
        {
            if(!p.HasData(Datas.jobStarted))
            { MainChat.SendErrorChat(p, "[HATA] Meslek için bir römorkunuz bulunmuyor."); return; }

            if (!p.HasData(Datas.jobFinisPos)) { MainChat.SendErrorChat(p, "[HATA] Meslekle ilgili bir hata meydana geldi."); return; }

            Position finishPos = p.lscGetdata<Position>(Datas.jobFinisPos);
            VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(p.lscGetdata<int>(Datas.jobStarted));
            if(v == null) { MainChat.SendErrorChat(p, "[HATA] Römork bulunamadı!"); return; }
            if (v.Position.Distance(finishPos) > 5) { MainChat.SendErrorChat(p, "[HATA] Römork bitirme noktasında değil!"); return; }
            if (p.Position.Distance(finishPos) > 10) { MainChat.SendErrorChat(p, "[HATA] Görev bitirme noktasında değilsiniz!"); return; }

            switch (v.Model)
            {
                case (uint)AltV.Net.Enums.VehicleModel.Tanker:
                    MainChat.SendInfoChat(p, "[TANKER BİTİRİŞ]");
                    break;

                case (uint)AltV.Net.Enums.VehicleModel.Tr3:
                case (uint)AltV.Net.Enums.VehicleModel.Tr4:
                case (uint)AltV.Net.Enums.VehicleModel.ArmyTrailer:
                    MainChat.SendInfoChat(p, "[ARAÇ BİTİRİŞ]");
                    break;

                case (uint)AltV.Net.Enums.VehicleModel.DockTrailer:
                case (uint)AltV.Net.Enums.VehicleModel.TvTrailer:
                case (uint)AltV.Net.Enums.VehicleModel.TrailerLogs:
                case (uint)AltV.Net.Enums.VehicleModel.Trailers:
                case (uint)AltV.Net.Enums.VehicleModel.Trailers2:
                case (uint)AltV.Net.Enums.VehicleModel.Trailers3:
                case (uint)AltV.Net.Enums.VehicleModel.Trailers4:
                    MainChat.SendInfoChat(p, "[RANDOM BİTİRİŞ]");
                    break;


                
                default:
                    MainChat.SendErrorChat(p, "[DEFAULT DEĞERİ DÖNDÜ HATA!]");
                    break;
            }

            v.DestroyAsync();
            p.DeleteData(Datas.jobStarted);
            p.DeleteData(Datas.jobFinisPos);
            GlobalEvents.DestroyBlip(p, "Romork");
            GlobalEvents.DestroyBlip(p, "RomorkSat");
            GlobalEvents.UINotifiy(p, 8, "Tircilik", "Gorev tamamlandı", "Tecrube", 40, 2);
            return;
        }

    }
}
