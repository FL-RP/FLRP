using AltV.Net;
using AltV.Net.Elements.Entities;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using AltV.Net.Enums;
using ReTimerLib;
using AltV.Net.Data;
using Timer = BasicTimer.Timer;
using System.Globalization;
using System.Linq;
using outRp.Utils;
using outRp.Models;

namespace outRp.ReTimerEvent
{
    public class ReTimerEvents
    {
        public static void OnReTimerEvent(ReTimerLib.Model.Timer timer)
        {
            if (timer == null) return;
            if (timer.RouterKey.Contains("plant_"))
            {
                int.TryParse(timer.Message, out int plantId);
                PlantModel plant = PlantUtil.GetPlant(plantId);
                if (plant.Timer > 0) // 大于0时始终计时，扣时间
                {
                    PlantUtil.SetPlants(plant.Id, plant.Type, plant.Timer - 1, plant.LessWaterStatus);
                }
                else // 小于等于0 植株成熟，进入收割状态
                {
                    PlantUtil.SetPlants(plant.Id, plant.Type, 0, plant.LessWaterStatus);
                    ReTimer.Service.ClearTimer($"plant_{plant.Id}");
                }
                return;
            }
            if (timer.RouterKey.Contains("plant_lesswater_"))
            {
                int.TryParse(timer.Message, out int plantId);
                PlantModel plant = PlantUtil.GetPlant(plantId);
                if (plant.LessWaterStatus) return;
                PlantUtil.SetPlants(plant.Id, PlantUtil.TypeList.None, 0, true);
                return;
            }
        }
    }
}
