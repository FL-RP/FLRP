using AltV.Net; using AltV.Net.Async;
using Newtonsoft.Json;
using outRp.Models;

namespace outRp.OtherSystem
{
    public class GuiNotification : IScript
    {

        public class Model
        {
            public string message { get; set; }
            public string color { get; set; }
            public string type { get; set; }
            public string textColor { get; set; }
            public string caption { get; set; }
            public string icon { get; set; }
            public string avatar { get; set; }
            public string position { get; set; }
            public string badgeColor { get; set; }
            public string badgeTextColor { get; set; }
            public string badgePosition { get; set; }
            public string badgeStyle { get; set; }
            public bool progress { get; set; }
            public int? timeout { get; set; }
        }
        /// <summary>
        /// Notify Gönderme
        /// </summary>
        /// <param name="p">Gönderilecek oyuncu</param>
        /// <param name="message">Mesaj</param>
        /// <param name="color">Color name for component from the Quasar Color Palette</param>
        /// <param name="type">that has been previously registered) or one of the out of the box ones ('positive', 'negative', 'warning', 'info', 'ongoing</param>
        /// <param name="textColor">Color name for component from the Quasar Color Palette</param>
        /// <param name="Caption">The content of your optional caption, Example: 5 minutes ago</param>
        /// <param name="icon">map ion-add img:https://lsrpcc-1302879497.cos.ap-chengdu.myqcloud.com/item/639f9dcfb1fccdcd36a2f2e1.png img:statics/path/to/some_image.png</param>
        /// <param name="avatar">(statics folder) statics/img/something.png(relative path format) require('./my_img.jpg')(URL) https://lsrpcc-1302879497.cos.ap-chengdu.myqcloud.com/item/639f9dcfb1fccdcd36a2f2e1.png</param>
        /// <param name="position">top-left top-right bottom-left bottom-right top bottom left right center</param>
        /// <param name="badgeColor">primary teal-10</param>
        /// <param name="badgeTextColor">primary teal-10</param>
        /// <param name="badgePosition">top-left top-right bottom-left bottom-right</param>
        /// <param name="badgeStyle">background-color: #ff0000</param>
        /// <param name="progress">aç/kapat</param>
        /// <param name="timeOut">Amount of time to display (in milliseconds)</param>
        public static void Send(PlayerModel p, string message, string color = null, string type = null, string textColor = null,
            string Caption = null, string icon = null, string avatar = null, string position = null, string badgeColor = null,
            string badgeTextColor = null, string badgePosition = null, string badgeStyle = null, bool progress = false, int? timeOut = null)
        {
            Model notfi = new()
            {
                message = message,
                color = color,
                type = type,
                textColor = textColor,
                caption = Caption,
                icon = icon,
                avatar = avatar,
                position = position,
                badgeColor = badgeColor,
                badgeTextColor = badgeTextColor,
                badgePosition = badgePosition,
                badgeStyle = badgeStyle,
                progress = progress,
                timeout = timeOut
            };

            p.EmitAsync("GUI:ShowNotification", JsonConvert.SerializeObject(notfi));
            return;
        }
    }
}
