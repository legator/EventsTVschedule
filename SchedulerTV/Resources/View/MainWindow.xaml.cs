using ItemClass;
using SchedulerTV.Resources.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SchedulerTV
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IntPtr handle;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = App.ViewModel;

            if (SchedulerTV.Properties.Settings.Default.Logged)
            {
                App.ViewModel.Login();
            }
        }

        private void WindowSourceInit(object sender, EventArgs e)
        {
            handle = new WindowInteropHelper(this).Handle;
        }

        private void WindowMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.GetPosition(this).Y < 38)
            {
                DragMove();
            }
        }

        private void ResizeGripMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SizeWindow(handle);
            }
        }

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, UInt32 wParam, IntPtr lParam);

        public static void SizeWindow(IntPtr handle)
        {
            SendMessage(handle, 274, 61448, IntPtr.Zero);
        }

        #region Top buttons click event
        private void SettingsApp(object sender, RoutedEventArgs e)
        {
            
        }

        void NotificationViewClosed(object sender, EventArgs e)
        {
            ((Notification)sender).Closed -= NotificationViewClosed;
            CloseSettings();
        }

        void CloseSettings()
        {
            GridHost.Children.Clear();
            GridHost.Visibility = Visibility.Collapsed;
        }

        private void AccountConnectClick(object sender, RoutedEventArgs e)
        {
            if (App.ViewModel.IsLogged)
            {
                MessageBox.Show(App.ViewModel.service.Name);
                App.ViewModel.IsLogged = false;
                App.ViewModel.service = null;
            }
            else App.ViewModel.Login();
        }
        
        private void SearchClick(object sender, RoutedEventArgs e)
        {
            App.ViewModel.IsUpdate = false;
            App.ViewModel.SearchActionTVByKeyWords();
        }

        private void PinApp(object sender, RoutedEventArgs e)
        {
            App.ViewModel.IsTopMost = Topmost = !Topmost;
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
        #endregion

        #region add/del key words
        private void add_key(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(KeyBox.Text) || string.IsNullOrEmpty(CategoryComboBox.Text))
            {
                MessageBox.Show("put all data");
            }
            else
            {
                SchedulerTV.Resources.Class.Key k = new SchedulerTV.Resources.Class.Key();
                k.Category = CategoryComboBox.Text;
                k.KeyName = KeyBox.Text;
                try
                {
                    App.ViewModel.SaveSearchKey(k);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void DeleteKeyWord(object sender, RoutedEventArgs e)
        {
            App.ViewModel.SearchKey.RemoveAt(Key.SelectedIndex);
            App.ViewModel.DeleteSearchKey();
        }
        #endregion

        private void ChangeChannelcategory(object sender, SelectionChangedEventArgs e)
        {
            App.ViewModel.LoadChannelByCategory(ChComboBox.SelectedValue.ToString());
        }

        private void OpenActionTV(object sender, MouseButtonEventArgs e)
        {
            App.ViewModel.LoadActionTVByChannel(ItemComboBox.SelectedValue.ToString());
        }

        private void AddEvent(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show(ActionList.SelectedIndex.ToString());
            ProgramItem pi = new ProgramItem();
            pi = (ProgramItem)ActionList.SelectedItem;
            App.ViewModel.SelectedEventTV = pi;
        }

        private void AddEvent(object sender, RoutedEventArgs e)
        {
            if (GridHost.Visibility == Visibility.Collapsed)
            {
                GridHost.Visibility = Visibility.Visible;
                var NotificationView = new Notification();
                NotificationView.Closed += NotificationViewClosed;
                GridHost.Children.Add(NotificationView);
            }
            else
            {
                CloseSettings();
            }
        }

        private void AddSearchEvent(object sender, RoutedEventArgs e)
        {
            ProgramItem pi = new ProgramItem();
            pi = (ProgramItem)Search.SelectedItem;
            App.ViewModel.SelectedEventTV = pi;
        }
    }
}
