using Juke.Control;
using Juke.IO;
using System;

namespace UiComponents
{
    public class ProgressTracker
    {
        public event EventHandler Changed;
        public double ProgressMax { get; set; }
        public double Progress { get; set; }
        public bool IsIndeterminate { get; set; }

        public ProgressTracker(LoadHandler loadHandler)
        {
            Progress = 0;
            ProgressMax = 100;
            IsIndeterminate = false;

            loadHandler.NewLoad += AsyncSongLoader_NewLoad;
            loadHandler.LoadInitiated += AsyncSongLoader_LoadInitiated;
            loadHandler.LoadProgress += AsyncSongLoader_LoadProgress;
        }

        private void AsyncSongLoader_NewLoad(object sender, EventArgs e)
        {
            Progress = 0;
            ProgressMax = 0;
            IsIndeterminate = true;
            RaisePropertyChanged(nameof(Progress));
           // RaisePropertyChanged(nameof(IsIndeterminate));
        }

        private void AsyncSongLoader_LoadInitiated(object sender, int load)
        {
            if (load <= 0) return;
            ProgressMax = load;
            Progress = 0;
            IsIndeterminate = false;
            Messenger.Log("Load initiated - New max: " + ProgressMax);
            RaisePropertyChanged(nameof(ProgressMax));
            //RaisePropertyChanged(nameof(Progress));
          //  RaisePropertyChanged(nameof(IsIndeterminate));
        }

        private void AsyncSongLoader_LoadProgress(object sender, int progress)
        {
            Progress += progress;
            RaisePropertyChanged(nameof(Progress));
        }

        void RaisePropertyChanged(string name)
        {
            Changed?.Invoke(name, EventArgs.Empty);
        }
    }
}
