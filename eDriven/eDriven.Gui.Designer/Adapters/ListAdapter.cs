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

namespace eDriven.Gui.Designer.Adapters
{
    /// <summary>
    /// The adapter for List component
    /// </summary>
    [Obfuscation(Exclude = true)]
    [Toolbox(Label = "List", Group = "Data", Icon = "eDriven/Editor/Controls/list")]

    public class ListAdapter : SkinnableComponentAdapter
    {
        public ListAdapter()
        {
            MinWidth = 150;
            MinHeight = 200;
        }

        public override Type ComponentType
        {
            get { return typeof(List); }
        }

        public override Type DefaultSkinClass
        {
            get { return typeof(ListSkin); }
        }

        public override Component NewInstance()
        {
            return new List();
        }

        /// <summary>
        /// Applies the values from the adapter to a component
        /// </summary>
        /// <param name="component"></param>
        public override void Apply(Component component)
        {
            base.Apply(component);
            // Component ID used by developer for locating the component via ComponentManager<br/>
            // Must be unique in the application
            List list = (List)component;
        }
    }
}