using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juke.UI.Wpf
{
    public interface SearchLogic
    {
        List<Song> Search(string input);
        string Name { get; }
    }
}
