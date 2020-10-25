using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.IO
{
    public interface LoadListener
    {
        void NotifyNewLoad2();

        void NotifyLoadInitiated2(int capacity);

        void NotifyProgress2(int progressed);
        void NotifyCompleted2(IList<Song> loadedSongs);
    }
}
