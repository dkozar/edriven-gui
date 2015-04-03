using eDriven.Core.Events;
using eDriven.Gui.Data;

namespace eDriven.Gui.Components
{
    public class ButtonBarButton : Button, IItemRenderer
    {
        public ButtonBarButton()
        {
            ToggleMode = true;
        }

        /**
         *  
         *  Storage for the allowDeselection property 
         */
        private bool _allowDeselection = true;
        public bool AllowDeselection
        {
            get { return _allowDeselection; }
            set { _allowDeselection = value; }
        }

        private bool _showsCaret;
        
        public bool ShowsCaret
        {
            get { 
                return _showsCaret;
            }
            set
            {
                if (value == _showsCaret)
                    return;

                _showsCaret = value;
                /*drawFocusAnyway = true;
                drawFocus(value);*/
            }
        }

        public bool Dragging
        {
            get { return false; } 
            set {}
        }

        public override object Data
        {
            get { return Text; }
            set
            {
                Text = value.ToString();
                DispatchEvent(new Event("dataChange"));
            }
        }

        private int _itemIndex;
        public int ItemIndex
        {
            get { 
                return _itemIndex;
            }
            set
            {
                _itemIndex = value;
            }
        }

        private string _text;
        public override string Text
        {
            get { return _text; }
            set
            {
                if (value == _text)
                    return;

                _text = value;

                if (null != LabelDisplay)
                    LabelDisplay.Text = _text;
            }
        }

        protected override void ButtonReleased()
        {
            if (Selected && !AllowDeselection)
                return;

            base.ButtonReleased();
        }
    }
}