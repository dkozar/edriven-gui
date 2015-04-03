using System;
using System.Collections.Generic;
using eDriven.Core.Events;
using eDriven.Gui.Containers;
using eDriven.Gui.Reflection;
using eDriven.Gui.Events;
using eDriven.Gui.Styles;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Components
{
    [Event(Name = "viewportChanged", Type = typeof(Event))]

    [Style(Name = "skinClass", Default = typeof(ScrollerSkin))]
    [Style(Name = "horizontalScrollPolicy", Type = typeof(ScrollPolicy), Default = ScrollPolicy.Auto)] // Auto
    [Style(Name = "verticalScrollPolicy", Type = typeof(ScrollPolicy), Default = ScrollPolicy.Auto)] // Auto
    [Style(Name = "interactionMode", Type = typeof(InteractionMode), Default = InteractionMode.Mouse)]

    public class Scroller : SkinnableComponent, IContentChildList
    {
        ///<summary>
        /// Constructor
        ///</summary>
        public Scroller()
        {
            FocusEnabled = false;
        }

        //--------------------------------------------------------------------------
        //
        //  Properties
        //
        //--------------------------------------------------------------------------
        
        private void InvalidateSkin()
        {
            if (null != Skin)
            {
                Skin.InvalidateSize();
                Skin.InvalidateDisplayList();
            }
        }

        //----------------------------------
        //  HorizontalScrollBar
        //---------------------------------- 
        
        /// <summary>
        /// A skin part that defines the horizontal scroll bar.
        /// </summary>
        [SkinPart(Required=false)]
        public HScrollBar HorizontalScrollBar;
        
        //----------------------------------
        //  VerticalScrollBar
        //---------------------------------- 

        /// <summary>
        /// A skin part that defines the vertical scroll bar.
        /// </summary>
        [SkinPart(Required=false)]
        public VScrollBar VerticalScrollBar;

        #region Viewport

        private IViewport _viewport;

        /**
         *  The Viewport component to be scrolled.
         * 
         *  <p>
         *  The Viewport is added to the Scroller component's skin, 
         *  which lays out both the Viewport and scroll bars.
         * 
         *  When the <code>Viewport</code> property is set, the Viewport's 
         *  <code>clipAndEnableScrolling</code> property is 
         *  set to true to enable scrolling.
         * 
         *  The Scroller does not support rotating the Viewport directly.  The Viewport's
         *  contents can be transformed arbitrarily, but the Viewport itself cannot.
         * </p>
         */
        public IViewport Viewport
        {       
            get
            {
                return _viewport;
            }
            set
            {
                if (value == _viewport)
                    return;

                //Debug.Log("Setting Viewport: " + value);

                UninstallViewport();
                _viewport = value;
                InstallViewport();
                DispatchEvent(new Event("viewportChanged"));
            }
        }

        private void InstallViewport()
        {
            //Debug.Log("InstallViewport: " + this);
            if (null != Skin && null != Viewport)
            {
                Viewport.ClipAndEnableScrolling = true;
                DisplayListMember dlm = (DisplayListMember) Viewport;
                ((Group)Skin).AddContentChildAt(dlm, 0);
                dlm.AddEventListener(PropertyChangeEvent.PROPERTY_CHANGE, ViewportPropertyChangeHandler);
                InteractiveComponent ic = (InteractiveComponent) Viewport;
                ic.MouseEnabled = true; // because of mouse-wheeling!
            }
            
            if (null != VerticalScrollBar)
                VerticalScrollBar.Viewport = Viewport;
            if (null != HorizontalScrollBar)
                HorizontalScrollBar.Viewport = Viewport;
        }

        private void UninstallViewport()
        {
            if (null != HorizontalScrollBar)
                HorizontalScrollBar.Viewport = null;
            if (null != VerticalScrollBar)
                VerticalScrollBar.Viewport = null;
            if (null != Skin && null != Viewport)
            {
                Viewport.ClipAndEnableScrolling = false;
                DisplayListMember dlm = (DisplayListMember)Viewport;
                ((Group)Skin).RemoveContentChild(dlm);
                dlm.RemoveEventListener(PropertyChangeEvent.PROPERTY_CHANGE, ViewportPropertyChangeHandler);
            }
        }

        #endregion

        //----------------------------------
        //  MinViewportInset
        //----------------------------------

        private float _minViewportInset;

        /**
         *  The minimum space between the Viewport and the edges of the Scroller.  
         * 
         *  If neither of the scroll bars is visible, then the Viewport is inset by 
         *  <code>MinViewportInset</code> on all four sides.
         * 
         *  If a scroll bar is visible then the Viewport is inset by <code>MinViewportInset</code>
         *  or by the scroll bar's size, whichever is larger.
         * 
         *  ScrollBars are laid out flush with the edges of the Scroller.   
         * 
         *  Default: 0 
         */
        public float MinViewportInset
        {
            get
            {
                return _minViewportInset;
            }
            set
            {
                _minViewportInset = value;
                InvalidateSkin();
            }
        }

        //----------------------------------
        //  MeasuredSizeIncludesScrollBars
        //----------------------------------

        private bool _measuredSizeIncludesScrollBars = true;

        /**
         *  If <code>true</code>, the Scroller's measured size includes the space required for
         *  the visible scroll bars, otherwise the Scroller's measured size depends
         *  only on its Viewport.
         * 
         *  <p>Components like TextArea, which "reflow" their contents to fit the
         *  available width or height may use this property to stabilize their
         *  measured size.  By default a TextArea's is defined by its <code>widthInChars</code>
         *  and <code>heightInChars</code> properties and in many applications it's preferable
         *  for the measured size to remain constant, event when scroll bars are displayed
         *  by the TextArea skin's Scroller.</p>
         * 
         *  <p>In components where the content does not reflow, like a typical List's
         *  items, the default behavior is preferable because it makes it less
         *  likely that the component's content will be obscured by a scroll bar.</p>
         * 
         *  Default: true
         */
        public bool MeasuredSizeIncludesScrollBars
        {
            get
            {
                return _measuredSizeIncludesScrollBars;
            }
            set
            {
                if (value == _measuredSizeIncludesScrollBars)
                    return;

                _measuredSizeIncludesScrollBars = value;
                InvalidateSkin();
            }
        }

        //--------------------------------------------------------------------------
        // 
        // Event Handlers
        //
        //--------------------------------------------------------------------------
        
        private void ViewportPropertyChangeHandler(Event e)
        {
            PropertyChangeEvent pce = (PropertyChangeEvent) e;
            switch (pce.Property) 
            {
                case "contentWidth": 
                case "contentHeight": 
                    InvalidateSkin();
                    break;
            }
        }

        #region IVisualElementContainer

        /// <summary>
        /// Content group children
        /// </summary>
        public List<DisplayListMember> ContentChildren
        {
            get { throw new Exception("This operation is not supported in Scroller"); }
        }

        public int NumberOfContentChildren
        {
            get
            {
                return null != Viewport ? 1 : 0;
            }
        }

        /// <summary>
        /// Checks if the content group has a specified child
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public bool HasContentChild(DisplayListMember child)
        {
            throw new Exception("This operation is not supported in Scroller");
        }

        /// <summary>
        /// Checks if the content group has a specified descendant
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public bool ContentContains(DisplayListMember child)
        {
            throw new Exception("This operation is not supported in Scroller");
        }

        /// <summary>
        /// Checks if the content group has a specified descendant
        /// </summary>
        /// <param name="child"></param>
        /// <param name="includeThisCheck"></param>
        /// <returns></returns>
        public bool ContentContains(DisplayListMember child, bool includeThisCheck)
        {
            throw new Exception("This operation is not supported in Scroller");
        }

        /// <summary>
        /// Adds a content group child
        /// </summary>
        /// <param name="child">A child</param>
        public DisplayListMember AddContentChild(DisplayListMember child)
        {
            throw new Exception("This operation is not supported in Scroller");
        }

        /// <summary>
        /// Adds a content group child at specified index
        /// </summary>
        /// <param name="child">A child</param>
        /// <param name="index">Child index</param>
        public DisplayListMember AddContentChildAt(DisplayListMember child, int index)
        {
            throw new Exception("This operation is not supported in Scroller");
        }

        /// <summary>
        /// Removes a content group child
        /// </summary>
        /// <param name="child">A child</param>
        public DisplayListMember RemoveContentChild(DisplayListMember child)
        {
            throw new Exception("This operation is not supported in Scroller");
        }

        /// <summary>
        /// Removes a content group child at specified index
        /// </summary>
        public DisplayListMember RemoveContentChildAt(int index)
        {
            throw new Exception("This operation is not supported in Scroller");
        }

        /// <summary>
        /// Removes all content group children
        /// </summary>
        public void RemoveAllContentChildren()
        {
            throw new Exception("This operation is not supported in Scroller");
        }

        ///<summary>
        /// Swaps content group children
        ///</summary>
        ///<param name="firstChild">First child</param>
        ///<param name="secondChild">Second child</param>
        public void SwapContentChildren(DisplayListMember firstChild, DisplayListMember secondChild)
        {
            throw new Exception("This operation is not supported in Scroller");
        }

        /// <summary>
        /// Gets content group child at specified position
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Child index</returns>
        public DisplayListMember GetContentChildAt(int index)
        {
            if (null != Viewport && index == 0)
                return (DisplayListMember)Viewport;
            throw new Exception("Index out of range");
        }

        /// <summary>
        /// Gets content group child index
        /// </summary>
        /// <param name="child">A child</param>
        /// <returns>The position</returns>
        public int GetContentChildIndex(DisplayListMember child)
        {
            if (null != child && Viewport != child)
                return 0;
            throw new Exception("Child not found in scroller");
        }

        #endregion

        /**
         *  
         */
        public override void StyleChanged(string styleProp)
        {
            base.StyleChanged(styleProp);

            var allStyles = (styleProp == null || styleProp == "styleName");
            base.StyleChanged(styleProp);
            if (allStyles || styleProp == "horizontalScrollPolicy" || 
                styleProp == "verticalScrollPolicy")
            {
                InvalidateSkin();
            }
        }

        protected override void AttachSkin()
        {
            base.AttachSkin();
            
            ((Group)Skin).Layout = new ScrollerLayout();
            InstallViewport();
            Skin.AddEventListener(MouseEvent.MOUSE_WHEEL, SkinMouseWheelHandler);
        }

        protected override void DetachSkin()
        {
            UninstallViewport();
            ((Group)Skin).Layout = null;
            Skin.RemoveEventListener(MouseEvent.MOUSE_WHEEL, SkinMouseWheelHandler);
            base.DetachSkin();
        }

        protected override void PartAdded(string partName, object instance)
        {
            base.PartAdded(partName, instance);

            if (instance == VerticalScrollBar)
                VerticalScrollBar.Viewport = Viewport;

            else if (instance == HorizontalScrollBar)
                HorizontalScrollBar.Viewport = Viewport;
        }

        protected override void PartRemoved(string partName, object instance)
        {
            base.PartRemoved(partName, instance);

            if (instance == VerticalScrollBar)
                VerticalScrollBar.Viewport = null;

            else if (instance == HorizontalScrollBar)
                HorizontalScrollBar.Viewport = null;
        }

        private void SkinMouseWheelHandler(Event e)
        {
            MouseEvent me = (MouseEvent) e;
            IViewport vp = Viewport;
            if (e.DefaultPrevented || null == vp || !vp.Visible)
                return;

            var delta = me.CurrentEvent.delta.y;
            int nSteps = (int) Math.Abs(delta);
            NavigationUnit navigationUnit;

            // Scroll event.delta "steps".  If the VSB is up, scroll vertically,
            // if -only- the HSB is up then scroll horizontally.
             
            if (null != VerticalScrollBar && VerticalScrollBar.Visible)
            {
                navigationUnit = (delta < 0) ? NavigationUnit.Down : NavigationUnit.Up;
                for (int vStep = 0; vStep < nSteps; vStep++)
                {
                    float? vspDelta = vp.GetVerticalScrollPositionDelta(navigationUnit);
                    //if (null != vspDelta)
                        vp.VerticalScrollPosition += (float)vspDelta;
                }
                e.PreventDefault();
            }
            else if (null != HorizontalScrollBar && HorizontalScrollBar.Visible)
            {
                navigationUnit = (delta < 0) ? NavigationUnit.Right : NavigationUnit.Left;
                for (int hStep = 0; hStep < nSteps; hStep++)
                {
                    float hspDelta = vp.GetHorizontalScrollPositionDelta(navigationUnit);
                    //if (null != hspDelta)
                        vp.HorizontalScrollPosition += hspDelta;
                }
                e.PreventDefault();
            }
        }

    }
}