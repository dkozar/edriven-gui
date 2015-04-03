using System;
using eDriven.Core.Events;
using eDriven.Gui.Layout;
using EventHandler=eDriven.Core.Events.EventHandler;

// ReSharper disable UnusedMember.Global

namespace eDriven.Gui.Components
{
    /// <summary>
    /// Group using the TileLayout class
    /// </summary>
    public class TileGroup : Group
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public TileGroup()
        {
            base.Layout = new TileLayout();
        }

        private TileLayout TileLayout
        {
            get
            {
                return (TileLayout)Layout;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ColumnAlign ColumnAlign
        {
            get { return TileLayout.ColumnAlign; }
            set { TileLayout.ColumnAlign = value; }
        }

        /// <summary>
        /// Column count
        /// </summary>
        public virtual int ColumnCount
        {
            get { return TileLayout.ColumnCount; }
        }

        /// <summary>
        /// Column width
        /// </summary>
        public virtual float? ColumnWidth
        {
            get { return TileLayout.ColumnWidth; }
            set { TileLayout.ColumnWidth = value; }
        }

        /// <summary>
        /// Horizontal align
        /// </summary>
        public virtual HorizontalAlign HorizontalAlign
        {
            get { return TileLayout.HorizontalAlign; }
            set { TileLayout.HorizontalAlign = value; }
        }

        /// <summary>
        /// Horizontal gap
        /// </summary>
        public virtual float HorizontalGap
        {
            get { return TileLayout.HorizontalGap; }
            set { TileLayout.HorizontalGap = value; }
        }

        /// <summary>
        /// Orientation
        /// </summary>
        public virtual TileOrientation Orientation
        {
            get { return TileLayout.Orientation; }
            set { TileLayout.Orientation = value; }
        }

        /// <summary>
        /// Requested column count
        /// </summary>
        public virtual int RequestedColumnCount
        {
            get { return TileLayout.RequestedColumnCount; }
            set { TileLayout.RequestedColumnCount = value; }
        }

        /// <summary>
        /// Requested row count
        /// </summary>
        public virtual int RequestedRowCount
        {
            get { return TileLayout.RequestedRowCount; }
            set { TileLayout.RequestedRowCount = value; }
        }

        /// <summary>
        /// Row align
        /// </summary>
        public virtual RowAlign RowAlign
        {
            get { return TileLayout.RowAlign; }
            set { TileLayout.RowAlign = value; }
        }

        /// <summary>
        /// Row count
        /// </summary>
        public virtual int RowCount
        {
            get { return TileLayout.RowCount; }
        }

        /// <summary>
        /// Row height
        /// </summary>
        public virtual float? RowHeight
        {
            get { return TileLayout.RowHeight; }
            set { TileLayout.RowHeight = value; }
        }

        /// <summary>
        /// Vertical align
        /// </summary>
        public virtual VerticalAlign VerticalAlign
        {
            get { return TileLayout.VerticalAlign; }
            set { TileLayout.VerticalAlign = value; }
        }

        /// <summary>
        /// Vertical gap
        /// </summary>
        public virtual float VerticalGap
        {
            get { return TileLayout.VerticalGap; }
            set { TileLayout.VerticalGap = value; }
        }

        /// <summary>
        /// Layout (read only)
        /// </summary>
        override public LayoutBase Layout
        {
            get { return base.Layout; }
            set { throw new Exception("TileGroup Layout is read only"); }
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
                    TileLayout.AddEventListener(eventType, RedispatchHandler, phases, priority);
            }
            base.AddEventListener(eventType, handler);
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
                    TileLayout.RemoveEventListener(eventType, RedispatchHandler);
            }
            base.RemoveEventListener(eventType, handler, phases);
        }

        private void RedispatchHandler(Event e)
        {
            DispatchEvent(e);
        }
    }
}

// ReSharper restore UnusedMember.Global