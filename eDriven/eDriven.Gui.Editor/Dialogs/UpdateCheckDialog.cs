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
using eDriven.Gui.Editor.Update;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Dialogs
{
    [Obfuscation(Exclude = true)]
    internal sealed class UpdateCheckDialog : EDrivenDialogBase
    {
#if DEBUG
    // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static UpdateCheckDialog _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private UpdateCheckDialog()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static UpdateCheckDialog Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating UpdateCheckDialog instance"));
#endif
                    _instance = (UpdateCheckDialog)CreateInstance(typeof(UpdateCheckDialog));
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        private readonly PanelRenderer _panelRenderer = new PanelRenderer
        {
            Collapsible = false,
            //Tools = new List<ToolDescriptor>(new[] { new ToolDescriptor("help", TextureCache.Instance.Help) })
        };

        private readonly List<IWizardStep> _renderers = new List<IWizardStep>();

        private readonly List<string> _titles = new List<string>();

        private readonly List<string> _categories = new List<string>();

        private readonly Dictionary<string, bool> _categoriesDict = new Dictionary<string, bool>();

        public float ButtonHeight = 30;

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private void Initialize()
        {
            _panelRenderer.ChromeStyle = StyleCache.Instance.PanelChromeSquared;

            title = "eDriven Update Check";
            Rectangle screenRectangle = Rectangle.FromSize(new Point(Screen.currentResolution.width, Screen.currentResolution.height));
            Rectangle winRectangle = Rectangle.FromSize(new Point(600, 400));
            position = winRectangle.CenterInside(screenRectangle).ToRect();
            minSize = new Vector2(240, 240);
            wantsMouseMove = true;

            //Debug.Log("Adding renderers");

            _categories.Clear();
            _categoriesDict.Clear();

            var count = _data.Count;
            for (int i = 0; i < count; i++)
            {
                var cat = _data[i].Category;
                if (!_categories.Contains(cat))
                {
                    _categories.Add(cat);
                    _categoriesDict.Add(cat, true);
                }
            }

            if (_data.Count > 0)
                _maxMessageId = Math.Max(_maxMessageId, _data[0].Id);

            _step = 0;

            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            //Debug.Log("UpdateDisplay");
            _renderers.Clear();
            _titles.Clear();

            var filteredCategories = new List<string>();
            foreach (var key in _categoriesDict.Keys)
            {
                if (_categoriesDict[key]) // == true
                {
                    filteredCategories.Add(key);
                }
            }

            //Debug.Log("filteredCategories: " + filteredCategories.Count);

            List<InfoMessage> filteredData = new List<InfoMessage>();
            foreach (InfoMessage infoMessage in _data)
            {
                if (filteredCategories.Contains(infoMessage.Category))
                {
                    filteredData.Add(infoMessage);
                }
            }

            var index = 0;
            var count = filteredData.Count;
            foreach (InfoMessage data in filteredData)
            {
                var step = new UpdateCheckStep(data);
                _renderers.Add(step);
                _titles.Add(string.Format("Update {0}/{1}: " + data.Title, index + 1, count));
                index++;
            }

            //Debug.Log("Renderers: " + _renderers.Count);

            //Debug.Log("_categoriesDict: " + _categoriesDict.Count);

            Step = 0;
        }

//        ~UpdateCheckDialog()
//        {
//            
//        }

        private int _step;

        private int _tabIndex;

        private List<InfoMessage> _data = new List<InfoMessage>();
        public List<InfoMessage> Data
        {
            get { return _data; }
            set
            {
                _data = value;
                Initialize();
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

                    _maxMessageId = Math.Max(_maxMessageId, _data[_step].Id);
                }
            }
        }

        private bool _categoryChanged;
        private bool _selected = true;
        private bool _newSelected = true;

        private bool _showNews = true;

// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming
        void OnGUI() {
// ReSharper restore InconsistentNaming
// ReSharper restore UnusedMember.Local

            DialogContentWrapper.Start();

            bool changed = false;
            
            if (_renderers.Count == 0)
            {
                _panelRenderer.RenderStart(new GUIContent("No new updates", TextureCache.Instance.CheckForUpdates), true);

                if (_categories.Count > 1)
                    changed = RenderCategories();

                EditorGUILayout.BeginVertical(StyleCache.Instance.ScrollviewBackground, GUILayout.ExpandWidth(true));

                GUILayout.Label(GuiContentCache.Instance.NoUpdatesFound, StyleCache.Instance.CenteredLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                
                EditorGUILayout.EndVertical();  // panel chrome

                //GUILayout.Space(3);
                
                _panelRenderer.RenderEnd();
            }

            else
            {
//                if (_step > _renderers.Count-1)
//                    return;

                /**
                 * 1. Panel start
                 * */
                _panelRenderer.RenderStart(new GUIContent(_titles[_step], TextureCache.Instance.CheckForUpdates), true);

                if (_categories.Count > 1)
                    changed = RenderCategories();
            
                //GUILayout.Space(3);

                /**
                 * 2. Render current step
                 * */
                _renderers[_step].Render();

                /**
                 * 3. Render buttons
                 * */
                if (_renderers.Count > 0)
                    RenderButtons();

                GUILayout.Space(3);

                /**
                 * 4. Panel end
                 * */
                _panelRenderer.RenderEnd();

                //GUILayout.Label("Max: " + _maxMessageId);
            }

            DialogContentWrapper.End();

            if (Event.current.type == EventType.MouseMove)
                Repaint();

            if (changed)
            {
                //Debug.Log("Category changed");
                UpdateDisplay();
            }
        }

        private bool RenderCategories()
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandHeight(false));

            _categoryChanged = false;
            foreach (var category in _categories)
            {
                _selected = _categoriesDict[category];
                _newSelected = GUILayout.Toggle(_selected, new GUIContent(category, _selected ? TextureCache.Instance.LightBulbOn : TextureCache.Instance.LightBulbOff), StyleCache.Instance.Toggle, GUILayout.Height(ButtonHeight));
                
                if (_newSelected != _selected)
                {
                    _categoriesDict[category] = _newSelected;
                    _selected = _newSelected;
                    _categoryChanged = true;
                }
            }
            //_showNews = GUILayout.Toggle(_showNews, new GUIContent("News", TextureCache.Instance.LightBulbOff), StyleCache.Instance.Toggle);

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUILayout.Space(3);

            return _categoryChanged;
        }

/*
        public void Init()
        {
            _step = 0;
        }
*/
        private int _maxMessageId;

        void RenderButtons()
        {
            if (_renderers.Count == 0)
                return;

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
            _maxMessageId = Math.Max(_maxMessageId, ((UpdateCheckStep)_renderers[_step]).Data.Id);
            EditorSettings.LastMessageId = _maxMessageId;
            //Debug.Log("EditorSettings.LastMessageId: " + EditorSettings.LastMessageId);
            Close();
        }

        public void Reset()
        {
            _data = new List<InfoMessage>();

            Step = 0;
            foreach (IWizardStep step in _renderers)
            {
                step.Reset();
            }
        }
    }
}