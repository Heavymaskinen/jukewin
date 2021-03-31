using System.Collections.Generic;

namespace GrpcClient
{
    public interface JukeClient
    {
        bool Startup(string serverLocation, bool verbose);
        bool Shutdown();
        string[] Search(string query);
        bool LoadLibrary(string file);
        void Play(string name);
        string PlayRandom();
        bool AddSongs(string folder);
        Dictionary<string, string> GetInfo();
        void StreamOutput();
    }
}