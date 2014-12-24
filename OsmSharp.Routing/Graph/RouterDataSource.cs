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

using OsmSharp.Collections.Tags.Index;
using OsmSharp.Math.Structures.Graph;
using OsmSharp.Math.Structures.Graph.Geometric;
using OsmSharp.Routing.Graph.Router;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Routing.Graph
{
    /// <summary>
    /// Representation of a routing datasource/graph.
    /// </summary>
    public class RouterDataSource
    {

        /// <summary>
        /// Holds the supported vehicle profiles.
        /// </summary>
        private readonly HashSet<Vehicle> _supportedVehicles;

        /// <summary>
        /// Gets the tags collection index.
        /// </summary>
        public ITagsCollectionIndex Tags
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the topological graph.
        /// </summary>
        public IGraph Graph
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the geometric graph.
        /// </summary>
        public GeometricGraph GeometricGraph
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Adds a new restricted sequence of vertices.
        /// </summary>
        /// <param name="restriction">The sequence of vertices that is forbidden.</param>
        public void AddRestriction(uint[] restriction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a new restricted sequence of vertices for one vehicle.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="restriction"></param>
        public void AddRestriction(Vehicle vehicle, uint[] restriction)
        {
            throw new NotImplementedException();
        }
    }
}