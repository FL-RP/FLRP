using AltV.Net.Data;

namespace outRp.OtherSystem.Vaults
{    
    public class Model
    {
        public int ID { get; set; }
        public Position Position { get; set; }
        public Rotation Rotation { get; set; }
        public VaultType Type { get; set; }
        
        
    }

    public enum VaultType
    {
        Log = 0,
        Metal = 1,
        Plastic = 2,
        Wine = 3,
        Fish = 4,
    }

}
