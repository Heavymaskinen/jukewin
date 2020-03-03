using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;

namespace Juke.IO
{
    public abstract class AsyncSongLoader
    {
        public static event EventHandler LoadInitiated;
        public static event EventHandler<string> LoadProgress;
        public static event EventHandler<IList<Song>> LoadCompleted;

        public string Path { get; set; }

        internal void BeginLoading()
        {
            LoadInitiated?.Invoke(this, EventArgs.Empty);
            InvokeLoad();
        }

        protected abstract void InvokeLoad();

        protected void NotifyProgress(string message)
        {
            LoadProgress?.Invoke(this, message);
        }

        protected void NotifyCompleted(IList<Song> loadedSongs)
        {
            LoadCompleted?.Invoke(this, loadedSongs);
        }

    }
}
