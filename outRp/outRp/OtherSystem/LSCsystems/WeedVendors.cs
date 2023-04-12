using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Data;
using outRp.Models;
using outRp.Chat;
using outRp.Globals;
using Newtonsoft.Json;
using System.Linq;
using AltV.Net.Resources.Chat.Api;

namespace outRp.OtherSystem.LSCsystems
{
    public class WeedVendors : IScript
    {
        public class Vendor
        {
            public ulong ID { get; set; }
            public string Name { get; set; }
            public int buyPrice { get; set; }
            public int buyType { get; set; } = 0;
            public int heading { get; set; } = 0;
            public string[] animation { get; set; } = new string[] {"a", "a" };
            public Position pos { get; set; }
            public int dimension { get; set; }
            public string Model { get; set; }
            public int OwnerCompany { get; set; } = 0;
        }

        public static List<Vendor> weedVendors = new List<Vendor>();

        public static void LoadVendors(string data)
        {
            weedVendors = JsonConvert.DeserializeObject<List<Vendor>>(data);

            foreach(Vendor x in weedVendors)
            {
                PedModel ped = PedStreamer.Create(x.Model, x.pos, x.dimension);
                x.ID = ped.Id;
                ped.heading = x.heading;
                ped.animation = x.animation;
                ped.nametag = x.Name;
            }
            return;
        }

        [Command("addvendor")]
        public void CreateVendor(PlayerModel p, params string[] args)
        {
            if(p.adminLevel <= 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if(args.Length < 1) { MainChat.SendInfoChat(p, "[Kullanım] /ualiciekle [Ped Model] [isim]"); return; }

            PedModel x = PedStreamer.Create(args[0], p.Position, p.Dimension);

            Vendor n = new Vendor()
            {
                ID =x.Id,
                Name = string.Join(" ", args[1..]),
                buyPrice = 100,
                pos = p.Position,
                dimension = p.Dimension,
                Model = args[0]
            };
            x.nametag = n.Name;
            weedVendors.Add(n);
            MainChat.SendInfoChat(p, "[!] Alıcı başarıyla oluşturuldu.");
            return;
        }

        [Command("editvendor")]
        public void EditVendor(PlayerModel p, params string[] args)
        {
            if(p.adminLevel <= 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if(args.Length < 1) { MainChat.SendInfoChat(p, "[用法] /editvendor [name-buyprice-buytype] [数值]"); return; }

            Vendor edit = weedVendors.Find(x => p.Position.Distance(x.pos) < 5);
            if(edit == null) { MainChat.SendErrorChat(p, "[错误] 附近没有NPC!"); return; }
            PedModel ped = PedStreamer.Get(edit.ID);
            if(ped == null) { MainChat.SendErrorChat(p, "[HATA] Yakınınızda NPC(Alıcı) bulunamadı!"); return; }


            switch (args[0])
            {
                case "isim":
                    edit.Name = string.Join(" ", args[1..]);
                    ped.nametag = edit.Name;
                    MainChat.SendInfoChat(p, "[PED] 成功更新名称.");
                    return;

                case "buyprice":
                    if (!Int32.TryParse(args[1], out int newPrice)) { MainChat.SendInfoChat(p, "[Kullanım] /ualiciduzenle [isim-buyprice-buytype] [değer]"); return; }
                    edit.buyPrice = newPrice;
                    MainChat.SendInfoChat(p, "[PED] Satın alma fiyatı başarıyla güncellendi.");
                    return;

                case "buytype":
                    if (!Int32.TryParse(args[1], out int newPrice2)) { MainChat.SendInfoChat(p, "[Kullanım] /ualiciduzenle [isim-buyprice-buytype] [değer]"); return; }
                    edit.buyType = newPrice2;
                    MainChat.SendInfoChat(p, "[PED] Satın alma çeşidi başarıyla güncellendi.");
                    return;

                case "heading":
                    if (!Int32.TryParse(args[1], out int newPrice3)) { MainChat.SendInfoChat(p, "[Kullanım] /ualiciduzenle [isim-buyprice-buytype] [değer]"); return; }
                    edit.heading = newPrice3;
                    ped.heading = newPrice3;                    
                    MainChat.SendInfoChat(p, "[PED] Bakış açısı başarıyla güncellendi.");
                    return;

                case "animation":
                    if (args.Length <= 1)
                        return; 

                    edit.animation = new string[] { args[1], args[2] };
                    ped.animation = new string[] { args[1], args[2] };
                    MainChat.SendInfoChat(p, "[PED] Bakış açısı başarıyla güncellendi.");
                    return;
            }
        }

        [Command("ualicikaldir")]
        public void DeleteVendor(PlayerModel p)
        {
            if(p.adminLevel < 4) { MainChat.SendErrorChat(p, "[HATA] Bu komutu kullanmak için yetkiniz yok!"); return; }
            Vendor del = weedVendors.Find(x => p.Position.Distance(x.pos) < 5);
            if (del == null) { MainChat.SendErrorChat(p, "[HATA] Yakınınızda NPC(Alıcı) bulunamadı!"); return; }
            PedModel ped = PedStreamer.Get(del.ID);
            if (ped == null) { MainChat.SendErrorChat(p, "[HATA] Yakınınızda NPC(Alıcı) bulunamadı!"); return; }

            ped.Destroy();
            weedVendors.Remove(del);

            MainChat.SendErrorChat(p, "[PED] Alıcı başarıyla kaldırıldı.");
            return;
        }


        public static async Task<bool> SellWeed(PlayerModel p, InventoryModel i)
        {
            //Vendor target = weedVendors.Find(x => p.Position.Distance(x.pos) < 3);
            Vendor target = weedVendors.Where(x => x.pos.Distance(p.Position) < 4).OrderBy(x => x.pos.Distance(p.Position)).FirstOrDefault();
            if (target == null)
                return false;

            if (target.buyType != i.itemId)
                return false;

            if(target.OwnerCompany != 0)
            {
                var company = await Company.Database.BusinessDatabase.GetCompany(target.OwnerCompany);
                if(company == null) { MainChat.SendErrorChat(p, "[HATA] Şirket bilgileri getirilirken bir hata meydana geldi."); return true; }
                if(company.Cash < target.buyPrice) { MainChat.SendErrorChat(p, "[HATA] Şirketin bu ürünü karşılayabilecek kadar parası yok!"); return true; }
                company.Cash -= target.buyPrice;
                company.BusinessPrice += (target.buyPrice / 2);
                company.Update();

            }
            p.cash += target.buyPrice;
            p.updateSql();
            Inventory.RemoveInventoryItem(p, i.ID, 1);
            return true;
        }
    }
}
