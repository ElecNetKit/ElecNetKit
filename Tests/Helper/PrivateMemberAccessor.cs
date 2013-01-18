using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Helper
{
    // adapted from http://www.codeproject.com/Articles/9715/How-to-Test-Private-and-Protected-methods-in-NET
    static class PrivateMemberAccessor
    {
        public static object RunStaticMethod(System.Type t, string strMethod,
 params object[] aobjParams)
        {
            BindingFlags eFlags =
             BindingFlags.Static | BindingFlags.Public |
             BindingFlags.NonPublic;
            return RunMethod(t, strMethod,
             null, aobjParams, eFlags);
        } //end of method

        public static object RunInstanceMethod(this object objInstance, System.Type t, string strMethod,
  params object[] aobjParams)
        {
            BindingFlags eFlags = BindingFlags.Instance | BindingFlags.Public |
             BindingFlags.NonPublic;
            return RunMethod(t, strMethod,
             objInstance, aobjParams, eFlags);
        } //end of method


        private static object RunMethod(System.Type t, string
 strMethod, object objInstance, object[] aobjParams, BindingFlags eFlags)
        {
            MethodInfo m;
            try
            {
                m = t.GetMethod(strMethod, eFlags);
                if (m == null)
                {
                    throw new ArgumentException("There is no method '" +
                     strMethod + "' for type '" + t.ToString() + "'.");
                }

                object objRet = m.Invoke(objInstance, aobjParams);
                return objRet;
            }
            catch
            {
                throw;
            }
        } //end of method
    }
}
