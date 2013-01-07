using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ElecNetKit.NetworkModelling;

namespace ElecNetKit.Graphing
{
    /// <summary>
    /// Indicates that the graphical output of an <see cref="INetworkGraph"/> will
    /// vary based upon a specific selected <see cref="NetworkElement"/>.
    /// </summary>
    /// <example>
    /// For an example, see the implementation of <see cref="ElecNetKit.Graphing.Graphs.FeederProfileGraph"/>
    /// </example>
    public interface IElementSelectable
    {
        /// <summary>
        /// The <see cref="NetworkElement"/> that will be used to vary, modify or focus the graph.
        /// </summary>
        NetworkElement SelectedElement { set; get; }
    }
}
