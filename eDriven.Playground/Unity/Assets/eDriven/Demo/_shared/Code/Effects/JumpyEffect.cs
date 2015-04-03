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

using System.Reflection;
using eDriven.Animation;
using eDriven.Core.Events;
using eDriven.Gui.Designer;
using Assets.eDriven.Demo.Tweens;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;
using Event=eDriven.Core.Events.Event;

[AddComponentMenu("eDriven/Gui/Effects/JumpyEffect")]
[Effect]
[Obfuscation(Exclude = true)]

/// <summary>
/// Jumpy effect. Attach this script to any eDriven.Gui component
/// </summary>
public class JumpyEffect : MonoBehaviour
{
    // a static factory for lounching tweens
    private static readonly TweenFactory Factory = new TweenFactory(new Jumpy());

    // the event type (set it from editor)
    public string Trigger = MouseEvent.ROLL_OVER; // default

    // fires after the GUI component is instantiated
    void ComponentInstantiated(Component component)
    {
        //Slider slider = (Slider) component; // you can do a cast here
        component.AddEventListener(Trigger, delegate(Event e)
        {
            Factory.Play(e.Target);
        }, EventPhase.Target);
    }
}