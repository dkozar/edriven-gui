using System;
using eDriven.Core.Reflection;
using eDriven.Gui.Reflection;

namespace eDriven.Gui.Styles
{
    internal static class StyleProxyReader
    {
        internal static object GetProxySingleton(Type proxyType)
        {
            try {
                return GuiReflectorInternal.GetSingletonInstance(proxyType);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Cannot get singleton instance of type {0}", proxyType), ex);
            }
        }

        internal static object GetProxyMemberValue(Type proxyType, string memberName)
        {
            object singleton = GetProxySingleton(proxyType);

            try
            {
                return CoreReflector.GetValue(singleton, memberName);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Cannot get member named [{0}] on singleton instance of type [{1}]", memberName, proxyType), ex);
            }
        }
    }
}
