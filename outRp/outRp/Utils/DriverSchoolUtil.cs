using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Data;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;
using outRp.Models;

namespace outRp.Utils
{
    public class DriverSchoolUtil
    {
        public enum LicenseType
        {
            MotorCycle = 0,
            Truck = 1,
            StandardCar = 2,
        };
        public static Position vehSpawnPos = new Position(-881.5648193359375f, -2049.86376953125f, 9.2960205078125f);
        public static IDictionary<LicenseType, Position> points = new Dictionary<LicenseType, Position>()
        {
            { LicenseType.MotorCycle, new Position(-916.72088f, -2040.47473f, 9.41394f) },
            { LicenseType.Truck, new Position(-914.03076f, -2037.547241f, 9.397094f) },
            { LicenseType.StandardCar, new Position(-915.52087f, -2039.12963f, 9.397094f) }
        };
        public static IDictionary<LicenseType, string> models = new Dictionary<LicenseType, string>()
        {
            { LicenseType.MotorCycle, "ruffian" },
            { LicenseType.Truck, "mule3" },
            { LicenseType.StandardCar, "dilettante2" }
        };
        public static IDictionary<LicenseType, string> typeNames = new Dictionary<LicenseType, string>()
        {
            { LicenseType.MotorCycle, "摩托车" },
            { LicenseType.Truck, "货车" },
            { LicenseType.StandardCar, "轿车" }
        };
        public static IList<Position> checkCoords = new List<Position>()
        {
            new Position(-943.912109375f, -2120.228515625f, 8.7568359375f), //1
            new Position(-858.3824462890625f, -2053.55615234375f, 8.7230224609375f), //2
            new Position(-801.2703247070312f, -2025.098876953125f, 8.4197998046875f), //3
            new Position(-775.002197265625f, -2044.5626220703125f, 8.3524169921875f), //4：此点需要倒车入库
            new Position(-715.92529296875f, -2071.5166015625f, 8.3355712890625f), //5
            new Position(-650.2681274414062f, -2064.46142578125f, 8.3187255859375f), //6
            new Position(-662.5318603515625f, -2006.940673828125f, 6.86962890625f), //7
            new Position(-231.65274047851562f, -2201.90771484375f, 9.801513671875f), //8
            new Position(-10.338462829589844f, -2123.89453125f, 9.801513671875f), //9
            new Position(-237.7978057861328f, -1829.3143310546875f, 29.2799072265625f), //10
        /*11-14号点需要强行停止10秒，在打字文本框输出“此处红绿灯需要停止10秒，住宅区速度禁止超过每小时80km/h”，
        并且这段期间内监测车辆速度，如果超过80km/h，提示超速一次，如果全过程超速3次，则需回到起点重新考试。*/
            new Position(-146.63735961914062f, -1751.169189453125f, 29.5662841796875f), //11   
            new Position(-41.52527618408203f, -1626.6197509765625f, 28.824951171875f), //12
            new Position(58.5494499206543f, -1523.195556640625f, 28.757568359375f), //13
            new Position(136.41758728027344f, -1413.5736083984375f, 28.7744140625f), //14
            new Position(129.4681396484375f, -1373.3406982421875f, 28.80810546875f), //15
            new Position(-351.5208740234375f, -1425.191162109375f, 28.9765625f), //16
        /*17-30号点需要在打字文本框输出“高速限速150km/h”，
        这段期间内监测车辆速度，如果超过150km/h，提示超速一次，如果全过程超速3次，则需回到起点重新考试。*/
            new Position(-455.6439514160156f, -1448.6636962890625f, 28.892333984375f), //17
            new Position(-915.3626098632812f, -1826.6505126953125f, 34.857177734375f), //18
            new Position(46.03516387939453f, -2633.195556640625f, 23.197021484375f), //19
            new Position(1197.94287109375f, -1884.6724853515625f, 36.120849609375f), //20
            new Position(1150.4571533203125f, -1745.77587890625f, 35.24462890625f), //21
            new Position(1024.971435546875f, -1754.914306640625f, 35.025634765625f), //22
            new Position(489.1780090332031f, -1648.5626220703125f, 28.892333984375f), //23
            new Position(300.8835144042969f, -1490.5450439453125f, 28.80810546875f), //24
            new Position(181.43736267089844f, -1403.6439208984375f, 28.80810546875f), //25
            new Position(-210.1714324951172f, -1414.6680908203125f, 30.779541015625f), //26
            new Position(-477.4813232421875f, -1409.6834716796875f, 28.892333984375f), //27
            new Position(-556.931884765625f, -1183.054931640625f, 18.192626953125f), //28
            new Position(-676.905517578125f, -1461.982421875f, 9.767822265625f), //29
            new Position(-1020.06591796875f, -2096.874755859375f, 13.0029296875f), //30
            new Position(-950.5186767578125f, -2142.751708984375f, 8.4703369140625f), //31
            new Position(-882.4219970703125f, -2049.57373046875f, 8.7568359375f), //32：此点需要倒车入库
        };

        // 查找玩家考试车
        public static VehModel GetPlayerTestingVeh(PlayerModel player)
        {
            VehModel vehicle = null;
            foreach (VehModel veh in Alt.GetAllVehicles())
            {
                if (!veh.HasData("DriverTesting")) continue;
                veh.GetData("DriverTesting", out PlayerModel someone);
                if (someone != player) continue;
                vehicle = veh;
            }
            return vehicle;
        }
    }
}
