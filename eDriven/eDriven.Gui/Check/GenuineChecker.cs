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

using eDriven.Core.Managers;
using eDriven.Gui;
using eDriven.Gui.Managers;
using UnityEngine;
using Random=System.Random;

namespace eDriven.Gui.Check
{
    internal static class GenuineChecker
    {
        /// <summary>
        /// For testing
        /// </summary>
        public static bool ForceUrlCheck;

        /// <summary>
        /// Checking piracy in debug mode
        /// Use the release mode for clients
        /// </summary>
        public static void Check()
        {
            bool isPirated = false;
            //bool isNotGenuine = false;
            //if (Application.genuineCheckAvailable)
            //    isNotGenuine = !Application.genuine;

#if TRIAL
            if (ForceUrlCheck || 
                (/*Application.isWebPlayer && */Application.internetReachability != NetworkReachability.NotReachable))
            {
                // TODO: do a smart check for those values:

                string url = Application.absoluteURL;
                
                isPirated = !url.StartsWith("http://edrivenunity.com") &&
                            !url.StartsWith("http://edrivengui.com") &&
                            !url.StartsWith("http://dankokozar.com") &&
                            !url.StartsWith("http://edriven.dankokozar.com");

                //string url = Application.absoluteURL;
                //uri = new Uri(url);
                //Core.Util.Logger.Log("uri.Host: " + uri.Host);
            }
#endif
            //isPirated = true;

            //if (isNotGenuine)
            //{
            //    Debug.Log("eDriven check: The application is not genuine. Somebody messed with it after it has been built.");
            //}

            if (isPirated)
            {
                const string text = @"Using the unauthorized version of eDriven.Gui.
Please purchase the package and support the further development!";
                Debug.Log(text);
                Random random = new Random();
                DeferManager.Instance.Defer(delegate
                {
                    eDriven.Gui.Alert.Show(
                        delegate (string action) {
                            SystemManager.Instance.Enabled = false;
                        },
                        new AlertOption(AlertOptionType.Title, ":-("),
                        new AlertOption(AlertOptionType.Message, text),
                        new AlertOption(AlertOptionType.Button, new AlertButtonDescriptor("ok", "OK, I will.", true))
                    );

                }, random.Next(100, 1000));
            }
        }
    }
}