namespace eDriven.Gui.Styles
{
    internal class CSSMergedStyleDeclaration : StyleDeclaration
    {
        /**
         *  
         * 
         *  Local storage for the _style in the local _style manager.
         */
        private readonly StyleDeclaration _style;
        
        ///**
        // *  
        // * 
        // *  Local storage for the _style in the parent _style manager.
        // */
        //private readonly StyleDeclaration _parentStyle;

        /**
         *  
         * 
         *  If true then update the overrides array from the _style and
         *  _parentStyle.
         */
        private bool _updateOverrides;

        private IStyleValuesFactory _defaultFactory;
        internal override IStyleValuesFactory Set1
        {
            get
            {
                if (_defaultFactory != null)
                        return _defaultFactory;
                    
                    if ((_style != null && _style.Set1 != null)/* || 
                       (_parentStyle != null && _parentStyle.DefaultValues != null)*/)
                    {
                        _defaultFactory = _style.Set1;
                        //_defaultFactory = function():void
                        //{
                        //    if (null != _parentStyle && _parentStyle.DefaultValues != null)
                        //        _parentStyle.DefaultValues.Produce(); // apply(this); // TODO?
                            
                        //    if (null != _style && _style.DefaultValues != null)
                        //        _style.DefaultValues.Produce();
                        //};            
                    }
                    
                    return _defaultFactory;
            }
            set
            {
                // not supported
            }
        }

        private IStyleValuesFactory _factory;
        internal override IStyleValuesFactory Set2
        {
            get
            {
                if (_factory != null)
                    return _factory;

                if ((_style != null && _style.Set2 != null)/* || 
                       (_parentStyle != null && _parentStyle.DefaultValues != null)*/
                                                                                )
                {
                    _factory = _style.Set2;
                    //_defaultFactory = function():void
                    //{
                    //    if (null != _parentStyle && _parentStyle.DefaultValues != null)
                    //        _parentStyle.DefaultValues.Produce(); // apply(this); // TODO?

                    //    if (null != _style && _style.DefaultValues != null)
                    //        _style.DefaultValues.Produce();
                    //};            
                }

                return _factory;
            }
            set
            {
                // not supported
            }
        }

        internal override StyleTable Overrides
        {
            get
            {
                if (!_updateOverrides)
                    return base.Overrides;
                
                StyleTable mergedOverrides = null;
                
                if (null != _style && null != _style.Overrides)
                {
                    mergedOverrides = new StyleTable();
                    
                    StyleTable childOverrides = _style.Overrides;
                    foreach (string obj in childOverrides.Keys)
                        mergedOverrides[obj] = childOverrides[obj];            
                }
                
                //if (null != _parentStyle && null != _parentStyle.Overrides)
                //{
                //    if (null == mergedOverrides)
                //        mergedOverrides = new StyleTable();
                        
                //    StyleTable parentOverrides = _parentStyle.Overrides;
                //    foreach (string obj in parentOverrides.Keys)
                //    {
                //        if (null == mergedOverrides[obj])
                //            mergedOverrides[obj] = parentOverrides[obj];
                //    }
                //}
                
                base.Overrides = mergedOverrides;
                _updateOverrides = false;
                
                return mergedOverrides;
            }
            set
            {
                // not supported
            }
        }

        internal override StyleTable AddStyleToProtoChain(StyleTable chain, object target)
        {
            // If we have a local _style, then add only it to the chain. It will
            // take are of adding its parent to the chain.
            // If then is no _style, but a _parentStyle, then add the parent Style
            // to the chain.
            if (null != _style)
                return _style.AddStyleToProtoChain(chain, target/*, filterMap*/);
            //if (null != _parentStyle)
            //    return _parentStyle.AddStyleToProtoChain(chain, target/*, filterMap*/);
            return chain;
        }

        ///<summary>
        ///</summary>
        ///<param name="style"></param>
        ///<param name="parentStyle"></param>
        ///<param name="selector"></param>
        ///<param name="autoRegisterWithStyleManager"></param>
        public CSSMergedStyleDeclaration(StyleDeclaration style, 
            StyleDeclaration parentStyle, 
            string selector, 
            bool autoRegisterWithStyleManager) : base(selector, autoRegisterWithStyleManager)
        {
            _style = style;
            //_parentStyle = parentStyle;

            //int i;
            //int n;
            
            //if (null != _parentStyle && _parentStyle.effects)
            //{
            //    if (!effects)
            //        effects = [];

            //    effectsArray = _parentStyle.effects;
            //    n = effectsArray.length;
            //    for (i = 0; i < n; i++)
            //    {
            //        effects[i] = effectsArray[i];
            //        if (effects.indexOf(effectsArray[i]) == -1)
            //            effects[i] = effectsArray[i];
            //    }
            //}
            
            _updateOverrides = true;
        }

        public override void SetStyle(string styleProp, object newValue)
        {
            // not supported
        }
    }
}