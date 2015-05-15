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

using OsmSharp.Math.VRP.Routes;
using System;
namespace OsmSharp.Math.TSPTW.LocalSearch
{
    /// <summary>
    /// A local 1-Shift search for the TSP with Time Window.
    /// </summary>
    /// <remarks>* 1-shift: Remove a customer and relocate it somewhere.</remarks>
    public class Local1Shift : IOperator
    {
        /// <summary>
        /// Returns the name of the operator.
        /// </summary>
        public string Name
        {
            get { return "LOCAL_1SHFT"; }
        }

        /// <summary>
        /// Returns true if there was an improvement, false otherwise.
        /// </summary>
        /// <param name="problem">The problem.</param>
        /// <param name="route">The route.</param>
        /// <param name="delta">The difference in fitness.</param>
        /// <returns></returns>
        public bool Apply(IProblem problem, IRoute route, out double delta)
        {
            var currentDelta = 0;
            do
            { // try to place each customer.
                var enumerator = route.GetEnumerator();
                enumerator.MoveNext();
                var previous = enumerator.Current;
                while(enumerator.MoveNext())
                {
                    foreach (var edge in route.Pairs())
                    {

                    }
                }
            } while (currentDelta > 0);
            throw new NotImplementedException();
        }
    }
}