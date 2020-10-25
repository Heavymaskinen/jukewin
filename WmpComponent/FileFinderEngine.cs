using DataModel;
using Juke.Control;
using Juke.Core;
using Juke.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Juke.External.Wmp
{
    public class FileFinderEngine : LoadEngine
    {
        private LoadListener listener;
        private string path;
        private List<string> collectedFiles;
        private List<string> doneDirs;
        private List<Task> tasks;
        private TagReaderFactory tagReaderFactory;

        public FileFinderEngine(TagReaderFactory tagReaderFactory)
        {
            this.tagReaderFactory = tagReaderFactory;
        }

        public void Load(string path, LoadListener listener)
        {
            this.listener = listener;
            this.path = path;
            this.listener.NotifyNewLoad2();
            collectedFiles = new List<string>();
            doneDirs = new List<string>();
            tasks = new List<Task>();
            Task.Run(async () => { await AsyncLoad(); });

        }

        private async Task AsyncLoad()
        {
            Console.WriteLine("Thread async: " + Thread.CurrentThread.ManagedThreadId);
            Messenger.PostMessage("Counting files, please be patient", Messenger.TargetType.Frontend);
            await CountFiles(path);
            Task.WaitAll(tasks.ToArray());
            Console.WriteLine("Hurra??");
            tasks.Clear();
            Messenger.PostMessage("Now adding " + collectedFiles.Count + " files to library. Please be patient.", Messenger.TargetType.Frontend);
            listener.NotifyLoadInitiated2(collectedFiles.Count);
            await ProcessFiles();
            Task.WaitAll(tasks.ToArray());
            Console.WriteLine("All done!!");
            if (loadedSongs.Count > 0)
            {
                listener.NotifyCompleted2(loadedSongs);
                loadedSongs.Clear();

            }
            Console.WriteLine("Tada?");

        }

        private async Task CountFiles(string path)
        {
            var foundFiles = Directory.EnumerateFiles(path, "*.mp3");
            var directories = Directory.EnumerateDirectories(path);

            collectedFiles.AddRange(foundFiles);
            doneDirs.Add(path);
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
                var t = CountFiles(dir);
                tasks.Add(t);
            }

        }

        private int threadMax = 10;

        private List<Song> loadedSongs = new List<Song>();
        private async Task ProcessFiles()
        {
            var innerTaskList = new List<Task>();
            foreach (var file in collectedFiles)
            {
                if (file.Contains("._"))
                {
                    continue;
                }

                if (tasks.Count > threadMax)
                {
                    Task.WaitAny(tasks.ToArray());
                    threadMax += 5;
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

                    listener.NotifyProgress2(1);

                });

                tasks.Add(task);
            }


        }
    }
}
