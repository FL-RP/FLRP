using AltV.Net.Async;
using AltV.Net.Data;
using outRp.Models;
using Newtonsoft.Json;

namespace outRp.OtherSystem.Textlabels
{
    public class Bar
    {
        public class options
        {
            public string text { get; set; }
            public int checkpoints { get; set; }
            public float progress { get; set; }
        }

        /// <summary>
        /// </summary>
        /// <param name="p"></param>
        /// <param name="name">id yerine geçecek isim</param>
        /// <param name="title"></param>
        /// <param name="rightText"></param>
        public static void CreateTextBar(PlayerModel p, string name, string title, string rightText = null)
        {
            if(rightText != null)
            {
                options x = new options();
                x.text = rightText;
                string json = JsonConvert.SerializeObject(x);
                p.EmitLocked("timerbars:create", name, "text", title, json);
                return;
            }
            p.EmitLocked("timerbars:create", name, "text", title, "");
        }
        public static void CreatePlayerBar(PlayerModel p, string name, string title, string rightText = null)
        {
            if (rightText != null)
            {
                options options = new options();
                options.text = rightText;
                string json = JsonConvert.SerializeObject(options);
                p.EmitLocked("timerbars:create", name, "player", title, json);
                return;
            }
            p.EmitLocked("timerbars:create", name, "player", title, "");
        }
        public static void CreateCheckpointBar(PlayerModel p, string name, string title, int checkPoints = 1)
        {
            options options = new options();
            options.checkpoints = checkPoints;
            string json = JsonConvert.SerializeObject(options);
            //Alt.Log(json);
            p.EmitLocked("timerbars:create", name, "checkpoint", title, json);
        }
        public static void CreateProgressBar(PlayerModel p, string name, string title, float progress = 0)
        {
            options options = new options();
            options.progress = progress;
            string json = JsonConvert.SerializeObject(options);            
            p.EmitLocked("timerbars:create", name, "progress", title, json);
        }

        public static void CreateCooldownBar(PlayerModel p, string name, string title, int time, string trigger = "", string value = "", bool removeAfter = true )
        {
            p.EmitLocked("timerbars:create", name, "text", title, "");
            p.EmitLocked("timerbars:cooldown", name, time, trigger, value, removeAfter);
        }
        public static void CreateUpdownBar(PlayerModel p, string name, string title, string rightText = null)
        {
            options x = new options();
            x.text = rightText;
            string json = JsonConvert.SerializeObject(x);
            p.EmitLocked("timerbars:create", name, "text", title, json);
        }
        public static void RemoveBar(PlayerModel p, string name)
        {
            p.EmitLocked("timerbars:remove", name);
        }
        public static void RemoveAllBars(PlayerModel p)
        {
            p.EmitLocked("timerbars:removeAll");
        }
        public static void BarSetTitle(PlayerModel p, string name, string title)
        {
            p.EmitLocked("timerbars:setTitle", name, title);
        }
        public static void BarSetText(PlayerModel p, string name, string text)
        {
            p.EmitLocked("timerbars:setText", name, text);
        }
        public static void BarSetColor(PlayerModel p, string name, Rgba renk)
        {
            p.EmitLocked("timerbars:setColor", name, renk);
        }
        public static void BarSetTextColor(PlayerModel p, string name, Rgba renk)
        {
            p.EmitLocked("timerbars:setTextColor", name, renk);
        }
        public static void BarSetHightLightColor(PlayerModel p, string name, Rgba renk)
        {
            p.EmitLocked("timerbars:setHighlightColor", name, renk);
        }
        public static void BarSetCheckpointState(PlayerModel p, string name, int pNo, bool state)
        {
            p.EmitLocked("timerbars:setCheckpointState", name, pNo, state);
        }
        public static void BarSetProgress(PlayerModel p, string name, float progress)
        {
            p.EmitLocked("timerbars:setProgress", name, progress);
        }
        public static void ClearTimerBar(PlayerModel p, string name)
        {
            p.EmitLocked("timerbars:cooldownStop", name);
        }

    }
}
