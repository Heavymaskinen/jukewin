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
    public class EventListener
    {
        public bool LoadCompleted { get; set; }
        public bool LoadInitiated { get; private set; }
        public string ProgressNoted { get; set; }
        public string CollectionChanged { get; private set; }
        public Song SongPlayed { get; set; }

        public EventListener()
        {
            AsyncSongLoader.LoadInitiated += SongLoader_LoadInitiated;
            AsyncSongLoader.LoadCompleted += AsyncSongLoader_LoadCompleted1;
            AsyncSongLoader.LoadProgress += AsyncSongLoader_LoadProgress;
            Player.SongPlayed += Instance_SongPlayed;
        }

        private void Instance_SongPlayed(object sender, Song e)
        {
            SongPlayed = e;
        }

        public void ListenToCollections(INotifyPropertyChanged coll)
        {
            coll.PropertyChanged += Coll_PropertyChanged;
        }

        private void Coll_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CollectionChanged = e.PropertyName;
        }
        
        private void AsyncSongLoader_LoadCompleted1(object sender, IList<Song> e)
        {
            LoadCompleted = true;
        }

        private void AsyncSongLoader_LoadProgress(object sender, string e)
        {
            ProgressNoted = e;
        }
        
        private void SongLoader_LoadInitiated(object sender, EventArgs e)
        {
            LoadInitiated = true;
        }
    }
}
