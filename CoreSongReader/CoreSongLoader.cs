using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataModel;
using Juke.IO;
using MessageRouting;

namespace CoreSongIO
{
    public class CoreSongLoader : SongLoader
    {
        private string extension;
        private IList<Song> files;
        private DirectoryInfo startDir;

        public CoreSongLoader(string extension, string path)
        {
            this.extension = extension;
            startDir  = new DirectoryInfo(path);
        }


        public virtual IList<Song> LoadSongs()
        {
            files = RecursiveLoad(startDir, 0);

            return files;
        }

        private IList<Song> RecursiveLoad(DirectoryInfo directory, int level)
        {
            var foundSongs = new List<Song>();
            foreach (var file in directory.EnumerateFiles(extension))
            {
                if (file.Name.StartsWith("._"))
                {
                    continue;
                }

                try
                {
                    var song = ReadSong(file);
                    foundSongs.Add(song);
                    Messenger.Post("∞");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to read file: "+file+", "+e.Message);
                    Messenger.Post("Failed to read file: "+file+", "+e.Message+"\n");
                }
            }
            Messenger.Post("|\n");

            foreach (var dir in directory.EnumerateDirectories())
            {
                if (dir.Name.Length < 3) continue;
                foundSongs.AddRange(RecursiveLoad(dir, level + 1));
            }

            return foundSongs;
        }

        private static Song ReadSong(FileInfo file)
        {
            var tag = TagLib.File.Create(file.FullName).Tag;
            var song = new Song(tag.FirstAlbumArtist ?? "<Unknown>",
                tag.Album ?? "<Unknown>",
                tag.Title ?? "<>Untitled<>",
                tag.Track.ToString(),
                file.FullName ?? "Invalid!");
            return song;
        }
    }
}