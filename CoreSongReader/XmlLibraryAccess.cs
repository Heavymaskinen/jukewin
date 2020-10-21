using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Juke.IO;

namespace CoreSongIO
{
    public class XmlLibraryAccess : LibraryDeserializer, LibrarySerializer
    {
        public XmlLibraryAccess()
        {
        }

        public List<PersistedSong> Deserialize(TextReader stream)
        {
            var serializer = new XmlSerializer(typeof(List<PersistedSong>));
            var list = (List<PersistedSong>)serializer.Deserialize(stream);
            return list;
        }

        public void Serialize(TextWriter stream, List<PersistedSong> persistableSongs)
        {
            var serializer = new XmlSerializer(typeof(List<PersistedSong>));
            serializer.Serialize(stream, persistableSongs);
        }
    }
}
