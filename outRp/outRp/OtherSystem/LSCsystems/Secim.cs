using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Resources.Chat.Api;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Models;

namespace outRp.OtherSystem.LSCsystems
{
    public class Secim : IScript
    {
        public Secim()
        {
            Alt.Log("加载 选择系统.");
        }

        [Command("oyver*****")]
        public void openPage(PlayerModel player)
        {
            player.EmitLocked("start:secim");
            return;
        }

        [AsyncClientEvent("secim:oy")]
        public async Task secimOy(PlayerModel player, int deger)
        {     

            int secim = await Database.DatabaseMain.GetSecim(player.accountId);
            if (secim == 0)
            {
                await Database.DatabaseMain.SetSecim(player.accountId, deger);
                MainChat.SendInfoChat(player, "Başarıyla " + ((deger == 1) ? "SADP" : "The Alliance Party") + " isimli partiye oy verdiniz.");
                return;
            }
            else
            {
                MainChat.SendErrorChat(player, "[HATA] Zaten oy vermiş görünüyorsunuz.");
                return;
            }
        }

        [Command("secimdurum")]
        public async Task COM_Secimdurum(PlayerModel p)
        {
            var secim = await Database.DatabaseMain.GetSecimAll();

            MainChat.SendInfoChat(p, "Seçim durumu:<br>Democratic Party: " + secim.Item1 + "<br>The Alliance Party: " + secim.Item2);
            return;
        }   
        
        [Command("silahlaritemizle")]
        public async Task COM_ClearAllWeapons( PlayerModel p)
        {
            if (p.adminLevel < 8)
                return;

            int total = 0;
            foreach(VehModel veh in Alt.GetAllVehicles())
            {
                List<ServerItems> items = JsonConvert.DeserializeObject<List<ServerItems>>(veh.vehInv);
                int count = items.Count;
                items = items.Where(x => x.ID != 28 || x.ID != 29 || x.ID != 30 ||x.ID != 35 || x.ID != 40 || x.ID != 41 || x.ID != 42 || x.ID != 43 || x.ID != 44 || x.ID != 45).ToList();
                int newCount = items.Count;
                if(count - newCount > 0)
                {
                    total += count - newCount;
                }
                veh.vehInv = JsonConvert.SerializeObject(items);
            }

            List<HouseModel> houses = await Database.DatabaseMain.GetAllServerHouses(); // LoadHouses
            foreach(var house in houses)
            {
                List<ServerItems> items = JsonConvert.DeserializeObject<List<ServerItems>>(house.houseEnv);
                int count = items.Count;
                items = items.Where(x => x.ID != 28 || x.ID != 29 || x.ID != 30 || x.ID != 35 || x.ID != 40 || x.ID != 41 || x.ID != 42 || x.ID != 43 || x.ID != 44 || x.ID != 45).ToList();
                int newCount = items.Count;
                if (count - newCount > 0)
                {
                    total += count - newCount;
                }
                house.houseEnv = JsonConvert.SerializeObject(items);
                await Database.DatabaseMain.UpdateHouse(house);
            }

            List<BusinessModel> serverBusiness = await Database.DatabaseMain.GetAllServerBusiness();
            foreach(var biz in serverBusiness)
            {
                List<ServerItems> items = JsonConvert.DeserializeObject<List<ServerItems>>(biz.settings.Env);
                int count = items.Count;
                items = items.Where(x => x.ID != 28 || x.ID != 29 || x.ID != 30 || x.ID != 35 || x.ID != 40 || x.ID != 41 || x.ID != 42 || x.ID != 43 || x.ID != 44 || x.ID != 45).ToList();
                int newCount = items.Count;
                if (count - newCount > 0)
                {
                    total += count - newCount;
                }
                biz.settings.Env = JsonConvert.SerializeObject(items);
                await Database.DatabaseMain.UpdateBusiness(biz);
            }

            Alt.Log("Tamamlandı: Toplam: " + total.ToString());
        }

        public static async void COM_ClearAllWeapons()
        {

            int total = 0;
            foreach (VehModel veh in Alt.GetAllVehicles())
            {
                List<ServerItems> items = JsonConvert.DeserializeObject<List<ServerItems>>(veh.vehInv);
                List<ServerItems> newitems = new();
                if(items != null)
                {

                    foreach (var x in items)
                    {
                        if (x.type != 20 && x.type != 21 && x.type != 22 && x.type != 32 && x.type != 27)
                        {
                            newitems.Add(x);
                        }
                    }

                    total += items.Count - newitems.Count;
                    veh.vehInv = JsonConvert.SerializeObject(newitems);
                    veh.Update();
                }
            }

            List<HouseModel> houses = await Database.DatabaseMain.GetAllServerHouses(); // LoadHouses
            Alt.Log("Evlere geçti. Toplam: " + houses.Count);
            foreach (var house in houses)
            {
                List<ServerItems> items = JsonConvert.DeserializeObject<List<ServerItems>>(house.houseEnv);
                List<ServerItems> newitems = new();

                foreach (var x in items)
                {
                    if (x.type != 20 && x.type != 21 && x.type != 22 && x.type != 32 && x.type != 27)
                    {
                        newitems.Add(x);
                    }
                }
                total += items.Count - newitems.Count;
                house.houseEnv = JsonConvert.SerializeObject(newitems);
                await Database.DatabaseMain.UpdateHouse(house);
            }

            List<BusinessModel> serverBusiness = await Database.DatabaseMain.GetAllServerBusiness();
            Alt.Log("İşyerlerine geçti. Toplam:" + serverBusiness.Count);
            foreach (var biz in serverBusiness)
            {
                List<ServerItems> items = JsonConvert.DeserializeObject<List<ServerItems>>(biz.settings.Env);
                List<ServerItems> newitems = new();

                foreach (var x in items)
                {
                    if (x.type != 20 && x.type != 21 && x.type != 22 && x.type != 32 && x.type != 27)
                    {
                        newitems.Add(x);
                    }
                }

                total += items.Count - newitems.Count;
                biz.settings.Env = JsonConvert.SerializeObject(newitems);
                await Database.DatabaseMain.UpdateBusiness(biz);
            }

            Alt.Log("Tamamlandı. Silinen Toplam: " + total.ToString());
        }
    }
}
