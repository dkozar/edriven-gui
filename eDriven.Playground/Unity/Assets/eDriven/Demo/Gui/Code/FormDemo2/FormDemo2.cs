using System.Collections;
using System.Collections.Generic;
using System.Text;
using Assets.eDriven.Demo._shared.Code.Util;
using Assets.eDriven.Skins;
using eDriven.Core.Caching;
using eDriven.Core.Data.Collections;
using eDriven.Gui;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Form;
using eDriven.Gui.Layout;
using eDriven.Gui.Plugins;
using Assets.eDriven.Demo.Components;
using UnityEngine;
using ArrayList = eDriven.Gui.Data.ArrayList;

public class FormDemo2 : Gui
{
    override protected void CreateChildren()
    {
        base.CreateChildren();

        #region Top label

        Label label = new TitleLabel { HorizontalCenter = 0, Top = 20, StyleName = "title" };
        AddChild(label);

        new TextRotator
        {
            Delay = 5, // 5 seconds delay
            Lines = new[]
            {
                "Form Demo 2",
                "Created with eDriven.Gui"/*,
                "Author: Danko Kozar"*/
            },
            Callback = delegate(string line) { label.Text = line; }
        }
        .Start();

        #endregion

        #region Scroller

        Scroller scroller = new Scroller
        {
            SkinClass = typeof(ScrollerSkin2),
            Left = 0,
            Right = 0,
            Top = 100,
            Bottom = 0
        };
        //scroller.SetStyle("horizontalScrollPolicy", ScrollPolicy.On);
        //scroller.SetStyle("verticalScrollPolicy", ScrollPolicy.On);
        AddChild(scroller);

        Group viewport = new Group
        {
            MouseEnabled = true,
            Layout = new VerticalLayout
            {
                HorizontalAlign = HorizontalAlign.Center,
                VerticalAlign = VerticalAlign.Middle,
                PaddingLeft = 10,
                PaddingRight = 10,
                PaddingTop = 0,
                PaddingBottom = 10,
                Gap = 10
            }
        };
        scroller.Viewport = viewport;

        #endregion

        Panel panel = new Panel
        {
            Title = "Form Demo 2",
            Icon = Resources.Load<Texture>("Icons/star"),
            SkinClass = typeof(PanelSkin2),
            MaxHeight = 800,
            //Height = 600 // for getting a scrollbar
        };
        viewport.AddChild(panel);

        Group container = new Group { Left = 10, Right = 10, Top = 10, Bottom = 10 };
        panel.AddContentChild(container);

        Form form = new Form {PercentWidth = 100};
        container.AddContentChild(form);

        #region Form items

        List list = new List
        {
            PercentWidth = 100,
            RequireSelection = true,
            SelectedItem = "Sine",
            DataProvider = new ArrayList(new List<object>
                                               {
                                                   new ListItem("Back", "Back"),
                                                   new ListItem("Bounce", "Bounce"),
                                                   new ListItem("Circ", "Circ"),
                                                   new ListItem("Cubic", "Cubic"),
                                                   new ListItem("Elastic", "Elastic"),
                                                   new ListItem("Expo", "Expo"),
                                                   new ListItem("Linear", "Linear"),
                                                   new ListItem("Quad", "Quad"),
                                                   new ListItem("Quart", "Quart"),
                                                   new ListItem("Quint", "Quint"),
                                                   new ListItem("Sine", "Sine")
                                               })
        };
        form.AddField("list", "List:", list);

        DropDownList dropDown = new DropDownList
        {
            PercentWidth = 100,
            DataProvider = new ArrayList(new List<object>
            {
                "Failure", "Teaches", "Success", "One", "Two", "Three", "Four", "Five", "Six"
            })
        };
        form.AddField("dd", "Drop down list:", dropDown);

        #region _test

//cb.Opening += delegate (Event evt)
        //                  {
        //                      Debug.Log("Opening");
        //                      //evt.PreventDefault();
        //                  };
        //cb.Closing += delegate(Event evt) { 
        //    Debug.Log("Closing");
        //    //evt.PreventDefault();
        //};
        //cb.Open += delegate { Debug.Log("Open"); };
        //cb.Close += delegate { Debug.Log("Close"); };

        //cb.AddEventListener(IndexChangeEvent.CHANGE, delegate (Event e)
        //                                     {
        //                                         IndexChangeEvent ice = (IndexChangeEvent)e;
        //                                         Debug.Log("Index changed from " + ice.OldIndex + " to " + ice.Index);
        //                                     });
        //cb.SelectedIndexChanged += delegate(Event e)
        //                                    {
        //                                        IndexChangeEvent ice = (IndexChangeEvent)e;
        //                                        Debug.Log("Index changed from " + ice.OldIndex + " to " + ice.Index);
        //                                    };

        #endregion

        ComboBox cb1 = new ComboBox
        {
            PercentWidth = 100,
            DataProvider = new ArrayList(new List<object>
                                                 {
                                                     new ListItem("Failure", "Failure"),
                                                     new ListItem("Teaches", "Teaches"),
                                                     new ListItem("Success", "Success")
                                                 })
        };
        form.AddField("combo1", "Combo 1:", cb1);

        ComboBox cb2 = new ComboBox
        {
            PercentWidth = 100,
            DataProvider = new ArrayList(new List<object>
                                                 {
                                                     new ListItem("Back", "Back"),
                                                     new ListItem("Bounce", "Bounce"),
                                                     new ListItem("Circ", "Circ"),
                                                     new ListItem("Cubic", "Cubic"),
                                                     new ListItem("Elastic", "Elastic"),
                                                     new ListItem("Expo", "Expo"),
                                                     new ListItem("Linear", "Linear"),
                                                     new ListItem("Quad", "Quad"),
                                                     new ListItem("Quart", "Quart"),
                                                     new ListItem("Quint", "Quint"),
                                                     new ListItem("Sine", "Sine")
                                                 })
        };
        form.AddField("combo2", "Combo 2:", cb2);

        TextField txtSubject = new TextField
                                   {
                                       FocusEnabled = true,
                                       PercentWidth = 100,
                                       Optimized = true,
                                       //PasswordMode = true // test password mode
                                   };
        form.AddField("subject", "Subject:", txtSubject);

        CheckBox chk1 = new CheckBox { Text = "Checkbox 1" };
        chk1.Change += delegate
        {
            form.ToggleItem("checkbox2");
        };
        form.AddField("checkbox", "Checkbox:", chk1);

        CheckBox chk2 = new CheckBox { Text = "Checkbox 2" };
        chk2.Change += delegate
        {
            form.ToggleItem("checkbox3");
        };
        form.AddField("checkbox2", "Checkbox:", chk2);
        form.ToggleItem("checkbox2", false);

        CheckBox chk3 = new CheckBox { Text = "Checkbox 3 with long text......." };
        form.AddField("checkbox3", "Checkbox:", chk3);
        form.ToggleItem("checkbox3", false);

        #endregion

        #region Buttons

        Button btnSet = new Button
                            {
                                Text = "Set data (true/Expo/Expo)",
                                Icon = ImageLoader.Instance.Load("Icons/arrow_up"),
                                SkinClass = typeof(ImageButtonSkin)
                            };
        btnSet.Press += delegate
                            {
                                form.Data = new Hashtable
                                                {
                                                    {"subject", "The subject"},
                                                    //{"message", "This is the message..."}
                                                    {"checkbox", true},
                                                    {"combo1", "Expo"},
                                                    {"list", "Expo"}
                                                };
                            };
        panel.ControlBarGroup.AddChild(btnSet);
        
        Button btnSet2 = new Button
        {
            Text = "Set data (false/Circ/Sine)",
            SkinClass = typeof(ImageButtonSkin),
            Icon = ImageLoader.Instance.Load("Icons/arrow_up")
        };
        btnSet2.Press += delegate
        {
            form.Data = new Hashtable
                                                {
                                                    {"subject", "The subject 2"},
                                                    //{"message", "This is the message..."}
                                                    {"checkbox", false},
                                                    {"combo1", "Circ"},
                                                    {"list", "Sine"}
                                                };
        };
        panel.ControlBarGroup.AddChild(btnSet2);
        
        panel.ControlBarGroup.AddChild(new Spacer {PercentWidth = 100});

        Button btnGet = new Button
                            {
                                Text = "Get data",
                                SkinClass = typeof(ImageButtonSkin),
                                Icon = ImageLoader.Instance.Load("Icons/arrow_down")
                            };
        btnGet.Press += delegate
                            {
                                StringBuilder sb = new StringBuilder();
                                int count = form.Data.Count;
                                int index = 0;
                                foreach (DictionaryEntry entry in form.Data)
                                {
                                    if (index < count - 1)
                                        sb.AppendLine(string.Format(@"[{0}]: {1}", entry.Key, entry.Value));
                                    else
                                        sb.Append(string.Format(@"[{0}]: {1}", entry.Key, entry.Value));
                                    //sb.AppendLine();
                                    index++;
                                }

                                Alert.Show("This is the form data", sb.ToString(), AlertButtonFlag.Ok,
                                    new AlertOption(AlertOptionType.HeaderIcon, Resources.Load<Texture>("Icons/information")));
                            };
        panel.ControlBarGroup.AddChild(btnGet);

        #endregion

        // focus
        list.SetFocus();

        panel.Plugins.Add(new TabManager
        {
            TabChildren = new List<DisplayListMember> { list, dropDown, cb1, cb2, txtSubject, chk1, chk2, chk3, btnSet, btnGet }
        });
    }
}