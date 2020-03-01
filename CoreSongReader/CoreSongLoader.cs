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


        public IList<Song> LoadSongs()
        {
            files = new List<Song>();
            foreach (var file in startDir.EnumerateFiles(extension))
            {
                var tag = TagLib.File.Create(file.FullName).Tag;
                var song = new Song(tag.FirstAlbumArtist, tag.Album, tag.Title, tag.Track.ToString(), file.FullName);
                files.Add(song);
            }

            return files;
        }
    }
}
