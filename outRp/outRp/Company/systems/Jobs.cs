using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Enums;
using outRp.Chat;
using outRp.Core;
using outRp.Globals;
using outRp.Models;
using outRp.OtherSystem;
using outRp.OtherSystem.Textlabels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;

namespace outRp.Company.systems
{
    class Jobs : IScript
    {
        // Positions 
        public static Position WoodCutPos = new Position(-1556.2814f, 4598.558f, 20.349f);
        // /posgit -1556 4598 20
        public static Position WoodToPlankPos = new Position(-512, 5270, 81);
        // --- TOPRAK
        public static Position EarthCarPos = new Position(2749.5166f, 2792.3867f, 35.8007f);
        public static Position EarchButtonPos = new Position(2758.2593f, 2804.532f, 41.61389f);
        // --- concrete (beton)
        public static Position ConcreteSetupPos = new Position(3505, 3677, 34);
        public static Position ConcreteMakePos = new Position(3532, 3672, 33);
        // --- Trash(çöp)
        public static Position trashSellPos = new Position(1072.8f, -1963.3319f, 30.998535f);
        public static Position TrashToPlasticPos = new Position(-48.26373f, -2504.1362f, 5.993f);
        public static ConcreteMake Concrete = new ConcreteMake();
        private static ulong[] _TrashCount = new ulong[] { 100, 0 };
        // --- Acit (Asit)
        public static Position acitStealPos = new Position(2792.0967f, 1569.9692f, 24.494507f);
        private static ulong[] _acitCount = new ulong[] { 2000, 0 };
        // --- Steal (Çelik)
        public static Position SteelBuyPos = new Position(193.6088f, 2757.5474f, 43.41687f);
        public static Position SteelTrailerSpawnPos = new Position(173.8945f, 2753, 43.4168f);
        // --- Iron (Demir)
        public static Position IronBuyPos = new Position(-118.81319f, -2220.0527f, 7.7963867f);

        public static ulong AcitCount
        {
            get { return _acitCount[0]; }
            set
            {
                if (_acitCount[0] != value)
                    _acitCount[0] = value;

                if (_acitCount[0] > 3000)
                    _acitCount[0] = 3000;

                PlayerLabel pr = TextLabelStreamer.GetDynamicTextLabel(_acitCount[1]);
                if (pr != null) { pr.Text = AcitText(_acitCount[0]); pr.Font = 0; }
            }
        }
        public static ulong TrashCount
        {
            get { return _TrashCount[0]; }
            set
            {
                if (_TrashCount[0] == value)

                    _TrashCount[0] = value;
                if (_TrashCount[0] > 3000)
                    _TrashCount[0] = 3000;
                PlayerLabel lbl = TextLabelStreamer.GetDynamicTextLabel(_TrashCount[1]);
                if (lbl == null)
                    return;

                lbl.Text = TrashText((int)value);
                lbl.Font = 0;
            }
        }
        public class ConcreteMake
        {
            public int currentMaker { get; set; } = 0;
            public int currentState { get; set; } = 0;
            public DateTime lastUsed { get; set; } = DateTime.Now;
            public ulong textlabelID { get; set; }
        }
        public static string TrashText(int trashCount)
        {
            return "~b~[垃圾]~n~~w~重量: ~g~" + trashCount + "~w~千克";
        }
        public static string AcitText(ulong acitCount)
        {
            return "~b~[Asit Varili]~n~~w~容量: ~g~" + acitCount + "~w~升";
        }
        public static async Task<string> ConcreteText()
        {
            string returnText = "~g~[泥土研磨机]";
            string user = "无";
            if (Concrete.currentMaker != 0)
            {
                var _user = await outRp.Database.DatabaseMain.getCharacterInfo(Concrete.currentMaker);
                user = _user.characterName.Replace("_", " ");

            }
            if (Concrete.currentMaker != 0)
            {
                returnText = "~g~[泥土研磨机]~n~~w~使用者: ~o~" + user + "~n~~w~进度: ~g~%" + Concrete.currentState;
            }
            if (Concrete.currentState >= 100) { returnText += "~n~~w~/betonyukle"; }
            return returnText;
        }
        // İçinde trash vs. var
        public static async Task UpdateConcrete()
        {
            if (Concrete.lastUsed < DateTime.Now)
            {
                Concrete.currentMaker = 0;
                Concrete.currentState = 0;
                Concrete.lastUsed = DateTime.Now;

                TextLabelStreamer.GetDynamicTextLabel(Concrete.textlabelID).Text = await ConcreteText();
                TextLabelStreamer.GetDynamicTextLabel(Concrete.textlabelID).Font = 0;
            }
            else
            {
                Concrete.currentState += 20;
                if (Concrete.currentState > 100)
                    Concrete.currentState = 100;

                TextLabelStreamer.GetDynamicTextLabel(Concrete.textlabelID).Text = await ConcreteText();
                TextLabelStreamer.GetDynamicTextLabel(Concrete.textlabelID).Font = 0;
            }

            // Çöp
            TrashCount += 10;
            AcitCount += 1;
        }

        public static async Task LoadJobSystems()
        {
            Concrete.textlabelID = TextLabelStreamer.Create(await ConcreteText(), ConcreteMakePos, dimension: 0, font: 0, streamRange: 10).Id;
            _TrashCount[1] = TextLabelStreamer.Create("[垃圾处理厂]", trashSellPos, dimension: 0, font: 0, streamRange: 15).Id;
            _acitCount[1] = TextLabelStreamer.Create("[工业盐酸桶]", acitStealPos, dimension: 0, font: 0, streamRange: 15).Id;
            TextLabelStreamer.Create("~o~[混凝土加工点]~n~~w~/~g~makeconcrete", ConcreteSetupPos, streamRange: 5, font: 0);
            TextLabelStreamer.Create("~o~[泥土装载点]~n~~w~/~g~getsoil", EarchButtonPos, streamRange: 5, font: 0);
            TextLabelStreamer.Create("~o~[泥土装载点]~n~~w~车辆操作", EarthCarPos, streamRange: 5, font: 0);
            TextLabelStreamer.Create("~o~[炼钢厂]~n~~w~价格: ~g~ $500", SteelBuyPos, streamRange: 5, font: 0);
            TextLabelStreamer.Create("~o~[塑料工厂]~n~~w~/plastikuret", TrashToPlasticPos, streamRange: 5, font: 0);
            TextLabelStreamer.Create("~o~[铁工厂]~n~~w~/demirsatinal", IronBuyPos, streamRange: 5, font: 0);
        }

        // Class 
        private static List<OtherSystem.Textlabels.LProp> Woods = new List<OtherSystem.Textlabels.LProp>();
        #region Component Oduncu
        [Command("cutwood")]
        public async void COM_WoodCut(PlayerModel p, params string[] args)
        {
            if (p.HasData("Wood:Busy")) { MainChat.SendErrorChat(p, "[错误] 您正在砍木头中!"); return; }
            if (p.Position.Distance(WoodCutPos) > 150) { MainChat.SendErrorChat(p, "[错误] 您不在伐木区."); return; }
            if (p.Vehicle != null) { MainChat.SendErrorChat(p, "[错误] 您需要是步行状态."); return; }

            if (p.HasData("Company:WoodCut"))
            {
                if (p.Position.Distance(p.lscGetdata<Position>("Company:WoodCut")) < 5) { MainChat.SendErrorChat(p, "[!] 您已经砍伐了该范围的树木, 请尝试其他位置."); return; }
            }

            GlobalEvents.FreezeEntity(p, true);
            GlobalEvents.ProgresBar(p, "正在砍木头中...", 10);
            p.SetData("Wood:Busy", 0);
            await Task.Delay(10000);

            if (!p.Exists)
                return;

            GlobalEvents.FreezeEntity(p);
            if (p.HasData("Wood:Busy"))
                p.DeleteData("Wood:Busy");

            Position propPos = p.Position;
            propPos.Z -= 1;
            OtherSystem.Textlabels.LProp wood = PropStreamer.Create("prop_tree_log_02", propPos, new Vector3(0, 0, 0), dimension: p.Dimension, frozen: true);
            Woods.Add(wood);
            MainChat.SendInfoChat(p, "[*] 成功砍倒木头 - 您现在可以使用拥有 平板 的车辆收集木头.");
            return;
        }

        [Command("getwood")]
        public static void COM_GetWood(PlayerModel p, params string[] args)
        {
            if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] 您不在车内."); return; }
            if (p.Vehicle.Model != (uint)VehicleModel.Flatbed) { MainChat.SendErrorChat(p, "[错误] 您需要一辆有 平板 的车辆来收集木头."); return; }

            VehModel veh = (VehModel)p.Vehicle;
            OtherSystem.Textlabels.LProp nearWood = Woods.Find(x => p.Position.Distance(x.Position) < 5);
            if (nearWood == null) { MainChat.SendErrorChat(p, "[错误] 附近没有砍倒的木头!"); return; }

            if (!veh.HasStreamSyncedMetaData("WoodStorage")) { veh.SetStreamSyncedMetaData("WoodStorage", 0); }

            veh.GetStreamSyncedMetaData("WoodStorage", out int WoodStorage);

            if (WoodStorage >= 10) { MainChat.SendErrorChat(p, "[错误] 此车最多容纳10个木头."); return; }
            WoodStorage++;
            Woods.Remove(nearWood);
            nearWood.Destroy();
            veh.SetStreamSyncedMetaData("WoodStorage", WoodStorage);
            MainChat.SendInfoChat(p, "[*] 成功将木头装载至车辆, 您现在可以去加工车上的木头, 使它们成为木板再出售给产业.");
            UpdateVehicleWoodInfo(veh, WoodStorage, "木头");
            return;
        }

        [Command("makeplank")]
        public static void COM_WoodToPlank(PlayerModel p)
        {
            VehModel veh = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (!veh.GetStreamSyncedMetaData("WoodStorage", out int Wood)) { MainChat.SendErrorChat(p, "[错误] 此车上平板没有装载的木头!"); return; }

            if (veh.Position.Distance(WoodToPlankPos) > 4) { MainChat.SendErrorChat(p, "[错误] 您不在木材加工点."); return; }
            veh.DeleteStreamSyncedMetaData("WoodStorage");
            veh.SetData("PlankStorage", Wood);
            GlobalEvents.SetVehicleTag(veh, "~b~[加工并装载]~n~~w~类型: ~o~木板");
            MainChat.SendInfoChat(p, "[*] 成功加工车上的木头, 您现在可以将 木板 出售给采购木板的产业.");
            return;

        }

        [Command("sellplank")]
        public async Task COM_SellPlank(PlayerModel p)
        {
            Models.Components comp = Component_System.GetNearComponent(p.Position);
            if (comp == null) { MainChat.SendErrorChat(p, "[错误] 附近没有仓库货物点."); return; }
            if (comp.Type != 1) { MainChat.SendErrorChat(p, "[错误] 此仓库货物点不采购木头."); return; }
            VehModel veh = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (veh.Driver != null || !await Vehicle.VehicleMain.GetKeysQuery(p, veh)) { MainChat.SendErrorChat(p, "[错误] 您无权操作此车辆!"); return; }
            if (!veh.GetData("PlankStorage", out int plankCount)) { MainChat.SendErrorChat(p, "[错误] 此车上的平板没有装载木板!"); return; }

            Models.CompanyModel componentOwner = await Database.BusinessDatabase.GetCompany(comp.OwnerBusiness);
            if (componentOwner == null)
                return;

            if (componentOwner.Cash < (plankCount * comp.Stock_2)) { MainChat.SendErrorChat(p, "[错误] 此仓库货物点的产业没有足够的钱购买您的木板!"); return; }
            if (comp.Stock_3 < plankCount) { MainChat.SendErrorChat(p, "[!] 此仓库货物点已经满了!"); return; }


            int total = (plankCount * comp.Stock_2);
            componentOwner.Cash -= total;
            int multipler = 0;
            if (await BusinesMain.GetPlayerCompany(p) == componentOwner.ID)
                multipler = comp.Stock_2 / 10;
            p.cash += total + multipler;
            veh.DeleteData("PlankStorage");
            GlobalEvents.ClearVehicleTag(veh);
            MainChat.SendInfoChat(p, "[$] 成功将车上的木板出售并获得了 $" + (total + multipler) + " , 此仓库货物点的产业: " + componentOwner.Name);

            componentOwner.BusinessPrice += total;
            await componentOwner.Update();

            comp.Stock_3 -= plankCount;
            comp.Stock_1 += plankCount;
            await comp.Update();

            TextLabelStreamer.GetDynamicTextLabel(comp.TextLabelID).Text = systems.Component_System.GetComponentDisplayName(comp.Type, comp.Stock_1, comp.Stock_2, comp.Stock_3, comp.SecurityLevel);
            return;
        }

        public static void UpdateVehicleWoodInfo(VehModel v, int totalWood, string Cesit)
        {
            string Text = "~g~[装载]~n~~w~类型: ~b~" + Cesit + "~n~~w~数量: " + totalWood + " / 10";
            GlobalEvents.SetVehicleTag(v, Text);
            return;
        }
        #endregion

        #region Component Toprak
        [Command("getsoil")]
        public static async Task COM_AddEarth(PlayerModel p)
        {
            if (p.Position.Distance(EarchButtonPos) > 1) { MainChat.SendErrorChat(p, "[错误] 您不在泥土装载点."); return; }
            VehModel veh = (VehModel)Alt.GetAllVehicles().Where(x => x.Position.Distance(EarthCarPos) < 10 && x.Dimension == 0).OrderBy(x => x.Position.Distance(EarthCarPos)).FirstOrDefault();
            if (veh == null) { MainChat.SendErrorChat(p, "[错误] 泥土装载点禁止车辆进入!"); return; }
            if (veh.Model != (uint)VehicleModel.Biff) { MainChat.SendErrorChat(p, "[错误] 此车无法装载泥土, 需要模型为 Biff 的车辆."); return; }
            if (!await Vehicle.VehicleMain.GetKeysQuery(p, veh)) { MainChat.SendErrorChat(p, "[错误] 您无权操作此车辆!"); return; }
            if (veh.HasData("EarthStorage")) { MainChat.SendErrorChat(p, "[错误] 此车已经装满了泥土."); return; }
            if (p.cash < 150) { MainChat.SendErrorChat(p, "[错误] 您没有足够的钱装载泥土!"); return; }
            p.cash -= 150;
            veh.SetData("EarthStorage", 50);
            GlobalEvents.SetVehicleTag(veh, "~g~[装载]~n~~w~类型: ~o~泥土~n~~w~数量: ~b~50千克");
            MainChat.SendInfoChat(p, "[?] 成功装载 50千克 的泥土, 价格: $150 (您可以选择出售 泥土 至需要的产业或者用来制作 混凝土 再将混凝土出售需要的产业)");
            return;
        }

        [Command("sellsoil")]
        public static async Task COM_SellEarth(PlayerModel p)
        {
            Models.Components comp = Component_System.GetNearComponent(p.Position);
            if (comp == null) { MainChat.SendErrorChat(p, "[错误] 附近没有仓库货物点."); return; }
            if (comp.Type != 0) { MainChat.SendErrorChat(p, "[错误] 此仓库货物点不采购泥土."); return; }
            VehModel veh = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (veh.Driver != null || !await Vehicle.VehicleMain.GetKeysQuery(p, veh)) { MainChat.SendErrorChat(p, "[错误] 您无权操作此车辆!"); return; }
            if (!veh.GetData("EarthStorage", out int plankCount)) { MainChat.SendErrorChat(p, "[错误] 此车没有装载泥土!"); return; }

            Models.CompanyModel componentOwner = await Database.BusinessDatabase.GetCompany(comp.OwnerBusiness);
            if (componentOwner == null)
                return;

            if (componentOwner.Cash < (plankCount * comp.Stock_2)) { MainChat.SendErrorChat(p, "[错误] 此仓库货物点的产业没有足够的钱购买您的泥土!"); return; }
            if (comp.Stock_3 < plankCount) { MainChat.SendErrorChat(p, "[!] 此仓库货物点已经满了!"); return; }


            int total = (plankCount * comp.Stock_2);
            componentOwner.Cash -= total;
            int multipler = 0;
            if (await BusinesMain.GetPlayerCompany(p) == componentOwner.ID)
                multipler = comp.Stock_2 / 10;
            p.cash += total + multipler;
            veh.DeleteData("EarthStorage");
            GlobalEvents.ClearVehicleTag(veh);
            MainChat.SendInfoChat(p, "[$] 成功将车上的泥土出售并获得了 $" + (total + multipler) + " , 此仓库货物点的产业: " + componentOwner.Name);

            componentOwner.BusinessPrice += total;
            await componentOwner.Update();

            comp.Stock_3 -= plankCount;
            comp.Stock_1 += plankCount;
            await comp.Update();
            TextLabelStreamer.GetDynamicTextLabel(comp.TextLabelID).Text = systems.Component_System.GetComponentDisplayName(comp.Type, comp.Stock_1, comp.Stock_2, comp.Stock_3, comp.SecurityLevel);
            return;
        }
        #endregion

        #region Component Beton
        [Command("makeconcrete")]
        public static async Task COM_DropEarth(PlayerModel p)
        {
            if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] 您必须在车内."); return; }
            VehModel veh = (VehModel)p.Vehicle;
            if (!veh.HasData("EarthStorage")) { MainChat.SendErrorChat(p, "[错误] 此车没有装载泥土!"); return; }
            if (p.Position.Distance(ConcreteSetupPos) > 4) { MainChat.SendErrorChat(p, "[错误] 您不在混凝土加工点."); return; }
            if (Concrete.currentMaker != 0) { MainChat.SendErrorChat(p, "[!] 混凝土搅拌设备正在被使用中, 请稍后再试."); return; }
            if (p.cash < 200) { MainChat.SendErrorChat(p, "[错误] 您没有足够的钱制作混凝土!"); return; }
            p.cash -= 200;
            await p.updateSql();

            veh.DeleteData("EarthStorage");
            GlobalEvents.ClearVehicleTag(veh);
            Concrete.lastUsed = DateTime.Now.AddMinutes(7);
            Concrete.currentMaker = p.sqlID;
            Concrete.currentState = 0;
            TextLabelStreamer.GetDynamicTextLabel(Concrete.textlabelID).Text = await ConcreteText();

            MainChat.SendInfoChat(p, "[?] 开始制作混凝土了, 请继续使用混凝土搅拌设备(此段可RP, 过程大概 7 分钟), 当处理完成后, 输入 /getconcrete 装载加工完成的混凝土.");
            return;
        }

        [Command("getconcrete")]
        public static async Task COM_GetConcrete(PlayerModel p)//
        {
            if (p.Position.Distance(ConcreteMakePos) > 5) { MainChat.SendErrorChat(p, "[错误] 您不在混凝土加工点."); return; }
            if (Concrete.currentMaker != p.sqlID) { MainChat.SendErrorChat(p, "[**] 混凝土工厂人员说: 这一批混凝土不属于您, 也就是说不是您加工的."); return; }
            if (Concrete.currentState < 100) { MainChat.SendErrorChat(p, "[**] 混凝土工厂人员说: 急什么啊, 还没有好呢!"); return; }
            VehModel veh = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (!await Vehicle.VehicleMain.GetKeysQuery(p, veh)) { MainChat.SendErrorChat(p, "[错误] 您无权操作此车辆!"); return; }
            if (veh.Model != (uint)VehicleModel.Mixer && veh.Model != (uint)VehicleModel.Mixer2) { MainChat.SendErrorChat(p, "[错误] 您只能在 mixer 和 mixer2 模型的车辆上装载混凝土."); return; }

            veh.SetData("ConcreteStorage", 20);
            Concrete.currentMaker = 0;
            Concrete.currentState = 0;
            TextLabelStreamer.GetDynamicTextLabel(Concrete.textlabelID).Text = await ConcreteText();
            GlobalEvents.SetVehicleTag(veh, "~g~[装载]~n~~w~类型: ~o~混凝土~n~~w~数量: ~b~20千克");
            MainChat.SendInfoChat(p, "[!] 成功将混凝土装载至车辆, 您可以出售给需要的产业.");
            return;
        }

        [Command("sellconcrete")]
        public static async Task COM_SellConcrete(PlayerModel p)
        {
            Models.Components comp = Component_System.GetNearComponent(p.Position);
            if (comp == null) { MainChat.SendErrorChat(p, "[错误] 附近没有仓库货物点."); return; }
            if (comp.Type != 2) { MainChat.SendErrorChat(p, "[错误] 此仓库货物点不采购混凝土."); return; }
            VehModel veh = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (veh.Driver != null || !await Vehicle.VehicleMain.GetKeysQuery(p, veh)) { MainChat.SendErrorChat(p, "[错误] 您无权操作此车辆!"); return; }
            if (!veh.GetData("ConcreteStorage", out int plankCount)) { MainChat.SendErrorChat(p, "[错误] 此车没有装载混凝土!"); return; }

            Models.CompanyModel componentOwner = await Database.BusinessDatabase.GetCompany(comp.OwnerBusiness);
            if (componentOwner == null)
                return;

            if (componentOwner.Cash < (plankCount * comp.Stock_2)) { MainChat.SendErrorChat(p, "[错误] 此仓库货物点的产业没有足够的钱购买您的混凝土!"); return; }
            if (comp.Stock_3 < plankCount) { MainChat.SendErrorChat(p, "[!] 此仓库货物点已经满了!"); return; }


            int total = (plankCount * comp.Stock_2);
            componentOwner.Cash -= total;
            int multipler = 0;
            if (await BusinesMain.GetPlayerCompany(p) == componentOwner.ID)
                multipler = comp.Stock_2 / 10;
            p.cash += total + multipler;
            veh.DeleteData("ConcreteStorage");
            GlobalEvents.ClearVehicleTag(veh);
            MainChat.SendInfoChat(p, "[$] 成功将车上的混凝土出售并获得了 $" + (total + multipler) + " , 此仓库货物点的产业: " + componentOwner.Name);

            componentOwner.BusinessPrice += total;
            await componentOwner.Update();

            comp.Stock_3 -= plankCount;
            comp.Stock_1 += plankCount;
            await comp.Update();
            TextLabelStreamer.GetDynamicTextLabel(comp.TextLabelID).Text = systems.Component_System.GetComponentDisplayName(comp.Type, comp.Stock_1, comp.Stock_2, comp.Stock_3, comp.SecurityLevel);
            return;
        }
        #endregion

        #region Component Asit
        [Command("stealacid")]
        public static void COM_StealAcit(PlayerModel p)
        {
            if (p.Position.Distance(acitStealPos) > 5) { MainChat.SendErrorChat(p, "[错误] 您不在工业盐酸桶附近."); return; }
            if (p.HasData("Acit:Steal")) { MainChat.SendErrorChat(p, "[错误] 您手上已经有偷取的工业盐酸了, 请先输入 /putacid 将它装载至车辆."); return; }

            if (AcitCount <= 10) { MainChat.SendErrorChat(p, "[!] 此工业盐酸桶内没有足够的工业盐酸!"); return; }
            if (TotalPD() < 5) { MainChat.SendErrorChat(p, "[!] 至少需要有 5 名 执法人员在线才可以偷取工业盐酸"); return; }            // Total'i 5 e çek.

            p.SetData("Acit:Steal", 0);
            Animations.PlayerAnimation(p, "acitbox");
            MainChat.SendInfoChat(p, "[*] 成功偷取工业盐酸整桶! (( 您已经被匿名举报, 也就是说警察已经知道您的非法行动了, 请合理扮演 ))");
            SendPDMessage("一名不明人士涉嫌偷窃 ~g~工业盐酸~w~ 并进行非法走私.");
            AcitCount -= 10;
            return;
        }
        [Command("putacid")]
        public static async Task COM_AddVehicleAcit(PlayerModel p)
        {
            VehModel veh = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (veh == null) { MainChat.SendErrorChat(p, "[错误] 附近没有车辆!"); return; }

            if (veh.Model != (uint)VehicleModel.Burrito && veh.Model != (uint)VehicleModel.Burrito2 && veh.Model != (uint)VehicleModel.Burrito3 && veh.Model != (uint)VehicleModel.Burrito4 && veh.Model != (uint)VehicleModel.Burrito5 && veh.Model != (uint)VehicleModel.GBurrito &&
               veh.Model != (uint)VehicleModel.GBurrito2 && veh.Model != (uint)VehicleModel.Pony && veh.Model != (uint)VehicleModel.Pony2 && veh.Model != (uint)VehicleModel.Rumpo && veh.Model != (uint)VehicleModel.Rumpo2 && veh.Model != (uint)VehicleModel.Rumpo3 &&
               veh.Model != (uint)VehicleModel.Speedo && veh.Model != (uint)VehicleModel.Speedo2) { MainChat.SendErrorChat(p, "[错误] 工业盐酸只能装载至 Burrito、gBurrito、Pony、Rumpo 和 Speedo 模型车辆."); return; }

            if (!p.HasData("Acit:Steal")) { MainChat.SendErrorChat(p, "[错误] 您手上没有偷取的工业盐酸桶!"); return; }

            if (!await Vehicle.VehicleMain.GetKeysQuery(p, veh)) { MainChat.SendErrorChat(p, "[错误] 您无权操作此车辆!"); return; }

            if (!veh.HasData("AcitStorage"))
                veh.SetData("AcitStorage", 0);

            int totalAcit = veh.lscGetdata<int>("AcitStorage");
            if (totalAcit >= 10) { MainChat.SendErrorChat(p, "[错误] 此车已经装满了工业盐酸."); return; }
            veh.lscSetData("AcitStorage", totalAcit + 1);
            MainChat.SendInfoChat(p, "[*] 成功将工业盐酸装载至车辆, 现在您可以出售给产业, 但请注意您目前的行为是 非法的!");
            GlobalEvents.SetVehicleTag(veh, "[装载]~n~类型: ~o~工业盐酸~w~~n~数量: " + (totalAcit + 1) + "~g~桶");
            p.DeleteData("Acit:Steal");
            Animations.PlayerStopAnimation(p);
            return;
        }

        [Command("sellacid")]
        public static async Task COM_SellAcit(PlayerModel p)
        {
            Models.Components comp = Component_System.GetNearComponent(p.Position);
            if (comp == null) { MainChat.SendErrorChat(p, "[错误] 附近没有仓库货物点."); return; }
            if (comp.Type != 4) { MainChat.SendErrorChat(p, "[错误] 此仓库货物点不采购工业盐酸."); return; }
            VehModel veh = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (veh.Driver != null || !await Vehicle.VehicleMain.GetKeysQuery(p, veh)) { MainChat.SendErrorChat(p, "[错误] 您无权操作此车辆!"); return; }
            if (!veh.GetData("AcitStorage", out int plankCount)) { MainChat.SendErrorChat(p, "[错误] 此车没有装载工业盐酸!"); return; }

            Models.CompanyModel componentOwner = await Database.BusinessDatabase.GetCompany(comp.OwnerBusiness);
            if (componentOwner == null)
                return;

            if (componentOwner.Cash < (plankCount * comp.Stock_2)) { MainChat.SendErrorChat(p, "[错误] 此仓库货物点的产业没有足够的钱购买您的工业盐酸!"); return; }
            if (comp.Stock_3 < plankCount) { MainChat.SendErrorChat(p, "[!] 此仓库货物点已经满了!"); return; }


            int total = (plankCount * comp.Stock_2);
            componentOwner.Cash -= total;
            int multipler = 0;
            if (await BusinesMain.GetPlayerCompany(p) == componentOwner.ID)
                multipler = comp.Stock_2 / 10;
            p.cash += total + multipler;
            veh.DeleteData("AcitStorage");
            GlobalEvents.ClearVehicleTag(veh);
            MainChat.SendInfoChat(p, "[$] 成功将车上的工业盐酸出售并获得了 $" + (total + multipler) + " , 此仓库货物点的产业: " + componentOwner.Name);

            componentOwner.BusinessPrice += total;
            await componentOwner.Update();

            comp.Stock_3 -= plankCount;
            comp.Stock_1 += plankCount;
            await comp.Update();
            TextLabelStreamer.GetDynamicTextLabel(comp.TextLabelID).Text = systems.Component_System.GetComponentDisplayName(comp.Type, comp.Stock_1, comp.Stock_2, comp.Stock_3, comp.SecurityLevel);
            return;
        }
        #endregion

        #region Component Celik
        [Command("buysteel")]
        public static async Task COM_BuySteel(PlayerModel p)
        {
            if (p.Position.Distance(SteelBuyPos) > 5) { MainChat.SendErrorChat(p, "[错误] 您不在钢材购买点."); return; }
            if (p.cash < 500) { MainChat.SendErrorChat(p, "[错误] 您没有足够的钱购买钢材!"); return; }
            p.cash -= 500;
            await p.updateSql();
            MainChat.SendInfoChat(p, "[?] 成功购买 50个 钢材, 价格: $500!");
            VehModel veh = (VehModel)Alt.CreateVehicle(VehicleModel.Trailers, SteelTrailerSpawnPos, new Rotation(0, 0, 0));
            veh.owner = p.sqlID;
            veh.SetData("SteelStorage", 50);
            GlobalEvents.SetVehicleTag(veh, "[装载]~n~类型: ~o~钢材~w~~n~数量: 50~g~个");

            return;
        }
        [Command("sellsteel")]
        public static async Task COM_SellSteel(PlayerModel p)
        {
            Models.Components comp = Component_System.GetNearComponent(p.Position);
            if (comp == null) { MainChat.SendErrorChat(p, "[错误] 附近没有仓库货物点."); return; }
            if (comp.Type != 5) { MainChat.SendErrorChat(p, "[错误] 此仓库货物点不收购钢材."); return; }
            VehModel veh = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (veh.Driver != null || !await Vehicle.VehicleMain.GetKeysQuery(p, veh)) { MainChat.SendErrorChat(p, "[错误] 您无权操作此车辆!"); return; }
            if (!veh.GetData("SteelStorage", out int plankCount)) { MainChat.SendErrorChat(p, "[错误] 此车没有装载钢材!"); return; }

            Models.CompanyModel componentOwner = await Database.BusinessDatabase.GetCompany(comp.OwnerBusiness);
            if (componentOwner == null)
                return;

            if (componentOwner.Cash < (plankCount * comp.Stock_2)) { MainChat.SendErrorChat(p, "[错误] 此仓库货物点的产业没有足够的钱购买您的钢材!"); return; }
            if (comp.Stock_3 < plankCount) { MainChat.SendErrorChat(p, "[!] 此仓库货物点已经满了!"); return; }


            int total = (plankCount * comp.Stock_2);
            componentOwner.Cash -= total;
            int multipler = 0;
            if (await BusinesMain.GetPlayerCompany(p) == componentOwner.ID)
                multipler = comp.Stock_2 / 10;
            p.cash += total + multipler;
            await veh.DestroyAsync();
            MainChat.SendInfoChat(p, "[$] 成功将车上的钢材出售并获得了 $" + (total + multipler) + " , 此仓库货物点的产业: " + componentOwner.Name);

            componentOwner.BusinessPrice += total;
            await componentOwner.Update();

            comp.Stock_3 -= plankCount;
            comp.Stock_1 += plankCount;
            await comp.Update();
            TextLabelStreamer.GetDynamicTextLabel(comp.TextLabelID).Text = systems.Component_System.GetComponentDisplayName(comp.Type, comp.Stock_1, comp.Stock_2, comp.Stock_3, comp.SecurityLevel);
            return;
        }
        #endregion
        #region Component Demir
        [Command("buyiron")]
        public static async Task COM_BuyIron(PlayerModel p)
        {
            if (p.Position.Distance(IronBuyPos) > 10) { MainChat.SendErrorChat(p, "[错误] 您不在铁购买点."); return; }
            VehModel veh = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (veh.Model != (uint)VehicleModel.Pounder2) { MainChat.SendErrorChat(p, "[错误] 工业盐酸只能装载至 Pounder2 模型车辆."); return; }
            if (!await Vehicle.VehicleMain.GetKeysQuery(p, veh)) { MainChat.SendErrorChat(p, "[错误] 您无权操作此车辆!"); return; }
            if (veh.HasData("IronStorage")) { MainChat.SendErrorChat(p, "[错误] 此车已经装载铁了."); return; }
            if (p.cash < 500) { MainChat.SendErrorChat(p, "[错误] 您没有足够的钱购买铁!"); return; }
            p.cash -= 500;
            await p.updateSql();
            GlobalEvents.SetVehicleTag(veh, "[装载]~n~类型: ~o~铁~w~~n~数量: 50~g~个");
            veh.SetData("IronStorage", 50);
            MainChat.SendInfoChat(p, "[*] 成功购买 50个 铁, 价格: $500!");
            return;
        }
        [Command("demirsat")]
        public static async Task COM_SellIron(PlayerModel p)
        {
            Models.Components comp = Component_System.GetNearComponent(p.Position);
            if (comp == null) { MainChat.SendErrorChat(p, "[错误] 附近没有仓库货物点."); return; }
            if (comp.Type != 6) { MainChat.SendErrorChat(p, "[错误] 此仓库货物点不采购铁."); return; }
            VehModel veh = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (veh.Driver != null || !await Vehicle.VehicleMain.GetKeysQuery(p, veh)) { MainChat.SendErrorChat(p, "[错误] 您无权操作此车辆!"); return; }
            if (!veh.GetData("IronStorage", out int plankCount)) { MainChat.SendErrorChat(p, "[错误] 此车没有装载铁!"); return; }

            Models.CompanyModel componentOwner = await Database.BusinessDatabase.GetCompany(comp.OwnerBusiness);
            if (componentOwner == null)
                return;

            if (componentOwner.Cash < (plankCount * comp.Stock_2)) { MainChat.SendErrorChat(p, "[错误] 此仓库货物点的产业没有足够的钱购买您的铁!"); return; }
            if (comp.Stock_3 < plankCount) { MainChat.SendErrorChat(p, "[!] 此仓库货物点已经满了!"); return; }


            int total = (plankCount * comp.Stock_2);
            componentOwner.Cash -= total;
            int multipler = 0;
            if (await BusinesMain.GetPlayerCompany(p) == componentOwner.ID)
                multipler = comp.Stock_2 / 10;
            p.cash += total + multipler;
            MainChat.SendInfoChat(p, "[$] 成功将车上的铁出售并获得了 $" + (total + multipler) + " , 此仓库货物点的产业: " + componentOwner.Name);

            componentOwner.BusinessPrice += total;
            await componentOwner.Update();

            comp.Stock_3 -= plankCount;
            comp.Stock_1 += plankCount;
            await comp.Update();
            veh.DeleteData("IronStorage");
            GlobalEvents.ClearVehicleTag(veh);
            TextLabelStreamer.GetDynamicTextLabel(comp.TextLabelID).Text = systems.Component_System.GetComponentDisplayName(comp.Type, comp.Stock_1, comp.Stock_2, comp.Stock_3, comp.SecurityLevel);
            return;
        }
        #endregion

        public static int TotalPD()
        {
            int count = 0;
            foreach (PlayerModel x in Alt.GetAllPlayers())
            {
                if (x.HasData(EntityData.PlayerEntityData.PDDuty))
                {
                    count++; continue;
                }
            }
            return count;
        }
        public static void SendPDMessage(string Message)
        {
            foreach (PlayerModel x in Alt.GetAllPlayers())
            {
                if (x.HasData(EntityData.PlayerEntityData.PDDuty))
                    GlobalEvents.SubTitle(x, Message, 5);
            }
            return;
        }
    }
}
