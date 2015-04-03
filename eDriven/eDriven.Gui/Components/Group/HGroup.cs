using System;
using eDriven.Core.Events;
using eDriven.Gui.Layout;
using UnityEngine;
using Event = eDriven.Core.Events.Event;
using EventHandler=eDriven.Core.Events.EventHandler;

namespace eDriven.Gui.Components
{
    public class HGroup : Group
    {
        public HGroup()
        {
            base.Layout = new HorizontalLayout();
        }

        /*protected override void UpdateDisplayList(float width, float height)
        {
            Debug.Log(@"HGroup. NonInheritingStyles->" + NonInheritingStyles);
            base.UpdateDisplayList(width, height);
        }*/

        private HorizontalLayout HorizontalLayout
        {
            get
            {
                return (HorizontalLayout)Layout;
            }
        }

        /// <summary>
        /// Horizontal spacing in the case of horizontal direction
        /// </summary>
        public int Gap
        {
            get { return HorizontalLayout.Gap; }
            set { HorizontalLayout.Gap = value; }
        }

        /// <summary>
        /// Horizontal align in the case of <b>vertical</b> direction
        /// </summary>
        public virtual HorizontalAlign HorizontalAlign
        {
            get { return HorizontalLayout.HorizontalAlign; }
            set { HorizontalLayout.HorizontalAlign = value; }
        }

        /// <summary>
        /// Vertical align in the case of <b>horizontal</b> direction
        /// </summary>
        public virtual VerticalAlign VerticalAlign
        {
            get { return HorizontalLayout.VerticalAlign; }
            set { HorizontalLayout.VerticalAlign = value; }
        }

        public virtual int ColumnCount
        {
            get { return HorizontalLayout.ColumnCount; }
        }

        public virtual float PaddingLeft
        {
            get { return HorizontalLayout.PaddingLeft; }
            set { HorizontalLayout.PaddingLeft = value; }
        }

        public virtual float PaddingRight
        {
            get { return HorizontalLayout.PaddingRight; }
            set { HorizontalLayout.PaddingRight = value; }
        }

        public virtual float PaddingTop
        {
            get { return HorizontalLayout.PaddingTop; }
            set { HorizontalLayout.PaddingTop = value; }
        }

        public virtual float PaddingBottom
        {
            get { return HorizontalLayout.PaddingBottom; }
            set { HorizontalLayout.PaddingBottom = value; }
        }

        public virtual int RequestedColumnCount
        {
            get { return HorizontalLayout.RequestedColumnCount; }
            set { HorizontalLayout.RequestedColumnCount = value; }
        }

        public virtual float ColumnWidth
        {
            get { return HorizontalLayout.ColumnWidth; }
            set { HorizontalLayout.ColumnWidth = value; }
        }

        public virtual bool VariableColumnWidth
        {
            get { return HorizontalLayout.VariableColumnWidth; }
            set { HorizontalLayout.VariableColumnWidth = value; }
        }

        /// <summary>
        /// Layout (read only)
        /// </summary>
        override public LayoutBase Layout
        {
            get { return base.Layout; }
            set { throw new Exception("HGroup Layout is read only"); }
        }

        /// <summary>
        /// Overriding the MAIN AddEventListener method!
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        /// <param name="phases"></param>
        /// <param name="priority"></param>
        public override void AddEventListener(string eventType, EventHandler handler, EventPhase phases, int priority)
        {
            if (eventType == "propertyChange")
            {
                if (!HasEventListener(eventType))
                    HorizontalLayout.AddEventListener(eventType, RedispatchHandler, phases, priority);
            }
            base.AddEventListener(eventType, handler, phases, priority);
        }

        /// <summary>
        /// Overriding the MAIN RemoveEventListener method!
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        /// <param name="phases"></param>
        public override void RemoveEventListener(string eventType, EventHandler handler, EventPhase phases)
        {
            if (eventType == "propertyChange")
            {
                if (!HasEventListener(eventType))
                    HorizontalLayout.RemoveEventListener(eventType, RedispatchHandler);
            }
            base.RemoveEventListener(eventType, handler, phases);
        }

        private void RedispatchHandler(Event e)
        {
            DispatchEvent(e);
        }
    }
}