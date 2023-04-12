using AltV.Net; using AltV.Net.Async;
using AltV.Net.Data;
using outRp.Models;
using System.Linq;

namespace outRp.OtherSystem.Ped
{
    public class ClientCallBacks : IScript
    {
        [AsyncClientEvent("NPC:UpdatePosition")]
        public void UpdatePosition(PlayerModel p, int npcID, float x, float y, float z)
        {
            Position newCoords = new Position(x, y, z - 1f);
            //Alt.Log(newCoords.X + " " + newCoords.Y + " " + newCoords.Z );
            PedModel ped = PedStreamer.Get((ulong)npcID);

            if (ped == null)
                return;

            ped.Position = newCoords;
        }
        
        [AsyncClientEvent("NPC:SetNetOwner")]
        public void NPCSetNewOwner(PlayerModel p, int npcID)
        {
            PedModel ped = PedStreamer.Get((ulong)npcID);

            if (ped == null)
                return;

            if (!ped.hasNetOwner)
                return;

            AltV.Net.Elements.Entities.IPlayer newOwner = Alt.GetAllPlayers().Where(x => x.Position.Distance(p.Position) < ped.Range).First();

            ped.netOwner = newOwner.Id;
        }  


        

    }
}
