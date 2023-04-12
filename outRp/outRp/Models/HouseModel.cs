using AltV.Net.Data;
using outRp.Globals;
using outRp.OtherSystem.Textlabels;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace outRp.Models
{
    public class HouseModel
    {
        public int ID { get; set; }
        public string name { get; set; } = "无";
        public int price { get; set; } = 50000;
        public int ownerId { get; set; } = 0;
        public Position pos { get; set; }
        public Position intPos { get; set; } = new Position(0, 0, 0);
        public bool isRentable { get; set; } = false;
        public int rentPrice { get; set; } = 0;
        public int rentOwner { get; set; } = 0;
        public int dimension { get; set; } = 0;
        public bool isLocked { get; set; } = true;
        public string houseEnv { get; set; } = "[]";
        public Settings settings { get; set; } = new Settings();

        public Task<int> Create() => CreateHouse(this);
        public void Update(Marker x, PlayerLabel i) => UpdateMarker(x, this, i);
        public void Delete() => Database.DatabaseMain.DeleteHouse(this.ID);

        public class Settings
        {
            public int MarkerType { get; set; } = 1;
            public Rgba MarkerColor { get; set; } = new Rgba(255, 0, 0, 50);
            public Vector3 markerScale { get; set; } = new Vector3(0, 0, 0);

            public string interiorMusic { get; set; } = null;

            public string houseInterior { get; set; } = "vw_casino_main﻿";
            public List<interiorSettings> houseInteriorSettings { get; set; } = new List<interiorSettings>();

            public string musicUrl { get; set; } = "none";
            public DateTime musicStartTime { get; set; } = DateTime.Now;
            public double musicSound { get; set; } = 1.0;

            public class interiorSettings
            {
                public string entitySet { get; set; } = "Int01_ba_Style02";
            }

            public int SecurityLevel { get; set; } = 0;

            public TVModel tv { get; set; } = new TVModel();
            public int Tax { get; set; } = 0;
            public int TotalTax { get; set; } = 0;
            public bool Light { get; set; } = true;

        }

        public class TVModel
        {
            public bool hasTv { get; set; } = false;
            public ulong TvPropID { get; set; }
            public Position Position { get; set; }
            public Rotation Rotation { get; set; }
            public int Volume { get; set; } = 100;
            public string URL { get; set; } = "无";
            public DateTime StartTime { get; set; } = DateTime.Now;

        }


        public static void UpdateMarker(Marker x, HouseModel h, PlayerLabel i)
        {
            if (h.settings.TotalTax > (h.price / 10))
            {
                h.isLocked = true;
                h.rentOwner = 0;
            }

            x.Position = h.pos;
            x.DisplayName = Props.Houses.setupHouseName(h);
            x.MarkerType = (MarkerTypes)h.settings.MarkerType;
            x.Scale = h.settings.markerScale;
            x.Color = h.settings.MarkerColor;
            x.isBusinessMarker = true;
            i.Dimension = h.dimension;
            i.Position = h.intPos;
            i.Text = "按 [~g~E键~w~] 离开";
            i.Font = 0;
            i.Scale = 0.4f;
            Database.DatabaseMain.UpdateHouse(h);
            return;
        }

        public static async Task<int> CreateHouse(HouseModel h)
        {
            h.ID = await Database.DatabaseMain.CreateHouse(h);
            Marker hM = MarkerStreamer.Create((MarkerTypes)h.settings.MarkerType, h.pos, h.settings.markerScale, streamRange: 10);
            hM.DisplayName = Props.Houses.setupHouseName(h);
            hM.isBusinessMarker = true;
            hM.Color = new Rgba(250, 0, 0, 250);
            hM.SetData(EntityData.GeneralSetting.DataType, EntityData.GeneralSetting.TypeHouse);
            hM.SetData(EntityData.houseData.HouseID, h.ID);

            PlayerLabel labelExit = TextLabelStreamer.Create("按 [~g~E键~w~] 离开", h.intPos, h.dimension, true, new Rgba(255, 255, 255, 255), streamRange: 2);
            labelExit.SetData(EntityData.EntranceTypes.EntranceType, EntityData.EntranceTypes.ExitWorld);
            labelExit.SetData(EntityData.EntranceTypes.ExitWorldPosition, h.pos);
            hM.SetData(EntityData.houseData.HouseExitWorldLbl, labelExit.Id); // 
            return h.ID;
        }


    }
}
