#region License

/*
 
Copyright (c) 2010-2014 Danko Kozar

All rights reserved.
 
*/

#endregion License

using System;
using System.Collections.Generic;
using System.Reflection;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Reflection;
using Component = eDriven.Gui.Components.Component;

namespace eDriven.Gui.Designer.Adapters
{
    /// <summary>
    /// The adapter for Scroller component
    /// </summary>
    [Obfuscation(Exclude = true)]
    [Toolbox(Label="Scroller", Icon = "eDriven/Editor/Controls/scroller")]

    public class ScrollerAdapter : SkinnableContainerAdapter
    {
        /// <summary>
        /// 
        /// </summary>
        public override Type ComponentType
        {
            get { return typeof(Scroller); }
        }

        public override Type DefaultSkinClass
        {
            get { return typeof(ScrollerSkin); }
        }

        /// <summary>
        /// Horizontal scroll policy
        /// </summary>
        [Saveable]
        public ScrollPolicy HorizontalScrollPolicy;

        /// <summary>
        /// Vertical scroll policy
        /// </summary>
        [Saveable]
        public ScrollPolicy VerticalScrollPolicy;

        public override Component NewInstance()
        {
            return new Scroller();
        }

        public override void Apply(Component component)
        {
            base.Apply(component);

            var scroller = (Scroller)component;
            scroller.SetStyle("horizontalScrollPolicy", HorizontalScrollPolicy);
            scroller.SetStyle("verticalScrollPolicy", VerticalScrollPolicy);
        }

        /// <summary>
        /// Adds the content child
        /// </summary>
        /// <param name="child"></param>
        /// <param name="register"></param>
        override public void AddChild(ComponentAdapter child, bool register)
        {
            if (ContentChildren.Contains(child))
                ContentChildren.Remove(child);

            ContentChildren.Add(child);
        
            if (child.FactoryMode)
                return;
        
            if (!Instantiated || !DesignerState.IsPlaying) // not instantiated
                return;

            DoInstantiate(child, register);
        }

        /// <summary>
        /// Adds the content child at position
        /// </summary>
        /// <param name="index"></param>
        /// <param name="child"></param>
        /// <param name="register"></param>
        override public void AddChildAt(int index, ComponentAdapter child, bool register)
        {
            ContentChildren.Remove(child);
        
            if (ContentChildren.Count > 0)
            {
                throw new Exception("Only a single child is supported for Scroller");
            }

            ContentChildren.Add(child);

//        Debug.Log(@"After: 
//" + LayoutOrder);

            if (child.FactoryMode)
                return;

            if (!Instantiated || !DesignerState.IsPlaying) // not instantiated
                return;

            DoInstantiate(child, register);
        }

        /// <summary>
        /// Removes the childAdapter
        /// </summary>
        /// <param name="childAdapter"></param>
        override public void RemoveChild(ComponentAdapter childAdapter)
        {
            ContentChildren.Remove(childAdapter);

            childAdapter.transform.parent = null;
        
            if (!Instantiated || !DesignerState.IsPlaying) // not instantiated
                return;

            var container = (Group)Component;
            if (null != container)
            {
                container.RemoveContentChild(childAdapter.Component);
            }
        }

        /// <summary>
        /// Removes all children
        /// </summary>
        override public void RemoveAllChildren()
        {
            //DepthOrder.Clear();
            //LayoutOrder.Clear();

            //if (!Instantiated && !State.IsPlaying) // not instantiated
            if (!Instantiated || !DesignerState.IsPlaying) // not instantiated // 20130512
                return;

            //Debug.Log("transform.childCount: " + transform.childCount);

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                var childAdapter = GuiLookup.GetAdapter(transform.GetChild(i));
                //Debug.Log("childAdapter: " + childAdapter);
                if (null != childAdapter)
                    RemoveChild(childAdapter);
            }

            ContentChildren.Clear();
            //ContentChildren = new List<ComponentAdapter>(ContentChildren);
        }

        /// <summary>
        /// Gets child index
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        override public int GetChildIndex(ComponentAdapter child)
        {
            //return LayoutOrder.FindIndex(delegate(string val) { return val == child.Guid; });
            return ContentChildren.FindIndex(delegate(ComponentAdapter componentAdapter)
            {
                return componentAdapter == child;
            });
        }

        /// <summary>
        /// Changes the child position
        /// </summary>
        /// <param name="index"></param>
        /// <param name="child"></param>
        override public void Reorder(int index, ComponentAdapter child)
        {
            AddChildAt(index, child, true);
        }

        #region Child instantiation

        /// <summary>
        /// Instantiates
        /// </summary>
        /// <param name="childAdapter"></param>
        /// <param name="register"></param>
        override public void DoInstantiate(ComponentAdapter childAdapter, bool register)
        {
            //Debug.Log("DoInstantiate1: " + childAdapter);

            if (!childAdapter.Instantiated)
            {
                childAdapter.DoInstantiate(register);
            }

            //Debug.Log("DoInstantiate2: " + childAdapter.Component);

            var scroller = (Scroller)Component;
            if (null != scroller)
            {
                //scroller.AddContentChild(childAdapter.Component);
                var group = childAdapter.Component as GroupBase;
                if (null == group)
                {
                    throw new Exception(string.Format(@"Scroller can have only a single child, extending the GroupBase ({0} supplied)", childAdapter.Component));
                }
                scroller.Viewport = group;
            }
        }

        /// <summary>
        /// Recursively instantiates components specified by descriptors in the hierarchy
        /// </summary>
        /// <param name="assignToDescriptor">Register to transform, and put into registry so it is available by clicking</param>
        public override void InstantiateChildren(bool assignToDescriptor)
        {
            // getting references to collections
            var childGroupDescriptors = DesignerReflection.GetChildGroupsReferences(this);

            foreach (ChildGroupDescriptor groupDescriptor in childGroupDescriptors)
            {
                List<ComponentAdapter> childAdapters = groupDescriptor.GetChildAdaptersCollection(this);

                if (null == childAdapters || childAdapters.Count == 0)
                    continue;

                //var attr = groupDescriptor.Attribute;

                if (null == Component) // not instantiated
                    return;

                var scroller = (Scroller)Component;
                if (null != groupDescriptor.TargetContainerMemberInfo)
                {
                    /*targetContainer = (Group)Core.Reflection.ReflectionUtil.GetMemberValue(
                        groupDescriptor.TargetContainerMemberInfo,
                        Component
                    );*/
                }

                if (!gameObject.activeInHierarchy) // no descriptors on this node or descriptor disabled. skip
                    continue; // 20130426

                try
                {
                    Component component = childAdapters[0].Produce(!FactoryMode, assignToDescriptor);
                    scroller.Viewport = (IViewport)component;
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError(string.Format(@"Scroller viewport should be of IViewport type: {0}
{1}", this, ex));
                }

                //ComponentAdapterUtil.PopulateContainer(assignToDescriptor, childAdapters.ToArray(), targetContainer, false, true);
            }
        }
    
        #endregion
    }
}