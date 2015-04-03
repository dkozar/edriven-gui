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

[CustomEditor(typeof(ListItemDataProvider))]
[CanEditMultipleObjects]
[Serializable]
public class ListItemDataProviderEditor : ComponentEditorBase
{
    public SerializedProperty Keys;
    public SerializedProperty Values;

    [NonSerialized]
    private string _errorMessage;
    [NonSerialized]
    private int _removeAtIndex = -1;
    [NonSerialized]
    private int _insertAtIndex = -1;
    [NonSerialized]
    private bool _doRemove;

    private bool _oldEnabled;

    protected override void Initialize()
    {
        EditorPanelKey = "eDrivenStringListDataProviderEditorExpanded";
        EditorPanelTitle = GuiContentCache.Instance.PanelListItemDataProviderTitle;
        AutoConfigure = false;

        Keys = serializedObject.FindProperty("Keys");
        if (!Keys.isArray)
        {
            throw new Exception("Keys is not an array");
        }

        Values = serializedObject.FindProperty("Values");
        if (!Values.isArray)
        {
            throw new Exception("Values is not an array");
        }
    }

    protected override void RenderMainOptions()
    {
        if (Keys.arraySize == 0)
        {
            EditorGUILayout.HelpBox(@"Add key/value items as data provider for the control.", MessageType.Info, true);
        }

        int count = Keys.arraySize;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Key");
        EditorGUILayout.LabelField("Value");
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < count; i++)
        {
            string oldKey = Keys.GetArrayElementAtIndex(i).stringValue;
            string oldValue = Values.GetArrayElementAtIndex(i).stringValue;

            EditorGUILayout.BeginHorizontal();

            string newKey = EditorGUILayout.TextField(string.Empty, oldKey, GUILayout.ExpandWidth(true));
            string newValue = EditorGUILayout.TextField(string.Empty, oldValue, GUILayout.ExpandWidth(true));

            if (GUILayout.Button(GuiContentCache.Instance.AddItem, StyleCache.Instance.ImageOnlyButton, GUILayout.ExpandWidth(false)))//, StyleCache.Instance.ControlButtonStyle))
            {
                _insertAtIndex = i;
            }

            if (GUILayout.Button(GuiContentCache.Instance.RemoveItem, StyleCache.Instance.ImageOnlyButton, GUILayout.ExpandWidth(false)))
            {
                _removeAtIndex = i;
                _doRemove = true;
            }

            if (null != newKey)
            {
                Keys.GetArrayElementAtIndex(i).stringValue = newKey;
            }

            if (null != newValue)
            {
                Values.GetArrayElementAtIndex(i).stringValue = newValue;
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);
        }

        if (GUILayout.Button(GuiContentCache.Instance.AddItemWithText, StyleCache.Instance.Button, GUILayout.Height(30)))//, StyleCache.Instance.ControlButtonStyle))
        {
            Keys.arraySize++;
            Keys.GetArrayElementAtIndex(Math.Max(Keys.arraySize - 1, 0)).stringValue = string.Empty;

            Values.arraySize++;
            Values.GetArrayElementAtIndex(Math.Max(Values.arraySize - 1, 0)).stringValue = string.Empty;

            SetEditorDirty();
        }

        if (!string.IsNullOrEmpty(_errorMessage))
        {
            EditorGUILayout.HelpBox(_errorMessage, MessageType.Error, true);
        }
        
        _oldEnabled = GUI.enabled;
        GUI.enabled = IsDirty;
        if (GUILayout.Button(GuiContentCache.Instance.Apply, StyleCache.Instance.Button, GUILayout.Height(30)))//, StyleCache.Instance.ControlButtonStyle))
        {
            if (!IsValid())
            {
                _errorMessage = "All items should have keys and values";
                return;
            }
            Apply();
        }
        
        GUI.enabled = _oldEnabled;

        if (_doRemove)
        {
            _doRemove = false;
            if (EditorApplication.isPlaying)
            {
                DoRemove();
            }
            else if (EditorUtility.DisplayDialog("Remove element?", string.Format(@"Are you sure you want to remove button group element?"), "OK", "Cancel"))
            {
                DoRemove();
            }
        }

        if (_insertAtIndex > -1)
        {
            Keys.InsertArrayElementAtIndex(_insertAtIndex);
            Keys.GetArrayElementAtIndex(_insertAtIndex).stringValue = string.Empty;

            Values.InsertArrayElementAtIndex(_insertAtIndex);
            Values.GetArrayElementAtIndex(_insertAtIndex).stringValue = string.Empty;

            SetEditorDirty();
            _insertAtIndex = -1;
        }

        if (GUI.changed)
        {
            SetEditorDirty();
        }
    }

    public override void SetEditorDirty()
    {
        base.SetEditorDirty();
        _errorMessage = null;
    }

    private void DoRemove()
    {
        Keys.DeleteArrayElementAtIndex(_removeAtIndex);
        Values.DeleteArrayElementAtIndex(_removeAtIndex);
        _removeAtIndex = -1;
        SetEditorDirty();
    }

    protected override void RenderExtendedOptions() { }

    override protected bool IsValid()
    {
        int count = Keys.arraySize;
        for (int i = 0; i < count; i++)
        {
            if (string.IsNullOrEmpty(Keys.GetArrayElementAtIndex(i).stringValue) ||
                string.IsNullOrEmpty(Values.GetArrayElementAtIndex(i).stringValue))
                return false;
        }
        return true;
    }
}