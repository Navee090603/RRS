using ConsoleApp1.User_Feature;
using ConsoleApp1.Features;

namespace ConsoleApp1.Features
{
    public class BookTicketFeature : IFeature
    {
        public void Execute() => BookTicket.bookTicket();
    }
}