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

using System.Reflection;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;

namespace eDriven.Gui.Designer.Adapters
{
    /// <summary>
    /// Component adapter base
    /// </summary>
    public abstract class ComponentAdapterBase : MonoBehaviour, IComponentAdapter
    {
        public abstract void Apply(Component component);

        private Component _component;
        public Component Component
        {
            get
            {
                return _component;
            }
            set
            {
                _component = value;
            }
        }

        // ReSharper disable UnusedMember.Local
        [Obfuscation(Exclude = true)]
        void ComponentInstantiated(Component component)
        // ReSharper restore UnusedMember.Local
        {
            _component = component;
        }

        public override string ToString()
        {
            return string.Format("Adapter [{0}]", GetType());
        }
    }
}