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

namespace OsmSharp.UI.Map.Layers.HttpClients
{
    /// <summary>
    /// Native HttpClient factory.
    /// 
    /// Uses dependency injection to build native HttpClients.
    /// </summary>
    public static class HttpClientCacheFactory
    {
        /// <summary>
        /// Deletate to create a native image cache.
        /// </summary>
        public delegate IHttpClient HttpClientCreate();

        /// <summary>
        /// The _native image cache create delegate.
        /// </summary>
        private static HttpClientCreate _httpClientCacheCreateDelegate;

        /// <summary>
        /// Create a new native image cache.
        /// </summary>
        public static IHttpClient Create()
        {
            if (_httpClientCacheCreateDelegate == null)
            { // oeps, not initialized.
                throw new InvalidOperationException("HttpClient creation delegate not initialized, call OsmSharp.{Platform).UI.Native.Initialize() in the native code.");
            }
            return _httpClientCacheCreateDelegate.Invoke();
        }

        /// <summary>
        /// Sets the delegate.
        /// </summary>
        /// <param name="createHttpClient">Create native image.</param>
        public static void SetDelegate(HttpClientCreate createHttpClient)
        {
            _httpClientCacheCreateDelegate = createHttpClient;
        }
    }
}