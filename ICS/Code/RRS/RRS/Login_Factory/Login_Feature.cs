using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RRS.Factory; //for IConcreate_Factory
using RRS.Login_Features; //for Login

namespace RRS.Login_Factory
{
    public class Login_Feature : IConcreate_Factory
    {
        public void Execute() => Login.login();
        //{
        //    Login.login(); if (Login.LoggedInUserId == null)
        //    {
        //        Console.WriteLine("Login failed.");
        //    }
        //}
    }
}
