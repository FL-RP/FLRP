using System.Collections.Generic;
using System.Numerics;
using AltV.Net.EntitySync;

namespace outRp.OtherSystem.Textlabels
{
    public class Blip : Entity, IEntity
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

        public int BlipType
        {
            get
            {
                if (!TryGetData("BlipType", out int blipType))
                    return 999;

                return BlipType;
            }
            set
            {
                if (BlipType == value)
                    return;

                SetData("BlipType", value);
            }
        }

        public int radius
        {
            get
            {
                if (!TryGetData("radius", out int radius))
                    return 999;

                return radius;
            }
            set
            {
                if (radius == value)
                    return;

                SetData("radius", value);
            }
        }

        public int width
        {
            get
            {
                if (!TryGetData("width", out int width))
                    return 999;

                return width;
            }
            set
            {
                if (width == value)
                    return;

                SetData("width", value);
            }
        }

        public int height
        {
            get
            {
                if (!TryGetData("height", out int height))
                    return 999;

                return height;
            }
            set
            {
                if (height == value)
                    return;

                SetData("height", value);
            }
        }
        public int alpha
        {
            get
            {
                if (!TryGetData("alpha", out int alpha))
                    return 999;

                return alpha;
            }
            set
            {
                if (alpha == value)
                    return;

                SetData("alpha", value);
            }
        }

        public bool? missionCreator
        {
            get
            {
                if (!TryGetData("missionCreator", out bool missionCreator))
                    return null;

                return missionCreator;
            }
            set
            {
                if (missionCreator == value)
                    return;

                SetData("missionCreator", value);
            }
        }

        public bool? bright
        {
            get
            {
                if (!TryGetData("bright", out bool bright))
                    return null;

                return bright;
            }
            set
            {
                if (bright == value)
                    return;

                SetData("bright", value);
            }
        }

        public int category
        {
            get
            {
                if (!TryGetData("category", out int category))
                    return 5;

                return category;
            }
            set
            {
                if (category == value)
                    return;

                SetData("category", value);
            }
        }

        public int? color
        {
            get
            {
                if (!TryGetData("color", out int color))
                    return null;

                return color;
            }
            set
            {
                if (color == value)
                    return;

                SetData("color", value);
            }
        }

        public bool? crewIndicatorVisible
        {
            get
            {
                if (!TryGetData("crewIndicatorVisible", out bool crewIndicatorVisible))
                    return null;

                return crewIndicatorVisible;
            }
            set
            {
                if (crewIndicatorVisible == value)
                    return;

                SetData("crewIndicatorVisible", value);
            }
        }

        public int? flashInterval
        {
            get
            {
                if (!TryGetData("flashInterval", out int flashInterval))
                    return null;

                return flashInterval;
            }
            set
            {
                if (flashInterval == value)
                    return;

                SetData("flashInterval", value);
            }
        }

        public int? flashTimer
        {
            get
            {
                if (!TryGetData("flashTimer", out int flashTimer))
                    return null;

                return flashTimer;
            }
            set
            {
                if (flashTimer == value)
                    return;

                SetData("flashTimer", value);
            }
        }

        public bool? flashes
        {
            get
            {
                if (!TryGetData("flashes", out bool flashes))
                    return null;

                return flashes;
            }
            set
            {
                if (flashes == value)
                    return;

                SetData("flashes", value);
            }
        }

        public bool? flashesAlternate
        {
            get
            {
                if (!TryGetData("flashesAlternate", out bool flashesAlternate))
                    return null;

                return flashesAlternate;
            }
            set
            {
                if (flashesAlternate == value)
                    return;

                SetData("flashesAlternate", value);
            }
        }

        public bool? friendIndicatorVisible
        {
            get
            {
                if (!TryGetData("friendIndicatorVisible", out bool friendIndicatorVisible))
                    return null;

                return friendIndicatorVisible;
            }
            set
            {
                if (friendIndicatorVisible == value)
                    return;

                SetData("friendIndicatorVisible", value);
            }
        }

        public bool? friendly
        {
            get
            {
                if (!TryGetData("friendly", out bool friendly))
                    return null;

                return friendly;
            }
            set
            {
                if (friendly == value)
                    return;

                SetData("friendly", value);
            }
        }

        public string? gxtName
        {
            get
            {
                if (!TryGetData("gxtName", out string gxtName))
                    return null;

                return gxtName;
            }
            set
            {
                if (gxtName == value)
                    return;

                SetData("gxtName", value);
            }
        }

        public int? heading
        {
            get
            {
                if (!TryGetData("heading", out int heading))
                    return null;

                return heading;
            }
            set
            {
                if (heading == value)
                    return;

                SetData("heading", value);
            }
        }

        public bool? headingIndicatorVisible
        {
            get
            {
                if (!TryGetData("headingIndicatorVisible", out bool headingIndicatorVisible))
                    return null;

                return headingIndicatorVisible;
            }
            set
            {
                if (headingIndicatorVisible == value)
                    return;

                SetData("headingIndicatorVisible", value);
            }
        }

        public bool? highDetail
        {
            get
            {
                if (!TryGetData("highDetail", out bool highDetail))
                    return null;

                return highDetail;
            }
            set
            {
                if (highDetail == value)
                    return;

                SetData("highDetail", value);
            }
        }

        public string? name
        {
            get
            {
                if (!TryGetData("name", out string name))
                    return null;

                return name;
            }
            set
            {
                if (name == value)
                    return;

                SetData("name", value);
            }
        }

        public int? number
        {
            get
            {
                if (!TryGetData("number", out int number))
                    return null;

                return number;
            }
            set
            {
                if (number == value)
                    return;

                SetData("number", value);
            }
        }

        public bool? outlineIndicatorVisible
        {
            get
            {
                if (!TryGetData("outlineIndicatorVisible", out bool outlineIndicatorVisible))
                    return null;

                return outlineIndicatorVisible;
            }
            set
            {
                if (outlineIndicatorVisible == value)
                    return;

                SetData("outlineIndicatorVisible", value);
            }
        }

        public int? priority
        {
            get
            {
                if (!TryGetData("priority", out int priority))
                    return null;

                return priority;
            }
            set
            {
                if (priority == value)
                    return;

                SetData("priorty", value);
            }
        }

        public bool? pulse
        {
            get
            {
                if (!TryGetData("pulse", out bool pulse))
                    return null;

                return pulse;
            }
            set
            {
                if (pulse == value)
                    return;

                SetData("pulse", value);
            }
        }

        public bool? route
        {
            get
            {
                if(!TryGetData("route", out bool route))
                    return null;

                return route;
            }
            set
            {
                if (route == value)
                    return;

                SetData("route", value);
            }
        }

        public int? routeColor
        {
            get
            {
                if (!TryGetData("routeColor", out int routeColor))
                    return null;

                return routeColor;
            }
            set
            {
                if (routeColor == value)
                    return;

                SetData("routeColor", value);
            }
        }

        public float? scale
        {
            get
            {
                if (!TryGetData("scale", out float scale))
                    return null;

                return scale;
            }
            set
            {
                if (scale == value)
                    return;

                SetData("scale", value);
            }
        }

        public int? secondaryColor
        {
            get
            {
                if (!TryGetData("secondaryColor", out int secondaryColor))
                    return null;

                return secondaryColor;
            }
            set
            {
                if (secondaryColor == value)
                    return;

                SetData("secondaryColor", value);
            }
        }

        public bool? shortRange
        {
            get
            {
                if (!TryGetData("shortRange", out bool shortRange))
                    return null;

                return shortRange;
            }
            set
            {
                if (shortRange == value)
                    return;

                SetData("shortRange", value);
            }
        }

        public bool? showCone
        {
            get
            {
                if (!TryGetData("showCone", out bool showCone))
                    return null;

                return showCone;
            }
            set
            {
                if (showCone == value)
                    return;

                SetData("showCone", value);
            }
        }

        public bool? shrinked
        {
            get
            {
                if (!TryGetData("shrinked", out bool shrinked))
                    return null;

                return shrinked;
            }
            set
            {
                if (shrinked == value)
                    return;

                SetData("shrinked", value);
            }
        }

        public int? sprite
        {
            get
            {
                if (!TryGetData("sprite", out int sprite))
                    return null;

                return sprite;
            }
            set
            {
                if (sprite == value)
                    return;

                SetData("sprite", value);
            }
        }

        public bool? tickVisible
        {
            get
            {
                if (!TryGetData("tickVisible", out bool tickVisible))
                    return null;

                return tickVisible;
            }
            set
            {
                if (tickVisible == value)
                    return;

                SetData("tickVisible", value);
            }
        }

        private static List<Blip> blipList = new List<Blip>();

        public static List<Blip> BlipList
        {
            get
            {
                lock (blipList)
                {
                    return blipList;
                }
            }
            set
            {
                blipList = value;
            }
        }

        public Blip(Vector3 position, int dimension, uint range, ulong entityType) : base (entityType, position, dimension, range)
        {
            EntityType = entityType;
        }

        public void Destroy()
        {
            if(Blip.BlipList.Contains(this))
                Blip.BlipList.Remove(this);
            AltEntitySync.RemoveEntity(this);
        }


    }
    public static class BlipStreamer
    {
        public static Blip Create(Vector3 position, int blipType = 1,  int color = 1, int dimension = 0, uint range = 450)
        {
            Blip blip = new Blip(position, dimension, range, 5)
            {
                BlipType = blipType,
                height = 80,
                width = 80,
                radius = 80,
                color = color,            
            };
            Blip.BlipList.Add(blip);
            AltEntitySync.AddEntity(blip);
            return blip;
        }

        public static bool Delete(ulong blipId)
        {
            Blip blip = GetBlip(blipId);

            if (blip == null)
                return false;
            if(Blip.BlipList.Contains(blip))
                Blip.BlipList.Remove(blip);
            AltEntitySync.RemoveEntity(blip);
            return true;
        }

        public static void Delete(Blip blip)
        {
            if (Blip.BlipList.Contains(blip))
                Blip.BlipList.Remove(blip);
            AltEntitySync.RemoveEntity(blip);
        }

        public static Blip GetBlip(ulong BlipId)
        {
            if(!AltEntitySync.TryGetEntity(BlipId, 5, out IEntity entity))
            {
                //Console.WriteLine($"[Blip-STREAMER] [GetBlip] ERROR: Entity with ID { BlipId } couldn't be found.");
                return null;
            }

            if (!(entity is Blip))
                return null;

            return (Blip)entity;
        }

        public static void DestroyAllBlips()
        {
            foreach(Blip blip in GetAllBlips())
            {
                AltEntitySync.RemoveEntity(blip);
            }
            Blip.BlipList.Clear();
        }

        public static List<Blip> GetAllBlips()
        {
            List<Blip> blips = new List<Blip>(Blip.BlipList);
            /*foreach(IEntity entity in Blip.BlipList)
            {
                Blip obj = GetBlip(entity.Id);

                if (obj != null)
                    blips.Add(obj);
            }*/

            return blips;
        }
    }
}
