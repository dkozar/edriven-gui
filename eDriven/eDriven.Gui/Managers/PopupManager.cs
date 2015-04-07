using System;
using System.Collections.Generic;
using eDriven.Animation;
using eDriven.Core;
using eDriven.Core.Events;
using eDriven.Core.Managers;
using eDriven.Gui.Check;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Stages;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;
using Event=eDriven.Core.Events.Event;
using MulticastDelegate=eDriven.Core.Events.MulticastDelegate;
using Point=eDriven.Core.Geom.Point;
using Rectangle=eDriven.Core.Geom.Rectangle;

namespace eDriven.Gui.Managers
{
    /// <summary>
    /// A class responsible for adding, removing and centering popups to the popup stage (system stage)
    /// </summary>
    public class PopupManager : EventDispatcher
    {

#if DEBUG
// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBePrivate.Global
        public new static bool DebugMode;
// ReSharper restore MemberCanBePrivate.Global
// ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static PopupManager _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private PopupManager()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static PopupManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PopupManager();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        #region Slots

        /// <summary>
        /// A slot for receiving the stage resize signal
        /// When a resize received, we go through each single popup and do a removal or centering
        /// </summary>
        /// <param name="parameters"></param>
        public void ResizeSlot(params object[] parameters)
        {
            Point p = (Point)parameters[0];
            Rectangle r = Rectangle.FromSize(p);
            var descriptors = Descriptors;
            foreach (Component popup in Descriptors.Keys)
            {
                // get descriptor
                var descriptor = descriptors[popup];

                // test for remove on screen resize

                if (descriptor.RemoveOnScreenResize)
                {
                    RemovePopup(popup);
                    return;
                }

                if (null != descriptor.Overlay)
                {
                    //descriptor.Overlay.Bounds = r; //Stage.Bounds;
                    descriptor.Overlay.X = r.X;
                    descriptor.Overlay.Y = r.Y;
                    descriptor.Overlay.Width = r.Width;
                    descriptor.Overlay.Height = r.Height;
                }

                // resizing overlay
                //if (null != descriptor.Overlay)
                //{
                //    var bounds = (null != descriptor.Owner) ? descriptor.Owner.Bounds : PopupManager.Stage.Bounds;

                //    //Debug.Log("Bounding overlay: " + bounds);
                //    //descriptor.Overlay.SetBounds(popup.Owner.Bounds);
                //    descriptor.Overlay.Bounds = (Rectangle) bounds.Clone();
                //}

                // center popup?
                if (descriptor.KeepCenter)
                {
                    CenterPopUp(popup, true);
                }
            }
        }

        /// <summary>
        /// A slot for receiving the stage resize signal
        /// </summary>
        /// <param name="parameters"></param>
        public void MouseDownSlot(params object[] parameters)
        {
            //Debug.Log("PopupManagerMouseDownSlot: receive");

            if (Popups.Count == 0)
                return;

            var lastPopup = Popups[Popups.Count - 1];

            //Debug.Log("Popups.Count: " + Popups.Count);

            Point p = (Point)parameters[1];

            //var container = lastPopup as Container;
            var container = lastPopup as Group;

            bool found = false;
            if (null != container) // I am a Container
            {
                //if (container.QClipContent/* || container.QScrollContent*/) // clipped, go for optimized version
                //{
                //    if (!lastPopup.Transform.GlobalBounds.Contains(p)) // check global bounds only
                //    {
                //        found = true;
                //    }
                //}
                //else // non-clipped
                //{
                    if (!lastPopup.ContainsPoint(p, true)) // check children recursivelly
                        found = true;
                //}
            }

            else // I am a DisplayObjectContainer
            {
                if (!lastPopup.ContainsPoint(p, true)) // go for recursion, no optimization for a DisplayObjectContainer
                    found = true;
            }

            //Debug.Log("found: " + found);

            if (found)
                MouseDownOutsideHandler(lastPopup, p);
        }

        /// <summary>
        /// A slot for receiving the mouse wheel signal
        /// </summary>
        /// <param name="parameters"></param>
        public void MouseWheelSlot(params object[] parameters)
        {
            if (Popups.Count == 0)
                return;

            var lastPopup = Popups[Popups.Count - 1];

            Point p = (Point)parameters[1];

            var container = lastPopup as Group;

            if (null != container) // I am a Container
            {
                //if (container.QClipContent/* || container.QScrollContent*/) // clipped, go for optimized version
                //{
                //    if (!lastPopup.Transform.GlobalBounds.Contains(p)) // check global bounds only
                //    {
                //        MouseWheelOutsideHandler(lastPopup, p);
                //    }
                //}
                //else // non-clipped
                //{
                    if (!lastPopup.ContainsPoint(p, true)) // check children recursivelly
                        MouseWheelOutsideHandler(lastPopup, p);
                //}
            }

            else // I am a DisplayObjectContainer
            {
                if (!lastPopup.ContainsPoint(p, true)) // go for recursion, no optimization for a DisplayObjectContainer
                    MouseWheelOutsideHandler(lastPopup, p);
            }
        }

        ///// <summary>
        ///// A slot for receiving the click signal
        ///// </summary>
        ///// <param name="parameters"></param>
        //public void ClickSlot(params object[] parameters)
        //{
        //    Point p = (Point)parameters[0];
        //    Rectangle r = Rectangle.FromSize(p);
        //    var descriptors = Descriptors;
        //    foreach (Component popup in Descriptors.Keys)
        //    {
        //        // get descriptor
        //        var descriptor = descriptors[popup];

        //        //if (null != descriptor.PopupRoot)
        //        //{
        //        //    descriptor.PopupRoot.Bounds = r; //Stage.Bounds;
        //        //}

        //        // resizing overlay
        //        if (null != descriptor.Overlay)
        //        {
        //            //Debug.Log("Bounding overlay: " + popup.Parent.Bounds);
        //            //descriptor.Overlay.SetBounds(popup.Owner.Bounds);
        //            descriptor.Overlay.Bounds = r;
        //            descriptor.Overlay.Width = r.X;
        //            descriptor.Overlay.Height = r.Y;
        //            ((InvalidationManagerClient)descriptor.Overlay).ValidateNow();
        //        }

        //        // center popup?
        //        if (descriptor.KeepCenter)
        //        {
        //            CenterPopUp(popup, true);
        //        }
        //    }
        //}

        #endregion

        #region Events

// ReSharper disable InconsistentNaming
#pragma warning disable 1591
        public const string OPENING = "opening";
        public const string OPEN = "open";
        public const string CLOSING = "closing";
        public const string CLOSE = "close";
        public const string CLOSE_ALL = "closeAll";
#pragma warning restore 1591
// ReSharper restore InconsistentNaming

        private MulticastDelegate _opening;
        public MulticastDelegate Opening
        {
            get
            {
                if (null == _opening)
                    _opening = new MulticastDelegate(this, OPENING);
                return _opening;
            }
            set
            {
                _opening = value;
            }
        }
        
        private MulticastDelegate _open;
        public MulticastDelegate Open
        {
            get
            {
                if (null == _open)
                    _open = new MulticastDelegate(this, OPEN);
                return _open;
            }
            set
            {
                _open = value;
            }
        }

        private MulticastDelegate _closing;
        public MulticastDelegate Closing
        {
            get
            {
                if (null == _closing)
                    _closing = new MulticastDelegate(this, CLOSING);
                return _closing;
            }
            set
            {
                _closing = value;
            }
        }

        private MulticastDelegate _close;
        public MulticastDelegate Close
        {
            get
            {
                if (null == _close)
                    _close = new MulticastDelegate(this, CLOSE);
                return _close;
            }
            set
            {
                _close = value;
            }
        }

        private MulticastDelegate _closeAll;
        public MulticastDelegate CloseAll
        {
            get
            {
                if (null == _closeAll)
                    _closeAll = new MulticastDelegate(this, CLOSE_ALL);
                return _closeAll;
            }
            set
            {
                _closeAll = value;
            }
        }
        
        #endregion

        #region Members

        /// <summary>
        /// Holding references to current popups
        /// </summary>
        private List<DisplayObject> _popups;

        /// <summary>
        /// References to current popups
        /// </summary>
        internal List<DisplayObject> Popups
        {
            get { return _popups; }
        }
        
        /// <summary>
        /// Storage for references to current descriptors
        /// </summary>
        private Dictionary<DisplayObject, PopupDescriptor> _descriptors;
        
        /// <summary>
        /// References to current descriptors
        /// </summary>
        internal Dictionary<DisplayObject, PopupDescriptor> Descriptors
        {
            get { return _descriptors; }
        }

        private bool _syncing;

        #endregion

        #region Methods

        /// <summary>
        /// The reference to stage
        /// </summary>
        /// <summary>
        /// Initialization routine
        /// </summary>
        private void Initialize()
        {
            // initialize lists
            _popups = new List<DisplayObject>();
            _descriptors = new Dictionary<DisplayObject, PopupDescriptor>();

            InitStage();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Modal overlay type (instantiated by the factory)
        /// </summary>
        public static Type ModalOverlayType = typeof(ModalOverlay);

        #endregion

        #region Create

        /// <summary>
        /// Adds a popup to popup stage
        /// </summary>
        /// <param name="popupType">The popup class for instantiation</param>
        /// <param name="options">Popup options</param>
        public DisplayObject CreatePopup(Type popupType, params PopupOption[] options)
        {
            Component popup = (Component)Activator.CreateInstance(popupType);
            AddPopup(popup, options);
            return popup;
        }

        /// <summary>
        /// Creates the popup via factory/reflection
        /// </summary>
        /// <param name="parent">Parent component (for position calculations)</param>
        /// <param name="popupType">The popup class for instantiation</param>
        /// <param name="modal">Is this a modal popup</param>
        /// <param name="centered">Should popup be centered</param>
        /// <param name="keepCenter">Should popup stay centered after the screen resize</param>
        /// <returns></returns>
        public DisplayObject CreatePopup(Type popupType, DisplayObjectContainer parent, bool modal, bool centered, bool keepCenter)
        {
            Component popup = (Component)Activator.CreateInstance(popupType);
            AddPopup(popup, parent, modal, centered, keepCenter);
            return popup;
        }

        /// <summary>
        /// Creates the popup via factory/reflection
        /// </summary>
        /// <param name="parent">Parent component (for position calculations)</param>
        /// <param name="popupType">The popup class for instantiation</param>
        /// <param name="modal">Is this a modal popup</param>
        /// <param name="centered">Should popup be centered</param>
        /// <returns></returns>
        public DisplayObject CreatePopup(Type popupType, DisplayObjectContainer parent, bool modal, bool centered)
        {
            return CreatePopup(popupType, parent, modal, centered, false);
        }

        /// <summary>
        /// Creates the popup via factory/reflection
        /// </summary>
        /// <param name="parent">Parent component (for position calculations)</param>
        /// <param name="popupType">The popup class for instantiation</param>
        /// <param name="modal">Is this a modal popup</param>
        /// <returns></returns>
        public DisplayObject CreatePopup(Type popupType, DisplayObjectContainer parent, bool modal)
        {
            return CreatePopup(popupType, parent, modal, true);
        }

        /// <summary>
        /// Creates the popup via factory/reflection
        /// </summary>
        /// <param name="parent">Parent component (for position calculations)</param>
        /// <param name="popupType">The popup class for instantiation</param>
        /// <returns></returns>
        public DisplayObject CreatePopup(Type popupType, DisplayObjectContainer parent)
        {
            return CreatePopup(popupType, parent, true);
        }

        //private void RemoveHandler(Event e)
        //{
        //    DisplayObject popup = e.Target as DisplayObject;
        //    if (null != popup)
        //    {
        //        popup.RemoveEventListener(MouseEvent.MOUSE_DOWN_OUTSIDE, RemoveHandler);
        //        popup.RemoveEventListener(MouseEvent.MOUSE_WHEEL_OUTSIDE, RemoveHandler);
        //        TerminatePopup(popup);
        //    }
        //    if (_popups.Count == 0 && SystemManager.Instance.HasEventListener(ResizeEvent.RESIZE))
        //        SystemManager.Instance.RemoveEventListener(ResizeEvent.RESIZE, RemoveHandler);
        //}

        //private static void TerminatePopup(DisplayObject popup)
        //{
        //    //Debug.Log("TerminatePopup");

        //    var dlm = popup as DisplayListMember;

        //    if (null != dlm)
        //    {
        //        Instance.RemovePopup(dlm);
        //        // TODO: SystemManager.Instance.RemoveEventListener(ResizeEvent.RESIZE, ...);
        //        dlm.Dispose();
        //    }
        //}

        #endregion

        #region Add

        /// <summary>
        /// Adds a popup to popup stage
        /// </summary>
        /// <param name="popup">A popup to add</param>
        /// <param name="options">Popup options</param>
        public void AddPopup(DisplayListMember popup, params PopupOption[] options)
        {
            Event e = new Event(OPENING, popup, false, true); // cancelable
            DispatchEvent(e);

            if (e.Canceled)
                return;

            if (IsDefaultPrevented(OPENING))
                return;

            if (_popups.Contains(popup))
                return;
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("AddPopup");
            }
#endif

            DisplayObjectContainer parent = null;
            bool modal = true;
            bool centered = true;
            bool keepCenter = false;
            bool removeOnMouseDownOutside = false;
            bool removeOnMouseWheelOutside = false;
            bool removeOnScreenResize = false;
            bool autoFocus = true;
            bool focusPreviousOnHide = true;
            Stage stage = _stage;

            bool visibleFlag = popup.Visible;
            popup.Visible = false;

            int len = options.Length;
            for (int i = 0; i < len; i++)
            {
                PopupOption option = options[i];
                switch (option.Type)
                {
                    case PopupOptionType.Parent:
                        parent = (DisplayObjectContainer)option.Value;
                        break;

                    case PopupOptionType.Modal:
                        modal = (bool)option.Value;
                        break;

                    case PopupOptionType.Centered:
                        centered = (bool)option.Value;
                        break;

                    case PopupOptionType.KeepCenter:
                        keepCenter = (bool)option.Value;
                        break;

                    case PopupOptionType.RemoveOnMouseDownOutside:
                        removeOnMouseDownOutside = (bool)option.Value;
                        break;

                    case PopupOptionType.RemoveOnMouseWheelOutside:
                        removeOnMouseWheelOutside = (bool)option.Value;
                        break;

                    case PopupOptionType.RemoveOnScreenResize:
                        removeOnScreenResize = (bool)option.Value;
                        break;

                    case PopupOptionType.AutoFocus:
                        autoFocus = (bool)option.Value;
                        break;

                    case PopupOptionType.FocusPreviousOnHide:
                        focusPreviousOnHide = (bool)option.Value;
                        break;

                    case PopupOptionType.Stage:
                        //Debug.Log("Exotic stage: " + option.Value);
                        stage = (Stage)option.Value;
                        break;

                    default:
                        throw new Exception("Unknown option");
                }
            }

            _popups.Add(popup);

            if (null == parent)
                parent = stage;

            DisplayListMember overlay = null;
            Group popupRoot = null;

            InvalidationManagerClient imc = popup as InvalidationManagerClient;
            Component comp = popup as Component;
            if (null != comp)
                comp.IsPopUp = true;

            if (modal)
            {
                overlay = (DisplayListMember)Activator.CreateInstance(ModalOverlayType);
                overlay.AddEventListener(MouseEvent.MOUSE_DOWN, OnOverlayMouseDown);
                
                // we are packing both the overlay and the popup to into an aditional container
                popupRoot = new Group();
                stage.AddChild(popupRoot);

                // BUG BUG BUG! If we do _stage.AddChild(popupRoot); AFTER the children are added, we get a null exception
                // this is the major problem when adding children, started appearing since 10.1.2012
                // solved. This had to to with the creationcomplete method, which has to be run after the complete invalidation pass

                popupRoot.AddChild(overlay);
                popupRoot.AddChild(popup);

                // popup has been added to popup stage
                // invalidation methods have been called upon the addition
                // now we want to run measure (to validate dimensions)
                // because we want to center the popup
                // now, the absolute layout won't do it on his own
                //overlay.Bounds = (Rectangle)parent.Bounds.Clone();

                overlay.Width = PopupManagerStage.Instance.Width;
                overlay.Height = PopupManagerStage.Instance.Height;

                /*var client = overlay as InvalidationManagerClient;
                if (client != null)
                {
                    /*var imc2 = client;
                    imc2.SetActualSize(
                        PopupManagerStage.Instance.Width,
                        PopupManagerStage.Instance.Height/*,
                        Math.Max(imc2.GetExplicitOrMeasuredWidth(), imc2.MinWidth),
                        Math.Max(imc2.GetExplicitOrMeasuredHeight(), imc2.MinHeight)#2#
                    );#1#
                    client.Width = PopupManagerStage.Instance.Width;
                    client.Height = PopupManagerStage.Instance.Height;
                    //imc2.InvalidateTransform();
                }
                else
                {
                    overlay.X = parent.X;
                    overlay.Y = parent.Y;
                    /*overlay.X = parent.X;
                    overlay.Y = parent.Y;#1#
                }*/
            }
            else
            {
                stage.AddChild(popup);
            }

            if (null != imc)
            {
                //InvalidationManager.Instance.ValidateClient(imc, true);
                imc.ValidateNow();
                //Debug.Log(string.Format("imc.Width:{0}, imc.Height:{1}", imc.Width, imc.Height));
                //Debug.Log(string.Format("imc.GetExplicitOrMeasuredWidth():{0}, imc.GetExplicitOrMeasuredHeight():{1}", imc.GetExplicitOrMeasuredWidth(), imc.GetExplicitOrMeasuredHeight()));
                //imc.SetActualSize(imc.GetExplicitOrMeasuredWidth(), imc.GetExplicitOrMeasuredHeight());

                imc.SetActualSize(
                    Math.Min(Math.Max(imc.GetExplicitOrMeasuredWidth(), imc.MinWidth), imc.MaxWidth),
                    Math.Min(Math.Max(imc.GetExplicitOrMeasuredHeight(), imc.MinHeight), imc.MaxHeight)
                );
            }

            var descriptor = new PopupDescriptor(parent, overlay, popupRoot)
                                 {
                                     Popup = popup,
                                     PopupRoot = modal ? popupRoot : popup,
                                     Owner = parent,
                                     Modal = modal,
                                     Centered = centered,
                                     KeepCenter = keepCenter,
                                     RemoveOnMouseDownOutside = removeOnMouseDownOutside,
                                     RemoveOnMouseWheelOutside = removeOnMouseWheelOutside,
                                     RemoveOnScreenResize = removeOnScreenResize,
                                     FocusPreviousOnHide = focusPreviousOnHide,
                                     Stage = stage
                                 };

            _descriptors.Add(popup, descriptor);

            if (centered)
            {
                CenterPopUp(popup);
            }

            InteractiveComponent ic = popup as InteractiveComponent;
            if (autoFocus && null != ic)
            {
                ic.SetFocus(); // TEMP disabled, 2.1.2012.
                //FocusManager.Instance.SetFocus(ic);
                /*ic.Defer(delegate
                {
                    //ic.SetFocus();
                    FocusManager.Instance.SetFocus(ic);
                }, 1);*/
            }
                

            // connect if not connected
            if (_descriptors.Count > 0)
            {
                SystemManager.Instance.ResizeSignal.Connect(ResizeSlot);
                SystemManager.Instance.MouseDownSignal.Connect(MouseDownSlot);
                SystemManager.Instance.RightMouseDownSignal.Connect(MouseDownSlot);
                SystemManager.Instance.MiddleMouseDownSignal.Connect(MouseDownSlot);
                SystemManager.Instance.MouseWheelSignal.Connect(MouseWheelSlot);
                
                // subscribe to stage to see if some component has been mouse-downed
                // NOTE: some components (i.e. window close button) could cancel the event, this is by design

                // note: it is safe ta call it multiple times, it is checked internally
                stage.AddEventListener(MouseEvent.MOUSE_DOWN, MouseDownHandler);
            }
            else
            {
                SystemManager.Instance.ResizeSignal.Disconnect(ResizeSlot);
                SystemManager.Instance.MouseDownSignal.Disconnect(MouseDownSlot);
                SystemManager.Instance.RightMouseDownSignal.Disconnect(MouseDownSlot);
                SystemManager.Instance.MiddleMouseDownSignal.Disconnect(MouseDownSlot);
                SystemManager.Instance.MouseWheelSignal.Disconnect(MouseWheelSlot);

                //MouseEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_DOWN, MouseDownHandler);
                stage.RemoveEventListener(MouseEvent.MOUSE_DOWN, MouseDownHandler);
            }

            // NOTE: This is needed when having effects because of the flicker (?):
            //descriptor.PopupRoot.SkipRender(100);

            // bring the popup root to front
            descriptor.PopupRoot.BringToFront();

            popup.Visible = visibleFlag;

            DispatchEvent(new Event(OPEN, popup));
        }

        private void OnOverlayMouseDown(Event e)
        {
            foreach (PopupDescriptor descriptor in _descriptors.Values)
            {
                if (descriptor.Overlay == e.Target)
                {
                    IFocusComponent client = descriptor.Popup as IFocusComponent;
                    if (null != client)
                        client.SetFocus();
                    return;
                }
            }
        }

        /// <summary>
        /// Adds a popup to popup stage
        /// </summary>
        /// <param name="popup">A popup to add</param>
        /// <param name="parent">Parent component (for position calculations)</param>
        /// <param name="modal">Is this a modal popup</param>
        /// <param name="centered">Should popup be centered</param>
        /// <param name="keepCenter">Should popup stay centered after the screen resize</param>
        public void AddPopup(DisplayListMember popup, DisplayObjectContainer parent, bool modal, bool centered, bool keepCenter)
        {
#if TRIAL
            /* HACK CHECK */
            Acme acme = (Acme) Framework.GetComponent<Acme>(true);
            if (null == acme || !acme.gameObject.activeInHierarchy/*active*/ || !acme.enabled)
                return;
#endif

            if (_popups.Contains(popup))
                return;
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("AddPopup");
            }
#endif
            List<PopupOption> options = new List<PopupOption>
                                            {
                                                new PopupOption(PopupOptionType.Parent, parent),
                                                new PopupOption(PopupOptionType.Modal, modal),
                                                new PopupOption(PopupOptionType.Centered, centered),
                                                new PopupOption(PopupOptionType.KeepCenter, keepCenter)
                                            };

            AddPopup(popup, options.ToArray());
        }

        /// <summary>
        /// Adds a popup to popup stage
        /// </summary>
        /// <param name="popup">A popup to add</param>
        /// <param name="parent">Parent component (for position calculations)</param>
        /// <param name="modal">Is this a modal popup</param>
        /// <param name="centered">Should popup be centered</param>
        public void AddPopup(DisplayListMember popup, DisplayObjectContainer parent, bool modal, bool centered)
        {
            AddPopup(popup, parent ?? _stage, modal, centered, false);
        }

        /// <summary>
        /// Adds a popup to popup stage
        /// </summary>
        /// <param name="popup">A popup to add</param>
        /// <param name="parent">Parent component (for position calculations)</param>
        /// <param name="modal">Is this a modal popup</param>
        public void AddPopup(DisplayListMember popup, DisplayObjectContainer parent, bool modal)
        {
            AddPopup(popup, parent ?? _stage, modal, true); // centered by default
        }

        /// <summary>
        /// Adds a popup to popup stage
        /// </summary>
        /// <param name="popup">A popup to add</param>
        /// <param name="modal">Is this a modal popup</param>
        public void AddPopup(DisplayListMember popup, bool modal)
        {
            AddPopup(popup, _stage, modal); // application by default
        }

        /// <summary>
        /// Adds a popup to popup stage
        /// </summary>
        /// <param name="popup">A popup to add</param>
        /// <param name="modal">Is this a modal popup</param>
        /// <param name="centered">Is the popup centered</param>
        public void AddPopup(DisplayListMember popup, bool modal, bool centered)
        {
            AddPopup(popup, _stage, modal, centered); // application by default
        }

        /// <summary>
        /// Adds a popup to popup stage
        /// </summary>
        /// <param name="popup">A popup to add</param>
        public void AddPopup(DisplayListMember popup)
        {
            AddPopup(popup, true); // modal by default
        }

        #endregion

        #region Has popup

        /// <summary>
        /// Returns true if popup is displayed
        /// </summary>
        /// <param name="popup">A popup to remove</param>
        public bool HasPopup(DisplayObject popup)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("HasPopup");
            }
#endif

            return _descriptors.ContainsKey(popup);
        }

        #endregion

        #region Remove popup

        /// <summary>
        /// Removes a popup from popup stage
        /// </summary>
        /// <param name="popup">A popup to remove</param>
        public void RemovePopup(DisplayObject popup)
        {
#if TRIAL
            /* HACK CHECK */
            Acme acme = (Acme) Framework.GetComponent<Acme>(true);
            if (null == acme || !acme.gameObject.activeInHierarchy/*active*/ || !acme.enabled)
                return;
#endif

#if DEBUG
            if (DebugMode)
            {
                Debug.Log("RemovePopup");
            }
#endif

            if (!_descriptors.ContainsKey(popup))
                return;

            var descriptor = _descriptors[popup];

            //if (popup.HasFocus)
            //    FocusManager.Instance.Blur();

            if (HasEventListener(CLOSING))
            {
                Event e = new Event(CLOSING, popup, false, true); // cancelable
                DispatchEvent(e);

                if (e.Canceled)
                    return;
            }

            if (IsDefaultPrevented(CLOSING))
                return;

            var stage = descriptor.Stage;

            //Debug.Log(string.Format(@"Removing {0} from {1}", descriptor.PopupRoot, stage));

            // removing children
            //descriptor.PopupRoot.Parent.RemoveChild(descriptor.PopupRoot);
            stage.RemoveChild(descriptor.PopupRoot);

            //Debug.Log("Descriptors remove");
            _descriptors.Remove(popup);

            //FocusManager.Instance.Blur(); // TEMP disabled 2.1.2012.

            _popups.Remove(popup);

            if (descriptor.FocusPreviousOnHide)
            {
                if (_popups.Count > 0) // we have more opened popups
                {
                    DisplayObject lastPopup = _popups[_popups.Count - 1];
                    //Debug.Log("_popups.Count: " + _popups.Count);
                    if (lastPopup is InteractiveComponent)
                    {
                        ((InteractiveComponent)lastPopup).SetFocus();
                        /*lastPopup.Defer(delegate
                        {
                            ((InteractiveComponent)lastPopup).SetFocus();
                        }, 1);*/
                        //FocusManager.Instance.SetFocus((InteractiveComponent)lastPopup);
                    }

                    // TEMP disabled on 2.1.2012. because the overlay popup constantly 
                    // appears and dissapears and takes focus
                    // and raises "ArgumentException: You can only call GUI functions from inside OnGUI."
                    // should enable back when overlay will be in it's top stage, non run by popup manager
                }

                else // this was the only popup
                {
                    if (popup is Dialog)
                    {
                        Dialog dlg = (Dialog)popup;
                        if (null != dlg.Owner)
                        {
                            // if owner is defined, focus the owner
                            InteractiveComponent ic = dlg.Owner as InteractiveComponent;
                            /*if (null != ic)
                                ic.SetFocus();*/

                            if (null != ic)
                            {
                                //((InteractiveComponent)lastPopup).SetFocus();
                                /*ic.Defer(delegate
                                {
                                    //ic.SetFocus();
                                    FocusManager.Instance.SetFocus(ic);
                                }, 1);*/
                                //FocusManager.Instance.SetFocus(ic);
                                ic.SetFocus();
                            }

                        }
                        //else
                        //    FocusManager.Instance.Blur(); // else blur everything // commented 20130331 - because after closing th eallert, the SetFocus te TextField (via the callback) didn't work
                    }
                }
            }

            // disconnect
            if (_descriptors.Count == 0)
            {
                SystemManager.Instance.ResizeSignal.Disconnect(ResizeSlot);
                SystemManager.Instance.MouseDownSignal.Disconnect(MouseDownSlot);
                SystemManager.Instance.MouseWheelSignal.Disconnect(MouseWheelSlot);

                stage.RemoveEventListener(MouseEvent.MOUSE_DOWN, MouseDownHandler);
            }

            //Debug.Log("_descriptors.Count: " + _descriptors.Count);

            //_stage.ValidateNow();

            if (HasEventListener(CLOSE))
                DispatchEvent(new Event(CLOSE, popup));

            if (descriptor.Popup != descriptor.PopupRoot)
            {
                // this is a modal popup
                // the removed effect won't play on popup, because PopupRoot is being removed, not the popup
                // we have to handle this case and play the effect here
                Component component = popup as Component;
                if (null != component && component.HasStyle("removedEffect")) {
                    ITweenFactory removeEffect = (ITweenFactory)component.GetStyle("removedEffect");
                    removeEffect.Play(popup);
                }
            }
        }

        #endregion

        #region Center

        /// <summary>
        /// Centers the popup after it is created
        /// </summary>
        /// <param name="popup">The popup to center</param>
        /// <param name="keepCenter">Should the popup manager take care of centering the popup while it's instantiated</param>
        public void CenterPopUp(DisplayObject popup, bool keepCenter)
        {
            if (popup is IInvalidating)
                ((IInvalidating)popup).ValidateNow();
            
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("CenterPopUp");
            }
#endif

            //popup.SetBounds(popup.Bounds.CenterInside(popup.Owner.Bounds));
            
            if (_descriptors.ContainsKey(popup))
            {
                var descriptor = _descriptors[popup];
                
                if (keepCenter != descriptor.KeepCenter)
                    descriptor.KeepCenter = keepCenter;

                //Debug.Log("Stage.Bounds: " + ((Component)popup).Stage.Bounds);
                //Debug.Log("popup.Bounds: " + popup.Bounds);
                var imc = popup as InvalidationManagerClient;
                if (null != imc)
                {
                    var bounds = popup.Bounds.CenterInside(null != descriptor.Owner ? descriptor.Owner.Bounds : descriptor.Stage.Bounds);
                    imc.Move(Mathf.Round(bounds.X), Mathf.Round(bounds.Y));
                }
            }
            else
            {
                throw new Exception("No popup to center");
            }
        }

        public void CenterPopUp(DisplayObject popup)
        {
            CenterPopUp(popup, false);
        }

        #endregion

        #region Handlers

        private void MouseDownHandler(Event e)
        {
            MouseEvent me = (MouseEvent)e;

            Component c = me.Target as Component;
            if (null == c)
                return;

            var targetComponent = (DisplayListMember)me.Target;

            var popup = _popups.Find(delegate(DisplayObject comp)
            {
                IChildList list = comp as IChildList;
                if (null != list)
                    return list.Contains(targetComponent, true);

                return false;
            });

            if (null != popup && _popups.IndexOf(popup) != - 1)
            {
                var descriptor = _descriptors[popup];
                descriptor.Stage.BringChildToFront(descriptor.PopupRoot);

                //if (descriptor.Stage == _stage) // bring to front only popup manager stage children
                //    _stage.BringToFront(descriptor.PopupRoot);
            }
        }

        internal void MouseDownOutsideHandler(DisplayObject popup, Point point)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("MouseDownOutsideHandler: " + popup);
            }
#endif
            // dispatch from here
            MouseEvent e = new MouseEvent(MouseEvent.MOUSE_DOWN_OUTSIDE)
                               {
                                   GlobalPosition = point, Target = popup
                               };
            DispatchEvent(e);

            if (e.Canceled)
                return;

            // dispatch from popup
            e = new MouseEvent(MouseEvent.MOUSE_DOWN_OUTSIDE)
            {
                GlobalPosition = point
            };
            popup.DispatchEvent(e);

            if (e.Canceled)
                return;

            // check auto remove
            if (_descriptors.ContainsKey(popup) && _descriptors[popup].RemoveOnMouseDownOutside)
            {
                RemovePopup(popup);
                return;
            }
        }

        internal void MouseWheelOutsideHandler(DisplayObject popup, Point point)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("MouseDownOutsideHandler: " + popup);
            }
#endif
            // dispatch from here
            MouseEvent e = new MouseEvent(MouseEvent.MOUSE_WHEEL_OUTSIDE)
                               {
                                   GlobalPosition = point, Target = popup
                               };
            DispatchEvent(e);

            if (e.Canceled)
                return;

            // dispatch from popup
            e = new MouseEvent(MouseEvent.MOUSE_WHEEL_OUTSIDE)
            {
                GlobalPosition = point
            };
            popup.DispatchEvent(e);

            if (e.Canceled)
                return;

            // check auto remove
            if (_descriptors.ContainsKey(popup) && _descriptors[popup].RemoveOnMouseWheelOutside)
            {
                RemovePopup(popup);
                return;
            }
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            base.Dispose();

            _popups.Clear();
            _descriptors.Clear();
            
            SystemManager.Instance.ResizeSignal.Disconnect(ResizeSlot);
            SystemManager.Instance.MouseDownSignal.Disconnect(MouseDownSlot);
            SystemManager.Instance.RightMouseDownSignal.Disconnect(MouseDownSlot);
            SystemManager.Instance.MiddleMouseDownSignal.Disconnect(MouseDownSlot);
            SystemManager.Instance.MouseWheelSignal.Disconnect(MouseWheelSlot);
        }

        #endregion

        private PopupManagerStage _stage;

        private void InitStage()
        {
            //Debug.Log("Initializing Stage");
            _stage = PopupManagerStage.Instance;
            
            /* Important: in some circumstances we have to get the stage size right
             * (for instance the popup manager creares the stage just before the first popup is created
             * and the stage has to be sized right for centering popup */
            _stage.ValidateNow();
        }

    }
}
