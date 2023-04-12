

/*namespace outRp.OtherSystem.LSCsystems
{
    public class farming : IScript
    {
        public class FarmModel
        {
            public int Owner { get; set; }
            public Position Position { get; set; }
            public int Type { get; set; }
            public ulong TextLabelID { get; set; }
            public int Dimension { get; set; }

            public List<Land> Lands { get; set; } = new List<Land>();
            public List<Animal> Animals { get; set; } = new List<Animal>();
            public List<Fruit> Fruits { get; set; } = new List<Fruit>();
            public List<FarmObject> Objects { get; set; } = new List<FarmObject>();

            public class Land
            {
                public ulong TextLabelID { get; set; }
                public int Type { get; set; }
                public Position Position { get; set; }
                public int Range { get; set; }
                public int Health { get; set; }
            }

            public class Animal
            {
                public ulong ID { get; set; }
                public string Model { get; set; }
                public int Type { get; set; }
                public int GrowthState { get; set; }
                public int Health { get; set; }
                public Position Position { get; set; }
                public int Dimension { get; set; }
                public void Update()
                {
                    PedModel animal = PedStreamer.Get(this.ID);
                    if (animal == null)
                        return;

                    animal.nametag = getAnimalName(this);

                }
            }

            public class Fruit
            {
                public ulong ID { get; set; }
                public ulong TextLabel { get; set; }
                public string Model { get; set; }
                public int FruitType { get; set; }
                public Position Position { get; set; }
                public int GrowthState { get; set; }
                public int Health { get; set; }
                public int Dimension { get; set; }
            }

            public class FarmObject
            {
                public ulong ID { get; set; }
                public ulong TextLabelID { get; set; }
                public int Type { get; set; }
                public string Model { get; set; }
                public Position Position { get; set; }
                public Rotation Rotation { get; set; }
                public int Dimension { get; set; }
                public int Vault { get; set; }
            }
        }

        public static List<FarmModel> serverFarms = new List<FarmModel>();


        #region Functions 
        public static void LoadServerFarms(string data)
        {
            return;
        }
        public static FarmModel getNearFarm(PlayerModel p)
        {
            return serverFarms.Where(x => x.Position.Distance(p.Position) < 50).OrderBy(x => x.Position.Distance(p.Position)).FirstOrDefault();
        }
        public static FarmModel getNearFarm(Position p)
        {
            return serverFarms.Where(x => x.Position.Distance(p) < 50).OrderBy(x => x.Position.Distance(p)).FirstOrDefault();
        }

        public static async Task<FarmModel> checkFarm(PlayerModel p)
        {
            var farm = getNearFarm(p);
            if(farm == null) { return null; }
            var farmBusiness = await getBusinessById(farm.Owner);
            if (farmBusiness.Item1 == null)
                return null;

            if (!await CheckBusinessKey(p, farmBusiness.Item1))
                return null;

            return farm;
        }

        public static string getFarmObjectName(FarmModel.FarmObject obj)
        {
            switch (obj.Type)
            {
                case 1:
                    return "[YALAK]~n~Doluluk: " + obj.Vault + "~g~%";

                default: return "BOŞ";
            }
        }
        public static string getAnimalName(FarmModel.Animal animal)
        {
            string growth = "~n~Gelişim: " + animal.GrowthState + "~g~%";
            if (animal.GrowthState >= 90)
                growth = "~n~~g~Ürün Toplanabilir";
            return "[" + animal.ID + "]" + getAnimalModelName(animal.Type) + "~n~Sağlık: " + animal.Health + growth;
        }
        public static string getAnimalModelName(int AnimalType)
        {
            switch (AnimalType)
            {
                case 1: return "Domuz";
                case 2: return "Leylek";
                case 3: return "İnek";
                case 4: return "Tavuk"; 
                case 5: return "PembeDomuz";
                case 6: return "Tavşan";
                default: return "İnek";
            }
        }

        public static string getFruitName(FarmModel.Fruit fruit)
        {
            string growth = "~n~Gelişim: " + fruit.GrowthState + "~g~%";
            if (fruit.GrowthState >= 90)
                growth = "~n~~g~Ürün Toplanabilir.";
            return "[" + fruit.ID + "]" + getFruitModelName(fruit.FruitType) + "~n~Sağlık: " + fruit.Health + growth;
        }
        public static string getFruitModelName(int FruitType)
        {
            switch (FruitType)
            {
                case 1: return "Limon Ağacı";
                case 2: return "Elma Ağacı";
                case 3: return "Domates Fidesi";
                case 4: return "Patates";
                case 5: return "Soğan";
                case 6: return "Çilek";
                case 7: return "Buğday";
                case 8: return "Arpa";
                default: return "Limon Ağacı";
            }
        }

        public static (FarmModel.Fruit, FarmModel) getNearFruit(PlayerModel p)
        {
            FarmModel farm = serverFarms.Where(x => x.Position.Distance(p.Position) < 70).FirstOrDefault();
            FarmModel.Fruit fruit = farm.Fruits.Where(x => x.Position.Distance(p.Position) < 4).OrderBy(x => x.Position.Distance(p.Position)).FirstOrDefault();
            return (fruit, farm);
        }
        public static void SetCarCarry(VehModel v)
        {
            string text = "~b~[~w~YÜK~b~]~w~";
            if (v.HasData("Farm:Carry:Pig"))
                text += "~n~Domuz: " + v.lscGetdata<int>("Farm:Carry:Pig");
            if (v.HasData("Farm:Carry:Cow"))
                text += "~n~İnek: " + v.lscGetdata<int>("Farm:Carry:Cow");
            if (v.HasData("Farm:Carry:Stork"))
                text += "~n~Leylek: " + v.lscGetdata<int>("Farm:Carry:Stork");
            if (v.HasData("Farm:Carry:Hen"))
                text += "~n~Tavuk: " + v.lscGetdata<int>("Farm:Carry:Hen");
            if (v.HasData("Farm:Carry:Rabbit"))
                text += "~n~Tavşan: " + v.lscGetdata<int>("Farm:Carry:Rabbit");

            GlobalEvents.SetVehicleTag(v, text);
            return;
        }

        public static (FarmModel.Animal, FarmModel) getNearAnimal(PlayerModel p)
        {
            FarmModel farm = serverFarms.Where(x => x.Position.Distance(p.Position) < 70).FirstOrDefault();
            FarmModel.Animal animal = farm.Animals.Where(x => x.Position.Distance(p.Position) < 4).OrderBy(x => x.Position.Distance(p.Position)).FirstOrDefault();
            return (animal, farm);
        }
        #endregion
        #region Admin Commands
        [Command("farmekle")]
        public async Task COM_CreateFarm(PlayerModel p, params string[] args)
        {
            if (p.adminLevel <= 4) { MainChat.SendErrorChat(p, "[HATA] Bu komutu kullanmak için yetkiniz yok!"); return; }
            if(args.Length <= 0) { MainChat.SendInfoChat(p, "[Kullanım] /farmekle [işyeri ID]"); return; }

            if(!Int32.TryParse(args[0], out int BizID)) { MainChat.SendInfoChat(p, "[Kullanım] /farmekle [işyeri ID]"); return; }
            var biz = await getBusinessById(BizID);
            if (biz.Item1 == null) { MainChat.SendErrorChat(p, "[HATA] Girdiğiniz ID'de bir işyeri bulunamadı!"); return; }
            if(biz.Item1.type != ServerGlobalValues.farmBusiness) { MainChat.SendErrorChat(p, "[HATA] Bu işyerinin tipi çiftlik değil!"); return; }
            FarmModel farm = new FarmModel();
            farm.Owner = biz.Item1.ID;
            farm.Position = p.Position;
            farm.Dimension = p.Dimension;
            farm.Type = 0;
            farm.TextLabelID = TextLabelStreamer.Create("~b~[~w~Çiftlik~b~]", p.Position, p.Dimension, font: 0).Id;

            serverFarms.Add(farm);

            MainChat.SendInfoChat(p, "[?] Bulunduğunuz konuma başarıyla bir çiftlik eklediniz!");
            return;
        }


        [Command("farmsil")]
        public static void COM_DeleteFarm(PlayerModel p)
        {
            if(p.adminLevel <= 4) { MainChat.SendErrorChat(p, "[HATA] Bu komutu kullanabilmek için yetkiniz yok!"); return; }
            var farm = getNearFarm(p);
            if(farm == null) { MainChat.SendErrorChat(p, "[HATA] Yakınınızda bir çiftlik bulunamadı!"); return; }
            PlayerLabel farmText = TextLabelStreamer.GetDynamicTextLabel(farm.TextLabelID);
            if (farmText != null)
                farmText.Delete();

            farm.Lands.ForEach(x =>
            {
                PlayerLabel landText = TextLabelStreamer.GetDynamicTextLabel(x.TextLabelID);
                if (landText != null)
                    landText.Delete();
            });

            farm.Animals.ForEach(x =>
            {
                PedModel ped = PedStreamer.Get(x.ID);
                if (ped == null)
                    return;

                ped.Destroy();
            });

            farm.Fruits.ForEach(x =>
            {
                LProp furuit = PropStreamer.GetProp(x.ID);
                if (furuit == null)
                    return;

                furuit.Destroy();
            });

            serverFarms.Remove(farm);
            MainChat.SendInfoChat(p, "[?] Çiftlik başarıyla kaldırıldı.");
            return;
        }
        #endregion

        #region Farm  Animal Actions
        
        public static async Task<bool> PlaceAnimal(PlayerModel p, int animalType)
        {
            var farm = await checkFarm(p);
            if (farm == null) { MainChat.SendErrorChat(p, "[HATA] Yakınınızda bir çiftlik bulunamadı!"); return false; }
            var land = farm.Lands.Where(x => x.Type == 1).OrderBy(x => x.Position.Distance(p.Position)).FirstOrDefault();
            if(land == null) { MainChat.SendErrorChat(p, "[HATA] Hayvan yerleştirebileceğiniz bir alan kurulu değil!"); return false; }
            if(land.Position.Distance(p.Position) > 20) { MainChat.SendErrorChat(p, "[HATA] Hayvan bölgesine en fazla 20 metre uzaklığa hayvan yerleştirebilirsiniz."); return false; }

            if (farm.Animals.Count >= 20) { MainChat.SendErrorChat(p, "[HATA] Çiftliğe en fazla 20 adet hayvan ekleyebilirsiniz."); return false; }

            FarmModel.Animal animal = new FarmModel.Animal();
            animal.Dimension = p.Dimension;
            animal.GrowthState = 0;
            animal.Health = 100;
            animal.Position = p.Position;
            animal.Type = animalType;

            switch (animalType)
            {
                case 1: // Domuz
                    animal.Model = "a_c_boar";
                    break;

                case 2: // Leylek
                    animal.Model = "a_c_cormorant";
                    break;

                case 3: // İnek
                    animal.Model = "a_c_cow";
                    break;

                case 4: // Tavuk
                    animal.Model = "a_c_hen";
                    break;

                case 5: // Domuz 2
                    animal.Model = "a_c_pig";
                    break;

                case 6: // Tavşan
                    animal.Model = "a_c_rabbit_01";
                    break;

                default:
                    animal.Model = "a_c_cow";
                    break;
            }

            PedModel _animal = PedStreamer.Create(animal.Model, animal.Position, animal.Dimension);
            animal.ID = _animal.Id;
            _animal.nametag = getAnimalName(animal);

            farm.Animals.Add(animal);

            MainChat.SendInfoChat(p, "[?] Hayvan başarıyla oluşturuldu.");
            return true;            
        }

        public static async Task<bool> PlaceAnimalObject(PlayerModel p)
        { 
            var farm = await checkFarm(p);
            if (farm == null) { MainChat.SendErrorChat(p, "[HATA] Yakınınızda bir çiftlik bulunamadı!"); return false; }
            var land = farm.Lands.Where(x => x.Type == 1).OrderBy(x => x.Position.Distance(p.Position)).FirstOrDefault();
            if (land == null) { MainChat.SendErrorChat(p, "[HATA] Hayvan yerleştirebileceğiniz bir alan kurulu değil!"); return false; }
            if (land.Position.Distance(p.Position) > 20) { MainChat.SendErrorChat(p, "[HATA] Hayvan bölgesine en fazla 20 metre uzaklığa hayvan yerleştirebilirsiniz."); return false; }

            GlobalEvents.ShowObjectPlacement(p, "cs6_06_trough_small_02", "Farm:Create:Trough");
            return true;
        }

        [AsyncClientEvent("Farm:Create:Trough")]
        public async Task EVENT_CreateTrough(PlayerModel p, string rot, string pos, string model)
        {
            var farm = await checkFarm(p);
            if (farm == null) { MainChat.SendErrorChat(p, "[HATA] Yakınınızda bir çiftlik bulunamadı!"); return; }
            var land = farm.Lands.Where(x => x.Type == 1).OrderBy(x => x.Position.Distance(p.Position)).FirstOrDefault();
            if (land == null) { MainChat.SendErrorChat(p, "[HATA] Hayvan yerleştirebileceğiniz bir alan kurulu değil!"); return; }
            if (land.Position.Distance(p.Position) > 20) { MainChat.SendErrorChat(p, "[HATA] Hayvan bölgesine en fazla 20 metre uzaklığa hayvan yerleştirebilirsiniz."); return; }


            Vector3 position = JsonConvert.DeserializeObject<Vector3>(pos);
            Vector3 rotation = JsonConvert.DeserializeObject<Vector3>(rot);

            FarmModel.FarmObject trough = new FarmModel.FarmObject();
            trough.Dimension = p.Dimension;
            trough.Model = "cs6_06_trough_small_02";
            trough.Position = position;
            trough.Rotation = rotation;
            trough.Type = 1;
            trough.Vault = 0;
            trough.ID = PropStreamer.Create(trough.Model, trough.Position, trough.Rotation, trough.Dimension, false, frozen: true).Id;
            trough.TextLabelID = TextLabelStreamer.Create(getFarmObjectName(trough), trough.Position, trough.Dimension, true, font: 0).Id;

            MainChat.SendInfoChat(p, "[?] Yalak başarıyla kuruldu!");
            return;
        }

        [Command("hayvan")]
        public async Task COM_Animal(PlayerModel p, params string[] args)
        {
            if (args.Length <= 1) { MainChat.SendInfoChat(p, "[Kullanım] /hayvan [çeşit] [varsa değer]"); return; }
            var _farm = getNearAnimal(p);
            var animal = _farm.Item1;
            if(_farm.Item1 == null || _farm.Item2 == null) { MainChat.SendErrorChat(p, "[HATA] Yakınınızda bir çiftlik veya hayvan bulunamadı!"); return; }
            var biz = await getBusinessById(_farm.Item2.Owner);
            if(biz.Item1 == null) { MainChat.SendErrorChat(p, "[HATA] Çiftliğin bağlı bulunduğu işyeri ile ilgili bir hata meydana geldi."); return; }

            ServerItems i = null;
            Random rnd = new Random();
            switch (args[0])
            {
                case "urunal":
                    if(animal.GrowthState <= 90) { MainChat.SendErrorChat(p, "[HATA] Hayvan ürün alınabilecek yetişkinliğe ulaşmamış!"); return; }
                    switch (animal.Type)
                    {
                        case 3: // İnek
                            i = Items.LSCitems.Find(x => x.ID == 72);
                            if (!await Inventory.AddInventoryItem(p, i, rnd.Next(1, 4))) { MainChat.SendErrorChat(p, "[HATA] Üzerinizde yeterli alan yok!"); return; }

                            if(rnd.Next(1,6) >= 5)
                            {
                                PedModel cow = PedStreamer.Get(animal.ID);
                                if (cow != null)
                                {
                                    cow.Destroy();                                    
                                }

                                _farm.Item2.Animals.Remove(animal);
                                MainChat.SendInfoChat(p, "[?] İneğiniz öldü.");
                                return;
                            }

                            animal.GrowthState = 50;
                            animal.Update();
                            return;

                        case 4: // tavuk
                            i = Items.LSCitems.Find(x => x.ID == 73);
                            if(!await Inventory.AddInventoryItem(p, i, rnd.Next(1, 7))) { MainChat.SendErrorChat(p, "[HATA] Üzerinizde yeterli alan yok!"); return; }

                            if(rnd.Next(1,6) >= 5)
                            {
                                PedModel hen = PedStreamer.Get(animal.ID);
                                if (hen != null)
                                {
                                    hen.Destroy();
                                }

                                _farm.Item2.Animals.Remove(animal);
                                MainChat.SendInfoChat(p, "[?] Tavuğunuz öldü!");
                                return;
                            }

                            animal.GrowthState = 50;
                            animal.Update();
                            break;

                        default:
                            MainChat.SendErrorChat(p, "[HATA] Bu hayvan türünden ürün alamazsınız!");
                            return;
                    }
                    return;

                case "aracayukle":
                    if (args.Length < 2) { MainChat.SendInfoChat(p, "[Kullanım] /hayvan aracayukle [araç ID]"); return; }
                    if (!Int32.TryParse(args[1], out int vehSQL)) { MainChat.SendInfoChat(p, "[Kullanım] /hayvan aracayukle [araç ID]"); return; }

                    VehModel upCar = Vehicle.VehicleMain.getVehicleFromSqlId(vehSQL);
                    if (upCar == null) { MainChat.SendErrorChat(p, "[HATA] Araç bulunamadı!"); return; }
                    if(upCar.LockState != VehicleLockState.Unlocked) { MainChat.SendErrorChat(p, "[HATA] Bu aracın kapıları kilitli durumda!"); return; }
                    if(upCar.Position.Distance(p.Position) >= 10) { MainChat.SendErrorChat(p, "[HATA] Araç yeterli yakınlıkta değil!"); return; }
                    switch (animal.Type)
                    {
                        case 1:
                            if (upCar.Model != (uint)VehicleModel.GrainTrailer)
                            {
                                MainChat.SendErrorChat(p, "[HATA] Bu tip hayvnları yükleyebilmek için GrainTrailer'e ihtiyacınız var!");
                                return;
                            }
                            else
                            {
                                if (upCar.HasData("Farm:Carry:Pig"))
                                {
                                    int PigCount = upCar.lscGetdata<int>("Farm:Carry:Pig");
                                    if (PigCount >= 2) { MainChat.SendErrorChat(p, "[HATA] Bu araca daha fazla domuz yükleyemezsiniz!"); return; }
                                    PigCount += 1;

                                    PedModel Pig1 = PedStreamer.Get(animal.ID);
                                    if (Pig1 != null)
                                        Pig1.Destroy();

                                    _farm.Item2.Animals.Remove(animal);
                                    upCar.lscSetData("Farm:Carry:Pig", PigCount);
                                    SetCarCarry(upCar);
                                    MainChat.SendInfoChat(p, "[?] Domuz başarıyla araca yüklendi.");
                                    return;
                                }
                                else
                                {
                                    upCar.lscSetData("Farm:Carry:Pig", 1);
                                    PedModel Pig1 = PedStreamer.Get(animal.ID);
                                    if (Pig1 != null)
                                        Pig1.Destroy();

                                    _farm.Item2.Animals.Remove(animal);

                                    SetCarCarry(upCar);
                                    MainChat.SendInfoChat(p, "[?] Domuz başarıyla araca yüklendi.");
                                    return;
                                }
                            }

                        case 5:
                            if (upCar.Model != (uint)VehicleModel.GrainTrailer)
                            {
                                MainChat.SendErrorChat(p, "[HATA] Bu tip hayvnları yükleyebilmek için GrainTrailer'e ihtiyacınız var!");
                                return;
                            }
                            else
                            {
                                if (upCar.HasData("Farm:Carry:Pig"))
                                {
                                    int PigCount1 = upCar.lscGetdata<int>("Farm:Carry:Pig");
                                    if (PigCount1 >= 2) { MainChat.SendErrorChat(p, "[HATA] Bu araca daha fazla domuz yükleyemezsiniz!"); return; }
                                    PigCount1 += 1;

                                    PedModel Pig2 = PedStreamer.Get(animal.ID);
                                    if (Pig2 != null)
                                        Pig2.Destroy();

                                    _farm.Item2.Animals.Remove(animal);
                                    upCar.lscSetData("Farm:Carry:Pig", PigCount1);
                                    SetCarCarry(upCar);
                                    MainChat.SendInfoChat(p, "[?] Domuz başarıyla araca yüklendi.");
                                    return;
                                }
                                else
                                {
                                    upCar.lscSetData("Farm:Carry:Pig", 1);
                                    PedModel Pig1 = PedStreamer.Get(animal.ID);
                                    if (Pig1 != null)
                                        Pig1.Destroy();

                                    _farm.Item2.Animals.Remove(animal);

                                    SetCarCarry(upCar);
                                    MainChat.SendInfoChat(p, "[?] Domuz başarıyla araca yüklendi.");
                                    return;
                                }
                            }
                        case 3:
                            if (upCar.Model != (uint)VehicleModel.GrainTrailer)
                            {
                                MainChat.SendErrorChat(p, "[HATA] Bu tip hayvnları yükleyebilmek için GrainTrailer'e ihtiyacınız var!");
                                return;
                            }
                            else
                            {
                                if (upCar.HasData("Farm:Carry:Cow"))
                                {
                                    int PigCount = upCar.lscGetdata<int>("Farm:Carry:Cow");
                                    if (PigCount >= 2) { MainChat.SendErrorChat(p, "[HATA] Bu araca daha fazla inek yükleyemezsiniz!"); return; }
                                    PigCount += 1;

                                    PedModel Pig1 = PedStreamer.Get(animal.ID);
                                    if (Pig1 != null)
                                        Pig1.Destroy();

                                    _farm.Item2.Animals.Remove(animal);
                                    upCar.lscSetData("Farm:Carry:Cow", PigCount);
                                    SetCarCarry(upCar);
                                    MainChat.SendInfoChat(p, "[?] İnek başarıyla araca yüklendi.");
                                    return;
                                }
                                else
                                {
                                    upCar.lscSetData("Farm:Carry:Cow", 1);
                                    PedModel Pig1 = PedStreamer.Get(animal.ID);
                                    if (Pig1 != null)
                                        Pig1.Destroy();

                                    _farm.Item2.Animals.Remove(animal);

                                    SetCarCarry(upCar);
                                    MainChat.SendInfoChat(p, "[?] İnek başarıyla araca yüklendi.");
                                    return;
                                }
                            }

                        case 2:
                            if (upCar.Model != (uint)VehicleModel.TrailerSmall)
                            {
                                MainChat.SendErrorChat(p, "[HATA] Bu tip hayvnları yükleyebilmek için TrailerSmall'e ihtiyacınız var!");
                                return;
                            }
                            else
                            {
                                if (upCar.HasData("Farm:Carry:Stork"))
                                {
                                    int PigCount = upCar.lscGetdata<int>("Farm:Carry:Stork");
                                    if (PigCount >= 2) { MainChat.SendErrorChat(p, "[HATA] Bu araca daha fazla leylek yükleyemezsiniz!"); return; }
                                    PigCount += 1;

                                    PedModel Pig1 = PedStreamer.Get(animal.ID);
                                    if (Pig1 != null)
                                        Pig1.Destroy();

                                    _farm.Item2.Animals.Remove(animal);
                                    upCar.lscSetData("Farm:Carry:Stork", PigCount);
                                    SetCarCarry(upCar);
                                    MainChat.SendInfoChat(p, "[?] Leylek başarıyla araca yüklendi.");
                                    return;
                                }
                                else
                                {
                                    upCar.lscSetData("Farm:Carry:Stork", 1);
                                    PedModel Pig1 = PedStreamer.Get(animal.ID);
                                    if (Pig1 != null)
                                        Pig1.Destroy();

                                    _farm.Item2.Animals.Remove(animal);

                                    SetCarCarry(upCar);
                                    MainChat.SendInfoChat(p, "[?] Leylek başarıyla araca yüklendi.");
                                    return;
                                }
                            }
                        case 4:
                            if (upCar.Model != (uint)VehicleModel.TrailerSmall)
                            {
                                MainChat.SendErrorChat(p, "[HATA] Bu tip hayvnları yükleyebilmek için TrailerSmall'e ihtiyacınız var!");
                                return;
                            }
                            else
                            {
                                if (upCar.HasData("Farm:Carry:Hen"))
                                {
                                    int PigCount = upCar.lscGetdata<int>("Farm:Carry:Hen");
                                    if (PigCount >= 2) { MainChat.SendErrorChat(p, "[HATA] Bu araca daha fazla tavuk yükleyemezsiniz!"); return; }
                                    PigCount += 1;

                                    PedModel Pig1 = PedStreamer.Get(animal.ID);
                                    if (Pig1 != null)
                                        Pig1.Destroy();

                                    _farm.Item2.Animals.Remove(animal);
                                    upCar.lscSetData("Farm:Carry:Hen", PigCount);
                                    SetCarCarry(upCar);
                                    MainChat.SendInfoChat(p, "[?] Tavuk başarıyla araca yüklendi.");
                                    return;
                                }
                                else
                                {
                                    upCar.lscSetData("Farm:Carry:Hen", 1);
                                    PedModel Pig1 = PedStreamer.Get(animal.ID);
                                    if (Pig1 != null)
                                        Pig1.Destroy();

                                    _farm.Item2.Animals.Remove(animal);

                                    SetCarCarry(upCar);
                                    MainChat.SendInfoChat(p, "[?] Tavuk başarıyla araca yüklendi.");
                                    return;
                                }
                            }
                        case 6:
                            if (upCar.Model != (uint)VehicleModel.TrailerSmall)
                            {
                                MainChat.SendErrorChat(p, "[HATA] Bu tip hayvnları yükleyebilmek için TrailerSmall'e ihtiyacınız var!");
                                return;
                            }
                            else
                            {
                                if (upCar.HasData("Farm:Carry:Rabbit"))
                                {
                                    int PigCount = upCar.lscGetdata<int>("Farm:Carry:Rabbit");
                                    if (PigCount >= 2) { MainChat.SendErrorChat(p, "[HATA] Bu araca daha fazla tavşan yükleyemezsiniz!"); return; }
                                    PigCount += 1;

                                    PedModel Pig1 = PedStreamer.Get(animal.ID);
                                    if (Pig1 != null)
                                        Pig1.Destroy();

                                    _farm.Item2.Animals.Remove(animal);
                                    upCar.lscSetData("Farm:Carry:Rabbit", PigCount);
                                    SetCarCarry(upCar);
                                    MainChat.SendInfoChat(p, "[?] Tavşan başarıyla araca yüklendi.");
                                    return;
                                }
                                else
                                {
                                    upCar.lscSetData("Farm:Carry:Rabbit", 1);
                                    PedModel Pig1 = PedStreamer.Get(animal.ID);
                                    if (Pig1 != null)
                                        Pig1.Destroy();

                                    _farm.Item2.Animals.Remove(animal);

                                    SetCarCarry(upCar);
                                    MainChat.SendInfoChat(p, "[?] Tavşan başarıyla araca yüklendi.");
                                    return;
                                }
                            }

                        default:
                            return;
                    }
                    


            }
        }

        [Command("hayvansat")]
        public static void COM_SellAnimals(PlayerModel p)
        {
            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v == null) { MainChat.SendErrorChat(p, "[HATA] Yakınınızda bir araç bulunamadı!"); return; }
            if(v.LockState != VehicleLockState.Unlocked) { MainChat.SendErrorChat(p, "[HATA] Bu araç kilitli."); return; }

            int total = 0;
            if (v.HasData("Farm:Carry:Pig"))
            {
                total += v.lscGetdata<int>("Farm:Carry:Pig") * 1000;
                v.DeleteData("Farm:Carry:Pig");
            }
            if (v.HasData("Farm:Carry:Cow"))
            {
                total += v.lscGetdata<int>("Farm:Carry:Pig") * 2500;
                v.DeleteData("Farm:Carry:Pig");
            }
            if (v.HasData("Farm:Carry:Stork"))
            {
                total += v.lscGetdata<int>("Farm:Carry:Stork") * 500;
                v.DeleteData("Farm:Carry:Stork");
            }
            if (v.HasData("Farm:Carry:Hen"))
            {
                total += v.lscGetdata<int>("Farm:Carry:Hen") * 100;
                v.DeleteData("Farm:Carry:Hen");
            }
            if (v.HasData("Farm:Carry:Rabbit"))
            {
                total += v.lscGetdata<int>("Farm:Carry:Rabbit") * 250;
                v.DeleteData("Farm:Carry:Rabbit");
            }

            p.cash += total;
            p.updateSql();
            GlobalEvents.ClearVehicleTag(v);
            MainChat.SendInfoChat(p, "[?] Hayvanları satarak toplamda $" + total + " kazandınız.");
            return;

        }
        #endregion

        #region Farm Fruit Actions
        public static async Task<bool> PlaceFruit(PlayerModel p, int FruitType)
        {
            var farm = await checkFarm(p);
            if (farm == null) { MainChat.SendErrorChat(p, "[HATA] Yakınınızda bir çiftlik bulunamadı!"); return false; }
            var land = farm.Lands.Where(x => x.Type == 2).OrderBy(x => x.Position.Distance(p.Position)).FirstOrDefault();
            if (land == null) { MainChat.SendErrorChat(p, "[HATA] Meyve yerleştirebileceğiniz bir alan kurulu değil!"); return false; }
            if (land.Position.Distance(p.Position) > 20) { MainChat.SendErrorChat(p, "[HATA] Meyve bölgesine en fazla 20 metre uzaklığa hayvan yerleştirebilirsiniz."); return false; }
                        
            FarmModel.Fruit fruit = new FarmModel.Fruit();
            fruit.Dimension = p.Dimension;
            fruit.GrowthState = 0;
            fruit.Health = 100;
            fruit.FruitType = FruitType;
            fruit.Position = p.Position;            
            switch (FruitType)
            {
                case 1:
                    fruit.Model = "prop_bush_lrg_04c";
                    break;

                case 2:
                    fruit.Model = "prop_bush_neat_02";
                    break;

                case 3:
                    fruit.Model = "prop_bush_neat_05";
                    break;

                case 4:
                    fruit.Model = "prop_coral_bush_01";
                    break;

                case 5:
                    fruit.Model = "prop_veg_crop_04_leaf";
                    break;

                case 6:
                    fruit.Model = "prop_pot_plant_inter_03a";
                    break;

                case 7:
                    fruit.Model = "prop_coral_grass_02";
                    break;

                case 8:
                    fruit.Model = "prop_coral_grass_01";
                    break;

                default:
                    fruit.Model = "prop_bush_lrg_04c";
                    break;
            }

            fruit.ID = PropStreamer.Create(fruit.Model, p.Position, p.Rotation, p.Dimension, false, frozen: true).Id;
            fruit.TextLabel = TextLabelStreamer.Create(getFruitName(fruit), p.Position, p.Dimension, true, font: 0, streamRange: 2).Id;
            farm.Fruits.Add(fruit);

            MainChat.SendInfoChat(p, "[?] Meyve başarıyla yerleştirildi.");
            return true;
        }

        [Command("meyvetopla")]
        public async Task COM_GatherFruits(PlayerModel p)
        {
            var _farm = getNearFruit(p);
            var fruit = _farm.Item1;
            if (_farm.Item1 == null || _farm.Item2 == null) { MainChat.SendErrorChat(p, "[HATA] Yakınınızda bir çiftlik veya bitki bulunamadı!"); return; }
            var biz = await getBusinessById(_farm.Item2.Owner);
            if (biz.Item1 == null) { MainChat.SendErrorChat(p, "[HATA] Çiftliğin bağlı bulunduğu işyeri ile ilgili bir hata meydana geldi."); return; }

            if (!await CheckBusinessKey(p, biz.Item1)) { MainChat.SendErrorChat(p, "[HATA] Bu çifliğe erişim yetkiniz yok!"); return; }

            if(fruit.GrowthState < 90) { MainChat.SendErrorChat(p, "[HATA] Bu bitki henüz toplanabilecek olgunluğa ulaşmamış."); return; }


            // TODO ITEM VERDIRME
            LProp _fObj = PropStreamer.GetProp(fruit.ID);
            if (_fObj != null)
                _fObj.Delete();

            PlayerLabel _fLabel = TextLabelStreamer.GetDynamicTextLabel(fruit.TextLabel);
            if (_fLabel != null)
                _fLabel.Delete();

            _farm.Item2.Fruits.Remove(fruit);
            MainChat.SendInfoChat(p, "[?] Ürünü başarıyla topladınız.");
            return;
        }
        #endregion

        #region Test Commands
        [Command("fhtest")]
        public static void COM_FHTEST(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0)
                return;

            if (!Int32.TryParse(args[0], out int Type))
                return;

            PlaceAnimal(p, Type);
            return;
        }

        [Command("fftest")]
        public static void COM_FFTEST(PlayerModel p, params string[] args)
        {
            if (args.Length <= 0)
                return;

            if (!Int32.TryParse(args[0], out int Type))
                return;

            PlaceFruit(p, Type);
            return;
        }

        [Command("fltest")]
        public static void COM_FLTEST(PlayerModel p, params string[] args)
        {
           
                if (args.Length <= 0)
                    return;

                if (!Int32.TryParse(args[0], out int Type))
                    return;

            var farm = getNearFarm(p);
            if (farm == null)
                return;

            farm.Lands.Add(new FarmModel.Land()
            {
                Health = 100,
                Position = p.Position,
                Range = 1000,
                TextLabelID = TextLabelStreamer.Create("Çiftlik Alanı", p.Position, p.Dimension, true, font: 0).Id,
                Type = Type
            });

                return;
            
        }
        #endregion
    }
}*/
