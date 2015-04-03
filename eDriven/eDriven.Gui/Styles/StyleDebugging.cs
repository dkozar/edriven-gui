using System;
using System.Collections.Generic;
using System.Text;
using eDriven.Gui.Components;
using eDriven.Gui.Util;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;

namespace eDriven.Gui.Styles
{
    public static class StyleDebugging
    {
        ///<summary>
        ///</summary>
        public static List<Type> DebugComponents = new List<Type>();

        internal static void DebugDeclarationApplication(IStyleClient client, List<StyleDeclaration> styleDeclarations)
        {
            StringBuilder sb = new StringBuilder();

            // 1. title
            sb.AppendLine(string.Format(@"### Applying {0} style declarations ###", styleDeclarations.Count));

            var comp = client as Component;

            // 2. path
            sb.AppendLine(ComponentUtil.PathToString(comp, "->"));

            if (null != comp)
            {
                // 3. class/id
                string className = "";
                if (comp.StyleName is string && !string.IsNullOrEmpty((string)comp.StyleName))
                    className = (string)comp.StyleName;
                string id = "";
                if (!string.IsNullOrEmpty(comp.Id))
                    id = comp.Id;

                if (!string.IsNullOrEmpty(className) || !string.IsNullOrEmpty(id))
                {
                    string text = string.Empty;
                    if (!string.IsNullOrEmpty(className))
                        text += "class: " + className + ";";
                    if (!string.IsNullOrEmpty(id))
                        text += "id: " + id + ";";
                    sb.AppendLine(text);
                }
            }

            // 4. declarations
            sb.AppendLine(ListUtil<StyleDeclaration>.Format(styleDeclarations));

            Debug.Log(sb);
        }
    }
}
