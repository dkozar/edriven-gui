using System.Collections.Generic;
using eDriven.Gui;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Events;
using eDriven.Gui.Layout;
using eDriven.Gui.Plugins;
using eDriven.Gui.Reflection;
using UnityEngine;
using Button=eDriven.Gui.Components.Button;
using Component=eDriven.Gui.Components.Component;
using Event=eDriven.Core.Events.Event;
using Form=eDriven.Gui.Form.Form;
using Label=eDriven.Gui.Components.Label;
using Panel=eDriven.Gui.Containers.Panel;
using Timer=eDriven.Core.Util.Timer;

namespace eDriven.Extensions.Login
{
    #region Event metadata

    // Note: the only use of event metadata is displaying the event in the designer

    // The event hat will be selected in the AddEventHandlerDialog by double-clicking the component
    [DefaultEvent(LoginEvent.LOGIN)]

    // Event name and type (used for event handler code autogeneration)
    [Event(Name = LoginEvent.LOGIN, Type = typeof(LoginEvent))]

    #endregion

    /// <summary>
    /// The example component<br/>
    /// Extends Panel
    /// </summary>
    public class Login : Panel
    {
        #region Members

        private Form _form;

        private TextField _txtUsername;
        private TextField _txtPassword;
        private Button _btnSubmit;

        #endregion

        #region Properties

        public SendMode SendMode = SendMode.SimulateSending;

        #endregion

        /// <summary>
        /// Overriding constructor for setup
        /// </summary>
        public Login()
        {
            Title = "Login";
            Layout = new VerticalLayout { /*Direction = LayoutDirection.Vertical, */Gap = 10 };
            MinWidth = 400;
            MinHeight = 300;
            //Padding = 10;
            Plugins.Add(new TabManager { CircularTabs = true });
            AddEventListener(ButtonEvent.PRESS, PressHandler);
            //AddEventListener(TextField.RETURN, ReturnHandler);
            SetStyle("paddingLeft", 10);
            SetStyle("paddingRight", 10);
            SetStyle("paddingTop", 10);
            SetStyle("paddingBottom", 10);
        }

        override protected void CreateChildren()
        {
            base.CreateChildren();

            HGroup hbox = new HGroup { PercentWidth = 100 };
            AddContentChild(hbox);

            _form = new Form { PercentWidth = 100/*, Padding = 0*/ };
            _form.SetStyle("paddingLeft", 0);
            _form.SetStyle("paddingRight", 0);
            _form.SetStyle("paddingTop", 0);
            _form.SetStyle("paddingBottom", 0);
            AddContentChild(_form);

            _txtUsername = new TextField { PercentWidth = 100, ProcessKeys = true };
            _form.AddField("to", _usernameLabel, _txtUsername);

            _txtPassword = new TextField { PercentWidth = 100, PasswordMode = true };
            _form.AddField("cc", _passwordLabel, _txtPassword);

            HGroup hbox2 = new HGroup { PercentWidth = 100, HorizontalAlign = HorizontalAlign.Right };
            AddContentChild(hbox2);

            _btnSubmit = new Button { Text = "Send", MinWidth = 150 };
            hbox2.AddChild(_btnSubmit);
        }

        private bool _labelWidthChanged;
        private float _labelWidth = 150;
        public float LabelWidth
        {
            get { 
                return _labelWidth;
            }
            set
            {
                if (value == _labelWidth)
                    return;
        
                _labelWidth = value;
                _labelWidthChanged = true;
                InvalidateProperties();
            }
        }

        private bool _usernameLabelChanged;
        private string _usernameLabel;
        public string UsernameLabel
        {
            get { 
                return _usernameLabel;
            }
            set
            {
                if (value == _usernameLabel)
                    return;
        
                _usernameLabel = value;
                _usernameLabelChanged = true;
                InvalidateProperties();
            }
        }

        private bool _passwordLabelChanged;
        private string _passwordLabel;
        public string PasswordLabel
        {
            get { 
                return _passwordLabel;
            }
            set
            {
                if (value == _passwordLabel)
                    return;
        
                _passwordLabel = value;
                _passwordLabelChanged = true;
                InvalidateProperties();
            }
        }

        private bool _usernameChanged;
        private string _username;
        public string Username
        {
            get { 
                return _username;
            }
            set
            {
                if (value == _username)
                    return;
        
                _username = value;
                _usernameChanged = true;
                InvalidateProperties();
            }
        }
        
        private bool _passwordChanged;
        private string _password;
        public string Password
        {
            get { 
                return _password;
            }
            set
            {
                if (value == _password)
                    return;
        
                _password = value;
                _passwordChanged = true;
                InvalidateProperties();
            }
        }

        private bool _submitTextChanged;
        private string _submitText;
        public string SubmitText
        {
            get { 
                return _submitText;
            }
            set
            {
                if (value == _submitText)
                    return;
        
                _submitText = value;
                _submitTextChanged = true;
                InvalidateProperties();
            }
        }

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_labelWidthChanged)
            {
                _labelWidthChanged = false;
                _form.LabelWidth = _labelWidth;
            }
            if (_usernameChanged)
            {
                _usernameChanged = false;
                _txtUsername.Text = _username;
            }
            if (_passwordChanged)
            {
                _passwordChanged = false;
                _txtPassword.Text = _password;
            }
            if (_usernameLabelChanged)
            {
                _usernameLabelChanged = false;
                if (null != _txtUsername.Parent)
                    ((Label)_txtUsername.Parent.Children[0]).Text = _usernameLabel;
            }
            if (_passwordLabelChanged)
            {
                _passwordLabelChanged = false;
                if (null != _txtPassword.Parent)
                    ((Label)_txtPassword.Parent.Children[0]).Text = _passwordLabel;
            }
            if (_submitTextChanged)
            {
                _submitTextChanged = false;
                _btnSubmit.Text = _submitText;
            }
        }

        public override List<DisplayListMember> GetTabChildren()
        {
            List<DisplayListMember> list = new List<DisplayListMember>();
            list.AddRange(new DisplayListMember[]{ _txtUsername, _txtPassword, _btnSubmit });
            return list;
        }

        readonly Timer _timer = new Timer(3, 1);
        private LoadingMask _mask;

        private Component _controlToFocus;

        protected void ReturnHandler(Event e)
        {
            if (e.Target is TextField)
                DoLogin();
        }

        private void PressHandler(Event e)
        {
            if (e.Target == _btnSubmit)
            {
                DoLogin();
                return;
            }
        }

        private void DoLogin()
        {
            if (null != _mask) // already logging in
                return;

            _controlToFocus = null;

            if (string.IsNullOrEmpty(_txtUsername.Text))
            {
                Alert.Show("Error", @"Username must not be empty", AlertButtonFlag.Ok, FocusBack);
                _controlToFocus = _txtUsername;
                return;
            }
            if (string.IsNullOrEmpty(_txtPassword.Text))
            {
                Alert.Show("Error", @"Password must not be empty", AlertButtonFlag.Ok, FocusBack);
                _controlToFocus = _txtPassword;
                return;
            }

            _username = _txtUsername.Text;
            _password = _txtPassword.Text;

            if (SendMode == SendMode.SimulateSending) // just simulate sending
            {
                _mask = new LoadingMask(this, "Logging in...");
                _timer.AddEventListener(Timer.COMPLETE, LogInHandler); // simulating sent result after 3 seconds
                _timer.Start();
            }
            else // notify the world
            {
                DispatchLoginEvent();
            }
        }

        private void DispatchLoginEvent()
        {
            var e = new LoginEvent(LoginEvent.LOGIN)
                        {
                            Bubbles = true, // bubble event
                            Username = Username,
                            Password = Password
                        };
            Debug.Log("Dispatching an event, catch it from outside: " + e);
            DispatchEvent(e);
        }

        /// <summary>
        /// Fires when alert closed
        /// Sets focus to the problematic text field
        /// </summary>
        /// <param name="option"></param>
        public void FocusBack(string option)
        {
            if (null != _controlToFocus)
                _controlToFocus.SetFocus(); // focus back this form
        }

        /// <summary>
        /// Fires after the message is sent
        /// </summary>
        /// <param name="e"></param>
        private void LogInHandler(Event e)
        {
            _mask.Unmask();
            _mask = null;
            _timer.RemoveEventListener(Timer.COMPLETE, LogInHandler);
            DispatchLoginEvent();
            Alert.Show("Info", "You are logged in.", AlertButtonFlag.Ok,
            delegate
            {
                SetFocus();
            });
        }

        public override void SetFocus()
        {
            _txtUsername.SetFocus();
        }

        /// <summary>
        /// Cleanup
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            RemoveEventListener(ButtonEvent.PRESS, PressHandler);
            //RemoveEventListener(TextField.RETURN, ReturnHandler);
        }
    }

    ///<summary>
    /// Send mode
    ///</summary>
    public enum SendMode
    {
        ///<summary>
        /// Display the loading mask, just for fun
        ///</summary>
        SimulateSending = 0,

        ///<summary>
        /// Dispatch event containing component values, and do something useful from the outside
        ///</summary>
        DispatchEvent = 1
    }
}