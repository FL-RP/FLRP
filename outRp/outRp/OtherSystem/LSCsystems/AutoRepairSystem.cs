using System.Collections.Generic;
using AltV.Net; using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Resources.Chat.Api;
using outRp.Models;
using outRp.Chat;
using outRp.Globals;
using outRp.OtherSystem.Textlabels;
using Newtonsoft.Json;

namespace outRp.OtherSystem.LSCsystems
{
    public class AutoRepairSystem : IScript
    {
        public class RepairModel
        {
            public Position repairPos { get; set; } = new Position(0, 0, 0);
            public int factionID { get; set; } = 0;
            public int Price { get; set; } = 0;
            public int textLabelID { get; set; } = 0;
            public int Dimension { get; set; } = 0;
        }

        public static List<RepairModel> repairSystem = new List<RepairModel>();

        public static void LoadRepairs(string data)
        {
            repairSystem = JsonConvert.DeserializeObject<List<RepairModel>>(data);

            foreach(RepairModel r in repairSystem)
            {
                r.textLabelID = (int)TextLabelStreamer.Create("~b~[~w~维修车辆~b~]~n~~w~指令: ~g~/fixcar~n~~w~价格: ~g~$" + r.Price, r.repairPos, dimension: r.Dimension, streamRange: 3, font: 0).Id;
            }
        }

        [Command("fixcar")]
        public static void COM_AutoRepair(PlayerModel p)
        {
            if(p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] 您必须在车内."); return; }
            VehModel v = (VehModel)p.Vehicle;

            RepairModel currStation = repairSystem.Find(x => x.repairPos.Distance(p.Position) < 5 && x.Dimension == p.Dimension);
            if(currStation == null) { MainChat.SendErrorChat(p, "[错误] 附近没有维修车辆点."); return; }

            if((v.factionId != currStation.factionID || p.factionId != currStation.factionID) && currStation.factionID != 0) { MainChat.SendErrorChat(p, "[错误] 无权使用."); return; } 

            if(p.cash < currStation.Price) { MainChat.SendErrorChat(p, CONSTANT.ERR_MoneyNotEnought); return; }
            p.cash -= currStation.Price;
            p.updateSql();

            v.NetworkOwner.EmitLocked("Vehicle:Repair", v.Id);
            MainChat.SendInfoChat(p, "[!] 已维修车辆.");
            Prometheus.RepairStationUsage(1, false);
            return;
        }

        
    }
}
