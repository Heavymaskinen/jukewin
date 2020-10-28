using DataModel;
using Juke.Control;
using Juke.Core;
using Juke.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Juke.External.Wmp
{
    public class SongCollector
    {
        private int splits;
        private LoadListener listener;
        private readonly TagReaderFactory tagReaderFactory;
        private List<Song> songs;

        public SongCollector(LoadListener listener, TagReaderFactory tagReaderFactory)
        {
            this.listener = listener;
            this.tagReaderFactory = tagReaderFactory;
            songs = new List<Song>();
        }

        public void Load(List<string> files)
        {
            songs.Clear();
            splits = 0;
            listener.NotifyLoadInitiated2(files.Count);

            SplitAndLoad(files);

            listener.NotifyCompleted2(songs);
        }

        private void SplitAndLoad(List<string> files)
        {
            int mid = files.Count / 2;
            var first = files.GetRange(0, mid);
            files.RemoveRange(0, mid);

            Task.WaitAll(
                Task.Run(async () => await LoadPartial(first)),
                Task.Run(async () => await LoadPartial(files))
             );
        }

        private async Task LoadPartial(List<string> list)
        {
            if (list.Count > 2000)
            {
                splits++;
                Console.WriteLine("Do split! " + splits+" Count "+list.Count);
                SplitAndLoad(list);
                splits--;
                Console.WriteLine("Post split. " + splits);
                return;
            }

            foreach (var file in list)
            {
                await Task.Run(() => LoadSong(file));
                listener.NotifyProgress2(1);
            }
            Console.WriteLine("Done chunk! " + splits);
        }

        private void LoadSong(string file)
        {
            try
            {
                var reader = tagReaderFactory.Create(file);
                var song = new Song(
                        reader.Artist,
                        reader.Album,
                        reader.Title,
                        reader.TrackNo,
                        file
                    );
                Messenger.PostMessage(reader.Title + " (" + reader.Artist + ")", Messenger.TargetType.Frontend);
                lock (songs)
                {
                    songs.Add(song);
                }
            } catch (Exception e)
            {
                Console.WriteLine("Load failed for "+file+": "+e.Message);
            }
        }
    }
}
