using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter first number (z): ");
            int z = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter second number (y): ");
            int y = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine($"{x1} = {a}");
            Console.WriteLine($"{x2} = {b}");
            Console.WriteLine($"{x3} = {c}");
            Console.WriteLine($"{x4} = {d}");
            Console.Read();
        }


        static int z= 50;
        static int y = 50;
        static string x1 = $"Addition of {z} + {y}";
        static string x2 = $"Subtraction of {z} - {y}";
        static string x3 = $"Multiplication of {z} * {y}";
        static string x4 = $"Division of {z} / {y}";
        static int a = z + y;
        static int b = z - y;
        static int c = z * y;
        static int d = z / y;
    }
}
