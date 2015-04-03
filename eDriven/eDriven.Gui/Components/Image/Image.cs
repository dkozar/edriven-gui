using System;
using eDriven.Gui.Geom;
using eDriven.Gui.Util;
using UnityEngine;
using Object=UnityEngine.Object;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// Copyright © Danko Kozar 2010-2014. All rights reserved.
    /// </summary>

    public class Image : Component
    {
#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

        /// <summary>
        /// The border containing th ePatch-9 metadata that is being clipped outside of bounds
        /// </summary>
        public static EdgeMetrics Patch9Border = new EdgeMetrics(1, 1, 1, 1);

        #region Properties

        private bool _textureChanged;
        private Texture _texture;
        /// <summary>
        /// The icon texture
        /// </summary>
// ReSharper disable VirtualMemberNeverOverriden.Global
        public virtual Texture Texture
// ReSharper restore VirtualMemberNeverOverriden.Global
        {
            get { return _texture; }
            set
            {
                if (value != _texture)
                {
                    if (AutoDisposeUnmanagedResources && null != _texture)
                        //UnityEngine.Object.Destroy(_texture);
                        Object.DestroyImmediate(_texture, true);

                    _texture = value;
                    _textureChanged = true;
                    InvalidateProperties();
                    InvalidateSize();
                    InvalidateDisplayList();
                    
                    //SetActualSize(_texture.width, _texture.height);
                    //Debug.Log("Texture changed");
                }
            }
        }

        private float? _aspectRatio;

        /// <summary>
        /// Image aspect ratio
        /// </summary>
        public float? AspectRatio
        {
            get
            {
                return _aspectRatio;
            }
            set
            {
                _aspectRatio = value;
                InvalidateSize();
            }
        }

        /// <summary>
        /// Image scale mode
        /// </summary>
// ReSharper disable ConvertToConstant.Local
        private ScaleMode _scaleModeInternal = UnityEngine.ScaleMode.ScaleToFit;
// ReSharper restore ConvertToConstant.Local

        //public ImageScaleMode ScaleMode = ImageScaleMode.OriginalSize;

        private bool _scaleModeChanged = true; // init
        private ImageScaleMode _scaleMode = ImageScaleMode.OriginalSize;
        public ImageScaleMode ScaleMode
        {
            get
            {
                return _scaleMode;
            }
            set
            {
                if (value == _scaleMode)
                    return;

                _scaleMode = value;
                _scaleModeChanged = true;
                InvalidateProperties();
                InvalidateDisplayList();
            }
        }

        /// <summary>
        /// When tiling the texture, we could set the tiling anchor here<br/>
        /// The anchor is the fixed point of the texture
        /// </summary>
        private bool _tilingAnchorChanged;
        private Anchor _tilingAnchor = Anchor.TopLeft;
        public Anchor TilingAnchor
        {
            get { 
                return _tilingAnchor;
            }
            set
            {
                if (value == _tilingAnchor)
                    return;

                _tilingAnchor = value;
                _tilingAnchorChanged = true;
                InvalidateProperties();
            }
        }

        private bool _modeChanged;
        private ImageMode _mode;
        public ImageMode Mode
        {
            get { 
                return _mode;
            }
            set
            {
                if (value == _mode)
                    return;

                _mode = value;
                _modeChanged = true;
                InvalidateProperties();
            }
        }

        private bool _scale9RectChanged;
        private EdgeMetrics _scale9Metrics;
        public EdgeMetrics Scale9Metrics
        {
            get { 
                return _scale9Metrics;
            }
            set
            {
                if (Equals(value, _scale9Metrics))
                    return;

                _scale9Metrics = value;
                _scale9RectChanged = true;
                InvalidateProperties();
            }
        }

        //private bool _adjustWidthToTextureChanged;
        private bool _adjustWidthToTexture = true; // default = true
        public bool AdjustWidthToTexture
        {
            get { 
                return _adjustWidthToTexture;
            }
            set
            {
                if (value == _adjustWidthToTexture)
                    return;

                _adjustWidthToTexture = value;
                //_adjustWidthToTextureChanged = true;
                InvalidateSize();
            }
        }
        //private bool _adjustHeightToTextureChanged;
        private bool _adjustHeightToTexture = true; // default = true
        public bool AdjustHeightToTexture
        {
            get { 
                return _adjustHeightToTexture;
            }
            set
            {
                if (value == _adjustHeightToTexture)
                    return;

                _adjustHeightToTexture = value;
                //_adjustHeightToTextureChanged = true;
                InvalidateSize();
            }
        }

        /// <summary>
        /// Alpha blend
        /// </summary>
        public bool AlphaBlend = true;

        #endregion

        #region Overriden properties

        private bool _sizeChanged;
        public override float Width
        {
            get
            {
                return base.Width;
            }
            set
            {
                base.Width = value;
                _sizeChanged = true;
                InvalidateProperties();
            }
        }

        public override float Height
        {
            get
            {
                return base.Height;
            }
            set
            {
                base.Height = value;
                _sizeChanged = true;
                InvalidateProperties();
            }
        }

        #endregion

        #region Members

        private TextureWrapMode _wrapMode;

        /// <summary>
        /// A flag needed to pass the tiling change from CommitProperties to UpdateDisplayList
        /// </summary>
        private bool _tilingChanged;

        /// <summary>
        /// This rect is used in special modes (tiling, or if texture bigger than image - cropping)
        /// </summary>
        private Rect _textureRect;

        /// <summary>
        /// A flag indicating the cropping
        /// </summary>
        private bool _crop;

        /// <summary>
        /// Since we are looking if the rendering rect changed in Render method, we have to watch for change
        /// </summary>
        private Rect _oldRenderingRect;

        private GUIStyle _scale9Style;

        #endregion

        #region Constructor

        public Image()
        {
            MinWidth = 10;
            MinHeight = 10;
            FocusEnabled = false;
        }

        #endregion

        #region RENDERING

        protected override void Render()
        {
            if (null == Texture)
                return;

            if (Event.current.type != EventType.Repaint)
                return;

            /**
             * 1. Scale 9
             * */
            if (ImageMode.Scale9 == _mode)
            {
                _scale9Style.Draw(RenderingRect, string.Empty, false, false, false, false);
                return;
            }

            /**
             * 2. 9-patch
             * */
            if (ImageMode.Patch9 == _mode)
            {
                GUI.BeginGroup(RenderingRect);
                _scale9Style.Draw(LocalRenderingRect, string.Empty, false, false, false, false);
                GUI.EndGroup();
                return;
            }

            /**
             * 2. Tiled
             * */
            if (ImageMode.Tiled == _mode)
            {
                GUI.DrawTextureWithTexCoords(RenderingRect, Texture, _textureRect, AlphaBlend);
                return;
            }

            /**
             * 3. Normal
             * */
            if (ImageScaleMode.OriginalSize == _scaleMode)
            {
                if (_crop) // texture is bigger than the image size
                {
                    GUI.DrawTextureWithTexCoords(RenderingRect, Texture, _textureRect, AlphaBlend);
                    return;
                }

                // image is bigger than the texture
                if (RenderingRect != _oldRenderingRect)
                {
                    _oldRenderingRect = RenderingRect;
                    //Debug.Log("RenderingRect changed: " + RenderingRect);
                    _textureRect = new Rect
                    {
                        width = Texture.width,
                        height = Texture.height,
                        x = RenderingRect.x + (RenderingRect.width - Texture.width) / 2,
                        y = RenderingRect.y + (RenderingRect.height - Texture.height) / 2
                    };
                }

                GUI.DrawTexture(_textureRect, Texture, _scaleModeInternal, AlphaBlend);
                return;
            }

            if (null != _aspectRatio)
            {
                GUI.DrawTexture(RenderingRect, Texture, _scaleModeInternal, AlphaBlend, (float) _aspectRatio);
            }
            else {
                GUI.DrawTexture(RenderingRect, Texture, _scaleModeInternal, AlphaBlend);
            }
        }

        #endregion

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_textureChanged || _sizeChanged || _modeChanged || _scale9RectChanged)
            {
                _sizeChanged = false;
                _modeChanged = false;
                _scale9RectChanged = false;
                //Debug.Log("_texture: " + _texture);
                if (_textureChanged)
                    _textureChanged = false;

                if (null == _texture)
                    return;

                if (ImageMode.Scale9 == _mode || ImageMode.Patch9 == _mode)
                {
                    if (null == _scale9Style)
                    {
                        _scale9Style = new GUIStyle();
                    }
                    _scale9Style.normal.background = (Texture2D) _texture;

                    if (ImageMode.Patch9 == _mode)
                    {
                        _scale9Style.overflow = new RectOffset(
                            (int) Patch9Border.Left,
                            (int) Patch9Border.Right,
                            (int) Patch9Border.Top,
                            (int) Patch9Border.Bottom
                        );
                        var b = TextureUtil.EvaluatePatch9Border(_texture);
                        _scale9Style.border = new RectOffset((int)b.Left, (int)b.Right, (int)b.Top, (int)b.Bottom);
                    }
                    else
                    {
                        _scale9Style.overflow = new RectOffset();
                        // read border from scale 9 metrics
                        _scale9Style.border = new RectOffset(
                            (int)Math.Round(_scale9Metrics.Left),
                            (int)Math.Round(_scale9Metrics.Right), 
                            (int)Math.Round(_scale9Metrics.Top), 
                            (int)Math.Round(_scale9Metrics.Bottom)
                        );
                    }
                    return;
                }

                /* Invalidating size, because we may need to re-measure when changing the mode to/from Patch9 */
                InvalidateSize();

                InitializeContent();

                _tilingChanged = true;
                InvalidateDisplayList();
                
                //if ((_sizeChanged || _heightChanged))
                //{
                //    _sizeChanged = false;
                //    _heightChanged = false;

                //    //Debug.Log("RenderingRect changed: " + RenderingRect);
                //    _textureRect = new Rect
                //    {
                //        width = Texture.width,
                //        height = Texture.height,
                //        x = RenderingRect.x + (RenderingRect.width - Texture.width) / 2,
                //        y = RenderingRect.y + (RenderingRect.height - Texture.height) / 2
                //    };
                //}
            }
        }

        /*private float _oldWidth;
        private float _oldHeight;*/

        protected override void Measure()
        {
            // reset all, we don't need it since manipulating spacer size from the outside
            if (null != _texture)
            {
                int subtractX = (int) (ImageMode.Patch9 == _mode ? Patch9Border.Left + Patch9Border.Right : 0);
                int subtractY = (int) (ImageMode.Patch9 == _mode ? Patch9Border.Top + Patch9Border.Bottom : 0);

                if (_adjustWidthToTexture)
                    MeasuredWidth = MeasuredMinWidth = _texture.width - subtractX;
                if (_adjustHeightToTexture)
                    MeasuredHeight = MeasuredMinHeight = _texture.height - subtractY;

// ReSharper disable CompareOfFloatsByEqualityOperator
                /*if (_oldWidth != MeasuredWidth || _oldHeight != MeasuredHeight)
// ReSharper restore CompareOfFloatsByEqualityOperator
                {
                    _oldWidth = MeasuredWidth;
                    _oldHeight = MeasuredHeight;
                    _sizeChanged = true;
                    InvalidateDisplayList();
                }*/
            }
        }

        protected override void UpdateDisplayList(float width, float height)
        {
            base.UpdateDisplayList(width, height);

            float h = 0;
            float w = 0;
            float xOffset = 0;
            float yOffset = 0;

            if (null != _texture)
            {
                /*if (_tilingChanged || _tilingAnchorChanged)
                {
                    _tilingChanged = false;
                    _tilingAnchorChanged = false;*/
                if (ImageMode.Tiled == _mode)
                {
#if DEBUG
                    if (DebugMode)
                    {
                        Debug.Log(string.Format("*** TILING ON ({0}) ***", _tilingAnchor));
                    }
#endif
                    _wrapMode = TextureWrapMode.Repeat;
                    _texture.wrapMode = _wrapMode;

                    h = Height / _texture.height;
                    w = Width / _texture.width;
                    xOffset = 0;
                    yOffset = 0;

                    var tw = _texture.width;
                    var th = _texture.height;
                    var xRes = Width % tw;
                    var yRes = Height % th;

                    switch (_tilingAnchor)
                    {
                        case Anchor.TopCenter:
                        case Anchor.MiddleCenter:
                        case Anchor.BottomCenter:
                            xRes = Width * 0.5f % tw;
                            break;
                    }

                    switch (_tilingAnchor)
                    {
                        case Anchor.MiddleLeft:
                        case Anchor.MiddleCenter:
                        case Anchor.MiddleRight:
                            yRes = Height * 0.5f % th;
                            break;
                    }

                    //Debug.Log("xRes: " + xRes + "; yRes: " + yRes);

                    switch (_tilingAnchor)
                    {
                        case Anchor.TopLeft:
                            yOffset = -yRes / th;
                            break;
                        case Anchor.TopCenter:
                            xOffset = -xRes / tw + 0.5f;
                            yOffset = -yRes / th;
                            break;
                        case Anchor.TopRight:
                            xOffset = -xRes / tw;
                            yOffset = -yRes / th;
                            break;
                        case Anchor.MiddleLeft:
                            yOffset = -yRes / th + 0.5f;
                            break;
                        case Anchor.MiddleCenter:
                            xOffset = -xRes / tw + 0.5f;
                            yOffset = -yRes / th + 0.5f;
                            break;
                        case Anchor.MiddleRight:
                            xOffset = -xRes / tw;
                            yOffset = -yRes / th + 0.5f;
                            break;
                        case Anchor.BottomLeft:
                            // do nothing
                            break;
                        case Anchor.BottomCenter:
                            xOffset = -xRes / tw + 0.5f;
                            break;
                        case Anchor.BottomRight:
                            xOffset = -xRes / tw;
                            break;
                    }
                    _textureRect = new Rect(xOffset, yOffset, w, h);
                    //Debug.Log("_textureRect: " + _textureRect);
#if DEBUG
                    if (DebugMode)
                    {
                        Debug.Log("  -> textureRect: " + _textureRect);
                    }
#endif
                    return;
                }
                /*}*/

                if (_scaleModeChanged)
                {
                    _scaleModeChanged = false;

                    float aspectRatio;
                    float textureAspectRatio;

                    switch (_scaleMode)
                    {
                        case ImageScaleMode.OriginalSize:
                            _scaleModeInternal = UnityEngine.ScaleMode.ScaleToFit;
                            _wrapMode = TextureWrapMode.Clamp;
                            _crop = false;
                            if (Width < Texture.width || Height < Texture.height)
                            {
                                //GUI.DrawTexture(RenderingRect, Texture, _scaleModeInternal, AlphaBlend);
                                h = Height / Texture.height;
                                w = Width / Texture.width;
                                xOffset = (Texture.width - Width) * 0.5f / Texture.width;
                                yOffset = (Texture.height - Height) * 0.5f / Texture.height;
                                _textureRect = new Rect
                                                   {
                                                       x = xOffset,
                                                       y = yOffset,
                                                       width = w,
                                                       height = h
                                                   };
                                _crop = true;
                            } 
                            //else
                            //{


                            //    _textureRect = new Rect
                            //    {
                            //        width = Texture.width,
                            //        height = Texture.height,
                            //        x = X + (Width - Texture.width) / 2,
                            //        y = Y + (Height - Texture.height) / 2
                            //    };
                            //}
                            //Debug.Log("rect: " + rect);
                            break;

                        case ImageScaleMode.ScaleToWidth:
                            aspectRatio = Width / Height;
                            textureAspectRatio = (float)Texture.width / Texture.height;
                            _scaleModeInternal = aspectRatio <= textureAspectRatio ?
                                                                                       UnityEngine.ScaleMode.ScaleToFit :
                                                                                                                            UnityEngine.ScaleMode.ScaleAndCrop;
                            break;
                        case ImageScaleMode.ScaleToHeight:
                            aspectRatio = Width / Height;
                            textureAspectRatio = (float)Texture.width / Texture.height;
                            _scaleModeInternal = aspectRatio <= textureAspectRatio ?
                                                                                       UnityEngine.ScaleMode.ScaleAndCrop :
                                                                                                                              UnityEngine.ScaleMode.ScaleToFit;
                            break;
                        case ImageScaleMode.ScaleToFit:
                            _scaleModeInternal = UnityEngine.ScaleMode.ScaleToFit;
                            break;
                        case ImageScaleMode.ScaleToFill:
                            _scaleModeInternal = UnityEngine.ScaleMode.ScaleAndCrop;
                            break;
                        case ImageScaleMode.StretchToFit:
                            _scaleModeInternal = UnityEngine.ScaleMode.StretchToFill;
                            break;
                    }
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            if (AutoDisposeUnmanagedResources)
                //UnityEngine.Object.Destroy(_texture);
                Object.DestroyImmediate(_texture, true);
        }
    }

    public enum ImageMode
    {
        Normal, Tiled, Scale9, Patch9
    }

    public enum ImageScaleMode
    {
        /// <summary>
        /// Original size
        /// </summary>
        OriginalSize,
    
        /// <summary>
        /// Scale to 100% width, maintain original aspect ratio for height (clipping height if necessary)
        /// </summary>
        ScaleToWidth, 
        
        /// <summary>
        /// Scale to 100% height, maintain aspect ratio for width (clipping width if necessary)
        /// </summary>
        ScaleToHeight, 
        
        /// <summary>
        /// Scale to fit internal, maintaining aspect ratio (no clipping)
        /// </summary>
        ScaleToFit, 
        
        /// <summary>
        /// Scale to fill, maintaining aspect ratio (clipping)
        /// </summary>
        ScaleToFill, 
        
        /// <summary>
        /// Stretch to fit, ignoring aspect ratio
        /// </summary>
        StretchToFit, 
    }
}
