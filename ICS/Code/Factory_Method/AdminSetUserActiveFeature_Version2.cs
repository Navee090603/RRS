using ConsoleApp1.User_Features;
using ConsoleApp1.Features;

namespace ConsoleApp1.Features
{
    public class AdminSetUserActiveFeature : IFeature
    {
        public void Execute() => AdminSetUserActive.adminSetUserActive(AdminLogin.LoggedInAdminId);
    }
}