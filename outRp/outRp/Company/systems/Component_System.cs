using System;
using System.Collections.Generic;
using System.Linq;
using AltV.Net; using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Enums;
using outRp.Models;
using System.Threading.Tasks;
using outRp.Globals;
using Newtonsoft.Json;
using outRp.Core;
using outRp.Chat;
using System.Numerics;
using AltV.Net.Resources.Chat.Api;
using outRp.OtherSystem.Textlabels;
using outRp.OtherSystem;

namespace outRp.Company.systems
{
    class Component_System : IScript
    {
        public static List<Models.Components> serverComponents = new List<Models.Components>();

        public static void LoadAllComponents()
        {
            serverComponents = Database.BusinessDatabase.GetAllComponents();

            foreach (Models.Components comp in serverComponents)
            {
                comp.ObjectID = PropStreamer.Create(GetComponentObjectModel(comp.Type), comp.ObjectPos, comp.ObjectRot, frozen: true).Id;
                comp.TextLabelID = TextLabelStreamer.Create(GetComponentDisplayName(comp.Type, comp.Stock_1, comp.Stock_2, comp.Stock_3, comp.SecurityLevel, comp.ID), comp.ObjectPos, streamRange: 5, font: 0).Id;
            }
        }

        public static void DescreaseCompSecurity()
        {
            serverComponents.ForEach(async(x) =>
            {
                if(x.SecurityLevel - 1 > 0)
                {
                    x.SecurityLevel -= 1;
                    await x.Update();
                }
            });

        }

        public static List<Models.Components> GetAllComponentsFromCompany(int CompanyID)
        {
            List<Models.Components> comps = new List<Models.Components>();

            serverComponents.ForEach(x =>
            {
                if (x.OwnerBusiness == CompanyID)
                    comps.Add(x);
            });

            return comps;
        }
        
        [Command("cbox")]
        public async Task CreateComponent(PlayerModel p, params string[] args)
        {
            if(args.Length < 0) { MainChat.SendInfoChat(p, "[用法] /cbox [货物点类型]"); return; }
            if(!Int32.TryParse(args[0], out int componentType)) { MainChat.SendInfoChat(p, "[用法] /cbox [货物点类型]"); return; }

            int CompanyID = await BusinesMain.GetPlayerCompany(p);
            if(CompanyID == -1) { MainChat.SendErrorChat(p, "[错误] 您的产业不隶属任何公司."); return; }
            Models.CompanyModel company = await Database.BusinessDatabase.GetCompany(CompanyID);
            if(company == null) { MainChat.SendErrorChat(p, "[错误] 您的产业不隶属任何公司."); return; }
            if(company.Owner != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 无权使用此指令."); return; } 

            if(Database.BusinessDatabase.CheckComponentWithType(company.ID, componentType))
            {
                MainChat.SendErrorChat(p, "[错误] 此产业已选择此类型的货物点类型了.");
                return;
            }
            p.SetData("Company:ID", company.ID);
            p.SetData("Company:Type", componentType);

            string model = "prop_container_01b";
            switch (componentType)
            {
                case 0:
                    model = "prop_container_01b";
                    break;
                case 1:
                    model = "prop_woodpile_01c";
                    break;
                case 2:
                    model = "prop_container_01c";
                    break;
                case 3:
                    model = "prop_container_01mb";
                    break;
                case 4:
                    model = "prop_side_spreader";
                    break;
                case 5:
                    model = "prop_container_01d";
                    break;
                case 6:
                    model = "prop_aircon_m_01";
                    break;
                case 7:
                    model = "sum_ac_prop_container_01a";
                    break;
            }

            GlobalEvents.ShowObjectPlacement(p, model, "Company:AddComponent");
        }

        [AsyncClientEvent("Company:AddComponent")]
        public static async Task AddComponentForCompany(PlayerModel p, string rot, string pos, string model)
        {
            if (p.Ping > 250)
                return;
            if (!p.HasData("Company:ID") || !p.HasData("Company:Type"))
                return;

            Vector3 position = JsonConvert.DeserializeObject<Vector3>(pos);
            position.Z -= 0.2f;
            Vector3 rotation = JsonConvert.DeserializeObject<Vector3>(rot);

            Models.Components comp = new Models.Components()
            {
                ObjectPos = position,
                ObjectRot = rotation,
                Type = p.lscGetdata<int>("Company:Type"),
                OwnerBusiness = p.lscGetdata<int>("Company:ID"),
                SecurityLevel = 0,
                Stock_1 = 0,
                Stock_2 = 0,
                Stock_3 = 0,
            };

            comp.ID = await Database.BusinessDatabase.CreateComponent(comp);
            comp.ObjectID = PropStreamer.Create(model, position, rotation, dimension: p.Dimension, frozen: true).Id;
            comp.TextLabelID = TextLabelStreamer.Create(GetComponentDisplayName(p.lscGetdata<int>("Company:Type")), position, p.Dimension, streamRange: 10,font: 0).Id;
            serverComponents.Add(comp);
            p.DeleteData("Company:Type"); p.DeleteData("Company:ID");
            return;
        }

        public static string GetComponentTypeName(int Type)
        {
            switch (Type)
            {
                case 0:
                    return "泥土";
                case 1:
                    return "木板";
                case 2:
                    return "混凝土";
                case 3:
                    return "塑料";
                case 4:
                    return "工业盐酸";
                case 5:
                    return "钢材";
                case 6:
                    return "铁";
                case 7:
                    return "电子材料";

                default:
                    return "杂货";
            }
        }

        public static string GetComponentObjectModel(int type)
        {
            string model = "prop_container_01b";
            switch (type)
            {
                case 0:
                    model = "prop_container_01b";
                    break;
                case 1:
                    model = "prop_woodpile_01c";
                    break;
                case 2:
                    model = "prop_container_01c";
                    break;
                case 3:
                    model = "prop_container_01mb";
                    break;
                case 4:
                    model = "prop_side_spreader";
                    break;
                case 5:
                    model = "prop_container_01d";
                    break;
                case 6:
                    model = "prop_aircon_m_01";
                    break;
                case 7:
                    model = "sum_ac_prop_container_01a";
                    break;
            }
            return model;
        }
        public static string GetComponentDisplayName(int Type, int stock_1 = 0, int stock_2 = 0, int stock_3 = 0, int securityLevel = 0, int CompID = 0)
        {
            string typeString = GetComponentTypeName(Type);

            return "~b~[" + CompID + "]~n~~w~类型: " + typeString + "~n~库存: " + stock_1 + "~n~采购价格: ~g~$" + stock_2 + "~w~~n~需求数量: ~r~" + stock_3 + "~n~~w~防盗等级: ~g~" + securityLevel;
        }
        public static string GetComponentDisplayName(Models.Components comp)
        {
            string typeString = GetComponentTypeName(comp.Type);

            return "~b~[-]~n~~w~类型: " + typeString + "~n~库存: " + comp.Stock_1 + "~n~采购价格: ~g~$" + comp.Stock_2 + "~w~~n~需求数量: ~r~" + comp.Stock_3 + "~n~~w~防盗等级: ~g~" + comp.SecurityLevel;
        }

        [Command("bwarehouse")]
        public static async Task COM_EditComponent(PlayerModel p, params string[] args)
        {
            var comp = GetNearComponent(p.Position);

            if(comp == null) { MainChat.SendErrorChat(p, "[错误] 附近没有货物点."); return; }
            var company = await Database.BusinessDatabase.GetCompany(comp.OwnerBusiness);
            if(company.Owner != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 您无权操作此仓库!"); return; }

            if(args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /bwarehouse [选项] [数值(如有)]"); return; }

            switch (args[0])
            {
                case "buy":
                    if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /bwarehouse [选项] [数值(如有)]"); return; }
                    if (!Int32.TryParse(args[1], out int newBuyPrice)) { MainChat.SendInfoChat(p, "[用法] /bwarehouse buy [数值]"); return; }
                    if(newBuyPrice < 0) { MainChat.SendErrorChat(p, "[错误] 无效数值."); return; }
                    comp.Stock_2 = newBuyPrice;
                    await comp.Update();
                    MainChat.SendInfoChat(p, "[?] 更新仓库产品采购价格为: $" + newBuyPrice);
                    return;

                case "need":
                    if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /bwarehouse [选项] [数值(如有)]"); return; }
                    if (!Int32.TryParse(args[1], out int newNeedStock)) { MainChat.SendInfoChat(p, "[用法] /bwarehouse need [数值]"); return; }
                    if(newNeedStock < 0) { MainChat.SendErrorChat(p, "[错误] 无效数值!"); return; }
                    comp.Stock_3 = newNeedStock;
                    await comp.Update();
                    MainChat.SendInfoChat(p, "[?] 更新仓库产品需求数值为: " + newNeedStock);
                    return;

                case "pos":
                    MainChat.SendInfoChat(p, "[?] 货物点对象将在 5 秒内刷新, 请不要将角色移动至更新的位置处, 以免出现BUG.");
                    Position pos = p.Position;
                    await Task.Delay(5000);

                    comp.ObjectPos = pos;
                    await comp.Update();
                    LProp propPos = PropStreamer.GetProp(comp.ObjectID);
                    if(propPos != null)
                        propPos.Position = pos;
                    return;

                case "rot":
                    if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /bwarehouse [选项] [数值(如有)]"); return; }
                    if (!Int32.TryParse(args[1], out int newRotation)) { MainChat.SendInfoChat(p, "[用法] /bwarehouse rot [数值]"); return; }

                    comp.ObjectRot = new Rotation(comp.ObjectRot.Roll, comp.ObjectRot.Pitch, newRotation);
                    LProp propRotation = PropStreamer.GetProp(comp.ObjectID);

                    if (propRotation != null)
                        propRotation.Rotation = comp.ObjectRot;
                    MainChat.SendInfoChat(p, "[!] 更新货物点对象旋转角度.");
                    return;
            }
        }

        [Command("loadstock")]
        public static async Task COM_StockMove(PlayerModel p, params string[] args)
        {
            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if(v == null) { MainChat.SendErrorChat(p, "[错误] 您附近没有车辆!"); return; }
            if(v.Model != (uint)VehicleModel.UtilliTruck2) { MainChat.SendErrorChat(p, "[错误] 此车辆模型不适合载重, 所需车辆模型: UtilliTruck2"); return; }

            var comp = GetNearComponent(p.Position);
            if(comp == null) { MainChat.SendErrorChat(p, "[错误] 附近没有货物点!"); return; }

            if(args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /getstock [数值]"); return; }

            var company = await Database.BusinessDatabase.GetCompany(comp.OwnerBusiness);
            if (company == null) { MainChat.SendErrorChat(p, "[错误] 获取仓库所属公司时出错/您的产业不隶属任何公司!"); return; }
            if(company.Owner != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 您无权操作此仓库!"); return; }

            if (!Int32.TryParse(args[0], out int amount)) { MainChat.SendErrorChat(p, "[用法] /getstock [数值]"); return; }

            if(comp.Stock_1 < amount) { MainChat.SendErrorChat(p, "[错误] 货物点库存不足!"); return; }

            if(GetVehicleTotalMoveStock(v) + amount > v.inventoryCapacity) { MainChat.SendErrorChat(p, "[错误] 此车辆的运输能力不足了!"); return; }
            AddCarMoveStock(v, comp.Type, amount);
            comp.Stock_1 -= amount;
            await comp.Update();

            OtherSystem.Animations.PlayerStopAnimation(p);

            MainChat.SendErrorChat(p, "[!] 成功从货物点将 " + amount + " 数量的 " + GetComponentTypeName(comp.Type) + " 装载至编号[ " + v.sqlID + " ]车辆!");
            return;
        }

        [Command("stealstock")]
        public async Task COM_TakeComponentStock(PlayerModel p, params string[] args)
        {
            if (p.HasData("StealStock:Type")) { MainChat.SendErrorChat(p, "[错误] 您手上已经有货物了, 请先将它装进车内. (/putstock)"); return; }
            var steal = Component_System.serverComponents.Where(x => x.ObjectPos.Distance(p.Position) < 5).FirstOrDefault();
            if (steal == null) { MainChat.SendErrorChat(p, "[错误] 附近没有货物点!"); return; }

            if(await BusinesMain.GetPlayerCompany(p) != steal.OwnerBusiness) { MainChat.SendErrorChat(p, "[错误] 您无权操作此仓库!"); return; }

            if (steal.Stock_1 < 100) { MainChat.SendErrorChat(p, "[错误] 货物点库存不足100!"); return; }

            p.SetData("StealStock:Type", steal.Type);
            steal.Stock_1 -= 100;
            PlayerLabel lbl = TextLabelStreamer.GetDynamicTextLabel(steal.TextLabelID);
            lbl.Text = Component_System.GetComponentDisplayName(steal);
            lbl.Font = 0;
            await steal.Update();

            Animations.PlayerAnimation(p, "carrybox3");

            MainChat.SendInfoChat(p, "[!] 成功从仓库偷取了 100 数量的货物.");
            return;
        }

        [Command("usestolen")]
        public static void COM_CreateStock(PlayerModel p)
        {
            var sellPoint = sellingPoints.GetNearSellPoint(p.Position, p.Dimension);
            if(sellPoint == null) { MainChat.SendErrorChat(p, "[错误] 附近没有赃物货物出售点!"); return; }
            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if(v == null) { MainChat.SendErrorChat(p, "[错误] 附近没有装载赃物货物的车辆, 如果您在车上请下车!"); return; }

            switch (sellPoint.Type)
            {
                case 1:
                    if (GetVehicleMoveStock(v, 6) < 1 || GetVehicleMoveStock(v, 5) < 2 || GetVehicleMoveStock(v, 3) < 4) { MainChat.SendErrorChat(p, "[错误] 此车辆中没有足够的赃物货物来制作服装货物库存(需要 4 铁 | 4 钢材 | 1 塑料)!"); return; }
                    RemoveVehicleMoveStock(v, 6, 1);
                    RemoveVehicleMoveStock(v, 3, 4);
                    RemoveVehicleMoveStock(v, 5, 2);
                    sellPoint.stock += 200;
                    sellPoint.Update();
                    MainChat.SendErrorChat(p, "[?] 通过倒卖 4 铁 | 4 钢材 | 1 塑料; 您获得了 240 车辆货物库存.");
                    return;

                case 6:
                    if (GetVehicleMoveStock(v, 3) < 2 || GetVehicleMoveStock(v, 4) < 1) { MainChat.SendErrorChat(p, "[错误] 此车辆中没有足够的赃物货物来制作服装货物库存(需要 2 塑料 | 1 工业盐酸)!"); return; }
                    RemoveVehicleMoveStock(v, 3, 2);
                    RemoveVehicleMoveStock(v, 4, 1);

                    sellPoint.stock += 10;
                    MainChat.SendErrorChat(p, "[?] 通过倒卖 2 塑料 | 1 工业盐酸; 您获得了 10 服装货物库存! ");
                    return;

                case 7:
                    if(GetVehicleMoveStock(v, 0) < 1 || GetVehicleMoveStock(v, 1) < 1 || GetVehicleMoveStock(v, 2) < 1 ||
                        GetVehicleMoveStock(v, 3) < 1 || GetVehicleMoveStock(v, 4) < 1 || GetVehicleMoveStock(v, 5) < 1 ||
                        GetVehicleMoveStock(v, 6) < 1 || GetVehicleMoveStock(v, 7) < 1)
                    { MainChat.SendErrorChat(p, "[错误] 此车辆中没有足够的赃物货物来制作服装货物库存(需要 1 每种材料)!"); return; }

                    RemoveVehicleMoveStock(v, 0, 1);
                    RemoveVehicleMoveStock(v, 1, 1);
                    RemoveVehicleMoveStock(v, 2, 1);
                    RemoveVehicleMoveStock(v, 3, 1);
                    RemoveVehicleMoveStock(v, 4, 1);
                    RemoveVehicleMoveStock(v, 5, 1);
                    RemoveVehicleMoveStock(v, 6, 1);
                    RemoveVehicleMoveStock(v, 7, 1);
                    RemoveVehicleMoveStock(v, 8, 1);

                    sellPoint.stock += 12;

                    MainChat.SendErrorChat(p, "[?] 通过倒卖 1 每种材料; 您获得了 12 通用货物库存.");
                    return;
            }
        }

        public static void AddCarMoveStock(VehModel v, int stockType, int amount)
        {
            if (v == null)
                return;

            switch (stockType)
            {
                case 0:
                    if (!v.HasData("MoveStock:Toprak"))
                        v.SetData("MoveStock:Toprak", amount);
                    else v.SetData("MoveStock:Toprak", v.lscGetdata<int>("MoveStock:Toprak") + amount);
                    break;

                case 1:
                    if (!v.HasData("MoveStock:Odun"))
                        v.SetData("MoveStock:Odun", amount);
                    else v.SetData("MoveStock:Odun", v.lscGetdata<int>("MoveStock:Odun") + amount);
                    break;

                case 2:
                    if (!v.HasData("MoveStock:Beton"))
                        v.SetData("MoveStock:Beton", amount);
                    else v.SetData("MoveStock:Beton", v.lscGetdata<int>("MoveStock:Beton") + amount);
                    break;

                case 3:
                    if (!v.HasData("MoveStock:Plastik"))
                        v.SetData("MoveStock:Plastik", amount);
                    else v.SetData("MoveStock:Plastik", v.lscGetdata<int>("MoveStock:Plastik") + amount);
                    break;

                case 4:
                    if (!v.HasData("MoveStock:Asit"))
                        v.SetData("MoveStock:Asit", amount);
                    else v.SetData("MoveStock:Asit", v.lscGetdata<int>("MoveStock:Asit") + amount);
                    break;

                case 5:
                    if (!v.HasData("MoveStock:Celik"))
                        v.SetData("MoveStock:Celik", amount);
                    else v.SetData("MoveStock:Celik", v.lscGetdata<int>("MoveStock:Celik") + amount);
                    break;

                case 6:
                    if (!v.HasData("MoveStock:Demir"))
                        v.SetData("MoveStock:Demir", amount);
                    else v.SetData("MoveStock:Demir", v.lscGetdata<int>("MoveStock:Demir") + amount);
                    break;

                case 7:
                    if (!v.HasData("MoveStock:Dijital"))
                        v.SetData("MoveStock:Dijital", amount);
                    else v.SetData("MoveStock:Dijital", v.lscGetdata<int>("MoveStock:Dijital") + amount);
                    break;
            }

            string lastText = "~b~[装载]~n~~w~";
            if (v.HasData("MoveStock:Toprak"))
                lastText += "~n~泥土: " + v.lscGetdata<int>("MoveStock:Toprak");

            if (v.HasData("MoveStock:Odun"))
                lastText += "~n~木板: " + v.lscGetdata<int>("MoveStock:Odun");

            if (v.HasData("MoveStock:Beton"))
                lastText += "~n~混凝土: " + v.lscGetdata<int>("MoveStock:Beton");

            if (v.HasData("MoveStock:Plastik"))
                lastText += "~n~塑料: " + v.lscGetdata<int>("MoveStock:Plastik");

            if (v.HasData("MoveStock:Asit"))
                lastText += "~n~工业盐酸: " + v.lscGetdata<int>("MoveStock:Asit");

            if (v.HasData("MoveStock:Celik"))
                lastText += "~n~钢材: " + v.lscGetdata<int>("MoveStock:Celik");

            if (v.HasData("MoveStock:Demir"))
                lastText += "~n~铁: " + v.lscGetdata<int>("MoveStock:Demir");

            if (v.HasData("MoveStock:Dijital"))
                lastText += "~n~电子材料: " + v.lscGetdata<int>("MoveStock:Dijital");

            GlobalEvents.SetVehicleTag(v, lastText);
            return;
        }

        public static void RemoveVehicleMoveStock(VehModel v, int stockType, int amount)
        {
            if (v == null)
                return;
            switch (stockType)
            {
                case 0:
                    if (!v.HasData("MoveStock:Toprak"))
                        return;
                    else v.SetData("MoveStock:Toprak", v.lscGetdata<int>("MoveStock:Toprak") - amount);
                    if (v.lscGetdata<int>("MoveStock:Toprak") <= 0)
                        v.DeleteData("MoveStock:Toprak");
                    break;

                case 1:
                    if (!v.HasData("MoveStock:Odun"))
                        return;
                    else v.SetData("MoveStock:Odun", v.lscGetdata<int>("MoveStock:Odun") - amount);
                    if (v.lscGetdata<int>("MoveStock:Odun") <= 0)
                        v.DeleteData("MoveStock:Odun");
                    break;

                case 2:
                    if (!v.HasData("MoveStock:Beton"))
                        return;
                    else v.SetData("MoveStock:Beton", v.lscGetdata<int>("MoveStock:Beton") - amount);
                    if (v.lscGetdata<int>("MoveStock:Beton") <= 0)
                        v.DeleteData("MoveStock:Beton");
                    break;

                case 3:
                    if (!v.HasData("MoveStock:Plastik"))
                        return;
                    else v.SetData("MoveStock:Plastik", v.lscGetdata<int>("MoveStock:Plastik") - amount);
                    if (v.lscGetdata<int>("MoveStock:Plastik") <= 0)
                        v.DeleteData("MoveStock:Plastik");
                    break;

                case 4:
                    if (!v.HasData("MoveStock:Asit"))
                        return;
                    else v.SetData("MoveStock:Asit", v.lscGetdata<int>("MoveStock:Asit") - amount);
                    if (v.lscGetdata<int>("MoveStock:Asit") <= 0)
                        v.DeleteData("MoveStock:Asit");
                    break;

                case 5:
                    if (!v.HasData("MoveStock:Celik"))
                        return;
                    else v.SetData("MoveStock:Celik", v.lscGetdata<int>("MoveStock:Celik") - amount);
                    if (v.lscGetdata<int>("MoveStock:Celik") <= 0)
                        v.DeleteData("MoveStock:Celik");
                    break;

                case 6:
                    if (!v.HasData("MoveStock:Demir"))
                        return;
                    else v.SetData("MoveStock:Demir", v.lscGetdata<int>("MoveStock:Demir") - amount);
                    if (v.lscGetdata<int>("MoveStock:Demir") <= 0)
                        v.DeleteData("MoveStock:Demir");
                    break;

                case 7:
                    if (!v.HasData("MoveStock:Dijital"))
                        return;
                    else v.SetData("MoveStock:Dijital", v.lscGetdata<int>("MoveStock:Dijital") - amount);
                    if (v.lscGetdata<int>("MoveStock:Dijital") <= 0)
                        v.DeleteData("MoveStock:Dijital");
                    break;
            }

            string lastText = "~b~[装载]~n~";
            if (v.HasData("MoveStock:Toprak"))
                lastText += "~n~泥土: " + v.lscGetdata<int>("MoveStock:Toprak");

            if (v.HasData("MoveStock:Odun"))
                lastText += "~n~木板: " + v.lscGetdata<int>("MoveStock:Odun");

            if (v.HasData("MoveStock:Beton"))
                lastText += "~n~混凝土: " + v.lscGetdata<int>("MoveStock:Beton");

            if (v.HasData("MoveStock:Plastik"))
                lastText += "~n~塑料: " + v.lscGetdata<int>("MoveStock:Plastik");

            if (v.HasData("MoveStock:Asit"))
                lastText += "~n~工业盐酸: " + v.lscGetdata<int>("MoveStock:Asit");

            if (v.HasData("MoveStock:Celik"))
                lastText += "~n~钢材: " + v.lscGetdata<int>("MoveStock:Celik");

            if (v.HasData("MoveStock:Demir"))
                lastText += "~n~铁: " + v.lscGetdata<int>("MoveStock:Demir");

            if (v.HasData("MoveStock:Dijital"))
                lastText += "~n~电子材料: " + v.lscGetdata<int>("MoveStock:Dijital");

            GlobalEvents.SetVehicleTag(v, lastText);
            return;

        }

        public static int GetVehicleTotalMoveStock(VehModel v)
        {
            if (v == null)
                return 0;
            int total = 0;
            if (v.HasData("MoveStock:Toprak"))
                total += v.lscGetdata<int>("MoveStock:Toprak");

            if (v.HasData("MoveStock:Odun"))
                total += v.lscGetdata<int>("MoveStock:Odun");

            if (v.HasData("MoveStock:Beton"))
                total += v.lscGetdata<int>("MoveStock:Beton");

            if (v.HasData("MoveStock:Plastik"))
                total += v.lscGetdata<int>("MoveStock:Plastik");

            if (v.HasData("MoveStock:Asit"))
                total += v.lscGetdata<int>("MoveStock:Asit");

            if (v.HasData("MoveStock:Celik"))
                total +=  v.lscGetdata<int>("MoveStock:Celik");

            if (v.HasData("MoveStock:Demir"))
                total +=  v.lscGetdata<int>("MoveStock:Demir");

            if (v.HasData("MoveStock:Dijital"))
                total += v.lscGetdata<int>("MoveStock:Dijital");

            return total;
        }

        public static int GetVehicleMoveStock(VehModel v, int stockType)
        {
            if (v == null)
                return 0;

            switch (stockType)
            {
                case 0:
                    if (v.HasData("MoveStock:Toprak"))
                        return v.lscGetdata<int>("MoveStock:Toprak");
                    else return 0;
                case 1:
                    if (v.HasData("MoveStock:Odun"))
                        return v.lscGetdata<int>("MoveStock:Odun");
                    else return 0;
                case 2:
                    if (v.HasData("MoveStock:Beton"))
                        return v.lscGetdata<int>("MoveStock:Beton");
                    else return 0;
                case 3:
                    if (v.HasData("MoveStock:Plastik"))
                        return v.lscGetdata<int>("MoveStock:Plastik");
                    else return 0;
                case 4:
                    if (v.HasData("MoveStock:Asit"))
                        return v.lscGetdata<int>("MoveStock:Asit");
                    else return 0;
                case 5:
                    if (v.HasData("MoveStock:Celik"))
                        return v.lscGetdata<int>("MoveStock:Celik");
                    else return 0;
                case 6:
                    if (v.HasData("MoveStock:Demir"))
                        return v.lscGetdata<int>("MoveStock:Demir");
                    else return 0;
                case 7:
                    if (v.HasData("MoveStock:Dijital"))
                        return v.lscGetdata<int>("MoveStock:Dijital");
                    else return 0;
                default:
                    return 0;
            }

        }

        public static Models.Components GetNearComponent(Position pos)
        {
            return serverComponents.Where(x => pos.Distance(x.ObjectPos) < 10).OrderBy(x => pos.Distance(x.ObjectPos)).FirstOrDefault();            
        }

        public static Models.Components GetComponentWithID(int ID)
        {
            return serverComponents.Where(x => x.ID == ID).FirstOrDefault();
        }
        [Command("balarm")]
        public static async Task COM_UpdateComponentSecurity(PlayerModel p)
        {
            
            var comp = GetNearComponent(p.Position);
            if(comp == null) { MainChat.SendErrorChat(p, "[错误] 附近没有货物点!"); return; }
            if(comp.SecurityLevel > 3) { MainChat.SendErrorChat(p, "[错误] 目前无法修复警报 (至少需要 防盗等级 小于 3 时才可以修复警报)"); return; }

            var company = await Database.BusinessDatabase.GetCompany(comp.OwnerBusiness);
            if (company == null) { MainChat.SendErrorChat(p, "[错误] 获取仓库所属公司时出错/您的产业不隶属任何公司."); return; }

            if(company.Owner != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 您无权操作此仓库!"); return; }

            if (company.Cash < company.storedCash) { MainChat.SendErrorChat(p, "[错误] 您的公司没有足够的钱修复警报!"); return; }
            if (company.Cash < (6 - comp.SecurityLevel) * 2000) { MainChat.SendErrorChat(p, "[错误] 您的公司没有足够的钱修复警报!"); return; }

            company.Cash -= (6 - comp.SecurityLevel) * 2000;
            comp.SecurityLevel += (6 - comp.SecurityLevel);
            await company.Update();
            await comp.Update();

            MainChat.SendInfoChat(p, "[!] 您修复了公司的警报.");
            return;
        }

    }
}
