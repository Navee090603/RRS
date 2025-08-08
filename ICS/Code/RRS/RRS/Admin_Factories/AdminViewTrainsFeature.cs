using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RRS.Factory; // for the AdminViewTrainsFeature
using RRS.Admin_Features;//for the AdminViewTrains

namespace RRS.Admin_Factories
{
    public class AdminViewTrainsFeature : IConcreate_Factory
    {
        public void Execute() => AdminViewTrains.adminViewTrains();
    }
}
