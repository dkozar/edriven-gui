//using System;
//using System.Reflection;
//using eDriven.Gui.Components;
//using eDriven.Gui.Editor;
//using UnityEditor;

//[CustomEditor(typeof(SliderAdapter))]
//[CanEditMultipleObjects]
//[Obfuscation(Exclude = true)]
//public class SliderEditor : ComponentEditor
//{
//    public SerializedProperty Direction;
//    public SerializedProperty NumberMode;
//    public SerializedProperty Reverse;
//    public SerializedProperty Value;
//    public SerializedProperty MinValue;
//    public SerializedProperty MaxValue;
//    public SerializedProperty Step;

//    /// <summary>
//    /// Note: method name OnEnableImpl (eDriven.Gui 1.6) was renamed to Initialize (eDriven.Gui 1.7)
//    /// </summary>
//    [Obfuscation(Exclude = true)]
//// ReSharper disable UnusedMember.Local
//    protected override void Initialize()
//// ReSharper restore UnusedMember.Local
//    {
//        base.Initialize();

//        Direction = serializedObject.FindProperty("Direction");
//        NumberMode = serializedObject.FindProperty("NumberMode");
//        Reverse = serializedObject.FindProperty("Reverse");
//        Value = serializedObject.FindProperty("Value");
//        MinValue = serializedObject.FindProperty("MinValue");
//        MaxValue = serializedObject.FindProperty("MaxValue");
//        Step = serializedObject.FindProperty("Step");

//        Hide("padding"); // hide Padding panel
//    }

//    /// <summary>
//    /// Rendering controls at the end of the main panel
//    /// </summary>
//    protected override void RenderMainOptions()
//    {
//        base.RenderMainOptions();

//        Direction.enumValueIndex = (int)(Direction)EditorGUILayout.EnumPopup(
//                                                                        "Direction",
//                                                                        (Direction)Enum.GetValues(typeof(Direction)).GetValue(Direction.enumValueIndex)
//                                                                    );

//        //NumberMode.enumValueIndex = (int)(SliderNumberMode)EditorGUILayout.EnumPopup(
//        //                                                                "NumberMode",
//        //                                                                (SliderNumberMode)Enum.GetValues(typeof(SliderNumberMode)).GetValue(NumberMode.enumValueIndex)
//        //                                                            );

//        Reverse.boolValue = EditorGUILayout.Toggle("Reverse", Reverse.boolValue);

//        Value.floatValue = EditorGUILayout.FloatField("Value", Value.floatValue);
//        MinValue.floatValue = EditorGUILayout.FloatField("Min value", MinValue.floatValue);
//        MaxValue.floatValue = EditorGUILayout.FloatField("Max value", MaxValue.floatValue);
//        Step.floatValue = EditorGUILayout.FloatField("Step", Step.floatValue);

//        EditorGUILayout.Space();
//    }

//    #region Example panel

//    //private const string ExampleSettingsVisibleKey = "eDrivenExampleSettingsVisible"; // unique key
//    //public static bool ExampleSettingsVisible // Editor setting example (saved by editor)
//    //{
//    //    get { return EditorPrefs.GetBool(ExampleSettingsVisibleKey); }
//    //    set
//    //    {
//    //        if (value == EditorPrefs.GetBool(ExampleSettingsVisibleKey))
//    //            return;

//    //        EditorPrefs.SetBool(ExampleSettingsVisibleKey, value);
//    //    }
//    //}

//    //private readonly PanelRenderer _examplePanelRenderer = new PanelRenderer();
//    //private readonly GUIContent _examplePanelHeader = new GUIContent("Example panel", (Texture)Resources.Load("eDriven/Editor/Icons/disk_multiple"));

//    ///// <summary>
//    ///// Rendering custom panels
//    ///// </summary>
//    //protected override void RenderExtendedOptions()
//    //{
//    //    base.RenderExtendedOptions();

//    //    /* panel start */
//    //    ExampleSettingsVisible = _examplePanelRenderer.RenderStart(_examplePanelHeader, ExampleSettingsVisible);

//    //    if (ExampleSettingsVisible)
//    //    {
//    //        GUILayout.Label("Example label");

//    //        if (GUILayout.Button("Example button"))
//    //        {
//    //            Debug.Log("Example button clicked");
//    //        }

//    //        /* panel end */
//    //        _examplePanelRenderer.RenderEnd();
//    //    }
            
//    //}

//    #endregion

//}