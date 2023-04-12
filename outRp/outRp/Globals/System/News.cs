using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Core;
using outRp.Models;
using outRp.OtherSystem.Textlabels;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;

namespace outRp.Globals.System
{
    public class News : IScript
    {
        public static void LoadNewsSystem()
        {
            TextLabelStreamer.Create("[威泽电视新闻]~n~输入 /ad 打广告~n~价格: ~g~$1/字符", ServerGlobalValues.adversimentPosition, font: 0, streamRange: 5);
            //TextLabelStreamer.Create("[SAN Network]~n~Reklam vermek için /reklam~n~Karakter başına ücret: ~g~$1", new Position(-1229.6307f, -856.2857f, 13.137695f), font: 0, streamRange: 5);
            GlobalEvents.blipModel factBlip = new GlobalEvents.blipModel();
            factBlip.blipname = "newsAdversimentBlip";
            factBlip.category = 2;
            factBlip.label = "威泽新闻";
            factBlip.position = ServerGlobalValues.adversimentPosition;
            factBlip.sprite = 459;
            GlobalEvents.serverBlips.Add(factBlip);
            Alt.Log("加载 威泽新闻系统.");
        }

        public class AdversimentModel
        {
            public int senderID { get; set; }
            public string addvesimentText { get; set; }
            public string pureText { get; set; } = "无";
            public int pureNumber { get; set; } = 0;

            public string kook_Text { get; set; }
        }

        public static List<AdversimentModel> serverAdversiment = new List<AdversimentModel>();
        public static async Task<bool> CheckPlayerInNews(PlayerModel p)
        {
            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            if (fact.type == ServerGlobalValues.fType_News) { return true; }
            else { return false; }
        }

        [Command("newsduty")]
        public static async Task COM_NewsDuty(PlayerModel p)
        {
            if (p.factionId <= 0) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }

            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            if (fact == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }

            if (fact.type != ServerGlobalValues.fType_News) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }

            if (p.HasData(EntityData.PlayerEntityData.NewsDuty))
            {
                p.DeleteData(EntityData.PlayerEntityData.NewsDuty);
                MainChat.SendInfoChat(p, "[新闻] 您下班了.");
                GlobalEvents.ClearPlayerTag(p);
                return;
            }
            else
            {
                p.SetData(EntityData.PlayerEntityData.NewsDuty, true);
                GlobalEvents.SetPlayerTag(p, "~y~记者证~n~" + fact.name);
                MainChat.SendInfoChat(p, "[新闻] 您上班了.");
                return;
            }

        }

        [Command(CONSTANT.COM_CreateBroadCast)]
        public static async Task COM_CreateBroadCast(PlayerModel p, params string[] args)
        {
            if (await CheckPlayerInNews(p) == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, CONSTANT.DESC_CreateBroadCast); return; }
            p.lscSetData(EntityData.Faction_News.InBroadCast, p.factionId);
            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            string message = "{D76F00}" + string.Format(CONSTANT.INFO_CreateBroadCast, fact.name, p.characterName.Replace("_", " ")) + "{FFFFFF}" + string.Join(" ", args);
            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (t.isNews)
                {
                    t.SendChatMessage(message);
                }
            }
            return;
        }

        [Command(CONSTANT.COM_BroadCastChat)]
        public static async Task COM_BroadCastChat(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, CONSTANT.DESC_BroadCastChat); return; }
            if (!p.HasData(EntityData.Faction_News.InBroadCast))
            {
                MainChat.SendErrorChat(p, CONSTANT.ERR_BroadCastChatNotIn); return;
            }
            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(p.lscGetdata<int>(EntityData.Faction_News.InBroadCast));
            string own = "嘉宾";
            if (fact.ID == p.factionId)
            {
                own = fact.name + "记者";
            }

            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (!t.HasData("News:BroadCast:Closed") && !t.HasData("News:Watching"))
                {
                    t.SendChatMessage("{D76F00}" + string.Format(CONSTANT.INFO_BroadCastChatSendMessage, own, p.characterName.Replace("_", " ")) + "{FFFFFF}" + string.Join(" ", args));
                }
            }
            return;
        }

        [Command("closebc")]
        public static async Task COM_CloseBroadCast(PlayerModel p)
        {
            if (await CheckPlayerInNews(p) == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (!p.HasData(EntityData.Faction_News.InBroadCast)) { MainChat.SendErrorChat(p, "[错误] 您没有在直播."); return; }
            if (p.lscGetdata<int>(EntityData.Faction_News.InBroadCast) != p.factionId) { MainChat.SendErrorChat(p, "[错误] 这场直播不属于您的组织!"); return; }

            foreach (PlayerModel x in Alt.GetAllPlayers())
            {
                if (x.HasData(EntityData.Faction_News.InBroadCast))
                {
                    if (x.lscGetdata<int>(EntityData.Faction_News.InBroadCast) == p.factionId)
                    {
                        MainChat.SendInfoChat(x, "[!] " + p.characterName.Replace("_", " ") + " 结束了现场直播.");
                        x.DeleteData(EntityData.Faction_News.InBroadCast);
                    }
                }
            }
            MainChat.SendInfoChat(p, "[!] 您结束了直播.");
            return;
        }

        [Command(CONSTANT.COM_Adversiment)]
        public static async Task COM_Addversiment(PlayerModel p, params string[] args)
        {
            //..GetFactionFromType
            if (args.Length <= 0) { MainChat.SendErrorChat(p, CONSTANT.DESC_Adversiment); return; }
            if (p.Position.Distance(ServerGlobalValues.adversimentPosition) > 5 && p.Position.Distance(new Position(-1229.6307f, -856.2857f, 13.137695f)) > 5) { MainChat.SendErrorChat(p, "[错误] 您不在威泽电视新闻广告点."); return; }
            var chechMessage = serverAdversiment.Find(x => x.senderID == p.sqlID);
            if (chechMessage != null) { MainChat.SendErrorChat(p, "[错误] 您已有一条待投放的广告, 请等待管理员审核."); return; }
            string message = string.Join(" ", args);
            if (p.cash < message.Length) { MainChat.SendErrorChat(p, CONSTANT.ERR_MoneyNotEnought); return; }
            p.cash -= message.Length;
            await p.updateSql();
            string Header = "{358039}<i class='far fa-newspaper'></i> [威泽电视新闻] {FFFFFF}";
            if (p.Position.Distance(new Position(-1229.6307f, -856.2857f, 13.137695f)) < 5)
            {
                Header = "{358039}<i class='far fa-newspaper'></i> [圣安地列斯新闻网络] {FFFFFF}";
            }
            string addversiment = Header + message + "<br> {358039}联系人:{FFFFFF} " + p.characterName.Replace("_", " ");
            if (p.phoneNumber > 0) { addversiment += " <i class='fas fa-phone'></i> #" + p.phoneNumber.ToString(); }
            AdversimentModel newAdd = new AdversimentModel();
            newAdd.addvesimentText = addversiment;
            newAdd.senderID = p.sqlID;
            newAdd.pureText = message;
            newAdd.pureNumber = p.phoneNumber;
            serverAdversiment.Add(newAdd);
            MainChat.SendInfoChat(p, "{358039}> 您的广告请求已由发送至管理员进行审核, 审核通过后会进行投放(非即时投放, 按队列投放), 需要支付广告费用: " + message.Length.ToString());

            //if (p.Position.Distance(new Position(-1229.6307f, -856.2857f, 13.137695f)) < 5)
            //{
            //    FactionModel fct = await Database.DatabaseMain.GetFactionInfo(297);
            //    if (fct != null)
            //    {
            //        fct.cash += message.Length;
            //        fct.Update();
            //    }
            //}
            //else
            //{
            FactionModel fact = await Database.DatabaseMain.GetFactionFromType(ServerGlobalValues.fType_News);
            if (fact != null)
            {
                fact.cash += message.Length;
                fact.Update();
            }
            //}

            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (t.adminLevel > 0 && t.adminWork)
                {
                    t.SendChatMessage("> {C66E04}收到新的广告投放请求, 请尽快进行审核.");
                }
            }
        }

        [Command("mad")]
        public async Task COM_MobileAdvertisiment(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendErrorChat(p, CONSTANT.DESC_Adversiment); return; }
            //if (p.Position.Distance(ServerGlobalValues.adversimentPosition) > 5 && p.Position.Distance(new Position(-1229.6307f, -856.2857f, 13.137695f)) > 5) { MainChat.SendErrorChat(p, "[错误] Bu komutu kullanabilmek için reklam alanında olmalısınız."); return; }
            var chechMessage = serverAdversiment.Find(x => x.senderID == p.sqlID);
            if (chechMessage != null) { MainChat.SendErrorChat(p, "[错误] 您已有一条待投放的广告, 请等待管理员审核."); return; }
            string message = string.Join(" ", args);
            if (p.cash < (message.Length * 2)) { MainChat.SendErrorChat(p, CONSTANT.ERR_MoneyNotEnought); return; }
            p.cash -= (message.Length * 2);
            await p.updateSql();
            string Header = "{358039}<i class='far fa-newspaper'></i> [威泽电视新闻] {FFFFFF}";
            string addversiment = Header + message + "<br> {358039}联系人:{FFFFFF} " + p.characterName.Replace("_", " ");
            if (p.phoneNumber > 0) { addversiment += " <i class='fas fa-phone'></i> #" + p.phoneNumber.ToString(); }
            AdversimentModel newAdd = new AdversimentModel();
            newAdd.addvesimentText = addversiment;
            newAdd.senderID = p.sqlID;
            newAdd.pureText = message;
            newAdd.pureNumber = p.phoneNumber;
            serverAdversiment.Add(newAdd);
            MainChat.SendInfoChat(p, "{358039}> 您的广告请求已由发送至管理员进行审核, 审核通过后会进行投放(非即时投放, 按队列投放), 需要支付广告费用: " + (message.Length * 6).ToString());

            //if (p.Position.Distance(new Position(-1229.6307f, -856.2857f, 13.137695f)) < 5)
            //{
            //    FactionModel fct = await Database.DatabaseMain.GetFactionInfo(297);
            //    if (fct != null)
            //    {
            //        fct.cash += message.Length;
            //        fct.Update();
            //    }
            //}
            //else
            //{
            FactionModel fact = await Database.DatabaseMain.GetFactionFromType(ServerGlobalValues.fType_News);
            if (fact != null)
            {
                fact.cash += message.Length;
                fact.Update();
            }
            //}

            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (t.adminLevel > 0 && t.adminWork)
                {
                    t.SendChatMessage("> {C66E04}收到新的广告投放请求, 请尽快进行审核.");
                }
            }
        }

        [Command("aad")]
        public static void COM_AdminAdvertisiment(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            if (args.Length <= 0) { MainChat.SendErrorChat(p, CONSTANT.DESC_Adversiment); return; }
            string message = string.Join(" ", args);
            string Header = "{358039}<i class='far fa-newspaper'></i> [威泽电视新闻] {FFFFFF}";
            string addversiment = Header + message + "<br> {358039}联系人:{FFFFFF} " + p.characterName.Replace("_", " ");
            if (p.phoneNumber > 0) { addversiment += " <i class='fas fa-phone'></i> #" + p.phoneNumber.ToString(); }
            AdversimentModel newAdd = new AdversimentModel();
            newAdd.addvesimentText = addversiment;
            newAdd.senderID = p.sqlID;
            newAdd.pureText = message;
            newAdd.pureNumber = p.phoneNumber;
            Globals.Commands.AdminCommands.trustedAdversiments.Add(newAdd);
            MainChat.SendInfoChat(p, "{358039}> 已经加入投放队列.");
        }

        [Command("bcinvite")]
        public static async Task COM_InviteBroadCast(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /bcinvite [ID]"); return; }
            if (!await CheckPlayerInNews(p)) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (!p.HasData(EntityData.Faction_News.InBroadCast)) { MainChat.SendErrorChat(p, "[错误] 您没有在现场直播!"); return; }
            if (!Int32.TryParse(args[0], out int tSql)) { MainChat.SendInfoChat(p, "[用法] /bcinvite [ID]"); return; }
            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家!"); return; }
            if (t.Position.Distance(p.Position) > 5) { MainChat.SendErrorChat(p, "[错误] 指定玩家离您太远."); return; }
            OtherSystem.NativeUi.Inputs.SendButtonInput(t, p.characterName.Replace("_", " ") + "您被邀请至新闻直播间.", "BroadCast:Invite", p.factionId.ToString());
            MainChat.SendInfoChat(p, "[!] " + t.characterName.Replace("_", " ") + " 被您邀请至直播间.");
            return;
        }

        [AsyncClientEvent("BroadCast:Invite")]
        public static void COM_AnswerBroadCast(PlayerModel p, bool state, string _factionId)
        {
            if (!Int32.TryParse(_factionId, out int factionId))
                return;
            if (state)
            {
                p.SetData(EntityData.Faction_News.InBroadCast, factionId);
                MainChat.SendInfoChat(p, "[!] 您加入了直播.");
                GlobalEvents.NativeNotifyAll(p, "~o~加入了直播间.");
            }
            else
            {
                GlobalEvents.NativeNotifyAll(p, "~r~拒绝了直播邀请.");
            }
            return;
        }

        [Command("cameraon")]
        public async Task COM_OpenCamera(PlayerModel p)
        {
            //MainChat.SendErrorChat(p, "[错误]Sistem bir süreliğine pasif haldedir.");
            //return;

            if (p.HasData("News:inCamera")) { MainChat.SendErrorChat(p, "[错误] 您已开启了直播设备, 请先关闭直播设备!"); return; }
            if (p.factionId <= 0) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }

            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            if (fact == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }

            if (fact.type != ServerGlobalValues.fType_News) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }

            p.SetData("News:inCamera", true);

            MainChat.SendInfoChat(p, "[!] 您开启了直播直播设备, 观众可以输入 '/joincr " + p.sqlID + "' 观看直播.");
            return;
        }

        [Command("cameraoff")]
        public static void COM_CloseCamera(PlayerModel p)
        {
            if (!p.HasData("News:inCamera")) { MainChat.SendErrorChat(p, "[错误] 您无法使用此指令, 因为您没有开启直播设备!"); return; }
            p.DeleteData("News:inCamera");
            int watchers = 0;
            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (t.Position.Distance(p.Position) < 20)
                {
                    if (t.HasData("News:Watching"))
                    {
                        //object[] inf = t.lscGetdata<object[]>("News:Watching");
                        //t.Position = (Position)inf[0];
                        //Position pos = (Position)inf[0];
                        //t.EmitLocked("News:StopCamera", pos);
                        //t.Dimension = (int)inf[1];
                        //t.DeleteData("News:Watching");
                        //t.Visible = true;
                        //t.fakeName = t.characterName;
                        //t.showSqlId = true;
                        MainChat.SendInfoChat(t, "[!] 直播已结束!");
                        watchers += 1;
                        COM_StartCamera(t);
                    }
                }
            }
            MainChat.SendInfoChat(p, "[!] 您关闭了直播并结束了有 " + watchers + " 观众的直播间!");
            GlobalEvents.ClearPlayerTag(p);
        }


        [Command("joincr")]
        [Obsolete]
        public static void COM_StartCamera(PlayerModel p, params string[] args)
        {
            if (p.HasData("News:Watching"))
            {
                MainChat.SendErrorChat(p, "[错误] 您已在直播间, 请先输入 /quitcr 退出直播.");
                return;
            }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /joincr [id]"); return; }
            if (!Int32.TryParse(args[0], out int tSql)) { MainChat.SendInfoChat(p, "[用法] /joincr [id]"); return; }

            if (tSql == p.sqlID) { MainChat.SendErrorChat(p, "[错误] 您无法对自己使用!"); return; }
            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null) { MainChat.SendInfoChat(p, "[用法] /joincr [id]"); return; }
            if (!t.HasData("News:inCamera")) { MainChat.SendErrorChat(p, "[错误] 指定玩家没有开启直播设备!"); return; }

            p.SetData("News:Watching", new object[] { p.Position, p.Dimension, t.sqlID });
            p.Position = t.Position;
            p.Dimension = t.Dimension;
            p.Visible = false;
            p.fakeName = "";
            p.showSqlId = false;

            p.EmitAsync("News:StartCamera", t.Id);
            MainChat.SendInfoChat(p, "[!] 您开始观看 " + t.characterName.Replace("_", " ") + " 的现场直播! 可以输入 /quitcr 退出直播.");
            CalcSpectators(t);
            return;
        }

        public static void CalcSpectators(PlayerModel p)
        {
            int count = 0;
            foreach (IPlayer pl in Alt.GetAllPlayers())
            {
                if (pl.Position.Distance(p.Position) < 5 && pl.HasData("News:Watching"))
                    ++count;
            }

            if (count > 0)
            {
                GlobalEvents.SetPlayerTag(p, "~b~观众人数: ~g~" + count);
                MainChat.SendInfoChat(p, "[?] 目前观众: " + count);
            }
            else
            {
                GlobalEvents.ClearPlayerTag(p);
            }
        }

        [Command("quitcr")]
        [Obsolete]
        public static void COM_StartCamera(PlayerModel p)
        {
            if (!p.HasData("News:Watching"))
            {
                MainChat.SendErrorChat(p, "[错误] 您没有在观看直播.");
                return;
            }

            object[] inf = p.lscGetdata<object[]>("News:Watching");
            p.Position = (Position)inf[0];
            Position pos = (Position)inf[0];
            p.Spawn(pos, 0);
            p.maxHp = 1000;
            p.hp = p.hp;
            p.EmitAsync("News:StopCamera", pos);
            p.Dimension = (int)inf[1];
            p.DeleteData("News:Watching");
            p.Visible = true;
            p.fakeName = p.characterName;
            p.showSqlId = true;
            MainChat.SendInfoChat(p, "[!] 您已退出直播观看!");
            if (Int32.TryParse(inf[2].ToString(), out int tsql))
            {
                var target = GlobalEvents.GetPlayerFromSqlID(tsql);
                if (target != null)
                    CalcSpectators(target);
            }
            p.maxHp = p.maxHp;
            p.hp = p.hp;
            //p.SetHealthAsync((ushort)p.hp);
            return;
        }

        [Command("nr")]
        public static async Task COM_NewsRadio(PlayerModel player, params string[] messages)
        {
            if (messages.Length <= 0) { MainChat.SendInfoChat(player, "[用法] /nr [文本]"); return; }
            bool pdCheck = await CheckPlayerInNews(player);
            if (!pdCheck) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotInPd); return; }
            //if (player.lscGetdata<bool>(EntityData.PlayerEntityData.NewsDuty) == false) { MainChat.SendErrorChat(player, CONSTANT.ERR_PD_NeedDuty); return; }
            OtherSystem.Animations.PlayerAnimation(player, new string[] { "radio2" });

            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(player.factionId);
            string rank = Factions.Faction.GetFactionRankString(player.factionRank, fact);

            string message = "<i class='fad fa-walkie-talkie'></i>[" + fact.name + "] " + rank + " " + player.fakeName.Replace("_", " ") + ": " + string.Join(" ", messages);

            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (t.factionId == player.factionId /*&& t.HasData(EntityData.PlayerEntityData.NewsDuty)*/) { MainChat.PDRadioChat(t, message); }
            }



            if (!player.Exists)
                return;
            await Task.Delay(2000);
            if (!player.Exists)
                return;
            OtherSystem.Animations.PlayerStopAnimation(player);

            return;
        }


        public class Camera
        {
            public int ownerID { get; set; } = 0;
            public Position camPos { get; set; } = new Position(0, 0, 0);
            public Position lookPosition { get; set; } = new Position(0, 0, 0);
            public int dimension { get; set; } = 0;
            public string cameraLink { get; set; } = "YOK";
            public int TextlabelID { get; set; } = 0;
            public int ObjectID { get; set; } = 0;
            public List<Client> clients { get; set; } = new List<Client>();

            public class Client
            {
                public int clientID { get; set; } = 0;
            }
        }

        public static List<Camera> serverCameras = new List<Camera>();


        [Command("cameraobj")]
        public async Task COM_CreateCamera(PlayerModel p)
        {
            if (await CheckPlayerInNews(p) == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            Camera checkCam = serverCameras.Find(x => x.ownerID == p.sqlID);
            if (checkCam != null) { MainChat.SendErrorChat(p, "[错误] 您已架设了直播设备, 请先关闭直播设备."); return; }

            GlobalEvents.ShowObjectPlacement(p, "prop_film_cam_01", "News:CrateCamera");
        }

        [AsyncClientEvent("News:CrateCamera")]
        public void Event_CreateCamera(PlayerModel p, string rot, string pos, string model)
        {
            Vector3 position = JsonConvert.DeserializeObject<Vector3>(pos);
            Vector3 rotation = JsonConvert.DeserializeObject<Vector3>(rot);

            Camera checkCam = new Camera()
            {
                ownerID = p.sqlID,
                camPos = p.Position,
                lookPosition = p.Position,
                dimension = p.Dimension,
                ObjectID = (int)PropStreamer.Create(model, position, rotation, dimension: p.Dimension, frozen: true).Id,
                TextlabelID = (int)TextLabelStreamer.Create("~b~[直播设备]~n~~y~观众: " + 0, position, dimension: p.Dimension, font: 0).Id,
            };

            serverCameras.Add(checkCam);
            MainChat.SendInfoChat(p, "[新闻] 成功设置直播设备.");
            return;
        }

        [Command("delcameraobj")]
        public async Task COM_DestroyCamera(PlayerModel p)
        {
            if (await CheckPlayerInNews(p) == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            Camera checkCam = serverCameras.Find(x => x.ownerID == p.sqlID);
            if (checkCam == null) { MainChat.SendErrorChat(p, "[错误] 无效直播设备."); return; }

            PropStreamer.GetProp((ulong)checkCam.ObjectID).Delete();
            TextLabelStreamer.GetDynamicTextLabel((ulong)checkCam.TextlabelID).Delete();

            foreach (Camera.Client t in checkCam.clients)
            {
                PlayerModel target = GlobalEvents.GetPlayerFromSqlID(t.clientID);
                if (target.HasData("News:Watching"))
                {
                    Position oldPos = target.lscGetdata<Position>("News:Watching");
                    target.Position = oldPos;
                    target.Dimension = p.lscGetdata<int>("News:OldDimension");

                    target.DeleteData("News:Watching");
                    target.DeleteData("News:OldDimension");

                    GlobalEvents.CloseCamera(target);
                    target.EmitLocked("NewsBackgroundClose");
                    GlobalEvents.FreezeEntity(target, false);

                    MainChat.SendInfoChat(target, "[新闻] 直播已结束.");
                }


            }

            serverCameras.Remove(checkCam);
            MainChat.SendInfoChat(p, "[新闻] 成功移除直播设备.");
            p.EmitLocked("NewsBackgroundClose");
            return;
        }

        public static void UpdateCamera(int LabelID, int totalClients)
        {
            TextLabelStreamer.GetDynamicTextLabel((ulong)LabelID).Text = "~b~[直播设备]~n~~y~观众: " + totalClients.ToString();
        }

        [Command("joinbc")]
        public async void COM_JoinBroadCast(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /joinbc [直播 ID]"); return; }

            if (p.HasData("News:Watching"))
            {
                MainChat.SendErrorChat(p, "[错误] 您已在直播间中, 请先退出.");
                return;
            }

            int bID; bool bIDisOk = Int32.TryParse(args[0], out bID);
            if (!bIDisOk) { MainChat.SendInfoChat(p, "[用法] /joinbc [直播 ID]"); return; }

            Camera checkCam = serverCameras.Find(x => x.ownerID == bID);
            if (checkCam == null) { MainChat.SendErrorChat(p, "[错误] 无效直播设备."); return; }

            Camera.Client nClient = new Camera.Client()
            {
                clientID = p.sqlID
            };

            checkCam.clients.Add(nClient);

            UpdateCamera(checkCam.TextlabelID, checkCam.clients.Count);

            p.lscSetData("News:Watching", p.Position);
            p.lscSetData("News:OldDimension", p.Dimension);

            p.Dimension = checkCam.dimension;
            p.Position = new Position(-245, -2009, 24);

            GlobalEvents.CreateCamera(p, checkCam.camPos, new Rotation(5, 5, 5), 40);
            await Task.Delay(300);
            if (!p.Exists)
                return;

            if (checkCam.cameraLink.Length > 5)
            {
                p.EmitLocked("NewsBackground", checkCam.cameraLink);
            }

            GlobalEvents.LookCamera(p, checkCam.lookPosition);
            GlobalEvents.FreezeEntity(p, true);
            p.EmitLocked("Controls:Disable");
            MainChat.SendInfoChat(p, "[新闻] 您已开启直播.");
            return;
        }

        [Command("quitbc")]
        [Obsolete]
        public static void COM_LeaveBroadCast(PlayerModel p)
        {
            if (!p.HasData("News:Watching")) { MainChat.SendErrorChat(p, "[错误] 您没有在观看直播."); return; }

            Position oldPos = p.lscGetdata<Position>("News:Watching");
            p.Position = oldPos;
            p.Dimension = p.lscGetdata<int>("News:OldDimension");

            p.DeleteData("News:Watching");
            p.DeleteData("News:OldDimension");

            Camera checkCam = serverCameras.Find(x => x.clients.Find(z => z.clientID == p.sqlID) != null);
            if (checkCam == null)
                return;

            Camera.Client Client = checkCam.clients.Find(x => x.clientID == p.sqlID);
            checkCam.clients.Remove(Client);

            GlobalEvents.CloseCamera(p);

            UpdateCamera(checkCam.TextlabelID, checkCam.clients.Count);

            GlobalEvents.FreezeEntity(p, false);
            p.EmitAsync("Controls:Disable:Close");

            MainChat.SendInfoChat(p, "[新闻] 您已退出观看直播.");

            p.EmitLocked("NewsBackgroundClose");

            return;
        }

        [Command("editcamera")]
        public static void COM_EditCamera(PlayerModel p)
        {

            Camera cam = serverCameras.Find(x => x.ownerID == p.sqlID);

            if (cam == null) { MainChat.SendErrorChat(p, "[错误] 无效直播设备."); return; }

            cam.lookPosition = p.Position;
            MainChat.SendInfoChat(p, "[新闻] 成功调整直播设备视角.");
            return;
        }

        [Command("editcamerabg")]
        [Obsolete]
        public async void COM_CameraBackground(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /editcamerabg [图片链接]"); return; }
            Camera cam = serverCameras.Find(x => x.ownerID == p.sqlID);
            if (cam == null) { MainChat.SendErrorChat(p, "[错误] 无效直播设备."); return; }

            foreach (Camera.Client cl in cam.clients)
            {
                PlayerModel camTarget = GlobalEvents.GetPlayerFromSqlID(cl.clientID);
                if (camTarget == null)
                    continue;

                await camTarget.EmitAsync("newBackground", args[0]);
            }


            p.EmitLocked("NewsBackgroundClose");
            cam.cameraLink = args[0];
            await Task.Delay(300);
            if (!p.Exists)
                return;
            MainChat.SendInfoChat(p, "[新闻] 成功调整直播背景!");
            p.EmitLocked("NewsBackground", args[0]);

            return;
        }

    }
}
