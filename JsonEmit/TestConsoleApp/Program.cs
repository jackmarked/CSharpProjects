using System;
using JsonEmit;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args) {
            var tests = new TestClass(100000000);
            var time1 = tests.GetTimeHardCode();
            var time2 = tests.GetTimeJsonText();
            Console.WriteLine("HardCode: " + time1.TotalSeconds);
            Console.WriteLine("JsonText: " + time2.TotalSeconds);
        }
    }
}
