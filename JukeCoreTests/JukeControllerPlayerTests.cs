using System.Collections.Generic;
using DataModel;
using Juke.Core;
using NUnit.Framework;

namespace Juke.Control.Tests
{
    [TestFixture]
    public class JukeControllerPlayerTests
    {
        private JukeController control;

        [SetUp]
        public void Setup()
        {
            control = JukeController.Create();
            control.Player.RegisterPlayerEngine(new FakePlayerEngine());
        }

        [TestCase]
        public void InitialState_NotPlayingAnything()
        {
            Assert.AreEqual(null, control.Player.NowPlaying);
        }

        [TestCase]
        public void PlaySong_IsNowPlaying()
        {
            control.Player.RegisterPlayerEngine(new FakePlayerEngine());
            control.Player.PlaySong(new Song("artist", "song", "song1"));
            Assert.AreEqual("song1", control.Player.NowPlaying.Name);
        }

        [TestCase]
        public void SongPlayed_ListenerNotified()
        {
            var listener = new EventListener();
            var song = new Song("blabal", "hej", "test");
            control.Player.PlaySong(song);
            Assert.AreEqual(song, listener.SongPlayed);
        }

        [TestCase]
        public void PlaySong_PlayedInEngine()
        {
            var engine = new FakePlayerEngine();
            PlayerEngineFactory.Engine = engine;
            control.Player.RegisterPlayerEngine(engine);
            var song = new Song("artist", "album", "song", "1", "path");
            control.Player.PlaySong(song);
            Assert.AreEqual(song, engine.PlayedSong);
        }

        [TestCase]
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

        [TestCase]
        public void SongFinished_EmptyQueue_PlayNothing()
        {
            var engine = new FakePlayerEngine();
            control.Player.RegisterPlayerEngine(engine);
            PlayerEngineFactory.Engine = engine;
            var song = new Song("artist", "album", "song", "1", "path");
            control.Player.PlaySong(song);
            engine.Finish();
            Assert.AreEqual(null, engine.PlayedSong);
            Assert.AreEqual(null, control.Player.NowPlaying);
        }

        [TestCase]
        public void PlaySongWhilePlaying_KeepPlayingCurrent()
        {
            control.Player.PlaySong(new Song("artist", "song", "song1"));
            control.Player.PlaySong(new Song("artist", "song", "song2"));
            Assert.AreEqual("song1", control.Player.NowPlaying.Name);
        }

        [TestCase]
        public void StopSong_ReactsAsFinished()
        {
            control.Player.PlaySong(new Song("artist", "song", "song1"));
            control.Player.Stop();
            Assert.IsNull(control.Player.NowPlaying);
        }

        [TestCase]
        public void InitialState_QueueEmpty()
        {
            control.Player.PlaySong(new Song("artist", "song", "song2"));
            Assert.AreEqual(0, control.Player.Queue.Count);
        }

        [TestCase]
        public void InitialState_QueueNextIsNull()
        {
            Assert.IsNull(control.Player.Queue.Next);
        }

        [TestCase]
        public void PlaySongWhilePlaying_EnqueueSecond()
        {
            control.Player.PlaySong(new Song("artist", "song", "song1"));
            control.Player.PlaySong(new Song("artist", "song", "song2"));
            Assert.AreEqual("song2", control.Player.Queue.Next.Name);
        }

        [TestCase]
        public void PlaySongsWhilePlaying_QueueIncreases()
        {
            control.Player.PlaySong(new Song("artist", "song", "song1"));
            control.Player.PlaySong(new Song("artist", "song", "song3"));
            control.Player.PlaySong(new Song("artist", "song", "song4"));
            Assert.AreEqual(2, control.Player.Queue.Count);
        }

        [TestCase]
        public void CantEnqueueDuplicates()
        {
            control.Player.PlaySong(new Song("artist", "song", "song1"));
            control.Player.PlaySong(new Song("artist", "song", "song2"));
            control.Player.PlaySong(new Song("artist", "song", "song2"));
            Assert.AreEqual(1, control.Player.Queue.Count);
        }

        [TestCase]
        public void CantEnqueueCurrentlyPlaying()
        {
            control.Player.PlaySong(new Song("artist", "song", "song1"));
            control.Player.PlaySong(new Song("artist", "song", "song2"));
            control.Player.PlaySong(new Song("artist", "song", "song1"));
            Assert.AreEqual(1, control.Player.Queue.Count);
        }

        [TestCase]
        public void EnqueuedSongsInList()
        {
            control.Player.PlaySong(new Song("artist", "song", "song1"));
            control.Player.PlaySong(new Song("artist", "song", "song2"));
            Assert.AreEqual(control.Player.Queue.Songs[0].Name, "song2");
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