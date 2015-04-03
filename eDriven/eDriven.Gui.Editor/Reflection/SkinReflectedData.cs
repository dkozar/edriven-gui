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
using System.Collections.Generic;
using System.Text;

namespace eDriven.Gui.Editor.Reflection
{
    internal class SkinReflectedData
    {
        public static string NewLine = @"
";
        public Type Default;
        public List<Type> DirectSkins = new List<Type>();
        public Dictionary<Type, List<Type>> SuperclassSkins = new Dictionary<Type, List<Type>>();

        public bool IsDirectSkin(Type skin)
        {
            return DirectSkins.Contains(skin);
        }

        public bool IsSuperclassSkin(Type skin)
        {
            foreach (var skins in SuperclassSkins.Values)
            {
                if (skins.Contains(skin))
                    return true;
            }
            return false;
        }

        public Type GetSuperclassForASkin(Type skin)
        {
            foreach (var skins in SuperclassSkins.Values)
            {
                if (skins.Contains(skin))
                    return skin;
            }
            return null;
        }

        public int Count
        {
            get
            {
                int count = DirectSkins.Count;
                foreach (var skins in SuperclassSkins.Values)
                {
                    count += skins.Count;
                }
                return count;
            }
        }

        public List<Type> ToList()
        {
            List<Type> list = new List<Type>();
            foreach (var skin in DirectSkins)
            {
                list.Add(skin);
            }
            foreach (var skins in SuperclassSkins.Values)
            {
                foreach (var skin in skins)
                {
                    list.Add(skin);
                }
            }
            return list;
        }

        public override string ToString()
        {
            if (DirectSkins.Count == 0 && SuperclassSkins.Count == 0)
            {
                return string.Format(@"Skins: Not skinnable." + NewLine + NewLine);
            }

            StringBuilder sb = new StringBuilder();
            
            if (null != Default)
            {
                string more = string.Empty;
                if (IsSuperclassSkin(Default))
                    more = string.Format(" [{0}]", GetSuperclassForASkin(Default));

                sb.AppendLine(string.Format(@"{0} [Default]{1}
    ", Default, more)); // write it out at the first place
            }

            foreach (var skin in DirectSkins)
            {
                if (skin != Default)
                    sb.AppendLine(skin.FullName);
            }

            foreach (var additional in SuperclassSkins)
            {
                foreach (var skin in additional.Value)
                {
                    if (skin != Default)
                        sb.AppendLine(string.Format("{0} [{1}]", skin.FullName, additional.Key));
                }
            }

            return string.Format(@"Skins ({0}):
----------------------------------------------------------------------------------------
{1}", Count, sb);
        }
    }
}