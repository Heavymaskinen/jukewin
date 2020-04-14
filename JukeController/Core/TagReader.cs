using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.Core
{
    public interface TagReader
    {
        string Title { get; }
        string Album { get; }
        string Artist { get; }
        string TrackNo { get; }
    }
}
