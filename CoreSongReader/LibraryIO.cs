using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using DataModel;
using Juke.IO;


namespace CoreSongIO
{
    public class LibraryIO :SongLoader, SongWriter
    {
        private string filename;

        public LibraryIO(string filename)
        {
            this.filename = filename;
        }
        
        public IList<Song> LoadSongs()
        {
            var file = File.Open(filename, FileMode.Open);
            var serializer = new XmlSerializer(typeof(List<PersistedSong>));
            var list = (List<PersistedSong>) serializer.Deserialize(file);
            var songs = list.Select(s => s.ToSong()).ToList();
            return songs;
        }

        public void Write(IList<Song> songs)
        {
            var persisted = new List<PersistedSong>();
            foreach (var song in songs)
            {
                persisted.Add(new PersistedSong(song));
            }


            var file = File.Open(filename, FileMode.OpenOrCreate);
            try
            {
                var serializer = new XmlSerializer(typeof(List<PersistedSong>));
                serializer.Serialize(file, persisted);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                file.Close();
            }
        }
    }
}