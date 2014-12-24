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

namespace OsmSharp.Math.Structures.Graph
{
    /// <summary>
    /// Abstracts a graph that is readonly. 
    /// </summary>
    public interface IGraphReadOnly
    {
        /// <summary>
        /// Returns true if a vertex with given id exists in this graph.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        bool HasVertex(uint vertex);

        /// <summary>
        /// Gets the edge information for the given edge.
        /// </summary>
        /// <param name="edge">The edge.</param>
        /// <param name="vertex1">The first vertex.</param>
        /// <param name="vertex2">The second vertex.</param>
        /// <param name="edgeData">The output array containing all the edge data.</param>
        /// <returns>True if the edge exists, false otherwise.</returns>
        bool GetEdge(long edge, out uint vertex1, out uint vertex2, ref uint[] edgeData);

        /// <summary>
        /// Gets the edge between the two vertices.
        /// </summary>
        /// <param name="vertex1">First vertex.</param>
        /// <param name="vertex2">Second vertex.</param>
        /// <param name="edge">The edge id, positive if the edge is forward relative to the order vertex1->vertex2, negative otherwise.</param>
        /// <returns>True if the edge exists, false otherwise.</returns>
        bool GetEdge(uint vertex1, uint vertex2, out long edge);

        /// <summary>
        /// Gets the edge information for the edge between the two vertices.
        /// </summary>
        /// <param name="vertex1">First vertex.</param>
        /// <param name="vertex2">Second vertex.</param>
        /// <param name="edge">The edge id, positive if the edge is forward relative to the order vertex1->vertex2, negative otherwise.</param>
        /// <param name="edgeData">The output array containing all the edge data.</param>
        /// <returns>True if the edge exists, false otherwise.</returns>
        bool GetEdge(uint vertex1, uint vertex2, out long edge, ref uint[] edgeData);

        /// <summary>
        /// Gets the all edges adjacent to the given vertex.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        /// <param name="edges">The output array containing the edge id's.</param>
        /// <param name="edgeData">The output array containing all the edge data.</param>
        /// <param name="neighbours">The array containing the neigbours of the given vertex.</param>
        /// <returns></returns>
        int GetEdges(uint vertex, ref long[] edges, ref uint[] neighbours, ref uint[] edgeData);

        /// <summary>
        /// Returns the size of the data in one edge.
        /// </summary>
        int EdgeDataSize { get; }

        /// <summary>
        /// Returns the number of edges.
        /// </summary>
        uint EdgeCount { get; }

        /// <summary>
        /// Returns the number of vertices.
        /// </summary>
        uint VertexCount { get; }
    }
}