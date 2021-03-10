
using System.Collections.Generic;
using System.Threading;
using DataModel;
using Juke.Control;
using Juke.Control.Tests;
using Juke.Core;
using Juke.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stubs2;

namespace JukeControllerTests
{
    [TestClass]
    public class JukeControllerLibraryTests
    {
        private JukeController control;
        private EventListener listener;

        [TestInitialize()]
        public void Setup()
        {
            control = JukeController.Create();
            listener = new EventListener();
        }

        [TestMethod]
        public void InitialState_NoSongs()
        {
            var songs = control.Browser.Songs;
            Assert.AreEqual(0, songs.Count);
        }

        [TestMethod]
        public void LoadSongs_ContainsOne()
        {
            FakeLoad(CreateSongs(1, 1, 1));
            var songs = control.Browser.Songs;
            Assert.AreEqual(1, songs.Count);
        }

        [TestMethod]
        public void LoadOneSong_GetByArtist()
        {
            FakeLoad(CreateSongs(1, 1, 1));
            var songs = control.Browser.GetSongsByArtist("artist1");
            Assert.AreEqual(1, songs.Count);
            Assert.AreEqual("song1", songs[0].Name);
        }

        [TestMethod]
        public void LoadAsync_NotifiedOfStart()
        {
            control.LoadHandler.LoadSongs(FakeAsyncLoad(new List<Song> { }), listener).Wait();
            Thread.Sleep(1);
            Assert.IsTrue(listener.LoadInitiated);
        }

        [TestMethod]
        public void InitialState_NoLoadNotification()
        {
            var loader = FakeAsyncLoad(new List<Song> { });
            Assert.IsFalse(listener.LoadInitiated);
        }

        [TestMethod]
        public void LoadAsync_NotifiedOfEnd()
        {
            var engine = new FakeSongLoadEngine();
            IAsyncSongLoader loader = new AsyncSongLoader(engine);
            WaitedLoad(loader);

            Assert.IsTrue(listener.LoadCompleted);
        }

        [TestMethod]
        public void LoadAsync_NotCompleteNotNotified()
        {
            WaitedLoadInternalListener(FakeAsyncLoad(new List<Song> { }));
            Assert.IsFalse(listener.LoadCompleted);
        }

        [TestMethod]
        public void LoadAsync_GetNotifiedOfProgress()
        {
            var engine = new FakeSongLoadEngine();
            IAsyncSongLoader loader = new AsyncSongLoader(engine);

            WaitedLoad(loader);
            engine.SignalProgress();
            Assert.AreEqual("1", listener.ProgressNoted);
        }

        [TestMethod]
        public void LoadAsyncCompleted_SongsAddedToLibrary()
        {
            var loader = FakeAsyncLoad(CreateSongs(1, 1, 1));
            WaitedLoadInternalListener(loader);
            Assert.AreEqual(1, control.Browser.Songs.Count);
        }

        [TestMethod]
        public void LoadSecondTime_SongsAdded()
        {
            FakeLoad(CreateSongs(1, 1, 1));
            FakeLoad(new List<Song> { new Song("artist", "album", "funky song") });
            Assert.AreEqual(2, control.Browser.Songs.Count);
        }

        [TestMethod]
        public void LoadSongs_NoDuplicates()
        {
            var songs = CreateSongs(1, 1, 1);
            FakeLoad(songs);
            FakeLoad(songs);
            Assert.AreEqual(1, control.Browser.Songs.Count);
        }

        [TestMethod]
        public void LoadSongs_SameTitleDifferentArtist()
        {
            FakeLoad(new List<Song> { new Song("artist", "album", "funky song"), new Song("artist1", "album", "funky song") });
            Assert.AreEqual(2, control.Browser.Songs.Count);
        }

        [TestMethod]
        public void SaveLibrary()
        {
            var songs = CreateSongs(2, 2, 2);
            FakeLoad(songs);
            var writer = new FakeSongCatalogue();
            control.SaveLibrary(writer);
            Assert.AreEqual(songs.Count, writer.WrittenSongs.Count);
        }

        [TestMethod]
        public void LoadLibrary()
        {
            var songs = CreateSongs(2, 2, 2);
            var loader = new FakeSongCatalogue(songs);
            control.LoadLibrarySync(loader);
            Assert.AreEqual(songs.Count, control.Browser.Songs.Count);
        }

        [TestMethod]
        public void GetByArtist_OneResult()
        {
            FakeLoad(CreateSongs(2, 1, 1));
            var songs = control.Browser.GetSongsByArtist("artist2");
            Assert.AreEqual(1, songs.Count);
            Assert.AreEqual("artist2", songs[0].Artist);
            Assert.AreEqual("song1", songs[0].Name);
        }

        [TestMethod]
        public void GetByArtists_NoResult()
        {
            var songs = control.Browser.GetSongsByArtist("artist2");
            Assert.AreEqual(0, songs.Count);
        }

        [TestMethod]
        public void GetByArtists_TwoResults()
        {
            FakeLoad(CreateSongs(1, 1, 2));
            var songs = control.Browser.GetSongsByArtist("artist1");
            Assert.AreEqual(2, CreateSongs(1, 1, 2).Count);
        }

        [TestMethod]
        public void GetArtists_NoResult()
        {
            var result = control.Browser.Artists;
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetArtists_OneResult()
        {
            FakeLoad(CreateSongs(1, 1, 1));
            var result = control.Browser.Artists;
            Assert.AreEqual("artist1", result[0]);
        }

        [TestMethod]
        public void GetArtists_TwoResults()
        {
            FakeLoad(CreateSongs(2, 1, 1));
            var result = control.Browser.Artists;
            Assert.AreEqual("artist2", result[1]);
        }

        [TestMethod]
        public void GetArtists_NoDuplicates()
        {
            FakeLoad(CreateSongs(1, 1, 3));
            var result = control.Browser.Artists;
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void GetAlbums_NoResult()
        {
            var albums = control.Browser.Albums;
            Assert.AreEqual(0, albums.Count);
        }

        [TestMethod]
        public void GetAlbums_OnEResult()
        {
            FakeLoad(CreateSongs(1, 1, 1));
            var albums = control.Browser.Albums;
            Assert.AreEqual("album1", albums[0]);
        }

        [TestMethod]
        public void GetAlbums_TwoResults()
        {
            FakeLoad(CreateSongs(1, 2, 1));
            var albums = control.Browser.Albums;
            Assert.AreEqual("album2", albums[1]);
        }

        [TestMethod]
        public void GetAlbums_NoDupliates()
        {
            FakeLoad(CreateSongs(1, 1, 3));
            var albums = control.Browser.Albums;
            Assert.AreEqual(1, albums.Count);
        }

        [TestMethod]
        public void GetSongsByAlbum_NoResut()
        {
            var songs = control.Browser.GetSongsByAlbum("album1");
            Assert.AreEqual(0, songs.Count);
        }

        [TestMethod]
        public void GetSongsByAlbum_OneResult()
        {
            FakeLoad(CreateSongs(1, 3, 1));
            var songs = control.Browser.GetSongsByAlbum("album1");
            Assert.AreEqual("song1", songs[0].Name);
        }

        [TestMethod]
        public void GetSongsByAlbum_TwoArtistsOneAlbum_GetSongsCombined()
        {
            FakeLoad(CreateSongs(2, 1, 1));
            var songs = control.Browser.GetSongsByAlbum("album1");
            Assert.AreEqual(2, songs.Count);
            Assert.AreEqual("artist2", songs[1].Artist);
        }

        [TestMethod]
        public void GetAlbumsByArtist_NoResult()
        {
            var albums = control.Browser.GetAlbumsByArtist("artist");
            Assert.AreEqual(0, albums.Count);
        }


        [TestMethod]
        public void GetAlbumsByArtist_OneResult()
        {
            FakeLoad(new List<Song> { new Song("artist1", "album1", "song1"), new Song("artist2", "album2", "song1") });
            var albums = control.Browser.GetAlbumsByArtist("artist1");
            Assert.AreEqual("album1", albums[0]);
            Assert.AreEqual(1, albums.Count);
        }

        [TestMethod]
        public void LoadAsync_GetAlbumsByArtist_OneResult()
        {
            var songs = new List<Song> { new Song("artist1", "album1", "song1"), new Song("artist2", "album2", "song1") };
            var engine = new FakeSongLoadEngine(songs);
            var loader = FakeAsyncLoad(engine, songs);
            control.LoadHandler.LoadSongs(loader).Wait();

            var albums = control.Browser.GetAlbumsByArtist("artist1");

            Assert.AreEqual(1, albums.Count);
            Assert.AreEqual("album1", albums[0]);
        }

        [TestMethod]
        public void GetAlbumsByArtist_NoDuplicates()
        {
            FakeLoad(CreateSongs(2, 2, 5));
            var albums = control.Browser.GetAlbumsByArtist("artist2");
            Assert.AreEqual(2, albums.Count);
        }

        [TestMethod]
        public void GetSongByTitle_NoResult()
        {

            var songs = control.Browser.GetSongsByTitle("none");
            Assert.AreEqual(0, songs.Count);
        }

        [TestMethod]
        public void GetSongByTitle_OneResult()
        {
            FakeLoad(CreateSongs(1, 1, 1));
            var songs = control.Browser.GetSongsByTitle("song1");
            Assert.AreEqual("song1", songs[0].Name);
        }

        [TestMethod]
        public void GetSongByTitle_TwoResults()
        {
            FakeLoad(CreateSongs(2, 1, 1));
            var songs = control.Browser.GetSongsByTitle("song1");
            Assert.AreEqual("song1", songs[1].Name);
        }

        [TestMethod]
        public void GetSongByArtistAndTitle_NoResult()
        {
            var songs = control.Browser.GetSongsByArtistAndTitle("artist1", "song1");
            Assert.AreEqual(0, songs.Count);
        }

        [TestMethod]
        public void GetSongsByArtistAndTitle_OneResult()
        {
            FakeLoad(CreateSongs(2, 1, 1));
            var songs = control.Browser.GetSongsByArtistAndTitle("artist1", "song1");
            Assert.AreEqual(1, songs.Count);
        }

        [TestMethod]
        public void ChangeSongInfo()
        {
            FakeLoad(CreateSongs(1, 1, 1));
            var edit = new SongUpdate(control.Browser.Songs[0]);
            edit.NewArtist = "new_artist";
            edit.NewAlbum = "new_album";
            edit.NewName = "new_name";
            edit.NewTrackNo = "12";

            control.LoadHandler.UpdateSong(edit);
            var songInLibrary = control.Browser.Songs[0];
            Assert.AreEqual("new_artist", songInLibrary.Artist);
            Assert.AreEqual("new_name", songInLibrary.Name);
            Assert.AreEqual("new_album", songInLibrary.Album);
            Assert.AreEqual("12", songInLibrary.TrackNo);
        }

        [TestMethod]
        public void SortSongs_ByAlbum()
        {
            var songs = new List<Song>
            {
                new Song("art1","album2","track1"),
                new Song("art1","album1","track1")
            };
            songs.Sort(Song.Comparison);
            Assert.AreEqual("album1", songs[0].Album);
        }

        [TestMethod]
        public void SortSongs_ByAlbumAndTrackNumber()
        {
            var songs = new List<Song>
            {
                new Song("art1","album2","track1","1",""),
                new Song("art1","album1","track2", "2",""),
                new Song("art1","album1","track1","1","")
            };
            songs.Sort(Song.Comparison);
            Assert.AreEqual("track1", songs[0].Name);
            Assert.AreEqual("track2", songs[1].Name);
        }

        [TestMethod]
        public void SortSongs_ByArtistAlbumAndTrackNumber()
        {
            var songs = new List<Song>
            {
                new Song("art1","album2","track1","1",""),
                new Song("art1","album1","track2", "2",""),
                new Song("art1","album1","track1","1",""),
                new Song("art3","album3","track1","1","")
            };
            songs.Sort(Song.Comparison);
            Assert.AreEqual("track1", songs[0].Name);
            Assert.AreEqual("album1", songs[0].Album);

            Assert.AreEqual("track2", songs[1].Name);
            Assert.AreEqual("album1", songs[1].Album);

            Assert.AreEqual("track1", songs[2].Name);
            Assert.AreEqual("album2", songs[2].Album);

            Assert.AreEqual("track1", songs[3].Name);
            Assert.AreEqual("album3", songs[3].Album);

        }

        [TestMethod]
        public void DeleteSong()
        {
            FakeLoad(CreateSongs(1, 1, 1));
            control.LoadHandler.DeleteSong(control.Browser.Songs[0]);
            Assert.AreEqual(0, control.Browser.Songs.Count);
        }

        [TestMethod]
        public void DeletedSong_DeletedArtist()
        {
            FakeLoad(CreateSongs(1, 1, 1));
            control.LoadHandler.DeleteSong(control.Browser.Songs[0]);
            Assert.AreEqual(0, control.Browser.Artists.Count);
        }

        [TestMethod]
        public void DeletedSong_DeletedAlbum()
        {
            FakeLoad(CreateSongs(1, 1, 1));
            control.LoadHandler.DeleteSong(control.Browser.Songs[0]);
            Assert.AreEqual(0, control.Browser.Albums.Count);
        }

        [TestMethod]
        public void DeleteSong_KeepArtist()
        {
            FakeLoad(CreateSongs(1, 1, 2));
            control.LoadHandler.DeleteSong(control.Browser.Songs[0]);
            Assert.AreEqual(1, control.Browser.Artists.Count);
        }

        [TestMethod]
        public void DeleteSong_KeepAlbum()
        {
            FakeLoad(CreateSongs(1, 1, 2));
            control.LoadHandler.DeleteSong(control.Browser.Songs[0]);
            Assert.AreEqual(1, control.Browser.Albums.Count);
        }

        [TestMethod]
        public void DeleteAlbum()
        {
            FakeLoad(CreateSongs(1, 2, 4));
            control.LoadHandler.DeleteAlbum("album2");
            Assert.AreEqual(4, control.Browser.Songs.Count);
            Assert.AreEqual(1, control.Browser.Albums.Count);
        }

        [TestMethod]
        public void EmptyLibrary_AddSong_AlbumAndArtistVisible()
        {
            var library = new Library();
            library.AddSong(new Song("artist","album","song"));
            Assert.AreEqual(1, library.Artists.Count);
            Assert.AreEqual(1, library.Albums.Count);
        }

        [TestMethod]
        public void ExistingIncompleteSongGetsMergedWhenSimilarIsAdded()
        {
            var library = new Library();
            library.AddSong(new Song("<unknown>", "<unknown>", "song.mp3","0", "ID"));
            library.AddSong(new Song("artist", "album", "song","0", "ID"));
            Assert.AreEqual(1, library.Songs.Count);
            Assert.AreEqual("song", library.Songs[0].Name);
        }

        [TestMethod]
        public void UpdateSongData()
        {
            var library = new Library();
            var sourceSong = new Song("<unknown>", "<unknown>", "song.mp3", "0", "ID");
            library.AddSong(sourceSong);
            library.UpdateSong(new SongUpdate(sourceSong) {NewAlbum = "album", NewArtist = "artist", NewName = "song"});
            Assert.AreEqual(1, library.Songs.Count);
            Assert.AreEqual("song", library.Songs[0].Name);
            Assert.AreEqual("artist", library.Songs[0].Artist);
            Assert.AreEqual("album", library.Songs[0].Album);
        }

        private void WaitedLoad(IAsyncSongLoader loader)
        {
            control.LoadHandler.LoadSongs(loader, listener).Wait();
        }

        private void WaitedLoadInternalListener(IAsyncSongLoader loader)
        {
            control.LoadHandler.LoadSongs(loader).Wait();
        }

        private FakeAsyncSongLoader FakeAsyncLoad(List<Song> list)
        {
            return new FakeAsyncSongLoader(new FakeSongLoadEngine(list), new FakeSongCollector(list));
        }

        private AsyncSongLoader FakeAsyncLoad(FakeSongLoadEngine engine, List<Song> songs)
        {
            return new AsyncSongLoader(engine,new FakeSongCollector(songs));
        }

        private void FakeLoad(List<Song> songs)
        {
            control.LoadHandler.LoadSongsSync(new FakeSongCatalogue(songs));
        }

        private static List<Song> CreateSongs(int artistMax, int albumMax, int songmax)
        {
            var songList = new List<Song>();
            for (int artist = 1; artist <= artistMax; artist++)
            {
                for (int album = 1; album <= albumMax; album++)
                {
                    for (int song = 1; song <= songmax; song++)
                    {
                        songList.Add(new Song("artist" + artist, "album" + album, "song" + song, (artist + song - 1) + "", artist + "/" + album + "/" + song));
                    }
                }
            }

            return songList;
        }
    }
}