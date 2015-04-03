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

using System.Text;
using eDriven.Core.Signals;
using eDriven.Gui.Editor.Rendering;
using eDriven.Gui.Styles;
using eDriven.Gui.Styles.Serialization;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Dialogs
{
    internal class StyleDeclarationCreationStep : IWizardStep
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif
        private const int LabelWidth = 120;

        #region Singleton

        private static StyleDeclarationCreationStep _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private StyleDeclarationCreationStep()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static StyleDeclarationCreationStep Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating StyleDeclarationCreationStep instance"));
#endif
                    _instance = new StyleDeclarationCreationStep();
                }

                return _instance;
            }
        }

        #endregion

        private StyleDeclarationDataObject _data;

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        public void Initialize()
        {
            _data = StyleDeclarationDialog.Instance.Data;
            if (null == _data)
                return;

            _stylePropertiesAsString = string.Empty;

            StringBuilder sb = new StringBuilder();
            var count = _data.StyleProperties.Count;
            var index = 0;
            foreach (StyleProperty stylePropertyDto in _data.StyleProperties)
            {
                //sb.AppendLine(string.Format("{0} [{1}]", styleAttribute.Name, styleAttribute.Type.Name));
                if (index < count-1)
                    sb.AppendLine(stylePropertyDto.Name);
                else
                    sb.Append(stylePropertyDto.Name);
                index++;
            }

            _stylePropertiesAsString = sb.ToString();

            _mediaQueriesAsString = string.Empty;

            sb = new StringBuilder();
            count = _data.MediaQueries.Count;
            index = 0;
            foreach (MediaQuery query in _data.MediaQueries)
            {
                //sb.AppendLine(string.Format("{0} [{1}]", styleAttribute.Name, styleAttribute.Type.Name));
                if (index < count - 1)
                    sb.AppendLine(query.Name);
                else
                    sb.Append(query.Name);
                index++;
            }

            _mediaQueriesAsString = sb.ToString();

            _focusSet = false;

            //// init error msg
            //_errorMsg = GetErrorMessage();
        }

        #region Static

        private const string InputTextFocusId = "eDrivenComponentTypeInputField";

        #endregion

        #region Properties

        private readonly Signal _gotoSignal = new Signal();
        public Signal GotoSignal
        {
            get { return _gotoSignal; }
        }

        public bool IsValid
        {
            get { return !_isError; }
        }

        #endregion

        #region Members
        
        private string _stylePropertiesAsString = string.Empty;
        private string _mediaQueriesAsString = string.Empty;
        
        private Vector2 _scrollPosition;

        private bool _isError;

        #endregion

        private bool _oldEnabled;

        public void Render()
        {
            if (!_focusSet)
            {
                GUI.SetNextControlName(InputTextFocusId);
            }

            //GUILayout.Space(-5);
            _isError = false;

            if (StyleDeclarationDialog.Instance.ShowHelp)
            {
                EditorGUILayout.HelpBox(Help.StyleDeclarationCreationSettings, MessageType.Info, true);
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandWidth(true));

            #region Selector

            // selector title
            GUILayout.Label(new GUIContent(" Selector: ", TextureCache.Instance.Component), StyleCache.Instance.Label);

            GUILayout.BeginVertical(StyleCache.Instance.Fieldset);

            // control type
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.Label(new GUIContent("Subject: "), StyleCache.Instance.Label,
                GUILayout.Width(LabelWidth));
            if (!_data.AllowSubjectOmmision)
            {
                _oldEnabled = GUI.enabled;
                GUI.enabled = false;
            }

            _data.Type = GUILayout.TextField(_data.Type);

            if (!_data.AllowSubjectOmmision)
            {
                GUI.enabled = _oldEnabled;
            }

            GUILayout.EndHorizontal();

            // class
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.Label(new GUIContent("Class: "), StyleCache.Instance.Label, GUILayout.Width(LabelWidth));

            _data.Class = GUILayout.TextField(_data.Class);

            GUILayout.EndHorizontal();

            // ID
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.Label(new GUIContent("ID: "), StyleCache.Instance.Label, GUILayout.Width(LabelWidth));
            _data.Id = GUILayout.TextField(_data.Id);
            GUILayout.EndHorizontal();

            if (string.IsNullOrEmpty(_data.Type) && string.IsNullOrEmpty(_data.Class) && string.IsNullOrEmpty(_data.Id))
            {
                EditorGUILayout.HelpBox(@"Selector cannot be empty.
Choose at least one of the three selector options!", MessageType.Error, true);
                _isError = true;
            }

            GUILayout.Space(8);

            if (!_isError)
            {
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                GUILayout.Label(new GUIContent("CSS notation: "), StyleCache.Instance.Label, GUILayout.Width(LabelWidth));
                var selector = string.Empty;
                if (!string.IsNullOrEmpty(_data.Type))
                {
                    selector += string.Format(@"[{0}]", _data.Type);
                }
                if (!string.IsNullOrEmpty(_data.Class))
                {
                    selector += string.Format(@".{0}", _data.Class);
                }
                if (!string.IsNullOrEmpty(_data.Id))
                {
                    selector += string.Format(@"#{0}", _data.Id);
                }
                GUILayout.Label(selector, EditorStyles.largeLabel);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            GUILayout.Space(10);

            #endregion

            GUILayout.BeginHorizontal();

            #region Properties

            GUILayout.BeginVertical();

            GUILayout.Label(new GUIContent(" Properties:", TextureCache.Instance.Style), StyleCache.Instance.Label);

            GUILayout.BeginVertical(StyleCache.Instance.Fieldset);

            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.Label(_stylePropertiesAsString, StyleCache.Instance.Label);
            GUILayout.EndVertical();

            if (_stylePropertiesAsString.Length == 0)
            {
                EditorGUILayout.HelpBox(@"No style properties selected.
Choose at least one of the style properties!", MessageType.Error, true);
                _isError = true;
            }

            GUILayout.EndVertical();

            GUILayout.Space(4);

            GUILayout.EndVertical();

            #endregion

            #region Media queries

            GUILayout.BeginVertical();

            GUILayout.Label(new GUIContent(" Media queries:", TextureCache.Instance.MediaQuery), StyleCache.Instance.Label);

            GUILayout.BeginVertical(StyleCache.Instance.Fieldset);

            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUILayout.Label(_mediaQueriesAsString, StyleCache.Instance.Label);
            GUILayout.EndVertical();

            /*if (_mediaQueriesAsString.Length == 0)
            {
                EditorGUILayout.HelpBox(@"No media queries applied.", MessageType.Info, true);
                _isError = true;
            }*/

            GUILayout.EndVertical();

            GUILayout.Space(4);

            GUILayout.EndVertical();

            #endregion

            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.EndScrollView();

            GUILayout.Space(1);

            if (!_focusSet)
            {
                GUI.FocusControl(InputTextFocusId);
                _focusSet = true;
            }
        }

        public void Reset()
        {
            //_isChosen = false;
        }

        private bool _focusSet;

        /// <summary>
        /// Processes the last wizard step
        /// </summary>
        internal void Process()
        {
            _focusSet = false;

            // grab the _data
            //var data = StyleDeclarationDialog.Instance.Data;
            //data.StyleSheet.Execute(data.GetClassname());
            //AddStyleDeclarationPersistedData persistedData = AddStyleDeclarationPersistedData.FromDataObject(data);
            //PersistedDataProcessingLogic.Process(persistedData);
        }
    }
}