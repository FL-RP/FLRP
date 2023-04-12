using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Core;
using outRp.Globals;
using outRp.Models;
using System;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;

namespace outRp.OtherSystem.LSCsystems
{
    public class General : IScript
    {
        public static void LoadGenerals()
        {
            VineyardSetup();
        }

        #region VineYard

        public static void VineyardSetup()
        {
            PedModel ch = PedStreamer.Create("ig_chef", VineConst.makeChampPos, 0);
            ch.nametag = "酿酒师 ~y~鲍勃";
        }

        public class VineConst
        {
            public static Position makeChampPos = new Position(181, 2793, 45.6f);
            public static Position vineSellPos = new Position(-799, -1019, 13);

        }

        [Command("gcode")]
        public void TryWine(PlayerModel p)
        {
            if (p.HasData("Wine:Code"))
            {
                GlobalEvents.SubTitle(p, "您已完成了一次葡萄采摘! 请先输入 ~g~/gcollect " + p.lscGetdata<int>("Wine:Code") + " 采集葡萄", 5);
                return;
            }

            CharacterSettings set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            if (set.uzum <= 0)
            {
                MainChat.SendErrorChat(p, "[错误] 您已超过每小时葡萄采摘次数限制, 请在下次发薪日后再来.");
                return;
            }

            --set.uzum;
            p.settings = JsonConvert.SerializeObject(set);
            p.updateSql();

            Random rnd = new Random();
            int key = rnd.Next(1000, 9999);
            GlobalEvents.SubTitle(p, "~w~已获取葡萄采集代码, 输入 : " + key + " | ~g~/gcollect ~w~采集葡萄" + key, 5);
            p.SetData("Wine:Code", key);
        }

        [Command("gcollect")]
        public void GetWineWithCode(PlayerModel p, params string[] args)
        {

            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /gcollect [采集代码]"); return; }
            if (!p.HasData("Wine:Code"))
            {
                MainChat.SendErrorChat(p, "[错误] 您没有获得葡萄采集代码, 输入 {4EC239}/gcode {F54949} 获取葡萄采集代码.");
                return;
            }
            if (!Int32.TryParse(args[0], out int WineCode)) { MainChat.SendInfoChat(p, "[用法] /gcollect [采集代码]"); return; }
            if (p.lscGetdata<int>("Wine:Code") != WineCode) { MainChat.SendErrorChat(p, "[错误] 无效采集代码, 请输入 {4EC239}/gcollect " + p.lscGetdata<int>("Wine:Code")); return; }
            p.EmitAsync("Vine:Start");
            p.DeleteData("Wine:Code");
            return;
        }

        [AsyncClientEvent("Vine:Success")]
        public static async Task GetWine(PlayerModel p)
        {
            if (p.Position.Distance(new Position(-1831, 2215, 86)) > 100) { Core.Logger.WriteLogData(Logger.logTypes.CheatLog, p.characterName + " 疑似采集葡萄作弊 :)"); }
            Random rnd = new Random();
            int amount = rnd.Next(1, 5);
            ServerItems i = new ServerItems { ID = 52, type = 38, name = "葡萄", picture = "52", weight = 1.0, objectModel = "xs_prop_burger_meat_wl", stackable = true, };
            if (!await Inventory.AddInventoryItem(p, i, amount)) { MainChat.SendErrorChat(p, "[错误] 您的库存满了."); }
            return;
        }

        public static async Task<bool> MakeChamp(PlayerModel p, InventoryModel i)
        {
            if (p.Position.Distance(VineConst.makeChampPos) > 3)
                return false;

            if (i.itemId != 52)
                return false;

            if (i.itemAmount < 10) { MainChat.SendErrorChat(p, "[错误] 您没有足够的葡萄! (需要: 10 个葡萄)"); return true; }


            ServerItems n = new ServerItems { ID = 53, type = 39, name = "葡萄酒", picture = "53", weight = 1.0, objectModel = "xs_prop_burger_meat_wl" };
            if (await Inventory.AddInventoryItem(p, n, 1))
            {
                await Inventory.RemoveInventoryItem(p, i.ID, 10);
                MainChat.SendInfoChat(p, "[!] 已使用10个葡萄制作1个<i class='far fa-wine-bottle'></i>葡萄酒!");
                MainChat.SendInfoChat(p, "[!] 您现在可以前往葡萄酒销售点, 打开库存右键您的葡萄酒, 点击'给予'出售葡萄酒!");
            }
            else
            {
                MainChat.SendErrorChat(p, "[错误] 您的库存满了!");
            }

            return true;
        }
        
        public static bool SellWine(PlayerModel p, InventoryModel i)
        {
            if(i.itemId != 53) { return false; }
            if(p.Position.Distance(VineConst.vineSellPos) > 5) { return false; }
            
            p.cash += 128;
            p.updateSql();
            Inventory.RemoveInventoryItem(p, i.ID, 1);
            return true;
        }

        #endregion
    }
}
