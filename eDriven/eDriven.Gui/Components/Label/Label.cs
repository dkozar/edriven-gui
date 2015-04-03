using eDriven.Gui.GUIStyles;
using eDriven.Gui.Styles;
using UnityEngine;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// Label
    /// </summary>
    [Style(Name = "labelStyle", Type = typeof(GUIStyle), ProxyType = (typeof(LabelStyle)))]
    [Style(Name = "color", Type = typeof(Color), Default = 0xffffff)]
    [Style(Name = "backgroundColor", Type = typeof(Color), Default = 0xffffff)]
    [Style(Name = "contentColor", Type = typeof(Color), Default = 0xffffff)]
    [Style(Name = "font", Type = typeof(Font))]

    public class Label : TextBase
    {
        ///<summary>
        /// Constructor
        ///</summary>
        public Label()
        {
            FocusEnabled = false;
            ResizeWithContent = true;
        }
        
        public override void StylesInitialized()
        {
            base.StylesInitialized();
            InitStyles();
        }

        public override void StyleChanged(string styleProp)
        {
            base.StyleChanged(styleProp);
            if (styleProp == "labelStyle" || styleProp == "font" || styleProp == "contentOffset")
            {
                InitStyles();
            }
        }

        private GUIStyle _previousStyle;

        private void InitStyles()
        {
            GUIStyle style = (GUIStyle)GetStyle("labelStyle");

            if (_previousStyle != style)
            {
                //Debug.Log("Style change: _previousStyle: " + (null == _previousStyle ? "-" : _previousStyle.ToString()) + "; style: " + style);

                /**
                 * VERY IMPORTANT: Clonning the style
                 * */
                ActiveStyle = new GUIStyle(style) { name = style.name + "_cloned" };
                InvalidateSize();
                InvalidateDisplayList();
                _previousStyle = style;
            }

            var font = GetStyle("font") as Font;
            if (null != font && ActiveStyle.font != font)
            {
                //Debug.Log("Font change: old font: " + ActiveStyle.font + "; new font: " + font);
                ActiveStyle.font = font;
                InvalidateSize();
                //Measure();
                InvalidateDisplayList();/*
                InvalidateParentSizeAndDisplayList();*/
                ValidateNow();
            }

            if (null != GetStyle("contentOffset"))
            {
                ActiveStyle.contentOffset = (Vector2)GetStyle("contentOffset");
                InvalidateSize();
                InvalidateDisplayList();
            }

            ActiveStyle.wordWrap = _wordWrap;
            ActiveStyle.richText = _richText;
        }

        private bool _wordWrapChanged;
        private bool _wordWrap;
        public bool WordWrap
        {
            get { 
                return _wordWrap;
            }
            set
            {
                if (value == _wordWrap)
                    return;

                _wordWrap = value;
                _wordWrapChanged = true;
                InvalidateProperties();
            }
        }

        private bool _richTextChanged;
        private bool _richText;
        public bool RichText
        {
            get { 
                return _richText;
            }
            set
            {
                if (value == _richText)
                    return;

                _richText = value;
                _richTextChanged = true;
                InvalidateProperties();
            }
        }

        private bool _alignmentChanged;
        private TextAnchor _alignment = TextAnchor.MiddleLeft;
        public TextAnchor Alignment
        {
            get { 
                return _alignment;
            }
            set
            {
                if (value == _alignment)
                    return;

                _alignment = value;
                _alignmentChanged = true;
                InvalidateProperties();
            }
        }

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_wordWrapChanged)
            {
                _wordWrapChanged = false;
                ActiveStyle.wordWrap = _wordWrap;
                InvalidateSize();
                InvalidateDisplayList();
            }

            if (_richTextChanged)
            {
                _richTextChanged = false;
                ActiveStyle.richText = _richText;
                InvalidateSize();
                InvalidateDisplayList();
            }

            if (_alignmentChanged)
            {
                _alignmentChanged = false;
                ActiveStyle.alignment = _alignment;
                InvalidateSize();
                InvalidateDisplayList();
            }
        }

        protected override void Measure()
        {
            Vector2 size = ActiveStyle.CalcSize(new GUIContent(Text));

            MeasuredWidth = MeasuredMinWidth = size.x;
            MeasuredHeight = MeasuredMinHeight = size.y;
        }

        protected override void UpdateDisplayList(float width, float height)
        {
            base.UpdateDisplayList(width, height);
            Color = (Color) GetStyle("color");
            BackgroundColor = (Color)GetStyle("backgroundColor");
            ContentColor = (Color)GetStyle("contentColor");
        }
    }
}