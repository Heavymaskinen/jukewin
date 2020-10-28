using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.IO
{
    public interface LoadEngine
    {
        void Load(string path, LoadListener listener);
        Task LoadAsync(string path, LoadListener listener);
    }
}
