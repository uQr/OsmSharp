using OsmSharp.IO.MemoryMappedFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OsmSharp.WinForms.UI.IO.MemoryMappedFiles.Streamed
{
    /// <summary>
    /// A memory mapped file accessor stream wrapper.
    /// </summary>
    public class MemoryMappedFileStreamAccessorWrapper : IMemoryMappedViewAccessor
    {
        private Stream _stream;

        public delegate object ReadDelegate(Type type, Stream stream);

        private ReadDelegate _read;

        public delegate void WriteDelegate(Type type, Stream stream, ref object structure);

        private WriteDelegate _write;

        internal MemoryMappedFileStreamAccessorWrapper(Stream stream, ReadDelegate read, WriteDelegate write)
        {
            _stream = stream;
            _read = read;
            _write = write;

            var byteArray = new byte[1024];
            for(int idx = 0; idx < _stream.Length; idx = idx + 1024)
            {
                _stream.Write(byteArray, 0, 1024);
            }
        }

        public bool CanRead
        {
            get { return _stream.CanRead; }
        }

        public bool CanWrite
        {
            get { return _stream.CanWrite; }
        }

        public long Capacity
        {
            get { return _stream.Length; }
        }

        public void Read<T>(long position, out T structure) where T : struct
        {
            _stream.Seek(position, SeekOrigin.Begin);
            structure = (T)_read.Invoke(typeof(T), _stream);
        }

        public int ReadArray<T>(long position, T[] array, int offset, int count) where T : struct
        {
            _stream.Seek(position, SeekOrigin.Begin);
            for(int idx = offset; idx < offset + count; idx++)
            {
                object structure = _read.Invoke(typeof(T), _stream);
                if(structure != null)
                {
                    array[idx] = (T)structure;
                }
                else
                {
                    return idx - offset;
                }
            }
            return count;
        }

        public void Write<T>(long position, ref T structure) where T : struct
        {
            _stream.Seek(position, SeekOrigin.Begin);
            var structureBoxed = (object)structure;
            _write.Invoke(typeof(T), _stream, ref structureBoxed);
        }

        public void WriteArray<T>(long position, T[] array, int offset, int count) where T : struct
        {
            _stream.Seek(position, SeekOrigin.Begin);
            for (int idx = offset; idx < offset + count; idx++)
            {
                var structureBoxed = (object)array[idx];
                _write.Invoke(typeof(T), _stream, ref structureBoxed);
            }
        }

        public void Dispose()
        {
            _stream.Dispose();
        }
    }
}
