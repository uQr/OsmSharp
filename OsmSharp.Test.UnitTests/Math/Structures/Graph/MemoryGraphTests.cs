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

using NUnit.Framework;
using OsmSharp.Math.Structures.Graph;
using System;
using System.Linq;

namespace OsmSharp.Test.Unittests.Math.Structures.Graph
{
    /// <summary>
    /// Contains tests for the memory graph data structure.
    /// </summary>
    [TestFixture]
    public class MemoryGraphTests
    {
        /// <summary>
        /// Tests adding a single vertex.
        /// </summary>
        [Test]
        public void TestAddVertex1()
        {
            var graph = this.CreateNew();
            uint vertex = graph.AddVertex();

            Assert.IsTrue(graph.HasVertex(vertex));
            Assert.AreEqual(1, graph.VertexCount);
        }

        /// <summary>
        /// Tests adding an edge.
        /// </summary>
        [Test]
        public void TestAddEdge1()
        {
            var graph = this.CreateNew();
            uint vertex1 = graph.AddVertex();
            uint vertex2 = graph.AddVertex();
            var edgeData = new uint[] { 1347 };
            long edge = graph.AddEdge(vertex1, vertex2, edgeData);

            // the first edge has id 1.
            Assert.AreEqual(1, edge);

            // see if the edge exists.
            long edgeActual;
            var edgeDataActual = new uint[1];
            Assert.IsTrue(graph.GetEdge(vertex1, vertex2, out edgeActual, ref edgeDataActual));
            Assert.AreEqual(edge, edgeActual);
            Assert.AreEqual(edgeData[0], edgeDataActual[0]);
            Assert.AreEqual(1, graph.EdgeCount);
        }

        /// <summary>
        /// Tests adding two edges.
        /// </summary>
        [Test]
        public void TestAddEdge2()
        {
            var graph = this.CreateNew();
            uint vertex1 = graph.AddVertex();
            uint vertex2 = graph.AddVertex();
            uint vertex3 = graph.AddVertex();
            var edgeData1 = new uint[] { 1347 };
            long edge1 = graph.AddEdge(vertex1, vertex2, edgeData1);
            var edgeData2 = new uint[] { 1348 };
            long edge2 = graph.AddEdge(vertex2, vertex3, edgeData2);

            // the first edge has id 1.
            Assert.AreEqual(1, edge1);
            Assert.AreEqual(2, edge2);

            // see if the edges exist.
            long edgeActual;
            var edgeDataActual = new uint[1];
            Assert.IsTrue(graph.GetEdge(vertex1, vertex2, out edgeActual, ref edgeDataActual));
            Assert.AreEqual(edge1, edgeActual);
            Assert.AreEqual(edgeData1[0], edgeDataActual[0]);
            Assert.IsTrue(graph.GetEdge(vertex2, vertex3, out edgeActual, ref edgeDataActual));
            Assert.AreEqual(edge2, edgeActual);
            Assert.AreEqual(edgeData2[0], edgeDataActual[0]);
            Assert.AreEqual(2, graph.EdgeCount);
        }

        /// <summary>
        /// Tests adding two edges starting in the same vertex.
        /// </summary>
        [Test]
        public void TestAddEdge3()
        {
            var graph = this.CreateNew();
            uint vertex1 = graph.AddVertex();
            uint vertex2 = graph.AddVertex();
            uint vertex3 = graph.AddVertex();
            var edgeData1 = new uint[] { 1347 };
            long edge1 = graph.AddEdge(vertex1, vertex2, edgeData1);
            var edgeData2 = new uint[] { 1348 };
            long edge2 = graph.AddEdge(vertex3, vertex1, edgeData2);

            // the first edge has id 1.
            Assert.AreEqual(1, edge1);
            Assert.AreEqual(2, edge2);

            // see if the edges exist.
            long edgeActual;
            var edgeDataActual = new uint[1];
            Assert.IsTrue(graph.GetEdge(vertex1, vertex2, out edgeActual, ref edgeDataActual));
            Assert.AreEqual(edge1, edgeActual);
            Assert.AreEqual(edgeData1[0], edgeDataActual[0]);
            Assert.IsTrue(graph.GetEdge(vertex3, vertex1, out edgeActual, ref edgeDataActual));
            Assert.AreEqual(edge2, edgeActual);
            Assert.AreEqual(edgeData2[0], edgeDataActual[0]);
            Assert.AreEqual(2, graph.EdgeCount);
        }

        /// <summary>
        /// Tests adding a duplicate but reversed edge.
        /// </summary>
        [Test]
        public void TestAddDuplicateEdge1()
        {
            var graph = this.CreateNew();
            uint vertex1 = graph.AddVertex();
            uint vertex2 = graph.AddVertex();
            var edgeData = new uint[] { 1347 };
            long edge = graph.AddEdge(vertex1, vertex2, edgeData);

            Assert.Catch<InvalidOperationException>(() =>
            {
                long otherEdge = graph.AddEdge(vertex2, vertex1, edgeData);
            });
        }

        /// <summary>
        /// Tests adding a duplicate edge.
        /// </summary>
        [Test]
        public void TestAddDuplicateEdge2()
        {
            var graph = this.CreateNew();
            uint vertex1 = graph.AddVertex();
            uint vertex2 = graph.AddVertex();
            var edgeData = new uint[] { 1347 };
            long edge = graph.AddEdge(vertex1, vertex2, edgeData);

            edgeData = new uint[] { 7431 };
            long otherEdge = graph.AddEdge(vertex1, vertex2, edgeData);
            Assert.AreEqual(edge, otherEdge);

            long edgeActual;
            var edgeDataActual = new uint[1];
            Assert.IsTrue(graph.GetEdge(vertex1, vertex2, out edgeActual, ref edgeDataActual));
            Assert.AreEqual(edge, edgeActual);
            Assert.AreEqual(edgeData[0], edgeDataActual[0]);
        }

        /// <summary>
        /// Tests get edge 1.
        /// </summary>
        [Test]
        public void TestGetEdge1()
        {
            var graph = this.CreateNew();

            uint vertex1 = graph.AddVertex();
            uint vertex2 = graph.AddVertex();
            var edgeData = new uint[] { 1347 };
            long edge = graph.AddEdge(vertex1, vertex2, edgeData);

            // use get edge forward and reverse.
            long edgeActual;
            var edgeDataActual = new uint[1];
            Assert.IsTrue(graph.GetEdge(vertex1, vertex2, out edgeActual, ref edgeDataActual));
            Assert.AreEqual(edge, edgeActual);
            Assert.AreEqual(edgeData[0], edgeDataActual[0]);
            Assert.IsTrue(graph.GetEdge(vertex2, vertex1, out edgeActual, ref edgeDataActual));
            Assert.AreEqual(-edge, edgeActual);
            Assert.AreEqual(edgeData[0], edgeDataActual[0]);
            Assert.AreEqual(1, graph.EdgeCount);
        }

        /// <summary>
        /// Tests get edge 2.
        /// </summary>
        [Test]
        public void TestGetEdge2()
        {
            var graph = this.CreateNew();

            uint vertex1 = graph.AddVertex();
            uint vertex2 = graph.AddVertex();
            var edgeData = new uint[] { 1347 };
            long edge = graph.AddEdge(vertex1, vertex2, edgeData);

            // use get edge forward and reverse.
            var edgeDataActual = new uint[1];
            uint vertex1Actual, vertex2Actual;
            Assert.IsTrue(graph.GetEdge(edge, out vertex1Actual, out vertex2Actual, ref edgeDataActual));
            Assert.AreEqual(vertex1, vertex1Actual);
            Assert.AreEqual(vertex2, vertex2Actual);
            Assert.AreEqual(edgeData[0], edgeDataActual[0]);
        }

        /// <summary>
        /// Tests get edge 3.
        /// </summary>
        [Test]
        public void TestGetEdge3()
        {
            var graph = this.CreateNew();

            uint vertex1 = graph.AddVertex();
            uint vertex2 = graph.AddVertex();
            var edgeData = new uint[] { 1347 };
            long edge = graph.AddEdge(vertex1, vertex2, edgeData);

            // use get edge forward and reverse.
            long edgeActual;
            Assert.IsTrue(graph.GetEdge(vertex1, vertex2, out edgeActual));
            Assert.AreEqual(edge, edgeActual);
            Assert.IsTrue(graph.GetEdge(vertex2, vertex1, out edgeActual));
            Assert.AreEqual(-edge, edgeActual);
        }

        /// <summary>
        /// Tests get edges 1.
        /// </summary>
        [Test]
        public void TestGetEdges1()
        {
            var graph = this.CreateNew();
            uint vertex1 = graph.AddVertex();
            uint vertex2 = graph.AddVertex();
            uint vertex3 = graph.AddVertex();
            var edgeData1 = new uint[] { 1347 };
            long edge1 = graph.AddEdge(vertex1, vertex2, edgeData1);
            var edgeData2 = new uint[] { 1348 };
            long edge2 = graph.AddEdge(vertex3, vertex1, edgeData2);

            var neighbours = new uint[2];
            var data = new uint[2 * graph.EdgeDataSize];
            var edges = new long[2];
            Assert.AreEqual(2, graph.GetEdges(vertex1, ref edges, ref neighbours, ref data));
            Assert.IsTrue(edges.Contains(edge1));
            Assert.IsTrue(edges.Contains(-edge2)); // reverse relative to vertex1
            Assert.IsTrue(neighbours.Contains(vertex2));
            Assert.IsTrue(neighbours.Contains(vertex3));
            Assert.IsTrue(data.Contains(edgeData1[0]));
            Assert.IsTrue(data.Contains(edgeData2[0]));
        }

        /// <summary>
        /// Creates a new empty memory graph.
        /// </summary>
        /// <returns></returns>
        protected virtual MemoryGraph CreateNew()
        {
            return new MemoryGraph(1);
        }
    }
}