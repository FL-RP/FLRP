using System;
using AltV.Net; using AltV.Net.Async;
using outRp.Models;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;
using outRp.Globals;
using outRp.Chat;
using outRp.OtherSystem.NativeUi;

namespace outRp.Company.systems
{
    public class InviteSystem : IScript
    {
        [Command("cinvite")]
        public static async Task COM_InviteCompany(PlayerModel p, params string[] args)
        {
            if(args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /cinvite biz 或者 faction"); return; }

            var company = Database.BusinessDatabase.GetPlayerOwnCompany(p.sqlID);
            if (company == null) { MainChat.SendErrorChat(p, "[错误] 您没有公司!"); return; }

            switch (args[0])
            {
                case "biz":
                    if(!Int32.TryParse(args[1], out int BizID)) { MainChat.SendInfoChat(p, "[用法] /cinvite biz [产业ID]"); return; }
                    var biz = await Props.Business.getBusinessById(BizID);
                    if (biz.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 未找到输入的产业!"); return; }

                    if(biz.Item1.company != 0) { MainChat.SendErrorChat(p, "[错误] 指定产业已经属于您的公司了!"); return; }
                    PlayerModel bizTarget = GlobalEvents.GetPlayerFromSqlID(biz.Item1.ownerId);
                    if(bizTarget == null) { MainChat.SendErrorChat(p, "[错误] 指定产业的业主不在线!"); return; }

                    MainChat.SendInfoChat(p, "[?] 您发送了邀请 [ " + biz.Item1.name + " 业主: " + bizTarget.characterName.Replace("_", " ") + " ] 加入您的公司.");
                    Inputs.SendButtonInput(bizTarget, p.characterName.Replace("_"," ") + " 向您发送了将您产业加入公司的邀请.(产业 " + biz.Item1.ID + ")", "Company:Invite", "产业," + company.ID + "," + p.sqlID + "," + biz.Item1.ID);
                    return;

                case "faction":
                    if (!Int32.TryParse(args[1], out int tSQL)) { MainChat.SendInfoChat(p, "[用法] /cinvite faction [组织领导人ID]"); return; }
                    PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSQL);
                    if(t == null) { MainChat.SendErrorChat(p, "[错误] 指定玩家不在线!"); return; }
                    if(t.factionId <= 0) { MainChat.SendErrorChat(p, "[错误] 指定玩家没有组织!"); return; }

                    FactionModel fact = await outRp.Database.DatabaseMain.GetFactionInfo(t.factionId);
                    if (fact.owner != t.sqlID) { MainChat.SendErrorChat(p, "[错误] 指定玩家不是组织领导人!"); return; }
                    if(fact.company != 0) { MainChat.SendErrorChat(p, "[错误] 指定玩家的组织已属于其他公司了."); return; }

                    MainChat.SendErrorChat(p, "[?] 您发送了邀请 [ " + fact.name + " ] 加入您的公司.");
                    Inputs.SendButtonInput(t, p.characterName.Replace("_", " ") + " 向您发送了将您组织加入公司的邀请", "Company:Invite", "组织," + company.ID + "," + p.sqlID + ",0");
                    return;

                default: return;
            }
        }


        [AsyncClientEvent("Company:Invite")]
        public async Task EVENT_CompanyInvite(PlayerModel p, bool selection, string _vals)
        {
            if (p.Ping > 250)
                return;
            string[] val = _vals.Split(',');
            if (!Int32.TryParse(val[2], out int t1SQL))
                return;
            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(t1SQL);

            switch (val[0])
            {
                case "Biz":
                    if (!selection)
                    {
                        MainChat.SendInfoChat(p, "[?] 您拒绝了对方的公司邀请.");

                        if (target != null) { MainChat.SendInfoChat(target, "[?] " + p.characterName.Replace("_", " ") + " 拒绝了您的公司邀请."); }
                        return;
                    }
                    else
                    {
                        if (!Int32.TryParse(val[3], out int bizID))
                            return;

                        var biz = await Props.Business.getBusinessById(bizID);
                        if (biz.Item1 == null)
                            return;

                        if (!Int32.TryParse(val[1], out int C1Int))
                            return;

                        biz.Item1.company = C1Int;
                        await biz.Item1.Update(biz.Item2, biz.Item3);

                        MainChat.SendInfoChat(p, "[?] 您接受了 " + biz.Item1.name + " 的邀请, 现在开始您的产业属于此公司了.");
                        if (target != null) {
                            MainChat.SendInfoChat(target, "[?] " + biz.Item1.name + " 接受了您的公司邀请.");
                                }
                    }
                    return;

                case "Faction":
                    if (!selection)
                    {
                        MainChat.SendInfoChat(p, "[?] 您拒绝了对方的公司邀请.");
                        if (target != null) { MainChat.SendInfoChat(target, "[?] " + p.characterName.Replace("_", " ") + " 拒绝了您的公司邀请."); }
                        return;
                    }
                    else
                    {
                        FactionModel fact = await outRp.Database.DatabaseMain.GetFactionInfo(p.factionId);
                        if (fact == null)
                            return;

                        if(fact.company > 0) { MainChat.SendErrorChat(p, "[错误] 在接受目前的邀请之前您的组织已经属于其他公司了."); return; }

                        if (!Int32.TryParse(val[1], out int c2))
                            return;

                        fact.company = c2;
                        fact.Update();

                        var company = Database.BusinessDatabase.GetPlayerOwnCompany(p.sqlID);
                        if(company != null)
                        {
                            Factions.Faction.FactionChatSendInfoWithFactionID(fact.ID, "[?] 您接受了 " + company.Name + " 的邀请, 现在开始您的组织属于此公司了.");
                        }

                        MainChat.SendInfoChat(p, "[?] 您接受了公司邀请.");
                        

                        return;
                    }
            }
        }

        [Command("ckick")]
        public async Task COM_KickFromCompany(PlayerModel p, params string[] args)
        {
            if(args.Length <= 1) { MainChat.SendErrorChat(p, "[用法] /ckick [biz/faction] [ID]"); return; }
            
            if(!Int32.TryParse(args[1], out int ID)) { MainChat.SendInfoChat(p, "[用法] /ckick [biz/faction] [ID]"); return; }

            var company = Database.BusinessDatabase.GetPlayerOwnCompany(p.sqlID);
            if (company == null) { MainChat.SendErrorChat(p, "[错误] 您没有公司!"); return; }

            switch (args[0])
            {
                case "biz":
                    var biz = await Props.Business.getBusinessById(ID);
                    if (biz.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 指定产业没有所属公司."); return; }

                    if(biz.Item1.company != company.ID) { MainChat.SendErrorChat(p, "[错误] 指定产业不属于您的公司."); return; }

                    biz.Item1.company = 0;
                    await biz.Item1.Update(biz.Item2, biz.Item3);

                    MainChat.SendInfoChat(p, "[?] 您解除了和 " + biz.Item1.name + " 的公司产业关系.");

                    PlayerModel tt = GlobalEvents.GetPlayerFromSqlID(biz.Item1.ownerId);
                    if(tt != null) { MainChat.SendInfoChat(tt, "[!] " + company.Name + " 已经解除了和 " + biz.Item1.name + " 的公司产业关系."); return; }

                    return; ;


                case "faction":
                    PlayerModel target = GlobalEvents.GetPlayerFromSqlID(ID);
                    if(target == null) { MainChat.SendErrorChat(p, "[错误] 输入的组织领导人不在线(或者您可以输入任意属于该组织的玩家ID)."); return; }

                    FactionModel fact = await outRp.Database.DatabaseMain.GetFactionInfo(target.factionId);
                    if(fact == null) { MainChat.SendErrorChat(p, "[错误] 指定组织无效."); return; }
                    if(fact.company != company.ID) { MainChat.SendErrorChat(p, "[错误] 指定组织不属于您的公司."); return; }

                    fact.company = 0;
                    fact.Update();

                    MainChat.SendInfoChat(p, "[?] 您解除了和 " + fact.name + " 的公司组织关系.");
                    Factions.Faction.FactionChatSendInfoWithFactionID(fact.ID, "[*] " + company.Name + " 已经解除了和您组织的公司组织关系.");
                    return; ;

                default: return;
            }
        }
    }
}
