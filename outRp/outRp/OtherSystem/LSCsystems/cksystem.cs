using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Globals;
using outRp.Models;
using outRp.OtherSystem.NativeUi;
using outRp.OtherSystem.Textlabels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;

namespace outRp.OtherSystem.LSCsystems
{
    public class cksystem : IScript
    {

        public class Corpse
        {
            public ulong ID { get; set; } = 0;
            public string Name { get; set; } = "无";
            public string Description { get; set; } = "空";
            public DateTime ckDate { get; set; } = DateTime.Now;
            public Position pos { get; set; } = new Position(0, 0, 0);
            public int Dimension { get; set; } = 0;
            public string spectators { get; set; } = "无";
            public int RemainingTime { get; set; } = 72;
            public bool isEmbeb { get; set; } = false;
            public string EmbebName { get; set; } = "";

        }

        public static List<Corpse> serverCorpses = new List<Corpse>();

        public static void LoadAllCorpses(string data)
        {
            serverCorpses = JsonConvert.DeserializeObject<List<Corpse>>(data);
            int coprseCounter = 0;
            foreach (Corpse c in serverCorpses)
            {
                //prop_gravestones_08a
                if (c.isEmbeb)
                {
                    c.ID = PropStreamer.Create("v_ilev_body_parts", c.pos, new System.Numerics.Vector3(0, 0, 0), c.Dimension, false, true, true).Id;
                }
                else
                {
                    c.ID = PropStreamer.Create("xm_prop_x17_corpse_03", c.pos, new System.Numerics.Vector3(0, 0, 0), c.Dimension, false, true, true).Id;
                }
                coprseCounter++;
            }

            Alt.Log(coprseCounter + " 已加载所有尸体.");
            return;
        }

        public static void CorpseUpdateRemaining()
        {
            foreach (Corpse c in serverCorpses)
            {
                c.RemainingTime -= 1;

                if (c.RemainingTime < 0) { c.RemainingTime = 0; }
            }
        }


        [Command("ck")]
        public static void COM_SendCKMenu(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /ck [id]"); return; }

            if (!Int32.TryParse(args[0], out int tSql)) { MainChat.SendInfoChat(p, "[用法] /ck [id]"); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家!"); return; }

            Inputs.SendTypeInput(t, "请输入您尸体留下的信息和证据.", "CK:Step1", "x");
            return;
        }

        [AsyncClientEvent("CK:Step1")]
        public async Task CK_Step1(PlayerModel p, string newName, string trash)
        {
            if (newName.Length <= 30) { MainChat.SendInfoChat(p, "[!] 您应该写一篇更长、更具描述性的文字，以便警察小组在检查尸体时可以找到有关您角色死亡的信息和证据."); Inputs.SendTypeInput(p, "请输入您尸体留下的信息和证据", "CK:Step1", "x"); return; }
            Corpse c = new Corpse();
            c.Name = p.characterName.Replace("_", " ");
            c.Description = newName;
            c.ckDate = DateTime.Now;
            Position cpos = p.Position;
            cpos.Z -= 1;
            c.pos = cpos;
            c.Dimension = p.Dimension;
            string Spectators = "";
            foreach (PlayerModel t in Alt.GetAllPlayers())
            {
                if (!t.adminWork && t.Position.Distance(p.Position) < 30)
                {
                    Spectators += " " + t.characterName.Replace("_", " ");
                }
            }
            c.spectators = Spectators;
            c.RemainingTime = 72;
            c.ID = PropStreamer.Create("xm_prop_x17_corpse_03", c.pos, new System.Numerics.Vector3(0, 0, 0), p.Dimension, false, true, true).Id;
            serverCorpses.Add(c);
            p.isCk = true;
            p.factionId = 0;
            p.factionRank = 0;
            await p.updateSql();

            AccountModel pAcc = await Database.DatabaseMain.getAccInfo(p.accountId);
            pAcc.characterLimit += 1;
            await pAcc.Update();
            MainChat.SendAdminChat(p.characterName.Replace("_", " ") + " CK了.");


            p.Kick("服务器: 您可以通过重新登录游戏来创建您的新角色.");
            return;
        }

        public static async Task<bool> CanUse(PlayerModel p)
        {
            if (!await Globals.System.PD.CheckPlayerInPd(p))
            {
                if (!await Globals.System.FD.CheckPlayerInFD(p))
                    return false;
                else
                    return true;
            }

            return true;
        }

        public static void placeCorpseGround(PlayerModel p, string item)
        {
            Corpse c = JsonConvert.DeserializeObject<Corpse>(item);
            Position pos = p.Position;
            pos.Z -= 1;
            c.ID = PropStreamer.Create("xm_prop_x17_corpse_03", pos, new System.Numerics.Vector3(0, 0, 0), p.Dimension, false, true, true).Id;
            c.isEmbeb = false;
            c.pos = pos;
            serverCorpses.Add(c);
            MainChat.SendInfoChat(p, "[!] 已将尸体放置至地上.");
            return;
        }


        [Command("checkcorpse")]
        public async Task COM_CheckCorpse(PlayerModel p)
        {
            if (!await CanUse(p)) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            //Corpse c = serverCorpses.Find(x => p.Position.Distance(x.pos) < 10);
            Corpse c = serverCorpses.Where(x => p.Position.Distance(x.pos) < 10 && x.Dimension == p.Dimension).OrderBy(x => p.Position.Distance(x.pos)).FirstOrDefault();
            if (c == null) { MainChat.SendErrorChat(p, "[错误] 附近没有尸体!"); return; }
            if (c.isEmbeb) { MainChat.SendErrorChat(p, "[错误] 这具尸体被埋了, 请先将尸体挖出(/emcorpse)."); return; }

            int change = 73 - c.RemainingTime;
            System.Text.StringBuilder desc = new StringBuilder(c.Description);
            Random rnd = new Random();
            for (int a = 0; a <= change; a++)
            {
                desc[rnd.Next(0, desc.Length - 1)] = '?';
            }

            StringBuilder name = new StringBuilder(c.Name);
            string showName = c.Name;
            //if(c.RemainingTime <= 50 && c.RemainingTime > 20)
            //{
            //    for(int b = 0; b <= 10; b++)
            //    {
            //        name[rnd.Next(0, name.Length - 1)] = '?';
            //    }
            //    showName = name.ToString();
            //}
            //else if(c.RemainingTime <= 20)
            //{
            //    showName = "??? ???";
            //}

            MainChat.SendInfoChat(p, "<center>尸体信息</center><br>姓名: " + showName + "<br>说明: " + desc.ToString());
            return;
        }

        [Command("takecorpse")]
        public async Task COM_TakeCorpse(PlayerModel p)
        {
            //Corpse c = serverCorpses.Find(x => p.Position.Distance(x.pos) < 10);
            Corpse c = serverCorpses.Where(x => p.Position.Distance(x.pos) < 10 && x.Dimension == p.Dimension).OrderBy(x => p.Position.Distance(x.pos)).FirstOrDefault();
            if (c == null) { MainChat.SendErrorChat(p, "[错误] 附近没有尸体."); return; }
            if (c.isEmbeb) { MainChat.SendErrorChat(p, "[错误] 这具尸体被埋了, 请先将尸体挖出(/emcorpse)."); return; }

            c.EmbebName += " " + p.characterName.Replace("_", " ");

            ServerItems i = new ServerItems();
            i.amount = 1;
            i.equipable = false;
            i.equipSlot = 0;
            i.ID = 49;
            i.name = "尸体";
            i.objectModel = "xm_prop_x17_corpse_03";
            i.picture = "49";
            i.price = 0;
            i.stackable = false;
            i.type = 36;
            i.weight = 5;
            i.data2 = JsonConvert.SerializeObject(c);

            if (await Inventory.AddInventoryItem(p, i, 1))
            {
                MainChat.SendInfoChat(p, "[!] 成功取出尸体(( 请合理扮演, 虽然尸体在您的库存, 但这只是为了配合您完成运送 ))!");
                PropStreamer.GetProp(c.ID).Delete();
                serverCorpses.Remove(c);
                return;
            }
            else { MainChat.SendErrorChat(p, "[错误] 您的库存满了."); return; }
        }

        [Command("setcorpse")]
        public static void COM_SetCorpseEmbed(PlayerModel p)
        {
            //Corpse c = serverCorpses.Find(x => p.Position.Distance(x.pos) < 10);
            Corpse c = serverCorpses.Where(x => p.Position.Distance(x.pos) < 10 && x.Dimension == p.Dimension).OrderBy(x => p.Position.Distance(x.pos)).FirstOrDefault();
            if (c == null) { MainChat.SendErrorChat(p, "[错误] 附近没有尸体."); return; }
            if (c.isEmbeb) { MainChat.SendErrorChat(p, "[错误] 这具尸体被埋了, 请先将尸体挖出(/emcorpse)."); return; }

            PropStreamer.GetProp(c.ID).Delete();
            c.ID = c.ID = PropStreamer.Create("v_ilev_body_parts", c.pos, new System.Numerics.Vector3(0, 0, 0), c.Dimension, false, true, true).Id;
            c.isEmbeb = true;
            c.EmbebName += " " + p.characterName.Replace("_", " ");
            MainChat.SendInfoChat(p, "[!] 已埋葬尸体.");
            return;
        }

        [Command("emcorpse")]
        public static void COM_SetCoprseDeEmbed(PlayerModel p)
        {
            //Corpse c = serverCorpses.Find(x => p.Position.Distance(x.pos) < 10);
            Corpse c = serverCorpses.Where(x => p.Position.Distance(x.pos) < 10 && x.Dimension == p.Dimension).OrderBy(x => p.Position.Distance(x.pos)).FirstOrDefault();
            if (c == null) { MainChat.SendErrorChat(p, "[错误] 附近没有尸体."); return; }
            if (!c.isEmbeb) { MainChat.SendErrorChat(p, "[错误] 此尸体没有被埋葬, 您可以使用 /setcorpse 埋葬."); return; }

            PropStreamer.GetProp(c.ID).Delete();
            c.ID = c.ID = PropStreamer.Create("xm_prop_x17_corpse_03", c.pos, new System.Numerics.Vector3(0, 0, 0), c.Dimension, false, true, true).Id;

            c.EmbebName += " " + p.characterName.Replace("_", " ");

            c.isEmbeb = false;

            MainChat.SendInfoChat(p, "[!] 已挖出尸体.");
            return;
        }

        [Command("getdna")]
        public async Task COM_GetDNA(PlayerModel p)
        {
            //Corpse c = serverCorpses.Find(x => p.Position.Distance(x.pos) < 10);
            Corpse c = serverCorpses.Where(x => p.Position.Distance(x.pos) < 10 && x.Dimension == p.Dimension).OrderBy(x => p.Position.Distance(x.pos)).FirstOrDefault();
            if (c == null) { MainChat.SendErrorChat(p, "[错误] 附近没有尸体."); return; }
            if (c.isEmbeb) { MainChat.SendErrorChat(p, "[错误] 这具尸体被埋了, 请先将尸体挖出(/emcorpse)."); return; }

            if (!await Globals.System.FD.CheckPlayerInFD(p)) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }

            MainChat.SendInfoChat(p, "<center>尸体上采集的DNA样本</center><br>" + c.EmbebName);
            return;
        }

        [Command("aremovecorpse")]
        public static void COM_DeleteCorpse(PlayerModel p)
        {
            if (p.adminLevel < 6) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            //Corpse c = serverCorpses.Find(x => p.Position.Distance(x.pos) < 10);
            Corpse c = serverCorpses.Where(x => p.Position.Distance(x.pos) < 10 && x.Dimension == p.Dimension).OrderBy(x => p.Position.Distance(x.pos)).FirstOrDefault();
            if (c == null) { MainChat.SendErrorChat(p, "[错误] 无效尸体."); return; }

            PropStreamer.GetProp(c.ID).Delete();
            serverCorpses.Remove(c);
            MainChat.SendInfoChat(p, "[!] 已删除尸体.");
            return;
        }

        [Command("acorpses")]
        public static void COM_ShowAllCorpses(PlayerModel p)
        {
            if (p.adminLevel < 5) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            foreach (Corpse c in serverCorpses)
            {
                p.SendChatMessage("ID: " + c.ID + " 姓名: " + c.Name + " 位置: " + JsonConvert.SerializeObject(c.pos));
            }
            return;
        }

        [Command("addcorpse")]
        public static void COM_AddCorpse(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 2) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /addcorpse [姓名]"); return; }

            Corpse c = new Corpse();
            c.ckDate = DateTime.Now;
            c.Name = string.Join(" ", args[0..]);
            c.Description = "x";
            c.Dimension = p.Dimension;
            Position cpos = p.Position;
            cpos.Z -= 1;
            c.pos = cpos;
            c.RemainingTime = 72;
            c.spectators = "无";
            c.ID = PropStreamer.Create("xm_prop_x17_corpse_03", c.pos, new System.Numerics.Vector3(0, 0, 0), c.Dimension, false, true, true).Id;
            serverCorpses.Add(c);
            MainChat.SendErrorChat(p, "[!] 已创建尸体.");
        }

        [Command("editcorpse")]
        public static void COM_EditCorpse(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 2) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /editcorpse [选项] [数值]<br>name[姓名] - desc[描述] - dna[DNA]"); return; }
            //Corpse c = serverCorpses.Find(x => p.Position.Distance(x.pos) < 4 && x.Dimension == p.Dimension);
            Corpse c = serverCorpses.Where(x => p.Position.Distance(x.pos) < 10 && x.Dimension == p.Dimension).OrderBy(x => p.Position.Distance(x.pos)).FirstOrDefault();
            if (c == null) { MainChat.SendErrorChat(p, "[HATA] Ceset bulunamadı."); return; }

            switch (args[0])
            {
                case "name":
                    c.Name = string.Join(" ", args[1..]);
                    break;

                case "desc":
                    c.Description = string.Join(" ", args[1..]);
                    break;

                case "dna":
                    c.EmbebName = string.Join(" ", args[1..]);
                    break;

                case "time":
                    if (!Int32.TryParse(args[1], out int remaining)) { MainChat.SendInfoChat(p, "[用法] /editcorpse [选项] [数值]<br>name[姓名] - desc[描述] - dna[DNA]"); return; }
                    c.RemainingTime = remaining;
                    break;

                case "timetoall":
                    CorpseUpdateRemaining();
                    break;



                default: MainChat.SendInfoChat(p, "[用法] /editcorpse [选项] [数值]<br>name[姓名] - desc[描述] - dna[DNA]"); return;

            }

            MainChat.SendInfoChat(p, "[!] 已更新尸体 " + args[0] + " 的信息.");
            return;
        }

        [Command("acccs")]
        public static void COM_SetAllCK(PlayerModel p)
        {
            if (p.adminLevel < 2)
                return;

            CorpseUpdateRemaining();
            MainChat.SendErrorChat(p, "[OK]");
            return;
        }

/*        #region NC Events

        [Command("nc")]
        public static void COM_SendNcMenu(PlayerModel p, params string[] args)
        {
            if (p.adminLevel < 6) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /nc [id]"); return; }

            if (!Int32.TryParse(args[0], out int tSql)) { MainChat.SendInfoChat(p, "[用法] /nc [id]"); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (t == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家!"); return; }

            Inputs.SendButtonInput(t, "Karakterinize NC atmak istiyor musunuz?", "NC:Step1", "x");
        }

        [AsyncClientEvent("NC:Step1")]
        public void NC_Step1(PlayerModel p, bool selection, string trash)
        {
            if (selection)
            {
                Inputs.SendTypeInput(p, "Yeni karakter adınız?", "NC:Step2", "x");
                return;
            }
            else
            {
                MainChat.SendErrorChat(p, "[!] NC işlemini iptal ettiniz.");
                return;
            }
        }

        [AsyncClientEvent("NC:Step2")]
        public async Task NC_Step2(PlayerModel p, string newName, string trash)
        {
            newName = newName.Replace(" ", "_");
            if (await Database.DatabaseMain.CheckCharacterName(newName)) { Globals.GlobalEvents.notify(p, 3, "Karakter adı veritabanında kayıtlı!"); Inputs.SendTypeInput(p, "Yeni karakter adınız?", "NC:Step2", "x"); return; }
            p.characterName = newName;
            p.updateSql();
            Inputs.SendTypeInput(p, "Karakter yaşınız?", "NC:Step3", "x");
            return;
        }

        [AsyncClientEvent("NC:Step3")]
        public void NC_Step3(PlayerModel p, params string[] args)
        {
            if (!Int32.TryParse(args[0], out int newAge)) { Inputs.SendTypeInput(p, "Karakter yaşınız?", "NC:Step3", "x"); return; }
            p.characterAge = newAge;
            p.updateSql();
            Inputs.SendButtonInput(p, "Karakteri yeniden şekillendir?", "NC:Step4", "x");
            return;
        }

        [AsyncClientEvent("NC:Step4")]
        public void NC_Step4(PlayerModel p, bool selection, string tash)
        {
            if (selection)
            {
                p.EmitLocked("character:Redit", p.charComps);
                MainChat.SendInfoChat(p, "Karakter uyruğu, ikincil dili ve diğer şeyler için rapor atmalısınız.");
                return;
            }
            else
            {
                MainChat.SendInfoChat(p, "[!] NC işleminiz başarıyla tamamlandı.");
                MainChat.SendInfoChat(p, "Karakter uyruğu, ikincil dili ve diğer şeyler için rapor atmalısınız.");
                return;
            }
        }
        #endregion */
    }


}
