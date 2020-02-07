using Juke.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using WMPLib;

namespace Juke.External.Wmp
{
    public class WmpPlayerEngine : PlayerEngine
    {
        private WindowsMediaPlayer player;
        private bool isPlaying;

        public WmpPlayerEngine()
        {
            player = new WindowsMediaPlayer();
            player.PlayStateChange += Player_PlayStateChange;
            player.settings.autoStart = false;
        }

        public override void Play(Song song)
        {
            if (song != null)
            {
                player.URL = song.FilePath;
                player.controls.play();
            }
        }

        private void Player_PlayStateChange(int NewState)
        {
            if ((WMPPlayState) NewState == WMPPlayState.wmppsPlaying)
            {
                isPlaying = true;
            } else if ((WMPPlayState)NewState == WMPPlayState.wmppsStopped)
            {
                if (!isPlaying) //Catching a bug where Media Player stops without playing
                {
                    player.controls.play();
                }
                else
                {
                    SignalFinished();
                    isPlaying = false;
                }
            }
        }

        public override void Stop()
        {
            player.controls.stop();
        }
    }
}
