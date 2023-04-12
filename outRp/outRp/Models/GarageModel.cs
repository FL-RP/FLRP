using System;
using System.Collections.Generic;
using AltV.Net; using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using outRp.Chat;
using outRp.Globals;
using outRp.OtherSystem.Textlabels;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Numerics;
using AltV.Net.Enums;
using AltV.Net.Resources.Chat.Api;

namespace outRp.Models
{
    public class GarageModel :IScript
    {
        public class Garage
        {
            public int ID { get; set; } = 0;
            public int type { get; set; } = 0; // Tipler -> 0: kişi | 1: işyeri | 2: ev | 3: faction
            public int ownerID { get; set; } = 0;
            public Position pos { get; set; } = new Position(0, 0, 0);
            public Position intPos { get; set; } = new Position(0, 0, 0);
            public bool isLocked { get; set; } = true;
            public int Dimension { get; set; } = 0;

            public Settings settings { get; set; } = new Settings();

            public class Settings
            {
                public int MarkerType { get; set; } = 1;
                public Rgba MarkerColor { get; set; } = new Rgba(255, 0, 0, 255);
                public Vector3 markerScale { get; set; } = new Vector3(0, 0, 0);

                public string businessInterior { get; set; } = "vw_casino_main﻿";
                public List<interiorSettings> businessInteriorSettings { get; set; } = new List<interiorSettings>();

                public class interiorSettings
                {
                    public string entitySet { get; set; } = "Int01_ba_Style02";
                }
            }

            public void Update(Marker m, PlayerLabel l) => update(this, m, l);
            public Task<int> Create() => create(this);
            public void Delete(Marker m, PlayerLabel l) => delete(this, m, l);
        }

        public static async Task<int> create(Garage x)
        {
            x.ID = await Database.DatabaseMain.CreateGarage(x);
            Marker y = MarkerStreamer.Create((MarkerTypes)x.settings.MarkerType, x.pos, x.settings.markerScale, color: x.settings.MarkerColor, streamRange: 10, faceCamera: true);
            y.SetData("GarageID", x.ID);

            PlayerLabel l = TextLabelStreamer.Create("~b~[~w~车库出口~b~]", x.intPos, dimension: x.Dimension, streamRange: 10, font: 0);
            l.SetData(EntityData.EntranceTypes.EntranceType, EntityData.EntranceTypes.ExitWorld);
            l.SetData(EntityData.EntranceTypes.ExitWorldPosition, x.pos);
            y.SetData("GarageTextID", l.Id);
            
            return x.ID;
        }

        public static void update(Garage g, Marker m, PlayerLabel l)
        {
            m.MarkerType = (MarkerTypes)g.settings.MarkerType;
            m.Position = g.pos;
            m.Scale = g.settings.markerScale;
            m.Color = g.settings.MarkerColor;
            m.FaceCamera = true;
            m.isBusinessMarker = true;
            m.DisplayName = "车库编号: " + g.ID.ToString();

            l.Position = g.intPos;
            l.Dimension = g.Dimension;
            l.SetData(EntityData.EntranceTypes.ExitWorldPosition, g.pos);
            Database.DatabaseMain.UpdateGarage(g);
        }

        public static void delete(Garage g, Marker m, PlayerLabel l)
        {
            m.Destroy(); l.Delete();
            Database.DatabaseMain.DeleteGarage(g);
        }

        // Sistem içeriği aşağıda

        public static async Task LoadServerGarages()
        {

            List<Garage> gList = await Database.DatabaseMain.GetAllServerGarages();
            foreach(Garage x in gList)
            {
                Marker y = MarkerStreamer.Create((MarkerTypes)x.settings.MarkerType, x.pos, x.settings.markerScale, color: x.settings.MarkerColor, streamRange: 10, faceCamera: true);
                y.isBusinessMarker = true;
                y.DisplayName = "车库编号: " + x.ID.ToString();
                y.SetData("GarageID", x.ID);

                PlayerLabel l = TextLabelStreamer.Create("~b~[~w~车库出口~b~]", x.intPos, dimension: x.Dimension, streamRange: 10, font: 0);
                l.SetData(EntityData.EntranceTypes.EntranceType, EntityData.EntranceTypes.ExitWorld);
                l.SetData(EntityData.EntranceTypes.ExitWorldPosition, x.pos);
                y.SetData("GarageTextID", l.Id);
            }

            Alt.Log("加载 车库: " + gList.Count);
        }

        public static async Task<(Garage, Marker, PlayerLabel)> GetNearGarage(PlayerModel p)
        {
            Marker x = MarkerStreamer.GetAllDynamicMarkers().Find(x => p.Position.Distance(x.Position) < 4 && x.Dimension == p.Dimension);
            if(x == null)
               return (null, null, null);

            if(!x.TryGetData("GarageID", out int ID)) { return (null, null, null); }

            Garage g = await Database.DatabaseMain.getGarageByID(ID);
            if(g == null) { return (null, null, null); }

            if(!x.TryGetData("GarageTextID", out ulong textID)) { return (null, null, null); }

            PlayerLabel l = TextLabelStreamer.GetDynamicTextLabel(textID);
            if(l == null) { return (null, null, null); }

            return (g, x, l);
        }

        public static async Task<(Garage, Marker, PlayerLabel)> getGarageByID(int ID)
        {
            foreach(Marker x in MarkerStreamer.GetAllDynamicMarkers())
            {
                x.TryGetData("GarageID", out int GID);
                if(GID == ID)
                {
                    x.TryGetData("GarageTextID", out ulong textID);
                    PlayerLabel l = TextLabelStreamer.GetDynamicTextLabel(textID);
                    Garage g = await Database.DatabaseMain.getGarageByID(GID);
                    return (g,x,l);
                }
            }
            return (null, null, null);
        }

        public static async Task<bool> GetGarageKeyQuery(PlayerModel p, Garage g)
        {
            // Tipler -> 0: kişi | 1: işyeri | 2: ev | 3: faction
            if (p.adminWork) { return true; }
            switch (g.type)
            {
                case 0:
                    if (g.ownerID == p.sqlID)
                        return true;
                    break;
                case 1:
                    (BusinessModel, Marker, PlayerLabel) biz = await Props.Business.getBusinessById(g.ownerID);
                    if (biz.Item1 == null)
                        return false;

                    return await Props.Business.CheckBusinessKey(p, biz.Item1);

                case 2:
                    (HouseModel, PlayerLabel, Marker) h = await Props.Houses.getHouseById(g.ownerID);
                    if (h.Item1 == null)
                        return false;
                    
                    bool houseUse = false;
                    houseUse = await Props.Houses.HouseKeysQuery(p, h.Item1);
                    return houseUse;

                case 3:
                    if (p.factionId == g.ownerID)
                        return true;
                    return false;

                default:
                    return false;
            }
            return false;

        }

        [Command("glock")]
        public static async Task COM_GLock(PlayerModel p)
        {    
            (Garage, Marker, PlayerLabel) g = await GetNearGarage(p);
            if(g.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 附近没有车库!"); return; }
            if(!await GetGarageKeyQuery(p, g.Item1)) { MainChat.SendErrorChat(p, "[错误] 您没有此车库的钥匙."); return; }

            g.Item1.isLocked = !g.Item1.isLocked;
            g.Item1.Update(g.Item2, g.Item3);
            string text = (g.Item1.isLocked) ? "上锁" : "解锁" ;

            MainChat.EmoteMe(p, " 从口袋拿出一个钥匙并使车库" + text);
            return;            
        }

        public static async Task<bool> enterGarage(PlayerModel p)
        {
            (Garage, Marker, PlayerLabel) g = await GetNearGarage(p);
            if(g.Item1 == null) { return false; }

            if (g.Item1.isLocked) { MainChat.SendErrorChat(p, "[!] 此车库是锁的."); return false; }
            p.EmitLocked("InteriorManager:LoadInterior", JsonConvert.SerializeObject(g.Item1.settings), g.Item1.intPos.X, g.Item1.intPos.Y, g.Item1.intPos.Z);
            p.SetWeather(WeatherType.Clear);
            if (p.Vehicle != null)
            {

                p.Vehicle.Position = g.Item1.intPos;
                p.Dimension = g.Item1.Dimension;
                p.Vehicle.Dimension = g.Item1.Dimension;
                await p.Vehicle.SetRotationAsync(new Rotation(0, 0, 0));
                p.Vehicle.NetworkOwner.EmitLocked("Vehicle:FreezePos", p.Vehicle, g.Item1.intPos.X, g.Item1.intPos.Y, g.Item1.intPos.Z);   

                return true;
            }
            else
            {
                //VehModel v = (VehModel)p.Vehicle;
                //if(v.Exists)
                //    GlobalEvents.ForceLeaveVehicle(p);
                p.Dimension = g.Item1.Dimension;
                p.Position = g.Item1.intPos;
                //if(v.Exists)
                //    GlobalEvents.ForceEnterVehicle(p, v);
                return true;
            }

        }

        [Command("aeditgarage")]
        public async Task COM_AEditGarage(PlayerModel p, params string[] args)
        {
            if(p.adminLevel <= 3) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if(args.Length <= 0) { MainChat.SendErrorChat(p, "[用法] /aeditgarage [id] [选项] [数值]"); return; }

            if(!Int32.TryParse(args[0], out int ID)) { MainChat.SendErrorChat(p, "[用法] /aeditgarage [id] [选项] [数值]"); return; }

            (Garage, Marker, PlayerLabel) g = await getGarageByID(ID);
            if(g.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效车库!"); return; }

            switch (args[1])
            {
                case "pos":
                    g.Item1.pos = p.Position;
                    g.Item1.Update(g.Item2, g.Item3);
                    break;

                case "interior":
                    g.Item1.intPos = p.Position;
                    g.Item1.Update(g.Item2, g.Item3);
                    break;

                case "type":
                    if(!Int32.TryParse(args[2], out int newType)) { MainChat.SendErrorChat(p, "[用法] /aeditgarage [id] [选项] [数值]"); return; }
                    g.Item1.type = newType;
                    g.Item1.Update(g.Item2, g.Item3);
                    break;

                case "owner":
                    if (!Int32.TryParse(args[2], out int newOwner)) { MainChat.SendErrorChat(p, "[用法] /aeditgarage [id] [选项] [数值]"); return; }
                    g.Item1.ownerID = newOwner;
                    g.Item1.Update(g.Item2, g.Item3);
                    break;

                case "lock":
                    g.Item1.isLocked = !g.Item1.isLocked;
                    g.Item1.Update(g.Item2, g.Item3);
                    break;

                case "vw":
                    if (!Int32.TryParse(args[2], out int newDime))
                        return;

                    g.Item1.Dimension = newDime;
                    g.Item1.Update(g.Item2, g.Item3);
                    return;

                case "markertype":
                    if (!Int32.TryParse(args[2], out int newMarkerType))
                        return;

                    if (newMarkerType > 43)
                        return;

                    g.Item1.settings.MarkerType = newMarkerType;
                    g.Item1.Update(g.Item2, g.Item3);
                    return;

                case "markerscale":
                    if (args.Length < 4)
                        return;

                    if (!float.TryParse(args[2], out float ns1) || !float.TryParse(args[3], out float ns2) || !float.TryParse(args[4], out float ns3))
                        return;

                    g.Item1.settings.markerScale = new Vector3(ns1, ns2, ns3);
                    g.Item1.Update(g.Item2, g.Item3);
                    return;

                case "markercolor":
                    if (args.Length < 5)
                        return;

                    if (!Int32.TryParse(args[2], out int nc1) || !Int32.TryParse(args[3], out int nc2) || !Int32.TryParse(args[4], out int nc3) || !Int32.TryParse(args[5], out int nc4))
                        return;

                    g.Item1.settings.MarkerColor = new Rgba((byte)nc1, (byte)nc2, (byte)nc3, (byte)nc4);
                    g.Item1.Update(g.Item2, g.Item3);
                    return;

                case "interiorset":
                    if (args[2] == null)
                        return;

                    g.Item1.settings.businessInterior = args[2];
                    g.Item1.Update(g.Item2, g.Item3);
                    MainChat.SendInfoChat(p, "[!] 已更新车库内饰.");
                    return;

                case "interiorplus":
                    if (args[2] == null)
                        return;

                    Garage.Settings.interiorSettings newInteriorPlus = new Garage.Settings.interiorSettings() { entitySet = args[2] };
                    g.Item1.settings.businessInteriorSettings.Add(newInteriorPlus);
                    g.Item1.Update(g.Item2, g.Item3);

                    MainChat.SendInfoChat(p, "[!] 已添加新的内饰至车库.");
                    return;

                case "interiorplusclear":
                    g.Item1.settings.businessInteriorSettings.Clear();
                    g.Item1.Update(g.Item2, g.Item3);

                    MainChat.SendInfoChat(p, "[!] 已清理车库已添加内饰.");
                    return;

                case "intset":
                    if (args.Length < 4)
                        return;

                    if (!float.TryParse(args[2], out float np1) || !float.TryParse(args[3], out float np2) || !float.TryParse(args[4], out float np3))
                        return;

                    g.Item1.intPos = new Position(np1, np2, np3);
                    g.Item1.Update(g.Item2, g.Item3);
                    MainChat.SendInfoChat(p, "[!] 车库内饰坐标更新为: " + np1.ToString() + "-" + np2.ToString() + "-" + np3.ToString());
                    return;

            }

            MainChat.SendInfoChat(p, "[?] 已更新车库信息.");
            return;
        }

        [Command("addgarage")]
        public async Task COM_CreateGarage(PlayerModel p)
        {
            if(p.adminLevel < 3) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            Garage g = new Garage();
            g.pos = p.Position;
            g.ID = await g.Create();
            MainChat.SendErrorChat(p, "[?] 成功添加车库: " + g.ID);
            return;            
        }
    }
}
