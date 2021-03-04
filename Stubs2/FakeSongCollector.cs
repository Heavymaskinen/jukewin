using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Juke.IO;

namespace Stubs2
{
    public class FakeSongCollector : ISongCollector
    {
        public Task Load(List<string> files, LoadListener listener1)
        {
            return Task.Run(() =>
            {
                listener1.NotifyProgress(Songs.Count);
                listener1.NotifyCompleted(Songs);
            });
        }
        

        public FakeSongCollector(List<Song> songs)
        {
            Songs = songs;
        }

        public List<Song> Songs { get; set; }
    }
}
