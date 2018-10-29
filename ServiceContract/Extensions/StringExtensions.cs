namespace N17Solutions.Semaphore.ServiceContract.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrBlank(this string target)
        {
            return string.IsNullOrEmpty(target?.Trim());
        }
    }
}