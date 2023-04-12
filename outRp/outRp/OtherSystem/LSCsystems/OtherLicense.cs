using System;
using AltV.Net; using AltV.Net.Async;
using AltV.Net.Resources.Chat.Api;
using outRp.Models;
using outRp.Chat;
using outRp.Globals;
using Newtonsoft.Json;

namespace outRp.OtherSystem.LSCsystems
{
    public class OtherLicense : IScript
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="admin"></param>
        /// <param name="bg"> 1: PD | 2: FD | 3: SilahLisansı | 4: Avcılık | 5: Avukat | 6: Diğer </param>
        /// <param name="licenseName"></param>
        /// <param name="licenseText"></param>
        public static void GiveLicense(PlayerModel p,PlayerModel admin,int bg, string licenseName, string licenseText)
        {
            Models.OtherLicense l = new Models.OtherLicense();
            l.background = bg;
            l.licenseID = licenseName;
            l.licenseString = licenseText;
            l.givenAdmin = admin.sqlID;

            CharacterSettings st = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            st.licenses.Add(l);
            p.settings = JsonConvert.SerializeObject(st);
            p.updateSql();
            MainChat.SendInfoChat(admin, "您给予 " + p.characterName.Replace("_", " ") + " 了 " + licenseName + "<br>内容: " + licenseText);
            MainChat.SendInfoChat(p, admin.characterName.Replace("_", " ") + " 给予了您 " + licenseName + "<br>内容: " + licenseText);
            return;
        }

        [Command("slic")]
        public void ShowLicenseToPlayer(PlayerModel p, params string[] args)
        {
            if(args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /slic [玩家ID] [执照名称]"); return; }
            int tSql; bool isOk = Int32.TryParse(args[0], out tSql);

            if (!isOk) { MainChat.SendInfoChat(p, "[用法] /slic [玩家ID] [执照名称]"); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if(t == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }
            if(p.Position.Distance(t.Position) > 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_NeedNearPlayer); return; }

            CharacterSettings st = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            Models.OtherLicense license = st.licenses.Find(x => x.licenseID == args[1]);
            if(license == null) { MainChat.SendErrorChat(p, "[错误] 无效执照."); return; }

            MainChat.EmoteMe(p, " 掏出一个执照并向 " + t.characterName.Replace("_", " ") + " 出示.");
            t.EmitLocked("License:Show", license.licenseID + "<br>" + license.licenseString + "<br>持证人: " + p.characterName.Replace("_"," "), license.background);
        }

        [Command("mylic")]
        public void ShowSelfLicenses(PlayerModel p)
        {
            CharacterSettings st = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            string LicenseText = "<center>您的角色执照</center>";
            bool hasLicense = false;
            foreach(Models.OtherLicense l in st.licenses)
            {
                hasLicense = true;
                LicenseText += "<br>" + l.licenseID + " | " + l.licenseString; 
            }
            if (!hasLicense) { LicenseText += "<br> 无"; }
            MainChat.SendInfoChat(p, LicenseText, true);
        }
    }
}
