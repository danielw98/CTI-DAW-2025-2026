namespace Lab03_Demo;

static class ExtensionMethods
{
    public static IEnumerable<int> MyWhere(this int[] data, Func<int, bool> predicate)
    {
        foreach (var item in data)
        {
            if (predicate(item))
                yield return item;
        }
    }
}

internal class Program
{

    static void Main(string[] args)
    {
        // The Three Parts of a LINQ Query:
        // 1. Data source.
        int[] numbers = [0, 1, 2, 3, 4, 5, 6];

        // 2. Query creation.
        // numQuery is an IEnumerable<int>
        List<int> numQuery = numbers.MyWhere(num => num % 2 == 0)
                .Select(num => num).ToList();

        numbers[0] = 1;

        // 3. Query execution.
        foreach (int num in numQuery)
        { 
            Console.WriteLine(num);
        }
    }
}
