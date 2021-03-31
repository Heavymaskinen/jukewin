using System;
using DataModel;
using Juke.Core;

namespace CoreAudioComponent
{
    public class CorePlayerEngine : PlayerEngine
    {
        private NetCoreAudio.Player player;
        public CorePlayerEngine()
        {
            player = new NetCoreAudio.Player();
            player.PlaybackFinished += (sender, args) => SignalFinished();
        }

        public override void Dispose()
        {
            player = null;
        }

        public override void Play(Song song)
        {
            player.Play(song.FilePath);
        }

        public override void Stop()
        {
            player.Stop();
        }
    }
}
