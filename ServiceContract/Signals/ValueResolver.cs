using System;
using Newtonsoft.Json;

namespace N17Solutions.Semaphore.ServiceContract.Signals
{
    public static class ValueResolver
    {
        public static object Resolve(object value, string valueType, bool isBaseType)
        {
            var type = Type.GetType(valueType);
            if (type != null)
                return isBaseType ? Convert.ChangeType(value, type) : JsonConvert.DeserializeObject(value.ToString(), type);

            return value;
        } 
    }
}