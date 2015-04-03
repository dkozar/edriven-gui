#region License

/*
 
Copyright (c) 2010-2014 Danko Kozar

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 
*/

#endregion License

using System;
using System.Collections.Generic;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Events;
using eDriven.Gui.Layout;
using eDriven.Gui.Plugins;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;
using UnityEngine;
using Event = eDriven.Core.Events.Event;

namespace eDriven.Gui
{
    /// <summary>
    /// A graphical representation of the alert
    /// It is raised using static Show methods of an Alert class
    /// </summary>

    [Style(Name = "skinClass", Default = typeof(AlertSkin))]
    [Style(Name = "buttonSkin", Type = typeof(Type), Default = typeof(ButtonSkin))]
    [Style(Name = "buttonCursor", Type = typeof(string))]
    //[Style(Name = "buttonSkin", Type = typeof(string), Default = "eDriven.Playground.Skins.MyButtonSkin3")]
    
    public sealed class AlertInstance : Dialog
    {
#if DEBUG
    // ReSharper disable UnassignedField.Global
        public new static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        public string AlertOkLabel = "OK";
        public string AlertCancelLabel = "Cancel";
        public string AlertYesLabel = "Yes";
        public string AlertNoLabel = "No";

        public static float MaxLabelWidth = 800;
        public static float MaxLabelHeight = 600;

        #region Members

        private string _message;
        internal string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                if (null != MessageDisplay)
                    MessageDisplay.Text = _message;
            }
        }

        private Texture _icon;
        internal Texture Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                if (null != IconDisplay)
                    IconDisplay.Texture = _icon;
            }
        }

        private bool _flagsChanged;
        private AlertButtonFlag _flags;
        public AlertButtonFlag Flags
        {
            get
            {
                return _flags;
            }
            set
            {
                _flags = value;
                _flagsChanged = true;
                InvalidateProperties();
            }
        }

        #endregion

        #region Skin parts

        ///<summary>
        /// Icon display
        ///</summary>
        [SkinPart(Required = false)]
        public Image IconDisplay;

        ///<summary>
        /// Label display
        ///</summary>
        [SkinPart(Required = false)]
        public Label MessageDisplay;

        #endregion

        #region Constructor

        public AlertInstance()
        {
            DefaultButtonId = "ok";

            Draggable = true;
            Resizable = false;
            CloseOnEsc = true;

            FocusEnabled = true;

            MinWidth = 200;

            /* Doesn't work? */
            MaxWidth = 800;
            MaxHeight = 600;
            
            // add plugin that manages tab gestures
            Plugins.Add(new TabManager { CircularTabs = Alert.CircularTabs, CircularArrows = Alert.CircularArrows, ArrowsEnabled = Alert.ArrowsEnabled });

            /*Layout = new VerticalLayout
                                      {
                                          HorizontalAlign = HorizontalAlign.Center,
                                          VerticalAlign = VerticalAlign.Middle
                                      };*/

            if (null != Alert.DefaultSkin)
                SetStyle("skinClass", Alert.DefaultSkin);
        }

        #endregion

        #region Initialization

        public override void Initialize()
        {
            base.Initialize();

            ControlBarGroup.Layout = new HorizontalLayout {HorizontalAlign = HorizontalAlign.Center, VerticalAlign = VerticalAlign.Middle };

            ControlBarGroup.AddEventListener(ButtonEvent.PRESS, PressHandler);
        }

        protected override void PartAdded(string partName, object instance)
        {
            base.PartAdded(partName, instance);

            if (instance == MessageDisplay)
            {
                MessageDisplay.Text = _message;
            }
            if (instance == IconDisplay)
            {
                IconDisplay.Texture = _icon;
            }
        }

        /*protected override void CreateChildren()
        {
            base.CreateChildren();

            // hbox for icon and label
            _contentBox = new HGroup
                              {
                                  MinHeight = 60,
                                  VerticalAlign = VerticalAlign.Middle
                              };
            AddContentChild(_contentBox); // adding hbox as CONTENT child

            // label
            _lblText = new Label
                           {
                               Id = "message",
                               Text = string.Empty,
                               //Multiline = true,
                               //WordWrap = true,
                               MaxWidth = MaxLabelWidth,
                               MaxHeight = MaxLabelHeight,
                               Color = Color.black,
                           };
            _lblText.SetStyle("labelStyle", GetStyle("labelStyle"));
            _lblText.SetStyle("font", GetStyle("labelFont"));
            _contentBox.AddChild(_lblText);
        }*/

        #endregion

        #region Lifecycle methods

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_optionsChanged)
            {
                _optionsChanged = false;
                var len = _options.Length;
                for (var i = 0; i < len; i++)
                {
                    var option = _options[i];
                    switch (option.Type)
                    {
                        case AlertOptionType.Message:
                            Message = (string)option.Value;
                            break;

                        case AlertOptionType.Title:
                            Title = (string)option.Value;
                            break;

                        case AlertOptionType.Icon:
                            Icon = (Texture)option.Value;
                            break;

                        case AlertOptionType.HeaderIcon:
                            HeaderIconDisplay.Texture = (Texture)option.Value; // 20131206 IconDisplay
                            break;

                        case AlertOptionType.Flags:
                            ProcessFlags((AlertButtonFlag)option.Value);
                            break;

                        case AlertOptionType.Button:
                            AddButton((AlertButtonDescriptor)option.Value);
                            break;

                        case AlertOptionType.DefaultButton:
                            //Debug.Log("Applying: " + option.Value);
                            DefaultButtonId = option.Value;
                            break;

                        case AlertOptionType.SkinClass:
                            SetStyle("skinClass", option.Value);
                            break;

                        case AlertOptionType.AddedEffect:
                            SetStyle("addedEffect", option.Value);
                            break;

                        case AlertOptionType.RemovedEffect:
                            SetStyle("removedEffect", option.Value);
                            break;
                    }
                }
            }

            /*if (_iconChanged)
            {
                if (null != _icon)
                {
                    if (null == _imgIcon)
                    {
                        _imgIcon = new Image {Texture = _icon/*, AlphaBlend = false#1#};
                        _contentBox.AddChildAt(_imgIcon, 0);
                        //_contentBox.ValidateNow();
                    }
                }
                else if (null != _imgIcon)
                {
                    _contentBox.RemoveChild(_imgIcon);
                    //_contentBox.ValidateNow();
                }
                _iconChanged = false;
            }*/

            /*if (_messageChanged)
            {
                _messageChanged = false;
                MessageDisplay.Text = _message;
            }*/

            if (_flagsChanged)
            {
                _flagsChanged = false;
                ProcessFlags(_flags);
            }

            if (_descriptorsChanged)
            {
                _descriptorsChanged = false;

                string buttonStyleMapper = (string)GetStyle("buttonStyleMapper");

                _descriptors.ForEach(
                    delegate(AlertButtonDescriptor descriptor)
                    {
                        Button btn = new Button
                        {
                            Text = descriptor.Text
                        };

                        var skinClass = EvaluateSkinClassFromStyle("buttonSkin");
                        if (null != skinClass)
                            btn.SkinClass = skinClass;

                        if (null != descriptor.Icon)
                            btn.Icon = descriptor.Icon;
                            
                        if (!string.IsNullOrEmpty(buttonStyleMapper))
                            btn.StyleName = buttonStyleMapper;

                        if (!btn.HasStyle("buttonStyle"))
                            btn.SetStyle("buttonStyle", GetStyle("buttonStyle"));
                        if (!btn.HasStyle("buttonCursor"))
                            btn.SetStyle("cursor", GetStyle("buttonCursor"));
                         
                        //btn.SetStyle("cursor", "pointer");
                        ControlBarGroup.AddChild(btn);

                        if (descriptor.Focused)
                            //btn.SetFocus();
                            DefaultButtonId = descriptor.Id;

                        DictId2Button.Add(descriptor.Id, btn);
                        DictButton2Id.Add(btn, descriptor.Id);
                    }
                );
                //ControlBarGroup.ValidateNow();
            }
        }

        /*public override void StyleChanged(string styleName)
        {
            base.StyleChanged(styleName);

            if (styleName == "labelStyle")
            {
                _lblText.SetStyle("labelStyle", GetStyle("labelStyle"));
            }
            if (styleName == "labelFont")
            {
                _lblText.SetStyle("font", GetStyle("labelFont"));
            }
        }*/

        protected override void UpdateDisplayList(float width, float height)
        {
            base.UpdateDisplayList(width, height);

            /*_lblText.SetStyle("labelStyle", GetStyle("labelStyle"));
            _lblText.Color = (Color) GetStyle("labelColor");
            if (HasStyle("labelFont"))
                _lblText.SetStyle("font", GetStyle("labelFont"));

            //_contentBox.Gap = (string.IsNullOrEmpty(Message) || null == Icon) ? 0 : 10;
            _lblText.Visible = _lblText.IncludeInLayout = !string.IsNullOrEmpty(Message);
            _imgIcon.Visible = _imgIcon.IncludeInLayout = null != Icon;*/
            
            // buttons
            var buttonSkinClass = EvaluateSkinClassFromStyle("buttonSkin");
            if (null != buttonSkinClass)
            {
                foreach (var child in ControlBarGroup.Children)
                {
                    Button button = child as Button;
                    if (null != button)
                    {

                        button.SkinClass = buttonSkinClass;
                    }
                }
            }
        }

        #endregion

        #region Internal event handlers

        /// <summary>
        /// Handler on button press
        /// </summary>
        /// <param name="e"></param>
        private void PressHandler(Event e)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("Press");
            }
#endif
            if (!DictButton2Id.ContainsKey((Button)e.Target))
                return;

            //Hide(); // crutial - start Hide() here
            ExecCallback(DictButton2Id[(Button)e.Target]);
        }

        /// <summary>
        /// Handler on key up
        /// </summary>
        /// <param name="e"></param>
        //protected override void KeyUpHandler(Event e)
        //{
        //    base.KeyUpHandler(e);

        //    KeyboardEvent ke = (KeyboardEvent)e;

        //    if (ke.KeyCode == KeyCode.Escape)
        //    {
        //        //Hide();
        //        _callback = null;
        //    }
        //    //DispatchEvent(new Event(CLOSE, this));
        //}

        #endregion

        #region Private methods

        private bool _optionsChanged;
        private AlertOption[] _options;
        internal void ApplyOptions(AlertOption[] options)
        {
            //Debug.Log("ApplyOptions");

            if (options == null) return;

            _options = options;
            _optionsChanged = true;
            InvalidateProperties();
        }

        override public void SetFocus()
        {   
            Button b = null;

            //Debug.Log("DefaultButtonId: " + DefaultButtonId);

            if (null != DefaultButtonId)
            {
                if (DefaultButtonId is AlertButtonFlag)
                {
                    switch ((AlertButtonFlag)DefaultButtonId)
                    {
                        case AlertButtonFlag.Ok:
                            b = DictId2Button["ok"];
                            if (null != b)
                                b.SetFocus();
                            break;
                        case AlertButtonFlag.Yes:
                            b = DictId2Button["yes"];
                            if (null != b)
                                b.SetFocus();
                            break;
                        case AlertButtonFlag.Cancel:
                            b = DictId2Button["cancel"];
                            if (null != b)
                                b.SetFocus();
                            break;
                        case AlertButtonFlag.No:
                            b = DictId2Button["no"];
                            if (null != b)
                                b.SetFocus();
                            break;
                    }
                }
                else
                {
                    if (DictId2Button.ContainsKey((string)DefaultButtonId))
                        b = DictId2Button[(string) DefaultButtonId];
                }
            }

            // nothing in focus?
            // try with OK
            if (null == b && null != DefaultButtonId && DictId2Button.ContainsKey((string)DefaultButtonId))
            {
                if (DictId2Button.ContainsKey((string)DefaultButtonId))
                    b = DictId2Button[(string) DefaultButtonId];
            }

            // still nothing?
            // try with the last button defined
            if (null == b)
            {
                IEnumerator<Button> enumerator = DictId2Button.Values.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    b = enumerator.Current;
                }
            }
            
            //Debug.Log("Setting focus to: " + b);

            // set focus
            if (null != b)
                b.SetFocus();
        }

        private bool _descriptorsChanged;
        private readonly List<AlertButtonDescriptor> _descriptors = new List<AlertButtonDescriptor>();

        private void AddButton(AlertButtonDescriptor descriptor)
        {
            _descriptors.Add(descriptor);
            _descriptorsChanged = true;
            InvalidateProperties();
            InvalidateSize();
            InvalidateDisplayList();
        }

        internal void ProcessFlags(AlertButtonFlag flag)
        {
            ControlBarGroup.RemoveAllChildren();

            if ((flag & AlertButtonFlag.Ok) == AlertButtonFlag.Ok)
            {
                AddButton(new AlertButtonDescriptor("ok", AlertOkLabel, false));
            }

            if ((flag & AlertButtonFlag.Cancel) == AlertButtonFlag.Cancel)
            {
                AddButton(new AlertButtonDescriptor("cancel", AlertCancelLabel, false));
            }

            if ((flag & AlertButtonFlag.Yes) == AlertButtonFlag.Yes)
            {
                AddButton(new AlertButtonDescriptor("yes", AlertYesLabel, false));
            }

            if ((flag & AlertButtonFlag.No) == AlertButtonFlag.No)
            {
                AddButton(new AlertButtonDescriptor("no", AlertNoLabel, false));
            }
        }

        #endregion
    }
}