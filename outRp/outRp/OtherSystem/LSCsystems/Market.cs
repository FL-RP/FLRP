using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net; using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Resources.Chat.Api;
using outRp.Chat;
using Newtonsoft.Json;
using outRp.Models;
using outRp.Globals;
using outRp.OtherSystem.Textlabels;

namespace outRp.OtherSystem.LSCsystems
{
    public class Market : IScript
    {
        public static void LoadMarketSystem()
        {
            if(serverMarketList.Count > 0 || serverMarketList != null)
            {
                foreach (var m in serverMarketList)
                {
                    m.marketLabel = TextLabelStreamer.Create("~g~[~w~商店~g~]~n~~g~/~w~mb", m.position, font: 0, streamRange: 10);
                }
            }
           
        }
        public class MarketModel
        {
            public int ID { get; set; }
            public int type { get; set; } = 1;  // 1 : server | 2: işyeri.
            public Position position { get; set; }
            public int business { get; set; } = 0;
            public int dimension { get; set; } = 1;
            public PlayerLabel marketLabel { get; set; }
            public List<MarketItems> items { get; set; } = new List<MarketItems>();
            public class MarketItems
            {
                public int itemId { get; set; }
                public int price { get; set; }
                public int stock { get; set; } = 0;
            }
        }

        public class MarketSaveModel
        {
            public int ID { get; set; }
            public int type { get; set; }
            public Position position { get; set; }
            public int business { get; set; }
            public int dimension { get; set; }
            public List<MarketModel.MarketItems> items { get; set; } = new List<MarketModel.MarketItems>();
        }

        public static string GetMarketsSaveString()
        {
            List<MarketSaveModel> save = new List<MarketSaveModel>();
            foreach(var s in serverMarketList)
            {
                MarketSaveModel nS = new MarketSaveModel();
                nS.ID = s.ID;
                nS.type = s.type;
                nS.position = s.position;
                nS.business = s.business;
                nS.items = s.items;
                save.Add(nS);
            }
            return JsonConvert.SerializeObject(save);
        }

        public static List<MarketModel> serverMarketList = new List<MarketModel>();

        [AsyncClientEvent("Market:WantBuy")]
        public async Task BuyItem(PlayerModel p, int Id)
        {
            if (p.Ping > 250)
                return;
            var M = serverMarketList.Find(x => p.Position.Distance(x.position) < 5);
            if (M == null) { MainChat.SendErrorChat(p, "[错误] 附近没有商店."); p.EmitLocked("Market:Canuse"); return; }
            var i = M.items.Find(x => x.itemId == Id);
            if (i.stock <= 0) { MainChat.SendInfoChat(p, "> 商店工作人员表示此商品没有货了."); p.EmitLocked("Market:Canuse"); return; }
            int price = 0;
            ServerItems item = Items.LSCitems.Find(x => x.ID == Id);
            if (item == null) { p.EmitLocked("Market:Canuse"); return; }
            if (i != null) { price = i.price; } else { price = item.price; }

            if (price >= p.cash) { MainChat.SendErrorChat(p, Globals.CONSTANT.ERR_MoneyNotEnought); p.EmitLocked("Market:Canuse"); return; }
            
            // Telefon
            if (item.type == 1)
            {
                List<int> PhoneNumbers = await Database.DatabaseMain.GetPhoneNumberList();
                Random num = new Random();
                int newNumber = num.Next(10000, 99999);
                bool numberfound = PhoneNumbers.Contains(newNumber);
                while (numberfound)
                {
                    newNumber = num.Next(10000, 99999);
                    numberfound = PhoneNumbers.Contains(newNumber);
                    await Task.Delay(100);
                }

                if (!p.Exists)
                    return;

                item.data = newNumber.ToString();
                //Phones.PhoneModel newPhone = new Phones.PhoneModel();
                //newPhone.phoneNumber = newNumber;
                Phone.PhoneMain.CheckAndCreatePhone(newNumber);
                //string phonejson = JsonConvert.SerializeObject(newPhone);
                //item.data2 = phonejson;
                item.name = "iFruit X " + newNumber;
                p.SendChatMessage("新电话号码: " + newNumber.ToString());
                
                if (p.isFinishTut == 30)
                {
                    // MainChat.SendChatMsg("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}手机系统");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}嘿, 您成功购买了一部手机!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}现在, 您可以按 {fc5e03}F3键{FFFFFF} 打开您的手机吧!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}并且, 我们有话对您说, 您已经完成了所有教程!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}您已经完成了入门, 您不再是新手教程玩家了, 您已经脱去了在LS-RP.CC的基本外衣!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}为了补偿您认真完成教程, 我们赠送了您$2000游戏币, 并且请您及时办理一张银行卡, 否则无法领取接下来的工资和补贴!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}最后, 祝您游戏愉快, 并且请遵守游戏规则, 要不然会被处置, 注意, 全时态扮演!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}更多帮助, 请输入 {fc5e03}/help{FFFFFF} 查看指令, 相信我 一定要看!");

                    p.isFinishTut = 100;
                    p.cash += 2000;
                    GlobalEvents.ClearPlayerTag(p);
                    p.updateSql();
                }                  
            }
            else if(item.type == 30)
            {
                item.data = 100.ToString();
            }
            Globals.GlobalEvents.NativeNotify(p, "~r~-$" + item.price);

            if(await Inventory.InvTotalWeight(p) + item.weight > p.Strength) { MainChat.SendErrorChat(p, "[错误] 您的库存超重了."); p.EmitLocked("Market:Canuse"); return; }

            bool isOk = await Inventory.AddInventoryItem(p, item, 1);
            if (!isOk) { MainChat.SendErrorChat(p, "[错误] 您的库存满了."); p.EmitLocked("Market:Canuse"); return; }
            p.cash -= price;
            p.updateSql();
            p.EmitLocked("Market:Canuse");
            if (M.type != 1)
            {
                i.stock -= 1;
            }
        }

        [Command("mb")]
        public static void COM_Market(PlayerModel p)
        {
            var M = serverMarketList.Find(x => p.Position.Distance(x.position) < 5);
            if(M == null) { MainChat.SendErrorChat(p, "[错误] 附近没有商店."); return; }


            List<ServerItems> items = GetMarketItems(M.items);
            if(items.Count <= 0) { MainChat.SendInfoChat(p, "> 这家商店什么都没有."); return; }
            string json = JsonConvert.SerializeObject(items);
            p.EmitLocked("Market:Show", json);
        }

        public static List<ServerItems> GetMarketItems(List<MarketModel.MarketItems> items)
        {
            List<ServerItems> i = new List<ServerItems>();
            foreach (var n in items)
            {
                var item = Items.LSCitems.Find(x => x.ID == n.itemId);
                if (item == null) { continue; }
                ServerItems addItem = new ServerItems();
                addItem = item;
                addItem.price = n.price;
                i.Add(addItem);
            }
            return i;
        }
    }
}
