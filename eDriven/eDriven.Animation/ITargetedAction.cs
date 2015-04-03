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

namespace eDriven.Animation
{
    /// <summary>
    /// The interface implemented by each action in tweening paradigm (tween, action, set property..)
    /// </summary>
    public interface ITargetedAction
    {
        /// <summary>
        /// The animation target
        /// This is the object which property has to be tweened
        /// </summary>
        object Target { get; set; }

        /// <summary>
        /// The property to tween
        /// </summary>
        string Property { get; set; }

        /// <summary>
        /// The time before the actual start of each tween
        /// </summary>
        float Delay { get; set; }

        /// <summary>
        /// Tween duration
        /// </summary>
// ReSharper disable UnusedMember.Global
        float Duration { get; set; }

        /// <summary>
        /// Just a descriptive name for logging etc.
        /// </summary>
        string Name { get; set; }
    }
}