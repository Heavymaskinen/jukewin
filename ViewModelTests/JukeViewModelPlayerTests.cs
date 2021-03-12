using System;
using System.Collections.Generic;
using DataModel;
using Juke.Control;
using Juke.Control.Tests;
using Juke.Log;
using Juke.UI.Admin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        [TestMethod]
        public void LoadSongs_LoaderStartedWithPath()
        {
            var engine = CreateFakeLoadEngine(ViewModelFaker.CreateSongs(1, 2, 1));
            var listener = new EventListener();
            var viewControl = new FakeViewControl("path");
            LoaderFactory.SetLoaderInstance(new FakeAsyncSongLoader(engine));

            var viewModel = CreateAdminViewModel(viewControl);
            viewModel.LoadSongs.Execute(this);

            AreEqual("path", engine.Path);
            IsTrue(listener.LoadInitiated);
        }

        [TestMethod]
        public void LoadSongs_ViewControlNotifiedOnCompletion()
        {
            var songs = ViewModelFaker.CreateSongs(1, 2, 1);
            var engine = CreateFakeLoadEngine(songs);

            var viewControl = new FakeViewControl("path");
            LoaderFactory.SetLoaderInstance(new FakeAsyncSongLoader(engine, new FakeSongCollector(songs)));

            var viewModel = CreateAdminViewModel(viewControl);
            viewModel.LoadSongs.Execute(this);
            engine.SignalComplete();
            IsTrue(viewControl.Completed);
        }

        [TestMethod]
        public void LoadSongs_ProgressIsTracked()
        {
            var engine = CreateFakeLoadEngine(ViewModelFaker.CreateSongs(1, 2, 1));
            var listener = new EventListener();
            var viewControl = new FakeViewControl("path");

            var viewModel = CreateAdminViewModel(viewControl);
            viewModel.LoadSongs.Execute(this);
            engine.SignalProgress();
            AreEqual(1, int.Parse(listener.ProgressNoted));
        }

        [TestMethod]
        public void LoadSongs_SongsAreAddedOnCompletion()
        {
            var engine = CreateFakeLoadEngine(ViewModelFaker.CreateSongs(1, 2, 1));
            var viewControl = new FakeViewControl("path");

            var viewModel = CreateAdminViewModel(viewControl);
            viewModel.LoadSongs.Execute(this);
            engine.SignalProgress();
            engine.SignalComplete();
            AreEqual(1, viewModel.Artists.Count);
            AreEqual(3, viewModel.Albums.Count); //Includes ALL_ALBUMS
        }

        [TestMethod]
        public void LoadSongs_PromptReturnsNull_NothingLoaded()
        {
            CreateFakeLoadEngine(ViewModelFaker.CreateSongs(1, 1, 1));
            var listener = new EventListener();
            var viewControl = new FakeViewControl(null);
            var viewModel = CreateAdminViewModel(viewControl);
            viewModel.LoadSongs.Execute(this);
            IsFalse(listener.LoadInitiated);
        }

        [TestMethod]
        public void LoadSongs_ViewModelCollectionsUpdated()
        {
            var viewModel = ViewModelFaker.InitializeLoadedViewModel(ViewModelFaker.CreateSongs(2, 2, 1));

            AreEqual(JukeController.Instance.Browser.Artists.Count, viewModel.Artists.Count);
        }

        [TestMethod]
        public void SelectArtist_AlbumsChanged()
        {
            var viewModel =
                ViewModelFaker.InitializeLoadedViewModel(new List<Song>
                {
                    new("artist1", "album1", "song1"),
                    new("artist2", "album2", "song1")
                });

            viewModel.SelectedArtist = "artist1";

            AreEqual(1, viewModel.Albums.Count);
        }

        [TestMethod]
        public void SelectNullAsArtist_SelectAllAlbums()
        {
            var viewModel =
                ViewModelFaker.InitializeLoadedViewModel(new List<Song>
                {
                    new("artist1", "album1", "song1"),
                    new("artist2", "album2", "song1")
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
            var songs = ViewModelFaker.CreateSongs(1, 2, 4);
            var viewModel = ViewModelFaker.InitializeLoadedViewModel(songs);
            viewModel.SelectedAlbum = "album1";
            AreEqual(4, viewModel.Songs.Count);
        }

        [TestMethod]
        public void SelectNullAsAlbum_SelectAllSongs()
        {
            var songs = ViewModelFaker.CreateSongs(1, 2, 4);
            var viewModel = ViewModelFaker.InitializeLoadedViewModel(songs);
            viewModel.SelectedAlbum = "album1";
            viewModel.SelectedAlbum = null;
            AreEqual(8, viewModel.Songs.Count);
        }

        [TestMethod]
        public void NoSelectedSong_CannotPlay()
        {
            var viewModel = ViewModelFaker.InitializeLoadedViewModel(ViewModelFaker.CreateSongs(1, 2, 3));
            viewModel.SelectedSong = null;
            IsFalse(viewModel.PlaySong.CanExecute(viewModel));
        }

        [TestMethod]
        public void SongSelected_CanPlay()
        {
            var songs = ViewModelFaker.CreateSongs(1, 2, 3);
            var viewModel = ViewModelFaker.InitializeLoadedViewModel(songs);
            viewModel.SelectedSong = null;
            viewModel.SelectedSong = songs[0];
            IsTrue(viewModel.PlaySong.CanExecute(viewModel));
        }

        [TestMethod]
        public void SelectConstantAsArtist_SelectAllSongsForSelectedAlbum()
        {
            var viewModel =
                ViewModelFaker.InitializeLoadedAdminViewModel(new List<Song>
                {
                    new("artist1", "album1", "song1"),
                    new("artist2", "album2", "song1"),
                    new("artist3", "album2", "song2")
                }, new FakeViewControl("path"));
            viewModel.SelectedAlbum = "album2";
            viewModel.SelectedArtist = Song.ALL_ARTISTS;
            AreEqual(3, viewModel.Albums.Count); //Including ALL_ALBUMS
        }

        [TestMethod]
        public void SelectArtistWithConstantAsAlbum_SelectAllSongs()
        {
            var viewModel =
                ViewModelFaker.InitializeLoadedAdminViewModel(new List<Song>
                {
                    new("artist1", "album1", "song1"),
                    new("artist1", "album2", "song1"),
                    new("artist1", "album3", "song1"),
                    new("artist2", "albumA", "song1"),
                    new("artist2", "albumB", "song1")
                }, new FakeViewControl("path"));
            viewModel.SelectedArtist = "artist1";
            viewModel.SelectedAlbum = Song.ALL_ALBUMS;
            AreEqual(3, viewModel.Songs.Count);
        }

        [TestMethod]
        public void SelectAllArtistWithAllAlbums_SelectAllSongs()
        {
            var viewModel =
                ViewModelFaker.InitializeLoadedAdminViewModel(new List<Song>
                {
                    new("artist1", "album1", "song1"),
                    new("artist1", "album2", "song1"),
                    new("artist1", "album3", "song1"),
                    new("artist2", "albumA", "song1"),
                    new("artist2", "albumB", "song1")
                }, new FakeViewControl("path"));
            viewModel.SelectedArtist = Song.ALL_ARTISTS;
            viewModel.SelectedAlbum = Song.ALL_ALBUMS;
            AreEqual(5, viewModel.Songs.Count);
        }

        [TestMethod]
        public void SelectSpecificArtist_AfterAllArtistWithAllAlbums_SelectArtistSongs()
        {
            var viewModel =
                ViewModelFaker.InitializeLoadedAdminViewModel(new List<Song>
                (new List<Song>
                {
                    new("artist1", "album1", "song1"),
                    new("artist1", "album2", "song1"),
                    new("artist1", "album3", "song1"),
                    new("artist2", "albumA", "song1"),
                    new("artist2", "albumB", "song1")
                }), new FakeViewControl("path"));
            viewModel.SelectedArtist = Song.ALL_ARTISTS;
            viewModel.SelectedAlbum = Song.ALL_ALBUMS;
            viewModel.SelectedArtist = "artist1";
            AreEqual(3, viewModel.Songs.Count);
        }

        [TestMethod]
        public void SelectSpecificAlbum_AfterAllArtistWithAllAlbums_SelectAlbumSongs()
        {
            var viewModel =
                ViewModelFaker.InitializeLoadedAdminViewModel(new List<Song>
                (new List<Song>
                {
                    new("artist1", "album1", "song1"),
                    new("artist1", "album2", "song1"),
                    new("artist1", "album3", "song1"),
                    new("artist2", "albumA", "song1"),
                    new("artist2", "albumB", "song1")
                }), new FakeViewControl("path"));
            viewModel.SelectedAlbum = Song.ALL_ALBUMS;
            viewModel.SelectedArtist = Song.ALL_ARTISTS;

            viewModel.SelectedAlbum = "album2";
            AreEqual(1, viewModel.Songs.Count);
        }

        [TestMethod]
        public void ChangeToArtistWithOneAlbum_PreselectAlbum()
        {
            var viewModel = ViewModelFaker.InitializeLoadedAdminViewModel(ViewModelFaker.CreateSongsDistinct(4, 1, 10), new FakeViewControl("path"));
            viewModel.SelectedArtist = "artist2";
            AreEqual("album21",viewModel.SelectedAlbum);
        }
        
        [TestMethod]
        public void ChangeToArtistWithOneAlbum_PreselectSong()
        {
            var viewModel = ViewModelFaker.InitializeLoadedAdminViewModel(ViewModelFaker.CreateSongsDistinct(4, 1, 10), new FakeViewControl("path"));
            viewModel.SelectedArtist = "artist2";
            AreEqual("song211",viewModel.SelectedSong.Name);
        }

        [TestMethod]
        public void NewSelectedArtistHasSameAlbum_AlbumStillSelected()
        {
            var viewModel = ViewModelFaker.InitializeLoadedAdminViewModel(ViewModelFaker.CreateSongsDistinct(4, 2, 10), new FakeViewControl("path"));
            viewModel.SelectedArtist = Song.ALL_ARTISTS;
            viewModel.SelectedAlbum = "album22";
            viewModel.SelectedArtist = "artist2";
            AreEqual("album22",viewModel.SelectedAlbum);
        }
        
        [TestMethod]
        public void AlbumRenamed_NewAlbumSelected()
        {
            var viewModel = ViewModelFaker.InitializeLoadedAdminViewModel(ViewModelFaker.CreateSongsDistinct(4, 2, 10), 
                new FakeViewControl("path") {SongDataToReturn = new SongUpdate(new Song(null, "album22", "artist2"))
                {
                    NewAlbum = "new",
                    NewArtist = "new"
                }});
            viewModel.SelectedArtist = "artist2";
            viewModel.SelectedAlbum = "album22";
            viewModel.EditSong.Execute(InfoType.Album);
            AreEqual("new",viewModel.SelectedArtist);
            AreEqual("new", viewModel.SelectedAlbum);
        }
        
        

        private static FakeSongLoadEngine CreateFakeLoadEngine(List<Song> songs)
        {
            var engine = new FakeSongLoadEngine(songs);
            LoaderFactory.SetLoaderInstance(new FakeAsyncSongLoader(engine, new FakeSongCollector(songs)));
            return engine;
        }

        private AdministratorModel CreateAdminViewModel(FakeViewControl viewControl)
        {
            return new AdminViewModel(viewControl);
        }
    }
}