using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Logging;
using GrpcJukeServer;
using ILogger = Grpc.Core.Logging.ILogger;


namespace GrpcClient
{
    public class GrpcJukeClient : JukeClient
    {
        private JukeDaemon.JukeDaemonClient client;
        private ILogger logger;

        public GrpcJukeClient()
        {
            logger = new TextWriterLogger(new StreamWriter("/Users/asbjornandersen/juke-client.log") {AutoFlush = true});
            client = new JukeDaemon.JukeDaemonClient(new Channel("localhost", 5008, ChannelCredentials.Insecure));
        }

        public bool Startup(string serverLocation, bool verbose)
        {
            logger.Debug("Server Startup");
            try
            {
                var p = Process.Start(new ProcessStartInfo(serverLocation)
                {
                    RedirectStandardOutput = true, RedirectStandardError = true
                });
                Thread.Sleep(500);

                return !p.HasExited && p.Responding;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                Console.Error.WriteLine(e.Message);
                return false;
            }
        }

        public bool Shutdown()
        {
            logger.Debug("Server shutdown request");
            var reply = client.Shutdown(new EmptyRequest());
            return reply.IsOk;
        }

        public string[] Search(string query)
        {
            logger.Debug("Song search for "+query);
            var result = client.Search(new SearchRequest {Query = query});
            var names  = result.Matches.Select(info => info.Name+" ("+info.Artist+"/"+info.Album+")");
            return names.ToArray();
        }

        public bool LoadLibrary(string file)
        {
            logger.Debug("Load library from "+file);
            var response = client.Load(new LoadRequest {FileName = file});
            return response.IsOk;
        }

        public void Play(string name)
        {
            logger.Debug("Play "+name);
            client.Play(new PlayRequest {Name = name});
        }

        public string PlayRandom()
        {
            logger.Debug("Play random");
            var info = client.PlayRandom(new EmptyRequest());
            return info.Artist + " - " + info.Name;
        }

        public bool AddSongs(string folder)
        {
            logger.Debug("Add songs from "+folder);
            StreamOutput();
            var t = Task.Run(() =>
            {
                client.Add(new AddRequest {Folder = folder});
            });

            Task.Run(async () => await t).Wait();

            return true;
        }

        public async void StreamOutput()
        {
            Console.WriteLine("Start stream!");
            var streamingCall = client.GetLogStream(new EmptyRequest());

            var streamTask = streamingCall.ResponseStream.ReadAllAsync(new CancellationToken(false));
            var count = 10;
            try
            {
                while (count>0)
                {
                    await foreach (var data in streamTask)
                    {
                        Console.Write(data.Message);
                    }
                    
                    await Task.Delay(500);
                    count--;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            Console.WriteLine("End of stream!");
        }

        public Dictionary<string, string> GetInfo()
        {
            var infoReply = client.GetInfo(new EmptyRequest());
            var dict = new Dictionary<string, string>
            {
                {"Current Song", infoReply.CurrentSong.Artist + " - " + infoReply.CurrentSong.Name}
            };

            for (var i = 0; i < infoReply.Queue.Count; i++)
            {
                var entry = infoReply.Queue[i];
                dict.Add((i + 1) + ")", entry.Artist + " - " + entry.Name);
            }

            return dict;
        }
    }
}