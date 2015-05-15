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

using OsmSharp.Math.TSPTW;
using OsmSharp.Math.VRP;
using System.Collections.Generic;

namespace OsmSharp.Test.Unittests.Math.TSPTW
{
    /// <summary>
    /// A stub problem for the TSP with time window tests.
    /// </summary>
    public class ProblemStub : IProblem
    {
        /// <summary>
        /// Hold the symmetric flag.
        /// </summary>
        private bool _symmetric;

        /// <summary>
        /// Holds the euclidean flag.
        /// </summary>
        private bool _euclidean;

        /// <summary>
        /// Holds the weights.
        /// </summary>
        private double[][] _weights;

        /// <summary>
        /// Holds the timewindows.
        /// </summary>
        private TimeWindow[] _windows;

        /// <summary>
        /// Holds the first customer.
        /// </summary>
        private int? _first;

        /// <summary>
        /// Holds the last customer.
        /// </summary>
        private int? _last;

        /// <summary>
        /// Creates a problem with the given size and the same weight everywhere.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="weight"></param>
        public ProblemStub(int size, double weight)
        {
            _weights = new double[size][];
            for(int x = 0; x < size; x++)
            {
                _weights[x] = new double[size];
                for (int y = 0; y < size; y++)
                {
                    _weights[x][y] = weight;
                }
            }
            _first = 0;
            _last = null;
            _symmetric = false;
            _euclidean = false;
        }

        /// <summary>
        /// Returns the weight between two customers.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public double Weight(int from, int to)
        {
            return _weights[from][to];
        }

        /// <summary>
        /// Returns true if the problem is symmetric.
        /// </summary>
        public bool Symmetric
        {
            get
            {
                return _symmetric;
            }
        }

        /// <summary>
        /// Returns true if the problem is euclidean.
        /// </summary>
        public bool Euclidean
        {
            get
            {
                return _euclidean;
            }
        }

        /// <summary>
        /// Returns the size of the problem.
        /// </summary>
        public int Size
        {
            get
            {
                return _weights.GetLength(0);
            }
        }

        /// <summary>
        /// Returns the first customer.
        /// </summary>
        public int? First
        {
            get
            {
                return _first;
            }
        }

        /// <summary>
        /// Returns the last customer.
        /// </summary>
        public int? Last
        {
            get
            {
                return _last;
            }
        }

        /// <summary>
        /// Returns the actual weight matrix.
        /// </summary>
        public double[][] WeightMatrix
        {
            get
            {
                return _weights;
            }
        }

        /// <summary>
        /// Returns the time windows.
        /// </summary>
        public TimeWindow[] Windows
        {
            get { return _windows; }
        }

        #region Nearest Neighbour

        /// <summary>
        /// Keeps the nearest neighbour list.
        /// </summary>
        private NearestNeighbours10[] _neighbours;

        /// <summary>
        /// Generate the nearest neighbour list.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public NearestNeighbours10 Get10NearestNeighbours(int v)
        {
            if (_neighbours == null)
            {
                _neighbours = new NearestNeighbours10[this.Size];
            }
            NearestNeighbours10 result = _neighbours[v];
            if (result == null)
            {
                var neighbours = new SortedDictionary<double, List<int>>();
                for (int customer = 0; customer < this.Size; customer++)
                {
                    if (customer != v)
                    {
                        double weight = this.WeightMatrix[v][customer];
                        List<int> customers = null;
                        if (!neighbours.TryGetValue(weight, out customers))
                        {
                            customers = new List<int>();
                            neighbours.Add(weight, customers);
                        }
                        customers.Add(customer);
                    }
                }

                result = new NearestNeighbours10();
                foreach (KeyValuePair<double, List<int>> pair in neighbours)
                {
                    foreach (int customer in pair.Value)
                    {
                        if (result.Count < 10)
                        {
                            if (result.Max < pair.Key)
                            {
                                result.Max = pair.Key;
                            }
                            result.Add(customer);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                _neighbours[v] = result;
            }
            return result;
        }

        #endregion
    }
}