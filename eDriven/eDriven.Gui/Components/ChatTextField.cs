/*using eDriven.Core.Events;
using eDriven.Gui.Components;
using eDriven.Gui.Managers;
using UnityEngine;
using Event = eDriven.Core.Events.Event;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// TextField that encaptulates Chat functionality
    /// If CTRL or SHIFT are pressed, ENTER doesn't submit message, but instead does a newline
    /// On sumbit dispatches the submit event
    /// </summary>
    public class ChatTextField : TextArea
    {
// ReSharper disable InconsistentNaming
        public const string INPUT = "chatInput";
// ReSharper restore InconsistentNaming

        private bool _isTextSubmitted;

        private MulticastDelegate _submit;
        public MulticastDelegate Submit
        {
            get
            {
                if (null == _submit)
                    _submit = new MulticastDelegate(this, INPUT);
                return _submit;
            }
            set
            {
                _submit = value;
            }
        }

        public bool ClearOnSubmit = true;

        /// <summary>
        /// Does submit fire on the keyDown or keyUp?
        /// </summary>
        public SubmitModeType SubmitMode = SubmitModeType.OnKeyUp; //SubmitModeType.OnKeyDown; // TODO: Frajeru ovo vrati da bude ko na GMailu!!! (Danko)
        
        public ChatTextField()
        {
            Editable = true;
        }

        override protected void KeyDownHandler(Event e)
        {
            if (this != FocusManager.Instance.FocusedComponent)
                return;
            
            base.KeyDownHandler(e);

            KeyboardEvent ke = (KeyboardEvent)e;

            if (ke.CurrentEvent.keyCode == KeyCode.None)
                return;

            //Debug.Log("ChatTextField.OnKeyDown: " + ke.KeyCode);
            
            if (ke.CurrentEvent.keyCode == KeyCode.Return && !ke.Shift && !ke.Control && !string.IsNullOrEmpty(Text)) // TODO: If Enter is 2 times clicked, text is not empty - fix this bug!  /*ke.Character == '\n'#1# 
            {
                //if (ke.CurrentEvent.keyCode == KeyCode.Return && !string.IsNullOrEmpty(Text))
                if (SubmitMode == SubmitModeType.OnKeyDown)
                    DoSubmit();
            }
        }

        override protected void KeyUpHandler(Event e)
        {
            if (this != FocusManager.Instance.FocusedComponent)
                return;
            
            base.KeyUpHandler(e);

            KeyboardEvent ke = (KeyboardEvent)e;
            
            if (ke.CurrentEvent.keyCode == KeyCode.None)
                return;

            //Debug.Log("ChatTextField.OnKeyUp: " + ke.KeyCode);
            
            if (ke.CurrentEvent.keyCode == KeyCode.Return && !ke.Shift && !ke.Control && !string.IsNullOrEmpty(Text)) // TODO: If Enter is 2 times clicked, text is not empty - fix this bug!  /*ke.Character == '\n'#1# 
            {
                //if (ke.CurrentEvent.keyCode == KeyCode.Return && !string.IsNullOrEmpty(Text))
                if (SubmitMode == SubmitModeType.OnKeyUp)
                    DoSubmit();
            }

            /**
             * Delete old text
             * #1#
            if (_isTextSubmitted)
            {
                _isTextSubmitted = false;
                if (ClearOnSubmit)
                    Text = string.Empty;
            }
        }

        private void DoSubmit()
        {
            if (Text == string.Empty)
                return;

            string text = Text.Trim();

            //Debug.Log("Input submitted: " + text);

            /**
             * 1) Dispatch event to outside world
             * #1#
            StringEvent ev = new StringEvent(INPUT, true) {Text = text}; // bubbles
            //ev.Bubbles = true;

            // Trim whitespace

            DispatchEvent(ev);

            /**
             * 2) Delete old text
             * #1#
            if (ClearOnSubmit)
                Text = string.Empty;

            _isTextSubmitted = true;
        }

        public enum SubmitModeType {
            OnKeyDown, OnKeyUp
        }
    }
}*/