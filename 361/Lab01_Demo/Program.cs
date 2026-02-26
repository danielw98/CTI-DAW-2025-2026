using System.Security.Cryptography.X509Certificates;

namespace Lab01_Demo;

static class ListExtensionMethods
{
    public static List<int> SquareNumbers(this List<int> numbers)
    {
        List<int> result = new List<int>();
        foreach (int number in numbers)
        {
            result.Add(number * number);
        }
        return result;
    }
    public static List<int> ApplyTransformation(this List<int> numbers, Func<int, int> func)
    {
        List<int> result = new List<int>();
        foreach (int number in numbers)
        {
            result.Add(func(number));
        }
        return result;
    }

    public static List<int> ApplyFilter(this List<int> numbers, Predicate<int> filter)
    {
        List<int> result = [];
        foreach (int number in numbers)
        {
            if(filter(number))
                result.Add(number);
        }
        return result;
    }
}
internal class Program
{
    static List<int> SquareNumbers(List<int> numbers)
    {
        List<int> result = new List<int>();
        foreach (int number in numbers)
        {
            result.Add(number*number);
        }
        return result;
    }
    static void Main(string[] args)
    {
        List<int> numbers = [1, 3, 4, 5, 6];

        //var numbersSquared = SquareNumbers(numbers);
        //numbersSquared.ForEach(x => Console.WriteLine(x));

        // varianta 2: cu metode de extensie
        //var numbersSquared = numbers.SquareNumbers();
        //numbersSquared.ForEach(x => Console.WriteLine(x));

        // varianta 3: metode de extensie + delegates
        //var numbersDoubled = numbers.ApplyTransformation(x => x*2);
        //numbersDoubled.ForEach(Console.WriteLine);

        //Predicate<int> isEven = nr => nr % 2 == 0;
        //isEven(10);
        //isEven(11);

        var evenNumbers = numbers.ApplyFilter(x => x % 2 == 0);
        evenNumbers.ForEach(Console.WriteLine);
    }
}
