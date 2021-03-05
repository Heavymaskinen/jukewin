using System;
using System.Collections.Generic;
using System.Threading;
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
        private CancellationToken cancelToken;

        public SongCollector(LoadListener listener, TagReaderFactory tagReaderFactory)
        {
            this.listener = listener;
            this.tagReaderFactory = tagReaderFactory;
            songs = new List<Song>();
        }

        public async Task Load(List<string> files, LoadListener listener, CancellationToken cancelToken)
        {
            this.listener = listener;
            this.cancelToken = cancelToken;
            lock (songs)
            {
                songs.Clear();
            }

            try
            {
                await Task.Run(() =>
                {
                    splits = 0;
                    listener.NotifyLoadInitiated(files.Count);
                    SplitAndLoad(files);
                }, cancelToken);
            }
            catch (Exception e)
            {
                Console.WriteLine("Canceled!");
            }
            
            lock (songs)
            {
                listener.NotifyCompleted(songs);
            }
        }

        private void SplitAndLoad(List<string> files)
        {
            var mid = files.Count / 2;
            var first = files.GetRange(0, mid);
            files.RemoveRange(0, mid);

            Task.WaitAll(
                Task.Run( async () =>  await LoadPartial(first), cancelToken),
                Task.Run( async () =>  await LoadPartial(files), cancelToken)
             );
        }

        private Task LoadPartial(List<string> list)
        {
            return Task.Run(() =>
            {
                if (list.Count > 2000)
                {
                    splits++;
                    Console.WriteLine("Do split! " + splits + " Count " + list.Count);
                    SplitAndLoad(list);
                    splits--;
                    Console.WriteLine("Post split. " + splits);
                    return;
                }

                foreach (var file in list)
                {
                    cancelToken.ThrowIfCancellationRequested();
                    LoadSong(file);
                    listener.NotifyProgress(1);
                }

                Console.WriteLine("Done chunk! " + splits);
            }, cancelToken);
        }

        private void LoadSong(string file)
        {
            try
            {
                var reader = tagReaderFactory.Create(file);
                var song = CreateSongFromTags(file, reader);
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

        private static Song CreateSongFromTags(string file, TagReader reader)
        {
            var song = new Song(
                reader.Artist,
                reader.Album,
                reader.Title,
                reader.TrackNo,
                file
            );

            return song;
        }
    }
}
