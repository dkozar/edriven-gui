using eDriven.Core.Geom;
using UnityEngine;

namespace eDriven.Gui.Components
{
    /**
     * A base GUI transform class<br/>
     * This class contains features important for the final execution of laying out the component (sizing, position etc.)<br/>
     * I call this process 'baking'<br/>
     * The subclass should implement the mechanism of updating the actual component (referenced with Target property)<br/>
     * Each subclass should implement its own component updating mechanism<br/>
     * For instance, the DisplayObject is going to be updated immediatelly after any of the positions or dimensions changed<br/>
     * At the other hand, the InvalidationManagerClient and its subclasses will be updated most probably after the layout pass<br/>
     * */
    /// <summary>
    /// A base GUI transform class
    /// </summary>
    public abstract class GuiTransformBase
    {
        #region Target reference

        /// <summary>
        /// The target component
        /// </summary>
        private readonly DisplayListMember _target;
        /// <summary>
        /// Target component
        /// </summary>
        internal DisplayListMember Target
        {
            get { return _target; }
        }

        #endregion

        private DisplayObjectContainer _doc;

        #region Constructor

        /// <summary>
        /// Constructor
        /// It takes a target as an argument, and it holds the reference to that target
        /// </summary>
        /// <param name="target"></param>
        protected GuiTransformBase(DisplayListMember target)
        {
            _target = target;
            _doc = target as DisplayObjectContainer;
        }

        #endregion

        #region Bounds, position, dimensions

        /// <summary>
        /// The bounds
        /// Bounds hold the central piece of information
        /// It is the master from which anything else is being derived
        /// </summary>
        internal Rectangle Bounds
        {
            get
            {
                /* Important: it has to be a new instance!!! */
                return new Rectangle(Target.X, Target.Y, Target.Width, Target.Height);
            }
        }

        /// <summary>
        /// Position (bakes rect)
        /// </summary>
        public Point Position // from protected to public on 28.1.2012.
        {
            get
            {
                return Bounds.Position;
            }
        }

        #endregion

        #region Read only properties

        /// <summary>
        /// Local bounds (*RARE*)<br/>
        /// Bounds of the component observed from that component POW<br/>
        /// Both X and Y are 0
        ///</summary>
        public Rectangle LocalBounds
        {
            get
            {
                return Rectangle.FromPositionAndSize(Point.Zero, Bounds.Size);
            }
        }

        /// <summary>
        /// Global (screen) Bounds for figuring out mouseovers (*RARE*)<br/>
        /// We are not baking global bounds, because they are dynamic, i.e. depend of bounds of parent hierarchy<br/>
        /// It would be too expensive if we are about to bake them each time we change the position of the ancestor
        /// </summary>
        public Rectangle GlobalBounds
        {
            get
            {
                return Rectangle.FromPositionAndSize(_globalPosition, Bounds.Size);
            }
        }

        private Point _globalPosition = Point.Zero;

        /// <summary>
        /// Global position
        /// </summary>
        public Point GlobalPosition { 
            get
            {
                return _globalPosition;
            }
            private set
            {
                _globalPosition = value;
            }
        }

        private Point _renderingPosition = Point.Zero;
        
        /// <summary>
        /// Rendering position
        /// </summary>
        public Point RenderingPosition
        {
            get
            {
                return _renderingPosition;
            }
            private set
            {
                _renderingPosition = value;
            }
        }
        
        #endregion

        #region Methods

        ///<summary>
        /// Applies the transform info to a target<br/>
        /// Usually called by the parent transform
        ///</summary>
        ///<param name="renderingPosition"></param>
        ///<param name="globalPosition"></param>
        ///<param name="renderingRect"></param>
        ///<param name="localRenderingRect"></param>
        public void Apply(Point renderingPosition, Point globalPosition, Rect renderingRect, Rect localRenderingRect)
        {
            RenderingPosition = renderingPosition;
            GlobalPosition = globalPosition;
            Target.RenderingRect = renderingRect;
            Target.LocalRenderingRect = localRenderingRect;
        }

        /// <summary>
        /// Returns true if bounds contains point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool ContainsPoint(Point point)
        {
            return GlobalBounds.Contains(point);
        }

        #endregion

        public override string ToString()
        {
            return string.Format("Target: {0}, Bounds: {1}, RenderingPosition: {2}, RenderingRect: {3}", _target, Bounds, RenderingPosition, Target.RenderingRect);
        }

        ///<summary>
        /// Updates child transforms based on this transform
        ///</summary>
        public void ValidateChildren()
        {
            if (null == _doc)
                return;

            foreach (var child in _doc.QChildren) // working with true children
            {
                InvalidateChild(child);
            }
        }

        ///<summary>
        /// Update a single child transform
        /// Has to be implemented by successor<br/>
        ///</summary>
        ///<param name="child"></param>
        protected abstract void ValidateChild(DisplayObject child);

        ///<summary>
        /// Invalidates the transform, meaning it has to be validated somehow<br/>
        /// This method has to be implemented by subclass<br/>
        /// The subclass might choose will it update immediatelly or later
        ///</summary>
        protected abstract void InvalidateChild(DisplayObject child);

        ///<summary>
        /// Invalidates the transform, meaning it has to be validated somehow<br/>
        /// This method has to be implemented by subclass<br/>
        /// The subclass might choose will it update immediatelly or later
        ///</summary>
        public abstract void Invalidate();

        ///<summary>
        /// Invalidates the transform, meaning it has to be validated somehow<br/>
        /// This method has to be implemented by subclass<br/>
        /// The subclass might choose will it update immediatelly or later
        ///</summary>
        //public abstract void Validate();

        ///<summary>
        /// Invalidates the transform, meaning it has to be validated somehow<br/>
        /// This method has to be implemented by subclass<br/>
        /// The subclass might choose will it update immediatelly or later
        ///</summary>
        public void Validate()
        {
            if (null != Target.Parent)
                Target.Parent.Transform.ValidateChild(Target);
        }
    }
}