using System.Diagnostics;
using JetDevel.Collections;
using JetDevel.TestUtilities;

ReflectionValueComparer comparer = new();
var first = new object();
var second = new object();
Debug.Assert(comparer.Equals(first, second));


Console.WriteLine("Hello World!");
var count = int.Parse(Console.ReadLine());
var coll = new AvlBinaryTreeCollection<int>();
for(int i = 0; i < count; i++)
{
    coll.Add(i);
}
Console.ReadKey();
coll.Clear();
Console.ReadKey();