using eDriven.Core.Geom;
using eDriven.Core.Managers;
using eDriven.Gui.Managers;
using UnityEngine;
using Event = eDriven.Core.Events.Event;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class PopUpAnchor : Component
    {
        /// <summary>
        /// 
        /// </summary>
        public PopUpAnchor()
        {
            AddEventListener(FrameworkEvent.ADDED_TO_STAGE, addedToStageHandler);
            AddEventListener(FrameworkEvent.REMOVE /*D_FROM_STAGE*/, removedFromStageHandler);
        }

        private float? _popUpWidth = 0;
        private float? _popUpHeight = 0;
    
        private bool _popUpIsDisplayed;
        private bool _addedToStage;

        private bool _popUpSizeCaptured;

        private bool _popupWidthMatchesAnchorWidth;
        /// <summary>
        /// 
        /// </summary>
        public bool PopupWidthMatchesAnchorWidth
        {
            get
            {
                return _popupWidthMatchesAnchorWidth;
            }
            set
            {
                if (value == _popupWidthMatchesAnchorWidth)
                    return;

                _popupWidthMatchesAnchorWidth = value;
                InvalidateDisplayList();
            }
        }

        private bool _popupHeightMatchesAnchorHeight;
        /// <summary>
        /// 
        /// </summary>
        public bool PopupHeightMatchesAnchorHeight
        {
            get { 
                return _popupHeightMatchesAnchorHeight;
            }
            set
            {
                if (value == _popupHeightMatchesAnchorHeight)
                    return;

                _popupHeightMatchesAnchorHeight = value;
                InvalidateDisplayList();
            }
        }

        private bool _displayPopup;
        /// <summary>
        /// 
        /// </summary>
        public bool DisplayPopup
        {
            get { 
                return _displayPopup;
            }
            set
            {
                if (value == _displayPopup)
                    return;

                _displayPopup = value;
                AddOrRemovePopup();
            }
        }
        
        private DisplayListMember _popup;
        /// <summary>
        /// 
        /// </summary>
        public DisplayListMember Popup
        {
            get { 
                return _popup;
            }
            set
            {
                if (value == _popup)
                    return;

                _popup = value;

                var popup = _popup as ISimpleStyleClient;
                /*if (popup != null)
                    popup.StyleName = this;*/ // TODO

                DispatchEvent(new Event("popUpChanged"));
            }
        }

        //----------------------------------
        //  popUpPosition
        //----------------------------------

        private PopupPosition _popupPosition = PopupPosition.TopLeft;
        /// <summary>
        /// 
        /// </summary>
        public PopupPosition PopupPosition
        {
            get { return _popupPosition; }
            set
            {
                if (value == _popupPosition)
                    return;

                _popupPosition = value;
                InvalidateDisplayList();    
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected override void UpdateDisplayList(float width, float height)
        {
            base.UpdateDisplayList(width, height);
            ApplyPopUpTransform(width, height);   
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdatePopUpTransform()
        {
            ApplyPopUpTransform(Width, Height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected Point CalculatePopUpPosition()
        {
            Point regPoint = new Point();
        
            /*if (!matrix)
                return regPoint;*/
        
            var popUpBounds = new Rectangle(); 
            var popUpAsDisplayObject = _popup as DisplayObject;
        
            DeterminePosition(_popupPosition, popUpAsDisplayObject.Width, popUpAsDisplayObject.Height,
                              /*matrix, */regPoint, popUpBounds);

            PopupPosition adjustedPosition = PopupPosition.Above;
            bool isAdjusted = false;

            var screenSize = SystemManager.Instance.ScreenSize;

            // Position the popUp in the opposite direction if it 
            // does not fit on the screen. 
            switch(_popupPosition)
            {
                case PopupPosition.Below :
                    if (popUpBounds.Bottom > screenSize.Y) { 
                        adjustedPosition = PopupPosition.Above;
                        isAdjusted = true;
                    }
                    break;
                case PopupPosition.Above :
                    if (popUpBounds.Top < 0)
                    {
                        adjustedPosition = PopupPosition.Below;
                        isAdjusted = true;
                    }
                    break;
                case PopupPosition.Left :
                    if (popUpBounds.Left < 0)
                    {
                        adjustedPosition = PopupPosition.Right;
                        isAdjusted = true;
                    }
                    break;
                case PopupPosition.Right :
                    if (popUpBounds.Right > screenSize.X)
                    {
                        adjustedPosition = PopupPosition.Left; 
                        isAdjusted = true;
                    }
                    break;
            }
        
            // Get the new registration point based on the adjusted position
            if (isAdjusted)
            {
                var adjustedRegPoint = new Point();
                var adjustedBounds = new Rectangle(); 
                DeterminePosition((PopupPosition) adjustedPosition, popUpAsDisplayObject.Width, 
                                  popUpAsDisplayObject.Height,
                                  /*matrix, */adjustedRegPoint, adjustedBounds);
         
                // If we adjusted the position but the popUp still doesn't fit, 
                // then revert to the original position. 
                switch(adjustedPosition)
                {
                    case PopupPosition.Below :
                        if (adjustedBounds.Bottom > screenSize.Y)
                            isAdjusted = false; 
                        break;
                    case PopupPosition.Above :
                        if (adjustedBounds.Top < 0)
                            isAdjusted = false; 
                        break;
                    case PopupPosition.Left :
                        if (adjustedBounds.Left < 0)
                            isAdjusted = false; 
                        break;
                    case PopupPosition.Right :
                        if (adjustedBounds.Right > screenSize.X)
                            isAdjusted = false;  
                        break;
                }

                if (isAdjusted)
                {
                    regPoint = adjustedRegPoint;
                    popUpBounds = adjustedBounds;
                }
            }
        
            /*MatrixUtil.decomposeMatrix(decomposition, matrix, 0, 0);
            var concatScaleX:Number = decomposition[3];
            var concatScaleY:Number = decomposition[4]; */
        
            // If the popUp still doesn't fit, then nudge it
            // so it is completely on the screen. Make sure to include scale.

            if (popUpBounds.Top < 0)
                regPoint.Y += (0 - popUpBounds.Top); // / concatScaleY;
            else if (popUpBounds.Bottom > screenSize.Y)
                regPoint.Y -= (popUpBounds.Bottom - screenSize.Y); // / concatScaleY;

            if (popUpBounds.Left < 0)
                regPoint.X += (0 - popUpBounds.Left); // / concatScaleX;    
            else if (popUpBounds.Right > screenSize.X)
                regPoint.X -= (popUpBounds.Right - screenSize.X);// / concatScaleX;

            return LocalToGlobal(regPoint);
            //return matrix.transformPoint(regPoint);
        }

        //--------------------------------------------------------------------------
        //
        //  Private Methods
        //
        //-------------------------------------------------------------------------- 

        /**
         *   
         */
        private void AddOrRemovePopup()
        {
            if (!_addedToStage)
                return;
        
            if (null == _popup)
                return;
                        
            if (_popup.Parent == null && _displayPopup)
            {
                PopupManager.Instance.AddPopup(_popup,this,false);
                _popUpIsDisplayed = true;
                var cmp = _popup as Component;
                if (null != cmp && !_popUpSizeCaptured)
                {
                    _popUpWidth = cmp.ExplicitWidth;
                    _popUpHeight = cmp.ExplicitHeight;
                    cmp.ValidateNow();
                    _popUpSizeCaptured = true;
                }   
            
                ApplyPopUpTransform(Width, Height);
            }
            else if (_popup.Parent != null && !DisplayPopup)
            {
                RemoveAndResetPopUp();
            }
        }
    
        /**
         *  
         */
        private void RemoveAndResetPopUp()
        {
            PopupManager.Instance.RemovePopup(_popup);
            _popUpIsDisplayed = false;
        }

        /**
         *   
         */
        internal void DeterminePosition(PopupPosition placement, float popUpWidth, float popUpHeight,
                                               /*matrix:Matrix, */Point registrationPoint, Rectangle bounds)
        {
            switch(placement)
            {
                case PopupPosition.Below:
                    registrationPoint.X = 0;
                    registrationPoint.Y = Height;
                    break;
                case PopupPosition.Above:
                    registrationPoint.X = 0;
                    registrationPoint.Y = -popUpHeight;
                    break;
                case PopupPosition.Left:
                    registrationPoint.X = -popUpWidth;
                    registrationPoint.Y = 0;
                    break;
                case PopupPosition.Right:
                    registrationPoint.X = Width;
                    registrationPoint.Y = 0;
                    break;            
                case PopupPosition.Center:
                    registrationPoint.X = (Width - popUpWidth) / 2;
                    registrationPoint.Y = (Height - popUpHeight) / 2;
                    break;            
                case PopupPosition.TopLeft:
                    // already 0,0
                    break;
            }
        
            //var popUpAsDisplayObject = _popup as DisplayObject;
                
            /*Point globalTl = (Point) registrationPoint.Clone(); //matrix.transformPoint(registrationPoint);
            registrationPoint.Y += popUpAsDisplayObject.Height;
            Point globalBl = (Point) registrationPoint.Clone(); //matrix.transformPoint(registrationPoint);
            registrationPoint.X += popUpAsDisplayObject.Width;
            Point globalBr = (Point) registrationPoint.Clone(); //matrix.transformPoint(registrationPoint);
            registrationPoint.Y -= popUpAsDisplayObject.Height;
            Point globalTr = (Point) registrationPoint.Clone(); //matrix.transformPoint(registrationPoint);
            registrationPoint.X -= popUpAsDisplayObject.Width;
        
            bounds.Left = Mathf.Min(globalTl.X, globalBl.X, globalBr.X, globalTr.X);
            bounds.Right = Mathf.Max(globalTl.X, globalBl.X, globalBr.X, globalTr.X);
            bounds.Top = Mathf.Min(globalTl.Y, globalBl.Y, globalBr.Y, globalTr.Y);
            bounds.Bottom = Mathf.Max(globalTl.Y, globalBl.Y, globalBr.Y, globalTr.Y);*/

            var globalRegPoint = Parent.LocalToGlobal(registrationPoint);
            bounds.X = globalRegPoint.X;
            bounds.Y = globalRegPoint.Y;
            bounds.Width = popUpWidth;
            bounds.Height = popUpHeight;
        }

        /**
         *   
         */ 
        private void ApplyPopUpTransform(float width, float height)
        {
            if (!_popUpIsDisplayed)
                return;
                
            //var m:Matrix = MatrixUtil.getConcatenatedMatrix(this);
         
            // Set the dimensions explicitly because UIComponents always set themselves to their
            // measured / explicit dimensions if they are parented by the SystemManager. 

            var cmp = _popup as Component;

            if (null != cmp)
            {
                if (_popupWidthMatchesAnchorWidth)
                    cmp.Width = width;
                else
                    cmp.ExplicitWidth = _popUpWidth;
            
                if (_popupHeightMatchesAnchorHeight)
                    cmp.Height = height;
                else
                    cmp.ExplicitHeight = _popUpHeight;

                //popUp.setActualSize(w, h);
            }

            var popupPoint = CalculatePopUpPosition();
            _popup.X = popupPoint.X;
            _popup.Y = popupPoint.Y;

            /*else
            {
                var w:Number = popUpWidthMatchesAnchorWidth ? Width : popUp.measuredWidth;
                var h:Number = popUpHeightMatchesAnchorHeight ? Height : popUp.measuredHeight;
                popUp.setActualSize(w, h);
            }*/
        }

        //--------------------------------------------------------------------------
        //
        //  Event Handlers
        //
        //-------------------------------------------------------------------------- 
    
        /**
         *   
         */ 
        private void addedToStageHandler(Event e)
        {
            _addedToStage = true;
            AddOrRemovePopup();    
        }
    
        /**
         *   
         */
        private void removedFromStageHandler(Event e)
        {
            if (null != _popup && (_popup).Parent != null)
                RemoveAndResetPopUp();
        
            _addedToStage = false;
        }
    }
}