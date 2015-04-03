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

using eDriven.Core.Signals;
using eDriven.Gui.Editor.Rendering;
using eDriven.Gui.Editor.Update;
using eDriven.Gui.Util;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Dialogs
{
    internal class UpdateCheckStep : IWizardStep
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        private readonly Signal _gotoSignal = new Signal();
        public Signal GotoSignal
        {
            get { return _gotoSignal; }
        }

        private Vector2 _scrollPosition;

        public GUIContent StageContent;

        private int _selectedIndex = -1;
        private int _newIndex = -1;

        private GUIContent[] _contents = new GUIContent[]{};

        private readonly InfoMessage _data;
        public InfoMessage Data
        {
            get { return _data; }
        }

        public UpdateCheckStep(InfoMessage message)
        {
            _data = message;
        }

        public void Render()
        {
            GUI.backgroundColor = ColorMixer.FromHex(0x335fd8).ToColor();

            GUI.backgroundColor = Color.white;

            EditorGUILayout.BeginVertical(StyleCache.Instance.ScrollviewBackground, GUILayout.ExpandWidth(true));

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));

            //GUILayout.Label(new GUIContent("ID: " + _message.Id), StyleCache.Instance.WrongSelection, GUILayout.ExpandWidth(false));//, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.Label(new GUIContent(_data.Title), StyleCache.Instance.Title, GUILayout.ExpandWidth(true));//, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.Box(new GUIContent(_data.Message), StyleCache.Instance.NormalBox, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true));
            if (!string.IsNullOrEmpty(_data.Url))
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent(string.IsNullOrEmpty(_data.UrlText) ? "Open Link" : _data.UrlText, TextureCache.Instance.CheckForUpdatesLink), StyleCache.Instance.BigLightButton, GUILayout.Height(30), GUILayout.ExpandWidth(false)))
                {
                    //Debug.Log("Opening: " + _message.Url);
                    Application.OpenURL(_data.Url);
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            GUILayout.Space(3);

            EditorGUILayout.EndVertical();  // panel chrome

            GUILayout.Space(1);
        }

        public bool IsValid
        {
            get { return true; }
        }

        public void Initialize()
        {
            //throw new NotImplementedException();
        }

        public void Reset()
        {
            _selectedIndex = -1;
        }
    }
}