using Juke.Control;
using Juke.External.Wmp;
using Juke.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WmpComponentUwp;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UwpGui2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private JukeController jukeControl;
        private WmpPlayerEngine engine = new WmpPlayerEngine();
        public MainPage()
        {
            this.InitializeComponent();
            jukeControl = JukeController.Instance;
            
            jukeControl.Player.RegisterPlayerEngine(engine);
            AsyncSongLoader.LoadCompleted += AsyncSongLoader_LoadCompleted;
            AsyncSongLoader.LoadProgress += AsyncSongLoader_LoadProgress;
        }


        private void AsyncSongLoader_LoadProgress(object sender, string e)
        {
            _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { block.Text += "Progress: " + e; });
        }

        private void AsyncSongLoader_LoadInitiated(object sender, EventArgs e)
        {
            _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { block.Text += "LOADING! "; });

        }

        private void AsyncSongLoader_LoadCompleted(object sender, IList<DataModel.Song> e)
        {
            _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (e.Count > 0)
                {
                    block.Text = "Play it: " + e[0].Name;
                    engine.Play(e[0]);
                }
                else
                {
                    block.Text += "No songs?!";
                }

            });



        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var loader = new UwpSongLoader();
            loader.FilesReady += Loader_FilesReady;
            loader.Open();

            block.Text = "pass on by!";

        }

        private void Loader_FilesReady(object sender, string e)
        {
            block.Text = "Load stuff! " + e;
            jukeControl.LoadHandler.LoadSongs(sender as AsyncSongLoader);
        }
    }
}
