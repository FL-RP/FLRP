using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net; using AltV.Net.Async;
using AltV.Net.Data;
using outRp.Models;
using outRp.OtherSystem.NativeUi;
using outRp.Chat;
using outRp.Globals;
using outRp.OtherSystem.Textlabels;
using System.Linq;
using AltV.Net.Resources.Chat.Api;

namespace outRp.OtherSystem.LSCsystems
{
    public class RacingSystem : IScript
    {
        /*public class RaceModel
        {
            public Position endPos { get; set; } = new Position(0,0,0);
            public int OwnerId { get; set; } = 0;
            public int totalBet { get; set; } = 0;
            public bool isRacestart { get; set; } = false;
            public bool isCanBet { get; set; } = true;
            public List<Racers> racers { get; set; } = new List<Racers>();
            public List<Betters> betClients { get; set; } = new List<Betters>();
            public List<CheckPoints> checkpoints { get; set; } = new List<CheckPoints>();
            public List<Invite> invites { get; set; } = new List<Invite>();

            public int checkpointCounter { get; set; } = 0;

            public DateTime startTime { get; set; } = DateTime.Now;

            public class Invite
            {
                public int ID { get; set; }
            }

            public class Racers
            {
                public int racer { get; set; }
                public string name { get; set; } = "yok";
                public VehModel racerVeh { get; set; }
                public DateTime finishTime { get; set; } = DateTime.Now;
                public bool ShowFinish { get; set; } = false;
            }

            public class Betters
            {
                public int betClient { get; set; }
                public int betPrice { get; set; }
                public int betTo { get; set; }
            }

            public class CheckPoints
            {
                public int ID { get; set; }
                public Position pos { get; set; }
            }
            
        }

        public static List<RaceModel> serverRaceList = new List<RaceModel>();

        [Command("yariskur")]
        public static void COM_CreateRace(PlayerModel p)
        {
            try
            {
                RaceModel checkRace = serverRaceList.Find(x => x.OwnerId == p.sqlID);
                if (checkRace != null) { MainChat.SendErrorChat(p, "[HATA] Zaten bir yarış kurmuşsunuz. Lütfen önce güncel yarışı sonlandırın."); return; }
                FactionModel fact = Database.DatabaseMain.GetFactionInfo(p.factionId);
                if (fact.type != ServerGlobalValues.fType_Racers) { MainChat.SendErrorChat(p, "[HATA] Sadece birlik tipi yarışçı olan kişiler yarış düzenleyebilir."); return; }

                RaceModel nRace = new RaceModel();
                nRace.OwnerId = p.sqlID;

                serverRaceList.Add(nRace);
                MainChat.SendInfoChat(p, "Yarış başarıyla oluşturuldu.");
                return;
            }
            catch { return; }
        }

        [Command("yaris")]
        public async void COM_EditRace(PlayerModel p, params string[] args)
        {
            try
            {
                if (args.Length <= 0) { MainChat.SendInfoChat(p, "[Kullanım] /yaris [cesit] [varsa değer]<br>Kullanılabilecek çeşitler:<br>cp, davet, kabulet, bahisbaşlat, hazır, baslat, kapat, bahis, resetle"); return; }


                switch (args[0])
                {
                    case "cp":
                        RaceModel race = serverRaceList.Find(x => x.OwnerId == p.sqlID);
                        if (race == null) { MainChat.SendErrorChat(p, "[HATA] Kurulmuş bir yarış yok, lütfen önce yeni bir yarış kurun."); return; }
                        if (race.checkpoints.Count >= 40) { MainChat.SendErrorChat(p, "[HATA] En fazla 10 adet nokta eklenebilir."); return; }

                        race.checkpointCounter++;
                        RaceModel.CheckPoints nCP = new RaceModel.CheckPoints();
                        nCP.ID = race.checkpointCounter;
                        nCP.pos = p.Position;
                        race.checkpoints.Add(nCP);
                        MainChat.SendInfoChat(p, "> Yeni nokta başarıyla eklendi.");
                        return;

                    case "davet":
                        RaceModel race2 = serverRaceList.Find(x => x.OwnerId == p.sqlID);
                        if (race2 == null) { MainChat.SendErrorChat(p, "[HATA] Kurulmuş bir yarış yok, lütfen önce yeni bir yarış kurun."); return; }
                        if (race2.checkpoints.Count <= 0) { MainChat.SendErrorChat(p, "[HATA] Yarışta hiç bitiş noktası yok!"); return; }
                        if (args.Length <= 1) { MainChat.SendInfoChat(p, "[Kullanım] /yarış davet [id]"); return; }
                        int Dtarget; bool isDtarget = Int32.TryParse(args[1], out Dtarget);
                        if (isDtarget == false) { MainChat.SendErrorChat(p, "[HATA] Girilen ID değeri hatalı."); return; }
                        PlayerModel dT = GlobalEvents.GetPlayerFromSqlID(Dtarget);
                        if (dT == null) { MainChat.SendErrorChat(p, "[HATA] Oyuncu bulunamadı."); return; }
                        if (dT.Position.Distance(p.Position) > 10) { MainChat.SendErrorChat(p, "[HATA] Oyuncuya yeterli yakınlıkta değilsiniz."); return; }

                        RaceModel.Invite Ninvite = new RaceModel.Invite()
                        {
                            ID = dT.sqlID,
                        };

                        race2.invites.Add(Ninvite);
                        MainChat.SendInfoChat(p, dT.fakeName.Replace("_", " ") + " isimli oyuncuyu yarışa katılması için davet ettiniz.");
                        MainChat.SendInfoChat(dT, p.fakeName.Replace("_", " ") + " isimli oyuncu sizi yarışa katılmanız için davet etti.<br>Kabul etmek için /yaris kabulet");
                        return;

                    case "kabulet":
                        if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[HATA] Bu komutu kullanabilmek için bir araçta olmalısınız."); return; }
                        RaceModel joinRace = null;

                        foreach (RaceModel c in serverRaceList)
                        {
                            RaceModel.Invite a = c.invites.Find(x => x.ID == p.sqlID);
                            if (a != null) { joinRace = c; }
                        }
                        if (joinRace == null) { MainChat.SendErrorChat(p, "[HATA] Bir yarış davetiniz bulunmuyor."); return; }

                        RaceModel.Racers nRacer = new RaceModel.Racers();
                        nRacer.racer = p.sqlID;
                        nRacer.racerVeh = (VehModel)p.Vehicle;
                        nRacer.name = p.characterName.Replace("_", " ");
                        joinRace.racers.Add(nRacer);

                        SendNotifiyToRace(joinRace, p.characterName.Replace("_", " ") + " isimli kişi yarışçı olarak katıldı.", "~g~Yeni yarışçı dahil oldu.", p);
                        return;

                    case "bahisbaşlat":
                        RaceModel betRace = serverRaceList.Find(x => x.OwnerId == p.sqlID);
                        if (betRace == null) { MainChat.SendErrorChat(p, "[HATA] Aktif yarış bulunamadı."); return; }
                        if (betRace.isCanBet) { MainChat.SendErrorChat(p, "[HATA] Zaten bahisler aktif hala getirilmiş."); return; }
                        if (betRace.checkpoints.Count <= 0) { MainChat.SendErrorChat(p, "[HATA] Yarışta hiç bitiş noktası yok!"); return; }

                        betRace.isCanBet = true;
                        SendNotifiyToRace(betRace, "Yarış için bahisler başladı! 20 Saniye boyunca bahis yapabilirsiniz.", "~g~Bahisler açıldı", p);
                        await Task.Delay(20000);

                        if (betRace == null)
                            return;
                        betRace.isCanBet = false;
                        SendNotifiyToRace(betRace, "~r~Yarış için bahisler kapandı! 20 Saniye boyunca bahis yapabilirsiniz.", "~r~Bahisler kapandı", p);
                        return;

                    case "hazır":
                        RaceModel startR = serverRaceList.Find(x => x.OwnerId == p.sqlID);
                        if (startR == null) { MainChat.SendErrorChat(p, "[HATA] Aktif yarış bulunamadı."); return; }
                        if (startR.isRacestart) { MainChat.SendErrorChat(p, "[HATA] Yarış zaten başlamış."); return; }
                        if (startR.checkpoints.Count <= 0) { MainChat.SendErrorChat(p, "[HATA] Yarışta hiç bitiş noktası yok!"); return; }
                        if (startR.racers.Count <= 0) { MainChat.SendErrorChat(p, "[HATA] Yarış içerisinde hiç bir yarışçı yok!"); return; }
                        /*foreach(RaceModel.Racers racerReady in startR.racers)
                                                {
                                                    PlayerModel readyRacer = GlobalEvents.GetPlayerFromSqlID(racerReady.racer);
                                                    if(readyRacer == null) { continue; }
                                                    GlobalEvents.FreezeEntity(readyRacer, true);
                                                }  */
        /*
                        SendNotifiyToRace(startR, "Yarışçılar hazır konuma geçti. Yarış başlamak üzere.", "~g~Yarış başlama üzere.", p);
                        return;

                    case "baslat":
                        RaceModel sR = serverRaceList.Find(x => x.OwnerId == p.sqlID);
                        if (sR == null) { MainChat.SendErrorChat(p, "[HATA] Aktif yarış bulunamadı."); return; }
                        if (sR.isRacestart) { MainChat.SendErrorChat(p, "[HATA] Yarış zaten başlamış durumda."); return; }
                        if (sR.checkpoints.Count <= 0) { MainChat.SendErrorChat(p, "[HATA] Yarışta hiç bitiş noktası yok!"); return; }

                        ShowLeaderBoard(sR);

                        sR.isRacestart = true;
                        sR.startTime = DateTime.Now;

                        SendNotifiyToRace(sR, "Yarış başlıyor!", "~g~Yarış başlıyor.", p);
                        await Task.Delay(1000);
                        SendNotifiyToRace2(sR, "~y~Hazır");
                        await Task.Delay(1000);
                        SendNotifiyToRace2(sR, "~r~3");
                        await Task.Delay(1000);
                        SendNotifiyToRace2(sR, "~r~2");
                        await Task.Delay(1000);
                        SendNotifiyToRace2(sR, "~r~1");
                        await Task.Delay(1000);
                        SendNotifiyToRace2(sR, "~g~BAŞLA");
                        foreach (RaceModel.Racers sRacer in sR.racers)
                        {
                            PlayerModel rr = GlobalEvents.GetPlayerFromSqlID(sRacer.racer);
                            if (rr == null) { continue; }
                            GlobalEvents.UINotifiy(rr, 6, "~r~BASLA", time: 500);
                            GlobalEvents.PlaySound(rr, "UaUa_0qPPgc");
                            RaceModel.CheckPoints rrC = sR.checkpoints.Find(x => x.ID == 1);
                            GlobalEvents.CheckpointCreate(rr, rrC.pos, 60, 13, new Rgba(0, 191, 255, 150), "RaceCP", "1");
                            //GlobalEvents.FreezeEntity(p, false);
                        }
                        return;

                    case "kapat":
                        RaceModel endR = serverRaceList.Find(x => x.OwnerId == p.sqlID);
                        if (endR == null) { return; }
                        FinishRace(endR);
                        serverRaceList.Remove(endR);
                        return;

                    case "bahis":
                        int bTarget; int selected; int value;
                        bool bTargetOk = Int32.TryParse(args[1], out bTarget);
                        bool selectedOk = Int32.TryParse(args[2], out selected);
                        bool valueOk = Int32.TryParse(args[3], out value);

                        if (!bTargetOk || !selectedOk || !valueOk) { MainChat.SendInfoChat(p, "[Kullanım] /yarış bahis [y.Sahibi ID] [Yarışçı ID] [Miktar]"); return; }

                        RaceModel.Betters newBetter = new RaceModel.Betters()
                        {
                            betClient = p.sqlID,
                            betPrice = value,
                            betTo = selected
                        };

                        RaceModel jBet = serverRaceList.Find(x => x.OwnerId == bTarget);
                        if (jBet == null) { MainChat.SendErrorChat(p, "[HATA] Yarış bulunamadı."); }

                        jBet.betClients.Add(newBetter);

                        SendNotifiyToRace(jBet, p.fakeName.Replace("_", " ") + " isimli kişi " + selected.ToString() + " numaralı yarışçıya $" + value.ToString() + " bahis oynadı.", "~g~Yeni Bahisçi.", p);
                        return;

                    case "resetle":
                        RaceModel reset = serverRaceList.Find(x => x.OwnerId == p.sqlID);
                        if (reset == null)
                            return;

                        reset.invites = new List<RaceModel.Invite>();
                        reset.isCanBet = true;
                        reset.isRacestart = false;
                        reset.racers = new List<RaceModel.Racers>();
                        reset.totalBet = 0;
                        reset.betClients = new List<RaceModel.Betters>();
                        FinishRace(reset);

                        MainChat.SendInfoChat(p, "[?] Yarış bilgileri sıfırlandı. (Yarış rotası hala kayıtlı.)");
                        return;
                }

            }
            catch
            {
                return;                
            }
        }


        [AsyncClientEvent("RaceCP")]
        public void RaceCP(PlayerModel p, string cpa)
        {
            try
            {
                int cp = Int32.Parse(cpa);
                RaceModel race = null;

                foreach (RaceModel c in serverRaceList)
                {
                    RaceModel.Racers a = c.racers.Find(x => x.racer == p.sqlID);
                    if (a != null) { race = c; }
                }
                if (race == null) { MainChat.SendErrorChat(p, "[HATA] Yarış bulunamadı."); return; }

                if (race.checkpoints.Count <= cp)
                {
                    RaceModel.Racers rrr = race.racers.Find(x => x.racer == p.sqlID);
                    if (rrr == null) { return; }
                    rrr.ShowFinish = true;
                    rrr.finishTime = DateTime.Now;
                    ShowLeaderBoard(race);
                    GlobalEvents.UINotifiy(p, 1, "Yaris Bitti", "Yarisi tamamlandiniz.");
                    return;
                }  // ! Finish event

                RaceModel.CheckPoints nCp = race.checkpoints.Find(x => x.ID == (cp + 1));
                if (nCp == null) { return; }

                GlobalEvents.CheckpointCreate(p, nCp.pos, 60, 13, new Rgba(0, 191, 255, 150), "RaceCP", nCp.ID.ToString());
            }
            catch { return; }
        }

        public static void ShowLeaderBoard(RaceModel race)
        {
            try
            {
                foreach (RaceModel.Betters b in race.betClients)
                {
                    PlayerModel p = GlobalEvents.GetPlayerFromSqlID(b.betClient);
                    Bar.RemoveAllBars(p);
                    if (p == null) { continue; }
                    foreach (RaceModel.Racers racer in race.racers)
                    {
                        var currTime = (racer.finishTime - race.startTime).TotalSeconds;
                        var model = (VehicleModel)racer.racerVeh.Model;
                        Bar.CreatePlayerBar(p, "racer_" + racer.racer.ToString(), racer.name, " " + model.ToString());
                        if (racer.ShowFinish)
                        {
                            Bar.CreateTextBar(p, "racertime_" + racer.racer.ToString(), "Süre: " + currTime.ToString() + " sn");
                        }
                    }
                    Bar.CreateTextBar(p, "rac", "~g~Yarış");
                }

                PlayerModel o = GlobalEvents.GetPlayerFromSqlID(race.OwnerId);       //
                Bar.RemoveAllBars(o);
                if (o == null) { return; }

                foreach (RaceModel.Racers racer in race.racers)
                {
                    var currTime = (racer.finishTime - race.startTime).TotalSeconds;
                    var model = (VehicleModel)racer.racerVeh.Model;
                    Bar.CreatePlayerBar(o, "racer_" + racer.racer.ToString(), racer.name, " " + model.ToString());
                    if (racer.ShowFinish)
                    {
                        Bar.CreateTextBar(o, "racertime_" + racer.racer.ToString(), "Süre: " + currTime.ToString() + " sn");
                    }

                }
                Bar.CreateTextBar(o, "rac", "~g~Yarış");
            }
            catch { return; }
        }

        public static void FinishRace(RaceModel r)
        {
            try
            {
                foreach (RaceModel.Betters b in r.betClients)
                {
                    PlayerModel bb = GlobalEvents.GetPlayerFromSqlID(b.betClient);
                    if (bb == null) { continue; }
                    Bar.RemoveAllBars(bb);
                }

                foreach (RaceModel.Racers ra in r.racers)
                {
                    PlayerModel rr = GlobalEvents.GetPlayerFromSqlID(ra.racer);
                    if (rr == null) { continue; }
                    Bar.RemoveAllBars(rr);
                }


                PlayerModel o = GlobalEvents.GetPlayerFromSqlID(r.OwnerId);
                if (o == null) { return; }
                Bar.RemoveAllBars(o);
            }
            catch { return; }
        }


        public static void SendNotifiyToRace(RaceModel race, string text1, string text2, PlayerModel sender = null)
        {
            try
            {
                foreach (RaceModel.Racers r in race.racers)
                {
                    PlayerModel racer = GlobalEvents.GetPlayerFromSqlID(r.racer);
                    if (racer == null) { continue; }
                    GlobalEvents.ShowNotification(racer, text1, "~g~Yarış", "", icon: sender, blink: true);
                    GlobalEvents.NativeNotify(racer, text2);
                    GlobalEvents.SubTitle(racer, text1, 10);
                }

                foreach (RaceModel.Betters b in race.betClients)
                {
                    PlayerModel better = GlobalEvents.GetPlayerFromSqlID(b.betClient);
                    if (better == null) { return; }
                    GlobalEvents.ShowNotification(better, text1, "~g~Yarış", "", icon: sender, blink: true);
                    GlobalEvents.NativeNotify(better, text2);
                    GlobalEvents.SubTitle(better, text2, 10);
                }

                PlayerModel owner = GlobalEvents.GetPlayerFromSqlID(race.OwnerId);
                if (owner == null) return;
                GlobalEvents.ShowNotification(owner, text1, "~g~Yarış", "", icon: sender, blink: true);
                GlobalEvents.NativeNotify(owner, text2);
                GlobalEvents.SubTitle(owner, text2, 10);
            }
            catch { return; }

        }
        public static void SendNotifiyToRace2(RaceModel race, string text1)
        {
            try
            {
                foreach (RaceModel.Racers r in race.racers)
                {
                    PlayerModel racer = GlobalEvents.GetPlayerFromSqlID(r.racer);
                    if (racer == null) { continue; }
                    GlobalEvents.NativeNotify(racer, text1);
                }

                foreach (RaceModel.Betters b in race.betClients)
                {
                    PlayerModel better = GlobalEvents.GetPlayerFromSqlID(b.betClient);
                    if (better == null) { return; }
                    GlobalEvents.NativeNotify(better, text1);
                }

                PlayerModel owner = GlobalEvents.GetPlayerFromSqlID(race.OwnerId);
                if (owner == null) return;
                GlobalEvents.NativeNotify(owner, text1);
            }
            catch { return; }
        }
    }*/

        public class RaceModel
        {
            public int Owner { get; set; }
            public List<Client> clients { get; set; }
            public List<Position> positions { get; set; }
            public List<int> spectators { get; set; }

            // Race Stats 
            private bool _raceStarted { get; set; }
            public bool raceStarted
            {
                get { return _raceStarted; }
                set { _raceStarted = value; canInvite = !value; startTime = DateTime.Now;  }
            }
            public bool canInvite { get; set; } = true;
            public DateTime startTime { get; set; } = DateTime.Now;
            // Classes
            public class Client
            {
                public int ID { get; set; }
                public int currentCP { get; set; }
                public bool finished { get; set; }
                public double finishTime { get; set; } = 0;
            }
        }

        public static List<RaceModel> races = new();

        [Command("yariskur")]
        public void COM_CreateRace(PlayerModel p)
        {
            var check = races.Find(x => x.Owner == p.sqlID || x.clients.Find(y => y.ID == p.sqlID) != null);
            if(check == null)
            {
                RaceModel nRace = new();
                nRace.Owner = p.sqlID;
                nRace.positions = new();
                nRace.clients = new();
                nRace.spectators = new();
                nRace.raceStarted = false;

                races.Add(nRace);
                MainChat.SendInfoChat(p, "[Y] Yarış başarıyla kuruldu.");
                GlobalEvents.SetPlayerTag(p, "~g~/~w~yarisizle ~b~" + p.sqlID);
                return;
            }
            else
            {
                MainChat.SendErrorChat(p, "[HATA] Bir yarışa dahilsiniz. Lütfen önce yarıştan ayrılın/sonlandırın.");
                return;
            }

        }

        [Command("yaris")]
        public async void COM_Race(PlayerModel p, params string[] args)
        {
            if(args.Length <= 0) { MainChat.SendInfoChat(p, "[Y] ..."); return; }
            var race = races.Find(x => x.Owner == p.sqlID);
            if(race == null) { MainChat.SendErrorChat(p, "[HATA] Kurmuş olduğunuz bir yarış bulunmuyor."); return; }

            switch (args[0])
            {
                case "davet":
                    foreach(string _invite in args[1..])
                    {
                        if (!Int32.TryParse(_invite, out int invite))
                            continue;
                        var iPlayer = GlobalEvents.GetPlayerFromSqlID(invite);
                        if (iPlayer == null || !iPlayer.Exists)
                            continue;

                        Inputs.SendButtonInput(iPlayer, "Yarış daveti", "Race:Invite", "123," + p.sqlID);
                    }
                    return;

                case "cpekle":
                    race.positions.Add(p.Position);
                    MainChat.SendErrorChat(p, "[Y] CP eklendi. Toplam:" + race.positions.Count);
                    return;

                case "cpkaldir":
                    var nearPos = race.positions.Find(x => x.Distance(p.Position) < 5);
                    if (nearPos == null || nearPos == new Position(0, 0, 0)) { MainChat.SendErrorChat(p, "[Y] Yanınızda bir CP bulunamadı!"); return; }
                    race.positions.Remove(nearPos);

                    MainChat.SendErrorChat(p, "[Y] CP başarıyla kaldırıldı. Kalan: " + race.positions.Count);
                    return;

                case "baslat":
                    if (race.raceStarted) { MainChat.SendErrorChat(p, "[HATA] Yarış başlamış durumda."); return; }
                    MainChat.SendInfoChat(p, "Yaris baslatiliyor...");
                    await Task.Delay(1000);
                    SendInfoToRace(race, "3");

                    await Task.Delay(1000);
                    SendInfoToRace(race, "2");

                    await Task.Delay(1000);
                    SendInfoToRace(race, "1");

                    await Task.Delay(1000);
                    SendInfoToRace(race, "BAŞLA!!!");

                    race.raceStarted = true;
                    lock (race.clients)
                    {
                        race.clients.ForEach(x =>
                        {
                            PlayerModel racer = GlobalEvents.GetPlayerFromSqlID(x.ID);
                            if(racer != null)
                                GlobalEvents.CheckpointCreate(racer, race.positions[x.currentCP], 60, 8, new Rgba(0, 250, 0, 80), "Race:CpReached", race.Owner + "," + (x.currentCP + 1).ToString());
                        });
                    }

                    return;

                case "bitir":
                    race.raceStarted = false;
                    race.clients.ForEach(x =>
                    {
                        PlayerModel cli = GlobalEvents.GetPlayerFromSqlID(x.ID);
                        if (cli != null)
                            Bar.RemoveAllBars(cli);

                        x.finished = false;
                        x.currentCP = 0;
                        x.finishTime = 0;
                    });

                    race.spectators.ForEach(y =>
                    {
                        PlayerModel spc = GlobalEvents.GetPlayerFromSqlID(y);
                        if (spc != null)
                            Bar.RemoveAllBars(spc);
                    });

                    Bar.RemoveAllBars(p);
                    MainChat.SendInfoChat(p, "[Y] Yarış bitirildi.");
                    return;

                case "sonucgoster":
                    List<RaceModel.Client> final = new();
                    foreach(var fin in race.clients.FindAll(x => x.finished == true).OrderBy(x => x.finishTime))
                    {
                        final.Add(fin);
                    }
                    foreach(var fin2 in race.clients.FindAll(x => x.finished == false).OrderByDescending(x => x.currentCP))
                    {
                        final.Add(fin2);
                    }

                    string finText = "<center>Yarış Sonuçları</center>";
                    foreach(var gg in final)
                    {
                        PlayerModel nameChar = GlobalEvents.GetPlayerFromSqlID(gg.ID);
                        if (gg.finished)
                        {
                            finText += "<br>[" + (final.IndexOf(gg) + 1) + "] " + ((nameChar != null) ? nameChar.fakeName.Replace('_', ' ') : "İsimsiz") + " Süre:" + gg.finishTime + "/saniye";
                        }
                        else
                        {
                            finText += "<br>[" + (final.IndexOf(gg) + 1) + "] " + ((nameChar != null) ? nameChar.fakeName.Replace('_', ' ') : "İsimsiz") + " CP:" + (gg.currentCP - 1) + "/" + race.positions.Count;
                        }
                    }
                    MainChat.SendInfoChat(p, finText, true);

                    return;

                case "debugekle":
                    RaceModel.Client db_cl = new();
                    db_cl.ID = p.sqlID;
                    db_cl.currentCP = 0;
                    db_cl.finished = false;

                    race.clients.Add(db_cl);
                    MainChat.SendInfoChat(p, "+");
                    return;

                case "sil":
                    races.Remove(race);
                    MainChat.SendInfoChat(p, "[Y] Yarış başarıyla kaldırıldı.");
                    return;
            }
        }

        [AsyncClientEvent("Race:Invite")]
        public void EVENT_InviteRace(PlayerModel p, bool selection, string _val)
        {
            string[] val = _val.Split(',');
            if(!Int32.TryParse(val[1], out int raceID))
            {                
                MainChat.SendErrorChat(p, "[HATA] Yarış ile ilgili bir hata meydana geldi.");
                return;
            }

            var race = races.Find(x => x.Owner == raceID);
            if(race == null)
            {
                MainChat.SendErrorChat(p, "[HATA] Yarış ile ilgili bir hata meydana geldi.");
                return;
            }

            if (!selection)
            {
                MainChat.SendInfoChat(p, "[?] Yarış davetini reddettiniz.");
                PlayerModel RaceOwner = GlobalEvents.GetPlayerFromSqlID(raceID);
                if(RaceOwner != null || RaceOwner.Exists)
                {
                    MainChat.SendInfoChat(RaceOwner, "[?] " + p.characterName.Replace('_', ' ') + " isimli oyuncu yarış davetini reddetti.");
                }
                return;
            }
            else
            {
                var checkCurrent = races.Find(x => x.Owner == p.sqlID || x.clients.Find(x => x.ID == p.sqlID) != null);
                if(checkCurrent != null)
                {
                    MainChat.SendErrorChat(p, "[HATA] Zaten bir yarışa dahil edilmişsiniz.");
                    return;
                }
                else
                {
                    RaceModel.Client nClient = new();
                    nClient.ID = p.sqlID;
                    nClient.currentCP = 0;
                    nClient.finished = false;

                    race.clients.Add(nClient);

                    SendInfoToRace(race, p.fakeName.Replace('_', ' ') + " isimli kişi yarışa katıldı.");
                    return;
                }
            }
        }

        [Command("yarisizle")]
        public void COM_SpectateRace(PlayerModel p, params string[] args)
        {
            var check = races.Find(x => x.Owner == p.sqlID || x.clients.Find(y => y.ID == p.sqlID) != null || x.spectators.Contains(p.sqlID));
            if(check != null) { MainChat.SendErrorChat(p, "[HATA] Bir yarışta yer alıyorsunuz. Lütfen önce yarıştan ayrılın."); return; }

            if(args.Length <= 0) { MainChat.SendInfoChat(p, "[Kullanım] /yarisizle [id]"); return; }
            if(!Int32.TryParse(args[0], out int tSql)) { MainChat.SendInfoChat(p, "[Kullanım] /yarisizle [id]"); return; }

            var race = races.Find(x => x.Owner == tSql);
            if (race == null) { MainChat.SendErrorChat(p, "[HATA] Yarış bulunamadı."); return; }

            race.spectators.Add(p.sqlID);
            MainChat.SendInfoChat(p, "[Y] Yarışa izleyici olarak katıldınız.");
            return;
        }
        public void SendInfoToRace(RaceModel race, string message)
        {
            var owner = GlobalEvents.GetPlayerFromSqlID(race.Owner);
            if(owner != null)
            {
                GlobalEvents.ShowLeftNotify(owner, message, true, FlashColor: new int[4] { 10, 10, 10, 10 });
            }

            foreach(var c in race.clients)
            {
                PlayerModel cli = GlobalEvents.GetPlayerFromSqlID(c.ID);
                if(cli != null)
                    GlobalEvents.ShowLeftNotify(cli, message, true, FlashColor: new int[4] { 10,10,10,10 });
            }
        }

        public void UpdateRaceBar(RaceModel race)
        {
            lock (race)
            {
                var res = GetRaceResult(race);
                string res1 = "YOK";
                if (res.Count == 1)
                {
                    var _res1 = GlobalEvents.GetPlayerFromSqlID(res[0].ID);
                    res1 = _res1.fakeName.Replace('_', ' ');
                    if (res[0].finished)
                        res1 += " S:" + res[0].finishTime;
                }

                string res2 = "YOK";
                if (res.Count == 2)
                {
                    var _res2 = GlobalEvents.GetPlayerFromSqlID(res[1].ID);
                    res2 = _res2.fakeName.Replace('_', ' ');
                    if (res[1].finished)
                        res2 += " S:" + res[1].finishTime;
                }

                string res3 = "YOK";
                if (res.Count == 3)
                {
                    var _res3 = GlobalEvents.GetPlayerFromSqlID(res[2].ID);
                    res3 = _res3.fakeName.Replace('_', ' ');
                    if (res[2].finished)
                        res3 += " S:" + res[2].finishTime;
                }

                PlayerModel owner = GlobalEvents.GetPlayerFromSqlID(race.Owner);
                if (owner != null)
                {
                    /*MainChat.SendInfoChat(owner, "Sıralama: <br>1. " + res1 + "<br>2. " + res2 + "<br>3. " + res3);
                    Bar.RemoveAllBars(owner);
                    Bar.CreatePlayerBar(owner, "res1", res1, "~g~1");
                    Bar.CreatePlayerBar(owner, "res2", res2, "~y~2");
                    Bar.CreatePlayerBar(owner, "res3", res3, "~o~3");
                    Bar.CreatePlayerBar(owner, "resname", "Yaris", "Bilgileri");*/
                    ShowRaceBar(owner, race);
                }

                foreach (var cl in race.clients)
                {
                    PlayerModel cli = GlobalEvents.GetPlayerFromSqlID(cl.ID);
                    if (cli != null)
                    {
                        /*MainChat.SendInfoChat(cli, "Sıralama: <br>1. " + res1 + "<br>2. " + res2 + "<br>3. " + res3);
                        Bar.RemoveAllBars(cli);
                        Bar.CreatePlayerBar(cli, "res1", res1, "~g~1");
                        Bar.CreatePlayerBar(cli, "res2", res2, "~y~2");
                        Bar.CreatePlayerBar(cli, "res3", res3, "~o~3");
                        Bar.CreateUpdownBar(cli, "resname", "Yaris", "Bilgileri");*/
                        ShowRaceBar(cli, race);
                    }
                }

                foreach (var sp in race.spectators)
                {
                    PlayerModel spec = GlobalEvents.GetPlayerFromSqlID(sp);
                    if (spec != null)
                    {
                        /*MainChat.SendInfoChat(spec, "Sıralama: <br>1. " + res1 + "<br>2. " + res2 + "<br>3. " + res3);
                        Bar.RemoveAllBars(spec);
                        Bar.CreatePlayerBar(spec, "res1", res1, "~g~1");
                        Bar.CreatePlayerBar(spec, "res2", res2, "~y~2");
                        Bar.CreatePlayerBar(spec, "res3", res3, "~o~3");
                        Bar.CreateUpdownBar(spec, "resname", "Yaris", "Bilgileri");*/
                        ShowRaceBar(spec, race);
                    }
                }
            }
            
        }

        public static void ShowRaceBar(PlayerModel p, RaceModel r)
        {
            Bar.RemoveAllBars(p);
            var result = GetRaceResult(r);

            for(int a = 0; a <= 3; a++)
            {
                if(result[a] != null)
                {
                    var res = result[a];
                    PlayerModel resTarget = GlobalEvents.GetPlayerFromSqlID(result[a].ID);
                    if(resTarget != null)
                    {
                        if (res.finished)
                        {
                            Bar.CreatePlayerBar(p, "res" + a.ToString(), "~g~" + resTarget.fakeName.Replace('_', ' ') + " - " + res.finishTime.ToString(), "~b~" + (a + 1).ToString());
                        }
                        else
                        {
                            Bar.CreatePlayerBar(p, "res" + a.ToString(), "~g~" + resTarget.fakeName.Replace('_', ' '), res.currentCP + "/" + r.positions.Count() + " | ~b~" + (a + 1).ToString());
                        }
                    }                        
                    else
                        Bar.CreatePlayerBar(p, "res" + a.ToString(), "~r~" + "YOK", "~b~" + (a + 1).ToString());
                }
                else
                {
                    Bar.CreatePlayerBar(p, "res" + a.ToString(), "~r~" + "YOK", "~b~" + (a + 1).ToString());
                }
                
            }

            if(r.clients.Find(x => x.ID == p.sqlID) != null)
            {
                Bar.CreatePlayerBar(p, "resname", "Yaris", "~b~CP: ~w~" + r.clients.Find(x => x.ID == p.sqlID).currentCP.ToString() + "/" + r.positions.Count.ToString());
            }
            else
            {
                Bar.CreatePlayerBar(p, "resname", "Yaris", "Mod: ~y~İzleyici");
            }
        }
        
        public static List<RaceModel.Client> GetRaceResult(RaceModel race)
        {
            /*List<RaceModel.Client> clients = new();

            var _finisheds = r.clients.FindAll(x => x.finished == true);
            var finisheds = _finisheds.OrderByDescending(x => x.finishTime);

            lock(finisheds)
            {
                foreach(var fn in finisheds)
                {
                    clients.Add(fn);
                }
            }

            var _contin = r.clients.FindAll(x => x.finished == false);
            var contin = _contin.OrderByDescending(x => x.currentCP);

            lock (contin)
            {
                foreach (var cn in contin)
                    clients.Add(cn);
            }

            return clients;*/
            List<RaceModel.Client> final = new();
            foreach (var fin in race.clients.FindAll(x => x.finished == true).OrderBy(x => x.finishTime))
            {
                final.Add(fin);
            }
            foreach (var fin2 in race.clients.FindAll(x => x.finished == false).OrderByDescending(x => x.currentCP))
            {
                final.Add(fin2);
            }

            return final;
        }

        [AsyncClientEvent("Race:CpReached")]
        public void EVENT_CP_Reached(PlayerModel p, string _val)
        {
            string[] val = _val.Split(',');
            if (!Int32.TryParse(val[0], out int raceID) || !Int32.TryParse(val[1], out int crCP))
                return;

            var race = races.Find(x => x.Owner == raceID);
            if (race == null)
                return;

            var raceClient = race.clients.Find(x => x.ID == p.sqlID);
            if (raceClient == null)
                return;
            
            // CP hesaplama
            if (crCP + 1 > race.positions.Count)
            {
                raceClient.currentCP = -1;
                raceClient.finished = true;
                raceClient.finishTime = (DateTime.Now - race.startTime).TotalSeconds;
                SendInfoToRace(race, "[*] " + p.fakeName.Replace('_', ' ') + " isimli kişi yarışı bitirdi.");
            }
            else
            {
                GlobalEvents.CheckpointCreate(p, race.positions[crCP], 60, 8, new Rgba(0, 250, 0, 80), "Race:CpReached", race.Owner + "," + (crCP + 1).ToString());
                raceClient.currentCP += 1;
            }
            
            UpdateRaceBar(race);
        }
    }
}
