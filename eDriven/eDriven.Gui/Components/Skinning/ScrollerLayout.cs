using System;
using eDriven.Core.Geom;
using eDriven.Gui.Containers;
using eDriven.Gui.Layout;
using UnityEngine;

namespace eDriven.Gui.Components
{
	internal class ScrollerLayout : LayoutBase
	{
		/**
		 *  
		 *  SDT - Scrollbar Display Threshold.  If the content size exceeds the
		 *  Viewport's size by SDT, then we show a scrollbar.  For example, if the 
		 *  contentWidth >= Viewport width + SDT, show the horizontal scrollbar.
		 */
// ReSharper disable InconsistentNaming
		internal const float SDT = 1.0f;
// ReSharper restore InconsistentNaming

		/**
		 *  
		 *  Used by updateDisplayList() to prevent looping.
		 */
		private int _invalidationCount;
		
		/**
		 *  
		 */
		private Scroller GetScroller()
		{
			//Skin g = Target as Skin; // TODO: check this
			Scroller scroller = (null != Target.Owner) ? 
				Target.Owner as Scroller : 
				null;

			//Debug.Log("***** scroller: " + scroller);
			return scroller;
		}

		/**
		 *  
		 *  Returns the Viewport's content size transformed into the Scroller's coordiante
		 *  system.   This makes it possible to compare the Viewport size (also reported
		 *  relative to the Scroller) and the content size when a transform has been applied
		 *  to the Viewport.
		 */
		private static Point GetLayoutContentSize(IViewport viewport)
		{
			float cw = viewport.ContentWidth;
			float ch = viewport.ContentHeight;
			if (cw == 0 && ch == 0 || null == cw || null == ch)
				return new Point(0,0);
			return new Point(cw, ch);
		}

		/**
		 *  
		 */
		private bool HsbVisible
		{
			get
			{
				ScrollBarBase hsb = GetScroller().HorizontalScrollBar;
				return null != hsb && hsb.Visible;
			}
			set
			{
				ScrollBarBase hsb = GetScroller().HorizontalScrollBar;
				if (null == hsb) // || hsb.Visible == value)
					return;

				hsb.IncludeInLayout = hsb.Visible = value;
				//hsb.Width = hsb.Height = 0;
				hsb.Enabled = value;
				hsb.MouseEnabled = value; // hide from inspector
				hsb.MouseChildren = value; // hide from inspector
			}
		}

		/**
		 *  
		 *  Returns the vertical space required by the horizontal scrollbar.   
		 *  That's the larger of the MinViewportInset and the hsb's preferred height.   
		 * 
		 *  Computing this value is complicated by the fact that if the HSB is currently 
		 *  HsbVisible=false, then it's scaleX,Y will be 0, and it's preferred size is 0.  
		 *  For that reason we specify postLayoutTransform=false to getPreferredBoundsHeight() 
		 *  and then multiply by the original scale factor, _hsbScaleY.
		 */
		private float HsbRequiredHeight()
		{
			Scroller scroller = GetScroller();
			float minViewportInset = scroller.MinViewportInset;
			ScrollBarBase hsb = scroller.HorizontalScrollBar;
			return Math.Max(minViewportInset, LayoutUtil.GetPreferredBoundsHeight(hsb));
		}

		/** --------------------------
		 * content      |   vscrollbar
		 * ---------------------------
		 * hscrollbar   |   whitebox
		 * --------------------------- */

		/**
		 *  
		 *  Return true if the specified dimensions provide enough space to layout 
		 *  the horizontal scrollbar (hsb) at its minimum size.   The HSB is assumed 
		 *  to be non-null and visible.
		 * 
		 *  If includeVSB is false we check to see if the HSB woudl fit if the 
		 *  VSB wasn't visible.
		 */
		private bool HsbFits(float w, float h, bool includeVsb)
		{
			if (VsbVisible && includeVsb)
			{
				ScrollBarBase vsb = GetScroller().VerticalScrollBar;
				w -= LayoutUtil.GetPreferredBoundsWidth(vsb);
				h -= LayoutUtil.GetMinBoundsHeight(vsb);
			}
			ScrollBarBase hsb = GetScroller().HorizontalScrollBar;
			return (w >= LayoutUtil.GetMinBoundsWidth(hsb)) && (h >= LayoutUtil.GetPreferredBoundsHeight(hsb));
		}

		//----------------------------------
		//  VsbVisible
		//----------------------------------    

		private float _vsbScaleX = 1;
		private float _vsbScaleY = 1;

		/**
		 *  
		 */
		private bool VsbVisible
		{
			get
			{
				ScrollBarBase vsb = GetScroller().VerticalScrollBar;
				return null != vsb && vsb.Visible;
			}
			set
			{
				ScrollBarBase vsb = GetScroller().VerticalScrollBar;
				if (null == vsb) // || vsb.Visible == value)
					return;

				vsb.IncludeInLayout = vsb.Visible = value;
				//vsb.Width = vsb.Height = 0;   
				vsb.Enabled = value;
				vsb.MouseEnabled = value; // hide from inspector
				vsb.MouseChildren = value; // hide from inspector
			}
		}

		/**
		 *  
		 *  Returns the vertical space required by the horizontal scrollbar.   
		 *  That's the larger of the MinViewportInset and the hsb's preferred height.  
		 *  
		 *  Computing this value is complicated by the fact that if the HSB is currently 
		 *  HsbVisible=false, then it's scaleX,Y will be 0, and it's preferred size is 0.  
		 *  For that reason we specify postLayoutTransform=false to getPreferredBoundsWidth() 
		 *  and then multiply by the original scale factor, _vsbScaleX.
		 */
		private float VsbRequiredWidth()
		{
			Scroller scroller = GetScroller();
			float minViewportInset = scroller.MinViewportInset;
			ScrollBarBase vsb = scroller.VerticalScrollBar;
			float sx = (VsbVisible) ? 1 : _vsbScaleX;
			return Math.Max(minViewportInset, LayoutUtil.GetPreferredBoundsWidth(vsb) * sx); //vsb.getPreferredBoundsWidth(VsbVisible) * sx);
		}

		/**
		 *  
		 *  Return true if the specified dimensions provide enough space to layout 
		 *  the vertical scrollbar (vsb) at its minimum size.   The VSB is assumed 
		 *  to be non-null and visible.
		 * 
		 *  If includeHSB is false, we check to see if the VSB would fit if the 
		 *  HSB wasn't visible.
		 */
		private bool VsbFits(float w, float h, bool includeHsb)
		{
			if (HsbVisible && includeHsb)
			{
				ScrollBarBase hsb = GetScroller().HorizontalScrollBar;
				w -= LayoutUtil.GetMinBoundsWidth(hsb);
				h -= LayoutUtil.GetPreferredBoundsHeight(hsb);
			}
			ScrollBarBase vsb = GetScroller().VerticalScrollBar;
			return (w >= LayoutUtil.GetPreferredBoundsWidth(vsb)) && (h >= LayoutUtil.GetMinBoundsHeight(vsb));
		}

		////----------------------------------
		////  canScrollHorizontally
		////----------------------------------  
		
		///**
		// *  
		// */
		//private bool _canScrollHorizontally;
		
		///**
		// *  
		// *  Helper function to determine whether the Viewport scrolls horizontally.
		// * 
		// *  <p>This is used for touch scrolling purposes to 
		// *  determine if one can scroll horizontally.</p>
		// * 
		// *  <p>The value is set in updateDisplayList()</p>
		// */
		//internal bool canScrollHorizontally
		//{
		//    get
		//    {
		//        return _canScrollHorizontally;
		//    }
		//}
		
		////----------------------------------
		////  canScrollVertically
		////----------------------------------  
		
		///**
		// *  
		// */
		//private bool _canScrollVertically;
		
		///**
		// *  
		// *  Helper function to determine whether the Viewport scrolls vertically.
		// * 
		// *  <p>This is used for touch scrolling purposes to 
		// *  determine if one can scroll vertically.</p>
		// * 
		// *  <p>The value is set in updateDisplayList()</p>
		// */
		//internal bool canScrollVertically
		//{
		//    get
		//    {
		//        return _canScrollVertically;
		//    }
		//}

		//--------------------------------------------------------------------------
		//
		//  Overidden Methods
		//
		//--------------------------------------------------------------------------
		
		/**
		 * 
		 *  Computes the union of the preferred size of the visible scrollbars 
		 *  and the Viewport if target.measuredSizeIncludesScrollbars=true, otherwise
		 *  it's just the preferred size of the Viewport.
		 * 
		 *  This becomes the ScrollerSkin's measuredWidth,Height.
		 *    
		 *  The Viewport does not contribute to the minimum size unless its
		 *  explicit size has been set.
		 */
		override internal void Measure()
		{
			Scroller scroller = GetScroller();
			if (null == scroller) 
				return;

			float minViewportInset = scroller.MinViewportInset;

//            var mode = scroller.GetStyle("interactionMode");
//            bool isMouseMode = null == mode || (InteractionMode)mode == InteractionMode.Mouse;

			bool measuredSizeIncludesScrollBars = scroller.MeasuredSizeIncludesScrollBars; // && isMouseMode;

			float measuredW = minViewportInset;
			float measuredH = minViewportInset;
			
			ScrollBarBase hsb = scroller.HorizontalScrollBar;
			bool showHsb = false;
			bool hAuto = false;
			if (measuredSizeIncludesScrollBars) {
				switch((ScrollPolicy)scroller.GetStyle("horizontalScrollPolicy")) 
				{
					case ScrollPolicy.On: 
						if (null != hsb) showHsb = true; 
						break;
					case ScrollPolicy.Auto: 
						if (null != hsb) showHsb = hsb.Visible;
						hAuto = true;
						break;
				}
			}

			ScrollBarBase vsb = scroller.VerticalScrollBar;
			bool showVsb = false;
			bool vAuto = false;
			if (measuredSizeIncludesScrollBars) {
				switch((ScrollPolicy)scroller.GetStyle("verticalScrollPolicy")) 
				{
				   case ScrollPolicy.On: 
						if (null != vsb) showVsb = true; 
						break;
					case ScrollPolicy.Auto: 
						if (null != vsb) showVsb = vsb.Visible;
						vAuto = true;
						break;
				}
			}

			measuredH += (showHsb) ? HsbRequiredHeight() : minViewportInset;
			measuredW += (showVsb) ? VsbRequiredWidth() : minViewportInset;

			// The measured size of the Viewport is just its preferredBounds, except:
			// don't give up space if doing so would make an auto scrollbar visible.
			// In other words, if an auto scrollbar isn't already showing, and using
			// the preferred size would force it to show, and the current size would not,
			// then use its current size as the measured size.  Note that a scrollbar
			// is only shown if the content size is greater than the Viewport size 
			// by at least SDT.

			IViewport viewport = scroller.Viewport;
			if (null != viewport)
			{
				if (measuredSizeIncludesScrollBars)
				{
					Point contentSize = GetLayoutContentSize(viewport);
		
					float viewportPreferredW =  LayoutUtil.GetPreferredBoundsWidth((InvalidationManagerClient) viewport);
					float viewportContentW = contentSize.X;
					float viewportW = LayoutUtil.GetLayoutBoundsWidth((InvalidationManagerClient) viewport);  // "current" size
					bool currentSizeNoHsb = null != viewportW && ((viewportW + SDT) > viewportContentW);
					if (hAuto && !showHsb && ((viewportPreferredW + SDT) <= viewportContentW) && currentSizeNoHsb)
						measuredW += viewportW;
					else
						measuredW += Math.Max(viewportPreferredW, (showHsb) ? LayoutUtil.GetMinBoundsWidth(hsb) : 0);
		
					float viewportPreferredH = LayoutUtil.GetPreferredBoundsHeight((InvalidationManagerClient) viewport);
					float viewportContentH = contentSize.Y;
					float viewportH = LayoutUtil.GetLayoutBoundsHeight((InvalidationManagerClient) viewport);  // "current" size
					bool currentSizeNoVsb = null != viewportH && ((viewportH + SDT) > viewportContentH);
					if (vAuto && !showVsb && ((viewportPreferredH + SDT) <= viewportContentH) && currentSizeNoVsb)
						measuredH += viewportH;
					else
						measuredH += Math.Max(viewportPreferredH, (showVsb) ? LayoutUtil.GetMinBoundsHeight(vsb) : 0);
				}
				else
				{
					measuredW += LayoutUtil.GetPreferredBoundsWidth((InvalidationManagerClient) viewport);
					measuredH += LayoutUtil.GetPreferredBoundsHeight((InvalidationManagerClient) viewport);
				}
			}

			float minW = minViewportInset * 2;
			float minH = minViewportInset * 2;

			// If the Viewport's explicit size is set, then 
			// include that in the scroller's minimum size

			Component component = viewport as Component;
			float? explicitViewportW = (null != component) ? component.ExplicitWidth : null;
			float? explicitViewportH = (null != component) ? component.ExplicitHeight : null;

			if (null != explicitViewportW)
				minW += (float)explicitViewportW;

			if (null != explicitViewportH)
				minH += (float)explicitViewportH;

			GroupBase g = Target;
			g.MeasuredWidth = Mathf.Ceil(measuredW);
			g.MeasuredHeight = Mathf.Ceil(measuredH);
			g.MeasuredMinWidth = Mathf.Ceil(minW);
			g.MeasuredMinHeight = Mathf.Ceil(minH);
		}

		/** 
		 *  
		 *  Arrange the Viewport and scrollbars conventionally within
		 *  the specified width and height: vertical scrollbar on the 
		 *  right, horizontal scrollbar along the bottom.
		 * 
		 *  Scrollbars for which the corresponding scrollPolicy=auto 
		 *  are made visible if the Viewport's content size is bigger 
		 *  than the actual size.   This introduces the possibility of
		 *  validateSize,DisplayList() looping because the measure() 
		 *  method computes the size of the Viewport and the currently
		 *  visible scrollbars. 
		 * 
		 */
		override internal void UpdateDisplayList(float w, float h)
		{
			//Debug.Log("UpdateDisplayList: " + w + ", " + h);

			Scroller scroller = GetScroller();
			if (null == scroller) 
				return;

			IViewport viewport = scroller.Viewport;
			ScrollBarBase hsb = scroller.HorizontalScrollBar;
			ScrollBarBase vsb = scroller.VerticalScrollBar;
			float minViewportInset = scroller.MinViewportInset;
			
			float contentW = 0;
			float contentH = 0;
			if (null != viewport)
			{
				Point contentSize = GetLayoutContentSize(viewport);
				contentW = contentSize.X;
				contentH = contentSize.Y;
			}
		
			// If the Viewport's size has been explicitly set (not typical) then use it
			// The initial values for viewportW,H are only used to decide if auto scrollbars
			// should be shown. 
	 
			Component viewportUIC = viewport as Component;
			//Debug.Log("viewportUIC: " + viewportUIC);
			//Debug.Log("viewportUIC.ExplicitWidth: " + viewportUIC.ExplicitWidth);
			float? explicitViewportW = null != viewportUIC ? viewportUIC.ExplicitWidth : null;
			float? explicitViewportH = null != viewportUIC ? viewportUIC.ExplicitHeight : null;

			//Debug.Log("explicitViewportW: " + explicitViewportW + "; explicitViewportH: " + explicitViewportH);

			float? viewportW = (null == explicitViewportW) ? (w - (minViewportInset * 2)) : explicitViewportW; // TODO: 0?
			float? viewportH = (null == explicitViewportH) ? (h - (minViewportInset * 2)) : explicitViewportH; // TODO: 0?

			//Debug.Log("viewportW: " + viewportW + "; viewportH: " + viewportH);
							
			// Decide which scrollbars will be visible based on the Viewport's content size
			// and the scroller's scroll policies.  A scrollbar is shown if the content size 
			// greater than the Viewport's size by at least SDT.
			
			bool oldShowHSB = HsbVisible;
			bool oldShowVSB = VsbVisible;
			
			bool hAuto = false;
			//bool hsbTakeUpSpace = true; // if visible
			switch((ScrollPolicy)scroller.GetStyle("horizontalScrollPolicy")) 
			{
				case ScrollPolicy.On: 
					//_canScrollHorizontally = true;
					HsbVisible = true;
					break;

				case ScrollPolicy.Auto: 
					if (null != hsb && null != viewport)
					{
						hAuto = true;
						// = (contentW >= (viewportW + SDT));
						//HsbVisible = (null != hsb && _canScrollHorizontally);
						HsbVisible = (contentW >= (viewportW + SDT));
					} 
					break;
				
				default:
					//_canScrollHorizontally = false;
					HsbVisible = false;
					break;
			}

			bool vAuto = false;
			//bool vsbTakeUpSpace = true; // if visible
			switch((ScrollPolicy)scroller.GetStyle("verticalScrollPolicy")) 
			{
			   case ScrollPolicy.On: 
					//_canScrollVertically = true;
					VsbVisible = true;
					break;

				case ScrollPolicy.Auto:
					if (null != vsb && null != viewport)
					{ 
						vAuto = true;
						//_canScrollVertically = (contentH >= (viewportH + SDT));
						//VsbVisible = (null != vsb && _canScrollVertically);
						VsbVisible = (contentH >= (viewportH + SDT));
					}                        
					break;
				
				default:
					//_canScrollVertically = false;
					VsbVisible = false;
					break;
			}
			
//            // if in touch mode, only show scrollbars if a scroll is currently in progress
//            if ((InteractionMode)scroller.GetStyle("interactionMode") == InteractionMode.Touch)
//            {
//                hsbTakeUpSpace = false;
//                HsbVisible = scroller.horizontalScrollInProgress;
//                
//                vsbTakeUpSpace = false;
//                VsbVisible = scroller.verticalScrollInProgress;
//            }

			// Reset the Viewport's width,height to account for the visible scrollbars, unless
			// the Viewport's size was explicitly set, then we just use that. 

			if (null == explicitViewportW) // || 0 == explicitViewportW) // TODO: 0? // if (null == explicitViewportW)
				viewportW = w - ((VsbVisible) ? (minViewportInset + VsbRequiredWidth()) : (minViewportInset * 2));
			else 
				viewportW = explicitViewportW;

			//Debug.Log("w: " + w);
			//Debug.Log("viewportW: " + viewportW);

			if (null == explicitViewportH) // || 0 == explicitViewportH) // TODO: 0? // if (null == explicitViewportH)
				viewportH = h - ((HsbVisible) ? (minViewportInset + HsbRequiredHeight()) : (minViewportInset * 2));
			else 
				viewportH = explicitViewportH;

			//Debug.Log("h: " + h);
			//Debug.Log("viewportH: " + viewportH);

			// If the scrollBarPolicy is auto, and we're only showing one scrollbar, 
			// the Viewport may have shrunk enough to require showing the other one.
			
			bool hsbIsDependent = false;
			bool vsbIsDependent = false;
			
			if (VsbVisible && !HsbVisible && hAuto && (contentW >= (viewportW + SDT)))
				HsbVisible = hsbIsDependent = true;
			else if (!VsbVisible && HsbVisible && vAuto && (contentH >= (viewportH + SDT)))
				VsbVisible = vsbIsDependent = true;

			// If the HSB doesn't fit, hide it and give the space back.   Likewise for VSB.
			// If both scrollbars are supposed to be visible but they don't both fit, 
			// then prefer to show the "non-dependent" auto scrollbar if we added the second
			// "dependent" auto scrollbar because of the space consumed by the first.
			
			if (HsbVisible && VsbVisible) 
			{
				if (HsbFits(w, h, true) && VsbFits(w, h, true))
				{
					// Both scrollbars fit, we're done.
				}
				else if (!HsbFits(w, h, false) && !VsbFits(w, h, false))
				{
					// Neither scrollbar would fit, even if the other scrollbar wasn't visible.
					HsbVisible = false;
					VsbVisible = false;
				}
				else
				{
					// Only one of the scrollbars will fit.  If we're showing a second "dependent"
					// auto scrollbar because the first scrollbar consumed enough space to
					// require it, if the first scrollbar doesn't fit, don't show either of them.

					if (hsbIsDependent)
					{
						if (VsbFits(w, h, false))  // VSB will fit if HSB isn't shown   
							HsbVisible = false;
						else 
							VsbVisible = HsbVisible = false;
	  
					}
					else if (vsbIsDependent)
					{
						if (HsbFits(w, h, false)) // HSB will fit if VSB isn't shown
							VsbVisible = false;
						else
							HsbVisible = VsbVisible = false; 
					}
					else if (VsbFits(w, h, false)) // VSB will fit if HSB isn't shown
						HsbVisible = false;
					else // HsbFits(w, h, false)   // HSB will fit if VSB isn't shown
						VsbVisible = false;
				}
			}
			else if (HsbVisible && !HsbFits(w, h, true))  // just trying to show HSB, but it doesn't fit
				HsbVisible = false;
			else if (VsbVisible && !VsbFits(w, h, true))  // just trying to show VSB, but it doesn't fit
				VsbVisible = false;
			
			// Reset the Viewport's width,height to account for the visible scrollbars, unless
			// the Viewport's size was explicitly set, then we just use that.

			if (null == explicitViewportW) // || 0 == explicitViewportW) // TODO: 0? // if (null == explicitViewportW)
				viewportW = w - ((VsbVisible) ? (minViewportInset + VsbRequiredWidth()) : (minViewportInset * 2));
			else 
				viewportW = explicitViewportW;

			if (null == explicitViewportH) // || 0 == explicitViewportH) // TODO: 0? // if (null == explicitViewportH)
				viewportH = h - ((HsbVisible) ? (minViewportInset + HsbRequiredHeight()) : (minViewportInset * 2));
			else 
				viewportH = explicitViewportH;
			
			// Layout the Viewport and scrollbars.

			if (null != viewport)
			{
				//Debug.Log("Setting Viewport size: " + viewportW + ", " + viewportH);
				LayoutUtil.SetLayoutBoundsSize((InvalidationManagerClient) viewport, viewportW, viewportH); //null, null); // 20131026
				//Debug.Log("Setting Viewport position: " + minViewportInset + ", " + minViewportInset);
				LayoutUtil.SetLayoutBoundsPosition((InvalidationManagerClient) viewport, minViewportInset, minViewportInset);
				
				// TODO: examine why this doesn't work without going explicit!
				//((Component)viewport).Width = (float)viewportW; //TEMP
				//((Component) viewport).Height = (float) viewportH; //TEMP
				//((Component)viewport).Transform.Apply();
			}
			
			if (HsbVisible)
			{
				float hsbW = (VsbVisible) ? w - LayoutUtil.GetPreferredBoundsWidth(vsb) : w;
				float hsbH = LayoutUtil.GetPreferredBoundsHeight(hsb);
				LayoutUtil.SetLayoutBoundsSize(hsb, Math.Max(LayoutUtil.GetMinBoundsWidth(hsb), hsbW), hsbH);
				LayoutUtil.SetLayoutBoundsPosition(hsb, 0, h - hsbH);
			}

			if (VsbVisible)
			{
				float vsbW = LayoutUtil.GetPreferredBoundsWidth(vsb); 
				float vsbH = (HsbVisible) ? h - LayoutUtil.GetPreferredBoundsHeight(hsb) : h;
				LayoutUtil.SetLayoutBoundsSize(vsb, vsbW, Math.Max(LayoutUtil.GetMinBoundsHeight(vsb), vsbH));
				LayoutUtil.SetLayoutBoundsPosition(vsb, w - vsbW, 0);
			}

			// If we've added an auto scrollbar, then the measured size is likely to have been wrong.
			// There's a risk of looping here, so we count.  
			if ((_invalidationCount < 2) && (((VsbVisible != oldShowVSB) && vAuto) || ((HsbVisible != oldShowHSB) && hAuto)))
			{
				Target.InvalidateSize();
				
				// If the Viewport's layout is virtual, it's possible that its
				// measured size changed as a consequence of laying it out,
				// so we invalidate its size as well.
				GroupBase viewportGroup = viewport as GroupBase;
				//if (null != viewportGroup && null != viewportGroup.Layout && viewportGroup.Layout.UseVirtualLayout)
				//    viewportGroup.InvalidateSize();
				
				_invalidationCount += 1; 
			}
			else
				_invalidationCount = 0;
				 
			Target.SetContentSize(w, h);
		}
	}
}