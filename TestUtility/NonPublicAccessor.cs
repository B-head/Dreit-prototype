using System;
using System.Reflection;

namespace TestUtility
{
    public static class NonPublicAccessor
    {
        public static object Invoke(this Type type, string name, params object[] args)
        {
            var method = type.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.OptionalParamBinding);
            return method.Invoke(null, args);
        }

        public static object Invoke(this object obj, string name, params object[] args)
        {
            var method = obj.GetType().GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.OptionalParamBinding);
            return method.Invoke(obj, args);
        }
    }
}
