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
using eDriven.Core.Reflection;
using UnityEngine;

namespace eDriven.Animation
{
    /// <summary>
    /// The class used to read the property from an object<br/>
    /// It could be instantiated using different constructors<br/>
    /// If setting the target, it will read the property from the target
    /// If only the property name set, the target will be set by the tween using this property
    /// </summary>
    public class PropertyReader : ICloneable
    {
#if DEBUG
        public static bool DebugMode;
#endif
        /// <summary>
        /// The target to read the property from
        /// </summary>
        public object Target;

        /// <summary>
        /// The name of te property to read
        /// </summary>
        public string Property;

        /// <summary>
        /// Constructor
        /// </summary>
        public PropertyReader()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="property">Property name</param>
        public PropertyReader(string property)
        {
            Property = property;
        }

        /// <summary>
        /// Constroctor
        /// </summary>
        /// <param name="target">Target object</param>
        /// <param name="property">Property name</param>
        public PropertyReader(object target, string property)
        {
            Target = target;
            Property = property;
        }

        /// <summary>
        /// Reads the property
        /// </summary>
        /// <returns></returns>
        public object Read()
        {
            object value = CoreReflector.GetValue(Target, Property);
#if DEBUG
            if (DebugMode)
                Debug.Log(string.Format("Value read [{0}, {1}]: {2}", Target, Property, value));
#endif

            return value;
        }

        #region Implementation of ICloneable

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            PropertyReader propertyReader = new PropertyReader(Target, Property);
            return propertyReader;
        }

        #endregion
    }
}
