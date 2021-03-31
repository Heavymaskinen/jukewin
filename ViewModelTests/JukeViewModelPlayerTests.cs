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
            var listener = new EventListener(JukeController.Instance);
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
            var listener = new EventListener(JukeController.Instance);
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
            AreEqual(1, viewModel.SelectionTracker.Artists.Count);
            AreEqual(2, viewModel.SelectionTracker.Albums.Count);
        }

        [TestMethod]
        public void LoadSongs_PromptReturnsNull_NothingLoaded()
        {
            CreateFakeLoadEngine(ViewModelFaker.CreateSongs(1, 1, 1));
            var listener = new EventListener(JukeController.Instance);
            var viewControl = new FakeViewControl(null);
            var viewModel = CreateAdminViewModel(viewControl);
            viewModel.LoadSongs.Execute(this);
            IsFalse(listener.LoadInitiated);
        }

        [TestMethod]
        public void LoadSongs_ViewModelCollectionsUpdated_InclueAllArtistsOption()
        {
            ViewModelFaker.InitializeLoadedViewModel(ViewModelFaker.CreateSongs(2, 2, 1));
            var viewModel = new SelectionTracker(JukeController.Instance.Browser);
            AreEqual(JukeController.Instance.Browser.Artists.Count+1, viewModel.Artists.Count); //Including ALL_ARTISTS
        }

        [TestMethod]
        public void SelectArtist_AlbumsChanged()
        {
            ViewModelFaker.InitializeLoadedViewModel(new List<Song>
                {
                    new("artist1", "album1", "song1"),
                    new("artist2", "album2", "song1")
                });
            var viewModel = new SelectionTracker(JukeController.Instance.Browser);
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

            var selector = new SelectionTracker(JukeController.Instance.Browser);
            selector.SelectedArtist = "artist1";
            selector.SelectedArtist = null;
            AreEqual(3, selector.Albums.Count); //Including ALL_ALBUMS
            AreEqual("album1", selector.Albums[1]);
            AreEqual("album2", selector.Albums[2]);
        }

        [TestMethod]
        public void AllArtistsSelected_SelectAlbumAndSong_SelectAlbumDifferentArtist()
        {
            ViewModelFaker.InitializeLoadedViewModel(new List<Song>
                {
                    new("artist1", "album1", "song1"),
                    new("artist2", "album2", "song1")
                });

            var selector = new SelectionTracker(JukeController.Instance.Browser);
            selector.SelectedArtist = null;
            selector.SelectedAlbum = "album1";
            selector.SelectedSong = selector.Songs[0];

            selector.SelectedAlbum = "album2";
            AreEqual("artist2", selector.SelectedSong.Artist);
        }

        [TestMethod]
        public void SelectAlbum_SongsChanged()
        {
            var songs = ViewModelFaker.CreateSongs(1, 2, 4);
            var viewModel = ViewModelFaker.InitializeLoadedViewModel(songs);
            var selector = new SelectionTracker(JukeController.Instance.Browser);
            selector.SelectedAlbum = "album1";
            AreEqual(4, selector.Songs.Count);
        }

        [TestMethod]
        public void SelectNullAsAlbum_SelectAllSongs()
        {            
            var songs = ViewModelFaker.CreateSongs(1, 2, 4);
            var viewModel = ViewModelFaker.InitializeLoadedViewModel(songs);
            var selector = new SelectionTracker(JukeController.Instance.Browser);
            selector.SelectedAlbum = "album1";
            selector.SelectedAlbum = null;
            AreEqual(8, selector.Songs.Count);
        }

        [TestMethod]
        public void NoSelectedSong_CannotPlay()
        {
            var viewModel = ViewModelFaker.InitializeLoadedViewModel(ViewModelFaker.CreateSongs(1, 2, 3));
            viewModel.SelectionTracker.SelectedSong = null;
            IsFalse(viewModel.PlaySong.CanExecute(viewModel));
        }

        [TestMethod]
        public void SongSelected_CanPlay()
        {
            var songs = ViewModelFaker.CreateSongs(1, 2, 3);
            var viewModel = ViewModelFaker.InitializeLoadedViewModel(songs);
            viewModel.SelectionTracker.SelectedSong = null;
            viewModel.SelectionTracker.SelectedSong = songs[0];
            IsTrue(viewModel.PlaySong.CanExecute(viewModel));
        }

        [TestMethod]
        public void SelectConstantAsArtist_SelectAllSongsForSelectedAlbum()
        {
            ViewModelFaker.InitializeLoadedAdminViewModel(new List<Song>
                {
                    new("artist1", "album1", "song1"),
                    new("artist2", "album2", "song1"),
                    new("artist3", "album2", "song2")
                }, new FakeViewControl("path"));
            var selector = new SelectionTracker(JukeController.Instance.Browser);
            selector.SelectedAlbum = "album2";
            selector.SelectedArtist = Song.ALL_ARTISTS;
            AreEqual(3, selector.Albums.Count); //Including ALL_ALBUMS
        }

        [TestMethod]
        public void SelectArtistWithConstantAsAlbum_SelectAllSongs()
        {
                ViewModelFaker.InitializeLoadedAdminViewModel(new List<Song>
                {
                    new("artist1", "album1", "song1"),
                    new("artist1", "album2", "song1"),
                    new("artist1", "album3", "song1"),
                    new("artist2", "albumA", "song1"),
                    new("artist2", "albumB", "song1")
                }, new FakeViewControl("path"));
            var selector = new SelectionTracker(JukeController.Instance.Browser);
            selector.SelectedArtist = "artist1";
            selector.SelectedAlbum = Song.ALL_ALBUMS;
            AreEqual(3, selector.Songs.Count);
        }

        [TestMethod]
        public void SelectAllArtistWithAllAlbums_SelectAllSongs()
        {
                ViewModelFaker.InitializeLoadedAdminViewModel(new List<Song>
                {
                    new("artist1", "album1", "song1"),
                    new("artist1", "album2", "song1"),
                    new("artist1", "album3", "song1"),
                    new("artist2", "albumA", "song1"),
                    new("artist2", "albumB", "song1")
                }, new FakeViewControl("path"));
            var selector = new SelectionTracker(JukeController.Instance.Browser);
            selector.SelectedArtist = Song.ALL_ARTISTS;
            selector.SelectedAlbum = Song.ALL_ALBUMS;
            AreEqual(5, selector.Songs.Count);
        }

        [TestMethod]
        public void SelectSpecificArtist_AfterAllArtistWithAllAlbums_SelectArtistSongs()
        {
                ViewModelFaker.InitializeLoadedAdminViewModel(new List<Song>
                (new List<Song>
                {
                    new("artist1", "album1", "song1"),
                    new("artist1", "album2", "song1"),
                    new("artist1", "album3", "song1"),
                    new("artist2", "albumA", "song1"),
                    new("artist2", "albumB", "song1")
                }), new FakeViewControl("path"));
            var selector = new SelectionTracker(JukeController.Instance.Browser);
            selector.SelectedArtist = Song.ALL_ARTISTS;
            selector.SelectedAlbum = Song.ALL_ALBUMS;
            selector.SelectedArtist = "artist1";
            AreEqual(3, selector.Songs.Count);
        }

        [TestMethod]
        public void SelectSpecificAlbum_AfterAllArtistWithAllAlbums_SelectAlbumSongs()
        {
                ViewModelFaker.InitializeLoadedAdminViewModel(new List<Song>
                (new List<Song>
                {
                    new("artist1", "album1", "song1"),
                    new("artist1", "album2", "song1"),
                    new("artist1", "album3", "song1"),
                    new("artist2", "albumA", "song1"),
                    new("artist2", "albumB", "song1")
                }), new FakeViewControl("path"));
            var selector = new SelectionTracker(JukeController.Instance.Browser);
            selector.SelectedAlbum = Song.ALL_ALBUMS;
            selector.SelectedArtist = Song.ALL_ARTISTS;

            selector.SelectedAlbum = "album2";
            AreEqual(1, selector.Songs.Count);
        }

        [TestMethod]
        public void ChangeToArtistWithOneAlbum_PreselectAlbum()
        {
            var viewModel = ViewModelFaker.InitializeLoadedAdminViewModel(ViewModelFaker.CreateSongsDistinct(4, 1, 10), new FakeViewControl("path"));
            var selector = new SelectionTracker(JukeController.Instance.Browser);
            selector.SelectedArtist = "artist2";
            AreEqual("album21", selector.SelectedAlbum);
        }
        
        [TestMethod]
        public void ChangeToArtistWithOneAlbum_PreselectSong()
        {
            
            var viewModel = ViewModelFaker.InitializeLoadedAdminViewModel(ViewModelFaker.CreateSongsDistinct(4, 1, 10), new FakeViewControl("path"));
            var selector = new SelectionTracker(JukeController.Instance.Browser); 
            selector.SelectedArtist = "artist2";
            AreEqual("song211", selector.SelectedSong.Name);
        }

        [TestMethod]
        public void NewSelectedArtistHasSameAlbum_AlbumStillSelected()
        {
            ViewModelFaker.InitializeLoadedAdminViewModel(ViewModelFaker.CreateSongsDistinct(4, 2, 10), new FakeViewControl("path"));
            var selector = new SelectionTracker(JukeController.Instance.Browser);
            selector.SelectedArtist = Song.ALL_ARTISTS;
            selector.SelectedAlbum = "album22";
            selector.SelectedArtist = "artist2";
            AreEqual("album22", selector.SelectedAlbum);
        }
        
        [TestMethod]
        public void AlbumRenamed_NewAlbumSelected()
        {
            var viewModel= ViewModelFaker.InitializeLoadedAdminViewModel(ViewModelFaker.CreateSongsDistinct(4, 2, 10), 
                new FakeViewControl("path") {SongDataToReturn = new SongUpdate(new Song("artist2", "album22", null))
                {
                    NewAlbum = "new",
                    NewArtist = "new"
                }});
            
            viewModel.SelectionTracker.SelectedArtist = "artist2";
            viewModel.SelectionTracker.SelectedAlbum = "album22";
            viewModel.EditSong.Execute(InfoType.Album);
            AreEqual("new", viewModel.SelectionTracker.SelectedArtist);
            AreEqual("new", viewModel.SelectionTracker.SelectedAlbum);
        }

        [TestMethod]
        public void AlbumRenamed_MovedFromArtist()
        {
            var viewModel = ViewModelFaker.InitializeLoadedAdminViewModel(ViewModelFaker.CreateSongsDistinct(4, 2, 10),
                new FakeViewControl("path")
                {
                    SongDataToReturn = new SongUpdate(new Song("artist2", "album22", null))
                    {
                        NewAlbum = "new",
                        NewArtist = "new"
                    }
                });

            var listener = new ModelUpdateListener(viewModel);

            viewModel.SelectionTracker.SelectedArtist = "artist2";
            viewModel.SelectionTracker.SelectedAlbum = "album22";
            viewModel.EditSong.Execute(InfoType.Album);

            viewModel.SelectionTracker.SelectedArtist = "artist2";
            AreEqual(1, viewModel.SelectionTracker.Albums.Count);
            AreEqual(6, viewModel.SelectionTracker.Artists.Count); //Including ALL_ARTISTS
            Assert.AreEqual("SelectionTracker", listener.PropertyChanged);
        }

        [TestMethod]
        public void AlbumRenamed_HasCorrectInfo()
        {
            Logger.ConsoleLog();
            var viewModel = ViewModelFaker.InitializeLoadedAdminViewModel(ViewModelFaker.CreateSongsDistinct(4, 2, 10),
                new FakeViewControl("path")
                {
                    SongDataToReturn = new SongUpdate(new Song("artist2", "album22", "ignore this"))
                    {
                        NewAlbum = "new",
                        NewArtist = "new"
                    }
                });           

            viewModel.SelectionTracker.SelectedSong = viewModel.SelectionTracker.Songs[0];
            viewModel.SelectionTracker.SelectedArtist = "artist2";
            viewModel.SelectionTracker.SelectedAlbum = "album22";
            
            var listener = new ModelUpdateListener(viewModel);
            viewModel.EditSong.Execute(InfoType.Album);
            Assert.AreEqual("SelectionTracker", listener.PropertyChanged);

            viewModel.SelectionTracker.SelectedArtist = "new";
            
            AreEqual(1, viewModel.SelectionTracker.Albums.Count);
            AreEqual(10, viewModel.SelectionTracker.Songs.Count);
            AreEqual("new", viewModel.SelectionTracker.SelectedAlbum);
            AreEqual("song221", viewModel.SelectionTracker.Songs[0].Name);
            AreEqual("song222", viewModel.SelectionTracker.Songs[2].Name);
        }


        [TestMethod]
        public void AlbumRenamed_AlbumsMerged()
        {
            var viewModel = ViewModelFaker.InitializeLoadedAdminViewModel(ViewModelFaker.CreateSongsDistinct(4, 2, 10),
                new FakeViewControl("path")
                {
                    SongDataToReturn = new SongUpdate(new Song("artist2", "album22", "ignore this"))
                    {
                        NewArtist = "artist1",
                        NewAlbum = "album11"
                    }
                });

            viewModel.SelectionTracker.SelectedSong = viewModel.SelectionTracker.Songs[0];
            viewModel.SelectionTracker.SelectedArtist = "artist2";
            viewModel.SelectionTracker.SelectedAlbum = "album22";

            var listener = new ModelUpdateListener(viewModel);
            viewModel.EditSong.Execute(InfoType.Album);
            Assert.AreEqual("SelectionTracker", listener.PropertyChanged);

            viewModel.SelectionTracker.SelectedArtist = "artist2";
            AreEqual(1, viewModel.SelectionTracker.Albums.Count);

            viewModel.SelectionTracker.SelectedArtist = "artist1";
            AreEqual(3, viewModel.SelectionTracker.Albums.Count); //Including ALL_ALBUMS
            viewModel.SelectionTracker.SelectedAlbum = "album11";
            AreEqual(20, viewModel.SelectionTracker.Songs.Count);
        }

        [TestMethod]
        public void SongRenamed_HasCorrectInfo()
        {
            Logger.ConsoleLog();
            var viewModel = ViewModelFaker.InitializeLoadedAdminViewModel(ViewModelFaker.CreateSongsDistinct(4, 2, 10),
                new FakeViewControl("path")
                {
                    SongDataToReturn = new SongUpdate(new Song("artist2", "album22", "song221"))
                    {
                        NewAlbum = "new",
                        NewArtist = "new"
                    }
                });

            viewModel.SelectionTracker.SelectedSong = viewModel.SelectionTracker.Songs[0];
            viewModel.SelectionTracker.SelectedArtist = "artist2";
            viewModel.SelectionTracker.SelectedAlbum = "album22";

            var listener = new ModelUpdateListener(viewModel);
            viewModel.EditSong.Execute(InfoType.Song);
            Assert.AreEqual("SelectionTracker", listener.PropertyChanged);

            viewModel.SelectionTracker.SelectedArtist = "new";

            AreEqual("new", viewModel.SelectionTracker.SelectedSong.Album);
            AreEqual("song221", viewModel.SelectionTracker.SelectedSong.Name);
        }

        [TestMethod]
        public void SongRenamed_RemainSelected()
        {
            Logger.ConsoleLog();
            var viewModel = ViewModelFaker.InitializeLoadedAdminViewModel(ViewModelFaker.CreateSongsDistinct(4, 2, 10),
                new FakeViewControl("path")
                {
                    SongDataToReturn = new SongUpdate(new Song("artist2", "album22", "song221"))
                    {
                        NewAlbum = "new",
                        NewArtist = "new"
                    }
                });

            viewModel.SelectionTracker.SelectedSong = viewModel.SelectionTracker.Songs[0];
            viewModel.SelectionTracker.SelectedArtist = "artist2";
            viewModel.SelectionTracker.SelectedAlbum = "album22";

            var listener = new ModelUpdateListener(viewModel);
            viewModel.EditSong.Execute(InfoType.Song);
            Assert.AreEqual("SelectionTracker", listener.PropertyChanged);

            AreEqual("new", viewModel.SelectionTracker.SelectedSong.Album);
            AreEqual("new", viewModel.SelectionTracker.SelectedSong.Artist);
            AreEqual("song221", viewModel.SelectionTracker.SelectedSong.Name);
        }

        class ModelUpdateListener
        {
            public ModelUpdateListener(SelectionModel model)
            {
                model.PropertyChanged += Model_PropertyChanged;
            }

            public string PropertyChanged { get; private set; }

            private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                PropertyChanged = e.PropertyName;
            }
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