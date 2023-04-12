

/*namespace outRp.OtherSystem.LSCsystems
{
    public class Flatbed : IScript
    {
        public class Const
        {
            public string TowwedCar = "Flatbed:TowwedCar";
        }

        [Command("uploadcar")]
        public static void COM_UploadCar(PlayerModel p, params string[] args)
        {
            VehModel v = Vehicle.VehicleMain.getNearVehFromPlayer(p);
            if (v == null) { MainChat.SendErrorChat(p, "[错误] Yakınınızda bir araç bulunamadı!"); return; }
            if(v.Model != (uint)VehicleModel.Flatbed) { MainChat.SendErrorChat(p, "[HATA] Bu komutu kullanabilmek için ");  }
        }


        public static (bool, List<Position>) CheckTow(uint Vehmodel)
        {
            List<Position> Pos = new List<Position>();
            switch (Vehmodel)
            {
                case (uint)VehicleModel.Flatbed:
                    
                    return (true, Pos);
                default:
                    return (false, Pos);
            }
        }

        [Command("flattest")]
        public static void COM_FlatTest(PlayerModel p, params string[] args)
        {
            if(!Int32.TryParse(args[0], out int oCar) || !Int32.TryParse(args[1], out int tCar) || !Int32.TryParse(args[2], out int short1) || !Int32.TryParse(args[3], out int short2) ||
                !Int32.TryParse(args[4], out int xPos) ||
                !Int32.TryParse(args[5], out int yPos) || !Int32.TryParse(args[6], out int zPos))
            {
                MainChat.SendErrorChat(p, "[ARGÜMAN HATASI]");
                return;
            }

            VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(oCar);
            VehModel t = Vehicle.VehicleMain.getVehicleFromSqlId(tCar);

            if (v == null || t == null) { MainChat.SendErrorChat(p, "[ARAÇ Bulunamadı!]"); return; }
            if (t.HasData("attached"))
            {
                //Position pos = Core.Core.getBackPos(v.Position, v.Rotation, 5);
                var pos = v.Position;
                pos.X += 3;
                t.Detach();           
                t.Position = pos;
                p.SendChatMessage("Okay ----");
                t.DeleteData("attached");
            }
            else
            {
                t.AttachToEntity(v, (short)short1, (short)short2, new Position(xPos, yPos, zPos), new Rotation(0, 0, 0), true, false);
                p.SendChatMessage("OKay ++++");
                t.SetData("attached", true);
            }
        }

    }
}*/
