using AltV.Net;
using AltV.Net.Data;
using outRp.Chat;
using outRp.Globals;
using outRp.Models;
using outRp.OtherSystem.Textlabels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;

namespace outRp.OtherSystem.LSCsystems
{
    public class InformationMarkers : IScript
    {
        public class informationMarker
        {
            public ulong ID { get; set; }
            public Position Position { get; set; }
            public int Dimension { get; set; }
            public string Header { get; set; }
            public string Text { get; set; }
            public int OwnerID { get; set; }
            public int Time { get; set; } = 1;
        }

        public static List<informationMarker> informationList = new List<informationMarker>();

        public static async Task<informationMarker> Create(int owner, Position pos,string header, string text, int dimension = 0, int time = 1)
        {
            string infoText = "";
            PlayerModelInfo p = await Database.DatabaseMain.getCharacterInfo(owner);
            if (p == null)
                return null;

            infoText = "~w~" + p.characterName.Replace("_", " ") + "~n~~p~" + header + "~n~指令: ~g~/im";

            Marker x = MarkerStreamer.Create(MarkerTypes.MarkerTypeQuestion, pos, new System.Numerics.Vector3(0.5f, 0.5f, 0.5f), new System.Numerics.Vector3(0, 0, 0), dimension: dimension, faceCamera: true, rotate: true, streamRange: 10);
            x.Color = new Rgba(226, 232, 140, 200);
            x.DisplayName = "~b~[" + x.Id + "]" + infoText;
            informationMarker i = new informationMarker()
            {
                ID = x.Id,
                Dimension = dimension,
                OwnerID = owner,
                Position = pos,
                Header = header,
                Text = text,
                Time = time
            };

            informationList.Add(i);
            return i;
        }

        public static informationMarker GetNearFromPos(Position pos, int Dimension = 0)
        {
            /*informationMarker result = null;
            foreach (informationMarker i in informationList)
            {
                if (i.Position.Distance(pos) < 2) { result = i; break; }
            }

            return result;*/
            return informationList.Where(x => x.Position.Distance(pos) < 2 && x.Dimension == Dimension).OrderBy(x => x.Position.Distance(pos)).FirstOrDefault();
        }

        public static informationMarker GetByID(ulong ID)
        {
            informationMarker result = null;
            foreach (informationMarker i in informationList)
            {
                if (i.ID == ID) { result = i; break; }
            }
            return result;
        }

        public static void Delete(informationMarker i)
        {
            MarkerStreamer.Delete(i.ID);
            informationList.Remove(i);
            return;
        }

        public static void DeleteAll()
        {
            foreach (informationMarker i in informationList)
            {
                MarkerStreamer.Delete(i.ID);
            }
            informationList = new List<informationMarker>();
            return;
        }

        // Evet Watchers
        public static void CheckAllInformations()
        {
            try
            {
                lock (informationList)
                {
                    List<int> removeIndexs = new List<int>();
                    for (int i = 0; i < informationList.Count; i++)
                    {
                        informationMarker x = informationList[i];
                        x.Time -= 1;
                        if (x.Time <= 0) { removeIndexs.Add(i); }
                    }

                    foreach (int a in removeIndexs)
                    {
                        if (informationList[a] != null)
                        {
                            MarkerStreamer.Delete(informationList[a].ID);
                            informationList.RemoveAt(a);
                        }
                    }
                }
            }
            catch { return;  }
            return;
        }


        [Command("addinfomarker", aliases: new string[] { "aim" })]
        public static void COM_AddInformationMarker(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /addinfomarker [时间(分钟)] [标题(如果有空格，使用_)] [文本]"); return; }
            informationMarker check = informationList.Find(x => x.OwnerID == p.sqlID);
            if (check != null) { MainChat.SendErrorChat(p, "[错误] 您已有创建的IC信息了, 请先删除(/dim)."); return; }
            if (!Int32.TryParse(args[0], out int time)) { MainChat.SendErrorChat(p, "[错误] 无效时间."); return; }

            InformationMarkers.Create(p.sqlID, p.Position, args[1].Replace('_', ' '), string.Join(" ", args[2..]), p.Dimension, time);
            MainChat.SendInfoChat(p, "[?] 已创建IC信息至您当前位置.");
            return;
        }

        [Command("deleteinfomarker", aliases: new string[] { "dim" })]
        public static void COM_RemoveInformationMarker(PlayerModel p)
        {
            informationMarker check = informationList.Find(x => x.OwnerID == p.sqlID);
            if (check == null) { MainChat.SendErrorChat(p, "[错误] 无效IC信息!"); return; }
            MarkerStreamer.Delete(check.ID);
            informationList.Remove(check);
            MainChat.SendInfoChat(p, "[!] 已删除指定IC信息.");
        }

        [Command("admindeleteinfomarker", aliases: new string[] { "adim" })]
        public static void COM_RemoveInformationMarkerAdmin(PlayerModel p)
        {
            if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            informationMarker curr = InformationMarkers.GetNearFromPos(p.Position, p.Dimension);
            if(curr == null) { MainChat.SendErrorChat(p, "[错误] 无效IC信息!"); return; }
            MarkerStreamer.Delete(curr.ID);
            informationList.Remove(curr);
            MainChat.SendInfoChat(p, "[!] 已删除指定IC信息.");
            return;
        }

        [Command("ashowim")]
        public static void COM_ShowAllMessages(PlayerModel p)
        {
            if(p.adminLevel <= 4) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            string message = "<center>所有IC信息</center>{FFFFFF}";
            foreach(informationMarker m in informationList)
            {
                message += "<br>时间:" + m.Time.ToString() + "<br>" + m.ID + " - " + m.Header + " | " + m.Text;
            }
            MainChat.SendInfoChat(p, message);
            return;
        }

        [Command("gotoim")]
        public static void COM_GoMessage(PlayerModel p, params string[] args)
        {
            if(p.adminLevel < 4) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            if(args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /gotoim [id]"); return; }
            if(!ulong.TryParse(args[0], out ulong ID)) { MainChat.SendInfoChat(p, "[用法] /gotoim [id]"); return; }
            informationMarker x = informationList.Find(x => x.ID == ID);
            if(x == null) { MainChat.SendErrorChat(p, "[错误] 无效IC信息."); return; }
            p.Position = x.Position;
            return;
        }

        [Command("im")]
        public static void COM_ShowMessage(PlayerModel p)
        {
            informationMarker i = InformationMarkers.GetNearFromPos(p.Position, p.Dimension);
            if(i == null) { MainChat.SendErrorChat(p, "[错误] 附近没有IC信息."); return; }
            p.SendChatMessage("{5fa186}* " + i.Text + " (( 当前位置 ))");
            return;
        }

    }
}
