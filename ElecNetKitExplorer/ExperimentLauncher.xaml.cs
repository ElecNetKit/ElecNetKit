using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ElecNetKitExplorer
{
    /// <summary>
    /// Interaction logic for ExperimentLauncher.xaml
    /// </summary>
    public partial class ExperimentLauncher : MetroWindow
    {
        public ExperimentLauncher()
        {
            InitializeComponent();
        }

        private void toolbarManagePackages_Click_1(object sender, RoutedEventArgs e)
        {
            ((App)App.Current).LaunchPackageManager();
        }
    }
}
