﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.ComponentModel;

//Necessary for Export Function.
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.IO;
using System.IO.Packaging;

using System.Windows.Media.Imaging;

namespace ElecNetKit.Graphing.Controls
{
    /// <summary>
    /// A control that can display one <see cref="Visual"/>, and handles layout
    /// accordingly.
    /// </summary>
    /// <remarks>
    /// This control should be used for displaying <see cref="Visual"/>s generated by
    /// <see cref="INetworkGraph"/>s in WPF applications.
    /// Adapted from http://msdn.microsoft.com/en-us/library/ms742254.aspx
    /// http://kentb.blogspot.com.au/2008/10/customizing-logical-children.html
    /// http://msdn.microsoft.com/en-us/library/system.windows.media.visual.removevisualchild.aspx
    /// http://www.codeproject.com/Articles/34741/Change-Notification-for-Dependency-Properties
    /// </remarks>
    public class VisualHost : FrameworkElement, INotifyPropertyChanged
    {
        /// <summary>
        /// Instantiates a new <see cref="VisualHost"/> control.
        /// </summary>
        public VisualHost()
        {
       
        }
        
        /// <summary>
        /// Counts the number of children owned by the host.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get { return Drawing == null ? 0 : 1; }
        }

        /// <summary>
        /// Provide a required override for the <see cref="GetVisualChild"/> method.
        /// </summary>
        /// <param name="index">The index of child to return.</param>
        /// <returns>The appropriate child.</returns>
        protected override Visual GetVisualChild(int index)
        {
            if (index == 0)
            {
                return Drawing;
            }

            throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// DependencyProperty backing the only child <see cref="Drawing"/> of the <see cref="VisualHost"/>.
        /// </summary>
        public static readonly DependencyProperty DrawingProperty = DependencyProperty.Register("Drawing", typeof(Visual), typeof(VisualHost),
            new FrameworkPropertyMetadata(null,FrameworkPropertyMetadataOptions.AffectsRender,new PropertyChangedCallback(OnDrawingChanged)));

        private static void OnDrawingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                  ((VisualHost)d).RemoveVisualChild((Visual)e.OldValue);

            ((VisualHost)d).AddVisualChild((Visual)e.NewValue);
            ((VisualHost)d).UpdateLayout();
        }

        /// <summary>
        /// The Drawing that the <see cref="VisualHost"/> should render.
        /// </summary>
        public Visual Drawing
        {
            set
            {
                SetValue(DrawingProperty, value);
            }
            get
            {
                return (Visual)GetValue(DrawingProperty);
            }
           
        }

        /// <summary>
        /// Export the currently displayed content to an xps (vector format) file.
        /// </summary>
        /// <param name="path">The .xps file to export to.</param>
        /// <remarks>
        /// Adapted from http://denisvuyka.wordpress.com/2007/12/03/wpf-diagramming-saving-you-canvas-to-image-xps-document-or-raw-xaml/.
        /// </remarks>
        public void ExportXPS(Uri path)
        {
            if (path == null) return;

            // Save current canvas transorm
            System.Windows.Media.Transform transform = this.LayoutTransform;
            Thickness oldMargin = this.Margin;
            // Temporarily reset the layout transform before saving
            this.LayoutTransform = null;
            this.Margin = new Thickness(0);

            // Get the size of canvas
            double w = this.Width.CompareTo(double.NaN) == 0 ? this.ActualWidth : this.Width;
            double h = this.Height.CompareTo(double.NaN) == 0 ? this.ActualHeight : this.Height;
            Size size = new Size(w, h);
            // Measure and arrange elements
            this.Measure(size);
            this.Arrange(new Rect(size));

            // Open new package
            Package package = Package.Open(path.LocalPath, FileMode.Create);
            // Create new xps document based on the package opened
            XpsDocument doc = new XpsDocument(package);
            // Create an instance of XpsDocumentWriter for the document
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);
            // Write the canvas (as Visual) to the document
            writer.Write(this);
            // Close document
            doc.Close();
            // Close package
            package.Close();

            // Restore previously saved layout
            this.Margin = oldMargin;
            this.LayoutTransform = transform;
        }

        /// <summary>
        /// Export the currently displayed content to a PNG file.
        /// </summary>
        /// <param name="path">The path to export to</param>
        /// <param name="dpi">The dpi of the png, defaults to 96.</param>
        /// <remarks>Adapted from http://msdn.microsoft.com/en-us/library/aa969819.aspx.
        /// </remarks>
        public void ExportPNG(String path, int dpi=96)
        {
            // The BitmapSource that is rendered with a Visual.
            var source = PresentationSource.FromVisual(Drawing);
            Matrix transformToDevice = source.CompositionTarget.TransformToDevice;
            double sizeScaleFactor = (double)dpi / 96.0;
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)(this.ActualWidth*sizeScaleFactor), (int)(this.ActualHeight*sizeScaleFactor), (double)dpi, (double)dpi, PixelFormats.Pbgra32);
            rtb.Render(Drawing);

            // Encoding the RenderBitmapTarget as a PNG file.
            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(rtb));
            using (Stream stm = File.Create(path))
            {
                png.Save(stm);
            }
        }

        /// <summary>
        /// Notifies when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Triggers the <see cref="PropertyChanged"/> event for the specified property name.
        /// </summary>
        /// <param name="name">The name of the property that has changed.</param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}