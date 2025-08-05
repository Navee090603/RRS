using ConsoleApp1.User_Features;
using ConsoleApp1.Features;

namespace ConsoleApp1.Features
{
    public class AdminViewSeatAvailabilityFeature : IFeature
    {
        public void Execute() => AdminViewSeatAvailability.adminViewSeatAvailability();
    }
}