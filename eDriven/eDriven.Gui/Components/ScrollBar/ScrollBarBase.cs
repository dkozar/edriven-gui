using System;
using eDriven.Core.Events;
using eDriven.Core.Geom;
using eDriven.Core.Managers;
using eDriven.Core.Util;
using eDriven.Gui.Events;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;
using UnityEngine;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Components
{
	//[Event(Name = ScrollEvent.SCROLL, Type = typeof(ScrollEvent), Bubbles = true)]

	///<summary>
	///</summary>
	
	[Style(Name = "autoThumbVisibility", Type = typeof(bool), Default = false)]
	[Style(Name = "fixedThumbSize", Type = typeof(bool), Default = false)]
	[Style(Name = "smoothScrolling", Type = typeof(bool), Default = false)]
	[Style(Name = "slideDuration", Type = typeof(float), Default = 0.75f)]
	[Style(Name = "repeatDelay", Type = typeof(float), Default = 0.5f)]
	[Style(Name = "repeatInterval", Type = typeof(float), Default = 0.035f)]

	public class ScrollBarBase : TrackBase
	{
		// ReSharper disable MemberCanBePrivate.Global

		///<summary>
		/// Decrement button
		///</summary>
		[SkinPart(Required = false)] // Id = "DecrementButton1",
		public Button DecrementButton;

		[SkinPart(Required = false)]
		public Button IncrementButton;

		/**
		 * 
		 * These variables track whether we are currently involved in a stepping
		 * animation, and which direction we are stepping
		 */
		private bool _steppingDown;
		private bool _steppingUp;
		
		/**
		 * 
		 * This variable tracks whether we are currently stepping the ScrollBarBase
		 */
		private bool _isStepping;

		private bool _trackScrollDown;
	
		// Timer used for repeated scrolling when mouse is held down on track
		private Timer _trackScrollTimer;
		
		// Cache current position on track for scrolling operations
		private Point _trackPosition = new Point();
		
		// Flag to indicate whether track-scrolling is in process
		private bool _trackScrolling;

		public override float Minimum
		{
			get 
			{ 
				 return base.Minimum;
			}
			set 
			{ 
				

				if (value == base.Minimum)
					return;
				
				base.Minimum = value;
				InvalidateSkinState();
			}
		}

		public override float Maximum
		{
			get 
			{ 
				 return base.Maximum;
			}
			set 
			{ 
				base.Maximum = value;

				if (value == base.Maximum)
					return;
				
				base.Maximum = value;
				InvalidateSkinState();
			}
		}

		public override float? SnapInterval
		{
			get
			{
				return base.SnapInterval;
			}
			set
			{
				base.SnapInterval = value;
				_pageSizeChanged = true;
			}
		}

		private float _pageSize = 20;

		private bool _pageSizeChanged;

// ReSharper disable MemberCanBePrivate.Global
		public float PageSize
// ReSharper restore MemberCanBePrivate.Global
		{
			get
			{
				return _pageSize;
			}
			set
			{
				if (value == _pageSize)
					return;
					
				_pageSize = value;
				_pageSizeChanged = true;

				//Debug.Log("PageSize changed to " + value);
				
				InvalidateProperties();
				InvalidateDisplayList();
			}
		}

		private float NearestValidSize(float size)
		{
			if (null == SnapInterval || SnapInterval == 0)
				return size;

			float interval = (float) SnapInterval;

			float validSize = Mathf.Round(size/interval)*interval;
			return (Mathf.Abs(validSize) < interval) ? interval : validSize;
		}

		protected override void CommitProperties()
		{
			base.CommitProperties();

			if (_pageSizeChanged)
			{
				_pageSize = NearestValidSize(_pageSize);
				_pageSizeChanged = false;
			}
		}

		protected override void PartAdded(string partName, object instance)
		{
			base.PartAdded(partName, instance);

			if (instance == DecrementButton)
			{
				DecrementButton.AddEventListener(ButtonEvent.BUTTON_DOWN,
												 ButtonDownHandler);
				DecrementButton.AddEventListener(MouseEvent.ROLL_OVER,
												 ButtonRollOverHandler);
				DecrementButton.AddEventListener(MouseEvent.ROLL_OUT,
												 ButtonRollOutHandler);
				DecrementButton.AutoRepeat = true;
				DecrementButton.Enabled = Enabled;
			}
			else if (instance == IncrementButton)
			{
				IncrementButton.AddEventListener(ButtonEvent.BUTTON_DOWN,
												 ButtonDownHandler);
				IncrementButton.AddEventListener(MouseEvent.ROLL_OVER,
												 ButtonRollOverHandler);
				IncrementButton.AddEventListener(MouseEvent.ROLL_OUT,
												 ButtonRollOutHandler);
				IncrementButton.AutoRepeat = true;
				IncrementButton.Enabled = Enabled;
			}
			else if (instance == Track)
			{
				Track.AddEventListener(MouseEvent.ROLL_OVER,
									   TrackRollOverHandler);
				Track.AddEventListener(MouseEvent.ROLL_OUT,
									   TrackRollOutHandler);
				Track.Enabled = Enabled;
			}
		}

		protected override void PartRemoved(string partName, object instance)
		{
			base.PartRemoved(partName, instance);

			if (instance == DecrementButton)
			{
				DecrementButton.RemoveEventListener(ButtonEvent.BUTTON_DOWN,
													ButtonDownHandler);
				DecrementButton.RemoveEventListener(MouseEvent.ROLL_OVER,
													ButtonRollOverHandler);
				DecrementButton.RemoveEventListener(MouseEvent.ROLL_OUT,
													ButtonRollOutHandler);
			}
			else if (instance == IncrementButton)
			{
				IncrementButton.RemoveEventListener(ButtonEvent.BUTTON_DOWN,
													ButtonDownHandler);
				IncrementButton.RemoveEventListener(MouseEvent.ROLL_OVER,
													ButtonRollOverHandler);
				IncrementButton.RemoveEventListener(MouseEvent.ROLL_OUT,
													ButtonRollOutHandler);
			}
			else if (instance == Track)
			{
				Track.RemoveEventListener(MouseEvent.ROLL_OVER,
										  TrackRollOverHandler);
				Track.RemoveEventListener(MouseEvent.ROLL_OUT,
										  TrackRollOutHandler);
			}
		}

		//override public function styleChanged(styleProp:String):void
		//{
		//    super.styleChanged(styleProp);
		//    if (styleProp == "autoThumbVisibility")
		//        invalidateDisplayList();
		//}

// ReSharper disable MemberCanBePrivate.Global
		///<summary>
		///</summary>
		///<param name="increase"></param>
		public virtual void ChangeValueByPage(bool increase)
// ReSharper restore MemberCanBePrivate.Global
		{
			//Debug.Log("Maximum: " + Maximum + " PageSize: " + PageSize + " Value: " + Value);
			float val = increase ? Mathf.Min(Value + PageSize, Maximum) : Mathf.Max(Value - PageSize, Minimum);
			//Debug.Log("ChangeValueByPage.increase: " + increase + ", val: " + val);

			//if (getStyle("smoothScrolling"))
			//{
			//    startAnimation(getStyle("repeatInterval"), val, linearEaser);
			//}
			//else
			//{
				SetValue(val);
				DispatchEvent(new Event(Event.CHANGE));
			//}
		}

		//protected override void ThumbMouseDownHandler(eDriven.Core.Events.Event e)
		//{
		//    //stopAnimation();

		//    base.ThumbMouseDownHandler(e);
		//}

		protected virtual void ButtonDownHandler(Event e)
		{
			//Debug.Log("ButtonDownHandler: " + e.Target);

			// Make sure we finish any running page animation before starting
			// to step.
			//if (!_isStepping)
			//    stopAnimation();
			
			bool increment = (e.Target == IncrementButton);
			
			// Dispatch changeStart for the first step if we can make a step.
			if (!_isStepping && 
				((increment && Value < Maximum) ||
					(!increment && Value > Minimum)))
			{
				DispatchEvent(new FrameworkEvent(FrameworkEvent.CHANGE_START));
				_isStepping = true;
				var sm = SystemEventDispatcher.Instance;
				sm.AddEventListener(MouseEvent.MOUSE_UP, ButtonButtonUpHandler, EventPhase.Capture | EventPhase.Target);
				sm.AddEventListener(MouseEvent.MOUSE_LEAVE, ButtonButtonUpHandler);
			}
			
			// Noop if we're currently running a stepping animation. We get
			// called repeatedly here due to the button's autoRepeat
			if (!_steppingDown && !_steppingUp)
			{
				ChangeValueByStep(increment);
				
				// Only animate if smoothScrolling enabled and we're not at the end already
				//if (getStyle("smoothScrolling") &&
				//    ((increment && value < maximum) ||
				//     (!increment && value > minimum)))
				//{
				//    // Default stepSize may be too small to be useful here; use fraction of
				//    // pageSize if it's larger
				//    animateStepping(increment ? maximum : minimum, 
				//        Math.max(pageSize/10, stepSize));
				//}
				//LineScroll(increment ? 1 : -1);

				//LineScroll(Direction == Direction.Vertical ^ increment ? 1 : -1);
				
				return;
			}
		}

		protected virtual void ButtonButtonUpHandler(Event e)
		{
			if (_steppingDown || _steppingUp)
			{
				// stopAnimation will not dispatch a changeEnd.
				//stopAnimation();
				
				DispatchEvent(new FrameworkEvent(FrameworkEvent.CHANGE_END));
				
				_steppingUp = _steppingDown = false;
				_isStepping = false;
			}
			else if (_isStepping)
			{
				// Dispatch changeEnd event for no animation case
				DispatchEvent(new FrameworkEvent(FrameworkEvent.CHANGE_END));
				_isStepping = false;
			}

			var sm = SystemEventDispatcher.Instance;
			sm.RemoveEventListener(MouseEvent.MOUSE_UP, ButtonButtonUpHandler, EventPhase.Capture | EventPhase.Target);
			sm.RemoveEventListener(MouseEvent.MOUSE_LEAVE, ButtonButtonUpHandler);
		}

		override protected void TrackMouseDownHandler(Event e)
		{
			MouseEvent me = (MouseEvent) e;

			// TODO (chaase): We might want a different event mechanism eventually
			// which would push this enabled check into the child/skin components
			if (!Enabled)
				return;

			//Debug.Log("TrackMouseDownHandler: " + this);

			// Make sure we finish any running page animation before starting
			// a new one.
			//stopAnimation();
			
			// Cache original event location for use on later repeating events
			_trackPosition = Track.GlobalToLocal(me.GlobalPosition);
			
			// If the user shift-clicks on the track, then offset the event coordinates so 
			// that the thumb ends up centered under the mouse.
			if (me.Shift)
			{
				float thumbW = (null != Thumb) ? Thumb.Width /*Thumb.getLayoutBoundsWidth()*/ : 0;
				float thumbH = (null != Thumb) ? Thumb.Height /*Thumb.getLayoutBoundsHeight()*/ : 0;
				_trackPosition.X -= (thumbW / 2);
				_trackPosition.Y -= (thumbH / 2); 
			}

			float newScrollValue = PointToValue(_trackPosition.X, _trackPosition.Y);
			//Debug.Log("newScrollValue: " + newScrollValue);
			_trackScrollDown = (newScrollValue > Value);

			if (me.Shift)
			{
				//Debug.Log("slideDuration: " + GetStyle("slideDuration"));
				//Debug.Log("smoothScrolling: " + GetStyle("smoothScrolling"));

				// shift-click positions jumps to the clicked location instead
				// of incrementally paging
				float slideDuration = (float)GetStyle("slideDuration");
				float adjustedValue = NearestValidValue(newScrollValue, SnapInterval);
				if ((bool)GetStyle("smoothScrolling") && 
					slideDuration != 0 && 
					(Maximum - Minimum) != 0)
				{
					DispatchEvent(new FrameworkEvent(FrameworkEvent.CHANGE_START));
					//// Animate the shift-click operation
					//startAnimation(slideDuration * 
					//    (Math.abs(value - newScrollValue) / (maximum - minimum)),
					//    adjustedValue, deceleratingSineEaser);
					//animatingOnce = true;
				}
				else
				{
					SetValue(adjustedValue);
					DispatchEvent(new Event(Event.CHANGE));
				}
				
				return;
			}
			
			DispatchEvent(new FrameworkEvent(FrameworkEvent.CHANGE_START));
			// Assume we're repeating unless user releases 
			//animatingOnce = false;
			
			ChangeValueByPage(_trackScrollDown);
			
			_trackScrolling = true;

			// Add event handlers for drag and up events
			var sm = SystemEventDispatcher.Instance;
			sm.AddEventListener(MouseEvent.MOUSE_MOVE, TrackMouseMoveHandler, EventPhase.Capture | EventPhase.Target);
			sm.AddEventListener(MouseEvent.MOUSE_UP, TrackMouseUpHandler, EventPhase.Capture | EventPhase.Target);
			sm.AddEventListener(MouseEvent.MOUSE_LEAVE, TrackMouseUpHandler);

			if (null == _trackScrollTimer)
			{
				_trackScrollTimer = new Timer((float) GetStyle("repeatDelay"), 1);
				_trackScrollTimer.AddEventListener(Timer.TICK, 
												  TrackScrollTimerHandler);
			} 
			else
			{
				// Note that this behavior, resetting the initial delay, differs 
				// from Flex3 but is more consistent with general application
				// scrollbar behavior
				_trackScrollTimer.Delay = (float) GetStyle("repeatDelay");
				_trackScrollTimer.RepeatCount = 1;
			}
			_trackScrollTimer.Start();
		}

		/**
		 *  
		 *  This gets called at certain intervals to repeat the scroll 
		 *  event when the user is still holding the mouse button 
		 *  down on the track.
		 */
		private void TrackScrollTimerHandler(Event e)
		{
			// Only repeat the scrolling if the current scroll position
			// (represented by fraction) is not past the current
			// mouse position on the track 
			float newScrollValue = PointToValue(_trackPosition.X, _trackPosition.Y);
			bool fixedThumbSize = (bool) GetStyle("fixedThumbSize");

			// The end result we want, with either animated or non-animated paging,
			// is for the thumb to end up under the click point.
			// For the fixedThumbSize case, where the thumb may be much smaller
			// than the pageSize, we instead want the thumb to end up
			// where it would in the variable size case (on a lower value than the 
			// clicked value), but to end up at the end of the track if it is
			// "close enough" to the end. The heuristic for "close enough" is
			// if the end of the track is the nearestValidValue on pageSize
			// boundaries.
			if (_trackScrollDown)
			{
				float range = Maximum - Minimum;
				if (range == 0)
					return;
				
				if ((Value + PageSize) > newScrollValue &&
					(!fixedThumbSize || NearestValidValue(newScrollValue, PageSize) != Maximum))
						return;
			}
			else if (newScrollValue > Value)
			{
				return;
			}

			if ((bool) GetStyle("smoothScrolling"))
			{
				// This gets called after an initial repeateDelay on a paging
				// operation, but after that we're just running the animation. This
				// function is only called repeatedly in the non-smoothScrolling case.
				//float valueDelta = Mathf.Abs(Value - newScrollValue);
				//int pages;
				//float pageToVal;
				//if (newScrollValue > Value)
				//{
				//    pages = (int) (PageSize != 0 ? 
				//                                     (int)(valueDelta / PageSize) :
				//                                                                      valueDelta);
				//    if (fixedThumbSize && NearestValidValue(newScrollValue, PageSize) == Maximum)
				//        pageToVal = Maximum;
				//    else
				//        pageToVal = Value + (pages * PageSize);
				//}
				//else
				//{
				//    pages = (int) (PageSize != 0 ? 
				//                                     (int)(Mathf.Ceil(valueDelta / PageSize)) :
				//                                                                                  valueDelta);
				//    pageToVal = Mathf.Max(Minimum, Value - (pages * PageSize));
				//}
				//animatePaging(pageToVal, PageSize);
				return;
			}

			//float oldValue = Value;
			
			ChangeValueByPage(_trackScrollDown);

			if (null != _trackScrollTimer && _trackScrollTimer.RepeatCount == 1)
			{
				// If this was the first time repeating, set the Timer to
				// repeat indefinitely with an appropriate interval delay
				_trackScrollTimer.Delay = (float) GetStyle("repeatInterval");
				_trackScrollTimer.RepeatCount = 0;
			}
		}

		private void TrackMouseMoveHandler(Event e)
		{
			if (_trackScrolling)
			{
				MouseEvent me = (MouseEvent) e;
				// Cache original event location for use on later repeating events
				_trackPosition = Track.GlobalToLocal(me.GlobalPosition);
			}
		}

		/**
		 *  
		 *  Stop scrolling the track if the mouse leaves the stage
		 *  area. Remove the listeners and stop the Timer.
		 */
		private void TrackMouseUpHandler(Event e)
		{
			_trackScrolling = false;

			var sm = SystemEventDispatcher.Instance;
			sm.RemoveEventListener(MouseEvent.MOUSE_MOVE, TrackMouseMoveHandler, EventPhase.Capture | EventPhase.Target);
			sm.RemoveEventListener(MouseEvent.MOUSE_UP, TrackMouseUpHandler, EventPhase.Capture | EventPhase.Target);
			sm.RemoveEventListener(MouseEvent.MOUSE_LEAVE, TrackMouseUpHandler);

			// First, we check for smoothScrolling and also if we are
			// in the non-repeating case.
			if ((bool) GetStyle("smoothScrolling"))
			{
				//if (!animatingOnce)
				//{
				//    // We check the timer to see if the user released the mouse
				//    // before the repeat delay has expired.
				//    if (_trackScrollTimer && _trackScrollTimer.running)
				//    {
				//        // If the animation has not yet finished before the repeat delay
				//        // we set animatingOnce to true. Otherwise, the animation
				//        // is done but repeating has not begun so we dispatch a changeEnd
				//        // event.
				//        if (animator.isPlaying)
				//            animatingOnce = true;
				//        else
				//            dispatchEvent(new FlexEvent(FlexEvent.CHANGE_END));
				//    }
				//    else
				//    {
				//        // repeating case
				//        stopAnimation();
				//        dispatchEvent(new FlexEvent(FlexEvent.CHANGE_END));
				//    }
				//}
			}
			else
			{
				// Dispatch changeEnd if there's no animation.
				DispatchEvent(new FrameworkEvent(FrameworkEvent.CHANGE_END));
			}
			
			if (null != _trackScrollTimer) {
				_trackScrollTimer.Reset();
				_trackScrollTimer.Stop();
			}
		}

		 /**
		 *  
		 *  If we are still in the middle of track-scrolling, restart the
		 *  timer when the mouse re-enters the track area.
		 */
		private void TrackRollOverHandler(Event e)
		{
			if (_trackScrolling && null != _trackScrollTimer)
				_trackScrollTimer.Start();
		}

		/**
		 *  
		 *  Stop the track-scrolling repeat events if the mouse leaves
		 *  the track area.
		 */
		private void TrackRollOutHandler(Event e)
		{
			if (_trackScrolling && null != _trackScrollTimer)
				_trackScrollTimer.Stop();
		}

		/**
		 *  
		 *  Resume the increment/decrement animation if the mouse enters the
		 *  button area
		 */
// ReSharper disable MemberCanBeMadeStatic.Local
		private void ButtonRollOverHandler(Event e)
// ReSharper restore MemberCanBeMadeStatic.Local
		{
			//if (_steppingUp || _steppingDown)
			//    animator.resume();
		}

		/**
		 *  
		 *  Pause the increment/decrement animation if the mouse leaves the
		 *  button area
		 */
// ReSharper disable MemberCanBeMadeStatic.Local
		private void ButtonRollOutHandler(Event e)
// ReSharper restore MemberCanBeMadeStatic.Local
		{
			//if (_steppingUp || _steppingDown)
			//    animator.pause();
		}

		/*private float _lineScrollSize = 1;
		public float LineScrollSize
		{
			get { return _lineScrollSize; }
			set
			{
				_lineScrollSize = value;
			}
		}*/

		/// <summary>
		/// Page scroll size
		/// </summary>
		public float PageScrollSize { get; set; }

		protected virtual float TrackHeight { 
			get
			{
				return 100;
			} 
		}

		/*/// <summary>
		/// Sets scroll properties in one go
		/// </summary>
		/// <param name="pageSize"></param>
		/// <param name="minScrollPosition"></param>
		/// <param name="maxScrollPosition"></param>
		/// <param name="pageScrollSize"></param>
		public void SetScrollProperties(float pageSize, float minScrollPosition, float maxScrollPosition, float pageScrollSize)
		{
#if DEBUG
			if (true)
			{
				Debug.Log(
				string.Format(
					"SetScrollProperties: pageSize: {0}, minScrollPosition: {1}, maxScrollPosition: {2}, pageScrollSize: {3}",
					pageSize, minScrollPosition, maxScrollPosition, pageScrollSize));
			}
#endif

			PageSize = pageSize;

			PageScrollSize = (pageScrollSize > 0) ? pageScrollSize : pageSize;

			Minimum = Math.Max(minScrollPosition, 0);
			Maximum = Math.Max(maxScrollPosition, 0);

			Value = Math.Max(Minimum, Value);
			Value = Math.Min(Maximum, Value);

			// If the ScrollBar is enabled and has a nonzero range ...
			if (Maximum - Minimum > 0 && Enabled)
			{
				IncrementButton.Enabled = true;
				DecrementButton.Enabled = true;
				Track.Enabled = true;

				//Track.AddEventListener(MouseEvent.MOUSE_DOWN, ScrollTrackMouseDownHandler);
				//Track.AddEventListener(MouseEvent.MOUSE_OVER, ScrollTrackMouseOverHandler);
				//Track.AddEventListener(MouseEvent.MOUSE_OUT, ScrollTrackMouseOutHandler);

				//if (null == Thumb)
				//{
				//    ScrollThumb = new ScrollThumb
				//    {
				//        Id = "thumb",
				//        FocusEnabled = false,
				//        Padding = 0,
				//        ResizeWithStyleBackground = true
				//    };

				//    ScrollThumb.SetStyle("buttonStyle",
				//        Direction == Direction.Vertical ? _verticalScrollbarThumbStyle : _horizontalScrollbarThumbStyle);
				//    //ScrollThumb.ValidateProperties();

				//    // Add the thumb above the up arrow but below the down arrow
				//    AddChildAt(ScrollThumb, GetChildIndex(DownArrow));
				//}

				//if (Direction == Direction.Vertical)
				//{
				//    float thumbHeight = (float)(TrackHeight < 0 ? 0 : Math.Round(pageSize / (Maximum - Minimum + pageSize) * TrackHeight));

				//    if (thumbHeight < ScrollThumb.MinHeight)
				//    {
				//        if (TrackHeight < ScrollThumb.MinHeight)
				//        {
				//            ScrollThumb.Visible = false;
				//        }
				//        else
				//        {
				//            //thumbHeight = ScrollThumb.MinHeight;
				//            ScrollThumb.Visible = true;
				//            ScrollThumb.SetActualSize(ScrollThumb.MeasuredWidth, ScrollThumb.MinHeight);
				//        }
				//    }
				//    else
				//    {
				//        ScrollThumb.Visible = true;
				//        ScrollThumb.SetActualSize(ScrollThumb.MeasuredWidth, thumbHeight);
				//    }

				//    ScrollThumb.SetRange(UpArrow.GetExplicitOrMeasuredHeight() + 0,
				//                         Height -
				//                         DownArrow.GetExplicitOrMeasuredHeight() -
				//                         ScrollThumb.Height,
				//                         _minScrollPosition,
				//                         _maxScrollPosition);
				//}
				//else // horizontal
				//{
					float thumbWidth = (float)(TrackHeight < 0 ? 0 : Math.Round(pageSize / (Maximum - Minimum + pageSize) * TrackHeight));

					if (thumbWidth < Thumb.MinWidth)
					{
						if (TrackHeight < Thumb.MinWidth)
						{
							Thumb.Visible = false;
						}
						else
						{
							//thumbWidth = ScrollThumb.MinWidth;
							Thumb.Visible = true;
							Thumb.SetActualSize(Thumb.MinWidth, Thumb.MeasuredHeight);
						}
					}
					else
					{
						Thumb.Visible = true;
						Thumb.SetActualSize(thumbWidth, Thumb.MeasuredHeight);
					}

					//Thumb.SetRange(DecrementButton.GetExplicitOrMeasuredWidth(), // + 0,
					//                     Width -
					//                     IncrementButton.GetExplicitOrMeasuredWidth() -
					//                     Thumb.Width,
					//                     Minimum,
					//                     Maximum);
				//}

				Value = Math.Max(Math.Min(Value, Maximum), Minimum);
			}
			else
			{
				IncrementButton.Enabled = false;
				DecrementButton.Enabled = false;
				Track.Enabled = false;

				if (null != Thumb)
					Thumb.Visible = false;
			}
		}*/

		//private void LineScroll(int direction)
		//{
		//    Debug.Log("LineScroll: " + direction);
		//    float delta = _lineScrollSize;
		//    //Debug.Log(" *** " + _lineScrollSize);

		//    float newPos = Value + direction * delta;
		//    if (newPos > Maximum)
		//        newPos = Maximum;
		//    else if (newPos < Minimum)
		//        newPos = Minimum;

		//    if (newPos != Value)
		//    {
		//        float oldPosition = Value;
		//        Value = newPos;
		//        string detail = direction < 0 ? LineMinusDetail : LinePlusDetail;
		//        DispatchScrollEvent(oldPosition, detail);
		//    }
		//}

		/*private Direction Direction
		{
			get
			{
				return this is VScrollBar ? Direction.Vertical : Direction.Horizontal;
			}
		}

		private string LineMinusDetail
		{
			get
			{
				return Direction == Direction.Vertical ?
				   ScrollEventDetail.LINE_UP :
				   ScrollEventDetail.LINE_LEFT;
			}
		}

		private string LinePlusDetail
		{
			get
			{
				return Direction == Direction.Vertical ?
				   ScrollEventDetail.LINE_DOWN :
				   ScrollEventDetail.LINE_RIGHT;
			}
		}*/

		//internal void DispatchScrollEvent(float oldPosition, string detail)
		//{
		//    ScrollEvent e = new ScrollEvent(ScrollEvent.SCROLL)
		//    {
		//        Detail = detail,
		//        Position = Value,
		//        Delta = Value - oldPosition,
		//        Direction = Direction
		//    };
		//    DispatchEvent(e);
		//}
		
		//----------------------------------
		//  Viewport
		//----------------------------------    

		private IViewport _viewport;
		
		///<summary>
        /// The Viewport controlled by this scrollbar
		///</summary>
		public virtual IViewport Viewport
		{
			get
			{
				return _viewport;
			}
			set
			{
				if (value == _viewport)
					return;

				DisplayListMember dlm = (DisplayListMember) _viewport;
					
				if (null != dlm)  // old _viewport
				{
					dlm.RemoveEventListener(PropertyChangeEvent.PROPERTY_CHANGE, ViewportPropertyChangeHandler);
					dlm.RemoveEventListener(ResizeEvent.RESIZE, ViewportResizeHandler);
					_viewport.ClipAndEnableScrolling = false;
				}

				_viewport = value;
				dlm = (DisplayListMember)_viewport;

				if (null != dlm)  // new _viewport
				{
					dlm.AddEventListener(PropertyChangeEvent.PROPERTY_CHANGE, ViewportPropertyChangeHandler);
					dlm.AddEventListener(ResizeEvent.RESIZE, ViewportResizeHandler);
					_viewport.ClipAndEnableScrolling = true;
				}
			}
		}

		//---------------------------------
		// Viewport property changes
		//---------------------------------
		 
		private void ViewportPropertyChangeHandler(Event e)
		{
			PropertyChangeEvent pce = (PropertyChangeEvent) e;
			//Debug.Log("ViewportPropertyChangeHandler: " + pce.Property);
			switch (pce.Property) 
			{
				case "contentWidth": 
					ViewportContentWidthChangeHandler(e);
					break;
					
				case "contentHeight": 
					ViewportContentHeightChangeHandler(e);
					break;
					
				case "horizontalScrollPosition":
					ViewportHorizontalScrollPositionChangeHandler(e);
					break;

				case "verticalScrollPosition":
					ViewportVerticalScrollPositionChangeHandler(e);
					break;
			}
		}

		/**
		*  
		*  Called when the Viewport's width or height value changes. Does nothing by default.
		*/
		internal virtual void ViewportResizeHandler(Event e)
		{
		}
		
	   /**
		*  
		*  Called when the Viewport's <code>contentWidth</code> value changes. Does nothing by default.
		*/
		internal virtual void ViewportContentWidthChangeHandler(Event e)
		{
		}
		
		/**
		 *   
		 *  Called when the Viewport's <code>contentHeight</code> value changes. Does nothing by default.
		 */
		internal virtual void ViewportContentHeightChangeHandler(Event e)
		{
		}
		
		/**
		 *  
		 *  Called when the Viewport's <code>horizontalScrollPosition</code> value changes. Does nothing by default.
		 */
		internal virtual void ViewportHorizontalScrollPositionChangeHandler(Event e)
		{
		}  
		
		/**
		 *  
		 *  Called when the Viewport's <code>verticalScrollPosition</code> value changes. Does nothing by default. 
		 */
		internal virtual void ViewportVerticalScrollPositionChangeHandler(Event e)
		{
		}

		public override bool Enabled
		{ // moje, zbog skin statea
			get
			{
				return base.Enabled;
			}
			set
			{
				if (value == base.Enabled)
					return;

				base.Enabled = value;
				if (null != Thumb)
					Thumb.Enabled = value;
				if (null != Track)
					Track.Enabled = value;
				if (null != IncrementButton)
					IncrementButton.Enabled = value;
				if (null != DecrementButton)
					DecrementButton.Enabled = value;
			}
		}
	}
}
