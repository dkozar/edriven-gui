using System;
using System.Collections.Generic;
using eDriven.Animation;
using eDriven.Gui.Components;
using eDriven.Gui.Managers;
using eDriven.Gui.Plugins;
using eDriven.Gui.Reflection;
using UnityEngine;
using Event=eDriven.Core.Events.Event;
using MulticastDelegate=eDriven.Core.Events.MulticastDelegate;

namespace eDriven.Gui.Containers
{
    [Event(Name = OPEN, Type = typeof(Event))]
    [Event(Name = CLOSE, Type = typeof(Event))]
    [Event(Name = SUBMIT, Type = typeof(Event))]
    [Event(Name = CANCEL, Type = typeof(Event))]
    
    public class Dialog : Window
    {
        #region Static

        /// <summary>
        /// The effect that fires when the dialog is about to be added to screen
        /// </summary>
        public static ITweenFactory AddingEffect;

        /// <summary>
        /// The effect that fires when the dialog is added to screen
        /// </summary>
        public static ITweenFactory AddedEffect;

        /// <summary>
        /// The effect that fires when the dialog is about to become visible
        /// </summary>
        public static ITweenFactory ShowingEffect;

        /// <summary>
        /// The effect that fires when the dialog becomes visible
        /// </summary>
        public static ITweenFactory ShowEffect;

        /// <summary>
        /// The effect that fires when the dialog becomes invisible
        /// </summary>
        public static ITweenFactory HideEffect;

        /// <summary>
        /// The effect that fires when the dialog is removed from screen
        /// </summary>
        public static ITweenFactory RemovedEffect;

        /// <summary>
        /// Optional default skin class
        /// </summary>
        public static Type DefaultSkin; // TODO: Do it with styling (implement skinClass as string in a picker)

        #endregion

        public bool DisposeOnClose;

        private MulticastDelegate _open;
        private MulticastDelegate _close;

        //private Button _resizeHandle;
        
        protected object DefaultButtonId;
        
        protected readonly Dictionary<string, Button> DictId2Button = new Dictionary<string, Button>();
        protected readonly Dictionary<Button, string> DictButton2Id = new Dictionary<Button, string>();
        
// ReSharper disable InconsistentNaming
        public const string OPEN = "dialogOpen";
        public const string CLOSE = "dialogClose";
        public const string SUBMIT = "dialogSubmit";
// ReSharper restore InconsistentNaming

        // ReSharper disable InconsistentNaming
        public const string CANCEL = "cancel";
        // ReSharper restore InconsistentNaming

        //public bool CloseOnEsc; // use for alerts only, because it clashes with combobox escape gesture

        private bool _closeOnEscChanged;
        private bool _closeOnEsc;
        public bool CloseOnEsc // use for alerts only, because it clashes with combobox escape gesture
        {
            get { 
                return _closeOnEsc;
            }
            set
            {
                if (value == _closeOnEsc)
                    return;
        
                _closeOnEsc = value;
                _closeOnEscChanged = true;
                InvalidateProperties();
            }
        }

        #region Callback definition

        // NOTE: Action<T1, T2> => YAGNI?
        public Action<string> Callback { get; set; }

        #endregion
        
        public Dialog()
        {
            MinWidth = 200;
            MinHeight = 100;

            MaxWidth = Screen.width; // moved from InitializationComplete on 20120531
            MaxHeight = Screen.height;

            Draggable = true; // by default, invalidating here

            if (null != AddingEffect/* && !HasStyle("addingEffect")*/)
                SetStyle("addingEffect", AddingEffect);

            if (null != AddedEffect/* && !HasStyle("addedEffect")*/)
                SetStyle("addedEffect", AddedEffect);

            if (null != RemovedEffect/* && !HasStyle("removedEffect")*/)
                SetStyle("removedEffect", RemovedEffect);

            if (null != ShowingEffect/* && !HasStyle("showingEffect")*/)
                SetStyle("showingEffect", ShowingEffect);

            if (null != ShowEffect/* && !HasStyle("showEffect")*/)
                SetStyle("showEffect", ShowEffect);

            if (null != HideEffect/* && !HasStyle("hideEffect")*/)
                SetStyle("hideEffect", HideEffect);

            // instantiate content group to be present if adding children from outside
            // (internal CreateChildren not run yet)
            //_buttonGroup = new Group
            //                   {
            //                       /*ScrollContent = false, */
            //                       Id = "button_group",
            //                       MouseEnabled = false
            //                   };
            //_buttonGroup.Plugins.Add(new TabManager()); // NOTE: This might collide with tab manager set on dialog as a whole. Investigate!
            
            //_buttonGroup.DrawBackground = true;
        }

        //#region Styles

        //private bool _buttonStylesChanged;

        //public override void StyleChanged(string styleName, object s)
        //{
        //    base.StyleChanged(styleName, s);

        //    //Debug.Log("Dialog style changed: " + styleName);
        //    switch (styleName)
        //    {
        //        case "showButtonGroupBackground":
        //            _buttonGroup.SetStyle("showBackground", s);
        //            break;
        //        case "buttonGroupBackgroundStyle":
        //            _buttonGroup.SetStyle("backgroundStyle", s);
        //            break;
        //        case "buttonCursor":
        //            _buttonGroup.Children.ForEach(delegate(DisplayListMember child)
        //                                              {
        //                                                  Button b = child as Button;
        //                                                  if (null != b)
        //                                                      b.SetStyle("cursor", s);
        //                                              });
        //            break;
                
        //            /**
        //         * Aggregate Button style changes
        //         * */
        //        case "overrideButtonStyle":
        //            _buttonStylesChanged = true;
        //            InvalidateProperties();
        //            break;
        //        case "buttonStyleMapper":
        //            _buttonStylesChanged = true;
        //            InvalidateProperties();
        //            break;
        //        case "buttonStyle":
        //            _buttonStylesChanged = true;
        //            InvalidateProperties();
        //            break;
        //    }
        //}

        //#endregion

        //protected override void Measure()
        //{
        //    base.Measure();
        //    Debug.Log("      Dialog Measure");
        //}

        protected override void CommitProperties()
        {
            base.CommitProperties();

            //Debug.Log("      Dialog CommitProperties: _buttonStylesChanged: " + _buttonStylesChanged);

            //if (_buttonStylesChanged)
            //{
            //    _buttonStylesChanged = true;
                
            //    if ((bool)GetStyle("overrideButtonStyle"))
            //    {
            //        string mapper = (string)GetStyle("buttonStyleMapper");

            //        // style mapper has priority
            //        if (!string.IsNullOrEmpty(mapper))
            //        {
            //            _buttonGroup.Children.ForEach(delegate(DisplayListMember child)
            //                                              {
            //                                                  Button b = child as Button;
            //                                                  if (null != b)
            //                                                      b.StyleName = (string)GetStyle("buttonStyleMapper");
            //                                              });
            //        }
            //        else
            //        {
            //            object style = GetStyle("buttonStyle");
            //            _buttonGroup.Children.ForEach(delegate(DisplayListMember child)
            //                                              {
            //                                                  Button b = child as Button;
            //                                                  if (null != b)
            //                                                      b.SetStyle("buttonStyle", style);
            //                                              });
            //        }
            //        _buttonGroup.InvalidateSize();
            //        _buttonGroup.InvalidateDisplayList();
            //    }
            //}

            //if (_draggableChanged)
            //{
            //    _draggableChanged = false;
            //    if (_draggable)
            //    {
            //        if (null == _draggablePlugin)
            //        {
            //            _draggablePlugin = new Draggable { DragHandle = HeaderGroup };
            //            Plugins.Add(_draggablePlugin);
            //            if (Initialized)
            //                _draggablePlugin.Initialize(this);
            //        }
            //        _draggablePlugin.Enabled = true;
            //    }
            //    else
            //    {
            //        if (null != _draggablePlugin)
            //            _draggablePlugin.Enabled = _draggable;
            //    }
            //}

            if (_closeOnEscChanged)
            {
                _closeOnEscChanged = false;

                if (_closeOnEsc)
                {
                    if (null == _closeOnEscPlugin) {
                        _closeOnEscPlugin = new DialogCloseOnEsc();
                        Plugins.Add(_closeOnEscPlugin);
                        if (Initialized)
                            _closeOnEscPlugin.Initialize(this);
                    }
                    //_closeOnEscPlugin.Initialize(this);
                    _closeOnEscPlugin.Enabled = true;
                }
                else
                {
                    if (null != _closeOnEscPlugin)
                        _closeOnEscPlugin.Enabled = _closeOnEsc;
                }
            }
        }

        protected override void InitializationComplete()
        {
            base.InitializationComplete();

            /*if (null != AddingEffect && !HasStyle("addingEffect"))
                SetStyle("addingEffect", AddingEffect);

            if (null != AddedEffect && !HasStyle("addedEffect"))
                SetStyle("addedEffect", AddedEffect);

            if (null != RemovedEffect && !HasStyle("removedEffect"))
                SetStyle("removedEffect", RemovedEffect);

            if (null != ShowingEffect && !HasStyle("showingEffect"))
                SetStyle("showingEffect", ShowingEffect);

            if (null != ShowEffect && !HasStyle("showEffect"))
                SetStyle("showEffect", ShowEffect);

            if (null != HideEffect && !HasStyle("hideEffect"))
                SetStyle("hideEffect", HideEffect);*/

            FocusEnabled = true;
        }

        public override void StylesInitialized()
        {
            base.StylesInitialized();

            if (null != DefaultSkin && !HasStyle("skinClass"))
                SetStyle("skinClass", DefaultSkin);

            // TODO: overlay
        }

        private DialogCloseOnEsc _closeOnEscPlugin;

        public override void Initialize()
        {
            base.Initialize();

            DictId2Button.Clear();
            DictButton2Id.Clear();
        }

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

        protected override void CreationComplete()
        {
            base.CreationComplete();

            // dispatch open event
            DispatchEvent(new Event(OPEN, true));
        }

        public virtual void ExecCallback(string id)
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format("Executing callback [{0}] with parameter [{1}]", null == Callback ? "-" : Callback.ToString(), id));
            }
#endif
            if (null != Callback)
            {
                Callback(id);
            }
            
            PopupManager.Instance.RemovePopup(this);

            if (DisposeOnClose) {
                Callback = null;
                Dispose();
            }
        }
    }
}