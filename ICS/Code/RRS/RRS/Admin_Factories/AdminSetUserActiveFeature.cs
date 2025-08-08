using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RRS.Factory;//for IConcreate_Factory
using RRS.Login_Features;//for AdminLogin
using RRS.Admin_Features;//for AdminSetUserActive

namespace RRS.Admin_Factories
{
    public class AdminSetUserActiveFeature : IConcreate_Factory
    {
        public void Execute() => AdminSetUserActive.adminSetUserActive(AdminLogin.LoggedInAdminId);
    }
}
