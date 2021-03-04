using System.Collections.Generic;
using System.Threading.Tasks;

namespace Juke.IO
{
    /// <summary>
    /// Used to create Song-objects from files asynchronously.
    /// Result should be passed to LoadListener through NotifyCompleted
    /// </summary>
    public interface ISongCollector
    {
        Task Load(List<string> files, LoadListener listener);
    }
}