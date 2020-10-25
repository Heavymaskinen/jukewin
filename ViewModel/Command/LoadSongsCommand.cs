using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Juke.Control;
using Juke.IO;

namespace Juke.UI.Command
{
    class LoadSongsCommand : JukeCommand
    {
        public LoadSongsCommand(JukeController controller, ViewControl view, JukeViewModel model) : base(controller, view, model)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        protected override void ControlledExecute(object parameter)
        {
            var path = view.PromptPath();
            if (!string.IsNullOrEmpty(path))
            {
                LoadAsync(path);
            }
        }

        private async void LoadAsync(string path)
        {
            var loader = new LoaderFactory().CreateAsync(path);
            controller.LoadHandler.LoadSongs(loader);
            return;
            await Task.Run(async ()=> {
               

                var done = false;
                while (!done)
                {
                    done = true;
                    for (var i = 0; i < AsyncSongLoader.tasks.Count; i++)
                    {
                        if (AsyncSongLoader.tasks[i].IsCompleted)
                        {
                            lock (AsyncSongLoader.tasks)
                            {
                                AsyncSongLoader.tasks.RemoveAt(i);
                                i--;
                            }
                        } else
                        {
                            done = false;
                        }

                    }

                    Thread.Sleep(2);
                }
               
                
                Console.WriteLine("Command completed! " + AsyncSongLoader.tasks.Count);
                view.CommandCompleted(this);
            });
            
        }
    }
}
