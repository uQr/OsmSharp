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

using System;
using System.IO;

namespace OsmSharp.UI.Map.Layers.HttpClients
{
    /// <summary>
    /// Abstract interpretation of an HTTP-client.
    /// </summary>
    public interface IHttpClient : IDisposable
    {
        /// <summary>
        /// Returns a stream for the given uri.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        Stream GetStream(string uri);

        /// <summary>
        /// Adds a new media type to the accept header.
        /// </summary>
        /// <param name="mediaType"></param>
        void AddAcceptHeader(string mediaType);

        /// <summary>
        /// Clears all media types from the accept header.
        /// </summary>
        void ClearAcceptHeader();

        /// <summary>
        /// Sets the useragent.
        /// </summary>
        /// <param name="userAgent"></param>
        void SetUserAgent(string userAgent);
    }
}