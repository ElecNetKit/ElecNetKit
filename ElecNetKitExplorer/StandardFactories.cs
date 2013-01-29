using ElecNetKit.Composition;
using ElecNetKit.Engines;
using ElecNetKit.Experimentation;
using ElecNetKit.Simulator;
using ElecNetKit.Transform;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKitExplorer
{
    [Export(typeof(IFactory<IResultsTransform>))]
    public sealed class DifferenceTransformFactory : ResultsTransformFactory<DifferenceTransform> { }

    [Export(typeof(IFactory<IExperimentor>))]
    public sealed class ChainExperimentorFactory : ExperimentorFactory<ChainExperimentor> { }

    [Export(typeof(IFactory<ISimulator>))]
    public sealed class OpenDSSSimulatorFactory : SimulatorFactory<OpenDSSSimulator> { }
}
