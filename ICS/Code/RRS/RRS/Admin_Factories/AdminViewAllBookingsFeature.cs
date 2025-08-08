using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RRS.Factory;//for Iconcreate_Factory
using RRS.Admin_Features;//for AdminViewAllBookings

namespace RRS.Admin_Factories
{
    public class AdminViewAllBookingsFeature : IConcreate_Factory
    {
        public void Execute() => AdminViewAllBookings.adminViewAllBookings();
    }
}
