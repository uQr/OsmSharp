using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OsmSharp.IO.Json
{
    public class JsonSerializer
    {
        /// <summary>
        /// Delegate for parsing json from a stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public delegate object DoDeserialize(Stream stream);

        /// <summary>
        /// Holds a delegate for deserializing jons.
        /// </summary>
        public static DoDeserialize DoSerializeNative;

        /// <summary>
        /// Deserializes json from the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static object Deserialize(Stream stream)
        {
            if (JsonSerializer.DoSerializeNative == null)
            { // delegate not set!
                throw new InvalidOperationException("Json serializer delegate not initialized, call OsmSharp.{Platform).UI.Native.Initialize() in the native code.");
            }
            return JsonSerializer.DoSerializeNative(stream);
        }
    }
}
