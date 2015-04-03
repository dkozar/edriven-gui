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

using UnityEngine;

namespace eDriven.Gui.Editor.Rendering
{
    internal class LayoutTabBar : TabBarRenderer
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static LayoutTabBar _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private LayoutTabBar()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static LayoutTabBar Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating LayoutTabBar instance"));
#endif
                    _instance = new LayoutTabBar();
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
            TabStyle = StyleCache.Instance.Toggle;
            Tabs.Add(
                new TabDescriptor(
                    new GUIContent("Absolute", TextureCache.Instance.LayoutAbsoluteOff),
                    new GUIContent("Absolute", TextureCache.Instance.LayoutAbsoluteOn)
                )
            );

            Tabs.Add(
                new TabDescriptor(
                    new GUIContent("Horizontal", TextureCache.Instance.LayoutHorizontalOff),
                    new GUIContent("Horizontal", TextureCache.Instance.LayoutHorizontalOn)
                )
            );

            Tabs.Add(
                new TabDescriptor(
                    new GUIContent("Vertical", TextureCache.Instance.LayoutVerticalOff),
                    new GUIContent("Vertical", TextureCache.Instance.LayoutVerticalOn)
                )
            );

            Tabs.Add(
                new TabDescriptor(
                    new GUIContent("Tile", TextureCache.Instance.LayoutTileOff),
                    new GUIContent("Tile", TextureCache.Instance.LayoutTileOn)
                )
            );

            //TabIndex = EditorSettings.TabIndex;
        }

        /*public override int TabIndex
        {
            get
            {
                return base.TabIndex;
            }
            set
            {
                base.TabIndex = value;
                EditorSettings.TabIndex = value;
            }
        } */

        /*protected override void OnChange(int index)
        {
            EditorSettings.TabIndex = index;
        }*/
    }
}