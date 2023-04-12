using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Resources.Chat.Api;
using outRp.Models;
using outRp.Chat;
using outRp.Globals;


namespace outRp.OtherSystem.LSCsystems
{
    public class GiveAwaySystem : IScript
    {
        public class GiveAway
        {
            public int owner { get; set; }
            public List<Client> clients { get; set; } = new List<Client>();
            public int price { get; set; } = 100;
            public bool canBuyTicket { get; set; } = true;
            public bool inProgress { get; set; } = false;

            public class Client
            {
                public string Name { get; set; }
                public int ID { get; set; }
            }
        }

        public static List<GiveAway> giveAways = new List<GiveAway>();

        [Command("makegiveaway")]
        public static void COM_CreateGiveAway(PlayerModel p)
        {
            GiveAway check = giveAways.Find(x => x.owner == p.sqlID);
            if(check != null) { MainChat.SendErrorChat(p, "[错误] 您已设置抽奖票, 请先使用."); return; }

            check = new GiveAway();
            check.owner = p.sqlID;
            giveAways.Add(check);

            MainChat.SendInfoChat(p, "[!] 已创建抽奖票.");
            return;
        }

        [Command("giveaway")]
        public static void COM_GiveAwayEvents(PlayerModel p, params string[] args)
        {
            GiveAway g = giveAways.Find(x => x.owner == p.sqlID);
            if(g== null) { MainChat.SendInfoChat(p, "[!] 您还没有抽奖票, 请先创建抽奖票 /makegiveaway."); return; }

            switch (args[0])
            {
                case "ticket":
                    g.canBuyTicket = !g.canBuyTicket;
                    string status = (g.canBuyTicket) ? "开启." : "关闭.";
                    MainChat.SendInfoChat(p, "[!] 抽奖票可出售状态: " + status);
                    return;

                case "price":
                    if(args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /giveaway price [价格]"); return; }
                    if(!Int32.TryParse(args[1], out int newPrice)) { MainChat.SendInfoChat(p, "[用法] /giveaway price [价格]"); return; }
                    if (newPrice <= 0)
                        return;
                    g.price = newPrice;
                    MainChat.SendInfoChat(p, "[!] 抽奖票价设置为 " + newPrice);
                    return;

                case "start":
                    if(g.clients.Count <= 0) { MainChat.SendErrorChat(p, "[错误] 还没有玩家购买您的抽奖票, 所以无法开奖!"); return; }
                    GiveAwayResult(g);
                    return;

                default:
                    MainChat.SendInfoChat(p, "[用法] /giveaway [选项] [数值]<br>ticket - price - start");
                    return;
            }
        }
        
        [Command("buyticket")]
        public static void COM_BuyTicket(PlayerModel p, params string[] args)
        {
            if(args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /buyticket [id]"); return; }
            if(!Int32.TryParse(args[0], out int tSql)) { MainChat.SendInfoChat(p, "[用法] /buyticket [id]"); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if(t == null) { MainChat.SendInfoChat(p, "[错误] 无效玩家."); return; }
            if(t.Position.Distance(p.Position) > 5) { MainChat.SendErrorChat(p, "[错误] 您离指定玩家太远."); return; }

            GiveAway check = giveAways.Find(x => x.owner == t.sqlID);
            if(check == null) { MainChat.SendErrorChat(p, "[错误] 指定玩家不出售抽奖票."); return; }
            if (!check.canBuyTicket) { MainChat.SendErrorChat(p, "[错误] 目前无法买抽奖票!"); return; }

            GiveAway.Client c2 = check.clients.Find(x => x.ID == p.sqlID);
            if(c2 != null) { MainChat.SendErrorChat(p, "[错误] 您已经购买过抽奖票了."); return; }
            if(p.cash < check.price) { MainChat.SendErrorChat(p, "[错误] 您没有足够的钱!"); return; }
            p.cash -= check.price;
            t.cash += check.price;            
            c2 = new GiveAway.Client();
            c2.ID = p.sqlID;
            c2.Name = p.characterName.Replace("_", " ");
            check.clients.Add(c2);
            MainChat.SendInfoChat(p, "[!] 成功购买抽奖票.");
            return;
        }

        public async static void GiveAwayResult(GiveAway g)
        {
            g.inProgress = true;
            g.canBuyTicket = false;

            Random rnd = new Random();
            string curr = "";
            int currIndex = 999;

   
            for (int a = 0; a < g.clients.Count; a++)
            {

                PlayerModel t = GlobalEvents.GetPlayerFromSqlID(g.clients[a].ID);
                if (t == null) { g.clients.RemoveAt(a); continue; }
                curr = t.characterName.Replace("_", " ");
                SendGiveAwayMessage(g, "~w~开奖中: ~y~" + curr, 1);
                await Task.Delay(1100);
            }
            

            currIndex = rnd.Next(0, g.clients.Count);
            curr = g.clients[currIndex].Name;
            SendGiveAwayMessage(g, "~b~获奖人员: ~y~" + curr, 5);
            g.clients.RemoveAt(currIndex);
            return;
        }

        public static void SendGiveAwayMessage(GiveAway g, string message, int time = 1)
        {
            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(g.owner);
            if (t == null)
                return;
            GlobalEvents.SubTitle(t, message, time);

            List<GiveAway.Client> remList = new List<GiveAway.Client>();
            foreach(GiveAway.Client c in g.clients)
            {
                PlayerModel t1 = GlobalEvents.GetPlayerFromSqlID(c.ID);
                if(t1 == null) { remList.Add(c); continue; }
                GlobalEvents.SubTitle(t1, message, time);
            }

            foreach(GiveAway.Client c2 in remList)
            {
                g.clients.Remove(c2);
            }
            return;
        }

        
    }
}
