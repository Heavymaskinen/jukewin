using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Juke.External.Wmp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WmpComponentTest
{
    [TestClass]
    public class FileCollectorTest
    {
        [TestMethod]
        public void ParseOneLevel()
        {
            var factory = new FakeFolderBrowserFactory {FakeFolderBrowser = new FakeFolderBrowser(0, 10)};
            var list = PerformAsyncCollect(factory);
            Assert.AreEqual(10, list.Count);
        }

        [TestMethod]
        public void ParseTwoLevels()
        {
            var factory = new FakeFolderBrowserFactory {FakeFolderBrowser = new FakeFolderBrowser(1, 10)};
            IList<string> result = PerformAsyncCollect(factory);

            Assert.AreEqual(20, result.Count);
        }

        [TestMethod]
        public void ParseThreeLevels()
        {
            var factory = new FakeFolderBrowserFactory {FakeFolderBrowser = new FakeFolderBrowser(2, 10)};
            IList<string> result = PerformAsyncCollect(factory);
            Assert.AreEqual(10 * 3 + 10 * 2, result.Count);
        }

        [TestMethod]
        public void ParserStressTest_NineLevels()
        {
            //16 sec
            var factory = new FakeFolderBrowserFactory {FakeFolderBrowser = new FakeFolderBrowser(9, 10)};
            IList<string> result = PerformAsyncCollect(factory);
            Assert.AreEqual(9864100, result.Count);
        }

        [TestMethod]
        public void ParserStressTest_ManyFiles()
        {
            //11 sec
            var factory = new FakeFolderBrowserFactory {FakeFolderBrowser = new FakeFolderBrowser(8, 100)};
            IList<string> result = PerformAsyncCollect(factory);
            Assert.AreEqual(10960100, result.Count);
        }

        [TestMethod]
        public void ParserStressTest_RandomLevels()
        {
            //9.4 sec
            var factory = new FakeFolderBrowserFactory {FakeFolderBrowser = new FakeFolderBrowser(700, 10, 10)};
            IList<string> result = PerformAsyncCollect(factory);
            Assert.IsTrue(result.Count >= 253400, result.Count + " was actual");
        }

        private static IList<string> PerformAsyncCollect(FakeFolderBrowserFactory factory)
        {
            var t = Task.Run<IList<string>>(async () => await DoCollect(factory));
            t.Wait();
            var list = t.Result;
            return list;
        }

        private static async Task<IList<string>> DoCollect(FakeFolderBrowserFactory factory)
        {
            var engine = new FileCollector(factory);
            var result = await engine.CollectFileNames("path");
            return result;
        }
    }

    class FakeFolderBrowserFactory : IFolderBrowserFactory
    {
        public FakeFolderBrowser FakeFolderBrowser { get; set; }

        public IFolderBrowser Create(string path)
        {
            return FakeFolderBrowser;
        }
    }

    class FakeFolderBrowser : IFolderBrowser
    {
        private List<string> files;
        private List<IFolderBrowser> subFolders;

        internal FakeFolderBrowser(int levels, int fileCount, int randomMax)
        {
            AddFiles(levels, fileCount);
            CreateRandomSubFolders(levels, fileCount, randomMax);
        }

        private void AddFiles(int levels, int fileCount)
        {
            files = new List<string>();
            for (var i = 0; i < fileCount; i++)
            {
                files.Add("Level " + levels + "file " + i);
            }
        }

        private void CreateRandomSubFolders(int levels, int fileCount, int randomMax)
        {
            var rand = new Random();
            subFolders = new List<IFolderBrowser>();
            for (var i = 0; i < levels; i++)
            {
                subFolders.Add(new FakeFolderBrowser(rand.Next(0, randomMax), fileCount, randomMax - 1));
            }
        }

        internal FakeFolderBrowser(int levels, int fileCount)
        {
            files = new List<string>();
            subFolders = new List<IFolderBrowser>();
            for (var i = 0; i < fileCount; i++)
            {
                files.Add("Level " + levels + "file " + i);
            }

            for (var i = 0; i < levels; i++)
            {
                subFolders.Add(new FakeFolderBrowser(levels - 1, fileCount));
            }
        }

        public IList<string> GetFiles()
        {
            return files;
        }

        public List<IFolderBrowser> GetSubFolders()
        {
            return subFolders;
        }
    }
}