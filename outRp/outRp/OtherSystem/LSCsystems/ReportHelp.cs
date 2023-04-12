using AltV.Net;
using outRp.Chat;
using outRp.Models;
using System.Collections.Generic;
using AltV.Net.Resources.Chat.Api;
using outRp.Kook;

namespace outRp.OtherSystem.LSCsystems
{
    public class ReportHelp : IScript
    {
        public class ReportModel
        {
            public int ID { get; set; }
            public string ReportMessage { get; set; }
            public int TakenSql { get; set; } = 0;
        }
        public class HelpModel
        {
            public int ID { get; set; }
            public string HelpMessage { get; set; }
        }

        public static List<ReportModel> _serverReports = new List<ReportModel>();
        public static List<HelpModel> _serverHelpReqs = new List<HelpModel>();

        public static List<ReportModel> serverReports
        {
            get
            {
                return _serverReports;
            }
            set
            {
                _serverReports = value;
                PushDC();
            }
        }

        public static List<HelpModel> serverHelpReqs
        {
            get { return _serverHelpReqs; }
            set { _serverHelpReqs = value; PushDC(); }
        }

        public static async void PushDC()
        {
            //await Discord.Main.PushRepots();

        }

        [Command("report")]
        public async void SendNewReport(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /report [举报内容]"); return; }
            if (args[0] == "qx")
            {
                var cancelRep = serverReports.Find(x => x.ID == p.sqlID);
                if (cancelRep == null) { MainChat.SendInfoChat(p, "> 无效活动"); return; }
                serverReports.Remove(cancelRep);
                MainChat.SendInfoChat(p, "> 已撤销举报.");
                return;
            }
            var hasReport = serverReports.Find(x => x.ID == p.sqlID);
            if (hasReport != null) { MainChat.SendErrorChat(p, "[错误] 您已经提交过一份举报了, 请等待处理和回复."); return; }
            ReportModel newReport = new ReportModel();
            newReport.ID = p.sqlID;
            newReport.ReportMessage = string.Join(" ", args);
            serverReports.Add(newReport);

            foreach (PlayerModel admin in Alt.GetAllPlayers())
            {
                if (admin.adminWork && admin.adminLevel > 3) { admin.SendChatMessage("{BFCB00}[!] 收到一份新的举报, /reports 查看列表 - 用法: /are 编号 回复内容."); }
            }
            p.SendChatMessage("{02CD71} 您的举报已成功提交, 目前待处理的举报数量: " + serverReports.Count);
            //await Discord.Main.PushRepots();
            AccountModel account = await Database.DatabaseMain.getAccountInfo(p.SocialClubId);
            await KookSpace.ReportMessage(account.kookId, p.fakeName.Replace("_", " "), newReport.ReportMessage);
            return;
        }

        [Command("askq")]
        public async void SendNewHelp(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /askq [求助问题]"); return; }
            if (args[0] == "qx")
            {
                var cancelRep = serverHelpReqs.Find(x => x.ID == p.sqlID);
                if (cancelRep == null) { MainChat.SendInfoChat(p, "> 无效求助问题"); return; }
                serverHelpReqs.Remove(cancelRep);
                MainChat.SendInfoChat(p, "> 您已撤销求助.");
                return;
            }
            var hasHelpReq = serverHelpReqs.Find(x => x.ID == p.sqlID);
            if (hasHelpReq != null) { MainChat.SendErrorChat(p, "[错误] 您已经提交过一份求助了, 请等待处理和回复."); return; }

            HelpModel newHelp = new HelpModel();
            newHelp.ID = p.sqlID;
            newHelp.HelpMessage = string.Join(" ", args);
            serverHelpReqs.Add(newHelp);
            foreach (PlayerModel admin in Alt.GetAllPlayers())
            {
                if ((admin.adminLevel > 1 && admin.adminLevel < 5) || admin.adminWork) { admin.SendChatMessage("{BFCB00}[!] 收到一份新的求助, /askql 查看列表 - 用法: /acpq 编号 回复内容."); }
            }
            p.SendChatMessage("{02CD71} 您的问题已成功提交, 目前待处理的求助数量: " + serverHelpReqs.Count);
            //await Discord.Main.PushRepots();
            AccountModel account = await Database.DatabaseMain.getAccountInfo(p.SocialClubId);
            await KookSpace.AskqMessage(account.kookId, p.fakeName.Replace("_", " "), newHelp.HelpMessage);
            return;
        }

    }
}
