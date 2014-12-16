// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2014 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using OsmSharp.Collections.Arrays;
using System;

namespace OsmSharp.Math.Structures.Graph
{
    /// <summary>
    /// Represents graph by using in-memory data structures.
    /// </summary>
    public class MemoryGraph : IGraph
    {
        private static int EDGE_CORE_SIZE = 4;
        private const uint EMPTY = uint.MaxValue;
        private const int NODEA = 0;
        private const int NODEB = 1;
        private const int NEXTNODEA = 2;
        private const int NEXTNODEB = 3;

        /// <summary>
        /// Holds the size of one edge entry.
        /// </summary>
        /// <remarks>EDGE_CORE_SIZE + (amount of data linked to one edge)</remarks>
        private int _edgeSize;

        /// <summary>
        /// Holds the next vertex id.
        /// </summary>
        private uint _nextVertexId;

        /// <summary>
        /// Holds the next edge id.
        /// </summary>
        private uint _nextEdgeId;

        /// <summary>
        /// Holds all vertices pointing to it's first edge.
        /// </summary>
        private IHugeArray<uint> _vertices;

        /// <summary>
        /// Holds all edges and edge data.
        /// </summary>
        private IHugeArray<uint> _edges;

        /// <summary>
        /// Creates a new graph.
        /// </summary>
        /// <param name="edgeDataSize">The size of the data package kept per edge.</param>
        public MemoryGraph(int edgeDataSize)
            : this(1000, 3000, edgeDataSize)
        {

        }

        /// <summary>
        /// Creates a new graph.
        /// </summary>
        /// <param name="vertexEstimate">The estimated vertex count.</param>
        /// <param name="edgeEstimate">The estimated edge count.</param>
        /// <param name="edgeDataSize">The size of the data package kept per edge.</param>
        public MemoryGraph(uint vertexEstimate, uint edgeEstimate, int edgeDataSize)
            : this(0, 0, new HugeArray<uint>(vertexEstimate), new HugeArray<uint>(edgeEstimate * (EDGE_CORE_SIZE + edgeDataSize)), edgeDataSize)
        {

        }

        /// <summary>
        /// Creates a new graph using the given arrays. Erases all data in the given arrays.
        /// </summary>
        /// <param name="vertices">The vertices array to use.</param>
        /// <param name="edges">The edges array to use.</param>
        /// <param name="edgeDataSize">The size of the data package kept per edge.</param>
        /// <returns></returns>
        public static MemoryGraph CreateNew(IHugeArray<uint> vertices, IHugeArray<uint> edges, int edgeDataSize)
        {
            for(long idx = 0; idx < vertices.Length; idx++)
            {
                vertices[idx] = EMPTY;
            }
            for (long idx = 0; idx < edges.Length; idx++)
            {
                edges[idx] = EMPTY;
            }

            return new MemoryGraph(0, 0, vertices, edges, edgeDataSize);
        }

        /// <summary>
        /// Creates a graph from the data in the given arrays. Assumes the arrays already contain proper vertices and edge information.
        /// </summary>
        /// <param name="vertices">The vertices array.</param>
        /// <param name="edges">The edges array.</param>
        /// <param name="edgeDataSize">The size of the data package kept per edge.</param>
        /// <returns></returns>
        public static MemoryGraph CreateFrom(IHugeArray<uint> vertices, IHugeArray<uint> edges, int edgeDataSize)
        {
            return new MemoryGraph((uint)vertices.Length, (uint)edges.Length, vertices, edges, edgeDataSize);
        }

        /// <summary>
        /// Creates a new graph using the given arrays.
        /// </summary>
        /// <param name="nextVertexId">The next vertex.</param>
        /// <param name="nextEdgeId">The next edge.</param>
        /// <param name="vertices">The vertices array.</param>
        /// <param name="edges">The edges array.</param>
        /// <param name="edgeDataSize">The size of the data package kept per edge.</param>
        internal MemoryGraph(uint nextVertexId, uint nextEdgeId, IHugeArray<uint> vertices, IHugeArray<uint> edges, int edgeDataSize)
        {
            _nextVertexId = nextVertexId;
            _nextEdgeId = nextEdgeId;
            _vertices = vertices;
            _edges = edges;
        }

        /// <summary>
        /// Adds a new vertex.
        /// </summary>
        /// <returns></returns>
        public uint AddVertex()
        {
            if (_nextVertexId >= _vertices.Length)
            { // make sure vertices array is large enough.
                this.IncreaseVertexSize();
            }

            uint newId = _nextVertexId;
            _vertices[newId] = EMPTY;
            _nextVertexId++;
            return newId;
        }

        /// <summary>
        /// Removes the given vertex and all of it's adjacent edges.
        /// </summary>
        /// <param name="vertex"></param>
        public void RemoveVertex(uint vertex)
        {

        }

        /// <summary>
        /// Adds a new edge between vertex1 and vertex2 with the data given. Overwrites existing edges and associated data.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="edgeData"></param>
        /// <returns></returns>
        public long AddEdge(uint vertex1, uint vertex2, uint[] edgeData)
        {
            if (vertex1 == vertex2) { throw new ArgumentException("Given vertices must be different."); }
            if (_nextVertexId <= vertex1) { throw new ArgumentOutOfRangeException("vertex1", "vertex1 is not part of this graph."); }
            if (_nextVertexId <= vertex2) { throw new ArgumentOutOfRangeException("vertex2", "vertex2 is not part of this graph."); }

            var edgeId = _vertices[vertex1];
            if (_vertices[vertex1] != EMPTY)
            { // check for an existing edge first.
                // check if the arc exists already.
                edgeId = _vertices[vertex1];
                uint nextEdgeSlot = 0;
                while (edgeId != EMPTY)
                { // keep looping.
                    uint otherVertexId = 0;
                    uint previousEdgeId = edgeId;
                    bool forward = true;
                    if (_edges[edgeId + NODEA] == vertex1)
                    {
                        otherVertexId = _edges[edgeId + NODEB];
                        nextEdgeSlot = edgeId + NEXTNODEA;
                        edgeId = _edges[edgeId + NEXTNODEA];
                    }
                    else
                    {
                        otherVertexId = _edges[edgeId + NODEA];
                        nextEdgeSlot = edgeId + NEXTNODEB;
                        edgeId = _edges[edgeId + NEXTNODEB];
                        forward = false;
                    }
                    if (otherVertexId == vertex2)
                    { // this is the edge we need: overwrite the existing data.
                        throw new InvalidOperationException("Edge already exists!");
                    }
                }

                // create a new edge.
                edgeId = _nextEdgeId;
                if (_nextEdgeId + _edgeSize >= _edges.Length)
                { // there is a need to increase edges array.
                    this.IncreaseEdgeSize();
                }
                _edges[_nextEdgeId + NODEA] = vertex1;
                _edges[_nextEdgeId + NODEB] = vertex2;
                _edges[_nextEdgeId + NEXTNODEA] = EMPTY;
                _edges[_nextEdgeId + NEXTNODEB] = EMPTY;
                _nextEdgeId = _nextEdgeId + (uint)_edgeSize;

                // append the new edge to the from list.
                _edges[nextEdgeSlot] = edgeId;

                // set data.
                for(int idx = 0; idx < _edgeSize - EDGE_CORE_SIZE; idx++)
                {
                    _edges[_nextEdgeId + EDGE_CORE_SIZE + idx] = edgeData[idx];
                }
            }
            else
            { // create a new edge and set.
                edgeId = _nextEdgeId;
                _vertices[vertex1] = _nextEdgeId;

                if (_nextEdgeId + _edgeSize >= _edges.Length)
                { // there is a need to increase edges array.
                    this.IncreaseEdgeSize();
                }
                _edges[_nextEdgeId + NODEA] = vertex1;
                _edges[_nextEdgeId + NODEB] = vertex2;
                _edges[_nextEdgeId + NEXTNODEA] = EMPTY;
                _edges[_nextEdgeId + NEXTNODEB] = EMPTY;
                _nextEdgeId = _nextEdgeId + (uint)_edgeSize;

                // set data.
                for (int idx = 0; idx < _edgeSize - EDGE_CORE_SIZE; idx++)
                {
                    _edges[_nextEdgeId + EDGE_CORE_SIZE + idx] = edgeData[idx];
                }
            }

            var toEdgeId = _vertices[vertex2];
            if (toEdgeId != EMPTY)
            { // there are existing edges.
                uint nextEdgeSlot = 0;
                while (toEdgeId != EMPTY)
                { // keep looping.
                    uint otherVertexId = 0;
                    if (_edges[toEdgeId + NODEA] == vertex2)
                    {
                        otherVertexId = _edges[toEdgeId + NODEB];
                        nextEdgeSlot = toEdgeId + NEXTNODEA;
                        toEdgeId = _edges[toEdgeId + NEXTNODEA];
                    }
                    else
                    {
                        otherVertexId = _edges[toEdgeId + NODEA];
                        nextEdgeSlot = toEdgeId + NEXTNODEB;
                        toEdgeId = _edges[toEdgeId + NEXTNODEB];
                    }
                }
                _edges[nextEdgeSlot] = edgeId;
            }
            else
            { // there are no existing edges point the vertex straight to it's first edge.
                _vertices[vertex2] = edgeId;
            }

            return edgeId;
        }

        /// <summary>
        /// Removes the edge between vertex1 and vertex2.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <returns></returns>
        public bool RemoveEdge(uint vertex1, uint vertex2)
        {
            if (_nextVertexId <= vertex1) { throw new ArgumentOutOfRangeException("vertex1", "vertex1 is not part of this graph."); }
            if (_nextVertexId <= vertex2) { throw new ArgumentOutOfRangeException("vertex2", "vertex2 is not part of this graph."); }

            if (_vertices[vertex1] == EMPTY ||
                _vertices[vertex2] == EMPTY)
            { // no edge to remove here!
                return false;
            }

            // remove for vertex1.
            var nextEdgeId = _vertices[vertex1];
            uint nextEdgeSlot = 0;
            uint previousEdgeSlot = 0;
            uint currentEdgeId = 0;
            while (nextEdgeId != EMPTY)
            { // keep looping.
                uint otherVertexId = 0;
                currentEdgeId = nextEdgeId;
                previousEdgeSlot = nextEdgeSlot;
                if (_edges[nextEdgeId + NODEA] == vertex1)
                {
                    otherVertexId = _edges[nextEdgeId + NODEB];
                    nextEdgeSlot = nextEdgeId + NEXTNODEA;
                    nextEdgeId = _edges[nextEdgeId + NEXTNODEA];
                }
                else
                {
                    otherVertexId = _edges[nextEdgeId + NODEA];
                    nextEdgeSlot = nextEdgeId + NEXTNODEB;
                    nextEdgeId = _edges[nextEdgeId + NEXTNODEB];
                }
                if (otherVertexId == vertex2)
                { // this is the edge we need.
                    if (_vertices[vertex1] == currentEdgeId)
                    { // the edge being remove if the 'first' edge.
                        // point to the next edge.
                        _vertices[vertex1] = nextEdgeId;
                    }
                    else
                    { // the edge being removed is not the 'first' edge.
                        // set the previous edge slot to the current edge id being the next one.
                        _edges[previousEdgeSlot] = nextEdgeId;
                    }
                    break;
                }
            }

            // remove for vertex2.
            nextEdgeId = _vertices[vertex2];
            nextEdgeSlot = 0;
            previousEdgeSlot = 0;
            currentEdgeId = 0;
            while (nextEdgeId != EMPTY)
            { // keep looping.
                uint otherVertexId = 0;
                currentEdgeId = nextEdgeId;
                previousEdgeSlot = nextEdgeSlot;
                if (_edges[nextEdgeId + NODEA] == vertex2)
                {
                    otherVertexId = _edges[nextEdgeId + NODEB];
                    nextEdgeSlot = nextEdgeId + NEXTNODEA;
                    nextEdgeId = _edges[nextEdgeId + NEXTNODEA];
                }
                else
                {
                    otherVertexId = _edges[nextEdgeId + NODEA];
                    nextEdgeSlot = nextEdgeId + NEXTNODEB;
                    nextEdgeId = _edges[nextEdgeId + NEXTNODEB];
                }
                if (otherVertexId == vertex1)
                { // this is the edge we need.
                    if (_vertices[vertex2] == currentEdgeId)
                    { // the edge being remove if the 'first' edge.
                        // point to the next edge.
                        _vertices[vertex2] = nextEdgeId;
                    }
                    else
                    { // the edge being removed is not the 'first' edge.
                        // set the previous edge slot to the current edge id being the next one.
                        _edges[previousEdgeSlot] = nextEdgeId;
                    }

                    // reset everything about this edge.
                    _edges[currentEdgeId + NODEA] = EMPTY;
                    _edges[currentEdgeId + NODEB] = EMPTY;
                    _edges[currentEdgeId + NEXTNODEA] = EMPTY;
                    _edges[currentEdgeId + NEXTNODEB] = EMPTY;

                    // reset data.
                    for (int idx = 0; idx < _edgeSize - EDGE_CORE_SIZE; idx++)
                    {
                        _edges[_nextEdgeId + EDGE_CORE_SIZE + idx] = 0;
                    }
                    return true;
                }
            }
            throw new Exception("Edge could not be reached from vertex2. Data in graph is invalid.");
        }

        /// <summary>
        /// Removes the given edge.
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public bool RemoveEdge(long edge)
        {
            return this.RemoveEdge(_edges[edge + NODEA], _edges[edge + NODEB]);
        }

        public void Compress()
        {
            throw new System.NotImplementedException();
        }

        public void Trim()
        {
            throw new System.NotImplementedException();
        }

        public void Resize(long vertexEstimate, long edgeEstimate)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Returns true if the given vertex exists.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <returns></returns>
        public bool HasVertex(uint vertex)
        {
            if (_nextVertexId > vertex)
            {
                return true;
            }
            return false;
        }

        public bool GetEdge(long edge, out uint vertex1, out uint vertex2, ref uint[] edgeData)
        {
            vertex1 = _edges[edge + NODEA];
            vertex2 = _edges[edge + NODEB];

            if(vertex1 == EMPTY || vertex2 == EMPTY)
            { // hmm, no valid information here for sure.
                return false;
            }

            // get data.
            for (int idx = 0; idx < _edgeSize - EDGE_CORE_SIZE && idx < edgeData.Length; idx++)
            {
                edgeData[idx] = _edges[edge + EDGE_CORE_SIZE + idx];
            }
            return true;
        }

        public bool GetEdge(uint vertex1, uint vertex2, out long edge)
        {
            if (_nextVertexId <= vertex1) { throw new ArgumentOutOfRangeException("vertex1", "vertex1 is not part of this graph."); }
            if (_nextVertexId <= vertex2) { throw new ArgumentOutOfRangeException("vertex2", "vertex2 is not part of this graph."); }

            if (_vertices[vertex1] == EMPTY)
            { // no edges here!
                edge = 0;
                return false;
            }
            var edgeId = _vertices[vertex1];
            uint nextEdgeSlot = 0;
            while (edgeId != EMPTY)
            { // keep looping.
                uint otherVertexId = 0;
                if (_edges[edgeId + NODEA] == vertex1)
                {
                    otherVertexId = _edges[edgeId + NODEB];
                    edgeId = _edges[edgeId + NEXTNODEA];
                    nextEdgeSlot = edgeId + NEXTNODEA;
                }
                else
                {
                    otherVertexId = _edges[edgeId + NODEA];
                    edgeId = _edges[edgeId + NEXTNODEB];
                    nextEdgeSlot = edgeId + NEXTNODEB;
                }
                if (otherVertexId == vertex2)
                { // this is the edge we need.
                    edge = edgeId;
                    return true;
                }
            }
            edge = 0;
            return false;
        }

        public bool GetEdge(uint vertex1, uint vertex2, out long edge, ref uint[] edgeData)
        {
            if (_nextVertexId <= vertex1) { throw new ArgumentOutOfRangeException("vertex1", "vertex1 is not part of this graph."); }
            if (_nextVertexId <= vertex2) { throw new ArgumentOutOfRangeException("vertex2", "vertex2 is not part of this graph."); }
            if (edgeData == null) { throw new ArgumentNullException("edgeData"); }

            if (_vertices[vertex1] == EMPTY)
            { // no edges here!
                edge = 0;
                return false;
            }
            var edgeId = _vertices[vertex1];
            uint nextEdgeSlot = 0;
            while (edgeId != EMPTY)
            { // keep looping.
                uint otherVertexId = 0;
                if (_edges[edgeId + NODEA] == vertex1)
                {
                    otherVertexId = _edges[edgeId + NODEB];
                    edgeId = _edges[edgeId + NEXTNODEA];
                    nextEdgeSlot = edgeId + NEXTNODEA;
                }
                else
                {
                    otherVertexId = _edges[edgeId + NODEA];
                    edgeId = _edges[edgeId + NEXTNODEB];
                    nextEdgeSlot = edgeId + NEXTNODEB;
                }
                if (otherVertexId == vertex2)
                { // this is the edge we need.
                    edge = edgeId;

                    // get data.
                    for (int idx = 0; idx < _edgeSize - EDGE_CORE_SIZE && idx < edgeData.Length; idx++)
                    {
                        edgeData[idx] = _edges[edge + EDGE_CORE_SIZE + idx];
                    }
                    return true;
                }
            }
            edge = 0;
            return false;
        }

        public int GetEdges(uint vertex, ref long[] edges, ref uint[] neighbours)
        {
            if (_nextVertexId <= vertex) { throw new ArgumentOutOfRangeException("vertex1", "vertex1 is not part of this graph."); }
            if (neighbours == null) { throw new ArgumentNullException("neighbours"); }

            if (_vertices[vertex] == EMPTY)
            { // no edges here!
                return -1;
            }
            var edgeId = _vertices[vertex];
            uint nextEdgeSlot = 0;
            int edgeIdx = 0;
            while (edgeId != EMPTY)
            { // keep looping.
                uint otherVertexId = 0;
                if (_edges[edgeId + NODEA] == vertex)
                {
                    otherVertexId = _edges[edgeId + NODEB];
                    edgeId = _edges[edgeId + NEXTNODEA];
                    nextEdgeSlot = edgeId + NEXTNODEA;
                    edges[edgeIdx] = edgeId;
                    neighbours[edgeIdx] = otherVertexId;
                }
                else
                {
                    otherVertexId = _edges[edgeId + NODEA];
                    edgeId = _edges[edgeId + NEXTNODEB];
                    nextEdgeSlot = edgeId + NEXTNODEB;
                    edges[edgeIdx] = -edgeId;
                    neighbours[edgeIdx] = otherVertexId;
                }
                edgeIdx++;
            }
            return edgeIdx;
        }

        public int GetEdges(uint vertex, ref long[] edges, ref uint[] neighbours, ref uint[] edgeData)
        {
            if (_nextVertexId <= vertex) { throw new ArgumentOutOfRangeException("vertex1", "vertex1 is not part of this graph."); }
            if (edgeData == null) { throw new ArgumentNullException("edgeData"); }
            if (neighbours == null) { throw new ArgumentNullException("neighbours"); }

            if (_vertices[vertex] == EMPTY)
            { // no edges here!
                return -1;
            }
            var edgeId = _vertices[vertex];
            uint nextEdgeSlot = 0;
            int edgeIdx = 0;
            while (edgeId != EMPTY)
            { // keep looping.
                uint otherVertexId = 0;
                if (_edges[edgeId + NODEA] == vertex)
                {
                    otherVertexId = _edges[edgeId + NODEB];
                    edgeId = _edges[edgeId + NEXTNODEA];
                    nextEdgeSlot = edgeId + NEXTNODEA;
                    edges[edgeIdx] = edgeId;
                    neighbours[edgeIdx] = otherVertexId;
                }
                else
                {
                    otherVertexId = _edges[edgeId + NODEA];
                    edgeId = _edges[edgeId + NEXTNODEB];
                    nextEdgeSlot = edgeId + NEXTNODEB;
                    edges[edgeIdx] = -edgeId;
                    neighbours[edgeIdx] = otherVertexId;
                }

                // get data.
                for (int idx = 0; idx < _edgeSize - EDGE_CORE_SIZE && idx < edgeData.Length; idx++)
                {
                    edgeData[idx] = _edges[(edgeIdx * _edgeSize) + idx];
                }
                edgeIdx++;
            }
            return edgeIdx;
        }

        /// <summary>
        /// Returns the size of the data in one edge.
        /// </summary>
        public int EdgeDataSize
        {
            get { return _edgeSize - EDGE_CORE_SIZE; }
        }

        /// <summary>
        /// The number of edges.
        /// </summary>
        public uint EdgeCount
        {
            get { return _nextEdgeId; }
        }

        /// <summary>
        /// The number of vertices.
        /// </summary>
        public uint VertexCount
        {
            get { return _nextVertexId; }
        }

        #region Internal Bookkeeping

        /// <summary>
        /// Gets the index associated with the given edge and return true if it exists.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="edgeDataIdx"></param>
        /// <param name="edgeDataForward"></param>
        /// <returns></returns>
        private bool GetEdgeIdx(uint vertex1, uint vertex2, out long edgeDataIdx, out bool edgeDataForward)
        {
            if (_nextVertexId <= vertex1) { throw new ArgumentOutOfRangeException("vertex1", "vertex1 is not part of this graph."); }
            if (_nextVertexId <= vertex2) { throw new ArgumentOutOfRangeException("vertex2", "vertex2 is not part of this graph."); }

            if (_vertices[vertex1] == EMPTY)
            { // no edges here!
                edgeDataIdx = -1;
                edgeDataForward = false;
                return false;
            }
            var edgeId = _vertices[vertex1];
            uint nextEdgeSlot = 0;
            while (edgeId != EMPTY)
            { // keep looping.
                uint otherVertexId = 0;
                var currentEdgeId = edgeId;
                edgeDataForward = true;
                if (_edges[edgeId + NODEA] == vertex1)
                {
                    otherVertexId = _edges[edgeId + NODEB];
                    edgeId = _edges[edgeId + NEXTNODEA];
                    nextEdgeSlot = edgeId + NEXTNODEA;
                }
                else
                {
                    otherVertexId = _edges[edgeId + NODEA];
                    edgeId = _edges[edgeId + NEXTNODEB];
                    nextEdgeSlot = edgeId + NEXTNODEB;
                    edgeDataForward = false;
                }
                if (otherVertexId == vertex2)
                { // this is the edge we need.
                    edgeDataIdx = currentEdgeId;
                    return true;
                }
            }
            edgeDataForward = false;
            edgeDataIdx = -1;
            return false;
        }

        /// <summary>
        /// Increases the memory allocation.
        /// </summary>
        private void IncreaseVertexSize()
        {
            this.IncreaseVertexSize(_vertices.Length + 10000);
        }

        /// <summary>
        /// Increases the memory allocation.
        /// </summary>
        /// <param name="size"></param>
        private void IncreaseVertexSize(long size)
        {
            var oldLength = _vertices.Length;
            _vertices.Resize(size);
            for (long idx = oldLength; idx < size; idx++)
            {
                _vertices[idx] = EMPTY;
            }
        }

        /// <summary>
        /// Increases the memory allocation.
        /// </summary>
        private void IncreaseEdgeSize()
        {
            this.IncreaseEdgeSize(_edges.Length + 10000);
        }

        /// <summary>
        /// Increases the memory allocation.
        /// </summary>
        private void IncreaseEdgeSize(long size)
        {
            var oldLength = _edges.Length;
            _edges.Resize(size);
            for (long idx = oldLength; idx < size; idx++)
            {
                _edges[idx] = EMPTY;
            }
        }

        #endregion
    }
}