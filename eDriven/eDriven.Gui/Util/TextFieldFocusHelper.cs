using System;
using eDriven.Gui.Components;
using eDriven.Gui.Managers;
using UnityEngine;
using Component = eDriven.Gui.Components.Component;

namespace eDriven.Gui.Util
{
    /// <summary>
    /// The class taking care of native Unity focus
    /// What Unity does is it walks the cursor thgought all the text fields on screen while tabbing
    /// This isn't suitable for us since we want to be in control of focus order etc.
    /// Thus we have to cancel out this default behaviour
    /// Additionally, if no eDriven.Gui textfield in focus, we have to remove the cursor
    /// 
    /// This is the internal class, and the user has nothing to do with it
    /// It's being called by StageManager, after all the stages are rendered (if FocusManager.Instance.AutoCorrectUnityFocus set to true)
    /// It also uses a flag of the FocusManager class: FocusManager.Instance.TabbedToFocus
    /// </summary>
    internal class TextFieldFocusHelper
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode = false;
        // ReSharper restore UnassignedField.Global
#endif

// ReSharper disable InconsistentNaming
        internal const string FOCUS_OUT_UID = "_________Copyright 2010-2014 by Danko Kozar";
// ReSharper restore InconsistentNaming

        public static void RenderDummyTextField()
        {
            GUI.SetNextControlName(FOCUS_OUT_UID);
            GUI.TextField(new Rect(-100, -100, 1, 1), "");
        }

        /// <summary>
        /// Blurs Unity focus (removes the cursor)
        /// Should be called from inside the OnGUI() handler
        /// </summary>
        public static void BlurUnityFocus()
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("BlurUnityFocus");
            }
#endif
            GUI.FocusControl(FOCUS_OUT_UID);
        }

        private static InteractiveComponent _previouslyFocusedComponent;

        //private static string _focusedUid;
        private static TextFieldBase _textField;

        /// <summary>
        /// The flag indicating that the component has been focused vith tabbing
        /// </summary>
        //public static bool TabbedToFocus;

        private static bool _shouldHandleFocus = true;
        /// <summary>
        /// A flag indicating that the focus should be handled at the end of the cycle
        /// even if the focused (text field) component didn't change<br/>
        /// This is used to solve the text field bug of loosing the keyboard focus (cursor) when clicked outside of the field<br/>
        /// However, if parent dialog is clicked, and it's SetFocus() sets focus to the currently focused field, GUI.FocusControl should be called again 
        /// </summary>
        public static bool ShouldHandleFocus
        {
            set { _shouldHandleFocus = value; }
        }

        public static string NextFocusId { get; set; }

        /// <summary>
        /// The focus at the Unity level could change with tabbing etc.
        /// It is important to correct this behaviour, because the FocusManager should handle all the focus logic
        /// This method is being called from StageManager, in each frame, from OnGUI() handler
        /// </summary>
        public static void HandleFocus()
        {
            /**
             * If this is the focused component and focus on this component already handled
             * our work is done, so return
             * */
            if (string.IsNullOrEmpty(NextFocusId) && !_shouldHandleFocus && FocusManager.Instance.FocusedComponent == _previouslyFocusedComponent)
                return;

            /* Reset the "force" flag */
            _shouldHandleFocus = false;

            _previouslyFocusedComponent = FocusManager.Instance.FocusedComponent;
            
            if (!string.IsNullOrEmpty(NextFocusId))
            {
#if DEBUG
                if (DebugMode)
                {
                    Debug.Log("Handling focus DIRECTLY with NextFocusId = " + NextFocusId);
                }
#endif
                DoFocusTextField(NextFocusId);
                NextFocusId = null;
                return;
            }

            //Debug.Log("GUIUtility.keyboardControl: " + GUIUtility.keyboardControl);
            _textField = FocusManager.Instance.FocusedComponent as TextFieldBase;

            if (null != _textField && _textField.FocusEnabled && _textField.Enabled)
            {
#if DEBUG
                if (DebugMode)
                {
                    Debug.Log("Handling focus on " + FocusManager.Instance.FocusedComponent + "(" + FocusManager.Instance.FocusedComponent.Uid + ")");
                }
#endif
                // @see http://answers.unity3d.com/questions/17169/select-text-in-gui-textfield.html
                // 'You must first focus something else (doesn't matter what, so long as it exists, "" or null will not suffice). Bit of an ugly kludge, but seems to work.'

                // A. Blur
                //BlurUnityFocus(); // BUG BUG BUG ??? Not needed!

                // B. Focus
                if (_textField.Rendered)
                {
                    DoFocusTextField();
                }
                else
                {
                    _textField.AddEventListener(FrameworkEvent.FIRST_SHOW, delegate
                    {
                        DoFocusTextField();
                    });
                }
            }
            else {
                BlurUnityFocus();
            }

            FocusManager.Instance.TabbedToFocus = false;
        }

        private static void DoFocusTextField(string id = null)
        {

            var theId = id ?? _textField.Uid;

#if DEBUG
                    if (DebugMode)
                    {
                        Debug.Log("DoFocusTextField: " + theId);
                    }
#endif
                    GUI.FocusControl(theId);

            //GUIUtility.keyboardControl = GUIUtility.GetControlID()

            /**
            * Handle TextEditor
            * */
            try
            {
                // NOTE: The TextEditor feature has not been documented by Unity
                TextEditor textEditor = GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl) as TextEditor;
                if (null != textEditor)
                {
#if DEBUG
                    if (DebugMode)
                    {
                        Debug.Log("--> Handling text editor on: " + theId);
                    }
#endif
                    var ta = _textField as TextArea;
                    if (null != ta)
                    {
                        textEditor.SelectNone();
                        textEditor.pos = 0;
                        textEditor.selectPos = 0;
                        textEditor.SelectTextStart();
                    }
                    else
                    {
                        textEditor.SelectAll();
                        //textEditor.pos = 0;
                        //textEditor.selectPos = 0;
                        //textEditor.SelectTextStart();
                        //textEditor.MoveCursorToPosition(new Vector2(0, 0));
                    }
                }
            }
            // ReSharper disable EmptyGeneralCatchClause
            /* NOTE: Silent fail */
            catch (Exception)
            {
                Debug.LogWarning("Problems with retrieving TextEditor");
            }
            // ReSharper restore EmptyGeneralCatchClause
        }
    }
}