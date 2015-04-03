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

using System.Collections;

namespace eDriven.Animation
{
    /// <summary>
    /// Properties and methods to be implemented by the tween factory
    /// in order to operate the tweens
    /// </summary>
    public interface ITweenFactoryOperator
    {
        /// <summary>
        /// The tween instance
        /// </summary>
        IAnimation Blueprint { get; set; }

        /// <summary>
        /// The handler that should be executed after all tween are finished playing
        /// </summary>
        Tween.CallbackFunction Callback { get; set; }

        /// <summary>
        /// Produces a tween instance (clones the blueprint)
        /// </summary>
        /// <returns></returns>
        IAnimation Produce();

        /// <summary>
        /// Configures a tween instance
        /// </summary>
        /// <returns></returns>
        void ConfigureInstance(IAnimation tween);

        /// <summary>
        /// Starts a tween with tween manager registration
        /// </summary>
        /// <param name="target"></param>
        IAnimation Play(object target);

        /// <summary>
        /// Plays multiple targets
        /// </summary>
        /// <param name="targets"></param>
        /// <returns></returns>
        IEnumerable Play(IEnumerable targets);

        /// <summary>
        /// Stops playing all the tweens
        /// </summary>
        void Stop();
    }
}