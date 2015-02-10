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

namespace OsmSharp.Routing.Graph.Routing.Edges.Generic
{
    /// <summary>
    /// A representation of a class to calculate weights for a generic edge based on a given vehicle profile.
    /// </summary>
    public class GenericEdgeWeightCalculator : EdgeWeightCalculator<GenericEdge>
    {
        /// <summary>
        /// Holds the vehicle.
        /// </summary>
        private Vehicle _vehicle;

        /// <summary>
        /// Holds the tags collection index.
        /// </summary>
        private ITagsCollectionIndexReadonly _tagsCollectionIndex;

        /// <summary>
        /// Creates a new generic edge weight calculator.
        /// </summary>
        /// <param name="tagsCollectionIndex">The tags collection index.</param>
        /// <param name="vehicle">The vehicle profile.</param>
        public GenericEdgeWeightCalculator(ITagsCollectionIndexReadonly tagsCollectionIndex, Vehicle vehicle)
        {
            _tagsCollectionIndex = tagsCollectionIndex;
            _vehicle = vehicle;
        }

        /// <summary>
        /// Calculate weight weight of the given edge.
        /// </summary>
        /// <param name="edgeData"></param>
        /// <returns></returns>
        public override float Calculate(GenericEdge edgeData)
        {
            var tags = _tagsCollectionIndex.Get(edgeData.Tags);
            return _vehicle.Weight(tags, edgeData.Distance);
        }
    }
}