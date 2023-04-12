using AltV.Net.Data;
//using Npgsql;
using System.Threading.Tasks;
using outRp.OtherSystem.Textlabels;

namespace outRp.Company.Models
{
    class Components
    {
        public int ID { get; set; }
        public ulong ObjectID { get; set; } = 0;
        public ulong TextLabelID { get; set; } = 0;
        public int Type { get; set; } = 0;
        public int OwnerBusiness { get; set; } = 0;
        public Position ObjectPos { get; set; } = new Position(0, 0, 0);
        public Rotation ObjectRot { get; set; } = new Rotation(0, 0, 0);
        public int Stock_1 { get; set; } = 0;
        public int Stock_2 { get; set; } = 0;
        public int Stock_3 { get; set; } = 0;
        public int SecurityLevel { get; set; } = 0;
        public async Task Update() => await Update(this);
        
        

        public static async Task<int> Create(Components comp)
        {
            return await Database.BusinessDatabase.CreateComponent(comp);
        }
        public static async Task Update(Components comp)
        {
            PlayerLabel lbl = TextLabelStreamer.GetDynamicTextLabel(comp.TextLabelID);
            if(lbl != null)
            {
                lbl.Text = GetComponentDisplayName(comp);
                lbl.Font = 0;
                lbl.Position = comp.ObjectPos;
            }
            await Database.BusinessDatabase.UpdateComponent(comp);
            return;
        }
        public static async Task Delete(Components comp)
        {
            await Database.BusinessDatabase.DeleteComponent(comp);
            return;
        }

        public static string GetComponentDisplayName(Models.Components comp)
        {
            string typeString = GetComponentTypeName(comp.Type);

            return "~b~[" + comp.ID + "]~n~~w~类型: " + typeString + "~n~库存: " + comp.Stock_1 + "~n~采购价格: ~g~$" + comp.Stock_2 + "~w~~n~需求数量: ~r~" + comp.Stock_3 + "~n~~w~防盗等级: ~g~" + comp.SecurityLevel;
        }

        public static string GetComponentTypeName(int Type)
        {
            switch (Type)
            {
                case 0:
                    return "泥土";
                case 1:
                    return "木板";
                case 2:
                    return "混凝土";
                case 3:
                    return "塑料";
                case 4:
                    return "工业盐酸";
                case 5:
                    return "钢材";
                case 6:
                    return "铁";
                case 7:
                    return "电子材料";

                default:
                    return "杂货";
            }
        }
    }

    public class Component_Stocs
    {
        public int ID { get; set; } = 0;
        public int General { get; set; } = 0;
        public int Car { get; set; } = 0;
        public int House { get; set; } = 0;
        public int Business { get; set; } = 0;
        public int Food { get; set; } = 0;
        public int Farm { get; set; } = 0;
        public int CarModify { get; set; } = 0;
        public int Cloth { get; set; } = 0;
        public int GasStation { get; set; } = 0;
        public int Weapon { get; set; } = 0;
        public int Stock { get; set; } = 0;
        public int Ship { get; set; } = 0;
        public int Plane { get; set; } = 0;
        public int Hospital { get; set; } = 0;
        public int CityBuilding { get; set; } = 0;
        public int Electric { get; set; } = 0;
        public int AutoPark { get; set; } = 0;
        public int Pharmacy { get; set; } = 0;

        public void Update() => Database.BusinessDatabase.UpdateCompanyStocks(this);
    }
}
