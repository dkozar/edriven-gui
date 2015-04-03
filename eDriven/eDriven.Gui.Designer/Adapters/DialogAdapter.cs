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
using eDriven.Gui.Containers;
using eDriven.Gui.Reflection;
using Component = eDriven.Gui.Components.Component;

namespace eDriven.Gui.Designer.Adapters
{
    /// <summary>
    /// The adapter for Dialog component
    /// </summary>
    [Obfuscation(Exclude = true)]
    [Toolbox(Icon = "eDriven/Editor/Controls/dialog")]

    public class DialogAdapter : WindowAdapter
    {
        public override Type ComponentType
        {
            get { return typeof(Dialog); }
        }

        public override Type DefaultSkinClass
        {
            get { return typeof(WindowSkin); }
        }

        /// <summary>
        /// Draggable
        /// </summary>
        [Saveable]
        public bool Draggable;

        /// <summary>
        /// Resizable
        /// </summary>
        [Saveable]
        public bool Resizable;

        /// <summary>
        /// Should the dialog close on Esc key?
        /// </summary>
        [Saveable]
        public bool CloseOnEsc;

        /// <summary>
        /// Constructor
        /// </summary>
        public DialogAdapter()
        {
            Title = "Dialog";
            MinWidth = 200;
            MinHeight = 100;
        }

        public override Component NewInstance()
        {
            return new Dialog();
        }

        //private ComponentAdapter[] _oldButtonGroupChildren = new ComponentAdapter[] { };

        public override void Apply(Component component)
        {
            base.Apply(component);

            Dialog dialog = (Dialog)component;
            dialog.Title = Title;
            dialog.Draggable = Draggable;
            dialog.Resizable = Resizable;
            dialog.CloseOnEsc = CloseOnEsc;
        }
    }
}