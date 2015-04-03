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
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Reflection;

namespace eDriven.Gui.Designer.Collections
{
    [Saveable]
    [Serializable]
    public class SaveableAdapterCollection : SaveableCollectionBase<ComponentAdapter>
    {
        /// <summary>
        /// Converts the IList to PersistedList
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static SaveableAdapterCollection FromList(IList<ComponentAdapter> list)
        {
            SaveableAdapterCollection collection = new SaveableAdapterCollection();
            foreach (var item in list)
            {
                collection.Add(item);
            }
            return collection;
        }

        #region To string

        private StringBuilder _sb;
        
        public override string ToString()
        {
            if (null == _sb)
                _sb = new StringBuilder();

            _sb.AppendLine(string.Format("SaveableAdapterCollection ({0} items):", Items.Length));
            foreach (ComponentAdapter item in Items)
            {
                _sb.AppendLine("  -> " + item);
            }

            return _sb.ToString();
        }

        #endregion

        /// <summary>
        /// Contains
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(ComponentAdapter item)
        {
            return (new List<ComponentAdapter>(Items)).Contains(item);
        }
    }
}