/*
 
eDriven.Gui
Copyright (c) 2010-2014 Danko Kozar
 
*/

using System.Collections.Generic;
using eDriven.Core.Events;
using eDriven.Gui.Components;
using eDriven.Gui.Managers;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Plugins
{
    public class TabManager : IPlugin
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        private Component _component;

        /// <summary>
        /// Focusable components (currently only the direct children of this component)
        /// </summary>
        private List<DisplayListMember> _components;

        private bool _arrowsEnabled;
        /// <summary>
        /// Should the focus be changed with arrow keys
        /// </summary>
        public bool ArrowsEnabled
        {
            get { return _arrowsEnabled; }
            set { _arrowsEnabled = value; }
        }

        private bool _upDownArrowsEnabled;
        /// <summary>
        /// Should the focus be changed with arrow keys
        /// </summary>
        public bool UpDownArrowsEnabled
        {
            get { return _upDownArrowsEnabled; }
            set { _upDownArrowsEnabled = value; }
        }

        private bool? _circularTabs;
        /// <summary>
        /// When using the TAB key, should we get to the first item after the last one
        /// </summary>
        public bool? CircularTabs
        {
            get { return _circularTabs; }
            set { _circularTabs = value; }
        }

        private bool? _circularArrows;
        /// <summary>
        /// When using the arrow keys, should we get to the first item after the last one
        /// </summary>
        public bool? CircularArrows
        {
            get { return _circularArrows; }
            set { _circularArrows = value; }
        }

        /// <summary>
        /// We are capturing key events in capture phase
        /// </summary>
        private const EventPhase Phases = EventPhase.Capture | EventPhase.Target;

        private bool _initialized;

        /// <summary>
        /// Initializes the plugin
        /// </summary>
        /// <param name="component"></param>
        public void Initialize(InvalidationManagerClient component)
        {
            if (_initialized)
                return;

            _initialized = true;

            //Debug.Log("TabManager init");
            _component = (Component) component;

            ITabManagerClient tabManagerClient = component as ITabManagerClient;
            if (null == tabManagerClient)
                return;

            _components = TabChildren ?? tabManagerClient.GetTabChildren(); // gets all focusable children

            if (null == _circularTabs) // if not set on plugin
                _circularTabs = tabManagerClient.CircularTabs;

            if (null == _circularArrows) // if not set on plugin
                _circularArrows = tabManagerClient.CircularArrows;
            
            if (_components.Count > 0)
                _component.AddEventListener(KeyboardEvent.KEY_DOWN, KeyDownHandler, Phases); // subscribe to keys
        }

        public void Dispose()
        {
            _component.RemoveEventListener(KeyboardEvent.KEY_DOWN, KeyDownHandler, Phases); // cleanup
        }

        private void KeyDownHandler(Event e)
        {
            KeyboardEvent ke = (KeyboardEvent)e;
            
            int index = _components.IndexOf(FocusManager.Instance.FocusedComponent);
            if (-1 == index)
                return;

            bool keyRecognized = false;

            switch (ke.KeyCode)
            {
                case KeyCode.Tab:
                    //Debug.Log("Tab");
                    keyRecognized = true;
                    index = ke.Shift ? Previous(index, _circularTabs ?? false) : Next(index, _circularTabs ?? false);
                    break;

                case KeyCode.LeftArrow:
                    //Debug.Log("LeftArrow");
                    if (_arrowsEnabled) {
                        keyRecognized = true;
                        index = Previous(index, _circularTabs ?? false);
                    }
                    break;

                case KeyCode.RightArrow:
                    //Debug.Log("CircularArrows: " + CircularArrows);
                    if (_arrowsEnabled) {
                        keyRecognized = true;
                        index = Next(index, _circularTabs ?? false);
                    }
                    break;

                case KeyCode.UpArrow:
                    //Debug.Log("LeftArrow");
                    if (_arrowsEnabled && _upDownArrowsEnabled) {
                        keyRecognized = true;
                        index = Previous(index, _circularTabs ?? false);
                    }
                    break;

                case KeyCode.DownArrow:
                    //Debug.Log("CircularArrows: " + CircularArrows);
                    if (_arrowsEnabled && _upDownArrowsEnabled) {
                        keyRecognized = true;
                        index = Next(index, _circularTabs ?? false);
                    }
                    break;
                default:
                    break;
                
            }

            if (keyRecognized)
            {
                /**
                 * Important: canceling the event alters with behavours of container children !!!
                 * (for instance, TabManager set on Form swallows the key events needed by combo box)
                 * */
                e.CancelAndStopPropagation();

                InteractiveComponent componentToFocus = _components[index] as InteractiveComponent;
                if (null != componentToFocus) {

                    if (!componentToFocus.FocusEnabled)
                    {
#if DEBUG
                        if (DebugMode)
                        {
                            Debug.Log("Component is not focus enabled: " + componentToFocus);
                            return;
                        }
#endif
                    }

#if DEBUG
                    if (DebugMode)
                    {
                        Debug.Log("Focusing component: " + componentToFocus);
                    }
#endif
                    FocusManager.Instance.TabbedToFocus = true;
                    componentToFocus.SetFocus();
                }
                else
                {
#if DEBUG
                    if (DebugMode)
                    {
                        Debug.LogWarning("Component to focus not instantiated");
                    }
#endif
                }
            }
        }

        private int Previous(int index, bool circular)
        {
            var comps = GetFocusableComponents();
            if (comps.Count <= 1)
                return index; // no change

            if (index > 0)
                index--;
            else if (circular)
                index = _components.Count - 1;
            return index;
        }

        private int Next(int index, bool circular)
        {
            var comps = GetFocusableComponents();
            if (comps.Count <= 1)
                return index; // no change

            if (index < _components.Count - 1)
                index++;
            else if (circular)
                index = 0;
            return index;
        }

        // TODO: handle skipping non-focused components and circular changes
        private List<FocusableComponentDescriptor> GetFocusableComponents()
        {
            List<FocusableComponentDescriptor> list = new List<FocusableComponentDescriptor>();

            ITabManagerClient fmc = _component as ITabManagerClient;
            if (null == fmc)
                return list;

            _components = TabChildren ?? fmc.GetTabChildren();

            int index = 0;
            _components.ForEach(delegate(DisplayListMember child)
            {
                InteractiveComponent comp = child as InteractiveComponent; // form item
                if (FocusManager.IsFocusCandidate(comp)) // visible & enabled & focus enabled?
                    list.Add(new FocusableComponentDescriptor(index, comp));
                index++;
            });

            return list;
        }

        /// <summary>
        /// Tab children (could be set from outside)
        /// </summary>
        public List<DisplayListMember> TabChildren { get; set; }
    }

    internal class FocusableComponentDescriptor
    {
        public int Index;
        public DisplayListMember Component;

        public FocusableComponentDescriptor(int index, DisplayListMember component)
        {
            Index = index;
            Component = component;
        }
    }
}
