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
    public class FileCollector
    {
        private readonly IFolderBrowserFactory folderBrowserFactory;
        private List<string> files;
        private bool highFlag;
        private bool midFlag;
        private bool lowFlag;

        public FileCollector(IFolderBrowserFactory folderBrowserFactory)
        {
            this.folderBrowserFactory = folderBrowserFactory;
            files = new List<string>();
        }

        public async Task<List<string>> CollectFileNames(string path)
        {
            files.Clear();
            var browser = folderBrowserFactory.Create(path);
            await LoadFromFolderBrowser(browser);

            return files;
        }

        public List<string> CollectFileNamesSync(string path)
        {
            files.Clear();
            var browser = folderBrowserFactory.Create(path);
            var task = LoadFromFolderBrowser(browser);
            task.Wait();
            return files;
        }

        private async Task LoadFromFolderBrowser(IFolderBrowser browser)
        {

            IList<string> folderFiles = browser.GetFiles();

            if (files.Count >= 20000 && !highFlag)
            {
                Messenger.PostMessage("Over 20000 files!! Don't worry, we'll get there ...", Messenger.TargetType.Frontend);
                highFlag = true;
            }
            else if (files.Count >= 10000 && !midFlag)
            {
                Messenger.PostMessage("Over 10000 files! That's quite a collection ...", Messenger.TargetType.Frontend);
                midFlag = true;
            }
            else if (files.Count >= 5000 && !lowFlag)
            {
                Messenger.PostMessage("Over 5000 files! This could take a while...", Messenger.TargetType.Frontend);
                lowFlag = true;
            }


            lock (files)
            {
                foreach (var file in folderFiles)
                {
                    if (file.Contains("._"))
                    {
                        continue;
                    }

                    files.Add(file);
                }
            }

            var subFolders = browser.GetSubFolders();

            var mid = subFolders.Count / 2;
            var first = subFolders.GetRange(0, mid);
            subFolders.RemoveRange(0, mid);

            Task.WaitAll(
                 RangedLoad(first),
                 RangedLoad(subFolders)
                );

        }

        private async Task RangedLoad(List<IFolderBrowser> browsers)
        {
            foreach (var folder in browsers)
            {
                await LoadFromFolderBrowser(folder);
            }
        }

    }
}
