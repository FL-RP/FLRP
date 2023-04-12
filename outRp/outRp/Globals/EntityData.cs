namespace outRp.Globals
{
    public class EntityData
    {
        public class GeneralSetting
        {
            public const string DataType = "DataType";
            public const string TypeHouse = "House";
            public const string TypeBusiness = "Business";
        }

        public class PlayerEntityData
        {
            public const string characterName = "PlayerCharacterName";
            public const string PDDuty = "PlayerPDDuty";
            public const string FDDuty = "PlayerFDDuty";
            public const string MDDuty = "PlayerMDDuty";
            public const string NewsDuty = "PlayerNewsDuty";

            public const string isCuffed = "PlayerIsCuffed";
            public const string hasProp = "PlayerHasProp";
            public const string propInfo = "PlayerPropInfo";
            public const string secondPropInfo = "PlayerSecondPropInfo";

            public const string Report = "PlayerReportInfo";

            public const string Bag = "PlayerBag";

            public const string FreezeClothes = "PlayerClothesLock";

            public const string UsingItem = "PlayerUsingItem";

            public const string PlayerCash = "PlayerCash";

            public const string PlayerPhoneId = "PlayerPhoneId";

            public const string PlayerInEmergencyCall_1 = "PlayerInEmergency";

            //Injured Model
            public const string injuredData = "PlayerHasInjuredFromHead";

            // ! Admin - Tester chat datas
            public const string AdminChatClose = "AdminChatClosed";
            public const string TesterChatClose = "TesterChatClosed";

            // AntiCheat
            public const string PlayerInDimension = "PlayerInDimension";
            public const string PlayerWeapon_1 = "PlayerWeapon_1";
            public const string PlayerWeapon_2 = "PlayerWeapon_2";
            public const string PlayerWeapon_3 = "PlayerWeapon_3";

        }
        public class VehicleEntityData
        {
            public const string TuningWantData = "VehicleTuningWant";
            public const string TuningDataStock = "VehicleTuningDataStock";

            public const string VehicleKM = "VehicleKM";

            public const string VehicleTempOwner = "VehicleTempOwner";
            public const string VehicleRentTime = "VehicleRentTime";

            public const string isRadarOn = "VehicleisRadarOn";

            public const string VehicleisTowwed = "VehicleisTowwed";

            public const string VehicleOnTaxiJob = "VehicleOnTaxiJob";
        }
        public class EntranceTypes
        {
            public const string EntranceType = "EntranceType";

            public const string ExitWorldPosition = "ExitWorldPos";

            public const int ExitWorld = 1;
            public const int Business = 2;
            public const int House = 3;
        }
        public class BusinessEntityData
        {
            public const string BusinessId = "BusinessId";
            public const string BusinessInfo = "BusinessInfo";
            public const string BusinessLabel = "BusinessLabel";
            public const string interiorLabelId = "BusinessInteriorLabelId";
        }

        public class CheckpointData
        {
            public const string jobId = "CJobId";
        }

        public class Faction_News
        {
            public const string InBroadCast = "InBroadCast";
        }
        public class Faction_FD
        {

        }
        public class houseData
        {
            public const string HouseID = "HouseID";
            public const string HouseInfo = "HouseINFO";
            public const string HouseLabel = "HouseLabel";
            public const string HouseExitWorldLbl = "HouseExitLbl";
        }
    }
}
