using DataModel;
using Juke.Control;
using Juke.IO;
using System;
using System.Collections.Generic;

namespace UiComponents
{
    public class ProgressTracker : LoadListener
    {
        public event EventHandler Changed;
        public event EventHandler<IList<Song>> Completed;

        public double ProgressMax { get; set; }
        public double Progress { get; set; }
        public bool IsIndeterminate { get; set; }

        public ProgressTracker(LoadHandler loadHandler)
        {
            Progress = 0;
            ProgressMax = 100;
            IsIndeterminate = false;
        }

        void RaisePropertyChanged(string name)
        {
            Changed?.Invoke(name, EventArgs.Empty);
        }

        public void NotifyNewLoad()
        {
            Progress = 0;
            ProgressMax = 0;
            IsIndeterminate = true;
            RaisePropertyChanged(nameof(Progress));
            Messenger.Log("New progress start");
        }

        public void NotifyLoadInitiated(int load)
        {
            if (load <= 0) return;
            ProgressMax = load;
            Progress = 0;
            IsIndeterminate = false;
            Messenger.Log("Progress initiated - New max: " + ProgressMax);
            RaisePropertyChanged(nameof(ProgressMax));
        }

        public void NotifyProgress(int progress)
        {
            Progress += progress;
            RaisePropertyChanged(nameof(Progress));
        }

        public void NotifyCompleted(IList<Song> loadedSongs)
        {
            Messenger.Log("Progress completed " + Progress);
            Completed?.Invoke(this, loadedSongs);
        }
    }
}