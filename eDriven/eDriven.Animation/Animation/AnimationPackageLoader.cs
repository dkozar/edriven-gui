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
using eDriven.Core.Caching;
using eDriven.Core.Serialization;
using eDriven.Core.Util;
using UnityEngine;

namespace eDriven.Animation.Animation
{
    public class AnimationPackageLoader : ISyncLoader<AnimationPackage>
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif
        private AnimationPackage _package;

        private readonly Cache<string, AnimationPackage> _cache = new Cache<string, AnimationPackage>();

        public AnimationPackage Load(string id)
        {
            _package = _cache.Get(id);

            if (null == _package)
            {
                // load new package and cache it
                //Debug.Log(string.Format("Loading animation package [{0}]", id));

                var package = Resources.Load(id);
                if (null == package)
                {
                    throw new Exception(string.Format(@"Cannot load package ""{0}""
Please check the existance of the animation package file at path: ""Resources/{0}""", id));
                }

                string xml = package.ToString();
#if DEBUG
                if (DebugMode)
                {
                    Debug.Log(xml);
                }
#endif
                _package = XmlSerializer<AnimationPackage>.Deserialize(xml);
                _package.LoadTextures();

                _cache.Put(id, _package); // cache it once
            }
            else
            {
#if DEBUG
                if (DebugMode)
                {
                    Debug.Log(string.Format("Package loaded from cache [{0}]", id));
                }
#endif
            }

            return _package;
        }
    }
}