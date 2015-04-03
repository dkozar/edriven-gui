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

using UnityEngine;

namespace eDriven.Gui
{
    public sealed class AlertButtonDescriptor
    {
        public string Id;
        public string Text;
        public Texture Icon;
        public bool Focused;

        public AlertButtonDescriptor(string id, string text)
        {
            Id = id;
            Text = text;
        }

        public AlertButtonDescriptor(string id, string text, Texture icon)
        {
            Id = id;
            Text = text;
            Icon = icon;
        }

        public AlertButtonDescriptor(string id, string text, bool focused)
        {
            Id = id;
            Text = text;
            Focused = focused;
        }

        public AlertButtonDescriptor(string id, string text, Texture icon, bool focused)
        {
            Id = id;
            Text = text;
            Icon = icon;
            Focused = focused;
        }

        public override string ToString()
        {
            return string.Format("Id: {0}, Text: {1}, Focused: {2}", Id, Text, Focused);
        }
    }
}