using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RRS.Factory;//for IConcreate_Factory
using RRS.User_Features;//for CancelBooking

namespace RRS.User_Factories
{
    public class CancelBookingFeature : IConcreate_Factory
    {
        public void Execute() => CancelBooking.cancelBooking();
    }
}
