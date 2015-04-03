using System;
using System.Collections.Generic;
using UnityEngine;

namespace eDriven.Gui.Reflection
{
    public class GlobalTypeDictionary : Dictionary<string, Type>
    {
#if DEBUG
    // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static GlobalTypeDictionary _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private GlobalTypeDictionary()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static GlobalTypeDictionary Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating TypeDictionary instance"));
#endif
                    _instance = new GlobalTypeDictionary();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private void Initialize()
        {
            var allTypes = TypeReflector.GetAllLoadedTypes(); // load types
            foreach (var type in allTypes)
            {
                this[type.FullName] = type;
            }
        }

        /// <summary>
        /// Gets the value gracefully
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="throwOnError"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Type Get(string fullName, bool throwOnError = true)
        {
            if (throwOnError && !ContainsKey(fullName))
                throw new GlobalTypeDictionaryException("Dictionary doesn't contain a key: " + fullName);

            return this[fullName];
        }
    }
}
