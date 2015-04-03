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

using eDriven.Animation;
using eDriven.Animation.Plugins;
using eDriven.Gui.Components;

namespace eDriven.Gui.Animation
{
    public class HideOnStartPlugin : ITweenFactoryPlugin
    {
        public void Initialize(object target)
        {
            TweenFactory factory = (TweenFactory) target;
            factory.StartSignal.Connect(StartSlot);
            factory.StopSignal.Connect(StopSlot);
        }

        private static void StartSlot(object[] parameters)
        {
            Component component = parameters[0] as Component;

            if (null == component)
                return;

            component.Visible = false;
        }

        private static void StopSlot(object[] parameters)
        {
            Component component = parameters[0] as Component;

            if (null == component)
                return;

            component.Visible = true;
        }
    }
}