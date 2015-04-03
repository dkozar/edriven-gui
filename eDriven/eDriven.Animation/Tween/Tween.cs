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

using eDriven.Animation.Easing;
using eDriven.Animation.Interpolators;
using eDriven.Core.Reflection;

#if RELEASE
using eDriven.Core;
using eDriven.Animation.Check;
#else
using UnityEngine;
#endif

namespace eDriven.Animation
{
    /// <summary>
    /// A class used for motion tweening
    /// </summary>
    /// <remarks>Coded by Danko Kozar</remarks>
    public class Tween : TweenBase, ITween
    {
#if DEBUG
        public static bool DebugMode;
#endif

        #region Delegate

        public delegate float EasingFunction(float t, float b, float c, float d, float a, float p);

        public delegate object PropertyReaderFunction(object target); //ITween anim

        public delegate void PropertySetterFunction(object target); //ITween anim

        public delegate void CallbackFunction(IAnimation anim);

        //public delegate object PropertyGetter(object target, string property);

        //public delegate void CallbackFunction(params object[] vars);

        #endregion

        #region Static

        //private static readonly List<Tween> ActiveTweens = new List<Tween>();

        //public static void RegisterTween(Tween tween)
        //{
        //    if (!ActiveTweens.Contains(tween))
        //    {
        //        ActiveTweens.Add(tween);
        //    }

        //    /*List<ITween> list;
        //    if (Dict.ContainsKey(target))
        //    {
        //        list = Dict[target];
        //    }
        //    else
        //    {
        //        list = new List<ITween>();
        //        Dict.Add(target, list);
        //    }
        //    if (!list.Contains(tween))
        //    {
        //        list.Add(tween);
        //    }*/
        //}

        //public static void UnregisterTween(Tween tween)
        //{
        //    if (ActiveTweens.Contains(tween))
        //    {
        //        tween.Destroy();
        //        ActiveTweens.Remove(tween);
        //    }
        //}

        //public static void UnregisterObject(object target)
        //{
        //    ActiveTweens.ForEach(delegate(Tween tween)
        //    {
        //        if (tween.Target.Equals(target))
        //            tween.Destroy();
        //    });

        //    ActiveTweens.RemoveAll(delegate (Tween anim)
        //                               {
        //                                   return anim.Target.Equals(target);
        //                               });
        //}

        #endregion

        #region Properties

        public override object Target
        {
            get
            {
                //if (null == _target)
                //    throw new TweenerException(TweenerException.NoTargetDefined);

                return base.Target;
            }
            set
            {
                base.Target = value;

                if (null != StartValueReader && null == StartValueReader.Target)
                    StartValueReader.Target = base.Target;

                if (null != EndValueReader && null == EndValueReader.Target)
                    EndValueReader.Target = base.Target;
            }
        }

        private EasingFunction _easer = Linear.EaseNone;
        public EasingFunction Easer
        {
            get { return _easer; }
            set { _easer = value; }
        }

        private IInterpolator _interpolator;
        public IInterpolator Interpolator
        {
            get { return _interpolator; }
            set
            {
                if (null == value)
                    throw new Exception("Cannot add null as interpolator");

                _interpolator = value;
            }
        }

        public object StartValue { get; set; }
        public object EndValue { get; set; }

        public PropertyReader StartValueReader { get; set; }
        public PropertyReader EndValueReader { get; set; }

        public PropertyReaderFunction StartValueReaderFunction { get; set; }
        public PropertyReaderFunction EndValueReaderFunction { get; set; }

        private bool _isLaunched;
        public bool IsLaunched
        {
            get
            {
                return _isLaunched;
            }
        }

        private bool _isMute;
        public bool IsMute
        {
            get
            {
                return _isMute;
            }
        }

        private object _currentValue;
        public object CurrentValue
        {
            get { return _currentValue; }
        }

        #endregion

        #region Members

        //PropertyInfo _pi;
        private const float Starting = 0;
        private const float Final = 1;
        private float _fraction;

        ISetterProxy _proxy;

        override public ISetterProxy Proxy
        {
            get { return _proxy; }
            set { _proxy = value; }
        }

        #endregion

        #region Methods

        private void InitProxy()
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("InitProxy");
#endif
#if RELEASE
            /* RELEASE HACK CHECK */
            Acme2 acme = (Acme2) Framework.GetComponent<Acme2>(true);
            if (null == acme || !acme.gameObject.activeInHierarchy || !acme.enabled)
                return;
#endif

            //Debug.Log("InitProxy");
            // NOTE: "Pause" doesn't use target nor property, thus we are alowing the posibility of not declaring it
            if (CheckTarget && null == Target)
                throw new Exception(string.Format("Target not defined: " + this));

            //Debug.Log("_property: " + _property);

            if (CheckProperty && string.IsNullOrEmpty(Property))
                throw new Exception(string.Format("Property not defined: " + this));

            if (null != Target && null != Property && null == _proxy)
                _proxy = new MemberProxy(Target, Property);

            //Debug.Log("Proxy built: " + _proxy);
        }

        private void InitInterpolator()
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("InitInterpolator");
#endif
            if (null == _proxy.MemberType)
                throw new Exception("_proxy.MemberType not defined");

            //Debug.Log(string.Format("Getting default interpolator for [{0}]", _proxy.MemberType));
            Type interpolatorType = DefaultInterpolators.Instance.Get(_proxy.MemberType);
            if (null == interpolatorType)
                throw new Exception(string.Format("Couldn't find a default interpolator for [{0}]", _proxy.MemberType));
            Interpolator = (IInterpolator)Activator.CreateInstance(interpolatorType);
        }

        public void Stop()
        {
            //SystemManager.Instance.UpdateSignal.Disconnect(this);
            //UnregisterObject(Target);
        }

        /// <summary>
        /// Mutes the tween
        /// </summary>
        public void Mute()
        {
            _isMute = true;
        }

        public void Reset()
        {
            _currentValue = StartValue;

            _isMute = false;
            //UnregisterObject(Target);
            //SystemManager.Instance.UpdateSignal.Disconnect(this);
        }

        private DateTime _startTime;

        public override void Play()
        {
#if DEBUG
            if (DebugMode)
                Debug.Log(string.Format("Tween.Play [Delay: {0}] {1}", Delay, this));
#endif

            IsDone = false; // IMPORTANT for re-playing the tween

            if (!_initialized)
                Initialize();
        }

        private bool _initialized;
        private void Initialize()
        {
            if (null == _proxy)
                InitProxy();

            // check if interpolator not defined, and add one from defaults
            if (null != _proxy && null == _interpolator)
            {
                InitInterpolator();
            }

            //Debug.Log("Proxy is null: " + (null == _proxy));

            //Debug.Log("StartValue: " + StartValue);
            //Debug.Log("EndValue: " + EndValue);

            /**
             * Note: The priority is as follows:
             * 1) StartValue is checked. If set, skipping other checks
             * 2) StartValueReader is checked. If set, it reads the specified property
             * 3) StartValueReaderFunction is checked. If set, it is being run to get a property value.
             * - if niether of above steps resulted in having StartValue, we throw an exception
             * */

            if (null != StartValue)
            {
                // do nothing
            }

            else if (null != StartValueReader)
            {
                if (null == StartValueReader.Target && null != Target)
                {
                    StartValueReader.Target = Target;
                }
                if (null == StartValueReader.Property && null != Property)
                {
                    StartValueReader.Property = Property;
                }
                StartValue = StartValueReader.Read();
            }

            else if (null != StartValueReaderFunction)
                StartValue = StartValueReaderFunction(Target);

            if (CheckStartValue && null == StartValue)
                throw new Exception("StartValue, StartValueGetter or StartValueReaderFunction not set: " + this);

            if (null != EndValue)
            {
                // do nothing
            }

            if (null != EndValueReader)
            {
                if (null == EndValueReader.Target && null != Target)
                {
                    EndValueReader.Target = Target;
                }
                if (null == EndValueReader.Property && null != Property)
                {
                    EndValueReader.Property = Property;
                }
                EndValue = EndValueReader.Read();
            }

            else if (null != EndValueReaderFunction)
                EndValue = EndValueReaderFunction(Target);

            if (CheckEndValue && null == EndValue)
                throw new Exception("EndValue, EndValueReader or EndValueReaderFunction not set: " + this);

            // check if tween is needed

            if (null != StartValue && null != EndValue && EndValue.Equals(StartValue))
            {
#if DEBUG
                if (DebugMode)
                    Debug.Log("EndValue equals StartValue. Returning...");
#endif
                //if (IsChild)
                //{
                //    Parent.FinishedCount++;
                //}
                return;
            }

#if DEBUG
            if (DebugMode)
                Debug.Log("Playing: " + this);
#endif

            // go for a tween

            _startTime = DateTime.Now;

            //if (reset)
            _fraction = Starting;

            /**
             * Check if this is a root tween
             * If so, and not connected, connect
             * */
            /*if (null == Parent && !SystemManager.Instance.UpdateSignal.HasSlot(this))
                SystemManager.Instance.UpdateSignal.Connect(this);*/

            //Debug.Log("* INIT *");

            TweenRegistry.Instance.Add(this);

            _initialized = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Tick()
        {
            //Debug.Log("Tick: " + this);
            //if (!Active)
            //    return;

            //if (_isDone) {
            //    return true;
            //}

            if (!_initialized)
                Initialize();

            float currentTime = (float)(DateTime.Now - _startTime).TotalSeconds;

            if (!_isLaunched)
            {
                if (currentTime < Delay)
                    return false;

                _isLaunched = true;
                currentTime = 0;

                //Debug.Log(string.Format("*** starting: currentTime: {0} delay {1}", currentTime, Delay));
            }

            //if (!_isLaunched)
            //    return false;

            if (currentTime > Duration)
            {
                _fraction = Final; // move to final position
                IsDone = true;
            }
            else
            {
                //_fraction = Mathf.SmoothDamp(_fraction, Final, ref _velocity, Duration);
                _fraction = Easer(currentTime, 0, 1, Duration, 0, 0);
            }

            /**
             * Tweening the target
             * */
            if (!_isMute)
                ApplyValue();

            if (IsDone)
            {
#if DEBUG
                if (DebugMode)
                    Debug.Log("Finished: " + this);
#endif
                if (null != Callback) // TODO: we have a callback called twice! Check this with ComboBox (CLOSE event doubled)
                    Callback(this);
                
                //Debug.Log("*** removing tween *** " + this);
                TweenRegistry.Instance.Remove(this);

                Reset();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Applies the interpolated value to a target
        /// </summary>
        /// <exception cref="Exception"></exception>
        protected virtual void ApplyValue()
        {
            //Debug.Log("ApplyValue StartValue: " + StartValue);
            //Debug.Log("ApplyValue EndValue: " + EndValue);

            try
            {
                _currentValue = Interpolator.Interpolate(_fraction, StartValue, EndValue);
            }
            catch (Exception ex)
            {
                TweenErrorSignal.Instance.Emit(Target, this, _fraction, StartValue, EndValue, ex);
            }
            
            if (null == _proxy)
                throw new Exception("Proxy not defined: " + Name);

            _proxy.SetValue(_currentValue);
        }

        #endregion

        #region IDisposable

        public bool DestroyOnFinish = true;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Destroy()
        {
            if (DestroyOnFinish)
                TweenRegistry.Instance.Remove(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            //Debug.Log("Disposing tween: " + this);

            /*if (null != Next)
            {
                Next.Dispose();
                Next = null;
            }*/

            Destroy();
        }

        #endregion

        #region Pool

        //private static readonly ObjectPool<Tween> Tweens = new ObjectPool<Tween>();

        ///<summary>
        /// Gets the tween from the pool
        ///</summary>
        ///<returns></returns>
        public static Tween New()
        {
            //Tween tween = Tweens.Get(); // premature optimization...
            //tween.Init();
            Tween tween = new Tween();
            return tween;
        }

        #endregion

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override object Clone()
        {
            Tween t = (Tween)MemberwiseClone();
            t.Parent = Parent;
            if (null != StartValueReader)
                t.StartValueReader = (PropertyReader)StartValueReader.Clone();
            if (null != EndValueReader)
                t.EndValueReader = (PropertyReader)EndValueReader.Clone();
            if (Proxy is ICloneable) // added 20131107
                t.Proxy = (ISetterProxy)(Proxy as ICloneable).Clone();
            return t;
        }

        public override string ToString()
        {
            string n = string.Empty;
            if (null != Name)
            {
                n = string.Format("[{0}]", Name);
            }

            string t = string.Empty;
            if (null != Target)
            {
                t = string.Format("Target:{0}, ", Target);
            }

            string p = string.Empty;
            if (!string.IsNullOrEmpty(Property))
            {
                p = string.Format(@"Property:""{0}"";", Property);
            }

            string c = string.Empty;
            if (null != CurrentValue)
            {
                c = string.Format(", CurrentValue:{0}", CurrentValue);
            }

            return string.Format(@"Tween{0} [{1}{2}StartValue:{3}; EndValue:{4}; Duration:{5}{6}]",
                n,
                t,
                p, StartValue, EndValue, Duration,
                c
            );
        }
    }
}