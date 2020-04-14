using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Juke.UI.Command;

namespace Juke.UI
{
    public interface ViewControl
    {
        string PromptPath();

        SongUpdate PromptSongData(JukeViewModel.InfoType infoType);

        void CommandCompleted(JukeCommand command);
    }
}
