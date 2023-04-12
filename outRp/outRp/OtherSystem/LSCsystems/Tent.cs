using System.Collections.Generic;
using System.Numerics;
using AltV.Net; using AltV.Net.Async;
using AltV.Net.Data;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;
using outRp.Chat;
using outRp.Models;
using outRp.Globals;
using outRp.OtherSystem.Textlabels;
using outRp.Core;
using Newtonsoft.Json;
using outRp.OtherSystem.NativeUi;

namespace outRp.OtherSystem.LSCsystems
{
    public class Tent : IScript
    {
       public class TentModel
        {
            public int Owner { get; set; }
            public ulong objectID { get; set; }
            public Position Position { get; set; }
            public Rotation Rotation { get; set; }
            public List<ulong> items { get; set; } = new List<ulong>();
        }

        public static List<TentModel> serverTents = new List<TentModel>();

        public static bool EVET_TentWant(PlayerModel p)
        {
            var check = serverTents.Find(x => x.Owner == p.sqlID);
            if (check != null)
                return false;

            GlobalEvents.ShowObjectPlacement(p, "ba_prop_battle_tent_02", "Tent:Create");
            return true;
        }

        [AsyncClientEvent("Tent:Create")]
        public void EVENT_CrateTent(PlayerModel p, string rot, string pos, string model)
        {
            if(p.Dimension != 0) { MainChat.SendErrorChat(p, "[错误] 只能在室外搭帐篷.");return; }
            Vector3 position = JsonConvert.DeserializeObject<Vector3>(pos);
            position.Z -= 0.2f;
            Vector3 rotation = JsonConvert.DeserializeObject<Vector3>(rot);

            TentModel tent = new TentModel()
            {
                Owner = p.sqlID,
                objectID = PropStreamer.Create("ba_prop_battle_tent_02", position, rotation, frozen: true).Id,
                Position = position,
                Rotation = rotation
            };

            serverTents.Add(tent);
            MainChat.SendInfoChat(p, "[?] 已搭建帐篷.");
            return;
        }


        [Command("tent")]
        public static void COM_TentObject(PlayerModel p)
        {
            var Tent = serverTents.Find(x => x.Owner == p.sqlID);
            if(Tent == null) { MainChat.SendErrorChat(p, "[错误] 无效帐篷, 请先创建帐篷."); return; }
            if(Tent.Position.Distance(p.Position) > 15) { MainChat.SendErrorChat(p, "[错误] 您离帐篷太远."); return; }
            if(Tent.items.Count >= 10) { MainChat.SendErrorChat(p, "[错误] 您摆放露营物体的限额已达最大值."); return; }
            List<GuiMenu> gMenu = new List<GuiMenu>();
            GuiMenu block1 = new GuiMenu { name = "露营床", triger = "Tent:Object", value = "bkr_prop_biker_campbed_01" };
            GuiMenu block2 = new GuiMenu { name = "营火", triger = "Tent:Object", value = "prop_beach_fire" };
            GuiMenu block3 = new GuiMenu { name = "露营椅", triger = "Tent:Object", value = "prop_old_deck_chair" };
            GuiMenu block4 = new GuiMenu { name = "露营椅 2", triger = "Tent:Object", value = "prop_skid_chair_01" };
            GuiMenu block5 = new GuiMenu { name = "露营椅 3", triger = "Tent:Object", value = "prop_skid_chair_02" };
            GuiMenu block6 = new GuiMenu { name = "露营箱子", triger = "Tent:Object", value = "v_ret_fh_coolbox" };

            gMenu.Add(block1); gMenu.Add(block2); gMenu.Add(block3); gMenu.Add(block4); gMenu.Add(block5); gMenu.Add(block6);

            GuiMenu close = GuiEvents.closeItem;
            gMenu.Add(close);
            Gui y = new Gui()
            {
                image = "https://www.upload.ee/image/12254339/1.png",
                guiMenu = gMenu,
                color = "#00AAFF"
            };
            y.Send(p);
        }

        [AsyncClientEvent("Tent:Object")]
        public void EVENT_TentObject(PlayerModel p, string value)
        {
            GlobalEvents.ShowObjectPlacement(p, value, "Tent:Object2");
            return;
        }

        [AsyncClientEvent("Tent:Object2")]
        public void EVENT_TentObject2(PlayerModel p, string rot, string pos, string model)
        {
            var Tent = serverTents.Find(x => x.Owner == p.sqlID);
            if (Tent == null) { MainChat.SendErrorChat(p, "[错误] 无效帐篷, 请先搭建帐篷."); return; }

            Vector3 position = JsonConvert.DeserializeObject<Vector3>(pos);
            position.Z -= 0.2f;
            Vector3 rotation = JsonConvert.DeserializeObject<Vector3>(rot);
            ulong id = PropStreamer.Create(model, position, rotation, 0, frozen: true).Id;

            Tent.items.Add(id);
            MainChat.SendInfoChat(p, "[!] 已摆放露营物件: " + id);

            return;
        }

        [Command("removetentitem")]
        public static void COM_TentRemoveItem(PlayerModel p, params string[] args)
        {
            if(args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /removetentitem [物件ID]"); return; }

            var Tent = serverTents.Find(x => x.Owner == p.sqlID);
            if (Tent == null) { MainChat.SendErrorChat(p, "[错误] 无效帐篷, 请先搭建帐篷."); return; }

            if(!ulong.TryParse(args[0], out ulong id)) { MainChat.SendInfoChat(p, "[用法] /removetentitem [物件ID]"); return; }
            var check = Tent.items.Find(x => x == id);
            if(check == null || check == 0) { MainChat.SendErrorChat(p, "[错误] 无效露营物件."); return; }
            PropStreamer.GetProp(id).Delete();
            Tent.items.Remove(id);
            MainChat.SendInfoChat(p, "[?] 已移除指定露营物件.");
            return;
        }

        [Command("removetent")]
        public static void COM_RemoveTent(PlayerModel p)
        {
            var check = serverTents.Find(x => x.Owner == p.sqlID);
            if(check == null) { MainChat.SendErrorChat(p, "[错误] 无效帐篷, 请先搭建帐篷."); return; }

            foreach(var item in check.items)
            {
                PropStreamer.GetProp(item).Destroy();
            }

            PropStreamer.GetProp(check.objectID).Delete();
            serverTents.Remove(check);
            MainChat.SendInfoChat(p, "[!] 已移除帐篷.");
            return;
        }


        public static bool JoinOrLeaveTent(PlayerModel p)
        {
            if (p.HasData("InTent"))
            {
                p.Position = p.lscGetdata<Position>("InTent");
                p.Dimension = 0;
                p.DeleteData("InTent");
                return true;
            }
            else
            {
                var check = serverTents.Find(x => x.Position.Distance(p.Position) < 5 && p.Dimension == 0);
                if (check == null)
                    return false;



                p.SetData("InTent", p.Position);
                p.Position = new Position(4129.253f, -4622.479f, 5.706909f);
                p.Dimension = (int)check.objectID;
                FreezePlayerJoinTent(p);                
                return true;
            }
        }

        public static async void FreezePlayerJoinTent(PlayerModel p)
        {
            GlobalEvents.FreezeEntity(p, true);
            p.SetData("Interior:Loading", true);

            await Task.Delay(2000);

            GlobalEvents.FreezeEntity(p);
            p.DeleteData("Interior:Loading");
        }

        [Command("aremovetent")]
        public static void COM_ARemoveTent(PlayerModel p)
        {
            if(p.adminLevel < 2) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            var check = serverTents.Find(x => x.Position.Distance(p.Position) < 10);
            if (check == null) { MainChat.SendErrorChat(p, "[错误] 无效帐篷, 请先搭建帐篷."); return; }

            foreach (var item in check.items)
            {
                PropStreamer.GetProp(item).Destroy();
            }

            PropStreamer.GetProp(check.objectID).Delete();
            serverTents.Remove(check);
            MainChat.SendInfoChat(p, "[!] 已移除帐篷.");
            return;

        }

        [Command("showtents")]
        public static void COM_ShowTentItems(PlayerModel p)
        {
            var check = serverTents.Find(x => x.Owner == p.sqlID);
            if (check == null) { MainChat.SendErrorChat(p, "[错误] 无效帐篷, 请先搭建帐篷!"); return; }

            string text = "<center>露营物件列表</center>";
            foreach(var item in check.items)
            {
                LProp i = PropStreamer.GetProp(item);
                if (i == null)
                    continue;

                text += "<br> ID: " + i.Id.ToString() + " | 模型: " + i.Model;
            }

            MainChat.SendInfoChat(p, text, true);
            return;
        }
    }
}
