using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RRS.Admin_Factories; //For Admin_Features
using RRS.User_Factories; //For User_Features
using RRS.Login_Factory; //For Login_Feature

namespace RRS.Factory
{
    class All_Factory
    {
        public static IConcreate_Factory Create(string featureName)
        {
            switch (featureName.ToLower())
            {
                case "register": return new RegisterUserFeature();//1
                case "login": return new Login_Feature();//2
                case "bookticket": return new BookTicketFeature();//3
                case "viewbookings": return new ViewBookingsFeature();//4
                case "cancelbooking": return new CancelBookingFeature();//5
                case "viewstations": return new AdminViewStationsFeature();//6
                case "adminlogin": return new AdminLoginFeature();//7
                case "viewallusers": return new AdminViewAllUsersFeature();//8
                case "setuseractive": return new AdminSetUserActiveFeature();//9
                case "addtrain": return new AdminAddTrainFeature();//10
                case "viewalltrains": return new AdminViewTrainsFeature();//11
                case "viewallbookings": return new AdminViewAllBookingsFeature();//12
                case "viewallpayments": return new AdminViewAllPaymentsFeature();//13
                case "viewseatavailability": return new AdminViewSeatAvailabilityFeature();//14
                case "usermenu": return new UserMenuFeature();//15
                case "adminmenu": return new AdminMenuFeature();//16
                case "viewreport": return new AdminRevenueReportFeature();//17
                case "viewpassengers": return new AdminViewPassengersFeature();//18
                case "downloadticket": return new DownloadTicketFeature();//19

                default: throw new System.ArgumentException("Unknown feature");
            }
        }
    }
}
