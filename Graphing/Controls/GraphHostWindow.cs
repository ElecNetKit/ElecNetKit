using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ElecNetKit.Graphing.Controls
{
    public class GraphHostWindow : System.Windows.Window
    {
        GraphHost _GraphHost;

        public INetworkGraph Graph
        {
            get { return _GraphHost.Graph; }
            set
            {
                _GraphHost.Graph = value;
            }
        }

        public GraphHostWindow()
        {
            _GraphHost = new GraphHost();
            this.Content = _GraphHost;
        }

        private void RefreshGraph()
        {
            _GraphHost.RefreshGraph();
        }

        private static void RunGraphHostWindow(object Graph)
        {
            var window = new GraphHostWindow();
            window.Graph = (INetworkGraph)Graph;
            window.Show();
            //See http://stackoverflow.com/questions/4183622/the-calling-thread-must-be-sta-because-many-ui-components-require-this-in-wpf
            window.Closed += (s, e) => System.Windows.Threading.Dispatcher.ExitAllFrames();
            System.Windows.Threading.Dispatcher.Run();
        }

        public static void StartGraphHostWindow(INetworkGraph Graph) {
            Thread t = new Thread(RunGraphHostWindow);
            t.SetApartmentState(ApartmentState.STA);
            t.Start(Graph);
        }
    }
}
