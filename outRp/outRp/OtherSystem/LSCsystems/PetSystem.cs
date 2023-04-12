using AltV.Net;
using AltV.Net.Async;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Core;
using outRp.Globals;
using outRp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;
using static outRp.OtherSystem.Animations;

namespace outRp.OtherSystem.LSCsystems
{
    public class PetSystem : IScript
    {
        public static async void PetTimer()
        {
            System.Timers.Timer timer = new System.Timers.Timer(60000); // 60000 olarak düzelt.
            timer.Elapsed += OnMinuteSpend;
            timer.Start();
        }

        public static void OnMinuteSpend(Object sender, EventArgs e)
        {
            Vehicle.VehicleMain.Event_Vehicle_RentTimeControl();
            JackingNPC.NpcTimer();
        }
        /*public class PetModel
        {
            public ulong ID { get; set; }
            public int Owner { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            private Position _position { get; set; }
            public Position Position
            {
                get
                {
                    if (Ped != null)
                    {
                        return Ped.Position;
                    }
                    else
                        return _position;
                }
                set
                {
                    if (Ped != null)
                    {
                        Ped.Position = value;
                        _position = value;
                    }
                    else return;
                }
            }
            public Rotation Rotation { get; set; }
            public int Dimension { get; set; }
            public double Hunger { get; set; }
            public int Level { get; set; }
            public int Exp { get; set; }
            public DateTime EndDate { get; set; }
            public DateTime LastHunger { get; set; }
            public PedModel Ped
            {
                get
                {
                    var _ped = PedStreamer.Get(ID);
                    if (_ped != null)
                        return _ped;
                    else
                        return null;
                }
            }
        }*/

        public class PetShowModel
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public double Hunger { get; set; }
            public int Level { get; set; }
            public int Exp { get; set; }
            public DateTime EndDate { get; set; }
        }

        public class PetModel
        {
            public ulong ID { get; set; }
            public int Owner { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public DateTime EndDate { get; set; }
            public PedModel Ped => GetPet(this.ID);
        }
        public static PedModel GetPet(ulong id)
        {
            return PedStreamer.Get(id);

        }

        public class PetPrices
        {
            public static int Cat = 30;
            public static int Chop = 30;
            public static int Husky = 30;
            public static int Poodle = 50;
            public static int Pug = 45;
            public static int Retriever = 40;
            public static int Rottweiler = 30;
            public static int Shepherd = 40;
            public static int Westy = 50;
        }

        public static List<PetModel> pets = new List<PetModel>();

        [Command("petmenu")]
        public static void COM_ShowPetMenu(PlayerModel p)
        {
            p.EmitAsync("GUI:ToggleMenu", "petmenu");
        }

        [AsyncClientEvent("PetMenu:GetPets")]
        public void EVENT_GetPetList(PlayerModel p)
        {
            List<PetShowModel> pets = new();

            var set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            set.petPerm.ForEach(x =>
            {
                PetShowModel nPet = new()
                {
                    EndDate = x.DateTime,
                    Exp = 5,
                    Level = 10,
                    Hunger = 100.0,
                    Name = x.Name,
                    Type = x.Type
                };

                pets.Add(nPet);
            });
            //var pets = getOwnedPets(p);
            p.EmitAsync("PetMenu:PushPets", JsonConvert.SerializeObject(pets));
        }

        public List<PetModel> getOwnedPets(PlayerModel p)
        {
            List<PetModel> _pets = new();
            pets.FindAll(x => x.Owner == p.sqlID).ForEach(x => { _pets.Add(x); });
            return _pets;
        }

        [AsyncClientEvent("PetMenu:Buy")]
        public async Task EVENT_BuyPet(PlayerModel p, int type)
        {
            var petInfo = GetPetDataFromType(type);

            var account = await Database.DatabaseMain.getAccInfo(p.accountId);
            if (account.lscPoint < petInfo.Item2)
            {
                GuiNotification.Send(p, "您没有足够的点券.", "white", "negative", "red", progress: true);
                return;
            }

            account.lscPoint -= petInfo.Item2;
            await account.Update();

            var set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            var check = set.petPerm.Find(x => x.Type == petInfo.Item1);
            if (check == null)
            {
                PetPerm newPerm = new();
                newPerm.Type = petInfo.Item1;
                newPerm.DateTime = DateTime.Now.AddDays(30);

                set.petPerm.Add(newPerm);
                GuiNotification.Send(p, "成功购买宠物.", "green", timeOut: 1000);
            }
            else
            {
                if (check.DateTime < DateTime.Now)
                {
                    check.DateTime = DateTime.Now.AddDays(30);
                }
                else
                    check.DateTime = check.DateTime.AddDays(30);
                GuiNotification.Send(p, "成功购买 30天 的宠物.", "green", timeOut: 1000);
            }

            p.settings = JsonConvert.SerializeObject(set);
            p.updateSql();

            await p.EmitAsync("GUI:CloseMenu", 0);

            Core.Logger.WriteLogData(Logger.logTypes.Pet, p.characterName + " | " + petInfo.Item1 + " | " + petInfo.Item2 + "点券");
            return;
        }

        [Command("spawnpet")]
        public static void COM_CreatePet(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /spawnpet [类型]"); return; }
            var set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            if (set == null)
                return;

            var check = set.petPerm.Find(x => x.Type == args[0]);
            if (check == null) { MainChat.SendErrorChat(p, "[错误] 您没有此类的宠物."); return; }
            if (check.DateTime < DateTime.Now) { MainChat.SendErrorChat(p, "[错误] 此宠物已到期!"); return; }


            var check2 = pets.Find(x => x.Owner == p.sqlID && x.Type == args[0]);
            if (check2 != null) { MainChat.SendErrorChat(p, "[错误] 无效宠物!"); return; }

            var info = GetPetDataFromType(args[0]);

            PedModel pet = PedStreamer.Create(info.Item3, p.Position, p.Dimension, 100);
            pet.netOwner = p.Id;
            pet.hasNetOwner = true;
            pet.nametag = "[" + pet.Id + "] " + check.Name;

            PetModel newPet = new()
            {
                ID = pet.Id,
                Owner = p.sqlID,
                Name = check.Name,
                Type = check.Type,
                EndDate = check.DateTime,
            };

            pets.Add(newPet);

            GuiNotification.Send(p, "成功刷出宠物.", "green", timeOut: 1000);
            return;
        }

        [Command("removepet")]
        public static void COM_RemovePet(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /removepet [类型]"); return; }

            var check = pets.Find(x => x.Owner == p.sqlID && x.Type == args[0]);
            if (check == null) { MainChat.SendErrorChat(p, "[错误] 您没有刷出此类型的宠物!"); return; }

            check.Ped.Destroy();
            pets.Remove(check);

            GuiNotification.Send(p, "成功收回宠物.", "green", timeOut: 1000);
            return;
        }

        [Command("pet")]
        public static void COM_Pet(PlayerModel p, params string[] args)
        {
            if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /pet [类型] [选项(come跟随 | stop停止跟随 | name名称 | anim动作)] [数值(如果有)]"); return; }
            var check = pets.Find(x => x.Owner == p.sqlID && x.Type == args[0]);


            var set = JsonConvert.DeserializeObject<CharacterSettings>(p.settings);
            if (set == null)
                return;

            var check2 = set.petPerm.Find(x => x.Type == args[0]);
            if (check2 == null) { MainChat.SendErrorChat(p, "[错误] 无权使用此宠物."); return; }

            switch (args[1])
            {
                case "come":
                    if (check == null) { MainChat.SendErrorChat(p, "[错误] 您没有刷出的宠物."); return; }
                    if (args.Length < 2) { MainChat.SendInfoChat(p, "[用法] /pet [类型] come [跟随玩家ID]"); return; }
                    if (!Int32.TryParse(args[2], out int _followId)) { MainChat.SendInfoChat(p, "[用法] /pet [类型] come [跟随玩家ID]"); return; }
                    PlayerModel followTarget = GlobalEvents.GetPlayerFromSqlID(_followId);
                    if (followTarget == null) { MainChat.SendErrorChat(p, "[错误] 无效玩家."); return; }
                    check.Ped.followTarget = followTarget.Id;
                    GuiNotification.Send(p, check.Name + " 开始跟随 " + followTarget.fakeName.Replace('_', ' '), "white", "positive", "green", icon: "pets");

                    check.Ped.nametag = "[" + check.Ped.Id + "] " + check2.Name + "~n~~g~跟随: ~w~" + followTarget.fakeName.Replace('_', ' ');
                    return;

                case "stop":
                    if (check == null) { MainChat.SendErrorChat(p, "[错误] 您没有刷出的宠物."); return; }
                    check.Ped.followTarget = null;
                    check.Ped.netOwner = null;
                    GuiNotification.Send(p, check.Name + " 停止了跟随.", "white", "positive", "green", icon: "pets");

                    check.Ped.nametag = "[" + check.Ped.Id + "] " + check2.Name;


                    return;

                case "name":
                    check2.Name = String.Join(" ", args[2..]);
                    p.settings = JsonConvert.SerializeObject(set);
                    p.updateSql();
                    if (check != null)
                        check.Ped.nametag = "[" + check.Ped.Id + "] " + check2.Name;
                    MainChat.SendInfoChat(p, "[?] 已更新宠物名称.");
                    return;

                case "anim":
                    if (args.Length < 3) { MainChat.SendInfoChat(p, "[用法] /pet [类型] anim [动作字典] [动作名称]"); return; }
                    check.Ped.followTarget = null;
                    check.Ped.animation = new string[] { args[2], args[3] };

                    MainChat.SendInfoChat(p, "[?] 已设定宠物动作.");
                    return;

                case "anim2":
                    if (args.Length < 2) { MainChat.SendErrorChat(p, "[用法] /pet [类型] anim2 [动作名称]"); return; }
                    AnimModel anim = Anims.Where(x => x.Key == args[2]).FirstOrDefault().Value;
                    if (anim != null)
                    {
                        check.Ped.animation = new string[] { anim.dict, anim.anim };
                    }
                    else
                    {
                        check.Ped.animation = new string[] { "1", "2" };
                    }

                    MainChat.SendInfoChat(p, "[?] 已设定NPC动作.");
                    return;
            }
        }

        [Command("resetpets")]
        public static void COM_resetPets(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /resetpets [id]"); return; }
            if (p.adminLevel <= 3)
                return;

            if (!Int32.TryParse(args[0], out int tSql))
                return;

            var target = GlobalEvents.GetPlayerFromSqlID(tSql);
            if (target == null) { MainChat.SendErrorChat(p, "[错误] 无效宠物!"); return; }

            var tarSet = JsonConvert.DeserializeObject<CharacterSettings>(target.settings);
            tarSet.petPerm = new List<PetPerm>();
            target.settings = JsonConvert.SerializeObject(tarSet);
            MainChat.SendInfoChat(p, "[已重置宠物]");
            return;
        }

        [Command("addpet")]
        public void COM_AddAdminPet(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 5) { MainChat.SendErrorChat(p, "[错误] 无权操作!"); return; }
            if (args.Length < 2) { MainChat.SendInfoChat(p, "[用法] /apet [id] [类型] [时间(天)]"); return; }
            if (!Int32.TryParse(args[0], out int _tSql)) { return; }
            var target = GlobalEvents.GetPlayerFromSqlID(_tSql);
            if (target == null) { MainChat.SendErrorChat(p, "[错误] 无效宠物!"); return; }

            var set = JsonConvert.DeserializeObject<CharacterSettings>(target.settings);
            if (set == null)
                return;

            var systemCheck = GetPetDataFromType(args[1]);
            if (systemCheck.Item1 == null) { MainChat.SendErrorChat(p, "[错误] 无效类型."); return; }

            if (!Int32.TryParse(args[2], out int Day)) { return; }
            var check = set.petPerm.Find(x => x.Type == args[1]);
            if (check == null)
            {
                PetPerm nPerm = new()
                {
                    DateTime = DateTime.Now.AddDays(Day),
                    Name = "未取名字",
                    Type = args[1]
                };

                set.petPerm.Add(nPerm);
                MainChat.SendInfoChat(p, "[?] 您给予 " + target.characterName + " 了 " + Day + " 天 " + args[1] + " 的宠物.");
                MainChat.SendInfoChat(target, "[?] " + p.characterName.Replace('_', ' ') + " 给予了您 " + Day + " 天 " + args[1] + " 的宠物.");
            }
            else
            {
                check.DateTime = check.DateTime.AddDays(Day);
                MainChat.SendInfoChat(p, "[?] 您给予 " + target.characterName + " 了 " + args[1] + " 的宠物 +" + Day + " 天.");
                MainChat.SendInfoChat(target, "[?] " + p.characterName.Replace('_', ' ') + " 给予了您 " + args[1] + " 的宠物 +" + Day + " 天.");
            }

            target.settings = JsonConvert.SerializeObject(set);
            target.updateSql();
            return;

        }

        public static (string, int, string) GetPetDataFromType(int type)
        {
            switch (type)
            {
                case 0: return ("kedi", PetPrices.Cat, "a_c_cat_01");
                case 1: return ("chop", PetPrices.Chop, "a_c_chop");
                case 2: return ("husky", PetPrices.Husky, "a_c_husky");
                case 3: return ("poodle", PetPrices.Poodle, "a_c_poodle");
                case 4: return ("pug", PetPrices.Pug, "a_c_pug");
                case 5: return ("retriever", PetPrices.Retriever, "a_c_retriever");
                case 6: return ("rottweiler", PetPrices.Rottweiler, "a_c_rottweiler");
                case 7: return ("shepherd", PetPrices.Shepherd, "a_c_shepherd");
                case 8: return ("westy", PetPrices.Westy, "a_c_westy");
                case 9: return ("yabandomuzu", 1, "a_c_boar");
                case 10: return ("kartal", 1, "a_c_chickenhawk");
                case 11: return ("maymun", 1, "a_c_chimp");
                case 12: return ("inek", 1, "a_c_cow");
                case 13: return ("kurt", 1, "a_c_coyote");
                case 14: return ("karga", 1, "a_c_crow");
                case 15: return ("geyik", 1, "a_c_deer");
                case 16: return ("tavuk", 1, "a_c_hen");
                case 17: return ("kaplan", 1, "a_c_mtlion");
                case 18: return ("panter", 1, "a_c_panther");
                case 19: return ("domuz", 1, "a_c_pig");
                case 20: return ("tavşan", 1, "a_c_rabbit_01");
                case 21: return ("goril", 1, "cs_orleans");
                case 22: return ("guvenlik", 1, "s_m_m_security_01");
                case 23: return ("dedektif", 1, "cs_michelle");
                case 24: return ("guvenlik2", 1, "csb_vincent_2");
                case 25: return ("guvenlik3", 1, "ig_prolsec_02");
                case 26: return ("hizmetci", 1, "s_f_m_maid_01");

                default: return ("null", 0, "null");
            }
        }

        public static (string, int, string) GetPetDataFromType(string type)
        {
            switch (type)
            {
                case "kedi": return ("kedi", PetPrices.Cat, "a_c_cat_01");
                case "chop": return ("chop", PetPrices.Chop, "a_c_chop");
                case "husky": return ("husky", PetPrices.Husky, "a_c_husky");
                case "poodle": return ("poodle", PetPrices.Poodle, "a_c_poodle");
                case "pug": return ("pug", PetPrices.Pug, "a_c_pug");
                case "retriever": return ("retriever", PetPrices.Retriever, "a_c_retriever");
                case "rottweiler": return ("rottweiler", PetPrices.Rottweiler, "a_c_rottweiler");
                case "shepherd": return ("shepherd", PetPrices.Shepherd, "a_c_shepherd");
                case "westy": return ("westy", PetPrices.Westy, "a_c_westy");
                case "yabandomuzu": return ("yabandomuzu", 1, "a_c_boar");
                case "kartal": return ("kartal", 1, "a_c_chickenhawk");
                case "maymun": return ("maymun", 1, "a_c_chimp");
                case "inek": return ("inek", 1, "a_c_cow");
                case "kurt": return ("kurt", 1, "a_c_coyote");
                case "karga": return ("karga", 1, "a_c_crow");
                case "geyik": return ("geyik", 1, "a_c_deer");
                case "tavuk": return ("tavuk", 1, "a_c_hen");
                case "kaplan": return ("kaplan", 1, "a_c_mtlion");
                case "panter": return ("panter", 1, "a_c_panther");
                case "domuz": return ("domuz", 1, "a_c_pig");
                case "tavşan": return ("tavşan", 1, "a_c_rabbit_01");
                case "goril": return ("goril", 1, "cs_orleans");
                case "guvenlik": return ("guvenlik", 1, "s_m_m_security_01");
                case "dedektif": return ("dedektif", 1, "cs_michelle");
                case "guvenlik2": return ("guvenlik2", 1, "csb_vincent_2");
                case "guvenlik3": return ("guvenlik3", 1, "ig_prolsec_02");
                case "hizmetci": return ("hizmetci", 1, "s_f_m_maid_01");


                default: return ("null", 0, "null");
            }
        }

        [AsyncScriptEvent(ScriptEventType.PlayerDisconnect)]
        public async Task OnPetDisconnect(PlayerModel p, string reason)
        {
            List<PetModel> deletes = new();
            lock (pets)
            {
                var petler = pets.Where(x => x.Owner == p.sqlID);
                foreach (var pet in petler)
                {
                    pets.Remove(pet);
                    pet.Ped.Destroy();
                }
            }
        }

        /*
        public static List<PetModel> pets = new();


        public static void LoadAllPets(string data)
        {
            pets = JsonConvert.DeserializeObject<List<PetModel>>(data);
            foreach(var pet in pets)
            {
                var info = GetPetDataFromType(pet.Type);
                var ped = PedStreamer.Create(info.Item3, pet.Position, pet.Dimension, 100);
                pet.ID = ped.Id;
                ped.nametag = "[" + pet.ID + "]" + pet.Name;
                ped.hasNetOwner = true;
                pet.Position = pet.Position;
                ped.Position = pet.Position;
            }
        }

        [Command("petmenu")]
        public void COM_ShowPetMenu(PlayerModel p)
        {
            p.EmitAsync("GUI:ToggleMenu", "petmenu");
        }

        [AsyncClientEvent("PetMenu:GetPets")]
        public void EVENT_GetPetList(PlayerModel p)
        {
            var pets = getOwnedPets(p);
            p.EmitAsync("PetMenu:PushPets", JsonConvert.SerializeObject(pets));
        }

        [AsyncClientEvent("PetMenu:Buy")]
        public void EVENT_BuyPet(PlayerModel p, int type)
        {
            var petInfo = GetPetDataFromType(type);

            var account = Database.DatabaseMain.getAccInfo(p.accountId);
            if(account.lscPoint < petInfo.Item2)
            {
                GuiNotification.Send(p, "Yeterli LSC-P Bulunamadı.", "white", "negative", "red", progress: true);
                return;
            }

            account.lscPoint -= petInfo.Item2;
            account.Update();

            PetModel newPet = new();
            PedModel ped = PedStreamer.Create(petInfo.Item3, p.Position, p.Dimension, 100);
            newPet.ID = ped.Id;
            newPet.Dimension = p.Dimension;
            newPet.Level = 3;
            newPet.Exp = 4;
            newPet.Hunger = 100;
            newPet.LastHunger = DateTime.Now;
            newPet.Owner = p.sqlID;
            newPet.Position = p.Position;
            newPet.Rotation = p.Rotation;
            newPet.Name = "Evcil Hayvan";            
            newPet.Type = petInfo.Item1;
            newPet.EndDate = DateTime.Now.AddDays(30);

            pets.Add(newPet);

            ped.nametag = "[" + newPet.ID + "]" +newPet.Name;
            ped.hasNetOwner = true;

            p.EmitAsync("GUI:CloseMenu", 0);
            GuiNotification.Send(p, "Evcil hayvan başarıyla alındı.", "green", timeOut: 1000);
            Core.Logger.WriteLogData(Logger.logTypes.Pet, p.characterName + " | " + newPet.Type + " | " + petInfo.Item2 + "LSC-P");
            Database.DatabaseMain.SaveServerSettings();
            return;
        }

        #region PetCommands
        [Command("pet")]
        public void COM_Pet(PlayerModel p, params string[] args)
        {
            if (args.Length <= 1) { MainChat.SendInfoChat(p, "[用法] /pet [id] [Çeşit]"); return; }
            if(!Int32.TryParse(args[0], out int petID)) { MainChat.SendInfoChat(p, "[用法] /pet [id] [Çeşit]"); return; }

            PetModel curPet = getPedFromID((ulong)petID);
            PedModel ped = PedStreamer.Get(curPet.ID);
            if (curPet == null || ped == null) { MainChat.SendErrorChat(p, "[错误] Girilen ID'de bir hayvan bulunamadı."); return; }
            if(p.Position.Distance(ped.Position) > 30 && args[1] != "fix") { MainChat.SendErrorChat(p, "[错误] Evcil hayvana yeterince yakın değilsiniz."); return; }
            if(curPet.Owner != p.sqlID) { MainChat.SendErrorChat(p, "[错误] Bu hayvanın sahibi siz değilsiniz!"); return; }
            switch (args[1])
            {
                case "takip":
                    if (args.Length <= 2) { MainChat.SendInfoChat(p, "[用法] /pet " + curPet.ID + " takip [id]"); return; }
                    if(!Int32.TryParse(args[2], out int followTarget)) { MainChat.SendInfoChat(p, "[用法] /pet " + curPet.ID + " takip [id]"); return; }
                    PlayerModel ftarget = GlobalEvents.GetPlayerFromSqlID(followTarget);
                    if(ftarget == null) { MainChat.SendErrorChat(p, "[错误] Oyuncu bulunamadı!"); return; }
                    ped.followTarget = ftarget.Id;
                    ped.nametag = "[" + ped.Id + "] " + curPet.Name + "~n~~g~Takip Ediyor: ~w~" + ftarget.fakeName.Replace('_', ' ');
                    GuiNotification.Send(p, curPet.Name + ", " + ftarget.fakeName.Replace('_', ' ') + "'yi takip etmeye başladı.", "white", "positive", "green", icon: "pets");
                    ped.netOwner = ftarget.Id;
                    return;

                case "takipbirak":
                    ped.followTarget = null;
                    ped.nametag = "[" + ped.Id + "]" + curPet.Name;
                    GuiNotification.Send(p, curPet.Name + ", takibi bıraktı.", "white", "positive", "green", icon: "pets");
                    ped.netOwner = null;
                    curPet.Position = ped.Pos;
                    curPet.Dimension = ped.Dimension;
                    return;

                case "animasyon":
                    if (args.Length <= 2) { MainChat.SendInfoChat(p, "[用法] /pet " + curPet.ID + " animasyon [Animasyon kütüphanesi] [animasyon adı]"); return; }
                    ped.followTarget = null;
                    ped.animation = new string[] { args[2], args[3] };

                    MainChat.SendInfoChat(p, "[?] Evcil hayvan animasyonu başarıyla değiştirildi.");
                    return;

                case "ietkilesim":
                    if(ped.Dimension == 0)
                    {
                        var h = Props.Houses.getHouseFromPos(ped.Position);
                        if(h.Item1 != null)
                        {
                            ped.Position = h.Item1.intPos;
                            ped.Dimension = h.Item1.dimension;
                            GuiNotification.Send(p, curPet.Name + ", evin içine girdi.", "white", "positive", "blue-3", icon: "pets");
                            return;
                        }

                        var b = Props.Business.getBusinessFromPos(ped.Position);
                        if(b.Item1 != null)
                        {
                            ped.Position = b.Item1.interiorPosition;
                            ped.Dimension = b.Item1.dimension;
                            GuiNotification.Send(p, curPet.Name + ", işyerine girdi.", "white", "positive", "blue-3", icon: "pets");
                        }

                        PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Find(x => ped.Pos.Distance(x.Position) < 3f && x.Dimension == ped.Dimension);
                        if (entranceLabel != null)
                        {
                            entranceLabel.TryGetData(EntityData.EntranceTypes.EntranceType, out int entranceType);
                            switch (entranceType)
                            {
                                case 1:
                                    if(entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position pos))
                                    {
                                        ped.Position = pos;
                                        ped.Dimension = 0;
                                    }                                    
                                    return;

                                default:
                                    MainChat.SendInfoChat(p, "[?] Petin etkileşimde bulunabileceği bir interior bulunamadı.");
                                    break;
                            }
                        }
                    }
                    return;

                case "isim":
                    if(args.Length <= 2) { MainChat.SendInfoChat(p, "[用法] /pet " + ped.Id + " isim [isim]"); return; }
                    curPet.Name = string.Join(" ", args[2..]);
                    ped.nametag = "[" + curPet.ID + "]" + curPet.Name;
                    ped.followTarget = null;
                    GuiNotification.Send(p, curPet.ID + " ID li evcil hayvanın ismi başarıyla " + curPet.Name + " olarak değiştirildi.", "white", "positive", "blue-3", icon: "pets");
                    return;

                case "fix":

                    //if(p.Position.Distance(curPet.Position) > 50) { MainChat.SendErrorChat(p, "[错误] Komutu uygulamayabilmek için pet konumuna 50metre mesafede olmalısınız."); return; }+
                    /*ped.Position = p.Position;
                    ped.Dimension = p.Dimension;
                    ped.netOwner = p.Id;*/
        /*
                    if(ped != null)
                    {
                        ped.Destroy();
                    }

                    PedModel newestPet = PedStreamer.Create(GetPetDataFromType(curPet.Type).Item3, p.Position, p.Dimension, 100);
                    curPet.ID = newestPet.Id;

                    newestPet.nametag = "[" + curPet.ID + "]" + curPet.Name;
                    newestPet.hasNetOwner = true;
                    newestPet.Position = p.Position;
                    newestPet.Dimension = p.Dimension;
                    curPet.Position = p.Position;
                    curPet.Dimension = p.Dimension;
                    GuiNotification.Send(p, curPet.Name + " isimli pet için fix işlemi uygulandı.", "white", "positive", "blue-3", icon: "pets");
                    return;

            }
        }

        [Command("petduzenle")]
        public void COM_PetEdit(PlayerModel p, params string[] args)
        {
            if(p.adminLevel <= 4) { MainChat.SendErrorChat(p, "[错误] Bu komutu kullanabilmek için yetkiniz yok!"); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /petduzenle id [çeşit]"); return; }
            if(!Int32.TryParse(args[0], out int petID )) { MainChat.SendInfoChat(p, "[用法] /petduzenle id [çeşit]"); return; }

            var pet = getPedFromID((ulong)petID);
            if(pet == null) { MainChat.SendErrorChat(p, "[错误] Pet bulunamadı!"); return; }

            switch (args[1])
            {
                case "cek":
                    pet.Ped.Position = p.Position;
                    pet.Ped.Dimension = p.Dimension;
                    pet.Position = p.Position;
                    pet.Dimension = p.Dimension;
                    MainChat.SendInfoChat(p, "[?] [" + pet.ID + "]" + pet.Name + " isimli peti çektiniz.");
                    return;

                case "fix":
                    pet.Ped.Dimension = p.Dimension + 1;
                    pet.Ped.Dimension = p.Dimension;
                    MainChat.SendInfoChat(p, "[?] [" + pet.ID + "]" + pet.Name + " isimli peti fixlediniz.");
                    return;

                case "sil":
                    pet.Ped.Destroy();
                    pets.Remove(pet);
                    MainChat.SendErrorChat(p, "[?] Pet silindi.");
                    return;
            }
        }

        [Command("petlist")]
        public void PetList(PlayerModel p)
        { //
            if(p.adminLevel <= 1)
            { MainChat.SendErrorChat(p, "[错误] Bu komutu kullanabilmeniz için yetkiniz yok!"); return; }
            string text = "<center>Petler</center>";
            foreach(var pet in pets)
            {
                text += "<br>" + pet.ID + " | " + pet.Name + " | Sahip: " + pet.Owner;
            }

            MainChat.SendInfoChat(p, text);
            return;
        }

        [Command("petliste")]
        public void ShowPLayerPetList(PlayerModel p)
        {
            string text = "<center>Evcil Hayvanlarınız</center>";
            lock (pets)
            {
                foreach(var pet in pets)
                {
                    if (pet.Owner == p.sqlID)
                        text += "<br>[" + pet.ID + "] " + pet.Name;
                }
            }

            MainChat.SendInfoChat(p, text);
        }

        [Command("tumpetleresureekle")]
        public void COM_AddAllPetsTime(PlayerModel p)
        {
            lock (pets)
            {
                foreach(var pet in pets)
                {
                    pet.EndDate.AddDays(4);
                    pet.Level = 4;
                }
            }

            MainChat.SendInfoChat(p, "[?] Tüm petlere 4 gün eklendi ve levelları 4'e çekildi.");
            return;
        }
        #endregion

        public static PetModel getPedFromID(ulong ID)
        {
            return pets.Find(x => x.ID == ID);
        }

        public static PetModel getNearPed(Position position)
        {
            return pets.Where(x => x.Ped.Pos.Distance(position) < 5).OrderBy(x => x.Ped.Pos.Distance(position)).FirstOrDefault();
        }

        public static PetModel getNearPed(PlayerModel p)
        {
            return pets.Where(x => x.Position.Distance(p.Position) < 5).OrderBy(x => x.Position.Distance(p.Position)).FirstOrDefault();
        }

        public static (string, int, string) GetPetDataFromType(int type)
        {
            switch (type)
            {
                case 0: return ("kedi", PetPrices.Cat, "a_c_cat_01");
                case 1: return ("chop", PetPrices.Chop, "a_c_chop");
                case 2: return ("husky", PetPrices.Husky, "a_c_husky");
                case 3: return ("poodle", PetPrices.Poodle, "a_c_poodle");
                case 4: return ("pug", PetPrices.Pug, "a_c_pug");
                case 5: return ("retriever", PetPrices.Retriever, "a_c_retriever");
                case 6: return ("rottweiler", PetPrices.Rottweiler, "a_c_rottweiler");
                case 7: return ("shepherd", PetPrices.Shepherd, "a_c_shepherd");
                case 8: return ("westy", PetPrices.Westy, "a_c_westy");

                default: return ("null", 0, "null");
            }
        }

        public static (string, int, string) GetPetDataFromType(string type)
        {
            switch (type)
            {
                case "kedi": return ("kedi", PetPrices.Cat, "a_c_cat_01");
                case "chop": return ("chop", PetPrices.Chop, "a_c_chop");
                case "husky": return ("husky", PetPrices.Husky, "a_c_husky");
                case "poodle": return ("poodle", PetPrices.Poodle, "a_c_poodle");
                case "pug": return ("pug", PetPrices.Pug, "a_c_pug");
                case "retriever": return ("retriever", PetPrices.Retriever, "a_c_retriever");
                case "rottweiler": return ("rottweiler", PetPrices.Rottweiler, "a_c_rottweiler");
                case "shepherd": return ("shepherd", PetPrices.Shepherd, "a_c_shepherd");
                case "westy": return ("westy", PetPrices.Westy, "a_c_westy");

                default: return ("null", 0, "null");
            }
        }

        public List<PetModel> getOwnedPets(PlayerModel p)
        {
            List<PetModel> _pets = new();
            pets.FindAll(x => x.Owner == p.sqlID).ForEach(x => { _pets.Add(x); });
            return _pets;
        }

        // Clientside Events

        [AsyncClientEvent("PetMenu:WantToGps")]
        public void EVENT_WantGPS(PlayerModel p, int ID)
        {
            var ped = getPedFromID((ulong)ID);
            GlobalEvents.CheckpointCreate(p, ped.Ped.Position, 1, 1, new Rgba(0, 250, 0, 50), "", "");
            GuiNotification.Send(p, "Evcil hayvanın pozisyonu haritada işaretlendi.", "grey", "positive", "white", icon: "pets");
            return;
        }


        // Hunger Events 
        public static void PetHourTimer()
        {
            List<PetModel> removeList = new();

            lock (pets)
            {
                foreach (var pet in pets)
                {
                    if (pet != null)
                    {
                        if (pet.EndDate < DateTime.Now)
                        {
                            removeList.Add(pet);
                        }
                        else
                        {
                            pet.Hunger -= 0.1;
                            if (pet.Hunger <= 0)
                            {
                                pet.Exp -= 1;
                                pet.Hunger = 20;

                                if(pet.Exp <= 0)
                                {
                                    pet.Level -= 1;

                                    if(pet.Level <= 0)
                                    {
                                        removeList.Add(pet);
                                    }
                                }
                            }
                            else if (pet.Hunger >= 90)
                            {
                                pet.Exp += 1;
                                if (pet.Exp >= (pet.Level * 4))
                                {
                                    pet.Exp = 1;
                                    pet.Level += 1;
                                    if (pet.Level >= 5)
                                        pet.Level = 5;
                                }
                            }
                        }

                    }

                }
            }

            removeList.ForEach(x =>
            {
                x.Ped.Destroy();
                pets.Remove(x);
            });
            
            
        }

        // PetMenu Hunger
        [Command("petbesle")]
        public void COM_HungPet(PlayerModel p, params string[] args)
        {
            if(args.Length <= 0) { MainChat.SendInfoChat(p, "[用法] /petbesle [id]"); }
            if(!Int32.TryParse(args[0], out int pedid)) { MainChat.SendInfoChat(p, "[用法] /petbesle [id]"); }
            var ped = getPedFromID((ulong)pedid);
            if (p.cash <= 300) { MainChat.SendErrorChat(p, "[错误] Yeterli paranız yok!"); return; }

            if(ped.Ped.Pos.Distance(p.Position) > 20) { MainChat.SendErrorChat(p, "[错误] Hayvanın yanında değilsiniz."); return; }
            if(ped.Ped.Dimension != p.Dimension) { MainChat.SendErrorChat(p, "[错误] Hayvanın yanında değilsiniz."); return; }
            if(ped.LastHunger >= DateTime.Now) { MainChat.SendErrorChat(p, "[错误] Peti şuan besleyemezsiniz. Kalan Dakika: " + (DateTime.Now - ped.LastHunger).TotalMinutes); }

            p.cash -= 300;
            ped.Hunger += 40;
            if (ped.Hunger >= 100)
                ped.Hunger = 100;

            ped.LastHunger = DateTime.Now.AddMinutes(45);
            p.updateSql();
            return;

        }*/
    }
}
