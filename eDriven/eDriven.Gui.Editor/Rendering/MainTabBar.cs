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
    internal class MainTabBar : TabBarRenderer
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static MainTabBar _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private MainTabBar()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static MainTabBar Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating MainTabBar instance"));
#endif
                    _instance = new MainTabBar();
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
            Tabs.Add(
                new TabDescriptor(
                    new GUIContent("Children", TextureCache.Instance.OrderNormal),
                    new GUIContent("Children", TextureCache.Instance.OrderSelected)
                )
            );

            Tabs.Add(
                new TabDescriptor(
                    new GUIContent("Layout", TextureCache.Instance.LayoutNormal),
                    new GUIContent("Layout", TextureCache.Instance.Layout)
                )
            );

            Tabs.Add(
                new TabDescriptor(
                    new GUIContent("Events", TextureCache.Instance.EventsNormal),
                    new GUIContent("Events", TextureCache.Instance.Events)
                )
            );

            TabIndex = EditorSettings.TabIndex;
        }

        public override int TabIndex
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
        } 

        protected override void OnChange(int index)
        {
            EditorSettings.TabIndex = index;
        }
    }
}