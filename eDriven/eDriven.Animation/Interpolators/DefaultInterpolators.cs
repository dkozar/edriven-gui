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
using eDriven.Core.Geom;
using UnityEngine;

namespace eDriven.Animation.Interpolators
{
    /// <summary>
    /// Default interpolators
    /// Contains the list of the most used interpolators
    /// The list should be completed on the application start
    /// </summary>
    public class DefaultInterpolators
    {

#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static DefaultInterpolators _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private DefaultInterpolators()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static DefaultInterpolators Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating DefaultInterpolators instance"));
#endif
                    _instance = new DefaultInterpolators();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private void Initialize()
        {
            _map.Add(typeof(float), typeof(FloatInterpolator));
            _map.Add(typeof(float?), typeof(FloatInterpolator));
            _map.Add(typeof(Point), typeof(PointInterpolator));
            _map.Add(typeof(Rectangle), typeof(RectangleInterpolator));
            _map.Add(typeof(Vector2), typeof (Vector2Interpolator));
            _map.Add(typeof(Vector3), typeof(Vector3Interpolator));
            _map.Add(typeof(Color), typeof(ColorInterpolator));
            _map.Add(typeof(Color?), typeof(ColorInterpolator));
        }

// ReSharper disable InconsistentNaming
        private readonly Dictionary<Type, Type> _map = new Dictionary<Type, Type>();
// ReSharper restore InconsistentNaming

// ReSharper disable UnusedMember.Global
        ///<summary>
        /// Sets the default interpolator for the supplied type
        ///</summary>
        ///<param name="targetType"></param>
        ///<param name="interpolatorType"></param>
        public void Put(Type targetType, Type interpolatorType)
// ReSharper restore UnusedMember.Global
        {
            if (!_map.ContainsKey(targetType))
                _map[targetType] = interpolatorType;
        }

        ///<summary>
        /// Tries to get the default interpolator for the supplied type 
        ///</summary>
        ///<param name="type"></param>
        ///<returns></returns>
        public Type Get(Type type)
        {
            if (_map.ContainsKey(type))
                return _map[type];
            
            return null;
        }
    }
}
