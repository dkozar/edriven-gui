using System;
using System.Collections;
using System.Collections.Generic;
using eDriven.Gui.Components;
using eDriven.Gui.Events;
using eDriven.Gui.GUIStyles;
using eDriven.Gui.Layout;
using eDriven.Gui.Plugins;
using eDriven.Gui.Styles;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;
using Event = eDriven.Core.Events.Event;
using MulticastDelegate = eDriven.Core.Events.MulticastDelegate;

namespace eDriven.Gui.Form
{
    // paddings
    /// <summary>
    /// Form component
    /// </summary>
    [Style(Name = "paddingLeft", Type = typeof(int), Default = 16)]
    [Style(Name = "paddingRight", Type = typeof(int), Default = 16)]
    [Style(Name = "paddingTop", Type = typeof(int), Default = 16)]
    [Style(Name = "paddingBottom", Type = typeof(int), Default = 16)]

    [Style(Name = "headingLabelStyle", Type = typeof(GUIStyle), ProxyType = typeof(FormHeadingStyle))]
    [Style(Name = "itemLabelStyle", Type = typeof(GUIStyle), ProxyType = typeof(FormItemLabelStyle))]
    [Style(Name = "buttonStyle", Type = typeof(GUIStyle), ProxyType = typeof(ButtonStyle))]

    public class Form : Group
    {
        // ReSharper disable InconsistentNaming
        /// <summary>
        /// Submit
        /// </summary>
        public const string SUBMIT = "formSubmit";
        // ReSharper restore InconsistentNaming

        /// <summary>
        /// The collection of form adapters (add yours here)
        /// </summary>
        public static Dictionary<Type, IFormAdapter> Adapters
        {
             get
             {
                 var dict = new Dictionary<Type, IFormAdapter>
                    {
                        {typeof (TextField), new TextFieldFormAdapter()},
                        {typeof (TextArea), new TextAreaFormAdapter()},
                        {typeof (List), new ListFormAdapter()},
                        {typeof (DropDownList), new DropDownListFormAdapter()},
                        {typeof (ComboBox), new ComboBoxFormAdapter()},
                        {typeof (Label), new LabelFormAdapter()},
                        {typeof (Button), new ButtonFormAdapter()},
                        {typeof (CheckBox), new CheckBoxFormAdapter()},
                        {typeof (NumericStepper), new NumericStepperAdapter()}
                    };
                 return dict;
             } 
        }

        private MulticastDelegate _submitHandler;
        /// <summary>
        /// Fires on form submit
        /// </summary>
        public MulticastDelegate SubmitHandler
        {
            get
            {
                if (null == _submitHandler)
                    _submitHandler = new MulticastDelegate(this, SUBMIT);
                return _submitHandler;
            }
            set
            {
                _submitHandler = value;
            }
        }

        private float _labelWidth = 150;
        private bool _labelWidthChanged;
        /// <summary>
        /// Label width
        /// </summary>
        public float LabelWidth
        {
            get
            {
                return _labelWidth;
            }
            set
            {
                _labelWidth = value;
                _labelWidthChanged = true;
                InvalidateProperties();
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Form()
        {
            ShowOverlay = false;
            //ProcessKeys = true;
            MinWidth = 100;
            MinHeight = 100;
            FocusEnabled = true;
            CircularTabs = true;
            CircularArrows = false;

            Layout = new VerticalLayout
            {
                HorizontalAlign = HorizontalAlign.Left,
                VerticalAlign = VerticalAlign.Top,
                Gap = 10
            };

            Plugins.Add(new TabManager());
        }

        public override void SetFocus()
        {
            InteractiveComponent firstFocusableItem = Children.Find(delegate(DisplayListMember control)
            {
                return ((InteractiveComponent)control).FocusEnabled;
            }) as InteractiveComponent;

            //Debug.Log("firstFocusableItem == null: " + (firstFocusableItem == null));

            if (null != firstFocusableItem)
                firstFocusableItem.SetFocus();
            else
                base.SetFocus();
        }

        protected override void CreationComplete()
        {
            base.CreationComplete();

            AddEventListener(ButtonEvent.PRESS, OnButton);
        }

        public void Submit()
        {
            // validate
            DispatchEvent(new GuiEvent(SUBMIT));
        }

        public virtual void Reset()
        {
            // override this
        }

        private readonly Dictionary<string, Component> _dictFields = new Dictionary<string, Component>();
        private readonly Dictionary<string, Group> _dictItems = new Dictionary<string, Group>();

        /// <summary>
        /// Adds the component to a form<br/>
        /// Registers th efield using the supplied ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="label"></param>
        /// <param name="control"></param>
        /// <param name="itemLayout"></param>
        /// <exception cref="Exception"></exception>
        public void AddField(string id, string label, Component control, LayoutBase itemLayout = null)
        {
            if (_dictFields.ContainsKey(id))
                throw new Exception("Duplicated form element ID: " + id);

            _dictFields.Add(id, control);

            Group itemGroup = new Group
            {
                PercentWidth = 100,
                Layout = itemLayout ?? new VerticalLayout() // TODO: different item layouts
            };

            /*_fieldGroup.*/AddChild(itemGroup);

            if (_dictItems.ContainsKey(id))
                throw new Exception("Duplicated form element ID: " + id);
            
            _dictItems.Add(id, itemGroup);

            // 1) label
            Label lbl = new Label { Text = label, Width = LabelWidth };
            lbl.SetStyle("labelStyle", GetStyle("itemLabelStyle"));
            itemGroup.AddChild(lbl);

            // 2) control
            if (null != control)
            {
                itemGroup.AddChild(control);
            }
            else
                lbl.PercentWidth = 100;
        }

// ReSharper disable VirtualMemberNeverOverriden.Global
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnButton(Event e)
// ReSharper restore VirtualMemberNeverOverriden.Global
        {
            ButtonEvent be = (ButtonEvent)e;
            if (be.ButtonId == "submit") // the default button ID for submittion
            {
                //Debug.Log("Submitting form...");
                Submit();
            }
        }

        /*public override DisplayListMember RemoveChild(DisplayListMember child)
        {
            FormItem fi = (FormItem)child;
            fi.Dispose();

            base.RemoveChild(child);

            return child;
        }*/

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_labelWidthChanged)
            {
                _labelWidthChanged = false;
                ContentChildren.ForEach(delegate(DisplayListMember child)
                {
                    FormItem fi = child as FormItem;
                    if (null != fi)
                        fi.LabelWidth = LabelWidth;
                });
            }
        }

        override public List<DisplayListMember> GetTabChildren()
        {
            List<DisplayListMember> list = new List<DisplayListMember>();

            /*_fieldGroup.*/Children.ForEach(delegate(DisplayListMember child) // form items
            {
                Group itemGroup = child as Group; // form item
                if (null != itemGroup)
                {
                    //Debug.Log("list: " + list.Count);
                    list.AddRange(
                        itemGroup.ContentChildren.FindAll(delegate(DisplayListMember child2)
                        {
                            InteractiveComponent c = child2 as InteractiveComponent;
                            return null != c && c.Enabled && c.Visible && c.FocusEnabled;
                        })
                    );
                }
            });

            return list;
        }

        /// <summary>
        /// Gets or sets a form data
        /// </summary>
        public new Hashtable Data
        {
            get
            {
                // sort by ID
                List<string> list = new List<string>(_dictFields.Keys);
                list.Sort();

                Hashtable data = new Hashtable();
                foreach (string key in list)
                {
                    Component control = _dictFields[key];
                    //if (control is TextField)
                    //{
                        // ((TextField)control).Text;
                    //}
                    //Type type = control.GetType();
                    var adapter = GetAdapter(control);
                    if (null == adapter)
                    {
                        Debug.LogWarning("Unknown form adapter for: " + control.GetType());
                        continue;
                    }
                    data[key] = adapter.GetValue(control); 
                }
                return data;
            }
            set
            {
                foreach (DictionaryEntry entry in value)
                {
                    string key = entry.Key.ToString();
                    if (!_dictFields.ContainsKey(key))
                    {
                        Debug.LogWarning(string.Format(@"Form: Key ""{0}"" not found", key));
                        continue;
                    }

                    Component control = _dictFields[key];
                    //if (control is TextField)
                    //{
                    //    ((TextField)control).Text = entry.Value.ToString();
                    //}
                    //Type type = control.GetType();
                    var adapter = GetAdapter(control);
                    if (null == adapter)
                    {
                        Debug.LogWarning("Unknown form adapter for: " + control.GetType());
                        continue;
                    }
                    adapter.SetValue(control, entry.Value); 
                }

                // reset other
                foreach (string key in _dictFields.Keys)
                {
                    if (!value.ContainsKey(key))
                    {
                        Component control = _dictFields[key];
                        var adapter = GetAdapter(control);
                        if (null != adapter)
                        {
                            adapter.Reset(control);
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Gets the appropriate form adapter for the supplied component
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static IFormAdapter GetAdapter(Component component)
        {
            Type type = component.GetType();
            if (!Adapters.ContainsKey(type))
            {
                Debug.LogWarning("Unknown form adapter for: " + component.GetType());
                return null;
            }
            return Adapters[type];
        }

        /// <summary>
        /// Gets the item group<br/>
        /// The item reference could be used for turning it ON and OFF
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Group GetItem(string id)
        {
            return _dictItems[id];
        }

        /// <summary>
        /// Toggles the group visibility
        /// </summary>
        /// <param name="id"></param>
        /// <param name="visible"></param>
        public void ToggleItem(string id, bool? visible = null)
        {
            var itemGroup = GetItem(id);
            itemGroup.Visible = itemGroup.IncludeInLayout = visible ?? !itemGroup.Visible;
        }
    }
}