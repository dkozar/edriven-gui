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
    /// The adapter for TextArea component
    /// </summary>
    [Obfuscation(Exclude = true)]
    [Toolbox(Icon = "eDriven/Editor/Controls/textarea")]

    public class TextAreaAdapter : ComponentAdapter
    {
        [Saveable]
        public string Text;

        [Saveable]
        public bool Optimized = true;

        public TextAreaAdapter()
        {
            MinWidth = 100;
            MinHeight = 60;
        }

        public override Type ComponentType
        {
            get { return typeof(TextArea); }
        }

        public override Component NewInstance()
        {
            return new TextArea();
        }

        public override void Apply(Component component)
        {
            base.Apply(component);

            TextArea textField = (TextArea)component;
            textField.Text = Text;
            textField.Optimized = Optimized;
        }
    }
}