using System;
namespace Stage0
{
    partial class Program
    {
        private static void Main(string[] args)
        {
            Welcome7210();
            Welcome1944();
            Console.ReadKey();
        }
        static partial void Welcome1944();
        private static void Welcome7210()
        {
            Console.WriteLine("Enter your name:");
            string? name = Console.ReadLine();
            Console.WriteLine($"{name}, welcome to my first console application");
        }
    }
}