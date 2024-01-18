using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Lavalink;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMusicBot.Commands
{
    public class CommandModule : BaseCommandModule
    {
        [Command("greet")]
        public async Task GreetEveryone(CommandContext ctx)
        {
            await ctx.RespondAsync("Hello @all");
        }

        [Command("play")]
        public async Task PlayMusic (CommandContext ctx, string link)
        {
            var userVC = ctx.Member.VoiceState.Channel; //returns a voicechannel if user is sitting in one
            var lavalinkInstance = ctx.Client.GetLavalink();

            if(ctx.Member.VoiceState == null || userVC == null)
            {
                await ctx.Channel.SendMessageAsync("Please enter a VC!");
                return;
            }


            // found a bug, can't find any nodes

            if(lavalinkInstance.ConnectedNodes.Any())
            {
                await ctx.Channel.SendMessageAsync("Lavalink is dead or not connected");
                return;
            }

            // bug ends



            if(userVC.Type != DSharpPlus.ChannelType.Voice) // if uservc is not in a valid voicechannel
            {
                await ctx.Channel.SendMessageAsync("Please enter a valid VC!");
                return;
            }

            // connection to a voice channel
            var node = lavalinkInstance.ConnectedNodes.Values.First();
            await node.ConnectAsync(userVC);

            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild); //get guild connection

            if (conn == null)
            {
                await ctx.Channel.SendMessageAsync("Lavalink failed to connect");
                return;
            }
        }
    }
}
