using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace outRp.Models
{
    public class AccountModel
    {
        public int ID { get; set; }
        public string? kookId { get; set; }
        public string password { get; set; }
        public int accountType { get; set; }
        public string accountEmail { get; set; }
        public string forumName { get; set; }
        public int adminLevel { get; set; }
        public bool banned { get; set; }
        public int characterLimit { get; set; }
        public bool isOnline { get; set; }
        public ulong socialClubID { get; set; }
        public int lscPoint { get; set; }
        public ulong HwId { get; set; }
        public ulong HwIdEx { get; set; }
        public int ReportCount { get; set; }
        public int QuestionCount { get; set; }
        public int AdversimentCount { get; set; }
        public OtherData OtherData { get; set; }

        public Task getMysql(string username) => Database.DatabaseMain.getAccountInfo(username);
        public async Task Update() => await Database.DatabaseMain.updateAccInfo(this);
        public void Update2() => Database.DatabaseMain.updateAccInfo2(this);
    }

    public class OtherData
    {
        public List<OtherData_Inner.Informations> Info { get; set; } = new List<OtherData_Inner.Informations>();
        public List<OtherData_Inner.Refund> Refunds { get; set; } = new List<OtherData_Inner.Refund>();
    }

    public class OtherData_Inner
    {
        public class Informations
        {
            public string Title { get; set; }
            public string Body { get; set; }
            public DateTime Date { get; set; }
        }

        public class Refund
        {
            public string Title { get; set; }
            public string Body { get; set; }
            public int Cash { get; set; }
            public DateTime Date { get; set; }
        }
    }
    public class ForumAccountModel
    {
        public int ID { get; set; }
        public string member_name { get; set; }
        public string passwd { get; set; }
        public string avatar { get; set; }
    }
}
