using System;
using DataModel;
using Juke.Core;
using NetCoreAudio;

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
