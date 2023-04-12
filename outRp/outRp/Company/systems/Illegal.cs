using AltV.Net;
using outRp.Models;
using System.Threading.Tasks;
using outRp.Globals;
using Newtonsoft.Json;
using outRp.Core;
using outRp.Chat;
using outRp.OtherSystem.Textlabels;
using System.Linq;
using AltV.Net.Resources.Chat.Api;
using outRp.OtherSystem;

namespace outRp.Company.systems
{
    class Illegal : IScript
    {
        [Command("gstealstock")]
        public static async Task COM_StealStock(PlayerModel p)
        {
            if (p.HasData("StealStock:Type")) { MainChat.SendErrorChat(p, "[错误] 您手上已经有偷取的货物了, 请先将它装进车内. (/putstock)"); return; }
            var steal = Component_System.serverComponents.Where(x => x.ObjectPos.Distance(p.Position) < 5).FirstOrDefault();
            if(steal == null) { MainChat.SendErrorChat(p, "[错误] 附近没有货物点!"); return; }

            if(Jobs.TotalPD() < 5) { MainChat.SendErrorChat(p, "[错误] 至少需要有 5 名 执法人员在线才可以偷取材料."); return; }

            await PoliceAlertSystem(p, steal); // İhbar sistemü.

            if(steal.SecurityLevel > 4) { MainChat.SendErrorChat(p, "[错误] 此仓库货物点防盗等级过高."); return; }

            if(p.factionId == 0) { MainChat.SendErrorChat(p, "[错误] 您没有组织!"); return; }

            if(steal.Stock_1 < 100) { MainChat.SendErrorChat(p, "[错误] 货物点库存不足100!"); return; }

            p.SetData("StealStock:Type", steal.Type);
            steal.Stock_1 -= 100;
            PlayerLabel lbl = TextLabelStreamer.GetDynamicTextLabel(steal.TextLabelID);
            lbl.Text = Component_System.GetComponentDisplayName(steal);
            lbl.Font = 0;
            await steal.Update();

            Animations.PlayerAnimation(p, "carrybox3");

            MainChat.SendInfoChat(p, "[!] 成功从仓库偷取了 100 数量的货物.");
            return;
        }

        [Command("putstock")]
        public static void COM_LoadStealStockToCar(PlayerModel p)
        {
            if (!p.HasData("StealStock:Type")) { MainChat.SendErrorChat(p, "[错误] 您手上没有偷取的货物!"); return; }

            VehModel veh = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if(veh == null) { MainChat.SendErrorChat(p, "[错误] 附近没有装载赃物货物的车辆!"); return; }

            if (veh.HasData("StealStock:Type")) { MainChat.SendErrorChat(p, "[错误] 此车内已有赃物货物."); return; }
            GlobalEvents.SetVehicleTag(veh, "~b~[装载]~o~~n~类型: " + Component_System.GetComponentTypeName(p.lscGetdata<int>("StealStock:Type")));

            veh.SetData("StealStock:Type", p.lscGetdata<int>("StealStock:Type"));
            p.DeleteData("StealStock:Type");
            Animations.PlayerStopAnimation(p);

            MainChat.SendErrorChat(p, "[!] 成功把偷取的货物装进车内!");
            return;
        }

        [Command("getstock")]
        public static void COM_GetStockFromCar(PlayerModel p)
        {
            VehModel veh = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if(veh == null) { MainChat.SendErrorChat(p, "[错误] 附近没有车辆"); return; }
            if (!veh.HasData("StealStock:Type")) { MainChat.SendErrorChat(p, "[错误] 此车内没有赃物货物!"); return; }
            if (p.HasData("StealStock:Type")) { MainChat.SendErrorChat(p, "[错误] 您手上已经有偷取的货物了, 请先将它装进车内. (/putstock)!"); return; }

            p.SetData("StealStock:Type", veh.lscGetdata<int>("StealStock:Type"));
            veh.DeleteData("StealStock:Type");

            GlobalEvents.ClearVehicleTag(veh);

            MainChat.SendErrorChat(p, "[!] 成功取出车内的赃物货物.");
            Animations.PlayerAnimation(p, "carrybox3");
            return;            
        }


        [Command("putstockwarehouse")]
        public static async Task COM_AddStockToComponent(PlayerModel p)
        {
            if (!p.HasData("StealStock:Type")) { MainChat.SendErrorChat(p, "[错误] 您手上没有偷取的货物!"); return; }
            var comp = Component_System.serverComponents.Where(x => x.ObjectPos.Distance(p.Position) < 5).FirstOrDefault();
            if(comp == null) { MainChat.SendErrorChat(p, "[错误] 附近没有可存入货物的仓库货物点."); return; }
            if(comp.Type != p.lscGetdata<int>("StealStock:Type")) { MainChat.SendErrorChat(p, "[错误] 此仓库货物点不支持您手上的赃物货物!"); return; }

            comp.Stock_1 += 100;
            await comp.Update();         

            p.DeleteData("StealStock:Type");
            Animations.PlayerStopAnimation(p);

            MainChat.SendInfoChat(p, "[!] 成功往仓库存入了 100 数量的货物.");
            return;            
        }



        public static async Task PoliceAlertSystem(PlayerModel p, Models.Components comp)
        {
    
            var company = await Database.BusinessDatabase.GetCompany(comp.OwnerBusiness);
            if (company == null)
                return;

            var MDC = AddMDCCall(p);
            string MDC_Call = " MDC 内容 " + MDC.reason;

            switch (comp.SecurityLevel)
            {
                case 2:
                    foreach(PlayerModel case4 in Alt.GetAllPlayers())
                    {
                        if (case4.HasData(EntityData.PlayerEntityData.PDDuty))
                        {
                            MainChat.SendInfoChat(case4, "{E4AA10}[注意]{87C6D5} 一位不明人士被举报, 涉嫌在 " + company.Name + " 的仓库附近进行非法活动. ");
                        }
                    }
                    return;

                case 1:
                    foreach(PlayerModel case5 in Alt.GetAllPlayers())
                    {
                        if (case5.HasData(EntityData.PlayerEntityData.PDDuty))
                        {
                            MainChat.SendInfoChat(case5, "{E4AA10}[注意]{87C6D5} 一位不明人士被举报, 涉嫌在 " + company.Name + " 的仓库附近进行非法活动.");
                        }
                    }

                    return;

                default:
                    foreach(PlayerModel case6 in Alt.GetAllPlayers())
                    {
                        if (case6.HasData(EntityData.PlayerEntityData.PDDuty))
                        {
                            MainChat.SendInfoChat(case6, "{E4AA10}[注意]{87C6D5} 一位不明人士涉嫌在 " + company.Name + " 的仓库附近进行非法活动.<br>监控有记录到嫌疑人的外貌(( 嫌疑人: " + p.characterName.Replace("_", " ") + " ))");
                        }

                    }
                    return;
            }
        }

        public static OtherSystem.LSCsystems.MDCEvents.MDCCalls AddMDCCall(PlayerModel p)
        {
            var check = OtherSystem.LSCsystems.MDCEvents.PDcalls.Find(x => x.pos.Distance(p.Position) < 30);
            if(check == null)
            {
                check = new OtherSystem.LSCsystems.MDCEvents.MDCCalls()
                {
                    callerName = "匿名",
                    callNumber = 911,
                    pos = p.Position,
                    reason = "入室盗窃 - " + JsonConvert.SerializeObject(p.Position),
                };
                OtherSystem.LSCsystems.MDCEvents.PDcalls.Add(check);
            }

            return check;
        }
    }
}
