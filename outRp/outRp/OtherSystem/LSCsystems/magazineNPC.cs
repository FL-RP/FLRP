

/*namespace outRp.OtherSystem.LSCsystems
{
    public class magazineNPC : IScript
    {
        public static Position npcPosition = new Position(855, -2120, 30);
        public static int FactionLevel = 2;
        public static bool canUse_Pistol = true;
        public static bool canUse_SubMachine = true;
        public static bool canUse_ShotGun = true;
        public static bool canUse_AssaultRifle = true;




        [Command("mermisatinal")]
        public async Task COM_OpenMagazineMenu(PlayerModel p)
        {
            if (p.Position.Distance(npcPosition) > 3)
            {
                MainChat.SendErrorChat(p, "[HATA] Bu komutu kullanabilmek için mermi satın alma bölgesinde olmalısınız!");
                return;
            }

            FactionModel playerFaction = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            if (playerFaction.type != 7 && playerFaction.type != 2) { MainChat.SendErrorChat(p, "[HATA] Bu komutu kullanabilmek için illegal tipli bir oluşumda olmalısınız!"); return; }
            if (playerFaction.factionLevel < FactionLevel) { MainChat.SendErrorChat(p, "[HATA] Bu komutu kullanabilmek için birliğiniz yeterli seviyede değil. Gereken seviye: " + FactionLevel); return; }


            LSCUI.UI ui = new LSCUI.UI();
            ui.StartPoint = new int[] { 600, 400 };
            ui.Title = "Mermi Satın alma menüsü";
            ui.SubTitle = "Bu menü aracılığıyla silahlarınız için şarjör satın alabilirsiniz.";

            LSCUI.SubMenu pistols = new LSCUI.SubMenu();
            pistols.Header = "Tabancalar " + ((canUse_Pistol) ? "" : "~r~Kapalı");
            pistols.SubTitle = "Tabanca model silahlar için mermi almanızı sağlar.";
            pistols.StartPoint = new int[] { 600, 400 };
            ui.SubMenu.Add(pistols);

            LSCUI.SubMenu subMachines = new LSCUI.SubMenu();
            subMachines.Header = "Yarı Otomatikler " + ((canUse_SubMachine) ? "" : "~r~Kapalı");
            subMachines.SubTitle = "Yarı otomatik model silahlar için mermi almanızı sağlar.";
            subMachines.StartPoint = new int[] { 600, 400 };
            ui.SubMenu.Add(subMachines);

            LSCUI.SubMenu shotguns = new LSCUI.SubMenu();
            shotguns.Header = "Pompalılar " + ((canUse_ShotGun) ? "" : "~r~Kapalı");
            shotguns.SubTitle = "Pompalı model silahlar için mermi almanızı sağlar.";
            shotguns.StartPoint = new int[] { 600, 400 };
            ui.SubMenu.Add(shotguns);

            LSCUI.SubMenu assautRifles = new LSCUI.SubMenu();
            assautRifles.Header = "Tam Otomatik " + ((canUse_AssaultRifle) ? "" : "~r~Kapalı");
            assautRifles.SubTitle = "Tam otomatik model silahlar için mermi almanızı sağlar.";
            assautRifles.StartPoint = new int[] { 600, 400 };
            ui.SubMenu.Add(assautRifles);


            if (canUse_Pistol)
            {
                LSCUI.Component_Item weapon_pistol = new LSCUI.Component_Item();
                weapon_pistol.Header = "Pistol ~g~$200";
                weapon_pistol.Description = "x30'lık bir mermi kutusu alırsınız.";
                weapon_pistol.Trigger = "Magazine:Buy";
                weapon_pistol.TriggerData = "weapon_pistol,30,200";
                pistols.Items.Add(weapon_pistol);

                LSCUI.Component_Item weapon_pistol_ceramic = new LSCUI.Component_Item();
                weapon_pistol_ceramic.Header = "Ceramic Pistol ~g~$200";
                weapon_pistol_ceramic.Description = "x30'lık bir mermi kutusu alırsınız.";
                weapon_pistol_ceramic.Trigger = "Magazine:Buy";
                weapon_pistol_ceramic.TriggerData = "weapon_ceramicpistol,30,200";
                pistols.Items.Add(weapon_pistol_ceramic);

                LSCUI.Component_Item weapon_pistol50 = new LSCUI.Component_Item();
                weapon_pistol50.Header = "Pistol 50 ~g~$400";
                weapon_pistol50.Description = "x30'lık bir mermi kutusu alırsınız.";
                weapon_pistol50.Trigger = "Magazine:Buy";
                weapon_pistol50.TriggerData = "weapon_pistol50,30,400";
                pistols.Items.Add(weapon_pistol50);

                LSCUI.Component_Item weapon_pistol_combat = new LSCUI.Component_Item();
                weapon_pistol_combat.Header = "Combat Pistol 50 ~g~$450";
                weapon_pistol_combat.Description = "x30'lık bir mermi kutusu alırsınız.";
                weapon_pistol_combat.Trigger = "Magazine:Buy";
                weapon_pistol_combat.TriggerData = "weapon_combatpistol,30,450";
                pistols.Items.Add(weapon_pistol_combat);

                LSCUI.Component_Item weapon_pistol_vintage = new LSCUI.Component_Item();
                weapon_pistol_vintage.Header = "Vintage Pistol 50 ~g~$500";
                weapon_pistol_vintage.Description = "x30'lık bir mermi kutusu alırsınız.";
                weapon_pistol_vintage.Trigger = "Magazine:Buy";
                weapon_pistol_vintage.TriggerData = "weapon_vintagepistol,30,500";
                pistols.Items.Add(weapon_pistol_vintage);
            }

            if (canUse_SubMachine)
            {
                LSCUI.Component_Item weapon_microsmg = new LSCUI.Component_Item();
                weapon_microsmg.Header = "Micro SMG ~g~$600";
                weapon_microsmg.Description = "x30'lik bir şarjör alırsınız.";
                weapon_microsmg.Trigger = "Magazine:Buy";
                weapon_microsmg.TriggerData = "weapon_microsmg,60,600";
                subMachines.Items.Add(weapon_microsmg);

                LSCUI.Component_Item weapon_minismg = new LSCUI.Component_Item();
                weapon_minismg.Header = "Mini SMG ~g~$600";
                weapon_minismg.Description = "x30'lik bir şarjör alırsınız.";
                weapon_minismg.Trigger = "Magazine:Buy";
                weapon_minismg.TriggerData = "weapon_minismg,60,600";
                subMachines.Items.Add(weapon_minismg);

                LSCUI.Component_Item weapon_machinepistol = new LSCUI.Component_Item();
                weapon_machinepistol.Header = "Machine Pistol ~g~$600";
                weapon_machinepistol.Description = "x30'lik bir şarjör alırsınız.";
                weapon_machinepistol.Trigger = "Magazine:Buy";
                weapon_machinepistol.TriggerData = "weapon_machinepistol,60,600";
                subMachines.Items.Add(weapon_machinepistol);

                LSCUI.Component_Item weapon_smg = new LSCUI.Component_Item();
                weapon_smg.Header = "SMG ~g~$700";
                weapon_smg.Description = "x30'lik bir şarjör alırsınız.";
                weapon_smg.Trigger = "Magazine:Buy";
                weapon_smg.TriggerData = "weapon_smg,60,700";
                subMachines.Items.Add(weapon_smg);
            }

            if (canUse_ShotGun)
            {
                LSCUI.Component_Item weapon_pumpshotgun_mk2 = new LSCUI.Component_Item();
                weapon_pumpshotgun_mk2.Header = "Pump Shotgun ~g~$500";
                weapon_pumpshotgun_mk2.Description = "x12'lik bir şarjör alırsınız.";
                weapon_pumpshotgun_mk2.Trigger = "Magazine:Buy";
                weapon_pumpshotgun_mk2.TriggerData = "weapon_pumpshotgun_mk2,24,500";
                shotguns.Items.Add(weapon_pumpshotgun_mk2);

                LSCUI.Component_Item weapon_dobulebarrel = new LSCUI.Component_Item();
                weapon_dobulebarrel.Header = "Double Barrel Shotgun ~g~$300";
                weapon_dobulebarrel.Description = "x12'lik bir şarjör alırsınız.";
                weapon_dobulebarrel.Trigger = "Magazine:Buy";
                weapon_dobulebarrel.TriggerData = "weapon_dbshotgun,24,300";
                shotguns.Items.Add(weapon_dobulebarrel);
            }

            if (canUse_AssaultRifle)
            {
                LSCUI.Component_Item weapon_compactrifle = new LSCUI.Component_Item();
                weapon_compactrifle.Header = "Compact Rifle ~g~$800";
                weapon_compactrifle.Description = "x30'lik bir şarjör alırsınız.";
                weapon_compactrifle.Trigger = "Magazine:Buy";
                weapon_compactrifle.TriggerData = "weapon_compactrifle,30,750";
                assautRifles.Items.Add(weapon_compactrifle);

                LSCUI.Component_Item weapon_assault = new LSCUI.Component_Item();
                weapon_assault.Header = "Assault Rifle ~g~$750";
                weapon_assault.Description = "x30'lik bir şarjör alırsınız.";
                weapon_assault.Trigger = "Magazine:Buy";
                weapon_assault.TriggerData = "weapon_assaultrifle,30,800";
                assautRifles.Items.Add(weapon_assault);

                LSCUI.Component_Item weapon_advenced = new LSCUI.Component_Item();
                weapon_advenced.Header = "Advanced Rifle ~g~$1000";
                weapon_advenced.Description = "x30'lik bir şarjör alırsınız.";
                weapon_advenced.Trigger = "Magazine:Buy";
                weapon_advenced.TriggerData = "weapon_advancedrifle,30,1000";
                assautRifles.Items.Add(weapon_advenced);
            }

            ui.Send(p);
            return;
        }

        [AsyncClientEvent("Magazine:Buy")]
        public async Task EVENT_BuyMagazine(PlayerModel p, string _val)
        {
            if (p.Ping > 250)
                return;
            string[] val = _val.Split(',');
            if (!Int32.TryParse(val[1], out int AmmoCount) || !Int32.TryParse(val[2], out int Price))
                return;

            if (p.cash < Price) { MainChat.SendErrorChat(p, "[HATA] Yeterli paranız bulunmuyor."); return; }
            if (Price < 0)
            {
                antiCheat.ACBAN(p, 3, "Mermi NPC Dump");
                return;
            }

            ServerItems item = Items.LSCitems.Find(x => x.ID == 46);

            WeaponSystem.MagazineModel newWep = new WeaponSystem.MagazineModel()
            {
                bulletCount = AmmoCount,
                toWeapon = Alt.Hash(val[0])
            };

            item.name = val[0].Split("_")[1];
            item.amount = 1;
            item.data = "0";
            item.data2 = JsonConvert.SerializeObject(newWep);
            if (await Inventory.AddInventoryItem(p, item, 1))
            {
                p.cash -= Price;
                p.updateSql();
                MainChat.SendInfoChat(p, "[?] " + val[0].Split("_")[0] + " isimli şarjörü $" + Price + " karşılığında satın aldınız.");
                return;
            }
            else
            {
                MainChat.SendErrorChat(p, "[HATA] Üzerinizde yeterli alan bulunamadı!");
            }
        }

        // Admin Commands
        [Command("mermiduzenle")]
        public static void COM_EditNPC(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, "[HATA] Bu komutu kullanabilmek için yetkiniz yok!"); return; }
            if (args.Length <= 1) { MainChat.SendInfoChat(p, "[Kullanım] /mermiduzenle [çeşit] [Değer]"); return; }

            switch (args[0])
            {
                case "ackapat":
                    switch (args[1])
                    {
                        case "pistol":
                            canUse_Pistol = !canUse_Pistol;
                            MainChat.SendInfoChat(p, "[X] Pistol mermi satışını " + ((canUse_Pistol) ? "açtınız." : "kapattınız."));
                            return;

                        case "shotgun":
                            canUse_ShotGun = !canUse_ShotGun;
                            MainChat.SendInfoChat(p, "[X] Shotgun mermi satışını " + ((canUse_ShotGun) ? "açtınız." : "kapattınız."));
                            return;

                        case "submachine":
                            canUse_SubMachine = !canUse_SubMachine;
                            MainChat.SendInfoChat(p, "[X] SubMachine mermi satışını " + ((canUse_SubMachine) ? "açtınız." : "kapattınız."));
                            return;

                        case "assaultrifle":
                            canUse_AssaultRifle = !canUse_AssaultRifle;
                            MainChat.SendInfoChat(p, "[X] AssaultRifle mermi satışını " + ((canUse_AssaultRifle) ? "açtınız." : "kapattınız."));
                            return;
                    }
                    return;

                case "birlikseviye":
                    if (!Int32.TryParse(args[1], out int factionLevel)) { MainChat.SendInfoChat(p, "[Kullanım] /mermiduzenle birlikseviye [seviye]"); return; }
                    FactionLevel = factionLevel;
                    MainChat.SendInfoChat(p, "[X] Mermi satın alabilecek birlik seviyesini " + FactionLevel + " olarak ayarladınız.");
                    return;
            }
        }
    }
}*/