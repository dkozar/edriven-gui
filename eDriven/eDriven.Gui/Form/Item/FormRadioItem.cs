//using System.Collections.Generic;
//using eDriven.Core.Events;
//using eDriven.Gui.Components;
//using eDriven.Gui.Events;
//using eDriven.Gui.Layout;
//using EventHandler = eDriven.Core.Events.EventHandler;

//namespace eDriven.Gui.Form
//{
//    public class FormRadioItem : FormItem
//    {
//        //private ButtonBar _bar;

//        private bool _buttonDirectionChanged;
//        private LayoutDirection _buttonDirection;
//        /// <summary>
//        /// Overrides layout from HBox
//        /// And wires it to ButtonBar
//        /// </summary>
//        public LayoutDirection ButtonDirection
//        {
//            get
//            {
//                return _buttonDirection;
//            }
//            set
//            {
//                _buttonDirection = value;
//                _buttonDirectionChanged = true;
//                InvalidateProperties();
//            }
//        }

//        private bool _dataChanged;
//        private List<object> _dataProvider;
//        public List<object> DataProvider
//        {
//            get
//            {
//                return _dataProvider;
//            }
//            set
//            {
//                _dataProvider = value;
//                _dataChanged = true;
//                InvalidateProperties();
//            }
//        }

//        public object SelectedItem
//        {
//            get
//            {
//                return 0; // return _bar.SelectedValue; // changed from _bar.SelectedItem 20120506
//            }
//        }

//        private bool _selectedIndexChanged;
//        private int _selectedIndex = -1;
//        public int SelectedIndex
//        {
//            get
//            {
//                return _selectedIndex; // _bar.SelectedIndex;
//            }
//            set
//            {
//                //_bar.SelectedIndex = value;
//                _selectedIndex = value;
//                _selectedIndexChanged = true;
//                InvalidateProperties();
//            }
//        }

//        private bool _selectedValueChanged;
//        private object _selectedValue;
//        public object SelectedValue
//        {
//            get
//            {
//                //if (null != SelectedItem)
//                //    return SelectedItem.Value;

//                if (null != _selectedValue)
//                    return _selectedValue;

//                return null;
//            }
//            set
//            {
//                //_bar.SelectedValue = value;
//                _selectedValue = value;
//                _selectedValueChanged = true;
//                InvalidateProperties();
//            }
//        }

//        public FormRadioItem()
//        {
//            //SelectedIndexChanged = new MulticastDelegate(this, IndexChangeEvent.CHANGE);

//            //VerticalAlign = VerticalAlign.Top;
//            //QLayout = new HorizontalLayout {VerticalAlign = VerticalAlign.Top};
//        }

//        protected override void CreateChildren()
//        {
//            base.CreateChildren();

//            //_bar = new ButtonBar();
//            ////_bar.StyleName = "YellowBg";
//            ////_bar.SelectedIndexChanged += new EventHandler(OnSelectedIndexChanged);
//            //AddContentChild(_bar);

//            //RegisterTabComponent(_bar);
//        }

//        private void OnSelectedIndexChanged(Event e)
//        {
//            e.Target = this; // Redispatch
//            DispatchEvent(e);
//        }

//        private MulticastDelegate _selectedIndexChangedHandler;
//        public MulticastDelegate SelectedIndexChangedHandler
//        {
//            get
//            {
//                if (null == _selectedIndexChangedHandler)
//                    _selectedIndexChangedHandler = new MulticastDelegate(this, IndexChangeEvent.CHANGE);
//                return _selectedIndexChangedHandler;
//            }
//            set
//            {
//                _selectedIndexChangedHandler = value;
//            }
//        }

//        protected override void CommitProperties()
//        {
//            base.CommitProperties();

//            if (_buttonDirectionChanged)
//            {
//                _buttonDirectionChanged = false;
//                //BoxLayout bl = ((BoxLayout)_bar.Layout);
//                //bl.Direction = _buttonDirection;
//                //_bar.Direction = _buttonDirection;
//                InvalidateSize();
//                InvalidateDisplayList();
//            }

//            if (_dataChanged)
//            {
//                _dataChanged = false;
//                //_bar.DataProvider = _dataProvider;
//            }

//            if (_selectedIndexChanged)// && _itemsCreated)
//            {
//                _selectedIndexChanged = false;
//                //_bar.SelectedIndex = _selectedIndex;
//            }

//            if (_selectedValueChanged)
//            {
//                _selectedValueChanged = false;
//                //Debug.Log("Selected value changed in validation to: " + SelectedValue);
//                //_bar.SelectedValue = _selectedValue;
//            }

//            if (_tooltipChanged)
//            {
//                _tooltipChanged = false;
//                //_bar.Tooltip = base.Tooltip;
//            }
//        }

//        private bool _tooltipChanged;
//        public override string Tooltip
//        {
//            get
//            {
//                return base.Tooltip;
//            }
//            set
//            {
//                base.Tooltip = value;
//                _tooltipChanged = true;
//            }
//        }
//    }
//}