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

namespace eDriven.Gui.Editor.Persistence
{
    /// <summary>
    /// Persists game object name change
    /// </summary>
    internal class PersistedNameChange : PersistedActionBase
    {
        /// <summary>
        /// Transform ID
        /// </summary>
        private readonly int _transformId;

        /// <summary>
        /// Transform ID visible from outside for comparison purposes
        /// </summary>
        public int TransformId
        {
            get { return _transformId; }
        }

        /// <summary>
        /// Name
        /// </summary>
        private readonly string _name;

        public PersistedNameChange(Transform transform)
        {
            _transformId = transform.GetInstanceID();
            _name = transform.gameObject.name;
        }

        public override string ToString()
        {
            return string.Format("TransformId: [{0}]; Name: [{1}]", _transformId, _name);
        }

        internal override void Apply()
        {
            Transform transform = TransformRegistry.Instance.Get(_transformId, true);
            transform.gameObject.name = _name;
        }
    }
}