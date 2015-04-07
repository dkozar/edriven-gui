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

#if TRIAL

using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Rendering
{
    internal class PurchaseBox
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static PurchaseBox _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private PurchaseBox()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static PurchaseBox Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating PurchaseBox instance"));
#endif
                    _instance = new PurchaseBox();
                    Initialize();
                }

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private static void Initialize()
        {

        }

        private readonly PanelRenderer _panelRenderer = new PanelRenderer
        {
            Collapsible = false,
            ContentStyle = StyleCache.Instance.AboutPanelContent
        };

        private Vector2 _scrollPosition;

        internal void Render()
        {
            //GUILayout.Space(-5);

            if (null == _panelRenderer.ChromeStyle)
                _panelRenderer.ChromeStyle = StyleCache.Instance.PanelChromeSquared;
            
            _panelRenderer.RenderStart(GuiContentCache.Instance.PurchasePanelTitle, true);

            // scroll start
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));

            GUILayout.Space(11);

            // title
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Thank you for evaluating eDriven.Gui!", StyleCache.Instance.AboutLabel, GUILayout.ExpandWidth(false));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            // text
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Box(string.Format(@"eDriven.Gui could be purchased via the Unity Asset Store.

Click the ""Purchase"" button below to open the Asset Store link.

By supporting eDriven.Gui you are helping the further development of the framework.

By purchasing now, you are eligible for all the future versions.

If still in doubt about the purchase, check out the folowing materials:

- eDriven.Gui site
- eDriven forum
- eDriven video channel
- eDriven API
- eDriven manual
- eDriven.Core @ GitHub
- eDriven WebPlayer demos

Use the ""Help -> eDriven"" menu for direct links and more information."), 
            StyleCache.Instance.PurchaseBox, GUILayout.ExpandWidth(false));

            // logo
            GUILayout.Space(6);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(new GUIContent(TextureCache.Instance.LogoSmall), GUILayout.ExpandWidth(false));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            // text 2
            GUILayout.Box(string.Format(@"Note: eDriven.Gui is (as all of the Unity Asset Store assets are) licenced PER-USER: not per-project, per-game or per-team.

Please be fair and purchase a separate license for each team member!

Thank you!

Danko Kozar, M.Eng."), StyleCache.Instance.PurchaseBox, GUILayout.ExpandWidth(false));
                /*, 
                eDrivenLinks.HOMEPAGE,
                eDrivenLinks.VIDEO,
                eDrivenLinks.API,
                eDrivenLinks.MANUAL,
                eDrivenLinks.GITHUB,
                eDrivenLinks.DEMO,
                eDrivenLinks.UNITY_FORUM_QA*/
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            // scroll end
            EditorGUILayout.EndScrollView();

            GUILayout.Space(9);

            // button
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(GuiContentCache.Instance.PurchaseButton, StyleCache.Instance.BigLightButton, GUILayout.Height(30)))
            {
                Application.OpenURL("http://u3d.as/content/adjungo/e-driven-gui/36Q");
                //Application.OpenURL("http://u3d.as/content/3796"); // nono, it doesn't work
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(9);
            _panelRenderer.RenderEnd();
        }
    }
}

#endif