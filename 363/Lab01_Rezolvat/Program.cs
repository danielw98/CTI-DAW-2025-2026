using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

namespace Lab01_Rezolvat;

static class Extension
{
    public static bool VerifPalindrom(this string text)
    {
        for(int i = 0; i < text.Length / 2; i++)
        {
            if (text[i] != text[text.Length - 1 - i])
                return false;
        }
        return true;
    }

    public static int EvenSum(this List<int> list)
    {
        int sum = 0;
        foreach (int num in list)
            if (num % 2 == 0)
                sum += num;
        return sum;
    }


}
internal class Program
{
    static List<int> Numere(List<int> numere, Predicate<int> gauntlet)
    {
        List<int> numereFiltrate = [];
        foreach(int numar in numere)
        {
            if (gauntlet(numar))
            {
                numereFiltrate.Add(numar);
            }
        }
        return numereFiltrate;
    }

    static List<int> Numere2(List<int> lista, Func<int, int> functie)
    {
        List<int> numereFiltrate = [];
        foreach(int elem in lista)
        {
            numereFiltrate.Add(functie(elem));
        }
        return numereFiltrate;
    }
    static bool CheckLambda(string text, Predicate<string> predicate)
    {
        
        return predicate(text);
    }

    static void ApplyAction(List<string> lista, Action<string> actiune)
    {
        foreach(var element in lista)
        {
            actiune(element);
        }
   
    }
    static void Ex1()
    {
        string text = "1221";
        Console.WriteLine(text.VerifPalindrom());
    }

    static void Ex2()
    {
        List<int> lista = [1, 233, 345, 6];
        var rezultat = Numere(lista, elem => elem > 10);
        foreach (int numar in rezultat) { 
            Console.WriteLine(numar);
        }
    }
    static void Ex3()
    {
        string text1 = "poo";
        string text2 = "control";
        Predicate<string> predicate =  x => x.Length > 5;
        Console.WriteLine(CheckLambda(text1, predicate));
        Console.WriteLine(CheckLambda(text2, predicate));
    }

    static void Ex4()
    {
        List<int> lista = [1, 233, 345, 6];
        var rezultat = Numere2(lista, elem => elem * 10);
        foreach (int numar in rezultat)
        {
            Console.WriteLine(numar);
        }
    }

    static void Ex5()
    {
        List<int> list = [1, 2, 3, 4, 5, 6];
        Console.WriteLine(list.EvenSum());
    }

    static void Ex6()
    {
        List<string> nume = new List<string>() { "Mihai", "Mara", "Iulia" };
        ApplyAction(nume, Console.WriteLine);
    }
    static void Main(string[] args)
    {
        //Ex1(); 
        //Ex2();
        //Ex3();
        //Ex4();
        //Ex5();
        //Ex6();
    }
}
