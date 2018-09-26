using System;

namespace N17Solutions.Semaphore.Handlers.Extensions
{
    public static class ObjectExtensions
    {
        private const string SystemNamespace = "System";
        
        public static string GetSignalValueType(this object target)
        {
            return target.IsBaseType()
                ? target.GetType().FullName 
                : new object().GetType().FullName;
        }

        public static bool IsBaseType(this object target)
        {
            var type = target.GetType();
            return type.IsPrimitive || (type.Namespace?.Equals(SystemNamespace, StringComparison.InvariantCultureIgnoreCase) ?? false);
        }
    }
}