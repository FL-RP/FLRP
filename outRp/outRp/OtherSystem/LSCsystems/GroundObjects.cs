using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Globals;
using outRp.Models;
using outRp.OtherSystem.Textlabels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;

namespace outRp.OtherSystem.LSCsystems
{
    public class GroundObjects : IScript
    {
        public class ServerObjects
        {
            public int ID { get; set; } = 0;
            public string model { get; set; } = "无";
            public Vector3 Position { get; set; } = new Position();
            public Vector3 rotation { get; set; } = new Rotation();
            public int Dimension { get; set; } = 0;
            public bool isDynamic { get; set; } = false;
            public bool isFire { get; set; } = false;
            public bool isLight { get; set; } = false;
            public Rgb color { get; set; } = new Rgb(0, 0, 0);
            public int Owner { get; set; } = 0;
        }

        public static List<ServerObjects> serverObjects = new List<ServerObjects>();

        public static void LoadServerObjects(string data)
        {
            serverObjects = JsonConvert.DeserializeObject<List<ServerObjects>>(data);

            foreach (ServerObjects obj in serverObjects)
            {
                if (obj.isLight) { obj.ID = (int)PropStreamer.Create(obj.model, obj.Position, obj.rotation, dimension: obj.Dimension, frozen: true, isDynamic: obj.isDynamic, onFire: obj.isFire, lightColor: obj.color).Id; }
                else { obj.ID = (int)PropStreamer.Create(obj.model, obj.Position, obj.rotation, dimension: obj.Dimension, frozen: true, isDynamic: obj.isDynamic, onFire: obj.isFire).Id; }
            }
        }

        [Command("clearobjects")]
        public async Task COM_ClearObjects(PlayerModel p)
        {
            if (p.adminLevel <= 6) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); }
            int total = 0;

            foreach (ServerObjects obj in serverObjects.ToList())
            {
                if (obj.Owner != 0)
                {
                    PlayerModelInfo owner = await Database.DatabaseMain.getCharacterInfo(obj.Owner);
                    if (owner == null)
                    {
                        PropStreamer.GetProp((ulong)obj.ID).Delete();
                        serverObjects.Remove(obj);
                        ++total;
                    }
                    else if (bannedObjects.Contains(obj.model))
                    {
                        PropStreamer.GetProp((ulong)obj.ID).Delete();
                        serverObjects.Remove(obj);
                        ++total;
                    }
                    else if (owner.isCk)
                    {
                        PropStreamer.GetProp((ulong)obj.ID).Delete();
                        serverObjects.Remove(obj);
                        ++total;
                    }
                    else
                    {
                        AccountModel acc = await Database.DatabaseMain.getAccInfo(owner.accountId);
                        if (acc.banned)
                        {
                            PropStreamer.GetProp((ulong)obj.ID).Delete();
                            serverObjects.Remove(obj);
                            ++total;
                        }
                    }
                }
            }

            MainChat.SendInfoChat(p, "[?] 成功清理总计 " + total + " 服务器对象!");
        }

        [Command("nearobj")]
        public void ShowNearObjects(PlayerModel p, params string[] args)
        {
            if(args.Length <= 0)
            {
                string text = "";
                foreach (ServerObjects obj in serverObjects)
                {
                    if (p.Position.Distance(obj.Position) < 30 && obj.Dimension == p.Dimension)
                    {
                        text += "<br>[ID: " + obj.ID + "] 模型: " + obj.model + " - 所有者: " + obj.Owner + " - 距离: " + p.Position.Distance(obj.Position) + "f";
                    }
                }

                MainChat.SendInfoChat(p, "<center>附近对象</center>" + text);
                return;
            } else {
                if(!Int32.TryParse(args[0], out int range)) {MainChat.SendInfoChat(p, "[用法] /nearobj [距离]"); return; }
                if(range > 100 || range < 0) {MainChat.SendErrorChat(p, "[错误] 距离为1-100!"); return; }
                string text = "";
                foreach (ServerObjects obj in serverObjects)
                {
                    if (p.Position.Distance(obj.Position) < range && obj.Dimension == p.Dimension)
                    {
                        text += "<br>[ID: " + obj.ID + "] 模型: " + obj.model + " - 所有者: " + obj.Owner + " - 距离: " + p.Position.Distance(obj.Position) + "f";
                    }
                }

                MainChat.SendInfoChat(p, "<center>附近对象</center>" + text);
                return;
            }

            
        }

        [Command("addobject")]
        public void CreateObject(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 4)
                return;

            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /addobject [模型]"); return; }
            GlobalEvents.ShowObjectPlacement(p, args[0], "Admin:PlaceObject");
            return;
        }

        [AsyncClientEvent("Admin:PlaceObject")]
        public void Admin_AddObject(PlayerModel p, string rot, string pos, string model)
        {
            Vector3 position = JsonConvert.DeserializeObject<Vector3>(pos);
            Vector3 rotation = JsonConvert.DeserializeObject<Vector3>(rot);

            ServerObjects nObj = new ServerObjects()
            {
                ID = (int)PropStreamer.Create(model, position, rotation, dimension: p.Dimension, frozen: true).Id,
                Dimension = p.Dimension,
                model = model,
                Position = position,
                rotation = rotation
            };

            serverObjects.Add(nObj);
            MainChat.SendInfoChat(p, "已创建对象: " + nObj.ID);
            return;
        }

        [Command("deleteobj")]
        public void DeleteObject(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 4)
                return;

            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /deleteobj [id]"); return; }

            ServerObjects dObject = serverObjects.Find(x => x.ID == Int32.Parse(args[0]));
            if (dObject == null) { MainChat.SendErrorChat(p, "[错误] 无效对象."); return; }


            PropStreamer.GetProp((ulong)dObject.ID).Delete();
            serverObjects.Remove(dObject);
            MainChat.SendInfoChat(p, "已删除指定对象.");
            return;

        }

        [Command("editobject")]
        public static void COM_EditObject(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /editobject [选项] [数值]"); return; }

            int objectID; bool isOK = Int32.TryParse(args[0], out objectID);
            if (!isOK) { MainChat.SendInfoChat(p, "[用法] /objededitobjectuzenle [选项] [数值]"); return; }

            ServerObjects obj = serverObjects.Find(x => x.ID == objectID);
            if (obj == null) { MainChat.SendErrorChat(p, "[错误] 无效对象."); return; }

            LProp o = PropStreamer.GetProp((ulong)obj.ID);
            if (o == null) { MainChat.SendErrorChat(p, "[错误] 无效对象."); return; }

            switch (args[1])
            {
                case "dynamic":
                    obj.isDynamic = !obj.isDynamic;
                    o.Dynamic = obj.isDynamic;
                    return;

                case "fire":
                    obj.isFire = !obj.isFire;
                    o.OnFire = obj.isFire;
                    return;

                case "light":
                    obj.isLight = !obj.isLight;
                    if (obj.isLight)
                    {
                        o.LightColor = obj.color;
                    }
                    return;

                case "lightcolor":
                    if (args[2] == null) { MainChat.SendErrorChat(p, "无效灯光颜色. 例如: 255,255,255 (3 位数字)"); return; }
                    string[] colors = args[2].Split(",");
                    if (colors.Length <= 2) { MainChat.SendErrorChat(p, "无效灯光颜色. 例如: 255,255,255 (3 位数字)"); return; }

                    int c1; int c2; int c3; bool c1ok = Int32.TryParse(colors[0], out c1); bool c2ok = Int32.TryParse(colors[1], out c2); bool c3ok = Int32.TryParse(colors[2], out c3);
                    if (!c1ok || !c2ok || !c3ok) { MainChat.SendErrorChat(p, "无效灯光颜色. 例如: 255,255,255 (3 位数字)"); return; }

                    obj.color = new Rgb(c1, c2, c3);
                    return;
            }


        }

        public static string[] bannedObjects = new string[]
        {
            "prop_s_pine_dead_01",
            "prop_tree_cedar_03",
            "prop_tree_cedar_04",
            "prop_s_pine_dead_01",
            "prop_gas_pump_1a",
            "prop_gas_pump_1b",
            "prop_gas_pump_1c",
            "prop_gas_pump_1d",
            "prop_gas_pump_old2",
            "prop_gas_pump_old3",
            "prop_vintage_pump",
            "hei_po1_01_shipmain",
            "xm_prop_x17_silo_rocket_01",
            "xs_combined2_dystplane_10",
            "ap1_02_planes00",
            "ap1_02_planes003",
            "ap1_02_planes005",
            "ap1_02_planes005bb",
            "ap1_02_planes009",
            "lf_house_10_",
            "lf_house_11_",
            "lf_house_13_",
            "lf_house_14_",
            "lf_house_15_",
            "lf_house_16_",
            "lf_house_17_",
            "lf_house_18_",
            "lf_house_19_",
            "lf_house_20_",
            "bkr_prop_biker_tube_l",
            "bkr_prop_biker_tube_m",
            "bkr_prop_biker_tube_s",
            "sr_prop_spec_tube_l_01a",
            "sr_prop_spec_tube_m_01a",
            "sr_prop_spec_tube_l_03a",
            "sr_prop_spec_tube_m_03a",
            "sr_prop_spec_tube_s_03a",
            "stt_prop_ramp_adj_loop",
            "stt_prop_ramp_adj_hloop",
            "stt_prop_ramp_multi_loop_rb",
            "stt_prop_ramp_jump_xxl",
            "stt_prop_stunt_bblock_xl3",
            "stt_prop_stunt_bblock_xl2",
            "stt_prop_stunt_bblock_xl1",
            "stt_prop_race_start_line_01",
            "stt_prop_race_start_line_01b",
            "stt_prop_race_start_line_02",
            "stt_prop_race_start_line_02b",
            "stt_prop_race_start_line_03",
            "stt_prop_race_start_line_03b",
            "v_ind_cfcarcass1",
            "v_ind_cfcarcass2",
            "v_ind_cfcarcass3",
            "prop_atm_01",
            "prop_atm_02",
            "prop_atm_03",
            "prop_fleeca_atm",
            "cs1_07_sea_plane_08",
            "cs1_07_sea_plane_13",
            "cs1_07_sea_plane_06",
            "prop_cs_plane_int_01 ",
            "apa_mp_apa_crashed_usaf_01a",
            "apa_mp_apa_yacht",
            "vw_prop_miniature_yacht_01a",
            "apa_ch2_superyacht_refproxy001",
            "apa_ch2_superyacht_refproxy002",
            "apa_ch2_superyacht_refproxy003",
            "apa_ch2_superyacht_refproxy004",
            "apa_ch2_superyacht_refproxy005",
            "apa_ch2_superyacht_refproxy006",
            "apa_ch2_superyacht_refproxy007 ",
            "v_ind_chickensx3",
            "v_ind_coo_heed",
            "v_ind_coo_quarter",
            "v_ind_meat_comm",
            "v_ind_ss_thread1",
            "v_ind_ss_thread2",
            "v_ind_ss_thread3",
            "v_ind_ss_thread10",
            "v_ind_ss_thread4",
            "v_ind_ss_thread5",
            "v_ind_ss_thread6",
            "v_ind_ss_thread7",
            "v_ind_ss_thread8",
            "v_ind_ss_thread9",
            "v_ind_ss_threadsa",
            "v_ind_ss_threadsb",
            "v_ind_ss_threadsc",
            "v_ind_ss_threadsd",
            "v_ilev_fbisecgate",
            "v_ilev_mm_faucet",
            "v_res_tt_cancrsh01",
            "v_res_tt_cancrsh02",
            "prop_ld_fan_01",
            "prop_new_drug_pack_01",
            "p_ferris_wheel_amo_l",
            "p_ferris_wheel_amo_p",
            "p_ferris_wheel_amo_l2",
            "p_parachute_fallen_s",
            "p_para_broken1_s",
            "p_shoalfish_s",
            "p_tram_crash_s",
            "prop_cs_beer_bot_test",
            "xs_prop_trinket_republican_01a",
            "ba_prop_club_dressing_poster_03",
            "ba_prop_club_dressing_posters_03",
            "ba_prop_club_dressing_posters_02",
            "v_ret_ps_ties_01",
            "v_ret_ps_ties_02",
            "v_ret_ps_ties_03",
            "v_ret_ps_ties_04",
            "prop_ind_deiseltank",
            "prop_pylon_01",
            "prop_pylon_02",
            "prop_pylon_03",
            "prop_pylon_04",
            "prop_sub_gantry",
            "prop_windmill_01",
            "prop_windmill_01_l1",
            "prop_windmill_01_slod",
            "prop_windmill_01_slod2",
            "p_stinger_02",
            "p_stinger_03",
            "p_stinger_04",
            "p_stinger_piece_01",
            "prop_asteroid_01",
            "prop_alien_egg_01",
            "prop_new_drug_pack_01",
            "p_ferris_wheel_amo_l",
            "p_ferris_wheel_amo_l2",
            "p_ferris_wheel_amo_p",
            "p_rcss_folded",
            "p_rcss_s",
            "p_tram_crash_s",
            "ex_prop_crate_minig",
            "prop_ld_bomb",
            "prop_ld_bomb_01",
            "prop_ld_bomb_01_open",
            "prop_ld_bomb_anim",
            "prop_military_pickup_01",
            "prop_mb_ordnance_02",
            "prop_mb_ordnance_03",
            "stt_prop_c4_stack",
            "hei_prop_carrier_defense_01",
            "hei_prop_carrier_defense_02",
            "hei_prop_carrier_gasbogey_01",
            "hei_prop_carrier_ord_01",
            "kt1_02_skyscraper",
            "des_stilthouse_root",
            "lf_house01",
            "lf_house04",
            "lf_house05",
            "lf_house07",
            "lf_house08",
            "lf_house09",
            "lf_house10",
            "lf_house11",
            "lf_house13",
            "lf_house14",
            "lf_house16",
            "lf_house17",
            "lf_house18",
            "lf_house19",
            "lf_house20",
            "po1_08_whouse_02",
            "po1_03_erhouse003",
            "vb_05_house555",
            "bh1_47_unburnt_house",
            "vb_05_house559",
            "ch1_12_house03ih",
            "ch1_12_ihwhouse_07",
            "ch1_12_sc03_housemain",
            "ch1_02_house_001",
            "ch1_02_house03ih",
            "ch1_02_housewoodfnt",
            "ch1_02_ihwhouse_07",
            "ch1_10_house101",
            "ch1_10_house102",
            "ch1_10_house13",
            "ch1_10_midhouse",
            "apa_ch2_04_house01",
            "apa_ch2_12b_house03mc",
            "ch2_09_house21",
            "ch2_12b_house_l1a",
            "ch2_12b_house_l1b",
            "ch2_12b_house_l1c",
            "stt_prop_ramp_adj_flip_s",
            "stt_prop_ramp_adj_flip_sb",
            "stt_prop_ramp_adj_hloop",
            "stt_prop_ramp_adj_loop",
            "stt_prop_ramp_jump_l",
            "stt_prop_ramp_jump_m",
            "stt_prop_ramp_jump_s",
            "stt_prop_ramp_jump_xl",
            "stt_prop_ramp_jump_xs",
            "stt_prop_ramp_jump_xxl",
            "stt_prop_ramp_multi_loop_rb",
            "stt_prop_ramp_spiral_l",
            "stt_prop_ramp_spiral_l_l",
            "stt_prop_ramp_spiral_l_m",
            "prop_side_lights",
            "des_traincrash_root1",
            "des_traincrash_root2",
            "des_traincrash_root3",
            "prop_watertower01",
            "prop_watertower02",
            "prop_watertower03",
            "prop_watertower04",
            "prop_snow_watertower01",
            "prop_snow_watertower01_l2",
            "prop_snow_watertower03",
            "prop_towercrane_01a",
            "prop_towercrane_02a",
            "prop_towercrane_02b",
            "prop_towercrane_02c",
            "prop_towercrane_02d",
            "prop_towercrane_02e",
            "prop_towercrane_02el",
            "prop_towercrane_02el2",
            "plg_05_tower",
            "dt1_11_dt1_tower",
            "prop_ind_oldcrane",
            "prop_tree_cedar_04",
            "prop_tree_cedar_03",
            "prop_s_pine_dead_01",
            "test_tree_cedar_trunk_001",
            "prop_tree_jacada_01",
            "prop_chall_lamp_02",
            "prop_wall_light_15a",
            "prop_wall_light_12a",
            "prop_wall_light_02a",
            "prop_wall_light_10a",
            "prop_wall_light_10b",
            "prop_wall_light_10c",
            "prop_walllight_ld_01",
            "prop_walllight_ld_01b",
            "prop_warninglight_01",
            "prop_air_conelight",
            "prop_air_lights_01a",
            "prop_air_lights_02a",
            "prop_air_lights_01b",
            "prop_air_lights_02b",
            "prop_air_lights_03a",
            "prop_air_lights_04a",
            "prop_air_lights_05a",
            "prop_air_terlight_01a",
            "prop_air_terlight_01b",
            "prop_air_terlight_01c",
            "prop_runlight_b",
            "prop_runlight_g",
            "prop_runlight_r",
            "prop_runlight_y",
            "apa_mp_h_lit_floorlamp_10",
            "xs_propintarena_lamps_01a",
            "xs_propintarena_lamps_01b",
            "xs_propintarena_lamps_01c",
            "v_ret_neon_baracho",
            "v_ret_neon_blarneys",
            "v_ret_neon_logger",
            "prop_beer_neon_01",
            "prop_beer_neon_02",
            "prop_beer_neon_03",
            "prop_beer_neon_04",
            "v_19_vanilla_sign_neon",
            "v_19_vanillasigneon",
            "v_19_vanillasigneon2",
            "prop_barrachneon",
            "prop_beerneon",
            "prop_cherenneon",
            "prop_cockneon",
            "prop_loggneon",
            "prop_patriotneon",
            "prop_ragganeon",
            "h4_prop_battle_lights_floorblue",
            "apa_mp_h_lit_floorlamp_10",
            "apa_mp_h_lit_floorlampnight_14",
            "bkr_prop_clubhouse_sofa_01a",
        };
        // Player ObjectSystem
        [Command("addobj")]
        public static void COM_CreateObject(PlayerModel p, params string[] args)
        {
            if (p.adminJail > 0 || p.jailTime > 0) { MainChat.SendErrorChat(p, "[错误] 您在监狱中."); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /addobj [模型]"); return; }
            if (getSettings(p).ObjectLimit <= serverObjects.Where(x => x.Owner == p.sqlID).Count()) { MainChat.SendErrorChat(p, "[错误] 您已达到物体摆放限制!"); return; }
            if (bannedObjects.Contains(args[0].ToLower()) || args[0].Contains('"'))
            {
                MainChat.SendErrorChat(p, "[错误] 此物体已被服务器禁止.");
                return;
            }

            GlobalEvents.ShowObjectPlacement(p, args[0], "Player:PlaceObject");
            MainChat.SendInfoChat(p, "[?] 操作物体:<br>左右键: 调整高度.<br>上下键: 调整举例<br>小键盘4-8: 调整左右旋转.");
            return;
        }

        [AsyncClientEvent("Player:PlaceObject")]
        public void EVENT_PlaceObject(PlayerModel p, string rot, string pos, string model)
        {
            if (getSettings(p).ObjectLimit <= serverObjects.Where(x => x.Owner == p.sqlID).Count()) { MainChat.SendErrorChat(p, "[错误] 您已达到物体摆放限制!"); return; }
            Vector3 position = JsonConvert.DeserializeObject<Vector3>(pos);
            Vector3 rotation = JsonConvert.DeserializeObject<Vector3>(rot);

            ServerObjects nObj = new ServerObjects()
            {
                ID = (int)PropStreamer.Create(model, position, rotation, dimension: p.Dimension, frozen: true).Id,
                Dimension = p.Dimension,
                model = model,
                Position = position,
                rotation = rotation,
                Owner = p.sqlID

            };

            serverObjects.Add(nObj);
            MainChat.SendInfoChat(p, "[!] 已添加物体, 编号: " + nObj.ID);
            return;
        }

        [Command("deobj")]
        public static void COM_RemoveObject(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /deobj [物体编号]<br>如果不知道编号, 可以输入 /objlist 查看物体列表"); return; }
            if (!Int32.TryParse(args[0], out int objectID)) { MainChat.SendErrorChat(p, "[用法] /deobj [物体编号]<br>如果不知道编号, 可以输入 /objlist 查看物体列表"); return; }

            var obj = serverObjects.Find(x => x.ID == objectID);
            if (obj == null) { MainChat.SendErrorChat(p, "[错误] 无效物体!"); return; }
            if (obj.Owner != p.sqlID) { MainChat.SendErrorChat(p, "[错误] 指定物体不属于您!"); return; }

            var prop = PropStreamer.GetProp((ulong)obj.ID);
            if (prop != null)
                prop.Delete();

            serverObjects.Remove(obj);
            MainChat.SendInfoChat(p, "[?] 已删除指定物体.");
            return;
        }

        [Command("objlist")]
        public static void COM_ShowPlayerObjects(PlayerModel p)
        {
            string obj = "<center>您的物体列表</center><br>";
            int total = 0;
            foreach (var o in serverObjects)
            {
                if (o.Owner == p.sqlID)
                {
                    obj += "编号: " + o.ID + " - 模型:" + o.model + " - 坐标: " + JsonConvert.SerializeObject(o.Position) + "<br>";
                    total++;
                }
            }
            obj += "<br>使用数量: " + total + "/" + getSettings(p).ObjectLimit;
            MainChat.SendInfoChat(p, obj);
            return;
        }

        public static CharacterSettings getSettings(PlayerModel p)
        {
            return JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
        }
        public static void UpdateSettings(PlayerModel p, CharacterSettings settings)
        {
            p.settings = JsonConvert.SerializeObject(settings);
            p.updateSql();
            return;
        }

/*        [Command("objepaket10")]
        public async Task COM_BuyObjectPaketTen(PlayerModel p)
        {
            AccountModel acc = await Database.DatabaseMain.getAccInfo(p.accountId);
            if (acc.lscPoint < 30) { MainChat.SendErrorChat(p, "[错误] Yeterli LSC Puanınız bulunmuyor. Gereken 30LSC-P"); return; }

            acc.lscPoint -= 30;
            await acc.Update();
            var setting = getSettings(p);
            setting.ObjectLimit += 10;
            UpdateSettings(p, setting);
            MainChat.SendInfoChat(p, "[!] Karakterinize 15 obje limiti eklendi.");
            return;
        }

        [Command("objepaket20")]
        public async Task COM_BuyObjectTwenty(PlayerModel p)
        {
            AccountModel acc = await Database.DatabaseMain.getAccInfo(p.accountId);
            if (acc.lscPoint < 50) { MainChat.SendErrorChat(p, "[错误] Yeterli LSC Puanınız bulunmuyor. Gereken 50LSC-P"); return; }

            acc.lscPoint -= 50;
            await acc.Update();
            var setting = getSettings(p);
            setting.ObjectLimit += 20;
            UpdateSettings(p, setting);
            MainChat.SendInfoChat(p, "[!] Karakterinize 25 obje limiti eklendi.");
            return;
        }*/

    }
}
