using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;

namespace Juke.IO
{
    public interface SongWriter
    {
        void Write(IList<Song> songs);
    }
}