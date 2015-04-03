using System.Collections.Generic;
using eDriven.Gui;
using eDriven.Gui.Components;
using eDriven.Gui.Events;
using eDriven.Gui.Layout;
using eDriven.Gui.Plugins;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Extensions.ExampleControl
{
    /// <summary>
    /// Simple component example<br/>
    /// Extends Panel
    /// </summary>
    public class ExampleControl : Group
    {
        private TextField _textField;
        private Button _button;

        /// <summary>
        /// Overriding constructor for setup
        /// </summary>
        public ExampleControl()
        {
            Layout = new VerticalLayout { /*Direction = LayoutDirection.Vertical, */Gap = 10 };
            SetStyle("showBackground", true);
            MinWidth = 150;
            MinHeight = 100;
            //Padding = 10;
            Plugins.Add(new TabManager {CircularTabs = true});
            AddEventListener(ButtonEvent.PRESS, PressHandler);

            this.SetStyle("paddingLeft", 10);
            this.SetStyle("paddingRight", 10);
            this.SetStyle("paddingTop", 10);
            this.SetStyle("paddingBottom", 10);
        }

        /// <summary>
        /// Create children
        /// Use AddChild inside this method exclusively
        /// </summary>
        override protected void CreateChildren()
        {
            base.CreateChildren();

            _textField = new TextField {PercentWidth = 100, ProcessKeys = true, StyleName = "examplecontrol_textfield"};
            AddChild(_textField);

            _button = new Button { Text = "Click me", MinWidth = 150, StyleName = "examplecontrol_button" };
            AddChild(_button);
        }

        /// <summary>
        /// Setting focus
        /// </summary>
        public override void SetFocus()
        {
            _textField.SetFocus();
        }

        /// <summary>
        /// Run when pressing tab
        /// </summary>
        /// <returns></returns>
        public override List<DisplayListMember> GetTabChildren()
        {
            return new List<DisplayListMember>(new DisplayListMember[] { _textField, _button });
        }

        private bool _textChanged;
        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (value == _text)
                    return;

                _text = value;
                _textChanged = true;
                InvalidateProperties();
            }
        }

        private bool _buttonTextChanged;
        private string _buttonText;
        public string ButtonText
        {
            get
            {
                return _buttonText;
            }
            set
            {
                if (value == _buttonText)
                    return;

                _buttonText = value;
                _buttonTextChanged = true;
                InvalidateProperties();
            }
        }

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_textChanged)
            {
                _textChanged = false;
                _textField.Text = _text;
            }

            if (_buttonTextChanged)
            {
                _buttonTextChanged = false;
                _button.Text = _buttonText;
            }
        }

        private void PressHandler(Event e)
        {
            Alert.Show("Info", string.Format(@"The button was pressed. The input text is:

""{0}""", _textField.Text), AlertButtonFlag.Ok, delegate { SetFocus(); });
        }

        /// <summary>
        /// Cleanup
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            RemoveEventListener(ButtonEvent.PRESS, PressHandler);
        }
    }
}