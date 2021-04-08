using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CoreAudioComponent;
using CoreSongIO;
using DataModel;
using Grpc.Core;
using Grpc.Core.Logging;
using Juke.Control;
using Juke.Core;
using Juke.External.Xml;
using ILogger = Grpc.Core.Logging.ILogger;

namespace GrpcJukeServer.Services
{
    public class JukeServer : JukeDaemon.JukeDaemonBase
    {
        public static event EventHandler ShutdownInitiated;
        public static bool ShuttingDown = false;

        private IJukeController jukeController;
        private ILogger logger;
        private static List<string> messages= new List<string>();

        public JukeServer()
        {
            logger = new TextWriterLogger(new StreamWriter("/Users/asbjornandersen/juke-server.log")
                {AutoFlush = true});
            PlayerEngineFactory.Engine = new CorePlayerEngine();
            jukeController             = JukeController.Instance;
            jukeController.Player.SongPlayed += (sender, song) => Messenger.Post("Now playing: " + song.Artist + " - " + song.Name);
            jukeController.Player.RegisterPlayerEngine(new CorePlayerEngine());
            Messenger.FrontendMessagePosted += (msg, s) =>
            {
                if (!messages.Contains(msg)) messages.Add(msg);
            };
        }

        public override async Task GetLogStream(EmptyRequest request, IServerStreamWriter<OutputData> responseStream,
            ServerCallContext context)
        {
            Console.WriteLine("Output stream opened");
            while (!ShuttingDown || messages.Count > 0)
            {
                try
                {
                    await WriteMessagesToStream(responseStream);
                }
                catch (Exception e)
                {
                    await responseStream.WriteAsync(new OutputData {Message = e.Message});
                    break;
                }
            }
            Console.WriteLine("Output stream closed");
        }

        private static async Task WriteMessagesToStream(IServerStreamWriter<OutputData> responseStream)
        {
            List<string> messagesToPost;
            lock (messages)
            {
                messagesToPost = messages.GetRange(0, messages.Count);
                messages.Clear();
            }

            foreach (var message in messagesToPost)
            {
                if (message == null) continue;
                await responseStream.WriteAsync(new OutputData {Message = message});
            }

            await Task.Delay(5);
        }

        public static void Touch()
        {
            new JukeServer().PreLoad();
        }

        internal void PreLoad()
        {
            var comboPath = Directory.GetCurrentDirectory() + "/library.xml";
            if (jukeController.Browser.Songs.Count == 0 && File.Exists(comboPath))
            {
                jukeController.LoadHandler.LoadSongs(new XmlSongReader(comboPath));
                logger.Debug("Library reloaded! " + jukeController.Browser.Songs.Count);
            }
            else
            {
                Console.WriteLine("Library not found? " + jukeController.Browser.Songs.Count + " " +
                                  File.Exists(comboPath));
                logger.Debug("Library not found " + comboPath + " " + File.Exists(comboPath));
            }
            
            Messenger.Post("J.U.K.E. is now running. Please enjoy.\n");
            Console.WriteLine("Running!");
        }

        public override Task<StatusReply> Add(AddRequest request, ServerCallContext context)
        {
            Console.WriteLine("Add songs from " + request.Folder);
            jukeController.LoadHandler.LoadSongs(new CoreSongLoader("*.mp3", request.Folder));
            Messenger.Post("All songs added ("+jukeController.Browser.Songs.Count+")\n");
            jukeController.SaveLibrary(new XmlSongWriter("library.xml"));
            Messenger.Post("Library saved\n");
            return Task.FromResult(new StatusReply {IsOk = true});
        }

        public override Task<StatusReply> Load(LoadRequest request, ServerCallContext context)
        {
            logger.Debug("Load library from " + request.FileName);
            var isOk = false;
            if (File.Exists(request.FileName))
            {
                jukeController.LoadHandler.LoadSongs(new XmlSongReader(request.FileName));
                isOk = true;
            }

            return Task.FromResult(new StatusReply {IsOk = isOk});
        }

        public override Task<SearchReply> Search(SearchRequest request, ServerCallContext context)
        {
            logger.Debug("Search for " + request.Query);
            var songs = jukeController.Browser.Songs.Where(s
                => s.Name.ToLower().Contains(request.Query.ToLower()));

            var infos = songs.Select(
                    song => new SongInfo
                    {
                        Album = song.Album, Artist = song.Artist, Name = song.Name, SongId = int.Parse(song.TrackNo)
                    })
                .ToList();

            logger.Debug("Found: " + infos.Count);

            return Task.FromResult(new SearchReply {Matches = {infos}});
        }

        public override Task<StatusReply> Play(PlayRequest request, ServerCallContext context)
        {
            logger.Debug("Play " + request.Name);
            var songs = jukeController.Browser.GetSongsByTitle(request.Name);
            if (songs.Count == 0)
            {
                return Task.FromResult(new StatusReply {IsOk = false});
            }

            jukeController.Player.PlaySong(songs[0]);
            return Task.FromResult(new StatusReply {IsOk = true});
        }

        public override Task<SongInfo> PlayRandom(EmptyRequest request, ServerCallContext context)
        {
            logger.Debug("Play random");
            SongInfo info;
            try
            {
                jukeController.Player.PlaySong(null);
                var song = jukeController.Player.NowPlaying;
                info = CreateSongInfo(song);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                Console.WriteLine(e);
                info = new SongInfo {Name = "Failed!", Artist = e.Message, Album = "ERROR", SongId = 0};
            }

            return Task.FromResult(info);
        }

        public override Task<InfoReply> GetInfo(EmptyRequest request, ServerCallContext context)
        {
            Messenger.Post(JukeController.Instance.Browser.Artists.Count+" artists\n");
            Messenger.Post(JukeController.Instance.Browser.Songs.Count+" songs\n");
            
            var player  = JukeController.Instance.Player;
            var playing = CreateSongInfo(player.NowPlaying);
            var queue   = player.EnqueuedSongs.Select(CreateSongInfo).ToList();
            var infoReply = new InfoReply
            {
                CurrentSong = playing,
                Queue       = {queue}
            };

            return Task.FromResult(infoReply);
        }

        private static SongInfo CreateSongInfo(Song song)
        {
            if (song == null)
            {
                return new SongInfo
                {
                    Album = "none", Artist = "none", Name = "none", SongId = 0
                };
            }

            return new SongInfo
            {
                Album = song.Album, Artist = song.Artist, Name = song.Name, SongId = int.Parse(song.TrackNo)
            };
        }

        public override Task<StatusReply> Shutdown(EmptyRequest request, ServerCallContext context)
        {
            Messenger.Post("Shutting down. Thank you for using J.U.K.E\n");
            ShuttingDown = true;
            jukeController.Dispose();
            var processes = Process.GetProcesses().Where(x => x.ProcessName == "afplay");
            
            foreach (var process in processes)
            {
                process.Kill(true);   
            }
            
            ShutdownInitiated?.Invoke(this, EventArgs.Empty);
            
            return Task.FromResult(new StatusReply {IsOk = true});
        }
    }
}