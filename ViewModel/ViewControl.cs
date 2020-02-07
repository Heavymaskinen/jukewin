using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;

namespace Juke.UI
{
    public interface ViewControl
    {
        string PromptPath();
        SongUpdate PromptSongData();
    }
}
