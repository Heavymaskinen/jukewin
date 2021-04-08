using System;
using System.Collections.Generic;
using JukeApiLibrary;
using JukeApiModel;

namespace UserMemoryRepository
{
    public class MemorySongLibrary : SongLibrary
    {
        private List<ApiSong> songs;

        public MemorySongLibrary(List<ApiSong> songs)
        {
            this.songs = songs;
        }

        public List<ApiSong> GetAllSongs()
        {
            return songs;
        }
    }
}
