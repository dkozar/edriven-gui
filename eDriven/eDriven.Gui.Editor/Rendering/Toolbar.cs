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

using eDriven.Gui.Designer.Rendering;
using eDriven.Gui.Editor.Prerequisites;
using eDriven.Gui.Editor.Processing;
using UnityEditor;
using UnityEngine;

#if TRIAL
using eDriven.Gui.Editor.Dialogs;
#endif

namespace eDriven.Gui.Editor.Rendering
{
    internal class Toolbar
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static Toolbar _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private Toolbar()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static Toolbar Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating Toolbar instance"));
#endif
                    _instance = new Toolbar();
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
            /* We should initialize she live media query state now */
            _liveMediaQueries = EditorSettings.LiveStyling;

            // Always apply on play mode start
            Gui.LiveMediaQueries = _liveMediaQueries;
        }

        private bool _watchChanges;
        private bool _oldWatchChanges;

        private bool _autoSave;
        private bool _oldAutoSave;
        
        private bool _inspectorEnabled;

        private bool _liveMediaQueries;
        private bool _oldLiveMediaQueries;

        //public bool ShowHelp;

        internal void Render()
        {
            EditorGUILayout.BeginHorizontal(StyleCache.Instance.Toolbar);

            _watchChanges = EditorSettings.WatchChanges;
            _oldWatchChanges = _watchChanges;
            _watchChanges = GUILayout.Toggle(_watchChanges, GuiContentCache.Instance.MonitorChanges, 
                EditorSettings.UseDarkSkin ? StyleCache.Instance.Toggle : StyleCache.Instance.GreenToggle, // no green toggle for Pro users
                GUILayout.ExpandWidth(false), GUILayout.Height(30));
            if (_watchChanges != _oldWatchChanges)
            {
                EditorSettings.WatchChanges = _watchChanges;
                EditorState.Instance.WatchChangesChanged();
            }

            GUI.enabled = _watchChanges;

            _autoSave = EditorSettings.AutoSave;
            _oldAutoSave = _autoSave;
            _autoSave = GUILayout.Toggle(_autoSave, GuiContentCache.Instance.AutoSave, StyleCache.Instance.Toggle, GUILayout.ExpandWidth(false), GUILayout.Height(30));
            if (_autoSave != _oldAutoSave)
                EditorSettings.AutoSave = _autoSave;

            GUI.enabled = true;

            /*if (EditorSettings.InspectorEnabled && Application.isEditor && Application.isPlaying/* && null == DesignerOverlay.Instance#1#)
            {
                //if (FontMapper.IsMapping(DipSwitches.DefaultFontMapperId))
                if (null != FontMapper.GetDefault())
                {
// ReSharper disable once UnusedVariable
                    DesignerOverlay overlay = (DesignerOverlay)Framework.GetComponent<DesignerOverlay>(true); // add if non-existing
                    //overlay.Font = FontMapper.GetWithFallback(DipSwitches.DefaultFontMapperId).Font; // set font
                }
            }*/

            _inspectorEnabled = GUILayout.Toggle(EditorSettings.InspectorEnabled, GuiContentCache.Instance.Inspect, StyleCache.Instance.Toggle, GUILayout.Height(30));
            if (_inspectorEnabled != EditorSettings.InspectorEnabled)
            {
                EditorSettings.InspectorEnabled = _inspectorEnabled;
                if (_inspectorEnabled)
                {
                    OverlayPrerequisitesChecker.Check();
                    DesignerOverlay.Attach();
                    DesignerOverlay.Instance.Select(null);
                    /*if (EditorSettings.InspectorEnabled)
                        return;*/ // we must go enable the designer overlay instance
                }

                if (null != DesignerOverlay.Instance) {
                    DesignerOverlay.Instance.enabled = _inspectorEnabled;
                    if (!_inspectorEnabled)
                        DesignerOverlay.Instance.Deselect();
                }
            }

            _liveMediaQueries = EditorSettings.LiveStyling;
            _oldLiveMediaQueries = _liveMediaQueries;
            _liveMediaQueries = GUILayout.Toggle(_liveMediaQueries, GuiContentCache.Instance.LiveStyling, StyleCache.Instance.Toggle, GUILayout.ExpandWidth(false), GUILayout.Height(30));
            if (_liveMediaQueries != _oldLiveMediaQueries)
            {
                EditorSettings.LiveStyling = _liveMediaQueries;
                Gui.LiveMediaQueries = _liveMediaQueries;
                if (_liveMediaQueries)
                {
                    EditorUtility.DisplayDialog("Live styling turned ON",
                        @"Live styling is working in play mode, editor only.

When turned ON:
A. styles are being re-processed upon each stylesheet change
B. media queries are being run upon each screen resize

Also the media queries are being run upon each screen resize. 

This is processor intensive so turn it OFF when not needed (or else the editor might not be so responsive).", "OK");

                    Gui.ProcessStyles(); // fhen switched ON
                }
            }
            
            GUILayout.FlexibleSpace();

#if TRIAL
            if (GUILayout.Button(GuiContentCache.Instance.Purchase, StyleCache.Instance.Button, GUILayout.Height(30)))
            {
                //Application.OpenURL("http://u3d.as/content/adjungo/e-driven-gui/36Q");
                PurchaseDialog.Instance.ShowUtility();
            }
#endif

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(1);
        }
    }
}