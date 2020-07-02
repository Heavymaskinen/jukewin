using System;
using System.Collections.Generic;
using CoreSongIO;
using DataModel;

namespace JukeAdminTest
{
    public class FakeLibraryIO: LibraryIO
    {
        public string UsedPath { get; }

        public FakeLibraryIO(string path) :base(path, null, null)
        {
            UsedPath = path;
        }

        public override IList<Song> LoadSongs()
        {
            return new List<Song>();
        }

        public override void Write(IList<Song> songs)
        {
            
        }
    }
}
