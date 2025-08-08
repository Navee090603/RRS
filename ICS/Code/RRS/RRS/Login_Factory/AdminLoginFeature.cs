using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RRS.Factory; //for IConcreate_Factory
using RRS.Login_Features; //for AdminLogin

namespace RRS.Login_Factory
{
    public class AdminLoginFeature : IConcreate_Factory
    {
        public void Execute() => AdminLogin.adminLogin();
        //{
        //    AdminLogin.adminLogin(); if (AdminLogin.LoggedInAdminId == null)
        //    {
        //        Console.WriteLine("Login failed.");
        //    }
        //}
    }
}
