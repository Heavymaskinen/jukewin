using DataModel;
using Juke.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using WMPLib;

namespace Juke.External.Wmp.IO
{
    public class AsyncFileFinder : AsyncSongLoader
    {
  //      private WindowsMediaPlayer player;
        private IList<Song> songs;

        public AsyncFileFinder(string path)
        {
            Path = path;
//player = new WindowsMediaPlayer();
            songs = new List<Song>();
        }

        protected override void InvokeLoad()
        {
            AddFiles(Path);
        }

        private void AddFiles(string path)
        {
            var foundFiles = Directory.EnumerateFiles(path, "*.mp3");
            var directories = Directory.EnumerateDirectories(path);

            var tasks = new List<Task>();
            var multi = directories.Count() <= 20;
            foreach (var dir in directories)
            {
                if (multi)
                {
                    Task.Factory.StartNew(() => new AsyncFileFinder(dir).InvokeLoad());
                }
                else
                {
                    AddFiles(dir);
                }
            }

            foreach (var file in foundFiles)
            {
                try
                {
                    AddSong(file);
                    NotifyProgress("Added: " + file);
                }
                catch (Exception e)
                {
                    break;
                }
            }

            NotifyProgress("Completed: " + path+" with "+songs.Count+" songs");
            NotifyCompleted(songs);
        }

        private void AddSong(string file)
        {
            /*
            var media = player.newMedia(file);
            var song = new Song(
                media.getItemInfo("Author"),
                media.getItemInfo("Album"),
                media.getItemInfo("Title"),
                media.getItemInfo("OriginalIndex"),
                file
            );

            lock (songs)
            {
                songs.Add(song);
            }*/
        }
    }
}
