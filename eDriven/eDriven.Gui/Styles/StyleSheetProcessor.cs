using System;
using System.Collections.Generic;
using System.Text;
using eDriven.Gui.Styles.MediaQueries;
using eDriven.Gui.Styles.Serialization;
using eDriven.Gui.Util;
using UnityEngine;
using Object = UnityEngine.Object;

namespace eDriven.Gui.Styles
{
    ///<summary>
    /// Loads style declarations from style declaration cache<br/>
    /// Processes media queries and filters only the styles declarations passing the media queries<br/>
    ///</summary>
    internal static class StyleSheetProcessor
    {
// ReSharper disable once CSharpWarnings::CS1591
// ReSharper disable once InconsistentNaming
        //public const string MODULE_ID = "gui";

#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif
        /// <summary>
        /// Loads style declarations<br/>
        /// We are actually doing a full process from scratch on each screen resize<br/>
        /// The things that are cached are default styles (reflected) and eDriven stylesheet styles<br/>
        /// Each style declaration is being tested against its media queries<br/>
        /// Only declarations passing the media query are turned into actual CSSStyleDeclarations and are further processed<br/>
        /// </summary>
        internal static void Process()
        {
            var styleManager = StyleManager.Instance;

#if DEBUG
            if (DebugMode)
            {
                Debug.Log("##### Loading style sheets #####");
            }
#endif

            /**
             * 1. Reloading the cache (the cache itself will do a full reload if needed)
             * */
            StyleDeclarationCache.Instance.Load();

            /**
             * 2. Getting ALL the style declarations from the cache
             * */
            List<Serialization.StyleDeclaration> declarations = StyleDeclarationCache.Instance.GetDeclarations();

            if (null == declarations)
                return; // nothing to do here

            int count = 0;
            var list = new List<string>();

            /**
             * 4. Merging (A)
             * We need to group the same declarations together
             * That's because we need - at this stage - to merge declarations for the same component
             * In the StyleDeclaration system, there are no duplicated declarations
             * */
            Dictionary<string, List<Serialization.StyleDeclaration>> groups = new Dictionary<string, List<Serialization.StyleDeclaration>>();
            foreach (Serialization.StyleDeclaration declaration in declarations)
            {
                var mediaQueryPasses = true;
                if (null != declaration.MediaQueries)
                {
                    foreach (MediaQuery query in declaration.MediaQueries)
                    {
                        /* if a single query doesn't pass, do not process this style declaration */
                        try
                        {
#if DEBUG
                            if (DebugMode)
                            {
                                Debug.Log("Query: " + query);
                            }
#endif
                            mediaQueryPasses = MediaQueryManager.Instance.EvaluateQuery(query.Name, query.Value);
#if DEBUG
                            if (DebugMode)
                            {
                                if (!mediaQueryPasses)
                                    Debug.Log("    " + query + " doesn't pass");
                            }
#endif
                            /**
                             * When a single query doesn't pass, break the loop!
                             * (this is the AND operation!)
                             * */
                            if (!mediaQueryPasses)
                                break;

                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex.Message);
                        }
                    }
                }
                if (!mediaQueryPasses)
                    continue; // skip

                //declarationsPassingMediaQueries.Add(declaration);
                Selector selector = Selector.BuildSelector(declaration.Type, declaration.Class, declaration.Id);
                var selString = selector.ToString();
                if (!groups.ContainsKey(selString))
                {
                    groups[selString] = new List<Serialization.StyleDeclaration>();
                }
                groups[selString].Add(declaration);

                /**
                    * If we are here, it means the style declaration passes its own media queries
                    * */

                count++;
            }

            /**
             * 5. Merging (B)
             * Mearge declarations from each group together
             * */
            foreach (var key in groups.Keys)
            {
                // check if the group contains declarations (it should, at least one)
                var decls = groups[key];
                if (decls.Count == 0)
                    continue;

                // take the first declaration
                var declaration = decls[0];

                // create selector (same for all the declarations in the group)
                Selector selector = Selector.BuildSelector(declaration.Type, declaration.Class, declaration.Id);

                list.Add(selector.ToString());

                // get the existing or create new declaration
                //StyleDeclaration mergedStyle = styleManager.GetMergedStyleDeclaration(selector.ToString());

                StyleDeclaration style = styleManager.GetStyleDeclaration(selector.ToString());
                if (null == style)
                {
                    style = new StyleDeclaration(selector, true) // register
                    {
                        Module = declaration.Module
                    };
                }

                /*StyleDeclaration style = new StyleDeclaration(selector, mergedStyle == null)
                {
                    Module = declaration.Module
                };*/

                // create (blank) factory
                if (style.Set2 == null)
                {
                    StyleTable mainTable = new StyleTable();
                    style.Set2 = new StyleTableValuesFactory(mainTable);

                    // override the factory with each declaration
                    foreach (Serialization.StyleDeclaration styleDeclaration in decls)
                    {
                        StyleTable styleTable = new StyleTable();
                        foreach (StyleProperty property in styleDeclaration.Properties)
                        {
                            var value = property.Value;

                            /**
                             * Very important:
                             * null == value -> works only in build!
                             * For the editor we need a paralel check: value.GetType() != typeof(Object)
                             * That's because in editor the value isn't null!!!!!
                             * */
                            if (null == value || value.GetType() != typeof(Object))
                            {
                                styleTable.Add(property.Name, value);
                            }
                        }
                        mainTable.OverrideWith(styleTable);
                    }
                }

                //Debug.Log("style: " + style);
            }

            /*Debug.Log(@"!!!! declarationsPassingMediaQueries:
" + ListUtil<StyleDeclaration>.Format(declarationsPassingMediaQueries));*/

#if DEBUG
            if (DebugMode)
            {
                StringBuilder sb = new StringBuilder();
                list.Sort();
                foreach (string name in list)
                {
                    var line = string.Format(@"============== {0} ==============
", name);
                    var decls = styleManager.GetStyleDeclarations(name);
                    if (null != decls)
                        line = string.Format(@"============== {0} [found {1}] ==============
{2}", name, decls.Count, ListUtil<StyleDeclaration>.Format(decls));
                    sb.AppendLine(line);
                }
                Debug.Log(string.Format(@"Style declarations loaded from stylesheet ({0}):
{1}", count, sb));
            }
#endif
        }
    }
}
