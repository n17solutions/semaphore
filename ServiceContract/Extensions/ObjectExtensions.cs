using System;

namespace N17Solutions.Semaphore.ServiceContract.Extensions
{
    public static class ObjectExtensions
    {
        private const string SystemNamespace = "System";
        
        public static string GetSignalValueType(this object target)
        {
            try
            {
                return target.GetType().AssemblyQualifiedName;
            }
            catch
            {
                return new object().GetType().FullName;
            }
        }

        public static bool IsBaseType(this object target)
        {
            var type = target.GetType();
            return type.IsPrimitive || (type.Namespace?.Equals(SystemNamespace, StringComparison.InvariantCultureIgnoreCase) ?? false);
        }
    }
}