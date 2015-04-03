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
    public class Vector3Interpolator : IInterpolator
    {
        private Vector3 _startValue;
        private Vector3 _endValue;
        private Vector3 _retVal;

        public object Interpolate(float fraction, object startValue, object endValue)
        {
            _startValue = (Vector3)startValue;
            _endValue = (Vector3)endValue;

            _retVal.x = _startValue.x + (_endValue.x - _startValue.x) * fraction;
            _retVal.y = _startValue.y + (_endValue.y - _startValue.y) * fraction;
            _retVal.z = _startValue.z + (_endValue.z - _startValue.z) * fraction;

            return _retVal;
        }
    }
}