using System.Collections.Generic;
using DataModel;
using Juke.UI.SearchLogics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juke.UI.Tests
{
    [TestClass]
    public class SearchLogicTest
    {
        [TestMethod]
        public void SingleMatch()
        {
            var viewModel = ViewModelFaker.InitializeLoadedViewModel(ViewModelFaker.CreateSongs(1, 1, 14));
            var logic = new SongSearchLogic(viewModel);
            var matches = logic.Search("song10");
            Assert.AreEqual(1, matches.Count);
            Assert.AreEqual("song10", matches[0].Name);
        }

        [TestMethod]
        public void FullMatchesComeFirst()
        {
            var viewModel = ViewModelFaker.InitializeLoadedViewModel(ViewModelFaker.CreateSongs(1, 1, 14));
            var logic = new SongSearchLogic(viewModel);
            var matches = logic.Search("song1");
            Assert.AreEqual(6, matches.Count);
            Assert.AreEqual("song1", matches[0].Name);
        }

        [TestMethod]
        public void FullMatch_ThenStartsWith_ThenContains()
        {
            var viewModel = ViewModelFaker.InitializeLoadedViewModel(
                new List<Song>
                {
                    new ("artist", "album","first"),
                    new ("artist", "album","first And"),
                    new ("artist", "album","first And Then"),
                    new ("artist", "album","lastWithFirst"),
                    new ("artist", "album","unmatched"),
                    new ("artist", "album","unmatched2"),
                });
            var logic = new SongSearchLogic(viewModel);
            var matches = logic.Search("first");
            Assert.AreEqual(4, matches.Count);
            Assert.AreEqual("first", matches[0].Name);
            Assert.AreEqual("first And", matches[1].Name);
            Assert.AreEqual("first And Then", matches[2].Name);
            Assert.AreEqual("lastWithFirst", matches[3].Name);
        }
    }
}