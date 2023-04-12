using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Data;
using outRp.Utils;
using outRp.Models;
using outRp.OtherSystem.Textlabels;
using System.Numerics;
using AltV.Net.Elements.Entities;
using outRp.Chat;
using AltV.Net.Enums;
using outRp.Globals;
using ReTimerLib;
using outRp.OtherSystem;
using System.Xml.Linq;

namespace outRp.Plants
{
    /// <summary>
    /// 种植田事件
    /// </summary>
    public class ServerEvent : IScript
    {
        // 种植田初始化
        public static void InitPlants()
        {
            IList<Position> plantsPositions = PlantUtil.GetPositions();
            for (int i = 0; i < plantsPositions.Count; i++)
            {
                PlantUtil.plants.Add(new PlantModel(i, plantsPositions[i], PlantUtil.TypeList.None, 0, false, default));
                PlayerLabel text = TextLabelStreamer.Create($"~o~[种植田]~n~~w~空闲中", plantsPositions[i]);
                text.SetServerData("plant", i);
            }
            IDictionary<string, Position> points = PlantUtil.Getpositions();
            points.TryGetValue("取出点", out Position vehSpawnTrigger);
            MarkerStreamer.Create(MarkerTypes.MarkerTypeVerticalCylinder, vehSpawnTrigger - new Position(0, 0, 1), new Vector3(1, 1, 1));
            TextLabelStreamer.Create($"~o~[种植]~n~~w~取出点", vehSpawnTrigger);
            points.TryGetValue("归还点", out Position vehBackTrigger);
            MarkerStreamer.Create(MarkerTypes.MarkerTypeVerticalCylinder, vehBackTrigger - new Position(0, 0, 1), new Vector3(1, 1, 1));
            TextLabelStreamer.Create($"~o~[种植]~n~~w~归还点", vehBackTrigger);
            points.TryGetValue("取水壶点", out Position waterTrigger);
            MarkerStreamer.Create(MarkerTypes.MarkerTypeVerticalCylinder, waterTrigger - new Position(0, 0, 1), new Vector3(1, 1, 1));
            TextLabelStreamer.Create($"~o~[种植]~n~~w~取水壶点", waterTrigger);
            Alt.Log($"[种植田] - 加载种植田 {plantsPositions.Count} 个");
        }

        // 种植田按键触发事件
        public static async Task<bool> PlantsKeyTrigger(PlayerModel player)
        {
            IDictionary<string, Position> points = PlantUtil.Getpositions();
            points.TryGetValue("取出点", out Position vehSpawnTrigger);
            if (vehSpawnTrigger.Distance(player.Position) <= 2.5f && player.Dimension == 0)
            {
                if (player.HasData("job:plant:duty"))
                {
                    MainChat.SendErrorChat(player, "您已处于工作状态，无法再次开始工作！");
                    return true;
                }
                points.TryGetValue("刷出点", out Position vehRefreshTrigger);
                VehModel veh = (VehModel)Alt.CreateVehicle(VehicleModel.Tractor2, vehRefreshTrigger, new Rotation(0, 0, 0));
                veh.owner = player.sqlID;
                veh.SetData("job:plant", player.sqlID);
                player.SetData("job:plant:duty", true);
                MainChat.SendInfoChat(player, "您成功生成您的工作载具，已停在您的旁边。");
                return true;
            }
            points.TryGetValue("归还点", out Position vehBackTrigger);
            if (vehBackTrigger.Distance(player.Position) <= 2.5f && player.Dimension == 0)
            {
                if (!player.HasData("job:plant:duty"))
                {
                    MainChat.SendErrorChat(player, "您不处于工作状态，无法归还工作载具！");
                    return true;
                }
                if (!player.IsInVehicle)
                {
                    MainChat.SendErrorChat(player, "您不在任何载具上，无法归还工作载具！");
                    return true;
                }
                VehModel vehModel = (VehModel)player.Vehicle;
                if (!vehModel.HasData("job:plant"))
                {
                    MainChat.SendErrorChat(player, "此载具不属于该工作，无法归还工作载具！");
                    return true;
                }
                vehModel.GetData("job:plant", out int someone);
                if (someone != player.sqlID)
                {
                    MainChat.SendErrorChat(player, "此载具不属于你，无法归还工作载具！");
                    return true;
                }
                vehModel.Remove();
                player.DeleteData("job:plant:duty");
                MainChat.SendInfoChat(player, "您已成功归还载具！");
                return true;
            }
            points.TryGetValue("取水壶点", out Position waterTrigger);
            if (waterTrigger.Distance(player.Position) <= 2.5f && player.Dimension == 0)
            {
                if (GetWaterTool(player)) return true;
                if (ReturnWaterTool(player)) return true;
            }
            if (await PickupPlant(player))
                return true;
            if (await GiveWater(player))
                return true;
            return false;
        }

        public static async Task<bool> TryToPlant(PlayerModel player, PlantUtil.TypeList type)
        {
            PlantModel plant = PlantUtil.CheckPlayerNearestPlant(player);
            if (plant == null)
            {
                MainChat.SendErrorChat(player, "您附近没有任何种植田，请靠近后再使用！");
                return false;
            }
            if (plant.Type != PlantUtil.TypeList.None)
            {
                MainChat.SendErrorChat(player, "该种植田不处于空闲状态，请寻找一处空地！");
                return false;
            }
            if (player.Frozen)
            {
                MainChat.SendErrorChat(player, "您正处于忙碌状态，请稍后再试！");
                return false;
            }
            MainChat.SendInfoChat(player, "正在种植作物，请稍后...");
            player.Frozen = true;
            GlobalEvents.PlayAnimation(player, new string[] { "anim@amb@clubhouse@tutorial@bkr_tut_ig3@", "machinic_loop_mechandplayer" }, 15, -1);
            await Task.Delay(2000); // 2000ms延迟
            if (!player.Exists) return false;
            player.Frozen = false;
            GlobalEvents.StopAnimation(player);
            PlantUtil.SetPlants(plant.Id, type, PlantUtil.InitPlantTimer, false);
            IDictionary<PlantUtil.TypeList, string> nameList = PlantUtil.objectNameList;
            nameList.TryGetValue(plant.Type, out string name);
            MainChat.SendInfoChat(player, $"您已种植完成 {name}！");
            return true;
        }

        public static async Task<bool> PickupPlant(PlayerModel player)
        {
            PlantModel plant = PlantUtil.CheckPlayerNearestPlant(player);
            if (plant == null) return false;
            if (plant.Type == PlantUtil.TypeList.None) return false;
            if (plant.Timer > 0)
            {
                MainChat.SendErrorChat(player, "该作物暂时没有成熟，请稍后！");
                return false;
            }
            if (plant.LessWaterStatus)
            {
                MainChat.SendErrorChat(player, "该作物正处于缺水状态，请先浇水！");
                return false;
            }
            if (player.Frozen)
            {
                MainChat.SendErrorChat(player, "您正处于忙碌状态，请稍后再试！");
                return false;
            }
            MainChat.SendInfoChat(player, "正在收获作物，请稍后...");
            player.Frozen = true;
            GlobalEvents.PlayAnimation(player, new string[] { "anim@amb@clubhouse@tutorial@bkr_tut_ig3@", "machinic_loop_mechandplayer" }, 15, -1);
            await Task.Delay(2000); // 2000ms延迟
            if (!player.Exists) return false;
            player.Frozen = false;
            GlobalEvents.StopAnimation(player);
            IDictionary<PlantUtil.TypeList, string> list = PlantUtil.objectNameList;
            list.TryGetValue(plant.Type, out string name);
            var item = Items.LSCitems.Find(x => x.name.Equals(name));
            ServerItems addItem = new ServerItems();
            addItem = item;
            int amount = new Random().Next(2, 21);
            if (await Inventory.AddInventoryItem(player, addItem, amount))
            {
                PlantUtil.SetPlants(plant.Id, PlantUtil.TypeList.None, 0, true);
                MainChat.SendErrorChat(player, $"您成功采摘 {name} x {amount}！");
            } else
            {
                MainChat.SendErrorChat(player, "您的背包已满，请稍后再试！");
            }
            return true;
        }

        // 浇水
        public static async Task<bool> GiveWater(PlayerModel player)
        {
            PlantModel plant = PlantUtil.CheckPlayerNearestPlant(player);
            if (plant == null) return false;
            if (!plant.LessWaterStatus) return false;
            if (player.Frozen)
            {
                MainChat.SendErrorChat(player, "您正处于忙碌状态，请稍后再试！");
                return false;
            }
            if (!player.HasData("job:plant:water"))
            {
                MainChat.SendErrorChat(player, "您没有拿水壶，请先去拿水壶！");
                return false;
            }
            MainChat.SendInfoChat(player, "正在浇水，请稍后...");
            player.Frozen = true;
            GlobalEvents.PlayAnimation(player, new string[] { "timetable@jimmy@doorknock@", "knockdoor_idle" }, 15, -1);
            await Task.Delay(2000); // 2000ms延迟
            if (!player.Exists) return false;
            player.Frozen = false;
            GlobalEvents.StopAnimation(player);
            ReturnWaterTool(player);
            PlantUtil.SetPlants(plant.Id, plant.Type, plant.Timer, false);
            MainChat.SendErrorChat(player, $"您成功为该作物浇水！");
            return true;
        }

        // 获取水壶
        public static bool GetWaterTool(PlayerModel player)
        {
            if (player.HasData("job:plant:water")) return false;
            long attachId = AttachmentSystem.AddAttach(player, new AttachmentSystem.ObjectModel() { Model = "bkr_prop_money_pokerbucket", boneIndex = "36029", xPos = 0.15f, yPos = 0, zPos = -0.043f, xRot = 15, yRot = 80, zRot = 150});
            player.SetData("job:plant:water", attachId);
            MainChat.SendInfoChat(player, "您成功拿取水桶！");
            return true;
        }

        // 归还水壶
        public static bool ReturnWaterTool(PlayerModel player)
        {
            if (!player.HasData("job:plant:water")) return false;
            player.GetData("job:plant:water", out long attachId);
            AttachmentSystem.deleteAttachsWithID(player, attachId);
            player.DeleteData("job:plant:water");
            return true;
        }
    }
}
