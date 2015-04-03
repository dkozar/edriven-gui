using System.Collections.Generic;
using eDriven.Gui.Components;
using eDriven.Gui.GUIStyles;
using eDriven.Gui.Layout;
using eDriven.Gui.Styles;
using UnityEngine;

namespace eDriven.Gui.Form
{
    //[StyleProxy(typeof(FormItemStyleProxy))]

    [Style(Name = "labelStyle", Type = typeof(GUIStyle), ProxyType = typeof(FormItemLabelStyle))]

    /// <summary>
        /// Base class for all form items
        /// </summary>
    public class FormItem : HGroup
    {
        public static float DefaultPaddingLeft = 5;
        public static float DefaultPaddingRight = 5;
        public static float DefaultPaddingTop = 5;
        public static float DefaultPaddingBottom = 7; // plus 2 because of the line (graphics)

        private bool _labelChanged;
        override public string NavigatorDescriptor
        {
            get { return base.NavigatorDescriptor; }
            set
            {
                base.NavigatorDescriptor = value;
                _labelChanged = true;
                InvalidateProperties();
            }
        }

        /// <summary>
        /// Sets the width of the label same as all other labels in the form
        /// </summary>
        public bool AlignLabelToForm = true;

        private Label _lbl;

        private readonly Group _contentGroup;
        public Group ContentGroup
        {
            get { return _contentGroup; }
        }

        #region Implementation of IContentChildList

        /// <summary>
        /// The child components of the container
        /// </summary>
        public override List<DisplayListMember> ContentChildren
        {
            get { return _contentGroup.Children; }
        }

        /// <summary>
        /// Number of elements
        /// </summary>
        public override int NumberOfContentChildren
        {
            get { return _contentGroup.NumberOfChildren; }
        }

        /// <summary>
        /// Checks if this is a Owner of a component
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public override bool HasContentChild(DisplayListMember child)
        {
            return _contentGroup.HasChild(child);
        }

        public override bool ContentContains(DisplayListMember child)
        {
            return _contentGroup.Contains(child);
        }

        public override bool ContentContains(DisplayListMember child, bool includeThisCheck)
        {
            return _contentGroup.Contains(child, includeThisCheck);
        }

        /// <summary>
        /// Adds a child to the container
        /// </summary>
        /// <param name="child">A child</param>
        public override DisplayListMember AddContentChild(DisplayListMember child)
        {
            return _contentGroup.AddChild(child);
        }

        /// <summary>
        /// Adds a child to the container to the specified index
        /// </summary>
        /// <param name="child">A child</param>
        /// <param name="index">Child index</param>
        public override DisplayListMember AddContentChildAt(DisplayListMember child, int index)
        {
            return _contentGroup.AddChildAt(child, index);
        }

        /// <summary>
        /// Removes a child from the container
        /// </summary>
        /// <param name="child">A child</param>
        public override DisplayListMember RemoveContentChild(DisplayListMember child)
        {
            return _contentGroup.RemoveChild(child);
        }

        /// <summary>
        /// Adds a child from the container at specified index
        /// </summary>
        public override DisplayListMember RemoveContentChildAt(int index)
        {
            return _contentGroup.RemoveChildAt(index);
        }

        /// <summary>
        /// Removes all children from the container
        /// </summary>
        public override void RemoveAllContentChildren()
        {
            _contentGroup.RemoveAllChildren();
        }

        ///<summary>
        /// Swaps two children
        ///</summary>
        ///<param name="firstElement">First child</param>
        ///<param name="secondElement">Second child</param>
        public override void SwapContentChildren(DisplayListMember firstElement, DisplayListMember secondElement)
        {
            _contentGroup.SwapChildren(firstElement, secondElement);
        }

        /// <summary>
        /// Gets child at specified position
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Child index</returns>
        public override DisplayListMember GetContentChildAt(int index)
        {
            return _contentGroup.GetChildAt(index);
        }

        /// <summary>
        /// Gets child index
        /// </summary>
        /// <param name="child">A child</param>
        /// <returns>The position</returns>
        public override int GetContentChildIndex(DisplayListMember child)
        {
            return _contentGroup.GetContentChildIndex(child);
        }

        #endregion

        private bool _labelWidthChanged;
        private float _labelWidth = -1;
        public float LabelWidth
        {
            get { return _labelWidth; }
            set
            {
                _labelWidth = value;
                _labelWidthChanged = true;
                InvalidateProperties();
            }
        }

        private bool _labelPercentWidthChanged;
        private float? _labelPercentWidth;
        public float? LabelPercentWidth
        {
            get
            {
                return _labelPercentWidth;
            }
            set
            {
                _labelPercentWidth = value;
                _labelPercentWidthChanged = true;
                InvalidateProperties();
            }
        }
        
        ////protected Label Lbl;

        //// StyleName;
        //private bool _labelStyleNameChanged;
        //private string _labelStyleName;
        ///// <summary>
        ///// Style name
        ///// For core components only! (which render Unity GUI elements (Button, Label etc.))
        ///// Custom made components don't use this property
        ///// </summary>
        //public string LabelStyleName
        //{
        //    get { return _labelStyleName; }
        //    set
        //    {
        //        _labelStyleName = value;
        //        _labelStyleNameChanged = true;
        //        InvalidateProperties();
        //    }
        //}

        //private bool _percentHeightChanged;
        //public override float? PercentHeight
        //{
        //    get
        //    {
        //        return base.PercentHeight;
        //    }
        //    set
        //    {
        //        base.PercentHeight = value;
        //        _percentHeightChanged = true;
        //        InvalidateProperties();
        //    }
        //}

        //// StyleName;
        //private bool _controlStyleNameChanged;
        //private string _controlStyleName;
        ///// <summary>
        ///// Style name
        ///// For core components only! (which render Unity GUI elements (Button, Label etc.))
        ///// Custom made components don't use this property
        ///// </summary>
        //public virtual string ControlStyleName
        //{
        //    get { return _controlStyleName; }
        //    set
        //    {
        //        _controlStyleName = value;
        //        _controlStyleNameChanged = true;
        //        InvalidateProperties();
        //    }
        //}

        private bool _highlightOnFocusChanged;
        private bool _highlightOnFocus = true;
        /// <summary>
        /// Totally override default behaviour and highlight children only
        /// </summary>
        public override bool HighlightOnFocus
        {
            get
            {
                //return base.HighlightOnFocus;
                return _highlightOnFocus;
            }
            set
            {
                //if (value != base.HighlightOnFocus)
                if (value != _highlightOnFocus)
                {
                    //base.HighlightOnFocus = value;
                    _highlightOnFocus = value;
                    _highlightOnFocusChanged = true;
                    InvalidateProperties();
                }
            }
        }

        private readonly List<Components.Component> _controls = new List<Components.Component>();
        public List<Components.Component> Controls
        {
            get
            {
                return _controls;
            }
        }

        private Form _form;
        internal Form Form
        {
            get { return _form; }
            set { _form = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public FormItem()
        {
            //SetStyle("labelStyle", FormStyleProxy.Instance.Default.ItemLabelStyle);

            //QLayout = new HorizontalLayout {Gap = 10};

            //HorizontalAlign = HorizontalAlign.Left;
            //VerticalAlign = VerticalAlign.Top;

            FocusEnabled = false;
            PercentWidth = 100;
            MinHeight = 25;
            
            //Padding = 5;
            SetStyle("paddingLeft", DefaultPaddingLeft);
            SetStyle("paddingRight", DefaultPaddingRight);
            SetStyle("paddingTop", DefaultPaddingTop);
            SetStyle("paddingBottom", DefaultPaddingBottom);

            //HorizontalSpacing = 10;

            // invalidation
            _displayLabelChanged = true;

            // instantiate content group to be present if adding children from outside
            // (internal CreateChildren not run yet)
            _contentGroup = new Group();

            _contentGroup.Layout = new VerticalLayout
                                       {
                                           Gap = 10,
                                           HorizontalAlign = HorizontalAlign.Left,
                                           VerticalAlign = VerticalAlign.Top
                                       };
        }

        //private bool _focusEnabledChanged;
        //private bool _focusEnabled;
        //public override bool FocusEnabled
        //{
        //    get
        //    {
        //        return _focusEnabled;
        //    }
        //    set
        //    {
        //        _focusEnabled = value;
        //        _focusEnabledChanged = true;
        //        InvalidateProperties();
        //    }
        //}

        //private bool _controlAdded;
        private bool _displayLabelChanged;
        private bool _displayLabel = true;
        public bool DisplayLabel
        {
            get
            {
                return _displayLabel;
            }
            set
            {
                _displayLabel = value;
                _displayLabelChanged = true;
                InvalidateProperties();
            }
        }

        //private Component _lastAddedControl;

        //internal override void StyleChanged(string styleName/*, object style*/)
        //{
        //    Debug.Log(string.Format("[{0}] -> StyleChanged [{1}]", this, styleName));

        //    base.StyleChanged(styleName/*, style*/);

        //    switch (styleName)
        //    {
        //        case "labelStyle":
        //            Children.ForEach(delegate(DisplayListMember child)
        //            {
        //                Label l = child as Label;
        //                if (null != l)
        //                    l.SetStyle("labelStyle", GetStyle("labelStyle"));
        //            });
        //            break;
        //    }
        //}

        protected override void CreateChildren()
        {
            base.CreateChildren();

            Debug.Log("CreateChildren.labelStyle: " + GetStyle("labelStyle"));

            _lbl = new Label();
            //_lbl.SetStyle("labelStyle", GetStyle("labelStyle"));
            _lbl.Text = base.NavigatorDescriptor;
            _lbl.FocusEnabled = false;
            AddChild(_lbl);

            //_contentGroup = new Container();
            
            _contentGroup.PercentWidth = 100;
            _contentGroup.PercentHeight = 100;
            AddChild(_contentGroup);
        }

        //internal override void StyleChanged(string styleName)
        //{
        //    base.StyleChanged(styleName);

        //    //Debug.Log("StyleChanged.labelStyle: " + GetStyle("labelStyle"));

        //    //switch (styleName)
        //    //{
        //    //    case "labelStyle":
        //    //        _lbl.SetStyle("labelStyle", GetStyle("labelStyle"));
        //    //        break;
        //    //}
        //}

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_displayLabelChanged)
            {
                _displayLabelChanged = false;
                _lbl.Visible = _displayLabel;
            }

            if (_labelChanged || _labelWidthChanged || _labelPercentWidthChanged)
            {
                if (null != _lbl)
                {
                    if (_labelChanged)
                        _lbl.Text = base.NavigatorDescriptor;

                    if (_labelWidthChanged)
                        _lbl.Width = _labelWidth;

                    if (_labelPercentWidthChanged)
                        _lbl.PercentWidth = _labelPercentWidth;

                    //if (_labelStyleNameChanged)
                    //    _lbl.SetStyle("labelStyle", GetStyle("labelStyle")); //_labelStyleName;
                }

                _labelChanged = false;
                _labelWidthChanged = false;
                _labelPercentWidthChanged = false;
            }

            //if (_controlStyleNameChanged)
            //{
            //    _controlStyleNameChanged = false;
            //    _controls.ForEach(delegate(Component control)
            //                          {
            //                              control.StyleName = _controlStyleName;
            //                          });
            //}

            //if (_controlAdded)
            //{
            //    _controlAdded = false;

            //    /**
            //     * RegisterTween controls to tab manager
            //     * */
            //    _controls.ForEach(delegate(Component control)
            //                          {
            //                              if (control.FocusEnabled)
            //                              {
            //                                  //if (null == TabManagerX)
            //                                  //    Debug.Log("TabManagerX is null");
            //                                  //else
            //                                  //{
            //                                  if (null != _tabManagerX)
            //                                      AddControlToTabManager(control);
            //                                  //}
            //                              }
            //                          });
            //}

            if (_highlightOnFocusChanged)
            {
                _highlightOnFocusChanged = false;
                _controls.ForEach(delegate(Components.Component control) { control.HighlightOnFocus = HighlightOnFocus; });
            }

            //if (_percentHeightChanged)
            //{
            //    _controls.ForEach(delegate(Component control) { control.PercentHeight = PercentHeight; });
            //}

            //if (_focusEnabledChanged)
            //{
            //    _focusEnabledChanged = false;
            //    ContentChildren.ForEach(delegate(DisplayListMember element)
            //                         {
            //                             var comp = element as InteractiveComponent;
            //                             if (null != comp)
            //                                comp.FocusEnabled = _focusEnabled;
            //                         });
            //    //if (HasFocus)
            //    //    SetFocus(); // re-init focus
            //}
        }

        public override void SetFocus()
        {
            DisplayListMember firstFocusableItem = Children.Find(delegate(DisplayListMember control) { return ((InteractiveComponent)control).FocusEnabled; });

            //Debug.Log("firstFocusableItem == null: " + (firstFocusableItem == null));

            if (null != firstFocusableItem)
                ((InteractiveComponent)firstFocusableItem).SetFocus();
            else
                base.SetFocus();
        }

        //protected override void KeyUpHandler(Event e)
        //{
        //    base.KeyUpHandler(e);

        //    if (e.Canceled) return;
        //}

        //override public VerticalAlign VerticalAlign
        //{
        //    set
        //    {
        //        Debug.Log("_contentGroup: " + _contentGroup);
        //        var bl = _contentGroup.Layout as BoxLayout;
        //        if (null != bl)
        //            bl.VerticalAlign = value;
        //    }
        //}
    }
}