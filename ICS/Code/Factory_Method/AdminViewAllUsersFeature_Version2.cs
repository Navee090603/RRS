using ConsoleApp1.User_Features;
using ConsoleApp1.Features;

namespace ConsoleApp1.Features
{
    public class AdminViewAllUsersFeature : IFeature
    {
        public void Execute() => AdminViewAllUsers.adminViewAllUsers();
    }
}