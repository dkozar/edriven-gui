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
using System.Collections.Generic;
using eDriven.Gui.Components;
using eDriven.Gui.Managers;

namespace eDriven.Gui
{
    /// <summary>
    /// Alert class
    /// </summary>
    public sealed class Alert
    {
        /// <summary>
        /// Optional default skin class
        /// </summary>
        public static Type DefaultSkin; // TODO: Do it with styling (implement skinClass as string in a picker)

        private static readonly List<Component> Popups = new List<Component>();

        /// <summary>
        /// Circular tabs for Alerts
        /// </summary>
        public static bool CircularTabs;

        /// <summary>
        /// Circular arrows for Alerts
        /// </summary>
        public static bool CircularArrows;

        /// <summary>
        /// Arrows enabled for Alerts
        /// </summary>
        public static bool ArrowsEnabled = true;

        #region Show / hide

        //public static void Show(string message)
        //{
        //    Show(message, string.Empty, null);
        //}

        /// <summary>
        /// Shows the alert
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public static void Show(string title, string message)
        {
            Show(title, message, null);
        }

        /// <summary>
        /// Shows the alert
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="callback"></param>
        public static void Show(string title, string message, Action<string> callback)
        {
            Show(title, message, callback, AlertButtonFlag.Ok | AlertButtonFlag.Cancel);
        }

        /// <summary>
        /// Shows the alert
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="flags"></param>
        public static void Show(string title, string message, AlertButtonFlag flags)
        {
            Show(title, message, null, flags);
        }

        /// <summary>
        /// Shows the alert
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="flags"></param>
        /// <param name="callback"></param>
        public static void Show(string title, string message, AlertButtonFlag flags, Action<string> callback)
        {
            Show(title, message, callback, flags);
        }

        /// <summary>
        /// Shows the alert
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="callback"></param>
        /// <param name="flags"></param>
        public static void Show(string title, string message, Action<string> callback, AlertButtonFlag flags)
        {
            //InitializeInstance();
            //Instance.Show();

            AlertInstance instance = new AlertInstance
                                         {
                                             Callback = callback,
                                             Title = title,
                                             Message = message,
                                             Flags = flags
                                         };

            //instance.Callback = callback;
            //instance.Message = message;
            //instance.Title = title;
            //instance.ProcessFlags(flags);

            PopupManager.Instance.AddPopup(instance);
            //PopupManager.Instance.CenterPopUp(instance);
            Popups.Add(instance);
            
            //_instance.SetFocus();

            //Instance.DoResize(SystemManager.Instance.ScreenSize);

            //Instance.ValidateNow();
        }

        /// <summary>
        /// Shows the alert
        /// </summary>
        /// <param name="options"></param>
        public static void Show(params AlertOption[] options)
        {
            Show(null, options);
        }

        /// <summary>
        /// Shows the alert
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="options"></param>
        public static void Show(Action<string> callback, params AlertOption[] options)
        {
            AlertInstance instance = new AlertInstance {Callback = callback};
            instance.ApplyOptions(options);
            PopupManager.Instance.AddPopup(instance, true);
            Popups.Add(instance);
        }

        //public static void Show(string message, Action<string> callback, params AlertOption[] options)
        //{
        //    AlertInstance instance = new AlertInstance { Message = message, Callback = callback };
        //    instance.ApplyOptions(options);
        //    PopupManager.Instance.AddPopup(instance, true);
        //    Popups.Add(instance);
        //}

        /// <summary>
        /// Shows the alert
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="flags"></param>
        /// <param name="options"></param>
        public static void Show(string title, string message, AlertButtonFlag flags, params AlertOption[] options)
        {
            AlertInstance instance = new AlertInstance { Message = message, Title = title, Flags = flags };
            instance.ApplyOptions(options);
            PopupManager.Instance.AddPopup(instance, true);
            Popups.Add(instance);
        }

        /// <summary>
        /// Shows the alert
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="callback"></param>
        /// <param name="options"></param>
        public static void Show(string title, string message, Action<string> callback, params AlertOption[] options)
        {
            AlertInstance instance = new AlertInstance { Message = message, Title = title, Callback = callback };
            instance.ApplyOptions(options);
            PopupManager.Instance.AddPopup(instance, true);
            Popups.Add(instance);
        }

        /// <summary>
        /// Shows the alert
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="flags"></param>
        /// <param name="callback"></param>
        /// <param name="options"></param>
        public static void Show(string title, string message, AlertButtonFlag flags, Action<string> callback, params AlertOption[] options)
        {
            AlertInstance instance = new AlertInstance { Message = message, Title = title, Flags = flags, Callback = callback };
            instance.ApplyOptions(options);
            PopupManager.Instance.AddPopup(instance, true);
            Popups.Add(instance);
        }

        /// <summary>
        /// Shows the alert
        /// </summary>
        public static void Hide()
        {
            //Component lastPopup = Popups[Popups.Count - 1];
            //if (lastPopup is AlertInstance)
            //    ((AlertInstance)lastPopup).Hide();

            //if (Popups.Count > 0)
            //    PopupManager.Instance.RemovePopup(Popups[Popups.Count - 1]);

            if (0 == Popups.Count)
                return;

            Component lastPopup = Popups[Popups.Count - 1];

            PopupManager.Instance.RemovePopup(lastPopup);
        }

        #endregion
    }
}