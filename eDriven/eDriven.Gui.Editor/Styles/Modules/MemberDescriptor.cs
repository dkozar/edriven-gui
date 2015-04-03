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
using eDriven.Gui.Styles;
using eDriven.Gui.Styles.Serialization;
using UnityEngine;

namespace eDriven.Gui.Editor.Styles
{
    public class MemberDescriptor
    {
        public readonly string Name;
        public readonly Type Type;
        public object Default;

        public readonly Texture Icon;

        public MemberDescriptor(string name, Type type, Texture icon, object @default = null)
        {
            Name = name;
            Type = type;
            Icon = icon;
            Default = @default;
        }

        public StyleProperty ToStyleProperty()
        {
            var property = NameValueBase.CreateProperty<StyleProperty>(Name, Type);
            property.Value = Default;
            return (StyleProperty) property;
        }

        public GUIContent ToGUIContent()
        {
            return new GUIContent(" " + Name, Icon);
        }
    }
}