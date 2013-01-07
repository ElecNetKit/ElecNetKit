using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media;

using System.Windows;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.IO;
using System.IO.Packaging;
using System.Windows.Media.Imaging;


namespace ElecNetKit.Graphing.Util
{
    /// <summary>
    /// Provides some convenience functions for serialising <see cref="Visual"/>s.
    /// </summary>
    public static class DrawingVisualSerialisers
    {
        /// <summary>
        /// Store a visual in an array of bytes.
        /// </summary>
        /// <param name="v">The <see cref="Visual"/> to serialise.</param>
        /// <returns>The serialised <see cref="Visual"/>.</returns>
        public static byte[] SaveVisualToByteArray(this Visual v)
        {
            // Open new package
            MemoryStream stream = new MemoryStream();
            Package package = Package.Open(stream, FileMode.Create);
            // Create new xps document based on the package opened
            XpsDocument doc = new XpsDocument(package);
            // Create an instance of XpsDocumentWriter for the document
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);
            // Write the Visual to the document
            writer.Write(v);
            // Close document
            doc.Close();
            // Close package
            package.Close();

            return stream.ToArray();
        }

        /// <summary>
        /// Restores a <see cref="Visual"/> from an array of bytes.
        /// </summary>
        /// <param name="data">The data to restore a <see cref="Visual"/> from.</param>
        /// <returns>The restored <see cref="Visual"/>.</returns>
        public static Visual LoadVisualFromByteArray(byte[] data)
        {
            // Open new package
            MemoryStream stream = new MemoryStream(data);
            Package pack = Package.Open(stream);
            Uri packUri = new Uri(@"memorystream://" + Guid.NewGuid().ToString(), UriKind.Absolute);
            PackageStore.AddPackage(packUri,pack);

            // Create new xps document based on the package opened
            XpsDocument doc = new XpsDocument(pack,CompressionOption.Normal,packUri.AbsoluteUri);

            Visual v = doc.GetFixedDocumentSequence().DocumentPaginator.GetPage(0).Visual;

            PackageStore.RemovePackage(packUri);
            pack.Close();

            return v;
        }
    }
}
