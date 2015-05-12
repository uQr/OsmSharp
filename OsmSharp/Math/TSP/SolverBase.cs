// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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

using OsmSharp.Math.TSP.Problems;
using OsmSharp.Math.VRP.Routes;
using OsmSharp.Math.VRP.Routes.ASymmetric;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Math.TSP
{
    /// <summary>
    /// Baseclass for all TSP solver.
    /// </summary>
    public abstract class SolverBase : ISolver
    {
        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// Executes the solver.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        public IRoute Solve(IProblem problem)
        {
            var convertedProblem = problem;

            var first = problem.First.HasValue;
            var last = problem.Last.HasValue;

            // convert the problem if needed.
            if (!first && !last)
            { // add a virtual customer with distance zero to all existing customers.
                var newWeights = new double[problem.WeightMatrix.Length + 1][];
                newWeights[0] = new double[problem.WeightMatrix.Length + 1];
                for (int idx = 0; idx < problem.WeightMatrix.Length + 1; idx++)
                {
                    newWeights[0][idx] = 0;
                }
                for(int idx = 0; idx < problem.WeightMatrix.Length; idx++)
                {
                    newWeights[idx+1] = new double[problem.WeightMatrix.Length + 1];
                    newWeights[idx+1][0] = 0;
                    problem.WeightMatrix[idx].CopyTo(newWeights[idx+1], 1);
                }
                convertedProblem = MatrixProblem.CreateATSP(newWeights, 0);
            }
            else if (!last)
            { // set all weights to the first customer to zero.
                for (int idx = 0; idx < problem.WeightMatrix.Length; idx++)
                {
                    problem.WeightMatrix[idx][problem.First.Value] = 0;
                }
                convertedProblem = MatrixProblem.CreateATSP(problem.WeightMatrix, problem.First.Value);
            }

            // execute the solver on the converted problem.
            var convertedRoute = this.DoSolve(convertedProblem);

            // convert the route back.
            if (!first && !last)
            { // when a virtual customer was added the route needs converting.
                var customersList = convertedRoute.ToList<int>();
                customersList.RemoveAt(0);
                for (int idx = 0; idx < customersList.Count; idx++)
                {
                    customersList[idx] = customersList[idx] - 1;
                }
                convertedRoute = DynamicAsymmetricRoute.CreateFrom(customersList, false);
            }
            else if (!last)
            { // the returned route will return to customer zero; convert the route.
                convertedRoute = DynamicAsymmetricRoute.CreateFrom(convertedRoute, false);
            }
            return convertedRoute;
        }

        /// <summary>
        /// Executes the actual solver code.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        protected abstract IRoute DoSolve(IProblem problem);

        /// <summary>
        /// Stops the solver.
        /// </summary>
        public virtual void Stop()
        {

        }

        #region Intermidiate Results

        /// <summary>
        /// Raised when an intermidiate result is available.
        /// </summary>
        public event SolverDelegates.IntermidiateDelegate IntermidiateResult;

        /// <summary>
        /// Returns true when the event has to be raised.
        /// </summary>
        /// <returns></returns>
        protected bool CanRaiseIntermidiateResult()
        {
            return this.IntermidiateResult != null;
        }

        /// <summary>
        /// Raises the intermidiate results event.
        /// </summary>
        /// <param name="result"></param>
        protected void RaiseIntermidiateResult(int[] result)
        {
            if (IntermidiateResult != null)
            {
                this.IntermidiateResult(result);
            }
        }

        #endregion
    }
}
