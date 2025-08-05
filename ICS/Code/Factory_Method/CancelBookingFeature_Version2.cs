using ConsoleApp1.User_Feature;
using ConsoleApp1.Features;

namespace ConsoleApp1.Features
{
    public class CancelBookingFeature : IFeature
    {
        public void Execute() => CancelBooking.cancelBooking();
    }
}