using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using DataModel;
using Juke.IO;
using MessageRouting;


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
            if (!File.Exists(filename))
            {
                Messenger.Post(filename+" doesn't exist!");
                return new Collection<Song>();
            }

            var stream = new StreamReader(filename);

            try
            {
                var serializer = new XmlSerializer(typeof(List<PersistedSong>));
                var list       = (List<PersistedSong>) serializer.Deserialize(stream);
                var songs      = list.Select(s => s.ToSong()).ToList();
                Messenger.Post(songs.Count + " songs loaded");
                return songs;
            }
            catch (Exception e)
            {
                Messenger.Post("Error occured: "+e.Message);
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
            
            return new Collection<Song>();
            ;
        }

        public void Write(IList<Song> songs)
        {
            Messenger.Post("Writing "+songs.Count+" songs");
            var persisted = new List<PersistedSong>();
            foreach (var song in songs)
            {
                persisted.Add(new PersistedSong(song));
            }

            var stream = new StreamWriter(filename,false);
            try
            {
                var serializer = new XmlSerializer(typeof(List<PersistedSong>));
                serializer.Serialize(stream, persisted);
                stream.Flush();
            }
            catch (Exception e)
            {
                Messenger.Post("Error occured! "+e.Message);
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}