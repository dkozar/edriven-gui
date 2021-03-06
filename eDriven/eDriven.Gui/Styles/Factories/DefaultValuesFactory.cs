using System;
using System.Text;
using UnityEngine;

namespace eDriven.Gui.Styles
{
    /// <summary>
    /// The factory that could be created only with referencing the serialized declaration<br/>
    /// It is used for creating a new value set
    /// </summary>
    internal class DefaultValuesFactory : IStyleValuesFactory
    {
        private readonly Type _componentType;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="componentType"></param>
        public DefaultValuesFactory(Type componentType)
        {
            _componentType = componentType;
        }

        private StyleTable _cached;

        /// <summary>
        /// Reads the component metadata and extracts key/value pairs
        /// </summary>
        /// <returns></returns>
        public StyleTable Produce()
        {
            //Debug.Log(string.Format("DefaultValuesFactory->Produce [{0}]", _componentType));

            if (null == _cached)
            {
                _cached = new StyleTable();

                var attributes = StyleReflector.GetStyleAttributes(_componentType);

#if DEBUG
                if (null != StyleProtoChain.TYPE_TO_MONITOR)
                {
                    if (_componentType == StyleProtoChain.TYPE_TO_MONITOR)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (StyleAttribute attribute in attributes)
                        {
                            sb.AppendLine(string.Format("[{0}, {1}]", attribute.Name, attribute.GetDefault()));
                        }
                        Debug.Log(string.Format(@"DefaultValuesFactory->Produced [{0}]
{1}", _componentType, sb));
                    }
                }
#endif

                foreach (StyleAttribute attribute in attributes)
                {
                    /**
                     * 1. Validate proxy
                     * */
                    ValidateAttribute(attribute);

                    /**
                     * 2. Extract possible value
                     * */
                    var def = attribute.GetDefault(); // ovdje je bio bug kod boja - nije se koristilo GetDefault(), već samo Default
                    if (null != def)
                        _cached.Add(attribute.Name, def);
                }
            }

            return (StyleTable) _cached.Clone();
        }

        private void ValidateAttribute(StyleAttribute attribute)
        {
            if (0 == attribute.Name.Length)
                throw new Exception(string.Format("Name not defined in Style attribute [class {0}]", _componentType));

            if (null == attribute.Type && attribute.Name != "skinClass")
                throw new Exception(string.Format(@"Type not defined in Style attribute [style ""{0}"", class {1}]", attribute.Name, _componentType));

            /* XOR! */
            //if ((null == attribute.ProxyType) ^ (0 == attribute.ProxyMemberName.Length)) {
            //    //Debug.Log("attribute.ProxyType: " + attribute.ProxyType);
            //    //Debug.Log("attribute.ProxyMemberName: " + attribute.ProxyMemberName);
            //    throw new Exception(string.Format(@"When using proxy in the Style attribute, both ProxyType and ProxyMemberName have to be defined in Style attribute [style ""{0}"", class {1}]", attribute.Name, _componentType));
            //}

//            /* XOR! */
//            if (null == attribute.ProxyType && !string.IsNullOrEmpty(attribute.ProxyMemberName) ||
//                null != attribute.ProxyType && string.IsNullOrEmpty(attribute.ProxyMemberName))
//                throw new Exception(string.Format("When using proxy in the Style attribute, both ProxyType and ProxyMemberName have to be defined in Style attribute [{0}], class {1}", attribute.Name, _componentType));
        }
    }
}