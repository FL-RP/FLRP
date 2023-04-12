using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace outRp.Models
{
    public class VehModel : AltV.Net.Elements.Entities.Vehicle
    {
        public int sqlID { get; set; }
        public int owner { get; set; }
        public int factionId { get; set; }
        public int jobId { get; set; }
        public int businessId { get; set; }
        public double km { get; set; }
        public int inventoryCapacity { get; set; } = 100;
        public int maxFuel { get; set; }
        public int currentFuel { get; set; }
        public int fuelConsumption { get; set; }
        public int price { get; set; }
        public int defaultTax { get; set; }
        private bool _towwed { get; set; } = false;

        public string radioLink { get; set; } = "none";

        public bool towwed
        {
            get
            {
                return _towwed;
            }
            set
            {
                if (_towwed == value)
                    return;

                _towwed = value;
                if (value == true)
                {
                    SetSyncedMetaData(Globals.EntityData.VehicleEntityData.VehicleisTowwed, value);
                }
                else
                {
                    if (HasSyncedMetaData(Globals.EntityData.VehicleEntityData.VehicleisTowwed))
                    {
                        DeleteSyncedMetaData(Globals.EntityData.VehicleEntityData.VehicleisTowwed);
                    }
                }
            }
        }
        public int fine { get; set; }
        public int rentOwner { get; set; }
        public string TuningData { get; set; }
        public int JobEvent { get; set; }
        public string vehInv { get; set; } = "[]";
        public bool isTrunkOpen { get; set; }
        public bool isHoodOpen { get; set; }
        public Position savePos { get; set; } = new Position(0, 0, 0);
        public float engineBoost { get; set; } = -10.0f;
        public VehSet settings { get; set; } = new VehSet();
        private bool _window { get; set; } = true;
        public bool window
        {
            get
            {
                return _window;
            }
            set
            {
                //this.SetWindowOpened(0, value); this.SetWindowOpened(1, value); this.SetWindowOpened(2, value); this.SetWindowOpened(3, value);
                this.SetStreamSyncedMetaData("Vehicle:Window", value);
                _window = value;
            }
        }

        // Selling System
        private int _sellPrice { get; set; } = 0;
        public int sellPrice
        {
            get
            {
                return _sellPrice;
            }
            set
            {
                if (_sellPrice == value)
                    return;

                _sellPrice = value;
                
                if(_sellPrice > 0) 
                {
                    try
                    {
                        Task.Run(async () => {
                            var owner = await Database.DatabaseMain.getCharacterInfo(this.owner);
                            string text = "* 车上贴着出售车辆的信息和车主的联系方式 *";
                            if(owner != null) { text += "~n~联系人: " + owner.characterName.Replace('_', ' '); }
                            if(owner != null && owner.phoneNumber > 0) { text += "~n~电话: " + owner.phoneNumber; }
                            
                            Globals.GlobalEvents.SetVehicleTag(this, $"{text}");
                        });
                    } catch{}
                } 
                else 
                {
                    Globals.GlobalEvents.ClearVehicleTag(this);
                    Globals.GlobalEvents.ClearVehicleTag(this, false);
                }
            }
        }

        public void Update() => Database.DatabaseMain.UpdateVehicleInfo(this);

        public VehModel(ICore server, IntPtr nativePointer, ushort id) : base(server, nativePointer, id)
        {
            try
            {
                sqlID = 0;
                owner = 0;
                factionId = 0;
                jobId = 0;
                businessId = 0;
                km = 0;
                inventoryCapacity = 0;
                maxFuel = 0;
                currentFuel = 0;
                fuelConsumption = 0;
                price = 0;
                defaultTax = 0;
                towwed = false;
                fine = 0;
                rentOwner = 0;
                JobEvent = 0;
                savePos = new Position(0, 0, 0);
                isTrunkOpen = false;
                isHoodOpen = false;
            }
            catch (Exception ex) { Alt.Core.LogDebug($"{ex}"); }
        }
    }
    public class MyVehicleFactory : IEntityFactory<IVehicle>
    {
        public IVehicle Create(ICore server, IntPtr playerPointer, ushort id)
        {
            try
            {
                return new VehModel(server, playerPointer, id);
            }
            catch (Exception ex) { Alt.Core.LogDebug($"{ex}"); return null; }
        }
    }

    public class VehSet
    {
        public bool hasNeon { get; set; } = false;
        public Rgba NeonColor { get; set; } = new Rgba(0, 0, 0, 0);
        public Position savePosition { get; set; } = new Position(0, 0, 0);
        public Rotation saveRotation { get; set; } = new Rotation(0, 0, 0);
        public int SaveDimension { get; set; } = 0;
        public string ModifiyData { get; set; } = "2802_BQUAAW8AACAAAIYUAAAPBgzKlYAAAAAAAAAAAAAAAA===";
        public string WantModifyData { get; set; } = "2802_BQUAAW8AACAAAIYUAAAPBgzKlYAAAAAAAAAAAAAAAA===";
        public bool isWanted { get; set; } = false;
        public int WheelType { get; set; } = 0;
        public int SecurityLevel { get; set; } = 0;
        public bool TrunkLock { get; set; } = true;
        public Position parkPosition { get; set; } = new Position(0, 0, 0);
        public bool PDLock { get; set; } = false;
        public string PDLockName { get; set; } = "";
        public string PDWantedName { get; set; } = "";
        public DateTime PDLockDate { get; set; } = DateTime.Now.AddDays(-10);
        public byte HeadlightColor { get; set; } = 0;
        public bool driftMode { get; set; } = false;
        public int RentPrice { get; set; } = 5;
        public List<string> fines { get; set; } = new();        
    }
}
