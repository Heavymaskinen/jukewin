using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Juke.UI.Wpf
{
    class WindowRouter
    {
        private Window mainWindow;

        public WindowRouter(Window mainWindow)
        {
            this.mainWindow = mainWindow;
            mainWindow.ShowActivated = true;
        }

        public void ShowDialog(Window window)
        {
            mainWindow.Visibility = Visibility.Hidden;
            window.ShowDialog();
            mainWindow.Visibility = Visibility.Visible;
            mainWindow.Focus();
        }

        public void ChangeScreen(Window window)
        {
            mainWindow.Visibility = Visibility.Hidden;
            mainWindow = window;
            mainWindow.Visibility = Visibility.Visible;
            mainWindow.Focus();
        }
    }
}
