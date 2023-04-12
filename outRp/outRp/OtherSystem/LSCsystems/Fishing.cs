using System;
using AltV.Net; using AltV.Net.Async;
using AltV.Net.Data;
using outRp.Models;
using outRp.Chat;
using outRp.Globals;
using outRp.Core;

namespace outRp.OtherSystem.LSCsystems
{
    class Fishing : IScript
    {
        public class fCons
        {
            public static string inFishing = "inFishing";
            public static Position fishingPos = new Position(-1859, -1242, 8);
            public static Position FishingSellPos = new Position(-1017, -1354, 5);
        }

        public static void LoadFishingSystem()
        {
            PedModel x = PedStreamer.Create("u_m_y_antonb", fCons.FishingSellPos);
            x.nametag = "收购鱼";            
        }

        public static bool SellFish(PlayerModel p, InventoryModel i)
        {
            if(i.itemId != 39) { return false; }
            if(p.Position.Distance(fCons.FishingSellPos) > 5) { return false; }

            //p.cash += ServerGlobalValues.FishingPrice;
            //ServerGlobalValues.FishingPrice -= 1;
            //if(ServerGlobalValues.FishingPrice <= 20) { ServerGlobalValues.FishingPrice = 20; }
            p.cash += 180;
            p.updateSql();
            Inventory.RemoveInventoryItem(p, i.ID, 1);
            return true;
        }

        public static void StartFishing(PlayerModel p, InventoryModel i)
        {           

            if (p.HasData(fCons.inFishing)) { MainChat.SendErrorChat(p, "[错误] 您已经在钓鱼中!"); return; }

            if(p.Position.Distance(fCons.fishingPos) > 10) { MainChat.SendErrorChat(p, "[错误] 您不在钓鱼区!"); return; }

            if(i.itemId != 38) { MainChat.SendErrorChat(p, "[!] 您没有钓鱼竿."); return; }
            
            GlobalEvents.InvForceClose(p);

            /*if(!Int32.TryParse(i.itemData, out int durability)) { MainChat.SendErrorChat(p, "[!] Bir hata meydana geldi."); return; }
            durability -= 2;

            if(durability <= 0)
            {
                Inventory.RemoveInventoryItem(p, i.ID, 1);
            }
            else
            {
                i.itemData = durability.ToString();
                i.Update();
            }*/
            p.SetData(fCons.inFishing, true);
            p.EmitAsync("Fishing:OpenPushPage");
            return;

        }


        [AsyncClientEvent("Fishing:Succes")]
        public void EVENT_FishingSucces(PlayerModel p)
        {
            if(p.Position.Distance(fCons.fishingPos) > 10) { Core.Logger.WriteLogData(Logger.logTypes.CheatLog, p.characterName + " 钓鱼疑似作弊 :)"); }
            if (p.HasData(fCons.inFishing)) { p.DeleteData(fCons.inFishing); }

            ServerItems i = Items.LSCitems.Find(x => x.ID == 39);
            if(i == null) { /*Alt.Log("Balıkçılık sistemiyle ilgili bir hata meydana geldi. Satır No: 60");*/ return; }

            Inventory.AddInventoryItem(p, i, 1);
            if (p.HasData("AC:LastFish"))
            {
                var lastDate = p.lscGetdata<DateTime>("AC:LastFish");
                if (lastDate >= DateTime.Now.AddMilliseconds(1500))
                {
                    antiCheat.ACBAN(p, 3, "Fishing Dump");
                    return;
                }
            }
            p.SetData("AC:LastFish", DateTime.Now.AddMilliseconds(2000));
            return;
        }

        [AsyncClientEvent("Fishing:Closed")]
        public void EVENT_FishingClosed(PlayerModel p)
        {
            if (p.HasData(fCons.inFishing)) { p.DeleteData(fCons.inFishing); }
            return;
        }


        

    }
}
