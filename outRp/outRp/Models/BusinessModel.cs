using AltV.Net.Data;
using outRp.Globals;
using outRp.OtherSystem.Textlabels;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace outRp.Models
{
    public class BusinessModel
    {
        public int ID { get; set; }
        public string name { get; set; } = "无";
        public int ownerId { get; set; } = -1;
        public int price { get; set; } = 5000;
        public int type { get; set; } = 1;
        public int vault { get; set; } = 0;
        public int entrancePrice { get; set; } = 0;
        public Position position { get; set; }
        public Position interiorPosition { get; set; }
        public int stock { get; set; }
        public int dimension { get; set; }
        public bool isLocked { get; set; } = true;
        public int company { get; set; } = 0;
        public BizSettings settings { get; set; } = new BizSettings();

        public Task<int> Create() => BusinessEvent.createBusiness(this);

        public async Task Update(Marker l, PlayerLabel i) => await BusinessEvent.updateBusiness(this, l, i);
        public async Task Delete(Marker l, PlayerLabel i) => await BusinessEvent.Delete(this, l, i);

        public class BizSettings
        {
            public int MarkerType { get; set; } = 1;
            public Rgba MarkerColor { get; set; } = new Rgba(255, 0, 0, 255);
            public Vector3 markerScale { get; set; } = new Vector3(0, 0, 0);

            public string businessInterior { get; set; } = "vw_casino_main﻿";
            public List<interiorSettings> businessInteriorSettings { get; set; } = new List<interiorSettings>();

            public string musicUrl { get; set; } = "none";
            public DateTime musicStartTime { get; set; } = DateTime.Now;
            public double musicSound { get; set; } = 1.0;

            public PaintBall paintBall { get; set; } = new PaintBall();
            public class interiorSettings
            {
                public string entitySet { get; set; } = "Int01_ba_Style02";
            }

            public int SecurityLevel { get; set; } = 0;

            public int interiorTime { get; set; } = 0;

            public TVModel tv { get; set; } = new TVModel();
            public int Tax { get; set; } = 0;
            public int TotalTax { get; set; } = 0;
            public string Env { get; set; } = "[]";
            public bool gps { get; set; } = false;
            public bool Light { get; set; } = true;
            public List<int> Admins { get; set; } = new List<int>();
            public bool floatText { get; set; } = false;
            //
        }

        // TODO Biz SEttings Class 
        public class PaintBall
        {
            public Position team1_pos { get; set; } = new Position(0, 0, 0);
            public Position team2_pos { get; set; } = new Position(0, 0, 0);
            public Position waitingRoom { get; set; } = new Position(0, 0, 0);
            public uint currentWeapon { get; set; } = 0;
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
        public class BusinessEvent
        {
            public static async Task updateBusiness(BusinessModel b, Marker l, PlayerLabel i)
            {
                //l.SetData(EntityData.BusinessEntityData.BusinessInfo, JsonConvert.SerializeObject(b));
                // Mühür
                if (b.settings.TotalTax > (b.price / 10))
                    b.isLocked = true;

                l.Position = b.position;
                l.DisplayName = await Props.Business.setupBusinessName(b);
                l.Scale = b.settings.markerScale;
                l.MarkerType = (MarkerTypes)b.settings.MarkerType;
                l.Color = b.settings.MarkerColor;
                l.isBusinessMarker = true;
                i.Dimension = b.dimension;
                i.Position = b.interiorPosition;
                i.Text = "按 [~g~E键~w~] 离开";
                i.Font = 0;
                i.Scale = 0.4f;
                i.SetData(EntityData.EntranceTypes.ExitWorldPosition, b.position);
                await Database.DatabaseMain.UpdateBusiness(b);
            }
            public static async Task<int> createBusiness(BusinessModel biz)
            {
                int ID = await Database.DatabaseMain.CreateBusiness(biz);
                biz.ID = ID;

                Marker bizMark = MarkerStreamer.Create((MarkerTypes)biz.settings.MarkerType, biz.position, new Vector3(1, 1, 1), dimension: 0, color: biz.settings.MarkerColor, streamRange: 5);
                bizMark.isBusinessMarker = true;
                bizMark.DisplayName = await Props.Business.setupBusinessName(biz);
                bizMark.SetData(EntityData.EntranceTypes.EntranceType, EntityData.EntranceTypes.Business);
                bizMark.SetData("Biz:ID", biz.ID);
                PlayerLabel labelExit = TextLabelStreamer.Create("按 [~g~E键~w~] 离开", biz.interiorPosition, biz.dimension, true, new Rgba(255, 255, 255, 255), streamRange: 2);
                labelExit.SetData(EntityData.EntranceTypes.EntranceType, EntityData.EntranceTypes.ExitWorld);
                labelExit.SetData(EntityData.EntranceTypes.ExitWorldPosition, biz.position);
                bizMark.SetData(EntityData.BusinessEntityData.interiorLabelId, labelExit.Id);

                return ID;
            }
            public static async Task Delete(BusinessModel biz, Marker l, PlayerLabel i)
            {
                await Database.DatabaseMain.DeleteBusiness(biz.ID);
                l.Destroy();
                i.Delete();
            }
        }
    }
}
