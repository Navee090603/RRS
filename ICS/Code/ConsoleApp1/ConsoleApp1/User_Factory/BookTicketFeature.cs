using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ConsoleApp1.User_Feature;
using ConsoleApp1.Features;

namespace ConsoleApp1.Features
{
    public class BookTicketFeature : IFeature
    {
        public void Execute() => BookTicket.bookTicket();
    }
}
