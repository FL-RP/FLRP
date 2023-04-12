using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using AltV.Net.Resources.Chat.Api;
using Newtonsoft.Json;
using outRp.Chat;
using outRp.Globals;
using outRp.Models;

namespace outRp.Tutorial;

public class TutorialMain : IScript
{
    public static List<Position> TutPos = new List<Position>()
    {
        new Position(-1024.444f, -2742.2769f,20.180908f), // 第一个
        new Position(-1007.578f, -2750.1626f, 13.744385f), // 出口了
        new Position(-754.33844f, -2292.633f, 12.413208f), // 酒店
        new Position(-875.7758f, -2278.7605f, 6.2630615f), // 停车场
        new Position(-740.189f, -2279.1296f, 13.053467f), // 主要教程点
        new Position(-531.03296f, -1208.0703f, 18.175781f), // 加油站
        new Position(-817.543f, -1079.29f, 11.0315f), // 服装店
        new Position(-813.8505f, -183.45494f, 37.5531f), // 理发店
        new Position(-1285.0813f, -566.33405f, 31.706177f), // 纳税点
        new Position(-1226.6505f, -902.2813f, 12.278442f) // 维斯普奇 商店
    };

    [AsyncClientEvent("Tutorial:Run")]
    public static async Task Tutorial_Run(PlayerModel p, string step)
    {
        Alt.Log("触发了");
        Alt.Log(p.sqlID + " 执行了 步骤" + step);
        switch (step)
        {
            case "0": // 出机场 1
            {
                if (p.isFinishTut == 0)
                {
                    p.isFinishTut = 1;
                    p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}欢迎来到{fc5e03}洛圣都{FFFFFF}!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}请跟随指引完成新手教程，否则很难游戏!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}教程全程靠地图标记点运行，所以请不要自行设置其他地图标记点!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}请先跟随指引出登机口!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}您在新手教程期间是受保护的!您不需要参与扮演!");

                    GlobalEvents.CheckpointCreate(p, TutorialMain.TutPos[1], 1, 2, new Rgba(255, 0, 0, 0), "Tutorial:Run", "1");
                    break;
                }   
                else 
                { 
                    MainChat.SendErrorChat(p, "您已经完成此新手教程了!");
                    break; 
                }
            }
            case "1": // 出机场 2 完全
            {
                if (p.isFinishTut == 1)
                {
                    if (p.tutCar == 0)
                    {
                        IVehicle newV = Alt.CreateVehicle((uint)VehicleModel.Asbo,
                            new Position(-1055.5f, -2647.38f, 13.3905f), new Rotation(0.00731635f, 0, -0.0341067f));
                        VehModel veh = (VehModel)newV;
                        veh.sqlID = await Database.DatabaseMain.CreateVehicle((VehModel)newV);
                        await veh.SetAppearanceDataAsync("2802_BQUAAW8AACAAAIYUAAAPBgzKlYAAAAAAAAAAAAAAAA==");
                        Random pR = new Random();
                        char[] chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
                        veh.NumberplateText = pR.Next(0, 9).ToString() + chars[pR.Next(chars.Length)] +
                                              chars[pR.Next(chars.Length)] + veh.sqlID.ToString() +
                                              chars[pR.Next(chars.Length)];
                        veh.PrimaryColorRgb = new Rgba(255, 255, 255, 255);
                        veh.SecondaryColorRgb = new Rgba(255, 255, 255, 255);
                        veh.owner = p.sqlID;
                        veh.defaultTax = 8;
                        veh.fuelConsumption = 2;
                        veh.price = 500;
                        veh.inventoryCapacity = 130;
                        veh.maxFuel = 55;
                        veh.currentFuel = 10;
                        veh.LockState = VehicleLockState.Locked;

                        List<ServerItems> items = new List<ServerItems>();
                        ServerItems item = Items.LSCitems.Find(x => x.ID == 20);
                        item.amount = 1;
                        items.Add(item);
                        string json = JsonConvert.SerializeObject(items);
                        veh.vehInv = json;
                        veh.settings.ModifiyData = veh.AppearanceData;
                        veh.settings.SecurityLevel = 1;
                        veh.Position = new Position(-1055.5f, -2647.38f, 13.3905f);
                        
                        p.tutCar = veh.sqlID;
                        p.isFinishTut = 2;
                        await p.updateSql();
                        p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                        MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}我有{fc5e03}车{FFFFFF}了!");
                        MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}为了保证扮演者正常的游戏， 系统为您赠送了一辆车!");
                        MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}请按T输入 /cgps " + veh.sqlID + " 定位您的车辆");
                        
                        veh.Update();
                    }
                    else
                    {
                        p.isFinishTut = 2;
                        await p.updateSql();
                        p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                        MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}我有{fc5e03}车{FFFFFF}了!");
                        MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}为了保证扮演者正常的游戏， 系统为您赠送过一辆车!");
                        MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}请按T输入 /mycars 查看车辆编号并使用 /cgps 车辆编号 定位您的车辆");  
                    }
                    break;
                }   
                else 
                { 
                    MainChat.SendErrorChat(p, "您已经完成此新手教程了!");
                    break; 
                }
            } 
            case "3": // 找到车 进车 1
            {
                if (p.isFinishTut == 3)
                {
                    p.isFinishTut = 4;
                    p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}我有{fc5e03}车{FFFFFF}了!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}您成功的找到了自己的车!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}请先按{fc5e03}N键{FFFFFF}解锁车辆, 然后按{fc5e03}F键{FFFFFF}进入车辆主驾驶!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: 请进入您的车辆! (按G键可以进入其他座位)");
                    // GlobalEvents.CheckpointCreate(p, Tutorial_Main.TutPos[1], 44, 1, new Rgba(255, 255, 255, 100), "Tutorial:Run", "2");
                    break;
                }   
                else 
                { 
                    MainChat.SendErrorChat(p, "您已经完成此新手教程了!");
                    break; 
                }
            }    
            case "7": // 到酒店
            {
                if (p.isFinishTut == 7)
                {
                    p.isFinishTut = 8;
                    p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}我有{fc5e03}车{FFFFFF}了!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}您已到达新的教程点!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}在此之前, 请先去停车!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}系统已经为您定位了附近的停车场!");
                    GlobalEvents.CheckpointCreate(p, TutorialMain.TutPos[3], 1, 2, new Rgba(255, 0, 0, 0), "Tutorial:Run", "8");
                    break;
                }   
                else 
                { 
                    MainChat.SendErrorChat(p, "您已经完成此新手教程了!");
                    break; 
                }
            }  
            case "8": // 到停车场
            {
                if (p.isFinishTut == 8)
                {
                    p.isFinishTut = 9;
                    p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}我有{fc5e03}车{FFFFFF}了!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}您已到达停车场!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}请输入 {fc5e03}/vmenu {FFFFFF}打开车辆菜单!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}您可以在{fc5e03}车辆菜单{FFFFFF}看见{fc5e03}泊车菜单-购买泊车位{FFFFFF}!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}使用上下键选择{fc5e03}泊车菜单-购买泊车位{FFFFFF}, 按回车购买一个停车位!");
                    // GlobalEvents.CheckpointCreate(p, Tutorial_Main.TutPos[1], 44, 1, new Rgba(255, 255, 255, 100), "Tutorial:Run", "8");
                    break;
                }   
                else 
                { 
                    MainChat.SendErrorChat(p, "您已经完成此新手教程了!");
                    break; 
                }
            }   
            case "11": // 到酒店
            {
                if (p.isFinishTut == 11)
                {
                    p.isFinishTut = 12;
                    p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}基本指令");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}为了进行扮演，我们增设了许多指令和按键供玩家进行扮演!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}现在请按T {fc5e03}随便说句话");
                    // GlobalEvents.CheckpointCreate(p, Tutorial_Main.TutPos[1], 44, 1, new Rgba(255, 255, 255, 100), "Tutorial:Run", "8");
                    break;
                }   
                else 
                { 
                    MainChat.SendErrorChat(p, "您已经完成此新手教程了!");
                    break; 
                }
            }   
            case "20": // 下一个点 去加油站
            {
                if (p.isFinishTut == 20)
                {
                    p.isFinishTut = 21;
                    p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}车辆加油");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}服务器为车辆增设了油箱系统, 如果您的车没有油了, 车是会停止的!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}现在请使用 {fc5e03}/fillcar{FFFFFF} 指令给您的车加油");
                    // GlobalEvents.CheckpointCreate(p, Tutorial_Main.TutPos[1], 44, 1, new Rgba(255, 255, 255, 100), "Tutorial:Run", "8");
                    break;
                }   
                else 
                { 
                    MainChat.SendErrorChat(p, "您已经完成此新手教程了!");
                    break; 
                }
            } 
            case "22": // 下一个点 去加油站
            {
                if (p.isFinishTut == 22)
                {
                    p.isFinishTut = 23;
                    p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}服装店");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}按E进入服装店, 然后输入 {fc5e03}/buyclothes{FFFFFF} 购买服装!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}注意, WASD和鼠标滚轮是可以浏览角色视角的, WASD用以调整视角和角色方向, 鼠标滚轮用以放大缩小视角");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}注意, {fc5e03}您点击购买会购买当前正在浏览的服装, 是单件服装噢, 请单个单个购买, 不是搭配好了成套购买的!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}注意, {fc5e03}服装完成购买后, 会进入您的背包, 不会被立即穿上, 您需要自己按I打开背包, 右键服装物品进行使用!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}现在, 购买服装吧!");
                    // GlobalEvents.CheckpointCreate(p, Tutorial_Main.TutPos[1], 44, 1, new Rgba(255, 255, 255, 100), "Tutorial:Run", "8");
                    break;
                }   
                else 
                { 
                    MainChat.SendErrorChat(p, "您已经完成此新手教程了!");
                    break; 
                }
            }    
            case "26": // 下一个点 去加油站
            {
                if (p.isFinishTut == 26)
                {
                    p.isFinishTut = 27;
                    p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}理发店");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}进入理发店, 然后输入 {fc5e03}/barber{FFFFFF} 更换角色发型!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}现在, 更换一个发型吧!");
                    // GlobalEvents.CheckpointCreate(p, Tutorial_Main.TutPos[1], 44, 1, new Rgba(255, 255, 255, 100), "Tutorial:Run", "8");
                    break;
                }   
                else 
                { 
                    MainChat.SendErrorChat(p, "您已经完成此新手教程了!");
                    break; 
                }
            }       
            case "28": // 市政府
            {
                if (p.isFinishTut == 28)
                {
                    VehModel v = Vehicle.VehicleMain.getVehicleFromSqlId(p.tutCar);
                    v.fine = 50;
                    
                    p.isFinishTut = 28;
                    p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}纳税系统");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}只要您拥有资产, 政府就会对您进行扣税!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}您目前有什么资产? 当然有啊, 是我们赠送您的车辆!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}您需要为您的资产纳税, 如果不纳税, 政府会没收您的资产(如果是车辆, 您的车辆会莫名其妙的消失, 您/cgps也找不到)!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}现在, 请在税务部门输入 {fc5e03}/paytax{FFFFFF} 给您的asbo进行纳税吧!");
                    break;
                }   
                else 
                { 
                    MainChat.SendErrorChat(p, "您已经完成此新手教程了!");
                    break; 
                }
            }      
            
            case "29": // 市政府
            {
                if (p.isFinishTut == 29)
                {
                    p.isFinishTut = 30;
                    p.SendChatMessage("<br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br><br>");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}关于{fc5e03}手机系统");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}嘿, 您已经到达了商店!");
                    MainChat.SendInfoChat(p, "{fc5e03}新手教程: {FFFFFF}现在, 请输入 {fc5e03}/mb{FFFFFF} 打开商店菜单和购买一部手机吧!");
                    break;
                }   
                else 
                { 
                    MainChat.SendErrorChat(p, "您已经完成此新手教程了!");
                    break; 
                }
            }               
        }
    }
}