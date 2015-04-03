using eDriven.Core.Geom;
using eDriven.Gui.Managers;
using UnityEngine;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// InvalidationManagerClientTransform<br/>
    /// Since we could affort to use the invalidation methods with InvalidationManagerClient, calling the Transform.Invalidate() is automatic<br/>
    /// by the system, when:
    /// 1. changing any of the X, Y, Width and Height properties
    /// 2. calling SetActualSize() or Move()
    /// </summary>
    internal sealed class InvalidationManagerClientTransform : GuiTransformBase
    {
#if DEBUG 
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;
// ReSharper restore UnassignedField.Global
#endif

        private readonly InvalidationManagerClient _imc;
        private readonly GroupBase _group;

        /// <summary>
        /// Constructor
        /// It takes a target as an argument, and it holds the reference to that target
        /// </summary>
        /// <param name="target"></param>
        public InvalidationManagerClientTransform(DisplayListMember target)
            : base(target)
        {
            _imc = (InvalidationManagerClient)target; // mandatory
            _group = target as GroupBase;
        }

        private Point _childRenderingPosition = new Point();
        private Point _childGlobalPosition = new Point();
        private bool _isClippingGroup;
        private Rect _childLocalRenderingRect;
        private Rect _childRenderingRect;
        private Rectangle _childBounds;

        ///<summary>
        /// Invalidates the transform, meaning it has to be validated somehow<br/>
        /// This method has to be implemented by subclass<br/>
        /// The subclass might choose will it update immediatelly or later
        ///</summary>
        public override void Invalidate()
        {
            /**
             * Invalidating via InvalidationManager
             * Note: if invalidating during the parent validation, that's fine because we are pushing to the end of the same queue
             * */
            InvalidationManager.Instance.InvalidateTransform(_imc);
        }

        ///<summary>
        /// When InvalidationManagerClientTransform is invalidated, we are not updating immediatelly,<br/>
        /// but going throught the invalidation instead<br/>
        /// The ValidateTransform (on target) will be called later and our ValidateChild will be called from there
        ///</summary>
        protected override void InvalidateChild(DisplayObject child)
        {
            //_imc.InvalidateTransform();
            child.Transform.Invalidate();
        }

        protected override void ValidateChild(DisplayObject child)
        {
            _childBounds = child.Transform.Bounds;

            _isClippingGroup = null != _group && _group.ClipAndEnableScrolling;

            _childRenderingPosition = _isClippingGroup ?
                                    child.Transform.Position : // parent is clipping. Origin is the distance from (0, 0) of the parent
                                    _imc.Transform.RenderingPosition.Add(child.Transform.Position); // add to rendering position

            _childGlobalPosition = _imc.Transform.GlobalPosition.Add(child.Transform.Position);

            _childRenderingRect = new Rect(_childRenderingPosition.X, _childRenderingPosition.Y, _childBounds.Width, _childBounds.Height);
            _childLocalRenderingRect = new Rect(0, 0, _childBounds.Width, _childBounds.Height);

            child.Transform.Apply(_childRenderingPosition, _childGlobalPosition, _childRenderingRect, _childLocalRenderingRect);

            child.Transform.ValidateChildren(); // RECURSION! (skin etc.)
        }
    }
}