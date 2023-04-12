using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Core;
using outRp.Models;
using outRp.OtherSystem.NativeUi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;

namespace outRp.OtherSystem.LSCsystems
{
    public class VehicleVendor : IScript
    {
        public class SellingVehicleModel
        {
            public int ID { get; set; }
            public string model { get; set; }
            public string name { get; set; }
            public int price { get; set; }
            public int inventoryCapacity { get; set; }
            public int petrolTank { get; set; }
            public int fuelConsumption { get; set; }
            public int defaultTax { get; set; }
            public string picture { get; set; }
        }
        public class vendorInfo
        {
            public int type { get; set; }
            public Position buyPos { get; set; }
            public Position spawnPos { get; set; }
        }

        public class AvaibleCars
        {
            public static int[] offroad = new int[] { 1, 2, 3, 4, 5, 6, 17, 20 };
            public static int[] suv = new int[] { 1, 2, 5, 7, 8, 10, 15 };

            // !--
            public static int[] sedan = new int[] { 1, 6, 8, 9, 11, 13, 17 };
            public static int[] muscules = new int[] { 1, 4, 5, 3, 12, 14 };
            public static int[] coupes = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            public static int[] compacts = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // ! --
            public static int[] cycles = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            // !--
            public static int[] motorcycles = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            // !-- 
            public static int[] supercars = new int[] { 2 };
            public static int[] suports = new int[] { 2 };
            public static int[] sportclasics = new int[] { 2 };

            // !--
            public static int[] commersials = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        }


        public static List<vendorInfo> VehicleVendors = new List<vendorInfo>()
        {
            new vendorInfo(){ type = 1, buyPos = new Position(-72.56703f, -1820.822f, 26.937744f), spawnPos = new Position(-55.727474f, -1835.7759f, 26.600708f)

            }, // Arazi Suv
            new vendorInfo(){ type = 2, buyPos = new Position(-82.905495f, -1326.1318f, 29.263062f), spawnPos = new Position(-72.5011f, -1340.4791f, 29.246094f)

            }, // Otomobil galerisi
            new vendorInfo(){ type = 3, buyPos = new Position(305.86813f, -1162.7076f, 29.279907f), spawnPos = new Position(325.1077f, -1163.0901f, 29.279907f)

            }, // bisiklet
            new vendorInfo(){ type = 4, buyPos = new Position(263.7209f, -1155.3231f, 29.279907f), spawnPos = new Position(259.47693f, -1164.145f, 29.161865f)

            }, // Motorcu
            new vendorInfo(){ type = 5, buyPos = new Position(-177, -1158, 23), spawnPos = new Position(-148.54945f, -1164.145f, 25.286499f)

            }, // Spor araç
            new vendorInfo(){ type = 6, buyPos = new Position(477.62637f, -1398.0132f, 31.032227f), spawnPos = new Position(489.56042f, -1401.4286f, 29.313599f )

            }, // ticari            
        };


        public static void LoadVehicleVendor()
        {

            foreach (vendorInfo x in VehicleVendors)
            {
                Textlabels.TextLabelStreamer.Create("~x~[~w~车辆展厅~x~]~n~~g~/~w~buycar", x.buyPos, streamRange: 10);
            }
        }

        [AsyncClientEvent("VehicleVendor:WantBuy")]
        public async Task WantBuyVehicle(PlayerModel p, int Id)
        {
            SellingVehicleModel v = new SellingVehicleModel();
            int type = p.lscGetdata<int>("VehVendorSelected");
            switch (type)
            {
                case 1:
                    v = offRoads.Find(x => x.ID == Id);
                    break;
                case 2:
                    v = suvs.Find(x => x.ID == Id);
                    break;
                case 3:
                    v = sedans.Find(x => x.ID == Id);
                    break;
                case 4:
                    v = muscles.Find(x => x.ID == Id);
                    break;
                case 5:
                    v = coupes.Find(x => x.ID == Id);
                    break;
                case 6:
                    v = compacts.Find(x => x.ID == Id);
                    break;
                case 7:
                    v = cycles.Find(x => x.ID == Id);
                    break;
                case 8:
                    v = motorcycles.Find(x => x.ID == Id);
                    break;
                case 9:
                    v = supercars.Find(x => x.ID == Id);
                    break;
                case 10:
                    v = sports.Find(x => x.ID == Id);
                    break;
                case 11:
                    v = sportclassic.Find(x => x.ID == Id);
                    break;
                case 12:
                    v = jobcars.Find(x => x.ID == Id);
                    break;
                default: return;
            }

            // Company Test
            if (p.HasData("VehicleVendor:Company"))
            {
                Company.systems.sellingPoints.SellPoint point = Company.systems.sellingPoints.GetNearSellPoint(p.Position,
                    p.Dimension, 1);
                if (point == null) { MainChat.SendErrorChat(p, "[错误] 附近没有车辆展厅!"); return; }

                if (point.stock < (v.price / 10)) { MainChat.SendErrorChat(p, "[错误] 产业没有足够的库存了!"); return; }
                if (p.cash < v.price) { Globals.GlobalEvents.notify(p, 3, "您没有足够的钱."); return; }
                p.cash -= v.price;
                await p.updateSql();
                point.stock -= (v.price / 5);
                point.Update();
                Company.Models.CompanyModel company = await Company.Database.BusinessDatabase.GetCompany(point.Owner_Company);
                if (company != null)
                {
                    // Şirket para ekleme!
                    company.BusinessPrice += v.price;
                    company.Cash += v.price - (v.price / 10);
                    company.Update();
                }
            }
            else
            {
                if (v.model == "bmx" || v.model == "cruiser" || v.model == "fixter" || v.model == "scorcher" || v.model == "tribike" ||
                    v.model == "tribike2" || v.model == "tribike3")
                {
                    var account = await Database.DatabaseMain.getAccInfo(p.accountId);
                    if (account == null)
                        return;

                    if (account.lscPoint < 20 || p.cash < v.price) { MainChat.SendErrorChat(p, "[错误] 您没有足够的赞助点购买此车!"); return; }
                    account.lscPoint -= 20;
                    await account.Update();
                }
                if (p.cash < v.price) { Globals.GlobalEvents.notify(p, 3, "您没有足够的钱."); return; }
                p.cash -= v.price;
                await p.updateSql();
            }

            vendorInfo venInfo = new vendorInfo();

            foreach (var g in VehicleVendors)
            {
                if (p.Position.Distance(g.buyPos) < 5) { venInfo = g; }
            }

            IVehicle NewV = Alt.CreateVehicle(v.model, venInfo.spawnPos, new Rotation(0, 0, 0));
            VehModel veh = (VehModel)NewV;
            veh.sqlID = await Database.DatabaseMain.CreateVehicle((VehModel)NewV);
            await veh.SetAppearanceDataAsync("2802_BQUAAW8AACAAAIYUAAAPBgzKlYAAAAAAAAAAAAAAAA==");
            Random pR = new Random();
            char[] chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            veh.NumberplateText = pR.Next(0, 9).ToString() + chars[pR.Next(chars.Length)] + chars[pR.Next(chars.Length)] + veh.sqlID.ToString() + chars[pR.Next(chars.Length)];
            veh.PrimaryColorRgb = new Rgba(255, 255, 255, 255);
            veh.SecondaryColorRgb = new Rgba(255, 255, 255, 255);
            veh.owner = p.sqlID;
            veh.defaultTax = v.defaultTax;
            veh.fuelConsumption = v.fuelConsumption;
            veh.price = v.price;
            veh.inventoryCapacity = v.inventoryCapacity;
            veh.maxFuel = v.petrolTank;
            veh.currentFuel = 10;

            if (veh.Model == (uint)VehicleModel.Bmx || veh.Model == (uint)VehicleModel.Cruiser || veh.Model == (uint)VehicleModel.Fixter || veh.Model == (uint)VehicleModel.Scorcher || veh.Model == (uint)VehicleModel.TriBike || veh.Model == (uint)VehicleModel.TriBike2 || veh.Model == (uint)VehicleModel.TriBike3)
                veh.defaultTax = 0;
            //else               
            //    veh.defaultTax = (veh.price / 1000);

            List<ServerItems> items = new List<ServerItems>();
            ServerItems item = Items.LSCitems.Find(x => x.ID == 20);
            item.amount = 1;
            items.Add(item);
            string json = JsonConvert.SerializeObject(items);
            veh.vehInv = json;
            veh.settings.ModifiyData = veh.AppearanceData;
            veh.settings.SecurityLevel = 1;
            if (p.HasData("VehicleVendor:SellingPos"))
                veh.Position = p.lscGetdata<Position>("VehicleVendor:SellingPos");
            else
            {
                if (venInfo.spawnPos != null && venInfo.spawnPos != new Position(0, 0, 0))
                    veh.Position = venInfo.spawnPos;
                else
                    veh.Position = p.Position;
            }


            veh.Update();

            if (p.HasData("VehicleVendor:Company"))
                p.DeleteData("VehicleVendor:Company");

            if (p.HasData("VehicleVendor:SellingPos"))
                p.DeleteData("VehicleVendor:SellingPos");

            if (p.HasData("VehicleVendor:Type"))
                p.DeleteData("VehicleVendor:Type");


            Globals.GlobalEvents.notify(p, 2, "恭喜您, 购买了车辆.");
            MainChat.SendInfoChat(p, "您可以从旁边的送货区取车.");

            p.EmitLocked("CarShop:Succes");
        }

        [Command("buycar")]
        public static void buyVehicle(PlayerModel p)
        {
            bool canuseGallery = false;
            int type = 0;

            foreach (var g in VehicleVendors)
            {
                if (p.Position.Distance(g.buyPos) < 5)
                {
                    canuseGallery = true; type = g.type;
                    //Globals.GlobalEvents.CreateCamera(p, Core.Events.GetTruePosition(g.spawnPos, new Rotation(0, 0, 0), 0, 10.0f), new Vector3(0, 0, 0));
                    //await Task.Delay(200);
                    //Globals.GlobalEvents.LookCamera(p, g.spawnPos);
                }
            }
            if (!canuseGallery) { MainChat.SendErrorChat(p, "[错误] 附近没有车辆展厅."); return; }


            List<GuiMenu> gMenu = new List<GuiMenu>();

            switch (type)
            {
                case 1:
                    GuiMenu offroad = new GuiMenu { name = "越野", triger = "ShowCar:SelectedList", value = "1" };// old 1
                    GuiMenu suv = new GuiMenu { name = "SUV", triger = "ShowCar:SelectedList", value = "2" };// old 2
                    gMenu.Add(offroad);
                    gMenu.Add(suv);
                    break;
                case 2:
                    GuiMenu sedans = new GuiMenu { name = "轿车", triger = "ShowCar:SelectedList", value = "3" };// old 3
                    GuiMenu muscules = new GuiMenu { name = "肌肉车", triger = "ShowCar:SelectedList", value = "4" };// old 4
                    GuiMenu coupes = new GuiMenu { name = "两门轿车", triger = "ShowCar:SelectedList", value = "5" }; // old 5
                    GuiMenu compacts = new GuiMenu { name = "小型轿车", triger = "ShowCar:SelectedList", value = "6" }; // old 6
                    gMenu.Add(sedans);
                    gMenu.Add(muscules);
                    gMenu.Add(coupes);
                    gMenu.Add(compacts);
                    break;

                case 3:
                    GuiMenu cycles = new GuiMenu { name = "自行车", triger = "ShowCar:SelectedList", value = "7" };// old 7
                    gMenu.Add(cycles);
                    break;

                case 4:
                    GuiMenu motocycles = new GuiMenu { name = "摩托车", triger = "ShowCar:SelectedList", value = "8" };// old 8
                    gMenu.Add(motocycles);
                    break;

                case 5:
                    MainChat.SendErrorChat(p, "[车展厅] 此类别暂时禁用, 待调整后出售.");
                    //GuiMenu supercars = new GuiMenu { name = "超跑", triger = "ShowCar:SelectedList", value = "9" };// old 9
                    //gMenu.Add(supercars);
                    //GuiMenu sports = new GuiMenu { name = "跑车", triger = "ShowCar:SelectedList", value = "10" }; // old 10
                    //GuiMenu sportclassic = new GuiMenu { name = "经典跑车", triger = "ShowCar:SelectedList", value = "11" };// old 11
                    //gMenu.Add(sports);
                    //gMenu.Add(sportclassic);
                    break;

                case 6:
                    GuiMenu commersials = new GuiMenu { name = "商业用车", triger = "ShowCar:SelectedList", value = "12" }; // old 12
                    gMenu.Add(commersials);
                    break;    //
            }

            GuiMenu close = GuiEvents.closeItem;
            gMenu.Add(close);

            Gui y = new Gui()
            {
                image = "https://www.upload.ee/files/12278457/1.png.html",
                guiMenu = gMenu,
                color = "#4AC27D"
            };
            y.Send(p);
        }

        [AsyncClientEvent("ShowCar:SelectedList")]
        public static void ShowSelectedCarList(PlayerModel p, int val)
        {
            if (val == 13) { MainChat.SendErrorChat(p, "[车展厅] 此类别禁用."); return; }
            vendorInfo ven = new vendorInfo();
            List<SellingVehicleModel> list = new List<SellingVehicleModel>();

            int selected = 0;
            Position camPos = new Position(0, 0, 0);

            //yeni
            if (!p.HasData("VehicleVendor:Company"))
            {
                foreach (var g in VehicleVendors)
                {
                    if (p.Position.Distance(g.buyPos) < 15) { ven = g; camPos = g.spawnPos; }
                }
            }
            else
            {
                if (p.HasData("VehicleVendor:SellingPos"))
                    camPos = p.lscGetdata<Position>("VehicleVendor:SellingPos");

                ven = VehicleVendors.Find(x => x.type == p.lscGetdata<int>("VehicleVendor:Type"));
            }

            // Yeni
            if (camPos.Distance(new Position(0, 0, 0)) < 5)
            {
                MainChat.SendErrorChat(p, "[错误] 尚未完成加载!");
                if (p.HasData("VehicleVendor:Company"))
                    p.DeleteData("VehicleVendor:Company");

                if (p.HasData("VehicleVendor:SellingPos"))
                    p.DeleteData("VehicleVendor:SellingPos");

                if (p.HasData("VehicleVendor:Type"))
                    p.DeleteData("VehicleVendor:Type");

                return;
            }

            switch (val)
            {
                case 1:
                    list = offRoads;
                    selected = 1;
                    break;

                case 2:
                    /* foreach (int number in AvaibleCars.suv)
                    {
                        list.Add(suvs.Find(x => x.ID == number));
                    }   */
                    list = suvs;
                    selected = 2;
                    break;

                case 3:
                    list = sedans;
                    selected = 3;
                    break;

                case 4:
                    list = muscles;
                    selected = 4;
                    break;

                case 5:
                    list = coupes;
                    selected = 5;
                    break;

                case 6:
                    list = compacts;
                    selected = 6;
                    break;

                case 7:
                    list = cycles;
                    selected = 7;
                    break;

                case 8:
                    list = motorcycles;
                    selected = 8;
                    break;

                case 9:
                    //list = supercars;
                    //selected = 9;
                    break;

                case 10:
                    //list = sports;
                    //selected = 10;
                    break;
                //---
                case 11:
                    //list = sportclassic;
                    //selected = 11;
                    break;

                case 12:
                    list = jobcars;
                    selected = 12;
                    break;
            }


            p.lscSetData("VehVendorSelected", selected);

            GuiEvents.GUIMenu_Close(p);
            //Globals.GlobalEvents.GameControls(p, true);
            Globals.GlobalEvents.FreezeEntity(p, true);
            string json = JsonConvert.SerializeObject(list);

            if (p.HasData("VehicleVendor:SellingPos"))
                p.EmitLocked("CarShop:Show", json, p.lscGetdata<Position>("VehicleVendor:SellingPos"));
            else p.EmitLocked("CarShop:Show", json, camPos);
            //p.Emit("VehicleVendor:Show", selected, json); 
            return;
        }

        [AsyncClientEvent("CarShop:Close")]
        public static void CarShop_Close(PlayerModel p)
        {
            //Globals.GlobalEvents.CloseCamera(p);
            Globals.GlobalEvents.GameControls(p, true);
            Globals.GlobalEvents.FreezeEntity(p, false);
            if (p.HasData("VehicleVendor:Company"))
                p.DeleteData("VehicleVendor:Company");

            if (p.HasData("VehicleVendor:SellingPos"))
                p.DeleteData("VehicleVendor:SellingPos");

            if (p.HasData("VehicleVendor:Type"))
                p.DeleteData("VehicleVendor:Type");
        }

        /*public static void ShowCarList(PlayerModel p)
        {
            vendorInfo i = VehicleVendors.Find(x => x.buyPos.Distance(p.Position) < 5);
            if (i == null) { MainChat.SendErrorChat(p, "[HATA] Bu komutu kullanabilmek için araç satın alma noktasında olmalısınız!"); return; }
            List<SellingVehicleModel> list = new List<SellingVehicleModel>();
            switch (i.type)
            {
                case 1:
                    //list = musculeCarList;
                    break;

                default:
                    break;
            }
            Globals.GlobalEvents.GameControls(p, false);
            string json = JsonConvert.SerializeObject(list);
            Alt.Log(json);
            p.Emit("VehicleVendor:Show", i.type, json);
        }

        */

        public static SellingVehicleModel getModelWithName(string name)
        {
            SellingVehicleModel result = null;
            result = offRoads.Find(x => x.model == name);
            if (result != null)
                return result;

            result = suvs.Find(x => x.model == name);
            if (result != null)
                return result;

            result = compacts.Find(x => x.model == name);
            if (result != null)
                return result;

            result = coupes.Find(x => x.model == name);
            if (result != null)
                return result;

            result = muscles.Find(x => x.model == name);
            if (result != null)
                return result;

            result = sedans.Find(x => x.model == name);
            if (result != null)
                return result;

            result = cycles.Find(x => x.model == name);
            if (result != null)
                return result;

            result = motorcycles.Find(x => x.model == name);
            if (result != null)
                return result;

            result = sportclassic.Find(x => x.model == name);
            if (result != null)
                return result;

            result = sports.Find(x => x.model == name);
            if (result != null)
                return result;

            result = supercars.Find(x => x.model == name);
            if (result != null)
                return result;

            result = jobcars.Find(x => x.model == name);
            if (result != null)
                return result;

            return result;
        }

        // TODO ARAZI & SUV 
        public static List<SellingVehicleModel> offRoads = new List<SellingVehicleModel>()
        {
            new SellingVehicleModel(){ ID = 1, model = "rumpo3", name = "Bravado Rumpo Custom", price = 50000, inventoryCapacity = 780, defaultTax = 40, petrolTank = 70, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/7/7a/Rumpo3.png" },
            new SellingVehicleModel(){ ID = 2, model = "caracara2", name = "威皮 Caracara 4x4", price = 73000, inventoryCapacity = 420, defaultTax = 65, petrolTank = 70, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/3/3b/Caracara2.png" },
            new SellingVehicleModel(){ ID = 3, model = "everon", name = "Karin Everon", price = 92500, inventoryCapacity = 420, defaultTax = 80, petrolTank = 70, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/8/83/Everon.png" },
            new SellingVehicleModel(){ ID = 4, model = "hellion", name = "Annis Hallion", price = 40000, inventoryCapacity = 280, defaultTax = 40, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/c/cf/Hellion.png" },
            new SellingVehicleModel(){ ID = 5, model = "rancherxl", name = "Declasse Rancher XL", price = 25000, inventoryCapacity = 350, defaultTax = 25, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/e/ef/Rancherxl.png" },
            new SellingVehicleModel(){ ID = 6, model = "rebel", name = "Karin Rusty Rebel", price = 17000, inventoryCapacity = 260, defaultTax = 17, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/e/e3/Rebel.png" },
            new SellingVehicleModel(){ ID = 7, model = "rebel2", name = "Karin Rebel", price = 28500, inventoryCapacity = 360, defaultTax = 28, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/9/90/Rebel2.png" },
           new SellingVehicleModel(){ ID = 8, model = "freecrawler", name = "Canis Freecrawler", price = 90000, inventoryCapacity = 100, defaultTax = 90, petrolTank = 67, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/thumb/5/56/Freecrawler.png/800px-Freecrawler.png" },
           new SellingVehicleModel(){ ID = 9, model = "sadler", name = "威皮 Sadler", price = 30000, inventoryCapacity = 130, defaultTax = 7, petrolTank = 90, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/3/37/Sadler.png" },
        };

        public static List<SellingVehicleModel> suvs = new List<SellingVehicleModel>()
        {
            new SellingVehicleModel(){ ID = 1, model = "baller", name = "Gallivanter Baller", price = 80000, inventoryCapacity = 220, defaultTax = 80, petrolTank = 70, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/7/79/Baller.png" },
            new SellingVehicleModel(){ ID = 2, model = "baller2", name = "Gallivanter Baller II", price = 120000, inventoryCapacity = 260, defaultTax = 120, petrolTank = 70, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/9/98/Baller2.png" },
            new SellingVehicleModel(){ ID = 3, model = "baller3", name = "Gallivanter Baller LE", price = 130000, inventoryCapacity = 220, defaultTax = 130, petrolTank = 70, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/9/9b/Baller3.png" },
            new SellingVehicleModel(){ ID = 4, model = "baller4", name = "Gallivanter Baller LWB", price = 140000, inventoryCapacity = 250, defaultTax = 140, petrolTank = 70, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/e/e8/Baller4.png" },
            new SellingVehicleModel(){ ID = 5, model = "bjxl", name = "Karin BeeJay XL", price = 75000, inventoryCapacity = 200, defaultTax = 58, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/d/da/Bjxl.png" },
            new SellingVehicleModel(){ ID = 6, model = "cavalcade", name = "Albany Cavalcade", price = 90000, inventoryCapacity = 250, defaultTax = 90, petrolTank = 70, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/9/9e/Cavalcade.png" },
            new SellingVehicleModel(){ ID = 7, model = "cavalcade2", name = "Albany Cavalcade II", price = 120000, inventoryCapacity = 230, defaultTax = 120, petrolTank = 80, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/b/bc/Cavalcade2.png" },
            new SellingVehicleModel(){ ID = 8, model = "dubsta", name = "Benefactor Dubsta", price = 150000, inventoryCapacity = 250, defaultTax = 150, petrolTank = 70, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/e/ec/Dubsta.png" },
            new SellingVehicleModel(){ ID = 9, model = "fq2", name = "Fathom FQ 2", price = 70000, inventoryCapacity = 200, defaultTax = 55, petrolTank = 70, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/a/a4/Fq2.png" },
            new SellingVehicleModel(){ ID = 10, model = "granger", name = "Declasse Granger", price = 120000, inventoryCapacity = 260, defaultTax = 120, petrolTank = 70, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/4/4d/Granger.png" },
            new SellingVehicleModel(){ ID = 11, model = "gresley", name = "Bravado Gresley", price = 65000, inventoryCapacity = 240, defaultTax = 65, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/d/d6/Gresley.png" },
            new SellingVehicleModel(){ ID = 12, model = "habanero", name = "Emperor Habanero", price = 40000, inventoryCapacity = 230, defaultTax = 40, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/1/1e/Habanero.png" },
            new SellingVehicleModel(){ ID = 13, model = "huntley", name = "埃努斯 Huntley S", price = 85000, inventoryCapacity = 230, defaultTax = 85, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/1/1c/Huntley.png" },
            new SellingVehicleModel(){ ID = 14, model = "landstalker", name = "Dundreary Landstalker", price = 95000, inventoryCapacity = 250, defaultTax = 95, petrolTank = 70, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/7/70/Landstalker.png" },
            new SellingVehicleModel(){ ID = 15, model = "landstalker2", name = "Dundreary Landstalker XL", price = 130000, inventoryCapacity = 280, defaultTax = 130, petrolTank = 80, fuelConsumption = 1, picture = "https://wiki.rage.mp/images/b/b1/Landstalker2.png" },
            new SellingVehicleModel(){ ID = 16, model = "mesa", name = "Canis Mesa", price = 32000, inventoryCapacity = 160, defaultTax = 25, petrolTank = 70, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/c/cc/Mesa.png" },
            new SellingVehicleModel(){ ID = 17, model = "novak", name = "Lampadati Novak", price = 150000, inventoryCapacity = 230, defaultTax = 150, petrolTank = 70, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/7/76/Novak.png" },
            new SellingVehicleModel(){ ID = 18, model = "patriot", name = "Mammoth Patriot", price = 150000, inventoryCapacity = 250, defaultTax = 150, petrolTank = 80, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/1/12/Patriot.png" },
            new SellingVehicleModel(){ ID = 19, model = "patriot2", name = "Mammoth Patriot Stretch", price = 500000, inventoryCapacity = 160, defaultTax = 500, petrolTank = 80, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/8/83/Patriot2.png" },
            new SellingVehicleModel(){ ID = 20, model = "radi", name = "威皮 Radius", price = 23000, inventoryCapacity = 190, defaultTax = 15, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/9/9c/Radi.png" },
            new SellingVehicleModel(){ ID = 21, model = "rebla", name = "Ubermacht Rebla GTS ", price = 180000, inventoryCapacity = 320, defaultTax = 180, petrolTank = 80, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/9/94/Rebla.png" },
            new SellingVehicleModel(){ ID = 22, model = "rocoto", name = "Obey Rocoto", price = 74000, inventoryCapacity = 230, defaultTax = 58, petrolTank = 70, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/2/23/Rocoto.png" },
            new SellingVehicleModel(){ ID = 23, model = "seminole", name = "Canis Seminole", price = 45500, inventoryCapacity = 240, defaultTax = 32, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/c/cc/Seminole.png" },
            new SellingVehicleModel(){ ID = 24, model = "seminole2", name = "Seminole Frontier", price = 34500, inventoryCapacity = 250, defaultTax = 20, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.rage.mp/images/d/d9/Seminole2.png" },
            new SellingVehicleModel(){ ID = 25, model = "serrano", name = "Benefactor Serrano", price = 35000, inventoryCapacity = 240, defaultTax = 22, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/a/ac/Serrano.png" },
            new SellingVehicleModel(){ ID = 26, model = "toros", name = "Pegassi Toros", price = 220000, inventoryCapacity = 250, defaultTax = 220, petrolTank = 100, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/f/f1/Toros.png" },
            new SellingVehicleModel(){ ID = 27, model = "xls", name = "Benefactor XLS", price = 115000, inventoryCapacity = 240, defaultTax = 115, petrolTank = 80, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/0/0f/Xls.png" },
            //new SellingVehicleModel(){ ID = 28, model = "baller5", name = "Gallivanter Baller LE (Armored)", price = 1, inventoryCapacity = 220, defaultTax = 0, petrolTank = 85, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/3/34/Baller5.png" },
           // new SellingVehicleModel(){ ID = 29, model = "baller6", name = "Gallivanter Baller LE LWB (Armored)", price = 1, inventoryCapacity = 250, defaultTax = 0, petrolTank = 90, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/8/81/Baller6.png" },
            new SellingVehicleModel(){ ID = 28, model = "dubsta2", name = "Benefactor Dubsta 2", price = 150000, inventoryCapacity = 250, defaultTax = 150, petrolTank = 90, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/3/36/Dubsta2.png" },
            new SellingVehicleModel(){ ID = 29, model = "iwagen", name = "Obey Iwagen", price = 140000, inventoryCapacity = 170, defaultTax = 130, petrolTank = 80, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/2/2e/Contender.png" },
            new SellingVehicleModel(){ ID = 30, model = "astron", name = "Pfister Astron", price = 120000, inventoryCapacity = 170, defaultTax = 130, petrolTank = 80, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/2/2e/Contender.png" },
            new SellingVehicleModel(){ ID = 31, model = "baller7", name = "Gallivanter Baller ST", price = 180000, inventoryCapacity = 170, defaultTax = 120, petrolTank = 80, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/2/2e/Contender.png" },
            new SellingVehicleModel(){ ID = 32, model = "granger2", name = "Declasse Granger 3600LX", price = 130000, inventoryCapacity = 200, defaultTax = 130, petrolTank = 80, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/2/2e/Contender.png" },
            new SellingVehicleModel(){ ID = 33, model = "jubilee", name = "埃努斯 Jubilee", price = 270000, inventoryCapacity = 200, defaultTax = 220, petrolTank = 100, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/2/2e/Contender.png" },
            //new SellingVehicleModel(){ ID = 36, model = "xls2", name = "Benefactor XLS (Armored)", price = 1, inventoryCapacity = 230, defaultTax = 0, petrolTank = 85, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/3/35/Xls2.png" },
            
        };

        // TODO Binek otomobil

        public static List<SellingVehicleModel> compacts = new List<SellingVehicleModel>()
        {
            // new SellingVehicleModel(){ ID = 1, model = "asbo", name = "Maxwell Asbo", price = 500, inventoryCapacity = 130, defaultTax = 8, petrolTank = 55, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/c/cb/Asbo.png" },
            new SellingVehicleModel(){ ID = 1, model = "blista", name = "Dinka Blista", price = 25000, inventoryCapacity = 140, defaultTax = 8, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/5/58/Blista.png" },
            new SellingVehicleModel(){ ID = 2, model = "blista2", name = "Dinka Blista Compact", price = 19000, inventoryCapacity = 150, defaultTax = 6, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/0/0c/Blista2.png" },
            new SellingVehicleModel(){ ID = 3, model = "brioso", name = "Grotti Brioso R/A", price = 50000, inventoryCapacity = 90, defaultTax = 20, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/6/6f/Brioso.png" },
            new SellingVehicleModel(){ ID = 4, model = "dilettante", name = "Karin Dilettante", price = 20000, inventoryCapacity = 160, defaultTax = 7, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/b/b9/Dilettante.png" },
            new SellingVehicleModel(){ ID = 5, model = "issi2", name = "Weeny Issi", price = 43500, inventoryCapacity = 130, defaultTax = 16, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/0/0b/Issi2.png" },
            new SellingVehicleModel(){ ID = 6, model = "club", name = "BF Club", price = 17900, inventoryCapacity = 110, defaultTax = 6, petrolTank = 55, fuelConsumption = 2, picture = "https://wiki.rage.mp/images/5/50/Club.png" },
            new SellingVehicleModel(){ ID = 7, model = "kanjo", name = "Dinka Blista Kanjo", price = 21500, inventoryCapacity = 120, defaultTax = 7, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/2/25/Kanjo.png" },
            new SellingVehicleModel(){ ID = 8, model = "panto", name = "Benefactor Panto", price = 29000, inventoryCapacity = 60, defaultTax = 9, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/e/e5/Panto.png" },
            new SellingVehicleModel(){ ID = 9, model = "prairie", name = "Bollokan Prairie", price = 22500, inventoryCapacity = 130, defaultTax = 7, petrolTank = 55, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/3/3d/Prairie.png" },
            new SellingVehicleModel(){ ID = 10, model = "rhapsody", name = "Declasse Rhapsody", price = 14700, inventoryCapacity = 100, defaultTax = 6, petrolTank = 55, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/e/e2/Rhapsody.png" },
        };

        public static List<SellingVehicleModel> coupes = new List<SellingVehicleModel>()
        {
            new SellingVehicleModel(){ ID = 1, model = "cogcabrio", name = "埃努斯 Cognoscenti Cabrio", price = 98740, inventoryCapacity = 190, defaultTax = 74, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/d/d0/Cogcabrio.png" },
            new SellingVehicleModel(){ ID = 2, model = "exemplar", name = "Dewbauchee Exemplar", price = 75000, inventoryCapacity = 220, defaultTax = 56, petrolTank = 70, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/a/a4/Exemplar.png" },
            new SellingVehicleModel(){ ID = 3, model = "f620", name = "Ocelot F620", price = 100000, inventoryCapacity = 200, defaultTax = 75, petrolTank = 70, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/2/21/F620.png" },
            new SellingVehicleModel(){ ID = 4, model = "felon", name = "Lampadati Felon", price = 65000, inventoryCapacity = 220, defaultTax = 43, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/0/04/Felon.png" },
            new SellingVehicleModel(){ ID = 5, model = "felon2", name = "Lampadati Felon GT", price = 80000, inventoryCapacity = 190, defaultTax = 50, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/6/6e/Felon2.png" },
            new SellingVehicleModel(){ ID = 6, model = "jackal", name = "Ocelot Jackal", price = 69500, inventoryCapacity = 220, defaultTax = 52, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/7/70/Jackal.png" },
            new SellingVehicleModel(){ ID = 7, model = "oracle2", name = "Übermacht Oracle", price = 55000, inventoryCapacity = 250, defaultTax = 41, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/4/4d/Oracle2.png" },
            new SellingVehicleModel(){ ID = 8, model = "oracle", name = "Übermacht Oracle XS", price = 40000, inventoryCapacity = 230, defaultTax = 30, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/1/17/Oracle.png" },
            new SellingVehicleModel(){ ID = 9, model = "sentinel", name = "Übermacht Sentinel XS", price = 60000, inventoryCapacity = 210, defaultTax = 45, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/9/91/Sentinel.png" },
            new SellingVehicleModel(){ ID = 10, model = "sentinel2", name = "Übermacht Sentinel Cabrio", price = 70000, inventoryCapacity = 210, defaultTax = 53, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/d/dd/Sentinel2.png" },
            new SellingVehicleModel(){ ID = 11, model = "zion", name = "Übermacht Zion", price = 75000, inventoryCapacity = 210, defaultTax = 56, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/5/51/Zion.png" },
            new SellingVehicleModel(){ ID = 12, model = "zion2", name = "Übermacht Zion Cabrio", price = 80000, inventoryCapacity = 210, defaultTax = 60, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/f/f2/Zion2.png" },
            new SellingVehicleModel(){ ID = 13, model = "windsor", name = "埃努斯 Windsor", price = 175000, inventoryCapacity = 220, defaultTax = 150, petrolTank = 70, fuelConsumption = 1, picture = "https://wiki.rage.mp/images/2/2d/Windsor.png" },
            new SellingVehicleModel(){ ID = 14, model = "windsor2", name = "埃努斯 Windsor Drop", price = 280000, inventoryCapacity = 220, defaultTax = 240, petrolTank = 80, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/5/57/Windsor2.png" },
            new SellingVehicleModel(){ ID = 15, model = "kanjosj", name = "Dinka Kanjo SJ", price = 35000, inventoryCapacity = 220, defaultTax = 35, petrolTank = 70, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/5/57/Windsor2.png" },

        };

        public static List<SellingVehicleModel> muscles = new List<SellingVehicleModel>()
        {
            new SellingVehicleModel(){ ID = 1, model = "impaler", name = "Declasse Impaler", price = 30000, inventoryCapacity = 300, defaultTax = 12, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/3/31/Impaler.png" },
            new SellingVehicleModel(){ ID = 2, model = "stalion", name = "Declasse Stallion", price = 17000, inventoryCapacity = 330, defaultTax = 6, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/c/ce/Stalion.png" },
            new SellingVehicleModel(){ ID = 3, model = "tampa", name = "Declasse Tampa", price = 14450, inventoryCapacity = 280, defaultTax = 5, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/3/30/Tampa.png" },
            new SellingVehicleModel(){ ID = 4, model = "sabregt", name = "Declasse Sabre Turbo", price = 20000, inventoryCapacity = 320, defaultTax = 7, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/0/04/Sabregt.png" },
            new SellingVehicleModel(){ ID = 5, model = "sabregt2", name = "Declasse Sabre Turbo Custom", price = 33000, inventoryCapacity = 280, defaultTax = 10, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/b/ba/Sabregt2.png" },
            new SellingVehicleModel(){ ID = 6, model = "picador", name = "Cheval Picador", price = 15000, inventoryCapacity = 450, defaultTax = 6, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/1/15/Picador.png" },
            new SellingVehicleModel(){ ID = 7, model = "blade", name = "威皮 Blade", price = 27000, inventoryCapacity = 280, defaultTax = 10, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/a/ad/Blade.png" },
            new SellingVehicleModel(){ ID = 8, model = "buccaneer", name = "Albany Bucanneer", price = 18000, inventoryCapacity = 300, defaultTax = 7, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/d/de/Buccaneer.png" },
            new SellingVehicleModel(){ ID = 9, model = "buccaneer2", name = "Albany Bucanneer Custom", price = 32000, inventoryCapacity = 300, defaultTax = 10, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/1/16/Buccaneer2.png" },
            new SellingVehicleModel(){ ID = 10, model = "chino", name = "威皮 Chino", price = 18000, inventoryCapacity = 260, defaultTax = 7, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/4/4d/Chino.png" },
            new SellingVehicleModel(){ ID = 11, model = "chino2", name = "威皮 Chino Custom", price = 30000, inventoryCapacity = 260, defaultTax = 12, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/6/61/Chino2.png" },
            new SellingVehicleModel(){ ID = 12, model = "deviant", name = "施特 Deviant", price = 47000, inventoryCapacity = 310, defaultTax = 18, petrolTank = 70, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/d/d5/Deviant.png" },
            new SellingVehicleModel(){ ID = 13, model = "dukes", name = "Imponte Dukes", price = 23000, inventoryCapacity = 300, defaultTax = 10, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/6/6e/Dukes.png" },
            new SellingVehicleModel(){ ID = 14, model = "dukes3", name = "Imponte Dukes V8", price = 30000, inventoryCapacity = 300, defaultTax = 14, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.rage.mp/images/4/45/Dukes3.png" },
            new SellingVehicleModel(){ ID = 15, model = "ellie", name = "威皮 Ellie", price = 45000, inventoryCapacity = 300, defaultTax = 20, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/e/ef/Ellie.png" },
            new SellingVehicleModel(){ ID = 16, model = "faction", name = "Willard Faction", price = 20000, inventoryCapacity = 250, defaultTax = 9, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/5/55/Faction.png" },
            new SellingVehicleModel(){ ID = 17, model = "faction2", name = "Willard Faction Custom", price = 30000, inventoryCapacity = 250, defaultTax = 12, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/4/41/Faction2.png" },
            new SellingVehicleModel(){ ID = 18, model = "gauntlet", name = "Bravado Gauntlet", price = 50000, inventoryCapacity = 270, defaultTax = 24, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/7/71/Gauntlet.png" },
            new SellingVehicleModel(){ ID = 19, model = "dominator", name = "威皮 Dominator", price = 65000, inventoryCapacity = 250, defaultTax = 35, petrolTank = 70, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/6/6e/Dominator.png" },
            new SellingVehicleModel(){ ID = 20, model = "dominator3", name = "威皮 Dominator GTX", price = 70000, inventoryCapacity = 280, defaultTax = 45, petrolTank = 70, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/b/ba/Dominator3.png" },
            new SellingVehicleModel(){ ID = 21, model = "phoenix", name = "Imponte Phoenix", price = 27000, inventoryCapacity = 260, defaultTax = 10, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/c/c1/Phoenix.png" },
            new SellingVehicleModel(){ ID = 22, model = "nightshade", name = "Imponte Nightshade", price = 50000, inventoryCapacity = 230, defaultTax = 30, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/e/ec/Nightshade.png" },
            new SellingVehicleModel(){ ID = 23, model = "moonbeam", name = "Declasse Moonbeam", price = 19500, inventoryCapacity = 450, defaultTax = 8, petrolTank = 70, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/5/5d/Moonbeam.png" },
            new SellingVehicleModel(){ ID = 24, model = "moonbeam2", name = "Declasse Moonbeam Custom", price = 29000, inventoryCapacity = 380, defaultTax = 12, petrolTank = 70, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/c/c1/Moonbeam2.png" },
            new SellingVehicleModel(){ ID = 25, model = "ruiner", name = "Imponte Ruiner", price = 30000, inventoryCapacity = 260, defaultTax = 30, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/b/b4/Ruiner.png" },
            new SellingVehicleModel(){ ID = 26, model = "voodoo2", name = "Declasse Rusty Voodoo", price = 5000, inventoryCapacity = 270, defaultTax = 2, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/5/5e/Voodoo2.png" },
            new SellingVehicleModel(){ ID = 27, model = "voodoo", name = "Declasse Voodoo Custom", price = 24000, inventoryCapacity = 270, defaultTax = 10, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/1/1e/Voodoo.png" },
            new SellingVehicleModel(){ ID = 28, model = "vigero", name = "Declasse Vigero", price = 21000, inventoryCapacity = 230, defaultTax = 9, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/2/22/Vigero.png" },
            new SellingVehicleModel(){ ID = 29, model = "virgo", name = "Albany Virgo", price = 20500, inventoryCapacity = 240, defaultTax = 8, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/4/41/Virgo.png" },
            new SellingVehicleModel(){ ID = 30, model = "virgo2", name = "Dundreary Virgo Classic Custom", price = 17000, inventoryCapacity = 240, defaultTax = 7, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/8/81/Virgo2.png" },
            new SellingVehicleModel(){ ID = 31, model = "virgo3", name = "Dundreary Virgo Classic", price = 16000, inventoryCapacity = 240, defaultTax = 6, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/4/4d/Virgo3.png" },
            new SellingVehicleModel(){ ID = 32, model = "tulip", name = "Declasse Tulip", price = 22000, inventoryCapacity = 270, defaultTax = 9, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/c/cf/Tulip.png" },
            new SellingVehicleModel(){ ID = 33, model = "vamos", name = "Declasse Vamos", price = 24750, inventoryCapacity = 230, defaultTax = 25, petrolTank = 65, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/0/07/Vamos.png" },
            new SellingVehicleModel(){ ID = 34, model = "buffalo", name = "Bravado Buffalo", price = 32000, inventoryCapacity = 240, defaultTax = 15, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/0/0a/Buffalo.png" },
            new SellingVehicleModel(){ ID = 35, model = "buffalo2", name = "Bravado Buffalo S", price = 42000, inventoryCapacity = 240, defaultTax = 25, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/2/2c/Buffalo2.png" },
            new SellingVehicleModel(){ ID = 36, model = "gauntlet3", name = "Bravado Gauntlet Classic", price = 42000, inventoryCapacity = 230, defaultTax = 25, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/8/88/Gauntlet3.png" },
            new SellingVehicleModel(){ ID = 37, model = "gauntlet4", name = "Bravado Gauntlet Hellfire", price = 90000, inventoryCapacity = 250, defaultTax = 75, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/e/ec/Gauntlet4.png" },
            new SellingVehicleModel(){ ID = 38, model = "yosemite3", name = "Declasse Yosemite Rancher", price = 29500, inventoryCapacity = 370, defaultTax = 14, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.rage.mp/images/8/87/Yosemite3.png" },
            new SellingVehicleModel(){ ID = 39, model = "vamos", name = "Declasse Vamos", price = 40000, inventoryCapacity = 50, defaultTax = 23, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/thumb/0/07/Vamos.png/800px-Vamos.png" },
            new SellingVehicleModel(){ ID = 40, model = "buffalo4", name = "Bravado Buffalo STX", price = 130000, inventoryCapacity = 170, defaultTax = 100, petrolTank = 80, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/thumb/0/07/Vamos.png/800px-Vamos.png" },
            new SellingVehicleModel(){ ID = 41, model = "greenwood", name = "Bravado Greenwood", price = 60000, inventoryCapacity = 200, defaultTax = 75, petrolTank = 80, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/thumb/0/07/Vamos.png/800px-Vamos.png" },
            new SellingVehicleModel(){ ID = 42, model = "vigero2", name = "Declasse Vigero ZX", price = 130000, inventoryCapacity = 170, defaultTax = 100, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/thumb/0/07/Vamos.png/800px-Vamos.png" },
        };

        public static List<SellingVehicleModel> sedans = new List<SellingVehicleModel>()
        {
            new SellingVehicleModel(){ ID = 1, model = "emperor2", name = "Albany Rusty Emperor", price = 5500, inventoryCapacity = 150, defaultTax = 2, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/1/16/Emperor2.png" },
            new SellingVehicleModel(){ ID = 2, model = "regina", name = "Dundreary Regina", price = 8500, inventoryCapacity = 190, defaultTax = 4, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/5/5f/Regina.png" },
            new SellingVehicleModel(){ ID = 3, model = "emperor", name = "Albany Emperor", price = 15000, inventoryCapacity = 150, defaultTax = 8, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/c/c5/Emperor.png" },
            new SellingVehicleModel(){ ID = 4, model = "ingot", name = "Vulcar Ingot", price = 13000, inventoryCapacity = 200, defaultTax = 6, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/7/74/Ingot.png" },
            new SellingVehicleModel(){ ID = 5, model = "stratum", name = "Zirconium Stratum", price = 19000, inventoryCapacity = 190, defaultTax = 8, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/b/ba/Stratum.png" },
            new SellingVehicleModel(){ ID = 6, model = "glendale", name = "Benefactor Glendale", price = 15750, inventoryCapacity = 160, defaultTax = 6, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/b/b2/Glendale.png" },
            new SellingVehicleModel(){ ID = 7, model = "surge", name = "Cheval Surge", price = 16000, inventoryCapacity = 130, defaultTax = 6, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/1/1a/Surge.png" },
            new SellingVehicleModel(){ ID = 8, model = "intruder", name = "Karin Intruder", price = 22600, inventoryCapacity = 160, defaultTax = 10, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/9/92/Intruder.png" },
            new SellingVehicleModel(){ ID = 9, model = "primo", name = "Albany Primo", price = 17500, inventoryCapacity = 140, defaultTax = 8, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/4/45/Primo.png" },
            new SellingVehicleModel(){ ID = 10, model = "premier", name = "Declasse Premier", price = 16700, inventoryCapacity = 150, defaultTax = 6, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/9/9d/Premier.png" },
            new SellingVehicleModel(){ ID = 11, model = "stanier", name = "威皮 Stanier", price = 18000, inventoryCapacity = 140, defaultTax = 8, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/5/57/Stanier.png" },
            new SellingVehicleModel(){ ID = 12, model = "washington", name = "Albany Washington", price = 21000, inventoryCapacity = 150, defaultTax = 10, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/e/e2/Washington.png" },
            new SellingVehicleModel(){ ID = 13, model = "warrener", name = "Vulcar Warrener", price = 14300, inventoryCapacity = 140, defaultTax = 6, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/2/22/Warrener.png" },
            new SellingVehicleModel(){ ID = 14, model = "fugitive", name = "Cheval Fugitive", price = 30000, inventoryCapacity = 160, defaultTax = 12, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/7/73/Fugitive.png" },
            new SellingVehicleModel(){ ID = 15, model = "primo2", name = "Albany Primo Custom", price = 21850, inventoryCapacity = 140, defaultTax = 10, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/3/30/Primo2.png" },
            new SellingVehicleModel(){ ID = 16, model = "glendale2", name = "Benefactor Glendale Custom", price = 24000, inventoryCapacity = 160, defaultTax = 10, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.rage.mp/images/b/bd/Glendale2.png" },
            new SellingVehicleModel(){ ID = 17, model = "tailgater", name = "Obey Tailgater", price = 40000, inventoryCapacity = 150, defaultTax = 20, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/a/af/Tailgater.png" },
            new SellingVehicleModel(){ ID = 18, model = "schafter2", name = "Benefactor Schafter", price = 53000, inventoryCapacity = 150, defaultTax = 32, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/c/cc/Schafter2.png" },
            new SellingVehicleModel(){ ID = 19, model = "schafter4", name = "Benefactor Schafter LVVB", price = 65000, inventoryCapacity = 150, defaultTax = 40, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/2/29/Schafter4.png" },
            new SellingVehicleModel(){ ID = 20, model = "cog55", name = "埃努斯 Cognoscenti 55", price = 90000, inventoryCapacity = 190, defaultTax = 45, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/d/d5/Cog55.png" },
            new SellingVehicleModel(){ ID = 21, model = "superd", name = "埃努斯 Super Diamond", price = 300000, inventoryCapacity = 150, defaultTax = 300, petrolTank = 70, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/6/6e/Superd.png" },
            new SellingVehicleModel(){ ID = 22, model = "asterope", name = "Karin Asterope", price = 67000, inventoryCapacity = 50, defaultTax = 30, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/thumb/3/35/Gb200.png/800px-Gb200.png" },
            new SellingVehicleModel(){ ID = 23, model = "cognoscenti", name = "埃努斯 Cognoscenti", price = 85000, inventoryCapacity = 190, defaultTax = 85, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/5/51/Cognoscenti.png" },
            new SellingVehicleModel(){ ID = 24, model = "cinquemila", name = "Lampadati Cinquemila", price = 180000, inventoryCapacity = 180, defaultTax = 120, petrolTank = 80, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/5/51/Cognoscenti.png" },
            new SellingVehicleModel(){ ID = 25, model = "postlude", name = "Dinka Postlude", price = 30000, inventoryCapacity = 140, defaultTax = 12, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/5/51/Cognoscenti.png" },
            new SellingVehicleModel(){ ID = 26, model = "rhinehart", name = "Übermacht Rhinehart", price = 150000, inventoryCapacity = 200, defaultTax = 150, petrolTank = 80, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/5/51/Cognoscenti.png" },

        };

        // TODO Bisiklet -

        public static List<SellingVehicleModel> cycles = new List<SellingVehicleModel>()
        {
            new SellingVehicleModel(){ ID = 1, model = "bmx", name = "BMX [20 赞助点]", price = 1500, inventoryCapacity = 1, defaultTax = 0, petrolTank = 2, fuelConsumption = 0, picture = "https://wiki.altv.mp/images/4/40/Bmx.png" },
            new SellingVehicleModel(){ ID = 2, model = "cruiser", name = "Cruiser [20 赞助点]", price = 2000, inventoryCapacity = 3, defaultTax = 0, petrolTank = 1, fuelConsumption = 0, picture = "https://wiki.altv.mp/images/6/66/Cruiser.png" },
            new SellingVehicleModel(){ ID = 3, model = "fixter", name = "Fixter [20 赞助点]", price = 2500, inventoryCapacity = 3, defaultTax = 0, petrolTank = 1, fuelConsumption = 0, picture = "https://wiki.altv.mp/images/2/29/Fixter.png" },
            new SellingVehicleModel(){ ID = 4, model = "scorcher", name = "Scorcher [20 赞助点]", price = 3000, inventoryCapacity = 4, defaultTax = 0, petrolTank = 1, fuelConsumption = 0, picture = "https://wiki.altv.mp/images/3/34/Scorcher.png" },
            new SellingVehicleModel(){ ID = 5, model = "tribike", name = "Whippet Race Bike [20 赞助点]", price = 3000, inventoryCapacity = 5, defaultTax = 0, petrolTank = 1, fuelConsumption = 0, picture = "https://wiki.altv.mp/images/2/2c/Tribike.png" },
            new SellingVehicleModel(){ ID = 6, model = "tribike2", name = "Endurex Race Bike [20 赞助点]", price = 3000, inventoryCapacity = 5, defaultTax = 0, petrolTank = 1, fuelConsumption = 0, picture = "https://wiki.altv.mp/images/b/b3/Tribike2.png" },
            new SellingVehicleModel(){ ID = 7, model = "tribike3", name = "Tri-Cycles Race Bike [20 赞助点]", price = 3000, inventoryCapacity = 5, defaultTax = 0, petrolTank = 1, fuelConsumption = 0, picture = "https://wiki.altv.mp/images/2/28/Tribike3.png" },
        };

        // TODO Motorsiklet 

        public static List<SellingVehicleModel> motorcycles = new List<SellingVehicleModel>()
        {
            new SellingVehicleModel(){ ID = 1, model = "akuma", name = "Dinka Akuma", price = 18000, inventoryCapacity = 5, defaultTax = 10, petrolTank = 40, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/1/16/Akuma.png" },
            new SellingVehicleModel(){ ID = 2, model = "bagger", name = "Western Bagger", price = 22000, inventoryCapacity = 20, defaultTax = 12, petrolTank = 40, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/6/64/Bagger.png" },
            new SellingVehicleModel(){ ID = 3, model = "bati", name = "Pegassi Bati 801", price = 90000, inventoryCapacity = 5, defaultTax = 110, petrolTank = 50, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/c/ce/Bati.png" },
            new SellingVehicleModel(){ ID = 4, model = "carbonrs", name = "Nagasaki Carbon RS", price = 65000, inventoryCapacity = 15, defaultTax = 55, petrolTank = 40, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/c/c7/Carbonrs.png" },
            new SellingVehicleModel(){ ID = 5, model = "daemon", name = "Western Daemon", price = 25000, inventoryCapacity = 5, defaultTax = 16, petrolTank = 30, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/2/27/Daemon.png" },
            new SellingVehicleModel(){ ID = 6, model = "daemon2", name = "Western Daemon II", price = 23000, inventoryCapacity = 5, defaultTax = 15, petrolTank = 30, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/8/86/Daemon2.png" },
            new SellingVehicleModel(){ ID = 7, model = "diablous", name = "Western Diabolus", price = 25000, inventoryCapacity = 5, defaultTax = 15, petrolTank = 40, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/4/48/Diablous.png" },
            new SellingVehicleModel(){ ID = 8, model = "diablous2", name = "Western Diabolus Custom", price = 28000, inventoryCapacity = 5, defaultTax = 17, petrolTank = 40, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/3/3e/Diablous2.png" },
            new SellingVehicleModel(){ ID = 9, model = "double", name = "Dinka Double T", price = 60000, inventoryCapacity = 5, defaultTax = 75, petrolTank = 40, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/0/01/Double.png" },
            new SellingVehicleModel(){ ID = 10, model = "faggio", name = "Pegassi Faggio Sport", price = 6400, inventoryCapacity = 5, defaultTax = 2, petrolTank = 20, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/4/4d/Faggio.png" },
            new SellingVehicleModel(){ ID = 11, model = "faggio2", name = "Pegassi Faggio", price = 5520, inventoryCapacity = 5, defaultTax = 2, petrolTank = 20, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/2/25/Faggio2.png" },
            new SellingVehicleModel(){ ID = 12, model = "faggio3", name = "Pegassi Faggio Mod", price = 6500, inventoryCapacity = 5, defaultTax = 2, petrolTank = 20, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/a/a6/Faggio3.png" },
            new SellingVehicleModel(){ ID = 13, model = "hexer", name = "Liberty City Cycles Hexer", price = 22300, inventoryCapacity = 5, defaultTax = 15, petrolTank = 30, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/5/56/Hexer.png" },
            new SellingVehicleModel(){ ID = 14, model = "innovation", name = "Liberty City Cycles Innovation", price = 23000, inventoryCapacity = 5, defaultTax = 15, petrolTank = 30, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/4/42/Innovation.png" },
            new SellingVehicleModel(){ ID = 15, model = "lectro", name = "Principe Lectro", price = 26000, inventoryCapacity = 5, defaultTax = 16, petrolTank = 30, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/0/00/Lectro.png" },
            new SellingVehicleModel(){ ID = 16, model = "nemesis", name = "Principe Nemesis", price = 15200, inventoryCapacity = 5, defaultTax = 7, petrolTank = 30, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/e/e3/Nemesis.png" },
            new SellingVehicleModel(){ ID = 17, model = "nightblade", name = "Western Nightblade", price = 32500, inventoryCapacity = 5, defaultTax = 28, petrolTank = 30, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/c/c1/Nightblade.png" },
            new SellingVehicleModel(){ ID = 18, model = "pcj", name = "Shitzu PCJ-600", price = 14300, inventoryCapacity = 5, defaultTax = 5, petrolTank = 30, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/9/99/Pcj.png" },
            new SellingVehicleModel(){ ID = 19, model = "ratbike", name = "Western Rat Bike", price = 4850, inventoryCapacity = 5, defaultTax = 2, petrolTank = 30, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/4/49/Ratbike.png" },
            new SellingVehicleModel(){ ID = 20, model = "ruffian", name = "Pegassi Ruffian", price = 22500, inventoryCapacity = 5, defaultTax = 12, petrolTank = 30, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/f/f6/Ruffian.png" },
            new SellingVehicleModel(){ ID = 21, model = "thrust", name = "Dinka Thrust", price = 27500, inventoryCapacity = 5, defaultTax = 17, petrolTank = 30, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/9/90/Thrust.png" },
            new SellingVehicleModel(){ ID = 22, model = "vader", name = "Shitzu Vader", price = 18500, inventoryCapacity = 5, defaultTax = 10, petrolTank = 30, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/d/dc/Vader.png" },
            new SellingVehicleModel(){ ID = 23, model = "wolfsbane", name = "Western Wolfsbane", price = 27000, inventoryCapacity = 5, defaultTax = 18, petrolTank = 30, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/c/c6/Wolfsbane.png" },
            new SellingVehicleModel(){ ID = 24, model = "zombiea", name = "Western Zombie Bobber", price = 20000, inventoryCapacity = 5, defaultTax = 14, petrolTank = 30, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/7/7f/Zombiea.png" },
            new SellingVehicleModel(){ ID = 25, model = "zombieb", name = "Western Zombie Chopper", price = 22000, inventoryCapacity = 5, defaultTax = 15, petrolTank = 30, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/2/2c/Zombieb.png" },
            new SellingVehicleModel(){ ID = 26, model = "avarus", name = "Liberty Avarus", price = 30000, inventoryCapacity = 5, defaultTax = 15, petrolTank = 37, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/1/13/Avarus.png" },
            new SellingVehicleModel(){ ID = 27, model = "deathbike", name = "Western Apocalypse Deathbike", price = 32500, inventoryCapacity = 5, defaultTax = 0, petrolTank = 32, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/e/eb/Deathbike.png" },
            new SellingVehicleModel(){ ID = 28, model = "gargoyle", name = "Western Gargoyle", price = 32000, inventoryCapacity = 5, defaultTax = 28, petrolTank = 30, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/4/44/Gargoyle.png" },
            //new SellingVehicleModel(){ ID = 29, model = "Nagasaki Shotaro", name = "dea", price = 1, inventoryCapacity = 5, defaultTax = 0, petrolTank = 39, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/e/e3/Shotaro.png" },
            new SellingVehicleModel(){ ID = 29, model = "sovereign", name = "Western Sovereign", price = 19500, inventoryCapacity = 5, defaultTax = 10, petrolTank = 30, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/a/ae/Sovereign.png" },
            new SellingVehicleModel(){ ID = 30, model = "Dinka Vindicator", name = "Dinka Vindicator", price = 42300, inventoryCapacity = 5, defaultTax = 43, petrolTank = 34, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/b/bb/Vindicator.png" },

        };

        // TODO sport & delux cars

        public static List<SellingVehicleModel> sportclassic = new List<SellingVehicleModel>()
        {
            new SellingVehicleModel(){ ID = 1, model = "ardent", name = "Ocelot Ardent", price = 350000, inventoryCapacity = 95, defaultTax = 0, petrolTank = 65, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/3/3d/Ardent.png" },
            new SellingVehicleModel(){ ID = 2, model = "btype", name = "Albany Roosevelt", price = 200000, inventoryCapacity = 190, defaultTax = 200, petrolTank = 50, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/6/64/Btype.png" },
            new SellingVehicleModel(){ ID = 3, model = "btype2", name = "Fränken Stange", price = 350000, inventoryCapacity = 190, defaultTax = 750, petrolTank = 55, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/9/98/Btype2.png" },
            new SellingVehicleModel(){ ID = 4, model = "btype3", name = "Fränken Roosevelt Valor", price = 210000, inventoryCapacity = 190, defaultTax = 210, petrolTank = 50, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/3/34/Btype3.png" },
            new SellingVehicleModel(){ ID = 5, model = "casco", name = "Lampadati Casco", price = 150000, inventoryCapacity = 130, defaultTax = 150, petrolTank = 60, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/d/d7/Casco.png" },
            new SellingVehicleModel(){ ID = 6, model = "cheetah2", name = "Grotti Cheetah Classic", price = 230000, inventoryCapacity = 80, defaultTax = 230, petrolTank = 60, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/d/dd/Cheetah2.png" },
            new SellingVehicleModel(){ ID = 7, model = "coquette2", name = "Invetero Coquette Classic", price = 180000, inventoryCapacity = 110, defaultTax = 180, petrolTank = 60, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/8/8a/Coquette2.png" },
            new SellingVehicleModel(){ ID = 8, model = "dynasty", name = "Weeny Dynasty", price = 43500, inventoryCapacity = 150, defaultTax = 43, petrolTank = 40, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/8/88/Dynasty.png" },
            new SellingVehicleModel(){ ID = 9, model = "fagaloa", name = "Vulcar Fagaloa", price = 38700, inventoryCapacity = 200, defaultTax = 18, petrolTank = 40, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/1/15/Fagaloa.png" },
            new SellingVehicleModel(){ ID = 10, model = "feltzer3", name = "Benefactor Stirling GT", price = 170000, inventoryCapacity = 130, defaultTax = 170, petrolTank = 60, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/0/0b/Feltzer3.png" },
            new SellingVehicleModel(){ ID = 11, model = "gt500", name = "Grotti GT500", price = 130000, inventoryCapacity = 130, defaultTax = 130, petrolTank = 60, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/8/84/Gt500.png" },
            new SellingVehicleModel(){ ID = 12, model = "infernus2", name = "Pegassi Infernus Classic", price = 230000, inventoryCapacity = 85, defaultTax = 230, petrolTank = 60, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/9/91/Infernus2.png" },
            new SellingVehicleModel(){ ID = 13, model = "jb7002", name = "Dewbauchee JB 700W", price = 174000, inventoryCapacity = 130, defaultTax = 174, petrolTank = 60, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/b/bc/Jb7002.png" },
            new SellingVehicleModel(){ ID = 14, model = "mamba", name = "Declasse Mamba", price = 195000, inventoryCapacity = 100, defaultTax = 195, petrolTank = 60, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/c/c0/Mamba.png" },
            new SellingVehicleModel(){ ID = 15, model = "manana", name = "Albany Manana", price = 28000, inventoryCapacity = 120, defaultTax = 15, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/5/50/Manana.png" },
            new SellingVehicleModel(){ ID = 16, model = "manana2", name = "Albany Manana Custom", price = 29000, inventoryCapacity = 120, defaultTax = 16, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.rage.mp/images/9/9e/Manana2.png" },
            new SellingVehicleModel(){ ID = 17, model = "michelli", name = "Lampadati Michelli GT", price = 33000, inventoryCapacity = 100, defaultTax = 22, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/1/15/Michelli.png" },
            new SellingVehicleModel(){ ID = 18, model = "monroe", name = "Pegassi Monroe", price = 160000, inventoryCapacity = 130, defaultTax = 160, petrolTank = 60, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/6/64/Monroe.png" },
            new SellingVehicleModel(){ ID = 19, model = "nebula", name = "Vulcar Nebula Turbo", price = 28700, inventoryCapacity = 120, defaultTax = 15, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/e/ea/Nebula.png" },
            new SellingVehicleModel(){ ID = 20, model = "peyote", name = "威皮 Peyote", price = 28100, inventoryCapacity = 130, defaultTax = 15, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/2/21/Peyote.png" },
            new SellingVehicleModel(){ ID = 21, model = "peyote3", name = "威皮 Peyote Custom", price = 35600, inventoryCapacity = 130, defaultTax = 20, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.rage.mp/images/d/d9/Peyote3.png" },
            new SellingVehicleModel(){ ID = 22, model = "pigalle", name = "Lampadati Pigalle", price = 21000, inventoryCapacity = 130, defaultTax = 15, petrolTank = 50, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/7/76/Pigalle.png" },
            new SellingVehicleModel(){ ID = 23, model = "rapidgt3", name = "Dewbauchee Rapid GT Classic", price = 60000, inventoryCapacity = 130, defaultTax = 40, petrolTank = 60, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/0/09/Rapidgt3.png" },
            new SellingVehicleModel(){ ID = 24, model = "retinue", name = "威皮 Retinue", price = 31300, inventoryCapacity = 120, defaultTax = 31, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/5/53/Retinue.png" },
            new SellingVehicleModel(){ ID = 25, model = "retinue2", name = "威皮 Retinue Mk II", price = 33750, inventoryCapacity = 120, defaultTax = 33, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/8/86/Retinue2.png" },
            new SellingVehicleModel(){ ID = 26, model = "savestra", name = "Annis Savestra", price = 38900, inventoryCapacity = 120, defaultTax = 38, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/2/26/Savestra.png" },
            new SellingVehicleModel(){ ID = 27, model = "stinger", name = "Grotti Stinger", price = 160000, inventoryCapacity = 130, defaultTax = 160, petrolTank = 60, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/b/b6/Stinger.png" },
            new SellingVehicleModel(){ ID = 28, model = "stingergt", name = "Grotti Stinger GT", price = 166000, inventoryCapacity = 130, defaultTax = 166, petrolTank = 60, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/8/8d/Stingergt.png" },
            new SellingVehicleModel(){ ID = 29, model = "torero", name = "Pegassi Torero", price = 195000, inventoryCapacity = 100, defaultTax = 195, petrolTank = 60, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/5/5f/Torero.png" },
            new SellingVehicleModel(){ ID = 30, model = "tornado", name = "Declasse Tornado", price = 18000, inventoryCapacity = 160, defaultTax = 8, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/4/40/Tornado.png" },
            new SellingVehicleModel(){ ID = 31, model = "tornado2", name = "Declasse Tornado", price = 23000, inventoryCapacity = 160, defaultTax = 11, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/0/05/Tornado2.png" },
            new SellingVehicleModel(){ ID = 32, model = "tornado3", name = "Declasse Beater Tornado", price = 7000, inventoryCapacity = 160, defaultTax = 2, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/7/7d/Tornado3.png" },
            new SellingVehicleModel(){ ID = 33, model = "tornado4", name = "Declasse Mariachi Tornado", price = 9000, inventoryCapacity = 160, defaultTax = 3, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/f/fa/Tornado4.png" },
            new SellingVehicleModel(){ ID = 34, model = "tornado5", name = "Declasse Tornado Custom", price = 24400, inventoryCapacity = 160, defaultTax = 12, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/8/83/Tornado5.png" },
            new SellingVehicleModel(){ ID = 35, model = "tornado6", name = "Declasse Tornado Rat Rod", price = 85000, inventoryCapacity = 160, defaultTax = 85, petrolTank = 60, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/6/69/Tornado6.png" },
            new SellingVehicleModel(){ ID = 36, model = "turismo2", name = "Grotti Turismo Classic", price = 240000, inventoryCapacity = 100, defaultTax = 240, petrolTank = 60, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/f/fa/Turismo2.png" },
            new SellingVehicleModel(){ ID = 37, model = "viseris", name = "Lampadati Viseris", price = 130000, inventoryCapacity = 110, defaultTax = 130, petrolTank = 60, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/2/24/Viseris.png" },
            new SellingVehicleModel(){ ID = 38, model = "z190", name = "Karin 190z", price = 49700, inventoryCapacity = 120, defaultTax = 49, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/e/e1/Z190.png" },
            new SellingVehicleModel(){ ID = 39, model = "ztype", name = "Truffade Z-Type", price = 350000, inventoryCapacity = 90, defaultTax = 350, petrolTank = 60, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/5/56/Ztype.png" },
            new SellingVehicleModel(){ ID = 40, model = "zion3", name = "Übermacht Zion Classic", price = 28200, inventoryCapacity = 130, defaultTax = 15, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/5/5a/Zion3.png" },
            new SellingVehicleModel(){ ID = 41, model = "cheburek", name = "Rune Cheburek", price = 19150, inventoryCapacity = 150, defaultTax = 19, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/4/4f/Cheburek.png" },
            new SellingVehicleModel(){ ID = 42, model = "rapidgt3", name = "Dewbauchee Rapid GT Classic", price = 60000, inventoryCapacity = 130, defaultTax = 60, petrolTank = 60, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/0/09/Rapidgt3.png" },
            new SellingVehicleModel(){ ID = 43, model = "stretch", name = "Dewbauchee Classic Stretch", price = 1000000, inventoryCapacity = 150, defaultTax = 500, petrolTank = 100, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/0/09/Rapidgt3.png" },
        };

        public static List<SellingVehicleModel> sports = new List<SellingVehicleModel>()
        {
            new SellingVehicleModel(){ ID = 1, model = "alpha", name = "Albany Alpha", price = 120000, inventoryCapacity = 160, defaultTax = 90, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/a/a8/Alpha.png" },
            new SellingVehicleModel(){ ID = 2, model = "banshee", name = "Bravado Banshee", price = 190000, inventoryCapacity = 130, defaultTax = 160, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/d/d3/Banshee.png" },
            new SellingVehicleModel(){ ID = 3, model = "bestiagts", name = "Grotti Bestia GTS", price = 170000, inventoryCapacity = 150, defaultTax = 170, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/5/5c/Bestiagts.png" },
            new SellingVehicleModel(){ ID = 4, model = "carbonizzare", name = "Grotti Carbonizzare", price = 260000, inventoryCapacity = 120, defaultTax = 260, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/4/43/Carbonizzare.png" },
            new SellingVehicleModel(){ ID = 5, model = "comet2", name = "Pfister Comet", price = 180000, inventoryCapacity = 100, defaultTax = 180, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/c/cb/Comet2.png" },
            new SellingVehicleModel(){ ID = 6, model = "comet3", name = "Pfister Comet Retro Custom", price = 200000, inventoryCapacity = 145, defaultTax = 200, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/b/b6/Comet3.png" },
            new SellingVehicleModel(){ ID = 7, model = "comet5", name = "Pfister Comet SR", price = 205000, inventoryCapacity = 100, defaultTax = 205, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/c/ca/Comet5.png" },
            new SellingVehicleModel(){ ID = 8, model = "coquette", name = "Invetero Coquette", price = 190000, inventoryCapacity = 130, defaultTax = 190, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/d/d6/Coquette.png" },
            new SellingVehicleModel(){ ID = 9, model = "coquette4", name = "Invetero Coquette D10", price = 400000, inventoryCapacity = 120, defaultTax = 400, petrolTank = 70, fuelConsumption = 2, picture = "https://wiki.rage.mp/images/9/97/Coquette4.png" },
            new SellingVehicleModel(){ ID = 10, model = "drafter", name = "Obey 8F Drafter", price = 130000, inventoryCapacity = 180, defaultTax = 100, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/a/aa/Drafter.png" },
            new SellingVehicleModel(){ ID = 11, model = "elegy2", name = "Annis Elegy", price = 125000, inventoryCapacity = 150, defaultTax = 100, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/3/33/Elegy2.png" },
            new SellingVehicleModel(){ ID = 12, model = "elegy", name = "Annis Elegy Retro Custom", price = 140000, inventoryCapacity = 120, defaultTax = 140, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/e/ea/Elegy.png" },
            new SellingVehicleModel(){ ID = 13, model = "feltzer2", name = "Benefactor Feltzer", price = 79000, inventoryCapacity = 120, defaultTax = 60, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/7/7c/Feltzer2.png" },
            new SellingVehicleModel(){ ID = 14, model = "flashgt", name = "威皮 Flash GT", price = 100000, inventoryCapacity = 160, defaultTax = 85, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/8/80/Flashgt.png" },
            new SellingVehicleModel(){ ID = 15, model = "furoregt", name = "Lampadati Furore GT", price = 105000, inventoryCapacity = 130, defaultTax = 88, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/c/cb/Furoregt.png" },
            new SellingVehicleModel(){ ID = 16, model = "fusilade", name = "施特 Fusilade", price = 68000, inventoryCapacity = 120, defaultTax = 55, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/5/56/Fusilade.png" },
            new SellingVehicleModel(){ ID = 17, model = "futo", name = "Karin Futo", price = 18000, inventoryCapacity = 110, defaultTax = 18, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/0/04/Futo.png" },
            new SellingVehicleModel(){ ID = 18, model = "imorgon", name = "Överflöd Imorgon", price = 160000, inventoryCapacity = 120, defaultTax = 130, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/1/1f/Imorgon.png" },
            new SellingVehicleModel(){ ID = 19, model = "iss7", name = "Weeny Issi Sport", price = 79000, inventoryCapacity = 160, defaultTax = 79, petrolTank = 65, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/0/04/Issi7.png" },
            new SellingVehicleModel(){ ID = 20, model = "italigto", name = "Grotti Itali GTO", price = 300000, inventoryCapacity = 140, defaultTax = 300, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/d/dd/Italigto.png" },
            new SellingVehicleModel(){ ID = 21, model = "jugular", name = "Ocelot Jugular", price = 180000, inventoryCapacity = 160, defaultTax = 150, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/6/69/Jugular.png" },
            new SellingVehicleModel(){ ID = 22, model = "jester", name = "Dinka Jester", price = 192000, inventoryCapacity = 120, defaultTax = 192, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/e/e0/Jester.png" },
            new SellingVehicleModel(){ ID = 23, model = "jester3", name = "Dinka Jester Classic", price = 80000, inventoryCapacity = 130, defaultTax = 70, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/1/19/Jester3.png" },
            new SellingVehicleModel(){ ID = 24, model = "khamelion", name = "Hijak Khamelion", price = 180000, inventoryCapacity = 100, defaultTax = 150, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/2/2e/Khamelion.png" },
            new SellingVehicleModel(){ ID = 25, model = "komoda", name = "Lampadati Komoda", price = 115000, inventoryCapacity = 120, defaultTax = 90, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/4/47/Komoda.png" },
            new SellingVehicleModel(){ ID = 26, model = "kuruma", name = "Karin Kuruma", price = 60000, inventoryCapacity = 140, defaultTax = 45, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/8/8f/Kuruma.png" },
            new SellingVehicleModel(){ ID = 27, model = "lynx", name = "Ocelot Lynx", price = 100000, inventoryCapacity = 130, defaultTax = 85, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/b/b8/Lynx.png" },
            new SellingVehicleModel(){ ID = 28, model = "massacro", name = "Dewbauchee Massacro", price = 150000, inventoryCapacity = 130, defaultTax = 130, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/7/78/Massacro.png" },
            new SellingVehicleModel(){ ID = 29, model = "neo", name = "Vysser Neo", price = 250000, inventoryCapacity = 100, defaultTax = 250, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/7/74/Neo.png" },
            new SellingVehicleModel(){ ID = 30, model = "neon", name = "Pfister Neon", price = 300000, inventoryCapacity = 130, defaultTax = 260, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/1/17/Neon.png" },
            new SellingVehicleModel(){ ID = 31, model = "ninef", name = "Obey 9F", price = 230000, inventoryCapacity = 100, defaultTax = 200, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/5/59/Ninef.png" },
            new SellingVehicleModel(){ ID = 32, model = "ninef2", name = "Obey 9F Cabrio", price = 245000, inventoryCapacity = 50, defaultTax = 220, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/f/f6/Ninef2.png" },
            new SellingVehicleModel(){ ID = 33, model = "paragon", name = "埃努斯 Paragon R", price = 260000, inventoryCapacity = 140, defaultTax = 260, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/1/1e/Paragon.png" },
            new SellingVehicleModel(){ ID = 34, model = "pariah", name = "Ocelot Pariah", price = 220000, inventoryCapacity = 120, defaultTax = 220, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/d/df/Pariah.png" },
            new SellingVehicleModel(){ ID = 35, model = "penumbra", name = "Maibatsu Penumbra", price = 32600, inventoryCapacity = 140, defaultTax = 20, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/9/93/Penumbra.png" },
            new SellingVehicleModel(){ ID = 36, model = "penumbra2", name = "Maibatsu Penumbra FF", price = 39000, inventoryCapacity = 140, defaultTax = 25, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.rage.mp/images/d/dd/Penumbra2.png" },
            new SellingVehicleModel(){ ID = 37, model = "raiden", name = "Coil Raiden", price = 200000, inventoryCapacity = 140, defaultTax = 200, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/5/5f/Raiden.png" },
            new SellingVehicleModel(){ ID = 38, model = "rapidgt", name = "Dewbauchee Rapid GT", price = 130000, inventoryCapacity = 110, defaultTax = 130, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/e/e9/Rapidgt.png" },
            new SellingVehicleModel(){ ID = 39, model = "rapidgt2", name = "Dewbauchee Rapid GT Cabrio", price = 145000, inventoryCapacity = 110, defaultTax = 145, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/3/3e/Rapidgt2.png" },
            new SellingVehicleModel(){ ID = 40, model = "revolter", name = "Übermacht Revolter", price = 350000, inventoryCapacity = 160, defaultTax = 350, petrolTank = 100, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/2/24/Revolter.png" },
            new SellingVehicleModel(){ ID = 41, model = "schafter3", name = "Benefactor Schafter V12", price = 80000, inventoryCapacity = 140, defaultTax = 70, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/a/a9/Schafter3.png" },
            new SellingVehicleModel(){ ID = 42, model = "schlagen", name = "Benefactor Schlagen GT", price = 240000, inventoryCapacity = 110, defaultTax = 240, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/9/97/Schlagen.png" },
            new SellingVehicleModel(){ ID = 43, model = "schwarzer", name = "Benefactor Schwarzer", price = 60000, inventoryCapacity = 130, defaultTax = 50, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/1/19/Schwarzer.png" },
            new SellingVehicleModel(){ ID = 44, model = "sentinel3", name = "Sentinel Classic", price = 35000, inventoryCapacity = 130, defaultTax = 15, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/0/0f/Sentinel3.png" },
            new SellingVehicleModel(){ ID = 45, model = "seven70", name = "Dewbauchee Seven-70", price = 250000, inventoryCapacity = 120, defaultTax = 250, petrolTank = 100, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/6/60/Seven70.png" },
            new SellingVehicleModel(){ ID = 46, model = "specter", name = "Dewbauchee Specter", price = 250000, inventoryCapacity = 130, defaultTax = 250, petrolTank = 80, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/f/f1/Specter.png" },
            new SellingVehicleModel(){ ID = 47, model = "sugoi", name = "Dinka Sugoi", price = 65000, inventoryCapacity = 140, defaultTax = 40, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/2/25/Sugoi.png" },
            new SellingVehicleModel(){ ID = 48, model = "sultan", name = "Karin Sultan", price = 60000, inventoryCapacity = 130, defaultTax = 38, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/f/f4/Sultan.png" },
            new SellingVehicleModel(){ ID = 49, model = "sultan2", name = "Karin Sultan Classic", price = 70000, inventoryCapacity = 120, defaultTax = 45, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/5/57/Sultan2.png" },
            new SellingVehicleModel(){ ID = 50, model = "surano", name = "Benefactor Surano", price = 260000, inventoryCapacity = 110, defaultTax = 260, petrolTank = 80, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/9/96/Surano.png" },
            new SellingVehicleModel(){ ID = 51, model = "verlierer2", name = "Bravado Verlierer", price = 130000, inventoryCapacity = 120, defaultTax = 130, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/8/81/Verlierer2.png" },
            new SellingVehicleModel(){ ID = 52, model = "vstr", name = "Albany V-STR", price = 160000, inventoryCapacity = 140, defaultTax = 130, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/4/43/Vstr.png" },
            new SellingVehicleModel(){ ID = 53, model = "specter2", name = "Dewbauchee Specter Custom", price = 300000, inventoryCapacity = 60, defaultTax = 300, petrolTank = 90, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/thumb/9/9f/Specter2.png/800px-Specter2.png" },
            new SellingVehicleModel(){ ID = 54, model = "tropos", name = "Lampadati Tropos Rallye", price = 74000, inventoryCapacity = 50, defaultTax = 74, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/thumb/7/71/Tropos.png/800px-Tropos.png" },
            new SellingVehicleModel(){ ID = 55, model = "issi7", name = "Weeny Issi Sport", price = 115000, inventoryCapacity = 50, defaultTax = 115, petrolTank = 86, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/thumb/0/04/Issi7.png/800px-Issi7.png" },
            new SellingVehicleModel(){ ID = 56, model = "champion", name = "Dewbauchee Champion", price = 200000, inventoryCapacity = 150, defaultTax = 180, petrolTank = 100, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/thumb/0/04/Issi7.png/800px-Issi7.png" },
            new SellingVehicleModel(){ ID = 57, model = "comet7", name = "Comet S2 Cabrio", price = 320000, inventoryCapacity = 150, defaultTax = 270, petrolTank = 100, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/thumb/0/04/Issi7.png/800px-Issi7.png" },
            new SellingVehicleModel(){ ID = 58, model = "deity", name = "埃努斯 Deity", price = 220000, inventoryCapacity = 150, defaultTax = 115, petrolTank = 100, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/thumb/0/04/Issi7.png/800px-Issi7.png" },
            new SellingVehicleModel(){ ID = 59, model = "gb200", name = "埃努斯 GB200", price = 50000, inventoryCapacity = 120, defaultTax = 23, petrolTank = 70, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/thumb/0/04/Issi7.png/800px-Issi7.png" },
            new SellingVehicleModel(){ ID = 60, model = "omnis", name = "埃努斯 Omnis", price = 50000, inventoryCapacity = 120, defaultTax = 23, petrolTank = 70, fuelConsumption = 1, picture = "https://wiki.altv.mp/images/thumb/0/04/Issi7.png/800px-Issi7.png" },
            new SellingVehicleModel(){ ID = 61, model = "omnisegt", name = "Obey Omnis e-GT", price = 250000, inventoryCapacity = 120, defaultTax = 250, petrolTank = 100, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/thumb/0/04/Issi7.png/800px-Issi7.png" },
            new SellingVehicleModel(){ ID = 62, model = "sentinel4", name = "Sentinel Classic Widebody", price = 100000, inventoryCapacity = 120, defaultTax = 90, petrolTank = 80, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/thumb/0/04/Issi7.png/800px-Issi7.png" },
            new SellingVehicleModel(){ ID = 63, model = "tenf", name = "Obey 10F", price = 330000, inventoryCapacity = 120, defaultTax = 330, petrolTank = 80, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/thumb/0/04/Issi7.png/800px-Issi7.png" },
            new SellingVehicleModel(){ ID = 64, model = "tenf2", name = "Obey 10F Widebody", price = 345000, inventoryCapacity = 120, defaultTax = 345, petrolTank = 80, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/thumb/0/04/Issi7.png/800px-Issi7.png" },

        };

        public static List<SellingVehicleModel> supercars = new List<SellingVehicleModel>()
        {
            new SellingVehicleModel(){ ID = 1, model = "adder", name = "Truffade Adder ", price = 342000, inventoryCapacity = 110, defaultTax = 342, petrolTank = 100, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/c/c2/Adder.png" },
            new SellingVehicleModel(){ ID = 2, model = "autarch", name = "Overflod Autarch", price = 500000, inventoryCapacity = 110, defaultTax = 500, petrolTank = 100, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/3/38/Autarch.png" },
            new SellingVehicleModel(){ ID = 3, model = "banshee2", name = "Bravado Banshee 900R", price = 250000, inventoryCapacity = 110, defaultTax = 250, petrolTank = 120, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/9/9b/Banshee2.png" },
            new SellingVehicleModel(){ ID = 4, model = "bullet", name = "威皮 Bullet GT", price = 220000, inventoryCapacity = 110, defaultTax = 220, petrolTank = 100, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/7/7a/Bullet.png" },
            new SellingVehicleModel(){ ID = 5, model = "cheetah", name = "Grotti Cheetah", price = 320000, inventoryCapacity = 110, defaultTax = 320, petrolTank = 100, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/9/9e/Cheetah.png" },
            new SellingVehicleModel(){ ID = 6, model = "cyclone", name = "Coil Cyclone ", price = 325000, inventoryCapacity = 110, defaultTax = 325, petrolTank = 100, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/7/79/Cyclone.png" },
            new SellingVehicleModel(){ ID = 7, model = "entity2", name = "Överflöd Entity XXR", price = 412000, inventoryCapacity = 110, defaultTax = 412, petrolTank = 100, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/f/f3/Entity2.png" },
            new SellingVehicleModel(){ ID = 8, model = "entityxf", name = "Överflöd Entity XF", price = 370000, inventoryCapacity = 110, defaultTax = 370, petrolTank = 100, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/6/61/Entityxf.png" },
            new SellingVehicleModel(){ ID = 9, model = "emerus", name = "Progen Emerus", price = 400000, inventoryCapacity = 120, defaultTax = 400, petrolTank = 120, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/0/0a/Emerus.png" },
            new SellingVehicleModel(){ ID = 10, model = "fmj", name = "威皮 FMJ", price = 360000, inventoryCapacity = 110, defaultTax = 360, petrolTank = 120, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/d/d2/Fmj.png" },
            new SellingVehicleModel(){ ID = 11, model = "furia", name = "Grotti Furia", price = 340000, inventoryCapacity = 130, defaultTax = 340, petrolTank = 120, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/0/05/Furia.png" },
            new SellingVehicleModel(){ ID = 12, model = "gp1", name = "Progen GP1", price = 230000, inventoryCapacity = 110, defaultTax = 230, petrolTank = 100, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/a/a4/Gp1.png" },
            new SellingVehicleModel(){ ID = 13, model = "infernus", name = "Pegassi Infernus", price = 260000, inventoryCapacity = 110, defaultTax = 260, petrolTank = 100, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/d/d2/Infernus.png" },
            new SellingVehicleModel(){ ID = 14, model = "italigtb", name = "Progen Itali GTB", price = 320000, inventoryCapacity = 110, defaultTax = 320, petrolTank = 120, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/d/dd/Italigtb.png" },
            new SellingVehicleModel(){ ID = 15, model = "italigtb2", name = "Progen Itali GTB Custom", price = 330000, inventoryCapacity = 110, defaultTax = 330, petrolTank = 120, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/5/57/Italigtb2.png" },
            new SellingVehicleModel(){ ID = 16, model = "krieger", name = "Benefactor Krieger", price = 450000, inventoryCapacity = 110, defaultTax = 450, petrolTank = 120, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/f/fc/Krieger.png" },
            new SellingVehicleModel(){ ID = 17, model = "le7b", name = "Annis RE-7B", price = 400000, inventoryCapacity = 110, defaultTax = 400, petrolTank = 120, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/e/ee/Le7b.png" },
            new SellingVehicleModel(){ ID = 18, model = "nero", name = "Truffade Nero", price = 345000, inventoryCapacity = 110, defaultTax = 345, petrolTank = 120, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/e/ed/Nero.png" },
            new SellingVehicleModel(){ ID = 19, model = "nero2", name = "Truffade Nero Custom", price = 356000, inventoryCapacity = 110, defaultTax = 356, petrolTank = 120, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/e/ec/Nero2.png" },
            new SellingVehicleModel(){ ID = 20, model = "osiris", name = "Pegassi Osiris", price = 450000, inventoryCapacity = 110, defaultTax = 450, petrolTank = 120, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/e/e3/Osiris.png" },
            new SellingVehicleModel(){ ID = 21, model = "penetrator", name = "Ocelot Penetrator", price = 220000, inventoryCapacity = 110, defaultTax = 220, petrolTank = 100, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/d/d9/Penetrator.png" },
            new SellingVehicleModel(){ ID = 22, model = "pfister811", name = "Pfister 811", price = 360000, inventoryCapacity = 110, defaultTax = 360, petrolTank = 100, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/8/8e/Pfister811.png" },
            new SellingVehicleModel(){ ID = 23, model = "prototipo", name = "Grotti X80 Proto", price = 555000, inventoryCapacity = 110, defaultTax = 555, petrolTank = 150, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/f/fb/Prototipo.png" },
            new SellingVehicleModel(){ ID = 24, model = "reaper", name = "Pegassi Reaper", price = 332000, inventoryCapacity = 110, defaultTax = 332, petrolTank = 120, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/6/6a/Reaper.png" },
            new SellingVehicleModel(){ ID = 25, model = "s80", name = "Annis S80RR", price = 430000, inventoryCapacity = 110, defaultTax = 430, petrolTank = 120, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/a/a2/S80.png" },
            new SellingVehicleModel(){ ID = 26, model = "sc1", name = "Übermacht SC1", price = 300000, inventoryCapacity = 110, defaultTax = 300, petrolTank = 100, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/5/59/Sc1.png" },
            //new SellingVehicleModel(){ ID = 27, model = "scramjet", name = "Declasse Scramjet", price = 1, inventoryCapacity = 110, defaultTax = 0, petrolTank = 61, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/8/85/Scramjet.png" },
            new SellingVehicleModel(){ ID = 28, model = "sheava", name = "Emperor ETR1", price = 230000, inventoryCapacity = 110, defaultTax = 230, petrolTank = 100, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/0/06/Sheava.png" },
            new SellingVehicleModel(){ ID = 29, model = "sultanrs", name = "Karin Sultan RS", price = 160000, inventoryCapacity = 110, defaultTax = 120, petrolTank = 80, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/8/88/Sultanrs.png" },
            new SellingVehicleModel(){ ID = 30, model = "t20", name = "Progen T20", price = 300000, inventoryCapacity = 110, defaultTax = 300, petrolTank = 120, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/7/7d/T20.png" },
            new SellingVehicleModel(){ ID = 31, model = "taipan", name = "Cheval Taipan", price = 260000, inventoryCapacity = 110, defaultTax = 260, petrolTank = 120, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/8/8a/Taipan.png" },
            new SellingVehicleModel(){ ID = 32, model = "tempesta", name = "Pegassi Tempesta", price = 320000, inventoryCapacity = 110, defaultTax = 320, petrolTank = 120, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/8/8a/Tempesta.png" },
            new SellingVehicleModel(){ ID = 33, model = "tezeract", name = "Pegassi Tezeract", price = 500000, inventoryCapacity = 110, defaultTax = 416, petrolTank = 120, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/a/ab/Tezeract.png" },
            new SellingVehicleModel(){ ID = 34, model = "thrax", name = "Truffade Thrax", price = 399000, inventoryCapacity = 110, defaultTax = 399, petrolTank = 100, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/4/4f/Thrax.png" },
            new SellingVehicleModel(){ ID = 35, model = "tigon", name = "Lampadati Tigon", price = 260000, inventoryCapacity = 110, defaultTax = 260, petrolTank = 100, fuelConsumption = 3, picture = "https://wiki.rage.mp/images/f/f4/Tigon.png" },
            new SellingVehicleModel(){ ID = 36, model = "turismor", name = "Grotti Turismo R", price = 350000, inventoryCapacity = 110, defaultTax = 350, petrolTank = 100, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/7/7f/Turismor.png" },
            new SellingVehicleModel(){ ID = 37, model = "tyrant", name = "Overflod Tyrant", price = 400000, inventoryCapacity = 110, defaultTax = 400, petrolTank = 120, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/0/0a/Tyrant.png" },
            new SellingVehicleModel(){ ID = 38, model = "tyrus", name = "Progen Tyrus", price = 272000, inventoryCapacity = 110, defaultTax = 272, petrolTank = 100, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/e/e4/Tyrus.png" },
            new SellingVehicleModel(){ ID = 39, model = "vacca", name = "Pegassi Vacca", price = 242000, inventoryCapacity = 110, defaultTax = 242, petrolTank = 100, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/1/14/Vacca.png" },
            new SellingVehicleModel(){ ID = 40, model = "vagner", name = "Dewbauchee Vagner", price = 340000, inventoryCapacity = 110, defaultTax = 340, petrolTank = 120, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/9/92/Vagner.png" },
            //new SellingVehicleModel(){ ID = 41, model = "vigilante", name = "Grotti Vigilante", price = 1, inventoryCapacity = 110, defaultTax = 0, petrolTank = 63, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/c/c7/Vigilante.png" },
            new SellingVehicleModel(){ ID = 42, model = "visione", name = "Grotti Visione", price = 380000, inventoryCapacity = 110, defaultTax = 380, petrolTank = 120, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/0/01/Visione.png" },
            //new SellingVehicleModel(){ ID = 43, model = "voltic", name = "Coil Voltic", price = 175000, inventoryCapacity = 110, defaultTax = 175, petrolTank = 80, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/e/ef/Voltic.png" },
            //new SellingVehicleModel(){ ID = 44, model = "voltic2", name = "Coil Voltic (Topless)", price = 1, inventoryCapacity = 110, defaultTax = 0, petrolTank = 64, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/4/4f/Voltic2.png" },
            new SellingVehicleModel(){ ID = 45, model = "xa21", name = "Ocelot XA-21", price = 350000, inventoryCapacity = 110, defaultTax = 350, petrolTank = 100, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/4/4d/Xa21.png" },
            new SellingVehicleModel(){ ID = 46, model = "zentorno", name = "Pegassi Zentorno", price = 450000, inventoryCapacity = 110, defaultTax = 450, petrolTank = 120, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/2/2b/Zentorno.png" },
            new SellingVehicleModel(){ ID = 47, model = "zorrusso", name = "Pegassi Zorrusso", price = 456000, inventoryCapacity = 110, defaultTax = 456, petrolTank = 120, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/7/79/Zorrusso.png" },
            new SellingVehicleModel(){ ID = 48, model = "italirsx", name = "Grotti Itali RSX", price = 380000, inventoryCapacity = 50, defaultTax = 380, petrolTank = 80, fuelConsumption = 2, picture = "https://cdn.discordapp.com/attachments/817425977490800654/837120569836306432/itali-rsx.jpg" },
            // Yeni Araç Liste
            new SellingVehicleModel(){ ID = 49, model = "calico", name = "Karin Calico GTF", price = 75000, inventoryCapacity = 150, defaultTax = 30, petrolTank = 60, fuelConsumption = 2, picture = "https://cdn.discordapp.com/attachments/785199429488738315/870364127594442752/unknown.png" },
            new SellingVehicleModel(){ ID = 50, model = "comet6", name = "Pfister Comet S2", price = 300000, inventoryCapacity = 100, defaultTax = 300, petrolTank = 120, fuelConsumption = 2, picture = "https://cdn.discordapp.com/attachments/785199429488738315/870364676834328636/unknown.png" },
            new SellingVehicleModel(){ ID = 51, model = "vectre", name = "Emperor Vectre", price = 200000, inventoryCapacity = 150, defaultTax = 200, petrolTank = 120, fuelConsumption = 2, picture = "https://cdn.discordapp.com/attachments/785199429488738315/870364956758007858/unknown.png" },
            new SellingVehicleModel(){ ID = 52, model = "growler", name = "Pfister Growler", price = 250000, inventoryCapacity = 100, defaultTax = 250, petrolTank = 120, fuelConsumption = 2, picture = "https://cdn.discordapp.com/attachments/785199429488738315/870365176636006520/unknown.png" },
            new SellingVehicleModel(){ ID = 53, model = "dominator7", name = "威皮 Dominator ASP", price = 70000, inventoryCapacity = 150, defaultTax = 28, petrolTank = 60, fuelConsumption = 2, picture = "https://cdn.discordapp.com/attachments/785199429488738315/870365389257842758/unknown.png" },
            new SellingVehicleModel(){ ID = 54, model = "sultan3", name = "Sultan RS Classic", price = 100000, inventoryCapacity = 170, defaultTax = 100, petrolTank = 60, fuelConsumption = 2, picture = "https://cdn.discordapp.com/attachments/785199429488738315/870365734058983484/unknown.png" },
            new SellingVehicleModel(){ ID = 55, model = "previon", name = "Karin Previon", price = 90000, inventoryCapacity = 250, defaultTax = 50, petrolTank = 60, fuelConsumption = 2, picture = "https://cdn.discordapp.com/attachments/785199429488738315/870365969841795122/unknown.png" },
            new SellingVehicleModel(){ ID = 56, model = "jester4", name = "Dinka Jester RR", price = 350000, inventoryCapacity = 100, defaultTax = 350, petrolTank = 120, fuelConsumption = 2, picture = "https://cdn.discordapp.com/attachments/785199429488738315/870366234414293032/unknown.png" },
            new SellingVehicleModel(){ ID = 57, model = "dominator8", name = "威皮 Dominator GTT", price = 60000, inventoryCapacity = 150, defaultTax = 25, petrolTank = 60, fuelConsumption = 2, picture = "https://cdn.discordapp.com/attachments/785199429488738315/870366511028662283/unknown.png" },
            new SellingVehicleModel(){ ID = 58, model = "remus", name = "Annis Remus", price = 90000, inventoryCapacity = 250, defaultTax = 40, petrolTank = 60, fuelConsumption = 2, picture = "https://cdn.discordapp.com/attachments/785199429488738315/870366754017255494/unknown.png" },
            new SellingVehicleModel(){ ID = 59, model = "futo2", name = "Karin Futo GXT", price = 50000, inventoryCapacity = 110, defaultTax = 20, petrolTank = 60, fuelConsumption = 2, picture = "https://cdn.discordapp.com/attachments/785199429488738315/870366979771498618/unknown.png" },
            new SellingVehicleModel(){ ID = 60, model = "tailgater2", name = "Obey Tailgater S", price = 120000, inventoryCapacity = 150, defaultTax = 120, petrolTank = 90, fuelConsumption = 2, picture = "https://cdn.discordapp.com/attachments/785199429488738315/870367156456550410/unknown.png" },
            new SellingVehicleModel(){ ID = 61, model = "euros", name = "Annis Euros", price = 160000, inventoryCapacity = 230, defaultTax = 160, petrolTank = 100, fuelConsumption = 2, picture = "https://cdn.discordapp.com/attachments/785199429488738315/870367410165800990/unknown.png" },
            new SellingVehicleModel(){ ID = 62, model = "rt3000", name = "Dinka RT3000", price = 180000, inventoryCapacity = 170, defaultTax = 180, petrolTank = 100, fuelConsumption = 2, picture = "https://cdn.discordapp.com/attachments/785199429488738315/870367670447530004/unknown.png" },
            new SellingVehicleModel(){ ID = 63, model = "warrener2", name = "Vulcar Warrener S", price = 40000, inventoryCapacity = 350, defaultTax = 12, petrolTank = 60, fuelConsumption = 2, picture = "https://cdn.discordapp.com/attachments/785199429488738315/870367858008403978/unknown.png" },
            new SellingVehicleModel(){ ID = 64, model = "zr350", name = "Annis ZR350", price = 100000, inventoryCapacity = 120, defaultTax = 100, petrolTank = 60, fuelConsumption = 2, picture = "https://cdn.discordapp.com/attachments/785199429488738315/870368085188702238/unknown.png" },
            new SellingVehicleModel(){ ID = 65, model = "cypher", name = "Übermacht Cypher", price = 170000, inventoryCapacity = 170, defaultTax = 170, petrolTank = 100, fuelConsumption = 2, picture = "https://cdn.discordapp.com/attachments/785199429488738315/870373389657866250/unknown.png" },
            new SellingVehicleModel(){ ID = 66, model = "ignus", name = "Pegassi Ignus", price = 470000, inventoryCapacity = 120, defaultTax = 450, petrolTank = 120, fuelConsumption = 3, picture = "https://cdn.discordapp.com/attachments/785199429488738315/870373389657866250/unknown.png" },
            new SellingVehicleModel(){ ID = 67, model = "corsita", name = "Lampadati Corsita", price = 400000, inventoryCapacity = 120, defaultTax = 400, petrolTank = 100, fuelConsumption = 2, picture = "https://cdn.discordapp.com/attachments/785199429488738315/870373389657866250/unknown.png" },
            new SellingVehicleModel(){ ID = 68, model = "sm722", name = "Benefactor SM722", price = 350000, inventoryCapacity = 120, defaultTax = 350, petrolTank = 90, fuelConsumption = 2, picture = "https://cdn.discordapp.com/attachments/785199429488738315/870373389657866250/unknown.png" },
            new SellingVehicleModel(){ ID = 69, model = "torero2", name = "Pegassi Torero XO", price = 550000, inventoryCapacity = 120, defaultTax = 550, petrolTank = 100, fuelConsumption = 3, picture = "https://cdn.discordapp.com/attachments/785199429488738315/870373389657866250/unknown.png" },


        };

        // TODO ticari

        public static List<SellingVehicleModel> jobcars = new List<SellingVehicleModel>()
        {
            new SellingVehicleModel(){ ID = 1, model = "benson", name = "威皮 Benson", price = 74000, inventoryCapacity = 1350, defaultTax = 50, petrolTank = 180, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/b/bd/Benson.png" },
            new SellingVehicleModel(){ ID = 2, model = "biff", name = "HVY Biff", price = 21000, inventoryCapacity = 850, defaultTax = 21, petrolTank = 110, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/6/62/Biff.png" },
            new SellingVehicleModel(){ ID = 3, model = "hauler", name = "Jobuilt Hauler", price = 70000, inventoryCapacity = 200, defaultTax = 70, petrolTank = 200, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/8/83/Hauler.png" },
            new SellingVehicleModel(){ ID = 4, model = "mule", name = "Maibatsu Mule", price = 23000, inventoryCapacity = 850, defaultTax = 15, petrolTank = 220, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/e/e7/Mule.png" },
            new SellingVehicleModel(){ ID = 5, model = "packer", name = "MTL Packer", price = 77000, inventoryCapacity = 250, defaultTax = 77, petrolTank = 215, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/0/03/Packer.png" },
            new SellingVehicleModel(){ ID = 6, model = "phantom", name = "Jobuilt Phantom", price = 81000, inventoryCapacity = 390, defaultTax = 81, petrolTank = 230, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/e/ec/Phantom.png" },
            new SellingVehicleModel(){ ID = 7, model = "phantom3", name = "Jobuilt Phantom Custom", price = 150000, inventoryCapacity = 400, defaultTax = 150, petrolTank = 290, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/8/81/Phantom3.png" },
            new SellingVehicleModel(){ ID = 8, model = "pounder", name = "MTL Pounder", price = 80000, inventoryCapacity = 1450, defaultTax = 80, petrolTank = 195, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/a/a6/Pounder.png" },
            new SellingVehicleModel(){ ID = 9, model = "stockade", name = "Brute Stockade", price = 90000, inventoryCapacity = 800, defaultTax = 90, petrolTank = 80, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/6/6b/Stockade.png" },
            new SellingVehicleModel(){ ID = 10, model = "scrap", name = "威皮 Scrap Truck", price = 16000, inventoryCapacity = 300, defaultTax = 16, petrolTank = 70, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/b/b8/Scrap.png" },
            new SellingVehicleModel(){ ID = 11, model = "towtruck", name = "威皮 Towtruck", price = 30000, inventoryCapacity = 20, defaultTax = 14, petrolTank = 80, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/5/52/Towtruck.png" },
            new SellingVehicleModel(){ ID = 12, model = "towtruck2", name = "威皮 Slamvan Towtruck", price = 26000, inventoryCapacity = 17, defaultTax = 26, petrolTank = 65, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/3/34/Towtruck2.png" },
            new SellingVehicleModel(){ ID = 13, model = "tractor2", name = "Stanley Fieldmaster Tractor", price = 40000, inventoryCapacity = 15, defaultTax = 12, petrolTank = 55, fuelConsumption = 3, picture = "https://wiki.altv.mp/images/c/c8/Tractor2.png" },
            new SellingVehicleModel(){ ID = 14, model = "forklift", name = "HVY Forklift", price = 18000, inventoryCapacity = 5, defaultTax = 18, petrolTank = 80, fuelConsumption = 0, picture = "https://wiki.altv.mp/images/a/ab/Forklift.png" },
            new SellingVehicleModel(){ ID = 15, model = "caddy", name = "Nagasaki Golf Caddy", price = 47000, inventoryCapacity = 20, defaultTax = 47, petrolTank = 60, fuelConsumption = 0, picture = "https://wiki.altv.mp/images/9/94/Caddy.png" },
            new SellingVehicleModel(){ ID = 16, model = "caddy2", name = "Nagasaki Civil Caddy", price = 47000, inventoryCapacity = 20, defaultTax = 47, petrolTank = 60, fuelConsumption = 0, picture = "https://wiki.altv.mp/images/2/25/Caddy2.png" },
            new SellingVehicleModel(){ ID = 17, model = "utillitruck3", name = "威皮 Utility Truck", price = 20000, inventoryCapacity = 400, defaultTax = 20, petrolTank = 65, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/a/aa/Utillitruck3.png" },
            new SellingVehicleModel(){ ID = 18, model = "bison", name = "Bravado Bison", price = 24000, inventoryCapacity = 450, defaultTax = 24, petrolTank = 90, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/f/f6/Bison.png" },
            new SellingVehicleModel(){ ID = 19, model = "bison2", name = "Bravado McGill-Olsen Bison", price = 22500, inventoryCapacity = 450, defaultTax = 23, petrolTank = 90, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/4/44/Bison2.png" },
            new SellingVehicleModel(){ ID = 20, model = "bison3", name = "Bravado Mighty Bush Bison", price = 22500, inventoryCapacity = 450, defaultTax = 23, petrolTank = 90, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/7/7f/Bison3.png" },
            new SellingVehicleModel(){ ID = 21, model = "youga", name = "Bravado Youga", price = 22000, inventoryCapacity = 750, defaultTax = 10, petrolTank = 100, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/d/d5/Youga.png" },
            new SellingVehicleModel(){ ID = 22, model = "youga2", name = "Bravado Youga Classic", price = 19000, inventoryCapacity = 750, defaultTax = 10, petrolTank = 100, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/e/e6/Youga2.png" },
            new SellingVehicleModel(){ ID = 23, model = "youga3", name = "Bravado Youga 4X4", price = 40500, inventoryCapacity = 750, defaultTax = 20, petrolTank = 100, fuelConsumption = 3, picture = "https://wiki.rage.mp/images/0/00/Youga3.png" },
            new SellingVehicleModel(){ ID = 24, model = "pony", name = "Brute Pony", price = 21000, inventoryCapacity = 760, defaultTax = 10, petrolTank = 90, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/b/b1/Pony.png" },
            new SellingVehicleModel(){ ID = 25, model = "burrito", name = "Declasse McGill-Olsen Burrito", price = 28500, inventoryCapacity = 790, defaultTax = 12, petrolTank = 80, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/e/e0/Burrito.png" },
            new SellingVehicleModel(){ ID = 26, model = "burrito2", name = "Declasse Bugstars Burrito", price = 28500, inventoryCapacity = 790, defaultTax = 12, petrolTank = 80, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/5/52/Burrito2.png" },
            new SellingVehicleModel(){ ID = 27, model = "burrito3", name = "Declasse Burrito", price = 30000, inventoryCapacity = 790, defaultTax = 15, petrolTank = 80, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/8/89/Burrito3.png" },
            new SellingVehicleModel(){ ID = 28, model = "bobcatxl", name = "威皮 Bobcat XL", price = 24000, inventoryCapacity = 550, defaultTax = 12, petrolTank = 65, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/5/50/Bobcatxl.png" },
            new SellingVehicleModel(){ ID = 29, model = "boxville", name = "Brute LSDWP Boxville", price = 26000, inventoryCapacity = 900, defaultTax = 26, petrolTank = 110, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/0/09/Boxville.png" },
            new SellingVehicleModel(){ ID = 30, model = "boxville2", name = "Brute GoPostal Boxville", price = 26000, inventoryCapacity = 900, defaultTax = 26, petrolTank = 110, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/f/fe/Boxville2.png" },
            new SellingVehicleModel(){ ID = 31, model = "boxville3", name = "Brute Humane L&R Boxville", price = 26000, inventoryCapacity = 900, defaultTax = 26, petrolTank = 110, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/5/5c/Boxville3.png" },
            new SellingVehicleModel(){ ID = 32, model = "boxville4", name = "Brute Post OP Boxville", price = 26000, inventoryCapacity = 900, defaultTax = 26, petrolTank = 110, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/2/29/Boxville4.png" },
            new SellingVehicleModel(){ ID = 33, model = "taco", name = "Brute Taco Van", price = 18000, inventoryCapacity = 400, defaultTax = 18, petrolTank = 100, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/4/4d/Taco.png" },
            new SellingVehicleModel(){ ID = 34, model = "speedo", name = "威皮 Speedo", price = 27000, inventoryCapacity = 620, defaultTax = 15, petrolTank = 80, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/2/2b/Speedo.png" },
            new SellingVehicleModel(){ ID = 35, model = "speedo2", name = "威皮 Speedo Clown Van", price = 30000, inventoryCapacity = 600, defaultTax = 17, petrolTank = 80, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/5/53/Speedo2.png" },
            new SellingVehicleModel(){ ID = 36, model = "gburrito2", name = "Declasse Gang Burrito", price = 34000, inventoryCapacity = 550, defaultTax = 20, petrolTank = 70, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/5/5d/Gburrito2.png" },
            new SellingVehicleModel(){ ID = 37, model = "paradise", name = "Bravado Paradise", price = 27500, inventoryCapacity = 550, defaultTax = 15, petrolTank = 70, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/b/b3/Paradise.png" },
            new SellingVehicleModel(){ ID = 38, model = "minivan", name = "威皮 Minivan", price = 27500, inventoryCapacity = 350, defaultTax = 15, petrolTank = 65, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/1/12/Minivan.png" },
            new SellingVehicleModel(){ ID = 39, model = "minivan2", name = "威皮 Minivan", price = 36000, inventoryCapacity = 350, defaultTax = 17, petrolTank = 65, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/2/21/Minivan2.png" },
            new SellingVehicleModel(){ ID = 40, model = "rumpo", name = "Bravado Rumpo", price = 26000, inventoryCapacity = 620, defaultTax = 13, petrolTank = 80, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/9/9f/Rumpo.png" },
            new SellingVehicleModel(){ ID = 41, model = "rumpo2", name = "Bravado Deludamol Rumpo", price = 26400, inventoryCapacity = 620, defaultTax = 14, petrolTank = 80, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/2/20/Rumpo2.png" },
            new SellingVehicleModel(){ ID = 42, model = "camper", name = "Brute Camper", price = 100000, inventoryCapacity = 850, defaultTax = 100, petrolTank = 200, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/b/bd/Camper.png" },
            new SellingVehicleModel(){ ID = 43, model = "journey", name = "Zirconium Journey", price = 60500, inventoryCapacity = 240, defaultTax = 65, petrolTank = 150, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/0/0c/Journey.png" },
            new SellingVehicleModel(){ ID = 44, model = "surfer", name = "BF Surfer", price = 28500, inventoryCapacity = 240, defaultTax = 29, petrolTank = 80, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/d/d7/Surfer.png" },
            new SellingVehicleModel(){ ID = 45, model = "surfer2", name = "BF Rusty Surfer", price = 13600, inventoryCapacity = 240, defaultTax = 14, petrolTank = 80, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/d/d5/Surfer2.png" },
            new SellingVehicleModel(){ ID = 46, model = "taxi", name = "Taxi", price = 15000, inventoryCapacity = 150, defaultTax = 25, petrolTank = 60, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/thumb/8/81/Streiter.png/800px-Streiter.png" },
            new SellingVehicleModel(){ ID = 47, model = "slamtruck", name = "威皮 Slamtruck", price = 48000, inventoryCapacity = 50, defaultTax = 48, petrolTank = 50, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/9/99/Slamtruck.png" },
            new SellingVehicleModel(){ ID = 48, model = "youga4", name = "威皮 Youga Custom", price = 50000, inventoryCapacity = 240, defaultTax = 48, petrolTank = 100, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/9/99/Slamtruck.png" },
            new SellingVehicleModel(){ ID = 49, model = "mule5", name = "Maibatsu Mule Custom", price = 35000, inventoryCapacity = 350, defaultTax = 20, petrolTank = 220, fuelConsumption = 2, picture = "https://wiki.altv.mp/images/9/99/Slamtruck.png" },
        };

    }
}
