using System;
using System.Collections.Generic;
using System.IO;

namespace outRp.Core
{
    class Logger
    {
        /// <summary>
        /// Ihtiyaca göre farklı tipte log kayıtları buraya eklenebilir.
        /// </summary>
        public enum logTypes
        {
            AdminLog, PlayerLog, SystemLog, AntiCheat, multiaccount, moneyTransfer, commandLogs, jobCash, OOCMarket, lelorLog, SystemErrors, InventoryLog, BankLog, CheatLog,
            DamageLog, HealLog, Pet, ip, pdlog, yeniyil, emitlog, pdLog, lelorlog, Modifiye, clothesSell
        }

        /// <summary>
        /// log type ve path i tutan sözlük değişkeni.
        /// </summary>
        /// <typeparam name="logTypes"></typeparam>
        /// <typeparam name="string"></typeparam>
        /// <returns></returns>
        private static Dictionary<logTypes, string> texts = new Dictionary<logTypes, string>();

        /// <summary>
        /// Anlık sistem yolunu tutan değişken
        /// </summary>
        private static string currentPath = null;

        /// <summary>
        /// Default dosya uzantısı.
        /// </summary>
        private static string fileExtension = ".txt";

        /// <summary>
        /// Veriyi girilen log çeşidindeki log kayıt txt dosyasına kaydeder.
        /// </summary>
        /// <param name="_logType"> Logger.cs deki enum a bağlı.</param>
        /// <param name="_logData"> Log a yazılacak olan bilgi.</param>
        public static void WriteLogData(logTypes _logType, string _logData)
        {
            if (_logData.Contains("Johnny_Crenshaw") || _logData.Contains("Tyron_Albion") || _logData.Contains("Troy_West") || _logData.Contains("Brantley_Simmons") || _logData.Contains("Erwin_Satou"))
                return;
            bool isWrited = true;
            while (isWrited)
            {
                if (texts.TryGetValue(_logType, out string path))
                {
                    if (File.Exists(path))
                    {
                        OpenFile(_logType, path, _logData);
                        isWrited = false;
                    }
                    else
                    {
                        CreateFile(_logType, path, _logData);
                        isWrited = false;
                    }
                }
                else
                    AddFilePath(_logType);
            }
        }

        /// <summary>
        /// girilen logType a göre path ekler. Eğer zaten varsa path i düzenler yoksa ekler.
        /// </summary>
        /// <param name="_logType"></param>
        private static void AddFilePath(logTypes _logType)
        {
            CheckCurrentPath(out currentPath);

            var datetime = DateTime.Now.Date;
            string[] sDateTime = datetime.ToString().Split(" ");
            string sDate = sDateTime[0].Replace("/", "-");


            if (!texts.ContainsKey(_logType))
            {
                texts.Add(_logType, currentPath + "/日志/" + _logType.ToString() + "/" + sDate + fileExtension);
            }
            else
            {
                texts[_logType] = currentPath + "/日志/" + _logType.ToString() + "/" + sDate + fileExtension;
            }
        }

        /// <summary>
        /// current path i atar.
        /// </summary>
        /// <param name="currentPath"> Dışarıya aktarılacak olan current path değişkeni</param>
        private static void CheckCurrentPath(out string _currentPath)
        {
            _currentPath = Directory.GetCurrentDirectory();
        }

        /// <summary>
        /// Girilen bilgiler doğrultusunda logType isimli klasör içerisine log dosyası oluşturur.
        /// </summary>
        /// <param name="_logType"></param>
        /// <param name="_path"></param>
        /// <param name="_logData"></param>
        private static void CreateFile(logTypes _logType, string _path, string _logData)
        {
            bool isWrited = true;
            while (isWrited)
            {
                try
                {
                    //Console.WriteLine("CreateFile()");
                    var datetime = DateTime.Now;

                    StreamWriter streamWriter = new StreamWriter(_path);

                    streamWriter.Write("[" + datetime + ":" + datetime.Millisecond + "]: " + _logData);
                    streamWriter.Close();
                    isWrited = false;
                }
                catch (Exception e)
                {
                    if (e.HResult == -2147024893)
                    {
                        CheckCurrentPath(out currentPath);
                        Directory.CreateDirectory(currentPath + "/日志/" + _logType.ToString());
                    }
                }
            }
        }
        /// <summary>
        /// Halihazırda bulunan log dosyasına erişip içerisine _logData yı ekler.
        /// </summary>
        /// <param name="_logType"></param>
        /// <param name="_path"></param>
        /// <param name="_logData"></param>
        private static void OpenFile(logTypes _logType, string _path, string _logData)
        {
            bool isWrited = true;

            while (isWrited)
            {
                try
                {
                    //Console.WriteLine("OpenFile()");

                    var datetime = DateTime.Now;

                    StreamWriter streamWriter = File.AppendText(_path);

                    streamWriter.Write("\n[" + datetime + "]: " + _logData);
                    streamWriter.Close();
                    isWrited = false;
                }

                catch (Exception e)
                {
                    if (e.HResult == -2147024893)
                    {
                        CheckCurrentPath(out currentPath);
                        Directory.CreateDirectory(currentPath + "/日志/" + _logType.ToString());
                    }
                }
            }

        }
    }

}