using DataModel;
using Juke.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WMPLib;

namespace Juke.External.Wmp.IO
{
    public class AsyncFileFinder : AsyncSongLoader
    {
        private static int ThreadCount = 0;
        private WindowsMediaPlayer player;
        private IList<Song> songs;

        public AsyncFileFinder(string path) : base(new TaglibTagReaderFactory())
        {
            Path = path;
            player = new WindowsMediaPlayer();
            songs = new List<Song>();
        }

        protected override void InvokeLoad()
        {
            //Console.WriteLine("Load started " + Thread.CurrentThread.ManagedThreadId + ", threads: " + ThreadCount + ", dir: " + Path);
            LoadAsync().ConfigureAwait(false);
        }

        private async Task LoadAsync()
        {
            await AddFiles(Path);
        }

        private async Task AddFiles(string path)
        {
            var foundFiles = Directory.EnumerateFiles(path, "*.mp3");
            var directories = Directory.EnumerateDirectories(path);

            //Console.WriteLine("Initiated " + foundFiles.Count());
            //NotifyLoadInitiated(foundFiles.Count());
            foreach (var dir in directories)
            {
                if (ThreadCount < 5)
                {
                    ThreadCount++;
                    Task item = Task.Run(() => new AsyncFileFinder(dir).InvokeLoad());
                    AsyncSongLoader.tasks.Add(item);
                }
                else
                {
                    //Console.WriteLine("Add sync dir " + dir + ", Thread " + Thread.CurrentThread.ManagedThreadId);
                    //await AddFiles(dir).ConfigureAwait(false);
                    AddFileSync(dir);
                }
            }

            if (foundFiles.Count() == 0)
            {
                NotifyProgress(1);
            }

           // Console.WriteLine("Add " + foundFiles.Count() + " async, Thread " + Thread.CurrentThread.ManagedThreadId);
            foreach (var file in foundFiles)
            {
                if (file.Contains("._"))
                {
                    NotifyProgress(1);
                    continue;
                }

                try
                {
                    await AddSong(file).ConfigureAwait(false);
                    NotifyProgress(1);
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: " + e.Message);
                    break;
                }
            }

            if (ThreadCount > 0)
            {
                ThreadCount--;
            }

            if (foundFiles.Count() > 0)
            {
                Console.WriteLine("Completed: " + path + " Thread " + Thread.CurrentThread.ManagedThreadId + " Threads now: " + ThreadCount);
                NotifyCompleted(songs);
            }
        }

        private void AddFileSync(string path)
        {
            var foundFiles = Directory.EnumerateFiles(path, "*.mp3");
            var directories = Directory.EnumerateDirectories(path);

            foreach (var dir in directories)
            {
                //Console.WriteLine("Sync dir " + dir + ", Thread " + Thread.CurrentThread.ManagedThreadId);
                AddFileSync(dir);
            }

            if (foundFiles.Count() == 0)
            {
                NotifyProgress(1);
                return;
            }

            //Console.WriteLine("Add " + foundFiles.Count() + " in sync, Thread " + Thread.CurrentThread.ManagedThreadId);


            foreach (var file in foundFiles)
            {
                if (file.Contains("._"))
                {
                    NotifyProgress(1);
                    continue;
                }

                try
                {
                    AddSongSync(file);
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: " + e.Message);
                    break;
                }
            }

            if (foundFiles.Count() > 0)
            {
                //Console.WriteLine("Sync Completed: " + path + " Thread " + Thread.CurrentThread.ManagedThreadId);
                NotifyCompleted(songs);
            }
        }

        private async Task AddSong(string file)
        {
            var task = Task.Run(() =>
               {
                   var reader = tagReaderFactory.Create(file);

                   var song = new Song(
                       reader.Artist,
                       reader.Album,
                       reader.Title,
                       reader.TrackNo,
                       file
                   );

                   songs.Add(song);

                //Console.WriteLine("Done async file " + file + ", Thread " + Thread.CurrentThread.ManagedThreadId);

                NotifyProgress(1);
               });
            AsyncSongLoader.tasks.Add(task);
            await task;
        }

        private void AddSongSync(string file)
        {
            var reader = tagReaderFactory.Create(file);

            var song = new Song(
                reader.Artist,
                reader.Album,
                reader.Title,
                reader.TrackNo,
                file
            );

            songs.Add(song);
           // Console.WriteLine("Done SYNC file " + file + ", Thread " + Thread.CurrentThread.ManagedThreadId);
            NotifyProgress(1);
        }


    }

    
}
