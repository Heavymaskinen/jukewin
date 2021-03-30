using System;
using System.Collections.Generic;
using System.IO;
using Juke.IO;

namespace CoreSongIO
{
    public interface LibraryDeserializer
    {
        List<PersistedSong> Deserialize(TextReader stream);
    }
}
