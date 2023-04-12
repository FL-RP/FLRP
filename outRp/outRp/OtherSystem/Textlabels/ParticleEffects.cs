using AltV.Net;
using AltV.Net.Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Numerics;

namespace outRp.OtherSystem.Textlabels
{
    public class ParticleEffects : IScript
    {
        public class ParticleModel
        {
            public int ID { get; set; }
            public string name { get; set; }
            public Position pos { get; set; }
            public Vector3 rot { get; set; }
            public float scale { get; set; }
            public float xAxis { get; set; }
            public float yAxis { get; set; }
            public float zAxis { get; set; }
            public void Update() => Edit(this);
        }

        public static List<ParticleModel> serverParticles = new List<ParticleModel>();

        public static int IDCounter = 0;
        public static int Create(string particleName, Position position, Vector3 Rotation, float scale = 1, float xaxis = 0, float yaxis = 0, float zaxis = 0)
        {
            ParticleModel n = new ParticleModel();
            n.ID = IDCounter;
            IDCounter++;
            n.name = particleName;
            n.pos = position;
            n.rot = Rotation;
            n.scale = scale;
            n.xAxis = xaxis;
            n.yAxis = yaxis;
            n.zAxis = zaxis;
            serverParticles.Add(n);
            string json = JsonConvert.SerializeObject(n);
            Alt.EmitAllClients("Particle:Create", json);
            return n.ID;
        }

        public static bool Remove(int ID)
        {
            ParticleModel x = serverParticles.Find(x => x.ID == ID);
            if (x == null) { return false; }
            Alt.EmitAllClients("Particle:Remove", x.ID);
            serverParticles.Remove(x);
            return true;
        }

        public static ParticleModel Get(int ID)
        {
            ParticleModel n = serverParticles.Find(x => x.ID == ID);
            return n;
        }
        public static void Edit(ParticleModel x)
        {
            ParticleModel n = serverParticles.Find(x => x.ID == x.ID);
            n = x;
            return;
        }

        /*[AsyncScriptEvent(ScriptEventType.PlayerConnect)]
        public static void LoadAllParticlesToPlayer(PlayerModel p, string reason)
        {
            string json = JsonConvert.SerializeObject(serverParticles);
            p.Emit("
        ", json);
        }*/
    }
}
