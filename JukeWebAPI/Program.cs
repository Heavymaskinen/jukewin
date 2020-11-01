using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Juke.Control;
using Juke.External.Xml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JukeWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            JukeController.Instance.LoadHandler.LoadSongs(new XmlSongReader("../Files/library.xml"));
            Console.WriteLine(JukeController.Instance.Browser.Songs.Count+" songs in JUKE");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}