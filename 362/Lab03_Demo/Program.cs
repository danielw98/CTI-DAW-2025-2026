using System.Runtime.CompilerServices;

namespace Lab03_Demo;

static class ExtensionMethods
{
    public static IEnumerable<int> MyWhere(
        this IEnumerable<int> numbers,
        Func<int, bool> predicate)
    {
        foreach (int number in numbers)
        {
            if (predicate(number))
            {
                yield return number;
            }
        }
    }
}
internal class Program
{
    static void Main(string[] args)
    {
        // The Three Parts of a LINQ Query: 
        // 1. Data source.
        int[] numbers = [ 0, 1, 2, 3, 4, 5, 6 ];

        // 2. Query creation. 
        // numQuery is an IEnumerable<int>
        var numQuery = numbers
            .Where(num => num % 2 == 0);

        numQuery = numQuery.OrderByDescending(num => num);

        // 3. Query execution.
        foreach (int num in numQuery) 
        { 
            Console.WriteLine(num); 
        }
    }
}
