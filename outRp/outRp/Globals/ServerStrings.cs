//using System.Security.RightsManagement;
using AltV.Net.Data;

namespace outRp.Globals
{
    public class ChatIcons
    {
        public const string infoIcon = "fas fa-times-circle";
        public const string DepartmentChatIcon = "<i class='fad fa-bell'></i>";
    }

    public class CONSTANT
    {
        public const string CharacterNotFoundError = "[错误] 未找到指定角色, 请重试.";
        public const string ERR_AdminLevel = "[错误] 无权操作.";

        public const string ERR_PlayerNotFound = "[错误] 无效玩家.";
        public const string ERR_NotUseOnYou = "[错误] 无法对自己使用此指令.";
        public const string ERR_ValueNotNegative = "[错误] 无效金额.";
        public const string ERR_MoneyNotEnought = "[错误] 您没有足够的钱.";
        public const string ERR_NotNearTarget = "[错误] 您不在目标附近.";
        public const string ERR_PermissionError = "[错误] 无权操作.";
        public const string ERR_NeedNearPlayer = "[错误] 您不在指定玩家附近.";

        #region AdminStrings
        public const string COM_Goto = "goto";
        public const int LVL_Goto = 1;
        public const string INFO_COM_Goto_Player = "传送至 {0}";
        public const string INFO_COM_Goto_Target = "管理员 {0} 传送至您角色的位置.";

        public const string COM_Get = "get";
        public const int LVL_Get = 1;
        public const string INFO_COM_Get_Player = "您将 {0} 传送至您角色的位置.";
        public const string INFO_COM_Get_Target = "{0} 将您传送至 Ta 角色的位置.";


        public const string COM_CreateBusiness = "addbiz";
        public const int LVL_CreateBusiness = 3;
        public const string COM_BusinessCreateDesc = "[用法] /addbiz [选项] [名称]<br> 可使用选项: 1: 车辆租赁 / 2: 批发店 / 3: 车库/仓库 / 4: 医院 / 5: 商店 / 6: 农场 / 7: 洗车店.";

        public const string COM_DeleteBusiness = "deletebiz";
        public const int LVL_DeleteBusiness = 3;

        public const string COM_UpdateBusiness = "aeditbiz";
        public const int LVL_UpdateBusiness = 3;
        public const string COM_BusinessUpdateDesc = "[用法] /editbiz [ID] [选项] [参数]<br>{0AD63C}可用选项:<br>{BDD60A} name : 产业名称<br>price: 价格<br>lock: 上锁<br>owner: 业主<br>type: 类型<br> kasa : iş yeri kasasındaki parayı düzenler. <br>dimension : iş yeri dimensionunu ayarlar.<br>interior : iş yerinin interiorunu ayarlar.<br>giris : iş yeri giriş kapısının yerini ayarlar.  ";

        public const string COM_EditCharacter = "editstat";
        public const int LVL_EditCharacter = 2;
        public const string DESC_EditCharacter = "[用法] /editstat ID 选项 数值 <br> 可用选项: name, age, level, factionid, bwork, cash, bank, jailtime, vw, nation, paydaytime";
        public const string INFO_COM_EditCharacter_Player = "成功将 {0} 的 {1} 更改为 {2}";
        public const string INFO_COM_EditCharacter_Target = "{0} 将您角色的 {1} 更改为 {2}";

        public const string COM_CreateVehicle = "spawncar";
        public const int LVL_CreateVehicle = 5;
        public const string DESC_CreateVehicle = "[用法] /spawncar model";

        public const string COM_EditVehicle = "editcar";
        public const int LVL_EditVehicle = 2;
        public const string DESC_EditVehicle = "[用法] /editcar [选项] [数值]<br>{0AD63C}可用选项;<br>{BDD60A} owner : 所有者 <br>jobid : 注册ID <br>color1: 主颜色 (RGBA)(例如: 255,0,0,255) <br>color2 : 副颜色<br> factionid : 组织ID <br> fuel : 油箱 (1-100) <br>plate : 车牌号 <br>lock : 上锁 <br>engine : 引擎开关 <br>price : 价格 <br>tax: 税 <br>fine: 罚款.";

        public const string COM_ADice6 = "adice";
        public const int LVL_ADice6 = 5;
        public const string DESC_ADice6 = "[用法] /adice [数值]";

        public const string COM_CreateHouse = "addhouse";
        public const int LVL_CreateHouse = 3;
        public const string DESC_CreateHouse = "[用法] /" + COM_CreateHouse + " [价格]";

        public const string COM_AEditHouse = "aedithouse";
        public const int LVL_AEdiotHouse = 3;
        public const string DESC_AEditHouse = "[用法] /" + COM_AEditHouse + " [ID] [选项] [数值]<br>可用选项;<br>name: 房屋名称<br>price: 房屋价格<br>owner : 所有者 <br>vw: 虚拟世界 <br>interior: 内饰出口坐标";

        public const string COM_CharacterCreateSend = "sendtocr";
        public const int LVL_CharacterCreateSend = 2;
        public const string DESC_CharacterCreateSend = "[用法] /sendtocr [ID]";

        public const string COM_TpToOtherPlayer = "sendto";
        public const int LVL_TpToOtherPlayer = 2;
        public const string DESC_TpToOtherPlayer = "[用法] /sendto [目标] [被传送玩家]";

        public const string COM_Announcement = "an";
        public const int LVL_Announcement = 2;
        public const string DESC_Announcement = "[用法] /an [文本]";

        public const string COM_KickPlayer = "kick";
        public const int LVL_KickPlayer = 2;
        public const string DESC_KickPlayer = "[用法] /kick [id] [原因]";

        public const string COM_SKickPlayer = "skick";
        public const int LVL_SKickPlayer = 2;
        public const string DESC_SKickPlayer = "[用法] /skick [id] [原因]";

        public const string COM_GotoBusiness = "gotobiz";
        public const int LVL_GotoBusiness = 1;
        public const string DESC_GotoBusiness = "[用法] /gotobiz [产业ID]";

        public const string COM_GotoHouse = "gotohouse";
        public const int LVL_GotoHouse = 1;
        public const string DESC_GotoHouse = "[用法] /gotohouse [房屋ID]";

        public const string COM_GotoCar = "gotocar";
        public const int LVL_GotoCar = 1;
        public const string DESC_GotoCar = "[用法] /gotocar [车辆ID]";

        public const string COM_GetCar = "getcar";
        public const int LVL_GetCar = 1;
        public const string DESC_GetCar = "[用法] /getcar [车辆ID]";

        public const string COM_AWork = "aduty";

        public const string COM_ShowAdversiments = "adlist";
        public const int LVL_ShowAdversiments = 1;

        public const string COM_AnswerAdversiment = "doad";
        public const int LVL_AnswerAdversiment = 1;
        public const string DESC_AnswerAdversiment = "[用法] /doad [id] [选项]<br>可用选项: yes | no";
        public const string ERR_AnswerAdversiment = "[错误] 无效广告";

        public const string COM_Reports = "reports";
        public const int LVL_Reports = 1;
        public const string ERR_Reports = "无举报.";

        public const string COM_AnswerReport = "are";
        public const int LVL_AnswerReport = 1;
        public const string DESC_AswerReport = "[用法] /are [id] [处理]<br>可用处理:<br>已接手举报, 并通知玩家.<br>已无回复并关闭举报, 并通知玩家.<br>已处理完成, 并关闭举报.";

        public const string COM_HelpReqs = "askql";
        public const int LVL_HelpReqs = 1;

        public const string COM_AnswerHelpReq = "acpq";
        public const int LVL_AsnwerHelpReq = 1;
        public const string DESC_AnswerHelpReq = "[用法] /acpq [求助ID] [回复]";

        public const string COM_AddMarket = "addmarket";
        public const int LVL_AddMarket = 2;
        public const string DESC_AddMarket = "[用法] /addmarket [类型]";

        public const string COM_AddMarketItem = "addmitem";
        public const int LVL_AddMarketItem = 2;
        public const string DESC_AddMarketItem = "[用法] /addmitem [物品ID] [价格]";

        public const string COM_AddMarketStock = "addmstock";
        public const int LVL_AddMarketStock = 2;
        public const string DESC_AddMarketStock = "[用法] /addmstock [物品ID] [库存]";

        public const string COM_DeleteMarket = "desmarket";
        public const int LVL_DeleteMarket = 2;
        public const string DESC_DeleteMarket = "[用法] /desmarket [商店ID]";

        public const string COM_GiveWeapon = "givegun";
        public const int LVL_GiveWeapon = 5;
        public const string DESC_GiveWeapon = "[用法] /givegun [武器ID] [弹药]";

        public const string COM_CheckCharacter = "check";
        public const int LVL_CheckCharacter = 2;
        public const string DESC_CheckCharacter = "[用法] /check [ID]";

        public const string COM_CreateDoor = "makedoor";
        public const int LVL_CreateDoor = 3;

        public const string COM_ShowNearDoors = "neardoor";
        public const int LVL_ShowNearDoors = 2;

        public const string COM_EditDoor = "editdoor";
        public const int LVL_EditDoor = 3;
        public const string DESC_EditDoor = "[用法] /editdoor [门ID] [des/type/owner] [数值]";

        public const string COM_AddBlip = "addblip";
        public const int LVL_AddBlip = 2;
        public const string DESC_AddBlip = "[用法] /addblip [图标] [号码] [名称]";

        public const string COM_ShowNearBlips = "nearblip";
        public const int LVL_ShowNearBlips = 2;

        public const string COM_DeleteBlip = "deblip";
        public const int LVL_DeleteBlip = 2;

        public const string COM_EditBlip = "editblip";
        public const string DESC_EditBlip = "[用法] /editblip [选项] [数值] [标记点名称]<br>可用选项: sprite / color / number<br>例如: /editblip sprite 3 测试标记点";

        public const string COM_AdminRepairVehicle = "afixcar";
        public const int LVL_AdminRepairVehicle = 2;

        public const string COM_FlipVehicle = "flipcar";
        public const int LVL_FlipVehicle = 1;

        public const string COM_SetHp = "sethp";
        public const int LVL_SetHp = 2;
        public const string DESC_SetHp = "[用法] /sethp [id] [血量(0-100)]";

        public const string COM_SetArmor = "setarmor";
        public const int LVL_SetArmor = 3;
        public const string DESC_SetArmor = "[用法] /setarmor [id] [护甲(0-100)]";

        public const string COM_CloseAdminChat = "cadminchat";
        public const int LVL_CloseAdminChat = 2;

        public const string COM_AdminChat = "ac";
        public const int LVL_AdminChat = 2;
        public const string DESC_AdminChat = "[用法] /ac [文本]";

        public const string COM_ClosaHelperChat = "closetc";
        public const int LVL_ClosaHelperChat = 1;

        public const string COM_HelperChat = "tc";
        public const int LVL_HelperChat = 1;
        public const string DESC_HelperChat = "[用法] /tc [志愿者频道]";

        public const string COM_AdminSlap = "slap";
        public const int LVL_AdminSlap = 2;
        public const string DESC_AdminSlap = "[用法] /slap [id]";

        public const string COM_Freeze = "freeze";
        public const int LVL_Freeze = 2;
        public const string DESC_Freeze = "[用法] /freeze [id] [on/off]";

        public const string COM_AdminRemoveRoadBlock = "arb";
        public const int LVL_AdminRemoveRoadBlock = 2;
        public const string DESC_AdminRemoveRoadBlock = "[用法] /arb [id]";

        public const string COM_AdminRevive = "revive";
        public const int LVL_AdminRevive = 2;
        public const string DESC_AdminRevive = "[用法] /revive [id]";

        public const string COM_SetDimension = "setvw";
        public static int LVL_SetDimension = 1;
        public const string DESC_SetDimension = "[用法] /setvw [id] [虚拟世界ID]";

        public const string COM_AdminRemoveRadio = "clearboombox";
        public const int LVL_AdminRemoveRadio = 1;

        public const string COM_CheckPlayerFaction = "checkfaction";
        public const int LVL_CheckPlayerFaction = 2;
        public const string DESC_CheckPlayerFaction = "[用法] /checkfaction [玩家ID]";

        public const string COM_EditPlayerFaction = "aeditfaction";
        public const int LVL_EditPlayerFaction = 3;
        public const string DESC_EditPlayerFaction = "[用法] /aeditfaction [组织ID] [选项] [数值]<br>可用选项:<br>type - owner - cash - level - name";

        public const string COM_AdminGiveLicense = "givelic";
        public const int LVL_AdminGiveLicense = 2;
        public const string DESC_AdminGiveLicense = "[用法] /givelic [id] [选项] [执照名称] [说明]<br>选项: 1: 执法 | 2: FD | 3: 武器 | 4: 狩猎 | 5: 律师 | 6: 其他";

        public const string COM_AdminCheckLicense = "checklic";
        public const int LVL_AdminCheckLicense = 2;
        public const string DESC_AdminCheckLicense = "[用法] /checklic [玩家ID]";

        public const string COM_AdminDeleteLicense = "disarmlic";
        public const int LVL_AdminDeleteLicense = 2;
        public const string DESC_AdminDeleteLicense = "[用法] /disarmlic [玩家ID] [执照ID]";


        public const string COM_Ajail = "ajail";
        public const int LVL_Ajail = 2;
        public const string DESC_Ajail = "[用法] /ajail [id] [时间(分钟)] [原因]";

        public const string COM_Ban = "ban";
        public const int LVL_Ban = 2;
        public const string DESC_Ban = "[用法] /ban [id] [原因]";

        public const string COM_SBan = "sban";
        public const int LVL_SBan = 4;
        public const string DESC_SBan = "[用法] /sban [id] [原因]";
        #endregion

        #region Chat Commands
        public const string COM_EmoteMe = "me";
        public const string COM_EmoteDo = "do";
        public const string COM_Shout = "s";
        public const string COM_LowVoice = "l";
        public const string COM_Whisper = "w";
        public const string COM_PM = "pm";
        public const string COM_OOC = "b";

        public const string COM_VehicleChat = "c";
        public const string DESC_VehicleChat = "[用法] /c [文本]";

        public const string COM_Megafon = "m";
        public const string DESC_Megafon = "[用法] /m [文本]";
        public const string ERR_Megafon = "[错误] 您必须靠近或在车内";
        #endregion

        #region VehicleStrings
        public const string COM_VQueryVehicleNotIn = "您不在车内";
        public const string COM_VQueryVehNotFound = "无效车辆.";
        public const string COM_VQueryVehNotNear = "附近没有车辆.";
        public const string COM_VQueryVehNotDriver = "您不在车辆主驾驶.";
        public const string COM_VQueryVehNotHaveKeys = "您没有此车的钥匙.";
        public const string COM_VQueryVehNotOwnYou = "[错误] 您不是这辆车的主人.";

        public const string COM_VehRent = "rentcar";
        public const string COM_RentVehNotRentable = "此车不出租.";
        public const string COM_RentAlreadyExist = "此车已被租用";
        public const string COM_RentOver = "您租用的车辆期限已经到期.";
        public const string DESC_VehRent = "[用法] /rentcar [时间(分钟)]";

        public const string COM_VehSave = "csave";

        public const string COM_VehEngine = "engine";
        public const string EMOTE_VQueryEngineOff = " 拔出车钥匙并关闭了车辆发动机.";
        public const string EMOTE_VQueryEngineOn = " 插入车辆钥匙并启动了车辆.";

        public const string COM_VehLock = "vlock";
        public const string EMOTE_VQueryLockVeh = " 取出车钥匙锁上了车辆.";
        public const string EMOTE_VQueryUnlockVeh = " 取出车钥匙解锁了车辆.";

        public const string COM_AddVehFactionOrBusiness = "registercar";
        public const string DESC_AddVehFactionOrBusiness = "[用法] /registercar faction/biz <br>格式说明: <br>如果注册给组织 - /registercar faction 组织ID";

        public const string COM_RemoveVehicleAll = "removeregistercar";
        public const string INFO_RemoveVehicleAllSucces = "{0} 成功取消车辆所有注册给组织和产业的信息.";

        public const string COM_SellVehicleToPlayer = "sellcarto";
        public const string DESC_SellVehicleToPlayer = "[用法] /sellcarto [玩家/系统] [ID] [金额]";
        public const string ERR_NotInSellPoint = "[错误] 您必须在车辆销售点才能将车辆出售给系统.";
        public const string INFO_SellVehicleSendPlayer = "[信息] 您请求将模型为 {0} 的车辆出售给 {1} 以 ${2} 的价格.";
        public const string INFO_SellVehicleSendTarget = "[信息] {0} 请求将模型为 {1} 的车辆以 ${2} 的价格出售给您.<br>接受报价输入 /acpcar";

        public const string COM_SellVehicleAccept = "acpcar";
        public const string ERR_SellVehicleNotFound = "[错误] 无效出售请求.";
        public const string INFO_SellVehicleSuccesPlayer = "[信息] 您成功出售车辆给 {0} 并获得了 ${1}";
        public const string INFO_SellVehicleSuccesTarget = "[信息] 成功购买 {0} 的车辆, 价格: ${1}";

        public const string COM_VehicleTunning = "cmod";
        public const string INFO_VehicleTuningSave = "[服务器] 成功保存车辆改装信息, 您可以输入 /{0} 开始改装.<br> 花费材料: {1}";

        public const string COM_Starttuning = "startcmod";
        public const string ERR_StarttuningNotFound = "[错误] 无效车辆改装信息!";

        public const string COM_VehicleInfo = "cstats";
        #endregion

        #region FactionStrings
        public const string ERR_FNotFound = "[错误] 无效组织.";

        public const string COM_CreateFaction = "createfaction";
        public const string DESC_CreateFaction = "{FFFFFF}/createfaction [选项] [名称]  {DBEEA0}例如: {34B1E0}警察局 <br> {FFFFFF}组织类型: <br> 1 -> {34CF02}合法 <br>{FFFFFF} 2 -> {CF1B02}非法";
        public const string INFO_CreateSucces = "{00E823} 成功创建组织.";

        public const string COM_InviteFaction = "finvite";
        public const string ERR_PlayerInFaction = "[错误] 指定玩家已有组织!";

        public const string COM_AcceptFactionInvite = "faccept";
        public const string COM_KickFaction = "fkick";
        public const string COM_RankFaction = "frank";
        public const string COM_FChat = "f";

        public const string COM_FOnline = "fonline";



        public const string ERR_FTypeError = "输入的组织类型错误.";
        public static string ERR_FCashError = "您没有足够的钱来创建组织.";
        public const string ERR_FInFact = "您已经有组织了!";
        public static string ERR_FChatDisablet = "[错误] 组织OOC频道已经关闭!";
        //PD
        public const string ERR_FDepartmenPermissionError = "[错误] 无权使用此指令.";





        #endregion

        #region BusinessStrings
        public const string ERR_BusinessOwnerNot = "[错误] 此产业不属于您.";
        public const string ERR_BusinessNotFound = "[错误] 无效产业.";

        public const string COM_BusinessLock = "block";
        public const string COM_BuyBusiness = "buybiz";

        public const string COM_SellBusiness = "sellbiz";
        public const string COM_SellBusinessDesc = "[用法] /sellbiz [玩家/系统] <br> {8EDC6C} 直接出售给系统 - /sellbiz system <br> 出售给玩家; <br> /sellbiz 玩家 玩家ID 价格 <br> 例如: /sellbiz 玩家 11 5000";

        public const string COM_GetMoneyInBusiness = "bmoney";
        public const string DESC_GetMoneyInBusiness = "[用法] /bmoney [金额]";
        public const string ERR_MoneyNotEnoughtInBussines = "[错误] 产业金库没有足够的钱";
        public const string INFO_GetMoneyInBusinessSucces = "{0} 从产业金库取出了 ${1}";

        public const string COM_InviteBusiness = "binvite";
        public const string DESC_InviteBusiness = "[用法] /binvite [产业ID] [玩家ID]";
        public const string ERR_PlayerInBusiness = "[错误] {0} 已经有工作的产业了.";
        public const string INFO_InviteTargetInfo = "[信息] {0} 邀请您成为 Ta 的产业员工. <br>输入 /acpbiz 可以接受";
        public const string INFO_InvitePlayerInfo = "[信息] 您发送了产业雇员邀请至 {0}";

        public const string COM_KickBusiness = "bfire";
        public const string DESC_KickBusiness = "[用法] /bfire ID";
        public const string ERR_NotInYourBusiness = " 指定玩家不在您的产业.";
        public const string INFO_KickBusinessSuccesPlayer = "您将 {0} 从您的产业开除了.";
        public const string INFO_KickBusinessSuccesTarget = "{0} 将您从产业开除了!";

        public const string COM_InviteBusinessAccept = "acpbiz";
        public const string ERR_BusinessInviteNotFound = "[错误] 无效产业邀请.";
        public const string INFO_InviteBusinessAccepted = "[信息] {0} 接受了您的产业雇员邀请.";

        public const string COM_EditEntranceBusiness = "bfee";
        public const string DESC_EditEntranceBusiness = "[用法] /bfee [金额]";
        //Busines Jobs
        public const string COM_BuyStock = "buystock";

        public const string COM_LoadStock = "loadstock";

        public const string COM_GetStock = "getstock";
        public const string COM_ShowStock = "showstock";

        // Hospital
        public const string COM_Inspection = "insp";
        public const string DESC_Inspection = "[用法] /insp [ID]";
        public const string INFO_NoDisease = "指定玩家是健康的.";
        public const string INFO_Diseases = "[H] {0} | [情况] {1}";

        public const string COM_Heal = "heal";
        public const string DESC_Heal = "[用法] /heal [ID]";
        public const string INFO_HealWithDisease = "[信息] {0} 被治疗了 ([病症]: {1})";
        public const string INFO_HealWithDiseaseOver = "[信息] {0} 的病症被治好了.";
        public const string INFO_Heal_t = "[信息] {0} 将您治疗了.";
        public const string INFO_Heal_p = "[信息] 您将 {0} 治疗了.";
        #endregion

        #region House Strings
        public const string ERR_NotNearHouse = "[错误] 附近没有房屋";
        public const string ERR_HouseOwnerNotYou = "[错误] 此房屋不属于您.";
        public const string ERR_NotOwnHouseKey = "[错误] 您没有此房屋的钥匙.";

        public const string COM_BuyHouse = "buyhouse";

        public const string COM_LockHouse = "hlock";

        public const string COM_EditHouse = "edithouse";
        public const string DESC_EditHouse = "[用法] /edithouse [选项] [数值(如果有)]<br>可用选项;<br>rentable : 是否出租开关.<br>rentprice : 出租价格(如果您有租户则不能更改价格.)<br>kick: 清空租户.";
        public const string ERR_RentOwnerHaveCant = "[错误] 因为您的房屋已有租户, 无法修改租金.";
        public const string INFO_RentOwnerRemove = "[信息] 您清空了 {0} 的租房.";
        #endregion

        #region PDStrings
        public const string ERR_PlayerNotInPd = "[错误] 无权操作!";
        public const string ERR_ReasoneNotBeNull = "[错误] 请输入有效的原因或说明.";
        public const string ERR_PD_NeedDuty = "[错误] 请先执勤, /duty.";
        public const string ERR_PD_NeedTeam = "[错误] 您不在单位中, 创建单位输入 /unit 搭档ID 单位名称.";
        public const string ERR_TargetNotInPD = "[错误] {0} 不是执法组织成员";

        public const string COM_PD_Duty = "duty";

        public const string COM_PD_Radio = "t";
        public const string DESC_PD_Radio = "[用法] /t [文本]";

        public const string COM_PD_RadioFreq = "tk";
        public const string ERR_PD_RadioWrong = "[用法] /tk [频道号码]<br>可用频道号码;<br>1-99";
        public const string INFO_PD_ReadioFreqSucces = "[信息] 成功切换频道至 {0}";

        public const string COM_HelpReq = "hr";
        public const string DESC_HelpReq = "[用法] /hr [类型]<br> 可用类型;<br>1 : PD支援请求<br>2: EMS支援请求<br>3: LSFD支援请求";

        public const string COM_HelpReqAccept = "hra";
        public const string DESC_HelpReqAcccept = "[用法] /hra [ID]";

        public const string COM_CreateTeam = "unit";
        public const string DESC_CreateTeam = "[用法] /unit 搭档ID 单位名称 (例如: 1ADAM5)";
        public const string ERR_PlayerInTeam = "[错误] 您已在单位中了!";
        public const string ERR_TargetInTeam = "[错误] {0} 已经有单位了.";
        public const string INFO_CreateTeamPlayerSucces = "[信息] 成功创建单位 {0}";
        public const string INFO_CreateTeamTargetSucces = "[信息] {0} 已将您加入至单位 {1}";

        public const string COM_PDDestroyTeam = "deunit";
        public const string ERR_PDTeamNotFound = "[错误] 无效单位.";
        public const string INFO_PDDestroyTeam = "[信息] 成功撤销单位.";

        public const string COM_PDTeamList = "unitlist";

        public const string COM_PD_Fine = "fine";
        public const string DESC_PD_Fine = "[用法] /fine [veh/man/dl] [车牌号/ID] [金额] [说明]";
        public const string FINE_InfoTarget = "{0} 给您开了一张罚单. <br>金额: {1}<br>说明: {2}";
        public const string FINE_InfoSender = "您给 {0} 开了一张罚单.<br>金额: {1}<br>说明: {2}";
        public const string ERR_FineValue = "[错误] 无效金额.";

        public const string COM_PD_RoadBlock = "rb";

        public const string COM_PD_DeleteRoadBlock = "drb";
        public const string DESC_PD_DeleteRoadBlock = "[用法] /drb [路障ID]";

        public const string COM_PD_Equipment = "equip";
        public const string DESC_PD_Equipment = "[用法] /equip [装备ID]<br> 可用装备;<br> 1: 巡逻装备";
        public const string ERR_PD_EquipmentWrong = "[错误] 无效装备.";
        public const string ERR_PD_EquiptmentPos = "[错误] 您必须在更衣室或靠近警车才能使用此命令.";

        public const string COM_PD_CuffPlayer = "cu";
        public const string DESC_PD_CuffPlayer = "[用法] /cu [ID]";

        public const string COM_DepChat = "dt";
        public const string DESC_DepChat = "[用法] /dt [文本]";

        public const string EER_JailTimeValue = "[错误] 无效关押时间.";
        public const string Jail_NotNearPos = "[错误] 您必须在监狱区才能执行此操作.";
        //------

        public const string Jail_InfoSender = "您关押了 {0}<br>时间: {1}<br>原因: {2}";
        public const string Jail_InfoTarget = "{0} 将您关押了.<br>时间: {1}<br>原因: {2}";

        public const string COM_PD_DestroyDoor = "bbd";
        public const string ERR_PD_DestroyDoorNotFound = "[错误] 无效产业.";
        public const string ERR_PD_DestroyDoorAlreadyUnlocked = "[错误] 此产业的门是开的.";

        public const string COM_FriskTarget = "frisk";
        public const string DESC_FriskTarget = "[用法] /frisk [ID] && /frisk acp/dec";

        public const string COM_Requisition = "disarm";
        public const string DESC_Requisition = "[用法] /disarm [ID] [物品ID]";

        public const string COM_TempJail = "arrest";
        public const string DESC_TempJail = "[用法] /arrest [ID]";

        public const string COM_TempJailOver = "arrestoff";
        public const string DESC_TempJailOver = "[用法] /arrestoff [id]";

        public const string COM_BeanBag = "beanbag";

        public const string COM_PDnearRadio = "yt";
        public const string DESC_PDnearRadio = "[用法] /yt [文本]";

        public const string COM_PDOperator = "o";
        public const string DESC_PDOperator = "[用法] /o [文本]";

        public const string COM_PDRadar = "radar";

        public const string COM_LeaveHelpReq = "dhr";

        public const string COM_PD_Jail = "jail";
        public const string DESC_PD_Jail = "[用法] /jail [ID] [时间(分钟)] [原因]";

        public const string COM_PD_CreateWanted = "apb";
        public const string DESC_PD_CreateWanted = "[用法] /apb [玩家ID] [原因]";
        #endregion

        #region FD Strings
        public const string COM_FDDuty = "fduty";

        public const string COM_FDEquipment = "fequip";
        #endregion

        #region News Strings
        public const string COM_CreateBroadCast = "cbc";
        public const string DESC_CreateBroadCast = "[用法] /cbc [标题]";
        public const string INFO_CreateBroadCast = "[{0}] <i class='fal fa-podcast'></i> 支持人 {1} 上线了.<br>标题: ";

        public const string COM_BroadCastChat = "bc";
        public const string DESC_BroadCastChat = "[用法] /bc [文本]";
        public const string ERR_BroadCastChatNotIn = "[错误] 您不在直播间, 如果您是新闻成员, 您可以创建直播, 如果您不是新闻成员, 您只能被邀请";
        public const string INFO_BroadCastChatSendMessage = "[{0}] {1} <i class='fad fa-microphone-stand'></i>: ";

        public const string COM_Adversiment = "ad";
        public const string DESC_Adversiment = "[用法] /ad [文本]<br>如果您有电话号码, 系统会自动标注联系方式.";
        #endregion

        #region Other Commands
        public const string COM_FlipCoin = "coin";
        public const string COM_Dice6 = "dice";

        public const string COM_GiveMoney = "pay";
        public const string DESC_GiveMoney = "[用法] /pay ID 金额";

        public const string COM_ShowNearPlates = "plates";

        public const string COM_Id = "id";
        public const string DESC_Id = "[用法] /id [玩家名称]";

        public const string COM_DateTime = "time";

        public const string COM_BuyClothes = "buyclothes";
        public const string ERR_NotNearClothesShop = "[错误] 附近没有服装购买点.";

        public const string COM_CrateEvent = "box";
        public static string DESC_CrateEvent = "[用法] /box pickup/drop<br> 货箱拾取: /box pickup [id]<br> 货箱丢弃: /box drop";

        public const string COM_CharacterStatus = "stats";

        public const string COM_ShowBusiness = "mybiz";

        public const string COM_WalkingStyle = "walkstyle";

        public const string COM_ShowAdmins = "admins";
        public const string COM_ShowHelpers = "helpers";

        public const string COM_ShowIdentiy = "showid";
        public const string DESC_ShowIdentiy = "[用法] /showid [ID]";
        public const string DESC_ForceDown = "[用法] /fod [ID]";

        public const string COM_ChangeArms = "torso";
        public const string DESC_ChangeArms = "[用法] /torso [ID] // ID: 1-76";

        public const string COM_ClosePM = "pmoff";
        public const string COM_CloseAdversiment = "adoff";

        public const string COM_GiveKeys = "givekey";
        public const string DESC_GiveKeys = "[用法] /givekey [玩家ID] [车辆ID] [临时/永久]<br>临时密钥一直保留到服务器重新启动.";

        public const string COM_HairModel = "dohair";

        public const string COM_ShowKeys = "mykeys";

        #endregion

        #region Inventroy Strings
        public const string ERR_TargetWeight = "[错误] 指定玩家的库存超重了.";
        public const string ERR_TargetSlot = "[错误] 指定玩家的库存没有槽位了.";

        public const string INFO_PlayerGiveYou = "{0} 给予您物品 {1}";
        public const string INFO_PlayerGiveSucces = "您给予 {0} 物品 {1}";

        public const string ERR_SmokeLighterNotFound = "[信息] 您需要打火机.";
        public const string ERR_LighterUsageAlone = "[信息] 打火机无法直接使用, 打火机需要保留在库存来使用香烟.";

        public const string INFO_ShowLicense = "{0} 的执照;<br>类型: {1}<br>惩罚: {2}";
        public const string INFO_WheelRepaired = "成功修理车辆 {0} 的轮胎.";
        #endregion

        #region Bag Strings
        public const string COM_BagMain = "bag";
        #endregion

        #region Bank Strings
        public const string COM_Bank = "bank";
        #endregion
    }

    public class ServerEmotes
    {
        public static string PD_FineEmote = " 开具了一张针对 {0} 的罚单.";
        public static string PD_MegaphoneEmote = " 将手放在扩音器的闩锁上并讲话.";

        public static string EMOTE_PDDestroyDoor = " 试图破门.";
        public static string EMOTE_PDDestroyDoorDo = "门被破开了.";

        public static string EMOTE_GiveMoney = " 掏出一些现金并递给了 {0}.";

        // House 
        public static string EMOTE_LockHouse = " 掏出钥匙并";

        // Kimlik
        public static string EMOTE_ShowIdentiyCard = " 拿出身份证并向 {0} 出示.";

        #region Inventory Emotes
        public const string EMOTE_Smoke = " 取出一根香烟并用打火机点燃.";
        public const string EMOTE_ShowLicense = " 拿出执照并向 {0} 出示.";
        public const string EMOTE_FillFuel = " 正在给车加油.";

        public const string EMOTE_RepairWheel = " 用双手牢牢固定住轮胎并将轮胎放在卡钳上固定.";
        public const string EMOTEDO_RepairWheelDone = "可以看到车辆 {0} 的轮胎已经被修好了.";
        #endregion
    }

    public class ServerGlobalValues
    {

        public static int PayDayPrice = 250;
        public static int PayDayExp = 1;
        public static int FishingPrice = 20;
        /*Faction Types
                 * 1: Legal | 2: İllegal
                 * 3: PD | 4: EMS | 5: LSFD
                 * 6: News
                */
        public const int fType_Legal = 1;
        public const int fType_illegal = 2;
        public const int fType_PD = 3;
        public const int fType_FD = 4;
        public const int fType_Goverment = 5;
        public const int fType_News = 6;
        public const int fType_Racers = 7;
        public const int fType_MD = 8;
        public const int fType_Taxi = 9;
        public const int fType_Security = 10;


        #region TextLabel Texts
        public const string createFactionLabelText = "组织创立点~n~指令: ~g~/createfaction~n~~w~条件: ~g~$12000";
        #endregion
        #region Server Positions
        public static Position createFactionPos = new Position(876.778f, -2043.4681f, 31.571411f);

        public static Position endJailPos = new Position(-1095.455f, -802.2066f, 18.664429f);

        public static Position TaxPayPos = new Position(-1285.0813f, -566.33405f, 31.706177f);

        public static Position pdEquiptmenPos = new Position(473.1296f, -986.9011f, 25.724487f);
        public static Position pdDutyPos = new Position(-1093f, -832f, 14f);

        public static Position pdCanJailPos = new Position(461.61758f, -989.2088f, 24.898926f);
        public static Position pdSendJailPos = new Position(1720, 2510, 45);
        public static Position pdEndJailPos = new Position(424.42f, -980.8483f, 30.695f);

        public static Position pdSendTempJailPos = new Position(-1071.1252f, -823.2f, 5.4710693f);
        public static Position pdTempJailPos = new Position(1089.3099f, -826.04834f, 6.4710693f); //  -1089.3099f, -826.12744f, 5,4710693f

        public static Position vehicleSellPoint = new Position(-230.46594f, -1169.6439f, 22.843262f);      // Kordinatlar düzenlecek - Araç satım için

        public static Position buyStockPosition = new Position(611.94727f, -3083.499f, 6.060791f);

        public static Position adversimentPosition = new Position(-589.8725f, -922.5626f, -26.156006f);

        public static Position inwPosition = new Position(-1213.9385f, -770.8615f, 17.939941f);
        #endregion

        //Busines Jobs, point etc.
        public const int FactionCreatePrice = 12000;
        public const int StockPrice = 2400;
        public const int GasPrice = 4;
        public const int CarRentPrice = 5;

        //Jobs
        public const int JOB_RentalCar = 1; // Vehicles Only
        public const int JOB_Cargo = 2;
        public const int JOB_Trucker = 3;
        public const int JOB_Taxi = 4;
        public const int JOB_Miner = 6;
        //Business Types
        public const int rentalCar = 1;
        public const int stockBusiness = 2;
        public const int mechanicBusiness = 3;
        public const int hospitalBusiness = 4;
        public const int marketBusiness = 5;
        public const int farmBusiness = 6;
        public const int carWashBusiness = 7;
        public const int casinoBusiness = 9;
        public const int dinnerBusiness = 10;
        public const int paintballBusiness = 11;
        public const int SecurityBusiness = 12;

        // SErver Settings
        public static bool serverCanTax = true;
    }

    public class ServerAnimations
    {
        public static string[] putObject = new string[2] { "anim@heists@box_carry@", "idle" };
        public static string[] searchJunkyard = new string[2] { "amb@world_human_gardener_plant@male@idle_a", "idle_a" };
        public static string[] Crate_TakeGround = new string[2] { "anim@heists@load_box", "lift_box" };
        public static string[] Crate_DropGround = new string[2] { "anim@heists@money_grab@briefcase", "put_down_case" };
    }
}

