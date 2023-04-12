using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using outRp.Chat;
using outRp.Globals;
using outRp.OtherSystem.NativeUi;
using outRp.OtherSystem.Textlabels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;

namespace outRp.Models
{
    public class teleporters : IScript
    {
        public class TeleportModel
        {
            public int ID { get; set; } = 0;
            public Position pos { get; set; } = new Position(0, 0, 0);
            public List<PosModel> coords { get; set; } = new List<PosModel>();
            public int dimension { get; set; } = 0;
            public ulong LabelID { get; set; } = 0;
            public int OwnerId { get; set; } = 0;
            public bool isLocked { get; set; } = true;

            public Task<int> Create() => Database.DatabaseMain.CreateTeleporter(this);
            public async Task Update() => await Database.DatabaseMain.UpdateTeleporters(this);
            public void Delete() => Database.DatabaseMain.DeleteTeleporter(this);

            public class PosModel
            {
                public ulong ID { get; set; } = 0;
                public string Name { get; set; } = "无";
                public Position pos { get; set; } = new Position(0, 0, 0);
                public int Dimension { get; set; } = 0;
            }
        }

        public static async Task LoadAllTeleporters()
        {
            List<TeleportModel> allT = await Database.DatabaseMain.GetAllTeleporters();
            foreach (TeleportModel t in allT)
            {
                PlayerLabel z = TextLabelStreamer.Create("[" + t.ID + "]按 ~g~[E]", t.pos, dimension: t.dimension, font: 0, streamRange: 3);
                t.LabelID = z.Id;
                z.SetData("isTeleporter", t.ID);

                foreach (TeleportModel.PosModel x in t.coords)
                {
                    PlayerLabel h = TextLabelStreamer.Create("按 ~g~[E]", x.pos, x.Dimension, font: 0, streamRange: 3);
                    x.ID = h.Id;
                    h.SetData("isTeleporter", t.ID);
                }
                t.Update();
            }

            Alt.Log("加载 建筑物.");
            return;
        }


        [Command("addtp")]
        public async Task CreateTeleporter(PlayerModel p)
        {
            if (p.adminLevel < 2) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            TeleportModel t = new TeleportModel();
            t.pos = p.Position;
            t.dimension = p.Dimension;
            t.ID = await t.Create();
            PlayerLabel z = TextLabelStreamer.Create("[" + t.ID + "]按 ~g~[E]", t.pos, dimension: p.Dimension, font: 0, streamRange: 3);
            t.LabelID = z.Id;
            z.SetData("isTeleporter", t.ID);

            MainChat.SendErrorChat(p, "[!] 成功创建建筑物 " + t.ID);
            return;
        }

        [Command("edittp")]
        public async Task UpdateTeleporter(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 2) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /edittp [选项] [数值]<br>可用选项:<br>near: 删除附近的建筑物<br>all: 删除指定建筑物."); return; }
            if (!Int32.TryParse(args[0], out int teleID)) { MainChat.SendErrorChat(p, "[错误] 无效建筑物!"); return; }

            TeleportModel t = await Database.DatabaseMain.GetTeleporterByID(teleID);
            if (t == null) { MainChat.SendErrorChat(p, "[错误] 无效建筑物!"); return; }

            switch (args[1])
            {

                case "near":
                    bool RemoveOk = false;
                    foreach (TeleportModel.PosModel x in t.coords)
                    {
                        if (x.pos.Distance(p.Position) < 3 && x.Dimension == p.Dimension)
                        {
                            RemoveOk = true;
                            TextLabelStreamer.GetDynamicTextLabel(x.ID).Delete();
                            t.coords.Remove(x);
                            break;
                        }
                    }
                    if (RemoveOk) { t.Update(); MainChat.SendErrorChat(p, "[!] 成功删除附近建筑物."); return; }
                    else { MainChat.SendErrorChat(p, "[!] 无效建筑物!"); return; }

                case "all":
                    //TextLabelStreamer.GetDynamicTextLabel(t.LabelID).Delete();
                    TextLabelStreamer.GetAllDynamicTextLabels().Find(x => x.Text == "[" + t.ID + "]按 ~g~[E]").Delete();
                    foreach (TeleportModel.PosModel y in t.coords)
                    {
                        TextLabelStreamer.GetDynamicTextLabel(y.ID).Delete();
                    }
                    t.Delete();
                    MainChat.SendErrorChat(p, "[!] 成功删除指定建筑物.");
                    return;

                case "vw":
                    if (!Int32.TryParse(args[2], out int dim)) { MainChat.SendErrorChat(p, "[错误] 无效虚拟世界."); return; }
                    t.dimension = dim;
                    t.Update();
                    MainChat.SendErrorChat(p, "[!] 已更新建筑物虚拟世界.");
                    return;

                case "owner":
                    if (!Int32.TryParse(args[2], out int owner)) { MainChat.SendErrorChat(p, "[错误] 无效玩家."); return; }
                    t.OwnerId = owner;
                    t.Update();
                    MainChat.SendInfoChat(p, "[!] 已更新建筑物所有人.");
                    return;


                default:
                    MainChat.SendInfoChat(p, "[用法] /edittp [选项] [数值]<br>可用选项:<br>near: 删除附近的建筑物<br>all: 删除指定建筑物."); return;
            }
        }

        public static async Task ShowTeleporter(PlayerModel p, int ID)
        {
            TeleportModel t = await Database.DatabaseMain.GetTeleporterByID(ID);
            if (t == null) { MainChat.SendErrorChat(p, "[!] 无效建筑物."); return; }

            List<GuiMenu> gMenu = new List<GuiMenu>();

            GuiMenu clothes2 = new GuiMenu { name = "主入口", triger = "Teleport:UseEvent", value = "主," + t.ID };
            gMenu.Add(clothes2);

            foreach (TeleportModel.PosModel c in t.coords)
            {
                GuiMenu clothes1 = new GuiMenu { name = c.Name, triger = "Teleport:UseEvent", value = c.Name + "," + t.ID };
                gMenu.Add(clothes1);
            }

            GuiMenu close = GuiEvents.closeItem;
            gMenu.Add(close);

            Gui y = new Gui()
            {
                image = "",
                guiMenu = gMenu,
                color = "#DEE9B5"
            };
            y.Send(p);

        }

        [AsyncClientEvent("Teleport:UseEvent")]
        public async Task TeleportUseEvent(PlayerModel p, string name)
        {
            if (p.Ping > 250)
                return;
            GuiEvents.GUIMenu_Close(p);
            string[] val = name.Split(",");
            if (!Int32.TryParse(val[1], out int tID))
                return;

            TeleportModel t = await Database.DatabaseMain.GetTeleporterByID(tID);
            if (t == null)
                return;

            if (t.isLocked)
            {
                MainChat.SendErrorChat(p, "[错误] 门是锁的.");
                return;
            }

            if (val[0] == "主") { p.Position = t.pos; p.Dimension = t.dimension; }
            else
            {
                TeleportModel.PosModel x = t.coords.Find(x => x.Name == val[0]);
                if (x == null)
                    return;

                p.Position = x.pos;
                p.Dimension = x.Dimension;
            }
            return;
        }

        [Command("tplock")]
        public async Task COM_TLock(PlayerModel p)
        {
            PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Find(x => p.Position.Distance(x.Position) < 3f && x.Dimension == p.Dimension);
            if (entranceLabel != null)
            {
                if (entranceLabel.TryGetData("isTeleporter", out int teleID))
                {
                    var tele = await Database.DatabaseMain.GetTeleporterByID(teleID);
                    if (tele != null)
                    {
                        if (tele.OwnerId == p.sqlID)
                        {
                            tele.isLocked = !tele.isLocked;
                            await tele.Update();
                            MainChat.SendInfoChat(p, "[?] 已更新建筑物解锁状态为: " + ((tele.isLocked) ? "是" : "否"));
                            return;
                        }
                        else
                        {
                            MainChat.SendErrorChat(p, "[错误] 无权操作!");
                            return;
                        }
                    }
                    else
                    {
                        MainChat.SendErrorChat(p, "[错误] 无权操作!");
                        return;
                    }
                }
                else
                {
                    MainChat.SendErrorChat(p, "[错误] 无权操作!");
                    return;
                }
            }
            else
            {
                MainChat.SendErrorChat(p, "[错误] 无权操作!");
                return;
            }
        }
    }
}
