using System;
using System.Collections.Generic;
using AltV.Net;
using AltV.Net.Data;
using outRp.OtherSystem.Textlabels;
using outRp.Chat;
using outRp.Globals;
using System.Numerics;
using outRp.Core;
using System.Threading.Tasks;
using System.Linq;
using AltV.Net.Resources.Chat.Api;

namespace outRp.Models
{
    public class Crate
    {
        public int ID { get; set; } = -1;
        public OtherSystem.Textlabels.LProp prop { get; set; } = null;
        public PlayerLabel textlabel { get; set; } = null;
        public int sqlID { get; set; } = -1;
        public string crateModel { get; set; } = null;
        /// <summary>
        /// Kullanılabilecek çeşitler;
        /// 1: İşyeri kutusu | 2: Ev Kutusu | 3: Kişisel kutu | 4: Hastane Kan Cihazı.
        /// </summary>
        public int type { get; set; } = -1;
        public int stock { get; set; } = 0;
        public int owner { get; set; } = -1;
        public string value { get; set; } = "[]";
        public Position pos { get; set; }
        public int dim { get; set; } = -1;
        public bool useable { get; set; } = false;
        public bool locked { get; set; } = false;
        public string password { get; set; } = null;

        public Cratesettings settings { get; set; } = new Cratesettings();

        public Task<int> CreateOnSql() => Database.DatabaseMain.CreateCrate(this);
        public void Delete() => Database.DatabaseMain.DeleteCrate(this);
        public void Update() => Database.DatabaseMain.UpdateCrate(this);

        public class Cratesettings
        {
            public int modifyLevel { get; set; } = 0;
            public int stockCost { get; set; } = 30;
        }
    }
    public class BagModel
    {
        public int ID { get; set; }
        public OtherSystem.Textlabels.LProp prop { get; set; }
        public PlayerLabel label { get; set; }
        public string Env { get; set; }
        public string model { get; set; }
        public int weight { get; set; }
        public Position pos {get; set;}
        public void Update() => Database.DatabaseMain.UpdateBag(this);
        public void Create(PlayerModel x) => Database.DatabaseMain.CreateBag(x, this);
        public void Delete() => Database.DatabaseMain.DeleteBag(this);
    }

    
    public class CrateEvents : IScript
    {
        public static List<Crate> serverCrates = new List<Crate>();
        public static void LoadServerCrates()
        {
            Database.DatabaseMain.LoadAllServerCrates();
            BagEvents.LoadServerBags();
        } 

        [Command("boxstat")]
        public async Task COM_SetCrateUsage(PlayerModel p)
        {
            Crate x = CheckNearCrate(p);
            if (x == null) { MainChat.SendErrorChat(p, "[错误] 附近没有货箱!"); return; }

            BusinessModel biz = await Database.DatabaseMain.GetBusinessInfo(x.owner);
            if (biz == null)
                return;

            if(biz.ownerId != p.sqlID) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }

            x.useable = !x.useable;
            string usableText = (x.useable) ? "开启了." : "关闭了.";

            x.Update();

            MainChat.SendInfoChat(p, "[?] 货箱的状态更新为: " + usableText);
            return;

        }

        [Command("boxprice")]
        public async Task COM_SetStockPrice(PlayerModel p, params string[] args)
        {
            if(args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /boxprice [价格]"); return; }
            Crate x = CheckNearCrate(p);
            if(x == null) { MainChat.SendErrorChat(p, "[错误] 附近没有货箱!"); return; }

            BusinessModel biz = await Database.DatabaseMain.GetBusinessInfo(x.owner);
            if (biz == null)
                return;

            if(biz.ownerId != p.sqlID) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }

            if(!Int32.TryParse(args[0], out int price)) { MainChat.SendInfoChat(p, "[用法] /boxprice [价格]"); return; }

            if(price > 35 || price < 25) { MainChat.SendErrorChat(p, "[错误] 价格可以在 25 - 35."); return; }
            x.settings.stockCost = price;
            MainChat.SendInfoChat(p, "[!] 库存商品价格更新为: $" + price.ToString());
            return;
        }

        public static async Task TakeGround(PlayerModel p, Crate x)
        {
            if (p.Position.Distance(x.prop.Position) > 3f && p.Dimension == x.dim) { MainChat.SendErrorChat(p, "[错误] 附近没有货箱."); return; }
            bool usage = false;
            switch (x.type)
            {
                case 1:
                    List<BusinessModel> pBizz = await Database.DatabaseMain.GetMemberBusinessList(p);
                    foreach(var c1 in pBizz)
                    {
                        if(c1.ID == x.owner) { usage = true; break; }                            
                    }
                    if(x.owner == p.businessStaff) { usage = true; }
                    break;
                    //TODO Ev ve kişisel için düzenle
                default:
                    break;
            }

            if (!usage) { MainChat.SendErrorChat(p, Globals.CONSTANT.ERR_PermissionError); return; }
            if (p.lscGetdata<bool>(EntityData.PlayerEntityData.hasProp)) { MainChat.SendErrorChat(p, "[错误] 您手上已经有货箱了!"); return; }
            //x.prop.Position = p.Position;
            GlobalEvents.PlayAnimation(p, ServerAnimations.Crate_TakeGround, 0);
            GlobalEvents.ProgresBar(p, "拿起了货箱...", 2);
            await Task.Delay(2000);

            if (!p.Exists)
                return;

            GlobalEvents.PlayAnimation(p, ServerAnimations.putObject, 53);
            GlobalEvents.notify(p, 2, "您拿起了货箱!");

            x.prop.Attach = new AttachObj() { attach = p.Id, bone = 60309, off = new Vector3(0.025f, 0.08f, 0.255f), rot = new Vector3(-145.0f, 290.0f, 0.0f) };
            p.lscSetData(EntityData.PlayerEntityData.propInfo, x);
            x.textlabel.Delete();
        }
        public static async void DropGround(PlayerModel p)
        {
            Position pPos = p.Position;
            pPos.Z -= 1f;
            Crate x = p.lscGetdata<Crate>(EntityData.PlayerEntityData.propInfo);
            if (x == null) { MainChat.SendErrorChat(p, "[错误] 您手上没有货箱!"); return; }
            GlobalEvents.PlayAnimation(p, ServerAnimations.Crate_DropGround, 0);
            GlobalEvents.ProgresBar(p, "放下了货箱...", 2);
            await Task.Delay(2000);
            GlobalEvents.notify(p, 2, "您放下了货箱!");
            GlobalEvents.StopAnimation(p);
            x.prop.Delete();
            await Task.Delay(200);

            if (!p.Exists)
                return;

            x.prop = PropStreamer.Create(x.crateModel, pPos, new Vector3(0, 0, 0),frozen: true, streamRange: 6000, dimension: p.Dimension);
            p.DeleteData(EntityData.PlayerEntityData.hasProp);
            p.DeleteData(EntityData.PlayerEntityData.propInfo);
            PlayerLabel propLabel = TextLabelStreamer.Create("~x~[~w~ID: " + x.sqlID + "~x~]~w~~n~货箱~n~指令: ~g~/box", x.prop.Position, streamRange: 3);
            x.textlabel = propLabel;
            x.pos = pPos;
            x.dim = p.Dimension;
            x.Update();
        }

        [Command("addbox")]
        public async Task addCrate(PlayerModel p, string kutu)
        {
            if(p.adminLevel < 4) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; }
            OtherSystem.Textlabels.PlayerLabel L = OtherSystem.Textlabels.TextLabelStreamer.Create("x", p.Position, dimension: p.Dimension);
            OtherSystem.Textlabels.LProp P = OtherSystem.Textlabels.PropStreamer.Create(kutu, p.Position, new Vector3(0, 0, 0), streamRange: 1000, frozen: true, dimension: p.Dimension);
            if (p == null)
                return;

            Crate x = new()
            {
                owner = p.businessStaff,
                crateModel = kutu,
                type = 1,
                value = "[]",
                pos = p.Position,
                dim = p.Dimension,
                useable = true,
                locked = false,
                password = "0",
                prop = P,
                textlabel = L
            };
            int id = await x.CreateOnSql();
            x.sqlID = id;
            x.ID = p.Id;
            serverCrates.Add(x);
            L.Text = "~x~[~w~ID: " + id + "~x~]~w~~n~货箱~n~指令: ~g~/box";
            return;
        }

        [Command(CONSTANT.COM_CrateEvent)]
        public static void COM_CrateEvent(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, CONSTANT.DESC_CrateEvent); return; }
            switch (args[0].ToString())
            {
                case "pickup":
                    Crate x = CrateEvents.serverCrates.Find(x => x.sqlID == Int32.Parse(args[1].ToString()));
                    if(x == null) { MainChat.SendErrorChat(p, "[错误] 附近没有货箱!"); return; }
                    CrateEvents.TakeGround(p, x);
                    break;
                case "drop":
                    DropGround(p);
                    break;
                default:
                    MainChat.SendInfoChat(p, CONSTANT.DESC_CrateEvent);
                    break;
            }
        }

        public static Crate CheckNearCrate(PlayerModel p, int radius = 5)
        {
            foreach(var x in serverCrates)
            {
                if (p.Position.Distance(x.pos) < radius && p.Dimension == x.dim) { return x; }
            }                                                         

            return null;
        }

        [Command("editbox")]
        public async Task COM_EditCreate(PlayerModel p, params string[] args)
        {
            AccountModel account = await Database.DatabaseMain.getAccInfo(p.accountId);
            if (account == null)
                return;

            if (p.adminLevel >= 7) {
                Crate c = null;
                foreach (var x in serverCrates)
                {
                    if (p.Position.Distance(x.pos) < 5 && p.Dimension == x.dim) { c = x; }
                }

                if(args.Length <= 1) { MainChat.SendInfoChat(p, "editbox"); return; }
                if (c == null) { MainChat.SendErrorChat(p, "[错误] 附近没有货箱"); return; }
                int value; bool isOk = Int32.TryParse(args[1], out value);
                if (!isOk) { MainChat.SendInfoChat(p, "editbox desc"); return; }

                switch (args[0])
                {
                    case "type":
                        c.type = value;
                        break;

                    case "stock":
                        c.stock = value;
                        break;

                    case "owner":
                        c.owner = value;
                        break;

                    case "stat":
                        bool useable = false; ;
                        if (value == 1) { useable = true; }
                        else { useable = false; }
                        c.useable = useable;
                        break;

                    case "lock":
                        bool isLocked = false;
                        if (value == 1) { isLocked = true; }
                        else { isLocked = false; }
                        c.locked = isLocked;
                        break;

                    case "pass":
                        c.password = value.ToString();
                        break;

                    case "modlevel":
                        c.settings.modifyLevel = value;
                        c.Update();
                        MainChat.SendInfoChat(p, "[货箱] 更新货箱等级为: " + value.ToString());
                        return;

                }

                c.Update();
                MainChat.SendInfoChat(p, "> 更新货箱信息.");
                return;
            }
            
            
            
            MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return;

           
        }

        [Command("boxstock")]
        public void ShowStock(PlayerModel p)
        {
            Crate c = CheckNearCrate(p);
            if (c == null) { MainChat.SendErrorChat(p, "[错误] 附近没有货箱"); return; }

            MainChat.SendInfoChat(p, "[货箱库存: " + c.stock.ToString() + "]");
        }
    }

    public class BagEvents : IScript
    {
        public class BagOptions
        {
            public int bone { get; set; }
            public Vector3 Pos { get; set; }
            public Vector3 rot { get; set; }
        }

        public static List<BagModel> serverBags = new List<BagModel>();

        public static Dictionary<string, BagOptions> bagOptions = new Dictionary<string, BagOptions>()
        {
            ["p_michael_backpack_s"] = new BagOptions() { bone = 24818, Pos = new Vector3( 0.07f, -0.11f, -0.05f), rot = new Vector3( 0.0f, 90.0f, 175.0f ) }
        };
        public static void LoadServerBags()
        {
            Database.DatabaseMain.LoadAllServerBags();
        }

        [Command("bag")]
        public static void COM_BagEvent(PlayerModel p, params string[] args)
        {
            if(args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /bag [选项] [ID]<br>可用指令: pickup | drop | show<br> 例如: /box pickup 5"); return; }
            
            switch (args[0])
            {
                case "pickup":
                    if (p.HasData(EntityData.PlayerEntityData.Bag)) { MainChat.SendErrorChat(p, "[错误] 您已有背着的包了!"); return; }
                    int cantaId = Int32.Parse(args[1]);
                    BagModel canta = serverBags.Find(x => x.ID == cantaId);
                    if (canta == null) { MainChat.SendErrorChat(p, "[错误] 无效包."); return; }
                    if (p.Position.Distance(canta.prop.Position) > 4) { MainChat.SendErrorChat(p, "[错误] 您离包太远!"); return; }

                    BagOptions caseAl = bagOptions.Where(x => x.Key == canta.model).FirstOrDefault().Value;
                    AttachObj caseAlAtt = new AttachObj()
                    {
                        attach = p.Id,
                        bone = caseAl.bone,
                        off = caseAl.Pos,
                        rot = caseAl.rot
                    };
                    canta.prop.Position = p.Position;
                    canta.prop.Attach = caseAlAtt;
                    canta.prop.Range = 7000;
                    canta.label.Delete();
                    p.lscSetData(EntityData.PlayerEntityData.Bag, canta.ID);
                    return;

                case "drop":
                    if (!p.HasData(EntityData.PlayerEntityData.Bag)) { MainChat.SendErrorChat(p, "[错误] 您没有背着包!"); return; }
                    BagModel birakBag = serverBags.Find(x => x.ID == p.lscGetdata<int>(EntityData.PlayerEntityData.Bag));
                    birakBag.prop.Delete();
                    Position bPos = p.Position;
                    bPos.Z -= 1;
                    birakBag.prop = PropStreamer.Create(birakBag.model, bPos, new Vector3(0, 0, 0));
                    birakBag.label = TextLabelStreamer.Create("[背包:" + birakBag.ID + "]~n~指令: ~g~/~w~bag", bPos, streamRange: 2);
                    p.DeleteData(EntityData.PlayerEntityData.Bag);
                    return;

                case "show":
                    if(args[1] == null) { MainChat.SendInfoChat(p, "[用法] /bag [选项] [ID]<br>可用指令: pickup | drop | show<br> 例如: /box pickup 5"); return; }
                    int cantaIdac = Int32.Parse(args[1]);
                    BagModel cantaac = serverBags.Find(x => x.ID == cantaIdac);
                    if (cantaac == null) { MainChat.SendErrorChat(p, "[错误] 无效包."); return; }
                    if (cantaac.label == null) { MainChat.SendErrorChat(p, "[错误] 您必须把包放在地上才可以查看"); return; }
                    if (p.Position.Distance(cantaac.label.Position) > 4) { MainChat.SendErrorChat(p, "[错误] 您离指定包太远!"); return; }                    
                    OtherSystem.otherEnv.ShowBag(p, cantaac);
                    return;

                default:
                    MainChat.SendInfoChat(p, "[用法] /bag [选项] [ID]<br>可用指令: pickup | drop | show<br> 例如: /box pickup 5");
                    return;
            }
        }

    }


}
