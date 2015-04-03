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
using eDriven.Gui.Editor.Rendering;
using eDriven.Gui.Editor.Styles;
using eDriven.Gui.Reflection;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Dialogs
{
    [Obfuscation(Exclude = true)]
    internal sealed class StyleDeclarationDialog : EDrivenDialogBase
    {
#if DEBUG
    // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static StyleDeclarationDialog _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private StyleDeclarationDialog()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static StyleDeclarationDialog Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating StyleDeclarationDialog instance"));
#endif
                    _instance = (StyleDeclarationDialog) CreateInstance(typeof(StyleDeclarationDialog));
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

        private readonly List<IWizardStep> _renderers = new List<IWizardStep>();
        private readonly List<string> _titles = new List<string>();

        /// <summary>
        /// True if shouwing help (read by each of the steps)
        /// </summary>
        public bool ShowHelp;

        /// <summary>
        /// Dialog callback
        /// </summary>
        public Action Callback;

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        public void Initialize()
        {
            //Debug.Log("Dialog initializing...");

            _panelRenderer.ChromeStyle = StyleCache.Instance.PanelChromeSquared;

            Rectangle screenRectangle = Rectangle.FromSize(new Point(Screen.currentResolution.width, Screen.currentResolution.height));
            Rectangle winRectangle = Rectangle.FromSize(new Point(600, 400));
            position = winRectangle.CenterInside(screenRectangle).ToRect();
            minSize = new Vector2(240, 240);
            wantsMouseMove = true;

            _renderers.Clear();
            _renderers.Add(StyleDeclarationDialogStep1.Instance);
            _renderers.Add(StyleDeclarationDialogStep2.Instance);
            _renderers.Add(StyleDeclarationDialogStep3.Instance);
            _renderers.Add(StyleDeclarationCreationStep.Instance);

            _titles.Clear();
            _titles.Add("Step 1: Choose a subject");
            _titles.Add("Step 2: Choose style properties");
            _titles.Add("Step 3: Choose media queries");
            _titles.Add("Step 4: Creation settings");

            StyleDeclarationDialogStep1.Instance.GotoSignal.Connect(GotoSlot);
            StyleDeclarationDialogStep2.Instance.GotoSignal.Connect(GotoSlot);
            StyleDeclarationDialogStep3.Instance.GotoSignal.Connect(GotoSlot);
            StyleDeclarationCreationStep.Instance.GotoSignal.Connect(GotoSlot);

            // initialize the 1st -> not needed, since Step initializes
            //StyleDeclarationDialogStep1.Instance.Initialize();

            // initializes the 1st
            Step = 0;
            
            //PlayModeStateChangeEmitter.Instance.SelectionChangedSignal.Connect(SelectionChangedSlot);
        }

        /*private void SelectionChangedSlot(object[] parameters)
        {
            //Debug.Log("StyleDeclarationDialog: SelectionChangedSlot");
            Repaint();
        }*/

        ~StyleDeclarationDialog()
        {
            StyleDeclarationDialogStep1.Instance.GotoSignal.Disconnect(GotoSlot);
            StyleDeclarationDialogStep2.Instance.GotoSignal.Disconnect(GotoSlot);
            StyleDeclarationDialogStep3.Instance.GotoSignal.Disconnect(GotoSlot);
            StyleDeclarationCreationStep.Instance.GotoSignal.Disconnect(GotoSlot);

            //PlayModeStateChangeEmitter.Instance.SelectionChangedSignal.Disconnect(SelectionChangedSlot);
        }

        private void GotoSlot(object[] parameters)
        {
            int amount = (int) parameters[0];
            //Debug.Log("GotoSlot: " + amount);
            bool absolute = parameters.Length > 1 && (bool) parameters[1];

            if (absolute)
                Step = amount;
            else
                Step += amount;
        }

        private int _step = -1; // must be -1 to initialize properly

        private StyleDeclarationDataObject _data = new StyleDeclarationDataObject();
        /// <summary>
        /// Data is being set from outside when adding a new or editing the old declaration
        /// Whenever the data is set, we should initialize all the child views
        /// </summary>
        public StyleDeclarationDataObject Data
        {
            get { return _data; }
            set
            {
                _data = value;

                // if type supplied, we are in the edit mode -> proceed to the second tab
                if (!string.IsNullOrEmpty(_data.Type))
                {
                    // set module ID based on the type (bug fix)
                    try
                    {
                        _data.ModuleId = StyleModuleManager.Instance.GetOwnerModule(_data.Type).Id;
                    }
                    catch (GlobalTypeDictionaryException ex)
                    {
                        EditorUtility.DisplayDialog("Info", string.Format(@"Couldn't locate type [{0}].

It must be due to changing the type name or namespace.

Please choose the existing type.", _data.Type), "OK");

                        _data.Type = string.Empty;
                    }

                    if (!string.IsNullOrEmpty(_data.Type))
                    {
                        // select the type in the 1st view
                        StyleDeclarationDialogStep1.Instance.Select(_data.Type);

                        // proceed to second view
                        Step = 1;
                    }
                }
                Repaint();
            }
        }

        /// <summary>
        /// Step is used for rendering
        /// Whenever the step changes, initialize the corresponding view
        /// </summary>
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
// ReSharper disable InconsistentNaming
        void OnGUI() {
// ReSharper restore InconsistentNaming
// ReSharper restore UnusedMember.Local

            DialogContentWrapper.Start();

            /**
             * 1. Panel start
             * */
            _panelRenderer.RenderStart(new GUIContent(_titles[_step], TextureCache.Instance.StyleDeclarationAdd), true);

            ShowHelp = _panelRenderer.ClickedTools.Contains("help");

            /**
             * 2. Render the current step
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
            StyleDeclarationCreationStep.Instance.Process();

            if (null != Callback)
                Callback();

            Close();
        }

        public void Reset()
        {
            //_data = new AddStyleDeclarationDataObject { StyleSheet = StyleSheet };

            /*_renderers.Clear();
            _titles.Clear();*/

            Step = 0;
            foreach (IWizardStep step in _renderers)
            {
                step.Reset();
            }

            // clear style properties to empty the right style list
            Data.StyleProperties.Clear();
            Data.MediaQueries.Clear();
            Repaint();
        }
    }
}