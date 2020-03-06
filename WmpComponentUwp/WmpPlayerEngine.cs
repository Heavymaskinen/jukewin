using Juke.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Windows.UI.Xaml.Controls;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Media.Playback;
//using WMPLib;

namespace Juke.External.Wmp
{
    public class WmpPlayerEngine : PlayerEngine
    {
        private MediaPlayer player;
        //private WindowsMediaPlayer player;
        private bool isPlaying;

        public WmpPlayerEngine()
        {
            player = new MediaPlayer();
            player.CurrentStateChanged += Player_CurrentStateChanged;
            player.MediaEnded += Player_MediaEnded;
            ////player = new WindowsMediaPlayer();
           // player.PlayStateChange += Player_PlayStateChange;
           // player.settings.autoStart = false;
        }

        private void Player_MediaEnded(MediaPlayer sender, object args)
        {
            SignalFinished();
        }

        private void Player_CurrentStateChanged(MediaPlayer sender, object args)
        {
            if (sender.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
            {
                isPlaying = true;
            }
        }

        public void Dispose()
        {
            player.Dispose();
        }

        public override void Play(Song song)
        {
            if (song != null)
            {
                PlayFileAsync(song);
            }
        }

        private async void PlayFileAsync(Song song)
        {
            var storage = await StorageFile.GetFileFromPathAsync(song.FilePath);
            player.Source = MediaSource.CreateFromStorageFile(storage);
            player.Play();
        }

        private void Player_PlayStateChange(int NewState)
        {
            /*
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
            }*/
        }

        public override void Stop()
        {
           // player.controls.stop();
        }
    }
}
