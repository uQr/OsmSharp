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
using System.Linq;

namespace OsmSharp.Math.TSPTW.VNS
{
    /// <summary>
    /// A Variable Neighbourhood Search solver.
    /// </summary>
    public class VNSSolver : ISolver
    {
        /// <summary>
        /// The solver that generates initial solutions.
        /// </summary>
        private ISolver _generator;

        /// <summary>
        /// The operator that makes sure the neighbourhood varies.
        /// </summary>
        private IPerturber _perturber;

        /// <summary>
        /// The local search operator.
        /// </summary>
        private IOperator _localSearch;

        /// <summary>
        /// A delegate for the stop condition.
        /// </summary>
        /// <param name="iteration">The iteration count.</param>
        /// <param name="problem">The problem.</param>
        /// <param name="route">The route.</param>
        /// <returns></returns>
        public delegate bool StopConditionDelegate(int iteration, IProblem problem, IRoute route);

        /// <summary>
        /// Holds the stop condition.
        /// </summary>
        private StopConditionDelegate _stopCondition;

        /// <summary>
        /// Creates a new VNS solver.
        /// </summary>
        /// <param name="generator">The solver that generates initial solutions.</param>
        /// <param name="perturber">The perturber that varies neighbourhood.</param>
        /// <param name="localSearch">The local search that improves solutions.</param>
        public VNSSolver(ISolver generator, IPerturber perturber, IOperator localSearch)
        {
            _generator = generator;
            _perturber = perturber;
            _localSearch = localSearch;
        }

        /// <summary>
        /// Creates a new VNS solver.
        /// </summary>
        /// <param name="generator">The solver that generates initial solutions.</param>
        /// <param name="perturber">The perturber that varies neighbourhood.</param>
        /// <param name="localSearch">The local search that improves solutions.</param>
        /// <param name="stopCondition">The stop condition to control the number of iterations.</param>
        public VNSSolver(ISolver generator, IPerturber perturber, IOperator localSearch, StopConditionDelegate stopCondition)
        {
            _generator = generator;
            _perturber = perturber;
            _localSearch = localSearch;
            _stopCondition = stopCondition;
        }

        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        public string Name
        {
            get { return string.Format("VNS_{0}_{1}", _perturber.Name, _localSearch.Name); }
        }

        /// <summary>
        /// Solves the given problem.
        /// </summary>
        /// <param name="problem">The problem to solve.</param>
        /// <param name="fitness">The fitness of the solution found.</param>
        /// <returns></returns>
        public IRoute Solve(IProblem problem, out double fitness)
        {
            var globalBestFitness = double.MaxValue;
            var globalBest = _generator.Solve(problem, out globalBestFitness);

            var difference = 0.0;
            if(_localSearch.Apply(problem, globalBest, out difference))
            { // localsearch leads to better solution, adjust the fitness.
                globalBestFitness = globalBestFitness + difference;
            }

            var i = 0;
            var level = 0;
            while(_stopCondition == null || _stopCondition.Invoke(i, problem, globalBest))
            {
                // copy current solution.
                var perturbedSolution = globalBest.Clone() as IRoute;
                var perturbedDifference = 0.0;
                _perturber.Apply(problem, perturbedSolution, level, out perturbedDifference); // don't care about improvement or not, just keep difference.
                var localSearchDifference = 0.0;
                _localSearch.Apply(problem, perturbedSolution, out localSearchDifference); // don't care about improvement or not, just keep difference.
                if (perturbedDifference - localSearchDifference < 0)
                { // there was an improvement, keep new solution as global.
                    globalBestFitness = globalBestFitness - perturbedDifference - localSearchDifference;
                    globalBest = perturbedSolution;
                    level = 1; // reset level.
                }
                else
                {
                    level = level + 1;
                }
            }
            fitness = globalBestFitness;
            return globalBest;
        }

        /// <summary>
        /// Stops this solver.
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
