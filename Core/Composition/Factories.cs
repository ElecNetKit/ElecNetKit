using ElecNetKit.Experimentation;
using ElecNetKit.Simulator;
using ElecNetKit.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElecNetKit.Composition
{
    public abstract class GenericFactory<TProvides,TProvidesWith> : IFactory<TProvides>
        where TProvidesWith : TProvides, new()
    {
        String[] _TypeNames;

        String[] GetTypeNames()
        {
            System.Reflection.MemberInfo info = typeof(TProvidesWith);
            var aliases = (AliasAttribute[])Attribute.GetCustomAttributes(typeof(TProvidesWith), typeof(AliasAttribute));
            if (aliases.Count() == 0)
                return new[] { typeof(TProvidesWith).ToString() };
            //else
            return aliases.Select(aliasAttrib => aliasAttrib.Alias).ToArray();
        }

        public TProvides Create()
        {
            return (TProvides)(new TProvidesWith());
        }

        public String[] TypeNames
        {
            get { return _TypeNames; }
        }

        internal GenericFactory()
        {
            _TypeNames = GetTypeNames();
        }
    }

    public interface IFactory<T>
    {
        T Create();
        String[] TypeNames {get;}
    }

    public abstract class ResultsTransformFactory<T> : GenericFactory<IResultsTransform, T>
        where T : IResultsTransform, new()
    {
    }

    public abstract class SimulatorFactory<T> : GenericFactory<ISimulator, T>
        where T : ISimulator, new()
    {
    }

    public abstract class ExperimentorFactory<T> : GenericFactory<IExperimentor, T>
        where T : IExperimentor, new()
    {
    }
}
