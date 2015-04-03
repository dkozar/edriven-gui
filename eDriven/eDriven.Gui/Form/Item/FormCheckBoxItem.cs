//using eDriven.Core.Events;
//using eDriven.Gui.Components;
//using eDriven.Gui.Events;
//using eDriven.Gui.Layout;
//using Event=eDriven.Core.Events.Event;
//using EventHandler=eDriven.Core.Events.EventHandler;

//namespace eDriven.Gui.Form
//{
//    public class FormCheckBoxItem : FormItem
//    {
//        // ReSharper disable UnusedMember.Global
//// ReSharper disable InconsistentNaming
//        public const string BUTTON = "onButton";
//// ReSharper restore InconsistentNaming
//        // ReSharper restore UnusedMember.Global

//        #region Event handlers

//        /// <summary>
//        /// The event that fires when the is clicked/keypressed
//        ///</summary>
//        private MulticastDelegate _buttonHandler;
//        public MulticastDelegate ButtonHandler
//        {
//            get
//            {
//                if (null == _buttonHandler)
//                    _buttonHandler = new MulticastDelegate(this, BUTTON);
//                return _buttonHandler;
//            }
//            set
//            {
//                _buttonHandler = value;
//            }
//        }

//        #endregion

//        private bool _controlStyleNameChanged = false;
//        private string _controlStyleName;
//        /// <summary>
//        /// Style name
//        /// For core components only! (which render Unity GUI elements (Button, Label etc.))
//        /// Custom made components don't use this property
//        /// </summary>
//        public string ButtonStyleName
//        {
//            get { return _controlStyleName; }
//            set
//            {
//                _controlStyleName = value;
//                _controlStyleNameChanged = true;
//                InvalidateProperties();
//            }
//        }

//        private CheckBox _chk;

//        private bool _selectedChanged;
//        private bool _selected;
//        public bool Selected
//        {
//            get
//            {
//                return _selected;
//            }
//            set
//            {
//                _selected = value;
//                _selectedChanged = true;
//                InvalidateProperties();
//            }
//        }

//        public FormCheckBoxItem()
//        {
//            //VerticalAlign = VerticalAlign.Middle;
//            //QLayout = new HorizontalLayout { VerticalAlign = VerticalAlign.Middle };
//        }

//        protected override void CreateChildren()
//        {
//            base.CreateChildren();

//            _chk = new CheckBox();
//            //_chk.PercentWidth = 100;
//            //_chk.Padding = 10;
//            _chk.SetStyle("paddingLeft", 10);
//            _chk.SetStyle("paddingRight", 10);
//            _chk.SetStyle("paddingTop", 10);
//            _chk.SetStyle("paddingBottom", 10);
//            //_chk.HighlightOnFocus = true;
//            _chk.Press += new EventHandler(OnCheckBoxClick);
//            AddContentChild(_chk);
//        }


//        private void OnCheckBoxClick(Event e)
//        {
//            Button b = ((Button) e.Target);
//            ButtonEvent be = new ButtonEvent(BUTTON)
//                                 {
//                                     ButtonId = b.ButtonId, 
//                                     ButtonText = b.Text, 
//                                     Selected = b.Selected
//                                 };
//            DispatchEvent(be);
//        }

//        //private void OnButtonKeyUp(Event e)
//        //{
//        //    //Form.Submit();
//        //    ButtonEvent be = new ButtonEvent(EVENT_BUTTON);
//        //    be.ButtonId = ((Button)e.Target).ButtonId;
//        //    be.ButtonText = ((Button)e.Target).Text;
//        //    DispatchEvent(be);
//        //}

//        //private void OnButtonClick(Event e) // TODO: Submit autside, default button in form!
//        //{
//        //    ButtonEvent be = new ButtonEvent(EVENT_BUTTON);
//        //    be.ButtonId = ((Button)e.Target).ButtonId;
//        //    be.ButtonText = ((Button)e.Target).Text;
//        //    DispatchEvent(be);
//        //}

//        protected override void CommitProperties()
//        {
//            base.CommitProperties();

//            if (_controlStyleNameChanged)
//            {
//                _controlStyleNameChanged = false;
//                //_chk.StyleName = _controlStyleName;
//            }

//            if (_selectedChanged)
//            {
//                _selectedChanged = false;
//                _chk.Selected = _selected;
//            }
//        }

//        //public class ButtonDescriptor
//        //{
//        //    public string Id;
//        //    public string Text;
//        //    public float? Width;
//        //    public float? PercentWidth;
//        //    public float? Height;
//        //    public float? PercentHeight;
//        //    public float? Paddings;
//        //    public string StyleName;

//        //    public ButtonDescriptor()
//        //    {
//        //    }

//        //    public ButtonDescriptor(string text)
//        //    {
//        //        Text = text;
//        //    }

//        //    public ButtonDescriptor(string id, string text)
//        //    {
//        //        Id = id;
//        //        Text = text;
//        //    }

//        //    public ButtonDescriptor(string id, string text, float? width, float? height, float? percentWidth, float? percentHeight)
//        //    {
//        //        Id = id;
//        //        Text = text;
//        //        Width = width;
//        //        Height = height;
//        //        PercentWidth = percentWidth;
//        //        PercentHeight = percentHeight;
//        //    }
//        //}
//    }
//}