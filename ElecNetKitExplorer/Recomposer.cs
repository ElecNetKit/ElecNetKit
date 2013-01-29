using ElecNetKit.Composition;
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
#pragma warning disable 0649
    class RecomposerSimulator
    {
        [ImportMany(typeof(IFactory<ISimulator>))]
        public IEnumerable<Lazy<IFactory<ISimulator>>> Values;
    }

    class RecomposerExperimentor
    {
        [ImportMany(typeof(IFactory<IExperimentor>))]
        public IEnumerable<Lazy<IFactory<IExperimentor>>> Values;
    }

    class RecomposerResultsTransform
    {
        [ImportMany(typeof(IFactory<IResultsTransform>))]
        public IEnumerable<Lazy<IFactory<IResultsTransform>>> Values;
    }
#pragma warning restore 0649
}
