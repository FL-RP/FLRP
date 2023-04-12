using System.Collections.Generic;
using Newtonsoft.Json;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;

namespace outRp.OtherSystem.NativeUi
{
    public class NativeUiModel
    {
        public screenPos screenPos { get; set; }
        public UiName Uiname { get; set; }
        public List<Listitem> ListItem { get; set; } = new List<Listitem>();
        public List<SliderItem> SliderItem { get; set; } = new List<SliderItem>();
        public List<CheckBoxItem> CheckBoxItem { get; set; } = new List<CheckBoxItem>();
        public List<JustMenu> JustMenu { get; set; } = new List<JustMenu>();
        public List<AutoListItem> AutoListItem { get; set; } = new List<AutoListItem>();
        public List<submenu> SubMenu { get; set; } = new List<submenu>();

        public void Send(IPlayer player) => Events.SendNative(player, this);
        public void SendClothes(IPlayer player) => Events.SendClothes(player, this);
    }

public struct screenPos
    {
        public int x;
        public int y;
        public screenPos(int X, int Y)
        {
            this.x = X;
            this.y = Y;
        }
    }
    public struct UiName
    {
        public string upperName { get; set; }
        public string subName { get; set; }
        public float size { get; set; }
        public bool shadow { get; set; }
        public string banner1 { get; set; }
        public string banner2 { get; set; }

        public UiName(string UpperName, string SubString, float Size, bool Shadow, string Banner1, string Banner2)
        {
            this.upperName = UpperName;
            this.subName = SubString;
            this.size = Size;
            this.shadow = Shadow;
            this.banner1 = Banner1;
            this.banner2 = Banner2;
        }
    }
    public struct Listitem
    {
        public string name;
        public string desc;
        public string trigger;
        public List<Litem> items;
    }
    public struct Litem
    {
        public string name;
    }
    public struct SliderItem
    {
        public string name;
        public string desc;
        public bool divider;
        public string trigger;
        public List<Sitem> items;
    }

    public class Sitem
    {
        public string name;
        public string value;
    }

    public struct CheckBoxItem
    {
        public string name;
        public string desc;
        public string trigger;
        public int value;
        public bool defaultValue;
    }
    public struct JustMenu
    {
        public string name;
        public string desc;
        public string value;
        public string trigger;
    }
    public struct AutoListItem
    {
        public string name;
        public string desc;
        public int maxVal;
        public int minVal;
        public int currentVal;
        public string trigger;
    }
    public struct submenu
    {
        public NativeUiModel subitems;
    }

    public class Events
    {
        //
        public static void SendNative(IPlayer player, NativeUiModel ui)
        {
            string json = JsonConvert.SerializeObject(ui);
            player.EmitLocked("Create:NativeUi", json);
        }

        public static void SendClothes(IPlayer player, NativeUiModel ui)
        {
            string json = JsonConvert.SerializeObject(ui);
            player.EmitLocked("Create:ClothesUI", json);
        }
    }

}
