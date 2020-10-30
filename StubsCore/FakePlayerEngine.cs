using Juke.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;

namespace Juke.Control.Tests
{
    public class FakePlayerEngine : PlayerEngine
    {
        public Song PlayedSong { get; set; }
        public bool Disposed { get; set; }

        public override void Play(Song song)
        {
            PlayedSong = song;
        }

        public void Finish()
        {
            SignalFinished();
        }

        public override void Stop()
        {
            SignalFinished();
        }

        public override void Dispose()
        {
            Disposed = true;
        }
    }
}
