using System.Collections.Generic;
using DataModel;
using Juke.Control.Tests;
using Juke.UI.Admin;
using Stubs2;
using ViewModelCommands;

namespace Juke.UI.Tests
{
    public class ViewModelFaker
    {
        public static List<Song> CreateSongs(int artistMax, int albumMax, int songmax)
        {
            var songList = new List<Song>();
            for (var artist = 1; artist <= artistMax; artist++)
            {
                for (var album = 1; album <= albumMax; album++)
                {
                    for (var song = 1; song <= songmax; song++)
                    {
                        songList.Add(new Song("artist" + artist, "album" + album, "song" + song,
                            artist + song - 1 + "", artist + "/" + album + "/" + song));
                    }
                }
            }

            return songList;
        }

        public static List<Song> CreateSongsDistinct(int artistMax, int albumMax, int songmax)
        {
            var songList = new List<Song>();
            for (var artist = 1; artist <= artistMax; artist++)
            {
                for (var album = 1; album <= albumMax; album++)
                {
                    for (var song = 1; song <= songmax; song++)
                    {
                        songList.Add(new Song("artist" + artist, "album" + artist + album, "song"+artist+album + song,
                            song - 1 + "", artist + "/" + album + "/" + song));
                    }
                }
            }

            return songList;
        }

        public static SelectionModel InitializeLoadedViewModel(List<Song> songs)
        {
            var fakeLoader = CreateFakeLoadEngine(songs);
            var viewControl = new FakeViewControl("path");
            var viewModel = CreateViewModel(viewControl);
            CreateAdminViewModel(viewControl).LoadSongs.Execute(viewModel);

            fakeLoader.SignalComplete();
            return viewModel;
        }

        public static AdministratorModel InitializeLoadedAdminViewModel(List<Song> songs)
        {
            return InitializeLoadedAdminViewModel(songs, new FakeViewControl("Path"));
        }
        
        public static AdministratorModel InitializeLoadedAdminViewModel(List<Song> songs, FakeViewControl viewControl)
        {
            var fakeLoader = CreateFakeLoadEngine(songs);
            var viewModel = CreateAdminViewModel(viewControl);
            viewModel.LoadSongs.Execute(viewModel);
            fakeLoader.SignalComplete();
            return viewModel;
        }

        private static FakeSongLoadEngine CreateFakeLoadEngine(List<Song> songs)
        {
            var engine = new FakeSongLoadEngine(songs);
            LoaderFactory.SetLoaderInstance(new FakeAsyncSongLoader(engine, new FakeSongCollector(songs)));
            return engine;
        }

        private static SelectionModel CreateViewModel(FakeViewControl viewControl)
        {
            return new JukeViewModel(viewControl);
        }

        private static AdministratorModel CreateAdminViewModel(FakeViewControl viewControl)
        {
            return new AdminViewModel(viewControl);
        }
    }
}