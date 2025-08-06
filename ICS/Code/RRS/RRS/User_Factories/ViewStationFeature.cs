using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RRS.Factory; //for teh use of IConcreate_Factory
using RRS.Admin_Features; //for the use of AdminStations

namespace RRS.User_Factories
{
    class ViewStationFeature : IConcreate_Factory
    {
        public void Execute() => AdminViewStations.adminViewStations();
    }
}
