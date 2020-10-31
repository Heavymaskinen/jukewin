using System;
using System.Collections.Generic;
using DataModel;
using Juke.Control;
using Juke.Control.Tests;
using NUnit.Framework;

namespace JukeControllerTests
{
    [TestFixture]
    public class JukeControllerLibraryTests
    {
        private JukeController control;
        private EventListener listener;

        [SetUp]
        public void Setup()
        {
            control  = (JukeController) JukeController.Create();
            listener = new EventListener();
        }

        [TestCase]
        public void InitialState_NoSongs()
        {
            var songs = control.Browser.Songs;
            Assert.AreEqual(0, songs.Count);
        }

        [TestCase]
        public void LoadSongs_ContainsOne()
        {
            FakeLoad(CreateSongs(1, 1, 1));
            var songs = control.Browser.Songs;
            Assert.AreEqual(1, songs.Count);
        }

        [TestCase]
        public void LoadOneSong_GetByArtist()
        {
            FakeLoad(CreateSongs(1, 1, 1));
            var songs = control.Browser.GetSongsByArtist("artist1");
            Assert.AreEqual(1, songs.Count);
            Assert.AreEqual("song1", songs[0].Name);
        }

        [TestCase]
        public void LoadAsync_NotifiedOfStart()
        {
            control.LoadHandler.LoadSongs(FakeAsyncLoad(new List<Song> { }));
            Assert.IsTrue(listener.LoadInitiated);
        }

        [TestCase]
        public void InitialState_NoLoadNotification()
        {
            var loader = FakeAsyncLoad(new List<Song> { });
            Assert.IsFalse(listener.LoadInitiated);
        }

        [TestCase]
        public void LoadAsync_NotifiedOfEnd()
        {
            var loader = FakeAsyncLoad(new List<Song> { });
            control.LoadHandler.LoadSongs(loader);
            loader.SignalComplete();
            Assert.IsTrue(listener.LoadCompleted);
        }

        [TestCase]
        public void LoadAsync_NotCompleteNotNotified()
        {
            control.LoadHandler.LoadSongs(FakeAsyncLoad(new List<Song> { }));
            Assert.IsFalse(listener.LoadCompleted);
        }

        [TestCase]
        public void LoadAsync_GetNotifiedOfProgress()
        {
            var loader = FakeAsyncLoad(new List<Song> { });
            control.LoadHandler.LoadSongs(loader);
            loader.SignalProgress("message");
            Assert.AreEqual("message", listener.ProgressNoted);
        }

        [TestCase]
        public void LoadAsyncCompleted_SongsAddedToLibrary()
        {
            var loader = FakeAsyncLoad(CreateSongs(1, 1, 1));
            control.LoadHandler.LoadSongs(loader);
            loader.SignalComplete();
            Assert.AreEqual(1, control.Browser.Songs.Count);
        }

        [TestCase]
        public void LoadSecondTime_SongsAdded()
        {
            FakeLoad(CreateSongs(1, 1, 1));
            FakeLoad(new List<Song> {new Song("artist", "album", "funky song")});
            Assert.AreEqual(2, control.Browser.Songs.Count);
        }

        [TestCase]
        public void LoadSongs_NoDuplicates()
        {
            var songs = CreateSongs(1, 1, 1);
            FakeLoad(songs);
            FakeLoad(songs);
            Assert.AreEqual(1, control.Browser.Songs.Count);
        }

        [TestCase]
        public void FirstSong_GetsIdOne()
        {
            var songs = CreateSongs(1, 1, 1);
            FakeLoad(songs);
            Assert.AreEqual(1, control.Browser.Songs[0].ID);
        }

        [TestCase]
        public void SecondSong_GetsIdTwo()
        {
            var songs = CreateSongs(1, 1, 2);
            FakeLoad(songs);
            Assert.AreEqual(2, control.Browser.Songs[1].ID);
        }

        [TestCase]
        public void IdSequenceContinuesFromLast()
        {
            var songs = CreateSongs(1, 1, 3);
            FakeLoad(songs.GetRange(0, 2));
            FakeLoad(songs.GetRange(2, 1));
            Assert.AreEqual(3, control.Browser.Songs[2].ID);
        }

        [TestCase]
        public void DuplicatesDoNotMoveSequence()
        {
            var songs = CreateSongs(1, 1, 3);
            FakeLoad(songs.GetRange(0, 2));
            FakeLoad(songs.GetRange(0, 2));
            FakeLoad(songs.GetRange(2, 1));
            Assert.AreEqual(3, control.Browser.Songs[2].ID);
        }

        [TestCase]
        public void LoadSongs_SameTitleDifferentArtist()
        {
            FakeLoad(new List<Song>
                {new Song("artist", "album", "funky song"), new Song("artist1", "album", "funky song")});
            Assert.AreEqual(2, control.Browser.Songs.Count);
        }

        [TestCase]
        public void SaveLibrary()
        {
            var songs = CreateSongs(2, 2, 2);
            FakeLoad(songs);
            var writer = new FakeSongIO();
            control.SaveLibrary(writer);
            Assert.AreEqual(songs.Count, writer.WrittenSongs.Count);
        }

        [TestCase]
        public void LoadLibrary_SongsAreLoaded()
        {
            var songs  = CreateSongs(2, 2, 2);
            var loader = new FakeSongIO(songs);
            control.LoadLibrary(loader);
            Assert.AreEqual(songs.Count, control.Browser.Songs.Count);
        }

        [TestCase]
        public void GetByArtist_OneResult()
        {
            FakeLoad(CreateSongs(2, 1, 1));
            var songs = control.Browser.GetSongsByArtist("artist2");
            Assert.AreEqual(1, songs.Count);
            Assert.AreEqual("artist2", songs[0].Artist);
            Assert.AreEqual("song1", songs[0].Name);
        }

        [TestCase]
        public void GetByArtists_NoResult()
        {
            var songs = control.Browser.GetSongsByArtist("artist2");
            Assert.AreEqual(0, songs.Count);
        }

        [TestCase]
        public void GetByArtists_TwoResults()
        {
            FakeLoad(CreateSongs(1, 1, 2));
            var songs = control.Browser.GetSongsByArtist("artist1");
            Assert.AreEqual(2, CreateSongs(1, 1, 2).Count);
        }

        [TestCase]
        public void GetArtists_NoResult()
        {
            var result = control.Browser.Artists;
            Assert.AreEqual(0, result.Count);
        }

        [TestCase]
        public void GetArtists_OneResult()
        {
            FakeLoad(CreateSongs(1, 1, 1));
            var result = control.Browser.Artists;
            Assert.AreEqual("artist1", result[0]);
        }

        [TestCase]
        public void GetArtists_TwoResults()
        {
            FakeLoad(CreateSongs(2, 1, 1));
            var result = control.Browser.Artists;
            Assert.AreEqual("artist2", result[1]);
        }

        [TestCase]
        public void GetArtists_NoDuplicates()
        {
            FakeLoad(CreateSongs(1, 1, 3));
            var result = control.Browser.Artists;
            Assert.AreEqual(1, result.Count);
        }

        [TestCase]
        public void GetAlbums_NoResult()
        {
            var albums = control.Browser.Albums;
            Assert.AreEqual(0, albums.Count);
        }

        [TestCase]
        public void GetAlbums_OnResult()
        {
            FakeLoad(CreateSongs(1, 1, 1));
            var albums = control.Browser.Albums;
            Assert.AreEqual("album1", albums[0]);
        }

        [TestCase]
        public void GetAlbums_TwoResults()
        {
            FakeLoad(CreateSongs(1, 2, 1));
            var albums = control.Browser.Albums;
            Assert.AreEqual("album2", albums[1]);
        }

        [TestCase]
        public void GetAlbums_NoDupliates()
        {
            FakeLoad(CreateSongs(1, 1, 3));
            var albums = control.Browser.Albums;
            Assert.AreEqual(1, albums.Count);
        }

        [TestCase]
        public void GetSongsByAlbum_NoResult()
        {
            var songs = control.Browser.GetSongsByAlbum("album1");
            Assert.AreEqual(0, songs.Count);
        }

        [TestCase]
        public void GetSongsByAlbum_OneResult()
        {
            FakeLoad(CreateSongs(1, 3, 1));
            var songs = control.Browser.GetSongsByAlbum("album1");
            Assert.AreEqual("song1", songs[0].Name);
        }

        [TestCase]
        public void GetSongByArtistAndTitle_OneResult()
        {
            FakeLoad(CreateSongs(1, 1, 4));
            var songs = control.Browser.GetSongsByArtistAndTitle("artist1", "song1");
            Assert.AreEqual(1, songs.Count);
            Assert.AreEqual("song1", songs[0].Name);
        }

        [TestCase]
        public void GetAlbumsByArtist_NoResult()
        {
            var albums = control.Browser.GetAlbumsByArtist("artist");
            Assert.AreEqual(0, albums.Count);
        }


        [TestCase]
        public void GetAlbumsByArtist_OneResult()
        {
            FakeLoad(new List<Song> {new Song("artist1", "album1", "song1"), new Song("artist2", "album2", "song1")});
            var albums = control.Browser.GetAlbumsByArtist("artist1");
            Assert.AreEqual("album1", albums[0]);
            Assert.AreEqual(1, albums.Count);
        }

        [TestCase]
        public void LoadAsync_GetAlbumsByArtist_OneResult()
        {
            var loader = FakeAsyncLoad(new List<Song>
                {new Song("artist1", "album1", "song1"), new Song("artist2", "album2", "song1")});
            control.LoadHandler.LoadSongs(loader);
            loader.SignalComplete();
            var albums = control.Browser.GetAlbumsByArtist("artist1");
            Assert.AreEqual("album1", albums[0]);
            Assert.AreEqual(1, albums.Count);
        }

        [TestCase]
        public void GetAlbumsByArtist_NoDuplicates()
        {
            FakeLoad(CreateSongs(2, 2, 5));
            var albums = control.Browser.GetAlbumsByArtist("artist2");
            Assert.AreEqual(2, albums.Count);
        }

        [TestCase]
        public void GetSongByTitle_NoResult()
        {
            var songs = control.Browser.GetSongsByTitle("none");
            Assert.AreEqual(0, songs.Count);
        }

        [TestCase]
        public void GetSongByTitle_OneResult()
        {
            FakeLoad(CreateSongs(1, 1, 1));
            var songs = control.Browser.GetSongsByTitle("song1");
            Assert.AreEqual("song1", songs[0].Name);
        }

        [TestCase]
        public void GetSongByTitle_TwoResults()
        {
            FakeLoad(CreateSongs(2, 1, 1));
            var songs = control.Browser.GetSongsByTitle("song1");
            Assert.AreEqual("song1", songs[1].Name);
        }

        [TestCase]
        public void GetSongByArtistAndTitle_NoResult()
        {
            var songs = control.Browser.GetSongsByArtistAndTitle("artist1", "song1");
            Assert.AreEqual(0, songs.Count);
        }

        [TestCase]
        public void GetSongsByArtistAndTitle_OneResult()
        {
            FakeLoad(CreateSongs(2, 1, 1));
            var songs = control.Browser.GetSongsByArtistAndTitle("artist1", "song1");
            Assert.AreEqual(1, songs.Count);
        }

        [TestCase]
        public void GetSongByIndex_ReturnsOne()
        {
            FakeLoad(CreateSongs(3,3,3));
            Song song = control.Browser.GetSongByID(3);
            Assert.AreEqual("song3", song.Name);
            Assert.AreEqual("artist1", song.Artist);
            Assert.AreEqual("album1", song.Album);
        }

        [TestCase]
        public void ChangeSongInfo()
        {
            FakeLoad(CreateSongs(1, 1, 4));
            var edit = new SongUpdate(control.Browser.Songs[0])
            {
                NewArtist = "new_artist", NewAlbum = "new_album", NewName = "new_name", NewTrackNo = "12"
            };

            control.UpdateSong(edit);
            var songs = control.Browser.GetSongsByTitle("new_name");
            var songInLibrary = songs[0];
            Assert.AreEqual(4, control.Browser.Songs.Count);
            Assert.AreEqual("new_name", songInLibrary.Name, "Wrong name");
            Assert.AreEqual("new_artist", songInLibrary.Artist, "Wrong artist");
            Assert.AreEqual("new_album", songInLibrary.Album, "Wrong album");
            Assert.AreEqual("12", songInLibrary.TrackNo, "Wrong Track Number");
            Assert.AreEqual(1, songInLibrary.ID, "Wrong ID");
            Assert.AreEqual(2, control.Browser.Artists.Count, "Missing new artist");
        }

        [TestCase]
        public void ChangeOnlyArtistName_OldArtistRemoved()
        {
            FakeLoad(CreateSongs(1, 1, 1));
            var edit = new SongUpdate(control.Browser.Songs[0])
            {
                NewArtist = "new_artist", NewAlbum = "new_album", NewName = "new_name", NewTrackNo = "12"
            };

            control.UpdateSong(edit);
            Assert.AreEqual(1, control.Browser.Artists.Count);
        }

        [TestCase]
        public void DeleteSong()
        {
            FakeLoad(CreateSongs(1, 1, 1));
            control.DeleteSong(control.Browser.Songs[0]);
            Assert.AreEqual(0, control.Browser.Songs.Count);
        }

        [TestCase]
        public void DeletedSong_DeletedArtist()
        {
            FakeLoad(CreateSongs(1, 1, 1));
            control.DeleteSong(control.Browser.Songs[0]);
            Assert.AreEqual(0, control.Browser.Artists.Count);
        }

        [TestCase]
        public void DeletedSong_DeletedAlbum()
        {
            FakeLoad(CreateSongs(1, 1, 1));
            control.DeleteSong(control.Browser.Songs[0]);
            Assert.AreEqual(0, control.Browser.Albums.Count);
        }

        [TestCase]
        public void DeleteSong_KeepArtist()
        {
            FakeLoad(CreateSongs(1, 1, 2));
            control.DeleteSong(control.Browser.Songs[0]);
            Assert.AreEqual(1, control.Browser.Artists.Count);
        }

        [TestCase]
        public void DeleteSong_KeepAlbum()
        {
            FakeLoad(CreateSongs(1, 1, 2));
            control.DeleteSong(control.Browser.Songs[0]);
            Assert.AreEqual(1, control.Browser.Albums.Count);
        }

        private FakeAsyncSongLoader FakeAsyncLoad(List<Song> list)
        {
            return new FakeAsyncSongLoader(list);
        }

        private void FakeLoad(List<Song> songs)
        {
            control.LoadHandler.LoadSongs(new FakeSongIO(songs));
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