using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using RRS.User_Features;

namespace Test_RRS
{
    [TestFixture]
    public class BookTicketTests
    {
        [SetUp]
        public void Setup()
        {
            BookTicket.loggedInUserId = null;
        }

        [Test]
        public void CanBookTicket_ReturnsFalse_IfUserNotLoggedIn()
        {
            BookTicket.loggedInUserId = null;
            ClassicAssert.IsFalse(BookTicket.CanBookTicket(), "Booking should not be allowed if user is not logged in.");
        }

        [Test]
        public void CanBookTicket_ReturnsTrue_IfUserLoggedIn()
        {
            BookTicket.loggedInUserId = 10;
            ClassicAssert.IsTrue(BookTicket.CanBookTicket(), "Booking should be allowed if user is logged in.");
        }

        [Test]
        public void ValidateSourceAndDestination_ReturnsFalse_IfSameStation()
        {
            var stationIds = new HashSet<int> { 1, 2, 3 };
            ClassicAssert.IsFalse(BookTicket.ValidateSourceAndDestination(2, 2, stationIds), "Source and destination cannot be the same.");
        }

        [Test]
        public void ValidateSourceAndDestination_ReturnsFalse_IfSourceNotInStationList()
        {
            var stationIds = new HashSet<int> { 1, 2, 3 };
            ClassicAssert.IsFalse(BookTicket.ValidateSourceAndDestination(4, 2, stationIds), "Source station must exist in the station list.");
        }

        [Test]
        public void ValidateSourceAndDestination_ReturnsFalse_IfDestinationNotInStationList()
        {
            var stationIds = new HashSet<int> { 1, 2, 3 };
            ClassicAssert.IsFalse(BookTicket.ValidateSourceAndDestination(1, 5, stationIds), "Destination station must exist in the station list.");
        }

        [Test]
        public void ValidateSourceAndDestination_ReturnsTrue_IfSourceAndDestinationValid_AndDifferent()
        {
            var stationIds = new HashSet<int> { 1, 2, 3 };
            ClassicAssert.IsTrue(BookTicket.ValidateSourceAndDestination(1, 3, stationIds), "Valid source and destination should return true.");
        }
    }
}

