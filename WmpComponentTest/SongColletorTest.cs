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
            PerformSyncSongCollect(collector, list, listener);
            while (!listener.IsCompleted)
            {
                Thread.Sleep(4);
            }
            Assert.AreEqual(loadCount - 3, listener.CompletedSongs.Count);
        }

        private static void PerformSyncSongCollect(SongCollector collector, List<string> list, FakeListener listener)
        {
            PerformSyncSongCollect(collector, list, listener, CancellationToken.None);
        }

        private static void PerformSyncSongCollect(SongCollector collector, List<string> list, FakeListener listener, CancellationToken cancelToken)
        {
            collector.Load(list, listener, cancelToken).Wait(cancelToken);
        }

        private static Task PerformAsyncSongCollect(SongCollector collector, List<string> list, FakeListener listener, CancellationToken cancelToken)
        {
            return collector.Load(list, listener, cancelToken);
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
            Assert.AreEqual(loadCount, listener.CompletedSongs.Count);
        }

        [TestMethod]
        public void SongCollector_Cancellation_ResultsAreKept()
        {
            var listener = new FakeListener();
            int loadCount = 3000;
            var provider = new CancellationTokenSource();
            var collector = new SongCollector(listener, new FakeTagReaderFactory { FakeTagReader = new SlowTagReader() { Title = "MySong" } });

            var list = new List<string>();
            for (var i = 0; i < loadCount; i++)
            {
                list.Add("test" + i);
            }
            var task = PerformAsyncSongCollect(collector, list, listener, provider.Token);
            provider.CancelAfter(TimeSpan.FromMilliseconds(5));

            try
            {
                task.Wait(CancellationToken.None);
            }
            catch (Exception)
            {
                //Don't care...
            }
            
            provider.Dispose();
            
            Assert.IsNotNull(listener.CompletedSongs, "Should have completed songs!");
            Assert.IsTrue(listener.CompletedSongs.Count > 0, "too few: "+listener.CompletedSongs.Count);
            Assert.IsTrue(listener.CompletedSongs.Count < loadCount, "Too many: "+ listener.CompletedSongs.Count);
        }

        private static void CollectWithListener(FakeListener listener)
        {
            var collector = new SongCollector(listener, new FakeTagReaderFactory { FakeTagReader = new FakeTagReader { Title = "MySong" } });
            PerformSyncSongCollect(collector, new List<string>() { "test" }, listener);
        }

        private static void SlowCollectWithListener(FakeListener listener, int loadCount)
        {
            var collector = new SongCollector(listener, new FakeTagReaderFactory { FakeTagReader = new SlowTagReader { Title = "MySlowSong" } });

            var list = new List<string>();
            for (var i = 0; i < loadCount; i++)
            {
                list.Add("test" + i);
            }

            PerformSyncSongCollect(collector, list, listener);
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
