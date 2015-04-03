using eDriven.Gui.Graphics.Base;
using UnityEngine;

namespace eDriven.Gui.Components
{
    public class ProgramaticStyle : IProgramaticStyle
    {
        private InvalidationManagerClient _client;
        public InvalidationManagerClient Client
        {
            get { return _client; }
            set { _client = value; }
        }

        //private bool _styleChanged;
        private GUIStyle _style;

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ProgramaticStyle()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ProgramaticStyle(InvalidationManagerClient client)
        {
            _client = client;
            _style = new GUIStyle();

            _normal = new GUIStyleState();
            _onNormal = new GUIStyleState();
            _hover = new GUIStyleState();
            _active = new GUIStyleState();
            _focused = new GUIStyleState();
            _onHover = new GUIStyleState();
            _onActive = new GUIStyleState();
            _onFocused = new GUIStyleState();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ProgramaticStyle(InvalidationManagerClient invalidationManagerClient, GUIStyle style)
            : this(invalidationManagerClient)
        {
            _style = style;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Advanced mode fixes the rollover 
        /// </summary>
        public bool AdvancedMode; // false by default // experimental

        /// <summary>
        /// Normaly, the invalidation occurs only when we are setting the top level properties of this class
        /// When set, the setter invalidates the style state
        /// When changing Unity properties (e.g. label.Style.NormalColor.textColor = Color.yellow;) we have no way of registering the inner settings
        /// However, we present this parameter which, when set to true, invalidates a style on each request (e.g. get)
        /// Use carefully!
        /// </summary>
        //public bool DeepChangeRegister;

        public GUIStyle Style
        {
            get { return _style; }
            set
            {
                if (_style != value)
                {
                    _style = value;
                    //_styleChanged = true;
                    Invalidate();
                }
            }
        }

        #endregion

        #region Style options

        #region NormalColor .. OnFocused

        private bool _normalChanged;
        private GUIStyleState _normal;
        /// <summary>
        /// NormalColor
        /// </summary>
        public GUIStyleState Normal
        {
            get
            {
                //if (DeepChangeRegister) {}
                //_normalChanged = true;
                //Invalidate();
                return _normal;
            }
            set
            {
                if (value == _normal)
                    return;
                _normal = value;
                _normalChanged = true;
                Invalidate();
            }
        }

        private bool _hoverChanged;
        private GUIStyleState _hover;
        /// <summary>
        /// Hover
        /// </summary>
        public GUIStyleState Hover
        {
            get { return _hover; }
            set
            {
                if (value == _hover)
                    return;
                _hover = value;
                _hoverChanged = true;
                Invalidate();
            }
        }

        private bool _activeChanged;
        private GUIStyleState _active;
        /// <summary>
        /// Active
        /// </summary>
        public GUIStyleState Active
        {
            get { return _active; }
            set
            {
                if (value == _active)
                    return;
                _active = value;
                _activeChanged = true;
                Invalidate();
            }
        }

        private bool _focusedChanged;
        private GUIStyleState _focused;
        /// <summary>
        /// Focused
        /// </summary>
        public GUIStyleState Focused
        {
            get { return _focused; }
            set
            {
                if (value == _focused)
                    return;
                _focused = value;
                _focusedChanged = true;
                Invalidate();
            }
        }

        private bool _onNormalChanged;
        private GUIStyleState _onNormal;
        /// <summary>
        /// OnNormal
        /// </summary>
        public GUIStyleState OnNormal
        {
            get { return _onNormal; }
            set
            {
                if (value == _onNormal)
                    return;
                _onNormal = value;
                _onNormalChanged = true;
                Invalidate();
            }
        }

        private bool _onHoverChanged;
        private GUIStyleState _onHover;
        /// <summary>
        /// OnHover
        /// </summary>
        public GUIStyleState OnHover
        {
            get { return _onHover; }
            set
            {
                if (value == _onHover)
                    return;
                _onHover = value;
                _onHoverChanged = true;
                Invalidate();
            }
        }

        private bool _onActiveChanged;
        private GUIStyleState _onActive;
        /// <summary>
        /// OnActive
        /// </summary>
        public GUIStyleState OnActive
        {
            get { return _onActive; }
            set
            {
                if (value == _onActive)
                    return;
                _onActive = value;
                _onActiveChanged = true;
                Invalidate();
            }
        }

        private bool _onFocusedChanged;
        private GUIStyleState _onFocused;
        /// <summary>
        /// OnFocused
        /// </summary>
        public GUIStyleState OnFocused
        {
            get { return _onFocused; }
            set
            {
                if (value == _onFocused)
                    return;
                _onFocused = value;
                _onFocusedChanged = true;
                Invalidate();
            }
        }

        #endregion

        #region NormalColor .. OnFocused Background

        private bool _normalBackgroundChanged;
        private Texture2D _normalBackground;
        /// <summary>
        /// NormalColor
        /// </summary>
        public Texture2D NormalBackground
        {
            get
            {
                return _normalBackground;
            }
            set
            {
                if (value == _normalBackground)
                    return;

                _normalBackground = value;
                _normalBackgroundChanged = true;
                Invalidate();
            }
        }

        private bool _hoverBackgroundChanged;
        private Texture2D _hoverBackground;
        /// <summary>
        /// NormalColor
        /// </summary>
        public Texture2D HoverBackground
        {
            get
            {
                return _hoverBackground;
            }
            set
            {
                if (value == _hoverBackground)
                    return;
                _hoverBackground = value;
                _hoverBackgroundChanged = true;
                Invalidate();
            }
        }

        private bool _activeBackgroundChanged;
        private Texture2D _activeBackground;
        /// <summary>
        /// NormalColor
        /// </summary>
        public Texture2D ActiveBackground
        {
            get
            {
                return _activeBackground;
            }
            set
            {
                if (value == _activeBackground)
                    return;
                _activeBackground = value;
                _activeBackgroundChanged = true;
                Invalidate();
            }
        }

        private bool _focusedBackgroundChanged;
        private Texture2D _focusedBackground;
        /// <summary>
        /// NormalColor
        /// </summary>
        public Texture2D FocusedBackground
        {
            get
            {
                return _focusedBackground;
            }
            set
            {
                if (value == _focusedBackground)
                    return;
                _focusedBackground = value;
                _focusedBackgroundChanged = true;
                Invalidate();
            }
        }

        private bool _onNormalBackgroundChanged;
        private Texture2D _onNormalBackground;
        /// <summary>
        /// NormalColor
        /// </summary>
        public Texture2D OnNormalBackground
        {
            get
            {
                return _onNormalBackground;
            }
            set
            {
                if (value == _onNormalBackground)
                    return;
                _onNormalBackground = value;
                _onNormalBackgroundChanged = true;
                Invalidate();
            }
        }

        private bool _onHoverBackgroundChanged;
        private Texture2D _onHoverBackground;
        /// <summary>
        /// NormalColor
        /// </summary>
        public Texture2D OnHoverBackground
        {
            get
            {
                return _onHoverBackground;
            }
            set
            {
                if (value == _onHoverBackground)
                    return;
                _onHoverBackground = value;
                _onHoverBackgroundChanged = true;
                Invalidate();
            }
        }

        private bool _onActiveBackgroundChanged;
        private Texture2D _onActiveBackground;
        /// <summary>
        /// NormalColor
        /// </summary>
        public Texture2D OnActiveBackground
        {
            get
            {
                return _onActiveBackground;
            }
            set
            {
                if (value == _onActiveBackground)
                    return;
                _onActiveBackground = value;
                _onActiveBackgroundChanged = true;
                Invalidate();
            }
        }

        private bool _onFocusedBackgroundChanged;
        private Texture2D _onFocusedBackground;
        /// <summary>
        /// NormalColor
        /// </summary>
        public Texture2D OnFocusedBackground
        {
            get
            {
                return _onFocusedBackground;
            }
            set
            {
                if (value == _onFocusedBackground)
                    return;
                _onFocusedBackground = value;
                _onFocusedBackgroundChanged = true;
                Invalidate();
            }
        }

        #endregion

        #region NormalColor .. OnFocused TextColor

        private bool _normalTextColorChanged;
        private Color _normalTextColor;
        /// <summary>
        /// NormalColor
        /// </summary>
        public Color NormalTextColor
        {
            get
            {
                return _normalTextColor;
            }
            set
            {
                if (value == _normalTextColor)
                    return;
                _normalTextColor = value;
                _normalTextColorChanged = true;
                Invalidate();
            }
        }

        private bool _hoverTextColorChanged;
        private Color _hoverTextColor;
        /// <summary>
        /// NormalColor
        /// </summary>
        public Color HoverTextColor
        {
            get
            {
                return _hoverTextColor;
            }
            set
            {
                if (value == _hoverTextColor)
                    return;
                _hoverTextColor = value;
                _hoverTextColorChanged = true;
                Invalidate();
            }
        }

        private bool _activeTextColorChanged;
        private Color _activeTextColor;
        /// <summary>
        /// NormalColor
        /// </summary>
        public Color ActiveTextColor
        {
            get
            {
                return _activeTextColor;
            }
            set
            {
                if (value == _activeTextColor)
                    return;
                _activeTextColor = value;
                _activeTextColorChanged = true;
                Invalidate();
            }
        }

        private bool _focusedTextColorChanged;
        private Color _focusedTextColor;
        /// <summary>
        /// NormalColor
        /// </summary>
        public Color FocusedTextColor
        {
            get
            {
                return _focusedTextColor;
            }
            set
            {
                if (value == _focusedTextColor)
                    return;
                _focusedTextColor = value;
                _focusedTextColorChanged = true;
                Invalidate();
            }
        }

        private bool _onNormalTextColorChanged;
        private Color _onNormalTextColor;
        /// <summary>
        /// NormalColor
        /// </summary>
        public Color OnNormalTextColor
        {
            get
            {
                return _onNormalTextColor;
            }
            set
            {
                if (value == _onNormalTextColor)
                    return;
                _onNormalTextColor = value;
                _onNormalTextColorChanged = true;
                Invalidate();
            }
        }

        private bool _onHoverTextColorChanged;
        private Color _onHoverTextColor = Color.white;
        /// <summary>
        /// NormalColor
        /// </summary>
        public Color OnHoverTextColor
        {
            get
            {
                return _onHoverTextColor;
            }
            set
            {
                if (value == _onHoverTextColor)
                    return;
                _onHoverTextColor = value;
                _onHoverTextColorChanged = true;
                Invalidate();
            }
        }

        private bool _onActiveTextColorChanged;
        private Color _onActiveTextColor;
        /// <summary>
        /// NormalColor
        /// </summary>
        public Color OnActiveTextColor
        {
            get
            {
                return _onActiveTextColor;
            }
            set
            {
                if (value == _onActiveTextColor)
                    return;
                _onActiveTextColor = value;
                _onActiveTextColorChanged = true;
                Invalidate();
            }
        }

        private bool _onFocusedTextColorChanged;
        private Color _onFocusedTextColor;
        /// <summary>
        /// NormalColor
        /// </summary>
        public Color OnFocusedTextColor
        {
            get
            {
                return _onFocusedTextColor;
            }
            set
            {
                if (value == _onFocusedTextColor)
                    return;
                _onFocusedTextColor = value;
                _onFocusedTextColorChanged = true;
                Invalidate();
            }
        }

        #endregion

        #region Other

        private bool _borderChanged;
        private RectOffset _border;
        /// <summary>
        /// Border
        /// </summary>
        public RectOffset Border
        {
            get { return _border; }
            set
            {
                _border = value;
                _borderChanged = true;
                Invalidate();
            }
        }

        private bool _paddingChanged;
        private RectOffset _padding;
        /// <summary>
        /// Padding
        /// </summary>
        public RectOffset Padding
        {
            get { return _padding; }
            set
            {
                _padding = value;
                _paddingChanged = true;
                Invalidate();
            }
        }

        private bool _marginChanged;
        private RectOffset _margin;
        /// <summary>
        /// Margin
        /// </summary>
        public RectOffset Margin
        {
            get { return _margin; }
            set
            {
                _margin = value;
                _marginChanged = true;
                Invalidate();
            }
        }

        private bool _overflowChanged;
        private RectOffset _overflow;
        /// <summary>
        /// Overflow
        /// </summary>
        public RectOffset Overflow
        {
            get { return _overflow; }
            set
            {
                _overflow = value;
                _overflowChanged = true;
                Invalidate();
            }
        }

        private bool _fontChanged;
        private Font _font;
        /// <summary>
        /// Font
        /// </summary>
        public Font Font
        {
            get { return _font; }
            set
            {
                _font = value;
                _fontChanged = true;
                Invalidate();
            }
        }

        private bool _imagePositionChanged;
        private ImagePosition _imagePosition;
        /// <summary>
        /// Image position
        /// </summary>
        public ImagePosition ImagePosition
        {
            get { return _imagePosition; }
            set
            {
                _imagePosition = value;
                _imagePositionChanged = true;
                Invalidate();
            }
        }

        private bool _alignmentChanged;
        private TextAnchor _alignment;
        /// <summary>
        /// Alignment
        /// </summary>
        public TextAnchor Alignment
        {
            get { return _alignment; }
            set
            {
                _alignment = value;
                _alignmentChanged = true;
                Invalidate();
            }
        }

        private bool _wordWrapChanged;
        private bool _wordWrap;
        /// <summary>
        /// Word wrap
        /// </summary>
        public bool WordWrap
        {
            get { return _wordWrap; }
            set
            {
                _wordWrap = value;
                _wordWrapChanged = true;
                Invalidate();
            }
        }

        private bool _textClippingChanged;
        private TextClipping _textClipping;
        /// <summary>
        /// Text clipping
        /// </summary>
        public TextClipping TextClipping
        {
            get { return _textClipping; }
            set
            {
                _textClipping = value;
                _textClippingChanged = true;
                Invalidate();
            }
        }

        private bool _contentOffsetChanged;
        private Vector2 _contentOffset;
        /// <summary>
        /// Content offset
        /// </summary>
        public Vector2 ContentOffset
        {
            get { return _contentOffset; }
            set
            {
                _contentOffset = value;
                _contentOffsetChanged = true;
                Invalidate();
            }
        }

        private bool _fixedWidthChanged;
        private float _fixedWidth;
        /// <summary>
        /// Fixed height
        /// </summary>
        public float FixedWidth
        {
            get { return _fixedWidth; }
            set
            {
                _fixedWidth = value;
                _fixedWidthChanged = true;
                Invalidate();
            }
        }

        private bool _fixedHeightChanged;
        private float _fixedHeight;
        /// <summary>
        /// Fixed width
        /// </summary>
        public float FixedHeight
        {
            get { return _fixedHeight; }
            set
            {
                _fixedHeight = value;
                _fixedHeightChanged = true;
                Invalidate();
            }
        }

        private bool _fontSizeChanged;
        private int _fontSize;
        /// <summary>
        /// Font size
        /// </summary>
        public int FontSize
        {
            get { return _fontSize; }
            set
            {
                _fontSize = value;
                _fontSizeChanged = true;
                Invalidate();
            }
        }

        private bool _fontStyleChanged;
        private FontStyle _fontStyle;
        /// <summary>
        /// Font style
        /// </summary>
        public FontStyle FontStyle
        {
            get { return _fontStyle; }
            set
            {
                _fontStyle = value;
                _fontStyleChanged = true;
                Invalidate();
            }
        }

        private bool _stretchWidthChanged;
        private bool _stretchWidth;
        /// <summary>
        /// Stretch width
        /// </summary>
        public bool StretchWidth
        {
            get { return _stretchWidth; }
            set
            {
                _stretchWidth = value;
                _stretchWidthChanged = true;
                Invalidate();
            }
        }

        private bool _stretchHeightChanged;
        private bool _stretchHeight;
        /// <summary>
        /// Stretch height
        /// </summary>
        public bool StretchHeight
        {
            get { return _stretchHeight; }
            set
            {
                _stretchHeight = value;
                _stretchHeightChanged = true;
                Invalidate();
            }
        }

        private bool _richTextChanged;
        private bool _richText;
        /// <summary>
        /// RichText
        /// </summary>
        public bool RichText
        {
            get
            {
                return _richText;
            }
            set
            {
                if (value == _richText)
                    return;
                _richText = value;
                _richTextChanged = true;
                Invalidate();
            }
        }

        #endregion

        #endregion

        #region Graphics

        private Texture2D _texture;

        private GraphicsBase _normalGraphics;
        /// <summary>
        /// NormalGraphics
        /// </summary>
        public GraphicsBase NormalGraphics
        {
            get
            {
                return _normalGraphics;
            }
            set
            {
                //Debug.Log("NormalGraphics setting: " + value);

                if (value == _normalGraphics)
                    return;

                _normalGraphics = value;
                _normalGraphics.Initialize();

                _texture = new Texture2D((int)value.Bounds.Width, (int)value.Bounds.Height);
                //_texture.filterMode = FilterMode.Point;
                //_texture.wrapMode = TextureWrapMode.Repeat;

                _normalGraphics.Texture = _texture;
                _normalGraphics.Draw();

                _texture.Apply();

                NormalBackground = _texture;

                //Debug.Log("NormalGraphics set");
            }
        }

        private GraphicsBase _hoverGraphics;
        /// <summary>
        /// HoverGraphics
        /// </summary>
        public GraphicsBase HoverGraphics
        {
            get
            {
                return _hoverGraphics;
            }
            set
            {
                //Debug.Log("HoverGraphics setting: " + value);

                if (value.Equals(_hoverGraphics))
                    return;

                _hoverGraphics = value;
                _hoverGraphics.Initialize();

                _texture = new Texture2D((int)value.Bounds.Width, (int)value.Bounds.Height);
                _hoverGraphics.Texture = _texture;
                _hoverGraphics.Draw();

                _texture.Apply();

                HoverBackground = _texture;
            }
        }

        private GraphicsBase _activeGraphics;
        /// <summary>
        /// ActiveGraphics
        /// </summary>
        public GraphicsBase ActiveGraphics
        {
            get
            {
                return _activeGraphics;
            }
            set
            {
                //Debug.Log("ActiveGraphics setting: " + value);

                if (value == _activeGraphics)
                    return;

                _activeGraphics = value;
                _activeGraphics.Initialize();

                _texture = new Texture2D((int)value.Bounds.Width, (int)value.Bounds.Height);
                _activeGraphics.Texture = _texture;
                _activeGraphics.Draw();

                _texture.Apply();

                ActiveBackground = _texture;
            }
        }

        private GraphicsBase _focusedGraphics;
        /// <summary>
        /// FocusedGraphics
        /// </summary>
        public GraphicsBase FocusedGraphics
        {
            get
            {
                return _focusedGraphics;
            }
            set
            {
                //Debug.Log("NormalGraphics setting: " + value);

                if (value == _focusedGraphics)
                    return;

                _focusedGraphics = value;
                _focusedGraphics.Initialize();

                _texture = new Texture2D((int)value.Bounds.Width, (int)value.Bounds.Height);
                _focusedGraphics.Texture = _texture;
                _focusedGraphics.Draw();

                _texture.Apply();

                FocusedBackground = _texture;
            }
        }

        private GraphicsBase _onNormalGraphics;
        /// <summary>
        /// NormalColor
        /// </summary>
        public GraphicsBase OnNormalGraphics
        {
            get
            {
                return _onNormalGraphics;
            }
            set
            {
                //Debug.Log("NormalGraphics setting: " + value);

                if (value == _onNormalGraphics)
                    return;

                _onNormalGraphics = value;
                _onNormalGraphics.Initialize();

                _texture = new Texture2D((int)value.Bounds.Width, (int)value.Bounds.Height);
                _onNormalGraphics.Texture = _texture;
                _onNormalGraphics.Draw();

                _texture.Apply();

                OnNormalBackground = _texture;
            }
        }

        private GraphicsBase _onHoverGraphics;
        /// <summary>
        /// NormalColor
        /// </summary>
        public GraphicsBase OnHoverGraphics
        {
            get
            {
                return _onHoverGraphics;
            }
            set
            {
                //Debug.Log("NormalGraphics setting: " + value);

                if (value == _onHoverGraphics)
                    return;

                _onHoverGraphics = value;
                _onHoverGraphics.Initialize();

                _texture = new Texture2D((int)value.Bounds.Width, (int)value.Bounds.Height);
                _onHoverGraphics.Texture = _texture;
                _onHoverGraphics.Draw();

                _texture.Apply();

                OnHoverBackground = _texture;
            }
        }

        private GraphicsBase _onActiveGraphics;
        /// <summary>
        /// NormalColor
        /// </summary>
        public GraphicsBase OnActiveGraphics
        {
            get
            {
                return _onActiveGraphics;
            }
            set
            {
                //Debug.Log("NormalGraphics setting: " + value);

                if (value == _onActiveGraphics)
                    return;

                _onActiveGraphics = value;
                _onActiveGraphics.Initialize();
                
                _texture = new Texture2D((int)value.Bounds.Width, (int)value.Bounds.Height);

                _onActiveGraphics.Texture = _texture;
                _onActiveGraphics.Draw();

                _texture.Apply();

                OnActiveBackground = _texture;
            }
        }

        private GraphicsBase _onFocusedGraphics;
        /// <summary>
        /// NormalColor
        /// </summary>
        public GraphicsBase OnFocusedGraphics
        {
            get
            {
                return _onFocusedGraphics;
            }
            set
            {
                //Debug.Log("NormalGraphics setting: " + value);

                if (value == _onFocusedGraphics)
                    return;

                _onFocusedGraphics = value;
                _onFocusedGraphics.Initialize();

                _texture = new Texture2D((int)value.Bounds.Width, (int)value.Bounds.Height);

                _onFocusedGraphics.Texture = _texture;
                _onFocusedGraphics.Draw();

                _texture.Apply();

                OnFocusedBackground = _texture;
            }
        }

        #endregion

        #region Invalidation

        public void Invalidate()
        {
            //if (null != _client)
            //    _client.InvalidateStyles(); // commented out 20130921 while refactoring for NotifyStyleChangeInChildren
        }

        public void Commit()
        {
            //if (_styleChanged)
            //{
            //    _styleChanged = false;
            //}

            //Debug.Log("Validate");

            if (null == Style)
                return;

            #region NormalColor .. OnFocused

            #region NormalColor

            if (_normalChanged)
            {
                _normalChanged = false;
                Style.normal = _normal;
            }

                #endregion

            #region Hover

            else if (_hoverChanged)
            {
                _hoverChanged = false;
                if (!AdvancedMode)
                    Style.hover = _hover;
            }

            #endregion

            #region Active

            if (_activeChanged)
            {
                _activeChanged = false;
                if (!AdvancedMode)
                    Style.active = _active;
            }

            #endregion

            #region Focused

            if (_focusedChanged)
            {
                _focusedChanged = false;
                if (!AdvancedMode)
                    Style.focused = _focused;
            }

            #endregion

            #region OnNormal

            if (_onNormalChanged)
            {
                _onNormalChanged = false;
                Style.onNormal = _onNormal;
            }

            #endregion

            #region OnHover

            if (_onHoverChanged)
            {
                _onHoverChanged = false;
                if (!AdvancedMode)
                    Style.onHover = _onHover;
            }

            #endregion

            #region OnActive

            if (_onActiveChanged)
            {
                _onActiveChanged = false;
                if (!AdvancedMode)
                    Style.onActive = _onActive;
            }

            #endregion

            #region OnFocused

            if (_onFocusedChanged)
            {
                _onFocusedChanged = false;
                if (!AdvancedMode)
                    Style.onFocused = _onFocused;
            }

            #endregion

            #endregion

            #region NormalColor .. OnFocused Background

            if (_normalBackgroundChanged)
            {
                //Debug.Log("_normalBackgroundChanged");
                _normalBackgroundChanged = false;
                Style.normal.background = _normalBackground;
            }

            if (_hoverBackgroundChanged)
            {
                _hoverBackgroundChanged = false;
                Style.hover.background = _hoverBackground;
            }

            if (_activeBackgroundChanged)
            {
                _activeBackgroundChanged = false;
                Style.active.background = _activeBackground;
            }

            if (_focusedBackgroundChanged)
            {
                _focusedBackgroundChanged = false;
                Style.focused.background = _focusedBackground;
            }

            if (_onNormalBackgroundChanged)
            {
                _onNormalBackgroundChanged = false;
                Style.onNormal.background = _onNormalBackground;
            }

            if (_onHoverBackgroundChanged)
            {
                _onHoverBackgroundChanged = false;
                Style.onHover.background = _onHoverBackground;
            }

            if (_onActiveBackgroundChanged)
            {
                _onActiveBackgroundChanged = false;
                Style.onActive.background = _onActiveBackground;
            }

            if (_onFocusedBackgroundChanged)
            {
                _onFocusedBackgroundChanged = false;
                Style.onFocused.background = _onFocusedBackground;
            }

            #endregion

            #region NormalColor .. OnFocused TextColor

            if (_normalTextColorChanged)
            {
                _normalTextColorChanged = false;
                Style.normal.textColor = _normalTextColor;
            }

            if (_hoverTextColorChanged)
            {
                _hoverTextColorChanged = false;
                Style.hover.textColor = _hoverTextColor;
            }

            if (_activeTextColorChanged)
            {
                _activeTextColorChanged = false;
                Style.active.textColor = _activeTextColor;
            }

            if (_focusedTextColorChanged)
            {
                _focusedTextColorChanged = false;
                Style.focused.textColor = _focusedTextColor;
            }

            if (_onNormalTextColorChanged)
            {
                _onNormalTextColorChanged = false;
                Style.onNormal.textColor = _onNormalTextColor;
            }

            if (_onHoverTextColorChanged)
            {
                _onHoverTextColorChanged = false;
                Style.onHover.textColor = _onHoverTextColor;
            }

            if (_onActiveTextColorChanged)
            {
                _onActiveTextColorChanged = false;
                Style.onActive.textColor = _onActiveTextColor;
            }

            if (_onFocusedTextColorChanged)
            {
                _onFocusedTextColorChanged = false;
                Style.onFocused.textColor = _onFocusedTextColor;
            }

            #endregion

            #region Other

            if (_borderChanged)
            {
                _borderChanged = false;
                Style.border = _border;
            }

            if (_paddingChanged)
            {
                _paddingChanged = false;
                Style.padding = _padding;
            }

            if (_marginChanged)
            {
                _marginChanged = false;
                Style.margin = _margin;
            }

            if (_overflowChanged)
            {
                _overflowChanged = false;
                Style.overflow = _overflow;
            }

            if (_fontChanged)
            {
                _fontChanged = false;
                Style.font = _font;
            }

            if (_imagePositionChanged)
            {
                _imagePositionChanged = false;
                Style.imagePosition = _imagePosition;
            }

            if (_alignmentChanged)
            {
                _alignmentChanged = false;
                Style.alignment = _alignment;
            }

            if (_wordWrapChanged)
            {
                _wordWrapChanged = false;
                Style.wordWrap = _wordWrap;
            }

            if (_textClippingChanged)
            {
                _textClippingChanged = false;
                Style.clipping = _textClipping;
            }

            if (_contentOffsetChanged)
            {
                _contentOffsetChanged = false;
                Style.contentOffset = _contentOffset;
            }

            if (_fixedWidthChanged)
            {
                _fixedWidthChanged = false;
                Style.fixedWidth = _fixedWidth;
            }

            if (_fixedHeightChanged)
            {
                _fixedHeightChanged = false;
                Style.fixedHeight = _fixedHeight;
            }

            if (_fontSizeChanged)
            {
                _fontSizeChanged = false;
                Style.fontSize = _fontSize;
            }

            if (_fontStyleChanged)
            {
                _fontStyleChanged = false;
                Style.fontStyle = _fontStyle;
            }

            if (_stretchWidthChanged)
            {
                _stretchWidthChanged = false;
                Style.stretchWidth = _stretchWidth;
            }

            if (_stretchHeightChanged)
            {
                _stretchHeightChanged = false;
                Style.stretchHeight = _stretchHeight;
            }

            if (_richTextChanged)
            {
                _richTextChanged = false;
                Style.richText = _richText;
            }

            #endregion

            #region Fix border

            // TODO
            // we got to loop through all the border edges to find the greatest width and height
            // then we have to fix the border

            #endregion
        }

        #endregion

        public void SetMouseAbove(bool above)
        {
            if (!AdvancedMode)
                return;

            if (above)
            {
                //Debug.Log("Setting above");
                Style.hover = _hover;
                Style.active = _active;
                Style.focused = _focused;
                Style.onHover = _onHover;
                Style.onActive = _onActive;
                Style.onFocused = _onFocused;
            }
            else
            {
                Style.hover = _normal;
                Style.active = _normal;
                Style.focused = _normal;
                Style.onHover = _onNormal;
                Style.onActive = _onNormal;
                Style.onFocused = _onNormal;
            }

            Invalidate();

        }
    }
}
