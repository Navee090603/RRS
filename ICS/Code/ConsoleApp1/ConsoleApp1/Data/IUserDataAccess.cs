using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Data
{
    public interface IUserDataAccess
    {
        (bool success, string message) RegisterUser(User user);
    }
}
