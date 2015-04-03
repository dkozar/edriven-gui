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
using eDriven.Gui.Reflection;

namespace eDriven.Gui.Designer.Adapters
{
    /// <summary>
    /// The adapter for VScrollBar component
    /// </summary>
    [Obfuscation(Exclude = true)]
    [Toolbox(Label = "VScrollBar", Icon = "eDriven/Editor/Controls/vscrollbar")]

    public class VScrollBarAdapter : SkinnableComponentAdapter
    {
        #region Saveable values

        [Saveable]
        public float Minimum;

        [Saveable]
        public float Maximum = 1000f;

        [Saveable]
        public float Value;

        [Saveable]
        public float PageSize = 300f;

        /*[Saveable]
        public float PageScrollSize = 1;

        [Saveable]
        public float LineScrollSize = 1;*/

        #endregion

        public VScrollBarAdapter()
        {
            MinHeight = 20;
            MinWidth = 20;

            // default: horizontal slider having 200px height
            UseHeight = true;
            UsePercentHeight = false;
            Height = 200;
        }

        public override Type ComponentType
        {
            get { return typeof(VScrollBar); }
        }

        public override Type DefaultSkinClass
        {
            get { return typeof(VScrollBarSkin); }
        }

        public override Component NewInstance()
        {
            return new VScrollBar();
        }

        public override void Apply(Component component)
        {
            base.Apply(component);

            VScrollBar scrollBar = (VScrollBar)component;
            scrollBar.Minimum = Minimum;
            scrollBar.Maximum = Maximum;
            scrollBar.Value = Value;
            scrollBar.PageSize = PageSize;
            /*scrollBar.PageScrollSize = PageScrollSize;
            scrollBar.LineScrollSize = LineScrollSize;*/
        }
    }
}