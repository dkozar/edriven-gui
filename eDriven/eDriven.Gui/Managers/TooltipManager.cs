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

using System;
using eDriven.Core;
using eDriven.Core.Events;
using eDriven.Core.Geom;
using eDriven.Core.Managers;
using eDriven.Core.Util;
using eDriven.Gui.Check;
using eDriven.Gui.Components;
using eDriven.Gui.Stages;
using Event=eDriven.Core.Events.Event;
using MulticastDelegate=eDriven.Core.Events.MulticastDelegate;

namespace eDriven.Gui.Managers
{
    /// <summary>
    /// A manager of chat messages appearing in Unity GUI
    /// Coded by Danko Kozar
    /// </summary>

    internal class TooltipManager : EventDispatcher
    {
        #region CONSTANTS

// ReSharper disable InconsistentNaming
        public const string TOOLTIP_SHOW = "tooltipShow";
        public const string TOOLTIP_HIDE = "tooltipHide";
// ReSharper restore InconsistentNaming

        #endregion

        #region MEMBERS

        private TooltipManagerStage _stage;

        private Timer _timer;

        private Component _comp;

        #endregion

        #region Singleton

        private static TooltipManager _instance;

        private TooltipManager()
        {
            // Constructor is protected!
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static TooltipManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TooltipManager();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        #region PROPERTIES

        ///// <summary>
        ///// Message lifetime in seconds
        ///// </summary>
        //public float MessageLifeTime = 5;

        ///// <summary>
        ///// Message fade in duration in seconds
        ///// </summary>
        //public float FadeInDuration = 1;

        ///// <summary>
        ///// Message fade out duration in seconds
        ///// </summary>

        //public float FadeOutDuration = 1; // in seconds

        /// <summary>
        /// Offset and dimensions in view coordinates (pixels)
        /// </summary>
        public Rectangle BubbleOffsetAndDimensions = new Rectangle(0, 40, 100, 150);

        public Point Offset = new Point(0, 25);

        public bool ContinuousMode;

        #endregion

        private void InitStage()
        {
            //Debug.Log("Initializing TooltipManagerStage");
            _stage = TooltipManagerStage.Instance;
        }

        #region PUBLIC METHODS

        public void Initialize()
        {
            _timer = new Timer(0.5f, 1);

            TooltipShowHandler = new MulticastDelegate(this, TOOLTIP_SHOW);
            TooltipHideHandler = new MulticastDelegate(this, TOOLTIP_HIDE);

            /**
             * Sanity check
             * */
            //if (FadeInDuration + FadeOutDuration > MessageLifeTime)
            //    throw new TooltipManagerException(TooltipManagerException.LifetimeError);

            InitStage();

            MouseEventDispatcher.Instance.AddEventListener(MouseEvent.MOUSE_OVER, MouseOverHandler);
            if (ContinuousMode)
                MouseEventDispatcher.Instance.AddEventListener(MouseEvent.MOUSE_MOVE, MouseMoveHandler);
            MouseEventDispatcher.Instance.AddEventListener(MouseEvent.MOUSE_OUT, MouseOutHandler);

#if TRIAL
            /* HACK CHECK */
            Acme acme = (Acme) Framework.GetComponent<Acme>(true);
            if (null == acme || !acme.gameObject.activeInHierarchy/*active*/ || !acme.enabled)
                return;
#endif
        }

        public override void Dispose()
        {
            base.Dispose();

            MouseEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_OVER, MouseOverHandler);
            MouseEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_MOVE, MouseMoveHandler);
            MouseEventDispatcher.Instance.RemoveEventListener(MouseEvent.MOUSE_OUT, MouseOutHandler);
        }

        private void MouseOverHandler(Event e)
        {
            _comp = e.Target as Component;
            if (null == _comp)
                return;

            //Debug.Log("Tooltip mouse over: " + _comp.Tooltip);

            //MouseEvent me = (MouseEvent) e;

            _timer.Stop();

            if (!string.IsNullOrEmpty(_comp.Tooltip))
            {
                _timer.Complete += OnTimerComplete;
                _timer.Start();
            }
        }

        private void OnTimerComplete(Event e)
        {
            _stage.ShowTooltip(_comp.Tooltip, SystemManager.Instance.MousePosition.Add(Offset));
            DispatchEvent(new Event(TOOLTIP_SHOW));
        }

        private void MouseMoveHandler(Event e)
        {
            MouseEvent me = (MouseEvent)e;
            _stage.MoveTooltip(me.GlobalPosition.Add(Offset));
        }

        private void MouseOutHandler(Event e)
        {
            Component comp = e.Target as Component;
            if (null == comp)
                return;

            //Debug.Log("Tooltip mouse out: " + comp.Tooltip);

            _stage.HideTooltip();

            DispatchEvent(new Event(TOOLTIP_HIDE));
        }

        #endregion

        #region EVENT HANDLING

        public MulticastDelegate TooltipShowHandler;
        public MulticastDelegate TooltipHideHandler;

        #endregion
    }
}