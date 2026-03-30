---
title: "DAW Laborator 01 - Extension Methods si Delegates"
slug: "lab01"
order: 1
excerpt: "Extension Methods, Delegates, Func, Action — fundamente C# pentru laboratoarele următoare."
downloadUrl: "/downloads/DAW-Laborator-01-Extension-Methods-si-Delegates.pdf"
repoUrl: "https://github.com/danielw98/DAW-2025-2026/tree/main/labs/lab01"
---

# Obiective

- Familiarizarea cu mediul de lucru Visual Studio
- Reamintirea convențiilor de cod C#
- *Extension Methods*
- *Delegates*, `Func<T>`, `Action<T>`
- Exerciții — aplicații consolă C#

---

# Mediul de lucru — Visual Studio

- Crearea unui proiect de tip **Console Application (.NET)**
- Structura unui proiect C# și fișierul `Program.cs`
- Rularea aplicației

*Demo: creare proiect consolă și rulare exemplu simplu.*

- [Getting started with C#](https://learn.microsoft.com/en-us/visualstudio/get-started/csharp/)
- [Tutorial: Create a simple C# console app](https://learn.microsoft.com/en-us/visualstudio/get-started/csharp/tutorial-console?view=visualstudio)

---

# Convenții de cod C#

Spre deosebire de C, unde convențiile variază de la o bibliotecă la alta, C# este dezvoltat de Microsoft, iar convențiile oficiale reprezintă standardul. Familiarizarea cu ele de la început este esențială pentru un cod lizibil și consistent.

- [C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [Identifier Naming Rules](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names)

---

# Extension Methods

Extension methods permit adăugarea de metode noi unor tipuri existente — inclusiv tipuri din .NET — fără a modifica tipul original și fără moștenire. LINQ se bazează masiv pe acest mecanism: `Where()`, `Select()`, `OrderBy()` sunt metode de extensie definite pe `IEnumerable<T>`.

Un extension method:
- este definit într-o clasă **statică**
- este la rândul lui **static**
- primul parametru indică tipul extins și este precedat de cuvântul cheie `this`
- se apelează ca o metodă de instanță obișnuită

**Exemplu — numărare cuvinte într-un string:**

```csharp
public static class StringExtensions
{
    public static int WordCount(this string str)
    {
        return str.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
    }
}

// Utilizare
var text = "ana are mere";
int count = text.WordCount(); // 3
```

- [Extension Methods — documentație Microsoft](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods)

---

# Delegates

Un delegate este un tip care poate face referire la o metodă, permițând transmiterea unei funcții ca parametru sau stocarea ei într-o variabilă. Definește semnătura funcției (tipurile parametrilor și tipul returnat) și acceptă doar metode cu semnătură compatibilă.

Deși similar conceptual cu un *function pointer*, delegate-ul este complet **type-safe**.

În C# modern, delegații sunt utilizați cu expresii lambda în LINQ, mecanisme de callback, evenimente și programare asincronă (`async/await`).

**Exemplu:**

```csharp
public delegate int Transformer(int x);

static int Square(int x)
{
    return x * x;
}

Transformer t = Square;
int result = t(5); // 25
```

- [Delegates — documentație Microsoft](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/delegates/)

---

# Func și Action

`Func<T>` și `Action<T>` sunt delegați generici predefiniți care elimină necesitatea definirii unor delegați personalizați.

| Tip | Descriere |
|-----|-----------|
| `Action<T1, ...>` | Funcție care nu returnează valoare (`void`), primește parametri de tipurile specificate |
| `Func<T1, ..., TResult>` | Funcție care returnează o valoare de tip `TResult` |
| `Predicate<T>` | Echivalent cu `Func<T, bool>` — utilizat pentru condiții logice și filtrare |

**Exemplu:**

```csharp
Action<string> log = s => Console.WriteLine(s);
log("Hello");

Func<int, bool> isEven = x => x % 2 == 0;
bool ok = isEven(10); // true
```

- [Delegates și Lambda Expressions](https://learn.microsoft.com/en-us/dotnet/standard/delegates-lambdas)

---

# Exerciții

1. Creați o metodă de extensie pentru tipul `string` care verifică dacă un text este palindrom. **(1p)**

2. Scrieți o funcție care primește o listă de numere și o funcție `Func<int, bool>` și returnează doar elementele care respectă condiția. **(1p)**

3. Creați o metodă care primește un `string` și un `Predicate<string>` și returnează `true` sau `false` în funcție de evaluare. Testați cu o expresie lambda care verifică dacă lungimea textului este mai mare decât 5. **(1p)**

4. Scrieți o funcție care primește o listă de numere întregi și o funcție `Func<int, int>`, și returnează o listă nouă obținută prin aplicarea funcției asupra fiecărui element. **(1p)**

5. Creați o metodă de extensie pentru `List<int>` care returnează suma elementelor pare. **(1p)**

6. Scrieți o metodă care primește o listă de string-uri și un `Action<string>`, și aplică acțiunea pentru fiecare element. Testați cu o acțiune care afișează elementele la consolă. **(1p)**

7. Implementați un mecanism simplu de notificare pentru utilizatori, folosind *delegates* și *extension methods*. Se consideră un tip de date `User` cu proprietățile `Name` și `Age`. La înregistrarea unui utilizator (signup), mai multe componente independente trebuie notificate.

   1. Definiți clasa `User` și utilizați un *delegate multicast* de tip `Action<User>` pentru notificare. Expuneți metodele `Subscribe()` și `Unsubscribe()`. **(1p)**
   2. Definiți o clasă `SignupObservers` cu cel puțin două metode statice observatori (ex: trimitere email și logare eveniment). **(1p)**
   3. Atașați observatorii cu `Subscribe()`, notificați la signup, eliminați un observator cu `Unsubscribe()` și demonstrați efectul. **(1p)**
   4. Definiți clasa statică `UserExtensions` cu o metodă de extensie `Display()` pentru afișarea informațiilor unui utilizator. **(1p)**
   5. În `UserExtensions`, implementați `NotifyAll()` pentru notificarea tuturor utilizatorilor dintr-o colecție `List<User>`. **(1p)**
   6. Iterați lista de delegați folosind `GetInvocationList()` și apelați fiecare handler separat. Ce avantaje oferă această abordare față de apelarea directă a delegate-ului multicast? **(1p)**

---

# Resurse suplimentare

- [Tour of C#](https://learn.microsoft.com/en-us/dotnet/csharp/tour-of-csharp/)
- [Tipuri de date built-in](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types)
- [Console I/O](https://learn.microsoft.com/en-us/dotnet/api/system.console)
- [Array-uri](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/arrays)
- [Enum-uri](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/enum)
- [Clase și OOP](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/object-oriented/)
