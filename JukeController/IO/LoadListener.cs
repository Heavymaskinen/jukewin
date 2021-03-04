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

        void NotifyNewLoad();

        void NotifyLoadInitiated(int capacity);

        void NotifyProgress(int progressed);
        void NotifyCompleted(IList<Song> loadedSongs);
    }
}
