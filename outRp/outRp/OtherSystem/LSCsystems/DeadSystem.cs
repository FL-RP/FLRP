using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using outRp.Chat;
using outRp.Core;
using outRp.Globals;
using outRp.Models;
using System;
using System.Threading.Tasks;

namespace outRp.OtherSystem.LSCsystems
{
    public class DeadSystem : IScript
    {

        [AsyncScriptEvent(ScriptEventType.PlayerDead)]
        public async Task DeadEvent(PlayerModel p, IEntity killer, uint wep)
        {
            try
            {
                /*
               if(p.adminWork) 
               {
                   p.Spawn(p.Position, 100);
                   return;[
               }
               if (p.HasData("Dead")) { return; }
               if(killer is PlayerModel target)
               {
                   p.SetData("Dead", true);
                   Alt.Log(target.characterName + " -> " + p.characterName + " öldürdü. (Silah:" + wep + ")");
                   if(wep == (uint)WeaponModel.Fist)
                   {
                       GlobalEvents.SubTitle(p, "~r~Yaralandınız. ~w~60 saniye sonra tekrar ayağa kalkacaksınız.", 60);
                       GlobalEvents.UINotifiy(p, 2, "~r~Yaralandiniz", time: 60000);
                       p.Spawn(p.Position, 60000);
                       p.injured.Injured = true;
                       p.injured.isDead = true;
                       await Task.Delay(60000);

                       if (!p.Exists)
                           return;

                       p.DeleteData("Dead");
                       Globals.GlobalEvents.UpdateInjured(p);
                   }                     
                   else
                   {
                       GlobalEvents.SubTitle(p, "~r~Yaralandınız. ~w~60 saniye sonra tekrar ayağa kalkacaksınız.", 10);
                       Globals.GlobalEvents.UINotifiy(p, 2, "~r~Yaralandiniz", time: 10000);                        
                       p.injured.Injured = true;
                       p.injured.isDead = true;
                       await Task.Delay(60000);

                       if (!p.Exists)
                           return;

                       p.Spawn(p.Position, 100);
                       p.DeleteData("Dead");
                       Globals.GlobalEvents.UpdateInjured(p);
                       return;
                   }

               }
               else // Player kill haricindeki durumlaar
               {
                   p.Spawn(p.Position, 2000);
                   Globals.GlobalEvents.UpdateInjured(p);
                   return;
               }
               */
                if (p.ckWarWatching)
                {
                    foreach (PlayerModel t in Alt.GetAllPlayers())
                    {
                        if (t.admiCkWatching)
                        {
                            MainChat.SendInfoChat(t, "[CK提示] " + p.characterName.Replace('_', ' ') + " 死亡了 -> CK");
                        }
                    }
                }


                Prometheus.Dead_Event(1);
                if (p.HasData("Player:Dead"))
                {
                    var pos = p.Position;
                    pos.Z += 1;
                    p.Position = pos;
                    p.GetData("Player:Dead", out int ragTime);
                    //p.Spawn(p.Position, (uint)(ragTime * 1000));
                    p.Spawn(pos, 120000);
                    p.DeleteData("Player:Dead");


                    p.injured.Injured = true;
                    p.maxHp = 1000;
                    //p.hp = 200;
                    p.MaxArmor = 1000;
                    p.Armor = 0;


                    return;
                }
                else
                {
                    var pos = p.Position;
                    pos.Z += 1;
                    p.Spawn(pos, 120000);
                    p.maxHp = 1000;
                    //p.hp = 200;
                    return;
                }
                //p.Spawn(p.Position, 0);
            }
            catch (Exception e) { Alt.Log("[玩家死亡] 事件发生错误: " + e.Message); }
        }


        [AsyncScriptEvent(ScriptEventType.WeaponDamage)]
        public async Task<bool> OnPlayerDamage(PlayerModel p, PlayerModel t, uint weapon, ushort damage, Position shotOffSet, BodyPart bodyPart)
        {
            if (weapon == (uint)WeaponModel.Snowballs)
            {
                t.hp = t.hp;
                return true;
            }
            t.SetData("Danger", DateTime.Now.ToString());
            int dmg = 0;
            bool isArmor = false;
            int ragTimer = 180;
            switch (weapon)
            {
                case (uint)WeaponModel.StunGun:
                    GlobalEvents.ShowNotification(p, "~g~ 您用电击枪击中了 " + t.fakeName.Replace("_", " "), "提示", icon: t, blink: true);
                    GlobalEvents.ShowNotification(t, "~r~" + p.fakeName.Replace("_", " ") + " 使用电击枪击中了您.", "电击效果", "效果将持续~b~20秒", icon: p, blink: true);
                    Animations.PlayerAnimation(t, "lay2");
                    break;

                case (uint)WeaponModel.AntiqueCavalryDagger:
                    dmg = 10 + p.Strength + p.tempStrength;
                    ragTimer = 10 + p.Strength + p.tempStrength;
                    break;

                case (uint)WeaponModel.BaseballBat:
                    dmg = 20 + p.Strength + p.tempStrength;
                    ragTimer = 10 + p.Strength + p.tempStrength;
                    break;

                case (uint)WeaponModel.BattleAxe:
                    dmg = 25 + p.Strength + p.tempStrength;
                    ragTimer = 10 + p.Strength + p.tempStrength;
                    break;

                case (uint)WeaponModel.BrassKnuckles:
                    dmg = 15 + p.Strength + p.tempStrength;
                    ragTimer = 10 + p.Strength + p.tempStrength;
                    break;

                case (uint)WeaponModel.BrokenBottle:
                    dmg = 25 + p.Strength + p.tempStrength;
                    ragTimer = 10 + p.Strength + p.tempStrength;
                    break;

                case (uint)WeaponModel.Crowbar:
                    dmg = 20 + p.Strength + p.tempStrength;
                    ragTimer = 10 + p.Strength + p.tempStrength;
                    break;

                case (uint)WeaponModel.Fist:
                    dmg = p.Strength + p.tempStrength;
                    ragTimer = 10 + p.Strength + p.tempStrength;
                    break;

                case (uint)WeaponModel.Flashlight:
                    dmg = 10 + p.Strength + p.tempStrength;
                    ragTimer = 10 + p.Strength + p.tempStrength;
                    break;

                case (uint)WeaponModel.GolfClub:
                    dmg = 25 + p.Strength + p.tempStrength;
                    ragTimer = 10 + p.Strength + p.tempStrength;
                    break;

                case (uint)WeaponModel.Hammer:
                    dmg = 15 + p.Strength + p.tempStrength;
                    ragTimer = 10 + p.Strength + p.tempStrength;
                    break;

                case (uint)WeaponModel.Hatchet:
                    dmg = 25 + p.Strength + p.tempStrength;
                    ragTimer = 10 + p.Strength + p.tempStrength;
                    break;

                case (uint)WeaponModel.Knife:
                    dmg = 20 + p.Strength + p.tempStrength;
                    ragTimer = 10 + p.Strength + p.tempStrength;
                    break;

                case (uint)WeaponModel.Machete:
                    dmg = 25 + p.Strength + p.tempStrength;
                    ragTimer = 10 + p.Strength + p.tempStrength;
                    break;

                case (uint)WeaponModel.Nightstick:
                    dmg = 70 + p.Strength + p.tempStrength;
                    ragTimer = 10 + p.Strength + p.tempStrength;
                    break;

                case (uint)WeaponModel.PipeWrench:
                    dmg = 20 + p.Strength + p.tempStrength;
                    ragTimer = 10 + p.Strength + p.tempStrength;
                    break;

                case (uint)WeaponModel.PoolCue:
                    dmg = 10 + p.Strength + p.tempStrength;
                    ragTimer = 10 + p.Strength + p.tempStrength;
                    break;

                case (uint)WeaponModel.StoneHatchet:
                    dmg = 10 + p.Strength + p.tempStrength;
                    ragTimer = 10 + p.Strength + p.tempStrength;
                    break;

                case (uint)WeaponModel.Switchblade:
                    dmg = 15 + p.Strength + p.tempStrength;
                    ragTimer = 10 + p.Strength + p.tempStrength;
                    break;

                // Weapons V

                case (uint)WeaponModel.APPistol:
                    dmg = 50+200;
                    isArmor = true;
                    break;

                case (uint)WeaponModel.CombatPistol:
                    dmg = 78+200;
                    isArmor = true;
                    break;

                case (uint)WeaponModel.DoubleActionRevolver:
                    dmg = 120+200;
                    isArmor = true;
                    break;

                case (uint)WeaponModel.HeavyPistol:
                    dmg = 85+200;
                    isArmor = true;
                    break;

                case (uint)WeaponModel.HeavyRevolverMkII:
                case (uint)WeaponModel.HeavyRevolver:
                    dmg = 250+200;
                    isArmor = true;
                    break;

                case (uint)WeaponModel.MarksmanPistol:
                    dmg = 1;  // Paintball Gun
                    isArmor = true;
                    break;

                case (uint)WeaponModel.Pistol:
                case (uint)WeaponModel.CeramicPistol:
                    dmg = 63+200;
                    isArmor = true;
                    break;

                case (uint)WeaponModel.Pistol50:
                    dmg = 98+200;
                    isArmor = true;
                    break;

                case (uint)WeaponModel.PistolMkII:
                    dmg = 70+200;
                    isArmor = true;
                    break;

                case (uint)WeaponModel.SNSPistol:
                    dmg = 20+200;
                    isArmor = true;
                    break;

                case (uint)WeaponModel.SNSPistolMkII:
                    dmg = 70+200;
                    isArmor = true;
                    break;

                case (uint)WeaponModel.VintagePistol:
                    dmg = 85+200;
                    isArmor = true;
                    break;

                // Sub Machine
                case (uint)WeaponModel.CombatPDW:
                case (uint)WeaponModel.AssaultSMG:
                    dmg = 50+200;
                    isArmor = true;
                    break;

                case (uint)WeaponModel.MachinePistol:
                    dmg = 32+200;
                    isArmor = true;
                    break;

                case (uint)WeaponModel.MiniSMG:
                case (uint)WeaponModel.MicroSMG:
                case (uint)WeaponModel.SMG:
                case (uint)WeaponModel.SMGMkII:
                    dmg = 28+200;
                    isArmor = true;
                    break;

                // Shotguns
                case (uint)WeaponModel.PumpShotgun:
                    // GlobalEvents.UINotifiy(t, 7, "战术豆袋", "您被豆袋击中了", "您倒下了", time: 5000);
                    GlobalEvents.ShowNotification(p, "~g~ 您用豆袋击中了 " + t.fakeName.Replace("_", " "), "提示", icon: t, blink: true);
                    GlobalEvents.ShowNotification(t, "~r~" + p.fakeName.Replace("_", " ") + " 使用豆袋击中了您.", "豆袋效果", "效果将持续~b~20秒", icon: p, blink: true);
                    Animations.PlayerAnimation(t, new string[] { "sleep3" });
                    break;

                case (uint)WeaponModel.AssaultShotgun:
                case (uint)WeaponModel.BullpupShotgun:
                case (uint)WeaponModel.HeavyShotgun:
                    dmg = 15+200;
                    isArmor = true;
                    break;

                case (uint)WeaponModel.DoubleBarrelShotgun:
                case (uint)WeaponModel.PumpShotgunMkII:
                    dmg = 27+200;
                    isArmor = true;
                    break;

                case (uint)WeaponModel.Musket:
                    dmg = 90+200;
                    isArmor = true;
                    break;

                case (uint)WeaponModel.SawedOffShotgun:
                case (uint)WeaponModel.SweeperShotgun:
                    dmg = 5;
                    isArmor = true;
                    break;


                // Assault Rifle
                case (uint)WeaponModel.AdvancedRifle:
                case (uint)WeaponModel.AssaultRifle:
                case (uint)WeaponModel.AssaultRifleMkII:
                case (uint)WeaponModel.BullpupRifle:
                case (uint)WeaponModel.BullpupRifleMkII:
                case (uint)WeaponModel.CarbineRifle:
                case (uint)WeaponModel.CarbineRifleMkII:
                case (uint)WeaponModel.SpecialCarbine:
                case (uint)WeaponModel.SpecialCarbineMkII:
                    dmg = 55+200;
                    isArmor = true;
                    break;

                case (uint)WeaponModel.CompactRifle:
                    dmg = 40+200;
                    isArmor = true;
                    break;

                // Light MG
                case (uint)WeaponModel.CombatMG:
                case (uint)WeaponModel.CombatMGMkII:
                case (uint)WeaponModel.MG:
                    dmg = 70+200;
                    isArmor = true;
                    break;

                case (uint)WeaponModel.GusenbergSweeper:
                    dmg = 30+200;
                    isArmor = true;
                    break;

                //snipers
                case (uint)WeaponModel.HeavySniper:
                case (uint)WeaponModel.HeavySniperMkII:
                case (uint)WeaponModel.MarksmanRifle:
                case (uint)WeaponModel.MarksmanRifleMkII:
                case (uint)WeaponModel.SniperRifle:
                    dmg = 2000;
                    isArmor = true;
                    break;


                case (uint)WeaponModel.CompactGrenadeLauncher:
                case (uint)WeaponModel.BZGas:
                    dmg = 10;
                    break;


                // Banned Weapons
                case (uint)WeaponModel.GrenadeLauncher:
                case (uint)WeaponModel.FireworkLauncher:
                case (uint)WeaponModel.GrenadeLauncherSmoke:
                case (uint)WeaponModel.HomingLauncher:
                case (uint)WeaponModel.Minigun:
                case (uint)WeaponModel.Railgun:
                case (uint)WeaponModel.RPG:
                case (uint)WeaponModel.Widowmaker:
                case (uint)WeaponModel.Flare:
                case (uint)WeaponModel.Grenade:
                case (uint)WeaponModel.PipeBombs:
                case (uint)WeaponModel.ProximityMines:
                case (uint)WeaponModel.StickyBomb:
                case (uint)WeaponModel.UnholyHellbringer:
                case (uint)WeaponModel.Snowballs:
                    dmg = 0;
                    break;

                default: dmg = damage; ragTimer = 80; break;
            }
            Logger.WriteLogData(Logger.logTypes.DamageLog, " (" + p.characterName + ") -> (" + t.characterName + ") | W: " +
                ((WeaponModel)weapon).ToString() + " | DMG: " + dmg.ToString() + " | PART: " + bodyPart.ToString() +
                " | HP: " + t.hp + " -> " + (t.hp - dmg));

            if (p.HasData("inPaintball"))
            {
                t.maxHp = 2000;
            }
            else
                t.maxHp = 1000;


            DamageCalc(t, dmg, isArmor);

            if (t.hp <= 400)
            {
                switch (bodyPart)
                {
                    case BodyPart.Chest:
                    case BodyPart.LowerTorso:
                    case BodyPart.UpperTorso:
                        t.injured.torso = true;
                        break;

                    case BodyPart.Head:
                    case BodyPart.Neck:
                    case BodyPart.UnderNeck:
                        t.injured.head = true;
                        break;

                    case BodyPart.LeftElbow:
                    case BodyPart.LeftShoulder:
                    case BodyPart.LeftUpperArm:
                    case BodyPart.LeftWrist:
                    case BodyPart.RightElbow:
                    case BodyPart.RightShoulder:
                    case BodyPart.RightUpperArm:
                    case BodyPart.RightWrist:
                        t.injured.arms = true;
                        break;

                    case BodyPart.LeftFoot:
                    case BodyPart.LeftHip:
                    case BodyPart.LeftLeg:
                    case BodyPart.RightFoot:
                    case BodyPart.RightHip:
                    case BodyPart.RightLeg:
                        t.injured.legs = true;
                        break;

                    default: break;
                }
            }


            if (t.hp <= 100)
            {
                //if (t.Vehicle != null) { t.Position = t.Vehicle.Position; }

                //t.EmitAsync("Damage:Stungun", ragTimer * 2);
                if (!t.HasData("Player:Dead"))
                {
                    t.SetData("Player:Dead", 120); // Rag Timer ' eklenecek.
                    t.hp = 99;
                }
            }
            else if (t.hp > 100 && t.hp <= 300)
            {
                //if (t.Vehicle != null) { t.Position = t.Vehicle.Position; }
                t.injured.Injured = true;
                //Animations.PlayerAnimation(t, "sleep3");
                //t.EmitAsync("Damage:Stungun", (ragTimer / 2), false);
            }

            //if (p.HasData("inPaintball"))
            //{
                //await Paintball.Player_Damage(t, p);
            //}
            //else
            //{
            GlobalEvents.UpdateInjured(t);
            //}


            if (t.hp < 0)
            {
                t.hp = 90; // Nefatif durum check ! 17.10.2021
            }

            return true;


        }
        /*
            public class InjuredModel
                {
                    public bool Injured { get; set; } = false;
                    public bool head { get; set; } = false;
                    public bool torso { get; set; } = false;
                    public bool arms { get; set; } = false;
                    public bool legs { get; set; } = false;
                    public bool isDead { get; set; } = false;
                    public List<DiseaseModel> diseases { get; set; } = new List<DiseaseModel>();

                }
         */

        
        public static void DamageCalc(PlayerModel p, int dmg, bool useArmor)
        {
            int damage = dmg * 2;
            if (useArmor)
            {
                if (p.Armor > 0)
                {
                    int _damage = damage / 10;
                    //Alt.Log("Armor 0'dan büyük");
                    if (p.Armor < (ushort)_damage)
                    {
                        // Alt.Log("Armor damageden küçük");
                        damage -= p.Armor * 10;
                        p.Armor = 0;
                    }
                    else
                    {
                        //Alt.Log("Armor damageden büyük");
                        int current = p.Armor - _damage;
                        damage = 0;
                        p.Armor = ((ushort)current);
                    }
                }

                if (damage > 0)
                {
                    //Alt.Log("Damage 0'dan büyük HP'den siliyorum.");
                    p.hp -= damage;
                }
            }
            else
            {
                //Alt.Log("Direk HP DAMAGESI");
                p.hp -= damage;
            }

            //if(p.hp <= 199) { p.hp = 199; }
            // TODO: Hp şunun altındaysa şu olsun.
            // TODO: Injured yazısını çağır.
        }

    }
}
