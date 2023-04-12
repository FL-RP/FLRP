using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using outRp.Chat;
using outRp.Globals;
using outRp.Models;
using outRp.OtherSystem.NativeUi;
using outRp.OtherSystem.Textlabels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;

namespace outRp.Factions
{
    public class Faction : IScript
    {
        public static void LoadFactionSystem()
        {
            TextLabelStreamer.Create(ServerGlobalValues.createFactionLabelText, ServerGlobalValues.createFactionPos, center: true, font: 0, streamRange: 5);
            GlobalEvents.blipModel factBlip = new GlobalEvents.blipModel();
            factBlip.blipname = "createFactionBlip";
            factBlip.sprite = 491;
            factBlip.category = 2;
            factBlip.label = "组织创立点";
            factBlip.position = ServerGlobalValues.createFactionPos;
            GlobalEvents.serverBlips.Add(factBlip);
            Alt.Log("加载 组织系统.");
        }
        public class FactionInviteModel
        {
            public int sender { get; set; }
            public int target { get; set; }
        }

        public static List<FactionInviteModel> FactionInvites = new List<FactionInviteModel>();

        public static async Task<bool> CheckAccountIllegalFaction(PlayerModel p, int wantToJoinFaction)
        {
            bool canJoin = true;
            List<AccountCharacterModel> characters = await Database.DatabaseMain.getAccountCharacters(p.accountId);
            foreach (AccountCharacterModel c in characters)
            {
                if (c.factionId == 0)
                    continue;

                FactionModel checkFact = await Database.DatabaseMain.GetFactionInfo(c.factionId);
                if (checkFact.type != ServerGlobalValues.fType_Legal && checkFact.ID != wantToJoinFaction)
                {
                    return false;
                }
            }

            return canJoin;
        }

        [Command(CONSTANT.COM_CreateFaction)]
        public static async Task CreateFaction(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 2)
            //if (p.Position.Distance(ServerGlobalValues.createFactionPos) > 5) { MainChat.SendErrorChat(p, "[错误] Bu komutu kullanabilmek için birlik bölgesinde olmalısınız"); return; }
            if (args.Length <= 1) { MainChat.SendInfoChat(p, CONSTANT.DESC_CreateFaction); return; }
            //if (await CheckAccountIllegalFaction(p, -1) == false) { MainChat.SendErrorChat(p, "[错误] 您的账号已经有其他."); return; }

            //int type = Int32.Parse(args[0]);
            if (!Int32.TryParse(args[0], out int type)) { MainChat.SendInfoChat(p, CONSTANT.DESC_CreateFaction); return; }
            if (type != 1) { MainChat.SendInfoChat(p, CONSTANT.DESC_CreateFaction); return; }
            string name = string.Join(" ", args[1..]);
            //if (p.cash < ServerGlobalValues.FactionCreatePrice) { MainChat.SendErrorChat(p, CONSTANT.ERR_FCashError); return; }
            if (p.factionId != 0) { MainChat.SendErrorChat(p, CONSTANT.ERR_FInFact); return; }
            FactionModel fact = new FactionModel();
            fact.name = name;
            fact.type = type;
            fact.owner = p.sqlID;
            fact.factionExp = 1;
            fact.factionLevel = 1;
            p.factionId = await fact.Create();
            p.factionRank = 1;
            //p.cash -= ServerGlobalValues.FactionCreatePrice;
            await p.updateSql();

            GlobalEvents.UINotifiy(p, 5, "~b~" + name, message: " 创建成功.", time: 10000);
        }

        [Command(CONSTANT.COM_InviteFaction)]
        public static async Task InviteFaction(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendErrorChat(p, "[用法] /finvite [玩家ID]"); return; }

            int sqlid; bool isIdOK = Int32.TryParse(args[0], out sqlid);
            if (!isIdOK) { MainChat.SendErrorChat(p, "[用法] /finvite [玩家ID]"); return; }
            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(sqlid);
            if (target == null)
                return;

            if (p.factionId <= 0) { MainChat.SendErrorChat(p, CONSTANT.ERR_FNotFound); return; }
            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            FactionRank pFactPermission = fact.rank.Find(x => x.Rank == p.factionRank);
            if (pFactPermission == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (!pFactPermission.permission.canUseInvite) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (await CheckAccountIllegalFaction(target, p.factionId) == false) { MainChat.SendErrorChat(p, "[错误] 您邀请的玩家有其他角色正在其他组织中, 所以您无法邀请它."); return; }

            FactionInvites.Add(new FactionInviteModel { target = target.Id, sender = p.Id });

            MainChat.SendErrorChat(target, $"{p.characterName.Replace("_", " ")} 邀请您加入组织. <br>您可以在40秒内输入 /faccept 接受邀请, 否则自动拒绝.");
            MainChat.SendErrorChat(p, "已邀请 " + $"{target.characterName.Replace("_", " ")} 加入组织, 请等待对方回应.");

            await Task.Delay(40000);
            var removeInvıte = FactionInvites.Find(r => r.sender == p.Id);
            if (removeInvıte != null) { FactionInvites.Remove(removeInvıte); }
            return;
        }
        [Command(CONSTANT.COM_AcceptFactionInvite)]
        public static async Task FactionInvıteAnswer(PlayerModel player)
        {
            if (player.factionId > 0) { MainChat.SendErrorChat(player, $"您已经有组织了."); return; }
            var itemRemove = FactionInvites.Find(r => r.target == player.Id);
            if (itemRemove == null) { MainChat.SendErrorChat(player, $"无有效的组织邀请."); return; }

            PlayerModel sender = GlobalEvents.GetPlayerFromId(itemRemove.sender);
            if (sender == null) { /*Alt.Log("HATA");*/ return; }
            player.factionId = sender.factionId;
            player.factionRank = 0;
            await player.updateSql();


            FactionChatSendInfo(sender, $"{player.characterName.Replace("_", " ")} 接受了 {sender.characterName.Replace("_", " ")} 的组织邀请.");
            return;
        }
        [Command(CONSTANT.COM_KickFaction)]
        public static async Task FactionKickPlayer(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendErrorChat(p, "[用法] /fkick [玩家ID]"); return; }
            if (p.factionId <= 0) { MainChat.SendErrorChat(p, CONSTANT.ERR_FNotFound); return; }

            int sqlid = Int32.Parse(args[0]);
            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(sqlid);
            if (target == null)
                return;

            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            FactionRank pFactPermission = fact.rank.Find(x => x.Rank == p.factionRank);
            if (p.sqlID != fact.owner)
            {
                if (pFactPermission == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
                else
                {
                    if (pFactPermission.permission.canUseKick == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
                }
            }


            if (target == null) { MainChat.SendErrorChat(p, $"{sqlid} 无效ID"); return; }
            if (target.factionId != p.factionId) { MainChat.SendErrorChat(p, $"[错误] {target.characterName} 与您不在一个组织."); return; }

            target.factionId = 0;
            target.factionRank = 0;
            await target.updateSql();

            MainChat.SendErrorChat(target, $"[信息] {p.characterName} 将您踢出了组织.");
            FactionChatSendInfo(p, $"{target.characterName.Replace("_", " ")} 被 {p.characterName.Replace("_", " ")} 踢出了组织.");
            return;
        }

        public static void FactionRankPlayer(PlayerModel player, int sqlId, int Rank)
        {
            //CharacterModel playerChar = GlobalEvents.PlayerGetData(player);
            if (player.factionId <= 0) { MainChat.SendErrorChat(player, CONSTANT.ERR_FNotFound); return; }
            if (player.factionRank < 7) { MainChat.SendErrorChat(player, CONSTANT.ERR_PermissionError); return; }

            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(sqlId);

            if (target == null) { MainChat.SendErrorChat(player, $"{sqlId} 无效组织ID"); return; }

            //CharacterModel targetChar = GlobalEvents.PlayerGetData(target);

            if (target.factionRank > player.factionRank) { MainChat.SendErrorChat(player, CONSTANT.ERR_PermissionError); return; }

            if (target.factionId != player.factionId) { MainChat.SendErrorChat(player, $"[错误] {target.characterName.Replace("_", " ")} 与您不在一个组织."); return; }

            target.factionRank = Rank;
            //GlobalEvents.playerSetData(target, targetChar);

            MainChat.SendInfoChat(target, $"[信息] {player.characterName.Replace("_", " ")} 更新了您的阶级.");
            MainChat.SendInfoChat(player, $"[信息] 已更新 {target.characterName.Replace("_", " ")} 的阶级.");
            return;
        }

        [Command(CONSTANT.COM_FChat)]
        public static async Task FactionChat(PlayerModel player, params string[] args)
        {
            if (args.Length <= 0) { return; }
            string message = string.Join(" ", args);
            if (player.factionId <= 0) { MainChat.SendErrorChat(player, CONSTANT.ERR_FNotFound); return; }
            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(player.factionId);
            if (!fact.settings.OOCChat && fact.owner != player.sqlID) { MainChat.SendErrorChat(player, CONSTANT.ERR_FChatDisablet); return; }
            string rank = GetFactionRankString(player.factionRank, fact);
            string[] nameParts = player.characterName.Split("_");
            string playername = nameParts[0] + " " + nameParts[1];

            foreach (PlayerModel target in Alt.GetAllPlayers())
            {
                if (target.factionId == player.factionId)
                {
                    MainChat.FactionChat(target, $"{fact.name} | {rank} | {playername}: {message}");
                }
            }
            return;
        }

        public static void FactionChatSendInfo(PlayerModel player, string message)
        {
            if (player.factionId < 1) { MainChat.SendErrorChat(player, $"您没有组织"); return; }

            foreach (PlayerModel target in Alt.GetAllPlayers())
            {

                if (target.factionId == player.factionId)
                {
                    MainChat.FactionChat(target, message);
                }
            }
            return;
        }

        public static void FactionChatSendInfoWithFactionID(int ID, string message)
        {
            foreach (PlayerModel target in Alt.GetAllPlayers())
            {

                if (target.factionId == ID)
                {
                    MainChat.FactionChat(target, message);
                }
            }
        }
        public static string GetFactionRankString(int rank, FactionModel fact)
        {
            string RankString = "无";
            var x = fact.rank.Find(x => x.Rank == rank);
            if (x == null) { return RankString; }
            RankString = x.RankName;
            return RankString;
        }

        public static async Task FactionDepartmentChatNear(PlayerModel player, string message)
        {
            FactionModel playerFaction = await Database.DatabaseMain.GetFactionInfo(player.factionId);

            if (player == null) { MainChat.SendErrorChat(player, CONSTANT.CharacterNotFoundError); return; }
            if (player.factionId <= 0) { MainChat.SendErrorChat(player, CONSTANT.CharacterNotFoundError); return; }

            List<int> GovermentFactionId = await Database.DatabaseMain.GetGovermentFactionIds();
            bool succes = false;
            foreach (int pId in GovermentFactionId)
            {
                if (player.factionId == pId) { succes = true; break; }
            }
            if (succes == false) { MainChat.SendErrorChat(player, CONSTANT.ERR_FNotFound); return; }

            string[] name = player.characterName.Split('_');
            string rank = GetFactionRankString(player.factionRank, playerFaction);
            string sendingMessage = "{EFE986} [" + playerFaction.name + "]" + rank + "  <i class='fad fa-user - circle'></i>" + name[0] + " " + name[1] + ": " + message;

            var onlinePlayers = Alt.GetAllPlayers();


            foreach (PlayerModel target in onlinePlayers)
            {
                //CharacterModel targetChar = GlobalEvents.PlayerGetData(target);
                foreach (int id in GovermentFactionId)
                {
                    if (target.factionId == id)
                    {
                        if (target.Position.Distance(player.Position) < 120)
                            MainChat.DepartmentChat(target, sendingMessage);
                    }

                }
            }
            return;
        }

        public static async Task FactionDepartmentChat(PlayerModel player, string message)
        {
            FactionModel playerFaction = await Database.DatabaseMain.GetFactionInfo(player.factionId);

            if (player == null) { MainChat.SendErrorChat(player, CONSTANT.CharacterNotFoundError); return; }
            if (player.factionId <= 0) { MainChat.SendErrorChat(player, CONSTANT.CharacterNotFoundError); return; }

            List<int> GovermentFactionId = await Database.DatabaseMain.GetGovermentFactionIds();
            bool succes = false;
            foreach (int pId in GovermentFactionId)
            {
                if (player.factionId == pId) { succes = true; break; }
            }
            if (succes == false) { MainChat.SendErrorChat(player, CONSTANT.ERR_FNotFound); return; }

            string[] name = player.characterName.Split('_');
            string rank = GetFactionRankString(player.factionRank, playerFaction);
            string sendingMessage = " [" + playerFaction.name + "]" + rank + "  <i class='fad fa-user - circle'></i>" + name[0] + " " + name[1] + ": " + message;

            var onlinePlayers = Alt.GetAllPlayers();


            foreach (PlayerModel target in onlinePlayers)
            {
                //CharacterModel targetChar = GlobalEvents.PlayerGetData(target);
                foreach (int id in GovermentFactionId)
                {
                    if (target.factionId == id)
                    {
                        MainChat.DepartmentChat(target, sendingMessage);
                    }

                }
            }
            return;
        }

        [Command(CONSTANT.COM_FOnline)]
        public static async Task COM_FOnline(PlayerModel p)
        {
            if (p.factionId < 1) { MainChat.SendErrorChat(p, $"您没有组织"); return; }
            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(p.factionId);

            //string colorList = "";

            p.SendChatMessage("<center>组织成员列表</center>");
            int counter = 0;
            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                /*if(t.factionId == p.factionId)
                {
                    counter++;
                    if(fact.type == ServerGlobalValues.fType_FD || fact.type == ServerGlobalValues.fType_PD || fact.type == ServerGlobalValues.fType_MD)
                    {
                        colorList = (t.HasData(EntityData.PlayerEntityData.PDDuty)) ? "{069CF2}" : "{9C9C9C}";
                        colorList = (t.HasData(EntityData.PlayerEntityData.FDDuty) ? "{069CF2}" : "{9C9C9C}");
                    }

                    

                    string rank = GetFactionRankString(t.factionRank, fact);
                    p.SendChatMessage(colorList + "[" + rank + "] (" + t.sqlID + ")" + t.characterName.Replace("_", " ") + " GSM:" + t.phoneNumber);
                }*/
                if (t.factionId == p.factionId)
                {
                    counter++;
                    string rank = GetFactionRankString(t.factionRank, fact);
                    p.SendChatMessage((t.HasData(EntityData.PlayerEntityData.PDDuty) ? "{0C94F2}" : "") + ((t.HasData(EntityData.PlayerEntityData.FDDuty) ? "{F20C2F}" : ""))
                        + (t.HasData(EntityData.PlayerEntityData.NewsDuty) ? "{DAE76C}" : "") + ((t.HasData(EntityData.PlayerEntityData.MDDuty) ? "{ED89AD}" : "")) +
                        "[" + rank + "] (" + t.sqlID + ") " + t.characterName.Replace('_', ' ') + ((t.phoneNumber > 0) ? "☎联系电话: " + t.phoneNumber : ""));
                }
            }
            p.SendChatMessage("{EECE1A}在线组织成员: " + counter.ToString() + " | " + fact.name);
            return;
        }

        [Command("frespawn")]
        public static async Task COM_FactionCarsRespawn(PlayerModel p)
        {
            if (p.factionId <= 0) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(p.factionId);

            if (fact.owner != p.sqlID) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            foreach (VehModel v in Alt.GetAllVehicles())
            {
                if (v.factionId == p.factionId)
                {
                    if (v.settings.savePosition.Distance(new Position(0, 0, 0)) > 1)
                    {
                        if (v.Driver == null)
                        {
                            await v.SetPositionAsync(v.settings.savePosition);
                            await v.SetRotationAsync(v.settings.saveRotation);
                            await v.SetDimensionAsync(v.settings.SaveDimension);
                        }
                    }
                }
            }

            if (!p.Exists)
                return;

            MainChat.SendInfoChat(p, "已刷新组织车辆.");
        }

        [Command("fshowcars")]
        public static void COM_ShowFactionCars(PlayerModel p)
        {
            if (p.factionId <= 0) { MainChat.SendErrorChat(p, "[错误] 您没有组织!"); return; }

            string text = "<center>查看组织车辆</center>";
            foreach (VehModel v in Alt.GetAllVehicles())
            {
                if (v.factionId == p.factionId)
                {
                    text += "[" + v.sqlID + "]模型: " + ((VehicleModel)v.Model).ToString() + " | 车牌: " + v.NumberplateText + " | 税: " + v.fine + "<br>";
                }
            }

            MainChat.SendInfoChat(p, text, true);
        }

        [Command("finfo")]
        public async Task COM_FactionInfo(PlayerModel p, params string[] args)
        {
            if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /finfo [标题(空格_)] [文本]"); return; }
            if (p.factionId <= 0) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(p.factionId);

            if (fact.owner != p.sqlID) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            string Title = "{FFB800}[组织信息 - " + fact.name + "]{FF1C00}" + args[0].Replace('_', ' ');
            string Body = string.Join(" ", args[1..]);
            DateTime date = DateTime.Now;

            foreach (var u in await Database.DatabaseMain.GetFactionMembers(fact.ID))
            {
                await GlobalEvents.AddAccountInfo(u.ID, Title, Body, date);
            }

            MainChat.SendInfoChat(p, "[?] 致所有组织成员 - " + args[0].Replace('_', ' ') + " 标题 - " + string.Join(" ", args[1]));
            return;
        }

        [Command("fequip")]
        public async Task COM_EditFactionEquiptment(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /fequip <br>[参数] add - delete - addgun - addguncomp - resetgun - addtint - armor - img - color"); return; }
            if (p.factionId <= 0) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(p.factionId);

            if (fact.owner != p.sqlID) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            switch (args[0])
            {
                case "add":
                    if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /fequip add [名称(空格_)]"); return; }
                    var addCheck = fact.settings.Equipments.Find(x => x.name == string.Join(" ", args[1..]));
                    if (addCheck != null) { MainChat.SendErrorChat(p, "[错误] 无可用装备."); return; }
                    fact.settings.Equipments.Add(new()
                    {
                        name = String.Join(" ", args[1..]),
                    });
                    fact.Update();
                    MainChat.SendInfoChat(p, "[?] " + fact.name + " 添加了装备 " + String.Join(" ", args[1..]));
                    return;

                case "delete":
                    if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /fequip delete [名称]"); return; }
                    string removeName = String.Join(" ", args[1..]);
                    var removeCheck = fact.settings.Equipments.Find(x => x.name == removeName);
                    if (removeCheck == null) { MainChat.SendErrorChat(p, "[错误] 无效装备!"); return; }
                    fact.settings.Equipments.Remove(removeCheck);
                    fact.Update();
                    MainChat.SendInfoChat(p, "[?] " + fact.name + " 删除了装备 " + removeName);
                    return;

                case "addgun":
                    if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /fequip addgun [名称(空格_)] [武器HASH]"); return; }
                    var addWeaponCheck = fact.settings.Equipments.Find(x => x.name == args[1].Replace('_', ' '));
                    if (addWeaponCheck == null) { MainChat.SendErrorChat(p, "[错误] 无效武器."); return; }
                    if (!uint.TryParse(args[2], out uint addWeaponID)) { MainChat.SendInfoChat(p, "[用法] /fequip addgun [名称(空格_)] [武器HASH]"); return; }
                    addWeaponCheck.weapon.Add(new()
                    {
                        weapon = addWeaponID
                    });
                    fact.Update();

                    MainChat.SendErrorChat(p, "[?] " + fact.name + "->" + addWeaponCheck.name);
                    return;

                case "addguncomp":
                    if (args.Length <= 2) { MainChat.SendErrorChat(p, "[用法] /fequip addguncomp [名称(空格_)] [武器ID] [配件ID]"); return; }
                    var addWeaponCompCheck = fact.settings.Equipments.Find(x => x.name == args[1].Replace('_', ' '));
                    if (addWeaponCompCheck == null) { MainChat.SendErrorChat(p, "[用法] /fequip addguncomp [名称(空格_)] [武器ID] [配件ID]"); return; }
                    if (!uint.TryParse(args[2], out uint addCompWeaponID) || !uint.TryParse(args[3], out uint addCompCompId)) { MainChat.SendErrorChat(p, "[用法] /fequip addguncomp [名称(空格_)] [武器ID] [配件ID]"); return; }
                    var checkCompWeapon = addWeaponCompCheck.weapon.Find(x => x.weapon == addCompWeaponID);
                    if (checkCompWeapon == null) { MainChat.SendErrorChat(p, "[错误] 无可用配件!"); return; }
                    checkCompWeapon.Components.Add(addCompCompId);

                    fact.Update();
                    MainChat.SendInfoChat(p, "[?] " + fact.name + "->" + addWeaponCompCheck.name + "->" + checkCompWeapon.weapon);
                    return;

                case "resetgun":
                    if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /fequip resetgun [名称(空格_)] [武器ID]"); return; }
                    var resetWeaponCheck = fact.settings.Equipments.Find(x => x.name == args[1].Replace('_', ' '));
                    if (resetWeaponCheck == null) { MainChat.SendInfoChat(p, "[用法] /fequip resetgun [名称(空格_)] [武器ID]"); return; }
                    if (!uint.TryParse(args[2], out uint resetWeaponId)) { MainChat.SendInfoChat(p, "[用法] /fequip resetgun [名称(空格_)] [武器ID]"); return; }

                    var _resetWeaponCheck = resetWeaponCheck.weapon.Find(x => x.weapon == resetWeaponId);
                    if (_resetWeaponCheck == null) { MainChat.SendErrorChat(p, "[错误] 无效武器."); return; }

                    _resetWeaponCheck = new FactionEquipmentWeapon();
                    fact.Update();

                    MainChat.SendInfoChat(p, "[?] " + fact.name + "->" + resetWeaponCheck.name + "->" + _resetWeaponCheck.weapon);
                    return;

                case "addtint":
                    if (args.Length <= 2) { MainChat.SendInfoChat(p, "[用法] /fequip addtint [名称(空格_)] [tint 1-7]"); return; }
                    var tintWeaponClass = fact.settings.Equipments.Find(x => x.name == args[1].Replace('_', ' '));
                    if (tintWeaponClass == null) { MainChat.SendErrorChat(p, "[错误] 无效装备!"); return; }
                    if (!uint.TryParse(args[2], out uint tintWeapon) || !byte.TryParse(args[3], out byte tintByte)) { MainChat.SendInfoChat(p, "[用法] /fequip addtint [名称(空格_)] [tint 1-7]"); return; }
                    var _tint = tintWeaponClass.weapon.Find(x => x.weapon == tintWeapon);
                    if (_tint == null) { MainChat.SendErrorChat(p, "[错误] 无效装备."); return; }
                    _tint.tint = tintByte;

                    fact.Update();
                    MainChat.SendInfoChat(p, "[?] " + fact.name + "->" + tintWeaponClass.name + "->" + _tint.weapon + "'a " + tintByte);
                    return;

                case "armor":
                    if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /fequip armor [名称(空格_)] [护甲数值]"); return; }
                    var armorClass = fact.settings.Equipments.Find(x => x.name == args[1].Replace('_', ' '));
                    if (armorClass == null) { MainChat.SendErrorChat(p, "[错误] 无效装备!"); return; }
                    if (!Int32.TryParse(args[2], out int setArmor)) { MainChat.SendInfoChat(p, "[用法] /fequip armor [名称(空格_)] [护甲数值]"); return; }
                    armorClass.Armor = setArmor;

                    fact.Update();

                    MainChat.SendInfoChat(p, "[?] " + fact.name + "->" + armorClass.name + " 护甲 " + setArmor);
                    return;

                case "img":
                    if (args.Length < 1) { MainChat.SendErrorChat(p, "[用法] /fequip img [link(url)]"); return; }
                    fact.settings.equipmentUrl[0] = args[1];

                    fact.Update();
                    MainChat.SendInfoChat(p, "[?] 已更新装备图片.");
                    return;

                case "color":
                    if (args.Length < 1) { MainChat.SendErrorChat(p, "[用法] /fequip color [标志颜色(#fff000)]"); return; }
                    fact.settings.equipmentUrl[1] = args[1];

                    fact.Update();
                    MainChat.SendInfoChat(p, "[?] 已更新装备颜色.");
                    return;

                default:
                    MainChat.SendInfoChat(p, "[用法] /fequip <br>[参数] add - delete - addgun - addguncomp - resetgun - addtint - armor - img - color");
                    return;
            }
        }

        [Command("equip")]
        public async Task COM_ShowEquipmentList(PlayerModel p)
        {
            if (p.factionId <= 0) { MainChat.SendErrorChat(p, "[错误] 您没有组织."); return; }
            VehModel veh = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (veh == null) { MainChat.SendErrorChat(p, "[错误] 附近没有车辆!"); return; }
            if (veh.factionId != p.factionId) { MainChat.SendErrorChat(p, "[错误] 此车不属于您的组织!"); return; }

            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            if (fact == null) { MainChat.SendErrorChat(p, "[错误] 无效组织."); return; }
            if (fact.settings.Equipments.Count <= 0) { MainChat.SendErrorChat(p, "[错误] 无装备."); return; }

            if (fact.type == 3 || fact.type == 4)
            {
                if (!p.HasData(EntityData.PlayerEntityData.PDDuty))
                    if (!p.HasData(EntityData.PlayerEntityData.FDDuty))
                    { MainChat.SendErrorChat(p, "[错误] 请先执勤."); return; }
            }

            List<GuiMenu> gMenu = new List<GuiMenu>();
            //GuiMenu clothes1 = new GuiMenu { name = "Devriye Ekipmanı(Pistol)", triger = "PD:Equiptment", value = "memur1" };
            fact.settings.Equipments.ForEach(x =>
            {
                gMenu.Add(new()
                {
                    name = x.name,
                    triger = "Equipment:Take",
                    value = x.name
                });
            });

            Gui y = new Gui()
            {
                image = fact.settings.equipmentUrl[0],
                guiMenu = gMenu,
                color = fact.settings.equipmentUrl[1]
            };
            y.Send(p);
        }

        [AsyncClientEvent("Equipment:Take")]
        public async Task Equipment_Take(PlayerModel p, string name)
        {
            if (p.Ping > 250)
                return;
            p.EmitLocked("GUI:Close");
            if (p.factionId <= 0) { MainChat.SendErrorChat(p, "[错误] 您没有组织."); return; }
            VehModel veh = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (veh == null) { MainChat.SendErrorChat(p, "[错误] 附近没有车辆!"); return; }
            if (veh.factionId != p.factionId) { MainChat.SendErrorChat(p, "[错误] 此车不属于您的组织!"); return; }

            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            if (fact == null) { MainChat.SendErrorChat(p, "[错误] 无效组织."); return; }
            if (fact.settings.Equipments.Count <= 0) { MainChat.SendErrorChat(p, "[错误] 无装备."); return; }

            var checkEq = fact.settings.Equipments.Find(x => x.name == name);
            if (checkEq == null) { MainChat.SendErrorChat(p, "[错误] 无效装备."); return; }

            p.RemoveAllWeapons();

            checkEq.weapon.ForEach(x =>
            {
                p.GiveWeapon(x.weapon, 200, false);
                p.SetWeaponTintIndex(x.weapon, x.tint);
                x.Components.ForEach(y =>
                {
                    p.AddWeaponComponentAsync(x.weapon, y);
                });
            });

            await p.SetArmorAsync((ushort)checkEq.Armor);

            MainChat.SendInfoChat(p, "[?] 您取出了 " + checkEq.name);
            return;
        }

/*        [Command("fkasayatir")]
        public async Task DepositFactionModey(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /fkasayatir [miktar]"); return; }
            if (!Int32.TryParse(args[0], out int money)) { MainChat.SendInfoChat(p, "[用法] /fkasayatir [miktar]"); return; }
            if (p.factionId <= 0) { MainChat.SendErrorChat(p, "[错误] Bu komutu kullanabilmek için bir birlikte olmalısınız."); return; }

            if (p.cash < money) { MainChat.SendErrorChat(p, "[错误] Yeterli paranız yok!"); return; }
            if (money <= 0) { MainChat.SendInfoChat(p, "[用法] /fkasayatir [miktar]"); return; }
            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            if (fact == null) { MainChat.SendErrorChat(p, "[错误] Birlik getirilirken bir hata meydana geldi."); return; }

            p.cash -= money;
            await p.updateSql();

            fact.cash += money;
            fact.Update();

            FactionChatSendInfo(p, p.characterName.Replace('_', ' ') + " isimli oyuncu birlik kasasına $" + money + " yatırdı.");
            return;
        }

        [Command("fkasacek")]
        public async Task WithdrawFactionMoney(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /fkasacek [miktar]"); return; }
            if (!Int32.TryParse(args[0], out int money)) { MainChat.SendInfoChat(p, "[用法] /fkasacek [miktar]"); return; }
            if (p.factionId <= 0) { MainChat.SendErrorChat(p, "[错误] Bu komutu kullanabilmek için bir birlikte olmalısınız."); return; }

            if (money <= 0) { MainChat.SendInfoChat(p, "[用法] /fkasacek [miktar]"); return; }
            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            if (fact == null) { MainChat.SendErrorChat(p, "[错误] Birlik getirilirken bir hata meydana geldi."); return; }
            if (fact.cash < money) { MainChat.SendInfoChat(p, "[错误] Birlik kasasında yeterli para yok!"); return; }
            var rank = fact.rank.Find(x => x.Rank == p.factionRank);
            if (fact.owner != p.sqlID && !rank.permission.canUseVault) { MainChat.SendErrorChat(p, "[错误] Bu komutu sadece birlik lideri veya kasa yetkisi olanlar kullanabilir. "); return; }

            p.cash += money;
            await p.updateSql();

            fact.cash -= money;
            fact.Update();

            Core.Logger.WriteLogData(Core.Logger.logTypes.moneyTransfer, "[Birlik] " + fact.name + " isimlik birlik kasasından " + p.characterName + " isimli oyuncuya $" + money + " çekildi.");

            FactionChatSendInfo(p, p.characterName.Replace('_', ' ') + " isimli oyuncu birlik kasasından $" + money + " çekti.");
            return;
        }*/

        [Command("fr")]
        public async Task FactionRadio(PlayerModel p, params string[] args)
        {
            if (p.factionId <= 0) { MainChat.SendErrorChat(p, "[错误] 您没有组织."); return; }
            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(p.factionId);

            if (!fact.settings.hasRadio) { MainChat.SendErrorChat(p, "[错误] 您的组织没有对讲机功能!"); return; }
            else
            {
                string message = "<i class='fad fa-walkie-talkie'></i>[对讲机] " + p.fakeName.Replace("_", " ") + ": " + string.Join(" ", args);
                //message2 = "<i class='fad fa-walkie-talkie'></i>" + string.Join(" ", messages);
                foreach (PlayerModel t in Alt.GetAllPlayers())
                {
                    if (t.factionId == p.factionId) { MainChat.PDRadioChat(t, message); }
                    else if (p.Position.Distance(t.Position) < 5)
                    {
                        t.SendChatMessage(message);
                    }
                }
            }
        }

        public static async Task<bool> CheckPlayerInPd(PlayerModel player)
        {
            bool t = false;
            //CharacterModel p = GlobalEvents.PlayerGetData(player);
            List<int> a = await Database.DatabaseMain.GetPDFactionIds();

            foreach (int fId in a)
            {
                if (fId == player.factionId)
                {
                    return true;
                }
            }
            return t;
        }

        public static async Task<bool> CheckPlayerInFD(PlayerModel p)
        {
            bool t = false;
            //CharacterModel p = GlobalEvents.PlayerGetData(player);
            List<int> a = await Database.DatabaseMain.GetTypeOfFactionIDs(ServerGlobalValues.fType_FD);
            List<int> b = await Database.DatabaseMain.GetTypeOfFactionIDs(ServerGlobalValues.fType_MD);

            foreach (int fId in a)
            {
                if (fId == p.factionId)
                {
                    return true;
                }
            }

            foreach (int mId in b)
            {
                if (mId == p.factionId)
                    return true;
            }
            return t;
        }


        [Command("fcar")]
        public async Task COM_SpawnFactionCar(PlayerModel p, params string[] args)
        {
            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            if (p.factionId <= 0) { MainChat.SendErrorChat(p, "[错误] 您没有组织."); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /fcar [model]"); return; }
            if (p.tempFactionCar != 0) { MainChat.SendErrorChat(p, "[错误] 您已经创建了一辆组织车辆, 请输入 /rfcar 删除之前的车辆."); return; }
            bool pdCheck = await CheckPlayerInPd(p);
            bool fdCheck = await CheckPlayerInFD(p);

            if (pdCheck || fdCheck)
            {
                if (args[0] == "hydra") { MainChat.SendErrorChat(p, "[?] 无法创建这个, 你要干什么."); return; }
                IVehicle v = Alt.CreateVehicle(args[0], p.Position, p.Rotation);
                VehModel veh = (VehModel)v;
                veh.sqlID = 200000 + veh.Id;
                veh.maxFuel = 100;
                veh.inventoryCapacity = 100;
                veh.currentFuel = veh.maxFuel;
                veh.owner = p.sqlID;
                veh.factionId = p.factionId;
                p.tempFactionCar = veh.sqlID;
                MainChat.SendInfoChat(p, "[!] 成功创建模型 " + args[0] + " 的车辆, 组织临时编号 " + v.Id + " 临时数据库ID / " + veh.sqlID);
            }

            return;
        }

        [Command("fcarlist")]
        public async Task COM_FactionCarList(PlayerModel p)
        {
            if (p.factionId <= 0) { MainChat.SendErrorChat(p, "[错误] 您没有组织."); return; }
            bool pdCheck = await CheckPlayerInPd(p);
            bool fdCheck = await CheckPlayerInFD(p);
            if (pdCheck || fdCheck)
            {
                string text = "<center>已刷出组织车辆列表</center><br>";
                foreach (VehModel v in Alt.GetAllVehicles())
                {
                    if (v.sqlID >= 200000 && p.tempFactionCar == v.sqlID)
                        text += ((VehicleModel)v.Model).ToString() + " | ID: " + v.Id + " | 数据库ID: " + v.sqlID + "<br>";
                }
                MainChat.SendInfoChat(p, text, true);
            }
            return;
        }

        [Command("rfcar")]
        public async Task COM_RemoveTempFactionCar(PlayerModel p)
        {
            if (p.factionId <= 0) { MainChat.SendErrorChat(p, "[错误] 您没有组织."); return; }
            bool pdCheck = await CheckPlayerInPd(p);
            bool fdCheck = await CheckPlayerInFD(p);
            if (p.tempFactionCar == 0) { MainChat.SendErrorChat(p, "[错误] 您没有创建的组织车辆, 请输入 /fcar 创建."); return; }
            if (pdCheck || fdCheck)
            {
                string text = "<center>已刷出组织车辆列表</center><br>";
                foreach (VehModel v in Alt.GetAllVehicles())
                {
                    if (v.sqlID >= 200000 && p.tempFactionCar == v.sqlID)
                    {
                        text += "您已删除您创建的组织车辆: " + v.Id + " | 数据库ID: " + v.sqlID + "<br>";
                        await Database.DatabaseMain.DeleteVehicle(v);
                        v.Remove();
                    }
                }
                MainChat.SendInfoChat(p, text, true);
                p.tempFactionCar = 0;
            }

            return;
        }

        [Command("feditplate")]
        public async Task FactionEditPlate(PlayerModel p, params string[] args)
        {
            if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] 您必须在车内."); return; }
            VehModel v = (VehModel)p.Vehicle;
            if (p.factionId <= 0) { MainChat.SendErrorChat(p, "[错误] 您没有组织."); return; }
            bool pdCheck = await CheckPlayerInPd(p);
            bool fdCheck = await CheckPlayerInFD(p);
            if (p.tempFactionCar == 0) { MainChat.SendErrorChat(p, "[错误] 您没有创建的组织车辆, 请输入 /fcar 创建."); return; }
            if (v == null) { MainChat.SendInfoChat(p, "无效车辆!"); return; }       
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /feditplate [车牌号]"); return; }
            
            v.NumberplateText = args[0].ToString();
            
            MainChat.SendInfoChat(p, "成功更新车牌号为" + v.NumberplateText);
        }
        
        [Command("feditcar")]
        public async Task FactionEditCar(PlayerModel p, params string[] args)
        {           
            if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] 您必须在车内."); return; }
            VehModel v = (VehModel)p.Vehicle;
            if (p.factionId <= 0) { MainChat.SendErrorChat(p, "[错误] 您没有组织."); return; }
            bool pdCheck = await CheckPlayerInPd(p);
            bool fdCheck = await CheckPlayerInFD(p);
            if (p.tempFactionCar == 0) { MainChat.SendErrorChat(p, "[错误] 您没有创建的组织车辆, 请输入 /fcar 创建."); return; }
            if (v == null) { MainChat.SendInfoChat(p, "无效车辆!"); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /feditcar [mod/color1/color2/tire/wheel/wheel2/sedef/tint/lmultipler/xeoncolor]"); return; }
            v.SetModKitAsync((byte)1);

            switch (args[0])
            {
                case "mod":
                    int tuningId; bool tuningIDOK = Int32.TryParse(args[1], out tuningId);
                    int tuningVal; bool tuningValOk = Int32.TryParse(args[2], out tuningVal);

                    if (!tuningIDOK || !tuningValOk)
                        return;

                    v.SetMod((byte)tuningId, (byte)tuningVal);
                    break;

                case "color1":
                    if (!Int32.TryParse(args[1], out int newCl1))
                        return;

                    v.SetPrimaryColorAsync((byte)newCl1);
                    break;

                case "color2":
                    if (!Int32.TryParse(args[1], out int newCl2))
                        return;

                    v.SetSecondaryColorAsync((byte)newCl2);
                    break;

                case "tire":
                    if (!Int32.TryParse(args[1], out int newLastik))
                        return;

                    v.SetWheelColorAsync((byte)newLastik);
                    break;

                case "wheel":
                    //v.WheelType = Int32.Parse(arg[1]);
                    if (!Int32.TryParse(args[1], out int newWheel))
                        return;

                    v.SetWheelsAsync((byte)newWheel, (byte)newWheel);
                    v.SetRearWheelAsync((byte)newWheel);
                    break;

                case "wheel2":
                    if (!Int32.TryParse(args[1], out int newWheel2))
                        return;

                    v.SetRearWheelAsync((byte)newWheel2);
                    break;

                case "sedef":
                    if (!Int32.TryParse(args[1], out int newSedef))
                        return;

                    v.SetPearlColorAsync((byte)newSedef);
                    break;

                case "tint":
                    if (!Int32.TryParse(args[1], out int newTint))
                        return;

                    v.SetWindowTintAsync((byte)newTint);

                    break;

                case "lmultipler":
                    if (!float.TryParse(args[1], out float newLightMul))
                        return;

                    v.SetLightsMultiplierAsync(newLightMul);
                    break;

                case "xeoncolor":
                    if (!Int32.TryParse(args[1], out int newXcolor))
                        return;

                    v.SetHeadlightColorAsync((byte)newXcolor);
                    break;

            }

            MainChat.SendInfoChat(p, "[成功更新车辆改装]");
            return;
        }
        
        [Command("carmod")]
        public async Task AdminSetMod(PlayerModel p, params string[] args)
        {           
            if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] 您必须在车内."); return; }
            VehModel v = (VehModel)p.Vehicle;
            if (v == null) { MainChat.SendInfoChat(p, "无效车辆!"); return; }

            switch (args[0])
            {
                case "mod":
                    int tuningId; bool tuningIDOK = Int32.TryParse(args[1], out tuningId);
                    int tuningVal; bool tuningValOk = Int32.TryParse(args[2], out tuningVal);

                    if (!tuningIDOK || !tuningValOk)
                        return;

                    v.SetMod((byte)tuningId, (byte)tuningVal);
                    break;
            }

            MainChat.SendInfoChat(p, "[成功更新车辆改装]");
            return;
        }
    }
}
