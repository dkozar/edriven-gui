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
using System.Text;
using eDriven.Gui.Reflection;
using UnityEngine;

namespace eDriven.Gui.Editor.Reflection
{
    internal class StyleSheetsReflector
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
        /// Cached types
        /// </summary>
        private static List<Type> _availableUnityComponents;

        public static List<Type> GetAllUnityComponents()
        {
            if (null == _availableUnityComponents)
            {
                var types = GuiReflector.GetAllLoadedTypesAsignableFrom(typeof (Component)); // UnityEngine.Component

#if DEBUG
                if (DebugMode)
                {
                    StringBuilder sb = new StringBuilder();
                    if (types.Count == 0)
                    {
                        sb.AppendLine("No styleable classes available.");
                    }
                    else
                    {
                        foreach (Type type in types)
                        {
                            sb.AppendLine(string.Format("    -> {0}", type));
                        }
                    }

                    Debug.Log(string.Format(@"====== Unity components ======
{0}", sb));
                }
#endif
                _availableUnityComponents = types;
                //Debug.Log("_availableUnityComponents: " + _availableUnityComponents.Count);
            }

            return _availableUnityComponents;
        }
    }
}
