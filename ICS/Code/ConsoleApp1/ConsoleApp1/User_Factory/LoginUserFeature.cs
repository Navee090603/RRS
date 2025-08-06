using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.User_Features;
using ConsoleApp1.Features;

namespace ConsoleApp1.Features
{
    public class LoginUserFeature : IFeature
    {
        public void Execute() => LoginUser.loginUser();
    }
}
