using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ElecNetKit.NetworkModelling;

namespace ElecNetKit.Graphing
{
    /// <summary>
    /// A class that provides basic layout management functions for a top-down
    /// geographic network map. Provides functions for scaling from 'Network Coordinates'
    /// to 'Image Coordinates' and vice-versa.
    /// </summary>
    public abstract class TreeGraph : INetworkGraph, IElementLocatable, IPresentationMode
    {
        /// <summary>
        /// Instantiates a new <see cref="TreeGraph"/>, with a default image
        /// size of <c>900x700</c> and a default margin of <c>20</c>.
        /// </summary>
        public TreeGraph()
        {
            ImgCoords = new Rect(0, 0, 900, 700);
            Margin = new Thickness(20);
        }

        /// <summary>
        /// Scales a <see cref="Point"/> from Network Coordinates to Image Coordinates.
        /// </summary>
        /// <param name="location">The point to scale, in Network Coordinates.</param>
        /// <returns>The point, in Image Coordinates.</returns>
        public Point ScaledLocation(Point location)
        {
            Rect from = Network.NetworkBounds;
            Rect to = new Rect(ImgCoords.X + Margin.Left,
                ImgCoords.Y + Margin.Top,
                ImgCoords.Width - Margin.Left - Margin.Right,
                ImgCoords.Height - Margin.Top - Margin.Bottom);


            double x, y;
            double ratio = Math.Min(to.Width / from.Width,
                to.Height / from.Height);
            x = (location.X - from.Left) * ratio + to.Left;
            y = (location.Y - from.Top) * ratio + to.Top;
            return new Point(x, y);
        }

        /// <summary>
        /// Scales a rectangle from Network Coordinates to Image Coordinates.
        /// </summary>
        /// <param name="NetworkCoords">A rectangle, in Network Coordinates.</param>
        /// <returns>The same rectange, in Image Coordinates.</returns>
        public Rect ScaledRectangle(Rect NetworkCoords)
        {
            return new Rect(ScaledLocation(NetworkCoords.TopLeft),
                 ScaledLocation(NetworkCoords.BottomRight));
        }

        /// <summary>
        /// Unscales a rectangle from Image Coordinates to Network Coordinates.
        /// </summary>
        /// <param name="ImgCoords">A rectangle, in Image Coordinates.</param>
        /// <returns>The same rectange, in Network Coordinates.</returns>
        public Rect UnscaledRectangle(Rect ImgCoords)
        {
            return new Rect(UnscaledLocation(ImgCoords.TopLeft),
                UnscaledLocation(ImgCoords.BottomRight));
        }

        /// <summary>
        /// Unscales a <see cref="Point"/> from Image Coordinates to Network Coordinates.
        /// </summary>
        /// <param name="location">The point to unscale, in Image Coordinates.</param>
        /// <returns>The same point, in Network Coordinates.</returns>
        public Point UnscaledLocation(Point location)
        {
            Rect from = new Rect(ImgCoords.X + Margin.Left,
                ImgCoords.Y + Margin.Top,
                ImgCoords.Width - Margin.Left - Margin.Right,
                ImgCoords.Height - Margin.Top - Margin.Bottom);

            Rect to = Network.NetworkBounds;

            double x, y;
            double ratio = Math.Max(to.Width / from.Width,
                to.Height / from.Height);
            x = (location.X - from.Left) * ratio + to.Left;
            y = (location.Y - from.Top) * ratio + to.Top;
            return new Point(x, y);
        }

        /// <summary>
        /// The electrical network model that the <see cref="TreeGraph"/> is to
        /// draw with the <see cref="Draw"/> function.
        /// </summary>
        public NetworkModel Network { get; set; }

        /// <inheritdoc />
        public Rect ImgCoords { get; set; }

        /// <inheritdoc />
        public Thickness Margin { get; set; }


        /// <inheritdoc />
        public virtual NetworkElement GetObjectAtLocation(Point Location)
        {
            Point LocationInNetworkCoords = UnscaledLocation(Location);
            double minDist = double.PositiveInfinity;
            Bus closestBus = null;
            foreach (Bus b in Network.Buses.Values.Where(bus=>bus.Location.HasValue))
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
        public abstract System.Windows.Media.Visual Draw();

        /// <inheritdoc />
        public bool PresentationMode { get; set; }
    }
}
