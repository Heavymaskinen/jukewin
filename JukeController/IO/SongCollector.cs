using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataModel;
using Juke.Control;
using Juke.Core;

namespace Juke.IO
{
    public class SongCollector : ISongCollector
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

        public async Task Load(List<string> files, LoadListener listener)
        {
            this.listener = listener;
            await Task.Run(() =>
            {
                lock (songs)
                {
                    songs.Clear();
                }

                splits = 0;
                listener.NotifyLoadInitiated(files.Count);
                SplitAndLoad(files);

                lock (songs)
                {
                    listener.NotifyCompleted(songs);
                }
            });
            }

        private void SplitAndLoad(List<string> files)
        {
            var mid = files.Count / 2;
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
                listener.NotifyProgress(1);
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
                Console.WriteLine("Load failed for "+file+": "+e.Message+"\n"+e.StackTrace);
            }
        }
    }
}
