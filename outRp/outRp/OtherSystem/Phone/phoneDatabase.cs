using MySqlConnector;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace outRp.OtherSystem.Phone
{
    public class phoneDatabase
    {
        #region DATABASE INFO
        private static string connectionString = "Server=localhost;Database=altv;Uid=root;Pwd='root';";
        //private static string connectionString = "Server=localhost;Database=altv;Uid=root;Pwd=;";
        #endregion


        public static bool checkPhone(int number)
        {
            bool banned = false;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM phone WHERE number = @number LIMIT 1";
                command.Parameters.AddWithValue("@number", number);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        banned = true;
                    }
                }
                connection.Close();
            }
            return banned;
        }
        public static int CreatePhoneData(int number)
        {
            int resultId = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO phone (number) VALUES (@number) ";

                command.Parameters.AddWithValue("@number", number);


                command.ExecuteNonQuery();
                resultId = (int)command.LastInsertedId;
                connection.Close();
            }
            return resultId;
        }
        public static List<pClass.Contact> getPhoneContacts(int PhoneNumber)
        {

            List<pClass.Contact> contacts = new List<pClass.Contact>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM phone WHERE number = @number";
                command.Parameters.AddWithValue("@number", PhoneNumber);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        contacts = JsonConvert.DeserializeObject<List<pClass.Contact>>(reader.GetString("contacts"));
                    }
                }
                connection.Close();
            }
            return contacts;
        }
        public static void UpdateContacts(int phoneNumber, List<pClass.Contact> contacts)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE phone SET contacts = @contacts WHERE number = @number; ";

                command.Parameters.AddWithValue("@contacts", JsonConvert.SerializeObject(contacts));


                command.Parameters.AddWithValue("@number", phoneNumber);

                command.ExecuteNonQuery();

                connection.Close();
            }
            return;
        }

        public static string getPhoneSettings(int phoneNumber)
        {
            string contacts = "[]";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM phone WHERE number = @number";
                command.Parameters.AddWithValue("@number", phoneNumber);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        contacts = reader.GetString("settings");
                    }
                }
                connection.Close();
            }
            return contacts;
        }

        public static void updatePhoneSettings(int phoneNumber, string settings)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE phone SET settings = @settings WHERE number = @number; ";

                command.Parameters.AddWithValue("@settings", settings);


                command.Parameters.AddWithValue("@number", phoneNumber);

                command.ExecuteNonQuery();

                connection.Close();
            }
            return;
        }
        public static List<pClass.Messages> getAllMessages(int phoneNumber)
        {
            List<pClass.Messages> contacts = new List<pClass.Messages>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM phone WHERE number = @number";
                command.Parameters.AddWithValue("@number", phoneNumber);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        contacts = JsonConvert.DeserializeObject<List<pClass.Messages>>(reader.GetString("messages"));
                    }
                }
                connection.Close();
            }
            return contacts;
        }
        public static void updateAllMessages(int phoneNumber, List<pClass.Messages> messages)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE phone SET messages = @messages WHERE number = @number; ";

                command.Parameters.AddWithValue("@messages", JsonConvert.SerializeObject(messages));


                command.Parameters.AddWithValue("@number", phoneNumber);

                command.ExecuteNonQuery();

                connection.Close();
            }
            return;
        }
        public static List<pClass.MessagesTiny> getAllMessagesTiny(int phoneNumber)
        {
            List<pClass.MessagesTiny> contacts = new List<pClass.MessagesTiny>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM phone WHERE number = @number";
                command.Parameters.AddWithValue("@number", phoneNumber);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        contacts = JsonConvert.DeserializeObject<List<pClass.MessagesTiny>>(reader.GetString("messages"));
                    }
                }
                connection.Close();
            }
            return contacts;
        }

        public static List<pClass.image> getAllimages(int phoneNumber)
        {
            List<pClass.image> contacts = new List<pClass.image>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM phone WHERE number = @number";
                command.Parameters.AddWithValue("@number", phoneNumber);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        contacts = JsonConvert.DeserializeObject<List<pClass.image>>(reader.GetString("photos"));
                    }
                }
                connection.Close();
            }
            return contacts;
        }

        public static void updateAllimages(int phoneNumber, List<pClass.image> images)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "UPDATE phone SET photos = @photos WHERE number = @number; ";

                command.Parameters.AddWithValue("@photos", JsonConvert.SerializeObject(images));


                command.Parameters.AddWithValue("@number", phoneNumber);

                command.ExecuteNonQuery();

                connection.Close();
            }
            return;
        }

    }
}
