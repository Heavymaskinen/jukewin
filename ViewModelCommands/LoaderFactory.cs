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
        private static IAsyncSongLoader libraryLoader;

        public static void SetLoaderInstance(IAsyncSongLoader loader)
        {
            loaderInstance = loader;
        }

        public static void SetLibraryLoaderInstance(IAsyncSongLoader loader)
        {
            libraryLoader = loader;
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
        
        public IAsyncSongLoader CreateLibraryLoader(string path)
        {
            if (libraryLoader != null)
            {
                libraryLoader.Path = path;
                return libraryLoader;
            }

            Messenger.Log("LibraryLoader not initialised!");
            throw new Exception("LibraryLoader not initialised!");
        }
    }
}