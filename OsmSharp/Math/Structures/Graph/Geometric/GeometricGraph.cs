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

using OsmSharp.Collections.Arrays;
using OsmSharp.Collections.Coordinates.Collections;
using OsmSharp.Math.Geo.Simple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Math.Structures.Graph.Geometric
{
    /// <summary>
    /// A graph with geometric data associated.
    /// </summary>
    public class GeometricGraph
    {
        /// <summary>
        /// Holds the graph.
        /// </summary>
        private IGraph _graph;

        /// <summary>
        /// Holds the coordinates of the vertices.
        /// </summary>
        private IHugeArray<GeoCoordinateSimple> _coordinates;

        /// <summary>
        /// Holds all shapes associated with edges.
        /// </summary>
        private HugeCoordinateCollectionIndex _edgeShapes;

        public GeometricGraph()
        {
            // _graph = new MemoryGraph()
        }

        /// <summary>
        /// Returns the topological graph.
        /// </summary>
        public IGraph Grap
        {
            get
            {
                return _graph;
            }
        }

        /// <summary>
        /// Adds a new vertex at the given location.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public uint AddVertex(float latitude, float longitude)
        {
            var vertex = _graph.AddVertex();
            _coordinates[vertex] = new GeoCoordinateSimple()
            {
                Latitude = latitude,
                Longitude = longitude
            };
            return vertex;
        }

        /// <summary>
        /// Sets the location of the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public void SetVertex(uint vertex, float latitude, float longitude)
        {
            if (_graph.HasVertex(vertex))
            {
                _coordinates[vertex] = new GeoCoordinateSimple()
                {
                    Latitude = latitude,
                    Longitude = longitude
                };
            }
        }

        /// <summary>
        /// Gets the location of the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public bool GetVertex(uint vertex, out float latitude, out float longitude)
        {
            if (_graph.HasVertex(vertex))
            {
                var location = _coordinates[vertex];
                latitude = location.Latitude;
                longitude = location.Longitude;
                return true;
            }
            latitude = 0;
            longitude = 0;
            return false;
        }

        public void AddEdge(uint from, uint to, byte[] edgeData, CoordinateArrayCollection<GeoCoordinateSimple> coordinateArrayCollection)
        {
            throw new NotImplementedException();
        }
    }
}
