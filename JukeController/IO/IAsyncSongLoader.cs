using System.Threading;
using System.Threading.Tasks;

namespace Juke.IO
{
    public interface IAsyncSongLoader
    {
        Task StartNewLoad(LoadListener listener, CancellationToken cancelToken);
        string Path { get; set; }
    }
}