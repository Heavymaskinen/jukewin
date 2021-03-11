using Juke.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Juke.Control;

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
            if (loaderInstance != null)
            {
                loaderInstance.Path = path;
                return loaderInstance;
            }

            Messenger.Log("AsyncSongLoader not initialised!");
            throw new Exception("AsyncSongLoader not initialised!");
        }
    }
}