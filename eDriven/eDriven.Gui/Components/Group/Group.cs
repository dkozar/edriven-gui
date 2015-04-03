using System.Collections.Generic;
using eDriven.Gui.Containers;
using eDriven.Gui.GUIStyles;
using eDriven.Gui.Managers;
using eDriven.Gui.Plugins;
using eDriven.Gui.Styles;
using UnityEngine;

namespace eDriven.Gui.Components
{
    [Style(Name = "backgroundStyle", Type = typeof(GUIStyle), ProxyType = typeof(GroupBackgroundStyle))]
    [Style(Name = "backgroundColor", Type = typeof(Color), Default = 0xffffff)]
    [Style(Name = "showBackground", Type = typeof(bool), Default = false)]

    public class Group : GroupBase, IContentChildList, ITabManagerClient
    {
        #region ITabManagerClient

        // TODO
        public bool CircularTabs { get; set; }

        // TODO
        public bool CircularArrows { get; set; }

        /// <summary>
        /// Returns the tab children
        /// This could be overriden in subclass
        /// </summary>
        /// <returns></returns>
        public virtual List<DisplayListMember> GetTabChildren()
        {
            return ContentChildren.FindAll(delegate(DisplayListMember child)
            {
                InteractiveComponent c = child as InteractiveComponent;
                //return null != c && c.Enabled && c.Visible && c.FocusEnabled;
                return FocusManager.IsFocusCandidate(c);
            });
        }

        #endregion

        #region Styles

        /*protected override void UpdateDisplayList(float width, float height)
        {
            //Debug.Log("UpdateDisplayList: " + this);
            _backgroundStyle = (GUIStyle)GetStyle("backgroundStyle");
            
            // temp fix style manager 20131109 - (Color)GetStyle("backgroundColor");
            // look also Component_styles class
            // GetStyle somehow doesn't deliver the hardcoded default
            _showBackground = (bool)GetStyle("showBackground");
            _bgColor = (Color)GetStyle("backgroundColor");

            // NOTE: Turning padding propagation to layout ON overrides explicit layout settings with styled versions:
            //if (Layout is HorizontalLayout) {
            //    var hl = ((HorizontalLayout) Layout);
            //    hl.PaddingLeft = (float)Convert.ToDouble(GetStyle("paddingLeft"));
            //    hl.PaddingRight = (float)Convert.ToDouble(GetStyle("paddingRight"));
            //    hl.PaddingTop = (float)Convert.ToDouble(GetStyle("paddingTop"));
            //    hl.PaddingBottom = (float)Convert.ToDouble(GetStyle("paddingBottom"));
            //}
            //else if (Layout is VerticalLayout)
            //{
            //    var vl = ((VerticalLayout) Layout);
            //    vl.PaddingLeft = (float)Convert.ToDouble(GetStyle("paddingLeft"));
            //    vl.PaddingRight = (float)Convert.ToDouble(GetStyle("paddingRight"));
            //    vl.PaddingTop = (float)Convert.ToDouble(GetStyle("paddingTop"));
            //    vl.PaddingBottom = (float)Convert.ToDouble(GetStyle("paddingBottom"));
            //}

            base.UpdateDisplayList(width, height);
        }*/

        public override void NotifyStyleChangeInChildren(string styleProp, object value, bool recursive)
        {
            if (!recursive) 
                return;
            
            int n = NumberOfContentChildren;
            for (int i = 0; i < n; i++)
            {
                ISimpleStyleClient child = GetContentChildAt(i) as ISimpleStyleClient;

                if (null != child)
                {
                    child.StyleChanged(styleProp);

                    // Always recursively call this function because of my
                    // descendants might have a styleName property that points
                    // to this object.  The recursive flag is respected in
                    // Container.notifyStyleChangeInChildren.
                    if (child is IStyleClient)
                        ((IStyleClient)child).NotifyStyleChangeInChildren(styleProp, value, true);
                }
            }
        }

        public override void RegenerateStyleCache(bool recursive)
        {
            /*Debug.Log("RegenerateStyleCache: " + this);
            InitProtoChain();

            // Recursively call this method on each child.
            var n = NumberOfContentChildren; // QNumberOfChildren;

            for (int i = 0; i < n; i++)
            {
                var child = GetContentChildAt(i); // QGetChildAt(i);

                if (child is IStyleClient)
                {
                    // Does this object already have a proto chain? 
                    // If not, there's no need to regenerate a new one.
                    if (((IStyleClient)child).InheritingStyles !=
                        StyleProtoChain.STYLE_UNINITIALIZED)
                    {
                        ((IStyleClient)child).RegenerateStyleCache(recursive);
                    }
                }
            }*/

            base.RegenerateStyleCache(recursive);

            if (null != _contentPane)
            {
                // Recursively call this method on each child.
                var n = _contentPane.QNumberOfChildren; // QNumberOfChildren;

                for (int i = 0; i < n; i++)
                {
                    var child = _contentPane.QGetChildAt(i); // QGetChildAt(i);

                    if (child is IStyleClient)
                    {
                        // Does this object already have a proto chain? 
                        // If not, there's no need to regenerate a new one.
                        if (((IStyleClient)child).InheritingStyles !=
                            StyleProtoChain.STYLE_UNINITIALIZED)
                        {
                            ((IStyleClient)child).RegenerateStyleCache(recursive);
                        }
                    }
                }
            }
        }

        #endregion

        #region Render

        //private Color _oldColor;

        /*protected override void PreRender()
        {
            base.PreRender();

            // drawing background
            if (_showBackground && Event.current.type == EventType.repaint)
            {
                //Debug.Log("ClipAndEnableScrolling: " + ClipAndEnableScrolling);
                Rect bounds = ClipAndEnableScrolling ? LocalRenderingRect : RenderingRect;

                _backgroundStyle = (GUIStyle) GetStyle("backgroundStyle");
                //if (Id == "hbox")
                //    Debug.Log("bounds: " + bounds);

                if (null != _backgroundStyle)
                {
                    if (Color.white != _bgColor)
                    {
                        _oldColor = GUI.color;
                        GUI.color = _bgColor;
                    }

                    //if (Id == "hbox")
                    //    Debug.Log("drawing background");

                    _backgroundStyle.Draw(bounds, //new Rect(10, 10, 699, 56), //   bounds, 
                        this == MouseEventDispatcher.MouseTarget,
                        this == MouseEventDispatcher.MouseDownComponent,
                        false, // this == FocusManager.Instance.FocusedComponent, // fix for background bug 20130704,
                        this == FocusManager.Instance.FocusedComponent
                    );

                    if (Color.white != _bgColor)
                    {
                        GUI.color = _oldColor;
                    }
                }
            }
        }*/

        #endregion

        #region Implementation of IContentChildList

        public virtual List<DisplayListMember> ContentChildren
        {
            get { return Children; }
        }

        override public int NumberOfContentChildren
        {
            get { return NumberOfChildren; }
        }

        public virtual bool HasContentChild(DisplayListMember child)
        {
            return HasChild(child);
        }

        public virtual bool ContentContains(DisplayListMember child)
        {
            return Contains(child);
        }

        // ReSharper disable UnusedMember.Global
        public virtual bool ContentContains(DisplayListMember child, bool includeThisCheck)
        // ReSharper restore UnusedMember.Global
        {
            return Contains(child, includeThisCheck);
        }

        public virtual DisplayListMember AddContentChild(DisplayListMember child)
        {
            return AddChild(child);
        }

        public virtual DisplayListMember AddContentChildAt(DisplayListMember child, int index)
        {
            return AddChildAt(child, index);
        }

        public virtual DisplayListMember RemoveContentChild(DisplayListMember child)
        {
            return RemoveChild(child);
        }

        public virtual DisplayListMember RemoveContentChildAt(int index)
        {
            return RemoveChildAt(index);
        }

        public virtual void RemoveAllContentChildren()
        {
            RemoveAllChildren();
        }

        public virtual void SwapContentChildren(DisplayListMember firstElement, DisplayListMember secondElement)
        {
            SwapChildren(firstElement, secondElement);
        }

        override public DisplayListMember GetContentChildAt(int index)
        {
            return GetChildAt(index);
        }

        override public int GetContentChildIndex(DisplayListMember child)
        {
            //return GetContentChildIndex(child);
            return GetChildIndex(child);
        }

        #endregion
    }
}