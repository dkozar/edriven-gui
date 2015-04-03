using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace eDriven.Gui.Styles
{
    /// <summary>
    /// Loads styles from component metadata<br/>
    /// Uses reflection to read component metadata<br/>
    /// This should run only once - on application start<br/>
    /// It should not reload on scene change, because there's no change in assembly values<br/>
    /// It's currently being called by StageManager during its initialization 
    /// </summary>
    internal static class ComponentDefaultStylesLoader
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
        /// The reflection should be run once
        /// If types already reflected, this has to be set to a value
        /// </summary>
        //private static List<StyleDeclaration> _defaultDeclarations;

        private static List<Type> _styleableClasses;

#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        private static DateTime _time;

// ReSharper restore UnassignedField.Global
#endif

        internal static void Load()
        {
            //Debug.Log("##### Loading default styles #####");

            var styleManager = StyleManager.Instance;

            if (null == _styleableClasses)
                _styleableClasses = StyleReflector.GetAllStyleableClasses();
            
#if DEBUG
            if (DebugMode)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Type type in _styleableClasses)
                {
                    sb.AppendLine(type.FullName);
                }
                Debug.Log(string.Format(@"Default style declarations from styleable types ({0}):
{1}", _styleableClasses.Count, sb));
            }
#endif

#if DEBUG
            if (DebugMode)
            {
                _time = DateTime.Now;
            }
#endif

            foreach (Type type in _styleableClasses)
            {
                //var selector = StyleSelector.FormatType(type.FullName);
                var fullName = type.FullName; // string.Format("[{0}]", type.FullName);
                object selector = new Selector(fullName, null);
                var mergedStyle = styleManager.GetMergedStyleDeclaration(fullName);
                /*if (null != mergedStyle)
                    Debug.Log("mergedStyle for " + type.FullName + " is:" + mergedStyle);*/

                /**
                 * Creating style declaration
                 * */
                var declaration = new StyleDeclaration(selector, mergedStyle == null); // {IsReflected = true};

                if (declaration.Set1 == null)
                {
                    declaration.Set1 = new DefaultValuesFactory(type);
                }

                if (mergedStyle != null &&
                    (null == mergedStyle.Set1 ||
                    //ObjectUtil.compare(new style.defaultFactory(), new mergedStyle.defaultFactory())))
                    declaration.Set1.Produce().Equals(mergedStyle.Set1.Produce())))
                {
                    //Debug.Log("*** Setting for " + style.SelectorString);
                    styleManager.SetStyleDeclaration(declaration.SelectorString, declaration, false);
                }
                //Debug.Log("   -> " + StyleManager.Instance.GetStyleDeclaration(selector));
            }

#if DEBUG
            if (DebugMode)
            {
                var diff = DateTime.Now.Subtract(_time);
                Debug.Log("It took " + diff.TotalMilliseconds + " ms.");
            }
#endif
        }
    }
}
