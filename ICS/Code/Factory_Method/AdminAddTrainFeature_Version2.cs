using ConsoleApp1.User_Features;
using ConsoleApp1.Features;

namespace ConsoleApp1.Features
{
    public class AdminAddTrainFeature : IFeature
    {
        public void Execute() => AdminAddTrain.adminAddTrain();
    }
}