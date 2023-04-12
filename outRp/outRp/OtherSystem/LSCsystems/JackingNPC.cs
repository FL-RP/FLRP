using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Enums;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Core;
using outRp.Globals;
using outRp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;

namespace outRp.OtherSystem.LSCsystems
{
    public class JackingNPC : IScript
    {
        public class JackNpc
        {
            public ulong ID { get; set; }
            public string Model { get; set; }
            public Position Position { get; set; }
            public int Heading { get; set; } = 0;
            public int Dimension { get; set; }
            public DateTime LastJack { get; set; }
            public int CurrentJacker { get; set; }
            public string Name { get; set; }
            public int State { get; set; }
        }

        public static List<JackNpc> npcs = new();

        public static void NpcTimer()
        {
            npcs.ForEach(x =>
            {
                var ped = getPet(x.ID);
                if (x.CurrentJacker != 0)
                {
                    var jacker = GlobalEvents.GetPlayerFromSqlID(x.CurrentJacker);
                    if (jacker == null)
                    {
                        x.CurrentJacker = 0;
                        if (ped != null)
                        {
                            ped.animation = new string[] { "a", "b" };
                            ped.nametag = x.Name;
                        }
                    }
                    else
                    {
                        if (jacker.Position.Distance(x.Position) > 20)
                        {
                            if (ped != null)
                            {
                                ped.animation = new string[] { "a", "b" };
                                ped.nametag = x.Name;
                            }
                            x.CurrentJacker = 0;

                        }
                    }
                }
                if (x.LastJack > DateTime.Now)
                {
                    if (ped != null)
                    {
                        ped.animation = new string[] { "a", "b" };
                        ped.nametag = x.Name;
                    }
                    x.CurrentJacker = 0;
                }
            });
        }
        public static void LoadAllNpc(string data)
        {
            npcs = JsonConvert.DeserializeObject<List<JackNpc>>(data);
            npcs.ForEach(x =>
            {
                var ped = PedStreamer.Create(x.Model, x.Position, x.Dimension, 100);
                x.ID = ped.Id;
                ped.nametag = x.Name;
                ped.heading = x.Heading;

            });
            Alt.Log("加载 可抢劫NPC, 数量: " + npcs.Count);
        }


        public static PedModel getPet(ulong id)
        {
            return PedStreamer.Get(id);
        }
        public static async Task<bool> TryJack(PlayerModel p)
        {
            if (Jacking.TotalPDGroup() < 4) { return false; }
            if (p.CurrentWeapon != (uint)WeaponModel.SNSPistol) { return false; }

            var npc = npcs.Find(x => x.Position.Distance(p.Position) < 2 && x.Dimension == p.Dimension);
            if (npc == null) { return false; }
            if (npc.CurrentJacker != 0 && npc.CurrentJacker != p.sqlID) { return false; }

            var target = getPet(npc.ID);
            if (target == null) { return false; }


            if (p.IsAiming)
            {
                if (npc.CurrentJacker == 0)
                {
                    target.nametag = "~r~看着很害怕~n~~p~*举着双手*~n~~w~" + npc.Name;
                    target.animation = new string[] { "missminuteman_1ig_2", "handsup_base" };
                    npc.CurrentJacker = p.sqlID;
                    npc.LastJack = DateTime.Now.AddMinutes(5);
                    MainChat.AME(p, "用枪指着面前的人.");
                    GlobalEvents.SetClothes(p, 5, 83, 0);
                    FreezeToJack(p);
                    string[] namecheck = p.fakeName.Split('_');
                    if (namecheck[0] == "陌生人")
                    {
                        Jacking.AddMDCCall(p, "[标记点抢劫] - 不明身份.");
                    }
                    else
                    {
                        Jacking.AddMDCCall(p, "[标记点抢劫] - 嫌犯: " + p.characterName.Replace('_', ' ') + " (( 请注意, 这是摄像头拍到的大概面部, 不可直接断定 ))");
                    }
                }
            }
            else
            {
                Random rnd = new Random();
                if (p.HasData("Jacking:LastUsed"))
                {
                    if (npc.LastJack > DateTime.Now)
                    {
                        if (p.lscGetdata<DateTime>("Jacking:LastUsed") > DateTime.Now) { return false; }
                        else
                        {
                            GlobalEvents.PlayAnimation(p, new string[] { "anim@heists@ornate_bank@grab_cash_heels", "grab" }, 0, 2500);
                            p.SetData("Jacking:LastUsed", DateTime.Now.AddSeconds(3));
                            p.cash += rnd.Next(150, 250);
                            await p.updateSql();
                            MainChat.AME(p, "一把抢过收银员拿出的现金.");
                        }
                    }
                }
                else
                {
                    if (npc.LastJack > DateTime.Now)
                    {
                        MainChat.AME(p, "一把抢过收银员拿出的现金.");
                        p.SetData("Jacking:LastUsed", DateTime.Now.AddSeconds(3));
                        p.cash += rnd.Next(300, 2000);
                        await p.updateSql();
                    }

                }
            }

            // TODO ihbar.

            return false;
        }

        public static async void FreezeToJack(PlayerModel p)
        {
            GlobalEvents.ProgresBar(p, "你四处寻觅中.", 20);
            GlobalEvents.FreezeEntity(p, true);
            p.SetData("Jacking:LastUsed", DateTime.Now.AddSeconds(20));
            await Task.Delay(20000);
            if (p.Exists)
            {
                GlobalEvents.FreezeEntity(p);
            }
        }

        [Command("addjacknpc")]
        public static void COM_CreateJackNpc(PlayerModel p, params string[] args)
        {
            if (args.Length < 2) { MainChat.SendErrorChat(p, "[用法] /addjacknpc [模型] [姓名(_代表空格)]"); return; }
            if (p.adminLevel < 4) { MainChat.SendErrorChat(p, "[错误] 无权操作"); return; }
            JackNpc npc = new()
            {
                Model = args[0],
                Dimension = p.Dimension,
                CurrentJacker = 0,
                LastJack = DateTime.Now.AddDays(-1),
                Name = args[1],
                Position = p.Position,
                Heading = 0
            };

            var ped = PedStreamer.Create(npc.Model, npc.Position, npc.Dimension, 100);
            npc.ID = ped.Id;
            ped.nametag = npc.Name;

            npcs.Add(npc);

            MainChat.SendInfoChat(p, "[?] 已创建可抢劫NPC.");
            return;
        }

        [Command("deletejacknpc")]
        public static void COM_DeleteJackNPC(PlayerModel p)
        {
            if (p.adminLevel < 4) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            var npc = npcs.Where(x => x.Position.Distance(p.Position) < 4 && x.Dimension == p.Dimension).OrderBy(x => x.Position.Distance(p.Position)).FirstOrDefault();
            if (npc == null) { MainChat.SendErrorChat(p, "[错误] 附近没有可抢劫NPC!"); return; }
            getPet(npc.ID).Destroy();
            npcs.Remove(npc);
            MainChat.SendErrorChat(p, "[?] 已删除附近的NPC.");
            return;
        }

        [Command("editjacknpcheading")]
        public static void COM_EditNPCHeading(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendErrorChat(p, "[用法] /editjacknpcheading [数值]"); return; }
            if (!Int32.TryParse(args[0], out int heding)) { MainChat.SendErrorChat(p, "[用法] /editjacknpcheading [数值]"); return; }

            var npc = npcs.Where(x => x.Position.Distance(p.Position) < 4 && x.Dimension == p.Dimension).OrderBy(x => x.Position.Distance(p.Position)).FirstOrDefault();
            if (npc == null) { MainChat.SendErrorChat(p, "[错误] 附近没有可抢劫NPC!"); return; }
            var ped = getPet(npc.ID);
            ped.heading = heding;
            npc.Heading = heding;
            MainChat.SendErrorChat(p, "[?] NPC头朝向更新!");
        }
    }
}
