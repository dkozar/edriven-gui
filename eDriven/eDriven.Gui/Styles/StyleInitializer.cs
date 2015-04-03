using System;
using System.Collections.Generic;
using eDriven.Gui.Reflection;
using UnityEngine;

namespace eDriven.Gui.Styles
{
    /// <summary>
    /// Loads styles for eDriven.Gui framework<br/>
    /// Uses reflection to read from component metadata (once per the application runtime)<br/>
    /// Default styles (from component metadata) are being cached
    /// Style declarations are also being cached, but per scene
    /// Upon each screen resize, media wueries kick in and are filtering the list of style declarations
    /// 
    /// </summary>
    internal static class StyleInitializer
    {
#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

        /// <summary>
        /// The flag indicating that the initialization run once
        /// If run once, it means we have already reflected all the hardcoded styles
        /// and this has to be done once per app execution
        /// </summary>
        //private static bool _runOnce;

        /// <summary>
        /// Style initialization should be run only once per level,
        /// so keeping the index of the last loaded level
        /// </summary>
        private static int _levelId = -1;

        private static bool _levelChanged;

        private static bool _shouldReloadAllStyles;
        private static List<IStyleApplyer> _appliers;

        /// <summary>
        /// Runs the style initialization
        /// </summary>
        internal static void Run(/*Stage stage = null*/)
        {
            /**
             * Style initialization should be run once per level in play mode
             * However, if media queries present and enabled, it should be run on each application resize
             * It should also be run when style declarations changed from editor
             * (for this you should reset the style declaration cache by setting Settings.Instance.StyleCacheDirty = true
             */

            /* Note: _levelChanged tlag is set to true also on the first level load */
            _levelChanged = Application.loadedLevel != _levelId;

            /**
             * Evaluate if we should reload ALL the styles
             * ("null == stage" is the indication of the MediaQueryManager call)
             * Settings.Instance.StyleCacheDirty is being set from the editor prior to calling this method
             * */
            _shouldReloadAllStyles = _levelChanged || /*null == stage || */Gui.StyleCacheDirty;
            
            if (_levelChanged)
            {
                // this is a new level
                _levelId = Application.loadedLevel;
            }

#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format("StyleInitializer->Run"));
            }
#endif

            if (_shouldReloadAllStyles) // level changed or screen resized or forced from editor
            {
                /**
                 * 1. Clear all the current declarations and starting from scratch
                 * I'm not saying that here is no place for optimization :)
                 * */
                StyleManager.Instance.ClearStyleDeclarations(false);

                /**
                 * 2. Initialize a global style
                 * */
                GlobalStyle.Instance.Initialize();

                /**
                 * 3. Process default (hardcoded) styles
                 * */
                PreloaderFeedback.Instance.StepStart(PreloaderFeedback.LOADING_DEFAULT_STYLES);
                ComponentDefaultStylesLoader.Load(); // currently only GUI

                /**
                 * 4. Load styles from stylesheet (and filtering them using media queries)
                 * */
                PreloaderFeedback.Instance.StepStart(PreloaderFeedback.LOADING_STYLESHEETS);
                StyleSheetProcessor.Process();
            
                /**
                 * 5. Init chains
                 * */
                PreloaderFeedback.Instance.StepStart(PreloaderFeedback.INITIALIZING_STYLES);
                StyleManager.Instance.InitProtoChainRoots();

                //Debug.Log(StyleManager.Instance.Report());

                /**
                 * 6. Init stages
                 * */
                if (null == _appliers) {
                    _appliers = InitAppliers();
                }
                foreach (IStyleApplyer applier in _appliers)
                {
                    applier.Apply();
                }
            }
        }

        /// <summary>
        /// Initializes all the appliers
        /// </summary>
        /// <returns></returns>
        private static List<IStyleApplyer> InitAppliers()
        {
            List<Type> allTypes = TypeReflector.GetAllLoadedTypes();
            var appliers = new List<IStyleApplyer>();

            foreach (Type type in allTypes)
            {
                if (type.IsClass)
                {
                    if (typeof(IStyleApplyer).IsAssignableFrom(type))
                    {
                        //if (CoreReflector.HasClassAttributes<StyleAttribute>(type))
                        try
                        {
                            appliers.Add((IStyleApplyer) Activator.CreateInstance(type));
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Cannot create style applier instance of " + type, ex);
                        }
                    }
                }
            }

            return appliers;
        }
    }
}
