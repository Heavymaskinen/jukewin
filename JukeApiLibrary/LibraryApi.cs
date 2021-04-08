using System.Collections.Generic;
using JukeApiModel;

namespace JukeApiLibrary
{
    public class LibraryApi
    {
        private UserRepository userRepository;
        private SongLibrary songLibrary;

        public LibraryApi()
        {
            userRepository = ApiConfiguration.UserRepository;
            songLibrary = ApiConfiguration.SongLibrary;
        }

        public List<ApiSong> GetSongs(int tokenId)
        {
            var token = userRepository.GetFromId(tokenId);
            return token.IsValid() ? songLibrary.GetAllSongs() : new List<ApiSong>() { };
        }
    }
}
