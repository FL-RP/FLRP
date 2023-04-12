using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Models;
using outRp.OtherSystem.Textlabels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace outRp.Globals
{
    public class GlobalEvents
    {
        public class blipModel
        {
            public int blipId { get; set; }
            public string blipname { get; set; }
            public int category { get; set; } = 2;
            public Position position { get; set; }
            public bool route { get; set; } = false;
            public int sprite { get; set; } = 0;
            public int color { get; set; } = 0;
            public string label { get; set; } = "空";
            public bool Short { get; set; } = true;
            public int rcolor { get; set; } = 0;
            public bool cone { get; set; } = false;
            public int number { get; set; } = 0;
        }
        public class attachModel
        {
            public IEntity attach { get; set; } = null;
            public IEntity target { get; set; } = null;
            public int? bone { get; set; } = null;
            public Vector3 off { get; set; }
            public Vector3 rot { get; set; }
        }
        public static List<blipModel> serverBlips = new List<blipModel>();
        public static PlayerLabel GetTextLabel(string searchData, int value)
        {
            PlayerLabel outer = null;
            foreach (PlayerLabel label in TextLabelStreamer.GetAllDynamicTextLabels())
            {
                label.TryGetData(searchData, out int val);
                if (val == value) { outer = label; break; }
            }
            return outer;
        }
        public static PlayerLabel GetInteriorTextLabel(string searchData, Position value)
        {
            PlayerLabel outer = null;
            foreach (PlayerLabel label in TextLabelStreamer.GetAllDynamicTextLabels())
            {
                label.TryGetData(searchData, out Position val);
                if (val == value) { outer = label; break; }
            }
            return outer;
        }
        public static PlayerModel GetPlayerFromId(int ID)
        {
            PlayerModel c = null;

            foreach (PlayerModel player in Alt.GetAllPlayers())
            {
                if (player.Id == ID) { c = player; return c; }
            }
            return c;
        }
        public static PlayerModel? GetPlayerFromSqlID(int SQLID)
        {
            foreach (PlayerModel player in Alt.GetAllPlayers())
            {
                if (player.sqlID == SQLID)
                {
                    return player;
                }
            }
            return null;
        }

        public static PlayerModel GetNearestPlayer(PlayerModel x)
        {
            return (PlayerModel)Alt.GetAllPlayers().Where(y => y.Position.Distance(x.Position) < 2 && x.Dimension == y.Dimension && y != x).OrderBy(z => z.Position.Distance(x.Position)).FirstOrDefault();
        }


        #region ClientSide Events
        /*
        public static void loadServerBlipsOnConnect(PlayerModel player)
        {
            foreach(blipModel x in serverBlips)
            {
                CreateBlip(player, x.blipname, x.category, x.position, route: x.route, sprite: x.sprite, color: x.color, label: x.label, Short: x.Short, rcolor: x.rcolor, cone: x.cone, number: x.number);
            }
        }
        
        public static void loadDynamicBlipsOnConnect(PlayerModel p)
        {
            foreach (blipModel x in OtherSystem.LSCsystems.BlipSystem.dynamicBlips)
            {
                CreateBlip(p, x.blipname, x.category, x.position, route: x.route, sprite: x.sprite, color: x.color, label: x.label, Short: x.Short, rcolor: x.rcolor, cone: x.cone, number: x.number);
            }
        }*/
        /// <summary>
        /// Sabit Blip oluşturma
        /// </summary>
        /// <param name="blipName">Blip'e verilecek isim</param>
        /// <param name="category">1 = No Text on blip or Distance | 2 = Text on blip | 3 = No text, just distance | 4+ No Text on blip or distance</param>
        /// <param name="pos">Pozisyon</param>
        /// <param name="route">Haritada işaretleyip açar kapatır</param>
        /// <param name="sprite">blip Modeli</param>
        /// <param name="color">Renk ID  https://altmp.github.io/altv-typings/classes/_alt_client_.pointblip.html#category </param>
        /// <param name="label">blipte yazacak isim</param>
        /// <param name="display">Görünüm, Defaul : 2</param>
        /// <param name="short">Kısa mesafe Evet : True - Hayır : False</param>
        /// <param name="rcolor">İkinci renk</param>
        /// <param name="number">Blip üzerindeki sayı - Sayı istemiyorsanız boş bırakın.</param>
        /// <param name="time">Ne kadar süre görüneceği</param>
        public static void CreateBlip(PlayerModel player, string blipName, int category, Position pos, bool route = false, int sprite = 1, int color = 1, string label = "", bool Short = true, int rcolor = 5, bool cone = false, int? number = null, int? time = null)
        {
            player.EmitLocked("blip:Create", blipName, category, pos, route, sprite, Short, color, rcolor, number, cone, label, time);
            return;
        }

        public static void DestroyBlip(PlayerModel player, string blipName)
        {
            player.EmitLocked("blip:Destroy", blipName);
            return;
        }

        /// <summary>
        /// Objeye attachli blip
        /// </summary>
        /// <param name="entitiy">Blip'in bağlanacağı entity</param>
        /// <param name="blipName">Blip'e verilecek isim</param>
        /// <param name="category">1 = No Text on blip or Distance | 2 = Text on blip | 3 = No text, just distance | 4+ No Text on blip or distance</param>
        /// <param name="route">Haritada işaretleyip açar kapatır</param>
        /// <param name="sprite">blip Modeli</param>
        /// <param name="color">Renk ID  https://altmp.github.io/altv-typings/classes/_alt_client_.pointblip.html#category </param>
        /// <param name="label">blipte yazacak isim</param>
        /// <param name="display">Görünüm, Defaul : 2</param>
        /// <param name="short">Kısa mesafe Evet : True - Hayır : False</param>
        /// <param name="rcolor">İkinci renk</param>
        /// <param name="number">Blip üzerindeki sayı - Sayı istemiyorsanız boş bırakın.</param>
        /// <param name="time">Ne kadar süre görüneceği</param>
        public static void CreateEntityBlip(PlayerModel player, IEntity entitiy, string blipName, int category, bool route = false, int sprite = 1, int color = 1, string label = "", bool Short = false, int rcolor = 5, int? time = null)
        {
            player.EmitLocked("blip:EntityCreate", entitiy.Id, blipName, category, route, sprite, Short, color, rcolor, label, time);
            return;
        }
        #endregion
        /// <summary>
        /// Notify gösterir.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="type">0:Admin Uyarı | 1: Mail-Mektup vs. | 2: Succes | 3: False| 4: Makbuz- Ödeme | 5: SOS | 6: Coin Para | 7: Yıldırım </param>
        /// <param name="text"></param>
        public static void notify(PlayerModel p, int type, string text)
        {
            string t = "";
            switch (type)
            {
                case 0:
                    t = "adm";
                    break;
                case 1:
                    t = "msg";
                    break;
                case 2:
                    t = "true";
                    break;
                case 3:
                    t = "false";
                    break;
                case 4:
                    t = "bill";
                    break;
                case 5:
                    t = "dispatch";
                    break;
                case 6:
                    t = "money";
                    break;
                case 7:
                    t = "light";
                    break;
            }
            string[] data = new string[2] { t, text };
            p.EmitLocked("showNotify", data);
            return;
        }

        public static void ProgresBar(PlayerModel p, string text, int time, string color = null)
        {
            p.EmitLocked("StartProgressBar", text, time, color);
            return;
        }
        /// <summary>
        /// Oyuncunun kontrolünü açar veya  kapatır(False hareket edemez / true hareket edebilir).
        /// </summary>
        public static void GameControls(PlayerModel p, bool status)
        {
            p.EmitLocked("Player:GameControls", status);
            return;
        }

        public static void PlayAnimation(PlayerModel p, string[] anim, int flag = 53, int duration = -1)
        {
            p.EmitLocked("Animation:Play", anim[0], anim[1], flag, duration);
            return;
        }
        public static void StopAnimation(PlayerModel p)
        {
            p.EmitLocked("Animation:Stop");
            return;
        }

        public static void AttachEntityToEntity(PlayerModel p, attachModel x)
        {

            p.EmitLocked("Entity:Attach", x.attach, x.target);
        }
        public static void DetachEntity(IEntity entity)
        {
            Alt.EmitAllClients("Entity:Detach", entity);
        }

        /// <summary>
        /// Oyuncunun kıyafet değişikliklerini durdurur.
        /// </summary>
        /// <param name="p">Oyuncu</param>
        /// <param name="status">true: kıyafet değişimi açık (Kıyafetleri yeniler) | false: kıyafet değişimini dondurur.</param>
        public static async Task FreezePlayerClothes(PlayerModel p, bool status)
        {
            switch (status)
            {
                case true:
                    if (p.HasSyncedMetaData(EntityData.PlayerEntityData.FreezeClothes)) { p.DeleteSyncedMetaData(EntityData.PlayerEntityData.FreezeClothes); }
                    List<ClothingModel> x = new List<ClothingModel>();
                    if (p.sex == 0)
                    {
                        x.Add(new ClothingModel() { cID = 1, iID = 0, tID = 0 });
                        //x.Add(new ClothingModel() { cID = 3, iID = 15, tID = 0 });
                        x.Add(new ClothingModel() { cID = 4, iID = 19, tID = 2 });
                        x.Add(new ClothingModel() { cID = 5, iID = 0, tID = 0 });
                        x.Add(new ClothingModel() { cID = 6, iID = 35, tID = 0 });
                        x.Add(new ClothingModel() { cID = 7, iID = 0, tID = 0 });
                        x.Add(new ClothingModel() { cID = 8, iID = 7, tID = 0 });
                        x.Add(new ClothingModel() { cID = 9, iID = 0, tID = 0 });
                        x.Add(new ClothingModel() { cID = 10, iID = 0, tID = 0 });
                        x.Add(new ClothingModel() { cID = 11, iID = 82, tID = 0 });
                    }
                    else
                    {
                        x.Add(new ClothingModel() { cID = 1, iID = 0, tID = 0 });
                        //x.Add(new ClothingModel() { cID = 3, iID = 15, tID = 0 });
                        x.Add(new ClothingModel() { cID = 4, iID = 61, tID = 1 });
                        x.Add(new ClothingModel() { cID = 5, iID = 0, tID = 0 });
                        x.Add(new ClothingModel() { cID = 6, iID = 34, tID = 0 });
                        x.Add(new ClothingModel() { cID = 7, iID = 0, tID = 0 });
                        x.Add(new ClothingModel() { cID = 8, iID = 15, tID = 0 });
                        x.Add(new ClothingModel() { cID = 9, iID = 0, tID = 0 });
                        x.Add(new ClothingModel() { cID = 10, iID = 0, tID = 0 });
                        x.Add(new ClothingModel() { cID = 11, iID = 91, tID = 0 });
                    }
                    SetClothSet(p, x);
                    await OtherSystem.Inventory.UpdatePlayerInventory(p);
                    break;

                case false:
                    p.SetSyncedMetaData(EntityData.PlayerEntityData.FreezeClothes, status);
                    break;
            }
        }

        public static void NativeNotify(PlayerModel p, string text)
        {
            p.EmitLocked("Text:ShowPlayer", text);
        }
        public static void NativeNotifyAll(PlayerModel targetPlayer, string text, int time = 3)
        {
            Alt.EmitAllClients("Text:ShowAll", targetPlayer, text, time);
        }
        public static void NativeNotifyVehicle(VehModel targetVehicle, string text)
        {
            Alt.EmitAllClients("Text:ShowAllVehicle", targetVehicle, text);
        }

        public static void CheckpointCreate(PlayerModel p, Position pos, int cType, int radius, Rgba color, string trigger, string CallBackValue)
        {
            p.EmitLocked("Checkpoint:Create", pos, cType, radius, (int)color.R, (int)color.G, (int)color.B, (int)color.A, trigger, CallBackValue);
        }

        public static void SetClothes(PlayerModel p, int componentID, int ItemID, int TextureID)
        {
            p.EmitLocked("SetClothes", componentID, ItemID, TextureID);
        }
        public class ClothingModel
        {
            public int cID { get; set; }
            public int iID { get; set; }
            public int tID { get; set; }
        }
        public static void SetClothSet(PlayerModel p, List<ClothingModel> cModel)
        {
            string json = JsonConvert.SerializeObject(cModel);
            p.EmitLocked("SetClothSet", json);
            /*foreach(var c in cModel)
            {
                p.SetClothes((byte)c.cID, (ushort)c.iID, (byte)c.tID, 2);
            }*/
        }

        /// <summary>
        /// oyuncu arabaya binince client -> server'a aracı ve triger'i yollar.
        /// dinleyici: Playermodel p, vehmodel v, string value
        /// </summary>
        /// <param name="p"></param>
        /// <param name="triger">geri çağırılacak clientevent</param>
        /// <param name="ErrorTriger">kişi belirtilen süre içerisinde araca binemediyse çağırılacak trigger.</param>
        /// <param name="value">varsa value</param>
        /// <param name="time"> süre -> 1sn, 2sn vs. not milisecond.</param>
        public static void VehicleWaitInterval(PlayerModel p, string triger, string ErrorTriger, string value = null, int time = 60)
        {
            p.EmitLocked("Player:WaitToEnterVehicle", triger, ErrorTriger, value, time);
        }

        /// <summary>
        /// public static void (Playermodel p, Vehmodel v)
        /// eğer oyuncu araçta değilse Vehmodel null gelir.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="triger"></param>
        public static void CheckVehicleInterval(PlayerModel p, string triger)
        {
            p.EmitLocked("Player:ExitVehicleSpector", triger);
        }

        /// <summary>
        /// Clientside oyuncuyu takip etmeye başlar oyuncu araçtan inince triger'e geri dönüş yapar.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="triger"></param>
        public static void PlayerExitVehicleWatcher(PlayerModel p, string triger)
        {
            p.EmitLocked("Player:VehicleExitWatch", triger);
        }

        public static void ShowNotification(PlayerModel p, string message, string title = "", string subtitle = "", PlayerModel icon = null, int color = 0, bool blink = false)
        {
            p.EmitLocked("Notification:Show", message, title, subtitle, icon, color, blink);
        }

        public static void SetPlayerTag(PlayerModel p, string text, bool head = true)
        {
            Alt.Log("SetPlayerTag 成功 运行");
            if (head)
            {
                p.SetSyncedMetaData("NameTag1", text);
            }
            else
            {
                p.SetSyncedMetaData("NameTag2", text);
            }
        }
        public static void ClearPlayerTag(PlayerModel p, bool head = true)
        {
            if (head)
            {
                if (p.HasSyncedMetaData("NameTag1"))
                    p.DeleteSyncedMetaData("NameTag1");
            }
            else
            {
                if (p.HasSyncedMetaData("NameTag2"))
                    p.DeleteSyncedMetaData("NameTag2");
            }

        }

        public static void SetVehicleTag(VehModel v, string text, bool head = true)
        {
            if (head)
            {
                v.SetStreamSyncedMetaData("NameTag1", text);
            }
            else
            {
                v.SetStreamSyncedMetaData("NameTag2", text);
            }
        }
        public static void ClearVehicleTag(VehModel v, bool head = true)
        {
            if (head)
            {
                if (v.HasStreamSyncedMetaData("NameTag1"))
                    v.DeleteStreamSyncedMetaData("NameTag1");
            }
            else
            {
                if (v.HasStreamSyncedMetaData("NameTag2"))
                    v.DeleteStreamSyncedMetaData("NameTag2");
            }
        }

        /// <summary>
        /// Obje yerleştirme modunu açar.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="objectModel"></param>
        /// <param name="callBackTriger"></param>
        /// <example>Geri dönüş değeri -> callBackTriger'e döner -> string Vector3 Rot - string Vector3 Pos - string objectModel verir.</example>
        public static void ShowObjectPlacement(PlayerModel p, string objectModel, string callBackTriger)
        {
            p.EmitLocked("PlacingModule:setObject", callBackTriger, objectModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="type">1:missionPassed | 2:mplargemsg | 3: showold | 4: rankup | 5:simpleshar | 6: wasted | 7:weapon(hash lazım) | 8: progres | 9: shard | 10: midsize | 11:midsize large | 12: plane(uçak hash)</param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="info"></param>
        /// <param name="total"></param>
        /// <param name="current"></param>
        /// <param name="hash"></param>
        /// <param name="time"></param>
        public static void UINotifiy(PlayerModel p, int type, string title, string message = "", string info = "", int total = 1, int current = 0, uint hash = 0, int time = 5000)
        {
            p.EmitLocked("Scale:ShowBig", type, title, message, info, total, current, hash, time);
        }

        public static void UpdateInjured(PlayerModel p)
        {

            GetInjuredStatus(p);


            p.SetSyncedMetaData(EntityData.PlayerEntityData.injuredData, JsonConvert.SerializeObject(p.injured));

        }
        public static void GetInjuredStatus(PlayerModel p)
        {
            if (p.HasData("Player:Dead"))
                p.injured.isDead = true;
            else p.injured.isDead = false;

            string status = "";
            if (p.injured.Injured)
            {
                if (p.injured.isDead)
                {
                    status = "~r~[重伤]~n~";
                    if (p.injured.head)
                    {
                        status += "[头部受伤]";
                    }
                    if (p.injured.torso)
                    {
                        status += " [躯干受伤]";
                    }
                    if (p.injured.legs)
                    {
                        status += " [腿部受伤]";
                    }
                    if (p.injured.arms)
                    {
                        status += " [手臂受伤]";
                    }
                }

            }


            GlobalEvents.SetPlayerTag(p, status, true);
            return;


        }

        public static void FreezeEntity(PlayerModel p, bool status = false)
        {
            p.EmitLocked("Player:FreezePos", status);
        }

        public static void CreateCamera(PlayerModel p, Vector3 position, Vector3 rotation, int fov = 10)
        {
            p.EmitLocked("Camera:Open", position, rotation, fov);
        }
        public static void CloseCamera(PlayerModel p)
        {
            p.EmitLocked("Camera:Close");
        }

        public static void LookCamera(PlayerModel p, Position lookPos)
        {
            p.EmitLocked("Camera:LookAt", lookPos);
        }

        /// <summary>
        /// Duration direk saniye girilecek ms değil
        /// </summary>
        /// <param name="p"></param>
        /// <param name="text"></param>
        /// <param name="duration"></param>
        public static void SubTitle(PlayerModel p, string text, int duration)
        {
            p.EmitLocked("Show:Subtitle", text, duration);
        }

        /// <summary>
        /// Play sound directly sadece youtube ID
        /// </summary>
        /// <param name="p"></param>
        /// <param name="link"></param>
        public static void PlaySound(PlayerModel p, string link)
        {
            p.EmitLocked("Boom:PlaySoundDirectly", link);
        }

        public static void InvForceClose(PlayerModel p)
        {
            p.EmitLocked("Inv:ForceClose");
        }

        public static void ForceLeaveVehicle(PlayerModel p)
        {
            VehModel veh = (VehModel)p.Vehicle;
            p.EmitLocked("setPlayerOutVehicle", veh);
        }
        public static void ForceEnterVehicle(PlayerModel p, VehModel v)
        {
            p.EmitLocked("setPlayerToVehicle", v);
        }

        public static async Task SetPlayerOnline(PlayerModel p, bool status)
        {
            await Database.DatabaseMain.setOnlineStatus(p, status);
        }

        public static string getCountryName(string code)
        {
            string name = "";
            switch (code)
            {
                case "skr":
                    name = "韩国";
                    break;

                case "wkr":
                    name = "朝鲜";
                    break;

                case "gd":
                    name = "苏格兰";
                    break;
                case "ar":
                    name = "阿根廷";
                    break;

                case "am":
                    name = "亚美尼亚";
                    break;

                case "au":
                    name = "澳大利亚";
                    break;

                case "bs":
                    name = "巴哈马";
                    break;

                case "be":
                    name = "比利时";
                    break;

                case "ba":
                    name = "波黑";
                    break;

                case "br":
                    name = "巴西";
                    break;

                case "bg":
                    name = "保加利亚";
                    break;

                case "ca":
                    name = "加拿大";
                    break;

                case "cu":
                    name = "古巴";
                    break;

                case "cz":
                    name = "捷克";
                    break;

                case "dk":
                    name = "丹麦";
                    break;

                case "eg":
                    name = "埃及";
                    break;

                case "gb":
                    name = "英国";
                    break;

                case "fi":
                    name = "芬兰";
                    break;

                case "fr":
                    name = "法国";
                    break;

                case "de":
                    name = "德国";
                    break;

                case "in":
                    name = "印度";
                    break;

                case "il":
                    name = "以色列";
                    break;

                case "it":
                    name = "意大利";
                    break;

                case "jm":
                    name = "牙买加";
                    break;

                case "jp":
                    name = "日本";
                    break;

                case "mk":
                    name = "马其顿";
                    break;

                case "mx":
                    name = "墨西哥";
                    break;

                case "pl":
                    name = "波兰";
                    break;

                case "pt":
                    name = "葡萄牙";
                    break;

                case "ro":
                    name = "罗马尼亚";
                    break;

                case "ru":
                    name = "俄罗斯";
                    break;

                case "es":
                    name = "西班牙";
                    break;

                case "se":
                    name = "瑞典";
                    break;

                case "ug":
                    name = "乌干达";
                    break;

                case "ua":
                    name = "乌克兰";
                    break;

                case "ve":
                    name = "委内瑞拉";
                    break;

                case "vn":
                    name = "越南";
                    break;

                case "av": name = "阿尔巴尼亚"; break;
                case "at": name = "奥地利"; break;
                case "by": name = "白俄罗斯"; break;
                case "ee": name = "爱沙尼亚"; break;
                case "gl": name = "格陵兰岛"; break;
                case "nl": name = "荷兰"; break;
                case "hr": name = "克罗地亚"; break;
                case "lv": name = "拉脱维亚"; break;
                case "lt": name = "立陶宛"; break;
                case "lu": name = "卢森堡"; break;
                case "hu": name = "匈牙利"; break;
                case "mt": name = "马耳他"; break;
                case "md": name = "摩尔多瓦"; break;
                case "mc": name = "摩纳哥"; break;
                case "no": name = "挪威"; break;
                case "sm": name = "圣马里奥"; break;
                case "sk": name = "斯洛伐克"; break;
                case "si": name = "斯洛文尼亚"; break;
                case "sb": name = "塞尔维亚"; break;
                case "vk": name = "梵蒂冈"; break;
                case "gr": name = "希腊"; break;
                case "ie": name = "爱尔兰"; break;
                case "ch": name = "瑞士"; break;
                case "is": name = "冰岛"; break;
                case "kr": name = "韩国"; break;
                case "co": name = "哥伦比亚"; break;
                case "cr": name = "哥斯达黎加"; break;
                case "us": name = "美国"; break;

                default:
                    name = "美国";
                    break;
            }
            return name;
        }

/*        public static string GetSecondLang(string code)
        {
            string name = "";
            switch (code)
            {
                case "gd":
                    name = "Keltçe";
                    break;

                case "am":
                    name = "Ermenice";
                    break;

                case "ba":
                    name = "Sırpça";
                    break;

                case "bg":
                    name = "Bulgarca";
                    break;

                case "cn":
                    name = "Çince";
                    break;

                case "cz":
                    name = "Çekçe";
                    break;

                case "dk":
                    name = "Danca";
                    break;

                case "fi":
                    name = "Fince";
                    break;

                case "fr":
                    name = "Fransızca";
                    break;

                case "de":
                    name = "Almanca";
                    break;

                case "in":
                    name = "Hintçe";
                    break;

                case "il":
                    name = "İbranice";
                    break;

                case "it":
                    name = "İtalyanca";
                    break;

                case "jp":
                    name = "Japonca";
                    break;

                case "pt":
                    name = "Portekizce";
                    break;

                case "ru":
                    name = "Rusça";
                    break;

                case "es":
                    name = "İspanyolca";
                    break;

                case "av": name = "Arnavutça"; break;
                case "ee": name = "Estonca"; break;
                case "gl": name = "Grönlandca"; break;
                case "ar": name = "Arapça"; break;
                case "an": name = "Flemenkçe"; break;
                case "hr": name = "Hırvatça"; break;
                case "lv": name = "Letonca"; break;
                case "lt": name = "Litvanca"; break;
                case "lu": name = "Lüksemburgça"; break;
                case "hu": name = "Macarca"; break;
                case "mk": name = "Makedonca"; break;
                case "mt": name = "Maltaca"; break;
                case "md": name = "Rumence"; break;
                case "no": name = "Norveçce"; break;
                case "pl": name = "Lehçe"; break;
                case "sk": name = "Slovakça"; break;
                case "si": name = "Slovenkçe"; break;
                case "sb": name = "Sırpça"; break;
                case "gr": name = "Yunanca"; break;
                case "ie": name = "İrlandaca"; break;
                case "is": name = "İzlandaca"; break;
                case "kr": name = "Korece"; break;

                default:
                    name = "Yok";
                    break;
            }
            return name;
        }*/

        public static void CallZCoord(PlayerModel p, string callBack, Position pos)
        {
            p.EmitLocked("Server:CalculateCoordZ", callBack, pos);
        }

        public static void ChangeGraphicMode(PlayerModel p, int selection)
        {
            p.EmitLocked("Player:Graphics", selection);
        }

        public static void RepairVehicle(VehModel v)
        {
            //v.NetworkOwner.EmitLocked("Vehicle:Repair", v.Id);
            v.Repair();
            return;
        }

        public static int GetAdminCount()
        {
            int count = 0;
            foreach (PlayerModel p in Alt.GetAllPlayers())
            {
                if (p.adminLevel >= 4)
                    count++;
            }
            return count;
        }
        public static int GetHelperCount()
        {
            int count = 0;
            foreach (PlayerModel p in Alt.GetAllPlayers())
            {
                if (p.adminLevel > 2 && p.adminLevel < 5)
                    count++;
            }
            return count;
        }

        public static int GetSupportCount()
        {
            int count = 0;
            foreach (PlayerModel p in Alt.GetAllPlayers())
            {
                if (p.adminLevel > 0 && p.adminLevel < 3)
                    count++;
            }

            return count;
        }
        public static int GetPlayerCount()
        {
            return Alt.GetAllPlayers().Count;
        }

        public static void ShowLeftNotify(PlayerModel p, string message, bool flashing = false, int textColor = -1, int[] FlashColor = null)
        {
            p.EmitLocked("Noti:Show", message, flashing, textColor, FlashColor);
            return;
        }

        public static void ShowLeftNotifyPicture(PlayerModel p, string tittle, string sender, string message, string notifyPic, int iconType = 0, bool flashing = false,
            int textColor = -1, int bgColor = -1, int[] flashColor = null)
        {
            p.EmitLocked("Noti:ShowPicture", tittle, sender, message, notifyPic, iconType, flashing, textColor, bgColor, flashColor);
            return;
        }

        public static async Task<bool> AddAccountInfo(int tSql, string Title, string Body, DateTime Date)
        {
            PlayerModel p = GetPlayerFromSqlID(tSql);
            if (p != null)
            {
                AccountModel acc = await Database.DatabaseMain.getAccInfo(p.accountId);

                if (acc == null)
                    return false;

                acc.OtherData.Info.Add(new OtherData_Inner.Informations()
                {
                    Title = Title,
                    Body = Body,
                    Date = Date
                });

                await acc.Update();
                MainChat.SendInfoChat(p, "{2d90a9}[服务器] {FFFFFF}您有 {DCFF00}" + acc.OtherData.Info.Count + "{FFFFFF} 个待查看的通知. {3FFF00}/{FFFFFF}showinfos");
                return true;
            }
            else
            {
                PlayerModelInfo pi = await Database.DatabaseMain.getCharacterInfo(tSql);
                if (pi == null)
                    return false;

                AccountModel acc = await Database.DatabaseMain.getAccInfo(pi.accountId);

                if (acc == null)
                    return false;

                acc.OtherData.Info.Add(new OtherData_Inner.Informations()
                {
                    Title = Title,
                    Body = Body,
                    Date = Date
                });

                await acc.Update();
                return true;
            }
        }

        public static async Task<bool> AddAccountRefund(int tSql, string Title, string Body, DateTime Date, int Amount)
        {
            PlayerModel p = GetPlayerFromSqlID(tSql);
            if (p != null)
            {
                AccountModel acc = await Database.DatabaseMain.getAccInfo(p.accountId);

                if (acc == null)
                    return false;

                acc.OtherData.Refunds.Add(new OtherData_Inner.Refund()
                {
                    Title = Title,
                    Body = Body,
                    Date = Date,
                    Cash = Amount
                });
                await acc.Update();
                MainChat.SendInfoChat(p, "{2d90a9}[服务器] {FFFFFF}您有 {DCFF00}" + acc.OtherData.Refunds.Count + "{FFFFFF} 条待接受的补偿/退款, 输入 {3FFF00}/{FFFFFF}showrefunds 查看列表");
                return true;
            }
            else
            {
                PlayerModelInfo pi = await Database.DatabaseMain.getCharacterInfo(tSql);
                if (pi == null) return false;

                AccountModel acc = await Database.DatabaseMain.getAccInfo(pi.accountId);

                if (acc == null)
                    return false;

                acc.OtherData.Refunds.Add(new OtherData_Inner.Refund()
                {
                    Title = Title,
                    Body = Body,
                    Date = Date,
                    Cash = Amount
                });
                await acc.Update();
                return true;
            }
        }

        /// <summary>
        /// Işığı açar kapatır
        /// </summary>
        /// <param name="p"></param>
        /// <param name="state">true ise ışık açık, false ise ışık kapalı.</param>
        public static void SetLightState(PlayerModel p, bool state)
        {
            p.EmitLocked("Ligt:State", state);
        }
    }
}
