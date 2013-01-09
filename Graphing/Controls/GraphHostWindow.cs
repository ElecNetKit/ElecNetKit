using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ElecNetKit.Graphing.Controls
{
    /// <summary>
    /// Enables pop-up <see cref="INetworkGraph"/>s to be displayed from a
    /// console or GUI application with minimum fuss.
    /// </summary>
    /// <remarks>When displaying graphs from a console application, you can use
    /// <see cref="StartGraphHostWindow"/> instead of instantiating a new
    /// <see cref="GraphHostWindow"/>.
    /// Alternatively, if you require a reference to a <see cref="GraphHostWindow"/>,
    /// you can mark the application entry point (usually <c>Main()</c>) with
    /// <see cref="System.STAThreadAttribute"/>. See <see href="http://stackoverflow.com/questions/4183622/the-calling-thread-must-be-sta-because-many-ui-components-require-this-in-wpf">The calling thread must be STA</see> for more information.</remarks>
    public class GraphHostWindow : System.Windows.Window
    {
        GraphHost _GraphHost;

        /// <summary>
        /// The <see cref="INetworkGraph"/> to be displayed by the window.
        /// </summary>
        public INetworkGraph Graph
        {
            get { return _GraphHost.Graph; }
            set
            {
                _GraphHost.Graph = value;
            }
        }

        /// <summary>
        /// Instantiates a new <see cref="GraphHostWindow"/>.
        /// </summary>
        public GraphHostWindow()
        {
            _GraphHost = new GraphHost();
            this.Content = _GraphHost;
        }

        /// <summary>
        /// Forces a refresh of the graph displayed in the window.
        /// </summary>
        public void RefreshGraph()
        {
            _GraphHost.RefreshGraph();
        }

        private static void RunGraphHostWindow(object Graph)
        {
            var window = new GraphHostWindow();
            window.Graph = (INetworkGraph)Graph;
            window.Show();
            //These next two lines from http://stackoverflow.com/questions/4183622/the-calling-thread-must-be-sta-because-many-ui-components-require-this-in-wpf
            window.Closed += (s, e) => System.Windows.Threading.Dispatcher.ExitAllFrames();
            System.Windows.Threading.Dispatcher.Run();
        }

        /// <summary>
        /// Starts a <see cref="GraphHostWindow"/> with the specified <paramref name="Graph"/> in a new, Single-Threaded Apartment (STA) thread.
        /// </summary>
        /// <param name="Graph">The graph that should be displayed.</param>
        public static void StartGraphHostWindow(INetworkGraph Graph) {
            Thread t = new Thread(RunGraphHostWindow);
            t.SetApartmentState(ApartmentState.STA);
            t.Start(Graph);
        }
    }
}
