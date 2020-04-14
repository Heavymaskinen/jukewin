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
    public abstract class AsyncSongLoader
    {
        public static event EventHandler NewLoad;
        public static event EventHandler<int> LoadInitiated;
        public static event EventHandler<int> LoadProgress;
        public static event EventHandler<IList<Song>> LoadCompleted;

        protected TagReaderFactory tagReaderFactory;
        public string Path { get; set; }

        public static List<Task> tasks = new List<Task>();

        protected AsyncSongLoader(TagReaderFactory tagReaderFactory)
        {
            this.tagReaderFactory = tagReaderFactory;
            tasks.Clear();
        }

        internal void BeginLoading()
        {
            _ = LoadAsync();
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

        protected abstract void InvokeLoad();

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
