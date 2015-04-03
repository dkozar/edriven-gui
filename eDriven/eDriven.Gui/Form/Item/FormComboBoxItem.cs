using System.Collections.Generic;
using eDriven.Core.Events;
using eDriven.Gui.Components;
using eDriven.Gui.Data;
using eDriven.Gui.Events;
using EventHandler=eDriven.Core.Events.EventHandler;

namespace eDriven.Gui.Form
{
    public class FormComboBoxItem : FormItem
    {
        private ComboBox _combo;

        private bool _dataChanged;
        private List<object> _data;
        public new List<object> Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
                _dataChanged = true;
                InvalidateProperties();
            }
        }

        //public ListItem SelectedItem
        //{
        //    get
        //    {
        //        return _combo.SelectedItem;
        //    }
        //}

        private bool _selectedIndexChanged;
        private int _selectedIndex = -1;
        public int SelectedIndex
        {
            get
            {
                //if (null != _combo)
                //    return _combo.SelectedIndex;

                return _selectedIndex; // _bar.SelectedIndex;
            }
            set
            {
                _selectedIndex = value;
                _selectedIndexChanged = true;
                InvalidateProperties();
            }
        }

        private bool _selectedValueChanged;
        private object _selectedValue;
        public object SelectedValue
        {
            get
            {
                //if (null != SelectedItem)
                //    return SelectedItem.Value;
                
                //return null;

                return _selectedValue;
            }
            set
            {
                _selectedValue = value;
                _selectedValueChanged = true;
                InvalidateProperties();
            }
        }

        private bool _expandedHeightChanged;
        private float _expandedHeight = 200;
        public float ExpandedHeight
        {
            get
            {
                return _expandedHeight;
            }
            set
            {
                if (value != _expandedHeight)
                {
                    _expandedHeight = value;
                    _expandedHeightChanged = true;
                    InvalidateProperties();
                }
            }
        }

        public FormComboBoxItem()
        {
            //VerticalAlign = VerticalAlign.Middle;
            //QLayout = new HorizontalLayout { VerticalAlign = VerticalAlign.Middle };
        }

        protected override void CreateChildren()
        {
            base.CreateChildren();

            _combo = new ComboBox();
            //_combo.HorizontalAlign = Core.Layout.HorizontalAlign.Left;
            _combo.PercentWidth = 100;
            _combo.Width = 300;
            //_combo.StyleName = "YellowBg";
            //_combo.SelectedIndexChanged += new EventHandler(OnSelectedIndexChanged);

            AddContentChild(_combo);

            //RegisterTabComponent(_combo);
        }

        /*private void OnSelectedIndexChanged(Event e)
        {
            IndexChangeEvent sice = (IndexChangeEvent) e;
            _selectedIndex = sice.NewIndex;
            _selectedValue = _combo.SelectedValue;
        }*/

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_dataChanged)
            {
                _dataChanged = false;
                _combo.DataProvider = new ArrayList(_data);
            }

            if (_selectedIndexChanged)// && _itemsCreated)
            {
                _selectedIndexChanged = false;
                _combo.SelectedIndex = _selectedIndex;
            }

            if (_selectedValueChanged)
            {
                _selectedValueChanged = false;
                _combo.SelectedItem = _selectedValue;
            }

            /*if (_expandedHeightChanged)
            {
                _expandedHeightChanged = false;
                _combo.MaxPopupHeight = _expandedHeight;
            }*/
        }
    }
}