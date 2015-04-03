using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using eDriven.Core.Util;
using eDriven.Gui;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Events;
using eDriven.Gui.Form;
using eDriven.Gui.Layout;
using eDriven.Gui.Plugins;
using eDriven.Gui.Reflection;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;
using Event=eDriven.Core.Events.Event;
using Group=eDriven.Gui.Components.Group;

namespace eDriven.Extensions.ExampleControl2
{
    #region Event metadata

    // Note: the only use of event metadata is displaying the event in the designer
    [Event(Name = ExampleEvent.SEND_MESSAGE, Type = typeof(ExampleEvent))]

    #endregion
    
    /// <summary>
    /// The example component<br/>
    /// Extends Panel
    /// </summary>
    public class ExampleControl2 : Panel
    {
        #region Members

        private Form _form;

        private Button _btnCc;
        private Button _btnBcc;

        private TextField _txtTo;
        private TextField _txtCc;
        private TextField _txtBcc;
        private TextField _txtMessage;
        private Button _btnSend;

        /// <summary>
        /// A regex to check the e-mail address against
        /// </summary>
        private const string EmailRegex = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";

        private const string HelpMsg = @"

Recipients should be delimited with "";"" and formatted as ""mail@domain.com"".";

        #endregion

        #region Properties

        public SendMode SendMode = SendMode.SimulateSending;

        #endregion

        /// <summary>
        /// Overriding constructor for setup
        /// </summary>
        public ExampleControl2()
        {
            Title = "e-mail client";
            Layout = new VerticalLayout { /*Direction = LayoutDirection.Vertical, */Gap = 10 };
            MinWidth = 400;
            MinHeight = 300;
            //Padding = 10;
            Plugins.Add(new TabManager { CircularTabs = true });
            AddEventListener(ButtonEvent.PRESS, PressHandler);
            //AddEventListener(TextField.RETURN, ReturnHandler);
            this.SetStyle("paddingLeft", 10);
            this.SetStyle("paddingRight", 10);
            this.SetStyle("paddingTop", 10);
            this.SetStyle("paddingBottom", 10);
        }

        override protected void CreateChildren()
        {
            base.CreateChildren();

            HGroup hbox = new HGroup { PercentWidth = 100 };
            AddContentChild(hbox);

            _btnCc = new Button { Text = "Show Cc:", ToggleMode = true};
            hbox.AddChild(_btnCc);

            _btnBcc = new Button { Text = "Show Bcc:", ToggleMode = true };
            hbox.AddChild(_btnBcc);

            _form = new Form { PercentWidth = 100/*, Padding = 0*/ };
            _form.SetStyle("paddingLeft", 0);
            _form.SetStyle("paddingRight", 0);
            _form.SetStyle("paddingTop", 0);
            _form.SetStyle("paddingBottom", 0);
            AddContentChild(_form);

            _txtTo = new TextField {PercentWidth = 100, ProcessKeys = true };
            _form.AddField("to", "To: ", _txtTo);

            _txtCc = new TextField { PercentWidth = 100 };
            _form.AddField("cc", "Cc: ", _txtCc);

            _txtBcc = new TextField { PercentWidth = 100 };
            _form.AddField("bcc", "Bcc: ", _txtBcc);

            _txtMessage = new TextField {PercentWidth = 100, Height = 150};
            _form.AddField("message", "Message: ", _txtMessage);

            HGroup hbox2 = new HGroup { PercentWidth = 100, HorizontalAlign = HorizontalAlign.Right };
            AddContentChild(hbox2);

            _btnSend = new Button { Text = "Send", MinWidth = 150 };
            hbox2.AddChild(_btnSend);
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

        protected override void CreationComplete()
        {
            base.CreationComplete();

            // hiding Cc and Bcc by default
            var parent = (Group)_txtCc.Parent;
            parent.Visible = parent.IncludeInLayout = _btnCc.Selected;

            parent = (Group)_txtBcc.Parent;
            parent.Visible = parent.IncludeInLayout = _btnBcc.Selected;
        }

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_labelWidthChanged)
            {
                _labelWidthChanged = false;
                _form.LabelWidth = _labelWidth;
            }
        }

        public override List<DisplayListMember> GetTabChildren()
        {
            List<DisplayListMember> list = new List<DisplayListMember>();
            list.AddRange(new DisplayListMember[]{ _btnCc, _btnBcc, _txtTo });

            if (_btnCc.Selected)
                list.Add(_txtCc);
            if (_btnBcc.Selected)
                list.Add(_txtBcc);

            list.AddRange(new DisplayListMember[]{ _txtMessage, _btnSend });

            return list;
        }

        readonly Timer _timer = new Timer(3, 1);
        private LoadingMask _mask;

        private Component _controlToFocus;

        protected void ReturnHandler(Event e)
        {
            if (e.Target is TextField)
                SendMessage();
        }

        private void PressHandler(Event e)
        {
            if (e.Target == _btnCc)
            {
                var parent = (Group)_txtCc.Parent;
                parent.Visible = parent.IncludeInLayout = _btnCc.Selected;
            }
            else if (e.Target == _btnBcc)
            {
                var parent = (Group)_txtBcc.Parent;
                parent.Visible = parent.IncludeInLayout = _btnBcc.Selected;
            }
            else if (e.Target == _btnSend)
            {
                SendMessage();
                return;
            }
        }

        /// <summary>
        /// Does a sanity check and then sends the message
        /// </summary>
        private void SendMessage()
        {
            if (null != _mask) // already sending
                return;

            _controlToFocus = null;

            if (string.IsNullOrEmpty(_txtTo.Text))
            {
                Alert.Show("Error", @"No recipients. Please add some...", AlertButtonFlag.Ok, FocusBack);
                _controlToFocus = _txtTo;
                return;
            }
                
            if (!CheckRecipients(_txtTo.Text))
            {
                Alert.Show("Error", @"Error with ""To:"" recipients. " + HelpMsg, AlertButtonFlag.Ok, FocusBack);
                _controlToFocus = _txtTo;
                return;
            }

            if (_btnCc.Selected && !string.IsNullOrEmpty(_txtCc.Text) && !CheckRecipients(_txtCc.Text))
            {
                Alert.Show("Error", @"Error with ""Cc:"" recipients. " + HelpMsg, AlertButtonFlag.Ok, FocusBack);
                _controlToFocus = _txtCc;
                return;
            }

            if (_btnBcc.Selected && !string.IsNullOrEmpty(_txtBcc.Text) && !CheckRecipients(_txtBcc.Text))
            {
                Alert.Show("Error", @"Error with ""Bcc:"" recipients. " + HelpMsg, AlertButtonFlag.Ok, FocusBack);
                _controlToFocus = _txtBcc;
                return;
            }

            if (string.IsNullOrEmpty(_txtMessage.Text))
            {
                Alert.Show("Error", @"No message. Please add some text.", AlertButtonFlag.Ok, FocusBack);
                _controlToFocus = _txtMessage;
                return;
            }

            if (SendMode == SendMode.SimulateSending) // just simulate sending
            {
                _mask = new LoadingMask(this, "Sending message...");
                _timer.AddEventListener(Timer.COMPLETE, MessageSentHandler); // simulating sent result after 3 seconds
                _timer.Start();
            }
            else // notify the world
            {
                var e = new ExampleEvent(ExampleEvent.SEND_MESSAGE)
                {
                    Bubbles = true, // bubble event
                    To = _txtTo.Text,
                    Cc = _txtCc.Text,
                    Bcc = _txtBcc.Text,
                    Message = _txtMessage.Text
                };
                Debug.Log("Dispatching an event, catch it from outside: " + e);
                DispatchEvent(e);
            }
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
        /// Checks the validity of e-mail addresses
        /// </summary>
        /// <param name="recipients"></param>
        /// <returns></returns>
        private static bool CheckRecipients(string recipients)
        {
            string[] parts = recipients.Split(new[] { ";", "," }, StringSplitOptions.None);
            foreach (string part in parts)
            {
                if (0 == part.Length)
                    return true;

                Match match = Regex.Match(part.Trim(), EmailRegex);
                if (!match.Success)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Fires after the message is sent
        /// </summary>
        /// <param name="e"></param>
        private void MessageSentHandler(Event e)
        {
            _mask.Unmask();
            _mask = null;
            _timer.RemoveEventListener(Timer.COMPLETE, MessageSentHandler);
            Alert.Show("Info", "Message sent!", AlertButtonFlag.Ok,
                       delegate
                           {
                               SetFocus();
                           });
        }

        public override void SetFocus()
        {
            _txtTo.SetFocus();
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