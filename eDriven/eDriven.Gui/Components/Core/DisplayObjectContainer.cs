using System.Collections.Generic;
using eDriven.Core.Geom;
using eDriven.Gui.Containers;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// DisplayListMember that has the ability to hold other display objects (display list members)
    /// NOTE: This is implemented here, not in Container only, because simple Components should have the ability to have children, without layout
    /// </summary>
    public class DisplayObjectContainer : DisplayListMember, IChildList, ITrueChildList
    {
        #region Mouse

        private bool _mouseChildren = true;
        /// <summary>
        /// Should the children of this container be tested for mouse events
        /// </summary>
        public virtual bool MouseChildren
        {
            get { return _mouseChildren; }
            set { _mouseChildren = value; }
        }

        #endregion

        /// <summary>
        /// If true, the drawing list will update after each child addition or removal
        /// The process of updating is being optimized by the Container
        /// However, with DisplayObjectContainer it is not beacuse there are no invalidation methods
        /// We might choose to optimize the process, set this to false and call the InvalidateDrawingList() manually
        /// </summary>
        public bool AutoUpdateDrawingList = true;

        /// <summary>
        /// Children
        /// On children change, we must also update the drawing list, because that is the list used for rendering!
        /// </summary>
        private readonly List<DisplayListMember> _children = new List<DisplayListMember>();
        
        /// <summary>
        /// Children sort collection (defined depth order)
        /// This list is used for rendering
        /// If no children in this list - nothing will actually get rendered!
        /// </summary>
        private readonly List<DisplayListMember> _drawingList = new List<DisplayListMember>();
        
        #region ITrueChildList

        ///<summary>
        ///</summary>
        // ReSharper disable InconsistentNaming
        public List<DisplayListMember> QChildren
        // ReSharper restore InconsistentNaming
        {
            get
            {
                return _children;
            }
        }

        ///<summary>
        ///</summary>
        // ReSharper disable InconsistentNaming
        public int QNumberOfChildren
        // ReSharper restore InconsistentNaming
        {
            get
            {
                return _children.Count;
            }
        }

        public DisplayListMember QAddChild(DisplayListMember child)
        {
            DisplayListMember m = QAddChildAt(child, _children.Count);
            InvalidateDrawingList();
            return m;
        }

        /// <summary>
        /// The main add child method
        /// </summary>
        /// <param name="index"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public DisplayListMember QAddChildAt(DisplayListMember child, int index) // TODO: Do delayed version
        {
            if (_children.Contains(child))
            {
                if (child.Parent == this && _children.IndexOf(child) == index)
                    return child; // do nothing

                if (_children.Count > 0 && index == _children.Count && index != 0)
                {
                    index--;
                }
                _children.Remove(child);
            }

//            Debug.Log(string.Format(@"QAddChildAt [{0}]
//Adding {1} to {2}", index, child, this));

            //AddingChild(child);

//            Debug.Log(string.Format(@"QAddChildAt: {0} [child count: {1}]
//Adding {2} to: {3}", index, _children.Count, child, this));
            _children.Insert(index, child);
            //Debug.Log("   OK");

            //Debug.Log(1);
            //_drawingList.Insert(index, child); // 20130507
            
            //Debug.Log(2);

            child.Parent = this;

            InvalidateDrawingList();
            
            //ChildAdded(child);

            //Debug.Log("   -> added. Count: " + _children.Count + " [" + this + ", " + child + "]");

            return child;
        }

        public DisplayListMember QRemoveChild(DisplayListMember child)
        {
            bool removed = false;

            //RemovingChild(child);

            if (_children.Contains(child)) // 20120428, radi popupa koji se zatvara na "X" i options modela
                removed = _children.Remove(child);

            //_drawingList.Remove(child); // 20130507
            if (AutoUpdateDrawingList)
                InvalidateDrawingList();

            if (removed)
                child.Parent = null;
            
            //ChildRemoved(child);

            InvalidateDrawingList();

            return removed ? child : null;
        }

        public DisplayListMember QRemoveChildAt(int index) // TODO: Do delayed version
        {
            DisplayListMember child = QGetChildAt(index);
            InvalidateDrawingList();
            return QRemoveChild(child);
        }

        public void QRemoveAllChildren()
        {
            //Debug.Log(string.Format("QRemoveAllChildren [{0}]", this));
            //Debug.Log("_children.Count: " + _children.Count);
            for (int i = _children.Count - 1; i >= 0; i--)
            {
                var child = _children[i];
                QRemoveChild(child);
            }
            InvalidateDrawingList();
        }

        ///<summary>
        ///</summary>
        ///<param name="firstChild"></param>
        ///<param name="secondChild"></param>
        public void QSwapChildren(DisplayListMember firstChild, DisplayListMember secondChild) // TODO: Do delayed version
        {
            int index = QGetChildIndex(firstChild);
            if (index > -1)
            {
                QRemoveChildAt(index);
                QAddChildAt(secondChild, index);
            }
        }

        ///<summary>
        ///</summary>
        ///<param name="index"></param>
        ///<returns></returns>
        ///<exception cref="DisplayObjectContainerException"></exception>
        public DisplayListMember QGetChildAt(int index)
        {
            if (index + 1 > _children.Count)
                throw new DisplayObjectContainerException(DisplayObjectContainerException.IndexOutOfRange);

            return _children[index];
        }

        public int QGetChildIndex(DisplayListMember child)
        {
            return _children.IndexOf(child);
        }

        /// <summary>
        /// Sets child index
        /// </summary>
        /// <param name="child">A child</param>
        /// <param name="index">New index</param>
        /// <returns>The position</returns>
        public void QSetChildIndex(DisplayListMember child, int index)
        {
            if (index > -1)
            {
                QRemoveChildAt(index);
                QAddChildAt(child, index);
            }
        }

        /// <summary>
        /// Checks if this is a Owner of a component
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public bool QHasChild(DisplayListMember child)
        {
            if (null != child.Parent)
            {
                if (this == child.Parent)
                    return true;
            }

            return false;
        }

        public bool QContains(DisplayListMember child)
        {
            // check children only
            return QContains(child, false);
        }

        /// <summary>
        /// Returns true if child is the descendant of the component or the component itself
        /// </summary>
        /// <param name="child"></param>
        /// <param name="includeThisCheck">Should the container (me) be included in the search</param>
        /// <returns></returns>
        /// <remarks>Walks up the hierarchy -> Recursive!!!</remarks>
        public bool QContains(DisplayListMember child, bool includeThisCheck)
        {
            if (null == child)
                return false;

            // contains returns true for myself
            if (this == child)
                return includeThisCheck;

            var p = child.Parent;
            if (null != p)
            {
                if (this == p)
                    return true;

                return Contains(p);
            }

            return false;
        }
        
        #endregion

        #region IChildList

        /// <summary>
        /// The child components of the container
        /// </summary>
        public virtual List<DisplayListMember> Children
        {
            get
            {
                return _children;
            }
        }

        /// <summary>
        /// Number of children
        /// </summary>
        public virtual int NumberOfChildren
        {
            get
            {
                return _children.Count;
            }
        }

        public virtual DisplayListMember AddChild(DisplayListMember child)
        {
            //Debug.Log("Parent: " + child.Parent);
            DisplayObjectContainer formerParent = child.Parent;
            if (null != formerParent)
                formerParent.RemoveChild(child);

            QAddChild(child);

            return child;
        }

        public virtual DisplayListMember AddChildAt(DisplayListMember child, int index) // TODO: Do delayed version
        {
            //return QAddChildAt(index, child);
            DisplayObjectContainer formerParent = child.Parent;
            if (null != formerParent)
                formerParent.RemoveChild(child);

            QAddChildAt(child, index);

            return child;
        }

        virtual public DisplayListMember RemoveChild(DisplayListMember child)
        {
            QRemoveChild(child);

            return child;
        }

        public virtual DisplayListMember RemoveChildAt(int index) // TODO: Do delayed version
        {
            //DisplayListMember child = QGetChildAt(index);
            //return QRemoveChild(child);

            DisplayListMember child = GetChildAt(index);

            QRemoveChild(child);

            return child;
        }

        public virtual void RemoveAllChildren()
        {
            ////QRemoveAllChildren();
            ////_children.ForEach(delegate(DisplayListMember c) { RemoveChild(c); }); // very dangerous
            //for (int i = _children.Count - 1; i >= 0; i--)
            //{
            //    RemoveChild(_children[i]);
            //}
            
            //// Not needed: (just in case)
            //_children.Clear();
            //InvalidateDrawingList();

            ////Debug.Log(" -- _drawingList: " + _drawingList.Count);
            QRemoveAllChildren();
        }

        public virtual void SwapChildren(DisplayListMember firstChild, DisplayListMember secondChild) // TODO: Do delayed version
        {
            QSwapChildren(firstChild, secondChild);
        }

        public virtual DisplayListMember GetChildAt(int index)
        {
            return QGetChildAt(index);
        }

        public virtual int GetChildIndex(DisplayListMember child)
        {
            return QGetChildIndex(child);
        }

        ///<summary>
        /// Sets child index
        ///</summary>
        ///<param name="child">Child</param>
        ///<param name="index">New index</param>
        public virtual void SetChildIndex(DisplayListMember child, int index)
        {
            QSetChildIndex(child, index);
        }

        public virtual bool HasChild(DisplayListMember child)
        {
            return QHasChild(child);
        }

        /// <summary>
        /// Returns true if child is the descendant of the component or the component itself
        /// Non-exclusive variant
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        /// <remarks>Walks up the hierarchy -> Recursive!!!</remarks>
        public virtual bool Contains(DisplayListMember child)
        {
            // check children only
            return QContains(child, false);
        }

        public virtual bool Contains(DisplayListMember child, bool includeThisCheck)
        {
            return QContains(child, includeThisCheck);
        }

        #endregion

        #region Depth sorting

        /// <summary>
        /// The list of components ordered by child Depth<br/>
        /// Used for rendering<br/>
        /// The rendering order might have different than layout order
        /// </summary>
// ReSharper disable InconsistentNaming
        public List<DisplayListMember> QDrawingList
// ReSharper restore InconsistentNaming
        {
            get
            {
                return _drawingList;
            }
        }

        ///// <summary>
        ///// The list of components ordered by child Depth<br/>
        ///// Used for rendering<br/>
        ///// The rendering order might have different than layout order
        ///// </summary>
        //public virtual List<DisplayListMember> DrawingList
        //{
        //    get
        //    {
        //        return _drawingList;
        //    }
        //}

        /// <summary>
        /// Updates the depth list which is used for rendering<br/>
        /// The depth list is based upon the Depth property of each child<br/>
        /// It is the same as the order list if no Depth is defined on each of the children
        /// </summary>
        internal virtual void InvalidateDrawingList()
        {
            // display object updates the list immediatelly (there are no invalidation methods)
            //if (this is Stage)
            //    Debug.Log(string.Format("     DOC: Updating drawing list -> {0} [{1}, {2}]", this, _children.Count, _drawingList.Count));

            if (AutoUpdateDrawingList)
                DepthUtil.UpdateDrawingList(this);
        }
        
        /// <summary>
        /// TODO - do the Depth thing for each child
        /// </summary>
        /// <param name="child"></param>
        internal virtual void BringChildToFront(DisplayListMember child)
        {
            DepthUtil.BringToFront(_drawingList, child);
        }

        /// <summary>
        /// TODO - do the Depth thing for each child
        /// </summary>
        /// <param name="child"></param>
        internal virtual void PushChildToBack(DisplayListMember child)
        {
            DepthUtil.PushToBack(_drawingList, child);
        }

        #endregion

        #region Child add/remove handlers

        /// <summary>
        /// The place where the developer could implement the custom logic before the child addition
        /// </summary>
        /// <param name="child"></param>
        //protected virtual void AddingChild(DisplayListMember child) { }

        /// <summary>
        /// The place where the developer could implement the custom logic after the child removal
        /// </summary>
        /// <param name="child"></param>
        // ReSharper disable UnusedParameter.Global
        // ReSharper disable VirtualMemberNeverOverriden.Global
        //protected virtual void ChildAdded(DisplayListMember child) { }
        // ReSharper restore VirtualMemberNeverOverriden.Global
        // ReSharper restore UnusedParameter.Global

        /// <summary>
        /// The place where the developer could implement the custom logic before the child removal
        /// </summary>
        /// <param name="child"></param>
        // ReSharper disable UnusedParameter.Global
        // ReSharper disable VirtualMemberNeverOverriden.Global
        //protected virtual void RemovingChild(DisplayListMember child) { }
        // ReSharper restore VirtualMemberNeverOverriden.Global
        // ReSharper restore UnusedParameter.Global

        /// <summary>
        /// The place where the developer could implement the custom logic after the child removal
        /// </summary>
        /// <param name="child"></param>
        // ReSharper disable VirtualMemberNeverOverriden.Global
        // ReSharper disable UnusedParameter.Global
        //protected virtual void ChildRemoved(DisplayListMember child) { }
        // ReSharper restore UnusedParameter.Global
        // ReSharper restore VirtualMemberNeverOverriden.Global

        #endregion

        #region ContainsPoint

        public override bool ContainsPoint(Point point, bool recursive)
        {
            if (base.ContainsPoint(point, recursive))
                return true;

            if (recursive)
            {
                return _children.Exists(delegate(DisplayListMember child)
                                           {
                                               return child.ContainsPoint(point, true);
                                           });
            }
            
            return false;
        }

        #endregion

        #region Draw

        protected override void Render()
        {
            //if (Id == "content_pane")
            //    Debug.Log("   -> Render DOC: " + _children.Count + " [" + this + "]");

            //base.Render(); commented 20130721
            
            //if (!(this is SkinnableComponent))
            base.Render(); //components that use default rendering from the base need this

            RenderChildren();
        }

        /// <summary>
        /// Renders container's children
        /// </summary>
        protected void RenderChildren()
        {
            _drawingList.ForEach(delegate(DisplayListMember child)
            {
                if (child.Visible)
                    child.Draw();
            });
        }

        #endregion

        #region Color

        ///// <summary>
        ///// Does a color apply to children also (tinting)
        ///// Or just the container
        ///// True by default
        ///// </summary>
        //public bool TintChildren = true;

        //protected override void PreRender()
        //{
        //    base.PreRender();

        //    if (!TintChildren)
        //    {
        //        /**
        //         * Reset color after background, but before children are rendered  // TODO: check this sequence
        //         * */
        //        if (null != Color) // && Color.IsSet) // set Alpha and/or color
        //        {
        //            GUI.color = PreRenderColor;
        //        }

        //        if (null != BackgroundColor) // && BackgroundColor.IsSet)
        //        {
        //            GUI.backgroundColor = PreRenderBackgroundColor;
        //        }

        //        if (null != ContentColor) // && ContentColor.IsSet)
        //        {
        //            GUI.contentColor = PreRenderContentColor;
        //        }
        //    }
        //}

        #endregion

        //#region NestLevel

        //public override int NestLevel
        //{
        //    get
        //    {
        //        return base.NestLevel;
        //    }
        //    set
        //    {
        //        if (value != base.NestLevel)
        //        {
        //            base.NestLevel = value;

        //            QChildren.ForEach(delegate(DisplayListMember child)
        //            {
        //                if (null != child)
        //                    child.NestLevel = value + 1;
        //            });
        //        }
        //    }
        //}

        //#endregion

        public override Stage Stage
        {
            get
            {
                return base.Stage;
            }
            internal set
            {
                if (value == base.Stage)
                    return;

                base.Stage = value;

                if (null != value)
                {
                    /*if (this is PopUpAnchor)
                        Debug.Log("PopupAnchor added: " + this);*/

                    if (HasEventListener(FrameworkEvent.ADDED_TO_STAGE))
                        DispatchEvent(new FrameworkEvent(FrameworkEvent.ADDED_TO_STAGE)); // TODO: optimize?
                }
                
                foreach (DisplayListMember dlm in QChildren)
                {
                    dlm.Stage = value;
                }
            }
        }

    }
}