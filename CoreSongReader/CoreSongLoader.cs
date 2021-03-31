using System;
using System.Collections.Generic;
using System.IO;
using DataModel;
using Juke.IO;

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
            this.startDir = new DirectoryInfo(path);
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
                var tag = TagLib.File.Create(file.FullName).Tag;
                var song = new Song(tag.FirstAlbumArtist, tag.Album, tag.Title, tag.Track.ToString(), file.FullName);
                foundSongs.Add(song);
            }

            foreach (var dir in directory.EnumerateDirectories())
            {
                if (dir.Name.Length < 3) continue;
                foundSongs.AddRange(RecursiveLoad(dir, level+1));
            }

            return foundSongs;
        }
    }
}
