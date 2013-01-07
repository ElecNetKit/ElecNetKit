using System;

using ElecNetKit.NetworkModelling;
using System.Windows;

namespace ElecNetKit.Graphing
{
    /// <summary>
    /// Any <see cref="INetworkGraph"/> that implements
    /// <see cref="IElementLocatable"/> can provide reverse-lookup capabilities,
    /// that is, can find network elements based on a point in the graph.
    /// </summary>
    public interface IElementLocatable
    {
        /// <summary>
        /// Finds the network element at the specified (device-independent) pixel location.
        /// </summary>
        /// <param name="Location">the pixel to find a <see cref="NetworkElement"/> at.</param>
        /// <returns>The <see cref="NetworkElement"/> nearest the pixel.</returns>
        NetworkElement GetObjectAtLocation(Point Location);
    }
}
