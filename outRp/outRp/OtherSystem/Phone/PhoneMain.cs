using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Core;
using outRp.Globals;
using outRp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;

namespace outRp.OtherSystem.Phone
{


    public class PhoneMain : IScript
    {
        public class PhoneData
        {
            public int ID { get; set; }
            private PlayerModel p
            {
                get
                {
                    return GlobalEvents.GetPlayerFromSqlID(this.ID);
                }
            }
            public bool inCall
            {
                get
                {
                    if (this.p == null)
                        return false;

                    if (this.p.HasData("inCall"))
                        return this.p.lscGetdata<bool>("inCall");

                    return false;
                }
                set
                {
                    if (this.p == null)
                        return;

                    if (this.p.HasData("inCall"))
                        if (this.p.lscGetdata<bool>("inCall") == value)
                            return;

                    this.p.SetData("inCall", value);
                }
            }
            public int CallTarget
            {
                get
                {
                    if (this.p == null)
                        return -1;

                    if (this.p.HasData("callTarget"))
                        return this.p.lscGetdata<int>("callTarget");

                    return -1;
                }
                set
                {
                    if (this.p == null)
                        return;

                    if (this.p.HasData("callTarget"))
                        if (this.p.lscGetdata<int>("callTarget") == value)
                            return;

                    this.p.SetData("callTarget", value);
                }
            }
            public int callTargetID
            {
                get
                {
                    if (this.p == null)
                        return 0;

                    if (!this.p.HasData("callTargetID"))
                        return 0;

                    return this.p.lscGetdata<int>("callTargetID");
                }
                set
                {
                    if (this.p == null)
                        return;

                    if (this.p.lscGetdata<int>("callTargetID") == value)
                        return;

                    this.p.SetData("callTargetID", value);
                }
            }
            public bool hoparlor
            {
                get
                {
                    if (this.p == null)
                        return false;

                    if (!this.p.HasData("callHoparlor"))
                        return false;

                    return this.p.lscGetdata<bool>("callHoparlor");
                }
                set
                {
                    if (this.p == null)
                        return;

                    this.p.SetData("callHoparlor", value);
                }
            }
            public string Settings { get; set; } = "[]";
        }

        [AsyncClientEvent("Phone:OnHand")]
        public static void PlayerPhoneTakeHand(PlayerModel p, int status)
        {
            if (p.Ping > 250)
                return;
            PhoneData playerD = getData(p.sqlID);
            if (status == 1)
            {
                if (!playerD.inCall)
                {
                    if (p.Vehicle == null)
                    {
                        Animations.PlayerAnimation(p, "phone2");
                    }

                }
            }
            else
            {
                if (!playerD.inCall)
                {
                    if (p.Vehicle == null)
                    {
                        Animations.PlayerStopAnimation(p);
                    }
                }

            }
        }

        public static PlayerModel getFromPhone(int number)
        {
            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (t.phoneNumber == number)
                    return t;
            }

            return null;
        }
        public static PhoneData getData(int ID)
        {
            PhoneData data = new PhoneData();
            data.ID = ID;
            return data;
        }

        public static void phoneMessageEvent(PlayerModel p, int targetSql, string Text)
        {
            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(targetSql);
            if (t == null)
            {
                PhoneData playerD = getData(p.sqlID);
                playerD.inCall = false;
                playerD.CallTarget = -1;
                playerD.callTargetID = -1;
                p.EmitAsync("Phone:CallAnswerCB", false);
                return;
            }


            foreach (PlayerModel x in Alt.GetAllPlayers())
            {
                if (p.Position.Distance(x.Position) <= 10 && x.Dimension == p.Dimension)
                {
                    x.SendChatMessage(p.fakeName.Replace("_", " ") + " 说(通话): " + Text);
                }
            }

            PhoneData targetD = getData(t.sqlID);
            if (targetD.hoparlor == true)
            {

                string sex = "[男性]";
                if (p.sex == 0)
                    sex = "[女性]";
                foreach (PlayerModel x in Alt.GetAllPlayers())
                {
                    if (x == t)
                    {
                        var contList = phoneDatabase.getPhoneContacts(t.phoneNumber);
                        string callerName = "";
                        if (contList != null)
                        {
                            var check = contList.Find(x => x.Number == p.phoneNumber);
                            if (check != null)
                            {
                                callerName = check.Name;
                                x.SendChatMessage("{DEB535}[通话] " + callerName + " " + sex + " 说: " + Text);
                            }
                            else
                            {
                                x.SendChatMessage("{DEB535}[通话] " + p.phoneNumber.ToString() + " " + sex + " 说: " + Text);
                            }
                        }
                        else { x.SendChatMessage("{DEB535}[通话] " + p.phoneNumber.ToString() + " " + sex + " 说: " + Text); }

                    }
                    else
                    {
                        if (t.Position.Distance(x.Position) <= 10 && x.Dimension == p.Dimension)
                        {
                            x.SendChatMessage("{DEB535}[手机扬声器] " + sex + " 说: " + Text);
                        }
                    }
                }
            }
            else
            {
                string sex = "[男性]";
                if (p.sex == 0)
                    sex = "[女性》]";
                //t.SendChatMsg("{DEB535}[Telefon] " + p.phoneNumber + " " + sex +": " + Text);

                var contList = phoneDatabase.getPhoneContacts(t.phoneNumber);
                string callerName = "";
                if (contList != null)
                {
                    var check = contList.Find(x => x.Number == p.phoneNumber);
                    if (check != null)
                    {
                        callerName = check.Name;
                        t.SendChatMessage("{DEB535}[通话] " + callerName + " " + sex + " 说: " + Text);
                    }
                    else
                    {
                        t.SendChatMessage("{DEB535}[通话] " + p.phoneNumber.ToString() + " " + sex + " 说: " + Text);
                    }
                }
                else { t.SendChatMessage("{DEB535}[通话] " + p.phoneNumber.ToString() + " " + sex + " 说: " + Text); }

            }


            return;
        }


        public static void PhoneSendNotifi(PlayerModel p, string title, string text, string icon, string color)
        {
            p.EmitAsync("Phone:Notifi", title, text, icon, color);
        }

        public static async Task TakePhone(PlayerModel p, InventoryModel i)
        {
            if (i.itemId == 1)
            {
                if (Int32.TryParse(i.itemData, out int newNumber))
                {
                    InventoryModel takedPhone = await Database.DatabaseMain.GetInventoryPhone(p);
                    if (takedPhone != null)
                    {
                        takedPhone.itemSlot = 0;
                        await takedPhone.Update();
                    }

                    p.phoneNumber = newNumber;
                    i.itemSlot = 12;
                    await i.Update();
                    await Inventory.UpdatePlayerInventory(p);
                    CheckAndCreatePhone(newNumber);
                    string settings = phoneDatabase.getPhoneSettings(p.phoneNumber);
                    await p.EmitAsync("Phone:Setup", settings);
                }
            }
        }

        public static async Task TakeOutPhone(PlayerModel p, InventoryModel i)
        {
            p.phoneNumber = -1;
            i.itemSlot = 0;
            await i.Update();
            await Inventory.UpdatePlayerInventory(p);
            await p.EmitAsync("Phone:Destroy");
            return;
        }

        public static void CheckAndCreatePhone(int phoneNumber)
        {
            bool phoneCheck = phoneDatabase.checkPhone(phoneNumber);
            if (!phoneCheck)
            {
                phoneDatabase.CreatePhoneData(phoneNumber);
            }
            return;
        }

        [AsyncClientEvent("cPhone:CallNumber")]
        public void phoneCall(PlayerModel p, int targetNumber)
        {
            if (p.Ping > 250)
                return;
            if (p.adminJail > 0 || p.jailTime > 0) { MainChat.SendErrorChat(p, "[错误] 您在监狱中(手机是被没收的)!"); return; }

            if (targetNumber == 0 || targetNumber == -1)
                return;
            // Telefon ame
            MainChat.AME(p, "掏出电话并拨打电话");

            p.SendChatMessage("[提示] 您开始拨打电话了，按T沟通。");

            if (targetNumber == 911)
            {
                p.SendChatMessage("紧急调度员 说: 您好,这里是紧急热线,请问您需要什么服务呢? <br>输入： 警察 | 医生 ");
                p.SetData(EntityData.PlayerEntityData.PlayerInEmergencyCall_1, 0);
                return;
            }

            PlayerModel t = getFromPhone(targetNumber);
            if (t == null)
            {
                p.EmitAsync("Phone:CallAnswerCB", false);
                return;
            }

            PhoneData targetD = getData(t.sqlID);
            if (targetD.callTargetID >= 1)
            {
                p.EmitAsync("Phone:CallAnswerCB", false);
                return;
            }

            // ARama başarılı
            targetD.inCall = false;
            targetD.CallTarget = p.phoneNumber;
            targetD.callTargetID = p.sqlID;
            MainChat.SendInfoChat(t, "[来电] 来自号码: " + p.phoneNumber);
            MainChat.EmoteDo(t, "电话响了.");

            PhoneData playerD = getData(p.sqlID);
            playerD.CallTarget = t.phoneNumber;
            playerD.inCall = false;
            playerD.callTargetID = t.sqlID;

            t.EmitAsync("Phone:IncomingCall", p.phoneNumber);
            return;
        }

        [AsyncClientEvent("cPhone:AnswerCall")]
        public void asnwerCall(PlayerModel p, bool state)
        {
            if (p.Ping > 250)
                return;
            if (p.adminJail > 0 || p.jailTime > 0) { MainChat.SendErrorChat(p, "[错误] 您在监狱中(手机是被没收的)!"); return; }
            PhoneData playerD = getData(p.sqlID);
            if (playerD.callTargetID == 0)
                return;

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(playerD.callTargetID);

            if (t == null)
            {
                playerD.CallTarget = -1;
                playerD.callTargetID = 0;
                playerD.inCall = false;

                p.EmitAsync("Phone:CallAnswerCB", false);

                return;
            }
            PhoneData targetD = getData(t.sqlID);

            targetD.inCall = true;
            playerD.inCall = true;


            p.EmitAsync("Phone:CallAnswerCB", state);
            t.EmitAsync("Phone:CallAnswerCB", state);
            if (state)
            {
                t.EmitAsync("Phone:CallAnswerCB", state);
                Animations.PlayerAnimation(p, "phone1");
                Animations.PlayerAnimation(t, "phone1");
            }
            else
            {
                playerD.callTargetID = 0;
                playerD.CallTarget = -1;
                playerD.inCall = false;
                targetD.callTargetID = 0;
                targetD.CallTarget = -1;
                targetD.inCall = false;
            }
            return;
        }

        [AsyncClientEvent("cPhone:CloseCurrentCall")]
        public void closeCurrentCall(PlayerModel p)
        {
            if (p.Ping > 250)
                return;
            PhoneData playerD = getData(p.sqlID);


            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(playerD.callTargetID);
            if (t != null)
            {
                PhoneData targetD = getData(t.sqlID);
                targetD.callTargetID = 0;
                targetD.CallTarget = -1;
                targetD.inCall = false;
                t.EmitAsync("Phone:CallAnswerCB", false);
            }


            playerD.CallTarget = -1;
            playerD.callTargetID = 0;
            playerD.inCall = false;
            p.EmitAsync("Phone:CallAnswerCB", false);
            return;
        }

        [AsyncClientEvent("cPhone:Hoparlor")]
        public void CallHoparlor(PlayerModel p, bool state)
        {
            if (p.Ping > 250)
                return;
            PhoneData playerD = getData(p.sqlID);
            playerD.hoparlor = state;
            return;
        }

        // Message Events 

        [AsyncClientEvent("cPhone:GetMessages")]
        public void getallMessages(PlayerModel p)
        {
            if (p.Ping > 250)
                return;
            p.EmitAsync("Phone:LoadMessageList", JsonConvert.SerializeObject(phoneDatabase.getAllMessagesTiny(p.phoneNumber)));
            return;
        }

        [AsyncClientEvent("cPhone:SelectMessage")]
        public void getSelectedMessage(PlayerModel p, int selectedNumber)
        {
            if (p.Ping > 250)
                return;
            pClass.Messages messages = phoneDatabase.getAllMessages(p.phoneNumber).Find(x => x.Number == selectedNumber);
            p.EmitAsync("Phone:SelectMessage", JsonConvert.SerializeObject(messages));
            return;
        }

        [AsyncClientEvent("cPhone:SendMessage")]
        public void sendMessage(PlayerModel p, int targetNo, string text)
        {
            if (p.Ping > 250)
                return;
            if (p.adminJail > 0 || p.jailTime > 0) { MainChat.SendErrorChat(p, "[错误] 您在监狱中(手机是被没收的)!"); return; }
            if (targetNo == 0 || targetNo == -1)
            {
                p.SendChatMessage("[错误] 无效号码!");
                return;
            }
            PlayerModel t = getFromPhone(targetNo);
            if (t == null)
            {
                return;
            }

            List<pClass.Messages> _Messages = phoneDatabase.getAllMessages(t.phoneNumber);
            pClass.Messages check = _Messages.Find(x => x.Number == p.phoneNumber);
            if (check == null)
            {
                check = new pClass.Messages();
                check.Number = p.phoneNumber;
                check.message = new List<pClass.Message>();
                _Messages.Add(check);
            }

            pClass.Message _mes = new pClass.Message();
            _mes.isOwner = false;
            _mes.text = text;
            _mes.DateTime = DateTime.Now;
            check.message.Add(_mes);

            phoneDatabase.updateAllMessages(t.phoneNumber, _Messages);
            t.EmitAsync("Phone:reloadMessage", JsonConvert.SerializeObject(check));
            MainChat.SendInfoChat(t, "[新短信] 来自号码: " + p.phoneNumber);
            GlobalEvents.NativeNotifyAll(t, "~p~电话的通知音响了");
            MainChat.AME(p, "使用电话发送了一条短信.");


            _Messages = phoneDatabase.getAllMessages(p.phoneNumber);
            check = _Messages.Find(x => x.Number == t.phoneNumber);
            if (check == null)
            {
                check = new pClass.Messages();
                check.Number = t.phoneNumber;
                check.message = new List<pClass.Message>();
                _Messages.Add(check);
            }

            _mes.isOwner = true;
            check.message.Add(_mes);
            phoneDatabase.updateAllMessages(p.phoneNumber, _Messages);
            p.EmitAsync("Phone:reloadMessage", JsonConvert.SerializeObject(check));
            return;
        }

        [AsyncClientEvent("cPhone:DeleteMessage")]
        public void deleteMessage(PlayerModel p, int number)
        {
            List<pClass.Messages> _m = phoneDatabase.getAllMessages(p.phoneNumber);
            pClass.Messages check = _m.Find(x => x.Number == number);
            if (check != null)
            {
                _m.Remove(check);
            }

            phoneDatabase.updateAllMessages(p.phoneNumber, _m);
            p.EmitAsync("Phone:LoadMessageList", JsonConvert.SerializeObject(phoneDatabase.getAllMessagesTiny(p.phoneNumber)));
            return;
        }

        [AsyncClientEvent("cPhone:AddNewContact")]
        public void AddNewContact(PlayerModel p, int number, string Name)
        {
            if (p.Ping > 250)
                return;
            List<pClass.Contact> _con = phoneDatabase.getPhoneContacts(p.phoneNumber);
            pClass.Contact check = _con.Find(x => x.Number == number);
            if (check == null)
            {
                check = new pClass.Contact();
                check.Name = Name;
                check.Number = number;
                _con.Add(check);
            }
            else
            {
                check.Name = Name;
                check.Number = number;
            }

            phoneDatabase.UpdateContacts(p.phoneNumber, _con);
            p.EmitAsync("Phone:UpdateContactList", JsonConvert.SerializeObject(_con));
            return;
        }

        [AsyncClientEvent("cPhone:ShowContactList")]
        public void loadContactList(PlayerModel p)
        {
            if (p.Ping > 250)
                return;
            List<pClass.Contact> _con = phoneDatabase.getPhoneContacts(p.phoneNumber);
            p.EmitAsync("Phone:UpdateContactList", JsonConvert.SerializeObject(_con));
            return;
        }

        [AsyncClientEvent("cPhone:RemoveContact")]
        public void removeContact(PlayerModel p, int number)
        {
            if (p.Ping > 250)
                return;
            List<pClass.Contact> _c = phoneDatabase.getPhoneContacts(p.phoneNumber);
            pClass.Contact check = _c.Find(x => x.Number == number);
            if (check != null)
            {
                _c.Remove(check);
            }

            phoneDatabase.UpdateContacts(p.phoneNumber, _c);
            p.EmitAsync("Phone:UpdateContactList", JsonConvert.SerializeObject(_c));
            return;
        }

        [AsyncClientEvent("cPhone:SettingsUpdate")]
        public void SettingsUpdate(PlayerModel p, string data)
        {
            if (p.Ping > 250)
                return;
            phoneDatabase.updatePhoneSettings(p.phoneNumber, data);
            return;
        }

        public class PhoneCars
        {
            public int ID { get; set; }
            public string Plate { get; set; }
            public int Tax { get; set; }
        }
        [AsyncClientEvent("cPhone:GetBankAccounts")]
        public async Task getBankAccounts(PlayerModel p)
        {
            if (p.Ping > 250)
                return;
            List<LSCsystems.ATM.BankModel> banks = await Database.DatabaseMain.FindBankAccount("owner", p.sqlID.ToString());
            await p.EmitAsync("Phone:SetBankAccounts", JsonConvert.SerializeObject(banks));
            return;
        }

        [AsyncClientEvent("cPhone:GetCarList")]
        public void getCarList(PlayerModel p)
        {
            if (p.Ping > 250)
                return;
            List<PhoneCars> cars = new List<PhoneCars>();
            foreach (VehModel v in Alt.GetAllVehicles())
            {
                if (v.owner != p.sqlID)
                    continue;

                if (v.fine <= 0)
                    continue;

                cars.Add(new PhoneCars()
                {
                    ID = v.sqlID,
                    Plate = v.NumberplateText,
                    Tax = v.fine
                });
            }

            p.EmitAsync("Phone:SetCarList", JsonConvert.SerializeObject(cars));
            return;
        }

        [AsyncClientEvent("cPhone:PayVehicleTax")]
        public async Task PayVehicleTax(PlayerModel p, int vSQL, int accountNo)
        {
            if (p.Ping > 250)
                return;
            VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(vSQL);

            if (v == null)
            {
                //Alt.Log("v == null");
                return;
            }

            if (v.fine <= 0)
            {
                //Alt.Log("vfine <= 0");
                return;
            }

            if (v.owner != p.sqlID)
            {
                //Alt.Log("v.owner != p.sql");
                return;
            }

            List<LSCsystems.ATM.BankModel> sBank = await Database.DatabaseMain.FindBankAccount("accountNo", accountNo.ToString());
            if (sBank[0] == null)
            {
                //Alt.Log("sbank null");
                return;
            }

            if (sBank[0].accountMoney < v.fine)
            {
                PhoneSendNotifi(p, "富力银行", "帐户余额不足!", "<i class='bx bx-bank'></i>", "danger");
                return;
            }

            if (sBank[0].owner != p.sqlID)
            {
                await antiCheat.ACBAN(p, 3, "Bank DUMP");
                return;
            }

            sBank[0].accountMoney -= v.fine;
            sBank[0].Update();

            PhoneSendNotifi(p, "富力银行", "成功支付车牌号码为" + v.NumberplateText + " 的税款/罚款, 已支付: $" + v.fine, "<i class='bx bx-check'></i>", "success");

            v.fine = 0;
            v.towwed = false;
            v.settings.fines = new List<string>();
            v.Update();

            return;
        }

        [AsyncClientEvent("cPhone:SendMoney")]
        public async Task SendMoney(PlayerModel p, int sender, int getter, int value)
        {
            if (p.Ping > 250)
                return;
            if (value <= 0)
            {
                PhoneSendNotifi(p, "富力银行", "请输入有效金额!", "<i class='bx bx-bank'></i>", "danger");
                return;
            }
            List<LSCsystems.ATM.BankModel> sBank = await Database.DatabaseMain.FindBankAccount("accountNo", sender.ToString());
            if (sBank[0] == null)
                return;
            if (sBank[0].accountMoney < value)
            {
                PhoneSendNotifi(p, "富力银行", "帐户余额不足!", "<i class='bx bx-bank'></i>", "danger");
                return;
            }

            List<LSCsystems.ATM.BankModel> gBank = await Database.DatabaseMain.FindBankAccount("accountNo", getter.ToString());
            if (gBank[0] == null)
            {
                PhoneSendNotifi(p, "富力银行", "无效收款账号!(( 可能离线了 ))", "<i class='bx bx-bank'></i>", "warn");
                return;
            }
            if (gBank[0].accountNo == sBank[0].accountNo)
            {
                PhoneSendNotifi(p, "富力银行", "无法转账至自己的帐户!", "<i class='bx bx-bank'></i>", "warn");
                return;
            }

            if (sBank[0].owner != p.sqlID)
            {
                await antiCheat.ACBAN(p, 3, "Bank DUMP");
                return;
            }

            sBank[0].accountMoney -= value;
            await sBank[0].Update();
            gBank[0].accountMoney += value;
            await gBank[0].Update();

            PhoneSendNotifi(p, "富力银行", "成功转账至帐户 " + getter.ToString() + " , 金额: $" + value, "<i class='bx bx-check'></i>", "success");



            await getBankAccounts(p);
            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(gBank[0].owner);
            if (t != null)
            {
                await getBankAccounts(t);
                PhoneSendNotifi(t, "富力银行", "帐户 " + sender.ToString() + " 转账至 " + getter.ToString() + " , 金额: $" + value, "<i class='bx bx-bank'></i>", "success");
                MainChat.SendInfoChat(t, "[富力银行] 帐户 " + sender.ToString() + " 转账至 " + getter.ToString() + " , 金额: $" + value);
                Core.Logger.WriteLogData(Logger.logTypes.BankLog, "[移动转账] " + p.characterName + "->" + t.characterName + " | Miktar: $" + value);
            }
            else
            {
                Core.Logger.WriteLogData(Logger.logTypes.BankLog, "[移动转账] " + p.characterName + "->" + gBank[0].owner + " , 金额: $" + value);
            }
            return;
        }

        [AsyncClientEvent("cPhone:AddNewPhoto")]
        public void addNewPhoto(PlayerModel p, string image)
        {
            if (p.phoneNumber <= 0)
                return;

            List<pClass.image> images = phoneDatabase.getAllimages(p.phoneNumber);
            if (image == null) { images = new List<pClass.image>(); }
            pClass.image nimage = new pClass.image();
            nimage.link = image;
            images.Add(nimage);
            p.EmitAsync("Phone:UpdatePhotos", JsonConvert.SerializeObject(images));
            phoneDatabase.updateAllimages(p.phoneNumber, images);
            return;
        }

        [AsyncClientEvent("cPhone:GetPhotos")]
        public void getAllPhotos(PlayerModel p)
        {
            if (p.phoneNumber <= 0)
                return;

            List<pClass.image> images = phoneDatabase.getAllimages(p.phoneNumber);
            p.EmitAsync("Phone:UpdatePhotos", JsonConvert.SerializeObject(images));
            return;
        }

        [AsyncClientEvent("cPhone:DeletePhoto")]
        public void DeletePhoto(PlayerModel p, string link)
        {
            if (p.phoneNumber <= 0)
                return;

            List<pClass.image> images = phoneDatabase.getAllimages(p.phoneNumber);
            pClass.image check = images.Find(x => x.link == link);
            if (check != null)
            {
                images.Remove(check);
            }

            p.EmitAsync("Phone:UpdatePhotos", JsonConvert.SerializeObject(images));
            phoneDatabase.updateAllimages(p.phoneNumber, images);
            return;
        }


        [AsyncClientEvent("cPhone:SharePhoto")]
        public void SharePhoto(PlayerModel p, int targetNo, string link)
        {
            PlayerModel t = getFromPhone(targetNo);
            if (t == null)
            {
                // TODO mesaj gönderilemedi.
                return;
            }

            List<pClass.Messages> _Messages = phoneDatabase.getAllMessages(t.phoneNumber);
            pClass.Messages check = _Messages.Find(x => x.Number == p.phoneNumber);
            if (check == null)
            {
                check = new pClass.Messages();
                check.Number = p.phoneNumber;
                check.message = new List<pClass.Message>();
                _Messages.Add(check);
            }

            pClass.Message _mes = new pClass.Message();
            _mes.isOwner = false;
            _mes.text = link;
            _mes.type = 2;
            check.message.Add(_mes);

            phoneDatabase.updateAllMessages(t.phoneNumber, _Messages);
            t.EmitAsync("Phone:reloadMessage", JsonConvert.SerializeObject(check));


            _Messages = phoneDatabase.getAllMessages(p.phoneNumber);
            check = _Messages.Find(x => x.Number == t.phoneNumber);
            if (check == null)
            {
                check = new pClass.Messages();
                check.Number = t.phoneNumber;
                check.message = new List<pClass.Message>();
                _Messages.Add(check);
            }

            _mes.isOwner = true;
            check.message.Add(_mes);
            phoneDatabase.updateAllMessages(p.phoneNumber, _Messages);
            p.EmitAsync("Phone:reloadMessage", JsonConvert.SerializeObject(check));
            return;
        }

        [AsyncClientEvent("cPhone:SendGPS")]
        public void SendGPS(PlayerModel p, int targetNo, Position pos)
        {
            PlayerModel t = getFromPhone(targetNo);
            if (t == null)
            {
                // TODO mesaj gönderilemedi.
                return;
            }

            List<pClass.Messages> _Messages = phoneDatabase.getAllMessages(t.phoneNumber);
            pClass.Messages check = _Messages.Find(x => x.Number == p.phoneNumber);
            if (check == null)
            {
                check = new pClass.Messages();
                check.Number = p.phoneNumber;
                check.message = new List<pClass.Message>();
                _Messages.Add(check);
            }

            pClass.Message _mes = new pClass.Message();
            _mes.isOwner = false;
            _mes.text = JsonConvert.SerializeObject(pos);
            _mes.type = 3;
            _mes.DateTime = DateTime.Now;
            check.message.Add(_mes);

            phoneDatabase.updateAllMessages(t.phoneNumber, _Messages);
            t.EmitAsync("Phone:reloadMessage", JsonConvert.SerializeObject(check));


            _Messages = phoneDatabase.getAllMessages(p.phoneNumber);
            check = _Messages.Find(x => x.Number == t.phoneNumber);
            if (check == null)
            {
                check = new pClass.Messages();
                check.Number = t.phoneNumber;
                check.message = new List<pClass.Message>();
                _Messages.Add(check);
            }

            _mes.isOwner = true;
            check.message.Add(_mes);
            phoneDatabase.updateAllMessages(p.phoneNumber, _Messages);
            p.EmitAsync("Phone:reloadMessage", JsonConvert.SerializeObject(check));
            return;
        }
    }
}
