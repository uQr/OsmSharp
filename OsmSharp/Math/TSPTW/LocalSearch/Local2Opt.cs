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

using OsmSharp.Math.VRP.Core.Routes;

namespace OsmSharp.Math.TSPTW.LocalSearch
{
    /// <summary>
    /// A local 2-Opt search for the TSP with Time Window.
    /// </summary>
    public class Local2Opt : IOperator
    {
        /// <summary>
        /// Returns the name of this operator.
        /// </summary>
        public string Name
        {
            get { return "2_OPT"; }
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="route">The route.</param>
        /// <param name="difference">The difference in fitness.</param>
        /// <returns></returns>
        public bool Apply(IProblem problem, IRoute route, out double difference)
        {
            throw new System.NotImplementedException();
        }
    }
}