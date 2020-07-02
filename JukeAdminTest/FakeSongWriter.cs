using System;
using System.Collections.Generic;
using DataModel;
using Juke.IO;

namespace JukeAdminTest
{
    public class FakeSongWriter : SongWriter
    {
        public string UsedFileName { get; set; }
        public bool IsWritten { get; private set; }

        public FakeSongWriter(string filename)
        {
            UsedFileName = filename;
        }

        public void Write(IList<Song> songs)
        {
            IsWritten = true;
        }
    }
}
