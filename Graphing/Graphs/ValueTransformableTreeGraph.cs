using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media;

using ElecNetKit.NetworkModelling;

using ElecNetKit.Util;
using ElecNetKit.Graphing.AdaptiveGradients;
using System.Windows;

namespace ElecNetKit.Graphing.Graphs
{
    /// <summary>
    /// A network topology graph that automatically manages element positioning
    /// and allows element representation to be controlled through Value
    /// Transforms.
    /// </summary>
    /// <remarks></remarks>
    public class ValueTransformableTreeGraph : TreeGraph
    {
        /// <summary>
        /// Instantiates a new <see cref="ValueTransformableTreeGraph"/>.
        /// </summary>
        public ValueTransformableTreeGraph()
        {
            //sensible defaults.
            BusColorGradient = AdaptiveGradient.BlueRedGradient();
            BusColorTransform = b => b.VoltagePU.Magnitude;
            BusSizeTransform = b => 2;
            BusSizeMax = 2;
            BusSizeMin = 2;
            BusVisibleTransform = b => true;

            RingEnabledTransform = b => b.ConnectedTo.OfType<Generator>().Any();
            RingColor = Colors.Green;
            RingThickness = 2;
            RingDistanceTransform = b => 2;
            RingDistanceFromCenter = false;

            LineThickness = 2;
        }

        private bool ColorConveysMostInformation()
        {
            NormalDist colorDist = new NormalDist(Network.Buses.Values.Where(BusVisibleTransform).Select(BusColorTransform));
            NormalDist sizeDist = new NormalDist(Network.Buses.Values.Where(BusVisibleTransform).Select(BusSizeTransform));
            return colorDist.StdDev >= sizeDist.StdDev;
        }

        /// <summary>
        /// An adaptive gradient that determines, in conjunction with
        /// <see cref="BusColorTransform"/>, how the buses on the network
        /// should be coloured.
        /// </summary>
        public AdaptiveGradient BusColorGradient { set; get; }

        /// <summary>
        /// A function that transforms <see cref="Bus"/>es into values that
        /// are passed to <see cref="BusColorGradient"/> to determine the
        /// colour of buses displayed on the network graph.
        /// </summary>
        public Func<Bus, double> BusColorTransform { set; get; }

        /// <summary>
        /// A function that should return <c>true</c> if a supplied
        /// <see cref="Bus"/> should be drawn.
        /// </summary>
        public Func<Bus, bool> BusVisibleTransform { set; get; }

        /// <summary>
        /// A function that controls the size of each <see cref="Bus"/>.
        /// </summary>
        public Func<Bus, double> BusSizeTransform { set; get; }

        /// <summary>
        /// The maximum allowable bus size, in pixel radius.
        /// </summary>
        public double BusSizeMax { set; get; }

        /// <summary>
        /// The minimum allowable bus size, in pixel radius.
        /// </summary>
        public double BusSizeMin { set; get; }

        /// <summary>
        /// A function that should return <c>true</c> if the bus should have
        /// a ring drawn around it.
        /// </summary>
        public Func<Bus, bool> RingEnabledTransform { set; get; }

        /// <summary>
        /// The color of the ring. Constant across all buses.
        /// </summary>
        public Color RingColor { set; get; }

        /// <summary>
        /// The thickness of the ring (in pixels). Constant across all buses.
        /// </summary>
        public double RingThickness { set; get; }

        /// <summary>
        /// Should be set to <c>true</c> if <see cref="RingDistanceTransform"/>
        /// measures the distance of the ring from the centre of the bus.
        /// Otherwise, <see cref="RingDistanceTransform"/> is interpreted as the
        /// distance between the edge of the bus circle and the ring.
        /// </summary>
        public bool RingDistanceFromCenter { set; get; }

        /// <summary>
        /// Should return values that specify the distance that the ring is to
        /// be drawn from either the centre of the bus or the edge of the bus,
        /// as controlled by <see cref="RingDistanceFromCenter"/>.
        /// </summary>
        public Func<Bus, double> RingDistanceTransform { set; get; }

        /// <summary>
        /// The thickness of <see cref="Line"/>s to be drawn on the graph.
        /// </summary>
        public double LineThickness { set; get; }

        //Ensures that default values are in place if otherwise parameters
        // would be null.
        private void ProtectInitialise()
        {
            if (BusColorGradient == null)
                BusColorGradient = AdaptiveGradient.BlueRedGradient();
        }

        /// <inheritdoc />
        public override NetworkElement GetObjectAtLocation(Point Location)
        {
            Point LocationInNetworkCoords = UnscaledLocation(Location);
            double minDist = double.PositiveInfinity;
            Bus closestBus = null;
            foreach (Bus b in Network.Buses.Values.Where(bus => bus.Location.HasValue && BusVisibleTransform(bus)))
            {
                double thisDist = (b.Location.Value - LocationInNetworkCoords).LengthSquared;
                if (thisDist < minDist)
                {
                    minDist = thisDist;
                    closestBus = b;
                }
            }
            return closestBus;
        }

        /// <inheritdoc />
        public override Visual Draw()
        {
            if (Network == null)
                throw new Exception("Network is Null");

            if (PresentationMode && ColorConveysMostInformation())
            {
                ValueTransformableTreeGraph newGraph = GetAccessibleGraph();
                return newGraph.Draw();
            }

            ProtectInitialise();
            Limits busSizeLimits = GetBusSizeLimits();

            AdaptiveGradientMap<Tuple<Brush, Pen>> colorMap = BuildGradientMap(Network);

            Pen ringColorPen = GetRingPen();

            //Create drawing target
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            //enables clicks to be registered all throughout the drawing (i.e.
            // no holes) but simultaneously remains transparent.
            drawingContext.DrawRectangle(Brushes.Transparent, null, ImgCoords);

            DrawLines(Network, colorMap, drawingContext);

            DrawBuses(Network, colorMap, busSizeLimits, ringColorPen, drawingContext);

            //free up resources
            drawingContext.Close();
            return drawingVisual;
        }

        // gets a PresentationMode-compliant graph.
        private ValueTransformableTreeGraph GetAccessibleGraph()
        {
            ValueTransformableTreeGraph newGraph = (ValueTransformableTreeGraph)this.MemberwiseClone();
            newGraph.BusSizeTransform = newGraph.BusColorTransform;
            newGraph.BusSizeMin = newGraph.LineThickness + 1;
            newGraph.BusSizeMax = newGraph.LineThickness + 10;
            newGraph.PresentationMode = false;
            return newGraph;
        }

        private Limits GetBusSizeLimits()
        {
            Limits busSizeLimits = new Limits();
            busSizeLimits.LimitMax = BusSizeMax;
            busSizeLimits.LimitMin = BusSizeMin;
            foreach (var b in Network.Buses.Values)
                busSizeLimits.ProcessData(BusSizeTransform(b));
            return busSizeLimits;
        }

        /// <summary>
        /// Draws all the network buses, using the specified value transforms.
        /// </summary>
        /// <param name="Network">The network from which to draw buses.</param>
        /// <param name="colorMap">A map of the colours to use from the specified gradient.</param>
        /// <param name="busSizeLimits">The limits of <see cref="BusSizeTransform"/>,
        /// used for scaling from that space to
        /// [<see cref="BusSizeMin"/>, <see cref="BusSizeMax"/>].</param>
        /// <param name="ringColorPen">A pen for drawing rings.</param>
        /// <param name="drawingContext">The target that we should draw to.</param>
        protected void DrawBuses(NetworkModel Network, AdaptiveGradientMap<Tuple<Brush, Pen>> colorMap, Limits busSizeLimits, Pen ringColorPen, DrawingContext drawingContext)
        {
            //Draw all the buses.
            foreach (var bus in Network.Buses.Values)
            {
                if (!(BusVisibleTransform(bus) && bus.Location.HasValue))
                    continue;
                var bSize = busSizeLimits.ValueScaledToLimits(BusSizeTransform(bus));
                //value-dependant fill, no outline, centre is scaled, radius of 2 (small dots).
                drawingContext.DrawEllipse(
                    colorMap.Map(BusColorTransform(bus)).Item1,
                    null, ScaledLocation(bus.Location.Value), bSize, bSize);

                //Bus Ring
                if (RingEnabledTransform(bus))
                {
                    var ringRadius = RingDistanceFromCenter ?
                        /*true*/ RingDistanceTransform(bus) :
                        /*false*/ bSize + RingDistanceTransform(bus);

                    drawingContext.DrawEllipse(null, ringColorPen,
                        ScaledLocation(bus.Location.Value), ringRadius, ringRadius);
                }
            }
        }

        /// <summary>
        /// Draws all the network lines.
        /// </summary>
        /// <param name="Network">The network from which to draw buses.</param>
        /// <param name="colorMap">A map of the colours to use from the specified gradient.</param>
        /// <param name="drawingContext">The target that we should draw to.</param>
        protected void DrawLines(NetworkModel Network, AdaptiveGradientMap<Tuple<Brush, Pen>> colorMap, DrawingContext drawingContext)
        {
            //Draw all the lines.
            foreach (Line line in Network.Lines)
            {
                if (line.ConnectedTo.Count() != 2)
                    continue;
                Bus bus1 = (Bus)line.ConnectedTo.ElementAt(0);
                Bus bus2 = (Bus)line.ConnectedTo.ElementAt(1);

                if (!(bus1.Location.HasValue && bus2.Location.HasValue))
                    continue;

                //color based on average value.
                var bCol1 = BusVisibleTransform(bus1) ? BusColorTransform(bus1) : 0;
                var bCol2 = BusVisibleTransform(bus2) ? BusColorTransform(bus2) : 0;
                
                double v_avg = (bCol1 + bCol2) / 2;
                drawingContext.DrawLine(
                    colorMap.Map(v_avg).Item2, //choose the right color for the value
                    ScaledLocation(bus1.Location.Value), //scale the locations to image coordinates.
                    ScaledLocation(bus2.Location.Value)
                    );
            }
        }

        //Builds a pen from the ring color.
        private Pen GetRingPen()
        {
            Brush ringColorBrush = new SolidColorBrush(RingColor);
            ringColorBrush.Freeze();
            Pen ringColorPen = new Pen(ringColorBrush, RingThickness);
            ringColorPen.Freeze();
            return ringColorPen;
        }

        /// <summary>
        /// Builds a gradient map based upon <see cref="BusColorGradient"/> and
        /// <see cref="BusColorTransform"/> for the data in this network.
        /// </summary>
        /// <param name="Network">The network that data should be taken from.</param>
        /// <returns>A map of gradients.</returns>
        protected AdaptiveGradientMap<Tuple<Brush, Pen>> BuildGradientMap(NetworkModel Network)
        {
            //1. Setup adaptive gradient.
            BusColorGradient.ResetAutoData();
            foreach (var b in Network.Buses.Values)
                BusColorGradient.ProcessData(BusColorTransform(b));

            AdaptiveGradientMapBuilder mapBuilder = new AdaptiveGradientMapBuilder(BusColorGradient);

            AdaptiveGradientMap<Tuple<Brush, Pen>> colorMap = mapBuilder.BuildGradientMap(c =>
            {
                Brush b = new SolidColorBrush(c);
                b.Freeze();
                Pen p = new Pen(b, LineThickness);
                p.Freeze();
                return new Tuple<Brush, Pen>(b, p);
            });
            return colorMap;
        }


    }
}
