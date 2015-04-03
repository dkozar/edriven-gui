using eDriven.Core.Events;
using eDriven.Gui.Events;
using eDriven.Gui.Reflection;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// Text field control
    /// </summary>

    #region Event metadata

    /// <summary>
    /// Button component
    /// </summary>
    [DefaultEvent(TextFieldEvent.TEXT_CHANGE)]

    [Event(Name = TextFieldEvent.TEXT_CHANGING, Type = typeof(TextFieldEvent), Bubbles = true)]
    [Event(Name = TextFieldEvent.TEXT_CHANGE, Type = typeof(TextFieldEvent), Bubbles = true)]

    #endregion

    public class TextFieldBase : SimpleComponent
    {
        #region Events

        private MulticastDelegate _textChanging;
        /// <summary>
        /// The event that fires before the text is changed
        ///</summary>
        [Event(Name = TextFieldEvent.TEXT_CHANGING, Type = typeof(TextFieldEvent), Bubbles = true)]
        public MulticastDelegate TextChanging
        {
            get
            {
                if (null == _textChanging)
                    _textChanging = new MulticastDelegate(this, TextFieldEvent.TEXT_CHANGING);
                return _textChanging;
            }
            set
            {
                _textChanging = value;
            }
        }

        private MulticastDelegate _textChange;
        /// <summary>
        /// The event that fires after the text is changed
        ///</summary>
        [Event(Name = TextFieldEvent.TEXT_CHANGE, Type = typeof(TextFieldEvent), Bubbles = true)]
        public MulticastDelegate TextChange
        {
            get
            {
                if (null == _textChange)
                    _textChange = new MulticastDelegate(this, TextFieldEvent.TEXT_CHANGE);
                return _textChange;
            }
            set
            {
                _textChange = value;
            }
        }

        #endregion

        private bool _rendered;
        /// <summary>
        /// This is a flag indicated that a text field has been rendered at least once
        /// Focus management is relying on this property
        /// </summary>
        public bool Rendered
        {
            get { return _rendered; }
            set
            {
                if (_rendered || !value)
                    return;

                _rendered = true;
                DispatchEvent(new FrameworkEvent(FrameworkEvent.FIRST_SHOW));
            }
        }

        /*public override void SetFocus()
        {
            Defer(delegate
            {
                base.SetFocus();
            }, 1);
        }*/
    }
}