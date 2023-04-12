using System;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using outRp.Core;
using outRp.Globals;
using outRp.Models;
using System.Linq;
using AltV.Net.Resources.Chat.Api;

namespace outRp.Chat
{
    public class MainChatSettings
    {
        public class ChatDistances
        {
            public const float NormalChatDistanceNear = 2f;
            public const float NormalChatDistanceMedium = 5f;
            public const float NormalChatDistanceFar = 11f;

            public const float WhisperChatDistance = 1.5f;

            public const float LowVoiceChatDistance = 2.5f;

            public const float ShoutChatDistance = 35f;

            public const float MegaphoneChatDistance = 50f;
        }
        public class ChatColors
        {
            public const string ErrorMessageColor = "{D20000}<i class='fas fa-times'></i> {F54949}";
            public const string InfoMessageColor = "{9BC69B}";

            public const string NormalChatColorNear = "{FFFFFF}";
            public const string NormalChatColorMedium = "{CCCCCC}";
            public const string NormalChatColorFar = "{B1B1B1}";

            public const string EmoteMeColor = "{C2A2DA}";
            public const string EmoteDoColor = "{C2A2DA}";

            public const string FactionChatColor = "{00CEE7}";
            public const string DepartmentChatColor = "{82d6f5}";

            public const string PDRadioChatColor = "{fce78a}";

            public const string MegaphoneColor = "{FFF400}";
        }
    }

    public class MainChat
    {
        #region System Messages
        /*public static void SendErrorChat(IPlayer player, string errorMsg, string messageTitle = "[HATA]",  bool showIcon = true, string icon = "<i class='fas fa-times'></i>", string iconColor = "{D20000}", string messageColor = "{F54949}")
        {
            if (!player.Exists)
                return;

            player.SendChatMessage( showIcon ? iconColor + icon : "" + messageColor + messageTitle + " " + errorMsg);
        }

        public static void SendInfoChat(IPlayer player, string errorMsg, string messageTitle = "[Bilgi]", bool showIcon = true, string icon = "<i class='fas fa-times'></i>", string iconColor = "{9BC69B}", string messageColor = "{9BC69B}")
        {
            if (!player.Exists)
                return;

            player.SendChatMessage( showIcon ? iconColor + icon : "" + messageColor + messageTitle + " " + errorMsg);
        }*/

        #endregion
        
        public static void SendErrorChat(IPlayer player, string message)
        {
            if (!player.Exists)
                return;
            
            player.SendChatMessage(MainChatSettings.ChatColors.ErrorMessageColor + message);
            //Quasar.NotifyStreamer.Create((PlayerModel)player, message, "negative", "negative", "blue-1", icon: "dangerous", HTML: true, Position: "bottom");
            return;


        }
        public static void SendInfoChat(IPlayer player, string message, bool inBox = false)
        {

            if (!player.Exists)
                return;

            if (inBox)
            {
                player.SendChatMessage("<div style='border - radius: 16px; background-color: #7771711c; padding: 15px; border: 1px solid #00000045;'>" +
                                       MainChatSettings.ChatColors.InfoMessageColor + message +
                                       "</div>");
            }
            else
            {
                player.SendChatMessage(MainChatSettings.ChatColors.InfoMessageColor + message);
            }
            return;
        }

        public static int getTotalintDimension(int dimension)
        {
            return Alt.GetAllPlayers().Where(x => x.Dimension == dimension).Count();
        }
        
        public static async void NormalChat(PlayerModel player, string message)
        {
            Alt.Log("NormalChat 以 " + player.fakeName + ", " + message + " | 调用了");
            try
            {
                if (!player.Exists)
                    return;
                if (player.HasData("News:Watching"))
                {
                    SendErrorChat(player, "[新闻] 现在的状态无法说话!");
                    return;
                }
                var allPlayers = Alt.GetAllPlayers();
                if (message.ToLower() == ":d") { EmoteMe(player, "笑了笑."); return; }
                else if (message.ToLower() == ":d") { EmoteMe(player, "笑了笑."); return; }
                else if (message == ":)") { EmoteMe(player, "微微一笑."); return; }
                else if (message == ";)") { EmoteMe(player, "眨了眨眼."); return; }
                else if (message == ":(") { EmoteMe(player, "撅了撅嘴."); return; }
                else if (message == "xD" || message == "xd") { EmoteMe(player, "咧嘴一笑."); return; }
                else if (message.ToLower() == ":P") { EmoteMe(player, "吐舌头."); return; }

                OtherSystem.Phone.PhoneMain.PhoneData Phone = OtherSystem.Phone.PhoneMain.getData(player.sqlID);
                if (Phone.inCall)
                {
                    OtherSystem.Phone.PhoneMain.phoneMessageEvent(player, Phone.callTargetID, message);
                    return;
                }

                if (player.isFinishTut == 12)
                {
                    player.isFinishTut = 13;
                    player.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                    SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}基本指令");
                    SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}嘿, 您成功做到了!");
                    SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}现在请您试试 {fc5e03}/me{FFFFFF} 指令!");
                    SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}此指令用以描述角色的第一人称动作!");
                    SendInfoChat(player, "{fc5e03}新手教程: {FFFFFF}例如用法, /me 从口袋掏出了一枚硬币");
                    // GlobalEvents.CheckpointCreate(player, TutorialMain.TutPos[1], 44, 1, new Rgba(255, 255, 255, 100), "Tutorial:Run", "11");
                }                   
                string accent = player.lscGetdata<string>("Player:Accent") ?? "";
                if (player.Vehicle != null)
                {
                    VehModel pVehicle = (VehModel)player.Vehicle;
                    if (pVehicle.window)
                    {
                        foreach (var target in allPlayers)
                        {
                            if (target.Position.Distance(player.Position) < MainChatSettings.ChatDistances.NormalChatDistanceNear && target.Dimension == player.Dimension)
                            {
                                target.SendChatMessage(MainChatSettings.ChatColors.NormalChatColorNear + player.fakeName.Replace("_", " ") + " 说: " + message);
                                target.EmitLocked("Chat:Bouble", player.Id, message);
                            }
                            else if (target.Position.Distance(player.Position) < MainChatSettings.ChatDistances.NormalChatDistanceMedium && target.Dimension == player.Dimension)
                            {
                                target.SendChatMessage(MainChatSettings.ChatColors.NormalChatColorMedium + player.fakeName.Replace("_", " ") + " 说: " + message);
                                target.EmitLocked("Chat:Bouble", player.Id, message);
                            }
                            else if (target.Position.Distance(player.Position) < MainChatSettings.ChatDistances.NormalChatDistanceFar && target.Dimension == player.Dimension)
                            {
                                target.SendChatMessage(MainChatSettings.ChatColors.NormalChatColorFar + player.fakeName.Replace("_", " ") + " 说: " + message);
                                target.EmitLocked("Chat:Bouble", player.Id, message);
                            }
                        }
                    }
                    else
                    {
                        foreach (var target in allPlayers)
                        {
                            if (target.Vehicle == player.Vehicle)
                            {
                                target.SendChatMessage(MainChatSettings.ChatColors.NormalChatColorNear + player.fakeName.Replace("_", " ") + " 说(车窗关闭): " + message); 
                                //player.SendChatMessage(MainChatSettings.ChatColors.NormalChatColorNear + player.fakeName.Replace("_", " ") + " 说(车窗关闭): " + message); 
                                target.EmitLocked("Chat:Bouble", player.Id, message);
                            }
                        }
                    }
                }
                else
                {
                    // Dimension check;
                    var biz = await Props.Business.getBusinessById(player.LastBusiness);                
                    if (player.Dimension != player.LastBusiness && biz.Item1 != null && getTotalintDimension(player.Dimension) > 5 && biz.Item1.settings.floatText)
                    {
                        foreach (var target in allPlayers)
                        {
                            if (target.Position.Distance(player.Position) < 1 && target.Dimension == player.Dimension)
                            {
                                target.SendChatMessage(MainChatSettings.ChatColors.NormalChatColorNear + player.fakeName.Replace("_", " ") + " 说: " + message);
                                target.EmitLocked("Chat:Bouble", player.Id, message);
                                //target.Emit("Chat:AddTag", player.Id, message);
                            }
                            else if (target.Position.Distance(player.Position) < 2.5f && target.Dimension == player.Dimension)
                            {
                                target.SendChatMessage(MainChatSettings.ChatColors.NormalChatColorMedium + player.fakeName.Replace("_", " ") + " 说: " + message);
                                target.EmitLocked("Chat:Bouble", player.Id, message);
                                //target.Emit("Chat:AddTag", player.Id, message);
                            }
                            else if (target.Position.Distance(player.Position) < 5 && target.Dimension == player.Dimension)
                            {
                                target.SendChatMessage(MainChatSettings.ChatColors.NormalChatColorFar + player.fakeName.Replace("_", " ") + " 说: " + message);
                                target.EmitLocked("Chat:Bouble", player.Id, message);
                                //target.Emit("Chat:AddTag", player.Id, message);
                            }
                        }
                    }
                    else
                    {
                        foreach (var target in allPlayers)
                        {
                            if (target.Position.Distance(player.Position) < MainChatSettings.ChatDistances.NormalChatDistanceNear && target.Dimension == player.Dimension)
                            {
                                target.SendChatMessage(MainChatSettings.ChatColors.NormalChatColorNear + player.fakeName.Replace("_", " ") + " 说: " + message);
                                target.EmitLocked("Chat:Bouble", player.Id, message);
                                //target.Emit("Chat:AddTag", player.Id, message);
                            }
                            else if (target.Position.Distance(player.Position) < MainChatSettings.ChatDistances.NormalChatDistanceMedium && target.Dimension == player.Dimension)
                            {
                                target.SendChatMessage(MainChatSettings.ChatColors.NormalChatColorMedium + player.fakeName.Replace("_", " ") + " 说: " + message);
                                target.EmitLocked("Chat:Bouble", player.Id, message);
                                //target.Emit("Chat:AddTag", player.Id, message);
                            }
                            else if (target.Position.Distance(player.Position) < MainChatSettings.ChatDistances.NormalChatDistanceFar && target.Dimension == player.Dimension)
                            {
                                target.SendChatMessage(MainChatSettings.ChatColors.NormalChatColorFar + player.fakeName.Replace("_", " ") + " 说: " + message);
                                target.EmitLocked("Chat:Bouble", player.Id, message);
                                //target.Emit("Chat:AddTag", player.Id, message);
                            }
                        }
                    }
                }

                if (player.HasData(EntityData.PlayerEntityData.PlayerInEmergencyCall_1))
                {
                    int eSelection = player.lscGetdata<int>(EntityData.PlayerEntityData.PlayerInEmergencyCall_1);
                    if (eSelection == 0)
                    {
                        switch (message)
                        {
                            case "警察":
                            case "pd":
                                player.SetData(EntityData.PlayerEntityData.PlayerInEmergencyCall_1, 1);
                                player.SendChatMessage( "{49A0CD}紧急调度员说 (通话): 请描述一下情况!");
                                return;
                            case "医生":
                            case "fd":
                                player.SetData(EntityData.PlayerEntityData.PlayerInEmergencyCall_1, 2);
                                player.SendChatMessage("{49A0CD}紧急调度员说 (通话): 请描述一下情况!");
                                return;
                        }
                    }
                    else
                    {
                        switch (eSelection)
                        {
                            case 1:
                                foreach (PlayerModel pd in Alt.GetAllPlayers())
                                {
                                    if (pd.HasData(EntityData.PlayerEntityData.PDDuty)) { pd.SendChatMessage("{49A0CD}[911] 收到新的紧急呼救."); }
                                }
                                player.DeleteData(EntityData.PlayerEntityData.PlayerInEmergencyCall_1);
                                player.SendChatMessage( "{49A0CD}紧急调度员说 (通话): 感谢您的来电, 我们已经通知相关单位.");
                                OtherSystem.LSCsystems.MDCEvents.MDC_CreateMDC_Call(player, message);
                                Logger.WriteLogData(Logger.logTypes.pdLog, "[POLICE]" + player.characterName + " -> " + message);
                                break;
                            case 2:
                                foreach (PlayerModel fd in Alt.GetAllPlayers())
                                {
                                    if (fd.HasData(EntityData.PlayerEntityData.FDDuty)) { fd.SendChatMessage("{e84577}[911] 收到新的紧急呼救.<br>来电者: " + player.characterName.Replace('_', ' ') + "<br>情况: " + message); }
                                }
                                Globals.System.FD.fdCalls ncall = new Globals.System.FD.fdCalls()
                                {
                                    pos = player.Position,
                                    tSql = player.sqlID,
                                };
                                Globals.System.FD.serverFDCalls.Add(ncall);
                                player.DeleteData(EntityData.PlayerEntityData.PlayerInEmergencyCall_1);
                                player.SendChatMessage( "{49A0CD}紧急调度员说 (通话): 感谢您的来电, 我们已经通知相关单位.");
                                Logger.WriteLogData(Logger.logTypes.pdLog, "[LSFD]" + player.characterName + " -> " + message);
                                break;
                        }
                    }
                }
                else
                {
                    try
                    {
                        Core.Logger.WriteLogData(Logger.logTypes.lelorLog, message);
                    }
                    catch { }
                }
            } 
            catch (Exception ex) {
                Alt.LogError(ex.StackTrace);
                Alt.LogError(ex.Message);
            }
        }
        public static async void EmoteMe(PlayerModel player, string message)
        {
            try
            {
                Core.Logger.WriteLogData(Logger.logTypes.lelorLog, "[ME]" + message);
            }
            catch { }
            if (!player.Exists)
                return;
            if (message.Contains('"')) { string[] args = message.Split('"'); args[1] = MainChatSettings.ChatColors.NormalChatColorNear + args[1] + MainChatSettings.ChatColors.EmoteMeColor; message = string.Join('"', args); }
            var allPlayers = Alt.GetAllPlayers();
            
            var biz = await Props.Business.getBusinessById(player.LastBusiness);                
            if (player.Dimension != player.LastBusiness && biz.Item1 != null && getTotalintDimension(player.Dimension) > 5 && biz.Item1.settings.floatText)//if (player.Dimension != 0 && getTotalintDimension(player.Dimension) > 5)
            {
                foreach (var target in allPlayers)
                {
                    if (target.Position.Distance(player.Position) < 7 && target.Dimension == player.Dimension)
                    {
                        target.SendChatMessage(MainChatSettings.ChatColors.EmoteMeColor + "* " + player.fakeName.Replace("_", " ") + " " + message);
                    }
                }
            }
            else
            {
                foreach (var target in allPlayers)
                {
                    if (target.Position.Distance(player.Position) < MainChatSettings.ChatDistances.NormalChatDistanceFar + 7 && target.Dimension == player.Dimension)
                    {
                        target.SendChatMessage(MainChatSettings.ChatColors.EmoteMeColor + "* " + player.fakeName.Replace("_", " ") + " " + message);
                    }
                }
            }
            return;
        }
        /// <summary>
        /// Mesajın içine karakter adı da eklenecek normal EMOTE'de isme gerek yok!
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="message"></param>
        /// <param name="dimension"></param>
        public static void EmoteMe(Position pos, string message, int dimension = 0, float distance = 18)
        {
            try
            {
                Core.Logger.WriteLogData(Logger.logTypes.lelorLog, "[ME]" + message);
            }
            catch { }
            if (message.Contains('"')) { string[] args = message.Split('"'); args[1] = MainChatSettings.ChatColors.NormalChatColorNear + args[1] + MainChatSettings.ChatColors.EmoteMeColor; message = string.Join('"', args); }
            var allPlayers = Alt.GetAllPlayers();
            foreach (var target in allPlayers)
            {
                if (target.Position.Distance(pos) < distance && target.Dimension == dimension)
                {
                    target.SendChatMessage(MainChatSettings.ChatColors.EmoteMeColor + "* " + message);
                }
            }

            return;
        }
        public static void EmoteMeLow(PlayerModel player, string message)
        {
            try
            {
                Core.Logger.WriteLogData(Logger.logTypes.lelorLog, "[MEL]" + message);
            }
            catch { }
            if (!player.Exists)
                return;
            if (message.Contains('"')) { string[] args = message.Split('"'); args[1] = MainChatSettings.ChatColors.NormalChatColorNear + args[1] + MainChatSettings.ChatColors.EmoteMeColor; message = string.Join('"', args); }
            var allPlayers = Alt.GetAllPlayers();

            foreach (var target in allPlayers)
            {
                if (target.Position.Distance(player.Position) < 4 && target.Dimension == player.Dimension)
                {
                    target.SendChatMessage(MainChatSettings.ChatColors.EmoteMeColor + "* " + player.fakeName.Replace("_", " ") + " " + message);
                }
            }

        }
        public static void EmoteDoLow(PlayerModel player, string message)
        {
            try
            {
                Core.Logger.WriteLogData(Logger.logTypes.lelorLog, "[DOL]" + message);
            }
            catch { }
            if (!player.Exists)
                return;
            var allPlayers = Alt.GetAllPlayers();

            foreach (var target in allPlayers)
            {
                if (target.Position.Distance(player.Position) < 4 && target.Dimension == player.Dimension)
                {
                    target.SendChatMessage(MainChatSettings.ChatColors.EmoteDoColor + message + " (( " + player.fakeName.Replace("_", " ") + " ))");
                }
            }


        }
        public static async void EmoteDo(PlayerModel player, string message)
        {
            try
            {
                Core.Logger.WriteLogData(Logger.logTypes.lelorLog, "[DO]" + message);
            }
            catch { }
            if (!player.Exists)
                return;
            var allPlayers = Alt.GetAllPlayers();

            var biz = await Props.Business.getBusinessById(player.LastBusiness);                
            if (player.Dimension != player.LastBusiness && biz.Item1 != null && getTotalintDimension(player.Dimension) > 5 && biz.Item1.settings.floatText) //if (player.Dimension != 0 && getTotalintDimension(player.Dimension) > 5)
            {
                foreach (var target in allPlayers)
                {
                    if (target.Position.Distance(player.Position) < 7 && target.Dimension == player.Dimension)
                    {
                        target.SendChatMessage(MainChatSettings.ChatColors.EmoteDoColor + message + " (( " + player.fakeName.Replace("_", " ") + " ))");
                    }
                }
            }
            else
            {
                foreach (var target in allPlayers)
                {
                    if (target.Position.Distance(player.Position) < MainChatSettings.ChatDistances.NormalChatDistanceFar + 7 && target.Dimension == player.Dimension)
                    {
                        target.SendChatMessage(MainChatSettings.ChatColors.EmoteDoColor + message + " (( " + player.fakeName.Replace("_", " ") + " ))");
                    }
                }
            }


            return;
        }
        public static void EmoteDo(Position pos, string message, int dimension = 0, float distance = 18)
        {
            try
            {
                Core.Logger.WriteLogData(Logger.logTypes.lelorLog, "[DO]" + message);
            }
            catch { }
            var allPlayers = Alt.GetAllPlayers();            
            
            if (getTotalintDimension(dimension) > 5)
            {
                foreach (var target in allPlayers)
                {
                    if (target.Position.Distance(pos) < distance / 2 && target.Dimension == dimension)
                    {
                        target.SendChatMessage(MainChatSettings.ChatColors.EmoteDoColor + message);
                    }
                }
            }
            else
            {
                foreach (var target in allPlayers)
                {
                    if (target.Position.Distance(pos) < distance && target.Dimension == dimension)
                    {
                        target.SendChatMessage(MainChatSettings.ChatColors.EmoteDoColor + message);
                    }
                }
            }


            return;
        }
        public static async void EmoteDoAlternative(PlayerModel player, string message)
        {
            try
            {
                Core.Logger.WriteLogData(Logger.logTypes.lelorLog, "[DOA]" + message);
            }
            catch { }
            if (!player.Exists)
                return;
            var allPlayers = Alt.GetAllPlayers();

            var biz = await Props.Business.getBusinessById(player.LastBusiness);                
            if (player.Dimension != player.LastBusiness && biz.Item1 != null && getTotalintDimension(player.Dimension) > 5 && biz.Item1.settings.floatText)
            //if (player.Dimension != 0 && getTotalintDimension(player.Dimension) > 5)
            {
                foreach (var target in allPlayers)
                {
                    if (target.Position.Distance(player.Position) < 7 && target.Dimension == player.Dimension)
                    {
                        target.SendChatMessage(MainChatSettings.ChatColors.EmoteDoColor + " (( " + message + " ))");
                    }
                }
            }
            else
            {
                foreach (var target in allPlayers)
                {
                    if (target.Position.Distance(player.Position) < MainChatSettings.ChatDistances.NormalChatDistanceFar + 7 && target.Dimension == player.Dimension)
                    {
                        target.SendChatMessage(MainChatSettings.ChatColors.EmoteDoColor + " (( " + message + " ))");
                    }
                }
            }


            return;
        }
        public static void FactionChat(PlayerModel player, string message)
        {
            if (!player.Exists)
                return;
            player.SendChatMessage( MainChatSettings.ChatColors.FactionChatColor + message);
            return;
        }
        public static void PDRadioChat(PlayerModel player, string message)
        {
            try
            {
                Core.Logger.WriteLogData(Logger.logTypes.lelorLog, "[RPD]" + message);
            }
            catch { }
            if (!player.Exists)
                return;
            player.SendChatMessage( MainChatSettings.ChatColors.PDRadioChatColor + message);

            return;
        }
        public static void DepartmentChat(IPlayer player, string message)
        {
            try
            {
                Core.Logger.WriteLogData(Logger.logTypes.lelorLog, message);
            }
            catch { }
            if (!player.Exists)
                return;
            player.SendChatMessage( MainChatSettings.ChatColors.DepartmentChatColor + ChatIcons.DepartmentChatIcon + " " + message);
            return;
        }
        public static void ShoutChat(PlayerModel player, string message)
        {
            try
            {
                Core.Logger.WriteLogData(Logger.logTypes.lelorLog, message);
            }
            catch { }
            if (!player.Exists)
                return;

            var allPlayers = Alt.GetAllPlayers();


            foreach (var target in allPlayers)
            {
                if (target.Position.Distance(player.Position) < MainChatSettings.ChatDistances.ShoutChatDistance && target.Dimension == player.Dimension)
                {
                    target.SendChatMessage(player.fakeName.Replace("_", " ") + " 大喊道: " + message + "!");
                }
            }

            return;
        }
        public static void ShoutChat(string message, Position pos, int dimension = 0)
        {
            try
            {
                Core.Logger.WriteLogData(Logger.logTypes.lelorLog, message);
            }
            catch { }
            var allPlayers = Alt.GetAllPlayers();


            foreach (var target in allPlayers)
            {
                if (target.Position.Distance(pos) < MainChatSettings.ChatDistances.ShoutChatDistance && target.Dimension == dimension)
                {
                    target.SendChatMessage(message);
                }
            }

            return;
        }
        public static void LowVoiceChat(PlayerModel player, string message)
        {
            try
            {
                Core.Logger.WriteLogData(Logger.logTypes.lelorLog, message);
            }
            catch { }
            if (!player.Exists)
                return;
            var allPlayers = Alt.GetAllPlayers();
            if (player.Vehicle == null)
            {

                foreach (var target in allPlayers)
                {
                    if (target.Position.Distance(player.Position) < MainChatSettings.ChatDistances.LowVoiceChatDistance && target.Dimension == player.Dimension)
                    {
                        target.SendChatMessage(MainChatSettings.ChatColors.NormalChatColorNear + player.fakeName.Replace("_", " ") + " 小声地说: " + message);
                    }
                }
            }
            else
            {
                VehModel v = (VehModel)player.Vehicle;
                if (v.window)
                {
                    foreach (var target in allPlayers)
                    {
                        if (target.Position.Distance(player.Position) < MainChatSettings.ChatDistances.LowVoiceChatDistance && target.Dimension == player.Dimension)
                        {
                            target.SendChatMessage(MainChatSettings.ChatColors.NormalChatColorNear + player.fakeName.Replace("_", " ") + " 小声地说: " + message);
                        }
                    }

                }
                else
                {
                    foreach (var target in allPlayers)
                    {
                        if (target.Vehicle == player.Vehicle) { target.SendChatMessage(MainChatSettings.ChatColors.NormalChatColorNear + player.fakeName.Replace("_", " ") + " 小声地说 (车窗关闭): " + message); }
                    }
                }
            }
            return;
        }
        public static void WhisperChat(PlayerModel player, int id, string message)
        {
            if (!player.Exists)
                return;
            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(id);
            if (target == null || target.Position.Distance(player.Position) > 2) { SendErrorChat(player, "[错误] 您离指定玩家太远了."); return; }
            string[] targetName = target.fakeName.Split("_");
            AME(player, "~p~" + targetName[0] + " " + targetName[1] + " 在悄悄说着什么...");
            //EmoteMe(player, " adlı kişi, " + targetName[0] + " " + targetName[1] + " adlı kişinin kulağına yaklaşarak fısıldar.");
            target.SendChatMessage("{D7C402}" + player.fakeName.Replace("_", " ") + " 对你密语: " + message);
            player.SendChatMessage( "{D7C402}你对 " + player.fakeName.Replace("_", " ") + " 密语: " + message);
            return;
        }
        public static void PMChat(PlayerModel player, int id, string message)
        {
            if (!player.Exists)
                return;

            if (player.jailTime > 0 || player.adminJail > 0)
            {
                SendErrorChat(player, "[错误] 无法在监狱里发送OOC私信.");
                return;
            }
            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(id);

            if (target == null) { SendErrorChat(player, "[错误] 指定玩家不存在."); return; }
            if (target.isPM == false && player.adminWork == false) { SendErrorChat(player, "[错误] 指定玩家OOC私信未开启."); return; }

            target.SendChatMessage("{D4E464}[收到OOC私信] " + player.fakeName.Replace("_", " ") + "[" + player.sqlID + "]: " + message);
            player.SendChatMessage( "{A9A648}[OOC私信已发送] " + target.fakeName.Replace("_", " ") + "[" + target.sqlID + "]: " + message);
            player.LastPm = target.sqlID;
            return;
        }
        public static void Megaphone(PlayerModel player, string message)
        {
            try
            {
                Core.Logger.WriteLogData(Logger.logTypes.lelorLog, "[MP]" + message);
            }
            catch { }
            if (!player.Exists)
                return;
            EmoteMe(player, ServerEmotes.PD_MegaphoneEmote);
            string[] playerName = player.fakeName.Split("_");
            var allPlayers = Alt.GetAllPlayers();
            foreach (var target in allPlayers)
            {
                if (target.Position.Distance(player.Position) < MainChatSettings.ChatDistances.MegaphoneChatDistance && target.Dimension == player.Dimension)
                {
                    target.SendChatMessage(MainChatSettings.ChatColors.MegaphoneColor + player.fakeName.Replace("_", " ") + "[:o< " + message + " ]");
                }
            }

            return;
        }
        public static void InCarChat(PlayerModel player, string message)
        {
            if (!player.Exists)
                return;

            SendErrorChat(player, "[错误] 系统不活跃, 请稍后再试.");
            return;
            //if (!player.Exists)
            //    return;
            //if (player.Vehicle == null) { SendErrorChat(player, "[HATA] Bu komutu kullanabilmek için araç içerisinde olmalısınız."); return; }
            //IVehicle veh = player.Vehicle;
            //VehModel v = (VehModel)veh;

            //foreach (PlayerModel p in Alt.GetAllPlayers())
            //{
            //    if (p.Vehicle == v)
            //    {
            //        p.SendChatMsg(MainChatSettings.ChatColors.MegaphoneColor + "[Araç içi] " + player.fakeName.Replace("_", " ") + ": " + message);
            //    }
            //}

            return;
        }
        public static void SendAdminChat(string message)
        {
            message = "{C97E0A}" + message;
            foreach (PlayerModel x in Alt.GetAllPlayers())
            {
                if (x.adminLevel <= 4) { continue; }
                if (x.HasData(EntityData.PlayerEntityData.AdminChatClose)) { continue; }
                x.SendChatMessage(message);
            }

            return;
        }
        public static void SendTesterChat(string message)
        {
            message = "{7CB715}" + message;
            foreach (PlayerModel x in Alt.GetAllPlayers())
            {
                if (x.adminLevel <= 2) { continue; }
                if (x.HasData(EntityData.PlayerEntityData.TesterChatClose)) { continue; }
                x.SendChatMessage(message);
            }

            return;
        }

        public static void SendSupportchat(string message)
        {
            message = "{EECE1A}" + message;
            foreach (PlayerModel x in Alt.GetAllPlayers())
            {
                if (x.adminLevel <= 1) { continue; }
                if (x.HasData("HelperChatClosed")) { continue; }
                x.SendChatMessage(message);
            }

            return;
        }
        public static void OOCChat(PlayerModel player, string message)
        {
            if (!player.Exists)
                return;
            string color = "";
            if (player.adminWork && player.adminLevel > 4) { color = "{DB4040}"; }
            foreach (var target in Alt.GetAllPlayers())
            {
                if (target.Position.Distance(player.Position) < MainChatSettings.ChatDistances.NormalChatDistanceNear && target.Dimension == player.Dimension)
                {
                    target.SendChatMessage("((" + color + " OOC " + player.fakeName.Replace("_", " ") + ": {FFFFFF}" + MainChatSettings.ChatColors.NormalChatColorNear + message + " ))");
                }
                else if (target.Position.Distance(player.Position) < MainChatSettings.ChatDistances.NormalChatDistanceMedium && target.Dimension == player.Dimension)
                {
                    target.SendChatMessage("((" + color + " OOC " + player.fakeName.Replace("_", " ") + ": {FFFFFF}" + MainChatSettings.ChatColors.NormalChatColorMedium + message + " ))");
                }
                else if (target.Position.Distance(player.Position) < MainChatSettings.ChatDistances.NormalChatDistanceFar && target.Dimension == player.Dimension)
                {
                    target.SendChatMessage("((" + color + " OOC " + player.fakeName.Replace("_", " ") + ": {FFFFFF}" + MainChatSettings.ChatColors.NormalChatColorFar + message + " ))");
                }
            }
        }

        public static void SME(PlayerModel p, string message, int time)
        {
            foreach (PlayerModel x in Alt.GetAllPlayers())
            {
                if (x.Position.Distance(p.Position) < 50 && p.Dimension == x.Dimension)
                    x.EmitLocked("Chat:AddTag", p.Id, message, 194, 162, 218, 250, time);
            };
        }
        public static void AME(PlayerModel p, string message)
        {
            foreach (PlayerModel x in Alt.GetAllPlayers())
            {
                if (x.Position.Distance(p.Position) < 50 && p.Dimension == x.Dimension)
                    x.EmitLocked("Chat:AddTag", p.Id, message, 194, 162, 218, 250, 7);
            };
        }
        public static void SDO(PlayerModel p, string message, int time)
        {
            foreach (PlayerModel x in Alt.GetAllPlayers())
            {
                if (x.Position.Distance(p.Position) < 50 && p.Dimension == x.Dimension)
                    x.EmitLocked("Chat:AddTag", p.Id, message, 95, 161, 134, 250, time);
            };
        }
        public static void ADO(PlayerModel p, string message)
        {
            foreach (PlayerModel x in Alt.GetAllPlayers())
            {
                if (x.Position.Distance(p.Position) < 50 && p.Dimension == x.Dimension)
                    x.EmitLocked("Chat:AddTag", p.Id, message, 95, 161, 134, 250, 7);
            };
        }
    }
}
