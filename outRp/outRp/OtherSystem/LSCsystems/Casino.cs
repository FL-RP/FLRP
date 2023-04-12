using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Resources.Chat.Api;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Globals;
using outRp.Models;
using outRp.OtherSystem.Textlabels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace outRp.OtherSystem.LSCsystems
{
    public class Casino : IScript
    {
        #region BlackJack
        public class BlackJack
        {
            public int ownerID { get; set; } = 0;
            public int minBet { get; set; } = 50;
            public int maxBet { get; set; } = 100;
            public int totalBet { get; set; } = 0;

            public Position position { get; set; } = new Position(0, 0, 0);

            public ulong PropID { get; set; } = 0;
            public ulong TextID { get; set; } = 0;

            public bool isGameStarted { get; set; } = false;
            public bool isUsersCanBet { get; set; } = false;
            public bool canPlayersJoin { get; set; } = true;

            public List<int> tableCards { get; set; } = new List<int>();
            //public List<int> tableDefaultCards { get; set; } = new List<int>() { 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 6, 6, 7, 7, 7, 7, 8, 8, 8, 8, 9, 9, 9, 9, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, };
            public List<int> tableDefaultCards { get; set; } = new List<int>() { 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 6, 6, 7, 7, 7, 7, 8, 8, 8, 8, 9, 9, 9, 9, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, };
            public bool isBlackJack { get; set; } = false;

            public List<Client> clients { get; set; } = new List<Client>();

            public class Client
            {
                public int ID { get; set; } = 0;
                public int currentBet { get; set; } = 0;
                public List<int> cards { get; set; } = new List<int>();
                public bool isBlackJack { get; set; } = false;
                public bool isCurrentPlayer { get; set; } = false;
            }
        }

        public static List<BlackJack> BlackJackTables = new List<BlackJack>();

        // new int[] { 1,1,1,1,2,2,2,2,3,3,3,3,4,4,4,4,5,5,5,5,6,6,6,6,7,7,7,7,8,8,8,8,9,9,9,9,10,10,10,10,10,10,10,10,10,10,10,10, };

        // Casino sistemlerini kurabilecek kişi olup olmadığını sorgular.
        public static Task<bool> canUse(PlayerModel p)
        {
            return Props.Business.GetPlayerBusinessType(p, ServerGlobalValues.casinoBusiness);
        }

        [Command("addbjt")]
        public async Task COM_CreateBlackJackTable(PlayerModel p)
        {
            if (!await canUse(p) == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }

            BlackJack check = BlackJackTables.Find(x => x.ownerID == p.sqlID);
            if (check != null) { MainChat.SendErrorChat(p, "[错误] 您已设置了二十一点游戏台, 请先将桌子移除."); return; }

            GlobalEvents.ShowObjectPlacement(p, "ch_prop_casino_blackjack_01a", "Casino:BJCreate");
            return;
        }

        [AsyncClientEvent("Casino:BJCreate")]
        public void Casino_BJPlace(PlayerModel p, string rot, string pos, string model)
        {
            BlackJack check = BlackJackTables.Find(x => x.ownerID == p.sqlID);
            if (check != null) { MainChat.SendErrorChat(p, "[错误] 您已设置了二十一点游戏台, 请先将桌子移除."); return; }

            Vector3 position = JsonConvert.DeserializeObject<Vector3>(pos);
            Vector3 rotation = JsonConvert.DeserializeObject<Vector3>(rot);

            Vector3 positionText = position;
            positionText.Z += 1.5f;


            BlackJack nTable = new BlackJack()
            {
                ownerID = p.sqlID,
                PropID = PropStreamer.Create(model, position, rotation, p.Dimension, frozen: true).Id,
                TextID = TextLabelStreamer.Create("[BJ Masası]", positionText, dimension: p.Dimension, font: 0, scale: 0.4f, streamRange: 3).Id,
                position = position
            };

            BlackJackTables.Add(nTable);
            MainChat.SendInfoChat(p, "[?] BlackJack masası kuruldu.");
            return;
        }

        [Command("bjjjj")]
        public static void COM_EditBJTable(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[Kullanım] /bj [çeşit] [varsa değer]<br>Çeşitler: minbahis - maxbahis - bahisbaslat - bahisbitir - bahis - siradaki - kartiste - kal - masakartac - katil - oyunbitir - masakaldir"); return; }
            BlackJack table = BlackJackTables.Find(x => x.position.Distance(p.Position) < 2);
            if (table == null) { MainChat.SendErrorChat(p, "[错误] BJ masası bulunamadı!"); return; }
            Random rnd = new Random();

            switch (args[0])
            {
                case "minbahis":
                    if (table.ownerID != p.sqlID) { MainChat.SendErrorChat(p, "[错误] Bu masa size ait değil."); return; }
                    if (table.isGameStarted) { MainChat.SendErrorChat(p, "[BJ][错误] Oyun başlamış durumdayken bahis miktarı değiştirilemez."); return; }
                    int minBet; bool minBetOk = Int32.TryParse(args[1], out minBet);
                    if (!minBetOk) { MainChat.SendErrorChat(p, "[错误] Minimum bahis için bir miktar girmelisiniz."); return; }
                    if (minBet <= 9) { MainChat.SendErrorChat(p, "[错误] Minimum bahis için bir miktar girmelisiniz."); return; }
                    table.minBet = minBet;
                    MainChat.SendInfoChat(p, "[BJ] Minimum bahis " + minBet.ToString() + " olarak ayarlandı.");
                    BJTextUpdate(table);
                    return;

                case "maxbahis":
                    if (table.ownerID != p.sqlID) { MainChat.SendErrorChat(p, "[错误] Bu masa size ait değil."); return; }
                    if (table.isGameStarted) { MainChat.SendErrorChat(p, "[BJ][错误] Oyun başlamış durumdayken bahis miktarı değiştirilemez."); return; }
                    int maxBet; bool maxBetOk = Int32.TryParse(args[1], out maxBet);
                    if (!maxBetOk) { MainChat.SendErrorChat(p, "[错误] Maksimum bahis için bir miktar girmelisiniz."); return; }
                    if (maxBet < 100) { MainChat.SendErrorChat(p, "[错误] Maksimum bahis için bir miktar girmelisiniz."); return; }
                    table.maxBet = maxBet;
                    MainChat.SendInfoChat(p, "[BJ] Maksimum bahis " + maxBet.ToString() + " olarak ayarlandı.");
                    BJTextUpdate(table);
                    return;

                case "bahisbaslat":
                    if (table.ownerID != p.sqlID) { MainChat.SendErrorChat(p, "[错误] Bu masa size ait değil."); return; }
                    if (table.isGameStarted) { MainChat.SendErrorChat(p, "[BJ][错误] Oyun zaten başlamış. Önce oyunun bitmesi gerek."); return; }
                    if (table.clients.Count <= 0) { MainChat.SendErrorChat(p, "[BJ][错误] Masada oyuncu bulunmuyor!"); table.isGameStarted = false; table.isUsersCanBet = false; return; }
                    table.isGameStarted = true;
                    table.isUsersCanBet = true;
                    table.canPlayersJoin = false;

                    foreach (BlackJack.Client startBetClients in table.clients)
                    {
                        startBetClients.cards = new List<int>();
                        startBetClients.currentBet = 0;
                        startBetClients.isBlackJack = false;
                    }
                    table.isBlackJack = false;
                    table.tableDefaultCards = new List<int>() { 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 6, 6, 7, 7, 7, 7, 8, 8, 8, 8, 9, 9, 9, 9, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, };

                    table.canPlayersJoin = true;

                    SendBlackJackMessage(table, "~y~[BJ]~w~Bahisler ~g~başladı!");
                    BJTextUpdate(table);

                    return;


                case "bahisbitir":
                    if (table.ownerID != p.sqlID) { MainChat.SendErrorChat(p, "[错误] Bu masa size ait değil."); return; }
                    if (!table.isGameStarted) { MainChat.SendErrorChat(p, "[BJ][错误] Oyun başlamamış, lütfen önce oyunu başlatın."); return; }
                    if (!table.isUsersCanBet) { MainChat.SendErrorChat(p, "[BJ][错误] Şuan aktif bahis yok."); return; }

                    table.canPlayersJoin = false;
                    table.isUsersCanBet = false;
                    int endBetTotal = 0;

                    int newCard = 0;
                    foreach (BlackJack.Client betTotalClients in table.clients)
                    {
                        newCard = rnd.Next(0, table.tableDefaultCards.Count - 1);
                        betTotalClients.cards.Add(table.tableDefaultCards[newCard]);
                        table.tableDefaultCards.RemoveAt(newCard);
                        endBetTotal += betTotalClients.currentBet;
                    }

                    newCard = rnd.Next(0, table.tableDefaultCards.Count - 1);
                    table.tableCards.Add(table.tableDefaultCards[newCard]);
                    table.tableDefaultCards.RemoveAt(newCard);

                    table.totalBet = endBetTotal;
                    BJTextUpdate(table);

                    return;

                case "bahis":
                    if (!table.isGameStarted) { MainChat.SendErrorChat(p, "[BJ][错误] Oyun henüz başlamamış."); return; }
                    if (!table.isUsersCanBet) { MainChat.SendErrorChat(p, "[BJ][错误] Şuan bahis oynayamazsınız."); return; }

                    BlackJack.Client betCheck = table.clients.Find(x => x.ID == p.sqlID);
                    if (betCheck == null) { MainChat.SendErrorChat(p, "[BJ][错误] Bu masada oyuncu değilsiniz."); return; }

                    if (betCheck.currentBet != 0) { MainChat.SendErrorChat(p, "[BJ][错误] Zaten bahis oynamışsınız."); return; }

                    int newBet; bool isNewBetOk = Int32.TryParse(args[1], out newBet);
                    if (!isNewBetOk) { MainChat.SendErrorChat(p, "[BJ][错误] Geçerli bir miktar girmelisiniz."); return; }
                    if (newBet < table.minBet || newBet > table.maxBet || newBet <= 0) { MainChat.SendErrorChat(p, "[BJ][错误] Geçerli bir miktar girmelisiniz."); return; }
                    if (p.cash < newBet) { MainChat.SendErrorChat(p, "[错误] Yeterli paranız yok!"); return; }

                    PlayerModel tableOwnerForBet = GlobalEvents.GetPlayerFromSqlID(table.ownerID);
                    if (tableOwnerForBet == null) { MainChat.SendErrorChat(p, "[错误] Bu masa kapatılmış."); return; }

                    p.cash -= newBet;
                    tableOwnerForBet.cash += newBet;
                    betCheck.currentBet = newBet;

                    SendBlackJackMessage(table, "~w~" + p.characterName.Replace("_", " ") + " isimli oyuncu ~g~$" + newBet.ToString() + "~w~ bahis oynadı");

                    return;

                case "siradaki":
                    if (table.ownerID != p.sqlID) { MainChat.SendErrorChat(p, "[BJ][错误] Bu masanın sahibi siz değlisiniz."); return; }
                    int nextPlayerID; bool isNextPlayerIDOK = Int32.TryParse(args[1], out nextPlayerID);
                    if (!isNextPlayerIDOK) { MainChat.SendErrorChat(p, "[错误] Geçerli bir sayı girmelisiniz."); return; }

                    BlackJack.Client nextPlayer = table.clients.Find(x => x.ID == nextPlayerID);
                    if (nextPlayer == null) { MainChat.SendErrorChat(p, "[BJ][错误] Oyuncu masada oynamıyor!"); return; }
                    PlayerModel nextPlayerTarget = GlobalEvents.GetPlayerFromSqlID(nextPlayer.ID);
                    if (nextPlayerTarget == null) { MainChat.SendErrorChat(p, "[错误] Kişi oyundan çıkmış"); table.clients.Remove(nextPlayer); return; }

                    if (nextPlayer.isCurrentPlayer) { MainChat.SendErrorChat(p, "[错误] Kişinin zaten oynanması bekleniyor."); return; }
                    if (nextPlayer.isBlackJack) { MainChat.SendErrorChat(p, "[错误] Kişi zaten BJ yapmış."); return; }
                    nextPlayer.isCurrentPlayer = true;
                    BJTextUpdate(table);
                    return;

                case "kartiste":
                    BlackJack.Client getCardPlayer = table.clients.Find(x => x.ID == p.sqlID);
                    if (getCardPlayer == null) { MainChat.SendErrorChat(p, "[BJ][错误] Bu masada oynamıyorsunuz."); return; }
                    if (!getCardPlayer.isCurrentPlayer) { MainChat.SendErrorChat(p, "[BJ][错误] Sıra sizde değil."); return; }

                    int getCardTotal = 0;
                    getCardPlayer.cards.ForEach(x =>
                    {
                        getCardTotal += x;
                    });

                    if (getCardTotal > 21) { MainChat.SendErrorChat(p, "[BJ][错误] İflas ettiniz. Yeni kart çekemezsiniz."); return; }

                    int getCardIndex = rnd.Next(0, table.tableDefaultCards.Count - 1);
                    getCardPlayer.cards.Add(table.tableDefaultCards[getCardIndex]);

                    getCardTotal += table.tableDefaultCards[getCardIndex];

                    table.tableDefaultCards.RemoveAt(getCardIndex);
                    getCardPlayer.isCurrentPlayer = false;
                    BJTextUpdate(table);
                    if (getCardTotal > 21) { SendBlackJackMessage(table, p.characterName.Replace("_", " ") + " isimli oyuncu ~r~iflas ~w~etti."); return; }
                    else if (getCardTotal == 21) { SendBlackJackMessage(table, p.characterName.Replace("_", " ") + " isimli oyuncu ~g~BJ ~w~yaptı!"); getCardPlayer.isBlackJack = true; return; }
                    else
                    {
                        SendBlackJackMessage(table, p.characterName.Replace("_", " ") + " isimli oyuncunun toplam kartı: " + getCardTotal.ToString());
                        return;
                    }

                case "kal":
                    BlackJack.Client stayCardPlayer = table.clients.Find(x => x.ID == p.sqlID);
                    if (stayCardPlayer == null) { MainChat.SendErrorChat(p, "[BJ][错误] Bu masada oynamıyorsunuz!"); return; }
                    if (!stayCardPlayer.isCurrentPlayer) { MainChat.SendErrorChat(p, "[BJ][错误] Sıra sizde değil."); return; }

                    int stayCardTotal = 0;
                    stayCardPlayer.cards.ForEach(x =>
                    {
                        stayCardTotal += x;
                    });

                    stayCardPlayer.isCurrentPlayer = false;

                    SendBlackJackMessage(table, p.characterName.Replace("_", " ") + " isimli oyuncu ~g~Kal ~w~ dedi.");
                    BJTextUpdate(table);
                    return;

                case "masakartac":
                    if (table.ownerID != p.sqlID) { MainChat.SendErrorChat(p, "[BJ][错误] Bu masanın sahibi siz değilsiniz."); return; }

                    int tableOpenIndex = rnd.Next(0, table.tableDefaultCards.Count - 1);

                    table.tableCards.Add(table.tableDefaultCards[tableOpenIndex]);
                    table.tableDefaultCards.RemoveAt(tableOpenIndex);

                    BJTextUpdate(table);

                    return;

                case "katil":
                    if (!table.canPlayersJoin) { MainChat.SendErrorChat(p, "[BJ][错误] Şuan oyuna katılamazsınız."); return; }
                    if (table.clients.Count >= 4) { MainChat.SendErrorChat(p, "[BJ][错误] Masa kişi limitine ulaştı."); return; }
                    BlackJack.Client joinReq = table.clients.Find(x => x.ID == p.sqlID);
                    if (joinReq != null) { MainChat.SendErrorChat(p, "[BJ][错误] Zaten oyuna katılmışsınız."); return; }

                    joinReq = new BlackJack.Client()
                    {
                        ID = p.sqlID,
                        currentBet = 0,
                        isBlackJack = false,
                        isCurrentPlayer = false,
                    };

                    table.clients.Add(joinReq);
                    SendBlackJackMessage(table, p.characterName.Replace("_", " ") + " isimli oyuncu masaya katıldı.");
                    BJTextUpdate(table);
                    return;

                case "oyunbitir":
                    if (table.ownerID != p.sqlID) { MainChat.SendErrorChat(p, "[BJ][错误] Bu masanın sahibi siz değilsiniz."); return; }

                    string info = "-";

                    int totalNeedMoney = 1;
                    int tableCardsTotal = table.tableCards.Sum();
                    if (tableCardsTotal > 21)
                    {
                        foreach (BlackJack.Client _inf in table.clients)
                        {
                            int _infTotal = _inf.cards.Sum();
                            if (_infTotal > 21) continue;
                            else totalNeedMoney += (int)(_inf.currentBet * 1.5);
                        }
                    }
                    else
                    {
                        foreach (BlackJack.Client _inf in table.clients)
                        {

                            if (_inf == null) continue;

                            int _infTotal = _inf.cards.Sum();
                            if (_infTotal == 21) { totalNeedMoney += _inf.currentBet * 2; }
                            else if (_infTotal < tableCardsTotal) continue;
                            else if (_infTotal > tableCardsTotal) { totalNeedMoney += (int)(_inf.currentBet * 1.5); }
                            else if (_infTotal == tableCardsTotal) { totalNeedMoney += _inf.currentBet; }
                        }
                    }
                    if (p.cash < totalNeedMoney)
                    {
                        MainChat.SendErrorChat(p, "[错误] Üzerinizdeki toplam para yetersiz olduğu için oyunu bitiremiyorsunuz. Lütfen masadakileri bekletip, işyeri sahibinden ödeme için para isteyin.");
                        return;
                    }



                    foreach (BlackJack.Client inf in table.clients)
                    {
                        PlayerModel infPlayer = GlobalEvents.GetPlayerFromSqlID(inf.ID);
                        if (infPlayer == null) continue;

                        if (inf.isBlackJack)
                        {
                            info += "<br>" + infPlayer.characterName.Replace("_", " ") + " BJ" + " | B: " + inf.currentBet.ToString();
                            infPlayer.cash += inf.currentBet * 2;
                            p.cash -= inf.currentBet * 2;
                            continue;
                        }

                        int infTotal = 0;
                        inf.cards.ForEach(x => { infTotal += x; });

                        if (infTotal > 21)
                        {
                            info += "<br>" + infPlayer.characterName.Replace("_", " ") + " IFLAS" + "  | B: " + inf.currentBet.ToString();
                            continue;
                        }
                        if (tableCardsTotal > 21 && infTotal < 22)
                        {
                            infPlayer.cash += (int)(inf.currentBet * 1.5);
                            p.cash -= (int)(inf.currentBet * 1.5);
                            continue;
                        }

                        info += "<br>" + infPlayer.characterName.Replace("_", " ") + " | " + infTotal.ToString() + " | B: " + inf.currentBet.ToString();
                        if (infTotal > tableCardsTotal)
                        {
                            infPlayer.cash += (int)(inf.currentBet * 1.5);
                            p.cash -= (int)(inf.currentBet * 1.5);
                            continue;
                        }
                        else if (infTotal == tableCardsTotal)
                        {
                            infPlayer.cash += inf.currentBet;
                            p.cash -= inf.currentBet;
                            continue;
                        }
                    }

                    string tableInfo = ""; int tableTotale = 0;
                    table.tableCards.ForEach(x => { tableInfo += x.ToString() + "-"; tableTotale += x; });


                    foreach (BlackJack.Client cl in table.clients)
                    {
                        PlayerModel target = GlobalEvents.GetPlayerFromSqlID(cl.ID);
                        if (target == null) continue;

                        Bar.RemoveAllBars(target);

                        //GlobalEvents.ShowNotification(target, info, title: "Oyun bitti", subtitle: "~b~Masa:~w~ " + tableInfo + " T:" + tableTotale.ToString(), p);
                        MainChat.SendInfoChat(target, "Oyun bitti. <br> Masa: " + tableInfo + " T:" + tableTotale.ToString() + "<br>" + info);
                    }

                    //GlobalEvents.ShowNotification(p, info, title: "Oyun bitti", subtitle: "~b~Masa:~w~ " + tableInfo + " T:" + tableTotale.ToString(), p);
                    MainChat.SendInfoChat(p, "Oyun bitti. <br> Masa: " + tableInfo + " T:" + tableTotale.ToString() + "<br>" + info);

                    int max = table.maxBet; int min = table.minBet;
                    table.isGameStarted = false;
                    table.isUsersCanBet = false;
                    table.canPlayersJoin = true;
                    table.clients.Clear();
                    table.tableCards.Clear();

                    BJTextUpdate(table);

                    return;

                case "masakaldir":
                    if (table.ownerID != p.sqlID) { MainChat.SendErrorChat(p, "[BJ][错误] Bu masanın sahibi siz değilsiniz."); return; }

                    Bar.RemoveAllBars(p);

                    foreach (BlackJack.Client removeCl in table.clients)
                    {
                        PlayerModel removeTarget = GlobalEvents.GetPlayerFromSqlID(removeCl.ID);
                        if (removeTarget == null) continue;

                        Bar.RemoveAllBars(removeTarget);
                    }

                    PropStreamer.GetProp(table.PropID).Delete();
                    TextLabelStreamer.GetDynamicTextLabel(table.TextID).Delete();

                    BlackJackTables.Remove(table);
                    MainChat.SendInfoChat(p, "[BJ] Masa başarıyla kaldırıldı.");
                    return;

                default:
                    MainChat.SendInfoChat(p, "[Kullanım] /bj [çeşit] [varsa değer]<br>Çeşitler: minbahis - maxbahis - bahisbaslat - bahisbitir - bahis - siradaki - kartiste - kal - masakartac - katil - oyunbitir - masakaldir"); return;

            }

        }



        static void SendBlackJackMessage(BlackJack table, string message)
        {
            foreach (BlackJack.Client client in table.clients)
            {
                PlayerModel target = GlobalEvents.GetPlayerFromSqlID(client.ID);
                if (target == null) { table.clients.Remove(client); continue; }

                GlobalEvents.NativeNotify(target, message);
                continue;
            }

            PlayerModel owner = GlobalEvents.GetPlayerFromSqlID(table.ownerID);
            if (owner == null) { return; }
            GlobalEvents.NativeNotify(owner, message);
        }

        static void BJTextUpdate(BlackJack table)
        {
            PlayerLabel lbl = TextLabelStreamer.GetDynamicTextLabel(table.TextID);
            if (lbl == null) { return; }

            int tableCards = 0;
            string tableCards2 = "";
            table.tableCards.ForEach(x =>
            {
                tableCards += x; tableCards2 += "|" + x.ToString();
            });


            string isCurrent = "-";

            foreach (BlackJack.Client cl in table.clients)
            {
                PlayerModel ttt = GlobalEvents.GetPlayerFromSqlID(cl.ID);
                if (ttt != null)
                {

                    if (cl.isCurrentPlayer)
                    {
                        isCurrent = "~g~> " + ttt.characterName.Replace("_", " ");
                    }

                    Bar.RemoveAllBars(ttt);

                    foreach (BlackJack.Client bT in table.clients)
                    {
                        PlayerModel barTarget = GlobalEvents.GetPlayerFromSqlID(bT.ID);
                        if (barTarget == null) continue;


                        string color = (bT.isCurrentPlayer) ? "~g~" : "~r~";
                        string totalCard = ">";
                        bT.cards.ForEach(x => { totalCard += x.ToString() + " | "; });

                        Bar.CreatePlayerBar(ttt, bT.ID.ToString(), color + barTarget.characterName.Replace("_", " "), totalCard);
                    }

                    Bar.CreatePlayerBar(ttt, table.ownerID.ToString(), "~y~Masa: " + tableCards.ToString());
                    Bar.CreatePlayerBar(ttt, table.TextID.ToString(), "~b~BlackJack Masası");
                }

            }

            PlayerModel owner = GlobalEvents.GetPlayerFromSqlID(table.ownerID);
            if (owner != null)
            {
                Bar.RemoveAllBars(owner);
                foreach (BlackJack.Client bT in table.clients)
                {
                    PlayerModel barTarget = GlobalEvents.GetPlayerFromSqlID(bT.ID);
                    if (barTarget == null) continue;
                    //Bar.RemoveAllBars(barTarget);

                    string color = (bT.isCurrentPlayer) ? "~g~" : "~r~";
                    string totalCard = ">";
                    bT.cards.ForEach(x => { totalCard += " | " + x.ToString(); });

                    Bar.CreatePlayerBar(owner, bT.ID.ToString(), color + barTarget.characterName.Replace("_", " "), totalCard);
                }

                Bar.CreatePlayerBar(owner, table.ownerID.ToString(), "~y~Masa: " + tableCards.ToString());
                Bar.CreatePlayerBar(owner, table.TextID.ToString(), "~b~BlackJack Masası");
            }

            string status = "-";
            if (isCurrent.Length >= 5) { status = isCurrent; }
            else if (table.isGameStarted && table.isUsersCanBet) { status = "~g~Bahisler Açık"; }
            else { status = "Durum: ~y~Beklemede"; }

            string text = "~b~[BJ]~n~~w~ Bahis: ~y~" + table.minBet.ToString() + " | " + table.maxBet.ToString() + "~n~~w~MasaKart: ~y~" + tableCards + "~n~~w~Kart Sayısı: " + table.tableDefaultCards.Count + "~n~" + status;
            lbl.Text = text;
            lbl.Scale = 0.5f;
            lbl.Font = 0;

            return;
        }

        #endregion
    }
}