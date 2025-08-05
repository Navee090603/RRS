using ConsoleApp1.User_Features;
using ConsoleApp1.Features;

namespace ConsoleApp1.Features
{
    public class AdminLoginFeature : IFeature
    {
        public void Execute() => AdminLogin.adminLogin();
    }
}