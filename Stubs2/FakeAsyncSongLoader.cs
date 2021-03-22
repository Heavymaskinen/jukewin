using Juke.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataModel;
using Juke.Core;
using Stubs2;

namespace Juke.Control.Tests
{
    public class FakeSongLoadEngine : LoadEngine
    {
        public List<Song> list;

        public FakeSongLoadEngine() : this(new List<Song>())
        {
        }

        public FakeSongLoadEngine(List<Song> list)
        {
            this.list = list;
        }

        public string Path { get; private set; }
        public bool IsInitiated { get; private set; }

        private LoadListener listener;

        public List<string> Load(string path, LoadListener listener)
        {
            Path = path;
            this.listener = listener;
            Initiate();
            return new List<string>();
        }

        public void SignalComplete()
        {
            listener?.NotifyCompleted(list);
        }

        public void SignalProgress()
        {
            listener?.NotifyProgress(1);
        }

        public void Initiate()
        {
            IsInitiated = true;
            listener?.NotifyLoadInitiated(list.Count);
        }

        public Task<List<string>> LoadAsync(string path, LoadListener listener)
        {
            var task = Task.Run(() =>
            {
                Path = path;
                this.listener = listener;
                Initiate();
                listener.NotifyProgress(1);
                var list1 = list.Select((e) => e.Name).ToList();
                return list1;
            });
            task.Wait();
            return task;
        }
    }

    /// <summary>
    /// Wraps an AsyncSongLoader to make in synchronous
    /// </summary>
    public class FakeAsyncSongLoader : IAsyncSongLoader
    {
        private IList<Song> list;
        private AsyncSongLoader innerLoader;

        public string Path
        {
            get => innerLoader.Path;
            set => innerLoader.Path = value;
        }

        public FakeAsyncSongLoader(AsyncSongLoader innerLoader)
        {
            this.innerLoader = innerLoader;
        }

        public FakeAsyncSongLoader(FakeSongLoadEngine engine, ISongCollector songCollector) : this(
            new AsyncSongLoader(engine, songCollector))
        {
        }

        public FakeAsyncSongLoader(FakeSongLoadEngine engine)
        {
            innerLoader = new AsyncSongLoader(engine, new FakeSongCollector(engine.list));
        }

        public Task StartNewLoad(LoadListener listener, CancellationToken cancelToken)
        {
            var t = innerLoader.StartNewLoad(listener, cancelToken);
            t.Wait();
            return t;
        }
    }
}