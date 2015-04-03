#region License

/*
 
Copyright (c) 2010-2014 Danko Kozar

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 
*/

#endregion License

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Rendering
{
    /// <summary>
    /// Since enum field having the variable (enum) type cannot be serialized as Enum
    /// we are serializing it as an int value. In order to manipulate such a value, we need
    /// a special implementation of popup
    /// </summary>
    internal static class EnumHelper
    {
        public static string Popup(Rect position, GUIContent label, Type type, string selected)
        {
            //var type = selected.GetType();
            if (!type.IsEnum)
                throw new Exception("parameter type must be of type System.Enum");
            string[] names = Enum.GetNames(type);
            if (names.Length == 0)
                throw new Exception("Enum has 0 velues");

            if (string.IsNullOrEmpty(selected)) // nothing selected (yet)
                selected = names[0];

            int selectedIndex = Array.IndexOf(names, selected); //Enum.GetName(type, selected));
            List<string> list = new List<string>();
            foreach (string s in Enumerable.Select(names, x => ObjectNames.NicifyVariableName(x)))
                list.Add(s);
            int index = EditorGUI.Popup(position, label, selectedIndex, TempContent(Enumerable.ToArray(list)), EditorStyles.popup);
            if (index < 0 || index >= names.Length)
                return selected;
            return names[index];
        }

        internal static GUIContent[] TempContent(string[] texts)
        {
            GUIContent[] guiContentArray = new GUIContent[texts.Length];
            for (int index = 0; index < texts.Length; ++index)
                guiContentArray[index] = new GUIContent(texts[index]);
            return guiContentArray;
        }
    }
}
