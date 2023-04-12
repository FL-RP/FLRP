

/*namespace outRp.OtherSystem.LSCsystems
{
    public class OOCmarket : IScript
    {
        #region NC function 
        [Command("isimdegistir")]
        public static void COM_ChangeName(PlayerModel p)
        {
            if (p.primary != null || p.secondary != null || p.melee != null) { MainChat.SendErrorChat(p, "[HATA] İşlem sırasında karakterinizin üzerinde silah bulunaması gerekiyor!"); return; }
            Inputs.SendButtonInput(p, "Bu işlem için 10 LSC puanı harcayacaksınız.", "OOCMarket:NC");
            return;
        }

        [AsyncClientEvent("OOCMarket:NC")]
        public async Task Market_NC_Step1(PlayerModel p, bool selection, string _trash)
        {
            if (selection)
            {
                AccountModel pAcc = await Database.DatabaseMain.getAccInfo(p.accountId);
                if (pAcc.lscPoint < 10) { MainChat.SendErrorChat(p, "[HATA] Yeterli LSC puanınız bulunmuyor."); return; }
                pAcc.lscPoint -= 10;
                await pAcc.Update();

                Inputs.SendTypeInput(p, "Yeni karakter adınız?", "OOCMarket:NC2", "x");
                return;
            }
            else
            {
                MainChat.SendErrorChat(p, "[!] NC işlemini reddettiniz.");
                return;
            }
        }

        [AsyncClientEvent("OOCMarket:NC2")]
        public async Task Market_NC_Step2(PlayerModel p, string newName, string _trash)
        {
            newName = newName.Replace(" ", "_");
            if (await Database.DatabaseMain.CheckCharacterName(newName)) { Globals.GlobalEvents.notify(p, 3, "Karakter adı veritabanında kayıtlı!"); Inputs.SendTypeInput(p, "Yeni karakter adınız?", "OOCMarket:NC2", "x"); return; }
            Core.Logger.WriteLogData(Logger.logTypes.OOCMarket, "[NC] " + p.characterName + " -> " + newName);
            MainChat.SendAdminChat(p.characterName + " isimli oyuncu adını " + newName + " olarak değiştirdi.");
            await Database.DatabaseMain.ClearPlayerMDCDatas(p.sqlID);
            p.injured.diseases = new List<DiseaseModel>();
            p.characterName = newName;
            p.businessStaff = 0;
            p.factionId = 0;
            p.factionRank = 0;
            p.updateSql();
            p.EmitLocked("reCreateChar");
            //Inputs.SendTypeInput(p, "Karakter yaşınız?", "OOCMarket:NC3", "x");
            return;
        }

        [AsyncClientEvent("OOCMarket:NCx")]
        public static async Task CharacterSettings(PlayerModel p, string data)
        {
            if (p.Ping > 250)
                return;
            try
            {
                Globals.GlobalEvents.CloseCamera(p);
                Models.CharacterSettings setting = JsonConvert.DeserializeObject<Models.CharacterSettings>(data);
                Models.CharacterSettings csettings = JsonConvert.DeserializeObject<Models.CharacterSettings>(p.settings);
                //csettings.secondLang = setting.secondLang;
                csettings.nation = setting.nation;
                csettings.age = setting.age;
                csettings.location = setting.location;
                csettings.licenses = new();
                csettings.isLawyer = false;
                p.settings = JsonConvert.SerializeObject(csettings);
                p.characterAge = setting.age;
                p.Position = new Position(-74f, -819f, 326f);
                p.injured.diseases = new();
                await p.updateSql();
                Inputs.SendButtonInput(p, "Karakteri yeniden şekillendir?", "OOCMarket:NC4", "x");
            }
            catch { }
        }


        [AsyncClientEvent("OOCMarket:NC3")]
        public void Market_NC_Step3(PlayerModel p, params string[] args)
        {
            if (!Int32.TryParse(args[0], out int newAge)) { Inputs.SendTypeInput(p, "Karakter yaşınız?", "OOCMarket:NC3", "x"); return; }
            p.characterAge = newAge;
            p.updateSql();
            Inputs.SendButtonInput(p, "Karakteri yeniden şekillendir?", "OOCMarket:NC4", "x");
            return;
        }

        [AsyncClientEvent("OOCMarket:NC4")]
        public void Market_NC_Step4(PlayerModel p, bool selection, string _trash)
        {
            if (selection)
            {
                p.EmitLocked("character:Redit", p.charComps);
                //MainChat.SendInfoChat(p, "Karakter uyruğu, ikincil dili ve diğer şeyler için rapor atmalısınız.");
                p.hp = 1000;
                p.maxHp = 1000;
                return;
            }
            else
            {
                MainChat.SendInfoChat(p, "[!] NC işleminiz başarıyla tamamlandı.");
                //MainChat.SendInfoChat(p, "Karakter uyruğu, ikincil dili ve diğer şeyler için rapor atmalısınız.");
                p.hp = 1000;
                p.maxHp = 1000;
                return;
            }
        }
        #endregion

        #region Özel Plaka
        [Command("ozelplaka")]
        public static void COM_SelectPrivatePlate(PlayerModel p)
        {
            if (p.HasData("InModify")) { MainChat.SendErrorChat(p, "[HATA] Modifiye menüsü açıkken bu komutu kullanamazsınız."); return; }
            Inputs.SendButtonInput(p, "Bu işlem için 30 LSC puanı harcayacaksınız.", "OOCMarket:Plate");
            return;
        }

        [AsyncClientEvent("OOCMarket:Plate")]
        public async Task Event_Plate_Step1(PlayerModel p, bool selection, string _trash)
        {
            if (selection)
            {
                AccountModel pAcc = await Database.DatabaseMain.getAccInfo(p.accountId);
                if (pAcc.lscPoint < 30) { MainChat.SendErrorChat(p, "[HATA] Yeterli LSC puanınız bulunmuyor."); return; }
                VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
                if (v == null) { MainChat.SendErrorChat(p, "[HATA] Yakınınızda bir araç bulunmuyor veya bir aracın içerisinde değilsiniz."); return; }
                if (v.owner != p.sqlID) { MainChat.SendErrorChat(p, "[HATA] Bu araç size ait değil."); return; }
                var model = (VehicleModel)v.Model;

                Inputs.SendButtonInput(p, model.ToString() + " model aracın plakasını değiştirekcesiniz.", "OOCMarket:Plate2");
                return;
            }
            else
            {
                MainChat.SendErrorChat(p, "[!] Araç plaka değişimini reddettiniz.");
            }
        }

        [AsyncClientEvent("OOCMarket:Plate2")]
        public async Task Event_Plate_Step2(PlayerModel p, bool selection, string _trash)
        {
            if (selection)
            {
                AccountModel pAcc = await Database.DatabaseMain.getAccInfo(p.accountId);
                if (pAcc.lscPoint < 30) { MainChat.SendErrorChat(p, "[HATA] Yeterli LSC puanınız bulunmuyor."); return; }
                VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
                if (v == null) { MainChat.SendErrorChat(p, "[HATA] Yakınınızda bir araç bulunmuyor veya bir aracın içerisinde değilsiniz."); return; }
                if (v.owner != p.sqlID) { MainChat.SendErrorChat(p, "[HATA] Bu araç size ait değil."); return; }
                var model = (VehicleModel)v.Model;

                Inputs.SendTypeInput(p, model.ToString() + " model araç için plaka girin", "OOCMarket:Plate3");
                return;
            }
            else
            {
                MainChat.SendErrorChat(p, "[!] Araç plaka değişimini reddettiniz.");
            }
        }

        [AsyncClientEvent("OOCMarket:Plate3")]
        public async Task Event_Plate_Step3(PlayerModel p, params string[] args)
        {
            AccountModel pAcc = await Database.DatabaseMain.getAccInfo(p.accountId);
            if (pAcc.lscPoint < 30) { MainChat.SendErrorChat(p, "[HATA] Yeterli LSC puanınız bulunmuyor."); return; }
            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v == null) { MainChat.SendErrorChat(p, "[HATA] Yakınınızda bir araç bulunmuyor veya bir aracın içerisinde değilsiniz."); return; }
            if (v.owner != p.sqlID) { MainChat.SendErrorChat(p, "[HATA] Bu araç size ait değil."); return; }
            var model = (VehicleModel)v.Model;


            bool canUse = true;
            foreach (VehModel veh in Alt.GetAllVehicles())
            {
                if (veh.NumberplateText.ToLower() == args[0].ToLower())
                    canUse = false;
            }

            if (!canUse) { MainChat.SendErrorChat(p, "[HATA] Bu plakayı kullanan başka bir araç mevcut"); return; }

            if (args[0].Length < 4 || args[0].Length > 8) { MainChat.SendErrorChat(p, "[HATA] Plaka en fazla 4 ile 8 karakter uzunluğunda olabilir."); Inputs.SendTypeInput(p, model.ToString() + " model araç için plaka girin", "OOCMarket:Plate3"); return; }

            string oldPlate = v.NumberplateText;
            v.NumberplateText = args[0].Replace(' ', '_');
            v.settings.ModifiyData = v.AppearanceData;
            v.Update();
            pAcc.lscPoint -= 30;
            await pAcc.Update();
            MainChat.SendInfoChat(p, "[!] Araç plakası başarıyla " + args[0] + " olarak kaydedildi.");
            Core.Logger.WriteLogData(Logger.logTypes.OOCMarket, p.characterName + " plate" + oldPlate + "->" + args[0]);
            MainChat.SendAdminChat("[Ozel Plaka] " + p.characterName.Replace(" ", "_") + " isimli oyuncu [" + v.sqlID + "]" + model.ToString() + " aracın plakasını değiştirdi. " + oldPlate + "->" + args[0]);
            return;
        }
        #endregion

        #region Plate Type
        [Command("plakacesit")]
        public async Task COM_SetPlateTemplate(PlayerModel p, params string[] args)
        {
            if (p.HasData("InModify")) { MainChat.SendErrorChat(p, "[HATA] Modifiye menüsü açıkken bu komutu kullanamazsınız."); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[Kullanım] /plakacesit [plaka çeşiti]<br>Plaka çeşitlerini görmek için UCP kısmını kontrol edebilirisiniz."); return; }
            if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[HATA] Bu komutu kullanabilmek için bir aracın içinde olmalısınız."); return; }
            AccountModel acc = await Database.DatabaseMain.getAccInfo(p.accountId);
            if (acc == null) { MainChat.SendErrorChat(p, "[HATA] Hesap bilgileri getirilirken bir hata meydana geldi. Bu yazıyla karşışlaştığınızda lütfen LSC yönetim ekibi ile irtibata geçiniz. HATA KODU: 0x000001"); return; }
            if (acc.lscPoint < 20) { MainChat.SendErrorChat(p, "[HATA] Yeterli LSC Puanı bulunamadı. Gerekli LSC Puanı: 20LSCP"); return; }
            Inputs.SendButtonInput(p, "Aracınıza özel plaka takılacak 15LSCP", "OOCMarket:PlateType_1", args[0]);
            return;
        }

        [AsyncClientEvent("OOCMarket:PlateType_1")]
        public async Task EVENT_PlateType_Step1(PlayerModel p, bool selection, string val)
        {
            if (!selection)
            {
                MainChat.SendInfoChat(p, "[?] Özel plaka işlemini iptal ettiniz.");
                return;
            }
            else
            {
                if (!Int32.TryParse(val, out int PlateType)) { MainChat.SendErrorChat(p, "[HATA] Plaka çeşidi getirilirken bir hata meydana geldi. Lütfen yeniden deneyin."); return; }
                if (p.Vehicle == null) { MainChat.SendErrorChat(p, "[HATA] Bu komutu kullanabilmek için bir aracın içinde olmalısınız."); return; }
                VehModel v = (VehModel)p.Vehicle;
                if (v == null) { MainChat.SendErrorChat(p, "[HATA] Bu komutu kullanabilmek için bir aracın içinde olmalısınız."); return; }
                if (v.owner != p.sqlID) { MainChat.SendErrorChat(p, "[HATA] Bu aracın sahibi siz değilsiniz."); return; }
                AccountModel acc = await Database.DatabaseMain.getAccInfo(p.accountId);
                if (acc == null) { MainChat.SendErrorChat(p, "[HATA] Hesap bilgileri getirilirken bir hata meydana geldi. Bu yazıyla karşışlaştığınızda lütfen LSC yönetim ekibi ile irtibata geçiniz. HATA KODU: 0x000002"); return; }
                if (acc.lscPoint < 20) { MainChat.SendErrorChat(p, "[HATA] Bu işlemi yapabilmek için yeterli LSC Puanınız bulunmuyor."); return; }
                if (!p.HasData("InModify"))
                {
                    v.NumberplateIndex = (uint)PlateType;
                    v.settings.ModifiyData = v.AppearanceData;
                    v.Update();
                    acc.lscPoint -= 20;
                    await acc.Update();
                    MainChat.SendInfoChat(p, "[?] Aracınıza 20 LSC-Puanı karşılığında özel plaka çeşidi taktırdınız.");
                    return;
                }
                else
                {
                    MainChat.SendErrorChat(p, "[HATA] Modifiye menüsü açıkken bu işlemi yapamazsınız.");
                }

            }
        }
        #endregion

        #region NPC system
        public class OOCMarketNPC
        {
            public int Owner { get; set; }
            public ulong PedID { get; set; }
            public string PedModel { get; set; }
            public DateTime EndDate { get; set; }
            public Position Position { get; set; }
            public int Heading { get; set; }
            public int Dimension { get; set; }
            public string Name { get; set; }
            public string[] Animation { get; set; } = new string[] { "idle", "idle" };
        }

        public static List<OOCMarketNPC> npcList = new List<OOCMarketNPC>();

        public static void LoadNPCs(string data)
        {
            npcList = JsonConvert.DeserializeObject<List<OOCMarketNPC>>(data);
            foreach (var NPC in npcList)
            {
                PedModel ped = PedStreamer.Create(NPC.PedModel, NPC.Position, NPC.Dimension);
                NPC.PedID = ped.Id;
                ped.nametag = NPC.Name;
                ped.heading = NPC.Heading;
            }
            return;
        }

        [Command("npcekle")]
        public async Task COM_AddNPC(PlayerModel p, string model)
        {
            AccountModel acc = await Database.DatabaseMain.getAccInfo(p.accountId);
            if (acc == null) { MainChat.SendErrorChat(p, "[HATA] Hesap bilgileri getirilirken bir hata meydana geldi. Bu yazıyla karşışlaştığınızda lütfen LSC yönetim ekibi ile irtibata geçiniz. HATA KODU: 0x000003"); return; }
            if (acc.lscPoint < 20) { MainChat.SendErrorChat(p, "[HATA] Hesabınızda yeterli LSC Puanı bulunamadı."); return; }
            Inputs.SendButtonInput(p, "20 LSCP karşılığında npc eklenecek.", "OOCMarket:Npc:Step1", model.ToString());
            return;
        }

        [AsyncClientEvent("OOCMarket:Npc:Step1")]
        public async Task EVENT_AddNpc_Step1(PlayerModel p, bool selection, string model)
        {
            if (!selection) { MainChat.SendInfoChat(p, "[?] NPC oluşturma işlemini iptal ettiniz."); return; }
            AccountModel acc = await Database.DatabaseMain.getAccInfo(p.accountId);
            if (acc == null) { MainChat.SendErrorChat(p, "[HATA] Hesap bilgileri getirilirken bir hata meydana geldi. Bu yazıyla karşışlaştığınızda lütfen LSC yönetim ekibi ile irtibata geçiniz. HATA KODU: 0x000004"); return; }
            if (acc.lscPoint < 20) { MainChat.SendErrorChat(p, "[HATA] Yeterli LSC Puanınız bulunmuyor."); return; }

            acc.lscPoint -= 20;
            await acc.Update();

            OOCMarketNPC npc = new OOCMarketNPC()
            {
                Owner = p.sqlID,
                PedModel = model.ToString(),
                EndDate = DateTime.Now.AddDays(30),
                Position = p.Position,
                Heading = 0,
                Dimension = p.Dimension,
                Name = "İsimsiz"
            };
            npc.PedID = PedStreamer.Create(model.ToString(), npc.Position, npc.Dimension).Id;
            npcList.Add(npc);
            MainChat.SendInfoChat(p, "[?] 20 LSC Puanı karşılığında 30 günlük NPC satın aldınız.<br> /npcduzenle komutuyla eklenen NPC'yi düzenleyebilirsiniz.");
            return;
        }

        [Command("npcduzenle")]
        public static void COM_EditNPC(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[Kullanım] /npcduzenle [çeşit] [varsa değer]<br>Kullanılabilir çeşitler:<br>isim, heading, animasyon"); return; }
            OOCMarketNPC npc = npcList.Where(x => x.Position.Distance(p.Position) < 4 && x.Dimension == p.Dimension).OrderBy(x => x.Position.Distance(p.Position)).FirstOrDefault();
            if (npc == null) { MainChat.SendErrorChat(p, "[HATA] Yakınınızda bir NPC bulunamadı."); return; }
            if (npc.Owner != p.sqlID) { MainChat.SendErrorChat(p, "[HATA] Düzenlemeye çalıştığınız NPC size ait değil."); return; }
            PedModel ped = PedStreamer.Get(npc.PedID);
            if (ped == null) { MainChat.SendErrorChat(p, "[HATA] NPC PED getirilirken bir hata meydana geldi."); return; }

            switch (args[0])
            {
                case "isim":
                    npc.Name = string.Join(" ", args[1..]);
                    ped.nametag = npc.Name;
                    MainChat.SendInfoChat(p, "[?] NPC adı başarıyla " + string.Join(" ", args[1..]) + " olarak değiştirildi.");
                    return;

                case "heading":
                    if (!Int32.TryParse(args[1], out int newHeading)) { MainChat.SendInfoChat(p, "[Kullanım] /npcduzenle heading [Sayı değeri]"); return; }
                    ped.heading = newHeading;
                    npc.Heading = newHeading;
                    MainChat.SendInfoChat(p, "[?] NPC'nin bakış yönü başarıyla ayarlandı.");
                    return;

                case "animasyon":
                    if (args.Length <= 1) { MainChat.SendErrorChat(p, "[Kullanım] /npcduzenle animasyon [Animasyon adı]"); return; }
                    AnimModel anim = Anims.Where(x => x.Key == args[1]).FirstOrDefault().Value;
                    if (anim != null)
                    {
                        ped.animation = new string[] { anim.dict, anim.anim };
                        npc.Animation = new string[] { anim.dict, anim.anim };
                    }
                    else
                    {
                        ped.animation = new string[] { "1", "2" };
                        npc.Animation = new string[] { "1", "2" };
                    }

                    MainChat.SendInfoChat(p, "[?] NPC animasyonu başarıyla değiştirildi.");
                    return;

                case "ozelanimasyon":
                    if (args.Length <= 2) { MainChat.SendInfoChat(p, "[Kullanım] /npcduzenle ozelanimasyon [Animasyon kütüphanesi] [animasyon adı]"); return; }
                    ped.animation = new string[] { args[1], args[2] };
                    npc.Animation = new string[] { args[1], args[2] };

                    MainChat.SendInfoChat(p, "[?] NPC animasyonu başarıyla değiştirildi.");
                    return;

                default:
                    MainChat.SendInfoChat(p, "[Kullanım] /npcduzenle [çeşit] [varsa değer]<br>Kullanılabilir çeşitler:<br>isim, heading, animasyon"); return;
            }
        }

        [Command("marketnpcsil")]
        public async Task COM_DeletePlayerMarketNPC(PlayerModel p)
        {
            OOCMarketNPC npc = npcList.Where(x => x.Position.Distance(p.Position) < 5 && x.Dimension == p.Dimension && x.Owner == p.sqlID).OrderBy(x => x.Position.Distance(p.Position)).FirstOrDefault();
            if (npc == null) { MainChat.SendErrorChat(p, "[HATA] Yakınınızda bir npc bulunamadı!"); return; }
            int refund = 0;
            if (npc.EndDate < DateTime.Now.AddMinutes(10))
                refund = 20;

            PedModel ped = PedStreamer.Get(npc.PedID);
            if (ped != null)
                ped.Destroy();

            npcList.Remove(npc);
            if (refund > 0)
            {
                MainChat.SendInfoChat(p, "[?] NPC'yi 10 dakika geçmeden kaldırdığınız için hesabınıza 20LSC-P iade edildi.");
                AccountModel acc = await Database.DatabaseMain.getAccInfo(p.accountId);
                if (acc != null)
                {
                    acc.lscPoint += refund;
                    await acc.Update();
                }
                return;
            }
            else
            {
                MainChat.SendInfoChat(p, "[?] NPC başarıyla kaldırıldı.");
                return;
            }

        }

        [Command("yakinmarketnpcler")]
        public static void COM_ShowNearMarketNPCS(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, "[HATA] Bu komutu kullanabilmek için yetkiniz yok!"); return; }
            string text = "<center>NPC Listesi</center>";
            if (args.Length <= 0)
            {
                foreach (var n in npcList)
                {
                    text += "<br>ID: " + n.PedID + "|Sahip ID: " + n.Owner + "|Pos:" + JsonConvert.SerializeObject(n.Position);
                }

                MainChat.SendInfoChat(p, text, true);
                return;
            }
            else
            {
                if (!Int32.TryParse(args[0], out int Distance)) { MainChat.SendInfoChat(p, "[Kullanım] /yakinmarketnpcler [mesafe]"); return; }
                foreach (var npc in npcList.Where(x => x.Position.Distance(p.Position) <= Distance))
                {
                    text += "<br>ID: " + npc.PedID + "|Sahip ID: " + npc.Owner + "|Pos:" + JsonConvert.SerializeObject(npc.Position);
                }
                MainChat.SendInfoChat(p, text, true);
                return;
            }
        }

        [Command("npclerim")]
        public void COM_ShowOwnNPCS(PlayerModel p)
        {
            var NPCS = npcList.Where(x => x.Owner == p.sqlID);
            string text = "<center>Size Ait NPC Listesi</center>";
            foreach (var n in NPCS)
            {
                text += "<br>ID: " + n.PedID + "|Pos: " + JsonConvert.SerializeObject(n.Position) + "| /npcisaretle " + n.PedID;
            }
            MainChat.SendInfoChat(p, text, true);
            return;
        }

        [Command("npcisaretle")]
        public void COM_WayPointNPC(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[Kullanım] /npcisaretle [NPC ID]"); return; }
            if (!Int32.TryParse(args[0], out int NPCID)) { MainChat.SendInfoChat(p, "[Kullanım] /npcisaretle [NPC ID]"); return; }
            var npc = npcList.Find(x => x.PedID == (ulong)NPCID);
            if (npc == null) { MainChat.SendErrorChat(p, "[HATA] NPC bulunamadı!"); return; }
            if (npc.Owner != p.sqlID && p.adminLevel <= 0) { MainChat.SendErrorChat(p, "[HATA] Bu NPC size ait değil!"); return; }
            if (npc.Dimension != 0 && npc.Dimension != p.Dimension)
            {
                PlayerLabel entranceLabel = TextLabelStreamer.GetAllDynamicTextLabels().Where(x => npc.Position.Distance(x.Position) <= 50 && x.Dimension == npc.Dimension).FirstOrDefault();
                if (entranceLabel == null) { MainChat.SendInfoChat(p, "[HATA] Interior'un çıkış kapısına yakın olmalısınız"); return; }
                if (!entranceLabel.TryGetData(EntityData.EntranceTypes.ExitWorldPosition, out Position pos))
                {
                    MainChat.SendErrorChat(p, "[HATA] NPC pozisyonu bulunamadı!");
                    return;
                }

                GlobalEvents.CheckpointCreate(p, pos, 1, 2, new Rgba(255, 0, 0, 255), "no", "no");
                return;
            }
            else
            {
                GlobalEvents.CheckpointCreate(p, npc.Position, 1, 2, new Rgba(255, 0, 0, 255), "no", "no");
                return;
            }
        }

        [Command("amarketnpcsil")]
        public void COM_DeleteMarketNPC(PlayerModel p)
        {
            if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, "[HATA] Bu komutu kullanabilmek için yetkiniz yok!"); return; }
            OOCMarketNPC npc = npcList.Where(x => x.Position.Distance(p.Position) < 4 && x.Dimension == p.Dimension).OrderBy(x => x.Position.Distance(p.Position)).FirstOrDefault();
            if (npc == null) { MainChat.SendErrorChat(p, "[HATA] NPC bulunamadı."); return; }

            PedModel ped = PedStreamer.Get(npc.PedID);
            if (ped != null)
            {
                ped.Destroy();
            }

            npcList.Remove(npc);
            MainChat.SendErrorChat(p, "[?] NPC başarıyla kaldırıldı.");
            return;
        }

        public static void EVENT_CheckNPCEndDate()
        {
            List<OOCMarketNPC> removeList = new List<OOCMarketNPC>();
            foreach (var NPC in npcList.ToList())
            {
                if (NPC.EndDate <= DateTime.Now)
                {
                    PedModel ped = PedStreamer.Get(NPC.PedID);
                    if (ped != null) { ped.Destroy(); }
                    removeList.Add(NPC);
                }
            }

            foreach (var NP in removeList)
            {
                if (npcList.Find(x => x.PedID == NP.PedID) != null)
                    npcList.Remove(NP);
            }
        }
        #endregion

        #region Character Creation Menu
        [Command("karakteryaratma")]
        public async Task COM_CreateCharacterMenu(PlayerModel p)
        {
            if (p.primary != null || p.secondary != null || p.melee != null) { MainChat.SendErrorChat(p, "[HATA] İşlem sırasında karakterinizin üzerinde silah bulunaması gerekiyor!"); return; }
            AccountModel acc = await Database.DatabaseMain.getAccInfo(p.accountId);
            if (acc == null) { MainChat.SendErrorChat(p, "[HATA] Hesap bilgileri getirilirken bir hata meydana geldi. Bu yazıyla karşışlaştığınızda lütfen LSC yönetim ekibi ile irtibata geçiniz. HATA KODU: 0x000004"); return; }
            if (acc.lscPoint < 5) { MainChat.SendErrorChat(p, "[HATA] Bu işlem için yeterli LSC Puanınız bulunmuyor."); return; }
            Inputs.SendButtonInput(p, "Karakter Yaratma Ekranı | Ücret 5LSC-P", "OOCMarket:CreateCharacter", "trash");
            return;
        }

        [AsyncClientEvent("OOCMarket:CreateCharacter")]
        public async Task EVENT_CrateCharacterMenu(PlayerModel p, bool selection, string trash)
        {
            if (!selection) { MainChat.SendInfoChat(p, "[?] Karakter yaratma işlemini iptal ettiniz."); return; }
            AccountModel acc = await Database.DatabaseMain.getAccInfo(p.accountId);
            if (acc == null) { MainChat.SendErrorChat(p, "[HATA] Hesap bilgileri getirilirken bir hata meydana geldi. Bu yazıyla karşışlaştığınızda lütfen LSC yönetim ekibi ile irtibata geçiniz. HATA KODU: 0x000005"); return; }
            if (acc.lscPoint < 2) { MainChat.SendErrorChat(p, "[HATA] Bu işlem için yeterli LSC Puanınız bulunmuyor."); return; }

            acc.lscPoint -= 5;
            await acc.Update();
            p.EmitLocked("character:Redit", p.charComps);
            MainChat.SendInfoChat(p, "[?] Başarıyla karakter yaratma ekranına yönlendirildiniz. Harcanan LSC Puanı 5");
            return;
        }
        #endregion
    }
}*/
