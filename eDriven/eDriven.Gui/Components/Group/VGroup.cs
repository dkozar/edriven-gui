using System;
using eDriven.Core.Events;
using eDriven.Gui.Layout;
using EventHandler=eDriven.Core.Events.EventHandler;

namespace eDriven.Gui.Components
{
    public class VGroup : Group
    {
        public VGroup()
        {
            base.Layout = new VerticalLayout();
        }

        private VerticalLayout VerticalLayout
        {
            get
            {
                return (VerticalLayout)Layout;
            }
        }

        /// <summary>
        /// Horizontal spacing in the case of horizontal direction
        /// </summary>
        public int Gap
        {
            get { return VerticalLayout.Gap; }
            set { VerticalLayout.Gap = value; }
        }

        /// <summary>
        /// Horizontal align in the case of <b>vertical</b> direction
        /// </summary>
        public virtual HorizontalAlign HorizontalAlign
        {
            get { return VerticalLayout.HorizontalAlign; }
            set { VerticalLayout.HorizontalAlign = value; }
        }

        /// <summary>
        /// Vertical align in the case of <b>horizontal</b> direction
        /// </summary>
        public virtual VerticalAlign VerticalAlign
        {
            get { return VerticalLayout.VerticalAlign; }
            set { VerticalLayout.VerticalAlign = value; }
        }

        public virtual int RowCount
        {
            get { return VerticalLayout.RowCount; }
        }

        public virtual float PaddingLeft
        {
            get { return VerticalLayout.PaddingLeft; }
            set { VerticalLayout.PaddingLeft = value; }
        }

        public virtual float PaddingRight
        {
            get { return VerticalLayout.PaddingRight; }
            set { VerticalLayout.PaddingRight = value; }
        }

        public virtual float PaddingTop
        {
            get { return VerticalLayout.PaddingTop; }
            set { VerticalLayout.PaddingTop = value; }
        }

        public virtual float PaddingBottom
        {
            get { return VerticalLayout.PaddingBottom; }
            set { VerticalLayout.PaddingBottom = value; }
        }

        public virtual int RequestedRowCount
        {
            get { return VerticalLayout.RequestedRowCount; }
            set { VerticalLayout.RequestedRowCount = value; }
        }

        public virtual float RowHeight
        {
            get { return VerticalLayout.RowHeight; }
            set { VerticalLayout.RowHeight = value; }
        }

        public virtual bool VariableRowHeight
        {
            get { return VerticalLayout.VariableRowHeight; }
            set { VerticalLayout.VariableRowHeight = value; }
        }

        /// <summary>
        /// Layout (read only)
        /// </summary>
        override public LayoutBase Layout
        {
            get { return base.Layout; }
            set { throw new Exception("VGroup Layout is read only"); }
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
                    VerticalLayout.AddEventListener(eventType, RedispatchHandler, phases, priority);
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
                    VerticalLayout.RemoveEventListener(eventType, RedispatchHandler);
            }
            base.RemoveEventListener(eventType, handler, phases);
        }

        private void RedispatchHandler(Event e)
        {
            DispatchEvent(e);
        }
    }
}