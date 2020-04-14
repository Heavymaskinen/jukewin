using DataModel;
using Juke.External.Wmp;
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
            //viewModel.PropertyChanged += ViewModel_PropertyChanged;
            DataContext = viewModel;
            viewModel.View = this;
            LoaderFactory.SetLoaderInstance(new AsyncFileFinder2(""));
        }

        public IntroWindow()
        {
            InitializeComponent();
        }


        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ProgressMax")
            {
                Console.WriteLine("Progress max " + e.PropertyName);
            }
        }

        public string PromptPath()
        {
            var dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
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
