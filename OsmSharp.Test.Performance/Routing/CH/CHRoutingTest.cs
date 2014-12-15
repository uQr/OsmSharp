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

using OsmSharp.Routing;
using System.IO;
using OsmSharp.Osm.PBF.Streams;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Math.Geo;
using OsmSharp.Routing.Instructions;
using System.Collections.Generic;
using OsmSharp.Collections.Tags;
using OsmSharp.Logging;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Osm.Streams.Filters;
using OsmSharp.IO.MemoryMappedFiles;
using OsmSharp.Routing.Graph;
using OsmSharp.Collections.Coordinates;
using OsmSharp.Routing.Osm.Streams.Graphs;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.CH.PreProcessing.Witnesses;
using OsmSharp.Routing.CH.PreProcessing.Ordering;

namespace OsmSharp.Test.Performance.Routing.CH
{
    /// <summary>
    /// Contains test for the CH routing.
    /// </summary>
    public static class CHRoutingTest
    {
        /// <summary>
        /// Tests the live routing.
        /// </summary>
        public static void Test()
        {
            var box = new GeoCoordinateBox(
                new GeoCoordinate(51.20190, 4.66540),
                new GeoCoordinate(51.30720, 4.89820));
            CHRoutingTest.TestRouting("CHRouting",
                "kempen.osm.pbf", box, 500);
        }

        /// <summary>
        /// Tests routing from a serialized routing file.
        /// </summary>
        public static void Test(Stream stream, int testCount)
        {
            var box = new GeoCoordinateBox(
                new GeoCoordinate(51.20190, 4.66540),
                new GeoCoordinate(51.30720, 4.89820));
            CHRoutingTest.TestSerializedRouting("CHRouting",
                stream, box, testCount);
        }

        /// <summary>
        /// Tests the live routing.
        /// </summary>
        public static void TestMemoryMapped()
        {
            var box = new GeoCoordinateBox(
                new GeoCoordinate(51.20190, 4.66540),
                new GeoCoordinate(51.30720, 4.89820));
            CHRoutingTest.TestMemoryMappedRouting("CHRouting",
                "kempen.osm.pbf", box, 500);
        }

        /// <summary>
        /// Tests routing from a serialized routing file.
        /// </summary>
        public static void TestMemoryMappedRouting(string name, string routeFile,
            GeoCoordinateBox box, int testCount)
        {
            var testFile = new FileInfo(string.Format(@".\TestFiles\{0}", routeFile));
            var stream = testFile.OpenRead();

            CHRoutingTest.TestMemoryMappedRouting(name, stream, box, testCount);

            stream.Dispose();
        }

        /// <summary>
        /// Tests routing from a serialized routing file.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="stream"></param>
        /// <param name="box"></param>
        /// <param name="testCount"></param>
        public static void TestSerializedRouting(string name, Stream stream,
            GeoCoordinateBox box, int testCount)
        {
            var router = Router.CreateCHFrom(new OsmSharp.Osm.PBF.Streams.PBFOsmStreamSource(stream),
                new OsmRoutingInterpreter(), Vehicle.Car);

            var performanceInfo = new PerformanceInfoConsumer("CHRouting");
            performanceInfo.Start();
            performanceInfo.Report("Routing {0} routes...", testCount);

            int successCount = 0;
            int totalCount = testCount;
            float latestProgress = -1;
            while (testCount > 0)
            {
                var from = box.GenerateRandomIn();
                var to = box.GenerateRandomIn();

                var fromPoint = router.Resolve(Vehicle.Car, from);
                var toPoint = router.Resolve(Vehicle.Car, to);

                if (fromPoint != null && toPoint != null)
                {
                    var route = router.Calculate(Vehicle.Car, fromPoint, toPoint, float.MaxValue, true);
                    if (route != null)
                    {
                        successCount++;
                    }
                }

                testCount--;

                // report progress.
                float progress = (float)System.Math.Round(((double)(totalCount - testCount)  / (double)totalCount) * 100);
                if (progress != latestProgress)
                {
                    OsmSharp.Logging.Log.TraceEvent("CHRouting", TraceEventType.Information,
                        "Routing... {0}%", progress);
                    latestProgress = progress;
                }
            }
            performanceInfo.Stop();

            OsmSharp.Logging.Log.TraceEvent("CHRouting", OsmSharp.Logging.TraceEventType.Information,
                string.Format("{0}/{1} routes successfull!", successCount, totalCount));
        }

        /// <summary>
        /// Tests routing from a serialized routing file.
        /// </summary>
        public static void TestRouting(string name, string osmPbfFile, 
            GeoCoordinateBox box, int testCount)
        {
            var testFile = new FileInfo(string.Format(@".\TestFiles\{0}", osmPbfFile));
            var stream = testFile.OpenRead();

            CHRoutingTest.TestSerializedRouting(name, stream, box, testCount);

            stream.Dispose();
        }

        /// <summary>
        /// Tests routing from a serialized routing file.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="stream"></param>
        /// <param name="box"></param>
        /// <param name="testCount"></param>
        public static void TestMemoryMappedRouting(string name, Stream stream,
            GeoCoordinateBox box, int testCount)
        {
            var tagsIndex = new TagsTableCollectionIndex();
            var source = new PBFOsmStreamSource(stream);
            var progressFilter = new OsmStreamFilterProgress();
            progressFilter.RegisterSource(source);

            // read from the OSM-stream.
            using (var fileFactory = new MemoryMappedFileFactory(@"c:\temp\"))
            {
                using (var memoryMappedGraph = new MemoryMappedGraph<CHEdgeData>(10000, fileFactory))
                {
                    //using (var coordinates = new HugeCoordinateIndex(fileFactory, 10000))
                    //{
                    var memoryData = new DynamicGraphRouterDataSource<CHEdgeData>(memoryMappedGraph, tagsIndex);
                    var targetData = new CHEdgeGraphOsmStreamTarget(memoryData, new OsmRoutingInterpreter(), tagsIndex, Vehicle.Car);
                    targetData.RegisterSource(progressFilter);
                    targetData.Pull();

                    // compress the graph.
                    var witnessCalculator = new DykstraWitnessCalculator();
                    var edgeDifference = new EdgeDifferenceContractedSearchSpace(memoryData, witnessCalculator);
                    var preProcessor = new CHPreProcessor(memoryData, edgeDifference, witnessCalculator);
                    preProcessor.Start();

                    // drop vertex index.
                    memoryData.DropVertexIndex();
                    memoryMappedGraph.SortHilbert(1000);

                    var router = Router.CreateCHFrom(memoryData, new OsmSharp.Routing.CH.CHRouter(), 
                       new OsmRoutingInterpreter());

                    var performanceInfo = new PerformanceInfoConsumer("CHRouting");
                    performanceInfo.Start();
                    performanceInfo.Report("Routing {0} routes...", testCount);

                    int successCount = 0;
                    int totalCount = testCount;
                    float latestProgress = -1;
                    while (testCount > 0)
                    {
                        var from = box.GenerateRandomIn();
                        var to = box.GenerateRandomIn();

                        var fromPoint = router.Resolve(Vehicle.Car, from);
                        var toPoint = router.Resolve(Vehicle.Car, to);

                        if (fromPoint != null && toPoint != null)
                        {
                            var route = router.Calculate(Vehicle.Car, fromPoint, toPoint);
                            if (route != null)
                            {
                                successCount++;
                            }
                        }

                        testCount--;

                        // report progress.
                        float progress = (float)System.Math.Round(((double)(totalCount - testCount) / (double)totalCount) * 100);
                        if (progress != latestProgress)
                        {
                            OsmSharp.Logging.Log.TraceEvent("CHRouting", TraceEventType.Information,
                                "Routing... {0}%", progress);
                            latestProgress = progress;
                        }
                    }
                    performanceInfo.Stop();
                    //}

                    OsmSharp.Logging.Log.TraceEvent("CHRouting", OsmSharp.Logging.TraceEventType.Information,
                        string.Format("{0}/{1} routes successfull!", successCount, totalCount));
                }
            }

        }
    }
}