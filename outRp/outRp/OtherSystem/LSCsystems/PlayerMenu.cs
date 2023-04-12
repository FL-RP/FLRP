using System;
using System.Collections.Generic;
using AltV.Net; using AltV.Net.Async;
using outRp.Models;
using Newtonsoft.Json;
using System.Linq;
using outRp.Globals;
using System.Threading.Tasks;

namespace outRp.OtherSystem.LSCsystems
{
    public class PlayerMenu : IScript
    {
        public class PMenu
        {
            public int ID { get; set; }
            public string name { get; set; }
            public int ping { get; set; }
            public int level { get; set; }
        }

        [AsyncClientEvent("WantPlayerMenu")]
        public async Task ShowPlayerMenu(PlayerModel p)
        {
            Random rnd = new Random();
            List<PMenu> pList = new List<PMenu>();
            foreach(PlayerModel t in Alt.GetAllPlayers())
            {
                PMenu tM = new PMenu();
                tM.ID = t.sqlID;
                tM.name = t.characterName.Replace("_", " ");
                tM.level = t.characterLevel;
                int ping = ((int)t.Ping / 2) + rnd.Next(0, 10);
                tM.ping = ping;
                pList.Add(tM);
            }

            int PD = 0; int FD = 0; int News = 0; int Admins = 0; int Helpers = 0; int taxi = 0;

            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (t.HasData(EntityData.PlayerEntityData.FDDuty))
                    ++FD;

                if (t.HasData(EntityData.PlayerEntityData.PDDuty))
                    ++PD;

                if (t.HasData(EntityData.PlayerEntityData.NewsDuty))
                    ++News;

                if (t.adminLevel >= 5 && t.adminLevel <= 8)
                    ++Admins;

                if (t.adminLevel > 1 && t.adminLevel < 5)
                    ++Helpers;

                if(t.Vehicle != null)
                    if(t.Vehicle.Driver == t)
                    {
                        VehModel v = (VehModel)t.Vehicle;
                        if (v.jobId == ServerGlobalValues.JOB_Taxi)
                            ++taxi;
                    }
                        
                       
            }

            string json = JsonConvert.SerializeObject(pList.OrderBy(x => x.ID));
            p.EmitLocked("ShowPMenu", json, Alt.GetAllPlayers(), await Database.DatabaseMain.GetServerRecord(), Admins, Helpers, PD, FD, News, 0, taxi);
            return;
        }

        [AsyncClientEvent("UpdatePlayerMenu")]
        public void UpdatePlayerMenu(PlayerModel p)
        {

        }
    }
}
