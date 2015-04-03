using System;
using System.Collections.Generic;
using eDriven.Core.Geom;
using UnityEngine;

namespace eDriven.Gui.Graphics.Base
{
    public abstract class GraphicsBase : ITextureDrawable, IGraphicsConstrainClient
    {
        internal virtual Texture2D Texture { get; set; }

        public IFill Fill;
        public IStroke Stroke;
        
        private readonly List<GraphicOption> _options = new List<GraphicOption>();
        public List<GraphicOption> Options
        {
            get { return _options; }
        }

        #region Constructor

        protected GraphicsBase()
        {
        }

        protected GraphicsBase(IFill fill, params GraphicOption[] options)
            : this(options)
        {
            Fill = fill;
        }

        protected GraphicsBase(IStroke stroke, params GraphicOption[] options)
            : this(options)
        {
            Stroke = stroke;
        }

        protected GraphicsBase(IFill fill, IStroke stroke, params GraphicOption[] options)
            :this(options)
        {
            Fill = fill;
            Stroke = stroke;
        }

        protected GraphicsBase(int width, int height, params GraphicOption[] options)
            : this(options)
        {
            Width = width;
            Height = height;
        }

        protected GraphicsBase(int width, int height, IStroke stroke, params GraphicOption[] options)
            : this(width, height, options)
        {
            Stroke = stroke;
        }

        protected GraphicsBase(int width, int height, IFill fill, params GraphicOption[] options)
            : this(width, height, options)
        {
            Fill = fill;
        }

        protected GraphicsBase(int width, int height, IFill fill, IStroke stroke, params GraphicOption[] options)
            : this(width, height, options)
        {
            Fill = fill;
            Stroke = stroke;
        }

        protected GraphicsBase(int x, int y, int width, int height, IStroke stroke, params GraphicOption[] options)
            : this(width, height, options)
        {
            Left = x;
            Top = y;
            Stroke = stroke;
        }

        protected GraphicsBase(int x, int y, int width, int height, IFill fill, params GraphicOption[] options)
            : this(width, height, options)
        {
            Left = x;
            Top = y;
            Fill = fill;
        }

        protected GraphicsBase(int x, int y, int width, int height, IFill fill, IStroke stroke, params GraphicOption[] options)
            : this(width, height, options)
        {
            Left = x;
            Top = y;
            Fill = fill;
            Stroke = stroke;
        }

        protected GraphicsBase(int x, int y, int width, int height, params GraphicOption[] options)
            : this(width, height, options)
        {
            Left = x;
            Top = y;
        }

        protected GraphicsBase(params GraphicOption[] options)
        {
            _options = new List<GraphicOption>(options);

            _options.ForEach(delegate (GraphicOption option)
                                 {
                                     switch (option.Type)
                                     {
                                         case GraphicOptionType.Left:
                                             _left = (float)option.Value;
                                             break;
                                         case GraphicOptionType.Right:
                                             _right = (float)option.Value;
                                             break;
                                         case GraphicOptionType.Top:
                                             _top = (float)option.Value;
                                             break;
                                         case GraphicOptionType.Bottom:
                                             _bottom = (float)option.Value;
                                             break;
                                         case GraphicOptionType.Width:
                                             _width = (float)option.Value;
                                             break;
                                         case GraphicOptionType.Height:
                                             _height = (float)option.Value;
                                             break;
                                         default:
                                             break;
                                     }
                                 });
        }

        #endregion

        internal Rectangle RenderingBounds
        {
            get
            {
                int w = Texture.width;
                int h = Texture.height;
                //Debug.Log("Texture.width: " + Texture.width + "; Texture.height: " + Texture.height);
                float totalHeight = (float)(null != Parent ? Parent.Bounds.Height : Height);
                return new Rectangle(Math.Min((int) Bounds.X, w),
                                     Math.Min((int) (totalHeight - (int)Bounds.Height - (int)(Top ?? 0)), h),
                                     Math.Min((int)Bounds.Width, w - Bounds.X),
                                     Math.Min((int)Bounds.Height, h - Bounds.Y)); // TODO
            }
        }

        protected Color[] GetPixels()
        {
            //Debug.Log("GetPixels RenderingBounds: " + Bounds);
            return Texture.GetPixels((int)RenderingBounds.X, (int)RenderingBounds.Y, (int)Bounds.Width, (int)Bounds.Height); // invert y to get top-left origin
        }

        protected void SetPixels(Color[] pixels)
        {
            Texture.SetPixels((int) RenderingBounds.X, (int) RenderingBounds.Y, (int)Bounds.Width, (int)Bounds.Height, pixels); // invert y for top-left origin
        }

        public virtual void Draw()
        {
            // fill in the zero alpha for top graphics
            if (null == Parent)
            {
                //Debug.Log("Drawing transparent background for " + this);

                Color[] colors = Texture.GetPixels(0, 0, Texture.width, Texture.height);

                Color color = new Color(1, 1, 1, 0);

                int count = 0;
                for (int y = 0; y < Texture.height; y++)
                {
                    for (int x = 0; x < Texture.width; x++)
                    {
                        colors[count] = color;
                        count++;
                    }
                }

                Texture.SetPixels(colors);
            }
        }

        #region Implementation of IGraphicsConstrainClient

        private float? _left;
        public float? Left
        {
            get { return _left; }
            set
            {
                _left = value;
                if (null != value)
                    Bounds.X = (float)value;
            }
        }

        private float? _right;
        public float? Right
        {
            get { return _right; }
            set
            {
                _right = value;
                if (null != value)
                    Bounds.Right = (float)value;
            }
        }

        private float? _top;
        public float? Top
        {
            get { return _top; }
            set
            {
                _top = value;
                if (null != value)
                    Bounds.Y = (float)value;
            }
        }

        private float? _bottom;
        public float? Bottom
        {
            get { return _bottom; }
            set
            {
                _bottom = value;
                if (null != value)
                    Bounds.Bottom = (float)value;
            }
        }

        private float? _width;
        public float? Width
        {
            get { return _width; }
            set
            {
                _width = value;
                if (null != value)
                    Bounds.Width = (float)value;
            }
        }

        private float? _height;
        public float? Height
        {
            get { return _height; }
            set { 
                _height = value;
                if (null != value)
                    Bounds.Height = (float) value;
            }
        }

        private GraphicGroup _parent;
        public GraphicGroup Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        private Rectangle _bounds = new Rectangle(0, 0, 50, 50);
        internal virtual Rectangle Bounds {
            get
            {
                return _bounds;
            }
            set
            {
                _bounds = value;
            }
        }

        //private Rectangle _textureBounds = new Rectangle(0, 0, 50, 50);
        //internal virtual Rectangle TextureBounds
        //{
        //    get
        //    {
        //        return _textureBounds;
        //    }
        //    set
        //    {
        //        _textureBounds = value;
        //    }
        //}

        internal virtual void Initialize()
        {
            if (null == Parent) // top group
            {
                if (null == Width)
                    throw new Exception("Top graphics has to have defined Width");
                
                if (null == Height)
                    throw new Exception("Top graphics has to have defined Height");

                //Debug.Log("Initialize: " + this);
                Bounds = new Rectangle(0, 0, (float) Width, (float) Height);
                //Debug.Log("Calculated Bounds: " + Bounds);
            }
        }

        #endregion

        /// <summary>
        /// If DrawFunction defined, uses it to find the color on the supplied coordinates
        /// If not, returns the given color
        /// </summary>
        /// <param name="drawable"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected Color CalculatePixelColor(IDraw drawable, int x, int y)
        {
            return null != drawable.DrawFunction ? drawable.DrawFunction(x, y, drawable.Color) : drawable.Color;
        }
    }
}