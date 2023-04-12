using System;
using System.Collections.Generic;
using AltV.Net;
using outRp.Models;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;
using outRp.Chat;
using outRp.OtherSystem.Textlabels;


namespace outRp.Company
{
    class BusinesMain : IScript
    {

        // Event Loaders 
        private static int totalHoursTimer = 0;

        public static async Task CheckDayTimer()
        {
            totalHoursTimer += 1;
            if(totalHoursTimer >= 20)
            {
                totalHoursTimer = 0;
                systems.Component_System.DescreaseCompSecurity();
            }
        }

        public static async Task CompanyHourTimer()
        {
            await CheckDayTimer();
        }
        public static async Task LoadCompanySystems()
        {
            systems.Component_System.LoadAllComponents();
            await systems.Jobs.LoadJobSystems();
            await systems.sellingPoints.LoadAllCompanySellPoints();
            Alt.Log("公司系统加载完毕.");
        }


        #region Admin Command
        [Command("createcompany")]
        public async Task COM_CreateBusinnes(PlayerModel p, params string[] args)
        {
            if(p.adminLevel <= 3) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            if(args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /createcompany [类型] [名称]"); return; }
            if(!Int32.TryParse(args[0], out int Type)) { MainChat.SendInfoChat(p, "[用法] /createcompany [类型] [名称]"); return; }

            int bizID = await Models.CompanyModel.CreateBusiness(p, string.Join(" ", args[1..]), Type);
            MainChat.SendInfoChat(p, "[!!] 公司 [" + bizID + "] " + string.Join(" ", args[1..]) + " 成功创建.");
            return;
        }

        [Command("editcompany")]
        public async Task COM_EditBusiness(PlayerModel p, params string[] args)
        {
            if(p.adminLevel <= 3) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            if(args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /editcompany [ID] [选项] [数值]"); return; }
            if(!Int32.TryParse(args[0], out int BizID)) { MainChat.SendInfoChat(p, "[用法] /editcompany [ID] [选项] [数值]"); return; }

            Models.CompanyModel biz = await Database.BusinessDatabase.GetCompany(BizID);
            if(biz == null) { MainChat.SendErrorChat(p, "[错误] 输入的公司不存在."); return; }
            


            switch (args[1])
            {
                case "name":
                    biz.Name = string.Join(" ", args[2..]);
                    await biz.Update();
                    MainChat.SendInfoChat(p, "[!] 成功更新公司名称为: " + biz.Name);
                    return;

                case "type":
                    if(args.Length <= 2) { MainChat.SendErrorChat(p, "[用法] /editcompany [ID] type [数值]"); return; }
                    if(!Int32.TryParse(args[2], out int bizType)) { MainChat.SendErrorChat(p, "[用法] /editcompany [ID] type [数值]"); return; }
                    biz.Type = bizType;
                    var typeName = (global.variables.CompanyTypes)biz.Type;
                    await biz.Update();
                    MainChat.SendInfoChat(p, "[!] 成功更新公司类型为: [" + biz.Type + "]" + typeName.ToString());
                    return;

                case "owner":
                    if(args.Length <= 2) { MainChat.SendInfoChat(p, "[用法] /editcompany [ID] owner [玩家ID]"); return; }
                    if(!Int32.TryParse(args[2], out int newOwner)) { MainChat.SendInfoChat(p, "[用法] /editcompany [ID] owner [玩家ID]"); return; }
                    biz.Owner = newOwner;
                    await biz.Update();
                    PlayerModelInfo newOwnerInfo = await outRp.Database.DatabaseMain.getCharacterInfo(newOwner);

                    if(newOwnerInfo == null) { MainChat.SendErrorChat(p, "[!] 成功更新公司创始人. [但是未找到输入的玩家ID]"); return; }
                    MainChat.SendInfoChat(p, "[!] 成功设置公司 " + biz.Name + " 的创始人为 [" + newOwner + "]" + newOwnerInfo.characterName.Replace("_", " "));
                    return;

                case "level":
                    if(args.Length <= 2) { MainChat.SendInfoChat(p, "[用法] /editcompany [ID] level [等级]"); return; }
                    if(!Int32.TryParse(args[2], out int newlevel)) { MainChat.SendInfoChat(p, "[用法] /editcompany [ID] level [等级]"); return; }
                    biz.Level = newlevel;
                    await biz.Update();

                    MainChat.SendInfoChat(p, "[!] 成功设置公司 " + biz.Name + " 的等级为: " + newlevel);
                    return;

                case "cash":
                    if(args.Length <= 2) { MainChat.SendInfoChat(p, "[用法] /editcompany [ID] cash [数值]"); return; }
                    if(!Int32.TryParse(args[2], out int newCash)) { MainChat.SendInfoChat(p, "[用法] /editcompany [ID] cash [数值]"); return; }
                    biz.Cash = newCash;
                    await biz.Update();

                    MainChat.SendInfoChat(p, "[!] 成功设置公司 " + biz.Name + " 的现金为: " + newCash);
                    return;

                case "bizprice":
                    if(args.Length <= 2) { MainChat.SendInfoChat(p, "[用法] /editcompany [ID] bizprice [数值]"); return; }
                    if(!Int32.TryParse(args[2], out int newBizPrice)) { MainChat.SendInfoChat(p, "[用法] /editcompany [ID] bizprice [数值]"); return; }
                    biz.BusinessPrice = newBizPrice;
                    await biz.Update();

                    MainChat.SendInfoChat(p, "[!] 成功设置公司 " + biz.Name + " 的市值为: $" + newBizPrice);
                    return;
            }

        }

        [Command("editwarehouse")]
        public async Task COM_EditBusinessComp(PlayerModel p, params string[] args)
        {
            if(p.adminLevel <= 2) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            if (!Int32.TryParse(args[0], out int COMP_ID)) { MainChat.SendInfoChat(p, "[用法] /editwarehouse ID [选项] [数值]"); return; }

            var comp = systems.Component_System.GetComponentWithID(COMP_ID);
            if (comp == null) { MainChat.SendErrorChat(p, "[错误] 无效仓库货物点!"); return; }

            switch (args[1])
            {
                case "stock":
                    if (!Int32.TryParse(args[2], out int stock_1)) { MainChat.SendInfoChat(p, "[用法] /editwarehouse ID stock [数值]"); return; }

                    comp.Stock_1 = stock_1;
                    await comp.Update();

                    MainChat.SendInfoChat(p, "[?] 成功更新仓库库存.");
                    return;

                case "buy":
                    if (!Int32.TryParse(args[2], out int stock_2)) { MainChat.SendInfoChat(p, "[用法] /editwarehouse ID buy [数值]"); return; }
                    comp.Stock_2 = stock_2;
                    await comp.Update();

                    MainChat.SendInfoChat(p, "[?] 成功更新仓库采购价格.");
                    return;

                case "need":
                    if (!Int32.TryParse(args[2], out int stock_3)) { MainChat.SendInfoChat(p, "[用法] /editwarehouse ID need [数值]"); return; }
                    comp.Stock_3 = stock_3;
                    await comp.Update();

                    MainChat.SendInfoChat(p, "[?] 成功更新仓库采购需求.");
                    return;

                case "alarm":
                    if (!Int32.TryParse(args[2], out int securtiy)) { MainChat.SendInfoChat(p, "[用法] /editwarehouse ID alarm [数值]"); return; }
                    comp.SecurityLevel = securtiy;
                    await comp.Update();

                    MainChat.SendInfoChat(p, "[?] 成功更新仓库防盗等级.");
                    return;

                case "pos":
                    comp.ObjectPos = p.Position;
                    LProp posProp = PropStreamer.GetProp(comp.ObjectID);
                    if(posProp != null) { posProp.Position = p.Position; }
                    MainChat.SendInfoChat(p, "[?] 成功更新仓库货物点对象位置.");
                    return;

                case "rot":
                    comp.ObjectRot = p.Rotation;
                    LProp rotProp = PropStreamer.GetProp(comp.ObjectID);
                    if (rotProp != null) { rotProp.Rotation = p.Rotation; }
                    MainChat.SendInfoChat(p, "[?] 成功更新仓库货物点对象旋转.");
                    return;

            }
        }

        #endregion

        #region Global Events
        public static async Task<int> GetPlayerCompany(PlayerModel p)
        {
            FactionModel faction = await outRp.Database.DatabaseMain.GetFactionInfo(p.factionId);
            Models.CompanyModel company = await Database.BusinessDatabase.GetCompany(faction.company);
            if(company == null)
            {
                List<BusinessModel> bizList = await outRp.Database.DatabaseMain.GetMemberBusinessList(p);
                foreach(BusinessModel biz in bizList)
                {
                    if (biz.company != 0)
                        return biz.company;
                }
            }
            else { return company.ID; }//
            return -1;
        }

        #endregion

    }
}
