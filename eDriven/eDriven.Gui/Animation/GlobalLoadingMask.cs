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

using eDriven.Gui.Components;
using eDriven.Gui.Stages;

namespace eDriven.Gui.Animation
{
    public class GlobalLoadingMask
    {
        private static LoadingMask _mask;
        private static bool _runOnce;

        #region Static

        /// <summary>
        /// Shows the global loading mask on it's own viewpoet
        /// </summary>
        /// <param name="message"></param>
        public static void Show(string message)
        {
            if (null != _mask) // destroy the previous mask
                _mask.Unmask();

            //GlobalLoadingMaskStage.Instance.Plugins[0].Initialize(GlobalLoadingMaskStage.Instance); // temp. fix (because CreationComplete not yet executed). Meditate on this!
            //GlobalLoadingMaskStage.Instance.DispatchEvent(new LoadingEvent(LoadingEvent.START, message));

            if (!_runOnce)
            {
                /* There's a glitch of not showing the first time. Investigate why this blobk is needed */
                var inst = GlobalLoadingMaskStage.Instance;
                inst.ValidateNow();
                _runOnce = true;
            }

            _mask = new LoadingMask(GlobalLoadingMaskStage.Instance, message);
        }

        /// <summary>
        /// Hides the global mask
        /// </summary>
        public static void Hide()
        {
            //GlobalLoadingMaskStage.Instance.DispatchEvent(new LoadingEvent(LoadingEvent.END));
            if (null == _mask)
                return;

            _mask.Unmask();
            _mask = null;
        }

        public static void SetMessage(string message)
        {
            if (null == _mask)
                return;
            _mask.SetMessage(message);
        }

        #endregion
    }
}
