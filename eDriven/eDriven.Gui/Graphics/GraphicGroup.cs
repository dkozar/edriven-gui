#region License

/*
 
Copyright (c) 2010-2014 Danko Kozar

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 
*/

#endregion License

using System.Collections.Generic;
using eDriven.Core.Geom;
using eDriven.Gui.Graphics.Base;
using UnityEngine;

namespace eDriven.Gui.Graphics
{
    public sealed class GraphicGroup : GraphicsBase
    {
        public GraphicGroup()
        {
        }

        public GraphicGroup(float width, float height, params GraphicsBase[] elements) : this(elements)
        {
            Width = width;
            Height = height;
        }

        public GraphicGroup(float left, float right, float top, float bottom, params GraphicsBase[] elements)
            : this(elements)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        public GraphicGroup(params GraphicsBase[] elements)
        {
            foreach (GraphicsBase element in elements)
            {
                Add(element);
            }
        }

        public GraphicGroup(params GraphicOption[] options)
            : base(options)
        {
        }

        private readonly List<GraphicsBase> _graphics = new List<GraphicsBase>();

        public void Add(GraphicsBase element)
        {
            _graphics.Add(element);
            element.Parent = this;
        }

        public void Remove(GraphicsBase element)
        {
            element.Parent = null;
            _graphics.Remove(element);
        }

        public void Clear()
        {
            _graphics.ForEach(delegate(GraphicsBase element)
                                  {
                                      element.Parent = null;
                                  });
            _graphics.Clear();
        }

        public int ElementCount
        {
            get
            {
                return _graphics.Count;
            }
        }

        internal override Rectangle Bounds
        {
            get
            {
                return base.Bounds;
            }
            set
            {
                base.Bounds = value;

                _graphics.ForEach(delegate(GraphicsBase g)
                {
                    g.Bounds = BoundsUtil.CalculateBounds(Bounds.Width, Bounds.Height, Bounds.X + g.Left, g.Right, Bounds.Y + g.Top, g.Bottom, g.Width, g.Height);
                });
            }
        }

        internal override Texture2D Texture
        {
            get
            {
                return base.Texture;
            }
            set
            {
                base.Texture = value;

                if (0 == _graphics.Count)
                    return;

                _graphics.ForEach(delegate(GraphicsBase graphics)
                {
                    graphics.Texture = value;
                });
            }
        }

        public override void Draw()
        {
            //Debug.Log("Drawing group");

            base.Draw();

            if (0 == _graphics.Count)
                return;

            _graphics.ForEach(delegate (GraphicsBase graphics)
                                  {
                                      graphics.Draw();
                                  });
        }
    }
}
