using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Enums;
using outRp.Chat;
using outRp.Core;
using outRp.Globals;
using outRp.Models;
using outRp.OtherSystem.NativeUi;
using outRp.OtherSystem.Textlabels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net.Data;
using AltV.Net.Resources.Chat.Api;
using outRp.Tutorial;

namespace outRp.OtherSystem.LSCsystems
{
    public class VehicleTaxPayment : IScript
    {
        public static void LoadTaxSystem()
        {
            TextLabelStreamer.Create("~b~[~w~税务部门~b~]~n~~w~缴纳税/罚款指令: ~g~/paytax", ServerGlobalValues.TaxPayPos, center: true, font: 0, streamRange: 5);
            GlobalEvents.blipModel factBlip = new GlobalEvents.blipModel();
            factBlip.blipname = "GovermentMain";
            factBlip.category = 2;
            factBlip.label = "税务部门";
            factBlip.position = ServerGlobalValues.TaxPayPos;
            factBlip.sprite = 419;
            GlobalEvents.serverBlips.Add(factBlip);
            Alt.Log("加载 税务部门系统.");
        }
        [Command("paytax")]
        public async Task COM_PayVehicleTax(PlayerModel p)
        {
            if (p.Position.Distance(ServerGlobalValues.TaxPayPos) > 5) { MainChat.SendErrorChat(p, "[错误] 您不在税务部门"); return; }
            List<VehModel> pVehicles = new List<VehModel>();
            foreach (VehModel v in Alt.GetAllVehicles())
            {
                bool vB = await Vehicle.VehicleMain.GetKeysQuery(p, v);
                if (vB) { pVehicles.Add(v); }
            }
            if (pVehicles.Count <= 0) { MainChat.SendErrorChat(p, "[错误] 无效车辆."); return; }
            List<GuiMenu> gMenu = new List<GuiMenu>();
            foreach (VehModel cv in pVehicles)
            {
                if (cv.fine >= 50)
                {
                    var model = (VehicleModel)cv.Model;
                    GuiMenu payingVeh = new GuiMenu { name = model.ToString() + " 车牌号码: " + cv.NumberplateText, triger = "PayVehicleTax:SelectVehicle", value = "__" + cv.sqlID.ToString() };
                    gMenu.Add(payingVeh);
                }
            }

            GuiMenu close = GuiEvents.closeItem;
            gMenu.Add(close);
            Gui y = new Gui()
            {
                image = "https://vignette.wikia.nocookie.net/gtawiki/images/2/21/Lossantos_seal.png",
                guiMenu = gMenu,
                color = "#00AAFF"
            };
            y.Send(p);
        }
        [AsyncClientEvent("PayVehicleTax:SelectVehicle")]
        public void PayVehicleTax_SelectVehicle(PlayerModel p, string value)
        {
            value = value.Replace("_", "");
            int vehId = Int32.Parse(value);
            VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(vehId);
            p.SetData("PayinTaxVeh", v.sqlID);
            if (v == null) { GuiEvents.GuiClose(p); return; }
            List<GuiMenu> gMenu = new List<GuiMenu>();
            if (v.fine > 50)
            {
                GuiMenu payingVeh = new GuiMenu { name = "支付: $50", triger = "PayVehicleTax:ThisVehicle", value = "50" };
                gMenu.Add(payingVeh);
            }
            if (v.fine > 100)
            {
                GuiMenu payingVeh2 = new GuiMenu { name = "支付: $100", triger = "PayVehicleTax:ThisVehicle", value = "100" };
                gMenu.Add(payingVeh2);
            }
            if (v.fine > 500)
            {
                GuiMenu payingVeh3 = new GuiMenu { name = "支付: $500", triger = "PayVehicleTax:ThisVehicle", value = "500" };
                gMenu.Add(payingVeh3);
            }

            GuiMenu payingVeh4 = new GuiMenu { name = "支付全部 $" + v.fine.ToString(), triger = "PayVehicleTax:ThisVehicle", value = v.fine.ToString() };
            gMenu.Add(payingVeh4);

            GuiMenu close = GuiEvents.closeItem;
            gMenu.Add(close);
            Gui y = new Gui()
            {
                image = "https://vignette.wikia.nocookie.net/gtawiki/images/2/21/Lossantos_seal.png",
                guiMenu = gMenu,
                color = "#00AAFF"
            };
            y.Send(p);
        }

        [AsyncClientEvent("PayVehicleTax:ThisVehicle")]
        public void payVehicleTax_ThisVehicle(PlayerModel p, int wp)
        {
            //value = value.Replace("_", "");
            //int wp = Int32.Parse(value);
            if (p.cash < wp) { MainChat.SendErrorChat(p, CONSTANT.ERR_MoneyNotEnought); return; }
            int vsql = p.lscGetdata<int>("PayinTaxVeh");
            VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(vsql);
            if (v == null) { return; }
            if (v.fine < wp) { MainChat.SendErrorChat(p, "[错误] 车辆税低于应缴金额."); return; }
            v.fine -= wp;
            p.cash -= wp;
            p.updateSql();
            if (v.fine < (v.price / 10))
            {
                v.towwed = false;
            }
            v.settings.fines = new();
            v.Update();
            var model = (VehicleModel)v.Model;
            GuiEvents.GuiClose(p);
            GlobalEvents.ShowNotification(p, "税务部门 ~n~~g~模型: ~y~" + model.ToString() + "~n~~g~车牌号码: ~y~" + v.NumberplateText + "~g~~n~剩余待付税款: " + v.fine.ToString());
            
            if (p.isFinishTut == 28)
            {
                p.isFinishTut = 29;
                p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}手机系统");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}嘿, 您成功缴纳了税款, 您的车辆暂时不会面对税务问题了!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}现在, 我们需要买一部手机!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}我们已经标记了商店, 这是一个可以购买手机和其他物品的地点!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}服务器分布了许多商店, 但每家商店出售的物品和价格是不同的!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}现在, 前往下一个教程点吧!");
                GlobalEvents.CheckpointCreate(p, TutorialMain.TutPos[9], 1, 2, new Rgba(255, 0, 0, 0), "Tutorial:Run", "29");
            }   
        }

        [Command("cekilenaraclarim")]
        public static void COM_GetTowwedVehicles(PlayerModel p)
        {

        }

    }
}
