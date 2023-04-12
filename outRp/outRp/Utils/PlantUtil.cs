using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using outRp.Models;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using outRp.OtherSystem.Textlabels;
using static System.Net.Mime.MediaTypeNames;
using ReTimerLib;
using AltV.Net;
using ReTimerLib.Model;

namespace outRp.Utils
{
    /// <summary>
    /// 种植系统工具类
    /// </summary>
    public class PlantUtil
    {
        public enum TypeList
        {
            None = 0,
            Growing = 2,
            Polo = 3,
            Cabbage = 4,
            Pumpkin = 5,
            Tomato = 6,
        }

        public const int InitPlantTimer = 1200;

        // 种植作物Object列表
        public static IDictionary<TypeList, string> objectList = new Dictionary<TypeList, string>()
        {
            { TypeList.Growing, "prop_aloevera_01" },
            { TypeList.Polo, "prop_plant_palm_01a" },
            { TypeList.Cabbage, "prop_veg_crop_03_cab" },
            { TypeList.Pumpkin, "prop_veg_crop_03_pump" },
            { TypeList.Tomato, "prop_veg_crop_02" }
        };

        // 种植作物名称列表
        public static IDictionary<TypeList, string> objectNameList = new Dictionary<TypeList, string>()
        {
            { TypeList.None, "空闲中" },
            { TypeList.Growing, "生长中" },
            { TypeList.Polo, "菠萝" },
            { TypeList.Cabbage, "白菜" },
            { TypeList.Pumpkin, "南瓜" },
            { TypeList.Tomato, "西红柿" }
        };

        // 种植载具位置坐标
        public static IDictionary<string, Position> positions = new Dictionary<string, Position>()
        {
            { "取出点", new Position(2029.806640625f, 4980.27685546875f, 42.085693359375f) },
            { "刷新点", new Position(2006.6109619140625f, 4987.23974609375f, 41.3948974609375f)},
            { "归还点", new Position(2019.4285888671875f, 4976.00439453125f, 41.192626953125f)},
            { "取水壶点", new Position(2051.261475f, 4980.61962890625f, 39.1875f) }
        };

        public static IDictionary<string, Position> Getpositions()
        {
            return positions;
        }

        // 种植点坐标
        public static IList<Position> plantsPosition = new List<Position>()
        {
            new Position(2045.7890625f, 4966.76025390625f, 41.07470703125f),//1
            new Position(2049.07250976562f,4963.33203125f,41.024169921875f),//2
            new Position(2052.369140625f,4960.2197265625f,41.041015625f),//3
            new Position(2055.73193359375f,4956.80419921875f,41.024169921875f),//4
            new Position(2059.10766601562f,4953.4814453125f,41.00732421875f),//5
            new Position(2062.4306640625f,4950.11865234375f,41.057861328125f),//6
            new Position(2065.81982421875f,4946.82177734375f,41.041015625f),//7
            new Position(2068.56274414062f,4944.263671875f,41.057861328125f),//8
            new Position(2067.27026367187f,4942.81298828125f,41.091552734375f),//9
            new Position(2064.46142578125f,4945.3056640625f,41.091552734375f),//10
            new Position(2061.08569335937f,4948.60205078125f,41.091552734375f),//11
            new Position(2057.72314453125f,4951.74072265625f,41.07470703125f),//12
            new Position(2054.42626953125f,4955.1826171875f,41.091552734375f),//13
            new Position(2051.03735351562f,4958.53173828125f,41.091552734375f),//14
            new Position(2047.70104980468f,4961.9208984375f,41.091552734375f),//15
            new Position(2044.49670410156f,4965.0595703125f,41.1083984375f),//16
            new Position(2043.05932617187f,4963.63525390625f,41.1083984375f),//17
            new Position(2045.69665527343f,4961.06396484375f,41.07470703125f),//18
            new Position(2048.91430664062f,4957.89892578125f,41.091552734375f),//19
            new Position(2052.25048828125f,4954.49658203125f,41.07470703125f),//20
            new Position(2055.45483398437f,4951.34521484375f,41.07470703125f),//21
            new Position(2058.68579101562f,4948.28564453125f,41.091552734375f),//22
            new Position(2062.0615234375f,4944.88330078125f,41.07470703125f),//23
            new Position(2065.35815429687f,4941.73193359375f,41.091552734375f),//24
            new Position(2064.3427734375f,4940.0703125f,41.07470703125f),//25
            new Position(2060.96704101562f,4943.03759765625f,41.091552734375f),//26
            new Position(2057.630859375f,4946.4130859375f,41.091552734375f),//27
            new Position(2053.701171875f,4950.3427734375f,41.091552734375f),//28
            new Position(2050.53637695312f,4953.52099609375f,41.091552734375f),//29
            new Position(2047.17358398437f,4956.90966796875f,41.091552734375f),//30
            new Position(2044.37805175781f,4959.62646484375f,41.091552734375f),//31
            new Position(2042.13623046875f,4961.90771484375f,41.07470703125f),//32
            new Position(2040.40881347656f,4960.720703125f,41.1083984375f),//33
            new Position(2043.61315917968f,4957.6220703125f,41.091552734375f),//34
            new Position(2047.00219726562f,4954.23291015625f,41.07470703125f),//35
            new Position(2050.80004882812f,4950.46142578125f,41.057861328125f),//36
            new Position(2054.62426757812f,4946.53173828125f,41.057861328125f),//37
            new Position(2057.9208984375f,4943.14306640625f,41.07470703125f),//38
            new Position(2060.62426757812f,4940.5712890625f,41.091552734375f),//39
            new Position(2062.86596679687f,4938.7119140625f,41.1083984375f),//40
        };

        // 返回种植田坐标
        public static IList<Position> GetPositions()
        {
            return plantsPosition;
        }

        // 种植田
        public static IList<PlantModel> plants = new List<PlantModel>();

        // 获取种植田信息
        public static PlantModel GetPlant(int Id)
        {
            PlantModel plant = null;
            foreach (PlantModel p in plants)
            {
                if (p.Id != Id) continue;
                plant = p;
                break;
            }
            return plant;
        }

        // 设置种植田信息
        public static void SetPlants(int Id, TypeList type, int Timer, bool water)
        {
            Alt.Log($"{Id} - {type} - {Timer} - {water}");
            foreach (PlantModel plant in plants)
            {
                if (plant.Id != Id) continue;
                double totalSeconds = Math.Floor(plant.startTime.Subtract(DateTime.Now).Duration().TotalSeconds);
                if (totalSeconds < InitPlantTimer && totalSeconds % 500 == 0 && Timer > 0 && totalSeconds > 0) // 判断是否需要浇水了
                {
                    SetPlants(plant.Id, type, Timer, true);
                    ReTimer.Service.CreateTimer($"plant_lesswater_{plant.Id}_{DateTime.Now.Ticks}", 200, $"{plant.Id}", false);
                    return;
                }
                if (plant.Timer <= 0 && type == TypeList.None && !water) // 当水源充足的菜地被摘除 删除obj 重置当前状态
                {
                    LProp p = null;
                    foreach (LProp prop in PropStreamer.GetAllProp())
                    {
                        if (!prop.HasServerData("plant")) continue;
                        if (prop.GetServerData<int>("plant") != plant.Id) continue;
                        p = prop;
                        break;
                    }
                    if (p != null) p.Delete();
                    SetPlants(plant.Id, plant.Type, InitPlantTimer, true);
                    return;
                }
                plant.Type = type;
                plant.Timer = Timer;
                plant.LessWaterStatus = water;
                foreach (PlayerLabel text in TextLabelStreamer.GetAllDynamicTextLabels())
                {
                    if (!text.HasServerData("plant")) continue;
                    if (text.GetServerData<int>("plant") != plant.Id) continue;
                    text.Text = $"~o~[种植田]~n~";
                    objectNameList.TryGetValue(plant.Type, out string plantName);
                    text.Text += $"{plantName}~n~";
                    if (plant.Timer > 0) text.Text += $"距离成熟还有 {plant.Timer} 秒~n~";
                    if (plant.LessWaterStatus) text.Text += $"~r~!!!缺水!!!";
                    break;
                }
                if (plant.Timer >= InitPlantTimer && plant.Type != TypeList.None) // 当菜地刚开始种植时 种植作物Growing
                {
                    objectList.TryGetValue(TypeList.Growing, out string plantModel);
                    LProp prop = PropStreamer.Create(plantModel, plant.Pos - new Position(0, 0, 1f), new System.Numerics.Vector3(0, 0, 0));
                    plant.startTime = DateTime.Now;
                    ReTimer.Service.CreateTimer($"plant_{plant.Id}", 1, $"{plant.Id}", true);
                    prop.SetData("plant", plant.Id);
                } else if (plant.Timer <= 0 && plant.Type != TypeList.None) // 当菜地完成种植 刷新
                {
                    foreach (LProp prop in PropStreamer.GetAllProp())
                    {
                        if (!prop.HasServerData("plant")) continue;
                        if (prop.GetServerData<int>("plant") != plant.Id) continue;
                        objectList.TryGetValue(plant.Type, out string plantModel);
                        prop.Model = plantModel;
                        break;
                    }
                } else if (plant.Timer <= 0 && plant.Type == TypeList.None && plant.LessWaterStatus) // 当水源不足的菜地被摘除，则彻底删除
                {
                    LProp p = null;
                    foreach (LProp prop in PropStreamer.GetAllProp())
                    {
                        if (!prop.HasServerData("plant")) continue;
                        if (prop.GetServerData<int>("plant") != plant.Id) continue;
                        p = prop;
                        break;
                    }
                    if (p != null) p.Delete();
                }
                break;
            }
        }

        public static PlantModel CheckPlayerNearestPlant(IPlayer player)
        {
            if (player.Dimension != 0) return null; //防止不同空间进行操作
            PlantModel plant = null;
            Position currentPostion = player.Position;
            foreach (PlantModel p in plants)
            {
                if (currentPostion.Distance(p.Pos) > 1.5f) continue;
                plant = p;
                break;
            }
            return plant;
        }
    }
}
