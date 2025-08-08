using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RRS.Factory;
using RRS.Login_Features;

namespace RRS.Login_Factory
{
    public class AdminMenuFeature : IConcreate_Factory
    {
        public void Execute() => AdminMenu.adminMenu();
    }
}
