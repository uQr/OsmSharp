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

using ProtoBuf;

namespace OsmSharp.UI.Renderer.Scene
{
    /// <summary>
    /// Zoom ranges.
    /// </summary>
    public class Scene2DZoomRange
    {
        /// <summary>
        /// Returns true if the given zoom range contains the given zoom. 
        /// </summary>
        /// <param name="minZoom"></param>
        /// <param name="maxZoom"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public static bool Contains(float minZoom, float maxZoom, float zoom)
        {
            if (minZoom >= zoom || maxZoom < zoom)
            { // outside of zoom bounds!
                return false;
            }
            return true;
        }
    }
}