using Juke.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace JukeControllerCore.IO
{
    public interface SaveHandler
    {
        void SaveSongs(SongWriter writer);
    }
}
