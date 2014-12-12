using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.Osm.Graphs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OsmSharp.Routing.Graph.MemoryMapping
{
    /// <summary>
    /// Holds methods to map structures.
    /// </summary>
    public static class MemoryMappedStructures
    {
        /// <summary>
        /// Holds the read buffer.
        /// </summary>
        private static byte[] _readBuffer = new byte[32];

        /// <summary>
        /// Reads an object of the given type from the stream at the current position.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static object Read(Type type, Stream stream)
        {
            if (typeof(LiveEdge) == type)
            {
                stream.Read(_readBuffer, 0, 8);
                return new LiveEdge()
                {
                    Value = BitConverter.ToUInt32(_readBuffer, 0),
                    Distance = BitConverter.ToSingle(_readBuffer, 4)
                };
            }
            else if (typeof(CHEdgeData) == type)
            {
                stream.Read(_readBuffer, 0, 21);
                return new CHEdgeData()
                {
                    ContractedDirectionValue = _readBuffer[0], // 1
                    TagsValue = BitConverter.ToUInt32(_readBuffer, 1), // 4
                    ForwardWeight = BitConverter.ToSingle(_readBuffer, 5), // 4
                    ForwardContractedId = BitConverter.ToUInt32(_readBuffer, 9), // 4
                    BackwardWeight = BitConverter.ToSingle(_readBuffer, 13), // 4
                    BackwardContractedId = BitConverter.ToUInt32(_readBuffer, 17) // 4
                };
            }
            throw new NotSupportedException(string.Format("Type {0} not supported for memory mapping.", type.ToInvariantString()));
        }

        /// <summary>
        /// Writes an object of the given type to the stream at the current position.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="stream"></param>
        /// <param name="structure"></param>
        public static void Write(Type type, Stream stream, ref object structure)
        {
            if (typeof(LiveEdge) == type)
            {
                var edge = (LiveEdge)structure;
                stream.Write(BitConverter.GetBytes(edge.Value), 0, 4);
                stream.Write(BitConverter.GetBytes(edge.Distance), 0, 4);
                return;
            }
            else if (typeof(CHEdgeData) == type)
            {
                var edge = (CHEdgeData)structure;
                stream.Write(new byte[] { edge.ContractedDirectionValue }, 0, 1);
                stream.Write(BitConverter.GetBytes(edge.TagsValue), 0, 4);
                stream.Write(BitConverter.GetBytes(edge.ForwardWeight), 0, 4);
                stream.Write(BitConverter.GetBytes(edge.ForwardContractedId), 0, 4);
                stream.Write(BitConverter.GetBytes(edge.BackwardWeight), 0, 4);
                stream.Write(BitConverter.GetBytes(edge.BackwardContractedId), 0, 4);
                return;
            }
            throw new NotSupportedException(string.Format("Type {0} not supported for memory mapping.", type.ToInvariantString()));
        }

        /// <summary>
        /// Returns the size of a structure of the given type on disk.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int SizeOf(Type type)
        {
            if (typeof(LiveEdge) == type)
            {
                return 8;
            }
            else if (typeof(CHEdgeData) == type)
            {
                return 21;
            }
            throw new NotSupportedException(string.Format("Type {0} not supported for memory mapping.", type.ToInvariantString()));
        }
    }
}
