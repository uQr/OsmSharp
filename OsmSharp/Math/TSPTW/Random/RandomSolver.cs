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
using OsmSharp.Math.VRP.Routes.ASymmetric;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Math.TSPTW.Random
{
    /// <summary>
    /// Just generates random routes.
    /// </summary>
    public class RandomSolver : ISolver
    {
        /// <summary>
        /// Creates a random solver.
        /// </summary>
        public RandomSolver()
        {

        }

        /// <summary>
        /// Retuns the name of this solver.
        /// </summary>
        public string Name
        {
            get
            {
                return "Random";
            }
        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <param name="problem">The problem to solve.</param>
        /// <param name="fitness">The fitness of the solution found.</param>
        /// <returns></returns>
        public IRoute Solve(IProblem problem, out double fitness)
        {
            var route = RandomSolver.DoSolveStatic(problem);

            // calculate fitness.
            fitness = 0;
            foreach(var pair in route.Pairs())
            {
                fitness = fitness + problem.Weight(pair.From, pair.To);
            }

            return route;
        }

        /// <summary>
        /// Generates a random route.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        public static IRoute DoSolveStatic(IProblem problem)
        {
            var customers = new List<int>();
            for (int customer = 0; customer < problem.Size; customer++)
            {
                customers.Add(customer);
            }
            customers.Shuffle<int>();
            return DynamicAsymmetricRoute.CreateFrom(customers);
        }

        /// <summary>
        /// Stops execution.
        /// </summary>
        public void Stop()
        {

        }

        /// <summary>
        /// Reports an intermediate result if someone is interested.
        /// </summary>
        /// <param name="result"></param>
        private void ReportIntermidiateResult(IRoute result)
        {
            if (this.IntermidiateResult != null)
            { // yes, there is a listener that cares!
                this.IntermidiateResult(result.ToArray());
            }
        }

        /// <summary>
        /// Event to report a new solution.
        /// </summary>
        public event SolverDelegates.IntermidiateDelegate IntermidiateResult;
    }
}