using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace outRp.Models
{
    public class ServerItems
    {
        public int ID { get; set; }
        public int selectID { get; set; } = 0;
        public int type { get; set; } = 0;
        public string name { get; set; } = "x";
        public string picture { get; set; } = "x";
        public int price { get; set; } = 0;
        public double weight { get; set; } = 0;
        public string data { get; set; } = "x";
        public string data2 { get; set; } = "x";
        public bool stackable { get; set; } = false;
        public bool equipable { get; set; } = false;
        public int equipSlot { get; set; } = 0;
        public int amount { get; set; } = 0;
        public string objectModel { get; set; } = "hei_prop_hei_paper_bag";
        public bool isNameChange { get; set; } = false;
    }
    public class InventoryModel
    {
        public int ID { get; set; }
        public int ownerId { get; set; } = 1;
        public string itemName { get; set; }
        public int itemId { get; set; }
        public int itemAmount { get; set; }
        public int itemSlot { get; set; }
        public string itemData { get; set; }
        public string itemData2 { get; set; }
        public double itemWeight { get; set; }
        public string itemPicture { get; set; } = "NaN";
        public bool Equipable { get; set; }        

        public async Task Update() => await Database.DatabaseMain.UpdatePlayerInventoryItem(this);
        public async Task Create() => await Database.DatabaseMain.CreatePlayerInventoryItem(this);
        public async Task Delete() => await Database.DatabaseMain.DeletePlayerInventoryItem(this);

    }

    public class Items
    {

        /*--------------------> ITEM TYPE LIST <------------------------/
         * 1: Telefon -> Data : Tel No
         * 2: Yemek -> Data : Gelen HP
         * 3: Araç tamir kiti -> Data:Tamir çeşidi. (1: Motor,2: gövde,3: benzin tankı);
         * 4: Elbise
         * 5: Sigara
         * 6: Çakmak
         * 7: Ehliyet
         * 8: Benzin Bidonu
         * 9: Odun
         * 10: Araç lastiği
         * 11: Banka Kartı
         * 12: Hurdalık eşyaları -> Satılabilir.
         * 13: Ciftlik Tohum
         * 14: Ciftlik SU
         * 15: Ciftlik alınan ürün
         * 16: Banka Kartı
         * 17: hotdog Stand
         * 18: Kan tüpü
         * 19: BoomBox
         * 20: Melee Wep
         * 21: second Wep
         * 22: MainWep
         * 23: Yerleştirilebilir obje
         * 24: Saman (Hayvan beslemek için)
         * 25: Yavru inek (Farm'a yerleştirmek için)
         * 26: Et (Restoranlarda kullanılacak)
         * 27: Weed (Uyuşturucu parça 1)
         * 28: Weed Tohumu (Uyuşturucu üretim parça 1)
         */

        public static readonly List<ServerItems> _LSCitems = new List<ServerItems>() {
            new ServerItems { ID = 1, type = 1, name = "IFruit X", picture = "1", price = 5000, weight = 0.700, equipSlot = 12, objectModel= "ba_prop_battle_amb_phone" },
            new ServerItems { ID = 2, type = 2, name = "汉堡包", picture = "2", price = 7, weight = 0.200, data = "80", equipSlot = -1, objectModel = "prop_cs_burger_01" },
            new ServerItems { ID = 3, type = 3, name = "工具箱", picture = "3", price = 5, weight = 2.500, data = "2", data2 = "20", stackable = true, objectModel = "ch_prop_toolbox_01a" },
            new ServerItems { ID = 4, type = 4, name = "眼镜", picture = "4", price = 5, weight = 0.2, data = "1", data2 = "2", equipable = true, equipSlot = 1, objectModel = "prop_safety_glasses", isNameChange = true },
            new ServerItems { ID = 5, type = 4, name = "面具", picture = "5", price = 5, weight = 0.2, data = "1", data2 = "2", equipable = true, equipSlot = 2, objectModel = "prop_mask_specops" , isNameChange = true },
            new ServerItems { ID = 6, type = 4, name = "帽子", picture = "6", price = 5, weight = 0.1, data = "1", data2 = "2", equipable = true, equipSlot = 3, objectModel = "prop_ld_hat_01" , isNameChange = true },
            new ServerItems { ID = 7, type = 4, name = "配饰", picture = "7", price = 5, weight = 0.1, data = "1", data2 = "2", equipable = true, equipSlot = 4 , isNameChange = true},
            new ServerItems { ID = 8, type = 4, name = "上衣", picture = "8", price = 5, weight = 0.5, data = "1", data2 = "2", equipable = true, equipSlot = 5, objectModel= "prop_ld_shirt_01" , isNameChange = true },
            new ServerItems { ID = 9, type = 4, name = "内衬", picture = "9", price = 5, weight = 0.3, data = "1", data2 = "2", equipable = true, equipSlot = 6, objectModel = "prop_ld_tshirt_01" , isNameChange = true},
            new ServerItems { ID = 10, type = 4, name = "手镯", picture = "10", price = 5, weight = 0.1, data = "1", data2 = "2", equipable = true, equipSlot = 7 , isNameChange = true},
            new ServerItems { ID = 11, type = 4, name = "裤子", picture = "11", price = 5, weight = 0.5, data = "1", data2 = "2", equipable = true, equipSlot = 8, objectModel = "prop_ld_jeans_02" , isNameChange = true},
            new ServerItems { ID = 12, type = 4, name = "手表", picture = "12", price = 5, weight = 0.2, data = "1", data2 = "2", equipable = true, equipSlot = 9 , isNameChange = true},
            new ServerItems { ID = 13, type = 4, name = "防弹背心", picture = "13", price = 5, weight = 0.1, data = "1", data2 = "2", equipable = true, equipSlot = 10 },
            new ServerItems { ID = 14, type = 4, name = "鞋子", picture = "14", price = 5, weight = 1, data = "1", data2 = "2", equipable = true, equipSlot = 11, objectModel = "prop_ld_shoe_02" , isNameChange = true},
            new ServerItems { ID = 15, type = 5, name = "香烟", picture = "15", price = 5, weight = 0.1, data = "1", data2 = "2", stackable = true, objectModel = "prop_cigar_pack_01" },
            new ServerItems { ID = 16, type = 6, name = "打火机", picture = "16", price = 5, weight = 0.1, data = "1", data2 = "2", objectModel = "p_cs_lighter_01" },
            new ServerItems { ID = 17, type = 7, name = "驾驶证", picture = "17", weight = 0.01, objectModel = "p_ld_id_card_002" },
            new ServerItems { ID = 18, type = 8, name = "加油罐", picture = "18", weight = 5, objectModel = "w_ch_jerrycan", data = "20"},
            new ServerItems { ID = 19, type = 9, name = "木头", picture = "19", weight = 1, stackable = true, objectModel = "prop_fncwood_13c" },
            new ServerItems { ID = 20, type = 10, name = "轮胎", picture = "20", weight = 20, objectModel = "prop_wheel_01" },
            new ServerItems { ID = 21, type = 13, name = "作物种子", picture = "21", weight = 0.5, stackable = true },
            new ServerItems { ID = 22, type = 14, name = "水桶", picture = "22", weight = 1.5 ,objectModel = "vw_prop_vw_ice_bucket_02a", stackable = true},
            new ServerItems { ID = 23, type = 15, name = "作物", picture = "23", weight = 1.5, objectModel = "bkr_prop_weed_bucket_01a", stackable = true },
            new ServerItems { ID = 24, type = 16, name = "银行卡", picture = "24", weight = 0.01, data = "", data2 = "", objectModel = "prop_cs_credit_card" },
            new ServerItems { ID = 25, type = 17, name = "热狗摊", picture = "25", weight = 7.5, data = "", data2 = ""},
            new ServerItems { ID = 26, type = 18, name = "采血管", picture = "26", weight = 0.5, data = "", data2 = "", objectModel = "bkr_prop_coke_mixtube_01" },
            new ServerItems { ID = 27, type = 19, name = "音响", picture = "27", weight = 2.5, data = "", data2 = "", objectModel = "prop_portable_hifi_01" },
            new ServerItems { ID = 28, type = 20, name = "小刀", picture = "28", weight = 3, equipSlot = 15, equipable = true, data = "", data2 = "", objectModel = "prop_knife" },
            new ServerItems { ID = 29, type = 21, name = "副武器", picture = "30", weight = 5,equipSlot=14, equipable = true, data = "", data2 = "", objectModel = "w_pi_combatpistol_luxe" },
            new ServerItems { ID = 30, type = 22, name = "主武器", picture = "29", weight = 10, equipSlot = 13, equipable = true, data = "", data2 = "", objectModel = "w_ar_carbinerifle_luxe" },
            new ServerItems { ID = 31, type = 23, name = "弹药", picture = "31", weight = 1, equipSlot = 13, equipable = true, data = "", data2 = ""},
            new ServerItems { ID = 32, type = 24, name = "稻草", picture = "32", weight = 1, stackable = true },
            new ServerItems { ID = 33, type = 25, name = "牛崽", picture = "33", weight = 5, stackable = true },
            new ServerItems { ID = 34, type = 26, name = "新鲜牛肉", picture = "34", weight = 1, objectModel = "ng_proc_binbag_02a", stackable = true},
            // DRUG SYSTEM ITEMS
            new ServerItems { ID = 35, type = 27, name = "大麻", picture = "35", weight = 0.1, objectModel = "prop_weed_bottle" },
            new ServerItems { ID = 36, type = 28, name = "大麻种子", picture = "36", weight = 0.5, objectModel = "bkr_prop_weed_bud_01a" },
            new ServerItems { ID = 37, type = 29, name = "大麻提取物", picture = "37", weight = 0.5, objectModel = "bkr_prop_weed_spray_01a" },            
            // DRUG SYSTEM ITEMS END
            new ServerItems { ID = 38, type = 30, name = "钓鱼竿", picture = "38", weight = 1, data = "100" },
            new ServerItems { ID = 39, type = 31, name = "新鲜鱼", picture = "39", weight = 0.5, stackable = true },
            new ServerItems { ID = 40, type = 32, name = "药片", picture = "40", weight = 0.5 },
            new ServerItems { ID = 41, type = 27, name = "摇头丸", picture = "41", weight = 0.1, objectModel = "prop_weed_bottle" },
            new ServerItems { ID = 42, type = 27, name = "海洛因", picture = "42", weight = 0.1, objectModel = "prop_weed_bottle" },
            new ServerItems { ID = 43, type = 27, name = "可卡因", picture = "43", weight = 0.1, objectModel = "prop_weed_bottle" },
            new ServerItems { ID = 44, type = 27, name = "LSD", picture = "44", weight = 0.1, objectModel = "prop_weed_bottle" },//
            new ServerItems { ID = 45, type = 27, name = "冰毒", picture = "45", weight = 0.1, objectModel = "prop_weed_bottle" },
            new ServerItems { ID = 46, type = 33, name = "弹匣", picture = "46", weight = 0.1, objectModel = "w_ar_advancedrifle_luxe_mag1" },
            new ServerItems { ID = 47, type = 34, name = "武器配件", picture = "47", weight = 0.1, objectModel = "w_at_scope_macro_2" },
            new ServerItems { ID = 48, type = 35, name = "武器喷漆", picture = "48", weight = 0.1, objectModel = "prop_cs_spray_can" },
            new ServerItems { ID = 49, type = 36, name = "尸体", picture = "49", weight = 26, objectModel = "prop_cs_spray_can" },
            new ServerItems { ID = 50, type = 37, name = "肉菜", picture = "50", weight = 1.0 },
            new ServerItems { ID = 51, type = 37, name = "鱼菜", picture = "51", weight = 1.0 },
            new ServerItems { ID = 52, type = 38, name = "葡萄", picture = "52", weight = 0.5, stackable = true },
            new ServerItems { ID = 53, type = 39, name = "葡萄酒", picture = "53", weight = 1.0, stackable = true},
            new ServerItems { ID = 54, type = 40, name = "车辆清洁工具", picture = "54", weight = 3, stackable = true},
            new ServerItems { ID = 55, type = 41, name = "帐篷", picture = "55", weight = 20},
            new ServerItems { ID = 56, type = 42, name = "灭火器", picture = "56", weight = 5, stackable = true },
            new ServerItems { ID = 57, type = 42, name = "烟盒", picture = "57", weight = 0.2, stackable = true },
            new ServerItems { ID = 58, type = 42, name = "包装食品", picture = "58", weight = 0.5, stackable = true },
            new ServerItems { ID = 59, type = 42, name = "易拉罐", picture = "59", weight = 0.2, stackable = true },
            new ServerItems { ID = 60, type = 42, name = "电视", picture = "60", weight = 5, stackable = true },
            new ServerItems { ID = 61, type = 42, name = "电脑", picture = "61", weight = 2, stackable = true },
            new ServerItems { ID = 62, type = 42, name = "家具", picture = "62", weight = 8, stackable = true },
            new ServerItems { ID = 63, type = 44, name = "开锁器", picture = "63", weight = 0.2, stackable = true },
            new ServerItems { ID = 64, type = 45, name = "契约文件", picture = "64", weight = 0.2, stackable = false, isNameChange = true },
            new ServerItems { ID = 65, type = 42, name = "宝石", picture = "65", weight = 0.5, stackable = true },
            new ServerItems { ID = 66, type = 42, name = "桌子", picture = "66", weight = 0.5, stackable = true },
            new ServerItems { ID = 67, type = 42, name = "邮票", picture = "67", weight = 0.5, stackable = true },
            new ServerItems { ID = 68, type = 42, name = "U盘录音文件", picture = "68", weight = 0.5, stackable = true },
            new ServerItems { ID = 69, type = 42, name = "香水", picture = "69", weight = 0.5, stackable = true },
            new ServerItems { ID = 70, type = 42, name = "陈年葡萄酒", picture = "70", weight = 0.5, stackable = true },
            new ServerItems { ID = 71, type = 42, name = "古董钟", picture = "71", weight = 0.5, stackable = true },
            // Farmin Items
            new ServerItems { ID = 72, type = 43, name = "牛奶", picture = "72", weight = 3, stackable = true },
            new ServerItems { ID = 73, type = 44, name = "鸡蛋", picture = "73", weight = 0.1, stackable = true },

            // 种植系统物品
            new ServerItems { ID = 74, type = 46, name = "菠萝种子", picture = "74", weight = 0.5, stackable = true },
            new ServerItems { ID = 75, type = 46, name = "白菜种子", picture = "74", weight = 0.6, stackable = true },
            new ServerItems { ID = 76, type = 46, name = "南瓜种子", picture = "74", weight = 0.7, stackable = true },
            new ServerItems { ID = 77, type = 46, name = "西红柿种子", picture = "74", weight = 0.8, stackable = true },
            new ServerItems { ID = 78, type = 47, name = "菠萝", picture = "75", weight = 1, stackable = true },
            new ServerItems { ID = 79, type = 47, name = "白菜", picture = "76", weight = 1.1, stackable = true },
            new ServerItems { ID = 80, type = 47, name = "南瓜", picture = "77", weight = 1.2, stackable = true },
            new ServerItems { ID = 81, type = 47, name = "西红柿", picture = "78", weight = 1.3, stackable = true },
        };
        
        public static List<ServerItems> LSCitems
        {
            get { return _LSCitems.ToList(); }
        }


        public static void LoadAllItems()
        {
           
        }
    }
    

}
