using System;
using System.Collections.Generic;
using System.IO;
using Juke.IO;

namespace CoreSongIO
{
    public interface LibrarySerializer
    {
        void Serialize(TextWriter stream, List<PersistedSong> persistableSongs);
    }
}
