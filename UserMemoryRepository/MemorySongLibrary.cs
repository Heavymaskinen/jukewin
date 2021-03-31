using System;
using System.Collections.Generic;
using JukeApiLibrary;
using JukeApiModel;

namespace UserMemoryRepository
{
    public class MemorySongLibrary : SongLibrary
    {
        private List<Song> songs;

        public MemorySongLibrary(List<Song> songs)
        {
            this.songs = songs;
        }

        public List<Song> GetAllSongs()
        {
            return songs;
        }
    }
}
