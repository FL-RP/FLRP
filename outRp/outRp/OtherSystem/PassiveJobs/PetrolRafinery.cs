using System;
using System.Collections.Generic;
using AltV.Net.Data;
using System.Threading.Tasks;
using outRp.Chat;
using outRp.Models;
using outRp.OtherSystem.Textlabels;
using Newtonsoft.Json;

namespace outRp.OtherSystem.PassiveJobs
{
    public class PetrolRafinery
    {
        public class Rafinery
        {
            public string Name { get; set; }
            public int Owner { get; set; } = 0;
            public int Progress { get; set; }
            public int Vault { get; set; }
            public Position Pos { get; set; }
            public ulong TextLabelID { get; set; }
            
        }

        public static List<Rafinery> Rafinerys = new();

        public static async Task LoadRafinerys(string json)
        {
            Rafinerys = JsonConvert.DeserializeObject<List<Rafinery>>(json);
            foreach(var raf in Rafinerys)
            {
                raf.TextLabelID = TextLabelStreamer.Create(await GetRafineryText(raf), raf.Pos, 0, true, scale: 0.5f, font: 0, streamRange: 10).Id;
            }

            Core.Core.OutputLog("[炼油厂] 系统加载.", ConsoleColor.Green);
            return;
        }

        public static string SaveRafinerys()
        {
            return JsonConvert.SerializeObject(Rafinerys);
        }

        public static async Task<string> GetRafineryText(Rafinery raf)
        {
            PlayerModelInfo player = await Database.DatabaseMain.getCharacterInfo(raf.Owner);
            return "~b~[~w~炼油厂~b~]~w~~n~所有人: " + ((player != null) ? player.characterName.Replace("_", " ") : "无主") + "~n~状态: " + raf.Progress + "/100";
        }

        public static async Task UpdateRafinery(Rafinery raf)
        {
            PlayerLabel textlabel = TextLabelStreamer.GetDynamicTextLabel(raf.TextLabelID);
            if(textlabel != null)
            {
                textlabel.SetText(await  GetRafineryText(raf));
                textlabel.Font = 0;
                textlabel.Scale = 0.5f;
            }
        }

        public static void RafineryTimerEvent()
        {
            foreach(var raf in Rafinerys)
            {
                if (raf.Vault <= 0)
                    continue;
                else if (raf.Progress >= 100)
                    continue;
                else
                {
                    raf.Progress += 1;
                    raf.Vault -= 1;
                    UpdateRafinery(raf);
                }
                
            }
        }

        #region Admin Commands 

        public void COM_RafineryAdmin(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, "[错误] Bu komutu kullanabilmek için yetkiniz yok!"); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /rafineri [çeşit] [varsa değer]"); return; }

        }
        #endregion
    }
}
