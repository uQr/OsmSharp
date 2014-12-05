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

using OsmSharp.Math.Geo.Simple;
using System;
using System.IO;

namespace OsmSharp.IO.MemoryMappedFiles
{
    /// <summary>
    /// Native image cache factory.
    /// 
    /// Uses dependency injection to build native images.
    /// </summary>
    public static class NativeMemoryMappedFileFactory
    {
        /// <summary>
        /// Delegate to create a native MemoryMappedFile.
        /// </summary>
        /// <param name="path">The path to file to map.</param>
        /// <param name="capacity">The maximum size, in bytes, to allocate to the memory-mapped file.</param>
        /// <param name="offset">The offset, in bytes, in the file to start the mapping.</param>
        public delegate IMemoryMappedFile NativeMemoryMappedFileCreate(string path, long capacity, long offset);

        /// <summary>
        /// The native MemoryMappedFile create delegate.
        /// </summary>
        private static NativeMemoryMappedFileCreate _nativeMemoryMappedFileDelegate;

        /// <summary>
        /// Creates a native MemoryMappedFile.
        /// </summary>
        /// <param name="path">The path to file to map.</param>
        /// <param name="capacity">The maximum size, in bytes, to allocate to the memory-mapped file.</param>
        /// <returns></returns>
        public static IMemoryMappedFile CreateFromFile(string path, long capacity)
        {
            return NativeMemoryMappedFileFactory.CreateFromFile(path, capacity, 0);
        }

        /// <summary>
        /// Creates a native MemoryMappedFile.
        /// </summary>
        /// <param name="path">The path to file to map.</param>
        /// <param name="capacity">The maximum size, in bytes, to allocate to the memory-mapped file.</param>
        /// <param name="offset">The offset, in bytes, in the file to start the mapping.</param>
        public static IMemoryMappedFile CreateFromFile(string path, long capacity, long offset)
        {
            if (_nativeMemoryMappedFileDelegate == null)
            { // oeps, not initialized.
                throw new InvalidOperationException("MemoryMappedFile creating delegate not initialized, call OsmSharp.{Platform).UI.Native.Initialize() in the native code.");
            }
            return _nativeMemoryMappedFileDelegate.Invoke(path, capacity, offset);
        }

        /// <summary>
        /// Delegate to create a memory-mapped file that has the specified capacity in system memory.
        /// </summary>
        /// <param name="mapName">A name to assign to the memory-mapped file.</param>
        /// <param name="capacity">The maximum size, in bytes, to allocate to the memory-mapped file.</param>
        public delegate IMemoryMappedFile NativeMemoryMappedFileSharedCreate(string mapName, long capacity);

        /// <summary>
        /// The native MemoryMappedFile shared create delegate.
        /// </summary>
        private static NativeMemoryMappedFileSharedCreate _nativeMemoryMappedFileSharedDelegate;

        /// <summary>
        /// Creates a new memory-mapped file that has the specified capacity in system memory.
        /// </summary>
        /// <param name="mapName">A name to assign to the memory-mapped file.</param>
        /// <param name="capacity">The maximum size, in bytes, to allocate to the memory-mapped file.</param>
        public static IMemoryMappedFile CreateNew(string mapName, long capacity)
        {
            if (_nativeMemoryMappedFileSharedDelegate == null)
            { // oeps, not initialized.
                throw new InvalidOperationException("MemoryMappedFile creating delegate not initialized, call OsmSharp.{Platform).UI.Native.Initialize() in the native code.");
            }
            return _nativeMemoryMappedFileSharedDelegate.Invoke(mapName, capacity);
        }

        /// <summary>
        /// Delegate to calculate the size in-memory of the given type.
        /// </summary>
        /// <param name="type">A name to assign to the memory-mapped file.</param>
        public delegate int SizeDelegate(Type type);

        /// <summary>
        /// The native get size delegate.
        /// </summary>
        private static SizeDelegate _getSizeDelegate;

        /// <summary>
        /// Calculates the size in-memory of the given type.
        /// </summary>
        /// <param name="type">A name to assign to the memory-mapped file.</param>
        public static int GetSize(Type type)
        {
            if (_getSizeDelegate == null)
            { // oeps, not initialized.
                throw new InvalidOperationException("MemoryMappedFile get size not initialized, call OsmSharp.{Platform).UI.Native.Initialize() in the native code.");
            }
            return _getSizeDelegate.Invoke(type);
        }

        /// <summary>
        /// Sets the delegate.
        /// </summary>
        /// <param name="createMemoryMappedFile">Creates a memory mapped file delegate.</param>
        /// <param name="createMemoryMappedSharedFile">Creates a shared memory mapped file delegate.</param>
        /// <param name="getSizeDelegate">Delegate to calculate the size in-memory of the given type.</param>
        public static void SetDelegates(NativeMemoryMappedFileCreate createMemoryMappedFile, NativeMemoryMappedFileSharedCreate createMemoryMappedSharedFile, SizeDelegate getSizeDelegate)
        {
            _nativeMemoryMappedFileDelegate = createMemoryMappedFile;
            _nativeMemoryMappedFileSharedDelegate = createMemoryMappedSharedFile;
            _getSizeDelegate = getSizeDelegate;
        }

        /// <summary>
        /// Holds the read buffer.
        /// </summary>
        private static byte[] _readBuffer = new byte[32];

        /// <summary>
        /// A delegate to read a structure of a given type to a stream.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public delegate object ReadStructureDelegate(Type type, Stream stream);

        /// <summary>
        /// Holds a delegate to read a structure of a given type to a stream.
        /// </summary>
        public static ReadStructureDelegate DoReadStructure;

        /// <summary>
        /// Reads an object of the given type from the stream at the current position.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static object Read(Type type, Stream stream)
        {
            if (typeof(uint) == type)
            {
                stream.Read(_readBuffer, 0, 4);
                return BitConverter.ToUInt32(_readBuffer, 0);
            }
            else if (typeof(int) == type)
            {
                stream.Read(_readBuffer, 0, 8);
                return BitConverter.ToInt32(_readBuffer, 0);
            }
            else if (typeof(long) == type)
            {
                stream.Read(_readBuffer, 0, 8);
                return BitConverter.ToInt64(_readBuffer, 0);
            }
            else if (typeof(ulong) == type)
            {
                stream.Read(_readBuffer, 0, 8);
                return BitConverter.ToUInt64(_readBuffer, 0);
            }
            else if (typeof(GeoCoordinateSimple) == type)
            {
                stream.Read(_readBuffer, 0, 8);
                return new GeoCoordinateSimple()
                {
                    Latitude = BitConverter.ToSingle(_readBuffer, 0),
                    Longitude = BitConverter.ToSingle(_readBuffer, 4)
                };
            }
            else if (typeof(float) == type)
            {
                stream.Read(_readBuffer, 0, 4);
                return BitConverter.ToSingle(_readBuffer, 0);
            }
            else if (DoReadStructure != null)
            {
                return DoReadStructure(type, stream);
            }
            throw new NotSupportedException(string.Format("Type {0} not supported for memory mapping.", type.ToInvariantString()));
        }

        /// <summary>
        /// A delegate to write a structure of a given type to a stream.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="stream"></param>
        /// <param name="structure"></param>
        /// <returns></returns>
        public delegate void WriteStructureDelegate(Type type, Stream stream, ref object structure);

        /// <summary>
        /// Holds a delegate to write a structure of a given type to a stream.
        /// </summary>
        public static WriteStructureDelegate DoWriteStructure;

        /// <summary>
        /// Writes an object of the given type to the stream at the current position.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="stream"></param>
        /// <param name="structure"></param>
        public static void Write(Type type, Stream stream, ref object structure)
        {
            if (typeof(uint) == type)
            {
                stream.Write(BitConverter.GetBytes((uint)structure), 0, 4);
            }
            else if (typeof(int) == type)
            {
                stream.Write(BitConverter.GetBytes((int)structure), 0, 4);
            }
            else if (typeof(long) == type)
            {
                stream.Write(BitConverter.GetBytes((long)structure), 0, 8);
            }
            else if (typeof(ulong) == type)
            {
                stream.Write(BitConverter.GetBytes((ulong)structure), 0, 8);
            }
            else if (typeof(GeoCoordinateSimple) == type)
            {
                var coordinate = (GeoCoordinateSimple)structure;
                stream.Write(BitConverter.GetBytes(coordinate.Latitude), 0, 4);
                stream.Write(BitConverter.GetBytes(coordinate.Longitude), 0, 4);
            }
            else if (typeof(float) == type)
            {
                stream.Write(BitConverter.GetBytes((float)structure), 0, 4);
            }
            else
            {
                if (DoWriteStructure == null)
                {
                    throw new NotSupportedException(string.Format("Type {0} not supported for memory mapping.", type.ToInvariantString()));
                }
                DoWriteStructure(type, stream, ref structure);
            }            
        }

        /// <summary>
        /// A delegate to get the size a structure.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public delegate int SizeOfDelegate(Type type);

        /// <summary>
        /// Holds a delegate to get the size a structure.
        /// </summary>
        public static SizeOfDelegate DoSizeOf;

        /// <summary>
        /// Returns the size of a structure of the given type on disk.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int SizeOf(Type type)
        {
            if(typeof(uint) == type)
            {
                return 4;
            }
            else if (typeof(int) == type)
            {
                return 4;
            }
            else if (typeof(long) == type)
            {
                return 8;
            }
            else if (typeof(ulong) == type)
            {
                return 8;
            }
            else if (typeof(GeoCoordinateSimple) == type)
            {
                return 8;
            }
            else if (typeof(float) == type)
            {
                return 4;
            }
            else
            {
                if (DoSizeOf != null)
                {
                    return DoSizeOf(type);
                }
                throw new NotSupportedException(string.Format("Type {0} not supported for memory mapping.", type.ToInvariantString()));
            }
        }
    }
}