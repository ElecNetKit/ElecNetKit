using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.ObjectModel;
using ElecNetKit.Convenience;

namespace ElecNetKit.NetworkModelling
{
    /// <summary>
    /// The <see cref="Tracing"/> class defines a set of functions for tracing
    /// the topology of <see cref="NetworkModel"/>s.
    /// </summary>
    public static class Tracing
    {
        /// <summary>
        /// Obtains a list of all buses that are directly on-route from
        /// <paramref name="FromBus"/> to <paramref name="TargetBus"/>.
        /// </summary>
        /// <param name="FromBus">The <see cref="Bus"/> to trace from.</param>
        /// <param name="TargetBus">The <see cref="Bus"/> to trace to.</param>
        /// <returns>A list of all buses that are directly on-route from
        /// <paramref name="FromBus"/> to <paramref name="TargetBus"/> (inclusive).</returns>
        public static IEnumerable<Bus> BusesOnRouteToTarget(Bus FromBus, Bus TargetBus)
        {
            //start at FromBus, take a step away from FromBus.
            //If we're at the source, add the path that we took to BusesOnRouteToSource.
            //If we're at a dead-end, then stop tracing this branch.
            //If we hit a bus that's already on the route, then terminate and add the branch.
            HashSet<Bus> results = new HashSet<Bus>(TargetBus.Yield());
            HashSet<Bus> thisTrace = new HashSet<Bus>(FromBus.Yield());
            TraceToSource(FromBus, FromBus, thisTrace, results);
            results.Add(FromBus);
            return (IEnumerable<Bus>)results;
        }

        /// <summary>
        /// Obtains a list of all buses directly or indirectly connected to <paramref name="FromBus"/>,
        /// so long as they are connected in a fashion that does not rely upon the presence of any elements of
        /// <paramref name="BusesToExclude"/>
        /// </summary>
        /// <param name="FromBus">The <see cref="Bus"/> to trace from.</param>
        /// <param name="BusesToExclude">A set of <see cref="Bus"/>es that should be avoided in the trace operation.</param>
        /// <returns>A list of all buses directly or indirectly connected to <paramref name="FromBus"/>, 
        /// so long as connection is not through any element in <paramref name="BusesToExclude"/>.</returns>
        public static IEnumerable<Bus> TraceWithoutCrossingBuses(Bus FromBus, IEnumerable<Bus> BusesToExclude)
        {
            var BusesInTrace = new HashSet<Bus>(FromBus.Yield());
            var BusesInPath = new HashSet<Bus>();
            _traceWithoutCrossingBuses(FromBus, new HashSet<Bus>(BusesToExclude), BusesInTrace, BusesInPath);
            BusesInPath.Add(FromBus);
            return BusesInPath;
        }

        private static void _traceWithoutCrossingBuses(Bus FromBus, HashSet<Bus> ExcludeList, HashSet<Bus> busesInThisTrace, HashSet<Bus> busesOnPath)
        {
            var connectedBuses = FromBus.ConnectedTo.OfType<Line>().Select(line => line.ConnectedTo.OfType<Bus>().Except(FromBus.Yield()).Single()) //all the buses neighbouring this one
               .Except(busesInThisTrace) //exclude any buses we've already touched in this trace
               ;
            if (connectedBuses.Count() == 0)
            {
                busesOnPath.UnionWith(busesInThisTrace);
                return;
            }
            foreach (var bus in connectedBuses)
            {
                if (ExcludeList.Contains(bus)) //we're connected to the target bus. end the thread.
                {
                    continue;
                }
                else
                {
                    //keep searching!
                    HashSet<Bus> NextStepTraceList;
                    if (connectedBuses.Count() == 1) //if this is the only possible way forward, then just keep using the same thingy.
                    {
                        NextStepTraceList = busesInThisTrace;
                    }
                    else
                    {
                        NextStepTraceList = new HashSet<Bus>(busesInThisTrace);
                    }
                    NextStepTraceList.Add(bus);
                    _traceWithoutCrossingBuses(bus, ExcludeList, NextStepTraceList, busesOnPath);
                }
            }
        }

        private static void TraceToSource(Bus FromBus, Bus BusThatStartedItAll, HashSet<Bus> busesInThisTrace, HashSet<Bus> busesOnPath)
        {
            //start at FromBus, take a step away from FromBus.
            //If we're at the source, add the path that we took to BusesOnRouteToSource.
            //If we're at a dead-end, then stop tracing this branch.
            //If we hit a bus that's already on the route, then terminate and add the branch.
            var connectedBuses = FromBus.ConnectedTo.OfType<Line>().Select(line => line.ConnectedTo.OfType<Bus>().Except(FromBus.Yield()).Single()) //all the buses neighbouring this one
                .Except(busesInThisTrace) //exclude any buses we've already touched in this trace
                ;
            foreach (var bus in connectedBuses)
            {
                if (busesOnPath.Contains(bus)) //we're connected to the target bus. no further processing required on this branch.
                {
                    busesOnPath.UnionWith(busesInThisTrace.Except(BusThatStartedItAll.Yield()));
                    continue;
                }
                else
                {
                    //keep searching!
                    HashSet<Bus> NextStepTraceList;
                    if (connectedBuses.Count() == 1) //if this is the only possible way forward, then just keep using the same thingy.
                    {
                        NextStepTraceList = busesInThisTrace;
                    }
                    else
                    {
                        NextStepTraceList = new HashSet<Bus>(busesInThisTrace);
                    }
                    NextStepTraceList.Add(bus);
                    TraceToSource(bus, BusThatStartedItAll, NextStepTraceList, busesOnPath);
                }
            }

        }

        /// <summary>
        /// Obtains the direct line length between two buses.
        /// </summary>
        /// <param name="FromBus">The bus to trace from.</param>
        /// <param name="ToBus">The bus to find the line length to.</param>
        /// <returns>The line length, in metres.</returns>
        public static double GetDirectLengthBetweenBuses(Bus FromBus, Bus ToBus)
        {
            var busList = Tracing.BusesOnRouteToTarget(FromBus, ToBus);

            Dictionary<Bus, double> lengths = new Dictionary<Bus, double>();
            Tracing.TraceFromWithCallback(ToBus, busList, (thisBus, line, nextBus) => lengths[nextBus] = (lengths.ContainsKey(thisBus) ? lengths[thisBus] : 0) + line.Length);
            return lengths[FromBus];
        }

        /// <summary>
        /// Traces from <paramref name="FromBus"/> along all buses in <paramref name="AllowedBuses"/>,
        /// and executing <paramref name="Callback"/> for every connection between two buses.
        /// </summary>
        /// <param name="FromBus">The bus to trace from.</param>
        /// <param name="AllowedBuses">All buses that are to be allowed in the trace.</param>
        /// <param name="Callback">A custom action to execute for every connection.</param>
        public static void TraceFromWithCallback(Bus FromBus, IEnumerable<Bus> AllowedBuses, Action<Bus, Line, Bus> Callback)
        {
            HashSet<Bus> thisTrace = new HashSet<Bus>(FromBus.Yield());
            _traceFromWithCallback(FromBus, AllowedBuses, thisTrace, Callback);
        }

        private static void _traceFromWithCallback(Bus FromBus, IEnumerable<Bus> AllowedBuses, HashSet<Bus> busesInThisTrace, Action<Bus, Line, Bus> Callback)
        {
            var connectedBuses = FromBus.ConnectedTo.OfType<Line>().Select(line => Tuple.Create(line, line.ConnectedTo.OfType<Bus>().Except(FromBus.Yield()).Single())) //all the buses neighbouring this one
                .Where(t => !busesInThisTrace.Contains(t.Item2) && AllowedBuses.Contains(t.Item2)) //exclude any buses we've already touched in this trace, and any buses that aren't in the allowed set.
                ;
            foreach (var bus in connectedBuses)
            {
                Callback(FromBus, bus.Item1, bus.Item2);

                //keep searching!
                HashSet<Bus> NextStepTraceList = connectedBuses.Count() == 1 ? busesInThisTrace : new HashSet<Bus>(busesInThisTrace);

                NextStepTraceList.Add(bus.Item2);
                _traceFromWithCallback(bus.Item2, AllowedBuses, NextStepTraceList, Callback);
            }

        }
    }
}
