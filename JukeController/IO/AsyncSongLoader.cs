using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Juke.Core;

namespace Juke.IO
{
    public class AsyncSongLoader : IAsyncSongLoader
    {
        public static List<Task> tasks = new List<Task>();

        private LoadEngine engine;
        protected TagReaderFactory tagReaderFactory;
        private ISongCollector songCollector;

        public string Path { get; set; }
        
        public AsyncSongLoader(LoadEngine engine)
        {
            this.engine = engine;
        }

        public AsyncSongLoader(LoadEngine engine, ISongCollector songCollector)
        {
            this.engine = engine;
            this.songCollector = songCollector;
        }

        public AsyncSongLoader(LoadEngine engine, TagReaderFactory tagReaderFactory)
        {
            this.engine = engine;
            this.tagReaderFactory = tagReaderFactory;
        }

        public Task StartNewLoad(LoadListener listener, CancellationToken cancelToken)
        {
            if (engine != null)
            {
                if (songCollector == null)
                {
                    songCollector = new SongCollector(listener, tagReaderFactory);
                }
                
                return Task.Run(async () =>
                {
                    var list = await engine.LoadAsync(Path, listener);
                    await songCollector.Load(list, listener, cancelToken);
                });
                
            }

            Console.WriteLine("No engine?");
            return null;
        }
    }
}
