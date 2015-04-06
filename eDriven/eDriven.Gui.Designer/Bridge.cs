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

using eDriven.Core.Signals;
using Object=UnityEngine.Object;

namespace eDriven.Gui.Designer
{
    /// <summary>
    /// A class dispatching signals to interested subscribers
    /// Part od eDriven.Gui.Designer assembly
    /// Used for communication with eDriven.Gui.Editor assembly
    /// </summary>
    public static class Bridge
    {
        #region Monitoring editor changes

        /// <summary>
        /// Monitor signal is emiting the object needed to monitor
        /// The signal is being used for decoupling the concrete implementation, and because this library 
        /// (eDriven.Gui.Designer) shouldn't reference the eDriven.Gui.Editor library because of the circular problem
        /// </summary>
        public static Signal MonitorSignal = new Signal();

        /// <summary>
        /// Signals the persistence layer to monitor the component
        /// </summary>
        /// <param name="target"></param>
        public static void Monitor(Object target)
        {
            MonitorSignal.Emit(target);
        }

        #endregion
    }
}
