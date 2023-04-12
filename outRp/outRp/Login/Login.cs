using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Core;
using outRp.Globals;
using outRp.Kook;
using outRp.Models;
using outRp.OtherSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using AltV.Net.Resources.Chat.Api;
using outRp.Tutorial;

namespace outRp.Login
{
    public class UpgradeNotes
    {
        //public int ID { get; set; }
        public string header { get; set; }
        public string text { get; set; }
    }
    public class playerSex
    {
        public int sex { get; set; } = 0;
    }
    public class Login : IScript
    {
        private const string _pattern = @"^[A-Za-z]+(?: |_)[A-Za-z]+$";
        public static void loginSystem()
        {
            //Alt.OnClient<PlayerModel, string, string>("LoginAttemp", LoginAttemp);

            //Alt.OnClient<PlayerModel, int>("login:selectedChar", characterSelect);

            //Alt.OnClient<PlayerModel, string>("login:SetupComp", setupComp);

            //Alt.OnClient<PlayerModel, string>("characterPage:CreateChar", CreateChar);

            //Alt.OnClient<PlayerModel, string>("characterPage:SettingChar", CharacterSettings);
        }

        [AsyncClientEvent("LoginAttemp")]
        public static async Task LoginAttemp(PlayerModel player) 
        {
            try
            {
                Alt.Log($"收到 {player.SocialClubId} 登录请求");
                if (player.Ping > 250)
                {
                    Alt.Log($"{player.SocialClubId} 登录请求因延迟过大已踢出");
                    return;
                }
                AccountModel account = await Database.DatabaseMain.getAccountInfo(player.SocialClubId);

                if (account.kookId == null)
                {
                    Alt.Log($"{player.SocialClubId} 新用户注册");
                    var tempAuthId = System.Guid.NewGuid().ToString();
                    Alt.Log($"{player.SocialClubId} tempAuthId: {tempAuthId}");
                    player.EmitLocked("login:New", tempAuthId);
                    Alt.Log($"{player.SocialClubId} {tempAuthId} 已调用 login:New 等待 KOOK 指令");
                    var kookId = await KookSpace.Register(tempAuthId);
                    Alt.Log($"{player.SocialClubId} {tempAuthId} kookId: {kookId}");
                    if (kookId == null)
                    {
                        Alt.Log($"{player.SocialClubId} {tempAuthId} 未获取到 kookId 调用 Login:Failed");
                        player.EmitLocked("login:Failed", "您已超时未进行操作, 登录失败");
                        Globals.GlobalEvents.notify(player, 3, "您已超时未进行操作, 登录失败");
                        return;
                    } else
                    {
                        Alt.Log($"{player.SocialClubId} {tempAuthId} 已获取 kookId 正在写入数据库");
                        int resultId = await Database.DatabaseMain.CreateAccount(player, kookId);
                        if (resultId == -1)
                        {
                            Alt.Log($"{player.SocialClubId} {tempAuthId} 创建失败, 请联系管理员");
                            player.EmitLocked("login:Failed", "创建失败, 请联系管理员");
                            Globals.GlobalEvents.notify(player, 3, "创建失败, 请联系管理员");
                            return;
                        }
                        Alt.Log($"{player.SocialClubId} {tempAuthId} 写入完成 注册完成");
                    }
                
                } else
                {
                    Alt.Log($"scId: {player.SocialClubId} kookId: {account.kookId} 该用户已注册, 正在等待 KOOK 验证");
                    KookSpace.LoginResult loginResult = await KookSpace.Login(account.kookId);
                    Alt.Log($"{player.SocialClubId} {account.kookId} 已收到登陆结果 {loginResult.result}");
                    if (!loginResult.result)
                    {
                        Alt.Log($"scId: {player.SocialClubId} kookId: {account.kookId} 登陆验证失败");
                        string message = loginResult.message;
                        if (message.Length == 0)
                        { 
                            message = "您已超时未进行操作, 登录失败";
                        }
                    
                        Alt.Log($"scId: {player.SocialClubId} kookId: {account.kookId} 失败原因: {message}");
                        player.EmitLocked("login:Failed", message);
                        Alt.Log($"scId: {player.SocialClubId} kookId: {account.kookId} 已发送, login:Failed");
                        Globals.GlobalEvents.notify(player, 3, message);
                        return;
                    }
                }
                account = await Database.DatabaseMain.getAccountInfo(player.SocialClubId);
                Alt.Log($"scId: {player.SocialClubId} kookId: {account.kookId} 登录成功");

                foreach(PlayerModel checkPl in Alt.GetAllPlayers()) {
                    if(checkPl.accountId == account.ID) { checkPl.Kick("很抱歉! 此账号已在其他设备登录."); }
                }
                
                if (account.banned)
                {
                    Globals.GlobalEvents.notify(player, 0, "很抱歉! 此账号已被服务器封禁."); await Task.Delay(5000);
                    if (player.Exists)
                    {
                        player.Kick("很抱歉! 此账号已被服务器封禁, 您已被系统踢出服务器.");
                    }
                    return;
                }
                List<AccountCharacterModel> accountCharacters = await Database.DatabaseMain.getAccountCharacters(account.ID);
                foreach (var c in accountCharacters)
                {
                    FactionModel f = await Database.DatabaseMain.GetFactionInfo(c.factionId);
                    c.factionName = f.name;
                }
                player.SetData("AccountId", account.ID);
                account.isOnline = true;
                string json = JsonConvert.SerializeObject(accountCharacters.OrderBy(x => x.isCK));
                player.EmitLocked("login:Succes", json, account.characterLimit);
                if (account.socialClubID != 0)
                {
                    if (account.socialClubID != player.SocialClubId) { Core.Logger.WriteLogData(Logger.logTypes.multiaccount, "旧 Social ID: " + account.socialClubID.ToString() + " 新的 ID: " + player.SocialClubId.ToString() + " | 账号 ID :" + account.forumName); }
                }
                else
                {
                    account.socialClubID = player.SocialClubId;

                }

                if (account.HwId != player.HardwareIdExHash || account.HwIdEx != player.HardwareIdExHash)
                {
                    Core.Logger.WriteLogData(Logger.logTypes.multiaccount, "OLD hwid: " + account.HwId + " New: " + player.HardwareIdHash + " / Old hwidEx: " + account.HwIdEx + " New: " + player.HardwareIdExHash + " | Account ID :" + account.forumName);
                    account.HwId = player.HardwareIdHash;
                    account.HwIdEx = player.HardwareIdExHash;
                }

                Core.Logger.WriteLogData(Logger.logTypes.ip, account.kookId + " -> " + player.Ip);

                await Database.DatabaseMain.updateAccInfo(account);
            }
            catch(Exception ex)
            {
                Alt.LogError(ex.Message);
                Alt.LogError(ex.StackTrace);
            }
        }


/*
        [AsyncClientEvent("LoginAttemp")]
        public static async Task LoginAttemp(PlayerModel player, string username, string password)
        {
            if (player.Ping > 250)
                return;
            AccountModel account = await Database.DatabaseMain.getAccountInfo(username);
            if (account != null)
            {
                string sha1 = Core.Core.idPassConverter(username, password).ToLower();
                //string input = Core.Core.idPassConverter(username, "123456").ToLower();
                //Alt.Log(input);
                if (sha1 == account.password)
                {
                    //if (account.isOnline == true) { Globals.GlobalEvents.notify(player, 3, "Bu hesaba zaten giriş yapılmış."); return; }
                    foreach(PlayerModel checkPl in Alt.GetAllPlayers()) {
                        if(checkPl.accountId == account.ID) { checkPl.Kick("很抱歉! 此账号已在其他设备登录."); }
                    }
                    if (account.banned)
                    {
                        Globals.GlobalEvents.notify(player, 0, "很抱歉! 此账号已被服务器封禁."); await Task.Delay(5000);
                        if (player.Exists)
                        {
                            player.Kick("很抱歉! 此账号已被服务器封禁, 您已被系统踢出服务器.");
                        }
                        return;
                    }
                    List<AccountCharacterModel> accountCharacters = await Database.DatabaseMain.getAccountCharacters(account.ID);
                    foreach (var c in accountCharacters)
                    {
                        FactionModel f = await Database.DatabaseMain.GetFactionInfo(c.factionId);
                        c.factionName = f.name;
                    }
                    player.SetData("AccountId", account.ID);
                    account.isOnline = true;
                    string json = JsonConvert.SerializeObject(accountCharacters.OrderBy(x => x.isCK));
                    player.EmitLocked("login:Succes", json, account.characterLimit);
                    if (account.socialClubID != 0)
                    {
                        if (account.socialClubID != player.SocialClubId) { Core.Logger.WriteLogData(Logger.logTypes.multiaccount, "旧 Social ID: " + account.socialClubID.ToString() + " 新的 ID: " + player.SocialClubId.ToString() + " | 账号 ID :" + account.forumName); }
                    }
                    else
                    {
                        account.socialClubID = player.SocialClubId;

                    }

                    if (account.HwId != player.HardwareIdExHash || account.HwIdEx != player.HardwareIdExHash)
                    {
                        Core.Logger.WriteLogData(Logger.logTypes.multiaccount, "OLD hwid: " + account.HwId + " New: " + player.HardwareIdHash + " / Old hwidEx: " + account.HwIdEx + " New: " + player.HardwareIdExHash + " | Account ID :" + account.forumName);
                        account.HwId = player.HardwareIdHash;
                        account.HwIdEx = player.HardwareIdExHash;
                    }

                    Core.Logger.WriteLogData(Logger.logTypes.ip, account.userName + " -> " + player.Ip);

                    await Database.DatabaseMain.updateAccInfo(account);
                }
                else
                {
                    player.EmitLocked("login:Failed");
                    Globals.GlobalEvents.notify(player, 3, "很抱歉! 账号或者密码错误, 请重试!");
                    return;
                }

            }
            else { 
                player.EmitLocked("login:Failed"); Globals.GlobalEvents.notify(player, 3, "很抱歉! 账号或者密码错误, 请重试!"); return; 
            }
        }*/

        [AsyncClientEvent("login:selectedChar")]
        public static async void characterSelect(PlayerModel player, int characterId)
        {
            if (player.Ping > 250)
                return;
            try
            {
                PlayerModelInfo infoPlayer = await Database.DatabaseMain.getCharacterInfo(characterId);
                if (infoPlayer == null)
                    return;
                if (!player.HasData("AccountId"))
                {
                    await player.KickAsync("账号 Dump");

                    Core.Logger.WriteLogData(Core.Logger.logTypes.AntiCheat, "[踢出] " + infoPlayer.characterName + " 尝试DUMP (İp: " + player.Ip + ")");

                    return;
                }

                if (player.lscGetdata<int>("AccountId") != infoPlayer.accountId)
                {
                    await player.KickAsync("账号 Dump");
                    AccountModel infoAcc = await Database.DatabaseMain.getAccInfo(player.lscGetdata<int>("AccountId"));
                    if (infoAcc != null)
                        Logger.WriteLogData(Core.Logger.logTypes.AntiCheat, "[踢出] " + infoPlayer.characterName + " 尝试DUMP (İp: " + player.Ip + ") 登录账号: ");
                    else
                        Core.Logger.WriteLogData(Core.Logger.logTypes.AntiCheat, "[踢出] " + infoPlayer.characterName + " 尝试DUMP (İp: " + player.Ip + ")");

                    return;
                }
                GlobalEvents.CloseCamera(player);
                await player.Setup(characterId);
                player.SetData("LoginDate", DateTime.Now.ToString());
                foreach (PlayerModel tgp in Alt.GetAllPlayers())
                {
                    if (tgp.accountId == player.accountId && tgp.Id != player.Id)
                    {
                        tgp.Kick("很抱歉! 此账号已登录.");
                    }
                }
                //player.SetData("LoginDate", DateTime.Now);
                if (player.firstLogin == true)
                {
                    player.isFinishTut = 0;
                    player.Dimension = -1;
                    player.Position = new Position(-74f, -819f, 326f);
                    player.Rotation = new Rotation(0f, 0f, 1.1168559f);
                    Globals.GlobalEvents.GameControls(player, false);
                    if (DateTime.Now.Day > 30)
                        player.SetDateTime(DateTime.Now.Day - 1, DateTime.Now.Month - 1, DateTime.Now.Year, DateTime.Now.Hour, DateTime.Now.Minute, 5);
                    else
                        player.SetDateTime(DateTime.Now.Day - 1, DateTime.Now.Month - 1, DateTime.Now.Year, DateTime.Now.Hour, DateTime.Now.Minute, 5);

                    player.Position = new Position(-74f, -819f, 326f);
                    await Task.Delay(4000);

                    if (!player.Exists)
                        return;


                    player.EmitLocked("character:Edit");
                    player.isOnline = true;
                    player.Dimension = -1;
                }
                else
                {
                    player.MaxArmor = 1000;
                    playerSex s = JsonConvert.DeserializeObject<playerSex>(player.charComps);
                    if (s.sex == 0) await player.SetModelAsync(Alt.Hash("mp_f_freemode_01"));
                    else await player.SetModelAsync(Alt.Hash("mp_m_freemode_01"));
                    player.sex = s.sex;
                    player.EmitLocked("character:ServerSync", player.charComps);
                    if (DateTime.Now.Day > 30)
                        player.SetDateTime(DateTime.Now.Day - 1, DateTime.Now.Month - 1, DateTime.Now.Year, DateTime.Now.Hour, DateTime.Now.Minute, 5);
                    else
                        player.SetDateTime(DateTime.Now.Day - 1, DateTime.Now.Month - 1, DateTime.Now.Year, DateTime.Now.Hour, DateTime.Now.Minute, 5);


                    AccountModel acc = await Database.DatabaseMain.getAccInfo(player.accountId);
                    player.adminLevel = acc.adminLevel;

                    await Task.Delay(1000);
                    if (!player.Exists)
                        return;


                    player.isOnline = true;

                    CharacterSettings settings = JsonConvert.DeserializeObject<CharacterSettings>(player.settings);
                    if (settings != null)
                    {
                        Globals.GlobalEvents.SetClothes(player, 3, settings.arms, 0);
                        player.EmitLocked("chat:fontsize", settings.FontSize.ToString().Replace(",", ".") + "em");
                        player.EmitLocked("ct:font", settings.Font[0], settings.Font[1]);
                        player.EmitLocked("Tatto:Load", JsonConvert.SerializeObject(settings.tattos));
                        GlobalEvents.ChangeGraphicMode(player, settings.GrapichMode);
                    }
                    // Admin Jail Check
                    if (player.adminJail > 0)
                    {
                        player.Position = new Position(-1421.015f, -3012.587f, -80.000f);
                        player.Dimension = player.sqlID;
                        //Globals.GlobalEvents.GameControls(player, false);
                    }
                    await player.SetMaxHealthAsync((ushort)settings.maxHp).ContinueWith(async (task) =>
                    {
                        await player.SetHealthAsync((ushort)player.hp);
                    });

                    /* Eski Hali
                     * await player.SetMaxHealthAsync((ushort)settings.maxHp);
                     * await player.SetHealthAsync((ushort)player.hp);
                     */
                    List<InventoryModel> playerInventory = await Database.DatabaseMain.GetPlayerInventoryItems(player.sqlID);
                    if (playerInventory != null)
                    {
                        foreach (var i in playerInventory)
                        {
                            if (i.itemId == 1 && i.itemSlot == 12)
                            {
                                await OtherSystem.Phone.PhoneMain.TakePhone(player, i);
                                //OtherSystem.Phones.TakePhone(player, i);
                            }
                            else if ((i.itemId == 28 || i.itemId == 29 || i.itemId == 30) && i.itemSlot != 0)
                            {
                                takeWeaponWithDelay(player, i);
                            }
                        }
                        string json = JsonConvert.SerializeObject(playerInventory);
                        player.EmitLocked("inventory:Update", json);
                    }

                    if (acc.OtherData.Info.Count > 0)
                    {
                        MainChat.SendInfoChat(player, "{2d90a9}[服务器] {FFFFFF}您有 {DCFF00}" + acc.OtherData.Info.Count + "{FFFFFF} 条未读信息, 请输入 {3FFF00}/showinfos{FFFFFF} 查看");
                    }
                    if (acc.OtherData.Refunds.Count > 0)
                    {
                        MainChat.SendInfoChat(player, "{2d90a9}[服务器] {FFFFFF}您有 {DCFF00}" + acc.OtherData.Refunds.Count + "{FFFFFF} 条未处理账单, 请输入 {3FFF00}/showrefunds{FFFFFF} 处理");
                    }
                }
                await Globals.GlobalEvents.SetPlayerOnline(player, true);

                if (player.Exists)
                    player.SetSyncedMetaData("IsLogin", true);

                MainChat.SendInfoChat(player, "欢迎来到洛圣都角色扮演服务器, 祝您生活和游戏愉快!");

                GlobalEvents.ProgresBar(player, "载入角色中...", 3);
                GlobalEvents.FreezeEntity(player, true);
                player.Health = 200;
                await Task.Delay(2000);
                if (player.Exists)
                {
                    player.Spawn(player.Position, 0);
                    player.Health = 200;
                    player.hp = player.hp;
                    player.Health = (ushort)player.hp;
                }
                await Task.Delay(1000);
                if (player.Exists)
                {
                    player.EmitLocked("player:SetStamina", player.Strength * 2);
                    GlobalEvents.FreezeEntity(player);
                }

                Logger.WriteLogData(Logger.logTypes.ip, "登录 > " + player.characterName);

                if (player.isFinishTut == 100) {  }
                try
                {
                    GlobalEvents.SetPlayerTag(player, "~q~(( 新手教程玩家 ))");
                    switch (player.isFinishTut.ToString())
                    {
                        case "0":
                        {
                            player.isFinishTut = 0;
                            player.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}欢迎来到{fc5e03}洛圣都{FFFFFF}!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}请跟随指引完成新手教程，否则很难游戏!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}教程全程靠地图标记点运行，所以请不要自行设置其他地图标记点!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}请先跟随指引出登机口!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}您在新手教程期间是受保护的!您不需要参与扮演!");
                            GlobalEvents.CheckpointCreate(player, TutorialMain.TutPos[0], 1, 2, new Rgba(255, 0, 0, 0), "Tutorial:Run", "0");
                            break;
                        }
                        case "1": // 出机场 1
                        {
                            player.isFinishTut = 1;
                            // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}欢迎来到{fc5e03}洛圣都{FFFFFF}!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}请跟随指引完成新手教程，否则很难游戏!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}教程全程靠地图标记点运行，所以请不要自行设置其他地图标记点!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}请先跟随指引出登机口!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}您在新手教程期间是受保护的!您不需要参与扮演!");

                            GlobalEvents.CheckpointCreate(player, TutorialMain.TutPos[1], 1, 2, new Rgba(255, 255, 255, 100), "Tutorial:Run", "1");
                            break;
                        }
                        case "2": // 出机场 2 完全
                        {
                            if (player.tutCar == 0)
                            {
                                IVehicle NewV = Alt.CreateVehicle((uint)VehicleModel.Asbo,
                                        new Position(-1055.5f, -2647.38f, 13.3905f), new Rotation(0.00731635f, 0, -0.0341067f));
                                VehModel veh = (VehModel)NewV;
                                veh.sqlID = await Database.DatabaseMain.CreateVehicle((VehModel)NewV);
                                await veh.SetAppearanceDataAsync("2802_BQUAAW8AACAAAIYUAAAPBgzKlYAAAAAAAAAAAAAAAA==");
                                Random pR = new Random();
                                char[] chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
                                veh.NumberplateText = pR.Next(0, 9).ToString() + chars[pR.Next(chars.Length)] +
                                                      chars[pR.Next(chars.Length)] + veh.sqlID.ToString() +
                                                      chars[pR.Next(chars.Length)];
                                veh.PrimaryColorRgb = new Rgba(255, 255, 255, 255);
                                veh.SecondaryColorRgb = new Rgba(255, 255, 255, 255);
                                veh.owner = player.sqlID;
                                veh.defaultTax = 8;
                                veh.fuelConsumption = 2;
                                veh.price = 500;
                                veh.inventoryCapacity = 130;
                                veh.maxFuel = 55;
                                veh.currentFuel = 10;
                                veh.LockState = VehicleLockState.Locked;

                                List<ServerItems> items = new List<ServerItems>();
                                ServerItems item = Items.LSCitems.Find(x => x.ID == 20);
                                item.amount = 1;
                                items.Add(item);
                                string json = JsonConvert.SerializeObject(items);
                                veh.vehInv = json;
                                veh.settings.ModifiyData = veh.AppearanceData;
                                veh.settings.SecurityLevel = 1;
                                veh.Position = new Position(-1055.5f, -2647.38f, 13.3905f);
                                    
                                veh.Update();
                                    
                                player.tutCar = veh.sqlID;
                                player.isFinishTut = 2;
                                await player.updateSql();
                                // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                                MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}我有{fc5e03}车{FFFFFF}了!");
                                MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}为了保证扮演者正常的游戏， 系统为您赠送了一辆车!");
                                MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}请按T输入 /cgps " + veh.sqlID + " 定位您的车辆");
                            }
                            else
                            {
                                player.isFinishTut = 2;
                                await player.updateSql();
                                // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                                MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}我有{fc5e03}车{FFFFFF}了!");
                                MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}为了保证扮演者正常的游戏， 系统为您赠送过一辆车!");
                                MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}请按T输入 /mycars 查看车辆编号并使用 /cgps 车辆编号 定位您的车辆");  
                            }
                            break;
                        }
                        case "3":
                        {
                            player.isFinishTut = 2;
                            // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}我有{fc5e03}车{FFFFFF}了!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}为了保证扮演者正常的游戏， 系统为您赠送过一辆车!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}请按T输入 /mycars 查看车辆编号并使用 /cgps 车辆编号 定位您的车辆"); 
                            break;
                        }
                        case "4": // 找到车 进车 1
                        {
                            player.isFinishTut = 2;
                            // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}我有{fc5e03}车{FFFFFF}了!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}为了保证扮演者正常的游戏， 系统为您赠送过一辆车!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}请按T输入 /mycars 查看车辆编号并使用 /cgps 车辆编号 定位您的车辆"); 
                            break;
                        }    
                        case "5": // 找到车 进车 1
                        {
                            player.isFinishTut = 2;
                            // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}我有{fc5e03}车{FFFFFF}了!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}为了保证扮演者正常的游戏， 系统为您赠送过一辆车!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}请按T输入 /mycars 查看车辆编号并使用 /cgps 车辆编号 定位您的车辆"); 
                            break;
                        }    
                        case "6": // 找到车 进车 1
                        {
                            player.isFinishTut = 2;
                            // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}我有{fc5e03}车{FFFFFF}了!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}为了保证扮演者正常的游戏， 系统为您赠送过一辆车!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}请按T输入 /mycars 查看车辆编号并使用 /cgps 车辆编号 定位您的车辆"); 
                            break;
                        }
                        case "7":
                        {
                            player.isFinishTut = 2;
                            // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}我有{fc5e03}车{FFFFFF}了!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}为了保证扮演者正常的游戏， 系统为您赠送过一辆车!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}请按T输入 /mycars 查看车辆编号并使用 /cgps 车辆编号 定位您的车辆"); 
                            break;
                        }
                        case "8": // 到酒店
                        {
                            player.isFinishTut = 8;
                            // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}我有{fc5e03}车{FFFFFF}了!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}您已到达新的教程点!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}在此之前, 请先去停车!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}系统已经为您定位了附近的停车场!");
                            GlobalEvents.CheckpointCreate(player, TutorialMain.TutPos[3], 1, 2, new Rgba(255, 255, 255, 100), "Tutorial:Run", "8");
                            break;
                        }  
                        case "9": // 到停车场
                        {
                            player.isFinishTut = 9;
                            // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}我有{fc5e03}车{FFFFFF}了!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}您已到达停车场!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}请输入 {fc5e03}/vmenu {FFFFFF}打开车辆菜单!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}您可以在{fc5e03}车辆菜单{FFFFFF}看见{fc5e03}泊车菜单-购买泊车位{FFFFFF}!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}使用上下键选择{fc5e03}泊车菜单-购买泊车位{FFFFFF}, 按回车购买一个泊车位!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: 如果车不见了, 请按T输入 /mycars 查看车辆编号并使用 /cgps 车辆编号 定位您的车辆!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: 如果定位的位置没有您的车, 请输入 /parkmenu 刷出车辆!");                              
                            break;
                        }
                        case "10":
                        {
                            player.isFinishTut = 10;
                            // player.cash += 500;
                            // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}我有{fc5e03}车{FFFFFF}了!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}嘿, 想必这肯定难不倒您!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}您成功购买了停车位， 虽然花钱了， 但是我们已经为您报销了!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}现在请重新输入 {fc5e03}/vmenu{FFFFFF} 选择泊车!");
                            break;
                        }
                        case "11":
                        {
                            player.isFinishTut = 11;
                            // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}我有{fc5e03}车{FFFFFF}了!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}嘿, 想必这肯定难不倒您!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}您成功泊车了，您的车辆会消失进其他世界，关于取车，您可以输入在您的泊车位附近输入{fc5e03}/parkmenu{FFFFFF}取出车辆!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}现在请徒步至地图标记点进入下一个教程!");
                            GlobalEvents.CheckpointCreate(player, TutorialMain.TutPos[4], 1, 2, new Rgba(255, 255, 255, 100), "Tutorial:Run", "11");
                            break;
                        }
                        case "12": // 到酒店
                        {
                            player.isFinishTut = 12;
                            // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}基本指令");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}为了进行扮演，我们增设了许多指令和按键供玩家进行扮演!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}现在请按T {fc5e03}随便说句话");
                            break;
                        }   
                        case "13": // 到酒店
                        {
                            player.isFinishTut = 12;
                            // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}基本指令");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}为了进行扮演，我们增设了许多指令和按键供玩家进行扮演!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}现在请按T {fc5e03}随便说句话");
                            break;
                        }    
                        case "14": // 到酒店
                        {
                            player.isFinishTut = 12;
                            // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}基本指令");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}为了进行扮演，我们增设了许多指令和按键供玩家进行扮演!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}现在请按T {fc5e03}随便说句话");
                            break;
                        }     
                        case "15": // 到酒店
                        {
                            player.isFinishTut = 12;
                            // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}基本指令");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}为了进行扮演，我们增设了许多指令和按键供玩家进行扮演!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}现在请按T {fc5e03}随便说句话");
                            break;
                        }   
                        case "16": // 到酒店
                        {
                            player.isFinishTut = 12;
                            // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}基本指令");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}为了进行扮演，我们增设了许多指令和按键供玩家进行扮演!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}现在请按T {fc5e03}随便说句话");
                            break;
                        }     
                        case "17": // 到酒店
                        {
                            player.isFinishTut = 12;
                            // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}基本指令");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}为了进行扮演，我们增设了许多指令和按键供玩家进行扮演!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}现在请按T {fc5e03}随便说句话");
                            break;
                        }   
                        case "18": // 到酒店
                        {
                            player.isFinishTut = 12;
                            // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}基本指令");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}为了进行扮演，我们增设了许多指令和按键供玩家进行扮演!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}现在请按T {fc5e03}随便说句话");
                            break;
                        }   
                        case "19": // 到酒店
                        {
                            player.isFinishTut = 12;
                            // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}基本指令");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}为了进行扮演，我们增设了许多指令和按键供玩家进行扮演!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}现在请按T {fc5e03}随便说句话");
                            break;
                        }       
                        case "20": // 到酒店
                        {
                            player.isFinishTut = 12;
                            // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}基本指令");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}为了进行扮演，我们增设了许多指令和按键供玩家进行扮演!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}现在请按T {fc5e03}随便说句话");
                            break;
                        }                                
                        case "21": // 下一个点 去加油站
                        {
                            player.isFinishTut = 20;
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}基本指令");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}嘿, 您成功查看了自己的角色外貌特征!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}现在, 让我们前往下一个教程点吧!");
                            GlobalEvents.CheckpointCreate(player, TutorialMain.TutPos[5], 1, 2, new Rgba(255, 255, 255, 100), "Tutorial:Run", "20");
                            break;
                        } 
                        case "22": // 下一个点 去加油站
                        {
                            player.isFinishTut = 20;
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}基本指令");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}嘿, 您成功查看了自己的角色外貌特征!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}现在, 让我们前往下一个教程点吧!");
                            GlobalEvents.CheckpointCreate(player, TutorialMain.TutPos[5], 1, 2, new Rgba(255, 255, 255, 100), "Tutorial:Run", "20");
                            break;
                        }
                        case "23": // 下一个点 去加油站
                        {
                            player.isFinishTut = 23;
                            // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}服装店");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}按E进入服装店, 然后输入 {fc5e03}/buyclothes{FFFFFF} 购买服装!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}注意, WASD和鼠标滚轮是可以浏览角色视角的, WASD用以调整视角和角色方向, 鼠标滚轮用以放大缩小视角");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}注意, {fc5e03}您点击购买会购买当前正在浏览的服装, 是单件服装噢, 请单个单个购买, 不是搭配好了成套购买的!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}注意, {fc5e03}服装完成购买后, 会进入您的背包, 不会被立即穿上, 您需要自己按I打开背包, 右键服装物品进行使用!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}现在, 购买服装吧!");
                            break;
                        }   
                        case "24": // 下一个点 去加油站
                        {
                            player.isFinishTut = 24;
                            // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}服装店");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}嘿, 您成功学会了购买服装!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}服务器是有套装保存系统的, 您可以输入 {fc5e03}/saveoutfit 套装名称{FFFFFF} 保存您当前的服装搭配, 以便下次更换!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}记得, 需要在{fc5e03}服装店入口{fc5e03}噢!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}现在, 试试{fc5e03}/saveoutfit 套装名称{FFFFFF} 吧!");
                            break;
                        }
                        case "25":
                        {
                            player.isFinishTut = 25;
                            // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}服装店");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}嘿, 您成功学会了保存套装!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}现在, 您可以输入 {fc5e03}/outfit{FFFFFF} 浏览和换上您保存的套装!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}现在, 试试{fc5e03}/outfit{FFFFFF} 吧!");
                            break;
                        }
                        case "26":
                        {
                            player.isFinishTut = 26;
                            // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}服装店");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}嘿, 您成功学会了打开套装菜单!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}现在, 请您前往下一个教程点吧!");
                            GlobalEvents.CheckpointCreate(player, TutorialMain.TutPos[7], 1, 2, new Rgba(255, 255, 255, 100), "Tutorial:Run", "26");
                            break;
                        }
                        case "27": // 下一个点 去加油站
                        {
                            player.isFinishTut = 27;
                            // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}理发店");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}进入理发店, 然后输入 {fc5e03}/barber{FFFFFF} 更换角色发型!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}现在, 更换一个发型吧!");
                            break;
                        }       
                        case "28": // 市政府
                        {
                            player.isFinishTut = 28;
                            // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}纳税系统");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}只要您拥有资产, 政府就会对您进行扣税!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}您目前有什么资产? 当然有啊, 是我们赠送您的车辆!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}您需要为您的资产纳税, 如果不纳税, 政府会没收您的资产(如果是车辆, 您的车辆会莫名其妙的消失, 您/cgps也找不到)!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}现在, 请在税务部门输入 {fc5e03}/paytax{FFFFFF} 给您的asbo进行纳税吧!");
                            break;
                        }    
                        
                        case "29": // 市政府
                        {
                            player.isFinishTut = 29;
                            // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}手机系统");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}嘿, 您成功缴纳了税款, 您的车辆暂时不会面对税务问题了!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}现在, 我们需要买一部手机!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}我们已经标记了商店, 这是一个可以购买手机和其他物品的地点!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}服务器分布了许多商店, 但每家商店出售的物品和价格是不同的!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}现在, 前往下一个教程点吧!");
                            GlobalEvents.CheckpointCreate(player, TutorialMain.TutPos[9], 1, 2, new Rgba(255, 255, 255, 100), "Tutorial:Run", "29");
                            break;
                        }                           
                        
                        case "30": // 市政府
                        {
                            player.isFinishTut = 30;
                            // player.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}手机系统");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}嘿, 您已经到达了商店!");
                            MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}现在, 请输入 {fc5e03}/mb{FFFFFF} 打开商店菜单和购买一部手机吧!");
                            break;
                        }
                        default:
                        {
                            player.isFinishTut = 100;
                            break;
                        }
                    }                    
                }
                catch (Exception e)
                {
                    Alt.Log(e.Message);
                    throw;
                }
                return;
            }
            catch (Exception e)
            {
                Alt.Log(JsonConvert.SerializeObject(e));
                Alt.Log(e.Message);
                return;
            }
        }
        [AsyncClientEvent("login:SetupComp")]
        public static async void setupComp(PlayerModel p, string data)
        {
            if (p.Ping > 250)
                return;
            p.charComps = data;
            p.firstLogin = false;
            p.gameTime = 2;
            //p.characterLevel = 1;
            await p.updateSql();
            //p.Position = new Position(-1037.8682f, -2738.334f, 20.164062f);
            Globals.GlobalEvents.GameControls(p, true);
            p.SetDateTime(DateTime.Now.Day - 1, DateTime.Now.Month, DateTime.Now.Year, DateTime.Now.Hour, DateTime.Now.Minute, 5);

            AccountModel acc = await Database.DatabaseMain.getAccInfo(p.accountId);
            p.adminLevel = acc.adminLevel;
            await p.SetDimensionAsync(0);
            //p.Spawn(p.Position, 500);
            if (p.sex == 0)
            {
                await p.SetModelAsync(Alt.Hash("mp_f_freemode_01"));
                var item = Items.LSCitems.Find(x => x.ID == 8); item.data = "0"; item.data2 = "2";
                await Inventory.AddInventoryItem(p, item, 1);
                item = Items.LSCitems.Find(x => x.ID == 11); item.data = "0"; item.data2 = "2";
                await Inventory.AddInventoryItem(p, item, 1);
                item = Items.LSCitems.Find(x => x.ID == 14); item.data = "1"; item.data2 = "2";
                await Inventory.AddInventoryItem(p, item, 1);
                Globals.Commands.MainCommands.COM_Arms(p, new[] { "4" });
            }
            else
            {
                await p.SetModelAsync(Alt.Hash("mp_m_freemode_01"));
                var item = Items.LSCitems.Find(x => x.ID == 8); item.data = "0"; item.data2 = "2";
                await Inventory.AddInventoryItem(p, item, 1);
                item = Items.LSCitems.Find(x => x.ID == 11); item.data = "0"; item.data2 = "1";
                await Inventory.AddInventoryItem(p, item, 1);
                item = Items.LSCitems.Find(x => x.ID == 14); item.data = "1"; item.data2 = "1";
                await Inventory.AddInventoryItem(p, item, 1);
                Globals.Commands.MainCommands.COM_Arms(p, new[] { "0" });
            }
            var items = await Database.DatabaseMain.GetPlayerInventoryItems(p.sqlID);
            string json = JsonConvert.SerializeObject(items);
            p.EmitLocked("inventory:Update", json);
            await Task.Delay(3000);
            p.Spawn(new Position(-1037.87f, -2738.33f, 20.1641f), 0);
            p.hp = 1000;
            p.EmitLocked("character:ServerSync", p.charComps);

            MainChat.SendInfoChat(p, "欢迎您加入洛圣都角色扮演! 请先按 I 打开背包 穿上初始服装");
            
            await Task.Delay(3000);
            p.Kick("首次登录, 需要重新进入游戏");
            return;
        }
        [AsyncClientEvent("characterPage:CreateChar")]
        public static async Task CreateChar(PlayerModel p, string name)
        {
            
            if (p.Ping > 250)
                return;
            try
            {
                if (Regex.IsMatch(name, _pattern) == false) { Globals.GlobalEvents.notify(p, 3, "角色名称错误, 应是Xxx_Xxx的格式!"); return; }
                if (name.Length <= 5) { Globals.GlobalEvents.notify(p, 3, "角色姓名错误!"); return; }
                p.Position = new Position(-74f, -819f, 326f);
                p.characterName = name.Replace(" ", "_");
                if (await Database.DatabaseMain.CheckCharacterName(p.characterName)) { Globals.GlobalEvents.notify(p, 3, "角色姓名已存在!"); return; }
                if (!p.HasData("AccountId"))
                    p.Kick("操作超时");
                int charid = await Database.DatabaseMain.CreateCharacter(p, p.lscGetdata<int>("AccountId"));
                p.EmitLocked("characterPage:Valid", p.characterName);
                await p.Setup(charid);
                p.Dimension = (int)p.Id;
                await Globals.GlobalEvents.SetPlayerOnline(p, true);
                p.Position = new Position(-74f, -819f, 326f);
                return;
            }
            catch { return; }
        }


        [AsyncClientEvent("characterPage:SettingChar")]
        public static async Task CharacterSettings(PlayerModel p, string data)
        {
            if (p.Ping > 250)
                return;
            try
            {
                Globals.GlobalEvents.CloseCamera(p);
                Models.CharacterSettings setting = JsonConvert.DeserializeObject<Models.CharacterSettings>(data);
                p.settings = data;
                p.characterAge = setting.age;
                if (p.cash <= 3000)
                {
                    p.cash = 3000;
                }
                p.Position = new Position(-74f, -819f, 326f);
                await p.updateSql();
                p.EmitLocked("character:Edit");
            }
            catch { }
        }


        public static async void takeWeaponWithDelay(PlayerModel p, InventoryModel i)
        {
            await Task.Delay(5000);
            if (!p.Exists)
                return;
            await OtherSystem.LSCsystems.WeaponSystem.TakeWeapon(p, i);
        }

    }
}
