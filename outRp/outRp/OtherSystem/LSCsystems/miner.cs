namespace outRp.OtherSystem.LSCsystems
{
   /* public class Miner : IScript
    {
        public static Position MinerStart = new Position(2654.677f, 1693.3319f, 24.477661f);
        public static Position MinerLoad = new Position(2962.6946f, 2835.877f, 45.152344f);
        public static Position MinerWorking = new Position(2930.1758f, 2791.055f, 40.131104f);
        public static int MinerWork = 0;
        public static string inMining = "inMining";


        public class PMine
        {
            public int Cooper { get; set; } = 0;
            public int Silver { get; set; } = 0;
            public int Iron { get; set; } = 0;
            public int Gold { get; set; } = 0;
        }
        public class VehicleMine
        {
            public int Cooper { get; set; } = 0;
            public int Silver { get; set; } = 0;
            public int Iron { get; set; } = 0;
            public int Gold { get; set; } = 0;
        }
        public static VehicleMine getVehicleMine(PlayerModel p)
        {
            if (p.HasData("Mining:VMining"))
            {
                return JsonConvert.DeserializeObject<VehicleMine>(p.lscGetdata<string>("Mining:VMining"));
            }
            else
                return new VehicleMine();
        }
        public static void setPlayerMine(PlayerModel p, VehicleMine vehicle)
        {
            p.SetData("Mining:VMining", JsonConvert.SerializeObject(mine));
        }
        public static PMine getPlayerMine(PlayerModel p)
        {
            if (p.HasData("Mining:PMining"))
            {
                return JsonConvert.DeserializeObject<PMine>(p.lscGetdata<string>("Mining:PMining"));
            }
            else
                return new PMine();
        }

        public static void setPlayerMine(PlayerModel p, PMine mine)
        {
            p.SetData("Mining:PMining", JsonConvert.SerializeObject(mine));
        }

        public static void LoadMinerJob()
        {
            TextLabelStreamer.Create("~r~[~w~Madencilik~r~]~n~Başlama için ~g~/minerisbasi", Miner.MinerStart, center: true, font: 0, streamRange: 5);
            Alt.Log("Madencilik Sistemi yüklendi.");
        }

        [Command("minerisbasi")]
        public void startMinerJob(PlayerModel p)
        {
            if (p.Position.Distance(Miner.MinerStart) > 5) { MainChat.SendErrorChat(p, "[HATA] Bu komutu kullanabilmek için maden meslek başlama bölgesinde olmalısınız."); return; }
            if (!p.HasData("Job:Miner:Duty"))
            {
                p.SetData("Job:Miner:Duty", true);
                MainChat.SendInfoChat(p, "[i] Miner mesleğine giriş yaptınız, 60 saniye içerisinde araca binin.");
                GlobalEvents.VehicleWaitInterval(p, "Miner:EnterVehicle", "", "x");
            }
            else { p.DeleteData("Job:Miner:Duty"); MainChat.SendInfoChat(p, "[i] Miner mesleğinden çıkış yaptınız."); }

        }
        [AsyncClientEvent("Miner:EnterVehicle")]
        public void TruckJobEnterVeh(PlayerModel p, VehModel v, string trash)
        {
            if (v.jobId != ServerGlobalValues.JOB_Miner && p.HasData("Job:Miner:Duty")) { MainChat.SendErrorChat(p, "[HATA] Bu bir meslek aracı değil. Görev iptal edildi."); p.DeleteData("Job:Miner:Duty"); return; }
            p.SetData("Truck:Vehicle", v.sqlID);
            v.lscSetData(EntityData.VehicleEntityData.VehicleTempOwner, p.sqlID);
            GlobalEvents.CheckpointCreate(p, Miner.MinerLoad, 63, 5, new Rgba(255, 255, 255, 100), "Miner:Step1", "");
            GlobalEvents.SubTitle(p, "~y~İşletme Şefi: ~w~Yol boyunca araçtan inmemeniz gerekiyor.Lütfen belirlediğimiz kurallara uyun.", 10);
            GlobalEvents.CreateBlip(p, "minerjob", 2, Miner.MinerLoad, route: true, sprite: 477, color: 5, label: "Görev Noktası", Short: false);
        }

        //GlobalEvents.CheckpointCreate(p, h.pos, 1, 2, new Rgba(255, 0, 0, 255), "no", "no");
        [AsyncClientEvent("Miner:Step1")]
        public async void TruckJobFirstCheckpoint(PlayerModel p)
        {
            VehModel v = (VehModel)p.Vehicle;
            if (v.jobId != ServerGlobalValues.JOB_Miner) { MainChat.SendErrorChat(p, "[HATA] Bu bir meslek aracı değil."); GlobalEvents.ForceLeaveVehicle(p); }
            if (v.Driver != p) { MainChat.SendErrorChat(p, "[HATA] Görevi tamamlamak için sürücü olmalısınız."); return; }
            if (p.sqlID != v.owner) { MainChat.SendErrorChat(p, "[HATA] Bu araç size ait değil."); return; }

            GlobalEvents.DestroyBlip(p, "minerjob");
            await Task.Delay(200);

            if (!p.Exists)
                return;


            GlobalEvents.SubTitle(p, "~y~İşletme Şefi: ~w~Kazmayı kullanırken dikkatli ol.", 10);
            GlobalEvents.CheckpointCreate(p, Miner.MinerWorking, 1, 2, new Rgba(255, 0, 0, 255), "no", "no");
        }
        [Command("minerbasla")]
        public void startMine(PlayerModel p)
        {
            if (p.HasData("Miner:Carry")) { MainChat.SendErrorChat(p, "[HATA] Bir maden taşıyorsunuz."); return; }
            if (p.HasData("inMining")) { MainChat.SendErrorChat(p, "[HATA] Kazı işlemi sürüyor."); return ;}
            if (!p.HasData("Job:Miner:Duty")) { MainChat.SendErrorChat(p, "[HATA] Madencilik görevinde değilsiniz."); return; }
            if (p.Position.Distance(Miner.MinerWorking) > 20) { MainChat.SendErrorChat(p, "[HATA] Kazı bölgesinde değilsiniz!"); return;
            }
            else { GlobalEvents.ProgresBar(p, text:"Kazılıyor",time:10);}
            p.EmitAsync("Miner:StartWorking");
            p.SetData("inMining", true);
        }
        [AsyncClientEvent("Miner:Succes")]
        public static void MinerSuccess(PlayerModel p)
        {
            if (p.HasData("Miner:Carry")) { return; }
            if(p.Position.Distance(MinerWorking) > 20) { Alt.Log("Bu adam hileci ;)"); return; }
            p.DeleteData("inMining");
            if (p.HasData("Job:Miner:Duty"))
            {
                PMine w = getPlayerMine(p);
                Random rnd = new Random();
                int key = rnd.Next(1, 100);
                MainChat.SendInfoChat(p, key + " Geldi");
                if(key >= 0 && key <= 25)
                {
                    if(key >= 15 && key <= 20) { w.Cooper += 2; MainChat.SendInfoChat(p, "[i] 2 adet bakır madeni buldunuz!"); } else{w.Cooper += 1; MainChat.SendInfoChat(p, "[i] 1 adet bakır madeni buldunuz!"); Animations.PlayerAnimation(p, "carrybox"); p.SetData("Miner:Carry", true); }
                   
                }else if(key >= 25 && key <= 50)
                {
                    if (key >= 30 && key <= 40) { w.Silver += 2; MainChat.SendInfoChat(p, "[i] 2 adet gümüş madeni buldunuz!"); } else { w.Silver += 1; MainChat.SendInfoChat(p, "[i] 1 adet gümüş madeni buldunuz!"); Animations.PlayerAnimation(p, "carrybox"); p.SetData("Miner:Carry", true); }
                }else if(key >= 50 && key <= 85)
                {
                    if (key >= 60 && key <= 70) { w.Iron += 2; MainChat.SendInfoChat(p, "[i] 2 adet demir madeni buldunuz!"); } else { w.Iron += 1; MainChat.SendInfoChat(p, "[i] 1 adet demir madeni buldunuz!"); Animations.PlayerAnimation(p, "carrybox"); p.SetData("Miner:Carry", true); }
                }else if(key >= 80 && key <= 90)
                {
                   w.Gold += 1; MainChat.SendInfoChat(p, "[i] 1 adet altın madeni buldunuz!"); Animations.PlayerAnimation(p, "carrybox"); p.SetData("Miner:Carry", true);
                }
                else if(key >= 90 && key <= 100)
                {
                    MainChat.SendInfoChat(p, "[i] Hiç bir şey bulamadınız!");
                }
            }
            else
            {
                Alt.Log("HATA");
            }
            
            return;
        }
        [Command("madenbibrak")]
        public void MineOnVehicle(PlayerModel p)
        {
            PMine w = getPlayerMine(p);
            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            int tempOwner = v.lscGetdata<int>(EntityData.VehicleEntityData.VehicleTempOwner);
            int total_Mine = 0;
            if (tempOwner != p.sqlID) { MainChat.SendInfoChat(p, "> Kargoyu koymaya çalıştığınız araç sizin değil."); return; }
            if (!p.HasData("Miner:Carry")) { MainChat.SendErrorChat(p, "> Elinizde bir kargo bulunmuyor!"); return; }
            if(w.Cooper != 0)
            {
                v.total_Mine = w.Cooper;
            }
            p.DeleteData("Miner:Carry");

        }
    }*/
}
