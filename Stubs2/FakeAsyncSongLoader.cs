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
