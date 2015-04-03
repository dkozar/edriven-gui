using System;
using System.Reflection;
using eDriven.Gui.Components;
using eDriven.Gui.Editor;
using UnityEditor;

[CustomEditor(typeof(ExampleAdapter))]
[CanEditMultipleObjects]
public class ExampleEditor : ComponentEditor
{
    public SerializedProperty Text;
    public SerializedProperty ButtonText;
    public SerializedProperty BoolExample;
    public SerializedProperty EnumExample;

    /// <summary>
    /// Note: method name OnEnableImpl (eDriven.Gui 1.6) will be renamed to Initialize (eDriven.Gui 1.7)
    /// </summary>
    [Obfuscation(Exclude = true)]
// ReSharper disable UnusedMember.Local
    protected override void Initialize()
// ReSharper restore UnusedMember.Local
    {
        base.Initialize();

        Text = serializedObject.FindProperty("Text");
        ButtonText = serializedObject.FindProperty("ButtonText");

        #region Example

        BoolExample = serializedObject.FindProperty("BoolExample");
        EnumExample = serializedObject.FindProperty("EnumExample");

        #endregion

        //Hide(PADDING); // hide Padding panel
    }

    /// <summary>
    /// Rendering controls at the end of the main panel
    /// </summary>
    protected override void RenderMainOptions()
    {
        base.RenderMainOptions();

        Text.stringValue = EditorGUILayout.TextField("Text", Text.stringValue);
        ButtonText.stringValue = EditorGUILayout.TextField("Button text", ButtonText.stringValue);

        #region Example

        BoolExample.boolValue = EditorGUILayout.Toggle("BoolExample", BoolExample.boolValue);
        EnumExample.enumValueIndex = (int)(Direction)EditorGUILayout.EnumPopup(
            "EnumExample",
            (Direction)Enum.GetValues(typeof(Direction)).GetValue(EnumExample.enumValueIndex)
        );

        #endregion

        EditorGUILayout.Space();
    }

    #region Example panel

    //private const string ExampleSettingsVisibleKey = "eDrivenExampleSettingsVisible"; // unique key
    //public static bool ExampleSettingsVisible // Editor setting example (saved by editor)
    //{
    //    get { return EditorPrefs.GetBool(ExampleSettingsVisibleKey); }
    //    set
    //    {
    //        if (value == EditorPrefs.GetBool(ExampleSettingsVisibleKey))
    //            return;

    //        EditorPrefs.SetBool(ExampleSettingsVisibleKey, value);
    //    }
    //}

    //private readonly PanelRenderer _examplePanelRenderer = new PanelRenderer();
    //private readonly GUIContent _examplePanelHeader = new GUIContent("Example panel", (Texture)Resources.Load("eDriven/Editor/Icons/disk_multiple"));

    ///// <summary>
    ///// Rendering custom panels
    ///// </summary>
    //protected override void RenderExtendedOptions()
    //{
    //    base.RenderExtendedOptions();

    //    /* panel start */
    //    ExampleSettingsVisible = _examplePanelRenderer.RenderStart(_examplePanelHeader, ExampleSettingsVisible);

    //    if (ExampleSettingsVisible)
    //    {
    //        GUILayout.Label("Example label");

    //        if (GUILayout.Button("Example button"))
    //        {
    //            Debug.Log("Example button clicked");
    //        }

    //        /* panel end */
    //        _examplePanelRenderer.RenderEnd();
    //    }
            
    //}

    #endregion

}