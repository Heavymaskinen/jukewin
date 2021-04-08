using System;
using System.Collections.Generic;
using JukeApiModel;

namespace JukeApiLibrary
{
    public interface SongLibrary
    {
        List<ApiSong> GetAllSongs();
    }
}
