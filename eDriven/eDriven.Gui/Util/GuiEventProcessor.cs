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
using eDriven.Core.Events;
using eDriven.Gui.Components;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Util
{
    /// <summary>
    /// Handles GUI events and event bubbling
    /// The way I do event bubbling:
    /// 
    /// 0. Finding the parent chain
    /// AFTER the target component is found (by other means), we walk up the DOM - following the Parent property. 
    /// This way we get a parent chain all to the Stage.
    /// Then we reverse the chain to get the top-down list (from Stage to leaf).
    /// 
    /// 1. Capture phase:
    /// For each component in the chain I call the internal-ish method "ExecuteListeners" on event dispatcher for each component in the chain.
    /// I continue to do this until event is canceled.
    /// If all the components in the chain are handled, we're done with the capture phase.
    /// 
    /// 2. Target phase
    /// We're calling the "ExecuteListeners" on target. If event not canceled, we continue to a bubbling phase.
    /// 
    /// 3. Bubbling phase
    /// We reverse the chain again to get the bottom-up list of components.
    /// We repeat the same process as for the capture phase, stopping if the event is canceled.
    /// </summary>
    internal static class GuiEventProcessor
    {
        /// <summary>
        /// Checks if the component has the listener or any of component's ancestors 
        /// have listeners for the supplied event type
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        internal static bool HasBubblingEventListener(string eventType, DisplayListMember target)
        {
            /**
             * 0) Get parent chain
             * */
            List<DisplayListMember> parentChain = ComponentUtil.GetParentChain(target, true);
            parentChain.Add(target); // add myself

            /**
             * 1) Look for at least one subscribed component
             * */
            foreach (DisplayListMember component in parentChain)
            {
                if (component.HasEventListener(eventType))
                    return true;
            }

            return false; // no subscribed components
        }

        /// <summary>
        /// Bubbles event through 3 phases
        /// <see cref="http://www.w3.org/TR/DOM-Level-2-Events/events.html#Events-flow-capture"/>
        /// </summary>
        /// <param name="e"></param>
        internal static void BubbleEvent(Event e)
        {
            if (e.Canceled)
                return;
            
            /**
             * 0) Get parent chain
             * */
            List<DisplayListMember> parentChain = ComponentUtil.GetParentChain((DisplayListMember)e.Target, true); // true for reversing
            
            /**
             * 1) Capture phase
             * */
            e.Phase = EventPhase.Capture;
            foreach (DisplayListMember d in parentChain)
            {
                // set current target
                e.CurrentTarget = d;

                // execute listeners on current target
                // any of the listeners my cancel the event, that's handled by ExecuteListeners method internally
                d.ExecuteListeners(e);

                // all the listeners (or part of the listeners, in the case of cancelation) 
                // on current target have been executed.
                // if event canceled or propagation stopped, go no further
                if (!e.Bubbles || e.Canceled)
                    return;
            }

            /**
             * 2) Target phase
             * */
            e.Phase = EventPhase.Target;
            e.CurrentTarget = e.Target;
            ((EventDispatcher)e.Target).ExecuteListeners(e); //, EventPhase.Target); // executing target listeners

            // if event canceled or propagation stopped, go no further
            if (!e.Bubbles || e.Canceled)
                return;

            /**
             * 3) Bubbling phase
             * */
            e.Phase = EventPhase.Bubbling;
            parentChain.Reverse(); // reversing the parent chain & starting to bubble

            foreach (DisplayListMember d in parentChain)
            {
                // set current target
                e.CurrentTarget = d;

                // execute listeners on current target
                // any of the listeners my cancel the event, that's handled by ExecuteListeners method internally
                d.ExecuteListeners(e);

                // all the listeners (or part of the listeners, in the case of cancelation) 
                // on current target have been executed.
                // if event canceled or propagation stopped, go no further
                if (!e.Bubbles || e.Canceled)
                    return;
            }

            /**
             * Cleanup
             * */
            if (e is IDisposable)
                ((IDisposable)e).Dispose();
        }
        
    }
}