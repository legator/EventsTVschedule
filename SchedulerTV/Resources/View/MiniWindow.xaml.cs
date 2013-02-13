using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SchedulerTV.Resources.View
{
    /// <summary>
    /// Interaction logic for MiniWindow.xaml
    /// </summary>
    public partial class MiniWindow : Window
    {
        public MiniWindow()
        {
            InitializeComponent();
        }

        private void AccountConnectClick(object sender, RoutedEventArgs e)
        {

        }

        private void PinApp(object sender, RoutedEventArgs e)
        {

        }

        private void ModeApp(object sender, RoutedEventArgs e)
        {

        }

        private void MinApp(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
