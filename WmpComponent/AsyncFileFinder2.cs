using DataModel;
using Juke.Control;
using Juke.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Juke.External.Wmp
{
    public class AsyncFileFinder2 : AsyncSongLoader
    {
        private List<Task> taskList;
        private List<string> collectedFiles;
        public AsyncFileFinder2(string path) : base(new TaglibTagReaderFactory())
        {
            Path = path;
            collectedFiles = new List<string>();
            taskList = new List<Task>();
        }

        protected override void InvokeLoad()
        {
            taskList.Clear();
            NotifyNewLoad();
            var task = AsyncLoad();
            AsyncSongLoader.tasks.Add(task);
            task.ConfigureAwait(false);
        }

        private async Task AsyncLoad()
        {
            var task = Task.Run(async () =>
            {
                Messenger.PostMessage("Counting files, please be patient", Messenger.TargetType.Frontend);
                await CountFiles(Path).ConfigureAwait(false);
                Messenger.PostMessage("Now adding " + collectedFiles.Count + " files to library. Please be patient.", Messenger.TargetType.Frontend);
                NotifyLoadInitiated(collectedFiles.Count);
                ProcessFiles();
            });
            AsyncSongLoader.tasks.Add(task);
            await task.ConfigureAwait(false);
        }

        private List<string> doneDirs = new List<string>();

        private async Task CountFiles(string path)
        {
            var foundFiles = Directory.EnumerateFiles(path, "*.mp3");
            var directories = Directory.EnumerateDirectories(path);

            collectedFiles.AddRange(foundFiles);

            if (directories.Count() > 0)
            {
                if (collectedFiles.Count >= 20000)
                {
                    Messenger.PostMessage("Over 20000 files! Don't worry, we'll get there ...", Messenger.TargetType.Frontend);
                }
                else if (collectedFiles.Count >= 10000)
                {
                    Messenger.PostMessage("Over 1000 files! That's quite a collection ...", Messenger.TargetType.Frontend);
                }
                else if (collectedFiles.Count >= 5000)
                {
                    Messenger.PostMessage("Over 5000 files! This could take a while...", Messenger.TargetType.Frontend);
                }
            }
            foreach (var dir in directories)
            {
                await CountFiles(dir).ConfigureAwait(false);
                doneDirs.Add(dir);
            }

        }


        private List<Song> loadedSongs = new List<Song>();
        private void ProcessFiles()
        {
            var innerTaskList = new List<Task>();
            foreach (var file in collectedFiles)
            {
                if (file.Contains("._"))
                {
                    continue;
                }

                if (innerTaskList.Count > 10)
                {
                    while (innerTaskList.Count > 0)
                    {
                        for (int i = 0; i < innerTaskList.Count; i++)
                        {
                            if (innerTaskList[i].IsCompleted)
                            {
                                innerTaskList.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }

                Task task = Task.Run(() =>
                   {

                       var reader = tagReaderFactory.Create(file);
                       Messenger.PostMessage(reader.Title + " (" + reader.Artist + ")", Messenger.TargetType.Frontend);

                       var song = new Song(
                           reader.Artist,
                           reader.Album,
                           reader.Title,
                           reader.TrackNo,
                           file
                       );

                       loadedSongs.Add(song);

                       NotifyProgress(1);
                       if (false && loadedSongs.Count >= 10)
                       {
                           NotifyCompleted(loadedSongs);
                           loadedSongs.Clear();
                       }

                   });
                innerTaskList.Add(task);
                tasks.Add(task);
                task.ConfigureAwait(false);
            }

            Console.WriteLine("Now await all!");
            var ta = Task.WhenAll(innerTaskList).ContinueWith((Task t) =>
            {
                if (loadedSongs.Count > 0)
                {
                    NotifyCompleted(loadedSongs);
                    loadedSongs.Clear();

                }
                Console.WriteLine("Done loading?");
            });
            tasks.Add(ta);

        }
    }
}
