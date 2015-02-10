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
    /// Represents a collection of turning costs associated with a graph.
    /// </summary>
    public interface IGraphReadOnlyTurnCosts
    {
        /// <summary>
        /// Returns true if this turn cost index can be used for the given vehicle profile.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        bool IsForVehicle(Vehicle vehicle);

        /// <summary>
        /// Returns or calculates all turning costs for a given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        TurnCostTable GetTurnCosts(uint vertex);

        /// <summary>
        /// Returns of calculate the turning cost between two edges along a given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="edgeIdFrom"></param>
        /// <param name="edgeIdTo"></param>
        /// <returns></returns>
        TurnCost GetTurnCost(uint vertex, long edgeIdFrom, long edgeIdTo);
    }
}