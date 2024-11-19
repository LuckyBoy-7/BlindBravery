using System;
using System.Reflection;

namespace Lucky.Kits.Utilities
{
    public class ReflectionUtils
    {
        public static Delegate GetDelegate<T>(object self, string methodName) where T : class
        {
            if (self.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) == null)
                return null;
            return Delegate.CreateDelegate(typeof(T), self, methodName);
        }
    }
}