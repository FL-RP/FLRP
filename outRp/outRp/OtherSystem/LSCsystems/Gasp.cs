using AltV.Net;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Core;
using outRp.Globals;
using outRp.Models;
using System;
using AltV.Net.Resources.Chat.Api;

namespace outRp.OtherSystem.LSCsystems
{
    public class Gasp : IScript
    {

        [Command("gasp")]
        public static void COM_Gasp(PlayerModel player, params string[] args)
        {
            if (Jacking.TotalPDGroup() < 4) { MainChat.SendErrorChat(player, "[错误] 服务器至少需要有 4 名执法组织成员在线, 您才可以敲诈勒索."); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(player, "[用法] /gaspet [id]"); return; }
            if (player.characterLevel < 5) { MainChat.SendErrorChat(player, "[错误] 您的账号至少达到 5 级才能使用此指令."); return; }

            CharacterSettings set = JsonConvert.DeserializeObject<CharacterSettings>(player.settings);
            if (set.GaspUsage > DateTime.Now) { MainChat.SendErrorChat(player, "[错误] 敲诈勒索冷却时间! 剩余时间: " + ((int)(set.GaspUsage - DateTime.Now).TotalMinutes) + "分钟"); return; }

            if (!Int32.TryParse(args[0], out int targetId)) { MainChat.SendInfoChat(player, "[用法] /gaspet [id]"); return; }
            if (targetId == player.sqlID) { MainChat.SendErrorChat(player, "[错误] 无法敲诈勒索自己 :)"); return; }
            var target = GlobalEvents.GetPlayerFromSqlID(targetId);
            if (target == null) { MainChat.SendErrorChat(player, "[错误] 无效玩家."); return; }
            if (target.Position.Distance(player.Position) > 3) { MainChat.SendErrorChat(player, "[错误] 您离玩家太远."); return; }

            var targetSet = JsonConvert.DeserializeObject<CharacterSettings>(target.settings);
            if (targetSet.Gasped > DateTime.Now) { MainChat.SendErrorChat(player, "[错误] 指定玩家在过去 48 小时内已被勒索(请遵守服务器规则, 虽然这违反现实, 但是为了更好的环境, 我们不得不这么做)."); return; }

            target.SetData("Gasp", player.sqlID);
            MainChat.SendInfoChat(player, "[?] 您向 " + target.fakeName.Replace('_', ' ') + " 发送了敲诈勒索请求.");
            MainChat.SendInfoChat(target, "[?] " + player.fakeName.Replace('_', ' ') + " 向您发送了敲诈勒索请求, 输入 /regasp acp 接受 & 输入 /regasp dec 拒绝");
        }

        [Command("regasp")]
        public async void COM_GaspResp(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /regasp acp 接受 & /regasp dec 拒绝"); return; }
            if (args[0] == "acp")
            {
                if (!p.HasData("Gasp")) { MainChat.SendErrorChat(p, "[错误] 无勒索请求!"); return; }
                var _tID = p.lscGetdata<int>("Gasp");
                var target = GlobalEvents.GetPlayerFromSqlID(_tID);
                if (target == null) { MainChat.SendErrorChat(p, "[错误] 无效请求者, 可能离线了."); p.DeleteData("Gasp"); return; }
                if (target.Position.Distance(p.Position) > 20) { MainChat.SendErrorChat(p, "[错误] 勒索者离您太远, 系统已关闭勒索请求(( 但勒索过的事情是存在的, 拒绝请求或系统关闭不能说明您和勒索者没有过交际 ))."); MainChat.SendErrorChat(target, "[错误] 您离勒索者太远, 系统已关闭勒索请求(( 但勒索过的事情是存在的, 拒绝请求或系统关闭不能说明您和勒索者没有过交际 ))."); p.DeleteData("Gasp"); return; }
                var targetSet = JsonConvert.DeserializeObject<CharacterSettings>(target.settings);
                if (targetSet.GaspUsage > DateTime.Now) { MainChat.SendErrorChat(p, "[错误] 此人的勒索次数已满(( 但勒索过的事情是存在的, 拒绝请求或系统关闭不能说明您和勒索者没有过交际 ))."); MainChat.SendErrorChat(target, "[错误] 您的勒索次数已满(( 但勒索过的事情是存在的, 拒绝请求或系统关闭不能说明您和勒索者没有过交际 ))."); p.DeleteData("Gasp"); return; }


                int price;
                if (p.cash >= 1500) price = 1500;
                else if (p.cash <= 0) { MainChat.SendErrorChat(target, "[错误] 系统已关闭敲诈勒索, 因为您没有足够的钱(( 但勒索过的事情是存在的, 拒绝请求或系统关闭不能说明您和勒索者没有过交际 ))."); MainChat.SendErrorChat(p, "[错误] 系统已关闭敲诈勒索, 因为对方没有足够的钱(( 但勒索过的事情是存在的, 拒绝请求或系统关闭不能说明您和勒索者没有过交际 ))."); p.DeleteData("Gasp"); return; }
                else price = p.cash;

                target.cash += price;
                p.cash -= price;

                var pSet = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
                pSet.Gasped = DateTime.Now.AddHours(48);
                p.settings = JsonConvert.SerializeObject(pSet);
                targetSet.GaspUsage = DateTime.Now.AddHours(12);
                target.settings = JsonConvert.SerializeObject(targetSet);

                MainChat.ADO(target, "试图威胁着 " + p.fakeName.Replace('_', ' '));

                MainChat.SendInfoChat(p, "[?] 您接受了敲诈勒索(( 请注意全时态扮演 )).");
                MainChat.SendInfoChat(target, "[?] " + p.fakeName.Replace('_', ' ') + " 接受了您的敲诈勒索, 获得: $" + price + " (( 请注意全时态扮演 ))");
                //MainChat.EmoteMe(target, " ellerini " + p.fakeName.Replace('_', ' ') + "'in üzerinde gezdirerek, bulduğu paraları alır.");
                await p.updateSql();
                await target.updateSql();
                return;
            }
            else
            {
                if (!p.HasData("Gasp")) { MainChat.SendErrorChat(p, "[错误] 无勒索请求!"); return; }
                var _tID = p.lscGetdata<int>("Gasp");
                var target = GlobalEvents.GetPlayerFromSqlID(_tID);
                if (target == null) { MainChat.SendErrorChat(p, "[错误] 无效请求者, 可能离线了."); p.DeleteData("Gasp"); return; }
                MainChat.SendInfoChat(p, "[!] 您拒绝了敲诈勒索请求(( 请注意全时态扮演 )).");
                MainChat.SendInfoChat(target, "[!] " + p.fakeName.Replace('_', ' ') + " 拒绝了您的敲诈勒索请求(( 请注意全时态扮演 )).");
                p.DeleteData("Gasp");
                return;
            }

        }

        [Command("resetgasp")]
        public async void ResetGasp(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 5) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /resetgasp [id]"); return; }
            if (!Int32.TryParse(args[0], out int tsql)) { MainChat.SendInfoChat(p, "[用法] /resetgasp [id]"); return; }
            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(tsql);
            if (target == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家!"); return; }
            var set = JsonConvert.DeserializeObject<CharacterSettings>(target.settings);
            set.Gasped = DateTime.Now.AddDays(-10);
            set.GaspUsage = DateTime.Now.AddDays(-10);
            target.settings = JsonConvert.SerializeObject(set);
            await target.updateSql();
            MainChat.SendInfoChat(p, "[=] 您重置了 " + target.characterName.Replace('_', ' ') + " 的敲诈勒索.");
            MainChat.SendInfoChat(target, "[?] 您的敲诈勒索状态被重置.");
            return;
        }
    }
}
