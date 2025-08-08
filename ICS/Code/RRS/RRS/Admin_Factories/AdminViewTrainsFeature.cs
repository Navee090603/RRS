using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RRS.Factory; // for IConcreate_Factory
using RRS.Admin_Features;//for AdminViewTrains

namespace RRS.Admin_Factories
{
    public class AdminViewTrainsFeature : IConcreate_Factory
    {
        public void Execute() => AdminViewTrains.adminViewTrains();
    }
}
