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
using System.Collections.Generic;

namespace OsmSharp.Routing.Graph
{
    /// <summary>
    /// Abstract representation of a routing datasource/graph.
    /// </summary>
    public interface IRouterDataSource
    {
        /// <summary>
        /// Returns true if the given vehicle profile is supported.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        bool SupportsProfile(Vehicle vehicle);

        /// <summary>
        /// Adds a supported profile.
        /// </summary>
        /// <param name="vehicle"></param>
        void AddSupportedProfile(Vehicle vehicle);

        /// <summary>
        /// Returns all supported profiles.
        /// </summary>
        IEnumerable<Vehicle> GetSupportedProfiles();

        /// <summary>
        /// Returns the core graph.
        /// </summary>
        /// <remarks>Contains the topology only.</remarks>
        IGraph Graph { get; }

        /// <summary>
        /// Gets the geometric graph.
        /// </summary>
        /// <remarks>Contains both topology and geometry.</remarks>
        GeometricGraph GeometricGraph { get; }

        /// <summary>
        /// Gets the tags index.
        /// </summary>
        ITagsCollectionIndexReadonly TagsIndex { get; }
    }
}