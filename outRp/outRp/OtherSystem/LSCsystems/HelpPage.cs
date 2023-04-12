using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Resources.Chat.Api;
using Newtonsoft.Json;
using outRp.Models;

namespace outRp.OtherSystem.LSCsystems
{
    public class HelpPage : IScript
    {
        public class Category
        {
            public string label { get; set; }
            public string icon { get; set; }
            public string value { get; set; }
        }

        public class Com
        {
            public string komut { get; set; }
            public string aciklama { get; set; }
            public string parametreler { get; set; }
            public string yetki { get; set; }
            public string ornek { get; set; }
        }

        [Command("help")]
        public static void COM_ShowHelp(PlayerModel p)
        {
            //p.EmitAsync("GUI:ToggleMenu", "yardim");
            p.EmitLocked("HelpPage:Show", p.adminLevel);
        }

        [AsyncClientEvent("HelpMenu:GetCategorys")]
        public void EVENT_GetCategory(PlayerModel p)
        {
            var helps = Database.DatabaseMain.getHelpCategorys();
            p.EmitAsync("HelpMenu:PushCategorys", JsonConvert.SerializeObject(helps));
            return;
        }

        [AsyncClientEvent("HelpMenu:GetCommands")]
        public void EVENT_GetCommands(PlayerModel p, string category)
        {
            var coms = Database.DatabaseMain.getHelpCommand(category);
            p.EmitAsync("HelpMenu:PushCommands", JsonConvert.SerializeObject(coms));
            return;
        }

        [AsyncClientEvent("HelpMenu:AddCategory")]
        public void EVENT_AddCategory(PlayerModel p, string name, string icon)
        {
            if (p.adminLevel <= 1)
                return;
            Database.DatabaseMain.addHelpCategory(name, icon);
            EVENT_GetCategory(p);
        }

        [AsyncClientEvent("HelpMenu:DelCategory")]
        public void EVENT_DelCategory(PlayerModel p, string name)
        {
            if (p.adminLevel <= 1)
                return;
            Database.DatabaseMain.removeHelpCategory(name);
            EVENT_GetCategory(p);
        }

        [AsyncClientEvent("HelpMenu:AddCommands")]
        public void EVENT_AddCommands(PlayerModel p, string cat, string com, string desc, string param, string perm, string example)
        {
            if (p.adminLevel <= 1)
                return;

            Database.DatabaseMain.addHelpCommand(cat, com, desc, param, perm, example);
        }

        [AsyncClientEvent("HelpMenu:DelCommand")]
        public void EVENT_DelCommand(PlayerModel p, string name)
        {
            if (p.adminLevel <= 1)
                return;
            Database.DatabaseMain.removeHelpCommand(name);
        }

        [AsyncClientEvent("HelpMenu:GetAdminLevel")]
        public void EVENT_GetAdminLevel(PlayerModel p)
        {
            p.EmitAsync("HelpMenu:Pushadmin", p.adminLevel);
        }
    }
}
