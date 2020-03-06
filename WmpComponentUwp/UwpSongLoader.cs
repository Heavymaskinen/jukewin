using DataModel;
using Juke.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace WmpComponentUwp
{
    public class UwpSongLoader : AsyncSongLoader
    {
        private ICollection<string> files;
        private IList<Song> songs;

        public event EventHandler<string> FilesReady;

        public bool Ready { get; private set; }

        public UwpSongLoader()
        {
        }

        public async void Open()
        {
            Ready = false;
            songs = new List<Song>();
            files = new List<string>();
            var dialog = new FileOpenPicker();
            dialog.FileTypeFilter.Add(".mp3");
            dialog.CommitButtonText = "Select";
            dialog.ViewMode = PickerViewMode.List;
            dialog.SuggestedStartLocation = PickerLocationId.MusicLibrary;

            var fileStores = await dialog.PickMultipleFilesAsync();
            if (files != null)
            {
                foreach (var file in fileStores)
                {
                    files.Add(file.Path);
                }

                FilesReady?.Invoke(this, files.Count + "!");
            }
        }

        protected override void InvokeLoad()
        {
            _ = RunThroughFilesAsync();
        }

        private async Task RunThroughFilesAsync()
        {
            foreach (var filename in files)
            {
                await LoadSongFromFileAsync(filename).ConfigureAwait(false);
            }

            NotifyCompleted(songs);
        }

        private async Task LoadSongFromFileAsync(string filename)
        {
            var file = await StorageFile.GetFileFromPathAsync(filename);
            var properties = await file.Properties.GetMusicPropertiesAsync();
            var song = new Song(properties.Artist, properties.Album, properties.Title, properties.TrackNumber.ToString(), filename);
            lock (songs)
            {
                songs.Add(song);

                NotifyProgress("Added " + song.Name);
            }
        }
    }
}
