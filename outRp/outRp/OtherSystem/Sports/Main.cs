using AltV.Net; using AltV.Net.Async;
using AltV.Net.Data;
using outRp.Models;
using outRp.Chat;
using outRp.Globals;
using outRp.OtherSystem.Textlabels;
using System.Collections.Generic;
using System;
using System.Linq;
using AltV.Net.Resources.Chat.Api;

namespace outRp.OtherSystem.Sports
{
    public class Main : IScript
    {
        public class Court_Basket
        {
            public int ID { get; set; }
            public int Owner { get; set; }
            public string Name { get; set; }
            public ulong textLabelID { get; set; }
            public Position Position { get; set; }            
            public int Dimension { get; set; }
            public Hoop Team_A { get; set; } = new Hoop();
            public Hoop Team_B { get; set; } = new Hoop();

            public Ball ball { get; set; }

            public class Hoop
            {
                public Position Position { get; set; }
                public string Name { get; set; } = "篮筐";
                public int Score { get; set; } = 0;
                public ulong textLabelID { get; set; } = 0;
                public List<int> Clients { get; set; } = new List<int>();
            }

            public class Ball
            {
                public ulong ObjectID { get; set; }
                private Position _Position { get; set; }
                public Position Position
                {
                    get
                    {
                        return _Position;
                    }
                    set
                    {
                        _Position = value;
                        if(Owner == 0)
                        {
                            LProp bal = PropStreamer.GetProp(this.ObjectID);
                            if (bal == null)
                            {
                                bal = PropStreamer.Create("prop_bskball_01", _Position, new Rotation(0, 0, 0), 0, frozen: true);
                                this.ObjectID = bal.Id;
                            }

                            bal.Position = value;
                            return;
                        }
                    }
                }
                private int _Owner { get; set; }
                public int Owner
                {
                    get { return _Owner; }
                    set
                    {
                        if (value == _Owner)
                            return;

                        if (value == 0)
                            return;

                        PlayerModel OldTarget = GlobalEvents.GetPlayerFromSqlID(_Owner);
                        PlayerModel newTarget = GlobalEvents.GetPlayerFromSqlID(value);
                        if (OldTarget != null && newTarget != null)
                        {
                            OldTarget.DeleteStreamSyncedMetaData("BasketBall:Ball:Owner");
                            newTarget.SetStreamSyncedMetaDataAsync("BasketBall:Ball:Owner", true);
                            this.Position = newTarget.Position;
                        }

                        _Owner = value;
                    }
                }
                public bool CanTouch { get; set; } = true;
            }
        }

        public static List<Court_Basket> Baskets = new List<Court_Basket>();
        public static int CourtCounter = 0;
        #region Functions
        public static Court_Basket getNearBasketCourt(PlayerModel p)
        {
            return Baskets.Where(x => x.Position.Distance(p.Position) <= 15 && x.Dimension == p.Dimension).OrderBy(x => x.Position.Distance(p.Position)).FirstOrDefault();
        }
        public static Court_Basket getNearBasketCourt(Position pos, int Dimension = 0)
        {
            return Baskets.Where(x => x.Position.Distance(pos) <= 15 && x.Dimension == Dimension).OrderBy(x => x.Position.Distance(pos)).FirstOrDefault();
        }

        public static void SetHoopText(Court_Basket.Hoop hoop, int Dimension = 0)
        {
            string text = "~b~[~w~篮筐~b~]~n~~w~" + hoop.Name + "~n~分数: ~g~" + hoop.Score;
            if(hoop.textLabelID == 0)
            {
                hoop.textLabelID = TextLabelStreamer.Create(text, hoop.Position, Dimension, true, font: 0, streamRange: 3).Id;
            }
            else
            {
                PlayerLabel lbl = TextLabelStreamer.GetDynamicTextLabel(hoop.textLabelID);
                if(lbl != null)
                {
                    lbl.SetText(text);
                    lbl.Position = hoop.Position;
                    lbl.Range = 3;
                    lbl.Font = 0;
                }
            }
            return;
        }

        
        #endregion
        #region Admin Commands
        [Command("makebasket")]
        public static void COM_AddBasketBallCourt(PlayerModel p)
        {
            if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            var Court = getNearBasketCourt(p.Position);
            if(Court != null) { MainChat.SendErrorChat(p, "[错误] 附近已有篮球场!"); return; }

            Court = new Court_Basket()
            {
                ID = CourtCounter,
                Dimension = p.Dimension,
                Name = "篮球场",
                Owner = 0,
                Position = p.Position,
                textLabelID = TextLabelStreamer.Create("~b~[~w~篮球场~b~]", p.Position, p.Dimension, true, font: 0, streamRange: 15).Id
            };

            Baskets.Add(Court);
            MainChat.SendInfoChat(p, "[?] 已创建篮球场, 使用之前请先创建篮筐.");
            return;
        }

        [Command("editbasket")]
        public static void COM_EditBasketballCourt(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /editbasket [选项] [数值]"); return; }

            var court = getNearBasketCourt(p);
            if (court == null) { MainChat.SendErrorChat(p, "[错误] 附近没有篮球场!"); return; }

            switch (args[0])
            {
                case "pota_a":
                    court.Team_A.Position = p.Position;
                    SetHoopText(court.Team_A, court.Dimension);
                    MainChat.SendInfoChat(p, "[!] 篮筐A已创建.");
                    return;

                case "pota_b":
                    court.Team_B.Position = p.Position;
                    SetHoopText(court.Team_B, court.Dimension);
                    MainChat.SendInfoChat(p, "[!] 篮筐B已创建.");
                    return;

                case "pota_a_name":
                    court.Team_A.Name = string.Join(" ", args[1..]);
                    SetHoopText(court.Team_A, court.Dimension);
                    MainChat.SendInfoChat(p, "[!] 篮筐A名字已设置.");
                    return;

                case "pota_b_name":
                    court.Team_B.Name = string.Join(" ", args[1..]);
                    SetHoopText(court.Team_B, court.Dimension);
                    MainChat.SendInfoChat(p, "[!] 篮筐B名字已设置.");
                    return;

                case "pota_a_score":
                    if(!Int32.TryParse(args[1], out int TeamAScore)) { MainChat.SendErrorChat(p, "[用法] /editbasket pota_a_score [数值]"); return; }
                    court.Team_A.Score = TeamAScore;
                    SetHoopText(court.Team_A, court.Dimension);
                    MainChat.SendInfoChat(p, "[!] 篮筐A分数已设置.");
                    return;

                case "pota_b_skor":
                    if (!Int32.TryParse(args[1], out int TeamBScore)) { MainChat.SendErrorChat(p, "[用法] /editbasket pota_b_score [数值]"); return; }
                    court.Team_B.Score = TeamBScore;
                    SetHoopText(court.Team_B, court.Dimension);
                    MainChat.SendInfoChat(p, "[!] 篮筐B分数已设置.");
                    return;

                case "resetball":
                    court.ball.Position = court.Position;
                    court.ball.Owner = 0;
                    MainChat.SendInfoChat(p, "[!] 已重置篮球和所有人.");
                    return;

                default:
                    return;
            }
        }
        #endregion

        #region Client Listeners
        [AsyncClientEvent("Basketball:PlayerEvent:BallPush")]
        public void EVENT_Ball_Push(PlayerModel p, int CourtID)
        {
            var check = Baskets.Find(x => x.ID == CourtID);
            if (check == null)
                return;

            PlayerModel ballTarget = GlobalEvents.GetPlayerFromSqlID(check.ball.Owner);
            if(ballTarget == null)
            {
                check.ball.Owner = p.sqlID;
                return;
            }
            else
            {
                if(ballTarget.Position.Distance(p.Position) <= 1)
                {
                    Random rnd = new Random();
                    if(rnd.Next(0,7) >= 4)
                    {
                        MainChat.ADO(p, "通过对 " + ballTarget.characterName.Replace("_", " ") + " 的抢断, 夺回了球权.");
                        check.ball.Owner = p.sqlID;
                    }
                    else
                    {
                        MainChat.ADO(p, "试图抢断, 但失败了.");
                        return;
                    }
                }
            }

        }
        
        [AsyncClientEvent("Basketball:PlayerEvent:BallShut")]
        public void EVENT_Ball_Shut(PlayerModel p, int CourtID, float strength)
        {
            var check = Baskets.Find(x => x.ID == CourtID);
            if (check == null)
                return;

            if (check.ball.Owner != p.sqlID)
                return;


        }

        
        #endregion
    }
}
