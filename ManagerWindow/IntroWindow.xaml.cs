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
    public partial class IntroWindow : ViewControl
    {
        public IntroWindow(JukeViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.View = this;

            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            LoaderFactory.SetLoaderInstance(new AsyncSongLoader(new FileFinderEngine(),
                new TaglibTagReaderFactory() {BackupFactory = new WmpTagReaderFactory()}));
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
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

        public SongUpdate PromptSongData(InfoType infoType)
        {
            throw new NotImplementedException();
        }
    }
}