using System;
using System.Collections.Generic;
using System.Reflection;
using eDriven.Core.Caching;
using eDriven.Core.Reflection;
using eDriven.Gui.Reflection;
using UnityEngine;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// To speed things up, we use caching<br/>
    /// This is a global skin part cache which holds the member types the dictionary of string/member relations<br/>
    /// This way we should reflect only once per skin class in the application lifetime (assuming that member types don't change)<br/>
    /// The cache could be cleared manually anytime
    /// </summary>
    internal class SkinPartCache : Cache<Type, Dictionary<string, bool>> // MemberWrapper
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static SkinPartCache _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private SkinPartCache()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static SkinPartCache Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating SkinPartCache instance"));
#endif
                    _instance = new SkinPartCache();
                    Initialize();
                }

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private static void Initialize()
        {

        }

        public override Dictionary<string, bool> Get(Type key)
        {
            //Debug.Log("Key: " + key);
            var partDict = base.Get(key);

            if (null == partDict)
            {
                partDict = new Dictionary<string, bool>();

                MemberInfo[] proxyMembers = key.GetMembers(BindingFlags.Public | BindingFlags.GetField | BindingFlags.Instance /*BindingFlags.NonPublic | BindingFlags.Instance*/); //BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty);

                foreach (MemberInfo proxyMemberInfo in proxyMembers)
                {
                    //var skinPartAttributes = proxyMemberInfo.GetCustomAttributes(typeof(SkinPartAttribute), true);
                    var skinPartAttributes = CoreReflector.GetMemberAttributes<SkinPartAttribute>(proxyMemberInfo);

                    foreach (SkinPartAttribute attribute in skinPartAttributes)
                    {
                        if (null != attribute)
                        {
                            string id = attribute.Id;
                            if (string.IsNullOrEmpty(id)) // Id is optional
                            {
                                // If Id not defined, lookup by member name
                                // Skin should contain a member having the same name
                                id = proxyMemberInfo.Name;
                            }

                            partDict.Add(id, attribute.Required);
                            //Debug.Log("    -> " + id + ": " + skinPartAttribute.Required);
                        }
                    }
                }

                Instance.Put(key, partDict);
#if DEBUG
                if (DebugMode)
                {
                    Debug.Log("Type added to SkinPartCache: " + key);
                }
#endif
            }

            return partDict;
        } 
    }
}