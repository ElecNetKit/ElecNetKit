using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace ElecNetKit.NetworkModelling
{
    /// <summary>
    /// A line connecting two buses in the electrical network model.
    /// </summary>
    [Serializable]
    public class Line : NetworkElement
    {
        /// <summary>
        /// The length of the line.
        /// </summary>
        public double Length { set; get; }
        /// <summary>
        /// Instantiates a new <see cref="Line"/>.
        /// </summary>
        /// <param name="ID">The ID of the line. Must be unique among lines,
        /// but not necessarily among all <see cref="NetworkElement"/>s.</param>
        /// <param name="Length">The length of the line.</param>
        public Line(String ID, double Length)
        {
            this.ID = ID;
            this.Length = Length;
        }
    }
}
