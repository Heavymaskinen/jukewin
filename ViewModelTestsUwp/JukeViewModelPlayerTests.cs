using System.Collections.Generic;
using Juke.Control.Tests;
using DataModel;
using Juke.Control;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juke.UI.Tests
{
    public class JukeViewModelTests
    {
        [TestInitialize()]
        public void Setup()
        {
            JukeController.Instance.ClearLibrary();
        }

        [TestMethod()]
        public void LoadSongs_LoaderStartedWithPath()
        {
            FakeAsyncSongLoader fakeLoader = CreateFakeLoader();
            var listener = new EventListener();
            var viewControl = new FakeViewControl("path");

            JukeViewModel viewModel = CreateViewModel(viewControl);
            viewModel.LoadSongs.Execute(this);
            AreEqual("path", fakeLoader.Path);
            IsTrue(listener.LoadInitiated);
        }

        [TestMethod]
        public void LoadSongs_PromptReturnsNull_NothingLoaded()
        {
            var fakeLoader = CreateFakeLoader();
            var listener = new EventListener();
            var viewControl = new FakeViewControl(null);
            var viewModel = CreateViewModel(viewControl);
            viewModel.LoadSongs.Execute(this);
            IsFalse(listener.LoadInitiated);
        }

        [TestMethod]
        public void LoadSongs_ViewModelCollectionsUpdated()
        {
            var viewModel = InitializeLoadedViewModel(CreateSongs(2, 2, 1));

            AreEqual(JukeController.Instance.Browser.Artists.Count, viewModel.Artists.Count);
        }

        [TestMethod]
        public void SelectArtist_AlbumsChanged()
        {
            var viewModel =
                InitializeLoadedViewModel(new List<Song>
                {
                    new Song("artist1", "album1", "song1"),
                    new Song("artist2", "album2", "song1")
                });
            viewModel.SelectedArtist = "artist1";
            AreEqual(1, viewModel.Albums.Count);
        }

        [TestMethod]
        public void SelectNullAsArtist_SelectAllAlbums()
        {
            var viewModel =
                InitializeLoadedViewModel(new List<Song>
                {
                    new Song("artist1", "album1", "song1"),
                    new Song("artist2", "album2", "song1")
                });
            viewModel.SelectedArtist = "artist1";
            viewModel.SelectedArtist = null;
        }

        [TestMethod]
        public void SelectAlbum_SongsChanged()
        {
            var songs = CreateSongs(1, 2, 4);
            var viewModel = InitializeLoadedViewModel(songs);
            viewModel.SelectedAlbum = "album1";
            AreEqual(4, viewModel.Songs.Count);
        }

        [TestMethod]
        public void SelectNullAsAlbum_SelectAllSongs()
        {
            var songs = CreateSongs(1, 2, 4);
            var viewModel = InitializeLoadedViewModel(songs);
            viewModel.SelectedAlbum = "album1";
            viewModel.SelectedAlbum = null;
            AreEqual(8, viewModel.Songs.Count);
        }

        [TestMethod]
        public void NoSelectedSong_CannotPlay()
        {
            var viewModel = InitializeLoadedViewModel(CreateSongs(1, 2, 3));
            viewModel.SelectedSong = null;
            IsFalse(viewModel.PlaySong.CanExecute(viewModel));
        }

        [TestMethod]
        public void SongSelected_CanPlay()
        {
            var songs = CreateSongs(1, 2, 3);
            var viewModel = InitializeLoadedViewModel(songs);
            viewModel.SelectedSong = null;
            viewModel.SelectedSong = songs[0];
            IsTrue(viewModel.PlaySong.CanExecute(viewModel));
        }


        [TestMethod]
        public void SelectConstantAsArtist_SelectAllAlbums()
        {
            var viewModel =
                InitializeLoadedViewModel(new List<Song>
                {
                    new Song("artist1", "album1", "song1"),
                    new Song("artist2", "album2", "song1")
                });
            viewModel.SelectedArtist = "artist1";
            viewModel.SelectedArtist = Song.ALL_ARTISTS;
            AreEqual(2, viewModel.Albums.Count);
        }

        [TestMethod]
        public void SelectConstantAsAlbum_SelectAllSongs()
        {
            var songs = CreateSongs(1, 2, 4);
            var viewModel = InitializeLoadedViewModel(songs);
            viewModel.SelectedAlbum = "album1";
            viewModel.SelectedAlbum = Song.ALL_ALBUMS;
            AreEqual(8, viewModel.Songs.Count);
        }

        private JukeViewModel InitializeLoadedViewModel(IList<Song> songs)
        {
            var fakeLoader = CreateFakeLoader(songs);
            var viewControl = new FakeViewControl("path");
            var viewModel = CreateViewModel(viewControl);
            viewModel.LoadSongs.Execute(this);
            fakeLoader.SignalComplete();
            return viewModel;
        }

        private FakeAsyncSongLoader CreateFakeLoader()
        {
            var fakeLoader = new FakeAsyncSongLoader();
            LoaderFactory.SetLoaderInstance(fakeLoader);
            return fakeLoader;
        }

        private FakeAsyncSongLoader CreateFakeLoader(IList<Song> songs)
        {
            var fakeLoader = new FakeAsyncSongLoader(songs);
            LoaderFactory.SetLoaderInstance(fakeLoader);
            return fakeLoader;
        }

        private JukeViewModel CreateViewModel(FakeViewControl viewControl)
        {
            return new JukeViewModel(viewControl, new FakePlayerEngine());
        }

        private List<Song> CreateSongs(int artistMax, int albumMax, int songmax)
        {
            var songList = new List<Song>();
            for (int artist = 1; artist <= artistMax; artist++)
            {
                for (int album = 1; album <= albumMax; album++)
                {
                    for (int song = 1; song <= songmax; song++)
                    {
                        songList.Add(new Song("artist" + artist, "album" + album, "song" + song,
                            (artist + song - 1) + "", artist + "/" + album + "/" + song));
                    }
                }
            }

            return songList;
        }
    }
}