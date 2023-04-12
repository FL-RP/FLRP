using AltV.Net;
using AltV.Net.Async;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Globals;
using outRp.Models;
using outRp.OtherSystem.Textlabels;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace outRp.OtherSystem.LSCsystems
{
    public class Boombox : IScript
    {
        public class BoomBoxModel
        {
            public int ID { get; set; }
            public LProp boxProp { get; set; }
            public PlayerLabel boxLabel { get; set; }
            public string link { get; set; } = "none";
            public double Volume { get; set; } = 1.0;
        }

        public static List<BoomBoxModel> serverBoomBoxs = new List<BoomBoxModel>();

        public static void CreateBoomBox(PlayerModel p)
        {
            bool canUse = true;
            foreach (var x in serverBoomBoxs)
            {
                if (p.Position.Distance(x.boxProp.Position) < 30) { canUse = false; break; }
                else if (x.ID == p.sqlID) { MainChat.SendErrorChat(p, "[错误] 您已放置了音响, 请先收回."); return; }
            }
            if (!canUse) { MainChat.SendErrorChat(p, "[错误] 周围已经有音响, 请将其删除或尝试将其安装在其他位置."); return; }
            if (p.Dimension >= 1) { MainChat.SendErrorChat(p, "[错误] 无法在室内使用."); return; }
            GlobalEvents.ShowObjectPlacement(p, "prop_portable_hifi_01", "Boombox:Create");
        }

        [AsyncClientEvent("Boombox:Create")]
        public void PlaceBoomBox(PlayerModel p, string rot, string pos, string model)
        {
            BoomBoxModel nB = new BoomBoxModel();
            Vector3 position = JsonConvert.DeserializeObject<Vector3>(pos);
            position.Z += 0.1f;
            Vector3 rotation = JsonConvert.DeserializeObject<Vector3>(rot);
            nB.boxProp = PropStreamer.Create(model, position, rotation, frozen: true, dimension: p.Dimension);
            nB.boxLabel = TextLabelStreamer.Create("[~y~音响~w~]~n~ ~b~/~g~boombox~n~所有者: ~w~" + p.characterName.Replace("_", " "), position, font: 0, streamRange: 5, dimension: p.Dimension);
            nB.ID = p.sqlID;
            serverBoomBoxs.Add(nB);
            Prometheus.Boombox_Usage(1);
            return;
        }

        /* [Command("stopmusic")]
        public static void COM_RadioSound(PlayerModel p)
        {
            p.EmitAsync("Boom:isSound");
            return;
        } */
    }
    public static class DateTimeJavaScript
    {
        private static readonly long DatetimeMinTimeTicks =
           (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;

        public static long ToJavaScriptMilliseconds(this DateTime dt)
        {
            return (long)((dt.ToUniversalTime().Ticks - DatetimeMinTimeTicks) / 10000);
        }
    }
}
