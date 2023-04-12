using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net; using AltV.Net.Async;
using AltV.Net.Resources.Chat.Api;
using outRp.Models;
using outRp.Chat;
using outRp.Globals;
using Newtonsoft.Json;

namespace outRp.OtherSystem.LSCsystems
{
    public class FactionPage : IScript
    {
        [Command("faction")]
        public async Task ShowFactionPage(PlayerModel p)
        {
            if(p.factionId <= 0) { MainChat.SendErrorChat(p, CONSTANT.ERR_FNotFound); return; }
            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            switch (fact.type)
            {
                case 1: fact.typeName = "合法"; break;
                case 2: fact.typeName = "非法"; break;
                case 3: fact.typeName = "执法"; break;
                case 4: fact.typeName = "FD/医院"; break;
                case 5: fact.typeName = "市政当局/律师事务所"; break;
                case 6: fact.typeName = "新闻电视台/报纸媒体"; break;
                case 7: fact.typeName = "车党/帮派"; break;
            }
            List<FactionUserModel> fUsers = await Database.DatabaseMain.GetFactionMembers(p.factionId);
            int factionCars = 0;
            foreach(VehModel v in Alt.GetAllVehicles())
            {
                if(v.factionId == p.factionId) { factionCars += 1; }
            }
            string factJson = JsonConvert.SerializeObject(fact);
            string fUsersJson = JsonConvert.SerializeObject(fUsers);
            p.EmitLocked("FactionBrowser:Show", factJson, factionCars, fUsersJson);
            return;
        }
        
        public static async Task UpdateFactionPage(PlayerModel p)
        {
            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            List<FactionUserModel> fUsers = await Database.DatabaseMain.GetFactionMembers(p.factionId);
            int factionCars = 0;

            foreach (VehModel v in Alt.GetAllVehicles())
            {
                if (v.factionId == p.factionId) { factionCars += 1; }
            }
            switch (fact.type)
            {
                case 1: fact.typeName = "合法"; break;
                case 2: fact.typeName = "非法"; break;
                case 3: fact.typeName = "执法"; break;
                case 4: fact.typeName = "FD/医院"; break;
                case 5: fact.typeName = "市政当局/律师事务所"; break;
                case 6: fact.typeName = "新闻电视台/报纸媒体"; break;
                case 7: fact.typeName = "车党/帮派"; break;
            }
            string factJson = JsonConvert.SerializeObject(fact);
            string fUsersJson = JsonConvert.SerializeObject(fUsers);
            p.EmitLocked("FactionBrowser:Update", factJson, factionCars, fUsersJson);
            return;
        }

        // FPage Listeners

        [AsyncClientEvent("FactionBrowser:UpdateMemberRank")]
        public async Task EditMemberRank(PlayerModel p, int targetSql, int rankId)
        {
            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            var perm = fact.rank.Find(x => x.Rank == p.factionRank);
            if(perm == null)
            {
                if(p.sqlID != fact.owner) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            }
            else
            {                
                if (fact.owner != p.sqlID)
                {
                    if(perm.permission.canUseRank == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
                }
            }

            var newRank = fact.rank.Find(x => x.Rank == rankId);
            if(newRank == null) {return; }

            string info = "{0} 将 {1} 的阶级更改为 {2}.";

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(targetSql);
            if(t == null)
            {
                PlayerModelInfo target = await Database.DatabaseMain.getCharacterInfo(targetSql);
                if(target == null) { return; }
                target.factionRank = newRank.Rank;
                target.updateSql();
                
                foreach (PlayerModel coT in Alt.GetAllPlayers())
                {
                    if (coT.factionId == p.factionId)
                    {
                        MainChat.FactionChat(coT, string.Format(info, p.characterName.Replace("_", " "), target.characterName.Replace("_", " "), newRank.RankName));
                    }
                }
                UpdateFactionPage(p);
                return;
            }
            t.factionRank = newRank.Rank;
            t.updateSql();
            foreach (PlayerModel cT in Alt.GetAllPlayers())
            {
                if (cT.factionId == p.factionId)
                {
                    MainChat.FactionChat(cT, string.Format(info, p.characterName.Replace("_", " "), t.characterName.Replace("_", " "), newRank.RankName));
                }
            }
            
            UpdateFactionPage(p);
            return;
        }
        
        [AsyncClientEvent("FactionBrowser:KickPlayer")]
        public async Task KickMemberFromFaction(PlayerModel p, int targetSql)
        {
            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            var perm = fact.rank.Find(x => x.Rank == p.factionRank);
            if (perm == null)
            {
                if (p.sqlID != fact.owner) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            }
            else
            {
                if (p.factionId != fact.owner)
                {                    
                    if (!perm.permission.canUseKick) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
                }
            }
            if(targetSql == fact.owner) { return; }
            string info = "{0} 将 {1} 踢出了组织.";

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(targetSql);
            if(t == null)
            {
                PlayerModelInfo target = await Database.DatabaseMain.getCharacterInfo(targetSql);
                if(target == null) { return; }
                target.factionId = 0;
                target.factionRank = 0;
                target.updateSql();
                MainChat.FactionChat(p, string.Format(info, p.characterName.Replace("_", " "), target.characterName.Replace("_", " ")));
                UpdateFactionPage(p);
                return;
            }

            MainChat.FactionChat(p, string.Format(info, p.characterName.Replace("_", " "), t.characterName.Replace("_", " ")));
            t.factionRank = 0;
            t.factionId = 0;
            UpdateFactionPage(p);
            return;
        }

        [AsyncClientEvent("FactionBrowser:EditRank")]
        public async Task UpdateCreateFactionRank(PlayerModel p, string arg)
        {            
            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            if (fact.owner != p.sqlID) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); UpdateFactionPage(p); return; }
            //int id = Int32.Parse(i);
            //int payday = Int32.Parse(pay);
            FactionRank nRank = JsonConvert.DeserializeObject<FactionRank>(arg);
            if (nRank.Rank != 0)
            {
                var x = fact.rank.Find(x => x.Rank == nRank.Rank);
                if (x == null) { return; }
                x.RankName = nRank.RankName;
                x.Payday = nRank.Payday;
                x.permission.canUseCar = nRank.permission.canUseCar;
                x.permission.canUseVault = nRank.permission.canUseVault;
                x.permission.canUsePayday = nRank.permission.canUsePayday;
                x.permission.canUseInvite = nRank.permission.canUseInvite;
                x.permission.canUseKick = nRank.permission.canUseKick;
                x.permission.canUseRank = nRank.permission.canUseRank;
                fact.Update();
                UpdateFactionPage(p);
                return;
            }
            
                              
            nRank.Rank = fact.settings.rankCounter;
            fact.settings.rankCounter += 1;
            fact.rank.Add(nRank);
            
            fact.Update();
            UpdateFactionPage(p);
            return;
        }

        [AsyncClientEvent("FactionBrowser:RemoveRank")]
        public async Task RemoveFactionRank(PlayerModel p, int rankId)
        {
            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            if (fact.owner != p.sqlID) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); UpdateFactionPage(p); return; }

            var x = fact.rank.Find(x => x.Rank == rankId);
            fact.rank.Remove(x);
            fact.Update();
            UpdateFactionPage(p);
            return;
        }

        [AsyncClientEvent("FactionBrowser:OOCChat")]
        public async Task ChangeFactionOOCState(PlayerModel p, bool state)
        {
            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            if (fact.owner != p.sqlID) { UpdateFactionPage(p); return; }

            if(fact.settings.OOCChat != state)
            {
                fact.settings.OOCChat = state;
                string stateText = (state) ? "开启的" : "关闭的";
                string info = "组织OOC频道是" + stateText;
                MainChat.FactionChat(p, info);
                fact.Update();
                return;
            }
            else { return; }
        }

        [AsyncClientEvent("FactionBrowser:UpdateMOTD")]
        public async Task ChangeFactionMotd(PlayerModel p, string header, string body)
        {
            if (p.factionId <= 0) { MainChat.SendErrorChat(p, "[错误] 您没有组织."); return; }
            if (header.Length <= 3 || body.Length <= 3) { MainChat.SendErrorChat(p, "[错误] 标题或内容的长度必须至少为 3 个字符."); return; }
            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            if( fact == null) { MainChat.SendErrorChat(p, "[错误] 无效组织."); return; }
            var check = fact.settings.Motd.Find(x => x.Header == header);
            if(check != null) { MainChat.SendErrorChat(p, "[错误] 您不能在同一标题上创建多个组织每日信息."); return; }
            if (p.sqlID != fact.owner) { MainChat.SendErrorChat(p, "[错误] 您不是组织领导人."); return; }
            fact.settings.Motd.Add(new()
            {
                Header = header,
                Body = body,
                Sender = p.characterName.Replace("_", " "),
                Time = DateTime.Now
            });

            fact.Update();
            MainChat.SendInfoChat(p, "[?] 已更新组织每日信息.");
            return;
        }

        [AsyncClientEvent("FactionBrowser:DeleteMOTD")]
        public async Task DeleteFactionMotdU(PlayerModel p, string header)
        {
            if (p.factionId <= 0) { MainChat.SendErrorChat(p, "[错误] 您没有组织!"); return; }
            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            if (fact == null) { MainChat.SendErrorChat(p, "[错误] 无效组织."); return; }
            var check = fact.settings.Motd.Find(x => x.Header == header);
            if (check == null) { MainChat.SendErrorChat(p, "[错误] 无效每日信息!"); return; }
            fact.settings.Motd.Remove(check);
            fact.Update();

            MainChat.SendInfoChat(p, "[?] 已删除组织每日信息.");
            return;
        }
    }
}
