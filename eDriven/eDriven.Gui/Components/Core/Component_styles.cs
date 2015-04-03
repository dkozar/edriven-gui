using System;
using System.Collections;
using System.Collections.Generic;
using eDriven.Animation;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;
using UnityEngine;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Components
{
    /**
     * Handles style-related issues
     * */
    public partial class Component
    {
        /// <summary>
        /// The list of altered style names
        /// Altered styles are styles set by the user using the SetStyle method
        /// These styles does not need to be overriden by the proxy, since they have presedence
        /// </summary>
        private readonly List<string> _overrides = new List<string>();
        
        #region Implementation of IStyleClient

        private StyleTable _inheritingStyles = StyleProtoChain.STYLE_UNINITIALIZED;

        /// <summary>
        /// Inheriting styles
        /// </summary>
        public StyleTable InheritingStyles
        {
            get { return _inheritingStyles; }
            set { _inheritingStyles = value; }
        }

        private StyleTable _nonInheritingStyles = StyleProtoChain.STYLE_UNINITIALIZED;
        /// <summary>
        /// Non-inheriting styles
        /// </summary>
        public StyleTable NonInheritingStyles
        {
            get { return _nonInheritingStyles; }
            set { _nonInheritingStyles = value; }
        }

        //----------------------------------
        //  styleDeclaration
        //----------------------------------

        /**
         *  
         *  Storage for the styleDeclaration property.
         */
        private StyleDeclaration _styleDeclaration;

        /**
         *  Storage for the inline inheriting styles on this object.
         *  This StyleDeclaration is created the first time that 
         *  the <code>setStyle()</code> method
         *  is called on this component to set an inheriting style.
         *  Developers typically never need to access this property directly.
         */
        /// <summary>
        /// Style declaration attached to this component
        /// </summary>
// ReSharper disable once ConvertToAutoProperty
        public StyleDeclaration StyleDeclaration
        {
            get
            {
                return _styleDeclaration;
            }
            set
            {
                _styleDeclaration = value;
            }
        }

        #endregion
        
        #region Get / set

        /// <summary>
        /// Sets the style
        /// </summary>
        /// <param name="styleName">Style name</param>
        /// <param name="newValue">Style value</param>
        public void SetStyle(string styleName, object newValue)
        {
            /**
             * Setting the style creates a style declaration specific to this object
             * */
            StyleProtoChain.SetStyle(this, styleName, newValue);
        }

        /// <summary>
        /// Gets the specified style
        /// </summary>
        /// <param name="styleName">Style name (string)</param>
        /// <returns></returns>
        public virtual object GetStyle(string styleName)
        {
            /**
             * Just reading the style from one of the two preprocessed collections
             * Style processing is expensive, so we do it rarely
             * Reading from preprocessed tables is not
             * */
            return StyleManager.Instance.IsInheritingStyle(styleName) ?
               _inheritingStyles.GetValue(styleName) :
               _nonInheritingStyles.GetValue(styleName);
        }

        /// <summary>
        /// Returns true if component has style defined by styleName
        /// </summary>
        /// <param name="styleName"></param>
        /// <returns></returns>
        public bool HasStyle(string styleName)
        {
            return null != GetStyle(styleName);
        }

        /// <summary>
        /// Clears the style defined by styleName
        /// </summary>
        /// <param name="styleName"></param>
        public void ClearStyle(string styleName)
        {
            SetStyle(styleName, StyleDeclaration.UNDEFINED);
        }

        /// <summary>
        /// A shorthend for calling SetStyle multiple times
        /// </summary>
        public Hashtable Styles
        {
            set
            {
                foreach (DictionaryEntry entry in value)
                {
                    var key = entry.Key as string;

                    if (null == key)
                    {
                        Debug.LogError(string.Format("Styles: Hashtable entry key has to be a string: {0}. Skipping...", entry.Key));
                        return;
                    }

                    SetStyle(key, entry.Value);
                }
            }
        }

        #endregion

        /// <summary>
        /// Runs each time after any of styles changes<br/>
        /// The name and the new value for the style are passed as arguments
        /// </summary>
        /// <param name="styleProp">Style property</param>
        public virtual void StyleChanged(string styleProp)
        {
            //if (styleProp == "paddingLeft")
            //    Debug.Log("Component -> StyleChanged: " + styleProp);

            StyleProtoChain.StyleChanged(this, styleProp);

            if (DispatchStyleChanges)
            {
                if (null != styleProp && (styleProp != "styleName"))
                {
                    if (HasEventListener(styleProp + "Changed"))
                        DispatchEvent(new Event(styleProp + "Changed"));
                }
                else
                {
                    if (HasEventListener("allStylesChanged"))
                        DispatchEvent(new Event("allStylesChanged"));
                }
            }

            switch (styleProp)
            {
                case "disabledOverlayStyle":
                case "overlayStyle":
                case "showOverlay":
                case "color":
                case "backgroundColor":
                    /*Debug.Log( ComponentUtil.PathToString(this, "->") + " ---> " + (Color)GetStyle("backgroundColor"));
                    InvalidateDisplayList();
                    break;*/
                case "contentColor":
                    InvalidateDisplayList();
                    break;
            }
        }

        protected override void UpdateDisplayList(float width, float height)
        {
            base.UpdateDisplayList(width, height);

            _disabledOverlay = (GUIStyle)GetStyle("disabledOverlayStyle");
            _overlayStyle = (GUIStyle)GetStyle("overlayStyle");
            ShowOverlay = (bool) GetStyle("showOverlay");
            _color = (Color) GetStyle("color");
            _backgroundColor = (Color)GetStyle("backgroundColor");
            _contentColor = (Color)GetStyle("contentColor");
            //_usePointerCursor = (Color)GetStyle("usePointerCursor");
        }

        /// <summary>
        /// Propagates style changes to child components
        /// </summary>
        /// <param name="styleProp"></param>
        /// <param name="value"></param>
        /// <param name="recursive"></param>
        public virtual void NotifyStyleChangeInChildren(string styleProp, object value, bool recursive)
        {
            int n = NumberOfChildren;
            for (int i = 0; i < n; i++)
            {
                ISimpleStyleClient child = GetChildAt(i) as ISimpleStyleClient;

                if (null != child)
                {
                    child.StyleChanged(styleProp);

                    // Always recursively call this function because of my
                    // descendants might have a styleName property that points
                    // to this object.  The recursive flag is respected in
                    // Container.notifyStyleChangeInChildren.
                    if (child is IStyleClient)
                        ((IStyleClient)child).NotifyStyleChangeInChildren(styleProp, value, recursive);
                }
            }
        }
 
        /// <summary>
        /// Refenerates component styles
        /// </summary>
        /// <param name="recursive"></param>
        public virtual void RegenerateStyleCache(bool recursive)
        {
            /*if (_regCount > 10)
                return;

            _regCount++;*/

            //Debug.Log("RegenerateStyleCache: " + this);
            InitProtoChain();

            // Recursively call this method on each child.
            var n = NumberOfChildren; // QNumberOfChildren;
                
            for (int i = 0; i < n; i++)
            {
                var child = GetChildAt(i); // QGetChildAt(i);

                var client = child as IStyleClient;
                if (client != null)
                {
                    // Does this object already have a proto chain? 
                    // If not, there's no need to regenerate a new one.
                    if (client.InheritingStyles !=
                        StyleProtoChain.STYLE_UNINITIALIZED)
                    {
                        client.RegenerateStyleCache(recursive);
                    }
                }
            }
        }

        internal void InitProtoChain()
        {
            StyleProtoChain.InitProtoChain(this);
        }

        #region Style name

        private object _styleName;

        /// <summary>
        /// The style name for this component<br/>
        /// Used by styling subsistem for style matching
        /// </summary>
        public virtual object StyleName
        {
            get
            {
                return _styleName;
            }
            set
            {
                //if (value == _styleName) // commented 20120626 - because setting style mappers from mappers doesn't work properly
                //    return; // no change

                _styleName = value;
                //_styleMapperChanged = true;

                // If inheritingStyles is undefined, then this object is being
                // initialized and we haven't yet generated the proto chain.
                // To avoid redundant work, don't bother to create
                // the proto chain here.
                if (_inheritingStyles == StyleProtoChain.STYLE_UNINITIALIZED)
                    return;

                RegenerateStyleCache(true);

                StyleChanged("styleName");

                NotifyStyleChangeInChildren("styleName", value, true);
            }
        }

        #endregion

        #region Skin class

        /// <summary>
        /// Style mapper being used for styling this component and/or its children
        /// </summary>
        public virtual Type SkinClass
        {
            get
            {
                return (Type)GetStyle("skinClass");
            }
            set
            {
                SetStyle("skinClass", value);
            }
        }

        #endregion

        #region Flush

        private Component _childCmp;

        /// <summary>
        /// Clears the altered styles, alowing the reading of the new style definition
        /// Used by the designer for "live" changing styles using the style mappers
        /// We are flushing the styles recursivelly, on all children
        /// </summary>
        public void FlushStyles()
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("FlushStyles: " + this + " [initialized: " + Initialized + "]");
            }
#endif
            _overrides.Clear();
            //_styles.Clear();
            InvalidateSize();
            foreach (DisplayListMember child in Children)
            {
                _childCmp = child as Component;
                if (null != _childCmp)
                {
                    _childCmp.FlushStyles();
                }
            }
        }

        #endregion

        /// <summary>
        /// Plays the styled effect if such style set
        /// </summary>
        /// <param name="effectStyleName"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        override public bool TriggerEffect(string effectStyleName, object target)
        {
            //if (null != GetStyle(effectStyleName))
            //{
            //    ((IAnimation)GetStyle(effectStyleName)).Play(this);
            //    return true;
            //}

            var effect = GetStyle(effectStyleName);
            if (null != effect)
            //if (_styles.ContainsKey(effectStyleName))
            {
                ITweenFactory animation = null;
                try
                {
                    animation = (ITweenFactory) effect; // _styles[effectStyleName];
                }
                catch (InvalidCastException)
                {
                    Debug.LogError(string.Format("* INVALID CAST: {0} {1}", this, effectStyleName));
                }

                if (null == animation)
                {
                    //Debug.LogWarning(string.Format("Style [{0}] is not an effect (it doesn't implement IAnimation). Skipping...", effectStyleName)); // commented 20120919 because of the null effects
                    return false;
                }

                //((ITweenFactory)_styles[effectStyleName]).Play(target);
                animation.Play(target);
                return true;
            }

            return false;
        }

        /**
         *  Finds the type selectors for this Component instance.
         *  The algorithm walks up the superclass chain.
         *  For example, suppose that class MyButton extends Button.
         *  A MyButton instance will first look for a MyButton type selector
         *  then, it will look for a Button type selector.
         *  then, it will look for a Component type selector.
         *  (The superclass chain is considered to stop at Component, not Object.)
         *
         *  Returns: An Array of type selectors for this Component instance.
         */
        ///<summary>
        ///</summary>
        ///<returns></returns>
        public List<StyleDeclaration> GetClassStyleDeclarations()
        {
            return StyleProtoChain.GetClassStyleDeclarations(this);
        }

        /**
         *  The state to be used when matching CSS pseudo-selectors. By default
         *  this is the currentState.
         */ 
        /// <summary>
        /// 
        /// </summary>
        protected string CurrentCSSState
        {
            get
            {
                return CurrentState;
            }
        }

        ///<summary>
        ///</summary>
        public IStyleClient StyleParent
        {
            get
            {
                return Parent as IStyleClient;
            }
        }
        
        public bool MatchesCSSType(string cssType)
        {
            return StyleProtoChain.MatchesCSSType(GetType(), cssType);
        }

        /// <summary>
        /// Evaluates the skin style<br/>
        /// This method should be used for getting a skin instead of reading a raw style using the GetStyle method<br/>
        /// This is because the style is being typed as object (so it can be both a class or a classname (string))
        /// </summary>
        /// <returns></returns>
        protected Type EvaluateSkinClassFromStyle(string styleName = "skinClass")
        {
            /**
             * PS: This is due to obfuscator, because Crypto converts the type to string (only for object-type values) :)
             * */
            var skinClassStyle = GetStyle(styleName);
            Type skinClass = skinClassStyle as Type;
            if (null == skinClass)
            {
                var className = skinClassStyle as string;
                if (null != className)
                    skinClass = GlobalTypeDictionary.Instance[className];
            }
            return skinClass;
        }
    }
}
