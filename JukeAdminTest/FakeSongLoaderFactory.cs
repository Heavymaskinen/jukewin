using System;
using CoreSongIO;
using Juke.IO;
using JukeAdminCli.Commands;

namespace JukeAdminTest
{
    public class FakeSongLoaderFactory : AbstractSongAccessFactory
    {
        public FakeSongLoader CreatedLoader;
        public FakeSongLoader SpecifiedLoader;
        public FakeSongWriter CreatedWriter;

        public FakeSongLoaderFactory()
        {
        }

        public FakeSongLoaderFactory(FakeSongLoader fakeLoader)
        {
            SpecifiedLoader = fakeLoader;
        }

        public SongLoader CreateLoader(string extension, string path)
        {
            if (SpecifiedLoader != null)
            {
                return SpecifiedLoader;
            }

            CreatedLoader = new FakeSongLoader(path);
            return CreatedLoader;
        }

        public SongWriter CreateWriter(string filename)
        {
            CreatedWriter = new FakeSongWriter(filename);
            return CreatedWriter;
        }
    }
}
