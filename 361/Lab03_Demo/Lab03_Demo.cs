using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab03_Demo;

internal static class Lab03_Demo
{
    static public IEnumerable<int> MyWhere(this IEnumerable<int> data, Func<int, bool> predicate)
    {
        foreach (var item in data)
        {
            if (predicate(item))
            {
                yield return item;
            }
        }
    }
}
