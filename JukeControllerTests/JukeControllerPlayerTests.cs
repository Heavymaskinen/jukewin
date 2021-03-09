using Microsoft.VisualStudio.TestTools.UnitTesting;
using Juke.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Juke.Core;

namespace Juke.Control.Tests
{
    [TestClass()]
    public class JukeControllerPlayerTests
    {
        private JukeController control;

        [TestInitialize()]
        public void Setup()
        {
            control = JukeController.Create();
            control.Player.RegisterPlayerEngine(new FakePlayerEngine());
        }

        [TestMethod()]
        public void InitialState_NotPlayingAnything()
        {
            Assert.AreEqual(null, control.Player.NowPlaying);
        }

        [TestMethod]
        public void PlaySong_IsNowPlaying()
        {
            control.Player.PlaySong(new Song("artist", "song", "song1"));
            Assert.AreEqual("song1", control.Player.NowPlaying.Name);
        }

        [TestMethod]
        public void SongPlayed_ListenerNotified()
        {
            var listener = new EventListener();
            var song = new Song("blabal", "hej", "test");
            control.Player.PlaySong(song);
            Assert.AreEqual(song, listener.SongPlayed);
        }

        [TestMethod]
        public void PlaySong_PlayedInEngine()
        {
            var engine = new FakePlayerEngine();
            control.Player.RegisterPlayerEngine(engine);
            var song = new Song("artist", "album", "song", "1", "path");
            control.Player.PlaySong(song);
            Assert.AreEqual(song, engine.PlayedSong);
        }

        [TestMethod]
        public void SongFinished_PlayNextFromQueue()
        {
            var engine = new FakePlayerEngine();
            control.Player.RegisterPlayerEngine(engine);
            PlayerEngineFactory.Engine = engine;
            var song = new Song("artist", "album", "song", "1", "path");
            var song2 = new Song("artist", "album", "song2", "2", "path2");
            control.Player.PlaySong(song);
            control.Player.PlaySong(song2);
            engine.Finish();
            Assert.AreEqual(song2, engine.PlayedSong);
            Assert.AreEqual(song2, control.Player.NowPlaying);
        }

        [TestMethod]
        public void SongFinished_EmptyQueue_EmptyLibrary_PlayNothing()
        {
            var engine = new FakePlayerEngine();
            control.Player.RegisterPlayerEngine(engine);
            var song = new Song("artist", "album", "song", "1", "path");
            control.Player.PlaySong(song);
            engine.Finish();
            Assert.AreEqual(null, control.Player.NowPlaying);
        }

        [TestMethod]
        public void PlaySongWhilePlaying_KeepPlayingCurrent()
        {
            control.Player.PlaySong(new Song("artist", "song", "song1"));
            control.Player.PlaySong(new Song("artist", "song", "song2"));
            Assert.AreEqual("song1", control.Player.NowPlaying.Name);
        }

        [TestMethod]
        public void SkipSong_PlayNext()
        {
            control.Player.PlaySong(new Song("artist", "song", "song1"));
            control.Player.PlaySong(new Song("artist", "song", "song2"));
            control.Player.Skip();
            Assert.AreEqual("song2", control.Player.NowPlaying?.Name);
        }

        [TestMethod]
        public void StopSong_ReactsAsFinished()
        {
            control.Player.PlaySong(new Song("artist", "song", "song1"));
            control.Player.Stop();
            Assert.IsNull(control.Player.NowPlaying);
        }

        [TestMethod]
        public void InitialState_QueueEmpty()
        {
            control.Player.PlaySong(new Song("artist", "song", "song2"));
            Assert.AreEqual(0, control.Player.Queue.Count);
        }

        [TestMethod]
        public void InitialState_QueueNextIsNull()
        {
            Assert.IsNull(control.Player.Queue.Next);
        }

        [TestMethod]
        public void PlaySongWhilePlaying_EnqueueSecond()
        {
            control.Player.PlaySong(new Song("artist", "song", "song1"));
            control.Player.PlaySong(new Song("artist", "song", "song2"));
            Assert.AreEqual("song2", control.Player.Queue.Next.Name);
        }

        [TestMethod]
        public void PlaySongsWhilePlaying_QueueIncreases()
        {
            control.Player.PlaySong(new Song("artist", "song", "song1"));
            control.Player.PlaySong(new Song("artist", "song", "song3"));
            control.Player.PlaySong(new Song("artist", "song", "song4"));
            Assert.AreEqual(2, control.Player.Queue.Count);
        }

        [TestMethod]
        public void CantEnqueueDuplicates()
        {
            control.Player.PlaySong(new Song("artist", "song", "song1"));
            control.Player.PlaySong(new Song("artist", "song", "song2"));
            control.Player.PlaySong(new Song("artist", "song", "song2"));
            Assert.AreEqual(1, control.Player.Queue.Count);
        }

        [TestMethod]
        public void CantEnqueueCurrentlyPlaying()
        {
            control.Player.PlaySong(new Song("artist", "song", "song1"));
            control.Player.PlaySong(new Song("artist", "song", "song2"));
            control.Player.PlaySong(new Song("artist", "song", "song1"));
            Assert.AreEqual(1, control.Player.Queue.Count);
        }

        [TestMethod]
        public void EnqueuedSongsInList()
        {
            control.Player.PlaySong(new Song("artist", "song", "song1"));
            control.Player.PlaySong(new Song("artist", "song", "song2"));
            Assert.AreEqual(control.Player.Queue.Songs[0].Name, "song2");
        }

        [TestMethod]
        public void PlayAlbum_EnqueuesEntireAlbum_PlaysFirstSong()
        {
            var songs = createSongs(1, 2, 10);
            FakeLoad(songs);
            control.Player.PlayAlbum("album1");
            Assert.AreEqual("song1", control.Player.NowPlaying.Name);
            Assert.AreEqual("album1", control.Player.NowPlaying.Album);
            Assert.AreEqual("song2", control.Player.Queue.Songs[0].Name);
            Assert.AreEqual("album1", control.Player.Queue.Songs[0].Album);
            Assert.AreEqual(9, control.Player.Queue.Songs.Count);
        }

        private void FakeLoad(List<Song> songs)
        {
            control.LoadHandler.LoadSongs(new FakeSongCatalogue(songs));
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