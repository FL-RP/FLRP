using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net; using AltV.Net.Async;
using outRp.Models;
using Newtonsoft.Json;
using AltV.Net.Data;
using AltV.Net.Resources.Chat.Api;
using outRp.OtherSystem.NativeUi;
using outRp.Chat;

namespace outRp.OtherSystem.LSCsystems
{
    public class NPC : IScript
    {
        public class NPCModel
        {
            public ulong ID { get; set; }
            public int Owner { get; set; }
            public Position position { get; set; }
            public Rotation rotation { get; set; }
            public string Name { get; set; }
            public DateTime endTime { get; set; }
            public int Hunger { get; set; }            
        }

        public static List<NPCModel> serverNPC = new List<NPCModel>();
        public static CharacterSettings GetSettings(PlayerModel p)
        {
            return JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
        }
        public static void SaveSettings(PlayerModel p, string settings)
        {
            p.settings = JsonConvert.SerializeObject(settings);
            p.updateSql();
        }

        [Command("npcmenu")]
        public static void COM_NPCMenu(PlayerModel p)
        {
            LSCUI.UI ui = new LSCUI.UI();
            ui.Banner = new string[] { "commonmenu", "interaction_bgd" };
            ui.StartPoint = new int[] { 600, 400 };
            ui.Title = "NPC 菜单";
            ui.SubTitle = "NPC 互动菜单";

            LSCUI.Component_Item buyNpc = new LSCUI.Component_Item();
            buyNpc.Header = "NPC 购买 ~b~10点券";
            buyNpc.Description = "购买新的NPC.";
            buyNpc.Trigger = "NPC:Event:BuyNPC";

            foreach(var npc in serverNPC.Where(x => x.Owner == p.sqlID))
            {
                LSCUI.SubMenu npcMenu = new LSCUI.SubMenu();
                npcMenu.Header = "NPC " + npc.Name;
                npcMenu.SubTitle = "NPC相关交互";

                LSCUI.Component_Item npc_name = new LSCUI.Component_Item();
                npc_name.Header = "更换名字";
                npc_name.Description = "允许您更改 NPC 的名称.";
                npc_name.Trigger = "NPC:Event:ChangeName_Step1";
                npc_name.TriggerData = npc.ID.ToString();
                npcMenu.Items.Add(npc_name);

                LSCUI.Component_Item npc_where = new LSCUI.Component_Item();
                npc_where.Header = "NPC 位置?";
                npc_where.Description = "在地图上标记 NPC 的位置.";
                npc_where.Trigger = "NPC:Event:Where";
                npc_where.TriggerData = npc.ID.ToString();
                npcMenu.Items.Add(npc_where);

                LSCUI.Component_Item npc_remove = new LSCUI.Component_Item();
                npc_remove.Header = "~r~NPC 删除";
                npc_remove.Description = "删除NPC";
                npc_remove.Trigger = "NPC:Event:Remove";
                npc_remove.TriggerData = npc.ID.ToString();
                npcMenu.Items.Add(npc_remove);

                ui.SubMenu.Add(npcMenu);
            }

            ui.Send(p);
        }


        [AsyncClientEvent("NPC:Event:BuyNPC")]
        public async Task EVENT_BuyNPC(PlayerModel p)
        {
            AccountModel acc = await Database.DatabaseMain.getAccInfo(p.accountId);
            if (acc == null)
                return;

            if (acc.lscPoint < 10) { MainChat.SendErrorChat(p, "[错误] 您没有足够的点券!"); return; }

        }
    }
}
