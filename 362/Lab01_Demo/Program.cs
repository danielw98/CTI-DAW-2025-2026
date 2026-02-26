using System.Threading.Channels;

namespace Lab01_Demo;

static class ListExtensionMethods
{
    public static List<int> EvenNumbers(this List<int> list)
    {
        List<int> result = [];
        foreach (int i in list)
        {
            if (i % 2 == 0)
            {
                result.Add(i);
            }
        }
        return result;
    }
    public static List<int> ApplyFunction(this List<int> list, Func<int, int> func)
    {
        List<int> result = [];
        foreach (int i in list)
        {
            result.Add(func(i));
        }
        return result;
    }
    public static List<int> FilteredList(this List<int> list, Predicate<int> filter)
    {
        // Func<Student, Student, int>
        List<int> result = [];
        foreach (int i in list)
        {
            if (filter(i))
            {
                result.Add(i);
            }
        }
        return result;

    }
    internal class Program
    {
        static List<int> EvenNumbers(List<int> list)
        {
            List<int> result = [];
            foreach (int i in list)
            {
                if (i % 2 == 0)
                {
                    result.Add(i);
                }
            }
            return result;
        }

        static void Main(string[] args)
        {
            List<int> numbers = [1, 2, 3, 4, 5];
            //EvenNumbers(numbers).ForEach(Console.WriteLine);
            //numbers.EvenNumbers().ForEach(Console.WriteLine);

            //Action<int> action = Console.WriteLine;
            //action += Console.WriteLine;

            //var actions = action.GetInvocationList();

            //foreach(Action<int> act in actions)
            //{
            //    Console.WriteLine("Apel");
            //    act(10);
            //}
            //action(10);

            //var filteredList = numbers.FilteredList(num => num % 2 == 0);
            //filteredList.ForEach(Console.WriteLine);

            var doubleNumbers = numbers.ApplyFunction(num => num * 2);
            doubleNumbers.ForEach(Console.WriteLine);
        
        }
    }
}
