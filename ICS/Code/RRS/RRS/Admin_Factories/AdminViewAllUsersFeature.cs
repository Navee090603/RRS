using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RRS.Factory; //for IConcreate_Factory
using RRS.Admin_Features;//for AdminViewAllUsers

namespace RRS.Admin_Factories
{
    public class AdminViewAllUsersFeature : IConcreate_Factory
    {
        public void Execute() => AdminViewAllUsers.adminViewAllUsers();
    }
}
