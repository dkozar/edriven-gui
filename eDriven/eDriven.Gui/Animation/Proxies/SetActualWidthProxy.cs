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
using eDriven.Core.Reflection;
using eDriven.Gui.Components;

namespace eDriven.Gui.Animation.Proxies
{
    /// <summary>
    /// The proxy used for calling the SetActualSize method rather than setting the Width property of the InvalidationManagerClient
    /// </summary>
    public class SetActualWidthProxy : ISetterProxy, ICloneable
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="target"></param>
        public SetActualWidthProxy(object target)
        {
            _target = (InvalidationManagerClient) target;
        }

        private readonly Type _memberType = typeof (float);

        public Type MemberType
        {
            get { return _memberType; }
            set { /* do nothing */ }
        }

        private readonly InvalidationManagerClient _target;
        
        public void SetValue(object value)
        {
            //Debug.Log("Setting width: " + value);
            _target.SetActualSize((float) value, _target.Height/*GetExplicitOrMeasuredHeight()*/);
        }

        public object Clone()
        {
            return new SetActualWidthProxy(_target) { MemberType = _memberType };
        }
    }
}