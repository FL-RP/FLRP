using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Globals;
using outRp.Models;
using outRp.OtherSystem.NativeUi;
using outRp.OtherSystem.Textlabels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;

namespace outRp.OtherSystem.LSCsystems
{
    public class DriverLicenses : IScript
    {
        public class LicenseConst
        {
            public static Position lisenceGetPos = new Position(-1283.341f, -567.6923f, 31.706177f);
            public static Position lisecenRefreshPos = new Position(439, -981, 30);
            public static Position licenseRewritePos = new Position(442, -981, 30);

            public static Position PDPayFinesPos = new Position(-1099.1208f, -840.8044f, 19.001465f);

            public static int lisenceCosh = 500;
            public static int lisenceRefreshCost = 200;
            public static int licenseRewriteCost = 5000;
        }

        public static void LoadLicenseSystem()
        {
            TextLabelStreamer.Create("~y~[办理驾驶证]~n~~w~指令: ~g~/buydlic~n~~w~费用: " + LicenseConst.lisenceCosh, LicenseConst.lisenceGetPos, streamRange: 5, font: 0);

            GlobalEvents.blipModel factBlip = new GlobalEvents.blipModel();
            factBlip.blipname = "licenseBlip";
            factBlip.category = 2;
            factBlip.label = "机动车辆管理局";
            factBlip.position = LicenseConst.lisenceGetPos;
            GlobalEvents.serverBlips.Add(factBlip);

            TextLabelStreamer.Create("~y~[更新驾驶证]~n~~w~指令: ~g~/updatedlic~n~~w~费用: " + LicenseConst.lisenceRefreshCost, LicenseConst.lisecenRefreshPos, streamRange: 5, font: 0);
            TextLabelStreamer.Create("~y~[补办驾驶证]~n~~w~指令: ~g~/remakedlic~n~~w~费用: " + LicenseConst.licenseRewriteCost, LicenseConst.licenseRewritePos, streamRange: 5, font: 0);
            TextLabelStreamer.Create("~y~[支付罚款]~n~~w~指令: ~g~/payfine", LicenseConst.PDPayFinesPos, streamRange: 5, font: 0);
        }

        [Command("buydlic")]
        public async Task COM_GetDriverLicense(PlayerModel p)
        {
            if (p.Position.Distance(LicenseConst.lisenceGetPos) > 5) { MainChat.SendErrorChat(p, "[错误] 您不在 办理驾驶证 点附近"); return; }
            CharacterSettings cSet = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            if (cSet.driverLicense != null) { MainChat.SendErrorChat(p, "[错误] 您已经有驾照了, 如果您的驾驶证丢了, 请联系警察局办理."); return; }
            if (p.cash < LicenseConst.lisenceCosh) { MainChat.SendErrorChat(p, CONSTANT.ERR_MoneyNotEnought); return; }
            ServerItems lcItem = Items.LSCitems.Find(x => x.ID == 17);
            lcItem.name = "驾驶证 " + p.sqlID.ToString();
            lcItem.data = p.sqlID.ToString();
            bool succes = await Inventory.AddInventoryItem(p, lcItem, 1);
            if (!succes) { MainChat.SendErrorChat(p, "[错误] 您的库存满了."); return; }
            p.cash -= LicenseConst.lisenceCosh;
            DriverLicense newLicense = new DriverLicense();
            newLicense.licenseDate = DateTime.Now;
            newLicense.licenseDate = newLicense.licenseDate.AddDays(20);
            newLicense.finePoint = 0;
            cSet.driverLicense = newLicense;
            p.settings = JsonConvert.SerializeObject(cSet);
            await p.updateSql();
            MainChat.SendInfoChat(p, "> 已支付 $" + LicenseConst.lisenceCosh + " 办理驾驶证, 请妥善保管.");
            return;
        }
        [Command("updatedlic")]
        public static void COM_DriverLicenseAddTime(PlayerModel p)
        {
            if (p.Position.Distance(LicenseConst.lisecenRefreshPos) > 5) { MainChat.SendErrorChat(p, "[错误] 您不在 更新驾驶证 点附近"); return; }
            if (p.cash < LicenseConst.lisenceRefreshCost) { MainChat.SendErrorChat(p, CONSTANT.ERR_MoneyNotEnought); return; }
            CharacterSettings cSet = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            if (cSet.driverLicense == null) { MainChat.SendErrorChat(p, "[错误] 您没有办理驾驶证, 请先办理驾驶证."); return; }
            if (cSet.driverLicense.licenseDate > DateTime.Now.AddDays(60)) { MainChat.SendInfoChat(p, "> 至少已办理 60 天才能更新您的驾驶证有效期."); return; }
            p.cash -= LicenseConst.lisenceRefreshCost;
            cSet.driverLicense.licenseDate = cSet.driverLicense.licenseDate.AddDays(20);
            p.settings = JsonConvert.SerializeObject(cSet);
            p.updateSql();
            MainChat.SendInfoChat(p, "> 您的驾驶证有效期已延长5年(OOC的20天).");
        }

        [Command("remakedlic")]
        public async Task COM_DriverLicenseRewrite(PlayerModel p)
        {
            if (p.Position.Distance(LicenseConst.licenseRewritePos) > 5) { MainChat.SendErrorChat(p, "[错误] 您不在 补办驾驶证 点附近."); return; }
            if (p.cash < LicenseConst.licenseRewriteCost) { MainChat.SendErrorChat(p, CONSTANT.ERR_MoneyNotEnought); return; }
            CharacterSettings cSet = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            if (cSet.driverLicense == null) { MainChat.SendErrorChat(p, "[错误] 您没有办理驾驶证, 请先办理驾驶证."); return; }
            cSet.driverLicense.finePoint = 0;
            p.settings = JsonConvert.SerializeObject(cSet);
            ServerItems lcItem = Items.LSCitems.Find(x => x.ID == 17);
            lcItem.name = "驾驶证 " + p.sqlID.ToString();
            lcItem.data = p.sqlID.ToString();
            bool succes = await Inventory.AddInventoryItem(p, lcItem, 1);
            if (!succes) { MainChat.SendErrorChat(p, "[错误] 您的库存满了."); return; }
            p.cash -= LicenseConst.licenseRewriteCost;
            await p.updateSql();
            MainChat.SendInfoChat(p, "> 已支付 $" + LicenseConst.licenseRewriteCost + " 补办驾驶证, 请妥善保管.");
            return;
        }

        [Command("payfine")]
        public static void COM_PayFines(PlayerModel p)
        {
            List<GuiMenu> gMenu = new List<GuiMenu>();
            CharacterSettings set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            if (set.fines.Count <= 0) { MainChat.SendInfoChat(p, "您没有待付罚款."); return; }

            foreach (PlayerFineModel f in set.fines)
            {
                GuiMenu fine = new GuiMenu { name = "罚款金额: $" + f.finePrice, triger = "PayFinePlayer", value = f.reason, popup = f.reason };
                gMenu.Add(fine);
            }


            GuiMenu close = GuiEvents.closeItem;

            gMenu.Add(close);
            Gui y = new Gui()
            {
                image = "https://i.hizliresim.com/0qYbXM.png",
                guiMenu = gMenu,
                color = "#2f6b8d"
            };
            y.Send(p);
        }

        [AsyncClientEvent("PayFinePlayer")]
        public void Client_PayFine(PlayerModel p, string value)
        {
            CharacterSettings set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            PlayerFineModel fine = set.fines.Find(x => x.reason == value);
            if (fine == null) { MainChat.SendErrorChat(p, "[错误] 无效罚款."); return; }
            if (p.cash < fine.finePrice) { MainChat.SendErrorChat(p, CONSTANT.ERR_MoneyNotEnought); return; }
            p.cash -= fine.finePrice;
            set.fines.Remove(fine);
            p.settings = JsonConvert.SerializeObject(set);
            p.updateSql();
            MainChat.SendInfoChat(p, "> 已支付罚款.");
            GuiEvents.GuiClose(p);
            return;
        }
    }
}
