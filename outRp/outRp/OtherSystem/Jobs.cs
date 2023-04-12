using System;
using System.Collections.Generic;
using System.Numerics;
using AltV.Net;
using AltV.Net.Data;
using System.Threading.Tasks;
using outRp.Models;
using outRp.Globals;

namespace outRp.OtherSystem
{
    public class BizJobs
    {
        public static void LoadJobs()
        {

            GlobalEvents.serverBlips.Add(new GlobalEvents.blipModel { blipname = "junkyardBlip", category = 2, position = GlobalJobs.TConst.junkYardMarkerPos, label = "捡破烂" , Short = true, sprite = 527, number = 0 });
        }
    }
    public class GlobalJobs : IScript
    {
        public class TConst
        {
            public static Vector3 defaultBlipScale = new Vector3(1f, 1f, 1f);
            //Truck
            public static Position junkYardMarkerPos = new Position(-427.62198f, -1722.1318f, 19.10254f);

            public static List<Position> junkyardPos = new List<Position>() 
            { 
                new Position(-580.7868f, -1692.1582f, 19.119385f),
                new Position(-518.95386f, -1633.1604f, 17.78833f),
                new Position(-469.02856f, -1725.8901f, 18.681274f)
            };

            public static List<ServerItems> junkyardItems = new List<ServerItems>() 
            {
                new ServerItems{ ID = 1, type = 12 ,name = "生锈的螺丝刀", data = "50" },
                new ServerItems{ ID = 2, type = 12 ,name = "生锈的锤子", data = "20"},
            };
        }
        public static async Task StartSearchTrunk(PlayerModel p)
        {
            foreach(Position x in TConst.junkyardPos)
            {
                if(x.Distance(p.Position) <= 25f)
                {
                    GlobalEvents.ProgresBar(p, "东张西望...", 10);
                    GlobalEvents.GameControls(p,false);
                    GlobalEvents.PlayAnimation(p, ServerAnimations.searchJunkyard, 0);
                    await Task.Delay(10000);
                    if (p.Exists)
                        return;
                    GlobalEvents.GameControls(p, true);
                    GlobalEvents.StopAnimation(p);                    
                    Random random = new Random();
                    int coin = random.Next(1, 2);
                    ServerItems item = null;
                    switch (coin)
                    {
                        case 1:
                            item = TConst.junkyardItems.Find(x => x.ID == 1);
                            item.price += coin;
                            bool case1Add = await Inventory.AddInventoryItem(p, item, 1);
                            if (case1Add) { GlobalEvents.notify(p, 2, "发现物品: " + item.name + "<br> 预估价值:" + item.price); Inventory.UpdatePlayerInventory(p); return; }
                            else { GlobalEvents.notify(p, 3, "您的背包满了!"); return; }

                        default:
                            GlobalEvents.notify(p, 3, "没有任何发现!");
                            break;
                    }
                }
            }
            return;
        }

    }
}
