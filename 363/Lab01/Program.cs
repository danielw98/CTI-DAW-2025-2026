namespace Lab01;


static class ExtensionMethods
{
    public static int WordCount(this string text)
    {
        return text.Split(" ", StringSplitOptions.RemoveEmptyEntries).Length;
    }
}

internal class Program
{
    static int WordCount(string text)
    {
        return text.Split(" ", StringSplitOptions.RemoveEmptyEntries).Length;
    }

    static void ExtensionMethodsDemo()
    {
        string text = "Extension Methods Are Cool";
        //Console.WriteLine($"word count = {WordCount(text)}");
        Console.WriteLine($"word count = {text.WordCount()}");
    }
    
    static void DisplayString(string text)
    {
        Console.WriteLine("F1");
        Console.WriteLine(text);
    }

    static void Display2(string text)
    {
        Console.WriteLine("F2");
        Console.WriteLine(text);
    }
    static void DelegatesDemo()
    {
        string text = "Delegates are very useful";

        Action<string>? action = DisplayString;
        action += Display2;
        action(text);
        action -= Display2;

        Predicate<int> isEven = x => x % 2 == 0;

        isEven(10);
        isEven(5);
        action!(text);


        //DisplayString(text);
        //Display2(text);
    }
    static void Main(string[] args)
    {
        //ExtensionMethodsDemo();
        DelegatesDemo();

    }
}
