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
using System.Reflection;
using eDriven.Gui.Editor.Dialogs;
using eDriven.Gui.Editor.Persistence;
using eDriven.Gui.Editor.Rendering;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor
{
// ReSharper disable InconsistentNaming
    public abstract class eDrivenEditorBase : UnityEditor.Editor
// ReSharper restore InconsistentNaming
    {
#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

        private readonly List<string> _hidden = new List<string>();
        private readonly PanelRenderer _mainPanelRenderer = new PanelRenderer();

        private AdapterAnalysis _adapterAnalysis;
        internal AdapterAnalysis AdapterAnalysis
        {
            get { return _adapterAnalysis; }
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        /* By overriding this method, we are avoiding the usage of InspectorContentWrapper */
        /*public override bool UseDefaultMargins()
        {
            return false;
        }*/

        // TODO:
        // 1. draw the script picker ABOVE the panel chrome
        // 2. get component name and render it inside the panel chrome

        // ReSharper disable UnusedMember.Local
        [Obfuscation(Exclude = true)]
        void OnEnable()
        // ReSharper restore UnusedMember.Local
        {
            //Debug.Log("ComponentEditor OnEnable: " + target);
            Hide("margins"); // TEMP

            // Unity bug: checking a target causes error
            // Actually found the additional info: 
            // "Instance of StageEditor couldn't be created because there is no script with that name."
            // the source of this error is because I'm recompiling the actual editor assembly (eDriven.Gui.Editor)
            // so doing this, Unity cannot find the appropriate editor class at a particular moment
            // when rebuilding scripts in the Playground, it works fine

            _adapterAnalysis = new AdapterAnalysis(target);

#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format(@"===== AdapterAnalysis =====
{0}", _adapterAnalysis));
            }
#endif

            Initialize();
            //Debug.Log(string.Format("OnEnable. Instance ID: {0}. isPlaying: {1}. isPlayingOrWillChangePlaymode: {2}.", target.GetInstanceID(), EditorApplication.isPlaying, EditorApplication.isPlayingOrWillChangePlaymode));

            if (EditorApplication.isPlaying && EditorSettings.WatchChanges)
            {
                PersistenceManager.Instance.Watch(target);
            }
        }

        protected abstract void Initialize();

        protected bool AutoConfigure = true;

        private bool _isDirty;
        protected bool IsDirty
        {
            get { return _isDirty; }
        }

        public virtual void SetEditorDirty()
        {
            _isDirty = true;
        }

        public virtual void Undirty()
        {
            _isDirty = false;
        }
        
// ReSharper disable UnusedMember.Global
        protected List<string> Hidden
// ReSharper restore UnusedMember.Global
        {
            get
            {
                return _hidden;
            }
        }

        #region Expanded

        private bool _expanded = true;
        public virtual bool Expanded
        {
            get { return _expanded; }
            set { _expanded = value; }
        }

        public GUIContent EditorPanelTitle = GuiContentCache.Instance.PanelPropertiesTitle; // default

        protected List<ToolDescriptor> MainPanelTools
        {
            get
            {
                return _mainPanelRenderer.Tools;
            }
            set
            {
                _mainPanelRenderer.Tools = value;
            }
        }

        protected List<string> MainPanelClickedTools
        {
            get
            {
                return _mainPanelRenderer.ClickedTools;
            }
        }

        #endregion

        protected void Hide(string keyword)
        {
            if (!_hidden.Contains(keyword))
                _hidden.Add(keyword);
        }

        protected void Hide(params string[] keywords)
        {
            foreach (string keyword in keywords)
            {
                if (!_hidden.Contains(keyword))
                    _hidden.Add(keyword);
            }
        }

        protected void Show(params string[] keywords)
        {
            foreach (string keyword in keywords)
            {
                if (_hidden.Contains(keyword))
                    _hidden.Remove(keyword);
            }
        }

        protected bool IsHidden(string keyword)
        {
            return _hidden.Contains(keyword);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            InspectorContentWrapper.Start();

            /*EditorSettings.ComponentDescriptorMainExpanded =*/
            Expanded = _mainPanelRenderer.RenderStart(EditorPanelTitle, Expanded);

            if (Expanded)
            {
                RenderMainOptions();
                _mainPanelRenderer.RenderEnd();
            }
            else
            {
                BlankCallback();
            }

            RenderExtendedOptions();

            /**
             * The order of applying the propertis MUST be as follows:
             * 1. Applying modified properties by inspector (serializedObject.ApplyModifiedProperties())
             * 2. Applying changes made in play mode
             * This is because serializedObject.ApplyModifiedProperties() re-applies the properties when back from play mode
             * */
            serializedObject.ApplyModifiedProperties();
            //PersistenceManager.Instance.CallApplyChanges();
            //PersistenceManager.Instance.ApplyChanges();
            
            if (AutoConfigure)
            {
                if (GUI.changed)
                {
#if DEBUG
                    if (DebugMode)
                    {
                        Debug.Log("GUI.changed: " + GUI.changed + " target: " + target);
                    }
#endif
                    Apply();
                }
            }

            InspectorContentWrapper.End();

            if (Event.current.type == EventType.MouseMove)
                Repaint();
        }

        protected virtual void BlankCallback()
        {
            
        }

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
        /// <summary>
        /// Base method, that should never be overriden
        /// </summary>
        public void BaseOnInspectorGUI()
// ReSharper restore InconsistentNaming
// ReSharper restore UnusedMember.Global
        {
            base.OnInspectorGUI();
        }

        protected abstract void Apply();

        /// <summary>
        /// Renders the main content<br/>
        /// That is the content within the main (properties) panel<br/>
        /// No RenderStart and RenderEnd calls needed on panel renderer
        /// </summary>
        protected abstract void RenderMainOptions();

        /// <summary>
        /// Renders extended options<br/>
        /// You could group options within other panels using panel renderer's RenderStart and RenderEnd calls
        /// </summary>
        protected abstract void RenderExtendedOptions();
    }
}