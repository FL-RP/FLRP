using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Enums;
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
    public class WoodCutterJob : IScript
    {

        [Command("wcode")]
        public void CutTree(PlayerModel p)
        {
            if (p.CurrentWeapon != (uint)WeaponModel.Hatchet) { MainChat.SendErrorChat(p, "[错误] 您手上没有斧头."); return; }
            if (p.HasData("WoodCutting")) { MainChat.SendErrorChat(p, "[错误] 您正在砍树中!"); return; }
            Random rnd = new Random();
            var rand = rnd.Next(10000, 99999);
            p.SetData("WoodCuttingRand", rand);
            MainChat.SendInfoChat(p, "[伐木] 您已获得随机代码, 现在请输入 /wcut " + rand + " 开始砍树.");
        }

        [Command("wcut")]
        public void StartCutt(PlayerModel p, params string[] args) 
        {
            if(args.Length <= 0) { MainChat.SendErrorChat(p, "[错误] 请输入 /wcut <随机代码>."); return; }
            try {
                if(!p.HasData("WoodCuttingRand")) { MainChat.SendErrorChat(p, "[错误] 您没有获取随机代码, 请先输入 /wcode 获取."); return; }
                var rand = p.lscGetdata<int>("WoodCuttingRand");
                if(args[0] != rand.ToString()) { MainChat.SendErrorChat(p, "[错误] 随机代码错误."); return; }
                else {
                    p.DeleteData("WoodCuttingRand");
                    p.EmitLocked("WoodCutter:GetTree");
                }
            } catch { MainChat.SendErrorChat(p, "[错误] 随机代码错误."); return; }
        }
        

        [AsyncClientEvent("WoodCutter:GetTreeResult")]
        public async void GetTreeResult(PlayerModel p)
        {
            //if (result == false) { MainChat.SendErrorChat(p, "[HATA] Bu komutu kullanabilmek için kesilebilecek bir ağaca yakın olmalısınız."); return; }

            CharacterSettings set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            if (set.odun <= 0)
            {
                MainChat.SendInfoChat(p, "[错误] 您已达到每小时伐木限制, 请在发薪日后再来.");
                return;
            }

            --set.odun;
            p.settings = JsonConvert.SerializeObject(set);
            p.updateSql();

            Position fPos = p.Position;
            fPos.Z -= 1f;

            p.SetData("WoodCutting", true);
            GlobalEvents.ProgresBar(p, "正在砍树中...", 10);
            GlobalEvents.PlaySound(p, "KJcEoYrrF4c");
            await Task.Delay(10000);
            p.DeleteData("WoodCutting");

            if (!p.Exists)
                return;

            GlobalEvents.NativeNotifyAll(p, "~y~砍树中.");
            GlobalEvents.SubTitle(p, "~y~伐木工: ~w~您可以找木头收购商进行出售.", 5);

            ServerItems nItem = Items.LSCitems.Find(x => x.ID == 19);


            ServerItems addItem = nItem;
            addItem.data = "0";
            addItem.data2 = "0";


            await Inventory.AddInventoryItem(p, addItem, 1);

            //LProp case3Prop = PropStreamer.Create("prop_fncwood_13c", fPos, p.Rotation, placeObjectOnGroundProperly: true, frozen: true);
            //string labelText = "[ESYA ID:" + case3Prop.Id + "]~n~ Odun parçası ~n~Almak icin ~r~/~w~yerdenal [ID]";

            //InventoryModel i = new InventoryModel()
            //{
            //    itemData = "",
            //    itemData2 = "",
            //    itemId = 19,
            //    itemName = "Odun parçası",
            //    itemPicture = "19"
            //};

            //Inventory.GroundObj dropedItem = new Inventory.GroundObj()
            //{
            //    ID = (int)case3Prop.Id,
            //    Prop = case3Prop,
            //    textLabel = TextLabelStreamer.Create(labelText, fPos),
            //    data = JsonConvert.SerializeObject(i)
            //};
            //Inventory.groundObjects.Add(dropedItem);
        }

    }

}
