using eDriven.Animation.Interpolators;
using eDriven.Core.Geom;
using UnityEngine;

namespace Assets.eDriven.Demo.Tweens.Interpolators
{
    /// <summary>
    /// The simple example of programming an interpolator
    /// Using this layer of abstraction you can animate virtually anything, the way you want
    /// </summary>
    public class FlyingBeeInterpolator : IInterpolator
    {
        private Point _startValue;
        private Point _endValue;

        public float Amplitude = 100;

        float _distance;
        float _fractionInv;
        float _currentX;

        private readonly Point _retVal = new Point();

        public object Interpolate(float fraction, object startValue, object endValue)
        {
            _startValue = (Point)startValue; // Point
            _endValue = (Point)endValue;
            _distance = _endValue.X - _startValue.X;
            
            _fractionInv = 1 - fraction;
            _currentX = _distance * _fractionInv;

            _retVal.X = _startValue.X + (_distance)*fraction;
            _retVal.Y = _startValue.Y + Mathf.Sin(_currentX / 60) * Amplitude; // luckilly, we had prof. Bartulica... ^_^

            return _retVal;
        }
    }
}