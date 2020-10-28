using DataModel;
using Juke.Control;
using Juke.Core;
using Juke.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Juke.External.Wmp
{
    public class FileFinderEngine : LoadEngine
    {
       
        private TagReaderFactory tagReaderFactory;

        private FileCollector fileCollector;
        private SongCollector songCollector;

        public FileFinderEngine(TagReaderFactory tagReaderFactory)
        {
            this.tagReaderFactory = tagReaderFactory;
            fileCollector = new FileCollector(new WindowsFolderBrowserFactory());
            
        }

        public void Load(string path, LoadListener listener)
        {
            songCollector = new SongCollector(listener, tagReaderFactory);
            listener.NotifyNewLoad2();
            Console.WriteLine("Now count!");
            Messenger.PostMessage("Counting files, please be patient", Messenger.TargetType.Frontend);
            var list = fileCollector.CollectFileNamesSync(path);
            Console.WriteLine("Now add!");
            Messenger.PostMessage("Now adding " + list.Count + " files to library. Please be patient.", Messenger.TargetType.Frontend);
            songCollector.Load(list);
        }

        public async Task LoadAsync(string path, LoadListener listener)
        {
            await Task.Run(() => Load(path, listener));
        }

    }
}
