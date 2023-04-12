using System;
using System.Collections.Generic;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Resources.Chat.Api;
using outRp.Chat;
using outRp.Models;
using outRp.Globals;
using outRp.OtherSystem.NativeUi;


namespace outRp.OtherSystem.LSCsystems
{
    public class AirSoft  // TODO IScript Eklenecek...
    {

        public class Const
        {
            public static string inMatch = "AirSoft:InMatch";
        }
        public class Match
        {
            public int ID { get; set; }
            public int Owner { get; set; }
            public List<int> Team_A_Players { get; set; }
            public List<int> Team_B_Players { get; set; }
            public int Map { get; set; }
            public List<int> Allowed_Weapons { get; set; }
            public Position Team_A_Start { get; set; }
            public Position Team_B_Start { get; set; }
            public int MatchType { get; set; }
            public List<Capture> CapturePoints { get; set; }
        }

        public class Capture
        {
            public Position Pos { get; set; }
            public bool isCapturing { get; set; }
            public bool isTeamA { get; set; }
            public int CaptureRemaining { get; set; }
        }
        public enum MatchType
        {
            CaptureFlag = 0,
            DeathMatch = 1,
            TeamDeathMatch = 2,
            LastManStanding = 3,
            Conqueror = 4
        }

        public static List<Match> airSoftMatchs = new();

        #region AdminCommands

        #endregion

        #region UserCommands
        [Command("asdavet")]
        public static void COM_AirSoft_Invite(PlayerModel p, params string[] args)
        {
            if (args.Length <= 1) { MainChat.SendInfoChat(p, "[Kullanım] /asdavet [takima, takimb] [id (birden fazla kişi için , koyarak yazın)<br>Örn: /asdavet 1,2,3,4"); return; }
            var match = airSoftMatchs.Find(x => x.Owner == p.sqlID);
            if(match == null) { MainChat.SendErrorChat(p, "[HATA] Kurulmuş bir aitsoft maçı bulunamadı!"); return; }

            if(args[0] == "takima")
            {
                if (args[1].Contains(','))
                {
                    string[] _ids = args[1].Split(',');
                    string added = "";
                    string fail = "";
                    foreach (var i in _ids)
                    {
                        if (!Int32.TryParse(i, out int ID))
                        {
                            fail += "| ID hatalı: " + i;
                            continue;
                        }
                        else
                        {
                            PlayerModel teama_invite = GlobalEvents.GetPlayerFromSqlID(ID);
                            if(teama_invite == null)
                            {
                                fail += "| Oyuncu Yok: " + i;
                                continue;
                            }
                            else
                            {
                                if (teama_invite.HasData(Const.inMatch))
                                {
                                    fail += "| Oyuncu zaten bir maçta: " + teama_invite.characterName.Replace("_", " ");
                                    continue;
                                }

                                Inputs.SendButtonInput(teama_invite, "AirSoft Daveti", "AirSoft:Join", match.ID.ToString() + ",teama");
                                added += " - " + teama_invite.characterName.Replace("_", " ");
                                continue;
                            }
                        }
                    }

                    MainChat.SendInfoChat(p, "<center>AirSoft</center><br>Davet Edilenler: " + added + "<br>Başarısızlar: " + fail);
                    return;
                }
                else
                {
                    if(!Int32.TryParse(args[1], out int ID)) { MainChat.SendInfoChat(p, "[Kullanım] /asdavet [takima, takimb] [id (birden fazla kişi için , koyarak yazın)<br>Örn: /asdavet 1,2,3,4"); return; }
                    PlayerModel teama_invite = GlobalEvents.GetPlayerFromSqlID(ID);
                    if(teama_invite == null) { MainChat.SendErrorChat(p, "[HATA] Oyuncu bulunamadı."); return; }
                    Inputs.SendButtonInput(teama_invite, "AirSoft Daveti", "AirSoft:Join", match.ID.ToString() + ",teama");
                }
            }
            else if(args[0] == "takimb")
            {
                if (args[1].Contains(','))
                {
                    string[] _ids = args[1].Split(',');
                    string added = "";
                    string fail = "";
                    foreach (var i in _ids)
                    {
                        if (!Int32.TryParse(i, out int ID))
                        {
                            fail += "| ID hatalı: " + i;
                            continue;
                        }
                        else
                        {
                            PlayerModel teama_invite = GlobalEvents.GetPlayerFromSqlID(ID);
                            if (teama_invite == null)
                            {
                                fail += "| Oyuncu Yok: " + i;
                                continue;
                            }
                            else
                            {
                                if (teama_invite.HasData(Const.inMatch))
                                {
                                    fail += "| Oyuncu zaten bir maçta: " + teama_invite.characterName.Replace("_", " ");
                                    continue;
                                }

                                Inputs.SendButtonInput(teama_invite, "AirSoft Daveti", "AirSoft:Join", match.ID.ToString() + ",teamb");
                                added += " - " + teama_invite.characterName.Replace("_", " ");
                                continue;
                            }
                        }
                    }

                    MainChat.SendInfoChat(p, "<center>AirSoft</center><br>Davet Edilenler: " + added + "<br>Başarısızlar: " + fail);
                    return;
                }
                else
                {
                    if (!Int32.TryParse(args[1], out int ID)) { MainChat.SendInfoChat(p, "[Kullanım] /asdavet [takima, takimb] [id (birden fazla kişi için , koyarak yazın)<br>Örn: /asdavet 1,2,3,4"); return; }
                    PlayerModel teama_invite = GlobalEvents.GetPlayerFromSqlID(ID);
                    if (teama_invite == null) { MainChat.SendErrorChat(p, "[HATA] Oyuncu bulunamadı."); return; }
                    Inputs.SendButtonInput(teama_invite, "AirSoft Daveti", "AirSoft:Join", match.ID.ToString() + ",teamb");
                }
            }
            else
            {
                MainChat.SendInfoChat(p, "[Kullanım] /asdavet [takima, takimb] [id (birden fazla kişi için , koyarak yazın)<br>Örn: /asdavet 1,2,3,4"); return; 
            }
        }
        #endregion

        #region Functions
        public static LSCUI.UI getMatchesGUI(Match match)
        {
            LSCUI.UI ui = new();
            ui.StartPoint = new int[] { 600, 400 };
            ui.Title = "AirSoft - " + match.Owner;
            ui.SubTitle = "Maç düzenleme menüsü.";

            LSCUI.SubMenu Team_A = new();
            Team_A.StartPoint = new int[] { 600, 400 };
            Team_A.Header = "Oyuncular: Takım A.";
            Team_A.SubTitle = "Takım A oyuncularını gösterir.";

            for (int a = 0; a < match.Team_A_Players.Count - 1; a++)
            {
                PlayerModel player_a = GlobalEvents.GetPlayerFromSqlID(match.Team_A_Players[a]);
                if(player_a == null)
                {
                    match.Team_A_Players.RemoveAt(a);
                    continue;
                }

                LSCUI.Component_Item player_a_item = new LSCUI.Component_Item();
                player_a_item.Header = player_a.characterName.Replace("_", " ");
                player_a_item.Description = "Kişiyi maçtan atmanızı sağlar.";
                player_a_item.Trigger = "AirSoft:KickPlayer";
                player_a_item.TriggerData = player_a.sqlID + ",1," + match.ID;
                Team_A.Items.Add(player_a_item);
            }
            ui.SubMenu.Add(Team_A);


            LSCUI.SubMenu Team_B = new();
            Team_B.StartPoint = new int[] { 600, 400 };
            Team_B.Header = "Oyuncular: Takım B.";
            Team_B.SubTitle = "Takım B oyuncularını gösterir.";
            for (int a = 0; a < match.Team_B_Players.Count - 1; a++)
            {
                PlayerModel player_b = GlobalEvents.GetPlayerFromSqlID(match.Team_B_Players[a]);
                if (player_b == null)
                {
                    match.Team_B_Players.RemoveAt(a);
                    continue;
                }

                LSCUI.Component_Item player_b_item = new LSCUI.Component_Item();
                player_b_item.Header = player_b.characterName.Replace("_", " ");
                player_b_item.Description = "Kişiyi maçtan atmanızı sağlar.";
                player_b_item.Trigger = "AirSoft:KickPlayer";
                player_b_item.TriggerData = player_b.sqlID + ",2," + match.ID;
                Team_B.Items.Add(player_b_item);
            }
            ui.SubMenu.Add(Team_B);

            LSCUI.SubMenu map = new LSCUI.SubMenu();
            map.StartPoint = new int[] { 600, 400 };
            map.Header = "Maç Haritası";
            map.SubTitle = "Maçın yapılacağı haritayı seçin.";

            // Todo MApler eklenecek.

            ui.SubMenu.Add(map);

            LSCUI.SubMenu settings = new LSCUI.SubMenu();
            settings.StartPoint = new int[] { 600, 400 };
            settings.Header = "AirSoft Ayarları";
            settings.SubTitle = "Maç ayarlarını yaparsınız.";

            LSCUI.Component_CheckboxItem s_hasPrivateWeapons = new LSCUI.Component_CheckboxItem();
            s_hasPrivateWeapons.Header = "Kişisel Silahlar";
            s_hasPrivateWeapons.Description = "Kişisel silahlara izin ver";
            s_hasPrivateWeapons.Trigger = "AirSoft:Settings";
            s_hasPrivateWeapons.TriggerData = match.ID + ",privateweapons";
            settings.CheckboxItems.Add(s_hasPrivateWeapons);

            ui.SubMenu.Add(settings);

            return ui;

        }

        public static void SendInfo(Match match, string message, PlayerModel infoPlayer = null)
        {
            if(infoPlayer == null)
            {
            }
        }
        [AsyncClientEvent("AirSoft:Join")]
        public void EVENT_Join_AirSoft(PlayerModel p, bool selection, string vals)
        {
            if (p.HasData(Const.inMatch)) { MainChat.SendErrorChat(p, "[HATA] Zaten bir maça girmişsiniz."); return; }
            string[] val = vals.Split(",");
            if (selection)
            {
                if(!Int32.TryParse(val[0], out int MatchID)) { MainChat.SendErrorChat(p, "[HATA] Maç ID'si ile ilgili bir hata meydana geldi."); return; }
                var match = airSoftMatchs.Find(x => x.ID == MatchID);
                if (match == null) { MainChat.SendErrorChat(p, "[HATA] Maç bulunamadı!"); return; }

                if(val[1] == "teama")
                {
                    match.Team_A_Players.Add(p.sqlID);
                    p.SetData(Const.inMatch, match.ID);

                    // TODO JoinINFO
                }
                else
                {
                    match.Team_B_Players.Add(p.sqlID);
                    p.SetData(Const.inMatch, MatchID);
                }
            }
            else
            {
                MainChat.SendInfoChat(p, "[?] Maç davetini reddettiniz.");
            }
        }
        #endregion
               
    }
}
