using Juke.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.UI
{
    public class LoaderFactory
    {
        private static IAsyncSongLoader loaderInstance;

        public static void SetLoaderInstance(IAsyncSongLoader loader)
        {
            loaderInstance = loader;
        }

        public IAsyncSongLoader CreateAsync(string path)
        {
            if (loaderInstance !=  null)
            {
                loaderInstance.Path = path;
                return loaderInstance;
            }

            throw new Exception("AsyncSongLoader not initialised!");
        }
    }
}
