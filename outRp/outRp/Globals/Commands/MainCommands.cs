using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using AltV.Net.Resources.Chat.Api;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Core;
using outRp.Models;
using outRp.OtherSystem;
using outRp.OtherSystem.NativeUi;
using outRp.OtherSystem.Textlabels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using outRp.OtherSystem.LSCsystems;
using outRp.Tutorial;

namespace outRp.Globals.Commands
{
    public class MainCommands : IScript
    {

        /*[Command("snowball")]
        public void COM_Kartopu(PlayerModel p)
        {
            p.SetSyncedMetaData("PWepBullet_" + Alt.Hash("weapon_snowball"), true);
            await p.GiveWeaponAsync(Alt.Hash("weapon_snowball"), 5, true);
            MainChat.SendInfoChat(p, "你从地上揉了团雪球.");
        }*/

        //[Command("lsc2022")]
        //public static void COM_lsc222(PlayerModel p, params string[] args)
        //{
        //    MainChat.SendInfoChat(p, "Yeni yıl kaydınız başarıyla yapıldı.", true);
        //    Core.Logger.WriteLogData(Logger.logTypes.yeniyil, p.characterName);
        //}

        #region Chat Commands
        [Command(CONSTANT.COM_EmoteMe)]
        public void COM_EmoteMe(PlayerModel player, params string[] messages)
        {
            var message = string.Join(" ", messages);
            if (message.Length <= 0 || message == string.Empty || message == "")
            {
                MainChat.SendErrorChat(player, "内容不能为空");
                return;
            }
            MainChat.EmoteMe(player, message);
            
            if (player.isFinishTut == 13)
            {
                player.isFinishTut = 14;
                player.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}基本指令");
                MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}嘿, 您成功使用了第一人称动作指令!");
                MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}第一人称动作指令同时也有小声版, 您可以使用/melow!");
                MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}现在, 请尝试一下{fc5e03}/do{FFFFFF} 角色第三人称环境描述指令!");
                MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}例如用法, /do 你可以看到我的呼吸很紧促");
                // GlobalEvents.CheckpointCreate(player, Tutorial_Main.TutPos[1], 44, 1, new Rgba(255, 255, 255, 100), "Tutorial:Run", "11");
            }             
        }

        [Command("melow")]
        public void COM_EmoteLow(PlayerModel p, params string[] _args)
        {
            var args = string.Join(" ", _args);
            if (args.Length <= 0 || args == string.Empty || args == "")
            {
                MainChat.SendErrorChat(p, "内容不能为空");
                return;
            }

            MainChat.EmoteMeLow(p, args);
        }

        [Command("dolow", greedyArg: true)]
        public void COM_EmoteLowDo(PlayerModel p, string message)
        {
            if (message.Length <= 0)
                return;
            MainChat.EmoteDoLow(p, message);
        }

        [Command(CONSTANT.COM_EmoteDo, greedyArg: true)]
        public void COM_EmoteDo(PlayerModel player, string message)
        {
            if (message.Length <= 0)
                return;
            MainChat.EmoteDo(player, message);
            
            if (player.isFinishTut == 14)
            {
                player.isFinishTut = 15;
                player.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}基本指令");
                MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}嘿, 您成功使用了第三人称动作指令!");
                MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}第三人称动作指令同时也有小声版, 您可以使用/dolow!");
                MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}现在, 请尝试一下{fc5e03}/try{FFFFFF} 第一人称尝试动作指令!");
                MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}例如用法, /try 破开面前的一扇门");
                // GlobalEvents.CheckpointCreate(player, Tutorial_Main.TutPos[1], 44, 1, new Rgba(255, 255, 255, 100), "Tutorial:Run", "11");
            } 
        }

        [Command(CONSTANT.COM_Shout, greedyArg: true)]
        public void COM_Shout(PlayerModel player, string mesaj)
        {
            MainChat.ShoutChat(player, mesaj);
        }

        [Command(CONSTANT.COM_LowVoice, greedyArg: true)]
        public void COM_LowVoice(PlayerModel player, string mesaj)
        {
            MainChat.LowVoiceChat(player, mesaj);
        }

        [Command(CONSTANT.COM_Whisper)]
        public void COM_Whiper(PlayerModel player, int ID, params string[] messages)
        {
            var message = string.Join(" ", messages);
            MainChat.WhisperChat(player, ID, message);
        }

        [Command(CONSTANT.COM_PM)]
        public void COM_Pm(PlayerModel player, int id, params string[] messages)
        {
            if (id == player.sqlID) { MainChat.SendErrorChat(player, "[错误] 无法对自己使用."); return; }
            if (id == 0) { MainChat.SendInfoChat(player, "[用法] /pm [id] [消息]"); return; }
            if (messages.Length <= 0) { MainChat.SendInfoChat(player, "[用法] /pm [id] [消息]"); return; }
            var message = string.Join(" ", messages);
            MainChat.PMChat(player, id, message);
            if (!player.isPM)
            {
                MainChat.SendInfoChat(player, "[?] 对方没有开启OOC私信.");
            }
        }

        [Command("re")]
        public void COM_PMLast(PlayerModel p, params string[] messages)
        {
            if (messages.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /re [回复内容]"); return; }
            string message = string.Join(" ", messages);
            MainChat.PMChat(p, p.LastPm, message);
        }

        [Command(CONSTANT.COM_VehicleChat)]
        public void COM_VehicleChat(PlayerModel player, params string[] messages)
        {
            if (messages.Length <= 0) { MainChat.SendInfoChat(player, CONSTANT.DESC_VehicleChat); return; }
            string msg = string.Join(" ", messages);
            MainChat.InCarChat(player, msg);
        }

        [Command(CONSTANT.COM_OOC)]
        public void COM_OOCChat(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendErrorChat(p, "[用法] /b [OOC本地消息]"); return; }
            string message = string.Join(" ", args);
            MainChat.OOCChat(p, message);
            
            if (p.isFinishTut == 16)
            {
                p.isFinishTut = 17;
                p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}基本指令");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}嘿, 您成功使用了本地OOC指令!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}如果您想扔硬币, 或者掷骰子来做些决定, 您可以使用{fc5e03}/coin{FFFFFF} 或者{fc5e03}/dice{FFFFFF} 指令!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}现在, 尝试一下其中任意指令吧!");
                // GlobalEvents.CheckpointCreate(player, Tutorial_Main.TutPos[1], 44, 1, new Rgba(255, 255, 255, 100), "Tutorial:Run", "11");
            }                
        }
        #endregion

        [Command(CONSTANT.COM_FlipCoin)]
        public void COM_FlipCoin(PlayerModel player)
        {
            Random result = new Random();
            int coin = result.Next(0, 2);
            string r;
            if (coin == 0) { r = "人头面"; } else { r = "徽标面"; }
            MainChat.EmoteMe(player, " 将手伸进口袋掏出一枚硬币并扔到空中.");
            MainChat.EmoteDoAlternative(player, "可以看到" + r + "朝上");
            if (player.isFinishTut == 17)
            {
                player.isFinishTut = 18;
                player.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}基本指令");
                MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}嘿, 您成功使用了扔硬币指令!");
                MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}现在, 让我们设置一下角色外貌描述吧!");
                MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}现在, 请尝试使用 {fc5e03}/appce{FFFFFF} 指令!");
                MainChat.SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}例如用法, /appce 大概1.8m的身高, 留着很明显的络腮胡....");
                // GlobalEvents.CheckpointCreate(player, Tutorial_Main.TutPos[1], 44, 1, new Rgba(255, 255, 255, 100), "Tutorial:Run", "11");
            }
            return;
        }

        [Command(CONSTANT.COM_GiveMoney)]
        public async Task COM_GiveMoney(PlayerModel player, params string[] values)
        {
            if (player.Ping > 250)
                return;
            if (values.Length <= 0) { MainChat.SendInfoChat(player, CONSTANT.DESC_GiveMoney); return; }
            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(Int32.Parse(values[0].ToString()));
            if (target == null) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotFound); return; }
            if (target.Position.Distance(player.Position) > 3f) { MainChat.SendErrorChat(player, CONSTANT.ERR_NotNearTarget); return; }
            if (player.Dimension != target.Dimension) { MainChat.SendErrorChat(player, CONSTANT.ERR_NotNearTarget); return; }
            int miktar = Int32.Parse(values[1].ToString());
            if (miktar <= 0) { MainChat.SendErrorChat(player, CONSTANT.ERR_ValueNotNegative); return; }
            if (player.cash <= miktar) { MainChat.SendErrorChat(player, CONSTANT.ERR_MoneyNotEnought); return; }
            if (player == target) { MainChat.SendErrorChat(player, CONSTANT.ERR_NotUseOnYou); return; }
            if (miktar >= 50000)
            {
                MainChat.SendAdminChat("大额 GiveMoeny 提示: " + player.characterName + " -> " + target.characterName + " | 金额: $" + miktar);
            }

            target.cash += miktar;
            player.cash -= miktar;

            await target.updateSql();
            await player.updateSql();

            string emote = string.Format(ServerEmotes.EMOTE_GiveMoney, target.fakeName.Replace("_", " "));
            MainChat.EmoteMe(player, emote);

            player.SendChatMessage(("{2E9C0B}[!]" + target.characterName.Replace("_", " ") + " 收到了您的 $" + miktar + " (( 适当扮演更宜扮演氛围噢 ))"));
            target.SendChatMessage("{2E9C0B}[!]" + player.characterName.Replace("_", " ") + " 给了您 $" + miktar + " (( 适当扮演更宜扮演氛围噢 ))");

            Core.Logger.WriteLogData(Logger.logTypes.moneyTransfer, player.characterName + " 给 > " + target.characterName + " | 金额: " + miktar.ToString());
            return;
        }

        [Command(CONSTANT.COM_ShowNearPlates)]
        public void COM_ShowNearPlates(PlayerModel p)
        {
            p.SendChatMessage("---[附近车辆信息]---");
            foreach (VehModel v in Alt.GetAllVehicles())
            {
                if (v.Position.Distance(p.Position) < 20f)
                {
                    var ModelName = (VehicleModel)v.Model;
                    p.SendChatMessage("模型" + ModelName.ToString() + " 车牌: " + v.NumberplateText);
                }
            }
            return;
        }

        [Command(CONSTANT.COM_Id)]
        public void COM_Id(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, CONSTANT.DESC_Id); return; }
            int intStr; bool intResultTryParse = int.TryParse(args[0], out intStr);

            bool findOk = false;

            if (!intResultTryParse)
            {

                foreach (PlayerModel t in Alt.GetAllPlayers())
                {
                    if (t.characterName.ToLower().Contains(args[0].ToLower())) { p.SendChatMessage(t.sqlID.ToString() + " " + t.characterName.Replace("_", " ")); findOk = true; }
                }
            }
            else
            {
                foreach (PlayerModel a in Alt.GetAllPlayers())
                {
                    if (a.sqlID == intStr) { p.SendChatMessage(a.sqlID.ToString() + " " + a.characterName.Replace("_", " ")); findOk = true; }
                }
            }

            if (!findOk) { MainChat.SendErrorChat(p, "无效玩家."); }
        }

        [Command(CONSTANT.COM_DateTime)]
        public void COM_DateTime(PlayerModel p)
        {
            MainChat.EmoteMe(p, " 看了眼时间.");
            p.SendChatMessage("时间: " + DateTime.Now.ToString("HH:mm:ss"));
            if (p.jailTime > 0) { p.SendChatMessage("监狱时间: " + p.jailTime.ToString() + " 分钟"); }
            return;
        }

        [Command(CONSTANT.COM_Dice6)]
        public void COM_Dice6(PlayerModel p)
        {
            Random result = new Random();
            int coin = result.Next(1, 7);
            MainChat.EmoteMe(p, " 晃了晃手中的骰子并松开.");
            MainChat.EmoteDoAlternative(p, "结果 - " + coin.ToString());
            
            if (p.isFinishTut == 17)
            {
                p.isFinishTut = 18;
                p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}基本指令");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}嘿, 您成功使用了扔硬币指令!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}现在, 让我们设置一下角色外貌描述吧!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}现在, 请尝试使用 {fc5e03}/appce{FFFFFF} 指令!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}例如用法, /appce 大概1.8m的身高, 留着很明显的络腮胡....");
                // GlobalEvents.CheckpointCreate(player, Tutorial_Main.TutPos[1], 44, 1, new Rgba(255, 255, 255, 100), "Tutorial:Run", "11");
            }
            return;
        }

        [Command("dice12")]
        public void COM_Dice12(PlayerModel p)
        {
            Random result = new Random();
            int coin = result.Next(1, 7);
            MainChat.EmoteMe(p, " 晃了晃手中的骰子并松开.");
            MainChat.EmoteDoAlternative(p, "结果 - " + coin.ToString() + " | " + result.Next(1, 7).ToString());
            return;
        }

        [Command("dice50")]
        public void COM_Dice50(PlayerModel p)
        {
            Random result = new Random();
            int coin = result.Next(1, 50);
            MainChat.EmoteMe(p, " 晃了晃手中的骰子并松开.");
            MainChat.EmoteDoAlternative(p, "结果 - " + coin.ToString() + " | " + result.Next(1, 7).ToString());
            return;
        }

        [Command("dice100")]
        public void COM_Dice100(PlayerModel p, params string[] args)
        {
            Random result = new Random();
            int coin = result.Next(1, 100);
            MainChat.EmoteMe(p, " 晃了晃手中的骰子并松开.");
            MainChat.EmoteDoAlternative(p, "结果 - " + coin.ToString() + " | " + result.Next(1, 7).ToString());
            return;
        }

        [Command("spin")]
        public async void COM_Rulet(PlayerModel p)
        {
            Random result = new Random();
            int coin = result.Next(0, 37);
            coin = result.Next(0, 37);
            MainChat.EmoteMe(p, " 慢慢地将球放入轮盘赌.");
            MainChat.EmoteDoAlternative(p, "球开始在轮盘上旋转.");
            await Task.Delay(5000);
            if (!p.Exists)
                return;

            MainChat.EmoteDoAlternative(p, "轮盘的球停在了槽: " + coin.ToString());
            return;
        }

        public static bool Key_BuyClothes(PlayerModel player)
        {
            Position x = OtherSystem.ClothingShop.clothingShop;
            if (player.Position.Distance(x) > 12f) return false;
            if (player.HasData("Combine:Equip")) return false;
            MainChat.SendInfoChat(player, "如果试穿衣服出现穿模的情况, 您可以输入 '/torso 0-100' 调整手臂模型");
            MainChat.SendInfoChat(player, "按 W A S D 可调整视角");
            player.EmitLocked("ClothingMenu:Show");
            GlobalEvents.notify(player, 7, "正在加载服装.");
            GlobalEvents.FreezePlayerClothes(player, true);
            return true;
        }
        
        [Command(CONSTANT.COM_BuyClothes)]
        public void COM_BuyClothes(PlayerModel player)
        {
            Position x = OtherSystem.ClothingShop.clothingShop;
            if (player.Position.Distance(x) > 12f) { MainChat.SendErrorChat(player, CONSTANT.ERR_NotNearClothesShop); return; }
            if (player.HasData("Combine:Equip")) { MainChat.SendErrorChat(player, "[错误] 无法穿着此身衣服购买衣服!"); return; }
            MainChat.SendInfoChat(player, "如果试穿衣服出现穿模的情况, 您可以输入 '/torso 0-100' 调整手臂模型");
            MainChat.SendInfoChat(player, "按 W A S D 可调整视角");
            player.EmitLocked("ClothingMenu:Show");
            GlobalEvents.notify(player, 7, "正在加载服装.");
            GlobalEvents.FreezePlayerClothes(player, true);
        }
        
        [AsyncClientEvent("Wheel:myStats")]
        public async Task WheelShowStats(PlayerModel p)
        {

            if (p.Name == "未登录玩家")
                return;
            
            string sex = "";
            if (p.sex == 0) { sex = "女性"; } else { sex = "男性"; }
            
            string healtStatus = "健康";
            if (p.injured.Injured) { healtStatus = "负伤"; }
            if (p.injured.isDead) { healtStatus = "重伤/死亡"; }

            int paydayLeft = 0;
            paydayLeft = 60 - p.gameTime;

            AccountModel acc = await Database.DatabaseMain.getAccInfo(p.accountId);
            p.EmitLocked("StatPage:Show", p.characterName.Replace("_", " "), acc.forumName, p.sqlID, sex, p.characterExp, p.characterLevel, paydayLeft, healtStatus, p.Strength.ToString());
            
            FactionModel fact = new FactionModel();
            if (p.factionId > 0)
            {
                fact = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            }
            string factName = "";
            string factRank = "";
            if (fact != null)
            {
                factName = fact.name;
                factRank = Factions.Faction.GetFactionRankString(p.factionRank, fact);
            }
            else { factName = "无"; factRank = "无"; }

            p.EmitLocked("StatPage:LoadFaction", p.factionId, factName, factRank);
            
            string bizinfo = "";
            if (p.businessStaff != 0)
            {
                BusinessModel biz = await Database.DatabaseMain.GetBusinessInfo(p.businessStaff);
                string bizOwner = "雇员";
                if (biz.ownerId == p.sqlID) { bizOwner = "业主"; }
                bizinfo = "产业: " + biz.name + " | 身份: " + bizOwner;
            }
            
            p.EmitLocked("StatPage:LoadBiz", bizinfo);
        }
        
        [Command(CONSTANT.COM_CharacterStatus)]
        public async Task COM_GetPlayerStats(PlayerModel p)
        {

            if (p.Name == "未登录玩家")
                return;
            
            string sex = "";
            if (p.sex == 0) { sex = "女性"; } else { sex = "男性"; }
            
            string healtStatus = "健康";
            if (p.injured.Injured) { healtStatus = "负伤"; }
            if (p.injured.isDead) { healtStatus = "重伤/死亡"; }

            int paydayLeft = 0;
            paydayLeft = 60 - p.gameTime;

            AccountModel acc = await Database.DatabaseMain.getAccInfo(p.accountId);
            p.EmitLocked("StatPage:Show", p.characterName.Replace("_", " "), acc.forumName, p.sqlID, sex, p.characterExp, p.characterLevel, paydayLeft, healtStatus, p.Strength.ToString());
            
            FactionModel fact = new FactionModel();
            if (p.factionId > 0)
            {
                fact = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            }
            string factName = "";
            string factRank = "";
            if (fact != null)
            {
                factName = fact.name;
                factRank = Factions.Faction.GetFactionRankString(p.factionRank, fact);
            }
            else { factName = "无"; factRank = "无"; }

            p.EmitLocked("StatPage:LoadFaction", p.factionId, factName, factRank);
            
            string bizinfo = "";
            if (p.businessStaff != 0)
            {
                BusinessModel biz = await Database.DatabaseMain.GetBusinessInfo(p.businessStaff);
                string bizOwner = "雇员";
                if (biz.ownerId == p.sqlID) { bizOwner = "业主"; }
                bizinfo = "产业: " + biz.name + " | 身份: " + bizOwner;
            }
            
            p.EmitLocked("StatPage:LoadBiz", bizinfo);
        }

        [Command(CONSTANT.COM_ShowBusiness)]
        public async Task COM_ShowBusiness(PlayerModel p)
        {
            string nullBiz = "无";
            List<BusinessModel> bizs = await Database.DatabaseMain.GetMemberBusinessList(p);
            p.SendChatMessage("{7EB278}________________[" + p.characterName.Replace("_", " ") + " 的产业]________________");
            if (bizs == null) { p.SendChatMessage("{C8C8C8}>> " + nullBiz); }
            else
            {
                foreach (BusinessModel x in bizs)
                {
                    p.SendChatMessage("{C8C8C8}>> 名称: " + x.name + " 唯一ID: " + x.ID + " 价格: $" + x.price);
                    p.SendChatMessage("{C8C8C8}>> 收入: $" + x.vault);
                    p.SendChatMessage("{C8C8C8}>> 总计税收: $" + x.settings.TotalTax + " (注意!如果 '总计税收 大于 产业价格' 将面临被银行征收的风险");
                }
            }
            p.SendChatMessage("{7EB278}________________[" + p.characterName.Replace("_", " ") + " 的产业]________________");
        }

        [Command(CONSTANT.COM_WalkingStyle)]
        public void COM_SetWalkingStyle(PlayerModel p)
        {
            /*
            GuiMenu walk1 = new GuiMenu { name = "Elinde Çanta Var", triger = "SetWalkingStyle", value = "ANIM_GROUP_MOVE_BALLISTIC" };
            GuiMenu walk2 = new GuiMenu { name = "Normal Yürüyüş", triger = "SetWalkingStyle", value = "ANIM_GROUP_MOVE_LEMAR_ALLEY" };
            GuiMenu walk3 = new GuiMenu { name = "Çanta2", triger = "SetWalkingStyle", value = "clipset@move@trash_fast_turn" };
            GuiMenu walk4 = new GuiMenu { name = "Kadın Yürüyüşü", triger = "SetWalkingStyle", value = "FEMALE_FAST_RUNNER" };
            GuiMenu walk5 = new GuiMenu { name = "Çanta3", triger = "SetWalkingStyle", value = "missfbi4prepp1_garbageman" };
            GuiMenu walk6 = new GuiMenu { name = "Normal Yürüyüş 1", triger = "SetWalkingStyle", value = "move_characters@franklin@fire" };
            GuiMenu walk7 = new GuiMenu { name = "Normal Yürüyüş 2", triger = "SetWalkingStyle", value = "move_characters@Jimmy@slow@" };
            GuiMenu walk8 = new GuiMenu { name = "Normal Yürüyüş 3", triger = "SetWalkingStyle", value = "move_characters@michael@fire" };
            GuiMenu walk9 = new GuiMenu { name = "Ürkek Yürüyüş", triger = "SetWalkingStyle", value = "move_f@scared" };
            GuiMenu walk10 = new GuiMenu { name = "Kadın Yürüyüş 2", triger = "SetWalkingStyle", value = "move_f@sexy@a" };
            GuiMenu walk11 = new GuiMenu { name = "Topal Yürüyüşü", triger = "SetWalkingStyle", value = "move_heist_lester" };
            GuiMenu walk12 = new GuiMenu { name = "Yaralı Yürüyüşü", triger = "SetWalkingStyle", value = "move_injured_generic" };
            GuiMenu walk13 = new GuiMenu { name = "Normal Yürüyüş 4", triger = "SetWalkingStyle", value = "MOVE_M@BAIL_BOND_NOT_TAZERED" };
            GuiMenu walk14 = new GuiMenu { name = "Sersem Yürüyüş", triger = "SetWalkingStyle", value = "MOVE_M@BAIL_BOND_TAZERED" };
            GuiMenu walk15 = new GuiMenu { name = "Normal Yürüyüş 5", triger = "SetWalkingStyle", value = "move_m@brave" };
            GuiMenu walk16 = new GuiMenu { name = "Normal Yürüyüş 6", triger = "SetWalkingStyle", value = "move_m@casual@d" };
            GuiMenu walk17 = new GuiMenu { name = "Normal Yürüyüş 7", triger = "SetWalkingStyle", value = "move_m@fire" };
            GuiMenu walk18 = new GuiMenu { name = "Gangster 1", triger = "SetWalkingStyle", value = "move_m@gangster@var_e" };
            GuiMenu walk19 = new GuiMenu { name = "Utangaç Yürüyüşü", triger = "SetWalkingStyle", value = "move_m@gangster@var_f" };
            GuiMenu walk20 = new GuiMenu { name = "Gangster 2", triger = "SetWalkingStyle", value = "move_m@gangster@var_i" };
            GuiMenu walk21 = new GuiMenu { name = "Normal Yürüyüş 8", triger = "SetWalkingStyle", value = "MOVE_P_M_ONE" };
            GuiMenu walk22 = new GuiMenu { name = "Normal Yürüyüş 9", triger = "SetWalkingStyle", value = "move_p_m_zero_slow" };
            GuiMenu walk23 = new GuiMenu { name = "Normal Yürüyüş 10", triger = "SetWalkingStyle", value = "move_ped_bucket" };
            GuiMenu walk24 = new GuiMenu { name = "Normal Yürüyüş 11", triger = "SetWalkingStyle", value = "MOVE_M@FEMME@" };
            GuiMenu walk25 = new GuiMenu { name = "Normal Yürüyüş 12", triger = "SetWalkingStyle", value = "MOVE_F@FEMME@" };
            GuiMenu walk26 = new GuiMenu { name = "Havalı Yürüyüş", triger = "SetWalkingStyle", value = "MOVE_M@GANGSTER@NG" };
            GuiMenu walk27 = new GuiMenu { name = "Normal Yürüyüş 13", triger = "SetWalkingStyle", value = "MOVE_F@GANGSTER@NG" };
            GuiMenu walk28 = new GuiMenu { name = "Sert Yürüyüş", triger = "SetWalkingStyle", value = "MOVE_M@TOUGH_GUY@" };*/

            GuiMenu walk1 = new GuiMenu { name = "太空漫步", triger = "SetWalkingStyle", value = "move_m@alien" };
            GuiMenu walk2 = new GuiMenu { name = "装甲战士", triger = "SetWalkingStyle", value = "anim_group_move_ballistic" };
            GuiMenu walk3 = new GuiMenu { name = "傲慢", triger = "SetWalkingStyle", value = "move_f@arrogant@a" };
            GuiMenu walk4 = new GuiMenu { name = "勇敢", triger = "SetWalkingStyle", value = "move_m@brave" };
            GuiMenu walk5 = new GuiMenu { name = "普通 1", triger = "SetWalkingStyle", value = "move_m@casual@a" };
            GuiMenu walk6 = new GuiMenu { name = "普通 2", triger = "SetWalkingStyle", value = "move_m@casual@b" };
            GuiMenu walk7 = new GuiMenu { name = "普通 3", triger = "SetWalkingStyle", value = "move_m@casual@c" };
            GuiMenu walk8 = new GuiMenu { name = "普通 4", triger = "SetWalkingStyle", value = "move_m@casual@d" };
            GuiMenu walk9 = new GuiMenu { name = "普通 5", triger = "SetWalkingStyle", value = "move_m@casual@e" };
            GuiMenu walk10 = new GuiMenu { name = "普通 6", triger = "SetWalkingStyle", value = "move_m@casual@f" };
            GuiMenu walk11 = new GuiMenu { name = "娇艳", triger = "SetWalkingStyle", value = "move_f@chichi" };
            GuiMenu walk12 = new GuiMenu { name = "自信", triger = "SetWalkingStyle", value = "move_m@confident" };
            GuiMenu walk13 = new GuiMenu { name = "警用 1", triger = "SetWalkingStyle", value = "move_m@business@a" };
            GuiMenu walk14 = new GuiMenu { name = "警用 2", triger = "SetWalkingStyle", value = "move_m@business@b" };
            GuiMenu walk15 = new GuiMenu { name = "警用 3", triger = "SetWalkingStyle", value = "move_m@business@c" };
            GuiMenu walk16 = new GuiMenu { name = "(普通) 女性", triger = "SetWalkingStyle", value = "move_f@multiplayer" };
            GuiMenu walk17 = new GuiMenu { name = "(普通) 男性", triger = "SetWalkingStyle", value = "move_m@multiplayer" };
            GuiMenu walk18 = new GuiMenu { name = "醉酒 1", triger = "SetWalkingStyle", value = "move_m@drunk@a" };
            GuiMenu walk19 = new GuiMenu { name = "醉酒 2", triger = "SetWalkingStyle", value = "move_m@drunk@slightlydrunk" };
            GuiMenu walk20 = new GuiMenu { name = "醉酒 3", triger = "SetWalkingStyle", value = "move_m@buzzed" };
            GuiMenu walk21 = new GuiMenu { name = "醉酒 4", triger = "SetWalkingStyle", value = "move_m@drunk@verydrunk" };
            GuiMenu walk22 = new GuiMenu { name = "魅力", triger = "SetWalkingStyle", value = "move_f@femme@" };
            GuiMenu walk23 = new GuiMenu { name = "硬汉 1", triger = "SetWalkingStyle", value = "move_characters@franklin@fire" };
            GuiMenu walk24 = new GuiMenu { name = "硬汉 2", triger = "SetWalkingStyle", value = "move_characters@michael@fire" };
            GuiMenu walk25 = new GuiMenu { name = "硬汉 3", triger = "SetWalkingStyle", value = "MOVE_F@FEMME@" };
            GuiMenu walk26 = new GuiMenu { name = "硬汉 4", triger = "SetWalkingStyle", value = "move_m@fire" };
            GuiMenu walk27 = new GuiMenu { name = "焦急", triger = "SetWalkingStyle", value = "move_f@flee@a" };
            GuiMenu walk28 = new GuiMenu { name = "富兰克林", triger = "SetWalkingStyle", value = "move_p_m_one" };

            // ----
            GuiMenu walk29 = new GuiMenu { name = "帮派成员 1", triger = "SetWalkingStyle", value = "move_m@gangster@generic" };
            GuiMenu walk30 = new GuiMenu { name = "帮派成员 2", triger = "SetWalkingStyle", value = "move_m@gangster@ng" };
            GuiMenu walk31 = new GuiMenu { name = "帮派成员 3", triger = "SetWalkingStyle", value = "move_m@gangster@var_e" };
            GuiMenu walk32 = new GuiMenu { name = "帮派成员 4", triger = "SetWalkingStyle", value = "move_m@gangster@var_f" };
            GuiMenu walk33 = new GuiMenu { name = "帮派成员 5", triger = "SetWalkingStyle", value = "move_m@gangster@var_i" };
            GuiMenu walk34 = new GuiMenu { name = "悠哉", triger = "SetWalkingStyle", value = "anim@move_m@grooving@" };
            GuiMenu walk35 = new GuiMenu { name = "持枪", triger = "SetWalkingStyle", value = "move_m@prison_gaurd" };
            GuiMenu walk36 = new GuiMenu { name = "被手铐铐上", triger = "SetWalkingStyle", value = "move_m@prisoner_cuffed" };
            GuiMenu walk37 = new GuiMenu { name = "疲惫 1", triger = "SetWalkingStyle", value = "move_f@heels@c" };
            GuiMenu walk38 = new GuiMenu { name = "疲惫 2", triger = "SetWalkingStyle", value = "move_f@heels@d" };
            GuiMenu walk39 = new GuiMenu { name = "旅行", triger = "SetWalkingStyle", value = "move_m@hiking" };
            GuiMenu walk40 = new GuiMenu { name = "潮人", triger = "SetWalkingStyle", value = "move_m@hipster@a" };
            GuiMenu walk41 = new GuiMenu { name = "流浪汉", triger = "SetWalkingStyle", value = "move_m@hobo@a" };
            GuiMenu walk42 = new GuiMenu { name = "焦急 2", triger = "SetWalkingStyle", value = "move_f@hurry@a" };
            GuiMenu walk43 = new GuiMenu { name = "保镖 1", triger = "SetWalkingStyle", value = "move_p_m_zero_janitor" };
            GuiMenu walk44 = new GuiMenu { name = "保镖 2", triger = "SetWalkingStyle", value = "move_p_m_zero_slow" };
            GuiMenu walk45 = new GuiMenu { name = "保镖 3", triger = "SetWalkingStyle", value = "move_m@jog@" };
            GuiMenu walk46 = new GuiMenu { name = "拉玛", triger = "SetWalkingStyle", value = "anim_group_move_lemar_alley" };
            GuiMenu walk47 = new GuiMenu { name = "莱斯特 1", triger = "SetWalkingStyle", value = "move_heist_lester" };
            GuiMenu walk48 = new GuiMenu { name = "莱斯特 2", triger = "SetWalkingStyle", value = "move_lester_caneup" };
            GuiMenu walk49 = new GuiMenu { name = "僵尸", triger = "SetWalkingStyle", value = "move_f@maneater" };
            GuiMenu walk50 = new GuiMenu { name = "麦克", triger = "SetWalkingStyle", value = "move_ped_bucket" };
            GuiMenu walk51 = new GuiMenu { name = "富豪", triger = "SetWalkingStyle", value = "move_m@money" };
            GuiMenu walk52 = new GuiMenu { name = "健壮", triger = "SetWalkingStyle", value = "move_m@muscle@a" };
            GuiMenu walk53 = new GuiMenu { name = "酷男 1", triger = "SetWalkingStyle", value = "move_m@posh@" };
            GuiMenu walk54 = new GuiMenu { name = "酷女 2", triger = "SetWalkingStyle", value = "move_f@posh@" };
            GuiMenu walk55 = new GuiMenu { name = "迅捷", triger = "SetWalkingStyle", value = "move_m@quick" };
            GuiMenu walk56 = new GuiMenu { name = "跑步", triger = "SetWalkingStyle", value = "female_fast_runner" };
            GuiMenu walk57 = new GuiMenu { name = "难过", triger = "SetWalkingStyle", value = "move_m@sad@a" };
            GuiMenu walk58 = new GuiMenu { name = "厚颜无耻男 1", triger = "SetWalkingStyle", value = "move_m@sassy" };
            GuiMenu walk59 = new GuiMenu { name = "厚颜无耻女 2", triger = "SetWalkingStyle", value = "move_f@sassy" };
            GuiMenu walk60 = new GuiMenu { name = "受惊", triger = "SetWalkingStyle", value = "move_f@scared" };
            GuiMenu walk61 = new GuiMenu { name = "性感", triger = "SetWalkingStyle", value = "move_f@sexy@a" };
            GuiMenu walk62 = new GuiMenu { name = "小偷", triger = "SetWalkingStyle", value = "move_m@shadyped@a" };
            GuiMenu walk63 = new GuiMenu { name = "慢步", triger = "SetWalkingStyle", value = "move_characters@jimmy@slow@" };
            GuiMenu walk64 = new GuiMenu { name = "昂首阔步", triger = "SetWalkingStyle", value = "move_m@swagge" };
            GuiMenu walk65 = new GuiMenu { name = "暴徒 1", triger = "SetWalkingStyle", value = "move_m@tough_guy@" };
            GuiMenu walk66 = new GuiMenu { name = "暴徒 2", triger = "SetWalkingStyle", value = "move_f@tough_guy@" };
            GuiMenu walk67 = new GuiMenu { name = "拾荒者 1", triger = "SetWalkingStyle", value = "clipset@move@trash_fast_turn" };
            GuiMenu walk68 = new GuiMenu { name = "拾荒者 2", triger = "SetWalkingStyle", value = "missfbi4prepp1_garbageman" };
            GuiMenu walk69 = new GuiMenu { name = "崔佛", triger = "SetWalkingStyle", value = "move_p_m_two" };


            GuiMenu close = GuiEvents.closeItem;

            List<GuiMenu> gMenu = new List<GuiMenu>();
            gMenu.Add(walk1); gMenu.Add(walk2); gMenu.Add(walk3); gMenu.Add(walk4); gMenu.Add(walk5); gMenu.Add(walk6); gMenu.Add(walk7);
            gMenu.Add(walk8); gMenu.Add(walk9); gMenu.Add(walk10); gMenu.Add(walk11); gMenu.Add(walk12); gMenu.Add(walk13); gMenu.Add(walk14);
            gMenu.Add(walk15); gMenu.Add(walk16); gMenu.Add(walk17); gMenu.Add(walk18); gMenu.Add(walk19); gMenu.Add(walk20); gMenu.Add(walk21);
            gMenu.Add(walk22); gMenu.Add(walk23); gMenu.Add(walk24); gMenu.Add(walk25); gMenu.Add(walk26); gMenu.Add(walk27); gMenu.Add(walk28);

            gMenu.Add(walk29);
            gMenu.Add(walk30);
            gMenu.Add(walk31);
            gMenu.Add(walk32);
            gMenu.Add(walk33);
            gMenu.Add(walk34);
            gMenu.Add(walk35);
            gMenu.Add(walk36);
            gMenu.Add(walk37);
            gMenu.Add(walk38);
            gMenu.Add(walk39);
            gMenu.Add(walk40);
            gMenu.Add(walk41);
            gMenu.Add(walk42);
            gMenu.Add(walk43);
            gMenu.Add(walk44);
            gMenu.Add(walk45);
            gMenu.Add(walk46);
            gMenu.Add(walk47);
            gMenu.Add(walk48);
            gMenu.Add(walk49);
            gMenu.Add(walk50);
            gMenu.Add(walk51);
            gMenu.Add(walk52);
            gMenu.Add(walk53);
            gMenu.Add(walk54);
            gMenu.Add(walk55);
            gMenu.Add(walk56);
            gMenu.Add(walk57);
            gMenu.Add(walk58);
            gMenu.Add(walk59);
            gMenu.Add(walk60);
            gMenu.Add(walk61);
            gMenu.Add(walk62);
            gMenu.Add(walk63);
            gMenu.Add(walk64);
            gMenu.Add(walk65);
            gMenu.Add(walk66);
            gMenu.Add(walk67);
            gMenu.Add(walk68);
            gMenu.Add(walk69);

            gMenu.Add(close);
            Gui y = new Gui()
            {
                image = "",
                guiMenu = gMenu,
                color = "#4AC27D"
            };
            y.Send(p);
        }
        [AsyncClientEvent("SetWalkingStyle")]
        public void PlayerSetWalkingStye(PlayerModel p, string value)
        {
            if (p.Ping > 250)
                return;
            p.SendChatMessage("已更新走姿.");
            p.EmitLocked("Player:SetWalkingStyle", value);
        }

        [Command(CONSTANT.COM_ShowAdmins)]
        public async Task COM_ShowAdmins(PlayerModel p)
        {
            string list = "<center>{5db8e7}在线管理员</center>";
            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (t.adminLevel > 4 && t.adminLevel < 9)
                {
                    var settings = JsonConvert.DeserializeObject<CharacterSettings>(t.settings);
                    AccountModel acc = await Database.DatabaseMain.getAccInfo(t.accountId);
                    string awork = (t.adminWork) ? "{17FF00}执勤" : "{A40505}休息";
                    list += "<br>{FFFFFF}[ID: " + t.sqlID.ToString() + "] " + acc.forumName + " - " + t.characterName.Replace("_", " ") + " {5db8e7}状态: " + awork;
                }
            }
            MainChat.SendInfoChat(p, list, true);
        }

        [Command(CONSTANT.COM_ShowHelpers)]
        public async Task COM_ShowHelpers(PlayerModel p)
        {
            string list = "<center>{5db8e7}在线志愿者</center>";
            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (t.adminLevel == 3 || t.adminLevel == 4)
                {
                    AccountModel acc = await Database.DatabaseMain.getAccInfo(t.accountId);
                    list += "<br>{FFFFFF}[ID: " + t.sqlID.ToString() + "] " + acc.forumName + " - " + t.characterName.Replace("_", " ");
                }
            }
            MainChat.SendInfoChat(p, list, true);
        }

        [Command(CONSTANT.COM_ShowIdentiy)]
        public void COM_ShowIdenty(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendErrorChat(p, CONSTANT.DESC_ShowIdentiy); return; }
            if (!Int32.TryParse(args[0], out int tSql))
                return;
            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }
            if (p.Position.Distance(t.Position) > 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_NeedNearPlayer); return; }
            //if (t.fakeName.Length < 3) { MainChat.SendErrorChat(p, CONSTANT.ERR_NeedNearPlayer); return; }
            if (t.HasSyncedMetaData("isInSpec")) { MainChat.SendErrorChat(p, CONSTANT.ERR_NeedNearPlayer); return; }
            CharacterSettings settings = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            string fromCountry = GlobalEvents.getCountryName(settings.nation);

            MainChat.EmoteMe(p, string.Format(ServerEmotes.EMOTE_ShowIdentiyCard, t.characterName.Replace("_", " ")));
            GlobalEvents.ShowNotification(t, "姓名: " + p.characterName.Replace("_", " ") + "~n~年龄: " + p.characterAge.ToString() + "~n~国籍: " + fromCountry + "~n~出生地: " + settings.location, "洛圣都市", "身份证~n~#54398" + p.sqlID, p);
        }
        
        [AsyncClientEvent("Wheel:showID")]
        public async static Task WheelShowID(PlayerModel p)
        {
            PlayerModel target = GlobalEvents.GetNearestPlayer(p);
            if (target != null)
            {
                if (target.HasSyncedMetaData("isInSpec")) { MainChat.SendErrorChat(p, CONSTANT.ERR_NeedNearPlayer); return; }
                CharacterSettings settings = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
                string fromCountry = GlobalEvents.getCountryName(settings.nation);

                MainChat.EmoteMe(p, string.Format(ServerEmotes.EMOTE_ShowIdentiyCard, target.characterName.Replace("_", " ")));
                GlobalEvents.ShowNotification(target, "姓名: " + p.characterName.Replace("_", " ") + "~n~年龄: " + p.characterAge.ToString() + "~n~国籍: " + fromCountry + "~n~出生地: " + settings.location, "洛圣都市", "身份证~n~#54398" + p.sqlID, p);
            }
        }

        [Command(CONSTANT.COM_ChangeArms)]
        public static void COM_Arms(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, CONSTANT.DESC_ChangeArms); return; }
            CharacterSettings settings = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            if (!Int32.TryParse(args[0], out int newArms)) { MainChat.SendInfoChat(p, CONSTANT.DESC_ChangeArms); return; }

            if ((newArms == 197 || newArms == 198) && settings.MuscleExp <= 47) { MainChat.SendErrorChat(p, "[错误] 你需要 47 健身经验值才能使用这些手臂."); return; }

            settings.arms = newArms;
            if (Int32.TryParse(args[0], out int arms))
            {
                settings.arms = arms;
            }

            if (args.Length > 1)
            {
                if (Int32.TryParse(args[1], out int armsCode))
                    GlobalEvents.SetClothes(p, 3, settings.arms, armsCode);
            }
            else
            {
                GlobalEvents.SetClothes(p, 3, settings.arms, 0);
            }

            p.settings = JsonConvert.SerializeObject(settings); //
            return;
        }

        [Command(CONSTANT.COM_GiveKeys)]
        public async Task COM_GiveKeys(PlayerModel p, params string[] args)
        {
            // anatarver [arac/isyeri/ev] [arac/isyeri/ev ID] [Verilecek kişi ID]
            if (args.Length < 2) { MainChat.SendInfoChat(p, "[用法] /givekey [veh/biz/house] [ID (veh/biz/house)] [将获得 钥匙 的玩家ID]"); return; }
            if (!Int32.TryParse(args[1], out int keySQL)) { MainChat.SendInfoChat(p, "[用法] /givekey [veh/biz/house] [车辆/产业/房屋的ID] [将获得 钥匙 的玩家ID]"); return; }
            if (!Int32.TryParse(args[2], out int targetSql)) { MainChat.SendInfoChat(p, "[用法] /givekey [veh/biz/house] [车辆/产业/房屋的ID] [将获得 钥匙 的玩家ID]"); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(targetSql);
            if (t == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家."); return; }
            if (t.Position.Distance(p.Position) > 5) { MainChat.SendErrorChat(p, "[错误] 离指定玩家太远."); return; }

            switch (args[0])
            {
                case "veh":
                    VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(keySQL);
                    if (v == null) { MainChat.SendErrorChat(p, "[错误] 无效车辆."); return; }
                    if (v.owner != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 您不是指定车辆的主人."); return; }

                    List<Database.DatabaseMain.KeyModel> carKeys = await Database.DatabaseMain.getVehicleKeys(v.sqlID);
                    Database.DatabaseMain.KeyModel cCheck = carKeys.Find(x => x.keyOwner == t.sqlID);
                    if (cCheck != null) { MainChat.SendErrorChat(p, "[错误] 指定玩家已经拥有这个钥匙了!"); return; }
                    cCheck = new Database.DatabaseMain.KeyModel();
                    cCheck.keyOwner = t.sqlID;
                    carKeys.Add(cCheck);
                    await Database.DatabaseMain.updateVehicleKeys(v.sqlID, carKeys);

                    MainChat.SendInfoChat(p, "[!] " + t.characterName.Replace("_", " ") + " 被您 授予 了车辆 [" + v.sqlID + "]" + v.NumberplateText + " 的钥匙.");
                    MainChat.SendInfoChat(t, "[!] " + p.characterName.Replace("_", " ") + " 给您 授予 了车辆 [" + v.sqlID + "]" + v.NumberplateText + " 的钥匙.");
                    return;

                case "biz":
                    (BusinessModel, Marker, PlayerLabel) biz = await Props.Business.getBusinessById(keySQL);
                    if (biz.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效产业!"); return; }
                    if (biz.Item1.ownerId != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 您不是指定产业的主人."); return; }

                    List<Database.DatabaseMain.KeyModel> bizKeys = await Database.DatabaseMain.getBusinessKeys(biz.Item1.ID);
                    Database.DatabaseMain.KeyModel checkKey = bizKeys.Find(x => x.keyOwner == t.sqlID);
                    if (checkKey != null) { MainChat.SendErrorChat(p, "[错误] 指定玩家已经拥有这个钥匙了!"); return; }

                    checkKey = new Database.DatabaseMain.KeyModel();
                    checkKey.keyOwner = t.sqlID;
                    bizKeys.Add(checkKey);
                    await Database.DatabaseMain.updateBusinessKeys(biz.Item1.ID, bizKeys);

                    MainChat.SendInfoChat(p, "[!] " + t.characterName.Replace("_", " ") + " 被您 授予 了产业 [" + biz.Item1.ID + "]" + biz.Item1.name + " 的钥匙.");
                    MainChat.SendInfoChat(t, "[!] " + p.characterName.Replace("_", " ") + " 给您 授予 了产业 [" + biz.Item1.ID + "]" + biz.Item1.name + " 的钥匙.");
                    return;

                case "house":
                    (HouseModel, PlayerLabel, Marker) house = await Props.Houses.getHouseById(keySQL);
                    if (house.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效房屋."); return; }
                    if (house.Item1.ownerId != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 您不是指定房屋的主人."); return; }

                    List<Database.DatabaseMain.KeyModel> houseKeys = await Database.DatabaseMain.getHouseKeys(house.Item1.ID);
                    Database.DatabaseMain.KeyModel hCheck = houseKeys.Find(x => x.keyOwner == t.sqlID);
                    if (hCheck != null) { MainChat.SendErrorChat(p, "[错误] 指定玩家已经拥有这个钥匙了."); return; }

                    hCheck = new Database.DatabaseMain.KeyModel();
                    hCheck.keyOwner = t.sqlID;

                    houseKeys.Add(hCheck);
                    await Database.DatabaseMain.updateHouseKeys(house.Item1.ID, houseKeys);

                    MainChat.SendInfoChat(p, "[!] " + t.characterName.Replace("_", " ") + " 被您 授予 了房屋 [" + house.Item1.ID + "]" + house.Item1.name + " 的钥匙.");
                    MainChat.SendInfoChat(t, "[!] " + p.characterName.Replace("_", " ") + " 给您 授予 了房屋 [" + house.Item1.ID + "]" + house.Item1.name + " 的钥匙.");
                    return;

                default:
                    MainChat.SendInfoChat(p, "[用法] /givekey [veh/biz/house] [车辆/产业/房屋的ID] [将获得 钥匙 的玩家ID]"); return;

            }

        }

        [Command(CONSTANT.COM_ClosePM)]
        public void COM_ClosePM(PlayerModel p)
        {
            string text = (p.isPM) ? "关闭了" : "开启了";
            p.isPM = !p.isPM;
            p.SendChatMessage("{EEE815}> OOC私信: " + text);
        }

        [Command(CONSTANT.COM_CloseAdversiment)]
        public void COM_CloseNews(PlayerModel p)
        {
            string text = (p.isNews) ? "关闭了" : "开启了";
            p.isNews = !p.isNews;
            p.SendChatMessage("{EEE815}> 广告显示: " + text);
        }

        [Command("turnbc")]
        public void COM_CloseBroadCast(PlayerModel p, params string[] args)
        {
            if (p.HasData("News:BroadCast:Closed"))
            {
                p.DeleteData("News:BroadCast:Closed");
                MainChat.SendInfoChat(p, "[?] 广播新闻: 开启了");
                return;
            }
            else
            {
                p.SetData("News:BroadCast:Closed", true);
                MainChat.SendInfoChat(p, "[?] 广播新闻: 关闭了");
                return;
            }
        }

        [Command(CONSTANT.COM_FriskTarget)]
        public async Task COM_FriskPlayer(PlayerModel p, params string[] args)
        {
            try
            {
                if (args.Length <= 0) { MainChat.SendInfoChat(p, CONSTANT.DESC_FriskTarget); return; }

                int ID; bool isOk = Int32.TryParse(args[0], out ID);

                if (isOk)
                {
                    PlayerModel target = GlobalEvents.GetPlayerFromSqlID(ID);
                    if (target == null || target.Position.Distance(p.Position) > 3) { MainChat.SendErrorChat(p, "无效玩家或者离您太远"); return; }
                    MainChat.SendInfoChat(target, p.characterName.Replace("_", " ") + " 向您发送搜身请求.<br>接受请输入 /ustara 反之输入 /reddet");
                    target.SetData("FriskSender", p.sqlID);
                    MainChat.SendInfoChat(p, target.characterName.Replace("_", " ") + " 您已发送搜身请求.");
                }
                else
                {
                    switch (args[0])
                    {
                        case "acp":
                            if (p.HasData("FriskSender"))
                            {
                                PlayerModel t = GlobalEvents.GetPlayerFromSqlID(p.lscGetdata<int>("FriskSender"));
                                if (t == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家."); return; }
                                else if (t.Position.Distance(p.Position) > 3) { MainChat.SendErrorChat(p, CONSTANT.ERR_NotNearTarget); return; }
                                List<InventoryModel> tInv = await Database.DatabaseMain.GetPlayerInventoryItems(p.sqlID);
                                string text = "";
                                foreach (var x in tInv)
                                {
                                    ServerItems a = Items.LSCitems.Find(y => y.ID == x.itemId);
                                    if (a.type == 4) {
                                    }
                                    //else if(a.type == 28) { text = text + " [ID:" + x.ID + " | Yakın Dövüş Silahı Miktar:" + x.itemAmount.ToString() + "]"; }
                                    //else if(a.type == 29) { text = text + " [ID:" + x.ID + " | Birincil Silah Miktar:" + x.itemAmount.ToString() + "]"; }
                                    //else if(a.type == 30) { text = text + " [ID:" + x.ID + " | İkincil Silah Miktar:" + x.itemAmount.ToString() + "]"; }
                                    else
                                    {
                                        text = text + " [ID:" + x.ID + " |" + x.itemName + " 数量:" + x.itemAmount.ToString() + "]";
                                    }

                                }
                                MainChat.EmoteMe(t, " 搜索 " + p.fakeName.Replace("_", " ") + " 的身体.");
                                MainChat.SendInfoChat(t, "{AAE903}" + p.characterName.Replace("_", " ") + "<br>{FFFFFF}" + text + "<br>现金: " + p.cash.ToString(), true);
                                p.DeleteData("FriskSender");
                                return;
                            }
                            else { MainChat.SendErrorChat(p, "[错误] 无搜索请求!"); return; }

                        case "dec":
                            if (p.HasData("FriskSender"))
                            {
                                PlayerModel t2 = GlobalEvents.GetPlayerFromSqlID(p.lscGetdata<int>("FriskTarget"));
                                if (t2 == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }
                                if (t2.Position.Distance(p.Position) > 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_NotNearTarget); return; }
                                MainChat.SendInfoChat(p, "> 您拒绝了搜身请求.");
                                MainChat.SendInfoChat(t2, p.characterName.Replace("_", " ") + " 拒绝了您的搜身请求");
                                p.DeleteData("FriskSender");
                            }
                            break;

                        default:
                            MainChat.SendInfoChat(p, CONSTANT.DESC_FriskTarget); return;
                    }
                }
                return;
            }
            catch { return; }
        }

        [Command(CONSTANT.COM_HairModel)]
        public async Task COM_HairModel(PlayerModel p, params string[] args)
        {
            if (p.HasData("sactopla"))
            {
                p.EmitLocked("character:ServerSync", p.charComps);
                await OtherSystem.Inventory.UpdatePlayerInventory(p);
                p.DeleteData("sactopla");
                CharacterSettings set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);

                p.EmitLocked("Tatto:Load", JsonConvert.SerializeObject(set.tattos));
                return;
            }
            else
            {
                if (Int32.TryParse(args[0], out int hairID)) { GlobalEvents.SetClothes(p, 2, hairID, 0); }
                else { GlobalEvents.SetClothes(p, 2, 11, 0); }
                p.SetData("sactopla", true);
            }
        }


        [Command("trunk")]
        public void COM_OpenCarTrunk(PlayerModel p)
        {
            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v == null) { MainChat.SendErrorChat(p, "[错误] 附近没有车."); return; }
            if (v.LockState == VehicleLockState.Locked) { MainChat.SendErrorChat(p, "[错误] 车是锁的."); return; }

            if (v.settings.TrunkLock == true) // 后备箱是锁的
            {
                if (v.isTrunkOpen == true) // 后备箱已是打开的
                {
                    // 那就允许关闭后备箱 锁的状态保持
                    if (v.NetworkOwner == null)
                        return;
                    
                    v.isTrunkOpen = false;
                    v.NetworkOwner.EmitLocked("Vehicle:OpenTrunk", v, v.isTrunkOpen);
                }
                else // 后备箱已是关闭的
                {
                    MainChat.SendErrorChat(p, "[错误] 车后备箱是锁的, 请输入/trunklock解锁后备箱");
                }
            }
            else // 后备箱是开的 那就随便操作
            {
                if (v.NetworkOwner == null)
                    return;
                
                v.isTrunkOpen = !v.isTrunkOpen;
                if (v.isTrunkOpen == true)
                {
                    MainChat.SendInfoChat(p, "您已成功打开后备箱, 现在可以输入/trunkinv查看后备箱库存了!");
                }
                v.NetworkOwner.EmitLocked("Vehicle:OpenTrunk", v, v.isTrunkOpen);
            }
        }

        [Command("hood")]
        public void COM_OpenCarHood(PlayerModel p, params string[] args)
        {
            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v == null) { MainChat.SendErrorChat(p, "[错误] 附近没有车."); return; }
            if (v.LockState == VehicleLockState.Locked) { MainChat.SendErrorChat(p, "[错误] 车是锁的."); return; }

            if (v.NetworkOwner == null)
                return;

            v.isHoodOpen = !v.isHoodOpen;

            v.NetworkOwner.EmitLocked("Vehicle:OpenHood", v, v.isHoodOpen);
        }

        [Command("ame")]
        public void COM_AME(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /ame [文本]"); return; }
            foreach (string msg in args) { msg.Replace("&lt;", "<").Replace("&#39", "'"); }
            string text = string.Join(" ", args);
            text = text.Replace("&lt;", "<").Replace("&#39", "'").Replace("<br>", "");


            p.SendChatMessage(Chat.MainChatSettings.ChatColors.EmoteMeColor + "** " + text);

            MainChat.AME(p, text);
            return;
        }

        [Command("sme")]
        public void COM_SME(PlayerModel p, params string[] args)
        {
            /*try
            {
                if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /sme [süre(saniye)] [metin]"); return; }
                if (p.HasData("UsingSME")) { MainChat.SendErrorChat(p, "[错误] Şuan bir SME aktif lütfen bitmesini bekleyin."); return; }
                int time; bool timeOK = Int32.TryParse(args[0], out time);
                if (!timeOK) { MainChat.SendInfoChat(p, "[用法] /sme [süre(saniye)] [metin]"); return; }

                if (time >= 61) { MainChat.SendErrorChat(p, "[错误] SME en fazla 60 saniye olarak ayarlanabilir."); return; }

                string text = string.Join(" ", args[1..]);
                GlobalEvents.SetPlayerTag(p, "~p~" + text, false);
                p.SetData("UsingSME", true);
                await Task.Delay(time * 1000);
                if (!p.Exists)
                    return;

                GlobalEvents.ClearPlayerTag(p, false);
                p.DeleteData("UsingSME");
            }
            catch
            {
                return;
            }*/
            if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /sme [时间] [文本]"); return; }
            if (!Int32.TryParse(args[0], out int time)) { MainChat.SendInfoChat(p, "[用法] /sme [时间] [文本]"); return; }

            foreach (string msg in args) { msg.Replace("&lt;", "<").Replace("&#39", "'"); }
            string text = string.Join(" ", args[1..]);
            text = text.Replace("&lt;", "<").Replace("&#39", "'");
            p.SendChatMessage(Chat.MainChatSettings.ChatColors.EmoteMeColor + "*** " + text);

            MainChat.SME(p, text, time);
            return;

        }

        [Command("ado")]
        public void COM_ADO(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /ado [文本]"); return; }
            foreach (string msg in args) { msg.Replace("&lt;", "<").Replace("&#39", "'"); }
            string text = string.Join(" ", args);
            text = text.Replace("&lt;", "<").Replace("&#39", "'");

            p.SendChatMessage(Chat.MainChatSettings.ChatColors.EmoteDoColor + "** " + text);

            MainChat.ADO(p, text);
            return;
        }

        [Command("sdo")]
        public void COM_SDI(PlayerModel p, params string[] args)
        {
            /*try
            {
                if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /sme [süre(saniye)] [metin]"); return; }
                if (p.HasData("UsingSME")) { MainChat.SendErrorChat(p, "[错误] Şuan bir SME aktif lütfen bitmesini bekleyin."); return; }
                int time; bool timeOK = Int32.TryParse(args[0], out time);
                if (!timeOK) { MainChat.SendInfoChat(p, "[用法] /sme [süre(saniye)] [metin]"); return; }

                if (time >= 61) { MainChat.SendErrorChat(p, "[错误] SME en fazla 60 saniye olarak ayarlanabilir."); return; }

                string text = string.Join(" ", args[1..]);
                GlobalEvents.SetPlayerTag(p, "~p~" + text, false);
                p.SetData("UsingSME", true);
                await Task.Delay(time * 1000);
                if (!p.Exists)
                    return;

                GlobalEvents.ClearPlayerTag(p, false);
                p.DeleteData("UsingSME");
            }
            catch
            {
                return;
            }*/
            if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /sdo [时间] [文本]"); return; }
            if (!Int32.TryParse(args[0], out int time)) { MainChat.SendInfoChat(p, "[用法] /sme [时间] [文本]"); return; }

            foreach (string msg in args) { msg.Replace("&lt;", "<").Replace("&#39", "'"); }
            string text = string.Join(" ", args[1..]);
            text = text.Replace("&lt;", "<").Replace("&#39", "'");
            p.SendChatMessage(Chat.MainChatSettings.ChatColors.EmoteMeColor + "*** " + text);


            MainChat.SDO(p, string.Join(" ", args[1..]), time);
            return;

        }

        [Command("taxis")]
        public void COM_ShowTaxiDrivers(PlayerModel p)
        {
            string text = "<center>在线出租车司机</center>";
            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (t.HasData("Taxi:Duty"))
                {
                    text += "<br>" + t.characterName.Replace("_", " ") + " 电话号码: " + t.phoneNumber;
                }
            }
            MainChat.SendInfoChat(p, text, true);
            return;
        }

        [Command("taxiduty")]
        public async Task COM_TaxiDuty(PlayerModel p)
        {
            if (p.HasData("Taxi:Duty"))
            {
                p.DeleteData("Taxi:Duty");
                MainChat.SendInfoChat(p, "[?] 您下班了.");
                GlobalEvents.ClearPlayerTag(p);
                return;
            }
            else
            {
                if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] 您不在车内."); return; }
                if (p.Vehicle.Driver != p) { MainChat.SendErrorChat(p, "[错误] 您必须在驾驶位!"); return; }
                VehModel v = (VehModel)p.Vehicle;
                if (v == null)
                    return;

                if (v.jobId != 4) { MainChat.SendErrorChat(p, "[错误] 此车不属于出租车工作!"); return; }
                if (!await Vehicle.VehicleMain.GetKeysQuery(p, v)) { MainChat.SendErrorChat(p, "[错误] 您无权操作此车(无钥匙)!"); return; }
                p.SetData("Taxi:Duty", true);
                GlobalEvents.SetPlayerTag(p, "~y~出租车~n~~w~联系电话: ~g~" + p.phoneNumber);
                MainChat.SendInfoChat(p, "[?] 您开始了出租车工作.<br>输入 /taximeter [计时费用] 开启计费器.");
                return;
            }
        }

        [Command("showclic")]
        public async Task COM_ShowCarLicense(PlayerModel p, params string[] args)
        {
            //if(p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] Bu komutu kullanabilmek için bir aracın içinde olmalısınız."); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /showclic [id]"); return; }

            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v == null) { MainChat.SendErrorChat(p, "[错误] 附近没有车辆!"); return; }
            if (!await Vehicle.VehicleMain.GetKeysQuery(p, v)) { MainChat.SendErrorChat(p, "[错误] 您无权访问此车辆(无钥匙)."); return; }

            int tSql; bool isOk = Int32.TryParse(args[0], out tSql);
            if (!isOk) { MainChat.SendInfoChat(p, "[用法] /showclic [id]"); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }
            if (t.Position.Distance(p.Position) > 5) { MainChat.SendInfoChat(p, "[错误] 无效玩家或者离您太远!"); return; }

            PlayerModelInfo ownerOfVehicle = await Database.DatabaseMain.getCharacterInfo(v.owner);
            if (ownerOfVehicle != null)
            {
                MainChat.SendInfoChat(t, "{BFA75D}<center> 车辆登记信息 </center><br>车牌号码: " + v.NumberplateText + "<br>车主: " + ownerOfVehicle.characterName.Replace("_", " ") + "<br>税: " + v.fine.ToString(), true);
            }
            else
            {
                MainChat.SendInfoChat(t, "{BFA75D}<center> 车辆登记信息 </center><br>车牌号码: " + v.NumberplateText + "<br>车主: 无 <br>税: " + v.fine.ToString(), true);
            }

            MainChat.SendInfoChat(p, "[?] 成功出示车辆登记信息.");
            MainChat.EmoteMe(p, " 向 " + t.characterName.Replace("_", " ") + " 出示了未知的证件");
            return;
        }

        [Command("fontsize")]
        public void COM_FontSize(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /fontsize [5 - 15] (默认: 10)"); return; }
            //double size; bool isOk = double.TryParse(args[0].Replace(".",","), out size);
            //if (!isOk) { MainChat.SendInfoChat(p, "[用法] /fontsize [5 - 15] (Varsılan: 10)"); return; }
            //if(size > 1.5 ||size < 0.5) { MainChat.SendInfoChat(p, "[用法] /fontsize [5 - 15 (Varsılan: 10)]"); return; }         
            if (!Int32.TryParse(args[0], out int fontSize)) { MainChat.SendInfoChat(p, "[用法] /fontsize [5 - 15] (默认: 10)"); return; }
            if (fontSize < 5 || fontSize > 15) { MainChat.SendInfoChat(p, "[用法] /fontsize [5 - 15] (默认: 10)"); return; }
            string newFT = ((double)fontSize / 10).ToString().Replace(",", ".") + "em";
            p.EmitLocked("chat:fontsize", newFT);

            CharacterSettings newSet = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            newSet.FontSize = (double)fontSize / 10;
            p.settings = JsonConvert.SerializeObject(newSet);
            return;
        }
        [Command("ssmod")]
        public void COM_SSMod(PlayerModel p)
        {
            p.EmitAsync("chat:ssmod", 0);
        }

        [Command("ssmod2")]
        public void COM_SSMod2(PlayerModel p)
        {
            p.EmitAsync("chat:ssmod", 1);
        }

        [Command("yardim2")]
        public void COM_ShowHelpPage(PlayerModel p)
        {
            p.EmitLocked("HelpPage:Show", p.adminLevel);
        }

        [Command("mic")]
        public void COM_Microphone(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /mic [消息]"); return; }
            CharacterSettings set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            if (set.hasMicrophone == false)
            {
                MainChat.SendErrorChat(p, "[错误] 您没有 麦克风 权限.");
                return;
            }

            string msg = string.Join(" ", args);
            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (t.Position.Distance(p.Position) < 50 && t.Dimension == p.Dimension) { t.SendChatMessage(p.characterName.Replace("_", " ") + " 说(麦克风): " + msg); }
            }
            return;
        }

        [Command("coord")]
        public void MyCommand2(IPlayer player)
        {
            Position p = player.Position;
            //Alt.Log(p.X.ToString() + "f, " + p.Y.ToString() + "f, " + p.Z.ToString() + "f");
            //Alt.Log(player.HeadRotation.ToString());
            //Alt.Log(player.Rotation.ToString());

            player.SendChatMessage((p.X.ToString() + "f, " + p.Y.ToString() + "f, " + p.Z.ToString() + "f"));
        }

        [Command("testgun")]
        public void oPos(PlayerModel p, WeaponModel id)
        {
            if (p.adminLevel < 6) { return; }
            p.GiveWeapon((uint)id, 9000, true);
        }

        [Command("oldcar")]
        public void ShowOldCar(PlayerModel p)
        {
            if (p.oldCar != 0) { MainChat.SendInfoChat(p, "最后使用车辆 : " + p.oldCar.ToString());
            }
        }

        [Command("setgm")]
        public async Task COM_EditGraphicMode(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /setgm [画质ID]<br> 以下可用画质ID;<br>{FFFFFF}1-31: 正常画面设置.<br>32-40: 低配置画面设置.<br>0: 默认画质."); return; }
            int selection; bool isOK = Int32.TryParse(args[0], out selection);
            if (!isOK) { MainChat.SendInfoChat(p, "[用法] /setgm [画质ID]<br> 以下可用画质ID;<br>{FFFFFF}1-31: 正常画面设置.<br>32-40: 低配置画面设置.<br>0: 默认画质."); return; }

            CharacterSettings set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            if (set == null) set = new CharacterSettings();

            set.GrapichMode = selection;
            p.settings = JsonConvert.SerializeObject(set);
            await p.updateSql();
            GlobalEvents.ChangeGraphicMode(p, selection);
            MainChat.SendInfoChat(p, "成功更新画质ID为: " + selection);
            return;
        }

        [Command("dutys")]
        public void COM_Dutys(PlayerModel p)
        {
            // int PD = 0;
            int FD = 0; int News = 0;

            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (t.HasData(EntityData.PlayerEntityData.FDDuty))
                    FD += 1;

                // if (t.HasData(EntityData.PlayerEntityData.PDDuty))
                //     PD += 1;

                if (t.HasData(EntityData.PlayerEntityData.NewsDuty))
                    News += 1;
            }

            MainChat.SendInfoChat(p, "<center>在线组织</center><br>{b22222}LSFD: {FFFFFF}" + FD.ToString() + "<br>{ffc000}新闻: {FFFFFF}" + News.ToString(), true);
            return;
        }

        [Command("starttrace")]
        public void COM_StartTrace(PlayerModel p)
        {
            if (p.adminLevel < 8)
                return;
            AltTrace.Start("serverTrace");
        }

        [Command("stoptrace")]
        public void COM_StopTrace(PlayerModel p)
        {
            if (p.adminLevel < 8)
                return;
            AltTrace.Stop();
        }

        [Command("removecarkey")]
        public async Task COM_RemoveCarKey(PlayerModel p, params string[] args)
        {
            // anahtaral [veh/house/biz] [ID(veh/house/biz)] [Anahtar Alınacak Kişi ID]
            if (args.Length < 2) { MainChat.SendInfoChat(p, "[用法] /removecarkey [veh/house/biz] [车辆/房屋/产业的ID] [钥匙主人的ID]"); return; }
            if (!Int32.TryParse(args[1], out int KeyID)) { MainChat.SendInfoChat(p, "[用法] /removecarkey [veh/house/biz] [车辆/房屋/产业的ID] [钥匙主人的ID]"); return; }
            if (!Int32.TryParse(args[2], out int tSql)) { MainChat.SendInfoChat(p, "[用法] /removecarkey [veh/house/biz] [车辆/房屋/产业的ID] [钥匙主人的ID]"); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);

            switch (args[0])
            {
                case "veh":
                    VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(KeyID);
                    if (v == null) { MainChat.SendErrorChat(p, "[错误] 无效车辆!"); return; }
                    if (v.owner != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 此车不属于指定玩家!"); return; }

                    List<Database.DatabaseMain.KeyModel> carKeys = await Database.DatabaseMain.getVehicleKeys(v.sqlID);
                    Database.DatabaseMain.KeyModel cCheck = carKeys.Find(x => x.keyOwner == tSql);
                    if (cCheck == null) { MainChat.SendErrorChat(p, "[错误] 此车钥匙不属于指定玩家!"); return; }
                    carKeys.Remove(cCheck);
                    await Database.DatabaseMain.updateVehicleKeys(v.sqlID, carKeys);

                    if (t == null)
                    {
                        MainChat.SendInfoChat(p, "[!] 您归还了唯一ID为 " + tSql + " 的车钥匙.");
                        return;
                    }
                    else
                    {
                        MainChat.SendInfoChat(p, "[!] " + t.characterName.Replace("_", " ") + " 收到了您归还的车钥匙.");
                        MainChat.SendInfoChat(t, "[!] " + p.characterName.Replace("_", " ") + " 归还了您的车钥匙.");
                        return;
                    }

                case "house":
                    (HouseModel, PlayerLabel, Marker) house = await Props.Houses.getHouseById(KeyID);
                    if (house.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效房屋."); return; }
                    if (house.Item1.ownerId != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 此房屋不属于指定玩家."); return; }

                    List<Database.DatabaseMain.KeyModel> houseKeys = await Database.DatabaseMain.getHouseKeys(house.Item1.ID);
                    Database.DatabaseMain.KeyModel hCheck = houseKeys.Find(x => x.keyOwner == t.sqlID);
                    if (hCheck == null) { MainChat.SendErrorChat(p, "[错误] 此房屋钥匙不属于指定玩家!"); return; }

                    houseKeys.Remove(hCheck);
                    await Database.DatabaseMain.updateHouseKeys(house.Item1.ID, houseKeys);

                    if (t == null)
                    {
                        MainChat.SendInfoChat(p, "[!] 您归还了唯一ID为 " + tSql + " 的房屋钥匙.");
                        return;
                    }
                    else
                    {
                        MainChat.SendInfoChat(p, "[!] " + t.characterName.Replace("_", " ") + " 收到了 " + house.Item1.name + " 您归还的房屋钥匙.");
                        MainChat.SendInfoChat(t, "[!] " + p.characterName.Replace("_", " ") + " 归还了 " + house.Item1.name + " 的房屋钥匙.");
                        return;
                    }

                case "biz":
                    (BusinessModel, Marker, PlayerLabel) biz = await Props.Business.getBusinessById(KeyID);
                    if (biz.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效产业!"); return; }
                    if (biz.Item1.ownerId != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 此产业不属于指定玩家."); return; }

                    List<Database.DatabaseMain.KeyModel> bizKeys = await Database.DatabaseMain.getBusinessKeys(biz.Item1.ID);
                    Database.DatabaseMain.KeyModel checkKey = bizKeys.Find(x => x.keyOwner == t.sqlID);
                    if (checkKey == null) { MainChat.SendErrorChat(p, "[错误] 此产业钥匙不属于指定玩家!"); return; }

                    bizKeys.Remove(checkKey);
                    await Database.DatabaseMain.updateBusinessKeys(biz.Item1.ID, bizKeys);

                    if (t == null)
                    {
                        MainChat.SendInfoChat(p, "[!] 您归还了唯一ID为 " + tSql + " 的产业钥匙.");
                        return;
                    }
                    else
                    {
                        MainChat.SendInfoChat(p, "[!] " + t.characterName.Replace("_", " ") + " 收到了 " + biz.Item1.name + " 您归还的产业钥匙.");
                        MainChat.SendInfoChat(t, "[!] " + p.characterName.Replace("_", " ") + " 归还了 " + biz.Item1.name + " 的产业钥匙.");
                        return;
                    }

                default:
                    MainChat.SendInfoChat(p, "[用法] /removecarkey [veh/house/biz] [车辆/房屋/产业的ID] [钥匙主人的ID]"); return;
            }

        }

        [Command("resetkeys")]
        public static async Task COM_ResetKeys(PlayerModel p, params string[] args)
        {
            if (args.Length < 1) { MainChat.SendInfoChat(p, "[用法] /resetkeys [house/biz/veh] [房屋/产业/车辆的ID]"); return; }

            if (!Int32.TryParse(args[1], out int KeySql)) { MainChat.SendInfoChat(p, "[用法] /resetkeys [house/biz/veh] [房屋/产业/车辆的ID]"); return; }

            switch (args[0])
            {
                case "veh":
                    VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(KeySql);
                    if (v == null) { MainChat.SendErrorChat(p, "[错误] 无效车辆!"); return; }
                    if (v.owner != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 此车不属于您!"); return; }

                    List<Database.DatabaseMain.KeyModel> carKeys = new List<Database.DatabaseMain.KeyModel>();
                    await Database.DatabaseMain.updateVehicleKeys(v.sqlID, carKeys);
                    MainChat.SendInfoChat(p, "[!] 您重置了车牌号为 " + v.NumberplateText + " 的车辆钥匙.");
                    return;

                case "biz":
                    (BusinessModel, Marker, PlayerLabel) biz = await Props.Business.getBusinessById(KeySql);
                    if (biz.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效产业!"); return; }
                    if (biz.Item1.ownerId != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 此产业不属于您."); return; }

                    List<Database.DatabaseMain.KeyModel> bizKeys = new List<Database.DatabaseMain.KeyModel>();
                    await Database.DatabaseMain.updateBusinessKeys(biz.Item1.ID, bizKeys);

                    MainChat.SendInfoChat(p, "[!] 您重置了 [" + biz.Item1.ID + "]" + biz.Item1.name + " 的产业钥匙.");
                    return;

                case "house":
                    (HouseModel, PlayerLabel, Marker) house = await Props.Houses.getHouseById(KeySql);
                    if (house.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效房屋."); return; }
                    if (house.Item1.ownerId != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 此房屋不属于您."); return; }

                    List<Database.DatabaseMain.KeyModel> houseKeys = new List<Database.DatabaseMain.KeyModel>();
                    await Database.DatabaseMain.updateHouseKeys(house.Item1.ID, houseKeys);

                    MainChat.SendInfoChat(p, "[!] 您重置了 [" + house.Item1.ID + "]" + house.Item1.name + " 的产业钥匙.");
                    return;

                default:
                    MainChat.SendInfoChat(p, "[用法] /resetkeys [house/biz/veh] [房屋/产业/车辆的ID]");
                    return;
            }

        }


        [Command("knock")]
        public async Task COM_KnockDoor(PlayerModel p)
        {
            (BusinessModel, Marker, PlayerLabel) biz = await Props.Business.getNearestBusiness(p);
            if (biz.Item1 != null)
            {
                MainChat.EmoteMe(p, " 按了下门铃.");
                if (!biz.Item1.isLocked) { MainChat.SendErrorChat(p, "此产业的门是打开的(( 请合理扮演 ))"); }

                foreach (PlayerModel bt in Alt.GetAllPlayers())
                {
                    if (bt.Position.Distance(biz.Item1.interiorPosition) < 25 && bt.Dimension == biz.Item1.dimension)
                    {
                        bt.SendChatMessage("{B18CE9} 门铃响了, 有人好像在敲门!");
                    }
                }
            }
            else
            {
                (HouseModel, PlayerLabel, Marker) h = await Props.Houses.getNearHouse(p);
                if (h.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 附近没有房屋."); return; }

                MainChat.EmoteMe(p, " 按了下门铃.");
                foreach (PlayerModel bh in Alt.GetAllPlayers())
                {
                    if (bh.Position.Distance(h.Item1.intPos) < 25 && bh.Dimension == h.Item1.dimension)
                    {
                        bh.SendChatMessage("{B18CE9} 门铃响了, 有人好像在敲门!");
                    }
                }
            }
        }

        [Command("ears")]
        public void COM_Ears(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /ears [耳朵饰品ID]"); return; }
            if (!Int32.TryParse(args[0], out int drawID)) { MainChat.SendInfoChat(p, "[用法] /ears [耳朵饰品ID]"); return; }

            p.EmitAsync("SetClothesProps", 2, drawID, 0);
            MainChat.SendInfoChat(p, "[!] 成功装配耳朵饰品.");
            return;
        }

        [Command("earsoff")]
        public void COM_RemoveEars(PlayerModel p)
        {
            p.EmitAsync("Clear:Ears");
        }

        [Command("body")]
        public void COM_ShowInjuredState(PlayerModel p)
        {
            p.SendChatMessage("<center>" + p.fakeName.Replace("_", " ") + "</center>");

            string total = "健康";
            if (p.injured.head || p.injured.torso || p.injured.arms || p.injured.legs) { total = "负伤"; }
            else if (p.injured.isDead) { total = "重伤/死亡"; }
            p.SendChatMessage("血量值: " + p.Health);
            p.SendChatMessage("总体健康状况: " + total);

            string head = (p.injured.head) ? "流血" : "健康";
            p.SendChatMessage("头部: " + head);

            string torso = (p.injured.torso) ? "流血" : "健康";
            p.SendChatMessage("躯干: " + torso);

            string arms = (p.injured.arms) ? "流血" : "健康";
            p.SendChatMessage("手臂: " + arms);

            string legs = (p.injured.legs) ? "流血" : "健康";
            p.SendChatMessage("腿部: " + legs);
        }

        [Command("myguns")]
        public void COM_ShowWeapons(PlayerModel p, params string[] args)
        {
            string text = "[ID {0}] 序列号: {1} | 耐久度: {2} | 弹药: {3}";
            if (args.Length <= 0)
            {
                p.SendChatMessage("<center>我的武器</center>");
                if (p.melee != null) { p.SendChatMessage(string.Format(text, p.melee.ID.ToString(), p.melee.Durability.ToString(), p.melee.bullet.ToString())); }
                if (p.secondary != null) { p.SendChatMessage(string.Format(text, p.secondary.ID.ToString(), p.secondary.Durability.ToString(), p.secondary.bullet.ToString())); }
                if (p.primary != null) { p.SendChatMessage(string.Format(text, p.primary.ID.ToString(), p.primary.Durability.ToString(), p.primary.bullet.ToString())); }
            }
            else
            {
                if (!Int32.TryParse(args[0], out int tSql)) { MainChat.SendInfoChat(p, "[用法] 查看自己的武器直接输入 /myguns , 展示给其他玩家则 /myguns [玩家ID]"); return; }
                PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
                if (t == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }
                if (t.Position.Distance(p.Position) > 3 && p.adminLevel < 1) { MainChat.SendErrorChat(p, "[错误] 离玩家太远(无权限)."); return; }

                p.SendChatMessage("<center>我的武器</center>");
                if (t.melee != null) { p.SendChatMessage(string.Format(text, t.melee.ID.ToString(), t.melee.Durability.ToString(), t.melee.bullet.ToString())); }
                if (t.secondary != null) { p.SendChatMessage(string.Format(text, t.secondary.ID.ToString(), t.secondary.Durability.ToString(), t.secondary.bullet.ToString())); }
                if (t.primary != null) { p.SendChatMessage(string.Format(text, t.primary.ID.ToString(), t.primary.Durability.ToString(), t.primary.bullet.ToString())); }
            }
        }


        [Command("mycarmods")]
        public async void COM_ShowModifies(PlayerModel p)
        {
            MainChat.SendErrorChat(p, "[错误] 此指令已被禁用."); return;
            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误]  Bu komutu kullanabilmek için bir araçta olmalısınız."); return; }

            string text = "<center>Araca Ait Modifiyetler</center>";
            text += "<br>Lastik Tipi: " + v.WheelVariation + " | Jant Tipi: " + v.WheelType;
            text += "<br> Spoiler: " + v.GetMod(0) + " | Ön Tampon: " + v.GetMod(1) + " | Arka Tampon: " + v.GetMod(2) +
                " | Yan Etek: " + v.GetMod(3) + " | Egzoz: " + v.GetMod(4) + " | Çerçeve: " + v.GetMod(5) +
                " | Kaput Eklentisi: " + v.GetMod(7) + " | Kanat(Sol): " + v.GetMod(8) + " | Kanat(sağ): " + v.GetMod(9) +
                " | Tavan: " + v.GetMod(10) + " | Motor: " + v.GetMod(11) + " | Frenler: " + v.GetMod(12) + " | Şanzıman: " + v.GetMod(13) +
                " | Korna: " + v.GetMod(14) + " | Zırh: " + v.GetMod(15) + " | Turbo: " + v.GetMod(18) + " | Xenon Far: " + v.GetMod(22) +
                " | Plaka Tutucu: " + v.GetMod(25) + " | Plaka Tipi: " + v.GetMod(26) + " | Hava Filtresi: " + v.GetMod(40) + " | Livery: " + v.GetMod(48);

            /*await Task.Delay(1000);
            if (!p.Exists)
                return;*/
            MainChat.SendInfoChat(p, text);

            //if(p.EmitLocked("Show:VehicleModify") == null) { MainChat.SendErrorChat(p, "[错误] Bir hata meydana geldi."); return; } else { p.EmitLocked("Show:VehicleModify", v.Id); return; }

        }

        [Command("cc", aliases: new string[] { "clearchat", "cchat", "cmychat" })]
        public void COM_ClearChat(PlayerModel p)
        { 
            p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
            GlobalEvents.SubTitle(p, "~w~聊天框已清理.", 2);
            return;
        }

        [Command("try")]
        public void COM_TrySomething(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /try [文本]<br>提示: /try 打开手中封闭的瓶子."); return; }
            Random rnd = new Random();
            int state = rnd.Next(0, 10);
            string _state;
            if (state <= 4) { _state = "{58D22B}成功"; }
            else _state = "{E85454}失败";

            MainChat.EmoteDoAlternative(p, string.Join(" ", args) + " - " + p.characterName.Replace("_", " ") + " | " + _state + "{5fa186}");
            
            if (p.isFinishTut == 15)
            {
                p.isFinishTut = 16;
                p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}基本指令");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}嘿, 您成功使用了尝试指令!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}现在, 让我们尝试使用本地OOC指令吧!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}现在, 请尝试使用 {fc5e03}/b{FFFFFF} 指令!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}例如用法, /b 无视, 继续RP....");
            }
            return;
        }


        [Command("addsanim")]
        public async Task COM_AddAlternateAnim(PlayerModel p, params string[] args)
        {
            if (args.Length <= 2) { MainChat.SendInfoChat(p, "[用法] /addsanim [动作名称] [动作字典] [动作字典名称]<br>提示: /addanim 单杠 amb@prop_human_muscle_chin_ups@male@base base"); return; }
            CharacterSettings set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            if (set.anims.Count >= 40) { MainChat.SendErrorChat(p, "[错误] 您最多可以添加 20 个备选动作, 请删除一个目前已存在的备选动以便您可以添加一个新的备选动作.<br> /delsanim"); return; }
            var check = set.anims.Find(x => x.name == args[0]);
            if (check != null) { MainChat.SendErrorChat(p, "[错误] 您已经添加过同名的动作, 请输入 /sanimlist 查看是否存在."); return; }

            AlternateAnim anim = new AlternateAnim()
            {
                name = args[0],
                dict = args[1],
                anim = args[2]
            };

            set.anims.Add(anim);
            p.settings = JsonConvert.SerializeObject(set);
            await p.updateSql();
            MainChat.SendInfoChat(p, "[?] 成功添加备选动作 '" + args[0]);
            return;
        }
        [Command("delsanim")]
        public async Task COM_RemoveAlternateAnimations(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /delsanim [动作名称]"); return; }
            CharacterSettings set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            var check = set.anims.Find(x => x.name == args[0]);
            if (check == null) { MainChat.SendErrorChat(p, "[错误] 无效备选动作!"); return; }

            set.anims.Remove(check);
            p.settings = JsonConvert.SerializeObject(set);
            await p.updateSql();
            MainChat.SendInfoChat(p, "[?] 成功删除 " + args[0] + " - 备选动作.");
            return;
        }
        
        [Command("sanimlist")]
        [AsyncClientEvent("showAnimlist")]
        public void COM_ShowAlternateAnims(PlayerModel p)
        {
            CharacterSettings set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            string anims = "<center>备选动作列表</center><br>";
            set.anims.ForEach(x =>
            {
                anims += x.name + " ";
            });
            MainChat.SendInfoChat(p, anims, true);
            return;
        }

        [Command("aa", aliases: new string[] { "alternateanim", "alternatifanim" })]
        public void COM_PlayerAlternateAnim(PlayerModel p, params string[] args)
        {
            if (p.adminJail > 0 || p.jailTime > 0) { MainChat.SendErrorChat(p, "[错误] 无法在目前的状态使用此指令."); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /aa [动作名称]<br>如果忘记了动作名称, 可以输入 /sanimlist 查看"); return; }
            if (p.Vehicle != null) { return; }
            CharacterSettings set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            var check = set.anims.Find(x => x.name == args[0]);
            if (check == null) { MainChat.SendErrorChat(p, "[错误] 备选动作 " + args[0] + " 不存在."); return; }
            GlobalEvents.PlayAnimation(p, new string[] { check.dict, check.anim }, 1);
            return;
        }

        [Command("cb")]
        public void COM_ChatBouble(PlayerModel p)
        {
            p.EmitAsync("Chat:BoubleClose");
        }

        [Command("accent")]
        public void COM_Accent(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /accent [口音]"); return; }
            p.SetData("Player:Accent", string.Join(" ", args));
            MainChat.SendInfoChat(p, "[?] 成功设置口音为: " + string.Join(" ", args) + " (( 请合理扮演 ))");
            return;
        }
        [Command("delaccent")]
        public void COM_ClearAccent(PlayerModel p)
        {
            if (p.HasData("Player:Accent"))
                p.DeleteData("Player:Accent");

            MainChat.SendInfoChat(p, "[?] 成功删除口音.");
            return;
        }

        [Command("quitfaction")]
        public async Task COM_LeaveFaction(PlayerModel p)
        {
            if (p.factionId <= 0) { MainChat.SendErrorChat(p, "[错误] 您没有组织."); return; }

            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (t.factionId == p.factionId)
                {
                    t.SendChatMessage("{15C5B0}" + p.characterName.Replace("_", " ") + " 手动离开了组织");
                }
            }

            p.factionId = 0;
            await p.updateSql();
            return;
        }

        [Command("quitbiz")]
        public async Task COM_LeaveBusiness(PlayerModel p)
        {
            if (p.businessStaff <= 0) { MainChat.SendErrorChat(p, "[错误] 您没有工作于产业."); return; }

            var business = await Props.Business.getBusinessById(p.businessStaff);
            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(business.Item1.ownerId);
            if (t != null) { MainChat.SendInfoChat(t, "[?] " + p.characterName.Replace("_", " ") + " 手动离开了产业."); }

            p.businessStaff = 0;
            await p.updateSql();
            MainChat.SendInfoChat(p, "[?] 成功从工作的产业辞职.");
            return;
        }

        [Command("hdo")]
        public async Task COM_HouseDo(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /hdo [文本]"); return; }
            var house = await Props.Houses.getNearHouse(p);
            if (house.Item1 == null)
            {
                PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Find(x => p.Position.Distance(x.Position) < 3f && x.Dimension == p.Dimension);
                if (entranceLabel == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_NotNearHouse); return; }
                entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position bussinesPos);

                house = await Props.Houses.getHouseFromPos(bussinesPos);
                if (house.Item1 == null)
                {
                    MainChat.SendErrorChat(p, "[错误] 附近没有房屋!"); return;
                }
            }
            string message = string.Join(" ", args);
            //MainChat.EmoteDo(p, message);
            message = p.fakeName.Replace("_", " ") + " " + message;
            MainChat.EmoteDo(house.Item1.intPos, message, dimension: house.Item1.dimension, distance: 25);
            MainChat.EmoteDo(house.Item1.pos, message);
            return;
        }

        [Command("hme")]
        public async Task COM_HouseMe(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /hme [文本]"); return; }
            var house = await Props.Houses.getNearHouse(p);
            if (house.Item1 == null)
            {
                PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Find(x => p.Position.Distance(x.Position) < 3f && x.Dimension == p.Dimension);
                if (entranceLabel == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_NotNearHouse); return; }
                entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position bussinesPos);

                house = await Props.Houses.getHouseFromPos(bussinesPos);
                if (house.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 附近没有房屋!"); return; }

            }
            string message = string.Join(" ", args);
            //MainChat.EmoteMe(p, message);
            message = p.fakeName.Replace("_", " ") + " " + message;
            MainChat.EmoteMe(house.Item1.intPos, message, house.Item1.dimension, 25);
            MainChat.EmoteMe(house.Item1.pos, message, 0, 25);
            return;
        }

        [Command("hs")]
        public async Task COM_HouseShout(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /hs [文本]"); return; }
            var house = await Props.Houses.getNearHouse(p);
            if (house.Item1 == null)
            {
                PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Find(x => p.Position.Distance(x.Position) < 3f && x.Dimension == p.Dimension);
                if (entranceLabel == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_NotNearHouse); return; }
                entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position bussinesPos);

                house = await Props.Houses.getHouseFromPos(bussinesPos);
                if (house.Item1 == null)
                {
                    MainChat.SendErrorChat(p, "[错误] 附近没有房屋!"); return;
                }

            }
            string message = string.Join(" ", args);
            message = p.fakeName.Replace("_", " ") + " 大喊道: " + message + "!";
            MainChat.ShoutChat(message, house.Item1.intPos, house.Item1.dimension);
            MainChat.ShoutChat(message, house.Item1.pos, 0);
            return;
        }



        [Command("bag")]
        public void COM_Bag(PlayerModel p, params string[] args)
        {
            if (p.HasData("Jacking:LastUsed"))
            {
                MainChat.SendInfoChat(p, "[错误] 无法在抢劫期间使用此指令.");
                return;
            }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /bag [0-200]"); return; }
            if (!Int32.TryParse(args[0], out int bagID)) { MainChat.SendInfoChat(p, "[用法] /bag [0-200]"); return; }
            if (bagID < 0 || bagID > 200) { MainChat.SendInfoChat(p, "[用法] /bag [0-200]"); return; }
            GlobalEvents.SetClothes(p, 5, bagID, 0);
            MainChat.SendInfoChat(p, "[!] 成功操作背包.");
            if (bagID == 0)
            {
                if (p.HasData("PlayerHasBag"))
                    p.DeleteData("PlayerHasBag");
            }
            else
            {
                p.SetData("PlayerHasBag", true);
            }
            return;
        }

        [Command("chatfont")]
        public async Task COM_ChatFont(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /chatfont [字体名称] [字体链接]<br>如果您需要在字体名称中使用空格, 请使用 _ (下划线)."); return; }
            var setting = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            if (setting == null) { MainChat.SendErrorChat(p, "[错误] Karakter ayarları getirilirken bir hata meydana geldi."); return; }

            setting.Font = new string[] { args[0].Replace("_", " "), args[1] };
            p.EmitAsync("ct:font", args[0].Replace("_", " "), args[1]);
            p.settings = JsonConvert.SerializeObject(setting);
            await p.updateSql();

            MainChat.SendInfoChat(p, "[?] 成功设置聊天框字体.<br> 现在来测试一下:<br>你好 世界");
            return;
        }

        [Command("lawyers")]
        public void COM_ShowLawyers(PlayerModel p)
        {
            string text = "<center>在线律师</center>";
            foreach (PlayerModel pl in Alt.GetAllPlayers())
            {
                if (!pl.isOnline)
                    continue;

                if (pl.sqlID <= 0)
                    continue;

                var set = JsonConvert.DeserializeObject<CharacterSettings>(pl.settings);
                if (set != null)
                {
                    if (set.isLawyer)
                    {
                        text += "<br>" + pl.characterName.Replace("_", " ") + " " + set.LawyerDep + " 联系电话: " + pl.phoneNumber;
                    }
                }
            }
            MainChat.SendInfoChat(p, text, true);
            return;
        }

        [Command("supports")]
        public async Task COM_ShowSupports(PlayerModel p)
        {
            string text = "<center>在线支持者</center>";
            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (t.adminLevel == 2)
                {
                    AccountModel acc = await Database.DatabaseMain.getAccInfo(t.accountId);
                    if (acc == null)
                        continue;

                    text += "<br>[ID: " + t.sqlID + "] " + t.characterName.Replace("_", " ") + " | " + acc.forumName;
                }
            }

            MainChat.SendInfoChat(p, text, true);
            return;
        }

        [Command("pagesize")]
        public void COM_PageSize(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /pagesize [数值]"); return; }
            if (!Int32.TryParse(args[0], out int PageSize)) { MainChat.SendInfoChat(p, "[用法] /pagesize [数值]"); return; }
            p.EmitAsync("chat:pagesize", PageSize);
            return;
        }

        [Command("faggiobag")]
        public void COM_FaggioBag(PlayerModel p)
        {
            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v == null) { MainChat.SendErrorChat(p, "[错误] 附近没有车辆!"); return; }
            if (v.Model != (uint)VehicleModel.Faggio) { MainChat.SendErrorChat(p, "[错误] 此指令只用于 Faggio 模型车辆."); return; }
            if (v.owner != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 此车不属于您."); return; }

            if (v.HasStreamSyncedMetaData("AttachedObjects"))
            {
                AttachmentSystem.deleteAllAttachs(v);
                MainChat.SendInfoChat(p, "[?] 已拆下 Faggio 的装饰品.");
                return;
            }
            else
            {
                AttachmentSystem.ObjectModel o = new AttachmentSystem.ObjectModel()
                {
                    Model = "v_club_vu_djbag",
                    boneIndex = "chassis",
                    zPos = 0.28,
                    yPos = -0.84,
                };
                AttachmentSystem.AddAttach(v, o);
                MainChat.SendInfoChat(p, "[?] 已安装 Faggio 的装饰品.");
                return;
            }
        }

        [Command("relog")]
        public async void COM_Relog(PlayerModel p)
        {
            //if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, "[错误] Bu sistem şuanda pasif durumdadır."); return; }
            //t.SetData("Danger", DateTime.Now.ToString());
            if (p.HasData("Danger"))
            {
                if (!DateTime.TryParse(p.lscGetdata<string>("Danger"), out DateTime danger))
                {
                    MainChat.SendErrorChat(p, "[错误] 发生了错误, 请联系管理员.");
                    return;
                }
                else
                {
                    if (danger.AddMinutes(10) > DateTime.Now && p.adminLevel < 4)
                    {
                        MainChat.SendErrorChat(p, "[错误] 您可以在受到伤害后 10 分钟重新登录.");
                        return;
                    }
                }
            }
            p.EmitAsync("Phone:Destroy");

            p.updateSql();
            p.updateSql();
            if (p.Exists)
                outRp.ServerEvents.OnPlayerDisconnect(p, "Relog");

            foreach (var data in p.GetAllDataKeys())
            {
                if (p.Exists)
                {
                    if (p.HasData(data))
                        p.DeleteData(data);
                    else if (p.HasStreamSyncedMetaData(data))
                        p.DeleteStreamSyncedMetaData(data);
                    else if (p.HasSyncedMetaData(data))
                        p.DeleteSyncedMetaData(data);
                }
            }

            try
            {
                p.sqlID = 0;
                p.showSqlId = true;
                p.accountId = 0;
                p.characterName = "未登录玩家";
                p.characterAge = 0;
                p.characterExp = 0;
                p.characterLevel = 0;
                p.gameTime = 1;
                p.factionId = 0;
                p.factionRank = 0;
                p.RadioFreq = 1;
                p.businessStaff = 0;
                p.cash = 0;
                p.bankCash = 0;
                p.Strength = 30;
                p.adminLevel = 0;
                p.isCk = false;
                p.jailTime = 0;
                p.phoneNumber = 0;
                p.firstLogin = false;
                p.isCuffed = false;
                p.stats = "[]";
                p.adminJail = 0;
                p.settings = "[]";
                //Data Keys  
                p.adminWork = false;
                p.adminLevel = 0;
                p.hp = 200;
                p.Strength = 30;
                p.charComps = "[]";
                p.stats = "[]";
                p.sex = 0;
                p.oldCar = 0;
                p.isPM = true;
                p.isNews = true;
                p.injured = new InjuredModel();
                p.isCuffed = false;
                p.melee = null;
                p.secondary = null;
                p.primary = null;
                p.maxHp = 1000;
            }
            catch
            {

            }

            Core.Logger.WriteLogData(Logger.logTypes.ip, "[RELOG] " + p.characterName);

            p.RemoveAllWeapons();

            p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>" +
                "<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>" +
                "<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>" +
                "<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>" +
                "<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>" +
                "<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");



            Main.PlayerConnect(p, "Relog");
        }

        [Command("sendrelog")]
        public void COM_RelogSend(PlayerModel a, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(a, "[用法] /sendrelog [id]"); return; }
            if (a.adminLevel <= 6) { MainChat.SendErrorChat(a, "[错误] 无权限操作!"); return; }
            if (!Int32.TryParse(args[0], out int tSql)) { MainChat.SendInfoChat(a, "[用法] /sendrelog [id]"); return; }

            PlayerModel p = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (!p.Exists || p == null)
            {
                MainChat.SendErrorChat(a, "[错误] 无效玩家!");
                return;
            }

            foreach (var data in p.GetAllDataKeys())
            {
                if (p.Exists)
                {
                    if (p.HasData(data))
                        p.DeleteData(data);
                    else if (p.HasStreamSyncedMetaData(data))
                        p.DeleteStreamSyncedMetaData(data);
                    else if (p.HasSyncedMetaData(data))
                        p.DeleteSyncedMetaData(data);
                }
            }

            p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>" +
                "<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>" +
                "<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>" +
                "<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>" +
                "<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>" +
                "<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");

            if (p.Exists)
                outRp.ServerEvents.OnPlayerDisconnect(p, "Relog");

            p.sqlID = 0;
            p.showSqlId = true;
            p.accountId = 0;
            p.characterName = "未登录玩家";
            p.characterAge = 0;
            p.characterExp = 0;
            p.characterLevel = 0;
            p.gameTime = 1;
            p.factionId = 0;
            p.factionRank = 0;
            p.RadioFreq = 1;
            p.businessStaff = 0;
            p.cash = 0;
            p.bankCash = 0;
            p.Strength = 30;
            p.adminLevel = 0;
            p.isCk = false;
            p.jailTime = 0;
            p.phoneNumber = 0;
            p.firstLogin = false;
            p.isCuffed = false;
            p.stats = "[]";
            p.adminJail = 0;
            p.settings = "[]";
            //Data Keys  
            p.adminWork = false;
            p.adminLevel = 0;
            p.hp = 200;
            p.Strength = 30;
            p.charComps = "[]";
            p.stats = "[]";
            p.sex = 0;
            p.oldCar = 0;
            p.isPM = true;
            p.isNews = true;
            p.injured = new InjuredModel();
            p.isCuffed = false;
            p.melee = null;
            p.secondary = null;
            p.primary = null;
            p.maxHp = 1000;
            Main.PlayerConnect(p, "Relog");


        }


        [Command("showinfos")]
        public async Task COM_ShowInfos(PlayerModel p)
        {
            AccountModel acc = await Database.DatabaseMain.getAccInfo(p.accountId);
            if (acc == null) { MainChat.SendErrorChat(p, "[错误] 获取账号信息出错, 请联系管理员! 错误代码 0xACC0002"); return; }
            if (acc.OtherData.Info.Count > 0)
            {
                string text = "<center>{2d90a9}[{FFFFFF}🔎通知🔍{2d90a9}]{FFFFFF}</center>";
                foreach (var a in acc.OtherData.Info)
                {
                    text += "<br>📌{FF1C00}" + a.Title + "{FFFFFF}[" + acc.OtherData.Info.IndexOf(a) + "]<br>🗓{8DFF00}" + a.Date.ToString("yyyy/MM/dd HH:mm");
                }
                text += "<br>/getinfo [id]";
                text += "<br>💰{FFFFFF}您有 {FFB800}" + acc.OtherData.Refunds.Count + "{FFFFFF} 条待接受的补偿/退款, 输入 /showrefunds 查看";
                p.SendChatMessage(text);
            }
            else
            {
                p.SendChatMessage("{2d90a9}[服务器] {FFFFFF}无任何通知.");
            }
        }

        [Command("getinfo")]
        public async Task COM_ShowInfo(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /getinfo [id]"); return; }
            if (!Int32.TryParse(args[0], out int Index)) { MainChat.SendInfoChat(p, "[用法] /getinfo [id]"); return; }
            AccountModel acc = await Database.DatabaseMain.getAccInfo(p.accountId);
            if (acc == null) { MainChat.SendErrorChat(p, "[错误] 获取账号信息出错, 请联系管理员! 错误代码 0xACC0005"); return; }
            var info = acc.OtherData.Info[Index];
            if (info == null) { MainChat.SendErrorChat(p, "[错误] 指定ID通知不存在!"); return; }
            p.SendChatMessage("📌{77FF00}" + info.Title + "<br>" + info.Body + "<br>🗓{8DFF00}" + info.Date.ToString("yyyy/MM/dd HH:mm"));
            return;
        }

        [Command("clearinfos")]
        public async Task COM_ClearInfos(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /clearinfos [ID]"); return; }
            if (!Int32.TryParse(args[0], out int Index)) { MainChat.SendInfoChat(p, "[用法] /clearinfos [ID]"); return; }
            AccountModel acc = await Database.DatabaseMain.getAccInfo(p.accountId);
            if (acc != null)
            {

                acc.OtherData.Info.RemoveAt(Index);
                await acc.Update();
                MainChat.SendInfoChat(p, "[?] 已清理通知, 输入 /showinfos 查看最新通知列表");
                return;
            }

            MainChat.SendErrorChat(p, "[错误] 获取账号信息出错, 请联系管理员! 错误代码 0xACC0001");
        }

        [Command("showrefunds")]
        public async Task COM_ShowRefunds(PlayerModel p)
        {
            AccountModel acc = await Database.DatabaseMain.getAccInfo(p.accountId);
            if (acc == null) { MainChat.SendErrorChat(p, "[错误] 获取账号信息出错, 请联系管理员! 错误代码 0xACC0003"); return; }
            string text = "<center>{66FF00}Bekleyen Ödemeleriniz</center>";
            if (acc.OtherData.Refunds.Count > 0)
            {
                foreach (var r in acc.OtherData.Refunds)
                {
                    text += "<br>💱{00FFA6}[" + r.Title + " | ID " + acc.OtherData.Refunds.IndexOf(r) + "] {FFFFFF}" + r.Body + "<br>日期: " + r.Date.ToString("yyyy/MM/dd HH:dd") + " | 现金: $" + r.Cash;
                }
                text += "<br></center>接受此账单, 请输入 {77FF00}/{FFFFFF}dealrefund</center>";
            }
            else
            {
                text += "{FFFFFF}无补偿/退款单.";
            }
            p.SendChatMessage(text);
            return;
        }

        [Command("dealrefund")]
        public async Task COM_GetRefund(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /dealrefund [补偿/退款单ID]"); return; }
            if (!Int32.TryParse(args[0], out int Index)) { MainChat.SendInfoChat(p, "[用法] /dealrefund [补偿/退款单ID]"); return; }
            AccountModel acc = await Database.DatabaseMain.getAccInfo(p.accountId);
            if (acc == null) { MainChat.SendErrorChat(p, "[错误] 获取账号信息出错, 请联系管理员! 错误代码 0xACC0004"); return; }
            var refund = acc.OtherData.Refunds[Index];
            if (refund == null) { MainChat.SendErrorChat(p, "[错误] 无效补偿/退款."); return; }
            p.cash += refund.Cash;
            acc.OtherData.Refunds.Remove(refund);
            await acc.Update();
            MainChat.SendInfoChat(p, "[?] 您接受了 " + refund.Title + " 补偿/退款单, 内含游戏现金: $" + refund.Cash);
            await p.updateSql();
        }

        [Command("mykeys")]
        public async Task COM_ShowKeys(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /mykeys [veh/house/biz]"); return; }
            string text = "<center>我的钥匙 [" + args[0] + "]</center>";
            switch (args[0])
            {
                case "veh":
                    var vehKeys = await Database.DatabaseMain.GetOwnedKeys(p.sqlID, 0);
                    foreach (var vk in vehKeys)
                    {
                        VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(vk);
                        if (v == null)
                            continue;
                        var model = (VehicleModel)v.Model;
                        text += "<br>[🚙][" + v.sqlID + "]" + model.ToString() + " 车牌号码: " + v.NumberplateText + " 税: " + v.fine + " [ 输入 /agps " + v.sqlID + " 可查找位置 ]";
                    }
                    break;

                case "house":
                    var houseKeys = await Database.DatabaseMain.GetOwnedKeys(p.sqlID, 1);
                    foreach (var hk in houseKeys)
                    {
                        var h = await Props.Houses.getHouseById(hk);
                        if (h.Item1 == null)
                            continue;

                        text += "<br>[🏠][" + h.Item1.ID + "]" + h.Item1.name + " 价格: $" + h.Item1.price + " 税: " + h.Item1.settings.Tax + " [ 输入 /hgps " + h.Item1.ID + " 可查找位置 ]";
                    }
                    break;

                case "biz":
                    var bizKeys = await Database.DatabaseMain.GetOwnedKeys(p.sqlID, 2);
                    foreach (var bk in bizKeys)
                    {
                        var b = await Props.Business.getBusinessById(bk);
                        if (b.Item1 == null)
                            continue;

                        text += "<br>[🏢][" + b.Item1.ID + "]" + b.Item1.name + " 价格: $" + b.Item1.price + " 税: " + b.Item1.settings.Tax + " [ 输入 /agps " + b.Item1.ID + " 可查找位置 ]";
                    }
                    break;

                default:
                    MainChat.SendInfoChat(p, "[用法] /mykeys [veh/house/biz]"); return;
            }

            MainChat.SendInfoChat(p, text, true);
        }

        [Command("decals")]
        public void COM_AddDecal(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /decals [id] [纹理ID(如果可用)]<br> 如果删除请输入:<br>/decals none"); return; }
            if (args[0].ToLower() == "none") { GlobalEvents.SetClothes(p, 10, 0, 0); MainChat.SendInfoChat(p, "[?] 已移除角色贴画."); return; }
            if (args.Length > 1)
            {
                if (!Int32.TryParse(args[0], out int cID) || !Int32.TryParse(args[1], out int tID)) { MainChat.SendInfoChat(p, "[用法] /decals [id] [纹理ID(如果可用)]<br> 如果删除请输入:<br>/decals none"); return; }
                GlobalEvents.SetClothes(p, 10, cID, tID);
                MainChat.SendInfoChat(p, "[?] 成功设置角色贴画.");
            }
            else
            {
                if (!Int32.TryParse(args[0], out int cID)) { MainChat.SendInfoChat(p, "[用法] /decals [id] [纹理ID(如果可用)]<br> 如果删除请输入:<br>/decals none"); return; }
                GlobalEvents.SetClothes(p, 10, cID, 0);
                MainChat.SendInfoChat(p, "[?] 成功设置角色贴画.");
            }
        }

        [Command("fixinv")]
        public void COM_EnvFix(PlayerModel p)
        {
            p.EmitLocked("Inv:CanUseYes");
        }

        //CMD NOT FOUND
        /*[CommandEvent(CommandEventType.CommandNotFound)]
        public static void CMDNotFound(PlayerModel player, string cmd)
        {
            player.SendChatMessage(("{49A0CD}[服务器]{FFFFFF} 很抱歉, 您输入的指令 '/" + cmd + "' 不存在, 请输入 /help 查看指令!");
        }*/

        [Command("anc")]
        public void Anchor(PlayerModel p)
        {
            if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] 您不在船上."); return; }
            var veh = (VehModel)p.Vehicle;
            veh.BoatAnchor = !veh.BoatAnchor;
            MainChat.SendInfoChat(p, "[?] 成功操作船锚状态为: " + ((veh.BoatAnchor) ? "开启" : "关闭"));
        }

        [Command("nametagfix")]
        public async Task COM_TagFix(PlayerModel p)
        {
            if (p.nameTagFix > DateTime.Now) { MainChat.SendErrorChat(p, "[错误] 您可以每 10 分钟使用一次这个指令."); return; }
            MainChat.SendInfoChat(p, "[?] 已重启名称标签.");
            var dim = p.Dimension;
            p.Dimension = p.Id;
            p.EmitLocked("nametag:Restart");

            await Task.Delay(1200);
            p.Dimension = dim;
            MainChat.SendInfoChat(p, "[?] 名称标签重启完毕.");
            p.nameTagFix = DateTime.Now.AddMinutes(10);
            return;
        }


        [Command("timestamp")]
        public void COM_TimeStamp(PlayerModel p)
        {
            p.EmitLocked("chat:timeStamp");
        }

        [Command("flatbed")]
        public void COM_Flat(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendErrorChat(p, "[用法] /flatbed [车辆ID]"); return; }
            if (!Int32.TryParse(args[0], out int tsql)) { MainChat.SendErrorChat(p, "[用法] /flatbed [车辆ID]"); return; }
            var setting = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            if (!setting.hasFlatbed) { MainChat.SendErrorChat(p, "[错误] 无权使用此指令, 因为您没有 FlatBed 权限!"); return; }
            if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] 您不在车内!"); return; }
            if (p.Vehicle.Model != (uint)VehicleModel.Flatbed) { MainChat.SendErrorChat(p, "[错误] 您必须在 FlatBed 模型车内!"); return; }
            var veh = (VehModel)p.Vehicle;
            var tVeh = Vehicle.VehicleMain.getVehicleFromSqlId(tsql);

            if (tVeh == null) { MainChat.SendErrorChat(p, "[错误] 无效车辆!"); return; }
            if (tVeh.Position.Distance(veh.Position) > 10) { MainChat.SendErrorChat(p, "[错误] 指定车辆离您的车辆太远了!"); return; }

            if (tVeh.HasData("FlatBed"))
            {
                var pos = veh.Position;
                pos.X += 3;
                tVeh.Position = pos;
                tVeh.Detach();
                tVeh.Position = pos;
                tVeh.Repair();
                tVeh.DeleteData("FlatBed");
                MainChat.SendInfoChat(p, "[?] 成功卸载 FlatBed 上的车辆.");
            }
            else
            {
                tVeh.AttachToEntity(veh, 0, 0, new Position(0, -10, 4), new Position(0, 0, 0), true, false);
                tVeh.SetData("FlatBed", true);
                tVeh.Repair();
                MainChat.SendInfoChat(p, "[?] 成功使用 FlatBed 运载车辆.");
            }
        }

        [Command("sellcar")]
        public void COM_SellTabela(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendErrorChat(p, "[用法] /sellcar [价格] - 如果您要取消挂牌出售, 重新使用 /sellcar 将价格设置为 0 即可."); return; }
            if (!Int32.TryParse(args[0], out int fiyat)) { MainChat.SendErrorChat(p, "[用法] /sellcar [价格]  - 如果您要取消挂牌出售, 重新使用 /sellcar 将价格设置为 0 即可."); return; }

            var setting = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            if (!setting.hasGallery) { MainChat.SendErrorChat(p, "[错误] 无权使用此指令, 因为您没有 挂牌出售 权限!"); return; }
            if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] 您必须在车内!"); return; }

            var veh = (VehModel)p.Vehicle;
            veh.sellPrice = fiyat;

            MainChat.SendInfoChat(p, "[?] 成功挂牌出售您的车辆.");
        }

        [Command("appce")]
        public async void COM_setCharDetail(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendErrorChat(p, "[用法] /appce [外貌特征]"); return; }
            foreach (string msg in args) { msg.Replace("&lt;", "<").Replace("&#39", "'"); }

            CharacterSettings detail = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            if (detail == null) detail = new CharacterSettings();

            string text = string.Join(" ", args);
            text = text.Replace("&lt;", "<").Replace("&#39", "'");
            detail.charDetail = text;
            p.settings = JsonConvert.SerializeObject(detail);
            await p.updateSql();
            MainChat.SendInfoChat(p, "成功设置您的角色外貌特征.");
            
            if (p.isFinishTut == 18)
            {
                p.isFinishTut = 19;
                p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}基本指令");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}嘿, 您成功设置了角色外貌特征!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}现在, 让我们看看我们角色的外貌描述吧!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}现在, 请尝试使用 {fc5e03}/ca " + p.sqlID + " 指令吧");
                // GlobalEvents.CheckpointCreate(player, Tutorial_Main.TutPos[1], 44, 1, new Rgba(255, 255, 255, 100), "Tutorial:Run", "11");
            }
        }

        [Command("ca")]
        public async void COM_getCharDetail(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendErrorChat(p, "[用法] /ca [ID]"); return; }
            if (!Int32.TryParse(args[0], out int targetID)) { MainChat.SendInfoChat(p, "[用法] /ca [ID]"); return; }

            PlayerModel targetPlayer = GlobalEvents.GetPlayerFromSqlID(targetID);
            if (targetPlayer == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家!"); return; }

            if (targetPlayer.Position.Distance(p.Position) > 3) { MainChat.SendErrorChat(p, "[错误] 指定玩家离您太远."); return; }

            CharacterSettings set = JsonConvert.DeserializeObject<CharacterSettings>(targetPlayer.settings);

            string text = "{006dff}<center>" + targetPlayer.characterName.Replace("_", " ") + " 的外貌特征</center><br>{ffffff}" + set.charDetail;
            MainChat.SendInfoChat(p, text, true);
            
            if (p.isFinishTut == 19)
            {
                p.isFinishTut = 20;
                p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}基本指令");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}嘿, 您成功查看了自己的角色外貌特征!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}现在, 让我们前往下一个教程点吧!");
                GlobalEvents.CheckpointCreate(p, TutorialMain.TutPos[5], 1, 2, new Rgba(255, 255, 255, 100), "Tutorial:Run", "20");
            }
        }

        [Command("graffiti")]
        public void COM_ShowGraffitiMenu(PlayerModel p)
        {
            if (p.isGraffiti == false) {
                MainChat.SendErrorChat(p, "无权操作");
                return; 
            }

            p.EmitLocked("graffitix:graffitix");
        }
        
        [Command("boombox")]
        public void COM_BoomBox(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /boombox [选项]<br>可用选项:<br>play[播放] - remove[收回] - vol[音量0.0-1.0]<br>例如:/boombox play 音乐链接"); return; }
            Boombox.BoomBoxModel x = Boombox.serverBoomBoxs.Find(x => p.Position.Distance(x.boxProp.Position) < 5 && x.boxProp.Dimension == p.Dimension);
            if (x == null) { MainChat.SendErrorChat(p, "> 附近没有音响."); return; }
            if (x.ID != p.sqlID) { MainChat.SendErrorChat(p, "> 此音响不属于您"); return; }
            switch (args[0])
            {
                case "play":
                    if (args[1] == null) { MainChat.SendErrorChat(p, "无效音乐链接."); return; }
                    string link = args[1];
                    x.link = link;
                    Alt.Emit("xear:playAudio", new Vector3(x.boxProp.Position.X, x.boxProp.Position.Y, x.boxProp.Position.Z), 
                        link, 1.0, true, true);
                    
                    MainChat.SendInfoChat(p, "成功播放音乐，注意，默认是单曲循环的，切换其他音乐再次使用/boombox play即可");
                    MainChat.EmoteDo(p, "此人点击了音响的播放键并播放了歌曲.");
                    return;

                case "remove":
                    if(x.link != "none") { Alt.Emit("xear:stopAudio", new Vector3(x.boxProp.Position.X, x.boxProp.Position.Y, x.boxProp.Position.Z), x.link); }
                    x.boxProp.Delete();
                    x.boxLabel.Delete();
                    x.link = "none";
                    Boombox.serverBoomBoxs.Remove(x);
                    MainChat.EmoteDo(p, "此人按下音响的暂停键并收回了音响.");
                    return;

                case "vol":
                    if (x.link == "none") { MainChat.SendErrorChat(p, "无已播放的音乐"); return; }
                    if (args[1] == null) { MainChat.SendInfoChat(p, "无效音量."); return; }
                    double vol; bool isOk = double.TryParse(args[1], out vol);
                    if (!isOk) { MainChat.SendInfoChat(p, "无效音量."); return; }
                    if (vol > 1.0 || vol < 0.0) { MainChat.SendInfoChat(p, "音量为0.0-1.0"); return; }
                    x.Volume = vol;
                    Alt.Emit("xear:setAudioVolume", new Vector3(x.boxProp.Position.X, x.boxProp.Position.Y, x.boxProp.Position.Z), x.link, x.Volume);
                    MainChat.EmoteDo(p, "此人蹲下并调整音响的音量大小为 " + vol);
                    return;
            }
        }

        [Command("bmusic")]
        public async Task COM_InteriorMusic(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /bmusic [选项] [音乐链接]<br>可用选项:<br>play[播放] - remove[收回] - vol[音量0.0-1.0]<br>例如:/bmusic play 音乐链接"); return; }
            //PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Find(x => p.Position.Distance(x.Position) < 35f && x.Dimension == p.Dimension);
            PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Where(x => p.Position.Distance(x.Position) <= 50 && x.Dimension == p.Dimension).OrderBy(x => p.Position.Distance(x.Position)).FirstOrDefault();
            if (entranceLabel == null) { MainChat.SendInfoChat(p, "[错误] 您需要在产业内饰的出口附近"); return; }
            if (!entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position pos))
                return;

            (BusinessModel, Marker, PlayerLabel) biz = await Props.Business.getBusinessFromPos(pos);
            if (biz.Item1 == null)
                return;

            if (!await Props.Business.CheckBusinessKey(p, biz.Item1))
            {
                MainChat.SendErrorChat(p, "[错误] 此产业不属于您!");
                return;
            }


            switch (args[0])
            {
                case "play":
                    if (args[1] == null) { MainChat.SendErrorChat(p, "无效音乐链接."); return; }
                    string link = args[1];
                    biz.Item1.settings.musicUrl = link;
                    
                    biz.Item1.settings.musicStartTime = DateTime.Now;

                    Alt.Emit("xear:playAudio",
                        new Vector3(biz.Item1.interiorPosition.X, biz.Item1.interiorPosition.Y,
                            biz.Item1.interiorPosition.Z),
                        biz.Item1.settings.musicUrl, 1.0, true, true, false, false);
                        
                    Alt.Log(new Vector3(biz.Item1.interiorPosition.X, biz.Item1.interiorPosition.Y, biz.Item1.interiorPosition.Z).ToString());
                    
                    Alt.Emit("xear:playAudio", new Vector3(biz.Item1.position.X, biz.Item1.position.Y, biz.Item1.position.Z), 
                        biz.Item1.settings.musicUrl, 1.0, true, true, true, false);                    

                    await biz.Item1.Update(biz.Item2, biz.Item3);
                
                    MainChat.SendInfoChat(p, "成功播放音乐，注意，默认是单曲循环的，切换其他音乐再次使用/bmusic play即可");
                    MainChat.EmoteDo(p, "此人在产业内饰播放了歌曲.");
                    return;

                case "vol":
                    if (biz.Item1.settings.musicUrl == "none") { MainChat.SendErrorChat(p, "无已播放的音乐."); return; }
                    if (args[1] == null) { MainChat.SendErrorChat(p, "无效音量."); return; }
                    double newVolume; bool isOk = double.TryParse(args[1], out newVolume);
                    if (!isOk) { MainChat.SendInfoChat(p, "无效音量."); return; }
                    if (newVolume > 1.0 || newVolume < 0.0) { MainChat.SendInfoChat(p, "音量为0.0-1.0"); return; }                    

                    biz.Item1.settings.musicSound = newVolume;
                    Alt.Emit("xear:setAudioVolume",
                        new Vector3(biz.Item1.interiorPosition.X, biz.Item1.interiorPosition.Y, biz.Item1.interiorPosition.Z), 
                                    biz.Item1.settings.musicUrl, biz.Item1.settings.musicSound);
                    
                    Alt.Emit("xear:setAudioVolume",
                        new Vector3(biz.Item1.position.X, biz.Item1.position.Y, biz.Item1.position.Z), 
                        biz.Item1.settings.musicUrl, biz.Item1.settings.musicSound);                    

                    await biz.Item1.Update(biz.Item2, biz.Item3);

                    MainChat.EmoteDo(p, "此人调整产业内饰音乐的音量大小为 " + newVolume);
                    return;

                case "remove":
                    if(biz.Item1.settings.musicUrl != "none") 
                    { 
                        Alt.Emit("xear:stopAudio", 
                        new Vector3(biz.Item1.interiorPosition.X, biz.Item1.interiorPosition.Y, biz.Item1.interiorPosition.Z), 
                        biz.Item1.settings.musicUrl); 
                        
                        Alt.Emit("xear:stopAudio", 
                            new Vector3(biz.Item1.position.X, biz.Item1.position.Y, biz.Item1.position.Z), 
                            biz.Item1.settings.musicUrl);                         
                    }
                    
                    biz.Item1.settings.musicUrl = "none";
                    await biz.Item1.Update(biz.Item2, biz.Item3);
                    MainChat.EmoteDo(p, "此人暂停了产业内饰的音乐.");
                    return;
            }
        }

        [Command("hmusic")]
        public async Task COM_InteriorMusicHouse(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /hmusic [选项] [音乐链接]<br>可用选项:<br>play[播放] - remove[收回] - vol[音量0.0-1.0]<br>例如:/hmusic play 音乐链接"); return; }
            //PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Find(x => p.Position.Distance(x.Position) < 15f && x.Dimension == p.Dimension);
            PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Where(x => p.Position.Distance(x.Position) <= 50 && x.Dimension == p.Dimension).OrderBy(x => p.Position.Distance(x.Position)).FirstOrDefault();
            if (entranceLabel == null) { MainChat.SendInfoChat(p, "[错误] 您需要在产业内饰的出口附近"); return; }
            if (!entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position pos))
                return;

            (HouseModel, PlayerLabel, Marker) biz = await Props.Houses.getHouseFromPos(pos);
            if (biz.Item1 == null)
                return;

            if (!await Props.Houses.HouseKeysQuery(p, biz.Item1))
            {
                MainChat.SendErrorChat(p, "[错误] 此房屋不属于您!");
                return;
            }


            switch (args[0])
            {
                case "play":
                    if (args[1] == null) { MainChat.SendErrorChat(p, "无效音乐链接."); return; }
                    string link = args[1];
                    biz.Item1.settings.musicUrl = link;
                    biz.Item1.settings.musicStartTime = DateTime.Now;
                    
                    Alt.Emit("xear:playAudio", new Vector3(biz.Item1.intPos.X, biz.Item1.intPos.Y, biz.Item1.intPos.Z), 
                        biz.Item1.settings.musicUrl, 1.0, true, true, false, false);
                    
                    Alt.Log(new Vector3(biz.Item1.intPos.X, biz.Item1.intPos.Y, biz.Item1.intPos.Z).ToString());
                    
                    Alt.Emit("xear:playAudio", new Vector3(biz.Item1.pos.X, biz.Item1.pos.Y, biz.Item1.pos.Z), 
                        biz.Item1.settings.musicUrl, 1.0, true, true, true, false);                
                    biz.Item1.Update(biz.Item3, biz.Item2);

                    MainChat.SendInfoChat(p, "成功播放音乐，注意，默认是单曲循环的，切换其他音乐再次使用/hmusic play即可");
                    MainChat.EmoteDo(p, "此人在房屋内饰播放了歌曲.");
                    return;

                case "vol":
                    if (biz.Item1.settings.musicUrl == "none") { MainChat.SendErrorChat(p, "无已播放的音乐."); return; }
                    if (args[1] == null) { MainChat.SendErrorChat(p, "无效音量."); return; }
                    double newVolume; bool isOk = double.TryParse(args[1], out newVolume);
                    if (!isOk) { MainChat.SendInfoChat(p, "无效音量."); return; }
                    if (newVolume > 1.0 || newVolume < 0.0) { MainChat.SendInfoChat(p, "音量为0.0-1.0"); return; }    

                    biz.Item1.settings.musicSound = newVolume;
                    
                    Alt.Emit("xear:setAudioVolume",
                        new Vector3(biz.Item1.intPos.X, biz.Item1.intPos.Y, biz.Item1.intPos.Z), 
                        biz.Item1.settings.musicUrl, biz.Item1.settings.musicSound); 
                    
                    Alt.Emit("xear:setAudioVolume",
                        new Vector3(biz.Item1.pos.X, biz.Item1.pos.Y, biz.Item1.pos.Z), 
                        biz.Item1.settings.musicUrl, biz.Item1.settings.musicSound);                     

                    biz.Item1.Update(biz.Item3, biz.Item2);
                    MainChat.EmoteDo(p, "此人调整房屋内饰音乐的音量大小为 " + newVolume);
                    return;

                case "remove":
                    if (biz.Item1.settings.musicUrl != "none")
                    {
                        Alt.Emit("xear:stopAudio",
                            new Vector3(biz.Item1.intPos.X, biz.Item1.intPos.Y, biz.Item1.intPos.Z),
                            biz.Item1.settings.musicUrl);
                        
                        Alt.Emit("xear:stopAudio",
                            new Vector3(biz.Item1.pos.X, biz.Item1.pos.Y, biz.Item1.pos.Z),
                            biz.Item1.settings.musicUrl);                        
                    }

                    biz.Item1.settings.musicUrl = "none";
                    biz.Item1.Update(biz.Item3, biz.Item2);
                    MainChat.EmoteDo(p, "此人暂停了房屋内饰的音乐.");
                    return;
            }
        }

        [Command("vradio", aliases: new string[] { "vmusic" })]
        public void COM_CarRadio(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /vradio [选项] [数值]<br>可用选项:<br>play[播放] - remove[收回] - vol[音量0.0-1.0]<br>例如:/vradio play 音乐链接"); return; }
            if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] 您必须在车内."); return; }

            VehModel v = (VehModel)p.Vehicle;
            if (v == null)
                return;


            switch (args[0])
            {
                case "play":
                    string link = args[1];
                    v.radioLink = link;
                    Alt.Emit("xear:playAudio", v, v.radioLink, 1.0, true, true);
                    v.SetStreamSyncedMetaData("Vehicle:Radio:Link", v.radioLink);
                    MainChat.EmoteDo(p, "此人按下了车载音响的播放键.");
                    return;

                case "vol":
                    if (!v.HasStreamSyncedMetaData("Vehicle:Radio:Link")) { MainChat.SendErrorChat(p, "[错误] 车载音响没有播放音乐."); return; }
                    if (args[1] == null) { MainChat.SendInfoChat(p, "[?] 音量为10-50."); return; }
                    double volume; bool isVolumeOk = double.TryParse(args[1], out volume);
                    if (!isVolumeOk || volume < 0.0 || volume > 1.0) { MainChat.SendInfoChat(p, "[?] 车载音量为0.0-1.0"); return; }

                    Alt.Emit("xear:setAudioVolume", v, v.radioLink, volume); 

                    MainChat.EmoteDo(p, "此人调整了车载音响的音量为 " + volume);
                    return;

                case "remove":
                    if (v.radioLink == "none") { MainChat.SendErrorChat(p, "[错误] 车载音响没有播放音乐."); return; }
                    Alt.Emit("xear:stopAudio", v, v.radioLink); 

                    v.DeleteStreamSyncedMetaData("Vehicle:Radio:Link");
                    MainChat.EmoteDo(p, "此人关闭了车载音响.");
                    return;

                case "adminvol":
                    if (p.adminLevel <= 1)
                        return;
                    if (!v.HasStreamSyncedMetaData("Vehicle:Radio:Link")) { MainChat.SendErrorChat(p, "[错误] 车载音响没有播放音乐."); return; }
                    if (args[1] == null) { MainChat.SendInfoChat(p, "[?] 音量为10-100."); return; }
                    double volume2; bool isVolumeOk2 = double.TryParse(args[1], out volume2);
                    if (!isVolumeOk2 || volume2 < 0.0 || volume2 > 1.0) { MainChat.SendInfoChat(p, "[?] 音量为0.0-1.0"); return; }

                    Alt.Emit("xear:setAudioVolume", v, v.radioLink, volume2);
                    return;

            }


        }        
    }
}
