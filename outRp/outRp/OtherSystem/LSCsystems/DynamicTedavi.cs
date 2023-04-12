using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Globals;
using outRp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using AltV.Net.Resources.Chat.Api;

namespace outRp.OtherSystem.LSCsystems
{
    public class DynamicTedavi : IScript
    {
        public class TedaviPoint
        {
            public Position Position { get; set; } = new();
            public int Price { get; set; } = 0;
            public int Dimension { get; set; } = 0;
            public int Faction { get; set; } = 0;
            public ulong lblId { get; set; } = 0;
        }
        public static List<TedaviPoint> points = new();

        public static void Init(string data)
        {
            points = JsonConvert.DeserializeObject<List<TedaviPoint>>(data);

            foreach (var p in points)
            {
                p.lblId = Textlabels.TextLabelStreamer.Create("~g~[治疗点]~w~~n~/heal ~g~$" + p.Price, p.Position, dimension: p.Dimension).Id;
            }
        }

        public static TedaviPoint getNear(Position pos, int dim = 0)
        {
            return points.Where(x => x.Position.Distance(pos) < 5 && x.Dimension == dim).OrderBy(x => x.Position.Distance(pos)).FirstOrDefault();
        }

        [Command(CONSTANT.COM_Heal)]
        public void TedaviYaptir(PlayerModel player)
        {
            var point = getNear(player.Position, player.Dimension);
            if (point == null) { MainChat.SendErrorChat(player, "[错误] 附近没有治疗点!"); return; }
            if (player.cash < point.Price) { MainChat.SendErrorChat(player, "[错误] 您没有足够的钱."); return; }

            player.cash -= point.Price;


            player.injured.Injured = false;
            player.injured.isDead = false;
            player.MaxHealth = 1000;
            player.hp = 1000;
            player.Health = 1000;
            GlobalEvents.UpdateInjured(player);
            player.updateSql();
            player.EmitLocked("Injured:ClearBloods");
            MainChat.SendInfoChat(player, "[?] 您已获得治疗.");
            return;
        }

        [Command("addhealpoint")]
        public void addPoint(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, "[错误] 无权操作."); return; }
            if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /addhealpoint [价格] [属于组织(如果没有就0)]"); return; }
            if (!Int32.TryParse(args[0], out int price) || !Int32.TryParse(args[1], out int faction)) { MainChat.SendInfoChat(p, "[用法] /addhealpoint [价格] [属于组织(如果没有就0)]"); return; }
            TedaviPoint point = new()
            {
                Position = p.Position,
                Price = price,
                Faction = faction,
                Dimension = p.Dimension,
                lblId = 0
            };
            point.lblId = Textlabels.TextLabelStreamer.Create("~g~[治疗点]~w~~n~/heal ~g~$" + point.Price, point.Position, dimension: point.Dimension).Id;
            points.Add(point);
            MainChat.SendInfoChat(p, "[?] 已添加治疗点.");
            return;
        }
        [Command("deletepoint")]
        public void DeletePoint(PlayerModel p)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, "[错误] 无权操作."); return; }
            var point = getNear(p.Position, p.Dimension);
            if (point == null) { MainChat.SendErrorChat(p, "[错误] 附近没有治疗点."); return; }

            var lbl = Textlabels.TextLabelStreamer.GetDynamicTextLabel(point.lblId);
            if (lbl != null)
            {
                lbl.Delete();
            }

            points.Remove(point);

            MainChat.SendInfoChat(p, "[?] 已删除治疗点.");
            return;
        }
    }
}
