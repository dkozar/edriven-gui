using System;
using System.Collections.Generic;
using eDriven.Core.Reflection;
using eDriven.Gui.Reflection;
using eDriven.Gui.Util;
using UnityEngine;
using Component = UnityEngine.Component;

namespace eDriven.Gui.Styles
{
    internal class StyleClientManager
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static StyleClientManager _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private StyleClientManager()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static StyleClientManager Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating StyleClientManager instance"));
#endif
                    _instance = new StyleClientManager();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        private Dictionary<Type, StyleClientAdapterBase> _adapters;
        /// <summary>
        /// Gets media queries metgos infos
        /// </summary>
        public Dictionary<Type, StyleClientAdapterBase> Adapters
        {
            get { return _adapters; }
        }
        
        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private void Initialize()
        {
            _adapters = new Dictionary<Type, StyleClientAdapterBase>();

            var adapters = GuiReflector.GetAllLoadedTypesDecoratedWith(typeof(StyleClientAdapterAttribute));

#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format(@"Found {0} adapters", adapters.Count));
            }
#endif
            foreach (var adapter in adapters)
            {
                if (!(typeof(StyleClientAdapterBase).IsAssignableFrom(adapter)))
                    throw new Exception(string.Format("{0} is not StyleClientAdapterBase", adapter));

                var attributes = CoreReflector.GetClassAttributes<StyleClientAdapterAttribute>(adapter);
                var client = (StyleClientAdapterBase)Activator.CreateInstance(adapter);
                _adapters[attributes[0].Type] = client;
            }

#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format(@"Retrieved {0} style client adapters:
{1}", _adapters.Count, DictionaryUtil<Type, StyleClientAdapterBase>.Format(_adapters)));
            }
#endif
        }

        private readonly DefaultUnityStyleClientAdapter _defaultAdapter = new DefaultUnityStyleClientAdapter();

        private StyleClientAdapterBase _styleClient;

        ///<summary>
        /// Returns true if the component matches the selector
        ///</summary>
        ///<param name="component"></param>
        ///<param name="selector"></param>
        ///<returns></returns>
        internal bool MatchesSelector(Component component, Selector selector)
        {
            if (null == component)
                throw new Exception("Component is null");

            if (null == selector)
                throw new Exception("Selector is null");

            var type = component.GetType();

            /**
             * 1. Check subject
             * */
            if (type.FullName != selector.Subject)
                return false;

            if (0 == selector.Conditions.Count)
                return true; // nothing more to do

            /**
             * 1. Default adapter
             * */
            _styleClient = _defaultAdapter;

            /**
             * 2. If specified for a component...
             * */
            if (_adapters.ContainsKey(type))
                _styleClient = _adapters[type];

            /**
             * 3. Initialize with component instance
             * */
            _styleClient.Initialize(component);

            /**
             * 4. Check conditions
             * */
            foreach (CSSCondition condition in selector.Conditions)
            {
                if (!condition.MatchesStyleClient(_styleClient))
                    return false;
            }

            return true;
        }
    }
}