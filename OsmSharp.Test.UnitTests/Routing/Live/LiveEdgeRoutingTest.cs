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

using NUnit.Framework;
using OsmSharp.Collections;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Osm.Streams.Filters;
using OsmSharp.Osm.Xml.Streams;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Osm.Streams.Graphs;
using System.Reflection;

namespace OsmSharp.Test.Unittests.Routing.Live
{
    /// <summary>
    /// Tests the sparse node ordering CH.
    /// </summary>
    [TestFixture]
    public class EdgeRoutingTest : SimpleRoutingTests<Edge>
    {
        /// <summary>
        /// Returns a new router.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="interpreter"></param>
        /// <param name="basicRouter"></param>
        /// <returns></returns>
        public override Router BuildRouter(BasicRouterDataSource<Edge> data,
            IRoutingInterpreter interpreter, IBasicRouter<Edge> basicRouter)
        {
            return Router.CreateLiveFrom(data, basicRouter, interpreter);
        }

        /// <summary>
        /// Returns a basic router.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override IBasicRouter<Edge> BuildBasicRouter(BasicRouterDataSource<Edge> data)
        {
            return new DykstraRoutingLive();
        }

        /// <summary>
        /// Builds the data.
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="embeddedString"></param>
        /// <returns></returns>
        public override BasicRouterDataSource<Edge> BuildData(IOsmRoutingInterpreter interpreter, 
            string embeddedString)
        {
            string key = string.Format("Edge.Routing.IBasicRouterDataSource<Edge>.OSM.{0}",
                embeddedString);
            var data = StaticDictionary.Get<BasicRouterDataSource<Edge>>(
                key);
            if (data == null)
            {
                var tagsIndex = new TagsTableCollectionIndex();

                // do the data processing.
                var memoryData = new RouterDataSource<Edge>(tagsIndex);
                var targetData = new LiveGraphOsmStreamTarget(
                    memoryData, interpreter, tagsIndex, null, false);
                var dataProcessorSource = new XmlOsmStreamSource(
                    Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedString));
                var sorter = new OsmStreamFilterSort();
                sorter.RegisterSource(dataProcessorSource);
                targetData.RegisterSource(sorter);
                targetData.Pull();

                data = memoryData;
                StaticDictionary.Add<BasicRouterDataSource<Edge>>(key, data);
            }
            return data;
        }

        /// <summary>
        /// Tests a simple shortest route calculation.
        /// </summary>
        [Test]
        public void TestEdgeShortedDefault()
        {
            this.DoTestShortestDefault();
        }

        /// <summary>
        /// Tests if the raw router preserves tags.
        /// </summary>
        [Test]
        public void TestEdgeResolvedTags()
        {
            this.DoTestResolvedTags();
        }

        /// <summary>
        /// Tests if the raw router preserves tags on arcs/ways.
        /// </summary>
        [Test]
        public void TestEdgeEdgeTags()
        {
            this.DoTestEdgeTags();
        }

        /// <summary>
        /// Test is the CH router can calculate another route.
        /// </summary>
        [Test]
        public void TestEdgeShortest1()
        {
            this.DoTestShortest1();
        }

        /// <summary>
        /// Test is the CH router can calculate another route.
        /// </summary>
        [Test]
        public void TestEdgeShortest2()
        {
            this.DoTestShortest2();
        }

        /// <summary>
        /// Test is the CH router can calculate another route.
        /// </summary>
        [Test]
        public void TestEdgeShortest3()
        {
            this.DoTestShortest3();
        }

        /// <summary>
        /// Test is the CH router can calculate another route.
        /// </summary>
        [Test]
        public void TestEdgeShortest4()
        {
            this.DoTestShortest4();
        }

        /// <summary>
        /// Test is the CH router can calculate another route.
        /// </summary>
        [Test]
        public void TestEdgeShortest5()
        {
            this.DoTestShortest5();
        }

        /// <summary>
        /// Test is the raw router can calculate another route.
        /// </summary>
        [Test]
        public void TestEdgeResolvedShortest1()
        {
            this.DoTestShortestResolved1();
        }

        /// <summary>
        /// Test is the raw router can calculate another route.
        /// </summary>
        [Test]
        public void TestEdgeResolvedShortest2()
        {
            this.DoTestShortestResolved2();
        }

        /// <summary>
        /// Test if the ch router many-to-many weights correspond to the point-to-point weights.
        /// </summary>
        [Test]
        public void TestEdgeManyToMany1()
        {
            this.DoTestManyToMany1();
        }

        /// <summary>
        /// Tests a simple shortest route calculation.
        /// </summary>
        [Test]
        public void TestEdgeResolveAllNodes()
        {
            this.DoTestResolveAllNodes();
        }

        /// <summary>
        /// Tests routing when resolving points.
        /// </summary>
        [Test]
        public void TestEdgeResolveBetweenClose()
        {
            this.DoTestResolveBetweenClose();
        }

        /// <summary>
        /// Tests routing when resolving points.
        /// </summary>
        [Test]
        public void TestEdgeResolveBetweenTwo()
        {
            this.DoTestResolveBetweenTwo();
        }

        /// <summary>
        /// Tests routing when resolving points.
        /// </summary>
        [Test]
        public void TestEdgeResolveCase1()
        {
            this.DoTestResolveCase1();
        }

        /// <summary>
        /// Tests routing when resolving points.
        /// </summary>
        [Test]
        public void TestEdgeResolveCase2()
        {
            this.DoTestResolveCase2();
        }
    }
}