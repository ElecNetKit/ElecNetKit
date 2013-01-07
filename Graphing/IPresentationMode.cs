using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElecNetKit.Graphing
{
    /// <summary>
    /// If <see cref="PresentationMode"/> is <c>true</c> when an
    /// <see cref="INetworkGraph"/> is drawing, the <see cref="INetworkGraph"/>
    /// should ensure that the primary information displayed by the graph is
    /// represented geometrically.
    /// </summary>
    /// <remarks>Implement <see cref="IPresentationMode"/> if
    /// your <see cref="INetworkGraph"/> has a lot of colour-based or fiddly
    /// detail, and especially if part of your audience is visually-impaired,
    /// colour-blind, or sitting a long way from the screen.</remarks>
    public interface IPresentationMode
    {
        /// <summary>
        /// If <c>true</c>, specifies that the graph should be drawn in presentation mode.
        /// </summary>
        bool PresentationMode { set; get; }
    }
}
