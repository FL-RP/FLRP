using AltV.Net.Data;
//using Npgsql;
//using MySql.Data.MySqlClient;
using MySqlConnector;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace outRp.Company.Database
{
    class BusinessDatabase
    {
        private static string connectionString = "Server=localhost;Database=altv_business;Uid=root;Pwd='root';";


        // Create Business
        public static async Task<int> CreateCompany(string Name, int Type, int Owner)
        {
            int resultId = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO company (name, owner, type, level, approved, cash) VALUES (@name, @owner, @type, @level, @approved, @cash) ";

                command.Parameters.AddWithValue("@name", Name);
                command.Parameters.AddWithValue("@owner", Owner);
                command.Parameters.AddWithValue("@type", Type);
                command.Parameters.AddWithValue("@level", 1);
                command.Parameters.AddWithValue("@approved", false);
                command.Parameters.AddWithValue("@cash", 0);


                await command.ExecuteNonQueryAsync();
                resultId = (int)command.LastInsertedId;
                connection.Close();
            }
            return resultId;
        }


        // Delete Business
        public static async Task DeleteCompany(int ID)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM company WHERE ID = @ID;";
                command.Parameters.AddWithValue("@ID", ID);

                await command.ExecuteNonQueryAsync();

                connection.Close();
            }
        }
        public static async Task DeleteCompany(string Name)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM company WHERE name = @name;";
                command.Parameters.AddWithValue("@name", Name);

                await command.ExecuteNonQueryAsync();

                connection.Close();
            }
            return;
        }

        // Update Business
        public static async Task UpdateCompany(Models.CompanyModel biz)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE company SET name = @name, owner = @owner, type = @type, level = @level, approved = @approved," +
                    "cash = @cash, businessprice = @businessprice, moral = @moral, canbuymoral = @canbuymoral, storedcash = @storedcash WHERE ID = @ID";

                command.Parameters.AddWithValue("@name", biz.Name);
                command.Parameters.AddWithValue("@owner", biz.Owner);
                command.Parameters.AddWithValue("@type", biz.Type);
                command.Parameters.AddWithValue("@level", biz.Level);
                command.Parameters.AddWithValue("@approved", biz.Approved);
                command.Parameters.AddWithValue("@cash", biz.Cash);
                command.Parameters.AddWithValue("@businessprice", biz.BusinessPrice);
                command.Parameters.AddWithValue("@moral", biz.Moral);
                command.Parameters.AddWithValue("@canbuymoral", biz.CanBuyMoral);
                command.Parameters.AddWithValue("@storedcash", biz.storedCash);

                command.Parameters.AddWithValue("@ID", biz.ID);

                await command.ExecuteNonQueryAsync();

                connection.Close();
            }
            return;
        }
        // Get Business
        public static async Task<Models.CompanyModel> GetCompany(int ID)
        {
            Models.CompanyModel biz = new Models.CompanyModel();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM company WHERE ID = @ID;";
                command.Parameters.AddWithValue("@ID", ID);

                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();

                        biz.ID = reader.GetInt32("ID");
                        biz.Name = reader.GetString("name");
                        biz.Owner = reader.GetInt32("owner");
                        biz.Type = reader.GetInt32("type");
                        biz.Level = reader.GetInt32("level");
                        biz.Approved = reader.GetBoolean("approved");
                        biz.Cash = reader.GetInt32("cash");
                        biz.BusinessPrice = reader.GetInt32("businessprice");
                        biz.Moral = reader.GetInt32("moral");
                        biz.CanBuyMoral = reader.GetBoolean("canbuymoral");
                        biz.storedCash = reader.GetInt32("storedcash");
                    }
                    else
                    {
                        biz = null;
                    }
                }
                connection.Close();
            }
            return biz;
        }

        public static async Task<List<Models.CompanyModel>> getAllCompanys()
        {
            List<Models.CompanyModel> comps = new List<Models.CompanyModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM company";

                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {


                            comps.Add(new Models.CompanyModel
                            {
                                ID = reader.GetInt32("ID"),
                                Name = reader.GetString("name"),
                                Owner = reader.GetInt32("owner"),
                                Type = reader.GetInt32("type"),
                                Level = reader.GetInt32("level"),
                                Approved = reader.GetBoolean("approved"),
                                Cash = reader.GetInt32("cash"),
                                BusinessPrice = reader.GetInt32("businessprice"),
                                Moral = reader.GetInt32("moral"),
                                CanBuyMoral = reader.GetBoolean("canbuymoral"),
                                storedCash = reader.GetInt32("storedcash")
                            });
                        }
                    }
                }
                connection.Close();
            }
            return comps;
        }
        public static Models.CompanyModel GetPlayerOwnCompany(int sqlID)
        {
            Models.CompanyModel biz = new Models.CompanyModel();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM company WHERE owner = @ID;";
                command.Parameters.AddWithValue("@ID", sqlID);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();

                        biz.ID = reader.GetInt32("ID");
                        biz.Name = reader.GetString("name");
                        biz.Owner = reader.GetInt32("owner");
                        biz.Type = reader.GetInt32("type");
                        biz.Level = reader.GetInt32("level");
                        biz.Approved = reader.GetBoolean("approved");
                        biz.Cash = reader.GetInt32("cash");
                        biz.BusinessPrice = reader.GetInt32("businessprice");
                        biz.Moral = reader.GetInt32("moral");
                        biz.CanBuyMoral = reader.GetBoolean("canbuymoral");
                        biz.storedCash = reader.GetInt32("storedcash");
                    }
                    else
                    {
                        biz = null;
                    }
                }
                connection.Close();
            }
            return biz;
        }


        // Create Business Vault
        public static async Task<int> CreateComponent(Models.Components component)
        {
            int resultId = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO components (type, ownerbusiness, objectpos, objectrot, stock_1, stock_2, stock_3, securitylevel)" +
                    " VALUES (@type, @ownerbusiness, @objectpos, @objectrot, @stock_1, @stock_2, @stock_3, @securitylevel) ";

                command.Parameters.AddWithValue("@type", component.Type);
                command.Parameters.AddWithValue("@ownerbusiness", component.OwnerBusiness);
                command.Parameters.AddWithValue("@objectpos", JsonConvert.SerializeObject(component.ObjectPos));
                command.Parameters.AddWithValue("@objectrot", JsonConvert.SerializeObject(component.ObjectRot));
                command.Parameters.AddWithValue("@stock_1", component.Stock_1);
                command.Parameters.AddWithValue("@stock_2", component.Stock_2);
                command.Parameters.AddWithValue("@stock_3", component.Stock_3);
                command.Parameters.AddWithValue("@securitylevel", component.SecurityLevel);


                await command.ExecuteNonQueryAsync();
                resultId = (int)command.LastInsertedId;
                connection.Close();
            }
            return resultId;
        }

        // Update Business Vault
        public static async Task UpdateComponent(Models.Components comp)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE components SET type = @type, ownerbusiness = @ownerbusiness, objectpos = @objectpos, objectrot = @objectrot," +
                    "stock_1 = @stock_1, stock_2 = @stock_2, stock_3 = @stock_3, securitylevel = @securitylevel WHERE ID = @ID";

                command.Parameters.AddWithValue("@type", comp.Type);
                command.Parameters.AddWithValue("@ownerbusiness", comp.OwnerBusiness);
                command.Parameters.AddWithValue("@objectpos", JsonConvert.SerializeObject(comp.ObjectPos));
                command.Parameters.AddWithValue("@objectrot", JsonConvert.SerializeObject(comp.ObjectRot));
                command.Parameters.AddWithValue("@stock_1", comp.Stock_1);
                command.Parameters.AddWithValue("@stock_2", comp.Stock_2);
                command.Parameters.AddWithValue("@stock_3", comp.Stock_3);
                command.Parameters.AddWithValue("@securitylevel", comp.SecurityLevel);

                command.Parameters.AddWithValue("@ID", comp.ID);

                await command.ExecuteNonQueryAsync();

                connection.Close();
            }
            return;
        }
        // Delete Component
        public static async Task DeleteComponent(Models.Components comp)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM components WHERE ID = @ID;";
                command.Parameters.AddWithValue("@ID", comp.ID);

                await command.ExecuteNonQueryAsync();

                connection.Close();
            }
            return;
        }

        // Get Component
        public static async Task<Models.Components> GetComponents(int ID)
        {
            Models.Components comp = new Models.Components();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM components WHERE ID = @ID;";
                command.Parameters.AddWithValue("@ID", ID);

                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();

                        comp.ID = reader.GetInt32("ID");
                        comp.Type = reader.GetInt32("type");
                        comp.OwnerBusiness = reader.GetInt32("ownerbusiness");
                        comp.ObjectPos = JsonConvert.DeserializeObject<Position>(reader.GetString("objectpos"));
                        comp.ObjectRot = JsonConvert.DeserializeObject<Rotation>(reader.GetString("objectrot"));
                        comp.Stock_1 = reader.GetInt32("stock_1");
                        comp.Stock_2 = reader.GetInt32("stock_2");
                        comp.Stock_3 = reader.GetInt32("stock_3");
                        comp.SecurityLevel = reader.GetInt32("securtiylevel");
                    }
                    else
                    {
                        comp = null;
                    }
                }
                connection.Close();
            }
            return comp;
        }
        // Check Component Type
        public static bool CheckComponentWithType(int CompanyID, int Type)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM components WHERE ownerbusiness = @ownerbusiness AND type = @type;";
                command.Parameters.AddWithValue("@ownerbusiness", CompanyID);
                command.Parameters.AddWithValue("@type", Type);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        connection.Close();
                        return true;
                    }
                    else
                    {
                        connection.Close();
                        return false;
                    }
                }
            }
        }

        public static List<Models.Components> GetAllComponents()
        {
            List<Models.Components> comps = new List<Models.Components>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM components";


                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {


                            comps.Add(new Models.Components
                            {
                                ID = reader.GetInt32("ID"),
                                Type = reader.GetInt32("type"),
                                OwnerBusiness = reader.GetInt32("ownerbusiness"),
                                ObjectPos = JsonConvert.DeserializeObject<Position>(reader.GetString("objectpos")),
                                ObjectRot = JsonConvert.DeserializeObject<Rotation>(reader.GetString("objectrot")),
                                Stock_1 = reader.GetInt32("stock_1"),
                                Stock_2 = reader.GetInt32("stock_2"),
                                Stock_3 = reader.GetInt32("stock_3"),
                                SecurityLevel = reader.GetInt32("securitylevel"),

                            });
                        }
                    }
                }
                connection.Close();
            }
            return comps;
        }

        public static Models.Component_Stocs GetCompanyStocks(int CompanyID)
        {
            Models.Component_Stocs stock = new Models.Component_Stocs();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM company_stocks WHERE company_ID = @ID;";
                command.Parameters.AddWithValue("@ID", CompanyID);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();

                        stock.ID = reader.GetInt32("ID");
                        stock.General = reader.GetInt32("general");
                        stock.Car = reader.GetInt32("car");
                        stock.House = reader.GetInt32("house");
                        stock.Business = reader.GetInt32("business");
                        stock.Food = reader.GetInt32("food");
                        stock.Farm = reader.GetInt32("farm");
                        stock.CarModify = reader.GetInt32("carmodify");
                        stock.Cloth = reader.GetInt32("stock");
                        stock.GasStation = reader.GetInt32("gasstation");
                        stock.Weapon = reader.GetInt32("weapon");
                        stock.Stock = reader.GetInt32("stock");
                        stock.Ship = reader.GetInt32("ship");
                        stock.Plane = reader.GetInt32("plane");
                        stock.Hospital = reader.GetInt32("hospital");
                        stock.CityBuilding = reader.GetInt32("citybuilding");
                        stock.Electric = reader.GetInt32("electric");
                        stock.AutoPark = reader.GetInt32("autopark");
                        stock.Pharmacy = reader.GetInt32("pharmacy");
                    }
                    else
                    {
                        stock = null;
                    }
                }
                connection.Close();
            }

            return stock;
        }
        public static void UpdateCompanyStocks(Models.Component_Stocs comp)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE company_stocks SET general = @general, car = @car, house = @house, business = @business," +
                    "food = @food, farm = @farm, carmodify = @carmodify, cloth = @cloth, gasstation = @gasstation, weapon = @weapon, stock = @stock," +
                    "ship = @ship, plane = @plane, hospital = @hospital, citybuilding = @citybuilding, electric = @electric," +
                    "autopark = @autopark, pharmacy = @pharmacy WHERE ID = @ID";

                command.Parameters.AddWithValue("@general", comp.General);
                command.Parameters.AddWithValue("@car", comp.Car);
                command.Parameters.AddWithValue("@house", comp.House);
                command.Parameters.AddWithValue("@business", comp.Business);
                command.Parameters.AddWithValue("@food", comp.Food);
                command.Parameters.AddWithValue("@farm", comp.Farm);
                command.Parameters.AddWithValue("@carmodify", comp.CarModify);
                command.Parameters.AddWithValue("@cloth", comp.Cloth);
                command.Parameters.AddWithValue("@gasstation", comp.GasStation);
                command.Parameters.AddWithValue("@weapon", comp.Weapon);
                command.Parameters.AddWithValue("@stock", comp.Stock);
                command.Parameters.AddWithValue("@ship", comp.Ship);
                command.Parameters.AddWithValue("@plane", comp.Plane);
                command.Parameters.AddWithValue("@hospital", comp.Hospital);
                command.Parameters.AddWithValue("@citybuilding", comp.CityBuilding);
                command.Parameters.AddWithValue("@electric", comp.Electric);
                command.Parameters.AddWithValue("@autopark", comp.AutoPark);
                command.Parameters.AddWithValue("@pharmacy", comp.Pharmacy);

                command.Parameters.AddWithValue("@ID", comp.ID);

                command.ExecuteNonQuery();

                connection.Close();
            }
            return;
        }

        public static List<Models.Components> GetAllCompanyComponent(int companyID)
        {
            List<Models.Components> comps = new List<Models.Components>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM components WHERE ownerbusiness = @ownerbiz";
                command.Parameters.AddWithValue("@ownerbiz", companyID);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {


                            comps.Add(new Models.Components
                            {
                                ID = reader.GetInt32("ID"),
                                Type = reader.GetInt32("type"),
                                OwnerBusiness = reader.GetInt32("ownerbusiness"),
                                ObjectPos = JsonConvert.DeserializeObject<Position>(reader.GetString("objectpos")),
                                ObjectRot = JsonConvert.DeserializeObject<Rotation>(reader.GetString("objectrot")),
                                Stock_1 = reader.GetInt32("stock_1"),
                                Stock_2 = reader.GetInt32("stock_2"),
                                Stock_3 = reader.GetInt32("stock_3"),
                                SecurityLevel = reader.GetInt32("securitylevel"),

                            });
                        }
                    }
                }
                connection.Close();
            }
            return comps;
        }

        // Company SellPoints
        public static int CreateSellPoint(systems.sellingPoints.SellPoint point)
        {
            int resultId = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO sellpoints (owner_company, type, type_2, vault, pricemultipler, dimension, position, position_2)" +
                    " VALUES (@owner_company, @type, @type_2, @vault, @pricemultipler, @dimension, @position, @position_2) ";

                command.Parameters.AddWithValue("@owner_company", point.Owner_Company);
                command.Parameters.AddWithValue("@type", point.Type);
                command.Parameters.AddWithValue("@type_2", point.Type_2);
                command.Parameters.AddWithValue("@vault", point.vault);
                command.Parameters.AddWithValue("@pricemultipler", point.priceMultipler);
                command.Parameters.AddWithValue("@dimension", point.Dimension);
                command.Parameters.AddWithValue("@position", JsonConvert.SerializeObject(point.Position));
                command.Parameters.AddWithValue("@position_2", JsonConvert.SerializeObject(point.Position_2));

                command.ExecuteNonQuery();
                resultId = (int)command.LastInsertedId;
                connection.Close();
            }
            return resultId;
        }
        public static systems.sellingPoints.SellPoint GetSellPoint(int ID)
        {
            systems.sellingPoints.SellPoint point = new systems.sellingPoints.SellPoint();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM sellpoints WHERE ID = @ID;";
                command.Parameters.AddWithValue("@ID", ID);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();

                        point.ID = reader.GetInt32("ID");
                        point.Owner_Company = reader.GetInt32("owner_company");
                        point.stock = reader.GetInt32("stock");
                        point.Type = reader.GetInt32("type");
                        point.Type_2 = reader.GetInt32("type_2");
                        point.vault = reader.GetInt32("vault");
                        point.priceMultipler = reader.GetInt32("pricemultipler");
                        point.Dimension = reader.GetInt32("dimension");
                        point.Position = JsonConvert.DeserializeObject<Position>(reader.GetString("position"));
                        point.Position_2 = JsonConvert.DeserializeObject<Position>(reader.GetString("position_2"));
                        point.canBuy = reader.GetBoolean("canbuy");
                    }
                    else
                    {
                        point = null;
                    }
                }
                connection.Close();
            }
            return point;
        }

        public static void UpdateSellPoint(systems.sellingPoints.SellPoint sell)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE sellpoints SET owner_company = @owner_company, stock = @stock, type = @type, type_2 = @type_2, " +
                    "vault = @vault, pricemultipler = @pricemultipler, dimension = @dimension, position = @position, position_2 = @position_2, canbuy = @canbuy WHERE ID = @ID";

                command.Parameters.AddWithValue("@owner_company", sell.Owner_Company);
                command.Parameters.AddWithValue("@stock", sell.stock);
                command.Parameters.AddWithValue("@type", sell.Type);
                command.Parameters.AddWithValue("@type_2", sell.Type_2);
                command.Parameters.AddWithValue("@vault", sell.vault);
                command.Parameters.AddWithValue("@pricemultipler", sell.priceMultipler);
                command.Parameters.AddWithValue("@dimension", sell.Dimension);
                command.Parameters.AddWithValue("@position", JsonConvert.SerializeObject(sell.Position));
                command.Parameters.AddWithValue("@position_2", JsonConvert.SerializeObject(sell.Position_2));
                command.Parameters.AddWithValue("@canbuy", sell.canBuy);

                command.Parameters.AddWithValue("@ID", sell.ID);

                command.ExecuteNonQuery();

                connection.Close();
            }
            return;
        }

        public static bool CheckSellPointWithType(int companyID, int type)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM sellpoints WHERE owner_company = @ID AND type = @type;";
                command.Parameters.AddWithValue("@ID", companyID);
                command.Parameters.AddWithValue("@type", type);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        connection.Close();
                        return true;
                    }
                    else
                    {
                        connection.Close();
                        return false;
                    }
                }
            }
        }

        public static void DeleteSellPoint(int ID)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM sellpoints WHERE ID = @ID;";
                command.Parameters.AddWithValue("@ID", ID);

                command.ExecuteNonQuery();

                connection.Close();
            }
        }
        // Get all SellPoints
        public static List<systems.sellingPoints.SellPoint> GetAllSellPoints()
        {
            List<systems.sellingPoints.SellPoint> comps = new List<systems.sellingPoints.SellPoint>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM sellpoints";


                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {


                            comps.Add(new systems.sellingPoints.SellPoint
                            {
                                ID = reader.GetInt32("ID"),
                                Owner_Company = reader.GetInt32("owner_company"),
                                stock = reader.GetInt32("stock"),
                                Type = reader.GetInt32("type"),
                                Type_2 = reader.GetInt32("type_2"),
                                vault = reader.GetInt32("vault"),
                                priceMultipler = reader.GetInt32("pricemultipler"),
                                Dimension = reader.GetInt32("dimension"),
                                Position = JsonConvert.DeserializeObject<Position>(reader.GetString("position")),
                                Position_2 = JsonConvert.DeserializeObject<Position>(reader.GetString("position_2")),
                                canBuy = reader.GetBoolean("canbuy")
                            });
                        }
                    }
                }
                connection.Close();
            }
            return comps;
        }
    }
}
