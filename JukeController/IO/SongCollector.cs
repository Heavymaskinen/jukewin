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
                Messenger.Log("Canceled!");
            }

            lock (songs)
            {
                Messenger.Log("Collected songs: " + songs.Count);
                listener.NotifyCompleted(songs);
            }
        }

        private void SplitAndLoad(List<string> files)
        {
            var mid = files.Count / 2;
            var first = files.GetRange(0, mid);
            files.RemoveRange(0, mid);

            Task.WaitAll(
                Task.Run(async () => await LoadPartial(first), cancelToken),
                Task.Run(async () => await LoadPartial(files), cancelToken)
            );
        }

        private Task LoadPartial(List<string> list)
        {
            return Task.Run(() =>
            {
                if (list.Count > 2000)
                {
                    splits++;
                    Messenger.Log("Do split! " + splits + " Count " + list.Count);
                    SplitAndLoad(list);
                    splits--;
                    Messenger.Log("Post split. " + splits);
                    return;
                }

                foreach (var file in list)
                {
                    cancelToken.ThrowIfCancellationRequested();
                    LoadSong(file);
                    listener.NotifyProgress(1);
                }

                Messenger.Log("Done chunk! " + splits);
            }, cancelToken);
        }

        private void LoadSong(string file)
        {
            try
            {
                var reader = tagReaderFactory.Create(file);
                var song = CreateSongFromTags(file, reader);
                song = EnsureSongRead(file, song);
                Messenger.PostMessage(reader.Title + " (" + reader.Artist + ")", Messenger.TargetType.Frontend);
                lock (songs)
                {
                    songs.Add(song);
                }
            }
            catch (Exception e)
            {
                Messenger.Log("Load failed for " + file + ": " + e.Message + "\n" + e.StackTrace);
                try
                {
                    if (tagReaderFactory.BackupFactory != null)
                    {
                        var song = EnsureSongRead(file, new Song());
                        lock (songs)
                        {
                            songs.Add(song);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Messenger.Log("Failed again! " + ex.Message);
                }
            }
        }

        private Song EnsureSongRead(string file, Song song)
        {
            if (tagReaderFactory.BackupFactory != null && (song.Album == "<unknown>" || song.Artist == "<unknown>"))
            {
                Messenger.Log("Retrying for " + song.Name + ", " + file);
                song = CreateSongFromTags(file, tagReaderFactory.BackupFactory.Create(file));
                Messenger.Log("Read from backup: " + song.Name + " " + song.Album + " " + song.Artist);
            }

            return song;
        }

        private static Song CreateSongFromTags(string file, TagReader reader)
        {
            var song = new Song(
                reader.Artist?.Trim(),
                reader.Album?.Trim(),
                reader.Title?.Trim(),
                reader.TrackNo,
                file
            );

            return song;
        }
    }
}