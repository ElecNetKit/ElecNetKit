using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

using ElecNetKit.NetworkModelling;


namespace ElecNetKit.Graphing
{
    /// <summary>
    /// <see cref="INetworkGraph"/>s plot aspects of the information embedded into the 
    /// electrical network model.
    /// </summary>
    public interface INetworkGraph
    {
        /// <summary>
        /// Draws the graph for the <see cref="NetworkModel"/> provided by
        /// <see cref="INetworkGraph.Network"/>.
        /// </summary>
        /// <returns>A <see cref="Visual"/> that contains the completed graph.</returns>
        Visual Draw();

        /// <summary>
        /// An electrical network model to render.
        /// </summary>
        NetworkModel Network { set; get; }

        /// <summary>
        /// The coordinates within which the image is to be displayed.
        /// </summary>
        /// <remarks>The <see cref="INetworkGraph"/> should confine all drawing
        /// to within the coordinates given by <see cref="ImgCoords"/>, shrunk by
        /// the values of the <see cref="Margin"/> property.</remarks>
        Rect ImgCoords { set; get; }

        /// <summary>
        /// A margin around the edge of the graph.
        /// </summary>
        Thickness Margin { set; get; }

    }
}
