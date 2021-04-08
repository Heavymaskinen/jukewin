using System;
using System.Net.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using System.Threading;
using CoreAudioComponent;
using Juke.Control;
using Juke.External.Xml;
using JukeApiLibrary;
using JukeApiModel;
using LocalRepositori;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UserMemoryRepository;

namespace BlazorUI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            //JukeController.Instance.Player.RegisterPlayerEngine(new CorePlayerEngine());

            var task = JukeController.Instance.LoadHandler.LoadSongs(new XmlSongReader("library.xml"));
            var memorySongLibrary = new MemorySongLibrary(new List<ApiSong>
            {
                new ApiSong("artist 1", "Album 1", "My song"),
                new ApiSong("artist 1", "Album 1", "My song 2"),
                new ApiSong("artist 1", "Album 1", "My song 3")
            });
            var memoryRepository = new MemoryRepository();
            memoryRepository.AddUser("user", "user");

            ApiConfiguration.SongLibrary    = new LocalSongLibrary(JukeController.Instance.Browser);
            ApiConfiguration.UserRepository = memoryRepository;

            builder.Services.AddScoped(
                sp => new HttpClient {BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)});

            var userSession = new UserSession();
            builder.Services.AddScoped(s => userSession);
            
            builder.Services.AddSingleton(userSession);

            builder.Services.AddScoped(
                jk => new LibraryApi());
            builder.Services.AddScoped(
                jk => new LoginApi());

            await task;
            await builder.Build().RunAsync();
        }
    }
}