using System;
using System.Reflection;
using eDriven.Extensions.ExampleControl2;
using eDriven.Gui.Editor;
using UnityEditor;

[CustomEditor(typeof(ExampleAdapter2))]
[CanEditMultipleObjects]
public class ExampleEditor2 : ComponentEditor
{
    public SerializedProperty LabelWidth;
    public SerializedProperty SimulateSending;

    /// <summary>
    /// Note: method name OnEnableImpl (eDriven.Gui 1.6) was renamed to Initialize (eDriven.Gui 1.7)
    /// </summary>
    [Obfuscation(Exclude = true)]
// ReSharper disable UnusedMember.Local
    protected override void Initialize()
// ReSharper restore UnusedMember.Local
    {
        base.Initialize();

        LabelWidth = serializedObject.FindProperty("LabelWidth");
        SimulateSending = serializedObject.FindProperty("SimulateSending");

        Hide("padding"); // hide Padding panel
    }

    /// <summary>
    /// Rendering controls at the end of the main panel
    /// </summary>
    protected override void RenderMainOptions()
    {
        base.RenderMainOptions();

        LabelWidth.intValue = EditorGUILayout.IntField("LabelWidth", LabelWidth.intValue);
        //SimulateSending.boolValue = EditorGUILayout.Toggle("SimulateSending", SimulateSending.boolValue);
        SimulateSending.enumValueIndex = (int)(SendMode)EditorGUILayout.EnumPopup(
            "SimulateSending",
            (SendMode)Enum.GetValues(typeof(SendMode)).GetValue(SimulateSending.enumValueIndex)
        );

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