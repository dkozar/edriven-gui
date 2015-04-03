using System;
using eDriven.Core.Events;
using eDriven.Gui.Events;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;
using eDriven.Gui.Util;
using UnityEngine;
using Event = eDriven.Core.Events.Event;
using MulticastDelegate=eDriven.Core.Events.MulticastDelegate;

namespace eDriven.Gui.Components
{
    #region Event metadata

    [Event(Name = ValueChangedEvent.CHANGE, Type = typeof(ValueChangedEvent), Bubbles = true)]

    #endregion

    [Style(Name = "skinClass", Default = typeof(NumericStepperSkin))]

    public class NumericStepper : Spinner 
    {
        public static string AlowedIntCharacters = "0123456789";
        public static string AlowedFloatCharacters = "0123456789-,.";

        private MulticastDelegate _change;
        public MulticastDelegate Change
        {
            get
            {
                if (null == _change)
                    _change = new MulticastDelegate(this, ValueChangedEvent.CHANGE);
                return _change;
            }
            set
            {
                _change = value;
            }
        }

        #region Skin parts

        [SkinPart(Required = false)]
        public TextField TextDisplay;

        #endregion

        private bool _modeChanged;
        private NumericStepperMode _mode = NumericStepperMode.Int;
        public NumericStepperMode Mode
        {
            get { 
                return _mode;
            }
            set
            {
                if (value == _mode)
                    return;
        
                _mode = value;
                _modeChanged = true;
                InvalidateProperties();
            }
        }

// ReSharper disable UnaccessedField.Local
        private bool _stepChanged;
// ReSharper restore UnaccessedField.Local
        private float _step;
        public float Step
        {
            get { return _step; }
            set
            {
                _step = value;
                _stepChanged = true;
                InvalidateProperties();
            }
        }

        public NumericStepper()
        {
            //Maximum = 10;
            //MinWidth = 60;
            FocusEnabled = false; // focus only the text display 
            HighlightOnFocus = true;
            //ProcessKeys = true;

            AddEventListener(MouseEvent.MOUSE_DOWN, MouseDownHandler);
            AddEventListener(KeyboardEvent.KEY_DOWN, KeyDownHandler);
        }

        private bool _maxChanged;
        public override float Maximum
        {
            get
            {
                return base.Maximum;
            }
            set
            {
                _maxChanged = true;
                base.Maximum = value;
            }
        }

        private bool _stepSizeChanged;
        public override float StepSize
        {
            get
            {
                return base.StepSize;
            }
            set
            {
                _stepSizeChanged = true;
                base.StepSize = value;
            }
        } 

        private bool _maxCharsChanged;
        private int _maxChars;
        public int MaxChars
        {
            get { 
                return _maxChars;
            }
            set
            {
                if (value == _maxChars)
                    return;

                _maxChars = value;
                _maxCharsChanged = true;
                InvalidateProperties();
            }
        }

        private bool _valueFormatFunctionChanged;
        private object _valueFormatFunction;
        public object ValueFormatFunction
        {
            get { 
                return _valueFormatFunction;
            }
            set
            {
                if (value == _valueFormatFunction)
                    return;

                _valueFormatFunction = value;
                _valueFormatFunctionChanged = true;
                InvalidateProperties();
            }
        }

        private bool _valueParseFunctionChanged;
        private object _valueParseFunction;
        public object ValueParseFunction
        {
            get { 
                return _valueParseFunction;
            }
            set
            {
                if (value == _valueParseFunction)
                    return;

                _valueParseFunction = value;
                _valueParseFunctionChanged = true;
                InvalidateProperties();
            }
        }

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_maxChanged || _stepSizeChanged || _valueFormatFunctionChanged)
            {
                //TextDisplay.widthInChars = calculateWidestValue();
                _maxChanged = false;
                _stepSizeChanged = false;

                if (_valueFormatFunctionChanged)
                {
                    ApplyDisplayFormatFunction();

                    _valueFormatFunctionChanged = false;
                }
            }

            if (_valueParseFunctionChanged)
            {
                CommitTextInput(false);
                _valueParseFunctionChanged = false;
            }

            if (_maxCharsChanged)
            {
                TextDisplay.MaxChars = _maxChars;
                _maxCharsChanged = false;
            }
        }

        #region Old

        //protected new void KeyDownHandler(Event e)
        //{
        //    KeyboardEvent ke = (KeyboardEvent)e;

        //    switch (ke.KeyCode)
        //    {
        //        case KeyCode.UpArrow:
        //            Value += _step;
        //            break;
        //        case KeyCode.DownArrow:
        //            Value -= _step;
        //            break;
        //    }
        //}

//protected override void CreateChildren()
        //{
        //    base.CreateChildren();

        //    TextDisplay.AlowedCharacters = Mode == NumericStepperMode.Int ? AlowedIntCharacters : AlowedFloatCharacters;
        //    TextDisplay.Change += new EventHandler(OnTextChange);
        //    TextDisplay.FocusOut += new EventHandler(OnTextBlur);
        //    AddChild(TextDisplay);

        //    //VBox vbox = new VBox {VerticalSpacing = 0};
        //    Container vbox = new Container
        //                         {
        //                             QLayout = new VerticalLayout {/*VerticalAlign = VerticalAlign.Middle*/}
        //                         };
        //    AddChild(vbox);

        //    vbox.SendToBack();

        //    vbox.AddChild(_btnPlus);

        //    vbox.AddChild(_btnMinus);
        //}

        //public override void StyleChanged(string styleName, object s)
        //{
        //    base.StyleChanged(styleName, s);

        //    switch (styleName)
        //    {
        //        case "textFieldStyle":
        //            TextField.SetStyle("textFieldStyle", s);
        //            break;
        //        case "plusButtonStyle":
        //            _btnPlus.SetStyle("buttonStyle", s);
        //            break;
        //        case "minusButtonStyle":
        //            _btnMinus.SetStyle("buttonStyle", s);
        //            break;
        //    }
        //}

//        private void OnTextChange(Event e)
//        {
//            TextFieldEvent tfe = (TextFieldEvent) e;
//            try {
//                Value = (float) (Mode == NumericStepperMode.Int ? Convert.ToInt32(tfe.NewText) : Convert.ToDouble(tfe.NewText));
//                _valueChangedByEditor = true;
//            }
//            catch (Exception ex)
//            {
//                Debug.Log("Error: " + ex.Message);
//            }
//        }

//        private void OnTextBlur(Event e)
//        {
//            //TextDisplay.Text = Convert.ToString(_value);
//        }

        //private bool _valueChangedByEditor;

        #endregion

        public override void SetFocus()
        {
            //TextFieldFocusHelper.NextFocusId = TextDisplay.Uid;
            
            //base.SetFocus();

            // TODO: do not set focus to text display
            // instead, just grab its underlying editor and focus it at lower level

            //Debug.Log("TextDisplay.Uid: " + TextDisplay.Uid);
            //TextFieldFocusHelper.NextFocusId = TextDisplay.Uid;
            //GUI.FocusControl(TextDisplay.Uid);
            
            TextDisplay.SetFocus();
        }

        protected override bool IsOurFocus(DisplayObject target)
        {
            return target == TextDisplay;
        }
        
        private void MouseDownHandler(Event e)
        {
            SetFocus();

            //Debug.Log("MouseDownHandler: " + e);
            
            
            //TextDisplay.SetFocus();
            
            
            //e.CancelAndStopPropagation();
            ////Debug.Log("MouseDownHandler");
            //if (e.Target == TextDisplay)
            //{
            //    TextDisplay.SetFocus();
            //}
            //else
            //{
            //    SetFocus();
            //    //if (e.Target == _btnMinus)
            //    //{
            //    //    //Debug.Log("_btnMinus");
            //    //    Value -= Step;
            //    //    _valueChangedByEditor = true;
            //    //}
            //    //else if (e.Target == _btnPlus)
            //    //{
            //    //    //Debug.Log("_btnPlus");
            //    //    Value += Step;
            //    //    _valueChangedByEditor = true;
            //    //}
            //}
        }

        //protected override void CommitProperties()
        //{
        //    base.CommitProperties();

        //    if (_minimumChanged || _maximumChanged)
        //    {
        //        _minimumChanged = false;
        //        _maximumChanged = false;
        //        Value = Value; // refresh value (clamp...)
        //    } 
            
        //    if (_valueChanged)
        //    {
        //        _valueChanged = false;
        //        HandleMode();
        //        TextDisplay.Text = Convert.ToString(_value);

        //        if (_valueChangedByEditor)
        //        {
        //            _valueChangedByEditor = false;

        //            ValueChangedEvent ce = new ValueChangedEvent(ValueChangedEvent.CHANGE)
        //                                       {
        //                                           OldValue = _oldValue,
        //                                           NewValue = _value,
        //                                           Bubbles = true
        //                                       };
        //            DispatchEvent(ce);
        //        }
        //    }

        //    if (_modeChanged/* || _stepChanged*/)
        //    {
        //        _modeChanged = false;
        //        //_stepChanged = false;
        //        HandleMode();
        //    }
        //}

        protected override void PartAdded(string partName, object instance)
        {
            base.PartAdded(partName, instance);

            if (instance == TextDisplay)
            {
                //Debug.Log("TextDisplay added");
                TextDisplay.AlowedCharacters = Mode == NumericStepperMode.Int ? AlowedIntCharacters : AlowedFloatCharacters;
                //TextDisplay.Change += new EventHandler(OnTextChange);
                //TextDisplay.Owner = this;
                TextDisplay.MaxChars = _maxChars;
                TextDisplay.HighlightOnFocus = false;
                //TextDisplay.Optimized = false;
                //TextDisplay.FocusOut += new EventHandler(OnTextBlur);

                TextDisplay.AddEventListener(KeyboardEvent.KEY_UP, TextDisplayKeyUpHandler);
                TextDisplay.AddEventListener(GuiEvent.FOCUS_OUT, TextDisplayFocusOutHandler);
                TextDisplay.AddEventListener(KeyboardEvent.KEY_DOWN, KeyDownHandler, EventPhase.CaptureAndTarget); //, EventPhase.Capture | EventPhase.Target);

                //TextDisplay.AddEventListener(FlexEvent.ENTER,
                //                           textDisplay_enterHandler);
                //TextDisplay.addEventListener(FocusEvent.FOCUS_OUT,
                //                           TextDisplayFocusOutHandler);
                //TextDisplay.focusEnabled = false;
                //TextDisplay.maxChars = _maxChars;
                //// Restrict to digits, minus sign, decimal point, and comma
                TextDisplay.Restrict = "0-9\\-\\.\\,";
                TextDisplay.Text = Value + "";
                //// Set the the textDisplay to be wide enough to display
                //// widest possible value. 
                //TextDisplay.widthInChars = calculateWidestValue();
            }
        }

        protected override void PartRemoved(string partName, object instance)
        {
            base.PartRemoved(partName, instance);

            if (instance == TextDisplay)
            {
                TextDisplay.RemoveEventListener(KeyboardEvent.KEY_UP, TextDisplayKeyUpHandler);
                TextDisplay.RemoveEventListener(GuiEvent.FOCUS_OUT, TextDisplayFocusOutHandler);
                TextDisplay.RemoveEventListener(KeyboardEvent.KEY_DOWN, KeyDownHandler, EventPhase.CaptureAndTarget); // EventPhase.Capture | EventPhase.Target);
            }
        }

        private void TextDisplayKeyUpHandler(Event e)
        {
            //Debug.Log("TextDisplayKeyUpHandler");
            KeyboardEvent ke = (KeyboardEvent) e;
            if (ke.KeyCode == KeyCode.Return)
            {
                //Debug.Log("Return");
                CommitTextInput(true);
            }
        }

        private void TextDisplayFocusOutHandler(Event e)
        {
            CommitTextInput(true);
        }

        protected override void SetValue(float value)
        {
            base.SetValue(value);

            ApplyDisplayFormatFunction();
        }

        public override void ChangeValueByStep(bool increase)
        {
            CommitTextInput(false);
            base.ChangeValueByStep(increase);
        }

        private void CommitTextInput(bool dispatchChange)
        {
            //Debug.Log("Commit.TextDisplay: " + TextDisplay);

            float? inputValue = null;
            float prevValue = Value;
            
            if (ValueParseFunction != null) {
                //inputValue = ValueParseFunction(TextDisplay.Text);
            }
            else  {
                try {
                    inputValue = (float?) Convert.ToDouble(TextDisplay.Text);
                }
                catch {} // Note: silent fail
            }

            if ((null != TextDisplay.Text && TextDisplay.Text.Length != Value.ToString().Length)
                || TextDisplay.Text == "" || (inputValue != null && inputValue != Value && 
                (Mathf.Abs((float) (inputValue - Value)) >= 0.000001 || null == inputValue)))
            {
                if (inputValue != null)
                {
                    SetValue(NearestValidValue((float) inputValue, SnapInterval));
                
                    // Dispatch valueCommit if the display needs to change.
                    if (Value == prevValue && inputValue != prevValue)
                        DispatchEvent(new FrameworkEvent(FrameworkEvent.VALUE_COMMIT));
                }
            }
            
            if (dispatchChange)
            {
                if (Value != prevValue)
                    DispatchEvent(new Event(Event.CHANGE));
            }
        }

        /**
         *  
         *  Helper method that applies the valueFormatFunction  
         */
        private void ApplyDisplayFormatFunction()
        {
            if (ValueFormatFunction != null)
            {
                //TextDisplay.Text = ValueFormatFunction(Value);   
            }
            else
            {
                if (null != TextDisplay)
                    TextDisplay.Text = Value.ToString();
            }
        }

        private void HandleMode()
        {
            //Debug.Log("HandleMode: " + Mode);
            TextDisplay.AlowedCharacters = Mode == NumericStepperMode.Int ? AlowedIntCharacters : AlowedFloatCharacters;

            if (_mode == NumericStepperMode.Int)
            {
                Value = Mathf.FloorToInt(Value);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            RemoveEventListener(MouseEvent.MOUSE_DOWN, MouseDownHandler);
            RemoveEventListener(KeyboardEvent.KEY_DOWN, KeyDownHandler);
        }
    }

    public enum NumericStepperMode
    {
        Int, Float
    }
}


