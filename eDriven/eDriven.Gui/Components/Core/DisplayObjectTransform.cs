using eDriven.Core.Geom;
using UnityEngine;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// DisplayObjectTransform
    /// DisplayObject doesn't use the invalidation, so I decided not to call InvalidateTransform each time the X, Y, Width or Height of the component changes<br/>
    /// Instead, decided to use the "commit" system where the Transform.Invalidate() method is the commit command
    /// It should be called to commit all the values<br/>
    /// For instance, it is called manually upon Group's _contentPane reposition (content pane is DisplayObjectContainer)
    /// </summary>
    internal sealed class DisplayObjectTransform : GuiTransformBase
    {
        private readonly DisplayObjectContainer _doc;
        private Rectangle _childBounds;

        /// <summary>
        /// Constructor
        /// It takes a target as an argument, and it holds the reference to that target
        /// </summary>
        /// <param name="target"></param>
        public DisplayObjectTransform(DisplayListMember target) : base(target)
        {
            _doc = Target as DisplayObjectContainer;
        }

        private Point _globalPosition;
        private Rect _localRenderingRect;
        private Rect _renderingRect;
        private Point _renderingPosition;

        ///<summary>
        /// Invalidates the transform, meaning it has to be validated somehow<br/>
        /// This method has to be implemented by subclass<br/>
        /// The subclass might choose will it update immediatelly or later
        ///</summary>
        public override void Invalidate()
        {
            /* VALIDATING TRANSFORM HERE */
            Validate();
        }

        ///<summary>
        /// When DisplayObjectTransform is invalidated, we are updating immediatelly<br/>
        /// There's no invalidation available for DisplayObjects
        ///</summary>
        protected override void InvalidateChild(DisplayObject child)
        {
            ValidateChild(child);
        }

        protected override void ValidateChild(DisplayObject child)
        {
            _childBounds = child.Transform.Bounds;

            _renderingPosition = _doc.Transform.RenderingPosition.Add(child.Transform.Position); // add to rendering position

            _globalPosition = _doc.Transform.GlobalPosition.Add(child.Transform.Position);

            _localRenderingRect = new Rect(0, 0, _childBounds.Width, _childBounds.Height);
            _renderingRect = new Rect(_renderingPosition.X, _renderingPosition.Y, _childBounds.Width, _childBounds.Height);

            child.Transform.Apply(_renderingPosition, _globalPosition, _renderingRect, _localRenderingRect);

            child.Transform.ValidateChildren(); // RECURSION!
        }
    }
}