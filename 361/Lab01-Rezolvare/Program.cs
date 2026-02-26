using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Lab01_Rezolvare;

public static class ExtensieString
{
    public static bool IsPalindrome(this string text)
    {
        for (int i = 0; i < text.Length/2; i++) {

            if (text[i] != text[text.Length - 1-i]) return false;
        }
        return true;
    }

    
} 

public static class ListExtensions
{
    public static int SumOfEvens(this List<int> list)
    {
        int sum = 0;
        foreach (var item in list)
        {
            if(item % 2 == 0) 
                sum += item;
        }
        return sum;
    }
}
internal class Program
{


    static void Main(string[] args)
    {
        //Exercitiul1();
        //Exercitiul2();
        //Exercitiul3();
        //Exercitiul4();
        //Exercitiul5();
        Exercitiul6();

    }

    static void Exercitiul1()
    {
        string text1 = "capac";
        string text2 = "pere";
        Console.WriteLine(text1.IsPalindrome());
        Console.WriteLine(text2.IsPalindrome());
    }

    static List<int> VerifyCondition(List<int> numbers, Predicate<int> predicate)
    {
        List<int> result = new();
        foreach(int number in numbers)
        {
            if(predicate(number)) result.Add(number);
        }
        return result;
    }
    static void Exercitiul2()
    {
        List<int> numbers = [1, 2, 3, 4, 5];
        var result = VerifyCondition(numbers, x => x %2==1);
        result.ForEach(Console.WriteLine);
    }

    static bool EvalText(string text, Predicate<string> conditie) {
        return conditie(text);
    }
    static void Exercitiul3()
    {
        string text1 = "text";
        string text2 = "program";

        Predicate<string> conditie = t => t.Length > 5; 

        Console.WriteLine($"Primul text: {text1}: {EvalText(text1, conditie)}" );
        Console.WriteLine($"Primul text: {text2}: {EvalText(text2, conditie)}");

    }

    static List<int> ApplyFunction(List<int> numbers, Func<int, int> func)
    {
        List<int> result = new();
        foreach( int number in numbers)
        {
            result.Add(func(number));
        }
        return result;
    }
    static void Exercitiul4()
    {
        List<int> numbers = [1, 100, 200, 54];
        var doubledNumbers = ApplyFunction(numbers, x => x * 2);
        doubledNumbers.ForEach(Console.WriteLine);
    }

    static void Exercitiul5()
    {
        var numbers = new List<int>() { 1, 2, 3, 4, 5 };
        Console.WriteLine(numbers.SumOfEvens());
    }

    static void ApplyAction(List<string> list, Action<string> action)
    {
        foreach( var item in list)
            action(item);
    }
    static void Exercitiul6()
    {
        List<string> words = new List<string>() { "Acestea", "sunt", "cuvinte" };

        Action <string> print = Console.WriteLine;

        ApplyAction(words, print);
    }


}
