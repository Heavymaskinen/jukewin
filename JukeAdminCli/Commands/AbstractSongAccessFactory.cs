using System;
using CoreSongIO;
using Juke.IO;

namespace JukeAdminCli.Commands
{
    public interface AbstractSongAccessFactory
    {
        SongLoader CreateLoader(string extension, string path);
        SongWriter CreateWriter(string filename);
    }
}
