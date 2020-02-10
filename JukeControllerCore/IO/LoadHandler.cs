using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.IO
{
    public interface LoadHandler
    {
        event EventHandler LibraryUpdated;

        void LoadSongs(SongLoader loader);
        void LoadSongs(AsyncSongLoader loader);
    }
}
