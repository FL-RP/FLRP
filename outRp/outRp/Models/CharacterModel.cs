using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Args;
using AltV.Net.Elements.Entities;
using outRp.Core;
using outRp.Globals;
using outRp.OtherSystem.LSCsystems;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace outRp.Models
{
    public class PlayerModel : Player
    {
        private int _sqlID { get; set; }
        public int sqlID
        {
            get { return _sqlID; }
            set
            {
                if (value == _sqlID)
                    return;

                _sqlID = value;
                this.SetStreamSyncedMetaData("Player:ID", value);
            }
        }
        
        private bool _showSqlId { get; set; }
        public bool showSqlId
        {
            get { return _showSqlId; }
            set { _showSqlId = value; fakeName = _fakeName; }
        }
        public int accountId { get; set; }
        private string _fakeName;
        public string fakeName
        {
            get { return _fakeName; }
            set
            {
                if (showSqlId == true)
                {
                    SetStreamSyncedMetaData(Globals.EntityData.PlayerEntityData.characterName, value + " (" + sqlID + ")");
                    _fakeName = value;
                }
                else
                {
                    SetStreamSyncedMetaData(Globals.EntityData.PlayerEntityData.characterName, value);
                    _fakeName = value;
                }

            }
        }
        private string _characterName;
        public string characterName
        {
            get { return _characterName; }
            set { fakeName = value; _characterName = value; }
        }
        public int characterAge { get; set; }
        public int characterExp { get; set; }
        public int characterLevel { get; set; }
        public int gameTime { get; set; } = 1;
        public int factionId { get; set; }
        public int factionRank { get; set; }
        public int businessStaff { get; set; }
        //public string secondLang { get; set; } = "en";
        private int _cash { get; set; }
        public int cash
        {
            get
            {
                return _cash;
            }
            set
            {
                if (this.isOnline)
                {
                    if (_cash < value)
                    {
                        GlobalEvents.NativeNotify(this, "~g~$" + (value - _cash).ToString());
                    }
                    else
                    {
                        GlobalEvents.NativeNotify(this, "~r~$" + (_cash - value).ToString());
                    }
                }

                this.SetSyncedMetaData(EntityData.PlayerEntityData.PlayerCash, value);
                _cash = value;
            }
        }
        public int bankCash { get; set; }
        public bool adminWork { get; set; } = false;
        public int adminLevel { get; set; }
        public bool isCk { get; set; }
        public int jailTime { get; set; }
        public int adminJail { get; set; }
        public int phoneNumber { get; set; }
        public bool firstLogin { get; set; }
        
        public int isFinishTut { get; set; }

        public int spawnLocation { get; set; }

        public bool isGraffiti { get; set; }

        public int tutCar { get; set; }
        private int _hp { get; set; }
        public int hp
        {
            get { return _hp; }
            set
            {
                if (_hp != value)
                {
                    Logger.WriteLogData(Logger.logTypes.HealLog, characterName + " | " + _hp + " -> " + value);
                }
                _hp = value; this.Health = ((ushort)value);

            }
        }
        private bool _isCuffed { get; set; }
        public int Strength { get; set; }
        public int tempStrength { get; set; } = 0;

        public int tempFactionCar { get; set; } = 0;
        private int _maxHp { get; set; } = 1000;
        public int maxHp
        {
            get
            {
                return _maxHp;
            }
            set
            {
                _maxHp = value;
                this.MaxHealth = ((ushort)value);
            }
        }
        public string charComps { get; set; }
        public string settings { get; set; } = "[]";
        public string stats { get; set; }
        private int _sex { get; set; }
        public int sex
        {
            get
            {
                return _sex;
            }
            set
            {
                SetSyncedMetaData("PlayerSex", value); _sex = value;
            }
        }
        public bool isOnline { get; set; } = false;

        public int oldCar { get; set; } = 0;
        //PD
        public int RadioFreq { get; set; } = 1;
        // OTHER SETTINGS
        public bool isPM { get; set; } = true;
        public bool isNews { get; set; } = true;

        public int LastBusiness { get; set; } = 0;
        public int LastPm { get; set; } = -1;

        private InjuredModel _injured { get; set; } = new InjuredModel();
        public InjuredModel injured
        {
            get { return _injured; }
            set
            {
                _injured = value;
                GlobalEvents.UpdateInjured(this);
            }
        }

        public WeaponSystem.WeaponModel melee { get; set; } = null;
        public WeaponSystem.WeaponModel? secondary { get; set; } = null;
        public WeaponSystem.WeaponModel primary { get; set; } = null;

        public bool ckWarWatching { get; set; } = false;
        public bool admiCkWatching { get; set; } = false;
        public bool isCuffed
        {
            get { return _isCuffed; }
            set { this.EmitLocked("player:CuffEvent", value); _isCuffed = value; }
        }

        // Nametag
        public DateTime nameTagFix { get; set; } = DateTime.Now;
        // Emit Defender.
        public List<EmitLog> Emits { get; set; } = new();

        [Obsolete]
        public async Task Setup(int id) => await Database.DatabaseMain.getCharacterFromMysql(this, id); // OnFirstLoad
        public async Task updateSql() => await Database.DatabaseMain.UpdateCharacterInfo(this);
        public PlayerModel(ICore server, IntPtr nativePointer, ushort id) : base(server, nativePointer, id)
        {
            try
            {
                sqlID = 0;
                showSqlId = true;
                accountId = 0;
                characterName = "未登录玩家";
                characterAge = 0;
                characterExp = 0;
                characterLevel = 0;
                gameTime = 1;
                factionId = 0;
                factionRank = 0;
                RadioFreq = 1;
                businessStaff = 0;
                cash = 0;
                bankCash = 0;
                Strength = 30;
                adminLevel = 0;
                isCk = false;
                jailTime = 0;
                phoneNumber = 0;
                firstLogin = false;
                isCuffed = false;
                stats = "[]";
                adminJail = 0;
                settings = "[]";
                //Data Keys  
                adminWork = false;
                adminLevel = 0;
                hp = 200;
                Strength = 30;
                charComps = "[]";
                stats = "[]";
                sex = 0;
                oldCar = 0;
                isPM = true;
                isNews = true;
                injured = new InjuredModel();
                isCuffed = false;
                melee = null;
                secondary = null;
                primary = null;
                maxHp = 1000;
                isFinishTut = 0;
                tutCar = 0;
                spawnLocation = 0;
            }
            catch (Exception ex) { Alt.Log($" 角色 模    {ex}"); }
        }
    }

    public class PlayerModelInfo
    {
        public int sqlID { get; set; } = 0;
        private bool _showSqlId { get; set; }
        public bool showSqlId
        {
            get { return _showSqlId; }
            set { _showSqlId = value; fakeName = _fakeName; }
        }
        public int accountId { get; set; } = 0;
        private string _fakeName;
        public string fakeName
        {
            get { return _fakeName; }
            set
            {
                if (showSqlId == true)
                {
                    _fakeName = value;
                }
                else
                {
                    _fakeName = value;
                }

            }
        }
        private string _characterName;
        public string characterName
        {
            get { return _characterName; }
            set { fakeName = value; _characterName = value; }
        }
        public int characterAge { get; set; }
        public Position Position { get; set; }
        public int Dimension { get; set; }
        public int characterExp { get; set; }
        public int characterLevel { get; set; }
        
        public int isFinishTut { get; set; }

        public bool isGraffiti { get; set; }
        public int spawnLocation { get; set; }
        public int tutCar { get; set; }
        public int gameTime { get; set; }
        public int factionId { get; set; }
        public int factionRank { get; set; }
        public int businessStaff { get; set; }
        public int cash { get; set; }
        public int bankCash { get; set; }
        public int adminLevel { get; set; }
        public bool isCk { get; set; }
        public int jailTime { get; set; }
        public int adminJail { get; set; }
        public int phoneNumber { get; set; }
        public bool firstLogin { get; set; }
        private bool _isCuffed { get; set; }
        public int Strength { get; set; }
        public string charComps { get; set; }
        public string disease { get; set; }
        public string settings { get; set; }
        public string stats { get; set; }
        //PD
        public int RadioFreq { get; set; }
        public bool isCuffed
        {
            get { return _isCuffed; }
            set { _isCuffed = value; }
        }
        public void updateSql() => Database.DatabaseMain.UpdateOfflineCharacterInfo(this);

    }
    public class MyPlayerFactory : IEntityFactory<IPlayer>
    {
        public IPlayer Create(ICore server, IntPtr playerPointer, ushort id)
        {
            try
            {
                //return new PlayerModel(playerPointer, id);
                return new PlayerModel(server, playerPointer, id);
            }
            catch (Exception ex) { Alt.Log($" CHARACTER MODELIN MYPLAYERFACTIRY SI PATLADI {ex}"); return null; }
        }
    }
    public class AccountCharacterModel
    {
        public int ID { get; set; }
        public string characterName { get; set; }
        public int factionId { get; set; }
        public string factionName { get; set; }
        public int characterLimit { get; set; }
        public bool firstLogin { get; set; }
        public bool isCK { get; set; } = false;
    }
    public class CharacterSettings
    {
        public string location { get; set; } = "洛圣都";
        public string nation { get; set; } = "en";
        public string charDetail { get; set; } = "-";
        public int age { get; set; } = 16;
        //public string secondLang { get; set; } = "en";
        public int arms { get; set; } = 0;
        public bool hasMicrophone { get; set; } = false;
        public DriverLicense driverLicense { get; set; } = null;
        public List<PlayerFineModel> fines { get; set; } = new List<PlayerFineModel>();
        public List<OtherLicense> licenses { get; set; } = new List<OtherLicense>();
        public double FontSize { get; set; } = 1.0;
        public DrugUser drugEvents { get; set; } = new DrugUser();
        public List<Tattos> tattos { get; set; } = new List<Tattos>();
        public int GrapichMode { get; set; } = 0;
        public bool jobBan { get; set; } = false;
        public int maxHp { get; set; } = 1000;
        public List<ShareModel> shares = new List<ShareModel>();
        public List<AlternateAnim> anims = new List<AlternateAnim>();
        public int ObjectLimit { get; set; } = 0;

        // Kıyafet Kombin.
        public List<ClothesCombine> Combine = new List<ClothesCombine>();
        public string AdminRankName { get; set; } = "";
        public string[] Font { get; set; } = new string[] { "Microsoft YaHei", "" };
        public bool isLawyer { get; set; } = false;
        public string LawyerDep { get; set; } = "-";

        // Kas sistemi
        public int MuscleExp { get; set; } = 0;
        public int MuscleUsable { get; set; } = 0;
        public int MuslceLast { get; set; } = 0;
        public int odun { get; set; } = 20;
        public int uzum { get; set; } = 40;

        // Flatbed sistem
        public bool hasFlatbed { get; set; } = false;

        // Has Galerry
        public bool hasGallery { get; set; } = false;
        // Pet Sistemi
        public List<PetPerm> petPerm { get; set; } = new List<PetPerm>();
        // Jacking
        public DateTime lastJackDate { get; set; } = DateTime.Now.AddMinutes(-1);

        // gasp
        public DateTime GaspUsage { get; set; } = DateTime.Now.AddDays(-10);
        public DateTime Gasped { get; set; } = DateTime.Now.AddDays(-10);
    }

    // Pet Model
    public class PetPerm
    {
        public string Type { get; set; }
        public string Name { get; set; } = "宠物";
        public DateTime DateTime { get; set; }
    }
    public class PlayerFineModel
    {
        public int finePrice { get; set; }
        public string reason { get; set; }
        public int sender { get; set; }
    }
    public class DriverLicense
    {
        public DateTime licenseDate { get; set; }
        public int finePoint { get; set; }
    }

    public class DrugUser
    {
        public int DrugLevel { get; set; } = 0;
        public int DrugExp { get; set; } = 1;
        public DateTime lastUsed { get; set; } = DateTime.Now;
        public int DrugWantLevel { get; set; } = 0;
    }

    public class OtherLicense
    {
        /// <summary>
        /// 1: PD | 2: FD | 3: SilahLisansı | 4: Avcılık | 5: Avukat | 6: Diğer
        /// </summary>
        public int background { get; set; } = 1;
        public string licenseID { get; set; }
        public string licenseString { get; set; }
        public int givenAdmin { get; set; }
    }

    public class DiseaseModel
    {
        public string DiseaseName { get; set; }
        public int DiseaseValue { get; set; }
    }
    public class InjuredModel
    {
        public bool Injured { get; set; } = false;
        public bool head { get; set; } = false;
        public bool torso { get; set; } = false;
        public bool arms { get; set; } = false;
        public bool legs { get; set; } = false;
        public bool isDead { get; set; } = false;
        public List<DiseaseModel> diseases { get; set; } = new List<DiseaseModel>();

    }


    public class Tattos
    {
        public string collection { get; set; } = "";
        public string value { get; set; } = "";
        public string Zone { get; set; } = "";
    }

    public class ShareModel
    {
        public int CompanyID { get; set; } = 0;
        public int ShareCount { get; set; } = 0;
        public int BuyPrice { get; set; } = 0;
    }


    public class AlternateAnim
    {
        public string name { get; set; }
        public string dict { get; set; }
        public string anim { get; set; }
    }

    public class ClothesCombine
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

    public class EmitLog
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public MValueConst[] args { get; set; }
    }

}

