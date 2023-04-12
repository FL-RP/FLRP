

/*namespace outRp.OtherSystem.LSCsystems
{
    public class Ball
    {
        public int BizID { get; set; }
        public int gameType { get; set; } = 0;
        public int owner { get; set; } = 0;
        public List<int> team1 { get; set; } = new List<int>();
        public List<int> team2 { get; set; } = new List<int>();
        public int team1_score { get; set; } = 0;
        public int team2_score { get; set; } = 0;
        public int points_left { get; set; } = 100;
        public bool gameStarted { get; set; } = false;
        public Position forceExit { get; set; }
    }

    public class Paintball : IScript
    {
        public static List<Ball> serverBalls = new List<Ball>();
        // ! GamePlay events
        [Command("pbkur")]
        public static async Task startPaintball(PlayerModel p, params string[] args)
        {
            PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Find(x => p.Position.Distance(x.Position) < 15f && x.Dimension == p.Dimension);
            if (entranceLabel == null) { MainChat.SendErrorChat(p, "[HATA] Yakınınızda bir iş yeri bulunamadı veya bir iş yeri içerisinde değilsiniz."); return; }
            entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position bussinesPos);

            (BusinessModel, Marker, PlayerLabel) biz = await Props.Business.getBusinessFromPos(bussinesPos);
            if (biz.Item1 == null) { MainChat.SendErrorChat(p, "[HATA] Yakınınızda bir iş yeri bulunamadı."); return; }

            if (biz.Item1.type != ServerGlobalValues.paintballBusiness || await Props.Business.CheckBusinessKey(p, biz.Item1) == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }

            if (biz.Item1.settings.paintBall == null) { MainChat.SendErrorChat(p, "[HATA] İş yerinin paint-ball ayarları henüz yapılmamış. Lütfen önce paintball ayarlarınızı tamamlayın."); return; }

            Ball check = serverBalls.Find(x => x.BizID == biz.Item1.ID);
            if (check != null) { MainChat.SendErrorChat(p, "[HATA] Zaten bir paintball oyunu kurulmuş."); return; }

            check = new Ball();
            check.BizID = biz.Item1.ID;
            check.forceExit = p.Position;
            check.owner = p.sqlID;
            serverBalls.Add(check);
            MainChat.SendInfoChat(p, "[!] Paintball oyunu bşaarıyla kuruldu.");
            return;
        }

        [Command("pbdavet")]
        public static async Task PaintBallInvite(PlayerModel p, params string[] args)
        {
            if (args.Length < 1) { MainChat.SendInfoChat(p, "[Kullanım] /pbdavet [takim1-takim2] [Oyuncu ID]"); return; }
            PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Find(x => p.Position.Distance(x.Position) < 15f && x.Dimension == p.Dimension);
            if (entranceLabel == null) { MainChat.SendErrorChat(p, "[HATA] Yakınınızda bir iş yeri bulunamadı veya bir iş yeri içerisinde değilsiniz."); return; }
            entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position bussinesPos);

            (BusinessModel, Marker, PlayerLabel) biz = await Props.Business.getBusinessFromPos(bussinesPos);
            if (biz.Item1 == null) { MainChat.SendErrorChat(p, "[HATA] Yakınınızda bir iş yeri bulunamadı."); return; }

            if (biz.Item1.type != ServerGlobalValues.paintballBusiness || await Props.Business.CheckBusinessKey(p, biz.Item1) == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }

            if (biz.Item1.settings.paintBall == null) { MainChat.SendErrorChat(p, "[HATA] İş yerinin paint-ball ayarları henüz yapılmamış. Lütfen önce paintball ayarlarınızı tamamlayın."); return; }

            Ball check = serverBalls.Find(x => x.BizID == biz.Item1.ID);
            if (check == null) { MainChat.SendInfoChat(p, "[HATA] Paintball oyunu kurulmamış. Lütfen önce paintball oyununu kurun."); return; }
            if (check.gameStarted) { MainChat.SendErrorChat(p, "[HATA] Oyun başlamış durumdayken yeni oyuncu davet edemezsiniz."); return; }

            if (!Int32.TryParse(args[1], out int tsql)) { MainChat.SendInfoChat(p, "[Kullanım] /pbdavet [takim1-takim2] [Oyuncu ID]"); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tsql);
            if (t == null) { MainChat.SendErrorChat(p, "[HATA] Oyuncu bulunamadı."); return; }
            if (t.Position.Distance(p.Position) > 10 || t.Dimension != p.Dimension) { MainChat.SendErrorChat(p, "[HATA] Oyuncu yakınınızda olmalı."); return; }


            switch (args[0])
            {
                case "takim1":
                    Inputs.SendButtonInput(t, "PaintBall oyununa takım 1 için davet edildiniz.", "PaintBall:JoinEvent", biz.Item1.ID.ToString() + ",1");
                    MainChat.SendInfoChat(p, "[!] " + t.characterName.Replace("_", " ") + " isimli oyuncu 1. takıma davet edildi.");
                    return;

                case "takim2":
                    Inputs.SendButtonInput(t, "PaintBall oyununa takım 2 için davet edildiniz.", "PaintBall:JoinEvent", biz.Item1.ID.ToString() + ",2");
                    MainChat.SendInfoChat(p, "[!] " + t.characterName.Replace("_", " ") + " isimli oyuncu 2. takıma davet edildi.");
                    return;

                default:
                    MainChat.SendInfoChat(p, "[Kullanım] /pbdavet [takim1-takim2] [Oyuncu ID]");
                    return;
            }

        }

        [Command("pbbitir")]
        public static void endPaintBall(PlayerModel p)
        {
            Ball check = serverBalls.Find(x => x.owner == p.sqlID);
            if (check == null) { MainChat.SendErrorChat(p, "[HATA] Paintball maçı bulunamadı!"); return; }

            check.team1.ForEach(x =>
            {
                PlayerModel t1 = GlobalEvents.GetPlayerFromSqlID(x);
                if (t1 != null) { t1.Spawn(check.forceExit, 0); t1.RemoveAllWeapons(); Bar.RemoveAllBars(t1); t1.hp = 1000; t1.maxHp = 1000; Inventory.UpdatePlayerInventory(t1); p.DeleteData("inPaintball"); }
            });
            check.team2.ForEach(x =>
            {
                PlayerModel t2 = GlobalEvents.GetPlayerFromSqlID(x);
                if (t2 != null) {  t2.Spawn(check.forceExit, 0); t2.RemoveAllWeapons(); Bar.RemoveAllBars(t2); t2.hp = 1000; t2.maxHp = 1000; Inventory.UpdatePlayerInventory(t2); p.DeleteData("inPaintball"); }
            });
            PlayerModel owner = GlobalEvents.GetPlayerFromSqlID(check.owner);
            if (owner != null) { Bar.RemoveAllBars(owner); }
            serverBalls.Remove(check);
        }

        [AsyncClientEvent("PaintBall:JoinEvent")]
        public void Paintball_JoinEvent(PlayerModel p, bool state, string other)
        {
            if (!state)
                return;

            string[] _s = other.Split(',');
            if (!Int32.TryParse(_s[0], out int bizID) || !Int32.TryParse(_s[1], out int teamId)) { MainChat.SendErrorChat(p, "[HATA] Bir hata meydana geldi."); return; }

            Ball check = serverBalls.Find(x => x.BizID == bizID);
            if (check == null) { MainChat.SendErrorChat(p, "[HATA] Paintball oyunu bulunamadı."); return; }
            if (check.gameStarted) { MainChat.SendErrorChat(p, "[!] Maç başlamış durumdayken maça katılamazsınız!"); return; }

            if (teamId == 1)
            {
                check.team1.Add(p.sqlID);
            }
            else
            {
                check.team2.Add(p.sqlID);
            }
            p.SetData("inPaintball", bizID);
            SendMessage(check, "~w~" + p.characterName.Replace("_", " ") + " isimli oyuncu " + ((teamId == 1) ? "~r~takım 1~w~" : "~g~takım 2~w") + "'e katıldı.", 3);

        }

        [Command("pbbaslat")]
        public async Task PaintBall_Start(PlayerModel p)
        {
            PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Find(x => p.Position.Distance(x.Position) < 15f && x.Dimension == p.Dimension);
            if (entranceLabel == null) { MainChat.SendErrorChat(p, "[HATA] Yakınınızda bir iş yeri bulunamadı veya bir iş yeri içerisinde değilsiniz."); return; }
            entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position bussinesPos);

            (BusinessModel, Marker, PlayerLabel) biz = await Props.Business.getBusinessFromPos(bussinesPos);
            if (biz.Item1 == null) { MainChat.SendErrorChat(p, "[HATA] Yakınınızda bir iş yeri bulunamadı."); return; }

            if (biz.Item1.type != ServerGlobalValues.paintballBusiness || await Props.Business.CheckBusinessKey(p, biz.Item1) == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }

            if (biz.Item1.settings.paintBall == null) { MainChat.SendErrorChat(p, "[HATA] İş yerinin paint-ball ayarları henüz yapılmamış. Lütfen önce paintball ayarlarınızı tamamlayın."); return; }

            Ball check = serverBalls.Find(x => x.BizID == biz.Item1.ID);
            if (check == null) { MainChat.SendErrorChat(p, "[HATA] Kurulmuş bir paintball oyunu bulunmuyor!"); return; }
            if (check.team1.Count < 1 || check.team2.Count < 1) { MainChat.SendErrorChat(p, "[HATA] Başlayabilmek için iki takımda da en az iki oyuncu olmalı."); return; } // TODO 2 Ye çek.

            check.gameStarted = true;
            check.team1.ForEach(x =>
           {
               PlayerModel t1 = GlobalEvents.GetPlayerFromSqlID(x);
               if (t1 != null)
               {
                   t1.Position = biz.Item1.settings.paintBall.team1_pos; t1.GiveWeapon(biz.Item1.settings.paintBall.currentWeapon, 1000, true); t1.MaxHealth = 2000; t1.hp = 2000;
                   //if (t1.sex == 0)
                   //{
                   //    List<GlobalEvents.ClothingModel> clothes = new List<GlobalEvents.ClothingModel>();
                   //    clothes.Add(new GlobalEvents.ClothingModel() { cID = 11, iID = 241, tID = 0 });
                   //    clothes.Add(new GlobalEvents.ClothingModel() { cID = 4, iID = 96, tID = 0 });
                   //    clothes.Add(new GlobalEvents.ClothingModel() { cID = 6, iID = 83, tID = 1 });
                   //    GlobalEvents.SetClothSet(t1, clothes);
                   //}
                   //else
                   //{
                   //    List<GlobalEvents.ClothingModel> clothes = new List<GlobalEvents.ClothingModel>();
                   //    clothes.Add(new GlobalEvents.ClothingModel() { cID = 11, iID = 231, tID = 0 });
                   //    clothes.Add(new GlobalEvents.ClothingModel() { cID = 4, iID = 93, tID = 0 });
                   //    clothes.Add(new GlobalEvents.ClothingModel() { cID = 6, iID = 79, tID = 1 });
                   //    GlobalEvents.SetClothSet(t1, clothes);
                   //}
               }
           });
            check.team2.ForEach(x =>
            {
                PlayerModel t2 = GlobalEvents.GetPlayerFromSqlID(x);
                if (t2 != null)
                {
                    t2.Position = biz.Item1.settings.paintBall.team2_pos; t2.GiveWeapon(biz.Item1.settings.paintBall.currentWeapon, 1000, true); t2.maxHp = 2000; t2.hp = 2000;
                    //if (t2.sex == 0)
                    //{
                    //    List<GlobalEvents.ClothingModel> clothes = new List<GlobalEvents.ClothingModel>();
                    //    clothes.Add(new GlobalEvents.ClothingModel() { cID = 11, iID = 238, tID = 12 });
                    //    clothes.Add(new GlobalEvents.ClothingModel() { cID = 4, iID = 95, tID = 12 });
                    //    clothes.Add(new GlobalEvents.ClothingModel() { cID = 6, iID = 62, tID = 0 });
                    //    GlobalEvents.SetClothSet(t2, clothes);
                    //}
                    //else
                    //{
                    //    List<GlobalEvents.ClothingModel> clothes = new List<GlobalEvents.ClothingModel>();
                    //    clothes.Add(new GlobalEvents.ClothingModel() { cID = 11, iID = 228, tID = 12 });
                    //    clothes.Add(new GlobalEvents.ClothingModel() { cID = 4, iID = 92, tID = 12 });
                    //    clothes.Add(new GlobalEvents.ClothingModel() { cID = 6, iID = 59, tID = 0 });
                    //    GlobalEvents.SetClothSet(t2, clothes);
                    //}
                }
            });

            check.team1_score = 0;
            check.team2_score = 0;
            UpdateScoreBoard(check);
            SendMessage(check, "~g~Maç başlıyor!");
        }



        public static async Task Player_Damage(PlayerModel p, PlayerModel attacker)
        {
            p.hp -= 50;



            Ball paint = serverBalls.Find(x => x.BizID == p.lscGetdata<int>("inPaintball"));
            if (paint == null)
                return;

            if (p.Health > 1000 || p.hp > 1000)
                return;

            bool isTeam1 = (paint.team1.Find(x => x == attacker.sqlID) != 0);
            bool _isSameTeam = (paint.team1.Find(x => x == p.sqlID) != 0);
            bool isSameTeam = (isTeam1 == _isSameTeam) ? true : false;


            if (paint.gameStarted && !isSameTeam)
            {
                if (isTeam1)
                {
                    paint.team1_score += 1;
                }
                else
                {
                    paint.team2_score += 1;
                }
                UpdateScoreBoard(paint);
            }


            (BusinessModel, Marker, PlayerLabel) biz = await Props.Business.getBusinessById(p.lscGetdata<int>("inPaintball"));
            if (biz.Item1 == null)
                return;

            if (paint.team1_score >= 40)
            {
                SendMessage(paint, "Maçı ~r~Kırmızı Takım~w~ kazandı!");
                paint.team1.ForEach(x =>
                {
                    PlayerModel endPlayer = GlobalEvents.GetPlayerFromSqlID(x);
                    if (endPlayer != null) { endPlayer.Position = biz.Item1.settings.paintBall.waitingRoom; endPlayer.RemoveAllWeapons(); Bar.RemoveAllBars(endPlayer); }
                });
                paint.team2.ForEach(x =>
                {
                    PlayerModel endPlayer2 = GlobalEvents.GetPlayerFromSqlID(x);
                    if (endPlayer2 != null) { endPlayer2.Position = biz.Item1.settings.paintBall.waitingRoom; endPlayer2.RemoveAllWeapons(); Bar.RemoveAllBars(endPlayer2); }
                });
            }
            else if (paint.team2_score >= 40)
            {
                SendMessage(paint, "Maçı ~b~Mavi Takım~w~ kazandı!");
                paint.team1.ForEach(x =>
                {
                    PlayerModel endPlayer = GlobalEvents.GetPlayerFromSqlID(x);
                    if (endPlayer != null) { endPlayer.Position = biz.Item1.settings.paintBall.waitingRoom; endPlayer.RemoveAllWeapons(); Bar.RemoveAllBars(endPlayer); }
                });
                paint.team2.ForEach(x =>
                {
                    PlayerModel endPlayer2 = GlobalEvents.GetPlayerFromSqlID(x);
                    if (endPlayer2 != null) { endPlayer2.Position = biz.Item1.settings.paintBall.waitingRoom; endPlayer2.RemoveAllWeapons(); Bar.RemoveAllBars(endPlayer2); }
                });
            }
            else
            {
                if ((paint.team1.Find(x => x == p.sqlID) != 0))
                {

                    SendMessage(paint, "~b~" + attacker.characterName.Replace("_", " ") + " > ~r~" + p.characterName.Replace("_", " ") + "~w~ : +1 Skor", 3);
                    //p.Position = biz.Item1.settings.paintBall.team1_pos;
                    p.Spawn(biz.Item1.settings.paintBall.team1_pos, 0);
                    p.hp = 2000;
                    p.GiveWeapon(biz.Item1.settings.paintBall.currentWeapon, 1000, true);
                }
                else
                {
                    SendMessage(paint, "~r~" + attacker.characterName.Replace("_", " ") + " > ~b~" + p.characterName.Replace("_", " ") + "~w~ : +1 Skor", 3);
                    //p.Position = biz.Item1.settings.paintBall.team2_pos;
                    p.Spawn(biz.Item1.settings.paintBall.team2_pos, 0);
                    p.hp = 2000;
                    p.GiveWeapon(biz.Item1.settings.paintBall.currentWeapon, 1000, true);
                }
            }





        }


        // Send Info Messages
        public static void SendMessage(Ball ball, string message, int type = 0, int notitype = 1, string message2 = "")
        {
            if (type == 0)
            {
                ball.team1.ForEach(x =>
                {
                    PlayerModel t1 = GlobalEvents.GetPlayerFromSqlID(x);
                    if (t1 != null) { GlobalEvents.SubTitle(t1, message, 2); }
                });
                ball.team2.ForEach(x =>
                {
                    PlayerModel t2 = GlobalEvents.GetPlayerFromSqlID(x);
                    if (t2 != null) { GlobalEvents.SubTitle(t2, message, 2); }
                });
                PlayerModel owner = GlobalEvents.GetPlayerFromSqlID(ball.owner);
                if (owner != null)
                {
                    GlobalEvents.SubTitle(owner, message, 2);
                }
                return;
            }
            else if (type == 1)
            {
                ball.team1.ForEach(x =>
                {
                    PlayerModel t1 = GlobalEvents.GetPlayerFromSqlID(x);
                    if (t1 != null) { GlobalEvents.UINotifiy(t1, notitype, "PaintBall", message); }
                });
                ball.team2.ForEach(x =>
                {
                    PlayerModel t2 = GlobalEvents.GetPlayerFromSqlID(x);
                    if (t2 != null) { GlobalEvents.UINotifiy(t2, notitype, "PaintBall", message); }
                });
                PlayerModel owner = GlobalEvents.GetPlayerFromSqlID(ball.owner);
                if (owner != null)
                {
                    GlobalEvents.UINotifiy(owner, notitype, "PaintBall", message, message2);
                }
            }
            else
            {
                ball.team1.ForEach(x =>
               {
                   PlayerModel t3 = GlobalEvents.GetPlayerFromSqlID(x);
                   if (t3 != null) { GlobalEvents.NativeNotify(t3, message); }
               });
                ball.team2.ForEach(x =>
                {
                    PlayerModel t4 = GlobalEvents.GetPlayerFromSqlID(x);
                    if (t4 != null) { GlobalEvents.NativeNotify(t4, message); }
                });
                PlayerModel owner = GlobalEvents.GetPlayerFromSqlID(ball.owner);
                if (owner != null)
                {
                    GlobalEvents.NativeNotify(owner, message);
                }

                return;
            }
        }
        public static void UpdateScoreBoard(Ball ball)
        {
            ball.team1.ForEach(x =>
            {
                PlayerModel t1 = GlobalEvents.GetPlayerFromSqlID(x);
                if (t1 != null)
                {
                    Bar.RemoveAllBars(t1);
                    Bar.CreatePlayerBar(t1, "pb1", "~r~Kırmızı Takım~w~", ball.team1_score.ToString());
                    Bar.CreatePlayerBar(t1, "pb2", "~b~Mavi Takım~w~", ball.team2_score.ToString());
                }
            });
            ball.team2.ForEach(x =>
            {
                PlayerModel t2 = GlobalEvents.GetPlayerFromSqlID(x);
                if (t2 != null)
                {
                    Bar.RemoveAllBars(t2);
                    Bar.CreatePlayerBar(t2, "pb1", "~r~Kırmızı Takım~w~", ball.team1_score.ToString());
                    Bar.CreatePlayerBar(t2, "pb2", "~b~Mavi Takım~w~", ball.team2_score.ToString());
                }
            });
            PlayerModel owner = GlobalEvents.GetPlayerFromSqlID(ball.owner);
            if (owner != null)
            {
                Bar.RemoveAllBars(owner);
                Bar.CreatePlayerBar(owner, "pb1", "~r~Kırmızı Takım~w~", ball.team1_score.ToString());
                Bar.CreatePlayerBar(owner, "pb2", "~b~Mavi Takım~w~", ball.team2_score.ToString());
            }
        }

    }
}*/
