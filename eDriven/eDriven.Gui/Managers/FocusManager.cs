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

using eDriven.Core;
using eDriven.Core.Events;
using eDriven.Gui.Check;
using eDriven.Gui.Components;
using eDriven.Gui.Util;
using UnityEngine;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Managers
{
    /// <summary>
    /// Singleton class for handling focus
    /// Coded by Danko Kozar
    /// </summary>
    public class FocusManager
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static FocusManager _instance;

        private FocusManager()
        {
            // Constructor is protected!
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static FocusManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FocusManager();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// Should the framework correct improper Unity focus?
        /// </summary>
        public static bool AutoCorrectUnityFocus = true;

        /// <summary>
        /// Initialization routine
        /// Put inside the initialization stuff, if needed
        /// </summary>
        private void Initialize()
        {
            MouseEventDispatcher.Instance.AddEventListener(MouseEvent.MOUSE_DOWN, ComponentMouseDownHandler);

#if TRIAL
            /* HACK CHECK */
            Acme acme = (Acme) Framework.GetComponent<Acme>(true);
            if (null == acme || !acme.gameObject.activeInHierarchy/*active*/ || !acme.enabled)
                return;
#endif
        }

        /// <summary>
        /// Fires when component clicked with a mouse
        /// The clicked component is obviously mouse enabled
        /// But not all the clickable components are focusable
        /// Thus we have to climb up the parent tree to find the first focusable component, or focus routing
        /// If such component exists, we should set focus on it or run the routing to other component
        /// </summary>
        /// <param name="e"></param>
        private void ComponentMouseDownHandler(Event e)
        {
#if DEBUG
            if (DebugMode)
                Debug.Log("FocusManager: ComponentMouseDownHandler: " + e.Target);
#endif
            //Debug.Log("FocusManager: ComponentMouseDownHandler: " + e.Target);
            //((InteractiveComponent)e.Target).SetFocus(); // breaks combobox!
            //return;

            InteractiveComponent component = FindFocusableAncestor((InteractiveComponent)e.Target);

            if (null == component)
                return;

            if (null != component.FocusRouting)
            {
                component.FocusRouting();
                return;
            }

            if (component == _focusedComponent)
                return;

            // blur the old one
            //if (null != _focusedComponent) // commented 20130331 (redundant ?)
            //{
            //    Blur(_focusedComponent);
            //}

            //Debug.Log("Setting focus on component: " + component);
            //SetFocus(component);
            component.SetFocus(); // give the component a chance to handle its focus (pass to child etc.)
        }

        /// <summary>
        /// Focused component UID
        /// </summary>
        public string FocusedComponentUid;

        private InteractiveComponent _focusedComponent;
        /// <summary>
        /// Focused component
        /// </summary>
        public InteractiveComponent FocusedComponent
        {
            get { return _focusedComponent; }
        }

        /// <summary>
        /// The flag indicating the component has been focused with tabbing<br/>
        /// This flag shoud be set prior to SetFocus call<br/>
        /// When setting the focus, the system will react depending of this flag, and then reset the flag back to false
        /// </summary>
        public bool TabbedToFocus;

        /// <summary>
        /// Is any of the components in focus
        /// </summary>
        public bool IsFocused
        {
            get
            {
                return null != _focusedComponent;
            }
        }

        /// <summary>
        /// Checks if the specified component is in focus
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool HasFocusedComponent(InteractiveComponent c)
        {
            return _focusedComponent == c;
        }

        /// <summary>
        /// Checks if the component having the specified ID is in focus
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool HasFocusedComponent(string id)
        {
            return null != _focusedComponent && _focusedComponent.Id == id;
        }

        /// <summary>
        /// Returns true if one of the specified component is in focus
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool HasFocusedComponents(string[] list)
        {
            foreach (var id in list)
            {
                if (HasFocusedComponent(id))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// The component to hightlight
        /// </summary>
        public InteractiveComponent ComponentToHighlight
        {
            get
            {
                if (IsFocused && _focusedComponent.HighlightOnFocus)
                    return _focusedComponent;

                return null;
            }
        }

        /// <summary>
        /// Sets focus on a component
        /// </summary>
        /// <param name="component"></param>
        public void SetFocus(InteractiveComponent component)
        {
            DoSetFocus(component);

            /*if (component.Initialized)
            {
                //FocusManager.Instance.TabbedToFocus = true;
                //Debug.Log("Setting focus: " + this);
                DoSetFocus(component);
            }
            else
            {
                component.AddEventListener(FrameworkEvent.CREATION_COMPLETE, SetFocusDelayed);
            }*/
        }

        private void SetFocusDelayed(Event e)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format("InteractiveComponent: SetFocusDelayed [{0}]", this));
            }
#endif

            var component = (InteractiveComponent)e.Target;
            component.RemoveEventListener(FrameworkEvent.CREATION_COMPLETE, SetFocusDelayed);
            //FocusManager.Instance.TabbedToFocus = true;
            //Instance.SetFocus(component);
            DoSetFocus(component);
        }
        
        /// <summary>
        /// Sets focus to the component
        /// Blurs previously focused component
        /// Called internaly by InteractiveComponent
        /// </summary>
        /// <param name="component">Component to focus</param>
        private void DoSetFocus(InteractiveComponent component)
        {
            //Debug.Log("DoSetFocus: " + component);

            /**
             * Clicking everywhere outside the text field messes the editor (cursor)
             * However, if the host dialog clicked, and it's SetFocus() routes to the same text field, 
             * the focus should stay intact (and the cursor working)
             * That's why we need to give TextFieldFocusHelper a chance to re-apply the focus
             * (it does it at the end of OnGUI)
             * */
            if (component is TextFieldBase)
                TextFieldFocusHelper.ShouldHandleFocus = true;

            /**
             * 1) If component not focus enabled, go no further
             * */
            if (!component.FocusEnabled)
            {
#if DEBUG
                if (DebugMode)
                    Debug.Log(string.Format("FocusManager: Focus not set to [{0}] because component not FocusEnabled", component));
#endif
                return; // else do nothing
            }

#if DEBUG
            if (DebugMode)
                Debug.Log("FocusManager: Trying to set focus to: " + component);
#endif
            //Debug.Log("_focusedComponent == component: " + (_focusedComponent == component));
            //Debug.Log("component.FocusEnabled: " + (component.FocusEnabled));

            /**
             * 2) If focusing the already focused component, do nothing
             * */
            if (_focusedComponent == component)
            {
#if DEBUG
                if (DebugMode)
                    Debug.Log(string.Format("FocusManager: Focus not set to [{0}] because it is the same component", component));
#endif
                return;
            }

            /**
             * 3) Blur previuosly focused component
             * */
            Blur();

            //Debug.Log("component: " + component);

            /**
             * 4) Set current component as focused component
             * */
            _focusedComponent = component;

#if DEBUG
            if (DebugMode)
                Debug.Log("FocusManager: Focus changed to: " + _focusedComponent);
#endif

            //Debug.Log("FocusManager: Focus changed to: " + _focusedComponent);
            //GUI.FocusControl(component.Uid);
            //if (0 != _focusedComponent.HotControlId)
            //{
            //    if (_focusedComponent is TextField)
            //    {
            //        GUIUtility.keyboardControl = _focusedComponent.HotControlId;
            //        Debug.Log("GUIUtility.keyboardControl: " + GUIUtility.keyboardControl);
            //    }
            //}

            /**
            / * 5) Execute focus on the component
            / * */
            component.FocusInHandler(new FocusEvent(FocusEvent.FOCUS_IN));

            //if (_focusedComponent is TextField)
            //{
            //    Debug.Log("Calling GUI.FocusControl: " + this);
            //    GUI.FocusControl(_focusedComponent.Uid);
            //    //TextEditor t = GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl) as TextEditor;
            //    //if (null != t)
            //    //    t.SelectAll();
            //}
        }

        /// <summary>
        /// Finds focusable parent or parent's parent
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        internal static InteractiveComponent FindFocusableAncestor(InteractiveComponent component)
        {
            //Debug.Log("FindFocusableAncestor: " + component);

            InteractiveComponent ancestor = null;

            DisplayObjectContainer child = component;

            do
            {
                var ic = child as InteractiveComponent;
                if (null != ic)
                {
                    if (ic.Enabled && ic.FocusEnabled || null != ic.FocusRouting)
                    {
                        ancestor = ic;
                        break;
                    }
                }
                //child = (InteractiveComponent) child.Parent;
                child = child.Parent;
            } 
            while (null != child);

#if DEBUG
if (DebugMode)
            {
                Debug.Log(null != ancestor
                              ? string.Format("Focusable ancestor found: {0}", ancestor)
                              : "Focusable ancestor NOT found");
            }
#endif

            //Debug.Log("ancestor: " + ancestor);
            return ancestor;
        }

        /// <summary>
        /// Blurs everything
        /// </summary>
        public void Blur()
        {
            //Debug.Log("Blur: " + _focusedComponent);
            //if (null != _focusedComponent)
            //    _focusedComponent.DispatchEvent(new GuiEvent(GuiEvent.FOCUS_OUT));

            //TextFieldFocusHelper.BlurUnityFocus();

            if (null != _focusedComponent)
                _focusedComponent.FocusOutHandler(new FocusEvent(FocusEvent.FOCUS_IN));

            _focusedComponent = null;
        }

        /// <summary>
        /// Blurs the referenced component but only if currently in focus
        /// Use this if you really know what you are doing; else you get en exception
        /// </summary>
        /// <param name="component"></param>
        public void Blur(InteractiveComponent component)
        {
            if (_focusedComponent == component)
                Blur();
            else
                throw new FocusManagerException(FocusManagerException.ComponentNotInFocus);
        }

        /// <summary>
        /// Gets the info if this is a focus candidate (for tabbing etc.)
        /// </summary>
        /// <param name="comp"></param>
        /// <returns></returns>
        public static bool IsFocusCandidate(InteractiveComponent comp)
        {
            return (null != comp && comp.Enabled && comp.Visible && comp.FocusEnabled);
        }

    }
}