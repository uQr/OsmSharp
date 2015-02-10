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

using OsmSharp.Collections.PriorityQueues;
using OsmSharp.Logging;
using OsmSharp.Routing.Constraints;
using OsmSharp.Routing.Graph.Routing.Edges;
using OsmSharp.Routing.Graph.TurnCosts;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Osm.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Routing.Graph.Routing
{
    /// <summary>
    /// A class containing a dykstra implementation suitable for a simple graph.
    /// </summary>
    public class Dykstra<TEdgeData> : RoutingAlgorithm<TEdgeData>
        where TEdgeData : IGraphEdgeData
    {
        /// <summary>
        /// Creates a new dykstra routing object.
        /// </summary>
        /// <param name="graph">The graph to use.</param>
        /// <param name="turnCosts">The turn costs to use.</param>
        /// <param name="edgeWeightCalculator">The edge weight calculator.</param>
        public Dykstra(IGraphReadOnly<TEdgeData> graph, IGraphReadOnlyTurnCosts turnCosts, EdgeWeightCalculator<TEdgeData> edgeWeightCalculator)
            : base(graph, turnCosts, edgeWeightCalculator)
        {

        }

        /// <summary>
        /// Calculates a shortest path from one of the given source edge visits to one of the given target edge visits.
        /// </summary>
        /// <param name="source">The edge visit(s) for the source-location.</param>
        /// <param name="target">The edge visit(s) for the target location.</param>
        /// <returns></returns>
        public override List<EdgeVisit> Calculate(IEnumerable<EdgeVisit> source, IEnumerable<EdgeVisit> target)
        {
            List<EdgeVisit> result = null;
            var maxWeight = float.MaxValue;
            var priorityQueue = new BinaryHeap<EdgeVisit>();
            var visits = new Dictionary<long, EdgeVisit>();

            // queue all source visits.
            foreach (var sourceVisit in source)
            {
                priorityQueue.Push(sourceVisit, sourceVisit.Weight);
                visits[sourceVisit.EdgeId] = sourceVisit;
            }

            // queue all target visits.
            var backwardVisits = new Dictionary<long, EdgeVisit>();
            foreach (var targetVisit in target)
            {
                backwardVisits[targetVisit.EdgeId] = targetVisit;
            }

            // start search.
            var edgeEnumerator = _graph.GetEdgeEnumerator();
            while(priorityQueue.Count > 0)
            {
                // dequeue next.
                var current = priorityQueue.Pop();
                visits[current.EdgeId] = current;

                // check if max weight has been reached.
                if(current.Weight >= maxWeight)
                { // maximum weight has been reached, stop the search.
                    return result;
                }
                // check if one of the targets has been reached.
                if(backwardVisits.ContainsKey(current.EdgeId))
                { // one of the targets was reached.
                    var toTarget = backwardVisits[current.EdgeId];
                    backwardVisits.Remove(current.EdgeId);

                    // build result if better.
                    var toTargetWeight = toTarget.Weight + current.Weight;
                    if(toTargetWeight < maxWeight)
                    { // the current result is better.
                        // TODO: build result.

                        // update max weight.
                        maxWeight = toTargetWeight;
                    }

                    // check if targets is empty.
                    if(backwardVisits.Count == 0)
                    { // no more targets.
                        return result;
                    }
                }

                // go over all neighbours.
                edgeEnumerator.MoveTo(current.To);
                while(edgeEnumerator.MoveNext())
                {
                    var edgeId = edgeEnumerator.EdgeId;

                    // check weights.
                    var turnCost = _turnCosts.GetTurnCost(current.To, current.EdgeId, edgeId);
                    if(turnCost.Weight == float.MaxValue)
                    { // path restricted.
                        continue;
                    }

                    // calculate weight.
                    var weight = _edgeWeightCalculator.Calculate(edgeEnumerator.EdgeData);
                    if(weight == float.MaxValue)
                    { // edge restricted.
                        continue;
                    }

                    // enqueue.
                    var newVisitWeight = weight + turnCost.Weight;
                    if (newVisitWeight < maxWeight)
                    {
                        var newVisit = new EdgeVisit()
                        {
                            EdgeId = edgeId,
                            From = current.To,
                            To = edgeEnumerator.Neighbour,
                            Weight = newVisitWeight
                        };
                        priorityQueue.Push(newVisit, newVisit.Weight);
                    }
                }
            }
        }
    }
}