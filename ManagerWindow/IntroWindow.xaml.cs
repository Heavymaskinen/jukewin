using DataModel;
using Juke.External.Wmp;
using Juke.IO;
using Juke.UI.Command;
using System;
using System.Windows;
using System.Windows.Forms;

namespace Juke.UI.Wpf
{
    /// <summary>
    /// Interaction logic for IntroWindow.xaml
    /// </summary>
    public partial class IntroWindow : Window, ViewControl
    {
        public IntroWindow(JukeViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.View = this;
            LoaderFactory.SetLoaderInstance(new IO.AsyncSongLoader(new FileFinderEngine(new TaglibTagReaderFactory())));
            AsyncSongLoader.LoadCompleted += AsyncSongLoader_LoadCompleted;
        }

        private void AsyncSongLoader_LoadCompleted(object sender, System.Collections.Generic.IList<Song> e)
        {
            Dispatcher.Invoke(() => Close());
        }

        public IntroWindow()
        {
            InitializeComponent();
        }


        public string PromptPath()
        {
            var dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                openButton.Visibility = Visibility.Hidden;
                openLabel.Visibility = Visibility.Hidden;
                return dlg.SelectedPath;
            }

            return null;
        }

        public void CommandCompleted(JukeCommand command)
        {
            Dispatcher.Invoke(() => Close());
        }

        public SongUpdate PromptSongData(JukeViewModel.InfoType infoType)
        {
            throw new NotImplementedException();
        }
    }
}
