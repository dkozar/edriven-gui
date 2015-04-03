using System.Collections.Generic;
using System.Text;
using eDriven.Core.Events;
using eDriven.Core.Geom;
using eDriven.Gui.Events;
using eDriven.Gui.GUIStyles;
using eDriven.Gui.Managers;
using eDriven.Gui.Styles;
using UnityEngine;
using Event = eDriven.Core.Events.Event;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// Text area
    /// </summary>
    [Style(Name = "font", Type = typeof(Font))]
    [Style(Name = "textAreaStyle", Type = typeof(GUIStyle), ProxyType = typeof(TextAreaStyle))]
    [Style(Name = "upTextColor", Type = typeof(Color), Default = 0x222222)]
    [Style(Name = "overTextColor", Type = typeof(Color), Default = 0x222222)]
    [Style(Name = "downTextColor", Type = typeof(Color), Default = 0x333333)]
    [Style(Name = "cursorColor", Type = typeof(Color), Default = 0x222222)]
    [Style(Name = "selectionColor", Type = typeof(Color), Default = 0xaaaaaa)]
    [Style(Name = "contentOffset", Type = typeof(Vector2))]

    public class TextArea : TextFieldBase
    {
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

        public int MaxChars = -1;

        private bool _editable = true;
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
        /// The flag indicating that TextArea is being drawn as box, rather than TextArea/TextArea when:
        /// 1) Not in focus
        /// 2) Not mouse-overed
        /// 3) Not a password field
        /// </summary>
        public bool Optimized = true;
        
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

        public string Restrict { get; set; }

        #endregion

        #region Members

        private Color _cursorColor;
        private Color _selectionColor;
        private GUIStyle _previousStyle;
        private TextFieldEvent _tfe;
        private string _currentText;
        private bool _shouldOptimize;
        private Color _oldCursorColor;
        private Color _oldSelectionColor;
        private Point _newCursorPos;
        private int _pos;
        private int _selectPos;
        private bool _activeMode;

        #endregion

        ///<summary>
        ///</summary>
        public TextArea()
        {
            MinWidth = 100; // default
            MinHeight = 80;
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
            if (styleProp == "textAreaStyle" || styleProp == "font" || styleProp == "contentOffset")
            {
                InitStyles();
            }
        }

        private void InitStyles()
        {
            GUIStyle style = (GUIStyle)GetStyle("textAreaStyle");

            if (_previousStyle != style)
            {
                /**
                 * VERY IMPORTANT: Clonning the style
                 * */
                ActiveStyle = new GUIStyle(style) { name = style.name + "_cloned" };
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

            if (null != GetStyle("contentOffset"))
            {
                ActiveStyle.contentOffset = (Vector2)GetStyle("contentOffset");
                InvalidateSize();
                InvalidateDisplayList();
            }
        }

        #endregion

        #region Invalidation

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_textChanged)
            {
                _textChanged = false;
                /*if (_multilineChanged)
                {
                    // update Style
                    InitializeStyle();
                }*/

                if (_textChanged)
                {
                    InitializeContent();
                    InvalidateSize();
                    InvalidateDisplayList();
                }
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
                    //if (null == ExplicitWidth)
                    //{
                    //    throw new Exception("The width should be explicitely set when ExpandHeightToContent set");
                    //}

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

                    //if (!HasExplicitHeight && !HasPercentHeight)
                    //{
                    //    if (contentHeight > MeasuredHeight)
                    //        MeasuredHeight = contentHeight;
                    //}

                    //MinHeight = contentHeight;

                    MeasuredWidth = MeasuredMinWidth = DEFAULT_MEASURED_WIDTH; // 50
                    MeasuredHeight = MeasuredMinHeight = contentHeight;
                }

                //Debug.LogWarning("[SimpleTextField] " + this + " -> MeasuredWidth: " + MeasuredWidth + "; MeasuredHeight: " + MeasuredHeight + "; NestLevel: " + NestLevel);
            }
        }

        protected override void UpdateDisplayList(float width, float height)
        {
            base.UpdateDisplayList(width, height);

            ActiveStyle = (GUIStyle) GetStyle("textAreaStyle");

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

            //var owner = Owner ?? this;

            var hasFocus = FocusManager.Instance.FocusedComponent == this; // I am not focusable, but the parent is

            //var ownerCmp = owner as Component;
            var isMouseOvered = Contains(MouseEventDispatcher.MouseTarget, true);

            //Debug.Log("MouseEventDispatcher.MouseTarget: " + MouseEventDispatcher.MouseTarget);

            _shouldOptimize = Optimized && !isMouseOvered && !hasFocus;

            if (_shouldOptimize)
            {
                //if (Uid == "TextField2")
                //    Debug.Log("Drawing box");
                if (UnityEngine.Event.current.type == EventType.Repaint)
                {
                    GUI.Box(RenderingRect, _text, ActiveStyle);
                }
                _activeMode = false;
            }
            else
            {
                if (hasFocus)
                {
                    //TextFieldFocusHelper.BlurUnityFocus();
                    //Debug.Log(Uid);
                    GUI.SetNextControlName(Uid);
                }

                _currentText = MaxChars > -1 ?
                                                    GUI.TextArea(RenderingRect, _text, MaxChars, ActiveStyle) :
                                                                                                                GUI.TextArea(RenderingRect, _text, ActiveStyle);
                //ActiveStyle.DrawCursor(RenderingRect, new GUIContent(_currentText), 1, 40);

                TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                //Debug.Log("editor.scrollOffset: " + editor.scrollOffset);
                //Debug.Log("editor.graphicalCursorPos: " + editor.graphicalCursorPos);

                if (null != _newCursorPos)
                {
                    //Debug.Log("Handling text");

                    Vector2 v = _newCursorPos.ToVector2();
                    //Debug.Log("   LocalRenderingRect: " + LocalRenderingRect);
                    //Debug.Log("index: " + index);

                    //Debug.Log("   v: " + v);

                    //int index = ActiveStyle.GetCursorStringIndex(editor.position, new GUIContent(_text), v);

                    //editor.MoveCursorToPosition(v);
                    //editor.SelectNone();
                    //editor.selectPos = -1;
                    //editor.selectPos = index;
                    //editor.pos = index;

                    //editor.DrawCursor("*****************");
                    //editor.selectPos = 10;
                    if (UnityEngine.Event.current.type == EventType.repaint)
                    {
                        //ActiveStyle.DrawCursor(RenderingRect, new GUIContent(_text), 111, index);
                        //ActiveStyle.DrawWithTextSelection(RenderingRect, new GUIContent(_text), 111, 0, 5);
                    }

                    _newCursorPos = null;
                }

                if (!_activeMode)
                {
                    // just switched
                    //Debug.Log("Loading: " + _selectPos + ", " + _pos);
                    editor.pos = _pos;
                    editor.selectPos = _selectPos;
                    _activeMode = true;
                }
                else
                {
                    //Debug.Log("Saving: " + _selectPos + ", " + _pos);
                    _pos = editor.pos;
                    _selectPos = editor.selectPos;
                }

                if (hasFocus)
                {
                    //Debug.Log("Has focus: " + Uid);
                    GUI.FocusControl(Uid);
                }
            }

            //Debug.Log(string.Format("hasFocus: {0}, Editable: {1}, Enabled: {2}", hasFocus, Editable, Enabled));
            if ((hasFocus || isMouseOvered) && Editable && Enabled)
            {
                //Debug.Log("miki " + _text + " -> " + _currentText);
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
                        //if (null == _tfe || !_tfe.Canceled) {
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
        }

        #endregion

        // ReSharper disable RedundantOverridenMember
        protected override void KeyDownHandler(Event e)
        {
            //Debug.Log("TextArea: KeyDownHandler: " + e.Bubbles);
            //Debug.Log("TextArea: KeyDownHandler");
            base.KeyDownHandler(e);
            //Debug.Log("TextArea: OnKeyDown");
        }

        protected override void KeyUpHandler(Event e)
        {
            //Debug.Log("TextArea: KeyUpHandler");
            base.KeyUpHandler(e);

            KeyboardEvent ke = (KeyboardEvent)e;
            if (/*!Multiline && */ke.KeyCode == KeyCode.Return)
            {
                //if (HasBubblingEventListener(RETURN))
                //    DispatchEvent(new Event(RETURN, true));
            }
        }

        protected override void InitializeContent()
        {
            base.InitializeContent();

            // text
            if (!string.IsNullOrEmpty(_text))
                Content.text = _text;
        }

        ///<summary>
        ///</summary>
        ///<param name="pos"></param>
        public void SetCursorPosition(Point pos)
        {
            Debug.Log("SetCursorPosition");
            Defer(delegate
            {
                _newCursorPos = pos;
            }, 2);
        }
    }
}