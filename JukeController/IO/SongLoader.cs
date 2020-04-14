using DataModel;
using System;
using System.Collections.Generic;

namespace Juke.IO
{
    public abstract class SongLoader
    {
        public static event EventHandler NewLoad;
        public static event EventHandler<int> LoadInitiated;
        public static event EventHandler<int> LoadProgress;
        public static event EventHandler<IList<Song>> LoadCompleted;

        public abstract IList<Song> LoadSongs();

        protected void NotifyNewLoad()
        {
            NewLoad?.Invoke(this, EventArgs.Empty);
        }

        protected void NotifyLoadInitiated(int capacity)
        {
            LoadInitiated?.Invoke(this, capacity);
        }

        protected void NotifyProgress(int progressed)
        {
            LoadProgress?.Invoke(this, progressed);
        }

        protected void NotifyCompleted(IList<Song> loadedSongs)
        {
            LoadCompleted?.Invoke(this, loadedSongs);
        }
    }
}
