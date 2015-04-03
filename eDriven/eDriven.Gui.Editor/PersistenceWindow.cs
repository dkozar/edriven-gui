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
using System.Reflection;
using System.Text;
using eDriven.Gui.Designer;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Editor.Dialogs;
using eDriven.Gui.Editor.Persistence;
using eDriven.Gui.Editor.Processing;
using eDriven.Gui.Editor.Rendering;
using eDriven.Gui.Editor.Util;
using eDriven.Gui.Util;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor
{
    [Obfuscation(Exclude = true)]
    internal sealed class PersistenceWindow : EDrivenEditorWindowBase
    {
#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

        private const string TextAreaControlName = "eDrivenPersistenceWindowTA";

        [MenuItem("Window/eDriven.Gui/Debug/Persistence")]
// ReSharper disable UnusedMember.Local
        static void Init()
// ReSharper restore UnusedMember.Local
        {
            /**
             * Instantiate window
             * */
            var window = GetWindow(typeof(PersistenceWindow), false, "Persistence");
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
            
            // subscribe to changes
            PlayModeStateChangeEmitter.Instance.ChangesAppliedSignal.Connect(ChangesAppliedSlot);
            /*EditorState.Instance.SelectionChangeSignal.Connect(SelectionChangeSlot);
            EditorState.Instance.HierarchyChangeSignal.Connect(HierarchyChangeSlot);*/
            PersistenceManager.Instance.MonitoredObjectAddedSignal.Connect(MonitoredObjectAddedSlot);
            //PersistenceManager.Instance.ChangesAppliedSignal.Connect(ChangesAppliedSlot);

            HierarchyChangeProcessor.Instance.ChangesProcessedSignal.Connect(ChangesProcessedSlot);
            //EditorState.Instance.HierarchyProcessedSignal.Connect(ChangesProcessedSlot);

            EditorState.Instance.WatchChangesChangedSignal.Connect(WatchChangesChangedSlot);
        }

        /*[Obfuscation(Exclude = true)]
        // ReSharper disable UnusedMember.Local
        void OnDisable()
        // ReSharper restore UnusedMember.Local
        {
            // unsubscribe from changes
            PlayModeStateChangeEmitter.Instance.ChangesAppliedSignal.Disconnect(ChangesAppliedSlot);
            /*EditorState.Instance.SelectionChangeSignal.Disconnect(SelectionChangeSlot);
            EditorState.Instance.HierarchyChangeSignal.Disconnect(HierarchyChangeSlot);#1#
        }*/

        private Vector2 _scrollPosition;

        public static bool ShowHelp;
        private string _text = string.Empty;
        private string _richText = string.Empty;
        private string _textToDisplay = string.Empty;
        private bool _doFocusOut;

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

            PanelRenderer.RenderStart(GuiContentCache.Instance.PersistenceDebuggerPanelTitle, true);

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
                EditorGUILayout.HelpBox(Help.PersistenceDebugWindow, MessageType.Info, true);
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

            #region Abandon chages

            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(false));
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(GuiContentCache.Instance.AbandonChanges, StyleCache.Instance.Button,
                GUILayout.ExpandWidth(false), GUILayout.Height(30)))
            {
                PersistenceManager.Instance.AbandonChanges();
                HierarchyChangeProcessor.Instance.Reset();
                ProcessInfo();
            }

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
                TextEditor te = new TextEditor {content = new GUIContent(_text)};
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

            bool oldAutoUpdate = GUILayout.Toggle(EditorSettings.PersistenceWindowAutoUpdate,
                EditorSettings.PersistenceWindowAutoUpdate ? GuiContentCache.Instance.AutoUpdateOn : GuiContentCache.Instance.AutoUpdateOff, 
                StyleCache.Instance.Toggle,
                GUILayout.ExpandWidth(false), GUILayout.Height(30));

            if (EditorSettings.PersistenceWindowAutoUpdate != oldAutoUpdate)
            {
                GUIUtility.keyboardControl = 0;
                EditorSettings.PersistenceWindowAutoUpdate = oldAutoUpdate;
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            #endregion

            #region Write to log

            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(false));
            GUILayout.FlexibleSpace();

            bool oldWriteToLog = GUILayout.Toggle(EditorSettings.PersistenceWindowWriteToLog,
                GuiContentCache.Instance.WriteToLog, StyleCache.Instance.GreenToggle,
                GUILayout.ExpandWidth(false), GUILayout.Height(30));

            if (EditorSettings.PersistenceWindowWriteToLog != oldWriteToLog)
            {
                GUIUtility.keyboardControl = 0;
                EditorSettings.PersistenceWindowWriteToLog = oldWriteToLog;
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            #endregion
            
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(1);

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            //GUILayout.Label(_text, StyleCache.Instance.NormalLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            GUI.SetNextControlName(TextAreaControlName);
            _textToDisplay = EditorGUILayout.TextArea(_textToDisplay, StyleCache.Instance.RichTextLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            bool isInFocus = GUI.GetNameOfFocusedControl() == TextAreaControlName;
            _textToDisplay = isInFocus ? _text : _richText;
            
            GUILayout.EndScrollView();
            
            RenderStatus(
                EditorSettings.WatchChanges ? 
                    (EditorApplication.isPlaying ? "Monitoring..." : "Will monitor in play mode.") : 
                    "Not monitoring.", 
                EditorSettings.WatchChanges && EditorApplication.isPlaying
            );

            PanelContentWrapper.End();

            PanelRenderer.RenderEnd();

            //GUILayout.Space(3);
            EditorWindowContentWrapper.End();
        }

        private static Color _oldColor;
        private static void RenderStatus(string label, bool monitoring)
        {
            _oldColor = GUI.color;
            
            GUI.color = monitoring ? Color.green : ColorMixer.FromHex(0x222222).ToColor();
            EditorGUILayout.BeginHorizontal(StyleCache.Instance.StatusToolbar, GUILayout.Height(35));

            GUI.color = monitoring ? Color.black : ColorMixer.FromHex(0xb1b1b1).ToColor();
            GUILayout.Label(label, StyleCache.Instance.CenteredWhiteLabel, GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true));

            GUILayout.EndHorizontal();
            GUI.color = _oldColor;
        }

        #region Slots

        /*private void HierarchyChangeSlot(object[] parameters)
        {
            LogUtil.PrintCurrentMethod();
        }

        private void SelectionChangeSlot(params object[] parameters)
        {
            LogUtil.PrintCurrentMethod();
        }*/

        private void MonitoredObjectAddedSlot(object[] parameters)
        {
            //LogUtil.PrintCurrentMethod();
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("MonitoredObjectAddedSlot");
            }
#endif
            if (EditorSettings.PersistenceWindowAutoUpdate)
                ProcessInfo(parameters[0]);
        }

        private void ChangesProcessedSlot(object[] parameters)
        {
            //LogUtil.PrintCurrentMethod();
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("ChangesProcessedSlot");
            }
#endif
            if (EditorSettings.PersistenceWindowAutoUpdate)
                ProcessInfo();
        }

        /// <summary>
        /// Fires when the play mode stopped and changes were already submitted to edit-mode objects
        /// </summary>
        /// <param name="parameters"></param>
        private void ChangesAppliedSlot(object[] parameters)
        {
            //LogUtil.PrintCurrentMethod();
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("ChangesAppliedSlot");
            }
#endif
            if (EditorSettings.PersistenceWindowAutoUpdate)
                SetDescription("Changes applied.");
        }

        private string _txt;
        private void ProcessInfo(object target = null)
        {
            GUIUtility.keyboardControl = 0; // focus out

            var monitoredObjects = PersistenceManager.Instance.MonitoredObjects;

            StringBuilder sb = new StringBuilder();
            StringBuilder sbRich = new StringBuilder();

            var stepDelta = HierarchyChangeProcessor.Instance.StepDelta;
            if (null != stepDelta && stepDelta.HasChanges)
            {
                _txt = "Latest change: " + stepDelta;
                sb.AppendLine(_txt);
                sbRich.AppendLine(StringUtil.WrapColor(_txt, 
                    EditorSettings.UseDarkSkin ? "yellow" : "blue")); // #00ff00
            }

            var fullDelta = EditorApplication.isPlaying ?
                HierarchyChangeProcessor.Instance.FullDelta : HierarchyChangeProcessor.Instance.StepDelta;
            if (null != fullDelta && fullDelta.HasChanges)
            {
                _txt = "Changes: " + fullDelta;
                sb.AppendLine(_txt);
                sbRich.AppendLine(_txt);
            }

            ApplyMonitoredComponents(monitoredObjects, sb);
            ApplyMonitoredComponents(monitoredObjects, sbRich);

            _richText = sbRich.ToString();

            // sb.AppendLine(PersistenceManager.Instance.Delta.ToString());

            SetDescription(sb.ToString(), null == target ? null : (target.GetType() + " added:"));

            _doFocusOut = true;
        }

        private static void ApplyMonitoredComponents(Dictionary<int, PersistedComponent> monitoredComponents, StringBuilder sb)
        {
            if (0 == monitoredComponents.Count)
                return;

            sb.AppendLine(string.Format("Monitored components ({0}):", monitoredComponents.Count));
            sb.AppendLine(
                "------------------------------------------------------------------------------------------------------------------------");

            foreach (KeyValuePair<int, PersistedComponent> pair in monitoredComponents)
            {
                var adapter = pair.Value.Target as ComponentAdapter;
                var value = pair.Value.ToString();
                if (null != adapter)
                    value = string.Format("{0} [{1}][{2}]", adapter.GetType().Name, pair.Key,
                        GuiLookup.PathToString(adapter.transform, "->"));
                sb.AppendLine(value);
            }
            sb.AppendLine();
        }

        private void SetDescription(string description, string prefix = null)
        {
            _text = description;
            if (EditorSettings.PersistenceWindowWriteToLog)
            {
                Debug.Log(string.Format(@"{0}
{1}", prefix ?? "Changes:", _text));
            }
        }

        private void WatchChangesChangedSlot(object[] parameters)
        {
            Repaint();
        }

        #endregion
    }
}
