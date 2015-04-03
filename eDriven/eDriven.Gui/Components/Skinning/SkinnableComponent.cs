using System;
using System.Collections.Generic;
using eDriven.Core.Reflection;
using eDriven.Gui.Events;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;
using UnityEngine;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Components
{
    #region Event metadata

    /// <summary>
    /// Skinnable component
    /// </summary>
    
    [Event(Name = SkinPartEvent.PART_ADDED, Type = typeof(SkinPartEvent))]
    [Event(Name = SkinPartEvent.PART_REMOVED, Type = typeof(SkinPartEvent))]

    #endregion

    #region Style metadata

    [Style(Name = "skinClass", Type = typeof(Type), ProxyMemberName = "SkinClass")]

    #endregion

    public partial class SkinnableComponent : Component
    {
        /**
         *  
         *  True if the skin has changed and hasn't gone through validation yet.
         */
        private bool _skinChanged;

        private bool _skinFactoryStyleChanged;

        /**
         *  
         * 
         * Contains a flat list of all the skin parts. This includes
         * inherited skin parts. It is best to use a for...in to loop
         * through the skin parts. The property name will be the name of the 
         * skin part and it's value will be a boolean specifying if it is required
         * or not.
         */
        /// <summary>
        /// Skin parts
        /// </summary>
        protected virtual Dictionary<string, bool> SkinParts
        {
            get
            {
                return SkinPartCache.Instance.Get(_thisType);
            }
        }

        /**
         *  
         * Storage for skin instance
         */
        private Component _skin;
        private bool _isMapperSkin;

        /// <summary>
        /// Instance of the skin class
        /// </summary>
        public Component Skin
        {
            get
            {
                return _skin;
            }
        }

        private void SetSkin(Component value)
        {
            if (value == _skin)
                return;

            //Debug.Log("Set skin: " + value);
            _skin = value;
            DispatchEvent(new Event("skinChanged"));
        }

// ReSharper disable MemberCanBePrivate.Global
        protected virtual void AttachSkin()
// ReSharper restore MemberCanBePrivate.Global
        {
            /*if (Id == "TooltipManagerStage")
                Debug.Log("AttachSkin: " + this + "; skin: " + GetStyle("skinClass"));*/

            // Factory
            IFactory skinClassFactory = GetStyle("skinFactory"/*, false*/) as IFactory; // do not check default style
            
            //Debug.Log(1);

            if (null != skinClassFactory)
                SetSkin(skinClassFactory.NewInstance() as Component);
            
            // Class
            if (null == Skin)
            {
                //Debug.Log(NonInheritingStyles.ToString());
                //Debug.Log(this + " -> skinClass: " + GetStyle("skinClass"));

                var skinClass = EvaluateSkinClassFromStyle();
                //Debug.Log(string.Format("******************* {0}: skinClass: {1}", this, skinClass));

                // TODO: the problem is in the folowing:
                // we don't have a skin reference here because our framework uses the invalidation methods for attaching the skin
                // we should change the framework to process the skin the moment the child is being added
                // no metter that setting the skin will mess with children and sub-children
                // it is simply so and we couldn't get away from it!

                // NOTE: This is why we have the problem of skinnable components not measuring themselves properly
                // the problem is that by the measurement time the skin is not yet being attached, so we cannot measure it
                // the skin is currently being measured in another pass, after the container (HBox) has already been measured!

                if (null != skinClass)
                    SetSkin((Component)Activator.CreateInstance(skinClass));
            }
            
            if (null != Skin)
            {
                AddSkinAsChild(Skin);
                Skin.AddEventListener(PropertyChangeEvent.PROPERTY_CHANGE, SkinPropertyChangeHandler);

                // TEMP: because of styling
                // currently the component ID doesn't propagate to Skin, and we need this for styling!
                Skin.Id = Id;
            }
            else
            {
                throw new Exception(this + " -> Skin not found: ");
            }

            FindSkinParts();
            
            InvalidateSkinState();
        }

        /// <summary>
        /// Used for deffered parts
        /// </summary>
        /// <param name="e"></param>
        private void SkinPropertyChangeHandler(Event e)
        {
            if (null != SkinParts)
            {
                PropertyChangeEvent pce = (PropertyChangeEvent) e;
                string skinPartID = pce.Property;
                if (SkinParts.ContainsKey(skinPartID))
                {
                    var part = CoreReflector.GetValue(Skin, skinPartID);

                    if (pce.NewValue == null)
                    {
                        if (!(part is IFactory))
                            PartRemoved(skinPartID, part);
                        CoreReflector.SetValue(this, skinPartID, pce.NewValue);
                    }
                    else
                    {
                        part = pce.NewValue;
                        if (!(part is IFactory))
                            PartAdded(skinPartID, part);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skin"></param>
        public void AttachAdapterSkin(Component skin)
        {
            //Debug.Log("AttachAdapterSkin: " + this);
            SetSkin(skin);
            _isMapperSkin = true;
            AddSkinAsChild(skin);
            //Debug.Log(this + " -> Adapter skin attached: " + skin);
            FindSkinParts();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skin"></param>
        private void AddSkinAsChild(Component skin)
        {
            //Debug.Log("AddSkinAsChild: " + skin);
            skin.Owner = this;

            skin.StyleName = StyleName;

            base.AddChild(skin);
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void DetachSkin()
        {       
            Skin.RemoveEventListener(PropertyChangeEvent.PROPERTY_CHANGE, SkinPropertyChangeHandler);
            
            ClearSkinParts();
            base.RemoveChild(Skin);
            SetSkin(null);
        }

        private Type _thisType;

        private void FindSkinParts()
        {
            if (null == _thisType)
                _thisType = GetType();

            var parts = SkinParts.Keys;

            foreach (var id in parts)
            {
                object part = null;

                /**
                 * 1. If this is a mapper skin, 
                 * */
                if (_isMapperSkin)
                {
                    part = Skin.GetChildComponent(id);
                    //if (null != part)
                    //    Debug.Log("Found mapper skin part: " + part);
                    //else
                    //    Debug.Log(Skin + " -> Couldn't find mapper skin part: " + id);
                }

                else
                {
                    if (CoreReflector.HasMember(Skin, id))
                    {
                        try
                        {
                            part = CoreReflector.GetValue(Skin, id);
                        }
                        catch (InvalidCastException/* ex*/)
                        {
                            Debug.LogError(string.Format("Cannot cast the skin part to InteractiveComponent. Skin: {0}; Part: {1}", Skin, id));
                        }
                    }
                        
                    //else
                    //    Debug.LogWarning("Couldn't find member: " + id);
                }

                if (SkinParts[id]) // == true (required part)
                {
                    if (null == part)
                        throw new Exception("Required skin part not found: " + id);
                }

                if (null != part)
                {
                    /**
                     * Note: we've been having a hard-core bug here (20131216)!
                     * The system FREEZED when using Panel with children in designer
                     * For instance, a panel had a single button child as a content child (not tool or control bar child)
                     * This is also the source of bug whereever we add a child prior to adding itself to the display list (both designer and code)
                     * I think it might be related to styles and StyleProtoChain process (?)
                     * This shoould - of course - be fixed
                     * The problem with designer was in ComponentAdapter ("Produce" method):
                     * _component.AddEventListener(FrameworkEvent.PREINITIALIZE, InitializeHandler)
                     * During the PREINITILIZE, child components have not yet been created - so the skin wasn't created
                     * When I changed it to INITILIZE, it started to work properly:
                     * _component.AddEventListener(FrameworkEvent.INITIALIZE, InitializeHandler)                     * 
                     * */

                    /*if (id == "ContentGroup")
                        Debug.LogWarning("ContentGroup: " + part);*/

                    //CoreReflector.SetValue(this, id, part);
                    MemberWrapper wrapper = new MemberWrapper(GetType(), id);
                    wrapper.SetValue(this, part);

                    // If the assigned part has already been instantiated, call partAdded() here,
                    // but only for static parts.

                    try
                    {
                        /* Note: this fails, because the wrapper wraps around the Skin's property, not the Panel's */
                        //var p = CoreReflector.GetValue(this, id);
                        wrapper = new MemberWrapper(GetType(), id);
                        var p = wrapper.GetValue(this);

                        // TODO: we should get the value silently here, because not to disturb the DoubleGroup.Modified flag
                        //Debug.Log("Just added: " + p);

                        // If the assigned part has already been instantiated, call partAdded() here,
                        // but only for static parts.
                        if (null != p && !(p is IFactory))
                            PartAdded(id, p);
                    }
                    catch (ArgumentException ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        private void ClearSkinParts()
        {
            if (null == _thisType) // not yet added
                return;

            var partDict = SkinPartCache.Instance.Get(_thisType);

            if (null != partDict)
            {
                foreach (var id in partDict.Keys)
                {
                    var p = CoreReflector.GetValue(this, id);

                    if (!(p is IFactory))
                    {
                        PartRemoved(id, p);
                    }
                    else
                    {
                        //var len:int = numDynamicParts(id);
                        //for (var j:int = 0; j < len; j++)
                        //    removeDynamicPartInstance(id, getDynamicPartAt(id, j));
                    }
                  
                    CoreReflector.SetValue(this, id, null);
                }
            }
        }

// ReSharper disable MemberCanBeMadeStatic.Global
// ReSharper disable MemberCanBePrivate.Global

        /// <summary>
        /// Executes after the part is added
        /// </summary>
        /// <param name="partName"></param>
        /// <param name="instance"></param>
        protected virtual void PartAdded(string partName, object instance)
// ReSharper restore MemberCanBePrivate.Global
// ReSharper restore MemberCanBeMadeStatic.Global
        {
            //Debug.Log(string.Format("PartAdded: {0} [{1}]", partName, instance));
            SkinPartEvent e = new SkinPartEvent(SkinPartEvent.PART_ADDED) {partName = partName, Instance = instance};
            DispatchEvent(e);
        }

        // ReSharper disable MemberCanBeMadeStatic.Global
// ReSharper disable MemberCanBePrivate.Global
        /// <summary>
        /// Executes after the part is removed
        /// </summary>
        /// <param name="partName"></param>
        /// <param name="instance"></param>
        protected virtual void PartRemoved(string partName, object instance)
// ReSharper restore MemberCanBePrivate.Global
// ReSharper restore MemberCanBeMadeStatic.Global
        {
            //Debug.Log(string.Format("PartRemoved: {0} [{1}]", partName, instance));
            SkinPartEvent e = new SkinPartEvent(SkinPartEvent.PART_REMOVED) {partName = partName, Instance = instance};
            DispatchEvent(e);
        }

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_skinChanged)
            {
                _skinChanged = false;
                //Debug.Log("Skin change detected: " + this);
                ValidateSkinChange();
            }

            if (_skinStateIsDirty)
            {
                //Debug.Log("Skin state is dirty");
                string pendingState = GetCurrentSkinState();
                StateChanged(Skin.CurrentState, pendingState, false);
                Skin.CurrentState = pendingState;
                _skinStateIsDirty = false;
            }
        }

        private void StateChanged(string oldState, string newState, bool recursive)
        {
            // This test only checks for pseudo conditions on the subject of the selector.
            // Pseudo conditions on ancestor selectors are not detected - eg:
            //    List ScrollBar:inactive #track
            // The track styles will not change when the scrollbar is in the inactive state.
            //if (currentCSSState && oldState != newState &&
            //       (styleManager.hasPseudoCondition(oldState) ||
            //        styleManager.hasPseudoCondition(newState)))
            //{
            //    regenerateStyleCache(recursive);
            //    initThemeColor();
            //    styleChanged(null);
            //    notifyStyleChangeInChildren(null, recursive);
            //}
        }

        public override void StyleChanged(string styleName)
        {
            //Debug.Log(string.Format("StyleChanged: {0} => {1}", styleName, s));

            if (styleName == "skinClass" || styleName == "skinFactory")
            {
                _skinChanged = true;
                InvalidateProperties();

                //if (styleName == "skinClass")
                //    Debug.Log(this + "; skinClass changed to: " + s);

                if (styleName == "skinFactory")
                    _skinFactoryStyleChanged = true;
            }

            base.StyleChanged(styleName);
        }

        private void ValidateSkinChange()
        {
            // If our new skin Class happens to match our existing skin Class there is no
            // reason to fully unload then reload our skin.  
            bool skipReload = false;
            
            if (null != _skin)
            {
                //var factory:Object = getStyle("skinFactory");
                object newSkinClass;
                
                //// if it's a factory, only reload the skin if the skinFactory
                //// style has been explicitly changed.  right now this style is only 
                //// used by design view, and this is the contract we have with them.
                //if (factory)
                //    skipReload = !skinFactoryStyleChanged;
                //else
                //{
                    newSkinClass = GetStyle("skinClass");
                    
                    skipReload = null != newSkinClass &&
                        newSkinClass.GetType() == _skin.GetType();
                //}
                
                //skinFactoryStyleChanged = false;
            }


            if (!skipReload)
            {
                if (null != _skin) // TEMP
                    DetachSkin();

                AttachSkin();
            }
        }

        protected override void Measure()
        {
            base.Measure();

            if (null != Skin)
            {
                MeasuredWidth = Skin.GetExplicitOrMeasuredWidth();
                MeasuredHeight = Skin.GetExplicitOrMeasuredHeight();
                MeasuredMinWidth = Skin.ExplicitWidth ?? Skin.MinWidth;
                MeasuredMinHeight = Skin.ExplicitHeight ?? Skin.MinHeight;
            }
        }

        protected override void UpdateDisplayList(float width, float height)
        {
            if (null != Skin)
                Skin.SetActualSize(width, height);
        }

        private bool _skinStateIsDirty;

        /**
         *  Marks the component so that the new state of the skin is set
         *  during a later screen update.
         */
        public void InvalidateSkinState()
        {
            //Debug.Log("InvalidateSkinState");

            if (_skinStateIsDirty)
                return; // State is already invalidated

            //Debug.Log("   -> done");

            _skinStateIsDirty = true;
            InvalidateProperties();
        }

        /// <summary>
        /// Gets the current skin state
        /// </summary>
        /// <returns></returns>
        public virtual string GetCurrentSkinState()
        {
            return null;
        }

        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;
                InvalidateSkinState();
            }
        }

        protected override void CreateChildren()
        {
            base.CreateChildren();

            ValidateSkinChange();
        }

        /*public override void DrawFocus(bool isFocused)
        {
            // TODO
            //if (isFocused)
            //{
            //    // For some composite components, the focused object may not
            //    // be "this". If so, we don't want to draw the focus.
            //    if (!drawFocusAnyway && focusManager.getFocus() != this)
            //        return;
                    
            //    if (!focusObj)
            //    {
            //        var focusSkinClass:Class = getStyle("focusSkin");
                    
            //        if (focusSkinClass)
            //            focusObj = new focusSkinClass();
                    
            //        if (focusObj)
            //            super.addChildAt(focusObj, 0);
            //    }
            //    if (focusObj && "target" in focusObj)
            //        focusObj["target"] = this;
            //}
            //else
            //{
            //    if (focusObj)
            //        super.removeChild(focusObj);
            //    focusObj = null;
            //}
            base.DrawFocus(isFocused);
        }*/
    }
}