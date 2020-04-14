using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.Core
{
    public interface TagReaderFactory
    {
        TagReader Create(string filename);
    }
}
