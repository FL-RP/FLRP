using AltV.Net;

namespace outRp.OtherSystem.LSCsystems
{
    public class CargoJob : IScript
    {
        /*
        public class CargoConst
        {
            public static Position cargoJobPos = new Position(78.5011f, 111.784615f, 81.1604f);
            public static Position cargoTakeCrate = new Position(113.53846f, 103.29231f, 81.07617f);
            public static Position cargoFinishPos = new Position(76.32527f, 89.762634f, 78.71716f);

            public static List<Position> cargoLevel2DeliveryPos = new List<Position>()
            {
                new Position(1853.9868f, 2585.8154f, 45.978027f),
                new Position(2590.167f, 3168.9758f, 50.94873f),
                new Position(2529.0593f, 2616.91f, 37.940674f),
                new Position(1585.411f, 2905.7407f, 57.065186f)
            };

            public static List<Position> cargoLevel3DeliveryPos = new List<Position>()
            {
                new Position(2892.1582f, 4391.011f, 50.325317f),
                new Position(2139.4946f, 4790.3735f, 40.956787f),
                new Position(149.01099f, 6362.703f, 31.520874f),
                new Position(-438.05273f, 6148.0483f, 31.470337f)
            };
            

            public const string COM_Cargo = "gopostal_22211212xxxxx121212";

            public const string COM_CargoJob = "kargo";


            public static int[] Cargo_Finish_Payment = new int[] {0, 300, 350, 500, 300 };
            #region Cargo Data
            public const string Cargo_PlayerVehicleId = "CargoPlayerVehicleID";
            // Other
            public const string Cargo_Duty = "PlayerOnCargoDuty";

            public const string Cargo_SelectedJobLevel = "PlayerCargoSelectedLevel";
            public const string Cargo_Carry = "PlayerOnCarryCargo";

            public const string Cargo_Payment = "PlayerCargoPayment";

            public const string Cargo_VehicleCargo = "CargoVehicleCargo";



            public const string Cargo_Finish = "PlayerCargoFinish";

            //Blip:
            public const string PlayerHasBlip = "CargoPlayerHasBlip";

            public const string cargoDeliveryBlip = "CargoDeliveryBlip";

            //Bar Names:
            public const string vehicleCargoBar = "CargoVehicleCargoBar";
            #endregion
        }
        public static void LoadCargoJob()
        {            
            
            //PedMain.CreatePed(0x62599034, CargoConst.cargoJobPos, Heading: 250);
            //PedMain.CreatePed(0x7367324F, CargoConst.cargoTakeCrate, Heading: 150);           
        }
        
        [Command(CargoConst.COM_Cargo)][AsyncClientEvent("Cargo:MainMenu")]
        public static void COM_CARGO(PlayerModel p, string type = "")
        {  
            if(p.Position.Distance(CargoConst.cargoJobPos) > 10) { MainChat.SendErrorChat(p, "[HATA] Bu komutu kullanabilmek için kargo bölgesinde olmalısınız."); return; }
            if (JsonConvert.DeserializeObject<CharacterSettings>(p.settings).jobBan) { MainChat.SendErrorChat(p, "[!] Yönetim tarafından meslek yapmanız yasaklanmış."); return; }
            List<GuiMenu> gMenu = new List<GuiMenu>();
            GuiMenu cargoDuty = new GuiMenu { name = "İşbaşı Yap / İşbaşından çık", triger = "Cargo:StartDuty", value = "xx" };
            gMenu.Add(cargoDuty);

            if (p.HasData(CargoConst.Cargo_Duty)) { GuiMenu selectJob = new GuiMenu { name = "Cargo Görevi Seç", triger = "Cargo:SelectJob", value = "xx" }; gMenu.Add(selectJob); }
            if (p.HasData(CargoConst.Cargo_Payment)) { GuiMenu getPayment = new GuiMenu { name = "Ödeme al", triger = "Cargo:GetPayment", value = "xx" }; gMenu.Add(getPayment); }

            GuiMenu close = GuiEvents.closeItem;           
            
            gMenu.Add(close);
            Gui y = new Gui()
            {
                image = "https://vignette.wikia.nocookie.net/gtawiki/images/b/be/GoPostal-GTAV-Logo.png",
                guiMenu = gMenu,
                color = "#E1513D"
            };
            y.Send(p);            
        }

        [AsyncClientEvent("Cargo:GetPayment")]
        public void Cargo_GetPayment(PlayerModel p)
        {
            if (p.HasData(CargoConst.Cargo_Payment))
            {
                int payment = p.lscGetdata<int>(CargoConst.Cargo_Payment);
                p.cash += payment;
                p.DeleteData(CargoConst.Cargo_Payment);
                p.updateSql();
                GlobalEvents.NativeNotify(p, "~y~Kargo Ödemesi: ~g~$" + payment.ToString());
                GlobalEvents.notify(p, 6, "Kargo Ödemesi: $" + payment.ToString());
                Bar.RemoveAllBars(p);
                COM_CARGO(p);
                return;
            }
        }

        [AsyncClientEvent("Cargo:StartDuty")]
        public void Cargo_StartDuty(PlayerModel p)
        {
            if (p.HasData(CargoConst.Cargo_Duty))
            {
                p.DeleteData(CargoConst.Cargo_Duty);
                GlobalEvents.NativeNotify(p, "~r~İşbaşını bitirdiniz.");
                CARGO_ForceEnd(p, "İşbaşını bitirdiniz, eğer almadığınız ödemeniz varsa gidip alabilirsiniz. Unutmayın oyundan çıktığınızda ödemeleriniz silinir.");
                COM_CARGO(p);
                GlobalEvents.FreezePlayerClothes(p, true);
                return;
            }

            GlobalEvents.FreezePlayerClothes(p, false);

            if(p.sex == 0)
            {
                List<GlobalEvents.ClothingModel> clothes = new List<GlobalEvents.ClothingModel>();
                clothes.Add(new GlobalEvents.ClothingModel() { cID = 11, iID = 144, tID = 1 });
                clothes.Add(new GlobalEvents.ClothingModel() { cID = 4, iID = 45, tID = 1 });
                clothes.Add(new GlobalEvents.ClothingModel() { cID = 8, iID = 81, tID = 1 });
                clothes.Add(new GlobalEvents.ClothingModel() { cID = 10, iID = 53, tID = 2 });
                clothes.Add(new GlobalEvents.ClothingModel() { cID = 6, iID = 24, tID = 0 });
                clothes.Add(new GlobalEvents.ClothingModel() { cID = 5, iID = 82, tID = 0 });
                clothes.Add(new GlobalEvents.ClothingModel() { cID = 3, iID = 14, tID = 0 });
                GlobalEvents.SetClothSet(p, clothes);
            }
            else
            {
                List<GlobalEvents.ClothingModel> clothes = new List<GlobalEvents.ClothingModel>();
                clothes.Add(new GlobalEvents.ClothingModel() { cID = 11, iID = 147, tID = 1 });
                clothes.Add(new GlobalEvents.ClothingModel() { cID = 10, iID = 45, tID = 2 });
                clothes.Add(new GlobalEvents.ClothingModel() { cID = 3, iID = 4, tID = 0 });
                clothes.Add(new GlobalEvents.ClothingModel() { cID = 6, iID = 4, tID = 1 });
                clothes.Add(new GlobalEvents.ClothingModel() { cID = 8, iID = 78, tID = 1 });
                clothes.Add(new GlobalEvents.ClothingModel() { cID = 11, iID = 147, tID = 1 });
                GlobalEvents.SetClothSet(p, clothes);
            }
            GlobalEvents.NativeNotify(p, "~g~İşbaşı yaptınız.");
            MainChat.SendInfoChat(p, "> İşbaşı yaptınız");
            p.SetData(CargoConst.Cargo_Duty, true);
            COM_CARGO(p);
        }

        [AsyncClientEvent("Cargo:SelectJob")]
        public void Cargo_SelectJob(PlayerModel p)
        {
            List<GuiMenu> gMenu = new List<GuiMenu>();
            GuiMenu Job_1 = new GuiMenu { name = "Motorlu Kurye (Şehir İçi Dağıtım) | Gereklilik yok.", triger = "Cargo:StartJob", value = "1" };
            gMenu.Add(Job_1);
            GuiMenu Job_2 = new GuiMenu { name = "Normal Kurye (Eyalet Hapisanesi) | Gereklilik: Kargo Level 2+", triger = "Cargo:StartJob", value = "2" };
            gMenu.Add(Job_2);
            GuiMenu Job_3 = new GuiMenu { name = "Normal Kurye (Şehir İçi Dağıtım) | Gereklilik: Kargo Level 5+", triger = "Cargo:StartJob", value = "3" };
            gMenu.Add(Job_3);
            GuiMenu Job_4 = new GuiMenu { name = "Lojistik Kurye (Şehir Dışı Dağıtım) | Gereklilik: Kargo Level 10+", triger = "Cargo:StartJob", value = "4" };
            gMenu.Add(Job_4);
            GuiMenu goBack = new GuiMenu { name = "Geri", triger = "Cargo:MainMenu", value = "" };
            gMenu.Add(goBack);

            Gui y = new Gui()
            {
                image = "https://vignette.wikia.nocookie.net/gtawiki/images/b/be/GoPostal-GTAV-Logo.png",
                guiMenu = gMenu,
                color = "#E1513D"
            };
            y.Send(p);
        }

        [AsyncClientEvent("Cargo:StartJob")]
        public void Cargo_StartJob(PlayerModel p, int selectedId)
        {
            if (p.HasData(CargoConst.Cargo_SelectedJobLevel)) { MainChat.SendErrorChat(p, "[HATA] Zaten aktif bir göreviniz var, öncelikle görevi tamamlamalısınız."); return; }
            if (p.HasData(CargoConst.Cargo_Finish)) { p.DeleteData(CargoConst.Cargo_Finish); }
            StatModel stats = StatSystem.getPlayerStats(p);
            //if(stats.cargoStats == null) { stats.cargoStats = new CargoStats() { cargoLevel = 1, lastJob = DateTime.Now.AddDays(-1) }; stats.Update(p); }
            //int selectedId = Int32.Parse(jobId);
            switch (selectedId)
            {
                case 1:
                    p.lscSetData(CargoConst.Cargo_SelectedJobLevel, 1);
                    GlobalEvents.VehicleWaitInterval(p, "Cargo:PlayerEnterVehicle", "Cargo:PlayerEnterVehicleTimeout");
                    GlobalEvents.NativeNotify(p, "~g~60 ~w~saniye içinde bir kargo motora binin.");
                    return;

                case 2:
                    if(stats.cargoStats.cargoLevel < 2) { MainChat.SendErrorChat(p, "[HATA] Bu görev için yeterli meslek tecrübeniz bulunmuyor!"); return; }
                    p.lscSetData(CargoConst.Cargo_SelectedJobLevel, 2);
                    GlobalEvents.VehicleWaitInterval(p, "Cargo:PlayerEnterVehicle", "Cargo:PlayerEnterVehicleTimeout");
                    GlobalEvents.NativeNotify(p, "~g~60 ~w~saniye içinde bir kargo aracına binin.");
                    return;

                case 3:
                    if (stats.cargoStats.cargoLevel < 5) { MainChat.SendErrorChat(p, "[HATA] Bu görev için yeterli meslek tecrübeniz bulunmuyor!"); return; }
                    p.lscSetData(CargoConst.Cargo_SelectedJobLevel, 3);
                    GlobalEvents.VehicleWaitInterval(p, "Cargo:PlayerEnterVehicle", "Cargo:PlayerEnterVehicleTimeout");
                    GlobalEvents.NativeNotify(p, "~g~60 ~w~saniye içinde bir kargo aracına binin.");
                    return;

                case 4:
                    if (stats.cargoStats.cargoLevel < 10) { MainChat.SendErrorChat(p, "[HATA] Bu görev için yeterli meslek tecrübeniz bulunmuyor!"); return; }
                    p.lscSetData(CargoConst.Cargo_SelectedJobLevel, 4);
                    GlobalEvents.VehicleWaitInterval(p, "Cargo:PlayerEnterVehicle", "Cargo:PlayerEnterVehicleTimeout");
                    GlobalEvents.NativeNotify(p, "~g~60 ~w~saniye içinde bir kargo aracına binin.");
                    return;

                default:
                    return;
            }
        }

        [Command(CargoConst.COM_CargoJob)]
        public static void COM_CargoJob(PlayerModel p, params string[] args)
        {
            if (p.Vehicle != null) { MainChat.SendErrorChat(p, "[HATA] Bu komut sadece yaya iken kullanılabilir."); return; }
            if (args.Length <= 0) { MainChat.SendInfoChat(p, "[Kullanım] /" + CargoConst.COM_CargoJob + " [çeşit]<br>Kullanılabilecek çeşitler: al, koy, araçtanal, yerebırak(fazladan kargo alanlar için)"); return; }
            if (!p.HasData(CargoConst.Cargo_Duty)) { MainChat.SendErrorChat(p, "[HATA] Kargoculuk için işbaşı yapmalısınız."); return; }
            switch (args[0])
            {
                case "al":
                    if(p.Position.Distance(CargoConst.cargoTakeCrate) > 5) { MainChat.SendErrorChat(p, "> Kargo alabilmek için kargo bölgesine yakın olmalısınız."); return; }
                    if(p.HasData(CargoConst.Cargo_Carry)) { MainChat.SendErrorChat(p, "> Zaten bir kargo taşıyorsunuz, lütfen öncelikle kargoyu araca yükleyin."); return; }
                    p.lscSetData(CargoConst.Cargo_Carry, true);
                    GlobalEvents.NativeNotify(p, "~w~+1 ~g~Kargo");
                    Animations.PlayerAnimation(p, "carrybox");
                    return;

                    
                case "yerebırak":
                    p.DeleteData(CargoConst.Cargo_Carry);
                    Animations.PlayerStopAnimation(p);
                    MainChat.SendInfoChat(p, "> Kargo silindi.");
                    break;

                case "koy":
                    VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
                    int tempOwner = v.lscGetdata<int>(EntityData.VehicleEntityData.VehicleTempOwner);
                    if(tempOwner != p.sqlID) { MainChat.SendInfoChat(p, "> Kargoyu koymaya çalıştığınız araç sizin değil."); return; }
                    if (!p.HasData(CargoConst.Cargo_Carry)) { MainChat.SendErrorChat(p, "> Elinizde bir kargo bulunmuyor!"); return; }

                    if (v.HasData(CargoConst.Cargo_VehicleCargo))
                    {
                        int total_cargo = v.lscGetdata<int>(CargoConst.Cargo_VehicleCargo);
                        if(total_cargo >= 4) { MainChat.SendErrorChat(p, "> Bu araca daha fazla kargo yükleyemezsiniz."); return; }
                        total_cargo += 1;
                        v.lscSetData(CargoConst.Cargo_VehicleCargo, total_cargo);
                        if (total_cargo == 4) { MainChat.SendInfoChat(p, "> Araç kargo limitine ulaştı. Daha fazla kargo yükleyemeyeceksiniz.");
                            p.DeleteData(CargoConst.Cargo_Carry);
                            Animations.PlayerStopAnimation(p);
                            GlobalEvents.NativeNotify(p, "~w~+1 ~g~Araç kargosu");
                            Bar.CreateCheckpointBar(p, CargoConst.vehicleCargoBar, "Kargo: ", checkPoints: total_cargo);
                            Bar.BarSetCheckpointState(p, CargoConst.vehicleCargoBar, -1, false);
                            cargoDeliverySet(p);
                            GlobalEvents.NativeNotify(p, "~g~Göreve başlamaya hazırsınız!~n~Araca binin.");
                            return; }

                    }
                    else
                    {
                        v.lscSetData(CargoConst.Cargo_VehicleCargo, 1);
                    }

                    p.DeleteData(CargoConst.Cargo_Carry);
                    Animations.PlayerStopAnimation(p);
                    GlobalEvents.NativeNotify(p, "~w~+1 ~g~Araç kargosu");
                    return;

                case "araçtanal":                    
                    if (p.HasData(CargoConst.Cargo_Carry)) { MainChat.SendErrorChat(p, "> Zaten bir kargo taşıyorsunuz, lütfen öncelikle kargoyu teslim edin veya araca geri koyun."); return; }
                    VehModel v2 = Vehicle.VehicleMain.getNearVehFromPlayer(p);
                    int tempOwner2 = v2.lscGetdata<int>(EntityData.VehicleEntityData.VehicleTempOwner);
                    if (tempOwner2 != p.sqlID) { MainChat.SendInfoChat(p, "> Kargoyu almaya çalıştığınız araç sizin değil."); return; }
                    if (!v2.HasData(CargoConst.Cargo_VehicleCargo)) { MainChat.SendErrorChat(p, "> Bu araçta hiç kargo yok!"); return; }
                    int total_cargo2 = v2.lscGetdata<int>(CargoConst.Cargo_VehicleCargo);
                    total_cargo2 -= 1;
                    v2.lscSetData(CargoConst.Cargo_VehicleCargo, total_cargo2);
                    GlobalEvents.NativeNotify(p, "~w~+1 ~g~Kargo");
                    Animations.PlayerAnimation(p, "carrybox");
                    p.SetData(CargoConst.Cargo_Carry, true);

                    if (total_cargo2 <= 0) { v2.DeleteData(CargoConst.Cargo_VehicleCargo); p.lscSetData(CargoConst.Cargo_Finish, true); }
                    return;
                default:
                    MainChat.SendInfoChat(p, "[Kullanım] /kargo al/aracakoy/araçtanal");
                    return;
            }
        }


        [AsyncClientEvent("Cargo:DeliveryTry")]
        public async void cargoDeliveryTry(PlayerModel p, string pos)
        {
            if (p.HasData(CargoConst.Cargo_Carry))
            {
                GlobalEvents.NativeNotify(p, "~g~Kargo teslim edildi!");
                if (p.HasData(CargoConst.Cargo_Finish))
                {

                    if (p.HasData(CargoConst.Cargo_Payment))
                    {
                        int totalPayment = p.lscGetdata<int>(CargoConst.Cargo_Payment);
                        totalPayment += CargoConst.Cargo_Finish_Payment[p.lscGetdata<int>(CargoConst.Cargo_SelectedJobLevel)];
                        p.SetData(CargoConst.Cargo_Payment, totalPayment);
                        Bar.BarSetText(p, "kazanc", "~g~$" + totalPayment.ToString());
                    }
                    else
                    {
                        int paymentModifier = CargoConst.Cargo_Finish_Payment[p.lscGetdata<int>(CargoConst.Cargo_SelectedJobLevel)];
                        Bar.CreatePlayerBar(p, "kazanc", "Toplan Kazanc: ", rightText: "~g~$" + paymentModifier);
                        p.SetData(CargoConst.Cargo_Payment, paymentModifier);
                    }

                    
                    MainChat.SendInfoChat(p, "> Bütün kargoları teslim ettiniz, merkeze dönerek ödemenizi alın.");
                    Animations.PlayerStopAnimation(p);
                    StatModel stats = StatSystem.getPlayerStats(p);
                    stats.cargoStats.cargoLevel += 1;
                    stats.Update(p);
                    await Task.Delay(1000);
                    p.DeleteData(CargoConst.Cargo_Carry);
                    GlobalEvents.NativeNotify(p, "~r~Araca geri binip yola çıkmak için ~w~60 ~r~saniyen var.");
                    GlobalEvents.VehicleWaitInterval(p, "Cargo:FinishGoingBack", "Cargo:PlayerEnterVehicleTimeout");
                    return;
                }
                p.DeleteData(CargoConst.Cargo_Carry);
                Animations.PlayerStopAnimation(p);
                VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(p.lscGetdata<int>(CargoConst.Cargo_PlayerVehicleId));
                int cargoNum = v.lscGetdata<int>(CargoConst.Cargo_VehicleCargo);
                Bar.RemoveAllBars(p);
                if(cargoNum > 0)
                {
                    Bar.CreateCheckpointBar(p, CargoConst.vehicleCargoBar, "Kargo", cargoNum);
                }
                cargoDeliverySet(p);
            }
            else
            {
                MainChat.SendErrorChat(p, "> Kargoyu bu alana getirmelisiniz! Araca dönün ve kargoyu alın.");
                await Task.Delay(500);
                Position deliveriyPos = JsonConvert.DeserializeObject<Position>(pos);
                GlobalEvents.CheckpointCreate(p, deliveriyPos, 47, 2, new Rgba(255, 0, 0, 100), "Cargo:DeliveryTry", pos);
            }

        }
        [AsyncClientEvent("Cargo:DeliveryPointReached")]
        public void cargoDeliveryReached(PlayerModel p, string pos)
        {
            Position deliveriyPos = JsonConvert.DeserializeObject<Position>(pos);
            GlobalEvents.CheckpointCreate(p, deliveriyPos, 47, 2, new Rgba(255, 0, 0, 100), "Cargo:DeliveryTry", pos);
            GlobalEvents.NativeNotify(p, "~y~Teslimat bölgesine ulaştınız.");
            MainChat.SendInfoChat(p, "> Teslimat bölgesine ulaştınız. Kargo'yu araçtan alarak teslim ediniz.");

        }

        public static async void cargoDeliverySet(PlayerModel p)
        {
            if (p.HasData(CargoConst.PlayerHasBlip))
            {
                string hasBlip = p.lscGetdata<string>(CargoConst.PlayerHasBlip);
                GlobalEvents.DestroyBlip(p, hasBlip);
            }
            int jobID = p.lscGetdata<int>(CargoConst.Cargo_SelectedJobLevel);
            switch (jobID)
            {
                case 1:
                    Random case1result = new Random();
                    int case1Random = case1result.Next(1, Props.Business.TotalBusiness);
                    BusinessModel case1Delivery = Database.DatabaseMain.GetBusinessInfo(case1Random);
                    while (!(p.Position.Distance(case1Delivery.position) < 3500))
                    {
                        case1Random = case1result.Next(1, Props.Business.TotalBusiness);
                        case1Delivery = Database.DatabaseMain.GetBusinessInfo(case1Random);
                        await Task.Delay(500);
                    }
                    Position case1Pos = case1Delivery.position;
                    case1Pos.Z -= 1f;
                    GlobalEvents.CreateBlip(p, CargoConst.cargoDeliveryBlip, 2, case1Pos, route: true, sprite: 478, label: "Kargo teslimati", Short: false);
                    p.lscSetData(CargoConst.PlayerHasBlip, CargoConst.cargoDeliveryBlip);
                    GlobalEvents.CheckpointCreate(p, case1Pos, 47, 20, new Rgba(255, 0, 0, 40), "Cargo:DeliveryPointReached", JsonConvert.SerializeObject(case1Delivery.position));
                    
                    return;

                case 2:
                    Random case2result = new Random();
                    int case2random = case2result.Next(1, 4);
                    Position case2DeliveryPos = CargoConst.cargoLevel2DeliveryPos[case2random];
                    GlobalEvents.CreateBlip(p, CargoConst.cargoDeliveryBlip, 2, case2DeliveryPos, route: true, sprite: 478, label: "Kargo teslimati", Short: false);
                    p.lscSetData(CargoConst.PlayerHasBlip, CargoConst.cargoDeliveryBlip);
                    GlobalEvents.CheckpointCreate(p, case2DeliveryPos, 47, 20, new Rgba(255, 0, 0, 40), "Cargo:DeliveryPointReached", JsonConvert.SerializeObject(case2DeliveryPos));
                    return;

                case 3:
                    Random case3result = new Random();
                    int case3Random = case3result.Next(1, Props.Business.TotalBusiness);
                    BusinessModel case3Delivery = Database.DatabaseMain.GetBusinessInfo(case3Random);
                    while (!(p.Position.Distance(case3Delivery.position) < 3500))
                    {
                        case3Random = case3result.Next(1, Props.Business.TotalBusiness);
                        case3Delivery = Database.DatabaseMain.GetBusinessInfo(case3Random);
                        await Task.Delay(500);
                    }
                    Position case3Pos = case3Delivery.position;
                    case3Pos.Z -= 1f;
                    GlobalEvents.CreateBlip(p, CargoConst.cargoDeliveryBlip, 2, case3Pos, route: true, sprite: 478, label: "Kargo teslimati", Short: false);
                    p.lscSetData(CargoConst.PlayerHasBlip, CargoConst.cargoDeliveryBlip);
                    GlobalEvents.CheckpointCreate(p, case3Pos, 47, 20, new Rgba(255, 0, 0, 40), "Cargo:DeliveryPointReached", JsonConvert.SerializeObject(case3Delivery.position));

                    return;

                case 4:
                    Random case4result = new Random();
                    int case4random = case4result.Next(1, 4);
                    Position case4DeliveryPos = CargoConst.cargoLevel2DeliveryPos[case4random];
                    GlobalEvents.CreateBlip(p, CargoConst.cargoDeliveryBlip, 2, case4DeliveryPos, route: true, sprite: 478, label: "Kargo teslimati", Short: false);
                    p.lscSetData(CargoConst.PlayerHasBlip, CargoConst.cargoDeliveryBlip);
                    GlobalEvents.CheckpointCreate(p, case4DeliveryPos, 47, 20, new Rgba(255, 0, 0, 40), "Cargo:DeliveryPointReached", JsonConvert.SerializeObject(case4DeliveryPos));
                    return;
            }
        }

        [AsyncClientEvent("Cargo:FinishGoingBack")]
        public void cargoFinishGoingBack(PlayerModel p)
        {
            if (p.HasData(CargoConst.PlayerHasBlip))
            {
                string hasBlip = p.lscGetdata<string>(CargoConst.PlayerHasBlip);
                GlobalEvents.DestroyBlip(p, hasBlip);                
            }
            if(p.Position.Distance(CargoConst.cargoFinishPos) <= 25)
            {
                int totalPayment = p.lscGetdata<int>(CargoConst.Cargo_Payment);
                Bar.BarSetText(p, "kazanc", "~g~$" + totalPayment.ToString());
                if (p.Vehicle != null)
                {
                    VehModel v = (VehModel)p.Vehicle;
                    p.Emit("setPlayerOutVehicle", v);                    
                    v.Dimension = p.sqlID;
                    if (v.HasData(EntityData.VehicleEntityData.VehicleTempOwner)) { v.DeleteData(EntityData.VehicleEntityData.VehicleTempOwner); }
                    v.EngineOn = false;
                    v.DeleteData(CargoConst.Cargo_VehicleCargo);
                    v.LockState = VehicleLockState.Unlocked;
                    v.EngineHealth = 1000;
                    v.BodyHealth = 1000;
                    v.PetrolTankHealth = 1000;
                    GlobalEvents.RepairVehicle(v);
                    v.Position = v.settings.savePosition;
                    v.Rotation = v.settings.saveRotation;
                    v.Dimension = 0;
                }
                else
                {                     
                    CARGO_ForceEnd(p, "Görev Tamamlandı");
                }
                StatModel stats = StatSystem.getPlayerStats(p);
                stats.cargoStats.cargoExp += 1;
                if (stats.cargoStats.cargoExp >= (stats.cargoStats.cargoLevel * 4)) { stats.cargoStats.cargoLevel += 1; stats.cargoStats.cargoExp = 1; MainChat.SendInfoChat(p, "[Kargo] Kargoculuk seviyeniz yükseldi. [Seviye: " + stats.cargoStats.cargoLevel + "]"); }
                else { MainChat.SendInfoChat(p, "[Kargo] Kargo tecrübeniz arttı. Güncel tecrübe: " + stats.cargoStats.cargoExp + "/" + (stats.cargoStats.cargoLevel * 4)); }
                StatSystem.UpdatePlayerStats(p, stats);
                return;
            }   
            GlobalEvents.CreateBlip(p, CargoConst.cargoDeliveryBlip, 3, CargoConst.cargoFinishPos, route: true, sprite: 605, label: "Kargo Bitis", Short: false);
            GlobalEvents.CheckpointCreate(p, CargoConst.cargoFinishPos, 47, 20, new Rgba(255, 0, 0, 40), "Cargo:FinishGoingBack", "");
            GlobalEvents.PlayerExitVehicleWatcher(p, "Cargo:FinishGoingBack");
            Core.Logger.WriteLogData(Logger.logTypes.jobCash, p.characterName + " | Kargo Bitiş");
        }

        // Listeners
        [AsyncClientEvent("Cargo:PlayerEnterVehicle")]
        public async void CARGO_PlayerEnterVehicle(PlayerModel p, VehModel v)
        {
            if(v.jobId != ServerGlobalValues.JOB_Cargo) { CARGO_ForceEnd(p, "Bindiğiniz araç, bir kargo aracı değil. Görev sonlandırıldı."); return; }
            if (v.HasData(EntityData.VehicleEntityData.VehicleTempOwner))
            {
                int tempOwner = v.lscGetdata<int>(EntityData.VehicleEntityData.VehicleTempOwner);
                if(tempOwner != p.sqlID) { MainChat.SendErrorChat(p, "[HATA] Bindiğiniz araç başka birine tanımlı. Lütfen farklı bir araca binin."); p.Emit("setPlayerOutVehicle", v);
                    await Task.Delay(500);

                    if (!p.Exists)
                        return;

                    GlobalEvents.VehicleWaitInterval(p, "Cargo:PlayerEnterVehicle", "Cargo:PlayerEnterVehicleTimeout");
                    return;
                }
                if (v.HasData(CargoConst.Cargo_VehicleCargo))
                {
                    int totalCargo = v.lscGetdata<int>(CargoConst.Cargo_VehicleCargo);
                    if (totalCargo >= 4) 
                    {
                        cargoDeliverySet(p);
                    }
                }
            }

            int jobID = p.lscGetdata<int>(CargoConst.Cargo_SelectedJobLevel);
            switch (jobID)
            {
                case 1:
                    if(v.Model != (uint)VehicleModel.Faggio)
                    {
                        MainChat.SendErrorChat(p, "[HATA] Bindiğiniz araç başka birine tanımlı. Lütfen farklı bir araca binin."); p.Emit("setPlayerOutVehicle", v);
                        await Task.Delay(500);
                        if (!p.Exists)
                            return;

                        GlobalEvents.VehicleWaitInterval(p, "Cargo:PlayerEnterVehicle", "Cargo:PlayerEnterVehicleTimeout");
                    }
                    break;

                case 2:
                    if (v.Model != (uint)VehicleModel.Speedo)
                    {
                        MainChat.SendErrorChat(p, "[HATA] Bindiğiniz araç başka birine tanımlı. Lütfen farklı bir araca binin."); p.Emit("setPlayerOutVehicle", v);
                        await Task.Delay(500);
                        if (!p.Exists)
                            return;

                        GlobalEvents.VehicleWaitInterval(p, "Cargo:PlayerEnterVehicle", "Cargo:PlayerEnterVehicleTimeout");
                    }
                    break;

                case 3:
                    if (v.Model != (uint)VehicleModel.Boxville2)
                    {
                        MainChat.SendErrorChat(p, "[HATA] Bindiğiniz araç başka birine tanımlı. Lütfen farklı bir araca binin."); p.Emit("setPlayerOutVehicle", v);
                        await Task.Delay(500);
                        if (!p.Exists)
                            return;

                        GlobalEvents.VehicleWaitInterval(p, "Cargo:PlayerEnterVehicle", "Cargo:PlayerEnterVehicleTimeout");
                    }
                    break;

                case 4:
                    if (v.Model != (uint)VehicleModel.Pounder)
                    {
                        MainChat.SendErrorChat(p, "[HATA] Bindiğiniz araç başka birine tanımlı. Lütfen farklı bir araca binin."); p.Emit("setPlayerOutVehicle", v);
                        await Task.Delay(500);
                        if (!p.Exists)
                            return;

                        GlobalEvents.VehicleWaitInterval(p, "Cargo:PlayerEnterVehicle", "Cargo:PlayerEnterVehicleTimeout");
                    }
                    break;
            }

            v.lscSetData(EntityData.VehicleEntityData.VehicleTempOwner, p.sqlID);
            GlobalEvents.NativeNotify(p, "~y~Araç Kullanıma açıldı!");
            p.lscSetData(CargoConst.Cargo_PlayerVehicleId, v.sqlID);
            MainChat.SendInfoChat(p, "> Bu aracın geçici sahibi olarak tanımlandınız. Görevi tamamlayana kadar kilit/motor kullanım yetkiniz bulunacak.");
        }

        [AsyncClientEvent("Cargo:PlayerEnterVehicleTimeout")]
        public void CARGO_PlayerEnterVehicleTimeOut(PlayerModel p)
        {
            GlobalEvents.NativeNotify(p, "~r~Görev Başarısız oldu!");
            CARGO_ForceEnd(p, "Görev başarısız oldu, 60 saniye içinde görev aracına binmediniz.");
        }

        public static void CARGO_ForceEnd(PlayerModel p, string reason)
        {
            MainChat.SendInfoChat(p, "> "+ reason);
            if(p.HasData(CargoConst.Cargo_PlayerVehicleId))
            {
                int vehInt = p.lscGetdata<int>(CargoConst.Cargo_PlayerVehicleId);
                VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(vehInt);
                if(p.Vehicle != null) { p.Emit("setPlayerOutVehicle", v); }
                v.Position = v.savePos;
                if (v.HasData(EntityData.VehicleEntityData.VehicleTempOwner)) { v.DeleteData(EntityData.VehicleEntityData.VehicleTempOwner); }
                v.EngineOn = false;
                v.DeleteData(CargoConst.Cargo_VehicleCargo);
                v.LockState = VehicleLockState.Unlocked;
                v.EngineHealth = 1000;
                v.BodyHealth = 1000;
                v.PetrolTankHealth = 1000;

                p.DeleteData(CargoConst.Cargo_PlayerVehicleId);
            }
            if (p.HasData(CargoConst.Cargo_SelectedJobLevel)) { p.DeleteData(CargoConst.Cargo_SelectedJobLevel); }
            if (p.HasData(CargoConst.Cargo_Carry)) { p.DeleteData(CargoConst.Cargo_Carry); }
            if (p.HasData(CargoConst.PlayerHasBlip))
            {
                string blip = p.lscGetdata<string>(CargoConst.PlayerHasBlip);
                GlobalEvents.DestroyBlip(p, blip);
                p.DeleteData(CargoConst.PlayerHasBlip);
            }
            Bar.RemoveAllBars(p);

            if (p.HasData(CargoConst.Cargo_Payment))
            {
                int totalPayment = p.lscGetdata<int>(CargoConst.Cargo_Payment);
                Bar.BarSetText(p, "kazanc", "~g~$" + totalPayment.ToString());
            }
        }
        */

    }
}
