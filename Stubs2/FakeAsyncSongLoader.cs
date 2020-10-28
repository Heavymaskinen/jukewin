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
        private IList<Song> list;

        public FakeSongLoadEngine(IList<Song> list)
        {
            this.list = list;
        }

        public string Path { get; private set; }
        public bool IsInitiated { get; private set; }

        private LoadListener listener;
        public void Load(string path, LoadListener listener)
        {
            this.Path = path;
            this.listener = listener;
            Initiate();
        }

        public void SignalComplete()
        {
            listener.NotifyCompleted2(list);
        }

        public void SignalProgress()
        {
            listener.NotifyProgress2(1);
        }

        public void Initiate()
        {
            IsInitiated = true;
            listener.NotifyLoadInitiated2(list.Count);
        }

        public Task LoadAsync(string path, LoadListener listener)
        {
            throw new NotImplementedException();
        }
    }

    public class FakeAsyncSongLoader : AsyncSongLoader
    {
        private IList<Song> list;

        public FakeAsyncSongLoader():this(new List<Song>())
        {
        }

        public FakeAsyncSongLoader(IList<Song> list):base(new FakeTagReaderFactory())
        {
            this.list = list;
        }

        public void SignalComplete()
        {
            NotifyCompleted(list);
        }

        public void SignalProgress()
        {
            NotifyProgress(1);
        }

        protected override void InvokeLoad()
        {
            NotifyLoadInitiated(10);
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
