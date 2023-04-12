using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net; using AltV.Net.Async;
using AltV.Net.Resources.Chat.Api;
using outRp.Models;
using outRp.Chat;
using outRp.Globals;
using Newtonsoft.Json;

namespace outRp.OtherSystem.LSCsystems
{
    class PdClothingSystem : IScript
    {

        [Command("uniformlist")]
        public async Task COM_ShowClothes(PlayerModel p)
        {
            if(p.adminLevel < 4) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if(p.factionId <= 0) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }

            FactionModel f = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            if(f == null) { MainChat.SendErrorChat(p, "[!] 无效组织."); return; }

            string showText = "";
            foreach(DutyClothes c in f.settings.clothes)
            {
                showText += " | 名称: " + c.name;
            }
            p.SendChatMessage("<center>组织制服</center>");
            p.SendChatMessage(showText);
        }

        [Command("removeuniform")]
        public async Task COM_RemoveClothes(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 4) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (p.factionId <= 0) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if(args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /removeuniform [id]"); return; }            

            FactionModel f = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            if (f == null) { MainChat.SendErrorChat(p, "[!] 无效组织."); return; }



            DutyClothes remove = f.settings.clothes.Find(x => x.name == string.Join(" ", args));
            if(remove == null) { MainChat.SendErrorChat(p, "[错误] 无效制服!"); return; }

            f.settings.clothes.Remove(remove);
            f.Update();
            MainChat.SendInfoChat(p, "[!] 已删除指定制服.");
            return;
        }

        [Command("adduniform")]
        public static void COM_AddClothes(PlayerModel p, params string[] args)
        {
            if(p.adminLevel < 4) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            if(args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /adduniform [名称]"); return; }
            p.EmitLocked("Faction:CheckClothes", string.Join(" ", args));
            return;
        }

        [AsyncClientEvent("Faction:SaveClothes")]
        public async Task EVENT_AddClothes(PlayerModel p, string name, string comp, string props)
        {
            if (p.adminLevel < 2) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (p.factionId <= 0) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }

            FactionModel f = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            if (f == null) { MainChat.SendErrorChat(p, "[!] 无效组织."); return; }

            DutyClothes x = new DutyClothes();
            x.name = name;
            x.cloth = JsonConvert.DeserializeObject<List<DutyClothes.Clothes>>(comp);
            x.prop = JsonConvert.DeserializeObject<List<DutyClothes.Props>>(props);
            f.settings.clothes.Add(x);
            f.Update();
            MainChat.SendInfoChat(p, "[?] 已添加制服.");

            return;
            

        }
    }
}
