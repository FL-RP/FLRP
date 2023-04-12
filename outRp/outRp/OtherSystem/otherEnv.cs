using System;
using System.Collections.Generic;
using AltV.Net; using AltV.Net.Async;
using AltV.Net.Data;
using outRp.Models;
using outRp.Globals;
using outRp.Chat;
using Newtonsoft.Json;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;
using outRp.OtherSystem.NativeUi;
using outRp.Vehicle;
using outRp.OtherSystem.Textlabels;

namespace outRp.OtherSystem
{
    public class otherEnv : IScript
    {
        /*
         * ENV Types
         * -----------
         * 1 : Vehicle | 2 Business | 3 House | 4 : Crate / Bag | 5 Player 
         */

        [Command("trunkinv")]
        public static void COM_VehInventory(PlayerModel p)
        {
            VehModel v = VehicleMain.getNearVehFromPlayer(p);
            if(v == null) { MainChat.SendErrorChat(p, CONSTANT.COM_VQueryVehNotNear); return; }
            if(v.Position.Distance(p.Position) > 4) { MainChat.SendErrorChat(p, CONSTANT.COM_VQueryVehNotNear); return; }
            if(v.settings.TrunkLock == true) { MainChat.SendErrorChat(p, "[错误] 此车后备箱是锁的."); return; }
            if(v.isTrunkOpen != true) { MainChat.SendErrorChat(p, "[错误] 请先打开车辆后备箱."); return; }
            GlobalEvents.NativeNotifyVehicle(v, "~g~车辆后备箱~n~~b~容量: " + v.inventoryCapacity.ToString());
            p.EmitLocked("otherEnv:Show", 1, v.vehInv);            
        }

        public static void ShowBag(PlayerModel p, BagModel b)
        {
            p.EmitLocked("otherEnv:Show", 4, b.Env);
        }

        [Native("otherInv:WantToTake")]
        public static async Task WantToTake(PlayerModel p, string iType, string iID)
        {
            int type; bool isTypeOk = Int32.TryParse(iType, out type);
            int id; bool isIdOk = Int32.TryParse(iID, out id);

            if (!isTypeOk || !isIdOk)
                return;
            
            await ServerEvents.EVENT_Defender(p, "otherInv:WantToTake", null);
            if(type == 1)
            {
                VehModel v = VehicleMain.getNearVehFromPlayer(p);
                if (v == null || v.Position.Distance(p.Position) > 4) { return; }
                if (v.settings.TrunkLock) { MainChat.SendErrorChat(p, "[错误] 车辆后备箱是锁的."); return; }
                List<ServerItems> items = JsonConvert.DeserializeObject<List<ServerItems>>(v.vehInv);
                ServerItems targetItem = items.Find(x => x.selectID == id);
                if(targetItem == null) { return; }
                if(targetItem.amount <= 0) { MainChat.SendErrorChat(p, "[!] 未找到相应物品!"); return; }
                bool succes = await Inventory.AddInventoryItem(p, targetItem, 1);
                if (succes) 
                { 
                    GlobalEvents.notify(p, 2, "成功取得物品");
                    if(targetItem.amount <= 1) 
                    { 
                        items.Remove(targetItem);
                        v.vehInv = JsonConvert.SerializeObject(items);
                        v.Update();
                        p.EmitLocked("otherEnv:Reload", 1, v.vehInv);
                    }
                    else
                    {
                        targetItem.amount -= 1;
                        v.vehInv = JsonConvert.SerializeObject(items);
                        v.Update();
                        p.EmitLocked("otherEnv:Reload", 1, v.vehInv);
                    }
                    Core.Logger.WriteLogData(Core.Logger.logTypes.InventoryLog, p.characterName + " <- " + v.sqlID + " 从车辆后备箱取出 " + targetItem.name);
                }
                else { GlobalEvents.notify(p, 3, "您的库存满了!"); }
            }
            else if(type == 2)
            {
                PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Find(x => p.Position.Distance(x.Position) < 3f && x.Dimension == p.Dimension);
                if (entranceLabel == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_NotNearHouse); return; }
                entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position bussinesPos);

                var t = await Props.Business.getBusinessFromPos(bussinesPos);

                if (t.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 附近没有产业!"); return; }

                if (!await Props.Business.CheckBusinessKey(p, t.Item1)) { MainChat.SendErrorChat(p, "[错误] 您没有此产业的钥匙!"); return; }

                List<ServerItems> items = JsonConvert.DeserializeObject<List<ServerItems>>(t.Item1.settings.Env);
                ServerItems targetItem = items.Find(x => x.selectID == id);
                if (targetItem == null) { return; }

                bool succes = await Inventory.AddInventoryItem(p, targetItem, 1);
                if (succes)
                {
                    GlobalEvents.notify(p, 2, "成功取得物品");
                    if (targetItem.amount <= 1)
                    {
                        items.Remove(targetItem);
                        t.Item1.settings.Env = JsonConvert.SerializeObject(items);
                        await t.Item1.Update(t.Item2, t.Item3);
                        p.EmitLocked("otherEnv:Reload", 2, t.Item1.settings.Env);
                    }
                    else
                    {
                        targetItem.amount -= 1;
                        t.Item1.settings.Env = JsonConvert.SerializeObject(items);
                        await t.Item1.Update(t.Item2, t.Item3);
                        p.EmitLocked("otherEnv:Reload", 2, t.Item1.settings.Env);
                    }
                    Core.Logger.WriteLogData(Core.Logger.logTypes.InventoryLog, p.characterName + " <- " + t.Item1.ID + " 从产业取出 " + targetItem.name);
                }
                else { GlobalEvents.notify(p, 3, "您的库存满了!"); }
            }
            else if(type == 3)
            {
                PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Find(x => p.Position.Distance(x.Position) < 3f && x.Dimension == p.Dimension);
                if (entranceLabel == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_NotNearHouse); return; }
                entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position bussinesPos);

                (HouseModel, PlayerLabel, Marker) t = await Props.Houses.getHouseFromPos(bussinesPos);

                if (t.Item1 == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_NotNearHouse); return; }

                if (!await Props.Houses.HouseKeysQuery(p, t.Item1)) { MainChat.SendErrorChat(p, "[错误] 您没有此房屋的钥匙!"); return; }

                List<ServerItems> items = JsonConvert.DeserializeObject<List<ServerItems>>(t.Item1.houseEnv);
                ServerItems targetItem = items.Find(x => x.selectID == id);
                if (targetItem == null) { return; }

                bool succes = await Inventory.AddInventoryItem(p, targetItem, 1);
                if (succes)
                {
                    GlobalEvents.notify(p, 2, "成功取得物品");
                    if (targetItem.amount <= 1)
                    {
                        items.Remove(targetItem);
                        t.Item1.houseEnv = JsonConvert.SerializeObject(items);
                        t.Item1.Update(t.Item3, t.Item2);
                        p.EmitLocked("otherEnv:Reload", 3, t.Item1.houseEnv);
                    }
                    else
                    {
                        targetItem.amount -= 1;
                        t.Item1.houseEnv = JsonConvert.SerializeObject(items);
                        t.Item1.Update(t.Item3, t.Item2);
                        p.EmitLocked("otherEnv:Reload", 3, t.Item1.houseEnv);
                    }
                    Core.Logger.WriteLogData(Core.Logger.logTypes.InventoryLog, p.characterName + " <- " + t.Item1.ID + " 从房屋取出 " + targetItem.name + " aldı");
                }
                else { GlobalEvents.notify(p, 3, "您的背包满了!"); }

            }
            else if(type == 4)
            {
                BagModel bag = BagEvents.serverBags.Find(x => p.Position.Distance(x.prop.Position) < 3);
                if(bag == null) { return; }
                List<ServerItems> items = JsonConvert.DeserializeObject<List<ServerItems>>(bag.Env);
                ServerItems targetItem = items.Find(x => x.selectID == id);
                if (targetItem == null) { return; }
                bool succes = await Inventory.AddInventoryItem(p, targetItem, 1);
                if (succes)
                {
                    GlobalEvents.notify(p, 2, "成功取得物品");
                    if (targetItem.amount <= 1)
                    {
                        items.Remove(targetItem);
                        bag.Env = JsonConvert.SerializeObject(items);
                        bag.Update();
                        p.EmitLocked("otherEnv:Reload", 4, bag.Env);
                    }
                    else
                    {
                        targetItem.amount -= 1;
                        bag.Env = JsonConvert.SerializeObject(items);
                        bag.Update();
                        p.EmitLocked("otherEnv:Reload", 4, bag.Env);
                    }
                }
                else { GlobalEvents.notify(p, 3, "您的背包满了!"); }
            }
        }
         
    }
}
