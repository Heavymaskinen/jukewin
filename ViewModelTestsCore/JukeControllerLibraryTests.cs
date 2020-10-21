﻿
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
            control = (JukeController)JukeController.Create();
            listener = new EventListener();
        }

        [Test]
        public void SongAsStringIsName()
        {
            var song = new Song("artist", "anAlbum", "MySong");
            Assert.AreEqual(song.Name, song.ToString());
        }

        [Test]
        public void InitialState_NoSongs()
        {
            var songs = control.Browser.Songs;
            Assert.AreEqual(0, songs.Count);
        }

        [Test]
        public void LoadSongs_ContainsOne()
        {
            FakeLoad(createSongs(1, 1, 1));
            var songs = control.Browser.Songs;
            Assert.AreEqual(1, songs.Count);
        }

        [Test]
        public void LoadOneSong_GetByArtist()
        {
            FakeLoad(createSongs(1, 1, 1));
            var songs = control.Browser.GetSongsByArtist("artist1");
            Assert.AreEqual(1, songs.Count);
            Assert.AreEqual("song1", songs[0].Name);
        }

        [Test]
        public void LoadAsync_NotifiedOfStart()
        {
            control.LoadHandler.LoadSongs(FakeAsyncLoad(new List<Song> { }));
            Assert.IsTrue(listener.LoadInitiated);
        }

        [Test]
        public void InitialState_NoLoadNotification()
        {
            var loader = FakeAsyncLoad(new List<Song> { });
            Assert.IsFalse(listener.LoadInitiated);
        }

        [Test]
        public void LoadAsync_NotifiedOfEnd()
        {
            var loader = FakeAsyncLoad(new List<Song> { });
            control.LoadHandler.LoadSongs(loader);
            loader.SignalComplete();
            Assert.IsTrue(listener.LoadCompleted);
        }

        [Test]
        public void LoadAsync_NotCompleteNotNotified()
        {
            control.LoadHandler.LoadSongs(FakeAsyncLoad(new List<Song> { }));
            Assert.IsFalse(listener.LoadCompleted);
        }

        [Test]
        public void LoadAsync_GetNotifiedOfProgress()
        {
            var loader = FakeAsyncLoad(new List<Song> { });
            control.LoadHandler.LoadSongs(loader);
            loader.SignalProgress("message");
            Assert.AreEqual("message", listener.ProgressNoted);
        }

        [Test]
        public void LoadAsyncCompleted_SongsAddedToLibrary()
        {
            var loader = FakeAsyncLoad(createSongs(1, 1, 1));
            control.LoadHandler.LoadSongs(loader);
            loader.SignalComplete();
            Assert.AreEqual(1, control.Browser.Songs.Count);
        }

        [Test]
        public void LoadSecondTime_SongsAdded()
        {
            FakeLoad(createSongs(1, 1, 1));
            FakeLoad(new List<Song> { new Song("artist", "album", "funky song") });
            Assert.AreEqual(2, control.Browser.Songs.Count);
        }

        [Test]
        public void LoadSongs_NoDuplicates()
        {
            var songs = createSongs(1, 1, 1);
            FakeLoad(songs);
            FakeLoad(songs);
            Assert.AreEqual(1, control.Browser.Songs.Count);
        }

        [Test]
        public void LoadSongs_SameTitleDifferentArtist()
        {
            FakeLoad(new List<Song> { new Song("artist", "album", "funky song"), new Song("artist1", "album", "funky song") });
            Assert.AreEqual(2, control.Browser.Songs.Count);
        }

        [Test]
        public void SaveLibrary()
        {
            var songs = createSongs(2, 2, 2);
            FakeLoad(songs);
            var writer = new FakeSongIO();
            control.SaveLibrary(writer);
            Assert.AreEqual(songs.Count, writer.WrittenSongs.Count);
        }

        [Test]
        public void LoadLibrary()
        {
            var songs = createSongs(2, 2, 2);
            var loader = new FakeSongIO(songs);
            control.LoadLibrary(loader);
            Assert.AreEqual(songs.Count, control.Browser.Songs.Count);
        }

        [Test]
        public void GetByArtist_OneResult()
        {
            FakeLoad(createSongs(2, 1, 1));
            var songs = control.Browser.GetSongsByArtist("artist2");
            Assert.AreEqual(1, songs.Count);
            Assert.AreEqual("artist2", songs[0].Artist);
            Assert.AreEqual("song1", songs[0].Name);
        }

        [Test]
        public void GetByArtists_NoResult()
        {
            var songs = control.Browser.GetSongsByArtist("artist2");
            Assert.AreEqual(0, songs.Count);
        }

        [Test]
        public void GetByArtists_TwoResults()
        {
            FakeLoad(createSongs(1, 1, 2));
            var songs = control.Browser.GetSongsByArtist("artist1");
            Assert.AreEqual(2, createSongs(1, 1, 2).Count);
        }

        [Test]
        public void GetAlbum_SongsAreSortedByTrack()
        {
            var songs = new List<Song>();
            songs.Add(new Song("artist", "album", "song2", "2", ""));
            songs.Add(new Song("artist", "album", "song1", "1", ""));
            FakeLoad(songs);
            var albumSongs = control.Browser.GetSongsByAlbum("album");
            Assert.AreEqual("song1", albumSongs[0].Name);
        }

        [Test]
        public void GetAlbum_SongsWithEqualTracksSortedByName()
        {
            var songs = new List<Song>();
            songs.Add(new Song("artist", "album", "song3", "1", ""));
            songs.Add(new Song("artist", "album", "song1", "1", ""));
            FakeLoad(songs);
            var albumSongs = control.Browser.GetSongsByAlbum("album");
            Assert.AreEqual("song1", albumSongs[0].Name);
        }

        [Test]
        public void GetArtists_NoResult()
        {
            var result = control.Browser.Artists;
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void GetArtists_OneResult()
        {
            FakeLoad(createSongs(1, 1, 1));
            var result = control.Browser.Artists;
            Assert.AreEqual("artist1", result[0]);
        }

        [Test]
        public void GetArtists_TwoResults()
        {
            FakeLoad(createSongs(2, 1, 1));
            var result = control.Browser.Artists;
            Assert.AreEqual("artist2", result[1]);
        }

        [Test]
        public void GetArtists_NoDuplicates()
        {
            FakeLoad(createSongs(1, 1, 3));
            var result = control.Browser.Artists;
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void GetAlbums_NoResult()
        {
            var albums = control.Browser.Albums;
            Assert.AreEqual(0, albums.Count);
        }

        [Test]
        public void GetAlbums_OnResult()
        {
            FakeLoad(createSongs(1, 1, 1));
            var albums = control.Browser.Albums;
            Assert.AreEqual("album1", albums[0]);
        }

        [Test]
        public void GetAlbums_TwoResults()
        {
            FakeLoad(createSongs(1, 2, 1));
            var albums = control.Browser.Albums;
            Assert.AreEqual("album2", albums[1]);
        }

        [Test]
        public void GetAlbums_NoDupliates()
        {
            FakeLoad(createSongs(1, 1, 3));
            var albums = control.Browser.Albums;
            Assert.AreEqual(1, albums.Count);
        }

        [Test]
        public void GetSongsByAlbum_NoResut()
        {
            var songs = control.Browser.GetSongsByAlbum("album1");
            Assert.AreEqual(0, songs.Count);
        }

        [Test]
        public void GetSongsByAlbum_OneResult()
        {
            FakeLoad(createSongs(1, 3, 1));
            var songs = control.Browser.GetSongsByAlbum("album1");
            Assert.AreEqual("song1", songs[0].Name);
        }

        [Test]
        public void GetAlbumsByArtist_NoResult()
        {
            var albums = control.Browser.GetAlbumsByArtist("artist");
            Assert.AreEqual(0, albums.Count);
        }


        [Test]
        public void GetAlbumsByArtist_OneResult()
        {
            FakeLoad(new List<Song> { new Song("artist1", "album1", "song1"), new Song("artist2", "album2", "song1") });
            var albums = control.Browser.GetAlbumsByArtist("artist1");
            Assert.AreEqual("album1", albums[0]);
            Assert.AreEqual(1, albums.Count);
        }

        [Test]
        public void LoadAsync_GetAlbumsByArtist_OneResult()
        {
            var loader = FakeAsyncLoad(new List<Song> { new Song("artist1", "album1", "song1"), new Song("artist2", "album2", "song1") });
            control.LoadHandler.LoadSongs(loader);
            loader.SignalComplete();
            var albums = control.Browser.GetAlbumsByArtist("artist1");
            Assert.AreEqual("album1", albums[0]);
            Assert.AreEqual(1, albums.Count);
        }

        [Test]
        public void GetAlbumsByArtist_NoDuplicates()
        {
            FakeLoad(createSongs(2, 2, 5));
            var albums = control.Browser.GetAlbumsByArtist("artist2");
            Assert.AreEqual(2, albums.Count);
        }

        [Test]
        public void GetSongByTitle_NoResult()
        {

            var songs = control.Browser.GetSongsByTitle("none");
            Assert.AreEqual(0, songs.Count);
        }

        [Test]
        public void GetSongByTitle_OneResult()
        {
            FakeLoad(createSongs(1, 1, 1));
            var songs = control.Browser.GetSongsByTitle("song1");
            Assert.AreEqual("song1", songs[0].Name);
        }

        [Test]
        public void GetSongByTitle_TwoResults()
        {
            FakeLoad(createSongs(2, 1, 1));
            var songs = control.Browser.GetSongsByTitle("song1");
            Assert.AreEqual("song1", songs[1].Name);
        }

        [Test]
        public void GetSongByArtistAndTitle_NoResult()
        {
            var songs = control.Browser.GetSongsByArtistAndTitle("artist1", "song1");
            Assert.AreEqual(0, songs.Count);
        }

        [Test]
        public void GetSongsByArtistAndTitle_OneResult()
        {
            FakeLoad(createSongs(2, 1, 1));
            var songs = control.Browser.GetSongsByArtistAndTitle("artist1", "song1");
            Assert.AreEqual(1, songs.Count);
        }

        [Test]
        public void ChangeSongInfo()
        {
            FakeLoad(createSongs(1, 1, 1));
            var edit = new SongUpdate(control.Browser.Songs[0]);
            edit.NewArtist = "new_artist";
            edit.NewAlbum = "new_album";
            edit.NewName = "new_name";
            edit.NewTrackNo = "12";

            control.UpdateSong(edit);
            var songInLibrary = control.Browser.Songs[0];
            Assert.AreEqual("new_artist", songInLibrary.Artist);
            Assert.AreEqual("new_name", songInLibrary.Name);
            Assert.AreEqual("new_album", songInLibrary.Album);
            Assert.AreEqual("12", songInLibrary.TrackNo);
        }

        [Test]
        public void DeleteSong()
        {
            FakeLoad(createSongs(1, 1, 1));
            control.DeleteSong(control.Browser.Songs[0]);
            Assert.AreEqual(0, control.Browser.Songs.Count);
        }

        [Test]
        public void DeletedSong_DeletedArtist()
        {
            FakeLoad(createSongs(1, 1, 1));
            control.DeleteSong(control.Browser.Songs[0]);
            Assert.AreEqual(0, control.Browser.Artists.Count);
        }

        [Test]
        public void DeletedSong_DeletedAlbum()
        {
            FakeLoad(createSongs(1, 1, 1));
            control.DeleteSong(control.Browser.Songs[0]);
            Assert.AreEqual(0, control.Browser.Albums.Count);
        }

        [Test]
        public void DeleteSong_KeepArtist()
        {
            FakeLoad(createSongs(1, 1, 2));
            control.DeleteSong(control.Browser.Songs[0]);
            Assert.AreEqual(1, control.Browser.Artists.Count);
        }

        [Test]
        public void DeleteSong_KeepAlbum()
        {
            FakeLoad(createSongs(1, 1, 2));
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

        private List<Song> createSongs(int artistMax, int albumMax, int songmax)
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