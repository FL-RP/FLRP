using AltV.Net;
using AltV.Net.Async;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Globals;
using outRp.Models;
using outRp.OtherSystem.NativeUi;
using System.Collections.Generic;
using System.Linq;
using AltV.Net.Data;
using AltV.Net.Resources.Chat.Api;
using outRp.Tutorial;

namespace outRp.OtherSystem.LSCsystems
{
    public class ClothesCombine : IScript
    {
        [Command("saveoutfit")]
        public static void COM_SaveCombine(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /saveoutfit [套装名称]"); return; }
            if (p.Position.Distance(OtherSystem.ClothingShop.clothingShop) < 13) { MainChat.SendErrorChat(p, "[错误] 您离服装店太远!"); return; }
            var set = GetSettings(p);
            if (set == null) { MainChat.SendErrorChat(p, "[错误] 发生了错误."); return; }

            if (set.Combine.Count >= 25)
            {
                MainChat.SendErrorChat(p, "[错误] 您最多可以保存 25 套服装!"); return;
            }
            p.EmitLocked("Player:CheckClothes", string.Join(" ", args));

            /* Models.ClothesCombine combine = new Models.ClothesCombine();
             combine.name = string.Join(" ", args);
             combine.cloth = new List<Models.ClothesCombine.Clothes>();
             combine.prop = new List<Models.ClothesCombine.Props>();

             var check = set.Combine.Find(x => x.name == combine.name);
             if (check != null) { MainChat.SendErrorChat(p, "[HATA] Bu isimde bir kombin kayıtlı!"); return; }



             for (int a = 0; a < 12; a++)
             {
                 Cloth x = p.GetClothes((byte)a);
                 if(x.Drawable > 0)
                 {
                     Models.ClothesCombine.Clothes c = new Models.ClothesCombine.Clothes();
                     c.ID = a;
                     c.comp = x.Drawable;
                     c.texture = x.Texture;
                 }
             }

             for(int a = 0; a < 7; a++)
             {
                 Prop x = p.GetProps((byte)a);
                 if(x.Drawable > 0)
                 {
                     Models.ClothesCombine.Props c = new Models.ClothesCombine.Props();
                     c.ID = a;
                     c.prop = x.Drawable;
                     c.texture = x.Texture;
                 }
             }

             set.Combine.Add(combine);
             SaveSettings(p, set);
             MainChat.SendInfoChat(p, "[?] Kombin başarıyla kaydedildi.");*/

            return;
        }

        [AsyncClientEvent("Player:SaveClothes")]
        public void EVENT_AddClothes(PlayerModel p, string name, string comp, string props)
        {
            if (p.Position.Distance(OtherSystem.ClothingShop.clothingShop) < 13) { MainChat.SendErrorChat(p, "[错误] 您离服装店太远!"); return; }
            if (Company.systems.sellingPoints.companySellPoints.Where(x => x.Position.Distance(p.Position) < 6 && x.Type == 6).FirstOrDefault() != null) { MainChat.SendErrorChat(p, "[错误] 请不要在服装店-购买点服装使用此指令!"); return; }
            var set = GetSettings(p);
            if (set == null) { MainChat.SendErrorChat(p, "[错误] 发生了错误."); return; }

            if (set.Combine.Count >= 25)
            {
                MainChat.SendErrorChat(p, "[错误] 您最多可以保存 25 套服装!"); return;
            }
            var check = set.Combine.Find(x => x.name == name);
            if (check != null) { MainChat.SendErrorChat(p, "[错误] 已有相同名称的套装!"); return; }

            Models.ClothesCombine combine = new Models.ClothesCombine();
            combine.name = name;
            combine.cloth = JsonConvert.DeserializeObject<List<Models.ClothesCombine.Clothes>>(comp);
            combine.prop = JsonConvert.DeserializeObject<List<Models.ClothesCombine.Props>>(props);

            set.Combine.Add(combine);
            SaveSettings(p, set);
            MainChat.SendInfoChat(p, "[?] 已保存角色套装.");
            
            if (p.isFinishTut == 24)
            {
                p.isFinishTut = 25;
                p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}服装店");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}嘿, 您成功学会了保存套装!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}现在, 您可以输入 {fc5e03}/outfit{FFFFFF} 浏览和换上您保存的套装!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}现在, 试试{fc5e03}/outfit{FFFFFF} 吧!");
            }                   
            return;
        }

        [Command("outfit")]
        public static void COM_ShowCombine(PlayerModel p)
        {
            LSCUI.Close(p);
            var set = GetSettings(p);
            if (set == null) { MainChat.SendErrorChat(p, "[错误] 发生了错误."); return; }

            LSCUI.UI ui = new LSCUI.UI();
            ui.Title = "";
            ui.SubTitle = "此菜单展示您的套装列表";
            ui.StartPoint = new int[] { 700, 400 };
            ui.Banner = new string[] { "shopui_title_highendsalon", "shopui_title_highendsalon" };

            if (set.Combine.Count <= 0)
            {
                LSCUI.Component_Item item = new LSCUI.Component_Item();
                item.Header = "~r~无套装!";
            }
            else
            {
                LSCUI.Component_Item item = new LSCUI.Component_Item();
                item.Header = "关闭";
                item.Trigger = "Combine:CloseFreeze";
                ui.Items.Add(item);

                foreach (var i in set.Combine)
                {
                    LSCUI.SubMenu sub = new LSCUI.SubMenu();
                    sub.Header = i.name;
                    sub.StartPoint = new int[] { 700, 400 };
                    sub.Banner = new string[] { "shopui_title_highendfashion", "shopui_title_highendfashion" };

                    LSCUI.Component_Item equip = new LSCUI.Component_Item();
                    equip.Header = "~g~选择";
                    equip.Trigger = "Combine:Take";
                    equip.TriggerData = i.name;
                    sub.Items.Add(equip);
                    p.SetData("Combine:Equip", true);

                    LSCUI.Component_Item rename = new LSCUI.Component_Item();
                    rename.Header = "~o~更换套装名称";
                    rename.Trigger = "Combine:Rename";
                    rename.TriggerData = i.name;
                    sub.Items.Add(rename);

                    LSCUI.Component_Item remove = new LSCUI.Component_Item();
                    remove.Header = "~r~移除";
                    remove.Trigger = "Combine:Remove";
                    remove.TriggerData = i.name;
                    sub.Items.Add(remove);
                    p.DeleteData("Combine:Equip");

                    ui.SubMenu.Add(sub);
                }

                ui.Send(p);
                
                if (p.isFinishTut == 25)
                {
                    p.isFinishTut = 26;
                    p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}服装店");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}嘿, 您成功学会了打开套装菜单!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}现在, 请您前往下一个教程点吧!");
                    GlobalEvents.CheckpointCreate(p, TutorialMain.TutPos[7], 1, 2, new Rgba(255, 0, 0, 0), "Tutorial:Run", "26");
                }                       
            }
        }

        [AsyncClientEvent("Combine:CloseFreeze")]
        public void EVENT_CloseCombineFreeze(PlayerModel p)
        {
            GlobalEvents.FreezePlayerClothes(p, true);
        }
        [AsyncClientEvent("Combine:Take")]
        public void EVENT_TakeCombine(PlayerModel p, string name)
        {
            var set = GetSettings(p);
            if (set == null) { MainChat.SendErrorChat(p, "[错误] 发生了错误."); return; }

            var check = set.Combine.Find(x => x.name == name);
            if (check == null) { MainChat.SendErrorChat(p, "[错误] 发生了错误!"); return; }

            // -----
            /*
            foreach (var cl in check.cloth)
            {
                 p.SetClothes((byte)cl.ID, (ushort)cl.comp, (byte)cl.texture, 2);
            }
            foreach (var cp in check.prop)
            {
                p.SetProps((byte)cp.ID, (ushort)cp.prop, (byte)cp.texture);
            }*/

            foreach (var cl in check.cloth)
            {
                GlobalEvents.SetClothes(p, cl.comp, cl.ID, cl.texture);
                //p.SetClothes((byte)cl.comp, (ushort)cl.ID, (byte)cl.texture, 2);
            }
            foreach (var cp in check.prop)
            {
                p.EmitAsync("SetClothesProps", cp.prop, cp.ID, cp.texture);
            }

            GlobalEvents.FreezePlayerClothes(p, false);
            LSCUI.Close(p);
            return;
        }

        [AsyncClientEvent("Combine:Rename")]
        public void EVENT_ReName(PlayerModel p, string name)
        {
            var set = GetSettings(p);
            if (set == null) { MainChat.SendErrorChat(p, "[错误] 发生了错误."); return; }

            var check = set.Combine.Find(x => x.name == name);
            if (check == null) { MainChat.SendErrorChat(p, "[错误] 发生了错误!"); return; }

            Inputs.SendTypeInput(p, "设置新名称", "Combine:Rename2", name);
            LSCUI.Close(p);
            return;

        }

        [AsyncClientEvent("Combine:Rename2")]
        public void EVENT_ReName_2(PlayerModel p, string newName, string Name)
        {
            var set = GetSettings(p);
            if (set == null) { MainChat.SendErrorChat(p, "[错误] 发生了错误."); return; }

            var check = set.Combine.Find(x => x.name == Name);
            if (check == null) { MainChat.SendErrorChat(p, "[错误] 发生了错误!"); return; }

            LSCUI.Close(p);
            check.name = newName;
            SaveSettings(p, set);
            COM_ShowCombine(p);
            MainChat.SendInfoChat(p, "[?] 套装名称更新为 " + newName);
            return;
        }

        [AsyncClientEvent("Combine:Remove")]
        public void EVENT_Remove(PlayerModel p, string Name)
        {
            var set = GetSettings(p);
            if (set == null) { MainChat.SendErrorChat(p, "[错误] 发生了错误."); return; }

            var check = set.Combine.Find(x => x.name == Name);
            if (check == null) { MainChat.SendErrorChat(p, "[错误] 发生了错误!"); return; }

            LSCUI.Close(p);
            set.Combine.Remove(check);
            SaveSettings(p, set);
            MainChat.SendInfoChat(p, "[?] 已移除套装 " + check.name);
            COM_ShowCombine(p);
            return;
        }
        public static CharacterSettings GetSettings(PlayerModel p)
        {
            return JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
        }

        public static void SaveSettings(PlayerModel p, CharacterSettings set)
        {
            p.settings = JsonConvert.SerializeObject(set);
            p.updateSql();
            return;
        }
    }
}
