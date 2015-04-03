//using System;
//using eDriven.Animation;
//using eDriven.Animation.Easing;
//using eDriven.Animation.Interpolators;
//using eDriven.Core.Events;
//using eDriven.Core.Geom;
//using eDriven.Core.Managers;
//using eDriven.Gui.Components;
//using eDriven.Gui.Events;
//using eDriven.Gui.Layout;
//using eDriven.Gui.Managers;
//using eDriven.Gui.Styles;
//using UnityEngine;
//using Component = eDriven.Gui.Components.Component;
//using Event=eDriven.Core.Events.Event;

//namespace eDriven.Gui.Containers
//{
//    /// <summary>
//    /// The container that is supposed to be scrollable when item count too big
//    /// It instantiates the content group when needed
//    /// Having this separate kind of container is an optimization
//    /// The other way of doing this would be to have the single kind of container, but which adds or removes 
//    /// the content group when Scrollable property changed, and that would move the children from or to this group
//    /// (worth thinking, 20130410)
//    /// All of our containers should finally work like this - having their own content area with controlled width, 
//    /// so that parent container horiz scroll never happens. This is the only guarantee of a proper container working.
//    /// Also I should eventually write my own scrollbar because the original UnityGUI scrollbar is buggy
//    /// </summary>

//    //[StyleProxy(typeof(ContainerStyleProxy))]
//    //[Style(Name = "scrollerSkin", Type = typeof(GUISkin), ProxyMemberName = "ScrollerSkin")]

//    public partial class ScrollableContainer : Container, /*IContentGroupParent, */IFitContent
//    {
//        #region Properties

//        //private DisplayObjectContainer _contentGroup;
//        /// <summary>
//        /// The Viewport to be scrolled
//        /// </summary>
//        //public DisplayObjectContainer ContentGroup
//        //{
//        //    get { return _contentGroup; }
//        //}

//        /// <summary>
//        /// Scroller easing function
//        /// </summary>
//        //public Tween.EasingFunction Easer = Quad.EaseOut;

//        #endregion

//        #region Members

//        //private bool _hSbarValueChanged;
//        //private float _hSbarValue;
//        //private float _oldHSbarValue;
//        //private bool _vSbarValueChanged;
//        //private float _vSbarValue;
//        //private float _oldVSbarValue;
//        //private TweenBase _tween;
//        //private bool _settingByScroller;
//        //private GUISkin _scrollerSkin;
//        //private bool _scrollerSkinChanged;
        
//        #endregion

//        /// <summary>
//        /// Constructor
//        /// </summary>
//        public ScrollableContainer()
//        {
//            MouseEnabled = false;
//            AutoLayout = false; // do not layout the content group automatically!
//            Layout = new AbsoluteLayoutOld();
//        }

//        //public override void StyleChanged(string styleName, object s)
//        //{
//        //    //Debug.Log("List - StyleChanged: " + styleName);
//        //    base.StyleChanged(styleName, s);

//        //    switch (styleName)
//        //    {
//        //        case "scrollerSkin":
//        //            // process style sizes
//        //            //Debug.Log("scrollerSkin changed");
//        //            _scrollerSkin = (GUISkin)s;
//        //            _scrollerSkinChanged = true;
//        //            InvalidateSize();
//        //            InvalidateDisplayList();
//        //            break;
//        //    }
//        //}

//        //private Point _scrollPosition = Point.Zero;
//        //public override Point ScrollPosition
//        //{
//        //    get
//        //    {
//        //        return _scrollPosition;
//        //    }
//        //    set
//        //    {
//        //        //Debug.Log("_calc.VerticalWalk: " + _calc.VerticalWalk);
//        //        //Debug.Log("value.Y: " + value.Y);

//        //        //Debug.Log("Setting scroll position to: " + value);

//        //        Point pos = new Point(Mathf.Clamp(value.X, 0, _calc.HorizontalWalk), Mathf.Clamp(value.Y, 0, _calc.VerticalWalk));

//        //        //Debug.Log("   -> pos: " + pos);

//        //        if (pos == _scrollPosition)
//        //            return;

//        //        _scrollPosition = pos;
//        //        //Debug.Log("   -> _scrollPosition: " + _scrollPosition);


//        //        if (!_settingByScroller)
//        //        {
//        //            _hSbarValue = pos.X;
//        //            _vSbarValue = pos.Y;
//        //        }

//        //        Point tweenPos = (Point)pos.Clone(); // a must, don't reuse the same Point
//        //        tweenPos.Invert();

//        //        //_contentGroup.Position = pos;

//        //        if (!_calc.NeedsHorizontalScroll)
//        //            tweenPos.X = _contentGroup.X;

//        //        if (!_calc.NeedsVerticalScroll)
//        //            tweenPos.Y = _contentGroup.Y;

//        //        //Debug.Log("tweenPos: " + tweenPos + "[" + this + "]");
//        //        //Debug.Log("_contentGroup.Position: " + _contentGroup.Position);
//        //        if (tweenPos.X == _contentGroup.X && tweenPos.Y == _contentGroup.Y)
//        //        {
//        //            //Debug.Log("doing nothing");
//        //            return;
//        //        }

//        //        //Debug.Log("Setting position to: " + tweenPos);
//        //        //_contentGroup.Position = tweenPos;

//        //        if (null != _tween)
//        //        {
//        //            _tween.Stop();
//        //            TweenRegistry.Instance.Remove(_tween);
//        //        }

//        //        _tween = CreateTween(tweenPos);
//        //        _tween.Play(_contentGroup);
//        //    }
//        //}

//        //private ScrollbarCalc _calc;
//        //protected ScrollbarCalc Calc
//        //{
//        //    get { return _calc; }
//        //}

//        //#region IFitContent

//        //private bool _fitContentWidthChanged;
//        //private bool _fitContentWidth;
//        //public bool FitContentWidth
//        //{
//        //    get
//        //    {
//        //        return _fitContentWidth;
//        //    }
//        //    set
//        //    {
//        //        if (value == _fitContentWidth)
//        //            return;

//        //        _fitContentWidth = value;
//        //        _fitContentWidthChanged = true;
//        //        InvalidateProperties();
//        //    }
//        //}

//        //private bool _fitContentHeightChanged;
//        //private bool _fitContentHeight;
//        //public bool FitContentHeight
//        //{
//        //    get
//        //    {
//        //        return _fitContentHeight;
//        //    }
//        //    set
//        //    {
//        //        if (value == _fitContentHeight)
//        //            return;

//        //        _fitContentHeight = value;
//        //        _fitContentHeightChanged = true;
//        //        InvalidateProperties();
//        //    }
//        //}

//        //#endregion

//        //private void OnContentChildMove(Event e)
//        //{
//        //    // Note: we have subscribed to moving of not the content group, but its children
//        //    // If any of the child is moved by ourselves (dragging), remeasure for scrollbars:
//        //    // this is because we might drag the child outside the bounds, and bounds wouldn't normally be remeasured until we resize the container itself

//        //    //Debug.Log("OnContentChildMove: " + e.Target + "; this: " + this);
//        //    if (DragManager.IsDragging)
//        //        ValidateDisplayList(); // remeasure for scrollbars
//        //}

//        //#region Render

//        ///// <summary>
//        ///// Complete override of PreRender
//        ///// </summary>
//        //protected override void PreRender()
//        //{
//        //    //if (!(this is Stage))
//        //    //{
//        //    //    //Debug.Log(string.Format("RenderingRect {0}; {1}", this, RenderingRect));
//        //    //    Debug.Log(string.Format("Bounds {0}; {1}", this, Bounds));
//        //    //}

//        //    //if (Id == "miki")
//        //    //    Debug.Log("RenderingRect: " + RenderingRect);

//        //    if (QIsClipping)
//        //        GUI.BeginGroup(RenderingRect); // ##### GROUP START #####
//        //}

//        ///// <summary>
//        ///// Complete override of PostRender
//        ///// Render scrollbars here just to be sure that they are rendered over all the components
//        ///// </summary>
//        //protected override void PostRender()
//        //{
//        //    GUI.skin = _scrollerSkin;

//        //    if (ScrollContent)
//        //    {
//        //        if (null != _calc)
//        //        {
//        //            //Debug.Log("Rendering scrollbars");
//        //            if (_calc.NeedsVerticalScroll)
//        //            {
//        //                var val = GUI.VerticalScrollbar(_calc.VerticalScrollbarRect, _vSbarValue,
//        //                                                _calc.VerticalThumbHeight, 0, _calc.MaxVertical);
//        //                if (val != _vSbarValue)
//        //                {
//        //                    if (DragManager.IsDragging)
//        //                    {
//        //                        _vSbarValue = val;
//        //                        //Debug.Log("Vert Change: " + val);
//        //                        _vSbarValueChanged = true;
//        //                    }
//        //                }
//        //            }

//        //            if (_calc.NeedsHorizontalScroll)
//        //            {
//        //                var val = GUI.HorizontalScrollbar(_calc.HorizontalScrollbarRect, _hSbarValue,
//        //                                                  _calc.HorizontalThumbWidth, 0, _calc.MaxHorizontal);
//        //                if (val != _hSbarValue)
//        //                {
//        //                    if (DragManager.IsDragging)
//        //                    {
//        //                        _hSbarValue = val;
//        //                        //Debug.Log("Horiz Change: " + val);
//        //                        _hSbarValueChanged = true;
//        //                    }
//        //                }
//        //            }

//        //            if (_calc.NeedsVerticalScroll && _calc.NeedsHorizontalScroll)
//        //            {
//        //                // draw whitebox
//        //                GUI.Label(_calc.WhiteBoxRect, string.Empty, WhiteboxStyle.Instance);
//        //            }
//        //        }

//        //        if (_hSbarValueChanged || _vSbarValueChanged)
//        //        {
//        //            _hSbarValueChanged = false;
//        //            _vSbarValueChanged = false;
//        //            _settingByScroller = true;
//        //            //ScrollPosition = new Point(_hSbarValue, _vSbarValue);
//        //            ScrollBy(new Point(_hSbarValue - _oldHSbarValue, _vSbarValue - _oldVSbarValue));

//        //            _oldHSbarValue = _hSbarValue;
//        //            _oldVSbarValue = _vSbarValue;
                            
//        //            _settingByScroller = false;
//        //        }
//        //    }

//        //    if (QIsClipping)
//        //        GUI.EndGroup(); // ##### GROUP END #####
//        //}

//        //#endregion

//        ///// <summary>
//        ///// Creates a tween used by scrollbars
//        ///// </summary>
//        ///// <returns></returns>
//        //private TweenBase CreateTween(Point endPosition)
//        //{
//        //    return new Parallel(
//        //        new Tween
//        //        {
//        //            Property = "Position",
//        //            Interpolator = new PointInterpolator(),
//        //            Duration = 0.6f,
//        //            Easer = Easer,
//        //            StartValueReader = new PropertyReader("Position"),
//        //            EndValue = endPosition
//        //        }
//        //    );
//        //}

//        //private bool _scrollContent;
//        //private bool _scrollContentChanged;
//        //public override bool ScrollContent
//        //{
//        //    get
//        //    {
//        //        //return _scrollContent;
//        //        return base.ScrollContent;
//        //    }
//        //    set
//        //    {
//        //        //if (value != _scrollContent)
//        //        //{
//        //        //    _scrollContent = value;
//        //        if (value != base.ScrollContent)
//        //        {
//        //            base.ScrollContent = value;
//        //            _scrollContentChanged = true;
//        //            InvalidateProperties();
//        //            InvalidateDisplayList();
//        //            //Debug.Log("_scrollContent: " + _scrollContent);
//        //        }
//        //    }
//        //}

//        //protected override void CommitProperties()
//        //{
//        //    base.CommitProperties();

//        //    if (_scrollContentChanged)
//        //    {
//        //        _scrollContentChanged = false;
//        //        //CreateOrDestroyContentGroup();

//        //        /**
//        //         * Subscribe to mouse down for working in another regime when dragging scrollbars
//        //         * This is needed to turn of all other GUI elements for overlay effects
//        //         * */
//        //        if (ScrollContent)
//        //        {
//        //            AddEventListener(MouseEvent.MOUSE_DOWN, MouseDownHandler, /*EventPhase.Capture | */EventPhase.Target);
//        //        }
//        //        else
//        //        {
//        //            RemoveEventListener(MouseEvent.MOUSE_DOWN, MouseDownHandler, /*EventPhase.Capture | */EventPhase.Target);
//        //        }
//        //    }

//        //    if (_scrollerSkinChanged)
//        //    {
//        //        _scrollerSkinChanged = false;

//        //        if (null != _calc)
//        //        {
//        //            // calculate the width as max width of these 5 elements
//        //            //Debug.Log("scrollerSkin changed to: " + _scrollerSkin);
//        //            SetCalcParams();
//        //            _calc.Measure();
//        //        }
//        //    }

//        //    if (_fitContentWidthChanged)
//        //    {
//        //        if (_fitContentWidth)
//        //        {
//        //            //_contentGroup.SetActualWidth(Width/*GetExplicitOrMeasuredWidth()*/);
//        //            if (null != _contentGroup)
//        //            {
//        //                _contentGroup.X = 0;
//        //                _contentGroup.Width = Width;
//        //            }
//        //        }
//        //        else
//        //        {
//        //            //_contentGroup.ExplicitWidth = null;
//        //        }
//        //    }

//        //    if (_fitContentHeightChanged)
//        //    {
//        //        if (_fitContentHeight)
//        //        {
//        //            //_contentGroup.SetActualHeight(Height/*GetExplicitOrMeasuredHeight()*/);
//        //            if (null != _contentGroup) {
//        //                _contentGroup.Y = 0;
//        //                _contentGroup.Height = Height;
//        //            }
//        //        }
//        //        else
//        //        {
//        //            //_contentGroup.ExplicitHeight = null;
//        //        }
//        //    }

//        //    if (_fitContentWidthChanged || _fitContentHeightChanged)
//        //    {
//        //        _fitContentHeightChanged = false;
//        //        _fitContentHeightChanged = false;
//        //        if (null != _calc)
//        //            _calc.Measure();
//        //    }
//        //}

//        //private bool CreateOrDestroyContentGroup()
//        //{
//        //    bool created = false;
//        //    if (ScrollContent && null == _contentGroup)
//        //    {
//        //        var numberOfChildren = NumberOfChildren; // snapshot now
//        //        //Debug.Log("* numberOfChildren: " + numberOfChildren);

//        //        /**
//        //         * Simple display object!
//        //         * */
//        //        _contentGroup = new DisplayObjectContainer
//        //        {
//        //            X = 0,
//        //            Y = 0
//        //        };

//        //        QAddChildAt(numberOfChildren, _contentGroup);
//        //        //Debug.Log(1);
//        //        //_contentGroup.BakePosition(); // TODO (weird behaviour of scrollable container inspector rollover)
//        //        //Debug.Log("* " + _contentGroup.RenderingRect);

//        //        _calc = new ScrollbarCalc(this, _contentGroup);

//        //        if (null != _scrollerSkin)
//        //        {
//        //            // calculate the width as max width of these 5 elements
//        //            //Debug.Log("scrollerSkin changed to: " + _scrollerSkin);
//        //            SetCalcParams();
//        //            _calc.Measure();
//        //        }

//        //        //Debug.Log("*** Content group initialized: " + this);

//        //        ChildMover mover = new ChildMover(this, _contentGroup, numberOfChildren);
//        //        mover.Move();

//        //        /**
//        //         * Subscribing to move event of each content group child
//        //         * */
//        //        foreach (DisplayListMember child in _contentGroup.Children)
//        //        {
//        //            child.AddEventListener(MoveEvent.MOVE, OnContentChildMove);
//        //        }

//        //        CreatingContentPane = true;

//        //        /**
//        //         * This has to be done at the end (after all children are transfered)
//        //         * That is because we should move the content group also if adding it earlier!
//        //         * */
//        //        //QAddChild(_contentGroup); // NOTE: important to do "base"

//        //        CreatingContentPane = false;

//        //        created = true;
//        //    }
//        //    else if (!ScrollContent && null != _contentGroup)
//        //    {
//        //        //if (this is CursorManagerStage) 
//        //        //    Debug.Log("   ... destroying");

//        //        _calc.Dispose();

//        //        var c = Parent as Container;
//        //        CreatingContentPane = true;

//        //        /**
//        //         * This has to be done before all the other,
//        //         * just to get back the right container (not to put children back into the content pane)
//        //         * */
//        //        QRemoveChild(_contentGroup);

//        //        CreatingContentPane = false;

//        //        foreach (DisplayListMember child in _contentGroup.Children)
//        //        {
//        //            child.RemoveEventListener(MoveEvent.MOVE, OnContentChildMove);
//        //        }

//        //        ChildMover mover = new ChildMover(_contentGroup, this, _contentGroup.NumberOfChildren);
//        //        mover.Move();

//        //        //_contentGroup.RemoveAllChildren();
//        //        //_contentGroup.Dispose();

//        //        _contentGroup = null;
//        //    }
//        //    //else
//        //    //{
//        //    //    Debug.Log(string.Format("_scrollContent: {0}; null == _contentGroup: {1}", ScrollContent, null == _contentGroup));
//        //    //}

//        //    return created;
//        //}

//        //public override void ValidateDisplayList()
//        //{
//        //    //Debug.Log("ValidateDisplayList");
//        //    base.ValidateDisplayList();

//        //    if (CreateOrDestroyContentGroup())
//        //    {
//        //        // Based on the positions of the children, determine
//        //        // whether a clip mask and scrollbars are needed.
//        //        if (AutoLayout || ForceLayout)
//        //        {
//        //            base.ValidateDisplayList();

//        //            // If a scrollbar was created, that may precipitate the need
//        //            // for a second scrollbar, so run it a second time.
//        //            //CreateOrDestroyContentGroup();
//        //        }
//        //    }

//        //    if (null != _contentGroup)
//        //        CalcContentGroupBounds();
//        //}

//        //protected override void UpdateDisplayList(float width, float height)
//        //{
//        //    //base.UpdateDisplayList(width, height);
//        //    //Debug.Log(string.Format("UpdateDisplayList [{0}]. Layout: {1}", this, Layout));
//        //    //Debug.Log(string.Format("Bounds [{0}]", Bounds));

//        //    // we might override this completely
//        //    Layout.LayoutChildren(Width, Height);

//        //    if (_scrollerSkinChanged)
//        //    {
//        //        _scrollerSkinChanged = false;
//        //        if (null != _calc)
//        //        {
//        //            SetCalcParams();
//        //            //_calc.Measure();
//        //        }
//        //    }

//        //    if (ScrollContent)
//        //    {
//        //        if (null != _contentGroup)
//        //            CalcContentGroupBounds();
//        //    }

//        //    if (null != _calc)
//        //        _calc.Measure();
//        //}

//        ///// <summary>
//        ///// Calculates width and height needed to fit all the children into the content pane
//        ///// </summary>
//        ///// <returns></returns>
//        //private Rectangle GetScrollableRect()
//        //{
//        //    float left = 0;
//        //    float top = 0;
//        //    float right = 0;
//        //    float bottom = 0;

//        //    int n = NumberOfContentChildren;
//        //    //Debug.Log("NumberOfContentChildren: " + NumberOfContentChildren);
//        //    for (int i = 0; i < n; i++)
//        //    {
//        //        float x;
//        //        float y;
//        //        float width;
//        //        float height;

//        //        DisplayObject child = GetContentChildAt(i);
//        //        if (child is Component)
//        //        {
//        //            var cmp = (Component)child;
//        //            if (!cmp.IncludeInLayout)
//        //                continue;

//        //            width = cmp.Width;
//        //            height = cmp.Height;
//        //            x = cmp.X;
//        //            y = cmp.Y;

//        //            //Debug.Log("x: " + x);
//        //            //Debug.Log("width: " + width);
//        //        }
//        //        else
//        //        {
//        //            //Debug.Log(2);
//        //            width = child.Width;
//        //            height = child.Height;
//        //            x = child.X;
//        //            y = child.Y;
//        //        }

//        //        left = Math.Min(left, x);
//        //        top = Math.Min(top, y);

//        //        if (width > 0)
//        //            right = Math.Max(right, x + width);
//        //        if (height > 0)
//        //            bottom = Math.Max(bottom, y + height);
//        //    }

//        //    right += PaddingRight;
//        //    bottom += PaddingBottom;

//        //    Rectangle bounds = new Rectangle { X = left, Y = top, Width = right - left, Height = bottom - top };
//        //    return bounds;
//        //}

//        ///// <summary>
//        ///// Scrolls the view so the component is fully visible
//        ///// </summary>
//        ///// <param name="component"></param>
//        //public void ScrollIntoView(Component component)
//        //{
//        //    //Debug.Log("ScrollIntoView");
//        //    float xEndValue = 0f;
//        //    float yEndValue = 0f;
//        //    bool changed = false;

//        //    Rectangle globalBounds = null != _contentGroup ? _contentGroup.LocalToGlobal(component.Bounds) : Transform.GlobalBounds;
//        //    Rectangle boundsInThis = GlobalToLocal(globalBounds);

//        //    //Debug.Log("boundsInThis: " + boundsInThis);

//        //    if (boundsInThis.Bottom > Height)
//        //    {
//        //        yEndValue = Mathf.Max(component.Bottom - Height, 0);
//        //        changed = true;
//        //    }
//        //    else if (boundsInThis.Top < 0)
//        //    {
//        //        yEndValue = component.Top;// -Height;
//        //        changed = true;
//        //    }

//        //    if (boundsInThis.Right > Width)
//        //    {
//        //        xEndValue = Mathf.Max(component.Bottom - Height, 0);
//        //        changed = true;
//        //    }
//        //    else if (boundsInThis.Top < 0)
//        //    {
//        //        xEndValue = component.Top;// -Height;
//        //        changed = true;
//        //    }

//        //    if (!changed)
//        //        return;

//        //    if (null != _tween)
//        //    {
//        //        _tween.Stop();
//        //        TweenRegistry.Instance.Remove(_tween);
//        //    }

//        //    _tween = CreateTween(new Point(-xEndValue, -yEndValue));
//        //    _tween.Play(_contentGroup);
//        //}

//        //#region Mouse

//        ///// <summary>
//        ///// Avoiding rollovering style on other components while dragging the thumb
//        ///// </summary>
//        ///// <param name="e"></param>
//        //private static void MouseDownHandler(Event e)
//        //{
//        //    //Debug.Log("MouseDownHandler");
//        //    DragManager.IsDragging = true;
//        //    SystemManager.Instance.MouseUpSignal.Connect(MouseUpSlot, true);
//        //}

//        //private static void MouseUpSlot(object[] parameters)
//        //{
//        //    ReleaseMouseListener();
//        //}

//        ///// <summary>
//        ///// TODO: Think about removing MOUSE_DOWN listener when scrollbars aren't displayed!
//        ///// </summary>
//        //private static void ReleaseMouseListener()
//        //{
//        //    //Debug.Log("Releasing mouse listener");
//        //    DragManager.IsDragging = false;
//        //    //RemoveEventListener(MouseEvent.MOUSE_DOWN, MouseDownHandler, /*EventPhase.Capture | */EventPhase.Target);
//        //}

//        //#endregion

//        #region IDisposable

//        public override void Dispose()
//        {
//            //if (null != _contentGroup) {
//            //    _calc.Dispose();
//            //    RemoveEventListener(MouseEvent.MOUSE_DOWN, MouseDownHandler, /*EventPhase.Capture | */EventPhase.Target);
//            //}
            
//            base.Dispose();
//        }

//        #endregion
//    }
//}