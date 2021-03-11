using Juke.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Threading;
using Juke.Control;

namespace Juke.External.Xml
{
    public class XmlSongReader : IAsyncSongLoader
    {
        private string filename;
        private static readonly XmlSerializer serializerInstance = new XmlSerializer(typeof(PersistedSong[]));

        public XmlSongReader(string filename)
        {
            this.filename = filename;
        }

        public IList<Song> LoadSongs()
        {
            return LoadFromXml();
        }

        private IList<Song> LoadFromXml()
        {
            if (!File.Exists(filename))
            {
                throw new Exception(filename + " not found!");
            }

            List<PersistedSong> songs = null;

            using (XmlReader reader = XmlReader.Create(filename))
            {
                songs = new List<PersistedSong>((PersistedSong[]) serializerInstance.Deserialize(reader));
            }

            return new SongConverter().ConvertPersistedSongs(songs);
        }

        public Task StartNewLoad(LoadListener listener, CancellationToken cancelToken)
        {
            return Task.Run(() => LoadFromXml(listener));
        }

        private IList<Song> LoadFromXml(LoadListener listener)
        {
            if (!File.Exists(filename))
            {
                throw new Exception(filename + " not found!");
            }

            listener.NotifyNewLoad();

            List<PersistedSong> songs = null;

            using (var reader = XmlReader.Create(filename))
            {
                songs = new List<PersistedSong>((PersistedSong[]) serializerInstance.Deserialize(reader));
            }

            Messenger.Log("XML library read");
            var convertedSongs = new SongConverter().ConvertPersistedSongs(songs);
            Messenger.Log("Persisted songs converted");
            listener.NotifyLoadInitiated(convertedSongs.Count);
            listener.NotifyCompleted(convertedSongs);
            return convertedSongs;
        }

        public string Path { get; set; }
    }
}