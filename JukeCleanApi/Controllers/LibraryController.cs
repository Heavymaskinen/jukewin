using System;
using System.Collections.Generic;
using JukeApiLibrary;
using JukeApiModel;
using Microsoft.AspNetCore.Mvc;

namespace JukeCleanApi
{
    [Route("api/[controller]")]
    public class LibraryController : ControllerBase
    {

        private LibraryApi controller = new LibraryApi();

        [HttpGet]
        public ActionResult<List<Song>> Get([FromQuery] int id)
        {
            return controller.GetSongs(id);
        }
    }
}
