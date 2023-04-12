using System;
using outRp.Models;
using System.Threading.Tasks;

namespace outRp.Company.Models
{
    class CompanyModel
    {
        public int ID { get; set; }
        public string Name { get; set; } = "无";
        public int Owner { get; set; } = 0;
        public int Type { get; set; } = 0;
        public int Level { get; set; } = 1;
        public Boolean Approved { get; set; } = false;
        public int Cash { get; set; } = 0;
        public int BusinessPrice { get; set; } = 0;
        public int Moral { get; set; } = 0;
        public bool CanBuyMoral { get; set; } = false;
        public int storedCash { get; set; } = 0;
        public Task Update() => UpdateBusiness(this);
        public Task Delete() => DeleteBusiness(this);


        public static async Task<int> CreateBusiness(PlayerModel p, string Name, int type)
        {
            return await Database.BusinessDatabase.CreateCompany(Name, type, p.sqlID);
        }
        public static async Task DeleteBusiness(CompanyModel biz)
        {
            await Database.BusinessDatabase.DeleteCompany(biz.ID);
            return;
        }
        public static async Task DeleteBusiness(string Name)
        {
            await Database.BusinessDatabase.DeleteCompany(Name);
            return;
        }
        public static async Task DeleteBusiness(int ID)
        {
            await Database.BusinessDatabase.DeleteCompany(ID);
            return;
        }
        public static async Task UpdateBusiness(CompanyModel biz)
        {
            await Database.BusinessDatabase.UpdateCompany(biz);
            return;
        }
    }
}
