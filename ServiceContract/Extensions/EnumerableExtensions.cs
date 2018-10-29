using System.Collections.Generic;
using System.Linq;

namespace N17Solutions.Semaphore.ServiceContract.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> target)
            => target == null || !target.Any();
    }
}