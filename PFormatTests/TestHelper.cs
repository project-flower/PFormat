using System.Reflection;

namespace PFormatTests
{
    internal static class TestHelper
    {
        #region Internal Methods

        internal static object InvokeMethod<T>(this object target, string name, BindingFlags bindingFlags, object[] args)
        {
            return typeof(T).InvokeMember(name, bindingFlags, null, target, args);
        }

        #endregion
    }
}
