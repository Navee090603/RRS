using ConsoleApp1.User_Features;
using ConsoleApp1.Features;

namespace ConsoleApp1.Features
{
    public class AdminViewAllBookingsFeature : IFeature
    {
        public void Execute() => AdminViewAllBookings.adminViewAllBookings();
    }
}