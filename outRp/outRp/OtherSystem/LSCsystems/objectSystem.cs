using System;
using System.Collections.Generic;
using AltV.Net; using AltV.Net.Async;
using AltV.Net.Data;
using outRp.OtherSystem.Textlabels;
using outRp.Models;
using outRp.Chat;
using System.Numerics;
using Newtonsoft.Json;
using outRp.OtherSystem.NativeUi;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;

namespace outRp.OtherSystem.LSCsystems
{
    public class objectSystem : IScript
    {
        public class playerObject
        {
            public int objectID { get; set; }
            //public int textLabelID { get; set; }
            public int ownerID { get; set; }
            public Position pos { get; set; }
            public Vector3 rot { get; set; }
            public int dimension { get; set; }
            public string oModel { get; set; }

        }

        public class sellingObject
        {
            public int ID { get; set; }
            public string model { get; set; }
            public int price { get; set; }
            public string img { get; set; }
        }

        public static List<playerObject> playerObjects = new List<playerObject>();

        public static void loadObjects(string data)
        {
            playerObjects = JsonConvert.DeserializeObject<List<playerObject>>(data);

            foreach (var o in playerObjects)
            {
                Position oPos = o.pos;
                oPos.Z -= 1;
                o.objectID = (int)PropStreamer.Create(o.oModel, oPos, o.rot, dimension: o.dimension, frozen: true).Id;
                //o.textLabelID = (int)TextLabelStreamer.Create("~b~[~w~Obje " + o.objectID + "~b~]~n~Etkileşim için ~g~/objekaldir",o.pos, dimension: o.dimension, font: 0, streamRange: 1).Id;
            }

            TextLabelStreamer.Create("~b~[~w~家具店~b~]~n~指令: ~g~/furniture", new Position(46.496704f, -1749.8374f, 29.616821f), streamRange: 10, font: 0);
            Alt.Log("加载 玩家家具.");
        }

        
        public static bool placePlayerObject(PlayerModel p, string model)
        {
            if(p.Dimension <= 0) { MainChat.SendErrorChat(p, "[错误] 您只能在内饰放置家具."); return false; }
            Globals.GlobalEvents.ShowObjectPlacement(p, model, "PlayerObject:Place");
            return true;
        }
        
        [AsyncClientEvent("PlayerObject:Place")]
        public void createObject(PlayerModel p, string rot, string pos, string model)
        {
            if(p.Dimension <= 0) { MainChat.SendErrorChat(p, "[错误] 您只能在内饰放置家具."); return; }
            playerObject n = new playerObject();
            Vector3 position = JsonConvert.DeserializeObject<Vector3>(pos);
            Vector3 rotation = JsonConvert.DeserializeObject<Vector3>(rot);

            n.objectID = (int)PropStreamer.Create(model, position, rotation, dimension: p.Dimension, frozen: true).Id;
            position.Z += 1;
            //n.textLabelID = (int)TextLabelStreamer.Create("~b~[~w~Obje " + n.objectID + "~b~]~n~Etkileşim için ~g~/objekaldir", position, dimension: p.Dimension, font: 0, streamRange: 1).Id;

            n.ownerID = p.sqlID;
            n.oModel = model;
            n.pos = position;
            n.rot = rotation;
            n.dimension = p.Dimension;

            playerObjects.Add(n);

            MainChat.SendInfoChat(p, "> 已放置家具.");
        }

        [Command("nearfurn")]
        public static void COM_ShowNearObject(PlayerModel p)
        {
            MainChat.SendInfoChat(p, "<center>附近的家具</center>");
            foreach(playerObject o in playerObjects)
            {
                if(p.Position.Distance(o.pos) < 5 && o.dimension == p.Dimension)
                {
                    p.SendChatMessage(o.objectID + " | 模型:" + o.oModel + " | 所有者:" + o.ownerID);
                }
            }
            return;
        }

        [Command("pickfurn")]
        public async Task COM_RemoveObject(PlayerModel p, params string[] args)
        {
            if(args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /pickfurn [id]"); return; }
            int id; bool isOk = Int32.TryParse(args[0], out id);
            if (!isOk) { MainChat.SendInfoChat(p, "[用法] /pickfurn [id]"); return; }

            var obj = playerObjects.Find(x => x.objectID == id && x.dimension == p.Dimension);
            if(obj == null) { MainChat.SendErrorChat(p, "[错误] 无效家具."); return; }

            bool canEdit = false;
            if (p.adminWork) { canEdit = true; }
            else
            {
                if(obj.ownerID == p.sqlID) { canEdit = true; }
            }

            if (!canEdit) { MainChat.SendErrorChat(p, "[错误] 此家具不属于您."); return; }

            ServerItems item = new ServerItems()
            {
                ID = 31,
                type = 22,
                name = "家具物品",
                picture = "31",
                weight = 0,
                equipSlot = 13,
                equipable = true,
                data = PropStreamer.GetProp((ulong)obj.objectID).Model,
                data2 = ""
            };

            if(!await Inventory.AddInventoryItem(p, item, 1))
            {
                MainChat.SendErrorChat(p, "[错误] 您的库存满了.");
                return;
            }

            //TextLabelStreamer.GetDynamicTextLabel((ulong)obj.textLabelID).Delete();
            PropStreamer.GetProp((ulong)obj.objectID).Delete();

            playerObjects.Remove(obj);

            MainChat.SendInfoChat(p, "> 已回收家具.");
        }


        // ! OBJECT MARKET 
        // 46,496704f, -1749,8374f, 29,616821f
        [AsyncClientEvent("objectMarket:Show")][Command("furniture")]
        public void showObjectMarket(PlayerModel p)
        {
            if(p.Position.Distance(new Position(46.496704f, -1749.8374f, 29.616821f)) > 5) { MainChat.SendErrorChat(p, "[错误] 您不在家具商店."); return; }
            GuiMenu chairs = new GuiMenu { name = "座椅 / 椅子", triger = "objectMarket:Category", value = "1" };
            GuiMenu bedroom = new GuiMenu { name = "卧室家具", triger = "objectMarket:Category", value = "2" };
            GuiMenu locker = new GuiMenu { name = "桌子 / 橱柜", triger = "objectMarket:Category", value = "3" };
            GuiMenu decoration = new GuiMenu { name = "装饰品", triger = "objectMarket:Category", value = "4" };
            GuiMenu kitchen = new GuiMenu { name = "厨房用品", triger = "objectMarket:Category", value = "5" };
            GuiMenu technology = new GuiMenu { name = "电子产品", triger = "objectMarket:Category", value = "6" };

            GuiMenu close = GuiEvents.closeItem;

            List<GuiMenu> gMenu = new List<GuiMenu>();

            gMenu.Add(chairs);
            gMenu.Add(bedroom);
            gMenu.Add(locker);
            gMenu.Add(decoration);
            gMenu.Add(kitchen);
            gMenu.Add(technology);

            gMenu.Add(close);
            Gui y = new Gui()
            {
                image = "https://ih0.redbubble.net/image.941083207.9340/flat,550x550,075,f.u3.jpg",
                guiMenu = gMenu,
                color = "#4AC27D"
            };
            y.Send(p);
        }

        [AsyncClientEvent("objectMarket:Category")]
        public void objectMarketSelectCategory(PlayerModel p, int cat)
        {
            List<GuiMenu> gMenu = new List<GuiMenu>();
            switch (cat)
            {
                case 1:
                    for(int a = 1; a <= 44; a++)
                    {
                        var c1o = sellingObjects.Find(x => x.ID == a);
                        GuiMenu case1i = new GuiMenu { name = c1o.model, triger = "objectMarket:buyitem", value = a.ToString(), popup = "价格: " + c1o.price + " |" + c1o.img };
                        gMenu.Add(case1i);
                    }
                    break;

                case 2:
                    for (int a = 45; a <= 57; a++)
                    {
                        var c2o = sellingObjects.Find(x => x.ID == a);
                        GuiMenu case2i = new GuiMenu { name = c2o.model, triger = "objectMarket:buyitem", value = a.ToString(), popup = "价格: " + c2o.price + " |" + c2o.img };
                        gMenu.Add(case2i);
                    }
                    break;

                case 3:
                    for (int a = 58; a <= 89; a++)
                    {
                        var c2o = sellingObjects.Find(x => x.ID == a);
                        GuiMenu case2i = new GuiMenu { name = c2o.model, triger = "objectMarket:buyitem", value = a.ToString(), popup = "价格: " + c2o.price + " |" + c2o.img };
                        gMenu.Add(case2i);
                    }
                break;

                case 4:
                    for (int a = 90; a <= 131; a++)
                    {
                        var c2o = sellingObjects.Find(x => x.ID == a);
                        GuiMenu case2i = new GuiMenu { name = c2o.model, triger = "objectMarket:buyitem", value = a.ToString(), popup = "价格: " + c2o.price + " |" + c2o.img };
                        gMenu.Add(case2i);
                    }
                break;

                case 5:
                    for (int a = 132; a <= 140; a++) // <img class="ui small image" src="/images/wireframe/image.png">
                    {
                        var c2o = sellingObjects.Find(x => x.ID == a);
                        GuiMenu case2i = new GuiMenu { name = c2o.model, triger = "objectMarket:buyitem", value = a.ToString(), popup = "价格: " + c2o.price + " |" + c2o.img };
                        gMenu.Add(case2i);
                    }
                break;

                case 6:
                    for (int a = 141; a <= 150; a++)
                    {
                        var c2o = sellingObjects.Find(x => x.ID == a);
                        GuiMenu case2i = new GuiMenu { name = c2o.model, triger = "objectMarket:buyitem", value = a.ToString(), popup = "价格: " + c2o.price + " |" + c2o.img };
                        gMenu.Add(case2i);
                    }
                    break;
            }

            GuiMenu back = new GuiMenu { name = "返回", triger = "objectMarket:Show", value = "a" };

            GuiMenu close = GuiEvents.closeItem;

            gMenu.Add(back);
            gMenu.Add(close);

            Gui y = new Gui()
            {
                image = "https://ih0.redbubble.net/image.941083207.9340/flat,550x550,075,f.u3.jpg",
                guiMenu = gMenu,
                color = "#4AC27D"
            };
            y.Send(p);

        }

        [AsyncClientEvent("objectMarket:buyitem")]
        public async Task objectMarketBuyItem(PlayerModel p, int ItemId)
        {
            var i = sellingObjects.Find(x => x.ID == ItemId);
            if(i == null) { return; }

            if(p.cash < i.price) { MainChat.SendErrorChat(p, "[错误] 您没有足够的钱!"); return; }

            ServerItems item = new ServerItems()
            {
                ID = 31,
                type = 22,
                name = "家具物品",
                picture = "31",
                weight = 0,
                equipSlot = 13,
                equipable = true,
                data = i.model,
                data2 = ""
            };

            bool isok = await Inventory.AddInventoryItem(p, item, 1);
            if (!isok) { MainChat.SendInfoChat(p, "> 您的库存满了."); return; }

            p.cash -= i.price;
            p.updateSql();
        }


        // TODO OBJECT LIST 

        public static List<sellingObject> sellingObjects = new List<sellingObject>()
        {
                // Koltuklar ve Sandalyeler
            new sellingObject(){ID = 1, model = "apa_mp_h_yacht_armchair_01", price = 80, img = "./img/1.png"},
            new sellingObject(){ID = 2, model = "apa_mp_h_yacht_armchair_03", price = 80, img = "./img/2.png"},
            new sellingObject(){ID = 3, model = "apa_mp_h_yacht_armchair_04", price = 80, img = "./img/3.png"},
            new sellingObject(){ID = 4, model = "bkr_prop_clubhouse_armchair_01a", price = 120, img = "./img/4.png"},
            new sellingObject(){ID = 5, model = "prop_armchair_01", price = 120, img = "./img/5.png"},
            new sellingObject(){ID = 6, model = "apa_mp_h_stn_chairarm_01", price = 100, img = "./img/6.png"},
            new sellingObject(){ID = 7, model = "apa_mp_h_stn_chairarm_03", price = 100, img = "./img/7.png"},
            new sellingObject(){ID = 8, model = "apa_mp_h_stn_chairarm_12", price = 120, img = "./img/8.png"},
            new sellingObject(){ID = 9, model = "apa_mp_h_stn_chairarm_23", price = 80, img = "./img/9.png"},
            new sellingObject(){ID = 10, model = "apa_mp_h_stn_chairarm_24", price = 100, img = "./img/10.png"},
            new sellingObject(){ID = 11, model = "apa_mp_h_stn_chairarm_26", price = 70, img = "./img/11.png"},
            new sellingObject(){ID = 12, model = "apa_mp_h_stn_chairstrip_01", price = 80, img = "./img/12.png"},
            new sellingObject(){ID = 13, model = "apa_mp_h_stn_chairstrip_07", price = 100, img = "./img/13.png"},
            new sellingObject(){ID = 14, model = "bkr_prop_biker_chairstrip_01", price = 150, img = "./img/14.png"},
            new sellingObject(){ID = 15, model = "ba_prop_battle_club_chair_02", price = 90, img = "./img/15.png"},
            new sellingObject(){ID = 16, model = "ba_prop_battle_club_chair_03", price = 130, img = "./img/16.png"},
            new sellingObject(){ID = 17, model = "ex_mp_h_off_chairstrip_01", price = 120, img = "./img/17.png"},
            new sellingObject(){ID = 18, model = "xm_lab_chairarm_26", price = 100, img = "./img/18.png"},
            new sellingObject(){ID = 19, model = "apa_mp_h_stn_sofa2seat_02", price = 180, img = "./img/19.png"},
            new sellingObject(){ID = 20, model = "hei_heist_stn_sofa2seat_06", price = 200, img = "./img/20.png"},
            new sellingObject(){ID = 21, model = "hei_heist_stn_sofa3seat_01", price = 210, img = "./img/21.png"},
            new sellingObject(){ID = 22, model = "hei_heist_stn_sofa3seat_06", price = 220, img = "./img/22.png"},
            new sellingObject(){ID = 23, model = "apa_mp_h_stn_sofacorn_07", price = 250, img = "./img/23.png"},
            new sellingObject(){ID = 24, model = "apa_mp_h_stn_sofacorn_09", price = 250, img = "./img/24.png"},
            new sellingObject(){ID = 25, model = "apa_mp_h_stn_sofacorn_10", price = 250, img = "./img/25.png"},
            new sellingObject(){ID = 26, model = "apa_mp_h_yacht_sofa_02", price = 160, img = "./img/26.png"},
            new sellingObject(){ID = 27, model = "bkr_prop_clubhouse_sofa_01a", price = 160, img = "./img/27.png"},
            new sellingObject(){ID = 28, model = "ex_mp_h_off_sofa_003", price = 200, img = "./img/28.png"},
            new sellingObject(){ID = 29, model = "ex_mp_h_off_sofa_02", price = 200, img = "./img/29.png"},
            new sellingObject(){ID = 30, model = "p_lev_sofa_s", price = 180, img = "./img/30.png"},
            new sellingObject(){ID = 31, model = "p_res_sofa_l_s", price = 300, img = "./img/31.png"},
            new sellingObject(){ID = 32, model = "p_v_med_p_sofa_s", price = 210, img = "./img/32.png"},
            new sellingObject(){ID = 33, model = "p_sofa_s", price = 150, img = "./img/33.png"},
            new sellingObject(){ID = 34, model = "apa_mp_h_din_chair_04", price = 40, img = "./img/34.png"},
            new sellingObject(){ID = 35, model = "apa_mp_h_din_chair_08", price = 40, img = "./img/35.png"},
            new sellingObject(){ID = 36, model = "apa_mp_h_din_chair_12", price = 45, img = "./img/36.png"},
            new sellingObject(){ID = 37, model = "bkr_prop_weed_chair_01a", price = 50, img = "./img/37.png"},
            new sellingObject(){ID = 38, model = "bkr_prop_clubhouse_chair_03", price = 20, img = "./img/38.png"},
            new sellingObject(){ID = 39, model = "ex_mp_h_din_chair_09", price = 40, img = "./img/39.png"},
            new sellingObject(){ID = 40, model = "ex_prop_offchair_exec_03", price = 70, img = "./img/40.png"},
            new sellingObject(){ID = 41, model = "hei_heist_din_chair_02", price = 60, img = "./img/41.png"},
            new sellingObject(){ID = 42, model = "hei_heist_din_chair_05", price = 50, img = "./img/42.png"},
            new sellingObject(){ID = 43, model = "prop_chair_07", price = 40, img = "./img/43.png"},
            new sellingObject(){ID = 44, model = "prop_cs_office_chair", price = 50, img = "./img/44.png"},

            // Yatak Odası
            new sellingObject(){ID = 45, model = "apa_mp_h_bed_double_08", price = 320, img = "./img/45.png"},
            new sellingObject(){ID = 46, model = "apa_mp_h_bed_double_09", price = 375, img = "./img/46.png"},
            new sellingObject(){ID = 47, model = "apa_mp_h_bed_wide_05", price = 250, img = "./img/47.png"},
            new sellingObject(){ID = 48, model = "apa_mp_h_bed_with_table_02", price = 390, img = "./img/48.png"},
            new sellingObject(){ID = 49, model = "apa_mp_h_yacht_bed_01", price = 350, img = "./img/49.png"},
            new sellingObject(){ID = 50, model = "apa_mp_h_yacht_bed_02", price = 350, img = "./img/50.png"},
            new sellingObject(){ID = 51, model = "ex_prop_exec_bed_01", price = 195, img = "./img/51.png"},
            new sellingObject(){ID = 52, model = "gr_prop_bunker_bed_01", price = 150, img = "./img/52.png"},
            new sellingObject(){ID = 53, model = "hei_heist_bed_double_08", price = 280, img = "./img/53.png"},
            new sellingObject(){ID = 54, model = "imp_prop_impexp_sofabed_01a", price = 140, img = "./img/54.png"},
            new sellingObject(){ID = 55, model = "p_lestersbed_s", price = 120, img = "./img/55.png"},
            new sellingObject(){ID = 56, model = "p_mbbed_s", price = 470, img = "./img/56.png"},
            new sellingObject(){ID = 57, model = "v_res_msonbed_s", price = 120, img = "./img/57.png"},


            // Masalar & Dolaplar
            new sellingObject(){ID = 58, model = "apa_mp_h_bed_chestdrawer_02", price = 95, img = "./img/58.png"},
            new sellingObject(){ID = 59, model = "hei_heist_bed_chestdrawer_04", price = 80, img = "./img/59.png"},
            new sellingObject(){ID = 60, model = "hei_heist_bed_table_dble_04", price = 140, img = "./img/60.png"},
            new sellingObject(){ID = 61, model = "apa_mp_h_bed_table_wide_12", price = 120, img = "./img/61.png"},
            new sellingObject(){ID = 62, model = "prop_tv_cabinet_03", price = 65, img = "./img/62.png"},
            new sellingObject(){ID = 63, model = "prop_tv_cabinet_04", price = 50, img = "./img/63.png"},
            new sellingObject(){ID = 64, model = "prop_tv_cabinet_05", price = 80, img = "./img/64.png"},
            new sellingObject(){ID = 65, model = "apa_mp_h_din_table_01", price = 200, img = "./img/65.png"},
            new sellingObject(){ID = 66, model = "apa_mp_h_din_table_04", price = 220, img = "./img/66.png"},
            new sellingObject(){ID = 67, model = "apa_mp_h_din_table_04", price = 180, img = "./img/67.png"},
            new sellingObject(){ID = 68, model = "apa_mp_h_din_table_06", price = 150, img = "./img/68.png"},
            new sellingObject(){ID = 69, model = "apa_mp_h_yacht_coffee_table_01", price = 320, img = "./img/69.png"},
            new sellingObject(){ID = 70, model = "apa_mp_h_yacht_coffee_table_02", price = 315, img = "./img/70.png"},
            new sellingObject(){ID = 71, model = "apa_mp_h_yacht_side_table_01", price = 250, img = "./img/71.png"},
            new sellingObject(){ID = 72, model = "apa_mp_h_yacht_side_table_02", price = 120, img = "./img/72.png"},
            new sellingObject(){ID = 73, model = "ba_prop_int_edgy_table_01", price = 100, img = "./img/73.png"},
            new sellingObject(){ID = 74, model = "ba_prop_int_edgy_table_02", price = 100, img = "./img/74.png"},
            new sellingObject(){ID = 75, model = "ba_prop_int_edgy_table_02", price = 50, img = "./img/75.png"},
            new sellingObject(){ID = 76, model = "ch_prop_ch_coffe_table_02", price = 210, img = "./img/76.png"},
            new sellingObject(){ID = 77, model = "ex_prop_ex_console_table_01", price = 115, img = "./img/77.png"},
            new sellingObject(){ID = 78, model = "gr_dlc_gr_yacht_props_table_01", price = 75, img = "./img/78.png"},
            new sellingObject(){ID = 79, model = "gr_dlc_gr_yacht_props_table_02", price = 95, img = "./img/79.png"},
            new sellingObject(){ID = 80, model = "gr_dlc_gr_yacht_props_table_03", price = 100, img = "./img/80.png"},
            new sellingObject(){ID = 81, model = "hei_heist_din_table_07", price = 125, img = "./img/81.png"},
            new sellingObject(){ID = 82, model = "prop_fbi3_coffee_table", price = 65, img = "./img/82.png"},
            new sellingObject(){ID = 83, model = "prop_ld_farm_table01", price = 35, img = "./img/83.png"},
            new sellingObject(){ID = 84, model = "prop_ld_farm_table02", price = 35, img = "./img/84.png"},
            new sellingObject(){ID = 85, model = "prop_patio_lounger1_table", price = 50, img = "./img/85.png"},
            new sellingObject(){ID = 86, model = "prop_tablesmall_01", price = 75, img = "./img/86.png"},
            new sellingObject(){ID = 87, model = "prop_table_01", price = 60, img = "./img/87.png"},
            new sellingObject(){ID = 88, model = "prop_table_06", price = 80, img = "./img/88.png"},
            new sellingObject(){ID = 89, model = "prop_t_coffe_table", price = 115, img = "./img/89.png"},

            // Dekorasyon
            new sellingObject(){ID = 90, model = "apa_mp_h_acc_artwalll_01", price = 20, img = "./img/90.png"},
            new sellingObject(){ID = 91, model = "apa_mp_h_acc_artwallm_03", price = 20, img = "./img/91.png"},
            new sellingObject(){ID = 92, model = "apa_mp_h_acc_bowl_ceramic_01", price = 10, img = "./img/92.png"},
            new sellingObject(){ID = 93, model = "apa_mp_h_acc_bottle_02", price = 5, img = "./img/93.png"},
            new sellingObject(){ID = 94, model = "apa_mp_h_acc_candles_01", price = 5, img = "./img/94.png"},
            new sellingObject(){ID = 95, model = "apa_mp_h_acc_candles_02", price = 5, img = "./img/95.png"},
            new sellingObject(){ID = 96, model = "apa_mp_h_acc_dec_head_01", price = 15, img = "./img/96.png"},
            new sellingObject(){ID = 97, model = "apa_mp_h_acc_dec_plate_01", price = 15, img = "./img/97.png"},
            new sellingObject(){ID = 98, model = "apa_mp_h_acc_dec_sculpt_03", price = 10, img = "./img/98.png"},
            new sellingObject(){ID = 99, model = "apa_mp_h_acc_drink_tray_02", price = 15, img = "./img/99.png"},
            new sellingObject(){ID = 100, model = "apa_mp_h_acc_phone_01", price = 15, img = "./img/100.png"},
            new sellingObject(){ID = 101, model = "apa_mp_h_acc_plant_palm_01", price = 10, img = "./img/101.png"},
            new sellingObject(){ID = 102, model = "apa_mp_h_acc_plant_tall_01", price = 5, img = "./img/102.png"},
            new sellingObject(){ID = 103, model = "apa_mp_h_acc_rugwoolm_01", price = 15, img = "./img/103.png"},
            new sellingObject(){ID = 104, model = "apa_mp_h_acc_rugwoolm_03", price = 15, img = "./img/104.png"},
            new sellingObject(){ID = 105, model = "apa_mp_h_acc_rugwools_01", price = 15, img = "./img/105.png"},
            new sellingObject(){ID = 106, model = "apa_mp_h_acc_vase_01", price = 10, img = "./img/106.png"},
            new sellingObject(){ID = 107, model = "prop_pooltable_02", price = 50, img = "./img/107.png"},
            new sellingObject(){ID = 108, model = "apa_mp_h_acc_vase_flowers_04", price = 15, img = "./img/108.png"},
            new sellingObject(){ID = 109, model = "apa_mp_h_lit_floorlampnight_07", price = 15, img = "./img/109.png"},
            new sellingObject(){ID = 110, model = "apa_mp_h_lit_floorlampnight_14", price = 10, img = "./img/110.png"},
            new sellingObject(){ID = 111, model = "apa_mp_h_lit_floorlamp_02", price = 15, img = "./img/111.png"},
            new sellingObject(){ID = 112, model = "apa_mp_h_lit_lamptable_09", price = 15, img = "./img/112.png"},
            new sellingObject(){ID = 113, model = "apa_p_apa_champ_flute_s", price = 5, img = "./img/113.png"},
            new sellingObject(){ID = 114, model = "ba_prop_battle_bag_01a", price = 5, img = "./img/114.png"},
            new sellingObject(){ID = 115, model = "ba_prop_battle_bar_beerfridge_01", price = 50, img = "./img/115.png"},
            new sellingObject(){ID = 116, model = "ba_prop_battle_bar_fridge_02", price = 50, img = "./img/116.png"},
            new sellingObject(){ID = 117, model = "ba_prop_battle_champ_01", price = 5, img = "./img/117.png"},
            new sellingObject(){ID = 118, model = "ba_prop_battle_champ_closed", price = 5, img = "./img/118.png"},
            new sellingObject(){ID = 119, model = "ba_prop_battle_champ_closed_02", price = 5, img = "./img/119.png"},
            new sellingObject(){ID = 120, model = "ba_prop_battle_dj_stand", price = 55, img = "./img/120.png"},
            new sellingObject(){ID = 121, model = "ba_prop_battle_trophy_battler", price = 5, img = "./img/121.png"},
            new sellingObject(){ID = 122, model = "bkr_prop_clubhouse_jukebox_02a", price = 55, img = "./img/122.png"},
            new sellingObject(){ID = 123, model = "bkr_prop_money_counter", price = 50, img = "./img/123.png"},
            new sellingObject(){ID = 124, model = "ch_prop_arcade_claw_plush_06a", price = 5, img = "./img/124.png"},
            new sellingObject(){ID = 125, model = "ch_prop_arcade_claw_plush_05a", price = 5, img = "./img/125.png"},
            new sellingObject(){ID = 126, model = "ch_prop_arcade_invade_01a", price = 50, img = "./img/126.png"},
            new sellingObject(){ID = 127, model = "ch_prop_calculator_01a", price = 5, img = "./img/127.png"},
            new sellingObject(){ID = 128, model = "apa_mp_h_acc_rugwools_03", price = 15, img = "./img/128.png"},
            new sellingObject(){ID = 129, model = "apa_mp_h_acc_rugwoolm_04", price = 15, img = "./img/129.png"},
            new sellingObject(){ID = 130, model = "hei_heist_acc_rughidel_01", price = 15, img = "./img/130.png"},
            new sellingObject(){ID = 131, model = "hei_heist_acc_rugwooll_02", price = 15, img = "./img/131.png"},

            // Mutfak Ürünleri
            new sellingObject(){ID = 132, model = "prop_trailr_fridge", price = 100, img = "./img/133.png"},
            new sellingObject(){ID = 133, model = "v_res_fridgemoda", price = 200, img = "./img/134.png"},
            new sellingObject(){ID = 134, model = "v_res_fridgemodsml", price = 250, img = "./img/135.png"},
            new sellingObject(){ID = 135, model = "v_res_tre_fridge", price = 300, img = "./img/136.png"},
            new sellingObject(){ID = 136, model = "apa_mp_h_acc_coffeemachine_01", price = 175, img = "./img/137.png"},
            new sellingObject(){ID = 137, model = "prop_micro_01", price = 90, img = "./img/138.png"},
            new sellingObject(){ID = 138, model = "prop_micro_02", price = 140, img = "./img/139.png"},
            new sellingObject(){ID = 139, model = "prop_watercooler", price = 250, img = "./img/140.png"},
            new sellingObject(){ID = 140, model = "p_new_j_counter_02", price = 70, img = "./img/141.png"},

            // Teknoloji
            new sellingObject(){ID = 141, model = "des_tvsmash_root", price = 200, img = "./img/141.png"},
            new sellingObject(){ID = 142, model = "ex_prop_ex_tv_flat_01", price = 250, img = "./img/142.png"},
            new sellingObject(){ID = 143, model = "prop_cs_tv_stand", price = 200, img = "./img/143.png"},
            new sellingObject(){ID = 144, model = "as_prop_as_laptop_01a", price = 250, img = "./img/144.png"},
            new sellingObject(){ID = 145, model = "xm_prop_x17_res_pctower", price = 120, img = "./img/145.png"},
            new sellingObject(){ID = 146, model = "as_prop_as_speakerdock", price = 175, img = "./img/146.png"},
            new sellingObject(){ID = 147, model = "prop_speaker_02", price = 90, img = "./img/147.png"},
            new sellingObject(){ID = 148, model = "prop_speaker_03", price = 90, img = "./img/148.png"},
            new sellingObject(){ID = 149, model = "prop_cs_keyboard_01", price = 250, img = "./img/149.png"},
            new sellingObject(){ID = 150, model = "prop_monitor_01a", price = 40, img = "./img/150.png"},
        };

    }
}
