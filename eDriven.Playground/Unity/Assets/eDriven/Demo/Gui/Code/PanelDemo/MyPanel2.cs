using System.Collections.Generic;
using Assets.eDriven.Skins;
using eDriven.Core.Caching;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Layout;

namespace Assets.eDriven.Demo.Gui.Code.PanelDemo
{
    public class MyPanel2 : Panel
    {
        public MyPanel2()
        {
            Id = "ct";
            Title = "This is a panel";
            Layout = new VerticalLayout
                         {
                             PaddingLeft = 6,
                             PaddingRight = 6,
                             PaddingTop = 6,
                             PaddingBottom = 6,
                             Gap = 6
                         };
        }

        override protected void CreateChildren()
        {
            base.CreateChildren();

            Button button;

            for (int i = 0; i < 14; i++)
            {
                button = new Button
                             {
                                 Text = "Button " + (i + 1),
                                 SkinClass = typeof(ImageButtonSkin),
                                 FocusEnabled = false,
                                 Icon = ImageLoader.Instance.Load("Icons/shape_square_add"),
                                 Width = 400,
                                 Height = 200
                             };
                AddContentChild(button);
            }

            /*List<string> icons = new List<string> { "maximize", "cancel" };
            for (int i = 0; i < icons.Count; i++)
            {
                button = new Button
                             {
                                 FocusEnabled = false,
                                 SkinClass = typeof(ImageButtonSkin),
                                 Icon = ImageLoader.Instance.Load("Icons/" + icons[i])
                             };
                button.Click += delegate(Event e) { Debug.Log("Clicked: " + e.Target); };
                ToolGroup.AddChild(button);
            }*/

            button = new Button
            {
                Text = "Toggle",
                SkinClass = typeof(ImageButtonSkin),
                Icon = ImageLoader.Instance.Load("Icons/color_swatch"),
                ToggleMode = true,
                FocusEnabled = false,
                Selected = true
            };
            ControlBarGroup.AddChild(button);

            List<string> icons = new List<string> { "lock", "star", "accept" };
            for (int i = 0; i < 3; i++)
            {
                button = new Button
                             {
                                 Text = UppercaseFirst(icons[i]),
                                 SkinClass = typeof(ImageButtonSkin),
                                 FocusEnabled = false,
                                 Icon = ImageLoader.Instance.Load("Icons/" + icons[i])
                             };
                ControlBarGroup.AddChild(button);
            }
        }

        static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}