using AltV.Net;
using AltV.Net.Elements.Entities;

namespace outRp.Voice
{
    public class outRp_Voice : IScript
    {
        public static IVoiceChannel channel = Alt.CreateVoiceChannel(true, 20f);
        [ScriptEvent(ScriptEventType.PlayerConnect)]
        public void PlayerConnect(IPlayer player, string reason)
        {
            channel.AddPlayer(player);
        }
        
        [ScriptEvent(ScriptEventType.PlayerDisconnect)]
        public void OnPlayerDisconnect(IPlayer client, string reason)
        {
            channel.RemovePlayer(client);
        }
    }
}