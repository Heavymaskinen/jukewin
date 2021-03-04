using System.Collections;
using System.Collections.Generic;
using Juke.Control;
using Juke.Core;
using Juke.IO;
using System.Threading.Tasks;
using TagLib.Riff;

namespace Juke.External.Wmp
{
    public class FileFinderEngine : LoadEngine
    {
        private FileCollector fileCollector;

        public FileFinderEngine()
        {
            fileCollector = new FileCollector(new WindowsFolderBrowserFactory());
        }

        public List<string> Load(string path, LoadListener listener)
        {
            listener.NotifyNewLoad();
            Messenger.PostMessage("Counting files, please be patient", Messenger.TargetType.Frontend);
            var list = fileCollector.CollectFileNamesSync(path);
            Messenger.PostMessage("Now adding " + list.Count + " files to library. Please be patient.", Messenger.TargetType.Frontend);
            return list;
        }

        public async Task<List<string>> LoadAsync(string path, LoadListener listener)
        {
            return await Task.Run<List<string>>(() => Load(path, listener));
        }
    }
}
