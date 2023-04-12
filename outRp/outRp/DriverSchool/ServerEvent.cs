using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using outRp.Utils;
using AltV.Net;
using AltV.Net.Data;
using outRp.OtherSystem.Textlabels;
using outRp.Models;
using System.Numerics;
using outRp.Chat;
using outRp.Globals;

namespace outRp.DriverSchool
{
    public class ServerEvent : IScript
    {
        public static void Init()
        {
            IDictionary<DriverSchoolUtil.LicenseType, Position> points = DriverSchoolUtil.points;
            IDictionary<DriverSchoolUtil.LicenseType, string> typeNames = DriverSchoolUtil.typeNames;
            foreach (KeyValuePair<DriverSchoolUtil.LicenseType, Position> point in points)
            {
                MarkerStreamer.Create(MarkerTypes.MarkerTypeVerticalCylinder, point.Value - new Position(0, 0, 1), new Vector3(1, 1, 1));
                typeNames.TryGetValue(point.Key, out string name);
                TextLabelStreamer.Create($"~o~[驾校]~n~~w~{name}驾照考试", point.Value);
            }
        }

        // 互动键触发
        public static async Task<bool> InteractionTrigger(PlayerModel player)
        {
            IDictionary<DriverSchoolUtil.LicenseType, Position> points = DriverSchoolUtil.points;
            foreach (KeyValuePair<DriverSchoolUtil.LicenseType, Position> point in points)
            {
                if (player.Position.Distance(point.Value) <= 2.5f && player.Dimension == 0)
                {
                    StartPaperTest(player, point.Key);
                    return true;
                }
            }
            return false;
        }

        // 理论考
        public static void StartPaperTest(PlayerModel player, DriverSchoolUtil.LicenseType type)
        {
            if (player.HasData("driverSchool:PaperTest") || DriverSchoolUtil.GetPlayerTestingVeh(player) != null || player.HasData("DS:PointStep")) return;
            player.Emit("DS:ShowDS");
            player.SetData("driverSchool:PaperTest", type);
        }

        // 结束理论考
        public static async Task EndPaperTest(PlayerModel player)
        {
            if (!player.HasData("driverSchool:PaperTest")) return;
            player.GetData("driverSchool:PaperTest", out DriverSchoolUtil.LicenseType type);
            await StartRoadTest(player);
        }

        // 路考
        public static async Task StartRoadTest(PlayerModel player)
        {
            if (!player.HasData("driverSchool:PaperTest")) return;
            player.GetData("driverSchool:PaperTest", out DriverSchoolUtil.LicenseType type);
            player.DeleteData("driverSchool:PaperTest");
            IDictionary<DriverSchoolUtil.LicenseType, string> models = DriverSchoolUtil.models;
            models.TryGetValue(type, out string model);
            VehModel veh = (VehModel)Alt.CreateVehicle(model, DriverSchoolUtil.vehSpawnPos, new Rotation(0, 0, 0));
            veh.maxFuel = 100;
            veh.currentFuel = 50;
            veh.owner = player.sqlID;
            MainChat.SendInfoChat(player, "考试车已刷新，请您在30s内上车，继续完成考试！");
            GlobalEvents.CheckpointCreate(player, DriverSchoolUtil.checkCoords[0], 1, 2, new Rgba(255, 0, 0, 1), "DS:ArrivalCheck", "2");
            player.SetData("DS:PointStep", 1);
            player.SetData("driverSchool:RoadTest", type);
            veh.SetData("DriverTesting", player);
            await Task.Delay(30000);
            if (player.Vehicle != veh && veh.Exists)
            {
                MainChat.SendErrorChat(player, "您没有按时上车！考试失败！");
                return;
            }
        }

        // 进入检查点
        public static async Task ArrivalCheck(PlayerModel player, string point)
        {
            if (!player.HasData("DS:PointStep") || !player.HasData("driverSchool:RoadTest")) return;
            player.GetData("DS:PointStep", out int CurrentPoint);
            int.TryParse(point, out int TargetPoint);
            if (CurrentPoint + 1 != TargetPoint) return;
            if (!player.IsInVehicle || player.Vehicle != DriverSchoolUtil.GetPlayerTestingVeh(player)) return;
            VehModel veh = (VehModel)player.Vehicle;
            CurrentPoint += 1;
            switch (CurrentPoint) //提示
            {
                case 4:
                    MainChat.SendInfoChat(player, "此点需要倒车入库");
                    break;
                case 11:
                case 12:
                case 13:
                case 14:
                    player.Frozen = true;
                    veh.Frozen = true;
                    MainChat.SendInfoChat(player, "此处红绿灯需要停止10秒，住宅区速度禁止超过每小时80km/h");
                    await Task.Delay(10000);
                    if (player.Exists) player.Frozen = false;
                    if (veh.Exists) veh.Frozen = false;
                    break;
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                case 30:
                    MainChat.SendInfoChat(player, "高速限速150km/h");
                    break;
                case 32:
                    MainChat.SendInfoChat(player, "此点需要倒车入库");
                    break;
            }
            if (CurrentPoint >= DriverSchoolUtil.checkCoords.Count) //跳出考试 考试成功
            {
                IDictionary<DriverSchoolUtil.LicenseType, string> typeNames = DriverSchoolUtil.typeNames;
                player.GetData("driverSchool:RoadTest", out DriverSchoolUtil.LicenseType type);
                typeNames.TryGetValue(type, out string name);
                MainChat.SendInfoChat(player, $"您考试成功，恭喜您获得了 {name} 驾照！");
                EndTesting(player);
                return;
            }
            player.SetData("DS:PointStep", CurrentPoint);
            GlobalEvents.CheckpointCreate(player, DriverSchoolUtil.checkCoords[CurrentPoint - 1], 1, 2, new Rgba(255, 0, 0, 1), "DS:ArrivalCheck", CurrentPoint.ToString());
        }

        // 中途下车
        public static async Task<bool> OnPlayerLeaveVehicle(VehModel veh, PlayerModel player)
        {
            if (!veh.HasData("DriverTesting")) return false;
            veh.GetData("DriverTesting", out PlayerModel someone);
            if (someone != player) return false;
            MainChat.SendInfoChat(player, "您已离开考试车，请在 10s 内上车，否则将车辆会消失，需重新考试！");
            await Task.Delay(10000);
            if (player.Exists) if (player.Vehicle != veh) EndTesting(player);
            return true;
        }

        // 中途下线
        public static void OnPlayerDisconnect(PlayerModel player)
        {
            EndTesting(player);
        }

        // 结束道路考试状态
        public static void EndTesting(PlayerModel player)
        {
            VehModel veh = DriverSchoolUtil.GetPlayerTestingVeh(player);
            if (player.Exists)
            {
                if (veh != null) veh.Destroy();
                if (player.HasData("DS:PointStep")) player.DeleteData("DS:PointStep");
                if (player.HasData("driverSchool:PaperTest")) player.DeleteData("driverSchool:PaperTest");
                if (player.HasData("DS:PointStep")) player.DeleteData("DS:PointStep");
                if (player.HasData("driverSchool:RoadTest")) player.DeleteData("driverSchool:RoadTest");
            }
        }
    }
}
