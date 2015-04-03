//using eDriven.Core.Events;
//using eDriven.Gui.Components;
//using eDriven.Gui.Events;
//using eDriven.Gui.Layout;

//namespace eDriven.Gui.Form
//{
//    public class FormTextFieldItem : FormItem
//    {
//        private TextArea _tf;

//        private bool _textChanged;
//        private string _text = string.Empty;
//        public string Text
//        {
//            get
//            {
//                return _text;
//            }
//            set
//            {
//                _text = value;
//                _textChanged = true;
//                InvalidateProperties();
//            }
//        }

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

//        private bool _passwordModeChanged;
//        private bool _passwordMode;
//        /// <summary>
//        /// Password mode
//        /// </summary>
//        public bool PasswordMode
//        {
//            get { return _passwordMode; }
//            set
//            {
//                _passwordMode = value;
//                _passwordModeChanged = true;
//                InvalidateProperties();
//            }
//        }

//        private bool _passwordCharMaskChanged;
//        private char _passwordCharMask;
//        /// <summary>
//        /// masking char for passwords
//        /// </summary>
//        public char PasswordCharMask
//        {
//            get { return _passwordCharMask; }
//            set
//            {
//                _passwordCharMask = value;
//                _passwordCharMaskChanged = true;
//                InvalidateProperties();
//            }
//        }

//        public FormTextFieldItem()
//        {
//            //VerticalAlign = VerticalAlign.Middle;
//            //ControlStyleName = "TextField";
//            //QLayout = new HorizontalLayout { VerticalAlign = VerticalAlign.Middle };
//        }

//        protected override void CreateChildren()
//        {
//            base.CreateChildren();

//            _tf = new TextArea();
//            _tf.PercentWidth = 100;
//            _tf.HighlightOnFocus = true;
//            _tf.Change += new EventHandler(OnTextChange);

//            AddContentChild(_tf);
//        }

//        override protected void KeyUpHandler(Event e)
//        {
//            base.KeyUpHandler(e);

//            //KeyboardEvent ke = (KeyboardEvent) e;

//            //if (ke.CurrentEvent.keyCode == KeyCode.Return)
//            //{
//            //    if (HasFocus){
//            //        ke.Cancel();
//            //        Debug.Log("Calling TabManagerX.Next() by " + this);
//            //        //TabManagerX.Next();
//            //    }
//            //}
//        }

//        private void OnTextChange(Event e)
//        {
//            TextFieldEvent tfe = (TextFieldEvent) e;
//            _text = tfe.NewText;
//        }

//        protected override void CommitProperties()
//        {
//            base.CommitProperties();

//            if (_textChanged)
//            {
//                _textChanged = false;
//                _tf.Text = _text;
//            }

//            //if (_controlStyleNameChanged)
//            //{
//            //    _controlStyleNameChanged = false;
//            //    _tf.StyleName = ControlStyleName;
//            //}

//            if (_passwordModeChanged)
//            {
//                _passwordModeChanged = false;
//                _tf.PasswordMode = _passwordMode;
//            }

//            if (_passwordCharMaskChanged)
//            {
//                _passwordCharMaskChanged = false;
//                _tf.PasswordCharMask = _passwordCharMask;
//            }
//        }

//        public override void SetFocus()
//        {
//            _tf.SetFocus();
//        }
//    }
//}