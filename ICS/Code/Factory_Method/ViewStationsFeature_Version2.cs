using ConsoleApp1.User_Features;
using ConsoleApp1.Features;

namespace ConsoleApp1.Features
{
    public class ViewStationsFeature : IFeature
    {
        public void Execute() => AdminViewStations.adminViewStations();
    }
}