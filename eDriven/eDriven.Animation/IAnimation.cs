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

namespace eDriven.Animation
{
    /// <summary>
    /// Ihe interface defining methods for animator classes
    /// </summary>
    public interface IAnimation : ITargetedAction, ICloneable, IDisposable
    {
        /// <summary>
        /// Starts a tween with tween manager registration
        /// </summary>
        /// <param name="target"></param>
        void Play(object target);

        /// <summary>
        /// Stops a tween
        /// </summary>
        void Stop();

        ///// <summary>
        ///// Starts a tween, with the option not to register to tween manager
        ///// (tis option should be off for composite tweens, and only the root Composite tween should be registered)
        ///// </summary>
        ///// <param name="target"></param>
        ///// <param name="registerToTweenManager"></param>
        //void Play(object target, bool registerToTweenManager);

        /// <summary>
        /// The handler that should be executed after the tween is finished playing
        /// </summary>
        Tween.CallbackFunction Callback { get; set; }
    }
}