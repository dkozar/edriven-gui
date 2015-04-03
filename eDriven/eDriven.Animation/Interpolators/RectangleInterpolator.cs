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

using eDriven.Core.Geom;

namespace eDriven.Animation.Interpolators
{
    ///<summary>
    /// Interpolates rectangle
    ///</summary>
    public class RectangleInterpolator : IInterpolator
    {
        private Rectangle _startValue;
        private Rectangle _endValue;
        //private readonly Rectangle _retVal = new Rectangle();

        public object Interpolate(float fraction, object startValue, object endValue)
        {
            _startValue = (Rectangle)startValue;
            _endValue = (Rectangle)endValue;

            // NOTE: Beware!
            // You cannot use _retVal with reference types!
            // That is because the first time the _retVal is being returned from this interpolator, it Bounds of the target object start referencing it
            // The next moment, we are changing values here (on the same object), and when "appliing" the new values to the object, 
            // Equals returns true because those values are already there (it's the same Rectangle), so nothing happens

            //_retVal.X = _startValue.X + (_endValue.X - _startValue.X) * fraction;
            //_retVal.Y = _startValue.Y + (_endValue.Y - _startValue.Y) * fraction;
            //_retVal.Width = _startValue.Width + (_endValue.Width - _startValue.Width) * fraction;
            //_retVal.Height = _startValue.Height + (_endValue.Height - _startValue.Height) * fraction;
            //return _retVal;

            return new Rectangle(_startValue.X + (_endValue.X - _startValue.X) * fraction, _startValue.Y + (_endValue.Y - _startValue.Y) * fraction, _startValue.Width + (_endValue.Width - _startValue.Width) * fraction, _startValue.Height + (_endValue.Height - _startValue.Height) * fraction);
        }
    }
}