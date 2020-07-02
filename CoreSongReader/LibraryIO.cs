using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using DataModel;
using Juke.IO;
using MessageRouting;


namespace CoreSongIO
{
    public class LibraryIO :SongLoader, SongWriter
    {
        private string filename;
        private LibrarySerializer serializer;
        private LibraryDeserializer deserializer;

        public LibraryIO(string filename, LibrarySerializer serializer, LibraryDeserializer deserializer)
        {
            this.filename = filename;
            this.serializer = serializer;
            this.deserializer = deserializer;
        }
        
        public virtual IList<Song> LoadSongs()
        {
            if (!File.Exists(filename))
            {
                Messenger.Post(filename+" doesn't exist!");
                return new Collection<Song>();
            }

            var stream = new StreamReader(filename);

            try
            {
                var list       = deserializer.Deserialize(stream);
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
            
            return new List<Song>();
        }

        public virtual void Write(IList<Song> songs)
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