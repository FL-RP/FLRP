using System.Collections.Generic;
using AltV.Net; using AltV.Net.Async;
using AltV.Net.Data;
using outRp.Models;
using outRp.Globals;
using outRp.Chat;
using Newtonsoft.Json;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;
using outRp.OtherSystem.Textlabels;
using outRp.Tutorial;

namespace outRp.OtherSystem.LSCsystems
{
    public class Barber : IScript
    {
        public static Position barberPos = new Position(-813.8505f, -183.45494f, 37.5531f);
        public static Position tattoPos = new Position(-1155.3099f, -1426.7869f, 4.948f);
        public static void LoadBarberSystem()
        {

            TextLabelStreamer.Create("~b~[~w~理发店~b~]~n~~w~指令: ~g~/barber~n~$300", barberPos, dimension: 0, font: 0, streamRange: 3);
            TextLabelStreamer.Create("~b~[~w~理发店~b~]~n~~w~指令: ~g~/barber~n~$300", new Position(-1282.022f, -1117.0417f, 6.987549f), dimension: 0, font: 0, streamRange: 3);
            TextLabelStreamer.Create("~b~[~w~纹身店~b~]~n~~w~指令: ~g~/tatto~n~$500", tattoPos, dimension: 0, font: 0, streamRange: 3);
            // TODO Dövmeci yapılacak.
        }


        public static bool Key_OpenBarber(PlayerModel p)
        {
            if(p.Position.Distance(barberPos) > 5) {
                if(p.Position.Distance(new Position(-1282.022f, -1117.0417f, 6.987549f)) > 5)
                    return false;
            }

            p.EmitLocked("character:Hair", p.charComps);
            return true;
        }
        
        [Command("barber")]
        public static void COM_OpenBarber(PlayerModel p)
        {
            if(p.Position.Distance(barberPos) > 5) {
                if(p.Position.Distance(new Position(-1282.022f, -1117.0417f, 6.987549f)) > 5)
                {
                    MainChat.SendErrorChat(p, "[错误] 附近没有理发店."); return;
                }
            }

            p.EmitLocked("character:Hair", p.charComps);
        }

        [AsyncClientEvent("barber:HairComp")]
        public async Task Event_SettingsHair(PlayerModel p, string data)
        {
            if(p.cash < 300) { MainChat.SendErrorChat(p, CONSTANT.ERR_MoneyNotEnought); return; }
            p.charComps = data;
            p.cash -= 300;
            await p.updateSql();

            MainChat.SendInfoChat(p, "> 已成功保存角色发型.");
            await Inventory.UpdatePlayerInventory(p);
            CharacterSettings set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            p.EmitLocked("Tatto:Load", JsonConvert.SerializeObject(set.tattos));
            
            if (p.isFinishTut == 27)
            {
                p.isFinishTut = 28;
                p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}理发店");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}嘿, 您已经学会了如何更换发型!");
                MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}现在, 前往下一个教程点吧!");
                GlobalEvents.CheckpointCreate(p, TutorialMain.TutPos[8], 1, 2, new Rgba(255, 0, 0, 0), "Tutorial:Run", "28");
            }              
            return;
        }

        [AsyncClientEvent("barber:Cancel")]
        
        public async Task Event_CancelHair(PlayerModel p)
        {
            p.EmitLocked("character:ServerSync", p.charComps);
            CharacterSettings set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);

            p.EmitLocked("Tatto:Load", JsonConvert.SerializeObject(set.tattos));

            await Inventory.UpdatePlayerInventory(p);
        }


        public static bool Key_StartTatto(PlayerModel p)
        {
            if(p.Position.Distance(tattoPos) > 2) return false;
            p.EmitLocked("Tatto:Show");
            return true;
        }
        
        [Command("tatto")]
        public void Event_StartTatto(PlayerModel p)
        {
            if(p.Position.Distance(tattoPos) > 2) { MainChat.SendErrorChat(p, "[错误] 附近没有纹身店."); return; }
            p.EmitLocked("Tatto:Show");
        }

        [Command("washtatto")]
        public static void COM_DeleteTatto(PlayerModel p)
        {
            if (p.Position.Distance(tattoPos) > 2) { MainChat.SendErrorChat(p, "[错误] 附近没有纹身店."); return; }
            var settings = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            if (settings == null)
                return;

            int totalPrice = 250;
            outRp.OtherSystem.NativeUi.Inputs.SendButtonInput(p, "清理纹身将花费您: $" + totalPrice, "Tatto:Delete");
            return;
        }

        [AsyncClientEvent("Tatto:Delete")]
        public void EVENT_DeleteTatto(PlayerModel p, bool selection, string _trash)
        {
            if (!selection) { MainChat.SendErrorChat(p, "[!] 您取消了纹身清理."); return; }

            var settings = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            if (settings == null)
                return;

            p.cash -= 250;
            MainChat.SendInfoChat(p, "[?] 成功清理纹身, 花费: $250");
            settings.tattos = new List<Tattos>();
            p.settings = JsonConvert.SerializeObject(settings);

            p.EmitLocked("Tatto:Load", JsonConvert.SerializeObject(settings.tattos));

            Inventory.UpdatePlayerInventory(p);
            return;
        }

        [AsyncClientEvent("Tatto:WantToBuy")]
        public void Event_AddTatto(PlayerModel p, string collection, string overlay, string zone)
        {
            if(p.cash < 500) { MainChat.SendErrorChat(p, CONSTANT.ERR_MoneyNotEnought); return; }
            p.cash -= 500;
            CharacterSettings set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            Tattos tatto = new Tattos() { collection = collection, value = overlay, Zone = zone };
            set.tattos.Add(tatto);          

            p.settings = JsonConvert.SerializeObject(set);
            p.updateSql();
            return;
        }

        [AsyncClientEvent("Tatto:Close")]
        public async Task Event_CancelTatto(PlayerModel p)
        {
            CharacterSettings set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);

            p.EmitLocked("Tatto:Load", JsonConvert.SerializeObject(set.tattos));

            await Inventory.UpdatePlayerInventory(p);
            return;
        }  

        [AsyncClientEvent("barber:DecorCancel")]
        public async Task Barber_DecorCancel(PlayerModel p)
        {
            p.EmitLocked("character:ServerSync", p.charComps);
            CharacterSettings set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);

            p.EmitLocked("Tatto:Load", JsonConvert.SerializeObject(set.tattos));

            await Inventory.UpdatePlayerInventory(p);
        }

        [AsyncClientEvent("barber:SaveDecor")]
        public async Task Barber_DecorSave(PlayerModel p, string data)
        {
            p.charComps = data;
            await p.updateSql();

            MainChat.SendInfoChat(p, "> 已成功保存角色妆容.");
            await Inventory.UpdatePlayerInventory(p);
            CharacterSettings set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            p.EmitLocked("Tatto:Load", JsonConvert.SerializeObject(set.tattos));
            return;
        }

        [Command("makeup")]
        public static void COM_ShowDecor(PlayerModel p)
        {
            if(p.Dimension <= 0) { MainChat.SendErrorChat(p, "[错误] 您必须在房屋/产业内才可以化妆."); return; }

            p.EmitLocked("character:Decor", p.charComps);
            return;
        }

    }
}
