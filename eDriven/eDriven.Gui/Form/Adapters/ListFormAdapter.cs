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

using eDriven.Gui.Components;

namespace eDriven.Gui.Form
{
    public class ListFormAdapter : FormAdapterBase<List>
    {
        #region Implementation of IFormAdapter

        public override object GetValue(Component component)
        {
            return Convert(component).SelectedItem;
        }

        /// <summary>
        /// Sets the component value
        /// </summary>
        /// <param name="component"></param>
        /// <param name="value"></param>
        public override void SetValue(Component component, object value)
        {
            Convert(component).SelectedItem = value;
        }

        /// <summary>
        /// Resets the component to its default value
        /// </summary>
        public override void Reset(Component component)
        {
            Convert(component).SelectedIndex = -1;
        }

        #endregion
    }
}