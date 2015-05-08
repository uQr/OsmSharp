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
using System.Linq;

namespace OsmSharp.Math.TSPTW.IterativeImprovement
{
    /// <summary>
    /// A solver that let's another solver try n-number of times and then returns the best solution.
    /// </summary>
    public class InterativeImprovementSolver : ISolver
    {
        /// <summary>
        /// The number of times to try.
        /// </summary>
        private int _n;

        /// <summary>
        /// The solver to try.
        /// </summary>
        private ISolver _solver;

        /// <summary>
        /// Creates a new iterative improvement solver.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="n"></param>
        public InterativeImprovementSolver(ISolver solver, int n)
        {
            _solver = solver;
            _n = n;
        }

        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        public string Name
        {
            get { return string.Format( "ITER_{0}_{1}", _n, _solver.Name); }
        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <param name="problem">The problem to solve.</param>
        /// <param name="fitness">The fitness of the solution found.</param>
        /// <returns></returns>
        public IRoute Solve(IProblem problem, out double fitness)
        {
            var i = 0;
            IRoute best = null;
            fitness = double.MaxValue;
            while(i < _n)
            {
                var nextFitness = double.MaxValue;
                var nextRoute = _solver.Solve(problem, out nextFitness);
                if (nextFitness < fitness)
                { // yep, found a better solution!
                    best = nextRoute;
                    fitness = nextFitness;

                    this.ReportIntermidiateResult(best);
                }
                i++;
            }
            return best;
        }

        /// <summary>
        /// Stops this solver.
        /// </summary>
        public void Stop()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Reports an intermediate result if someone is interested.
        /// </summary>
        /// <param name="result"></param>
        private void ReportIntermidiateResult(IRoute result)
        {
            if(this.IntermidiateResult != null)
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