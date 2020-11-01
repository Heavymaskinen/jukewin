using System.Collections.Generic;
using System.Linq;
using ApiObjects;
using DataModel;
using Juke.Control;
using Microsoft.AspNetCore.Mvc;

namespace JukeWebAPI.Controllers
{
    [Route("[controller]")]
    public class SongsController : ControllerBase
    {
        private IJukeControl jukeControl;

        public SongsController(IJukeControl jukeControl)
        {
            this.jukeControl = jukeControl;
        }
        
        public ICollection<ApiSong> GetAllSongs()
        {
            var songs = jukeControl.Browser.Songs;
            var apiSongs = songs.Select(ConvertToApiSong).ToList();

            return apiSongs;
        }

        [HttpGet]
        [Route("/{id}")]
        public ApiSong GetSong(int id)
        {
            var song = jukeControl.Browser.GetSongByID(id);
            return song == null ? null : ConvertToApiSong(song);
        }

        private static ApiSong ConvertToApiSong(Song song)
        {
            return new ApiSong()
                {Title = song.Name, Album = song.Album, Artist = song.Artist};
        }
    }
}