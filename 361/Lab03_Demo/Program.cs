
// The Three Parts of a LINQ Query: 
// 1. Data source.
using Lab03_Demo;

int[] numbers = [ 0, 1, 2, 3, 4, 5, 6 ];

// 2. Query creation. 
// numQuery is an IEnumerable<int>
IEnumerable<int> numQuery = numbers.Where(num => num % 2 == 0);

numQuery = numQuery.OrderByDescending(num => num);

// 3. Query execution.

foreach (int num in numQuery)
{ 
    Console.WriteLine(num); 
}

