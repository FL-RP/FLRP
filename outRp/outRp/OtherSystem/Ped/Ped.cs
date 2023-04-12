using System;
using System.Collections.Generic;
using System.Numerics;
using AltV.Net.Data;
using AltV.Net.EntitySync;

namespace outRp.OtherSystem
{
    public class PedModel : Entity, IEntity
    {
        private ulong EntityType
        {
            get
            {
                if (!TryGetData("entityType", out ulong type))
                    return 999;
                return type;
            }
            set
            {
                if (EntityType == value)
                    return;

                SetData("entityType", value);
            }
        }
        private string Model
        {
            get
            {
                if (!TryGetData("model", out string model))
                    return null;

                return model;
            }
            set
            {
                if (Model == value)
                    return;

                SetData("model", value);
            }
        }
        public ushort health
        {
            get
            {
                if (!TryGetData("health", out ushort health))
                    return 0;

                return health;
            }
            set
            {
                if (health == value)
                    return;

                SetData("health", value);
            }
        }

        public int heading
        {
            get
            {
                if (!TryGetData("heading", out int heading))
                    return 0;

                return heading;
            }
            set
            {
                if (heading == value)
                    return;

                SetData("heading", value);
            }
        }

        public string nametag
        {
            get
            {
                if(!TryGetData("nametag", out string nametag))
                    return "无";

                return nametag;
            }
            set
            {
                if (nametag == value)
                    return;

                SetData("nametag", value);
            }
        }

        public bool hasNetOwner
        {
            get
            {
                if (!TryGetData("hasNetOwner", out bool hasNetOwner))
                    return false;

                return hasNetOwner;
            }
            set
            {
                if (hasNetOwner == value)
                    return;

                SetData("hasNetOwner", value);
            }
        }

        
        public ushort? netOwner
        {
            get
            {
                if (!TryGetData("netOwner", out ushort netOwner))
                    return null;

                return netOwner;
            }
            set
            {
                if (netOwner == value)
                    return;

                SetData("netOwner", value);
            }
        }

        public ushort? followTarget
        {
            get
            {
                if (!TryGetData("followTarget", out ushort followTarget))
                    return null;

                return followTarget;
            }
            set
            {
                if (followTarget == value)
                    return;

                SetData("followTarget", value);
            }
        }

        public ushort? goToEntity
        {
            get
            {
                if (!TryGetData("goToEntity", out ushort goToEntity))
                    return null;

                return goToEntity;
            }
            set
            {
                if (goToEntity == value)
                    return;

                SetData("goToEntity", value);
            }
        }

        public ushort? combatTarget
        {
            get
            {
                if (!TryGetData("combatTarget", out ushort combatTarget))
                    return null;

                return combatTarget;
            }
            set
            {
                if (goToEntity == value)
                    return;

                SetData("combatTarget", value);
            }
        }

        public ushort? enterVehicle
        {
            get
            {
                if (!TryGetData("enterVehicle", out ushort enterVehicle))
                    return null;

                return enterVehicle;
            }
            set
            {
                if (enterVehicle == value)
                    return;

                SetData("enterVehicle", value);
            }
        }

        public ushort? exitVehicle
        {
            get
            {
                if (!TryGetData("exitVehicle", out ushort exitVehicle))
                    return null;

                return exitVehicle;
            }
            set
            {
                if (exitVehicle == value)
                    return;
                enterVehicle = null;
                SetData("exitVehicle", value);                
            }
        }

        public string[] animation
        {
            get
            {
                if (!TryGetData("animation", out string[] animation))
                    return null;

                return animation;
            }
            set
            {
                if (animation == value)
                    return;

                SetData("animation", value);
            }
        }

        public bool isHuntingNPC
        {
            get
            {
                if (!TryGetData("isHuntingNPC", out bool isHuntingNPC))
                    return false;

                return isHuntingNPC;
            }
            set
            {
                if (isHuntingNPC == value)
                    return;

                SetData("isHuntingNPC", value);
            }
        }

        public Position Pos
        {
            get
            {
                return (AltV.Net.Data.Position)this.Position;
            }
        }

        // ! Listeler
        public static object LabelLockHandle = new object();

        private static List<PedModel> serverpeds = new List<PedModel>();

        public static List<PedModel> serverPeds
        {
            get
            {
                lock (LabelLockHandle)
                {
                    return serverpeds;
                }
            }
            set
            {
                serverpeds = value;
            }
        }
        // ! Oluşturucular
        public PedModel(Vector3 position, int dimension, uint range, ulong entityType, string model) : base(entityType, position, dimension, range)
        {
            EntityType = entityType;
            Model = model;
            hasNetOwner = false;
            //netOwner = null;
        }

        public void Destroy()
        {
            serverPeds.Remove(this);
            AltEntitySync.RemoveEntity(this);
        }


    }

    public class PedStreamer
    {
        public static PedModel Create(string model, Vector3 position, int dimension = 0, uint range = 200)
        {
            PedModel ped = new PedModel(position, dimension, range, 4, model)
            {
                heading = 1,
                health = 100,
            };

            PedModel.serverPeds.Add(ped);
            AltEntitySync.AddEntity(ped);

            return ped;
        }

        public static PedModel Get(ulong pedId)
        {

            if (!AltEntitySync.TryGetEntity(pedId, 4, out IEntity entity))
            {
                Console.WriteLine($"[PED-STREAMER] [GetDynamicPed] ERROR: Entity with ID { pedId } couldn't be found.");
                return null;
            }

            if (!(entity is PedModel))
                return null;
            
            return (PedModel)entity;
        }

        public static void Delete(PedModel ped)
        {
            PedModel.serverPeds.Remove(ped);
            AltEntitySync.RemoveEntity(ped);
        }

        public static void Delete(ulong pedId)
        {
            PedModel dPed = Get(pedId);
            PedModel.serverPeds.Remove(dPed);
            AltEntitySync.RemoveEntity(dPed);
        }

        public static void DeleteAll()
        {
            foreach(var ped in PedModel.serverPeds)
            {
                AltEntitySync.RemoveEntity(ped);
                PedModel.serverPeds.Remove(ped);
            }
        }

        
    }

}
