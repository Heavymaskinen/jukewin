using System;
using System.Threading.Tasks;
using DataModel;
using Juke.Core;
using MessageRouting;
using NetCoreAudio;

namespace CoreAudioComponent
{
    public class CorePlayerEngine : PlayerEngine
    {
        private NetCoreAudio.Player player;
        private Task playingTask;
        public CorePlayerEngine()
        {
            player = new NetCoreAudio.Player();
            player.PlaybackFinished += (sender, args) => SignalFinished();
        }

        public override void Play(Song song)
        {
            player.Play(song.FilePath).Wait();
        }

        public override void Stop()
        {
            player.Stop().Wait();
        }

        public override void Dispose()
        {
            player.Stop().Wait();
            player = null;
        }
    }
}
