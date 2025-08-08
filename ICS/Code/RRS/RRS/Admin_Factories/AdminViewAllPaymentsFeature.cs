using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RRS.Factory;//for IConcreate_Factory
using RRS.Admin_Features;//for AdminViewAllPayments

namespace RRS.Admin_Factories
{
    public class AdminViewAllPaymentsFeature : IConcreate_Factory
    {
        public void Execute() => AdminViewAllPayments.adminViewAllPayments();
    }
}
