using eDriven.Core.Geom;
using eDriven.Gui.GUIStyles;
using eDriven.Gui.Shapes;
using eDriven.Gui.Styles;
using UnityEngine;

namespace eDriven.Gui.Components
{
    [Style(Name = "borderStyle", Type = typeof(GUIStyle), ProxyType = typeof(InspectorOverlay2PxRectStyle))]
    [Style(Name = "borderColor", Type = typeof(Color), Default = 0x00aa33)]
    [Style(Name = "textColor", Type = typeof(Color), Default = 0xffffff)]
    [Style(Name = "labelStyle", Type = typeof(GUIStyle), ProxyType = typeof(InspectorOverlayLabelStyle))]
    [Style(Name = "showLabel", Type = typeof(bool), Default = true)]
    [Style(Name = "font", Type = typeof(Font))]
    
    public class InspectorOverlay : Component
    {
        private RectShape _frame;
        private Label _label;

        private bool _textChanged;
        private string _text;
        public string Text
        {
            set
            {
                if (value == _text)
                    return;
                {
                    _text = value;
                    _textChanged = true;
                    InvalidateProperties();
                }
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            MouseEnabled = false;
            MouseChildren = false;
        }

        protected override void CreateChildren()
        {
            base.CreateChildren();

            /**
             * Frame does not need to measure itself, and its size will always be explicitely set
             * */
            _frame = new RectShape {Width = 50, Height = 50, FocusEnabled = false};
            AddChild(_frame);

            /**
             * Label will measure itself, depending of text
             * */
            _label = new Label();
            AddChild(_label);
        }

        private Rectangle _overlayBounds;
        private Rectangle _stageBounds;
        
        /// <summary>
        /// Redraws the overlay<br/>
        /// We need to know the stage bounds because we want to inteligently place the label (not to be clipped)
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="stageBounds"></param>
        /// <param name="text"></param>
        public void Redraw(Rectangle bounds, Rectangle stageBounds, string text)
        {
            //Debug.Log("Setting overlay to: " + bounds + "; stageBounds: " + stageBounds);
            _overlayBounds = bounds;
            _stageBounds = stageBounds;
            Text = text;
            InvalidateSize();
            InvalidateDisplayList();
        }

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_textChanged)
            {
                _textChanged = false;
                if (null != _label)
                    _label.Text = _text;
            }
        }

        protected override void Measure()
        {
            base.Measure();

            var h = null != _overlayBounds ? _overlayBounds.Height : 0;
            if (null != _label)
                h += _label.GetExplicitOrMeasuredHeight();

            MeasuredWidth = MeasuredMinWidth = _frame.GetExplicitOrMeasuredWidth();
            MeasuredHeight = MeasuredMinHeight = h;
        }

        protected override void UpdateDisplayList(float width, float height)
        {
            //Debug.Log("UpdateDisplayList");

            base.UpdateDisplayList(width, height);

            if (null == _overlayBounds)
                return;

            Move(_overlayBounds.X, _overlayBounds.Y);

            // Note: SetActualSize is not a good way
            // this is because explicit sizes are set during the button configuration time
            // on the other hand, if we do not set the explicit sizing, the button want to get measured, but having no font defined so there are problems
            // we don't want it to measure itself anyway, so we are setting the explicit size here
            //_frame.SetActualSize(_overlayBounds.Width, _overlayBounds.Height); 
            _frame.Width = _overlayBounds.Width;
            _frame.Height = _overlayBounds.Height;
            _frame.SetStyle("backgroundStyle", GetStyle("borderStyle")); // ?? InspectorOverlay2PxRectStyle.Instance);
            _frame.SetStyle("backgroundColor", GetStyle("borderColor"));
            
            if (null != _label)
            {
                _label.SetStyle("labelStyle", GetStyle("labelStyle"));// InspectorOverlayLabelStyle.Instance);
                _label.Visible = (bool)GetStyle("showLabel");
                _label.ContentColor = (Color)GetStyle("textColor");
                _label.SetStyle("font", GetStyle("font"));
                _label.BackgroundColor = (Color)GetStyle("borderColor");
                // just a small calculation on where to place the label
                float y = _overlayBounds.Height; // draw label below rectangle
                if (null != _stageBounds && _overlayBounds.Bottom + _label.Height > _stageBounds.Bottom) // rectangle is outside stage bounds
                    y = 0; // draw label inside rectangle
                _label.Move(0, y);
                _label.SetActualSize(_label.GetExplicitOrMeasuredWidth(), _label.GetExplicitOrMeasuredHeight());
            }
        }
    }
}