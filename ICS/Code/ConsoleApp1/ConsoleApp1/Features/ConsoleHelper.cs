using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Features
{
    public static class ConsoleHelper
    {
        // Clears console and executes a feature
        public static void ExecuteFeature(string featureKey)
        {
            Console.Clear();
            FeatureFactory.Create(featureKey).Execute();
        }

        // Displays pause message and clears console
        public static void PauseAndClear()
        {
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            Console.Clear();
        }
    }
}
