using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RRS.Factory; //for Iconcreate_Factory
using RRS.Admin_Features; //for AdminAddTrain

namespace RRS.Admin_Factories
{
    public class AdminAddTrainFeature : IConcreate_Factory
    {
        public void Execute() => AdminAddTrain.adminAddTrain();
    }
}
