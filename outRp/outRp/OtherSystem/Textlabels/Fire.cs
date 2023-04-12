using System.Collections.Generic;
using System.Numerics;
using AltV.Net.EntitySync;

namespace outRp.OtherSystem.Textlabels
{
    public class Fire : Entity, IEntity
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

        public bool isGasFire
        {
            get
            {
                if (!TryGetData("isGasFire", out bool isGasFire))
                    return false;

                return isGasFire;
            }
            set
            {
                if (isGasFire == value)
                    return;

                SetData("isGasFire", value);
            }
        }

        public int maxChild
        {
            get
            {
                if (!TryGetData("maxChild", out int maxChild))
                    return 0;

                return maxChild;
            }
            set
            {
                if (maxChild == value)
                    return;

                SetData("maxChild", value);
            }
        }

        private static List<Fire> fireList = new List<Fire>();

        public static List<Fire> FireList
        {
            get
            {
                lock (fireList)
                {
                    return fireList;
                }
            }
            set
            {
                fireList = value;
            }
        }

        public Fire(Vector3 position, int dimension, uint range, ulong entityType) : base (entityType, position, dimension, range)
        {
            EntityType = entityType;
        }

        public void Destroy()
        {
            Fire.FireList.Remove(this);
            AltEntitySync.RemoveEntity(this);
        }
    }

    public static class FireStreamer
    {
        public static Fire Create(Vector3 position, int dimension = 0, bool isGas = false, int maxChild = 0, uint range = 200)
        {
            Fire fire = new Fire(position, dimension, range, 6)
            {
                maxChild = maxChild,
                isGasFire = isGas
            };

            Fire.FireList.Add(fire);
            AltEntitySync.AddEntity(fire);
            return fire;
        }

        public static bool Delete(ulong fireId)
        {
            Fire fire = GetFire(fireId);

            if (fire == null)
                return false;

            Fire.FireList.Remove(fire);
            AltEntitySync.RemoveEntity(fire);
            return true;
        }

        public static void Delete(Fire fire)
        {
            Fire.FireList.Remove(fire);
            AltEntitySync.RemoveEntity(fire);
        }


        public static Fire GetFire(ulong fireId)
        {
            if(!AltEntitySync.TryGetEntity(fireId, 6, out IEntity entity))
            {
                //Console.WriteLine("Ateş objesi getirilemedi.");
                return null;
            }

            if (!(entity is Fire))
                return null;

            return (Fire)entity;
        }

        public static void DestroyAllFire()
        {
            foreach(Fire fire in GetAllFires())
            {
                AltEntitySync.RemoveEntity(fire);
            }
            Fire.FireList.Clear();
        }

        public static List<Fire> GetAllFires()
        {
            List<Fire> fires = new List<Fire>();
            foreach(IEntity entity in Fire.FireList)
            {
                Fire obj = GetFire(entity.Id);

                if (obj != null)
                    fires.Add(obj);
            }

            return fires;
        }
    }
}
