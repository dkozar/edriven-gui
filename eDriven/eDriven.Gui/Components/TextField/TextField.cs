using System.Collections.Generic;
using System.Text;
using eDriven.Core.Geom;
using eDriven.Gui.Events;
using eDriven.Gui.GUIStyles;
using eDriven.Gui.Managers;
using eDriven.Gui.Styles;
using UnityEngine;

namespace eDriven.Gui.Components
{
    [Style(Name = "font", Type = typeof(Font))]
    [Style(Name = "textFieldStyle", Type = typeof(GUIStyle), ProxyType = typeof(TextFieldStyle))]
    [Style(Name = "upTextColor", Type = typeof(Color), Default = 0x222222)]
    [Style(Name = "overTextColor", Type = typeof(Color), Default = 0x222222)]
    [Style(Name = "downTextColor", Type = typeof(Color), Default = 0x333333)]
    [Style(Name = "cursorColor", Type = typeof(Color), Default = 0x222222)]
    [Style(Name = "selectionColor", Type = typeof(Color), Default = 0xaaaaaa)]
    [Style(Name = "contentOffsetX", Type = typeof(float), Default = 0f)]
    [Style(Name = "contentOffsetY", Type = typeof(float), Default = 0f)]

    public class TextField : TextFieldBase
    {
        /* NOTE: I found out that TextField is the most problematic/hacky Unity (native) control.
         * So things are not perfect. There was lots of trial and error.
         * Hopefully some of you guys iron it out. :) */

        #region Properties

        private bool _textChanged;
        private string _text = string.Empty;
        /// <summary>
        /// Button Label
        /// </summary>
        public virtual string Text
        {
            get { return _text; }
            set
            {
                if (value != _text)
                {
                    _text = value ?? string.Empty;
                    _textChanged = true;
                    InvalidateProperties();
                }
            }
        }

        /// <summary>
        /// Max number of characters
        /// </summary>
        public int MaxChars = -1;
        
        private bool _editable = true;
        /// <summary>
        /// Is text field editable or read-only?
        /// </summary>
        public bool Editable
        {
            get { return _editable; }
            set
            {
                _editable = value;
                HighlightOnFocus = value;
            }
        }

        private bool _expandMinHeightToContent;
        /// <summary>
        /// Do not allow content cropping on Y axis
        /// </summary>
        public virtual bool ExpandHeightToContent
        {
            get { return _expandMinHeightToContent; }
            set
            {
                if (value != _expandMinHeightToContent)
                {
                    _expandMinHeightToContent = value;
                    InvalidateSize();
                }
            }
        }

        /// <summary>
        /// The flag indicating that TextField is being drawn as box, rather than TextField/TextArea when:
        /// 1) Not in focus
        /// 2) Not mouse-overed
        /// 3) Not a password field
        /// </summary>
        // ReSharper disable ConvertToConstant.Global
        public bool Optimized = true; // NOTE: Something strange (20120616) - when true by default, couldn't set it to false from outside (!?!)
        // ReSharper restore ConvertToConstant.Global        
        private bool _passwordModeChanged;
        private bool _passwordMode;
        /// <summary>
        /// Mask characters?
        /// </summary>
        public bool PasswordMode
        {
            get { return _passwordMode; }
            set
            {
                _passwordMode = value;
                _passwordModeChanged = true;
                InvalidateProperties();
            }
        }

        /// <summary>
        /// Password masking character
        /// </summary>
        public char PasswordCharMask = '*';

        private string _alowedCharacters = string.Empty;
        private List<char> _alowedCharsList = new List<char>();
        /// <summary>
        /// Concatenated string characters that are alowed
        /// </summary>
        /// <example>AlowedCharacters = "0123456789-"</example>
        public string AlowedCharacters
        {
            get
            {
                return _alowedCharacters;
            }
            set
            {
                _alowedCharacters = value;
                _alowedCharsList = new List<char>(_alowedCharacters.ToCharArray());
            }
        }

        /// <summary>
        /// Restrict
        /// </summary>
        public string Restrict { get; set; }

        #endregion

        #region Members

        private Color _cursorColor; // = UnityEngine.Color.black;
        private Color _selectionColor; // = UnityEngine.Color.gray;
        private GUIStyle _previousStyle;
        private TextFieldEvent _tfe;
        private string _currentText;
        private bool _shouldOptimize;
        private Color _oldCursorColor;
        private Color _oldSelectionColor;
        
        #endregion

        ///<summary>
        ///</summary>
        public TextField()
        {
            MinWidth = 40; // default
            MinHeight = 10;
            FocusEnabled = true; // will be turned off from the outside
            HighlightOnFocus = true;
            ProcessKeys = true;
        }

        #region Styles

        public override void StylesInitialized()
        {
            base.StylesInitialized();
            InitStyles();
        }

        public override void StyleChanged(string styleProp)
        {
            base.StyleChanged(styleProp);
            if (null == styleProp || styleProp == "textFieldStyle" || styleProp == "font" || styleProp == "contentOffsetX" || styleProp == "contentOffsetY")
            {
                InitStyles();
            }
        }

        private void InitStyles()
        {
            //Debug.Log("TextField->InitStyles");
            GUIStyle style = (GUIStyle)GetStyle("textFieldStyle");

            if (_previousStyle != style)
            {
                //Debug.Log("Style change: _previousStyle: " + (null == _previousStyle ? "-" : _previousStyle.ToString()) + "; style: " + style);
                
                /**
                 * VERY IMPORTANT: Clonning the style
                 * */
                ActiveStyle = new GUIStyle(style) {name = style.name + "_cloned"};
                InvalidateSize();
                InvalidateDisplayList();
                _previousStyle = style;
            }

            var font = (Font)GetStyle("font");
            if (null != font && ActiveStyle.font != font)
            {
                //Debug.Log("Font change: old font: " + ActiveStyle.font + "; new font: " + font);
                ActiveStyle.font = font;
                InvalidateSize();
                InvalidateDisplayList();
            }

            var newOffset = new Vector2((float) GetStyle("contentOffsetX"), (float) GetStyle("contentOffsetY"));
            if (newOffset != ActiveStyle.contentOffset)
            {
                ActiveStyle.contentOffset = newOffset;
                InvalidateSize();
                InvalidateDisplayList();
            }
        }

        #endregion

        #region Invalidation

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_textChanged || _passwordModeChanged)
            {
                if (_textChanged)
                {
                    InitializeContent();
                    InvalidateSize();
                    InvalidateDisplayList();
                }

                _textChanged = false;
                _passwordModeChanged = false;
            }
        }

        /// <summary>
        /// This component uses it's custom measurement
        /// </summary>
        protected override void Measure()
        {
            if (null != Content) // content exists and height not defined in percentage
            {
                if (null == ActiveStyle.font)
                    throw new TextFieldException(string.Format("{0} [Style:{1}]", ComponentException.FontRequiredToCalculateSize, ActiveStyle.name));

                if (ExpandHeightToContent)
                {
                    MeasuredMinWidth = DEFAULT_MEASURED_MIN_WIDTH;

                    MeasuredWidth = MeasuredMinWidth = (null == ExplicitWidth) ? DEFAULT_MEASURED_WIDTH : (float)ExplicitWidth;

                    MeasuredMinHeight = DEFAULT_MEASURED_MIN_HEIGHT;

                    /**
                     * Calculate HEIGHT ONLY
                     * Do that by using the EMPTY STRING, since width is currently 0
                     * */
                    MeasuredHeight = ActiveStyle.CalcHeight(Content, MeasuredWidth) + (int)GetStyle("paddingTop") + (int)GetStyle("paddingBottom");
                }
                else
                {
                    /**
                     * Calculate HEIGHT ONLY
                     * Do that by using the EMPTY STRING, since width is currently 0
                     * */
                    float contentHeight = ActiveStyle.CalcHeight(new GUIContent("M"), Width) + (int)GetStyle("paddingTop") + (int)GetStyle("paddingBottom");
                    
                    MeasuredWidth = MeasuredMinWidth = DEFAULT_MEASURED_WIDTH; // 50
                    MeasuredHeight = MeasuredMinHeight = contentHeight;
                }

                //Debug.LogWarning("[SimpleTextField] " + this + " -> MeasuredWidth: " + MeasuredWidth + "; MeasuredHeight: " + MeasuredHeight + "; NestLevel: " + NestLevel);
            }
        }

        protected override void UpdateDisplayList(float width, float height)
        {
            base.UpdateDisplayList(width, height);

            _cursorColor = (Color)GetStyle("cursorColor");
            _selectionColor = (Color)GetStyle("selectionColor");

            //Debug.Log("ActiveStyle: " + ActiveStyle);
            ActiveStyle.normal.textColor = (Color)GetStyle("upTextColor");
            ActiveStyle.hover.textColor = (Color)GetStyle("overTextColor");
            ActiveStyle.active.textColor = (Color)GetStyle("downTextColor");
        }

        #endregion

        #region Rendering

        protected override void Render()
        {
            _oldCursorColor = GUI.skin.settings.cursorColor;
            _oldSelectionColor = GUI.skin.settings.selectionColor;
            
            GUI.skin.settings.cursorColor = _cursorColor;
            GUI.skin.settings.selectionColor = _selectionColor;

            var hasFocus = FocusManager.Instance.FocusedComponent == this; // I am not focusable, but the parent is

            /*if (hasFocus)
                Debug.Log("Focused: " + Uid);*/

            var isMouseOvered = Contains(MouseEventDispatcher.MouseTarget, true);

            _shouldOptimize = Optimized && !isMouseOvered && !hasFocus && !PasswordMode; /* 20150402 Uncommented "&& !PasswordMode" - BUGFIX for http://forum.edrivengui.com/index.php?threads/textfield-passwordmode-property-bug.295/ */

            //if (_shouldOptimize)
            //    Debug.LogWarning("_shouldOptimize is true!?! " + _shouldOptimize);

            /**
             * NOTE (20120616)
             * While investigating for the reason why text field/area looses selection while optimized
             * I found out that it's not the metter of this text field, but of the other optimized text field which is mouse-overed
             * Until this other field hasn't been mouse-overed, it is being drawn as a bow
             * However, when mouse-overed, it is being drawn as a text field and then it steals unity focus immediatelly!
             * This demands the further investigation
             * */
            
            // draw 2 instances as text fields, others as boxes
            // also, if there is a password mode on, draw TextBox
            if (_shouldOptimize)
            {
                if (UnityEngine.Event.current.type == EventType.Repaint)
                {
                    GUI.Box(RenderingRect, _text, ActiveStyle);
                }
            }
            else
            {
                var shouldRender = hasFocus || UnityEngine.Event.current.type == EventType.Repaint;

                if (shouldRender)
                {
                    GUI.SetNextControlName(Uid);

                    /**
                        * TextField
                        * */
                    if (_passwordMode)
                    {
                        _currentText = MaxChars > -1 ?
                            GUI.PasswordField(RenderingRect, _text, PasswordCharMask, MaxChars, ActiveStyle) :
                            GUI.PasswordField(RenderingRect, _text, PasswordCharMask, ActiveStyle);
                    }

                    else
                    {
                        _currentText = GUI.TextField(RenderingRect, _text, ActiveStyle);
                        if (MaxChars > 0)
                            _currentText = _currentText.Substring(0, MaxChars);
                    }
                }
            }

            //Debug.Log(string.Format("hasFocus: {0}, Editable: {1}, Enabled: {2}", hasFocus, Editable, Enabled));
            if ((hasFocus || isMouseOvered) && Editable && Enabled)
            {
                if (_currentText != _text)
                {
                    // if a list has a length greater than 0:
                    if (_alowedCharsList.Count > 0)
                    {
                        char[] oldArr = _currentText.ToCharArray();
                        StringBuilder sb = new StringBuilder();
                        foreach (char c in oldArr)
                        {
                            if (_alowedCharsList.Contains(c))
                                sb.Append(c);
                        }

                        _currentText = sb.ToString();
                    }

                    // one more time
                    if (_currentText != _text)
                    {
                        //Debug.Log("Dispatching change");
                        if (HasEventListener(TextFieldEvent.TEXT_CHANGING))
                        {
                            _tfe = new TextFieldEvent(TextFieldEvent.TEXT_CHANGING, true)
                                       {
                                           OldText = _text,
                                           NewText = _currentText
                                       }; // bubbles
                            DispatchEvent(_tfe);
                        }

                        if (null != _tfe && _tfe.Canceled) // _tfe is null if nobody listens
                            return;

                        /**
                         * Checking if event is canceled from the outside
                         * */
                        _text = _currentText;
                        _textChanged = true;
                        InvalidateProperties();

                        if (HasEventListener(TextFieldEvent.TEXT_CHANGE))
                        {
                            _tfe = new TextFieldEvent(TextFieldEvent.TEXT_CHANGE, true)
                                       {
                                           OldText = _text,
                                           NewText = _currentText
                                       }; // bubbles
                            DispatchEvent(_tfe);
                        }
                    }
                }
            }

            GUI.skin.settings.cursorColor = _oldCursorColor;
            GUI.skin.settings.selectionColor = _oldSelectionColor;

            Rendered = true;
        }

        #endregion
        
        protected override void InitializeContent()
        {
            base.InitializeContent();

            // text
            if (!string.IsNullOrEmpty(_text))
                Content.text = _text;
        }
    }
}