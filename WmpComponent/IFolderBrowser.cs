using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.External.Wmp
{
    public interface IFolderBrowser
    {
        List<IFolderBrowser> GetSubFolders();
        IList<string> GetFiles();
    }
}