using System.Collections.Generic;
using AltV.Net; using AltV.Net.Async;
using outRp.Models;
using Newtonsoft.Json;

namespace outRp.OtherSystem.NativeUi
{
    public class LSCUI : IScript
    {
        public class UI
        {
            public string Title { get; set; } = "";
            public string SubTitle { get; set; } = "";
            public int[] StartPoint { get; set; } = new int[] { 50, 50 };
            public double Scale { get; set; } = 1.5;
            public string[] Banner { get; set; } = new string[] { "commonmenu", "interaction_bgd" };
            public bool DropShadow { get; set; } = false;

            public List<SubMenu> SubMenu = new List<SubMenu>();
            public List<Component_ListItem> ListItems = new List<Component_ListItem>();
            public List<Component_SliderItem> SliderItems = new List<Component_SliderItem>();
            public List<Component_CheckboxItem> CheckboxItems = new List<Component_CheckboxItem>();
            public List<Component_Item> Items = new List<Component_Item>();

            public void Send(PlayerModel p) => p.EmitLocked("LSCUI:Create", JsonConvert.SerializeObject(this));
        }

        public class Component_ListItem
        {
            public string Header { get; set; } = "";
            public string Description { get; set; } = "";
            public string[] Items { get; set; } = new string[] { };
            public string Trigger { get; set; } = "无";
            public string TriggerData { get; set; } = "无";
        }

        public class Component_SliderItem
        {
            public string Header { get; set; } = "";
            public string[] Items { get; set; } = new string[] { };
            public int Index { get; set; } = 0;
            public string Description { get; set; } = "";
            public bool Divider { get; set; } = false;
            public string Trigger { get; set; } = "无";
            public string TriggerData { get; set; } = "无";
        }

        public class Component_CheckboxItem
        {
            public string Header { get; set; } = "";
            public bool Check { get; set; } = false;
            public string Description { get; set; } = "";
            public string Value { get; set; } = "";
            public string Trigger { get; set; } = "无";
            public string TriggerData { get; set; } = "无";
        }

        public class Component_Item
        {
            //UIMenuItem(text: any, description?: string, trigger?: any, value?: any, data?: any)
            public string Header { get; set; } = "";
            public string Description { get; set; } = "";
            public string Trigger { get; set; } = "无";
            public string TriggerData { get; set; } = "无";
        }

        public class SubMenu
        {
            public string Header { get; set; } = "";
            public string SubTitle { get; set; } = "";
            public int[] StartPoint { get; set; } = new int[] { 50, 50 };
            public double Scale { get; set; } = 1.5;
            public bool DropShadow { get; set; } = false;
            public string[] Banner { get; set; } = new string[] { "commonmenu", "interaction_bgd" };

            public List<Component_ListItem> ListItems = new List<Component_ListItem>();
            public List<Component_SliderItem> SliderItems = new List<Component_SliderItem>();
            public List<Component_CheckboxItem> CheckboxItems = new List<Component_CheckboxItem>();
            public List<Component_Item> Items = new List<Component_Item>();
        }

        public static void Close(PlayerModel p)
        {
            p.EmitAsync("LSCUI:Close");
            return;
        }
    }
}
