using System;
using DataModel;
using Juke.Control;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Juke.UI.Tests
{
    [TestClass]
    public class AdminViewModelStressTest
    {
        [TestInitialize]
        public void Setup()
        {
            JukeController.Reset();
        }

        [TestMethod]
        public void StressTest_ManyArtists_FewAlbums_ManySongs()
        {
            Console.WriteLine("Start it up! " + DateTime.Now);
            ViewModelFaker.InitializeLoadedAdminViewModel(ViewModelFaker.CreateSongsDistinct(200, 3, 20),
                    new FakeViewControl("path"));
            var viewModel = new SelectionTracker(JukeController.Instance.Browser);
            viewModel.SelectedArtist = Song.ALL_ARTISTS;
            Console.WriteLine("Roll it! " + DateTime.Now);
            var start = DateTime.Now;
            viewModel.SelectedArtist = "artist2";
            var end = DateTime.Now;
            var diff = end - start;
            IsTrue(diff.TotalMilliseconds <= 500, diff.TotalMilliseconds + "ms - max 500ms!");
        }

        [TestMethod]
        public void StressTest_ManyArtists_ManyAlbums_ManySongs()
        {
            Console.WriteLine("Start it up! " + DateTime.Now);
             ViewModelFaker.InitializeLoadedAdminViewModel(ViewModelFaker.CreateSongsDistinct(200, 20, 20),
                    new FakeViewControl("path"));
            var viewModel = new SelectionTracker(JukeController.Instance.Browser);
            viewModel.SelectedArtist = Song.ALL_ARTISTS;
            Console.WriteLine("Roll it! " + DateTime.Now);
            var start = DateTime.Now;
            viewModel.SelectedArtist = "artist2";
            var end = DateTime.Now;
            var diff = end - start;
            IsTrue(diff.TotalMilliseconds <= 500, diff.TotalMilliseconds + "ms - max 500ms!");
        }

        [TestMethod]
        public void StressTest_ManyArtists_ManyAlbums_FewSongs()
        {
            Console.WriteLine("Start it up! " + DateTime.Now);
            var viewModel =
                ViewModelFaker.InitializeLoadedAdminViewModel(ViewModelFaker.CreateSongsDistinct(200, 20, 5),
                    new FakeViewControl("path"));
            var selector = new SelectionTracker(JukeController.Instance.Browser);
            viewModel.SelectedArtist = Song.ALL_ARTISTS;
            Console.WriteLine("Roll it! " + DateTime.Now);
            var start = DateTime.Now;
            viewModel.SelectedArtist = "artist2";
            var end = DateTime.Now;
            var diff = end - start;
            IsTrue(diff.TotalMilliseconds <= 875, diff.TotalMilliseconds + "ms - max 875ms!");
        }

        [TestMethod]
        public void StressTest_ManyArtists_FewAlbums_FewSongs()
        {
            Console.WriteLine("Start it up! " + DateTime.Now);
            ViewModelFaker.InitializeLoadedAdminViewModel(ViewModelFaker.CreateSongsDistinct(400, 5, 5),
                new FakeViewControl("path"));
            var viewModel = new SelectionTracker(JukeController.Instance.Browser);
            viewModel.SelectedArtist = Song.ALL_ARTISTS;
            Console.WriteLine("Roll it! " + DateTime.Now);
            var start = DateTime.Now;
            viewModel.SelectedArtist = "artist2";
            var end = DateTime.Now;
            var diff = end - start;
            IsTrue(diff.TotalMilliseconds <= 1070, diff.TotalMilliseconds + "ms - max 1070ms!");
        }

        [TestMethod]
        public void StressTest_FewArtists_FewAlbums_ManySongs()
        {
            Console.WriteLine("Start it up! " + DateTime.Now);
            ViewModelFaker.InitializeLoadedAdminViewModel(ViewModelFaker.CreateSongsDistinct(5, 3, 500),
                new FakeViewControl("path"));
            var viewModel = new SelectionTracker(JukeController.Instance.Browser);
            viewModel.SelectedArtist = Song.ALL_ARTISTS;
            Console.WriteLine("Roll it! " + DateTime.Now);
            var start = DateTime.Now;
            viewModel.SelectedArtist = "artist2";
            var end = DateTime.Now;
            var diff = end - start;
            IsTrue(diff.TotalMilliseconds <= 500, diff.TotalMilliseconds + "ms - max 500ms!");
        }
    }
}