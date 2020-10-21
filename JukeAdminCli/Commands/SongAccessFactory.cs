using System;
using CoreSongIO;
using Juke.IO;

namespace JukeAdminCli.Commands
{
    public class SongAccessFactory : AbstractSongAccessFactory
    {
        public SongAccessFactory()
        {
        }

        public SongLoader CreateLoader(string extension, string path)
        {
            return new CoreSongLoader(extension, path);
        }

        public SongWriter CreateWriter(string filename)
        {
            var access = new JsonLibraryAccess();
            return new LibraryIO(filename, access, access);
        }
    }
}
