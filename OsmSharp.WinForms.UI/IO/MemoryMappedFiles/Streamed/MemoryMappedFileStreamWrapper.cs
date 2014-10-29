using OsmSharp.IO;
using OsmSharp.IO.MemoryMappedFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OsmSharp.WinForms.UI.IO.MemoryMappedFiles.Streamed
{
    /// <summary>
    /// A memory mapped file based on a given folder.
    /// </summary>
    public class MemoryMappedFileStreamWrapper : IMemoryMappedFile
    {
        /// <summary>
        /// Holds the stream.
        /// </summary>
        private Stream _stream;

        private MemoryMappedFileStreamAccessorWrapper.ReadDelegate _read;

        private MemoryMappedFileStreamAccessorWrapper.WriteDelegate _write;

        public delegate int SizeOfDelegate(Type type);

        private SizeOfDelegate _sizeOf;

        /// <summary>
        /// Creates a new memory mapped file stream wrapper.
        /// </summary>
        /// <param name="stream"></param>
        public MemoryMappedFileStreamWrapper(Stream stream,
            MemoryMappedFileStreamAccessorWrapper.ReadDelegate read,
            MemoryMappedFileStreamAccessorWrapper.WriteDelegate write,
            SizeOfDelegate sizeOf)
        {
            _stream = stream;
            _read = read;
            _write = write;
            _sizeOf = sizeOf;
        }

        /// <summary>
        /// Creates a new view accessor.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IMemoryMappedViewAccessor CreateViewAccessor(long offset, long size)
        {
            return new MemoryMappedFileStreamAccessorWrapper(new CappedStream(_stream, offset, size), _read, _write);
        }

        public long GetSizeOf<T>() where T : struct
        {
            return _sizeOf.Invoke(typeof(T));
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}
