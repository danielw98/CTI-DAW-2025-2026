namespace Lab01;

static class ExtensionMethods
{
    public static int WordCount(this string text)
    {
        return text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
    }
}

// public delegate int Transformer(int x);
// typedef int (*Transformer)(int x);


internal class Program
{
    static int WordCount(string text)
    {
        Console.WriteLine("apel WordCount");
        return text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
    }
    static void ExtensionMethodsDemo()
    {
        string text = "metode de extensie";
        Console.WriteLine(text.WordCount());
    }

    static void DelegateDemo()
    {
        string text = "metode de extensie";

        //Func<string, int> myDelegate = WordCount;
        Action<string> myAction = x => Console.WriteLine(x);

        myAction += x => Console.Write(x.WordCount());
        myAction(text);
        //myDelegate += WordCount;
        //myDelegate -= WordCount;

        var functionsToCall = myAction.GetInvocationList().ToList();

        foreach (Action<string> function in functionsToCall)
        {
            function(text);
        }



        //myDelegate!(text);



    }
    static void Main(string[] args)
    {
        //ExtensionMethodsDemo();
        DelegateDemo();
    }
}
