using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using outRp.Chat;
using outRp.Globals;
using outRp.Models;
using outRp.OtherSystem.NativeUi;
using outRp.OtherSystem.Textlabels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace outRp.OtherSystem.LSCsystems
{
    public class VehicleModify : IScript
    {

        #region Colors
        [AsyncClientEvent("Vehicle:PrimaryColorShow")]
        public void Vehicle_SetFirstColorRGBAWant(PlayerModel p, VehModel v)
        {
            if (!v.Exists)
                return;
            Inputs.SendTypeInput(p, "Araç rengini seçin", "Vehicle:SetPrimaryRGBA", v.sqlID.ToString());
        }
        [AsyncClientEvent("Vehicle:SetPrimaryRGBA")]
        public void Vehicle_SetFirstColorRGBA(PlayerModel p, string colorstring, string vehid)
        {
            string[] cc = colorstring.Split(",");
            if (cc.Length <= 3) { MainChat.SendErrorChat(p, "[错误] 无效RGBA代码: 255,0,0,1"); return; }

            int r; int g; int b; int a; bool isR = Int32.TryParse(cc[0], out r); bool isG = Int32.TryParse(cc[1], out g);
            bool isB = Int32.TryParse(cc[2], out b); bool isA = Int32.TryParse(cc[3], out a);

            int vid; bool isVid = Int32.TryParse(vehid, out vid);
            if (!isVid)
                return;

            if (!isR || !isG || !isB || !isA) { MainChat.SendErrorChat(p, "[错误] 无效RGBA代码: 255,0,0,1"); return; }

            Rgba color = new Rgba((byte)r, (byte)g, (byte)b, (byte)a);

            VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(vid);
            if (v == null)
                return;

            v.SetModKitAsync(1);
            v.SetPrimaryColorRgbAsync(color);
            return;
        }

        [AsyncClientEvent("Vehicle:SetFirstColor")]
        public void Vehicle_SetFirstColor(PlayerModel p, VehModel v, int color)
        {
            if (!v.Exists)
                return;
            v.SetModKitAsync(1);
            v.SetPrimaryColorAsync((byte)color);
            return;
        }

        [AsyncClientEvent("Vehicle:SecondayColorShow")]
        public void Vehicle_SetSecondColorRGBAWant(PlayerModel p, VehModel v)
        {
            if (!v.Exists)
                return;
            Inputs.SendTypeInput(p, "选择车辆副颜色", "Vehicle:SetSecondaryRGBA", v.sqlID.ToString());
        }

        [AsyncClientEvent("Vehicle:SetSecondaryRGBA")]
        public void Vehicle_SetSecondColorRGBA(PlayerModel p, string colorstring, string vehid)
        {
            string[] cc = colorstring.Split(",");
            if (cc.Length <= 3) { MainChat.SendErrorChat(p, "[错误] 无效RGBA代码: 255,0,0,1"); return; }

            int r; int g; int b; int a; bool isR = Int32.TryParse(cc[0], out r); bool isG = Int32.TryParse(cc[1], out g);
            bool isB = Int32.TryParse(cc[2], out b); bool isA = Int32.TryParse(cc[3], out a);

            int vid; bool isVid = Int32.TryParse(vehid, out vid);
            if (!isVid)
                return;

            if (!isR || !isG || !isB || !isA) { MainChat.SendErrorChat(p, "[错误] 无效RGBA代码: 255,0,0,1"); return; }

            Rgba color = new Rgba((byte)r, (byte)g, (byte)b, (byte)a);

            VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(vid);
            if (v == null)
                return;

            v.SetModKitAsync(1);
            v.SetSecondaryColorRgbAsync(color);
            return;
        }

        [AsyncClientEvent("Vehicle:SetSecondColor")]
        public void Vehicle_SetSecondColor(PlayerModel p, VehModel v, int color)
        {
            if (!v.Exists)
                return;
            v.SetModKitAsync(1);
            v.SetSecondaryColorAsync((byte)color);
            return;
        }
        [AsyncClientEvent("Vehicle:SetInteriorColor")]
        public void Vehicle_InteriorColor(PlayerModel p, VehModel v, int color)
        {
            if (!v.Exists)
                return;
            v.SetModKitAsync(1);
            v.SetInteriorColorAsync((byte)color);
            return;
        }
        [AsyncClientEvent("Vehicle:SetDashboardColor")]
        public void Vehicle_DashboardColor(PlayerModel p, VehModel v, int color)
        {
            if (!v.Exists)
                return;
            v.SetModKitAsync(1);
            v.SetDashboardColorAsync((byte)color);
            return;
        }
        [AsyncClientEvent("Vehicle:SetHeadlightColor")]
        public void Vehicle_HeadlightColor(PlayerModel p, VehModel v, int color)
        {
            if (!v.Exists)
                return;
            v.SetModKitAsync(1);
            v.SetHeadlightColorAsync((byte)color);
            v.settings.HeadlightColor = ((byte)color);
            return;
        }
        [AsyncClientEvent("Vehicle:SetPearlColor")]
        public void Vehicle_PearlColor(PlayerModel p, VehModel v, int color)
        {
            if (!v.Exists)
                return;
            v.SetModKitAsync(1);
            v.SetPearlColorAsync((byte)color);
            return;
        }

        [AsyncClientEvent("Vehicle:TireSmokeWant")]
        public void Vehicle_TireColorWant(PlayerModel p, VehModel v)
        {
            if (!v.Exists)
                return;
            Inputs.SendTypeInput(p, "轮胎烟色", "Vehicle:TireSmokeSet", v.sqlID.ToString());
        }

        [AsyncClientEvent("Vehicle:TireSmokeSet")]
        public void Vehicle_TireColor(PlayerModel p, string colorstring, string vehid)
        {
            string[] cc = colorstring.Split(",");
            if (cc.Length <= 3) { MainChat.SendErrorChat(p, "[错误] 无效RGBA代码: 255,0,0,1"); return; }

            int r; int g; int b; int a; bool isR = Int32.TryParse(cc[0], out r); bool isG = Int32.TryParse(cc[1], out g);
            bool isB = Int32.TryParse(cc[2], out b); bool isA = Int32.TryParse(cc[3], out a);

            int vid; bool isVid = Int32.TryParse(vehid, out vid);
            if (!isVid)
                return;

            if (!isR || !isG || !isB || !isA) { MainChat.SendErrorChat(p, "[错误] 无效RGBA代码: 255,0,0,1"); return; }

            Rgba color = new Rgba((byte)r, (byte)g, (byte)b, (byte)a);

            VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(vid);
            if (v == null)
                return;

            v.SetModKitAsync(1);
            v.CustomTires = (true);
            v.SetTireSmokeColorAsync(color);

            return;
        }
        [AsyncClientEvent("Vehicle:SetWheelColor")]
        public void Vehicle_WheelColor(PlayerModel p, VehModel v, int color)
        {
            if (!v.Exists)
                return;
            v.SetModKitAsync(1);
            v.SetWheelColorAsync((byte)color);
            return;
        }
        [AsyncClientEvent("Vehicle:SetWindowTint")]
        public void Vehicle_WindowTint(PlayerModel p, VehModel v, int type)
        {
            if (!v.Exists)
                return;
            v.SetModKitAsync(1);
            v.SetWindowTintAsync((byte)type);
            return;
        }
        #endregion

        [AsyncClientEvent("Vehicle:SetWheel")]
        public void Vehicle_Wheels(PlayerModel p, VehModel v, int wheel)
        {
            if (!v.Exists)
                return;
            v.SetModKitAsync(1);
            v.SetWheelsAsync((byte)v.settings.WheelType, (byte)wheel);
            v.SetRearWheelAsync((byte)wheel);
            return;
        }

        [AsyncClientEvent("Vehicle:SetModify")]
        public void Vehicle_Modify(PlayerModel p, VehModel v, int type, int value)
        {
            if (!v.Exists)
                return;
            v.SetModKitAsync(1);
            v.SetModAsync((byte)type, (byte)value);
            return;
        }

        [AsyncClientEvent("Vehicle:SetRoofModify")]
        public void Vehicle_RoofModify(PlayerModel p, VehModel v, int value)
        {
            if (!v.Exists)
                return;
            v.SetRoofLiveryAsync((byte)value);
            //v.SetRoofOpenedAsync(!v.RoofOpened);
        }

        // Vehicle Repair 
        [AsyncClientEvent("Vehicle:MakeRepair")]
        public async Task Vehicle_MakeRepair(PlayerModel p, VehModel v)
        {
            if (!v.Exists)
                return;
            if (await Props.Business.GetPlayerBusinessType(p, ServerGlobalValues.mechanicBusiness) == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            Crate x = null;
            foreach (var c in CrateEvents.serverCrates)
            {
                if (c.pos.Distance(p.Position) < 10f) { x = c; break; }
            }
            if (x == null) { MainChat.SendErrorChat(p, "[错误] 附近没有改装点!"); return; }
            if (x.type != 1) { MainChat.SendErrorChat(p, "[错误] 附近没有改装点!"); return; }
            if (p.businessStaff != x.owner) { MainChat.SendErrorChat(p, "[错误] 无权使用!"); return; }
            if (x.stock < 10) { MainChat.SendErrorChat(p, "[错误] 没有货了!"); return; }


            if (v == null) { MainChat.SendErrorChat(p, "[错误] 无效车辆!"); return; }
            if (p.cash <= x.settings.stockCost * 10) { MainChat.SendErrorChat(p, "[错误] 您没有足够的钱!"); return; }
            (BusinessModel, Marker, PlayerLabel) biz = await Props.Business.getBusinessById(x.owner);
            if (biz.Item1 == null || biz.Item2 == null || biz.Item3 == null) { MainChat.SendErrorChat(p, "[错误] 产业发生错误!"); return; }

            v.Repair();
            p.cash -= x.settings.stockCost * 10;
            await p.updateSql();
            biz.Item1.vault += x.settings.stockCost * 10;
            await biz.Item1.Update(biz.Item2, biz.Item3);


            x.stock -= 10;
            x.Update();

            MainChat.SendInfoChat(p, "[?] 已修理车辆, 花费 10 库存, 总计收费: " + x.settings.stockCost.ToString());

            await Task.Delay(1000);
            if (v == null)
                return;

            GlobalEvents.RepairVehicle(v);
        }

        // GoBack OldModify
        [AsyncClientEvent("Vehicle:ModifyPageClose")]
        public async void Vehicle_LoadOldModify(PlayerModel p, VehModel v)
        {
            if (!v.Exists)
                return;


            if (v.settings.ModifiyData.Contains("2628_"))
                v.AppearanceData = (v.settings.ModifiyData.Replace("2628_", "2802_"));
            else if (v.settings.ModifiyData.Contains("2802_"))
                v.AppearanceData = (v.settings.ModifiyData);
            else v.AppearanceData = ("2802_" + v.settings.ModifiyData);

            p.DeleteData("InModify");
            Core.Logger.WriteLogData(Core.Logger.logTypes.Modifiye, p.characterName + " 关闭了改装菜单.");
        }

        //Calculate Modifiy Cost 

        class CModify
        {
            public int ID { get; set; } = 0;
            public byte Value { get; set; } = 0;
        }

        [AsyncClientEvent("Vehicle:TryToMakeModify")]
        public async Task ModifyCheck(PlayerModel p, VehModel v)
        {
            if (!v.Exists)
                return;
            if (await Props.Business.GetPlayerBusinessType(p, ServerGlobalValues.mechanicBusiness) == false) { MainChat.SendErrorChat(p, CONSTANT.ERR_PermissionError); return; }
            Crate x = null;
            foreach (var c in CrateEvents.serverCrates)
            {
                if (c.pos.Distance(p.Position) < 10f) { x = c; break; }
            }
            if (x == null) { MainChat.SendErrorChat(p, "[错误] 附近没有改装点!"); return; }
            if (x.type != 1) { MainChat.SendErrorChat(p, "[错误] 附近没有改装点!"); return; }
            if (p.businessStaff != x.owner) { MainChat.SendErrorChat(p, "[错误] 无权使用!"); return; }

            int cost = await Vehicle_CalculateModifyCost(p, v);
            if (!p.Exists)
                return;
            MainChat.SendInfoChat(p, "[改装] 消耗库存: " + cost.ToString());
            Inputs.SendButtonInput(p, "消耗库存: " + cost.ToString(), "Vehicle:ModifyApply_1", v.sqlID.ToString() + "," + cost.ToString() + "," + p.sqlID.ToString() + "," + (x.settings.stockCost * cost).ToString());

        }
        [AsyncClientEvent("Vehicle:ModifyApply_1")]
        public static void ModifyApply(PlayerModel p, bool selection, string otherVal)
        {
            if (!selection) { MainChat.SendInfoChat(p, "[改装] 您取消了改装."); return; }

            Inputs.SendTypeInput(p, "输入收费玩家ID", "Vehicle:ModifyApply_2", otherVal);
        }

        [AsyncClientEvent("Vehicle:ModifyApply_2")]
        public void ModifyApply_1(PlayerModel p, string id, string otherVal)
        {
            bool istIDOk = Int32.TryParse(id, out int tID);
            if (!istIDOk) { Inputs.SendTypeInput(p, "输入收费玩家ID", "Vehicle:ModifyApply_2", otherVal); return; }

            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(tID);
            if (target == null) { Inputs.SendTypeInput(p, "输入收费玩家ID", "Vehicle:ModifyApply_2", otherVal); return; }
            if (target.Position.Distance(p.Position) > 10) { MainChat.SendErrorChat(p, "[错误] 您离玩家太远!"); return; }

            string[] _otherVal = otherVal.Split(",");
            Inputs.SendButtonInput(target, "是否以 $" + _otherVal[3] + " 价格完成改装?", "Vehicle:ModifyTarget", otherVal);
            MainChat.SendInfoChat(p, "[?] 已发送付款请求, 请等待客户付款.");
            Core.Logger.WriteLogData(Core.Logger.logTypes.Modifiye, p.characterName + " 发送了付款请求至 " + target.characterName + " - " + target.sqlID.ToString() + " 费用: " + _otherVal[3]);
            return;
        }

        [AsyncClientEvent("Vehicle:ModifyTarget")]
        public async Task ModifyApply_Last(PlayerModel p, bool selection, string otherValue)
        {
            string[] _otherValue = otherValue.Split(",");
            if (_otherValue.Length <= 2) { MainChat.SendErrorChat(p, "[!] 发生了错误!"); return; }

            // v.sqlID.ToString() + "," + cost.ToString() + "," + p.sqlID.ToString() + "," + (x.settings.stockCost * cost).ToString()
            bool isVehID = Int32.TryParse(_otherValue[0], out int vehID);
            bool isCost = Int32.TryParse(_otherValue[1], out int cost);
            bool istID = Int32.TryParse(_otherValue[2], out int tID);
            bool isPrice = Int32.TryParse(_otherValue[3], out int Price);
            if (!isVehID || !isCost || !istID || !isPrice) { MainChat.SendErrorChat(p, "[!] 发生了错误!"); return; }

            PlayerModel target = GlobalEvents.GetPlayerFromSqlID(tID);
            if (target == null) { MainChat.SendErrorChat(p, "[错误] 无效改装师!"); return; }

            if (!selection)
            {
                MainChat.SendInfoChat(p, "[?] 您拒绝了付款.");
                MainChat.SendErrorChat(target, "[?] 客户拒绝了付款.");
                Core.Logger.WriteLogData(Core.Logger.logTypes.Modifiye, p.characterName + " 拒绝了付款.");
                return;
            }


            if (p.Position.Distance(target.Position) > 10)
            {
                MainChat.SendErrorChat(p, "[错误] 您离改装师太远!");
                MainChat.SendErrorChat(target, "[错误] 客户离您太远.");
                return;
            }

            VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(vehID);
            if (v == null || v.Position.Distance(target.Position) > 10)
            {
                MainChat.SendErrorChat(p, "[错误] 改装师离车辆太远!");
                MainChat.SendErrorChat(target, "[错误] 您离车辆太远.");
                return;
            }



            if (await Props.Business.GetPlayerBusinessType(target, ServerGlobalValues.mechanicBusiness) == false) { MainChat.SendErrorChat(target, CONSTANT.ERR_PermissionError); return; }
            Crate x = null;
            foreach (var c in CrateEvents.serverCrates)
            {
                if (c.pos.Distance(target.Position) < 10f) { x = c; break; }
            }
            if (x == null) { MainChat.SendErrorChat(target, "[错误] 您不在改装点!"); return; }
            if (x.type != 1) { MainChat.SendErrorChat(target, "[错误] 您不在改装点!"); return; }
            if (target.businessStaff != x.owner) { MainChat.SendErrorChat(target, "[错误] 无权使用!"); return; }
            if (x.stock < cost) { MainChat.SendErrorChat(target, "[错误] 改装店库存不足了!"); MainChat.SendErrorChat(p, "[错误] 改装店库存不足了!"); return; }

            if (p.cash < Price) { MainChat.SendErrorChat(p, "[错误] 您没有足够的钱!"); MainChat.SendInfoChat(target, "[错误] 客户没有足够的钱!"); return; }

            (BusinessModel, Marker, PlayerLabel) biz = await Props.Business.getBusinessById(x.owner);
            if (biz.Item1 == null || biz.Item2 == null || biz.Item3 == null) { MainChat.SendErrorChat(p, "[错误] 无效产业!"); MainChat.SendErrorChat(target, "[错误] 无效产业."); return; }

            p.cash -= Price;
            await p.updateSql();
            biz.Item1.vault += Price;
            await biz.Item1.Update(biz.Item2, biz.Item3);

            x.stock -= cost;
            x.Update();

            v.settings.ModifiyData = v.settings.WantModifyData;
            v.settings.WantModifyData = "a";
            await v.SetAppearanceDataAsync(v.settings.ModifiyData);
            v.Update();

            MainChat.SendInfoChat(p, "[?] 已支付改装费用.");
            MainChat.SendInfoChat(target, "[?] 客户支付了改装费用.");
            Core.Logger.WriteLogData(Core.Logger.logTypes.Modifiye, p.characterName + " 支付了改装费用.");
            return;
        }





        // Boya için 

        public async Task<int> Vehicle_CalculateModifyCost(PlayerModel p, VehModel v)
        {
            if (!v.Exists)
                return 0;
            //v.settings.WantModifyData = v.AppearanceData;
            v.settings.WantModifyData = v.AppearanceData;

            byte oldDashboardColor = 0; byte oldHeadlightColor = 0; byte oldInteriorColor = 0; byte oldPearlColor = 0;
            Rgba oldTireColor = new Rgba(0, 0, 0, 0); byte oldWheelColor = 0; byte oldWindowTint = 0; byte WheelType = 0;

            bool oldFirstColorRGB = false; byte oldFirstColor = 0; Rgba oldFirstColorRGBA = new Rgba(0, 0, 0, 0);
            bool oldSecondColorRGB = false; byte oldSecondColor = 0; Rgba oldSeconColorRGBA = new Rgba(0, 0, 0, 0);

            List<CModify> CurrentMod = new List<CModify>();

            for (int i = 0; i <= 48; i++)
            {
                if (i == 17 || i == 19 || i == 21)
                    continue;

                byte modValue = v.GetMod((byte)i);

                CModify _currentMod = new CModify()
                {
                    ID = i,
                    Value = modValue,
                };

                CurrentMod.Add(_currentMod);
            }

            oldDashboardColor = await v.GetDashboardColorAsync();
            oldHeadlightColor = await v.GetHeadlightColorAsync();
            oldInteriorColor = await v.GetInteriorColorAsync();
            oldPearlColor = await v.GetPearlColorAsync();
            //oldFirstColor = await v.GetPearlColorAsync();
            //oldSecondColor = await v.GetSecondaryColorAsync();
            oldTireColor = await v.GetTireSmokeColorAsync();
            oldWheelColor = await v.GetWheelColorAsync();
            oldWindowTint = await v.GetWindowTintAsync();
            WheelType = await v.GetWheelVariationAsync();

            oldFirstColorRGB = await v.IsPrimaryColorRgbAsync();

            if (oldFirstColorRGB)
            {
                oldFirstColorRGBA = await v.GetPrimaryColorRgbAsync();
            }
            else
            {
                oldFirstColor = await v.GetPrimaryColorAsync();
            }

            oldSecondColorRGB = await v.IsSecondaryColorRgbAsync();

            if (oldSecondColorRGB)
            {
                oldSeconColorRGBA = await v.GetSecondaryColorRgbAsync();
            }
            else
            {
                oldSecondColor = await v.GetSecondaryColorAsync();
            }

            // Eski Modifiye Datasını yükle.

            v.AppearanceData = v.settings.ModifiyData;

            List<CModify> oldModify = new List<CModify>();

            for (int i = 0; i <= 48; i++)
            {
                if (i == 17 || i == 19 || i == 21)
                    continue;

                byte modValue = v.GetMod((byte)i);


                CModify _oldModify = new CModify()
                {
                    ID = i,
                    Value = modValue,
                };

                oldModify.Add(_oldModify);
            }

            // Cost Hesaplama

            int totalCost = 0;

            if (oldDashboardColor != v.DashboardColor && v.DashboardColor != 0)
                totalCost += 80;

            if (oldHeadlightColor != v.HeadlightColor && v.HeadlightColor != 0)
                totalCost += 160;

            if (oldInteriorColor != v.InteriorColor && v.InteriorColor != 0)
                totalCost += 80;

            if (oldPearlColor != v.PearlColor && v.PearlColor != 0)
                totalCost += 20;

            if (oldWheelColor != v.WheelColor)
                totalCost += 64;

            if (oldWindowTint != v.WindowTint)
                totalCost += 80;

            if (oldTireColor != v.TireSmokeColor)
                totalCost += 200;

            if (WheelType != v.WheelVariation)
            {
                totalCost += 180;
            }


            // Birincil - ikincil Renk

            if (oldFirstColorRGB)
            {

                totalCost += 5; // Önceki ve yeni olan RGB Ama değişti renk                    

            }
            else
            {
                totalCost += 5; // Önceki veya yeni olan RGB %100 değişiklik
                //if(v.PrimaryColor == 120) { totalCost += 200; }
            }




            if (oldSecondColorRGB)
            {
                totalCost += 5;
            }
            else
            {
                totalCost += 5;
                //if(v.SecondaryColor == 120) { totalCost += 200; }
            }

            foreach (CModify x in CurrentMod)
            {
                CModify oldData = oldModify.Find(y => y.ID == x.ID);
                if (oldData == null)
                    continue;

                if (oldData.Value != x.Value)
                {
                    if (x.Value > 0)
                    {
                        totalCost += GetModifyCost(x.ID, x.Value);
                    }

                }
            }

            return totalCost;


        }


        static int GetModifyCost(int ID, int CurrValue)
        {
            // Eğer parça sökülmüşse cost yok!
            if (CurrValue <= 0)
                return 0;

            switch (ID)
            {
                case 0:
                    return 120; // Spoiler

                case 1:
                    return 120; // Ön Tampon

                case 2:
                    return 120; // Arka Tampon

                case 3:
                    return 120; // Yan Etekler

                case 4:
                    return 60; // Egzos

                case 5:
                    return 16; // Çerçeve / Frame

                case 6:
                    return 16; // Grille

                case 7:
                    return 120; // bonnet

                case 8:
                    return 60; // Wing Left

                case 9:
                    return 60; // Right Wing

                case 10:
                    return 120; // roof

                case 11:
                    return (350 * CurrValue); // engine

                case 12:
                    return (120 * CurrValue); // Brakes

                case 13:
                    return (120 * CurrValue); // Transmission

                case 14:
                    return 16; // Horn

                case 15:
                    return (60 * CurrValue); // Suspansion

                case 16:
                    return (800 * CurrValue); // Armor

                case 18:
                    return 240; // turbo

                case 22:
                    return 160; // Xenon Far

                case 27: // Trim Design
                case 28: // Ormanents
                case 30: // Dial Desing
                case 31: // Door Interior
                    return 24;

                case 32: // Seats
                    return 40;

                case 33: // Steering Wheel
                case 34: // Shift level
                case 37: // Trunk
                    return 40;

                case 38: // Hidrolik
                    return 200;

                case 39: // EngineBlock
                    return 80;

                case 40: // Air Filter
                    return 80;

                case 41: // Strut Bar
                    return 80;

                case 42: // ArchCover
                case 43: // Anten
                case 44: // Exterior Part
                case 45: // Tank (Petrol)
                case 46: // Door
                    return 16;

                case 48: // Livery
                    return 320;

                default:     // Diğer Herşey
                    return 60;
            }


        }


    }
}
