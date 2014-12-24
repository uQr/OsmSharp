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
    /// Abstracts a graph.
    /// </summary>
    public interface IGraph : IGraphWriteOnly
    {
        /// <summary>
        /// Removes the vertex and all of it's adjacent edges.
        /// </summary>
        /// <param name="vertex">The vertex.</param>
        void RemoveVertex(uint vertex);

        /// <summary>
        /// Removes the edge between the two given vertices. Also removes the edge vertex2->vertex1.
        /// </summary>
        /// <param name="vertex1">The first vertex.</param>
        /// <param name="vertex2">The second vertex.</param>
        bool RemoveEdge(uint vertex1, uint vertex2);

        /// <summary>
        /// Removes the edge.
        /// </summary>
        /// <param name="edge">The edge.</param>
        /// <returns></returns>
        bool RemoveEdge(long edge);
    }
}