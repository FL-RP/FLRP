using Kook.Commands;
using System.Threading.Tasks;
using AltV.Net;

namespace outRp.Kook.Modules
{
    public class PublicModule : ModuleBase<SocketCommandContext>
    {

        [Command("ping")]
        [Alias("pong", "hello")]
        public Task PingAsync()
            => ReplyTextAsync("pong!");

        [Command("reg")]
        public async Task RegAsync(string uuid)
        {
            string uuidLowcase = uuid.ToLower();
            Alt.Log($"收到 kook id: {Context.User.Id.ToString()} 登录请求, uuid: {uuidLowcase} | ({uuid})");
            if (KookSpace.waitRegister.ContainsKey(uuidLowcase))
            {
                KookSpace.waitRegister[uuidLowcase] = Context.User.Id.ToString();
                await Context.Channel.SendTextAsync($"(met){Context.User.Id}(met) 注册成功", ephemeralUser: Context.User);
                await Context.Message.DeleteAsync();
            }
            else
            {
                await Context.Channel.SendTextAsync($"(met){Context.User.Id}(met) 注册失败, uuid 不存在", ephemeralUser: Context.User);
                await Context.Message.DeleteAsync();
            }
        }

        [Command("reply")]
        public async Task ReplyAsync(string uuid)
        {
            string uuidLowcase = uuid.ToLower();
            if (KookSpace.waitRegister.ContainsKey(uuidLowcase))
            {
                KookSpace.waitRegister[uuidLowcase] = Context.User.Id.ToString();
                await Context.Channel.SendTextAsync($"(met){Context.User.Id}(met) 注册成功", ephemeralUser: Context.User);
                await Context.Message.DeleteAsync();
            }
            else
            {
                await Context.Channel.SendTextAsync($"(met){Context.User.Id}(met) 注册失败, uuid 不存在", ephemeralUser: Context.User);
                await Context.Message.DeleteAsync();
            }
        }        

    }
}
