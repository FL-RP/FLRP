using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net; using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Resources.Chat.Api;
using outRp.Models;
using outRp.OtherSystem.Textlabels;
using Newtonsoft.Json;
using outRp.Chat;

namespace outRp.OtherSystem.LSCsystems
{
    public class DoorSystem : IScript
    {
        public class Door
        {
            public int type { get; set; } = 5;// ! Type List : 1 : Faction | 2: Busines | 3: house | 4: player(owner) | 5: useAll
            public int owner { get; set; } = 0;
            public int hash { get; set; }
            public Position pos { get; set; }
            public Rotation rot { get; set; }
            public bool state { get; set; }
            public ulong textlblID { get; set; }
            
        }

        public static List<Door> serverDoors = new List<Door>();


        // ! Kapı yükleyicileri
        public static void LoadServerDoors(string val)
        {
            serverDoors = JsonConvert.DeserializeObject<List<Door>>(val);
            foreach(var d in serverDoors)
            {
                string color = (d.state) ? "~r~" : "~g~";
                Position tPos = d.pos;
                tPos.Z += 0.5f;
                PlayerLabel dL = TextLabelStreamer.Create("~b~[" + color + "E~b~]", tPos, streamRange: 1, font: 0);
                d.textlblID = dL.Id;
            }
            Alt.Log("加载 门系统.");
        }
        // ! Kapı yükleyicileri SON

        public static void LoadDoorsToPlayer(PlayerModel p)
        {
            string json = JsonConvert.SerializeObject(serverDoors);
            p.EmitLocked("Door:UpdateAll", json);
        }

        [AsyncClientEvent("Doors:CreateCallBack")]
        public void CreateDoorCB(PlayerModel p, int hash, float pX, float pY, float pZ, float rX, float rY, float rZ)
        {
            Door nD = new Door();
            nD.hash = hash;
            nD.pos = new Position(pX, pY, pZ - 0.5f);
            nD.rot = new Rotation(rX, rY, rZ);
            nD.state = false;
            string color = (nD.state) ? "~r~" : "~g~";
            nD.textlblID = TextLabelStreamer.Create("~b~[" + color + "E~b~]", new Position(pX, pY, pZ + 0.5f), streamRange: 1, font: 0).Id;
            nD.type = 1;
            serverDoors.Add(nD);
            p.SendChatMessage("已添加门.");
            string json = JsonConvert.SerializeObject(nD);
            foreach(PlayerModel t in Alt.GetAllPlayers())
            {
                t.EmitLocked("Door:Update", json);
            }
        }

        public static async Task<bool> DoorUseEvent(PlayerModel p)
        {
            var nearDoor = serverDoors.Find(x => p.Position.Distance(x.pos) < 1.5);
            if(nearDoor == null) { return false; }
            bool canUse = false;

            switch (nearDoor.type)
            {
                case 1:
                    if (nearDoor.owner == p.factionId) { canUse = true; }
                    break;
                case 2:
                    if (nearDoor.owner == p.businessStaff) { canUse = true; }
                    List<BusinessModel> bizs = await Database.DatabaseMain.GetMemberBusinessList(p);
                    var biz = bizs.Find(x => x.ID == nearDoor.owner);
                    if (biz != null && biz.ownerId == p.sqlID) { canUse = true; }
                    break;
                case 3:
                    // TODO Evleri ekle
                    break;
                case 4:
                    if (nearDoor.owner == p.sqlID) { canUse = true; }
                    break;
                case 5:
                    canUse = true;
                    break;
            }

            if(p.adminWork == true) { canUse = true; }


            if (!canUse) { return false; }

            nearDoor.state = !nearDoor.state;
            doorLblUpdate(nearDoor);

            string json = JsonConvert.SerializeObject(nearDoor);
            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                t.EmitLocked("Door:Update", json);
            }

            return true;
        }

        public static void doorLblUpdate(Door door)
        {
            PlayerLabel dL = TextLabelStreamer.GetDynamicTextLabel(door.textlblID);
            string color = (door.state) ? "~r~" : "~g~";
            dL.Text = "~b~[" + color + "E~b~]";
            dL.Scale = 0.7f;
        }

        public static void LoadDoorStatus(PlayerModel p)
        {
            string json = JsonConvert.SerializeObject(serverDoors);
            p.EmitLocked("Door:UpdateAll", json);
        }
    }
}
