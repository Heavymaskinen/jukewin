using DataModel;
using Juke.Control;
using Juke.Control.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Juke.Log;
using Juke.UI.Admin;
using Stubs2;
using ViewModelCommands;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Juke.UI.Tests
{
    [TestClass]
    public class JukeViewModelTests
    {
        [TestInitialize]
        public void Setup()
        {
            JukeController.Reset();
            Logger.ConsoleLog();
        }

        [TestMethod()]
        public void LoadSongs_LoaderStartedWithPath()
        {
            FakeSongLoadEngine engine = CreateFakeLoadEngine(CreateSongs(1, 2, 1));
            var listener = new EventListener();
            var viewControl = new FakeViewControl("path");
            LoaderFactory.SetLoaderInstance(new FakeAsyncSongLoader(engine));

            var viewModel = CreateAdminViewModel(viewControl);
            viewModel.LoadSongs.Execute(this);

            Assert.AreEqual("path", engine.Path);
            IsTrue(listener.LoadInitiated);
        }

        [TestMethod()]
        public void LoadSongs_ViewControlNotifiedOnCompletion()
        {
            var songs = CreateSongs(1, 2, 1);
            var engine = CreateFakeLoadEngine(songs);

            var viewControl = new FakeViewControl("path");
            LoaderFactory.SetLoaderInstance(new FakeAsyncSongLoader(engine, new FakeSongCollector(songs)));

            var viewModel = CreateAdminViewModel(viewControl);
            viewModel.LoadSongs.Execute(this);
            engine.SignalComplete();
            IsTrue(viewControl.Completed);
        }

        [TestMethod()]
        public void LoadSongs_ProgressIsTracked()
        {
            FakeSongLoadEngine engine = CreateFakeLoadEngine(CreateSongs(1, 2, 1));
            var listener = new EventListener();
            var viewControl = new FakeViewControl("path");

            var viewModel = CreateAdminViewModel(viewControl);
            viewModel.LoadSongs.Execute(this);
            engine.SignalProgress();
            AreEqual(1, int.Parse(listener.ProgressNoted));
        }

        [TestMethod()]
        public void LoadSongs_SongsAreAddedOnCompletion()
        {
            FakeSongLoadEngine engine = CreateFakeLoadEngine(CreateSongs(1, 2, 1));
            var viewControl = new FakeViewControl("path");

            var viewModel = CreateAdminViewModel(viewControl);
            viewModel.LoadSongs.Execute(this);
            engine.SignalProgress();
            engine.SignalComplete();
            AreEqual(1, viewModel.Artists.Count);
            AreEqual(3, viewModel.Albums.Count);    //Includes ALL_ALBUMS
        }

        [TestMethod]
        public void LoadSongs_PromptReturnsNull_NothingLoaded()
        {
            CreateFakeLoadEngine(CreateSongs(1, 1, 1));
            var listener = new EventListener();
            var viewControl = new FakeViewControl(null);
            var viewModel = CreateAdminViewModel(viewControl);
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
            AreEqual(2, viewModel.Albums.Count);
            AreEqual("album1", viewModel.Albums[0]);
            AreEqual("album2", viewModel.Albums[1]);
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
        public void SelectConstantAsArtist_SelectAllSongsForSelectedAlbum()
        {
            var viewModel =
                InitializeLoadedAdminViewModel(new List<Song>
                {
                    new Song("artist1", "album1", "song1"),
                    new Song("artist2", "album2", "song1"),
                    new Song("artist3", "album2", "song2")
                });
            viewModel.SelectedAlbum = "album2";
            viewModel.SelectedArtist = Song.ALL_ARTISTS;
            AreEqual(3, viewModel.Albums.Count); //Including ALL_ALBUMS
        }

        [TestMethod]
        public void SelectArtistWithConstantAsAlbum_SelectAllSongs()
        {
            var viewModel =
                InitializeLoadedAdminViewModel(new List<Song>
                {
                    new Song("artist1", "album1", "song1"),
                    new Song("artist1", "album2", "song1"),
                    new Song("artist1", "album3", "song1"),
                    new Song("artist2", "albumA", "song1"),
                    new Song("artist2", "albumB", "song1")
                });
            viewModel.SelectedArtist = "artist1";
            viewModel.SelectedAlbum = Song.ALL_ALBUMS;
            AreEqual(3, viewModel.Songs.Count);
        }
        
        [TestMethod]
        public void SelectAllArtistWithAllAlbums_SelectAllSongs()
        {
            var viewModel =
                InitializeLoadedAdminViewModel(new List<Song>
                {
                    new Song("artist1", "album1", "song1"),
                    new Song("artist1", "album2", "song1"),
                    new Song("artist1", "album3", "song1"),
                    new Song("artist2", "albumA", "song1"),
                    new Song("artist2", "albumB", "song1")
                });
            viewModel.SelectedArtist = Song.ALL_ARTISTS;
            viewModel.SelectedAlbum = Song.ALL_ALBUMS;
            AreEqual(5, viewModel.Songs.Count);
        }

        [TestMethod]
        public void SelectSpecificArtist_AfterAllArtistWithAllAlbums_SelectArtistSongs()
        {
            var viewModel =
                InitializeLoadedAdminViewModel(new List<Song>
                (new List<Song>
                {
                    new Song("artist1", "album1", "song1"),
                    new Song("artist1", "album2", "song1"),
                    new Song("artist1", "album3", "song1"),
                    new Song("artist2", "albumA", "song1"),
                    new Song("artist2", "albumB", "song1")
                }));
            viewModel.SelectedArtist = Song.ALL_ARTISTS;
            viewModel.SelectedAlbum = Song.ALL_ALBUMS;
            viewModel.SelectedArtist = "artist1";
            AreEqual(3, viewModel.Songs.Count);
        }

        [TestMethod]
        public void SelectSpecificAlbum_AfterAllArtistWithAllAlbums_SelectAlbumSongs()
        {
            var viewModel =
                InitializeLoadedAdminViewModel(new List<Song>
                (new List<Song>
                {
                    new Song("artist1", "album1", "song1"),
                    new Song("artist1", "album2", "song1"),
                    new Song("artist1", "album3", "song1"),
                    new Song("artist2", "albumA", "song1"),
                    new Song("artist2", "albumB", "song1")
                }));
            viewModel.SelectedAlbum = Song.ALL_ALBUMS;
            viewModel.SelectedArtist = Song.ALL_ARTISTS;
            
            viewModel.SelectedAlbum = "album2";
            AreEqual(1, viewModel.Songs.Count);
        }

        private SelectionModel InitializeLoadedViewModel(List<Song> songs)
        {
            var fakeLoader = CreateFakeLoadEngine(songs);
            var viewControl = new FakeViewControl("path");
            var viewModel = CreateViewModel(viewControl);
            CreateAdminViewModel(viewControl).LoadSongs.Execute(this);

            fakeLoader.SignalComplete();
            return viewModel;
        }

        private SelectionModel InitializeLoadedAdminViewModel(List<Song> songs)
        {
            var fakeLoader = CreateFakeLoadEngine(songs);
            var viewControl = new FakeViewControl("path");
            var viewModel = CreateAdminViewModel(viewControl);
            viewModel.LoadSongs.Execute(this);
            fakeLoader.SignalComplete();
            return viewModel;
        }

        private static FakeSongLoadEngine CreateFakeLoadEngine(List<Song> songs)
        {
            FakeSongLoadEngine engine = new FakeSongLoadEngine(songs);
            LoaderFactory.SetLoaderInstance(new FakeAsyncSongLoader(engine, new FakeSongCollector(songs)));
            return engine;
        }

        private SelectionModel CreateViewModel(FakeViewControl viewControl)
        {
            return new JukeViewModel(viewControl);
        }

        private AdministratorModel CreateAdminViewModel(FakeViewControl viewControl)
        {
            return new AdminViewModel(viewControl);
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