using DiscordMusicBot.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;


namespace DiscordMusicBot
{
    public class Program
    {
        public static DiscordClient Client { get; private set; }
        public static InteractivityExtension Interactivity { get; private set; }
        public static CommandsNextExtension Commands { get; private set; }

        public static async Task Main(string[] args)
        {
            string path = new Program().GetPath();
            Dictionary<string, string> Token_ = new Dictionary<string, string>();
            if (path != null)
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string json = reader.ReadToEnd();
                    Token_ = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                }

            }


            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = Token_["Token"],
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents
            });


            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "!" }
            });

            commands.RegisterCommands<CommandModule>();

            var endpoint = new ConnectionEndpoint()
            {
                Hostname = "lava.dcmusic.ca",
                Port = 443,
                Secured = true
            };

            var lavalinkConfig = new LavalinkConfiguration()
            {
                Password = "youshallnotpass",
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint,
            };

            var lavalink = discord.UseLavalink();

            await discord.ConnectAsync();
            await lavalink.ConnectAsync(lavalinkConfig);

            await Task.Delay(-1);
        }


        public string GetPath()
        {
            DirectoryInfo di = new DirectoryInfo(AppContext.BaseDirectory);
            string path = di.Parent.Parent.Parent.FullName;
            string checkpath = path.Substring(path.Length - 15);
            if (checkpath == "DiscordMusicBot" && Path.Exists(Path.Combine(path, "JsonData", "Config.json")))
            {
                return Path.Combine(path, "JsonData", "Config.json");
            }
            return null;
        }



    }
}