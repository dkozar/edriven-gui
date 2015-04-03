using System;
using System.Collections.Generic;
using eDriven.Gui.Styles.Serialization;
using UnityEngine;
using Object = UnityEngine.Object;

namespace eDriven.Gui.Styles
{
    /// <summary>
    /// Deserialized style sheets and caches them<br/>
    /// Resets and reloads automatically on level change<br/>
    /// It could be forcet to reset by setting the Settings.Instance.StyleCacheDirty to true<br/>
    /// This is usually done by the editor classes (when adding/removing/editing the style declaration)
    /// </summary>
    internal class StyleDeclarationCache
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
// ReSharper disable once CSharpWarnings::CS1591
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif
        #region Singleton

        private static StyleDeclarationCache _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private StyleDeclarationCache()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static StyleDeclarationCache Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating StyleDeclarationCache instance"));
#endif
                    _instance = new StyleDeclarationCache();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        readonly Dictionary<string, List<Serialization.StyleDeclaration>> _modules = new Dictionary<string, List<Serialization.StyleDeclaration>>(); 
        private readonly List<Serialization.StyleDeclaration> _allDeclarations = new List<Serialization.StyleDeclaration>();
        
        /// <summary>
        /// A hard lock that disables loading the  
        /// </summary>
        private bool _loaded;

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private void Initialize()
        {
            
        }

        /// <summary>
        /// Gets style declarations processed by this module
        /// </summary>
        /// <param name="module"></param>
        /// <param name="throwOnError"></param>
        /// <returns></returns>
        public List<Serialization.StyleDeclaration> GetDeclarations(string module, bool throwOnError = false)
        {
            if (!_modules.ContainsKey(module))
            {
                if (throwOnError)
                    throw new Exception(string.Format(@"Module [{0}] not registered", module));
                return null;
            }
            return _modules[module];
        }

        /// <summary>
        /// Gets all style declarations
        /// </summary>
        /// <returns></returns>
        public List<Serialization.StyleDeclaration> GetDeclarations()
        {
            return _allDeclarations;
        }

        private int _loadedLevel = -1;

        /// <summary>
        /// Loads ALL the style declarations available in the scene<br/>
        /// Note: this doesn't filter any declaration based on media query rules<br/>
        /// This should be done once per scene (loaded level)<br/>
        /// The outside logic should set the Settings.Instance.StyleCacheDirty parameter
        /// </summary>
        public void Load()
        {
            bool resetCache = Gui.StyleCacheDirty;

            Gui.StyleCacheDirty = false; // reset flag

            /**
             * If another level loaded, we need to reset the collection, 
             * even if the supplied resetCache parameter is false
             * */
            if (Application.loadedLevel != _loadedLevel)
            {
                _loadedLevel = Application.loadedLevel;
                resetCache = true;
            }

            if (resetCache)
                Reset();

            if (_loaded)
                return;

#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format(@"### Deserializing stylesheets ###"));
            }
#endif

            _modules.Clear();

            Object[] stylesheets = Object.FindObjectsOfType(typeof(eDrivenStyleSheet));

            foreach (Object o in stylesheets)
            {
                eDrivenStyleSheet eds = (eDrivenStyleSheet)o;

                if (!eds.enabled)
                    continue;
                
                foreach (Serialization.StyleDeclaration declaration in eds.StyleSheet.Declarations)
                {
                    _allDeclarations.Add(declaration);

                    if (string.IsNullOrEmpty(declaration.Module))
                        throw new Exception("Module ID cannot be empty");

                    if (!_modules.ContainsKey(declaration.Module))
                    {
                        _modules[declaration.Module] = new List<Serialization.StyleDeclaration>();
                    }
                    _modules[declaration.Module].Add(declaration);
                }
            }

            _loaded = true;
        }

        /// <summary>
        /// Resets the cache
        /// </summary>
        private void Reset()
        {
            _allDeclarations.Clear();
            _modules.Clear();
            _loaded = false; // important
        }
    }
}
