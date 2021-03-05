using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataModel;
using Juke.IO;

namespace Stubs2
{
    public class FakeSongCollector : ISongCollector
    {
        public Task Load(List<string> files, LoadListener listener1, CancellationToken cancelToken)
        {
            return Task.Run(() =>
            {
                listener1.NotifyProgress(Songs.Count);
                listener1.NotifyCompleted(Songs);
            }, cancelToken);
        }
        

        public FakeSongCollector(List<Song> songs)
        {
            Songs = songs;
        }

        public List<Song> Songs { get; set; }
    }
}
