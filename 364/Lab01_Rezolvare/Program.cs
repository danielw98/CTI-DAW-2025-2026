using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace Lab01_Rezolvare;

static class ExtensionMethods
{
    public static bool IsPalindrom(this string str)
    {
        string reverse = new(str.Reverse().ToArray());
        return str.CompareTo(reverse) == 0;
    }

    public static int MySum(this List<int> lista)
    {
        int sum = 0;
        foreach (int i in lista)
        {
            sum += i;
        }
        return sum;
    }
}

internal class Program
{
    static void PalindromDemo(string text)
    {
        if (text.IsPalindrom()) Console.WriteLine($"\"{text}\" is palindrom");
        else Console.WriteLine($"\"{text}\" is NOT palindrom");
    }

    static List<int> IsVerifyingCondition(List<int> numbers, Predicate<int> condition)
    {
        List<int> res = new List<int>();
        foreach (int nr in numbers)
        {
            if (condition(nr))
            {
                res.Add(nr);
            }
        }
        return res;
    }

    static bool EvaluteString(string str, Predicate<string> condition)
    {
        return condition(str);
    }
    
    static void StringLengthDemo()
    {
        string text = "Hello world";
        Predicate<string> condition = x => x.Length > 5;
        bool isLong = EvaluteString(text, condition);
        Console.WriteLine(isLong);
    }

    static void VerifyingConditionDemo()
    {
        List<int> test = new List<int> { 1, 2, 3, 4, 5, 6 };
        Predicate<int> isEven = x => x % 2 == 0;
        List<int> res = IsVerifyingCondition(test, isEven);
        foreach (var nr in res)
        {
            Console.WriteLine(nr);
        }
    }
    
    static List<int> ApplyFunc(List<int> list,Func<int, int> func)
    {
        for (int i=0;i<list.Count;i++)
        {
            list[i] = func(list[i]);
        }
        return list;

    }
    static void SquareListDemo()
    {
        List<int> list = [1, 2, 3, 4, 5, 6];
        ApplyFunc(list, x => x * x).ForEach(x => Console.WriteLine(x));
    }

    static void DemoExtensieLista()
    {
        List<int> lista = [1,2,3,4,5,6];
        Console.WriteLine(lista.MySum());
    }

    static void ApplyActions(List<string> strs, Action<string> action)
    {
        strs.ForEach(str => action(str));
    }

    static void ApplyActionsDemo()
    {
        List<string> strs = new List<string> { "Test1", "Test2" };
        ApplyActions(strs, Console.WriteLine);
    }

    static void Main(string[] args)
    {
        //PalindromDemo("aerisirea");
        //VerifyingConditionDemo();
        //SquareListDemo();
        //StringLengthDemo();
        //DemoExtensieLista();
        ApplyActionsDemo();
    }
}
