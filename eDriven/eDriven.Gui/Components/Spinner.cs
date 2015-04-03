using eDriven.Core.Events;
using eDriven.Core.Managers;
using eDriven.Gui.Events;
using eDriven.Gui.Reflection;
using UnityEngine;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Components
{
    public class Spinner : Range
    {
        #region Skin parts

// ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable FieldCanBeMadeReadOnly.Global
        
        ///<summary>Decrement button
        ///</summary>
        [SkinPart(Required=false)]
        public Button DecrementButton;

        ///<summary>Increment button
        ///</summary>
        [SkinPart(Required = false)]
        public Button IncrementButton;

        // ReSharper restore FieldCanBeMadeReadOnly.Global
        // ReSharper restore MemberCanBePrivate.Global

        #endregion

        private bool _allowValueWrap;
        ///<summary>Determines the behavior of the control for a step when the current value is either the maximum or minimum value.<br/>
        ///If allowValueWrap is true, then the value property wraps from the maximum<br/>
        ///to the minimum value, or from the minimum to the maximum value.
        ///</summary>
// ReSharper disable UnusedMember.Global
        public bool AllowValueWrap
// ReSharper restore UnusedMember.Global
        {
            get { return _allowValueWrap; }
            set { _allowValueWrap = value; }
        }

        public Spinner()
        {
            ProcessKeys = true;
        }

        protected override void PartAdded(string partName, object instance)
        {
            base.PartAdded(partName, instance);

            if (instance == IncrementButton)
            {
                //Debug.Log("IncrementButton added");
                IncrementButton.FocusEnabled = false;
                IncrementButton.AddEventListener(ButtonEvent.BUTTON_DOWN, IncrementButtonButtonDownHandler);
                IncrementButton.AutoRepeat = true;
            }
            else if (instance == DecrementButton)
            {
                DecrementButton.FocusEnabled = false;
                DecrementButton.AddEventListener(ButtonEvent.BUTTON_DOWN, DecrementButtonButtonDownHandler);
                DecrementButton.AutoRepeat = true;
            }
        }

        protected override void PartRemoved(string partName, object instance)
        {
            base.PartRemoved(partName, instance);

            if (instance == IncrementButton)
            {
                IncrementButton.RemoveEventListener(ButtonEvent.BUTTON_DOWN,
                                               IncrementButtonButtonDownHandler);
            }
            else if (instance == DecrementButton)
            {
                DecrementButton.RemoveEventListener(ButtonEvent.BUTTON_DOWN,
                                               DecrementButtonButtonDownHandler);
            }
        }

        public override void ChangeValueByStep(bool increase)
        {
            if (_allowValueWrap)
            {
                if (increase && (Value == Maximum))
                    Value = Minimum;
                else if (!increase && (Value == Minimum))
                    Value = Maximum;
                else
                    base.ChangeValueByStep(increase);
            }
            else
                base.ChangeValueByStep(increase);
        }

// ReSharper disable MemberCanBePrivate.Global
        protected virtual void IncrementButtonButtonDownHandler(Event e)
// ReSharper restore MemberCanBePrivate.Global
        {
            //Debug.Log("IncrementButtonButtonDownHandler");
            float prevValue = Value;
            
            ChangeValueByStep(true);
            
            if (Value != prevValue)
                DispatchEvent(new Event("change"));
        }

// ReSharper disable MemberCanBePrivate.Global
        protected virtual void DecrementButtonButtonDownHandler(Event e)
// ReSharper restore MemberCanBePrivate.Global
        {
            //Debug.Log("DecrementButtonButtonDownHandler");
            float prevValue = Value;
            
            ChangeValueByStep(false);
            
            if (Value != prevValue)
                DispatchEvent(new Event("change"));
        }

        protected override void KeyDownHandler(Event e)
        {
            //Debug.Log("KeyDownHandler: " + e.Target);

            base.KeyDownHandler(e);

            if (e.DefaultPrevented)
                return;

            //Debug.Log("KeyDownHandler: " + e.Target);
                    
            float prevValue = Value;

            KeyboardEvent ke = (KeyboardEvent) e;
            switch (ke.KeyCode)
            {
                case KeyCode.DownArrow:
                //case Keyboard.LEFT:
                {
                    ChangeValueByStep(false);
                    e.PreventDefault();
                    break;
                }

                case KeyCode.UpArrow:
                //case Keyboard.RIGHT:
                {
                    ChangeValueByStep(true);
                    e.PreventDefault();
                    break;
                }

                case KeyCode.Home:
                {
                    Value = Minimum;
                    e.PreventDefault();
                    break;
                }

                case KeyCode.End:
                {
                    Value = Maximum;
                    e.PreventDefault();
                    break;
                }
                
                default:
                {
                    base.KeyDownHandler(e);
                    break;
                }
            }

            if (Value != prevValue)
                DispatchEvent(new Event("change"));
        }

        public override void FocusInHandler(Event e)
        {
            //Debug.Log("Focus in: " + this);
            base.FocusInHandler(e);

            SystemEventDispatcher.Instance.AddEventListener(
            MouseEvent.MOUSE_WHEEL, SystemMouseWheelHandler, EventPhase.Capture | EventPhase.Target);
        }

        public override void FocusOutHandler(Event e)
        {
            base.FocusOutHandler(e);
            SystemEventDispatcher.Instance.RemoveEventListener(
            MouseEvent.MOUSE_WHEEL, SystemMouseWheelHandler, EventPhase.Capture | EventPhase.Target);
        }

        private void SystemMouseWheelHandler(Event e)
        {
            if (!e.DefaultPrevented)
            {
                MouseEvent me = (MouseEvent) e;
                float newValue = NearestValidValue(Value + me.CurrentEvent.delta.y * StepSize, StepSize);
                SetValue(newValue);
                DispatchEvent(new Event(Event.CHANGE));
                e.PreventDefault();
            }
        }
    }
}
