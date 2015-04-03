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
using UnityEngine;

namespace eDriven.Gui.Editor.Dialogs
{
    internal class EventHandlerDesc
    {
        public string Name;
        public string ScriptName;
        internal string Tooltip;
        internal Texture Icon;
        public Type Type;

        private GUIContent _content;
        public GUIContent GetContent()
        {
            if (null == _content)
                _content = new GUIContent(Name, Icon, Tooltip);

            return _content;
        }

        public EventHandlerDesc(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public EventHandlerDesc(string name, Type type, string tooltip)
        {
            Name = name;
            Type = type;
            Tooltip = tooltip;
        }

        public EventHandlerDesc(string name, Type type, Texture icon, string tooltip)
        {
            Name = name;
            Type = type;
            Icon = icon;
            Tooltip = tooltip;
        }

        public override string ToString()
        {
            return Name; // string.Format("{0} [{1}]", Name, Type);
        }
    }
}