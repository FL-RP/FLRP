using AltV.Net; using AltV.Net.Async;
using Newtonsoft.Json;
using outRp.Models;
using outRp.OtherSystem.LSCsystems;
using System;

namespace outRp.OtherSystem.NativeUi
{
    public class Quasar : IScript
    {
        public class Notify
        {
            public string? type { get; set; }
            public string? color { get; set; }
            public string? textColor { get; set; }
            public string message { get; set; }
            public string? caption { get; set; }
            public bool? html { get; set; }
            public string? icon { get; set; }
            public string? avatar { get; set; }
            public bool? spinner { get; set; }
            public string? position { get; set; }
            public object? group { get; set; }
            public string? badgeColor { get; set; }
            public string? badgeTextColor { get; set; }
            public string? badgePosition { get; set; }
            public string? badgeStyle { get; set; }
            public string? badgeClass { get; set; }
            public bool? progress { get; set; }
            public string? progressClass { get; set; }
            public string? classes { get; set; }
            public object? attrs { get; set; }
            public int? timeout { get; set; }
            public string? closeBtn { get; set; }
            public bool? multiline { get; set; }
        }
        public class LoadingFullScreen
        {
            public _FullScreenObject opts { get; set; }

        }
        public class _FullScreenObject
        {
            public int? delay { get; set; }
            public string message { get; set; }
            public bool? html { get; set; }
            public string? boxClass { get; set; }
            public int? spinnerSize { get; set; }
            public string? spinnerColor { get; set; }
            public string? messageColor { get; set; }
            public string? backgroundColor { get; set; }
            public string? spinner { get; set; }
            public string? customClass { get; set; }
            public bool? ignoreDefaults { get; set; }
        }

        public class Dialog
        {
            public _Dialog opts { get; set; }
        }
        public class _Dialog
        {
            public string? style { get; set; }
            public string? title { get; set; }
            public string? message { get; set; }
            public bool? html { get; set; }
            public string? position { get; set; }
            public _Dialog_Prompt prompt { get; set; }
            public _Dialog_Options options { get; set; }
        }
        public class _Dialog_Prompt
        {
            public string model { get; set; }
            public string? type { get; set; }
            public string? label { get; set; }
            public bool? stackLabel { get; set; }
            public bool? filled { get; set; }
            public bool? outlined { get; set; }
            public string? standout { get; set; }
            public bool? rounded { get; set; }
            public bool? square { get; set; }
            public bool? counter { get; set; }
            public int? maxlength { get; set; }
            public string? prefix { get; set; }
            public string? suffix { get; set; }
        }
        public class _Dialog_Options
        {
            public string model { get; set; } = "[]";
            public string? type { get; set; }
            public string? items { get; set; }
            public string? ok { get; set; } // qBTN 'u json olarak atabilirsin.
            public string? cancel { get; set; } // QBTN'u json olarak atabilirsin.
            public string? focus { get; set; }
            public bool? stackButtons { get; set; }
            public string? color { get; set; }
            public bool? dark { get; set; }
            public bool? persistent { get; set; }
            public bool? noEscDismiss { get; set; }
            public bool? noBackdropDismiss { get; set; }
            public bool? noRouteDismiss { get; set; }
            public bool? seamless { get; set; }
            public bool? maximized { get; set; }
            public bool? fullWidth { get; set; }
            public bool? fullHeight { get; set; }
            public string? transitionShow { get; set; }
            public string? transitionHide { get; set; }
            public string? component { get; set; }
            public string? componentProps { get; set; }
        }
        public class Streamer
        {
            /// <summary>
            /// Quasar Framework'u ile Notify Stream eder.
            /// </summary>
            /// <param name="p">Gönderilecek Player</param>
            /// <param name="type">Optional type(that has been previously registered) or one of the out of the box ones('positive', 'negative', 'warning', 'info', 'ongoing') Examples negativecustom-type</param>
            /// <param name="message">Color name for component from the Quasar Color Palette Examples primaryteal-10</param>
            /// <param name="color">Color name for component from the Quasar Color Palette Examples primaryteal-10</param>
            /// <param name="textcolor">The content of your message \n Example John Doe pinged you</param>
            /// <param name="Caption">The content of your optional caption Example 5 minutes ago</param>
            /// <param name="HTML">Render the message as HTML; This can lead to XSS attacks so make sure that you sanitize the message first</param>
            /// <param name="icon">Icon name following Quasar convention; Make sure you have the icon library installed unless you are using 'img:' prefix Examples mapion-addimg:https://cdn.quasar.dev/logo-v2/svg/logo.svgimg:statics/path/to/some_image.png</param>
            /// <param name="Avatar">URL to an avatar/image; Suggestion: use statics folder Examples (statics folder) statics/img/something.png(relative path format) require('./my_img.jpg')(URL) https://some-site.net/some-img.gif</param>
            /// <param name="spinner">Useful for notifications that are updated; Displays the default Quasar spinner instead of an avatar or icon</param>
            /// <param name="Position">Window side/corner to stick to Default value "bottom" Accepted values top-lefttop-rightbottom-leftbottom-righttopbottomleftrightcenter Example top-right</param>
            /// <param name="Group">Override the auto generated group with custom one; Grouped notifications cannot be updated; String or number value inform this is part of a specific group, regardless of its options; When a new notification is triggered with same group name, it replaces the old one and shows a badge with how many times the notification was triggered Default value "(message + caption + multiline + actions labels + position)" Example my-group</param>
            /// <param name="badgeColor">Color name for the badge from the Quasar Color Palette Examples primaryteal-10</param>
            /// <param name="badgeTextColor">Color name for the badge text from the Quasar Color Palette Examples primaryteal-10</param>
            /// <param name="badgePosition">Notification corner to stick badge to; If notification is on the left side then default is top-right otherwise it is top-left Default value "(top-left/top-right)" Accepted values top-lefttop-rightbottom-leftbottom-right Example bottom-right</param>
            /// <param name="badgeStyle">Style definitions to be attributed to the badge Example background-color: #ff0000</param>
            /// <param name="badgeClass">Class definitions to be attributed to the badge Example my-special-class</param>
            /// <param name="progress">Show progress bar to detail when notification will disappear automatically (unless timeout is 0)</param>
            /// <param name="progressClass">Class definitions to be attributed to the progress bar Example my-special-class</param>
            /// <param name="classes">Add CSS class(es) to the notification for easier customization Example my-notif-class</param>
            /// <param name="attrs">Key-value for attributes to be set on the notification Example { role: 'alertdialog' }</param>
            /// <param name="timeout">Amount of time to display (in milliseconds) Default value 5000 Example 2500</param>
            /// <param name="closeButton">Convenient way to add a dismiss button with a specific label, without using the 'actions' convoluted prop Example Close me</param>
            /// <param name="multiLine">Put notification into multi-line mode; If this prop isn't used and more than one 'action' is specified then notification goes into multi-line mode by default</param>
            /// <param name="Trigger"></param>
            /// <param name="TriggerData"></param>
            public static void Notify(PlayerModel p, string message, string? type = null, string? color = null, string? textcolor = null, string? Caption = null, bool? HTML = null, string? icon = null, string? Avatar = null,
                bool? spinner = null, string? Position = null, object? Group = null, string? badgeColor = null, string? badgeTextColor = null, string? badgePosition = null, string? badgeStyle = null, string? badgeClass = null, bool? progress = null,
                string? progressClass = null, string? classes = null, object? attrs = null, int? timeout = null, string? closeButton = null, bool? multiLine = null, string? Trigger = null, string? TriggerData = null)
            {
                Notify n = new Notify();
                n.message = message;
                n.type = type ?? null;
                n.color = color ?? null;
                n.textColor = textcolor ?? null;
                n.caption = Caption ?? null;
                n.html = HTML ?? null;
                n.icon = icon ?? null;
                n.avatar = Avatar ?? null;
                n.spinner = spinner ?? null;
                n.position = Position ?? null;
                n.group = Group ?? DateTime.Now.ToJavaScriptMilliseconds();
                n.badgeColor = badgeColor ?? null;
                n.badgePosition = badgePosition ?? null;
                n.badgeStyle = badgeStyle ?? null;
                n.badgeClass = badgeClass ?? null;
                n.badgeTextColor = badgeTextColor ?? null;
                n.progress = progress ?? null;
                n.progressClass = progressClass ?? null;
                n.classes = classes ?? null;
                n.attrs = attrs ?? null;
                n.timeout = timeout ?? 5000;
                n.closeBtn = closeButton ?? null;
                n.multiline = multiLine ?? null;

                p.EmitAsync("Quasar:Notify", JsonConvert.SerializeObject(n), Trigger ?? null, TriggerData ?? null);
                return;
            }

            public static void Loading_FS(PlayerModel p, string message, int delay = 3000, bool? html = null, string boxClass = null, int? spinnerSize = null,
                string? spinnerColor = null, string? messageColor = null, string? backgroundColor = null, string? spinner = null, string? customClass = null,
                bool? ignoreDefaults = null)
            {
                LoadingFullScreen l = new LoadingFullScreen();
                _FullScreenObject n = new _FullScreenObject();
                n.message = message;
                n.delay = delay;
                n.html = html ?? null;
                n.boxClass = boxClass ?? null;
                n.spinnerSize = spinnerSize ?? null;
                n.spinnerColor = spinnerColor ?? null;
                n.messageColor = messageColor ?? null;
                n.backgroundColor = backgroundColor ?? null;
                n.spinner = spinner ?? null;
                n.customClass = customClass ?? null;

                l.opts = n;

                p.EmitAsync("Quasar:FullScreenLoading", JsonConvert.SerializeObject(l));
                return;
            }

            public static void Dialog(PlayerModel p, Dialog dialog, string trigger)
            {
                p.EmitAsync("Quasar:Dialog", JsonConvert.SerializeObject(dialog), trigger);
                return;
            }
        }
    }
}
