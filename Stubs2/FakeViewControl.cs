using Juke.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Juke.UI.Command;

namespace Juke.UI.Tests
{
    public class FakeViewControl : ViewControl
    {
        private string path;

        public FakeViewControl(string pathForPrompt)
        {
            path = pathForPrompt;
        }

        public string PromptPath()
        {
            return path;
        }

        public SongUpdate PromptSongData(JukeViewModel.InfoType info)
        {
            return SongDataToReturn;
        }

        public void CommandCompleted(JukeCommand command)
        {
            LastCompletedCommand = command;
        }

        public SongUpdate SongDataToReturn { get; set; }
        public JukeCommand LastCompletedCommand { get; private set; }
    }
}
