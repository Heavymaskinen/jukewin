using System;
using JukeApiModel;

namespace JukeApiLibrary
{
    public interface SongPlayer
    {
        void Play(ApiSong song);
    }
}
