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

using System;
using System.Reflection;
using eDriven.Gui.Components;
using eDriven.Gui.Geom;
using eDriven.Gui.Reflection;
using UnityEngine;
using Component = eDriven.Gui.Components.Component;

namespace eDriven.Gui.Designer.Adapters
{
    /// <summary>
    /// The adapter for Image component
    /// </summary>
    [Obfuscation(Exclude = true)]
    [Toolbox(Label = "Image", Icon = "eDriven/Editor/Controls/image")]

    public class ImageAdapter : ComponentAdapter
    {
        /// <summary>
        /// Image texture
        /// </summary>
        [Saveable]
        public Texture Texture;

        /// <summary>
        /// Image scale mode
        /// </summary>
        [Saveable]
        public ImageScaleMode ScaleMode;

        /// <summary>
        /// Image aspect ratio
        /// </summary>
        [Saveable]
        public float AspectRatio = 1f;

        /// <summary>
        /// Alpha blend
        /// </summary>
        [Saveable]
        public bool AlphaBlend;

        /// <summary>
        /// Tiling
        /// </summary>
        [Saveable]
        public ImageMode Mode;

        /// <summary>
        /// Scale9 left
        /// </summary>
        [Saveable]
        public int Scale9Left;

        /// <summary>
        /// Scale9 right
        /// </summary>
        [Saveable]
        public int Scale9Right;

        /// <summary>
        /// Scale9 top
        /// </summary>
        [Saveable]
        public int Scale9Top;

        /// <summary>
        /// Scale9 bottom
        /// </summary>
        [Saveable]
        public int Scale9Bottom;

        /// <summary>
        /// Scale9 bottom
        /// </summary>
        [Saveable]
        public Anchor TilingAnchor;

        /// <summary>
        /// Constructor
        /// </summary>
        public ImageAdapter()
        {
            MinWidth = 10;
            MinHeight = 10;
            TilingAnchor = Anchor.TopLeft;
        }

        public override Type ComponentType
        {
            get { return typeof(Image); }
        }

        public override Component NewInstance()
        {
            return new Image();
        }

        public override void Apply(Component component)
        {
            base.Apply(component);

            Image image = (Image)component;
            image.Texture = Texture;
            image.ScaleMode = ScaleMode;
            image.AspectRatio = AspectRatio;
            image.AlphaBlend = AlphaBlend;
            image.Mode = Mode;
            image.Scale9Metrics = new EdgeMetrics(Scale9Left, Scale9Right, Scale9Top, Scale9Bottom);
            image.TilingAnchor = TilingAnchor;
        }
    }
}