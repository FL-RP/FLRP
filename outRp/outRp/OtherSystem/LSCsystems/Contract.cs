using System;
using AltV.Net; using AltV.Net.Async;
using AltV.Net.Resources.Chat.Api;
using outRp.Models;
using outRp.OtherSystem.NativeUi;
using outRp.Chat;
using outRp.Globals;

namespace outRp.OtherSystem.LSCsystems
{
    public class Contract : IScript
    {
        [Command("contract")]
        public static void COM_MakeContract(PlayerModel p, params string[] args)
        {
            if (args.Length <= 1) { MainChat.SendErrorChat(p, "[用法] /contract [id] [标题(用_代替空格) [内容]"); return; }
            if (!Int32.TryParse(args[0], out int sqlID)) { MainChat.SendErrorChat(p, "[用法] /contract [id] [标题(用_代替空格) [内容]"); return; }

            PlayerModel t = GlobalEvents.GetPlayerFromSqlID(sqlID);
            if (t == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家!"); return; }
            if(t.Position.Distance(p.Position) > 5) { MainChat.SendErrorChat(p, "[错误] 您离指定玩家太远."); return; }

            MainChat.SendInfoChat(p, "已成功发送契约请求.<br>标题: " + args[0].Replace("_", " ") + "<br>内容: " + string.Join(" ", args[2..]), true);
            MainChat.SendInfoChat(t, "收到一份契约请求.<br>标题: " + args[0].Replace("_", " ") + "<br>内容: " + string.Join(" ", args[2..]), true);
            Inputs.SendButtonInput(t, "回应契约请求", "Contract:Response", p.sqlID.ToString() + "," + args[1] + "," + string.Join(" ", args[2..]));
            return;
        }

        [AsyncClientEvent("Contract:Response")]
        public void EVENT_Contract(PlayerModel p, bool selection, string _val)
        {
            string[] val = _val.Split(",");
            if (!Int32.TryParse(val[0], out int tSQL) || val.Length < 2)
                return;

            if (!selection)
            {
                MainChat.SendInfoChat(p, "[?] 您拒绝了契约请求.");

                PlayerModel t = GlobalEvents.GetPlayerFromSqlID(tSQL);
                if(t == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家!"); return; }
                MainChat.SendInfoChat(t, "[?] 契约请求被拒绝.");
                return;
            }
            else
            {
                ServerItems i = Items.LSCitems.Find(x => x.ID == 64);
                if (i == null)
                    return;

                PlayerModel tok = GlobalEvents.GetPlayerFromSqlID(tSQL);
                if (tok == null)
                    return;

                i.data = "0";
                i.data2 = "<center>" + val[1].Replace("_", " ") + "</center><br><br>" + string.Join(",", val[2..]) + "<br><br><left>" + tok.characterName.Replace("_", " ") + "</left><br><right>" + p.characterName.Replace("_", " ") + "</right><br><center>" + DateTime.Now.ToLongDateString() + "</center>";
                Inventory.AddInventoryItem(p, i, 1);
                Inventory.AddInventoryItem(tok, i, 1);
                MainChat.SendInfoChat(p, "[?] 契约签署成功, 每人一份契约文件!");
                MainChat.SendInfoChat(tok, "[?] 契约签署成功, 每人一份契约文件!");
                return;
            }
        }
    }
}
