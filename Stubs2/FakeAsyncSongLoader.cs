using Juke.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Juke.Core;

namespace Juke.Control.Tests
{
    public class FakeSongLoadEngine : LoadEngine
    {
        public IList<Song> list;

        public FakeSongLoadEngine() : this(new List<Song>())
        {
        }

        public FakeSongLoadEngine(IList<Song> list)
        {
            this.list = list;
        }

        public string Path { get; private set; }
        public bool IsInitiated { get; private set; }

        private LoadListener listener;
        public List<string> Load(string path, LoadListener listener)
        {
            this.Path = path;
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
                               this.Path = path;
                               this.listener = listener;
                               Initiate();
                               return new List<string>();
                           });
            task.Wait();
            return task;
        }
    }

    public class FakeAsyncSongLoader : AsyncSongLoader
    {
        private IList<Song> list;
        private FakeSongLoadEngine engine;

        public FakeAsyncSongLoader():this(new FakeSongLoadEngine())
        {
        }

        public FakeAsyncSongLoader(FakeSongLoadEngine engine):base(engine ,new FakeTagReaderFactory())
        {
            this.engine = engine;
            this.list = engine.list;
        }

        public void SignalComplete()
        {
            engine.SignalComplete();
        }

        public void SignalProgress()
        {
            engine.SignalComplete();
        }

    }

    class FakeTagReaderFactory : TagReaderFactory
    {
        public TagReader Create(string filename)
        {
            return null;
        }
    }
}
