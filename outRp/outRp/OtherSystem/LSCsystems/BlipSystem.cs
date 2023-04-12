using System;
using System.Collections.Generic;
using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Resources.Chat.Api;
using outRp.Models;
using outRp.Chat;
using outRp.Globals;
using outRp.OtherSystem.Textlabels;
using Newtonsoft.Json;

namespace outRp.OtherSystem.LSCsystems
{
    public class BlipSystem : IScript
    {
        public class BlipModel
        {
            public ulong ID { get; set; } = 0;
            public Position pos { get; set; } = new Position(0, 0, 0);
            public uint range { get; set; } = 450;
            public int radius { get; set; } = 0;
            public int width { get; set; } = 0;
            public int height { get; set; } = 0;
            public int blipType { get; set; } = 3;
            public int? sprite { get; set; } = null;
            public int alpha { get; set; } = 250;
            public int? category { get; set; } = 1;
            public int? color { get; set; } = 1;
            public int? heading { get; set; } = 0;
            public string? name { get; set; } = ".";
            public int? number { get; set; } = null;
            public float? scale { get; set; } = 0.5f;
            public int? secondaryColor { get; set; } = 0;
            public bool? shortRange { get; set; } = false;
            public bool? tickVisible { get; set; } = false;

        }

        public static List<BlipModel> serverBlips = new List<BlipModel>();

        public static void LoadAllBlips(string data)
        {
            serverBlips = JsonConvert.DeserializeObject<List<BlipModel>>(data);

            foreach(BlipModel b in serverBlips)
            {
                Textlabels.Blip x = BlipStreamer.Create(b.pos, b.blipType, b.color ?? 1, range: b.range);
                b.ID = x.Id;
                x.width = b.width;
                x.sprite = b.sprite;
                x.category = b.category ?? 3;
                x.heading = b.heading;
                x.Range = b.range;
                x.alpha = b.alpha;
                x.heading = b.heading;
                x.name = b.name;
                x.number = b.number;
                x.scale = b.scale ?? null;
                x.secondaryColor = b.secondaryColor;
                x.shortRange = b.shortRange;
                x.tickVisible = b.tickVisible;
            }
            Alt.Log("加载 标记点系统.");
        }

        public static (BlipModel, Textlabels.Blip) CreateBlip(Position pos, int blipType)
        {
            BlipModel b = new BlipModel();
            b.pos = pos;
            b.blipType = blipType;
            b.range = 4000;
            Textlabels.Blip blip = BlipStreamer.Create(pos, blipType, range: 4000);
            b.ID = blip.Id;
            serverBlips.Add(b);
            return (b, blip);
        }

        public static bool DeleteBlip(ulong ID)
        {
            BlipModel b = serverBlips.Find(x => x.ID == ID);
            if (b == null)
                return false;

            Textlabels.Blip delB = BlipStreamer.GetBlip(ID);
            if (delB == null)
                return false;

            delB.Destroy();
            serverBlips.Remove(b);
            return true;
        }

        [Command("addblip")]
        public static void COM_AddBlip(PlayerModel p, params string[] args)
        {
            if(p.adminLevel < 4) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if(args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /addblip [标记点类型]"); return; }
            if (!Int32.TryParse(args[0], out int BlipType)) { MainChat.SendInfoChat(p, "[用法] /addblip [标记点类型]"); return; }

            (BlipModel, Textlabels.Blip) blip = CreateBlip(p.Position, BlipType);
            MainChat.SendInfoChat(p, "[!] 已创建标记点 " + blip.Item2.Id);
            return;
        }

        [Command("editblip")]
        public static void COM_EditBlip(PlayerModel p, params string[] args)
        {
            if(p.adminLevel < 4 ) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; } // usage DESC
            if(args.Length <= 1) {
                MainChat.SendInfoChat(p, "[用法] /editblip [blipId] [选项] [数值]<br>" +
                    "可用选项:<br>" +
                    "range - alpha - radius - width - height - category - color - heading - name - number - secondarycolor - shortrange - sprite - tickvisible - scale" +
                    "<br>未在服务器注册的选项:<br>" +
                    "mcreator - bright - crewindicatorvisible - flashinterval - flashtimer - flashes - flashalternate - friendindicator - friendly - headingindivisible - highdetail - priority - outlineindicatorvisible - " +
                    "pulse - route - routecolor - shrinked - showcone");
                return;
            } // usage DESC
            if(!ulong.TryParse(args[0], out ulong BlipID)) { MainChat.SendErrorChat(p, "[HATA] Blip bulunamadı!"); return; }
            (BlipModel, Textlabels.Blip) blip = (serverBlips.Find(x => x.ID == BlipID), BlipStreamer.GetBlip(BlipID));
            if(blip.Item1 == null || blip.Item2 == null) { MainChat.SendErrorChat(p, "[HATA] Blip bulunamadı!"); return; }

            switch (args[1].ToLower())
            {
                case "range":
                    if (!Int32.TryParse(args[2], out int newRange))
                        return;

                    blip.Item1.range = (uint)newRange;
                    blip.Item2.Range = (uint)newRange;
                    MainChat.SendInfoChat(p, "[!] 已更新标记范围为 " + newRange);
                    return;

                case "alpha":
                    if (!Int32.TryParse(args[2], out int newAlpha))
                        return;

                    blip.Item1.alpha = newAlpha;
                    blip.Item2.alpha = newAlpha;
                    MainChat.SendInfoChat(p, "[!] 已更新标记Alpha为 " + newAlpha);
                    return;
                    /*
                case "bliptype":
                    if (!Int32.TryParse(args[2], out int newBlipType))
                        return;

                    blip.Item1.blipType = newBlipType;
                    blip.Item2.BlipType = newBlipType;
                    MainChat.SendInfoChat(p, "[!] Blip tipi " + newBlipType + " olarak ayarlandı!");
                    return; */

                case "radius":
                    if (!Int32.TryParse(args[2], out int newRadius))
                        return;

                    blip.Item1.radius = newRadius;
                    blip.Item2.radius = newRadius;
                    MainChat.SendInfoChat(p, "[!] 已更新标记radius为 " + newRadius);
                    return;

                case "width":
                    if (!Int32.TryParse(args[2], out int newWidth))
                        return;

                    blip.Item1.width = newWidth;
                    blip.Item2.width = newWidth;
                    MainChat.SendInfoChat(p, "[!] 已更新标记width为 " + newWidth);
                    return;

                case "height":
                    if (!Int32.TryParse(args[2], out int newHeight))
                        return;

                    blip.Item1.height = newHeight;
                    blip.Item2.height = newHeight;
                    MainChat.SendInfoChat(p, "[!] 已更新标记height为 " + newHeight);
                    return;

                case "mcreator":
                    if (!bool.TryParse(args[2], out bool newmcreator))
                        return;
                    blip.Item2.missionCreator = newmcreator;
                    MainChat.SendInfoChat(p, "[!] 已更新标记mcreator为 " + newmcreator.ToString());
                    return;

                case "bright":
                    if (!bool.TryParse(args[2], out bool newbright))
                        return;
                    blip.Item2.bright = newbright;
                    MainChat.SendInfoChat(p, "[!] 已更新标记bright为 " + newbright.ToString());
                    return;

                case "category":
                    if (!Int32.TryParse(args[2], out int category))
                        return;

                    blip.Item1.category = category;
                    blip.Item2.category = category;
                    MainChat.SendInfoChat(p, "[!] 已更新标记category为 " + category);
                    return;

                case "color":
                    if (!Int32.TryParse(args[2], out int color))
                        return;

                    blip.Item1.color = color;
                    blip.Item2.color = color;
                    MainChat.SendInfoChat(p, "[!] 已更新标记颜色为 " + color);
                    return;

                case "crewindicatorvisible":
                    if (!bool.TryParse(args[2], out bool newcrewindicatorvisible))
                        return;
                    blip.Item2.crewIndicatorVisible = newcrewindicatorvisible;
                    MainChat.SendInfoChat(p, "[!] 已更新标记crewindicatorvisible为 " + newcrewindicatorvisible);
                    return;

                case "flashinterval":
                    if (!Int32.TryParse(args[2], out int flashinterval))
                        return;

                    blip.Item2.flashInterval = flashinterval;
                    MainChat.SendInfoChat(p, "[!] 已更新标记flashinterval为 " + flashinterval);
                    return;

                case "flashtimer":
                    if (!Int32.TryParse(args[2], out int flashtimer))
                        return;

                    blip.Item2.flashTimer = flashtimer;
                    MainChat.SendInfoChat(p, "[!] 已更新标记flashtimer为 " + flashtimer);
                    return;

                case "flashes":
                    if (!bool.TryParse(args[2], out bool flashes))
                        return;

                    blip.Item2.flashes = flashes;
                    MainChat.SendInfoChat(p, "[!] 已更新标记flashes为 " + flashes);
                    return;

                case "flashalternate":
                    if (!bool.TryParse(args[2], out bool newflashAlternate))
                        return;

                    blip.Item2.flashesAlternate = newflashAlternate;
                    MainChat.SendInfoChat(p, "[!] 已更新标记flashalternate为 " + newflashAlternate);
                    return;

                case "friendindicator":
                    if (!bool.TryParse(args[2], out bool friendIndicator))
                        return;

                    blip.Item2.friendIndicatorVisible = friendIndicator;
                    MainChat.SendInfoChat(p, "[!] 已更新标记friendindicator为 " + friendIndicator);
                    return;

                case "friendly":
                    if (!bool.TryParse(args[2], out bool friendly))
                        return;

                    blip.Item2.friendly = friendly;
                    MainChat.SendInfoChat(p, "[!] 已更新标记friendly为 " + friendly);
                    return;

                case "heading":
                    if (!Int32.TryParse(args[2], out int heading))
                        return;

                    blip.Item1.heading = heading;
                    blip.Item2.heading = heading;
                    MainChat.SendInfoChat(p, "[!] 已更新标记heading为 " + heading);
                    return;

                case "headingindivisible":
                    if (!bool.TryParse(args[2], out bool headingIndicatorVisible))
                        return;

                    blip.Item2.headingIndicatorVisible = headingIndicatorVisible;
                    MainChat.SendInfoChat(p, "[!] 已更新标记headingindivisible为 " + headingIndicatorVisible);
                    return;

                case "highdetail":
                    if (!bool.TryParse(args[2], out bool highDetail))
                        return;

                    blip.Item2.highDetail = highDetail;
                    MainChat.SendInfoChat(p, "[!] 已更新标记highdetail为 " + highDetail);
                    return;

                case "name":
                    string name = string.Join(" ", args[2..]);
                    blip.Item1.name = name;
                    blip.Item2.name = name;
                    MainChat.SendInfoChat(p, "[!] 已更新标记name为 " + name);
                    return;

                case "number":
                    if (!Int32.TryParse(args[2], out int number))
                        return;

                    blip.Item1.number = number;
                    blip.Item2.number = number;
                    MainChat.SendInfoChat(p, "[!] 已更新标记number为 " + number);
                    return;

                case "outlineindicatorvisible":
                    if (!bool.TryParse(args[2], out bool outlineIndıcatorVisible))
                        return;

                    blip.Item2.outlineIndicatorVisible = outlineIndıcatorVisible;
                    MainChat.SendInfoChat(p, "[!] 已更新标记outlineindicatorvisible为 " + outlineIndıcatorVisible);
                    return;

                case "priority":
                    if (!Int32.TryParse(args[2], out int priorty))
                        return;

                    blip.Item2.priority = priorty;
                    MainChat.SendInfoChat(p, "[!] 已更新标记priority为 " + priorty);
                    return;

                case "pulse":
                    if (!bool.TryParse(args[2], out bool pulse))
                        return;

                    blip.Item2.pulse = pulse;
                    MainChat.SendInfoChat(p, "[!] 已更新标记pulse为 " + pulse);
                    return;

                case "route":
                    if (!bool.TryParse(args[2], out bool route))
                        return;

                    blip.Item2.route = route;
                    MainChat.SendInfoChat(p, "[!] 已更新标记route为 " + route);
                    return;

                case "routecolor":
                    if (!Int32.TryParse(args[2], out int routecolor))
                        return;

                    blip.Item2.routeColor = routecolor;
                    MainChat.SendInfoChat(p, "[!] 已更新标记routecolor为 " + routecolor);
                    return;

                case "secondarycolor":
                    if (!Int32.TryParse(args[2], out int secondaryColor))
                        return;

                    blip.Item1.secondaryColor = secondaryColor;
                    blip.Item2.secondaryColor = secondaryColor;
                    MainChat.SendInfoChat(p, "[!] 已更新标记secondarycolor为 " + secondaryColor);
                    return;

                case "shortrange":
                    if (!bool.TryParse(args[2], out bool shortrange))
                        return;

                    blip.Item1.shortRange = shortrange;
                    blip.Item2.shortRange = shortrange;
                    MainChat.SendInfoChat(p, "[!] 已更新标记shortrange为 " + shortrange);
                    return;

                case "showcone":
                    if (!bool.TryParse(args[2], out bool showcone))
                        return;

                    blip.Item2.showCone = showcone;
                    MainChat.SendInfoChat(p, "[!] 已更新标记showcone为 " + showcone);
                    return;

                case "shrinked":
                    if (!bool.TryParse(args[2], out bool shrinked))
                        return;

                    blip.Item2.shrinked = shrinked;
                    MainChat.SendInfoChat(p, "[!] 已更新标记shrinked为 " + shrinked);
                    return;

                case "sprite":
                    if (!Int32.TryParse(args[2], out int sprite))
                        return;

                    blip.Item1.sprite = sprite;
                    blip.Item2.sprite = sprite;
                    MainChat.SendInfoChat(p, "[!] 已更新标记sprite为 " + sprite);
                    return;

                case "tickvisible":
                    if (!bool.TryParse(args[2], out bool tickvisible))
                        return;

                    blip.Item1.tickVisible = tickvisible;
                    blip.Item2.tickVisible = tickvisible;
                    MainChat.SendInfoChat(p, "[!] 已更新标记tickvisible为 " + tickvisible);

                    return;

                case "scale":
                    if (!float.TryParse(args[2], out float scale))
                        return;

                    blip.Item1.scale = scale;
                    blip.Item2.scale = scale;
                    MainChat.SendInfoChat(p, "[!] 已更新标记scale为 " + scale);
                    return;

                default:
                    MainChat.SendInfoChat(p, "[用法] /editblip [blipId] [选项] [数值]<br>" +
                        "可用选项:<br>" +
                        "range - alpha - radius - width - height - category - color - heading - name - number - secondarycolor - shortrange - sprite - tickvisible - scale" +
                        "<br>未在服务器注册的选项:<br>" +
                        "mcreator - bright - crewindicatorvisible - flashinterval - flashtimer - flashes - flashalternate - friendindicator - friendly - headingindivisible - highdetail - priority - outlineindicatorvisible - " +
                        "pulse - route - routecolor - shrinked - showcone");
                    return;
            }
        }

        [Command("deblip")]
        public static void COM_RemoveBlip(PlayerModel p, params string[] args)
        {
            if(p.adminLevel < 4) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if(args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /deblip [id]"); return; }
            if(!ulong.TryParse(args[0], out ulong BlipID)) { MainChat.SendInfoChat(p, "[用法] /deblip [id]"); return; }

            if (!DeleteBlip(BlipID)) { MainChat.SendErrorChat(p, "[错误] 无效标记点!"); return; }
            MainChat.SendInfoChat(p, "[?] 已删除标记点 " + BlipID);
            return;
        }

        [Command("nearblip")]
        public void COM_ShowNearBlips(PlayerModel p)
        {
            if(p.adminLevel < 3) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            string text = "<center>附近标记点</center>";
            foreach(Textlabels.Blip b in BlipStreamer.GetAllBlips())
            {
                if(p.Position.Distance(b.Position) < 200)
                {
                    text += "<br>ID: " + b.Id + " 名称:" + b.name + " 图标: " + b.sprite;
                }
            }
            MainChat.SendInfoChat(p, text);
            return;
        }
    }
}

/*
 
    public class BlipSystem : IScript
    {
        public static List<GlobalEvents.blipModel> dynamicBlips = new List<GlobalEvents.blipModel>();

        public int blipCounter = 0;
        public static void CreateBlip(PlayerModel p, string label, int sprite, int number = 0)
        {
            GlobalEvents.blipModel nB = new GlobalEvents.blipModel()
            {
                category = 2,
                cone = false,
                blipname = "dynamicBlip" + label,
                color = 1,
                label = label,
                number = number,
                position = p.Position,
                Short = true,
                sprite = sprite,
                
            };
            dynamicBlips.Add(nB);
            foreach(PlayerModel t in Alt.GetAllPlayers())
            {
                GlobalEvents.CreateBlip(t, "dynamicBlip" + label, 2, p.Position, sprite: sprite, label: label);
            }
        }        

        public static void COM_CreateBlip(PlayerModel p, int sprite, int number, string label)
        {        
            CreateBlip(p, label, sprite, number);                     
        }

        public static void COM_RemoveBlip(PlayerModel p, string[] args)
        {
            //TODO desc
            var blip = dynamicBlips.Find(x => x.blipname == "dynamicBlip" + string.Join(" ", args));
            
            foreach(PlayerModel t in Alt.GetAllPlayers())
            {
                GlobalEvents.DestroyBlip(t, "dynamicBlip" + string.Join(" ", args));
            }

            dynamicBlips.Remove(blip);

            MainChat.SendInfoChat(p, "> Blip başarıyla kaldırıldı.");
        }

        public static void COM_ShowNearBlips(PlayerModel p)
        {
            string list = "<center>Yakın Blipler</center>";
            foreach(var b in dynamicBlips)
            {
                if(p.Position.Distance(b.position) < 100)
                {
                    list += "<br>İsim: " + b.label;
                }
            }
            MainChat.SendInfoChat(p, list, true);
        }

        public static void UpdateBlip(GlobalEvents.blipModel blip)
        {
            foreach(PlayerModel p in Alt.GetAllPlayers())
            {
                GlobalEvents.DestroyBlip(p, blip.blipname);
            }

            foreach(PlayerModel t in Alt.GetAllPlayers())
            {
                GlobalEvents.CreateBlip(t, blip.blipname, blip.category, blip.position, sprite: blip.sprite, color: blip.color, label: blip.label, cone: blip.cone, number: blip.number);
            }
        }

        [Command(CONSTANT.COM_EditBlip)]
        public void COM_EditBlip(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < CONSTANT.LVL_AsnwerHelpReq) { MainChat.SendErrorChat(p, CONSTANT.ERR_AdminLevel); return; } // Duzenle 
            string label = string.Join(" ", args[2..]);
            var blip = dynamicBlips.Find(x => x.blipname == "dynamicBlip" + label);
            if(blip == null) { MainChat.SendErrorChat(p, "[HATA] Blip buluanamdı."); return; }

            int value; bool valueOk = Int32.TryParse(args[1], out value);
            if(!valueOk) { MainChat.SendInfoChat(p, CONSTANT.DESC_EditBlip); return; }

            switch (args[0])
            {
                case "sprite":
                    blip.sprite = value;
                    break;

                case "color":
                    blip.color = value;
                    break;

                case "number":
                    blip.number = value;
                    break;
            }

            UpdateBlip(blip);
            MainChat.SendInfoChat(p, "> Blip başarıyla düzenlendi.");
        }

    }
 */