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
using eDriven.Gui.Reflection;
using UnityEngine;
using Component = eDriven.Gui.Components.Component;

#pragma warning disable 1591

namespace eDriven.Gui.Designer.Adapters
{
    [Serializable]
    public abstract class ComponentAdapter : ComponentAdapterBase, IComponentFactory
    {
        #region Static

        /// <summary>
        /// Registry relating display objects to game objects
        /// Used when clicking display objects on screen and wanting to select game object in the hierarchy 
        /// </summary>
        public static readonly Dictionary<Component, GameObject> Registry = new Dictionary<Component, GameObject>();

        #endregion

        #region Properties

        /// <summary>
        /// The event map
        /// </summary>
        [Saveable]
        [SerializeField]
        public EventMappingDescriptor EventMap = new EventMappingDescriptor();

        private bool _instantiated;

        [Saveable(false)]
        public bool Instantiated
        {
            get { return _instantiated; }
        }

        #endregion

        /// <summary>
        /// Reset to default values.<br/>
        /// Reset is called when the user hits the Reset button in the Inspector's context menu or when adding the component the first time. This function is only called in editor mode. Reset is most commonly used to give good default values in the inspector. 
        /// </summary>
        [Obfuscation(Exclude = true)]
        public void Reset ()
        {
            // Reset to default values.
            // Reset is called when the user hits the Reset button in the Inspector's context menu or when adding the component the first time. This function is only called in editor mode. Reset is most commonly used to give good default values in the inspector. 
        }

        /// <summary>
        /// Awake
        /// </summary>
        [Obfuscation(Exclude = true)]
        public void Awake()
        {
            // hide 3-D transform on all descriptors but Stage
            // (will be used when Unity implements rendering GUI to 3-D)
            //transform.hideFlags = (this is StageAdapter) ? HideFlags.DontSave : (HideFlags.HideInInspector | HideFlags.NotEditable);
            OnAwake();
        }

        /// <summary>
        /// Handler that fires on awake
        /// </summary>
        protected virtual void OnAwake()
        {
            //transform.hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;
        }

// ReSharper disable UnusedMember.Local
        [Obfuscation(Exclude = true)]
        void OnEnable()
// ReSharper restore UnusedMember.Local
        {
            CheckLevelLoadedSignalConnection();

            // hide transform in inspector
            transform.hideFlags = HideFlags.HideInInspector | HideFlags.NotEditable;

            if (null != Component) {
                Component.Visible = Component.IncludeInLayout = true;
            }

            OnEnableImpl();
        }

        /// <summary>
        /// Whenever using ComponentDescriptors, there's the question of GUIs
        /// So, when enabling the first one, we are making this check to see if we are connected to the level change signal
        /// On each level (scene) change, we should remove all the GUIs from GuidUtil list
        /// That's because if we not do that, the stale GUIDs in the list would relate to instantiating the unexisting children
        /// </summary>
        private static void CheckLevelLoadedSignalConnection()
        {
            if (!LevelLoadedStrategy.Started)
                LevelLoadedStrategy.Start();
        }

        protected virtual void OnEnableImpl()
        {
            EventMap.ComponentAdapter = this;
        }

        [Obfuscation(Exclude = true)]
// ReSharper disable UnusedMember.Local
        void OnDisable()
// ReSharper restore UnusedMember.Local
        {
            if (null != Component) {
                Component.Visible = Component.IncludeInLayout = false;
            }

            // TODO: signalize the editor that the component is removed
            // so it can react immediatelly and remove the selection
            DesignerState.Instance.ComponentRemoved();

            //Registry.Remove(_component);
            Registry.Clear(); // Note: there's a redundancy here: this fires for all the components - once should be enough
        }

        public abstract Type ComponentType { get; }
        public abstract Component NewInstance();

        #region Special properties

        [Saveable]
        public string Id;

        [Saveable]
        [Obfuscation(Exclude = true)]
        public bool FactoryMode;

        /**
     * GUID is used for adding children
     * After the play mode is stopped, we are loosing all our references to objects, and we have to find them using the GUID after they are recreated
     * Then, we must fill the ContentChildren and other collections with new references
     * */
        //[Saveable]
        //public string Guid;
    
        #endregion

        #region Saveable properties

        [Saveable]
        public int Depth;

        [Saveable]
        public string Tooltip;

        [Saveable]
        public string StyleName; // previously StyleMapper

        [Saveable]
        public int X;

        [Saveable]
        public int Y;

        [Saveable]
        public int Width = 20;

        [Saveable]
        public int Height = 20;

        [Saveable]
        public int MinWidth = 20;

        [Saveable]
        public int MinHeight = 20;

        [Saveable]
        public int MaxWidth = 10000;

        [Saveable]
        public int MaxHeight = 10000;

        [Saveable]
        public bool UsePercentWidth;

        [Saveable]
        public bool UsePercentHeight;

        [Saveable]
        public bool UseX;

        [Saveable]
        public bool UseY;

        [Saveable]
        public bool UseWidth;

        [Saveable]
        public bool UseHeight;

        [Saveable]
        public bool UseLeft;

        [Saveable]
        public bool UseRight;

        [Saveable]
        public bool UseTop;

        [Saveable]
        public bool UseBottom;

        [Saveable]
        public int Left;

        [Saveable]
        public int Right;

        [Saveable]
        public int Top;

        [Saveable]
        public int Bottom;

        [Saveable]
        public bool UseHorizontalCenter;

        [Saveable] 
        public int HorizontalCenter;

        [Saveable]
        public bool UseVerticalCenter;

        [Saveable]
        public int VerticalCenter;
    
        /*[Saveable]
    public bool SyncMargins = true;

    [Saveable]
    public int MarginLeft;

    [Saveable]
    public int MarginRight;

    [Saveable]
    public int MarginTop;

    [Saveable]
    public int MarginBottom;*/

        [Saveable]
        public bool Visible = true;

        [Saveable]
        public bool IncludeInLayout = true;

        [Saveable]
        public bool Enabled = true;

        [Saveable]
        public bool MouseEnabled = true;

        [Saveable]
        public bool MouseChildren = true;

        [Saveable]
        public bool FocusEnabled = true;

        [Saveable]
        public bool HighlightOnFocus;

        [Saveable]
        public string NavigatorDescriptor;

        [Saveable]
        public bool ShowGuid;

        [Saveable]
        public bool ShowTransform = true;

        /*[Saveable]
    public bool ShowPadding = true;*/

        /*[Saveable]
    public bool ShowMargins = true;*/

        [Saveable]
        public bool ShowStyles = true;

        [Saveable]
        public bool ShowEvents = true;

        [Saveable]
        public bool UseColor;

        [Saveable]
        public Color Color = Color.white;

        [Saveable]
        public Color ContentColor = Color.white;

        [Saveable]
        public Color BackgroundColor = Color.white;

        [Saveable]
        public float Alpha = 1f;

        [Saveable]
        public float Rotation;

        [Saveable]
        public Vector2 RotationPivot = new Vector2(0.5f, 0.5f);

        [Saveable]
        public Vector2 Scale = new Vector2(1, 1);

        #endregion
    
        /// <summary>
        /// Instantiates a single component
        /// </summary>
        /// <param name="register">Register to registry so it is available by clicking</param>
        /// <returns></returns>
        public Component DoInstantiate(bool register) // TODO: Set this internal and expose the whole class through adapter
        {
            //Debug.Log("DoInstantiate: " + this);
            //if (_instantiated)
            //    return _component;

            //var constructor = ComponentType.GetConstructor(Type.EmptyTypes);
            //_component = (Component)constructor.Invoke(null);
            Component component = NewInstance();
            Apply(component);
            //Debug.Log("Validating now: " + component);
            //component.ValidateNow();

            if (register)
            {
                Component = component;
                Registry.Add(Component, gameObject);
                //Debug.Log(string.Format("Adding component {0} [gameObject: {1}] to registry", Component, gameObject));
                _instantiated = true;
            }

            if (Application.isPlaying) {
                // send messages
                SendMessage("ComponentInstantiated", component, SendMessageOptions.DontRequireReceiver);
                SendMessage("InitializeComponent", component, SendMessageOptions.DontRequireReceiver);
            }

            return component;
        }

        /// <summary>
        /// Applies the values from the adapter to a component
        /// </summary>
        /// <param name="component"></param>
        public override void Apply(Component component)
        {
            // Component ID used by developer for locating the component via ComponentManager<br/>
            // Must be unique in the application
            component.Id = Id;
            component.Depth = Depth;
            component.Tooltip = Tooltip;
            component.Visible = Visible;
            component.IncludeInLayout = IncludeInLayout;
            component.Enabled = Enabled;
            component.MouseEnabled = MouseEnabled;
            component.MouseChildren = MouseChildren;
            component.FocusEnabled = FocusEnabled;
            component.HighlightOnFocus = HighlightOnFocus;
            component.NavigatorDescriptor = NavigatorDescriptor;
        
            #region Position and sizing

            component.X = UseX ? X : 0;
            component.Y = UseY ? Y : 0;

            if (UseWidth)
            {
                if (UsePercentWidth)
                    component.PercentWidth = Width;
                else
                    component.Width = Width;
            }
            else
            {
                /* 2 times - by purpose */
                component.ExplicitWidth = null;
                component.PercentWidth = null;
                component.InvalidateParentSizeAndDisplayList();
            }

            if (UseHeight)
            {
                if (UsePercentHeight)
                    component.PercentHeight = Height;
                else
                    component.Height = Height;
            }
            else
            {
                /* 2 times - by purpose */
                component.ExplicitHeight = null;
                component.PercentHeight = null;
                component.InvalidateParentSizeAndDisplayList();
            }

            component.MinWidth = MinWidth;
            component.MinHeight = MinHeight;
            component.MaxWidth = MaxWidth;
            component.MaxHeight = MaxHeight;

            #endregion

            #region StyleName

/* Note: with adapters, we could possibly apply only strings as style names! */
            /* NOTE:; Here was a BUG!!! Style have never applied to the actual component because component.StyleName is null and not "component.StyleName is string"!! */
            //if ((component.StyleName is string) && StyleName != (string) component.StyleName)
            if (/*(component.StyleName is string) && */StyleName != (string)component.StyleName)
                // /*!string.IsNullOrEmpty(StyleName) &&*/ commented fixes combobox
            {
                component.StyleName = !string.IsNullOrEmpty(StyleName) ? StyleName : null;
            }

            #endregion

            #region Constrains

            if (UseLeft)
            {
                component.Left = Left;
            }
            else
            {
                component.Left = null;
            }
            if (UseRight)
            {
                component.Right = Right;
            }
            else
            {
                component.Right = null;
            }
            if (UseTop)
            {
                component.Top = Top;
            }
            else
            {
                component.Top = null;
            }
            if (UseBottom)
            {
                component.Bottom = Bottom;
            }
            else
            {
                component.Bottom = null;
            }
            if (UseHorizontalCenter)
            {
                component.HorizontalCenter = HorizontalCenter;
            }
            else
            {
                component.HorizontalCenter = null;
            }
            if (UseVerticalCenter)
            {
                component.VerticalCenter = VerticalCenter;
            }
            else
            {
                component.VerticalCenter = null;
            }

            #endregion

            #region Color & alpha

            if (UseColor)
            {
                component.Color = Color;
                component.ContentColor = ContentColor;
                component.BackgroundColor = BackgroundColor;
            }
            else
            {
                component.Color = Color.white;
                component.ContentColor = Color.white;
                component.BackgroundColor = Color.white;
            }
            component.Alpha = Alpha;

            #endregion
        
            #region Rotation & scale

            component.Rotation = Rotation;
            component.RotationPivot = RotationPivot;

            component.Scale = Scale;

            //component.MarginLeft = MarginLeft;
            //component.MarginRight = MarginRight;
            //component.MarginTop = MarginTop;
            //component.MarginBottom = MarginBottom;
            /*component.SetStyle("marginLeft", MarginLeft);
        component.SetStyle("marginRight", MarginRight);
        component.SetStyle("marginTop", MarginTop);
        component.SetStyle("marginBottom", MarginBottom);*/

            #endregion
        }

        #region Child instantiation

        /// <summary>
        /// To avoid instantiation of this list for each container, we keep it in the single place
        /// </summary>
        //private static List<ComponentAdapter> _childrenToSkip = new List<ComponentAdapter>();

        private bool _assignToDescriptor;

        private Component _component;

        /// <summary>
        /// Recursively instantiates components specified by descriptors in the hierarchy
        /// </summary>
        /// <param name="instantiate">Instantiate component</param>
        /// <param name="assignToDescriptor">Register to transform, and put into registry so it is available by clicking</param>
        public Component Produce(bool instantiate, bool assignToDescriptor)
        {
            _assignToDescriptor = assignToDescriptor;

            //Debug.Log("Producing " + descriptor);
            //Component component = null;

            if (instantiate)
            {
                _component = DoInstantiate(assignToDescriptor);
                InitEvents(_component);
            }

            //Debug.Log("Produce: " + this);

            /**
             * IMPORTANT (20130813):
             * We have to follow the instantiation cycle!
             * Cannot create children until the PREINITIALIZE is fired on the parent!!!
             * Until this was not done on PREINITIALIZE, but directly, there were problems with creating children (ScrollBar children were rendered in the top-left corner of the Stage)
             * */
            if (null != _component)
            {
                //_component.AddEventListener(FrameworkEvent.PREINITIALIZE, PreinitializeHandler);
                _component.AddEventListener(FrameworkEvent.INITIALIZE, InitializeHandler); // 20131216! (more in e-mail)
            }

            //ContainerAdapter containerAdapter = this as ContainerAdapter;
            //if (null != containerAdapter) // && !containerAdapter.FactoryMode)
            //{
            //    containerAdapter.InstantiateChildren(assignToDescriptor);
            //}

            //Debug.Log("    --- Produced");

            return _component;
        }

        /*public virtual GroupAdapter GetDefaultGroupAdapter()
        {
            return this as GroupAdapter; // for a group
        }*/

        private void InitializeHandler(Core.Events.Event e)
        {
            //_component.RemoveEventListener(FrameworkEvent.PREINITIALIZE, PreinitializeHandler);
            _component.RemoveEventListener(FrameworkEvent.INITIALIZE, InitializeHandler);

            GroupAdapter groupAdapter = this as GroupAdapter;
            //GroupAdapter groupAdapter = GetDefaultGroupAdapter();
            if (null != groupAdapter) // && !containerAdapter.FactoryMode)
            {
                //_childrenToSkip.Clear(); // important!
                //List<ComponentAdapter> childrenToSkip = new List<ComponentAdapter>();
                groupAdapter.InstantiateChildren(_assignToDescriptor);
            }
            else
            {
                //Debug.Log("_component: " + _component);

                // TODO: This is for some future implementation (designer skins etc..)
                // PREINITIALIZE was needed *because of this *
                // but INITIALIZE is the right event, because coded skins are created after PREINITIALIZE!!!
                /*SkinnableComponent skinnableComponent = _component as SkinnableComponent;
                if (null != skinnableComponent)
                {
                    //Debug.Log("Processing skinnable component", this);

                    // this is a skinnable component
                    // look for a skin

                    var adapter = GuiLookup.FindAdapter(gameObject, "skin");
                    if (null == adapter || !gameObject.activeInHierarchy || !adapter.enabled)
                        return;

                    //Debug.Log("    adapter skin: " + adapter, adapter);

                    var skin = adapter.Produce(true, true);
                    if (null != skin)
                    {
                        //Debug.Log("        About to attach adapter skin: " + skin, adapter);
                        try {
                            skinnableComponent.AttachAdapterSkin(skin);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex, adapter);
                        }
                    }
                }*/
            }
        }

        /// <summary>
        /// Initializes event handlers if mappings on component enabled
        /// </summary>
        /// <param name="component">The component dispatching events</param>
        internal void InitEvents(Component component)
        {
            if (!EventMap.Enabled)
                return;

            // map descriptor is enabled, so process event handlers!
            EventMappingDescriptor.ProcessListeners(this, component, true);
        }

        public GroupAdapter ParentAdapter
        {
            get
            {
                if (null == transform)
                    return null;
                if (null == transform.parent)
                    return null;
                return transform.parent.GetComponent<GroupAdapter>();
            }
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            // ToString will return the name of the game object
            return gameObject.name;
        }

        #endregion
    }
}

#pragma warning restore 1591