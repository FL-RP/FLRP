using System;
using AltV.Net; using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using System.Reflection;
using System.Linq;
using outRp.Chat;

namespace outRp.OtherSystem.NativeUi
{

    [AttributeUsage(AttributeTargets.Method)]
    public class NativeAttribute : Attribute
    {
        public string trigger { get; set; }

        public NativeAttribute(string trigger)
        {
            this.trigger = trigger;
        }
    }


    public class NativeEventHandler : IScript
    {
        [AsyncClientEvent("NativeLSC:sendCallBack")]
        public async void OnNativeSendCMD(IPlayer player, string trigger, object[] args)
        {
            if (args == null)
                args = new object[] { };

            InvokeCommand(player, trigger, args);

            //Alt.Log($"{args[0]}");
        }

        public MethodInfo ReturnNativeMethod(string trigger)
        {
            return Assembly.GetExecutingAssembly().GetTypes().SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(NativeAttribute), false).Length > 0 &&
                m.GetCustomAttribute<NativeAttribute>().trigger == trigger).FirstOrDefault();
        }
        public void InvokeCommand(IPlayer player, string trigger, object[] args)
        {
            try
            {

                MethodInfo method = ReturnNativeMethod(trigger);
                var methodParams = method.GetParameters();
                var obj = Activator.CreateInstance(method.DeclaringType);

                method.Invoke(obj, NativeTypeParser(player, methodParams, args));
            }
            catch
            {
            }
        }

        public object[] NativeTypeParser(IPlayer player, ParameterInfo[] param, object[] EventArguments)
        {

            object[] array = new object[param.Length];

            array[0] = player;


            if (EventArguments.Length != 0)
            {
                for (int i = 1; i < param.Length; i++)
                {
                    try
                    {
                        
                        //Alt.Log(EventArguments[i - 1].ToString());
                        array[i] = EventArguments[i - 1].ToString();
                        
                    }
                    catch (NullReferenceException)
                    {
                        MainChat.SendErrorChat(player, "[信息] " + param[i].Name + " 参数丢失!");
                        break;
                    }
                }
            }
            return array;
        }



    }
}
