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

using OsmSharp.IO.MemoryMappedFiles;
using OsmSharp.Routing.Graph.MemoryMapping;
using OsmSharp.WinForms.UI.IO.MemoryMappedFiles;
using OsmSharp.WinForms.UI.IO.MemoryMappedFiles.Streamed;
using OsmSharp.WinForms.UI.Renderer.Images;
using System;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace OsmSharp.WinForms.UI
{
    /// <summary>
    /// Class responsable for creating native hooks for platform-specific functionality.
    /// </summary>
    public static class Native
    {
        /// <summary>
        /// Initializes some platform-specifics for OsmSharp to use.
        /// </summary>
        public static void Initialize()
        {
            // enable edge read/writing.
            NativeMemoryMappedFileFactory.DoSizeOf = MemoryMappedStructures.SizeOf;
            NativeMemoryMappedFileFactory.DoReadStructure = MemoryMappedStructures.Read;
            NativeMemoryMappedFileFactory.DoWriteStructure = MemoryMappedStructures.Write;
            
            // intialize the native image cache factory.
            OsmSharp.UI.Renderer.Images.NativeImageCacheFactory.SetDelegate(
                () =>
                {
                    return new NativeImageCache();
                });
            OsmSharp.IO.MemoryMappedFiles.NativeMemoryMappedFileFactory.SetDelegates(
                (path, capacity) =>
                {
                    var file = new FileInfo(path);
                    var fs = file.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    //return new MemoryMappedFileWrapper(
                    //    MemoryMappedFile.CreateFromFile(fs, file.Name, capacity, MemoryMappedFileAccess.ReadWrite, null, HandleInheritability.Inheritable, false),
                    //    file.FullName);
                    return new MemoryMappedFileStreamWrapper(fs, 
                        NativeMemoryMappedFileFactory.Read, NativeMemoryMappedFileFactory.Write, NativeMemoryMappedFileFactory.SizeOf);
                },
                (mapName, capacity) =>
                {
                    throw new NotSupportedException("Only file-based memory mapped files are supported.");
                },
                (type) =>
                {
                    return System.Runtime.InteropServices.Marshal.SizeOf(type);
                });
        }
    }
}