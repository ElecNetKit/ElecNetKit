using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.ComponentModel;

//Necessary for Export Function.
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.IO;
using System.IO.Packaging;

using System.Windows.Media.Imaging;

namespace ElecNetKit.Graphing.Controls
{
    /// <summary>
    /// A WPF control that can display one <see cref="INetworkGraph"/>, and handles layout,
    /// redrawing etc. Simply assign an <see cref="INetworkGraph"/> to the <see cref="Graph"/>
    /// property and you're good to go.
    /// </summary>
    /// <remarks>
    /// Note that <see cref="ElecNetKit.NetworkModelling.NetworkModel"/>s don't notify
    /// of changes, and <see cref="INetworkGraph"/>s don't notify of changes to their parameters
    /// either. If you set the <see cref="INetworkGraph.Network"/> property, you can force a 
    /// redraw of the graph with the <see cref="RefreshGraph"/> method.</remarks>
    public class GraphHost : VisualHost, INotifyPropertyChanged
    {
        INetworkGraph _Graph;

        /// <summary>
        /// The <see cref="INetworkGraph"/> that should be drawn by the control.
        /// </summary>
        public INetworkGraph Graph
        {
            set
            {
                _Graph = value;
                _Graph.ImgCoords = new Rect(0, 0, this.Width, this.Height);
                RefreshGraph();
            }
            get
            {
                return _Graph;
            }
        }

        /// <summary>
        /// Force a redraw. Useful after setting a parameter of the <see cref="INetworkGraph"/>
        /// assigned in the <see cref="Graph"/> property, or modifying the
        /// <see cref="INetworkGraph.Network"/>.
        /// </summary>
        public void RefreshGraph()
        {
            if (_Graph != null)
                Drawing = _Graph.Draw();
        }

        /// <summary>
        /// Instantiates a new <see cref="GraphHost"/> control.
        /// </summary>
        public GraphHost()
        {
            this.SizeChanged += GraphHost_SizeChanged;
        }

        void GraphHost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_Graph != null)
                _Graph.ImgCoords = new Rect(e.NewSize);
            RefreshGraph();
        }
    }
}
