using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net; using AltV.Net.Async;
using AltV.Net.Data;
using outRp.OtherSystem.Textlabels;
using outRp.Models;
using outRp.Chat;
using outRp.OtherSystem.NativeUi;
using Newtonsoft.Json;
using AltV.Net.Enums;
using AltV.Net.Resources.Chat.Api;

namespace outRp.OtherSystem.LSCsystems
{
    public class PawnShop : IScript
    {
        public static void LoadPawnShopSystem()
        {
            TextLabelStreamer.Create("~r~[~w~典当店~r~]~n~~w~指令: ~g~/buygun", new Position(21.7f, -1107.4022f, 29.785f), font: 0, streamRange:5);
        }


        [Command("buygun")]
        public void OpenPawnShop(PlayerModel p)
        {
            if(p.Position.Distance(new Position(21.7f, -1107.4022f, 29.785f))> 5) { MainChat.SendErrorChat(p, "[错误] 您必须在典当店才可以使用此指令."); return; }
            List<GuiMenu> gMenu = new List<GuiMenu>();

            GuiMenu gun1 = new GuiMenu { name = "匕首", triger = "PawnShop:WantBuy", value = "1" };
            GuiMenu gun2 = new GuiMenu { name = "棒球棒", triger = "PawnShop:WantBuy", value = "2" };
            GuiMenu gun3 = new GuiMenu { name = "战斧", triger = "PawnShop:WantBuy", value = "3" };
            GuiMenu gun4 = new GuiMenu { name = "撬棍", triger = "PawnShop:WantBuy", value = "4" };
            GuiMenu gun5 = new GuiMenu { name = "手电筒", triger = "PawnShop:WantBuy", value = "5" };
            GuiMenu gun6 = new GuiMenu { name = "高尔夫球棍", triger = "PawnShop:WantBuy", value = "6" };
            GuiMenu gun7 = new GuiMenu { name = "锤子", triger = "PawnShop:WantBuy", value = "7" };
            GuiMenu gun8 = new GuiMenu { name = "斧头", triger = "PawnShop:WantBuy", value = "8" };
            GuiMenu gun9 = new GuiMenu { name = "小刀", triger = "PawnShop:WantBuy", value = "9" };
            GuiMenu gun10 = new GuiMenu { name = "弯刀", triger = "PawnShop:WantBuy", value = "10" };
            GuiMenu gun11 = new GuiMenu { name = "扳手", triger = "PawnShop:WantBuy", value = "11" };
            GuiMenu gun12 = new GuiMenu { name = "弹簧刀", triger = "PawnShop:WantBuy", value = "12" };
            // GuiMenu gun13 = new GuiMenu { name = "指虎", triger = "PawnShop:WantBuy", value = "13" };

            GuiMenu close = GuiEvents.closeItem;

            gMenu.Add(gun1);
            gMenu.Add(gun2);
            gMenu.Add(gun3);
            gMenu.Add(gun4);
            gMenu.Add(gun5);
            gMenu.Add(gun6);
            gMenu.Add(gun7);
            gMenu.Add(gun8);
            gMenu.Add(gun9);
            gMenu.Add(gun10);
            gMenu.Add(gun11);
            gMenu.Add(gun12);
            // gMenu.Add(gun13);
            gMenu.Add(close);
            Gui y = new Gui()
            {
                image = "https://www.chadspawnshop.com/images/logo.png",
                guiMenu = gMenu,
                color = "#4AC27D",
                info = "样样 $2000"
            };
            y.Send(p);
        }
        
        [AsyncClientEvent("PawnShop:WantBuy")]
        public async Task PawnShop_WantBuy(PlayerModel p, int wep)
        {
            if(p.cash <= 2000) { MainChat.SendErrorChat(p, "[错误] 您没有足够的钱."); return; }
            uint wepID = 0;
            string weaponName = "";
            switch (wep) { 
                case 1: wepID = (uint)WeaponModel.AntiqueCavalryDagger; weaponName = "匕首"; break;
                case 2: wepID = (uint)WeaponModel.BaseballBat; weaponName = "棒球棒"; break;
                case 3: wepID = (uint)WeaponModel.BattleAxe; weaponName = "战斧"; break;
                case 4: wepID = (uint)WeaponModel.Crowbar; weaponName = "撬棍"; break;
                case 5: wepID = (uint)WeaponModel.Flashlight; weaponName = "手电筒"; break;
                case 6: wepID = (uint)WeaponModel.GolfClub; weaponName = "高尔夫球棍"; break;
                case 7: wepID = (uint)WeaponModel.Hammer; weaponName = "锤子"; break;
                case 8: wepID = (uint)WeaponModel.Hatchet; weaponName = "斧头"; break;
                case 9: wepID = (uint)WeaponModel.Knife; weaponName = "小刀"; break;
                case 10: wepID = (uint)WeaponModel.Machete; weaponName = "弯刀"; break;
                case 11: wepID = (uint)WeaponModel.PipeWrench; weaponName = "扳手"; break;
                case 12: wepID = (uint)WeaponModel.Switchblade; weaponName = "弹簧刀"; break;
                // case 13: wepID = (uint)WeaponModel.BrassKnuckles; weaponName = "指虎"; break;
            }

            WeaponSystem.WeaponModel newWep = new WeaponSystem.WeaponModel();
            newWep.WeaponHash = wepID;
            newWep.bullet = 1;
            newWep.Serial = "PWNSHP";
            newWep.Durability = 100;
            ServerItems addWeapon = Items.LSCitems.Find(x => x.ID == 28);
            addWeapon.data = "无";
            addWeapon.name = weaponName;
            addWeapon.data2 = JsonConvert.SerializeObject(newWep);
            bool succes = await Inventory.AddInventoryItem(p, addWeapon, 1);
            if (succes) { MainChat.SendInfoChat(p, "购买成功."); p.cash -= 2000; p.updateSql(); return; }
            else { MainChat.SendErrorChat(p, "[错误] 您的库存没有足够的空间或已达到携带限制."); return; }
        }

    }
}
