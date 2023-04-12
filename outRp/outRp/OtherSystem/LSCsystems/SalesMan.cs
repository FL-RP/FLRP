using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Globals;
using outRp.Models;
using outRp.OtherSystem.NativeUi;
using outRp.OtherSystem.Textlabels;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;

namespace outRp.OtherSystem.LSCsystems
{
    public class SalesMan : IScript
    {
        //--------------------------
        //   Tezgahtalık Sistemi
        //--------------------------


        public class SaleModel
        {
            public int OwnerID { get; set; }
            public int Price { get; set; }
            public OtherSystem.Textlabels.LProp SaleProp { get; set; }
            public PlayerLabel Textlabel { get; set; }
            public List<OtherSystem.Textlabels.LProp> objects { get; set; } = new List<OtherSystem.Textlabels.LProp>();
        }

        public static List<SaleModel> standList = new List<SaleModel>();

        public static void createSaleStand(PlayerModel p)
        {
            var stand = standList.Find(x => x.OwnerID == p.sqlID);
            if (stand != null) { MainChat.SendErrorChat(p, "[错误] 您已经摆摊了."); return; }

            GuiMenu chair = new GuiMenu { name = "汉堡摊", triger = "SalesMan:CreateStand", value = "prop_burgerstand_01" };
            GuiMenu table = new GuiMenu { name = "热狗摊", triger = "SalesMan:CreateStand", value = "prop_hotdogstand_01" }; //prop_fruitstand_b_nite
            GuiMenu close = GuiEvents.closeItem;

            List<GuiMenu> gMenu = new List<GuiMenu>();
            gMenu.Add(chair);
            gMenu.Add(table);
            gMenu.Add(close);
            Gui y = new Gui()
            {
                image = "https://cdn4.iconfinder.com/data/icons/food-drinks-vol-2/66/48-512.png",
                guiMenu = gMenu,
                color = "#CEC80D"
            };
            y.Send(p);
        }

        [AsyncClientEvent("SalesMan:CreateStand")]
        public void saleManCreateStand(PlayerModel p, string model)
        {
            GuiEvents.GuiClose(p);
            GlobalEvents.ShowObjectPlacement(p, model, "SalesMan:PlaceStand");
        }

        [AsyncClientEvent("SalesMan:PlaceStand")]
        public void saleManPlaceStand(PlayerModel p, string rot, string pos, string model)
        {
            SaleModel stand = new SaleModel();
            stand.OwnerID = p.sqlID;
            stand.Price = 5;
            Vector3 position = JsonConvert.DeserializeObject<Vector3>(pos);
            position.Z -= 0.2f;
            Vector3 rotation = JsonConvert.DeserializeObject<Vector3>(rot);
            stand.SaleProp = PropStreamer.Create(model, position, rotation, placeObjectOnGroundProperly: true);
            Position labelPos = position;
            labelPos.Z += 1;
            stand.Textlabel = TextLabelStreamer.Create("~r~[~w~摊位~r~]~n~~w~所有者: ~b~" + p.fakeName.Replace("_", " ") + "~n~~w~价格: ~g~$" + stand.Price + "~n~~w~指令: ~g~/stand", labelPos, streamRange: 5);
            MainChat.SendInfoChat(p, "> 已放置摊位.<br>输入 /stand 管理");
            standList.Add(stand);
        }

        [Command("stand")]
        public static void COM_Stand(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0)
            {
                MainChat.SendInfoChat(p, "[用法] /stand [选项]<br>可用选项:<br>des -> 收回摊位.<br>add -> 添加物体.<br>" +
                "price -> 编辑价格.<br>buy -> 购买."); return;
            }
            switch (args[0])
            {
                case "des":
                    destroyStand(p);
                    break;

                case "add":
                    createSubProp(p);
                    break;

                case "price":
                    if (args.Length <= 1) { MainChat.SendInfoChat(p, "> 用法: /stand price 金额 <br>例如: /stand price 5"); return; }
                    if (!Int32.TryParse(args[1], out int newPrice)) { MainChat.SendInfoChat(p, "> 用法: /stand price 金额 <br>例如: /stand price 5"); return; }
                    if (newPrice <= 0 || newPrice > 20) { MainChat.SendInfoChat(p, "> 用法: /stand price 金额 <br>例如: /stand price 5"); return; }
                    //int price = Int32.Parse(args[1]);
                    priceStand(p, newPrice);
                    break;

                case "buy":
                    buyFromStand(p);
                    break;
            }
        }

        public static void destroyStand(PlayerModel p)
        {
            var stand = standList.Find(x => x.OwnerID == p.sqlID);
            if (stand == null) { MainChat.SendErrorChat(p, "[错误] 无效摊位."); return; }
            if (p.Position.Distance(stand.SaleProp.Position) > 5) { MainChat.SendErrorChat(p, "[错误] 您离摊位太远."); return; }

            stand.SaleProp.Destroy();
            stand.Textlabel.Delete();
            foreach (var s in stand.objects)
            {
                s.Delete();
            }
            standList.Remove(stand);
            MainChat.SendInfoChat(p, "> 已收回摊位.");
        }

        public static void priceStand(PlayerModel p, int price)
        {
            var stand = standList.Find(x => x.OwnerID == p.sqlID);
            if (stand == null) { MainChat.SendErrorChat(p, "[错误] 无效摊位."); return; }
            if (p.Position.Distance(stand.SaleProp.Position) > 5) { MainChat.SendErrorChat(p, "[错误] 您离摊位太远."); return; }
            if (price <= 5) { MainChat.SendErrorChat(p, "[错误] 价格最低为 5."); return; }

            stand.Price = price;
            stand.Textlabel.Text = "~r~[~w~摊位~r~]~n~~w~所有者: ~b~" + p.fakeName.Replace("_", " ") + "~n~~w~价格: ~g~$" + price + "~n~~w~指令: ~g~/stand";
        }

        public static async Task buyFromStand(PlayerModel p)
        {
            SaleModel stand = null;
            foreach (var x in standList)
            {
                if (p.Position.Distance(x.SaleProp.Position) < 5)
                {
                    stand = x;
                    break;
                }
            }

            if (p.cash < stand.Price) { MainChat.SendErrorChat(p, CONSTANT.ERR_MoneyNotEnought); return; }
            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(stand.OwnerID);
            if (t == null) { MainChat.SendErrorChat(p, "[错误] 所有者可能不在线, 您无法购买."); return; }
            if (t.Position.Distance(stand.SaleProp.Position) > 5) { MainChat.SendErrorChat(p, "[错误] 您离摊位太远."); return; }
            ServerItems i = Items.LSCitems.Find(x => x.ID == 2);
            i.name = "快餐";
            bool succes = await Inventory.AddInventoryItem(p, i, 1);
            if (succes)
            {
                MainChat.SendInfoChat(p, "[信息] 已购买快餐, 已支付: " + stand.Price + "$");
                p.cash -= stand.Price;
                t.cash += stand.Price - 2;
                await p.updateSql();
                await t.updateSql();
                return;
            }
            else { MainChat.SendErrorChat(p, "[错误] 您的库存满了."); return; }
        }
        public static void createSubProp(PlayerModel p)
        {
            var stand = standList.Find(x => x.OwnerID == p.sqlID);
            if (stand == null) { MainChat.SendErrorChat(p, "[错误] 无效摊位."); return; }
            if (p.Position.Distance(stand.SaleProp.Position) > 15) { MainChat.SendErrorChat(p, "[错误] 您离摊位太远."); return; }

            GuiMenu chair = new GuiMenu { name = "椅子", triger = "SalesMan:AddObject", value = "prop_rub_stool" };
            GuiMenu table = new GuiMenu { name = "桌子", triger = "SalesMan:AddObject", value = "prop_chateau_table_01" }; //prop_patio_lounger1_table
            GuiMenu table2 = new GuiMenu { name = "长桌子", triger = "SalesMan:AddObject", value = "prop_patio_lounger1_table" }; //prop_patio_lounger1_table
            GuiMenu tent = new GuiMenu { name = "水果摊", triger = "SalesMan:AddObject", value = "prop_fruitstand_b_nite" };
            GuiMenu close = GuiEvents.closeItem;

            List<GuiMenu> gMenu = new List<GuiMenu>();
            gMenu.Add(chair);
            gMenu.Add(table);
            gMenu.Add(table2);
            gMenu.Add(tent);
            gMenu.Add(close);
            Gui y = new Gui()
            {
                info = "最大放置: 10",
                image = "https://cdn4.iconfinder.com/data/icons/food-drinks-vol-2/66/48-512.png",
                guiMenu = gMenu,
                color = "#CEC80D"
            };
            y.Send(p);
        }

        [AsyncClientEvent("SalesMan:AddObject")]
        public void createSalesManObject(PlayerModel p, string objectModel)
        {
            //Alt.Log(objectModel);
            //Alt.Log(p.characterName);
            SaleModel stand = standList.Find(x => x.OwnerID == p.sqlID);
            if (stand == null) { MainChat.SendErrorChat(p, "[错误] 无效摊位!"); return; }
            if (stand.objects.Count >= 10) { MainChat.SendErrorChat(p, "[错误] 您已达到摊位最大物体摆放限额了."); return; }
            GuiEvents.GuiClose(p);
            GlobalEvents.ShowObjectPlacement(p, objectModel, "SalesMan:PlaceObject");
        }

        [AsyncClientEvent("SalesMan:PlaceObject")]
        public void saleManPlaceObject(PlayerModel p, string rot, string pos, string oModel)
        {
            var stand = standList.Find(x => x.OwnerID == p.sqlID);
            if (stand == null) { MainChat.SendErrorChat(p, "[错误] 无效摊位!"); return; }
            Position position = JsonConvert.DeserializeObject<Vector3>(pos);
            position.Z -= 0.2f;
            Vector3 rotation = JsonConvert.DeserializeObject<Vector3>(rot);
            if (position.Distance(stand.SaleProp.Position) > 15) { MainChat.SendErrorChat(p, "[错误] 您离摊位太远."); return; }
            OtherSystem.Textlabels.LProp n = PropStreamer.Create(oModel, position, rotation, placeObjectOnGroundProperly: true, frozen: true);
            stand.objects.Add(n);
            return;
        }
    }
}
