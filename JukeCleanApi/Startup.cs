using System.Collections.Generic;
using JukeApiLibrary;
using JukeApiModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserMemoryRepository;

namespace JukeCleanApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var repo = new MemoryRepository();
            repo.AddUser("admin", "1234");
            repo.AddUser("dummy", "password");

            var library = new MemorySongLibrary(new List<ApiSong>() {
                new ApiSong("Lovelove","Love first","Love me now"),
                new ApiSong("Badass Inc.","That's a bad ass","I want ass")
            });

            ApiConfiguration.UserRepository = repo;
            ApiConfiguration.SongLibrary = library;


            services.AddMvc(option => option.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Latest);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == "Development")
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            
            app.UseMvc();
        }
    }
}
