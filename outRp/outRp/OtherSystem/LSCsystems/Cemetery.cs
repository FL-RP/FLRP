using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net; using AltV.Net.Async;
using AltV.Net.Data;
using outRp.OtherSystem.Textlabels;
using outRp.Models;
using outRp.Chat;
using outRp.OtherSystem.NativeUi;
using Newtonsoft.Json;
using outRp.Database;
using System.Linq;
using AltV.Net.Resources.Chat.Api;

namespace outRp.OtherSystem.LSCsystems
{
    public class Cemetery : IScript
    {
        public class CemeteryModel
        {
            public ulong label { get; set; }
            public string Title { get; set; }
            public Position Position { get; set; }
        }

        public static List<CemeteryModel> cemeterys = new List<CemeteryModel>();
        public static void LoadAllCemeterys(string data)
        {
            cemeterys = JsonConvert.DeserializeObject<List<CemeteryModel>>(data);

            foreach(var ce in cemeterys)
            {
                ce.label = TextLabelStreamer.Create(ce.Title, ce.Position, center: true, scale: 0.6f, font: 0, streamRange: 3).Id;
            }
        }

        public static void CreateCemetery(PlayerModel p, int itemID)
        {
            Inputs.SendTypeInput(p, "死者姓名?", "Cemetery:Create", "尸体," + itemID);
            return;
        }

        [AsyncClientEvent("Cemetery:Create")]
        public static async Task CreateCemetery(PlayerModel p, string val, string other)
        {
            string[] _o = other.Split(",");
            if (!Int32.TryParse(_o[1], out int itemID))
                return;

            List<InventoryModel> pInv = await DatabaseMain.GetPlayerInventoryItems(p.sqlID);
            InventoryModel i = pInv.Find(x => x.ID == itemID);
            if (i == null)
                return;

            cksystem.Corpse corpse = JsonConvert.DeserializeObject<cksystem.Corpse>(i.itemData2);
            if (corpse == null)
                return;

            string text = "~r~[~w~无名之墓~r~]";
            if(corpse.Name.Replace("_", " ").ToLower() == val.Replace("_", " ").ToLower())
            {
                PlayerModelInfo target = await Database.DatabaseMain.getCharacterInfo(corpse.Name.Replace(" ", "_"));
                if (target == null)
                    return;
                text = "~r~[~w~墓碑~r~]~w~~n~" + corpse.Name + "~n~" + DateTime.Now.AddYears((target.characterAge * -1)).ToString("dd/MM/yyyy") + "~n~" + DateTime.Now.ToString("dd/MM/yyyy");
            }

            CemeteryModel cem = new CemeteryModel();
            cem.Position = p.Position;
            cem.Title = text;
            cem.label = TextLabelStreamer.Create(cem.Title, cem.Position, font: 0, scale: 0.6f, streamRange: 3).Id;

            cemeterys.Add(cem);

            i.Delete();
            Inventory.UpdatePlayerInventory(p);
            return;
        }

        [Command("adeletecemet")]
        public void AdminDeleteCemetery(PlayerModel p)
        {
            if (p.adminLevel < 1) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            var corp = cemeterys.Where(x => x.Position.Distance(p.Position) < 5).OrderBy(x => x.Position.Distance(p.Position)).FirstOrDefault();
            if (corp == null) { MainChat.SendErrorChat(p, "[错误] 附近没有墓碑!"); return; }

            var lbl = TextLabelStreamer.GetDynamicTextLabel(corp.label);
            if (lbl != null)
                lbl.Delete();

            cemeterys.Remove(corp);

            MainChat.SendInfoChat(p, "[?] 已删除附近墓碑.");
            return;
        }

        [Command("addcemet")]
        public async Task COM_CreateCemetery(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /addcemet [ID]"); return; }
            if (!Int32.TryParse(args[0], out int pSQL)) { MainChat.SendInfoChat(p, "[用法] /addcemet [ID]"); return; }

            PlayerModelInfo t = await Database.DatabaseMain.getCharacterInfo(pSQL);
            if (t == null)
            { MainChat.SendErrorChat(p, "[错误] 无效玩家!"); return; }

            string text = "~r~[~w~墓碑~r~]~w~~n~" + t.characterName.Replace("_", " ") + "~n~" + DateTime.Now.AddYears((t.characterAge * -1)).ToString("dd/MM/yyyy") + "~n~" + DateTime.Now.ToString("dd/MM/yyyy");
            CemeteryModel cem = new CemeteryModel();
            cem.Position = p.Position;
            cem.Title = text;
            cem.label = TextLabelStreamer.Create(cem.Title, cem.Position, font: 0, scale: 0.6f, streamRange: 3).Id;

            cemeterys.Add(cem);
            MainChat.SendInfoChat(p, "[!] 已创建墓碑.");
            return;
        }
    }
}
