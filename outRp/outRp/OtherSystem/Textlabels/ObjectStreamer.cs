using AltV.Net;
using AltV.Net.EntitySync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace outRp.OtherSystem.Textlabels
{
    public enum TextureVariation
    {
        Pacific = 0,
        Azure = 1,
        Nautical = 2,
        Continental = 3,
        Battleship = 4,
        Intrepid = 5,
        Uniform = 6,
        Classico = 7,
        Mediterranean = 8,
        Command = 9,
        Mariner = 10,
        Ruby = 11,
        Vintage = 12,
        Pristine = 13,
        Merchant = 14,
        Voyager = 15
    }

    public class MoveData : IWritable
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Speed { get; set; }

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("X");
            writer.Value(X);
            writer.Name("Y");
            writer.Value("Y");
            writer.Name("Z");
            writer.Value(Z);
            writer.Name("Speed");
            writer.Value(Speed);
            writer.EndObject();
        }
    }

    public class Rgb : IWritable
    {
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }

        public Rgb(int red, int green, int blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();
            writer.Name("Red");
            writer.Value(Red);
            writer.Name("Green");
            writer.Value(Green);
            writer.Name("Blue");
            writer.Value(Blue);
            writer.EndObject();
        }
    }

    public class AttachObj
    {
        public int? attach { get; set; } = null;
        public object? bone { get; set; }
        public Vector3 off { get; set; }
        public Vector3 rot { get; set; }
    }

    /// <summary>
    /// DynamicObject class that stores all data related to a single object
    /// </summary>
    public class LProp : Entity, IEntity
    {
        private readonly IDictionary<string, object> ServerData = new Dictionary<string, object>();
        public void SetServerData(string key, object value)
        {
            ServerData[key] = value;
        }
        public bool HasServerData(string key)
        {
            if (ServerData.ContainsKey(key)) return true;
            else return false;
        }
        public T GetServerData<T>(string key)
        {
            return (T)ServerData[key];
        }
        public void DeleteServerData(string key)
        {
            if (HasServerData(key)) ServerData.Remove(key);
        }
        private static List<LProp> propList = new List<LProp>();

        public static List<LProp> PropList
        {
            get
            {
                lock (propList)
                {
                    return propList;
                }
            }
            set
            {
                propList = value;
            }
        }

        public AltV.Net.Elements.Entities.IColShape colshape { get; set; }

        /// <summary>
        /// Set or get the current object's rotation (in degrees).
        /// </summary>
        public Vector3 Rotation
        {
            get
            {
                if (!TryGetData("rotation", out Dictionary<string, object> data))
                    return default;

                return new Vector3()
                {
                    X = Convert.ToSingle(data["x"]),
                    Y = Convert.ToSingle(data["y"]),
                    Z = Convert.ToSingle(data["z"]),
                };
            }
            set
            {
                // No data changed
                if (Rotation != null && Rotation.X == value.X && Rotation.Y == value.Y && Rotation.Z == value.Z && value != new Vector3(0, 0, 0))
                    return;

                Dictionary<string, object> dict = new Dictionary<string, object>()
                {
                    ["x"] = value.X,
                    ["y"] = value.Y,
                    ["z"] = value.Z,
                };
                SetData("rotation", dict);
            }
        }

        public AttachObj? Attach
        {
            get
            {
                if (!TryGetData("attach", out Dictionary<string, object> data))
                    return null;

                return new AttachObj()
                {
                    attach = (int)data["attach"],
                    bone = (int)data["bone"],
                    rot = new Vector3()
                    {
                        X = Convert.ToSingle(data["X"]),
                        Y = Convert.ToSingle(data["Y"]),
                        Z = Convert.ToSingle(data["Z"])
                    },
                    off = new Vector3()
                    {
                        X = Convert.ToSingle(data["X"]),
                        Y = Convert.ToSingle(data["Y"]),
                        Z = Convert.ToSingle(data["Z"])
                    }

                };
            }
            set
            {
                Dictionary<string, object> dict = new Dictionary<string, object>()
                {
                    ["attach"] = value.attach,
                    ["bone"] = value.bone,
                    ["offX"] = value.off.X,
                    ["offY"] = value.off.Y,
                    ["offZ"] = value.off.Z,
                    ["rotX"] = value.rot.X,
                    ["rotY"] = value.rot.Y,
                    ["rotZ"] = value.rot.Z,
                };
                SetData("attach", dict);
            }
        }

        public Vector3 Velocity
        {
            get
            {
                if (!TryGetData("velocity", out Dictionary<string, object> data))
                    return default;

                return new Vector3()
                {
                    X = Convert.ToSingle(data["x"]),
                    Y = Convert.ToSingle(data["y"]),
                    Z = Convert.ToSingle(data["z"]),
                };
            }
            set
            {
                // No data changed
                if (Velocity != null && Velocity.X == value.X && Velocity.Y == value.Y && Velocity.Z == value.Z && value != new Vector3(0, 0, 0))
                    return;

                Dictionary<string, object> dict = new Dictionary<string, object>()
                {
                    ["x"] = value.X,
                    ["y"] = value.Y,
                    ["z"] = value.Z,
                };
                SetData("velocity", dict);
            }
        }

        public Vector3 SlideToPosition
        {
            get
            {
                if (!TryGetData("slideToPosition", out Dictionary<string, object> data))
                    return default;

                return new Vector3()
                {
                    X = Convert.ToSingle(data["x"]),
                    Y = Convert.ToSingle(data["y"]),
                    Z = Convert.ToSingle(data["z"]),
                };
            }
            set
            {
                // No data changed

                Dictionary<string, object> dict = new Dictionary<string, object>()
                {
                    ["x"] = value.X,
                    ["y"] = value.Y,
                    ["z"] = value.Z,
                };
                //Log.Important("SetData SlideToPosition ");
                SetData("slideToPosition", dict);
            }
        }

        /// <summary>
        /// Set or get the current object's model.
        /// </summary>
        public string Model
        {
            get
            {
                if (!TryGetData("model", out string model))
                    return null;

                return model;
            }
            set
            {
                // No data changed
                if (Model == value)
                    return;

                SetData("model", value);
            }
        }

        /// <summary>
        /// Set or get LOD Distance of the object.
        /// </summary>
        public uint? LodDistance
        {
            get
            {
                if (!TryGetData("lodDistance", out uint lodDist))
                    return null;

                return lodDist;
            }
            set
            {
                // if value is set to null, reset the data
                if (value == null)
                {
                    SetData("lodDistance", null);
                    return;
                }

                // No data changed
                if (LodDistance == value)
                    return;

                SetData("lodDistance", value);
            }
        }

        /// <summary>
        /// Get or set the current texture variation, use null to reset it to default.
        /// </summary>
        public TextureVariation? TextureVariation
        {
            get
            {
                if (!TryGetData("textureVariation", out int variation))
                    return null;

                return (TextureVariation)variation;
            }
            set
            {
                // if value is set to null, reset the data
                if (value == null)
                {
                    SetData("textureVariation", null);
                    return;
                }

                // No data changed
                if (TextureVariation == value)
                    return;

                SetData("textureVariation", (int)value);
            }
        }

        /// <summary>
        /// Get or set the object's dynamic state. Some objects can be moved around by the player when dynamic is set to true.
        /// </summary>
        public bool? Dynamic
        {
            get
            {
                if (!TryGetData("dynamic", out bool isDynamic))
                    return false;

                return isDynamic;
            }
            set
            {
                // if value is set to null, reset the data
                if (value == null)
                {
                    SetData("dynamic", null);
                    return;
                }

                // No data changed
                if (Dynamic == value)
                    return;

                SetData("dynamic", value);
            }
        }

        /// <summary>
        /// Set/get visibility state of object
        /// </summary>
        public bool? Visible
        {
            get
            {
                if (!TryGetData("visible", out bool visible))
                    return false;

                return visible;
            }
            set
            {
                // if value is set to null, reset the data
                if (value == null)
                {
                    SetData("visible", null);
                    return;
                }

                // No data changed
                if (Visible == value)
                    return;

                SetData("visible", value);
            }
        }

        /// <summary>
        /// Set/get an object on fire, NOTE: does not work very well as of right now, fire is very small.
        /// </summary>
        public bool? OnFire
        {
            get
            {
                if (!TryGetData("onFire", out bool onFire))
                    return false;

                return onFire;
            }
            set
            {
                // if value is set to null, reset the data
                if (value == null)
                {
                    SetData("onFire", null);
                    return;
                }

                // No data changed
                if (OnFire == value)
                    return;

                SetData("onFire", value);
            }
        }

        /// <summary>
        /// Freeze an object into it's current position. or get it's status
        /// </summary>
        public bool? Freeze
        {
            get
            {
                if (!TryGetData("freeze", out bool frozen))
                    return false;

                return frozen;
            }
            set
            {
                // if value is set to null, reset the data
                if (value == null)
                {
                    SetData("freeze", null);
                    return;
                }

                // No data changed
                if (Freeze == value)
                    return;

                SetData("freeze", value);
            }
        }


        /// <summary>
        /// Set the light color of the object, use null to reset it to default.
        /// </summary>
        public Rgb LightColor
        {
            get
            {
                if (!TryGetData("lightColor", out Dictionary<string, object> data))
                    return null;

                return new Rgb(
                    Convert.ToInt32(data["r"]),
                    Convert.ToInt32(data["g"]),
                    Convert.ToInt32(data["b"])
                );
            }
            set
            {
                // if value is set to null, reset the data
                if (value == null)
                {
                    SetData("lightColor", null);
                    return;
                }

                // No data changed
                if (LightColor != null && LightColor.Red == value.Red && LightColor.Green == value.Green && LightColor.Blue == value.Blue)
                    return;

                Dictionary<string, object> dict = new Dictionary<string, object>
                {
                    { "r", value.Red },
                    { "g", value.Green },
                    { "b", value.Blue }
                };
                SetData("lightColor", dict);
            }
        }

        public bool isTelevision
        {
            get
            {
                if (!TryGetData("isTelevision", out bool isTelevision))
                    return false;

                return isTelevision;
            }
            set
            {
                if (isTelevision == value)
                    return;

                SetData("isTelevision", value);
            }
        }

        public string televisionLink
        {
            get
            {
                if (!TryGetData("televisionLink", out string televisionLink))
                    return "yok";

                return televisionLink;
            }

            set
            {
                if (televisionLink == value)
                    return;

                SetData("televisionLink", value);
            }
        }

        public string televisionPropHash
        {
            get
            {
                if (!TryGetData("televisionPropHash", out string _televisionPropHash))
                    return "yok";

                return _televisionPropHash;
            }
            set
            {
                if (televisionPropHash == value)
                    return;

                SetData("televisionPropHash", value);
            }
        }

        public string televisionTexture
        {
            get
            {
                if (!TryGetData("televisionTexture", out string televisionTexture))
                    return "yok";

                return televisionTexture;
            }
            set
            {
                if (televisionTexture == value)
                    return;

                SetData("televisionTexture", value);
            }
        }

        public Vector3 PositionInitial { get; internal set; }

        public LProp(Vector3 position, int dimension, uint range, ulong entityType) : base(entityType, position, dimension, range)
        {
        }

        public void SetRotation(Vector3 rot)
        {
            Rotation = rot;
        }

        public void SetPosition(Vector3 pos)
        {
            Position = pos;
        }

        public void Delete()
        {
            if (this.Exists)
            {
                AltEntitySync.RemoveEntity(this);
                if (LProp.PropList.Where(x => x == this).FirstOrDefault() != null)
                    LProp.PropList.Remove(this);
            }

        }

        public void Destroy()
        {
            if (this.Exists)
            {
                AltEntitySync.RemoveEntity(this);
                if (LProp.PropList.Contains(this))
                    LProp.PropList.Remove(this);
            }
        }
    }

    public static class PropStreamer
    {
        /// <summary>
        /// Create a new dynamic object.
        /// </summary>
        /// <param name="model">The object model name.</param>
        /// <param name="position">The position to spawn the object at.</param>
        /// <param name="rotation">The rotation to spawn the object at(degrees).</param>
        /// <param name="dimension">The dimension to spawn the object in.</param>
        /// <param name="isDynamic">(Optional): Set object dynamic or not.</param>
        /// <param name="frozen">(Optional): Set object frozen.</param>
        /// <param name="lodDistance">(Optional): Set LOD distance.</param>
        /// <param name="lightColor">(Optional): set light color.</param>
        /// <param name="onFire">(Optional): set object on fire(DOESN'T WORK PROPERLY YET!)</param>
        /// <param name="textureVariation">(Optional): Set object texture variation.</param>
        /// <param name="visible">(Optional): Set object visibility.</param>
        /// <param name="streamRange">(Optional): The range that a player has to be in before the object spawns, default value is 400.</param>
        /// <returns>The newly created dynamic object.</returns>
        public static LProp Create(
            string model, Vector3 position, Vector3 rotation, int dimension = 0, bool? isDynamic = null, bool? placeObjectOnGroundProperly = false, bool? frozen = null, uint? lodDistance = null,
            Rgb lightColor = null, bool? onFire = null, TextureVariation? textureVariation = null, bool? visible = null, uint streamRange = 400, AttachObj attach = null
            )
        {
            LProp obj = new LProp(position, dimension, streamRange, 2)
            {
                Rotation = rotation,
                Model = model,
                Dynamic = isDynamic ?? null,
                Freeze = frozen ?? null,
                LodDistance = lodDistance ?? null,
                LightColor = lightColor ?? null,
                OnFire = onFire ?? null,
                TextureVariation = textureVariation ?? null,
                Visible = visible ?? null,
                PositionInitial = position,
                Attach = attach ?? new AttachObj()
            };
            LProp.PropList.Add(obj);
            AltEntitySync.AddEntity(obj);
            return obj;
        }

        public static bool Delete(ulong dynamicObjectId)
        {
            LProp obj = GetProp(dynamicObjectId);

            if (obj == null)
                return false;

            if (LProp.PropList.Contains(obj))
                LProp.PropList.Remove(obj);
            AltEntitySync.RemoveEntity(obj);
            return true;
        }

        public static void Delete(LProp obj)
        {
            if (LProp.PropList.Contains(obj))
                LProp.PropList.Remove(obj);
            AltEntitySync.RemoveEntity(obj);
        }

        public static LProp GetProp(ulong dynamicObjectId)
        {
            if (!AltEntitySync.TryGetEntity(dynamicObjectId, 2, out IEntity entity))
            {
                Console.WriteLine($"[Prop-Stream] [GetProp] ERROR: Entity with ID {dynamicObjectId} couldn't be found.");
                return default;
            }

            if (!(entity is LProp))
                return default;

            return (LProp)entity;
        }

        /// <summary>
        /// Destroy all created dynamic objects.
        /// </summary>
        public static void DestroyAllDynamicObjects()
        {
            foreach (LProp obj in GetAllProp())
            {
                AltEntitySync.RemoveEntity(obj);
            }
            LProp.PropList.Clear();
        }

        /// <summary>
        /// Get all created dynamic objects.
        /// </summary>
        /// <returns>A list of dynamic objects.</returns>
        public static List<LProp> GetAllProp()
        {
            List<LProp> objects = new List<LProp>(LProp.PropList);

            /*foreach (IEntity entity in LProp.PropList)
            {
                LProp obj = GetProp(entity.Id);

                if (obj != null)
                    objects.Add(obj);
            }*/

            return objects.ToList();
        }

        /// <summary>
        /// Get the dynamic object that's closest to a specified position.
        /// </summary>
        /// <param name="pos">The position from which to check.</param>
        /// <returns>The closest dynamic object to the specified position, or null if none found.</returns>
        public static (LProp obj, float distance) GetClosestDynamicObject(Vector3 pos)
        {
            if (GetAllProp().Count == 0)
                return (null, 5000);

            LProp obj = null;
            float distance = 5000;

            foreach (LProp o in GetAllProp())
            {
                float dist = Vector3.Distance(o.Position, pos);
                if (dist < distance)
                {
                    obj = o;
                    distance = dist;
                }
            }

            return (obj, distance);
        }
    }
}
