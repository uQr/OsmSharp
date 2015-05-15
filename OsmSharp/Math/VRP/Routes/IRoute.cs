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

using System;
using System.Collections.Generic;

namespace OsmSharp.Math.VRP.Routes
{
    /// <summary>
    /// Represents a route.
    /// </summary>
    public interface IRoute : IEnumerable<int>, ICloneable
    {
        /// <summary>
        /// Returns true if the route is empty.
        /// </summary>
        bool IsEmpty
        {
            get;
        }

        /// <summary>
        /// Returns true if the last customer is linked with the first one.
        /// </summary>
        bool IsRound
        {
            get;
        }

        /// <summary>
        /// Returns the amount of customers in the route.
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// Returns the first customer.
        /// </summary>
        /// <returns></returns>
        int First
        {
            get;
        }

        /// <summary>
        /// Returns the last customer.
        /// </summary>
        int Last
        {
            get;
        }

        /// <summary>
        /// Returns true if there is an edge in this route from from to to.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        bool Contains(int from, int to);

        /// <summary>
        /// Returns true if the given customer is in this route.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        bool Contains(int customer);

        /// <summary>
        /// Removes a customer from the route.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        bool Remove(int customer);

        /// <summary>
        /// Removes a customer from the route.
        /// </summary>
        /// <param name="customer">The customer to remove.</param>
        /// <param name="after">The customer that used to exist after.</param>
        /// <param name="before">The customer that used to exist before.</param>
        /// <returns></returns>
        bool Remove(int customer, out int before, out int after);

        /// <summary>
        /// Shifts the given customer to a new location and places it after the given 'before' customer.
        /// </summary>
        /// <param name="customer">The customer to shift.</param>
        /// <param name="before">The new customer that will come right before.</param>
        /// <returns></returns>
        /// <remarks>example:
        /// route: 1->2->3->4->5->6
        ///     customer:   2
        ///     before:     4
        ///     
        /// new route: 1->3->4->2->5->6
        /// </remarks>
        bool ShiftAfter(int customer, int before);

        /// <summary>
        /// Shifts the given customer to a new location and places it after the given 'before' customer.
        /// </summary>
        /// <param name="customer">The customer to shift.</param>
        /// <param name="before">The new customer that will come right before.</param>
        /// <param name="oldBefore">The customer that used to exist before.</param>
        /// <param name="oldAfter">The customer that used to exist after.</param>
        /// <param name="newAfter">The customer that new exists after.</param>
        /// <returns></returns>
        /// <remarks>example:
        /// route: 1->2->3->4->5->6
        ///     customer:   2
        ///     before:     4
        ///     
        /// new route: 1->3->4->2->5->6
        ///     oldBefore:  1
        ///     oldAfter:   3
        ///     newAfter:   5
        /// </remarks>
        bool ShiftAfter(int customer, int before, out int oldBefore, out int oldAfter, out int newAfter);

        /// <summary>
        /// Removes the edge from->unknown and replaces it with the edge from->to.
        /// 0->1->2:ReplaceEdgeFrom(0, 2):0->2 without resetting the last customer property.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        void ReplaceEdgeFrom(int from, int to);

        /// <summary>
        /// Replaces the first customer.
        /// </summary>
        /// <param name="first"></param>
        void ReplaceFirst(int first);

        /// <summary>
        /// Removes the edge from->unknown and replaces it with the edge from->to->unknown.
        /// 0->1:InsertAfter(0, 2):0->2-1
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        void InsertAfter(int from, int to);

        /// <summary>
        /// Inserst a new first customer.
        /// </summary>
        /// <param name="first"></param>
        void InsertFirst(int first);

        /// <summary>
        /// Returns the neigbours of a customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        int[] GetNeigbours(int customer);

        /// <summary>
        /// Returns the index of the given customer the first being zero.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        int GetIndexOf(int customer);

        /// <summary>
        /// Returns true if the route is valid.
        /// </summary>
        /// <returns></returns>
        bool IsValid();

        /// <summary>
        /// Returns an enumerable that enumerates between the two given customers.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        IEnumerable<int> Between(int from, int to);

        /// <summary>
        /// Returns an enumerable that enumerates all customer pairs that occur in the route as 1->2. If the route is a round the pair that contains last->first is also included.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Pair> Pairs();

        /// <summary>
        /// Returns an enumerable that enumerates all customer triples that occur in the route as 1->2-3. If the route is a round the tuples that contain last->first are also included.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Triple> Triples();

        /// <summary>
        /// Removes all customers in this route.
        /// </summary>
        void Clear();
    }
}
