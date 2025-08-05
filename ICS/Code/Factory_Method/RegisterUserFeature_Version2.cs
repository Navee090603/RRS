using ConsoleApp1.User_Feature;
using ConsoleApp1.Features;

namespace ConsoleApp1.Features
{
    public class RegisterUserFeature : IFeature
    {
        public void Execute() => RegisterUser.registerUser();
    }
}