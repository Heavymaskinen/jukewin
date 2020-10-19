using System.Collections.Generic;
using System.Linq;
using ApiObjects;
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
            var apiSongs = songs.Select(song => new ApiSong()
                {Title = song.Name, Album = song.Album, Artist = song.Artist}).ToList();

            return apiSongs;
        }
    }
}