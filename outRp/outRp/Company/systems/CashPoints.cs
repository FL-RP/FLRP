using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Resources.Chat.Api;
using outRp.Models;
using Newtonsoft.Json;
using outRp.Core;
using outRp.Chat;

namespace outRp.Company.systems
{
    public class CashPoints : IScript
    {
        [Command("poslog")]
        public void POS_LOG(PlayerModel p, params string[] args)
        {
            if(args.Length <= 0) { MainChat.SendErrorChat(p, "[错误]"); return; }
            Core.Logger.WriteLogData(Logger.logTypes.lelorLog, JsonConvert.SerializeObject(p.Position) + " | " + string.Join("_", args));
            MainChat.SendInfoChat(p, "[收据日志]");
            return;
        }
        #region GasPumps
        public class GasPump
        {
            public int ID { get; set; }
            public int OwnerID { get; set; }
            public int TotalGas { get; set; }
            public int GasPrice { get; set; }
            public Position MainPos { get; set; }
            public Position BuyPosition { get; set; }
        }


        /*public static List<GasPump> gasPumps = new List<GasPump>()
        {
            new GasPump{ ID = 1, GasPrice = 0, OwnerID = 0, TotalGas = 0, MainPos = new Position(1697.5912f, -1432.3253f, 112.53479f), BuyPosition = new Position(1683.8374f, -1462.5231f, 112.45056f)},
            new GasPump{ ID = 2, GasPrice = 0, OwnerID = 0, TotalGas = 0, MainPos = new Position(1688.7032f, -1451.7362f, 112.231445f), BuyPosition = new Position(1658.8748f, -1521.7979f, 112.6864f)},
            new GasPump{ ID = 3, GasPrice = 0, OwnerID = 0, TotalGas = 0, MainPos = new Position(1657.7803f, -1528.9714f, 112.6864f), BuyPosition = new Position(1571.4462f, -1586.5319f, 91.48938f)},
            new GasPump{ ID = 4, GasPrice = 0, OwnerID = 0, TotalGas = 0, MainPos = new Position(1562.466f, -1600.3912f, 90.57947f), BuyPosition = new Position(1508.7296f, -1733.2748f, 78.5824f)},
            new GasPump{ ID = 5, GasPrice = 0, OwnerID = 0, TotalGas = 0, MainPos = new Position(1511.3143f, -1720.2461f, 78.93628f), BuyPosition = new Position(1504.1274f, -1749.9033f, 78.49817f)},
            new GasPump{ ID = 6, GasPrice = 0, OwnerID = 0, TotalGas = 0, MainPos = new Position(1509.1912f, -1743.6f, 78.49817f), BuyPosition = new Position(1570.6813f, -1854.989f, 92.77002f)},
            new GasPump{ ID = 7, GasPrice = 0, OwnerID = 0, TotalGas = 0, MainPos = new Position(1553.0769f, -1851.6132f, 92.50037f), BuyPosition = new Position(1586.5714f, -1852.4572f, 94.337036f)},
            new GasPump{ ID = 8, GasPrice = 0, OwnerID = 0, TotalGas = 0, MainPos = new Position(1579.2924f, -1851.7979f, 94f), BuyPosition = new Position(1661.9736f, -1862.8748f, 108.86157f)},
            new GasPump{ ID = 9, GasPrice = 0, OwnerID = 0, TotalGas = 0, MainPos = new Position(1680.0791f, -1852.444f, 108.42346f), BuyPosition = new Position(1656.5934f, -1842.1978f, 109.24902f)},
            new GasPump{ ID = 10, GasPrice = 0, OwnerID = 0, TotalGas = 0, MainPos = new Position(1678.022f, -1832.9934f, 110.02417f), BuyPosition = new Position(1704.4484f, -1914.5934f, 115.31506f)},
            new GasPump{ ID = 11, GasPrice = 0, OwnerID = 0, TotalGas = 0, MainPos = new Position(1698.1055f, -1923.4418f, 115.3656f), BuyPosition = new Position(1710.4879f, -1680.3693f, 112.551636f)},
        };*/


        #endregion
    }
}
