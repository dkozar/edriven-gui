using System;
using System.Collections.Generic;
using eDriven.Gui.Mappers;
using eDriven.Gui.Util;
using UnityEngine;

namespace eDriven.Gui.Styles
{
    /// <summary>
    /// Style attribute<br/>
    /// When used with a class, both the Name, Type and Default (or ProxyType, ProxyMemberName) should be supplied<br/>
    /// When using with property/field, nothing is mandatory:<br/>
    /// If Default supplied, it is being evaluated when clearing the style and the default value will be used
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class StyleAttribute : Attribute
    {
        /// <summary>
        /// Style name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Value type
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Default value (integer for Color, FontMapper ID for font)<br/>
        /// This value will be ignored if ProxyType defined (the value will be proxied)
        /// </summary>
        public object Default { get; set; }

        ///<summary>
        ///</summary>
        public Type ProxyType { get; set; }

        ///<summary>
        ///</summary>
        public string ProxyMemberName { get; set; }

        ///<summary>
        /// Note: sanity check done outside (in DefaultValuesFactory class)
        ///</summary>
        ///<returns></returns>
        public object GetDefault()
        {
            /* XOR! */
            //if (null == ProxyType ^ !string.IsNullOrEmpty(ProxyMemberName))
            //    throw new Exception("When using proxy, both ProxyType and ProxyMemberName have to be defined: " + GetType());

            try
            {
                /**
                 * 1. Proxy takes precedence
                 * */
                if (null != ProxyType)
                {
                    return string.IsNullOrEmpty(ProxyMemberName) ?
                        StyleProxyReader.GetProxySingleton(ProxyType) :
                        StyleProxyReader.GetProxyMemberValue(ProxyType, ProxyMemberName);
                }

                /**
                 * 2. Color
                 * */
                if (IsValidColor())
                {
                    return ColorMixer.FromHexAndAlpha((int)Default, 1f).ToColor();
                }

                /**
                 * 3. Font
                 * */
                if (IsValidFont())
                {
                    var fontMapperId = Default as string;
                    /* Use the font mapper */
                    return string.IsNullOrEmpty(fontMapperId) ?
                        FontMapper.GetDefault().Font : // get default font
                        FontMapper.GetWithFallback(fontMapperId).Font; // get named font
                }

                /**
                 * 3. Default value
                 * */
                return Default;
            }
            catch (Exception ex)
            {
                throw new Exception("Problem processing style attribute " + Name + ": ", ex);
            }
        }

        private bool IsValidFont()
        {
            if (Type == typeof(Font))
            {
                if (null == Default)
                    return true; // the default could also not be specified

                if (Default is string)
                    return true;

                /*if (null != Default)
                {
                    if (Default.GetType() != typeof(string))
                        throw new Exception("The Font attribute must contain a string as the default value. Style: " + Name);

                    // TODO: perhaps we should map to a default font anyway (when Default not specified?)
                    if (((string)Default).Length == 0)
                    {
                        throw new Exception("The Font attribute must not be an empty string. Style: " + Name + " [Default: " + Default + "]");
                    }
                    return true;
                }*/
            }
            return false;
        }

        private bool IsValidColor()
        {
            if (Type == typeof(Color) || Type == typeof(Color?))
            {
                if (null != Default)
                {
                    if (Default.GetType() != typeof(int))
                        throw new Exception("The Color attribute must contain an integer as the default value. Style: " + Name);

                    if (!((int)Default > -1 && (int)Default <= 0xffffff))
                    {
                        throw new Exception("The Color attribute must be in valid range. Style: " + Name + " [Default: " + Default + "]");
                    }
                    return true;
                }
            }

            return false;
            //return (Type == typeof(Color) || Type == typeof(Color?) && null != Default && Default.GetType() == typeof(int) && (int)Default > -1);
        }

        public override string ToString()
        {
            List<string> list = new List<string>();

            if (null != Type)
            {
                list.Add(string.Format("Type: {0}", Type));
            }

            var dflt = GetDefault();
            if (null != dflt)
            {
                list.Add(string.Format("Default: {0}", dflt));
            }

            if (null != ProxyType)
            {
                list.Add(string.Format("ProxyType: {0}", ProxyType));
            }

            if (!string.IsNullOrEmpty(ProxyMemberName))
            {
                list.Add(string.Format("ProxyMemberName: {0}", ProxyMemberName));
            }

            var strDesc = string.Empty;
            if (list.Count > 0)
            {
                strDesc = string.Format(" [{0}]", string.Join(", ", list.ToArray()));
            }

            //return string.Format(@"Name: ""{0}"", Type: {1}{2}{3}{4}", _name, _type, strDefault, strProxy, strProxyMember);
            return string.Format(@"""{0}""{1}", Name, strDesc);
        }
    }
}