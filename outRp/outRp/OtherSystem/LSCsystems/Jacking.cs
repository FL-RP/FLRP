using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Core;
using outRp.Globals;
using outRp.Models;
using outRp.OtherSystem.Textlabels;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;
using static outRp.OtherSystem.Inventory;

namespace outRp.OtherSystem.LSCsystems
{
    public class Jacking : IScript
    {
        /*  TODO
         *  Yarım saatte bir lock kırılacak.
         *  Her lock kırıldığında bir seviye düşecek.
         *  lock belli bir seviyenin altındayken evlerin envanterlerine, işyerlerinin kasalarına araçların bagaj + anahtarlarına erişilebilecek.
         *  araçlar için mini bir imha sistemi (tekerleri vs. sil. Hurdaya çevirip bırak oyuncuya item veya para ver.)
         */

        public static int defLockPick = 10;
        public static DateTime lastJack = DateTime.Now;

        [Command("maxlockpick")]
        public static void COM_SetDefaul(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 3) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            if (!Int32.TryParse(args[0], out int total)) { MainChat.SendInfoChat(p, "[用法] /maxlockpick [数值] 使用后重置每小时值."); return; }

            defLockPick = total;
            CanUseLockPick = total;
            MainChat.SendInfoChat(p, "[!] 每小时开锁器使用限制更新为: " + total.ToString());
            return;
        }

        public static int CanUseLockPick = 10;
        public static async Task<bool> StartPicking(PlayerModel p)
        {
            if (p.Ping > 250)
                return false;


            GlobalEvents.InvForceClose(p);
            if (CanUseLockPick <= 0) { MainChat.SendErrorChat(p, "[错误] 已达到每小时开锁器使用限制, 剩余冷却时间: " + (60 - ServerEvent.serverHourTimer).ToString() + "分钟"); return false; }
            bool isPicked = false;
            if (TotalPDGroup() < 4)
            {
                MainChat.SendErrorChat(p, "[!] 至少需要有 4 名执法成员在线才可以使用开锁器.");
                GlobalEvents.FreezeEntity(p);
                return false;
            }

            Animations.PlayerAnimation(p, new string[] { "mechanic4" });
            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v != null)
            {
                if (v.LockState != AltV.Net.Enums.VehicleLockState.Locked)
                {
                    MainChat.SendInfoChat(p, "[?] 这辆车是开的!");
                    return false;
                }

                GlobalEvents.FreezeEntity(p);
                // TODO neler gideceği ve nasıl gideceğini düzenle, içine securitylevel'i de ekle.
                p.EmitAsync("Lockpick:Start", 0, v.sqlID);
                return true;
            }

            var house = await Props.Houses.getNearHouse(p);
            if (house.Item1 != null)
            {
                if(lastJack.AddMinutes(5) > DateTime.Now)
                {
                    MainChat.SendErrorChat(p, "[!] 此房屋 5 分钟前才被开过锁, 请稍后再试.");
                    return false;
                }

                if (house.Item1.isLocked == false) { MainChat.SendInfoChat(p, "[?] 此房屋是开的!"); GlobalEvents.FreezeEntity(p); return false; }

                // TODO neler gideceği ve nasıl gideceğini düzenle, içine securitylevel'i de ekle.
                p.EmitAsync("Lockpick:Start", 1, house.Item1.ID);
                return true;
            }


            var biz = await Props.Business.getNearestBusiness(p);
            if (biz.Item1 != null)
            {
                if(lastJack.AddMinutes(5) > DateTime.Now)
                {
                    MainChat.SendErrorChat(p, "[!] 此产业 5 分钟前才被开过锁, 请稍后再试.");
                    return false;
                }
                if (biz.Item1.isLocked == false) { MainChat.SendInfoChat(p, "[?] 此产业是开的!"); GlobalEvents.FreezeEntity(p); return false; }

                // TODO neler gideceği ve nasıl gideceğini düzenle, içine securitylevel'i de ekle.
                p.EmitAsync("Lockpick:Start", 2, biz.Item1.ID);
                return true;
            }

            return isPicked;
        }

        [AsyncClientEvent("Lockpick:Result")]
        public static async Task EVENT_LockPickResult(PlayerModel p, bool success, int type, int ID)
        {
            Random rnd = new Random();
            if (!success)
            {
                MainChat.SendInfoChat(p, "[?] 开锁失败!");
                if (rnd.Next(0, 10) > 7)
                {
                    MainChat.SendInfoChat(p, "[!] 有人看见你的行为并报警了...");
                    AddMDCCall(p, "[盗窃未遂]");
                }
                return;
            }

            CanUseLockPick--;
            if (CanUseLockPick <= 0)
                CanUseLockPick = 0;

            switch (type)
            {
                case 0: // Araç
                    GlobalEvents.FreezeEntity(p);
                    if (p.Position.Distance(new Position(-181, -1145, 23)) > 2670) { MainChat.SendErrorChat(p, "[错误] 您必须在城市内!"); return; }
                    VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(ID);
                    if (v == null) { MainChat.SendErrorChat(p, "[错误] 无效车辆."); return; }
                    if (v.Position.Distance(p.Position) > 5) { MainChat.SendErrorChat(p, "[错误] 您离车太远."); return; }

                    GlobalEvents.SetVehicleTag(v, "~p~有被撬过锁的迹象.", true);
                    v.LockState = AltV.Net.Enums.VehicleLockState.Unlocked;
                    MainChat.SendInfoChat(p, "[?] 车门解锁成功, 您现在可以进入车内了! 要发动车辆, 您必须进入第二阶段解锁.<br>/hotwire 发动车辆, 请记住, 只有一次机会!", true);
                    MainChat.SendAdminChat("{#B2CB87}[信息] " + p.characterName + " 撬开了车门(第一阶段).");
                    v.SetData("Jacker", p.sqlID);
                    return;

                case 1: // Ev
                    lastJack = DateTime.Now;
                    var house = await Props.Houses.getHouseById(ID);
                    if (house.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效房屋."); return; }
                    if (house.Item1.pos.Distance(p.Position) > 5) { MainChat.SendErrorChat(p, "[错误] 您离房门太远."); return; }

                    house.Item1.isLocked = false;
                    house.Item1.Update(house.Item3, house.Item2);
                    p.SetData("Jacker:CanHouse", house.Item1.ID);
                    MainChat.SendInfoChat(p, "[?] 第一步顺利成功, 要偷东西, 您必须进入第二阶段解锁!", true);
                    MainChat.SendAdminChat("{#B2CB87}[信息] " + p.characterName + " 撬开了房门(第一阶段).");
                    return;

                case 2: // İşyeri
                    lastJack = DateTime.Now;
                    var biz = await Props.Business.getBusinessById(ID);
                    if (biz.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效产业."); return; }
                    if (biz.Item1.position.Distance(p.Position) > 5) { MainChat.SendErrorChat(p, "[错误] 您离产业门太远."); return; }

                    biz.Item1.isLocked = false;
                    await biz.Item1.Update(biz.Item2, biz.Item3);
                    MainChat.SendInfoChat(p, "[?] 第一步顺利成功, 要偷东西, 您必须进入第二阶段解锁!", true);
                    MainChat.SendAdminChat("{#B2CB87}[信息] " + p.characterName + " 撬开了产业门(第一阶段).");
                    return;

                default:
                    MainChat.SendInfoChat(p, "[?] 发生了错误.");
                    return;
            }
        }

        [Command("trunkhack")]
        public static void COM_TrunkHack(PlayerModel p)
        {
            MainChat.SendInfoChat(p, "[?] 系统已经禁用, 请使用/hotwire - 解锁引擎.");
            return;
            if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] Bu komutu kullanabilmek için aracın içinde olmalısınız!"); return; }
            VehModel v = (VehModel)p.Vehicle;
            if (!v.HasData("Jacker")) { MainChat.SendErrorChat(p, "[错误] Bu araç için bu komutu kullanamazsınız!"); return; }

            NativeUi.Inputs.SendButtonInput(p, "İşlem " + (v.settings.SecurityLevel * 20) + " Saniye sürecek ve polislere ihbar gidecek! Araçtan inmemeniz gerekiyor.", "Jacker:Vehicle");
            return;
        }

        [Command("hotwire")]
        public static void COM_EngineHack(PlayerModel p)
        {
            if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] 您必须在车内!"); return; }
            VehModel v = (VehModel)p.Vehicle;
            if (!v.HasData("Jacker")) { MainChat.SendErrorChat(p, "[错误] 此车没有被撬开(不是您第一阶段撬开的车)!"); return; }

            NativeUi.Inputs.SendButtonInput(p, "此过程将持续 " + (v.price / 2000) + " 秒, 并且报警了! 期间不能下车, 下车算作失败.", "Jacker:VehicleEngine");
            return;
        }

        [AsyncClientEvent("Jacker:VehicleEngine")]
        public async void EVENT_JackVehicleEngine(PlayerModel p, bool state)
        {
            if (!state) { MainChat.SendErrorChat(p, "[?] 您放弃了发动车辆."); return; }
            if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] 您必须在车内才能完成该过程!"); return; }

            if (p.HasData("Jacker:isInJack")) { MainChat.SendErrorChat(p, "[错误] 您已经在尝试发动车辆了!"); return; }

            Random porand = new Random();
            if (porand.Next(0, 10) > 7)
            {
                MainChat.SendInfoChat(p, "[!] 有人看见你的行为并报警了...");
                AddMDCCall(p, "盗窃车辆");
            };

            VehModel v = (VehModel)p.Vehicle;
            GlobalEvents.ProgresBar(p, "发动车辆中...", (v.price / 2000));
            p.SetData("Jacker:isInJack", true);
            await Task.Delay((v.price / 2));
            if (!p.Exists)
                return;

            if (p.Vehicle == null)
                return;

            if (!p.Vehicle.HasData("Jacker"))
                return;

            p.Vehicle.DeleteData("Jacker");


            if (p.HasData("Jacker:isInJack"))
                p.DeleteData("Jacker:isInJack");

            v.settings.SecurityLevel -= 1;
            if (v.settings.SecurityLevel <= 0)
                v.settings.SecurityLevel = 1;


            Random rnd = new Random();
            int cur = rnd.Next(1, 20);
            if (cur < (15 - v.settings.SecurityLevel))
            {
                p.Vehicle.SetEngineOnAsync(true);
                MainChat.SendInfoChat(p, "[?] 成功发动车辆! (可以前往车辆回收区报废车辆获得金钱)");
                p.Vehicle.SetData("Jacker:CanSell", p.sqlID);
            }
            else { MainChat.SendInfoChat(p, "[=] 尝试发动车辆失败!", true); return; }
            return;
        }

        [AsyncClientEvent("Jacker:Vehicle")]
        public async void EVENT_JackVehicle(PlayerModel p, bool state)
        {
            if (!state) { MainChat.SendErrorChat(p, "[?] 您放弃了."); return; }
            if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] İşlemin tamamlanabilmesi için araçta olmalısınız!"); return; }

            AddMDCCall(p, "Araç Hırsızlığı");
            MainChat.SendInfoChat(p, "[?] Polis ekiplerine ihbar gitti.");

            VehModel v = (VehModel)p.Vehicle;
            GlobalEvents.ProgresBar(p, "Hırsızlık...", v.settings.SecurityLevel * 20);
            await Task.Delay((v.settings.SecurityLevel * 20) * 1000);
            if (!p.Exists)
                return;

            if (p.Vehicle == null)
                return;

            if (!p.Vehicle.HasData("Jacker"))
                return;

            p.Vehicle.DeleteData("Jacker");

            Random rnd = new Random();
            int cur = rnd.Next(1, 20);
            /*if(cur < 10) // Todo 7' yapılacak.
            {
                CreateJackItem(p.Vehicle.Position, rnd.Next(1, 10), dimension: p.Dimension);
            }*/
            v.settings.SecurityLevel -= 1;
            if (v.settings.SecurityLevel <= 0)
                v.settings.SecurityLevel = 1;

            if (cur < 3)
            {
                VehModel veh = (VehModel)p.Vehicle;
                if (veh != null)
                {
                    veh.settings.TrunkLock = false;
                    MainChat.SendInfoChat(p, "[?] Araç bagajı açıldı!");
                }

            }
            if (cur >= 7) { MainChat.SendInfoChat(p, "[=] Hırsızlık girişimi başarısız oldu!", true); return; }
            return;
        }

        [Command("househack")]
        public async Task EVENT_JackHouse(PlayerModel p)
        {
            if (CanUseLockPick <= 0) { MainChat.SendErrorChat(p, "[错误] 已达到每小时开锁器使用限制, 剩余冷却时间: " + (60 - ServerEvent.serverHourTimer).ToString() + "分钟"); return; }

            PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Find(x => p.Position.Distance(x.Position) < 15f && x.Dimension == p.Dimension);
            if (entranceLabel == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_NotNearHouse); return; }
            entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position bussinesPos);

            var t = await Props.Houses.getHouseFromPos(bussinesPos);

            if (t.Item1 == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_NotNearHouse); return; }

            if (await Props.Houses.HouseKeysQuery(p, t.Item1)) { MainChat.SendErrorChat(p, "[错误] 这是您的房屋!"); return; }
            if (!p.HasData("Jacker:CanHouse")) { MainChat.SendErrorChat(p, "[错误] 现在无法抢劫这个房屋!"); return; }
            if (p.lscGetdata<int>("Jacker:CanHouse") != t.Item1.ID) { MainChat.SendErrorChat(p, "[错误] 现在无法抢劫这个房屋!"); return; }

            Animations.PlayerAnimation(p, new string[] { "mechanic3" });
            GlobalEvents.ProgresBar(p, "搜寻房屋中...", 120);
            MainChat.SendInfoChat(p, "[?] 有人看到了你的行为并报警了...");
            await Task.Delay(70000);
            AddMDCCall(bussinesPos, "入室盗窃");
            await Task.Delay(50000);


            if (!p.Exists)
                return;

            Animations.PlayerStopAnimation(p);
            if (p.Position.Distance(entranceLabel.Position) > 30) { MainChat.SendInfoChat(p, "[!] 解锁取消, 因为您离开了该区域."); return; }

            t.Item1.settings.SecurityLevel -= 1;
            if (t.Item1.settings.SecurityLevel <= 0)
                t.Item1.settings.SecurityLevel = 1;

            t.Item1.Update(t.Item3, t.Item2);

            Random rnd = new Random();
            CreateJackItem(p.Position, rnd.Next(5, 15), 2, dimension: p.Dimension, housePrice: t.Item1.price);
            if (p.HasData("Jacker:CanHouse"))
                p.DeleteData("Jacker:CanHouse");


            MainChat.SendInfoChat(p, "[?] 已完成盗窃.");
            CanUseLockPick -= 1;
            if (CanUseLockPick <= 0)
                CanUseLockPick = 0;

            return;

        }

        [Command("tashcar")]
        public async Task EVENT_TashVehicle(PlayerModel p)
        {
            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v == null) { MainChat.SendErrorChat(p, "[错误] 无效车辆!"); return; }
            if (v.Position.Distance(new Position(-554, -1697, 19)) > 5) { MainChat.SendErrorChat(p, "[错误] 您不在车辆回收区."); return; }
            if (!v.HasData("Jacker:CanSell")) { MainChat.SendErrorChat(p, "[错误] 您无法出售此车!"); return; }
            if (await Vehicle.VehicleMain.GetKeysQuery(p, v)) { MainChat.SendErrorChat(p, "[错误] 您无法出售此车(此车属于您)!"); return; }

            if (v.lscGetdata<int>("Jacker:CanSell") != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 您无法出售此车!"); return; }

            v.DeleteData("Jacker:CanSell");

            p.cash += (((v.price / 100) * v.settings.SecurityLevel) + 3000);
            v.SetDamageDataAsync("//n/2EP+Ag==");
            v.SetWheelDetached(0, true);
            v.SetWheelDetached(1, true);
            v.SetWheelDetached(2, true);
            v.SetWheelDetached(3, true);

            p.updateSql();
            MainChat.SendInfoChat(p, "[?] 已报废车辆并获得: $" + (((v.price / 100) * v.settings.SecurityLevel) + 3000));
            return;
        }

        public static OtherSystem.LSCsystems.MDCEvents.MDCCalls AddMDCCall(PlayerModel p, string reason)
        {
            var check = OtherSystem.LSCsystems.MDCEvents.PDcalls.Find(x => x.pos.Distance(p.Position) < 30);
            if (check == null)
            {
                check = new OtherSystem.LSCsystems.MDCEvents.MDCCalls()
                {
                    callerName = "市民",
                    callNumber = 911,
                    pos = p.Position,
                    reason = reason + " " + DateTime.Now.ToShortTimeString() + " " + JsonConvert.SerializeObject(p.Position),
                };
                OtherSystem.LSCsystems.MDCEvents.PDcalls.Add(check);

                foreach (PlayerModel pd in Alt.GetAllPlayers())
                {
                    if (pd.HasData(EntityData.PlayerEntityData.PDDuty)) { pd.SendChatMessage("{49A0CD}[911] 收到了新的报警."); }
                }
            }

            return check;
        }
        public static OtherSystem.LSCsystems.MDCEvents.MDCCalls AddMDCCall(Position pos, string reason)
        {
            var check = OtherSystem.LSCsystems.MDCEvents.PDcalls.Find(x => x.pos.Distance(pos) < 30);
            if (check == null)
            {
                check = new OtherSystem.LSCsystems.MDCEvents.MDCCalls()
                {
                    callerName = "市民",
                    callNumber = 911,
                    pos = pos,
                    reason = reason + " " + JsonConvert.SerializeObject(pos),
                };
                OtherSystem.LSCsystems.MDCEvents.PDcalls.Add(check);

                foreach (PlayerModel pd in Alt.GetAllPlayers())
                {
                    if (pd.HasData(EntityData.PlayerEntityData.PDDuty)) { pd.SendChatMessage("{49A0CD}[911] 收到了新的报警."); }
                }
            }

            return check;
        }

        public static void CreateJackItem(Position pos, int count, int radius = 5, int dimension = 0, int housePrice = 10000)
        {
            List<Vector3> nps = Core.Core.SetRandomPositionsWithRadius(pos, count, radius, 0.2f);

            if (nps == null)
                return;

            foreach (Vector3 mPos in nps)
            {
                string json1 = JsonConvert.SerializeObject(mPos);
                Position case3ItemPos = mPos;
                case3ItemPos.Z = pos.Z;
                LProp case3Prop = PropStreamer.Create("hei_prop_hei_paper_bag", case3ItemPos, new Rotation(0, 0, 0), placeObjectOnGroundProperly: true, frozen: true, dimension: dimension);
                string labelText = "[可盗窃物品: " + case3Prop.Id.ToString() + "]~n~拾取物品指令: ~r~/~w~pickup [ID]";
                PlayerLabel case3Label = TextLabelStreamer.Create(labelText, case3ItemPos, streamRange: 2, font: 0, dimension: dimension);

                InventoryModel i = GetRandomStealItem(housePrice);

                string json = JsonConvert.SerializeObject(i);
                GroundObj case3Obj = new GroundObj()
                {
                    ID = (int)case3Prop.Id,
                    Prop = case3Prop,
                    textLabel = case3Label,
                    data = json
                };
                groundObjects.Add(case3Obj);
            }
        }

        public static InventoryModel GetRandomStealItem(int housePrice = 10000)
        {
            InventoryModel i = new InventoryModel();
            i.itemData = "";
            i.itemData2 = "";
            Random rnd = new Random();
            switch (rnd.Next(0, 17))
            {
                case 1:
                    i.itemId = 57;
                    i.itemName = "烟盒";
                    i.itemPicture = "57";
                    break;

                case 2:
                    i.itemId = 58;
                    i.itemName = "包装食品";
                    i.itemPicture = "58";
                    break;

                case 3:
                    i.itemId = 59;
                    i.itemName = "易拉罐";
                    i.itemPicture = "59";
                    break;

                case 4:
                    i.itemId = 60;
                    i.itemName = "电视";
                    i.itemPicture = "60";
                    break;

                case 5:
                    i.itemId = 61;
                    i.itemName = "电脑";
                    i.itemPicture = "61";
                    break;

                case 6:
                    i.itemId = 62;
                    i.itemName = "家具";
                    i.itemPicture = "56";
                    break;

                case 7:
                    i.itemId = 65;
                    i.itemName = "宝石";
                    i.itemPicture = "65";
                    break;

                case 8:
                    i.itemId = 66;
                    i.itemName = "桌子";
                    i.itemPicture = "66";
                    break;

                case 9:
                    i.itemId = 67;
                    i.itemName = "邮票";
                    i.itemPicture = "67";
                    break;

                case 10:
                    i.itemId = 68;
                    i.itemName = "U盘录音文件";
                    i.itemPicture = "68";
                    break;

                case 11:
                    i.itemId = 69;
                    i.itemName = "香水";
                    i.itemPicture = "69";
                    break;

                case 12:
                    i.itemId = 70;
                    i.itemName = "陈年葡萄酒";
                    i.itemPicture = "70";
                    break;

                case 13:
                    i.itemId = 71;
                    i.itemName = "古董钟";
                    i.itemPicture = "71";
                    break;

                case 14:
                    i.itemId = 69;
                    i.itemName = "香水";
                    i.itemPicture = "69";
                    break;
                case 15:
                    i.itemId = 56;
                    i.itemName = "灭火器";
                    i.itemPicture = "56";
                    break;

                default:
                    i.itemId = 71;
                    i.itemName = "古董钟";
                    i.itemPicture = "71";
                    break;
            }


            return i;
        }

        public static int TotalPDGroup()
        {
            int count = 0;
            Globals.System.PD.pdTeams.ForEach(x =>
            {
                if (!x.name.Contains("#"))
                    count++;
            });

            return count;
        }
    }
}
