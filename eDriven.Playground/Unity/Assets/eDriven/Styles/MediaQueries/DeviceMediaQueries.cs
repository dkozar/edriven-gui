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

using eDriven.Gui.Styles;
using UnityEngine;

namespace Assets.eDriven.Styles.MediaQueries
{
    public static class DeviceMediaQueries
    {
        [MediaQuery("platform", "Runtime platform")]
        public static bool Device(RuntimePlatform platform)
        {
            return Application.platform == platform;
        }

        [MediaQuery("deviceOrientation", "Device orientation")]
        public static bool DeviceOrientation(DeviceOrientation deviceOrientation)
        {
            return Input.deviceOrientation == deviceOrientation;
        }

        /// <summary>
        /// Note: Overriden in DeviceMediaQueriesOverrides
        /// </summary>
        /// <param name="minDeviceWidth"></param>
        /// <returns></returns>
        [MediaQuery("minDeviceWidth", "Min device width in pixels")]
        public static bool MinDeviceWidth(int minDeviceWidth)
        {
            return Screen.width >= minDeviceWidth;
        }

        /// <summary>
        /// Note: Overriden in DeviceMediaQueriesOverrides
        /// </summary>
        /// <param name="minDeviceHeight"></param>
        /// <returns></returns>
        [MediaQuery("minDeviceHeight", "Min device height in pixels")]
        public static bool MinDeviceHeight(int minDeviceHeight)
        {
            return Screen.height >= minDeviceHeight;
        }

        /// <summary>
        /// Note: Overriden in DeviceMediaQueriesOverrides
        /// </summary>
        /// <param name="minDeviceWidth"></param>
        /// <returns></returns>
        [MediaQuery("maxDeviceWidth", "Max device width in pixels")]
        public static bool MaxDeviceWidth(int minDeviceWidth)
        {
            return Screen.width <= minDeviceWidth;
        }

        /// <summary>
        /// Note: Overriden in DeviceMediaQueriesOverrides
        /// </summary>
        /// <param name="minDeviceHeight"></param>
        /// <returns></returns>
        [MediaQuery("maxDeviceHeight", "Max device height in pixels")]
        public static bool MaxDeviceHeight(int minDeviceHeight)
        {
            return Screen.height <= minDeviceHeight;
        }
    }
}