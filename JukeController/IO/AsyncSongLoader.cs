using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Juke.Core;

namespace Juke.IO
{
    public class AsyncSongLoader
    {
        public static List<Task> tasks = new List<Task>();

        private LoadEngine engine;
        protected TagReaderFactory tagReaderFactory;

        public string Path { get; set; }
        
        public AsyncSongLoader(LoadEngine engine)
        {
            this.engine = engine;
        }

        public AsyncSongLoader(LoadEngine engine, TagReaderFactory tagReaderFactory)
        {
            this.engine = engine;
            this.tagReaderFactory = tagReaderFactory;
        }

        public void StartNewLoad(LoadListener listener)
        {
            if (engine != null)
            {
                var songCollector = new SongCollector(listener, tagReaderFactory);
                Task.Run(async () =>
                {
                    var list = await engine.LoadAsync(Path, listener);
                    await songCollector.Load(list);
                });
                
            }
            else
            {
                Console.WriteLine("No engine?");
            }
        }
    }
}
