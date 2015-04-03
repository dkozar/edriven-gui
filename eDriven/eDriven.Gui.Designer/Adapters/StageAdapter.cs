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
using System.Reflection;
using eDriven.Core.Events;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Reflection;
using UnityEngine;
using Component = eDriven.Gui.Components.Component;
using Event = eDriven.Core.Events.Event;
using EventHandler = eDriven.Core.Events.EventHandler;

namespace eDriven.Gui.Designer.Adapters
{
    /// <summary>
    /// The adapter for Stage component
    /// </summary>
    [Obfuscation(Exclude = true)]
    [Toolbox(Icon = "eDriven/Editor/Controls/stage", Label = "Create new Stage")]
    public class StageAdapter : GroupAdapter, IChildList, IEventDispatcher, IDisposable
    {

        /// <summary>
        /// Gui class inherits UnityEngine.MonoBehaviour
        /// Normally it should be extended and put into the hierarchy manually
        /// Alternatively it could be created dinamically
        /// Upon starting or enabling, it registers itself to the StageManager instance
        /// The Depth property is used for layering the multiple GUIs in the application
        /// Internally it creates the Stage instance
        /// It listens for the changes in the inspector and propagates them to the Stage instance
        /// </summary>
        /// <remarks>Author: Danko Kozar</remarks>

#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Properties

        /// <summary>
        /// Z-index for stage
        /// </summary>
        [Saveable]
        public int ZIndex;

        /// <summary>
        /// Matrix (not used)
        /// </summary>
        [Saveable]
        public Matrix4x4 Matrix;

        /// <summary>
        /// Exposes Stage instance to the script
        /// </summary>
        public Stage Stage;

        /// <summary>
        /// Constructor
        /// </summary>
        public StageAdapter()
        {
            //LayoutDescriptor = eDriven.Gui.Layout.LayoutDescriptor.VerticalTopLeft;
            Layout = LayoutEnum.Absolute;
            PaddingLeft = PaddingRight = PaddingTop = PaddingBottom = 10;
            //ClipContent = true;
        }

        #endregion

        #region Unity Messages

//    // ReSharper disable UnusedMember.Local
//    [Obfuscation(Exclude = true)]
//    void Start()
//    // ReSharper restore UnusedMember.Local
//    {
//        if (!enabled)
//            return; // do not register this stage
//#if DEBUG
//        if (DebugMode)
//            Debug.Log("Gui started.");
//#endif
//        //if (!Instantiated)
//        //    DoInstantiate();

//        Produce(!FactoryMode, true); // StageAdapter is ContainerAdapter, so we are walking down

//        // Stage could be null if it is a template
//        if (null != Stage)
//            Stage.Register();
//    }

// ReSharper disable UnusedMember.Local
        [Obfuscation(Exclude = true)]
        void OnEnable()
// ReSharper restore UnusedMember.Local
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("StageAdapter OnEnable: " + this); 
#endif

            if (!enabled)
                return; // do not register this stage

            // assembly reload happened?
            Produce(!FactoryMode, true); // StageAdapter is ContainerAdapter, so we are walking down

            // Stage could be null if it is a template
            if (null != Stage)
                Stage.Register();
        }

        /// <summary>
        /// Creates a new instance of Stage
        /// </summary>
        /// <returns></returns>
        public override Component NewInstance()
        {
            Stage = new Stage();
            return Stage;
        }

        public override void Apply(Component component)
        {
            base.Apply(component);

            Stage.ZIndex = ZIndex;
            Stage.Matrix = Matrix;
        }

        // ReSharper disable UnusedMember.Local
        [Obfuscation(Exclude = true)]
        protected override void OnEnableImpl()
            // ReSharper restore UnusedMember.Local
        {
            base.OnEnableImpl();

            if (null != Stage)
            {
                Stage.Enabled = true;
                Stage.Register();
            }
        }

        // ReSharper disable UnusedMember.Local
        [Obfuscation(Exclude = true)]
        void OnDisable()
            // ReSharper restore UnusedMember.Local
        {
            if (null != Stage)
            {
                Stage.Enabled = false;
                Stage.Unregister();
            }
        }

        #endregion

        #region New methods

        protected override void OnAwake()
        {
            // completely override setting hide flags by ComponentAdapter
        }

        #endregion

        #region IEventDispatcher

        /// <summary>
        /// Adds event listener
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="handler">Event handler (function)</param>
        public void AddEventListener(string eventType, EventHandler handler)
        {
            Stage.AddEventListener(eventType, handler);
        }

        /// <summary>
        /// Adds the event listener
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="handler">Event handler</param>
        /// <param name="priority">Event priority</param>
        public void AddEventListener(string eventType, EventHandler handler, int priority)
        {
            Stage.AddEventListener(eventType, handler, priority);
        }

        /// <summary>
        /// Adds the event listener
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="handler">Event handler</param>
        /// <param name="phases">Event phases</param>
        public void AddEventListener(string eventType, EventHandler handler, EventPhase phases)
        {
            Stage.AddEventListener(eventType, handler, phases);
        }

        /// <summary>
        /// Adds the event listener
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="handler">Event handler</param>
        /// <param name="phases">Event bubbling phases that we listen to</param>
        /// <param name="priority">Event priority</param>
        public void AddEventListener(string eventType, EventHandler handler, EventPhase phases, int priority)
        {
            Stage.AddEventListener(eventType, handler, phases, priority);
        }

        /// <summary>
        /// Removes event listener
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="handler">Event handler (function)</param>
        public void RemoveEventListener(string eventType, EventHandler handler)
        {
            Stage.RemoveEventListener(eventType, handler);
        }

        /// <summary>
        /// Removes event listener
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        /// <param name="phases"></param>
        public void RemoveEventListener(string eventType, EventHandler handler, EventPhase phases)
        {
            Stage.RemoveEventListener(eventType, handler, phases);
        }

        /// <summary>
        /// Removes all event listeners
        /// </summary>
        /// <param name="eventType"></param>
        public void RemoveAllListeners(string eventType)
        {
            Stage.RemoveAllListeners(eventType);
        }

        /// <summary>
        /// Checks whether the EventDispatcher object has any listeners registered for a specific type of event. 
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public bool HasEventListener(string eventType)
        {
            return Stage.HasEventListener(eventType);
        }

        /// <summary>
        /// Checks whether an event listener is registered with this EventDispatcher object or any of its ancestors for the specified event type. 
        /// This method returns true if an event listener is triggered during any phase of the event flow when an event of the specified type is dispatched to this EventDispatcher object or any of its descendants.
        /// The difference between HasEventListener() and HasBubblingEventListener() is that HasEventListener() examines only the object to which it belongs, whereas HasBubblingEventListener() examines the entire event flow for the event specified by the type parameter.
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public bool HasBubblingEventListener(string eventType)
        {
            return Stage.HasBubblingEventListener(eventType);
        }

        /// <summary>
        /// Dispatches an event
        /// </summary>
        /// <param name="e"></param>
        public void DispatchEvent(Event e)
        {
            Stage.DispatchEvent(e);
        }

        /// <summary>
        /// Dispatches an event
        /// </summary>
        /// <param name="e"></param>
        /// <param name="immediate"></param>
        public void DispatchEvent(Event e, bool immediate)
        {
            Stage.DispatchEvent(e, immediate);
        }

        #endregion

        #region IChildList

        /// <summary>
        /// The child components of the container
        /// </summary>
        public List<DisplayListMember> Children
        {
            get { return Stage.Children; }
        }

        /// <summary>
        /// Number of children
        /// </summary>
        public int NumberOfChildren
        {
            get { return Stage.NumberOfChildren; }
        }

        /// <summary>
        /// Checks if the component is a child of this stage
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public bool HasChild(DisplayListMember child)
        {
            return Stage.HasChild(child);
        }

        /// <summary>
        /// Checks if the stage contains child component including checking the stage
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public bool Contains(DisplayListMember child)
        {
            return Stage.Contains(child);
        }

        /// <summary>
        /// Checks if the stage contains child component
        /// </summary>
        /// <param name="child"></param>
        /// <param name="exclusive"></param>
        /// <returns></returns>
        public bool Contains(DisplayListMember child, bool exclusive)
        {
            return Stage.Contains(child, exclusive);
        }

        /// <summary>
        /// Adds a child to stage
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public DisplayListMember AddChild(DisplayListMember child)
        {
            return Stage.AddChild(child);
        }

        /// <summary>
        /// Adds a child to the container to the specified index
        /// </summary>
        /// <param name="child">A child</param>
        /// <param name="index">Index</param>
        public DisplayListMember AddChildAt(DisplayListMember child, int index)
        {
            return Stage.AddChildAt(child, index);
        }

        /// <summary>
        /// Removes a child from stage
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public DisplayListMember RemoveChild(DisplayListMember child)
        {
            return Stage.RemoveChild(child);
        }

        /// <summary>
        /// Adds a child from the container at specified index
        /// </summary>
        public DisplayListMember RemoveChildAt(int index)
        {
            return Stage.RemoveChildAt(index);
        }

        /*/// <summary>
        /// Removes all stage children
        /// </summary>
        public void RemoveAllChildren()
        {
            Stage.RemoveAllChildren();
        }*/

        ///<summary>
        /// Swaps two children
        ///</summary>
        ///<param name="firstChild">First child</param>
        ///<param name="secondChild">Second child</param>
        public void SwapChildren(DisplayListMember firstChild, DisplayListMember secondChild)
        {
            Stage.SwapChildren(firstChild, secondChild);
        }

        /// <summary>
        /// Gets child at specified position
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Child index</returns>
        public DisplayListMember GetChildAt(int index)
        {
            return Stage.GetChildAt(index);
        }

        /// <summary>
        /// Gets child index
        /// </summary>
        /// <param name="child">A child</param>
        /// <returns>The position</returns>
        public int GetChildIndex(DisplayListMember child)
        {
            return Stage.GetChildIndex(child);
        }

        ///<summary>
        /// Sets child index
        ///</summary>
        ///<param name="child"></param>
        ///<param name="index"></param>
        public void SetChildIndex(DisplayListMember child, int index)
        {
            Stage.SetChildIndex(child, index);
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
            if (null != Stage)
                Stage.Unregister();
        }

        #endregion
    }
}