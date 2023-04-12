using AltV.Net;
using outRp.Chat;
using outRp.Models;
using System.Collections.Generic;
using AltV.Net.Resources.Chat.Api;
using Newtonsoft.Json;

namespace outRp.OtherSystem
{
    public class AttachmentSystem : IScript
    {
        public static int ObjectCounter = 0;
        public class ObjectModel
        {
            public string Model { get; set; }
            public string boneIndex { get; set; }
            public double xPos { get; set; } = 0;
            public double yPos { get; set; } = 0;
            public double zPos { get; set; } = 0;
            public double xRot { get; set; } = 0;
            public double yRot { get; set; } = 0;
            public double zRot { get; set; } = 0;
            public long serverID { get; set; } = 0;
        }

        public static long AddAttach(PlayerModel p, ObjectModel o)
        {
            if (p.HasStreamSyncedMetaData("AttachedObjects"))
            {
                p.GetStreamSyncedMetaData<string>("AttachedObjects", out string _pObjects);
                p.DeleteStreamSyncedMetaData("AttachedObjects");
                List<ObjectModel> pObjects = JsonConvert.DeserializeObject<List<ObjectModel>>(_pObjects);
                ++ObjectCounter;
                o.serverID = ObjectCounter;
                pObjects.Add(o);
                p.SetStreamSyncedMetaData("AttachedObjects", JsonConvert.SerializeObject(pObjects));
                return o.serverID;
            }
            else
            {
                List<ObjectModel> nObjects = new List<ObjectModel>();
                nObjects.Add(o);
                ++ObjectCounter;
                o.serverID = ObjectCounter;
                p.SetStreamSyncedMetaData("AttachedObjects", JsonConvert.SerializeObject(nObjects));
                return o.serverID;
            }
        }

        public static long AddAttach(VehModel p, ObjectModel o)
        {
            if (p.HasStreamSyncedMetaData("AttachedObjects"))
            {
                p.GetStreamSyncedMetaData<string>("AttachedObjects", out string _pObjects);
                p.DeleteStreamSyncedMetaData("AttachedObjects");
                List<ObjectModel> pObjects = JsonConvert.DeserializeObject<List<ObjectModel>>(_pObjects);
                ++ObjectCounter;
                o.serverID = ObjectCounter;                
                pObjects.Add(o);
                p.SetStreamSyncedMetaData("AttachedObjects", JsonConvert.SerializeObject(pObjects));
                return o.serverID;
            }
            else
            {
                List<ObjectModel> nObjects = new List<ObjectModel>();
                nObjects.Add(o);
                ++ObjectCounter;
                o.serverID = ObjectCounter;                
                p.SetStreamSyncedMetaData("AttachedObjects", JsonConvert.SerializeObject(nObjects));
                return o.serverID;
            }
        }

        public static void deleteAllAttachs(PlayerModel p)
        {
            if (p.HasStreamSyncedMetaData("AttachedObjects"))
            {
                p.DeleteStreamSyncedMetaData("AttachedObjects");
            }
        }

        public static void deleteAllAttachs(VehModel p)
        {
            if (p.HasStreamSyncedMetaData("AttachedObjects"))
            {
                p.DeleteStreamSyncedMetaData("AttachedObjects");
            }
        }

        public static void deleteAttachsWithID(PlayerModel p, long ID)
        {
            if (p.HasStreamSyncedMetaData("AttachedObjects"))
            {
                p.GetStreamSyncedMetaData<string>("AttachedObjects", out string _pObjects);
                List<ObjectModel> pObjects = JsonConvert.DeserializeObject<List<ObjectModel>>(_pObjects);
                if(pObjects != null)
                {
                    var deleteObject = pObjects.Find(x => x.serverID == ID);
                    pObjects.Remove(deleteObject);
                    p.SetStreamSyncedMetaData("AttachedObjects", JsonConvert.SerializeObject(pObjects));
                    return;
                }
                return;
            }
            return;
        }
            
        [Command("testa")]
        public void TTesta(PlayerModel p)
        {
            var g = new ObjectModel()
            {
                Model = "prop_rub_bike_03",
                serverID = 1,
                boneIndex = "chassis",
            };

            if(p.Vehicle != null)
            {
                VehModel v = (VehModel)p.Vehicle;
                AddAttach(v, g);
            }
            else
            {                
                AddAttach(p, g);
            }
            
            MainChat.SendInfoChat(p, "[?] 测试 TTesta");
            return;            
        }
    }
}
