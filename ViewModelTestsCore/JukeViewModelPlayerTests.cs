using System.Collections.Generic;
using Juke.Control.Tests;
using DataModel;
using Juke.Control;
using NUnit.Framework;
using static NUnit.Framework.Assert;
using System.Threading;

namespace Juke.UI.Tests
{
    [TestFixture]
    public class JukeViewModelTests
    {
        private JukeController controller;
        [SetUp]
        public void Setup()
        {
            controller = (JukeController)JukeController.Instance;
            controller.ClearLibrary();
        }

        [Test]
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

        [Test]
        public void UpdateObervedCollections_EventIsCaught()
        {
            var fakeLoader = CreateFakeLoader (CreateSongs(1,1,10));
            var listener = new EventListener();
            var viewControl = new FakeViewControl("path");
            JukeViewModel viewModel = CreateViewModel(viewControl);
            viewModel.Songs.CollectionChanged += listener.CollectionChangedHandler;
            viewModel.Songs.Add(new Song("artist", "album", "song"));
            Assert.AreEqual(1, listener.CollectionArgs.NewItems.Count);
        }

        [Test]
        public void LoadSongs_ViewModelGetsUpdated()
        {
            JukeViewModel viewModel = CreateViewModel(new FakeViewControl("path"));
            CreateFakeLoader(CreateSongs(1, 1, 10)).AutoComplete = true;
            viewModel.LoadSongs.Execute(this);
            AreEqual(10, viewModel.Songs.Count);
        }

        [Test]
        public void LoadSongs_PromptReturnsNull_NothingLoaded()
        {
            CreateFakeLoader();
            var listener = new EventListener();
            var viewControl = new FakeViewControl(null);
            var viewModel = CreateViewModel(viewControl);
            viewModel.LoadSongs.Execute(this);
            IsFalse(listener.LoadInitiated);
        }

        [Test]
        public void LoadSongs_ViewModelCollectionsUpdated()
        {
            var viewModel = InitializeLoadedViewModel(CreateSongs(2, 2, 1));

            AreEqual(JukeController.Instance.Browser.Artists.Count, viewModel.Artists.Count);
        }

        [Test]
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

        [Test]
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

        [Test]
        public void SelectAlbum_SongsChanged()
        {
            var songs = CreateSongs(1, 2, 4);
            var viewModel = InitializeLoadedViewModel(songs);
            viewModel.SelectedAlbum = "album1";
            AreEqual(4, viewModel.Songs.Count);
        }

        [Test]
        public void SelectNullAsAlbum_SelectAllSongs()
        {
            var songs = CreateSongs(1, 2, 4);
            var viewModel = InitializeLoadedViewModel(songs);
            viewModel.SelectedAlbum = "album1";
            viewModel.SelectedAlbum = null;
            AreEqual(8, viewModel.Songs.Count);
        }

        [Test]
        public void NoSelectedSong_CannotPlay()
        {
            var viewModel = InitializeLoadedViewModel(CreateSongs(1, 2, 3));
            viewModel.SelectedSong = null;
            IsFalse(viewModel.PlaySong.CanExecute(viewModel));
        }

        [Test]
        public void SongSelected_CanPlay()
        {
            var songs = CreateSongs(1, 2, 3);
            var viewModel = InitializeLoadedViewModel(songs);
            viewModel.SelectedSong = null;
            viewModel.SelectedSong = songs[0];
            IsTrue(viewModel.PlaySong.CanExecute(viewModel));
        }


        [Test]
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

        [Test]
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