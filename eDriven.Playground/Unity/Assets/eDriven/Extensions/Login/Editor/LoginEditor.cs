using System;
using System.Reflection;
using eDriven.Extensions.Login;
using eDriven.Gui.Editor;
using eDriven.Gui.Editor.Rendering;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LoginAdapter))]
[CanEditMultipleObjects]
public class LoginEditor : ComponentEditor
{
    public SerializedProperty LabelWidth;
    public SerializedProperty Username;
    public SerializedProperty Password;
    public SerializedProperty UsernameLabel;
    public SerializedProperty PasswordLabel;
    public SerializedProperty SubmitText;
    public SerializedProperty SendMode;

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
        Username = serializedObject.FindProperty("Username");
        Password = serializedObject.FindProperty("Password");
        UsernameLabel = serializedObject.FindProperty("UsernameLabel");
        PasswordLabel = serializedObject.FindProperty("PasswordLabel");
        SubmitText = serializedObject.FindProperty("SubmitText");
        SendMode = serializedObject.FindProperty("SendMode");

        Hide("padding"); // hide Padding panel
    }

    private const string LoginSettingsVisibleKey = "eDrivenExampleSettingsVisible"; // unique key
    public static bool LoginSettingsVisible // Editor setting example (saved by editor)
    {
        get { return EditorPrefs.GetBool(LoginSettingsVisibleKey); }
        set
        {
            if (value == EditorPrefs.GetBool(LoginSettingsVisibleKey))
                return;

            EditorPrefs.SetBool(LoginSettingsVisibleKey, value);
        }
    }

    private readonly PanelRenderer _panelRenderer = new PanelRenderer();
    private readonly GUIContent _panelHeader = new GUIContent("Login", (Texture)Resources.Load("eDriven/Editor/Controls/login"));

    /// <summary>
    /// Rendering controls at the end of the main panel
    /// </summary>
    protected override void RenderExtendedOptions()
    {
 	    base.RenderExtendedOptions();

        ///* panel start */
        LoginSettingsVisible = _panelRenderer.RenderStart(_panelHeader, LoginSettingsVisible);
        // NOTE: in eDriven.Gui 1.6 PanelRenderer.RenderStart is not public. Will be fixed in 1.7

        if (LoginSettingsVisible)
        {
            LabelWidth.intValue = EditorGUILayout.IntField("Label Width", LabelWidth.intValue);
            Username.stringValue = EditorGUILayout.TextField("Username", Username.stringValue);
            Password.stringValue = EditorGUILayout.TextField("Password", Password.stringValue);
            UsernameLabel.stringValue = EditorGUILayout.TextField("Username Label", UsernameLabel.stringValue);
            PasswordLabel.stringValue = EditorGUILayout.TextField("Password Label", PasswordLabel.stringValue);
            SubmitText.stringValue = EditorGUILayout.TextField("Submit text", SubmitText.stringValue);
            SendMode.enumValueIndex = (int)(SendMode)EditorGUILayout.EnumPopup(
                "SendMode",
                (SendMode)Enum.GetValues(typeof(SendMode)).GetValue(SendMode.enumValueIndex)
            );

            EditorGUILayout.Space();

            /* panel end */
            _panelRenderer.RenderEnd();
            // NOTE: in eDriven.Gui 1.6 PanelRenderer.RenderEnd is not public. Will be fixed in 1.7
        }
    }
}