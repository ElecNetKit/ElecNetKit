using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;
using System.IO;

namespace ElecNetKit.Util
{
    /// <summary>
    /// A Twin-Key dictionary, useful for working with tables of data, such as
    /// sensitivities. Can index by either key or both. Could also be interpreted
    /// as a sparse matrix with flexible types for the rows and cols.
    /// </summary>
    /// <typeparam name="TKeyX">One type to index on.</typeparam>
    /// <typeparam name="TKeyY">The other type to index on.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    [Serializable]
    public class TwinKeyDictionary<TKeyX, TKeyY, TValue> : IDeserializationCallback
    {
        private Dictionary<TKeyX, Dictionary<TKeyY, TValue>> _MapX;

        /// <summary>
        /// Access the TwinKeyDictionary first by Row.
        /// </summary>
        public Dictionary<TKeyX, Dictionary<TKeyY, TValue>> MapX { get { return _MapX; } }

        [NonSerialized]
        private Dictionary<TKeyY, Dictionary<TKeyX, TValue>> _MapY;

        /// <summary>
        /// Access the TwinKeyDictionary first by Column.
        /// </summary>
        public Dictionary<TKeyY, Dictionary<TKeyX, TValue>> MapY { get { return _MapY; } }

        /// <summary>
        /// Instantiate a new dictionary.
        /// </summary>
        public TwinKeyDictionary()
        {
            _MapX = new Dictionary<TKeyX, Dictionary<TKeyY, TValue>>();
            _MapY = new Dictionary<TKeyY, Dictionary<TKeyX, TValue>>();
        }

        /// <summary>
        /// Add a new entry to the table.
        /// </summary>
        /// <param name="keyX">The X parameter.</param>
        /// <param name="keyY">The Y parameter.</param>
        /// <param name="value">The value to store at (X,Y).</param>
        public void Add(TKeyX keyX, TKeyY keyY, TValue value)
        {
            if (!_MapX.ContainsKey(keyX))
                _MapX[keyX] = new Dictionary<TKeyY, TValue>();

            if (!_MapY.ContainsKey(keyY))
                _MapY[keyY] = new Dictionary<TKeyX, TValue>();

            _MapX[keyX].Add(keyY, value);
            _MapY[keyY].Add(keyX, value);
        }

        /// <summary>
        /// Tests whether the dictionary contains a key at the specified XY
        /// location.
        /// </summary>
        /// <param name="keyX">The row to look in.</param>
        /// <param name="keyY">The column to look in.</param>
        /// <returns>True if the key exists.</returns>
        public bool ContainsKey(TKeyX keyX, TKeyY keyY)
        {
            return (_MapX.ContainsKey(keyX) &&
                _MapX[keyX].ContainsKey(keyY));
        }

        /// <summary>
        /// Tests whether a column exists in the dictionary.
        /// </summary>
        /// <param name="keyY">The key of the column to test for.</param>
        /// <returns>True if the column exists.</returns>
        public bool ContainsKeyY(TKeyY keyY)
        {
            return _MapY.ContainsKey(keyY);
        }

        /// <summary>
        /// Tests whether a row exists in the dictionary.
        /// </summary>
        /// <param name="keyX">The row to test existence of.</param>
        /// <returns>True if the row exists.</returns>
        public bool ContainsKeyX(TKeyX keyX)
        {
            return _MapX.ContainsKey(keyX);
        }

        /// <summary>
        /// Remove the table entry at (X,Y).
        /// </summary>
        /// <param name="keyX">The X location of the value to remove.</param>
        /// <param name="keyY">The Y location of the value to remove.</param>
        public void Remove(TKeyX keyX, TKeyY keyY)
        {
            _MapX[keyX].Remove(keyY);
            _MapY[keyY].Remove(keyX);

            if (_MapX[keyX].Count == 0)
                _MapX.Remove(keyX);

            if (_MapY[keyY].Count == 0)
                _MapY.Remove(keyY);
        }
        
        /// <summary>
        /// Imports the entries from <paramref name="otherDictionary"/> into this dictionary.
        /// </summary>
        /// <param name="otherDictionary">The other dictionary to merge entries from.</param>
        public void Merge(TwinKeyDictionary<TKeyX, TKeyY, TValue> otherDictionary)
        {
            foreach (var mX in otherDictionary.MapX)
            {
                foreach (var mY in mX.Value)
                {
                    this.Add(mX.Key, mY.Key, mY.Value);
                }
            }
        }

        /// <summary>
        /// Merges in values from a dictionary of dictionaries.
        /// </summary>
        /// <param name="otherDictionary">The dictionary to merge in.</param>
        public void MergeX(Dictionary<TKeyX, Dictionary<TKeyY, TValue>> otherDictionary)
        {
            foreach (var kvpX in otherDictionary)
            {
                foreach (var kvpY in kvpX.Value)
                {
                    this.Add(kvpX.Key, kvpY.Key, kvpY.Value);
                }
            }
        }

        /// <summary>
        /// Merges in values from a dictionary of dictionaries.
        /// </summary>
        /// <param name="otherDictionary">The dictionary to merge in.</param>
        public void MergeY(Dictionary<TKeyY, Dictionary<TKeyX, TValue>> otherDictionary)
        {
            foreach (var kvpY in otherDictionary)
                foreach (var kvpX in kvpY.Value)
                    this.Add(kvpX.Key, kvpY.Key, kvpX.Value);
        }

        /// <summary>
        /// Rebuilds index after deserialisation.
        /// </summary>
        /// <param name="sender">Not used or implemented.</param>
        public void OnDeserialization(object sender)
        {
            _MapX.OnDeserialization(sender);
            //now rebuild the MapY dictionary from the MapX dictionary.
            _MapY = new Dictionary<TKeyY, Dictionary<TKeyX, TValue>>();
            foreach (var kvpX in _MapX)
            {
                kvpX.Value.OnDeserialization(sender);
                foreach (var kvpY in kvpX.Value)
                {
                    if (!_MapY.ContainsKey(kvpY.Key))
                        _MapY[kvpY.Key] = new Dictionary<TKeyX, TValue>();

                    _MapY[kvpY.Key][kvpX.Key] = kvpY.Value;
                }
            }
        }
    }
}
