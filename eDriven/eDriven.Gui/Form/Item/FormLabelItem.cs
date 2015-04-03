//using eDriven.Gui.Components;
//using eDriven.Gui.Layout;

//namespace eDriven.Gui.Form
//{
//    public class FormLabelItem : FormItem
//    {
//        private Label _lbl;

//        private bool _textChanged;
//        private string _text;
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

//        public FormLabelItem()
//        {
//            //VerticalAlign = VerticalAlign.Middle;
//            //QLayout = new HorizontalLayout { VerticalAlign = VerticalAlign.Middle };

//            //FocusEnabled = false;
//        }

//        protected override void CreateChildren()
//        {
//            base.CreateChildren();

//            _lbl = new Label();
//            _lbl.PercentWidth = 100;
//            //_lbl.FocusEnabled = false;
//            AddContentChild(_lbl);
//        }

//        protected override void CommitProperties()
//        {
//            base.CommitProperties();

//            if (_textChanged)
//            {
//                _textChanged = false;
//                _lbl.Text = _text;
//            }

//            //if (_controlStyleNameChanged)
//            //{
//            //    _controlStyleNameChanged = false;
//            //    //_lbl.StyleName = ControlStyleName;
//            //}
//        }
//    }
//}