using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Enums;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Globals;
using outRp.Models;
using outRp.OtherSystem.Textlabels;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;


namespace outRp.OtherSystem.LSCsystems
{
    public class MDC
    {
        public int ID { get; set; }
        /// <summary>
        /// 1: Para cezası | 2: Hapis | 3: Sicil Cezası |
        /// </summary>
        public int type { get; set; } = 1;
        public int owner { get; set; }
        public int sender { get; set; }
        public string value { get; set; } = "[]";

        public void Update() => Database.DatabaseMain.UpdateMDCEntry(this);
        public void Create() => Database.DatabaseMain.CreateMDCEntry(this);
    }

    public class MDCEvents : IScript
    {
        private class MDCPlayerData
        {
            public int ID { get; set; }
            public string name { get; set; } = "无";
            public int GSM { get; set; } = 0;
            public bool isLicense { get; set; } = false;
            public List<fineModel> fines { get; set; } = new List<fineModel>();
            public List<JailModel> lastJails { get; set; } = new List<JailModel>();
            public List<VehicleModel> vehicles { get; set; } = new List<VehicleModel>();
            public List<HouseModel> houses { get; set; } = new List<HouseModel>();
            public List<BusinessModel> business { get; set; } = new List<BusinessModel>();

            public class fineModel
            {
                public int value { get; set; }
                public string reason { get; set; }
            }
            public class JailModel
            {
                public string date { get; set; }
                public int time { get; set; }
                public string reason { get; set; }
            }

            public class HouseModel
            {
                public int ID { get; set; }
                public string Name { get; set; }
            }
            public class BusinessModel
            {
                public int ID { get; set; }
                public string Name { get; set; }
            }
            public class VehicleModel
            {
                public int ID { get; set; }
                public string Model { get; set; }
                public string Plate { get; set; }
                public int Fine { get; set; }
            }
        }

        private class MDCHouseData
        {
            public int ID { get; set; }
            public int ownerID { get; set; }
            public string ownerName { get; set; }
            public Vector3 pos { get; set; }
        }

        private class MDCBusinessData
        {
            public int ID { get; set; }
            public int ownerID { get; set; }
            public string ownerName { get; set; }
            public Vector3 pos { get; set; }
        }

        public class MDCPhoneData
        {
            public int ID { get; set; }
            public int ownerID { get; set; }
            public string ownerName { get; set; }
            public Vector3 pos { get; set; }
        }

        public class WantedList
        {
            public int ID { get; set; }
            public string name { get; set; }
            public string reason { get; set; }
            public string current { get; set; } = "未采取任何行动.";
            public string opener { get; set; }
        }

        public class MDCCalls
        {
            public int ID { get; set; } = 1;
            public int callNumber { get; set; }
            public string callerName { get; set; }
            public string pdTeam { get; set; } = "无";
            public string reason { get; set; }
            public Position pos { get; set; }
        }

        public class WantedCar
        {
            public string Plate { get; set; } = "无";
            public string OwnerName { get; set; } = "无";
            public int Fine { get; set; } = 0;
            public string Model { get; set; } = "无";
            public bool isWanted { get; set; } = false;

        }

        public class ForumReports
        {
            public int ID { get; set; }
            public int id_topic { get; set; }
            public string Name { get; set; }

        }
        public class ForumReport
        {
            public int ID { get; set; }
            public string subject { get; set; }
        }

        public static List<WantedList> serverWantedList = new List<WantedList>();

        // Geçici Listeler
        public static List<PlayerModel> onMDC = new List<PlayerModel>();
        public static List<MDCCalls> PDcalls = new List<MDCCalls>();

        public class saveData {
            public List<WantedList> wantedList { get; set; } = new List<WantedList>();
            public List<MDCCalls> calls { get; set; } = new List<MDCCalls>();
        }


        // public static void LoadMDCData(string Data) {
        //     if(Data.Length < 10) return;
        //     saveData data = JsonConvert.DeserializeObject<saveData>(Data);
        //     serverWantedList = data.wantedList;
        //     PDcalls = data.calls;
        // }

        // public static string getSaveData() {
        //     saveData data = new saveData();
        //     data.wantedList = serverWantedList;
        //     data.calls = PDcalls;
        //     return JsonConvert.SerializeObject(data);
        // }
        // Geçici Liste Son
        public static void LoadWantedList(string data)
        {
            serverWantedList = JsonConvert.DeserializeObject<List<WantedList>>(data);
        }

        public static async Task<List<MDC>> GetMDC(int type, int owner)
        {
            return await Database.DatabaseMain.GetMDCEntry(type, owner);
        }
        public static async Task<List<MDC>> GetAllMDC(int type)
        {
            return await Database.DatabaseMain.GetMDCAllEntrys(type);
        }

        public static void MDC_CreateMDC_Wanted(PlayerModel p, string reason, string creatorName)
        {
            WantedList w = new WantedList()
            {
                ID = p.sqlID,
                name = p.characterName.Replace("_", " "),
                reason = reason,
                current = "未采取任何行动",
                opener = creatorName
            };

            serverWantedList.Add(w);
        }
        public static void MDC_CreateMDC_Wanted(PlayerModelInfo p, string reason, string creatorName)
        {
            WantedList w = new WantedList()
            {
                ID = p.sqlID,
                name = p.characterName.Replace("_", " "),
                reason = reason,
                current = "未采取任何行动",
                opener = creatorName
            };

            serverWantedList.Add(w);
        }
        public static void MDC_CreateMDC_Call(PlayerModel p, string reason)
        {
            MDCCalls n = new MDCCalls()
            {
                callerName = p.characterName.Replace("_", " "),
                callNumber = p.phoneNumber,
                reason = reason + " " + DateTime.Now.ToShortTimeString(),
                pos = p.Position
            };
            PDcalls.Add(n);
        }

        public static void MDC_CreateRecord_Fine(PlayerModel p, PlayerModel sender, int price, string reason)
        {
            MDCPlayerData.fineModel f = new MDCPlayerData.fineModel()
            {
                reason = reason,
                value = price
            };

            MDC x = new MDC()
            {
                owner = p.sqlID,
                sender = sender.sqlID,
                type = 1,
                value = JsonConvert.SerializeObject(f),
            };

            x.Create();
        }

        public static void MDC_CreateRecord_Jail(PlayerModel p, PlayerModel sender, int time, string reason)
        {
            MDCPlayerData.JailModel j = new MDCPlayerData.JailModel()
            {
                date = DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
                reason = reason,
                time = time
            };
            MDC x = new MDC()
            {
                owner = p.sqlID,
                sender = sender.sqlID,
                type = 2,
                value = JsonConvert.SerializeObject(j)
            };

            x.Create();
        }

        #region MDC WEB LISTENERS 
        [AsyncClientEvent("MDC:SearchPeople")]
        public async Task MDC_SearchPlayerData(PlayerModel p, string name)
        {
            PlayerModelInfo target = await Database.DatabaseMain.getCharacterinfoWithName(name);
            if (target == null) { return; } // Bulunamadı emit.

            List<MDC> _fines = await GetMDC(1, target.sqlID);
            List<MDCPlayerData.fineModel> fines = new List<MDCPlayerData.fineModel>();
            foreach (MDC f in _fines)
            {
                fines.Add(JsonConvert.DeserializeObject<MDCPlayerData.fineModel>(f.value));
            }

            List<MDC> _jails = await GetMDC(2, target.sqlID);
            List<MDCPlayerData.JailModel> jails = new List<MDCPlayerData.JailModel>();
            foreach (MDC j in _jails)
            {
                jails.Add(JsonConvert.DeserializeObject<MDCPlayerData.JailModel>(j.value));
            }
            List<MDCPlayerData.VehicleModel> vehicles = new List<MDCPlayerData.VehicleModel>();
            foreach (VehModel vehs in Alt.GetAllVehicles())
            {
                if (vehs.owner == target.sqlID)
                {
                    vehicles.Add(new MDCPlayerData.VehicleModel()
                    {
                        ID = vehs.Id,
                        Fine = vehs.fine,
                        Plate = vehs.NumberplateText,
                        Model = ((VehicleModel)vehs.Model).ToString()
                    });
                }
            };

            List<MDCPlayerData.HouseModel> houses = new List<MDCPlayerData.HouseModel>();
            foreach (var house in await Database.DatabaseMain.getPlayerHouses(target.sqlID))
            {
                houses.Add(new MDCPlayerData.HouseModel()
                {
                    ID = house.ID,
                    Name = house.name,
                });
            };

            List<MDCPlayerData.BusinessModel> bizs = new List<MDCPlayerData.BusinessModel>();
            foreach (var biz in await Database.DatabaseMain.GetMemberBusinessList(target.sqlID))
            {
                bizs.Add(new MDCPlayerData.BusinessModel()
                {
                    ID = biz.ID,
                    Name = biz.name
                });
            };

            MDCPlayerData data = new MDCPlayerData();
            data.ID = target.sqlID;
            data.name = target.characterName.Replace("_", " ");
            data.GSM = target.phoneNumber;
            data.fines = fines;
            data.lastJails = jails;
            data.vehicles = vehicles;
            data.houses = houses;
            data.business = bizs;
            CharacterSettings set = JsonConvert.DeserializeObject<CharacterSettings>(target.settings);
            if (set.driverLicense != null) { data.isLicense = true; }
            else { data.isLicense = false; }

            p.EmitLocked("MDC:ShowData:Player", JsonConvert.SerializeObject(data));
            return;
        }

        [AsyncClientEvent("MDC:SearchHouse")]
        public async Task MDC_SearchHouse(PlayerModel p, int hID)
        {
            (HouseModel, PlayerLabel, Marker) h = await Props.Houses.getHouseById(hID);
            if (h.Item1 == null) { return; } // send error info

            string oName = "无主";
            if (h.Item1.ownerId > 0)
            {
                var _oName = await Database.DatabaseMain.getCharacterInfo(h.Item1.ownerId);
                oName = _oName.characterName.Replace("_", " ");
            }
            MDCHouseData hd = new MDCHouseData()
            {
                ID = h.Item1.ID,
                ownerID = h.Item1.ownerId,
                ownerName = oName,
                pos = h.Item1.pos
            };
            p.EmitLocked("MDC:ShowData:House", JsonConvert.SerializeObject(hd));
            return;
        }

        [AsyncClientEvent("MDC:SearchBusiness")]
        public async Task MDC_SearchBusiness(PlayerModel p, int bID)
        {
            BusinessModel b = await Database.DatabaseMain.GetBusinessInfo(bID);
            if (b == null) { return; } // send wrong info

            string oName = "无主";
            if (b.ownerId > 0)
            {
                var _oName = await Database.DatabaseMain.getCharacterInfo(b.ownerId);
                oName = _oName.characterName.Replace("_", " ");
            }

            MDCBusinessData bd = new MDCBusinessData()
            {
                ID = b.ID,
                ownerID = b.ownerId,
                ownerName = oName,
                pos = b.position
            };

            p.EmitLocked("MDC:ShowData:Business", JsonConvert.SerializeObject(bd));
            return;
        }

        [AsyncClientEvent("MDC:SearchPhone")]
        public void MDC_SearchPhone(PlayerModel p, int number)
        {
            PlayerModel t = null;
            foreach (PlayerModel c in Alt.GetAllPlayers())
            {
                if (c.phoneNumber == number) { t = c; break; }
            }
            if (t == null) { return; } // errror send

            Position rndPos = Core.Core.SetRandomPositionsWithRadius(t.Position, 1, 45, 1)[0];

            MDCPhoneData ph = new MDCPhoneData()
            {
                ID = t.sqlID,
                ownerID = t.sqlID,
                ownerName = t.characterName.Replace("_", " "),
                pos = rndPos
            };

            p.EmitLocked("MDC:ShowData:Phone", JsonConvert.SerializeObject(ph));
            return;
        }

        [AsyncClientEvent("MDC:WantedStatus")]
        public void MDC_WantedStatusUpdate(PlayerModel p, int ID, string status)
        {
            WantedList curr = serverWantedList.Find(x => x.ID == ID);
            if (curr == null)
                return;

            curr.current = status;

            string json = JsonConvert.SerializeObject(serverWantedList);
            foreach (PlayerModel t in onMDC)
            {
                if (!t.isOnline || !t.Exists) { onMDC.Remove(t); continue; }

                t.EmitLocked("MDC:ShowWantedList", json);
            }
        }

        [AsyncClientEvent("MDC:RemoveWanted")]
        public void MDC_WantedListRemove(PlayerModel p, int ID)
        {
            WantedList curr = serverWantedList.Find(x => x.ID == ID);
            if (curr == null)
                return;

            serverWantedList.Remove(curr);

            string json = JsonConvert.SerializeObject(serverWantedList);
            foreach (PlayerModel t in onMDC)
            {
                if (!t.isOnline || !t.Exists) { onMDC.Remove(t); continue; }

                t.EmitLocked("MDC:ShowWantedList", json);
            }
            return;
        }

        [AsyncClientEvent("MDC:JoinCall")]
        public void MDC_JoinCall(PlayerModel p, int id)
        {
            MDCCalls curr = PDcalls.Find(x => x.ID == id);
            if (curr == null)
                return;

            Globals.System.PDTeamModel team = Globals.System.PD.pdTeams.Find(x => x.leaderId == p.sqlID || x.memberId == p.sqlID);
            if (team == null)
                return;

            curr.pdTeam = team.name;
            string json = JsonConvert.SerializeObject(PDcalls);
            foreach (PlayerModel t in onMDC)
            {
                if (!t.Exists || !t.isOnline) { onMDC.Remove(t); }
                else
                {
                    t.EmitLocked("MDC:ShowCallList", json);
                }
            }
        }

        [AsyncClientEvent("MDC:RemoveCall")]
        public void MDC_RemoveCall(PlayerModel p, int id)
        {
            MDCCalls call = PDcalls.Find(x => x.ID == id);
            if (call == null)
                return;

            PDcalls.Remove(call);
            string json = JsonConvert.SerializeObject(PDcalls);
            foreach (PlayerModel t in onMDC)
            {
                if (!t.Exists || !t.isOnline) { onMDC.Remove(t); }
                else
                {
                    t.EmitLocked("MDC:ShowCallList", json);
                }
            }
        }

        [AsyncClientEvent("MDC:VehicleSearch")]
        public async Task MDC_VehicleSearch(PlayerModel p, string Plate)
        {
            VehModel v = null;
            foreach (VehModel a in Alt.GetAllVehicles())
            {
                if (a.NumberplateText == Plate) { v = a; break; }
            }
            if (v == null) { p.EmitLocked("MDC:ShowVehicle", "[]"); return; }
            PlayerModelInfo t = await Database.DatabaseMain.getCharacterInfo(v.owner);
            if (t == null) { p.EmitLocked("MDC:ShowVehicle", "[]"); return; }

            WantedCar c = new WantedCar();
            c.Fine = v.fine;
            c.isWanted = v.settings.isWanted;
            var model = (VehicleModel)v.Model;
            c.Model = model.ToString();
            c.OwnerName = t.characterName.Replace("_", " ");
            c.Plate = v.NumberplateText;

            p.EmitLocked("MDC:ShowVehicle", JsonConvert.SerializeObject(c));
        }

        [AsyncClientEvent("MDC:GetReports")]
        public async Task MDC_GetAllReports(PlayerModel p, int type)
        {
            List<ForumReports> _reports = await Database.DatabaseMain.MDC_GetReports(type);
            List<ForumReports> reports = new List<ForumReports>();

            _reports.ForEach(x =>
            {
                if (reports.Find(y => y.id_topic == x.id_topic) == null)
                    reports.Add(x);
            });
            //Alt.Log(reports.Count.ToString());

            p.EmitLocked("MDC:LoadReports", JsonConvert.SerializeObject(reports));
            //Alt.Log(JsonConvert.SerializeObject(reports));
            return;
        }

        [AsyncClientEvent("MDC:GetReport")]
        public async Task MDC_GetReport(PlayerModel p, int id_topic)
        {
            var report = await Database.DatabaseMain.MDC_GetReport(id_topic);
            List<ForumReport> repor = new List<ForumReport>();
            foreach (var x in report)
            {
                repor.Add(new ForumReport()
                {
                    ID = x.ID,
                    subject = Core.Core.BBcodeToHTML(x.subject),
                });
            };
            p.EmitLocked("MDC:LoadReport", JsonConvert.SerializeObject(repor));
            return;
        }
        [Command("mdc")]
        public async Task MDC_Open(PlayerModel p)
        {
            if (!await Globals.System.PD.CheckPlayerInPd(p)) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotInPd); return; }
            onMDC.Add(p);

            string d1 = JsonConvert.SerializeObject(serverWantedList);
            string d2 = JsonConvert.SerializeObject(PDcalls);

            p.EmitLocked("MDC:Open", d2, d1);
        }

        [AsyncClientEvent("MDC:Close")]
        public void MDC_CLose(PlayerModel p)
        {
            PlayerModel check = onMDC.Find(x => x.sqlID == p.sqlID);
            if (check == null)
                return;

            onMDC.Remove(check);
        }
        #endregion
    }
}
