using DataModel;
using Juke.Core;
using Juke.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.Control.Tests
{
    public class EventListener : LoadListener
    {
        public bool LoadCompleted { get; set; }
        public bool LoadInitiated { get; private set; }
        public string ProgressNoted { get; set; }
        public Song SongPlayed { get; set; }

        public EventListener()
        {
            JukeController.Instance.LoadHandler.LoadInitiated += SongLoader_LoadInitiated;
            JukeController.Instance.LoadHandler.LoadCompleted += LoadHandler_LoadCompleted;
            JukeController.Instance.LoadHandler.LoadProgress += LoadHandlerOnLoadProgress;
            Player.SongPlayed += Instance_SongPlayed;
        }

        private void LoadHandlerOnLoadProgress(object sender, int e)
        {
            ProgressNoted = e.ToString();
        }

        private void LoadHandler_LoadCompleted(object sender, EventArgs e)
        {
            LoadCompleted = true;
        }

        private void Instance_SongPlayed(object sender, Song e)
        {
            SongPlayed = e;
        }


        private void SongLoader_LoadInitiated(object sender, int e)
        {
            LoadInitiated = true;
        }

        public event EventHandler<IList<Song>> Completed;

        public void NotifyNewLoad()
        {
            LoadInitiated = true;
        }

        public void NotifyLoadInitiated(int capacity)
        {
            LoadInitiated = true;
        }

        public void NotifyProgress(int progressed)
        {
            ProgressNoted = progressed.ToString();
        }

        public void NotifyCompleted(IList<Song> loadedSongs)
        {
            LoadCompleted = true;
        }
    }
}