using System.Collections.Generic;
using System.Windows;

namespace Juke.UI.Wpf
{
    /// <summary>
    /// Interaction logic for SearchSelectorWindow.xaml
    /// </summary>
    public partial class SearchSelectorWindow : Window
    {
        public SearchLogic SearchLogic { get; set; }

        public SearchSelectorWindow()
        {
            InitializeComponent();
        }

        public SearchSelectorWindow(IList<SearchLogic> list) : this()
        {
            foreach (var l in list)
            {
                listBox.Items.Add(l);
            }

            listBox.SelectedIndex = 0;
        }

        private void okBUtton_Click(object sender, RoutedEventArgs e)
        {
            SearchLogic = listBox.SelectedItem as SearchLogic;
            DialogResult = true;
            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}