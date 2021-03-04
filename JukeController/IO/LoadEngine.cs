using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.IO
{
    public interface LoadEngine
    {
        List<string> Load(string path, LoadListener listener);
        Task<List<string>> LoadAsync(string path, LoadListener listener);
    }
}
