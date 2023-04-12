using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using outRp.Models;

namespace outRp.DriverSchool
{
    public class ClientEvent : IScript
    {
        [AsyncClientEvent("DS:SucExam")]
        public static async Task DS_SucExam(PlayerModel player)
        {
            await ServerEvent.EndPaperTest(player);
        }

        [AsyncClientEvent("DS:ArrivalCheck")]
        public static async Task DS_ArrivalCheck(PlayerModel player, string step)
        {
            await ServerEvent.ArrivalCheck(player, step);
        }

        [ClientEvent("DS:ClosePaperTest")]
        public static void DS_ClosePaperTest(PlayerModel player)
        {
            ServerEvent.EndTesting(player);
        }
    }
}
