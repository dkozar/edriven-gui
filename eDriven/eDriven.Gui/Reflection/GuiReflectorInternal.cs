using System;
using System.Reflection;

namespace eDriven.Gui.Reflection
{
    internal static class GuiReflectorInternal
    {
// ReSharper disable InconsistentNaming
        public static string SINGLETON_INSTANCE_PROPERTY_NAME = "Instance";
// ReSharper restore InconsistentNaming

        /// <summary>
        /// Returns the singleton instance
        /// </summary>
        /// <param name="singletonType">The Singleton type</param>
        /// <param name="propertyName">The property name ("Instance" by default)</param>
        /// <returns></returns>
        public static object GetSingletonInstance(Type singletonType, string propertyName)
        {
            /*if (false) // TODO miki
            {
                Debug.Log(string.Format("GetSingletonInstance({0}, {1})", singletonType, propertyName));
            }*/
            PropertyInfo propertyInfo = singletonType.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public);
//#if DEBUG
//          if (DebugMode)
//              Debug.Log("propertyInfo: " + propertyInfo);
//#endif
            if (null == propertyInfo)
            {
                throw new Exception(string.Format(@"Couldn't reflect property [{0}] on singleton type [{1}]", propertyName, singletonType.FullName));
            }

            return propertyInfo.GetValue(null, null);
        }

// ReSharper disable UnusedMember.Local
        public static object GetSingletonInstance(Type singletonType)
// ReSharper restore UnusedMember.Local
        {
            return GetSingletonInstance(singletonType, SINGLETON_INSTANCE_PROPERTY_NAME);
        }
    }
}
