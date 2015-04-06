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
using System.Text;
using eDriven.Core.Reflection;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Designer.Collections;
using eDriven.Gui.Designer.Util;
using eDriven.Gui.Layout;
using eDriven.Gui.Reflection;
using UnityEngine;
using Component = eDriven.Gui.Components.Component;

namespace eDriven.Gui.Designer.Adapters
{
    /// <summary>
    /// The adapter for Group component
    /// </summary>
    [Obfuscation(Exclude = true)]
    [Toolbox(Label="Group", Icon = "eDriven/Editor/Controls/container")]

    public class GroupAdapter : ComponentAdapter
    {
        /// <summary>
        /// The type of the adapted component is Group
        /// </summary>
        public override Type ComponentType
        {
            get { return typeof(Group); }
        }

        // NOTE: no DefaultSkinClass for a Group!

        /// <summary>
        /// The enumeration used by the layout display
        /// </summary>
        public enum LayoutEnum
        {
            /// <summary>
            /// Absolute layout
            /// </summary>
            Absolute, 
        
            /// <summary>
            /// Box horizontal
            /// </summary>
            Horizontal, 
        
            /// <summary>
            /// Box vertical
            /// </summary>
            Vertical, 
        
            /// <summary>
            /// Tile layout
            /// </summary>
            Tile
        }

        /// <summary>
        /// Clip and enable scrolling
        /// </summary>
        [Saveable] 
        public bool ClipAndEnableScrolling;

        #region Layout

        /// <summary>
        /// If using a complex layout, LayoutEnum.None is the default
        /// </summary>
        [Saveable]
        public LayoutEnum Layout = LayoutEnum.Absolute; // default, just as Group

        /// <summary>
        /// Gap if using HorizontalLayout or VerticalLayout
        /// </summary>
        [Saveable]
        public int Gap;

        /// <summary>
        /// HorizontalAlign direction if using TileLayout
        /// </summary>
        [Saveable]
        public HorizontalAlign HorizontalAlign;

        /// <summary>
        /// VerticalAlign direction if using TileLayout
        /// </summary>
        [Saveable]
        public VerticalAlign VerticalAlign;

        /// <summary>
        /// Orientation for TileLayout
        /// </summary>
        [Saveable]
        public TileOrientation TileOrientation;

        /// <summary>
        /// HorizontalGap if using TileLayout
        /// </summary>
        [Saveable]
        public int HorizontalGap;

        /// <summary>
        /// VerticalGap if using TileLayout
        /// </summary>
        [Saveable]
        public int VerticalGap;

        /// <summary>
        /// RequestedRowCount if using TileLayout
        /// </summary>
        [Saveable]
        public int RequestedRowCount;

        /// <summary>
        /// RequestedColumnCount if using TileLayout
        /// </summary>
        [Saveable]
        public int RequestedColumnCount;

        /// <summary>
        /// UseColumnWidth if using TileLayout
        /// </summary>
        [Saveable]
        public bool UseColumnWidth;

        /// <summary>
        /// ColumnWidth if using TileLayout
        /// </summary>
        [Saveable]
        public int ColumnWidth;

        /// <summary>
        /// UseRowHeight if using TileLayout
        /// </summary>
        [Saveable]
        public bool UseRowHeight;

        /// <summary>
        /// ColumnWidth if using TileLayout
        /// </summary>
        [Saveable]
        public int RowHeight;

        /// <summary>
        /// RowAlign if using TileLayout
        /// </summary>
        [Saveable]
        public RowAlign RowAlign;

        /// <summary>
        /// ColumnAlign if using TileLayout
        /// </summary>
        [Saveable]
        public ColumnAlign ColumnAlign;

        /// <summary>
        /// True to Sync all 4 padding values
        /// </summary>
        [Saveable]
        public bool SyncPadding = true;

        /// <summary>
        /// Left padding
        /// </summary>
        [Saveable]
        public int PaddingLeft;

        /// <summary>
        /// Right padding
        /// </summary>
        [Saveable]
        public int PaddingRight;

        /// <summary>
        /// Top padding
        /// </summary>
        [Saveable]
        public int PaddingTop;

        /// <summary>
        /// Bottom padding
        /// </summary>
        [Saveable]
        public int PaddingBottom;
    
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Component NewInstance()
        {
            return new Group();
        }

        public override void Apply(Component component)
        {
            base.Apply(component);

            var group = component as Group;
            if (null == group) // could be Scroller or other non-group component able to have children
                return;

            group.ClipAndEnableScrolling = ClipAndEnableScrolling;

            switch (Layout)
            {
                case LayoutEnum.Absolute:
                    group.Layout = new AbsoluteLayout();
                    break;
                case LayoutEnum.Horizontal:
                    group.Layout = new HorizontalLayout
                    {
                        Gap = Gap,
                        HorizontalAlign = HorizontalAlign,
                        VerticalAlign = VerticalAlign,
                        PaddingLeft = PaddingLeft,
                        PaddingRight = PaddingRight,
                        PaddingTop = PaddingTop,
                        PaddingBottom = PaddingBottom
                    };
                    break;
                case LayoutEnum.Vertical:
                    group.Layout = new VerticalLayout
                    {
                        Gap = Gap,
                        HorizontalAlign = HorizontalAlign,
                        VerticalAlign = VerticalAlign,
                        PaddingLeft = PaddingLeft,
                        PaddingRight = PaddingRight,
                        PaddingTop = PaddingTop,
                        PaddingBottom = PaddingBottom
                    };
                    break;
                case LayoutEnum.Tile:
                    group.Layout = new TileLayout
                    {
                        Orientation = TileOrientation,
                        HorizontalGap = HorizontalGap,
                        VerticalGap = VerticalGap,
                        HorizontalAlign = HorizontalAlign,
                        VerticalAlign = VerticalAlign,
                        RowHeight = UseRowHeight ? RowHeight : (float?) null,
                        ColumnWidth = UseColumnWidth ? ColumnWidth : (float?) null,
                        RequestedRowCount = RequestedRowCount,
                        RequestedColumnCount = RequestedColumnCount,
                        RowAlign = RowAlign,
                        ColumnAlign = ColumnAlign
                    };
                    break;
                default:
                    group.Layout = null;
                    break;
            }
        }

        /// <summary>
        /// Content children
        /// </summary>
        [Saveable]
        [SerializeField]
        //public ComponentAdapter[] ContentChildren = new ComponentAdapter[] { };
        [ChildCollection(ShowHeader = true, Label = "Content", Icon = "eDriven/Editor/Icons/group_content")]
        public List<ComponentAdapter> ContentChildren = new List<ComponentAdapter>();

        /// <summary>
        /// Layout order
        /// </summary>
        //[Saveable] // not saveable anymore
        [SerializeField]
        public SaveableStringCollection LayoutOrder = new SaveableStringCollection();

        /// <summary>
        /// The collection of GUIDs of controls being special (non-content) children, like button group children, tools etc.
        /// </summary>
        [Saveable]
        [SerializeField]
        public SaveableStringCollection NonContentChildren = new SaveableStringCollection();

        /// <summary>
        /// Fires on enable
        /// </summary>
        protected override void OnEnableImpl()
        {
            base.OnEnableImpl();
            LayoutOrder.ComponentAdapter = this;
        }

        /// <summary>
        /// Returns true if the child specified by GUID is the non-content child
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public bool NonContentChildRegistered(string guid)
        {
            return NonContentChildren.Contains(guid);
        }

        /// <summary>
        /// Returns the string presentation of layout order
        /// </summary>
        /// <returns></returns>
        public string DescribeLayoutOrder()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in LayoutOrder.Items)
            {
                if (!NonContentChildRegistered(s))
                    sb.AppendLine(s);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Adds the content child
        /// </summary>
        /// <param name="child"></param>
        /// <param name="register"></param>
        public virtual void AddChild(ComponentAdapter child, bool register)
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
        public virtual void AddChildAt(int index, ComponentAdapter child, bool register)
        {
            ContentChildren.Remove(child);
        
            if (ContentChildren.Count >= index)
                ContentChildren.Insert(index, child);
            else
                ContentChildren.Add(child);

            if (child.FactoryMode)
                return;

            if (!Instantiated || !DesignerState.IsPlaying) // not instantiated
                return;

            DoInstantiateAt(child, index, register);
        }

        /// <summary>
        /// Removes the childAdapter
        /// </summary>
        /// <param name="childAdapter"></param>
        public virtual void RemoveChild(ComponentAdapter childAdapter)
        {
            if (null == childAdapter)
                return;

            ContentChildren.Remove(childAdapter);

            childAdapter.transform.parent = null;
        
            if (!Instantiated || !DesignerState.IsPlaying) // not instantiated
                return;

            var container = (IContentChildList)Component;
            if (null != container)
            {
                container.RemoveContentChild(childAdapter.Component);
            }
        }

        /// <summary>
        /// Removes all children
        /// </summary>
        public virtual void RemoveAllChildren()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                var childAdapter = GuiLookup.GetAdapter(transform.GetChild(i));
                //Debug.Log("childAdapter: " + childAdapter);
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
        public virtual int GetChildIndex(ComponentAdapter child)
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
        public virtual void Reorder(int index, ComponentAdapter child)
        {
            //Debug.Log("Reordering " + child);
            AddChildAt(index, child, true);
        }

        #region Child instantiation

        /// <summary>
        /// Instantiates
        /// </summary>
        /// <param name="childAdapter"></param>
        /// <param name="register"></param>
        public virtual void DoInstantiate(ComponentAdapter childAdapter, bool register)
        {
            if (!childAdapter.Instantiated)
            {
                childAdapter.DoInstantiate(register);
            }

            var container = Component as IContentChildList;
            if (null == container)
            {
                Debug.LogError(Component.GetType().Name + " cannot host children");
                return;
            }
            container.AddContentChild(childAdapter.Component);
        }

        private void DoInstantiateAt(ComponentAdapter childAdapter, int index, bool useAsInstance)
        {
            if (!childAdapter.Instantiated)
            {
                childAdapter.DoInstantiate(useAsInstance);
            }

            var container = (IContentChildList)Component;
            if (null != container)
            {
                container.AddContentChildAt(childAdapter.Component, index);
            }
        }

        /// <summary>
        /// </summary>
        public readonly Dictionary<string, Transform> GuidToTransformDict = new Dictionary<string, Transform>();

        /// <summary>
        /// Returns true if all the (transform) children are not properly mapped to child collection(s)
        /// </summary>
        public virtual bool HasMappingErrors
        {
            get
            {
                return transform.childCount != ContentChildren.Count;
            }
        }

        /// <summary>
        /// Recursively instantiates components specified by descriptors in the hierarchy
        /// </summary>
        /// <param name="assignToDescriptor">Register to transform, and put into registry so it is available by clicking</param>
        public virtual void InstantiateChildren(bool assignToDescriptor)
        {
            // getting references to collections
            var childGroupDescriptors = DesignerReflection.GetChildGroupsReferences(this);

            foreach (ChildGroupDescriptor groupDescriptor in childGroupDescriptors)
            {
                List<ComponentAdapter> childAdapters = groupDescriptor.GetChildAdaptersCollection(this);

                if (null == childAdapters)
                    continue;
                
                if (null == Component) // not instantiated
                    return;

                Group targetContainer;
                if (null != groupDescriptor.TargetContainerMemberInfo)
                {
                    /**
                     * 1. Try reading the member as a Group
                     * */
                    targetContainer = (Group) CoreReflector.GetMemberValue(
                        groupDescriptor.TargetContainerMemberInfo,
                        Component
                    );

                }
                else
                {
                    /**
                     * 2. Try casting this to Group (if this is a Stage class for instance)
                     * */
                    targetContainer = Component as Group;
                }

                ComponentAdapterUtil.PopulateContainer(Component, targetContainer, childAdapters.ToArray(), assignToDescriptor, false, true);
            }
        }
    
        #endregion
    }
}