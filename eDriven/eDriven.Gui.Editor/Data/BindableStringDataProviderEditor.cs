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
using eDriven.Gui.Designer.Data;
using eDriven.Gui.Editor;
using eDriven.Gui.Editor.Rendering;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(BindableStringDataProvider))]
[CanEditMultipleObjects]
[Serializable]
public class BindableStringDataProviderEditor : ComponentEditorBase
{
    public SerializedProperty Data;

    [NonSerialized]
    private string _errorMessage = "No data source referenced";

    protected override void Initialize()
    {
        //eDrivenEditorWindowBase.SerializedObject = serializedObject;

        EditorPanelKey = "eDrivenBindableStringListDataProviderExpanded";
        EditorPanelTitle = GuiContentCache.Instance.PanelBindableStringListDataProviderTitle;

        Data = serializedObject.FindProperty("Data");
        if (!Data.isArray)
        {
            throw new Exception("Data is not an array");
        }
    }

    protected override void RenderMainOptions()
    {
        if (GUILayout.Button(new GUIContent("Reference data source", TextureCache.Instance.Add), StyleCache.Instance.Button, GUILayout.Height(30)))//, StyleCache.Instance.ControlButtonStyle))
        {
        
        }

        if (!string.IsNullOrEmpty(_errorMessage))
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox(_errorMessage, MessageType.Error, true);
            //if (GUILayout.Button(new GUIContent("Dismiss", TextureCache.Instance.Cancel), StyleCache.Instance.Button, GUILayout.Height(38)))
            //{
            //    _errorMessage = null;
            //}
            GUILayout.EndHorizontal();
        }
    }

    protected override void RenderExtendedOptions()
    {
    
    }
}