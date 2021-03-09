using DataModel;
using System;
using System.Collections.Generic;

namespace Juke.IO
{
    public interface SongLoader
    {
       IList<Song> LoadSongs();
    }
}
