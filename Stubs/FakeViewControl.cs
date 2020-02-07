using Juke.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;

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

        public SongUpdate PromptSongData()
        {
            return SongDataToReturn;
        }

        public SongUpdate SongDataToReturn { get; set; }
    }
}
