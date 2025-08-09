using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RRS.Factory;
using RRS.Admin_Features;

namespace RRS.Admin_Factories
{
    class AdminRevenueReportFeature : IConcreate_Factory
    {
        public void Execute() => AdminRevenueReport.ViewRevenueReport();
    }
}
