﻿using DataModel;
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

        public SongUpdate PromptSongData(InfoType info)
        {
            return SongDataToReturn;
        }

        public void CommandCompleted(JukeCommand command)
        {
            Completed = true;
            LastCompletedCommand = command;
        }

        public bool Completed { get; set; }

        public SongUpdate SongDataToReturn { get; set; }
        public JukeCommand LastCompletedCommand { get; private set; }
    }
}