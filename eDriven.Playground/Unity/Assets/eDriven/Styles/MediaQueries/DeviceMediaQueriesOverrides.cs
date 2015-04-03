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

using UnityEngine;
#if UNITY_EDITOR
using eDriven.Gui.Styles;
using UnityEditor;

namespace Assets.eDriven.Styles.MediaQueries
{
    /// <summary>
    /// Note: this is the editor class<br/>
    /// All of these queries override the default queries residing in the DeviceMediaQueries class<br/>
    /// Overrides are being done by ID (being read from the MediaQuery attribute), 
    /// e.g. "minDeviceWidth" and NOT by the method name ("MinDeviceWidth")
    /// </summary>
    public static class DeviceMediaQueriesOverrides
    {
        /// <summary>
        /// Note: Override of DeviceMediaQueries
        /// </summary>
        /// <param name="minDeviceWidth"></param>
        /// <returns></returns>
        [MediaQuery("minDeviceWidth", "Min device width in pixels", EditorOverride = true)]
        public static bool MinDeviceWidth(int minDeviceWidth)
        {
            return GetScreenWidth() >= minDeviceWidth;
        }

        /// <summary>
        /// Note: Override of DeviceMediaQueries
        /// </summary>
        /// <param name="minDeviceHeight"></param>
        /// <returns></returns>
        [MediaQuery("minDeviceHeight", "Min device height in pixels", EditorOverride = true)]
        public static bool MinDeviceHeight(int minDeviceHeight)
        {
            return GetScreenHeight() >= minDeviceHeight;
        }

        /// <summary>
        /// Note: Override of DeviceMediaQueries
        /// </summary>
        /// <param name="minDeviceWidth"></param>
        /// <returns></returns>
        [MediaQuery("maxDeviceWidth", "Max device width in pixels", EditorOverride = true)]
        public static bool MaxDeviceWidth(int minDeviceWidth)
        {
            return GetScreenWidth() <= minDeviceWidth;
        }

        /// <summary>
        /// Note: Override of DeviceMediaQueries
        /// </summary>
        /// <param name="minDeviceHeight"></param>
        /// <returns></returns>
        [MediaQuery("maxDeviceHeight", "Max device height in pixels", EditorOverride = true)]
        public static bool MaxDeviceHeight(int minDeviceHeight)
        {
            return GetScreenHeight() <= minDeviceHeight;
        }

        #region Helper

        /// <summary>
        /// A hack for targeting the Game view (and not the Inspector window) when in editor
        /// Using this because media queries are (currently) also being called from stylesheet property drawers
        /// and Screen.width might return the Inspector window width
        /// Note: UnityStats.screenRes contains the information from the previous frame
        /// so you might get for instance 599px instead of 600px (editor results are not synchronous)
        /// </summary>
        /// <returns></returns>
        private static Vector2 GetScreenSize()
        {
            string[] res = UnityStats.screenRes.Split('x');
            return new Vector2(float.Parse(res[0]), float.Parse(res[1]));
        }

        private static float GetScreenWidth()
        {
            return GetScreenSize().x;
        }

        private static float GetScreenHeight()
        {
            return GetScreenSize().y;
        }

        #endregion

    }
}

#endif