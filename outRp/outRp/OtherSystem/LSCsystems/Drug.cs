using System;
using System.Collections.Generic;
using System.Numerics;
using AltV.Net; using AltV.Net.Async;
using AltV.Net.Data;
using outRp.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;
using outRp.Chat;
using outRp.Globals;
using outRp.OtherSystem.Textlabels;
using System.Linq;
using AltV.Net.Resources.Chat.Api;

namespace outRp.OtherSystem.LSCsystems
{
    public class Drug : IScript
    {

        public static List<DrugFarm> serverDrugs = new List<DrugFarm>();

        public static void LoadServerDrugs(string data)
        {
            serverDrugs = JsonConvert.DeserializeObject<List<DrugFarm>>(data);
            foreach(DrugFarm d in serverDrugs)
            {
                d.PropId = (int)PropStreamer.Create("bkr_prop_weed_01_small_01a", d.pos, new Rotation(0, 0, 0), dimension: d.Dimension, frozen: true).Id;
                d.LabelId = (int)TextLabelStreamer.Create("~g~[大麻植株]~w~~n~成熟度: " + 0 + "~g~%", d.pos, dimension: d.Dimension).Id;
                UpdateDrugInfo(d, 0, 0);
            }
        }




        public static bool UseDrug(PlayerModel p, string d)
        {
            DrugModel drug = JsonConvert.DeserializeObject<DrugModel>(d);
            if (drug == null)
                return false;


            switch (drug.type)
            {
                case 1:
                    return Use_Weed(p, drug.quality, drug.infection);

            }
            return false;
        }

        public static bool Use_Weed(PlayerModel p, int quality, double infection)
        {
            DiseaseModel weedAddiction = p.injured.diseases.Find(x => x.DiseaseName == "毒瘾");
            if (weedAddiction == null) { weedAddiction = new DiseaseModel() { DiseaseName = "毒瘾", DiseaseValue = 1 };
                p.injured.diseases.Add(weedAddiction);
            }

            int upgradeVal = RandomInfectionLevel(weedAddiction.DiseaseValue);
            CharacterSettings set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            if(set.drugEvents.DrugLevel == 0) { set.drugEvents.DrugLevel = 1; }
            if(set.drugEvents.lastUsed > DateTime.Now) { MainChat.SendErrorChat(p, "[!] 吸食冷却时间 - 吸食后 30 分钟内不可再次吸食"); return false; }

            set.drugEvents.DrugExp += 1;
            if(set.drugEvents.DrugExp >= (set.drugEvents.DrugLevel * 4)) { set.drugEvents.DrugLevel += 1; set.drugEvents.DrugExp = 1; }
            set.drugEvents.lastUsed = DateTime.Now.AddMinutes(30);
            set.drugEvents.DrugWantLevel = 1;
            p.settings = JsonConvert.SerializeObject(set);
            p.updateSql();

            if(quality <= 85) { weedAddiction.DiseaseValue += 1; }
            if (upgradeVal >= weedAddiction.DiseaseValue) { weedAddiction.DiseaseValue += 1; }

            if (weedAddiction.DiseaseValue >= 10)
            {
                MainChat.EmoteDo(p, "可以观察到这个人的眼神迷离, 全身在发抖.");
                p.Health -= 200;
            }
            else if(weedAddiction.DiseaseValue >= 7 && weedAddiction.DiseaseValue < 10)
            {
                MainChat.EmoteDo(p, "可以观察到这个人的眼神有点迷离, 手在发抖.");
                p.Health -= 100;
            }
            else if (weedAddiction.DiseaseValue >= 4 && weedAddiction.DiseaseValue < 7)
            {
                MainChat.EmoteDo(p, "可以观察到这个人的眼睛开始变红, 嘴唇变干.");
                p.Health -= 50;
            }
            else
            {
                MainChat.EmoteDo(p, "可以观察到他的手开始发.");
            }


            p.EmitLocked("Drug:UseWeed", set.drugEvents.DrugLevel);
            p.tempStrength = ((quality * set.drugEvents.DrugLevel) / 10) / 4;
            p.Armor += (ushort)((quality * set.drugEvents.DrugLevel) / 10);
            if(p.Armor > 100) { p.Armor = 100; }
            return true;
        }

        [AsyncClientEvent("Drug:StateEnd")]
        public void DrugStateEnd(PlayerModel p)
        {
            p.tempStrength = 0;
            return;
        }
        public static int RandomInfectionLevel(int AddictionLevel)
        {
            Random rnd = new Random();
            int result = rnd.Next(0, (AddictionLevel * 5));
            result = (result / 4) - AddictionLevel;

            return result;
        }

        //   1 : bkr_prop_weed_01_small_01a | 2: bkr_prop_weed_med_01a | 3: bkr_prop_weed_lrg_01b
        //
        //
        //
        public static bool PlaceWeed(PlayerModel p)
        {
            if(p.Dimension <= 0) { return false; }
            GlobalEvents.ShowObjectPlacement(p, "bkr_prop_weed_01_small_01a", "Drug:Place");
            return true;
        }

        [AsyncClientEvent("Drug:Place")]
        public void DrugPlace(PlayerModel p, string rot, string pos, string model)
        {
            Vector3 position = JsonConvert.DeserializeObject<Vector3>(pos);
            position.Z += 0.2f;
            Vector3 rotation = JsonConvert.DeserializeObject<Vector3>(rot);

            DrugFarm farm = new DrugFarm()
            {
                PropId = (int)PropStreamer.Create(model, position, rotation, dimension: p.Dimension, frozen: true).Id,
                LabelId = (int)TextLabelStreamer.Create("~g~[大麻植株]~w~~n~成熟度: " + 0 + "~g~%", position, dimension: p.Dimension, streamRange: 5).Id,
                growthLevel = 1,
                pos = position,
                Dimension = p.Dimension,
                owner = p.sqlID,
            };

            serverDrugs.Add(farm);
            MainChat.SendInfoChat(p, "您开始种植大麻了, 请细心照料植株, 否则成品的质量会降低. (可以用 大麻提取物 加快植株生长)");
            return;
        }

        // Sunucu tarafından check edilerek belirli aralıklarla büyümesi sağlanacak.
        public static void UpdateDrugInfo(DrugFarm drug, int growthLevel = 0, int Quality = 0)
        {
            drug.growthLevel += growthLevel;
            drug.Quality += Quality;
            if (drug.Quality >= 100) drug.Quality = 100;
            if (drug.Quality <= 0) drug.Quality = 0;

            LProp drugProp = PropStreamer.GetProp((ulong)drug.PropId);
            if(drugProp == null)
            {
                drug.PropId = (int)PropStreamer.Create("bkr_prop_weed_01_small_01a", drug.pos, new Rotation(0, 0, 0), dimension: drug.Dimension, frozen: true).Id;
            }

            LProp drugP = PropStreamer.GetProp((ulong)drug.PropId);
            PlayerLabel drugT = TextLabelStreamer.GetDynamicTextLabel((ulong)drug.LabelId);
            if (drugP == null || drugT == null)
                return;

            if (drug.growthLevel <= 5)
            {
                if (drugP.Model != "bkr_prop_weed_01_small_01a") drugP.Model = "bkr_prop_weed_01_small_01a";
            }
            else if(drug.growthLevel > 5 && drug.growthLevel <= 9)
            {
                if (drugP.Model != "bkr_prop_weed_med_01a")
                {
                    drugP.Model = "bkr_prop_weed_med_01a";
                    Position calc = drug.pos;
                    calc.Z -= 2;
                    drugP.Position = calc;
                }
            }
            else if(drug.growthLevel >= 10)
            {
                if (drugP.Model != "bkr_prop_weed_lrg_01b") 
                {
                    drugP.Model = "bkr_prop_weed_lrg_01b";
                    Position calc = drug.pos;
                    calc.Z -= 2;
                    drugP.Position = calc;
                } 

                if(drug.growthLevel > 10) drug.Quality -= Quality * 3;
                drug.growthLevel = 10;

                
            }
            else
            {
                if (drugP.Model != "bkr_prop_weed_01_small_01a") drugP.Model = "bkr_prop_weed_01_small_01a";
            }

            if(drug.growthLevel >= 10)
            {
                drugT.Text = "~g~[大麻植株]~w~~n~成熟度: " + drug.growthLevel + "0~g~%~w~~n~质量: " + drug.Quality + "~g~%~w~~n~可输入 /getdrug 收获成品";
            }
            else
            {
                drugT.Text = "~g~[大麻植株]~w~~n~成熟度: " + drug.growthLevel + "0~g~%~w~~n~质量: " + drug.Quality + "~g~%";
            }
            drugT.Scale = 0.7f;
            drugT.Range = 5;
            drugT.Dimension = drug.Dimension;
            drugP.Dimension = drug.Dimension;
            return;
        }


        // Su verirken kullanılacak.
        public static bool InteractionDrugFarm(PlayerModel p)
        {
            //DrugFarm nFarm = serverDrugs.Find(x => x.pos.Distance(p.Position) < 2.5f && x.Dimension == p.Dimension);
            DrugFarm nFarm = serverDrugs.Where(x => x.pos.Distance(p.Position) < 2.5f && x.Dimension == p.Dimension).OrderBy(x => x.pos.Distance(p.Position)).FirstOrDefault();
            if(nFarm == null) { MainChat.SendErrorChat(p, "附近没有大麻植株."); return false; }

            if(nFarm.lastUsed >= DateTime.Now) { MainChat.SendErrorChat(p, "您在最近时间已浇过水了, 不需要频繁浇水."); return false; }
            if(nFarm.growthLevel >= 8) { MainChat.SendErrorChat(p, "[错误] 成熟度超过 80% 后不能对植株使用大麻提取物."); return false; }
            nFarm.lastUsed = DateTime.Now.AddMinutes(45);

            UpdateDrugInfo(nFarm, 0, 5);

            MainChat.SendInfoChat(p, "您已浇灌 大麻植株, 您可以在 45 分钟后再次浇灌.");
            return true;
        }

        [Command("getdrug")]
        public async Task COM_GetDrug(PlayerModel p)
        {
            DrugFarm gFarm = serverDrugs.Find(x => x.pos.Distance(p.Position) < 3 && x.Dimension == p.Dimension);
            if(gFarm == null) { MainChat.SendErrorChat(p, "附近没有可以收集的大麻植株."); return; }

            if(gFarm.growthLevel < 10) { MainChat.SendErrorChat(p, "再等等吧, 这颗植株还未生长成型."); return; }

            ServerItems i = Items.LSCitems.Find(x => x.ID == 35);
            i.name = "大麻 " + gFarm.Quality + "%";
            i.amount = 1;
            DrugModel nDrug = new DrugModel()
            {
                type = 1,
                quality = 100,
                infection = (double)gFarm.Quality / 3,
            };

            i.data2 = JsonConvert.SerializeObject(nDrug);
            Random rnd = new Random();
            bool result = await Inventory.AddInventoryItem(p, i, rnd.Next(2, 10));

            if (!result) { MainChat.SendErrorChat(p, "[错误] 您的背包满了."); return; }
            TextLabelStreamer.GetDynamicTextLabel((ulong)gFarm.LabelId).Delete();
            PropStreamer.GetProp((ulong)gFarm.PropId).Delete();
            serverDrugs.Remove(gFarm);
            MainChat.SendInfoChat(p, "> 您已收获成品.");
            return;
        }

        public static void COM_RemoveDrugOnGround(PlayerModel p) // Sadece PD tarafından kullanılabilir.
        {
            DrugFarm rDrug = serverDrugs.Find(x => x.pos.Distance(p.Position) < 1 && x.Dimension == p.Dimension);
            if(rDrug == null) { MainChat.SendErrorChat(p, "[错误] 附近没有大麻植株."); return; }

            TextLabelStreamer.GetDynamicTextLabel((ulong)rDrug.LabelId).Delete();
            PropStreamer.GetProp((ulong)rDrug.PropId).Delete();
            serverDrugs.Remove(rDrug);

            MainChat.SendInfoChat(p, "> 已销毁附近的大麻植株.");
            return;
        }
    }

    public class DrugModel
    {
        public int type { get; set; } = 1;
        public int quality { get; set; } = 5;
        public double infection { get; set; } = 1.0;
    }

    // {type:1, quality: 5, infection: 1.0}

    public class DrugFarm
    {
        public int PropId { get; set; } = 1;
        public int LabelId { get; set; } = 1;
        public int growthLevel { get; set; } = 1;
        public DateTime lastUsed { get; set; } = DateTime.Now;
        public int Quality { get; set; } = 100;
        public Position pos { get; set; } = new Position(0, 0, 0);
        public int Dimension { get; set; } = 1;
        public int owner { get; set; } = 0;
    }
}
