using System.Collections.Generic;
using eDriven.Core.Events;
using eDriven.Core.Managers;
using eDriven.Gui.Events;
using eDriven.Gui.Managers;
using UnityEngine;
using Event = eDriven.Core.Events.Event;
using Timer = eDriven.Core.Util.Timer;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// The DropDownController class handles the mouse, keyboard, and focus interactions for an anchor button and its associated drop down
    /// </summary>
    internal class DropDownController : EventDispatcher
    {
        //--------------------------------------------------------------------------
        //
        //  Variables
        //
        //--------------------------------------------------------------------------

        private bool _mouseIsDown;

        //--------------------------------------------------------------------------
        //
        //  Properties
        //
        //--------------------------------------------------------------------------
    
        //----------------------------------
        //  openButton
        //----------------------------------
    
        private Button _openButton;

        public Button OpenButton
        {
            get { return _openButton; }
            set
            {
                if (_openButton == value)
                    return;
		
                RemoveOpenTriggers();
            
                _openButton = value;
        
                AddOpenTriggers();
            }
        }

        public List<DisplayObject> HitAreaAdditions;

        //----------------------------------
        //  dropDown
        //----------------------------------
    
        private DisplayObject _dropDown;
    
        public DisplayObject DropDown
        {
            get
            {
                return _dropDown;
            }
            set
            {
                _dropDown = value;
            }
        }

        //----------------------------------
        //  isOpen
        //----------------------------------
    
        /**
         *   
         */
        private bool _isOpen;

        /// <summary>
        /// 
        /// </summary>
        public bool IsOpen
        {
            get { return _isOpen; }
        }

        private Timer _rollOverOpenDelayTimer;

        private float? _rolloverOpenDelay;
        /// <summary>
        /// 
        /// </summary>
        public float? RolloverOpenDelay
        {
            get { 
                return _rolloverOpenDelay;
            }
            set
            {
// ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value == _rolloverOpenDelay)
                    return;

                RemoveOpenTriggers();

                _rolloverOpenDelay = value;

                AddOpenTriggers();
            }
        }

        /**
         *   
         *  Adds event triggers to the openButton to open the popup.
         * 
         *  <p>This is called from the openButton setter after the openButton has been set.</p>
         */ 
        private void AddOpenTriggers()
        {
            // TODO (jszeto): Change this to be mouseDown. Figure out how to not 
            // trigger systemManager_mouseDown.
            if (null == OpenButton) 
                return;
            if (null == RolloverOpenDelay)
                OpenButton.AddEventListener(ButtonEvent.BUTTON_DOWN, OpenButtonButtonDownHandler);
            else
                OpenButton.AddEventListener(MouseEvent.ROLL_OVER, OpenButtonRollOverHandler);
        }

        /**
         *  
         *  Removes event triggers from the openButton to open the popup.
         * 
         *  <p>This is called from the openButton setter after the openButton has been set.</p>
         */ 
        private void RemoveOpenTriggers()
        {
            // TODO (jszeto): Change this to be mouseDown. Figure out how to not 
            // trigger systemManager_mouseDown.
            if (null == OpenButton)
                return;
            if (null == RolloverOpenDelay)
                OpenButton.RemoveEventListener(ButtonEvent.BUTTON_DOWN, OpenButtonButtonDownHandler);
            else
                OpenButton.RemoveEventListener(MouseEvent.ROLL_OVER, OpenButtonRollOverHandler);
        }

        /**
         *  
         *  Adds event triggers close the popup.
         * 
         *  <p>This is called when the drop down is popped up.</p>
         */
        private void AddCloseTriggers()
        {
            var systemManager = SystemEventDispatcher.Instance;

            if (null == RolloverOpenDelay)
            {
                MouseEventDispatcher.Instance.AddEventListener(MouseEvent.MOUSE_DOWN, SystemManagerMouseDownHandler);
                //systemManager.addEventListener(SandboxMouseEvent.MOUSE_DOWN_SOMEWHERE, SystemManagerMouseDownHandler);
                systemManager.AddEventListener(ResizeEvent.RESIZE, SystemManagerResizeHandler); //, false, 0, true);
                systemManager.AddEventListener(MouseEvent.MOUSE_UP, SystemManagerMouseUpHandlerNoRollOverOpenDelay);
            }
            else
            {
                systemManager.AddEventListener(MouseEvent.MOUSE_MOVE, SystemManagerMouseMoveHandler);
                //systemManager.addEventListener(SandboxMouseEvent.MOUSE_MOVE_SOMEWHERE, SystemManagerMouseMoveHandler);
                // MOUSEUP triggers may be added in SystemManagerMouseMoveHandler
                systemManager.AddEventListener(ResizeEvent.RESIZE, SystemManagerResizeHandler);//, false, 0, true);
            }

            /* Note: we want this handler to fire AFTER the List processing event
               However, changing the priority doesn't help here, because MouseEventDispatcher always fires first 
             */
            MouseEventDispatcher.Instance.AddEventListener(MouseEvent.MOUSE_WHEEL, SystemManagerMouseWheelHandler); //, EventPhase.TargetAndBubbling, -50); // lower priority (let the list first process itself)
        }

        /**
         *  
         *  Adds event triggers close the popup.
         * 
         *  <p>This is called when the drop down is closed.</p>
         */ 
        private void RemoveCloseTriggers()
        {
            var systemManager = SystemEventDispatcher.Instance;

            if (null == RolloverOpenDelay)
            {
                MouseEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_DOWN, SystemManagerMouseDownHandler);
                //systemManager.RemoveEventListener(SandboxMouseEvent.MOUSE_DOWN_SOMEWHERE, SystemManagerMouseDownHandler);
                systemManager.RemoveEventListener(ResizeEvent.RESIZE, SystemManagerResizeHandler);//, false);
                systemManager.RemoveEventListener(MouseEvent.MOUSE_UP, SystemManagerMouseUpHandlerNoRollOverOpenDelay);
            }
            else
            {
                systemManager.RemoveEventListener(MouseEvent.MOUSE_MOVE, SystemManagerMouseMoveHandler);
                //systemManager.RemoveEventListener(SandboxMouseEvent.MOUSE_MOVE_SOMEWHERE, SystemManagerMouseMoveHandler);
                systemManager.RemoveEventListener(MouseEvent.MOUSE_UP, SystemManagerMouseUpHandler);
                //systemManager.RemoveEventListener(SandboxMouseEvent.MOUSE_UP_SOMEWHERE, SystemManagerMouseUpHandler);
                systemManager.RemoveEventListener(ResizeEvent.RESIZE, SystemManagerResizeHandler);
            }

            MouseEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_WHEEL, SystemManagerMouseWheelHandler);
        } 

        /**
         *  
         *  Helper method for the mouseMove and mouseUp handlers to see if 
         *  the mouse is over a "valid" region.  This is used to help determine 
         *  when the dropdown should be closed.
         */ 
        private bool IsTargetOverDropDownOrOpenButton(DisplayObject target)
        {
            if (null != target)
            {
                // check if the target is the openButton or contained within the openButton
                if (OpenButton.Contains(target as DisplayListMember))
                    return true;
                if (HitAreaAdditions != null)
                {
                    foreach (DisplayObject displayObject in HitAreaAdditions)
                    {
                        if (displayObject == target ||
                            ((displayObject is DisplayObjectContainer) && ((DisplayObjectContainer)displayObject).Contains(target as DisplayListMember)))
                            return true;
                    }
                }
            
                // check if the target is the dropdown or contained within the dropdown
                var down = DropDown as DisplayObjectContainer;
                if (down != null)
                {
                    if (down.Contains((DisplayListMember) target))
                        return true;
                }
                else
                {
                    if (target == DropDown)
                        return true;
                }
            }
        
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void OpenDropDown()
        {
            OpenDropDownHelper(true);
        }

        /**
         *  
         *  Set isProgrammatic to true if you are opening the dropDown programmatically 
         *  or not through a mouse click or rollover.  
         */ 
        private void OpenDropDownHelper(bool isProgrammatic = false)
        {
            if (!IsOpen)
            {
                AddCloseTriggers();
            
                _isOpen = true;
                // Force the button to stay in the down state
                if (null != OpenButton)
                    OpenButton.KeepDown(true, !isProgrammatic); 
            
                DispatchEvent(new DropDownEvent(DropDownEvent.OPEN));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commit"></param>
        public void CloseDropDown(bool commit)
        {
            if (IsOpen)
            {   
                _isOpen = false;
                if (null != OpenButton)
                    OpenButton.KeepDown(false);
            
                var dde = new DropDownEvent(DropDownEvent.CLOSE, false, true);
            
                if (!commit)
                    dde.PreventDefault();
            
                DispatchEvent(dde);
            
                RemoveCloseTriggers();
            }
        }   

        //--------------------------------------------------------------------------
        //
        //  Event handling
        //
        //--------------------------------------------------------------------------
    
         /**
         *  
         *  Called when the buttonDown event is dispatched. This function opens or closes
         *  the dropDown depending upon the dropDown state. 
         */ 
        internal void OpenButtonButtonDownHandler(Event e)
        {
            if (IsOpen)
                CloseDropDown(true);
            else
            {
                _mouseIsDown = true;
                OpenDropDownHelper();
            }
        }

        /**
         *  
         *  Called when the openButton's <code>rollOver</code> event is dispatched. This function opens 
         *  the drop down, or opens the drop down after the length of time specified by the 
         *  <code>rollOverOpenDelay</code> property.
         */ 
        internal void OpenButtonRollOverHandler(Event e)
        {
            if (null == RolloverOpenDelay)
                return;

            if (RolloverOpenDelay == 0)
                OpenDropDownHelper();
            else
            {
                OpenButton.AddEventListener(MouseEvent.ROLL_OUT, OpenButtonRollOutHandler);
                _rollOverOpenDelayTimer = new Timer((float) RolloverOpenDelay, 1);
                _rollOverOpenDelayTimer.AddEventListener(Timer.COMPLETE, rollOverDelay_timerCompleteHandler);
                _rollOverOpenDelayTimer.Start();
            }
        }

        /**
         *   
         *  Called when the openButton's rollOut event is dispatched while waiting 
         *  for the rollOverOpenDelay. This will cancel the timer so we don't open
         *  any more.
         */ 
        private void OpenButtonRollOutHandler(Event e)
        {
            if (null != _rollOverOpenDelayTimer && _rollOverOpenDelayTimer.IsRunning)
            {
                _rollOverOpenDelayTimer.Stop();
                _rollOverOpenDelayTimer = null;
            }
        
            OpenButton.RemoveEventListener(MouseEvent.ROLL_OUT, OpenButtonRollOutHandler);
        }

        /**
         *  
         *  Called when the rollOverDelay Timer is up and we should show the drop down.
         */ 
         private void rollOverDelay_timerCompleteHandler(Event e)
         {
             OpenButton.RemoveEventListener(MouseEvent.ROLL_OUT, OpenButtonRollOutHandler);
             _rollOverOpenDelayTimer = null;
         
             OpenDropDownHelper();
         }

        /**
         *  
         *  Called when the systemManager receives a mouseDown event. This closes
         *  the dropDown if the target is outside of the dropDown. 
         */     
        internal void SystemManagerMouseDownHandler(Event e)
        {
            // stop here if mouse was down from being down on the open button
            if (_mouseIsDown)
            {
                _mouseIsDown = false;
                return;
            }

            if (null == DropDown ||
                (DropDown is DisplayObjectContainer && 
                    !((DisplayObjectContainer)DropDown).Contains((DisplayListMember) e.Target, true)))
            {
                if (HitAreaAdditions != null)
                {
                    foreach (DisplayObject displayObject in HitAreaAdditions)
                    {
                        if (displayObject == e.Target ||
                            ((displayObject is DisplayObjectContainer) && ((DisplayObjectContainer)displayObject).Contains(e.Target as DisplayListMember)))
                            return;
                    }
                }

                CloseDropDown(true);
            } 
        }

        /**
         *  
         *  Called when the dropdown is popped up from a rollover and the mouse moves 
         *  anywhere on the screen.  If the mouse moves over the openButton or the dropdown, 
         *  the popup will stay open.  Otherwise, the popup will close.
         */ 
        internal void SystemManagerMouseMoveHandler(Event e)
        {
            var systemManager = SystemEventDispatcher.Instance;

            DisplayObject target = e.Target as DisplayObject;
            var containedTarget = IsTargetOverDropDownOrOpenButton(target);
        
            if (containedTarget)
                return;
        
            // if the mouse is down, wait until it's released to close the drop down
            if (e is MouseEvent && ((MouseEvent)e).ButtonDown)/* ||
                (event is SandboxMouseEvent && SandboxMouseEvent(event).buttonDown)*/
            {
                systemManager.AddEventListener(MouseEvent.MOUSE_UP, SystemManagerMouseUpHandler);
                //systemManager.AddEventListener(SandboxMouseEvent.MOUSE_UP_SOMEWHERE, SystemManagerMouseUpHandler);
                return;
            }
        
            CloseDropDown(true);
        }

        /**
         *  
         *  Debounce the mouse
         */
        internal void SystemManagerMouseUpHandlerNoRollOverOpenDelay(Event e)
        {
            // stop here if mouse was down from being down on the open button
            if (_mouseIsDown)
            {
                _mouseIsDown = false;
            }
        }

        /**
         *  
         *  Called when the dropdown is popped up from a rollover and the mouse is released 
         *  anywhere on the screen.  This will close the popup.
         */ 
        internal void SystemManagerMouseUpHandler(Event e)
        {
            var target = e.Target as DisplayObject;
            var containedTarget = IsTargetOverDropDownOrOpenButton(target);

            // if we're back over the target area, remove this event listener
            // and do nothing.  we handle this in mouseMoveHandler()
            if (containedTarget)
            {
                SystemEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_UP, SystemManagerMouseUpHandler);
                //systemManager.getSandboxRoot().removeEventListener(SandboxMouseEvent.MOUSE_UP_SOMEWHERE, SystemManagerMouseUpHandler);
                return;
            }
        
            CloseDropDown(true);
        }

        /**
         *  
         *  Close the dropDown if the stage has been resized.
         */
        internal void SystemManagerResizeHandler(Event e)
        {
            CloseDropDown(true);
        }

        /**
         *  
         *  Called when the mouseWheel is used
         */
        private void SystemManagerMouseWheelHandler(Event e)
        {
            // Close the dropDown unless we scrolled over the dropdown and the dropdown handled the event
            /*if (!(((DisplayObjectContainer)DropDown).Contains(((DisplayObject)e.Target) as DisplayListMember) && e.DefaultPrevented))
                CloseDropDown(false);*/
            MouseEvent me = (MouseEvent) e;
            if (!DropDown.Transform.GlobalBounds.Contains(me.GlobalPosition))
                CloseDropDown(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public void ProcessFocusOut(Event e)
        {
            FocusEvent fe = (FocusEvent) e;
            // Note: event.relatedObject is the object getting focus.
            // It can be null in some cases, such as when you open
            // the dropdown and then click outside the application.
        
            // If the dropdown is open...
            if (IsOpen)
            {
                // If focus is moving outside the dropdown...
                if (null == fe.RelatedObject ||
                    (null == DropDown || 
                        (DropDown is DisplayObjectContainer &&
                         !((DisplayObjectContainer)DropDown).Contains(fe.RelatedObject))))
                {
                    // Close the dropdown.
                    CloseDropDown(true);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool ProcessKeyDown(Event e)
        {
            KeyboardEvent ke = (KeyboardEvent) e;

            if (e.DefaultPrevented)
                return true;
        
            if (ke.Control && ke.KeyCode == KeyCode.DownArrow)
            {
                OpenDropDownHelper(true); // Programmatically open
                e.PreventDefault();
            }
            else if (ke.Control && ke.KeyCode == KeyCode.UpArrow)
            {
                CloseDropDown(true);
                e.PreventDefault();
            }    
            else if (ke.KeyCode == KeyCode.Return)
            {
                // Close the dropDown and eat the event if appropriate.
                if (IsOpen)
                {
                    CloseDropDown(true);
                    e.PreventDefault();
                }
            }
            else if (ke.KeyCode == KeyCode.Escape)
            {
                // Close the dropDown and eat the event if appropriate.
                if (IsOpen)
                {
                    CloseDropDown(false);
                    e.PreventDefault();
                }
            }
            else
            {
                return false;
            }   
            
            return true;        
        }
    }
}