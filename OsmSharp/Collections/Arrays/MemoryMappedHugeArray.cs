// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2014 Abelshausen Ben
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

using OsmSharp.Collections.Cache;
using OsmSharp.IO.MemoryMappedFiles;
using System;
using System.Collections.Generic;

namespace OsmSharp.Collections.Arrays
{
    /// <summary>
    /// Represents a memory mapped huge array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MemoryMappedHugeArray<T> : IHugeArray<T>
        where T : struct
    {
        /// <summary>
        /// Holds the length of this array.
        /// </summary>
        private long _length;

        /// <summary>
        /// Holds the factory to create the memory mapped files.
        /// </summary>
        private MemoryMappedFileFactory _factory;

        /// <summary>
        /// Holds the list of files for each range.
        /// </summary>
        private List<IMemoryMappedFile> _files;

        /// <summary>
        /// Holds the list of accessors for each file.
        /// </summary>
        private List<IMemoryMappedViewAccessor> _accessors;

        /// <summary>
        /// Holds the default file element size.
        /// </summary>
        //private static long DefaultFileElementSize = (long)1024 * (long)1024 * (long)128;
        private static long DefaultFileElementSize = (long)1024 * (long)128;

        /// <summary>
        /// Holds the file element size.
        /// </summary>
        private long _fileElementSize = DefaultFileElementSize;

        /// <summary>
        /// Holds the element size.
        /// </summary>
        private int _elementSize;

        /// <summary>
        /// Holds the maximum array size in bytes.
        /// </summary>
        private long _fileSizeBytes;

        /// <summary>
        /// Holds the size of one cache block.
        /// </summary>
        private int _cacheBlockSize;

        /// <summary>
        /// Holds the cache.
        /// </summary>
        private LRUCache<long, T[]> _cache;

        /// <summary>
        /// Creates a memory mapped huge array.
        /// </summary>
        /// <param name="size">The size of the array.</param>
        public MemoryMappedHugeArray(long size)
            : this(size, DefaultFileElementSize)
        {

        }

        /// <summary>
        /// Creates a memory mapped huge array.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="arraySize"></param>
        public MemoryMappedHugeArray(long size, long arraySize)
            : this(new MemoryMappedFileFactory(), size, arraySize)
        {

        }

        /// <summary>
        /// Creates a memory mapped huge array.
        /// </summary>
        /// <param name="factory">The factory to create the memory mapped files.</param>
        /// <param name="size">The size of the array.</param>
        public MemoryMappedHugeArray(MemoryMappedFileFactory factory, long size)
            : this(factory, size, DefaultFileElementSize)
        {

        }

        /// <summary>
        /// Creates a memory mapped huge array.
        /// </summary>
        /// <param name="factory">The factory to create the memory mapped files.</param>
        /// <param name="size">The size of the array.</param>
        /// <param name="arraySize">The size of an indivdual array block.</param>
        public MemoryMappedHugeArray(MemoryMappedFileFactory factory, long size, long arraySize)
        {
            _factory = factory;
            _length = size;
            _fileElementSize = arraySize;
            _elementSize = NativeMemoryMappedFileFactory.GetSize(typeof(T));
            _fileSizeBytes = arraySize * _elementSize;

            _cacheBlockSize = 32;
            if (arraySize % _cacheBlockSize != 0)
            { // cacheblock size needs to be a multiple of arraySize.
                int n = (int)(arraySize / _cacheBlockSize) + 1;
                _cacheBlockSize = (int)(n * arraySize);
            }
            _cache = new LRUCache<long, T[]>(1000);

            var arrayCount = (int)System.Math.Ceiling((double)size / _fileElementSize);
            _files = new List<IMemoryMappedFile>(arrayCount);
            _accessors = new List<IMemoryMappedViewAccessor>(arrayCount);
            for (int arrayIdx = 0; arrayIdx < arrayCount; arrayIdx++)
            {
                var file = _factory.New(_fileSizeBytes);
                _files.Add(file);
                _accessors.Add(file.CreateViewAccessor(0, _fileSizeBytes));
            }
        }

        /// <summary>
        /// Creates a memory mapped huge array.
        /// </summary>
        /// <param name="fileName">The file to use as a mapping.</param>
        /// <param name="size">The size of the array.</param>
        public MemoryMappedHugeArray(string fileName, long size)
            : this(fileName, 0, size, DefaultFileElementSize)
        {

        }

        /// <summary>
        /// Creates a memory mapped huge array.
        /// </summary>
        /// <param name="fileName">The file to use as a mapping.</param>
        /// <param name="offset">The offset in the file.</param>
        /// <param name="size">The size of the array.</param>
        public MemoryMappedHugeArray(string fileName, long offset, long size)
            : this(fileName, offset, size, DefaultFileElementSize)
        {

        }

        /// <summary>
        /// Creates a memory mapped huge array.
        /// </summary>
        /// <param name="fileName">The file to use as a mapping.</param>
        /// <param name="offset">The offset in the file.</param>
        /// <param name="size">The size of the array.</param>
        /// <param name="arraySize">The size of an indivdual array block.</param>
        public MemoryMappedHugeArray(string fileName, long offset, long size, long arraySize)
        {
            _length = size;
            _fileElementSize = arraySize;
            _elementSize = NativeMemoryMappedFileFactory.GetSize(typeof(T));
            _fileSizeBytes = arraySize * _elementSize;

            var arrayCount = (int)System.Math.Ceiling((double)size / _fileElementSize);
            _files = new List<IMemoryMappedFile>(arrayCount);
            _accessors = new List<IMemoryMappedViewAccessor>(arrayCount);

            var file = MemoryMappedFileFactory.New(fileName, _fileSizeBytes, offset);
            _files.Add(file);
            _accessors.Add(file.CreateViewAccessor(0, _fileSizeBytes));
        }

        /// <summary>
        /// Returns the length of this array.
        /// </summary>
        public long Length
        {
            get { return _length; }
        }

        /// <summary>
        /// Returns true if this array can be resized.
        /// </summary>
        public bool CanResize
        { // when factory is null, one fixed size file is used and no resizing is possible.
            get { return _factory != null ; }
        }

        /// <summary>
        /// Resizes this array.
        /// </summary>
        /// <param name="size"></param>
        public void Resize(long size)
        {
            _length = size;
            _cache.Clear();

            var arrayCount = (int)System.Math.Ceiling((double)size / _fileElementSize);
            if (arrayCount < _files.Count)
            { // decrease files/accessors.
                for (int arrayIdx = (int)arrayCount; arrayIdx < _files.Count; arrayIdx++)
                {
                    _accessors[arrayIdx].Dispose();
                    _accessors[arrayIdx] = null;
                    _files[arrayIdx].Dispose();
                    _files[arrayIdx] = null;
                }
                _files.RemoveRange((int)arrayCount, (int)(_files.Count - arrayCount));
                _accessors.RemoveRange((int)arrayCount, (int)(_accessors.Count - arrayCount));
            }
            else
            { // increase files/accessors.
                for (int arrayIdx = _files.Count; arrayIdx < arrayCount; arrayIdx++)
                {
                    var file = _factory.New(_fileSizeBytes);
                    _files.Add(file);
                    _accessors.Add(file.CreateViewAccessor(0, _fileSizeBytes));
                }
            }
        }

        /// <summary>
        /// Returns the element at the given index.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public T this[long idx]
        {
            get
            {
                // calculate cacheblock and see if it's in cach.
                long cacheBlockId = idx - (idx % _cacheBlockSize);
                T[] cacheBlock;
                if (!_cache.TryGet(cacheBlockId, out cacheBlock))
                { // not in cache.
                    cacheBlock = new T[_cacheBlockSize];

                    long arrayIdx = (long)System.Math.Floor(cacheBlockId / _fileElementSize);
                    long localIdx = cacheBlockId % _fileElementSize;
                    long localPosition = localIdx * _elementSize;
                    _accessors[(int)arrayIdx].ReadArray<T>(localPosition, cacheBlock, 0, cacheBlock.Length);
                    _cache.Add(cacheBlockId, cacheBlock);
                }
                T structure = cacheBlock[idx - cacheBlockId];

                //T refStructure;
                //long refArrayIdx = (long)System.Math.Floor(idx / _fileElementSize);
                //long refLocalIdx = idx % _fileElementSize;
                //long refLocalPosition = refLocalIdx * _elementSize;
                //// OsmSharp.Logging.Log.TraceEvent("MemoryMappedHugeArray.this.set", Logging.TraceEventType.Information, string.Format("{0}.{1}", arrayIdx, localPosition));
                //_accessors[(int)refArrayIdx].Read<T>(refLocalPosition, out refStructure);
                ////// OsmSharp.Logging.Log.TraceEvent("MemoryMappedHugeArray.this.get", Logging.TraceEventType.Information, string.Format("{0}.{1}", arrayIdx, localPosition));
                ////// _accessors[(int)arrayIdx].Read<T>(localPosition, out structure);
                //if(!refStructure.Equals(structure))
                //{ // oeps!
                //    throw new Exception();
                //}
                return structure;

                //T structure;
                //long arrayIdx = (long)System.Math.Floor(idx / _fileElementSize);
                //long localIdx = idx % _fileElementSize;
                //long localPosition = localIdx * _elementSize;
                //// OsmSharp.Logging.Log.TraceEvent("MemoryMappedHugeArray.this.set", Logging.TraceEventType.Information, string.Format("{0}.{1}", arrayIdx, localPosition));
                //_accessors[(int)arrayIdx].Read<T>(localPosition, out structure);
                //return structure;
            }
            set
            {
                long cacheBlockId = idx - (idx % _cacheBlockSize);
                T[] cacheBlock;
                if (_cache.TryGet(cacheBlockId, out cacheBlock))
                { // the current index is in cache, also update it there.
                    cacheBlock[idx - cacheBlockId] = value;
                }

                long arrayIdx = (long)System.Math.Floor(idx / _fileElementSize);
                long localIdx = idx % _fileElementSize;
                long localPosition = localIdx * _elementSize;
                // OsmSharp.Logging.Log.TraceEvent("MemoryMappedHugeArray.this.set", Logging.TraceEventType.Information, string.Format("{0}.{1}", arrayIdx, localPosition));
                _accessors[(int)arrayIdx].Write<T>(localPosition, ref value);
            }
        }

        /// <summary>
        /// Diposes of all native resource associated withh this array.
        /// </summary>
        public void Dispose()
        {
            foreach (var accessor in _accessors)
            {
                accessor.Dispose();
            }
            _accessors.Clear();
            foreach(var file in _files)
            {
                file.Dispose();
            }
            _files.Clear();
        }
    }
}
