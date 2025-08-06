using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.Features;

namespace ConsoleApp1
{
    public static class FeatureFactory
    {
        public static IFeature Create(string featureName)
        {
            switch (featureName.ToLower())
            {
                case "register": return new RegisterUserFeature();
                case "login": return new LoginUserFeature();
                case "bookticket": return new BookTicketFeature();
                case "viewbookings": return new ViewBookingsFeature();
                case "cancelbooking": return new CancelBookingFeature();
                case "viewstations": return new ViewStationsFeature();
                case "adminlogin": return new AdminLoginFeature();
                case "viewallusers": return new AdminViewAllUsersFeature();
                case "setuseractive": return new AdminSetUserActiveFeature();
                case "addtrain": return new AdminAddTrainFeature();
                case "viewalltrains": return new AdminViewTrainsFeature();
                case "viewallbookings": return new AdminViewAllBookingsFeature();
                case "viewallpayments": return new AdminViewAllPaymentsFeature();
                case "viewseatavailability": return new AdminViewSeatAvailabilityFeature();
                default: throw new System.ArgumentException("Unknown feature");
            }
        }
    }
}
