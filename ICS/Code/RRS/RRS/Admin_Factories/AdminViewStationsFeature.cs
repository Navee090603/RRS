using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RRS.Factory;//for IConcreate_Factory
using RRS.Admin_Features;//for AdminViewStations

namespace RRS.Admin_Factories
{
    public class AdminViewStationsFeature : IConcreate_Factory
    {
        public void Execute() => AdminViewStations.adminViewStations();
    }
}
