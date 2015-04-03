using System.Collections.Generic;
using eDriven.Gui.Containers;
using eDriven.Gui.Events;
using eDriven.Gui.Layout;
using eDriven.Gui.Managers;
using eDriven.Gui.Plugins;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Components
{
    [Style(Name = "skinClass", Default = typeof(SkinnableContainerSkin))]

    public class SkinnableContainer : SkinnableContainerBase, IContentChildList, ITabManagerClient
    {
        /**
         *  An optional skin part that defines the Group where the content 
         *  children get pushed into and laid out.
         */
        ///<summary>
        /// Content group
        ///</summary>
        [SkinPart(Required = true)]
// ReSharper disable FieldCanBeMadeReadOnly.Global
        public Group ContentGroup;
// ReSharper restore FieldCanBeMadeReadOnly.Global

        private bool _contentModified;

        /**
         *  
         *  Several properties are proxied to contentGroup.  However, when contentGroup
         *  is not around, we need to store values set on SkinnableContainer.  This object 
         *  stores those values.  If contentGroup is around, the values are stored 
         *  on the contentGroup directly.  However, we need to know what values 
         *  have been set by the developer on the SkinnableContainer (versus set on 
         *  the contentGroup or defaults of the contentGroup) as those are values 
         *  we want to carry around if the contentGroup changes (via a new skin). 
         *  In order to store this info effeciently, contentGroupProperties becomes 
         *  a uint to store a series of BitFlags.  These bits represent whether a 
         *  property has been explicitely set on this SkinnableContainer.  When the 
         *  contentGroup is not around, contentGroupProperties is a typeless 
         *  object to store these proxied properties.  When contentGroup is around,
         *  contentGroupProperties stores booleans as to whether these properties 
         *  have been explicitely set or not.
         */
        private ContentGroupProperties _contentGroupProperties = new ContentGroupProperties();

        // Used to hold the content until the contentGroup is created. 
        private Group _placeHolderGroup;
        
// ReSharper disable MemberCanBePrivate.Global
        internal Group CurrentContentGroup
// ReSharper restore MemberCanBePrivate.Global
        {          
            get
            {
                //CreateContentIfNeeded();

                if (null == ContentGroup)
                {
                    //Debug.Log("Adding to placeholder group");
                    if (null == _placeHolderGroup)
                    {
                        _placeHolderGroup = new Group();
                         
                        _placeHolderGroup.AddEventListener(
                            ElementExistenceEvent.ELEMENT_ADD, ContentGroupElementAddedHandler);
                        _placeHolderGroup.AddEventListener(
                            ElementExistenceEvent.ELEMENT_REMOVE, ContentGroupElementRemovedHandler);
                    }
                    return _placeHolderGroup;
                }

                //Debug.Log("Adding to content group");
                    
                return ContentGroup;
            }
        }

        private void ContentGroupElementAddedHandler(Event e)
        {
            ElementExistenceEvent eee = (ElementExistenceEvent)e;
            eee.Element.Owner = this;
        
            // Re-dispatch the event
            DispatchEvent(e);
        }

        private void ContentGroupElementRemovedHandler(Event e)
        {
            ElementExistenceEvent eee = (ElementExistenceEvent)e;
            eee.Element.Owner = null;
        
            // Re-dispatch the event
            DispatchEvent(e);
        }

        ///<summary>
        /// Auto layout
        ///</summary>
        public bool AutoLayout
        {
            get
            {
                if (null != ContentGroup)
                    return ContentGroup.AutoLayout;
                
                // want the default to be true
                bool? autoLayout = _contentGroupProperties.AutoLayout;
                return autoLayout ?? true;
            }
            set
            {
                if (null != ContentGroup)
                {
                    ContentGroup.AutoLayout = value;
                    _contentGroupProperties.AutoLayoutSet = true;
                }
                else
                    _contentGroupProperties.AutoLayout = value;
            }
        }

        ///<summary>
        /// Layout
        ///</summary>
        public LayoutBase Layout
        {
            get
            {
                if (null != ContentGroup)
                    return ContentGroup.Layout;
                return _contentGroupProperties.Layout;
            }
            set
            {
                if (null != ContentGroup)
                {
                    ContentGroup.Layout = value;
                    _contentGroupProperties.LayoutSet = true;
                }
                else
                    _contentGroupProperties.Layout = value;
            }
        }

        protected override void PartAdded(string partName, object instance)
        {
            //Debug.Log("PartAdded: " + partName);

            base.PartAdded(partName, instance);

            if (instance == ContentGroup) 
            {
                //Debug.Log("Part added: ContentGroup: " + ContentGroup);
                if (_contentModified)
                {
                    if (_placeHolderGroup != null)
                    {
                        for (int i = _placeHolderGroup.NumberOfChildren - 1; i >= 0; i--)
                        {
                            var child = _placeHolderGroup.RemoveChildAt(i);
                            ContentGroup.AddChildAt(child, 0);
                        }
                    }
                }
                
                // copy proxied values from contentGroupProperties (if set) to contentGroup

                var newContentGroupProperties = new ContentGroupProperties();

                if (null != _contentGroupProperties.AutoLayout)
                {
                    ContentGroup.AutoLayout = (bool) _contentGroupProperties.AutoLayout;
                    newContentGroupProperties.AutoLayoutSet = true;
                }
                
                if (null != _contentGroupProperties.Layout)
                {
                    ContentGroup.Layout = _contentGroupProperties.Layout;
                    newContentGroupProperties.LayoutSet = true;
                }
                
                _contentGroupProperties = newContentGroupProperties;
                
                ContentGroup.AddEventListener(
                    ElementExistenceEvent.ELEMENT_ADD, ContentGroupElementAddedHandler);
                ContentGroup.AddEventListener(
                    ElementExistenceEvent.ELEMENT_REMOVE, ContentGroupElementRemovedHandler);
                
                if (null != _placeHolderGroup)
                {
                    _placeHolderGroup.RemoveEventListener(
                        ElementExistenceEvent.ELEMENT_ADD, ContentGroupElementAddedHandler);
                    _placeHolderGroup.RemoveEventListener(
                        ElementExistenceEvent.ELEMENT_REMOVE, ContentGroupElementRemovedHandler);
                    
                    _placeHolderGroup = null;
                }
            }
        }

        protected override void PartRemoved(string partName, object instance)
        {
            base.PartRemoved(partName, instance);

            if (instance == ContentGroup) {
                ContentGroup.RemoveEventListener(
                    ElementExistenceEvent.ELEMENT_ADD, ContentGroupElementAddedHandler);
                ContentGroup.RemoveEventListener(
                    ElementExistenceEvent.ELEMENT_REMOVE, ContentGroupElementRemovedHandler);
                
                // copy proxied values from contentGroup (if explicitely set) to contentGroupProperties
                
                ContentGroupProperties newContentGroupProperties = new ContentGroupProperties();
                
                //if (BitFlagUtil.isSet(contentGroupProperties as uint, AUTO_LAYOUT_PROPERTY_FLAG))
                if (_contentGroupProperties.AutoLayoutSet)
                    newContentGroupProperties.AutoLayout = ContentGroup.AutoLayout;
                
                //if (BitFlagUtil.isSet(contentGroupProperties as uint, LAYOUT_PROPERTY_FLAG))
                if (_contentGroupProperties.LayoutSet)
                    newContentGroupProperties.Layout = ContentGroup.Layout;
                    
                _contentGroupProperties = newContentGroupProperties;
                
                if (_contentModified)
                {
                    _placeHolderGroup = new Group();
                         
                    _placeHolderGroup.AddEventListener(
                        ElementExistenceEvent.ELEMENT_ADD, ContentGroupElementAddedHandler);
                    _placeHolderGroup.AddEventListener(
                        ElementExistenceEvent.ELEMENT_REMOVE, ContentGroupElementRemovedHandler);
                }
                
                ContentGroup.Layout = null;
            }
        }

        #region Implementation of IContentChildList

        public virtual List<DisplayListMember> ContentChildren
        {
            get { return CurrentContentGroup.Children; }
        }

        public virtual int NumberOfContentChildren
        {
            get { return CurrentContentGroup.NumberOfChildren; }
        }

        public virtual bool HasContentChild(DisplayListMember child)
        {
            return CurrentContentGroup.HasChild(child);
        }

        public virtual bool ContentContains(DisplayListMember child)
        {
            return CurrentContentGroup.Contains(child);
        }

        // ReSharper disable UnusedMember.Global
        public virtual bool ContentContains(DisplayListMember child, bool includeThisCheck)
        // ReSharper restore UnusedMember.Global
        {
            return CurrentContentGroup.Contains(child, includeThisCheck);
        }

        public virtual DisplayListMember AddContentChild(DisplayListMember child)
        {
            _contentModified = true;
            //Debug.Log("CurrentContentGroup: " + CurrentContentGroup);
            return CurrentContentGroup.AddChild(child);
        }

        public virtual DisplayListMember AddContentChildAt(DisplayListMember child, int index)
        {
            _contentModified = true;
            return CurrentContentGroup.AddChildAt(child, index);
        }

        public virtual DisplayListMember RemoveContentChild(DisplayListMember child)
        {
            _contentModified = true;
            return CurrentContentGroup.RemoveChild(child);
        }

        public virtual DisplayListMember RemoveContentChildAt(int index)
        {
            _contentModified = true;
            return CurrentContentGroup.RemoveChildAt(index);
        }

        public virtual void RemoveAllContentChildren()
        {
            _contentModified = true;
            CurrentContentGroup.RemoveAllChildren();
        }

        public virtual void SwapContentChildren(DisplayListMember firstElement, DisplayListMember secondElement)
        {
            _contentModified = true;
            CurrentContentGroup.SwapChildren(firstElement, secondElement);
        }

        public virtual DisplayListMember GetContentChildAt(int index)
        {
            return CurrentContentGroup.GetChildAt(index);
        }

        public virtual int GetContentChildIndex(DisplayListMember child)
        {
            //return GetContentChildIndex(child);
            return CurrentContentGroup.GetChildIndex(child);
        }

        #endregion

        #region ITabManagerClient

        public bool CircularTabs { get; set; }

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
    }
}