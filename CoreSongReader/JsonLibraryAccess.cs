using System;
using System.Collections.Generic;
using System.IO;

using Juke.IO;
using Newtonsoft.Json;

namespace CoreSongIO
{
    public class JsonLibraryAccess: LibraryDeserializer, LibrarySerializer
    {
        private JsonSerializer serializer;
        public JsonLibraryAccess()
        {
            serializer = new JsonSerializer();
        }

        public List<PersistedSong> Deserialize(TextReader stream)
        {
            return (List<PersistedSong>) serializer.Deserialize(stream, typeof(List<PersistedSong>));
        }

        public void Serialize(TextWriter stream, List<PersistedSong> persistableSongs)
        {
            serializer.Serialize(stream, persistableSongs);
        }
    }
}
