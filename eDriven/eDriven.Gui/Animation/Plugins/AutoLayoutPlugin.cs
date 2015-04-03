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
using eDriven.Animation;
using eDriven.Animation.Plugins;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;

namespace eDriven.Gui.Animation
{
    public class AutoLayoutPlugin : ITweenFactoryPlugin
    {
#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

        private readonly Dictionary<Group, int> _dict = new Dictionary<Group, int>();

        public void Initialize(object target)
        {
            TweenFactory factory = (TweenFactory) target;
            factory.StartSignal.Connect(StartSlot);
            factory.StopSignal.Connect(StopSlot);
            factory.StopAllSignal.Connect(StopAllSlot);
        }

        private void StartSlot(object[] parameters)
        {
            Component component = parameters[0] as Component;

            if (null == component)
                return;

            Group container = component.Parent as Group;
            if (null == container)
                return;

            if (!_dict.ContainsKey(container)) {
                _dict[container] = 0;
#if DEBUG
                //Debug.Log("container.AutoLayout: " + container.AutoLayout);

                if (DebugMode)
                {
                    Debug.Log(string.Format(@"Setting AutoLayout to {0} on {1}", container.AutoLayout, container));
                }
#endif
                container.AutoLayout = false; // turn AutoLayout OFF
                container.ForceLayout = true;
            }

            _dict[container]++;
        }

        private void StopSlot(object[] parameters)
        {
            Component component = parameters[0] as Component;

            if (null == component)
                return;

            Group container = component.Parent as Group;
            if (null == container)
                return;

            if (_dict.ContainsKey(container))
                _dict[container] --;

            if (_dict[container] <= 0) {
                _dict.Remove(container);
#if DEBUG
                if (DebugMode)
                {
                    Debug.Log(string.Format(@"Setting AutoLayout to true on " + container));
                }
#endif
                container.AutoLayout = true; // turn AutoLayout ON
                //container.ForceLayout = true;
                //container.AutoLayout = false; // turn AutoLayout ON
            }
        }

        private void StopAllSlot(object[] parameters)
        {
            foreach (Group container in _dict.Keys)
            {
                container.AutoLayout = true; // turn AutoLayout ON
            }
            _dict.Clear();
        }
    }
}