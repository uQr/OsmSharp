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

namespace OsmSharp.Routing.Graph.TurnCosts
{
    /// <summary>
    /// Represents a cost of turning from one edge into another.
    /// </summary>
    public struct TurnCost
    {
        /// <summary>
        /// Holds the source edge.
        /// </summary>
        public long EdgeIdFrom;

        /// <summary>
        /// Holds the target edge.
        /// </summary>
        public long EdgeIdTo;

        /// <summary>
        /// Holds the weight of this turn.
        /// </summary>
        public float Weight;
    }
}