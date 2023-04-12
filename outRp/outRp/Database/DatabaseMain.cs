using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
//using Npgsql;
using MySqlConnector;
using Newtonsoft.Json;
using outRp.Models;
using outRp.OtherSystem.LSCsystems;
using outRp.OtherSystem.Textlabels;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace outRp.Database
{
    public class DatabaseMain : IScript
    {
        #region DATABASE INFO
        private static readonly string connectionString = "Server=localhost;Database=altv;Uid=root;Pwd='root'";
        //private static readonly string connectionString = "Server=localhost;Database=altv;Uid=root;Pwd=''";
        private static readonly string forumConnectionString = "Server=ls-rp.cc;Database=lsrp_cc;Uid=lsrp_cc;Pwd='rjzxbRzY4zLDRf8d'";
        #endregion

        public static async Task<AccountModel> getAccInfo(int accountID)
        {
            AccountModel account = new AccountModel();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM accounts WHERE ID = @ID LIMIT 1";
                command.Parameters.AddWithValue("@ID", accountID);

                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        account.ID = reader.GetInt32("ID");
                        account.kookId = reader.GetString("kookId");
                        account.accountType = reader.GetInt32("accountType");
                        account.accountEmail = reader.GetString("accountEmail");
                        account.forumName = reader.GetString("forumName");
                        account.adminLevel = reader.GetInt32("adminLevel");
                        account.banned = reader.GetBoolean("banned");
                        account.characterLimit = reader.GetInt32("characterlimit");
                        account.isOnline = reader.GetBoolean("isOnline");
                        account.socialClubID = reader.GetUInt64("socialClub");
                        account.lscPoint = reader.GetInt32("lscpoint");

                        if (ulong.TryParse(reader.GetString("hwid"), out ulong hwid))
                            account.HwId = hwid;

                        if (ulong.TryParse(reader.GetString("hwidex"), out ulong hwidex))
                            account.HwIdEx = hwidex;

                        account.ReportCount = reader.GetInt32("reportcount");
                        account.QuestionCount = reader.GetInt32("questioncount");
                        account.AdversimentCount = reader.GetInt32("adversimentcount");
                        account.OtherData = JsonConvert.DeserializeObject<OtherData>(reader.GetString("otherdata"));
                    }
                }
                await connection.CloseAsync();
            }

            return account;
        }

        public static AccountModel getAccInfo2(int accountID)
        {
            AccountModel account = new AccountModel();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM accounts WHERE ID = @ID LIMIT 1";
                command.Parameters.AddWithValue("@ID", accountID);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        account.ID = reader.GetInt32("ID");
                        account.kookId = reader.GetString("kookId");
                        account.accountType = reader.GetInt32("accountType");
                        account.accountEmail = reader.GetString("accountEmail");
                        account.forumName = reader.GetString("forumName");
                        account.adminLevel = reader.GetInt32("adminLevel");
                        account.banned = reader.GetBoolean("banned");
                        account.characterLimit = reader.GetInt32("characterlimit");
                        account.isOnline = reader.GetBoolean("isOnline");
                        account.socialClubID = reader.GetUInt64("socialClub");
                        account.lscPoint = reader.GetInt32("lscpoint");

                        if (ulong.TryParse(reader.GetString("hwid"), out ulong hwid))
                            account.HwId = hwid;

                        if (ulong.TryParse(reader.GetString("hwidex"), out ulong hwidex))
                            account.HwIdEx = hwidex;

                        account.ReportCount = reader.GetInt32("reportcount");
                        account.QuestionCount = reader.GetInt32("questioncount");
                        account.OtherData = JsonConvert.DeserializeObject<OtherData>(reader.GetString("otherdata"));
                    }
                }
                connection.Close();
            }

            return account;
        }
        public static List<Login.UpgradeNotes> getUpdateInfo()
        {
            List<Login.UpgradeNotes> u = new List<Login.UpgradeNotes>();
            /*using (MySqlConnection connection = new MySqlConnection(forumConnectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM lsc_topics WHERE id_board = 5 AND is_sticky != 1 ORDER BY id_topic DESC LIMIT 3";
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                            MySqlCommand sCommand = connection.CreateCommand();
                            sCommand.CommandText = "SELECT body FROM lsc_messages WHERE id_message = @id_message";
                            sCommand.Parameters.AddWithValue("@id_message", reader.GetInt32("id_first_message"));
                            using(MySqlDataReader sReader = sCommand.ExecuteReader())
                            {
                                if (sReader.HasRows)
                                {
                                    while (sReader.Read())
                                    {
                                        Login.UpgradeNotes update = new Login.UpgradeNotes()
                                        {
                                            header = sReader.GetString("subject"),
                                            text = sReader.GetString("body")
                                        };
                                        u.Add(update);
                                    }
                                }
                            }
                            }
                        }
                    }
                    connection.Close();
            }*/
            return u;
        }
        public static async Task updateAccInfo(AccountModel x)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE accounts SET adminLevel = @adminLevel, banned = @banned, characterlimit = @characterlimit, socialClub = @socialClub, isOnline = @isOnline, lscpoint = @lscpoint," +
                    "hwid = @hwid, hwidex = @hwidex, reportcount = @reportcount, questioncount = @questioncount, adversimentcount = @adversimentcount, otherdata = @otherdata WHERE ID = @ID; ";

                command.Parameters.AddWithValue("@adminLevel", x.adminLevel);
                command.Parameters.AddWithValue("@banned", x.banned);
                command.Parameters.AddWithValue("@characterlimit", x.characterLimit);
                command.Parameters.AddWithValue("@socialClub", x.socialClubID);
                command.Parameters.AddWithValue("@isOnline", x.isOnline);
                command.Parameters.AddWithValue("@lscpoint", x.lscPoint);
                command.Parameters.AddWithValue("@hwid", x.HwId.ToString());
                command.Parameters.AddWithValue("@hwidex", x.HwIdEx.ToString());
                command.Parameters.AddWithValue("@reportcount", x.ReportCount);
                command.Parameters.AddWithValue("@questioncount", x.QuestionCount);
                command.Parameters.AddWithValue("@adversimentcount", x.AdversimentCount);
                command.Parameters.AddWithValue("@otherdata", JsonConvert.SerializeObject(x.OtherData));

                command.Parameters.AddWithValue("@ID", x.ID);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }

        public static void updateAccInfo2(AccountModel x)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE accounts SET adminLevel = @adminLevel, banned = @banned, characterlimit = @characterlimit, socialClub = @socialClub, isOnline = @isOnline, lscpoint = @lscpoint," +
                    "hwid = @hwid, hwidex = @hwidex, reportcount = @reportcount, questioncount = @questioncount, adversimentcount = @adversimentcount, otherdata = @otherdata WHERE ID = @ID; ";

                command.Parameters.AddWithValue("@adminLevel", x.adminLevel);
                command.Parameters.AddWithValue("@banned", x.banned);
                command.Parameters.AddWithValue("@characterlimit", x.characterLimit);
                command.Parameters.AddWithValue("@socialClub", x.socialClubID);
                command.Parameters.AddWithValue("@isOnline", x.isOnline);
                command.Parameters.AddWithValue("@lscpoint", x.lscPoint);
                command.Parameters.AddWithValue("@hwid", x.HwId.ToString());
                command.Parameters.AddWithValue("@hwidex", x.HwIdEx.ToString());
                command.Parameters.AddWithValue("@reportcount", x.ReportCount);
                command.Parameters.AddWithValue("@questioncount", x.QuestionCount);
                command.Parameters.AddWithValue("@adversimentcount", x.AdversimentCount);
                command.Parameters.AddWithValue("@otherdata", JsonConvert.SerializeObject(x.OtherData));

                command.Parameters.AddWithValue("@ID", x.ID);

                command.ExecuteNonQuery();

                connection.Close();
            }
            return;
        }

        public static async Task<bool> CheckSocialBan(ulong clubID)
        {
            bool banned = false;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM socialbans WHERE socialClubId = @ID LIMIT 1";
                command.Parameters.AddWithValue("@ID", clubID);

                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        banned = true;
                    }
                }
                await connection.CloseAsync();
            }

            return banned;
        }

        public static async Task AddSocialBan(ulong clubID, string AdminName, string reason)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO socialbans (socialClubId, Admin, reason) VALUES (@socialClubId, @Admin, @reason) ";

                command.Parameters.AddWithValue("@socialClubId", clubID);
                command.Parameters.AddWithValue("@Admin", AdminName);
                command.Parameters.AddWithValue("@reason", reason);

                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }
        }

        public static async Task<bool> CheckHwidBan(IPlayer p)
        {
            bool banned = false;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM hwidban WHERE hwid = @hwid LIMIT 1";
                command.Parameters.AddWithValue("@hwid", p.HardwareIdHash.ToString());

                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        banned = true;
                    }
                }
                await connection.CloseAsync();
            }

            return banned;

        }

        public static async Task AddHwidBan(PlayerModel p, string AdminName, string Reason)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO hwidban (hwid, hwidex, banner, reason, date) VALUES (@hwid, @hwidex, @banner, @reason, @date)";

                command.Parameters.AddWithValue("@hwid", p.HardwareIdHash);
                command.Parameters.AddWithValue("@hwidex", p.HardwareIdExHash);
                command.Parameters.AddWithValue("@banner", AdminName);
                command.Parameters.AddWithValue("@reason", Reason);
                command.Parameters.AddWithValue("@date", DateTime.Now.ToString());

                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }
        }

        public static async Task AddHwidBan(ulong hwid, ulong hwidex, string AdminName, string Reason)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO hwidban (hwid, hwidex, banner, reason, date) VALUES (@hwid, @hwidex, @banner, @reason, @date)";

                command.Parameters.AddWithValue("@hwid", hwid);
                command.Parameters.AddWithValue("@hwidex", hwidex);
                command.Parameters.AddWithValue("@banner", AdminName);
                command.Parameters.AddWithValue("@reason", Reason);
                command.Parameters.AddWithValue("@date", DateTime.Now.ToString());

                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }
        }

        #region ServerSettings
        public static async Task SaveServerSettings()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = "UPDATE serversettings SET serverMarkets = @serverMarkets, serverDoors = @serverDoors," +
                        "blips = @blips, playerObjects = @playerObjects, drugFarm = @drugFarm, serverObjects = @serverObjects, repairStations = @repairStations, corpses = @corpses," +
                        "vendors = @vendors, cementery = @cementery, jacknpc = @jacknpc, turfs = @turfs, tedavi = @tedavi WHERE ID = @ID; ";

                    //string farmSettings = OtherSystem.LSCsystems.farming.SaveFarmString();
                    //command.Parameters.AddWithValue("@farmSettings", JsonConvert.SerializeObject(OtherSystem.LSCsystems.farming.serverFarms));

                    command.Parameters.AddWithValue("@serverMarkets", OtherSystem.LSCsystems.Market.GetMarketsSaveString());

                    command.Parameters.AddWithValue("@serverDoors", JsonConvert.SerializeObject(OtherSystem.LSCsystems.DoorSystem.serverDoors));

                    //command.Parameters.AddWithValue("@dynamicBlips", JsonConvert.SerializeObject(OtherSystem.LSCsystems.BlipSystem.dynamicBlips));
                    command.Parameters.AddWithValue("@blips", JsonConvert.SerializeObject(OtherSystem.LSCsystems.BlipSystem.serverBlips));

                    command.Parameters.AddWithValue("@playerObjects", JsonConvert.SerializeObject(OtherSystem.LSCsystems.objectSystem.playerObjects));

                    command.Parameters.AddWithValue("@drugFarm", JsonConvert.SerializeObject(OtherSystem.LSCsystems.Drug.serverDrugs));

                    command.Parameters.AddWithValue("@serverObjects", JsonConvert.SerializeObject(OtherSystem.LSCsystems.GroundObjects.serverObjects));

                    command.Parameters.AddWithValue("@repairStations", JsonConvert.SerializeObject(OtherSystem.LSCsystems.AutoRepairSystem.repairSystem));

                    command.Parameters.AddWithValue("@corpses", JsonConvert.SerializeObject(OtherSystem.LSCsystems.cksystem.serverCorpses));

                    command.Parameters.AddWithValue("@vendors", JsonConvert.SerializeObject(OtherSystem.LSCsystems.WeedVendors.weedVendors));

                    //command.Parameters.AddWithValue("@piyango", JsonConvert.SerializeObject(OtherSystem.LSCsystems.piyango.serverPiyango));

                    //command.Parameters.AddWithValue("@oocmarketnpc", JsonConvert.SerializeObject(OtherSystem.LSCsystems.OOCmarket.npcList));

                    command.Parameters.AddWithValue("@cementery", JsonConvert.SerializeObject(OtherSystem.LSCsystems.Cemetery.cemeterys));

                    // JAck NPC
                    command.Parameters.AddWithValue("@jacknpc", JsonConvert.SerializeObject(OtherSystem.LSCsystems.JackingNPC.npcs));

                    command.Parameters.AddWithValue("@turfs", JsonConvert.SerializeObject(OtherSystem.TurfSystem.SaveTurfVoid()));

                    command.Parameters.AddWithValue("@tedavi", JsonConvert.SerializeObject(OtherSystem.LSCsystems.DynamicTedavi.points));

                    //command.Parameters.AddWithValue("@mdc", JsonConvert.SerializeObject(OtherSystem.LSCsystems.MDCEvents.getSaveData()));

                    //command.Parameters.AddWithValue("@pets", JsonConvert.SerializeObject(OtherSystem.LSCsystems.PetSystem.pets));

                    // serversettings

                    command.Parameters.AddWithValue("@ID", 1);

                    await command.ExecuteNonQueryAsync();

                    await connection.CloseAsync();
                }
                return;
            }
            catch (Exception ex)
            {
                Alt.Log("~r~[Save Server Settings Error]~w~ " + ex.Message);
                return;
            }
        }
        public static async Task LoadServerSettings()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM serversettings WHERE ID = @ID;";
                command.Parameters.AddWithValue("@ID", 1);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();

                        //OtherSystem.LSCsystems.farming.serverFarms = JsonConvert.DeserializeObject<List<OtherSystem.LSCsystems.farming.FarmModel>>(reader.GetString("farmSettings"));
                        //OtherSystem.LSCsystems.farming.LoadServerFarms(reader.GetString("farmSettings"));
                        OtherSystem.LSCsystems.Market.serverMarketList = JsonConvert.DeserializeObject<List<OtherSystem.LSCsystems.Market.MarketModel>>(reader.GetString("serverMarkets"));
                        OtherSystem.LSCsystems.Market.LoadMarketSystem();
                        //OtherSystem.LSCsystems.BlipSystem.dynamicBlips = JsonConvert.DeserializeObject<List<Globals.GlobalEvents.blipModel>>(reader.GetString("dynamicBlips"));
                        OtherSystem.LSCsystems.BlipSystem.LoadAllBlips(reader.GetString("blips"));

                        string pObjects = reader.GetString("playerObjects");
                        OtherSystem.LSCsystems.objectSystem.playerObjects = JsonConvert.DeserializeObject<List<OtherSystem.LSCsystems.objectSystem.playerObject>>(pObjects);
                        OtherSystem.LSCsystems.objectSystem.loadObjects(pObjects);

                        // Coprses
                        OtherSystem.LSCsystems.cksystem.LoadAllCorpses(reader.GetString("corpses"));

                        //!doors
                        OtherSystem.LSCsystems.DoorSystem.LoadServerDoors(reader.GetString("serverDoors"));

                        // piyango
                        //OtherSystem.LSCsystems.piyango.LoadPiyango(reader.GetString("piyango"));

                        // Drugs
                        OtherSystem.LSCsystems.Drug.LoadServerDrugs(reader.GetString("drugFarm"));

                        // Vendors
                        OtherSystem.LSCsystems.WeedVendors.LoadVendors(reader.GetString("vendors"));

                        // Objects

                        OtherSystem.LSCsystems.GroundObjects.LoadServerObjects(reader.GetString("serverObjects"));

                        OtherSystem.LSCsystems.AutoRepairSystem.LoadRepairs(reader.GetString("repairStations"));

                        //OtherSystem.LSCsystems.OOCmarket.LoadNPCs(reader.GetString("oocmarketnpc"));

                        OtherSystem.LSCsystems.Cemetery.LoadAllCemeterys(reader.GetString("cementery"));

                        OtherSystem.LSCsystems.JackingNPC.LoadAllNpc(reader.GetString("jacknpc"));

                        OtherSystem.LSCsystems.DynamicTedavi.Init(reader.GetString("tedavi"));

                        OtherSystem.TurfSystem.LoadAllTurfs(JsonConvert.DeserializeObject<List<OtherSystem.TurfSystem.TurfModel>>(reader.GetString("turfs")));

                        Plants.ServerEvent.InitPlants();
                        //OtherSystem.LSCsystems.MDCEvents.LoadMDCData(reader.GetString("mdc"));
                        // Pet system
                        //OtherSystem.LSCsystems.PetSystem.LoadAllPets(reader.GetString("pets"));
                    }
                }
                await connection.CloseAsync();
            }
        }

        public static async Task setOfflineAll()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE accounts SET isOnline = @isOnline; ";
                command.Parameters.AddWithValue("@isOnline", false);
                await command.ExecuteNonQueryAsync();

                MySqlCommand command2 = connection.CreateCommand();
                command2.CommandText = "UPDATE characters SET isOnline = @isOnline";
                command2.Parameters.AddWithValue("@isOnline", false);

                await command2.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }
            return;
        }

        public static async Task<int> GetServerRecord()
        {
            int record = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT record FROM serversettings WHERE ID = @ID;";
                command.Parameters.AddWithValue("@ID", 1);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();

                        record = reader.GetInt32("record");
                    }
                }
                await connection.CloseAsync();
            }

            return record;
        }

        public static async Task UpdateRecord(int Record)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE serversettings SET record = @record WHERE ID = @ID; ";

                command.Parameters.AddWithValue("@record", Record);

                command.Parameters.AddWithValue("@ID", 1);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }
        #endregion

        #region Character System
        public static async Task<AccountModel> getAccountInfo(string kookId) //Account Bilgilerini getirir.
        {
            AccountModel account = new AccountModel();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM accounts WHERE kookId = @kookId LIMIT 1";
                command.Parameters.AddWithValue("@kookId", kookId);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        account.ID = reader.GetInt32("ID");
                        account.kookId = reader.GetString("kookId");

                        account.accountType = reader.GetInt32("accountType");
                        account.accountEmail = reader.GetString("accountEmail");
                        account.forumName = reader.GetString("forumName");
                        account.adminLevel = reader.GetInt32("adminLevel");
                        account.banned = reader.GetBoolean("banned");
                        account.characterLimit = reader.GetInt32("characterlimit");
                        account.socialClubID = reader.GetUInt64("socialClub");
                        account.isOnline = reader.GetBoolean("isOnline");
                        account.lscPoint = reader.GetInt32("lscpoint");
                        account.ReportCount = reader.GetInt32("reportcount");
                        account.QuestionCount = reader.GetInt32("questioncount");
                        account.AdversimentCount = reader.GetInt32("adversimentcount");
                        account.OtherData = JsonConvert.DeserializeObject<OtherData>(reader.GetString("otherdata"));
                    }
                }
                await connection.CloseAsync();
            }
            return account;
        }

        public static async Task<AccountModel> getAccountInfo(ulong socialClub) //Account Bilgilerini getirir.
        {
            AccountModel account = new AccountModel();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM accounts WHERE socialClub = @socialClub LIMIT 1";
                command.Parameters.AddWithValue("@socialClub", socialClub);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        account.ID = reader.GetInt32("ID");
                        account.kookId = reader.GetString("kookId");

                        account.accountType = reader.GetInt32("accountType");
                        account.accountEmail = reader.GetString("accountEmail");
                        account.forumName = reader.GetString("forumName");
                        account.adminLevel = reader.GetInt32("adminLevel");
                        account.banned = reader.GetBoolean("banned");
                        account.characterLimit = reader.GetInt32("characterlimit");
                        account.socialClubID = reader.GetUInt64("socialClub");
                        account.isOnline = reader.GetBoolean("isOnline");
                        account.lscPoint = reader.GetInt32("lscpoint");
                        account.ReportCount = reader.GetInt32("reportcount");
                        account.QuestionCount = reader.GetInt32("questioncount");
                        account.AdversimentCount = reader.GetInt32("adversimentcount");
                        account.OtherData = JsonConvert.DeserializeObject<OtherData>(reader.GetString("otherdata"));
                    }
                }
                await connection.CloseAsync();
            }
            return account;
        }

        public static async Task<ForumAccountModel> getForumAccount(string username)
        {
            ForumAccountModel account = new ForumAccountModel();
            using (MySqlConnection connection = new MySqlConnection(forumConnectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT id_member,member_name,passwd,avatar FROM lsc_members WHERE member_name = @username LIMIT 1";
                command.Parameters.AddWithValue("@username", username);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        account.ID = reader.GetInt32("id_member");
                        account.member_name = reader.GetString("member_name");
                        account.passwd = reader.GetString("passwd");
                        account.avatar = reader.GetString("avatar");
                    }
                }
                await connection.CloseAsync();
            }
            return account;
        }
        public static async Task<List<AccountCharacterModel>> getAccountCharacters(int accountId) // Acount içindeki karakterleri getirir.
        {
            List<AccountCharacterModel> accCharacters = new List<AccountCharacterModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT ID, name, factionId, isCk FROM characters WHERE accountId = @accountId";
                command.Parameters.AddWithValue("@accountId", accountId);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            accCharacters.Add(new AccountCharacterModel
                            {
                                ID = reader.GetInt32("ID"),
                                characterName = reader.GetString("name"),
                                factionId = reader.GetInt32("factionId"),
                                isCK = reader.GetBoolean("isCk")
                            });
                        }
                    }
                }
                await connection.CloseAsync();
            }
            return accCharacters;
        }

        public static async Task setOnlineStatus(PlayerModel p, bool status)
        {
            AccountModel acc = await getAccInfo(p.accountId);
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE accounts SET isOnline = @isOnline WHERE ID = @ID; ";
                command.Parameters.AddWithValue("@isOnline", status);
                command.Parameters.AddWithValue("@ID", acc.ID);
                await command.ExecuteNonQueryAsync();

                MySqlCommand command2 = connection.CreateCommand();
                command2.CommandText = "UPDATE characters SET isOnline = @isOnline WHERE ID = @ID; ";
                command2.Parameters.AddWithValue("@isOnline", status);
                command2.Parameters.AddWithValue("@ID", p.sqlID);
                await command2.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }
        }

        [Obsolete]
        public static async Task getCharacterFromMysql(PlayerModel p, int Id) // Acount içindeki karakterleri getirir.
        {
            Globals.GlobalEvents.FreezeEntity(p, false);
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM characters WHERE ID = @ID";
                command.Parameters.AddWithValue("@ID", Id);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            //p.Spawn(new Position(reader.GetFloat("posX"), reader.GetFloat("posY"), reader.GetFloat("posZ")), 0);
                            p.sqlID = reader.GetInt32("ID");
                            p.accountId = reader.GetInt32("accountId");
                            p.characterName = reader.GetString("name");
                            p.characterAge = reader.GetInt32("age");
                            p.characterExp = reader.GetInt32("exp");
                            p.characterLevel = reader.GetInt32("level");
                            p.gameTime = reader.GetInt32("gameTime");
                            p.factionId = reader.GetInt32("factionId");
                            p.factionRank = reader.GetInt32("factionRank");
                            p.businessStaff = reader.GetInt32("businessStaff");
                            p.cash = reader.GetInt32("cash");
                            p.bankCash = reader.GetInt32("bankCash");
                            p.Strength = reader.GetInt32("strenght");
                            p.isCk = reader.GetBoolean("isCk");
                            p.injured = JsonConvert.DeserializeObject<InjuredModel>(reader.GetString("isInjured"));
                            p.jailTime = reader.GetInt32("jailTime");
                            p.phoneNumber = reader.GetInt32("phoneNumber");
                            p.SetPositionLocked(new Position(reader.GetFloat("posX"), reader.GetFloat("posY"), reader.GetFloat("posZ")));
                            await p.SetDimensionAsync(reader.GetInt32("dimension"));
                            p.firstLogin = reader.GetBoolean("firstLogin");
                            p.charComps = reader.GetString("charComps");
                            p.settings = reader.GetString("settings");
                            p.stats = reader.GetString("stats");
                            p.adminJail = reader.GetInt32("adminJail");
                            p.maxHp = 1000;
                            p.hp = reader.GetInt32("hp");
                            p.isFinishTut = reader.GetInt32("isFinishTut");
                            p.tutCar = reader.GetInt32("tutCar");
                            p.isGraffiti = reader.GetBoolean("isGraffiti");
                            
                        }
                    }
                }
                await connection.CloseAsync();
            }
            return;
        }
        public static async Task<PlayerModelInfo> getCharacterInfo(int Id)
        {
            PlayerModelInfo p = new PlayerModelInfo();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM characters WHERE ID = @ID";
                command.Parameters.AddWithValue("@ID", Id);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            p.sqlID = reader.GetInt32("ID");
                            p.accountId = reader.GetInt32("accountId");
                            p.characterName = reader.GetString("name");
                            p.characterAge = reader.GetInt32("age");
                            p.characterExp = reader.GetInt32("exp");
                            p.characterLevel = reader.GetInt32("level");
                            p.gameTime = reader.GetInt32("gameTime");
                            p.factionId = reader.GetInt32("factionId");
                            p.factionRank = reader.GetInt32("factionRank");
                            p.businessStaff = reader.GetInt32("businessStaff");
                            p.cash = reader.GetInt32("cash");
                            p.bankCash = reader.GetInt32("bankCash");
                            p.Strength = reader.GetInt32("strenght");
                            p.isCk = reader.GetBoolean("isCk");
                            p.jailTime = reader.GetInt32("jailTime");
                            p.phoneNumber = reader.GetInt32("phoneNumber");
                            p.Position = new Position(reader.GetFloat("posX"), reader.GetFloat("posY"), reader.GetFloat("posZ"));
                            p.Dimension = reader.GetInt32("dimension");
                            p.firstLogin = reader.GetBoolean("firstLogin");
                            p.charComps = reader.GetString("charComps");
                            p.settings = reader.GetString("settings");
                            p.stats = reader.GetString("stats");
                            p.adminJail = reader.GetInt32("adminJail");
                            p.isFinishTut = reader.GetInt32("isFinishTut");
                            p.tutCar = reader.GetInt32("tutCar");    
                            p.spawnLocation = reader.GetInt32("spawnLocation");
                            p.isGraffiti = reader.GetBoolean("isGraffiti");
                        }
                    }
                }
                await connection.CloseAsync();
            }
            return p;
        }

        public static async Task<PlayerModelInfo> getCharacterInfo(string name)
        {
            PlayerModelInfo p = new PlayerModelInfo();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM characters WHERE name = @ID";
                command.Parameters.AddWithValue("@ID", name);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            p.sqlID = reader.GetInt32("ID");
                            p.accountId = reader.GetInt32("accountId");
                            p.characterName = reader.GetString("name");
                            p.characterAge = reader.GetInt32("age");
                            p.characterExp = reader.GetInt32("exp");
                            p.characterLevel = reader.GetInt32("level");
                            p.gameTime = reader.GetInt32("gameTime");
                            p.factionId = reader.GetInt32("factionId");
                            p.factionRank = reader.GetInt32("factionRank");
                            p.businessStaff = reader.GetInt32("businessStaff");
                            p.cash = reader.GetInt32("cash");
                            p.bankCash = reader.GetInt32("bankCash");
                            p.Strength = reader.GetInt32("strenght");
                            p.isCk = reader.GetBoolean("isCk");
                            p.jailTime = reader.GetInt32("jailTime");
                            p.phoneNumber = reader.GetInt32("phoneNumber");
                            p.Position = new Position(reader.GetFloat("posX"), reader.GetFloat("posY"), reader.GetFloat("posZ"));
                            p.Dimension = reader.GetInt32("dimension");
                            p.firstLogin = reader.GetBoolean("firstLogin");
                            p.charComps = reader.GetString("charComps");
                            p.settings = reader.GetString("settings");
                            p.stats = reader.GetString("stats");
                            p.adminJail = reader.GetInt32("adminJail");
                            p.isFinishTut = reader.GetInt32("isFinishTut");
                            p.tutCar = reader.GetInt32("tutCar");     
                            p.spawnLocation = reader.GetInt32("spawnLocation");
                            p.isGraffiti = reader.GetBoolean("isGraffiti");
                        }
                    }
                }
                await connection.CloseAsync();
            }
            return p;
        }
        public static async Task<PlayerModelInfo> getCharacterinfoWithName(string name)
        {
            PlayerModelInfo p = new PlayerModelInfo();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM characters WHERE name LIKE @name LIMIT 1";
                command.Parameters.AddWithValue("@name", "%" + name + "%");


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            p.sqlID = reader.GetInt32("ID");
                            p.accountId = reader.GetInt32("accountId");
                            p.characterName = reader.GetString("name");
                            p.characterAge = reader.GetInt32("age");
                            p.characterExp = reader.GetInt32("exp");
                            p.characterLevel = reader.GetInt32("level");
                            p.gameTime = reader.GetInt32("gameTime");
                            p.factionId = reader.GetInt32("factionId");
                            p.factionRank = reader.GetInt32("factionRank");
                            p.businessStaff = reader.GetInt32("businessStaff");
                            p.cash = reader.GetInt32("cash");
                            p.bankCash = reader.GetInt32("bankCash");
                            p.Strength = reader.GetInt32("strenght");
                            p.isCk = reader.GetBoolean("isCk");
                            p.jailTime = reader.GetInt32("jailTime");
                            p.phoneNumber = reader.GetInt32("phoneNumber");
                            p.Position = new Position(reader.GetFloat("posX"), reader.GetFloat("posY"), reader.GetFloat("posZ"));
                            p.Dimension = reader.GetInt32("dimension");
                            p.firstLogin = reader.GetBoolean("firstLogin");
                            p.charComps = reader.GetString("charComps");
                            p.settings = reader.GetString("settings");
                            p.stats = reader.GetString("stats");
                            p.adminJail = reader.GetInt32("adminJail");
                            p.isFinishTut = reader.GetInt32("isFinishTut");
                            p.tutCar = reader.GetInt32("tutCar");   
                            p.spawnLocation = reader.GetInt32("spawnLocation");
                            p.isGraffiti = reader.GetBoolean("isGraffiti");
                        }
                    }
                }
                await connection.CloseAsync();
            }
            return p;
        }
        public static async Task UpdateCharacterInfo(PlayerModel character) // 
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE characters SET accountId = @accountId, name = @name, age = @age,exp = @exp, level = @level, gameTime = @gameTime, factionId = @factionId," +
                    "factionRank = @factionRank, businessStaff = @businessStaff, cash = @cash, bankCash = @bankCash, strenght = @strenght, isCk = @isCk, jailTime = @jailTime, phoneNumber = @phoneNumber," +
                    "posX = @posX, posY = @posY, posZ = @posZ, dimension = @dimension, firstLogin = @firstLogin, charComps = @charComps, settings = @settings, stats = @stats," +
                    "isInjured = @isInjured, adminJail = @adminJail, hp = @hp, isFinishTut = @isFinishTut, tutCar = @tutCar, spawnLocation = @spawnLocation, isGraffiti = @isGraffiti WHERE ID = @ID; ";

                command.Parameters.AddWithValue("@accountId", character.accountId);
                command.Parameters.AddWithValue("@name", character.characterName);
                command.Parameters.AddWithValue("@age", character.characterAge);
                command.Parameters.AddWithValue("@exp", character.characterExp);
                command.Parameters.AddWithValue("@level", character.characterLevel);
                command.Parameters.AddWithValue("@gameTime", character.gameTime);
                command.Parameters.AddWithValue("@factionId", character.factionId);
                command.Parameters.AddWithValue("@factionRank", character.factionRank);
                command.Parameters.AddWithValue("@businessStaff", character.businessStaff);
                command.Parameters.AddWithValue("@cash", character.cash);
                command.Parameters.AddWithValue("@bankCash", character.bankCash);
                command.Parameters.AddWithValue("@strenght", character.Strength);
                command.Parameters.AddWithValue("@isCk", character.isCk);
                command.Parameters.AddWithValue("@jailTime", character.jailTime);
                command.Parameters.AddWithValue("@phoneNumber", character.phoneNumber);
                command.Parameters.AddWithValue("@posX", character.Position.X);
                command.Parameters.AddWithValue("@posY", character.Position.Y);
                command.Parameters.AddWithValue("@posZ", character.Position.Z);
                command.Parameters.AddWithValue("@dimension", character.Dimension);
                command.Parameters.AddWithValue("@firstLogin", character.firstLogin);
                command.Parameters.AddWithValue("@charComps", character.charComps);
                command.Parameters.AddWithValue("@settings", character.settings);
                command.Parameters.AddWithValue("@stats", character.stats);
                command.Parameters.AddWithValue("@isInjured", JsonConvert.SerializeObject(character.injured));
                command.Parameters.AddWithValue("@adminJail", character.adminJail);
                command.Parameters.AddWithValue("@hp", character.hp);
                command.Parameters.AddWithValue("@isFinishTut", character.isFinishTut);
                command.Parameters.AddWithValue("@tutCar", character.tutCar); 
                command.Parameters.AddWithValue("@spawnLocation", character.spawnLocation);
                command.Parameters.AddWithValue("@isGraffiti", character.isGraffiti);

                command.Parameters.AddWithValue("@ID", character.sqlID);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }

        public static async Task updatePlayerPosition(int Id, Position pos, int dim)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE characters SET posX = @posX, posY = @posY, posZ = @posZ, dimension = @dimension WHERE ID = @ID; ";

                command.Parameters.AddWithValue("@posX", pos.X);
                command.Parameters.AddWithValue("@posY", pos.Y);
                command.Parameters.AddWithValue("@posZ", pos.Z);
                command.Parameters.AddWithValue("@dimension", dim);

                command.Parameters.AddWithValue("@ID", Id);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }

        public static async Task UpdateOfflineCharacterInfo(PlayerModelInfo character)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE characters SET accountId = @accountId, name = @name, age = @age, level = @level, gameTime = @gameTime, factionId = @factionId," +
                    "factionRank = @factionRank, businessStaff = @businessStaff, cash = @cash, bankCash = @bankCash, strenght = @strenght, isCk = @isCk, jailTime = @jailTime, phoneNumber = @phoneNumber," +
                    "posX = @posX, posY = @posY, posZ = @posZ, dimension = @dimension, firstLogin = @firstLogin, charComps = @charComps, settings = @settings, stats = @stats," +
                    " adminJail = @adminJail, isFinishTut = @isFinishTut, tutCar = @tutCar, spawnLocation = @spawnLocation, isGraffiti = @isGraffiti WHERE ID = @ID; ";

                command.Parameters.AddWithValue("@accountId", character.accountId);
                command.Parameters.AddWithValue("@name", character.characterName);
                command.Parameters.AddWithValue("@age", character.characterAge);
                command.Parameters.AddWithValue("@exp", character.characterExp);
                command.Parameters.AddWithValue("@level", character.characterLevel);
                command.Parameters.AddWithValue("@gameTime", character.gameTime);
                command.Parameters.AddWithValue("@factionId", character.factionId);
                command.Parameters.AddWithValue("@factionRank", character.factionRank);
                command.Parameters.AddWithValue("@businessStaff", character.businessStaff);
                command.Parameters.AddWithValue("@cash", character.cash);
                command.Parameters.AddWithValue("@bankCash", character.bankCash);
                command.Parameters.AddWithValue("@strenght", character.Strength);
                command.Parameters.AddWithValue("@isCk", character.isCk);
                command.Parameters.AddWithValue("@jailTime", character.jailTime);
                command.Parameters.AddWithValue("@phoneNumber", character.phoneNumber);
                command.Parameters.AddWithValue("@posX", character.Position.X);
                command.Parameters.AddWithValue("@posY", character.Position.Y);
                command.Parameters.AddWithValue("@posZ", character.Position.Z);
                command.Parameters.AddWithValue("@dimension", character.Dimension);
                command.Parameters.AddWithValue("@firstLogin", character.firstLogin);
                command.Parameters.AddWithValue("@charComps", character.charComps);
                command.Parameters.AddWithValue("@settings", character.settings);
                command.Parameters.AddWithValue("@stats", character.stats);
                command.Parameters.AddWithValue("@adminJail", character.adminJail);
                command.Parameters.AddWithValue("@isFinishTut", character.isFinishTut);
                command.Parameters.AddWithValue("@tutCar", character.tutCar);
                command.Parameters.AddWithValue("@spawnLocation", character.spawnLocation);
                command.Parameters.AddWithValue("@isGraffiti", character.isGraffiti);
                
                command.Parameters.AddWithValue("@ID", character.sqlID);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }
        public static async Task<bool> CheckCharacterName(string name)
        {
            bool result = true;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM characters WHERE name = @ID";
                command.Parameters.AddWithValue("@ID", name);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                await connection.CloseAsync();
            }
            return result;
        }
        public static async Task<int> CreateCharacter(PlayerModel p, int AccountID)
        {
            int resultId = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO characters (name, accountId) VALUES (@name, @accountId) ";

                command.Parameters.AddWithValue("@name", p.characterName);
                command.Parameters.AddWithValue("@accountId", AccountID);


                await command.ExecuteNonQueryAsync();
                resultId = (int)command.LastInsertedId;
                await connection.CloseAsync();
            }
            return resultId;
        }

        public static async Task<int> CreateAccount(PlayerModel p, string kookId)
        {
            int resultId = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO accounts (kookId, socialClub) VALUES (@kookId, @socialClub) ";

                command.Parameters.AddWithValue("@kookId", kookId);
                command.Parameters.AddWithValue("@socialClub", p.SocialClubId);


                await command.ExecuteNonQueryAsync();
                resultId = (int)command.LastInsertedId;
                await connection.CloseAsync();
            }
            return resultId;
        }
        #endregion

        #region AccountLog
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Type">1: Jail, 2: Kick, 3: CharacterBan</param>
        /// <param name="pSQL"></param>
        /// <param name="AccountID"></param>
        /// <param name="Reason"></param>
        /// <param name="Admin"></param>
        /// <returns></returns>
        public static async Task<int> AddAccountLog(int Type, int pSQL, int AccountID, string Reason, string Admin)
        {
            int resultId = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO account_logs (Type, player, account, Reason, Admin, Date) VALUES (@Type, @player, @account, @Reason, @Admin, @Date);";

                command.Parameters.AddWithValue("@Type", Type);
                command.Parameters.AddWithValue("@Reason", Reason);
                command.Parameters.AddWithValue("@player", pSQL);
                command.Parameters.AddWithValue("@account", AccountID);
                command.Parameters.AddWithValue("@Admin", Admin);
                command.Parameters.AddWithValue("@Date", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

                await command.ExecuteNonQueryAsync();
                resultId = (int)command.LastInsertedId;
                await connection.CloseAsync();
            }
            return resultId;
        }

        public static async Task<int> AddPlayTimeLog(int pSQL, int accSQL, string JoinDate, string LeftDate, int TotalMinute, string Reason)
        {
            int resultId = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO account_playtime (characterSQL, account, JoinDate, LeftDate, TotalMinute, Reason) VALUES (@characterSQL, @account," +
                    "@JoinDate, @LeftDate, @TotalMinute, @Reason);";

                command.Parameters.AddWithValue("@characterSQL", pSQL);
                command.Parameters.AddWithValue("@account", accSQL);
                command.Parameters.AddWithValue("@JoinDate", JoinDate);
                command.Parameters.AddWithValue("@LeftDate", LeftDate);
                command.Parameters.AddWithValue("@TotalMinute", TotalMinute);
                command.Parameters.AddWithValue("@Reason", "Kendi isteği ile.");

                await command.ExecuteNonQueryAsync();
                resultId = (int)command.LastInsertedId;
                await connection.CloseAsync();
            }
            return resultId;
        }
        #endregion
        #region Vehicles
        [Obsolete]
        public static void getAllServerVehicles()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM vehicles";


                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                string vehicleSettings = reader.GetString("settings");
                                bool isJobCar = false;
                                Int32 vehFine = reader.GetInt32("fine");
                                Int32 vehMaxTax = reader.GetInt32("defaultTax");

                                if (reader.GetInt32("jobId") > 0)
                                    isJobCar = true;
                                Position pos = new Position(reader.GetFloat("posX"), reader.GetFloat("posY"), reader.GetFloat("posZ"));
                                Rotation rot = new Rotation(reader.GetFloat("rotX"), reader.GetFloat("rotY"), reader.GetFloat("rotZ"));
                                Int64 model = Int64.Parse(reader.GetString("model"));

                                if (isJobCar)
                                {
                                    if (vehicleSettings.Length >= 2)
                                    {
                                        VehSet settt = JsonConvert.DeserializeObject<VehSet>(vehicleSettings);
                                        pos = settt.savePosition;
                                        rot = settt.saveRotation;
                                    }
                                }
                                IVehicle v = Alt.CreateVehicle((uint)model, pos, rot);
                                VehModel veh = (VehModel)v;
                                veh.sqlID = reader.GetInt32("ID");
                                veh.owner = reader.GetInt32("owner");
                                string[] fColor = reader.GetString("firstColor").Split(",");
                                string[] sColor = reader.GetString("secondColor").Split(",");
                                veh.PrimaryColorRgb = new Rgba(byte.Parse(fColor[0]), byte.Parse(fColor[1]), byte.Parse(fColor[2]), byte.Parse(fColor[3]));
                                veh.SecondaryColorRgb = new Rgba(byte.Parse(sColor[0]), byte.Parse(sColor[1]), byte.Parse(sColor[2]), byte.Parse(sColor[3]));
                                veh.factionId = reader.GetInt32("factionId");
                                veh.jobId = reader.GetInt32("jobId");
                                veh.businessId = reader.GetInt32("businessId");
                                veh.km = reader.GetDouble("km");
                                veh.inventoryCapacity = reader.GetInt32("inventoryCapacity");
                                veh.maxFuel = reader.GetInt32("maxFuel");
                                veh.currentFuel = reader.GetInt32("currentFuel");
                                veh.fuelConsumption = reader.GetInt32("fuelConsumption");
                                veh.LockState = (VehicleLockState)reader.GetInt32("isLocked");
                                veh.EngineOn = reader.GetBoolean("isEngine");
                                veh.price = reader.GetInt32("price");
                                veh.defaultTax = vehMaxTax;
                                veh.towwed = reader.GetBoolean("towwed");
                                veh.fine = vehFine;
                                veh.PetrolTankHealth = reader.GetInt32("petrolTankHealth");
                                //veh.DamageData = reader.GetString("damageData");
                                veh.BodyHealth = (uint)reader.GetInt32("bodyHealth");
                                veh.EngineHealth = reader.GetInt32("engineHealth");
                                veh.TuningData = reader.GetString("tuningdata");
                                //string damage = reader.GetString("damageData");
                                //if (damage.Length <= 1) { veh.DamageData = "AQ=="; } else { veh.DamageData = damage; }
                                if (veh.TuningData != string.Empty) { Props.Business.ServerVehicleTuning(veh, veh.TuningData); }
                                veh.vehInv = reader.GetString("bagaj");
                                veh.ManualEngineControl = true;
                                veh.SetSyncedMetaData("VehicleFactionID", veh.factionId);
                                if (reader.GetString("savePos").Length > 5)
                                    veh.savePos = JsonConvert.DeserializeObject<Position>(reader.GetString("savePos"));

                                if (vehicleSettings.Length >= 2)
                                {
                                    veh.settings = JsonConvert.DeserializeObject<VehSet>(vehicleSettings);
                                    //if (!veh.settings.ModifiyData.Contains("2628_"))
                                    //    veh.AppearanceData = "2628_" + veh.settings.ModifiyData;
                                    //else if (veh.settings.ModifiyData.Contains("2545_"))
                                    //    veh.AppearanceData = veh.settings.ModifiyData.Replace("2545_", "2628_");
                                    //else
                                    //    veh.AppearanceData = veh.settings.ModifiyData;

                                    if (veh.settings.ModifiyData.Contains("2628_"))
                                        veh.AppearanceData = (veh.settings.ModifiyData.Replace("2628_", "2802_"));
                                    else if (veh.settings.ModifiyData.Contains("2802_"))
                                        v.AppearanceData = (veh.settings.ModifiyData);
                                    else v.AppearanceData = ("2802_" + veh.settings.ModifiyData);

                                    if (!veh.settings.WantModifyData.Contains("2628_"))
                                        veh.settings.WantModifyData = "2802_" + veh.settings.ModifiyData;
                                    else if (veh.settings.WantModifyData.Contains("2628_"))
                                        veh.settings.WantModifyData = veh.settings.ModifiyData.Replace("2628_", "2802_");

                                }
                                else
                                {
                                    veh.settings = new VehSet();
                                    //Vehicle.VehicleMain.VehicleMakeSettings(veh);
                                    veh.SetAppearanceDataAsync(veh.settings.ModifiyData);
                                }

                                if (veh.jobId > 0)
                                {
                                    veh.Position = veh.settings.savePosition;
                                    //veh.DamageData = "AQ==";
                                }
                                veh.engineBoost = reader.GetFloat("engineBoost");

                                veh.NumberplateText = reader.GetString("plate");

                                veh.SetHeadlightColorAsync(veh.settings.HeadlightColor);

                                if (veh.towwed)
                                {
                                    veh.Dimension = veh.sqlID;
                                }
                                else
                                {
                                    veh.Dimension = reader.GetInt32("dimension");
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
                connection.Close();
            }
            return;

        }
        public static async Task UpdateVehicleInfo(VehModel vehicle)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE vehicles SET model = @model, owner = @owner, firstColor = @firstColor, secondColor = @secondColor," +
                    " posX = @posX, posY = @posY, posZ = @posZ, rotX = @rotX, rotY = rotY, rotZ = @rotZ, factionId = @factionId, jobId = @jobId, businessId = @businessId, km = @km, inventoryCapacity = @inventoryCapacity," +
                    "maxFuel = @maxFuel, currentFuel = @currentFuel, fuelConsumption = @fuelConsumption, plate = @plate, isLocked = @isLocked, isEngine = @isEngine," +
                    "price = @price, defaultTax = @defaultTax, towwed = @towwed, fine = @fine, petrolTankHealth = @petrolTankHealth, damageData = @damageData," +
                    "bodyHealth = @bodyHealth, engineHealth = @engineHealth, tuningdata = @tuningdata, bagaj = @bagaj, savePos = @savePos, engineBoost = @engineBoost, settings = @settings," +
                    "dimension = @dimension WHERE ID = @ID; ";

                command.Parameters.AddWithValue("@model", vehicle.Model.ToString());
                command.Parameters.AddWithValue("@owner", vehicle.owner);
                string fColor = vehicle.PrimaryColorRgb.R.ToString() + "," + vehicle.PrimaryColorRgb.G.ToString() + "," + vehicle.PrimaryColorRgb.B + "," + vehicle.PrimaryColorRgb.A;
                string sColor = vehicle.SecondaryColorRgb.R.ToString() + "," + vehicle.SecondaryColorRgb.G.ToString() + "," + vehicle.SecondaryColorRgb.B + "," + vehicle.SecondaryColorRgb.A;
                command.Parameters.AddWithValue("@firstColor", fColor);
                command.Parameters.AddWithValue("@secondColor", sColor);
                command.Parameters.AddWithValue("@posX", vehicle.Position.X);
                command.Parameters.AddWithValue("@posY", vehicle.Position.Y);
                command.Parameters.AddWithValue("@posZ", vehicle.Position.Z);
                command.Parameters.AddWithValue("@rotX", vehicle.Rotation.Roll);
                command.Parameters.AddWithValue("@rotY", vehicle.Rotation.Pitch);
                command.Parameters.AddWithValue("@rotZ", vehicle.Rotation.Yaw);
                command.Parameters.AddWithValue("@factionId", vehicle.factionId);
                command.Parameters.AddWithValue("@jobId", vehicle.jobId);
                command.Parameters.AddWithValue("@businessId", vehicle.businessId);
                command.Parameters.AddWithValue("@km", vehicle.km);
                command.Parameters.AddWithValue("@inventoryCapacity", vehicle.inventoryCapacity);
                command.Parameters.AddWithValue("@maxFuel", vehicle.maxFuel);
                command.Parameters.AddWithValue("@currentFuel", vehicle.currentFuel);
                command.Parameters.AddWithValue("@fuelConsumption", vehicle.fuelConsumption);
                command.Parameters.AddWithValue("@plate", vehicle.NumberplateText);
                command.Parameters.AddWithValue("@isLocked", vehicle.LockState);
                command.Parameters.AddWithValue("@isEngine", vehicle.EngineOn);
                command.Parameters.AddWithValue("@price", vehicle.price);
                command.Parameters.AddWithValue("@defaultTax", vehicle.defaultTax);
                command.Parameters.AddWithValue("@towwed", vehicle.towwed);
                command.Parameters.AddWithValue("@fine", vehicle.fine);
                command.Parameters.AddWithValue("@petrolTankHealth", vehicle.PetrolTankHealth);
                command.Parameters.AddWithValue("@damageData", vehicle.DamageData);
                command.Parameters.AddWithValue("@bodyHealth", vehicle.BodyHealth);
                command.Parameters.AddWithValue("@engineHealth", vehicle.EngineHealth);
                command.Parameters.AddWithValue("@tuningdata", vehicle.TuningData);
                command.Parameters.AddWithValue("@bagaj", vehicle.vehInv);
                string savePos = JsonConvert.SerializeObject(vehicle.savePos);
                command.Parameters.AddWithValue("@savePos", savePos);
                command.Parameters.AddWithValue("@engineBoost", vehicle.engineBoost);
                command.Parameters.AddWithValue("@settings", JsonConvert.SerializeObject(vehicle.settings));
                command.Parameters.AddWithValue("@dimension", vehicle.Dimension);

                command.Parameters.AddWithValue("@ID", vehicle.sqlID);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }

        public static async Task<int> CreateVehicle(VehModel vehicle)
        {
            int resultId = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO vehicles (model) VALUES (@model) ";

                command.Parameters.AddWithValue("@model", vehicle.Model.ToString());


                await command.ExecuteNonQueryAsync();
                resultId = (int)command.LastInsertedId;
                await connection.CloseAsync();
            }
            return resultId;
        }
        public static async Task DeleteVehicle(VehModel veh)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM vehicles WHERE ID = @ID;";
                command.Parameters.AddWithValue("@ID", veh.sqlID);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }

        #endregion

        #region FactionSystem

        public static async Task<int> CreateFactionMysql(FactionModel fact)
        {
            int factionId = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO factions (name, type, settings, rank, owner) VALUES (@name, @type, @setting, @rank, @owner)";

                command.Parameters.AddWithValue("@name", fact.name);
                command.Parameters.AddWithValue("@type", fact.type);
                command.Parameters.AddWithValue("@setting", JsonConvert.SerializeObject(fact.settings));
                command.Parameters.AddWithValue("@rank", JsonConvert.SerializeObject(fact.rank));
                command.Parameters.AddWithValue("@owner", fact.owner);

                await command.ExecuteNonQueryAsync();
                factionId = (int)command.LastInsertedId;
                await connection.CloseAsync();
            }
            return factionId;
        }

        public static async Task<List<FactionUserModel>> GetFactionMembers(int factionId)
        {
            List<FactionUserModel> members = new List<FactionUserModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT ID, name, factionRank FROM characters WHERE factionId = @factionId;";
                command.Parameters.AddWithValue("@factionId", factionId);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            members.Add(new FactionUserModel
                            {
                                ID = reader.GetInt32("ID"),
                                name = reader.GetString("name"),
                                rank = reader.GetInt32("factionRank")
                            });
                        }
                    }
                }
                await connection.CloseAsync();
            } //
            return members;
        }

        public static async Task<FactionModel> GetFactionInfo(int factionId)
        {
            FactionModel fact = new FactionModel();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM factions WHERE ID = @ID;";
                command.Parameters.AddWithValue("@ID", factionId);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();

                        fact.ID = reader.GetInt32("ID");
                        fact.name = reader.GetString("name");
                        fact.type = reader.GetInt32("type");
                        fact.cash = reader.GetInt32("cash");
                        fact.factionExp = reader.GetInt32("factionExp");
                        fact.factionLevel = reader.GetInt32("factionLevel");
                        fact.isApproved = reader.GetBoolean("isApproved");
                        fact.settings = JsonConvert.DeserializeObject<FactionSetting>(reader.GetString("settings"));
                        fact.rank = JsonConvert.DeserializeObject<List<FactionRank>>(reader.GetString("rank"));
                        fact.owner = reader.GetInt32("owner");
                        fact.company = reader.GetInt32("company");
                        fact.side = reader.GetInt32("side");
                    }
                }
                await connection.CloseAsync();
            }
            return fact;
        }

        public static async Task<FactionModel> GetFactionFromType(int type)
        {
            FactionModel fact = new FactionModel();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM factions WHERE type = @ID LIMIT 1;";
                command.Parameters.AddWithValue("@ID", type);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();

                        fact.ID = reader.GetInt32("ID");
                        fact.name = reader.GetString("name");
                        fact.type = reader.GetInt32("type");
                        fact.cash = reader.GetInt32("cash");
                        fact.factionExp = reader.GetInt32("factionExp");
                        fact.factionLevel = reader.GetInt32("factionLevel");
                        fact.isApproved = reader.GetBoolean("isApproved");
                        fact.settings = JsonConvert.DeserializeObject<FactionSetting>(reader.GetString("settings"));
                        fact.rank = JsonConvert.DeserializeObject<List<FactionRank>>(reader.GetString("rank"));
                        fact.owner = reader.GetInt32("owner");
                        fact.company = reader.GetInt32("company");
                        fact.side = reader.GetInt32("side");
                    }
                }
                await connection.CloseAsync();
            }
            return fact;
        }
        public static async Task<List<int>> GetGovermentFactionIds()
        {
            List<int> ids = new List<int>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT ID FROM factions WHERE type= 3 or type=4 or type=8;";

                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            ids.Add(reader.GetInt32("ID"));
                        }


                    }
                }
                await connection.CloseAsync();
            }
            return ids;
        }

        public static async Task<List<int>> GetPDFactionIds()
        {
            List<int> ids = new List<int>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT ID FROM factions WHERE type = 3;";

                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            ids.Add(reader.GetInt32("ID"));
                        }


                    }
                }

                await connection.CloseAsync();
            }
            return ids;
        }

        public static async Task UpdateFactionInfo(FactionModel fact)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE factions SET name = @name, type = @type, cash = @cash, factionExp = @factionExp," +
                    "factionLevel = @factionLevel, isApproved = @isApproved, settings = @settings, rank = @rank," +
                    "owner = @owner, company = @company, side = @side WHERE ID = @ID; ";

                command.Parameters.AddWithValue("@name", fact.name);
                command.Parameters.AddWithValue("@type", fact.type);
                command.Parameters.AddWithValue("@cash", fact.cash);
                command.Parameters.AddWithValue("@factionExp", fact.factionExp);
                command.Parameters.AddWithValue("@factionLevel", fact.factionLevel);
                command.Parameters.AddWithValue("@isApproved", fact.isApproved);
                command.Parameters.AddWithValue("@settings", JsonConvert.SerializeObject(fact.settings));
                command.Parameters.AddWithValue("@rank", JsonConvert.SerializeObject(fact.rank));
                command.Parameters.AddWithValue("@owner", fact.owner);
                command.Parameters.AddWithValue("@company", fact.company);
                command.Parameters.AddWithValue("@side", fact.side);

                command.Parameters.AddWithValue("@ID", fact.ID);


                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }
        #endregion

        #region Business System

        public static async Task<BusinessModel> GetBusinessInfo(int businessId)
        {
            BusinessModel bus = new BusinessModel();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM business WHERE ID = @ID;";
                command.Parameters.AddWithValue("@ID", businessId);

                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();

                        bus.ID = reader.GetInt32("ID");
                        bus.name = reader.GetString("name");
                        bus.ownerId = reader.GetInt32("ownerId");
                        bus.price = reader.GetInt32("price");
                        bus.type = reader.GetInt32("type");
                        bus.dimension = reader.GetInt32("dimension");
                        bus.vault = reader.GetInt32("vault");
                        bus.entrancePrice = reader.GetInt32("entrancePrice");
                        bus.position = new Position(reader.GetFloat("posX"), reader.GetFloat("posY"), reader.GetFloat("posZ"));
                        bus.interiorPosition = new Position(reader.GetFloat("iPosX"), reader.GetFloat("iPosY"), reader.GetFloat("iPosZ"));
                        bus.isLocked = reader.GetBoolean("isLocked");
                        bus.stock = reader.GetInt32("stock");
                        bus.settings = JsonConvert.DeserializeObject<BusinessModel.BizSettings>(reader.GetString("settings"));
                        bus.company = reader.GetInt32("company");
                    }
                }
                await connection.CloseAsync();
            }
            return bus;
        }
        public static async Task<List<BusinessModel>> GetAllServerBusiness()
        {
            List<BusinessModel> business = new List<BusinessModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM business";


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {


                            business.Add(new BusinessModel
                            {
                                ID = reader.GetInt32("ID"),
                                name = reader.GetString("name"),
                                ownerId = reader.GetInt32("ownerId"),
                                price = reader.GetInt32("price"),
                                type = reader.GetInt32("type"),
                                vault = reader.GetInt32("vault"),
                                entrancePrice = reader.GetInt32("entrancePrice"),
                                position = new Position(reader.GetFloat("posX"), reader.GetFloat("posY"), reader.GetFloat("posZ")),
                                interiorPosition = new Position(reader.GetFloat("iPosX"), reader.GetFloat("iPosY"), reader.GetFloat("iPosZ")),
                                dimension = reader.GetInt32("dimension"),
                                isLocked = reader.GetBoolean("isLocked"),
                                stock = reader.GetInt32("stock"),
                                settings = JsonConvert.DeserializeObject<BusinessModel.BizSettings>(reader.GetString("settings"))
                            });
                        }
                    }
                }
                await connection.CloseAsync();
            }
            return business;

        }
        public static async Task<int> CreateBusiness(BusinessModel biz)
        {
            int resultId = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO business (name, ownerId, price, type, posX, posY, posZ, isLocked) VALUES (@name, @ownerId, @price, @type, @posX, @posY, @posZ, @isLocked);";

                command.Parameters.AddWithValue("@name", biz.name);
                command.Parameters.AddWithValue("@ownerId", biz.ownerId);
                command.Parameters.AddWithValue("@price", biz.price);
                command.Parameters.AddWithValue("@type", biz.type);
                command.Parameters.AddWithValue("@posX", biz.position.X);
                command.Parameters.AddWithValue("@posY", biz.position.Y);
                command.Parameters.AddWithValue("@posZ", biz.position.Z);
                command.Parameters.AddWithValue("@isLocked", biz.isLocked);

                await command.ExecuteNonQueryAsync();
                resultId = (int)command.LastInsertedId;
                await connection.CloseAsync();
            }
            return resultId;
        }
        public static async Task UpdateBusiness(BusinessModel biz)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE business SET name = UPPER(@name), ownerId = @ownerId, price = @price, type = @type, vault = @vault," +
                    "entrancePrice = @entrancePrice, posX = @posX, posY = @posY, posZ = @posZ, iPosX = @iPosX, iPosY = @iPosY," +
                    "iPosZ = @iPosZ, dimension = @dimension, isLocked = @isLocked, stock = @stock, settings = @settings WHERE ID = @ID; ";

                command.Parameters.AddWithValue("@name", biz.name);
                command.Parameters.AddWithValue("@ownerId", biz.ownerId);
                command.Parameters.AddWithValue("@price", biz.price);
                command.Parameters.AddWithValue("@type", biz.type);
                command.Parameters.AddWithValue("@vault", biz.vault);
                command.Parameters.AddWithValue("@entrancePrice", biz.entrancePrice);
                command.Parameters.AddWithValue("@posX", biz.position.X);
                command.Parameters.AddWithValue("@posY", biz.position.Y);
                command.Parameters.AddWithValue("@posZ", biz.position.Z);
                command.Parameters.AddWithValue("@iPosX", biz.interiorPosition.X);
                command.Parameters.AddWithValue("@iPosY", biz.interiorPosition.Y);
                command.Parameters.AddWithValue("@iPosZ", biz.interiorPosition.Z);
                command.Parameters.AddWithValue("@dimension", biz.dimension);
                command.Parameters.AddWithValue("@isLocked", biz.isLocked);
                command.Parameters.AddWithValue("@stock", biz.stock);
                command.Parameters.AddWithValue("@settings", JsonConvert.SerializeObject(biz.settings));

                command.Parameters.AddWithValue("@ID", biz.ID);


                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }
        public static async Task DeleteBusiness(int Id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM business WHERE ID = @ID;";
                command.Parameters.AddWithValue("@ID", Id);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }
        public static async Task<List<BusinessModel>> GetMemberBusinessList(PlayerModel player)
        {
            List<BusinessModel> business = new List<BusinessModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM business WHERE ownerId = @ownerId";
                command.Parameters.AddWithValue("@ownerId", player.sqlID);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            business.Add(new BusinessModel
                            {
                                ID = reader.GetInt32("ID"),
                                name = reader.GetString("name"),
                                ownerId = reader.GetInt32("ownerId"),
                                price = reader.GetInt32("price"),
                                type = reader.GetInt32("type"),
                                vault = reader.GetInt32("vault"),
                                entrancePrice = reader.GetInt32("entrancePrice"),
                                position = new Position(reader.GetFloat("posX"), reader.GetFloat("posY"), reader.GetFloat("posZ")),
                                interiorPosition = new Position(reader.GetFloat("iPosX"), reader.GetFloat("iPosY"), reader.GetFloat("iPosZ")),
                                dimension = reader.GetInt32("dimension"),
                                isLocked = reader.GetBoolean("isLocked"),
                                stock = reader.GetInt32("stock"),
                                settings = JsonConvert.DeserializeObject<BusinessModel.BizSettings>(reader.GetString("settings"))
                            });
                        }
                    }
                }
                await connection.CloseAsync();
            }
            return business;

        }
        public static async Task<List<BusinessModel>> GetMemberBusinessList(int pSql)
        {
            List<BusinessModel> business = new List<BusinessModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM business WHERE ownerId = @ownerId";
                command.Parameters.AddWithValue("@ownerId", pSql);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            business.Add(new BusinessModel
                            {
                                ID = reader.GetInt32("ID"),
                                name = reader.GetString("name"),
                                ownerId = reader.GetInt32("ownerId"),
                                price = reader.GetInt32("price"),
                                type = reader.GetInt32("type"),
                                vault = reader.GetInt32("vault"),
                                entrancePrice = reader.GetInt32("entrancePrice"),
                                position = new Position(reader.GetFloat("posX"), reader.GetFloat("posY"), reader.GetFloat("posZ")),
                                interiorPosition = new Position(reader.GetFloat("iPosX"), reader.GetFloat("iPosY"), reader.GetFloat("iPosZ")),
                                dimension = reader.GetInt32("dimension"),
                                isLocked = reader.GetBoolean("isLocked"),
                                stock = reader.GetInt32("stock"),
                                settings = JsonConvert.DeserializeObject<BusinessModel.BizSettings>(reader.GetString("settings"))
                            });
                        }
                    }
                }
                await connection.CloseAsync();
            }
            return business;

        }
        public static async Task<List<Props.Business.BusinesMemberList>> GetBusinessStafList(int ID)
        {
            List<Props.Business.BusinesMemberList> list = new List<Props.Business.BusinesMemberList>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM characters WHERE businessStaff = @ID";
                command.Parameters.AddWithValue("@ID", ID);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new Props.Business.BusinesMemberList()
                            {
                                ID = reader.GetInt32("ID"),
                                Name = reader.GetString("name")
                            });
                        }
                    }
                }
                await connection.CloseAsync();
            }
            return list;
        }
        public static async Task<List<int>> getAllBusinesIDS()
        {
            List<int> IDS = new List<int>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM business";


                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {


                            IDS.Add(reader.GetInt32("ID"));
                        }
                    }
                }
                await connection.CloseAsync();
            }
            return IDS;
        }
        public static async Task ResetBusinessStafs(int ID)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE characters WHERE businessStaff = 0 WHERE businessStaff = @ID";
                command.Parameters.AddWithValue("@ID", ID);


                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }
            return;
        }
        #endregion

        #region House System
        public static async Task<List<HouseModel>> GetAllServerHouses()
        {
            List<HouseModel> business = new List<HouseModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM houses";


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            business.Add(new HouseModel
                            {
                                ID = reader.GetInt32("ID"),
                                name = reader.GetString("name"),
                                price = reader.GetInt32("price"),
                                ownerId = reader.GetInt32("ownerId"),
                                pos = JsonConvert.DeserializeObject<Position>(reader.GetString("pos")),
                                intPos = JsonConvert.DeserializeObject<Position>(reader.GetString("intPos")),
                                isRentable = reader.GetBoolean("isRentable"),
                                rentPrice = reader.GetInt32("rentPrice"),
                                rentOwner = reader.GetInt32("rentowner"),
                                dimension = reader.GetInt32("dimension"),
                                isLocked = reader.GetBoolean("isLocked"),
                                houseEnv = reader.GetString("hEnv"),
                                settings = JsonConvert.DeserializeObject<HouseModel.Settings>(reader.GetString("settings"))
                            });
                        }
                    }
                }
                await connection.CloseAsync();
            }
            return business;
        }

        public static async Task<List<HouseModel>> getPlayerHouses(PlayerModel p)
        {
            List<HouseModel> business = new List<HouseModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM houses WHERE ownerId = @ownerId";
                command.Parameters.AddWithValue("@ownerId", p.sqlID);

                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            business.Add(new HouseModel
                            {
                                ID = reader.GetInt32("ID"),
                                name = reader.GetString("name"),
                                price = reader.GetInt32("price"),
                                ownerId = reader.GetInt32("ownerId"),
                                pos = JsonConvert.DeserializeObject<Position>(reader.GetString("pos")),
                                intPos = JsonConvert.DeserializeObject<Position>(reader.GetString("intPos")),
                                isRentable = reader.GetBoolean("isRentable"),
                                rentPrice = reader.GetInt32("rentPrice"),
                                rentOwner = reader.GetInt32("rentowner"),
                                dimension = reader.GetInt32("dimension"),
                                isLocked = reader.GetBoolean("isLocked"),
                                houseEnv = reader.GetString("hEnv"),
                                settings = JsonConvert.DeserializeObject<HouseModel.Settings>(reader.GetString("settings"))
                            });
                        }
                    }
                }
                await connection.CloseAsync();
            }
            return business;
        }
        public static async Task<List<HouseModel>> getPlayerHouses(int pSql)
        {
            List<HouseModel> business = new List<HouseModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM houses WHERE ownerId = @ownerId";
                command.Parameters.AddWithValue("@ownerId", pSql);

                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            business.Add(new HouseModel
                            {
                                ID = reader.GetInt32("ID"),
                                name = reader.GetString("name"),
                                price = reader.GetInt32("price"),
                                ownerId = reader.GetInt32("ownerId"),
                                pos = JsonConvert.DeserializeObject<Position>(reader.GetString("pos")),
                                intPos = JsonConvert.DeserializeObject<Position>(reader.GetString("intPos")),
                                isRentable = reader.GetBoolean("isRentable"),
                                rentPrice = reader.GetInt32("rentPrice"),
                                rentOwner = reader.GetInt32("rentowner"),
                                dimension = reader.GetInt32("dimension"),
                                isLocked = reader.GetBoolean("isLocked"),
                                houseEnv = reader.GetString("hEnv"),
                                settings = JsonConvert.DeserializeObject<HouseModel.Settings>(reader.GetString("settings"))
                            });
                        }
                    }
                }
                await connection.CloseAsync();
            }
            return business;
        }

        public static async Task<HouseModel> GetHouseByID(int ID)
        {
            HouseModel h = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM houses WHERE ID = @ID";
                command.Parameters.AddWithValue("@ID", ID);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        h = new HouseModel()
                        {
                            ID = reader.GetInt32("ID"),
                            name = reader.GetString("name"),
                            price = reader.GetInt32("price"),
                            ownerId = reader.GetInt32("ownerId"),
                            pos = JsonConvert.DeserializeObject<Position>(reader.GetString("pos")),
                            intPos = JsonConvert.DeserializeObject<Position>(reader.GetString("intPos")),
                            isRentable = reader.GetBoolean("isRentable"),
                            rentPrice = reader.GetInt32("rentPrice"),
                            rentOwner = reader.GetInt32("rentowner"),
                            dimension = reader.GetInt32("dimension"),
                            isLocked = reader.GetBoolean("isLocked"),
                            houseEnv = reader.GetString("hEnv"),
                            settings = JsonConvert.DeserializeObject<HouseModel.Settings>(reader.GetString("settings"))
                        };

                    }
                }
                await connection.CloseAsync();
            }
            return h;
        }
        public static async Task<int> CreateHouse(HouseModel h)
        {
            int resultId = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO houses (name, price, ownerId, pos, intPos, isRentable, rentPrice, rentowner, dimension, isLocked, hEnv)" +
                    " VALUES (@name, @price, @ownerId, @pos, @intPos, @isRentable, @rentPrice, @rentowner, @dimension, @isLocked, @hEnv);";

                command.Parameters.AddWithValue("@name", h.name);
                command.Parameters.AddWithValue("@price", h.price);
                command.Parameters.AddWithValue("@ownerId", h.ownerId);
                command.Parameters.AddWithValue("@pos", JsonConvert.SerializeObject(h.pos));
                command.Parameters.AddWithValue("@intPos", JsonConvert.SerializeObject(h.intPos));
                command.Parameters.AddWithValue("@isRentable", h.isRentable);
                command.Parameters.AddWithValue("@rentPrice", h.rentPrice);
                command.Parameters.AddWithValue("@rentowner", h.rentOwner);
                command.Parameters.AddWithValue("@dimension", h.dimension);
                command.Parameters.AddWithValue("@isLocked", h.isLocked);
                command.Parameters.AddWithValue("@hEnv", h.houseEnv);

                await command.ExecuteNonQueryAsync();
                resultId = (int)command.LastInsertedId;
                await connection.CloseAsync();
            }
            return resultId;
        }
        public static async Task UpdateHouse(HouseModel h)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE houses SET name = @name, price = @price, ownerId = @ownerId, pos = @pos, intPos = @intPos, isRentable = @isRentable," +
                    "rentPrice = @rentPrice, rentowner = @rentowner, dimension = @dimension, isLocked = @isLocked, hEnv = @hEnv, settings = @settings WHERE ID = @ID; ";

                command.Parameters.AddWithValue("@name", h.name);
                command.Parameters.AddWithValue("@price", h.price);
                command.Parameters.AddWithValue("@ownerId", h.ownerId);
                command.Parameters.AddWithValue("@pos", JsonConvert.SerializeObject(h.pos));
                command.Parameters.AddWithValue("@intPos", JsonConvert.SerializeObject(h.intPos));
                command.Parameters.AddWithValue("@isRentable", h.isRentable);
                command.Parameters.AddWithValue("@rentPrice", h.rentPrice);
                command.Parameters.AddWithValue("@rentowner", h.rentOwner);
                command.Parameters.AddWithValue("@dimension", h.dimension);
                command.Parameters.AddWithValue("@isLocked", h.isLocked);
                command.Parameters.AddWithValue("@hEnv", h.houseEnv);
                command.Parameters.AddWithValue("@settings", JsonConvert.SerializeObject(h.settings));

                command.Parameters.AddWithValue("@ID", h.ID);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }
        public static async Task DeleteHouse(int Id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM houses WHERE ID = @ID;";
                command.Parameters.AddWithValue("@ID", Id);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }
        public static async Task<List<int>> getAllHouseIDS()
        {
            List<int> IDS = new List<int>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM houses";


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {


                            IDS.Add(reader.GetInt32("ID"));
                        }
                    }
                }
                await connection.CloseAsync();
            }
            return IDS;
        }
        #endregion

        #region GOV System
        public static async Task<int> CreatePdFine(Globals.System.FineModel fine)
        {
            int resultId = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO fines (sender, target, fine, reason) VALUES (@sender, @target, @fine , @reason);";

                command.Parameters.AddWithValue("@sender", fine.sender);
                command.Parameters.AddWithValue("@target", fine.target);
                command.Parameters.AddWithValue("@fine", fine.fine);
                command.Parameters.AddWithValue("@reason", fine.reason);


                await command.ExecuteNonQueryAsync();
                resultId = (int)command.LastInsertedId;
                await connection.CloseAsync();
            }
            return resultId;
        }

        /// <summary>
        /// girinlen türdeki faction listesini çeker (faction ıd olarak)
        /// </summary>
        /// <param name="factionType"></param>
        /// <returns></returns>
        public static async Task<List<int>> GetTypeOfFactionIDs(int factionType)
        {
            List<int> ids = new List<int>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT ID FROM factions WHERE type =" + factionType + ";";

                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            ids.Add(reader.GetInt32("ID"));
                        }


                    }
                }

                await connection.CloseAsync();
            }
            return ids;
        }
        #endregion

        #region Jail System
        public static async Task<int> CreateJail(Globals.System.JailModel jModel)
        {
            int resultId = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO pdjailarchive (characterId, senderId, reason, time, isAdminJail) VALUES (@characterId, @senderId, @reason, @time, @isAdminJail);";

                command.Parameters.AddWithValue("@characterId", jModel.target);
                command.Parameters.AddWithValue("@senderId", jModel.sender);
                command.Parameters.AddWithValue("@reason", jModel.reason);
                command.Parameters.AddWithValue("@time", jModel.time);

                await command.ExecuteNonQueryAsync();
                resultId = (int)command.LastInsertedId;
                await connection.CloseAsync();
            }
            return resultId;

        }
        #endregion

        #region Inventory System
        public static async Task<List<InventoryModel>> GetPlayerInventoryItems(int playerId)
        {
            List<InventoryModel> inv = new List<InventoryModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM inventory WHERE ownerId = @ownerId";
                command.Parameters.AddWithValue("@ownerId", playerId);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            inv.Add(new InventoryModel
                            {
                                ID = reader.GetInt32("ID"),
                                ownerId = reader.GetInt32("ownerId"),
                                itemName = reader.GetString("itemName"),
                                itemId = reader.GetInt32("itemId"),
                                itemAmount = reader.GetInt32("itemAmount"),
                                itemSlot = reader.GetInt32("itemSlot"),
                                itemData = reader.GetString("itemData"),
                                itemData2 = reader.GetString("itemData2"),
                                itemWeight = reader.GetDouble("itemWeight"),
                                itemPicture = reader.GetString("itemPicture"),
                                Equipable = reader.GetBoolean("Equipable")
                            });
                        }
                    }
                }
                await connection.CloseAsync();
            }
            return inv;
        }

        public static async Task<List<InventoryModel>> getPlayerClientInventoryItems(int playerId)
        {
            List<InventoryModel> inv = new List<InventoryModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM inventory WHERE ownerId = @ownerId";
                command.Parameters.AddWithValue("@ownerId", playerId);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            inv.Add(new InventoryModel
                            {
                                ID = reader.GetInt32("ID"),
                                ownerId = reader.GetInt32("ownerId"),
                                itemName = reader.GetString("itemName"),
                                itemId = reader.GetInt32("itemId"),
                                itemAmount = reader.GetInt32("itemAmount"),
                                itemSlot = reader.GetInt32("itemSlot"),
                                itemData = reader.GetString("itemData"),
                                itemData2 = (reader.GetString("itemData2").Length > 5) ? "yok" : reader.GetString("itemData2"),
                                itemWeight = reader.GetDouble("itemWeight"),
                                itemPicture = reader.GetString("itemPicture"),
                                Equipable = reader.GetBoolean("Equipable")
                            });
                        }
                    }
                }
                await connection.CloseAsync();
            }
            return inv;
        }

        public static async Task<bool> CheckInventoryItem(int ownerId, int itemId)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM inventory WHERE ownerId = @ownerId AND itemId = @itemId LIMIT 1;";
                command.Parameters.AddWithValue("@ownerId", ownerId);
                command.Parameters.AddWithValue("@itemId", itemId);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        return true;
                    }
                }
                await connection.CloseAsync();
            }
            return false;
        }
        public static async Task<InventoryModel> FindInventoryItemWithOwnerId(int ownerId, int itemId)
        {
            InventoryModel item = new InventoryModel();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM inventory WHERE ownerId = @ownerId AND itemId = @itemId";
                command.Parameters.AddWithValue("@ownerId", ownerId);
                command.Parameters.AddWithValue("@itemId", itemId);

                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        item.ID = reader.GetInt32("ID");
                        item.ownerId = reader.GetInt32("ownerId");
                        item.itemName = reader.GetString("itemName");
                        item.itemId = reader.GetInt32("itemId");
                        item.itemAmount = reader.GetInt32("itemAmount");
                        item.itemSlot = reader.GetInt32("itemSlot");
                        item.itemData = reader.GetString("itemData");
                        item.itemData2 = reader.GetString("itemData2");
                        item.itemWeight = reader.GetDouble("itemWeight");
                        item.itemId = reader.GetInt32("itemId");
                        item.itemPicture = reader.GetString("itemPicture");
                        item.Equipable = reader.GetBoolean("Equipable");
                    }
                }
                await connection.CloseAsync();
            }
            return item;
        }
        public static async Task<InventoryModel> FindInventoryItem(int itemID)
        {
            InventoryModel item = new InventoryModel();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM inventory WHERE ID = @ID";
                command.Parameters.AddWithValue("@ID", itemID);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        item.ID = reader.GetInt32("ID");
                        item.ownerId = reader.GetInt32("ownerId");
                        item.itemName = reader.GetString("itemName");
                        item.itemId = reader.GetInt32("itemId");
                        item.itemAmount = reader.GetInt32("itemAmount");
                        item.itemSlot = reader.GetInt32("itemSlot");
                        item.itemData = reader.GetString("itemData");
                        item.itemData2 = reader.GetString("itemData2");
                        item.itemWeight = reader.GetDouble("itemWeight");
                        item.itemId = reader.GetInt32("itemId");
                        item.itemPicture = reader.GetString("itemPicture");
                        item.Equipable = reader.GetBoolean("Equipable");
                    }
                }
                await connection.CloseAsync();
            }
            return item;
        }
        public static async Task<InventoryModel> GetInventoryPhone(PlayerModel p)
        {
            InventoryModel item = new InventoryModel();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM inventory WHERE ownerId = @ownerId AND itemId = '1' AND itemSlot = '12';";
                command.Parameters.AddWithValue("@ownerId", p.sqlID);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        item.ID = reader.GetInt32("ID");
                        item.ownerId = reader.GetInt32("ownerId");
                        item.itemName = reader.GetString("itemName");
                        item.itemId = reader.GetInt32("itemId");
                        item.itemAmount = reader.GetInt32("itemAmount");
                        item.itemSlot = reader.GetInt32("itemSlot");
                        item.itemData = reader.GetString("itemData");
                        item.itemData2 = reader.GetString("itemData2");
                        item.itemWeight = reader.GetDouble("itemWeight");
                        item.itemId = reader.GetInt32("itemId");
                        item.itemPicture = reader.GetString("itemPicture");
                        item.Equipable = reader.GetBoolean("Equipable");
                    }
                }
                await connection.CloseAsync();
            }
            return item;
        }
        public static async Task UpdatePlayerInventoryItem(InventoryModel inv)
        {
            /*using (var context = new db())
            {
                var item = context.Inventory.FirstOrDefault(x => x.ID == inv.ID);
                item = inv;
                context.SaveChanges();
            }*/
            //return;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE inventory SET ownerId = @ownerId, itemName = @itemName, itemId = @itemId, itemAmount = @itemAmount, itemSlot = @itemSlot," +
                    "itemData = @itemData, itemData2 = @itemData2, itemWeight = @itemWeight, itemPicture = @itemPicture, Equipable = @Equipable WHERE ID = @ID;";

                command.Parameters.AddWithValue("@ownerId", inv.ownerId);
                command.Parameters.AddWithValue("@itemName", inv.itemName);
                command.Parameters.AddWithValue("@itemId", inv.itemId);
                command.Parameters.AddWithValue("@itemAmount", inv.itemAmount);
                command.Parameters.AddWithValue("@itemSlot", inv.itemSlot);
                command.Parameters.AddWithValue("@itemData", inv.itemData);
                command.Parameters.AddWithValue("@itemData2", inv.itemData2);
                command.Parameters.AddWithValue("@itemWeight", inv.itemWeight);
                command.Parameters.AddWithValue("@itemPicture", inv.itemPicture);
                command.Parameters.AddWithValue("@Equipable", inv.Equipable);

                command.Parameters.AddWithValue("@ID", inv.ID);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;

        }
        public static async Task CreatePlayerInventoryItem(InventoryModel inv)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO inventory (ownerId, itemName, itemId, itemAmount, itemData, itemData2, itemWeight, itemPicture, Equipable) VALUES (" +
                    "@ownerId, @itemName, @itemId, @itemAmount, @itemData, @itemData2, @itemWeight, @itemPicture, @Equipable);";

                command.Parameters.AddWithValue("@ownerId", inv.ownerId);
                command.Parameters.AddWithValue("@itemName", inv.itemName);
                command.Parameters.AddWithValue("@itemId", inv.itemId);
                command.Parameters.AddWithValue("@itemAmount", inv.itemAmount);
                command.Parameters.AddWithValue("@itemData", inv.itemData);
                command.Parameters.AddWithValue("@itemData2", inv.itemData2);
                command.Parameters.AddWithValue("@itemWeight", inv.itemWeight);
                command.Parameters.AddWithValue("@itemPicture", inv.itemPicture);
                command.Parameters.AddWithValue("@Equipable", inv.Equipable);

                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }
            return;
        }
        public static async Task DeletePlayerInventoryItem(InventoryModel inv)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM inventory WHERE ID = @ID;";
                command.Parameters.AddWithValue("@ID", inv.ID);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }

        public static async Task<List<int>> GetPhoneNumberList()
        {
            List<int> phoneNumbers = new List<int>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM inventory WHERE itemId = '1'";

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            string num = reader.GetString("itemData");
                            Int32.TryParse(num, out int number);
                            if (number != 0)
                            {
                                phoneNumbers.Add(number);
                            }
                        }
                    }
                }
                await connection.CloseAsync();
            }
            return phoneNumbers;
        }
        #endregion

        #region Create System
        public static async Task LoadAllServerCrates()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM crates";


                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            Crate x = new Crate();
                            x.sqlID = reader.GetInt32("ID");
                            x.crateModel = reader.GetString("model");
                            x.type = reader.GetInt32("type");
                            x.owner = reader.GetInt32("owner");
                            x.value = reader.GetString("value");
                            x.pos = JsonConvert.DeserializeObject<Vector3>(reader.GetString("pos"));
                            x.dim = reader.GetInt32("dim");
                            x.useable = reader.GetBoolean("useable");
                            x.locked = reader.GetBoolean("locked");
                            x.password = reader.GetString("password");
                            x.stock = reader.GetInt32("stock");
                            x.settings = JsonConvert.DeserializeObject<Crate.Cratesettings>(reader.GetString("settings"));
                            x.prop = PropStreamer.Create(x.crateModel, x.pos, new Vector3(0, 0, 0), streamRange: 1000, frozen: true, isDynamic: true, dimension: x.dim);
                            x.ID = (int)x.prop.Id;
                            x.textlabel = TextLabelStreamer.Create("~x~[~w~ID: " + x.sqlID + "~x~]~w~~n~Kutu~n~Etkilesim için ~g~/kutu", x.prop.Position, streamRange: 3, dimension: x.dim);
                            CrateEvents.serverCrates.Add(x);
                        }
                    }
                }
                await connection.CloseAsync();
            }
            return;
        }
        public static async Task<int> CreateCrate(Crate x)
        {
            int resultId = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO crates (type, owner, model, value, pos, dim, useable, locked, password, stock) VALUES (@type, @owner, @model, @value, @pos, @dim, @useable, @locked, @password, @stock) ";

                command.Parameters.AddWithValue("@type", x.type);
                command.Parameters.AddWithValue("@owner", x.owner);
                command.Parameters.AddWithValue("@model", x.crateModel);
                command.Parameters.AddWithValue("@value", x.value);
                string position = JsonConvert.SerializeObject(x.pos);
                command.Parameters.AddWithValue("@pos", position);
                command.Parameters.AddWithValue("@dim", x.dim);
                command.Parameters.AddWithValue("@useable", x.useable);
                command.Parameters.AddWithValue("@locked", x.locked);
                command.Parameters.AddWithValue("@password", x.password);
                command.Parameters.AddWithValue("@stock", x.stock);

                await command.ExecuteNonQueryAsync();
                resultId = (int)command.LastInsertedId;
                await connection.CloseAsync();
            }
            x.ID = (int)x.prop.Id;
            x.sqlID = resultId;
            CrateEvents.serverCrates.Add(x);
            return resultId;
        }
        public static async Task DeleteCrate(Crate x)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM crates WHERE ID = @ID;";
                command.Parameters.AddWithValue("@ID", x.sqlID);

                await command.ExecuteNonQueryAsync();
                CrateEvents.serverCrates.Remove(x);

                await connection.CloseAsync();
            }
            return;
        }
        public static async Task UpdateCrate(Crate x)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE crates SET type = @type, owner = @owner, value = @value, pos = @pos, dim = @dim, useable = @useable, locked = @locked, password = @password, model = @model, " +
                    "stock = @stock, settings = @settings WHERE ID = @ID; ";

                command.Parameters.AddWithValue("@type", x.type);
                command.Parameters.AddWithValue("@owner", x.owner);
                command.Parameters.AddWithValue("@value", x.value);
                string position = JsonConvert.SerializeObject(x.pos);
                command.Parameters.AddWithValue("@pos", position);
                command.Parameters.AddWithValue("@dim", x.dim);
                command.Parameters.AddWithValue("@useable", x.useable);
                command.Parameters.AddWithValue("@locked", x.locked);
                command.Parameters.AddWithValue("@password", x.password);
                command.Parameters.AddWithValue("@model", x.crateModel);
                command.Parameters.AddWithValue("@stock", x.stock);
                command.Parameters.AddWithValue("@settings", JsonConvert.SerializeObject(x.settings));

                command.Parameters.AddWithValue("@ID", x.sqlID);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }

        #endregion

        #region Bag System
        public static async Task LoadAllServerBags()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM bags";


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            BagModel x = new BagModel();
                            x.ID = reader.GetInt32("ID");
                            x.model = reader.GetString("model");
                            x.prop = PropStreamer.Create(x.model, JsonConvert.DeserializeObject<Position>(reader.GetString("pos")), new Vector3(0, 0, 0));
                            x.label = TextLabelStreamer.Create("[背包:" + x.ID.ToString() + "]~n~指令: ~g~/~w~bag", JsonConvert.DeserializeObject<Position>(reader.GetString("pos")), streamRange: 2);
                            x.weight = reader.GetInt32("weight");
                            x.Env = reader.GetString("env");
                            BagEvents.serverBags.Add(x);
                        }
                    }
                }
                await connection.CloseAsync();
            }
            return;
        }
        public static async Task<int> CreateBag(PlayerModel p, BagModel b)
        {
            int resultId = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO bags (pos, model, dim, env, weight) VALUES (@pos, @model, @dim, @env, @weight) ";

                command.Parameters.AddWithValue("@pos", JsonConvert.SerializeObject(b.prop.Position));
                command.Parameters.AddWithValue("@model", b.model);
                command.Parameters.AddWithValue("@Dimension", b.prop.Dimension);
                command.Parameters.AddWithValue("@env", b.Env);
                command.Parameters.AddWithValue("@weight", b.weight);

                await command.ExecuteNonQueryAsync();
                resultId = (int)command.LastInsertedId;
                await connection.CloseAsync();
            }
            b.ID = resultId;
            BagEvents.serverBags.Add(b);
            return resultId;
        }
        public static async Task UpdateBag(BagModel b)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE bags SET pos = @pos, model = @model, env = @env, dim = @dim, weight = @weight WHERE ID = @ID; ";

                command.Parameters.AddWithValue("@pos", JsonConvert.SerializeObject(b.prop.Position));
                command.Parameters.AddWithValue("@model", b.model);
                command.Parameters.AddWithValue("@env", b.Env);
                command.Parameters.AddWithValue("@dim", b.prop.Dimension);
                command.Parameters.AddWithValue("@weight", b.weight);

                command.Parameters.AddWithValue("@ID", b.ID);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }
        public static async Task DeleteBag(BagModel b)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM bags WHERE ID = @ID;";
                command.Parameters.AddWithValue("@ID", b.ID);

                await command.ExecuteNonQueryAsync();
                b.prop.Delete();
                b.label.Delete();
                BagEvents.serverBags.Remove(b);

                await connection.CloseAsync();
            }
            return;
        }

        #endregion

        #region Bank System
        public static async Task<OtherSystem.LSCsystems.ATM.BankModel> GetBankAccount(int ID)
        {
            OtherSystem.LSCsystems.ATM.BankModel a = new OtherSystem.LSCsystems.ATM.BankModel();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM bank WHERE ID = @ID LIMIT 1";
                command.Parameters.AddWithValue("@ID", ID);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        a.ID = reader.GetInt32("ID");
                        a.owner = reader.GetInt32("owner");
                        a.accountNo = reader.GetInt32("accountNo");
                        a.accountType = reader.GetInt32("accountType");
                        a.accountMoney = reader.GetInt32("accountMoney");
                        a.otherSettings = reader.GetString("otherSettings");
                        a.usageLogs = reader.GetString("usageLogs");
                    }
                }
                await connection.CloseAsync();
            }
            return a;
        }
        public static async Task UpdateBankAccount(OtherSystem.LSCsystems.ATM.BankModel acc)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE bank SET owner = @owner, accountNo = @accountNo, accountType = @accountType, accountMoney = @accountMoney, " +
                    "otherSettings = @otherSettings, usageLogs = @usageLogs WHERE ID = @ID; ";

                command.Parameters.AddWithValue("@owner", acc.owner);
                command.Parameters.AddWithValue("@accountNo", acc.accountNo);
                command.Parameters.AddWithValue("@accountType", acc.accountType);
                command.Parameters.AddWithValue("@accountMoney", acc.accountMoney);
                command.Parameters.AddWithValue("@otherSettings", acc.otherSettings);
                command.Parameters.AddWithValue("@usageLogs", acc.usageLogs);

                command.Parameters.AddWithValue("@ID", acc.ID);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }
        public static async Task<int> CreateBankAccount(OtherSystem.LSCsystems.ATM.BankModel acc)
        {
            int resultId = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO bank (owner, accountType) VALUES (@owner, @accountType) ";

                command.Parameters.AddWithValue("@owner", acc.owner);
                command.Parameters.AddWithValue("@accountType", acc.accountType);


                await command.ExecuteNonQueryAsync();
                resultId = (int)command.LastInsertedId;
                await connection.CloseAsync();
            }
            return resultId;
        }
        public static async Task DeleteBankAccount(OtherSystem.LSCsystems.ATM.BankModel acc)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM bank WHERE ID = @ID;";
                command.Parameters.AddWithValue("@ID", acc.ID);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }
        public static async Task<List<OtherSystem.LSCsystems.ATM.BankModel>> FindBankAccount(string Key, string Value)
        {
            List<OtherSystem.LSCsystems.ATM.BankModel> a = new List<OtherSystem.LSCsystems.ATM.BankModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM bank WHERE " + Key + " = @Value";
                command.Parameters.AddWithValue("@Value", Value);

                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            a.Add(new OtherSystem.LSCsystems.ATM.BankModel()
                            {
                                ID = reader.GetInt32("ID"),
                                owner = reader.GetInt32("owner"),
                                accountNo = reader.GetInt32("accountNo"),
                                accountType = reader.GetInt32("accountType"),
                                accountMoney = reader.GetInt32("accountMoney"),
                                otherSettings = reader.GetString("otherSettings"),
                                usageLogs = reader.GetString("usageLogs")
                            });
                        }
                    }
                }
                await connection.CloseAsync();
            }
            return a;
        }
        public static async Task<OtherSystem.LSCsystems.ATM.BankModel> GetBankAccWithID(string Key, int value)
        {
            OtherSystem.LSCsystems.ATM.BankModel fact = new OtherSystem.LSCsystems.ATM.BankModel();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM bank WHERE " + Key + " = @ID;";
                command.Parameters.AddWithValue("@ID", value);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();

                        fact.ID = reader.GetInt32("ID");
                        fact.owner = reader.GetInt32("owner");
                        fact.accountNo = reader.GetInt32("accountNo");
                        fact.accountType = reader.GetInt32("accountType");
                        fact.accountMoney = reader.GetInt32("accountMoney");
                        fact.otherSettings = reader.GetString("otherSettings");
                        fact.usageLogs = reader.GetString("usageLogs");
                    }
                }
                await connection.CloseAsync();
            }
            return fact;
        }
        #endregion

        #region MDC Database 
        public static async Task<int> CreateMDCEntry(OtherSystem.LSCsystems.MDC x)
        {
            int resultId = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO mdc (type, owner, sender, value) VALUES (@type, @owner, @sender, @value);";

                command.Parameters.AddWithValue("@type", x.type);
                command.Parameters.AddWithValue("@owner", x.owner);
                command.Parameters.AddWithValue("@sender", x.sender);
                command.Parameters.AddWithValue("@value", x.value);

                await command.ExecuteNonQueryAsync();
                resultId = (int)command.LastInsertedId;
                await connection.CloseAsync();
            }
            return resultId;
        }

        public static async Task UpdateMDCEntry(OtherSystem.LSCsystems.MDC b)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE mdc SET type = @type, owner = @owner, sender = @sender, value = @value WHERE ID = @ID; ";

                command.Parameters.AddWithValue("@type", b.type);
                command.Parameters.AddWithValue("@owner", b.owner);
                command.Parameters.AddWithValue("@sender", b.sender);
                command.Parameters.AddWithValue("@value", b.value);

                command.Parameters.AddWithValue("@ID", b.ID);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }

        public static async Task<List<OtherSystem.LSCsystems.MDC>> GetMDCEntry(int type, int owner)
        {
            List<OtherSystem.LSCsystems.MDC> inv = new List<OtherSystem.LSCsystems.MDC>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM mdc WHERE type = @type AND owner = @owner;";
                command.Parameters.AddWithValue("@type", type);
                command.Parameters.AddWithValue("@owner", owner);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            inv.Add(new OtherSystem.LSCsystems.MDC
                            {
                                ID = reader.GetInt32("ID"),
                                type = reader.GetInt32("type"),
                                owner = reader.GetInt32("owner"),
                                sender = reader.GetInt32("sender"),
                                value = reader.GetString("value")
                            });
                        }
                    }
                }
                await connection.CloseAsync();
            }
            return inv;
        }

        public static async Task<List<OtherSystem.LSCsystems.MDC>> GetMDCAllEntrys(int type)
        {
            List<OtherSystem.LSCsystems.MDC> inv = new List<OtherSystem.LSCsystems.MDC>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM mdc WHERE type = @type";
                command.Parameters.AddWithValue("@type", type);

                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            inv.Add(new OtherSystem.LSCsystems.MDC
                            {
                                ID = reader.GetInt32("ID"),
                                type = reader.GetInt32("type"),
                                owner = reader.GetInt32("owner"),
                                sender = reader.GetInt32("sender"),
                                value = reader.GetString("value")
                            });
                        }
                    }
                }
                await connection.CloseAsync();
            }
            return inv;
        }

        public static async Task<OtherSystem.LSCsystems.MDC> getSingleEntry(int ID)
        {
            OtherSystem.LSCsystems.MDC a = new OtherSystem.LSCsystems.MDC();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM mdc WHERE ID = @ID LIMIT 1";
                command.Parameters.AddWithValue("@ID", ID);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        a.ID = reader.GetInt32("ID");
                        a.type = reader.GetInt32("type");
                        a.owner = reader.GetInt32("owner");
                        a.sender = reader.GetInt32("sender");
                        a.value = reader.GetString("value");
                    }
                }
                await connection.CloseAsync();
            }
            return a;
        }

        public static async Task ClearPlayerMDCDatas(int sqlID)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM mdc WHERE owner = @ID;";
                command.Parameters.AddWithValue("@ID", sqlID);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }

        public static async Task<List<OtherSystem.LSCsystems.MDCEvents.ForumReports>> MDC_GetReports(int type)
        {
            //Alt.Log(type.ToString());
            List<OtherSystem.LSCsystems.MDCEvents.ForumReports> reports = new List<OtherSystem.LSCsystems.MDCEvents.ForumReports>();

            using (MySqlConnection connection = new MySqlConnection(forumConnectionString))
            {
                await connection.OpenAsync();
                //Alt.Log("Connection Açıldı");

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT id_msg, id_topic, subject FROM lsc_messages WHERE id_board = @id_board";
                command.Parameters.AddWithValue("@id_board", type);
                //Alt.Log("Parametreler yerleştirildi.");

                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    //Alt.Log("MYSQL işlendi.");
                    if (reader.HasRows)
                    {
                        //Alt.Log("Row Bulundu.");
                        while (await reader.ReadAsync())
                        {
                            //Alt.Log("While ile okuyorum!");
                            reports.Add(new OtherSystem.LSCsystems.MDCEvents.ForumReports
                            {
                                ID = reader.GetInt32("id_msg"),
                                Name = reader.GetString("subject"),
                                id_topic = reader.GetInt32("id_topic")
                            });
                        }
                    }
                }
                await connection.CloseAsync();
            }

            return reports;
        }
        public static async Task<List<OtherSystem.LSCsystems.MDCEvents.ForumReport>> MDC_GetReport(int type)
        {
            List<OtherSystem.LSCsystems.MDCEvents.ForumReport> reports = new List<OtherSystem.LSCsystems.MDCEvents.ForumReport>();

            using (MySqlConnection connection = new MySqlConnection(forumConnectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT id_msg, body, id_topic FROM lsc_messages WHERE id_topic = @id_topic";
                command.Parameters.AddWithValue("@id_topic", type);

                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            reports.Add(new OtherSystem.LSCsystems.MDCEvents.ForumReport
                            {
                                ID = reader.GetInt32("id_msg"),
                                subject = reader.GetString("body")
                            });
                        }
                    }
                }
                await connection.CloseAsync();
            }

            return reports;
        }

        #endregion

        #region Garage
        public static async Task<List<GarageModel.Garage>> GetAllServerGarages()
        {
            List<GarageModel.Garage> business = new List<GarageModel.Garage>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM garages";


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            business.Add(new GarageModel.Garage
                            {
                                ID = reader.GetInt32("ID"),
                                type = reader.GetInt32("type"),
                                ownerID = reader.GetInt32("ownerID"),
                                pos = JsonConvert.DeserializeObject<Position>(reader.GetString("pos")),
                                intPos = JsonConvert.DeserializeObject<Position>(reader.GetString("intPos")),
                                isLocked = reader.GetBoolean("isLocked"),
                                Dimension = reader.GetInt32("dimension"),
                                settings = JsonConvert.DeserializeObject<GarageModel.Garage.Settings>(reader.GetString("settings"))
                            });
                        }
                    }
                }
                await connection.CloseAsync();
            }
            return business;
        }

        public static async Task<GarageModel.Garage> getGarageByID(int id)
        {
            GarageModel.Garage h = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM garages WHERE ID = @ID";
                command.Parameters.AddWithValue("@ID", id);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        h = new GarageModel.Garage()
                        {
                            ID = reader.GetInt32("ID"),
                            type = reader.GetInt32("type"),
                            ownerID = reader.GetInt32("ownerID"),
                            pos = JsonConvert.DeserializeObject<Position>(reader.GetString("pos")),
                            intPos = JsonConvert.DeserializeObject<Position>(reader.GetString("intPos")),
                            isLocked = reader.GetBoolean("isLocked"),
                            Dimension = reader.GetInt32("dimension"),
                            settings = JsonConvert.DeserializeObject<GarageModel.Garage.Settings>(reader.GetString("settings"))
                        };

                    }
                }
                await connection.CloseAsync();
            }
            return h;

        }

        public static async Task UpdateGarage(GarageModel.Garage g)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE garages SET type = @type, ownerID = @ownerID, pos = @pos, intPos = @intPos, isLocked = @isLocked, dimension = @dimension, settings = @settings WHERE ID = @ID; ";

                command.Parameters.AddWithValue("@type", g.type);
                command.Parameters.AddWithValue("@ownerID", g.ownerID);
                command.Parameters.AddWithValue("@pos", JsonConvert.SerializeObject(g.pos));
                command.Parameters.AddWithValue("@intPos", JsonConvert.SerializeObject(g.intPos));
                command.Parameters.AddWithValue("@isLocked", g.isLocked);
                command.Parameters.AddWithValue("@dimension", g.Dimension);
                command.Parameters.AddWithValue("@settings", JsonConvert.SerializeObject(g.settings));


                command.Parameters.AddWithValue("@ID", g.ID);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }

        public static async Task<int> CreateGarage(GarageModel.Garage g)
        {
            int resultId = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO garages (pos)" +
                    " VALUES (@pos);";

                command.Parameters.AddWithValue("@pos", JsonConvert.SerializeObject(g.pos));


                await command.ExecuteNonQueryAsync();
                resultId = (int)command.LastInsertedId;
                await connection.CloseAsync();
            }
            return resultId;
        }

        public static async Task DeleteGarage(GarageModel.Garage g)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM garages WHERE ID = @ID;";
                command.Parameters.AddWithValue("@ID", g.ID);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }
        #endregion

        #region Teleporters
        public static async Task<List<teleporters.TeleportModel>> GetAllTeleporters()
        {
            List<teleporters.TeleportModel> business = new List<teleporters.TeleportModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM teleporters";


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            business.Add(new teleporters.TeleportModel
                            {
                                ID = reader.GetInt32("ID"),
                                pos = JsonConvert.DeserializeObject<Position>(reader.GetString("pos")),
                                coords = JsonConvert.DeserializeObject<List<teleporters.TeleportModel.PosModel>>(reader.GetString("coords")),
                                dimension = reader.GetInt32("dimension"),
                                OwnerId = reader.GetInt32("owner"),
                                isLocked = reader.GetBoolean("locked")
                            });
                        }
                    }
                }
                await connection.CloseAsync();
            }
            return business;
        }

        public static async Task<teleporters.TeleportModel> GetTeleporterByID(int id)
        {
            teleporters.TeleportModel h = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM teleporters WHERE ID = @ID";
                command.Parameters.AddWithValue("@ID", id);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        h = new teleporters.TeleportModel()
                        {
                            ID = reader.GetInt32("ID"),
                            pos = JsonConvert.DeserializeObject<Position>(reader.GetString("pos")),
                            coords = JsonConvert.DeserializeObject<List<teleporters.TeleportModel.PosModel>>(reader.GetString("coords")),
                            OwnerId = reader.GetInt32("owner"),
                            isLocked = reader.GetBoolean("locked")
                        };

                    }
                }
                await connection.CloseAsync();
            }
            return h;

        }

        public static async Task UpdateTeleporters(teleporters.TeleportModel g)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE teleporters SET pos = @pos, coords = @coords, dimension = @dimension, locked = @locked, owner = @owner WHERE ID = @ID; ";

                command.Parameters.AddWithValue("@pos", JsonConvert.SerializeObject(g.pos));
                command.Parameters.AddWithValue("@coords", JsonConvert.SerializeObject(g.coords));
                command.Parameters.AddWithValue("@dimension", g.dimension);
                command.Parameters.AddWithValue("@locked", g.isLocked);
                command.Parameters.AddWithValue("@owner", g.OwnerId);

                command.Parameters.AddWithValue("@ID", g.ID);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }

        public static async Task<int> CreateTeleporter(teleporters.TeleportModel g)
        {
            int resultId = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO teleporters (pos, dimension)" +
                    " VALUES (@pos, @dimension);";

                command.Parameters.AddWithValue("@pos", JsonConvert.SerializeObject(g.pos));
                command.Parameters.AddWithValue("@dimension", g.dimension);


                await command.ExecuteNonQueryAsync();
                resultId = (int)command.LastInsertedId;
                await connection.CloseAsync();
            }
            return resultId;
        }

        public static async Task DeleteTeleporter(teleporters.TeleportModel g)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM teleporters WHERE ID = @ID;";
                command.Parameters.AddWithValue("@ID", g.ID);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }
        #endregion

        #region Keys
        public class KeyModel
        {
            public int keyOwner { get; set; } = 0;
        }

        public static async Task<List<KeyModel>> getVehicleKeys(int vehicleID)
        {
            List<KeyModel> keys = new List<KeyModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM vehicles WHERE ID = @ID LIMIT 1";
                command.Parameters.AddWithValue("@ID", vehicleID);

                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        keys = JsonConvert.DeserializeObject<List<KeyModel>>(reader.GetString("keyusers"));
                    }
                }
                await connection.CloseAsync();
            }

            return keys;
        }

        public static async Task updateVehicleKeys(int vehicleID, List<KeyModel> keys)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE vehicles SET keyusers = @keyusers WHERE ID = @ID; ";

                command.Parameters.AddWithValue("@keyusers", JsonConvert.SerializeObject(keys));

                command.Parameters.AddWithValue("@ID", vehicleID);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }


        public static async Task<List<KeyModel>> getBusinessKeys(int businessID)
        {
            List<KeyModel> keys = new List<KeyModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM business WHERE ID = @ID LIMIT 1";
                command.Parameters.AddWithValue("@ID", businessID);

                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        keys = JsonConvert.DeserializeObject<List<KeyModel>>(reader.GetString("keyusers"));
                    }
                }
                await connection.CloseAsync();
            }

            return keys;
        }

        public static async Task updateBusinessKeys(int businessID, List<KeyModel> keys)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE business SET keyusers = @keyusers WHERE ID = @ID; ";

                command.Parameters.AddWithValue("@keyusers", JsonConvert.SerializeObject(keys));

                command.Parameters.AddWithValue("@ID", businessID);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }


        public static async Task<List<KeyModel>> getHouseKeys(int houseID)
        {
            List<KeyModel> keys = new List<KeyModel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM houses WHERE ID = @ID LIMIT 1";
                command.Parameters.AddWithValue("@ID", houseID);

                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        keys = JsonConvert.DeserializeObject<List<KeyModel>>(reader.GetString("keyusers"));
                    }
                }
                await connection.CloseAsync();
            }

            return keys;
        }

        public static async Task updateHouseKeys(int houseID, List<KeyModel> keys)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE houses SET keyusers = @keyusers WHERE ID = @ID; ";

                command.Parameters.AddWithValue("@keyusers", JsonConvert.SerializeObject(keys));

                command.Parameters.AddWithValue("@ID", houseID);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }

        /// <summary>
        /// type : 0 ARAÇ | 1 EV | 2 İşyeri
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static async Task<List<int>> GetOwnedKeys(int ID, int type = 0)
        {
            // SELECT * FROM `vehicles` WHERE `keyusers` LIKE '%\"keyOwner\":3152%'
            List<int> keys = new();
            switch (type)
            {
                default:
                case 0:
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        MySqlCommand command = connection.CreateCommand();
                        command.CommandText = "SELECT * FROM vehicles WHERE JSON_CONTAINS(JSON_EXTRACT(keyusers, '$[*].keyOwner'), @ID, '$');";
                        command.Parameters.AddWithValue("@ID", ID);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (await reader.ReadAsync())
                                {
                                    keys.Add(reader.GetInt32("ID"));
                                }
                            }
                        }
                        await connection.CloseAsync();
                    }
                    return keys;

                case 1:
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        MySqlCommand command = connection.CreateCommand();
                        command.CommandText = "SELECT * FROM houses WHERE JSON_CONTAINS(JSON_EXTRACT(keyusers, '$[*].keyOwner'), @ID, '$');";
                        command.Parameters.AddWithValue("@ID", ID);

                        using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                while (await reader.ReadAsync())
                                {
                                    keys.Add(reader.GetInt32("ID"));
                                }
                            }
                        }
                        await connection.CloseAsync();
                    }
                    return keys;

                case 2:
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        MySqlCommand command = connection.CreateCommand();
                        command.CommandText = "SELECT * FROM business WHERE JSON_CONTAINS(JSON_EXTRACT(keyusers, '$[*].keyOwner'), @ID, '$');";
                        command.Parameters.AddWithValue("@ID", ID);

                        using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                while (await reader.ReadAsync())
                                {
                                    keys.Add(reader.GetInt32("ID"));
                                }
                            }
                        }
                        await connection.CloseAsync();
                    }
                    return keys;
            }
        }
        #endregion

        #region HelpMenu
        public static async Task<List<OtherSystem.LSCsystems.HelpPage.Category>> getHelpCategorys()
        {
            List<HelpPage.Category> cate = new();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM help_categorys";

                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            cate.Add(new()
                            {
                                label = reader.GetString("label"),
                                icon = reader.GetString("icon"),
                                value = reader.GetString("label")
                            });
                        }

                    }
                }
                await connection.CloseAsync();
            }
            return cate;
        }

        public static async Task removeHelpCategory(string name)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM help_categorys WHERE label = @ID;";
                command.Parameters.AddWithValue("@ID", name);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }

        public static async Task addHelpCategory(string label, string icon)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO help_categorys (label, icon) VALUES (@label, @icon);";

                command.Parameters.AddWithValue("@label", label);
                command.Parameters.AddWithValue("@icon", icon);

                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }
            return;
        }

        public static async Task<List<OtherSystem.LSCsystems.HelpPage.Com>> getHelpCommand(string category)
        {
            List<HelpPage.Com> cate = new();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM help_commands WHERE category = @category";
                command.Parameters.AddWithValue("@category", category);

                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            cate.Add(new()
                            {
                                komut = reader.GetString("komut"),
                                aciklama = reader.GetString("aciklama"),
                                parametreler = reader.GetString("parametreler"),
                                yetki = reader.GetString("yetki"),
                                ornek = reader.GetString("ornek")
                            });
                        }

                    }
                }
                await connection.CloseAsync();
            }
            return cate;
        }

        public static async Task removeHelpCommand(string name)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "DELETE FROM help_commands WHERE komut = @ID;";
                command.Parameters.AddWithValue("@ID", name);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }

        public static async Task addHelpCommand(string category, string comm, string use, string parameter, string perm, string example)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO help_commands (category, komut, aciklama, parametreler, yetki, ornek) VALUES (" +
                    "@category, @komut, @aciklama, @parametreler, @yetki, @ornek);";

                command.Parameters.AddWithValue("@category", category);
                command.Parameters.AddWithValue("@komut", comm);
                command.Parameters.AddWithValue("@aciklama", use);
                command.Parameters.AddWithValue("@parametreler", parameter);
                command.Parameters.AddWithValue("@yetki", perm);
                command.Parameters.AddWithValue("@ornek", example);

                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
            }
            return;
        }
        #endregion

        #region Seçim
        public static async Task<int> GetSecim(int AccountId)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT ID, secim FROM accounts WHERE ID = @ID";
                command.Parameters.AddWithValue("@ID", AccountId);


                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        res = reader.GetInt32("secim");

                    }
                }
                await connection.CloseAsync();
            }
            return res;
        }
        public static async Task SetSecim(int accountId, int secim)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE accounts SET secim = @keyusers WHERE ID = @ID; ";

                command.Parameters.AddWithValue("@keyusers", secim);

                command.Parameters.AddWithValue("@ID", accountId);

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
            return;
        }

        public static async Task<(int, int)> GetSecimAll()
        {
            int sapd = 0;
            int alianceParty = 0;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT secim FROM accounts WHERE secim = 1 OR secim = 2";

                using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            if (reader.GetInt32("secim") == 1)
                                ++sapd;
                            else
                                ++alianceParty;
                        }

                    }
                }
                await connection.CloseAsync();
            }

            return (sapd, alianceParty);
        }
        #endregion
    }


}
