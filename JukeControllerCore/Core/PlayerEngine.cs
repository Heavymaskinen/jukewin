using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.Core
{
    public abstract class PlayerEngine : IDisposable
    {
        public event EventHandler SongFinished;

        public abstract void Play(Song song);

        protected void SignalFinished()
        {
            SongFinished?.Invoke(this, EventArgs.Empty);
        }

        public abstract void Stop();
        public abstract void Dispose();
    }
}
