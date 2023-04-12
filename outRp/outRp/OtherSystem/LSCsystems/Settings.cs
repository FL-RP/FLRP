using AltV.Net.Async;
using AltV.Net.Resources.Chat.Api;
using outRp.Models;

namespace outRp.OtherSystem.LSCsystems
{
    public class Settings // todo düzenlenecek.
    {
        [Command("settings")]
        public static void COM_SettingPage(PlayerModel p)
        {
            p.EmitLocked("Settings:Open");
            return;
        }
    }
}
