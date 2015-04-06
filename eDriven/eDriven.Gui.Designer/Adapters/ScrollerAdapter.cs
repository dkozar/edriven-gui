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
            if (!Instantiated || !DesignerState.IsPlaying) // not instantiated // 20130512
                return;

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                var childAdapter = GuiLookup.GetAdapter(transform.GetChild(i));
                if (null != childAdapter)
                    RemoveChild(childAdapter);
            }

            ContentChildren.Clear();
        }

        /// <summary>
        /// Gets child index
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        override public int GetChildIndex(ComponentAdapter child)
        {
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
            if (!childAdapter.Instantiated)
            {
                childAdapter.DoInstantiate(register);
            }

            var scroller = (Scroller)Component;
            if (null != scroller)
            {
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
            }
        }
    
        #endregion
    }
}