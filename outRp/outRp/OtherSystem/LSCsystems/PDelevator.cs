using System.Collections.Generic;
using AltV.Net; using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Resources.Chat.Api;
using outRp.Chat;
using outRp.Models;
using outRp.OtherSystem.NativeUi;
using outRp.OtherSystem.Textlabels;

namespace outRp.OtherSystem.LSCsystems
{
    public class PDelevator : IScript
    {
        public class Elevator
        {
            public string name { get; set; } = "楼层 1";
            public Position pos { get; set; } = new Position(0, 0, 0);
        }

        public static List<Elevator> elevator2 = new List<Elevator>()
        {
            new Elevator(){ name = "楼顶", pos = new Position(-1097.7891f, -849.9165f, 39.22705f) },
            new Elevator(){ name = "警探楼层", pos = new Position(-1097.7891f, -849.9165f, 35.351685f) },
            new Elevator(){ name = "指挥中心", pos = new Position(-1097.7891f, -849.9165F, 31.745728f) },
            new Elevator(){ name = "健身房 / 简报室", pos = new Position(-1097.6835f, -849.82416f, 27.819824f) },
            new Elevator(){ name = "餐厅 / 咖啡厅", pos = new Position(-1097.8022f, -849.9297f, 24.028564f ) },
            new Elevator(){ name = "大厅", pos = new Position(-1097.4462f, -850.2725f, 19.98462f) },
            new Elevator(){ name = "更衣室 / 装备库", pos = new Position(-1097.7759f, -849.9165f, 14.67688f) },
            new Elevator(){ name = "研究室 / 证据库", pos = new Position(-1097.1428f, -850.022f, 11.273315f) },
            new Elevator(){ name = "审讯室 / 代码室", pos = new Position(-1097.5516f, -850.1011f, 5.8813477f) },
        };

        public static List<Elevator> elevator1 = new List<Elevator>()
        {
            new Elevator(){ name = "健身房 / 简报室", pos = new Position(-1067.7098F, -833.26154f, 28.021973f) },
            new Elevator(){ name = "餐厅 / 咖啡厅", pos = new Position(-1097.8022f, -849.9297f, 24.028564f) },
            new Elevator(){ name = "大厅", pos = new Position(-1067.6307f, -833.47253f, 20.035156f) },
            new Elevator(){ name = "更衣室 / 装备库", pos = new Position(-1067.5912f, -833.433f, 15.87391f) },
            new Elevator(){ name = "研究室 / 证据库", pos = new Position(-1067.288f, -833.8022f, 12.031494f) },
            new Elevator(){ name = "审讯室 / 代码室", pos = new Position(-1067.433f, -833.578f, 6.4710693f) },
        };

        public static void LoadElevatorSystem()
        {
            foreach(var el1 in elevator1)
            {
                TextLabelStreamer.Create("~b~[电梯]~w~~n~" + el1.name + "~n~按 ~y~[E]", el1.pos, streamRange: 2, font: 0);
            }
            foreach (var el2 in elevator2)
            {
                TextLabelStreamer.Create("~b~[电梯]~w~~n~" + el2.name + "~n~按 ~y~[E]", el2.pos, streamRange: 2, font: 0);
            }
        }

        public static bool UseElevator(PlayerModel p)
        {
            List<Elevator> el = null;
            if(elevator1.Find(x => p.Position.Distance(x.pos) < 1.5f) != null) { el = elevator1; }
            if(el == null)
            {
                if(elevator2.Find(x => p.Position.Distance(x.pos) < 1.5f) != null) { el = elevator2; }
            }
            if(el == null) { return false; }
            
            List<GuiMenu> gMenu = new List<GuiMenu>();
            GuiMenu close = GuiEvents.closeItem;
            foreach(var e in el)
            {
                GuiMenu elevator_kat = new GuiMenu { name = e.name, triger = "Elevator:Use", value = e.name };
                gMenu.Add(elevator_kat);
            }

            gMenu.Add(close);
            Gui y = new Gui()
            {
                image = "https://upload.wikimedia.org/wikipedia/commons/b/b3/Aiga_elevator.svg",
                guiMenu = gMenu,
                color = "#4AC27D",
            };
            y.Send(p);
            return true;
        }

        [AsyncClientEvent("Elevator:Use")]
        public void Elevator_WantUse(PlayerModel p, string value)
        {
            GuiEvents.GUIMenu_Close(p);
            List<Elevator> el = null;
            if (elevator1.Find(x => p.Position.Distance(x.pos) < 1.5f) != null) { el = elevator1; }
            if (el == null)
            {
                if (elevator2.Find(x => p.Position.Distance(x.pos) < 1.5f) != null) { el = elevator2; }
            }

            p.Position = el.Find(x => x.name == value).pos;
            p.SendChatMessage(value + "您出来了.");
            return;
        }
    }
}
