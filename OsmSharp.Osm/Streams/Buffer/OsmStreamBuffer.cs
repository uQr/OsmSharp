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

using System.Collections.Generic;
using System.Threading;

namespace OsmSharp.Osm.Streams.Buffer
{
    /// <summary>
    /// Represents a stream buffer that acts as a buffer between source and target.
    /// </summary>
    public class OsmStreamBuffer : OsmStreamFilter
    {
        /// <summary>
        /// Holds the actual buffered objects.
        /// </summary>
        private Queue<OsmGeo> _buffer;

        /// <summary>
        /// Holds the buffer size.
        /// </summary>
        private int _bufferSize;

        /// <summary>
        /// Creates a new osm stream buffer.
        /// </summary>
        public OsmStreamBuffer()
            : this(100)
        {

        }

        /// <summary>
        /// Creates a new osm stream buffer.
        /// </summary>
        /// <param name="bufferSize">The amount of objects to buffer.</param>
        public OsmStreamBuffer(int bufferSize)
        {
            _readingTimer = new Timer(new TimerCallback(FillBuffer), null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            _bufferSize = bufferSize;
            _buffer = new Queue<OsmGeo>(_bufferSize);
            _bufferWaitHandle = new ManualResetEvent(false);
            _isFilling = new IsFillingState();
            _stopFilling = new IsFillingState();
        }

        /// <summary>
        /// Initializes this buffer.
        /// </summary>
        public override void Initialize()
        {             
            // also initialize source.
            this.Source.Initialize();

            // reset current.
            _current = null;

            // already start filling the buffer, no need to wait here.
            this.StartFillBuffer();
        }

        /// <summary>
        /// Holds the current object.
        /// </summary>
        private OsmGeo _current;

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public override OsmGeo Current()
        {
            return _current;
        }

        /// <summary>
        /// Move to the next object.
        /// </summary>
        /// <param name="ignoreNodes"></param>
        /// <param name="ignoreWays"></param>
        /// <param name="ignoreRelations"></param>
        /// <returns></returns>
        public override bool MoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
        {
            // start fill buffer is needed.
            if (!_sourceEmpty)
            { // there is still data in the source.
                this.StartFillBuffer();
            }

            // try to read object from buffer.
            while(true)
            {
                lock(_buffer)
                {
                    if(_buffer.Count > 0)
                    {
                        _current = _buffer.Dequeue();
                        break;
                    }
                    else if(_sourceEmpty)
                    { // there is no more data in the source and the buffer is empty.
                        _current = null;
                        return false;
                    }
                }

                // waits for a signal from the buffer thread.
                _bufferWaitHandle.Reset();
                this.StartFillBuffer();
                _bufferWaitHandle.WaitOne();
            }
            return true;
        }

        /// <summary>
        /// Resets this buffer and associated source.
        /// </summary>
        public override void Reset()
        {
            // make sure to stop filling buffer and reset the current buffered objects.
            this.ResetBuffer();

            // reset current.
            _current = null;

            // reset the source after buffer has stopped.
            this.Source.Reset();
        }

        /// <summary>
        /// Returns true if this buffer can be reset.
        /// </summary>
        public override bool CanReset
        {
            get { return this.Source.CanReset; }
        }

        #region Buffer Filling

        /// <summary>
        /// Holds a flag for when the source is empty.
        /// </summary>
        private bool _sourceEmpty = false;

        /// <summary>
        /// Holds the buffer wait handle.
        /// </summary>
        private EventWaitHandle _bufferWaitHandle;

        /// <summary>
        /// Holds the timer used to spawn threads to fill buffer.
        /// </summary>
        private Timer _readingTimer;

        /// <summary>
        /// Holds the isfilling flag.
        /// </summary>
        private IsFillingState _isFilling;

        /// <summary>
        /// Holds the stop filling status.
        /// </summary>
        private IsFillingState _stopFilling;

        /// <summary>
        /// Starts a new read on the buffer.
        /// </summary>
        private void StartFillBuffer()
        {
            lock (_isFilling)
            {
                if (!_isFilling.Value)
                {
                    _isFilling.Value = true;

                    // trigger buffer filling just once.
                    _readingTimer.Change(0, System.Threading.Timeout.Infinite);
                }
            }
        }

        /// <summary>
        /// Resets the buffer and stops filling.
        /// </summary>
        private void ResetBuffer()
        {
            // stop filling.
            _stopFilling.Value = true;

            // wait for it.
            while (_isFilling.Value) { }

            // reset stop filling.
            _stopFilling.Value = false;

            // clear buffer.
            _buffer.Clear();
        }

        /// <summary>
        /// Fills the buffer.
        /// </summary>
        private void FillBuffer(object state)
        {
            int objectsToRead = 0;
            lock(_buffer)
            { // synchronize access to buffer and determine current count.
                objectsToRead = _bufferSize - _buffer.Count;
            }

            while(objectsToRead > 0 && this.Source.MoveNext())
            { // TODO: read a few objects at once and only then lock the buffer.
                lock(_buffer)
                {
                    _buffer.Enqueue(this.Source.Current());
                }
                _bufferWaitHandle.Set();
                objectsToRead--;

                if(_stopFilling.Value)
                { // filling has to be stopped, probably because the buffer has been reset.
                    break;
                }
            }

            if(objectsToRead > 0)
            { // oeps, source is empty.
                _sourceEmpty = true;
                
                // make sure to signal again.
                _bufferWaitHandle.Set();
            }

            lock (_isFilling)
            { 
                _isFilling.Value = false;
            }
        }

        /// <summary>
        /// Represents the isfilling state.
        /// </summary>
        private class IsFillingState
        {
            /// <summary>
            /// Gets or sets the state.
            /// </summary>
            public bool Value { get; set; }
        }

        #endregion
    }
}
