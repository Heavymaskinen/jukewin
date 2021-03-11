using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.External.Wmp
{
    class WindowsFolderBrowserFactory : IFolderBrowserFactory
    {
        public IFolderBrowser Create(string path)
        {
            return new WindowsFolderBrowser(path);
        }
    }
}