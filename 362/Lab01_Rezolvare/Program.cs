namespace Lab01_Rezolvare;

public static class Extensions
{
    public static void Palindrome(this string str)
    {
        string aux = new(str.Reverse().ToArray());
        //aux.Reverse().ToArray();
        Console.WriteLine(aux);
        if(aux == str)
        {
            Console.WriteLine($"{str} este palindrom!");
        }
        else
        {
            Console.WriteLine($"{str} nu este palindrom!");
        }
    }

    public static int SumPare(this List<int> numbers)
    {
        int sum = 0;
        foreach (int i in numbers)
        {
            if (i % 2 == 0)
            {
                sum+=i;
            }
        }
        return sum;
    }

    public static void ApplyStringAction(this List<string> list, Action<string> action)
    {
        foreach (string str in list)
        {
            action(str);
        }
    }
}
internal class Program
{
    static void Exercitiul1()
    {
        string cuvant = "121";
        cuvant.Palindrome();
    }

    static List<int> FilterList2(List<int> list, Func<int,bool> func)  {
        var filteredList = new List<int>();
        foreach (int i in list)
        {
            if (func(i)) filteredList.Add(i);
        }
        return filteredList;
    }
    static void Exercitiul2()
    {
        var list= new List<int>() {101,90,50,1,200,300 };
        var filteredList = FilterList2(list, elem => elem > 100);
        foreach (var item in filteredList)
        {
            Console.WriteLine(item);
        }

    }
    static bool Verify(string str, Predicate<string> condition)
    {
        return condition(str);
    }

    static void Exercitiul3()
    {
        string test = "An";
        Console.WriteLine(Verify(test, text => text.Length > 5 ));
    }

    static List<int> ApplyFuncToList (List<int> numbers, Func<int, int> func)
    {
        var result = new List<int>();
        foreach (var number in numbers)
        {
            result.Add(func(number));
        }
        return result;
    }

    static void Exercitiul4 ()
    {
        List<int> list = [1, 2, 3];
        var newList = ApplyFuncToList(list, (x) => 2 * x);
        newList.ForEach(Console.WriteLine);
    }

    static void Exercitiul5()
    {
        List<int> list = [2, 3, 6, 8, 9, 10, 12];
        Console.WriteLine(list.SumPare());

    }

    static void ApplyStringAction(List<String> list, Action<string> action)
    {
        foreach(String str in list)
        {
            action(str);
        }
    }

    static void Exercitiul6()
    {
        List<string> list = ["Ana", "are", "mere"];
        //ApplyStringAction(list, Console.WriteLine);
        list.ApplyStringAction(Console.WriteLine);
    }

    static void Main(string[] args)
    {
        //Exercitiul1();
        //Exercitiul2();
        //Exercitiul3();
        //Exercitiul4();
        //Exercitiul5();
        //Exercitiul6();
    }
}
