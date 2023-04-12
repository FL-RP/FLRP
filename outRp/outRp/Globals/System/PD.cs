using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Core;
using outRp.Models;
using outRp.OtherSystem.NativeUi;
using outRp.OtherSystem.Textlabels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;
using static outRp.Globals.GlobalEvents;

namespace outRp.Globals.System
{
    /*
     *  Faction Types =  3 : PD | 4 : EMS | 5 : Goverment | 6 : Logistic |  
     * 
     * 
     */
    public class FineModel
    {
        public int sender { get; set; }
        public int target { get; set; }
        public int fine { get; set; }
        public string reason { get; set; }
    }
    public class JailModel
    {
        public int ID { get; set; }
        public int target { get; set; }
        public int sender { get; set; }
        public int time { get; set; }
        public string reason { get; set; }
        public Task<int> Create() => Database.DatabaseMain.CreateJail(this);
    }
    public class PDTeamModel
    {
        public string name { get; set; }
        public int leaderId { get; set; }
        public int memberId { get; set; }
    }
    public class HelpReqModel
    {
        public int helpClientID { get; set; }
        public int helpClientSqlID { get; set; }
        public int helpCar { get; set; }
        public string Reason { get; set; } = "无";
        public List<helpClients> inHelp { get; set; } = new List<helpClients>();

        public class helpClients
        {
            public int ClientSqlID { get; set; }
            public int ClientID { get; set; }
        }
    }
    public class RoadBlockModel
    {
        public int owner { get; set; }
        public PlayerLabel textlbl { get; set; }
        public OtherSystem.Textlabels.LProp prop { get; set; }
    }


    public class PD : IScript
    {
        //SETUP PD
        public static List<PDTeamModel> pdTeams = new List<PDTeamModel>();
        public static List<HelpReqModel> pdHelpReqs = new List<HelpReqModel>();
        public static List<RoadBlockModel> pdRoadBlocks = new List<RoadBlockModel>();

        public static Position PDRevivePos = new Position(464.66373f, -1013.44617f, 28.06f);
        public static Position PDRevivePos2 = new Position(368.1758f, -1602.0791f, 29.279907f);
        public static Position PDCallOfficerPos = new Position(441.2044f, -981.1912f, 30.678f);
        public static void LoadPDSystems()
        {
            TextLabelStreamer.Create("~b~[治疗点]~n~~g~/~w~pdheal", PDRevivePos, font: 0, streamRange: 5);
            TextLabelStreamer.Create("~b~[治疗点]~n~~g~/~w~pdheal", PDRevivePos2, font: 0, streamRange: 5);
            // new Position(428, -962, 29)
            TextLabelStreamer.Create("~b~[车辆修理点]~n~~g~/~w~pdfixcar", new Position(428, -962, 29), font: 0, streamRange: 25);
            TextLabelStreamer.Create("~b~[警员呼叫]~n~~g~/~w~pdcall", PDCallOfficerPos, font: 0, streamRange: 5);
            Alt.Log("加载 PD 系统.");
        }
        //-------------
        public static async Task<bool> CheckPlayerInPd(PlayerModel player)
        {
            bool t = false;
            //CharacterModel p = GlobalEvents.PlayerGetData(player);
            List<int> a = await Database.DatabaseMain.GetPDFactionIds();

            foreach (int fId in a)
            {
                if (fId == player.factionId)
                {
                    if (player.HasData(EntityData.PlayerEntityData.PDDuty))
                    {
                        t = true; break;
                    }
                }
            }
            return t;
        }

        public static async Task CreatePDJail(PlayerModel player, int? targetId, int? time, string reason)
        {
            //TODO -> Jail Bölgesi eklenecek.
            if (player.Position.Distance(ServerGlobalValues.pdSendJailPos) > 4f) { MainChat.SendErrorChat(player, CONSTANT.Jail_NotNearPos); return; }
            bool PdCheck = await CheckPlayerInPd(player);
            if (PdCheck == false) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotInPd); return; }
            if (targetId == null) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotFound); return; }
            PlayerModel target = GlobalEvents.GetPlayerFromId((int)targetId);
            if (target == null) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotFound); return; }
            if (time <= 0) { MainChat.SendErrorChat(player, CONSTANT.EER_JailTimeValue); return; }

            //CharacterModel pChar = GlobalEvents.PlayerGetData(player);
            //CharacterModel tChar = GlobalEvents.PlayerGetData(target);
            JailModel j = new JailModel();
            j.sender = player.sqlID;
            j.target = target.sqlID;
            j.reason = reason;
            j.time = (int)time;
            j.ID = await j.Create();

            target.jailTime = (int)time;
            await target.updateSql();

            string jailSenderInfo = string.Format(CONSTANT.Jail_InfoSender, target.characterName.Replace("_", " "), j.time, j.reason);
            string jailTargetInfo = string.Format(CONSTANT.Jail_InfoTarget, player.characterName.Replace("_", " "), j.time, j.reason);

            MainChat.SendErrorChat(player, jailSenderInfo);
            MainChat.SendErrorChat(target, jailTargetInfo);
            return;
        }

        [Command(CONSTANT.COM_PD_Duty)]
        public async Task COM_PDDuty(PlayerModel player)
        {
            List<int> a = await Database.DatabaseMain.GetPDFactionIds();

            foreach (int fId in a)
            {
                if (fId == player.factionId)
                {
                    if (player.HasData(EntityData.PlayerEntityData.PDDuty))
                    {
                        player.DeleteData(EntityData.PlayerEntityData.PDDuty);
                        await GlobalEvents.FreezePlayerClothes(player, true);
                        player.SendChatMessage("您结束了执勤.");
                        GlobalEvents.ClearPlayerTag(player);
                        player.RemoveAllWeapons();
                        player.EmitLocked("PD:CloseCars");
                        player.Armor = 0;
                        return;
                    }
                    else
                    {
                        VehModel pdVeh = Vehicle.VehicleMain.getNearVehFromPlayer(player);
                        if (player.Position.Distance(ServerGlobalValues.pdEquiptmenPos) > 5f)
                        {
                            if (pdVeh == null || pdVeh.factionId != player.factionId)
                            {
                                MainChat.SendErrorChat(player, CONSTANT.ERR_PD_EquiptmentPos); return;
                            }
                        }
                    }

                    FactionModel f = await Database.DatabaseMain.GetFactionInfo(player.factionId);
                    if (f == null) { MainChat.SendErrorChat(player, "[错误] 无效组织!"); return; }
                    //if(player.Position.Distance(ServerGlobalValues.pdDutyPos) > 5) { MainChat.SendErrorChat(player, "[错误] Bu komutu kullanabilmek için iş başı odasında olmalısınız."); return; }
                    List<GuiMenu> gMenu = new List<GuiMenu>();


                    foreach (DutyClothes c in f.settings.clothes)
                    {
                        GuiMenu clothes1 = new GuiMenu { name = c.name, triger = "PD:Duty", value = c.name };
                        gMenu.Add(clothes1);
                    }

                    GuiMenu close = GuiEvents.closeItem;
                    gMenu.Add(close);

                    Gui y = new Gui()
                    {
                        image = "https://pic.imgdb.cn/files/65292/A68756B9970FC7AE517EE725CDD05E6CE3FCF8FA",
                        guiMenu = gMenu,
                        color = "#00AAFF"
                    };
                    y.Send(player);
                    player.lscSetData(EntityData.PlayerEntityData.PDDuty, true);
                    await GlobalEvents.FreezePlayerClothes(player, false);
                    player.SendChatMessage("您开始了执勤 - 请自行前往服装店购买制服搭配 - 制服系统待开发 - 使用 /equip 查看装备");
                    player.SendChatMessage("/fcar 刷出组织车辆 - /fcarlist 查看已刷组织车辆 - /rfcar 删除已刷车辆 - /feditcar 编辑车辆颜色MOD等 - /feditplate 编辑车牌号");
                    FactionModel faction = await Database.DatabaseMain.GetFactionInfo(player.factionId);
                    //string rank = Factions.Faction.GetFactionRankString(player.factionRank, f);
                    //GlobalEvents.SetPlayerTag(player, "~b~" + rank, true);
                    // player.EmitLocked("PD:ShowCars", player.factionId);
                    return;
                }
            }
        }
        [AsyncClientEvent("PD:Duty")]
        public static async Task PD_Duty_SelectClothes(PlayerModel p, string value)
        {
            if (p.factionId <= 0) { MainChat.SendErrorChat(p, "[错误] 无效组织!"); return; }
            FactionModel f = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            if (f == null) { MainChat.SendErrorChat(p, "[错误] 无效组织!"); return; }

            DutyClothes x = f.settings.clothes.Find(x => x.name == value);
            if (x == null) { MainChat.SendErrorChat(p, "[错误] 无效制服!"); return; }

            p.EmitLocked("GUI:Close");

            foreach (DutyClothes.Clothes cl in x.cloth)
            {
                SetClothes(p, cl.comp, cl.ID, cl.texture);
                //p.SetClothes((byte)cl.comp, (ushort)cl.ID, (byte)cl.texture, 2);
            }
            foreach (DutyClothes.Props cp in x.prop)
            {
                p.EmitLocked("SetClothesProps", cp.prop, cp.ID, cp.texture);
                //p.SetProps((byte)cp.prop, (ushort)cp.ID, (byte)cp.texture);
            }
        }

        [Command("pdheal")]
        public async Task COM_RevivePD(PlayerModel p)
        {
            if (!await CheckPlayerInPd(p)) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            if (p.Position.Distance(PDRevivePos) > 5 && p.Position.Distance(PDRevivePos2) > 5) { MainChat.SendErrorChat(p, "[错误] 您不在治疗点."); return; }

            p.Spawn(p.Position, 0);
            p.hp = 1000;
            p.maxHp = 1000;
            p.injured.Injured = false;
            p.injured.isDead = false;
            GlobalEvents.UpdateInjured(p);


            MainChat.SendInfoChat(p, "[?] 成功治疗!");
            return;
        }


        [Command(CONSTANT.COM_PD_Fine)]
        public async Task COM_CreatePDFine(PlayerModel player, params string[] args)
        {
            bool pdCheck = await CheckPlayerInPd(player);
            if (pdCheck == false) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotInPd); return; }
            if (args.Length <= 3) { MainChat.SendInfoChat(player, CONSTANT.DESC_PD_Fine); return; }
            int Fine = Int32.Parse(args[2]);
            if (Fine <= 0 || Fine > 10000) { MainChat.SendErrorChat(player, "[错误] 金额范围为1-10000."); return; }
            switch (args[0])
            {
                case "veh":
                    VehModel v = null;
                    foreach (VehModel x in Alt.GetAllVehicles())
                    {
                        if (x.NumberplateText == args[1]) { v = x; break; }
                    }
                    if (v == null) { MainChat.SendErrorChat(player, "[错误] 无效车辆"); return; }
                    //if (v.Position.Distance(player.Position) > 10) { MainChat.SendErrorChat(player, "[错误] Araca yeterince yakın değilsiniz."); return; }
                    v.fine += Fine;
                    v.settings.fines.Add("[罚单] 日期:" + DateTime.Now.ToString("g") + " | 金额: $" + Fine + " | 说明: " + String.Join(" ", args[3..]));
                    v.Update();
                    MainChat.EmoteMe(player, " 将一张罚单张贴到了车上.");
                    MainChat.EmoteDoAlternative(player, "金额: " + Fine + " | 原因: " + string.Join(" ", args[3..]));
                    break;

                case "man":
                    int targetSqlId = Int32.Parse(args[1]);
                    PlayerModel t = GlobalEvents.GetPlayerFromSqlID(targetSqlId);
                    if (t == null || t.Position.Distance(player.Position) > 5) { MainChat.SendErrorChat(player, "[错误] 无效玩家或离您太远."); return; }
                    CharacterSettings targetSettings = JsonConvert.DeserializeObject<CharacterSettings>(t.settings);
                    PlayerFineModel nFine = new PlayerFineModel()
                    {
                        sender = player.sqlID,
                        finePrice = Fine,
                        reason = string.Join(" ", args[3..]),
                    };
                    targetSettings.fines.Add(nFine);
                    t.settings = JsonConvert.SerializeObject(targetSettings);
                    await t.updateSql();
                    MainChat.EmoteMe(player, " 将一张罚单递给了 " + t.fakeName.Replace("_", " "));
                    MainChat.EmoteDoAlternative(player, "金额: " + Fine + " | 原因: " + string.Join(" ", args[3..]));
                    OtherSystem.LSCsystems.MDCEvents.MDC_CreateRecord_Fine(t, player, Fine, string.Join(" ", args[3..]));
                    break;

                case "dl":
                    int licenseId; bool isOk = Int32.TryParse(args[1], out licenseId);
                    if (!isOk) { MainChat.SendInfoChat(player, CONSTANT.DESC_PD_Fine); return; }
                    PlayerModelInfo dT = await Database.DatabaseMain.getCharacterInfo(licenseId);
                    if (dT == null) { MainChat.SendErrorChat(player, "[错误] 无效驾照."); return; }
                    CharacterSettings dTset = JsonConvert.DeserializeObject<CharacterSettings>(dT.settings);
                    dTset.driverLicense.finePoint += Fine;
                    PlayerModel checkOnline = GlobalEvents.GetPlayerFromSqlID(dT.sqlID);
                    if (checkOnline == null)
                    {
                        dT.settings = JsonConvert.SerializeObject(dTset);
                        dT.updateSql();
                    }
                    else
                    {
                        checkOnline.settings = JsonConvert.SerializeObject(dTset);
                        await checkOnline.updateSql();
                    }
                    MainChat.EmoteMe(player, " 开具了一张针对驾照 " + licenseId.ToString() + " 的罚单.");
                    MainChat.EmoteDoAlternative(player, "金额: " + Fine + " | 原因: " + string.Join(" ", args[3..]));
                    break;

                case "biz":
                    // [Kullanım] /cezakes [araç/kişi/ehliyet] [plaka/ID] [Miktar] [Açıklama]
                    if (!Int32.TryParse(args[1], out int bizId)) { MainChat.SendInfoChat(player, CONSTANT.DESC_PD_Fine); return; }
                    var biz = await Props.Business.getBusinessById(bizId);
                    if (biz.Item1 == null) { MainChat.SendErrorChat(player, "[错误] 无效产业或附近没有产业."); return; }
                    if (biz.Item1.position.Distance(player.Position) > 20) { MainChat.SendErrorChat(player, "[错误] 离产业太远."); return; }

                    biz.Item1.settings.TotalTax += Fine;
                    await biz.Item1.Update(biz.Item2, biz.Item3);

                    var tBiz = GlobalEvents.GetPlayerFromSqlID(biz.Item1.ownerId);
                    if (tBiz != null)
                    {
                        MainChat.SendInfoChat(tBiz, "[?] 您的产业 " + bizId + " 被罚款了, 金额: " + Fine + " 原因: " + String.Join(" ", args[3..]));
                    }

                    MainChat.EmoteMe(player, " 开具了一张针对产业 " + bizId.ToString() + " 的罚单并张贴到产业门上.");
                    MainChat.EmoteDoAlternative(player, "金额: " + Fine + " | 原因: " + string.Join(" ", args[3..]));

                    break;

                case "house":
                    // [Kullanım] /cezakes [araç/kişi/ehliyet] [plaka/ID] [Miktar] [Açıklama]
                    if (!Int32.TryParse(args[1], out int hId)) { MainChat.SendInfoChat(player, CONSTANT.DESC_PD_Fine); return; }
                    var house = await Props.Business.getBusinessById(hId);
                    if (house.Item1 == null) { MainChat.SendErrorChat(player, "[错误] 无效房屋或附近没有房屋."); return; }
                    if (house.Item1.position.Distance(player.Position) > 20) { MainChat.SendErrorChat(player, "[错误] 离房屋太远."); return; }

                    house.Item1.settings.TotalTax += Fine;
                    await house.Item1.Update(house.Item2, house.Item3);

                    var tHouse = GlobalEvents.GetPlayerFromSqlID(house.Item1.ownerId);
                    if (tHouse != null)
                    {
                        MainChat.SendInfoChat(tHouse, "[?] 您的房屋 " + hId + " 被罚款了, 金额: " + Fine + " 原因: " + String.Join(" ", args[3..]));
                    }

                    MainChat.EmoteMe(player, " 开具了一张针对房屋 " + hId.ToString() + " 的罚单并张贴到房屋门上.");
                    MainChat.EmoteDoAlternative(player, "金额: " + Fine + " | 原因: " + string.Join(" ", args[3..]));

                    break;

                default:
                    MainChat.SendInfoChat(player, CONSTANT.DESC_PD_Fine);
                    break;
            }
        }

        [Command(CONSTANT.COM_PD_RoadBlock)]
        public async Task COM_CreatePDRoadBlock(PlayerModel player)
        {
            bool pdCheck = await System.PD.CheckPlayerInPd(player);
            if (pdCheck == false)
            {
                if (await FD.CheckPlayerInFD(player) == false)
                {
                    MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotInPd); return;
                }
            }

            List<GuiMenu> gMenu = new List<GuiMenu>();
            GuiMenu block1 = new GuiMenu { name = "大型金属路障(1)", triger = "RoadBlock:Selected", value = "p_barriercrash_01_s" };
            GuiMenu block2 = new GuiMenu { name = "小型金属路障(2)", triger = "RoadBlock:Selected", value = "ba_prop_battle_barrier_02a" };
            GuiMenu block3 = new GuiMenu { name = "小型道路反光镜(3)", triger = "RoadBlock:Selected", value = "prop_barrier_work01b" };
            GuiMenu block4 = new GuiMenu { name = "天道路障(4)", triger = "RoadBlock:Selected", value = "prop_barrier_work01a" };
            GuiMenu block5 = new GuiMenu { name = "大路障(5)", triger = "RoadBlock:Selected", value = "prop_barrier_work04a" };
            GuiMenu block6 = new GuiMenu { name = "加强警用路障(6)", triger = "RoadBlock:Selected", value = "prop_barrier_work05" };
            GuiMenu block7 = new GuiMenu { name = "交通警示牌(7)", triger = "RoadBlock:Selected", value = "prop_barrier_work06a" };
            GuiMenu block8 = new GuiMenu { name = "黑布路障(8)", triger = "RoadBlock:Selected", value = "prop_ld_barrier_01" };
            GuiMenu block9 = new GuiMenu { name = "反光路障(9)", triger = "RoadBlock:Selected", value = "prop_mp_arrow_barrier_01" };
            GuiMenu block10 = new GuiMenu { name = "大交通路障(10)", triger = "RoadBlock:Selected", value = "prop_mp_barrier_02b" };
            GuiMenu block11 = new GuiMenu { name = "交通锥(11)", triger = "RoadBlock:Selected", value = "prop_roadcone02a" };
            GuiMenu block12 = new GuiMenu { name = "反光路标(12)", triger = "RoadBlock:Selected", value = "prop_air_conelight" };
            GuiMenu block13 = new GuiMenu { name = "治安官锥(13)", triger = "RoadBlock:Selected", value = "prop_mp_cone_04" };
            GuiMenu block14 = new GuiMenu { name = "梯子(14)", triger = "RoadBlock:Selected", value = "prop_byard_ladder01" };
            GuiMenu block15 = new GuiMenu { name = "尖刺陷阱(15)", triger = "RoadBlock:Selected", value = "xs_prop_arena_spikes_02a" };

            gMenu.Add(block1); gMenu.Add(block2); gMenu.Add(block3); gMenu.Add(block4); gMenu.Add(block5); gMenu.Add(block6); gMenu.Add(block7); gMenu.Add(block8); gMenu.Add(block9); gMenu.Add(block10);
            gMenu.Add(block11); gMenu.Add(block12); gMenu.Add(block13); gMenu.Add(block14); gMenu.Add(block15);

            if (player.HasData(EntityData.PlayerEntityData.FDDuty))
            {
                GuiMenu block16 = new GuiMenu { name = "B 型胶管(16)", triger = "RoadBlock:Selected", value = "prop_roofpipe_06" };
                GuiMenu block17 = new GuiMenu { name = "C 型胶管(17)", triger = "RoadBlock:Selected", value = "hei_prop_heist_hose_01" };
                GuiMenu block18 = new GuiMenu { name = "通风扇(18)", triger = "RoadBlock:Selected", value = "v_res_fa_fan" };
                GuiMenu block19 = new GuiMenu { name = "卡车梯子(19)", triger = "RoadBlock:Selected", value = "port_xr_stairs_01" };
                GuiMenu block20 = new GuiMenu { name = "钩梯(20)", triger = "RoadBlock:Selected", value = "hw1_06_ldr_05" };
                GuiMenu block21 = new GuiMenu { name = "绳索(21)", triger = "RoadBlock:Selected", value = "p_cs_15m_rope_s" };
                GuiMenu block22 = new GuiMenu { name = "便携式电源装置(22)", triger = "RoadBlock:Selected", value = "prop_generator_01a" };
                GuiMenu block23 = new GuiMenu { name = "起重桅杆(23)", triger = "RoadBlock:Selected", value = "xs_prop_x18_axel_stand_01a" };
                GuiMenu block24 = new GuiMenu { name = "后勤支持(24)", triger = "RoadBlock:Selected", value = "xs_prop_x18_strut_compressor_01a" };
                GuiMenu block25 = new GuiMenu { name = "透气垫(25)", triger = "RoadBlock:Selected", value = "xs_prop_arena_flipper_small_01a_wl" };
                GuiMenu block26 = new GuiMenu { name = "黄色标记(26)", triger = "RoadBlock:Selected", value = "v_ind_cf_bollard" };
                GuiMenu block27 = new GuiMenu { name = "镇定机(27)", triger = "RoadBlock:Selected", value = "prop_carjack" };
                GuiMenu block28 = new GuiMenu { name = "稳定器柱(28)", triger = "RoadBlock:Selected", value = "prop_engine_hoist" };

                gMenu.Add(block16); gMenu.Add(block17); gMenu.Add(block18); gMenu.Add(block19); gMenu.Add(block20); gMenu.Add(block21); gMenu.Add(block22); gMenu.Add(block23); gMenu.Add(block24); gMenu.Add(block25); gMenu.Add(block26); gMenu.Add(block27); gMenu.Add(block28);
            }





            GuiMenu close = GuiEvents.closeItem;
            gMenu.Add(close);
            Gui y = new Gui()
            {
                image = "https://www.upload.ee/image/12254339/1.png",
                guiMenu = gMenu,
                color = "#00AAFF"
            };
            y.Send(player);
        }

        [AsyncClientEvent("RoadBlock:Selected")]
        public static void Roadblock_Selected(PlayerModel p, string value)
        {
            GuiEvents.GUIMenu_Close(p);
            GlobalEvents.ShowObjectPlacement(p, value, "RoadBlock:Place");
        }
        [AsyncClientEvent("RoadBlock:Place")]
        public static void Roadblock_Place(PlayerModel p, string rot, string pos, string model)
        {
            RoadBlockModel block = new RoadBlockModel();
            Vector3 position = JsonConvert.DeserializeObject<Vector3>(pos);
            position.Z -= 0.2f;
            Vector3 rotation = JsonConvert.DeserializeObject<Vector3>(rot);

            block.owner = p.sqlID;
            block.prop = PropStreamer.Create(model, position, rotation, dimension: p.Dimension, placeObjectOnGroundProperly: true, frozen: true);
            block.textlbl = TextLabelStreamer.Create("~b~[~w~路障 " + block.prop.Id.ToString() + "~b~]~n~~w~管理人: ~y~" + p.characterName.Replace("_", " "), position, streamRange: 2);
            pdRoadBlocks.Add(block);
        }

        [Command(CONSTANT.COM_PD_DeleteRoadBlock)]
        public static async Task COM_RemoveRoadBlock(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, CONSTANT.DESC_PD_DeleteRoadBlock); return; }
            bool pdCheck = await System.PD.CheckPlayerInPd(p);
            if (pdCheck == false)
            {
                if (!await FD.CheckPlayerInFD(p))
                {
                    MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotInPd); return;
                }
            }

            int id; bool isOk = Int32.TryParse(args[0], out id);
            if (!isOk)
                return;

            RoadBlockModel delete = pdRoadBlocks.Find(x => x.prop.Id == (ulong)id);
            if (delete == null) { MainChat.SendErrorChat(p, "[错误] 无效路障."); return; }

            delete.prop.Delete();
            delete.textlbl.Delete();
            pdRoadBlocks.Remove(delete);
            MainChat.SendInfoChat(p, "已移除路障."); return;
        }


        [Command(CONSTANT.COM_PD_CuffPlayer)]
        public static async Task COM_CuffPlayer(PlayerModel player, params string[] targetId)
        {
            bool pdCheck = await CheckPlayerInPd(player);
            if (pdCheck == false) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotInPd); return; }
            if (targetId.Length <= 0) { MainChat.SendInfoChat(player, CONSTANT.DESC_PD_CuffPlayer); return; }
            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(Int32.Parse(targetId[0].ToString()));
            if (target == null) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotFound); return; }
            if (target == player) { MainChat.SendErrorChat(player, CONSTANT.ERR_NotUseOnYou); return; }
            if (target.Position.Distance(player.Position) > 3f) { MainChat.SendErrorChat(player, CONSTANT.ERR_NotNearTarget); return; }

            target.isCuffed = !target.isCuffed;
            string emoteMsgSet = (target.isCuffed) ? " 从腰带取出手铐并铐住了 {0}" : " 伸手解开了 {0} 手上的手铐.";
            string emoteMsg = string.Format(emoteMsgSet, target.fakeName.Replace("_", " "));
            MainChat.EmoteMe(player, emoteMsg);
            return;
        }

        [Command(CONSTANT.COM_Megafon)]
        public static async Task COM_Megaphone(PlayerModel player, params string[] messages)
        {
            if (messages.Length <= 0) { MainChat.SendInfoChat(player, CONSTANT.DESC_Megafon); return; }
            string message = string.Join(" ", messages);
            VehModel veh = Vehicle.VehicleMain.getNearVehFromPlayer(player);
            if (veh == null) { MainChat.SendErrorChat(player, CONSTANT.ERR_Megafon); return; }
            if (veh.factionId != player.factionId) { MainChat.SendErrorChat(player, CONSTANT.ERR_PermissionError); return; }
            List<int> govIds = await Database.DatabaseMain.GetGovermentFactionIds();

            foreach (int gId in govIds)
            {
                if (player.factionId == gId) { MainChat.Megaphone(player, message); return; }
            }
            return;
        }

        [Command("m1")]
        public static async Task COM_Megaphone1(PlayerModel player)
        {
            string message = "LSPD, 靠边停车熄火!";
            VehModel veh = Vehicle.VehicleMain.getNearVehFromPlayer(player);
            if (veh == null) { MainChat.SendErrorChat(player, CONSTANT.ERR_Megafon); return; }
            if (veh.factionId != player.factionId) { MainChat.SendErrorChat(player, CONSTANT.ERR_PermissionError); return; }
            List<int> govIds = await Database.DatabaseMain.GetGovermentFactionIds();

            foreach (int gId in govIds)
            {
                if (player.factionId == gId) { MainChat.Megaphone(player, message); return; }
            }
            return;
        }

        [Command("m2")]
        public static async Task COM_Megaphone2(PlayerModel player)
        {
            string message = "LSPD, 立即靠边停车并关闭引擎!";
            VehModel veh = Vehicle.VehicleMain.getNearVehFromPlayer(player);
            if (veh == null) { MainChat.SendErrorChat(player, CONSTANT.ERR_Megafon); return; }
            if (veh.factionId != player.factionId) { MainChat.SendErrorChat(player, CONSTANT.ERR_PermissionError); return; }
            List<int> govIds = await Database.DatabaseMain.GetGovermentFactionIds();

            foreach (int gId in govIds)
            {
                if (player.factionId == gId) { MainChat.Megaphone(player, message); return; }
            }
            return;
        }

        [Command("m3")]
        public static async Task COM_Megaphone3(PlayerModel player)
        {
            string message = "LSPD, 靠边让路!";
            VehModel veh = Vehicle.VehicleMain.getNearVehFromPlayer(player);
            if (veh == null) { MainChat.SendErrorChat(player, CONSTANT.ERR_Megafon); return; }
            if (veh.factionId != player.factionId) { MainChat.SendErrorChat(player, CONSTANT.ERR_PermissionError); return; }
            List<int> govIds = await Database.DatabaseMain.GetGovermentFactionIds();

            foreach (int gId in govIds)
            {
                if (player.factionId == gId) { MainChat.Megaphone(player, message); return; }
            }
            return;
        }

        [Command("ms1")]
        public static async Task COM_Megaphones1(PlayerModel player)
        {
            string message = "LSSD, 靠边停车熄火!";
            VehModel veh = Vehicle.VehicleMain.getNearVehFromPlayer(player);
            if (veh == null) { MainChat.SendErrorChat(player, CONSTANT.ERR_Megafon); return; }
            if (veh.factionId != player.factionId) { MainChat.SendErrorChat(player, CONSTANT.ERR_PermissionError); return; }
            List<int> govIds = await Database.DatabaseMain.GetGovermentFactionIds();

            foreach (int gId in govIds)
            {
                if (player.factionId == gId) { MainChat.Megaphone(player, message); return; }
            }
            return;
        }

        [Command("ms2")]
        public static async Task COM_Megaphones2(PlayerModel player)
        {
            string message = "LSSD, 立即靠边停车并关闭引擎!";
            VehModel veh = Vehicle.VehicleMain.getNearVehFromPlayer(player);
            if (veh == null) { MainChat.SendErrorChat(player, CONSTANT.ERR_Megafon); return; }
            if (veh.factionId != player.factionId) { MainChat.SendErrorChat(player, CONSTANT.ERR_PermissionError); return; }
            List<int> govIds = await Database.DatabaseMain.GetGovermentFactionIds();

            foreach (int gId in govIds)
            {
                if (player.factionId == gId) { MainChat.Megaphone(player, message); return; }
            }
            return;
        }

        [Command("ms3")]
        public static async Task COM_Megaphones3(PlayerModel player)
        {
            string message = "LSSD, 靠边让路!";
            VehModel veh = Vehicle.VehicleMain.getNearVehFromPlayer(player);
            if (veh == null) { MainChat.SendErrorChat(player, CONSTANT.ERR_Megafon); return; }
            if (veh.factionId != player.factionId) { MainChat.SendErrorChat(player, CONSTANT.ERR_PermissionError); return; }
            List<int> govIds = await Database.DatabaseMain.GetGovermentFactionIds();

            foreach (int gId in govIds)
            {
                if (player.factionId == gId) { MainChat.Megaphone(player, message); return; }
            }
            return;
        }
        [Command(CONSTANT.COM_DepChat)]
        public static async Task COM_DepartmentChat(PlayerModel player, params string[] messages)
        {
            if (messages.Length <= 0) { MainChat.SendErrorChat(player, CONSTANT.DESC_DepChat); return; }
            var msg = string.Join(" ", messages);
            await Factions.Faction.FactionDepartmentChat(player, msg);
            return;
        }

        [Command("dyt")]
        public static async Task COM_DepartmentChatNear(PlayerModel player, params string[] messages)
        {
            if (messages.Length <= 0) { MainChat.SendErrorChat(player, "[用法] /dyt [文本]"); return; }
            var msg = string.Join(" ", messages);
            await Factions.Faction.FactionDepartmentChatNear(player, msg);
            return;
        }


        /*[Command(CONSTANT.COM_PD_Equipment)]
        public static void COM_PDEquipment(PlayerModel player)
        {
            bool pdCheck = CheckPlayerInPd(player);
            if (!pdCheck) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotInPd); return; }
            if (!player.HasData(EntityData.PlayerEntityData.PDDuty)) { MainChat.SendErrorChat(player, CONSTANT.ERR_PD_NeedDuty); return; }

            VehModel pdVeh = Vehicle.VehicleMain.getNearVehFromPlayer(player);
            if (player.Position.Distance(ServerGlobalValues.pdEquiptmenPos) > 5f)
            {
                if (pdVeh == null || pdVeh.factionId != player.factionId)
                {
                    MainChat.SendErrorChat(player, CONSTANT.ERR_PD_EquiptmentPos); return;
                }
            }


            List<GuiMenu> gMenu = new List<GuiMenu>();
            GuiMenu clothes1 = new GuiMenu { name = "Devriye Ekipmanı(Pistol)", triger = "PD:Equiptment", value = "memur1" };
            GuiMenu clothes2 = new GuiMenu { name = "Devriye Ekipmanı(CombatPistol)", triger = "PD:Equiptment", value = "memur2" };
            GuiMenu clothes3 = new GuiMenu { name = "Devriye Ekipmanı(PistolMkII)", triger = "PD:Equiptment", value = "memur3" };
            GuiMenu clothes4 = new GuiMenu { name = "Devriye Ekipmanı(Pistol50)", triger = "PD:Equiptment", value = "memur4" };
            GuiMenu clothes5 = new GuiMenu { name = "Acil Durum Ekipmanı", triger = "PD:Equiptment", value = "memur5" };
            GuiMenu clothes6 = new GuiMenu { name = "Ağır Ekipman", triger = "PD:Equiptment", value = "memur6" };
            GuiMenu clothes7 = new GuiMenu { name = "SWAT Ekipman", triger = "PD:Equiptment", value = "memur7" };
            //GuiMenu clothes6 = new GuiMenu { name = "Eylem Bastırma Ekipmanı", triger = "PD:Equiptment", value = "memur6" };


            gMenu.Add(clothes1);
            gMenu.Add(clothes2);
            gMenu.Add(clothes3);
            gMenu.Add(clothes4);
            gMenu.Add(clothes5);
            gMenu.Add(clothes6);
            gMenu.Add(clothes7);


            Gui y = new Gui()
            {
                image = "https://steamuserimages-a.akamaihd.net/ugc/781854100510855418/A68756B9970FC7AE517EE725CDD05E6CE3FCF8FA/",
                guiMenu = gMenu,
                color = "#00AAFF"
            };
            y.Send(player);

            return;
        }
        [AsyncClientEvent("PD:Equiptment")]
        public static void PD_EquipmentSelect(PlayerModel p, string value)
        {
            p.RemoveAllWeapons();
            switch (value)
            {
                case "memur1":
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.Nightstick, 1, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.Flashlight, 1, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.StunGun, 1, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.Pistol, 100, false);
                    p.Armor = 25;
                    p.Emit("GUI:Close");
                    break;

                case "memur2":
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.Nightstick, 1, false);                    
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.Flashlight, 1, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.StunGun, 1, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.CombatPistol, 100, false);
                    p.Armor = 25;
                    p.Emit("GUI:Close");
                    break;

                case "memur3":
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.Nightstick, 1, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.Flashlight, 1, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.StunGun, 1, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.PistolMkII, 100, false);
                    p.Armor = 25;
                    p.Emit("GUI:Close");
                    break;

                case "memur4":
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.Nightstick, 1, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.Flashlight, 1, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.StunGun, 1, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.Pistol50, 100, false);
                    p.Armor = 25;
                    p.Emit("GUI:Close");
                    break;

                case "memur5":
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.Nightstick, 1, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.Flashlight, 1, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.StunGun, 1, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.CombatPistol, 100, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.SMG, 300, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.PumpShotgun, 50, false);
                    p.Armor = 100;
                    p.Emit("GUI:Close");
                    break;

                case "memur6":
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.Nightstick, 1, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.Flashlight, 1, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.StunGun, 1, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.Pistol50, 50, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.CarbineRifle, 300, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.PumpShotgun, 50, false);
                    p.Armor = 100;
                    p.Emit("GUI:Close");
                    break;

                case "memur7":
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.Nightstick, 1, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.Flashlight, 1, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.StunGun, 1, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.CombatPistol, 50, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.CarbineRifle, 300, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.PumpShotgun, 50, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.SMG, 50, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.BZGas, 50, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.Flare, 50, false);
                    p.GiveWeapon(AltV.Net.Enums.WeaponModel.GrenadeLauncherSmoke, 50, false);
                    p.Armor = 100;
                    p.Emit("GUI:Close");
                    break;
            }
        }*/

        [Command(CONSTANT.COM_PD_Radio)]
        public async static Task COM_PdRadio(PlayerModel player, params string[] messages)
        {
            if (messages.Length <= 0) { MainChat.SendInfoChat(player, CONSTANT.DESC_PD_Radio); return; }
            bool pdCheck = await CheckPlayerInPd(player);
            if (!pdCheck) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotInPd); return; }
            if (player.lscGetdata<bool>(EntityData.PlayerEntityData.PDDuty) == false) { MainChat.SendErrorChat(player, CONSTANT.ERR_PD_NeedDuty); return; }
            //OtherSystem.Animations.PlayerAnimation(player, new string[] { "radio2" }); // Telsiz animasyonu.

            var Team = pdTeams.Find(x => x.leaderId == player.sqlID || x.memberId == player.sqlID);
            string TeamName = "";
            if (Team != null) { TeamName = Team.name; }

            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(player.factionId);
            string rank = Factions.Faction.GetFactionRankString(player.factionRank, fact);
            string message = "";
            //string message2 = "";
            if (player.RadioFreq == 1)
            {
                message = "<i class='fad fa-walkie-talkie'></i>[CH-911] " + rank + " " + player.fakeName.Replace("_", " ") + " " + TeamName + ": " + string.Join(" ", messages);
                //message2 = "<i class='fad fa-walkie-talkie'></i>" + string.Join(" ", messages);
                foreach (PlayerModel t in Alt.GetAllPlayers())
                {
                    if (t.factionId == player.factionId && t.HasData(EntityData.PlayerEntityData.PDDuty)) { MainChat.PDRadioChat(t, message); }
                    else if (player.Position.Distance(t.Position) < 5)
                    {
                        t.SendChatMessage(message);
                    }
                }
            }
            else
            {
                message = "<i class='fad fa-walkie-talkie'></i>[CH-911-" + player.RadioFreq.ToString() + "] " + rank + " " + player.fakeName.Replace("_", " ") + " " + TeamName + ": " + string.Join(" ", messages);
                //message2 = "<i class='fad fa-walkie-talkie'></i>" + string.Join(" ", messages);
                foreach (PlayerModel t in Alt.GetAllPlayers())
                {
                    if (t.factionId == player.factionId && t.RadioFreq == player.RadioFreq && t.HasData(EntityData.PlayerEntityData.PDDuty)) { MainChat.PDRadioChat(t, message); }
                }
            }

            //MainChat.NormalChat(player, message2);

            if (!player.Exists)
                return;
            await Task.Delay(2000);
            if (!player.Exists)
                return;
            OtherSystem.Animations.PlayerStopAnimation(player);

            return;
        }

        [Command(CONSTANT.COM_CreateTeam)]
        public static async Task COM_PDCreateTeam(PlayerModel player, params string[] values)
        {
            if (values.Length <= 1) { MainChat.SendInfoChat(player, CONSTANT.DESC_CreateTeam); return; }
            bool pdCheck = await CheckPlayerInPd(player);
            if (pdCheck == false) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotInPd); return; }
            var playerTeam = pdTeams.Find(x => x.leaderId == player.sqlID && x.memberId == player.sqlID);
            if (playerTeam != null) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerInTeam); return; }
            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(Int32.Parse(values[0].ToString()));
            if (target == null) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotFound); return; }
            var targetTeam = pdTeams.Find(x => x.leaderId == target.sqlID && x.memberId == target.sqlID);
            if (targetTeam != null) { MainChat.SendErrorChat(player, string.Format(CONSTANT.ERR_TargetInTeam, target.fakeName.Replace("_", " "))); return; }
            bool targetpdCheck = await CheckPlayerInPd(target);
            if (targetpdCheck == false) { MainChat.SendErrorChat(player, string.Format(CONSTANT.ERR_TargetNotInPD, target.fakeName.Replace("_", " "))); return; }

            pdTeams.Add(new PDTeamModel { name = values[1].ToString(), leaderId = player.sqlID, memberId = target.sqlID });
            MainChat.SendInfoChat(player, string.Format(CONSTANT.INFO_CreateTeamPlayerSucces, values[1].ToString()));
            MainChat.SendInfoChat(target, string.Format(CONSTANT.INFO_CreateTeamTargetSucces, player.fakeName.Replace("_", " "), values[1].ToString()));
            return;
        }

        [Command(CONSTANT.COM_PDDestroyTeam)]
        public static async Task COM_PDDestroyTeam(PlayerModel player, params string[] notUsed)
        {
            bool pdCheck = await CheckPlayerInPd(player);
            if (pdCheck == false) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotInPd); return; }
            var team = pdTeams.Find(x => x.leaderId == player.sqlID || x.memberId == player.sqlID);
            if (team == null) { MainChat.SendErrorChat(player, CONSTANT.ERR_PDTeamNotFound); return; }
            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(team.leaderId);
            if (target == player) { target = GlobalEvents.GetPlayerFromSqlID(team.memberId); }
            pdTeams.Remove(team);
            MainChat.SendInfoChat(player, CONSTANT.INFO_PDDestroyTeam);
            MainChat.SendInfoChat(target, CONSTANT.INFO_PDDestroyTeam);
            return;
        }

        [Command("aclearpdteams")]
        public static void COM_DestroyAllPDTeam(PlayerModel p)
        {
            if (p.adminLevel <= 2) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            pdTeams = new List<PDTeamModel>();
            MainChat.SendInfoChat(p, "[!] 成功清理所有 警用单位!");
            return;
        }
        [Command(CONSTANT.COM_PDTeamList)]
        public static async Task COM_PDTeamList(PlayerModel player)
        {
            bool pdCheck = await CheckPlayerInPd(player);
            if (pdCheck == false) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotInPd); return; }
            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(player.factionId);

            MainChat.SendInfoChat(player, "--------------[单位列表]------------");
            foreach (PDTeamModel x in pdTeams)
            {
                PlayerModel leader = GlobalEvents.GetPlayerFromSqlID(x.leaderId);
                PlayerModel member = GlobalEvents.GetPlayerFromSqlID(x.memberId);
                if (leader == null || member == null)
                {
                    pdTeams.Remove(x);
                    continue;
                }
                string rankLeader = Factions.Faction.GetFactionRankString(leader.factionRank, fact);
                string rankMember = Factions.Faction.GetFactionRankString(member.factionRank, fact);
                string text;
                if (leader == member) { text = x.name + " [" + rankLeader + "] " + leader.fakeName.Replace("_", " "); }
                else { text = x.name + " [" + rankLeader + "] " + leader.fakeName.Replace("_", " ") + " | [" + rankMember + "] " + member.fakeName.Replace("_", " "); }
                MainChat.SendInfoChat(player, text);
            }
            return;
        }

        [Command(CONSTANT.COM_PD_RadioFreq)]
        public static async Task COM_PDRadioFreqSet(PlayerModel player, params string[] values)
        {
            bool pdCheck = await CheckPlayerInPd(player);
            if (pdCheck == false) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotInPd); return; }
            if (values.Length <= 0) { MainChat.SendErrorChat(player, CONSTANT.ERR_PD_RadioWrong); return; }
            int Freq = Int32.Parse(values[0].ToString());
            if (Freq <= 0 || Freq > 99) { MainChat.SendErrorChat(player, CONSTANT.ERR_PD_RadioWrong); return; }
            player.RadioFreq = Freq;
            MainChat.SendInfoChat(player, string.Format(CONSTANT.INFO_PD_ReadioFreqSucces, Freq.ToString()));
            return;
        }

        [Command(CONSTANT.COM_PD_DestroyDoor)]
        public static async Task COM_PDDestroyDoor(PlayerModel p)
        {
            bool pdCheck = await CheckPlayerInPd(p);
            if (pdCheck == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotInPd); return; }
            (BusinessModel, Marker, PlayerLabel) biz = await Props.Business.getNearestBusiness(p);
            if (biz.Item1 == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PD_DestroyDoorNotFound); return; }
            //if(biz.Item1.isLocked == true) { MainChat.SendErrorChat(p, CONSTANT.ERR_PD_DestroyDoorAlreadyUnlocked); return; }            

            MainChat.EmoteMe(p, ServerEmotes.EMOTE_PDDestroyDoor);
            await Task.Delay(5000);

            if (!p.Exists)
                return;


            MainChat.EmoteDo(p, ServerEmotes.EMOTE_PDDestroyDoorDo);
            biz.Item1.isLocked = false;
            await biz.Item1.Update(biz.Item2, biz.Item3);
            return;
        }
        [Command("bhd")]
        public static async Task COM_PDDestroyDoorHouse(PlayerModel p)
        {
            bool pdCheck = await CheckPlayerInPd(p);
            if (pdCheck == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotInPd); return; }
            var biz = await Props.Houses.getNearHouse(p);
            if (biz.Item1 == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PD_DestroyDoorNotFound); return; }
            //if(biz.Item1.isLocked == true) { MainChat.SendErrorChat(p, CONSTANT.ERR_PD_DestroyDoorAlreadyUnlocked); return; }            

            MainChat.EmoteMe(p, ServerEmotes.EMOTE_PDDestroyDoor);
            await Task.Delay(5000);

            if (!p.Exists)
                return;


            MainChat.EmoteDo(p, ServerEmotes.EMOTE_PDDestroyDoorDo);
            biz.Item1.isLocked = false;
            biz.Item1.Update(biz.Item3, biz.Item2);
            return;
        }



        [Command(CONSTANT.COM_Requisition)]
        public static async Task COM_TakeItem(PlayerModel p, params string[] args)
        {
            if (args.Length <= 1) { MainChat.SendInfoChat(p, CONSTANT.DESC_Requisition); return; }
            bool pdCheck = await CheckPlayerInPd(p);
            if (pdCheck == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotInPd); return; }
            int ID = Int32.Parse(args[0]);
            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(ID);
            if (target == null || target.Position.Distance(p.Position) > 3) { MainChat.SendErrorChat(p, "无效玩家或离您太远"); return; }
            int ItemId = Int32.Parse(args[1]);
            List<Models.InventoryModel> tInv = await Database.DatabaseMain.GetPlayerInventoryItems(target.sqlID);
            InventoryModel tItem = tInv.Find(x => x.ID == ItemId);
            if (tItem == null) { MainChat.SendErrorChat(p, "指定玩家没有指定物品"); return; }

            if (tItem.itemId == 28 || tItem.itemId == 29 || tItem.itemId == 30)
            {
                OtherSystem.LSCsystems.WeaponSystem.TakeOutWeapon(target, tItem);
            }

            p.SendChatMessage("您没收了 " + target.fakeName.Replace("_", " ") + " 的物品 " + tItem.itemName);
            target.SendChatMessage(p.fakeName.Replace("_", " ") + " 没收了您的 " + tItem.itemName);
            //MainChat.EmoteMe(p, " 没收了 " + target.fakeName.Replace("_", " ") + " 的物品.");

            await OtherSystem.Inventory.RemoveInventoryItem(target, tItem.ID, tItem.itemAmount);

            foreach (PlayerModel ts in Alt.GetAllPlayers())
            {
                if (ts.factionId == p.factionId && ts.HasData(EntityData.PlayerEntityData.PDDuty)) { ts.SendChatMessage("{DA2108}" + p.fakeName.Replace("_", " ") + " 没收了 " + target.fakeName.Replace("_", " ") + " 的物品 " + tItem.itemName); }
            }
            Core.Logger.WriteLogData(Logger.logTypes.pdlog, p.fakeName.Replace("_", " ") + " 没收了 " + target.fakeName.Replace("_", " ") + " 的物品 " + tItem.itemName);
            return;
        }

        [Command(CONSTANT.COM_PDRadar)]
        public static async Task COM_VehicleRadar(PlayerModel p)
        {
            bool pdCheck = await CheckPlayerInPd(p);
            if (pdCheck == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotInPd); return; }
            if (p.Vehicle == null) { MainChat.SendErrorChat(p, "您不在车内."); return; }
            VehModel v = (VehModel)p.Vehicle;
            if (v.factionId != p.factionId) { MainChat.SendErrorChat(p, "警用雷达只能在车内使用."); return; }

            if (v.HasSyncedMetaData(EntityData.VehicleEntityData.isRadarOn))
            {
                v.DeleteSyncedMetaData(EntityData.VehicleEntityData.isRadarOn);
                MainChat.SendInfoChat(p, "警用雷达关闭.");
                p.EmitLocked("APL:Stop");
            }
            else
            {
                v.SetSyncedMetaData(EntityData.VehicleEntityData.isRadarOn, true);
                MainChat.SendInfoChat(p, "警用雷达开启.");
                p.EmitLocked("APL:Start");
            }
        }

        [Command(CONSTANT.COM_PDnearRadio)]
        public static async Task COM_YT(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, CONSTANT.DESC_PDnearRadio); return; }
            bool pdCheck = await CheckPlayerInPd(p);
            if (pdCheck == false)
            {
                if (!await FD.CheckPlayerInFD(p))
                {
                    MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotInPd); return;
                }
            }


            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            string rank = Factions.Faction.GetFactionRankString(p.factionRank, fact);

            string message = "{b5af02}<i class='fad fa-walkie-talkie'></i>[YT-911] " + rank + " " + p.fakeName.Replace("_", " ") + ": " + string.Join(" ", args);

            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (t.factionId == p.factionId && p.Position.Distance(t.Position) < 120 && (t.HasData(EntityData.PlayerEntityData.PDDuty) || t.HasData(EntityData.PlayerEntityData.FDDuty)))
                {
                    MainChat.PDRadioChat(t, message);
                }
                else if (p.Position.Distance(t.Position) < 5)
                {
                    t.SendChatMessage(message);
                }
            }
        }

        [Command(CONSTANT.COM_PDOperator)]
        public static async Task COM_Operator(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, CONSTANT.DESC_PDOperator); return; }
            bool pdCheck = await CheckPlayerInPd(p);
            if (pdCheck == false)
            {
                if (await FD.CheckPlayerInFD(p) == false)
                {
                    MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotInPd); return;
                }
            }

            string message = "{f2ea00}<i class='fad fa-walkie-talkie'></i>[" + p.sqlID + "] [调度员]: " + string.Join(" ", args);

            if (p.HasData(EntityData.PlayerEntityData.PDDuty))
            {
                foreach (PlayerModel t in Alt.GetAllPlayers())
                {
                    if (t.HasData(EntityData.PlayerEntityData.PDDuty))
                    {
                        MainChat.PDRadioChat(t, message);
                    }
                }
            }
            else if (p.HasData(EntityData.PlayerEntityData.FDDuty))
            {
                foreach (PlayerModel t in Alt.GetAllPlayers())
                {
                    if (t.HasData(EntityData.PlayerEntityData.FDDuty))
                    {
                        MainChat.PDRadioChat(t, message);
                    }
                }
            }
        }

        [Command(CONSTANT.COM_BeanBag)]
        public static async Task COM_BeanBag(PlayerModel p)
        {
            bool pdCheck = await CheckPlayerInPd(p);
            if (pdCheck == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotInPd); return; }

            VehModel pdVeh = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (pdVeh == null) { MainChat.SendErrorChat(p, "[错误] 附近没有警车."); return; }
            if (pdVeh.factionId != p.factionId) { MainChat.SendErrorChat(p, "[错误] 附近没有警车."); return; }

            p.GiveWeapon(AltV.Net.Enums.WeaponModel.PumpShotgun, 10, false);
            MainChat.EmoteMe(p, " 从车内取出了豆袋枪.");
            return;
        }

        [Command(CONSTANT.COM_TempJail)]
        public static async Task COM_SendTempJail(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, CONSTANT.DESC_TempJail); return; }
            if (p.Position.Distance(ServerGlobalValues.pdSendTempJailPos) > 10) { MainChat.SendErrorChat(p, "[错误] 您不在 LSPD拘留所."); return; }
            bool pdCheck = await CheckPlayerInPd(p);
            if (pdCheck == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotInPd); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(Int32.Parse(args[0]));
            if (t == null || t.Position.Distance(p.Position) > 5) { MainChat.SendInfoChat(p, "[错误] 无效玩家或离您太远."); return; }

            t.Position = ServerGlobalValues.pdTempJailPos;

            MainChat.SendInfoChat(p, "{08D54C}> 成功关押至拘留所.");
            MainChat.SendInfoChat(t, "{08D54C}> " + p.characterName.Replace("_", " ") + " 将您关押至 LSPD拘留所.");
        }

        [Command(CONSTANT.COM_TempJailOver)]
        public static async Task COM_StopTempJail(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, CONSTANT.DESC_TempJailOver); return; }
            if (p.Position.Distance(ServerGlobalValues.pdSendTempJailPos) > 10) { MainChat.SendErrorChat(p, "[错误] 您不在 LSPD拘留所."); return; }
            bool pdCheck = await CheckPlayerInPd(p);
            if (pdCheck == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotInPd); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(Int32.Parse(args[0]));
            if (t == null || t.Position.Distance(ServerGlobalValues.pdTempJailPos) > 7) { MainChat.SendErrorChat(p, "[错误] 无效玩家或离您太远."); return; }

            t.Position = p.Position;

            MainChat.SendInfoChat(p, "{08D54C}您将 " + t.characterName.Replace("_", " ") + " 从 LSPD拘留所 释放.");
            MainChat.SendInfoChat(t, "{08D54C}" + p.characterName.Replace("_", " ") + " 将您从 LSPD拘留所 释放.");
            return;
        }

        ///     !!!!!   ÖNEMLİ  !!!!
        ///     Hapse gönderme komutu yazılacak
        ///     !!!!!           !!!!        
        ///     

        //[Command(CONSTANT.COM_HelpReq)]

        //[Command(CONSTANT.COM_HelpReqAccept)]

        [AsyncClientEvent("PD:HelpReqUpdate")]
        public void UpdateHelpREq(PlayerModel p)
        {
            HelpReqModel checkHelp = pdHelpReqs.Find(x => x.helpClientSqlID == p.sqlID);
            if (checkHelp == null)
                return;

            foreach (HelpReqModel.helpClients cc in checkHelp.inHelp)
            {
                GetPlayerFromSqlID(cc.ClientSqlID).EmitAsync("PD:UpdatePosition", p.Id, p.Position);
            }
            return;
        }

        [Command("pdhelpreqs", aliases: new string[] { "pds" })]
        public static async Task ShowHelpReqs(PlayerModel p)
        {
            if (!await CheckPlayerInPd(p)) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            string text = "<center>可用警员支援</center><br>";
            foreach (var req in pdHelpReqs)
            {
                text += "支援请求: [" + req.helpClientSqlID + "]" + req.Reason + "<br>";
            }
            MainChat.SendInfoChat(p, text);
            return;
        }

        [Command(CONSTANT.COM_HelpReq, aliases: new string[] { "pd" })]
        public static async Task OpenHelpReq(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /hr [说明]"); return; }
            bool pdCheck = await CheckPlayerInPd(p);
            if (pdCheck == false)
            {
                pdCheck = await FD.CheckPlayerInFD(p);
                if (pdCheck == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotInPd); return; }
            }

            HelpReqModel checkHelp = pdHelpReqs.Find(x => x.helpClientSqlID == p.sqlID || x.inHelp.Find(x => x.ClientSqlID == p.sqlID) != null);
            if (checkHelp != null) { MainChat.SendErrorChat(p, "[错误] 您已经有一条支援请求了."); return; }

            string reason = string.Join(" ", args);
            HelpReqModel newHelp = new HelpReqModel()
            {
                helpCar = 0,
                helpClientSqlID = p.sqlID,
                helpClientID = p.Id,
                Reason = reason
            };

            pdHelpReqs.Add(newHelp);

            List<int> GovFacts = await Database.DatabaseMain.GetGovermentFactionIds();
            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                foreach (int check in GovFacts)
                {
                    if (t.factionId == check && (t.HasData(EntityData.PlayerEntityData.PDDuty) || t.HasData(EntityData.PlayerEntityData.FDDuty))) { t.SendChatMessage("{FF3E00}<i class='fas fa-map-marker-question'></i> [支援单位]{FFFFFF} " + p.characterName.Replace("_", " ") + " 请求支援. (( 响应支援请求输入 /hra " + p.sqlID + " ))<br>{FF3E00}说明: {FFFFFF}" + reason); }
                }
            }
            p.EmitLocked("PD:SetOwner");
            return;
        }

        [Command(CONSTANT.COM_HelpReqAccept, aliases: new string[] { "dk" })]
        public static async Task JoinHelpReq(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendErrorChat(p, CONSTANT.DESC_HelpReqAcccept); return; }
            bool pdCheck = await CheckPlayerInPd(p);
            if (pdCheck == false)
            {
                pdCheck = await FD.CheckPlayerInFD(p);
                if (pdCheck == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotInPd); return; }
            }

            HelpReqModel checkHelp = pdHelpReqs.Find(x => x.helpClientSqlID == p.sqlID || x.inHelp.Find(x => x.ClientSqlID == p.sqlID) != null);
            if (checkHelp != null) { MainChat.SendErrorChat(p, "[错误] 您已经有一条支援请求了."); return; }

            int tSql; bool isOk = Int32.TryParse(args[0], out tSql);
            if (!isOk) { MainChat.SendErrorChat(p, CONSTANT.DESC_HelpReqAcccept); return; }

            HelpReqModel joinReq = pdHelpReqs.Find(x => x.helpClientSqlID == tSql);
            if (joinReq == null) { MainChat.SendErrorChat(p, "[错误] 无效支援请求."); return; }

            HelpReqModel.helpClients newClient = new HelpReqModel.helpClients()
            {
                ClientID = p.Id,
                ClientSqlID = p.sqlID
            };

            joinReq.inHelp.Add(newClient);

            foreach (HelpReqModel.helpClients clients in joinReq.inHelp)
            {
                PlayerModel target = GlobalEvents.GetPlayerFromSqlID(clients.ClientSqlID);
                if (target == null)
                    continue;

                MainChat.SendInfoChat(target, "{FF3E00}[支援单位]{FFFFFF} " + p.characterName.Replace("_", " ") + " 响应了支援请求.");
                target.EmitLocked("PD:AddPlayer", p.Id, false);
                p.EmitLocked("PD:AddPlayer", target.Id, false);
            }


            PlayerModel owner = GlobalEvents.GetPlayerFromSqlID(joinReq.helpClientSqlID);
            if (owner == null)
            {
                foreach (HelpReqModel.helpClients clients in joinReq.inHelp)
                {
                    PlayerModel target = GlobalEvents.GetPlayerFromSqlID(clients.ClientSqlID);
                    if (target == null)
                        continue;

                    MainChat.SendInfoChat(target, "{FF3E00}[支援单位]{FFFFFF} 支援请求已关闭 (( 请求者离线 )).");
                    target.EmitLocked("PD:CloseReq");
                }

                pdHelpReqs.Remove(joinReq);
                return;
            }
            p.EmitLocked("PD:AddPlayer", owner.Id, true);
            MainChat.SendInfoChat(owner, "{FF3E00}[支援单位]{FFFFFF} " + p.characterName.Replace("_", " ") + " 响应了支援请求.");
            owner.EmitLocked("PD:AddPlayer", p.Id, false);
            return;
        }

        [Command(CONSTANT.COM_LeaveHelpReq, aliases: new string[] { "da" })]
        public static void COM_DestroyHelpReq(PlayerModel p)
        {
            HelpReqModel checkHelp = pdHelpReqs.Find(x => x.helpClientSqlID == p.sqlID || x.inHelp.Find(x => x.ClientSqlID == p.sqlID) != null);
            if (checkHelp == null) { MainChat.SendErrorChat(p, "[错误] 您没有在支援请求中."); return; }

            if (checkHelp.helpClientSqlID == p.sqlID)
            {
                foreach (HelpReqModel.helpClients clients in checkHelp.inHelp)
                {
                    PlayerModel target = GlobalEvents.GetPlayerFromSqlID(clients.ClientSqlID);
                    if (target == null)
                        continue;

                    MainChat.SendInfoChat(target, "{FF3E00}[支援单位]{FFFFFF} 支援请求已关闭 (( 由请求者关闭 )).");
                    target.EmitAsync("PD:CloseReq");
                }

                MainChat.SendInfoChat(p, "{FF3E00}[支援单位]{FFFFFF} 支援请求已关闭 (( 由请求者关闭 )).");
                p.EmitAsync("PD:CloseReq");
                pdHelpReqs.Remove(checkHelp);
                return;
            }
            else
            {
                HelpReqModel.helpClients currClient = checkHelp.inHelp.Find(x => x.ClientSqlID == p.sqlID);
                if (currClient == null)
                    return;

                foreach (HelpReqModel.helpClients clients in checkHelp.inHelp)
                {
                    PlayerModel target = GlobalEvents.GetPlayerFromSqlID(clients.ClientSqlID);
                    if (target == null)
                        continue;

                    MainChat.SendInfoChat(target, "{FF3E00}[支援单位]{FFFFFF} " + p.characterName.Replace("_", " ") + " 退出了支援请求.");

                }

                checkHelp.inHelp.Remove(currClient);
                p.EmitAsync("PD:CloseReq");
            }

        }

        [Command(CONSTANT.COM_PD_Jail)]
        public static async Task COM_CreatePlayerJail(PlayerModel p, params string[] args)
        {
            if (args.Length <= 2) { MainChat.SendInfoChat(p, CONSTANT.DESC_PD_Jail); return; }

            bool pdCheck = await CheckPlayerInPd(p);
            if (pdCheck == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotInPd); return; }

            if (p.Position.Distance(new Position(462, -989, 24)) > 5 && p.Position.Distance(new(591, -3, 76)) > 5 && p.Position.Distance(new(360.51428f, -1612.6681f, 29.279907f)) > 5) { MainChat.SendErrorChat(p, "[错误] 您不在监狱区."); return; }
            int tId; bool tIdOk = Int32.TryParse(args[0], out tId);
            int time; bool timeOk = Int32.TryParse(args[1], out time);

            if (!tIdOk || !timeOk) { MainChat.SendInfoChat(p, CONSTANT.DESC_PD_Jail); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tId);
            if (t == null) { MainChat.SendErrorChat(p, CONSTANT.CharacterNotFoundError); return; }

            t.Position = ServerGlobalValues.pdSendJailPos;
            t.jailTime += time;

            string text = t.characterName.Replace("_", " ") + " 被 " + p.characterName.Replace("_", " ") + " 关押进监狱, 时长: " + time.ToString() + " 分钟.";

            MainChat.SendInfoChat(t, p.characterName.Replace("_", " ") + " 将您关押 " + time.ToString() + " 分钟.");

            foreach (PlayerModel ct in Alt.GetAllPlayers())
            {
                if (ct.factionId == p.factionId) { MainChat.PDRadioChat(ct, text); }
            }

            OtherSystem.LSCsystems.MDCEvents.MDC_CreateRecord_Jail(t, p, time, string.Join(" ", args[2..]));

            return;
        }

        [Command(CONSTANT.COM_PD_CreateWanted)]
        public static async Task COM_OpenWanted(PlayerModel p, params string[] args)
        {
            if (args.Length <= 1) { MainChat.SendInfoChat(p, CONSTANT.DESC_PD_CreateWanted); return; }

            bool pdCheck = await CheckPlayerInPd(p);
            if (pdCheck == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotInPd); return; }

            int tSql; bool isOk = Int32.TryParse(args[0], out tSql);

            if (!isOk)
                return;

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null)
            {
                PlayerModelInfo offlineT = await Database.DatabaseMain.getCharacterInfo(tSql);
                if (offlineT == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家!"); return; }
                OtherSystem.LSCsystems.MDCEvents.MDC_CreateMDC_Wanted(offlineT, string.Join(" ", args[1..]), p.characterName.Replace("_", " "));
                MainChat.SendInfoChat(p, "[?] 您通缉了 {D89818}" + offlineT.characterName.Replace("_", " ") + "<br>原因: " + string.Join(" ", args[1..]));
                return;
            }
            else
            {
                string reason = string.Join(" ", args[1..]);
                OtherSystem.LSCsystems.MDCEvents.MDC_CreateMDC_Wanted(t, reason, p.characterName.Replace("_", " "));
                MainChat.SendInfoChat(p, "[?] 您通缉了 {D89818}" + t.characterName.Replace("_", " ") + "<br>原因: " + string.Join(" ", args[1..]));
                return;
            }


        }

        [Command("apbcar")]
        public static async Task COM_OpenWantedCar(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /apbcar [车牌号]"); return; }

            bool pdCheck = await CheckPlayerInPd(p);
            if (pdCheck == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotInPd); return; }

            VehModel v = (VehModel)Alt.GetAllVehicles().Where(x => x.NumberplateText == args[0]).FirstOrDefault();
            if (v == null) { MainChat.SendErrorChat(p, "[错误] 无效车辆!"); return; }
            v.settings.isWanted = !v.settings.isWanted;
            v.settings.PDWantedName = p.fakeName.Replace("_", " ");
            MainChat.SendInfoChat(p, "[!] 已更新此车的通缉状态.");
            return;
        }

        [Command("apbcarlist", aliases: new string[] { "apbcl" })]
        public static async Task COM_ShowWantedCars(PlayerModel p)
        {
            bool pdCheck = await CheckPlayerInPd(p);
            if (pdCheck == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotInPd); return; }

            string list = "<center>正在被通缉的车辆</center>";
            foreach (VehModel v in Alt.GetAllVehicles())
            {
                if (v.settings.isWanted)
                {
                    list += "<br>" + ((AltV.Net.Enums.VehicleModel)v.Model).ToString() + " | " + v.NumberplateText + " | " + v.settings.PDWantedName;
                }
            }
            MainChat.SendInfoChat(p, list);
            return;
        }

        [Command("jails")]
        public async Task COM_ShowJails(PlayerModel p)
        {
            bool pdCheck = await CheckPlayerInPd(p);
            if (pdCheck == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotInPd); return; }

            string text = "<center>正在被关押人员</center>";
            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (t.jailTime > 0)
                {
                    text += "<br>姓名: " + t.characterName.Replace("_", " ") + " 剩余时间: " + t.jailTime.ToString();
                }
            }
            if (text == "<center>正在被关押人员</center>") { text += "<br> 无"; }
            MainChat.SendInfoChat(p, text, true);
        }

        [Command("ssiren")]
        public async Task COM_SlientSiren(PlayerModel p, params string[] args)
        {
            bool pdCheck = await CheckPlayerInPd(p);
            if (!pdCheck)
            {
                bool fdCheck = await FD.CheckPlayerInFD(p);
                if (!fdCheck) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            }

            if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] 您必须在车内."); return; }

            VehModel v = (VehModel)p.Vehicle;
            if (v.HasStreamSyncedMetaData("Vehicle:Siren"))
            {
                v.DeleteStreamSyncedMetaData("Vehicle:Siren");
                MainChat.SendInfoChat(p, "[?] 警车静音警报器已移除.");
            }
            else
            {
                await v.SetStreamSyncedMetaDataAsync("Vehicle:Siren", true);
                MainChat.SendInfoChat(p, "[?] 警车静音警报器已开启.");
            }
            return;
        }

        [Command("msiren")]
        public async Task COM_ManageSiren(PlayerModel p, params string[] args)
        {
            bool pdCheck = await CheckPlayerInPd(p);
            if (!pdCheck)
            {
                bool fdCheck = await FD.CheckPlayerInFD(p);
                if (!fdCheck) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            }

            if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] 您必须在车内."); return; }

            VehModel v = (VehModel)p.Vehicle;
            if (v.HasStreamSyncedMetaData("Vehicle:Siren"))
            {
                v.DeleteStreamSyncedMetaData("Vehicle:Siren");
                MainChat.SendInfoChat(p, "[?] 警车静音警报器已移除.");
            }
            else
            {
                await v.SetStreamSyncedMetaDataAsync("Vehicle:Siren", true);
                MainChat.SendInfoChat(p, "[?] 警车静音警报器已开启.");
            }
            return;
        }

        [Command("removedrug")]
        public async Task COM_RemoveDrug(PlayerModel p)
        {
            bool pdCheck = await CheckPlayerInPd(p);

            if (!pdCheck) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }

            OtherSystem.LSCsystems.Drug.COM_RemoveDrugOnGround(p);
        }

        [Command("friskcargun")]
        public async Task COM_FriskVehicle(PlayerModel p)
        {
            if (p.Position.Distance(new Position(451.68793f, -1017.3494f, 28.48f)) > 10) { MainChat.SendErrorChat(p, "[错误] 您必须在 PD 车库入口才能使用此指令!"); return; }
            if (!await CheckPlayerInPd(p)) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }

            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v == null) { MainChat.SendErrorChat(p, "[错误] 附近没有车辆!"); return; }

            v.settings.PDLock = !v.settings.PDLock;
            v.settings.PDLockName = p.fakeName.Replace("_", " ");
            string text = "{DA2108}" + p.fakeName.Replace("_", " ") + " 将车辆 [" + v.sqlID + "]" + ((AltV.Net.Enums.VehicleModel)v.Model).ToString() + "(" + v.NumberplateText + ") 的武器搜查禁令设置为: " + ((v.settings.PDLock) ? "没收" : "解除禁令");
            v.Update();
            foreach (PlayerModel ts in Alt.GetAllPlayers())
            {
                if (ts.factionId == p.factionId && ts.HasData(EntityData.PlayerEntityData.PDDuty)) { ts.SendChatMessage(text); }
            }
            return;
        }

        [Command("removenearroadblocks", aliases: new string[] { "rnrb" })]
        public async Task COM_RemoveNearBarricades(PlayerModel p)
        {
            if (!await CheckPlayerInPd(p)) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            List<RoadBlockModel> removeList = new List<RoadBlockModel>();
            foreach (var bar in pdRoadBlocks)
            {
                if (p.Position.Distance(bar.prop.Position) < 10 && bar.prop.Dimension == p.Dimension)
                {
                    removeList.Add(bar);
                }
            };

            foreach (var dl in removeList)
            {
                var check = pdRoadBlocks.Find(x => x.prop.Id == dl.prop.Id);
                if (check == null)
                    continue;

                check.prop.Destroy();
                check.textlbl.Delete();
                pdRoadBlocks.Remove(check);
            }

            MainChat.SendInfoChat(p, "[?] 已移除范围 10 内的所有路障.");
            return;
        }

        [Command("k9")]
        public async Task COM_k9(PlayerModel p, params string[] args)
        {
            if (!await CheckPlayerInPd(p)) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /k9 [选项] [数值(如果有)]<br>可用选项<br>create[创建], on[跟随], off[取消跟随], incar[上车], excar[下车], des[删除]"); return; }
            switch (args[0])
            {
                case "create":
                    if (p.HasData("K9:ID")) { MainChat.SendErrorChat(p, "[错误] 您已创建K9, 请先删除目前的K9."); return; }
                    if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /k9 create [模型]"); return; }
                    OtherSystem.PedModel ped = OtherSystem.PedStreamer.Create(args[1], p.Position);
                    //ped.netOwner = p;
                    ped.hasNetOwner = true;
                    ped.nametag = "~b~K9";
                    ped.Dimension = p.Dimension;
                    p.SetData("K9:ID", ped.Id);
                    MainChat.SendInfoChat(p, "[?] 成功创建K9 ID为: " + ped.Id);
                    return;

                case "on":
                    if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /k9 on [跟随玩家ID] "); return; }
                    if (!Int32.TryParse(args[1], out int followTargetSql)) { MainChat.SendInfoChat(p, "[用法] /k9 on [跟随玩家ID] "); return; }
                    PlayerModel fallowTarget = GlobalEvents.GetPlayerFromSqlID(followTargetSql);
                    if (fallowTarget == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家!"); return; }
                    if (!p.HasData("K9:ID")) { MainChat.SendErrorChat(p, "[错误] 无效K9, 请使用 /k9 create [模型] 创建."); return; }
                    OtherSystem.PedModel follow = OtherSystem.PedStreamer.Get(p.lscGetdata<ulong>("K9:ID"));
                    if (follow == null) { MainChat.SendErrorChat(p, "[错误] 获取 K9 数据错误, 请联系管理员."); return; }
                    follow.followTarget = fallowTarget.Id;
                    MainChat.EmoteDo(follow.Position, "K9 开始跟随 " + fallowTarget.fakeName.Replace("_", " ") + " , 虚拟世界: ", follow.Dimension);
                    return;

                case "off":
                    if (!p.HasData("K9:ID")) { MainChat.SendErrorChat(p, "[错误] 无效K9, 请使用 /k9 create [模型] 创建."); return; }
                    OtherSystem.PedModel followstop = OtherSystem.PedStreamer.Get(p.lscGetdata<ulong>("K9:ID"));
                    if (followstop == null) { MainChat.SendErrorChat(p, "[错误] 获取 K9 数据错误, 请联系管理员."); return; }
                    followstop.followTarget = null;
                    MainChat.EmoteDo(followstop.Position, "K9 取消跟随.", followstop.Dimension);
                    return;

                case "incar":
                    if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] 您必须在车内."); return; }
                    if (!p.HasData("K9:ID")) { MainChat.SendErrorChat(p, "[错误] 无效K9, 请使用 /k9 create [模型] 创建."); return; }
                    OtherSystem.PedModel entercar = OtherSystem.PedStreamer.Get(p.lscGetdata<ulong>("K9:ID"));
                    if (entercar == null) { MainChat.SendErrorChat(p, "[错误] 获取 K9 数据错误, 请联系管理员."); return; }
                    entercar.enterVehicle = p.Vehicle.Id;
                    MainChat.EmoteDo(entercar.Position, "K9 上车了.", entercar.Dimension);
                    return;

                case "excar":
                    if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] 您必须在车内."); return; }
                    if (!p.HasData("K9:ID")) { MainChat.SendErrorChat(p, "[错误] 无效K9, 请使用 /k9 create [模型] 创建."); return; }
                    OtherSystem.PedModel exitcar = OtherSystem.PedStreamer.Get(p.lscGetdata<ulong>("K9:ID"));
                    if (exitcar == null) { MainChat.SendErrorChat(p, "[错误] 获取 K9 数据错误, 请联系管理员."); return; }
                    exitcar.enterVehicle = null;
                    MainChat.EmoteDo(exitcar.Position, "K9 下车了.", exitcar.Dimension);
                    return;

                case "des":
                    if (!p.HasData("K9:ID")) { MainChat.SendErrorChat(p, "[错误] 无效K9, 请使用 /k9 create [模型] 创建."); return; }
                    OtherSystem.PedModel remove = OtherSystem.PedStreamer.Get(p.lscGetdata<ulong>("K9:ID"));
                    if (remove == null) { MainChat.SendErrorChat(p, "[错误] 获取 K9 数据错误, 请联系管理员."); return; }
                    remove.Destroy();
                    p.DeleteData("K9:ID");
                    return;

                default:
                    MainChat.SendInfoChat(p, "[用法] /k9 [选项] [数值(如果有)]<br>可用选项<br>create[创建], on[跟随], off[取消跟随], incar[进车], excar[下车], des[删除]");
                    return;
            }
        }
        [Command("fod")]
        public async Task forcedown(PlayerModel p, params string[] args)
        {
            if (!await CheckPlayerInPd(p)) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            if (args.Length <= 0) { MainChat.SendErrorChat(p, CONSTANT.DESC_ForceDown); return; }
            if (!Int32.TryParse(args[0], out int tSql))
                return;
            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }
            if (p.Position.Distance(t.Position) > 3) { MainChat.SendErrorChat(p, CONSTANT.ERR_NeedNearPlayer); return; }
            MainChat.EmoteMe(p, "试图制服 " + string.Format(t.characterName.Replace("_", " ")));
            var rand = new Random();
            int ForceS = rand.Next(0, 4);
            if (ForceS == 0 || ForceS == 1 || ForceS == 2)
            {
                MainChat.EmoteDo(p, t.characterName.Replace("_", " ") + " 被制服在地.");
                MainChat.SendInfoChat(t, "[信息] " + p.characterName.Replace("_", " ") + " 制服了您.");
                p.Position = t.Position;
                OtherSystem.Animations.PlayerAnimation(p, "sleep2");
                OtherSystem.Animations.PlayerAnimation(t, "sleep2");
            }
            else
            {
                MainChat.EmoteDo(p, "未能成功制服前方的人并倒地.");
                p.Position = t.Position;
                OtherSystem.Animations.PlayerAnimation(p, "fall2");
            }

        }

        [Command("ctag")]
        public async Task COM_CarTagPD(PlayerModel p)
        {
            if (!await CheckPlayerInPd(p)) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[错误] 您必须在车内."); return; }
            VehModel v = (VehModel)p.Vehicle;
            if (v == null) { MainChat.SendErrorChat(p, "[错误] 无效车辆."); return; }
            if (v.factionId != p.factionId) { MainChat.SendErrorChat(p, "[错误] 此车不属于您的组织."); return; }
            var team = pdTeams.Find(x => x.leaderId == p.sqlID);
            if (team == null) { MainChat.SendErrorChat(p, "[错误] 要使用此命令, 您必须已创建一个单位并且是单位队长."); return; }
            //v.SetStreamSyncedMetaData("NameTag1", text);
            if (v.HasStreamSyncedMetaData("NameTag1"))
            {
                v.DeleteStreamSyncedMetaData("NameTag1");
                MainChat.SendInfoChat(p, "[?] 已移除 警车标签.");
                return;
            }
            else
            {
                v.SetStreamSyncedMetaData("NameTag1", team.name);
                MainChat.SendInfoChat(p, "[?] 已添加 警车标签.");
                return;
            }
        }

        [Command("ttcar")]
        public async Task COM_VehTT(PlayerModel p, params string[] args)
        {
            if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /ttcar [小时/天/分钟] [数值]"); return; }
            if (!await CheckPlayerInPd(p)) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }

            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v == null) { MainChat.SendErrorChat(p, "[错误] 附近没有车辆!"); return; }
            if (!Int32.TryParse(args[1], out int date)) { MainChat.SendInfoChat(p, "[用法] /ttcar [小时/天/分钟] [数值]"); return; }
            int minute = 0;
            switch (args[0])
            {
                case "分钟":
                    minute = date;
                    break;
                case "小时":
                    minute = date * 60;
                    break;

                case "天":
                    minute = date * 60 * 24;
                    break;

                default: MainChat.SendInfoChat(p, "[用法] /ttcar [小时/天/分钟] [数值]"); return;
            }
            v.settings.PDLockDate = DateTime.Now.AddMinutes(minute);
            v.settings.PDLock = true;
            v.settings.PDLockName = p.fakeName.Replace("_", " ");
            await v.SetDimensionAsync(v.sqlID);
            await v.SetPositionAsync(new Position(451.68793f, -1017.3494f, 28.48f));

            string text = "{DA2108}" + p.fakeName.Replace("_", " ") + " 将嫌犯车辆 [" + v.sqlID + "]" + ((AltV.Net.Enums.VehicleModel)v.Model).ToString() + "(" + v.NumberplateText + ") 的扣押状态设置为: " + date + args[0] + " 扣押.";
            v.Update();
            foreach (PlayerModel ts in Alt.GetAllPlayers())
            {
                if (ts.factionId == p.factionId && ts.HasData(EntityData.PlayerEntityData.PDDuty)) { ts.SendChatMessage(text); }
            }
            return;
        }
        [Command("dettcar")]
        public async Task COM_DeVehTT(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /dettcar [车辆ID]"); }
            if (p.Position.Distance(new Position(428, -962, 29)) > 15) { MainChat.SendErrorChat(p, "[错误] 您必须在 PD 车库入口才能使用此指令!"); return; }
            //if (!await CheckPlayerInPd(p)) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (!Int32.TryParse(args[0], out int ID)) { MainChat.SendInfoChat(p, "[用法] /dettcar [车辆ID]"); }

            VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(ID);
            if (v == null)
            {
                MainChat.SendErrorChat(p, "[错误] 无效车辆!");
                return;
            }

            if (v.Dimension != v.sqlID && v.settings.PDLock == false)
            {
                MainChat.SendErrorChat(p, "[错误] 此车不在扣押世界中, 请联系管理员.");
                return;
            }
            if (v.fine > 300) { MainChat.SendErrorChat(p, "[错误] 您可以在还清所有车辆债务后将车辆开走."); return; }
            if (v.settings.PDLockDate > DateTime.Now) { MainChat.SendErrorChat(p, "[错误] 您还不能开走您的车辆, 剩余扣押时间: " + ((int)(v.settings.PDLockDate - DateTime.Now).TotalMinutes) + " 分钟."); return; }

            v.settings.PDLock = false;
            v.settings.PDLockName = "";
            await v.SetDimensionAsync(p.Dimension);
            await v.SetPositionAsync(p.Position);

            //string text = "{DA2108}" + p.fakeName.Replace("_", " ") + " isimli memur [" + v.sqlID + "]" + ((AltV.Net.Enums.VehicleModel)v.Model).ToString() + "(" + v.NumberplateText + ") model aracı bağlama durumunu kaldırdı.";

            v.Update();
            //foreach (PlayerModel ts in Alt.GetAllPlayers())
            //{
            //    if (ts.factionId == p.factionId && ts.HasData(EntityData.PlayerEntityData.PDDuty)) { ts.SendChatMsg(text); }
            //}
            return;
        }
        /*[Command("helicam")]
        public void helicam(PlayerModel p, params string[] args)
        {
            if (!CheckPlayerInPd(p)) { MainChat.SendErrorChat(p, "[错误] Bu komutu kullanmak için yetkiniz yok!"); return; }
            if (args.Length <= 0) { MainChat.SendErrorChat(p, CONSTANT.DESC_ForceDown); return; }
            if (!Int32.TryParse(args[0], out int tSql))
                return;
            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(t);
            p.EmitAsync("Helicam:Vehicle", v.Id);
        }*/

        /* [Command("kalkan")]
         public void shield(PlayerModel p)
         {
              if (!CheckPlayerInPd(p)) { MainChat.SendErrorChat(p, "[错误] Bu komutu kullanmak için yetkiniz yok!"); return; }
                if (p.HasData("shield"))
                {
                    MainChat.SendInfoChat(p, "Çıkarıldı.");
                       p.Emit("Server:Shield:removeShield", p);
                       p.DeleteData("shield");
                       p.DeleteData("shieldStat");
                   return;
               }
                else
                {
                    p.SetData("shield", true);
                    p.Emit("Server:Shield:giveShield", p, false);
                    MainChat.SendInfoChat(p, "Takıldı.");
                   return;
                }
            p.GiveWeapon(1141389967, 1, true);
         }

         [AsyncClientEvent("Server:Shield:shieldUp")]
         public void sheildUp(PlayerModel p)
         {
            MainChat.SendInfoChat(p, "shieldUP!");
             if (p.HasData("shield"))
             {
                 p.SetData("shiledStat", true);
             }
         }
         [AsyncClientEvent("Server:Shield:shieldDown")]
         public void sheildDown(PlayerModel p)
         {
             if (p.HasData("shield"))
             {
                MainChat.SendInfoChat(p, "shielddown!");
                p.SetData("shiledStat", false);
             }
         }
         [AsyncClientEvent("playerEnteringVehicle")]
         public void sheildInVeh(PlayerModel p)
         {
             if (p.IsInVehicle)
             {
                 if (p.HasData("shield"))
                 {
                    MainChat.SendInfoChat(p, "car");
                     p.Emit("Client:Shield:RemoveShieldForVehicle", p);
                 }
             }

         }
         [AsyncClientEvent("playerLeftVehicle")]
         public void sheildleftVeh(PlayerModel p)
         {
             if (!p.IsInVehicle)
             {
                 if (p.HasData("shield"))
                 {
                    MainChat.SendInfoChat(p, "care");
                    p.Emit("Client:Shield:AddShieldAfterVehicle", p);
                 }
             }

         }*/

        [Command("pdcall")]
        public async Task COM_CallOfficer(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /pdcall [说明]"); return; }
            if (TotalPDGroup() <= 0) { MainChat.SendErrorChat(p, "[!] 没有足够警察在线."); return; }
            if (p.Position.Distance(PDCallOfficerPos) > 5) { MainChat.SendErrorChat(p, "[错误] 您不在警员呼叫点."); return; }

            AddMDCCall(p, string.Join(" ", args));
            MainChat.SendInfoChat(p, "[?] 您的警员呼叫点已转接至相关单位, 会有相关人员尽快与您联系或在警局面对面会面.");
            return;
        }
        public static OtherSystem.LSCsystems.MDCEvents.MDCCalls AddMDCCall(PlayerModel p, string reason)
        {
            var check = OtherSystem.LSCsystems.MDCEvents.PDcalls.Find(x => x.pos.Distance(p.Position) < 30);
            if (check == null)
            {
                check = new OtherSystem.LSCsystems.MDCEvents.MDCCalls()
                {
                    callerName = "警局警员上门呼叫",
                    callNumber = 911,
                    pos = p.Position,
                    reason = reason
                };
                OtherSystem.LSCsystems.MDCEvents.PDcalls.Add(check);

                foreach (PlayerModel pd in Alt.GetAllPlayers())
                {
                    if (pd.HasData(EntityData.PlayerEntityData.PDDuty)) { pd.SendChatMessage("{49A0CD}[911] 收到新的警员上门呼叫, 您可以在 MDC 查看."); }
                }
            }

            return check;
        }
        public static int TotalPDGroup()
        {
            int count = 0;
            foreach (PlayerModel x in Alt.GetAllPlayers())
            {
                if (x.HasData(EntityData.PlayerEntityData.PDDuty))
                    ++count;
            }

            return count;
        }
    }

}
