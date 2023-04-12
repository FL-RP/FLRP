using AltV.Net;
using AltV.Net.Data;
using System.Collections.Generic;
using outRp.OtherSystem.Textlabels;
using System.Numerics;

namespace outRp.OtherSystem.ImprovedJobs
{
    public class Truck : IScript
    {
        public class Station
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public Position Position { get; set; }
            public Position TrailerPosition { get; set; }
            public List<Stock> Stocks { get; set; }

            public ulong Marker_ID { get; set; } = 0;

            public Station(int id, string name, Position pos, Position trailerpos)
            {
                ID = id;
                Name = name;
                Position = pos;
                TrailerPosition = trailerpos;
                Stocks = new List<Stock>();
            }
        }

        public class Stock
        {
            public int Type { get; set; }
            public string Name { get; set; }
            public int Total { get; set; } = 500;
            public Stock(int type, string name, int total = 500)
            {
                Type = type;
                Name = name;
                Total = total;
            }
        }

        public static List<Station> Stations = new List<Station>()
        {
            new Station(1, "洛圣都国际机场 - 肯塔机库", new Position(-760.7868f, -2629.2131f, 13.92f), new Position(-754.981f, -2601.732f, 13.8117f))
            {
                Stocks = new List<Stock>()
                {
                    new Stock(1, "木材"),
                    new Stock(2, "混凝土"),
                    new Stock(3, "石油"),
                    new Stock(4, "食品")
                }
            },
            new Station(2, "埃尔布罗 - 天启机库", new Position(1569, -2130, 78), new Position(1555, -2141, 77))
            {
                Stocks = new List<Stock>()
                {
                    new Stock(1, "木材"),
                    new Stock(2, "混凝土"),
                    new Stock(5, "金属")
                }
            },
            new Station(3, "加里 - 月露机库", new Position(720.989f, 1291.4506f, 360.29443f), new Position(741.0725f, 1297.1472f, 360.2944f))
            {
                Stocks = new List<Stock>()
                {
                    new Stock(2, "木材"),
                    new Stock(3, "石油"),
                    new Stock(5, "金属")
                }
            },
            new Station(4, "大丛林 - 沃森农场", new Position(-112.61539f, 1882.022f, 197.32312f), new Position(-126.514f, 1886.874f, 197.64319f))
            {
                Stocks = new List<Stock>()
                {
                    new Stock(1, "木材"),
                    new Stock(4, "食品"),
                    new Stock(5, "金属")
                }
            },
            new Station(5, "红杉灯 - 彼得森建筑集团", new Position(1129.2f, 2124.83008f, 55.53186f), new Position(1102.8528f, 2124.3823f, 53.4593f))
            {
                Stocks = new List<Stock>()
                {
                    new Stock(1, "木材"),
                    new Stock(2, "混凝土"),
                    new Stock(3, "石油"),
                    new Stock(4, "食品"),
                    new Stock(5, "金属")
                }
            },
            new Station(6, "葡萄籽 - 海隆农场", new Position(2159.2615f, 4789.8857f, 41.34436f), new Position(2122.945f, 4800.4614f, 40.990f))
            {
                Stocks = new List<Stock>()
                {
                    new Stock(1, "木材"),
                    new Stock(2, "混凝土"),
                    new Stock(3, "石油"),
                    new Stock(4, "食品"),
                    new Stock(5, "金属")
                }
            },
            new Station(7, "奇利亚山 - 永永锯木厂", new Position(-567.6923f, 5253.3364f, 70.477f), new Position(-592.33844f, 5288.3076f, 70.20801f))
            {
                Stocks = new List<Stock>()
                {
                    new Stock(1, "木材"),
                    new Stock(3, "石油"),
                    new Stock(4, "食品")
                }
            },
            new Station(8, "帕莱托湾 - 哈德逊垃圾场", new Position(-195.75824f, 6265.556f, 31.4871f), new Position(-192.58022f, 6269.0835f, 31.487183f))
            {
                Stocks = new List<Stock>()
                {
                    new Stock(2, "木材"),
                    new Stock(3, "石油"),
                    new Stock(4, "食品"),
                    new Stock(5, "金属")
                }
            }
        };

        public void StartUp()
        {
            foreach(var station in Stations)
            {
                station.Marker_ID = MarkerStreamer.Create(MarkerTypes.MarkerTypeDallorSign, station.Position, new Vector3(0.3f, 0.3f, 0.3f), color: new Rgba(128, 250, 0, 255), faceCamera: true, streamRange: 10).Id;
            }
        }
    }
}
