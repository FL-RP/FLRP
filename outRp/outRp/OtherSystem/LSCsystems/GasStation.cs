using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Resources.Chat.Api;
using outRp.Chat;
using outRp.Globals;
using outRp.Models;
using outRp.Tutorial;

namespace outRp.OtherSystem.LSCsystems
{
    public class GasStation : IScript
    {

        [Command("fillcar")]
        public static void COM_BuyGas(PlayerModel p, params string[] args)
        {
            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v == null) { MainChat.SendErrorChat(p, "[错误] 附近没有车辆."); return; }
            if (v.currentFuel + 10 >= v.maxFuel) { MainChat.SendErrorChat(p, "[错误] 这辆车的油箱已经满了!"); return; }

            p.EmitLocked("GasStation:Start", v.Id);
        }


        [AsyncClientEvent("GasStation:Buy")]
        public void WantToBuyGas(PlayerModel p, VehModel v)
        {
            if (p.Ping > 250)
                return;
            if (v.Driver != null) { MainChat.SendErrorChat(p, "[错误] 应无人在车辆的驾驶座!"); return; }
            if (v.EngineOn) { MainChat.SendErrorChat(p, "[错误] 应先关闭车辆的发动机."); return; }
            if (p.cash < 25) { MainChat.SendErrorChat(p, CONSTANT.ERR_MoneyNotEnought); return; }
            if (v.Position.Distance(p.Position) > 5) { MainChat.SendErrorChat(p, "[错误] 您离车太远."); return; }
            if (v.currentFuel + 5 > v.maxFuel)
            {
                MainChat.SendErrorChat(p, "[错误] 这辆车的油箱已经满了.");
                p.EmitLocked("GasStation:Over");
                return;
            }

            p.cash -= 25;
            p.updateSql();

            v.currentFuel += 5;
            GlobalEvents.NativeNotifyVehicle(v, "~w~正在加油中...油箱 ~g~+5~w~Lt");
            
            if (p.isFinishTut == 21)
            {
                p.isFinishTut = 22;
                p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}车辆加油");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}嘿, 您已经学会了如何给车辆加油!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}现在请前往下一个教程点");
                GlobalEvents.CheckpointCreate(p, TutorialMain.TutPos[6], 1, 1, new Rgba(255, 0, 0, 0), "Tutorial:Run", "22");
            }               
            return;
        }
    }
}
