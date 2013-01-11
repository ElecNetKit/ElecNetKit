using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace ElecNetKit.NetworkModelling
{
    public interface Phased<T>
    {
        T this[int index] {set; get;}
        IEnumerable<int> Phases { get; }
        bool PhaseExists(int phase);
    }

    [Serializable]
    class PhasedValues<T> : Phased<T>, IDeserializationCallback
    {
        Dictionary<int, T> phases;

        public PhasedValues()
        {
            phases = new Dictionary<int, T>();
        }

        public T this[int pIndex]
        {
            get
            {
                if (!phases.ContainsKey(pIndex))
                    throw new IndexOutOfRangeException();
                return phases[pIndex];
            }
            set
            {
                phases[pIndex] = value;
            }
        }

        public IEnumerable<int> Phases
        {
            get
            {
                return phases.Keys;
            }
        }

        public void OnDeserialization(object sender)
        {
            phases.OnDeserialization(sender);
        }


        public bool PhaseExists(int phase)
        {
            return phases.ContainsKey(phase);
        }
    }

    class PhasedEvaluated<T> : Phased<T>
    {
        Func<int, T> getEvaluator;
        Action<int, T> setEvaluator;
        Func<IEnumerable<int>> phasesEvaluator;

        public PhasedEvaluated(Func<int,T> getEvaluator, Action<int, T> setEvaluator, Func<IEnumerable<int>> phasesEvaluator)
        {
            this.getEvaluator = getEvaluator;
            this.setEvaluator = setEvaluator;
            this.phasesEvaluator = phasesEvaluator;
        }
        public T this[int index]
        {
            get
            {
                return this.getEvaluator(index);
            }
            set
            {
                setEvaluator(index, value);
            }
        }

        public IEnumerable<int> Phases
        {
            get { return phasesEvaluator(); }
        }

        public bool PhaseExists(int phase)
        {
            return Phases.Contains(phase);
        }
    }
}
