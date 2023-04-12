using System;
using AltV.Net; using AltV.Net.Async;
using AltV.Net.Resources.Chat.Api;
using outRp.Models;
using outRp.Chat;
using Newtonsoft.Json;

namespace outRp.OtherSystem.LSCsystems
{

   public class gym : IScript
    {

       [Command("gym")]
        public static void gymUsable(PlayerModel p)
        {
            /*var set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            set.MuscleLevel = 5;
            p.settings = JsonConvert.SerializeObject(set);
            p.updateSql();...*/
            var set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            if(set.MuscleExp >= 50) { MainChat.SendErrorChat(p, "[错误] 您的肌肉经验值已经是最大值了, 无法继续健身了."); return; }
            if (set.MuscleUsable >= 4)
            {
                MainChat.SendErrorChat(p, "[错误] 一个小时内最多锻炼 4 次.");
                    return;
            }
            else { p.EmitLocked("GYM:WantToUse");  return; }
        }
        
        public static void gymTimer(PlayerModel p)
        {
            var set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            if (set.MuslceLast >= 6)
            {
                
                MainChat.SendInfoChat(p, "[健身] 肌肉经验值下降了, 因为您的角色很久没有健身了.");
                if (p.Strength > 30)
                {
                    p.Strength = p.Strength - 5;
                    set.MuscleExp = set.MuscleExp - 6;
                    p.settings = JsonConvert.SerializeObject(set);
                    p.updateSql();
                }
                return;
            }
            if (set.MuscleUsable >= 4)
            {
                set.MuslceLast++;
                set.MuscleUsable = 0;
                p.settings = JsonConvert.SerializeObject(set);
                p.updateSql();
            }
        }

        [AsyncClientEvent("GYM:CanUse")]
        public static void gymStart(PlayerModel p)
        {
            if (!p.HasData("Player:InGym")) { 
                p.EmitLocked("GYM:ShowGYM");
                p.SetData("Player:InGym", true);
            }
            else { MainChat.SendErrorChat(p, "[错误] 您正在健身中."); return; }
        }

        [AsyncClientEvent("GYM:NotNear")]
        public static void gymNotnear(PlayerModel p)
        {
            MainChat.SendErrorChat(p, "[错误] 附近没有运动器材. ");
        }

        [AsyncClientEvent("GYM:MSuccess")]
        public static void gymSuccess(PlayerModel p)
        {
            var set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            if(set.MuscleExp <= 50){
            if (p.HasData("Player:InGym"))
            {

                    if (set.MuscleExp >= 0 && set.MuscleExp <= 7)
                {
                    Random rand = new Random();
                    int Rnumber = rand.Next(10, 20);
                    if (Rnumber >= 11 && Rnumber <= 18)
                    {
                        set.MuscleExp++;
                        set.MuscleUsable++;
                        set.MuslceLast = 0;
                        MainChat.SendInfoChat(p, "[健身] 角色肌肉经验值提升了一点!" + " | " + "当前肌肉经验值: " + set.MuscleExp);
                        p.DeleteData("Player:InGym");
                        p.settings = JsonConvert.SerializeObject(set);
                        Animations.PlayerStopAnimation(p);
                        p.updateSql();
                            return;
                    }
                    else { MainChat.SendInfoChat(p, "[健身] 角色肌肉经验值没有增加."); p.DeleteData("Player:InGym");  return;}
                }else if(set.MuscleExp >= 8 && set.MuscleExp <= 14)
                {
                        if (set.MuscleExp == 8 && p.Strength <= 50) { p.Strength = 35; MainChat.SendInfoChat(p, "[健身] 角色身体因为健身已经达到了一个新的水平, 可以使用一些强壮的手臂了(/torso)."); }
                        Random rand = new Random();
                    int Rnumber = rand.Next(10, 30);
                    if (Rnumber >= 11 && Rnumber <= 20)
                    {
                            set.MuscleExp++;
                            set.MuscleUsable++;
                            set.MuslceLast = 0;
                            MainChat.SendInfoChat(p, "[健身] 角色肌肉经验值提升了一点.");
                            p.DeleteData("Player:InGym");
                            p.settings = JsonConvert.SerializeObject(set);
                            Animations.PlayerStopAnimation(p);
                            p.updateSql();
                            return;
                    }
                    else { MainChat.SendInfoChat(p, "[健身] 角色肌肉经验值没有增加."); p.DeleteData("Player:InGym");  return; }

                }else if(set.MuscleExp >= 15 && set.MuscleExp <= 29)
                {
                        if (set.MuscleExp == 15 && p.Strength <= 50) { p.Strength = 40; MainChat.SendInfoChat(p, "[健身] 角色身体因为健身已经达到了一个新的水平, 可以使用一些强壮的手臂了(/torso)."); }
                        Random rand = new Random();
                    int Rnumber = rand.Next(10, 40);
                    if (Rnumber >= 11 && Rnumber <= 25)
                    {
                            set.MuscleExp++;
                            set.MuscleUsable++;
                            set.MuslceLast = 0;
                            MainChat.SendInfoChat(p, "[健身] 角色肌肉经验值提升了一点." + " | " + "当前肌肉经验值: " + set.MuscleExp);
                            p.DeleteData("Player:InGym");
                            p.settings = JsonConvert.SerializeObject(set);
                            Animations.PlayerStopAnimation(p);
                            p.updateSql();
                            return;
                    }
                    else { MainChat.SendInfoChat(p, "[健身] 角色肌肉经验值没有增加."); p.DeleteData("Player:InGym");  return; }
                }else if(set.MuscleExp >= 30 && set.MuscleExp <= 49)
                {
                    if (set.MuscleExp == 30 && p.Strength <= 50) { p.Strength = 45;  MainChat.SendInfoChat(p, "[健身] 角色身体因为健身已经达到了一个新的水平, 可以使用一些强壮的手臂了(/torso)."); }
                    Random rand = new Random();
                    int Rnumber = rand.Next(10, 60);
                    if (Rnumber >= 11 && Rnumber <= 35)
                    {
                            set.MuscleExp++;
                            set.MuscleUsable++;
                            set.MuslceLast = 0;
                            MainChat.SendInfoChat(p, "[健身] 角色肌肉经验值提升了一点." + " | " + "当前肌肉经验值: " + set.MuscleExp);
                            p.DeleteData("Player:InGym");
                            p.settings = JsonConvert.SerializeObject(set);
                            Animations.PlayerStopAnimation(p);
                            p.updateSql();
                        return;
                    }
                    else { MainChat.SendInfoChat(p, "[健身] 角色肌肉经验值没有增加."); p.DeleteData("Player:InGym");  return; }
                }
            }

            }
            else
            {
                MainChat.SendErrorChat(p, "[错误] 角色肌肉经验值已经达到最大值了, 无法继续健身了."); return;
                p.DeleteData("Player:InGym");
            }

        }
        [AsyncClientEvent("GYM:Failed")]
        public static void restartVoid(PlayerModel p)
        {
            p.DeleteData("Player:InGym");
        }


    }
}
