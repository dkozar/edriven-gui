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
using System.Collections;
using System.Collections.Generic;
using eDriven.Animation.Easing;
using eDriven.Animation.Plugins;
using eDriven.Core.Signals;
using UnityEngine;

namespace eDriven.Animation
{
    /// <summary>
    /// The factory that produces and handles tweens
    /// </summary>
    public class TweenFactory : ITweenFactory
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        /// <summary>
        /// The signal that fires when factory stops working. Used by plugins.
        /// </summary>
        //public Signal StartAllSignal = new Signal();

        /// <summary>
        /// The signal that fires when factory starts working. Used by plugins.
        /// </summary>
        public Signal StartSignal = new Signal();

        /// <summary>
        /// The signal that fires when factory stops working. Used by plugins.
        /// </summary>
        public Signal StopSignal = new Signal();

        /// <summary>
        /// The signal that fires when factory stops working. Used by plugins.
        /// </summary>
        public Signal StopAllSignal = new Signal();

        #region Members

        /// <summary>
        /// The list of instances handled by this factory
        /// </summary>
        private readonly List<IAnimation> _instances = new List<IAnimation>();

        private int _count;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public TweenFactory()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TweenFactory(Type blueprintType)
        {
            _bluePrint = (IAnimation)Activator.CreateInstance(blueprintType);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TweenFactory(IAnimation blueprint)
        {
            //Debug.Log("Constructing TweenFactory with " + blueprint);
            _bluePrint = blueprint;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TweenFactory(Type blueprintType, ITweenFactoryPlugin[] plugins)
            : this(blueprintType)
        {
            _plugins = plugins;
            InitPlugins();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TweenFactory(IAnimation blueprint, ITweenFactoryPlugin[] plugins)
            : this(blueprint)
        {
            _plugins = plugins;
            InitPlugins();
        }

        #endregion

        #region Implementation of ITweenFactory

        private IAnimation _bluePrint;
        public IAnimation Blueprint
        {
            get { return _bluePrint; }
            set { _bluePrint = value; }
        }

        #region Properties

        public object Target { get; set; }

        public string Property { get; set; }

        /// <summary>
        /// The delay before the first active tween
        /// </summary>
        public float StartDelay { get; set; }

        public float Delay { get; set; }

        public float Duration { get; set; }

        /// <summary>
        /// TEMP
        /// Just a descriptive name for logging etc.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        private Tween.EasingFunction _easer = Expo.EaseOut;
        public Tween.EasingFunction Easer
        {
            get { return _easer; }
            set { _easer = value; }
        }

        #endregion

        #region Producing

        /// <summary>
        /// Produces a tween instance
        /// NOTE: optimize cloning
        /// <see cref="http://stackoverflow.com/questions/129389/how-do-you-do-a-deep-copy-an-object-in-net-c-specifically"/>
        /// Using the "Nested MemberwiseClone" technique is an order of magnitude faster again (see my post under @Gravitas). – Gravitas Jan 1 at 23:29
        /// </summary>
        /// <returns></returns>
        public IAnimation Produce()
        {
            if (null == _bluePrint)
                throw new Exception("Blueprint is null");

            IAnimation clone = (IAnimation)_bluePrint.Clone();

            //Debug.Log("*** CLONED: " + clone);

#if DEBUG
            if (DebugMode)
            {
                Debug.Log(@"Blueprint cloned: " + clone);
            }
#endif
            // set default delay here, so it could be overriden
            clone.Delay = Delay;
            clone.Duration = Duration;

            var t = clone as ITween;
            if (null != t)
                t.Easer = _easer;

            ConfigureInstance(clone);

#if DEBUG
            if (DebugMode)
            {
                Debug.Log("TweenFactory->Produce: " + clone);
            }
#endif
            //Debug.Log("Instance configured: " + clone);

            return clone;
        }

        public virtual void ConfigureInstance(IAnimation tween)
        {
            // override in wrappers
            //Debug.Log("TweenFactory->ConfigureInstance " + tween);
        }

        #endregion

        #region Play / stop

        public virtual IAnimation Play(object target)
        {
            //Debug.Log("Play: " + target);
            //PlayOnTargets(target, 0);
            _callbackFired = false;
            IAnimation anim = ProduceAndPlay(target);
            return anim;
        }

        public virtual IEnumerable Play(IEnumerable targets)
        {
            //Debug.Log("Play: " + targets);
            _callbackFired = false;
            return PlayOnTargets(targets);
        }

        public void Stop()
        {
            //Debug.Log("Stopping");
            _count = 0;
            _totalDelay = 0;
            StopAllSignal.Emit();

            // NOTE: We do not dispose. TweenRegistry disposes!
            //_instances.ForEach(delegate(IAnimation tween) { tween.Dispose(); });
            _instances.Clear();
            if (!_callbackFired && null != Callback)
            {
                Callback(null);
            }
            _callbackFired = true;
        }

        public Tween.CallbackFunction Callback { get; set; }

        #endregion

        #endregion

        #region Handlers

        private bool _callbackFired;

        private void SingleTweenFinishedHandler(IAnimation anim)
        {
            _count--;
            //TweenRegistry.Instance.Remove(anim);

            //Debug.Log("_count:" + _count);

            StopSignal.Emit(anim.Target);

            if (0 == _count)
            {
                Stop();
                ////// all tweens finished
                ////if (null != Callback)
                ////    Callback(null);

                ////_callbackFired = true;

                ////_instances.Clear();
            }
        }

        #endregion

        #region Helper

        /// <summary>
        /// The summed up delay
        /// When in delay mode (Delay != 0) TweenFactory functions like a delay line
        /// Meaning it introduces a delay for each consequent tween
        /// It doesn't metter if multiple objects are being played at once, or one-by-one
        /// This variable holds the sum of all delays
        /// After all the tweens stopped playing, this variable is being set back to 0.
        /// </summary>
        private float _totalDelay;

        private IEnumerable PlayOnTargets(IEnumerable targets)
        {
            //Debug.Log("------------- PlayOnTargets called ---");
            //int count = 0;
            List<IAnimation> tweens = new List<IAnimation>();
            foreach (object target in targets)
            {
                tweens.Add(ProduceAndPlay(target/*,Delay * count*/));
                //count++;
            }
            //Debug.Log("------- Playing " + count + " targets");
            return tweens;
        }

        private IAnimation ProduceAndPlay(object target)
        {
            _count++;
            _totalDelay += (1 == _count ? StartDelay : Delay);

            //if (1 == _count)
            //    StartAllSignal.Emit();

            StartSignal.Emit(target);

            //Debug.Log("_totalDelay: " + _totalDelay);

#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format("TweenFactory.Play->{0} [Delay: {1} s]", target, _totalDelay));
            }
#endif
            var tween = Produce();
            //tween.Easer = _easer;
            tween.Target = target;
            tween.Delay = _totalDelay; // NOTE: Delay is being set here from the parameter

            //Debug.Log("Produced tween ---> " + tween);

            tween.Play(target);

            //Debug.Log("Played tween ---> " + tween);

            _instances.Add(tween);

            //Debug.Log("Plus ---> " + _count);

            //if (null != Callback)
            tween.Callback = SingleTweenFinishedHandler; // SingleTweenFinishedHandler;

            //// register to tween manager
            //TweenRegistry.Instance.Add(tween);

            return tween;
        }

        #endregion

        #region To string

        public override string ToString()
        {
            return string.Format("TweenFactory -> {0}", _bluePrint);
        }

        public void Initialize(object target)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Plugins

        private readonly ITweenFactoryPlugin[] _plugins = new ITweenFactoryPlugin[] { };

        /// <summary>
        /// Initializes plugins
        /// </summary>
        private void InitPlugins()
        {
            if (null == _plugins)
                return;

            foreach (ITweenFactoryPlugin plugin in _plugins)
            {
                plugin.Initialize(this);
            }
        }

        #endregion


    }
}