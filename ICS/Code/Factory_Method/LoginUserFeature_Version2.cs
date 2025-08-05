using ConsoleApp1.User_Features;
using ConsoleApp1.Features;

namespace ConsoleApp1.Features
{
    public class LoginUserFeature : IFeature
    {
        public void Execute() => LoginUser.loginUser();
    }
}