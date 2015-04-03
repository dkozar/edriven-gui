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
using eDriven.Core.Geom;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Editor.Processing;
using eDriven.Gui.Editor.Rendering;
using UnityEngine;

namespace eDriven.Gui.Editor.Dialogs
{
    [Obfuscation(Exclude = true)]
    internal sealed class AddEventHandlerDialog : EDrivenDialogBase
    {
#if DEBUG
    // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static AddEventHandlerDialog _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private AddEventHandlerDialog()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static AddEventHandlerDialog Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating AddEventHandlerDialog instance"));
#endif
                    _instance = (AddEventHandlerDialog) CreateInstance(typeof(AddEventHandlerDialog));
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        private readonly PanelRenderer _panelRenderer = new PanelRenderer
        {
            Collapsible = false,
            Tools = new List<ToolDescriptor>(new[] { new ToolDescriptor("help", TextureCache.Instance.Help) })
        };

        private readonly Breadcrumbs _breadcrumbs = new Breadcrumbs();

        private readonly List<IWizardStep> _renderers = new List<IWizardStep>();

        private readonly List<string> _titles = new List<string>();

        public bool ShowHelp;

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private void Initialize()
        {
            _panelRenderer.ChromeStyle = StyleCache.Instance.PanelChromeSquared;

            title = "Adding Event Handler";
            Rectangle screenRectangle = Rectangle.FromSize(new Point(Screen.currentResolution.width, Screen.currentResolution.height));
            Rectangle winRectangle = Rectangle.FromSize(new Point(600, 400));
            position = winRectangle.CenterInside(screenRectangle).ToRect();
            minSize = new Vector2(240, 240);
            wantsMouseMove = true;

            //Debug.Log("Adding renderers");
            _renderers.Add(EventListStep.Instance);
            _renderers.Add(ScriptActionsStep.Instance);
            _renderers.Add(EventHandlerListStep.Instance);
            _renderers.Add(EventHandlerCreationStep.Instance);

            _titles.Add("Step 1: Choose event type");
            _titles.Add("Step 2: Choose script");
            _titles.Add("Step 3: Choose event handler");
            _titles.Add("Step 4: Creation settings");

            //Debug.Log("_titles.Count: " + _titles.Count);

            EventListStep.Instance.GotoSignal.Connect(GotoSlot);
            ScriptActionsStep.Instance.GotoSignal.Connect(GotoSlot);
            EventHandlerListStep.Instance.GotoSignal.Connect(GotoSlot);
            EventHandlerCreationStep.Instance.GotoSignal.Connect(GotoSlot);

            // initialize 1st
            EventListStep.Instance.Initialize();

            //_step = -1;
            Step = 0;

            PlayModeStateChangeEmitter.Instance.SelectionChangedSignal.Connect(SelectionChangedSlot);
        }

        private void SelectionChangedSlot(object[] parameters)
        {
            //Debug.Log("AddEventHandlerDialog: SelectionChangedSlot");
            Repaint();
        }

        ~AddEventHandlerDialog()
        {
            EventListStep.Instance.GotoSignal.Disconnect(GotoSlot);
            ScriptActionsStep.Instance.GotoSignal.Disconnect(GotoSlot);
            EventHandlerListStep.Instance.GotoSignal.Disconnect(GotoSlot);
            EventHandlerCreationStep.Instance.GotoSignal.Disconnect(GotoSlot);

            PlayModeStateChangeEmitter.Instance.SelectionChangedSignal.Disconnect(SelectionChangedSlot);
        }

        private void GotoSlot(object[] parameters)
        {
            int amount = (int) parameters[0];
            //Debug.Log("GotoSlot: " + amount);
            bool absolute = parameters.Length > 1 ? (bool) parameters[1] : false;

            if (absolute)
                Step = amount;
            else
                Step += amount;
        }

        private int _step;

        private ComponentAdapter _adapter;

        private AddHandlerDataObject _data;
        public AddHandlerDataObject Data
        {
            get { return _data; }
        }

        public ComponentAdapter Adapter
        {
            get
            {
                return _adapter;
            }
            set
            {
                _adapter = value;
                _data.Adapter = _adapter;
                _breadcrumbs.RefreshPath();
                _data.Reset();
                EventListStep.Instance.Process();
            }
        }

        public int Step
        {
            get { return _step; }
            set
            {
                if (value == -1 || _renderers.Count == 0)
                {
                    _step = -1;
                    return;
                }

                //Debug.Log("_renderers.Count: " + _renderers.Count);
                int prev = _step;

                _step = Math.Min(Math.Max(value, 0), _renderers.Count-1);
                if (prev != _step)
                {
                    if (null != _renderers[_step])
                        _renderers[_step].Initialize();
                }
            }
        }

// ReSharper disable UnusedMember.Local
        void OnGUI() {
// ReSharper restore UnusedMember.Local

            DialogContentWrapper.Start();

            /**
             * 1. Panel start
             * */
            //_panelRenderer.RenderStart(GuiContentCache.Instance.CreateEventHandler, true);
            _panelRenderer.RenderStart(new GUIContent(_titles[_step], TextureCache.Instance.EventHandlerAddMapping), true);

            ShowHelp = _panelRenderer.ClickedTools.Contains("help");

            //GUILayout.Space(3);

            /**
             * 1a. Render breadcrumbs
             * */
            GUI.enabled = false;
            _breadcrumbs.Render();
            GUI.enabled = true;

            GUILayout.Space(6);

            /**
             * 2. Render current step
             * */
            _renderers[_step].Render();

            /**
             * 3. Render buttons
             * */
            RenderButtons();

            GUILayout.Space(3);

            /**
             * 4. Panel end
             * */
            _panelRenderer.RenderEnd();

            DialogContentWrapper.End();

            if (Event.current.type == EventType.MouseMove)
                Repaint();
        }

/*
        public void Init()
        {
            _step = 0;
        }
*/

        void RenderButtons()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUI.enabled = _step > 0;
            // previous button
            if (GUILayout.Button(GuiContentCache.Instance.WizardPrevious, StyleCache.Instance.BigButton))
            {
                Step--;
            }
            GUI.enabled = true;

            if (_step == _renderers.Count - 1) // && _renderers[_step].IsValid)
            {
                // finish button
                GUI.enabled = _renderers[_step].IsValid;
                if (GUILayout.Button(GuiContentCache.Instance.WizardFinish, StyleCache.Instance.BigButton))
                {
                    Submit();
                }
                GUI.enabled = true;
            }
            else
            {
                // next button
                GUI.enabled = _step != _renderers.Count - 1 && _renderers[_step].IsValid;

                if (GUILayout.Button(GuiContentCache.Instance.WizardNext, StyleCache.Instance.BigButton))
                {
                    Step++;
                }
                GUI.enabled = true;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void Submit()
        {
            //Debug.Log("Submit");
            EventHandlerCreationStep.Instance.Process();
            Close();
        }

        public void Reset()
        {
            _data = new AddHandlerDataObject {Adapter = Adapter};

            Step = 0;
            foreach (IWizardStep step in _renderers)
            {
                step.Reset();
            }
        }
    }
}