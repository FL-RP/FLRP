using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Enums;
using outRp.Chat;
using outRp.Globals;
using outRp.Models;
using System.Threading.Tasks;

namespace outRp.OtherSystem
{
    public class antiCheat : IScript
    {
        public class AC
        {
            public static string WeaponDataName = "PWepBullet_";
        }

        [AsyncScriptEvent(ScriptEventType.PlayerWeaponChange)]
        public async Task AC_WCEvent(PlayerModel p, uint oldWeapon, uint newWeapon)
        {
            if (!p.isOnline)
                return;

            if (p.adminLevel > 0)
                return;

            if (newWeapon == (uint)WeaponModel.Fist)
                return;

            if (newWeapon == 966099553)
                return;

            if (newWeapon == 0)
                return;

            if (p.HasData(EntityData.PlayerEntityData.PDDuty) || p.HasData(EntityData.PlayerEntityData.FDDuty))
                return;

            if (p.HasSyncedMetaData(AC.WeaponDataName + newWeapon.ToString()))
                return;

            if (p.melee != null && p.melee.WeaponHash == newWeapon)
                return;

            if (p.secondary != null && p.secondary.WeaponHash == newWeapon)
                return;

            if (p.primary != null && p.primary.WeaponHash == newWeapon)
                return;

            if (p.HasData("inPaintball"))
                return;

            if (getAdminCounts() <= 0)
            {
                ACBAN(p, 1, "武器作弊.");
            }
            else
            {
                MainChat.SendAdminChat("反作弊系统: " + p.characterName + " 使用数据不存在的武器 " + p.CurrentWeapon.ToString() + ". [涉嫌作弊]");
            }
            //p.SendChatMessage("Hile Tespit edildi."); //TODO: ACBAN(p, 1(kick), "Silah hilesi");

            return;

        }

        public int getAdminCounts()
        {
            int count = 0;
            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (t.adminLevel > 3)
                    count++;
            }
            return count;
        }

        public static async Task ACBAN(PlayerModel t, int type, string reason)
        {
            switch (type)
            {
                case 1:
                    AccountModel banAccount3 = new AccountModel();

                    banAccount3 = await Database.DatabaseMain.getAccInfo(t.accountId);
                    await t.KickAsync(reason);
                    Core.Logger.WriteLogData(Core.Logger.logTypes.AntiCheat, "[踢出] " + t.characterName.Replace("_", " ") + "(" + banAccount3.forumName + ") | 原因: " + reason);
                    return;

                case 2:
                    AccountModel banAccount = new AccountModel();

                    banAccount = await Database.DatabaseMain.getAccInfo(t.accountId);
                    t.Kick("您已被服务器封禁账号, 原因: " + reason);
                    string name = t.characterName.Replace("_", " ");

                    banAccount.banned = true;
                    await banAccount.Update();

                    //foreach (PlayerModel ct in Alt.GetAllPlayers())
                    //{
                    //    ct.SendChatMessage("{FF0000}[!] {FFFFFF}" + name + "(" + banAccount.forumName + ")" + " {FF0000}isimli oyuncu {FFFFFF}Sistem {FF0000}tarafından sunucudan yasaklandı.<br>Sebep: {FFFFFF}" + reason);
                    //}
                    Core.Logger.WriteLogData(Core.Logger.logTypes.AntiCheat, "[封禁] " + name + "(" + banAccount.forumName + ") | 封禁: 系统原因: " + reason);
                    return;

                case 3:
                    await Database.DatabaseMain.AddSocialBan(t.SocialClubId, "System", reason);

                    AccountModel banAccount2 = await Database.DatabaseMain.getAccInfo(t.accountId);
                    banAccount2.banned = true;
                    await banAccount2.Update();
                    //foreach (PlayerModel ct in Alt.GetAllPlayers())
                    //{
                    //    ct.SendChatMessage("{FF0000}[!] {FFFFFF}" + t.characterName.Replace("_", " ") + "(" + banAccount2.forumName + ")" + " {FF0000}isimli oyuncu {FFFFFF}Sistem {FF0000}tarafından sunucudan yasaklandı (Social Club Ban).<br>Sebep: {FFFFFF}" + reason);
                    //}

                    t.Kick("您的R星账号和角色已经被服务器封禁, 原因: " + reason);
                    Core.Logger.WriteLogData(Core.Logger.logTypes.AntiCheat, "[封禁] " + t.characterName.Replace("_", " ") + "(" + banAccount2.forumName + ") | 封禁: 系统 | 原因: " + reason);
                    Core.Logger.WriteLogData(Core.Logger.logTypes.AntiCheat, "[R星封禁] " + t.SocialClubId.ToString() + " | 封禁: 系统 | 原因: " + reason);
                    return;

            }
        }


        public static void BAN(PlayerModel t, int type, string reason)
        {
            switch (type)
            {
                case 1:
                    AccountModel banAccount3 = new AccountModel();

                    banAccount3 = Database.DatabaseMain.getAccInfo2(t.accountId);
                    t.Kick("您已被踢出服务器, 原因: " + reason);
                    Core.Logger.WriteLogData(Core.Logger.logTypes.AntiCheat, "[踢出] " + t.characterName.Replace("_", " ") + "(" + banAccount3.forumName + ") | 原因: " + reason);
                    return;

                case 2:
                    AccountModel banAccount = Database.DatabaseMain.getAccInfo2(t.accountId);
                    t.Kick("您已被服务器封禁, 原因: " + reason);
                    string name = t.characterName.Replace("_", " ");

                    banAccount.banned = true;
                    banAccount.Update2();

                    Core.Logger.WriteLogData(Core.Logger.logTypes.AntiCheat, "[封禁] " + name + "(" + banAccount.forumName + ") | 封禁: 系统原因: " + reason);
                    return;

            }
        }


        // Dosya integration CHECK
    }
}
