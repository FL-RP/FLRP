using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Core;
using outRp.Models;
using outRp.OtherSystem;
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
    public class FD : IScript
    {
        public class fdCalls
        {
            public int tSql { get; set; }
            public string Mesagge { get; set; } = "负伤";
            public Position pos { get; set; }
        }

        public static List<fdCalls> serverFDCalls = new List<fdCalls>();

        public static async Task<bool> CheckPlayerInFD(PlayerModel p)
        {
            bool t = false;
            //CharacterModel p = GlobalEvents.PlayerGetData(player);
            List<int> a = await Database.DatabaseMain.GetTypeOfFactionIDs(ServerGlobalValues.fType_FD);
            List<int> b = await Database.DatabaseMain.GetTypeOfFactionIDs(ServerGlobalValues.fType_MD);

            foreach (int fId in a)
            {
                if (fId == p.factionId)
                {
                    return true;
                }
            }

            foreach (int mId in b)
            {
                if (mId == p.factionId)
                    return true;
            }
            return t;
        }

        public static async Task<bool> CheckPlayerInMD(PlayerModel p)
        {
            bool t = false;
            //CharacterModel p = GlobalEvents.PlayerGetData(player);
            List<int> a = await Database.DatabaseMain.GetTypeOfFactionIDs(ServerGlobalValues.fType_MD);

            foreach (int fId in a)
            {
                if (fId == p.factionId)
                {
                    return true;
                }
            }
            return t;
        }

        public static Position MD_Duty_Pos = new(336, -580, 28);
        public static Position FD_Duty_Pos = new(202, -1651, 34);
        public static void LoadFDSystem()
        {
            TextLabelStreamer.Create("~y~[呼叫医生]~n~~w~/emscall", new Position(338, -587, 28), streamRange: 5);
            MarkerStreamer.Create(MarkerTypes.MarkerTypeReplayIcon, MD_Duty_Pos, new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0, 0, 0), color: new Rgba(250, 0, 0, 140), faceCamera: true, bobUpDown: true, streamRange: 5);
            MarkerStreamer.Create(MarkerTypes.MarkerTypeReplayIcon, FD_Duty_Pos, new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0, 0, 0), color: new Rgba(250, 0, 0, 140), faceCamera: true, bobUpDown: true, streamRange: 5);
        }

        [Command(CONSTANT.COM_FDDuty)]
        public async Task FD_Duty(PlayerModel p)
        {
            bool inFD = await CheckPlayerInFD(p);
            if (!inFD) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (p.HasData(EntityData.PlayerEntityData.FDDuty))
            {
                await GlobalEvents.FreezePlayerClothes(p, true);
                p.SendChatMessage("您下班了.");
                //GlobalEvents.ClearPlayerTag(p);
                p.RemoveAllWeapons();
                GlobalEvents.NativeNotify(p, "~w~工作状态更新为~r~下班");
                p.DeleteData(EntityData.PlayerEntityData.FDDuty);
            }
            else
            {
                VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
                if (v.factionId != 46 && v.factionId != 7)
                {
                    MainChat.SendErrorChat(p, "[错误] 您必须在更衣室内使用此指令!");
                    return;
                }
                p.lscSetData(EntityData.PlayerEntityData.FDDuty, true);
                await GlobalEvents.FreezePlayerClothes(p, false);
                GlobalEvents.NativeNotify(p, "~w~工作状态更新为~g~执勤");
                p.SendChatMessage("您开始了执勤 - 请自行前往服装店购买制服搭配 - 制服系统待开发 - 使用 /equip 查看装备");
                p.SendChatMessage("/fcar 刷出组织车辆 - /fcarlist 查看已刷组织车辆 - /rfcar 删除已刷车辆 - /feditcar 编辑车辆颜色MOD等 - /feditplate 编辑车牌号");
                //FactionModel faction = await Database.DatabaseMain.GetFactionInfo(p.factionId);
                //string rank = Factions.Faction.GetFactionRankString(p.factionRank, faction);
                // GlobalEvents.SetPlayerTag(p, "~q~" + rank, true);
                await FD_Equipment(p);
            }
        }
        [Command("mduty")]
        public async Task MD_Duty(PlayerModel p)
        {
            bool inFD = await CheckPlayerInMD(p);
            if (!inFD) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (p.HasData(EntityData.PlayerEntityData.MDDuty))
            {
                await GlobalEvents.FreezePlayerClothes(p, true);
                p.SendChatMessage("您下班了.");
                // GlobalEvents.ClearPlayerTag(p);
                p.RemoveAllWeapons();
                p.Armor = 0;
                GlobalEvents.NativeNotify(p, "~w~工作状态更新为~r~下班.");
                p.DeleteData(EntityData.PlayerEntityData.MDDuty);
            }
            else
            {
                VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
                if (p.Position.Distance(MD_Duty_Pos) > 5)
                {
                    if (v == null)
                    {
                        MainChat.SendErrorChat(p, "[错误] 您必须在更衣室内使用此指令!");
                        return;
                    }
                }

                if (v.factionId != p.factionId)
                {
                    MainChat.SendErrorChat(p, "[错误] 您必须在更衣室内使用此指令!");
                    return;
                }
                p.lscSetData(EntityData.PlayerEntityData.MDDuty, true);
                await GlobalEvents.FreezePlayerClothes(p, false);
                GlobalEvents.NativeNotify(p, "~w~工作状态更新为~g~执勤.");
                p.SendChatMessage("您开始了执勤 - 请自行前往服装店购买制服搭配 - 制服系统待开发 - 使用 /equip 查看装备");
                p.SendChatMessage("/fcar 刷出组织车辆 - /fcarlist 查看已刷组织车辆 - /rfcar 删除已刷车辆 - /feditcar 编辑车辆颜色MOD等 - /feditplate 编辑车牌号");
                //FactionModel faction = await Database.DatabaseMain.GetFactionInfo(p.factionId);
                //string rank = Factions.Faction.GetFactionRankString(p.factionRank, faction);
                // GlobalEvents.SetPlayerTag(p, "~q~" + rank, true);
                await MD_Equipment(p);
            }
        }

        public async Task FD_Equipment(PlayerModel p)
        {
            //if(p.Position.Distance(new Position(1195f, -1478f, 34f)) > 10) { MainChat.SendErrorChat(p, "[错误] Ekipman dolabına yakın olmalısınız."); return; }
            bool inFD = await CheckPlayerInFD(p);
            if (!inFD) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }

            FactionModel f = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            if (f == null) { MainChat.SendErrorChat(p, "[错误] 无效组织数据!"); return; }

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
                image = "./img/firedepartment_logo.png",
                guiMenu = gMenu,
                color = "#8E0D05"
            };
            y.Send(p);
        }

        public async Task MD_Equipment(PlayerModel p)
        {
            //if(p.Position.Distance(new Position(1195f, -1478f, 34f)) > 10) { MainChat.SendErrorChat(p, "[错误] Ekipman dolabına yakın olmalısınız."); return; }
            bool inMD = await CheckPlayerInMD(p);
            if (!inMD) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }

            FactionModel f = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            if (f == null) { MainChat.SendErrorChat(p, "[错误] 无效组织数据!"); return; }

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
                image = "./img/firedepartment_logo.png",
                guiMenu = gMenu,
                color = "#8E0D05"
            };
            y.Send(p);
        }



        [AsyncClientEvent("FD:Duty")]
        public void PD_Duty_SelectClothes(PlayerModel p, string value)
        {
            List<ClothingModel> x = new List<ClothingModel>();
            switch (value)
            {
                case "护理人员":
                    if (p.sex == 0)
                    {
                        x.Add(new ClothingModel() { cID = 3, iID = 109, tID = 0 });
                        x.Add(new ClothingModel() { cID = 4, iID = 34, tID = 0 });
                        x.Add(new ClothingModel() { cID = 6, iID = 25, tID = 0 });
                        x.Add(new ClothingModel() { cID = 7, iID = 97, tID = 0 });
                        x.Add(new ClothingModel() { cID = 8, iID = 189, tID = 0 });
                        x.Add(new ClothingModel() { cID = 10, iID = 66, tID = 0 });
                        x.Add(new ClothingModel() { cID = 11, iID = 258, tID = 0 });
                    }
                    else
                    {
                        x.Add(new ClothingModel() { cID = 3, iID = 85, tID = 0 });
                        x.Add(new ClothingModel() { cID = 4, iID = 35, tID = 0 });
                        x.Add(new ClothingModel() { cID = 6, iID = 25, tID = 0 });
                        x.Add(new ClothingModel() { cID = 7, iID = 127, tID = 0 });
                        x.Add(new ClothingModel() { cID = 8, iID = 153, tID = 0 });
                        x.Add(new ClothingModel() { cID = 10, iID = 58, tID = 0 });
                        x.Add(new ClothingModel() { cID = 11, iID = 250, tID = 0 });
                    }
                    break;

                case "护理人员2":
                    if (p.sex == 0)
                    {
                        x.Add(new ClothingModel() { cID = 3, iID = 109, tID = 1 });
                        x.Add(new ClothingModel() { cID = 4, iID = 34, tID = 0 });
                        x.Add(new ClothingModel() { cID = 6, iID = 25, tID = 0 });
                        x.Add(new ClothingModel() { cID = 7, iID = 97, tID = 0 });
                        x.Add(new ClothingModel() { cID = 8, iID = 189, tID = 0 });
                        x.Add(new ClothingModel() { cID = 10, iID = 73, tID = 0 });
                        x.Add(new ClothingModel() { cID = 11, iID = 258, tID = 0 });
                    }
                    else
                    {
                        x.Add(new ClothingModel() { cID = 3, iID = 85, tID = 1 });
                        x.Add(new ClothingModel() { cID = 4, iID = 35, tID = 0 });
                        x.Add(new ClothingModel() { cID = 6, iID = 25, tID = 0 });
                        x.Add(new ClothingModel() { cID = 7, iID = 127, tID = 0 });
                        x.Add(new ClothingModel() { cID = 8, iID = 153, tID = 0 });
                        x.Add(new ClothingModel() { cID = 10, iID = 64, tID = 0 });
                        x.Add(new ClothingModel() { cID = 11, iID = 250, tID = 0 });
                    }
                    break;

                case "消防员":
                    if (p.sex == 0)
                    {
                        x.Add(new ClothingModel() { cID = 3, iID = 14, tID = 0 });
                        x.Add(new ClothingModel() { cID = 4, iID = 126, tID = 0 });
                        x.Add(new ClothingModel() { cID = 6, iID = 52, tID = 0 });
                        x.Add(new ClothingModel() { cID = 11, iID = 73, tID = 1 });
                        x.Add(new ClothingModel() { cID = 10, iID = 73, tID = 0 });
                        x.Add(new ClothingModel() { cID = 8, iID = 189, tID = 0 });
                    }
                    else
                    {
                        x.Add(new ClothingModel() { cID = 3, iID = 0, tID = 0 });
                        x.Add(new ClothingModel() { cID = 4, iID = 120, tID = 0 });
                        x.Add(new ClothingModel() { cID = 6, iID = 25, tID = 0 });
                        x.Add(new ClothingModel() { cID = 8, iID = 153, tID = 0 });
                        x.Add(new ClothingModel() { cID = 10, iID = 64, tID = 0 });
                        x.Add(new ClothingModel() { cID = 11, iID = 242, tID = 4 });
                    }
                    break;

                case "消防员2":
                    if (p.sex == 0)
                    {
                        x.Add(new ClothingModel() { cID = 7, iID = 97, tID = 0 });
                        x.Add(new ClothingModel() { cID = 4, iID = 126, tID = 0 });
                        x.Add(new ClothingModel() { cID = 6, iID = 52, tID = 0 });
                        x.Add(new ClothingModel() { cID = 10, iID = 73, tID = 0 });
                        x.Add(new ClothingModel() { cID = 8, iID = 189, tID = 0 });
                        x.Add(new ClothingModel() { cID = 3, iID = 109, tID = 1 });
                        x.Add(new ClothingModel() { cID = 11, iID = 258, tID = 0 });
                    }
                    else
                    {
                        x.Add(new ClothingModel() { cID = 3, iID = 85, tID = 0 });
                        x.Add(new ClothingModel() { cID = 4, iID = 120, tID = 0 });
                        x.Add(new ClothingModel() { cID = 6, iID = 25, tID = 0 });
                        x.Add(new ClothingModel() { cID = 8, iID = 153, tID = 0 });
                        x.Add(new ClothingModel() { cID = 7, iID = 127, tID = 0 });
                        x.Add(new ClothingModel() { cID = 10, iID = 64, tID = 0 });
                        x.Add(new ClothingModel() { cID = 11, iID = 250, tID = 0 });
                    }
                    break;

                case "消防员3":
                    if (p.sex == 0)
                    {
                        x.Add(new ClothingModel() { cID = 3, iID = 158, tID = 0 });
                        x.Add(new ClothingModel() { cID = 4, iID = 126, tID = 0 });
                        x.Add(new ClothingModel() { cID = 6, iID = 52, tID = 0 });
                        x.Add(new ClothingModel() { cID = 8, iID = 187, tID = 0 });
                        x.Add(new ClothingModel() { cID = 10, iID = 73, tID = 0 });
                        x.Add(new ClothingModel() { cID = 11, iID = 326, tID = 0 });
                    }
                    else
                    {
                        x.Add(new ClothingModel() { cID = 3, iID = 72, tID = 0 });
                        x.Add(new ClothingModel() { cID = 4, iID = 120, tID = 0 });
                        x.Add(new ClothingModel() { cID = 6, iID = 51, tID = 0 });
                        x.Add(new ClothingModel() { cID = 8, iID = 151, tID = 0 });
                        x.Add(new ClothingModel() { cID = 11, iID = 315, tID = 0 });
                    }
                    break;

                case "正式制服":
                    if (p.sex == 0)
                    {
                        x.Add(new ClothingModel() { cID = 3, iID = 14, tID = 0 });
                        x.Add(new ClothingModel() { cID = 4, iID = 34, tID = 0 });
                        x.Add(new ClothingModel() { cID = 6, iID = 25, tID = 0 });
                        x.Add(new ClothingModel() { cID = 11, iID = 327, tID = 0 });
                        x.Add(new ClothingModel() { cID = 7, iID = 87, tID = 9 });
                    }
                    else
                    {
                        x.Add(new ClothingModel() { cID = 3, iID = 1, tID = 0 });
                        x.Add(new ClothingModel() { cID = 4, iID = 35, tID = 0 });
                        x.Add(new ClothingModel() { cID = 6, iID = 25, tID = 0 });
                        x.Add(new ClothingModel() { cID = 11, iID = 316, tID = 8 });
                        x.Add(new ClothingModel() { cID = 7, iID = 38, tID = 0 });
                    }
                    break;

                case "指挥官制服":
                    if (p.sex == 0)
                    {
                        x.Add(new ClothingModel() { cID = 10, iID = 73, tID = 0 });
                        x.Add(new ClothingModel() { cID = 11, iID = 327, tID = 5 });
                        x.Add(new ClothingModel() { cID = 6, iID = 25, tID = 0 });
                        x.Add(new ClothingModel() { cID = 7, iID = 22, tID = 0 });
                        x.Add(new ClothingModel() { cID = 8, iID = 189, tID = 0 });
                        x.Add(new ClothingModel() { cID = 3, iID = 14, tID = 0 });
                        x.Add(new ClothingModel() { cID = 4, iID = 34, tID = 0 });
                    }
                    else
                    {
                        x.Add(new ClothingModel() { cID = 10, iID = 64, tID = 0 });
                        x.Add(new ClothingModel() { cID = 11, iID = 318, tID = 5 });
                        x.Add(new ClothingModel() { cID = 8, iID = 153, tID = 0 });
                        x.Add(new ClothingModel() { cID = 3, iID = 0, tID = 0 });
                        x.Add(new ClothingModel() { cID = 7, iID = 38, tID = 0 });
                        x.Add(new ClothingModel() { cID = 6, iID = 25, tID = 0 });
                        x.Add(new ClothingModel() { cID = 4, iID = 35, tID = 0 });
                    }
                    break;
            }
            SetClothSet(p, x);
            p.EmitLocked("GUI:Close");
        }

        public async Task SendWarningFD(string text)
        {
            List<int> a = await Database.DatabaseMain.GetTypeOfFactionIDs(ServerGlobalValues.fType_FD);

            foreach (int fId in a)
            {
                foreach (PlayerModel p in Alt.GetAllPlayers())
                {
                    if (p.factionId == fId)
                    {
                        p.SendChatMessage(text);
                    }
                }
            }
        }

        // Revive - Heal Player
        [Command("checkbody")]
        public async Task InspectPlayer(PlayerModel p, params string[] args)
        {
            if (await canUseHospital(p) == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /checkbody [id]"); return; }
            if (!Int32.TryParse(args[0], out int tSql)) { MainChat.SendInfoChat(p, "[用法] /checkbody [id]"); return; }
            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }
            p.SendChatMessage("___[" + t.fakeName.Replace("_", " ") + "]___");

            string total = "健康";
            if (t.injured.head || t.injured.torso || t.injured.arms || t.injured.legs) { total = "Orta"; }
            else if (t.injured.isDead) { total = "重伤/死亡"; }
            p.SendChatMessage("总体健康状况: " + total);

            string head = (t.injured.head) ? "流血" : "健康";
            p.SendChatMessage("头部: " + head);

            string torso = (t.injured.torso) ? "流血" : "健康";
            p.SendChatMessage("躯干: " + torso);

            string arms = (t.injured.arms) ? "流血" : "健康";
            p.SendChatMessage("手臂: " + arms);

            string legs = (t.injured.legs) ? "流血" : "健康";
            p.SendChatMessage("腿部: " + legs);
        }

        [Command("tblood")]
        public async Task InpectPlayerDisease(PlayerModel p, params string[] args)
        {
            if (await canUseHospital(p) == false && await PD.CheckPlayerInPd(p) == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /tblood [ID]"); return; }
            if (!Int32.TryParse(args[0], out int tSql)) { MainChat.SendInfoChat(p, "[用法] /tblood [ID]"); return; }
            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }
            if (p.Position.Distance(t.Position) > 3) { MainChat.SendErrorChat(p, CONSTANT.ERR_NotNearTarget); return; }

            ServerItems bTube = Items.LSCitems.Find(x => x.ID == 26);
            bTube.name = t.fakeName.Replace("_", " ") + " 的采血管";
            bTube.data = t.sqlID.ToString();

            bool succes = await Inventory.AddInventoryItem(p, bTube, 1);
            if (succes) { MainChat.SendInfoChat(p, "[信息] 您采样了 " + t.fakeName.Replace("_", " ") + " 的血液(( 作为一名医务人员请合理扮演 ))"); return; }
            else { MainChat.SendErrorChat(p, "[错误] 您的背包已经满了."); return; }
        }

        [Command("rblood")]
        public async Task GetPlayerDiseaseResult(PlayerModel p)
        {
            if (await canUseHospital(p) == false && await PD.CheckPlayerInPd(p) == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            Crate x = new Crate();
            foreach (var c in CrateEvents.serverCrates)
            {
                if (p.Position.Distance(c.pos) < 5f) { x = c; break; }
            }
            if (x == null) { MainChat.SendErrorChat(p, "[错误] 附近没有血液分析设备"); return; }
            if (x.type != 4) { MainChat.SendErrorChat(p, "[错误] 此设备不是血液分析设备"); return; }
            if (x.owner != p.businessStaff) { MainChat.SendErrorChat(p, "此设备不属于您"); return; }
            List<InventoryModel> labEnv = JsonConvert.DeserializeObject<List<InventoryModel>>(x.value);

            List<GuiMenu> gMenu = new List<GuiMenu>();
            foreach (var i in labEnv)
            {
                //DateTime finishTime = Convert.ToDateTime(i.data2);
                /*if (finishTime > DateTime.Now)
                {*/
                if (!Int32.TryParse(i.itemData, out int tSql))
                    return;

                PlayerModelInfo t = await Database.DatabaseMain.getCharacterInfo(tSql);
                if (t != null)
                {
                    GuiMenu bloodResult = new GuiMenu { name = t.characterName.Replace("_", " "), triger = "BloodLab:GetResult", value = t.sqlID.ToString() };
                    gMenu.Add(bloodResult);
                }
                //}
            }

            GuiMenu close = GuiEvents.closeItem;

            gMenu.Add(close);
            Gui y = new Gui()
            {
                image = "https://cdn.discordapp.com/attachments/728461562104512533/750854488222466098/PinClipart.com_lifeguard-clipart_51320.png",
                guiMenu = gMenu,
                color = "#4AC27D"
            };
            y.Send(p);
        }

        [AsyncClientEvent("BloodLab:GetResult")]
        public void GetBloodResult(PlayerModel p, int id)
        {
            Crate x = CrateEvents.CheckNearCrate(p);
            if (x == null) { MainChat.SendErrorChat(p, "[错误] 附近没有血液分析设备"); return; }
            if (x.type != 4) { MainChat.SendErrorChat(p, "[错误] 此设备不是血液分析设备"); return; }
            if (x.owner != p.businessStaff) { MainChat.SendErrorChat(p, "此设备不属于您"); return; }
            List<InventoryModel> labEnv = JsonConvert.DeserializeObject<List<InventoryModel>>(x.value);

            InventoryModel RemoveItem = null;
            foreach (var e in labEnv)
            {
                if (e.itemData == id.ToString()) { RemoveItem = e; break; }
            }

            labEnv.Remove(RemoveItem);
            x.value = JsonConvert.SerializeObject(labEnv);
            x.Update();

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(id);
            if (t == null) { return; }
            string diseases = "";
            if (t.injured.diseases.Count >= 1)
            {
                foreach (var tD in t.injured.diseases)
                {
                    string push = "[病症: " + tD.DiseaseName + " | 程度: " + tD.DiseaseValue + "/10]";
                    diseases += push;
                }
            }
            else
            {
                diseases = "血液分析结果是正常的.";
            }

            p.SendChatMessage(diseases);
        }

        [Command("giverest")]
        public async Task COM_GiveRest(PlayerModel p, params string[] args)
        {
            if (await canUseHospital(p) == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /giverest [id] [body/disease/general] [副参数]<br>可用副参数 body - 身体: 头部 - 躯干 - 手臂 - 腿部<br>disease - 病症: [病症名称]"); return; }
            if (!Int32.TryParse(args[0], out int tSql)) { MainChat.SendInfoChat(p, "[用法] /giverest [id] [body/disease/general] [副参数]<br>可用副参数 body - 身体: 头部 - 躯干 - 手臂 - 腿部<br>disease - 病症: [病症名称]"); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);

            if (t == null) { MainChat.SendErrorChat(p, CONSTANT.ERR_PlayerNotFound); return; }
            if (p.Position.Distance(t.Position) > 3) { MainChat.SendErrorChat(p, CONSTANT.ERR_NeedNearPlayer); return; }

            switch (args[1])
            {
                case "body":
                    switch (args[2])
                    {
                        case "头部":
                            if (!t.injured.head) { MainChat.SendErrorChat(p, "指定玩家头部是健康的"); return; }
                            t.injured.head = false;

                            CheckHealthStatus(t);
                            GlobalEvents.ProgresBar(p, "正在尝试治疗.", 5);
                            return;

                        case "躯干":
                            if (!t.injured.torso) { MainChat.SendErrorChat(p, "指定玩家躯干是健康的"); return; }
                            t.injured.torso = false;

                            CheckHealthStatus(t);
                            GlobalEvents.ProgresBar(p, "正在尝试治疗.", 5);
                            break;

                        case "手臂":
                            if (!t.injured.arms) { MainChat.SendErrorChat(p, "指定玩家手臂是健康的"); return; }
                            t.injured.arms = false;

                            CheckHealthStatus(t);
                            GlobalEvents.ProgresBar(p, "正在尝试治疗.", 5);
                            break;

                        case "腿部":
                            if (!t.injured.legs) { MainChat.SendErrorChat(p, "指定玩家腿部是健康的"); return; }
                            t.injured.legs = false;

                            CheckHealthStatus(t);
                            GlobalEvents.ProgresBar(p, "正在尝试治疗.", 5);
                            break;

                        default:
                            return;
                    }
                    break;

                case "disease":
                    List<DiseaseModel> tDiseases = t.injured.diseases;
                    var disease = tDiseases.Find(x => x.DiseaseName == string.Join(" ", args[2..]));
                    if (disease == null) { MainChat.SendErrorChat(p, "未发现病症"); return; }

                    disease.DiseaseValue -= 1;
                    if (disease.DiseaseValue <= 0) { tDiseases.Remove(disease); }
                    t.injured.diseases = tDiseases;

                    GlobalEvents.ProgresBar(p, "正在尝试治疗.", 5);
                    break;

                case "general":
                    if (t.Health >= 999) { MainChat.SendErrorChat(p, "指定玩家是健康的"); return; }
                    t.Health = 1000;
                    t.hp = 1000;
                    await t.SetMaxHealthAsync(1000);
                    if (t.Health > 1000) { t.Health = 1000; }

                    GlobalEvents.ProgresBar(p, "正在尝试治疗.", 5);
                    break;
            }
            t.EmitLocked("Injured:ClearBloods");
        }
        public void CheckHealthStatus(PlayerModel p)
        {
            if (!p.injured.arms || !p.injured.head || !p.injured.torso || !p.injured.legs)
            {
                p.injured.isDead = false;
                p.injured.Injured = false;
                GlobalEvents.ClearPlayerTag(p);
                GlobalEvents.UpdateInjured(p);
            }
        }
        public async Task<bool> canUseHospital(PlayerModel p)
        {
            return await Props.Business.GetPlayerBusinessType(p, ServerGlobalValues.hospitalBusiness);
        }

        //[Command("tedaviol")]
        //public async Task COM_SelfRevive(PlayerModel p)
        //{
        //    if (p.Position.Distance(new Position(-448, -342, 34)) > 5) { MainChat.SendErrorChat(p, "[错误] Bu komutu kullanmak için tedavi bölgesinde olmalısınız."); return; }

        //    if (p.cash < 1000) { MainChat.SendErrorChat(p, CONSTANT.ERR_MoneyNotEnought); return; }

        //    int totalParamedic = 0;
        //    foreach (PlayerModel t in Alt.GetAllPlayers())
        //    {
        //        if (t.HasData(EntityData.PlayerEntityData.MDDuty)) { totalParamedic++; }
        //    }

        //    if (totalParamedic >= 5) { MainChat.SendInfoChat(p, "[!] Toplamda 5 sağlık çalışanı aktifken tedavi olamazsınız!"); return; }

        //    p.injured.Injured = false;
        //    p.injured.isDead = false;
        //    await p.SetMaxHealthAsync(1000);
        //    p.hp = 1000;
        //    p.Health = 1000;
        //    GlobalEvents.UpdateInjured(p);
        //    p.cash -= 1000;
        //    await p.updateSql();
        //    p.EmitLocked("Injured:ClearBloods");
        //    return;
        //}

        [Command("fdcalls")]
        public async Task COM_CheckCalls(PlayerModel p)
        {
            bool inFD = await CheckPlayerInFD(p);
            if (!inFD) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            string text = "<center>呼叫列表</center>";
            foreach (fdCalls call in serverFDCalls)
            {
                text += "<br>呼叫编号: " + call.tSql.ToString() + " - 内容: " + call.Mesagge;
            }
            if (text != "<center>呼叫列表</center>")
            {
                text += "<br>如需回应呼叫, 请输入 /fdacall [呼叫编号]";
            }

            MainChat.SendInfoChat(p, text, true);
            return;
        }

        [Command("fdacall")]
        public async Task COM_AnswerCall(PlayerModel p, params string[] args)
        {
            bool inFD = await CheckPlayerInFD(p);
            if (!inFD) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /fdacall [id]"); return; }
            int tSql; bool isOK = Int32.TryParse(args[0], out tSql);

            if (!isOK) { MainChat.SendInfoChat(p, "[用法] /fdacall [id]"); return; }

            fdCalls curr = serverFDCalls.Find(x => x.tSql == tSql);
            if (curr == null) { MainChat.SendErrorChat(p, "[错误] 无效呼叫."); return; }

            GlobalEvents.CheckpointCreate(p, curr.pos, 21, 5, new Rgba(255, 0, 0, 150), "", "");
            MainChat.SendInfoChat(p, "> 呼叫者位置已标记至小地图.");
            serverFDCalls.Remove(curr);
            return;
        }

        [Command("fr")]
        public async Task COM_FDRadio(PlayerModel p, params string[] args)
        {
            bool inFD = await CheckPlayerInFD(p);
            if (!inFD) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /ft [文本]"); return; }
            string message = string.Join(" ", args);

            var Team = fdTeams.Find(x => x.leaderId == p.sqlID || x.memberId == p.sqlID);
            string TeamName = "";
            if (Team != null) { TeamName = Team.name; }

            MainChat.NormalChat(p, "[对讲机] " + message);
            var faction = await Database.DatabaseMain.GetFactionInfo(p.factionId);
            string Head = "{DA615E} LSFD 对讲机 | ";
            if (faction.type == ServerGlobalValues.fType_MD)
                Head = "{DA615E} MZMC 对讲机 |";
            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (t.factionId == p.factionId)
                {
                    MainChat.SendInfoChat(t, Head + p.characterName.Replace("_", " ") + " [" + TeamName + "]: " + message);
                }
            }
        }

        [Command("emscall")]
        public static void COM_CallDoctor(PlayerModel p)
        {
            if (p.Position.Distance(new(338, -587, 28)) > 5) { MainChat.SendErrorChat(p, "[错误] 您不在呼叫医生点."); return; }

            int count = 0;
            foreach (PlayerModel fd in Alt.GetAllPlayers())
            {
                if (fd.HasData(EntityData.PlayerEntityData.FDDuty)) { count++; fd.SendChatMessage("{e84577}[911] 收到新的呼叫医生.<br>来电者: " + p.characterName.Replace('_', ' ')); }
            }

            MainChat.SendInfoChat(p, "[?] 您的呼叫医生已转接至相关部门, 目前在线医护人员: " + count);
            return;
        }

        public class Medicine
        {
            public bool isAdd { get; set; } = false;
            public string name { get; set; } = "";
            public int _str { get; set; } = 0;
        } // [{isAdd: 0, name: "Grip", _str: 0}]

        public static void useMedicine(PlayerModel p, string stat)
        {
            List<Medicine> y = JsonConvert.DeserializeObject<List<Medicine>>(stat);
            if (y == null)
                return;

            foreach (Medicine i in y)
            {
                if (i.isAdd)
                {
                    DiseaseModel d = p.injured.diseases.Find(x => x.DiseaseName == i.name);
                    if (d == null)
                    {
                        p.injured.diseases.Add(new DiseaseModel
                        {
                            DiseaseName = i.name,
                            DiseaseValue = 1,
                        });
                    }
                    else
                    {
                        d.DiseaseValue += 1;
                    }
                }
                else
                {
                    DiseaseModel g = p.injured.diseases.Find(x => x.DiseaseName == i.name);
                    if (g != null)
                    {
                        g.DiseaseValue -= 1;
                        if (g.DiseaseValue <= 0)
                        {
                            p.injured.diseases.Remove(g);
                        }
                    }



                }

                p.tempStrength += i._str;
            }

            MainChat.SendInfoChat(p, "[!] 您使用了药物.");
        }


        public class FDTeamModel
        {
            public string name { get; set; }
            public int leaderId { get; set; }
            public int memberId { get; set; }
        }

        public static List<FDTeamModel> fdTeams = new List<FDTeamModel>();

        [Command("fdunit")]
        public static async Task COM_PDCreateTeam(PlayerModel player, params string[] values)
        {
            if (values.Length <= 1) { MainChat.SendInfoChat(player, CONSTANT.DESC_CreateTeam); return; }
            bool pdCheck = await CheckPlayerInFD(player);
            if (pdCheck == false) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotInPd); return; }
            var playerTeam = fdTeams.Find(x => x.leaderId == player.sqlID && x.memberId == player.sqlID);
            if (playerTeam != null) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerInTeam); return; }
            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(Int32.Parse(values[0].ToString()));
            if (target == null) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotFound); return; }
            var targetTeam = fdTeams.Find(x => x.leaderId == target.sqlID && x.memberId == target.sqlID);
            if (targetTeam != null) { MainChat.SendErrorChat(player, string.Format(CONSTANT.ERR_TargetInTeam, target.fakeName.Replace("_", " "))); return; }
            bool targetpdCheck = await CheckPlayerInFD(target);
            if (targetpdCheck == false) { MainChat.SendErrorChat(player, string.Format(CONSTANT.ERR_TargetNotInPD, target.fakeName.Replace("_", " "))); return; }

            fdTeams.Add(new FDTeamModel { name = values[1].ToString(), leaderId = player.sqlID, memberId = target.sqlID });
            MainChat.SendInfoChat(player, string.Format(CONSTANT.INFO_CreateTeamPlayerSucces, values[1].ToString()));
            MainChat.SendInfoChat(target, string.Format(CONSTANT.INFO_CreateTeamTargetSucces, player.fakeName.Replace("_", " "), values[1].ToString()));
            return;
        }

        [Command("dfdunit")]
        public static async Task COM_PDDestroyTeam(PlayerModel player, params string[] notUsed)
        {
            bool pdCheck = await CheckPlayerInFD(player);
            if (pdCheck == false) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotInPd); return; }
            var team = fdTeams.Find(x => x.leaderId == player.sqlID || x.memberId == player.sqlID);
            if (team == null) { MainChat.SendErrorChat(player, CONSTANT.ERR_PDTeamNotFound); return; }
            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(team.leaderId);
            if (target == player) { target = GlobalEvents.GetPlayerFromSqlID(team.memberId); }
            fdTeams.Remove(team);
            MainChat.SendInfoChat(player, CONSTANT.INFO_PDDestroyTeam);
            MainChat.SendInfoChat(target, CONSTANT.INFO_PDDestroyTeam);
            return;
        }

        [Command("fdunitlist")]
        public static async Task COM_PDTeamList(PlayerModel player)
        {
            bool pdCheck = await CheckPlayerInFD(player);
            if (pdCheck == false) { MainChat.SendErrorChat(player, CONSTANT.ERR_PlayerNotInPd); return; }
            FactionModel fact = await Database.DatabaseMain.GetFactionInfo(player.factionId);

            MainChat.SendInfoChat(player, "--------------[单位列表]------------");
            foreach (FDTeamModel x in fdTeams)
            {
                PlayerModel leader = GlobalEvents.GetPlayerFromSqlID(x.leaderId);
                PlayerModel member = GlobalEvents.GetPlayerFromSqlID(x.memberId);
                if (leader == null || member == null)
                {
                    fdTeams.Remove(x);
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

        [Command("fdequip")]
        public async Task COM_FDEquipment(PlayerModel p)
        {
            bool pdCheck = await CheckPlayerInFD(p);
            if (!pdCheck) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v == null) { MainChat.SendErrorChat(p, "[错误] 附近没有车辆!"); return; }
            if (v.factionId != p.factionId) { MainChat.SendErrorChat(p, "[错误] 此车辆不属于您的组织!"); return; }

            p.GiveWeapon(WeaponModel.Crowbar, 1, false);
            p.GiveWeapon(WeaponModel.Flashlight, 1, false);
            p.GiveWeapon(WeaponModel.BattleAxe, 1, false);
            p.GiveWeapon(WeaponModel.Flare, 20, false);
            p.GiveWeapon(WeaponModel.FireExtinguisher, 2000, true);

            MainChat.SendInfoChat(p, "[?] 您从车上取出了 LSFD 装备.");
            return;
        }

    }


    public class FD_FireSystem : IScript
    {
        public FD_FireSystem()
        {
            Alt.Log("加载 LSFD Fire 系统.");
        }
        public static bool canFire = false;
        public class FireModel
        {
            public ulong ID { get; set; }
            public Position pos { get; set; } = new Position(0, 0, 0);
            public int Dimension { get; set; } = 0;
            public int Health { get; set; }
        }

        public static List<FireModel> fires = new();

        public static FireModel GetNearFire(Position pos, int dimension, int range = 5)
        {
            return fires
                .Where(x => x.pos.Distance(pos) < range && x.Dimension == dimension)
                .OrderBy(x => x.pos.Distance(pos))
                .FirstOrDefault();
        }

        [Command("astartfire")]
        public static void COM_StartFire(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            if (!Int32.TryParse(args[0], out int health) || !Boolean.TryParse(args[1], out bool isGas) || !Int32.TryParse(args[2], out int child)) { MainChat.SendInfoChat(p, "[用法] /astartfire [血量(1-100)] [毒气(0-1)] [最大Child(1-10)]"); return; }
            Position firePos = p.Position;
            firePos.Z -= 0.7f;
            FireModel newFire = new()
            {
                pos = firePos,
                Dimension = p.Dimension,
                Health = health
            };

            var fire = FireStreamer.Create(firePos, p.Dimension, isGas, child);
            newFire.ID = fire.Id;
            fires.Add(newFire);
            MainChat.SendInfoChat(p, "成功创建火源.");
            return;
        }

        public static void EventAddFire(Position pos, int dimension)
        {
            if (!canFire)
                return;
            var check = GetNearFire(pos, dimension, 1);
            if (check == null)
            {
                FireModel newFire = new()
                {
                    pos = pos,
                    Dimension = dimension,
                    Health = 10
                };

                if (dimension > 0)
                    return;

                var fire = FireStreamer.Create(pos, dimension, false, 1, 50);
                newFire.ID = fire.Id;
                fires.Add(newFire);

                var checkIhbar = FD.serverFDCalls.Where(x => x.pos.Distance(pos) < 50).FirstOrDefault();
                if (checkIhbar == null)
                {
                    FD.serverFDCalls.Add(new()
                    {
                        tSql = (int)newFire.ID,
                        Mesagge = "火灾",
                        pos = pos
                    });

                    foreach (PlayerModel fd in Alt.GetAllPlayers())
                    {
                        if (fd.HasData(EntityData.PlayerEntityData.FDDuty)) { fd.SendChatMessage("{e84577}[911] 收到新的紧急呼救.<br>情况: 火警"); }
                    }
                }
            }
        }

        [Command("astopfire")]
        public static void COM_RemoveFire(PlayerModel p)
        {
            if (p.adminLevel <= 4) { MainChat.SendInfoChat(p, "[错误] 无权操作!"); return; }
            var fire = GetNearFire(p.Position, p.Dimension);
            if (fire == null)
            {
                MainChat.SendErrorChat(p, "[错误] 附近没有火源!");
                return;
            }

            fires.Remove(fire);
            var obj = FireStreamer.GetFire(fire.ID);
            if (obj != null)
            {
                FireStreamer.Delete(obj);
            }

            MainChat.SendInfoChat(p, "[!] 成功清理火源");
            return;
        }

        public bool descreaseHealth(Position pos, int dimension, int range = 5)
        {

            var fire = GetNearFire(pos, dimension, range);
            if (fire != null)
            {
                fire.Health -= 1;
                if (fire.Health <= 0)
                {
                    fires.Remove(fire);
                    var obj = FireStreamer.GetFire(fire.ID);
                    if (obj != null)
                    {
                        FireStreamer.Delete(obj);
                    }
                    return true;
                }
                else
                    return false;
            }
            else
                return false;

        }


        [AsyncClientEvent("Fire:StopFire")]
        public void FireStopFire(PlayerModel player)
        {

            if (player.Vehicle != null)
            {
                if (descreaseHealth(player.Position, player.Dimension, 20))
                {
                    MainChat.ADO(player, "火源灭了!");
                }
            }
            else
            {
                if (descreaseHealth(player.Position, player.Dimension, 5))
                {
                    MainChat.ADO(player, "火源灭了!");
                }
            }

            return;
            //WeaponModel.FireExtinguisher
            //WeaponModel.MolotovCocktail
        }

        [Command("astopallfires")]
        public void ComClearFires(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 1) { MainChat.SendInfoChat(p, "[!] 无权操作!"); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /astopallfires [范围(int)]"); return; }
            if (!Int32.TryParse(args[0], out int Range)) { MainChat.SendInfoChat(p, "[用法] /astopallfires [范围(int)]"); return; }

            bool oldFire = canFire;
            canFire = false;

            List<FireModel> removed = new();
            foreach (var fire in fires)
            {
                if (fire.pos.Distance(p.Position) <= Range && fire.Dimension == p.Dimension)
                {
                    removed.Add(fire);
                }
            }

            removed.ForEach(x =>
            {
                fires.Remove(x);
                var obj = FireStreamer.GetFire(x.ID);
                if (obj != null)
                {
                    FireStreamer.Delete(obj);
                }
            });

            foreach (var t in Alt.GetAllPlayers())
            {//
                if (p.Position.Distance(t.Position) <= 250 && t.Dimension == p.Dimension)
                    t.EmitLocked("ClearFires", p.Position, Range);
            }

            MainChat.SendInfoChat(p, "[!] 范围 " + Range.ToString() + " 内的火源被 OOC 熄灭了");
            return;
        }

        [Command("afire")]
        public static void COM_FireChange(PlayerModel p)
        {
            if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            canFire = !canFire;

            MainChat.SendAdminChat(p.characterName + " 的火源权限 " + ((canFire) ? "开启了" : "关闭了"));
            return;
        }

    }


}