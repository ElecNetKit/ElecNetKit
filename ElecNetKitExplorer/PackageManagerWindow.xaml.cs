using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for PackageManagerWindow.xaml
    /// </summary>
    public partial class PackageManagerWindow : MetroWindow
    {
        public PackageManagerWindow()
        {
            InitializeComponent();
        }

        private void btnAddPackage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Assemblies|*.dll;*.exe|All Files|*.*";
            dlg.Multiselect = true;
            dlg.Title = "Please select one or more assemblies containing ElecNetKit components";
            if (false == dlg.ShowDialog())
                return;
            //else
            foreach (var filename in dlg.FileNames)
                App.Instance.Packages.Add(new Package(filename));
        }
    }
}
