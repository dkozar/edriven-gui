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
using eDriven.Animation.Animation;
using eDriven.Core.Signals;
using eDriven.Core.Util;
using UnityEngine;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Animation
{
    /// <summary>
    /// Receives the animation package
    /// Plays the specified animation
    /// </summary>
    public sealed class FrameAnimator : IAnimator, IDisposable
    {
#if DEBUG
    // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif
        //public bool HideNativeCursorOnShow = true;

        private readonly Timer _timer;

        /// <summary>
        /// The signal indicating the animation changed
        /// </summary>
        public Signal AnimationChangeSignal = new Signal();

        /// <summary>
        /// The signal indicating that the frame changed
        /// </summary>
        public Signal FrameChangeSignal = new Signal();
        
        private AnimationPackage _package;
        public AnimationPackage Package
        {
            get
            {
                return _package;
            }
            set
            {
                _package = value;
                Reset();
            }
        }

        public FrameAnimator()
        {
            _timer = new Timer();
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        private void OnTimerTick(Event e)
        {
            var frame = _animation.Next();
            FrameChangeSignal.Emit(frame);
            if (frame.Duration > 0) {
                _timer.Delay = frame.Duration;_timer.Start();
            }
        }

        private Animation.Animation _animation;
        public Animation.Animation Animation
        {
            get
            {
                return _animation;
            }
            set
            {   /* IMPORTANT: skip0 this check, or else sometimes no timer start (this was the ValidateNow problem!) */
                /*if (_animation == value)
                    return;*/

                _animation = value;
                
                AnimationChangeSignal.Emit(_animation);

                if (null == _animation)
                    return;
                
                _animation.Reset(); // to set current frame
                FrameChangeSignal.Emit(_animation.Frames[0]);

                // animate if more that one frame
                if (_animation.Frames.Count > 1)
                {
                    _timer.Delay = _animation.Frames[0].Duration;
                    _timer.Start();
                }
                else
                {
                    _timer.Stop();
                }
            }
        }

        public void Play(string animationId)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("FrameAnimator.Play: " + animationId);
            }
#endif
            if (null != Package)
                Animation = Package.Get(animationId);
                //Animation = (Animation)Package.Get(animationId).Clone();
        }

        public void Stop()
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("FrameAnimator.Stop");
            }
#endif
            if (null != _timer)
                _timer.Stop();
        }

        public bool IsPlaying
        {
            get
            {
                return _timer.IsRunning;
            }
        }

        public void Reset()
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("FrameAnimator.Reset");
            }
#endif
            if (null != _animation)
            {
                _animation.Reset();
                if (null != _animation.CurrentFrame)
                {
                    //_cursorImage.Texture = _animation.CurrentFrame.Texture;
                    if (0 != _animation.CurrentFrame.Duration)
                        _timer.Delay = _animation.CurrentFrame.Duration;
                }
            }
        }

        public void Dispose()
        {
            AnimationChangeSignal.DisconnectAll();
            FrameChangeSignal.DisconnectAll();
        }
    }
}