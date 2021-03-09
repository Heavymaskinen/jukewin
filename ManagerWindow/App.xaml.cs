using System.Windows;
using Juke.Control;
using Juke.External.Wmp;
using Juke.IO;
using Juke.Log;

namespace Juke.UI.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            JukeController.Instance.Player.RegisterPlayerEngine(new WmpPlayerEngine());
            LoaderFactory.SetLoaderInstance(new AsyncSongLoader(new FileFinderEngine(), new TaglibTagReaderFactory() {BackupFactory = new WmpTagReaderFactory()}));
            //Logger.Start("juke.log").EnableFrontendLog();
            Messenger.Log("Starting J.U.K.E.");
        }
    }
}
