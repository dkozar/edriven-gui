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

using System.Collections.Generic;
using System.Text;

namespace eDriven.Gui.Editor.Hierarchy
{
    /// <summary>
    /// SaveablePack is the object that is safe to persist, since it contains only 
    /// the (integer) IDs of the parent and child adapters
    /// </summary>
    internal class SaveablePack
    {
        private readonly int _parentInstanceId;
        public int ParentInstanceId
        {
            get { return _parentInstanceId; }
        }

        private readonly List<List<int>> _instanceIds = new List<List<int>>();
        public List<List<int>> InstanceIds
        {
            get
            {
                return _instanceIds;
            }
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parentInstanceId"></param>
        public SaveablePack(int parentInstanceId)
        {
            _parentInstanceId = parentInstanceId;
        }

        public void Add(List<int> instanceIds)
        {
            _instanceIds.Add(instanceIds);
        }

        /// <summary>
        /// Looks for adapters in the scene specified by saved IDs
        /// Applies the values to each parent adapet (collection)
        /// </summary>
        public void Write()
        {
            ChildGroupPack.Apply(this); //.Write();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("SaveablePack [{0}] ({1} groups)", _parentInstanceId, _instanceIds.Count));

            for (int i = 0; i < _instanceIds.Count; i++)
            {
                var ids = _instanceIds[i];
                sb.AppendLine(string.Format("   Group [{0}] ({1} adapters)", i, ids.Count));
                foreach (int id in ids)
                {
                    sb.AppendLine(string.Format("       [{0}]", id));
                }
            }

            return sb.ToString();
        }
    }
}