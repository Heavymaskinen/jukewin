using DataModel;
using Juke.Core;
using Juke.External.Wmp;
using Juke.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WmpComponentTest
{

    [TestClass]
    public class SongColletorTest
    {
        [TestMethod]
        public void NotifyOnLoadInitiated()
        {
            var listener = new FakeListener();
            CollectWithListener(listener);
            Assert.IsTrue(listener.IsInitiated);
        }


        [TestMethod]
        public void NotifyOnProgress()
        {
            var listener = new FakeListener();
            CollectWithListener(listener);
            Assert.IsTrue(listener.HasProgress);
        }

        [TestMethod]
        public void NotifyOnCompletion()
        {
            var listener = new FakeListener();
            CollectWithListener(listener);
            Assert.IsTrue(listener.IsCompleted);
        }

        [TestMethod]
        public void ProvideSongsOnComplete()
        {
            var listener = new FakeListener();
            CollectWithListener(listener);
            Assert.AreEqual(1, listener.CompletedSongs.Count);
        }

        [TestMethod]
        public void SongDataReadFromTagReader()
        {
            var listener = new FakeListener();
            CollectWithListener(listener);
            Assert.AreEqual("MySong", listener.CompletedSongs[0].Name);
        }

        [TestMethod]
        public void SongCollector_CatchExceptions_SongsNotIncluded()
        {
            //9.4 sec
            var listener = new FakeListener();
            int loadCount = 400;
            var list = new List<string>();
            for (var i = 0; i < loadCount; i++)
            {
                list.Add("test" + i);
            }

            var collector = new SongCollector(listener, new FakeTagReaderFactory { FakeTagReader = new SlowTagReader { Fails = 3 } });
            collector.Load(list, listener).Wait();
            while (!listener.IsCompleted)
            {
                Thread.Sleep(4);
            }
            Assert.AreEqual(loadCount - 3, listener.CompletedSongs.Count);
        }

        [TestMethod]
        public void SongCollector_StressTest()
        {
            //9.4 sec
            var listener = new FakeListener();
            int loadCount = 600;
            SlowCollectWithListener(listener, loadCount);
            Assert.AreEqual(listener.CompletedSongs.Count, loadCount);
        }

        [TestMethod]
        public void SongCollector_StressTest2()
        {
            //3 min
            var listener = new FakeListener();
            int loadCount = 21620;
            SlowCollectWithListener(listener, loadCount);
            Assert.AreEqual(listener.CompletedSongs.Count, loadCount);
        }

        private static void CollectWithListener(FakeListener listener)
        {
            var collector = new SongCollector(listener, new FakeTagReaderFactory { FakeTagReader = new FakeTagReader { Title = "MySong" } });
            collector.Load(new List<string>() { "test" }, listener).Wait();
        }

        private static void SlowCollectWithListener(FakeListener listener, int loadCount)
        {
            var collector = new SongCollector(listener, new FakeTagReaderFactory { FakeTagReader = new SlowTagReader { Title = "MySlowSong" } });

            var list = new List<string>();
            for (var i = 0; i < loadCount; i++)
            {
                list.Add("test" + i);
            }

            collector.Load(list, listener).Wait();
        }
    }

    class FakeTagReader : TagReader
    {
        public virtual string Title { get; set; }

        public string Album { get; set; }

        public string Artist { get; set; }

        public string TrackNo { get; set; }

        public virtual FakeTagReader New()
        {
            return new FakeTagReader { Title = this.Title };
        }
    }

    class SlowTagReader : FakeTagReader
    {
        private int count = 0;
        private string title;
        public override string Title
        {
            set { title = value; }
            get
            {
                Thread.Sleep(2);
                return title;
            }
        }

        public int Fails { get; set; }

        public override FakeTagReader New()
        {
            count++;
            if (Fails <= 0) return new SlowTagReader {Title = title+count};

            Fails--;
            throw new Exception("Test failure! " + Fails);
        }
    }
    
    class FakeTagReaderFactory : TagReaderFactory
    {
        public FakeTagReader FakeTagReader { get; set; }
        public TagReader Create(string filename)
        {
            return FakeTagReader.New();
        }
    }

    class FakeListener : LoadListener
    {
        public bool IsCompleted { get; private set; }
        public IList<Song> CompletedSongs { get; private set; }
        public bool IsInitiated { get; private set; }
        public bool IsNewLoad { get; private set; }
        public bool HasProgress { get; private set; }

        public void NotifyCompleted(IList<Song> loadedSongs)
        {
            IsCompleted = true;
            CompletedSongs = loadedSongs;
        }

        public void NotifyLoadInitiated(int capacity)
        {
            IsInitiated = true;
        }

        public void NotifyNewLoad()
        {
            IsNewLoad = true;
        }

        public void NotifyProgress(int progressed)
        {
            HasProgress = true;
        }
    }
}
