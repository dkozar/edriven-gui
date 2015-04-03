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

namespace eDriven.Animation
{
    /// <summary>
    /// The base class for tweens and similar (actions..)
    /// </summary>
    public abstract class TweenBase : IAnimation, IAsyncAction
    {
        /// <summary>
        /// The parent tween
        /// </summary>
        public ICompositeAsyncAction Parent { get; set; }

        /// <summary>
        /// Returns true if tween has a parent
        /// </summary>
        public bool IsChild
        {
            get { return null != Parent; }
        }

        protected bool CheckTarget = true;
        protected bool CheckProperty = true;
        protected bool CheckStartValue = true;
        protected bool CheckEndValue = true;

        /// <summary>
        /// Callback function when fires after the tween is finished
        /// </summary>
        public Tween.CallbackFunction Callback { get; set; }

        private object _target;
        /// <summary>
        /// Tweening target
        /// </summary>
        public virtual object Target
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value;
            }
        }

        private string _property;
        /// <summary>
        /// Tweening property
        /// </summary>
        public string Property
        {
            get
            {
                return _property;
            }
            set
            {
                _property = value;
            }
        }

        /// <summary>
        /// The delay before actual start
        /// </summary>
        public float Delay { get; set; }

        private float _duration = 1f;
        /// <summary>
        /// Tween duration
        /// </summary>
        public float Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        private bool _isDone;
        /// <summary>
        /// Returns true if tween finished playing
        /// </summary>
        public bool IsDone
        {
            get
            {
                return _isDone;
            }
            set
            {
                _isDone = value;
            }
        }

        /// <summary>
        /// Tween name
        /// </summary>
        public string Name { get; set; }

        public virtual ISetterProxy Proxy { get; set; }

        /// <summary>
        /// Plays the tween
        /// </summary>
        public virtual void Play()
        {
            
        }

        /// <summary>
        /// Plays the tween
        /// </summary>
        /// <param name="target"></param>
        public virtual void Play(object target)
        {
            Target = target;
            Play();
        }

        ///<summary>
        ///</summary>
        public virtual void Stop()
        {
            //if (!IsChild)
            //    TweenRegistry.Instance.Remove(this);
        }

        /// <summary>
        /// Does a single tick
        /// </summary>
        /// <returns></returns>
        public abstract bool Tick();

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public virtual object Clone()
        {
            TweenBase t = (TweenBase)MemberwiseClone();
            t.Parent = Parent;
            return t;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public abstract void Dispose();

        #region Options

        /// <summary>
        /// Sets target
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public TweenBase SetTarget(object target)
        {
            if (null == target)
                throw new Exception("Target cannot be null");

            Target = target;

            return this;
        }

        /// <summary>
        /// Sets property
        /// </summary>
        /// <param name="tweeningProperty"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public TweenBase SetProperty(string tweeningProperty)
        {
            if (string.IsNullOrEmpty(tweeningProperty))
                throw new Exception("Tweening property not defined");

            Property = tweeningProperty;

            return this;
        }

        /// <summary>
        /// Sets option
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public TweenBase SetOption(TweenOption option)
        {
            TweenUtil.ApplyOptions(this, new[] { option });
            return this;
        }

        /// <summary>
        /// Sets multiple options
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public TweenBase SetOptions(params TweenOption[] options)
        {
            TweenUtil.ApplyOptions(this, options);
            return this;
        }

        #endregion

        #region Helper

        /// <summary>
        /// Reads the value from the given object
        /// TODO: use the one from Core utils?
        /// </summary>
        /// <param name="target"></param>
        /// <param name="variableName"></param>
        /// <returns></returns>
        public static object ReadValue(object target, string variableName)
        {
            //return target.GetType().GetProperty(variableName).GetValue(target, Index);
            return CoreReflector.GetValue(target, variableName);
        }

        #endregion
    }
}