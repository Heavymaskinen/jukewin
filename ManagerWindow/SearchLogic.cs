using DataModel;
using System.Collections.Generic;

namespace Juke.UI.Wpf
{
    public interface SearchLogic
    {
        List<Song> Search(string input);
        string Name { get; }
    }
}
