// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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

using OsmSharp.Collections.Coordinates.Collections;
namespace OsmSharp.Routing.Graph
{
    /// <summary>
    /// Abstracts a graph implementation.
    /// </summary>
    public abstract class Graph<TEdgeData>
        where TEdgeData : IEdge
    {
        /// <summary>
        /// Returns true if an edge is only as an outgoing edge. When false, edges are added both from and to.
        /// </summary>
        public abstract bool IsDirected { get; }

        /// <summary>
        /// Returns true if there can be multiple edges 
        /// </summary>
        public abstract bool CanHaveDuplicates { get; }

        /// <summary>
        /// Gets an existing vertex.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public abstract bool GetVertex(uint id, out float latitude, out float longitude);

        /// <summary>
        /// Returns true if the given edge exists.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <param name="neighbour"></param>
        /// <returns></returns>
        public abstract bool ContainsEdge(uint vertexId, uint neighbour);

        /// <summary>
        /// Returns true if the given edge exists.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <param name="neighbour"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract bool ContainsEdge(uint vertexId, uint neighbour, TEdgeData data);

        /// <summary>
        /// Returns all edges adjacent to the given vertex.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <returns></returns>
        public abstract IEdgeEnumerator<TEdgeData> GetEdges(uint vertexId);

        /// <summary>
        /// Returns all edges between the given vertices.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <returns></returns>
        public abstract IEdgeEnumerator<TEdgeData> GetEdges(uint vertex1, uint vertex2);

        /// <summary>
        /// Returns the edge between the two given vertices. Throws exception if this graph allows duplicate edges, use GetEdges in that case..
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public abstract bool GetEdge(uint vertex1, uint vertex2, out TEdgeData data);

        /// <summary>
        /// Returns the edge shape between the two given vertices. Throws exception if this graph allows duplicate edges, use GetEdges in that case.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public abstract bool GetEdgeShape(uint vertex1, uint vertex2, out ICoordinateCollection shape);

        /// <summary>
        /// Returns the total number of vertices.
        /// </summary>
        public abstract uint VertexCount
        {
            get;
        }
        /// <summary>
        /// Adds a vertex.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public abstract uint AddVertex(float latitude, float longitude);

        /// <summary>
        /// Sets the vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public abstract void SetVertex(uint vertex, float latitude, float longitude);

        /// <summary>
        /// Adds an edge with associated data and no shapes.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="data"></param>
        public abstract void AddEdge(uint from, uint to, TEdgeData data);

        /// <summary>
        /// Adds an edge with associated data.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="data"></param>
        /// <param name="coordinates"></param>
        public abstract void AddEdge(uint from, uint to, TEdgeData data, ICoordinateCollection coordinates);

        /// <summary>
        /// Compresses all the data in this graph.
        /// </summary>
        /// <remarks>Does not trim or resize the internal data structures. Use Trim()</remarks>
        public abstract void Compress();

        /// <summary>
        /// Trims all internal datastructures to their smallest possible size.
        /// </summary>
        /// <remarks>Be careful when doing this, could need up to 2x the current memory usage.</remarks>
        public abstract void Trim();

        /// <summary>
        /// Resizes the internal data structures of the graph to handle the number of vertices/edges estimated.
        /// </summary>
        /// <param name="vertexEstimate"></param>
        /// <param name="edgeEstimate"></param>
        public abstract void Resize(long vertexEstimate, long edgeEstimate);

        /// <summary>
        /// Removes all edges adjacent to the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        public abstract void RemoveEdges(uint vertex);

        /// <summary>
        /// Removes all the edges between the two given vertices.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public abstract void RemoveEdge(uint from, uint to);

        /// <summary>
        /// Removes the edge between the two given vertices and with matching data.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="data"></param>
        public abstract void RemoveEdge(uint from, uint to, TEdgeData data);
    }
}