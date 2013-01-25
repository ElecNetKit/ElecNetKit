using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using ElecNetKit.NetworkModelling;

namespace ElecNetKitExplorer
{
    static class QuickSerialisers
    {
        public static void Serialise(this Object obj, String path)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(path, FileMode.Create);
            formatter.Serialize(stream, obj);
            stream.Close();
        }

        public static void Serialise(this Object obj, Stream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
        }

        public static Object Deserialise(String path)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            Object model = formatter.Deserialize(stream);
            stream.Close();
            return model;
        }

        public static Object Deserialise(Stream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            Object model = formatter.Deserialize(stream);
            return model;
        }
    }
}
