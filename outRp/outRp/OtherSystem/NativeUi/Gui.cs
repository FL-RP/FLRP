using System.Collections.Generic;
using AltV.Net; using AltV.Net.Async;
using outRp.Models;
using Newtonsoft.Json;

namespace outRp.OtherSystem.NativeUi
{
    public class Gui
    {
        public string image { get; set; } = "";
        public string color { get; set; } = "#49A0CD";
        public string info { get; set; } = "";
        public List<GuiMenu> guiMenu { get; set; } = null;
        public void Send(PlayerModel p) => GuiEvents.GuiSend(p, this);
    }
    public class GuiMenu
    {
        public string name { get; set; }
        public string triger { get; set; }
        public string value { get; set; }
        public string popup { get; set; } = null;
    }
    
    public class GuiEvents : IScript
    {
        public static GuiMenu closeItem = new GuiMenu() { name = "关闭", triger = "GUIMenu:Close", value = "0" };

        [AsyncClientEvent("GUIMenu:Close")]
        public static void GUIMenu_Close(PlayerModel p)
        {
            p.EmitLocked("GUI:Close");
        }
        public static void GuiSend(PlayerModel p, Gui g)
        {
            if(g.guiMenu != null) { GuiMenuSend(p, g); }
        }

        public static void GuiMenuSend(PlayerModel p, Gui a)
        {
            string json = JsonConvert.SerializeObject(a);
            p.EmitLocked("GUI:Create", json);
        }

        public static void GuiClose(PlayerModel p)
        {
            p.EmitLocked("GUI:Close");             
        }
    }

    public class Inputs : IScript
    {
         /// <summary>
         ///   Dönüş : callBackTrigger -> (string input, string otherValue)
         /// </summary>
         /// <param name="p"></param>
         /// <param name="headerText"></param>
         /// <param name="callBackTrigger"></param>
         /// <param name="otherValue"></param>
        public static void SendTypeInput(PlayerModel p, string headerText, string callBackTrigger, string otherValue = "x")
        {
            p.EmitLocked("UI:Input", 0,  headerText, callBackTrigger, otherValue);
        }

        /// <summary>
        ///  Dönüş : callBackTrigger -> (bool selection, string otherValue)
        /// </summary>
        /// <param name="p"></param>
        /// <param name="headerText"></param>
        /// <param name="callBackTrigger"></param>
        /// <param name="otherValue"></param>
        public static void SendButtonInput(PlayerModel p, string headerText, string callBackTrigger, string otherValue = "x")
        {
            p.EmitLocked("UI:Input", 1, headerText, callBackTrigger, otherValue);
        }

        /// <summary>
        ///  Dönüş : callBackTrigger -> (int r, int g, int b, int a, string otherValue)
        /// </summary>
        /// <param name="p"></param>
        /// <param name="headerText"></param>
        /// <param name="callBackTrigger"></param>
        /// <param name="otherValue"></param>
        public static void SendColorInput(PlayerModel p, string headerText, string callBackTrigger, string otherValue = "x")
        {
            p.EmitLocked("UI:Input", 2, headerText, callBackTrigger, otherValue);
        }
    }
    
}
