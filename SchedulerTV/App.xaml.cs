using SchedulerTV.Resources.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace SchedulerTV
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MainViewModel ViewModel { get; private set; }

        public App()
        {
            ViewModel = new MainViewModel();
            ViewModel.Root = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            ViewModel.LoadSearchKey();
            ViewModel.LoadDataTV();
            ViewModel.IsUpdate = false;
        }
    }
}
