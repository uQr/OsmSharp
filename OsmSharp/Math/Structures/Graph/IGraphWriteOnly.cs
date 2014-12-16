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

namespace OsmSharp.Math.Structures.Graph
{
    /// <summary>
    /// Abstracts a graph that is writeonly. 
    /// </summary>
    public interface IGraphWriteOnly : IGraphReadOnly
    {
        /// <summary>
        /// Adds a new vertex.
        /// </summary>
        /// <returns></returns>
        uint AddVertex();

        /// <summary>
        /// Adds an edge with associated data.
        /// </summary>
        /// <param name="vertex1">The vertex were the edge starts.</param>
        /// <param name="vertex2">The vertex where the edge ends.</param>
        /// <param name="edgeData">The data associated with the edge.</param>
        /// <returns>The new edge.</returns>
        /// <remarks>Overwrites existing data if the edge already exists. Also if an edge vertex2->vertex1 exists.</remarks>
        long AddEdge(uint vertex1, uint vertex2, uint[] edgeData);

        /// <summary>
        /// Compresses all the data in this graph. Does not trim or resize the internal data structures. Use Compress() on a graph that has had a lot op recent changes, use Trim() before saving a graph to disk.
        /// </summary>
        /// <remarks></remarks>
        void Compress();

        /// <summary>
        /// Trims all internal datastructures to their smallest possible size. Use Compress() on a graph that has had a lot op recent changes, use Trim() before saving a graph to disk.
        /// </summary>
        void Trim();

        /// <summary>
        /// Resizes the internal data structures of the graph to handle the number of vertices/edges estimated.
        /// </summary>
        /// <param name="vertexEstimate"></param>
        /// <param name="edgeEstimate"></param>
        void Resize(long vertexEstimate, long edgeEstimate);
    }
}