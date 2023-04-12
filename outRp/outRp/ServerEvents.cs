using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Args;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Core;
using outRp.Globals;
using outRp.Kook;
using outRp.Models;
using outRp.OtherSystem;
using outRp.OtherSystem.LSCsystems;
using outRp.OtherSystem.Textlabels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Timers;
using static outRp.OtherSystem.LSCsystems.ATM;
using AltV.Net.Resources.Chat.Api;
using outRp.Company.systems;
using outRp.Globals.Commands;
using outRp.Plants;

namespace outRp
{
    public class ServerEvent
    {
        public static int serverHourTimer = 0;
        public static int serverSaveTimer = 0;
        public static int serverDrugTimer = 0;
        public static int adversimentTimer = 0;
        public static void ServerTimer()
        {
            Timer timer = new Timer(60000); // 60000 olarak düzelt.
            timer.Elapsed += OnMinuteSpend;
            timer.Start();
        }


        public static int CurrentWeather = 1;

        //public static Timer OnMinuteSpent = new Timer(OnMinuteSpend, null, 1000, 1000);
        public static async void OnMinuteSpend(Object sender, EventArgs e)
        {
            serverHourTimer += 1;
            serverSaveTimer += 1;
            serverDrugTimer += 1;
            adversimentTimer += 1;

            await Database.DatabaseMain.SaveServerSettings(); // Sunucu kayıt

            if (adversimentTimer >= 2)
            {
                Globals.Commands.AdminCommands.Event_ShowAutomaticAdversiment();
                adversimentTimer = 0;
            }


            //ServerGlobalValues.FishingPrice += 1;
            //if (ServerGlobalValues.FishingPrice > 60)
            //{
            //    ServerGlobalValues.FishingPrice = 60;
            //}

            // Vehicle Rent System ! OtherSystem.LSCSystem.PetSystem'deki timer'a taşındı.
            //Vehicle.VehicleMain.Event_Vehicle_RentTimeControl(); ,


            await PlayerGameTimeUpdate();

            if (serverHourTimer >= 60) // TODO 60'a çek
            {
                serverHourTimer = 0;
                await VehicleGameTimeUpdate(); // TODO önceki hali awaitli.
                await BusinessHouseTax();// Tax sistemi
                OtherSystem.LSCsystems.cksystem.CorpseUpdateRemaining();
                //OtherSystem.Inventory.ClearGroundItems();
                //await Company.BusinesMain.CompanyHourTimer();
                outRp.OtherSystem.LSCsystems.Jacking.CanUseLockPick = OtherSystem.LSCsystems.Jacking.defLockPick;
                //OtherSystem.LSCsystems.OOCmarket.EVENT_CheckNPCEndDate();


                // Yeni vergi sistemi
                //Props.Business.BusinessTax();
                //Props.Houses.UpdateHouseTax();   .

                foreach (OtherSystem.LSCsystems.DrugFarm drug in OtherSystem.LSCsystems.Drug.serverDrugs)
                {
                    OtherSystem.LSCsystems.Drug.UpdateDrugInfo(drug, 1, -5);
                }
            }

            if (serverSaveTimer > 30)
            {
                serverSaveTimer = 0;

                foreach (VehModel tV in Alt.GetAllVehicles())
                {
                    tV.Update();
                }

                /*foreach (PlayerModel t in Alt.GetAllPlayers())
                {
                    t.updateSql();
                    foreach (DiseaseModel d in t.injured.diseases)
                    {
                        if (d.DiseaseName != "Uyuşturucu Bağımlılığı")
                        {
                            if (d.DiseaseValue >= 5) { t.EmitAsync("Damage:Stungun", (d.DiseaseValue * 10)); outRp.Chat.MainChat.SendInfoChat(t, "[!] Hastalığınız nedeniyle sendelediniz."); break; }
                        }
                    }
                }*/
            }


            // Bilgi mesajlarını temizleme
            OtherSystem.LSCsystems.InformationMarkers.CheckAllInformations();

        }

        public static async Task PlayerGameTimeUpdate()
        {
            try
            {
                foreach (PlayerModel p in Alt.GetAllPlayers())
                {
                    if (p.isOnline == true)
                    {
                        if (DateTime.Now.Day > 30)
                            p.SetDateTime(DateTime.Now.Day - 1, DateTime.Now.Month - 1, DateTime.Now.Year, DateTime.Now.AddHours(1).Hour, DateTime.Now.Minute, DateTime.Now.Second);
                        else
                            p.SetDateTime(DateTime.Now.Day, DateTime.Now.Month - 1, DateTime.Now.Year, DateTime.Now.AddHours(1).Hour, DateTime.Now.Minute, DateTime.Now.Second);

                        if (p.Dimension == 0)
                        {
                            p.SetWeather((uint)CurrentWeather);
                        }
                        // Jail
                        if (p.jailTime > 0)
                        {
                            p.jailTime -= 1;
                            if (p.jailTime == 0)
                            {
                                p.Position = ServerGlobalValues.endJailPos;
                                p.Dimension = 0;
                                GlobalEvents.UINotifiy(p, 12, "~g~监狱时间结束了.", "~y~您的角色出狱了.");
                            }
                            else
                            {
                                GlobalEvents.SubTitle(p, "~y~出狱时间: ~w~" + p.jailTime.ToString() + " 分钟", 59);
                                //p.SendChatMessage("AFK sistemi tarafından kicklenmemek için belirli aralıklarla hareket etmelisiniz.");
                            }
                        }

                        // Admin Jail
                        if (p.adminJail > 0)
                        {
                            p.adminJail -= 1;
                            if (p.adminJail == 0)
                            {
                                p.Position = ServerGlobalValues.endJailPos;
                                p.Dimension = 0;
                                GlobalEvents.UINotifiy(p, 12, "~g~监狱时间结束了.", "....");
                                GlobalEvents.GameControls(p, true);
                            }
                            else
                            {
                                GlobalEvents.SubTitle(p, "~y~OOC关押结束时间: ~w~" + p.adminJail.ToString() + " 分钟", 59);
                                //p.SendChatMessage("AFK sistemi tarafından kicklenmemek için belirli aralıklarla hareket etmelisiniz.");
                            }
                        }

                        // DimensionCheck
                        if (p.Dimension != 0)
                        {
                            if (!p.HasData(EntityData.PlayerEntityData.PlayerInDimension))
                                p.SetData(EntityData.PlayerEntityData.PlayerInDimension, true);
                        }
                        else
                        {
                            if (p.HasData(EntityData.PlayerEntityData.PlayerInDimension))
                                p.DeleteData(EntityData.PlayerEntityData.PlayerInDimension);
                        }


                        // Payday
                        p.gameTime += 1;
                        if (p.gameTime >= 60)
                        {
                            // Hatalık arttır
                            foreach (DiseaseModel d in p.injured.diseases)
                            {
                                if (d.DiseaseValue < 15 && d.DiseaseName != "毒瘾") { d.DiseaseValue += 1; }
                            }

                            if (p.injured.diseases.Count >= 1) { Chat.MainChat.SendInfoChat(p, "[!] 您的角色感觉不舒服, 请前往医院检查."); }
                            // Hastalık End
                            // GYM
                            gym.gymTimer(p);
                            // GTM Over
                            p.gameTime = 0;
                            p.characterExp += ServerGlobalValues.PayDayExp;

                            int fPayday = 0;
                            int pdPrice = ServerGlobalValues.PayDayPrice;
                            //if (p.characterLevel <= 5) { pdPrice *= 2; } // 5 lvl olana kadar total 36k - x3 çarpan ile.
                            int payday = pdPrice + (p.characterLevel * 2);
                            CharacterSettings drugEvent = JsonConvert.DeserializeObject<CharacterSettings>(p.settings); // TODO Aşağıda yorum satırında payday için yukarı çekildi.
                            if (drugEvent.fines.Count >= 1)
                            {
                                payday = payday / 2;
                            }
                            drugEvent.odun = 40;
                            drugEvent.uzum = 80;
                            p.settings = JsonConvert.SerializeObject(drugEvent);

                            if (p.factionId > 0)
                            {
                                FactionModel pFact = await Database.DatabaseMain.GetFactionInfo(p.factionId);
                                var pRank = pFact.rank.Find(x => x.Rank == p.factionRank);
                                if (pRank != null)
                                {
                                    if (pRank.permission.canUsePayday)
                                    {
                                        if (pFact.type == ServerGlobalValues.fType_PD || pFact.type == ServerGlobalValues.fType_FD || pFact.type == ServerGlobalValues.fType_News || pFact.type == ServerGlobalValues.fType_MD)
                                        {
                                            if (p.HasData(EntityData.PlayerEntityData.PDDuty) || p.HasData(EntityData.PlayerEntityData.FDDuty) || p.HasData(EntityData.PlayerEntityData.NewsDuty) || p.HasData(EntityData.PlayerEntityData.MDDuty))
                                            {
                                                if (pFact.cash > pRank.Payday)
                                                {
                                                    fPayday = pRank.Payday;
                                                    pFact.cash -= pRank.Payday;
                                                    pFact.Update();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (pFact.cash > pRank.Payday)
                                            {
                                                fPayday = pRank.Payday;
                                                pFact.cash -= pRank.Payday;
                                                pFact.Update();
                                            }
                                        }

                                    }
                                }
                            }

                            if (p.characterExp >= (p.characterLevel * 4))
                            {
                                p.characterLevel += 1;
                                p.characterExp = 0;
                                p.EmitLocked("Scale:ShowText", "~b~您的账号等级提升了", p.characterName.Replace("_", " "), p.characterLevel);
                            }

                            List<BankModel> bankAccounts = await FindBankAccount("owner", p.sqlID.ToString());
                            BankModel paydayAccount = bankAccounts.Find(x => x.accountType == 1);
                            if (p.adminJail <= 10)
                            {
                                if (paydayAccount != null)
                                {
                                    payday = (int)(payday * 1);
                                    if (fPayday > 0)
                                    {
                                        paydayAccount.accountMoney += payday + fPayday;
                                        GlobalEvents.ShowNotification(p, "~w~政府补贴: ~g~$" + payday.ToString() + "~n~~w~组织工资: ~g~$" + fPayday.ToString() + "~n~~b~账号经验值: ~y~" + p.characterExp.ToString() + "~w~/~g~" + (p.characterLevel * 4).ToString(), icon: p, title: "~g~富力银行", subtitle: "~g~每小时发薪日");
                                    }
                                    else
                                    {
                                        paydayAccount.accountMoney += payday;
                                        GlobalEvents.ShowNotification(p, "~w~政府补贴: ~g~$" + payday.ToString() + "~n~~w~账号经验值: ~y~" + p.characterExp.ToString() + "~w~/~g~" + (p.characterLevel * 4).ToString(), icon: p, title: "~g~富力银行", subtitle: "~g~每小时发薪日");
                                    }
                                    await paydayAccount.Update();
                                }
                                else
                                {
                                    /*if (fPayday > 0)
                                    {
                                        p.cash += payday + fPayday;
                                        await p.updateSql();
                                        GlobalEvents.ShowNotification(p, "~w~政府补贴: ~g~$" + payday.ToString() + "~n~~w~组织工资: ~g~$" + fPayday.ToString() + "~n~~b~账号经验值: ~y~" + p.characterExp.ToString() + "~w~/~g~" + (p.characterLevel * 4).ToString() + "~n~~r~无薪水帐户", icon: p, title: "富力银行", subtitle: "~g~每小时发薪日");
                                    }
                                    else
                                    {
                                        p.cash += payday;
                                        GlobalEvents.ShowNotification(p, "~w~政府补贴: ~g~$" + payday.ToString() + "~n~~w~账号经验值: ~y~" + p.characterExp.ToString() + "~w~/~g~" + (p.characterLevel * 4).ToString() + "~n~~r~无薪水帐户", icon: p, blink: true, title: "~g~富力银行", subtitle: "~g~每小时发薪日");
                                    }*/
                                    GlobalEvents.ShowNotification(p, "~y~您还没有办理银行卡(工资卡)~n~~y~所以您是无法获取工资的~n~请前往银行办理", icon: p, blink: true, title: "~g~富力银行", subtitle: "~g~每小时发薪日");
                                }
                            }
                            else
                            {
                                MainChat.SendInfoChat(p, "[?] 因为您在监狱中, 所以跳过发薪日.");
                            }

                            // Ev Kira ödemesi
                            List<HouseModel> allHouses = await Database.DatabaseMain.GetAllServerHouses();
                            foreach (HouseModel h in allHouses)
                            {
                                if (h.rentOwner == p.sqlID)
                                {
                                    if (p.cash > h.rentPrice)
                                    {
                                        p.cash -= h.rentPrice;
                                        await p.updateSql();
                                        Chat.MainChat.SendInfoChat(p, "[租金支付] 地址编号: " + h.ID.ToString() + " 已支付: $" + h.rentPrice.ToString());

                                        PlayerModel rentOwner = GlobalEvents.GetPlayerFromSqlID(h.ownerId);

                                        var rentOwnerBankAccount = await Database.DatabaseMain.FindBankAccount("owner", h.rentOwner.ToString());
                                        if (rentOwnerBankAccount != null)
                                        {
                                            rentOwnerBankAccount[0].accountMoney += h.rentPrice;
                                            await rentOwnerBankAccount[0].Update();
                                        }

                                        if (rentOwner != null)
                                        {
                                            Chat.MainChat.SendInfoChat(rentOwner, "[房屋租金(收入)] 地址编号: " + h.ID.ToString() + " 已收入: $" + h.rentPrice.ToString());
                                        }

                                    }
                                    else
                                    {
                                        if (paydayAccount.accountMoney > h.rentPrice)
                                        {
                                            paydayAccount.accountMoney -= h.rentPrice;
                                            Chat.MainChat.SendInfoChat(p, "[租金支付 - 工资帐户扣除] 地址编号: " + h.ID.ToString() + " 已支付: $" + h.rentPrice.ToString());

                                            PlayerModel rentOwner = GlobalEvents.GetPlayerFromSqlID(h.rentOwner);

                                            var rentOwnerBankAccount = await Database.DatabaseMain.FindBankAccount("owner", h.rentOwner.ToString());
                                            if (rentOwnerBankAccount != null)
                                            {
                                                rentOwnerBankAccount[0].accountMoney += h.rentPrice;
                                                await rentOwnerBankAccount[0].Update();
                                            }

                                            if (rentOwner != null)
                                            {
                                                Chat.MainChat.SendInfoChat(rentOwner, "[房屋租金(收入)] 地址编号 : " + h.ID.ToString() + " 已收入: $" + h.rentPrice.ToString());
                                            }
                                        }
                                        else
                                        {
                                            CharacterSettings targetSettings = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
                                            PlayerFineModel nFine = new PlayerFineModel()
                                            {
                                                sender = p.sqlID,
                                                finePrice = h.rentPrice,
                                                reason = "租金债务 | 房屋地址编号: " + h.ID.ToString(),
                                            };
                                            targetSettings.fines.Add(nFine);
                                            p.settings = JsonConvert.SerializeObject(targetSettings);
                                            await p.updateSql();
                                        }
                                    }
                                }
                            }

                            await paydayAccount.Update();
                            // Ev kira ödemesi SON

                            if (drugEvent.drugEvents.DrugLevel >= 4)
                            {
                                drugEvent.drugEvents.DrugWantLevel += 2;
                                if (drugEvent.drugEvents.DrugWantLevel >= 10)
                                {
                                    Chat.MainChat.EmoteDo(p, "可以观察到这个人的手开始颤抖.");
                                }
                            }
                        }
                    }
                }
            }
            catch { return; }

        }
        public static async Task VehicleGameTimeUpdate()
        {

            foreach (VehModel v in Alt.GetAllVehicles())
            {
                int multipler = 1;

                int addfine = v.defaultTax / multipler;

                if (v.Dimension == v.sqlID)
                    addfine /= 2;

                if (v.factionId > 0)
                {
                    FactionModel vehfact = await Database.DatabaseMain.GetFactionFromType(v.factionId);
                    if (vehfact != null)
                    {
                        if (vehfact.side == 1)
                        {
                            addfine -= (addfine / 10);
                        }
                    }
                }


                v.fine += addfine;
                if (v.fine > (v.price / 10))
                {
                    v.towwed = true;
                    v.EngineOn = false;
                    v.Dimension = v.sqlID;
                }

            }

        }
        public static async Task BusinessHouseTax()
        {
            List<BusinessModel> serverBusiness = await Database.DatabaseMain.GetAllServerBusiness();
            foreach (var biz in serverBusiness)
            {
                if (biz.ownerId <= 1)
                    continue;
                if (biz.settings.Tax == -1)
                    continue;

                double multipler = 1;
                double priceMultipler = 1;

                int totalBiz = serverBusiness.Where(x => x.ownerId == biz.ownerId).Count();
                if (totalBiz > 1)
                {
                    multipler = 1.2;
                }

                var b = await Props.Business.getBusinessById(biz.ID);
                if (b.Item1.settings.Tax != 0)
                    b.Item1.settings.TotalTax += (int)((b.Item1.settings.Tax) * multipler * priceMultipler);
                else b.Item1.settings.TotalTax += (int)(((biz.price / 4000)) * multipler * priceMultipler);
                await b.Item1.Update(b.Item2, b.Item3);
            }

            List<HouseModel> houses = await Database.DatabaseMain.GetAllServerHouses();
            foreach (var ho in houses)
            {
                if (ho.ownerId <= 1)
                    continue;

                if (ho.settings.Tax == -1)
                    continue;

                double priceMultipler = 1;

                int totalBiz = houses.Where(x => x.ownerId == ho.ownerId).Count();
                double multipler = 1;

                if (totalBiz > 2)
                    multipler = 1.2;

                var house = await Props.Houses.getHouseById(ho.ID);
                if (house.Item1.settings.Tax != 0)
                    house.Item1.settings.TotalTax += (int)((ho.settings.Tax) * multipler * priceMultipler);
                else
                    house.Item1.settings.TotalTax += (int)(((ho.price / 6666)) * multipler * priceMultipler);
                house.Item1.Update(house.Item3, house.Item2);
            }
        }
    }
    public class ServerEvents : IScript
    {
        [AsyncScriptEvent(ScriptEventType.PlayerDisconnect)]
        public static async Task OnPlayerDisconnect(PlayerModel player, string reason)
        {
            try
            {

                //p.SetData("Modify:Vehicle", v.sqlID);
                if (player.HasData("Modify:Vehicle"))
                {
                    VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(player.lscGetdata<int>("Modify:Vehicle"));
                    if (v != null)
                    {
                        v.AppearanceData = v.settings.ModifiyData;
                    }
                }
                if (reason == "" || reason == string.Empty)
                    reason = "手动退出";

                Core.Logger.WriteLogData(Logger.logTypes.ip, "离开 > " + player.characterName + " 原因:" + reason);
                if (!player.HasSyncedMetaData("isInSpec"))
                {
                    foreach (IPlayer target in Alt.GetAllPlayers())
                    {
                        if (target.Dimension == player.Dimension && target.Position.Distance(player.Position) < 10)
                        {
                            MainChat.SendErrorChat(target, "{AFADAA} <i>" + player.characterName.Replace('_', ' ') + " 离开了服务器.</i>");
                        }
                    }
                }

                await GlobalEvents.SetPlayerOnline(player, false);

                await player.updateSql();



                if (player.Vehicle != null && player.Vehicle.Driver == player)
                {
                    player.Vehicle.EngineOn = false;
                }

                if (player.HasData(EntityData.PlayerEntityData.hasProp))
                {
                    player.lscGetdata<OtherSystem.Textlabels.LProp>(EntityData.PlayerEntityData.hasProp).Delete();
                }
                if (player.HasData(EntityData.PlayerEntityData.secondPropInfo))
                {
                    player.lscGetdata<OtherSystem.Textlabels.LProp>(EntityData.PlayerEntityData.secondPropInfo).Delete();
                }

                if (player.HasData("AccountId"))
                {

                    AccountModel account = await Database.DatabaseMain.getAccInfo(player.lscGetdata<int>("AccountId"));
                    account.isOnline = false;
                    await account.Update();
                }

                if (player.injured.Injured)
                {
                    player.injured.Injured = false;
                }

                //Vehicle.VehicleMain.RemoveTempOwner(player);
                if (player.HasData("Taxi:SpawnedCar"))
                {
                    VehModel taxi = Vehicle.VehicleMain.getVehicleFromSqlId(player.lscGetdata<int>("Taxi:SpawnedCar"));
                    if (taxi != null)
                    {
                        taxi.Remove();
                    }
                }

                if (player.HasData("Job:Trash:RentCar"))
                {
                    VehModel trash = Vehicle.VehicleMain.getVehicleFromSqlId(player.lscGetdata<int>("Job:Trash:RentCar"));
                    if (trash != null)
                    {
                        trash.Remove();
                    }
                }
                if (player.HasData("K9:ID"))
                {
                    OtherSystem.PedModel followstop = OtherSystem.PedStreamer.Get(player.lscGetdata<ulong>("K9:ID"));
                    if (followstop != null)
                    {
                        followstop.Destroy();
                    }
                }

                // p.SetData("Modify:Vehicle", v.sqlID);
                if (player.HasData("Modify:Vehicle"))
                {
                    VehModel resetVeh = Vehicle.VehicleMain.getVehicleFromSqlId(player.lscGetdata<int>("Modify:Vehicle"));
                    if (resetVeh != null)
                    {
                        resetVeh.AppearanceData = resetVeh.settings.ModifiyData;
                    }
                }




                await GlobalEvents.SetPlayerOnline(player, false);
                await player.updateSql();
                //await Discord.Main.PushServerStatus();


                if (player.HasData("News:Watching"))
                {
                    object[] inf = player.lscGetdata<object[]>("News:Watching");
                    await Database.DatabaseMain.updatePlayerPosition(player.sqlID, (Position)inf[0], (int)inf[1]);
                }

                if (player.HasData("LoginDate"))
                {
                    //Alt.Log(player.lscGetdata<DateTime>("LoginDate").ToString());


                    DateTime JoinDate = DateTime.Parse(player.lscGetdata<string>("LoginDate"));
                    //Alt.Log((DateTime.Now - JoinDate).TotalMinutes.ToString());

                    await Database.DatabaseMain.AddPlayTimeLog(player.sqlID, player.accountId, JoinDate.ToString(), DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), (int)(DateTime.Now - JoinDate).TotalMinutes, "离开服务器");
                }

                if (player.Position.Distance(new(4129.253f, -4622.479f, 5.706909f)) < 25 && player.Dimension != 0)
                {
                    var check = OtherSystem.LSCsystems.Tent.serverTents.Where(x => x.objectID == (ulong)player.Dimension).FirstOrDefault();
                    if (check == null)
                    {
                        return;
                    }

                    var pos = check.Position;
                    pos.X += 2;
                    await Database.DatabaseMain.updatePlayerPosition(player.sqlID, check.Position, 0);
                }
                DriverSchool.ServerEvent.OnPlayerDisconnect(player);
                return;
            }
            catch (Exception e)
            {
                //Core.Core.OutputLog("[Disconnect Error]", ConsoleColor.Red); Core.Core.OutputLog("[" + e.Message + "]", ConsoleColor.Blue);
                Alt.Log("Disconnect Error: " + JsonConvert.SerializeObject(e));
                return;
            }
        }
        

        [AsyncScriptEvent(ScriptEventType.PlayerEnterVehicle)]
        [Obsolete]
        public async Task OnPlayerEnterVehicle(VehModel v, PlayerModel p, byte seat)
        {
            try
            {
                if (v.Model == (uint)VehicleModel.FireTruck)
                    p.EmitLocked("Fire:Shoot", true);
                AttachmentSystem.deleteAllAttachs(p);
                //if(seat > 1) { return; } ...
                if (v.HasData("InModify"))
                    await v.SetAppearanceDataAsync(v.settings.ModifiyData);
                if (p.HasData("Job:TrashTaken"))
                {
                    outRp.Chat.MainChat.SendInfoChat(p, "[!] 您手上的垃圾掉了.");
                    p.DeleteData("Job:TrashTaken");
                }

                if (v.settings.PDLock)
                {
                    Chat.MainChat.SendErrorChat(p, "[!] 此车已被执法扣押: " + v.settings.PDLockName);
                }

                var model = (VehicleModel)v.Model;
                string oName = "";
                if (v.owner > 0)//
                {
                    PlayerModelInfo t = await Database.DatabaseMain.getCharacterInfo(v.owner);
                    if (t != null)
                    {
                        oName = "{49A0CD} • [所有者] {C8C8C8}" + t.characterName.Replace("_", " ");
                    }
                }

                if (v.jobId == 1 && v.HasData(EntityData.VehicleEntityData.VehicleRentTime))
                {
                    string text = "";
                    if (v.HasData(EntityData.VehicleEntityData.VehicleTempOwner))
                    {
                        PlayerModelInfo rentTarget = await Database.DatabaseMain.getCharacterInfo(v.lscGetdata<int>(EntityData.VehicleEntityData.VehicleTempOwner));
                        if (rentTarget != null) { text = "此车辆被 " + rentTarget.characterName.Replace("_", " ") + " 租用, 剩余时间: " + v.lscGetdata<int>(EntityData.VehicleEntityData.VehicleRentTime) + " 分钟."; }
                        else { text = "此车已被租用, 剩余时间: " + v.lscGetdata<int>(EntityData.VehicleEntityData.VehicleRentTime) + " 分钟."; }
                    }
                    else
                    {
                        text = "此车已被租用, 剩余时间: " + v.lscGetdata<int>(EntityData.VehicleEntityData.VehicleRentTime) + " 分钟.";
                    }

                    Chat.MainChat.SendInfoChat(p, text);
                }
                else if (v.jobId == 1 && !v.HasData(EntityData.VehicleEntityData.VehicleRentTime))
                {
                    Chat.MainChat.SendInfoChat(p, "[?] 此车可以租用, 输入 /rentcar [时间(分钟)] | 每分钟收费: $" + v.settings.RentPrice);
                }
                if (v.jobId <= 0)
                    p.SendChatMessage("<i class='fad fa-tire'></i>{49A0CD} [" + v.sqlID + "] {C8C8C8}" + model.ToString() + oName + " •{49A0CD} [车牌] {C8C8C8}" + v.NumberplateText);
                
                p.EmitLocked("startKM", v.currentFuel, v.fuelConsumption, v.maxFuel, v.km, v.engineBoost + 10);
                if (v.HasSyncedMetaData(EntityData.VehicleEntityData.isRadarOn)) { p.EmitLocked("APL:Start"); }
                p.oldCar = v.sqlID;

                if (v.settings.SecurityLevel <= 0)
                    v.settings.SecurityLevel = 1;

                if (v.Model == (uint)VehicleModel.Polmav || v.Model == (uint)VehicleModel.Maverick || v.Model == (uint)VehicleModel.Frogger)
                    await p.EmitAsync("Helicam:Start");

                if (v.settings.driftMode)
                {
                    if (!v.HasStreamSyncedMetaData("DriftMode"))
                    {
                        await v.SetStreamSyncedMetaDataAsync("DriftMode", v.settings.driftMode);
                    }
                }
                
                if (p.isFinishTut == 4)
                {
                    p.isFinishTut = 5;
                    p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}我有{fc5e03}车{FFFFFF}了!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}Yo, 终于到这一步了, 你做到了!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}按{fc5e03}Y键{FFFFFF}可以启动车辆引擎!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: 尝试启动一下车辆吧!");                    
                }                
                return;
            }
            catch { return; }
        }

        [AsyncScriptEvent(ScriptEventType.PlayerLeaveVehicle)]
        [Obsolete]
        public async Task OnPlayerLeaveVehicle(VehModel v, PlayerModel p, byte seat)
        {
            if (v.Model == (uint)VehicleModel.Polmav || v.Model == (uint)VehicleModel.Maverick || v.Model == (uint)VehicleModel.Frogger)
                await p.EmitAsync("Helicam:Stop");

            if (v.Model == (uint)VehicleModel.FireTruck)
                p.EmitLocked("Fire:Shoot", false);
            //v.SetPositionAsync(v.Position; // 7.0.0 Update fixed rotation fix.

            if (await DriverSchool.ServerEvent.OnPlayerLeaveVehicle(v, p))
                return;
        }

        [AsyncClientEvent("Player:SetSex")]
        [Obsolete]
        public static void PlayerSetSex(PlayerModel p, int sex)
        {
            if (sex != p.sex)
            {
                p.sex = sex;
                if (p.sex == 0)
                {
                    p.SetModelAsync(Alt.Hash("mp_f_freemode_01"));
                }
                else
                {
                    p.SetModelAsync(Alt.Hash("mp_m_freemode_01"));
                }

                p.EmitAsync("character:SyncServerSkin", p.charComps);
            }

            return;
        }


        [AsyncClientEvent("Server:CommandUsage")]
        public static void CommandLog(PlayerModel p, string commad)
        {
            Core.Logger.WriteLogData(Logger.logTypes.commandLogs, p.characterName + " | CMD: " + commad);
        }

        [AsyncScriptEvent(ScriptEventType.PlayerEvent)]
        public static async Task EVENT_Defender(PlayerModel p, string eventName, object[] args)
        {
            //////if (eventName == "NativeLSC:sendCallBack" || eventName == "FactionBrowser:OOCChat" || eventName.ToLower().Contains("vehicle"))
            //////    return;
            ////if (eventName == "Weapon:BulletUpdate" || eventName == "FactionBrowser:OOCChat")
            //if(eventName.ToLower().Contains("atm:") || eventName.ToLower().Contains("inventory") || eventName.ToLower().Contains("bulletupdate"))
            //    return;


            //if (p.HasData("Defender:LastEvent"))
            //{
            //    if (p.lscGetdata<string>("Defender:LastEvent") == eventName)
            //    {
            //        if (p.lscGetdata<DateTime>("Defender:LastEventUsage").AddMilliseconds(150) > DateTime.Now)
            //        {
            //            if (p.HasData("Defender:LastEventCounter"))
            //            {
            //                if (p.lscGetdata<int>("Defender:LastEventCounter") > 15)
            //                {
            //                    MainChat.SendAdminChat(p.characterName + " isimli oyuncu sunucuya çok fazla emit göndermeye çalıştı. -> " + eventName);
            //                    p.Kick("Sunucuya aynı anda çok fazla veri göndermeye çalıştığınız için bağlantınız kesildi.");
            //                    Core.Logger.WriteLogData(Logger.logTypes.CheatLog, p.characterName + " isimli oyuncu sunucuya çok fazla emit göndermeye çalıştı. -> " + eventName);
            //                }
            //                else
            //                {
            //                    p.SetData("Defender:LastEventCounter", p.lscGetdata<int>("Defender:LastEventCounter") + 1);
            //                }
            //            }
            //            else
            //            {
            //                p.SetData("Defender:LastEventCounter", 1);
            //            }
            //        }
            //        else if (p.lscGetdata<DateTime>("Defender:LastEventUsage").AddMilliseconds(100) > DateTime.Now)
            //        {
            //            if (p.HasData("Defender:LastEventCounter"))
            //            {
            //                if (p.lscGetdata<int>("Defender:LastEventCounter") > 15)
            //                {
            //                    MainChat.SendAdminChat(p.characterName + " isimli oyuncu sunucuya çok fazla emit göndermeye çalıştı. -> " + eventName);
            //                    p.Kick("Sunucuya aynı anda çok fazla veri göndermeye çalıştığınız için bağlantınız kesildi.");
            //                    Core.Logger.WriteLogData(Logger.logTypes.CheatLog, p.characterName + " isimli oyuncu sunucuya çok fazla emit göndermeye çalıştı. -> " + eventName);
            //                }
            //                else
            //                {
            //                    p.SetData("Defender:LastEventCounter", p.lscGetdata<int>("Defender:LastEventCounter") + 1);
            //                }
            //            }
            //            else
            //            {
            //                p.SetData("Defender:LastEventCounter", 1);
            //            }

            //        }
            //        else
            //        {
            //            p.lscSetData("Defender:LastEventUsage", DateTime.Now);
            //        }
            //    }
            //    else
            //    {
            //        p.lscSetData("Defender:LastEvent", eventName);
            //        p.lscSetData("Defender:LastEventUsage", DateTime.Now);
            //        p.lscSetData("Defender:LastEventCounter", 1);
            //    }
            //}
            //else
            //{
            //    p.lscSetData("Defender:LastEvent", eventName);
            //    p.lscSetData("Defender:LastEventUsage", DateTime.Now);
            //}
        }

        [AsyncScriptEvent(ScriptEventType.PlayerConnect)]
        public async Task OnPlayerConnect(IPlayer player, string reason)
        {
            Random rnd = new Random();
            int currCam = rnd.Next(0, 5);
            Position curPos = cameras[currCam].camera;
            curPos.Z -= 5;
            player.Position = curPos;
            GlobalEvents.FreezeEntity((PlayerModel)player, true);
            GlobalEvents.CreateCamera((PlayerModel)player, cameras[currCam].camera, new Rotation(0, 0, 0), fov: 50);
            GlobalEvents.LookCamera((PlayerModel)player, cameras[currCam].lookat);
            bool isBanned = await Database.DatabaseMain.CheckSocialBan(player.SocialClubId);
            if (isBanned) { player.Kick("很抱歉, 您的R星账号已被封禁."); return; }
            bool isHwidBanned = await Database.DatabaseMain.CheckHwidBan(player);
            if (isHwidBanned) { player.Kick("很抱歉, 您已被服务器封禁."); return; }
            DoorSystem.LoadDoorsToPlayer((PlayerModel)player);
            string json = JsonConvert.SerializeObject(Database.DatabaseMain.getUpdateInfo());
            player.EmitLocked("login:Start", json);
            player.Dimension = 0;
            //await Discord.Main.PushServerStatus();
            await KookSpace.JoinMessage(player.Name);
            Prometheus.PlayerCounter(Alt.GetAllPlayers().Count);
        }
        
        [AsyncServerEvent("chat:message")]
        public void OnPlayerChatMessage(PlayerModel player, string msg)
        {
            MainChat.NormalChat(player, msg);
        }

        public class LoginCameras
        {
            public Position camera { get; set; }
            public Position lookat { get; set; }
        }

        public static List<LoginCameras> cameras = new List<LoginCameras>() {
            new LoginCameras(){ camera = new Position(-94, -763, 46), lookat = new Position(-132, -656, 86)},
            new LoginCameras(){ camera = new Position(710, 1104, 330), lookat = new Position(728, 1192, 337)},
            new LoginCameras(){ camera = new Position(-1574, 2104, 69), lookat = new Position(-1607, 2094, 72)},
            new LoginCameras(){ camera = new Position(-1124, -286, 38), lookat = new Position(-1113, -270, 41)},
            new LoginCameras(){ camera = new Position(-813, -1409, 5), lookat = new Position(-898, -1373, 11)},
            new LoginCameras(){ camera = new Position(2115, 1862, 131), lookat = new Position(2167, 1929, 133)}
        };

        [AsyncClientEvent("player:EventKey:N")]
        public async static Task PlayerEventKey_N(PlayerModel player)
        {
            player.EmitLocked("wheelnav:open");
        }
        
        [AsyncClientEvent("player:EventKey:E")]
        [Obsolete]
        public async static Task PlayerEventKey_E(PlayerModel player)
        {
            //interior giriş çıkış
            if (player.Vehicle == null)
            {
                if (Vehicle.VehicleMain.leaveCaravan(player))
                    return;

                if (await Vehicle.VehicleMain.tryLoginCaravan(player))
                    return;

                if (await OtherSystem.LSCsystems.DoorSystem.DoorUseEvent(player) == true) { return; }

                if (sellingPoints.Key_Buy(player))
                    return;
                
                if (MainCommands.Key_BuyClothes(player))
                    return;
                
                if (Barber.Key_StartTatto(player))
                    return;
                
                if (Barber.Key_OpenBarber(player))
                    return;

                if (await Plants.ServerEvent.PlantsKeyTrigger(player))
                    return;

                if (await DriverSchool.ServerEvent.InteractionTrigger(player))
                    return;
                
                PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Find(x => player.Position.Distance(x.Position) < 3f && x.Dimension == player.Dimension);
                if (entranceLabel != null)
                {
                    if (entranceLabel.TryGetData(EntityData.EntranceTypes.EntranceType, out int entranceType))
                    {
                        switch (entranceType)
                        {
                            case 1:
                                entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position pos);
                                player.Position = pos;
                                player.Dimension = 0;
                                // OtherSystem.LSCsystems.Boombox.StopSoundDirectly(player);
                                Props.Business.EVENT_BusinessStopTV(player);
                                player.EmitLocked("InteriorManager:ExitInterior");
                                player.SetWeather((uint)ServerEvent.CurrentWeather);
                                GlobalEvents.SetLightState(player, true);
                                return;

                            default:
                                break;
                        }
                    }
                    else if (entranceLabel.TryGetData("isTeleporter", out int teleID))
                    {
                        await teleporters.ShowTeleporter(player, teleID);
                        return;
                    }

                }

                (HouseModel, PlayerLabel, Marker) house = await Props.Houses.getNearHouse(player);
                if (house.Item1 != null)
                {
                    if (house.Item1.isLocked == false)
                    {
                        player.SetWeather(WeatherType.Clear);
                        player.Position = house.Item1.intPos;
                        player.Dimension = house.Item1.dimension;

                        GlobalEvents.FreezeEntity(player, true);
                        player.SetData("Interior:Loading", true);

                        await player.EmitAsync("InteriorManager:LoadInterior", JsonConvert.SerializeObject(house.Item1.settings), house.Item1.intPos.X, house.Item1.intPos.Y, house.Item1.intPos.Z);

                        await Task.Delay(3500);
                        if (!player.Exists)
                            return;

                        GlobalEvents.FreezeEntity(player);
                        player.DeleteData("Interior:Loading");

                        // OtherSystem.LSCsystems.Boombox.HousePlaySound(player, house.Item1);
                        //Props.Houses.EVENT_StartTV(player, house.Item1);
                        GlobalEvents.SetLightState(player, house.Item1.settings.Light);

                        return;
                    }
                    else
                    {
                        MainChat.SendErrorChat(player, "> 此房屋是锁的.");
                        return;
                    }
                }



                (BusinessModel, Marker, PlayerLabel) biz = await Props.Business.getNearestBusiness(player);
                if (biz.Item1 != null)
                {
                    if (player.HasData("Interior:Loading"))
                        return;

                    if (biz.Item1.isLocked == true) { MainChat.SendErrorChat(player, "此产业是锁的."); return; }

                    if (player.LastBusiness != biz.Item1.ID)
                    {

                        if (biz.Item1.ID != player.businessStaff && biz.Item1.ownerId != player.sqlID)
                        {
                            if (player.cash < biz.Item1.entrancePrice) { MainChat.SendErrorChat(player, "您支付不起产业入场费."); return; }
                            if (biz.Item1.entrancePrice > 0)
                            {
                                player.cash -= biz.Item1.entrancePrice;
                                biz.Item1.vault += biz.Item1.entrancePrice;

                                await player.updateSql();
                                await biz.Item1.Update(biz.Item2, biz.Item3);
                            }
                        }
                    }

                    player.LastBusiness = biz.Item1.ID;

                    player.SetWeather(WeatherType.Clear);
                    player.Position = biz.Item1.interiorPosition;
                    player.Dimension = biz.Item1.dimension;

                    GlobalEvents.FreezeEntity(player, true);
                    player.SetData("Interior:Loading", true);

                    await player.EmitAsync("InteriorManager:LoadInterior", JsonConvert.SerializeObject(biz.Item1.settings), biz.Item1.interiorPosition.X, biz.Item1.interiorPosition.Y, biz.Item1.interiorPosition.Z);

                    await Task.Delay(3500);
                    if (!player.Exists)
                        return;

                    GlobalEvents.FreezeEntity(player);
                    player.DeleteData("Interior:Loading");

                    // OtherSystem.LSCsystems.Boombox.BusinessPlaySound(player, biz.Item1);
                    //Props.Business.EVENT_BusinessPlayTV(player, biz.Item1);
                    GlobalEvents.SetLightState(player, biz.Item1.settings.Light);

                    return;
                }

                if (await GarageModel.enterGarage(player))
                    return;


                if (OtherSystem.LSCsystems.PDelevator.UseElevator(player) == true) { return; }
                await OtherSystem.GlobalJobs.StartSearchTrunk(player);

                if (OtherSystem.LSCsystems.Tent.JoinOrLeaveTent(player))
                    return;
                
                if (await OtherSystem.LSCsystems.JackingNPC.TryJack(player))
                    return;
            }
            else
            {
                PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Find(x => player.Position.Distance(x.Position) < 3f && x.Dimension == player.Dimension);
                if (entranceLabel != null)
                {
                    entranceLabel.TryGetData(EntityData.EntranceTypes.EntranceType, out int entranceType);
                    switch (entranceType)
                    {
                        case 1:
                            entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position pos);

                            if (player.Vehicle != null)
                            {
                                VehModel _pveh2 = (VehModel)player.Vehicle;
                                int _pfuel2 = _pveh2.currentFuel;
                                player.Vehicle.Position = pos;
                                player.Vehicle.Dimension = 0;
                                player.Dimension = 0;
                                await player.Vehicle.NetworkOwner.EmitAsync("Vehicle:FreezePos", player.Vehicle, pos.X, pos.Y, pos.Z);
                                player.EmitLocked("InteriorManager:ExitInterior");
                                player.SetWeather((uint)ServerEvent.CurrentWeather);
                                _pveh2.currentFuel = _pfuel2;

                                return;
                            }

                            VehModel _pveh = (VehModel)player.Vehicle;
                            int _pfuel = _pveh.currentFuel;
                            player.Position = pos;
                            player.Dimension = 0;
                            player.EmitLocked("InteriorManager:ExitInterior");
                            GlobalEvents.SetLightState(player, true);
                            _pveh.currentFuel = _pfuel;
                            return;

                        default:
                            break;
                    }
                }

                if (await GarageModel.enterGarage(player))
                    return;
                
                if (await OtherSystem.LSCsystems.JackingNPC.TryJack(player))
                    return;
            }

            return;
        }

        [ScriptEvent(ScriptEventType.ConsoleCommand)]
        [Obsolete]
        public async void Test(string name, string[] args)
        {
            if (name == "kickall")
            {
                string kick_message = string.Join(" ", args);
                int counter = 0;
                foreach (IPlayer kickplayer in Alt.GetAllPlayers())
                {
                    ++counter;
                    kickplayer.KickAsync(kick_message);
                }

                Alt.Log(counter.ToString() + " 名玩家被踢出服务器, 原因: " + kick_message);
                return;
            }
            else if (name == "ann")
            {
                string announce = string.Join(" ", args);
                int counter = 0;
                foreach (IPlayer kickplayer in Alt.GetAllPlayers())
                {
                    ++counter;
                    MainChat.SendErrorChat(kickplayer, "[公告] {FFFFFF}" + announce);
                }

                Alt.Log("公告 -> " + announce + "\n " + counter.ToString() + " 发布公告.");
                return;
            }
            else if (name == "sifre")
            {
                string pass = string.Join(" ", args);
                Alt.Core.SetPassword(pass);
                Alt.Log("Sunucu şifresi " + pass + " olarak ayarlandı.");
            }
            else if (name == "sunucukapat")
            {
                foreach (PlayerModel p in Alt.GetAllPlayers())
                {
                    await p.updateSql();
                }
                foreach (VehModel v in Alt.GetAllVehicles())
                {
                    v.Update();
                }
                await Database.DatabaseMain.SaveServerSettings();
                Alt.Log("Araç bilgileri kaydedildi.");
                Alt.Log("Karakter bilgileri kaydedildi.");
                await Database.DatabaseMain.SaveServerSettings();
                Alt.Log("Çiftlik bilgileri kaydedildi.");
                Alt.Log("Market sistemi kaydedildi.");
                Alt.StopResource("outRp");
            }
            else if (name == "sunucudurum")
            {
                //Discord.Main.userCanJoin = !Discord.Main.userCanJoin;
                Alt.Log("if (name == \"sunucudurum\").");
            }
            else if (name == "silahlaritemizle")
            {
                Secim.COM_ClearAllWeapons();
            }
            else
            {
                Alt.Log("Böyle bir konsol komutu bulunamadı...");
                return;
            }
            return;
        }

        [ScriptEvent(ScriptEventType.Fire)]
        public void Event_Fire(IPlayer player, AltV.Net.FireInfo[] fires)
        {
            if (!Globals.System.FD_FireSystem.canFire)
                return;

            foreach (var fi in fires)
            {
                Globals.System.FD_FireSystem.EventAddFire(fi.Position, player.Dimension);
            }
        }

        [ScriptEvent(ScriptEventType.PlayerWeaponChange)]
        public void Event_FireWeapon(IPlayer player, uint oldWeapon, uint newWeapon)
        {
            if (oldWeapon == (uint)WeaponModel.FireExtinguisher)
            {
                player.EmitLocked("Fire:Shoot", false);
            }
            else if (newWeapon == (uint)WeaponModel.FireExtinguisher)
            {
                player.EmitLocked("Fire:Shoot", true);
            }
        }

        [ScriptEvent(ScriptEventType.PlayerCustomEvent)]
        public void Custom_Events(PlayerModel player, string eventName, MValueConst[] args)
        {
            lock (player.Emits)
            {
                //player.Emits
                if (player.adminLevel < 5 && player.Ping < 80)
                {
                    var date = DateTime.Now;
                    
                    player.Emits.ForEach(emit =>
                    {
                        //if (emit.Name == eventName && emit.args.All(x => args.All(y => (x.GetType() == y.GetType()) && x.Equals(y))) && (date - emit.Date).TotalMilliseconds <= 250)
                        if (emit.Name == eventName && (date - emit.Date).TotalMilliseconds <= 30)
                        {
                            //ACBAN(p, 1, "Silah Hilesi.");                            
                            antiCheat.BAN(player, 1, "作弊 ~ " + eventName + "\n 如果您认为此封禁有误, 请将本文截图并开工单联系管理员. \n" + DateTime.Now.ToString());
                            MainChat.SendAdminChat("反作弊系统: " + player.characterName + " 涉嫌作弊, 程序: " + eventName);
                            //player.Kick("Dupe ~ " + eventName + "\n Bu yasaklanmanın hatalı olduğunu düşünüyorsanız, bu yazının ekran görüntüsünü alarak ticket açarak yöneticilerle iletişime geçmelisiniz. \n" + DateTime.Now.ToString());

                            return;
                        }
                    });



                    var log = new EmitLog
                    {
                        Name = eventName,
                        Date = date,
                        args = args
                    };

                    if (
                        eventName != "chat:message" &&
                        eventName != "cPhone:GetMessages" &&
                        eventName != "cPhone:CallNumber" &&
                        eventName != "cPhone:CloseCurrentCall" &&
                        eventName != "Player:SetSex" &&
                        eventName != "Player:SetSkin" &&
                        eventName != "FactionBrowser:OOCChat" &&
                        eventName != "Phone:OnHand" &&
                        eventName != "cPhone:SelectMessage" &&
                        eventName != "NativeLSC:sendCallBack" &&
                        eventName != "cPhone:DeleteMessage" &&
                        eventName != "LoginAttemp" &&
                        eventName != "cPhone:Hoparlor" &&
                        eventName != "Vehicle:SetModify" &&
                        eventName != "player:EventKey:E" &&
                        eventName != "NPC:SetNetOwner" &&
                        eventName != "Vehicle:SetSecondColor" &&
                        eventName != "Vehicle:SetWheelColor" &&
                        eventName != "Weapon:BulletUpdate" &&
                        eventName != "Truck:EnterVehicle" &&
                        eventName != "Truck:ExitVehicle" &&
                        eventName != "Vehicle:Park" &&
                        !eventName.Contains("NPC:") &&
                        !eventName.Contains("Fire:") &&
                        !eventName.Contains("Vehicle:")
                        )
                        player.Emits.Add(log);
                    if (player.Emits.Count > 10)
                    {
                        player.Emits.RemoveAt(0);
                    }
                }

                Logger.WriteLogData(Logger.logTypes.emitlog, player.characterName + " -> " + eventName);
            }
        }


    }
    // 
}
