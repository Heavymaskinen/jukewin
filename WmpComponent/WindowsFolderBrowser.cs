using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.External.Wmp
{
    class WindowsFolderBrowser : IFolderBrowser
    {
        private string path;

        public WindowsFolderBrowser(string path)
        {
            this.path = path;
        }

        public IList<string> GetFiles()
        {
            return Directory.EnumerateFiles(path, "*.mp3").ToList();
        }

        public List<IFolderBrowser> GetSubFolders()
        {
            var dirs = Directory.EnumerateDirectories(path);
            var folderBrowsers = new List<IFolderBrowser>();
            foreach (var dir in dirs)
            {
                folderBrowsers.Add(new WindowsFolderBrowser(dir));
            }

            return folderBrowsers;
        }
    }
}