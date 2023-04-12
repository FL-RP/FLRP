using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using outRp.Chat;
using outRp.Globals;
using outRp.Models;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;

namespace outRp.OtherSystem
{
    public class ClothingShop : IScript
    {
        public class ClothesModel
        {
            public int ID { get; set; }
            public int price { get; set; }
        }
        public static void LoadClothingShop()
        {
            OtherSystem.Textlabels.TextLabelStreamer.Create("~b~[~w~购买服装~b~]~n~~w~指令: ~g~/buyclothes", clothingShop, streamRange: 5, dimension: 0);

            PedModel x = PedStreamer.Create("a_f_m_eastsa_01", new Position(451, -774, 27));
            x.nametag = "二手服装店";

        }

        public static bool SellClothes(PlayerModel p, InventoryModel i)
        {
            if (p.Position.Distance(new Position(451, -774, 27)) < 5)
            {
                // if (i.itemId == 4 || i.itemId == 6 || i.itemId == 7 || i.itemId == 8 || i.itemId == 9 || i.itemId == 10 || i.itemId == 11 || i.itemId == 12 || i.itemId == 13 || i.itemId == 14)
                // {
                //     Random rng = new Random();
                //     p.cash += 75;
                //     p.updateSql();
                //     return true;
                // }
                if(
                    i.itemId == 4 || i.itemId == 6 || i.itemId == 7 ||
                    i.itemId == 8 || i.itemId == 9 || i.itemId == 10  || i.itemId == 11 || i.itemId == 12 || i.itemId == 13 || i.itemId == 14
                ) {
                    p.cash += 250;
                    p.updateSql();
                    Core.Logger.WriteLogData(Core.Logger.logTypes.clothesSell, p.characterName + " 出售了服装 " + i.itemId + " ($250)");
                    return true;
                }
                
            }

            //return false;
            return false;
        }


        [AsyncClientEvent("ClothingShop:WantToBuy")]
        public async Task ClothingShopBuy(PlayerModel p, bool isAcc, int type, int id, int texture, int price)
        {
            if (p.Ping > 250)
                return;
            string name = "";
            ServerItems i = null;
            if (isAcc)
            {
                switch (type)
                {
                    case 7:
                        name = "手链";
                        i = Items.LSCitems.Find(x => x.ID == 10);
                        price = 250;
                        break;
                    case 1:
                        name = "眼镜";
                        i = Items.LSCitems.Find(x => x.ID == 4);
                        price = 250;
                        break;
                    case 0:
                        name = "帽子";
                        i = Items.LSCitems.Find(x => x.ID == 6);
                        price = 250;
                        break;
                    case 6:
                        name = "表";
                        i = Items.LSCitems.Find(x => x.ID == 12);
                        price = 250;
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case 7:
                        name = "配饰";
                        i = Items.LSCitems.Find(x => x.ID == 7);
                        price = 250;
                        break;
                    case 4:
                        name = "裤子";
                        i = Items.LSCitems.Find(x => x.ID == 11);
                        price = 250;
                        break;
                    case 6:
                        name = "鞋子";
                        i = Items.LSCitems.Find(x => x.ID == 14);
                        price = 250;
                        break;
                    case 11:
                        name = "上衣";
                        i = Items.LSCitems.Find(x => x.ID == 8);
                        price = 250;
                        break;
                    case 8:
                        name = "内衬";
                        i = Items.LSCitems.Find(x => x.ID == 9);
                        price = 250;
                        break;
                    case 1:
                        name = "面具";
                        i = Items.LSCitems.Find(x => x.ID == 5);
                        price = 5000;
                        break;
                }
            }
            if (p.cash <= price) { MainChat.SendErrorChat(p, CONSTANT.ERR_MoneyNotEnought); return; }
            //if (p.HasData("ClothingVendor:PointID"))
            //{
            //    var point = Company.systems.sellingPoints.GetNearSellPoint(p.Position, p.Dimension, 6);
            //    if(point == null) { MainChat.SendErrorChat(p, "[HATA] Bir hata meydana geldi."); return; }
            //    if(point.stock < 5) { MainChat.SendErrorChat(p, "[HATA] Satıcının bu ürün için yeterli stoğu yok!"); return; }
            //    var company = Company.Database.BusinessDatabase.GetCompany(point.Owner_Company);
            //    if (company == null) { MainChat.SendErrorChat(p, "[HATA] Satıcının bağlı bulunduğu şirket ile ilgili bir hata meydana geldi!"); return; }
            //    company.BusinessPrice += 250;
            //    company.Cash += 150;
            //    point.stock -= 10;
            //    point.Update();
            //    company.Update();
            //    p.DeleteData("ClothingVendor:PointID");
            //}
            i.data = id.ToString();
            i.data2 = texture.ToString();
            i.name = name + " " + id.ToString();
            bool succes = await Inventory.AddInventoryItem(p, i, 1);
            p.EmitLocked("Clothes:Can:Use");
            if (succes)
            {
                MainChat.SendInfoChat(p, "[信息] 成功购买服装, 价格: $" + price); 
                GlobalEvents.notify(p, 2, "成功购买服装"); p.cash -= price; 
                await p.updateSql(); 
                
                if (p.isFinishTut == 23)
                {
                    p.isFinishTut = 24;
                    p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}服装店");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}嘿, 您成功学会了购买服装!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}服务器是有套装保存系统的, 您可以输入 {fc5e03}/saveoutfit 套装名称{FFFFFF} 保存您当前的服装搭配, 以便下次更换!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}记得, 需要在{fc5e03}服装店入口{fc5e03}噢!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}现在, 试试{fc5e03}/saveoutfit 套装名称{FFFFFF} 吧!");
                }                 
                return;
            }
            else { MainChat.SendErrorChat(p, "[错误] 您的库存满了."); return; }
        }
        [AsyncClientEvent("ClothingShop:Close")]
        public async Task ClothingShopClose(PlayerModel p)
        {
            await Inventory.UpdatePlayerInventory(p);
        }

        public static Position clothingShop = new Position(-1186.5494f, -770.7956f, 17.316528f);

    }
}
