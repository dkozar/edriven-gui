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

using System.Collections.Generic;
using System.Net.Mime;
using System.Reflection;
using eDriven.Gui.Editor.Dialogs;
using eDriven.Gui.Editor.Display;
using eDriven.Gui.Editor.Hierarchy;
using eDriven.Gui.Editor.Processing;
using eDriven.Gui.Editor.Rendering;
using eDriven.Gui.Editor.Util;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor
{
    [Obfuscation(Exclude = true)]
    internal sealed class HierarchyWindow : EDrivenEditorWindowBase
    {
        [MenuItem("Window/eDriven.Gui/Debug/Hierarchy")]
// ReSharper disable UnusedMember.Local
        static void Init()
// ReSharper restore UnusedMember.Local
        {
            /**
             * Instantiate window
             * */
            var window = GetWindow(typeof(HierarchyWindow), false, "Hierarchy");
            window.autoRepaintOnSceneChange = true;
            window.minSize = new Vector2(100, 100);
            window.Show();

            IconSetter.SetIcon(window);
        }

        private static readonly PanelRenderer PanelRenderer = new PanelRenderer
        {
            Collapsible = false,
            Tools = new List<ToolDescriptor>(new[]
            {
                new ToolDescriptor("help", TextureCache.Instance.Help)
            })
        };

        [Obfuscation(Exclude = true)]
        // ReSharper disable UnusedMember.Local
        void OnEnable()
            // ReSharper restore UnusedMember.Local
        {
            IconSetter.SetIcon(this);

            /**
             * 1. Initialize the PlayModeStateChangeEmitter instance
             * We are subscribing to it's ChangesAppliedSignal to react when changes applied
             * so we could then pust the changes to views (events, children and layout view)
             * */
            var p = PlayModeStateChangeEmitter.Instance; // init
            p.PlayModeStoppedSignal.Connect(PlayModeStoppedSlot);
            p.ChangesAppliedSignal.Connect(ChangesAppliedSlot); // a single slot connects only once, i.e. no need to vorry about the duplication

            OrderDisplay.Instance.CollectionChangedSignal.Connect(CollectionChangedSlot);

            EditorState.Instance.SelectionChangeSignal.Connect(SelectionChangeSlot);

            HierarchyChangeProcessor.Instance.ChangesProcessedSignal.Connect(ChangesProcessedSlot);

            //EditorState.Instance.HierarchyProcessedSignal.Connect(HierarchyChangeSlot);
        }

        private Vector2 _scrollPosition;

        public static bool ShowHelp;
        private string _description = string.Empty;
        private string _descriptionRich = string.Empty;
        private static bool _doFocusOut;

        /// <summary>
        /// Renders the controls allowed for instantiation in relation to selected parent
        /// </summary>
        [Obfuscation(Exclude = true)]
// ReSharper disable UnusedMember.Local
        void OnGUI()
// ReSharper restore UnusedMember.Local
        {
            if (_doFocusOut)
            {
                _doFocusOut = false;
                GUIUtility.keyboardControl = 0;
            }

            EditorWindowContentWrapper.Start();

            if (null == PanelRenderer.ChromeStyle)
                PanelRenderer.ChromeStyle = StyleCache.Instance.PanelChromeSquared;

            PanelRenderer.RenderStart(GuiContentCache.Instance.HierarchyDebuggerPanelTitle, true);

            PanelContentWrapper.Start();

            if (PanelRenderer.ClickedTools.Count > 0)
            {
                if (PanelRenderer.ClickedTools.Contains("options"))
                {
                    PanelRenderer.ClickedTools.Remove("options");
                }
                ShowHelp = PanelRenderer.ClickedTools.Contains("help");
            }
            else
            {
                ShowHelp = false;
            }

            if (ShowHelp)
            {
                EditorGUILayout.HelpBox(Help.HierarchyDebugWindow, MessageType.Info, true);
            }

            EditorGUILayout.BeginHorizontal(StyleCache.Instance.Toolbar, GUILayout.Height(35));

            #region Refresh

            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(false));
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(GuiContentCache.Instance.Refresh, StyleCache.Instance.Button,
                GUILayout.ExpandWidth(false), GUILayout.Height(30)))
            {
                GUIUtility.keyboardControl = 0;
                ProcessInfo();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            #endregion

            #region Fix

            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(false));
            GUILayout.FlexibleSpace();

            /*bool oldEnabled = GUI.enabled;
            GUI.enabled = !Application.isPlaying;*/
            if (GUILayout.Button(GuiContentCache.Instance.FixHierarchy, StyleCache.Instance.GreenToggle,
                GUILayout.ExpandWidth(false), GUILayout.Height(30)))
            {
                var text =
                    @"eDriven will look for adapters present in the hierarchy but not listed in any of the order lists, and add them as list children.

It will also remove adapters not present in the hierarchy from all of the order lists.";

                if (EditorApplication.isPlaying)
                {
                    text += @"

Note: The play mode will be stopped in order to fix the hierarchy.";
                }

                text += @"

Are you sure you want to do this?";

                if (EditorUtility.DisplayDialog("Fix hierarchy?", text, "OK", "Cancel"))
                {
                    if (EditorApplication.isPlaying)
                    {
                        // 1. delay fixing to after the stop
                        EditorState.ShouldFixHierarchyAfterStop = true;
                        // 2. stop the play mode
                        EditorApplication.isPlaying = false;
                    }
                    else // fix immediatelly
                    {
                        FixHierarchy();
                    }
                }
            }
            /*GUI.enabled = oldEnabled;*/

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            #endregion

            #region Copy

            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(false));
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(GuiContentCache.Instance.CopyToClipboard, StyleCache.Instance.Button,
                GUILayout.ExpandWidth(false), GUILayout.Height(30)))
            {
                GUIUtility.keyboardControl = 0;
                TextEditor te = new TextEditor {content = new GUIContent(_description)};
                te.SelectAll();
                te.Copy();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            #endregion

            GUILayout.FlexibleSpace();

            #region Auto-update

            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(false));
            GUILayout.FlexibleSpace();

            bool oldAutoUpdate = GUILayout.Toggle(EditorSettings.HierarchyWindowAutoUpdate,
                EditorSettings.HierarchyWindowAutoUpdate ? GuiContentCache.Instance.AutoUpdateOn : GuiContentCache.Instance.AutoUpdateOff,
                StyleCache.Instance.Toggle,
                GUILayout.ExpandWidth(false), GUILayout.Height(30));

            if (EditorSettings.HierarchyWindowAutoUpdate != oldAutoUpdate)
            {
                GUIUtility.keyboardControl = 0;
                EditorSettings.HierarchyWindowAutoUpdate = oldAutoUpdate;
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            #endregion

            #region Write to log

            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(false));
            GUILayout.FlexibleSpace();

            bool oldWriteToLog = GUILayout.Toggle(EditorSettings.HierarchyWindowWriteToLog,
                GuiContentCache.Instance.WriteToLog, StyleCache.Instance.GreenToggle,
                GUILayout.ExpandWidth(false), GUILayout.Height(30));

            if (EditorSettings.HierarchyWindowWriteToLog != oldWriteToLog)
            {
                GUIUtility.keyboardControl = 0;
                EditorSettings.HierarchyWindowWriteToLog = oldWriteToLog;
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            #endregion

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(1);

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            //GUILayout.Label(HierarchyState.Instance.State, StyleCache.Instance.NormalLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            //EditorGUILayout.TextArea("<color=#00ff00>miki</color>" + _description, StyleCache.Instance.RichTextLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUILayout.TextArea(_descriptionRich, StyleCache.Instance.RichTextLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.EndScrollView();

            PanelContentWrapper.End();

            PanelRenderer.RenderEnd();

            //GUILayout.Space(3);
            EditorWindowContentWrapper.End();
        }

        private void FixHierarchy()
        {
            GUIUtility.keyboardControl = 0;
            var hierarchy = HierarchyBuilder.Instance.BuildHierarchy();
            HierarchyRepairer.Fix(hierarchy);
            SetDescription(hierarchy.DescribeHierarchy(true));
            _descriptionRich = hierarchy.DescribeHierarchy(true, true);
            Repaint();
        }

        #region Slots

        private void SelectionChangeSlot(params object[] parameters)
        {
            // do nothing for now
            // this should be done later (because the selection changes immediately after the component is added)
            //Debug.Log("OnSelectionChange");
            if (EditorSettings.HierarchyWindowAutoUpdate)
                ProcessInfo();
        }

        private void ChangesProcessedSlot(params object[] parameters)
        {
            //Debug.Log("OnSelectionChange");
            if (EditorSettings.HierarchyWindowAutoUpdate)
                ProcessInfo();
        }

        /*private void HierarchyChangeSlot(object[] parameters)
        {
            if (EditorSettings.HierarchyWindowAutoUpdate)
                ProcessInfo();
        }*/

        private void PlayModeStoppedSlot(object[] parameters)
        {
            if (EditorState.ShouldFixHierarchyAfterStop)
            {
                EditorState.ShouldFixHierarchyAfterStop = false;
                FixHierarchy();
            }
        }

        /// <summary>
        /// Fires when the play mode stopped and changes were already submitted to edit-mode objects
        /// </summary>
        /// <param name="parameters"></param>
        private void ChangesAppliedSlot(object[] parameters)
        {
            if (EditorSettings.HierarchyWindowAutoUpdate)
                ProcessInfo();
        }

        private void CollectionChangedSlot(object[] parameters)
        {
            if (EditorSettings.HierarchyWindowAutoUpdate)
                ProcessInfo();
        }

        #endregion

        private void ProcessInfo()
        {
            GUIUtility.keyboardControl = 0; // focus out
            //HierarchyState.Instance.Rebuild();
            Node hierarchy = HierarchyBuilder.Instance.BuildHierarchy();

            _descriptionRich = hierarchy.DescribeHierarchy(true, true);

            SetDescription(hierarchy.DescribeHierarchy(true));
            _doFocusOut = true;
        }

        private void SetDescription(string description, string prefix = null)
        {
            _description = description;
            if (EditorSettings.HierarchyWindowWriteToLog)
            {
                Debug.Log(_description);
            }
        }
    }
}