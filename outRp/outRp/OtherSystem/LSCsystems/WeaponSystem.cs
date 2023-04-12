using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net; using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltV.Net.Resources.Chat.Api;
using outRp.Models;
using outRp.Chat;
using outRp.Globals;
using Newtonsoft.Json;
using outRp.Core;

namespace outRp.OtherSystem.LSCsystems
{
    public class WeaponSystem : IScript
    {
       public class WeaponModel
        {
            public int ID { get; set; } = 0;
            public int slot { get; set; } = 1; // 1 melee - 2 secondary - 3 primary
            public uint WeaponHash { get; set; } = 0;
            public int bullet { get; set; } = 0;
            public string Serial { get; set; } = "RU001";
            public double Durability { get; set; } = 100;
            public byte tint { get; set; } = 0;
            public List<uint> Components { get; set; } = new List<uint>();
            
        }  // {ID: 0, slot: 2, WeaponHash: 736523883, bullet: 20, Serial: "RU001", Durability: 100, tint: 0, Components: [0]}

        public class MagazineModel
        {
            public int ID { get; set; } = 0;
            public uint toWeapon { get; set; } = 0;
            public int bulletCount { get; set; } = 0;
        } // {ID: 0, toWEapon: 2323, bulletCount: 30}

        public class ComponentModel
        {
            public uint toWeapon { get; set; } = 0;
            public uint componentHash { get; set; } = 0;
        } // {toWeapon: 2323, componentHash: 222}
       
        public class TintModel
        {
            public uint toWeapon { get; set; } = 0;
            public byte tint { get; set; } = 0;
        } // {toWeapon: 222, tint: 2}
        public class WepCons
        {
            public static string melee = "Weapon_Melee";
            public static string secondary = "Weapon_Secondary";
            public static string primary = "Weapon_Primary";

        }

        [Command("addguntocar")]
        public static async void AddWeaponToVehicle(PlayerModel p, params string[] args) {
            if(p.adminLevel < 5) {
                MainChat.SendErrorChat(p, "[错误] 无权操作!");
                return;
            }

            if(args.Length <= 4) {
                MainChat.SendInfoChat(p, "[错误] 用法: /addguntocar [武器] [弹药] [类型] [序列号(RU-EN)] [数值]");
                return;
            }

            if(!Int32.TryParse(args[1], out int bullet) || !Int32.TryParse(args[2], out int weaponType) || !Int32.TryParse(args[4], out int amount)){
                MainChat.SendInfoChat(p, "[错误] 用法: /addguntocar [武器] [弹药] [tip] [序列号(RU-EN)] [数值]");
                return;
            }

            VehModel veh = Vehicle.VehicleMain.getNearVehFromPlayer(p);

            if(veh == null) {
                MainChat.SendErrorChat(p, "[错误] 无效车辆.");
                return;
            }

            var inv = JsonConvert.DeserializeObject<List<ServerItems>>(veh.vehInv);
            ServerItems item = new ServerItems();
            int slot = 1;
            switch(weaponType) {
                case 1:
                    slot = 1;
                    item = Items._LSCitems.Find(x => x.ID == 28);
                    break;

                case 2:
                    slot = 2;
                    item = Items._LSCitems.Find(x => x.ID == 29);
                    break;
                case 3:
                    slot = 3;
                    item = Items._LSCitems.Find(x => x.ID == 30);
                    break;

                default: 
                    MainChat.SendInfoChat(p, "[错误] 用法: /addguntocar [武器] [弹药] [tip] [序列号(RU-EN)] [数值]");
                    return;
            }
            WeaponModel newWep = new WeaponModel() {
                slot = slot,
                bullet = bullet,
                WeaponHash = Alt.Hash(args[0]),
                Serial = ""
            };

            for(int i = 0; i < amount; ++i) {
                string serialText = args[3] + "-" + i;
                newWep.Serial = serialText;
                var _i = new ServerItems();
                _i.ID = item.ID;
                _i.name = serialText;
                _i.type = item.type;
                _i.selectID = item.selectID;
                _i.picture = item.picture;
                _i.weight = item.weight;
                _i.data = "0";
                _i.data2 = JsonConvert.SerializeObject(newWep);
                _i.stackable = false;
                _i.equipable = item.equipable;
                _i.equipSlot = item.equipSlot;
                _i.amount = 1;
                _i.objectModel = item.objectModel;
                _i.isNameChange = false;
                inv.Add(_i);
            }

            if(veh.Exists){
                veh.vehInv = JsonConvert.SerializeObject(inv);
                veh.Update();
            }

            if(p.Exists)
                MainChat.SendInfoChat(p, "[?] 已添加 " + amount + " 个武器 " + args[3] + " 至车辆.");
            return;
        }

        [Command("giveadmingun")]
        public static void COM_GiveAdminWeapon(PlayerModel p, params string[] args)
        {
            if(args.Length < 4) { MainChat.SendInfoChat(p, "[用法] /giveadmingun [ID] [武器HASH] [弹药] [类型] [序列号]<br>类型: <br>1: 近战武器<br>2: 副武器<br>3: 主武器"); return; }
            if (p.adminLevel < 4) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            if(!Int32.TryParse(args[0], out int tSql)) { MainChat.SendInfoChat(p, "[用法] /giveadmingun [ID] [武器HASH] [弹药] [类型] [序列号]<br>类型: <br>1: 近战武器<br>2: 副武器<br>3: 主武器"); return; }
            if(!uint.TryParse(args[1], out uint weaponHash)) { MainChat.SendInfoChat(p, "[用法] /giveadmingun [ID] [武器HASH] [弹药] [类型] [序列号]<br>类型: <br>1: 近战武器<br>2: 副武器<br>3: 主武器"); return; }
            if(!Int32.TryParse(args[2], out int bullet)) { MainChat.SendInfoChat(p, "[用法] /giveadmingun [ID] [武器HASH] [弹药] [类型] [序列号]<br>类型: <br>1: 近战武器<br>2: 副武器<br>3: 主武器"); return; }
            if(!Int32.TryParse(args[3], out int type)) { MainChat.SendInfoChat(p, "[用法] /giveadmingun [ID] [武器HASH] [弹药] [类型] [序列号]<br>类型: <br>1: 近战武器<br>2: 副武器<br>3: 主武器"); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if(t == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家!"); return; }

            ServerItems item = new ServerItems();
            int slot = 1;
            switch (type)
            {
                case 1:
                    slot = 1;
                    item = Items.LSCitems.Find(x => x.ID == 28);
                    break;

                case 2:
                    slot = 2;
                    item = Items.LSCitems.Find(x => x.ID == 29);
                    break;

                case 3:
                    slot = 3;
                    item = Items.LSCitems.Find(x => x.ID == 30);
                    break;

                default:
                    MainChat.SendInfoChat(p, "[用法] /giveadmingun [ID] [武器HASH] [弹药] [类型] [序列号]<br>类型: <br>1: 近战武器<br>2: 副武器<br>3: 主武器"); return;
            }

            WeaponModel newWep = new WeaponModel() {
                slot = slot,
                bullet = bullet,
                WeaponHash = weaponHash,
                Serial = String.Join(" ", args[4..])
            };

            item.amount = 1;
            item.data = "0";
            item.data2 = JsonConvert.SerializeObject(newWep);
            Inventory.AddInventoryItem(t, item, 1);
            MainChat.SendInfoChat(p, "[?] " + t.characterName.Replace("_", " ") + " 被您给予武器.");
            MainChat.SendInfoChat(t, "[?] 您收到了管理员给予的武器.");
            return;
        }
        [Command("giveammo")]
        public static void COM_GiveBullet(PlayerModel p, params string[] args)
        {
            if(p.adminLevel < 3) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            if(args.Length < 4) { MainChat.SendInfoChat(p, "[用法] /giveammo [ID] [武器HASH] [弹药数量] [弹匣名称]"); return; }
            if (!Int32.TryParse(args[0], out int targetSql)) { MainChat.SendInfoChat(p, "[用法] /giveammo [ID] [武器HASH] [弹药数量] [弹匣名称]"); return; }
            if(!uint.TryParse(args[1], out uint hash)) { MainChat.SendInfoChat(p, "[用法] /giveammo [ID] [武器HASH] [弹药数量] [弹匣名称]"); return; }
            if(!Int32.TryParse(args[2], out int bullet)) { MainChat.SendInfoChat(p, "[用法] /giveammo [ID] [武器HASH] [弹药数量] [弹匣名称]"); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(targetSql);
            if(t == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家."); return; }

            ServerItems item = Items.LSCitems.Find(x => x.ID == 46);

            MagazineModel newWep = new MagazineModel()
            {
                bulletCount = bullet,
                toWeapon = hash
            };

            item.name = string.Join(" ", args[3..]);
            item.amount = 1;
            item.data = "0";
            item.data2 = JsonConvert.SerializeObject(newWep);
            Inventory.AddInventoryItem(t, item, 1);
            MainChat.SendInfoChat(p, "[*] " + t.characterName.Replace("_", " ") + " 被您给予弹药.");
            return;
        }

        [Command("giveguncomp")]
        public static async Task COM_GiveWeaponComponent(PlayerModel p, params string[] args)
        {
            if(p.adminLevel < 3) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            if(args.Length < 2) { MainChat.SendInfoChat(p, "[用法] /giveguncomp [ID] [武器HASH] [配件HASH]"); return; }
            if (!Int32.TryParse(args[0], out int targetSql)) { MainChat.SendInfoChat(p, "[用法] /giveguncomp [ID] [武器HASH] [配件HASH]"); return; }
            if (!uint.TryParse(args[1], out uint hash)) { MainChat.SendInfoChat(p, "[用法] /giveguncomp [ID] [武器HASH] [配件HASH]"); return; }
            if (!uint.TryParse(args[2], out uint type)) { MainChat.SendInfoChat(p, "[用法] /giveguncomp [ID] [武器HASH] [配件HASH]"); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(targetSql);
            if(t == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家."); return; }

            ServerItems item = Items.LSCitems.Find(x => x.ID == 47);

            ComponentModel newWep = new ComponentModel()
            {
                componentHash = hash,
                toWeapon = type
            };

            item.amount = 1;
            item.data = "0";
            item.data2 = JsonConvert.SerializeObject(newWep);
            await Inventory.AddInventoryItem(t, item, 1);
            MainChat.SendInfoChat(p, "[*] " + t.characterName.Replace("_", " ") + " 被您给予武器配件.");
            return;

        }
        public static async Task<bool> TakeWeapon(PlayerModel p, InventoryModel i)
        {   

            WeaponModel x = JsonConvert.DeserializeObject<WeaponModel>(i.itemData2);
            if (x == null)
                return false;

            x.ID = i.ID;
            switch (x.slot)
            {
                case 1: // Melee.
                    if (p.melee != null)
                        return false;

                    p.melee = x;

                    break;

                case 2: // Secondary
                    if (p.secondary != null)
                        return false;

                    p.secondary = x;
                    break;

                case 3: // Primary
                    if (p.primary != null)
                        return false;

                    p.primary = x;

                    break;

                default: return false;
            }

            p.GiveWeapon(x.WeaponHash, x.bullet, true);
            foreach(uint comp in x.Components)
            {
                p.AddWeaponComponent(x.WeaponHash, comp);
            }
            p.SetWeaponTintIndex(x.WeaponHash, x.tint);

            i.itemData2 = JsonConvert.SerializeObject(x);
            await i.Update();
            await Inventory.UpdatePlayerInventory(p);
            p.EmitLocked("Weapon:Add", x.WeaponHash, x.bullet);
            return true;

        }

        [AsyncScriptEvent(ScriptEventType.PlayerWeaponChange)]
        public async Task OnPlayerWeaponChange(IPlayer player, uint oldWeapon, uint newWeapon)
        {
            if (newWeapon != null)
                player.EmitLocked("Weapon:SwitchAnimation");
        }

        public static async Task<bool> TakeOutWeapon(PlayerModel p, InventoryModel i)
        {
            WeaponModel x = JsonConvert.DeserializeObject<WeaponModel>(i.itemData2);
            if (x == null)
                return false;

            switch (x.slot)
            {
                case 1:
                    if (p.melee == null)
                        return false;

                    i.itemData2 = JsonConvert.SerializeObject(p.melee);
                    await i.Update();                    
                    p.RemoveWeapon(p.melee.WeaponHash);
                    p.EmitLocked("Weapon:Remove", p.melee.WeaponHash);
                    p.melee = null;

                    await Inventory.UpdatePlayerInventory(p);                    
                    return true;

                case 2:
                    if (p.secondary == null)
                        return false;

                    i.itemData2 = JsonConvert.SerializeObject(p.secondary);
                    await i.Update();                    
                    p.RemoveWeapon(p.secondary.WeaponHash);
                    p.EmitLocked("Weapon:Remove", p.secondary.WeaponHash);
                    p.secondary = null;

                    await Inventory.UpdatePlayerInventory(p);
                    return true;

                case 3:
                    if (p.primary == null)
                        return false;

                    i.itemData2 = JsonConvert.SerializeObject(p.primary);
                    await i.Update();
                    p.RemoveWeapon(p.primary.WeaponHash);
                    p.EmitLocked("Weapon:Remove", p.primary.WeaponHash);
                    p.primary = null;

                    await Inventory.UpdatePlayerInventory(p);
                    return true;

                default: return false;
            }

            

        }

        public static void CheckWeaponTake(PlayerModel p, InventoryModel i)
        {
            //AltV.Net.Enums.WeaponModel.SMG
            WeaponModel x = JsonConvert.DeserializeObject<WeaponModel>(i.itemData2);
            if (x == null)
                return;

            switch (x.slot)
            {
                case 1:
                    if (p.melee != null)
                        return;

                    p.melee = new WeaponModel();
                    p.melee = x;
                    return;

                case 2:
                    if (p.secondary != null)
                        return;

                    p.secondary = new WeaponModel();
                    p.secondary = x;
                    return;

                case 3:
                    if (p.primary != null)
                        return;

                    p.primary = new WeaponModel();
                    p.primary = x;
                    return;

                default: return;
            }
        }

        public static bool AddBulletFromMagazine(PlayerModel p, InventoryModel i)
        {
            MagazineModel m = JsonConvert.DeserializeObject<MagazineModel>(i.itemData2);
            if (m == null)
                return false;

            if(p.secondary != null && p.secondary.WeaponHash == m.toWeapon)
            {
                if(p.secondary.bullet > 100)
                    return false;
                p.secondary.bullet += m.bulletCount;
                GlobalEvents.ProgresBar(p, "正在填装弹药...", 2);
                p.EmitAsync("Weapon:AddMagazine", m.toWeapon, m.bulletCount);
                return true;
            }
            else if(p.primary != null && p.primary.WeaponHash == m.toWeapon)
            {
                if(p.primary.bullet > 100)
                    return false;
                p.primary.bullet += m.bulletCount;
                GlobalEvents.ProgresBar(p, "正在填装弹药...", 2);
                p.EmitAsync("Weapon:AddMagazine", m.toWeapon, m.bulletCount);
                return true;
            }

            return false;

        }

        public static bool AddComponentFromItem(PlayerModel p, InventoryModel i)
        {
            ComponentModel x = JsonConvert.DeserializeObject<ComponentModel>(i.itemData2);
            if (x == null)
                return false;

            if(p.secondary != null && p.secondary.WeaponHash == x.toWeapon)
            {
                p.secondary.Components.Add(x.componentHash);
                p.AddWeaponComponent(p.secondary.WeaponHash, x.componentHash);
                GlobalEvents.ProgresBar(p, "正在安装武器配件...", 2);
                return true;
            }
            else if (p.primary != null && p.primary.WeaponHash == x.toWeapon)
            {
                p.primary.Components.Add(x.componentHash);
                p.AddWeaponComponent(p.primary.WeaponHash, x.componentHash);
                GlobalEvents.ProgresBar(p, "正在安装武器配件...", 2);
                return true;
            }
            else if(p.melee != null && p.melee.WeaponHash == x.toWeapon)
            {
                p.melee.Components.Add(x.componentHash);
                p.AddWeaponComponent(p.melee.WeaponHash, x.componentHash);
                GlobalEvents.ProgresBar(p, "正在安装武器配件...", 2);
                return true;
            }

            return false;

        }

        public static bool AddWeaponTintFromItem(PlayerModel p, InventoryModel i)
        {
            TintModel t = JsonConvert.DeserializeObject<TintModel>(i.itemData2);
            if (t == null)
                return false;

            if(p.secondary != null && p.secondary.WeaponHash == t.toWeapon)
            {
                p.SetWeaponTintIndex(p.secondary.WeaponHash, t.tint);
                p.secondary.tint = t.tint;
                GlobalEvents.ProgresBar(p, "正在给武器上漆...", 3);
                return true;
            }
            else if (p.primary != null && p.primary.WeaponHash == t.toWeapon)
            {
                p.SetWeaponTintIndex(p.primary.WeaponHash, t.tint);
                p.secondary.tint = t.tint;
                GlobalEvents.ProgresBar(p, "正在给武器上漆...", 3);
                return true;
            }

            return false;
        }

        [AsyncClientEvent("Weapon:BulletUpdate")]
        public void WeaponShooting(PlayerModel p, uint weaponHash, int bullet)
        {
            if(p.secondary != null && p.secondary.WeaponHash == weaponHash)
            {
                if(p.secondary.bullet < bullet) { Core.Logger.WriteLogData(Logger.logTypes.AntiCheat, p.characterName + " 疑似刷子弹/BUG作弊!"); MainChat.SendAdminChat(p.characterName + " 疑似刷子弹/BUG作弊."); }

                p.secondary.bullet = bullet;
                p.secondary.Durability -= 0.05;
                return;
            }
            else if(p.primary != null && p.primary.WeaponHash == weaponHash)
            {
                if (p.primary.bullet < bullet) { Core.Logger.WriteLogData(Logger.logTypes.AntiCheat, p.characterName + " 疑似刷子弹/BUG作弊!"); MainChat.SendAdminChat(p.characterName + " 疑似刷子弹/BUG作弊."); }

                p.primary.bullet = bullet;
                p.primary.Durability -= 0.05;
                return;
            }

            return;
        }

        [AsyncScriptEvent(ScriptEventType.PlayerDisconnect)]
        public async Task Player_Disconnect(PlayerModel p,string reason)
        {            
            if(p.melee != null)
            {
                InventoryModel i = await Database.DatabaseMain.FindInventoryItem(p.melee.ID);
                if (i.itemId <= 0)
                    return;

                i.itemData2 = JsonConvert.SerializeObject(p.melee);
                await i.Update();
            }

            if(p.secondary != null)
            {
                InventoryModel i2 = await Database.DatabaseMain.FindInventoryItem(p.secondary.ID);
                if (i2.itemId <= 0)
                    return;

                i2.itemData2 = JsonConvert.SerializeObject(p.secondary);
                await i2.Update();
            }

            if(p.primary != null)
            {
                InventoryModel i3 = await Database.DatabaseMain.FindInventoryItem(p.primary.ID);
                if (i3.itemId <= 0)
                    return;
                
                i3.itemData2 = JsonConvert.SerializeObject(p.primary);
                await i3.Update();
            }

            return;
        }
    
    }
}


/*
 
        public class WeaponModel
        {
            public int itemID { get; set; } = 0;
            public uint weaponModel { get; set; } = 0;
            public int bullet { get; set; } = 0;
            public int searialNo { get; set; } = 0;
            public string mensei { get; set; } = "RU";
            public int weaponTint { get; set; } = 0;

            public List<Components> components { get; set; } = new List<Components>();

            public class Components
            {
                public uint Component { get; set; } = 0;
            }
        }

        public class wCons
        {
            public static string weapon = "PlayerWeapon_";
        }

        public void addWeapon(PlayerModel p, int itemID, string weaponModel)
        {
            WeaponModel wep = JsonConvert.DeserializeObject<WeaponModel>(weaponModel);
            if(wep == null) { Alt.Log(p.characterName + " isimli oyuncunun silahıyla ilgili bir problem meydana geldi."); return; }

            wep.itemID = itemID;

            p.GiveWeapon(wep.weaponModel, wep.bullet, true);
            foreach(WeaponModel.Components comp in wep.components)
            {
                p.AddWeaponComponent(wep.weaponModel, comp.Component);
            }

            p.SetData(wCons.weapon + wep.weaponModel.ToString(), wep);
        }

        public void RemoveWeapon(PlayerModel p, int itemID, string weaponModel)
        {
            WeaponModel wep = JsonConvert.DeserializeObject<WeaponModel>(weaponModel);
            if(wep == null) { Alt.Log(p.characterName + " isimli oyuncunun silahıyla ilgili bir problem meydana geldi."); return; }
               

            

            p.RemoveWeapon(wep.weaponModel);
            p.DeleteData(wCons.weapon + wep.weaponModel.ToString());

        }



        // Events

 
 */
