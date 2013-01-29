using ElecNetKit.Composition;
using ElecNetKit.Experimentation;
using ElecNetKit.Simulator;
using ElecNetKit.Transform;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
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
        
        public ObservableCollection<IFactory<ISimulator>> Simulators { private set; get; }

        public ObservableCollection<IFactory<IExperimentor>> Experimentors { private set; get; }
        
        public ObservableCollection<IFactory<IResultsTransform>> ResultsTransforms { private set; get; }

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
            Simulators = new ObservableCollection<IFactory<ISimulator>>();
            Experimentors = new ObservableCollection<IFactory<IExperimentor>>();
            ResultsTransforms = new ObservableCollection<IFactory<IResultsTransform>>();
            Packages.CollectionChanged += UpdateComponents;
            UpdateComponents(Packages, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void UpdateComponents(object sender, NotifyCollectionChangedEventArgs e)
        {
            CompositionContainer container;
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(App).Assembly));
            foreach (var path in Packages.Where(package => !package.Error).Select(package => package.Path))
            {
                catalog.Catalogs.Add(new AssemblyCatalog(path));
            }
            container = new CompositionContainer(catalog);
            try
            {
                ComposeSimulators(container,Simulators);
                ComposeExperimentors(container,Experimentors);
                ComposeResultsTransform(container,ResultsTransforms);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ComposeSimulators(CompositionContainer container, ICollection<IFactory<ISimulator>> collectionToComposeInto)
        {
            var parts = new RecomposerSimulator();
            container.ComposeParts(parts);
            lock (collectionToComposeInto)
            {
                collectionToComposeInto.Clear();
                foreach (var part in parts.Values.Select(x => x.Value))
                {
                    collectionToComposeInto.Add(part);
                }
            }
        }

        private void ComposeExperimentors(CompositionContainer container, ICollection<IFactory<IExperimentor>> collectionToComposeInto)
        {
            var parts = new RecomposerExperimentor();
            container.ComposeParts(parts);
            lock (collectionToComposeInto)
            {
                collectionToComposeInto.Clear();
                foreach (var part in parts.Values.Select(x => x.Value))
                {
                    collectionToComposeInto.Add(part);
                }
            }
        }

        private void ComposeResultsTransform(CompositionContainer container, ICollection<IFactory<IResultsTransform>> collectionToComposeInto)
        {
            var parts = new RecomposerResultsTransform();
            container.ComposeParts(parts);
            lock (collectionToComposeInto)
            {
                collectionToComposeInto.Clear();
                foreach (var part in parts.Values.Select(x => x.Value))
                {
                    collectionToComposeInto.Add(part);
                }
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
