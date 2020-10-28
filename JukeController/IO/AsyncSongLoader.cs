using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataModel;
using Juke.Core;

namespace Juke.IO
{
    public class AsyncSongLoader : LoadListener
    {
        public static event EventHandler NewLoad;
        public static event EventHandler<int> LoadInitiated;
        public static event EventHandler<int> LoadProgress;
        public static event EventHandler<IList<Song>> LoadCompleted;

        protected TagReaderFactory tagReaderFactory;
        public string Path { get; set; }

        public static List<Task> tasks = new List<Task>();
        private LoadEngine engine;

        protected AsyncSongLoader(TagReaderFactory tagReaderFactory)
        {
            this.tagReaderFactory = tagReaderFactory;
            tasks.Clear();
        }

        public AsyncSongLoader(LoadEngine engine)
        {
            this.engine = engine;
        }

        public AsyncSongLoader(LoadEngine engine, TagReaderFactory tagReaderFactory)
        {
            this.engine = engine;
            this.tagReaderFactory = tagReaderFactory;
        }

        public void StartNewLoad()
        {
            if (engine != null)
            {
                engine.LoadAsync(Path, this);

            } else
            {
                BeginLoading();
            }
        }

        internal void BeginLoading()
        {
            engine.LoadAsync(Path, this);
        }

        private async Task LoadAsync()
        {
            Task task = Task.Run(() =>
            {
                try
                {
                    //var counter = new FileCounter("mp3");
                    //var fileCount = counter.Count(Path);
                    NotifyNewLoad();
                    //NotifyLoadInitiated(fileCount);
                    InvokeLoad();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            });
            tasks.Add(task);
            await task.ConfigureAwait(false);
        }

        protected virtual void InvokeLoad() { }

        protected void NotifyNewLoad()
        {
            NewLoad?.Invoke(this, EventArgs.Empty);
        }

        protected void NotifyLoadInitiated(int capacity)
        {
            LoadInitiated?.Invoke(this, capacity);
        }

        protected void NotifyProgress(int progressed)
        {
            LoadProgress?.Invoke(this, progressed);
        }

        protected void NotifyCompleted(IList<Song> loadedSongs)
        {
            LoadCompleted?.Invoke(this, loadedSongs);
        }

        public void NotifyNewLoad2()
        {
            NotifyNewLoad();
        }

        public void NotifyLoadInitiated2(int capacity)
        {
            NotifyLoadInitiated(capacity);
        }

        public void NotifyProgress2(int progressed)
        {
            NotifyProgress(progressed);
        }

        public void NotifyCompleted2(IList<Song> loadedSongs)
        {
            NotifyCompleted(loadedSongs);
        }
    }

    class FileCounter
    {
        private string extension;

        internal FileCounter(string extension)
        {
            this.extension = extension;
        }
        public int Count(string path)
        {
            var foundFiles = Directory.EnumerateFiles(path, extension);
            var directories = Directory.EnumerateDirectories(path);
            int subCount = 0;

            foreach (var dir in directories)
            {
                subCount += Count(dir);
            }

            return subCount + foundFiles.Count();
        }
    }
}
