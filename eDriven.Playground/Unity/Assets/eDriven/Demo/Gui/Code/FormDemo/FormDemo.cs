using System.Collections;
using System.Collections.Generic;
using System.Text;
using Assets.eDriven.Demo._shared.Code.Util;
using Assets.eDriven.Skins;
using eDriven.Core.Caching;
using eDriven.Gui;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Form;
using eDriven.Gui.Plugins;
using Assets.eDriven.Demo.Components;
using UnityEngine;

public class FormDemo : Gui
{
    #region Dummy text

    private const string LoremIpsum = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. 
Aliquam non urna purus. Suspendisse tincidunt scelerisque euismod. 
Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. 
Praesent ipsum elit, consectetur ac scelerisque vitae, rhoncus porta nulla. 
In semper placerat sem nec consectetur. 
Donec mi arcu, tristique at viverra eget, accumsan at erat. 
Nulla ut ligula nibh, sit amet consequat neque. 
Aliquam a turpis sem, at dictum leo.";

    #endregion

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
                "Form Demo 1",
                "Created with eDriven.Gui"/*,
                "Author: Danko Kozar"*/
            },
            Callback = delegate(string line) { label.Text = line; }
        }
        .Start();

        #endregion

        Panel panel = new Panel
        {
            Title = "Form Demo",
            Icon = Resources.Load<Texture>("Icons/star"),
            SkinClass = typeof(PanelSkin2),
            //SkinClass = typeof(WindowSkin),
            Width = 350,
            MaxHeight = 500,
            HorizontalCenter = 0, VerticalCenter = 0,
            //Top = 100, Bottom = 100 // additional constrains for screen resize
        };
        AddChild(panel);

        Group container = new Group { Left = 10, Right = 10, Top = 10, Bottom = 10 };
        panel.AddContentChild(container);

        Form form = new Form { PercentWidth = 100 };
        container.AddContentChild(form);

        #region Text Fields

        TextField txtSubject = new TextField
                                   {
                                       FocusEnabled = true,
                                       PercentWidth = 100,
                                       Text = "Input text",
                                       //Optimized = true
                                       //AlowedCharacters = "a1"
                                   };
        form.AddField("subject", "Subject:", txtSubject);

        TextArea txtMessage = new TextArea
                       {
                           FocusEnabled = true,
                           PercentWidth = 100,
                           Height = 200,
                           Text = LoremIpsum,
                           //Optimized = true
                       };
        form.AddField("message", "Message:", txtMessage);

        #endregion

        #region Buttons

        panel.ControlBarGroup.AddChild(new Spacer {PercentWidth = 100});

        Button btnSet = new Button
                       {
                           Text = "Set data",
                           Icon = ImageLoader.Instance.Load("Icons/arrow_up"),
                           SkinClass = typeof(ImageButtonSkin)
                       };
        btnSet.Press += delegate
        {
            form.Data = new Hashtable
                            {
                                {"subject", "The subject"},
                                {"message", "This is the message..."}
                            };
        };
        panel.ControlBarGroup.AddChild(btnSet);
        btnSet.SetFocus();

        Button btnGet = new Button
        {
            Text = "Get data",
            SkinClass = typeof(ImageButtonSkin),
            Icon = ImageLoader.Instance.Load("Icons/arrow_down")
        };
        btnGet.Press += delegate
        {
            StringBuilder sb = new StringBuilder();
            foreach (DictionaryEntry entry in form.Data)
            {
                sb.AppendLine(string.Format(@"""{0}"": {1}", entry.Key, entry.Value));
                sb.AppendLine();
            }

            Alert.Show("This is the form data", sb.ToString(), AlertButtonFlag.Ok,
                new AlertOption(AlertOptionType.HeaderIcon, Resources.Load<Texture>("Icons/information")));
        };
        panel.ControlBarGroup.AddChild(btnGet);

        #endregion

        panel.Plugins.Add(new TabManager
        {
            TabChildren = new List<DisplayListMember> { txtSubject, txtMessage, btnSet, btnGet }
        });
    }
}