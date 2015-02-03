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

using Ionic.Zlib;
using OsmSharp.Collections.Coordinates.Collections;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Collections.Tags.Serializer.Index;
using OsmSharp.IO;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Tiles;
using OsmSharp.Routing.Contracted.PreProcessing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Graph.Serialization;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Routing.Contracted.Serialization.Sorted
{
    /// <summary>
    /// A v2 routing serializer.
    /// </summary>
    /// <remarks>Versioning is implemented in the file format to guarantee backward compatibility.</remarks>
    public class CHEdgeDataDataSourceSerializer : DataSourceSerializerBase<ContractedEdge>
    {
        /// <summary>
        /// Holds the zoom-level of the regions.
        /// </summary>
        private int _regionZoom = 18;

        /// <summary>
        /// Holds the size of the height-bins to be sorted in.
        /// </summary>
        private int _heightBinSize = 3;

        /// <summary>
        /// Holds the maximum number of vertices in a block.
        /// </summary>
        private int _blockVertexSize = 2;

        /// <summary>
        /// Holds the runtime type model.
        /// </summary>
        private readonly RuntimeTypeModel _runtimeTypeModel;

        /// <summary>
        /// Holds the sorted
        /// </summary>
        private bool _sort = false;

        /// <summary>
        /// Holds the index size.
        /// </summary>
        private const int INDEX_SIZE = 20;

        /// <summary>
        /// Creates a new v2 serializer.
        /// </summary>
        public CHEdgeDataDataSourceSerializer()
            : this(true)
        {

        }

        /// <summary>
        /// Creates a new v2 serializer.
        /// </summary>
        /// <param name="sort"></param>
        public CHEdgeDataDataSourceSerializer(bool sort)
        {
            RuntimeTypeModel typeModel = TypeModel.Create();
            typeModel.Add(typeof(ContractedBlockIndex), true); // the index containing all blocks.
            typeModel.Add(typeof(ContractedBlock), true); // the block definition.
            typeModel.Add(typeof(ContractedBlockCoordinates), true); // the block shapes index.
            typeModel.Add(typeof(ContractedBlockReverse), true); // the block reverse neighbour index.
            typeModel.Add(typeof(ContractedVertex), true); // a vertex definition.
            typeModel.Add(typeof(ContractedEdge), true); // an arc definition.
            typeModel.Add(typeof(ContractedEdgeCoordinates), true); // the shapes.
            typeModel.Add(typeof(ContractedEdgeReverse), true); // the reverse neighbours.

            typeModel.Add(typeof(ContractedVertexRegionIndex), true); // the index containing all regions.
            typeModel.Add(typeof(ContractedVertexRegion), true); // the region definition.

            _runtimeTypeModel = typeModel;
            _sort = sort;
        }

        /// <summary>
        /// Creates a new v2 serializer.
        /// </summary>
        /// <param name="regionZoom"></param>
        /// <param name="heightBinSize"></param>
        /// <param name="blockVertexSize"></param>
        public CHEdgeDataDataSourceSerializer(int regionZoom, int heightBinSize, int blockVertexSize)
            : this(true, regionZoom, heightBinSize, blockVertexSize)
        {

        }

        /// <summary>
        /// Creates a new v2 serializer.
        /// </summary>
        /// <param name="sort"></param>
        /// <param name="regionZoom"></param>
        /// <param name="heightBinSize"></param>
        /// <param name="blockVertexSize"></param>
        public CHEdgeDataDataSourceSerializer(bool sort, int regionZoom, int heightBinSize, int blockVertexSize)
        {
            _regionZoom = regionZoom;
            _heightBinSize = heightBinSize;
            _blockVertexSize = blockVertexSize;

            RuntimeTypeModel typeModel = TypeModel.Create();
            typeModel.Add(typeof(ContractedBlockIndex), true); // the index containing all blocks.
            typeModel.Add(typeof(ContractedBlock), true); // the block definition.
            typeModel.Add(typeof(ContractedBlockCoordinates), true); // the block shapes index.
            typeModel.Add(typeof(ContractedBlockReverse), true); // the block reverse neighbour index.
            typeModel.Add(typeof(ContractedVertex), true); // a vertex definition.
            typeModel.Add(typeof(ContractedEdge), true); // an arc definition.
            typeModel.Add(typeof(ContractedEdgeCoordinates), true); // the shapes.
            typeModel.Add(typeof(ContractedEdgeReverse), true); // the reverse neighbours.

            typeModel.Add(typeof(ContractedVertexRegionIndex), true); // the index containing all regions.
            typeModel.Add(typeof(ContractedVertexRegion), true); // the region definition.

            _runtimeTypeModel = typeModel;
            _sort = sort;
        }

        /// <summary>
        /// Returns the version string.
        /// </summary>
        public override string VersionString
        {
            get { return "CHEdgeData.v3"; }
        }

        /// <summary>
        /// Does the v2 serialization.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="graph"></param>
        /// <returns></returns>
        protected override void DoSerialize(LimitedStream stream, RouterDataSource<ContractedEdge> graph)
        {
            // sort the graph.
            var sortedGraph = graph;

            // create the regions.
            var regions = new SortedDictionary<ulong, List<uint>>();
            for (uint newVertexId = 1; newVertexId < sortedGraph.VertexCount + 1; newVertexId++)
            {
                // add to the CHRegions.
                float latitude, longitude;
                sortedGraph.GetVertex(newVertexId, out latitude, out longitude);
                var tile = Tile.CreateAroundLocation(new GeoCoordinate(
                    latitude, longitude), _regionZoom);
                List<uint> regionVertices;
                if (!regions.TryGetValue(tile.Id, out regionVertices))
                {
                    regionVertices = new List<uint>();
                    regions.Add(tile.Id, regionVertices);
                }
                regionVertices.Add(newVertexId);
            }

            // serialize the sorted graph.
            // [START_OF_BLOCKS][START_OF_SHAPES][START_OF_REVERSE][START_OF_TAGS][[SIZE_OF_REGION_INDEX][REGION_INDEX][REGIONS]]
            //                                                                    [[SIZE_OF_BLOCK_INDEX][BLOCK_INDEX][BLOCKS]]
            //                                                                    [[SIZE_OF_SHAPE_INDEX][SHAPE_INDEX][SHAPES]]
            //                                                                    [[SIZE_OF_REVERSE_INDEX][REVERSE_INDEX][REVERSE]]
            //                                                                    [TAGS]
            // START_OF_BLOCKS:         4bytes
            // START_OF_SHAPES:         4bytes
            // START_OF_STAGS:          4bytes
            // START_OF_REVERSE         4bytes

            // SIZE_OF_REGION_INDEX:    4bytes
            // REGION_INDEX:            see SIZE_OF_REGION_INDEX
            // REGIONS:                 see START_OF_BLOCKS - 4bytes

            // SIZE_OF_BLOCK_INDEX:     4bytes
            // BLOCK_INDEX:             see SIZE_OF_BLOCK_INDEX.
            // BLOCKS:                  from START_OF_BLOCKS + 4bytes + SIZE_OF_BLOCKS_INDEX until END.

            // serialize regions and build their index.
            var chRegionIndex = new ContractedVertexRegionIndex();
            chRegionIndex.LocationIndex = new int[regions.Count];
            chRegionIndex.RegionIds = new ulong[regions.Count];
            var memoryStream = new MemoryStream();
            int regionIdx = 0;
            foreach (var region in regions)
            {
                // serialize.
                var chRegion = new ContractedVertexRegion();
                chRegion.Vertices = region.Value.ToArray();
                _runtimeTypeModel.Serialize(memoryStream, chRegion);

                // set index.
                chRegionIndex.LocationIndex[regionIdx] = (int)memoryStream.Position;
                chRegionIndex.RegionIds[regionIdx] = region.Key;
                regionIdx++;
            }
            stream.Seek(INDEX_SIZE, SeekOrigin.Begin); // move to beginning of [REGION_INDEX]
            _runtimeTypeModel.Serialize(stream, chRegionIndex); // write region index.
            var sizeRegionIndex = (int)(stream.Position - INDEX_SIZE); // now at beginning of [REGIONS]
            memoryStream.Seek(0, SeekOrigin.Begin);
            memoryStream.WriteTo(stream); // write regions.
            memoryStream.Dispose();
            var startOfBlocks = (int)stream.Position; // now at beginning of [SIZE_OF_BLOCK_INDEX]

            // serialize the blocks and build their index.
            memoryStream = new MemoryStream();
            var blocks = new List<ContractedBlock>();
            var blockLocations = new List<int>();
            var blockShapeLocations = new List<int>();
            var blockReverseLocations = new List<int>();
            var blockShapes = new List<ContractedBlockCoordinates>();
            var reverseNeighbours = new Dictionary<uint, List<uint>>();
            uint vertexId = 1;
            while (vertexId < sortedGraph.VertexCount + 1)
            {
                uint blockId = vertexId;
                var blockArcs = new List<ContractedEdgeSimple>();
                var blockArcCoordinates = new List<ContractedEdgeCoordinates>();
                var blockVertices = new List<ContractedVertex>();
                while (vertexId < blockId + _blockVertexSize &&
                    vertexId < sortedGraph.VertexCount + 1)
                { // create this block.
                    var chVertex = new ContractedVertex();
                    float latitude, longitude;
                    sortedGraph.GetVertex(vertexId, out latitude, out longitude);
                    chVertex.Latitude = latitude;
                    chVertex.Longitude = longitude;
                    chVertex.EdgeIndex = (ushort)(blockArcs.Count);
                    List<uint> reverse = null;
                    foreach (var sortedArc in sortedGraph.GetEdges(vertexId))
                    {
                        var chArc = new ContractedEdgeSimple();
                        chArc.TargetId = sortedArc.Neighbour;
                        chArc.Meta = sortedArc.EdgeData.Meta;
                        chArc.Value = sortedArc.EdgeData.Value;
                        chArc.Weight = sortedArc.EdgeData.Weight;
                        blockArcs.Add(chArc);

                        var chArcCoordinates = new ContractedEdgeCoordinates();
                        chArcCoordinates.Coordinates = sortedArc.Intermediates.ToSimpleArray();
                        blockArcCoordinates.Add(chArcCoordinates);

                        if (sortedArc.EdgeData.RepresentsNeighbourRelations)
                        {
                            if (!reverseNeighbours.TryGetValue(sortedArc.Neighbour, out reverse))
                            {
                                reverse = new List<uint>();
                                reverseNeighbours[sortedArc.Neighbour] = reverse;
                            }
                            reverse.Add(vertexId);
                        }
                    }
                    chVertex.EdgeCount = (ushort)(blockArcs.Count - chVertex.EdgeIndex);
                    blockVertices.Add(chVertex);

                    vertexId++; // move to the next vertex.
                }

                // create block.
                var block = new ContractedBlock();
                block.Edges = blockArcs.ToArray();
                block.Vertices = blockVertices.ToArray(); // TODO: get rid of the list and create an array to begin with.
                blocks.Add(block);
                var blockCoordinates = new ContractedBlockCoordinates();
                blockCoordinates.Edges = blockArcCoordinates.ToArray();

                // write blocks.
                var blockStream = new MemoryStream();
                _runtimeTypeModel.Serialize(blockStream, block);
                var compressed = GZipStream.CompressBuffer(blockStream.ToArray());
                blockStream.Dispose();

                memoryStream.Write(compressed, 0, compressed.Length);
                blockLocations.Add((int)memoryStream.Position);

                blockShapes.Add(blockCoordinates);
            }
            var blockIndex = new ContractedBlockIndex();
            blockIndex.BlockLocationIndex = blockLocations.ToArray();

            stream.Seek(startOfBlocks + 4, SeekOrigin.Begin); // move to beginning of [BLOCK_INDEX]
            _runtimeTypeModel.Serialize(stream, blockIndex); // write block index.
            int sizeBlockIndex = (int)(stream.Position - (startOfBlocks + 4)); // now at beginning of [BLOCKS]
            memoryStream.Seek(0, SeekOrigin.Begin);
            memoryStream.WriteTo(stream); // write blocks.
            memoryStream.Dispose();

            // write shapes and index.
            memoryStream = new MemoryStream();
            var startOfShapes = stream.Position;
            foreach (var blockCoordinates in blockShapes)
            {
                // write shape blocks.
                var blockShapeStream = new MemoryStream();
                _runtimeTypeModel.Serialize(blockShapeStream, blockCoordinates);
                var compressed = GZipStream.CompressBuffer(blockShapeStream.ToArray());
                blockShapeStream.Dispose();

                memoryStream.Write(compressed, 0, compressed.Length);
                blockShapeLocations.Add((int)memoryStream.Position);
            }
            blockIndex = new ContractedBlockIndex();
            blockIndex.BlockLocationIndex = blockShapeLocations.ToArray();

            stream.Seek(startOfShapes + 4, SeekOrigin.Begin); // move to beginning of [SHAPE_INDEX]
            _runtimeTypeModel.Serialize(stream, blockIndex); // write shape index.
            int sizeShapeIndex = (int)(stream.Position - (startOfShapes + 4)); // now at beginning of [SHAPE]
            memoryStream.Seek(0, SeekOrigin.Begin);
            memoryStream.WriteTo(stream); // write shapes.
            memoryStream.Dispose();

            // build reverse blocks.
            var blockReverses = new List<ContractedBlockReverse>();
            List<uint> storedReverses = null;
            uint currentVertexId = 0;
            for (int i = 0; i < blocks.Count; i++)
            {
                var blockReverse = new ContractedBlockReverse();
                blockReverse.Vertices = new ContractedEdgeReverse[blocks[i].Vertices.Length];
                for(int j = 0; j <  blocks[i].Vertices.Length; j++)
                {
                    currentVertexId++;
                    var blockReverseArc = new ContractedEdgeReverse();
                    blockReverseArc.Neighbours = new uint[0];
                    if (reverseNeighbours.TryGetValue(currentVertexId, out storedReverses))
                    {
                        blockReverseArc.Neighbours = storedReverses.ToArray();
                    }
                    blockReverse.Vertices[j] = blockReverseArc;
                }
                blockReverses.Add(blockReverse);
            }

            // write reverse and index.
            memoryStream = new MemoryStream();
            var startOfReverses = stream.Position;
            foreach (var blockReverse in blockReverses)
            {
                // write reverse blocks.
                var blockReverseStream = new MemoryStream();
                _runtimeTypeModel.Serialize(blockReverseStream, blockReverse);
                var compressed = GZipStream.CompressBuffer(blockReverseStream.ToArray());
                blockReverseStream.Dispose();

                memoryStream.Write(compressed, 0, compressed.Length);
                blockReverseLocations.Add((int)memoryStream.Position);
            }
            blockIndex = new ContractedBlockIndex();
            blockIndex.BlockLocationIndex = blockReverseLocations.ToArray();

            stream.Seek(startOfReverses + 4, SeekOrigin.Begin); // move to beginning of [REVERSE_INDEX]
            _runtimeTypeModel.Serialize(stream, blockIndex); // write reverse index.
            int sizeReverseIndex = (int)(stream.Position - (startOfReverses + 4)); // now at beginning of [REVERSE]
            memoryStream.Seek(0, SeekOrigin.Begin);
            memoryStream.WriteTo(stream); // write reverses.
            memoryStream.Dispose();

            // write tags after reverses.
            int tagsPosition = (int)stream.Position;
            TagIndexSerializer.SerializeBlocks(stream, graph.TagsIndex, 10);

            stream.Seek(startOfBlocks, SeekOrigin.Begin); // move to [SIZE_OF_BLOCK_INDEX]
            stream.Write(BitConverter.GetBytes(sizeBlockIndex), 0, 4); // write start position of blocks.
            stream.Seek(startOfShapes, SeekOrigin.Begin); // move to [SIZE_OF_BLOCK_INDEX]
            stream.Write(BitConverter.GetBytes(sizeShapeIndex), 0, 4); // write start position of blocks.
            stream.Seek(startOfReverses, SeekOrigin.Begin); // move to [SIZE_OF_REVERSE_INDEX]
            stream.Write(BitConverter.GetBytes(sizeReverseIndex), 0, 4); // write start position of blocks.

            // write index.
            stream.Seek(0, SeekOrigin.Begin); // move to beginning
            stream.Write(BitConverter.GetBytes(startOfBlocks), 0, 4); // write start position of blocks. 
            stream.Write(BitConverter.GetBytes(startOfShapes), 0, 4); // write start position of shapes. 
            stream.Write(BitConverter.GetBytes(startOfReverses), 0, 4); // write start position of shapes. 
            stream.Write(BitConverter.GetBytes(tagsPosition), 0, 4);
            stream.Write(BitConverter.GetBytes(sizeRegionIndex), 0, 4); // write size of region index.
            stream.Flush();
        }

        /// <summary>
        /// Searches for a vertex and returns it's new id.
        /// </summary>
        /// <param name="oldVertexId"></param>
        /// <param name="currentBin"></param>
        /// <param name="heightBins"></param>
        /// <returns></returns>
        private static uint SearchVertex(uint oldVertexId, Dictionary<uint, uint> currentBin, List<uint>[] heightBins)
        {
            uint newVertexId;
            if (!currentBin.TryGetValue(oldVertexId, out newVertexId))
            { // search vertex somewhere in the higher bins.
                uint currentBinStartId = 1;
                for (int binIdx = 0; binIdx < heightBins.Length; binIdx++)
                {
                    if (heightBins[binIdx] != null)
                    {
                        int indexOf = heightBins[binIdx].IndexOf(oldVertexId);
                        if (indexOf > 0)
                        { // vertex was found in this bin.
                            newVertexId = currentBinStartId + (uint)indexOf;
                            break;
                        }
                        currentBinStartId = currentBinStartId +
                            (uint)heightBins[binIdx].Count;
                    }
                }
            }
            return newVertexId;
        }

        /// <summary>
        /// Does the v2 deserialization.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="lazy"></param>
        /// <param name="vehicles"></param>
        /// <returns></returns>
        protected override BasicRouterDataSource<ContractedEdge> DoDeserialize(
            LimitedStream stream, bool lazy, IEnumerable<string> vehicles)
        {
            var intBytes = new byte[4];
            stream.Read(intBytes, 0, 4);
            int startOfBlocks = BitConverter.ToInt32(intBytes, 0);
            stream.Read(intBytes, 0, 4);
            int startOfShapes = BitConverter.ToInt32(intBytes, 0);
            stream.Read(intBytes, 0, 4);
            int startOfReverses = BitConverter.ToInt32(intBytes, 0);
            stream.Read(intBytes, 0, 4);
            int startOfTags = BitConverter.ToInt32(intBytes, 0);
            stream.Read(intBytes, 0, 4);
            int sizeRegionIndex = BitConverter.ToInt32(intBytes, 0);

            // deserialize regions index.
            var chVertexRegionIndex = (ContractedVertexRegionIndex)_runtimeTypeModel.Deserialize(
                new CappedStream(stream, stream.Position, sizeRegionIndex), null,
                    typeof(ContractedVertexRegionIndex));

            // deserialize blocks index.
            stream.Seek(startOfBlocks, SeekOrigin.Begin);
            stream.Read(intBytes, 0, 4);
            int sizeBlockIndex = BitConverter.ToInt32(intBytes, 0);
            var blockIndex = (ContractedBlockIndex)_runtimeTypeModel.Deserialize(
                new CappedStream(stream, stream.Position, sizeBlockIndex), null,
                    typeof(ContractedBlockIndex));

            // deserialize shapes index.
            stream.Seek(startOfShapes, SeekOrigin.Begin);
            stream.Read(intBytes, 0, 4);
            int sizeShapesIndex = BitConverter.ToInt32(intBytes, 0);
            var shapesIndex = (ContractedBlockIndex)_runtimeTypeModel.Deserialize(
                new CappedStream(stream, stream.Position, sizeShapesIndex), null,
                    typeof(ContractedBlockIndex));

            // deserialize reverses index.
            stream.Seek(startOfReverses, SeekOrigin.Begin);
            stream.Read(intBytes, 0, 4);
            int sizeReversesIndex = BitConverter.ToInt32(intBytes, 0);
            var reversesIndex = (ContractedBlockIndex)_runtimeTypeModel.Deserialize(
                new CappedStream(stream, stream.Position, sizeReversesIndex), null,
                    typeof(ContractedBlockIndex));

            // deserialize tags.
            stream.Seek(startOfTags, SeekOrigin.Begin);
            var tagsIndex = TagIndexSerializer.DeserializeBlocks(stream);

            return new ContractedDataDataSource(stream, this, vehicles, sizeRegionIndex + INDEX_SIZE,
                chVertexRegionIndex, _regionZoom,
                startOfBlocks + sizeBlockIndex + 4, blockIndex, (uint)_blockVertexSize,
                startOfShapes + sizeShapesIndex + 4, shapesIndex,
                startOfReverses + sizeReversesIndex + 4, reversesIndex,
                tagsIndex);
        }

        /// <summary>
        /// Deserializes the given block of data.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="decompress"></param>
        /// <returns></returns>
        internal ContractedBlock DeserializeBlock(Stream stream, long offset, int length, bool decompress)
        {
            ContractedBlock value = null;
            if (decompress)
            { // decompress the data.
                var buffer = new byte[length];
                stream.Seek(offset, SeekOrigin.Begin);
                stream.Read(buffer, 0, length);

                var memoryStream = new MemoryStream(buffer);
                var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
                value = (ContractedBlock)_runtimeTypeModel.Deserialize(gZipStream, null, typeof(ContractedBlock));
            }
            else
            {
                value = (ContractedBlock)_runtimeTypeModel.Deserialize(
                    new CappedStream(stream, offset, length), null,
                        typeof(ContractedBlock));
            }
            return value;
        }

        /// <summary>
        /// Deserializes the given block shape of data.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="decompress"></param>
        /// <returns></returns>
        internal ContractedBlockCoordinates DeserializeBlockShape(Stream stream, long offset, int length, bool decompress)
        {
            ContractedBlockCoordinates value = null;
            if (decompress)
            { // decompress the data.
                var buffer = new byte[length];
                stream.Seek(offset, SeekOrigin.Begin);
                stream.Read(buffer, 0, length);

                var memoryStream = new MemoryStream(buffer);
                var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
                value = (ContractedBlockCoordinates)_runtimeTypeModel.Deserialize(gZipStream, null, typeof(ContractedBlockCoordinates));
            }
            else
            {
                value = (ContractedBlockCoordinates)_runtimeTypeModel.Deserialize(
                        new CappedStream(stream, offset, length), null,
                            typeof(ContractedBlockCoordinates));
            }
            return value;
        }

        /// <summary>
        /// Deserializes the given block reverse of data.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="decompress"></param>
        /// <returns></returns>
        internal ContractedBlockReverse DeserializeBlockReverse(Stream stream, int offset, int length, bool decompress)
        {
            ContractedBlockReverse value = null;
            if (decompress)
            { // decompress the data.
                var buffer = new byte[length];
                stream.Seek(offset, SeekOrigin.Begin);
                stream.Read(buffer, 0, length);

                var memoryStream = new MemoryStream(buffer);
                var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
                value = (ContractedBlockReverse)_runtimeTypeModel.Deserialize(gZipStream, null, typeof(ContractedBlockReverse));
            }
            else
            {
                value = (ContractedBlockReverse)_runtimeTypeModel.Deserialize(
                        new CappedStream(stream, offset, length), null,
                            typeof(ContractedBlockReverse));
            }
            return value;
        }

        /// <summary>
        /// Deserialize the given region of data.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="decompress"></param>
        /// <returns></returns>
        internal ContractedVertexRegion DeserializeRegion(Stream stream, long offset, int length, bool decompress)
        {
            if (decompress)
            { // decompress the data.
                var buffer = new byte[length];
                stream.Seek(offset, SeekOrigin.Begin);
                stream.Read(buffer, 0, length);

                var memoryStream = new MemoryStream(buffer);
                var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
                return (ContractedVertexRegion)_runtimeTypeModel.Deserialize(gZipStream, null,
                                                                             typeof(ContractedVertexRegion));
            }
            return (ContractedVertexRegion)_runtimeTypeModel.Deserialize(
                new CappedStream(stream, offset, length), null,
                    typeof(ContractedVertexRegion));
        }
    }
}