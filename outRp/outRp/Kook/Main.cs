// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AltV.Net;
using Kook;
using AltV.Net.Resources.Chat.Api;
using Kook.Commands;
using Kook.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using outRp.Kook.Services;

namespace outRp.Kook
{
    public class KookSpace
    {
        private static KookSocketClient _client;
        public static Task Init() => new KookSpace().StartAsync();

        Boolean serverStatus = false;

        public struct LoginObject
        {
            public string authId;
            public bool success;
            public LoginObject Init(string authId, bool success)
            {
                this.authId = authId;
                this.success = success;
                return this;
            }
        };
        
        public struct LoginResult
        {
            public bool result;
            public string message;
            public LoginResult Init(bool result, string message)
            {
                this.result = result;
                this.message = message;
                return this;
            }
        };

        public static Dictionary<string, string?> waitRegister = new Dictionary<string, string?> {}; // tempAuthUuid, kookId
        public static Dictionary<string, LoginObject> waitLogin = new Dictionary<string, LoginObject> {}; // kookId, LoginObject

        public async Task StartAsync()
        {
            try
            {
                using (var services = ConfigureServices())
                {
                    _client = services.GetRequiredService<KookSocketClient>();

                    var token = "1/MTUyMzM=/ffVWASJx5fwAXlVseZYVrw==";

                    await _client.LoginAsync(TokenType.Bot, token);
                    await _client.StartAsync();

                    // Here we initialize the logic required to register our commands.
                    await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

                    _client.MessageButtonClicked += ButtonHandler;


                    _client.Ready += async () =>
                    {
                        Alt.Log("机器人登录成功!");
                        //SocketTextChannel main_channel = _client.GetGuild(9751081812917632).GetTextChannel(5577434421332481);
                        //await main_channel.SendTextAsync("Yo, wassup, am comin");
                        serverStatus = true;
                    };

                    await Task.Delay(Timeout.Infinite);
                }
            }
            catch (Exception ex) { Alt.Log($"{ex}"); }
        }

        public static Task StopAsync()
        {
            Alt.Log("机器人取消异步!");
            return Task.CompletedTask;
        }

        private async Task MessageUpdated(Cacheable<IMessage, Guid> before, SocketMessage after, ISocketMessageChannel channel)
        {
            // 如果没有启用消息缓存，消息下载方法可能会获得与 `after` 完全相同的实体
            var message = await before.GetOrDownloadAsync();
            Alt.Log($"{message} -> {after}");
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(_ => new KookSocketClient(new KookSocketConfig()
                {
                    AlwaysDownloadUsers = true,
                    LogLevel = LogSeverity.Debug,
                    AcceptLanguage = "zh-CN"
                }))
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .BuildServiceProvider();
        }

        public static async Task JoinMessage(string name)
        {
            try
            {
                SocketTextChannel main_channel = _client.GetGuild(6664149531385716).GetTextChannel(7887264593405868);
                await main_channel.SendTextAsync("玩家编号 " + name + " 加入了服务器(当前服务器人数: " + Globals.GlobalEvents.GetPlayerCount() + ")");
            }
            catch (Exception ex) { Console.WriteLine(ex); }

        }

        public static async Task AdMessage(string msg)
        {
            try
            {
                    SocketTextChannel main_channel = _client.GetGuild(6664149531385716).GetTextChannel(7204723498166243);
                    await main_channel.SendTextAsync(msg);
            }
            catch (Exception ex) { Console.WriteLine(ex); }

        }
        
        public static async Task AskqMessage(string kookid, string name, string msg)
        {
            try
            {
                SocketTextChannel main_channel = _client.GetGuild(6664149531385716).GetTextChannel(5297097201949681);
                await main_channel.SendTextAsync($"[一条求助 - 请(rol){9661514}(rol)及时回应]\n用户：(met){kookid}(met) - 角色：" + name + "\n内容："+ msg);
            }
            catch (Exception ex) { Console.WriteLine(ex); }
        }    
        
        public static async Task ReportMessage(string kookid, string name, string msg)
        {
            try
            {
                SocketTextChannel main_channel = _client.GetGuild(6664149531385716).GetTextChannel(5297097201949681);
                await main_channel.SendTextAsync($"[一条举报 - 请(rol){9661514}(rol)及时回应]\n用户：(met){kookid}(met) - 角色：" + name + "\n内容："+ msg);
            }
            catch (Exception ex) { Console.WriteLine(ex); }

        }        

        public static async Task<string?> Register(string uuid)
        {
            try
            {
                if (waitRegister.ContainsKey(uuid))
                {
                    Alt.Log($"{uuid} 已存在注册请求 已直接返回 null");
                    return null;
                }
                waitRegister.Add(uuid, null);
                for (var i = 0; i <= 300; i++)
                {
                    if (waitRegister[uuid] != null)
                    {
                        break;
                    }
                    await Task.Delay(500);
                }

                var kookId = waitRegister[uuid];
                waitRegister.Remove(uuid);
                Alt.Log($"{uuid} 注册结果 {kookId}");
                return kookId;
            }
            catch (Exception ex)
            {
                Alt.LogError(ex.Message);
                Alt.LogError(ex.StackTrace);
            }

            return null;
                
        }

        public static async Task<LoginResult> Login(string kookId)
        {
            try
            {
                if (waitLogin.ContainsKey(kookId))
                {
                    Alt.Log($"重复点击 已取消 {kookId}");
                    return new LoginResult().Init(false, "您的上一次登陆验证未结束, 请先进行验证或不要重复点击登录");
                }
                string tempAuthId = System.Guid.NewGuid().ToString();
                Alt.Log($"收到新的等待验证登录请求 {kookId} {tempAuthId}");
                waitLogin.Add(kookId, new LoginObject().Init(tempAuthId, false));

                JObject obj = new JObject();
                obj["operation"] = "login";
                obj["authId"] = tempAuthId;

                var messageCard = new CardBuilder()
                    .WithSize(CardSize.Large).WithTheme(CardTheme.Info)
                    .AddModule(new HeaderModuleBuilder().WithText(new PlainTextElementBuilder().WithContent("登录确认")))
                    .AddModule(new DividerModuleBuilder())
                    .AddModule(new SectionModuleBuilder().WithText(new KMarkdownElementBuilder().WithContent($"(met){kookId}(met)" + "")))
                    .AddModule(
                        new ActionGroupModuleBuilder()
                            .AddElement(
                                new ButtonElementBuilder()
                                    .WithClick(ButtonClickEventType.ReturnValue)
                                    .WithText("确认")
                                    .WithValue(obj.ToString())
                                    .WithTheme(ButtonTheme.Primary)
                            )
                    );
                SocketGuild guild = _client.GetGuild(6664149531385716);
                SocketTextChannel channel = guild.GetTextChannel(7993164409557940);
                SocketUser user = guild.GetUser(Convert.ToUInt64(kookId));
                var sendResult = await channel.SendCardAsync(messageCard.Build());
                int count = 0;
                for (int i = 0; i <= 300; i++)
                {
                    count++;
                    if (waitLogin[kookId].success)
                    {
                        Alt.Log($"登录已验证 {i} {kookId} {tempAuthId}");
                        break;
                    }
                    else
                    {
                        await Task.Delay(500);
                    }
                }
                bool result = waitLogin[kookId].success;
                Alt.Log($"{kookId} {tempAuthId} 登陆验证结果 {result}");
                waitLogin.Remove(kookId);
                if (count >= 300)
                {
                    await (await sendResult.GetOrDownloadAsync()).DeleteAsync();
                }
                return new LoginResult().Init(result, "");
            }
            catch (Exception ex)
            {
                Alt.LogError(ex.Message);
                Alt.LogError(ex.StackTrace);
                if (waitLogin.ContainsKey(kookId))
                {
                    waitLogin.Remove(kookId);
                }
                return new LoginResult().Init(false, "内部错误, 请重启后重试, 若仍故障, 请联系管理员");
            }

        }

        public static async Task ButtonHandler(string value, SocketUser user, IMessage message, SocketTextChannel channel, SocketGuild guild)
        {
            await Task.Run(async() =>
            {
                if (waitLogin.ContainsKey(user.Id.ToString()))
                {
                    LoginObject loginObject = waitLogin[user.Id.ToString()];

                    JObject jobj = JObject.Parse(value);
                    if ((jobj["operation"] ?? "").ToString() != "login")
                    {
                        return;
                    }
                    string authId = (jobj["authId"] ?? "").ToString();

                    if (loginObject.authId == authId)
                    {
                        if (loginObject.success)
                        {
                            await channel.SendTextAsync("您已经确认过登录了", ephemeralUser: user);
                        }
                        else
                        {
                            loginObject.success = true;
                            waitLogin[user.Id.ToString()] = loginObject;
                            await channel.SendTextAsync("登陆成功", ephemeralUser: user);
                            await message.DeleteAsync();
                        }
                    }
                    else
                    {
                        await channel.SendTextAsync("这不是您的登陆确认请求", ephemeralUser: user);
                    }
                }
            });
        }
    }
}