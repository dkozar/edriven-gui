//using System.Collections.Generic;
//using eDriven.Core.Events;
//using eDriven.Gui.Components;
//using eDriven.Gui.Layout;
//using UnityEngine;
//using Event=eDriven.Core.Events.Event;
//using EventHandler=eDriven.Core.Events.EventHandler;

//namespace eDriven.Gui.Form
//{
//    public class FormButtonItem : FormItem
//    {
//        // ReSharper disable UnusedMember.Global
//// ReSharper disable InconsistentNaming
//        public const string PRESS = "onPress";
//// ReSharper restore InconsistentNaming
//        // ReSharper restore UnusedMember.Global

//        #region Event handlers

//        /// <summary>
//        /// The event that fires when the is clicked/keypressed
//        ///</summary>
//        private MulticastDelegate _pressHandler;
//        public MulticastDelegate PressHandler
//        {
//            get
//            {
//                if (null == _pressHandler)
//                    _pressHandler = new MulticastDelegate(this, PRESS);
//                return _pressHandler;
//            }
//            set
//            {
//                _pressHandler = value;
//            }
//        }

//        #endregion

//        //private bool _controlStyleNameChanged;
//        //public override string ControlStyleName
//        //{
//        //    get
//        //    {
//        //        return base.ControlStyleName;
//        //    }
//        //    set
//        //    {
//        //        base.ControlStyleName = value;
//        //        _controlStyleNameChanged = true;
//        //        InvalidateProperties();
//        //    }
//        //}

//        public FormButtonItem()
//        {
//            //VerticalAlign = VerticalAlign.Middle;
//            //QLayout = new HorizontalLayout { VerticalAlign = VerticalAlign.Middle };

//            DisplayLabel = false;

//            // invalidation
//            _buttonsChanged = true;

//            PressHandler += new EventHandler(OnButton);
//        }

//        private bool _buttonsChanged;
//        private List<FormButtonDescriptor> _buttons = new List<FormButtonDescriptor>();
//        public List<FormButtonDescriptor> Buttons
//        {
//            get
//            {
//                return _buttons;
//            } 
//            set
//            {
//                _buttons = value;
//                _buttonsChanged = true;
//                InvalidateProperties();
//            }
//        }

//        public void AddButton(FormButtonDescriptor bd)
//        {
//            _buttons.Add(bd);

//            _buttonsChanged = true;
//            InvalidateProperties();
//        }

//        protected virtual void OnButton(Event e)
//        {
//            //Debug.Log("FormButtonItem: button pressed");

//            //ButtonEvent be = new ButtonEvent(EVENT_BUTTON);
//            //be.ButtonId = ((Button)e.Target).ButtonId;
//            //be.ButtonText = ((Button)e.Target).Text;
//            //DispatchEvent(be);
//        }

//        protected override void CommitProperties()
//        {
//            base.CommitProperties();

//            if (_buttonsChanged)
//            {
//                _buttonsChanged = false;
//                _buttons.ForEach(delegate(FormButtonDescriptor bd)
//                                     {
//                                         /**
//                                         * Spacer
//                                         * */
//                                         if (bd.IsSpacer)
//                                         {
//                                             Spacer l = new Spacer();
//                                             if (null != bd.Width)
//                                                 l.Width = (float)bd.Width;
//                                             else
//                                                 l.PercentWidth = bd.PercentWidth ?? 100;

//                                             if (null != bd.Height)
//                                                 l.Height = (float)bd.Height;
//                                             else
//                                                 l.PercentHeight = bd.PercentHeight ?? 100;

//                                             l.FocusEnabled = false;
//                                             l.MouseEnabled = false;

//                                             //l.Text = "label";

//                                             AddContentChild(l);
//                                         }

//                                             /**
//                                         * Button
//                                         * */
//                                         else
//                                         {

//                                             Button b = new Button();
//                                             b.ButtonId = bd.Id; // NOTE: Not the component ID!!!
//                                             b.Text = bd.Text;

//                                             if (null != bd.Icon)
//                                                 b.Icon = bd.Icon;

//                                             if (null != bd.Tooltip)
//                                                 b.Tooltip = bd.Tooltip;

//                                             //if (null != bd.StyleDeclaration)
//                                             //    b.StyleDeclaration = bd.StyleDeclaration;
//                                             // TODO: this mess with form buttons
//                                             //b.StyleName = bd.StyleName ?? ControlStyleName ?? "button";

//                                             if (null != bd.Width)
//                                                 b.Width = (float)bd.Width;

//                                             if (null != bd.Height)
//                                                 b.Height = (float)bd.Height;

//                                             if (null != bd.PercentWidth)
//                                                 b.PercentWidth = bd.PercentWidth;

//                                             if (null != bd.PercentHeight)
//                                                 b.PercentHeight = bd.PercentHeight;

//                                             if (null != bd.Paddings)
//                                             {
//                                                 //b.Padding = (float)bd.Paddings;
//                                                 b.SetStyle("paddingLeft", bd.Paddings);
//                                                 b.SetStyle("paddingRight", bd.Paddings);
//                                                 b.SetStyle("paddingTop", bd.Paddings);
//                                                 b.SetStyle("paddingBottom", bd.Paddings);
//                                             }
//                                             else
//                                             {
//                                                 //b.Padding = 10;
//                                                 b.SetStyle("paddingLeft", 10);
//                                                 b.SetStyle("paddingRight", 10);
//                                                 b.SetStyle("paddingTop", 10);
//                                                 b.SetStyle("paddingBottom", 10);
//                                             }

//                                             if (null != bd.ClickHandler)
//                                                 b.Click += bd.ClickHandler;

//                                             if (null != bd.PressHandler)
//                                                 b.Press += bd.PressHandler;

//                                             AddContentChild(b);
//                                         }
//                                     });
//            }

//            //if (_controlStyleNameChanged)
//            //{
//            //    _controlStyleNameChanged = false;
//            //    //_btn.StyleName = _buttonStyleName;
//            //    Controls.ForEach(delegate(Component control)
//            //    {
//            //        if (control is Button)
//            //            control.StyleName = ControlStyleName;
//            //    });
//            //}

//            //if (_hideCalled)
//            //{
//            //    _hideCalled = false;

//            //    _buttons.ForEach(delegate(AlertButtonDescriptor bd)
//            //                        {
//            //                            /**
//            //                             * Spacer
//            //                             * */
//            //                            if (bd.IsSpacer)
//            //                            {
//            //                                Label l = new Label();
//            //                                if (null != bd.Width)
//            //                                    l.Width = (float)bd.Width;

//            //                                if (null != bd.Height)
//            //                                    l.Height = (float)bd.Height;

//            //                                l.PercentWidth = bd.PercentWidth ?? 100;

//            //                                l.PercentHeight = bd.PercentHeight ?? 100;

//            //                                //l.Text = "label";

//            //                                AddControl(l);
//            //                            }
//            //}
//        }

//        public override void SetFocus()
//        {
//            base.SetFocus();

//            if (Controls.Count > 0)
//                Controls[0].SetFocus();
//        }

//        //private bool _hideCalled;
//        //private readonly List<string> _buttonsToHide;
//        //public void Hide(string buttonId)
//        //{
//        //    _buttonsToHide.Add(buttonId);
//        //    //_hideCalled = true;
//        //    InvalidateProperties();
//        //}

//        public class FormButtonDescriptor
//        {
//            // ReSharper disable UnassignedField.Global
//            public string Id;
//            public string Text;
//            public Texture Icon;
//            public string Tooltip;
//            public float? Width;
//            public float? PercentWidth;
//            public float? Height;
//            public float? PercentHeight;
//            public float? Paddings;
//            public string StyleName;
//            public bool ToggleMode;
//            public bool IsSpacer;
//            public EventHandler ClickHandler;
//            public EventHandler PressHandler;
//            //public StyleDeclaration StyleDeclaration;
            
//            // ReSharper restore UnassignedField.Global

//            public FormButtonDescriptor()
//            {
//            }

//            public FormButtonDescriptor(string text)
//            {
//                Text = text;
//            }

//            public FormButtonDescriptor(string id, string text) : this(text)
//            {
//                Id = id;
//            }

//            public FormButtonDescriptor(string id, string text, Texture icon, string tooltip, float? width, float? height, float? percentWidth, float? percentHeight, bool toggleMode, bool isSpacer, EventHandler clickHandler, EventHandler pressHandler)
//            {
//                Id = id;
//                Text = text;
//                Icon = icon;
//                Tooltip = tooltip;
//                Width = width;
//                Height = height;
//                PercentWidth = percentWidth;
//                PercentHeight = percentHeight;
//                ToggleMode = toggleMode;
//                IsSpacer = isSpacer;
//                ClickHandler = clickHandler;
//                PressHandler = pressHandler;
//            }
//        }
//    }
//}