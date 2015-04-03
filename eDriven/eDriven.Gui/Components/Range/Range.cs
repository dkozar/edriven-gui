using eDriven.Core.Events;
using UnityEngine;

namespace eDriven.Gui.Components
{
    public class Range : SkinnableComponent
    {
        private MulticastDelegate _valueCommit;
        public MulticastDelegate ValueCommit
        {
            get
            {
                if (null == _valueCommit)
                    _valueCommit = new MulticastDelegate(this, FrameworkEvent.VALUE_COMMIT);
                return _valueCommit;
            }
            set
            {
                _valueCommit = value;
            }
        }

        private bool _maximumChanged;
        private float _maximum = 100;
        /// <summary>
        /// Maximal value
        /// </summary>
        public virtual float Maximum
        {
            get { 
                return _maximum;
            }
            set
            {
                if (value == _maximum)
                    return;
                
                _maximum = value;
                _maximumChanged = true;
                InvalidateProperties();
            }
        }

        private bool _minimumChanged;
        private float _minimum;
        /// <summary>
        /// Minimal value
        /// </summary>
        public virtual float Minimum
        {
            get { 
                return _minimum;
            }
            set
            {
                if (value == _minimum)
                    return;

                _minimum = value;
                _minimumChanged = true;
                InvalidateProperties();
            }
        }

        /**
         *  The amount that the <code>value</code> property 
         *  changes when the <code>changeValueByStep()</code> method is called. It must
         *  be a multiple of <code>snapInterval</code>, unless 
         *  <code>snapInterval</code> is 0. 
         *  If <code>stepSize</code>
         *  is not a multiple, it is rounded to the nearest 
         *  multiple that is greater than or equal to <code>snapInterval</code>.
         */
        private bool _stepSizeChanged;
        private float _stepSize = 1;
        /// <summary>
        /// Step size
        /// </summary>
        public virtual float StepSize
        {
            get { 
                return _stepSize;
            }
            set
            {
                if (value == _stepSize)
                    return;

                _stepSize = value;
                _stepSizeChanged = true;
                InvalidateProperties();
            }
        }

        private float _changedValue;

        private bool _valueChanged;
        private float _value;
        /// <summary>
        /// Value
        /// </summary>
        public virtual float Value
        {
            get {
                return _valueChanged ? _changedValue : _value;
            }
            set
            {
                if (value == _value)
                    return;

                _changedValue = value;
                _valueChanged = true;
                InvalidateProperties();
            }
        }

        private bool _explicitSnapInterval;

        private bool _snapIntervalChanged;
        private float? _snapInterval = 1;
// ReSharper disable MemberCanBeProtected.Global
        /// <summary>
        /// Snap interval
        /// </summary>
        public virtual float? SnapInterval
// ReSharper restore MemberCanBeProtected.Global
        {
            get { 
                return _snapInterval;
            }
            set
            {
                _explicitSnapInterval = true;

                if (value == _snapInterval)
                    return;

                // NaN effectively clears the snapInterval and resets it to 1.
                if (null == value)
                {
                    _snapInterval = 1;
                    _explicitSnapInterval = false;
                }
                else
                {
                    _snapInterval = value;
                }

                _snapIntervalChanged = true;
                _stepSizeChanged = true;

                InvalidateProperties();
            }
        }

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_minimum > _maximum)
            {
                // Check min <= max
                if (!_maximumChanged)
                    _minimum = _maximum;
                else
                    _maximum = _minimum;
            }

            if (_valueChanged || _maximumChanged || _minimumChanged || _snapIntervalChanged)
            {
                float currentValue = (_valueChanged) ? _changedValue : _value;
                _valueChanged = false;
                _maximumChanged = false;
                _minimumChanged = false;
                _snapIntervalChanged = false;
                SetValue(NearestValidValue(currentValue, _snapInterval));
            }
            
            if (_stepSizeChanged)
            {
                // Only modify stepSize if snapInterval was explicitly set.
                // Otherwise, snapInterval defaults to stepSize and we set
                // the value to respect the new snapInterval.
                if (_explicitSnapInterval)
                {
                    _stepSize = NearestValidSize(_stepSize);
                }
                else
                {
                    _snapInterval = _stepSize;
                    SetValue(NearestValidValue(_value, _snapInterval));
                }
                
                _stepSizeChanged = false;
            }
        }

        private float NearestValidSize(float size)
        {
            if (null == _snapInterval || _snapInterval == 0) // TODO???
                return size;

            float interval = (float)_snapInterval;

            float validSize = Mathf.Round((size / interval)) * interval;
            return (Mathf.Abs(validSize) < interval) ? interval : validSize;
        }

        protected float NearestValidValue(float value, float? interval)
        {
            if (null == interval || interval == 0)
                return Mathf.Max(_minimum, Mathf.Min(_maximum, value));
            
            float maxValue = _maximum - _minimum;
            float scale = 1;
            
            value -= _minimum;
            
            // If interval isn't an integer, there's a possibility that the floating point 
            // approximation of value or value/interval will be slightly larger or smaller 
            // than the real value.  This can lead to errors in calculations like 
            // floor(value/interval)*interval, which one might expect to just equal value, 
            // when value is an exact multiple of interval.  Not so if value=0.58 and 
            // interval=0.01, in that case the calculation yields 0.57!  To avoid problems, 
            // we scale by the implicit precision of the interval and then round.  For 
            // example if interval=0.01, then we scale by 100.    

            float interval2 = (float) interval;

            if (interval2 != Mathf.Round(interval2)) 
            { 
                string[] parts = ((1 + interval)+string.Empty).Split('.'); 
                scale = Mathf.Pow(10, parts[1].Length);
                maxValue *= scale;
                value = Mathf.Round(value * scale);
                interval2 = Mathf.Round(interval2 * scale);
            }   
            
            float lower = Mathf.Max(0, Mathf.Floor(value / interval2) * interval2);
            float upper = Mathf.Min(maxValue, Mathf.Floor((value + interval2) / interval2) * interval2);
            float validValue = ((value - lower) >= ((upper - lower) / 2)) ? upper : lower;
            
            return (validValue / scale) + _minimum;
        }

        protected virtual void SetValue(float value)
        {
            if (_value == value)
                return;
            if (/*0 != _maximum && 0 != _minimum &&*/ _maximum > _minimum)
                _value = Mathf.Min(_maximum, Mathf.Max(_minimum, value));
            else
                _value = value;

            DispatchEvent(new FrameworkEvent(FrameworkEvent.VALUE_COMMIT));
        }

        ///<summary>Changes the value by step
        ///</summary>
        ///<param name="increase"></param>
        public virtual void ChangeValueByStep(bool increase)
        {
            if (_stepSize == 0)
                return;

            float newValue = (increase) ? _value + _stepSize : _value - _stepSize;
            SetValue(NearestValidValue(newValue, _snapInterval));
        }
    }
}
