using ElecNetKit.NetworkModelling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

using ElecNetKit.Convenience;
using ElecNetKit.Util;
using ElecNetKit.Graphing.Util;

using System.Collections.ObjectModel;
using System.Globalization;

namespace ElecNetKit.Graphing.Graphs
{
    /// <summary>
    /// Draws feeder profile graphs - plots of line voltage (pu) against length
    /// from source bus.
    /// </summary>
    /// <remarks>
    /// Only certain lines are displayed, dependant on the <see cref="SelectedElement"/>.
    /// The following lines are displayed:
    /// <list type="bullet">
    /// <item>All lines that are connected between <see cref="SelectedElement"/> and <see cref="NetworkModel.SourceBus"/>, and</item>
    /// <item>All lines from the <see cref="SelectedElement"/> away from the <see cref="NetworkModel.SourceBus"/>.</item>
    /// </list>
    /// This structure allows the <see cref="SelectedElement"/> to be varied along the feeder to obtain different levels of focus.
    /// </remarks>
    public class FeederProfileGraph: IPresentationMode, INetworkGraph, IElementSelectable
    {
        /// <inheritdoc />
        public Visual Draw()
        {
            if (Network == null)
                throw new Exception("Network is Null");

            //Create drawing target
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            //enables clicks to be registered all throughout the drawing (i.e.
            // no holes) but simultaneously remains transparent.
            drawingContext.DrawRectangle(Brushes.Transparent, null, ImgCoords);

            Pen p = new Pen(Brushes.Black,2);
            p.Freeze();

            var busesToSource = Tracing.BusesOnRouteToTarget((Bus)SelectedElement, (Bus)Network.SourceBus);
            var busesFromSelection = Tracing.TraceWithoutCrossingBuses((Bus)SelectedElement, busesToSource);
            var buses = busesToSource.Union(busesFromSelection);

            Dictionary<Bus, double> lengths = new Dictionary<Bus, double>();
            lengths[Network.SourceBus] = 0;
            Collection<Line> lines = new Collection<Line>();

            Tracing.TraceFromWithCallback(Network.SourceBus, buses, (thisBus, line, nextBus) =>
            {
                lengths[nextBus] = (lengths.ContainsKey(thisBus) ? lengths[thisBus] : 0) + line.Length;
                lines.Add(line);
            });

            Limits lengthLimits = new Limits();
            lengthLimits.ProcessData(lengths.Values);
            lengthLimits.LimitMax = ImgCoords.Right - Margin.Right;
            lengthLimits.LimitMin = ImgCoords.Left + Margin.Left;

            Limits voltLimits = new Limits();
            voltLimits.ProcessData(buses.Select(b => -b.VoltagePU.Magnitude));

            //choose grid for volts.
            MagicGridSpacer voltSpacer = new MagicGridSpacer();
            voltSpacer.MinValue = voltLimits.AutoMin;
            voltSpacer.MaxValue = voltLimits.AutoMax;
            voltSpacer.MaxLines = 10; //TODO: change this based on pixels. min(pixelMax, 2)
            var voltSpacings = voltSpacer.GetGridSpacings();
            voltLimits.ProcessData(voltSpacings.LowerLimit);
            voltLimits.ProcessData(voltSpacings.UpperLimit);

            voltLimits.LimitMin = ImgCoords.Top + Margin.Top;
            voltLimits.LimitMax = ImgCoords.Bottom - Margin.Bottom;

            MagicGridSpacer lengthSpacer = new MagicGridSpacer();
            lengthSpacer.MinValue = lengthLimits.AutoMin;
            lengthSpacer.MaxValue = lengthLimits.AutoMax;
            lengthSpacer.MaxLines = 20; //TODO: change this based on pixels.
            var lengthSpacings = lengthSpacer.GetGridSpacings();
            lengthLimits.ProcessData(lengthSpacings.LowerLimit);
            lengthLimits.ProcessData(lengthSpacings.UpperLimit);

            Dictionary<Bus, Point> BusPoints = new Dictionary<Bus, Point>();
            foreach (Bus b in buses)
            {
                BusPoints[b] = new Point(lengthLimits.ValueScaledToLimits(lengths[b]), voltLimits.ValueScaledToLimits(-b.VoltagePU.Magnitude));
            }

            Pen gridPen = new Pen(Brushes.LightGray, 1);
            gridPen.Freeze();

            foreach (double yPos in voltSpacings.GetTicks())
            {
                var y = voltLimits.ValueScaledToLimits(yPos);
                var leftPoint = new Point(ImgCoords.Left + Margin.Left, y);
                drawingContext.DrawLine(gridPen, leftPoint, new Point(ImgCoords.Right - Margin.Right, y));
                var ft = new FormattedText((-yPos).ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(""), 12, Brushes.Black);
                drawingContext.DrawText(ft, leftPoint);
            }

            foreach (double xPos in lengthSpacings.GetTicks())
            {
                var x = lengthLimits.ValueScaledToLimits(xPos);
                var bottomPoint = new Point(x, ImgCoords.Bottom - Margin.Bottom);
                drawingContext.DrawLine(gridPen, bottomPoint, new Point(x,ImgCoords.Bottom - Margin.Bottom - 10));
                var ft = new FormattedText(xPos.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(""), 12, Brushes.Black);
                drawingContext.DrawText(ft, bottomPoint);
            }

            foreach (Line l in lines)
            {
                Bus b1, b2;
                b1 = l.ConnectedTo.OfType<Bus>().First();
                b2 = l.ConnectedTo.OfType<Bus>().Last();

                drawingContext.DrawLine(p, BusPoints[b1], BusPoints[b2]);
            }

            drawingContext.Close();
            return drawingVisual;
        }

        /// <summary>
        /// Instantiates a new <see cref="FeederProfileGraph"/>.
        /// </summary>
        public FeederProfileGraph()
        {
            
        }

        /// <inheritdoc />
        public NetworkModel Network { get; set; }

        /// <inheritdoc />
        public Rect ImgCoords { get; set; }

        /// <inheritdoc />
        public Thickness Margin { get; set; }

        /// <inheritdoc />
        public NetworkElement SelectedElement  { get; set; }

        /// <inheritdoc />
        public bool PresentationMode { get; set; }
    }
}
