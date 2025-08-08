using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RRS.Factory;//for IConcreate_Factory
using RRS.User_Features;//for BookTicket

namespace RRS.User_Factories
{
    public class BookTicketFeature : IConcreate_Factory
    {
        public void Execute() => BookTicket.bookTicket();
    }
}
