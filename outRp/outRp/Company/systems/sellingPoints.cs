using System;
using System.Collections.Generic;
using AltV.Net; using AltV.Net.Async;
using AltV.Net.Data;
using outRp.Models;
using System.Threading.Tasks;
using outRp.Chat;
using outRp.OtherSystem.Textlabels;
using System.Linq;
using AltV.Net.Resources.Chat.Api;
using outRp.OtherSystem;
using outRp.OtherSystem.NativeUi;
//using static outRp.OtherSystem.LSCsystems.farming;
using static outRp.OtherSystem.LSCsystems.WeedVendors;

namespace outRp.Company.systems
{
    class sellingPoints : IScript
    {
        public class SellPoint
        {
            public int ID { get; set; } = 0;
            public int Owner_Company { get; set; } = 0;
            public ulong TextlblID { get; set; } = 0;

            /*
             * Types: 1: car | 2: Food | 3: Farm (biz) | 
             * 4: Farm (buy-mahsül) | 5: Farm (buy-Taze Et) |
             * 6: Cloth | 7: Stock
             */
            public int Type { get; set; } = 0;
            public int Type_2 { get; set; } = 0;
            public int vault { get; set; } = 0;
            public int priceMultipler { get; set; } = 0;
            public int Dimension { get; set; } = 0;
            public int stock { get; set; } = 0;
            public bool canBuy { get; set; } = false;
            public Position Position { get; set; } = new Position(0, 0, 0);
            public Position Position_2 { get; set; } = new Position(0, 0, 0);

            public void Update() => Database.BusinessDatabase.UpdateSellPoint(this);
        }

        public static List<SellPoint> companySellPoints = new List<SellPoint>();

        public static async Task LoadAllCompanySellPoints()
        {
            companySellPoints = Database.BusinessDatabase.GetAllSellPoints();

            foreach (SellPoint po in companySellPoints)
            {
                Models.CompanyModel company = await Database.BusinessDatabase.GetCompany(po.Owner_Company);
                if (po.Type == 1)
                    po.TextlblID = TextLabelStreamer.Create("~b~[" + company.Name + "]~n~~w~车辆经销商~n~~g~/buy", po.Position, dimension: 0, streamRange: 5, font: 0).Id;
                else if(po.Type == 6)
                    po.TextlblID = TextLabelStreamer.Create("~b~[" + company.Name + "]~n~~w~车辆改装店~n~~g~/buy", po.Position, dimension: 0, streamRange: 5, font: 0).Id;
                else po.TextlblID = 0;
            }
        }


        [Command("addsellpoint")]
        public static void COM_AddSellPoint(PlayerModel p, params string[] args)
        {
            Models.CompanyModel company = Database.BusinessDatabase.GetPlayerOwnCompany(p.sqlID);
            if(company == null) { MainChat.SendErrorChat(p, "[错误] 您没有公司!"); return; }

            List<GuiMenu> gMenu = new List<GuiMenu>();


            GuiMenu m1 = new GuiMenu { name = "车辆经销商", triger = "Company:AddSellPoint", value = "car" };
            GuiMenu m3 = new GuiMenu { name = "农场1", triger = "Company:AddSellPoint", value = "farm" };
            GuiMenu m4 = new GuiMenu { name = "农场2", triger = "Company:AddSellPoint", value = "farm_2" };
            GuiMenu m5 = new GuiMenu { name = "农场3", triger = "Company:AddSellPoint", value = "farm_3" };
            GuiMenu m6 = new GuiMenu { name = "车辆改装店", triger = "Company:AddSellPoint", value = "cloth" };

            gMenu.Add(m1); gMenu.Add(m3); gMenu.Add(m4);
            gMenu.Add(m6); gMenu.Add(m5);


            GuiMenu close = GuiEvents.closeItem;
            gMenu.Add(close);

            Gui y = new Gui()
            {
                image = "https://img.pngio.com/building-business-company-estate-office-work-icon-company-png-512_512.png",
                guiMenu = gMenu,
                color = "#00AAFF"
            };
            y.Send(p);
        }

        [Command("editsellpos")]
        public void EVENT_CarSellPos(PlayerModel p, params string[] args)
        {
            Models.CompanyModel company = Database.BusinessDatabase.GetPlayerOwnCompany(p.sqlID);
            if (company == null) { MainChat.SendErrorChat(p, "[错误] 您没有公司!"); return; }
            SellPoint point = companySellPoints.Where(x => x.Position.Distance(p.Position) < 40 && x.Dimension == p.Dimension
            && x.Type == 1 && x.Owner_Company == company.ID).FirstOrDefault();

            if(point == null) { MainChat.SendErrorChat(p, "[错误] 未找到销售点!"); return; }
            point.Position_2 = p.Position;
            point.Update();
            MainChat.SendInfoChat(p, "[*] 成功更新车辆初始购买刷新点.");
            return;
        }
        [AsyncClientEvent("Company:AddSellPoint")]
        public static async Task Event_AddSellPoint(PlayerModel p, string value)
        {
            if (p.Ping > 250)
                return;
            GuiEvents.GuiClose(p);
            Models.CompanyModel company = Database.BusinessDatabase.GetPlayerOwnCompany(p.sqlID);
            if (company == null) { MainChat.SendErrorChat(p, "[错误] 您没有公司!"); return; }

            SellPoint point = new SellPoint();
            var company_components = Component_System.GetAllComponentsFromCompany(company.ID);

            switch (value)
            {
                case "car":

                    Models.Components digital = company_components.Where(x => x.Type == 7).FirstOrDefault();
                    Models.Components iron = company_components.Where(x => x.Type == 6).FirstOrDefault();
                    Models.Components steel = company_components.Where(x => x.Type == 5).FirstOrDefault();
                    Models.Components plastic = company_components.Where(x => x.Type == 3).FirstOrDefault();

                    if(digital == null || iron == null || steel == null || plastic == null) { MainChat.SendErrorChat(p, "[错误] 公司仓库没有足够的材料开设车辆经销商!<br>需要材料:<br>电子材料: 8000<br>铁: 6500<br>钢材: 5000<br>塑料: 3000"); return; }
                    if (digital.Stock_1 < 200 && iron.Stock_1 < 1000 && steel.Stock_1 < 1000 && plastic.Stock_1 < 500) { MainChat.SendErrorChat(p, "[错误] 公司仓库没有足够的材料开设车辆经销商!<br>需要材料:<br>电子材料: 8000<br>铁: 6500<br>钢材: 5000<br>塑料: 3000"); return; }
                    digital.Stock_1 -= 200; TextLabelStreamer.GetDynamicTextLabel(digital.TextLabelID).Text = systems.Component_System.GetComponentDisplayName(digital.Type, digital.Stock_1, digital.Stock_2, digital.Stock_3, digital.SecurityLevel); await digital.Update();
                    iron.Stock_1 -= 1000; TextLabelStreamer.GetDynamicTextLabel(iron.TextLabelID).Text = systems.Component_System.GetComponentDisplayName(iron.Type, iron.Stock_1, iron.Stock_2, iron.Stock_3, iron.SecurityLevel); await iron.Update();
                    steel.Stock_1 -= 1000; TextLabelStreamer.GetDynamicTextLabel(steel.TextLabelID).Text = systems.Component_System.GetComponentDisplayName(steel.Type, steel.Stock_1, steel.Stock_2, steel.Stock_3, steel.SecurityLevel); await steel.Update();
                    plastic.Stock_1 -= 500; TextLabelStreamer.GetDynamicTextLabel(plastic.TextLabelID).Text = systems.Component_System.GetComponentDisplayName(plastic.Type, plastic.Stock_1, plastic.Stock_2, plastic.Stock_3, plastic.SecurityLevel); await plastic.Update();

                    point.Type = 1;
                    point.Position = p.Position;
                    point.Owner_Company = company.ID;
                    point.Dimension = p.Dimension;

                    point.ID = Database.BusinessDatabase.CreateSellPoint(point);

                    companySellPoints.Add(point);

                    List<GuiMenu> gMenu = new List<GuiMenu>();


                    GuiMenu m1 = new GuiMenu { name = "SUV", triger = "Company:VehicleType", value = "suv," + point.ID };
                    GuiMenu m2 = new GuiMenu { name = "轿车", triger = "Company:VehicleType", value = "sedan," + point.ID };
                    GuiMenu m3 = new GuiMenu { name = "自行车", triger = "Company:VehicleType", value = "bisiklet," + point.ID };
                    GuiMenu m4 = new GuiMenu { name = "摩托车", triger = "Company:VehicleType", value = "motor," + point.ID };
                    GuiMenu m5 = new GuiMenu { name = "跑车", triger = "Company:VehicleType", value = "spor," + point.ID };

                    gMenu.Add(m1); gMenu.Add(m2); gMenu.Add(m3); gMenu.Add(m4); gMenu.Add(m5);


                    GuiMenu close = GuiEvents.closeItem;
                    gMenu.Add(close);

                    Gui y = new Gui()
                    {
                        image = "https://img.pngio.com/building-business-company-estate-office-work-icon-company-png-512_512.png",
                        guiMenu = gMenu,
                        color = "#00AAFF",
                        info = "请选择要出售的车辆类型"
                    };
                    y.Send(p);

                    point.TextlblID = TextLabelStreamer.Create("~b~[" + company.Name + "]~n~~w~车辆经销商~n~~g~/buy", point.Position, dimension: 0, streamRange: 5, font: 0).Id;

                    break;

                case "food":
                    /*point.Type = 2;
                    point.Position = p.Position;
                    point.Owner_Company = company.ID;
                    point.Dimension = p.Dimension;

                    point.ID = Database.BusinessDatabase.CreateSellPoint(point);

                    companySellPoints.Add(point);*/
                    break;

                case "farm":                   
                    Inputs.SendTypeInput(p, "输入将开设农场的公司 ID.", "Company:AddFarm", "");
                    GuiEvents.GuiClose(p);
                    break;

                case "farm_2":
                    OtherSystem.PedModel x = PedStreamer.Create("u_m_y_antonb", p.Position, p.Dimension);

                    Vendor n = new Vendor()
                    {
                        ID = x.Id,
                        Name =  "~w~["+company.Name+ "]~w~~n~购买~n~类型: ~o~农作物",
                        buyPrice = 100,
                        buyType = 23,
                        pos = p.Position,
                        dimension = p.Dimension,
                        OwnerCompany = company.ID,
                        Model = "u_m_y_antonb"
                    };
                    x.nametag = n.Name;
                    weedVendors.Add(n);
                    MainChat.SendInfoChat(p, "[!] 成功开设农场销售点.");
                    break;

                case "farm_3":
                    OtherSystem.PedModel xy = PedStreamer.Create("u_m_y_antonb", p.Position, p.Dimension);

                    Vendor nn = new Vendor()
                    {
                        ID = xy.Id,
                        Name = "~w~[" + company.Name + "]~w~~n~购买~n~类型: ~o~肉类",
                        buyPrice = 100,
                        buyType = 34,
                        pos = p.Position,
                        dimension = p.Dimension,
                        OwnerCompany = company.ID,
                        Model = "u_m_y_antonb"
                    };
                    xy.nametag = nn.Name;
                    weedVendors.Add(nn);
                    MainChat.SendInfoChat(p, "[!] 成功开设农场销售点.");
                    break;

                case "cloth":
                    var cloth_plastic = company_components.Find(x => x.Type == 3);
                    var cloth_iron = company_components.Find(x => x.Type == 6);
                    if(cloth_iron == null || cloth_plastic == null) { MainChat.SendErrorChat(p, "[错误] 公司仓库没有铁和塑料来开设新的销售点!"); return; }
                    if(cloth_plastic.Stock_1 < 1000 || cloth_iron.Stock_1 < 200) { MainChat.SendErrorChat(p, "[错误] 公司仓库没有足够的铁和塑料来开设新的销售点!"); return; }

                    cloth_iron.Stock_1 -= 200; TextLabelStreamer.GetDynamicTextLabel(cloth_iron.TextLabelID).Text = systems.Component_System.GetComponentDisplayName(cloth_iron.Type, cloth_iron.Stock_1, cloth_iron.Stock_2, cloth_iron.Stock_3, cloth_iron.SecurityLevel); await cloth_iron.Update();
                    cloth_plastic.Stock_1 -= 100; TextLabelStreamer.GetDynamicTextLabel(cloth_plastic.TextLabelID).Text = systems.Component_System.GetComponentDisplayName(cloth_plastic.Type, cloth_plastic.Stock_1, cloth_plastic.Stock_2, cloth_plastic.Stock_3, cloth_plastic.SecurityLevel); await cloth_plastic.Update();

                    point.Type = 6;
                    point.Position = p.Position;
                    point.Owner_Company = company.ID;
                    point.Dimension = p.Dimension;

                    point.ID = Database.BusinessDatabase.CreateSellPoint(point);
                    point.TextlblID = TextLabelStreamer.Create("~b~[" + company.Name + "]~n~~w~车辆改装店~n~~g~/buy", point.Position, dimension: 0, streamRange: 5, font: 0).Id;
                    companySellPoints.Add(point);

                    MainChat.SendErrorChat(p, "[?] " + company.Name + " 开设了新的车辆改装店.<br>花费:<br>塑料 - 1000 | 铁 - 200");
                    break;

                case "stock":
                    break;

                default:
                    return;
            }
            return;
        }

        [AsyncClientEvent("Company:VehicleType")]
        public static void SelectVehicleType(PlayerModel p, string value)
        {
            if (p.Ping > 250)
                return;
            GuiEvents.GuiClose(p);
            string[] val = value.Split(",");
            SellPoint point = companySellPoints.Find(x => x.ID == Convert.ToInt32(val[1]));
            if (point == null) return;
            switch (val[0])
            {
                case "suv":
                    point.Type_2 = 1;
                    break;

                case "sedan":
                    point.Type_2 = 2;
                    break;

                case "bisiklet":
                    point.Type_2 = 3;
                    break;

                case "motor":
                    point.Type_2 = 4;
                    break;

                case "sport":
                    point.Type_2 = 5;
                    break;
            }
            point.Update();
            MainChat.SendInfoChat(p, "[*] 成功更新车辆经销商出售的车辆类型为: " + val[1]);
            return;
        }

        [AsyncClientEvent("Company:AddFarm")]
        public static async Task CompanyAddFarmToBusiness(PlayerModel p, string ID, string _trash)
        {
            if (p.Ping > 250)
                return;
            Models.CompanyModel company = Database.BusinessDatabase.GetPlayerOwnCompany(p.sqlID);
            if (company == null) { MainChat.SendErrorChat(p, "[错误] 您没有公司!"); return; }
           
            if (!Int32.TryParse(ID, out int bizID)) { MainChat.SendErrorChat(p, "无效的产业ID"); return; }

            (BusinessModel, Marker, PlayerLabel) biz = await Props.Business.getBusinessById(bizID);

            if (biz.Item1 == null) { MainChat.SendErrorChat(p, "无效产业"); Inputs.SendTypeInput(p, "输入将开设农场的公司ID.", "Company:AddFarm", ""); return; }
            if(biz.Item1.type != 6) { MainChat.SendErrorChat(p, "[错误] 输入的产业类型不适合开设农场!"); return; }

            var company_components = Database.BusinessDatabase.GetAllCompanyComponent(company.ID);

            var demir_farm = company_components.Find(x => x.Type == 6);
            var toprak_farm = company_components.Find(x => x.Type == 0);
            var odun_farm = company_components.Find(x => x.Type == 1);
            if (demir_farm == null || toprak_farm == null || odun_farm == null) { MainChat.SendErrorChat(p, "[错误] 公司仓库没有足够的材料开设新的销售点! 需要材料:<br>铁: 1000<br>泥土: 5000<br>木板: 3000"); return; }
            if (demir_farm.Stock_1 < 500 || toprak_farm.Stock_1 < 700 || odun_farm.Stock_1 < 2500) { MainChat.SendErrorChat(p, "[错误] 公司仓库没有足够的材料开设新的销售点! 需要材料:<br>铁: 1000<br>泥土: 5000<br>木板: 3000"); return; }

            demir_farm.Stock_1 -= 500; TextLabelStreamer.GetDynamicTextLabel(demir_farm.TextLabelID).Text = systems.Component_System.GetComponentDisplayName(demir_farm.Type, demir_farm.Stock_1, demir_farm.Stock_2, demir_farm.Stock_3, demir_farm.SecurityLevel); await demir_farm.Update();
            toprak_farm.Stock_1 -= 700; TextLabelStreamer.GetDynamicTextLabel(toprak_farm.TextLabelID).Text = systems.Component_System.GetComponentDisplayName(toprak_farm.Type, toprak_farm.Stock_1, toprak_farm.Stock_2, toprak_farm.Stock_3, toprak_farm.SecurityLevel); await toprak_farm.Update();
            odun_farm.Stock_1 -= 2500; TextLabelStreamer.GetDynamicTextLabel(odun_farm.TextLabelID).Text = systems.Component_System.GetComponentDisplayName(odun_farm.Type, odun_farm.Stock_1, odun_farm.Stock_2, odun_farm.Stock_3, odun_farm.SecurityLevel); await odun_farm.Update();

            /*
            farmModel f = new farmModel();
            f.businessId = biz.Item1.ID;
            f.farmPos = p.Position;
            f.textlblId = (int)TextLabelStreamer.Create("~b~Çiftlik~n~~w~KapıNo: " + f.businessId.ToString(), f.farmPos, font: 0, streamRange: 10).Id;
            serverFarms.Add(f);*/
            MainChat.SendInfoChat(p, "成功开设新的农场.<br>花费:<br> 铁 - 500 | 泥土 - 700 | 木板 - 2500");
            return;
        }

        public static SellPoint GetNearSellPoint(Position pos, int dimension = 0, int type = -1)
        {
            
            //if (type != -1)
              //  return companySellPoints.Where(x => x.Position.Distance(pos) < 15 && x.Dimension == dimension && x.Type == type).FirstOrDefault();
            //else
                return companySellPoints.Where(x => x.Position.Distance(pos) < 15 && x.Dimension == dimension).FirstOrDefault();
        }

        public static bool Key_Buy(PlayerModel p)
        {
            SellPoint point = companySellPoints.Where(x => x.Position.Distance(p.Position) < 5 && x.Dimension == p.Dimension).FirstOrDefault();
            if(point == null) return false;
            if (!point.canBuy) { MainChat.SendErrorChat(p, "[错误] 此店目前不销售产品!"); return false; }

            switch (point.Type)
            {                
                case 1: // Araç
                    
                    p.SetData("VehicleVendor:Company", point.Owner_Company);
                    p.SetData("VehicleVendor:SellingPos", point.Position_2);
                    p.SetData("VehicleVendor:Type", point.Type_2);
                    List<GuiMenu> gMenu = new List<GuiMenu>();
                    switch (point.Type_2)
                    {
                        case 1: // suv
                            GuiMenu offroad = new GuiMenu { name = "越野车", triger = "ShowCar:SelectedList", value = "1" };// old 1
                            GuiMenu suv = new GuiMenu { name = "SUV", triger = "ShowCar:SelectedList", value = "2" };// old 2
                            gMenu.Add(offroad);
                            gMenu.Add(suv);
                            break;

                        case 2: // sedan
                            GuiMenu sedans = new GuiMenu { name = "轿车", triger = "ShowCar:SelectedList", value = "3" };// old 3
                            GuiMenu muscules = new GuiMenu { name = "肌肉车", triger = "ShowCar:SelectedList", value = "4" };// old 4
                            GuiMenu coupes = new GuiMenu { name = "轿跑车", triger = "ShowCar:SelectedList", value = "5" }; // old 5
                            GuiMenu compacts = new GuiMenu { name = "小轿车", triger = "ShowCar:SelectedList", value = "6" }; // old 6
                            gMenu.Add(sedans);
                            gMenu.Add(muscules);
                            gMenu.Add(coupes);
                            gMenu.Add(compacts);
                            break;

                        case 3: // bisiklet
                            GuiMenu cycles = new GuiMenu { name = "自行车", triger = "ShowCar:SelectedList", value = "7" };// old 7
                            gMenu.Add(cycles);
                            break;

                        case 4: // motor
                            GuiMenu motocycles = new GuiMenu { name = "摩托车", triger = "ShowCar:SelectedList", value = "8" };// old 8
                            gMenu.Add(motocycles);
                            break;

                        case 5: // sport
                            GuiMenu supercars = new GuiMenu { name = "超级跑车", triger = "ShowCar:SelectedList", value = "9" };// old 9
                            GuiMenu sports = new GuiMenu { name = "跑车", triger = "ShowCar:SelectedList", value = "10" }; // old 10
                            GuiMenu sportclassic = new GuiMenu { name = "经典跑车", triger = "ShowCar:SelectedList", value = "11" };// old 11
                            gMenu.Add(supercars);
                            gMenu.Add(sports);
                            gMenu.Add(sportclassic);
                            break;
                    }
                    GuiMenu close = GuiEvents.closeItem;
                    gMenu.Add(close);
                    Gui y = new Gui()
                    {image = "https://www.upload.ee/files/12278457/1.png.html", guiMenu = gMenu, color = "#4AC27D"};
                    y.Send(p);
                    return true;

                case 2: // Food
                    break;

                case 6: // Cloth ( elbise )
                    p.SetData("ClothingVendor:PointID", point.ID);
                    p.EmitLocked("ClothingMenu:Show");
                    return true;

                case 7: // Stock
                    break;

                default:
                    MainChat.SendErrorChat(p, "[错误] 无可购买的产品!");
                    return false;
            }

            return true;
        }
        
        [Command("buy")]
        public static void COM_Buy(PlayerModel p, params string[] args)
        {
            SellPoint point = companySellPoints.Where(x => x.Position.Distance(p.Position) < 5 && x.Dimension == p.Dimension).FirstOrDefault();
            if(point == null) return;
            if (!point.canBuy) { MainChat.SendErrorChat(p, "[错误] 此店目前不销售产品!"); return; }

            switch (point.Type)
            {                
                case 1: // Araç
                    
                    p.SetData("VehicleVendor:Company", point.Owner_Company);
                    p.SetData("VehicleVendor:SellingPos", point.Position_2);
                    p.SetData("VehicleVendor:Type", point.Type_2);
                    List<GuiMenu> gMenu = new List<GuiMenu>();
                    switch (point.Type_2)
                    {
                        case 1: // suv
                            GuiMenu offroad = new GuiMenu { name = "越野车", triger = "ShowCar:SelectedList", value = "1" };// old 1
                            GuiMenu suv = new GuiMenu { name = "SUV", triger = "ShowCar:SelectedList", value = "2" };// old 2
                            gMenu.Add(offroad);
                            gMenu.Add(suv);
                            break;

                        case 2: // sedan
                            GuiMenu sedans = new GuiMenu { name = "轿车", triger = "ShowCar:SelectedList", value = "3" };// old 3
                            GuiMenu muscules = new GuiMenu { name = "肌肉车", triger = "ShowCar:SelectedList", value = "4" };// old 4
                            GuiMenu coupes = new GuiMenu { name = "轿跑车", triger = "ShowCar:SelectedList", value = "5" }; // old 5
                            GuiMenu compacts = new GuiMenu { name = "小轿车", triger = "ShowCar:SelectedList", value = "6" }; // old 6
                            gMenu.Add(sedans);
                            gMenu.Add(muscules);
                            gMenu.Add(coupes);
                            gMenu.Add(compacts);
                            break;

                        case 3: // bisiklet
                            GuiMenu cycles = new GuiMenu { name = "自行车", triger = "ShowCar:SelectedList", value = "7" };// old 7
                            gMenu.Add(cycles);
                            break;

                        case 4: // motor
                            GuiMenu motocycles = new GuiMenu { name = "摩托车", triger = "ShowCar:SelectedList", value = "8" };// old 8
                            gMenu.Add(motocycles);
                            break;

                        case 5: // sport
                            GuiMenu supercars = new GuiMenu { name = "超级跑车", triger = "ShowCar:SelectedList", value = "9" };// old 9
                            GuiMenu sports = new GuiMenu { name = "跑车", triger = "ShowCar:SelectedList", value = "10" }; // old 10
                            GuiMenu sportclassic = new GuiMenu { name = "经典跑车", triger = "ShowCar:SelectedList", value = "11" };// old 11
                            gMenu.Add(supercars);
                            gMenu.Add(sports);
                            gMenu.Add(sportclassic);
                            break;
                    }
                    GuiMenu close = GuiEvents.closeItem;
                    gMenu.Add(close);
                    Gui y = new Gui()
                    {image = "https://www.upload.ee/files/12278457/1.png.html", guiMenu = gMenu, color = "#4AC27D"};
                    y.Send(p);
                    break;

                case 2: // Food
                    break;

                case 6: // Cloth ( elbise )
                    p.SetData("ClothingVendor:PointID", point.ID);
                    p.EmitLocked("ClothingMenu:Show");
                    break;

                case 7: // Stock
                    break;

                default:
                    MainChat.SendErrorChat(p, "[错误] 无可购买的产品!");
                    return;
            }
        }

        [Command("editbuy")]
        public static void COM_EditSellPoint(PlayerModel p, params string[] args)
        {
            if(args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /editbuy [选项] [数值(如果有)]"); return; }

            SellPoint point = companySellPoints.Where(x => x.Position.Distance(p.Position) < 5 && x.Dimension == p.Dimension).FirstOrDefault();
            if (point == null) { MainChat.SendErrorChat(p, "[错误] 附近没有购买点."); return; }
            var company = Database.BusinessDatabase.GetPlayerOwnCompany(p.sqlID);
            if(company == null) { MainChat.SendErrorChat(p, "[错误] 您没有公司."); return; }
            if(point.Owner_Company != company.ID) { MainChat.SendErrorChat(p, "[错误] 此购买点不属于您公司!"); return; }

            switch (args[0])
            {
                case "pricer":
                    if(!Int32.TryParse(args[1], out int priceMultipler)) { MainChat.SendErrorChat(p, "[!] 无效数值."); return; }
                    if(priceMultipler < 0) { MainChat.SendErrorChat(p, "[!] 无效数值."); return; }
                    point.priceMultipler = priceMultipler;
                    point.Update();
                    break;

                case "lock":
                    point.canBuy = !point.canBuy;
                    string durum_text = (point.canBuy) ? "营业" : "关门";
                    point.Update();
                    MainChat.SendErrorChat(p, "[!] 公司购买点设置为: '" + durum_text);
                    break;

                case "delete":

                    if(point.Type == 1 ||point.Type == 6) // araç
                    {
                        PlayerLabel textlbl = TextLabelStreamer.GetDynamicTextLabel(point.TextlblID);
                        if (textlbl != null)
                            textlbl.Delete();
                    }                

                    companySellPoints.Remove(point);
                    Database.BusinessDatabase.DeleteSellPoint(point.ID);
                    break;
            }
        }

/*        [Command("aliciduzenle")]
        public static void COM_EditSellPointBuyer(PlayerModel p, params string[] args)
        {
            var vendor = weedVendors.Where(x => x.pos.Distance(p.Position) < 5 && x.dimension == p.Dimension).FirstOrDefault();
            if (vendor == null) { MainChat.SendErrorChat(p, "[错误] Alıcı bulunamadı!"); return; }

            var company = Database.BusinessDatabase.GetPlayerOwnCompany(p.sqlID);
            if (vendor.OwnerCompany != company.ID) { MainChat.SendErrorChat(p, "[HATA] Bu alıcı sizin şirketinize bağlı değil!"); return; }

            if(args.Length <= 0) { MainChat.SendInfoChat(p, "[Kullanım] /aliciduzenle [Alış fiyatı]"); return; }
            if(!Int32.TryParse(args[0], out int newPrice)) { MainChat.SendInfoChat(p, "[Kullanım] /aliciduzenle [Alış fiyatı]"); return; }
            if(newPrice <= 0) { MainChat.SendErrorChat(p, "[HATA] Geçerli bir miktar girmelisiniz!"); return; }


            vendor.buyPrice = newPrice;

            if(vendor.buyType == 23)
            {
                vendor.Name = "~w~[" + company.Name + "]~w~~n~Satın Alım~n~Tür: ~o~Masül~n~~g~Fiyat $" + newPrice;
            }
            else if(vendor.buyType == 34)
            {
                vendor.Name = "~w~[" + company.Name + "]~w~~n~Satın Alım~n~Tür: ~o~Taze ET~n~~g~Fiyat $" + newPrice;
            }
            MainChat.SendInfoChat(p, "[?] Alıcı, alış fiyatı $" + newPrice + " olarak düzenlendi.");
            return;
        }

        [Command("alicikaldir")]
        public static void COM_RemoveBuyer(PlayerModel p, params string[] args)
        {
            var vendor = weedVendors.Where(x => x.pos.Distance(p.Position) < 5 && x.dimension == p.Dimension).FirstOrDefault();
            if (vendor == null) { MainChat.SendErrorChat(p, "[HATA] Alıcı bulunamadı!"); return; }

            var company = Database.BusinessDatabase.GetPlayerOwnCompany(p.sqlID);
            if (vendor.OwnerCompany != company.ID) { MainChat.SendErrorChat(p, "[HATA] Bu alıcı sizin şirketinize bağlı değil!"); return; }

            var ped = PedStreamer.Get((ulong)vendor.ID);
            if (ped != null)
                ped.Destroy();

            weedVendors.Remove(vendor);

            MainChat.SendInfoChat(p, "[?] Alıcı başarıyla kaldırıldı.");
            return;
        }

        [Command("satisnoktastok")]
        public static void COM_SellPointShowStock(PlayerModel p, params string[] args)
        {
            SellPoint point = companySellPoints.Where(x => x.Position.Distance(p.Position) < 5 && x.Dimension == p.Dimension).FirstOrDefault();
            if (point == null) { MainChat.SendErrorChat(p, "[HATA] Herhangi bir satış noktasına yakın değilsiniz."); return; }

            MainChat.SendInfoChat(p, "[?] Satış noktasında " + point.stock + " adet stok bulunmakta.");
            return;
        }*/
    }
}
