using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ElecNetKitExplorer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public ObservableCollection<Package> Packages {private set; get;}

        private String DataPath;

        public void LaunchPackageManager()
        {
            new PackageManagerWindow().Show();
        }

        public static App Instance { get { return (App)App.Current; } }

        private void Application_DispatcherUnhandledException_1(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Serialise();
        }

        private void Deserialise()
        {
            DataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\ElecNetKitExplorer";
            try
            {
                Packages = (ObservableCollection<Package>)QuickSerialisers.Deserialise(DataPath + @"\Packages.bin");
            }
            catch
            {
                Packages = new ObservableCollection<Package>();
            }
        }

        private void Serialise()
        {
            if (!System.IO.Directory.Exists(DataPath))
                System.IO.Directory.CreateDirectory(DataPath);
            Packages.Serialise(DataPath + @"\Packages.bin");
        }

        private void Application_Startup_1(object sender, StartupEventArgs e)
        {
            Deserialise();
        }

        private void Application_Exit_1(object sender, ExitEventArgs e)
        {
            Serialise();
        }
    }
}
