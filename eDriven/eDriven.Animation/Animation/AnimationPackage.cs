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
using System.Xml.Serialization;
using eDriven.Core.Serialization;

namespace eDriven.Animation.Animation
{
    [Serializable]
    [XmlRoot("AnimationPackage", Namespace = "http://edriven.dankokozar.com/")]
    public class AnimationPackage : PathDescriptor, IDisposable
    {
        [XmlArray("Animations")]
        [XmlArrayItem("Animation")]
        public StringIndexedList<Animation> Animations = new StringIndexedList<Animation>();

        /// <summary>
        /// Loads textures from descriptor
        /// </summary>
        public void LoadTextures()
        {
            Animations.ForEach(delegate (Animation a) { a.LoadTextures(Path); });
        }

        public void Dispose()
        {
            Animations.ForEach(delegate (Animation a) { a.Dispose(); });
        }

        public Animation Get(string id)
        {
            //Debug.Log("Package.Get: " + id);

            if (Animations.Contains(id))
                return Animations[id];

            return null; // if no such animation found
        }

        public override string ToString()
        {
            return string.Format("Animations.Count: {0}", Animations.Count);
        }
    }
}