using System.Threading.Tasks;

namespace Juke.IO
{
    public interface IAsyncSongLoader
    {
        Task StartNewLoad(LoadListener listener);
        string Path { get; set; }
    }
}