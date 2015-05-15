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

using NUnit.Framework;
using OsmSharp.Math.VRP.Routes;
using System.Collections.Generic;

namespace OsmSharp.Test.Unittests.Math.VRP.Routes
{
    /// <summary>
    /// General tests for routes.
    /// </summary>
    public abstract class RouteTest
    {
        /// <summary>
        /// Creates the IRoute implementation to perform tests on with an initial customer.
        /// </summary>
        /// <returns></returns>
        protected abstract IRoute BuildRoute(int customer, bool isRound);

        /// <summary>
        /// Creates the IRoute implementation to perform tests one but one that is empty.
        /// </summary>
        /// <param name="isRound"></param>
        /// <returns></returns>
        protected abstract IRoute BuildRoute(bool isRound);

        /// <summary>
        /// Some tests on an IRoute.
        /// </summary>
        public void DoTestAdd()
        {
            // create a new empty route.
            var route = this.BuildRoute(0, true);
            if (route != null)
            { // this part needs testing!
                Assert.AreEqual(1, route.Count);
                Assert.AreEqual(false, route.IsEmpty);
                Assert.AreEqual(true, route.IsRound);

                for (int customer = 1; customer < 100; customer++)
                {
                    route.InsertAfter(customer - 1, customer);
                    //route.InsertAfterAndRemove(customer - 1, customer, -1);

                    Assert.AreEqual(customer + 1, route.Count);
                    Assert.AreEqual(false, route.IsEmpty);
                    Assert.AreEqual(true, route.IsRound);
                    Assert.AreEqual(0, route.First);
                    Assert.AreEqual(0, route.Last);
                }
            }

            // create a new empty route.
            route = this.BuildRoute(true);
            if (route != null)
            { // this part needs testing.
                Assert.AreEqual(0, route.Count);
                Assert.AreEqual(true, route.IsEmpty);
                Assert.AreEqual(true, route.IsRound);

                for (int customer = 0; customer < 100; customer++)
                {
                    route.InsertAfter(customer - 1, customer);
                    //route.InsertAfterAndRemove(customer - 1, customer, -1);

                    Assert.AreEqual(customer + 1, route.Count);
                    Assert.AreEqual(false, route.IsEmpty);
                    Assert.AreEqual(true, route.IsRound);
                    Assert.AreEqual(0, route.First);
                    Assert.AreEqual(0, route.Last);
                }
            }
        }

        /// <summary>
        /// Test removing customers.
        /// </summary>
        public void DoTestRemove()
        {
            // create a new empty route.
            const int count = 100;
            IRoute route = this.BuildRoute(0, true);
            var customers = new List<int>();
            if (route != null)
            { // this part needs testing!
                customers.Add(0);
                for (int customer = 1; customer < count; customer++)
                {
                    route.InsertAfter(customer - 1, customer);
                    //route.InsertAfterAndRemove(customer - 1, customer, -1);
                    customers.Add(customer);
                }

                // remove customers.
                while (customers.Count > 0)
                {
                    int customerIdx = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(
                        customers.Count);
                    int customer = customers[customerIdx];
                    customers.RemoveAt(customerIdx);

                    route.Remove(customer);

                    Assert.AreEqual(customers.Count, route.Count);
                    Assert.AreEqual(customers.Count == 0, route.IsEmpty);
                    Assert.AreEqual(true, route.IsRound);
                    Assert.AreEqual(route.Last, route.First);
                }
            }

            route = this.BuildRoute(true);
            customers = new List<int>();
            if (route != null)
            { // this part needs testing!
                for (int customer = 0; customer < count; customer++)
                {
                    route.InsertAfter(customer - 1, customer);
                    //route.InsertAfterAndRemove(customer - 1, customer, -1);
                    customers.Add(customer);
                }

                // remove customers.
                while (customers.Count > 0)
                {
                    int customerIdx = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(
                        customers.Count);
                    int customer = customers[customerIdx];
                    customers.RemoveAt(customerIdx);

                    route.Remove(customer);

                    Assert.AreEqual(customers.Count, route.Count);
                    Assert.AreEqual(customers.Count == 0, route.IsEmpty);
                    Assert.AreEqual(true, route.IsRound);
                    Assert.AreEqual(route.Last, route.First);
                }
            }
        }

        /// <summary>
        /// Tests if the route containts functions work correctly.
        /// </summary>
        public void DoTestContains()
        {
            // create a new empty route.
            const int count = 100;
            IRoute route = this.BuildRoute(true);
            if (route != null)
            { // this part needs testing.
                Assert.AreEqual(0, route.Count);
                Assert.AreEqual(true, route.IsEmpty);
                Assert.AreEqual(true, route.IsRound);

                for (int customer = 0; customer < count; customer++)
                {
                    route.InsertAfter(customer - 1, customer);
                    //route.InsertAfterAndRemove(customer - 1, customer, -1);

                    Assert.AreEqual(customer + 1, route.Count);
                    Assert.AreEqual(false, route.IsEmpty);
                    Assert.AreEqual(true, route.IsRound);
                    Assert.AreEqual(0, route.First);
                    Assert.AreEqual(0, route.Last);
                }

                for (int customer = 0; customer < count - 1; customer++)
                {
                    Assert.IsTrue(route.Contains(customer));
                    Assert.IsTrue(route.Contains(customer, customer + 1));
                }
                Assert.IsTrue(route.Contains(count - 1));
                Assert.IsTrue(route.Contains(count - 1, 0));
            }
        }

        /// <summary>
        /// Test removing adding every customer at every position.
        /// </summary>
        public void DoTestAddRemoveComplete()
        {
            // create a new empty route.
            const int count = 10;
            for (int customerToRemove = 0; customerToRemove < count; customerToRemove++)
            {
                for (int customerToPlaceAfter = 0; customerToPlaceAfter < count; customerToPlaceAfter++)
                {
                    if (customerToRemove != customerToPlaceAfter)
                    {
                        IRoute route = this.BuildRoute(true);
                        if (route != null)
                        { // this part needs testing.
                            for (int customer = 0; customer < count; customer++)
                            {
                                route.InsertAfter(customer - 1, customer);
                                //route.InsertAfterAndRemove(customer - 1, customer, -1);
                            }

                            route.Remove(customerToRemove);
                            route.InsertAfter(customerToPlaceAfter, customerToRemove);
                            //route.InsertAfterAndRemove(customer_to_place_after, customer_to_remove, -1);

                            Assert.IsTrue(route.Contains(customerToPlaceAfter, customerToRemove));
                            Assert.AreEqual(count, route.Count);
                            var customersInRoute = new HashSet<int>(route);
                            Assert.AreEqual(customersInRoute.Count, route.Count);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tests all enumerations of a route.
        /// </summary>
        protected void DoTestEnumerateBetween()
        {
            int count = 10;

            IRoute route = this.BuildRoute(true);
            if (route != null)
            { // this part needs testing.
                for (int customer = 0; customer < count; customer++)
                {
                    route.InsertAfter(customer - 1, customer);
                    //route.InsertAfterAndRemove(customer - 1, customer, -1);
                }
            }

            for (int from = 0; from < count; from++)
            {
                for (int to = 0; to < count; to++)
                {
                    IEnumerator<int> enumerator = route.Between(from, to).GetEnumerator();
                    for (int customer = from; customer - 1 != to; customer++)
                    {
                        if (customer == count)
                        {
                            customer = 0;
                        }

                        Assert.IsTrue(enumerator.MoveNext());
                        Assert.AreEqual(customer, enumerator.Current);
                    }
                }
            }
        }

        /// <summary>
        /// Do get neighbour tests.
        /// </summary>
        protected void DoTestGetNeighbours()
        {
            int count = 10;

            IRoute route = this.BuildRoute(true);
            if (route != null)
            { // this part needs testing.
                for (int customer = 0; customer < count; customer++)
                {
                    route.InsertAfter(customer - 1, customer);
                    //route.InsertAfterAndRemove(customer - 1, customer, -1);
                }
            }

            int[] neighbours;
            for (int customer = 0; customer < count - 1; customer++)
            {
                neighbours = route.GetNeigbours(customer);
                Assert.IsTrue(neighbours[0] == customer + 1);
            }
            neighbours = route.GetNeigbours(count - 1);
            Assert.IsTrue(neighbours[0] == 0);
        }

        /// <summary>
        /// Test shifting customers.
        /// </summary>
        public void DoTestShift()
        {
            // create a new empty route.
            const int count = 10;
            const int testCount = 10;
            var route = this.BuildRoute(0, true);
            var customers = new List<int>();
            if (route != null)
            { // this part needs testing!
                customers.Add(0);
                for (int customer = 1; customer < count; customer++)
                {
                    route.InsertAfter(customer - 1, customer);
                    customers.Add(customer);
                }

                // remove customers.
                int testIdx = 0;
                while (testIdx < testCount)
                {
                    var customerIdx = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(
                        customers.Count);
                    var insertIdx = OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(
                        customers.Count - 2);
                    if(customerIdx <= insertIdx)
                    {
                        insertIdx = insertIdx + 1;
                    }

                    var customer = customers[customerIdx];
                    var insert = customers[insertIdx];

                    if (customerIdx < insertIdx)
                    {
                        customers.Insert(insertIdx + 1, customer);
                        customers.RemoveAt(customerIdx);
                    }
                    else
                    {
                        customers.RemoveAt(customerIdx);
                        customers.Insert(insertIdx + 1, customer);
                    }

                    route.ShiftAfter(customer, insert);

                    Assert.AreEqual(customers.Count, route.Count);
                    Assert.AreEqual(customers.Count == 0, route.IsEmpty);
                    Assert.AreEqual(true, route.IsRound);
                    Assert.AreEqual(route.Last, route.First);

                    OsmSharpAssert.ItemsAreEqual(customers, route);

                    testIdx++;
                }
            }
        }
    }
}