using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace outRp.Models
{
    public class FactionModel
    {
        public int ID { get; set; }
        public string name { get; set; }
        public int type { get; set; }
        public string typeName { get; set; } = "错误";
        public int cash { get; set; } = 0;
        public int factionExp { get; set; } = 0;
        public int factionLevel { get; set; } = 0;
        public bool isApproved { get; set; } = false;
        public FactionSetting settings { get; set; } = new FactionSetting();
        public List<FactionRank> rank { get; set; } = new List<FactionRank>();
        public int owner { get; set; } = 0;
        public int company { get; set; }
        public int side { get; set; } = 0;
        public Task<int> Create() => Database.DatabaseMain.CreateFactionMysql(this);
        public void Update() => Database.DatabaseMain.UpdateFactionInfo(this);
    }

    public class FactionUserModel
    {
        public int ID { get; set; }
        public string name { get; set; }
        public int rank { get; set; }
    }
    public class FactionSetting
    {
        public bool OOCChat { get; set; } = false;
        public bool PayDay { get; set; } = false;
        public bool isServerFaction { get; set; } = false;
        public int rankCounter { get; set; } = 1;
        public List<DutyClothes> clothes { get; set; } = new List<DutyClothes>();
        public List<FactionEquipment> Equipments { get; set; } = new List<FactionEquipment>();
        public string[] equipmentUrl { get; set; } = new string[] { "", "" };
        public List<FactionMOTD> Motd { get; set; } = new();
        public bool hasRadio { get; set; } = false;
    }

    public class FactionMOTD
    {
        public string Header { get; set; }
        public string Body { get; set; }
        public DateTime Time { get; set; }
        public string Sender { get; set; }
    }
    public class FactionEquipment
    {
        public string name { get; set; }
        public List<FactionEquipmentWeapon> weapon { get; set; } = new List<FactionEquipmentWeapon>();
        public int Armor { get; set; } = 0;
    }

    public class FactionEquipmentWeapon
    {
        public uint weapon { get; set; } = 0;
        public byte tint { get; set; } = 0;
        public List<uint> Components { get; set; } = new List<uint>();
    }
    public class FactionRank
    {
        public int Rank { get; set; } = 1;
        public string RankName { get; set; } = "无";
        public int Payday { get; set; } = 0;
        public FactionRankPermission permission { get; set; } = new FactionRankPermission();
    }
    public class FactionRankPermission
    {
        public bool canUseCar { get; set; } = false;
        public bool canUseVault { get; set; } = false;
        public bool canUsePayday { get; set; } = false;
        public bool canUseInvite { get; set; } = false;
        public bool canUseKick { get; set; } = false;
        public bool canUseRank { get; set; } = false;        
    }

    public class DutyClothes
    {
        public string name { get; set; } = "无";
        public List<Clothes> cloth { get; set; } = new List<Clothes>();
        public List<Props> prop { get; set; } = new List<Props>();

        public class Clothes
        {
            public int comp { get; set; } = 0;
            public int ID { get; set; } = 0;
            public int texture { get; set; } = 0;
        }
        public class Props
        {
            public int prop { get; set; } = 0;
            public int ID { get; set; } = 0;
            public int texture { get; set; } = 0;
        }
    }
}
