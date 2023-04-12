using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using outRp.Chat;
using outRp.Core;
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
    public class ATM : IScript
    {
        public class BankModel
        {
            public int ID { get; set; }
            public int owner { get; set; }
            public int accountNo { get; set; }
            public int accountType { get; set; } = 0;
            public int accountMoney { get; set; } = 0;
            public string otherSettings { get; set; } = "[]";
            public string usageLogs { get; set; } = "[]";
            public Task Create() => CreateBank(this);
            public Task Update() => Database.DatabaseMain.UpdateBankAccount(this);
            public Task Delete() => Database.DatabaseMain.DeleteBankAccount(this);
            public static async Task CreateBank(BankModel b)
            {
                int a = await Database.DatabaseMain.CreateBankAccount(b);
                b.accountNo = 18200000 + a;
                b.ID = a;
                await Database.DatabaseMain.UpdateBankAccount(b);
            }
        }

        public class BankConst
        {
            public static List<Position> bankPos = new List<Position>()
            {
                new Position(150.06593f, -1040.7957f, 29.364136f),
            };
            public static string BankString = CONSTANT.COM_Bank;
        }
        public static void LoadBankSystem()
        {
            foreach (Position x in BankConst.bankPos)
            {
                TextLabelStreamer.Create("~g~/~w~" + BankConst.BankString, x, streamRange: 5);
            }
        }

        public static async Task<List<BankModel>> FindBankAccount(string Key, string Value)
        {
            return await Database.DatabaseMain.FindBankAccount(Key, Value);
        }

        [Command(CONSTANT.COM_Bank)]
        public async Task COM_Bank(PlayerModel p)
        {
            bool canUse = false;
            foreach (Position x in BankConst.bankPos)
            {
                if (p.Position.Distance(x) < 2 && GlobalEvents.GetNearestPlayer(p) == null) { canUse = true; break; }
            }
            if (!canUse)
            {
                MainChat.SendErrorChat(p, "[错误] 有人正在使用附近的银行柜台, 请等待他人使用完毕(或不在银行柜台附近).");
                return;
            }

            await Bank_MainPage(p);

            return;
        }

        [AsyncClientEvent("Bank:MainPage")]
        public async Task Bank_MainPage(PlayerModel p)
        {
            if (p.Ping > 250)
                return;
            List<BankModel> bankAccounts = await FindBankAccount("owner", p.sqlID.ToString());
            GuiMenu createAccount = new GuiMenu { name = "<i class='circular inverted edit outline icon'></i> 创建帐户", triger = "Bank:CreateAccount:Step1", value = "xx" };
            //GuiMenu AccountInfo = new GuiMenu { name = "<i class='circular inverted calendar alternate icon'></i> Hesaplarım", triger = "xx", value = "xx" };
            GuiMenu getBankCard = new GuiMenu { name = "<i class='circular inverted address card icon'></i> 签发银行卡", triger = "Bank:CreateCreditCard:Step1", value = "xx" };
            GuiMenu close = GuiEvents.closeItem;

            List<GuiMenu> gMenu = new List<GuiMenu>();
            gMenu.Add(createAccount);
            if (bankAccounts.Count > 0)
            {
                //gMenu.Add(AccountInfo); 
                gMenu.Add(getBankCard);
            }
            gMenu.Add(close);
            Gui y = new Gui()
            {
                image = "https://ih0.redbubble.net/image.941083207.9340/flat,550x550,075,f.u3.jpg",
                guiMenu = gMenu,
                color = "#4AC27D"
            };
            y.Send(p);
        }

        [AsyncClientEvent("Bank:CreateAccount:Step1")]
        public void Bank_CreateAccount_Step1(PlayerModel p)
        {
            GuiMenu createAccountPayDay = new GuiMenu { name = "工资帐户 200$", triger = "Bank:CreateAccount:Step2", value = "1" };
            //GuiMenu createAccountCurency = new GuiMenu { name = "Vadeli Mevduat Hesabı 1500$", triger = "Bank:CreateAccount:Step2", value = "2" };
            GuiMenu createAccountNormal = new GuiMenu { name = "储蓄帐户 500$", triger = "Bank:CreateAccount:Step2", value = "3" };
            GuiMenu Back = new GuiMenu { name = "后退", triger = "Bank:MainPage", value = "0" };
            List<GuiMenu> gMenu = new List<GuiMenu>();
            gMenu.Add(createAccountPayDay);
            //gMenu.Add(createAccountCurency);
            gMenu.Add(createAccountNormal);
            gMenu.Add(Back);
            Gui y = new Gui()
            {
                image = "https://ih0.redbubble.net/image.941083207.9340/flat,550x550,075,f.u3.jpg",
                guiMenu = gMenu,
                color = "#4AC27D",
            };
            y.Send(p);
        }

        [AsyncClientEvent("Bank:CreateAccount:Step2")]
        public async Task Bank_CreateAccount_Step2(PlayerModel p, int select)
        {
            //Alt.Log(select.ToString());
            List<BankModel> bankAccounts = await FindBankAccount("owner", p.sqlID.ToString());
            BankModel b = new BankModel();
            switch (select)
            {
                case 1:
                    if (bankAccounts.Count > 0)
                    {
                        BankModel case1Check = bankAccounts.Find(x => x.accountType == 1);
                        if (case1Check != null) { MainChat.SendInfoChat(p, "{4AC27D}[富力银行]{FFFFFF} 很抱歉, 我们无法为您开设第二个工资账户, 因为您已经有一个工资账户了."); return; }
                    }
                    if (p.cash <= 200) { MainChat.SendErrorChat(p, CONSTANT.ERR_MoneyNotEnought); return; }
                    b.accountType = 1;
                    b.accountMoney = 0;
                    b.owner = p.sqlID;
                    p.cash -= 200;
                    await p.updateSql();
                    await b.Create();
                    MainChat.SendInfoChat(p, "{4AC27D}[富力银行]{FFFFFF} 您开设了属于 " + p.characterName.Replace("_", " ") + " 的工资帐户.");
                    await Bank_MainPage(p);
                    return;

                case 2:
                    if (bankAccounts.Count > 0)
                    {
                        int limit = 0;
                        foreach (var c2Check in bankAccounts)
                        {
                            if (c2Check.accountType == 2) { limit++; }
                        }
                        if (limit >= 3) { MainChat.SendInfoChat(p, "{4AC27D}[富力银行]{FFFFFF} 很抱歉, 由于您已达到最大储蓄帐户数量, 我们无法为您创建更多的储蓄帐户."); return; }
                    }
                    if (p.cash <= 1500) { MainChat.SendErrorChat(p, CONSTANT.ERR_MoneyNotEnought); return; }
                    b.accountType = 2;
                    b.accountMoney = 0;
                    b.owner = p.sqlID;
                    p.cash -= 1500;
                    await p.updateSql();
                    await b.Create();
                    MainChat.SendInfoChat(p, "{4AC27D}[富力银行]{FFFFFF} 您开设了属于 " + p.characterName.Replace("_", " ") + " 的工资帐户.");
                    await Bank_MainPage(p);
                    return;

                case 3:
                    if (bankAccounts.Count > 0)
                    {
                        int limit2 = 0;
                        foreach (var c3Check in bankAccounts)
                        {
                            if (c3Check.accountType == 3) { limit2++; }
                        }
                        if (limit2 >= 3) { MainChat.SendInfoChat(p, "{4AC27D}[富力银行]{FFFFFF} 很抱歉, 由于您已达到最大支票帐户数量, 我们无法为您创建更多的支票帐户."); return; }
                    }
                    if (p.cash <= 500) { MainChat.SendErrorChat(p, CONSTANT.ERR_MoneyNotEnought); return; }
                    b.accountType = 3;
                    b.accountMoney = 0;
                    b.owner = p.sqlID;
                    p.cash -= 500;
                    await p.updateSql();
                    await b.Create();
                    MainChat.SendInfoChat(p, "{4AC27D}[富力银行]{FFFFFF} 您开设了属于 " + p.characterName.Replace("_", " ") + " 的支票帐户.");
                    await Bank_MainPage(p);
                    return;
            }
        }

        [AsyncClientEvent("Bank:CreateCreditCard:Step1")]
        public async Task Bank_CreateCreditCard_Step1(PlayerModel p)
        {
            List<BankModel> bankAccounts = await FindBankAccount("owner", p.sqlID.ToString());
            List<GuiMenu> gMenu = new List<GuiMenu>();
            foreach (var x in bankAccounts)
            {
                string iName = "";
                switch (x.accountType)
                {
                    case 1:
                        iName = " 工资帐户 | 费用:100$";
                        break;
                    case 2:
                        iName = " 储蓄帐户(暂无功能,无需办理) | 费用:300$";
                        break;
                    case 3:
                        iName = " 支票帐户 | 费用:200$";
                        break;
                }
                iName = "<i class='fas fa-barcode'></i>" + x.accountNo.ToString() + " " + iName;
                GuiMenu menuitem = new GuiMenu { name = iName, triger = "Bank:CreateCreditCard:Step2", value = x.accountNo.ToString() };
                gMenu.Add(menuitem);
            }
            GuiMenu Back = new GuiMenu { name = "后退", triger = "Bank:MainPage", value = "0" };
            gMenu.Add(Back);
            Gui y = new Gui()
            {
                image = "https://ih0.redbubble.net/image.941083207.9340/flat,550x550,075,f.u3.jpg",
                guiMenu = gMenu,
                color = "#4AC27D",
            };
            y.Send(p);
        }

        [AsyncClientEvent("Bank:CreateCreditCard:Step2")]
        public async Task Bank_CreateCreditCard_Step2(PlayerModel p, int accountNo)
        {
            int cost = 0;
            List<BankModel> bankAccounts = await FindBankAccount("owner", p.sqlID.ToString());
            BankModel x = bankAccounts.Find(x => x.accountNo == accountNo);
            if (x == null) { return; }
            switch (x.accountType)
            {
                case 1:
                    cost = 100;
                    break;

                case 2:
                    cost = 300;
                    break;

                case 3:
                    cost = 200;
                    break;
            }
            if (p.cash <= cost) { MainChat.SendErrorChat(p, CONSTANT.ERR_MoneyNotEnought); return; }
            ServerItems card = Items.LSCitems.Find(x => x.ID == 24);
            card.data = "0";
            card.data2 = x.accountNo.ToString();
            card.amount = 1;
            card.name = "银行卡 " + x.accountNo.ToString();
            p.SetData("PlayerPINItem", card);
            card = new ServerItems { ID = 24, type = 16, name = "银行卡", picture = "24", weight = 0.01, data = "", data2 = "", objectModel = "prop_cs_credit_card" };

            p.cash -= cost;
            await p.updateSql();

            GuiEvents.GUIMenu_Close(p);

            p.SetData("PlayerCardCost", cost);
            p.EmitLocked("ATM:SetCardPin");

            return;
        }

        [AsyncClientEvent("Bank:SetCardPin")]
        public async Task Bank_SetCardPin(PlayerModel p, string pin)
        {
            //Alt.Log(pin);
            if (pin.Length <= 3) { GlobalEvents.notify(p, 3, "您必须输入一个 4 位数的 PIN"); return; }
            ServerItems card = p.lscGetdata<ServerItems>("PlayerPINItem");
            int cost = p.lscGetdata<int>("PlayerCardCost");
            p.DeleteData("PlayerPINItem");
            p.DeleteData("PlayerCardCost");
            card.data = pin;
            bool succes = await Inventory.AddInventoryItem(p, card, 1);
            card = new ServerItems { ID = 24, type = 16, name = "银行卡", picture = "24", weight = 0.01, data = "", data2 = "", objectModel = "prop_cs_credit_card" };
            if (!succes) { GlobalEvents.notify(p, 3, "您的库存满了!"); p.cash += cost; await p.updateSql(); p.EmitLocked("ATM:PINPageClose"); return; }
            MainChat.SendInfoChat(p, "{4AC27D}[富力银行]{FFFFFF} 您成功为帐户 " + card.data2 + " 创建了一张银行卡, PIN 为: " + pin.ToString());
            p.EmitLocked("ATM:PINPageClose");

        }

        // ATM

        [AsyncClientEvent("ATM:CanUse")]
        public void ShowATMPage(PlayerModel p)
        {
            p.EmitLocked("ATM:ShowLoginPage");
        }

        [AsyncClientEvent("ATM:NotNear")]
        public void NotNearATM(PlayerModel p)
        {
            MainChat.SendErrorChat(p, "[错误] 附近没有ATM.");
        }

        [AsyncClientEvent("ATM:PassTry")]
        public async Task ATM_CheckPassword(PlayerModel p, string value)
        {
            if (p.Ping > 250)
                return;
            InventoryModel cardData = p.lscGetdata<InventoryModel>(EntityData.PlayerEntityData.UsingItem);
            p.DeleteData(EntityData.PlayerEntityData.UsingItem);
            if (value.Length != 0)
            {
                BankModel x = await Database.DatabaseMain.GetBankAccWithID("accountNo", Int32.Parse(cardData.itemData2));
                PlayerModelInfo t = await Database.DatabaseMain.getCharacterInfo(x.owner);
                if (cardData.itemData == value && value != null) { p.EmitLocked("ATM:ShowMainPage", t.characterName.Replace("_", " "), x.accountMoney); p.SetData("ATMinAcc", x.ID); return; }
                GlobalEvents.notify(p, 3, "您输入的密码不正确"); return;
            }
            else { GlobalEvents.notify(p, 3, "您不能将密码留空."); return; }

        }

        [ClientEvent("ATM:WithdrawMoney")]
        public async Task ATM_WithdrawMoney(PlayerModel p, string value)
        {
            if (p.Ping > 250)
                return;
            int bID = p.lscGetdata<int>("ATMinAcc");
            int val = Int32.Parse(value);
            if (val <= 0) { MainChat.SendErrorChat(p, CONSTANT.ERR_ValueNotNegative); GlobalEvents.notify(p, 3, CONSTANT.ERR_ValueNotNegative); return; }
            BankModel t = await Database.DatabaseMain.GetBankAccount(bID);
            if (t.owner != p.sqlID) { GuiNotification.Send(p, "使用不属于您的银行卡会暂时处于被动状态..", "white", "negative", "red", progress: true); return; }
            if (val > t.accountMoney) { MainChat.SendErrorChat(p, CONSTANT.ERR_ValueNotNegative); GlobalEvents.notify(p, 3, CONSTANT.ERR_ValueNotNegative); return; }
            t.accountMoney -= val;
            p.cash += val;
            await p.updateSql();
            await t.Update();
            GlobalEvents.notify(p, 4, "成功取款(ATM).");
            Core.Logger.WriteLogData(Logger.logTypes.BankLog, p.characterName + " | 帐户编号: " + t.accountNo + " | 取款: $" + val + " | 余额: $" + t.accountMoney);
            p.EmitLocked("ATM:MoneyUpdate", t.accountMoney);
            p.EmitLocked("ATM:CanUsePage");
            return;
        }

        [AsyncClientEvent("ATM:DepositMoney")]
        public async Task ATM_DepositMoney(PlayerModel p, string value)
        {
            if (p.Ping > 250)
                return;
            int bID = p.lscGetdata<int>("ATMinAcc");
            int val = Int32.Parse(value);
            if (val <= 0) { MainChat.SendErrorChat(p, CONSTANT.ERR_ValueNotNegative); GlobalEvents.notify(p, 3, CONSTANT.ERR_ValueNotNegative); return; }
            if (val > p.cash) { MainChat.SendErrorChat(p, CONSTANT.ERR_ValueNotNegative); GlobalEvents.notify(p, 3, CONSTANT.ERR_ValueNotNegative); return; }
            p.cash -= val;
            BankModel t = await Database.DatabaseMain.GetBankAccount(bID);
            if (t.owner != p.sqlID) { GuiNotification.Send(p, "使用不属于您的银行卡会暂时处于被动状态..", "white", "negative", "red", progress: true); p.cash += val; return; }
            t.accountMoney += val;
            await p.updateSql();
            await t.Update();
            GlobalEvents.notify(p, 4, "成功存款(ATM).");
            Core.Logger.WriteLogData(Logger.logTypes.BankLog, p.characterName + " | 帐户编号: " + t.accountNo + " | 存款: $" + val + " | 余额: $" + t.accountMoney);
            p.EmitLocked("ATM:MoneyUpdate", t.accountMoney);
            p.EmitLocked("ATM:CanUsePage");
            return;
        }

        [AsyncClientEvent("ATM:MenuClosed")]
        public void ATM_MenuClosed(PlayerModel p)
        {
            if (p.Ping > 250)
                return;
            if (p.HasData(EntityData.PlayerEntityData.UsingItem))
            {
                p.DeleteData(EntityData.PlayerEntityData.UsingItem);
            }
            if (p.HasData("ATMinAcc"))
            {
                p.DeleteData("ATMinAcc");
            }

        }

    }
}
