using System.Collections.Generic;
using AltV.Net; using AltV.Net.Async;
using outRp.Models;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;
using Newtonsoft.Json;
using outRp.Chat;

namespace outRp.Company.systems
{
    public class Bourse : IScript
    {

        public class Company_Bourse
        {
            public int ID { get; set; } = 0;
            public string Name { get; set; } = "无";
            public int TotalPrice { get; set; } = 0;
            public double Percent { get; set; } = 0;
            public int PlayerShareTotal { get; set; } = 0;
            public int LastShareBuyPrice { get; set; } = 0;
        }


        public static async Task<List<Company_Bourse>> getBourse()
        {
            List<Models.CompanyModel> companys = await Database.BusinessDatabase.getAllCompanys();
            List<Company_Bourse> bourse = new List<Company_Bourse>();

            long bourseTotal = 0;
            companys.ForEach(x =>
            {
                bourseTotal += x.BusinessPrice;
            });

            foreach(Models.CompanyModel comp in companys)
            {
                bourse.Add(new Company_Bourse { ID = comp.ID, Name = comp.Name, Percent = ((double)comp.BusinessPrice / bourseTotal) * 100, TotalPrice = comp.Cash });
            }

            return bourse;
        }


        [Command("shares")]
        public static async Task Bourse_Test(PlayerModel p)
        {
            List<Company_Bourse> bourse = await getBourse();

            CharacterSettings set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);

            bourse.ForEach(x =>
            {
                var comp = set.shares.Find(y => y.CompanyID == x.ID);
                if (comp != null)
                {
                    x.LastShareBuyPrice = comp.BuyPrice;
                    x.PlayerShareTotal = comp.ShareCount;
                }
            });

            await p.EmitAsync("Bourse:Show", JsonConvert.SerializeObject(bourse));
            return;
        }

        public static async Task UpdateBourse(PlayerModel p)
        {
            List<Company_Bourse> bourse = await getBourse();

            CharacterSettings set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);

            bourse.ForEach(x =>
            {
                var comp = set.shares.Find(y => y.CompanyID == x.ID);
                if (comp != null)
                {
                    x.LastShareBuyPrice = comp.BuyPrice;
                    x.PlayerShareTotal = comp.ShareCount;
                }
            });

            await p.EmitAsync("Bourse:Update", JsonConvert.SerializeObject(bourse));
            return;
        }

        
        [AsyncClientEvent("Share:Buy")]
        public async Task BuyShare(PlayerModel p, int companyID)
        {
            if (p.Ping > 250)
                return;

            var company = await Database.BusinessDatabase.GetCompany(companyID);
            if (company == null)
                return;

            if (!company.CanBuyMoral)
            {
                MainChat.SendErrorChat(p, "[错误] 此公司的股票已经暂停售卖!");
                return;
            }

            var bourse = await getBourse();
            var bourseComp = bourse.Find(x => x.ID == company.ID);
            if (bourseComp == null)
                return;

            CharacterSettings set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            var check = set.shares.Find(x => x.CompanyID == companyID);
            if(check == null)
            {
                ShareModel newShare = new ShareModel()
                {
                    CompanyID = companyID,
                    ShareCount = 1,
                    BuyPrice = (int)(100 + (2 * bourseComp.Percent))
                };
                set.shares.Add(newShare);
            }
            else
            {
                if(check.ShareCount >= 500) { MainChat.SendErrorChat(p, "[错误] 您最多可以购买一家公司的 500 股票."); return; }
                check.ShareCount += 1;
            }

            if(p.cash < (int)(100 + (2 * bourseComp.Percent))) { MainChat.SendErrorChat(p, "[错误] 您没有足够的现金!"); return; }
            company.BusinessPrice -= (int)(95 + (2 * bourseComp.Percent));
            if (company.BusinessPrice <= 0)
                company.BusinessPrice = 0;
            company.Cash += (int)(100 + (2 * bourseComp.Percent));
            company.storedCash += (int)(100 + (2 * bourseComp.Percent));
            company.Update();

            p.cash -= (int)(100 + (2 * bourseComp.Percent));
            p.settings = JsonConvert.SerializeObject(set);
            await p.updateSql();

            MainChat.SendInfoChat(p, "[!] " + "成功购买 " + company.Name + " 的 1 股票! 价格: $" + (int)(100 + (2 * bourseComp.Percent)));
            await UpdateBourse(p);
            return;
        }

        [AsyncClientEvent("Share:Sell")]
        public async Task SellShare(PlayerModel p, int companyID)
        {
            if (p.Ping > 250)
                return;
            var company = await Database.BusinessDatabase.GetCompany(companyID);
            if (company == null)
                return;

            var bourse = await getBourse();
            var bourseComp = bourse.Find(x => x.ID == company.ID);
            if (bourseComp == null)
                return;

            CharacterSettings set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            var check = set.shares.Find(x => x.CompanyID == companyID);

            if(check == null) { MainChat.SendErrorChat(p, "[错误] 您在此公司没有股份!"); return; }
            if(check.ShareCount <= 0) { MainChat.SendErrorChat(p, "[错误] 您在此公司没有股份!"); return; }


            if(company.Cash <= (int)((100 + (2 * bourseComp.Percent) * 2))){ MainChat.SendErrorChat(p, "[错误] 此公司没有足够的钱支付这笔费用!"); return; }
            company.Cash -= (int)(95 + (2 * bourseComp.Percent));
            company.BusinessPrice += (int)((95 + (2 * bourseComp.Percent)) / 2);
            company.storedCash -= (int)(95 + (2 * bourseComp.Percent));
            if (company.storedCash <= 0)
                company.storedCash = 0;

            if (company.Cash <= 0)
                company.Cash = 0;
            company.Update();

            check.ShareCount -= 1;
            if(check.ShareCount <= 0)
            {
                set.shares.Remove(check);
            }

            p.cash += (int)(95 + (2 * bourseComp.Percent));
            p.settings = JsonConvert.SerializeObject(set);
            await p.updateSql();

            MainChat.SendInfoChat(p, "[?] " + "成功出售 " + company.Name + " 的 1 股票! 价格: $" + (int)(95 + (2 * bourseComp.Percent)));
            await UpdateBourse(p);
            return;
        }
    }
}
