using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ElecNetKit.Sensitivities
{
    /// <summary>
    /// Holds a set of sensitivities between two buses.
    /// </summary>
    [Serializable]
    public class VoltageSensitivityToPQDataSet
    {
        /// <summary>
        /// Change in Voltage (single phase) with respect to change in three-phase Power.
        /// </summary>
        public double dV_dP { set; get; }

        /// <summary>
        /// Change in Voltage (single phase) with respect to change in three-phase Reactive Power.
        /// </summary>
        public double dV_dQ { set; get; }
        
        /// <summary>
        /// Change in Voltage angle (radians) with respect to change in three-phase Power.
        /// </summary>
        public double dd_dP { set; get; }

        /// <summary>
        /// Change in Voltage angle (radians) with respect to change in three-phase Reactive Power.
        /// </summary>
        public double dd_dQ { set; get; }
    }
}
