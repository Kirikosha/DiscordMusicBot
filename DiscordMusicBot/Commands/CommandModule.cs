using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
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
        public async Task PlayMusic (CommandContext ctx, [RemainingText] string link)
        {
            var userVC = ctx.Member.VoiceState.Channel; //returns a voicechannel if user is sitting in one
            var lavalinkInstance = ctx.Client.GetLavalink();

            if(ctx.Member.VoiceState == null || userVC == null)
            {
                await ctx.Channel.SendMessageAsync("Please enter a VC!");
                return;
            }



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


            var searchQuery = await node.Rest.GetTracksAsync(link);
            if (searchQuery.LoadResultType == LavalinkLoadResultType.NoMatches || searchQuery.LoadResultType == LavalinkLoadResultType.LoadFailed)
            {
                await ctx.Channel.SendMessageAsync($"Failed to find music");
                return;
            }

            var musicTrack = searchQuery.Tracks.FirstOrDefault();
            await conn.PlayAsync(musicTrack);

            string musicDescription = $"Now playing: {musicTrack.Title}";

            var nowPlayingEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Purple,
                Title = $"successfully joined channel and playing music",
                Description = musicDescription
            };

            await ctx.Channel.SendMessageAsync(embed: nowPlayingEmbed);
        }


        [Command("pause")]
        public async Task PauseMusic(CommandContext ctx)
        {
            var userVC = ctx.Member.VoiceState.Channel; //returns a voicechannel if user is sitting in one
            var lavalinkInstance = ctx.Client.GetLavalink();

            if (ctx.Member.VoiceState == null || userVC == null)
            {
                await ctx.Channel.SendMessageAsync("Please enter a VC!");
                return;
            }



            if (userVC.Type != DSharpPlus.ChannelType.Voice) // if uservc is not in a valid voicechannel
            {
                await ctx.Channel.SendMessageAsync("Please enter a valid VC!");
                return;
            }


            var node = lavalinkInstance.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if(conn == null)
            {
                await ctx.Channel.SendMessageAsync("Failed to connect lavalink");
                return;
            }


            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.Channel.SendMessageAsync("Nothing is playing");
                return;
            }
            else
            {
                await conn.PauseAsync();
                return;
            }

            await conn.DisconnectAsync();
        }

        [Command("stop")]
        public async Task StopBot(CommandContext ctx)
        {
            var userVC = ctx.Member.VoiceState.Channel; //returns a voicechannel if user is sitting in one
            var lavalinkInstance = ctx.Client.GetLavalink();

            if (ctx.Member.VoiceState == null || userVC == null)
            {
                await ctx.Channel.SendMessageAsync("Please enter a VC!");
                return;
            }



            if (userVC.Type != DSharpPlus.ChannelType.Voice) // if uservc is not in a valid voicechannel
            {
                await ctx.Channel.SendMessageAsync("Please enter a valid VC!");
                return;
            }


            var node = lavalinkInstance.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.Channel.SendMessageAsync("Failed to connect lavalink");
                return;
            }


            await conn.DisconnectAsync();
        }
    }
}
