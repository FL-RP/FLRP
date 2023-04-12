using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Database;
using outRp.Globals;
using outRp.Models;
using outRp.OtherSystem.LSCsystems;
using outRp.OtherSystem.Textlabels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;
using outRp.Utils;

namespace outRp.OtherSystem
{
    public class Inventory : IScript
    {
        public class GroundObj
        {
            public int ID { get; set; }
            public OtherSystem.Textlabels.LProp Prop { get; set; }
            public PlayerLabel textLabel { get; set; }
            public string data { get; set; }
        }

        public static List<GroundObj> groundObjects = new List<GroundObj>();

        // Setup Enventar 
        public static void LoadInventoryListener()
        {
            //Alt.OnClient<PlayerModel, int, int, int>("inventory:wantUse", InventoryAction);
            Items.LoadAllItems();
            return;
        }
        public static async Task LoadPlayerInventory(PlayerModel p)
        {
            await DatabaseMain.GetPlayerInventoryItems(p.sqlID);
            return;
        }
        public static ServerItems GetItemType(int itemId)
        {
            ServerItems x = Items.LSCitems.Find(x => x.ID == itemId);
            return x;
        }

        public static byte getItemComponent(int itemID, bool isProp = false)
        {
            if (isProp)
            {
                switch (itemID)
                {
                    case 4: return 1;
                    case 6: return 0;
                    case 10: return 7;
                    case 12: return 6;
                    default: return 0;
                }
            }
            else
            {
                switch (itemID)
                {
                    case 5: return 1;
                    case 7: return 7;
                    case 8: return 11;
                    case 9: return 8;
                    case 11: return 4;
                    case 13: return 9;
                    case 14: return 6;
                    default: return 0;
                }
            }
        }

        public static void Clothes(PlayerModel p, int slot, int variation, int texture, bool isProp = false)
        {
            /*await AltAsync.Do(() =>
            {

                if (isProp)
                {
                    p.SetProps(Convert.ToByte(slot), (ushort)variation, Convert.ToByte(texture));
                }
                else
                {
                    p.SetClothes(Convert.ToByte(slot), (ushort)variation, Convert.ToByte(texture), 2);
                }

            });*/
        }
        public static async Task UpdatePlayerInventory(PlayerModel player)
        {
            List<InventoryModel> playerInventory = await DatabaseMain.getPlayerClientInventoryItems(player.sqlID);
            /*if (!player.HasSyncedMetaData("PlayerClothesLock"))
            {    
                player.ClearProps(0); player.ClearProps(1); player.ClearProps(6); player.ClearProps(7);        
                if(player.sex == 0)
                {
                    player.SetClothes(1, 0, 0, 2);
                    player.SetClothes(7, 0, 0, 2);
                    player.SetClothes(11, 1, 20, 2);
                    player.SetClothes(8, 1, 20, 2);
                    player.SetClothes(3, 15, 0, 2);
                    player.SetClothes(6, 35, 0, 2);
                    player.SetClothes(9, 1, 20, 2);
                    player.SetClothes(4, 15, 0, 2);
                }
                else
                {
                    player.SetClothes(1, 0, 0, 2);
                    player.SetClothes(7, 0, 0, 2);
                    player.SetClothes(11, 1, 20, 2);
                    player.SetClothes(8, 1, 20, 2);
                    player.SetClothes(3, 15, 0, 2);
                    player.SetClothes(6, 34, 0, 2);
                    player.SetClothes(9, 1, 20, 2);
                    player.SetClothes(4, 21, 0, 2);
                }
                foreach(InventoryModel i in playerInventory)
                {
                    if(i.itemSlot != 0)
                    {
                        if(i.itemId == 5 || i.itemId == 7 || i.itemId == 8 || i.itemId == 9 || i.itemId == 11 ||
                            i.itemId == 13 || i.itemId == 14)
                        {
                            if (!ushort.TryParse(i.itemData, out ushort variation) || !byte.TryParse(i.itemData2, out byte texture))
                                continue;
                        
                            player.SetClothes(getItemComponent(i.itemId), variation, texture, 2);
                        }
                        else if(i.itemId == 4 || i.itemId == 6 || i.itemId == 10 || i.itemId == 12)
                        {
                            if (!ushort.TryParse(i.itemData, out ushort variation) || !byte.TryParse(i.itemData2, out byte texture))
                                continue;

                            player.SetProps(getItemComponent(i.itemId, true), variation, texture);
                        }
                    }
                }
            }*/
            string json = JsonConvert.SerializeObject(playerInventory);
            await player.EmitAsync("inventory:Update", json);
            return;
        }
        public static async Task<bool> AddInventoryItem(PlayerModel p, ServerItems item, int amount)
        {
            bool hasItem = await DatabaseMain.CheckInventoryItem(p.sqlID, item.ID);
            if (await InvTotalSlot(p) + 1 > 30) { return false; }
            if (await InvTotalWeight(p) + (item.weight * amount) > p.Strength + p.tempStrength)
                return false;

            if (hasItem == false)
            {
                InventoryModel checkItem = new InventoryModel
                {
                    ownerId = p.sqlID,
                    itemName = item.name,
                    itemId = item.ID,
                    itemAmount = amount,
                    itemData = item.data,
                    itemData2 = item.data2,
                    itemWeight = item.weight,
                    itemPicture = item.picture,
                    Equipable = item.equipable
                };
                await checkItem.Create();
            }
            else
            {
                if (item.stackable)
                {
                    InventoryModel checkItem = await DatabaseMain.FindInventoryItemWithOwnerId(p.sqlID, item.ID);
                    checkItem.itemAmount += amount;
                    await checkItem.Update();
                }
                else
                {
                    InventoryModel checkItem = new InventoryModel
                    {
                        ownerId = p.sqlID,
                        itemName = item.name,
                        itemId = item.ID,
                        itemAmount = amount,
                        itemData = item.data,
                        itemData2 = item.data2,
                        itemWeight = item.weight,
                        itemPicture = item.picture,
                        Equipable = item.equipable
                    };
                    await checkItem.Create();
                }

            }
            GlobalEvents.NativeNotify(p, "~w~+" + amount + " ~g~" + item.name);
            await UpdatePlayerInventory(p);
            return true;
        }
        public static async Task RemoveInventoryItem(PlayerModel p, int itemId, int amount)
        {
            InventoryModel i = await DatabaseMain.FindInventoryItem(itemId);
            if (i == null) { return; }

            if (i.itemSlot > 0) { i.itemSlot = 0; await i.Update(); await UpdatePlayerInventory(p); }

            /*if (i.itemAmount <= amount || i.itemAmount <= 0)
            {
                i.Delete();
            }
            else
            {
                i.itemAmount -= amount;
                i.Update();
            }*/
            i.itemAmount -= amount;
            if (i.itemAmount <= 0)
                await i.Delete();
            else
                await i.Update();

            GlobalEvents.NativeNotify(p, "~w~-" + amount + " ~r~" + i.itemName);
            await UpdatePlayerInventory(p);
            p.EmitLocked("Inv:CanUseYes");
            return;
        }
        public static async Task ItemClearSlot(PlayerModel p, int itemSlot)
        {
            List<InventoryModel> x = await DatabaseMain.GetPlayerInventoryItems(p.sqlID);
            InventoryModel a = x.Find(x => x.itemSlot == itemSlot);
            if (a == null) { return; }
            if (a.itemId == 28 || a.itemId == 29 || a.itemId == 30)
            {
                a.itemSlot = 0;
                await WeaponSystem.TakeOutWeapon(p, a);
                return;
            }
            a.itemSlot = 0;
            await a.Update();
        }
        public static async Task<double> InvTotalWeight(PlayerModel player)
        {
            double a = 0;
            List<InventoryModel> items = await DatabaseMain.GetPlayerInventoryItems(player.sqlID);
            foreach (var item in items)
            {
                a += item.itemWeight * item.itemAmount;
            }

            return a;
        }
        public static async Task<int> InvTotalSlot(PlayerModel player)
        {
            List<InventoryModel> items = await DatabaseMain.GetPlayerInventoryItems(player.sqlID);
            return items.Count + 1;
        }

        [ClientEvent("inventory:wantUse")]
        public static async Task InventoryAction(PlayerModel p, int itemId, int usageType, int usageAmount)
        {
            if (p.Ping > 250)
                return;

            try
            {
                Random rnd = new Random();
                // Usage Types : 1 -> Envanter içinden direk kullan | 2 -> Ver | 3 -> Yere bırak | 4 -> Arabaya bırak | 5 -> Kutuya Bırak
                List<InventoryModel> pInv = await DatabaseMain.GetPlayerInventoryItems(p.sqlID);
                InventoryModel i = pInv.Find(x => x.ID == itemId);
                if (i == null) { /*Alt.Log("item bulamadım");*/ return; }
                ServerItems item = Items.LSCitems.Find(x => x.ID == i.itemId);

                switch (usageType)
                {
                    case 1:
                        switch (item.type)
                        {
                            case 1:
                                ServerItems case1Data = Items.LSCitems.Find(x => x.ID == i.itemId);
                                if (i.itemSlot != 0)
                                {
                                    await OtherSystem.Phone.PhoneMain.TakeOutPhone(p, i);
                                    //Phones.TakeOffPhone(p, i);
                                    p.EmitLocked("Inv:CanUseYes");
                                }
                                else
                                {
                                    await OtherSystem.Phone.PhoneMain.TakePhone(p, i);
                                    //Phones.TakePhone(p, i);
                                    p.EmitLocked("Inv:CanUseYes");

                                }
                                break;

                            case 2:
                                MainChat.SendInfoChat(p, "[?] 汉堡包的使用被禁止.");
                                GlobalEvents.InvForceClose(p);
                                /*if(p.injured.Injured || p.injured.isDead) { MainChat.SendErrorChat(p, "[错误] Yaralıyken hamburger yiyemezsiniz. Lütfen önce hastaneye giderek tedavi olun."); return; }
                                ServerItems case2Data = Items.LSCitems.Find(x => x.ID == i.itemId);
                                p.maxHp = 1000;
                                if (p.Health < 800) { p.Health += ushort.Parse(case2Data.data); p.hp += ushort.Parse(case2Data.data); }
                                else { MainChat.SendErrorChat(p, "[!] Can değeriniz yüksek görünüyor. Daha fazla hamburger yiyemezsiniz."); return; }
                                MainChat.EmoteDo(p, "Hamburger yedi.");
                                RemoveInventoryItem(p, itemId, usageAmount); */
                                break;

                            case 3:
                                VehModel veh = Vehicle.VehicleMain.getNearVehFromPlayer(p);
                                if (veh == null) { MainChat.SendErrorChat(p, CONSTANT.COM_VQueryVehNotNear); break; }
                                veh.BodyHealth += uint.Parse(i.itemData);
                                if (veh.BodyHealth >= 800) { veh.BodyHealth = 800; veh.Dimension = 9999; /*await Task.Delay(500);*/ veh.Dimension = p.Dimension; }
                                veh.Dimension = p.sqlID;
                                veh.DamageData = "AA==";
                                //await Task.Delay(500);
                                veh.Dimension = p.Dimension;
                                await RemoveInventoryItem(p, itemId, usageAmount);
                                p.EmitLocked("Inv:CanUseYes");
                                break;

                            case 4:
                                ServerItems case4Data = Items.LSCitems.Find(x => x.ID == i.itemId);
                                if (i.itemSlot != 0) { i.itemSlot = 0; if (i.itemId == 5) { p.fakeName = p.characterName; p.showSqlId = true; } }
                                else { await ItemClearSlot(p, case4Data.equipSlot); i.itemSlot = case4Data.equipSlot; if (i.itemId == 5) { p.fakeName = "陌生人_" + p.sqlID.ToString() + rnd.Next(1000, 2000).ToString(); p.showSqlId = false; } }
                                await i.Update();
                                await UpdatePlayerInventory(p);
                                break;

                            case 5:
                                ServerItems case5Data = Items.LSCitems.Find(x => x.ID == i.itemId);
                                InventoryModel case5Lighter = pInv.Find(x => x.itemId == 6);
                                if (case5Lighter == null) { MainChat.SendInfoChat(p, CONSTANT.ERR_SmokeLighterNotFound); return; }
                                MainChat.EmoteMe(p, ServerEmotes.EMOTE_Smoke);
                                await RemoveInventoryItem(p, itemId, usageAmount);
                                break;

                            case 6:
                                MainChat.SendInfoChat(p, CONSTANT.ERR_LighterUsageAlone);
                                break;

                            case 7:
                                //DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                                PlayerModelInfo case7Target = await Database.DatabaseMain.getCharacterInfo(Int32.Parse(i.itemData));
                                if (case7Target == null) { MainChat.SendErrorChat(p, "[错误] 发生了错误."); p.EmitLocked("Inv:CanUseYes"); return; }
                                CharacterSettings case7targetSettings = JsonConvert.DeserializeObject<CharacterSettings>(case7Target.settings);
                                if (case7targetSettings.driverLicense == null) { MainChat.SendErrorChat(p, "[错误] 发生了错误2."); return; }
                                PlayerModel case7TargetModel = GlobalEvents.GetPlayerFromSqlID(case7Target.sqlID);
                                if (case7TargetModel == null) { case7TargetModel = p; }
                                GlobalEvents.ShowNotification(p, "~w~驾驶证信息 - 生效日期: " + case7targetSettings.driverLicense.licenseDate.ToString("yyyy/MM/dd HH:mm") + "~n~惩罚: " + case7targetSettings.driverLicense.finePoint.ToString(), "驾驶证", case7Target.characterName.Replace("_", " "), case7TargetModel);
                                MainChat.EmoteDoAlternative(p, "可以看到驾驶证上写着生效日期: " + case7targetSettings.driverLicense.licenseDate.ToString("yyyy/MM/dd HH:mm") + " 惩罚: " + case7targetSettings.driverLicense.finePoint.ToString() + " ||" + case7Target.characterName.Replace("_", " "));
                                p.EmitLocked("Inv:CanUseYes");
                                break;

                            case 8:
                                VehModel case8veh = Vehicle.VehicleMain.getNearVehFromPlayer(p);
                                if (case8veh == null) { MainChat.SendErrorChat(p, CONSTANT.COM_VQueryVehNotNear); p.EmitLocked("Inv:CanUseYes"); break; }
                                case8veh.currentFuel += 20;
                                if (case8veh.currentFuel >= case8veh.maxFuel) { case8veh.currentFuel = case8veh.maxFuel; }
                                MainChat.EmoteMe(p, ServerEmotes.EMOTE_FillFuel);
                                await RemoveInventoryItem(p, itemId, usageAmount);
                                break;

                            case 9:
                                InventoryModel case9Lighter = pInv.Find(x => x.itemId == 6);
                                if (case9Lighter == null) { MainChat.SendInfoChat(p, "[信息] 您需要打火机来烧木头."); return; }
                                // TODO TODO yanan ateş eklenecek.
                                //RemoveInventoryItem(p, itemId, usageAmount);
                                break;

                            //case 10:
                            //    VehModel case10veh = Vehicle.VehicleMain.getNearVehFromPlayer(p);
                            //    if (case10veh == null) { MainChat.SendErrorChat(p, CONSTANT.COM_VQueryVehNotNear); break; }
                            //    for(byte case10i = 0; case10i <= case10veh.WheelsCount; case10i++ )
                            //    {
                            //        if(case10veh.IsWheelBurst(case10i) == true)
                            //        {
                            //            MainChat.EmoteMe(p, ServerEmotes.EMOTE_RepairWheel);
                            //            p.SendChatMessage(string.Format(CONSTANT.INFO_WheelRepaired, case10i.ToString()));
                            //            RemoveInventoryItem(p, itemId, usageAmount);
                            //            await Task.Delay(10000);

                            //            if (!p.Exists)
                            //                return;

                            //            MainChat.EmoteDo(p, string.Format(ServerEmotes.EMOTEDO_RepairWheelDone, case10i.ToString()));
                            //            case10veh.Dimension = 999;
                            //            case10veh.SetWheelHealth(case10i, 100);
                            //            case10veh.SetWheelBurst(case10i, false);
                            //            case10veh.SetWheelHasTire(case10i, true);                                        
                            //            await Task.Delay(500);

                            //            if (!p.Exists)
                            //                return;

                            //            case10veh.Dimension = p.Dimension;
                            //            break;
                            //        }
                            //    }
                            //    break;

                            case 11:
                                // TODO BANKA KARTI DÜZENLENECEK
                                break;

                            case 13:
                                /*
                                if (farming.WantToPlaceFruit(p))
                                {
                                    RemoveInventoryItem(p, itemId, usageAmount);
                                    return;
                                }*/
                                break;

                            case 14:/*
                                if (farming.GrowUpFruit(p))
                                {
                                    RemoveInventoryItem(p, itemId, usageAmount);
                                }         */
                                break;

                            case 16:
                                p.SetData(EntityData.PlayerEntityData.UsingItem, i);
                                GlobalEvents.InvForceClose(p);
                                p.EmitLocked("ATM:WantToUse");
                                p.EmitLocked("Inv:CanUseYes");
                                break;

                            case 17:
                                GlobalEvents.InvForceClose(p);
                                SalesMan.createSaleStand(p);
                                break;

                            case 19:
                                GlobalEvents.InvForceClose(p);
                                Boombox.CreateBoomBox(p);
                                break;

                            //TODO Silah Sistemi
                            case 20: //! Yakın dövüş silahı                                
                                ServerItems case20Data = Items.LSCitems.Find(x => x.ID == i.itemId);
                                if (i.itemSlot != 0) { i.itemSlot = 0; await i.Update(); await WeaponSystem.TakeOutWeapon(p, i); }
                                else { await ItemClearSlot(p, case20Data.equipSlot); i.itemSlot = case20Data.equipSlot; await i.Update(); WeaponSystem.TakeWeapon(p, i); }
                                //UpdatePlayerInventory(p);
                                p.EmitLocked("Inv:CanUseYes");
                                break;

                            case 21:
                                ServerItems case21Data = Items.LSCitems.Find(x => x.ID == i.itemId);
                                if (i.itemSlot != 0) { i.itemSlot = 0; await i.Update(); await WeaponSystem.TakeOutWeapon(p, i); }
                                else { await ItemClearSlot(p, case21Data.equipSlot); i.itemSlot = case21Data.equipSlot; await i.Update(); WeaponSystem.TakeWeapon(p, i); }
                                //UpdatePlayerInventory(p);
                                p.EmitLocked("Inv:CanUseYes");
                                break;

                            case 22:
                                ServerItems case22Data = Items.LSCitems.Find(x => x.ID == i.itemId);
                                if (i.itemSlot != 0) { i.itemSlot = 0; await i.Update(); await WeaponSystem.TakeOutWeapon(p, i); }
                                else { await ItemClearSlot(p, case22Data.equipSlot); i.itemSlot = case22Data.equipSlot; await i.Update(); WeaponSystem.TakeWeapon(p, i); }
                                //UpdatePlayerInventory(p);
                                p.EmitLocked("Inv:CanUseYes");
                                break;
                            //TODO Silah sistemi Son

                            case 23:
                                bool status = LSCsystems.objectSystem.placePlayerObject(p, i.itemData);
                                if (status) { await RemoveInventoryItem(p, itemId, 1); GlobalEvents.InvForceClose(p); }
                                p.EmitLocked("Inv:CanUseYes");
                                return;

                            // TODO Yavru hayvan yerleştirme;
                            case 24: //saman
                                /*
                                bool case24Result = OtherSystem.LSCsystems.farming.growUpAnimal(p);
                                if(case24Result) RemoveInventoryItem(p, itemId, usageAmount);*/
                                return;

                            case 25: // inek yavrusu
                                /*
                                if (OtherSystem.LSCsystems.farming.PlaceAnimal(p))
                                {
                                    RemoveInventoryItem(p, itemId, usageAmount);
                                }*/
                                return;

                            case 26: // İnek eti.
                                await Props.Business.makeDinner(p, 1, i);
                                p.EmitLocked("Inv:CanUseYes");
                                return;

                            case 27:
                                if (Drug.UseDrug(p, i.itemData2))
                                {
                                    await RemoveInventoryItem(p, itemId, usageAmount);
                                }
                                p.EmitLocked("Inv:CanUseYes");
                                break;

                            case 28:
                                bool case28Result = Drug.PlaceWeed(p);
                                if (case28Result) await RemoveInventoryItem(p, itemId, usageAmount);
                                p.EmitLocked("Inv:CanUseYes");
                                break;

                            case 29:
                                bool case29Result = Drug.InteractionDrugFarm(p);
                                if (case29Result) await RemoveInventoryItem(p, itemId, usageAmount);
                                p.EmitLocked("Inv:CanUseYes");
                                break;

                            case 30:
                                OtherSystem.LSCsystems.Fishing.StartFishing(p, i);
                                break;

                            case 31:
                                await Props.Business.makeDinner(p, 2, i);
                                p.EmitLocked("Inv:CanUseYes");
                                return;

                            case 32:
                                Globals.System.FD.useMedicine(p, i.itemData2);
                                await RemoveInventoryItem(p, itemId, usageAmount);
                                break;

                            case 33:
                                if (WeaponSystem.AddBulletFromMagazine(p, i))
                                {
                                    await RemoveInventoryItem(p, itemId, usageAmount);
                                    return;
                                }
                                else { MainChat.SendErrorChat(p, "[错误] 您没有装备武器!"); p.EmitLocked("Inv:CanUseYes"); return; }


                            case 34:
                                if (WeaponSystem.AddComponentFromItem(p, i))
                                {
                                    await RemoveInventoryItem(p, itemId, usageAmount);
                                    return;
                                }
                                else { MainChat.SendErrorChat(p, "[错误] 您没有装备武器来安装这个配件!"); p.EmitLocked("Inv:CanUseYes"); return; }

                            case 35:
                                if (WeaponSystem.AddWeaponTintFromItem(p, i))
                                {
                                    await RemoveInventoryItem(p, itemId, usageAmount);
                                    return;
                                }
                                else { MainChat.SendErrorChat(p, "[错误] 当前武器无法使用喷漆!"); p.EmitLocked("Inv:CanUseYes"); return; }

                            case 37:
                                if (!DateTime.TryParse(i.itemData2, out DateTime case37createDate)) { MainChat.SendErrorChat(p, "[!] 发生了错误."); p.EmitLocked("Inv:CanUseYes"); return; }
                                if (case37createDate.AddDays(3) < DateTime.Now)
                                {
                                    DiseaseModel dis = p.injured.diseases.Find(x => x.DiseaseName == "腹泻");
                                    if (dis == null)
                                    {
                                        dis = new DiseaseModel() { DiseaseName = "腹泻", DiseaseValue = 3 };
                                        p.injured.diseases.Add(dis);
                                    }
                                    else
                                    {
                                        dis.DiseaseValue += 1;
                                    }

                                    await p.updateSql();
                                    MainChat.SendInfoChat(p, "[?] 您的角色感觉到肚子不舒服, 可能因为食物变质了.");
                                }
                                await RemoveInventoryItem(p, itemId, usageAmount);
                                if (i.itemData == "1")
                                {
                                    if (p.tempStrength + 7 <= 21)
                                    {
                                        p.tempStrength += 7;
                                        MainChat.SendInfoChat(p, "[!] 您的角色感觉到身体更加强壮了(临时效果).");
                                    }
                                    p.MaxHealth = 1000;
                                    p.Health = 1000;
                                    MainChat.SendInfoChat(p, "[!] 您的角色食用了食物并感觉更加健康了!<br>临时强壮度: [" + p.tempStrength + "]");
                                }
                                else
                                {
                                    if (p.tempStrength + 2 <= 20)
                                    {
                                        p.tempStrength += 1;
                                        MainChat.SendInfoChat(p, "[!] 您的角色感觉到身体更加强壮了(临时效果).");
                                    }
                                    p.MaxHealth = 1000;
                                    if (p.Health <= 700) p.Health = 750;
                                    MainChat.SendInfoChat(p, "[!] 您的角色食用了食物并感觉更加健康了!<br>临时强壮度: [" + p.tempStrength + "]");
                                }
                                return;

                            case 40:
                                if (Vehicle.VehicleMain.CleanVehicle(p))
                                {
                                    await RemoveInventoryItem(p, itemId, usageAmount);
                                }
                                else p.EmitLocked("Inv:CanUseYes");
                                return;

                            case 41:
                                if (!OtherSystem.LSCsystems.Tent.EVET_TentWant(p))
                                    MainChat.SendErrorChat(p, "[错误] 您只能搭建一座帐篷, 请先撤销原来的帐篷.");
                                p.EmitLocked("Inv:CanUseYes");
                                return;

                            case 44: // LockPick
                                GlobalEvents.InvForceClose(p);
                                if (await Jacking.StartPicking(p))
                                {
                                    await RemoveInventoryItem(p, itemId, usageAmount);
                                }
                                else p.EmitLocked("Inv:CanUseYes");
                                return;

                            case 45:
                                // Open Document
                                GlobalEvents.InvForceClose(p);
                                p.SendChatMessage("{D0D613}" + i.itemData2);
                                p.EmitLocked("Inv:CanUseYes");
                                //p.Emit("Contract:Show", i.itemData2);
                                return;
                            case 46:
                                PlantUtil.TypeList type = default;
                                if (i.itemId == 74) type = PlantUtil.TypeList.Polo;
                                else if (i.itemId == 75) type = PlantUtil.TypeList.Cabbage;
                                else if (i.itemId == 76) type = PlantUtil.TypeList.Pumpkin;
                                else if (i.itemId == 77) type = PlantUtil.TypeList.Tomato;
                                Alt.Log($"{type} - {i.itemId}");
                                if (await Plants.ServerEvent.TryToPlant(p, type)) await RemoveInventoryItem(p, itemId, usageAmount);
                                else p.EmitLocked("Inv:CanUseYes");
                                return;
                        }
                        break;

                    case 2:
                        /* if(p.Position.Distance(farming.farmBuyPos) < 5 && i.itemId == 23)
                         {
                             RemoveInventoryItem(p, itemId, usageAmount);
                             p.cash += 400;
                             p.updateSql();
                             p.SendChatMessage("Mahsülü 400$ karşılığında sattınız.");
                             return;
                         }
                         */
                        // Kıyafet Satış
                        if (ClothingShop.SellClothes(p, i))
                        {
                            await RemoveInventoryItem(p, itemId, usageAmount);
                            return;
                        }

                        // Üzüm Çevirme Noktası
                        if (await General.MakeChamp(p, i))
                            return;

                        // Şarap satma
                        // if (General.SellWine(p, i))
                            // return;

                        // Balıkçılık
                        if (Fishing.SellFish(p, i))
                            return;

                        // Item Vendor
                        if (await WeedVendors.SellWeed(p, i))
                            return;

                        // Kutunun içine koyma.
                        Crate nearCrate = CrateEvents.CheckNearCrate(p);
                        if (nearCrate != null)
                        {
                            if (nearCrate.type == 4)
                            {
                                List<InventoryModel> labEnv = JsonConvert.DeserializeObject<List<InventoryModel>>(nearCrate.value);
                                if (labEnv == null) { labEnv = new List<InventoryModel>(); }
                                //i.itemData2 = DateTime.Now.ToString();
                                labEnv.Add(i);
                                nearCrate.value = JsonConvert.SerializeObject(labEnv);
                                nearCrate.Update();
                                await RemoveInventoryItem(p, itemId, usageAmount);
                                //p.SendChatMessage("kutuya koydun.");
                                return;
                            }
                        }


                        PlayerModel usage2Target = GlobalEvents.GetNearestPlayer(p);
                        if (usage2Target == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }
                        double targetWeight = await InvTotalWeight(usage2Target);
                        if ((targetWeight + i.itemWeight) > 30.0)
                        {
                            MainChat.SendErrorChat(p, CONSTANT.ERR_TargetWeight);
                            return;
                        }
                        item.data = i.itemData;
                        item.data2 = i.itemData2;
                        item.name = i.itemName;
                        bool case2Succes = await AddInventoryItem(usage2Target, item, usageAmount);
                        if (case2Succes == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_TargetSlot); return; }
                        await RemoveInventoryItem(p, itemId, usageAmount);
                        await UpdatePlayerInventory(usage2Target);

                        if (item.ID == 28 || item.ID == 29 || item.ID == 30 || item.ID == 35 || item.type == 27)
                        {
                            MainChat.EmoteDo(p, usage2Target.characterName.Replace("_", " ") + " 获得了 " + item.name);
                        }
                        else MainChat.AME(p, " 将" + item.name + " 给予" + usage2Target.characterName.Replace("_", " "));

                        Core.Logger.WriteLogData(Core.Logger.logTypes.InventoryLog, p.characterName + " -> " + usage2Target.characterName.Replace("_", " ") + " 获得了 " + item.name);

                        MainChat.SendInfoChat(usage2Target, string.Format(CONSTANT.INFO_PlayerGiveYou, p.characterName.Replace("_", " "), item.name));
                        MainChat.SendInfoChat(p, string.Format(CONSTANT.INFO_PlayerGiveSucces, usage2Target.characterName.Replace("_", " "), item.name));
                        return;
                    case 3:
                        if (i.itemId == 1 && i.itemSlot != 0)
                        {
                            await OtherSystem.Phone.PhoneMain.TakeOutPhone(p, i);
                            //Phones.TakeOffPhone(p, i);
                        }
                        if (i.itemId == 28 || i.itemId == 29 || i.itemId == 30)
                            return;

                        if (i.itemSlot != 0)
                            return;

                        if (i.itemId == 49)
                        {
                            if (p.HasData(EntityData.PlayerEntityData.FDDuty))
                            {
                                if (p.Position.Distance(new Position(-1737, -239, 53)) < 50)
                                {
                                    Cemetery.CreateCemetery(p, i.ID);
                                    return;
                                }
                            }
                            cksystem.placeCorpseGround(p, i.itemData2);
                            await RemoveInventoryItem(p, itemId, usageAmount);
                            return;
                        }

                        await RemoveInventoryItem(p, itemId, usageAmount);
                        Position case3ItemPos = p.Position;
                        case3ItemPos.Z -= 1;
                        OtherSystem.Textlabels.LProp case3Prop = PropStreamer.Create(item.objectModel, case3ItemPos, p.Rotation, placeObjectOnGroundProperly: true, frozen: true, dimension: p.Dimension);
                        string labelText = "[物品编号:" + case3Prop.Id.ToString() + "]~n~" + i.itemName + " ~n~拾取指令:~r~/~w~pickup [ID]";
                        PlayerLabel case3Label = TextLabelStreamer.Create(labelText, case3ItemPos, streamRange: 2, font: 0, dimension: p.Dimension);
                        string json = JsonConvert.SerializeObject(i);
                        GroundObj case3Obj = new GroundObj()
                        {
                            ID = (int)case3Prop.Id,
                            Prop = case3Prop,
                            textLabel = case3Label,
                            data = json
                        };
                        groundObjects.Add(case3Obj);
                        Core.Logger.WriteLogData(Core.Logger.logTypes.InventoryLog, p.characterName + " -> 丢弃物品. \n[" + json + "]");
                        return;

                    case 4:
                        if (i.itemSlot > 0)
                            return;
                        bool case4Succes = await MoveItemOtherEnv(p, i);
                        if (case4Succes) { GlobalEvents.notify(p, 2, "成功移动物品"); await RemoveInventoryItem(p, itemId, usageAmount); p.EmitLocked("Inv:CanUseYes"); return; }
                        else { GlobalEvents.notify(p, 3, "无法移动此物品!"); return; }

                    case 5:
                        //Alt.Log("envanter isteği geldi."); 
                        if (!item.isNameChange && p.adminLevel <= 4) { MainChat.SendErrorChat(p, "[错误] 无法更改此物品的名称."); p.EmitLocked("Inv:CanUseYes"); return; }
                        GlobalEvents.InvForceClose(p);
                        //await Task.Delay(500);

                        if (!p.Exists)
                            return;

                        NativeUi.Inputs.SendTypeInput(p, "为此物品设置新的名称吧!", "Inventory:UpdateItemName", i.ID.ToString());
                        p.EmitLocked("Inv:CanUseYes");
                        break;

                    default:
                        return;
                }
            }
            catch
            {

            }

            return;
        }

        public static async void ClearGroundItems()
        {
            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                GlobalEvents.SubTitle(t, "所有掉落在地上的物品将在 10 秒后清空.", 5);
            }
            await Task.Delay(10000);
            foreach (GroundObj to in groundObjects)
            {
                if (to.Prop.Dimension == 0)
                {
                    to.textLabel.Delete();
                    to.Prop.Delete();
                    groundObjects.Remove(to);
                }
            }
        }


        [Command("pickup")]
        public async Task COM_TakeItemOnGround(PlayerModel p, params string[] args)
        {
            if (p.Ping > 250)
                return;
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /pickup [物品ID]"); return; }
            Int32.TryParse(args[0], out int ID);
            GroundObj t = groundObjects.Find(x => x.ID == ID);
            if (t == null) { MainChat.SendErrorChat(p, "[错误] 附近没有物品."); return; }
            if (p.Position.Distance(t.Prop.Position) > 2) { MainChat.SendErrorChat(p, "[错误] 您离物品太远."); return; }
            InventoryModel tItem = JsonConvert.DeserializeObject<InventoryModel>(t.data);
            ServerItems newItem = Items.LSCitems.Find(x => x.ID == tItem.itemId);
            newItem.data = tItem.itemData;
            newItem.data2 = tItem.itemData2;
            newItem.amount = 1;
            newItem.picture = tItem.itemPicture;
            newItem.ID = tItem.itemId;
            newItem.name = tItem.itemName;
            if ((await InvTotalWeight(p) + newItem.weight) > p.Strength) { GlobalEvents.notify(p, 3, "您的库存超重了."); return; }
            bool succes = await AddInventoryItem(p, newItem, 1);
            if (!succes) { MainChat.SendErrorChat(p, "[错误] 您的库存满了."); return; }
            t.Prop.Delete();
            t.textLabel.Delete();
            Animations.PlayerAnimation(p, new string[] { "pickup" });
            groundObjects.Remove(t);
            await Task.Delay(1000);
            if (!p.Exists)
                return;
            Animations.PlayerStopAnimation(p);
            GlobalEvents.notify(p, 2, "成功拾取物品至库存");
            Core.Logger.WriteLogData(Core.Logger.logTypes.InventoryLog, p.characterName + " -> 拾取物品. \n" + "[" + t.data + "]");
            await UpdatePlayerInventory(p);//
            return;
        }

        public static async Task<bool> MoveItemOtherEnv(PlayerModel p, InventoryModel i)
        {
            //MainChat.SendInfoChat(p, "! Sistem bir süreliğine pasif haldedir.");
            //return false;
            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v != null)
            {
                if (v.settings.TrunkLock) { MainChat.SendErrorChat(p, "[错误] 车辆后备箱是锁的!"); return false; }
                List<ServerItems> items = JsonConvert.DeserializeObject<List<ServerItems>>(v.vehInv);
                double totalWeight = 0;
                if (items != null)
                {
                    foreach (ServerItems x in items)
                    {
                        totalWeight += (x.weight * x.amount);
                    }
                }
                else
                {
                    items = new List<ServerItems>();
                }

                if (items.Count + i.itemAmount > 60) { return false; }
                else if ((totalWeight + (i.itemWeight / i.itemAmount)) > (double)v.inventoryCapacity) { return false; } // Öenmli Eski Hali |  else if ((int)(totalWeight + (i.itemWeight)) > v.inventoryCapacity) { return false; }
                else
                {
                    ServerItems sItem = Items.LSCitems.Find(x => x.ID == i.itemId);
                    sItem.selectID = i.ID;
                    sItem.data = i.itemData;
                    sItem.data2 = i.itemData2;
                    sItem.name = i.itemName;
                    if (sItem.stackable)
                        sItem.amount = 0;
                    else
                        sItem.amount = 1;

                    if (sItem.stackable)
                    {
                        ServerItems t = items.Find(x => x.ID == i.itemId);
                        if (t != null)
                        {
                            t.amount += 1;
                            v.vehInv = JsonConvert.SerializeObject(items);
                            v.Update();
                            p.EmitLocked("otherEnv:Reload", 1, v.vehInv);
                            if (sItem.ID == 28 || sItem.ID == 29 || sItem.ID == 30 || sItem.ID == 35 || sItem.type == 27)
                                MainChat.EmoteDo(p, i.itemName + "被存入后备箱.");
                            else MainChat.ADO(p, i.itemName + "被存入后备箱.");

                            Core.Logger.WriteLogData(Core.Logger.logTypes.InventoryLog, p.characterName + " -> " + v.sqlID + " 存入物品 " + t.name);
                            return true;
                        }
                        else
                        {
                            items.Add(sItem);
                            v.vehInv = JsonConvert.SerializeObject(items);
                            v.Update();
                            p.EmitLocked("otherEnv:Reload", 1, v.vehInv);
                            if (sItem.ID == 28 || sItem.ID == 29 || sItem.ID == 30 || sItem.ID == 35 || sItem.type == 27)
                                MainChat.EmoteDo(p, i.itemName + "被存入后备箱.");
                            else MainChat.ADO(p, i.itemName + "被存入后备箱.");

                            Core.Logger.WriteLogData(Core.Logger.logTypes.InventoryLog, p.characterName + " -> " + v.sqlID + " 存入物品 " + t.name);
                            return true;
                        }
                    }
                    else
                    {
                        items.Add(sItem);
                        v.vehInv = JsonConvert.SerializeObject(items);
                        v.Update();
                        p.EmitLocked("otherEnv:Reload", 1, v.vehInv);
                        if (sItem.ID == 28 || sItem.ID == 29 || sItem.ID == 30 || sItem.ID == 35 || sItem.type == 27)
                            MainChat.EmoteDo(p, i.itemName + "被存入后备箱.");
                        else MainChat.ADO(p, i.itemName + "被存入后备箱.");

                        Core.Logger.WriteLogData(Core.Logger.logTypes.InventoryLog, p.characterName + " -> " + v.sqlID + " 存入物品 " + sItem.name);

                        return true;
                    }
                }
            }

            BagModel bag = BagEvents.serverBags.Find(x => p.Position.Distance(x.label.Position) <= 3);
            if (bag != null)
            {
                List<ServerItems> bitems = JsonConvert.DeserializeObject<List<ServerItems>>(bag.Env);
                double btotalWeight = 0;
                if (bitems != null)
                {
                    foreach (ServerItems x in bitems)
                    {
                        btotalWeight += x.weight;
                    }
                }
                else
                {
                    bitems = new List<ServerItems>();
                }

                if (btotalWeight > bag.weight) { return false; }
                else
                {
                    ServerItems bsItem = Items.LSCitems.Find(x => x.ID == i.itemId);
                    bsItem.selectID = i.ID;
                    bsItem.data = i.itemData;
                    bsItem.data2 = i.itemData2;
                    bsItem.name = i.itemName;
                    bsItem.amount = 1;
                    if (bsItem.stackable)
                    {
                        ServerItems t = bitems.Find(x => x.ID == i.itemId);
                        if (t != null)
                        {
                            //Alt.Log("5");
                            t.amount += 1;
                            bag.Env = JsonConvert.SerializeObject(bitems);
                            bag.Update();
                            p.EmitLocked("otherEnv:Reload", 4, bag.Env);
                            return true;
                        }
                        else
                        {
                            bitems.Add(bsItem);
                            bag.Env = JsonConvert.SerializeObject(bitems);
                            bag.Update();
                            p.EmitLocked("otherEnv:Reload", 4, bag.Env);
                            return true;
                        }
                    }
                    else
                    {
                        bitems.Add(bsItem);
                        bag.Env = JsonConvert.SerializeObject(bitems);
                        bag.Update();
                        p.EmitLocked("otherEnv:Reload", 4, bag.Env);
                        return true;
                    }
                }
            }

            PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Find(x => p.Position.Distance(x.Position) < 3f && x.Dimension == p.Dimension);
            if (entranceLabel == null) { MainChat.SendErrorChat(p, "[错误] 附近没有车辆/房屋/产业!"); return false; }
            entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position bussinesPos);

            (HouseModel, PlayerLabel, Marker) tH = await Props.Houses.getHouseFromPos(bussinesPos);

            if (tH.Item1 != null)
            {
                if (await Props.Houses.HouseKeysQuery(p, tH.Item1))
                {
                    List<ServerItems> Hitems = JsonConvert.DeserializeObject<List<ServerItems>>(tH.Item1.houseEnv);
                    double btotalWeight = 0;
                    if (Hitems != null)
                    {
                        foreach (ServerItems x in Hitems)
                        {
                            btotalWeight += x.weight;
                        }
                    }
                    else
                    {
                        Hitems = new List<ServerItems>();
                    }

                    if (btotalWeight + i.itemWeight > 450.0) { return false; }
                    else
                    {
                        ServerItems bsItem = Items.LSCitems.Find(x => x.ID == i.itemId);
                        bsItem.selectID = i.ID;
                        bsItem.data = i.itemData;
                        bsItem.data2 = i.itemData2;
                        bsItem.name = i.itemName;
                        bsItem.amount = 1;
                        if (bsItem.stackable)
                        {
                            ServerItems t = Hitems.Find(x => x.ID == i.itemId);
                            if (t != null)
                            {
                                t.amount += 1;
                                tH.Item1.houseEnv = JsonConvert.SerializeObject(Hitems);
                                tH.Item1.Update(tH.Item3, tH.Item2);
                                p.EmitLocked("otherEnv:Reload", 3, tH.Item1.houseEnv);

                                if (bsItem.ID == 28 || bsItem.ID == 29 || bsItem.ID == 30 || bsItem.ID == 35 || bsItem.type == 27)
                                    MainChat.EmoteDo(p, i.itemName + "被存入房屋.");
                                else MainChat.ADO(p, i.itemName + "被存入房屋.");

                                Core.Logger.WriteLogData(Core.Logger.logTypes.InventoryLog, p.characterName + " -> " + tH.Item1.ID + " 存入物品 " + bsItem.name);
                                return true;
                            }
                            else
                            {
                                Hitems.Add(bsItem);
                                tH.Item1.houseEnv = JsonConvert.SerializeObject(Hitems);
                                tH.Item1.Update(tH.Item3, tH.Item2);
                                p.EmitLocked("otherEnv:Reload", 3, tH.Item1.houseEnv);
                                if (bsItem.ID == 28 || bsItem.ID == 29 || bsItem.ID == 30 || bsItem.ID == 35 || bsItem.type == 27)
                                    MainChat.EmoteDo(p, i.itemName + "被存入房屋.");
                                else MainChat.ADO(p, i.itemName + "被存入房屋.");

                                Core.Logger.WriteLogData(Core.Logger.logTypes.InventoryLog, p.characterName + " -> " + tH.Item1.ID + " 存入物品 " + bsItem.name);

                                return true;
                            }
                        }
                        else
                        {
                            Hitems.Add(bsItem);
                            tH.Item1.houseEnv = JsonConvert.SerializeObject(Hitems);
                            tH.Item1.Update(tH.Item3, tH.Item2);
                            p.EmitLocked("otherEnv:Reload", 3, tH.Item1.houseEnv);
                            if (bsItem.ID == 28 || bsItem.ID == 29 || bsItem.ID == 30 || bsItem.ID == 35 || bsItem.type == 27)
                                MainChat.EmoteDo(p, i.itemName + "被存入房屋.");
                            else MainChat.ADO(p, i.itemName + "被存入房屋.");

                            Core.Logger.WriteLogData(Core.Logger.logTypes.InventoryLog, p.characterName + " -> " + tH.Item1.ID + " 存入物品 " + bsItem.name);
                            return true;
                        }
                    }
                }
            }


            // İşyeri

            var tB = await Props.Business.getBusinessFromPos(bussinesPos);
            if (tB.Item1 != null)
            {
                if (await Props.Business.CheckBusinessKey(p, tB.Item1))
                {
                    List<ServerItems> Hitems = JsonConvert.DeserializeObject<List<ServerItems>>(tB.Item1.settings.Env);
                    double btotalWeight = 0;
                    if (Hitems != null)
                    {
                        foreach (ServerItems x in Hitems)
                        {
                            btotalWeight += x.weight;
                        }
                    }
                    else
                    {
                        Hitems = new List<ServerItems>();
                    }

                    if (btotalWeight + i.itemWeight > 450.0) { return false; }
                    else
                    {
                        ServerItems bsItem = Items.LSCitems.Find(x => x.ID == i.itemId);
                        bsItem.selectID = i.ID;
                        bsItem.data = i.itemData;
                        bsItem.data2 = i.itemData2;
                        bsItem.name = i.itemName;
                        bsItem.amount = 1;
                        if (bsItem.stackable)
                        {
                            ServerItems t = Hitems.Find(x => x.ID == i.itemId);
                            if (t != null)
                            {
                                t.amount += 1;
                                tB.Item1.settings.Env = JsonConvert.SerializeObject(Hitems);
                                await tB.Item1.Update(tB.Item2, tB.Item3);
                                p.EmitLocked("otherEnv:Reload", 2, tB.Item1.settings.Env);

                                if (bsItem.ID == 28 || bsItem.ID == 29 || bsItem.ID == 30 || bsItem.ID == 35 || bsItem.type == 27)
                                    MainChat.EmoteDo(p, i.itemName + "被存入产业.");
                                else MainChat.ADO(p, i.itemName + "被存入产业.");

                                Core.Logger.WriteLogData(Core.Logger.logTypes.InventoryLog, p.characterName + " -> " + tB.Item1.ID + " 存入物品 " + bsItem.name);
                                return true;
                            }
                            else
                            {
                                Hitems.Add(bsItem);
                                tB.Item1.settings.Env = JsonConvert.SerializeObject(Hitems);
                                await tB.Item1.Update(tB.Item2, tB.Item3);
                                p.EmitLocked("otherEnv:Reload", 2, tB.Item1.settings.Env);
                                if (bsItem.ID == 28 || bsItem.ID == 29 || bsItem.ID == 30 || bsItem.ID == 35 || bsItem.type == 27)
                                    MainChat.EmoteDo(p, i.itemName + "被存入产业.");
                                else MainChat.ADO(p, i.itemName + "被存入产业.");

                                Core.Logger.WriteLogData(Core.Logger.logTypes.InventoryLog, p.characterName + " -> " + tB.Item1.ID + " 存入物品 " + bsItem.name);

                                return true;
                            }
                        }
                        else
                        {
                            Hitems.Add(bsItem);
                            tB.Item1.settings.Env = JsonConvert.SerializeObject(Hitems);
                            await tB.Item1.Update(tB.Item2, tB.Item3);
                            p.EmitLocked("otherEnv:Reload", 2, tB.Item1.settings.Env);
                            if (bsItem.ID == 28 || bsItem.ID == 29 || bsItem.ID == 30 || bsItem.ID == 35 || bsItem.type == 27)
                                MainChat.EmoteDo(p, i.itemName + "被存入产业.");
                            else MainChat.ADO(p, i.itemName + "被存入产业.");

                            Core.Logger.WriteLogData(Core.Logger.logTypes.InventoryLog, p.characterName + " -> " + tB.Item1.ID + " 存入物品 " + bsItem.name);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        [AsyncClientEvent("Inventory:UpdateItemName")]
        public static async Task<bool> ChangeItemName(PlayerModel p, string newName, string ii)
        {
            if (p.Ping > 250)
                return false;
            int itemId = Int32.Parse(ii);
            InventoryModel item = await Database.DatabaseMain.FindInventoryItem(itemId);
            if (item == null)
                return false;

            item.itemName = newName;
            await item.Update();
            await UpdatePlayerInventory(p);
            return true;
        }         //
    }
}
