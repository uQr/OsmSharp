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

using OsmSharp.Collections.Coordinates.Collections;
using OsmSharp.Routing.Graph.Routing.Edges;
using OsmSharp.Routing.Graph.TurnCosts;
using System.Collections.Generic;

namespace OsmSharp.Routing.Graph.Routing
{
    /// <summary>
    /// Abstract a router that works on a dynamic graph.
    /// </summary>
    public abstract class RoutingAlgorithm<TEdgeData>
        where TEdgeData : IGraphEdgeData
    {
        /// <summary>
        /// Holds the graph.
        /// </summary>
        protected IGraphReadOnly<TEdgeData> _graph;

        /// <summary>
        /// Holds the turn costs.
        /// </summary>
        protected IGraphReadOnlyTurnCosts _turnCosts;

        /// <summary>
        /// Holds the edge weight calculator.
        /// </summary>
        protected EdgeWeightCalculator<TEdgeData> _edgeWeightCalculator;

        /// <summary>
        /// Creates a new routing algorithm instance.
        /// </summary>
        /// <param name="graph">The graph to use.</param>
        /// <param name="turnCosts">The turn costs to use.</param>
        /// <param name="edgeWeightCalculator">The edge weight calculator.</param>
        protected RoutingAlgorithm(IGraphReadOnly<TEdgeData> graph, IGraphReadOnlyTurnCosts turnCosts, EdgeWeightCalculator<TEdgeData> edgeWeightCalculator)
        {
            _graph = graph;
            _turnCosts = turnCosts;
            _edgeWeightCalculator = edgeWeightCalculator;
        }

        /// <summary>
        /// Calculates a shortest path from one of the given source edge visits to one of the given target edge visits.
        /// </summary>
        /// <param name="source">The edge visit(s) for the source-location.</param>
        /// <param name="target">The edge visit(s) for the target location.</param>
        /// <returns></returns>
        public virtual List<EdgeVisit> Calculate(IEnumerable<EdgeVisit> source, IEnumerable<EdgeVisit> target);

        #region Search Closest

        /// <summary>
        /// Searches the data for a point on an edge closest to the given coordinate.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="vehicle"></param>
        /// <param name="coordinate"></param>
        /// <param name="delta"></param>
        /// <param name="matcher"></param>
        /// <param name="pointTags"></param>
        /// <param name="interpreter"></param>
        /// <param name="parameters"></param>
        public virtual SearchClosestResult<TEdgeData> SearchClosest(IBasicRouterDataSource<TEdgeData> graph, IRoutingInterpreter interpreter, Vehicle vehicle,
            GeoCoordinate coordinate, float delta, IEdgeMatcher matcher, TagsCollectionBase pointTags, Dictionary<string, object> parameters)
        {
            return this.SearchClosest(graph, interpreter, vehicle, coordinate, delta, matcher, pointTags, false, null);
        }

        /// <summary>
        /// Searches the data for a point on an edge closest to the given coordinate.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="vehicle"></param>
        /// <param name="coordinate"></param>
        /// <param name="delta"></param>
        /// <param name="matcher"></param>
        /// <param name="pointTags"></param>
        /// <param name="interpreter"></param>
        /// <param name="verticesOnly"></param>
        /// <param name="parameters"></param>
        public virtual SearchClosestResult<TEdgeData> SearchClosest(IBasicRouterDataSource<TEdgeData> graph, IRoutingInterpreter interpreter, Vehicle vehicle,
            GeoCoordinate coordinate, float delta, IEdgeMatcher matcher, TagsCollectionBase pointTags, bool verticesOnly, Dictionary<string, object> parameters)
        {
            Meter distanceEpsilon = .1; // 10cm is the tolerance to distinguish points.

            var closestWithMatch = new SearchClosestResult<TEdgeData>(double.MaxValue, 0);
            var closestWithoutMatch = new SearchClosestResult<TEdgeData>(double.MaxValue, 0);

            double searchBoxSize = delta;
            // create the search box.
            var searchBox = new GeoCoordinateBox(new GeoCoordinate(
                coordinate.Latitude - searchBoxSize, coordinate.Longitude - searchBoxSize),
                                                               new GeoCoordinate(
                coordinate.Latitude + searchBoxSize, coordinate.Longitude + searchBoxSize));

            // get the arcs from the data source.
            var arcs = graph.GetEdges(searchBox);

            if (!verticesOnly)
            { // find both closest arcs and vertices.
                // loop over all.
                while (arcs.MoveNext())
                {
                    if (!graph.TagsIndex.Contains(arcs.EdgeData.Tags))
                    { // skip this edge, no valid tags found.
                        continue;
                    }
                    var arcTags = graph.TagsIndex.Get(arcs.EdgeData.Tags);
                    var canBeTraversed = vehicle.CanTraverse(arcTags);
                    if (canBeTraversed)
                    { // the edge can be traversed.
                        // test the two points.
                        float fromLatitude, fromLongitude;
                        float toLatitude, toLongitude;
                        double distance;
                        if (graph.GetVertex(arcs.Vertex1, out fromLatitude, out fromLongitude) &&
                            graph.GetVertex(arcs.Vertex2, out toLatitude, out toLongitude))
                        { // return the vertex.
                            var fromCoordinates = new GeoCoordinate(fromLatitude, fromLongitude);
                            distance = coordinate.DistanceReal(fromCoordinates).Value;
                            ICoordinateCollection coordinates;
                            ICoordinate[] coordinatesArray = null;
                            if (!graph.GetEdgeShape(arcs.Vertex1, arcs.Vertex2, out coordinates))
                            {
                                coordinates = null;
                            }
                            if (coordinates != null)
                            {
                                coordinatesArray = coordinates.ToArray();
                            }

                            if (distance < distanceEpsilon.Value)
                            { // the distance is smaller than the tolerance value.
                                closestWithoutMatch = new SearchClosestResult<TEdgeData>(
                                    distance, arcs.Vertex1);
                                if (matcher == null ||
                                    (pointTags == null || pointTags.Count == 0) ||
                                    matcher.MatchWithEdge(vehicle, pointTags, arcTags))
                                {
                                    closestWithMatch = new SearchClosestResult<TEdgeData>(
                                        distance, arcs.Vertex1);
                                    break;
                                }
                            }

                            if (distance < closestWithoutMatch.Distance)
                            { // the distance is smaller for the without match.
                                closestWithoutMatch = new SearchClosestResult<TEdgeData>(
                                    distance, arcs.Vertex1);
                            }
                            if (distance < closestWithMatch.Distance)
                            { // the distance is smaller for the with match.
                                if (matcher == null ||
                                    (pointTags == null || pointTags.Count == 0) ||
                                    matcher.MatchWithEdge(vehicle, pointTags, graph.TagsIndex.Get(arcs.EdgeData.Tags)))
                                {
                                    closestWithMatch = new SearchClosestResult<TEdgeData>(
                                        distance, arcs.Vertex1);
                                }
                            }
                            var toCoordinates = new GeoCoordinate(toLatitude, toLongitude);
                            distance = coordinate.DistanceReal(toCoordinates).Value;

                            if (distance < closestWithoutMatch.Distance)
                            { // the distance is smaller for the without match.
                                closestWithoutMatch = new SearchClosestResult<TEdgeData>(
                                    distance, arcs.Vertex2);
                            }
                            if (distance < closestWithMatch.Distance)
                            { // the distance is smaller for the with match.
                                if (matcher == null ||
                                    (pointTags == null || pointTags.Count == 0) ||
                                    matcher.MatchWithEdge(vehicle, pointTags, arcTags))
                                {
                                    closestWithMatch = new SearchClosestResult<TEdgeData>(
                                        distance, arcs.Vertex2);
                                }
                            }

                            // search along the line.
                            var distanceTotal = 0.0;
                            var previous = fromCoordinates;
                            var arcValueValueCoordinates = arcs.Intermediates;
                            if (arcValueValueCoordinates != null)
                            { // calculate distance along all coordinates.
                                var arcValueValueCoordinatesArray = arcValueValueCoordinates.ToArray();
                                for (int idx = 0; idx < arcValueValueCoordinatesArray.Length; idx++)
                                {
                                    var current = new GeoCoordinate(arcValueValueCoordinatesArray[idx].Latitude, arcValueValueCoordinatesArray[idx].Longitude);
                                    distanceTotal = distanceTotal + current.DistanceReal(previous).Value;
                                    previous = current;
                                }
                            }
                            distanceTotal = distanceTotal + toCoordinates.DistanceReal(previous).Value;
                            if (distanceTotal > 0)
                            { // the from/to are not the same location.
                                // loop over all edges that are represented by this arc (counting intermediate coordinates).
                                previous = fromCoordinates;
                                GeoCoordinateLine line;
                                var distanceToSegment = 0.0;
                                if (arcValueValueCoordinates != null)
                                {
                                    var arcValueValueCoordinatesArray = arcValueValueCoordinates.ToArray();
                                    for (int idx = 0; idx < arcValueValueCoordinatesArray.Length; idx++)
                                    {
                                        var current = new GeoCoordinate(
                                            arcValueValueCoordinatesArray[idx].Latitude, arcValueValueCoordinatesArray[idx].Longitude);
                                        line = new GeoCoordinateLine(previous, current, true, true);

                                        distance = line.DistanceReal(coordinate).Value;

                                        if (distance < closestWithoutMatch.Distance)
                                        { // the distance is smaller.
                                            var projectedPoint = line.ProjectOn(coordinate);

                                            // calculate the position.
                                            if (projectedPoint != null)
                                            { // calculate the distance
                                                var distancePoint = previous.DistanceReal(new GeoCoordinate(projectedPoint)).Value + distanceToSegment;
                                                var position = distancePoint / distanceTotal;

                                                closestWithoutMatch = new SearchClosestResult<TEdgeData>(
                                                    distance, arcs.Vertex1, arcs.Vertex2, position, arcs.EdgeData, coordinatesArray);
                                            }
                                        }
                                        if (distance < closestWithMatch.Distance)
                                        {
                                            var projectedPoint = line.ProjectOn(coordinate);

                                            // calculate the position.
                                            if (projectedPoint != null)
                                            { // calculate the distance
                                                var distancePoint = previous.DistanceReal(new GeoCoordinate(projectedPoint)).Value + distanceToSegment;
                                                var position = distancePoint / distanceTotal;

                                                if (matcher == null ||
                                                    (pointTags == null || pointTags.Count == 0) ||
                                                    matcher.MatchWithEdge(vehicle, pointTags, arcTags))
                                                {

                                                    closestWithMatch = new SearchClosestResult<TEdgeData>(
                                                        distance, arcs.Vertex1, arcs.Vertex2, position, arcs.EdgeData, coordinatesArray);
                                                }
                                            }
                                        }

                                        // add current segment distance to distanceToSegment for the next segment.
                                        distanceToSegment = distanceToSegment + line.LengthReal.Value;

                                        // set previous.
                                        previous = current;
                                    }
                                }

                                // check the last segment.
                                line = new GeoCoordinateLine(previous, toCoordinates, true, true);

                                distance = line.DistanceReal(coordinate).Value;

                                if (distance < closestWithoutMatch.Distance)
                                { // the distance is smaller.
                                    var projectedPoint = line.ProjectOn(coordinate);

                                    // calculate the position.
                                    if (projectedPoint != null)
                                    { // calculate the distance
                                        double distancePoint = previous.DistanceReal(new GeoCoordinate(projectedPoint)).Value + distanceToSegment;
                                        double position = distancePoint / distanceTotal;

                                        closestWithoutMatch = new SearchClosestResult<TEdgeData>(
                                            distance, arcs.Vertex1, arcs.Vertex2, position, arcs.EdgeData, coordinatesArray);
                                    }
                                }
                                if (distance < closestWithMatch.Distance)
                                {
                                    var projectedPoint = line.ProjectOn(coordinate);

                                    // calculate the position.
                                    if (projectedPoint != null)
                                    { // calculate the distance
                                        double distancePoint = previous.DistanceReal(new GeoCoordinate(projectedPoint)).Value + distanceToSegment;
                                        double position = distancePoint / distanceTotal;

                                        if (matcher == null ||
                                            (pointTags == null || pointTags.Count == 0) ||
                                            matcher.MatchWithEdge(vehicle, pointTags, arcTags))
                                        {

                                            closestWithMatch = new SearchClosestResult<TEdgeData>(
                                                distance, arcs.Vertex1, arcs.Vertex2, position, arcs.EdgeData, coordinatesArray);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            { // only find closest vertices.
                // loop over all.
                while (arcs.MoveNext())
                {
                    float fromLatitude, fromLongitude;
                    float toLatitude, toLongitude;
                    if (graph.GetVertex(arcs.Vertex1, out fromLatitude, out fromLongitude) &&
                        graph.GetVertex(arcs.Vertex2, out toLatitude, out toLongitude))
                    {
                        var vertexCoordinate = new GeoCoordinate(fromLatitude, fromLongitude);
                        double distance = coordinate.DistanceReal(vertexCoordinate).Value;
                        if (distance < closestWithoutMatch.Distance)
                        { // the distance found is closer.
                            closestWithoutMatch = new SearchClosestResult<TEdgeData>(
                                distance, arcs.Vertex1);
                        }

                        vertexCoordinate = new GeoCoordinate(toLatitude, toLongitude);
                        distance = coordinate.DistanceReal(vertexCoordinate).Value;
                        if (distance < closestWithoutMatch.Distance)
                        { // the distance found is closer.
                            closestWithoutMatch = new SearchClosestResult<TEdgeData>(
                                distance, arcs.Vertex2);
                        }

                        var arcValueValueCoordinates = arcs.Intermediates;
                        if (arcValueValueCoordinates != null)
                        { // search over intermediate points.
                            var arcValueValueCoordinatesArray = arcValueValueCoordinates.ToArray();
                            for (int idx = 0; idx < arcValueValueCoordinatesArray.Length; idx++)
                            {
                                vertexCoordinate = new GeoCoordinate(
                                    arcValueValueCoordinatesArray[idx].Latitude,
                                    arcValueValueCoordinatesArray[idx].Longitude);
                                distance = coordinate.DistanceReal(vertexCoordinate).Value;
                                if (distance < closestWithoutMatch.Distance)
                                { // the distance found is closer.
                                    closestWithoutMatch = new SearchClosestResult<TEdgeData>(
                                        distance, arcs.Vertex1, arcs.Vertex2, idx, arcs.EdgeData, arcValueValueCoordinatesArray);
                                }
                            }
                        }
                    }
                }
            }

            // return the best result.
            if (closestWithMatch.Distance < double.MaxValue)
            {
                return closestWithMatch;
            }
            return closestWithoutMatch;
        }

        #endregion
    }

    /// <summary>
    /// Enumerates the type of weights used by a basis router.
    /// </summary>
    public enum RouterWeightType
    {
        /// <summary>
        /// The router weights are time-estimates.
        /// </summary>
        Time,
        /// <summary>
        /// The router weights are distances.
        /// </summary>
        Distance,
        /// <summary>
        /// The router-weights are completely custom.
        /// </summary>
        Custom
    }

    /// <summary>
    /// The result the search closest returns.
    /// </summary>
    public struct SearchClosestResult<TEdgeData>
    {
        /// <summary>
        /// The result is located exactly at one vertex.
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="vertex"></param>
        public SearchClosestResult(double distance, uint vertex)
            : this()
        {
            this.Distance = distance;
            this.Vertex1 = vertex;
            this.Position = 0;
            this.Vertex2 = null;
        }

        /// <summary>
        /// The result is located between two other vertices but on an intermediate point.
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="intermediateIndex"></param>
        /// <param name="edge"></param>
        /// <param name="coordinates"></param>
        public SearchClosestResult(double distance, uint vertex1, uint vertex2, int intermediateIndex, TEdgeData edge, ICoordinate[] coordinates)
            : this()
        {
            this.Distance = distance;
            this.Vertex1 = vertex1;
            this.Vertex2 = vertex2;
            this.IntermediateIndex = intermediateIndex;
            this.Edge = edge;
            this.Coordinates = coordinates;
        }

        /// <summary>
        /// The result is located between two other vertices.
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="position"></param>
        /// <param name="edge"></param>
        /// <param name="coordinates"></param>
        public SearchClosestResult(double distance, uint vertex1, uint vertex2, double position, TEdgeData edge, ICoordinate[] coordinates)
            : this()
        {
            this.Distance = distance;
            this.Vertex1 = vertex1;
            this.Vertex2 = vertex2;
            this.Position = position;
            this.Edge = edge;
            this.Coordinates = coordinates;
        }

        /// <summary>
        /// The first vertex.
        /// </summary>
        public uint? Vertex1 { get; private set; }

        /// <summary>
        /// The second vertex.
        /// </summary>
        public uint? Vertex2 { get; private set; }

        /// <summary>
        /// The intermediate point position.
        /// </summary>
        public int? IntermediateIndex { get; private set; }

        /// <summary>
        /// The position between vertex1 and vertex2 (0=vertex1, 1=vertex2).
        /// </summary>
        public double Position { get; private set; }

        /// <summary>
        /// The distance from the point being resolved.
        /// </summary>
        public double Distance { get; private set; }

        /// <summary>
        /// The edge data.
        /// </summary>
        public TEdgeData Edge { get; private set; }

        /// <summary>
        /// The coordinates.
        /// </summary>
        public ICoordinate[] Coordinates { get; set; }
    }
}