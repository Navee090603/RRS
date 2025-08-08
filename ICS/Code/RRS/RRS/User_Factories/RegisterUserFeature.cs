using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RRS.Factory;
using RRS.User_Features;

namespace RRS.User_Factories
{
    public class RegisterUserFeature : IConcreate_Factory
    {
        public void Execute() => RegisterUser.registerUser();
    }
}
