

/*namespace outRp.OtherSystem.LSCsystems
{
    public class piyango : IScript
    {
        public class Piyango
        {
            public int ID { get; set; }
            public List<int> tam { get; set; } = new List<int>();
            public List<int> yarim { get; set; } = new List<int>();
            public List<int> ceyrek { get; set; } = new List<int>();
            public bool selled { get; set; } = false;
        }

        public class ServerPiyango
        {
            public int winnerType { get; set; } = 0;
            public int winnerTicket { get; set; } = 0;
            public bool canBuyTicket { get; set; } = true;
            public int total { get; set; } = 0;
            
            public bool piyangoEnd { get; set; } = false;
            public List<Piyango> serverTickets = new List<Piyango>();
        }

        public static ServerPiyango serverPiyango = new ServerPiyango();

        public static void LoadPiyango(string data)
        {
            serverPiyango = JsonConvert.DeserializeObject<ServerPiyango>(data);
        }

        [Command("piyango")]
        public void buyTicket(PlayerModel p)
        {
            if (p.Position.Distance(new Position(-1247, -917, 11.4f)) > 5) { MainChat.SendErrorChat(p, "[HATA] Bilet alma noktasında olmalısınız."); return; }
            if (!serverPiyango.canBuyTicket) { MainChat.SendErrorChat(p, "[HATA] Bilet satışları şuan kapalı durumda."); return; }
            List<GuiMenu> gMenu = new List<GuiMenu>();
            GuiMenu ticket_1 = new GuiMenu { name = "Tam Bilet ($5000)", triger = "Piyango:BuyTicket", value = "1" };
            GuiMenu ticket_2 = new GuiMenu { name = "Yarım Bilet($2500)", triger = "Piyango:BuyTicket", value = "2" };
            GuiMenu ticket_3 = new GuiMenu { name = "Çeyrek Bilet($1250)", triger = "Piyango:BuyTicket", value = "3" };
            gMenu.Add(ticket_1);
            gMenu.Add(ticket_2);
            gMenu.Add(ticket_3);

            GuiMenu close = GuiEvents.closeItem;

            gMenu.Add(close);
            Gui y = new Gui()
            {
                image = "https://besthqwallpapers.com/Uploads/12-8-2021/174701/thumb2-2022-new-year-golden-2022-background-happy-new-year-2022-golden-leather-texture-2022-concepts.jpg",
                guiMenu = gMenu,
                color = "#1CDF75"
            };
            y.Send(p);
        }

        [AsyncClientEvent("Piyango:BuyTicket")]
        public async Task buy_Ticket(PlayerModel p, int selected)
        {
            if (serverPiyango.piyangoEnd) { MainChat.SendErrorChat(p, "[HATA] Piyango sonuçlanmış artık bilet alamazsınız!"); return; }
            Piyango curr = serverPiyango.serverTickets.Find(x => x.ID == p.sqlID);
            if(curr == null)
            {
                curr = new Piyango();
                curr.ID = p.sqlID;

                Random rnd = new Random();

                if (selected == 1)
                {
                    if (curr.tam.Count >= 15) { MainChat.SendErrorChat(p, "[HATA] Her biletten en fazla 15 adet alabilirsiniz."); return; }
                    // 1 00000
                    if (p.cash < 5000) { MainChat.SendErrorChat(p, CONSTANT.ERR_MoneyNotEnought); return; }
                    p.cash -= 5000;
                    p.updateSql();
                    int newTicket_1 = (rnd.Next(100000, 999999) + 10000000);
                    bool hasTicket = false;
                    while (hasTicket)
                    {
                        serverPiyango.serverTickets.ForEach(x =>
                        {
                            x.tam.ForEach(x =>
                            {
                                if (x == newTicket_1) { newTicket_1 = (rnd.Next(100000, 999999) + 1000000); }
                                else hasTicket = true;
                            });
                        });
                        await Task.Delay(50);
                    }
                    curr.tam.Add(newTicket_1);
                    MainChat.SendInfoChat(p, "🎀 " + newTicket_1 + " numaralı tam bileti başarıyla aldınız.");
                    serverPiyango.serverTickets.Add(curr);
                    await Database.DatabaseMain.SaveServerSettings();
                    return;
                }
                else if (selected == 2)
                {
                    if (curr.yarim.Count >= 15) { MainChat.SendErrorChat(p, "[HATA] Her biletten en fazla 15 adet alabilirsiniz."); return; }
                    // 1 00000
                    if (p.cash < 2500) { MainChat.SendErrorChat(p, CONSTANT.ERR_MoneyNotEnought); return; }
                    p.cash -= 2500;
                    p.updateSql();
                    int newTicket_2 = (rnd.Next(100000, 999999) + 20000000);
                    bool hasTicket = false;
                    while (hasTicket)
                    {
                        int count = 0;
                        serverPiyango.serverTickets.ForEach(x =>
                        {
                            x.ceyrek.ForEach(x =>
                            {
                                if (x == newTicket_2)
                                {
                                    count += 1;
                                    if (count >= 2)
                                    {
                                        newTicket_2 = (rnd.Next(100000, 999999) + 20000000);
                                    }
                                }
                                else hasTicket = true;
                            });
                        });
                        await Task.Delay(50);
                    }
                    curr.yarim.Add(newTicket_2);
                    MainChat.SendInfoChat(p, "🎀 " + newTicket_2 + " numaralı yarım bileti başarıyla aldınız.");
                    serverPiyango.serverTickets.Add(curr);
                    await Database.DatabaseMain.SaveServerSettings();
                    return;
                }
                else
                {
                    if (curr.ceyrek.Count >= 15) { MainChat.SendErrorChat(p, "[HATA] Her biletten en fazla 15 adet alabilirsiniz."); return; }
                    // 1 00000
                    if (p.cash < 1250) { MainChat.SendErrorChat(p, CONSTANT.ERR_MoneyNotEnought); return; }
                    p.cash -= 1250;
                    p.updateSql();
                    int newTicket_3 = (rnd.Next(100000, 999999) + 30003000);
                    bool hasTicket = false;
                    while (hasTicket)
                    {
                        int count = 0;
                        serverPiyango.serverTickets.ForEach(x =>
                        {
                            x.ceyrek.ForEach(x =>
                            {
                                if (x == newTicket_3)
                                {
                                    count += 1;
                                    if (count >= 4)
                                    {
                                        newTicket_3 = (rnd.Next(100000, 999999) + 3000000);
                                    }
                                }
                                else hasTicket = true;
                            });
                        });
                        await Task.Delay(50);
                    }
                    curr.ceyrek.Add(newTicket_3);
                    MainChat.SendInfoChat(p, "🎀 " + newTicket_3 + " numaralı çeyrek bileti başarıyla aldınız.");
                    serverPiyango.serverTickets.Add(curr);
                    await Database.DatabaseMain.SaveServerSettings();
                    return;
                }
            }
            else
            {
                Random rnd = new Random();

                if (selected == 1)
                {
                    if (curr.tam.Count >= 15) { MainChat.SendErrorChat(p, "[HATA] Her biletten en fazla 15 adet alabilirsiniz."); return; }
                    // 1 00000
                    if (p.cash < 5000) { MainChat.SendErrorChat(p, CONSTANT.ERR_MoneyNotEnought); return; }
                    p.cash -= 5000;
                    p.updateSql();
                    int newTicket_1 = (rnd.Next(100000, 999999) + 10000000);
                    bool hasTicket = false;
                    while (hasTicket)
                    {
                        serverPiyango.serverTickets.ForEach(x =>
                        {
                            x.tam.ForEach(x =>
                            {
                                if (x == newTicket_1) { newTicket_1 = (rnd.Next(100000, 999999) + 10000000); }
                                else hasTicket = true;
                            });
                        });
                        await Task.Delay(50);
                    }
                    
                    curr.tam.Add(newTicket_1);
                    MainChat.SendInfoChat(p, "🎀 " + newTicket_1 + " numaralı tam bileti başarıyla aldınız.");
                    await Database.DatabaseMain.SaveServerSettings();
                    return;
                }
                else if (selected == 2)
                {
                    if (curr.yarim.Count >= 15) { MainChat.SendErrorChat(p, "[HATA] Her biletten en fazla 15 adet alabilirsiniz."); return; }
                    // 1 00000
                    if (p.cash < 2500) { MainChat.SendErrorChat(p, CONSTANT.ERR_MoneyNotEnought); return; }
                    p.cash -= 2500;
                    p.updateSql();
                    int newTicket_2 = (rnd.Next(100000, 999999) + 20000000);
                    bool hasTicket = false;
                    while (hasTicket)
                    {
                        int count = 0;
                        serverPiyango.serverTickets.ForEach(x =>
                        {
                            x.ceyrek.ForEach(x =>
                            {
                                if (x == newTicket_2) { count += 1; 
                                if(count >= 2)
                                    {
                                        newTicket_2 = (rnd.Next(100000, 999999) + 20000000);
                                    }
                                }
                                else hasTicket = true;
                            });
                        });
                        await Task.Delay(50);
                    }
                    curr.yarim.Add(newTicket_2);
                    MainChat.SendInfoChat(p, "🎀 " + newTicket_2 + " numaralı yarım bileti başarıyla aldınız.");
                    await Database.DatabaseMain.SaveServerSettings();
                    return;
                }
                else
                {
                    if (curr.ceyrek.Count >= 15) { MainChat.SendErrorChat(p, "[HATA] Her biletten en fazla 15 adet alabilirsiniz."); return; }
                    // 1 00000
                    if (p.cash < 1250) { MainChat.SendErrorChat(p, CONSTANT.ERR_MoneyNotEnought); return; }
                    p.cash -= 1250;
                    p.updateSql();
                    int newTicket_3 = (rnd.Next(100000, 999999) + 30000000);
                    bool hasTicket = false;
                    while (hasTicket)
                    {
                        int count = 0;
                        serverPiyango.serverTickets.ForEach(x =>
                        {
                            x.ceyrek.ForEach(x =>
                            {
                                if (x == newTicket_3)
                                {
                                    count += 1;
                                    if (count >= 4)
                                    {
                                        newTicket_3 = (rnd.Next(100000, 999999) + 30000000);
                                    }
                                }
                                else hasTicket = true;
                            });
                        });
                        await Task.Delay(50);
                    }
                    curr.ceyrek.Add(newTicket_3);
                    MainChat.SendInfoChat(p, "🎀 " + newTicket_3 + " numaralı çeyrek bileti başarıyla aldınız.");
                    await Database.DatabaseMain.SaveServerSettings();
                    return;
                }
            }
           
        }

        [Command("piyangobilgi")]
        public static void COM_PiyangoInfo(PlayerModel p)
        {
            int total = 0;
            foreach(Piyango pi in serverPiyango.serverTickets)
            {
                total += pi.tam.Count * 5000;
                total += pi.yarim.Count * 2500;
                total += pi.ceyrek.Count * 1250;
            }
            string text = "<center>Yılbaşı Çekilişi</center><br>Alınan Biletlerden gelen: $" + total + "<br>Devlet Tarafından eklenen: $300.000<br>Toplam: " + (total + 300000);
            MainChat.SendInfoChat(p, text);
            return;
        }

        [Command("piyangobiletlerim")]
        public static void COM_SelfPiyangoInfo(PlayerModel p)
        {
            string text = "<center>Biletleriniz</center><br>";
            Piyango pp = serverPiyango.serverTickets.Find(x => x.ID == p.sqlID);
            if(pp == null) { text += "Bilet bulunamadı!"; }
            else
            {
                text += "Tam Biletler: ";
                pp.tam.ForEach(x => { text += " " + x.ToString(); });
                text += "<br>Yarım Biletler: ";
                pp.yarim.ForEach(x => { text += " " + x.ToString(); });
                text += "<br>Çeyrek Biletler: ";
                pp.ceyrek.ForEach(x => { text += " " + x.ToString(); });                    
            }

            MainChat.SendInfoChat(p, text);
            return;
        }

        [Command("piyangobiletsat")]
        public static void COM_SellPiyango(PlayerModel p)
        {
            if (p.Position.Distance(new Position(-1247, -917, 11.4f)) > 5) { MainChat.SendErrorChat(p, "[HATA] Bilet alma noktasında olmalısınız."); return; }
            if (!serverPiyango.piyangoEnd) { MainChat.SendErrorChat(p, "[!] Piyango henüz sonuçlanmamış. Bu durumda bilet satamazsınız."); return; }


            Piyango user = serverPiyango.serverTickets.Find(x => x.ID == p.sqlID);
            if(user == null) { MainChat.SendErrorChat(p, "[HATA] Biletiniz bulunmuyor."); return; }
            if (user.selled) { MainChat.SendErrorChat(p, "[HATA] Zaten biletinizi satmışsınız."); return; }
            int total = 0;
            string endText = "<center>Biletler & Kazanımlar</center><br>";
            List<int> current = user.tam;

            foreach(int x in current)
            {
                int kazanc = (int)getWiningPrice(x);
                endText += x.ToString() + " numaralı biletten $" + kazanc.ToString() + "<br>";
                total += kazanc;
            }
            current = user.yarim;

            foreach (int x in current)
            {
                int kazanc = (int)getWiningPrice(x);
                endText += x.ToString() + " numaralı biletten $" + kazanc.ToString() + "<br>";
                total += kazanc;
            }
            current = user.ceyrek;

            foreach (int x in current)
            {
                int kazanc = (int)getWiningPrice(x);
                endText += x.ToString() + " numaralı biletten $" + kazanc.ToString() + "<br>";
                total += kazanc;
            }
            endText += "Toplam: $" + total;
            MainChat.SendInfoChat(p, endText);
            user.selled = true;
            p.cash += total;
            p.updateSql();
            return;
        }


        [Command("piyangocek++++")]
        public async Task COM_StartPiyango(PlayerModel p)
        {
            if(p.adminLevel <= 3) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); }
            foreach(PlayerModel t in Alt.GetAllPlayers())
            {
                GlobalEvents.SubTitle(t, "~y~Yılbaşı çekilişi için toplar dönmeye başladı...", 3);
            }
            await Task.Delay(3000);
            int total = 0;
            foreach (Piyango pi in serverPiyango.serverTickets)
            {
                total += pi.tam.Count * 5000;
                total += pi.yarim.Count * 2500;
                total += pi.ceyrek.Count * 1250;
            }
            serverPiyango.total = total + 200000;

            Random rnd = new Random();
            serverPiyango.winnerType = rnd.Next(1, 3);
            if (serverPiyango.winnerType == 1) {
                Piyango xx = serverPiyango.serverTickets[rnd.Next(0, serverPiyango.serverTickets.Count)];
                serverPiyango.winnerTicket = xx.tam[rnd.Next(0, xx.tam.Count - 1)];
                PlayerModelInfo winner = await Database.DatabaseMain.getCharacterInfo(xx.ID);
                foreach (PlayerModel t in Alt.GetAllPlayers())
                {
                    GlobalEvents.SubTitle(t, "~w~Talihli bilet internet sayfasında ~g~[PANEL]", 5);
                }

            }
            else if (serverPiyango.winnerType == 2)
            {
                Piyango xx = serverPiyango.serverTickets[rnd.Next(0, serverPiyango.serverTickets.Count)];
                serverPiyango.winnerTicket = xx.yarim[rnd.Next(0, xx.yarim.Count - 1)];
                PlayerModelInfo winner = await Database.DatabaseMain.getCharacterInfo(xx.ID);
                foreach (PlayerModel t in Alt.GetAllPlayers())
                {
                    GlobalEvents.SubTitle(t, "~w~Talihli bilet internet sayfasında ~g~[PANEL]", 5);
                }
            }
            else
            {
                Piyango xx = serverPiyango.serverTickets[rnd.Next(0, serverPiyango.serverTickets.Count)];
                serverPiyango.winnerTicket = xx.ceyrek[rnd.Next(0, xx.ceyrek.Count - 1)];
                PlayerModelInfo winner = await Database.DatabaseMain.getCharacterInfo(xx.ID);
                foreach (PlayerModel t in Alt.GetAllPlayers())
                {
                    GlobalEvents.SubTitle(t, "~w~Talihli bilet internet sayfasında ~g~[PANEL]", 5);
                }
            }
            string type = "";
            if (serverPiyango.winnerType == 1) type = "Tam Bilete"; else if (serverPiyango.winnerType == 2) type = "Yarım Bilete"; else type = "Çeyrek Bilete";
            string text = "<center>Yılbaşı Piyango Sonuçları</center><br>Yılbaşı piyangosu " + type + " çıktı<br>Kazanan bilet: " + serverPiyango.winnerTicket;
            foreach(PlayerModel x in Alt.GetAllPlayers())
            {
                MainChat.SendInfoChat(p, text);
            }
            serverPiyango.piyangoEnd = true;
            serverPiyango.canBuyTicket = false;
            return;
        }

        public double getWiningPrice(int number)
        {
            string winnerNumber = serverPiyango.winnerTicket.ToString();
            string currentNumber = number.ToString();
            // 6 basamak
            if(serverPiyango.winnerType == 1)
            {
                if (winnerNumber == currentNumber) return getWinning(100);
                else if (winnerNumber.Substring(winnerNumber.Length - 4) == currentNumber.Substring(currentNumber.Length - 4)) return getWinning(0.4);
                else if (winnerNumber.Substring(winnerNumber.Length - 3) == currentNumber.Substring(currentNumber.Length - 3)) return getWinning(0.35);
                else if (winnerNumber.Substring(winnerNumber.Length - 2) == currentNumber.Substring(currentNumber.Length - 2)) return getWinning(0.15);
                else if (winnerNumber.Substring(winnerNumber.Length - 1) == currentNumber.Substring(currentNumber.Length - 1)) return getWinning(0.005);
            }
            else if (serverPiyango.winnerType == 2)
            {
                if (winnerNumber == currentNumber) return getWinning(50);
                else if (winnerNumber.Substring(winnerNumber.Length - 4) == currentNumber.Substring(currentNumber.Length - 4)) return getWinning(0.4);
                else if (winnerNumber.Substring(winnerNumber.Length - 3) == currentNumber.Substring(currentNumber.Length - 3)) return getWinning(0.1);
                else if (winnerNumber.Substring(winnerNumber.Length - 2) == currentNumber.Substring(currentNumber.Length - 2)) return getWinning(0.015);
                else if (winnerNumber.Substring(winnerNumber.Length - 1) == currentNumber.Substring(currentNumber.Length - 1)) return getWinning(0.005);
            }
            else
            {
                if (winnerNumber == currentNumber) return getWinning(25);
                else if (winnerNumber.Substring(winnerNumber.Length - 4) == currentNumber.Substring(currentNumber.Length - 4)) return getWinning(0.4);
                else if (winnerNumber.Substring(winnerNumber.Length - 3) == currentNumber.Substring(currentNumber.Length - 3)) return getWinning(0.35);
                else if (winnerNumber.Substring(winnerNumber.Length - 2) == currentNumber.Substring(currentNumber.Length - 2)) return getWinning(0.15);
                else if (winnerNumber.Substring(winnerNumber.Length - 1) == currentNumber.Substring(currentNumber.Length - 1)) return getWinning(0.005);
                
            }

            return 0;
        }
        public double getWinning(double ratio)
        {
            return serverPiyango.total * ratio / 100;
        }

        public class piyangoResult
        {
            public int winnerType { get; set; }
            public int winnerTicket { get; set; }
            public int total { get; set; }
            public List<user> users { get; set; } = new List<user>();
            public class user
            {
                public string userName { get; set; } = "YOK";
                public int totalWin { get; set; } = 0;
                public List<tickets> tickets = new List<tickets>();
            }

            public class tickets
            {
                public int ticket { get; set; }
                public int getPrice { get; set; }
            }
        }

        [Command("piyangobilgicikar")]
        public async Task com_createPiyangoJson(PlayerModel p)
        {
            if (p.adminLevel <= 3) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            piyangoResult result = new piyangoResult();
            result.winnerType = serverPiyango.winnerType;
            result.winnerTicket = serverPiyango.winnerTicket;
            result.total = serverPiyango.total;

            foreach (Piyango x in serverPiyango.serverTickets)
            {
                piyangoResult.user Member = new piyangoResult.user();
                PlayerModelInfo target = await Database.DatabaseMain.getCharacterInfo(x.ID);
                Member.userName = target.characterName.Replace("_", " ");

                x.tam.ForEach(a =>
                {
                    piyangoResult.tickets a1 = new piyangoResult.tickets();
                    a1.ticket = a;
                    a1.getPrice = (int)getWiningPrice(a);
                    Member.totalWin += a1.getPrice;
                    if(a1.getPrice != 0)
                    {
                        Member.tickets.Add(a1);
                    }
                    
                });
                x.yarim.ForEach(b =>
                {
                    piyangoResult.tickets b1 = new piyangoResult.tickets();
                    b1.ticket = b;
                    b1.getPrice = (int)getWiningPrice(b);
                    Member.totalWin += b1.getPrice;
                    if (b1.getPrice != 0) {
                        Member.tickets.Add(b1);
                    }
                    
                });
                x.ceyrek.ForEach(c =>
                {
                    piyangoResult.tickets c1 = new piyangoResult.tickets();
                    c1.ticket = c;
                    c1.getPrice = (int)getWiningPrice(c);
                    Member.totalWin += c1.getPrice;
                    if (c1.getPrice != 0) {
                        Member.tickets.Add(c1);
                    }
                });
                result.users.Add(Member);

            }
            string json = JsonConvert.SerializeObject(result);
            File.WriteAllText(Directory.GetCurrentDirectory() + "\\Piyango\\piyango.json", json);
            MainChat.SendErrorChat(p, "JSON dosyası basıldı!");
            return;
        }
    }
}*/
