using System.Collections.Generic;
using System.Linq;

namespace MiniGames.Extensions
{
    public static class Extensions
    {
        public static List<T> Shuffle<T>(this List<T> item)
        {
            System.Random rnd = new();
            return item.OrderBy(x => rnd.Next()).ToList();
        }
    }
}

