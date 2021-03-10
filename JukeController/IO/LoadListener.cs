using DataModel;
using System;
using System.Collections.Generic;

namespace Juke.IO
{
    public interface LoadListener
    {
        event EventHandler<IList<Song>> Completed;
        void NotifyNewLoad();
        void NotifyLoadInitiated(int capacity);
        void NotifyProgress(int progressed);
        void NotifyCompleted(IList<Song> loadedSongs);
    }
}
