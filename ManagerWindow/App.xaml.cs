using System;
using System.IO;
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
            LoaderFactory.SetLoaderInstance(new AsyncSongLoader(new FileFinderEngine(),
                new TaglibTagReaderFactory() {BackupFactory = new WmpTagReaderFactory()}));

            Logger.Start("juke.log");
            //Logger.ConsoleLog();
            Messenger.Log("Starting J.U.K.E.");
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Messenger.Log("Fatal exception: " + e.Exception.Message + " " + e.Exception.StackTrace);
        }
    }
}