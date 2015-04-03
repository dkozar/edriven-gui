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

namespace eDriven.Animation.Interpolators
{
    ///<summary>
    /// The interpolator that interpolates animation frames for float values
    ///</summary>
    public class ColorInterpolator : IInterpolator
    {
        private Color _startValue;
        private Color _endValue;

        public object Interpolate(float fraction, object startValue, object endValue)
        {
            _startValue = (Color)startValue;
            _endValue = (Color)endValue;
            
            var color = new Color(
                _startValue.r + (_endValue.r - _startValue.r) * fraction,
                _startValue.g + (_endValue.g - _startValue.g) * fraction,
                _startValue.b + (_endValue.b - _startValue.b) * fraction
            );
//            Debug.Log(string.Format(@"startValue: {0}; endValue: {1};
//fraction:{2}; out:{3} ", _startValue, _endValue, fraction, color));
            return color;
        }
    }
}